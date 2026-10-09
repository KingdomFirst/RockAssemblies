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
import { expect, test } from "./eventbriteSync.fixture";

// Traces keep __VIEWSTATE, which can hold the Eventbrite private token (see eventbritePages.ts).
test.use( { trace: "off" } );

/*
    Covers the manual plan's "Group Viewer Page": a group of a type with the Eventbrite
    attributes; select the event under Group Attribute Values and save; the "Eventbrite
    Sync Attendees" and "Unlink Eventbrite Event" buttons appear; syncing brings the
    event's attendees in as members with their ticket details in the member grid. The
    page is a copy of Rock's Group Viewer (Group Detail, the Eventbrite Sync Button,
    Group Member List) under the test pages.
*/
test.describe( "Eventbrite Sync: group viewer", () => {
    test( "SaveGroup_EventbriteEventSelected_LinksGroupAndShowsEventbriteButtons", async ( { page, eb } ) => {
        const groupId = await eb.createGroup( { linked: false } );
        const viewer = await eb.openGroupViewer( page, groupId );
        await expect( viewer.syncButton, "No Eventbrite buttons before the group is linked" ).toHaveCount( 0 );

        await viewer.linkEventInGroupEditor( eb.eventAttributeId, eb.event );

        expect( ( await eb.getEventValue( groupId ) ).split( "^" )[ 0 ] ).toBe( eb.event.id );
        // The buttons are drawn on page load, so open the group again as a user would.
        await eb.openGroupViewer( page, groupId );
        await expect( viewer.syncButton ).toBeVisible();
        await expect( viewer.unlinkButton ).toBeVisible();
    } );

    test( "SyncAttendees_LinkedGroup_AddsAttendeesWithTicketDetails", async ( { page, eb } ) => {
        test.setTimeout( 300_000 );
        const groupId = await eb.createGroup( { linked: true } );
        const viewer = await eb.openGroupViewer( page, groupId );

        await viewer.clickAndWaitForReload( viewer.syncButton );

        const members = await eb.api.query( "GroupMembers", `GroupId eq ${groupId}` );
        expect( members.length, "The sync should add the event's attendees as members" ).toBeGreaterThan( 0 );
        for ( const member of members ) {
            // "{attendee id}^{ticket class}^{order id}^{count}"
            const value = await eb.api.getAttributeValue( eb.personAttributeId, member.Id ) ?? "";
            expect( value, `Eventbrite Person value of group member ${member.Id}` ).toMatch( /^\d+\^[^^]*\^\d+/ );
        }
        expect( await eb.getEventValue( groupId ), "The sync records its time on the group" ).toMatch( new RegExp( `^${eb.event.id}\\^.+` ) );

        // The grid shows the field's condensed text ("Ticket Qty: 1, Ticket Class: General Admission,..."),
        // cut short before the attendee and order ids checked above.
        await expect( viewer.memberRows ).toHaveCount( members.length );
        for ( const row of await viewer.memberRows.all() ) {
            await expect( row, "Ticket details in the member grid" ).toContainText( "Ticket Class:" );
        }
    } );

    // Not in the manual plan: the other button.
    test( "Unlink_LinkedGroup_ClearsEventAndHidesButtons", async ( { page, eb } ) => {
        const groupId = await eb.createGroup( { linked: true } );
        const viewer = await eb.openGroupViewer( page, groupId );

        await viewer.clickAndWaitForReload( viewer.unlinkButton );

        expect( await eb.getEventValue( groupId ) ).toBe( "" );
        await expect( viewer.syncButton ).toHaveCount( 0 );
        await expect( viewer.unlinkButton ).toHaveCount( 0 );
    } );
} );
