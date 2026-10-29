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
import { CheckinFamily, CheckinName } from "../../../shared/testData/checkin";
import { expect, openManagerPage, test, verifyManagerCanCloseRooms } from "./checkinManager.fixture";

/*
    Room-level controls: opening and closing a room, its threshold, and the block's
    Location navigation mode. The fixture puts the shared test room back to open with no
    thresholds before and after every test.
*/
test.describe( "Advanced Check-in Monitor: open and close rooms", () => {
    // The toggles are shown only to people who may close rooms (README: one-time setup).
    test.beforeEach( async ( { checkinManager } ) => {
        await verifyManagerCanCloseRooms( checkinManager );
    } );

    test( "Close_RoomFromItsHeading_InactivatesLocationAndReopens", async ( { page, checkinManager, block } ) => {
        const { api, setup } = checkinManager;

        await openManagerPage( page, checkinManager );
        await block.navigate( CheckinName.area, CheckinName.group, CheckinName.room );
        await expect( block.headingToggle ).toBeVisible();

        await block.setToggle( block.headingToggle, false );
        expect( ( await api.getById( "Locations", setup.roomId ) ).IsActive, "Close should inactivate the room" ).toBe( false );

        await block.setToggle( block.headingToggle, true );
        expect( ( await api.getById( "Locations", setup.roomId ) ).IsActive, "Open should reactivate the room" ).toBe( true );
    } );

    test( "Close_RoomFromGroupList_InactivatesLocation", async ( { page, checkinManager, block } ) => {
        const { api, setup } = checkinManager;

        await openManagerPage( page, checkinManager );
        await block.navigate( CheckinName.area, CheckinName.group );
        await expect( block.navToggle( CheckinName.room ) ).toBeVisible();

        await block.setToggle( block.navToggle( CheckinName.room ), false );

        expect( ( await api.getById( "Locations", setup.roomId ) ).IsActive, "Close should inactivate the room" ).toBe( false );
    } );
} );

test.describe( "Advanced Check-in Monitor: thresholds", () => {
    test( "Threshold_SoftThresholdSet_ShowsCountOutOfThreshold", async ( { page, checkinManager, block } ) => {
        await checkinManager.api.patch( "Locations", checkinManager.setup.roomId, { SoftRoomThreshold: 5 } );

        await openManagerPage( page, checkinManager );
        await block.navigate( CheckinName.area, CheckinName.group );

        await expect( block.currentCount( CheckinName.room ) ).toHaveText( "1/5" );
        await expect( block.currentCount( CheckinName.room ) ).toHaveClass( /badge-success/ );
    } );

    test( "Threshold_RoomAtSoftThreshold_FlagsRoomAsFull", async ( { page, checkinManager, block } ) => {
        await checkinManager.api.patch( "Locations", checkinManager.setup.roomId, { SoftRoomThreshold: 1 } );

        await openManagerPage( page, checkinManager );
        await block.navigate( CheckinName.area, CheckinName.group );

        await expect( block.currentCount( CheckinName.room ) ).toHaveText( "1/1" );
        await expect( block.currentCount( CheckinName.room ) ).toHaveClass( /badge-danger/ );
    } );

    test( "Threshold_EditedInRoomHeading_SavesNewThreshold", async ( { page, checkinManager, block } ) => {
        const { api, setup } = checkinManager;
        await api.patch( "Locations", setup.roomId, { SoftRoomThreshold: 5 } );

        await openManagerPage( page, checkinManager );
        await block.navigate( CheckinName.area, CheckinName.group, CheckinName.room );
        await expect( block.thresholdLabel ).toHaveText( "5" );

        await block.editThreshold( 3 );

        await expect( block.thresholdLabel ).toHaveText( "3" );
        expect( ( await api.getById( "Locations", setup.roomId ) ).SoftRoomThreshold ).toBe( 3 );
    } );

    test( "Threshold_NoThreshold_HidesThresholdEditor", async ( { page, checkinManager, block } ) => {
        await openManagerPage( page, checkinManager );
        await block.navigate( CheckinName.area, CheckinName.group, CheckinName.room );

        await expect( block.personRow( CheckinFamily.childFirstName ) ).toHaveCount( 1 );
        await expect( block.thresholdPanel ).toHaveCount( 0 );
    } );
} );

test.describe( "Advanced Check-in Monitor: Location navigation mode", () => {
    test( "Navigate_LocationMode_CampusLocationThenRoomListsCheckedInPerson", async ( { page, checkinManager, block } ) => {
        await checkinManager.configure( { Mode: "L" } );

        await openManagerPage( page, checkinManager );
        await expect( block.navItem( CheckinName.area ), "Location mode should not list check-in areas" ).toHaveCount( 0 );

        await block.navigate( checkinManager.campusLocationName, CheckinName.room );

        await expect( block.personRow( CheckinFamily.childFirstName ) ).toHaveCount( 1 );
    } );

    test( "Navigate_LocationMode_CountsCheckedInPersonOnRoom", async ( { page, checkinManager, block } ) => {
        await checkinManager.configure( { Mode: "L" } );

        await openManagerPage( page, checkinManager );
        await block.navigate( checkinManager.campusLocationName );

        await expect( block.currentCount( CheckinName.room ) ).toHaveText( "1" );
    } );
} );
