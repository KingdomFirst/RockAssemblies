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
import { getTestSettings } from "../../../shared/config";
import { guardFixtureSetup } from "../../../shared/runGuard";
import { RockApi } from "../../../shared/rockApi";
import { openPageAsPerson, waitForPostback } from "../../../shared/rockBrowser";
import { TestNamePrefix } from "../../../shared/systemGuids";
import { createPageCopy, ensureTestPagesParent, setBlockSettings, getBlockAttributeIds, TestBlockPage } from "../../../shared/testData/cms";
import { TestConnectionSetup, createConnectionSetup, deleteConnectionType, deleteLeftoverConnectionTypes } from "../../../shared/testData/connections";
import { TestPerson, ensureFamilyMember, ensurePerson } from "../../../shared/testData/people";
import { verifyRoleAccess } from "../../../shared/testData/security";
import { CareDashboardBlock, CareEntryBlock } from "./stepsToCarePages";

/** Guids the plugin installs (rocks.kfs.StepsToCare: SystemGuid, Migrations/004_CreatePages.cs). */
export const PluginGuid = {
    dashboardPage: "1F93E9AA-ECCA-42A2-8C91-73D991DBCD9F",
    entryPage: "27953B65-21E2-4CA9-8461-3AFAD46D9BC8",
    configurationPage: "39F72E9D-22B7-4F1E-8633-6C3745AC6F34",
    dashboardBlockType: "AF14CB6C-F915-4449-9CB7-7C44B624B051",
    entryBlockType: "4F0F9ED7-9F74-4152-B27F-D9B2A458AFBE",
    careWorkersBlockType: "FC3E03F3-80A0-42BF-AAFB-DD2095B7BE86",
    noteTemplatesBlockType: "561E0D77-12F9-4863-B5E3-4C5F36FB2DB1",
    categoryDefinedType: "4915FF6B-4E8E-40FF-B853-EFF6B643611B",
    statusDefinedType: "F965CB2C-23D0-42D6-8BAF-10F552249B7A",
    statusOpen: "811ECA2D-2B74-469A-9CFB-AB47B9643A02",
    statusSnoozed: "6A5890DE-D784-47A7-B83E-98EE70BFFA91",
    notificationAttribute: "330C665A-E348-40D8-8D56-01D7E2466CC5"
} as const;

/**
 * Fixed Guids for this suite's persistent test data. The pages are kept between runs because
 * the blocks' own security actions (Complete Needs, Update Status) can only be granted on the
 * block itself, by hand in Rock (rules saved through REST are ignored on Rock 17.9).
 */
const TestGuid = {
    dashboardPage: "9A0B4C33-E2E0-4C0E-8C0E-0000000000B1",
    entryPage: "9A0B4C33-E2E0-4C0E-8C0E-0000000000B2",
    configurationPage: "9A0B4C33-E2E0-4C0E-8C0E-0000000000B3",
    category: "9A0B4C33-E2E0-4C0E-8C0E-0000000000B4"
} as const;

export const names = {
    dashboardPage: "Care Dashboard",
    entryPage: "Care Entry",
    configurationPage: "Care Configuration",
    category: `${TestNamePrefix} Care Category`
};

/** A persistent copy of one of the plugin's pages and the plugin block on it. */
export type CarePage = TestBlockPage & { guid: string };

export type StepsToCareFixture = {
    api: RockApi;
    rockVersion: string;
    /** Staff member who enters needs and adds themself as the care worker. */
    tester: TestPerson;
    /** The person a need is entered for; has a child in the same family. */
    requestor: TestPerson;
    requestorChild: TestPerson;
    /** Added as a care worker on the Care Configuration page. */
    worker: TestPerson;
    category: { id: number; name: string };
    /** Connection type and opportunity the dashboard's "Add Connection Request" offers (created per run). */
    connection: TestConnectionSetup;
    pages: { dashboard: CarePage; entry: CarePage; configuration: CarePage & { careWorkersBlockId: number; noteTemplatesBlockId: number } };

    /** Opens the Care Configuration copy as the tester. */
    openConfiguration( page: Page ): Promise<void>;

    openDashboard( page: Page ): Promise<CareDashboardBlock>;
    openEntry( page: Page, parameters?: Record<string, string | number> ): Promise<CareEntryBlock>;
    /** Unique text for a test need's description, so the test can find its rows. */
    uniqueDetails( label: string ): string;

    /**
     * Enters a need through Care Entry the way a user does (there is no REST endpoint for
     * care needs): the requestor, the test category, the description, the tester as the
     * assigned worker, and optionally Include Family. Ends on the Care Dashboard.
     */
    enterNeed( page: Page, args: { details: string; includeFamily?: boolean } ): Promise<CareDashboardBlock>;

    /** Deletes every test need still on the dashboard (description starting "KFS E2E"), family needs first. */
    deleteTestNeeds( page: Page ): Promise<void>;
};

export const test = base.extend<{ resetState: void }, { stc: StepsToCareFixture }>( {
    stc: [ async ( { browser }, use ) => guardFixtureSetup( "Steps to Care", async markSetupDone => {
        const settings = getTestSettings();
        const api = await RockApi.create();
        const rockVersion = await api.getRockVersion();
        console.log( `Rock version under test: ${rockVersion} at ${settings.baseUrl}` );

        let connection: TestConnectionSetup | undefined;
        try {
            await deleteLeftoverConnectionTypes( api, "STC Connections" );
            connection = await createConnectionSetup( api, "STC Connections" );
            const connectionTypeGuid = String( ( await api.getById( "ConnectionTypes", connection.connectionTypeId ) ).Guid ).toLowerCase();

            const tester = await ensurePerson( api, {
                guid: "6B0A9E43-2C5A-4E0B-9D0C-6F1F2B8A7E01",
                firstName: "Pafa",
                lastName: "KFS E2E Tester",
                email: "pafa.adult@kfs-e2e.invalid"
            } );
            const requestor = await ensurePerson( api, {
                guid: "A1B2C3D4-6F70-4A81-9B92-0C1D2E3F4A51",
                firstName: "Stcparent",
                lastName: "KFS E2E Tester",
                email: "stc.parent@kfs-e2e.invalid"
            } );
            const requestorChild = await ensureFamilyMember( api, requestor.familyId, {
                guid: "A1B2C3D4-6F70-4A81-9B92-0C1D2E3F4A52",
                firstName: "Stckid",
                lastName: "KFS E2E Tester",
                email: "stc.kid@kfs-e2e.invalid"
            } );

            const worker = await ensurePerson( api, {
                guid: "A1B2C3D4-6F70-4A81-9B92-0C1D2E3F4A53",
                firstName: "Stcworker",
                lastName: "KFS E2E Tester",
                email: "stc.worker@kfs-e2e.invalid"
            } );

            const category = await ensureCategory( api );

            // Persistent copies of the three pages, nested as installed: Care Entry returns to its parent page after Save.
            const testPagesParentId = await ensureTestPagesParent( api, settings.parentPageId );
            const dashboard = await ensurePageCopy( api, { guid: TestGuid.dashboardPage, sourcePageGuid: PluginGuid.dashboardPage, name: names.dashboardPage, blockTypeGuid: PluginGuid.dashboardBlockType, parentPageId: testPagesParentId } );
            const entry = await ensurePageCopy( api, { guid: TestGuid.entryPage, sourcePageGuid: PluginGuid.entryPage, name: names.entryPage, blockTypeGuid: PluginGuid.entryBlockType, parentPageId: dashboard.pageId } );
            const configurationCopy = await ensurePageCopy( api, { guid: TestGuid.configurationPage, sourcePageGuid: PluginGuid.configurationPage, name: names.configurationPage, blockTypeGuid: PluginGuid.careWorkersBlockType, parentPageId: dashboard.pageId } );
            const configuration = {
                ...configurationCopy,
                careWorkersBlockId: configurationCopy.blockId,
                noteTemplatesBlockId: ( await api.single( "Blocks", `PageId eq ${configurationCopy.pageId} and BlockTypeId eq ${await api.getIdByGuid( "BlockTypes", PluginGuid.noteTemplatesBlockType )}` ) ).Id
            };

            // Rock registers a block type's settings when it first loads it.
            for ( const [ pageId, blockPage ] of [ [ dashboard.pageId, dashboard ], [ entry.pageId, entry ] ] as const ) {
                const warmUp = await browser.newPage( { baseURL: settings.baseUrl } );
                await openPageAsPerson( warmUp, api, { pageId, personId: tester.id } );
                await warmUp.close();
                blockPage.attributeIds = await getBlockAttributeIds( api, blockPage.blockTypeId );
            }

            // The installed settings (copied with the page), pointed at the test copies. Auto
            // assignment is off so no real care worker is assigned to (or notified about) a test need.
            await setBlockSettings( api, dashboard, {
                DetailPage: entry.guid.toLowerCase(),
                ConfigurationPage: configuration.guid.toLowerCase(),
                ConnectionRequestEnable: "True",
                IncludeConnectionTypes: connectionTypeGuid,
                WorkflowEnable: "True",
                QuickNoteAutoSave: "True"
            } );
            await setBlockSettings( api, entry, {
                AutoAssignWorker: "False",
                AutoAssignWorkerGeofence: "False",
                GroupTypeAndRole: "",
                EnableFamilyNeeds: "True",
                EnableCustomFollowUp: "True",
                PreviewAssignedPeople: "True"
            } );

            await verifySetup( api, tester, { dashboard, entry } );

            const fixture: StepsToCareFixture = {
                api,
                rockVersion,
                tester,
                requestor,
                requestorChild,
                worker,
                category,
                connection,
                pages: { dashboard, entry, configuration },
                openDashboard: async page => {
                    await openPageAsPerson( page, api, { pageId: dashboard.pageId, personId: tester.id, allPages: true } );
                    return new CareDashboardBlock( page, dashboard.blockId );
                },
                openConfiguration: async page => {
                    await openPageAsPerson( page, api, { pageId: configuration.pageId, personId: tester.id, allPages: true } );
                },
                openEntry: async ( page, parameters ) => {
                    await openPageAsPerson( page, api, { pageId: entry.pageId, personId: tester.id, parameters, allPages: true } );
                    return new CareEntryBlock( page, entry.blockId );
                },
                uniqueDetails: label => `KFS E2E ${label} ${Date.now()}`,
                enterNeed: async ( page, args ) => {
                    const entry = await fixture.openEntry( page );
                    await entry.pickRequestor( requestor.fullName, requestor.id );
                    await entry.chooseCategory( category.id );
                    await entry.details.fill( args.details );
                    await entry.assignPerson( tester.fullName, tester.id );
                    if ( args.includeFamily ) {
                        await entry.setIncludeFamily( true );
                    }
                    await entry.save();
                    return new CareDashboardBlock( page, dashboard.blockId );
                },
                deleteTestNeeds: page => deleteTestNeeds( page, fixture )
            };

            markSetupDone();
            await use( fixture );
        }
        finally {
            if ( connection ) {
                const connectionTypeId = connection.connectionTypeId;
                await deleteConnectionType( api, connectionTypeId ).catch( error => console.warn( `Cleanup of the test connection type failed; the next run will retry it. ${( error as Error ).message}` ) );
            }
            await api.dispose();
        }
    } ), { scope: "worker", timeout: 600_000 } ],

    /*
        Before and after every test, with the test's own page: delete test needs (an earlier
        test's, or a killed run's), so each test starts from the same dashboard. A separate
        browser page here collides with Playwright's trace recording for the test.
    */
    resetState: [ async ( { stc, page }, use ) => {
        test.info().annotations.push( { type: "rock-version", description: stc.rockVersion } );
        await stc.deleteTestNeeds( page );
        await use();
        await stc.deleteTestNeeds( page );
    }, { auto: true } ]
} );

export { expect } from "@playwright/test";

/** The test care category (a value of the plugin's Care Need Category defined type), kept between runs. */
async function ensureCategory( api: RockApi ): Promise<{ id: number; name: string }> {
    const existing = await api.getByGuid( "DefinedValues", TestGuid.category );
    if ( existing ) {
        return { id: existing.Id, name: String( existing.Value ) };
    }

    const definedTypeId = await api.getIdByGuid( "DefinedTypes", PluginGuid.categoryDefinedType );
    const id = await api.post( "/api/DefinedValues", {
        Guid: TestGuid.category,
        DefinedTypeId: definedTypeId,
        Value: names.category,
        Description: "Care category for the KFS end-to-end tests. Do not use.",
        IsActive: true,
        IsSystem: false,
        Order: 999
    } );
    return { id, name: names.category };
}

/** Finds the persistent page copy by Guid, or copies the installed page. Returns the plugin block on it. */
async function ensurePageCopy( api: RockApi, args: { guid: string; sourcePageGuid: string; name: string; blockTypeGuid: string; parentPageId: number } ): Promise<CarePage> {
    const blockTypeId = await api.getIdByGuid( "BlockTypes", args.blockTypeGuid );
    const existing = await api.getByGuid( "Pages", args.guid );
    if ( existing ) {
        const block = await api.single( "Blocks", `PageId eq ${existing.Id} and BlockTypeId eq ${blockTypeId}` );
        return { pageId: existing.Id, blockId: block.Id, blockTypeId, attributeIds: await getBlockAttributeIds( api, blockTypeId ), guid: args.guid };
    }

    const copy = await createPageCopy( api, { sourcePageGuid: args.sourcePageGuid, name: args.name, blockTypeId, parentPageId: args.parentPageId, pageGuid: args.guid } );
    return { ...copy, guid: args.guid };
}

/**
 * One-time setup that only Rock's UI can do on Rock 17.9 (block security). Every missing
 * step is collected and reported together.
 */
async function verifySetup( api: RockApi, tester: TestPerson, pages: { dashboard: CarePage; entry: CarePage } ): Promise<void> {
    const problems: string[] = [];
    const check = async ( blockPage: CarePage, pageName: string, blockName: string, action: string ): Promise<void> => {
        try {
            await verifyRoleAccess( api, {
                person: tester,
                entityTypeName: "Rock.Model.Block",
                entityId: blockPage.blockId,
                entityLabel: `the '${blockName}' block on page '${TestNamePrefix} ${pageName}' (page ${blockPage.pageId}; block configuration bar > Security)`,
                action,
                suggestedRole: "KFS E2E Plugin Testers"
            } );
        }
        catch ( error ) {
            problems.push( ( error as Error ).message );
        }
    };

    // Administrate on the dashboard page reaches every block on it and on its two child pages:
    // Care Entry only shows the Assigned panel ("add yourself as a Care Worker") to administrators,
    // and the dashboard only offers its Delete column to them (used to remove test needs).
    try {
        await verifyRoleAccess( api, {
            person: tester,
            entityTypeName: "Rock.Model.Page",
            entityId: pages.dashboard.pageId,
            entityLabel: `the page '${TestNamePrefix} ${names.dashboardPage}' (page ${pages.dashboard.pageId}, under '${TestNamePrefix} Test Pages')`,
            action: "Administrate",
            suggestedRole: "KFS E2E Plugin Testers"
        } );
    }
    catch ( error ) {
        problems.push( ( error as Error ).message );
    }
    await check( pages.dashboard, names.dashboardPage, "Care Dashboard", "CompleteNeeds" );
    // Care Entry gives family members' needs no workers; only View All shows them under the "+".
    await check( pages.dashboard, names.dashboardPage, "Care Dashboard", "ViewAll" );
    await check( pages.entry, names.entryPage, "Care Entry", "CompleteNeeds" );
    await check( pages.entry, names.entryPage, "Care Entry", "UpdateStatus" );

    if ( problems.length > 0 ) {
        throw new Error( problems.map( ( problem, index ) => `(${index + 1}) ${problem}` ).join( "\n" ) );
    }
}

/**
 * Deletes the test needs on the dashboard with its Delete column (shown to people with
 * Administrate). Family needs are deleted before their parent need, which references them.
 */
async function deleteTestNeeds( page: Page, stc: StepsToCareFixture ): Promise<void> {
    // A filter left by a stopped test (kept as the tester's preference) would hide test needs.
    const dashboard = await stc.openDashboard( page );
    await dashboard.clearFilter();
    const testRows = page.locator( `table[id$='_gList'] tbody tr, table[id$='_gFollowUp'] tbody tr` ).filter( { hasText: /KFS E2E \S+.* \d{13}/ } );

    for ( let pass = 0; pass < 50; pass++ ) {
        // Delete a family need before its parent need, which references it.
        const familyRow = testRows.and( page.locator( "tr[class*='hasParentNeed']" ) ).first();
        const target = await familyRow.count() > 0 ? familyRow : testRows.first();
        if ( await target.count() === 0 ) {
            return;
        }
        const deleteButton = target.locator( "a.grid-delete-button" );
        if ( await deleteButton.count() === 0 ) {
            throw new Error( "The Care Dashboard shows no Delete button; the test role needs Administrate on the test dashboard page." );
        }
        // A family need's row stays hidden unless its parent's "+" is open; cleanup presses the
        // button directly (it still asks for confirmation) rather than showing the row first.
        await deleteButton.evaluate( button => ( button as HTMLElement ).click() );
        await page.locator( ".bootbox .btn-primary, .modal-footer .btn-primary" ).filter( { visible: true } ).first().click();
        await waitForPostback( page );
        await page.waitForLoadState( "load" );
    }
}