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
import { expect, openManagerPage, test } from "./checkinManager.fixture";

/*
    Goes past launch into what the plugin adds over the core block: the room's people list
    with Move, Checkout, Delete and View Labels. Each test starts with the test child
    checked into the test room (see resetState in the fixture).
*/
test.describe( "Advanced Check-in Manager: Room", () => {
    test( "Navigate_AreaGroupRoom_ListsCheckedInPersonWithActions", async ( { page, checkinManager, block } ) => {
        await openManagerPage( page, checkinManager );
        await block.navigate( CheckinName.area, CheckinName.group, CheckinName.room );

        const row = block.personRow( CheckinFamily.childFirstName );
        await expect( row ).toHaveCount( 1 );
        await expect( row.locator( "[id$='_lbMovePerson']" ) ).toBeVisible();
        await expect( row.locator( "[id$='_lbCheckOut']" ) ).toBeVisible();
        await expect( row.locator( "[id$='_lbPrintLabel']" ) ).toBeVisible();
        await expect( row.locator( "[id$='_lbRemoveAttendance']" ) ).toBeVisible();
        await expect( block.moveAllButton ).toBeVisible();
    } );

    test( "Navigate_HeadingClicked_GoesUpALevel", async ( { page, checkinManager, block } ) => {
        await openManagerPage( page, checkinManager );
        await block.navigate( CheckinName.area, CheckinName.group );
        await expect( block.navItem( CheckinName.room ) ).toHaveCount( 1 );

        await block.navHeading.click();

        await expect( block.navItem( CheckinName.group ) ).toHaveCount( 1 );
    } );

    test( "Search_ByName_FindsCheckedInPerson", async ( { page, checkinManager, block } ) => {
        await openManagerPage( page, checkinManager );

        await block.search( `${CheckinFamily.childFirstName} ${CheckinFamily.lastName}` );

        await expect( block.personRow( CheckinFamily.childFirstName ) ).toHaveCount( 1 );
    } );

    test( "Checkout_Person_EndsAttendanceAndLeavesRoom", async ( { page, checkinManager, block, resetState: attendanceId } ) => {
        await openManagerPage( page, checkinManager );
        await block.navigate( CheckinName.area, CheckinName.group, CheckinName.room );

        await block.personActionWithConfirm( CheckinFamily.childFirstName, "_lbCheckOut" );

        await expect( block.personRow( CheckinFamily.childFirstName ) ).toHaveCount( 0 );
        const attendance = await checkinManager.api.getById( "Attendances", attendanceId );
        expect( attendance.EndDateTime, "Checkout should end the attendance, not delete it" ).toBeTruthy();
    } );

    test( "Delete_Person_RemovesAttendance", async ( { page, checkinManager, block, resetState: attendanceId } ) => {
        await openManagerPage( page, checkinManager );
        await block.navigate( CheckinName.area, CheckinName.group, CheckinName.room );

        await block.personActionWithConfirm( CheckinFamily.childFirstName, "_lbRemoveAttendance" );

        await expect( block.personRow( CheckinFamily.childFirstName ) ).toHaveCount( 0 );
        const remaining = await checkinManager.api.query( "Attendances", `Id eq ${attendanceId}` );
        expect( remaining, "Delete should remove the attendance record" ).toHaveLength( 0 );
    } );

    test( "Move_Person_OpensLocationDialog", async ( { page, checkinManager, block } ) => {
        await openManagerPage( page, checkinManager );
        await block.navigate( CheckinName.area, CheckinName.group, CheckinName.room );

        await block.openMoveDialog( CheckinFamily.childFirstName );

        await expect( block.moveDialog ).toContainText( "Selected Location" );
    } );

    test( "Settings_AttendeeActionsOff_HidesButtons", async ( { page, checkinManager, block } ) => {
        await checkinManager.configure( { ShowDelete: "False", ShowCheckout: "False", ShowMove: "False", ShowPrintLabel: "False" } );
        await openManagerPage( page, checkinManager );
        await block.navigate( CheckinName.area, CheckinName.group, CheckinName.room );

        const row = block.personRow( CheckinFamily.childFirstName );
        await expect( row ).toHaveCount( 1 );
        for ( const button of [ "_lbMovePerson", "_lbCheckOut", "_lbPrintLabel", "_lbRemoveAttendance" ] ) {
            await expect( row.locator( `[id$='${button}']` ), `${button} should be hidden` ).toHaveCount( 0 );
        }
    } );
} );
