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
import { RockApi, RockEntity, asEnumValue } from "../rockApi";
import { TestNamePrefix } from "../systemGuids";

export type TestGroup = {
    id: number;
    guid: string;
    groupTypeGuid: string;
};

/** Rock's GroupMemberStatus enum. */
export const GroupMemberStatus = {
    Inactive: 0,
    Active: 1,
    Pending: 2
} as const;

/** Gets a group member's status as its numeric GroupMemberStatus value. */
export function getMemberStatus( member: RockEntity ): number {
    return asEnumValue( member.GroupMemberStatus, [ "Inactive", "Active", "Pending" ] );
}

export async function createGroup( api: RockApi, args: { name: string; groupTypeGuid: string } ): Promise<TestGroup> {
    const guid = randomUUID();

    // Some sites set "Groups Require Campus" on group types like Small Group; any active campus satisfies it.
    const campus = await api.first( "Campuses", "IsActive eq true", "&$orderby=Order" );

    const id = await api.post( "/api/Groups", {
        CampusId: campus ? campus.Id : null,
        Guid: guid,
        Name: `${TestNamePrefix} ${args.name}`,
        GroupTypeId: await api.getIdByGuid( "GroupTypes", args.groupTypeGuid ),
        IsActive: true,
        IsPublic: true,
        IsSystem: false,
        IsSecurityRole: false,
        Order: 0
    } );

    return { id, guid, groupTypeGuid: args.groupTypeGuid };
}

/** Adds the person to the group in the group type's default role. Returns the GroupMember Id. */
export async function addGroupMember( api: RockApi, args: { groupId: number; personId: number; status: number } ): Promise<number> {
    const group = await api.getById( "Groups", args.groupId );
    const groupType = await api.getById( "GroupTypes", group.GroupTypeId as number );

    return await api.post( "/api/GroupMembers", {
        Guid: randomUUID(),
        GroupId: args.groupId,
        GroupTypeId: groupType.Id,
        PersonId: args.personId,
        GroupRoleId: groupType.DefaultGroupRoleId,
        GroupMemberStatus: args.status,
        IsSystem: false
    } );
}

export async function getGroupMembers( api: RockApi, groupId: number, personId: number ): Promise<RockEntity[]> {
    return await api.query( "GroupMembers", `GroupId eq ${groupId} and PersonId eq ${personId}` );
}

export async function deleteGroupMembers( api: RockApi, groupId: number ): Promise<void> {
    for ( const member of await api.query( "GroupMembers", `GroupId eq ${groupId}` ) ) {
        await api.delete( "GroupMembers", member.Id );
    }
}

export async function deleteGroup( api: RockApi, groupId: number ): Promise<void> {
    await deleteGroupMembers( api, groupId );
    await api.delete( "Groups", groupId );
}

/** Deletes groups left behind by an earlier run that did not finish cleanup. */
export async function deleteLeftoverGroups( api: RockApi, name: string ): Promise<void> {
    for ( const group of await api.query( "Groups", `Name eq '${TestNamePrefix} ${name}'` ) ) {
        await deleteGroup( api, group.Id );
    }
}
