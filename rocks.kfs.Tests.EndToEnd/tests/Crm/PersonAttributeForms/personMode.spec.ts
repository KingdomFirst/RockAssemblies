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
import { openPageAsPerson, waitForPostback } from "../../../shared/rockBrowser";
import { expect, test } from "./personAttributeForms.fixture";

/*
    Covers the manual plan's setup step: "Verify 'Select a Family Member' drop down
    displays and functions as expected with each combination of Display Family Member
    Picker/Person Mode", plus the Person page parameter rules each mode enforces.
*/
test.describe( "Person Attribute Forms Advanced: person mode", () => {
    const pickerMatrix = [
        { personMode: "Family Members", displayPicker: true, isPickerShown: true },
        { personMode: "Family Members", displayPicker: false, isPickerShown: false },
        { personMode: "Anyone", displayPicker: true, isPickerShown: true },
        { personMode: "Anyone", displayPicker: false, isPickerShown: false },
        { personMode: "Logged in Person only", displayPicker: true, isPickerShown: false },
        { personMode: "Logged in Person only", displayPicker: false, isPickerShown: false }
    ];

    for ( const combination of pickerMatrix ) {
        const scenario = `${combination.personMode.replace( /\s/g, "" )}ModeDisplayPicker${combination.displayPicker ? "On" : "Off"}`;
        const expected = combination.isPickerShown ? "ShowsFamilyMemberPicker" : "HidesFamilyMemberPicker";

        test( `PageLoad_${scenario}_${expected}`, async ( { page, pafa, block } ) => {
            await pafa.configure( {
                PersonMode: combination.personMode,
                DisplayFamilyMemberPicker: combination.displayPicker ? "True" : "False"
            } );

            await openPageAsPerson( page, pafa.api, { pageId: pafa.blockPage.pageId, personId: pafa.person.id } );

            if ( combination.isPickerShown ) {
                await expect( block.familyMemberPicker ).toBeVisible();
                await expect( block.familyMemberPicker.locator( "option", { hasText: pafa.familyMember.fullName } ) ).toHaveCount( 1 );
            }
            else {
                await expect( block.familyMemberPicker ).toHaveCount( 0 );
            }
        } );
    }

    test( "SelectFamilyMember_FamilyMembersMode_ReloadsFormForSelectedPerson", async ( { page, pafa, block } ) => {
        await pafa.configure( { PersonMode: "Family Members", DisplayFamilyMemberPicker: "True" } );
        const middleName = `Kid${Date.now()}`;

        await openPageAsPerson( page, pafa.api, { pageId: pafa.blockPage.pageId, personId: pafa.person.id } );
        await block.familyMemberPicker.selectOption( { label: pafa.familyMember.fullName } );
        await page.waitForURL( new RegExp( `Person=${pafa.familyMember.guid}`, "i" ) );
        await waitForPostback( page );

        await block.middleNameInput.fill( middleName );
        await block.finish();

        expect( ( await pafa.api.getById( "People", pafa.familyMember.id ) ).MiddleName ).toBe( middleName );
        expect( ( await pafa.api.getById( "People", pafa.person.id ) ).MiddleName ?? "" ).toBe( "" );
    } );

    test( "Submit_FamilyMembersModeWithFamilyMemberGuid_SavesToFamilyMember", async ( { page, pafa, block } ) => {
        await pafa.configure( { PersonMode: "Family Members" } );
        const middleName = `Fam${Date.now()}`;

        await openPageAsPerson( page, pafa.api, { pageId: pafa.blockPage.pageId, personId: pafa.person.id, parameters: { Person: pafa.familyMember.guid } } );
        await expect( block.notification ).toBeHidden();
        await block.middleNameInput.fill( middleName );
        await block.finish();

        expect( ( await pafa.api.getById( "People", pafa.familyMember.id ) ).MiddleName ).toBe( middleName );
        expect( ( await pafa.api.getById( "People", pafa.person.id ) ).MiddleName ?? "" ).toBe( "" );
    } );

    test( "Submit_FamilyMembersModeWithNonFamilyGuid_WarnsAndSavesToCurrentPerson", async ( { page, pafa, block } ) => {
        await pafa.configure( { PersonMode: "Family Members" } );
        const middleName = `Out${Date.now()}`;

        await openPageAsPerson( page, pafa.api, { pageId: pafa.blockPage.pageId, personId: pafa.outsider.id, parameters: { Person: pafa.person.guid } } );
        await expect( block.notification ).toContainText( "You must be a family member" );
        await block.middleNameInput.fill( middleName );
        await block.finish();

        expect( ( await pafa.api.getById( "People", pafa.outsider.id ) ).MiddleName ).toBe( middleName );
        expect( ( await pafa.api.getById( "People", pafa.person.id ) ).MiddleName ?? "" ).toBe( "" );
    } );

    test( "Submit_LoggedInPersonOnlyModeWithPersonGuid_WarnsAndSavesToCurrentPerson", async ( { page, pafa, block } ) => {
        await pafa.configure( { PersonMode: "Logged in Person only" } );
        const middleName = `Self${Date.now()}`;

        await openPageAsPerson( page, pafa.api, { pageId: pafa.blockPage.pageId, personId: pafa.person.id, parameters: { Person: pafa.familyMember.guid } } );
        await expect( block.notification ).toContainText( "Logged in Person Only mode" );
        await block.middleNameInput.fill( middleName );
        await block.finish();

        expect( ( await pafa.api.getById( "People", pafa.person.id ) ).MiddleName ).toBe( middleName );
        expect( ( await pafa.api.getById( "People", pafa.familyMember.id ) ).MiddleName ?? "" ).toBe( "" );
    } );

    test( "Submit_AnyoneModeWithNonFamilyGuid_SavesToThatPerson", async ( { page, pafa, block } ) => {
        await pafa.configure( { PersonMode: "Anyone" } );
        const middleName = `Any${Date.now()}`;

        await openPageAsPerson( page, pafa.api, { pageId: pafa.blockPage.pageId, personId: pafa.outsider.id, parameters: { Person: pafa.person.guid } } );
        await expect( block.notification ).toBeHidden();
        await block.middleNameInput.fill( middleName );
        await block.finish();

        expect( ( await pafa.api.getById( "People", pafa.person.id ) ).MiddleName ).toBe( middleName );
        expect( ( await pafa.api.getById( "People", pafa.outsider.id ) ).MiddleName ?? "" ).toBe( "" );
    } );
} );
