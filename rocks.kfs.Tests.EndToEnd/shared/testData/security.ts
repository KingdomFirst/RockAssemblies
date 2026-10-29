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
import { RockApi, RockEntity, asEnumValue } from "../rockApi";
import { GroupMemberStatus, getMemberStatus } from "./groups";
import { TestPerson } from "./people";

/*
    10/5/2026 - CLAUDE

    Tests cannot grant themselves access on Rock 17.9:

    - A security rule saved through the REST API is stored but ignored. Rock
      keeps every rule in one cache that only Rock's own security editor (or a
      security-role membership change) refreshes.
    - Security role membership cannot be changed through the REST API ("Security
      role membership cannot be managed through this endpoint").

    So every test person gets access once, by hand in Rock: a test-only security
    role (Elevated Security Level: None) with the rights the tests need. These
    checks confirm that setup before any browser opens a page, and stop with the
    exact step that is missing.

    Reason: Test access must be set up once by an admin through Rock's own UI.
*/

/** Rock's AccountProtectionProfile enum, in value order. */
const AccountProtectionProfiles = [ "Low", "Medium", "High", "Extreme" ] as const;

/**
 * Checks that the person is an active member of a security role that has an Allow rule for
 * the action directly on the entity (site, page, block, note type, group type or attribute), and that the site lets them sign in by
 * token. Throws with the one-time setup step if not.
 */
export async function verifyRoleAccess( api: RockApi, args: {
    person: TestPerson;
    entityTypeName: "Rock.Model.Site" | "Rock.Model.Page" | "Rock.Model.Block" | "Rock.Model.NoteType" | "Rock.Model.GroupType" | "Rock.Model.Attribute";
    entityId: number;
    entityLabel: string;
    /** A Rock action (View, Edit, Administrate) or a block's own security action key. */
    action: string;
    suggestedRole: string;
} ): Promise<void> {
    const entityTypeId = await api.getEntityTypeId( args.entityTypeName );
    const rules = await api.query( "Auths", `EntityTypeId eq ${entityTypeId} and EntityId eq ${args.entityId} and Action eq '${args.action}' and AllowOrDeny eq 'A'` );

    const roleNames: string[] = [];
    for ( const rule of rules.filter( r => r.GroupId ) ) {
        const role = await api.getById( "Groups", rule.GroupId as number );
        roleNames.push( String( role.Name ) );
        if ( await isActiveMember( api, role, args.person ) ) {
            await verifyTokenSignInAllowed( api, args.person );
            return;
        }
    }

    throw new Error( `One-time setup: ${args.person.fullName} (person ${args.person.id}) needs ${args.action} on ${args.entityLabel} through a security role. ` +
        `In Rock, create a security role '${args.suggestedRole}' with Elevated Security Level None, add ${args.person.fullName} to it, and add an Allow rule for ${args.action} for it on ${args.entityLabel} (above any 'All Users' Deny rule). ` +
        `Roles that currently have ${args.action} there: ${roleNames.join( ", " ) || "(none)"}. ` +
        "Avoid roles with Elevated Security Level Extreme: Rock refuses token sign-in for their members." );
}

/**
 * Checks that the site lets the tests sign in as this person by impersonation token.
 * Rock refuses token sign-in for the account protection profiles listed in the site's
 * security settings (Extreme by default); a person's profile rises with the Elevated
 * Security Level of their roles, and only the Process Elevated Security job lowers it.
 */
export async function verifyTokenSignInAllowed( api: RockApi, person: TestPerson ): Promise<void> {
    const record = await api.getById( "People", person.id );
    const profileValue = asEnumValue( record.AccountProtectionProfile, AccountProtectionProfiles );

    const setting = await api.first( "Attributes", "Key eq 'core_RockSecuritySettings'" );
    const raw = setting ? ( await api.first( "AttributeValues", `AttributeId eq ${setting.Id}` ) )?.Value ?? setting.DefaultValue : null;
    let disabled: number[] = [ 3 ];
    try {
        const parsed = JSON.parse( String( raw ) ) as { DisableTokensForAccountProtectionProfiles?: unknown[] };
        disabled = ( parsed.DisableTokensForAccountProtectionProfiles ?? [] ).map( p => asEnumValue( p, AccountProtectionProfiles ) );
    }
    catch {
        // No readable settings: Rock's default disables tokens for Extreme only.
    }

    if ( disabled.includes( profileValue ) ) {
        throw new Error( `${person.fullName} (person ${person.id}) has account protection profile ${AccountProtectionProfiles[ profileValue ] ?? profileValue}, and this site refuses sign-in tokens for that profile, so the tests cannot sign in as them. ` +
            "The profile comes from the Elevated Security Level of their security roles. Remove them from any Extreme role (e.g. RSR - Staff Workers), then run the 'Process Elevated Security' job in Jobs Administration; only that job lowers the profile." );
    }
}

async function isActiveMember( api: RockApi, role: RockEntity, person: TestPerson ): Promise<boolean> {
    const memberships = await api.query( "GroupMembers", `GroupId eq ${role.Id} and PersonId eq ${person.id}` );
    return memberships.some( member => getMemberStatus( member ) === GroupMemberStatus.Active );
}
