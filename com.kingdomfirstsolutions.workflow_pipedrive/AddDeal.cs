// <copyright>
// Copyright by the Spark Development Network
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
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Net;

using Newtonsoft.Json.Linq;
using Rock.Attribute;
using Rock.Data;
using Rock.Model;

namespace Rock.Workflow.Action.Pipedrive
{
    /// <summary>
    /// Adds a Deal to Pipedrive using workflow form
    /// </summary>
    //[ActionCategory( "Check-In" )]
    [Description( "Adds a Deal to Pipedrive" )]
    [Export( typeof( ActionComponent ) )]
    [ExportMetadata( "ComponentName", "Add Deal to Pipedrive" )]
    [TextField( "API Token", "Authorized API Token", true, "", "Pipedrive Settings", 0 )]
    [TextField( "Inquiry Field Token", "Custom field token to populate with Inquiry", true, "", "Pipedrive Settings", 1 )]
    [WorkflowAttribute( "Organization Name", "The attribute to use to populate the Organization Name.", true, "", "Field Map", 0 )]
    [WorkflowAttribute( "Person First Name", "The attribute to use to populate the Person First Name.", true, "", "Field Map", 1 )]
    [WorkflowAttribute( "Person Last Name", "The attribute to use to populate the Person Last Name.", true, "", "Field Map", 2 )]
    [WorkflowAttribute( "Person Email", "The attribute to use to populate the Person Email.", true, "", "Field Map", 3 )]
    [WorkflowAttribute( "Person Phone", "The attribute to use to populate the Person Phone.", true, "", "Field Map", 4 )]
    [WorkflowAttribute( "Inquiry", "The attribute to use to populate the initial inquiry.", true, "", "Field Map", 5 )]

    public class AddDeal : ActionComponent
    {
        /// <summary>
        /// Executes the specified workflow.
        /// </summary>
        /// <param name="rockContext">The rock context.</param>
        /// <param name="action">The action.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="errorMessages">The error messages.</param>
        /// <returns></returns>
        public override bool Execute( RockContext rockContext, WorkflowAction action, Object entity, out List<string> errorMessages )
        {
            errorMessages = new List<string>();

            string apiToken = GetAttributeValue( action, "APIToken" );
            string organization = action.GetWorklowAttributeValue( GetAttributeValue( action, "OrganizationName" ).AsGuidOrNull().Value );
            string personName = String.Format( "{0} {1}", action.GetWorklowAttributeValue( GetAttributeValue( action, "PersonFirstName" ).AsGuidOrNull().Value ), action.GetWorklowAttributeValue( GetAttributeValue( action, "PersonLastName" ).AsGuidOrNull().Value ) );
            string personEmail = action.GetWorklowAttributeValue( GetAttributeValue( action, "PersonEmail" ).AsGuidOrNull().Value );
            string personPhone = action.GetWorklowAttributeValue( GetAttributeValue( action, "PersonPhone" ).AsGuidOrNull().Value );
            string inquiry = action.GetWorklowAttributeValue( GetAttributeValue( action, "Inquiry" ).AsGuidOrNull().Value );
            string inquiryFieldToken = GetAttributeValue( action, "InquiryFieldToken" );
            string _orgId = "";
            string _personId = "";

            if ( !string.IsNullOrWhiteSpace( apiToken ) )
            {
                // add organization
                using ( WebClient organizationCall = new WebClient() )
                {

                    byte[] response =
                    organizationCall.UploadValues( "https://api.pipedrive.com/v1/organizations?api_token=" + apiToken, new NameValueCollection()
                           {
                               { "name", organization }
                           } );

                    JObject json = JObject.Parse( System.Text.Encoding.UTF8.GetString( response ) );
                    _orgId = ( string )json["data"]["id"];
                }

                // add person
                using ( WebClient personCall = new WebClient() )
                {

                    byte[] response =
                    personCall.UploadValues( "https://api.pipedrive.com/v1/persons?api_token=" + apiToken, new NameValueCollection()
                           {
                               { "name", personName },
                               { "email", personEmail },
                               { "phone", personPhone },
                               { "org_id", _orgId }
                           } );

                    JObject json = JObject.Parse( System.Text.Encoding.UTF8.GetString( response ) );
                    _personId = ( string )json["data"]["id"];
                }

                // add deal
                using ( WebClient dealCall = new WebClient() )
                {
                    if ( String.IsNullOrWhiteSpace( inquiryFieldToken ) )
                    {
                        byte[] response =
                        dealCall.UploadValues( "https://api.pipedrive.com/v1/deals?api_token=" + apiToken, new NameValueCollection()
                            {
                                { "title", String.Format("{0} Deal",organization) },
                                { "person_id", _personId },
                                { "org_id", _orgId }
                            } );

                        string result = System.Text.Encoding.UTF8.GetString( response );
                    }
                    else
                    {
                        byte[] response =
                        dealCall.UploadValues( "https://api.pipedrive.com/v1/deals?api_token=" + apiToken, new NameValueCollection()
                            {
                                { "title", String.Format("{0} Deal",organization) },
                                { "person_id", _personId },
                                { "org_id", _orgId },
                                { inquiryFieldToken, inquiry }
                            } );

                        string result = System.Text.Encoding.UTF8.GetString( response );
                    }
                }

                return true;
            }
            else
            {
                errorMessages.Add( "Invalid API Token!" );
            }

            return false;
        }

        /// <summary>
        /// Gets or sets a dictionary containing the Organization values
        /// </summary>
        /// <value>
        ///  A <see cref="System.Collections.Generic.Dictionary&lt;String,String&gt;"/> of <see cref="System.String"/> objects containing organization values
        /// </value>
        public virtual Dictionary<string, object> OrganizationReturnValues
        {
            get { return _organizationReturnValues; }
            set { _organizationReturnValues = value; }
        }
        private Dictionary<string, object> _organizationReturnValues = new Dictionary<string, object>();

        /// <summary>
        /// Gets or sets a dictionary containing the Organization values
        /// </summary>
        /// <value>
        ///  A <see cref="System.Collections.Generic.Dictionary&lt;String,String&gt;"/> of <see cref="System.String"/> objects containing organization values
        /// </value>
        public virtual Dictionary<string, object> PersonReturnValues
        {
            get { return _personReturnValues; }
            set { _personReturnValues = value; }
        }
        private Dictionary<string, object> _personReturnValues = new Dictionary<string, object>();
    }
}
