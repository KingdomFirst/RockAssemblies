// <copyright>
// Copyright 2024 by Kingdom First Solutions
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
using NuGet;
using Rock;
using Rock.Communication;
using Rock.Data;
using Rock.Logging;
using Rock.Model;
using Rock.Web;
using Rock.Web.Cache;
using Rock.Web.UI;
using rocks.kfs.StepsToCare.Model;

namespace rocks.kfs.StepsToCare
{
    public class CareUtilities
    {
        public static List<AssignedPerson> AutoAssignWorkers( CareNeed careNeed, bool roundRobinOnly = false, bool childAssignment = false, bool autoAssignWorker = false, bool autoAssignWorkerGeofence = false, string loadBalanceType = "Exclusive", List<Guid> leaderRoleGuids = null, bool enableLogging = false, bool previewAssigned = false )
        {
            var assignedPeople = new List<AssignedPerson>();

            var rockContext = new RockContext();

            var careNeedService = new CareNeedService( rockContext );
            var careWorkerService = new CareWorkerService( rockContext );
            var careAssigneeService = new AssignedPersonService( rockContext );
            var careNeedHistory = new History.HistoryChangeList();

            // reload careNeed to fully populate child properties
            if ( !previewAssigned )
            {
                careNeed = careNeedService.Get( careNeed.Guid );
            }

            var careWorkers = careWorkerService.Queryable().AsNoTracking().Where( cw => cw.IsActive );

            var addedWorkerAliasIds = new List<int?>();
            var closedStatusId = DefinedValueCache.Get( rocks.kfs.StepsToCare.SystemGuid.DefinedValue.CARE_NEED_STATUS_CLOSED ).Id;

            // auto assign Deacon/Worker by Geofence
            if ( autoAssignWorkerGeofence && !roundRobinOnly )
            {
                if ( enableLogging )
                {
                    LogEvent( null, "AutoAssignWorkers", string.Format( "Care Need Guid: {0}, CareWorkers Count: {1}", careNeed.Guid, careWorkers.Count() ), "Geofence assignment start." );
                }
                var careWorkersWithFence = careWorkers.Where( cw => cw.GeoFenceId != null );
                foreach ( var worker in careWorkersWithFence )
                {
                    var geofenceLocation = new LocationService( rockContext ).Get( worker.GeoFenceId.Value );
                    if ( enableLogging )
                    {
                        LogEvent( null, "AutoAssignWorkers", string.Format( "Care Need Guid: {0}, geofenceLocation: {1}", careNeed.Guid, geofenceLocation.Id ), "Care Workers with Fence" );
                    }
                    var homeLocation = careNeed.PersonAlias.Person.GetHomeLocation();
                    if ( enableLogging )
                    {
                        LogEvent( null, "AutoAssignWorkers", string.Format( "Care Need Guid: {0}, geofenceLocation: {1}, homeLocation: {2}", careNeed.Guid, geofenceLocation.Id, ( homeLocation != null ) ? homeLocation.Id.ToString() : "null" ), "Care Workers with Fence" );
                    }
                    if ( homeLocation != null && homeLocation.GeoPoint != null )
                    {
                        var geofenceIntersect = homeLocation.GeoPoint.Intersects( geofenceLocation.GeoFence );
                        if ( enableLogging )
                        {
                            LogEvent( null, "AutoAssignWorkers", string.Format( "Care Need Guid: {0}, geofenceIntersect: {1}, homeLocation: {2}", careNeed.Guid, geofenceIntersect, homeLocation.Id ), "geofenceIntersect" );
                        }
                        if ( geofenceIntersect )
                        {
                            var careAssignee = new AssignedPerson { Id = 0 };
                            careAssignee.CareNeed = careNeed;
                            careAssignee.PersonAliasId = worker.PersonAliasId;
                            careAssignee.WorkerId = worker.Id;
                            careAssignee.Type = AssignedType.Geofence;
                            careAssignee.FollowUpWorker = false;

                            assignedPeople.Add( careAssignee );
                            addedWorkerAliasIds.Add( careAssignee.PersonAliasId );

                            string newStringValue = History.GetValue<PersonAlias>( worker.PersonAlias, worker.PersonAliasId, rockContext, "" );
                            careNeedHistory.AddChange( History.HistoryVerb.Add, History.HistoryChangeType.Record, "Assign Person", null, newStringValue ).SourceOfChange = "Geofence Assignment";
                            AddPersonHistory( rockContext, careNeed, careNeed.PersonAlias.Person, worker.PersonAlias, "ASSIGNED", "Geofence Assignment" );
                        }
                    }
                    else if ( homeLocation != null && homeLocation.GeoPoint == null )
                    {
                        LogEvent( null, "AutoAssignWorkers", string.Format( "Care Need Id: {0}, homeLocation: {1}", careNeed.Id, homeLocation.Id ), "Home Location does not have a valid GeoPoint. Please verify their address and manually assign their geo worker." );
                    }
                }
                if ( enableLogging )
                {
                    LogEvent( null, "AutoAssignWorkers", string.Format( "Care Need Guid: {0}, careWorkersWithFence Count: {1} addedWorkerAliasIds Count: {2}", careNeed.Guid, careWorkersWithFence.Count(), addedWorkerAliasIds.Count() ), "Geofence assignment end." );
                }
            }

            //auto assign worker/pastor by load balance assignment
            if ( autoAssignWorker )
            {
                if ( enableLogging )
                {
                    LogEvent( null, "AutoAssignWorkers", string.Format( "Care Need Guid: {0}, CareWorkers Count: {1}", careNeed.Guid, careWorkers.Count() ), "Auto Assign Worker start." );
                }
                var careWorkersNoFence = careWorkers.Where( cw => cw.GeoFenceId == null );
                var workerAssigned = false;

                // Campus, Category, Ignore Age Range and Gender
                var careWorkerCount1 = GenerateAgeQuery( careNeed, careWorkersNoFence, closedStatusId, false, false, true, true, true );

                if ( enableLogging )
                {
                    LogEvent( null, "AutoAssignWorkers", string.Format( "Care Need Guid: {0}, careWorkersNoFence Count: {1}, careWorkerCount1 Count: {2}", careNeed.Guid, careWorkersNoFence.Count(), careWorkerCount1.Count() ), "careWorkerCount1, Category AND Campus" );
                }

                // Category, Ignore Age Range and Gender
                var careWorkerCount2 = GenerateAgeQuery( careNeed, careWorkersNoFence, closedStatusId, false, false, false, true, true );

                if ( enableLogging )
                {
                    LogEvent( null, "AutoAssignWorkers", string.Format( "Care Need Guid: {0}, careWorkerCount2 Count: {1}", careNeed.Guid, careWorkerCount2.Count() ), "careWorkerCount2, Category NOT Campus" );
                }

                // Campus, Ignore Age Range and Gender
                var careWorkerCount3 = GenerateAgeQuery( careNeed, careWorkersNoFence, closedStatusId, false, false, true, false, true );

                if ( enableLogging )
                {
                    LogEvent( null, "AutoAssignWorkers", string.Format( "Care Need Guid: {0}, careWorkerCount3 Count: {1}", careNeed.Guid, careWorkerCount3.Count() ), "careWorkerCount3, Campus NOT Category" );
                }

                // None, doesn't include parameters for other values though.
                var careWorkerCount4 = GenerateAgeQuery( careNeed, careWorkersNoFence, closedStatusId, false, false, false, false, true );

                if ( enableLogging )
                {
                    LogEvent( null, "AutoAssignWorkers", string.Format( "Care Need Guid: {0}, careWorkerCount4 Count: {1}", careNeed.Guid, careWorkerCount4.Count() ), "careWorkerCount4, NOT Campus or Category" );
                }

                IOrderedQueryable<WorkerResult> careWorkersCountChild1 = null;
                IOrderedQueryable<WorkerResult> careWorkersCountChild2 = null;
                IOrderedQueryable<WorkerResult> careWorkersCountChild3 = null;
                IOrderedQueryable<WorkerResult> careWorkersCountChild4 = null;
                IOrderedQueryable<WorkerResult> careWorkersCountChild5 = null;
                IOrderedQueryable<WorkerResult> careWorkersCountChild6 = null;
                IOrderedQueryable<WorkerResult> careWorkersCountChild7 = null;
                IOrderedQueryable<WorkerResult> careWorkersCountChild8 = null;
                IOrderedQueryable<WorkerResult> careWorkersCountChild9 = null;
                IOrderedQueryable<WorkerResult> careWorkersCountChild10 = null;
                IOrderedQueryable<WorkerResult> careWorkersCountChild11 = null;
                IOrderedQueryable<WorkerResult> careWorkersCountChild12 = null;
                IOrderedQueryable<WorkerResult> careWorkersCountChild13 = null;
                IOrderedQueryable<WorkerResult> careWorkersCountChild14 = null;
                IOrderedQueryable<WorkerResult> careWorkersCountChild15 = null;
                IOrderedQueryable<WorkerResult> careWorkersCountChild16 = null;
                if ( childAssignment )
                {
                    // AgeRange, Gender, Campus, Category
                    careWorkersCountChild1 = GenerateAgeQuery( careNeed, careWorkersNoFence, closedStatusId, true, true, true, true );

                    // AgeRange, Gender, Category
                    careWorkersCountChild2 = GenerateAgeQuery( careNeed, careWorkersNoFence, closedStatusId, true, true, false, true );

                    // AgeRange, Gender, Campus
                    careWorkersCountChild3 = GenerateAgeQuery( careNeed, careWorkersNoFence, closedStatusId, true, true, true, false );

                    // AgeRange, Campus, Category
                    careWorkersCountChild4 = GenerateAgeQuery( careNeed, careWorkersNoFence, closedStatusId, true, false, true, true );

                    // Gender, Campus, Category
                    careWorkersCountChild5 = GenerateAgeQuery( careNeed, careWorkersNoFence, closedStatusId, false, true, true, true );

                    // AgeRange, Gender
                    careWorkersCountChild6 = GenerateAgeQuery( careNeed, careWorkersNoFence, closedStatusId, true, true, false, false );

                    // AgeRange, Category
                    careWorkersCountChild7 = GenerateAgeQuery( careNeed, careWorkersNoFence, closedStatusId, true, false, false, true );

                    // AgeRange, Campus
                    careWorkersCountChild8 = GenerateAgeQuery( careNeed, careWorkersNoFence, closedStatusId, true, false, true, false );

                    // Gender, Category
                    careWorkersCountChild9 = GenerateAgeQuery( careNeed, careWorkersNoFence, closedStatusId, false, true, false, true );

                    // Gender, Campus
                    careWorkersCountChild10 = GenerateAgeQuery( careNeed, careWorkersNoFence, closedStatusId, false, true, true, false );

                    // Campus, Category
                    careWorkersCountChild11 = GenerateAgeQuery( careNeed, careWorkersNoFence, closedStatusId, false, false, true, true );

                    // AgeRange
                    careWorkersCountChild12 = GenerateAgeQuery( careNeed, careWorkersNoFence, closedStatusId, true, false, false, false );

                    // Gender
                    careWorkersCountChild13 = GenerateAgeQuery( careNeed, careWorkersNoFence, closedStatusId, false, true, false, false );

                    // Category
                    careWorkersCountChild14 = GenerateAgeQuery( careNeed, careWorkersNoFence, closedStatusId, false, false, false, true );

                    // Campus
                    careWorkersCountChild15 = GenerateAgeQuery( careNeed, careWorkersNoFence, closedStatusId, false, false, true, false );

                    // None
                    careWorkersCountChild16 = GenerateAgeQuery( careNeed, careWorkersNoFence, closedStatusId, false, false, false, false );
                }

                var careWorkerCounts = careWorkerCount1;
                if ( loadBalanceType == "Prioritize" )
                {
                    if ( childAssignment )
                    {
                        careWorkerCounts = careWorkersCountChild1
                                            .Concat( careWorkersCountChild2 )
                                            .Concat( careWorkersCountChild3 )
                                            .Concat( careWorkersCountChild4 )
                                            .Concat( careWorkersCountChild5 )
                                            .Concat( careWorkersCountChild6 )
                                            .Concat( careWorkersCountChild7 )
                                            .Concat( careWorkersCountChild8 )
                                            .Concat( careWorkersCountChild9 )
                                            .Concat( careWorkersCountChild10 )
                                            .Concat( careWorkersCountChild11 )
                                            .Concat( careWorkersCountChild12 )
                                            .Concat( careWorkersCountChild13 )
                                            .Concat( careWorkersCountChild14 )
                                            .Concat( careWorkersCountChild15 )
                                            .Concat( careWorkersCountChild16 )
                                            .OrderBy( ct => ct.Count )
                                            .ThenByDescending( ct => ct.HasAgeRange && ct.HasGender && ct.HasCampus && ct.HasCategory )         // AgeRange, Gender, Campus, Category
                                            .ThenByDescending( ct => ct.HasAgeRange && ct.HasGender && !ct.HasCampus && ct.HasCategory )        // AgeRange, Gender, Category
                                            .ThenByDescending( ct => ct.HasAgeRange && ct.HasGender && ct.HasCampus && !ct.HasCategory )        // AgeRange, Gender, Campus
                                            .ThenByDescending( ct => ct.HasAgeRange && !ct.HasGender && ct.HasCampus && ct.HasCategory )        // AgeRange, Campus, Category
                                            .ThenByDescending( ct => !ct.HasAgeRange && ct.HasGender && ct.HasCampus && ct.HasCategory )        // Gender, Campus, Category
                                            .ThenByDescending( ct => ct.HasAgeRange && ct.HasGender && !ct.HasCampus && !ct.HasCategory )       // AgeRange, Gender
                                            .ThenByDescending( ct => ct.HasAgeRange && !ct.HasGender && !ct.HasCampus && ct.HasCategory )       // AgeRange, Category
                                            .ThenByDescending( ct => ct.HasAgeRange && !ct.HasGender && ct.HasCampus && !ct.HasCategory )       // AgeRange, Campus
                                            .ThenByDescending( ct => !ct.HasAgeRange && ct.HasGender && !ct.HasCampus && ct.HasCategory )       // Gender, Category
                                            .ThenByDescending( ct => !ct.HasAgeRange && ct.HasGender && ct.HasCampus && !ct.HasCategory )       // Gender, Campus
                                            .ThenByDescending( ct => !ct.HasAgeRange && !ct.HasGender && ct.HasCampus && ct.HasCategory )       // Campus, Category
                                            .ThenByDescending( ct => ct.HasAgeRange && !ct.HasGender && !ct.HasCampus && !ct.HasCategory )      // AgeRange
                                            .ThenByDescending( ct => !ct.HasAgeRange && ct.HasGender && !ct.HasCampus && !ct.HasCategory )      // Gender
                                            .ThenByDescending( ct => !ct.HasAgeRange && !ct.HasGender && !ct.HasCampus && ct.HasCategory )      // Category
                                            .ThenByDescending( ct => !ct.HasAgeRange && !ct.HasGender && ct.HasCampus && !ct.HasCategory )      // Campus
                                            .ThenByDescending( ct => !ct.HasAgeRange && !ct.HasGender && !ct.HasCampus && !ct.HasCategory );    // None
                    }
                    else
                    {
                        careWorkerCounts = careWorkerCount1
                                        .Concat( careWorkerCount2 )
                                        .Concat( careWorkerCount3 )
                                        .Concat( careWorkerCount4 )
                                        .OrderBy( ct => ct.Count )
                                        .ThenByDescending( ct => ct.HasCategory && ct.HasCampus )
                                        .ThenByDescending( ct => ct.HasCategory && !ct.HasCampus )
                                        .ThenByDescending( ct => ct.HasCampus && !ct.HasCategory );
                    }
                }
                else
                {
                    if ( childAssignment )
                    {
                        if ( careWorkersCountChild1.Any() )
                        {
                            careWorkerCounts = careWorkersCountChild1;
                        }
                        else if ( careWorkersCountChild2.Any() )
                        {
                            careWorkerCounts = careWorkersCountChild2;
                        }
                        else if ( careWorkersCountChild3.Any() )
                        {
                            careWorkerCounts = careWorkersCountChild3;
                        }
                        else if ( careWorkersCountChild4.Any() )
                        {
                            careWorkerCounts = careWorkersCountChild4;
                        }
                        else if ( careWorkersCountChild5.Any() )
                        {
                            careWorkerCounts = careWorkersCountChild5;
                        }
                        else if ( careWorkersCountChild6.Any() )
                        {
                            careWorkerCounts = careWorkersCountChild6;
                        }
                        else if ( careWorkersCountChild7.Any() )
                        {
                            careWorkerCounts = careWorkersCountChild7;
                        }
                        else if ( careWorkersCountChild8.Any() )
                        {
                            careWorkerCounts = careWorkersCountChild8;
                        }
                        else if ( careWorkersCountChild9.Any() )
                        {
                            careWorkerCounts = careWorkersCountChild9;
                        }
                        else if ( careWorkersCountChild10.Any() )
                        {
                            careWorkerCounts = careWorkersCountChild10;
                        }
                        else if ( careWorkersCountChild11.Any() )
                        {
                            careWorkerCounts = careWorkersCountChild11;
                        }
                        else if ( careWorkersCountChild12.Any() )
                        {
                            careWorkerCounts = careWorkersCountChild12;
                        }
                        else if ( careWorkersCountChild13.Any() )
                        {
                            careWorkerCounts = careWorkersCountChild13;
                        }
                        else if ( careWorkersCountChild14.Any() )
                        {
                            careWorkerCounts = careWorkersCountChild14;
                        }
                        else if ( careWorkersCountChild15.Any() )
                        {
                            careWorkerCounts = careWorkersCountChild15;
                        }
                        else if ( careWorkersCountChild16.Any() )
                        {
                            careWorkerCounts = careWorkersCountChild16;
                        }
                    }
                    else
                    {
                        if ( careWorkerCount1.Any() )
                        {
                            careWorkerCounts = careWorkerCount1;
                        }
                        else if ( careWorkerCount2.Any() )
                        {
                            careWorkerCounts = careWorkerCount2;
                        }
                        else if ( careWorkerCount3.Any() )
                        {
                            careWorkerCounts = careWorkerCount3;
                        }
                        else if ( careWorkerCount4.Any() )
                        {
                            careWorkerCounts = careWorkerCount4;
                        }
                    }
                }

                if ( enableLogging )
                {
                    LogEvent( null, "AutoAssignWorkers", string.Format( "Care Need Guid: {0}, careWorkerCounts Count: {1}", careNeed.Guid, careWorkerCounts.Count() ), "Combined careWorkerCounts" );
                }

                foreach ( var workerCount in careWorkerCounts )
                {
                    var worker = workerCount.Worker;
                    if ( !workerAssigned && !addedWorkerAliasIds.Contains( worker.PersonAliasId ) && worker.PersonAlias != null && worker.PersonAliasId != careNeed.PersonAliasId && careAssigneeService.GetByPersonAliasAndCareNeed( worker.PersonAlias.Id, careNeed.Id ) == null )
                    {
                        var careAssignee = new AssignedPerson { Id = 0 };
                        careAssignee.CareNeed = careNeed;
                        careAssignee.PersonAliasId = worker.PersonAliasId;
                        careAssignee.WorkerId = worker.Id;
                        careAssignee.FollowUpWorker = true;
                        careAssignee.Type = AssignedType.Worker;
                        careAssignee.TypeQualifier = $"{workerCount.Count}^{workerCount.HasAgeRange}^{workerCount.HasCampus}^{workerCount.HasCategory}^{workerCount.HasGender}";

                        assignedPeople.Add( careAssignee );
                        addedWorkerAliasIds.Add( careAssignee.PersonAliasId );

                        string sourceOfChange = $"Worker Assignment [Count: {workerCount.Count}]";
                        string newStringValue = History.GetValue<PersonAlias>( worker.PersonAlias, worker.PersonAliasId, rockContext, "" );
                        careNeedHistory.AddChange( History.HistoryVerb.Add, History.HistoryChangeType.Record, "Assign Person", null, newStringValue ).SourceOfChange = sourceOfChange;
                        AddPersonHistory( rockContext, careNeed, careNeed.PersonAlias.Person, worker.PersonAlias, "ASSIGNED", sourceOfChange );

                        workerAssigned = true;

                        if ( enableLogging )
                        {
                            LogEvent( null, "AutoAssignWorkers", string.Format( "Care Need Guid: {0}, Worker PersonAliasId: {1}, WorkerId: {2}", careNeed.Guid, worker.PersonAliasId, worker.Id ), "Worker Assigned" );
                        }
                    }
                }
            }

            // auto assign Small Group Leader by Role
            foreach ( var leaderRoleGuid in leaderRoleGuids )
            {
                var leaderRole = new GroupTypeRoleService( rockContext ).Get( leaderRoleGuid );
                if ( enableLogging )
                {
                    LogEvent( null, "AutoAssignWorkers", string.Format( "Care Need Guid: {0}, Leader Role Guid: {1}, Leader Role: {2}", careNeed.Guid, leaderRoleGuid, leaderRole.Id ), "Get Leader Role" );
                }

                if ( leaderRole != null && !roundRobinOnly )
                {
                    var groupMemberService = new GroupMemberService( rockContext );
                    var inGroups = groupMemberService.GetByPersonId( careNeed.PersonAlias.PersonId ).Where( gm => gm.Group != null && gm.Group.IsActive && !gm.Group.IsArchived && gm.Group.GroupTypeId == leaderRole.GroupTypeId && !gm.IsArchived && gm.GroupMemberStatus == GroupMemberStatus.Active ).Select( gm => gm.GroupId );

                    if ( inGroups.Any() )
                    {
                        if ( enableLogging )
                        {
                            LogEvent( null, "AutoAssignWorkers", string.Format( "Care Need Guid: {0}, In Groups Count: {1}, leaderRole.GroupTypeId: {2}", careNeed.Guid, inGroups.Count(), leaderRole.GroupTypeId ), "In Small Groups" );
                        }
                        var groupLeaders = groupMemberService.GetByGroupRoleId( leaderRole.Id ).Where( gm => inGroups.Contains( gm.GroupId ) && !gm.IsArchived && gm.GroupMemberStatus == GroupMemberStatus.Active );
                        foreach ( var member in groupLeaders )
                        {
                            if ( !addedWorkerAliasIds.Contains( member.Person.PrimaryAliasId ) && careAssigneeService.GetByPersonAliasAndCareNeed( member.Person.PrimaryAliasId, careNeed.Id ) == null && member.PersonId != careNeed.PersonAlias.Person.Id )
                            {
                                var careAssignee = new AssignedPerson { Id = 0 };
                                careAssignee.CareNeed = careNeed;
                                careAssignee.PersonAliasId = member.Person.PrimaryAliasId;
                                careAssignee.Type = AssignedType.GroupRole;
                                careAssignee.TypeQualifier = $"{member.GroupRoleId}^{member.GroupTypeId}^{member.Group.GroupType.Name} > {member.GroupRole.Name}^{member.Group.Id}^{member.Group.GroupType.Name} > {member.Group.Name} > {member.GroupRole.Name}";
                                careAssignee.FollowUpWorker = false;

                                assignedPeople.Add( careAssignee );
                                addedWorkerAliasIds.Add( careAssignee.PersonAliasId );

                                string sourceOfChange = $"Group Role Assignment - {member.Group.GroupType.Name} > {member.Group.Name} [{member.GroupId}] > {member.GroupRole.Name}";
                                string newStringValue = History.GetValue<PersonAlias>( member.Person.PrimaryAlias, member.Person.PrimaryAliasId, rockContext, "" );
                                careNeedHistory.AddChange( History.HistoryVerb.Add, History.HistoryChangeType.Record, "Assign Person", null, newStringValue ).SourceOfChange = sourceOfChange;
                                AddPersonHistory( rockContext, careNeed, careNeed.PersonAlias.Person, member.Person.PrimaryAlias, "ASSIGNED", sourceOfChange );
                            }
                        }
                        if ( enableLogging )
                        {
                            LogEvent( null, "AutoAssignWorkers", string.Format( "Care Need Guid: {0}, groupLeaders Count: {1} addedWorkerAliasIds Count: {2}", careNeed.Guid, groupLeaders.Count(), addedWorkerAliasIds.Count() ), "In Small Groups, Leader Count" );
                        }
                    }
                }
            }

            if ( careNeed.CategoryValueId.HasValue )
            {
                Guid matrixGuid = Guid.Empty;
                var categoryGroups = new List<int>();
                if ( careNeed.Category != null )
                {
                    matrixGuid = careNeed.Category.GetAttributeValue( "CareTouchTemplates" ).AsGuid();
                    categoryGroups = careNeed.Category.GetAttributeValues( "AssignToGroups" ).AsIntegerList();
                }
                else
                {
                    var catCache = DefinedValueCache.Get( careNeed.CategoryValueId.Value );
                    matrixGuid = catCache.GetAttributeValue( "CareTouchTemplates" ).AsGuid();
                    categoryGroups = catCache.GetAttributeValues( "AssignToGroups" ).AsIntegerList();
                }
                var touchTemplates = new List<TouchTemplate>();
                if ( matrixGuid != Guid.Empty )
                {
                    var matrix = new AttributeMatrixService( rockContext ).Get( matrixGuid );
                    if ( matrix != null )
                    {
                        foreach ( var matrixItem in matrix.AttributeMatrixItems )
                        {
                            matrixItem.LoadAttributes();

                            var noteTemplateGuid = matrixItem.GetAttributeValue( "NoteTemplate" ).AsGuid();
                            var noteTemplate = new NoteTemplateService( rockContext ).Get( noteTemplateGuid );

                            if ( noteTemplate != null )
                            {
                                // only load the properties we need to use in the following code, hopefully lighten the attribute call.
                                var touchTemplate = new TouchTemplate();
                                touchTemplate.NoteTemplate = noteTemplate;
                                touchTemplate.MinimumCareTouches = matrixItem.GetAttributeValue( "MinimumCareTouches" ).AsInteger();
                                touchTemplate.MinimumCareTouchHours = matrixItem.GetAttributeValue( "MinimumCareTouchHours" ).AsInteger();
                                touchTemplate.AssignToGroups = matrixItem.GetAttributeValues( "AssignToGroups" ).AsIntegerList();
                                touchTemplate.Order = matrixItem.Order;

                                touchTemplates.Add( touchTemplate );
                            }
                        }
                    }
                }

                var currentlyAssignedPeople = careAssigneeService.Queryable().AsNoTracking();

                if ( categoryGroups.Any() )
                {
                    foreach ( var groupId in categoryGroups )
                    {
                        AssignToGroupMember( careNeed, enableLogging, assignedPeople, rockContext, careAssigneeService, addedWorkerAliasIds, closedStatusId, groupId, currentlyAssignedPeople, careNeedHistory: careNeedHistory );
                    }
                }

                if ( touchTemplates.Any( t => t.AssignToGroups.Any() ) )
                {
                    foreach ( var touchTemplate in touchTemplates.Where( t => t.AssignToGroups.Any() ).OrderBy( t => t.Order ) )
                    {
                        foreach ( var groupId in touchTemplate.AssignToGroups )
                        {
                            AssignToGroupMember( careNeed, enableLogging, assignedPeople, rockContext, careAssigneeService, addedWorkerAliasIds, closedStatusId, groupId, currentlyAssignedPeople, touchTemplate, careNeedHistory: careNeedHistory );
                        }
                    }
                }
            }

            if ( !previewAssigned )
            {
                careAssigneeService.AddRange( assignedPeople );
                rockContext.WrapTransaction( () =>
                {
                    if ( rockContext.SaveChanges() > 0 )
                    {
                        if ( careNeedHistory.Any() )
                        {
                            HistoryService.SaveChanges(
                                rockContext,
                                typeof( CareNeed ),
                                SystemGuid.Category.HISTORY_CARE_NEED.AsGuid(),
                                careNeed.Id,
                                careNeedHistory,
                                "Auto Assign People",
                                null,
                                null
                            );
                        }
                    }
                } );
            }
            return assignedPeople;
        }

        private static void AssignToGroupMember( CareNeed careNeed, bool enableLogging, List<AssignedPerson> assignedPeople, RockContext rockContext, AssignedPersonService careAssigneeService, List<int?> addedWorkerAliasIds, int closedStatusId, int groupId, IQueryable<AssignedPerson> currentlyAssignedPeople, TouchTemplate touchTemplate = null, History.HistoryChangeList careNeedHistory = null )
        {
            if ( careNeedHistory == null )
            {
                careNeedHistory = new History.HistoryChangeList();
            }

            var groupMemberService = new GroupMemberService( rockContext );

            var groupMembers = groupMemberService
                .GetByGroupId( groupId )
                .AsNoTracking()
                .Where( gm => !gm.IsArchived
                              && gm.GroupMemberStatus == GroupMemberStatus.Active )
                .OrderBy( gm => currentlyAssignedPeople.Count( ap => ap.PersonAlias.PersonId == gm.Person.Id && ap.CareNeed.StatusValueId != closedStatusId ) )
                .ThenBy( gm => currentlyAssignedPeople.Where( ap => ap.PersonAlias.PersonId == gm.Person.Id ).OrderByDescending( ap => ap.CreatedDateTime ).Select( ap => ap.CreatedDateTime ).FirstOrDefault() )
                .ToList();

            var groupMemberAssignedCount = 0;
            foreach ( var gm in groupMembers )
            {
                if ( !addedWorkerAliasIds.Contains( gm.Person.PrimaryAliasId ) && gm.Person.PrimaryAlias != null && gm.Person.PrimaryAliasId != careNeed.PersonAliasId && careAssigneeService.GetByPersonAliasAndCareNeed( gm.Person.PrimaryAlias.Id, careNeed.Id ) == null )
                {
                    var sourceOfChange = "";
                    var careAssignee = new AssignedPerson { Id = 0 };
                    careAssignee.CareNeed = careNeed;
                    careAssignee.PersonAliasId = gm.Person.PrimaryAliasId;
                    careAssignee.FollowUpWorker = false;
                    if ( touchTemplate != null )
                    {
                        careAssignee.Type = AssignedType.TouchTemplateGroup;
                        careAssignee.TypeQualifier = $"{touchTemplate.NoteTemplate.Note}^{touchTemplate.MinimumCareTouches}^{gm.Group.Id}^{gm.Group.Name}^{gm.Id}^{currentlyAssignedPeople.Count( ap => ap.PersonAlias.PersonId == gm.Person.Id )}^{touchTemplate.MinimumCareTouchHours}";
                        // This TypeQualifier could have a tighter connection to the attribute matrix touch template if we somehow identified the touch template, such as using an attribute or attribute value id.
                        sourceOfChange = $"Touch Template Assignment - {touchTemplate.NoteTemplate.Note} ({gm.Group.Name} [{gm.GroupId}])";
                    }
                    else
                    {
                        careAssignee.Type = AssignedType.CategoryGroup;
                        careAssignee.TypeQualifier = $"{careNeed.CategoryValueId}^{careNeed.Category?.Value ?? DefinedValueCache.Get( careNeed.CategoryValueId.Value ).Value}^{gm.Group.Id}^{gm.Group.Name}^{gm.Id}^{currentlyAssignedPeople.Count( ap => ap.PersonAlias.PersonId == gm.Person.Id )}";
                        sourceOfChange = $"Category Assignment - {careNeed.Category?.Value ?? DefinedValueCache.Get( careNeed.CategoryValueId.Value ).Value} ({gm.Group.Name} [{gm.GroupId}])";

                    }
                    assignedPeople.Add( careAssignee );
                    addedWorkerAliasIds.Add( careAssignee.PersonAliasId );

                    string newStringValue = History.GetValue<PersonAlias>( gm.Person.PrimaryAlias, gm.Person.PrimaryAliasId, rockContext, "" );
                    careNeedHistory.AddChange( History.HistoryVerb.Add, History.HistoryChangeType.Record, "Assign Person", null, newStringValue ).SourceOfChange = sourceOfChange;
                    AddPersonHistory( rockContext, careNeed, careNeed.PersonAlias.Person, gm.Person.PrimaryAlias, "ASSIGNED", sourceOfChange );

                    groupMemberAssignedCount++;
                    if ( touchTemplate == null || groupMemberAssignedCount >= touchTemplate.MinimumCareTouches )
                    {
                        break;
                    }

                    if ( enableLogging )
                    {
                        LogEvent( null, "AutoAssignWorkers", string.Format( "Care Need Guid: {0}, GroupMember.Id: {1}, PersonId: {2}", careNeed.Guid, gm.Id, gm.PersonId ), "TouchTemplate GroupMember Assigned" );
                    }
                }
            }
        }

        public static void SendWorkerNotification( RockContext rockContext, CareNeed careNeed, bool isNew, List<AssignedPerson> newlyAssignedPersons, Guid? assignmentEmailTemplateGuid, RockPage rockPage = null, string detailPagePath = "", string dashboardPagePath = "", Person person = null )
        {
            if ( rockContext == null )
            {
                rockContext = new RockContext();
            }

            var assignedPersons = careNeed.AssignedPersons;
            if ( newlyAssignedPersons.Any() && !isNew )
            {
                assignedPersons = newlyAssignedPersons;
            }
            else
            {
                // Reload Care Need after save changes
                careNeed = new CareNeedService( new RockContext() ).Get( careNeed.Id );
                assignedPersons = careNeed.AssignedPersons;
                if ( careNeed.ChildNeeds != null && careNeed.ChildNeeds.Any() )
                {
                    foreach ( var need in careNeed.ChildNeeds )
                    {
                        assignedPersons.AddRange( need.AssignedPersons );
                    }
                }
            }

            if ( assignedPersons != null && assignedPersons.Any() && assignmentEmailTemplateGuid.HasValue && ( isNew || newlyAssignedPersons.Any() ) )
            {
                var errors = new List<string>();
                var errorsSms = new List<string>();
                Dictionary<string, object> linkedPages = new Dictionary<string, object>();
                linkedPages.Add( "CareDetail", ( rockPage != null ) ? rockPage.PageReference.BuildUrl() : detailPagePath );
                linkedPages.Add( "CareDashboard", ( rockPage != null ) ? GetParentPage( rockPage.PageId ).BuildUrl() : dashboardPagePath );

                var noteTemplates = new NoteTemplateService( rockContext ).Queryable().AsNoTracking().Where( n => n.IsActive ).OrderBy( nt => nt.Order );

                var systemCommunication = new SystemCommunicationService( rockContext ).Get( assignmentEmailTemplateGuid.Value );
                var emailMessage = new RockEmailMessage( systemCommunication );
                var smsMessage = new RockSMSMessage( systemCommunication );
                if ( rockPage != null )
                {
                    emailMessage.AppRoot = smsMessage.AppRoot = rockPage.ResolveRockUrl( "~/" );
                    emailMessage.ThemeRoot = smsMessage.ThemeRoot = rockPage.ResolveRockUrl( "~~/" );
                }
                foreach ( var assignee in assignedPersons )
                {
                    assignee.PersonAlias.Person.LoadAttributes();
                    var smsNumber = assignee.PersonAlias.Person.PhoneNumbers.GetFirstSmsNumber();
                    if ( !assignee.PersonAlias.Person.CanReceiveEmail( false ) && smsNumber.IsNullOrWhiteSpace() )
                    {
                        continue;
                    }

                    var mergeFields = Rock.Lava.LavaHelper.GetCommonMergeFields( rockPage, ( rockPage != null ) ? rockPage.CurrentPerson : person );
                    mergeFields.Add( "CareNeed", careNeed );
                    mergeFields.Add( "LinkedPages", linkedPages );
                    mergeFields.Add( "AssignedPerson", assignee );
                    mergeFields.Add( "Person", assignee.PersonAlias.Person );
                    mergeFields.Add( "NoteTemplates", noteTemplates );

                    var notificationType = assignee.PersonAlias.Person.GetAttributeValue( rocks.kfs.StepsToCare.SystemGuid.PersonAttribute.NOTIFICATION.AsGuid() );

                    if ( notificationType == null || notificationType == "Email" || notificationType == "Both" )
                    {
                        if ( !assignee.PersonAlias.Person.CanReceiveEmail( false ) )
                        {
                            var emailWarningMessage = string.Format( "{0} does not have a valid email address.", assignee.PersonAlias.Person.FullName );
                            RockLogger.Log.Warning( "RockWeb.Plugins.rocks_kfs.StepsToCare.CareEntry", emailWarningMessage );
                            errors.Add( emailWarningMessage );
                        }
                        else
                        {
                            emailMessage.AddRecipient( new RockEmailMessageRecipient( assignee.PersonAlias.Person, mergeFields ) );
                        }
                    }

                    if ( notificationType == "SMS" || notificationType == "Both" )
                    {
                        if ( string.IsNullOrWhiteSpace( smsNumber ) )
                        {
                            var smsWarningMessage = string.Format( "No SMS number could be found for {0}.", assignee.PersonAlias.Person.FullName );
                            RockLogger.Log.Warning( "RockWeb.Plugins.rocks_kfs.StepsToCare.CareEntry", smsWarningMessage );
                            errorsSms.Add( smsWarningMessage );
                        }

                        smsMessage.AddRecipient( new RockSMSMessageRecipient( assignee.PersonAlias.Person, smsNumber, mergeFields ) );
                    }
                }
                if ( emailMessage.GetRecipients().Count > 0 )
                {
                    emailMessage.Send( out errors );
                }
                if ( smsMessage.GetRecipients().Count > 0 )
                {
                    smsMessage.Send( out errorsSms );
                }

                if ( errors.Any() || errorsSms.Any() )
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append( string.Format( "{0} Errors Sending Care Assignment Notification: ", errors.Count + errorsSms.Count ) );
                    errors.ForEach( es => { sb.AppendLine(); sb.Append( es ); } );
                    errorsSms.ForEach( es => { sb.AppendLine(); sb.Append( es ); } );
                    string errorStr = sb.ToString();
                    var exception = new Exception( errorStr );
                    HttpContext context = HttpContext.Current;
                    ExceptionLogService.LogException( exception, context );
                }
            }
        }

        private static IOrderedQueryable<WorkerResult> GenerateAgeQuery( CareNeed careNeed, IQueryable<CareWorker> careWorkersNoFence, int closedId, bool includeAgeRange, bool includeGender, bool includeCampus, bool includeCategory, bool ignoreAgeRangeAndGender = false )
        {
            var ageAsDecimal = ( decimal? ) careNeed.PersonAlias.Person.AgePrecise;
            var tempQuery = careWorkersNoFence;

            if ( includeAgeRange )
            {
                tempQuery = tempQuery.Where( cw => ageAsDecimal.HasValue && (
                                                    ( cw.AgeRangeMin.HasValue && cw.AgeRangeMax.HasValue && ( ageAsDecimal > cw.AgeRangeMin.Value && ageAsDecimal < cw.AgeRangeMax.Value ) ) ||
                                                    ( !cw.AgeRangeMin.HasValue && cw.AgeRangeMax.HasValue && ageAsDecimal < cw.AgeRangeMax.Value ) ||
                                                    ( cw.AgeRangeMin.HasValue && !cw.AgeRangeMax.HasValue && ageAsDecimal > cw.AgeRangeMin.Value )
                                                   )
                                            );
            }
            else if ( !ignoreAgeRangeAndGender )
            {
                tempQuery = tempQuery.Where( cw => ageAsDecimal.HasValue && !(
                                                     ( cw.AgeRangeMin.HasValue && cw.AgeRangeMax.HasValue && ( ageAsDecimal > cw.AgeRangeMin.Value && ageAsDecimal < cw.AgeRangeMax.Value ) ) ||
                                                     ( !cw.AgeRangeMin.HasValue && cw.AgeRangeMax.HasValue && ageAsDecimal < cw.AgeRangeMax.Value ) ||
                                                     ( cw.AgeRangeMin.HasValue && !cw.AgeRangeMax.HasValue && ageAsDecimal > cw.AgeRangeMin.Value )
                                                    )
                                            );
            }

            if ( includeGender )
            {
                tempQuery = tempQuery.Where( cw => cw.Gender == careNeed.PersonAlias.Person.Gender );
            }
            else if ( !ignoreAgeRangeAndGender )
            {
                tempQuery = tempQuery.Where( cw => cw.Gender != careNeed.PersonAlias.Person.Gender );
            }

            if ( includeCategory )
            {
                tempQuery = tempQuery.Where( cw => cw.CategoryValues.Contains( careNeed.CategoryValueId.ToString() ) );
            }
            else
            {
                tempQuery = tempQuery.Where( cw => !cw.CategoryValues.Contains( careNeed.CategoryValueId.ToString() ) );
            }

            if ( includeCampus )
            {
                tempQuery = tempQuery.Where( cw => cw.Campuses.Contains( careNeed.CampusId.ToString() ) );
            }
            else
            {
                tempQuery = tempQuery.Where( cw => !cw.Campuses.Contains( careNeed.CampusId.ToString() ) );
            }

            return tempQuery.Select( cw => new WorkerResult
            {
                Count = cw.AssignedPersons.Where( ap => ap.CareNeed != null && ap.CareNeed.StatusValueId != closedId ).Count(),
                LastAssignmentDate = cw.AssignedPersons.Where( ap => ap.CareNeed != null && ap.CareNeed.StatusValueId != closedId ).OrderByDescending( ap => ap.CreatedDateTime ).Select( ap => ap.CreatedDateTime ).FirstOrDefault(),
                Worker = cw,
                HasCategory = includeCategory,
                HasCampus = includeCampus,
                HasAgeRange = includeAgeRange,
                HasGender = includeGender
            }
                                    )
                                    .OrderBy( cw => cw.Count )
                                    .ThenBy( cw => cw.LastAssignmentDate )
                                    .ThenBy( cw => cw.Worker.CategoryValues.Contains( careNeed.CategoryValueId.ToString() ) )
                                    .ThenBy( cw => cw.Worker.Campuses.Contains( careNeed.CampusId.ToString() ) );
        }


        public static void AddPersonHistory( RockContext rockContext, CareNeed careNeed, Person person, PersonAlias assignedPersonAlias, string historyVerb, string sourceOfChange = null, bool commitSave = false )
        {
            var assignedPersonHistory = new History.HistoryChangeList();
            assignedPersonHistory.AddCustom( historyVerb, "Record", $"{assignedPersonAlias.Person.FullName} [{assignedPersonAlias.PersonId}]</span> {( ( historyVerb == "ASSIGNED" ) ? "to" : "from" )} Care Need [{careNeed.Id}] for <span class='field-value'>{person.FullName}</span><span class='field-value'>" ).SourceOfChange = sourceOfChange;
            if ( assignedPersonHistory.Any() )
            {
                HistoryService.SaveChanges(
                    rockContext,
                    typeof( Person ),
                    SystemGuid.Category.HISTORY_PERSON_STEPS_TO_CARE.AsGuid(),
                    assignedPersonAlias.PersonId,
                    assignedPersonHistory,
                    $"Care Need for {person.FullName}",
                    typeof( CareNeed ),
                    careNeed.Id,
                    commitSave );
            }
        }

        public static DefinedValue DefinedValueFromCache( string definedValueGuid )
        {
            var definedValueCache = DefinedValueCache.Get( definedValueGuid );
            var definedValue = new DefinedValue
            {
                Id = definedValueCache.Id,
                Guid = definedValueCache.Guid,
                Value = definedValueCache.Value,
                Description = definedValueCache.Description,
                Order = definedValueCache.Order,
                IsActive = definedValueCache.IsActive
            };
            return definedValue;
        }

        public static DefinedValue DefinedValueFromCache( int? definedValueId )
        {
            var definedValueCache = DefinedValueCache.Get( definedValueId ?? 0 );
            return DefinedValueFromCache( definedValueCache.Guid.ToString() );
        }

        private static PageReference GetParentPage( int pageId )
        {
            var pageCache = PageCache.Get( pageId );
            if ( pageCache != null )
            {
                var parentPage = pageCache.ParentPage;
                if ( parentPage != null )
                {
                    return new PageReference( parentPage.Guid.ToString() );
                }
            }
            return new PageReference( pageCache.Guid.ToString() );
        }

        public static ServiceLog LogEvent( RockContext rockContext, string type, string input, string result )
        {
            if ( rockContext == null )
            {
                rockContext = new RockContext();
            }

            var rockLogger = new ServiceLogService( rockContext );
            ServiceLog serviceLog = new ServiceLog
            {
                Name = "Steps To Care",
                Type = type,
                LogDateTime = RockDateTime.Now,
                Input = input,
                Result = result,
                Success = true
            };
            rockLogger.Add( serviceLog );
            rockContext.SaveChanges();
            return serviceLog;
        }
    }
}