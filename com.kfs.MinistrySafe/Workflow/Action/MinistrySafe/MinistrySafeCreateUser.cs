using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using Newtonsoft.Json.Linq;
using Rock;
using Rock.Attribute;
using Rock.Data;
using Rock.Model;
using Rock.Security;
using Rock.Web.Cache;
using Rock.Workflow;

namespace com.kfs.MinistrySafe.Workflow.Action.MinistrySafe
{
    /// <summary>
    /// Creates a user at Ministry Safe for the specified Person and stores the Ministry Safe User Id in a Workflow Attribute.
    /// </summary>
    [ActionCategory( "Ministry Safe" )]
    [Description( "Creates a user at Ministry Safe for the selected person." )]
    [Export( typeof( ActionComponent ) )]
    [ExportMetadata( "ComponentName", "Ministry Safe Create User" )]
    [WorkflowAttribute( "Person", "Workflow attribute that contains the person to update.", true, "", "", 0, null,
        new string[] { "Rock.Field.Types.PersonFieldType" } )]
    [WorkflowAttribute( "Ministry Safe Id", "Workflow attribute to store the Ministry Safe User Id.", true, "", "", 1 )]
    [WorkflowAttribute( "Direct Login Url", "Workflow attribute to store the Ministry Safe Direct Login Url.", true, "", "", 2 )]
    [WorkflowTextOrAttribute( "Training Type", "Attribute Value", "The training type that should be assigned to the User.  If left blank or none selected, the Standard Survey will be assigned.", false, "", "", 3, "TType" )]
    [WorkflowTextOrAttribute( "User Type", "Attribute Value", "The user type the Person will be assigned at Ministry Safe. Current options are 'volunteer' or 'employee'. Default is 'volunteer'", false, order: 4 )]
    [EncryptedTextField( "API Key", "Optional API Key to override Global Attribute.", false, "", "Advanced", 0 )]
    [BooleanField( "Staging Mode", "Flag indicating if Ministry Safe Staging Mode should be used.", false, "Advanced", 1 )]
    [BooleanField( "Use Workflow Id", "Flag indicating if the Workflow Id should be used as the Ministry Safe External Id.", true, "Advanced", 2 )]
    public class MinistrySafeCreateUser : ActionComponent
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

            var attributeMinistrySafeId = AttributeCache.Get( GetAttributeValue( action, "MinistrySafeId" ).AsGuid(), rockContext );
            var attributeDirectLoginUrl = AttributeCache.Get( GetAttributeValue( action, "DirectLoginUrl" ).AsGuid(), rockContext );

            if ( attributeMinistrySafeId != null && attributeDirectLoginUrl != null )
            {
                // Get Person for processing
                var guid = GetAttributeValue( action, "Person" ).AsGuid();

                if ( !guid.IsEmpty() )
                {
                    var attributePerson = AttributeCache.Get( guid, rockContext );
                    if ( attributePerson != null )
                    {
                        string attributePersonValue = action.GetWorklowAttributeValue( guid );
                        if ( !string.IsNullOrWhiteSpace( attributePersonValue ) )
                        {
                            if ( attributePerson.FieldType.Class == "Rock.Field.Types.PersonFieldType" )
                            {
                                Guid personAliasGuid = attributePersonValue.AsGuid();
                                if ( !personAliasGuid.IsEmpty() )
                                {
                                    var person = new PersonAliasService( rockContext ).Queryable()
                                        .Where( a => a.Guid.Equals( personAliasGuid ) )
                                        .Select( a => a.Person )
                                        .FirstOrDefault();
                                    if ( person != null )
                                    {
                                        // Process Workflow Action settings
                                        var apiKeySetting = GetAttributeValue( action, "APIKey" );
                                        var apiKey = string.IsNullOrWhiteSpace( apiKeySetting ) ? GlobalAttributesCache.Value( "MinistrySafeAPIKey" ) : apiKeySetting;
                                        var externalId = GetAttributeValue( action, "UseWorkflowId" ).AsBoolean( true ) ? action.Activity.Workflow.Id.ToString() : string.Empty;
                                        var userTypeSetting = GetAttributeValue( action, "UserType" );
                                        var userType = string.IsNullOrWhiteSpace( userTypeSetting ) ? "volunteer" : userTypeSetting;
                                        var stagingMode = GetAttributeValue( action, "StagingMode" ).AsBoolean( false );

                                        // Create Ministry Safe User
                                        JObject user = Users.CreateUser( Encryption.DecryptString( apiKey ), person, userTypeSetting, externalId, stagingMode );
                                        var userId = user?.Value<string>( "id" );
                                        if ( user != null && !string.IsNullOrWhiteSpace( userId ) )
                                        {
                                            // Save Ministry Safe User Id
                                            SetWorkflowAttributeValue( action, attributeMinistrySafeId.Guid, userId );
                                            action.AddLogEntry( string.Format( "Set '{0}' attribute to '{1}'.", attributeMinistrySafeId.Name, userId ) );

                                            // Save Ministry Safe User Url
                                            var directLoginUrl = user.Value<string>( "direct_login_url" );
                                            SetWorkflowAttributeValue( action, attributeDirectLoginUrl.Guid, directLoginUrl );
                                            action.AddLogEntry( string.Format( "Set '{0}' attribute to '{1}'.", attributeDirectLoginUrl.Name, directLoginUrl ) );

                                            // Assign training type to user if provided
                                            var attributeTrainingType = GetAttributeValue( action, "TType", true );
                                            var surveyCode = DefinedValueCache.Get( attributeTrainingType );
                                            if ( surveyCode != null )
                                            {
                                                JObject training = Trainings.AssignTraining( Encryption.DecryptString( apiKey ), userId, surveyCode.Value, stagingMode );
                                                if ( training != null )
                                                {
                                                    var message = training.Value<string>( "message" );
                                                    action.AddLogEntry( string.Format( "Problem assigning training '{0}'. {1}", surveyCode.Description, message ) );
                                                }
                                                else
                                                {
                                                    action.AddLogEntry( string.Format( "Assigned '{0}' to '{1}'.", surveyCode.Description, person.FullName ) );
                                                }
                                            }

                                            return true;
                                        }
                                        else
                                        {
                                            errorMessages.Add( string.Format( "There was a problem creating the Ministry Safe User. {0}", user ) );
                                            return false;
                                        }
                                    }
                                    else
                                    {
                                        errorMessages.Add( string.Format( "Person could not be found for selected value ('{0}')!", guid.ToString() ) );
                                        return false;
                                    }
                                }
                            }
                            else
                            {
                                errorMessages.Add( "The attribute used to provide the person was not of type 'Person'." );
                                return false;
                            }
                        }
                    }
                    else
                    {
                        errorMessages.Add( string.Format( "Selected workflow attribute for Person was not found." ) );
                        return false;
                    }
                }
                else
                {
                    errorMessages.Add( string.Format( "Selected workflow attribute for Person is not a valid Guid." ) );
                    return false;
                }
            }
            else
            {
                errorMessages.Add( string.Format( "Ministry Safe attributes not found in Workflow." ) );
                return false;
            }

            return true;
        }
    }
}
