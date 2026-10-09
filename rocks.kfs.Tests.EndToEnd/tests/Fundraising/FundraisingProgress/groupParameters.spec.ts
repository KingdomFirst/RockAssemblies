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
import { expect, test } from "./fundraisingProgress.fixture";

/*
    Covers the manual plan's first steps: launch the page for a fundraising group with
    contributions, and check everything works with no errors (openPageAsPerson fails on
    any Rock error page or block error). Every number shown is checked against the goals
    and gifts the fixture recorded.
*/
test.describe( "Fundraising Progress: GroupId and GroupMemberId parameters", () => {
    test( "PageLoad_GroupIdParameter_ShowsGroupTotalsAndEachMembersProgress", async ( { page, afp, block } ) => {
        await afp.open( page, { GroupId: afp.group.id } );

        await expect( block.title ).toHaveText( afp.groupName );
        await expect( block.totalRaised ).toHaveText( `$${afp.expectedTotals.raised}` );
        await expect( block.groupTotalAmounts ).toHaveText( `$${afp.expectedTotals.raised}/$${afp.expectedTotals.goal}` );
        await expect( block.groupProgressBar ).toHaveText( afp.expectedTotals.percent );
        await expect( block.groupProgressBar ).toHaveClass( new RegExp( afp.expectedTotals.barClass ) );

        // Sorted by last name, then nick name.
        await expect( block.memberRows ).toHaveCount( afp.participants.length );
        for ( const [ index, participant ] of afp.participants.entries() ) {
            const name = participant.person.fullName;
            await expect( block.memberRows.nth( index ) ).toContainText( name );
            await expect( block.memberAmounts( name ) ).toHaveText( `$${participant.expected.raised}/$${participant.expected.goal}` );
            await expect( block.memberProgressBar( name ) ).toHaveText( `${participant.expected.percent}% Complete` );
            await expect( block.memberProgressBar( name ) ).toHaveClass( new RegExp( participant.expected.barClass ) );
        }
    } );

    // Not in the manual plan: the block's other page parameter.
    test( "PageLoad_GroupMemberIdParameter_ShowsOnlyThatMemberWithoutGroupTotals", async ( { page, afp, block } ) => {
        const participant = afp.participants[ 0 ];

        await afp.open( page, { GroupMemberId: participant.groupMemberId } );

        await expect( block.title ).toHaveText( afp.groupName );
        await expect( block.memberRows ).toHaveCount( 1 );
        await expect( block.memberAmounts( participant.person.fullName ) ).toHaveText( `$${participant.expected.raised}/$${participant.expected.goal}` );
        // Fixed in v1.1: up to v1.0 the display settings re-showed the group totals here.
        await expect( block.groupTotals ).toHaveCount( 0 );
    } );

    // Not in the manual plan: what a page without a group shows.
    test( "PageLoad_NoGroupParameterOrSetting_ShowsNoProgress", async ( { page, afp, block } ) => {
        await afp.open( page );

        await expect( block.root ).toHaveCount( 1 );
        await expect( block.view ).toHaveCount( 0 );
    } );
} );
