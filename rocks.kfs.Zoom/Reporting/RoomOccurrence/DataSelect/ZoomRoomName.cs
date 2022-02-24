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

using Rock;
using Rock.Model;
using Rock.Reporting;

using rocks.kfs.Zoom.Model;

namespace rocks.kfs.Zoom.Reporting.RoomOccurrence.DataSelect
{
    /// <summary>
    /// A tabular report field that displays the name of the Zoom Room associated with the RoomOccurrence.
    /// </summary>
    [Description( "Show Zoom Room Name" )]
    [Export( typeof( DataSelectComponent ) )]
    [ExportMetadata( "ComponentName", "Select Zoom Room Name" )]
    public class ZoomRoomNameSelect : DataSelectComponent
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
                return "Zoom Room Name";
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
                return "Zoom Room Fields";
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
            var zoomRoomDTGuid = ZoomGuid.DefinedType.ZOOM_ROOM.AsGuid();
            var definedValueQuery = new DefinedValueService( context ).Queryable( "DefinedType" )
                .Where( dv => dv.DefinedType.Guid == zoomRoomDTGuid )
                .Select( dv => new { GuidString = dv.Guid.ToString(), Description = dv.Description } );

            var serviceInstance = new AttributeValueService( context );

            var zRLocationEntityAttGuid = ZoomGuid.Attribute.ZOOM_ROOM_LOCATION_ENTITY_ATTRIBUTE.AsGuid();
            var valuesQuery = serviceInstance.Queryable( "Attribute" )
                .Where( x => x.Attribute.Guid == zRLocationEntityAttGuid )
                .Select( x => new { EntityId = x.EntityId, Value = x.Value } );

            var occurrenceService = new RoomOccurrenceService( context );

            var resultQuery = occurrenceService.Queryable( "Location" )
                .Select( ro => definedValueQuery.FirstOrDefault( dv => valuesQuery.FirstOrDefault( v => v.EntityId == ro.Location.Id && v.Value == dv.GuidString ).Value == dv.GuidString ).Description );

            return SelectExpressionExtractor.Extract( resultQuery, entityIdProperty, "ro" );
        }
    }
}
