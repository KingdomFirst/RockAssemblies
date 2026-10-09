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
    Covers the manual plan's last steps: set "Show Specific Group" to the fundraising
    group, open the page with no group in the URL, and check the block shows that group.
*/
test.describe( "Fundraising Progress: Show Specific Group setting", () => {
    // Fail (not skip): the manual plan requires this setting, so an installed block without it is a finding.
    test.beforeEach( ( { afp } ) => {
        expect( afp.showSpecificGroupMissing, "Show Specific Group setting" ).toBeNull();
    } );

    test( "PageLoad_ShowSpecificGroupSettingWithoutParameters_ShowsThatGroup", async ( { page, afp, block } ) => {
        await afp.configure( { ShowSpecificGroup: afp.group.guid.toLowerCase() } );

        await afp.open( page );

        await expect( block.title ).toHaveText( afp.groupName );
        await expect( block.totalRaised ).toHaveText( `$${afp.expectedTotals.raised}` );
        await expect( block.groupTotalAmounts ).toHaveText( `$${afp.expectedTotals.raised}/$${afp.expectedTotals.goal}` );
        await expect( block.memberRows ).toHaveCount( afp.participants.length );
    } );

    // Not in the manual plan: the setting's description says the URL parameter still wins.
    test( "PageLoad_GroupIdParameterAndShowSpecificGroupSetting_ParameterWins", async ( { page, afp, block } ) => {
        await afp.configure( { ShowSpecificGroup: afp.group.guid.toLowerCase() } );

        await afp.open( page, { GroupId: afp.otherGroup.id } );

        await expect( block.title ).toHaveText( afp.otherGroupName );
        await expect( block.memberRows ).toHaveCount( 0 );
    } );
} );
