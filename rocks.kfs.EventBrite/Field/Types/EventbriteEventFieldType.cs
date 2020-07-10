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
using System.Collections.Generic;
using System.Linq;
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
    class EventbriteEventFieldType : FieldType
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

            long? longVal = value.AsLongOrNull();
            if ( longVal.HasValue )
            {
                using ( var rockContext = new RockContext() )
                {
                    var eventbriteEvent = new EBApi( Settings.GetAccessToken() ).GetEventById( longVal.Value );
                    if ( eventbriteEvent != null )
                    {
                        if ( condensed )
                        {
                            formattedValue = string.Format( "{0} - {1} ({2})", eventbriteEvent.Name.Text, eventbriteEvent.Start.Local, eventbriteEvent.Status );
                        }
                        else
                        {
                            formattedValue = string.Format( "{0} - {1} ({2})", eventbriteEvent.Name.Html, eventbriteEvent.Start.Local, eventbriteEvent.Status );
                        }
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
            var editControl = new RockDropDownList { ID = id };
            editControl.Items.Add( new ListItem() );

            var ownedEvents = new EBApi( Settings.GetAccessToken() ).GetOrganizationEvents();

            if ( ownedEvents != null && ownedEvents.Events != null )
            {
                foreach ( var template in ownedEvents.Events )
                {
                    editControl.Items.Add( new ListItem( string.Format( "{0} - {1} ({2})", template.Name.Text.ToString(), template.Start.Local, template.Status ), template.Id.ToString() ) );
                }

                return editControl;
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
            var editControl = control as ListControl;
            if ( editControl != null )
            {
                return editControl.SelectedValue;
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
            var editControl = control as ListControl;
            if ( editControl != null )
            {
                editControl.SetValue( value );
            }
        }

        #endregion

    }
}