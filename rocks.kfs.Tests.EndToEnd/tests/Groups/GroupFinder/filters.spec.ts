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
import { DayOfWeek, TopicKey, expect, test } from "./groupFinder.fixture";

/*
    Covers the manual plan: a page with the Group Finder KFS block, launched and exercised
    through its elements. The block uses the settings from the KFS beta site's page 800
    (Auto Load, Auto Filter, Campus / Day of Week / Keyword / Topic filters, full groups
    hidden), aimed at a test group type with four groups whose differences each filter
    should pick out.
*/
test.describe( "Group Finder: filters", () => {
    test( "PageLoad_AutoLoad_ListsPublicGroupsWithRoom", async ( { page, gf } ) => {
        const block = await gf.open( page );

        // Sorted by name; the full and the private group are left out.
        await expect( block.results ).toHaveText( [ gf.groups.mondayBibleStudy.name, gf.groups.wednesdayPrayer.name ] );
    } );

    test( "Keyword_WordInDescription_ShowsOnlyThatGroup", async ( { page, gf } ) => {
        const block = await gf.open( page );

        await block.typeKeyword( "potluck" );

        await expect( block.results ).toHaveText( [ gf.groups.wednesdayPrayer.name ] );
    } );

    test( "Keyword_WordInName_ShowsOnlyThatGroup", async ( { page, gf } ) => {
        const block = await gf.open( page );

        await block.typeKeyword( "Bible" );

        await expect( block.results ).toHaveText( [ gf.groups.mondayBibleStudy.name ] );
    } );

    test( "Campus_SecondCampusChecked_ShowsOnlyItsGroups", async ( { page, gf } ) => {
        const block = await gf.open( page );

        await block.check( block.campusCheckBox( gf.campuses[ 1 ].id ) );

        await expect( block.results ).toHaveText( [ gf.groups.wednesdayPrayer.name ] );
    } );

    test( "DayOfWeek_MondayChecked_ShowsOnlyMondayGroups", async ( { page, gf } ) => {
        const block = await gf.open( page );

        await block.check( block.dayCheckBox( DayOfWeek.Monday ) );

        await expect( block.results ).toHaveText( [ gf.groups.mondayBibleStudy.name ] );
    } );

    test( "AttributeFilter_TopicPrayer_ShowsOnlyPrayerGroups", async ( { page, gf } ) => {
        const block = await gf.open( page );

        await block.check( block.attributeCheckBox( TopicKey, "Prayer" ) );

        await expect( block.results ).toHaveText( [ gf.groups.wednesdayPrayer.name ] );
    } );

    test( "Filters_DayAndCampusThatDoNotMatch_ShowsNoGroups", async ( { page, gf } ) => {
        const block = await gf.open( page );

        await block.check( block.dayCheckBox( DayOfWeek.Wednesday ) );
        await block.check( block.campusCheckBox( gf.campuses[ 0 ].id ) );

        await expect( block.results ).toHaveCount( 0 );
    } );

    test( "UrlParameters_TopicAndDay_PrefillFiltersAndShowMatches", async ( { page, gf } ) => {
        const block = await gf.open( page, { [ `filter_${TopicKey}_${gf.topicFieldTypeId}` ]: "Bible Study", filter_dow: DayOfWeek.Monday } );

        await expect( block.attributeCheckBox( TopicKey, "Bible Study" ) ).toBeChecked();
        await expect( block.dayCheckBox( DayOfWeek.Monday ) ).toBeChecked();
        await expect( block.results ).toHaveText( [ gf.groups.mondayBibleStudy.name ] );
    } );
} );
