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
import { CheckinFamily, CheckinGuid, CheckinSetup, createCurrentAttendance, deleteGroupAttendance, ensureCheckinSetup, ensureRoomUnderDefaultCampus, refreshCheckinCaches, verifyCheckinLinks } from "../../../shared/testData/checkin";
import { CopiedPage, copyBlockSettings, createPageCopy, deleteBlockPage, deleteLeftoverTestPages, getBlockAttributeIds, getBlockTypeId, setBlockSettings } from "../../../shared/testData/cms";
import { TestPerson, ensureFamilyMember, ensurePerson } from "../../../shared/testData/people";
import { verifyRoleAccess } from "../../../shared/testData/security";
import { CheckinManagerLocationsBlock } from "./checkinManagerLocationsBlock";

export const blockType = { name: "Locations", category: "KFS > Check-in > Manager" };

/** Rock's Check-in Manager "Live Metrics" page and the core block on it that the plugin replaces. */
export const CoreLiveMetrics = {
    pageGuid: "04F70D50-5D27-4C12-A76D-B25E6E4CB177",
    blockTypeGuid: "A14D43A7-46EE-493E-9993-F89B86DF1604"
} as const;

const names = {
    page: "Advanced Check-in Manager"
};

/** Everything the Advanced Check-in Manager tests need, built once per run. */
export type CheckinManagerFixture = {
    api: RockApi;
    rockVersion: string;
    setup: CheckinSetup;
    campus: RockEntity;

    /** Copy of Rock's Live Metrics page with the KFS Locations block in place of the core one. */
    managerPage: CopiedPage;
    /** Setting keys copied from the core Live Metrics block to the KFS block. */
    copiedSettingKeys: string[];

    /** Staff member who opens the manager page. Has View on the test page. */
    manager: TestPerson;
    /** Persistent check-in test family; the child is checked in before every test. */
    familyAdult: TestPerson;
    familyChild: TestPerson;

    /** Resets the block to the baseline configuration, then applies the overrides. */
    configure( overrides?: Record<string, string> ): Promise<void>;
};

export const test = base.extend<{ block: CheckinManagerLocationsBlock; resetState: number }, { checkinManager: CheckinManagerFixture }>( {
    checkinManager: [ async ( { browser }, use ) => guardFixtureSetup( "Advanced Check-in Manager", async markSetupDone => {
        const settings = getTestSettings();
        const api = await RockApi.create();
        const rockVersion = await api.getRockVersion();
        console.log( `Rock version under test: ${rockVersion} at ${settings.baseUrl}` );

        await deleteLeftoverTestPages( api, names.page );

        let managerPage: CopiedPage | undefined;
        let setup: CheckinSetup | undefined;

        try {
            setup = await ensureCheckinSetup( api );
            await verifyCheckinLinks( api, setup );
            const campus = await ensureRoomUnderDefaultCampus( api, setup );
            await refreshCheckinCaches( api, setup );
            await deleteGroupAttendance( api, setup.groupId );

            const familyAdult = await ensurePerson( api, {
                guid: CheckinGuid.familyAdult,
                firstName: CheckinFamily.adultFirstName,
                lastName: CheckinFamily.lastName,
                email: "checkin.adult@kfs-e2e.invalid"
            } );
            const familyChild = await ensureFamilyMember( api, familyAdult.familyId, {
                guid: CheckinGuid.familyChild,
                firstName: CheckinFamily.childFirstName,
                lastName: CheckinFamily.lastName,
                email: "checkin.child@kfs-e2e.invalid"
            } );
            const manager = await ensurePerson( api, {
                guid: "C4EC0E2E-E2E0-4C0E-8C0E-0000000000C1",
                firstName: "Manny",
                lastName: "KFS E2E Tester",
                email: "checkin.manager@kfs-e2e.invalid"
            } );
            await verifyManagerCanViewCheckinManagerSite( api, manager );

            const kfsBlockTypeId = await getBlockTypeId( api, blockType.name, blockType.category );
            managerPage = await createPageCopy( api, {
                sourcePageGuid: CoreLiveMetrics.pageGuid,
                name: names.page,
                swapBlockTypeGuid: CoreLiveMetrics.blockTypeGuid,
                blockTypeId: kfsBlockTypeId
            } );
            const page = managerPage;

            // Rock registers a block type's settings the first time it loads it.
            if ( Object.keys( page.attributeIds ).length === 0 ) {
                const warmUpPage = await browser.newPage( { baseURL: settings.baseUrl } );
                await openPageAsPerson( warmUpPage, api, { pageId: page.pageId, personId: manager.id } );
                await warmUpPage.close();
                page.attributeIds = await getBlockAttributeIds( api, kfsBlockTypeId );
            }

            // What an admin does when swapping the block: carry the core block's settings over.
            const copiedSettingKeys = await copyBlockSettings( api, {
                fromBlockId: page.sourceBlockId,
                fromBlockTypeId: page.sourceBlockTypeId,
                toBlockId: page.blockId,
                toBlockTypeId: kfsBlockTypeId
            } );

            const checkinTypeGuid = CheckinGuid.checkinType.toLowerCase();
            const baseline = (): Record<string, string> => ( {
                Mode: "T",
                GroupTypeTemplate: checkinTypeGuid,
                SearchByCode: "False",
                LookbackMinutes: "120",
                LocationActive: "False",
                CloseOccurrence: "False",
                ShowDelete: "True",
                ShowCheckout: "True",
                ShowMove: "True",
                IncludeGroupMove: "False",
                ShowPrintLabel: "True",
                ShowAdvancedPrintOptions: "False"
            } );

            markSetupDone();
            await use( {
                api,
                rockVersion,
                setup,
                campus,
                managerPage: page,
                copiedSettingKeys,
                manager,
                familyAdult,
                familyChild,
                configure: async ( overrides = {} ) => {
                    await setBlockSettings( api, page, { ...baseline(), ...overrides } );
                }
            } );
        }
        finally {
            for ( const [ label, step ] of [
                [ "manager page", async () => managerPage && deleteBlockPage( api, managerPage ) ],
                [ "attendance", async () => setup && deleteGroupAttendance( api, setup.groupId ) ]
            ] as Array<[ string, () => Promise<unknown> ]> ) {
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

    /*
        Runs before every test: baseline block settings, and exactly one person checked
        in (the test child, in the test room, now). The value is that attendance Id.
    */
    resetState: [ async ( { checkinManager }, use ) => {
        const { api, setup } = checkinManager;
        test.info().annotations.push( { type: "rock-version", description: checkinManager.rockVersion } );

        await deleteGroupAttendance( api, setup.groupId );
        await checkinManager.configure();
        const attendanceId = await createCurrentAttendance( api, setup, checkinManager.familyChild );

        await use( attendanceId );
    }, { auto: true } ],

    block: async ( { page, checkinManager }, use ) => {
        await use( new CheckinManagerLocationsBlock( page, checkinManager.managerPage.blockId ) );
    }
} );

export { expect } from "@playwright/test";

/** Opens the copied manager page as the test staff member. */
export async function openManagerPage( page: import( "@playwright/test" ).Page, checkinManager: CheckinManagerFixture ): Promise<void> {
    await openPageAsPerson( page, checkinManager.api, { pageId: checkinManager.managerPage.pageId, personId: checkinManager.manager.id } );
}

/** Checks the one-time setup that lets the staff test person open Check-in Manager pages. */
async function verifyManagerCanViewCheckinManagerSite( api: RockApi, manager: TestPerson ): Promise<void> {
    const page = await api.getByGuid( "Pages", CoreLiveMetrics.pageGuid );
    if ( !page ) {
        throw new Error( `Rock's Check-in Manager Live Metrics page (${CoreLiveMetrics.pageGuid}) is missing on the site under test.` );
    }
    const layout = await api.getById( "Layouts", page.LayoutId as number );

    // The copied page has no rules of its own; it inherits the Check-in Manager site's.
    await verifyRoleAccess( api, {
        person: manager,
        entityTypeName: "Rock.Model.Site",
        entityId: layout.SiteId as number,
        entityLabel: "the Rock Check-in Manager site",
        action: "View",
        suggestedRole: "KFS E2E Plugin Testers"
    } );
}
