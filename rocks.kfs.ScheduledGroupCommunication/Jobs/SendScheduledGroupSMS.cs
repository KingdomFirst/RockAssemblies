﻿// <copyright>
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
    /// Job to send scheduled group SMS messages.
    /// </summary>
    [DisallowConcurrentExecution]
    public class SendScheduledGroupSMS : IJob
    {
        /// <summary>
        /// Empty constructor for job initialization
        /// <para>
        /// Jobs require a public empty constructor so that the
        /// scheduler can instantiate the class whenever it needs.
        /// </para>
        /// </summary>
        public SendScheduledGroupSMS()
        {
        }

        /// <summary>
        /// Job that will send scheduled group SMS messages.
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
            var smsMediumType = EntityTypeCache.Get( "Rock.Communication.Medium.Sms" );
            var dateAttributeId = Rock.Web.Cache.AttributeCache.Get( KFSConst.Attribute.MATRIX_ATTRIBUTE_SMS_SEND_DATE.AsGuid() ).Id;
            var recurrenceAttributeId = Rock.Web.Cache.AttributeCache.Get( KFSConst.Attribute.MATRIX_ATTRIBUTE_SMS_SEND_RECURRENCE.AsGuid() ).Id;
            var fromNumberAttributeId = Rock.Web.Cache.AttributeCache.Get( KFSConst.Attribute.MATRIX_ATTRIBUTE_SMS_FROM_NUMBER_SYSTEM_PHONE.AsGuid() ).Id;
            var messageAttributeId = Rock.Web.Cache.AttributeCache.Get( KFSConst.Attribute.MATRIX_ATTRIBUTE_SMS_MESSAGE.AsGuid() ).Id;
            var groupEntityTypeId = EntityTypeCache.Get( Rock.SystemGuid.EntityType.GROUP.AsGuid() ).Id;

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
                        rockContext.Database.CommandTimeout = commandTimeout;

                        var attributeMatrixId = new AttributeMatrixItemService( rockContext )
                            .GetNoTracking( d.EntityId.Value )
                            .AttributeMatrixId;

                        var attributeMatrixGuid = new AttributeMatrixService( rockContext )
                            .GetNoTracking( attributeMatrixId )
                            .Guid
                            .ToString();

                        var attributeValue = new AttributeValueService( rockContext )
                            .Queryable().AsNoTracking()
                            .Where( av => av.EntityId.HasValue && av.Attribute.EntityTypeId.Value.Equals( groupEntityTypeId ) )
                            .GroupBy( av => av.Value )
                            .Select( av => new { EntityId = av.Max( v => v.EntityId.Value ), Value = av.Key } )
                            .FirstOrDefault( av => av.Value.Equals( attributeMatrixGuid, StringComparison.CurrentCultureIgnoreCase ) );

                        if ( attributeValue != null )
                        {
                            dAttributeMatrixItemAndGroupIds.Add( d.EntityId.Value, attributeValue.EntityId );
                        }
                    }
                }

                foreach ( var attributeMatrixItemAndGroupId in dAttributeMatrixItemAndGroupIds )
                {
                    // Use a new context to limit the amount of change-tracking required
                    using ( var rockContext = new RockContext() )
                    {
                        rockContext.Database.CommandTimeout = commandTimeout;

                        var fromNumberGuid = new AttributeValueService( rockContext )
                            .GetByAttributeIdAndEntityId( fromNumberAttributeId, attributeMatrixItemAndGroupId.Key )
                            .Value;
                        var fromNumber = SystemPhoneNumberCache.Get( fromNumberGuid.AsGuid() );

                        var message = new AttributeValueService( rockContext )
                            .GetByAttributeIdAndEntityId( messageAttributeId, attributeMatrixItemAndGroupId.Key )
                            .Value;

                        var attachments = new List<BinaryFile>();

                        var group = new GroupService( rockContext )
                            .GetNoTracking( attributeMatrixItemAndGroupId.Value );

                        if ( !message.IsNullOrWhiteSpace() && smsMediumType != null )
                        {
                            var recipients = new GroupMemberService( rockContext )
                                .GetByGroupId( attributeMatrixItemAndGroupId.Value ).AsNoTracking()
                                .Where( m => m.GroupMemberStatus == GroupMemberStatus.Active )
                                .ToList();

                            if ( recipients.Any() )
                            {
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
                                foreach ( var groupMember in recipients )
                                {
                                    // Use a new context to limit the amount of change-tracking required
                                    using ( var groupMemberContext = new RockContext() )
                                    {
                                        if ( !personIdHash.Contains( groupMember.PersonId ) )
                                        {
                                            var person = new PersonService( groupMemberContext )
                                                .GetNoTracking( groupMember.PersonId );

                                            if ( person != null && person.PrimaryAliasId.HasValue )
                                            {
                                                personIdHash.Add( groupMember.PersonId );
                                                var communicationRecipient = new CommunicationRecipient();
                                                communicationRecipient.PersonAliasId = person.PrimaryAliasId;
                                                communicationRecipient.AdditionalMergeValues = new Dictionary<string, object>();
                                                communicationRecipient.AdditionalMergeValues.Add( "GroupMember", groupMember );
                                                //communicationRecipient.AdditionalMergeValues.Add( "Group", group );
                                                communication.Recipients.Add( communicationRecipient );
                                            }
                                        }
                                    }
                                }

                                communication.IsBulkCommunication = false;
                                communication.CommunicationType = CommunicationType.SMS;
                                communication.CommunicationTemplateId = null;

                                foreach ( var recipient in communication.Recipients )
                                {
                                    recipient.MediumEntityTypeId = smsMediumType.Id;
                                }

                                communication.SMSMessage = message;
                                communication.SmsFromSystemPhoneNumberId = fromNumber.Id;
                                communication.Subject = string.Empty;
                                communication.Status = CommunicationStatus.Approved;

                                rockContext.SaveChanges();

                                Rock.Model.Communication.Send( communication );

                                communicationsSent = communicationsSent + personIdHash.Count;

                                var recurrence = new AttributeValueService( rockContext )
                                    .GetByAttributeIdAndEntityId( recurrenceAttributeId, attributeMatrixItemAndGroupId.Key );

                                if ( recurrence != null && !string.IsNullOrWhiteSpace( recurrence.Value ) )
                                {
                                    var sendDate = new AttributeValueService( rockContext )
                                        .GetByAttributeIdAndEntityId( dateAttributeId, attributeMatrixItemAndGroupId.Key );

                                    switch ( recurrence.Value )
                                    {
                                        case "1":
                                            sendDate.Value = sendDate.ValueAsDateTime.Value.AddDays( 7 ).ToString();
                                            break;
                                        case "2":
                                            sendDate.Value = sendDate.ValueAsDateTime.Value.AddDays( 14 ).ToString();
                                            break;
                                        case "3":
                                            sendDate.Value = sendDate.ValueAsDateTime.Value.AddMonths( 1 ).ToString();
                                            break;
                                        case "4":
                                            sendDate.Value = sendDate.ValueAsDateTime.Value.AddDays( 1 ).ToString();
                                            break;
                                        default:
                                            break;
                                    }
                                    rockContext.SaveChanges();
                                }
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
