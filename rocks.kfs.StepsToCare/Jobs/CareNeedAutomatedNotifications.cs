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
        }

        private int assignedPersonEmails = 0;
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

                var careNeedService = new CareNeedService( rockContext );
                var assignedPersonService = new AssignedPersonService( rockContext );

                var noteType = NoteTypeCache.GetByEntity( EntityTypeCache.Get( typeof( CareNeed ) ).Id, "", "", true ).FirstOrDefault();
                var careNeedNotesQry = new NoteService( rockContext )
                    .GetByNoteTypeId( noteType.Id ).AsNoTracking();

                var closedValueId = DefinedValueCache.Get( SystemGuid.DefinedValue.CARE_NEED_STATUS_CLOSED.AsGuid() ).Id;
                var followUpValue = DefinedValueCache.Get( SystemGuid.DefinedValue.CARE_NEED_STATUS_FOLLOWUP.AsGuid() );
                var openValueId = DefinedValueCache.Get( SystemGuid.DefinedValue.CARE_NEED_STATUS_OPEN.AsGuid() ).Id;

                var careNeeds = careNeedService.Queryable( "PersonAlias,SubmitterPersonAlias" ).Where( n => n.StatusValueId != closedValueId );
                var careAssigned = assignedPersonService.Queryable().AsNoTracking().Where( ap => ap.PersonAliasId != null && ap.NeedId != null && ap.CareNeed.StatusValueId != closedValueId ).DistinctBy( ap => ap.PersonAliasId );

                var careNeedFollowUp = careNeeds.Where( n => n.StatusValueId == openValueId && n.DateEntered <= DbFunctions.AddDays( RockDateTime.Now, -followUpDays ) );

                var careNeed24Hrs = careNeeds.AsNoTracking().Where( n => n.StatusValueId == openValueId && DbFunctions.DiffHours( n.DateEntered.Value, RockDateTime.Now ) >= minimumCareTouchesHours );
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
                var careTouchNeededCommunication = new SystemCommunicationService( rockContext ).Get( careTouchNeededCommunicationGuid );
                var outstandingNeedsCommunication = new SystemCommunicationService( rockContext ).Get( outstandingNeedsCommunicationGuid );

                var detailPage = PageCache.Get( dataMap.GetString( AttributeKey.CareDetailPage ) );
                var detailPageRoute = detailPage.PageRoutes.FirstOrDefault();
                var dashboardPage = PageCache.Get( dataMap.GetString( AttributeKey.CareDashboardPage ) );
                var dashboardPageRoute = dashboardPage.PageRoutes.FirstOrDefault();
                Dictionary<string, object> linkedPages = new Dictionary<string, object>();
                linkedPages.Add( "CareDetail", detailPageRoute != null ? detailPageRoute.Route : "page/" + detailPage.Id );
                linkedPages.Add( "CareDashboard", dashboardPageRoute != null ? dashboardPageRoute.Route : "page/" + dashboardPage.Id );

                // Update status to follow up and email follow up messages
                foreach ( var careNeed in careNeedFollowUp )
                {
                    careNeed.StatusValueId = followUpValue.Id;

                    if ( !followUpSystemCommunicationGuid.IsEmpty() )
                    {
                        foreach ( var assignee in careNeed.AssignedPersons.Where( ap => ap.FollowUpWorker.HasValue && ap.FollowUpWorker.Value ) )
                        {
                            if ( !assignee.PersonAlias.Person.CanReceiveEmail( false ) )
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

                            var emailMessage = new RockEmailMessage( followUpSystemCommunicationGuid );
                            emailMessage.SetRecipients( recipients );
                            var errors = new List<string>();
                            emailMessage.Send( out errors );

                            if ( errors.Any() )
                            {
                                errorCount += errors.Count;
                                errorMessages.AddRange( errors );
                            }
                            else
                            {
                                assignedPersonEmails++;
                            }
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
                        var emailMessage = new RockEmailMessage( careTouchNeededCommunication );
                        //var smsMessage = new RockSMSMessage( careTouchNeededCommunication );
                        //var pushMessage = new RockPushMessage( careTouchNeededCommunication );
                        var recipients = new List<RockMessageRecipient>();
                        var errors = new List<string>();

                        foreach ( var assignee in careNeed.AssignedPersons )
                        {
                            if ( !assignee.PersonAlias.Person.CanReceiveEmail( false ) )
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

                            emailMessage.AddRecipient( new RockEmailMessageRecipient( assignee.PersonAlias.Person, mergeFields ) );
                            //emailMessage.AddRecipient( new RockSMSMessageRecipient( assignee.PersonAlias.Person, assignee.PersonAlias.Person.PhoneNumbers.GetFirstSmsNumber() mergeFields ) )
                            //pushMessage.AddRecipient( new RockPushMessageRecipient( assignee.PersonAlias.Person, assignee.PersonAlias.Person.Devices, mergeFields ) );
                        }
                        emailMessage.Send( out errors );

                        if ( errors.Any() )
                        {
                            errorCount += errors.Count;
                            errorMessages.AddRange( errors );
                        }
                        else
                        {
                            assignedPersonEmails++;
                        }

                    }
                }

                // Send Outstanding needs daily notification
                if ( outstandingNeedsCommunication != null && outstandingNeedsCommunication.Id > 0 )
                {
                    foreach ( var assigned in careAssigned )
                    {
                        if ( !assigned.PersonAlias.Person.CanReceiveEmail( false ) )
                        {
                            continue;
                        }
                        var emailMessage = new RockEmailMessage( outstandingNeedsCommunication );
                        //var smsMessage = new RockSMSMessage( outstandingNeedsCommunication );
                        //var pushMessage = new RockPushMessage( outstandingNeedsCommunication );
                        var recipients = new List<RockMessageRecipient>();
                        var errors = new List<string>();

                        var assignedNeeds = careNeeds.Where( cn => cn.AssignedPersons.Any( ap => ap.PersonAliasId == assigned.PersonAliasId ) );

                        var mergeFields = Rock.Lava.LavaHelper.GetCommonMergeFields( null, assigned.PersonAlias.Person );
                        mergeFields.Add( "CareNeeds", assignedNeeds );
                        mergeFields.Add( "LinkedPages", linkedPages );
                        mergeFields.Add( "AssignedPerson", assigned );
                        mergeFields.Add( "Person", assigned.PersonAlias.Person );

                        emailMessage.AddRecipient( new RockEmailMessageRecipient( assigned.PersonAlias.Person, mergeFields ) );
                        //emailMessage.AddRecipient( new RockSMSMessageRecipient( assignee.PersonAlias.Person, assignee.PersonAlias.Person.PhoneNumbers.GetFirstSmsNumber() mergeFields ) )
                        //pushMessage.AddRecipient( new RockPushMessageRecipient( assignee.PersonAlias.Person, assignee.PersonAlias.Person.Devices, mergeFields ) );
                        emailMessage.Send( out errors );

                        if ( errors.Any() )
                        {
                            errorCount += errors.Count;
                            errorMessages.AddRange( errors );
                        }
                        else
                        {
                            assignedPersonEmails++;
                        }

                    }
                }

            }
            context.Result = string.Format( "{0} emails sent", assignedPersonEmails );
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
    }
}