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
using EventbriteDotNetFramework.Responses;
using Rock;
using Rock.Data;
using Rock.Field;
using Rock.Web.UI.Controls;
using rocks.kfs.Eventbrite.Utility.ExtensionMethods;

namespace rocks.kfs.Eventbrite.Field.Types
{
    [Serializable]
    public class EventbriteEventFieldType : FieldType
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
            var splitVal = value.SplitDelimitedValues( "^" );
            long? longVal = ( splitVal.Count() > 0 ) ? splitVal[0].AsLongOrNull() : null;
            if ( longVal.HasValue )
            {
                var eb = new EBApi( Settings.GetAccessToken(), Settings.GetOrganizationId().ToLong( 0 ) );
                var eventbriteEvent = eb.GetEventById( longVal.Value );
                var linkedEventTickets = eb.GetTicketsById( longVal.Value );
                var sold = 0;
                var total = 0;
                foreach ( var ticket in linkedEventTickets.Ticket_Classes )
                {
                    sold += ticket.QuantitySold;
                    total += ticket.QuantityTotal;
                }
                var available = total - sold;

                if ( eventbriteEvent != null )
                {
                    if ( condensed )
                    {
                        formattedValue = string.Format( "{0} - {1} ({2})", eventbriteEvent.Name.Text, eventbriteEvent.Start.Local, eventbriteEvent.Status );
                    }
                    else
                    {
                        var eventbriteStatus = new StringBuilder();
                        eventbriteStatus.Append( "<dl>" );
                        eventbriteStatus.AppendFormat( "<dd>{0} - {1} ({2})</dd>", eventbriteEvent.Name.Html, eventbriteEvent.Start.Local, eventbriteEvent.Status );
                        eventbriteStatus.AppendFormat( "<dt>Link</dt><dd><a href={0}>{0}</a></dd>", eventbriteEvent.Url );
                        eventbriteStatus.AppendFormat( "<dt>Capacity</dt><dd>{0}</dd>", eventbriteEvent.Capacity.ToString() );
                        eventbriteStatus.AppendFormat( "<dt>Tickets</dt><dd>{0} Available + {1} Sold = {2} Total</dd>", available, sold, total );
                        eventbriteStatus.AppendFormat( "<dt>Synced</dt><dd>{0}</dd>", ( splitVal.Length > 1 ) ? splitVal[1] : "Never" );
                        eventbriteStatus.Append( "</dl>" );

                        formattedValue = eventbriteStatus.ToString();
                    }
                }
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
            var parentControl = new Panel();
            var editControl = new RockDropDownList { ID = id };
            editControl.Items.Add( new ListItem() );
            var eb = new EBApi( Settings.GetAccessToken(), Settings.GetOrganizationId().ToLong( 0 ) );

            var organizationEvents = eb.GetOrganizationEvents( "all", 500 );
            if ( organizationEvents.Pagination.Has_More_Items )
            {
                var looper = new OrganizationEventsResponse();
                for ( int i = 2; i <= organizationEvents.Pagination.PageCount; i++ )
                {
                    looper = eb.GetOrganizationEvents( i, "all", 500 );
                    organizationEvents.Events.AddRange( looper.Events );
                }
            }

            if ( organizationEvents != null && organizationEvents.Events != null )
            {
                foreach ( var template in organizationEvents.Events )
                {
                    editControl.Items.Add( new ListItem( string.Format( "{0} - {1} ({2})", template.Name.Text.ToString(), template.Start.Local, template.Status ), template.Id.ToString() ) );
                }
                editControl.Attributes["data-syncdate"] = "";
                parentControl.Controls.Add( editControl );

                return parentControl;
            }

            return null;
        }

        /// <summary>
        /// Reads new values entered by the user for the field
        /// </summary>
        /// <param name="control">Parent control that controls were added to in the CreateEditControl() method</param>
        /// <param name="configurationValues"></param>
        /// <returns></returns>
        public override string GetEditValue( Control control, Dictionary<string, ConfigurationValue> configurationValues )
        {
            var editControl = control as RockDropDownList;
            if ( editControl != null )
            {
                var syncDate = editControl.Attributes["data-syncdate"];
                if ( editControl.SelectedValue.IsNotNullOrWhiteSpace() )
                {
                    return string.Format( "{0}^{1}", editControl.SelectedValue, syncDate );
                }
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
            var parentControl = control as Panel;
            var editControl = parentControl.Controls.OfType<RockDropDownList>().FirstOrDefault();
            if ( editControl != null )
            {
                if ( value.IsNotNullOrWhiteSpace() )
                {
                    var valueSplit = value.SplitDelimitedValues( "^" );
                    editControl.SetValue( valueSplit[0] );
                    if ( valueSplit.Length > 1 )
                    {
                        editControl.Attributes["data-syncdate"] = valueSplit[1];
                    }
                    parentControl.Controls.Add( new LiteralControl( "<span class='text-danger'>Warning, if you edit the event linked to this group, any previously synced members will remain in the group with old order id's.</span>" ) );
                }
            }
        }
    }

    #endregion

}