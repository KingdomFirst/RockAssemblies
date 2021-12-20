// <copyright>
// Copyright 2021 by Kingdom First Solutions
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
        IsRequired = false,
        Order = 2,
        Key = AttributeKey.DaysPrior )]

    [AttributeField(
        "Reminder Group Attribute",
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
    public class ZoomMeetingGroupReminder : IJob
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
        /// <param name="context">The context.</param>
        public virtual void Execute( IJobExecutionContext context )
        {
            var rockContext = new RockContext();
            var dataMap = context.JobDetail.JobDataMap;
            resGroupAttribute = AttributeCache.Get( dataMap.GetString( AttributeKey.GroupAttributeSetting ).AsGuid() );
            reservationLocationEntityTypeId = new EntityTypeService( rockContext ).GetNoTracking( com.bemaservices.RoomManagement.SystemGuid.EntityType.RESERVATION_LOCATION.AsGuid() ).Id;

            context.Result = "0 meeting reminders sent.";

            if ( resGroupAttribute == null )
            {
                var errorMessages = new List<string>
                {
                    $"The Reservation Group Attribute job setting is invalid. Please check your Room Registration and KFS Zoom Room plugin configuration."
                };
                HandleErrorMessages( context, errorMessages );
            }

            var systemEmailGuid = dataMap.GetString( AttributeKey.SystemCommunication ).AsGuid();
            var systemCommunication = new SystemCommunicationService( rockContext ).Get( systemEmailGuid );

            var jobPreferredCommunicationType = ( CommunicationType ) dataMap.GetString( AttributeKey.SendUsing ).AsInteger();
            var isSmsEnabled = MediumContainer.HasActiveSmsTransport() && !string.IsNullOrWhiteSpace( systemCommunication.SMSMessage );

            if ( jobPreferredCommunicationType == CommunicationType.SMS && !isSmsEnabled )
            {
                // If sms selected but not usable default to email.
                var errorMessages = new List<string>
                {
                    $"The job is setup to send via SMS but either SMS isn't enabled or no SMS message was found in system communication {systemCommunication.Title}."
                };
                HandleErrorMessages( context, errorMessages );
            }

            var results = new StringBuilder();
            if ( jobPreferredCommunicationType != CommunicationType.Email && string.IsNullOrWhiteSpace( systemCommunication.SMSMessage ) )
            {
                var warning = $"No SMS message found in system communication {systemCommunication.Title}. All Zoom meeting reminders were sent via email.";
                results.AppendLine( warning );
                RockLogger.Log.Warning( RockLogDomains.Jobs, warning );
                jobPreferredCommunicationType = CommunicationType.Email;
            }

            // Get upcoming Zoom Room occurrences
            var roomOccurrenceInfo = GetOccurenceAndGroupData( dataMap, rockContext );

            // Process reminders
            var meetingRemindersResults = SendMeetingReminders( context, rockContext, roomOccurrenceInfo, systemCommunication, jobPreferredCommunicationType );

            results.AppendLine( $"{meetingRemindersResults.MessagesSent} meeting reminders sent." );

            results.Append( FormatWarningMessages( meetingRemindersResults.Warnings ) );

            context.Result = results.ToString();
            HandleErrorMessages( context, meetingRemindersResults.Errors );
        }

        /// <summary>
        /// Gets the room occurrence and group data.
        /// </summary>
        /// <param name="dataMap">The data map.</param>
        /// <param name="rockContext">The rock context.</param>
        /// <returns></returns>
        private Dictionary<RoomOccurrence, Group> GetOccurenceAndGroupData( JobDataMap dataMap, RockContext rockContext )
        {
            var result = new Dictionary<RoomOccurrence, Group>();
            var dates = GetSearchDates( dataMap );
            var startDate = dates.Min();
            var endDate = dates.Max().AddDays( 1 );

            var zRoomOccurrenceService = new RoomOccurrenceService( rockContext );
            var reservationLocationEntityTypeId = new EntityTypeService( rockContext ).GetNoTracking( com.bemaservices.RoomManagement.SystemGuid.EntityType.RESERVATION_LOCATION.AsGuid() ).Id;

            // Find all RoomOccurrences that occur on the targeted dates
            var occurrences = new Dictionary<int, List<DateTime>>();
            var occurrencesToRemind = zRoomOccurrenceService
                .Queryable().AsNoTracking()
                .Where( ro => ro.EntityTypeId == reservationLocationEntityTypeId
                    && ro.ZoomMeetingId > 0
                    && dates.Contains( ro.StartTime.Date ) )
                .ToList();

            // Find all Reservation Locations tied to the occurrences that need reminder sent.
            var occurrenceEntityIds = occurrencesToRemind.Select( o => o.EntityId ).ToList();
            var occReminderInfo = new List<Tuple<Group, RoomOccurrence>>();
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
                var groupAttributeValue = resLoc.Reservation.AttributeValues.FirstOrDefault( v => v.Key == resGroupAttribute.Key ).Value;
                if ( groupAttributeValue != null )
                {
                    var group = groupService.Get( groupAttributeValue.Value.AsGuid() );
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
        /// <param name="dataMap">The data map.</param>
        /// <returns></returns>
        private List<DateTime> GetSearchDates( JobDataMap dataMap )
        {

            // Get the occurrence dates that apply
            var dates = new List<DateTime>();

            try
            {
                var reminderDays = dataMap.GetString( AttributeKey.DaysPrior ).Split( ',' );
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
        private SendMessageResult SendMeetingReminders( IJobExecutionContext context,
                        RockContext rockContext,
                        Dictionary<RoomOccurrence, Group> occurrenceData,
                        SystemCommunication systemCommunication,
                        CommunicationType jobPreferredCommunicationType )
        {
            var result = new SendMessageResult();
            var errorsEmail = new List<string>();
            var errorsSms = new List<string>();
            var errorsPush = new List<string>();

            // Loop through the room occurrence data
            foreach ( var occurrence in occurrenceData )
            {
                var emailMessage = new RockEmailMessage( systemCommunication );
                var smsMessage = new RockSMSMessage( systemCommunication );
                var pushMessage = new RockPushMessage( systemCommunication );
                var group = occurrence.Value;
                foreach ( var groupMember in group.ActiveMembers().ToList() )
                {
                    groupMember.Person.LoadAttributes();
                    var smsNumber = groupMember.Person.PhoneNumbers.GetFirstSmsNumber();
                    if ( !groupMember.Person.CanReceiveEmail( false ) && smsNumber.IsNullOrWhiteSpace() )
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
                            if ( string.IsNullOrWhiteSpace( smsNumber ) )
                            {
                                errorCount += 1;
                                errorMessages.Add( string.Format( "No SMS number could be found for {0}.", groupMember.Person.FullName ) );
                            }
                            smsMessage.AddRecipient( new RockSMSMessageRecipient( groupMember.Person, smsNumber, mergeFields ) );
                            break;
                        case CommunicationType.PushNotification:
                            var personAlias = new PersonAliasService( rockContext ).Get( groupMember.Person.PrimaryAliasId.Value );
                            List<string> devices = new PersonalDeviceService( rockContext ).Queryable()
                                .Where( a => a.PersonAliasId.HasValue && a.PersonAliasId == personAlias.Id && a.NotificationsEnabled )
                                .Select( a => a.DeviceRegistrationId )
                                .ToList();
                            string deviceIds = String.Join( ",", devices );
                            if ( devices.Count == 0 )
                            {
                                errorCount += 1;
                                errorMessages.Add( string.Format( "No devices that support notifications could be found for {0}.", groupMember.Person.FullName ) );
                            }
                            pushMessage.AddRecipient( new RockPushMessageRecipient( groupMember.Person, deviceIds, mergeFields ) );
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
                        errorCount += errorsEmail.Count;
                        errorMessages.AddRange( errorsEmail );
                    }
                    else
                    {
                        notificationEmails++;
                    }
                }
                if ( smsMessage.GetRecipients().Count > 0 )
                {
                    smsMessage.Send( out errorsSms );
                    if ( errorsSms.Any() )
                    {
                        errorCount += errorsSms.Count;
                        errorMessages.AddRange( errorsSms );
                    }
                    else
                    {
                        notificationSms++;
                    }
                }
                if ( pushMessage.GetRecipients().Count > 0 )
                {
                    pushMessage.Send( out errorsSms );
                    if ( errorsPush.Any() )
                    {
                        errorCount += errorsPush.Count;
                        errorMessages.AddRange( errorsPush );
                    }
                    else
                    {
                        notificationPush++;
                    }
                }
            }
            context.Result = string.Format( "{0} emails sent \n{1} SMS messages sent \n{2} push notifications sent", notificationEmails, notificationSms, notificationPush );
            if ( errorMessages.Any() )
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine();
                sb.Append( string.Format( "{0} Errors: ", errorCount ) );
                errorMessages.ForEach( e => { sb.AppendLine(); sb.Append( e ); } );
                string errors = sb.ToString();
                context.Result += errors;
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
        private void HandleErrorMessages( IJobExecutionContext context, List<string> errorMessages )
        {
            if ( errorMessages.Any() )
            {
                StringBuilder sb = new StringBuilder( context.Result.ToString() );

                sb.Append( FormatMessages( errorMessages, "Error" ) );

                var resultMessage = sb.ToString();

                context.Result = resultMessage;

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