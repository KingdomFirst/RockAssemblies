// <copyright>
// Copyright 2026 by Kingdom First Solutions
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
import { openPageAsPerson } from "../../../shared/rockBrowser";
import { createConnectionRequest, getConnectionRequests } from "../../../shared/testData/connections";
import { getLastCapturedEntity } from "../../../shared/testData/workflows";
import { asEnumValue } from "../../../shared/rockApi";
import { expect, test } from "./personAttributeForms.fixture";

/*
    Covers the manual plan's "Testing - Connections" steps: creating a connection request
    from OpportunityId and launching the Connection Request workflow with either the new
    request or an existing one passed as ConnectionRequestId.
*/
test.describe( "Person Attribute Forms Advanced: connections", () => {
    test( "Submit_OpportunityId_CreatesConnectionRequestInDefaultStatus", async ( { page, pafa, block } ) => {
        await pafa.configure( { AllowConnectionOpportunity: "True" } );

        await openPageAsPerson( page, pafa.api, { pageId: pafa.blockPage.pageId, personId: pafa.person.id, parameters: { OpportunityId: pafa.connection.opportunityId } } );
        await block.finish();

        const requests = await getConnectionRequests( pafa.api, pafa.connection.opportunityId, pafa.person.primaryAliasId );
        expect( requests ).toHaveLength( 1 );
        expect( requests[ 0 ].ConnectionStatusId ).toBe( pafa.connection.defaultStatusId );
        expect( asEnumValue( requests[ 0 ].ConnectionState, [ "Active", "Inactive", "FutureFollowUp", "Connected" ] ), "ConnectionState should be Active" ).toBe( 0 );
    } );

    test( "Submit_OpportunityIdWithConnectionRequestWorkflowEntity_LaunchesWorkflowWithNewRequest", async ( { page, pafa, block } ) => {
        await pafa.configure( {
            AllowConnectionOpportunity: "True",
            Workflow: pafa.workflows.connectionRequest.guid,
            WorkflowEntity: "ConnectionRequest"
        } );

        await openPageAsPerson( page, pafa.api, { pageId: pafa.blockPage.pageId, personId: pafa.person.id, parameters: { OpportunityId: pafa.connection.opportunityId } } );
        await block.finish();

        const requests = await getConnectionRequests( pafa.api, pafa.connection.opportunityId, pafa.person.primaryAliasId );
        expect( requests ).toHaveLength( 1 );

        const captured = await getLastCapturedEntity( pafa.api, pafa.workflows.connectionRequest );
        expect( captured, "Workflow was not launched" ).not.toBeNull();
        expect( captured?.entityTypeGuid ).toBe( pafa.entityTypeGuids.connectionRequest );
        expect( captured?.entityId ).toBe( requests[ 0 ].Id );
    } );

    test( "Submit_ConnectionRequestId_LaunchesWorkflowWithExistingRequest", async ( { page, pafa, block } ) => {
        const existingRequestId = await createConnectionRequest( pafa.api, pafa.connection, pafa.person.primaryAliasId );
        await pafa.configure( {
            Workflow: pafa.workflows.connectionRequest.guid,
            WorkflowEntity: "ConnectionRequest"
        } );

        await openPageAsPerson( page, pafa.api, { pageId: pafa.blockPage.pageId, personId: pafa.person.id, parameters: { ConnectionRequestId: existingRequestId } } );
        await block.finish();

        const captured = await getLastCapturedEntity( pafa.api, pafa.workflows.connectionRequest );
        expect( captured, "Workflow was not launched" ).not.toBeNull();
        expect( captured?.entityTypeGuid ).toBe( pafa.entityTypeGuids.connectionRequest );
        expect( captured?.entityId ).toBe( existingRequestId );

        // No new request should have been created.
        expect( await getConnectionRequests( pafa.api, pafa.connection.opportunityId, pafa.person.primaryAliasId ) ).toHaveLength( 1 );
    } );

    test( "Submit_OpportunityIdWhenConnectionsDisallowed_DoesNotCreateConnectionRequest", async ( { page, pafa, block } ) => {
        await pafa.configure( { AllowConnectionOpportunity: "False" } );

        await openPageAsPerson( page, pafa.api, { pageId: pafa.blockPage.pageId, personId: pafa.person.id, parameters: { OpportunityId: pafa.connection.opportunityId } } );
        await block.finish();

        expect( await getConnectionRequests( pafa.api, pafa.connection.opportunityId, pafa.person.primaryAliasId ) ).toHaveLength( 0 );
    } );
} );
