// <copyright>
// Copyright 2019 by Kingdom First Solutions
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

using Newtonsoft.Json.Linq;

using Rock;
using Rock.Attribute;
using Rock.Data;
using Rock.Model;
using Rock.Security;
using Rock.Web.Cache;
using Rock.Workflow;

namespace rocks.kfs.MinistrySafe.Workflow.Action.MinistrySafe
{
    #region Action Attributes

    [ActionCategory( "Ministry Safe" )]
    [Description( "Gets the training test results from Ministry Safe." )]
    [Export( typeof( ActionComponent ) )]
    [ExportMetadata( "ComponentName", "Ministry Safe Get User Results" )]

    #endregion

    #region Action Settings

    [WorkflowAttribute( "Ministry Safe Id", "The User Id in the Ministry Safe system.", true, "", "", 0 )]
    [WorkflowAttribute( "Date Completed", "Workflow attribute to store the Date Completed.", true, "", "", 1 )]
    [WorkflowAttribute( "Score", "Workflow attribute to store the Score.", true, "", "", 2 )]
    [IntegerField( "Pass Score", "The minimum score to consider a pass.", true, 80, "", 3 )]
    [WorkflowAttribute( "Result", "Workflow attribute to store the Result.", true, "", "", 4 )]
    [EncryptedTextField( "API Key", "Optional API Key to override Global Attribute.", false, "", "Advanced", 0 )]
    [BooleanField( "Staging Mode", "Flag indicating if Ministry Safe Staging Mode should be used.", false, "Advanced", 1 )]

    #endregion

    /// <summary>
    /// Retrieves a user at Ministry Safe for the provided Ministry Safe User Id.
    /// </summary>
    public class MinistrySafeGetUserResults : ActionComponent
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
            var attributeDateCompleted = AttributeCache.Get( GetAttributeValue( action, "DateCompleted" ).AsGuid(), rockContext );
            var attributeScore = AttributeCache.Get( GetAttributeValue( action, "Score" ).AsGuid(), rockContext );
            var attributePassScore = GetAttributeValue( action, "PassScore" ).AsIntegerOrNull();
            var attributeResult = AttributeCache.Get( GetAttributeValue( action, "Result" ).AsGuid(), rockContext );

            if ( attributeMinistrySafeId != null && attributeDateCompleted != null && attributeScore != null )
            {
                var userId = action.GetWorklowAttributeValue( attributeMinistrySafeId.Guid );
                if ( !string.IsNullOrWhiteSpace( userId ) )
                {
                    // Process Workflow Action settings
                    var apiKeySetting = GetAttributeValue( action, "APIKey" );
                    var apiKey = string.IsNullOrWhiteSpace( apiKeySetting ) ? GlobalAttributesCache.Value( "MinistrySafeAPIKey" ) : apiKeySetting;
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

                        // Save Result
                        if ( attributePassScore != null && attributeResult != null )
                        {
                            var result = "Fail";
                            if ( score.AsInteger() >= attributePassScore )
                            {
                                result = "Pass";
                            }

                            SetWorkflowAttributeValue( action, attributeResult.Guid, result );
                            action.AddLogEntry( string.Format( "Set '{0}' attribute to '{1}'.", attributeResult.Name, result ) );
                        }

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
