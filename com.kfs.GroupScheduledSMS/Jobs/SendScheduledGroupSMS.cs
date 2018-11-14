using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

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

            var dGroupAndAttributeMatrixItemIds = new Dictionary<int, int>();
            var rockContext = new RockContext();
            var exceptionMsgs = new List<string>();
            int communicationsSent = 0;
            var smsMediumType = EntityTypeCache.Get( "Rock.Communication.Medium.Sms" );

            // get the last run date or yesterday
            DateTime? lastSuccessfulRunDateTime = null;
            var jobId = context.JobDetail.Description.AsInteger();
            var jobService = new ServiceJobService( rockContext );
            var job = jobService.Get( jobId );
            if ( job != null && job.Guid != Rock.SystemGuid.ServiceJob.JOB_PULSE.AsGuid() )
            {
                lastSuccessfulRunDateTime = job.LastSuccessfulRunDateTime;
            }
            var beginDateTime = lastSuccessfulRunDateTime ?? RockDateTime.Now.AddDays( -1 );

            // get the date attributes
            var dateAttribute = AttributeCache.Get( KFSConst.Attribute.MATRIX_ATTRIBUTE_SMS_SEND_DATE.AsGuid() );
            var dateAttributes = new AttributeValueService( rockContext ).Queryable()
                                        .Where( d => d.AttributeId == dateAttribute.Id &&
                                                d.ValueAsDateTime >= beginDateTime && d.ValueAsDateTime <= RockDateTime.Now )
                                        .ToList();

            foreach ( var d in dateAttributes )
            {
                if ( d.EntityId.HasValue )
                {
                    var attributeMatrixId = new AttributeMatrixItemService( rockContext ).Queryable().FirstOrDefault( i => i.Id == d.EntityId.Value ).AttributeMatrixId;
                    var attributeMatrixGuid = new AttributeMatrixService( rockContext ).Queryable().FirstOrDefault( m => m.Id == attributeMatrixId ).Guid.ToString();
                    var group = new AttributeValueService( rockContext ).Queryable().FirstOrDefault( a => a.Value.Equals( attributeMatrixGuid, StringComparison.CurrentCultureIgnoreCase ) );
                    if ( group != null && group.EntityId.HasValue )
                    {
                        dGroupAndAttributeMatrixItemIds.Add( d.EntityId.Value, group.EntityId.Value );
                    }
                }
            }

            foreach ( var groupAndAttributeMatrixItemId in dGroupAndAttributeMatrixItemIds )
            {
                try
                {
                    var fromNumberAttributeId = AttributeCache.Get( KFSConst.Attribute.MATRIX_ATTRIBUTE_SMS_FROM_NUMBER.AsGuid() ).Id;
                    var fromNumberGuid = new AttributeValueService( rockContext ).Queryable()
                                                .FirstOrDefault( v => v.AttributeId == fromNumberAttributeId &&
                                                        v.EntityId == groupAndAttributeMatrixItemId.Key ).Value;
                    var fromNumber = DefinedValueCache.Get( fromNumberGuid.AsGuid() );

                    var messageAttributeId = AttributeCache.Get( KFSConst.Attribute.MATRIX_ATTRIBUTE_SMS_MESSAGE.AsGuid() ).Id;
                    var message = new AttributeValueService( rockContext ).Queryable()
                                                .FirstOrDefault( v => v.AttributeId == messageAttributeId &&
                                                        v.EntityId == groupAndAttributeMatrixItemId.Key ).Value;

                    var attachments = new List<BinaryFile>();

                    var group = new GroupService( rockContext ).Queryable().Where( g => g.Id == groupAndAttributeMatrixItemId.Value ).FirstOrDefault();

                    if ( !message.IsNullOrWhiteSpace() && smsMediumType != null )
                    {
                        IQueryable<GroupMember> qry = new GroupMemberService( rockContext ).GetByGroupId( groupAndAttributeMatrixItemId.Value ).AsNoTracking();

                        if ( qry != null )
                        {
                            var recipients = new List<GroupMember>();
                            recipients = qry
                                .Where( m => m.GroupMemberStatus == GroupMemberStatus.Active )
                                .ToList();

                            var communicationService = new CommunicationService( rockContext );

                            var communication = new Rock.Model.Communication();
                            communication.Status = CommunicationStatus.Transient;
                            communication.ReviewedDateTime = RockDateTime.Now;
                            communication.ReviewerPersonAliasId = group.ModifiedByPersonAliasId;
                            communication.SenderPersonAliasId = group.ModifiedByPersonAliasId;
                            communication.CreatedByPersonAliasId = group.ModifiedByPersonAliasId;
                            communicationService.Add( communication );

                            communication.EnabledLavaCommands = dataMap.GetString( "EnabledLavaCommands" );
                            var personIdHash = new HashSet<int>();
                            foreach ( var groupMember in recipients )
                            {
                                if ( !personIdHash.Contains( groupMember.PersonId ) )
                                {
                                    var person = new PersonService( rockContext ).Get( groupMember.PersonId );
                                    if ( person != null )
                                    {
                                        personIdHash.Add( groupMember.PersonId );
                                        var communicationRecipient = new CommunicationRecipient();
                                        communicationRecipient.PersonAlias = person.PrimaryAlias;
                                        communicationRecipient.AdditionalMergeValues = new Dictionary<string, object>();
                                        communicationRecipient.AdditionalMergeValues.Add( "GroupMember", groupMember );
                                        communicationRecipient.AdditionalMergeValues.Add( "Group", group );
                                        communication.Recipients.Add( communicationRecipient );
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

                            var transaction = new Rock.Transactions.SendCommunicationTransaction();
                            transaction.CommunicationId = communication.Id;
                            transaction.PersonAlias = group.ModifiedByPersonAlias;
                            Rock.Transactions.RockQueue.TransactionQueue.Enqueue( transaction );

                            communicationsSent = communicationsSent + personIdHash.Count;
                        }
                    }
                }
                catch ( Exception ex )
                {
                    exceptionMsgs.Add( string.Format( "Exception occurred sending message from group {0}:{1}    {2}", groupAndAttributeMatrixItemId.Value, Environment.NewLine, ex.Messages().AsDelimited( Environment.NewLine + "   " ) ) );
                    ExceptionLogService.LogException( ex, System.Web.HttpContext.Current );
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

            if ( exceptionMsgs.Any() )
            {
                throw new Exception( "One or more exceptions occurred sending communications..." + Environment.NewLine + exceptionMsgs.AsDelimited( Environment.NewLine ) );
            }
        }
    }
}
