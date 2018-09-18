using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;

using Rock;
using Rock.Attribute;
using Rock.CheckIn;
using Rock.Data;
using Rock.Model;
using Rock.Workflow;
using Rock.Workflow.Action.CheckIn;

namespace com.kfs.Workflow.Action.CheckIn
{
    /// <summary>
    /// Adds the locations for each members group types
    /// </summary>
    [ActionCategory( "KFS: Check-In" )]
    [Description( "If group allows, the locations will be sorted by number of current attendees" )]
    [Export( typeof( ActionComponent ) )]
    [ExportMetadata( "ComponentName", "Load Balance Locations" )]
    [BooleanField( "Load All", "By default locations are only loaded for the selected person and group type.  Select this option to load locations for all the loaded people and group types." )]
    public class LoadBalanceLocations : CheckInActionComponent
    {
        /// <summary>
        /// Executes the specified workflow.
        /// </summary>
        /// <param name="rockContext">The rock context.</param>
        /// <param name="action">The workflow action.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="errorMessages">The error messages.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override bool Execute( RockContext rockContext, WorkflowAction action, Object entity, out List<string> errorMessages )
        {
            var checkInState = GetCheckInState( entity, out errorMessages );
            if ( checkInState != null )
            {
                bool loadAll = GetAttributeValue( action, "LoadAll" ).AsBoolean();

                foreach ( var family in checkInState.CheckIn.GetFamilies( true ) )
                {
                    foreach ( var person in family.GetPeople( !loadAll ) )
                    {
                        foreach ( var groupType in person.GetGroupTypes( !loadAll ).ToList() )
                        {
                            var kioskGroupType = checkInState.Kiosk.ActiveGroupTypes( checkInState.ConfiguredGroupTypes )
                                .Where( g => g.GroupType.Id == groupType.GroupType.Id )
                                .FirstOrDefault();

                            if ( kioskGroupType != null )
                            {
                                var groups = groupType.GetGroups( !loadAll );
                                var groupIds = groups.Select( g => g.Group.Id ).ToList();
                                var dayStart = RockDateTime.Today;
                                var now = RockDateTime.Now;
                                var schedules = new List<Schedule>();

                                var attendanceQry = new AttendanceService( rockContext ).Queryable()
                                    .Where( a =>
                                        a.Occurrence.ScheduleId.HasValue &&
                                        a.Occurrence.GroupId.HasValue &&
                                        a.Occurrence.LocationId.HasValue &&
                                        a.StartDateTime > dayStart &&
                                        a.StartDateTime < now &&
                                        !a.EndDateTime.HasValue &&
                                        a.DidAttend.HasValue &&
                                        a.DidAttend.Value &&
                                        groupIds.Contains( a.Occurrence.GroupId.Value ) );

                                foreach ( var group in groups )
                                {
                                    var loadBalance = group.Group.GetAttributeValue( "com.kfs.LoadBalanceLocations" ).AsBoolean();
                                    if ( loadBalance && loadAll )
                                    {
                                        group.Locations.Clear();
                                    }

                                    var locationAttendance = new Dictionary<CheckInLocation, int>();

                                    foreach ( var kioskGroup in kioskGroupType.KioskGroups
                                        .Where( g => g.Group.Id == group.Group.Id && g.IsCheckInActive )
                                        .ToList() )
                                    {
                                        foreach ( var kioskLocation in kioskGroup.KioskLocations.Where( l => l.IsCheckInActive && l.IsActiveAndNotFull ) )
                                        {
                                            if ( !group.Locations.Any( l => l.Location.Id == kioskLocation.Location.Id ) )
                                            {
                                                var checkInLocation = new CheckInLocation();
                                                checkInLocation.Location = kioskLocation.Location.Clone( false );
                                                checkInLocation.Location.CopyAttributesFrom( kioskLocation.Location );
                                                checkInLocation.CampusId = kioskLocation.CampusId;
                                                checkInLocation.Order = kioskLocation.Order;

                                                var activeSchedules = kioskLocation.KioskSchedules.Select( s => s.Schedule.Id ).ToList();

                                                var attendanceList = attendanceQry.Where( a => a.Occurrence.LocationId.Value == kioskLocation.Location.Id ).ToList();
                                                if ( !loadBalance || !attendanceList.Any() )
                                                {
                                                    locationAttendance.Add( checkInLocation, 0 );
                                                }
                                                else
                                                {
                                                    foreach ( var groupLocSched in attendanceList
                                                        .Where( a =>
                                                            a.PersonAlias != null &&
                                                            activeSchedules.Contains( a.Occurrence.ScheduleId.Value ) )
                                                        .GroupBy( a => new
                                                        {
                                                            ScheduleId = a.Occurrence.ScheduleId.Value,
                                                            GroupId = a.Occurrence.GroupId.Value,
                                                            LocationId = a.Occurrence.LocationId.Value
                                                        } )
                                                        .Select( g => new
                                                        {
                                                            ScheduleId = g.Key.ScheduleId,
                                                            GroupId = g.Key.GroupId,
                                                            LocationId = g.Key.LocationId,
                                                            PersonIds = g.Select( a => a.PersonAlias.PersonId ).Distinct().ToList()
                                                        } ) )
                                                    {
                                                        locationAttendance.Add( checkInLocation, groupLocSched.PersonIds.Count() );
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    if ( loadBalance )
                                    {
                                        var sortedLocationAttendance = locationAttendance.ToList();
                                        sortedLocationAttendance.Sort( ( x, y ) => x.Key.Location.Name.CompareTo( y.Key.Location.Name ) );
                                        sortedLocationAttendance.Sort( ( x, y ) => x.Value.CompareTo( y.Value ) );
                                        var order = 0;
                                        foreach (var checkInLocationPair in sortedLocationAttendance)
                                        {
                                            var checkInLocation = checkInLocationPair.Key;
                                            checkInLocation.Order = order;
                                            group.Locations.Add( checkInLocation );

                                            order++;
                                        }
                                    }
                                    else
                                    {
                                        group.Locations.AddRange( locationAttendance.Select( l => l.Key ).ToList() );
                                    }
                                }
                            }
                        }
                    }
                }

                return true;
            }

            return false;
        }
    }
}
