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
import { DayOfWeek, expect, test } from "./groupFinder.fixture";

/*
    Beyond page 800's configuration: block settings the KFS version adds or changes
    (README "Summary"), each switched on for one test.
*/
test.describe( "Group Finder: settings", () => {
    test( "ShowAllGroups_On_ListsNonPublicGroup", async ( { page, gf } ) => {
        await gf.configure( { ShowAllGroups: "True" } );

        const block = await gf.open( page );

        await expect( block.results ).toContainText( [ gf.groups.private.name ] );
    } );

    test( "HideOvercapacityGroups_Off_ListsFullGroup", async ( { page, gf } ) => {
        await gf.configure( { HideOvercapacityGroups: "False" } );

        const block = await gf.open( page );

        await expect( block.results ).toHaveText( [ gf.groups.full.name, gf.groups.mondayBibleStudy.name, gf.groups.wednesdayPrayer.name ] );
    } );

    test( "AutoFilterOff_SearchThenClear_FiltersOnlyOnSearchAndClearResets", async ( { page, gf } ) => {
        await gf.configure( { AutoFilterEnabled: "False" } );
        const block = await gf.open( page );

        await block.dayCheckBox( DayOfWeek.Wednesday ).check();
        await expect( block.results, "Without Auto Filter nothing changes until Search" ).toHaveCount( 2 );

        await block.click( block.searchButton );
        await expect( block.results ).toHaveText( [ gf.groups.wednesdayPrayer.name ] );

        await block.click( block.clearButton );
        await expect( block.dayCheckBox( DayOfWeek.Wednesday ) ).not.toBeChecked();
    } );

    test( "SingleSelectFilters_On_CampusFilterIsDropDown", async ( { page, gf } ) => {
        await gf.configure( { SingleSelectFilters: "True" } );

        const block = await gf.open( page );

        const campusDropDown = block.root.locator( "select[id$='_filter_ddlCampus']" );
        await expect( campusDropDown ).toBeVisible();
        await campusDropDown.selectOption( String( gf.campuses[ 0 ].id ) );
        await page.waitForLoadState( "load" );
        await expect( block.results ).toHaveText( [ gf.groups.mondayBibleStudy.name ] );
    } );
} );
