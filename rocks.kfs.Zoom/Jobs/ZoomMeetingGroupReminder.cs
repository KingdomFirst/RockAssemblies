﻿// <copyright>
// Copyright 2022 by Kingdom First Solutions
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//
using com.bemaservices.RoomManagement.Model;
using Quartz;
using Rock;
using Rock.Attribute;
using Rock.Communication;
using Rock.Data;
using Rock.Jobs;
using Rock.Logging;
using Rock.Model;
using Rock.Web.Cache;
using rocks.kfs.Zoom.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Web;

namespace rocks.kfs.Zoom.Jobs
{
    /// <summary>
    /// Job to process communications
    /// </summary>
    [DisplayName( "Send Zoom Meeting Reminders" )]
    [Description( "Sends a reminder to members of a group connected with a room reservation that has a Zoom meeting attached to it." )]

    #region Job Attributes
    [SystemCommunicationField(
        "System Communication",
        Description = "The system communication to use when sending Zoom meeting reminders.",
        DefaultValue = ZoomGuid.SystemComunication.ZOOM_MEETING_REMINDER,
        IsRequired = true,
        Order = 1,
        Key = AttributeKey.SystemCommunication )]

    [TextField(
        "Days Prior",
        Description = "Comma delimited list of days prior to a scheduled Zoom meeting to send a reminder. For example, a value of '2,4' would result in reminders getting sent two and four days prior to the Zoom meeting's scheduled meeting date.",
        DefaultValue = "1",
        IsRequired = true,
        Order = 2,
        Key = AttributeKey.DaysPrior )]

    [AttributeField(
        "Reminder Group Attribute",
        EntityTypeGuid = com.bemaservices.RoomManagement.SystemGuid.EntityType.RESERVATION,
        Description = "The \"Group Type Group\" type attribute on the Room Reservation entity to be used for sending reminders. This attribute is what connects a Room Reservation to a Group for Zoom meeting purposes.",
        DefaultValue = ZoomGuid.Attribute.ROOM_RESERVATION_GROUP_ATTRIBUTE,
        IsRequired = true,
        Order = 3,
        Key = AttributeKey.GroupAttributeSetting )]

    [CustomDropdownListField(
        "Send Using",
        Description = "Specifies how the reminder will be sent.",
        ListSource = "1^Email,2^SMS,0^Recipient Preference",
        IsRequired = true,
        DefaultValue = "1",
        Order = 4,
        Key = AttributeKey.SendUsing )]

    #endregion Job Attributes

    [DisallowConcurrentExecution]
    public class ZoomMeetingGroupReminder : RockJob
    {
        /// <summary>
        /// Attribute Keys
        /// </summary>
        private static class AttributeKey
        {
            public const string SystemCommunication = "SystemCommunication";
            public const string DaysPrior = "DaysPrior";
            public const string GroupAttributeSetting = "GroupAttributeSetting";
            public const string SendUsing = "SendUsing";
        }

        private int errorCount = 0;
        private int notificationEmails = 0;
        private int notificationSms = 0;
        private int notificationPush = 0;
        private int? reservationLocationEntityTypeId;
        private AttributeCache resGroupAttribute;

        private List<string> errorMessages = new List<string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ZoomRoomSchedulingAndMaintenance"/> class.
        /// </summary>
        public ZoomMeetingGroupReminder()
        {
        }

        /// <summary>
        /// Executes the specified context.
        /// </summary>
        public override void Execute()
        {
            var rockContext = new RockContext();
            resGroupAttribute = AttributeCache.Get( GetAttributeValue( AttributeKey.GroupAttributeSetting ).AsGuid() );
            reservationLocationEntityTypeId = new EntityTypeService( rockContext ).GetNoTracking( com.bemaservices.RoomManagement.SystemGuid.EntityType.RESERVATION_LOCATION.AsGuid() ).Id;

            Result = "0 meeting reminders sent.";

            if ( resGroupAttribute == null )
            {
                var errorMessages = new List<string>
                {
                    $"The Reservation Group Attribute job setting is invalid. Please check your Room Registration and KFS Zoom Room plugin configuration."
                };
                HandleErrorMessages( errorMessages );
            }

            var systemCommunicationGuid = GetAttributeValue( AttributeKey.SystemCommunication ).AsGuid();
            var systemCommunication = new SystemCommunicationService( rockContext ).Get( systemCommunicationGuid );

            var jobPreferredCommunicationType = ( CommunicationType ) GetAttributeValue( AttributeKey.SendUsing ).AsInteger();
            var isSmsEnabled = MediumContainer.HasActiveSmsTransport() && !string.IsNullOrWhiteSpace( systemCommunication.SMSMessage );
            var isPushEnabled = MediumContainer.HasActivePushTransport() && !string.IsNullOrWhiteSpace( systemCommunication.PushData );

            if ( jobPreferredCommunicationType == CommunicationType.SMS && !isSmsEnabled )
            {
                // If sms selected but not usable default to email.
                var errorMessages = new List<string>
                {
                    $"The job is setup to send via SMS but either SMS isn't enabled or no SMS message was found in system communication {systemCommunication.Title}."
                };
                HandleErrorMessages( errorMessages );
            }

            if ( jobPreferredCommunicationType == CommunicationType.PushNotification && !isPushEnabled )
            {
                // If push notification selected but not usable default to email.
                var errorMessages = new List<string>
                {
                    $"The job is setup to send via Push Notification but either Push Notifications are not enabled or no Push message was found in system communication {systemCommunication.Title}."
                };
                HandleErrorMessages( errorMessages );
            }

            var results = new StringBuilder();
            if ( jobPreferredCommunicationType == CommunicationType.SMS && string.IsNullOrWhiteSpace( systemCommunication.SMSMessage ) )
            {
                var warning = $"No SMS message found in system communication {systemCommunication.Title}. All Zoom meeting reminders will be attempted via email.";
                results.AppendLine( warning );
                RockLogger.Log.Warning( RockLogDomains.Jobs, warning );
                jobPreferredCommunicationType = CommunicationType.Email;
            }

            if ( jobPreferredCommunicationType == CommunicationType.PushNotification && string.IsNullOrWhiteSpace( systemCommunication.PushMessage ) )
            {
                var warning = $"No Push message found in system communication {systemCommunication.Title}. All Zoom meeting reminders will be attempted via email.";
                results.AppendLine( warning );
                RockLogger.Log.Warning( RockLogDomains.Jobs, warning );
                jobPreferredCommunicationType = CommunicationType.Email;
            }

            // Get upcoming Zoom Room occurrences
            var roomOccurrenceInfo = GetOccurenceAndGroupData( rockContext );

            // Process reminders
            var meetingRemindersResults = SendMeetingReminders( rockContext, roomOccurrenceInfo, systemCommunication, jobPreferredCommunicationType, isSmsEnabled, isPushEnabled );

            results.AppendLine( $"{notificationEmails + notificationSms + notificationPush } meeting reminders sent." );

            results.AppendLine( string.Format( "- {0} email(s)\n- {1} SMS message(s)\n- {2} push notification(s)", notificationEmails, notificationSms, notificationPush ) );

            results.Append( FormatWarningMessages( meetingRemindersResults.Warnings ) );

            Result = results.ToString();
            HandleErrorMessages( meetingRemindersResults.Errors );
        }

        /// <summary>
        /// Gets the room occurrence and group data.
        /// </summary>
        /// <param name="dataMap">The data map.</param>
        /// <param name="rockContext">The rock context.</param>
        /// <returns></returns>
        private Dictionary<RoomOccurrence, Group> GetOccurenceAndGroupData( RockContext rockContext )
        {
            var result = new Dictionary<RoomOccurrence, Group>();
            var dates = GetSearchDates();
            var startDate = dates.Min();

            var zRoomOccurrenceService = new RoomOccurrenceService( rockContext );
            var reservationLocationEntityTypeId = new EntityTypeService( rockContext ).GetNoTracking( com.bemaservices.RoomManagement.SystemGuid.EntityType.RESERVATION_LOCATION.AsGuid() ).Id;

            // Find all RoomOccurrences that occur on the targeted dates
            var occurrences = new Dictionary<int, List<DateTime>>();
            var occurrencesToRemind = zRoomOccurrenceService
                .Queryable().AsNoTracking()
                .Where( ro => ro.EntityTypeId == reservationLocationEntityTypeId
                    && ro.ZoomMeetingId > 0
                    && dates.Contains( DbFunctions.TruncateTime( ro.StartTime ).Value ) )
                .ToList();

            // Find all Reservation Locations tied to the occurrences that need reminder sent.
            var occurrenceEntityIds = occurrencesToRemind.Select( o => o.EntityId ).ToList();
            var groupService = new GroupService( rockContext );
            var reservationLocationService = new ReservationLocationService( rockContext );
            var resLocationsToRemind = reservationLocationService.Queryable( "Reservation" )
                                                                .AsNoTracking()
                                                                .Where( rl => occurrenceEntityIds.Contains( rl.Id ) )
                                                                .ToList();
            foreach ( var roomOcc in occurrencesToRemind )
            {
                var resLoc = resLocationsToRemind.FirstOrDefault( rl => rl.Id == roomOcc.EntityId );
                resLoc.Reservation.LoadAttributes();
                var groupAttributeValue = resLoc.Reservation.GetAttributeValue( resGroupAttribute.Key );
                if ( !string.IsNullOrWhiteSpace( groupAttributeValue ) )
                {
                    var groupGuid = groupAttributeValue.Split( '|' )[1].AsGuid();
                    var group = groupService.Get( groupGuid );
                    if ( group != null )
                    {
                        result.Add( roomOcc, group );
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Gets the search dates.
        /// </summary>
        /// <returns></returns>
        private List<DateTime> GetSearchDates()
        {
            // Get the occurrence dates that apply
            var dates = new List<DateTime>();

            try
            {
                var reminderDays = GetAttributeValue( AttributeKey.DaysPrior ).Split( ',' );
                foreach ( string reminderDay in reminderDays )
                {
                    if ( reminderDay.Trim().IsNotNullOrWhiteSpace() )
                    {
                        var reminderDate = RockDateTime.Today.AddDays( Convert.ToInt32( reminderDay ) );
                        if ( !dates.Contains( reminderDate ) )
                        {
                            dates.Add( reminderDate );
                        }
                    }
                }
            }
            catch
            {
                // if an exception occurs just use today's date.
                dates.Add( RockDateTime.Today );
            }

            return dates;
        }

        /// <summary>
        /// Sends the meeting reminders.
        /// </summary>
        /// <param name="context">The overall job context.</param>
        /// <param name="rockContext">The rockContext.</param>
        /// <param name="occurrenceData">The occurrenceData to process.</param>
        /// <param name="systemCommunication">The system communication.</param>
        /// <param name="jobPreferredCommunicationType">Type of the job preferred communication.</param>
        /// <returns></returns>
        private SendMessageResult SendMeetingReminders( RockContext rockContext,
                        Dictionary<RoomOccurrence, Group> occurrenceData,
                        SystemCommunication systemCommunication,
                        CommunicationType jobPreferredCommunicationType,
                        bool isSmsEnabled,
                        bool isPushEnabled )
        {
            var result = new SendMessageResult();
            var errorsEmail = new List<string>();
            var errorsSms = new List<string>();
            var errorsPush = new List<string>();

            // Loop through the room occurrence data
            foreach ( var occurrence in occurrenceData )
            {
                var emailMessage = new RockEmailMessage( systemCommunication );
                RockSMSMessage smsMessage = isSmsEnabled ? new RockSMSMessage( systemCommunication ) : null;
                RockPushMessage pushMessage = isPushEnabled ? new RockPushMessage( systemCommunication ) : null;
                var group = occurrence.Value;
                foreach ( var groupMember in group.ActiveMembers().ToList() )
                {
                    groupMember.Person.LoadAttributes();
                    var smsNumber = groupMember.Person.PhoneNumbers.GetFirstSmsNumber();
                    var personAlias = new PersonAliasService( rockContext ).Get( groupMember.Person.PrimaryAliasId.Value );
                    List<string> pushDevices = new PersonalDeviceService( rockContext ).Queryable()
                        .Where( a => a.PersonAliasId.HasValue && a.PersonAliasId == personAlias.Id && a.NotificationsEnabled )
                        .Select( a => a.DeviceRegistrationId )
                        .ToList();
                    if ( !groupMember.Person.CanReceiveEmail( false ) && smsNumber.IsNullOrWhiteSpace() && pushDevices.Count == 0 )
                    {
                        continue;
                    }
                    var mergeFields = Rock.Lava.LavaHelper.GetCommonMergeFields( null, groupMember.Person );
                    mergeFields.Add( "Group", group );
                    mergeFields.Add( "Occurrence", occurrence.Key );
                    mergeFields.Add( "Person", groupMember.Person );

                    var notificationType = ( CommunicationType ) Communication.DetermineMediumEntityTypeId(
                                        ( int ) CommunicationType.Email,
                                        ( int ) CommunicationType.SMS,
                                        ( int ) CommunicationType.PushNotification,
                                        jobPreferredCommunicationType,
                                        groupMember.CommunicationPreference,
                                        groupMember.Person.CommunicationPreference );

                    switch ( notificationType )
                    {
                        case CommunicationType.Email:
                            if ( !groupMember.Person.CanReceiveEmail( false ) )
                            {
                                errorCount += 1;
                                errorMessages.Add( string.Format( "{0} does not have a valid email address.", groupMember.Person.FullName ) );
                            }
                            else
                            {
                                emailMessage.AddRecipient( new RockEmailMessageRecipient( groupMember.Person, mergeFields ) );
                            }
                            break;
                        case CommunicationType.SMS:
                            if ( string.IsNullOrWhiteSpace( smsNumber ) || smsMessage == null )
                            {
                                errorCount += 1;
                                errorMessages.Add( string.Format( "No SMS number could be found for {0}.", groupMember.Person.FullName ) );
                                goto case CommunicationType.Email;
                            }
                            else
                            {
                                smsMessage.AddRecipient( new RockSMSMessageRecipient( groupMember.Person, smsNumber, mergeFields ) );
                            }
                            break;
                        case CommunicationType.PushNotification:
                            if ( pushDevices.Count == 0 || pushMessage == null )
                            {
                                errorCount += 1;
                                errorMessages.Add( string.Format( "No devices that support notifications could be found for {0}.", groupMember.Person.FullName ) );
                                goto case CommunicationType.Email;
                    }
                            else
                            {
                                string deviceIds = String.Join( ",", pushDevices );
                                pushMessage.AddRecipient( new RockPushMessageRecipient( groupMember.Person, deviceIds, mergeFields ) );
                            }
                            break;
                        default:
                            break;
                    }
                }

                if ( emailMessage.GetRecipients().Count > 0 )
                {
                    emailMessage.Send( out errorsEmail );
                    if ( errorsEmail.Any() )
                    {
                        result.Errors.AddRange( errorsEmail );
                    }
                    else
                    {
                        notificationEmails++;
                    }
                }
                if ( smsMessage != null && smsMessage.GetRecipients().Count > 0 )
                {
                    smsMessage.Send( out errorsSms );
                    if ( errorsSms.Any() )
                    {
                        result.Errors.AddRange( errorsSms );
                    }
                    else
                    {
                        notificationSms++;
                    }
                }
                if ( pushMessage != null && pushMessage.GetRecipients().Count > 0 )
                {
                    pushMessage.Send( out errorsPush );
                    if ( errorsPush.Any() )
                    {
                        result.Errors.AddRange( errorsPush );
                    }
                    else
                    {
                        notificationPush++;
                    }
                }
            }
            if ( errorMessages.Any() )
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine();
                sb.Append( string.Format( "{0} Errors: ", errorCount ) );
                errorMessages.ForEach( e => { sb.AppendLine(); sb.Append( e ); } );
                string errors = sb.ToString();
                Result += errors;
                var exception = new Exception( errors );
                HttpContext context2 = HttpContext.Current;
                ExceptionLogService.LogException( exception, context2 );
                throw exception;
            }

            return result;
        }

        private StringBuilder FormatWarningMessages( List<string> warnings )
        {
            return FormatMessages( warnings, "Warning" );
        }

        /// <summary>
        /// Handles the error messages.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="errorMessages">The error messages.</param>
        private void HandleErrorMessages( List<string> errorMessages )
        {
            if ( errorMessages.Any() )
            {
                StringBuilder sb = new StringBuilder( Result.ToString() );

                sb.Append( FormatMessages( errorMessages, "Error" ) );

                var resultMessage = sb.ToString();

                Result = resultMessage;

                var exception = new Exception( resultMessage );

                HttpContext context2 = HttpContext.Current;
                ExceptionLogService.LogException( exception, context2 );
                throw exception;
            }
        }

        private StringBuilder FormatMessages( List<string> messages, string label )
        {
            StringBuilder sb = new StringBuilder();
            if ( messages.Any() )
            {
                var pluralizedLabel = label.PluralizeIf( messages.Count > 1 );

                sb.AppendLine( $"{messages.Count} {pluralizedLabel}:" );

                messages.ForEach( w => { sb.AppendLine( w ); } );
            }
            return sb;
        }
    }
}