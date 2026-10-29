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
import { randomUUID } from "crypto";
import { RockApi, RockEntity } from "../rockApi";
import { TestNamePrefix } from "../systemGuids";

/** A connection type with one default status and one opportunity. */
export type TestConnectionSetup = {
    connectionTypeId: number;
    defaultStatusId: number;
    opportunityId: number;
};

export async function createConnectionSetup( api: RockApi, name: string ): Promise<TestConnectionSetup> {
    const fullName = `${TestNamePrefix} ${name}`;

    const connectionTypeId = await api.post( "/api/ConnectionTypes", {
        Guid: randomUUID(),
        Name: fullName,
        Description: "Created by rocks.kfs.Tests.EndToEnd. Safe to delete.",
        IsActive: true,
        EnableFutureFollowup: false,
        EnableFullActivityList: false,
        RequiresPlacementGroupToConnect: false,
        EnableRequestSecurity: false
    } );

    const defaultStatusId = await api.post( "/api/ConnectionStatus", {
        Guid: randomUUID(),
        ConnectionTypeId: connectionTypeId,
        Name: "No Contact",
        IsDefault: true,
        IsActive: true,
        IsCritical: false,
        Order: 0
    } );

    const opportunityId = await api.post( "/api/ConnectionOpportunities", {
        Guid: randomUUID(),
        ConnectionTypeId: connectionTypeId,
        Name: fullName,
        PublicName: fullName,
        IsActive: true
    } );

    return { connectionTypeId, defaultStatusId, opportunityId };
}

/** Creates an active connection request, as a staff member would before emailing the form link. */
export async function createConnectionRequest( api: RockApi, setup: TestConnectionSetup, personAliasId: number ): Promise<number> {
    return await api.post( "/api/ConnectionRequests", {
        Guid: randomUUID(),
        ConnectionOpportunityId: setup.opportunityId,
        ConnectionStatusId: setup.defaultStatusId,
        PersonAliasId: personAliasId,
        // ConnectionState.Active
        ConnectionState: 0,
        Comments: "Created by rocks.kfs.Tests.EndToEnd."
    } );
}

export async function getConnectionRequests( api: RockApi, opportunityId: number, personAliasId: number ): Promise<RockEntity[]> {
    return await api.query( "ConnectionRequests", `ConnectionOpportunityId eq ${opportunityId} and PersonAliasId eq ${personAliasId}` );
}

export async function deleteConnectionRequests( api: RockApi, opportunityId: number ): Promise<void> {
    for ( const connectionRequest of await api.query( "ConnectionRequests", `ConnectionOpportunityId eq ${opportunityId}` ) ) {
        for ( const activity of await api.query( "ConnectionRequestActivities", `ConnectionRequestId eq ${connectionRequest.Id}` ) ) {
            await api.delete( "ConnectionRequestActivities", activity.Id );
        }
        await api.delete( "ConnectionRequests", connectionRequest.Id );
    }
}

export async function deleteConnectionType( api: RockApi, connectionTypeId: number ): Promise<void> {
    for ( const opportunity of await api.query( "ConnectionOpportunities", `ConnectionTypeId eq ${connectionTypeId}` ) ) {
        await deleteConnectionRequests( api, opportunity.Id );
        await api.delete( "ConnectionOpportunities", opportunity.Id );
    }

    for ( const status of await api.query( "ConnectionStatus", `ConnectionTypeId eq ${connectionTypeId}` ) ) {
        await api.delete( "ConnectionStatus", status.Id );
    }

    await api.delete( "ConnectionTypes", connectionTypeId );
}

/** Deletes connection types left behind by an earlier run that did not finish cleanup. */
export async function deleteLeftoverConnectionTypes( api: RockApi, name: string ): Promise<void> {
    for ( const connectionType of await api.query( "ConnectionTypes", `Name eq '${TestNamePrefix} ${name}'` ) ) {
        await deleteConnectionType( api, connectionType.Id );
    }
}
