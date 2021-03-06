﻿// <copyright>
// Copyright by the Spark Development Network
//
// Licensed under the Rock Community License (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.rockrms.com/license
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//
// <notice>
// This file contains modifications by Kingdom First Solutions
// and is a derivative work.
//
// Modification (including but not limited to):
// * Added the ability to create multiple connection requests with a single action.
// </notice>
//
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;

using Rock;
using Rock.Attribute;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;
using Rock.Workflow;

namespace rocks.kfs.Workflow.Action
{
    #region Action Attributes

    [ActionCategory( "KFS: Connections" )]
    [Description( "Creates multiple connection requests." )]
    [Export( typeof( ActionComponent ) )]
    [ExportMetadata( "ComponentName", "Connection Requests Create" )]

    #endregion

    #region Action Settings

    [WorkflowAttribute( "Person Attribute", "The Person attribute that contains the person that connection request should be created for.", true, "", "", 0, null,
        new string[] { "Rock.Field.Types.PersonFieldType" } )]
    [WorkflowAttribute( "Connection Opportunities Attribute", "The attribute that contains the types of connection opportunity to create. Expecting guid1^name1,guid2^name2,guid3^name3,.... Sample SQL: <code>SELECT co.[Guid] AS[Value], ct.[Name] + ': ' + co.[Name] AS[Text] FROM ConnectionOpportunity co JOIN ConnectionType ct ON co.ConnectionTypeId = ct.Id ORDER BY ct.[Name], co.[Name]</code>", true, "", "", 1, null )]
    [WorkflowAttribute( "Connection Status Attribute", "The attribute that contains the connection status to use for the new request.", false, "", "", 2, null,
        new string[] { "Rock.Field.Types.ConnectionStatusFieldType" } )]
    [ConnectionStatusField( "Connection Status", "The connection status to use for the new request (when Connection Status Attribute is not specified or invalid). If neither this setting or the Connection Status Attribute setting are set, the default status will be used.", false, "", "", 3 )]
    [WorkflowAttribute( "Campus Attribute", "An optional attribute that contains the campus to use for the request.", false, "", "", 4, null,
        new string[] { "Rock.Field.Types.CampusFieldType" } )]
    [WorkflowAttribute( "Connection Comment Attribute", "An optional attribute that contains the comment to use for the request.", false, "", "", 5, null,
        new string[] { "Rock.Field.Types.TextFieldType", "Rock.Field.Types.MemoFieldType" } )]

    #endregion

    /// <summary>
    /// Creates multiple connection requests.
    /// </summary>
    public class CreateConnectionRequests : ActionComponent
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

            // Get the person
            PersonAlias personAlias = null;
            Guid personAliasGuid = action.GetWorkflowAttributeValue( GetAttributeValue( action, "PersonAttribute" ).AsGuid() ).AsGuid();
            personAlias = new PersonAliasService( rockContext ).Get( personAliasGuid );
            if ( personAlias == null )
            {
                errorMessages.Add( "Invalid Person Attribute or Value!" );
                return false;
            }

            var connectionRequestService = new ConnectionRequestService( rockContext );

            // Get the opportunity
            List<Guid> opportunityTypeGuids = Array.ConvertAll( action.GetWorkflowAttributeValue( GetAttributeValue( action, "ConnectionOpportunitiesAttribute" ).AsGuid() ).Split( ',' ), s => new Guid( s ) ).ToList();

            foreach ( var opportunityTypeGuid in opportunityTypeGuids )
            {
                ConnectionOpportunity opportunity = null;
                opportunity = new ConnectionOpportunityService( rockContext ).Get( opportunityTypeGuid );
                if ( opportunity == null )
                {
                    errorMessages.Add( "Invalid Connection Opportunity Attribute or Value!" );
                    return false;
                }

                // Get connection status
                ConnectionStatus status = null;
                Guid? connectionStatusGuid = null;
                Guid? connectionStatusAttributeGuid = GetAttributeValue( action, "ConnectionStatusAttribute" ).AsGuidOrNull();
                if ( connectionStatusAttributeGuid.HasValue )
                {
                    connectionStatusGuid = action.GetWorkflowAttributeValue( connectionStatusAttributeGuid.Value ).AsGuidOrNull();
                    if ( connectionStatusGuid.HasValue )
                    {
                        status = opportunity.ConnectionType.ConnectionStatuses
                            .Where( s => s.Guid.Equals( connectionStatusGuid.Value ) )
                            .FirstOrDefault();
                    }
                }
                if ( status == null )
                {
                    connectionStatusGuid = GetAttributeValue( action, "ConnectionStatus" ).AsGuidOrNull();
                    if ( connectionStatusGuid.HasValue )
                    {
                        status = opportunity.ConnectionType.ConnectionStatuses
                            .Where( s => s.Guid.Equals( connectionStatusGuid.Value ) )
                            .FirstOrDefault();
                    }
                }
                if ( status == null )
                {
                    status = opportunity.ConnectionType.ConnectionStatuses
                        .Where( s => s.IsDefault )
                        .FirstOrDefault();
                }

                // Get Campus
                int? campusId = null;
                Guid? campusAttributeGuid = GetAttributeValue( action, "CampusAttribute" ).AsGuidOrNull();
                if ( campusAttributeGuid.HasValue )
                {
                    Guid? campusGuid = action.GetWorkflowAttributeValue( campusAttributeGuid.Value ).AsGuidOrNull();
                    if ( campusGuid.HasValue )
                    {
                        var campus = CampusCache.Get( campusGuid.Value );
                        if ( campus != null )
                        {
                            campusId = campus.Id;
                        }
                    }
                }

                // Get the Comment
                String comment = action.GetWorkflowAttributeValue( GetAttributeValue( action, "ConnectionCommentAttribute" ).AsGuid() );

                //var connectionRequestService = new ConnectionRequestService( rockContext );

                var connectionRequest = new ConnectionRequest();
                connectionRequest.PersonAliasId = personAlias.Id;
                connectionRequest.ConnectionOpportunityId = opportunity.Id;
                connectionRequest.ConnectionState = ConnectionState.Active;
                connectionRequest.ConnectionStatusId = status.Id;
                connectionRequest.CampusId = campusId;
                connectionRequest.ConnectorPersonAliasId = opportunity.GetDefaultConnectorPersonAliasId( campusId );
                connectionRequest.Comments = comment;

                connectionRequestService.Add( connectionRequest );
            }

            rockContext.SaveChanges();

            return true;
        }
    }
}
