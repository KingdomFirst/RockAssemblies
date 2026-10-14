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
import { Page, test as base } from "@playwright/test";
import { randomUUID } from "crypto";
import { getOptionalSetting, getTestSettings } from "../../../shared/config";
import { guardFixtureSetup } from "../../../shared/runGuard";
import { RockApi, RockEntity } from "../../../shared/rockApi";
import { openPageAsPerson } from "../../../shared/rockBrowser";
import { EntityTypeName, TestNamePrefix } from "../../../shared/systemGuids";
import { CopiedPage, TestBlockPage, copyBlockSettings, createBlockPage, createPageCopy, deleteBlockPage, deleteLeftoverTestPages, ensureTestPagesParent, setBlockSettings } from "../../../shared/testData/cms";
import { deleteGroup } from "../../../shared/testData/groups";
import { TestPerson, ensurePerson } from "../../../shared/testData/people";
import { verifyRoleAccess } from "../../../shared/testData/security";
import { EventbriteSettingsBlock, GroupViewerPage, protectEventbriteToken } from "./eventbritePages";

/** Guids the plugin installs (rocks.kfs.EventBrite: EBGuid/FieldType.cs, Migrations/002_PageAndBlocks.cs). */
const PluginGuid = {
    eventFieldType: "904A0CD2-217C-4CC0-B26D-5D0B48487D21",
    personFieldType: "3BB985EA-DC6A-4292-99D2-63F16F25A592",
    settingsBlockType: "7B62C3FA-8AA9-4DFE-B9A9-F7CA86AE99C2",
    syncBlockType: "4B8AD808-1378-456D-A568-A9C844B2151D",
    settingsPage: "B23A7712-54FB-4BA1-BBE7-F0B6077166FD"
} as const;

/** Rock core Guids: the Group Viewer page and the blocks the tests drive on its copy. */
const CoreGuid = {
    groupViewerPage: "4E237286-B715-4109-A578-C1445EC02707",
    groupDetailBlockType: "582BEEA1-5B27-444D-BC0A-F60CEB053981",
    groupMemberListBlockType: "88B7EFA9-7419-4D05-9F88-38B936E61EDD"
} as const;

/** Fixed Guids for this suite's persistent test data. */
const TestGuid = {
    groupType: "7E8F2A11-E2E0-4C0E-8C0E-0000000000D1",
    eventAttribute: "7E8F2A11-E2E0-4C0E-8C0E-0000000000D2",
    personAttribute: "7E8F2A11-E2E0-4C0E-8C0E-0000000000D3",
    parentGroup: "7E8F2A11-E2E0-4C0E-8C0E-0000000000D4"
} as const;

const names = {
    groupType: `${TestNamePrefix} Eventbrite`,
    parentGroup: `${TestNamePrefix} Eventbrite Groups`,
    settingsPage: "EB Settings Test Page",
    viewerPage: "EB Group Viewer Test Page"
};

export type EventbriteSyncFixture = {
    api: RockApi;
    rockVersion: string;
    tester: TestPerson;

    /** The hand-made Eventbrite event the tests link and sync. */
    event: { id: string; name: string };

    groupTypeId: number;
    parentGroupId: number;
    eventAttributeId: number;
    personAttributeId: number;

    settingsPage: TestBlockPage;
    viewerPage: CopiedPage & { groupDetailBlockId: number; memberListBlockId: number };

    /** Opens the settings test page as the tester, with the private token redacted. */
    openSettings( page: Page ): Promise<EventbriteSettingsBlock>;
    /** Opens the Group Viewer copy for the group as the tester. */
    openGroupViewer( page: Page, groupId: number ): Promise<GroupViewerPage>;

    /** Creates a test group under the test parent group, optionally already linked to the event. */
    createGroup( args: { linked: boolean } ): Promise<number>;
    /** The group's Eventbrite Event value: "{eventId}" or "{eventId}^{last sync}". */
    getEventValue( groupId: number ): Promise<string>;
    /** Test groups currently under the parent group (the ones the settings page creates land here too). */
    getChildGroups(): Promise<RockEntity[]>;
};

export const test = base.extend<{ resetState: void }, { eb: EventbriteSyncFixture }>( {
    eb: [ async ( { browser }, use ) => guardFixtureSetup( "Eventbrite Sync", async markSetupDone => {
        const settings = getTestSettings();
        const api = await RockApi.create();
        const rockVersion = await api.getRockVersion();
        console.log( `Rock version under test: ${rockVersion} at ${settings.baseUrl}` );

        await deleteLeftoverTestPages( api, names.settingsPage );
        await deleteLeftoverTestPages( api, names.viewerPage );

        const tester = await ensurePerson( api, {
            guid: "6B0A9E43-2C5A-4E0B-9D0C-6F1F2B8A7E01",
            firstName: "Pafa",
            lastName: "KFS E2E Tester",
            email: "pafa.adult@kfs-e2e.invalid"
        } );

        const { groupTypeId, eventAttributeId, personAttributeId } = await ensureGroupType( api );
        const parentGroupId = await ensureParentGroup( api, groupTypeId );
        await deleteChildGroups( api, parentGroupId );

        // One-time setup that REST cannot do on Rock 17.9: checked here, done by hand in Rock.
        // Every missing step is collected and reported together.
        const setupProblems: string[] = [];
        const collect = async ( check: () => Promise<void> ): Promise<void> => {
            try {
                await check();
            }
            catch ( error ) {
                setupProblems.push( ( error as Error ).message );
            }
        };
        await collect( () => verifyChildGroupType( api, groupTypeId ) );
        for ( const action of [ "View", "Edit" ] as const ) {
            await collect( () => verifyRoleAccess( api, {
                person: tester,
                entityTypeName: "Rock.Model.GroupType",
                entityId: groupTypeId,
                entityLabel: `the '${names.groupType}' group type (Admin Tools > Settings > Group Types > ${names.groupType} > Security)`,
                action,
                suggestedRole: "KFS E2E Plugin Testers"
            } ) );
        }

        // Group Detail only offers attributes the person may Edit, judged on the attribute itself.
        await collect( () => verifyRoleAccess( api, {
            person: tester,
            entityTypeName: "Rock.Model.Attribute",
            entityId: eventAttributeId,
            entityLabel: `the 'Eventbrite Event' group attribute of '${names.groupType}' (Admin Tools > Settings > Group Types > ${names.groupType} > Edit > Group Attributes > Eventbrite Event > lock icon)`,
            action: "Edit",
            suggestedRole: "KFS E2E Plugin Testers"
        } ) );

        const created: { pages: number[] } = { pages: [] };

        try {
            const testPagesParentId = await ensureTestPagesParent( api, settings.parentPageId );

            const syncBlockTypeId = await api.getIdByGuid( "BlockTypes", PluginGuid.syncBlockType );
            const copied = await createPageCopy( api, { sourcePageGuid: CoreGuid.groupViewerPage, name: names.viewerPage, blockTypeId: syncBlockTypeId, parentPageId: testPagesParentId } );
            created.pages.push( copied.pageId );
            const copiedBlockId = async ( blockTypeGuid: string ): Promise<number> => ( await api.single( "Blocks",
                `PageId eq ${copied.pageId} and BlockTypeId eq ${await api.getIdByGuid( "BlockTypes", blockTypeGuid )}` ) ).Id;
            const viewerPage = {
                ...copied,
                groupDetailBlockId: await copiedBlockId( CoreGuid.groupDetailBlockType ),
                memberListBlockId: await copiedBlockId( CoreGuid.groupMemberListBlockType )
            };
            const viewerPageGuid = String( ( await api.getById( "Pages", copied.pageId ) ).Guid ).toLowerCase();

            // The settings block as installed on the site, pointed at the test group type, parent and viewer page.
            const settingsBlockTypeId = await api.getIdByGuid( "BlockTypes", PluginGuid.settingsBlockType );
            const settingsPage = await createBlockPage( api, { name: names.settingsPage, parentPageId: testPagesParentId, blockTypeId: settingsBlockTypeId, zone: settings.zone } );
            created.pages.push( settingsPage.pageId );
            const installedPage = await api.getByGuid( "Pages", PluginGuid.settingsPage );
            const installedBlock = installedPage ? await api.first( "Blocks", `PageId eq ${installedPage.Id} and BlockTypeId eq ${settingsBlockTypeId}` ) : null;
            if ( installedBlock ) {
                await copyBlockSettings( api, { fromBlockId: installedBlock.Id, fromBlockTypeId: settingsBlockTypeId, toBlockId: settingsPage.blockId, toBlockTypeId: settingsBlockTypeId } );
            }
            await setBlockSettings( api, settingsPage, {
                GroupDetail: viewerPageGuid,
                NewGroupParent: TestGuid.parentGroup.toLowerCase(),
                NewGroupType: TestGuid.groupType.toLowerCase(),
                NewEventStatuses: "live,completed,draft,canceled,started,ended",
                EnableLogging: "False"
            } );

            const openSettings = async ( page: Page ): Promise<EventbriteSettingsBlock> => {
                await protectEventbriteToken( page, settingsPage.pageId );
                await openPageAsPerson( page, api, { pageId: settingsPage.pageId, personId: tester.id } );
                return new EventbriteSettingsBlock( page, settingsPage.blockId );
            };

            // The test event, found by name in the settings page's list of events not yet linked to a group.
            const eventName = getOptionalSetting( "EVENTBRITE_TEST_EVENT_NAME" ) ?? `${TestNamePrefix} Test Event`;
            const lookupPage = await browser.newPage( { baseURL: settings.baseUrl } );
            let eventId: string | null;
            let offered: string[] = [];
            let notice = "";
            try {
                const block = await openSettings( lookupPage );
                eventId = await block.findEventId( eventName );
                if ( !eventId ) {
                    offered = ( await block.eventDropDown.locator( "option" ).allTextContents() ).map( text => text.trim() );
                    notice = ( await block.createNotice.allTextContents() ).join( " " ).replace( /\s+/g, " " ).trim();
                }
            }
            finally {
                await lookupPage.close();
            }
            if ( !eventId ) {
                setupProblems.push( `One-time setup: no Eventbrite event named '${eventName}' is available to link. In the Eventbrite organization set on the site (Admin Tools > Installed Plugins > Eventbrite Settings), ` +
                    "create an event with that name and a free ticket, publish it, and register at least one attendee (an address you control). " +
                    "If it exists, it is probably linked to a Rock group already: unlink it there (Eventbrite Settings > Linked Groups). Set EVENTBRITE_TEST_EVENT_NAME in .env to use another event name. " +
                    `Events the settings block offers (${offered.length}): ${offered.slice( 0, 25 ).join( " | " ) || "(none)"}.${notice ? ` Block notice: ${notice}` : ""}` );
            }
            if ( setupProblems.length > 0 ) {
                throw new Error( setupProblems.map( ( problem, index ) => `(${index + 1}) ${problem}` ).join( "\n" ) );
            }

            const fixture: EventbriteSyncFixture = {
                api,
                rockVersion,
                tester,
                event: { id: eventId!, name: eventName },
                groupTypeId,
                parentGroupId,
                eventAttributeId,
                personAttributeId,
                settingsPage,
                viewerPage,
                openSettings,
                openGroupViewer: async ( page, groupId ) => {
                    await openPageAsPerson( page, api, { pageId: viewerPage.pageId, personId: tester.id, parameters: { GroupId: groupId } } );
                    return new GroupViewerPage( page, { sync: viewerPage.blockId, groupDetail: viewerPage.groupDetailBlockId, memberList: viewerPage.memberListBlockId } );
                },
                createGroup: async ( { linked } ) => {
                    const groupId = await api.post( "/api/Groups", {
                        Guid: randomUUID(),
                        Name: `${TestNamePrefix} EB Group ${Date.now()}`,
                        GroupTypeId: groupTypeId,
                        ParentGroupId: parentGroupId,
                        IsActive: true,
                        IsPublic: false,
                        IsSystem: false,
                        IsSecurityRole: false,
                        Order: 0
                    } );
                    if ( linked ) {
                        await api.setAttributeValue( eventAttributeId, groupId, eventId! );
                    }
                    return groupId;
                },
                getEventValue: async groupId => await api.getAttributeValue( eventAttributeId, groupId ) ?? "",
                getChildGroups: async () => await api.query( "Groups", `ParentGroupId eq ${parentGroupId}` )
            };

            markSetupDone();
            await use( fixture );
        }
        finally {
            for ( const [ label, step ] of [
                [ "test groups", () => deleteChildGroups( api, parentGroupId ) ],
                ...created.pages.map( pageId => [ `page ${pageId}`, () => deleteBlockPage( api, { pageId } ) ] as const )
            ] as Array<readonly [ string, () => Promise<void> ]> ) {
                try {
                    await step();
                }
                catch ( error ) {
                    console.warn( `Cleanup of ${label} failed; the next run will retry it. ${( error as Error ).message}` );
                }
            }
            await api.dispose();
        }
    } ), { scope: "worker", timeout: 600_000 } ],

    // Runs before every test: removes the groups earlier tests created, which also unlinks the event from them.
    resetState: [ async ( { eb }, use ) => {
        test.info().annotations.push( { type: "rock-version", description: eb.rockVersion } );
        await deleteChildGroups( eb.api, eb.parentGroupId );
        await use();
    }, { auto: true } ]
} );

export { expect } from "@playwright/test";

/** The test group type: a Group "Eventbrite Event" attribute and a member "Eventbrite Person" attribute shown in the grid. */
async function ensureGroupType( api: RockApi ): Promise<{ groupTypeId: number; eventAttributeId: number; personAttributeId: number }> {
    let groupType = await api.getByGuid( "GroupTypes", TestGuid.groupType );
    if ( !groupType ) {
        await api.post( "/api/GroupTypes", {
            Guid: TestGuid.groupType,
            Name: names.groupType,
            Description: "Group type for the KFS end-to-end tests of Eventbrite Sync. Do not use.",
            GroupTerm: "Group",
            GroupMemberTerm: "Attendee",
            ShowInGroupList: true,
            ShowInNavigation: true,
            TakesAttendance: true,
            IsSystem: false,
            Order: 0
        } );
        groupType = await api.getByGuid( "GroupTypes", TestGuid.groupType );
    }
    const groupTypeId = groupType!.Id;

    let role = await api.first( "GroupTypeRoles", `GroupTypeId eq ${groupTypeId}`, "&$orderby=Order" );
    if ( !role ) {
        await api.post( "/api/GroupTypeRoles", { Guid: randomUUID(), GroupTypeId: groupTypeId, Name: "Attendee", Order: 0, IsLeader: false, IsSystem: false } );
        role = await api.first( "GroupTypeRoles", `GroupTypeId eq ${groupTypeId}`, "&$orderby=Order" );
    }
    if ( groupType!.DefaultGroupRoleId !== role!.Id ) {
        await api.patch( "GroupTypes", groupTypeId, { DefaultGroupRoleId: role!.Id } );
    }

    const ensureAttribute = async ( args: { guid: string; entityTypeName: string; fieldTypeGuid: string; key: string; name: string; isGridColumn: boolean } ): Promise<number> => {
        // An empty (not null) default, as Rock's attribute editor saves it. The plugin treats any
        // value other than "" as linked; a null default only arises from an attribute created by code.
        const existing = await api.getByGuid( "Attributes", args.guid );
        if ( existing ) {
            if ( existing.DefaultValue !== "" ) {
                await api.patch( "Attributes", existing.Id, { DefaultValue: "" } );
            }
            return existing.Id;
        }
        return await api.post( "/api/Attributes", {
            DefaultValue: "",
            Guid: args.guid,
            EntityTypeId: await api.getEntityTypeId( args.entityTypeName ),
            EntityTypeQualifierColumn: "GroupTypeId",
            EntityTypeQualifierValue: String( groupTypeId ),
            FieldTypeId: await api.getIdByGuid( "FieldTypes", args.fieldTypeGuid ),
            Key: args.key,
            Name: args.name,
            Description: "KFS end-to-end tests. Do not use.",
            IsGridColumn: args.isGridColumn,
            IsMultiValue: false,
            IsRequired: false,
            IsSystem: false,
            Order: 0
        } );
    };

    return {
        groupTypeId,
        eventAttributeId: await ensureAttribute( { guid: TestGuid.eventAttribute, entityTypeName: EntityTypeName.Group, fieldTypeGuid: PluginGuid.eventFieldType, key: "KFSE2EEventbriteEvent", name: "Eventbrite Event", isGridColumn: false } ),
        personAttributeId: await ensureAttribute( { guid: TestGuid.personAttribute, entityTypeName: EntityTypeName.GroupMember, fieldTypeGuid: PluginGuid.personFieldType, key: "KFSE2EEventbritePerson", name: "Eventbrite Person", isGridColumn: true } )
    };
}

/** The persistent parent group new test groups are created under (also the settings block's New Group Parent). */
async function ensureParentGroup( api: RockApi, groupTypeId: number ): Promise<number> {
    const existing = await api.getByGuid( "Groups", TestGuid.parentGroup );
    if ( existing ) {
        return existing.Id;
    }

    return await api.post( "/api/Groups", {
        Guid: TestGuid.parentGroup,
        Name: names.parentGroup,
        Description: "Parent of the groups the KFS Eventbrite end-to-end tests create. Never linked to an event itself.",
        GroupTypeId: groupTypeId,
        IsActive: true,
        IsPublic: false,
        IsSystem: false,
        IsSecurityRole: false,
        Order: 0
    } );
}

/**
 * The settings block only creates groups of the New Group Type under a parent whose group
 * type lists it as a child group type. Rock's REST API cannot set that list.
 */
async function verifyChildGroupType( api: RockApi, groupTypeId: number ): Promise<void> {
    let childIds: number[] | null = null;
    try {
        const [ groupType ] = await api.get<Array<{ ChildGroupTypes?: Array<{ Id: number }> }>>( `/api/GroupTypes?$filter=Id eq ${groupTypeId}&$expand=ChildGroupTypes` );
        childIds = ( groupType?.ChildGroupTypes ?? [] ).map( child => child.Id );
    }
    catch ( error ) {
        console.warn( `Could not read the child group types of '${names.groupType}' (${( error as Error ).message}); the Create New Rock Group test will show the block's own error if it is missing.` );
        return;
    }

    if ( !childIds.includes( groupTypeId ) ) {
        throw new Error( `One-time setup: the '${names.groupType}' group type must allow itself as a child group type. In Rock, Admin Tools > Settings > Group Types > ${names.groupType} > Edit: under Child Group Types add '${names.groupType}', and save.` );
    }
}

/** Deletes the test groups under the parent, with their members and the attendance occurrences a sync adds. */
async function deleteChildGroups( api: RockApi, parentGroupId: number ): Promise<void> {
    for ( const group of await api.query( "Groups", `ParentGroupId eq ${parentGroupId}` ) ) {
        for ( const occurrence of await api.query( "AttendanceOccurrences", `GroupId eq ${group.Id}` ) ) {
            for ( const attendance of await api.query( "Attendances", `OccurrenceId eq ${occurrence.Id}` ) ) {
                await api.delete( "Attendances", attendance.Id );
            }
            await api.delete( "AttendanceOccurrences", occurrence.Id );
        }
        await deleteGroup( api, group.Id );
    }
}
