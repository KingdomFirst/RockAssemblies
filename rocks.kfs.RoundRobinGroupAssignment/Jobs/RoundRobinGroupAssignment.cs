// <copyright>
// Copyright 2023 by Kingdom First Solutions
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
using Rock.Data;
using Rock.Model;

namespace rocks.kfs.RoundRobinGroupAssignment.Jobs
{
    /// <summary>
    /// Job to process Round Robin Group Assignment
    /// </summary>
    ///
    [DataViewField(
        "People Data View",
        Description = "Select the data view you wish to use as your source for people to add to the round robin group assignment.",
        IsRequired = true,
        EntityTypeName = "Rock.Model.Person",
        Key = AttributeKey.PeopleToAddDataView )]

    [CustomEnhancedListField(
        "Groups to Assign People to",
        Description = "Select the groups and/or parent groups to cycle assigning users to.",
         ListSource = @"SELECT 
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
        END",
        IsRequired = true,
        Key = AttributeKey.GroupsToCycleThrough )]

    [CampusField(
        name: "Default Campus",
        description: "Default campus to assign people to and use if a family does not have a campus. If both Default Group and Default Campus are set, Default Group takes precedence. If neither are set the person will remain unassigned.",
        required: false,
        includeInactive: false,
        key: AttributeKey.DefaultCampus )]

    [GroupField(
        "Default Group",
        Description = "Default group to assign people to and use if a family does not have a campus. If both Default Group and Default Campus are set, Default Group takes precedence. If neither are set the person will remain unassigned.",
        IsRequired = false )]

    [BooleanField(
        "Include Selected Groups",
        Description = "Should we include the selected groups in the round robin assignment or only the child groups? Default: No",
        DefaultBooleanValue = false,
        Key = AttributeKey.IncludeSelectedGroups )]

    [CustomDropdownListField(
        "Include Family Members",
        Description = "Should we include family members in the group as each person gets assigned? 'None', each person in the data view gets assigned to a different group. 'Adults Only', Family members with the 'Adult' Role will be added to the same group. 'All Family Members', every family member for the first individual encountered will be assigned to the same group. Default: All Family Members",
        DefaultValue = "All",
        ListSource = "All^All Family Members,Adults^Adults Only,None",
        Key = AttributeKey.IncludeFamilyMembers )]

    [BooleanField(
        "Output Errors to History",
        Description = "Should the job output all errors to job history? Default: Yes",
        DefaultBooleanValue = true,
        Key = AttributeKey.OutputErrors )]

    [BooleanField(
        "Use Group Campus",
        Description = "Should the job use Group Campus to match groups to person campus when assigning people to groups? Default: No",
        DefaultBooleanValue = false,
        Key = AttributeKey.UseGroupCampus )]

    [BooleanField(
        "Remove Members not in Data View",
        Description = "Should the job automatically remove group members not in the Data View? (Note: if you 'Include Family Members' your Data View should include them to prevent them from being removed with this setting.) Default: No",
        DefaultBooleanValue = false,
        Key = AttributeKey.RemoveMembers )]


    [DisallowConcurrentExecution]
    public class RoundRobinGroupAssignment : IJob
    {
        /// <summary>
        /// Attribute Keys
        /// </summary>
        private static class AttributeKey
        {
            public const string PeopleToAddDataView = "PeopleToAddDataView";
            public const string GroupsToCycleThrough = "GroupsToCycleThrough";
            public const string DefaultCampus = "DefaultCampus";
            public const string DefaultGroup = "DefaultGroup";
            public const string IncludeSelectedGroups = "IncludeSelectedGroups";
            public const string IncludeFamilyMembers = "IncludeFamilyMembers";
            public const string OutputErrors = "OutputErrors";
            public const string UseGroupCampus = "UseGroupCampus";
            public const string RemoveMembers = "RemoveMembers";
        }

        private List<string> errorMessages = new List<string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupRoundRobinAssignment"/> class.
        /// </summary>
        public RoundRobinGroupAssignment()
        {
        }

        /// <summary>
        /// Executes the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        public virtual void Execute( IJobExecutionContext context )
        {
            JobDataMap dataMap = context.JobDetail.JobDataMap;

            var dataViewGuid = dataMap.GetString( AttributeKey.PeopleToAddDataView ).AsGuidOrNull();
            var groupIds = dataMap.GetString( AttributeKey.GroupsToCycleThrough ).StringToIntList().ToList();
            var defaultGroupGuid = dataMap.GetString( AttributeKey.DefaultGroup ).AsGuidOrNull();
            var defaultCampusGuid = dataMap.GetString( AttributeKey.DefaultCampus ).AsGuidOrNull();
            var includeSelectedGroups = dataMap.GetBooleanFromString( AttributeKey.IncludeSelectedGroups );
            var includeFamilyMembers = dataMap.GetString( AttributeKey.IncludeFamilyMembers );
            var outputErrors = dataMap.GetBooleanFromString( AttributeKey.OutputErrors );
            var useGroupCampus = dataMap.GetBooleanFromString( AttributeKey.UseGroupCampus );
            var removeMembers = dataMap.GetBooleanFromString( AttributeKey.RemoveMembers );
            var addedCount = 0;
            var peopleRemoved = 0;

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
                    var groupMemberService = new GroupMemberService( rockContext );

                    var defaultCampus = campusService.Get( defaultCampusGuid ?? Guid.Empty );
                    var defaultGroup = groupService.Get( defaultGroupGuid ?? Guid.Empty );

                    // Filter people by dataview
                    var paramExpression = personService.ParameterExpression;
                    var whereExpression = dataView.GetExpression( personService, paramExpression );
                    var dataViewPersonQry = personService
                        .Queryable( false, false ).AsNoTracking()
                        .Where( paramExpression, whereExpression, null );

                    // Get Groups and child groups from job setting.
                    var groupQry = groupService.GetByIds( groupIds );
                    var groups = new List<Group>();

                    foreach ( var group in groupQry )
                    {
                        if ( includeSelectedGroups )
                        {
                            groups.Add( group );
                        }

                        var descendantGroups = groupService.GetAllDescendentGroups( group.Id, false );
                        groups.AddRange( descendantGroups );
                    }

                    if ( !groups.Any() )
                    {
                        errorMessages.Add( "No valid groups were found. Did you select active groups?" );
                    }

                    var allGroupIds = groups.Select( g => g.Id ).ToList();
                    var groupMemberServiceQry = groupMemberService.Queryable( true ).Where( gm => allGroupIds.Contains( gm.GroupId ) );
                    var peopleToAddQry = dataViewPersonQry.Where( p => !groupMemberServiceQry.Any( gm => gm.PersonId == p.Id ) );

                    var addedPeopleIds = new List<int>();

                    foreach ( var person in peopleToAddQry )
                    {
                        var personCampusId = person.PrimaryCampusId;
                        Group groupToAddTo = null;

                        // If both Default Campus and Default Group are set, only the Default Group will be used
                        if ( useGroupCampus && personCampusId == null && defaultCampus != null && defaultGroup == null )
                        {
                            personCampusId = defaultCampus.Id;
                            errorMessages.Add( string.Format( "Campus not found for {0} ({1}), setting to Default Campus for group search.", person.FullName, person.Id ) );
                        }

                        if ( personCampusId != null && useGroupCampus )
                        {
                            var groupsForCampus = groups
                                .Where( g => g.Campus != null && g.CampusId.Equals( personCampusId ) )
                                .OrderBy( g => g.ActiveMembers().Count() );

                            groupToAddTo = groupsForCampus.FirstOrDefault();
                        }
                        else
                        {
                            groupToAddTo = groups.OrderBy( g => g.ActiveMembers().Count() ).FirstOrDefault();
                        }

                        // If Person campus is not set and Default Group is set or a matching group to Person's campus is not found use Default Group
                        if ( groupToAddTo == null && defaultGroup != null )
                        {
                            groupToAddTo = defaultGroup;
                            errorMessages.Add( string.Format( "Group not found with matching campus for {0} ({1}), added to Default Group.", person.FullName, person.Id ) );
                        }

                        if ( groupToAddTo != null && !groupMemberService.GetByGroupIdAndPersonId( groupToAddTo.Id, person.Id ).Any() && !addedPeopleIds.Contains( person.Id ) )
                        {
                            var groupMember = new GroupMember
                            {
                                PersonId = person.Id,
                                GroupId = groupToAddTo.Id,
                                GroupRoleId = groupToAddTo.GroupType.DefaultGroupRoleId.Value,
                                GroupMemberStatus = GroupMemberStatus.Active
                            };

                            if ( groupMember.IsValidGroupMember( rockContext ) )
                            {
                                groupMemberService.Add( groupMember );
                                addedPeopleIds.Add( person.Id );
                                addedCount++;

                                if ( includeFamilyMembers != "None" )
                                {
                                    foreach ( var familyMember in person.GetFamilyMembers( false, rockContext ) )
                                    {
                                        var fPerson = familyMember.Person;

                                        if ( !groupMemberServiceQry.Any( gm => gm.PersonId == fPerson.Id ) && ( includeFamilyMembers != "Adults" || fPerson.GetFamilyRole( rockContext ).Guid == Rock.SystemGuid.GroupRole.GROUPROLE_FAMILY_MEMBER_ADULT.AsGuid() ) )
                                        {
                                            try
                                            {
                                                var fGroupMember = new GroupMember
                                                {
                                                    PersonId = fPerson.Id,
                                                    GroupId = groupToAddTo.Id,
                                                    GroupRoleId = groupToAddTo.GroupType.DefaultGroupRoleId.Value,
                                                    GroupMemberStatus = GroupMemberStatus.Active
                                                };

                                                if ( fGroupMember.IsValidGroupMember( rockContext ) )
                                                {
                                                    groupMemberService.Add( fGroupMember );
                                                    addedPeopleIds.Add( fPerson.Id );
                                                    addedCount++;
                                                }
                                            }
                                            catch ( Exception e )
                                            {
                                                errorMessages.Add( string.Format( "There was an error adding family member {0} ({1}) to {2} group. Exception: {3}", fPerson.FullName, fPerson.Id, groupToAddTo.Name, e.Message ) );
                                            }
                                        }
                                        else if ( groupMemberServiceQry.Any( gm => gm.PersonId == fPerson.Id ) )
                                        {
                                            errorMessages.Add( string.Format( "This family member, {0} ({1}), already exists in a group. Skipping addition, you may want to double check group assignment.", fPerson.FullName, fPerson.Id ) );
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if ( useGroupCampus && personCampusId == null )
                            {
                                errorMessages.Add( string.Format( "{0} ({1}) does not have a campus set and there are no defaults set on the job. Please try again.", person.FullName, person.Id ) );
                            }
                            else if ( groupToAddTo != null )
                            {
                                errorMessages.Add( string.Format( "{0} ({1}) is already in a group assignment or was added with a family member. You may ignore this error or consider refining your data view to single members of household. ", person.FullName, person.Id ) );
                            }
                            else
                            {
                                errorMessages.Add( string.Format( "No Group for the {2} campus ({3}) found to add {0} ({1}) to. Please check the person's campus, group campuses and the default settings on this job to ensure there is a match available.", person.FullName, person.Id, person.PrimaryCampus.Name, person.PrimaryCampusId ) );
                            }
                        }
                    }

                    rockContext.SaveChanges();

                    if ( removeMembers )
                    {
                        // Reload group Members and people in data view after save.
                        dataViewPersonQry = personService.Queryable( false, false ).AsNoTracking().Where( paramExpression, whereExpression, null );
                        var reloadedGroupMemberQry = groupMemberService.Queryable( true ).Where( gm => allGroupIds.Contains( gm.GroupId ) );
                        var peopleToRemove = reloadedGroupMemberQry.Where( gm => !dataViewPersonQry.Any( p => p.Id == gm.PersonId ) );

                        foreach ( var groupMember in peopleToRemove )
                        {
                            groupMemberService.Delete( groupMember );
                            peopleRemoved++;
                        }

                        rockContext.SaveChanges();

                    }
                }
            }
            context.Result += string.Format( "{0} {1} added to groups.", addedCount, "person".PluralizeIf( addedCount > 1 ) );
            if ( removeMembers )
            {
                context.Result += string.Format( "<br>{0} {1} removed from groups.", peopleRemoved, "person".PluralizeIf( peopleRemoved > 1 ) );
            }

            if ( errorMessages.Any() )
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine();
                sb.AppendLine();
                sb.Append( string.Format( "{0} Errors: ", errorMessages.Count ) );
                errorMessages.ForEach( e => { sb.AppendLine(); sb.Append( e ); } );
                string errors = sb.ToString();
                if ( outputErrors )
                {
                    context.Result += errors;
                }
                // Log errors as exceptions if addedCount == 0 or errors are not output to Job history.
                if ( addedCount == 0 || !outputErrors )
                {
                    var exception = new Exception( errors );
                    HttpContext context2 = HttpContext.Current;
                    ExceptionLogService.LogException( exception, context2 );
                    throw exception;
                }
            }
        }
    }
}
