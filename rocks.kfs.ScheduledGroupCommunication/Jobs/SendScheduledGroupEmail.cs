// <copyright>
// Copyright 2019 by Kingdom First Solutions
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
using System.Web;

using Quartz;

using Rock;
using Rock.Attribute;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;

using KFSConst = rocks.kfs.ScheduledGroupCommunication.SystemGuid;

namespace rocks.kfs.ScheduledGroupCommunication.Jobs
{
    [LavaCommandsField( "Enabled Lava Commands", "The Lava commands that should be enabled for this job.", false, order: 0 )]
    [IntegerField( "Command Timeout", "Maximum amount of time (in seconds) to wait for the each group communication creation to complete.", false, 180, "General", 1, "CommandTimeout" )]
    [IntegerField( "Last Run Buffer", "Use this setting to add a buffer to not double send, too large of a buffer may cause messages to be missed. By default this job will send any communications that have been scheduled since the last run date time minus the LastRunDurationSeconds, due to the way some server schedules run it is possible to be a few seconds off and double send.", false, -5, "General", 2 )]

    /// <summary>
    /// Job to send scheduled group emails.
    /// </summary>
    [DisallowConcurrentExecution]
    public class SendScheduledGroupEmail : IJob
    {
        /// <summary>
        /// Empty constructor for job initialization
        /// <para>
        /// Jobs require a public empty constructor so that the
        /// scheduler can instantiate the class whenever it needs.
        /// </para>
        /// </summary>
        public SendScheduledGroupEmail()
        {
        }

        /// <summary>
        /// Job that will send scheduled group emails.
        ///
        /// Called by the <see cref="IScheduler" /> when a
        /// <see cref="ITrigger" /> fires that is associated with
        /// the <see cref="IJob" />.
        /// </summary>
        public virtual void Execute( IJobExecutionContext context )
        {
            var dataMap = context.JobDetail.JobDataMap;
            int? commandTimeout = dataMap.GetString( "CommandTimeout" ).AsIntegerOrNull();
            int? lastRunBuffer = dataMap.GetString( "LastRunBuffer" ).AsIntegerOrNull();
            var enabledLavaCommands = dataMap.GetString( "EnabledLavaCommands" );
            var JobStartDateTime = RockDateTime.Now;
            var dateAttributes = new List<AttributeValue>();
            var dAttributeMatrixItemAndGroupIds = new Dictionary<int, int>(); // Key: AttributeMatrixItemId   Value: GroupId
            int communicationsSent = 0;
            var emailMediumType = EntityTypeCache.Get( "Rock.Communication.Medium.Email" );
            var dateAttributeId = Rock.Web.Cache.AttributeCache.Get( KFSConst.Attribute.MATRIX_ATTRIBUTE_EMAIL_SEND_DATE.AsGuid() ).Id;
            var recurrenceAttributeId = Rock.Web.Cache.AttributeCache.Get( KFSConst.Attribute.MATRIX_ATTRIBUTE_EMAIL_SEND_RECURRENCE.AsGuid() ).Id;
            var fromEmailAttributeId = Rock.Web.Cache.AttributeCache.Get( KFSConst.Attribute.MATRIX_ATTRIBUTE_EMAIL_FROM_EMAIL.AsGuid() ).Id;
            var fromNameAttributeId = Rock.Web.Cache.AttributeCache.Get( KFSConst.Attribute.MATRIX_ATTRIBUTE_EMAIL_FROM_NAME.AsGuid() ).Id;
            var subjectAttributeId = Rock.Web.Cache.AttributeCache.Get( KFSConst.Attribute.MATRIX_ATTRIBUTE_EMAIL_SUBJECT.AsGuid() ).Id;
            var messageAttributeId = Rock.Web.Cache.AttributeCache.Get( KFSConst.Attribute.MATRIX_ATTRIBUTE_EMAIL_MESSAGE.AsGuid() ).Id;

            try
            {
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
                        lastStartDateTime = job.LastRunDateTime?.AddSeconds( 0.0d - ( double ) ( job.LastRunDurationSeconds + lastRunBuffer ) );
                    }
                    var beginDateTime = lastStartDateTime ?? JobStartDateTime.AddDays( -1 );

                    // get the date attributes
                    dateAttributes = new AttributeValueService( rockContext )
                        .Queryable().AsNoTracking()
                        .Where( d => d.AttributeId == dateAttributeId &&
                                d.EntityId.HasValue &&
                                d.ValueAsDateTime >= beginDateTime &&
                                d.ValueAsDateTime <= JobStartDateTime )
                        .ToList();
                }

                foreach ( var d in dateAttributes )
                {
                    // Use a new context to limit the amount of change-tracking required
                    using ( var rockContext = new RockContext() )
                    {
                        var attributeMatrixId = new AttributeMatrixItemService( rockContext )
                            .GetNoTracking( d.EntityId.Value )
                            .AttributeMatrixId;

                        var attributeMatrixGuid = new AttributeMatrixService( rockContext )
                            .GetNoTracking( attributeMatrixId )
                            .Guid
                            .ToString();

                        var attributeValue = new AttributeValueService( rockContext )
                            .Queryable().AsNoTracking()
                            .FirstOrDefault( a => a.Value.Equals( attributeMatrixGuid, StringComparison.CurrentCultureIgnoreCase ) );

                        if ( attributeValue != null && attributeValue.EntityId.HasValue )
                        {
                            dAttributeMatrixItemAndGroupIds.Add( d.EntityId.Value, attributeValue.EntityId.Value );
                        }
                    }
                }

                foreach ( var attributeMatrixItemAndGroupId in dAttributeMatrixItemAndGroupIds )
                {
                    // Use a new context to limit the amount of change-tracking required
                    using ( var rockContext = new RockContext() )
                    {
                        rockContext.Database.CommandTimeout = commandTimeout;

                        var fromEmail = new AttributeValueService( rockContext )
                            .GetByAttributeIdAndEntityId( fromEmailAttributeId, attributeMatrixItemAndGroupId.Key )
                            .Value;

                        var fromName = new AttributeValueService( rockContext )
                            .GetByAttributeIdAndEntityId( fromNameAttributeId, attributeMatrixItemAndGroupId.Key )
                            .Value;

                        var subject = new AttributeValueService( rockContext )
                            .GetByAttributeIdAndEntityId( subjectAttributeId, attributeMatrixItemAndGroupId.Key )
                            .Value;

                        var message = new AttributeValueService( rockContext )
                            .GetByAttributeIdAndEntityId( messageAttributeId, attributeMatrixItemAndGroupId.Key )
                            .Value;

                        var attachments = new List<BinaryFile>();

                        var group = new GroupService( rockContext )
                            .GetNoTracking( attributeMatrixItemAndGroupId.Value );

                        if ( !message.IsNullOrWhiteSpace() && emailMediumType != null )
                        {
                            var groupMembers = group.Members.Where( m => m.Person != null && m.Person.Email != null && m.Person.Email != string.Empty && m.GroupMemberStatus == GroupMemberStatus.Active );

                            if ( !groupMembers.Any() )
                            {
                                continue;
                            }

                            var communicationService = new CommunicationService( rockContext );

                            var communication = new Rock.Model.Communication();
                            communication.Status = CommunicationStatus.Transient;
                            communication.ReviewedDateTime = JobStartDateTime;
                            communication.ReviewerPersonAliasId = group.ModifiedByPersonAliasId;
                            communication.SenderPersonAliasId = group.ModifiedByPersonAliasId;
                            communication.CreatedByPersonAliasId = group.ModifiedByPersonAliasId;
                            communicationService.Add( communication );

                            communication.EnabledLavaCommands = enabledLavaCommands;

                            var personIdHash = new HashSet<int>();
                            foreach ( var member in groupMembers )
                            {
                                if ( !personIdHash.Contains( member.PersonId ) )
                                {
                                    personIdHash.Add( member.PersonId );
                                    var communicationRecipient = new CommunicationRecipient();
                                    communicationRecipient.PersonAliasId = member.Person.PrimaryAliasId.Value;
                                    communicationRecipient.AdditionalMergeValues = new Dictionary<string, object>();
                                    communicationRecipient.AdditionalMergeValues.Add( "GroupMember", member );
                                    //communicationRecipient.AdditionalMergeValues.Add( "Group", group );
                                    communication.Recipients.Add( communicationRecipient );
                                }
                            }

                            communication.IsBulkCommunication = false;
                            communication.CommunicationType = CommunicationType.Email;
                            communication.CommunicationTemplateId = null;

                            foreach ( var recipient in communication.Recipients )
                            {
                                recipient.MediumEntityTypeId = emailMediumType.Id;
                            }

                            communication.FromEmail = fromEmail;
                            communication.FromName = fromName;
                            communication.Subject = subject;
                            communication.Message = message;
                            communication.Status = CommunicationStatus.Approved;

                            rockContext.SaveChanges();

                            Rock.Model.Communication.Send( communication );

                            communicationsSent += personIdHash.Count;

                            var recurrence = new AttributeValueService( rockContext )
                                .GetByAttributeIdAndEntityId( recurrenceAttributeId, attributeMatrixItemAndGroupId.Key )
                                .Value;

                            if ( !string.IsNullOrWhiteSpace( recurrence ) )
                            {
                                var sendDate = new AttributeValueService( rockContext )
                                    .GetByAttributeIdAndEntityId( dateAttributeId, attributeMatrixItemAndGroupId.Key );

                                switch ( recurrence )
                                {
                                    case "Weekly":
                                        sendDate.Value = sendDate.ValueAsDateTime.Value.AddDays( 7 ).ToString();
                                        break;
                                    case "BiWeekly":
                                        sendDate.Value = sendDate.ValueAsDateTime.Value.AddDays( 14 ).ToString();
                                        break;
                                    case "Monthly":
                                        sendDate.Value = sendDate.ValueAsDateTime.Value.AddMonths( 1 ).ToString();
                                        break;
                                    default:
                                        break;
                                }
                                rockContext.SaveChanges();
                            }
                        }
                    }
                }

                if ( communicationsSent > 0 )
                {
                    context.Result = string.Format( "Sent {0} {1}", communicationsSent, "communication".PluralizeIf( communicationsSent > 1 ) );
                }
                else
                {
                    context.Result = "No communications to send";
                }
            }
            catch ( System.Exception ex )
            {
                HttpContext context2 = HttpContext.Current;
                ExceptionLogService.LogException( ex, context2 );
                throw;
            }
        }
    }
}
