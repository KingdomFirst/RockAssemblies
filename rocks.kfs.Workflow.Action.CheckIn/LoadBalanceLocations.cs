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
// * Added the ability to load balance the selected location.
// </notice>
//
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Data.Entity;
using System.Linq;

using Rock;
using Rock.Attribute;
using Rock.CheckIn;
using Rock.Data;
using Rock.Model;
using Rock.Workflow;
using Rock.Workflow.Action.CheckIn;

namespace rocks.kfs.Workflow.Action.CheckIn
{
    #region Action Attributes

    [ActionCategory( "KFS: Check-In" )]
    [Description( "If group allows, the locations will be sorted by number of current attendees" )]
    [Export( typeof( ActionComponent ) )]
    [ExportMetadata( "ComponentName", "Load Balance Locations" )]

    #endregion

    #region Action Settings

    [BooleanField( "Load All", "By default locations are only loaded for the selected person and group type.  Select this option to load locations for all the loaded people and group types." )]

    #endregion

    /// <summary>
    /// Adds the locations for each members group types
    /// </summary>
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
                                foreach ( var group in groupType.GetGroups( !loadAll ) )
                                {
                                    var closedGroupLocationIds = new AttendanceOccurrenceService( rockContext )
                                                    .Queryable()
                                                    .AsNoTracking()
                                                    .Where( o =>
                                                        o.GroupId == group.Group.Id &&
                                                        o.OccurrenceDate == RockDateTime.Today )
                                                    .WhereAttributeValue( rockContext, "rocks.kfs.OccurrenceClosed", "True" )
                                                    .Select( l => l.LocationId )
                                                    .ToList();

                                    var loadBalance = group.Group.GetAttributeValue( "rocks.kfs.LoadBalanceLocations" ).AsBoolean();
                                    if ( loadBalance && loadAll )
                                    {
                                        group.Locations.Clear();
                                    }

                                    var locationAttendance = new Dictionary<CheckInLocation, int>();

                                    foreach ( var kioskGroup in kioskGroupType.KioskGroups
                                        .Where( g => g.Group.Id == group.Group.Id && g.IsCheckInActive )
                                        .ToList() )
                                    {
                                        foreach ( var kioskLocation in kioskGroup.KioskLocations.Where( l => l.IsCheckInActive && l.IsActiveAndNotFull && !closedGroupLocationIds.Contains( l.Location.Id ) ) )
                                        {
                                            if ( !group.Locations.Any( l => l.Location.Id == kioskLocation.Location.Id ) )
                                            {
                                                var checkInLocation = new CheckInLocation();
                                                checkInLocation.Location = kioskLocation.Location.Clone( false );
                                                checkInLocation.Location.CopyAttributesFrom( kioskLocation.Location );
                                                checkInLocation.CampusId = kioskLocation.CampusId;
                                                checkInLocation.Order = kioskLocation.Order;
                                                locationAttendance.Add( checkInLocation, KioskLocationAttendance.Get( checkInLocation.Location.Id ).CurrentCount );
                                            }
                                        }
                                    }

                                    if ( loadBalance )
                                    {
                                        var sortedLocationAttendance = locationAttendance.ToList();
                                        sortedLocationAttendance.Sort( ( x, y ) => x.Key.Location.Name.CompareTo( y.Key.Location.Name ) );
                                        sortedLocationAttendance.Sort( ( x, y ) => x.Value.CompareTo( y.Value ) );
                                        var order = 0;
                                        foreach ( var checkInLocationPair in sortedLocationAttendance )
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
