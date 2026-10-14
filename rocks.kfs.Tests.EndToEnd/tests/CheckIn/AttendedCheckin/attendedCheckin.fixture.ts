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
import { RockApi } from "../../../shared/rockApi";
import { CheckinFamily, CheckinGuid, CheckinSetup, createRunLastName, createRunPerson, deleteGroupAttendance, ensureCheckinSetup, ensureMobilePhone, inactivateLeftoverRunPeople, inactivateRunPeople, refreshCheckinCaches, verifyCheckinLinks } from "../../../shared/testData/checkin";
import { getBlockAttributeIds, getBlockTypeId } from "../../../shared/testData/cms";
import { TestPerson, ensureFamilyMember, ensurePerson } from "../../../shared/testData/people";
import { AttendedCheckinPages, AttendedCheckinRoute } from "./attendedCheckinPages";

export const adminBlockType = { name: "Check-in Administration", category: "Check-in > Attended" };

/** Everything the Attended Check-in tests need. Built once per run; the check-in configuration persists between runs. */
export type AttendedCheckinFixture = {
    api: RockApi;
    rockVersion: string;
    setup: CheckinSetup;

    /** Persistent test family, found by the search tests: adult (with a mobile number) and child. */
    familyAdult: TestPerson;
    familyChild: TestPerson;

    /**
     * Unique last name for this run. Teardown inactivates everyone whose last name
     * starts with it (REST cannot delete people).
     */
    runLastName: string;

    /**
     * Creates an adult in a new family named for this run plus `tag` (one or more
     * letters), so each test that adds people has a family of its own to search for.
     */
    createRunFamily( tag: string ): Promise<TestPerson>;
};

export const test = base.extend<{ checkinPages: AttendedCheckinPages; resetState: void }, { checkin: AttendedCheckinFixture }>( {
    checkin: [ async ( {}, use ) => guardFixtureSetup( "Attended Check-in", async markSetupDone => {
        const settings = getTestSettings();
        const api = await RockApi.create();
        const rockVersion = await api.getRockVersion();
        console.log( `Rock version under test: ${rockVersion} at ${settings.baseUrl}` );

        const runLastName = createRunLastName();

        try {
            await inactivateLeftoverRunPeople( api );

            // Create the entities before any check that can stop the run, so on a new site
            // the SQL link script has something to link after the first run.
            const setup = await ensureCheckinSetup( api );
            await verifyAdminAllowsManualSetup( api );
            await verifyCheckinLinks( api, setup );
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
            await ensureMobilePhone( api, familyAdult.id, CheckinFamily.mobilePhone );

            const createRunFamily = async ( tag: string ): Promise<TestPerson> => {
                if ( !/^[a-z]+$/.test( tag ) ) {
                    throw new Error( `Run family tag '${tag}' must be lower-case letters (a numeric search is a phone search).` );
                }

                return await createRunPerson( api, { firstName: "Rhea", lastName: `${runLastName}${tag}` } );
            };

            markSetupDone();
            await use( { api, rockVersion, setup, familyAdult, familyChild, runLastName, createRunFamily } );

            await deleteGroupAttendance( api, setup.groupId );
        }
        finally {
            try {
                await inactivateRunPeople( api, runLastName );
            }
            catch ( error ) {
                console.warn( `Could not inactivate this run's people (${runLastName}); the next run will retry. ${( error as Error ).message}` );
            }

            await api.dispose();
        }
    } ), { scope: "worker", timeout: 600_000 } ],

    /*
        Runs before every test. Each test starts from a fresh browser session (no kiosk
        configured) and with no attendance in the test group, so no one shows as already
        checked in from an earlier test.
    */
    resetState: [ async ( { checkin }, use ) => {
        test.info().annotations.push( { type: "rock-version", description: checkin.rockVersion } );

        await deleteGroupAttendance( checkin.api, checkin.setup.groupId );

        await use();
    }, { auto: true } ],

    checkinPages: async ( { page }, use ) => {
        await use( new AttendedCheckinPages( page ) );
    }
} );

export { expect } from "@playwright/test";

/*
    10/1/2026 - CLAUDE

    Without "Allow Manual Setup" the Admin block only accepts a kiosk whose IP
    address or host name matches a Device, so a test browser can never get past
    it. The fixture checks the setting and stops with a message rather than
    changing it, since it is a site-wide security choice.

    Reason: The tests must not change the site's check-in security settings.
*/
async function verifyAdminAllowsManualSetup( api: RockApi ): Promise<void> {
    const blockTypeId = await getBlockTypeId( api, adminBlockType.name, adminBlockType.category );

    const routePath = AttendedCheckinRoute.admin.replace( /^\//, "" );
    const route = await api.first( "PageRoutes", `Route eq '${routePath}'` );
    if ( !route ) {
        throw new Error( `No page has the route '${routePath}'. Is Attended Check-in installed, or did the site change its routes? (See AttendedCheckinRoute in attendedCheckinPages.ts.)` );
    }

    const block = await api.first( "Blocks", `PageId eq ${route.PageId} and BlockTypeId eq ${blockTypeId}` );
    if ( !block ) {
        throw new Error( `The page at '${routePath}' has no ${adminBlockType.name} block.` );
    }

    const attributeId = ( await getBlockAttributeIds( api, blockTypeId ) )[ "AllowManualSetup" ];
    if ( !attributeId ) {
        throw new Error( `${adminBlockType.name} has no 'AllowManualSetup' setting. The block's settings changed; update the tests.` );
    }

    const value = await api.getAttributeValue( attributeId, block.Id )
        ?? ( await api.getById( "Attributes", attributeId ) ).DefaultValue as string | null;
    if ( ( value ?? "" ).toLowerCase() !== "true" ) {
        throw new Error( `Turn on "Allow Manual Setup" in the ${adminBlockType.name} block settings on '${routePath}' (block ${block.Id}). The tests pick the test kiosk by hand, which that setting allows.` );
    }
}
