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
    Covers the manual plan's "Eventbrite Settings Page": the Private Token and
    Organization are set; choose the event in "Create new Rock Group from existing
    Eventbrite Event", click "Create New Rock Group", and check the new group exists and
    shows in "Linked Groups". The test page holds the settings block as installed on the
    site, pointed at the test group type and parent group.
*/
test.describe( "Eventbrite Sync: settings page", () => {
    test( "PageLoad_TokenAndOrganizationSet_ShowsAuthenticatedWithLinkedGroupsAndEvents", async ( { page, eb } ) => {
        const block = await eb.openSettings( page );

        await expect( block.loginStatus ).toHaveText( "Authenticated" );
        // Shown redacted: the test only checks a token is there.
        await expect( block.privateToken ).toHaveText( /^\*+$/ );
        await expect( block.organization ).toHaveText( /\S/ );
        await expect( block.linkedGroupsGrid ).toBeVisible();
        await expect( block.eventDropDown.locator( `option[value='${eb.event.id}']` ) ).toHaveCount( 1 );
    } );

    test( "CreateNewRockGroup_TestEventSelected_CreatesGroupLinkedToEvent", async ( { page, eb } ) => {
        const block = await eb.openSettings( page );

        await block.createGroupFor( eb.event.id );

        await expect( block.createNotice, "The block reported an error" ).toBeHidden();
        const groups = await eb.getChildGroups();
        expect( groups, "One new group under the New Group Parent" ).toHaveLength( 1 );
        const group = groups[ 0 ];
        expect( group.GroupTypeId ).toBe( eb.groupTypeId );
        expect( String( group.Name ) ).toMatch( new RegExp( ` - ${eb.event.name.replace( /[.*+?^${}()|[\]\\]/g, "\\$&" )}$` ) );
        expect( ( await eb.getEventValue( group.Id ) ).split( "^" )[ 0 ] ).toBe( eb.event.id );

        // Linked Groups lists it, and the event is no longer offered for a new group.
        await expect( block.linkedGroupsGrid.locator( "tr", { hasText: String( group.Name ) } ) ).toHaveCount( 1 );
        await expect( block.eventDropDown.locator( `option[value='${eb.event.id}']` ) ).toHaveCount( 0 );
    } );
} );
