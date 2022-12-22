// <copyright>
// Copyright 2020 by Kingdom First Solutions
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

namespace rocks.kfs.FundraisingParticipantSummary.Jobs
{
    /// <summary>
    /// Job to send fundraising participant summary emails with donations since the last run of the job.
    /// </summary>
    [SystemCommunicationField( "System Communication",
        Description = "The system communication to use when sending the fundraising participant summary email.",
        IsRequired = true,
        DefaultSystemCommunicationGuid = "553B6FCA-9AFF-4618-BDE9-FF41A1EC689E",
        Key = AttributeKey.SystemCommunication )]

    [GroupTypesField( "Group Types",
        Description = "Use this setting to send the fundraising participant summary email to entire GroupType(s).",
        Key = AttributeKey.GroupTypes )]

    [GroupField( "Group",
        Description = "Use this setting to send the fundraising participant summary email to a specific Group and its child Groups.",
        Key = AttributeKey.Group )]

    [BooleanField( "Show Address",
        Description = "Determines if the Address column should be displayed in the Contributions List. (Sent to lava, has to be handled in lava display).",
        DefaultBooleanValue = true,
        Key = AttributeKey.ShowAddress )]

    [BooleanField( "Show Amount",
        Description = "Determines if the Amount column should be displayed in the Contributions List. (Sent to lava, has to be handled in lava display).",
        DefaultBooleanValue = true,
        Key = AttributeKey.ShowAmount )]

    [BooleanField( "Send Emails with Zero Donations",
        Description = "Should the emails to the group members still be sent if they had 0 donations in the time period? The time period for this job is anything since it last ran.",
        DefaultBooleanValue = false,
        Key = AttributeKey.SendZeroDonations )]
    [DisallowConcurrentExecution]
    public class FundraisingParticipantSummary : IJob
    {
        /// <summary>
        /// Attribute Keys
        /// </summary>
        private static class AttributeKey
        {
            public const string SystemCommunication = "SystemCommunication";
            public const string GroupTypes = "GroupTypes";
            public const string Group = "Group";
            public const string ShowAddress = "ShowAddress";
            public const string ShowAmount = "ShowAmount";
            public const string SendZeroDonations = "SendEmailswithZeroDonations";
        }

        private int groupMemberEmails = 0;

        private List<string> emailSendErrorMessages = new List<string>();
        private List<string> generalErrorMessages = new List<string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="SendCommunications"/> class.
        /// </summary>
        public FundraisingParticipantSummary()
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
            var systemCommunicationGuid = Guid.Empty;
            var groupTypes = new List<int>();
            var groups = new List<int>();
            var showAddress = dataMap.GetBooleanFromString( AttributeKey.ShowAddress ); ;
            var showAmount = dataMap.GetBooleanFromString( AttributeKey.ShowAmount ); ;
            var sendZero = dataMap.GetBooleanFromString( AttributeKey.SendZeroDonations ); ;

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

                var groupTypeService = new GroupTypeService( rockContext );
                var groupMemberService = new GroupMemberService( rockContext );
                var groupService = new GroupService( rockContext );

                var selectedGroupTypes = dataMap.GetString( AttributeKey.GroupTypes ).Split( new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries );

                if ( selectedGroupTypes.Any() )
                {
                    groupTypes = new List<int>( groupTypeService.GetByGuids( selectedGroupTypes.Select( Guid.Parse ).ToList() ).Select( gt => gt.Id ) );
                }

                var groupGuid = dataMap.GetString( AttributeKey.Group ).AsGuidOrNull();

                if ( ( groupTypes.IsNull() || groupTypes.Count == 0 ) && !groupGuid.HasValue )
                {
                    context.Result = "Job failed. Unable to find group type or selected group. Check your settings.";
                    throw new Exception( "No group type found or group found." );
                }
                Group groupSetting = null;
                if ( groupGuid.HasValue )
                {
                    groupSetting = groupService.Get( groupGuid.Value );
                    groups = groupService.GetAllDescendentGroupIds( groupSetting.Id, false );
                    groups.Add( groupSetting.Id );
                }

                systemCommunicationGuid = dataMap.GetString( AttributeKey.SystemCommunication ).AsGuid();

                var groupMembers = groupMemberService
                    .Queryable( "Group,Person" ).AsNoTracking()
                    .Where( m =>
                        m.GroupMemberStatus == GroupMemberStatus.Active &&
                        (
                          ( groupGuid.HasValue && groups.Contains( m.GroupId ) ) ||
                          ( !groupGuid.HasValue && groupTypes.Contains( m.Group.GroupTypeId ) )
                        ) )
                    .ToList();

                foreach ( var groupMember in groupMembers )
                {
                    groupMember.LoadAttributes();
                    var email = groupMember.Person.Email;
                    var person = groupMember.Person;
                    var group = groupMember.Group;
                    group.LoadAttributes();
                    bool disablePublicContributionRequests = groupMember.GetAttributeValue( "DisablePublicContributionRequests" ).AsBoolean();

                    // only show Contribution stuff if contribution requests haven't been disabled
                    if ( email.IsNotNullOrWhiteSpace() && !disablePublicContributionRequests )
                    {
                        // Progress
                        var entityTypeIdGroupMember = EntityTypeCache.GetId<Rock.Model.GroupMember>();

                        var contributionTotal = new FinancialTransactionDetailService( rockContext ).Queryable()
                                    .Where( d => d.EntityTypeId == entityTypeIdGroupMember
                                            && d.EntityId == groupMember.Id )
                                    .Sum( a => ( decimal? ) a.Amount ) ?? 0.00M;

                        var individualFundraisingGoal = groupMember.GetAttributeValue( "IndividualFundraisingGoal" ).AsDecimalOrNull();
                        if ( !individualFundraisingGoal.HasValue )
                        {
                            individualFundraisingGoal = group.GetAttributeValue( "IndividualFundraisingGoal" ).AsDecimalOrNull();
                        }

                        var amountLeft = individualFundraisingGoal - contributionTotal;
                        var percentMet = individualFundraisingGoal > 0 ? contributionTotal * 100 / individualFundraisingGoal : 100;

                        var financialTransactions = new FinancialTransactionDetailService( rockContext ).Queryable()
                            .Where( d => d.EntityTypeId == entityTypeIdGroupMember
                                    && d.EntityId == groupMember.Id
                                    && d.Transaction.TransactionDateTime >= beginDateTime )
                            .OrderByDescending( a => a.Transaction.TransactionDateTime );

                        if ( financialTransactions.Any() || sendZero )
                        {
                            var mergeFields = Rock.Lava.LavaHelper.GetCommonMergeFields( null, groupMember.Person );
                            mergeFields.Add( "BeginDateTime", beginDateTime );
                            mergeFields.Add( "FundraisingGoal", individualFundraisingGoal );
                            mergeFields.Add( "AmountLeft", amountLeft );
                            mergeFields.Add( "ContributionTotal", contributionTotal );
                            mergeFields.Add( "PercentMet", percentMet );
                            mergeFields.Add( "ShowAddress", showAddress );
                            mergeFields.Add( "ShowAmount", showAmount );
                            mergeFields.Add( "GroupMember", groupMember );
                            mergeFields.Add( "Contributions", financialTransactions );

                            //var queryParams = new Dictionary<string, string>();
                            //queryParams.Add( "GroupId", group.Id.ToString() );
                            //queryParams.Add( "GroupMemberId", groupMember.Id.ToString() );
                            //mergeFields.Add( "MakeDonationUrl", LinkedPageUrl( "DonationPage", queryParams ) );

                            //string makeDonationButtonText = null;
                            //makeDonationButtonText = "Make Payment";

                            //mergeFields.Add( "MakeDonationButtonText", makeDonationButtonText );

                            var recipients = new List<RockMessageRecipient>();
                            recipients.Add( new RockEmailMessageRecipient( person, mergeFields ) );

                            var emailMessage = new RockEmailMessage( systemCommunicationGuid );
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
                                groupMemberEmails++;
                            }
                        }
                    }
                    else if ( !disablePublicContributionRequests )
                    {
                        errorCount += 1;
                        errorMessages.Add( string.Format( "No email specified for {0}.", groupMember.Person.FullName ) );
                    }
                }
            }
            context.Result = string.Format( "{0} emails sent", groupMemberEmails );
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