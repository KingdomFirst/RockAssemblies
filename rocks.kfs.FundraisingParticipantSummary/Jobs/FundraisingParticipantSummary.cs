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
using Rock.Jobs;
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

    [BooleanField( "Verbose Logging",
        Description = "Enable verbose Logging to help in troubleshooting errors with obscure traceability. Not recommended for normal use.",
        DefaultBooleanValue = false,
        Key = AttributeKey.VerboseLogging )]
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
            public const string VerboseLogging = "VerboseLogging";
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
            var showAddress = dataMap.GetBooleanFromString( AttributeKey.ShowAddress );
            var showAmount = dataMap.GetBooleanFromString( AttributeKey.ShowAmount );
            var sendZero = dataMap.GetBooleanFromString( AttributeKey.SendZeroDonations );
            var enableLogging = dataMap.GetBooleanFromString( AttributeKey.VerboseLogging );

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

                LogEvent( null, "GroupInfo", string.Format( "Selected Group Types: {0}", selectedGroupTypes.Count() ), "Start getting GroupTypes from selected group types.", enableLogging );
                if ( selectedGroupTypes.Any() )
                {
                    groupTypes = new List<int>( groupTypeService.GetByGuids( selectedGroupTypes.Select( Guid.Parse ).ToList() ).Select( gt => gt.Id ) );
                }
                LogEvent( null, "GroupInfo", string.Format( "GroupTypes: {0}", groupTypes.Count() ), "Finished getting GroupTypes from selected group types.", enableLogging );

                var groupGuid = dataMap.GetString( AttributeKey.Group ).AsGuidOrNull();

                if ( ( groupTypes.IsNull() || groupTypes.Count == 0 ) && !groupGuid.HasValue )
                {
                    context.Result = "Job failed. Unable to find group type or selected group. Check your settings.";
                    throw new Exception( "No group type found or group found." );
                }
                Group groupSetting = null;

                if ( groupGuid.HasValue )
                {
                    LogEvent( null, "GroupInfo", string.Format( "Selected Group: {0}", groupGuid.Value ), "Start getting Groups from selected group.", enableLogging );
                    groupSetting = groupService.Get( groupGuid.Value );
                    groups = groupService.GetAllDescendentGroupIds( groupSetting.Id, false );
                    groups.Add( groupSetting.Id );
                    LogEvent( null, "GroupInfo", string.Format( "Groups: {0}", groups.Count() ), "Finish getting Groups from selected group.", enableLogging );
                }

                systemCommunicationGuid = dataMap.GetString( AttributeKey.SystemCommunication ).AsGuid();


                LogEvent( null, "GroupInfo", string.Format( "Selected Group: {0}", groupGuid.Value ), "Start getting Group Members from groups.", enableLogging );
                var groupMembers = groupMemberService
                    .Queryable( "Group,Person" ).AsNoTracking()
                    .Where( m =>
                        m.GroupMemberStatus == GroupMemberStatus.Active &&
                        (
                          ( groupGuid.HasValue && groups.Contains( m.GroupId ) ) ||
                          ( !groupGuid.HasValue && groupTypes.Contains( m.Group.GroupTypeId ) )
                        ) )
                    .ToList();
                LogEvent( null, "GroupInfo", string.Format( "GroupMembers: {0}", groupMembers.Count() ), "Finish getting Group Members from groups.", enableLogging );
                foreach ( var groupMember in groupMembers )
                {
                    LogEvent( null, "GroupMemberInfo", string.Format( "GroupMember: {0}", groupMember.Id ), "Start processing group member.", enableLogging );
                    LogEvent( null, "GroupMemberInfo", string.Format( "GroupMember: {0}", groupMember.Id ), "Start loading group member attributes.", enableLogging );
                    groupMember.LoadAttributes();
                    LogEvent( null, "GroupMemberInfo", string.Format( "GroupMember: {0}", groupMember.Id ), "Finish loading group member attributes.", enableLogging );
                    var email = groupMember.Person.Email;
                    var person = groupMember.Person;
                    var group = groupMember.Group;
                    LogEvent( null, "GroupMemberInfo", string.Format( "GroupMember: {0}, Group: {1}", groupMember.Id, groupMember.Group.Id ), "Start loading group attributes.", enableLogging );
                    group.LoadAttributes();
                    LogEvent( null, "GroupMemberInfo", string.Format( "GroupMember: {0}, Group: {1}", groupMember.Id, groupMember.Group.Id ), "Finish loading group attributes.", enableLogging );
                    bool disablePublicContributionRequests = groupMember.GetAttributeValue( "DisablePublicContributionRequests" ).AsBoolean();

                    // only show Contribution stuff if contribution requests haven't been disabled
                    if ( email.IsNotNullOrWhiteSpace() && !disablePublicContributionRequests )
                    {
                        // Progress
                        var entityTypeIdGroupMember = EntityTypeCache.GetId<Rock.Model.GroupMember>();

                        LogEvent( null, "GroupMemberContributionInfo", string.Format( "GroupMember: {0}, Group: {1}", groupMember.Id, groupMember.Group.Id ), "Start calculating contribution total.", enableLogging );
                        var contributionTotal = new FinancialTransactionDetailService( rockContext ).Queryable()
                                    .Where( d => d.EntityTypeId == entityTypeIdGroupMember
                                            && d.EntityId == groupMember.Id )
                                    .Sum( a => ( decimal? ) a.Amount ) ?? 0.00M;
                        LogEvent( null, "GroupMemberContributionInfo", string.Format( "GroupMember: {0}, Group: {1}", groupMember.Id, groupMember.Group.Id ), "Finished calculating contribution total.", enableLogging );

                        var individualFundraisingGoal = groupMember.GetAttributeValue( "IndividualFundraisingGoal" ).AsDecimalOrNull();
                        if ( !individualFundraisingGoal.HasValue )
                        {
                            individualFundraisingGoal = group.GetAttributeValue( "IndividualFundraisingGoal" ).AsDecimalOrNull();
                        }

                        var amountLeft = individualFundraisingGoal - contributionTotal;
                        var percentMet = individualFundraisingGoal > 0 ? contributionTotal * 100 / individualFundraisingGoal : 100;

                        LogEvent( null, "GroupMemberContributionInfo", string.Format( "GroupMember: {0}, Group: {1}", groupMember.Id, groupMember.Group.Id ), "Start getting financial transactions.", enableLogging );
                        var financialTransactions = new FinancialTransactionDetailService( rockContext ).Queryable()
                            .Where( d => d.EntityTypeId == entityTypeIdGroupMember
                                    && d.EntityId == groupMember.Id
                                    && d.Transaction.TransactionDateTime >= beginDateTime )
                            .OrderByDescending( a => a.Transaction.TransactionDateTime );

                        LogEvent( null, "GroupMemberContributionInfo", string.Format( "GroupMember: {0}, Group: {1}", groupMember.Id, groupMember.Group.Id ), "Finish getting financial transactions.", enableLogging );

                        if ( financialTransactions.Any() || sendZero )
                        {
                            LogEvent( null, "ProcessCommunication", string.Format( "GroupMember: {0}, Group: {1}", groupMember.Id, groupMember.Group.Id ), "Start processing group member for communication.", enableLogging );
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

                            LogEvent( null, "ProcessCommunication", string.Format( "GroupMember: {0}, Group: {1}", groupMember.Id, groupMember.Group.Id ), "Start adding group member as recipient.", enableLogging );
                            recipients.Add( new RockEmailMessageRecipient( person, mergeFields ) );
                            LogEvent( null, "ProcessCommunication", string.Format( "GroupMember: {0}, Group: {1}", groupMember.Id, groupMember.Group.Id ), "Finish adding group member as recipient.", enableLogging );

                            var emailMessage = new RockEmailMessage( systemCommunicationGuid );

                            LogEvent( null, "ProcessCommunication", string.Format( "GroupMember: {0}, Group: {1}", groupMember.Id, groupMember.Group.Id ), "Start set recipients.", enableLogging );
                            emailMessage.SetRecipients( recipients );
                            LogEvent( null, "ProcessCommunication", string.Format( "GroupMember: {0}, Group: {1}", groupMember.Id, groupMember.Group.Id ), "Finish set recipients.", enableLogging );
                            var errors = new List<string>();
                            LogEvent( null, "ProcessCommunication", string.Format( "GroupMember: {0}, Group: {1}", groupMember.Id, groupMember.Group.Id ), "Start email send.", enableLogging );
                            emailMessage.Send( out errors );
                            LogEvent( null, "ProcessCommunication", string.Format( "GroupMember: {0}, Group: {1}", groupMember.Id, groupMember.Group.Id ), "Finish email send.", enableLogging );

                            if ( errors.Any() )
                            {
                                errors.ForEach( e => { emailSendErrorMessages.Add( $"{e} - {groupMember.Person.FullName}" ); } );
                            }
                            else
                            {
                                groupMemberEmails++;
                            }
                            LogEvent( null, "ProcessCommunication", string.Format( "GroupMember: {0}, Group: {1}", groupMember.Id, groupMember.Group.Id ), "Finish processing group member for communication.", enableLogging );
                        }
                    }
                    else if ( !disablePublicContributionRequests )
                    {
                        generalErrorMessages.Add( string.Format( "No email specified for {0}.", groupMember.Person.FullName ) );
                    }
                    LogEvent( null, "GroupMemberInfo", string.Format( "GroupMember: {0}", groupMember.Id ), "Finish processing group member.", enableLogging );
                }
            }
            var redIcon = "<i class='fa fa-circle text-danger'></i>";
            var greenIcon = "<i class='fa fa-circle text-success'></i>";
            context.Result = $"{greenIcon} {groupMemberEmails} {"Email".PluralizeIf( groupMemberEmails != 1 )} sent.";

            if ( emailSendErrorMessages.Any() || generalErrorMessages.Any() )
            {
                List<string> errorsCombined = new List<string>();
                errorsCombined.AddRange( generalErrorMessages );
                errorsCombined.AddRange( emailSendErrorMessages );

                StringBuilder sbException = new StringBuilder();
                sbException.Append( $"{errorsCombined.Count()} {"Error".PluralizeIf( errorsCombined.Count() != 1 )}." );
                errorsCombined.ForEach( e => { sbException.AppendLine(); sbException.Append( e ); } );
                string exceptionString = sbException.ToString();
                var exception = new Exception( exceptionString );
                HttpContext context2 = HttpContext.Current;
                ExceptionLogService.LogException( exception, context2 );
                if ( emailSendErrorMessages.Any() && groupMemberEmails == 0 )
                {
                    throw exception;
                }
                else
                {
                    // Build string for result display
                    StringBuilder sbErrors = new StringBuilder();
                    sbErrors.AppendLine();
                    sbErrors.Append( $"{redIcon} {errorsCombined.Count()} {"Error".PluralizeIf( errorsCombined.Count() > 1 )}:" );
                    errorsCombined.ForEach( e => { sbErrors.AppendLine(); sbErrors.Append( $"{e}" ); } );
                    string errors = sbErrors.ToString();
                    context.Result += errors;
                    var ex = new AggregateException( "Fundraising Participant Summary completed with errors.", exception );
                    throw new RockJobWarningException( "Fundraising Participant Summary completed with errors.", ex );
                }
            }
        }

        private static ServiceLog LogEvent( RockContext rockContext, string type, string input, string result, bool enableLogging = false )
        {
            if ( enableLogging )
            {
                if ( rockContext == null )
                {
                    rockContext = new RockContext();
                }

                var rockLogger = new ServiceLogService( rockContext );
                ServiceLog serviceLog = new ServiceLog
                {
                    Name = "Fundraising Participant Summary",
                    Type = type,
                    LogDateTime = RockDateTime.Now,
                    Input = input,
                    Result = result,
                    Success = true
                };
                rockLogger.Add( serviceLog );
                rockContext.SaveChanges();
                return serviceLog;
            }
            else
            {
                return null;
            }
        }
    }
}