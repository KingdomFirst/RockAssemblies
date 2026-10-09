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
import { RockApi } from "../rockApi";
import { SystemGuid } from "../systemGuids";

export type TestPerson = {
    id: number;
    guid: string;
    primaryAliasId: number;
    familyId: number;
    fullName: string;
};

export type TestPersonArgs = {
    /** Fixed Guid, so the same record is reused on every run instead of piling up duplicates. */
    guid: string;
    firstName: string;
    lastName: string;
    email: string;
};

/*
    9/23/2026 - CLAUDE

    Test people are find-or-create by a fixed Guid and are never deleted. Rock
    resists deleting people (aliases, history, family membership), and a stable
    record keeps the site tidy across hundreds of runs. Tests reset any field
    they assert on before acting.

    Reason: People cannot be cleanly deleted through the REST API.
*/

/** Finds the test person by Guid, or creates them in a new family. */
export async function ensurePerson( api: RockApi, args: TestPersonArgs ): Promise<TestPerson> {
    let person = await api.getByGuid( "People", args.guid );

    if ( !person ) {
        await api.post( "/api/People", {
            Guid: args.guid,
            FirstName: args.firstName,
            NickName: args.firstName,
            LastName: args.lastName,
            Email: args.email,
            IsEmailActive: true,
            Gender: 0
        } );

        person = await api.getByGuid( "People", args.guid );
        if ( !person ) {
            throw new Error( `Could not create test person ${args.firstName} ${args.lastName}. Rock may have matched an existing person with the same name and email; merge or rename that record.` );
        }
    }

    return await loadTestPerson( api, person.Id );
}

/** Finds the family member by Guid, or adds them to the given family as a child. */
export async function ensureFamilyMember( api: RockApi, familyId: number, args: TestPersonArgs ): Promise<TestPerson> {
    let person = await api.getByGuid( "People", args.guid );

    if ( !person ) {
        const childRoleId = await api.getIdByGuid( "GroupTypeRoles", SystemGuid.GroupRole.FamilyMemberChild );
        await api.post( `/api/People/AddNewPersonToFamily/${familyId}?groupRoleId=${childRoleId}`, {
            Guid: args.guid,
            FirstName: args.firstName,
            NickName: args.firstName,
            LastName: args.lastName,
            Email: args.email,
            IsEmailActive: true,
            Gender: 0
        } );

        person = await api.getByGuid( "People", args.guid );
        if ( !person ) {
            throw new Error( `Could not create test family member ${args.firstName} ${args.lastName}.` );
        }
    }

    return await loadTestPerson( api, person.Id );
}

export async function loadTestPerson( api: RockApi, personId: number ): Promise<TestPerson> {
    const person = await api.getById( "People", personId );
    // PersonAlias.AliasPersonId is not exposed to OData; the primary alias shares the person's Guid.
    const primaryAliasId = person.PrimaryAliasId as number | null
        ?? ( await api.single( "PersonAlias", `PersonId eq ${personId} and Guid eq guid'${person.Guid}'` ) ).Id;
    const familyId = person.PrimaryFamilyId as number | null;
    if ( !familyId ) {
        throw new Error( `Test person ${personId} has no primary family.` );
    }

    return {
        id: person.Id,
        guid: person.Guid,
        primaryAliasId,
        familyId,
        fullName: `${person.NickName} ${person.LastName}`
    };
}

/**
 * Gets a "rckipid=..." query string fragment that signs the browser in as this person,
 * limited to one page unless pageId is null. This avoids handling any password in the tests.
 */
export async function getImpersonationParameter( api: RockApi, personId: number, pageId: number | null ): Promise<string> {
    const pageLimit = pageId === null ? "" : `&pageId=${pageId}`;
    return await api.get<string>( `/api/People/GetImpersonationParameter?personId=${personId}${pageLimit}&usageLimit=10` );
}
