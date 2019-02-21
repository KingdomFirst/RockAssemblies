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

using KFSConst = com.kfs.GroupScheduledSMS.SystemGuid;

namespace com.kfs.GroupScheduledSMS.Jobs
{
    [LavaCommandsField( "Enabled Lava Commands", "The Lava commands that should be enabled for this job.", false, order: 0 )]
    [IntegerField( "Command Timeout", "Maximum amount of time (in seconds) to wait for the each group communication creation to complete.", false, 180, "General", 1, "CommandTimeout" )]

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
            var enabledLavaCommands = dataMap.GetString( "EnabledLavaCommands" );
            var JobStartDateTime = RockDateTime.Now;
            var dateAttributes = new List<AttributeValue>();
            var dAttributeMatrixItemAndGroupIds = new Dictionary<int, int>(); // Key: AttributeMatrixItemId   Value: GroupId
            int communicationsSent = 0;
            var smsMediumType = EntityTypeCache.Get( "Rock.Communication.Medium.Sms" );
            var dateAttributeId = Rock.Web.Cache.AttributeCache.Get( KFSConst.Attribute.MATRIX_ATTRIBUTE_SMS_SEND_DATE.AsGuid() ).Id;
            var fromNumberAttributeId = Rock.Web.Cache.AttributeCache.Get( KFSConst.Attribute.MATRIX_ATTRIBUTE_SMS_FROM_NUMBER.AsGuid() ).Id;
            var messageAttributeId = Rock.Web.Cache.AttributeCache.Get( KFSConst.Attribute.MATRIX_ATTRIBUTE_SMS_MESSAGE.AsGuid() ).Id;

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
                        lastStartDateTime = job.LastRunDateTime?.AddSeconds( 0.0d - ( double ) job.LastRunDurationSeconds );
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

                        var fromNumberGuid = new AttributeValueService( rockContext )
                            .GetByAttributeIdAndEntityId( fromNumberAttributeId, attributeMatrixItemAndGroupId.Key )
                            .Value;
                        var fromNumber = DefinedValueCache.Get( fromNumberGuid.AsGuid() );

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
                                                communicationRecipient.PersonAliasId = person.PrimaryAliasId.Value;
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
                                communication.SMSFromDefinedValueId = fromNumber.Id;
                                communication.Subject = string.Empty;
                                communication.Status = CommunicationStatus.Approved;

                                rockContext.SaveChanges();

                                Rock.Model.Communication.Send( communication );

                                communicationsSent = communicationsSent + personIdHash.Count;
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
