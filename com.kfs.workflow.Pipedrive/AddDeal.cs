using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Net;

using Newtonsoft.Json.Linq;
using Rock;
using Rock.Attribute;
using Rock.Data;
using Rock.Model;
using Rock.Workflow;

namespace com.kfs.Workflow.Action.Pipedrive
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
    [TextField( "Topic Field Token", "Custom field token to populate with Topic", true, "", "Pipedrive Settings", 2 )]
    [TextField( "Plan Field Token", "Custom field token to populate with Plan", true, "", "Pipedrive Settings", 3 )]
    [TextField( "Platform Field Token", "Custom field token to populate with Platform", true, "", "Pipedrive Settings", 4 )]
    [TextField( "Source Field Token", "Custom field token to populate with Source", true, "", "Pipedrive Settings", 5 )]
    [WorkflowAttribute( "Organization Name", "The attribute to use to populate the Organization Name.", true, "", "Field Map", 0 )]
    [WorkflowAttribute( "Person First Name", "The attribute to use to populate the Person First Name.", true, "", "Field Map", 1 )]
    [WorkflowAttribute( "Person Last Name", "The attribute to use to populate the Person Last Name.", true, "", "Field Map", 2 )]
    [WorkflowAttribute( "Person Email", "The attribute to use to populate the Person Email.", true, "", "Field Map", 3 )]
    [WorkflowAttribute( "Person Phone", "The attribute to use to populate the Person Phone.", true, "", "Field Map", 4 )]
    [WorkflowAttribute( "Inquiry", "The attribute to use to populate the initial inquiry.", true, "", "Field Map", 5 )]
    [WorkflowAttribute( "Topic", "The attribute to use to populate the topic.", true, "", "Field Map", 6 )]
    [WorkflowAttribute( "Plan", "The attribute to use to populate the plan.", true, "", "Field Map", 7 )]
    [WorkflowAttribute( "Product", "The attribute to use to populate the product.", true, "", "Field Map", 8 )]
    [WorkflowAttribute( "Platform", "The attribute to use to populate the platform.", true, "", "Field Map", 9 )]
    [WorkflowAttribute( "Source", "The attribute to use to populate the source.", true, "", "Field Map", 10 )]

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
            string topic = action.GetWorklowAttributeValue( GetAttributeValue( action, "Topic" ).AsGuidOrNull().Value );
            string topicFieldToken = GetAttributeValue( action, "TopicFieldToken" );
            string plan = action.GetWorklowAttributeValue( GetAttributeValue( action, "Plan" ).AsGuidOrNull().Value );
            string planFieldToken = GetAttributeValue( action, "PlanFieldToken" );
            string product = action.GetWorklowAttributeValue( GetAttributeValue( action, "Product" ).AsGuidOrNull().Value );
            string platformFieldToken = GetAttributeValue( action, "PlatformFieldToken" );
            string platform = action.GetWorklowAttributeValue( GetAttributeValue( action, "Platform" ).AsGuidOrNull().Value );
            string sourceFieldToken = GetAttributeValue( action, "SourceFieldToken" );
            string source = action.GetWorklowAttributeValue( GetAttributeValue( action, "Source" ).AsGuidOrNull().Value );
            string _orgId = "";
            string _personId = "";
            string _dealId = "";

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
                    _orgId = (string)json["data"]["id"];
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
                    _personId = (string)json["data"]["id"];
                }

                // add deal
                using ( WebClient dealCall = new WebClient() )
                {
                    // Topic, Plan, Platform and Inquiry are linked
                    if ( !String.IsNullOrWhiteSpace( inquiryFieldToken ) && !String.IsNullOrWhiteSpace( topicFieldToken ) && !String.IsNullOrWhiteSpace( planFieldToken ) && !String.IsNullOrWhiteSpace( platformFieldToken ) && !String.IsNullOrWhiteSpace( sourceFieldToken ) )
                    {
                        byte[] response =
                        dealCall.UploadValues( "https://api.pipedrive.com/v1/deals?api_token=" + apiToken, new NameValueCollection()
                            {
                                { "title", String.Format("{0} Deal",organization) },
                                { "person_id", _personId },
                                { "org_id", _orgId },
                                { inquiryFieldToken, inquiry },
                                { topicFieldToken, topic },
                                { planFieldToken, plan },
                                { platformFieldToken, platform },
                                { sourceFieldToken, source }
                            } );

                        JObject json = JObject.Parse( System.Text.Encoding.UTF8.GetString( response ) );
                        _dealId = (string)json["data"]["id"];
                    }
                    // Only Inquiry is linked
                    else if ( !String.IsNullOrWhiteSpace( inquiryFieldToken ) && String.IsNullOrWhiteSpace( topicFieldToken ) && String.IsNullOrWhiteSpace( planFieldToken ) )
                    {
                        byte[] response =
                        dealCall.UploadValues( "https://api.pipedrive.com/v1/deals?api_token=" + apiToken, new NameValueCollection()
                            {
                                { "title", String.Format("{0} Deal",organization) },
                                { "person_id", _personId },
                                { "org_id", _orgId },
                                { inquiryFieldToken, inquiry }
                            } );

                        JObject json = JObject.Parse( System.Text.Encoding.UTF8.GetString( response ) );
                        _dealId = (string)json["data"]["id"];
                    }
                    // Only Topic is linked
                    else if ( String.IsNullOrWhiteSpace( inquiryFieldToken ) && !String.IsNullOrWhiteSpace( topicFieldToken ) && String.IsNullOrWhiteSpace( planFieldToken ) )
                    {
                        byte[] response =
                        dealCall.UploadValues( "https://api.pipedrive.com/v1/deals?api_token=" + apiToken, new NameValueCollection()
                            {
                                { "title", String.Format("{0} Deal",organization) },
                                { "person_id", _personId },
                                { "org_id", _orgId },
                                { topicFieldToken, topic }
                            } );

                        JObject json = JObject.Parse( System.Text.Encoding.UTF8.GetString( response ) );
                        _dealId = (string)json["data"]["id"];
                    }
                    // Only Plan is linked
                    else if ( String.IsNullOrWhiteSpace( inquiryFieldToken ) && String.IsNullOrWhiteSpace( topicFieldToken ) && !String.IsNullOrWhiteSpace( planFieldToken ) )
                    {
                        byte[] response =
                        dealCall.UploadValues( "https://api.pipedrive.com/v1/deals?api_token=" + apiToken, new NameValueCollection()
                            {
                                { "title", String.Format("{0} Deal",organization) },
                                { "person_id", _personId },
                                { "org_id", _orgId },
                                { planFieldToken, plan }
                            } );

                        JObject json = JObject.Parse( System.Text.Encoding.UTF8.GetString( response ) );
                        _dealId = (string)json["data"]["id"];
                    }
                    // Only Inquiry Not Linked
                    else if ( String.IsNullOrWhiteSpace( inquiryFieldToken ) && !String.IsNullOrWhiteSpace( topicFieldToken ) && !String.IsNullOrWhiteSpace( planFieldToken ) )
                    {
                        byte[] response =
                        dealCall.UploadValues( "https://api.pipedrive.com/v1/deals?api_token=" + apiToken, new NameValueCollection()
                            {
                                { "title", String.Format("{0} Deal",organization) },
                                { "person_id", _personId },
                                { "org_id", _orgId },
                                { topicFieldToken, topic },
                                { planFieldToken, plan }
                            } );

                        JObject json = JObject.Parse( System.Text.Encoding.UTF8.GetString( response ) );
                        _dealId = (string)json["data"]["id"];
                    }
                    // Only Topic Not Linked
                    else if ( !String.IsNullOrWhiteSpace( inquiryFieldToken ) && String.IsNullOrWhiteSpace( topicFieldToken ) && !String.IsNullOrWhiteSpace( planFieldToken ) )
                    {
                        byte[] response =
                        dealCall.UploadValues( "https://api.pipedrive.com/v1/deals?api_token=" + apiToken, new NameValueCollection()
                            {
                                { "title", String.Format("{0} Deal",organization) },
                                { "person_id", _personId },
                                { "org_id", _orgId },
                                { inquiryFieldToken, inquiry },
                                { planFieldToken, plan }
                            } );

                        JObject json = JObject.Parse( System.Text.Encoding.UTF8.GetString( response ) );
                        _dealId = (string)json["data"]["id"];
                    }
                    // Only Plan Not Linked
                    else if ( !String.IsNullOrWhiteSpace( inquiryFieldToken ) && !String.IsNullOrWhiteSpace( topicFieldToken ) && String.IsNullOrWhiteSpace( planFieldToken ) )
                    {
                        byte[] response =
                        dealCall.UploadValues( "https://api.pipedrive.com/v1/deals?api_token=" + apiToken, new NameValueCollection()
                            {
                                { "title", String.Format("{0} Deal",organization) },
                                { "person_id", _personId },
                                { "org_id", _orgId },
                                { inquiryFieldToken, inquiry },
                                { topicFieldToken, topic }
                            } );

                        JObject json = JObject.Parse( System.Text.Encoding.UTF8.GetString( response ) );
                        _dealId = (string)json["data"]["id"];
                    }
                    // Nothing is linked, why even show up for Monday?
                    else
                    {
                        byte[] response =
                        dealCall.UploadValues( "https://api.pipedrive.com/v1/deals?api_token=" + apiToken, new NameValueCollection()
                            {
                                { "title", String.Format("{0} Deal",organization) },
                                { "person_id", _personId },
                                { "org_id", _orgId }
                            } );

                        JObject json = JObject.Parse( System.Text.Encoding.UTF8.GetString( response ) );
                        _dealId = (string)json["data"]["id"];
                    }
                }

                if ( !String.IsNullOrWhiteSpace( product ) )
                {
                    // add products
                    using ( WebClient productCall = new WebClient() )
                    {

                        byte[] response =
                        productCall.UploadValues( "https://api.pipedrive.com/v1/deals/" + _dealId + "/products?api_token=" + apiToken, new NameValueCollection()
                           {
                               { "id", _dealId },
                               { "product_id", product},
                               { "item_price", "0" },
                               { "quantity" , "1"},
                           } );
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
