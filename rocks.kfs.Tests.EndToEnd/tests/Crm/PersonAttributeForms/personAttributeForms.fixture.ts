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
import { randomUUID } from "crypto";
import { getTestSettings } from "../../../shared/config";
import { guardFixtureSetup } from "../../../shared/runGuard";
import { RockApi } from "../../../shared/rockApi";
import { openPageAsPerson } from "../../../shared/rockBrowser";
import { EntityTypeName, SystemGuid } from "../../../shared/systemGuids";
import { createPersonTextAttribute, deleteLeftoverPersonAttribute, deletePersonAttribute } from "../../../shared/testData/attributes";
import { TestBlockPage, TestPagesParent, createBlockPage, deleteBlockPage, deleteLeftoverTestPages, ensureTestPagesParent, getBlockAttributeIds, getBlockTypeId, setBlockSettings } from "../../../shared/testData/cms";
import { TestConnectionSetup, createConnectionSetup, deleteConnectionRequests, deleteConnectionType, deleteLeftoverConnectionTypes } from "../../../shared/testData/connections";
import { TestGroup, createGroup, deleteGroup, deleteGroupMembers, deleteLeftoverGroups } from "../../../shared/testData/groups";
import { TestPerson, ensureFamilyMember, ensurePerson } from "../../../shared/testData/people";
import { verifyRoleAccess } from "../../../shared/testData/security";
import { EntityCaptureWorkflowType, deleteLeftoverWorkflowTypes, deleteWorkflowType, deleteWorkflowsOfType, findEntityCaptureWorkflowType } from "../../../shared/testData/workflows";
import { PersonAttributeFormsBlock, confirmationCssClass } from "./personAttributeFormsBlock";

export const blockType = { name: "Person Attribute Forms Advanced", category: "KFS > CRM" };

/** Values of the block's FormFieldSource enum. */
const FormFieldSource = { PersonAttribute: 0, PersonField: 1 } as const;

/** Values of the block's PersonFieldType enum. */
export const PersonFieldType = { Email: 4, MiddleName: 14 } as const;

const names = {
    page: "PAFA Test Page",
    attributeKey: "KFSE2EPafaText",
    attributeName: "PAFA Text",
    allowedGroup: "PAFA Small Group",
    disallowedGroup: "PAFA Serving Team",
    connectionType: "PAFA Connections",
    personWorkflow: "PAFA Person Workflow",
    groupMemberWorkflow: "PAFA Group Member Workflow",
    connectionRequestWorkflow: "PAFA Connection Request Workflow"
};

/** Everything the Person Attribute Forms tests need, built once per run and torn down after. */
export type PersonAttributeFormsFixture = {
    api: RockApi;
    rockVersion: string;
    blockPage: TestBlockPage;

    /** Adult who fills out the form. Has View and Edit on the test page. */
    person: TestPerson;
    /** Child in the same family as `person`. */
    familyMember: TestPerson;
    /** Adult in a different family. */
    outsider: TestPerson;

    personAttributeId: number;

    /** Small Group: an allowed group type. */
    allowedGroup: TestGroup;
    /** Serving Team: not an allowed group type. */
    disallowedGroup: TestGroup;

    connection: TestConnectionSetup;

    workflows: {
        person: EntityCaptureWorkflowType;
        groupMember: EntityCaptureWorkflowType;
        connectionRequest: EntityCaptureWorkflowType;
    };

    /** Entity type Guids as recorded by the capture workflows (upper case). */
    entityTypeGuids: {
        personAlias: string;
        groupMember: string;
        connectionRequest: string;
    };

    /** Resets the block to the baseline configuration, then applies the overrides. */
    configure( overrides?: Record<string, string> ): Promise<void>;

    /** Builds the Forms setting JSON. By default one form with Middle Name, Email and the test attribute. */
    buildFormsJson( formCount?: number ): string;
};

export const test = base.extend<{ block: PersonAttributeFormsBlock; resetState: void }, { pafa: PersonAttributeFormsFixture }>( {
    pafa: [ async ( { browser }, use ) => guardFixtureSetup( "Person Attribute Forms", async markSetupDone => {
        const settings = getTestSettings();
        const api = await RockApi.create();
        const rockVersion = await api.getRockVersion();
        console.log( `Rock version under test: ${rockVersion} at ${settings.baseUrl}` );

        // Clear leftovers from an earlier run that was killed before cleanup.
        await deleteLeftoverTestPages( api, names.page );
        await deleteLeftoverWorkflowTypes( api, names.personWorkflow );
        await deleteLeftoverWorkflowTypes( api, names.groupMemberWorkflow );
        await deleteLeftoverWorkflowTypes( api, names.connectionRequestWorkflow );
        await deleteLeftoverGroups( api, names.allowedGroup );
        await deleteLeftoverGroups( api, names.disallowedGroup );
        await deleteLeftoverConnectionTypes( api, names.connectionType );
        await deleteLeftoverPersonAttribute( api, names.attributeKey );

        const person = await ensurePerson( api, {
            guid: "6B0A9E43-2C5A-4E0B-9D0C-6F1F2B8A7E01",
            firstName: "Pafa",
            lastName: "KFS E2E Tester",
            email: "pafa.adult@kfs-e2e.invalid"
        } );
        const familyMember = await ensureFamilyMember( api, person.familyId, {
            guid: "6B0A9E43-2C5A-4E0B-9D0C-6F1F2B8A7E02",
            firstName: "Pafakid",
            lastName: "KFS E2E Tester",
            email: "pafa.child@kfs-e2e.invalid"
        } );
        const outsider = await ensurePerson( api, {
            guid: "6B0A9E43-2C5A-4E0B-9D0C-6F1F2B8A7E03",
            firstName: "Pafaoutsider",
            lastName: "KFS E2E Tester",
            email: "pafa.outsider@kfs-e2e.invalid"
        } );

        /*
            10/5/2026 - CLAUDE

            Access comes from a test-only security role set up once by hand on the
            "KFS E2E Test Pages" parent page; the fixture cannot grant it itself on
            Rock 17.9 (see shared/testData/security.ts). View is inherited from the
            site. Only the form-filling adult opens the page, and the Edit Forms and
            Fields test needs Edit on the block, inherited from the parent page.

            Reason: REST-saved security rules are ignored on Rock 17.9.
        */
        const testPagesParentId = await ensureTestPagesParent( api, settings.parentPageId );
        await verifyRoleAccess( api, {
            person,
            entityTypeName: EntityTypeName.Page,
            entityId: testPagesParentId,
            entityLabel: `the page '${TestPagesParent.name}' (page ${testPagesParentId})`,
            action: "Edit",
            suggestedRole: "KFS E2E Plugin Testers"
        } );

        // One persistent capture workflow type serves all three cases: each test points the
        // block at it and checks which entity the workflow received (see workflows.ts).
        const captureWorkflowType = await findEntityCaptureWorkflowType( api );

        const created: { blockPage?: TestBlockPage; personAttributeId?: number; groups: TestGroup[]; connectionTypeId?: number; workflowTypeIds: number[] } = { groups: [], workflowTypeIds: [] };

        try {
            created.personAttributeId = await createPersonTextAttribute( api, { key: names.attributeKey, name: names.attributeName } );

            const allowedGroup = await createGroup( api, { name: names.allowedGroup, groupTypeGuid: SystemGuid.GroupType.SmallGroup } );
            created.groups.push( allowedGroup );
            const disallowedGroup = await createGroup( api, { name: names.disallowedGroup, groupTypeGuid: SystemGuid.GroupType.ServingTeam } );
            created.groups.push( disallowedGroup );

            const connection = await createConnectionSetup( api, names.connectionType );
            created.connectionTypeId = connection.connectionTypeId;

            const workflows = {
                person: captureWorkflowType,
                groupMember: captureWorkflowType,
                connectionRequest: captureWorkflowType
            };

            const blockTypeId = await getBlockTypeId( api, blockType.name, blockType.category );
            const blockPage = await createBlockPage( api, { name: names.page, parentPageId: testPagesParentId, blockTypeId, zone: settings.zone } );
            created.blockPage = blockPage;

            // Rock registers a block type's settings the first time it loads it.
            if ( Object.keys( blockPage.attributeIds ).length === 0 ) {
                const warmUpPage = await browser.newPage( { baseURL: settings.baseUrl } );
                await openPageAsPerson( warmUpPage, api, { pageId: blockPage.pageId, personId: person.id } );
                await warmUpPage.close();
                blockPage.attributeIds = await getBlockAttributeIds( api, blockTypeId );
            }

            const entityTypeGuid = async ( name: string ): Promise<string> => ( ( await api.single( "EntityTypes", `Name eq '${name}'` ) ).Guid ).toUpperCase();

            const personAttributeId = created.personAttributeId;
            const personAttributeGuid = String( ( await api.getById( "Attributes", personAttributeId ) ).Guid ).toLowerCase();

            const buildFormsJson = ( formCount = 1 ): string => {
                const fields = [
                    { FieldSource: FormFieldSource.PersonField, PersonFieldType: PersonFieldType.MiddleName, AttributeId: null },
                    { FieldSource: FormFieldSource.PersonField, PersonFieldType: PersonFieldType.Email, AttributeId: null },
                    { FieldSource: FormFieldSource.PersonAttribute, PersonFieldType: 0, AttributeId: personAttributeId }
                ].map( ( field, order ) => ( {
                    // The block keys a Person Attribute field by the attribute's own Guid (it resets any other
                    // Guid when the Edit Forms and Fields dialog binds, for conditional-field support), so match it.
                    Guid: field.FieldSource === FormFieldSource.PersonAttribute ? personAttributeGuid : randomUUID(),
                    AttributeId: field.AttributeId,
                    ShowCurrentValue: true,
                    IsRequired: false,
                    Order: order,
                    PreText: "",
                    PostText: "",
                    FieldSource: field.FieldSource,
                    RegistrationFieldSource: 0,
                    PersonFieldType: field.PersonFieldType,
                    RegistrationPersonFieldType: 0
                } ) );

                // With more than one form, split the fields across them so Next/Previous is exercised.
                const forms = [];
                for ( let i = 0; i < formCount; i++ ) {
                    forms.push( {
                        Guid: randomUUID(),
                        Name: `KFS E2E Form ${i + 1}`,
                        Header: "",
                        Footer: "",
                        Order: i,
                        Expanded: true,
                        Fields: formCount === 1 ? fields : fields.filter( ( _field, index ) => index % formCount === i )
                    } );
                }

                return JSON.stringify( forms );
            };

            const baseline = (): Record<string, string> => ( {
                AllowConnectionOpportunity: "False",
                AllowGroupMembership: "False",
                EnablePassingGroupId: "True",
                AllowedGroupTypes: SystemGuid.GroupType.SmallGroup.toLowerCase(),
                Group: "",
                GroupMemberStatus: "2",
                DisplaySMSCheckboxonMobilePhone: "False",
                PersonMode: "Family Members",
                DisplayFamilyMemberPicker: "False",
                DisplayProgressBar: "False",
                SaveValues: "END",
                Workflow: "",
                WorkflowEntity: "Person",
                DonePage: "",
                Forms: buildFormsJson(),
                ConfirmationText: `<p class="${confirmationCssClass}">KFS E2E form complete.</p>`,
                SignatureDocumentTemplate: "",
                DisableFormforChildren: "False"
            } );

            const fixture: PersonAttributeFormsFixture = {
                api,
                rockVersion,
                blockPage,
                person,
                familyMember,
                outsider,
                personAttributeId,
                allowedGroup,
                disallowedGroup,
                connection,
                workflows,
                entityTypeGuids: {
                    personAlias: await entityTypeGuid( EntityTypeName.PersonAlias ),
                    groupMember: await entityTypeGuid( EntityTypeName.GroupMember ),
                    connectionRequest: await entityTypeGuid( EntityTypeName.ConnectionRequest )
                },
                configure: async ( overrides = {} ) => {
                    await setBlockSettings( api, blockPage, { ...baseline(), ...overrides } );
                },
                buildFormsJson
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
        Runs before every test: removes anything an earlier test created, so each test
        starts from the same state and tests do not depend on order.
    */
    resetState: [ async ( { pafa }, use ) => {
        const { api } = pafa;

        test.info().annotations.push( { type: "rock-version", description: pafa.rockVersion } );

        await deleteGroupMembers( api, pafa.allowedGroup.id );
        await deleteGroupMembers( api, pafa.disallowedGroup.id );
        await deleteConnectionRequests( api, pafa.connection.opportunityId );
        // All three point at the one persistent capture workflow type.
        for ( const workflowTypeId of new Set( [ pafa.workflows.person.id, pafa.workflows.groupMember.id, pafa.workflows.connectionRequest.id ] ) ) {
            await deleteWorkflowsOfType( api, workflowTypeId );
        }

        for ( const testPerson of [ pafa.person, pafa.familyMember, pafa.outsider ] ) {
            await api.patch( "People", testPerson.id, { MiddleName: "" } );
            await api.setAttributeValue( pafa.personAttributeId, testPerson.id, "" );
        }

        await pafa.configure();

        await use();
    }, { auto: true } ],

    block: async ( { page, pafa }, use ) => {
        await use( new PersonAttributeFormsBlock( page, pafa.blockPage.blockId ) );
    }
} );

export { expect } from "@playwright/test";

/** Best-effort teardown: keeps going past failures so one stuck record does not strand the rest. */
async function cleanUp( api: RockApi, created: { blockPage?: TestBlockPage; personAttributeId?: number; groups: TestGroup[]; connectionTypeId?: number; workflowTypeIds: number[] } ): Promise<void> {
    const steps: Array<[ string, () => Promise<void> ]> = [];

    if ( created.blockPage ) {
        const blockPage = created.blockPage;
        steps.push( [ "test page", () => deleteBlockPage( api, blockPage ) ] );
    }
    for ( const workflowTypeId of created.workflowTypeIds ) {
        steps.push( [ `workflow type ${workflowTypeId}`, () => deleteWorkflowType( api, workflowTypeId ) ] );
    }
    if ( created.connectionTypeId ) {
        const connectionTypeId = created.connectionTypeId;
        steps.push( [ "connection type", () => deleteConnectionType( api, connectionTypeId ) ] );
    }
    for ( const group of created.groups ) {
        steps.push( [ `group ${group.id}`, () => deleteGroup( api, group.id ) ] );
    }
    if ( created.personAttributeId ) {
        const personAttributeId = created.personAttributeId;
        steps.push( [ "person attribute", () => deletePersonAttribute( api, personAttributeId ) ] );
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
