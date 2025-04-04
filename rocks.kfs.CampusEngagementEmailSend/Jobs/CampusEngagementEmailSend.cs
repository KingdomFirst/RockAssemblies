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
using System.Text;
using System.Web;

using Quartz;

using Rock;
using Rock.Attribute;
using Rock.Communication;
using Rock.Data;
using Rock.Jobs;
using Rock.Model;
using Rock.Web.Cache;
using Rock.Web.UI.Controls;

namespace rocks.kfs.CampusEngagementEmailSend.Jobs
{
    /// <summary>
    /// Job to process communications
    /// </summary>
    [GroupTypesField( "Group Types", "The group types to include attendance numbers for in the campus engagement email.", true, "", "", 0 )]
    [SlidingDateRangeField( "Occurrence Date Range", "The date range of group attendance occurrences to include in the GoupAttendance Lava object.", required: true, order: 1 )]
    [SystemCommunicationField( "System Email", "The system email to use when sending the campus engagement email.", true, "", "", 2 )]

    [DisallowConcurrentExecution]
    public class CampusEngagementEmailSend : RockJob
    {
        private int engagementEmailsSent = 0;
        private int errorCount = 0;

        private List<string> errorMessages = new List<string>();
        private Guid systemEmailGuid = Guid.Empty;
        private List<GroupType> groupTypes = new List<GroupType>();
        private DateTime startDate = new DateTime();
        private DateTime endDate = new DateTime();

        /// <summary>
        /// Initializes a new instance of the <see cref="SendCommunications"/> class.
        /// </summary>
        public CampusEngagementEmailSend()
        {
        }

        public override void Execute()
        {
            var groupTypeService = new GroupTypeService( new RockContext() );

            groupTypes = new List<GroupType>( groupTypeService.GetByGuids( GetAttributeValue( "GroupTypes" ).ToString().Split( ',' ).Select( Guid.Parse ).ToList() ) );

            var requestDateRange = SlidingDateRangePicker.CalculateDateRangeFromDelimitedValues( GetAttributeValue( "OccurrenceDateRange" ).ToString() ?? "-1||" );

            startDate = ( DateTime ) requestDateRange.Start;
            endDate = ( DateTime ) requestDateRange.End;

            if ( groupTypes == null || groupTypes.Count == 0 )
            {
                Result = "Job failed. Unable to find group role/type";
                throw new Exception( "No group role/type found" );
            }

            systemEmailGuid = GetAttributeValue( "SystemEmail" ).AsGuid();

            SendEmailToCampusPastors();

            Result = string.Format( "{0} emails sent", engagementEmailsSent );
            if ( errorMessages.Any() )
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine();
                sb.Append( string.Format( "{0} Errors: ", errorCount ) );
                errorMessages.ForEach( e => { sb.AppendLine(); sb.Append( e ); } );
                string errors = sb.ToString();
                Result += errors;
                var exception = new Exception( errors );
                HttpContext context2 = HttpContext.Current;
                ExceptionLogService.LogException( exception, context2 );
                throw exception;
            }
        }

        private void SendEmailToCampusPastors()
        {
            var rockContext = new RockContext();
            var campusService = new CampusService( rockContext );
            var campuses = campusService.Queryable().AsNoTracking().ToList();

            foreach ( Campus campus in campuses )
            {
                if ( campus.IsActive == true )
                {
                    Person pastor = new Person();
                    if ( campus.LeaderPersonAlias != null )
                    {
                        pastor = campus.LeaderPersonAlias.Person;
                    }
                    else
                    {
                        errorCount += 1;
                        errorMessages.Add( string.Format( "No leader specified for {0} campus.", campus.Name ) );
                        continue;
                    }

                    string email;
                    if ( pastor.Email.IsNotNullOrWhiteSpace() )
                    {
                        email = pastor.Email;
                    }
                    else
                    {
                        errorCount += 1;
                        errorMessages.Add( string.Format( "No email listed for for {0} campus pastor.", campus.Name ) );
                        continue;
                    }


                    var groupService = new GroupService( rockContext );
                    List<Group> allCampusGroups = new List<Group>();

                    foreach ( GroupType groupType in groupTypes )
                    {
                        List<Group> groups = new List<Group>( groupService.GetByGroupTypeId( groupType.Id ) ).Where( g => g.CampusId == campus.Id ).ToList();
                        allCampusGroups.AddRange( groups );
                    }

                    var attendanceOccurrenceService = new AttendanceOccurrenceService( rockContext );
                    List<AttendanceOccurrence> groupAttendanceOccurrences = new List<AttendanceOccurrence>();

                    foreach ( Group group in allCampusGroups )
                    {
                        List<int> groupLocationIds = new List<int>();
                        List<int> groupScheduleIds = new List<int>();

                        foreach ( GroupLocation loc in group.GroupLocations )
                        {
                            groupLocationIds.Add( loc.LocationId );

                            foreach ( Schedule schedule in loc.Schedules )
                            {
                                groupScheduleIds.Add( schedule.Id );
                            }
                        }

                        List<AttendanceOccurrence> occurrences = new List<AttendanceOccurrence>( attendanceOccurrenceService.GetGroupOccurrences( group, startDate, endDate, groupLocationIds, groupScheduleIds ) ).ToList();
                        groupAttendanceOccurrences.AddRange( occurrences );
                    }

                    if ( email.IsNotNullOrWhiteSpace() )
                    {
                        var mergeObjects = Rock.Lava.LavaHelper.GetCommonMergeFields( null, pastor != null ? pastor : null );
                        mergeObjects.Add( "Person", pastor != null ? pastor : null );
                        mergeObjects.Add( "Groups", allCampusGroups );
                        mergeObjects.Add( "GroupAttendance", groupAttendanceOccurrences );
                        mergeObjects.Add( "Campus", campus );

                        var recipients = new List<RockEmailMessageRecipient>();
                        recipients.Add( new RockEmailMessageRecipient( pastor, mergeObjects ) );

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
                        errorMessages.Add( string.Format( "No email specified for {0}.", pastor.FullName ) );
                    }
                }

            }
        }

    }
}