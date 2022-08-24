// <copyright>
// Copyright 2022 by Kingdom First Solutions
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
using Rock.Model;
using Rock.Web.Cache;

namespace rocks.kfs.GroupRoundRobinAssignment.Jobs
{
    /// <summary>
    /// Job to process Group Round Robin Assignments
    /// </summary>
    ///
    [DataViewField(
        "People to Add Data View",
        "Select the data view you wish to use as your source for people to add to the round robin group assignment. For speed purposes we recommend it filter out people already assigned to groups.",
        true,
        "",
        "Rock.Model.Person" )]
    [CustomEnhancedListField( "Groups to Cycle Through", "Select the groups and/or parent groups to cycle assigning users to based on campus group = person group.", @"SELECT 
        CASE WHEN ggpg.Name IS NOT NULL THEN
	        CONCAT(ggpg.name, ' > ',gpg.Name,' > ',pg.Name,' > ', g.Name)
        WHEN gpg.Name IS NOT NULL THEN
	        CONCAT(gpg.Name,' > ',pg.Name,' > ', g.Name)
        WHEN pg.Name IS NOT NULL THEN
	        CONCAT(pg.Name,' > ', g.Name)
        ELSE
	        g.Name 
        END as Text, g.Id as Value
        FROM [Group] g
            LEFT JOIN [Group] pg ON g.ParentGroupId = pg.Id
            LEFT JOIN [Group] gpg ON pg.ParentGroupId = gpg.Id
            LEFT JOIN [Group] ggpg ON gpg.ParentGroupId = ggpg.Id
        WHERE g.GroupTypeId NOT IN (1,10,11,12) 
        ORDER BY 
            CASE WHEN ggpg.Name IS NOT NULL THEN
	            CONCAT(ggpg.name, ' > ',gpg.Name,' > ',pg.Name,' > ', g.Name)
            WHEN gpg.Name IS NOT NULL THEN
	            CONCAT(gpg.Name,' > ',pg.Name,' > ', g.Name)
            WHEN pg.Name IS NOT NULL THEN
	            CONCAT(pg.Name,' > ', g.Name)
            ELSE
                g.Name 
        END", true )]
    [CampusField(
        "Default Campus",
        "Default campus to assign people to and use if a family does not have a campus. If both group and campus are set, group takes precedence. If neither are set the person will remain unassigned.",
        false,
        "",
        false )]
    [GroupField(
        "Default Group",
        "Default group to assign people to and use if a family does not have a campus. If both group and campus are set, group takes precedence. If neither are set the person will remain unassigned.",
        false )]
    [DisallowConcurrentExecution]
    public class GroupRoundRobinAssignment : IJob
    {
        private int errorCount = 0;
        private List<string> errorMessages = new List<string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupRoundRobinAssignment"/> class.
        /// </summary>
        public GroupRoundRobinAssignment()
        {
        }

        /// <summary>
        /// Executes the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        public virtual void Execute(IJobExecutionContext context)
        {
            JobDataMap dataMap = context.JobDetail.JobDataMap;

            var dataViewGuid = dataMap.GetString( "PeopletoAddDataView" ).AsGuidOrNull();
            var groupIds = dataMap.GetString( "GroupstoCycleThrough" ).StringToIntList().ToList();
            var defaultGroupGuid = dataMap.GetString( "DefaultGroup" ).AsGuidOrNull();
            var defaultCampusGuid = dataMap.GetString( "DefaultCampus" ).AsGuidOrNull();
            var addedCount = 0;

            using ( var rockContext = new RockContext() )
            {
                var dataView = new DataViewService( rockContext ).Get( dataViewGuid ?? Guid.Empty );
                if ( dataView == null )
                {
                    errorMessages.Add( "This job requires a valid Data View setting." );
                }
                else
                {
                    var campusService = new CampusService( rockContext );
                    var personService = new PersonService( rockContext );
                    var groupService = new GroupService( rockContext );

                    Campus defaultCampus = null;
                    if ( defaultCampusGuid != null )
                    {
                        defaultCampus = campusService.Get( defaultCampusGuid.Value );
                    }
                    Group defaultGroup = null;
                    if ( defaultGroupGuid != null )
                    {
                        defaultGroup = groupService.Get( defaultGroupGuid.Value );
                    }

                    // Filter people by dataview
                    var paramExpression = personService.ParameterExpression;
                    var whereExpression = dataView.GetExpression( personService, paramExpression );
                    var personQry = personService
                        .Queryable( false, false ).AsNoTracking()
                        .Where( paramExpression, whereExpression, null );

                    // Get Groups and child groups from job setting.
                    var groupQry = groupService.GetByIds( groupIds );
                    var groups = new List<Group>();

                    foreach ( var group in groupQry )
                    {
                        var childGroups = groupService.GetChildren( group.Id, 0, false, new List<int>(), new List<int>(), false, false );
                        if ( childGroups.Any() )
                        {
                            groups.AddRange( childGroups );
                        }
                        else
                        {
                            groups.Add( group );
                        }
                    }

                    if ( !groups.Any() )
                    {
                        errorMessages.Add( "No valid groups were found. Did you select active groups?" );
                    }

                    var groupMemberService = new GroupMemberService( rockContext );
                    foreach ( var person in personQry )
                    {
                        var personCampusId = person.PrimaryCampusId;
                        Group groupToAddTo = null;

                        // If both Default Campus and Default Group are set, just set to Default group if person campus is empty.
                        if ( personCampusId == null && defaultCampus != null && defaultGroup == null )
                        {
                            personCampusId = defaultCampus.Id;
                        }

                        if ( personCampusId != null )
                        {
                            var groupsForCampus = groups
                                .Where( g => g.Campus != null && g.CampusId.Equals( personCampusId ) )
                                .OrderBy( g => g.ActiveMembers().Count() );

                            groupToAddTo = groupsForCampus.FirstOrDefault();
                        }
                        else if ( defaultGroup != null )
                        {
                            groupToAddTo = defaultGroup;
                        }
                        else
                        {
                            errorMessages.Add( string.Format( "Campus not set for {0} ({1}) and no Default Group set. Please check the person's campus and the default settings if this was unexpected.", person.FullName, person.Id ) );
                        }

                        if ( groupToAddTo != null )
                        {
                            var groupMember = new GroupMember
                            {
                                Person = person,
                                Group = groupToAddTo,
                                GroupRoleId = groupToAddTo.GroupType.DefaultGroupRoleId.Value,
                                GroupMemberStatus = GroupMemberStatus.Active
                            };

                            groupMemberService.Add( groupMember );

                            addedCount++;
                        }
                        else
                        {
                            errorMessages.Add( string.Format( "No Group found to add {0} ({1}) to. Please check the person's campus, the default settings and group campuses to ensure there is a match available.", person.FullName, person.Id ) );
                        }
                    }

                    rockContext.SaveChanges();

                }
            }
            context.Result += string.Format( "{0} people added to groups.", addedCount );

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
    }
}