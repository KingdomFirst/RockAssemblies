// <copyright>
// Copyright by the Spark Development Network
//
// Licensed under the Rock Community License (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.rockrms.com/license
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//
// <notice>
// This file contains modifications by Kingdom First Solutions
// and is a derivative work.
//
// Modification (including but not limited to):
// * Ability to email a specific role or staff member instead of leader.
// </notice>
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

namespace rocks.kfs.GroupRoleAttendanceReminder.Jobs
{
    /// <summary>
    /// Job to process communications
    /// </summary>
    [GroupRoleField( null, "Group Role to Send to", "The Group Role the attendance reminders will be sent to, attendance reminders will be sent for the groups of the parent type of role.", true, "", "", 0 )]
    [SystemCommunicationField( "System Email", "The system email to use when sending reminder.", true, Rock.SystemGuid.SystemCommunication.GROUP_ATTENDANCE_REMINDER, "", 1 )]
    [TextField( "Send Reminders", "Comma delimited list of days after a group meets to send a reminder. For example, a value of '2,4' would result in a reminder getting sent two and four days after group meets if attendance was not entered.", true, "", "", 2 )]
    [EmailField( "Staff Email", "Staff email address to send to if no member with specified role is in the group or parent structure.", false, "", "", 4 )]
    [DisallowConcurrentExecution]
    public class GroupRoleSendAttendanceReminder : IJob
    {
        private int attendanceRemindersSent = 0;
        private int errorCount = 0;
        private List<string> errorMessages = new List<string>();
        private Guid systemEmailGuid = Guid.Empty;
        private List<int> groupsNotified = new List<int>();
        private string staffEmail = "";

        /// <summary>
        /// Initializes a new instance of the <see cref="SendCommunications"/> class.
        /// </summary>
        public GroupRoleSendAttendanceReminder()
        {
        }

        /// <summary>
        /// Executes the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        public virtual void Execute( IJobExecutionContext context )
        {
            JobDataMap dataMap = context.JobDetail.JobDataMap;

            Guid? groupRoleGuid = dataMap.GetString( "GroupRoletoSendto" ).AsGuidOrNull();

            if ( !groupRoleGuid.HasValue || groupRoleGuid == Guid.Empty )
            {
                context.Result = "Job failed. Unable to find group role/type";
                throw new Exception( "No group role/type found" );
            }

            systemEmailGuid = dataMap.GetString( "SystemEmail" ).AsGuid();
            staffEmail = dataMap.GetString( "StaffEmail" );

            var groupTypeRoleService = new GroupTypeRoleService( new RockContext() );
            var groupRole = groupTypeRoleService.Get( groupRoleGuid.Value );
            var groupType = GroupTypeCache.Get( groupRole.GroupType );

            if ( groupType.TakesAttendance && groupType.SendAttendanceReminder )
            {
                // Get the occurrence dates that apply
                var dates = new List<DateTime>();
                // This job was specifically requested for future reminders only.
                //dates.Add( RockDateTime.Today );
                try
                {
                    string[] reminderDays = dataMap.GetString( "SendReminders" ).Split( ',' );
                    foreach ( string reminderDay in reminderDays )
                    {
                        if ( reminderDay.Trim().IsNotNullOrWhiteSpace() )
                        {
                            var reminderDate = RockDateTime.Today.AddDays( 0 - Convert.ToInt32( reminderDay ) );
                            if ( !dates.Contains( reminderDate ) )
                            {
                                dates.Add( reminderDate );
                            }
                        }
                    }
                }
                catch { }

                var rockContext = new RockContext();
                var groupService = new GroupService( rockContext );
                var groupMemberService = new GroupMemberService( rockContext );
                var scheduleService = new ScheduleService( rockContext );
                var attendanceOccurrenceService = new AttendanceOccurrenceService( rockContext );

                var startDate = dates.Min();
                var endDate = dates.Max().AddDays( 1 );

                // Find all 'occurrences' for the groups that occur on the affected dates
                var occurrences = new Dictionary<int, List<DateTime>>();
                foreach ( var group in groupService
                    .Queryable( "Schedule" ).AsNoTracking()
                    .Where( g =>
                        g.GroupTypeId == groupType.Id &&
                        g.IsActive &&
                        g.Schedule != null &&
                        g.Members.Any( m =>
                            m.GroupMemberStatus == GroupMemberStatus.Active &&
                            m.GroupRole.IsLeader &&
                            m.Person.Email != null &&
                            m.Person.Email != String.Empty ) ) )
                {
                    // Add the group
                    occurrences.Add( group.Id, new List<DateTime>() );

                    // Check for a iCal schedule
                    if ( !string.IsNullOrWhiteSpace( group.Schedule.iCalendarContent ) )
                    {
                        // If schedule has an iCal schedule, get occurrences between first and last dates
                        foreach ( var occurrence in group.Schedule.GetOccurrences( startDate, endDate ) )
                        {
                            var startTime = occurrence.Period.StartTime.Value;
                            if ( dates.Contains( startTime.Date ) )
                            {
                                occurrences[group.Id].Add( startTime );
                            }
                        }
                    }
                    else
                    {
                        // if schedule does not have an iCal, then check for weekly schedule and calculate occurrences starting with first attendance or current week
                        if ( group.Schedule.WeeklyDayOfWeek.HasValue )
                        {
                            foreach ( var date in dates )
                            {
                                if ( date.DayOfWeek == group.Schedule.WeeklyDayOfWeek.Value )
                                {
                                    var startTime = date;
                                    if ( group.Schedule.WeeklyTimeOfDay.HasValue )
                                    {
                                        startTime = startTime.Add( group.Schedule.WeeklyTimeOfDay.Value );
                                    }
                                    occurrences[group.Id].Add( startTime );
                                }
                            }
                        }
                    }
                }

                // Remove any occurrences during group type exclusion date ranges
                foreach ( var exclusion in groupType.GroupScheduleExclusions )
                {
                    if ( exclusion.Start.HasValue && exclusion.End.HasValue )
                    {
                        foreach ( var keyVal in occurrences )
                        {
                            foreach ( var occurrenceDate in keyVal.Value.ToList() )
                            {
                                if ( occurrenceDate >= exclusion.Start.Value &&
                                    occurrenceDate < exclusion.End.Value.AddDays( 1 ) )
                                {
                                    keyVal.Value.Remove( occurrenceDate );
                                }
                            }
                        }
                    }
                }

                // Remove any 'occurrences' that already have attendance data entered
                foreach ( var occurrence in attendanceOccurrenceService
                    .Queryable().AsNoTracking()
                    .Where( a =>
                        a.OccurrenceDate >= startDate &&
                        a.OccurrenceDate < endDate &&
                        a.GroupId.HasValue &&
                        occurrences.Keys.Contains( a.GroupId.Value ) &&
                        a.ScheduleId.HasValue &&
                        ( a.Attendees.Any() || ( a.DidNotOccur.HasValue && a.DidNotOccur.Value ) ) )
                    .Select( a => new
                    {
                        GroupId = a.GroupId.Value,
                        a.OccurrenceDate
                    } )
                    .Distinct()
                    .ToList() )
                {
                    occurrences[occurrence.GroupId].RemoveAll( d => d.Date == occurrence.OccurrenceDate.Date );
                }

                // Get the groups that have occurrences
                var groupIds = occurrences.Where( o => o.Value.Any() ).Select( o => o.Key ).ToList();

                // Get the members with the specific role of those groups
                var roleMembers = groupMemberService
                    .Queryable( "Group,Person" ).AsNoTracking()
                    .Where( m =>
                        groupIds.Contains( m.GroupId ) &&
                        m.GroupMemberStatus == GroupMemberStatus.Active &&
                        m.GroupRoleId.Equals( groupRole.Id ) &&
                        m.Person.Email != null &&
                        m.Person.Email != string.Empty )
                    .ToList();

                // Loop through the members with specific role
                foreach ( var member in roleMembers )
                {
                    foreach ( var group in occurrences.Where( o => o.Key == member.GroupId ) )
                    {
                        SendEmailToMember( member, member.Group, group );
                    }
                }

                var roleMembersGroupIds = roleMembers.Select( m => m.GroupId );

                var groupsWithoutRole = groupService
                    .Queryable().AsNoTracking()
                    .Where( g =>
                        groupIds.Contains( g.Id ) &&
                        !roleMembersGroupIds.Contains( g.Id ) )
                    .ToList();

                foreach ( var group in groupsWithoutRole )
                {
                    if ( group.ParentGroupId.HasValue )
                    {
                        SendToParentGroupRole( group, group.ParentGroup, groupRole, occurrences, groupMemberService, staffEmail );
                    }
                }

                var groupsToNotifyStaff = groupsWithoutRole
                    .Where( g => !groupsNotified.Contains( g.Id ) )
                    .ToList();

                foreach ( var group in groupsToNotifyStaff )
                {
                    foreach ( var occGroup in occurrences.Where( o => o.Key == group.Id ) )
                    {
                        SendEmailToMember( null, group, occGroup );
                    }
                }
            }

            context.Result = string.Format( "{0} attendance reminders sent", attendanceRemindersSent );
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

        private void SendToParentGroupRole( Group group, Group parentGroup, GroupTypeRole groupRole, Dictionary<int, List<DateTime>> occurrences, GroupMemberService groupMemberService, string staffEmail )
        {
            // Get the members with the specific role of those groups
            var roleMembers = groupMemberService
                    .Queryable( "Group,Person" ).AsNoTracking()
                    .Where( m =>
                        m.GroupId.Equals( parentGroup.Id ) &&
                        m.GroupMemberStatus == GroupMemberStatus.Active &&
                        m.GroupRoleId.Equals( groupRole.Id ) &&
                        m.Person.Email != null &&
                        m.Person.Email != string.Empty )
                    .ToList();

            if ( roleMembers.Count > 0 )
            {
                // Loop through the members with specific role
                foreach ( var member in roleMembers )
                {
                    foreach ( var occGroup in occurrences.Where( o => o.Key == group.Id ) )
                    {
                        SendEmailToMember( member, group, occGroup );
                    }
                }
            }
            else
            {
                if ( parentGroup.ParentGroupId.HasValue )
                {
                    SendToParentGroupRole( group, parentGroup.ParentGroup, groupRole, occurrences, groupMemberService, staffEmail );
                }
            }
        }

        private void SendEmailToMember( GroupMember member, Group group, KeyValuePair<int, List<DateTime>> occGroup )
        {
            var email = staffEmail;
            if ( member.IsNotNull() )
            {
                email = member.Person.Email;
            }

            if ( email.IsNotNullOrWhiteSpace() )
            {
                groupsNotified.Add( group.Id );

                var mergeObjects = Rock.Lava.LavaHelper.GetCommonMergeFields( null, member.IsNotNull() ? member.Person : null );
                mergeObjects.Add( "Person", member.IsNotNull() ? member.Person : null );
                mergeObjects.Add( "Group", group );
                mergeObjects.Add( "Occurrence", occGroup.Value.Max() );

                var recipients = new List<RockEmailMessageRecipient>();
                recipients.Add( RockEmailMessageRecipient.CreateAnonymous( email, mergeObjects ) );

                var emailMessage = new RockEmailMessage( systemEmailGuid );
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
                    attendanceRemindersSent++;
                }
            }
            else
            {
                errorCount += 1;
                errorMessages.Add( string.Format( "No email specified for group {0} and no fallback email provided.", group.Id ) );
            }
        }
    }
}