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
import { addMember, expect, test } from "./roundRobin.fixture";

/*
    Covers the manual plan: a data view of people not yet in the target groups; a "Round
    Robin Group Assignment" job with "Groups to Assign People to" set to a parent group (whose
    child groups take the people) and "People Data View" set to that data view; run it; the
    job history shows people were assigned and each person is in one of the target groups.
    The job is inactive and its own (rockbeta's job 123 is not touched); Run Now runs it.
*/
test.describe( "Round Robin Group Assignment: job", () => {
    test( "Run_FourPeopleTwoChildGroups_AssignsEachPersonRoundRobin", async ( { rr } ) => {
        const job = await rr.runJob();

        expect( String( job.LastStatus ), `Job result: ${job.LastStatusMessage}` ).toMatch( /Success/i );
        expect( String( job.LastStatusMessage ) ).toMatch( /^4 people added to groups\./ );

        const members = await rr.membersByGroup();
        // Every person from the data view is in exactly one child group, and the groups are balanced.
        expect( members.flat().sort( ( a, b ) => a - b ) ).toEqual( rr.people.map( p => p.id ).sort( ( a, b ) => a - b ) );
        expect( members.map( m => m.length ) ).toEqual( [ 2, 2 ] );
    } );

    test( "Run_PeopleAlreadyAssigned_AddsNoOne", async ( { rr } ) => {
        await rr.runJob();
        const before = await rr.membersByGroup();

        const job = await rr.runJob();

        expect( String( job.LastStatus ), `Job result: ${job.LastStatusMessage}` ).toMatch( /Success/i );
        expect( String( job.LastStatusMessage ) ).toMatch( /^0 person added to groups\./ );
        expect( await rr.membersByGroup() ).toEqual( before );
    } );

    // Not in the manual plan: the job's family setting.
    test( "Run_IncludeAllFamilyMembers_AddsFamilyToSameGroup", async ( { rr } ) => {
        const job = await rr.runJob( { IncludeFamilyMembers: "All" } );

        expect( String( job.LastStatusMessage ) ).toMatch( /^5 people added to groups\./ );
        const members = await rr.membersByGroup();
        const parentsGroup = members.find( m => m.includes( rr.people[ 0 ].id ) );
        expect( parentsGroup, "The family's child (not in the data view) is in the same group as the parent" ).toContain( rr.familyChild.id );
    } );

    // Not in the manual plan: the job's removal setting.
    test( "Run_RemoveMembersNotInDataView_RemovesOthersFromTargetGroups", async ( { rr } ) => {
        await addMember( rr.api, rr.childGroupIds[ 0 ], rr.outsider.id );

        const job = await rr.runJob( { RemoveMembers: "True" } );

        expect( String( job.LastStatusMessage ) ).toMatch( /1 person removed from groups\./ );
        const members = await rr.membersByGroup();
        expect( members.flat() ).not.toContain( rr.outsider.id );
        expect( members.flat().sort( ( a, b ) => a - b ) ).toEqual( rr.people.map( p => p.id ).sort( ( a, b ) => a - b ) );
    } );
} );
