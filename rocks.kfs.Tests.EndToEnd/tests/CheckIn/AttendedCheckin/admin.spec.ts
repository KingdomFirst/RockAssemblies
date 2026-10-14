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
import { expect, test } from "./attendedCheckin.fixture";

/*
    Covers the manual plan's "Launch the admin screen and ensure configuration works
    smoothly": the kiosk list, the area list for the chosen kiosk, the validation
    messages, and handing off to Search with a working check-in session.
*/
test.describe( "Attended Check-in: Admin", () => {
    test( "Admin_Load_ListsTestKiosk", async ( { checkinPages } ) => {
        await checkinPages.openAdmin();

        await expect( checkinPages.kioskDropDown ).toBeVisible();
        await expect( checkinPages.kioskDropDown.locator( "option", { hasText: CheckinName.kiosk } ) ).toHaveCount( 1 );
    } );

    test( "Admin_SelectKiosk_ListsTestArea", async ( { checkin, checkinPages } ) => {
        await checkinPages.openAdmin();
        await checkinPages.selectKiosk( checkin.setup.kioskId );

        await expect( checkinPages.areaButton( checkin.setup.areaId ), "Test area missing; has setup/AttendedCheckin_Links.sql been run?" ).toBeVisible();
        await expect( checkinPages.areaButton( checkin.setup.areaId ) ).toHaveValue( CheckinName.area );
    } );

    test( "Admin_OkWithoutDevice_WarnsToSelectDevice", async ( { checkinPages } ) => {
        await checkinPages.openAdmin();
        await checkinPages.clickOk();

        await expect( checkinPages.alert ).toContainText( "Please select a check-in device and area." );
    } );

    test( "Admin_OkWithoutArea_WarnsToSelectArea", async ( { checkin, checkinPages } ) => {
        await checkinPages.openAdmin();
        await checkinPages.selectKiosk( checkin.setup.kioskId );
        await checkinPages.clickOk();

        await expect( checkinPages.alert ).toContainText( "Please select at least one check-in area." );
    } );

    test( "Admin_AreaToggledTwice_IsDeselected", async ( { checkin, checkinPages } ) => {
        await checkinPages.openAdmin();
        await checkinPages.selectKiosk( checkin.setup.kioskId );
        await checkinPages.toggleArea( checkin.setup.areaId );
        await expect( checkinPages.areaButton( checkin.setup.areaId ) ).toHaveClass( /\bactive\b/ );

        await checkinPages.toggleArea( checkin.setup.areaId );
        await expect( checkinPages.areaButton( checkin.setup.areaId ) ).not.toHaveClass( /\bactive\b/ );

        await checkinPages.clickOk();
        await expect( checkinPages.alert ).toContainText( "Please select at least one check-in area." );
    } );

    test( "Admin_KioskAndArea_OpensSearch", async ( { checkin, checkinPages } ) => {
        await checkinPages.startCheckin( { kioskId: checkin.setup.kioskId, areaId: checkin.setup.areaId } );

        await expect( checkinPages.searchBox ).toBeVisible();
        await expect( checkinPages.alert, "Check-in reported a problem on arrival at Search" ).toBeHidden();
    } );
} );
