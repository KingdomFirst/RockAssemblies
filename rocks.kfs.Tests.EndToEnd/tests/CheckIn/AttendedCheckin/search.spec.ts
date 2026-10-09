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
import { CheckinFamily } from "../../../shared/testData/checkin";
import { expect, test } from "./attendedCheckin.fixture";

/*
    Covers the manual plan's "Search / Search Results": name and phone search reach the
    test family with its members listed, and bad input is rejected. The search runs the
    plugin's "Family Search" workflow activity, so a broken check-in workflow action
    shows up here.
*/
test.describe( "Attended Check-in: Search", () => {
    test.beforeEach( async ( { checkin, checkinPages } ) => {
        await checkinPages.startCheckin( { kioskId: checkin.setup.kioskId, areaId: checkin.setup.areaId } );
    } );

    test( "Search_ByLastName_FindsFamilyAndMembers", async ( { checkin, checkinPages } ) => {
        await checkinPages.searchForFamily( CheckinFamily.lastName );

        await expect( checkinPages.familyButton( CheckinFamily.lastName ) ).toHaveCount( 1 );
        await expect( checkinPages.personButton( checkin.familyAdult.id ) ).toBeVisible();
        await expect( checkinPages.personButton( checkin.familyChild.id ) ).toBeVisible();
    } );

    test( "Search_ByPhone_FindsFamily", async ( { checkin, checkinPages } ) => {
        await checkinPages.searchForFamily( CheckinFamily.mobilePhone );

        await expect( checkinPages.familyButton( CheckinFamily.lastName ) ).toHaveCount( 1 );
        await expect( checkinPages.personButton( checkin.familyAdult.id ) ).toBeVisible();
    } );

    test( "Search_PhoneTooShort_WarnsMinimumLength", async ( { checkinPages } ) => {
        await checkinPages.search( "12" );

        await expect( checkinPages.alert ).toContainText( /Please enter at least \d+ character\(s\)/ );
        await expect( checkinPages.page ).toHaveURL( /\/attendedcheckin\/search/i );
    } );

    test( "Search_NoMatch_ShowsNoResults", async ( { checkinPages } ) => {
        await checkinPages.search( "Zzqkfsnomatch" );

        // The plugin either reports the empty search as a warning or shows its "No Results"
        // panel with the Not Found Text; either way no family is offered.
        const noResults = checkinPages.page.getByRole( "heading", { name: "No Results" } );
        await expect( checkinPages.alert.or( noResults ).first() ).toBeVisible();
        await expect( checkinPages.familyButtons ).toHaveCount( 0 );
    } );
} );
