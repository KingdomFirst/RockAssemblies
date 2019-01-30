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

using KFSConst = com.kfs.GroupScheduledEmails.SystemGuid;

namespace com.kfs.GroupScheduledEmails.Jobs
{
    [LavaCommandsField( "Enabled Lava Commands", "The Lava commands that should be enabled for this job.", false, order: 0 )]
    [IntegerField( "Command Timeout", "Maximum amount of time (in seconds) to wait for the each group communication creation to complete.", false, 180, "General", 1, "CommandTimeout" )]

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
            var enabledLavaCommands = dataMap.GetString( "EnabledLavaCommands" );
            var JobStartDateTime = RockDateTime.Now;
            var dateAttributes = new List<AttributeValue>();
            var dAttributeMatrixItemAndGroupIds = new Dictionary<int, int>(); // Key: AttributeMatrixItemId   Value: GroupId
            var exceptionMsgs = new List<string>();
            int communicationsSent = 0;
            var emailMediumType = EntityTypeCache.Get( "Rock.Communication.Medium.Email" );
            var dateAttribute = Rock.Web.Cache.AttributeCache.Get( KFSConst.Attribute.MATRIX_ATTRIBUTE_EMAIL_SEND_DATE.AsGuid() );
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
                        lastStartDateTime = job.LastRunDateTime?.AddSeconds( 0.0d - ( double ) job.LastRunDurationSeconds );
                    }
                    var beginDateTime = lastStartDateTime ?? JobStartDateTime.AddDays( -1 );

                    // get the date attributes
                    dateAttributes = new AttributeValueService( rockContext )
                        .Queryable().AsNoTracking()
                        .Where( d => d.AttributeId == dateAttribute.Id &&
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
                        /*
                        var attributeMatrixId = new AttributeMatrixItemService( rockContext )
                            .Queryable().AsNoTracking()
                            .FirstOrDefault( i => i.Id == d.EntityId.Value )
                            .AttributeMatrixId;

                        var attributeMatrixGuid = new AttributeMatrixService( rockContext )
                            .Queryable().AsNoTracking()
                            .FirstOrDefault( m => m.Id == attributeMatrixId )
                            .Guid
                            .ToString();
                        */

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
                        //try
                        //{
                            /*
                            var fromEmail = new AttributeValueService( rockContext )
                                .Queryable().AsNoTracking()
                                .FirstOrDefault( v => v.AttributeId == fromEmailAttributeId && v.EntityId == attributeMatrixItemAndGroupId.Key )
                                .Value;

                            var fromName = new AttributeValueService( rockContext )
                                .Queryable().AsNoTracking()
                                .FirstOrDefault( v => v.AttributeId == fromNameAttributeId && v.EntityId == attributeMatrixItemAndGroupId.Key )
                                .Value;

                            var subject = new AttributeValueService( rockContext )
                                .Queryable().AsNoTracking()
                                .FirstOrDefault( v => v.AttributeId == subjectAttributeId && v.EntityId == attributeMatrixItemAndGroupId.Key )
                                .Value;

                            var message = new AttributeValueService( rockContext )
                                .Queryable().AsNoTracking()
                                .FirstOrDefault( v => v.AttributeId == messageAttributeId && v.EntityId == attributeMatrixItemAndGroupId.Key )
                                .Value;
                            */

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

                            /*
                            var group = new GroupService( rockContext )
                                .Queryable().AsNoTracking()
                                .Where( g => g.Id == attributeMatrixItemAndGroupId.Value )
                                .FirstOrDefault();
                            */

                            var group = new GroupService( rockContext )
                                .GetNoTracking( attributeMatrixItemAndGroupId.Value );

                            if ( !message.IsNullOrWhiteSpace() && emailMediumType != null )
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
                                                    communicationRecipient.AdditionalMergeValues.Add( "Group", group );
                                                    communication.Recipients.Add( communicationRecipient );
                                                }
                                            }
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

                                    //var transaction = new Rock.Transactions.SendCommunicationTransaction();
                                    //transaction.CommunicationId = communication.Id;
                                    //transaction.PersonAlias = group.ModifiedByPersonAlias;
                                    //Rock.Transactions.RockQueue.TransactionQueue.Enqueue( transaction );

                                    communicationsSent = communicationsSent + personIdHash.Count;
                                }
                            }
                        //}
                        //catch ( Exception ex )
                        //{
                        //    exceptionMsgs.Add( string.Format( "Exception occurred sending message from group {0}:{1}    {2}", attributeMatrixItemAndGroupId.Value, Environment.NewLine, ex.Messages().AsDelimited( Environment.NewLine + "   " ) ) );
                        //    ExceptionLogService.LogException( ex, System.Web.HttpContext.Current );
                        //}
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

                //if ( exceptionMsgs.Any() )
                //{
                //    throw new Exception( "One or more exceptions occurred sending communications..." + Environment.NewLine + exceptionMsgs.AsDelimited( Environment.NewLine ) );
                //}
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
