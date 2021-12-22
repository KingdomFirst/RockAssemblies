using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Data.Entity;
using System.Linq;
using System.Web.UI.WebControls;
using Rock.Attribute;
using Rock.Data;
using Rock.Model;
using Rock.Reporting;
using Rock.Web.Cache;

using rocks.kfs.Zoom.ZoomGuid;
using rocks.kfs.Zoom.Model;
using Rock;

namespace rocks.kfs.Zoom.Reporting.RoomOccurrence.DataSelect
{
    /// <summary>
    /// A tabular report field that displays the Zoom Room Id related to a RoomOccurrence.
    /// </summary>
    [Description( "Show Zoom Room Id" )]
    [Export( typeof( DataSelectComponent ) )]
    [ExportMetadata( "ComponentName", "Select Zoom Room Id" )]
    public class ZoomRoomIdSelect : DataSelectComponent
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
                return "Zoom Room Id";
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
            var definedValueQuery = new DefinedValueService( context ).Queryable( "DefinedType" )
                .Where( dv => dv.DefinedType.Guid == ZoomGuid.DefinedType.ZOOM_ROOM.AsGuid() )
                .Select( dv => new { GuidString = dv.Guid.ToString(), Value = dv.Value } );

            var serviceInstance = new AttributeValueService( context );

            var valuesQuery = serviceInstance.Queryable()
                .Where( x => x.Attribute.Guid == ZoomGuid.Attribute.ZOOM_ROOM_LOCATION_ENTITY_ATTRIBUTE.AsGuid() )
                .Select( x => new { EntityId = x.EntityId, Value = x.Value } );

            var occurrenceService = new RoomOccurrenceService( context );

            var resultQuery = occurrenceService.Queryable( "Location" )
                .Select( ro => definedValueQuery.FirstOrDefault( dv => valuesQuery.FirstOrDefault( v => v.EntityId == ro.Location.Id && v.Value == dv.GuidString ).Value == dv.GuidString ).Value );

            return SelectExpressionExtractor.Extract( resultQuery, entityIdProperty, "ro" );
        }
    }
}
