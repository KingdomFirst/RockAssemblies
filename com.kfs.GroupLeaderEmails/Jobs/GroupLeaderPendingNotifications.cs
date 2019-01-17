using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Quartz;

using Rock;
using Rock.Attribute;
using Rock.Communication;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;
                                                             
namespace com.kfs.GroupLeaderEmails.Jobs
{
    /// <summary>
    /// Job send list of new pending group members to the group's leaders.
    /// </summary>
    /// 

    [GroupTypeField( "Group Type", "The group type to look for new pending registrations", true, "", "", 0 )]
    [BooleanField( "Include Previously Notified", "Includes pending group members that have already been notified.", false, "", 1 )]
    [SystemEmailField( "Notification Email", "", true, "", "", 2 )]
    [GroupRoleField( null, "Group Role Filter", "Optional group role to filter the pending members by. To select the role you'll need to select a group type.", false, null, null, 3 )]
    [IntegerField( "Pending Age", "The number of days since the record was last updated. This keeps the job from notifing all the pending registrations on first run.", false, 1, order: 4 )]
    [BooleanField( "Set Member Status Active", "This will change the group member status from pending to active after notification email is sent.", false, "", 5 )]
    [DisallowConcurrentExecution]
    public class GroupLeaderPendingNotifications : IJob
    {
        /// <summary> 
        /// Empty constructor for job initialization
        /// <para>
        /// Jobs require a public empty constructor so that the
        /// scheduler can instantiate the class whenever it needs.
        /// </para>
        /// </summary>
        public GroupLeaderPendingNotifications()
        {
        }

        /// <summary>
        /// Job that will sync groups.
        /// 
        /// Called by the <see cref="IScheduler" /> when a
        /// <see cref="ITrigger" /> fires that is associated with
        /// the <see cref="IJob" />.
        /// </summary>
        public virtual void Execute( IJobExecutionContext context )
        {
            JobDataMap dataMap = context.JobDetail.JobDataMap;

            try
            {
                int notificationsSent = 0;
                int pendingMembersCount = 0;

                // get groups set to sync
                var rockContext = new RockContext();

                Guid? groupTypeGuid = dataMap.GetString( "GroupType" ).AsGuidOrNull();
                Guid? systemEmailGuid = dataMap.GetString( "NotificationEmail" ).AsGuidOrNull();
                Guid? groupRoleFilterGuid = dataMap.GetString( "GroupRoleFilter" ).AsGuidOrNull();
                int? pendingAge = dataMap.GetString( "PendingAge" ).AsIntegerOrNull();


                bool includePreviouslyNotificed = dataMap.GetString( "IncludePreviouslyNotified" ).AsBoolean();
                bool changeToActive = dataMap.GetString( "SetMemberStatusActive" ).AsBoolean();

                // get system email
                var emailService = new SystemEmailService( rockContext );

                SystemEmail systemEmail = null;
                if ( systemEmailGuid.HasValue )
                {
                    systemEmail = emailService.Get( systemEmailGuid.Value );
                }

                if ( systemEmail == null )
                {
                    // no email specified, so nothing to do
                    return;
                }

                // get group members
                if ( groupTypeGuid.HasValue && groupTypeGuid != Guid.Empty )
                {
                    var qry = new GroupMemberService( rockContext ).Queryable( "Person, Group, Group.Members.GroupRole" )
                                            .Where( m => m.Group.GroupType.Guid == groupTypeGuid.Value
                                                && m.GroupMemberStatus == GroupMemberStatus.Pending );

                    if ( !includePreviouslyNotificed )
                    {
                        qry = qry.Where( m => m.IsNotified == false );
                    }

                    if ( groupRoleFilterGuid.HasValue )
                    {
                        qry = qry.Where( m => m.GroupRole.Guid == groupRoleFilterGuid.Value );
                    }

                    if ( pendingAge.HasValue && pendingAge.Value > 0 )
                    {
                        var ageDate = RockDateTime.Now.AddDays( pendingAge.Value * -1 );
                        qry = qry.Where( m => m.ModifiedDateTime > ageDate );
                    }

                    var pendingGroupMembers = qry.ToList();


                    var groups = pendingGroupMembers.GroupBy( m => m.Group );

                    foreach ( var groupKey in groups )
                    {
                        var group = groupKey.Key;

                        // get list of pending people
                        var qryPendingIndividuals = group.Members.Where( m => m.GroupMemberStatus == GroupMemberStatus.Pending );

                        if ( !includePreviouslyNotificed )
                        {
                            qryPendingIndividuals = qryPendingIndividuals.Where( m => m.IsNotified == false );
                        }

                        if ( groupRoleFilterGuid.HasValue )
                        {
                            qryPendingIndividuals = qryPendingIndividuals.Where( m => m.GroupRole.Guid == groupRoleFilterGuid.Value );
                        }

                        var pendingIndividuals = qryPendingIndividuals.Select( m => m.Person ).ToList();

                        // get list of leaders
                        var groupLeaders = group.Members.Where( m => m.GroupRole.IsLeader == true );

                        //var appRoot = Rock.Web.Cache.GlobalAttributesCache.Read( rockContext ).GetValue( "PublicApplicationRoot" );
                        var appRoot = GlobalAttributesCache.Get().GetValue( "PublicApplicationRoot" );

                        var recipients = new List<RecipientData>();
                        foreach ( var leader in groupLeaders.Where( l => l.Person != null && l.Person.Email != "" ) )
                        {
                            // create merge object
                            var mergeFields = new Dictionary<string, object>();
                            mergeFields.Add( "PendingIndividuals", pendingIndividuals );
                            mergeFields.Add( "Group", group );
                            mergeFields.Add( "ParentGroup", group.ParentGroup );
                            mergeFields.Add( "Person", leader.Person );
                            recipients.Add( new RecipientData( leader.Person.Email, mergeFields ) );
                        }

                        if ( pendingIndividuals.Count() > 0 )
                        {
                            var emailMessage = new RockEmailMessage( systemEmail.Guid );
                            emailMessage.SetRecipients( recipients );
                            emailMessage.Send();
                            notificationsSent += recipients.Count();
                        }

                        // mark pending members as notified as we go in case the job fails
                        var notifiedPersonIds = pendingIndividuals.Select( p => p.Id );
                        foreach ( var pendingGroupMember in pendingGroupMembers.Where( m => m.IsNotified == false && notifiedPersonIds.Contains( m.PersonId ) ) )
                        {
                            pendingGroupMember.IsNotified = true;
                            if ( changeToActive )
                            {
                                pendingGroupMember.GroupMemberStatus = GroupMemberStatus.Active;
                            }
                        }

                        rockContext.SaveChanges();

                    }
                }

                context.Result = string.Format( "Sent {0} emails to leaders for {1} pending individuals", notificationsSent, pendingMembersCount );
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
