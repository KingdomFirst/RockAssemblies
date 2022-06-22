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
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Web;
using Quartz;
using Rock;
using Rock.Attribute;
using Rock.Communication;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;
using rocks.kfs.StepsToCare.Model;

namespace rocks.kfs.StepsToCare.Jobs
{
    /// <summary>
    /// Job to send automated Care Need Notifications with donations since the last run of the job.
    /// </summary>
    [SystemCommunicationField( "Follow Up Notification Template",
        Description = "The system communication to use when sending the Care Need Follow Up.",
        IsRequired = false,
        Key = AttributeKey.FollowUpSystemCommunication )]

    [SystemCommunicationField( "Outstanding Needs Notification Template",
        Description = "The system communication to use when sending the Outstanding Care Needs notification.",
        IsRequired = false,
        Key = AttributeKey.OutstandingNeedsCommunication )]

    [SystemCommunicationField( "Care Touch Needed Notification Template",
        Description = "The system communication to use when sending the Care Touch needed notification.",
        IsRequired = false,
        Key = AttributeKey.CareTouchNeededCommunication )]

    [IntegerField(
        "Minimum Care Touches",
        Description = "Minimum care touches in 'Minimum Care Touch Hours' before the Care Touch needed notification gets sent out.",
        DefaultIntegerValue = 2,
        IsRequired = true,
        Key = AttributeKey.MinimumCareTouches )]

    [IntegerField(
        "Minimum Care Touch Hours",
        Description = "Minimum care touches in this time period before the Care Touch needed notification gets sent out.",
        DefaultIntegerValue = 24,
        IsRequired = true,
        Key = AttributeKey.MinimumCareTouchesHours )]

    [IntegerField(
        "Follow Up Days",
        Description = "Days after a Care Need has been entered before it changes status to Follow Up.",
        DefaultIntegerValue = 10,
        IsRequired = true,
        Key = AttributeKey.FollowUpDays )]

    [LinkedPage( "Care Dashboard Page",
        Description = "Page used to populate 'LinkedPages.CareDashboard' lava field in notification.",
        Key = AttributeKey.CareDashboardPage )]

    [LinkedPage( "Care Detail Page",
        Description = "Page used to populate 'LinkedPages.CareDetail' lava field in notification.",
        Key = AttributeKey.CareDetailPage )]

    [GroupRoleField( null, "Group Type and Role",
        Description = "Select the group Type and Role of the leader you would like auto assigned to care need. If none are selected it will not auto assign the small group member to the need. ",
        IsRequired = false,
        Key = AttributeKey.GroupTypeAndRole,
        Category = CategoryKey.FutureNeedAssignments )]

    [BooleanField( "Auto Assign Worker with Geofence",
        Description = "Care Need Workers can have Geofence locations assigned to them, if there are workers with geofences and this block setting is enabled it will auto assign workers to this need on new entries based on the requester home being in the geofence.",
        DefaultBooleanValue = true,
        Key = AttributeKey.AutoAssignWorkerGeofence,
        Category = CategoryKey.FutureNeedAssignments )]

    [BooleanField( "Auto Assign Worker (load balanced)",
        Description = "Use intelligent load balancing to auto assign care workers to a care need based on their workload and other parameters?",
        DefaultBooleanValue = true,
        Key = AttributeKey.AutoAssignWorker,
        Category = CategoryKey.FutureNeedAssignments )]

    [SystemCommunicationField( "Newly Assigned Need Notification",
        Description = "Select the system communication template for the new assignment notification.",
        DefaultSystemCommunicationGuid = rocks.kfs.StepsToCare.SystemGuid.SystemCommunication.CARE_NEED_ASSIGNED,
        Key = AttributeKey.NewAssignmentNotification,
        Category = CategoryKey.FutureNeedAssignments )]

    [CustomDropdownListField( "Load Balanced Workers assignment type",
        Description = "How should the auto assign worker load balancing work? Default: Exclusive. \"Prioritize\", it will prioritize the workers being assigned based on campus, category and any other parameters on the worker but still assign to any worker if their workload matches. \"Exclusive\", if there are workers with matching campus, category or other parameters it will only load balance between those workers.",
        ListSource = "Prioritize,Exclusive",
        DefaultValue = "Exclusive",
        Key = AttributeKey.LoadBalanceWorkersType,
        Category = CategoryKey.FutureNeedAssignments )]

    [CustomDropdownListField( "Adults in Family Worker Assignment",
        Description = "How should workers be assigned to spouses and other adults in the family when using 'Family Needs'. Normal behavior, use the same settings as a normal Care Need (Group Leader, Geofence and load balanced), or assign to Care Workers Only (load balanced).",
        ListSource = "Normal,Workers Only",
        DefaultValue = "Normal",
        Key = AttributeKey.AdultFamilyWorkers,
        Category = CategoryKey.FutureNeedAssignments )]

    [IntegerField( "Threshold of Days before Assignment",
        Description = "The number of days you can schedule a need in the future before a need will be assigned to workers.",
        IsRequired = true,
        DefaultIntegerValue = 3,
        Key = AttributeKey.FutureThresholdDays,
        Category = CategoryKey.FutureNeedAssignments )]

    [BooleanField( "Verbose Logging",
        Description = "Enable verbose Logging to help in determining issues with adding needs or auto assigning workers. Not recommended for normal use.",
        DefaultBooleanValue = false,
        Key = AttributeKey.VerboseLogging )]

    [DisallowConcurrentExecution]
    public class CareNeedAutomatedNotifications : IJob
    {
        /// <summary>
        /// Attribute Keys
        /// </summary>
        private static class AttributeKey
        {
            public const string FollowUpSystemCommunication = "FollowUpSystemCommunication";
            public const string OutstandingNeedsCommunication = "OutstandingNeedsCommunication";
            public const string CareTouchNeededCommunication = "CareTouchNeededCommunication";
            public const string MinimumCareTouches = "MinimumCareTouches";
            public const string MinimumCareTouchesHours = "MinimumCareTouchesHours";
            public const string FollowUpDays = "FollowUpDays";
            public const string CareDashboardPage = "CareDashboardPage";
            public const string CareDetailPage = "CareDetailPage";
            public const string GroupTypeAndRole = "GroupTypeAndRole";
            public const string AutoAssignWorkerGeofence = "AutoAssignWorkerGeofence";
            public const string AutoAssignWorker = "AutoAssignWorker";
            public const string NewAssignmentNotification = "NewAssignmentNotification";
            public const string VerboseLogging = "VerboseLogging";
            public const string AdultFamilyWorkers = "AdultFamilyWorkers";
            public const string LoadBalanceWorkersType = "LoadBalanceWorkersType";
            public const string FutureThresholdDays = "FutureThresholdDays";
        }

        private static class CategoryKey
        {
            public const string FutureNeedAssignments = "FutureNeedAssignments";
        }

        private int assignedPersonEmails = 0;
        private int assignedPersonSms = 0;
        private int errorCount = 0;

        private List<string> errorMessages = new List<string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="SendCommunications"/> class.
        /// </summary>
        public CareNeedAutomatedNotifications()
        {
        }

        /// <summary>
        /// Executes the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        public virtual void Execute( IJobExecutionContext context )
        {
            var dataMap = context.JobDetail.JobDataMap;
            var JobStartDateTime = RockDateTime.Now;
            var minimumCareTouches = dataMap.GetIntegerFromString( AttributeKey.MinimumCareTouches );
            var minimumCareTouchesHours = dataMap.GetIntegerFromString( AttributeKey.MinimumCareTouches );
            var followUpDays = dataMap.GetIntegerFromString( AttributeKey.FollowUpDays );

            using ( var rockContext = new RockContext() )
            {
                // get the last run date or yesterday
                DateTime? lastStartDateTime = null;

                // get job type id
                int jobId = context.JobDetail.Description.AsInteger();

                // load job
                var job = new ServiceJobService( rockContext )
                    .GetNoTracking( jobId );

                if ( job != null && job.Guid != Rock.SystemGuid.ServiceJob.JOB_PULSE.AsGuid() )
                {
                    lastStartDateTime = job.LastRunDateTime?.AddSeconds( 0.0d - ( double ) job.LastRunDurationSeconds );
                }
                var beginDateTime = lastStartDateTime ?? JobStartDateTime.AddDays( -1 );
                //beginDateTime = JobStartDateTime.AddDays( -3 );

                var detailPage = PageCache.Get( dataMap.GetString( AttributeKey.CareDetailPage ) );
                var detailPageRoute = detailPage.PageRoutes.FirstOrDefault();
                var detailPagePath = detailPageRoute != null ? "/" + detailPageRoute.Route : "/page/" + detailPage.Id;
                var dashboardPage = PageCache.Get( dataMap.GetString( AttributeKey.CareDashboardPage ) );
                var dashboardPageRoute = dashboardPage.PageRoutes.FirstOrDefault();
                var dashboardPagePath = dashboardPageRoute != null ? "/" + dashboardPageRoute.Route : "/page/" + dashboardPage.Id;

                AssignWorkersToNeeds( rockContext, beginDateTime, dataMap, detailPagePath, dashboardPagePath );

                var careNeedService = new CareNeedService( rockContext );
                var assignedPersonService = new AssignedPersonService( rockContext );

                var noteType = NoteTypeCache.GetByEntity( EntityTypeCache.Get( typeof( CareNeed ) ).Id, "", "", true ).FirstOrDefault();
                var careNeedNotesQry = new NoteService( rockContext )
                    .GetByNoteTypeId( noteType.Id ).AsNoTracking();

                var closedValueId = DefinedValueCache.Get( SystemGuid.DefinedValue.CARE_NEED_STATUS_CLOSED.AsGuid() ).Id;
                var followUpValue = DefinedValueCache.Get( SystemGuid.DefinedValue.CARE_NEED_STATUS_FOLLOWUP.AsGuid() );
                var openValueId = DefinedValueCache.Get( SystemGuid.DefinedValue.CARE_NEED_STATUS_OPEN.AsGuid() ).Id;

                var careNeeds = careNeedService.Queryable( "PersonAlias,SubmitterPersonAlias" ).Where( n => n.StatusValueId != closedValueId );
                var careAssigned = assignedPersonService.Queryable().Where( ap => ap.PersonAliasId != null && ap.NeedId != null && ap.CareNeed.StatusValueId != closedValueId ).DistinctBy( ap => ap.PersonAliasId );

                var careNeedFollowUp = careNeeds.Where( n => n.StatusValueId == openValueId && n.DateEntered <= DbFunctions.AddDays( RockDateTime.Now, -followUpDays ) );

                var careNeed24Hrs = careNeeds.Where( n => n.StatusValueId == openValueId && DbFunctions.DiffHours( n.DateEntered.Value, RockDateTime.Now ) >= minimumCareTouchesHours );
                var careNeedFlagged = careNeed24Hrs
                    .SelectMany( cn => careNeedNotesQry.Where( n => n.EntityId == cn.Id && cn.AssignedPersons.Any( ap => ap.FollowUpWorker.HasValue && ap.FollowUpWorker.Value && ap.PersonAliasId == n.CreatedByPersonAliasId ) ).DefaultIfEmpty(),
                    ( cn, n ) => new
                    {
                        CareNeed = cn,
                        HasFollowUpWorkerNote = n != null,
                        TouchCount = careNeedNotesQry.Where( note => note.EntityId == cn.Id ).Count()
                    } )
                    .Where( f => !f.HasFollowUpWorkerNote || f.TouchCount <= minimumCareTouches )
                    .ToList();

                var followUpSystemCommunicationGuid = dataMap.GetString( AttributeKey.FollowUpSystemCommunication ).AsGuid();
                var careTouchNeededCommunicationGuid = dataMap.GetString( AttributeKey.CareTouchNeededCommunication ).AsGuid();
                var outstandingNeedsCommunicationGuid = dataMap.GetString( AttributeKey.OutstandingNeedsCommunication ).AsGuid();
                var followUpSystemCommunication = new SystemCommunicationService( rockContext ).Get( followUpSystemCommunicationGuid );
                var careTouchNeededCommunication = new SystemCommunicationService( rockContext ).Get( careTouchNeededCommunicationGuid );
                var outstandingNeedsCommunication = new SystemCommunicationService( rockContext ).Get( outstandingNeedsCommunicationGuid );

                Dictionary<string, object> linkedPages = new Dictionary<string, object>();
                linkedPages.Add( "CareDetail", detailPagePath );
                linkedPages.Add( "CareDashboard", dashboardPagePath );

                var errors = new List<string>();
                var errorsSms = new List<string>();

                // Update status to follow up and email follow up messages
                foreach ( var careNeed in careNeedFollowUp )
                {
                    careNeed.StatusValueId = followUpValue.Id;
                    careNeed.LoadAttributes();

                    if ( !followUpSystemCommunicationGuid.IsEmpty() )
                    {
                        var emailMessage = new RockEmailMessage( followUpSystemCommunication );
                        var smsMessage = new RockSMSMessage( followUpSystemCommunication );

                        foreach ( var assignee in careNeed.AssignedPersons.Where( ap => ap.FollowUpWorker.HasValue && ap.FollowUpWorker.Value ) )
                        {
                            assignee.PersonAlias.Person.LoadAttributes();

                            var smsNumber = assignee.PersonAlias.Person.PhoneNumbers.GetFirstSmsNumber();
                            if ( !assignee.PersonAlias.Person.CanReceiveEmail( false ) && smsNumber.IsNullOrWhiteSpace() )
                            {
                                continue;
                            }
                            var mergeFields = Rock.Lava.LavaHelper.GetCommonMergeFields( null, assignee.PersonAlias.Person );
                            mergeFields.Add( "CareNeed", careNeed );
                            mergeFields.Add( "LinkedPages", linkedPages );
                            mergeFields.Add( "AssignedPerson", assignee );
                            mergeFields.Add( "Person", assignee.PersonAlias.Person );

                            var recipients = new List<RockMessageRecipient>();
                            recipients.Add( new RockEmailMessageRecipient( assignee.PersonAlias.Person, mergeFields ) );

                            var notificationType = assignee.PersonAlias.Person.GetAttributeValue( SystemGuid.PersonAttribute.NOTIFICATION.AsGuid() );

                            if ( notificationType == null || notificationType == "Email" || notificationType == "Both" )
                            {
                                if ( !assignee.PersonAlias.Person.CanReceiveEmail( false ) )
                                {
                                    errorCount += 1;
                                    errorMessages.Add( string.Format( "{0} does not have a valid email address.", assignee.PersonAlias.Person.FullName ) );
                                }
                                else
                                {
                                    emailMessage.AddRecipient( new RockEmailMessageRecipient( assignee.PersonAlias.Person, mergeFields ) );
                                }
                            }
                            if ( notificationType == "SMS" || notificationType == "Both" )
                            {
                                if ( string.IsNullOrWhiteSpace( smsNumber ) )
                                {
                                    errorCount += 1;
                                    errorMessages.Add( string.Format( "No SMS number could be found for {0}.", assignee.PersonAlias.Person.FullName ) );
                                }
                                smsMessage.AddRecipient( new RockSMSMessageRecipient( assignee.PersonAlias.Person, smsNumber, mergeFields ) );
                            }
                            //pushMessage.AddRecipient( new RockPushMessageRecipient( assignee.PersonAlias.Person, assignee.PersonAlias.Person.Devices, mergeFields ) );
                        }

                        if ( emailMessage.GetRecipients().Count > 0 )
                        {
                            emailMessage.Send( out errors );
                        }
                        if ( smsMessage.GetRecipients().Count > 0 )
                        {
                            smsMessage.Send( out errorsSms );
                        }

                        if ( errors.Any() )
                        {
                            errorCount += errors.Count;
                            errorMessages.AddRange( errors );
                        }
                        else
                        {
                            assignedPersonEmails++;
                        }
                        if ( errorsSms.Any() )
                        {
                            errorCount += errorsSms.Count;
                            errorMessages.AddRange( errorsSms );
                        }
                        else
                        {
                            assignedPersonSms++;
                        }
                    }
                }
                rockContext.SaveChanges();

                // Send notification about "Flagged" messages (any messages without a care touch by the follow up worker or minimum care touches within the set minimum Care Touches Hours.
                if ( careTouchNeededCommunication != null && careTouchNeededCommunication.Id > 0 )
                {
                    foreach ( var flagNeed in careNeedFlagged )
                    {
                        var careNeed = flagNeed.CareNeed;
                        careNeed.LoadAttributes();
                        var emailMessage = new RockEmailMessage( careTouchNeededCommunication );
                        var smsMessage = new RockSMSMessage( careTouchNeededCommunication );
                        //var pushMessage = new RockPushMessage( careTouchNeededCommunication );
                        var recipients = new List<RockMessageRecipient>();

                        foreach ( var assignee in careNeed.AssignedPersons.Where( ap => ap.FollowUpWorker.HasValue && ap.FollowUpWorker.Value ) )
                        {
                            assignee.PersonAlias.Person.LoadAttributes();

                            var smsNumber = assignee.PersonAlias.Person.PhoneNumbers.GetFirstSmsNumber();
                            if ( !assignee.PersonAlias.Person.CanReceiveEmail( false ) && smsNumber.IsNullOrWhiteSpace() )
                            {
                                continue;
                            }

                            var mergeFields = Rock.Lava.LavaHelper.GetCommonMergeFields( null, assignee.PersonAlias.Person );
                            mergeFields.Add( "CareNeed", careNeed );
                            mergeFields.Add( "LinkedPages", linkedPages );
                            mergeFields.Add( "AssignedPerson", assignee );
                            mergeFields.Add( "Person", assignee.PersonAlias.Person );
                            mergeFields.Add( "TouchCount", flagNeed.TouchCount );
                            mergeFields.Add( "HasFollowUpWorkerNote", flagNeed.HasFollowUpWorkerNote );

                            var notificationType = assignee.PersonAlias.Person.GetAttributeValue( SystemGuid.PersonAttribute.NOTIFICATION.AsGuid() );

                            if ( notificationType == null || notificationType == "Email" || notificationType == "Both" )
                            {
                                if ( !assignee.PersonAlias.Person.CanReceiveEmail( false ) )
                                {
                                    errorCount += 1;
                                    errorMessages.Add( string.Format( "{0} does not have a valid email address.", assignee.PersonAlias.Person.FullName ) );
                                }
                                else
                                {
                                    emailMessage.AddRecipient( new RockEmailMessageRecipient( assignee.PersonAlias.Person, mergeFields ) );
                                }
                            }
                            if ( notificationType == "SMS" || notificationType == "Both" )
                            {
                                if ( string.IsNullOrWhiteSpace( smsNumber ) )
                                {
                                    errorCount += 1;
                                    errorMessages.Add( string.Format( "No SMS number could be found for {0}.", assignee.PersonAlias.Person.FullName ) );
                                }
                                smsMessage.AddRecipient( new RockSMSMessageRecipient( assignee.PersonAlias.Person, smsNumber, mergeFields ) );
                            }
                            //pushMessage.AddRecipient( new RockPushMessageRecipient( assignee.PersonAlias.Person, assignee.PersonAlias.Person.Devices, mergeFields ) );

                        }
                        if ( emailMessage.GetRecipients().Count > 0 )
                        {
                            emailMessage.Send( out errors );
                        }
                        if ( smsMessage.GetRecipients().Count > 0 )
                        {
                            smsMessage.Send( out errorsSms );
                        }

                        if ( errors.Any() )
                        {
                            errorCount += errors.Count;
                            errorMessages.AddRange( errors );
                        }
                        else
                        {
                            assignedPersonEmails++;
                        }
                        if ( errorsSms.Any() )
                        {
                            errorCount += errorsSms.Count;
                            errorMessages.AddRange( errorsSms );
                        }
                        else
                        {
                            assignedPersonSms++;
                        }
                    }
                }

                // Send Outstanding needs daily notification
                if ( outstandingNeedsCommunication != null && outstandingNeedsCommunication.Id > 0 )
                {
                    foreach ( var assigned in careAssigned )
                    {
                        var smsNumber = assigned.PersonAlias.Person.PhoneNumbers.GetFirstSmsNumber();

                        if ( !assigned.PersonAlias.Person.CanReceiveEmail( false ) && smsNumber.IsNullOrWhiteSpace() )
                        {
                            continue;
                        }
                        var emailMessage = new RockEmailMessage( outstandingNeedsCommunication );
                        var smsMessage = new RockSMSMessage( outstandingNeedsCommunication );
                        //var pushMessage = new RockPushMessage( outstandingNeedsCommunication );
                        var recipients = new List<RockMessageRecipient>();

                        var assignedNeeds = careNeeds.Where( cn => cn.AssignedPersons.Any( ap => ap.PersonAliasId == assigned.PersonAliasId ) );

                        assigned.PersonAlias.Person.LoadAttributes();

                        var mergeFields = Rock.Lava.LavaHelper.GetCommonMergeFields( null, assigned.PersonAlias.Person );
                        mergeFields.Add( "CareNeeds", assignedNeeds );
                        mergeFields.Add( "LinkedPages", linkedPages );
                        mergeFields.Add( "AssignedPerson", assigned );
                        mergeFields.Add( "Person", assigned.PersonAlias.Person );

                        var notificationType = assigned.PersonAlias.Person.GetAttributeValue( SystemGuid.PersonAttribute.NOTIFICATION.AsGuid() );

                        if ( notificationType == null || notificationType == "Email" || notificationType == "Both" )
                        {
                            if ( !assigned.PersonAlias.Person.CanReceiveEmail( false ) )
                            {
                                errorCount += 1;
                                errorMessages.Add( string.Format( "{0} does not have a valid email address.", assigned.PersonAlias.Person.FullName ) );
                            }
                            else
                            {
                                emailMessage.AddRecipient( new RockEmailMessageRecipient( assigned.PersonAlias.Person, mergeFields ) );
                            }
                        }
                        if ( notificationType == "SMS" || notificationType == "Both" )
                        {
                            if ( string.IsNullOrWhiteSpace( smsNumber ) )
                            {
                                errorCount += 1;
                                errorMessages.Add( string.Format( "No SMS number could be found for {0}.", assigned.PersonAlias.Person.FullName ) );
                            }
                            smsMessage.AddRecipient( new RockSMSMessageRecipient( assigned.PersonAlias.Person, smsNumber, mergeFields ) );
                        }
                        //pushMessage.AddRecipient( new RockPushMessageRecipient( assignee.PersonAlias.Person, assignee.PersonAlias.Person.Devices, mergeFields ) );

                        if ( emailMessage.GetRecipients().Count > 0 )
                        {
                            emailMessage.Send( out errors );
                        }
                        if ( smsMessage.GetRecipients().Count > 0 )
                        {
                            smsMessage.Send( out errorsSms );
                        }

                        if ( errors.Any() )
                        {
                            errorCount += errors.Count;
                            errorMessages.AddRange( errors );
                        }
                        else
                        {
                            assignedPersonEmails++;
                        }
                        if ( errorsSms.Any() )
                        {
                            errorCount += errorsSms.Count;
                            errorMessages.AddRange( errorsSms );
                        }
                        else
                        {
                            assignedPersonSms++;
                        }
                    }
                }

            }
            context.Result = string.Format( "{0} emails sent \n{1} SMS messages sent", assignedPersonEmails, assignedPersonSms );
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
        }

        private void AssignWorkersToNeeds( RockContext rockContext, DateTime beginDateTime, JobDataMap dataMap, string detailPagePath, string dashboardPagePath )
        {
            var autoAssignWorker = dataMap.GetBooleanFromString( AttributeKey.AutoAssignWorker );
            var autoAssignWorkerGeofence = dataMap.GetBooleanFromString( AttributeKey.AutoAssignWorkerGeofence );
            var loadBalanceType = dataMap.GetString( AttributeKey.LoadBalanceWorkersType );
            var enableLogging = dataMap.GetBooleanFromString( AttributeKey.VerboseLogging );
            var leaderRoleGuid = dataMap.GetString( AttributeKey.GroupTypeAndRole ).AsGuidOrNull() ?? Guid.Empty;
            var futureThresholdDays = dataMap.GetDoubleFromString( AttributeKey.FutureThresholdDays );
            var assignmentEmailTemplateGuid = dataMap.GetString( AttributeKey.NewAssignmentNotification ).AsGuidOrNull();
            var adultFamilyWorkers = dataMap.GetString( AttributeKey.AdultFamilyWorkers );
            var newlyAssignedPersons = new List<AssignedPerson>();

            var careNeedService = new CareNeedService( rockContext );
            var futureThresholdDate = DateTime.Now.AddDays( futureThresholdDays );
            var unassignedCareNeeds = careNeedService.Queryable().Where( cn => cn.DateEntered >= beginDateTime && cn.DateEntered <= futureThresholdDate && !cn.AssignedPersons.Any() );

            foreach ( var careNeed in unassignedCareNeeds )
            {
                CareUtilities.AutoAssignWorkers( careNeed, autoAssignWorker: autoAssignWorker, autoAssignWorkerGeofence: autoAssignWorkerGeofence, loadBalanceType: loadBalanceType, enableLogging: enableLogging, leaderRoleGuid: leaderRoleGuid );

                if ( careNeed.ChildNeeds != null && careNeed.ChildNeeds.Any() )
                {
                    var familyGroupType = GroupTypeCache.GetFamilyGroupType();
                    var adultRoleId = familyGroupType.Roles.FirstOrDefault( a => a.Guid == Rock.SystemGuid.GroupRole.GROUPROLE_FAMILY_MEMBER_ADULT.AsGuid() ).Id;
                    foreach ( var need in careNeed.ChildNeeds )
                    {
                        if ( need.PersonAlias != null && need.PersonAlias.Person.GetFamilyRole().Id != adultRoleId )
                        {
                            CareUtilities.AutoAssignWorkers( need, true, true, autoAssignWorker: autoAssignWorker, autoAssignWorkerGeofence: autoAssignWorkerGeofence, loadBalanceType: loadBalanceType, enableLogging: enableLogging, leaderRoleGuid: leaderRoleGuid );
                        }
                        else
                        {
                            CareUtilities.AutoAssignWorkers( need, adultFamilyWorkers == "Workers Only", autoAssignWorker: autoAssignWorker, autoAssignWorkerGeofence: autoAssignWorkerGeofence, loadBalanceType: loadBalanceType, enableLogging: enableLogging, leaderRoleGuid: leaderRoleGuid );
                        }
                    }
                }

                CareUtilities.SendWorkerNotification( rockContext, careNeed, true, newlyAssignedPersons, assignmentEmailTemplateGuid, null, detailPagePath, dashboardPagePath, careNeed.SubmitterPersonAlias.Person );
            }
        }
    }
}