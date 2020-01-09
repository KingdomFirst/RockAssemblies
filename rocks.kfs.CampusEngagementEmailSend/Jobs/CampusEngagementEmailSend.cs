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

namespace rocks.kfs.CampusEngagementEmailSend.Jobs
{
    /// <summary>
    /// Job to process communications
    /// </summary>
    [GroupTypesField( "Group Types", "The group types to include attendance numbers for in the campus engagement email.", true, "", "", 0 )]
    [SystemEmailField( "System Email", "The system email to use when sending the campus engagement email.", true, "", "", 1 )]
    [DisallowConcurrentExecution]
    public class CampusEngagementEmailSend : IJob
    {
        private int engagementEmailsSent = 0;
        private int errorCount = 0;

        private List<string> errorMessages = new List<string>();
        private Guid systemEmailGuid = Guid.Empty;
        private List<GroupType> groupTypes = new List<GroupType>();     
        //private string staffEmail = "";

        /// <summary>
        /// Initializes a new instance of the <see cref="SendCommunications"/> class.
        /// </summary>
        public CampusEngagementEmailSend()
        {
        }

        /// <summary>
        /// Executes the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        public virtual void Execute( IJobExecutionContext context )
        {
            JobDataMap dataMap = context.JobDetail.JobDataMap;

            var groupTypeService = new GroupTypeService( new RockContext() );

            groupTypes = new List<GroupType>( groupTypeService.GetByGuids( dataMap.Get( "GroupTypes" ).ToString().Split(',').Select(Guid.Parse).ToList() ) );


            //groupTypes = ( dataMap.Get( "GroupTypes" ) as IEnumerable<Guid> ).ToList();

            //if ( !groupRoleGuid.HasValue || groupRoleGuid == Guid.Empty )
            //{
            //    context.Result = "Job failed. Unable to find group role/type";
            //    throw new Exception( "No group role/type found" );
            //}

            systemEmailGuid = dataMap.GetString( "SystemEmail" ).AsGuid();

            foreach ( GroupType groupType in groupTypes )
            {                                                                  

                if ( groupType.TakesAttendance && groupType.SendAttendanceReminder )
                {
                    // Get the occurrence dates that apply
                    var dates = new List<DateTime>();                     

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
                        if ( exclusion.StartDate.HasValue && exclusion.EndDate.HasValue )
                        {
                            foreach ( var keyVal in occurrences )
                            {
                                foreach ( var occurrenceDate in keyVal.Value.ToList() )
                                {
                                    if ( occurrenceDate >= exclusion.StartDate.Value &&
                                        occurrenceDate < exclusion.EndDate.Value.AddDays( 1 ) )
                                    {
                                        keyVal.Value.Remove( occurrenceDate );
                                    }
                                }
                            }
                        }
                    }

                    // Get the groups that have occurrences
                    var groupIds = occurrences.Where( o => o.Value.Any() ).Select( o => o.Key ).ToList();
                }

                SendEmailToCampusPastors();

            }        

            context.Result = string.Format( "{0} emails sent", engagementEmailsSent );
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

        private void SendEmailToCampusPastors()
        {
            var rockContext = new RockContext();      
            var campuses = rockContext.Campuses.ToList();

            foreach( Campus campus in campuses )
            {
                var pastor = campus.LeaderPersonAlias.Person;
                var email = pastor.Email;   

                var groupService = new GroupService( rockContext );
                List<Group> allCampusGroups = new List<Group>();

                foreach( GroupType groupType in groupTypes )
                {
                    List<Group> groups = new List<Group>( groupService.GetByGroupTypeId( groupType.Id ) ).Where( g => g.CampusId == campus.Id ).ToList();
                    allCampusGroups.AddRange( groups );
                }
                
                   
                if ( email.IsNotNullOrWhiteSpace() )
                {
                    //groupsNotified.Add( group.Id );

                    var mergeObjects = Rock.Lava.LavaHelper.GetCommonMergeFields( null, pastor.IsNotNull() ? pastor : null );
                    mergeObjects.Add( "Person", pastor.IsNotNull() ? pastor : null );
                    mergeObjects.Add( "Groups", allCampusGroups );
                    mergeObjects.Add( "Campus", campus );

                    var recipients = new List<RecipientData>();
                    recipients.Add( new RecipientData( email, mergeObjects ) );

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
                        engagementEmailsSent++;
                    }
                }
                else
                {
                    errorCount += 1;
                    errorMessages.Add( string.Format( "No email specified for {0} and no fallback email provided.", pastor.FullName ) );
                }


            }    
        }

    }
}