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
import { openPageAsPerson } from "../../../shared/rockBrowser";
import { EntityTypeName, SystemGuid } from "../../../shared/systemGuids";
import { TestBlockPage, createBlockPage, deleteBlockPage, deleteLeftoverTestPages, ensureTestPagesParent, getBlockAttributeIds, getBlockTypeId, setBlockSettings } from "../../../shared/testData/cms";
import { createGroupMemberContribution, deleteLeftoverTransactions, deleteTransaction } from "../../../shared/testData/finance";
import { GroupMemberStatus, TestGroup, addGroupMember, createGroup, deleteGroup, deleteLeftoverGroups } from "../../../shared/testData/groups";
import { TestPerson, ensurePerson } from "../../../shared/testData/people";
import { verifyTokenSignInAllowed } from "../../../shared/testData/security";
import { FundraisingProgressBlock } from "./fundraisingProgressBlock";

export const blockType = { name: "Fundraising Progress", category: "KFS > Fundraising" };

const names = {
    page: "AFP Test Page",
    group: "AFP Trip",
    otherGroup: "AFP Other Trip"
};

/** A participant's goal and gifts, and what the block should show for them. */
export type Participant = {
    person: TestPerson;
    groupMemberId: number;
    /** Goal set on the member; null means the group's default goal applies. */
    memberGoal: number | null;
    gifts: number[];
    expected: { goal: string; raised: string; percent: string; barClass: string };
};

/** Everything the Fundraising Progress tests need, built once per run and torn down after. */
export type FundraisingProgressFixture = {
    api: RockApi;
    rockVersion: string;
    blockPage: TestBlockPage;

    /** Staff member who opens the page. */
    tester: TestPerson;

    /** Fundraising group with a default goal of 1,000 and two participants with gifts. */
    group: TestGroup;
    groupName: string;
    /** Second fundraising group with no participants, for the URL-overrides-setting test. */
    otherGroup: TestGroup;
    otherGroupName: string;

    /** Sorted the way the block sorts them (last name, then nick name). */
    participants: Participant[];

    /** Group totals as the block formats them. */
    expectedTotals: { raised: string; goal: string; percent: RegExp; barClass: string };

    /**
     * Null when the installed block has the "Show Specific Group" setting; otherwise why not.
     * RockShop package v1.0 predates the setting (added to the source in June 2020).
     */
    showSpecificGroupMissing: string | null;

    /** Resets the block to the baseline configuration, then applies the overrides. */
    configure( overrides?: Record<string, string> ): Promise<void>;

    /** Opens the test page as the tester. */
    open( page: Page, parameters?: Record<string, string | number> ): Promise<void>;
};

export const test = base.extend<{ block: FundraisingProgressBlock; resetState: void }, { afp: FundraisingProgressFixture }>( {
    afp: [ async ( { browser }, use ) => guardFixtureSetup( "Fundraising Progress", async markSetupDone => {
        const settings = getTestSettings();
        const api = await RockApi.create();
        const rockVersion = await api.getRockVersion();
        console.log( `Rock version under test: ${rockVersion} at ${settings.baseUrl}` );

        // Clear leftovers from an earlier run that was killed before cleanup.
        await deleteLeftoverTestPages( api, names.page );
        await deleteLeftoverTransactions( api );
        await deleteLeftoverGroups( api, names.group );
        await deleteLeftoverGroups( api, names.otherGroup );

        // Same person as the other suites' tester: View on the test pages comes from the site.
        const tester = await ensurePerson( api, {
            guid: "6B0A9E43-2C5A-4E0B-9D0C-6F1F2B8A7E01",
            firstName: "Pafa",
            lastName: "KFS E2E Tester",
            email: "pafa.adult@kfs-e2e.invalid"
        } );
        await verifyTokenSignInAllowed( api, tester );

        const participantA = await ensurePerson( api, {
            guid: "8D2C1A65-4E7C-4A2D-9F2E-8B3B4DAC9A21",
            firstName: "Fundtesta",
            lastName: "KFS E2E Tester",
            email: "afp.a@kfs-e2e.invalid"
        } );
        const participantB = await ensurePerson( api, {
            guid: "8D2C1A65-4E7C-4A2D-9F2E-8B3B4DAC9A22",
            firstName: "Fundtestb",
            lastName: "KFS E2E Tester",
            email: "afp.b@kfs-e2e.invalid"
        } );

        // The fundraising goal attributes Rock defines on the Fundraising Opportunity group type.
        const groupTypeId = await api.getIdByGuid( "GroupTypes", SystemGuid.GroupType.FundraisingOpportunity );
        const goalAttributeId = async ( entityTypeName: string ): Promise<number> => ( await api.single( "Attributes",
            `EntityTypeId eq ${await api.getEntityTypeId( entityTypeName )} and EntityTypeQualifierColumn eq 'GroupTypeId' and EntityTypeQualifierValue eq '${groupTypeId}' and Key eq 'IndividualFundraisingGoal'` ) ).Id;
        const groupGoalAttributeId = await goalAttributeId( EntityTypeName.Group );
        const memberGoalAttributeId = await goalAttributeId( EntityTypeName.GroupMember );

        const account = await api.first( "FinancialAccounts", "IsActive eq true", "&$orderby=Id" );
        if ( !account ) {
            throw new Error( "The site has no active financial account to record test contributions to." );
        }

        const created: { blockPage?: TestBlockPage; groups: TestGroup[]; transactionIds: number[] } = { groups: [], transactionIds: [] };

        try {
            const group = await createGroup( api, { name: names.group, groupTypeGuid: SystemGuid.GroupType.FundraisingOpportunity } );
            created.groups.push( group );
            const otherGroup = await createGroup( api, { name: names.otherGroup, groupTypeGuid: SystemGuid.GroupType.FundraisingOpportunity } );
            created.groups.push( otherGroup );
            await api.setAttributeValue( groupGoalAttributeId, group.id, "1000" );

            /*
                Participant A: own goal 500, two gifts totalling 200 -> 40%, "warning" (the block
                shows "info" only above 40). Participant B: group default goal 1,000, one gift of
                1,000 -> 100%, "success". Group: 1,200 of 1,500 -> 80%, "info".
            */
            const plans = [
                { person: participantA, memberGoal: 500, gifts: [ 125.5, 74.5 ], expected: { goal: "500", raised: "200", percent: "40", barClass: "progress-bar-warning" } },
                { person: participantB, memberGoal: null, gifts: [ 1000 ], expected: { goal: "1000", raised: "1000", percent: "100", barClass: "progress-bar-success" } }
            ];

            const participants: Participant[] = [];
            for ( const plan of plans ) {
                const groupMemberId = await addGroupMember( api, { groupId: group.id, personId: plan.person.id, status: GroupMemberStatus.Active } );
                if ( plan.memberGoal !== null ) {
                    await api.setAttributeValue( memberGoalAttributeId, groupMemberId, plan.memberGoal.toString() );
                }
                for ( const amount of plan.gifts ) {
                    created.transactionIds.push( await createGroupMemberContribution( api, { groupMemberId, authorizedPersonAliasId: tester.primaryAliasId, accountId: account.Id, amount } ) );
                }
                participants.push( { ...plan, groupMemberId } );
            }

            const testPagesParentId = await ensureTestPagesParent( api, settings.parentPageId );
            const blockTypeId = await getBlockTypeId( api, blockType.name, blockType.category );
            const blockPage = await createBlockPage( api, { name: names.page, parentPageId: testPagesParentId, blockTypeId, zone: settings.zone } );
            created.blockPage = blockPage;

            const open = async ( page: Page, parameters?: Record<string, string | number> ): Promise<void> => {
                await openPageAsPerson( page, api, { pageId: blockPage.pageId, personId: tester.id, parameters } );
            };

            // Rock registers a block type's settings when it first loads the block file, including
            // after a new version is deployed, so always load the page once before reading them.
            const warmUpPage = await browser.newPage( { baseURL: settings.baseUrl } );
            await open( warmUpPage );
            await warmUpPage.close();
            blockPage.attributeIds = await getBlockAttributeIds( api, blockTypeId );

            const showSpecificGroupMissing = blockPage.attributeIds[ "ShowSpecificGroup" ]
                ? null
                : "The installed Fundraising Progress block has no 'Show Specific Group' setting, so it is older than the source in KFSRockBlocks. " +
                  "RockShop package 'KFS Fundraising Progress' v1.0 predates the setting (added to the source 2020-06-09); deploy the current Fundraising/FundraisingProgress.ascx and .ascx.cs to test it.";

            // Matches the block on the KFS beta site's Fundraising page (page 486): everything shown.
            const baseline = (): Record<string, string> => ( {
                ShowGroupTitle: "True",
                ShowTotalAmountRaised: "True",
                ShowGroupTotalGoals: "True",
                ShowGroupTotalGoalsAmount: "True",
                ShowGroupTotalGoalsProgressBar: "True",
                ShowGroupMemberGoals: "True",
                ShowGroupMemberGoalAmounts: "True",
                ShowGroupMemberGoalProgressBars: "True",
                ShowExcelExportButton: "True",
                ...( showSpecificGroupMissing ? {} : { ShowSpecificGroup: "" } )
            } );

            const fixture: FundraisingProgressFixture = {
                api,
                rockVersion,
                blockPage,
                tester,
                group,
                groupName: String( ( await api.getById( "Groups", group.id ) ).Name ),
                otherGroup,
                otherGroupName: String( ( await api.getById( "Groups", otherGroup.id ) ).Name ),
                participants,
                showSpecificGroupMissing,
                expectedTotals: { raised: "1,200.00", goal: "1,500.00", percent: /^\s*80(\.0+)?% Complete\s*$/, barClass: "progress-bar-info" },
                configure: async ( overrides = {} ) => {
                    await setBlockSettings( api, blockPage, { ...baseline(), ...overrides } );
                },
                open
            };

            markSetupDone();
            await use( fixture );
        }
        finally {
            await cleanUp( api, created );
            await api.dispose();
        }
    } ), { scope: "worker", timeout: 600_000 } ],

    // Runs before every test, so each test starts from the baseline settings.
    resetState: [ async ( { afp }, use ) => {
        test.info().annotations.push( { type: "rock-version", description: afp.rockVersion } );
        await afp.configure();

        await use();
    }, { auto: true } ],

    block: async ( { page, afp }, use ) => {
        await use( new FundraisingProgressBlock( page, afp.blockPage.blockId ) );
    }
} );

export { expect } from "@playwright/test";

/** Best-effort teardown: keeps going past failures so one stuck record does not strand the rest. */
async function cleanUp( api: RockApi, created: { blockPage?: TestBlockPage; groups: TestGroup[]; transactionIds: number[] } ): Promise<void> {
    const steps: Array<[ string, () => Promise<void> ]> = [];

    if ( created.blockPage ) {
        const blockPage = created.blockPage;
        steps.push( [ "test page", () => deleteBlockPage( api, blockPage ) ] );
    }
    for ( const transactionId of created.transactionIds ) {
        steps.push( [ `transaction ${transactionId}`, () => deleteTransaction( api, transactionId ) ] );
    }
    for ( const group of created.groups ) {
        steps.push( [ `group ${group.id}`, () => deleteGroup( api, group.id ) ] );
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
