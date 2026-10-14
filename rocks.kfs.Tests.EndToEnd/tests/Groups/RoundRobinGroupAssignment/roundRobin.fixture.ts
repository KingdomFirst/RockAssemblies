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
import { RockApi, RockEntity } from "../../../shared/rockApi";
import { EntityTypeName, SystemGuid, TestNamePrefix } from "../../../shared/systemGuids";
import { GroupMemberStatus, addGroupMember, deleteGroup, deleteGroupMembers, getMemberStatus } from "../../../shared/testData/groups";
import { ensureTestJob, getJobSettingIds, runJobNow } from "../../../shared/testData/jobs";
import { TestPerson, ensureFamilyMember, ensurePerson } from "../../../shared/testData/people";

const JobClass = "rocks.kfs.RoundRobinGroupAssignment.Jobs.RoundRobinGroupAssignment";

/** Fixed Guids for this suite's persistent test data (job and data view). */
const TestGuid = {
    job: "0C1D2E3F-E2E0-4C0E-8C0E-0000000000A1",
    dataView: "0C1D2E3F-E2E0-4C0E-8C0E-0000000000A2",
    rootFilter: "0C1D2E3F-E2E0-4C0E-8C0E-0000000000A3",
    lastNameFilter: "0C1D2E3F-E2E0-4C0E-8C0E-0000000000A4"
} as const;

/** Last name of the people the test data view selects (and only them). */
export const DataViewLastName = `${TestNamePrefix} Rrtester`;

const names = {
    dataView: `${TestNamePrefix} Round Robin People`,
    parentGroup: "RR Parent",
    childGroups: [ "RR Group A", "RR Group B" ]
};

export type RoundRobinFixture = {
    api: RockApi;
    rockVersion: string;

    /** The four people the data view selects, each in their own family. */
    people: TestPerson[];
    /** Child in the first person's family; not in the data view. */
    familyChild: TestPerson;
    /** Not in the data view; added to a target group by hand to test member removal. */
    outsider: TestPerson;

    parentGroupId: number;
    childGroupIds: number[];

    /** Sets the test job's settings (on top of the baseline) and presses Run Now. Returns the job after the run. */
    runJob( settings?: Record<string, string> ): Promise<RockEntity>;
    /** Active member person Ids of each child group. */
    membersByGroup(): Promise<number[][]>;
};

export const test = base.extend<{ resetState: void }, { rr: RoundRobinFixture }>( {
    rr: [ async ( {}, use ) => guardFixtureSetup( "Round Robin Group Assignment", async markSetupDone => {
        const settings = getTestSettings();
        const api = await RockApi.create();
        const rockVersion = await api.getRockVersion();
        console.log( `Rock version under test: ${rockVersion} at ${settings.baseUrl}` );

        const created: { parentGroupId?: number } = {};
        try {
            await deleteLeftoverGroups( api );

            const people: TestPerson[] = [];
            for ( const [ index, firstName ] of [ "Rrone", "Rrtwo", "Rrthree", "Rrfour" ].entries() ) {
                people.push( await ensurePerson( api, {
                    guid: `0C1D2E3F-6F70-4A81-9B92-0C1D2E3F4A6${index + 1}`,
                    firstName,
                    lastName: DataViewLastName,
                    email: `rr.${firstName.toLowerCase()}@kfs-e2e.invalid`
                } ) );
            }
            const familyChild = await ensureFamilyMember( api, people[ 0 ].familyId, {
                guid: "0C1D2E3F-6F70-4A81-9B92-0C1D2E3F4A65",
                firstName: "Rrkid",
                lastName: `${TestNamePrefix} Rrfamily`,
                email: "rr.kid@kfs-e2e.invalid"
            } );
            const outsider = await ensurePerson( api, {
                guid: "0C1D2E3F-6F70-4A81-9B92-0C1D2E3F4A66",
                firstName: "Rroutsider",
                lastName: `${TestNamePrefix} Rrfamily`,
                email: "rr.outsider@kfs-e2e.invalid"
            } );

            const dataViewGuid = await ensureDataView( api );
            const job = await ensureTestJob( api, { guid: TestGuid.job, name: `${TestNamePrefix} Round Robin Group Assignment`, jobClass: JobClass } );
            const jobSettingIds = await getJobSettingIds( api, JobClass );
            if ( Object.keys( jobSettingIds ).length === 0 ) {
                throw new Error( `No settings are registered for job type ${JobClass}. Is the Round Robin Group Assignment plugin installed?` );
            }

            // Per run: a parent Small Group with two child groups to assign people to.
            const groupTypeId = await api.getIdByGuid( "GroupTypes", SystemGuid.GroupType.SmallGroup );
            // Sites can set "Groups Require Campus" on Small Group (rockbeta does); Use Group Campus stays off, so it does not affect assignment.
            const campus = await api.first( "Campuses", "IsActive eq true", "&$orderby=Order" );
            const createGroup = async ( name: string, parentGroupId: number | null ): Promise<number> => await api.post( "/api/Groups", {
                Guid: randomUUID(),
                Name: `${TestNamePrefix} ${name}`,
                Description: "KFS end-to-end tests of Round Robin Group Assignment.",
                GroupTypeId: groupTypeId,
                CampusId: campus ? campus.Id : null,
                ParentGroupId: parentGroupId,
                IsActive: true,
                IsPublic: false,
                IsSystem: false,
                IsSecurityRole: false,
                Order: 0
            } );
            const parentGroupId = await createGroup( names.parentGroup, null );
            created.parentGroupId = parentGroupId;
            const childGroupIds: number[] = [];
            for ( const name of names.childGroups ) {
                childGroupIds.push( await createGroup( name, parentGroupId ) );
            }

            const baseline = (): Record<string, string> => ( {
                PeopleToAddDataView: dataViewGuid.toLowerCase(),
                GroupsToCycleThrough: String( parentGroupId ),
                DefaultCampus: "",
                DefaultGroup: "",
                IncludeSelectedGroups: "False",
                IncludeFamilyMembers: "None",
                OutputErrors: "True",
                UseGroupCampus: "False",
                RemoveMembers: "False"
            } );

            const fixture: RoundRobinFixture = {
                api,
                rockVersion,
                people,
                familyChild,
                outsider,
                parentGroupId,
                childGroupIds,
                runJob: async ( overrides = {} ) => {
                    for ( const [ key, value ] of Object.entries( { ...baseline(), ...overrides } ) ) {
                        const attributeId = jobSettingIds[ key ];
                        if ( !attributeId ) {
                            throw new Error( `Job setting '${key}' does not exist for ${JobClass}. Known settings: ${Object.keys( jobSettingIds ).join( ", " )}` );
                        }
                        await api.setAttributeValue( attributeId, job.id, value );
                    }
                    return await runJobNow( api, job );
                },
                membersByGroup: async () => {
                    const result: number[][] = [];
                    for ( const groupId of childGroupIds ) {
                        const members = await api.query( "GroupMembers", `GroupId eq ${groupId}` );
                        result.push( members.filter( m => getMemberStatus( m ) === GroupMemberStatus.Active ).map( m => m.PersonId as number ).sort( ( a, b ) => a - b ) );
                    }
                    return result;
                }
            };

            markSetupDone();
            await use( fixture );
        }
        finally {
            if ( created.parentGroupId ) {
                await deleteGroupTree( api, created.parentGroupId ).catch( error => console.warn( `Cleanup of the test groups failed; the next run will retry it. ${( error as Error ).message}` ) );
            }
            await api.dispose();
        }
    } ), { scope: "worker", timeout: 600_000 } ],

    // Runs before every test: empties the child groups, so each run starts from no assignments.
    resetState: [ async ( { rr }, use ) => {
        test.info().annotations.push( { type: "rock-version", description: rr.rockVersion } );
        for ( const groupId of rr.childGroupIds ) {
            await deleteGroupMembers( rr.api, groupId );
        }
        await use();
    }, { auto: true } ]
} );

export { expect } from "@playwright/test";

/** Adds a person to a child group by hand (Active), as if assigned outside the job. */
export async function addMember( api: RockApi, groupId: number, personId: number ): Promise<void> {
    await addGroupMember( api, { groupId, personId, status: GroupMemberStatus.Active } );
}

/**
 * The persistent data view: people whose last name is exactly DataViewLastName. Built
 * through REST as Rock stores data view filters: an "all of" group holding one
 * PropertyFilter whose selection is [property, comparison (1 = equal to), value].
 */
async function ensureDataView( api: RockApi ): Promise<string> {
    if ( await api.getByGuid( "DataViews", TestGuid.dataView ) ) {
        return TestGuid.dataView;
    }

    const rootFilterId = await api.post( "/api/DataViewFilters", {
        Guid: TestGuid.rootFilter,
        // FilterExpressionType.GroupAll
        ExpressionType: 1
    } );
    await api.post( "/api/DataViewFilters", {
        Guid: TestGuid.lastNameFilter,
        // FilterExpressionType.Filter
        ExpressionType: 0,
        ParentId: rootFilterId,
        EntityTypeId: await api.getEntityTypeId( "Rock.Reporting.DataFilter.PropertyFilter" ),
        Selection: JSON.stringify( [ "LastName", "1", DataViewLastName ] )
    } );
    await api.post( "/api/DataViews", {
        Guid: TestGuid.dataView,
        Name: names.dataView,
        Description: "People for the KFS end-to-end tests of Round Robin Group Assignment. Do not use.",
        EntityTypeId: await api.getEntityTypeId( EntityTypeName.Person ),
        DataViewFilterId: rootFilterId,
        IsSystem: false
    } );

    return TestGuid.dataView;
}

/** Deletes a group and its child groups, with their members. */
async function deleteGroupTree( api: RockApi, groupId: number ): Promise<void> {
    for ( const child of await api.query( "Groups", `ParentGroupId eq ${groupId}` ) ) {
        await deleteGroupTree( api, child.Id );
    }
    await deleteGroup( api, groupId );
}

/** Deletes test parent groups an earlier run left behind. */
async function deleteLeftoverGroups( api: RockApi ): Promise<void> {
    for ( const group of await api.query( "Groups", `Name eq '${TestNamePrefix} ${names.parentGroup}'` ) ) {
        await deleteGroupTree( api, group.Id );
    }
}
