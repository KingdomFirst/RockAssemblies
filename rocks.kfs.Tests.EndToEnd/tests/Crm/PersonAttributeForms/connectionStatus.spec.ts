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
import { openPageAsPerson } from "../../../shared/rockBrowser";
import { SystemGuid } from "../../../shared/systemGuids";
import { PersonFieldType, expect, test } from "./personAttributeForms.fixture";

/*
    The Connection Status person field. v2.7 changed the block's fallback status from Web
    Prospect (removed in Rock 20) to Prospect, so this covers the field end to end on the
    Rock version under test: it renders Rock's statuses, saves a chosen one, and survives
    being left blank.

    10/13/2026 - CLAUDE

    The Prospect fallback itself cannot be reached from the form. The block only records a
    person field's value when it is not blank, and it saves only recorded values, so a blank
    Connection Status leaves the person's status as it was rather than setting Prospect.
    Submit_ConnectionStatusLeftBlank_KeepsExistingStatus pins that actual behavior. If the
    block is changed so a blank really means Prospect, change that test's expectation.

    Reason: The fallback is unreachable through the UI; test what users can actually do.
*/
test.describe( "Person Attributes Form Advanced: Connection Status field", () => {
    test.beforeEach( async ( { pafa } ) => {
        await pafa.configure( { Forms: pafa.buildCustomFormJson( [ { field: PersonFieldType.ConnectionStatus } ] ) } );
        await setConnectionStatus( pafa, SystemGuid.DefinedValue.ConnectionStatusAttendee );
    } );

    test( "PageLoad_ConnectionStatusField_ListsStatusesWithCurrentSelected", async ( { page, pafa, block } ) => {
        const attendeeId = await pafa.api.getIdByGuid( "DefinedValues", SystemGuid.DefinedValue.ConnectionStatusAttendee );
        const prospectId = await pafa.api.getIdByGuid( "DefinedValues", SystemGuid.DefinedValue.ConnectionStatusProspect );

        await openPageAsPerson( page, pafa.api, { pageId: pafa.blockPage.pageId, personId: pafa.person.id } );

        await expect( block.connectionStatusPicker ).toBeVisible();
        await expect( block.connectionStatusPicker.locator( `option[value='${prospectId}']` ) ).toHaveCount( 1 );
        await expect( block.connectionStatusPicker ).toHaveValue( String( attendeeId ) );
    } );

    test( "Submit_ConnectionStatusSelected_SavesStatus", async ( { page, pafa, block } ) => {
        const prospectId = await pafa.api.getIdByGuid( "DefinedValues", SystemGuid.DefinedValue.ConnectionStatusProspect );

        await openPageAsPerson( page, pafa.api, { pageId: pafa.blockPage.pageId, personId: pafa.person.id } );
        await block.connectionStatusPicker.selectOption( String( prospectId ) );
        await block.finish();

        const person = await pafa.api.getById( "People", pafa.person.id );
        expect( person.ConnectionStatusValueId ).toBe( prospectId );
    } );

    test( "Submit_ConnectionStatusLeftBlank_KeepsExistingStatus", async ( { page, pafa, block } ) => {
        const attendeeId = await pafa.api.getIdByGuid( "DefinedValues", SystemGuid.DefinedValue.ConnectionStatusAttendee );

        await openPageAsPerson( page, pafa.api, { pageId: pafa.blockPage.pageId, personId: pafa.person.id } );
        await block.connectionStatusPicker.selectOption( "" );
        await block.finish();

        const person = await pafa.api.getById( "People", pafa.person.id );
        expect( person.ConnectionStatusValueId, "A blank Connection Status should leave the person's status alone" ).toBe( attendeeId );
    } );
} );

async function setConnectionStatus( pafa: { api: import( "../../../shared/rockApi" ).RockApi; person: { id: number } }, statusGuid: string ): Promise<void> {
    await pafa.api.patch( "People", pafa.person.id, { ConnectionStatusValueId: await pafa.api.getIdByGuid( "DefinedValues", statusGuid ) } );
}
