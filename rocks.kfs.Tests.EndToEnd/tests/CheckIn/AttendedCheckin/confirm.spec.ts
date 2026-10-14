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
import { CheckinFamily, CheckinName, getGroupAttendance } from "../../../shared/testData/checkin";
import { expect, test } from "./attendedCheckin.fixture";

/*
    Covers the manual plan's "Confirm": the selected people are listed with their room and
    schedule, Print All and Done save attendance (the "Save Attendance" and "Create Labels"
    workflow activities), and the row Edit and Delete buttons work. The test kiosk has no
    printer and the test area has no labels, so nothing is ever sent to a printer.
*/
test.describe( "Attended Check-in: Confirm", () => {
    test.beforeEach( async ( { checkin, checkinPages } ) => {
        await checkinPages.startCheckin( { kioskId: checkin.setup.kioskId, areaId: checkin.setup.areaId } );
        await checkinPages.searchForFamily( CheckinFamily.lastName );
        await checkinPages.familyNext();
        await expect( checkinPages.page, "Family Next did not reach Confirm" ).toHaveURL( /\/attendedcheckin\/confirm/i );
    } );

    test( "Confirm_SelectedFamily_ListsPeopleWithRoomAndSchedule", async ( { checkinPages } ) => {
        await expect( checkinPages.confirmRows ).toHaveCount( 2 );

        for ( const firstName of [ CheckinFamily.adultFirstName, CheckinFamily.childFirstName ] ) {
            const row = checkinPages.confirmRow( firstName );
            await expect( row ).toHaveCount( 1 );
            await expect( row ).toContainText( CheckinName.room );
            await expect( row ).toContainText( CheckinName.schedule );
        }
    } );

    test( "Confirm_PrintAll_SavesAttendanceWithoutPrinterError", async ( { checkin, checkinPages } ) => {
        await checkinPages.printAll();

        await expect( checkinPages.checkedInMark( CheckinFamily.adultFirstName ) ).toBeVisible();
        await expect( checkinPages.checkedInMark( CheckinFamily.childFirstName ) ).toBeVisible();
        await expect( checkinPages.alert, "Print All reported a problem" ).toBeHidden();

        expect( await getGroupAttendance( checkin.api, checkin.setup.groupId, checkin.familyAdult.id ), "No attendance saved for the adult" ).toHaveLength( 1 );
        expect( await getGroupAttendance( checkin.api, checkin.setup.groupId, checkin.familyChild.id ), "No attendance saved for the child" ).toHaveLength( 1 );
    } );

    test( "Confirm_Done_SavesAttendanceAndReturnsToSearch", async ( { checkin, checkinPages } ) => {
        await checkinPages.confirmDoneButton.click();
        await checkinPages.page.waitForURL( /\/attendedcheckin\/search/i );

        expect( await getGroupAttendance( checkin.api, checkin.setup.groupId ), "Done did not save attendance" ).toHaveLength( 2 );
        await expect( checkinPages.searchBox ).toBeVisible();
    } );

    test( "Confirm_DeleteRow_RemovesPerson", async ( { checkinPages } ) => {
        await checkinPages.deleteRow( CheckinFamily.childFirstName );

        await expect( checkinPages.confirmRow( CheckinFamily.childFirstName ) ).toHaveCount( 0 );
        await expect( checkinPages.confirmRow( CheckinFamily.adultFirstName ) ).toHaveCount( 1 );
    } );

    test( "Confirm_EditRow_OpensActivitySelectAndReturns", async ( { checkinPages } ) => {
        await checkinPages.editRow( CheckinFamily.adultFirstName );

        await expect( checkinPages.activityPersonName ).toContainText( CheckinFamily.adultFirstName );

        await checkinPages.activityBackButton.click();
        await checkinPages.page.waitForURL( /\/attendedcheckin\/confirm/i );
        await expect( checkinPages.confirmRow( CheckinFamily.adultFirstName ) ).toHaveCount( 1 );
    } );
} );
