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
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Web.UI.WebControls;
using com.bemaservices.RoomManagement.Model;
using Rock;
using Rock.Model;
using Rock.Reporting;

using rocks.kfs.Zoom.Model;

namespace rocks.kfs.Zoom.Reporting.RoomOccurrence.DataSelect
{
    /// <summary>
    /// A tabular report field that displays the Group set on the Notify Group attribute of the Reservation.
    /// </summary>
    [Description( "Show Reservation Notify Group" )]
    [Export( typeof( DataSelectComponent ) )]
    [ExportMetadata( "ComponentName", "Select Reservation Notify Group" )]
    public class NotifyGroupSelect : DataSelectComponent
    {
        /// <summary>
        /// Gets the name of the entity type. Filter should be an empty string
        /// if it applies to all entities
        /// </summary>
        /// <value>
        /// The name of the entity type.
        /// </value>
        public override string AppliesToEntityType
        {
            get
            {
                return typeof( Model.RoomOccurrence ).FullName;
            }
        }

        /// <summary>
        /// The PropertyName of the property in the anonymous class returned by the SelectExpression
        /// </summary>
        /// <value>
        /// The name of the column property.
        /// </value>
        public override string ColumnPropertyName
        {
            get
            {
                return "Reservation Notify Group";
            }
        }

        /// <summary>
        /// Gets the section that this will appear in the Field Selector
        /// </summary>
        /// <value>
        /// The section.
        /// </value>
        public override string Section
        {
            get
            {
                return "Room Reservation Fields";
            }
        }

        /// <summary>
        /// Gets the grid field.
        /// </summary>
        /// <param name="entityType">Type of the entity.</param>
        /// <param name="selection">The selection.</param>
        /// <returns></returns>
        public override System.Web.UI.WebControls.DataControlField GetGridField( Type entityType, string selection )
        {
            var result = new BoundField();

            return result;
        }

        /// <summary>
        /// Gets the expression.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="entityIdProperty">The entity identifier property.</param>
        /// <param name="selection">The selection.</param>
        /// <returns></returns>
        public override System.Linq.Expressions.Expression GetExpression( Rock.Data.RockContext context, System.Linq.Expressions.MemberExpression entityIdProperty, string selection )
        {
            var attributeValueService = new AttributeValueService( context );
            var roomReservationGroupAttGuid = ZoomGuid.Attribute.ROOM_RESERVATION_GROUP_ATTRIBUTE.AsGuid();
            var reservationLocationEntityTypeId = new EntityTypeService( context ).GetNoTracking( com.bemaservices.RoomManagement.SystemGuid.EntityType.RESERVATION_LOCATION.AsGuid() ).Id;

            var resGroupAttValues = attributeValueService.Queryable()
                .Where( x => x.Attribute.Guid == roomReservationGroupAttGuid )
                .Select( x => new { EntityId = x.EntityId, Value = x.Value } );

            var groupQuery = new GroupService( context ).Queryable()
                .Select( g => new { GuidString = g.Guid.ToString(), GroupName = g.Name + " (" + g.Id.ToString() + ")" } );

            var reservationQuery = new ReservationService( context ).Queryable()
                .Select( r => new { r.Id } );

            var reservationlocationQuery = new ReservationLocationService( context ).Queryable()
                .Select( rl => new { rl.Id, rl.ReservationId } );

            var occurrenceService = new RoomOccurrenceService( context );

            var resultQuery = occurrenceService.Queryable( "ReservationLocation" )
                .Select( ro => groupQuery.FirstOrDefault( g => resGroupAttValues.FirstOrDefault( v => reservationQuery.FirstOrDefault( r => reservationlocationQuery.FirstOrDefault( rl => ro.EntityTypeId == reservationLocationEntityTypeId && rl.Id == ro.EntityId ).ReservationId == r.Id ).Id == v.EntityId ).Value.Contains( g.GuidString ) ).GroupName );

            return SelectExpressionExtractor.Extract( resultQuery, entityIdProperty, "ro" );
        }
    }
}
