// <copyright>
// Copyright 2020 by Kingdom First Solutions
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
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Linq.Expressions;
using System.Web.UI;
using Rock;
using Rock.Data;
using Rock.Model;
using Rock.Reporting;
using Rock.Web.Cache;
using Rock.Web.UI.Controls;

namespace rocks.kfs.Eventbrite.DataFilter
{
    /// <summary>
    /// 
    /// </summary>
    [Description( "Filter groups by Eventbrite ticket class" )]
    [Export( typeof( DataFilterComponent ) )]
    [ExportMetadata( "ComponentName", "Eventbrite Ticket Class Filter" )]
    public class TicketClassFilter : DataFilterComponent
    {
        #region Properties

        /// <summary>
        /// Gets the entity type that filter applies to.
        /// </summary>
        /// <value>
        /// The entity that filter applies to.
        /// </value>
        public override string AppliesToEntityType
        {
            get { return typeof( Rock.Model.GroupMember ).FullName; }
        }

        /// <summary>
        /// Gets the section.
        /// </summary>
        /// <value>
        /// The section.
        /// </value>
        public override string Section
        {
            get { return "Additional Filters"; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <param name="entityType"></param>
        /// <returns></returns>
        /// <value>
        /// The title.
        /// </value>
        public override string GetTitle( Type entityType )
        {
            return "Eventbrite Ticket Class";
        }

        /// <summary>
        /// Formats the selection on the client-side.  When the filter is collapsed by the user, the Filterfield control
        /// will set the description of the filter to whatever is returned by this property.  If including script, the
        /// controls parent container can be referenced through a '$content' variable that is set by the control before 
        /// referencing this property.
        /// </summary>
        /// <value>
        /// The client format script.
        /// </value>
        public override string GetClientFormatSelection( Type entityType )
        {
            return @"
function() {
  var ticketClassName = $('.rock-text-box', $content).text()
  return ticketClassName;
}
";
        }

        /// <summary>
        /// Formats the selection.
        /// </summary>
        /// <param name="entityType">Type of the entity.</param>
        /// <param name="selection">The selection.</param>
        /// <returns></returns>
        public override string FormatSelection( Type entityType, string selection )
        {
            string result = "EB Ticket Class";
            if ( selection.IsNotNullOrWhiteSpace() )
            {
                result = string.Format( "Ticket Class: {0}", selection );
            }

            return result;
        }

        /// <summary>
        /// Creates the child controls.
        /// </summary>
        /// <returns></returns>
        public override Control[] CreateChildControls( Type entityType, FilterField filterControl )
        {
            RockTextBox tbTicketClass = new RockTextBox();
            tbTicketClass.ID = filterControl.ID + "_tbTicketClass";
            tbTicketClass.Label = "Eventbrite Ticket Class";
            filterControl.Controls.Add( tbTicketClass );

            return new Control[] { tbTicketClass };
        }

        /// <summary>
        /// Renders the controls.
        /// </summary>
        /// <param name="entityType">Type of the entity.</param>
        /// <param name="filterControl">The filter control.</param>
        /// <param name="writer">The writer.</param>
        /// <param name="controls">The controls.</param>
        public override void RenderControls( Type entityType, FilterField filterControl, HtmlTextWriter writer, Control[] controls )
        {
            base.RenderControls( entityType, filterControl, writer, controls );
        }

        /// <summary>
        /// Gets the selection.
        /// </summary>
        /// <param name="entityType">Type of the entity.</param>
        /// <param name="controls">The controls.</param>
        /// <returns></returns>
        public override string GetSelection( Type entityType, Control[] controls )
        {
            string ticketClass = ( controls[0] as RockTextBox ).Text;
            if ( ticketClass.IsNotNullOrWhiteSpace() )
            {
                return ticketClass;
            }

            return string.Empty;
        }

        /// <summary>
        /// Sets the selection.
        /// </summary>
        /// <param name="entityType">Type of the entity.</param>
        /// <param name="controls">The controls.</param>
        /// <param name="selection">The selection.</param>
        public override void SetSelection( Type entityType, Control[] controls, string selection )
        {
            if ( selection.IsNotNullOrWhiteSpace() )
            {
                ( controls[0] as RockTextBox ).Text = selection;
            }
        }

        /// <summary>
        /// Gets the expression.
        /// </summary>
        /// <param name="entityType">Type of the entity.</param>
        /// <param name="serviceInstance">The service instance.</param>
        /// <param name="parameterExpression">The parameter expression.</param>
        /// <param name="selection">The selection.</param>
        /// <returns></returns>
        public override Expression GetExpression( Type entityType, IService serviceInstance, ParameterExpression parameterExpression, string selection )
        {
            if ( selection.IsNotNullOrWhiteSpace() )
            {
                var ebPersonFieldType = FieldTypeCache.Get( EBGuid.FieldType.EVENTBRITE_PERSON.AsGuid() );
                var ebPersonAttribute = new AttributeService( ( RockContext ) serviceInstance.Context ).GetByFieldTypeId( ebPersonFieldType.Id ).FirstOrDefault();

                if ( ebPersonAttribute != null )
                {
                    var qry = new GroupMemberService( ( RockContext ) serviceInstance.Context ).Queryable()
                        .WhereAttributeValue( ( RockContext ) serviceInstance.Context, a => a.Attribute.Key == ebPersonAttribute.Key && a.Value != null && a.Value != "" && a.Value.Contains( "^" ) );

                    var groupMemberIds = new List<int>();
                    foreach ( var groupMember in qry.ToList() )
                    {
                        if ( groupMember.Attributes == null )
                        {
                            groupMember.LoadAttributes();
                        }
                        var attributeVal = groupMember.GetAttributeValue( ebPersonAttribute.Key );
                        if ( attributeVal.IsNotNullOrWhiteSpace() )
                        {
                            var containsValue = attributeVal.Split( new char[] { '^' } )[1].Split( new string[] { "||" }, StringSplitOptions.RemoveEmptyEntries ).Contains( selection );
                            if ( containsValue )
                            {
                                groupMemberIds.Add( groupMember.Id );
                            }
                        }

                    }
                    qry = qry.Where( gm => groupMemberIds.Contains( gm.Id ) );

                    Expression extractedFilterExpression = FilterExpressionExtractor.Extract<Rock.Model.GroupMember>( qry, parameterExpression, "gm" );

                    return extractedFilterExpression;
                }
            }

            return null;
        }
        #endregion
    }
}