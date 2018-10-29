using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

using Quartz;

using Rock;
using Rock.Communication;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;

using KFSConst = com.kfs.GroupScheduledSMS.SystemGuid;

namespace com.kfs.GroupScheduledSMS.Jobs
{
    /// <summary>
    /// Job to send scheduled group SMS messages.
    /// </summary>
    [DisallowConcurrentExecution]
    class SendScheduledGroupSMS : IJob
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
            var dGroupAndAttributeMatrixItemIds = new Dictionary<int, int>();
            var rockContext = new RockContext();
            var exceptionMsgs = new List<string>();
            int communicationsSent = 0;

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
                    var groupId = new AttributeValueService( rockContext ).Queryable().FirstOrDefault( a => a.Value.Equals( attributeMatrixGuid, StringComparison.CurrentCultureIgnoreCase ) ).EntityId;
                    if ( groupId.HasValue )
                    {
                        dGroupAndAttributeMatrixItemIds.Add( d.EntityId.Value, groupId.Value );
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

                    var mergeFields = new Dictionary<string, object>()
                    {
                        {"Group", group}
                    };

                    if ( !message.IsNullOrWhiteSpace() )
                    {
                        IQueryable<GroupMember> qry = new GroupMemberService( rockContext ).GetByGroupId( groupAndAttributeMatrixItemId.Value ).AsNoTracking();

                        if ( qry != null )
                        {
                            foreach ( var person in qry
                                .Where( m => m.GroupMemberStatus == GroupMemberStatus.Active )
                                .Select( m => m.Person ) )
                            {
                                Guid personAliasGuid = person.PrimaryAlias.Guid;
                                if ( !personAliasGuid.IsEmpty() )
                                {
                                    var phoneNumber = new PersonAliasService( rockContext ).Queryable()
                                        .Where( a => a.Guid.Equals( personAliasGuid ) )
                                        .SelectMany( a => a.Person.PhoneNumbers )
                                        .Where( p => p.IsMessagingEnabled )
                                        .FirstOrDefault();

                                    if ( phoneNumber != null )
                                    {
                                        string smsNumber = phoneNumber.Number;
                                        if ( !string.IsNullOrWhiteSpace( phoneNumber.CountryCode ) )
                                        {
                                            smsNumber = "+" + phoneNumber.CountryCode + phoneNumber.Number;
                                        }

                                        var recipient = new RecipientData( smsNumber, mergeFields );

                                        if ( person != null )
                                        {
                                            recipient.MergeFields.Add( "Person", person );
                                        }

                                        Send( fromNumber, recipient, message, attachments );
                                        communicationsSent++;
                                    }
                                }
                            }
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

        private void Send( DefinedValueCache fromNumber, RecipientData recipient, string message, List<BinaryFile> attachments )
        {
            var smsMessage = new RockSMSMessage();
            smsMessage.FromNumber = fromNumber;
            smsMessage.AddRecipient( recipient );
            smsMessage.Message = message;

            if ( attachments.Count > 0 )
            {
                smsMessage.Attachments = attachments;
            }

            smsMessage.Send();
        }
    }
}
