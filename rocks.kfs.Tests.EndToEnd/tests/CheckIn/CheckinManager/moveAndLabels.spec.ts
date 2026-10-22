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
import { waitForPostback } from "../../../shared/rockBrowser";
import { CheckinFamily, CheckinName, addAreaCheckinLabel, createCurrentAttendance, deleteGroupAttendance, getOpenAttendanceAt, removeAreaCheckinLabel } from "../../../shared/testData/checkin";
import { expect, openManagerPage, test } from "./checkinManager.fixture";

/*
    Completing the attendee actions that the Room tests only open: Move (picking a new
    room and saving), Move All, and View Labels. Each test starts with the test child
    checked into the test room.
*/
test.describe( "Advanced Check-in Monitor: Move and labels", () => {
    test( "Move_PersonToAnotherRoom_EndsOldAttendanceAndChecksIntoNewRoom", async ( { page, checkinManager, block, resetState: attendanceId } ) => {
        const { api, setup, familyChild, moveRoomId } = checkinManager;

        await openManagerPage( page, checkinManager );
        await block.navigate( CheckinName.area, CheckinName.group, CheckinName.room );
        await block.openMoveDialog( CheckinFamily.childFirstName );
        await block.completeMove( moveRoomId, CheckinName.moveRoom );

        await expect( block.personRow( CheckinFamily.childFirstName ), "The moved person should leave the room" ).toHaveCount( 0 );
        expect( ( await api.getById( "Attendances", attendanceId ) ).EndDateTime, "Move should end the old attendance" ).toBeTruthy();
        expect( await getOpenAttendanceAt( api, setup, familyChild, moveRoomId ), "Move should check the person into the new room" ).not.toBeNull();
    } );

    test( "MoveAll_TwoPeopleInRoom_MovesEveryoneToNewRoom", async ( { page, checkinManager, block } ) => {
        const { api, setup, familyChild, familyAdult, moveRoomId } = checkinManager;
        await createCurrentAttendance( api, setup, familyAdult );

        await openManagerPage( page, checkinManager );
        await block.navigate( CheckinName.area, CheckinName.group, CheckinName.room );
        await expect( block.personRows ).toHaveCount( 2 );
        await block.openMoveAllDialog();
        await block.completeMove( moveRoomId, CheckinName.moveRoom );

        await expect( block.personRows, "Everyone should leave the room" ).toHaveCount( 0 );
        for ( const person of [ familyChild, familyAdult ] ) {
            expect( await getOpenAttendanceAt( api, setup, person, setup.roomId ), `${person.fullName} should no longer be in the old room` ).toBeNull();
            expect( await getOpenAttendanceAt( api, setup, person, moveRoomId ), `${person.fullName} should be in the new room` ).not.toBeNull();
        }
    } );

    /*
        10/13/2026 - CLAUDE

        Viewing a label renders it the way a reprint would: the block merges the person's
        check-in into the area's label, sends the ZPL to labelary.com from the server, and
        embeds the PDF it gets back. So this needs what a real check-in leaves behind (a
        label on the area, a security code on the attendance) and outbound internet access
        from the Rock server. The label and code are added here and removed afterward.

        Reason: The test area has no label, and API-made attendance has no security code.
    */
    test( "ViewLabel_CheckedInPerson_ShowsRenderedLabelPdf", async ( { page, checkinManager, block, resetState: attendanceId } ) => {
        const { api, setup } = checkinManager;
        const label = await addAreaCheckinLabel( api, setup );
        const codeId = await api.post( "/api/AttendanceCodes", { Code: "E2E", IssueDateTime: new Date().toISOString().substring( 0, 19 ) } );
        await api.patch( "Attendances", attendanceId, { AttendanceCodeId: codeId } );

        try {
            await openManagerPage( page, checkinManager );
            await block.navigate( CheckinName.area, CheckinName.group, CheckinName.room );
            await block.openPrintLabelDialog( CheckinFamily.childFirstName );

            await expect( block.labelSelect.locator( "option", { hasText: label.fileName } ), "The area's label should be listed" ).toHaveCount( 1 );
            await block.labelSelect.selectOption( { label: label.fileName } );
            await waitForPostback( page );

            await expect( block.labelPreview, "No label preview: the server may not be able to reach labelary.com" ).toBeVisible();
            const pdfUrl = await block.labelPreview.getAttribute( "data" );
            expect( pdfUrl ).toMatch( /\/Cache\/.+\.pdf$/i );

            const pdf = await page.request.get( pdfUrl as string );
            expect( pdf.status() ).toBe( 200 );
            expect( ( await pdf.body() ).subarray( 0, 4 ).toString(), "The preview should be a PDF" ).toBe( "%PDF" );
        }
        finally {
            await removeAreaCheckinLabel( api, setup );
            await deleteGroupAttendance( api, setup.groupId );
            await api.delete( "AttendanceCodes", codeId );
        }
    } );
} );
