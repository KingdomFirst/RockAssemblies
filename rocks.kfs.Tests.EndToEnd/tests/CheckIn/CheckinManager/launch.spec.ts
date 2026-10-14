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
import { CheckinName } from "../../../shared/testData/checkin";
import { expect, openManagerPage, test } from "./checkinManager.fixture";

/*
    Covers the manual plan's two steps: "Setup/configure a copy of Rock Check-in manager
    page, swapping out the core Locations block" (done by the fixture, checked here) and
    "Launch Check-in manager page". The chart is the riskiest piece on new Rock versions:
    Rock 20 deleted the chart options class the block used, so it now carries its own copy.
*/
test.describe( "Advanced Check-in Manager: Launch", () => {
    test( "Setup_CopiedPage_CarriesCoreBlockSettings", async ( { checkinManager } ) => {
        // The core block's page links must survive the swap, or Person and Area Select stop working.
        expect( checkinManager.copiedSettingKeys ).toEqual( expect.arrayContaining( [ "Mode", "PersonPage", "AreaSelectPage" ] ) );
    } );

    test( "Launch_ManagerPage_ShowsCheckinTypeWithoutWarning", async ( { page, checkinManager, block } ) => {
        await openManagerPage( page, checkinManager );

        await expect( block.title ).toContainText( CheckinName.checkinType );
        await expect( block.warning ).toBeHidden();
        await expect( block.navItem( CheckinName.area ) ).toHaveCount( 1 );
    } );

    test( "Launch_ManagerPage_DrawsAttendanceChart", async ( { page, checkinManager, block } ) => {
        const scriptErrors: string[] = [];
        page.on( "pageerror", error => scriptErrors.push( error.message ) );

        await openManagerPage( page, checkinManager );

        await expect( block.chart.locator( "canvas" ).first(), "The attendance chart did not draw" ).toBeVisible();
        expect( scriptErrors, "The page raised script errors" ).toEqual( [] );
    } );

    test( "Launch_AreaCount_IncludesCheckedInPerson", async ( { page, checkinManager, block } ) => {
        await openManagerPage( page, checkinManager );

        await expect( block.currentCount( CheckinName.area ) ).toHaveText( /\b1\b/ );
    } );
} );
