using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

using Quartz;

using Rock;
using Rock.Communication;
using Rock.Data;
using Rock.Model;

using KFSConst = com.kfs.GroupScheduledEmails.SystemGuid;

namespace com.kfs.GroupScheduledEmails.Jobs
{
    /// <summary>
    /// Job to send scheduled group emails.
    /// </summary>
    [DisallowConcurrentExecution]
    class SendScheduledGroupEmail : IJob
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
            var dGroupAndAttributeMatrixItemIds = new Dictionary<int, int>();
            var rockContext = new RockContext();
            var exceptionMsgs = new List<string>();
            int communicationsSent = 0;

            // get the last run date or yesterday
            DateTime? lastRunDateTime = null;
            var jobId = context.JobDetail.Description.AsInteger();
            var jobService = new ServiceJobService( rockContext );
            var job = jobService.Get( jobId );
            if ( job != null && job.Guid != Rock.SystemGuid.ServiceJob.JOB_PULSE.AsGuid() )
            {
                lastRunDateTime = job.LastRunDateTime;
            }
            var beginDateTime = lastRunDateTime ?? RockDateTime.Now.AddDays( -1 );

            // get the date attributes
            var dateAttribute = Rock.Web.Cache.AttributeCache.Read( KFSConst.Attribute.MATRIX_ATTRIBUTE_EMAIL_SEND_DATE.AsGuid() );
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
                        dGroupAndAttributeMatrixItemIds.Add( groupId.Value, d.EntityId.Value );
                    }
                }
            }

            foreach ( var groupAndAttributeMatrixItemId in dGroupAndAttributeMatrixItemIds )
            {
                try
                {
                    var fromEmailAttributeId = Rock.Web.Cache.AttributeCache.Read( KFSConst.Attribute.MATRIX_ATTRIBUTE_EMAIL_FROM_EMAIL.AsGuid() ).Id;
                    var fromEmail = new AttributeValueService( rockContext ).Queryable()
                                                .FirstOrDefault( v => v.AttributeId == fromEmailAttributeId &&
                                                        v.EntityId == groupAndAttributeMatrixItemId.Value ).Value;

                    var fromNameAttributeId = Rock.Web.Cache.AttributeCache.Read( KFSConst.Attribute.MATRIX_ATTRIBUTE_EMAIL_FROM_NAME.AsGuid() ).Id;
                    var fromName = new AttributeValueService( rockContext ).Queryable()
                                                .FirstOrDefault( v => v.AttributeId == fromNameAttributeId &&
                                                        v.EntityId == groupAndAttributeMatrixItemId.Value ).Value;

                    var subjectAttributeId = Rock.Web.Cache.AttributeCache.Read( KFSConst.Attribute.MATRIX_ATTRIBUTE_EMAIL_SUBJECT.AsGuid() ).Id;
                    var subject = new AttributeValueService( rockContext ).Queryable()
                                                .FirstOrDefault( v => v.AttributeId == subjectAttributeId &&
                                                        v.EntityId == groupAndAttributeMatrixItemId.Value ).Value;

                    var messageAttributeId = Rock.Web.Cache.AttributeCache.Read( KFSConst.Attribute.MATRIX_ATTRIBUTE_EMAIL_MESSAGE.AsGuid() ).Id;
                    var message = new AttributeValueService( rockContext ).Queryable()
                                                .FirstOrDefault( v => v.AttributeId == messageAttributeId &&
                                                        v.EntityId == groupAndAttributeMatrixItemId.Value ).Value;

                    var attachments = new List<BinaryFile>();

                    var group = new GroupService( rockContext ).Queryable().Where( g => g.Id == groupAndAttributeMatrixItemId.Key ).FirstOrDefault();

                    var mergeFields = new Dictionary<string, object>()
                    {
                        {"Group", group}
                    };

                    IQueryable<GroupMember> qry = new GroupMemberService( rockContext ).GetByGroupId( groupAndAttributeMatrixItemId.Key );

                    if ( qry != null )
                    {
                        foreach ( var person in qry
                            .Where( m => m.GroupMemberStatus == GroupMemberStatus.Active )
                            .Select( m => m.Person ) )
                        {
                            if ( person.IsEmailActive &&
                                person.EmailPreference != EmailPreference.DoNotEmail &&
                                !string.IsNullOrWhiteSpace( person.Email ) )
                            {
                                var personDict = new Dictionary<string, object>( mergeFields );
                                personDict.Add( "Person", person );
                                Send( person.Email, fromEmail, fromName, subject, message, personDict, rockContext, true, attachments );
                                communicationsSent++;
                            }
                        }
                    }


                }

                catch ( Exception ex )
                {
                    exceptionMsgs.Add( string.Format( "Exception occurred sending message from group {0}:{1}    {2}", groupAndAttributeMatrixItemId.Key, Environment.NewLine, ex.Messages().AsDelimited( Environment.NewLine + "   " ) ) );
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

        private void Send( string recipients, string fromEmail, string fromName, string subject, string body, Dictionary<string, object> mergeFields, RockContext rockContext, bool createCommunicationRecord, List<BinaryFile> attachments )
        {
            var emailMessage = new RockEmailMessage();

            foreach ( string recipient in recipients.SplitDelimitedValues().ToList() )
            {
                emailMessage.AddRecipient( new RecipientData( recipient, mergeFields ) );
            }
            emailMessage.FromEmail = fromEmail;
            emailMessage.FromName = fromName;
            emailMessage.Subject = subject;
            emailMessage.Message = body;

            if ( attachments.Count > 0 )
            {
                emailMessage.Attachments = attachments;
            }

            emailMessage.CreateCommunicationRecord = createCommunicationRecord;
            emailMessage.AppRoot = Rock.Web.Cache.GlobalAttributesCache.Read().GetValue( "InternalApplicationRoot" ) ?? string.Empty;

            emailMessage.Send();
        }
    }
}
