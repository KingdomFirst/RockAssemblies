using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;

using Rock;
using Rock.Attribute;
using Rock.Data;
using Rock.Model;
using Rock.Security;
using Rock.Web.Cache;
using Rock.Workflow;

using Newtonsoft.Json.Linq;

namespace com.kfs.MinistrySafe.Workflow.Action.MinistrySafe
{
    /// <summary>
    /// Retrieves a user at Ministry Safe for the provided Ministry Safe User Id.
    /// </summary>
    [ActionCategory( "Ministry Safe" )]
    [Description( "Gets the training test results from Ministry Safe." )]
    [Export( typeof( ActionComponent ) )]
    [ExportMetadata( "ComponentName", "Ministry Safe Get User Results" )]

    [WorkflowAttribute( "Ministry Safe Id", "The User Id in the Ministry Safe system.", true, "", "", 0 )]
    [WorkflowAttribute( "Date Completed", "Workflow attribute to store the Date Completed.", true, "", "", 1 )]
    [WorkflowAttribute( "Score", "Workflow attribute to store the Score.", true, "", "", 2 )]
    
    [EncryptedTextField( "API Key", "Optional API Key to override Global Attribute.", false, "", "Advanced", 0 )]
    [BooleanField( "Staging Mode", "Flag indicating if Ministry Safe Staging Mode should be used.", false, "Advanced", 1 )]

    class MinistrySafeGetUserResults : ActionComponent
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

            var attributeMinistrySafeId = AttributeCache.Read( GetAttributeValue( action, "MinistrySafeId" ).AsGuid(), rockContext );
            var attributeDateCompleted = AttributeCache.Read( GetAttributeValue( action, "DateCompleted" ).AsGuid(), rockContext );
            var attributeScore = AttributeCache.Read( GetAttributeValue( action, "Score" ).AsGuid(), rockContext );

            if ( attributeMinistrySafeId != null && attributeDateCompleted != null && attributeScore != null )
            {
                var userId = action.GetWorklowAttributeValue( attributeMinistrySafeId.Guid );
                if ( !string.IsNullOrWhiteSpace( userId ) )
                {
                    // Process Workflow Action settings
                    var apiKeySetting = GetAttributeValue( action, "APIKey" );
                    var apiKey = string.IsNullOrWhiteSpace( apiKeySetting ) ? GlobalAttributesCache.Read( rockContext ).GetValue( "MinistrySafeAPIKey" ) : apiKeySetting;
                    var stagingMode = GetAttributeValue( action, "StagingMode" ).AsBoolean( false );

                    // Get User
                    JObject user = Users.GetUser( Encryption.DecryptString( apiKey ), userId, stagingMode );
                    var dateCompleted = user.Value<string>( "complete_date" );
                    var score = user.Value<string>( "score" );

                    if ( !string.IsNullOrWhiteSpace( dateCompleted ) && !string.IsNullOrWhiteSpace( score ) )
                    {
                        // Save Date Completed
                        SetWorkflowAttributeValue( action, attributeDateCompleted.Guid, dateCompleted );
                        action.AddLogEntry( string.Format( "Set '{0}' attribute to '{1}'.", attributeDateCompleted.Name, dateCompleted ) );

                        // Save Score
                        SetWorkflowAttributeValue( action, attributeScore.Guid, score );
                        action.AddLogEntry( string.Format( "Set '{0}' attribute to '{1}'.", attributeScore.Name, score ) );

                        return true;
                    }
                }
                else
                {
                    errorMessages.Add( string.Format( "Ministry Safe User Id cannot be blank." ) );
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
