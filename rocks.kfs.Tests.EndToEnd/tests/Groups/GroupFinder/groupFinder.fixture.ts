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
import { getTestSettings } from "../../../shared/config";
import { guardFixtureSetup } from "../../../shared/runGuard";
import { RockApi, RockEntity } from "../../../shared/rockApi";
import { openPageAsPerson } from "../../../shared/rockBrowser";
import { EntityTypeName, TestNamePrefix } from "../../../shared/systemGuids";
import { TestBlockPage, createBlockPage, deleteBlockPage, deleteLeftoverTestPages, ensureTestPagesParent, getBlockAttributeIds, getBlockTypeId, setBlockSettings } from "../../../shared/testData/cms";
import { GroupMemberStatus, addGroupMember, deleteGroup } from "../../../shared/testData/groups";
import { TestPerson, ensurePerson } from "../../../shared/testData/people";
import { GroupFinderBlock } from "./groupFinderBlock";

export const blockType = { name: "Group Finder KFS", category: "KFS > Groups" };

/** Fixed Guids for this suite's persistent test data. */
const TestGuid = {
    groupType: "8F9A3B22-E2E0-4C0E-8C0E-0000000000C1",
    topicAttribute: "8F9A3B22-E2E0-4C0E-8C0E-0000000000C2"
} as const;

const names = {
    groupType: `${TestNamePrefix} Group Finder`,
    page: "GF Test Page",
    groupPrefix: `${TestNamePrefix} GF`
};

/** Key of the test group type's "Topic" single-select attribute, used as an attribute filter. */
export const TopicKey = "KFSE2EGFTopic";
export const Topics = [ "Bible Study", "Prayer", "Service" ] as const;

/** System.DayOfWeek values, as the Day of Week filter's check boxes use them. */
export const DayOfWeek = { Monday: 1, Wednesday: 3 } as const;

/** CSS class each result carries in the suite's Lava template. */
export const resultCssClass = "kfs-e2e-gf-group";

export type FinderGroup = { id: number; name: string };

export type GroupFinderFixture = {
    api: RockApi;
    rockVersion: string;
    tester: TestPerson;
    blockPage: TestBlockPage;
    campuses: Array<{ id: number; name: string }>;
    topicAttributeId: number;
    topicFieldTypeId: number;

    /** Public, open groups the finder should list. */
    groups: {
        /** Monday, first campus, topic Bible Study. */
        mondayBibleStudy: FinderGroup;
        /** Wednesday, second campus, topic Prayer; "potluck" only in its description. */
        wednesdayPrayer: FinderGroup;
        /** Public but at capacity: hidden while Hide Overcapacity Groups is on. */
        full: FinderGroup;
        /** Not public: hidden unless Show All Groups is on. */
        private: FinderGroup;
    };

    /** Resets the block to the baseline configuration, then applies the overrides. */
    configure( overrides?: Record<string, string> ): Promise<void>;
    open( page: Page, parameters?: Record<string, string | number> ): Promise<GroupFinderBlock>;
};

export const test = base.extend<{ resetState: void }, { gf: GroupFinderFixture }>( {
    gf: [ async ( { browser }, use ) => guardFixtureSetup( "Group Finder", async markSetupDone => {
        const settings = getTestSettings();
        const api = await RockApi.create();
        const rockVersion = await api.getRockVersion();
        console.log( `Rock version under test: ${rockVersion} at ${settings.baseUrl}` );

        const tester = await ensurePerson( api, {
            guid: "6B0A9E43-2C5A-4E0B-9D0C-6F1F2B8A7E01",
            firstName: "Pafa",
            lastName: "KFS E2E Tester",
            email: "pafa.adult@kfs-e2e.invalid"
        } );

        const { groupTypeId, topicAttributeId } = await ensureGroupType( api );
        await deleteLeftovers( api, groupTypeId );
        await deleteLeftoverTestPages( api, names.page );

        const campuses = ( await api.query( "Campuses", "IsActive eq true", "&$orderby=Order" ) ).map( c => ( { id: c.Id as number, name: String( c.Name ) } ) );
        if ( campuses.length < 2 ) {
            throw new Error( `The Group Finder campus filter test needs at least two active campuses on the site; found ${campuses.length}.` );
        }

        const created: { blockPage?: TestBlockPage } = {};

        try {
            const createFinderGroup = async ( args: { label: string; description: string; campusId: number; day: number; topic: string; isPublic?: boolean; capacity?: number } ): Promise<FinderGroup> => {
                const name = `${names.groupPrefix} ${args.label}`;
                const scheduleId = await api.post( "/api/Schedules", {
                    Guid: randomUUID(),
                    Name: name,
                    Description: "KFS end-to-end tests.",
                    iCalendarContent: "",
                    WeeklyDayOfWeek: args.day,
                    WeeklyTimeOfDay: "19:00:00",
                    IsActive: true
                } );
                const id = await api.post( "/api/Groups", {
                    Guid: randomUUID(),
                    Name: name,
                    Description: args.description,
                    GroupTypeId: groupTypeId,
                    CampusId: args.campusId,
                    ScheduleId: scheduleId,
                    GroupCapacity: args.capacity ?? null,
                    IsActive: true,
                    IsPublic: args.isPublic ?? true,
                    IsSystem: false,
                    IsSecurityRole: false,
                    Order: 0
                } );
                await api.setAttributeValue( topicAttributeId, id, args.topic );
                return { id, name };
            };

            const groups = {
                mondayBibleStudy: await createFinderGroup( { label: "Monday Bible Study", description: "Verse by verse through the Gospels.", campusId: campuses[ 0 ].id, day: DayOfWeek.Monday, topic: "Bible Study" } ),
                wednesdayPrayer: await createFinderGroup( { label: "Wednesday Prayer", description: "Prayer for the church, with a potluck dinner.", campusId: campuses[ 1 ].id, day: DayOfWeek.Wednesday, topic: "Prayer" } ),
                full: await createFinderGroup( { label: "Full Group", description: "Has reached its capacity.", campusId: campuses[ 0 ].id, day: DayOfWeek.Monday, topic: "Service", capacity: 1 } ),
                private: await createFinderGroup( { label: "Private Group", description: "Not public.", campusId: campuses[ 0 ].id, day: DayOfWeek.Monday, topic: "Service", isPublic: false } )
            };
            await addGroupMember( api, { groupId: groups.full.id, personId: tester.id, status: GroupMemberStatus.Active } );

            const testPagesParentId = await ensureTestPagesParent( api, settings.parentPageId );
            const blockTypeId = await getBlockTypeId( api, blockType.name, blockType.category );
            const blockPage = await createBlockPage( api, { name: names.page, parentPageId: testPagesParentId, blockTypeId, zone: settings.zone } );
            created.blockPage = blockPage;

            const open = async ( page: Page, parameters?: Record<string, string | number> ): Promise<GroupFinderBlock> => {
                await openPageAsPerson( page, api, { pageId: blockPage.pageId, personId: tester.id, parameters } );
                return new GroupFinderBlock( page, blockPage.blockId );
            };

            // Rock registers a block type's settings when it first loads the block file.
            const warmUpPage = await browser.newPage( { baseURL: settings.baseUrl } );
            await open( warmUpPage );
            await warmUpPage.close();
            blockPage.attributeIds = await getBlockAttributeIds( api, blockTypeId );

            const topicAttributeGuid = TestGuid.topicAttribute.toLowerCase();

            // The settings of the block on the KFS beta site's page 800, aimed at the test group
            // type and its Topic attribute, with a Lava template that lists results plainly.
            const baseline = (): Record<string, string> => ( {
                GroupType: TestGuid.groupType.toLowerCase(),
                AttributeFilters: topicAttributeGuid,
                AutoFilterEnabled: "True",
                AutoLoad: "True",
                AllowSearchPersonGuid: "False",
                CampusLabel: "Campuses",
                CollapseFiltersonSearch: "False",
                DayOfWeekLabel: "Day of Week",
                DisplayCampusFilter: "True",
                DisplayKeywordSearch: "True",
                EnableCampusContext: "False",
                EnablePostalCodeSearch: "True",
                RequirePostalCode: "False",
                FilterLabel: "Filter",
                FilterOrder: `1^filter_dow|2^filter_time|3^filter_campus|4^filter_address|5^filter_postalcode|6^filter_keyword|7^filter_showfullgroups|8^${topicAttributeGuid}|`,
                HideOvercapacityGroups: "True",
                IncludePending: "True",
                KeywordLabel: "Keyword",
                LoadInitialResults: "False",
                MoreFiltersLabel: "More Filters",
                OvercapacityGroupsincludePending: "False",
                PostalCodeLabel: "Zip Code",
                ScheduleFilters: "Days",
                ShowAllGroups: "False",
                ShowFullGroupsLabel: "Show Full Groups",
                ShowGrid: "False",
                ShowMap: "False",
                ShowLavaOutput: "True",
                ShowProximity: "True",
                SingleSelectFilters: "False",
                LavaOutput: `<div class="kfs-e2e-gf-results">{% for group in Groups %}<div class="${resultCssClass}" data-group-id="{{ group.Id }}">{{ group.Name }}</div>{% endfor %}</div>`
            } );

            const fixture: GroupFinderFixture = {
                api,
                rockVersion,
                tester,
                blockPage,
                campuses,
                topicAttributeId,
                topicFieldTypeId: ( await api.getById( "Attributes", topicAttributeId ) ).FieldTypeId as number,
                groups,
                configure: async ( overrides = {} ) => {
                    await setBlockSettings( api, blockPage, { ...baseline(), ...overrides } );
                },
                open
            };

            markSetupDone();
            await use( fixture );
        }
        finally {
            for ( const [ label, step ] of [
                [ "test groups", () => deleteLeftovers( api, groupTypeId ) ],
                [ "test page", async () => { if ( created.blockPage ) { await deleteBlockPage( api, created.blockPage ); } } ]
            ] as Array<[ string, () => Promise<void> ]> ) {
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

    resetState: [ async ( { gf }, use ) => {
        test.info().annotations.push( { type: "rock-version", description: gf.rockVersion } );
        await gf.configure();
        await use();
    }, { auto: true } ]
} );

export { expect } from "@playwright/test";

/** The test group type, with a "Topic" single-select group attribute. */
async function ensureGroupType( api: RockApi ): Promise<{ groupTypeId: number; topicAttributeId: number }> {
    let groupType = await api.getByGuid( "GroupTypes", TestGuid.groupType );
    if ( !groupType ) {
        await api.post( "/api/GroupTypes", {
            Guid: TestGuid.groupType,
            Name: names.groupType,
            Description: "Group type for the KFS end-to-end tests of Group Finder. Do not use.",
            GroupTerm: "Group",
            GroupMemberTerm: "Member",
            ShowInGroupList: true,
            ShowInNavigation: false,
            TakesAttendance: false,
            IsSystem: false,
            Order: 0
        } );
        groupType = await api.getByGuid( "GroupTypes", TestGuid.groupType );
    }
    const groupTypeId = groupType!.Id;

    let role = await api.first( "GroupTypeRoles", `GroupTypeId eq ${groupTypeId}`, "&$orderby=Order" );
    if ( !role ) {
        await api.post( "/api/GroupTypeRoles", { Guid: randomUUID(), GroupTypeId: groupTypeId, Name: "Member", Order: 0, IsLeader: false, IsSystem: false } );
        role = await api.first( "GroupTypeRoles", `GroupTypeId eq ${groupTypeId}`, "&$orderby=Order" );
    }
    if ( groupType!.DefaultGroupRoleId !== role!.Id ) {
        await api.patch( "GroupTypes", groupTypeId, { DefaultGroupRoleId: role!.Id } );
    }

    let attribute: RockEntity | null = await api.getByGuid( "Attributes", TestGuid.topicAttribute );
    if ( !attribute ) {
        await api.post( "/api/Attributes", {
            Guid: TestGuid.topicAttribute,
            EntityTypeId: await api.getEntityTypeId( EntityTypeName.Group ),
            EntityTypeQualifierColumn: "GroupTypeId",
            EntityTypeQualifierValue: String( groupTypeId ),
            FieldTypeId: await api.getFieldTypeId( "Rock.Field.Types.SelectSingleFieldType" ),
            Key: TopicKey,
            Name: "Topic",
            Description: "KFS end-to-end tests. Do not use.",
            DefaultValue: "",
            IsGridColumn: false,
            IsMultiValue: false,
            IsRequired: false,
            IsSystem: false,
            Order: 0
        } );
        attribute = await api.getByGuid( "Attributes", TestGuid.topicAttribute );
    }
    const qualifier = await api.first( "AttributeQualifiers", `AttributeId eq ${attribute!.Id} and Key eq 'values'` );
    if ( !qualifier ) {
        await api.post( "/api/AttributeQualifiers", { AttributeId: attribute!.Id, Key: "values", Value: Topics.join( "," ), IsSystem: false, Guid: randomUUID() } );
    }

    return { groupTypeId, topicAttributeId: attribute!.Id };
}

/** Deletes every group of the test group type (with members and its schedule). They exist only for these tests. */
async function deleteLeftovers( api: RockApi, groupTypeId: number ): Promise<void> {
    for ( const group of await api.query( "Groups", `GroupTypeId eq ${groupTypeId}` ) ) {
        const scheduleId = group.ScheduleId as number | null;
        await deleteGroup( api, group.Id );
        if ( scheduleId ) {
            await api.delete( "Schedules", scheduleId );
        }
    }
}
