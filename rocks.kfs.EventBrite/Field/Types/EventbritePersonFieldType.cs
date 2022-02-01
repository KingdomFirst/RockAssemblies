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
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using EventbriteDotNetFramework;
using Rock;
using Rock.Data;
using Rock.Field;
using Rock.Web.UI.Controls;
using rocks.kfs.Eventbrite.Utility.ExtensionMethods;

namespace rocks.kfs.Eventbrite.Field.Types
{
    [Serializable]
    public class EventbritePersonFieldType : FieldType
    {

        #region Edit Control

        #region Formatting

        /// <summary>
        /// Returns the field's current value(s)
        /// </summary>
        /// <param name="parentControl">The parent control.</param>
        /// <param name="value">Information about the value</param>
        /// <param name="configurationValues">The configuration values.</param>
        /// <param name="condensed">Flag indicating if the value should be condensed (i.e. for use in a grid column)</param>
        /// <returns></returns>
        public override string FormatValue( Control parentControl, string value, Dictionary<string, ConfigurationValue> configurationValues, bool condensed )
        {
            string formattedValue = value;

            var splitValues = value.SplitDelimitedValues( "^" );
            if ( splitValues.Length > 3 )
            {
                var ticketClasses = splitValues[1].SplitDelimitedValues( "||" );
                var ticketQuantities = splitValues[3].SplitDelimitedValues( "||" );
                if ( ticketClasses.Length > 1 )
                {
                    var ticketString = new StringBuilder();
                    for ( var i = 0; i < ticketClasses.Length; i++ )
                    {
                        ticketString.AppendFormat( "{0} - Qty: {1}, ", ticketClasses[i], ticketQuantities[i] );
                    }
                    formattedValue = string.Format( "{1} <strong>Attendee id:</strong> {0}, <strong>Order id:</strong> {2}", splitValues[0], ticketString.ToString(), splitValues[2] );
                }
                else
                {
                    formattedValue = string.Format( "<strong>Ticket Qty:</strong> {3}, <strong>Ticket Class:</strong> {1}, <strong>Attendee id:</strong> {0}, <strong>Order id:</strong> {2}", splitValues[0], splitValues[1], splitValues[2], splitValues[3] );
                }
            }
            else if ( splitValues.Length > 2 )
            {
                formattedValue = string.Format( "<strong>Ticket Class:</strong> {1}, <strong>Attendee id:</strong> {0}, <strong>Order id:</strong> {2}", splitValues[0], splitValues[1], splitValues[2] );
            }
            else
            {
                formattedValue = value;
            }


            return base.FormatValue( parentControl, formattedValue, configurationValues, condensed );
        }

        #endregion

        /// <summary>
        /// Creates the control(s) necessary for prompting user for a new value
        /// </summary>
        /// <param name="configurationValues">The configuration values.</param>
        /// <param name="id"></param>
        /// <returns>
        /// The control
        /// </returns>
        public override Control EditControl( Dictionary<string, ConfigurationValue> configurationValues, string id )
        {
            var editControl = new LiteralControl();
            return editControl;
        }

        /// <summary>
        /// Reads new values entered by the user for the field
        /// </summary>
        /// <param name="control">Parent control that controls were added to in the CreateEditControl() method</param>
        /// <param name="configurationValues"></param>
        /// <returns></returns>
        public override string GetEditValue( Control control, Dictionary<string, ConfigurationValue> configurationValues )
        {
            var editControl = control as LiteralControl;
            if ( editControl != null )
            {
                return editControl.Text;
            }

            return null;
        }

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="configurationValues"></param>
        /// <param name="value">The value.</param>
        public override void SetEditValue( Control control, Dictionary<string, ConfigurationValue> configurationValues, string value )
        {
            var editControl = control as LiteralControl;
            if ( editControl != null )
            {
                var splitValues = value.SplitDelimitedValues( "^" );
                if ( splitValues.Length > 3 )
                {
                    var ticketClasses = splitValues[1].SplitDelimitedValues( "||" );
                    var ticketQuantities = splitValues[3].SplitDelimitedValues( "||" );
                    if ( ticketClasses.Length > 1 )
                    {
                        var ticketString = new StringBuilder();
                        for ( var i = 0; i < ticketClasses.Length; i++ )
                        {
                            ticketString.AppendFormat( "{0} - Qty: {1}<br>", ticketClasses[i], ticketQuantities[i] );
                        }
                        editControl.Text = string.Format( "<dl><dt>Attendee id:</dt><dd>{0}</dd><dt>Ticket Classes:</dt><dd>{1}</dd><dt>Order id:</dt><dd>{2}</dd></dl>", splitValues[0], ticketString.ToString(), splitValues[2] );
                    }
                    else
                    {
                        editControl.Text = string.Format( "<dl><dt>Attendee id:</dt><dd>{0}</dd><dt>Ticket Class:</dt><dd>{1}</dd><dt>Order id:</dt><dd>{2}</dd><dt>Ticket Quantity:</dt><dd>{3}</dd></dl>", splitValues[0], splitValues[1], splitValues[2], splitValues[3] );
                    }
                }
                else if ( splitValues.Length > 2 )
                {
                    editControl.Text = string.Format( "<dl><dt>Attendee id:</dt><dd>{0}</dd><dt>Ticket Class:</dt><dd>{1}</dd><dt>Order id:</dt><dd>{2}</dd></dl>", splitValues[0], splitValues[1], splitValues[2] );
                }
                else
                {
                    editControl.Text = value;
                }
            }
        }

        #endregion

    }
}