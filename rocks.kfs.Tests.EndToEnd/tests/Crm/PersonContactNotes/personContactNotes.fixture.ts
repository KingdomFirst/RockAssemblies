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
import { test as base } from "@playwright/test";
import { getTestSettings } from "../../../shared/config";
import { guardFixtureSetup } from "../../../shared/runGuard";
import { RockApi, RockEntity } from "../../../shared/rockApi";
import { openPageAsPerson } from "../../../shared/rockBrowser";
import { EntityTypeName, SystemGuid, TestNamePrefix } from "../../../shared/systemGuids";
import { TestBlockPage, createBlockPage, deleteBlockPage, deleteLeftoverTestPages, ensureTestPagesParent, getBlockAttributeIds, getBlockTypeId, setBlockSettings } from "../../../shared/testData/cms";
import { GroupMemberStatus, TestGroup, addGroupMember, createGroup, deleteGroup } from "../../../shared/testData/groups";
import { TestPerson, ensurePerson } from "../../../shared/testData/people";
import { verifyRoleAccess } from "../../../shared/testData/security";
import { EntityCaptureWorkflowType, deleteWorkflowsOfType, findEntityCaptureWorkflowType } from "../../../shared/testData/workflows";
import { PersonContactNotesBlock } from "./personContactNotesBlock";

export const blockType = { name: "Person Contact Note", category: "KFS > CRM" };

/** The KFS beta site's workflow for the manual plan's "simple verification" in group member mode. */
export const setStatusActiveWorkflowTypeName = "Group Member - Set Status Active";

const names = {
    page: "PCN Test Page",
    group: "PCN Group"
};

/** Text the fixture puts in the Default Note Text setting, so the tests can see it pre-filled. */
export const defaultNoteText = "KFS E2E default note text";

/** Everything the Person Contact Notes tests need, built once per run and torn down after. */
export type PersonContactNotesFixture = {
    api: RockApi;
    rockVersion: string;
    blockPage: TestBlockPage;

    /** Staff member who enters the notes. Has Edit on both note types through the test role. */
    tester: TestPerson;
    /** Person the notes are about, picked with the person picker; also an active group member. */
    subject: TestPerson;
    /** Second group member, Pending, for the Set Status Active workflow. */
    pendingSubject: TestPerson;

    group: TestGroup;
    subjectMemberId: number;
    pendingMemberId: number;

    noteTypeIds: { person: number; groupMember: number };
    captureWorkflowType: EntityCaptureWorkflowType;

    /** Entity type Guids as recorded by the capture workflow (upper case). */
    entityTypeGuids: { personAlias: string; groupMember: string; note: string };

    /** Resets the block to the baseline configuration, then applies the overrides. */
    configure( overrides?: Record<string, string> ): Promise<void>;

    /** Opens the test page as the tester. */
    open( page: import( "@playwright/test" ).Page, parameters?: Record<string, string | number> ): Promise<void>;

    /** Notes of the type on the entity, newest first. */
    getNotes( noteTypeId: number, entityId: number ): Promise<RockEntity[]>;
};

export const test = base.extend<{ block: PersonContactNotesBlock; resetState: void }, { pcn: PersonContactNotesFixture }>( {
    pcn: [ async ( { browser }, use ) => guardFixtureSetup( "Person Contact Notes", async markSetupDone => {
        const settings = getTestSettings();
        const api = await RockApi.create();
        const rockVersion = await api.getRockVersion();
        console.log( `Rock version under test: ${rockVersion} at ${settings.baseUrl}` );

        // Clear leftovers from an earlier run that was killed before cleanup.
        await deleteLeftoverTestPages( api, names.page );
        for ( const leftover of await api.query( "Groups", `Name eq '${TestNamePrefix} ${names.group}'` ) ) {
            await deleteGroupAndMemberNotes( api, leftover.Id );
        }

        // Same person as the Person Attribute Forms tester: already in the KFS E2E Plugin Testers role.
        const tester = await ensurePerson( api, {
            guid: "6B0A9E43-2C5A-4E0B-9D0C-6F1F2B8A7E01",
            firstName: "Pafa",
            lastName: "KFS E2E Tester",
            email: "pafa.adult@kfs-e2e.invalid"
        } );
        const subject = await ensurePerson( api, {
            guid: "7C1B0F54-3D6B-4F1C-8E1D-7A2A3C9B8F11",
            firstName: "Pcnsubject",
            lastName: "KFS E2E Tester",
            email: "pcn.subject@kfs-e2e.invalid"
        } );
        const pendingSubject = await ensurePerson( api, {
            guid: "7C1B0F54-3D6B-4F1C-8E1D-7A2A3C9B8F12",
            firstName: "Pcnpending",
            lastName: "KFS E2E Tester",
            email: "pcn.pending@kfs-e2e.invalid"
        } );

        /*
            10/5/2026 - CLAUDE

            The note editor only offers note types the signed-in person may Edit, and on
            the KFS beta site only the RSR staff roles have Edit on Personal Note and
            Group Member Note. Members of those roles cannot sign in by token (Extreme
            account protection), and rules saved through REST are ignored on Rock 17.9,
            so the test role gets Edit on the two note types once, by hand.

            Reason: The tester needs Edit on the note types, granted through Rock's UI.
        */
        const noteTypeIds = {
            person: await api.getIdByGuid( "NoteTypes", SystemGuid.NoteType.PersonTimelineNote ),
            groupMember: await api.getIdByGuid( "NoteTypes", SystemGuid.NoteType.GroupMemberNote )
        };
        for ( const [ label, noteTypeId ] of [ [ "Personal Note", noteTypeIds.person ], [ "Group Member Note", noteTypeIds.groupMember ] ] as const ) {
            await verifyRoleAccess( api, {
                person: tester,
                entityTypeName: EntityTypeName.NoteType,
                entityId: noteTypeId,
                entityLabel: `the '${label}' note type (Admin Tools > Settings > Note Types, note type ${noteTypeId})`,
                action: "Edit",
                suggestedRole: "KFS E2E Plugin Testers"
            } );
        }

        // The block passes Person, GroupMember or Note to the workflow; the capture workflow records which.
        const captureWorkflowType = await findEntityCaptureWorkflowType( api );

        const created: { blockPage?: TestBlockPage; group?: TestGroup } = {};

        try {
            const group = await createGroup( api, { name: names.group, groupTypeGuid: SystemGuid.GroupType.SmallGroup } );
            created.group = group;
            const subjectMemberId = await addGroupMember( api, { groupId: group.id, personId: subject.id, status: GroupMemberStatus.Active } );
            const pendingMemberId = await addGroupMember( api, { groupId: group.id, personId: pendingSubject.id, status: GroupMemberStatus.Pending } );

            const testPagesParentId = await ensureTestPagesParent( api, settings.parentPageId );
            const blockTypeId = await getBlockTypeId( api, blockType.name, blockType.category );
            const blockPage = await createBlockPage( api, { name: names.page, parentPageId: testPagesParentId, blockTypeId, zone: settings.zone } );
            created.blockPage = blockPage;

            const open = async ( page: import( "@playwright/test" ).Page, parameters?: Record<string, string | number> ): Promise<void> => {
                await openPageAsPerson( page, api, { pageId: blockPage.pageId, personId: tester.id, parameters } );
            };

            // Rock registers a block type's settings the first time it loads it.
            if ( Object.keys( blockPage.attributeIds ).length === 0 ) {
                const warmUpPage = await browser.newPage( { baseURL: settings.baseUrl } );
                await open( warmUpPage );
                await warmUpPage.close();
                blockPage.attributeIds = await getBlockAttributeIds( api, blockTypeId );
            }

            const entityTypeGuid = async ( name: string ): Promise<string> => ( ( await api.single( "EntityTypes", `Name eq '${name}'` ) ).Guid ).toUpperCase();

            // Matches page 821 on the KFS beta site, limited to one note type per entity so the editor has no type choice.
            const baseline = (): Record<string, string> => ( {
                NoteTerm: "Note",
                DisplayType: "Full",
                ShowAlertCheckbox: "True",
                ShowPrivateCheckbox: "True",
                ShowSecurityButton: "True",
                AllowBackdatedNotes: "False",
                PersonNoteTypes: SystemGuid.NoteType.PersonTimelineNote.toLowerCase(),
                GroupMemberNoteTypes: SystemGuid.NoteType.GroupMemberNote.toLowerCase(),
                DisplayNoteTypeHeading: "False",
                ExpandReplies: "False",
                DefaultNoteText: defaultNoteText,
                Workflow: captureWorkflowType.guid.toLowerCase(),
                WorkflowEntity: "Person",
                StartNewSearch: "False"
            } );

            const fixture: PersonContactNotesFixture = {
                api,
                rockVersion,
                blockPage,
                tester,
                subject,
                pendingSubject,
                group,
                subjectMemberId,
                pendingMemberId,
                noteTypeIds,
                captureWorkflowType,
                entityTypeGuids: {
                    personAlias: await entityTypeGuid( EntityTypeName.PersonAlias ),
                    groupMember: await entityTypeGuid( EntityTypeName.GroupMember ),
                    note: await entityTypeGuid( EntityTypeName.Note )
                },
                configure: async ( overrides = {} ) => {
                    await setBlockSettings( api, blockPage, { ...baseline(), ...overrides } );
                },
                open,
                getNotes: async ( noteTypeId, entityId ) => await api.query( "Notes", `NoteTypeId eq ${noteTypeId} and EntityId eq ${entityId}`, "&$orderby=Id desc" )
            };

            markSetupDone();
            await use( fixture );
        }
        finally {
            await cleanUp( api, created );
            await api.dispose();
        }
    } ), { scope: "worker", timeout: 600_000 } ],

    /*
        Runs before every test: removes the notes and workflows an earlier test created
        and puts the group members back, so each test starts from the same state.
    */
    resetState: [ async ( { pcn }, use ) => {
        const { api } = pcn;

        test.info().annotations.push( { type: "rock-version", description: pcn.rockVersion } );

        await deleteWorkflowsOfType( api, pcn.captureWorkflowType.id );
        await deleteNotes( api, pcn );
        await api.patch( "GroupMembers", pcn.pendingMemberId, { GroupMemberStatus: GroupMemberStatus.Pending } );
        await pcn.configure();

        await use();
    }, { auto: true } ],

    block: async ( { page, pcn }, use ) => {
        await use( new PersonContactNotesBlock( page, pcn.blockPage.blockId ) );
    }
} );

export { expect } from "@playwright/test";

/** Deletes every note on the test people and group members. They exist only for these tests. */
async function deleteNotes( api: RockApi, pcn: PersonContactNotesFixture ): Promise<void> {
    const targets: Array<[ number, number ]> = [
        [ pcn.noteTypeIds.person, pcn.subject.id ],
        [ pcn.noteTypeIds.person, pcn.pendingSubject.id ],
        [ pcn.noteTypeIds.groupMember, pcn.subjectMemberId ],
        [ pcn.noteTypeIds.groupMember, pcn.pendingMemberId ]
    ];

    for ( const [ noteTypeId, entityId ] of targets ) {
        for ( const note of await pcn.getNotes( noteTypeId, entityId ) ) {
            await api.delete( "Notes", note.Id );
        }
    }
}

/** Best-effort teardown: keeps going past failures so one stuck record does not strand the rest. */
async function cleanUp( api: RockApi, created: { blockPage?: TestBlockPage; group?: TestGroup } ): Promise<void> {
    const steps: Array<[ string, () => Promise<void> ]> = [];

    if ( created.blockPage ) {
        const blockPage = created.blockPage;
        steps.push( [ "test page", () => deleteBlockPage( api, blockPage ) ] );
    }
    if ( created.group ) {
        const group = created.group;
        steps.push( [ `group ${group.id}`, () => deleteGroupAndMemberNotes( api, group.id ) ] );
    }

    for ( const [ name, step ] of steps ) {
        try {
            await step();
        }
        catch ( error ) {
            console.warn( `Cleanup of ${name} failed; the next run will retry it. ${( error as Error ).message}` );
        }
    }
}

/** Deletes the group and the notes on its members. Notes point at a member only by EntityId, so Rock leaves them behind. */
async function deleteGroupAndMemberNotes( api: RockApi, groupId: number ): Promise<void> {
    const noteTypeId = await api.getIdByGuid( "NoteTypes", SystemGuid.NoteType.GroupMemberNote );
    for ( const member of await api.query( "GroupMembers", `GroupId eq ${groupId}` ) ) {
        for ( const note of await api.query( "Notes", `NoteTypeId eq ${noteTypeId} and EntityId eq ${member.Id}` ) ) {
            await api.delete( "Notes", note.Id );
        }
    }

    await deleteGroup( api, groupId );
}
