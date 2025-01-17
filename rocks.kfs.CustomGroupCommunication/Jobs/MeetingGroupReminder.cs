// <copyright>
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
using Quartz;
using Rock;
using Rock.Attribute;
using Rock.Communication;
using Rock.Data;
using Rock.Logging;
using Rock.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;

namespace rocks.kfs.CustomGroupCommunication.Jobs
{
    /// <summary>
    /// Job to process communications
    /// </summary>
    [DisplayName( "Send Custom Meeting Reminders" )]
    [Description( "Sends a reminder to members of a group with the appropriate attribute set for this custom reminder." )]

    #region Job Attributes
    [SystemCommunicationField(
        "System Communication",
        Description = "The system communication to use when sending meeting reminders.",
        DefaultValue = Guid.SystemComunication.CUSTOM_GROUP_MEETING_REMINDER,
        IsRequired = true,
        Order = 1,
        Key = AttributeKey.SystemCommunication )]

    [TextField(
        "Days Prior",
        Description = "Comma delimited list of days prior to a scheduled meeting to send a reminder. For example, a value of '2,4' would result in reminders getting sent two and four days prior to the meeting's scheduled meeting date.",
        DefaultValue = "1",
        IsRequired = true,
        Order = 2,
        Key = AttributeKey.DaysPrior )]

    [CustomDropdownListField(
        "Send Using",
        Description = "Specifies how the reminder will be sent.",
        ListSource = "1^Email,2^SMS,0^Recipient Preference",
        IsRequired = true,
        DefaultValue = "1",
        Order = 3,
        Key = AttributeKey.SendUsing )]

    [AttributeField(
        "Group Attribute",
        Description = "The Group Attribute where we will look for the set value. We recommend a Boolean field type attribute.",
        EntityTypeGuid = Rock.SystemGuid.EntityType.GROUP,
        IsRequired = true,
        Order = 4,
        Key = AttributeKey.GroupAttributeSetting
        )]

    #endregion Job Attributes

    [DisallowConcurrentExecution]
    public class CustomMeetingGroupReminder : IJob
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

        private List<string> errorMessages = new List<string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Custom Meeting Group Reminder"/> class.
        /// </summary>
        public CustomMeetingGroupReminder()
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
            var groupsToNotify = new Dictionary<Group, List<DateTime>>();

            // Get the schedule dates that apply
            var dates = new List<DateTime>();
            // This job was specifically requested for future reminders only.
            //dates.Add( RockDateTime.Today );
            try
            {
                List<int> reminderDays = dataMap.GetString( AttributeKey.DaysPrior ).Split( ',' ).Select( d => Convert.ToInt32( d.Trim() ) ).ToList();
                foreach ( int reminderDay in reminderDays )
                {
                    var reminderDate = RockDateTime.Today.AddDays( reminderDay );
                    if ( !dates.Contains( reminderDate ) )
                    {
                        dates.Add( reminderDate );
                    }
                }
            }
            catch { }

            var startDateSearch = dates.Min();
            var endDateSearch = dates.Max().AddDays( 1 );

            var groupAttributeSetting = dataMap.GetString( AttributeKey.GroupAttributeSetting ).AsGuid();
            var daysPrior = dataMap.GetDouble( AttributeKey.DaysPrior );
            var groupQuery = new GroupService( rockContext )
                            .Queryable( "Schedule" )
                            .Where( g =>
                                g.IsActive &&
                                g.Schedule != null )
                            .WhereAttributeValue( rockContext, av => av.Attribute.Guid.Equals( groupAttributeSetting ) && av.ValueAsBoolean == true );

            foreach ( var group in groupQuery.ToList() )
            {
                if ( !string.IsNullOrWhiteSpace( group.Schedule.iCalendarContent ) )
                {
                    // If schedule has an iCal schedule, get occurrences between first and last dates
                    foreach ( var occurrence in group.Schedule.GetICalOccurrences( startDateSearch, endDateSearch ) )
                    {
                        var startTime = occurrence.Period.StartTime.Value;
                        if ( dates.Contains( startTime.Date ) )
                        {
                            if ( !groupsToNotify.ContainsKey( group ) )
                            {
                                groupsToNotify.Add( group, new List<DateTime>() );
                            }
                            groupsToNotify[group].Add( startTime );
                        }
                    }
                }
                else if ( group.Schedule.WeeklyDayOfWeek.HasValue )
                {
                    foreach ( var date in dates )
                    {
                        if ( date.DayOfWeek == group.Schedule.WeeklyDayOfWeek.Value )
                        {
                            var startTime = date;
                            if ( group.Schedule.WeeklyTimeOfDay.HasValue )
                            {
                                startTime = startTime.Add( group.Schedule.WeeklyTimeOfDay.Value );
                            }
                            if ( !groupsToNotify.ContainsKey( group ) )
                            {
                                groupsToNotify.Add( group, new List<DateTime>() );
                            }
                            groupsToNotify[group].Add( startTime );
                        }
                    }
                }

                // Remove any occurrences during group type exclusion date ranges
                foreach ( var exclusion in group.GroupType.GroupScheduleExclusions )
                {
                    if ( exclusion.StartDate.HasValue && exclusion.EndDate.HasValue )
                    {
                        foreach ( var keyVal in groupsToNotify )
                        {
                            foreach ( var occurrenceDate in keyVal.Value.ToList() )
                            {
                                if ( occurrenceDate >= exclusion.StartDate.Value &&
                                    occurrenceDate < exclusion.EndDate.Value.AddDays( 1 ) )
                                {
                                    keyVal.Value.Remove( occurrenceDate );
                                }
                            }
                        }
                    }
                }
            }

            context.Result = "0 meeting reminders sent.";

            var systemCommunicationGuid = dataMap.GetString( AttributeKey.SystemCommunication ).AsGuid();
            var systemCommunication = new SystemCommunicationService( rockContext ).Get( systemCommunicationGuid );

            var jobPreferredCommunicationType = ( CommunicationType ) dataMap.GetString( AttributeKey.SendUsing ).AsInteger();
            var isSmsEnabled = MediumContainer.HasActiveSmsTransport() && !string.IsNullOrWhiteSpace( systemCommunication.SMSMessage );
            var isPushEnabled = MediumContainer.HasActivePushTransport() && !string.IsNullOrWhiteSpace( systemCommunication.PushData );

            if ( jobPreferredCommunicationType == CommunicationType.SMS && !isSmsEnabled )
            {
                // If sms selected but not usable default to email.
                var errorMessages = new List<string>
                {
                    $"The job is setup to send via SMS but either SMS isn't enabled or no SMS message was found in system communication {systemCommunication.Title}."
                };
                HandleErrorMessages( context, errorMessages );
            }

            if ( jobPreferredCommunicationType == CommunicationType.PushNotification && !isPushEnabled )
            {
                // If push notification selected but not usable default to email.
                var errorMessages = new List<string>
                {
                    $"The job is setup to send via Push Notification but either Push Notifications are not enabled or no Push message was found in system communication {systemCommunication.Title}."
                };
                HandleErrorMessages( context, errorMessages );
            }

            var results = new StringBuilder();
            if ( jobPreferredCommunicationType == CommunicationType.SMS && string.IsNullOrWhiteSpace( systemCommunication.SMSMessage ) )
            {
                var warning = $"No SMS message found in system communication {systemCommunication.Title}. All meeting reminders will be attempted via email.";
                results.AppendLine( warning );
                RockLogger.Log.Warning( RockLogDomains.Jobs, warning );
                jobPreferredCommunicationType = CommunicationType.Email;
            }

            if ( jobPreferredCommunicationType == CommunicationType.PushNotification && string.IsNullOrWhiteSpace( systemCommunication.PushMessage ) )
            {
                var warning = $"No Push message found in system communication {systemCommunication.Title}. All meeting reminders will be attempted via email.";
                results.AppendLine( warning );
                RockLogger.Log.Warning( RockLogDomains.Jobs, warning );
                jobPreferredCommunicationType = CommunicationType.Email;
            }

            // Process reminders
            SendMessageResult meetingRemindersResults;

            if ( groupsToNotify.Any() )
            {
                meetingRemindersResults = SendMeetingReminders( context, rockContext, groupsToNotify, systemCommunication, jobPreferredCommunicationType, isSmsEnabled, isPushEnabled );
            }


            results.AppendLine( $"{notificationEmails + notificationSms + notificationPush } meeting reminders sent." );

            results.AppendLine( string.Format( "- {0} email(s)\n- {1} SMS message(s)\n- {2} push notification(s)", notificationEmails, notificationSms, notificationPush ) );

            //results.Append( FormatWarningMessages( meetingRemindersResults.Warnings ) );

            context.Result = results.ToString();
            //HandleErrorMessages( context, meetingRemindersResults.Errors );
        }

        /// <summary>
        /// Sends the meeting reminders.
        /// </summary>
        /// <param name="context">The overall job context.</param>
        /// <param name="rockContext">The rockContext.</param>
        /// <param name="groups">The groups.</param>
        /// <param name="systemCommunication">The system communication.</param>
        /// <param name="jobPreferredCommunicationType">Type of the job preferred communication.</param>
        /// <param name="isSmsEnabled">if set to <c>true</c> [is SMS enabled].</param>
        /// <param name="isPushEnabled">if set to <c>true</c> [is push enabled].</param>
        /// <returns></returns>
        private SendMessageResult SendMeetingReminders( IJobExecutionContext context,
                        RockContext rockContext,
                        Dictionary<Group, List<DateTime>> groups,
                        SystemCommunication systemCommunication,
                        CommunicationType jobPreferredCommunicationType,
                        bool isSmsEnabled,
                        bool isPushEnabled )
        {
            var result = new SendMessageResult();
            var errorsEmail = new List<string>();
            var errorsSms = new List<string>();
            var errorsPush = new List<string>();

            foreach ( var notify in groups )
            {
                var emailMessage = new RockEmailMessage( systemCommunication );
                RockSMSMessage smsMessage = isSmsEnabled ? new RockSMSMessage( systemCommunication ) : null;
                RockPushMessage pushMessage = isPushEnabled ? new RockPushMessage( systemCommunication ) : null;

                var group = notify.Key;
                var meetingDates = notify.Value.Select( d => d.ToString() );

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
                    mergeFields.Add( "Person", groupMember.Person );
                    mergeFields.Add( "NextMeetingDates", string.Join( "<br />", meetingDates ) );


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
                context.Result += errors;
                var exception = new Exception( errors );
                HttpContext context2 = HttpContext.Current;
                ExceptionLogService.LogException( exception, context2 );
                throw exception;
            }

            return result;
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

        /// <summary>
        /// Formats the messages.
        /// </summary>
        /// <param name="messages">The messages.</param>
        /// <param name="label">The label.</param>
        /// <returns></returns>
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