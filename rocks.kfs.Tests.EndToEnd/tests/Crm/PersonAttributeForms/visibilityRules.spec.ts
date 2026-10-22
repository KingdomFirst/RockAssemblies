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
import { openPageAsPerson, waitForPostback, expectNoRockError } from "../../../shared/rockBrowser";
import { PersonFieldType, expect, test } from "./personAttributeForms.fixture";

const showValue = "show";

/*
    Field visibility rules (the filter icon on a field in Edit Forms and Fields). The form
    has the test attribute, then a dependent attribute with a rule: show it only while the
    test attribute equals "show". The block evaluates the rules when the form renders and
    when the test attribute's value changes (its text box posts back on change), and must
    not save a hidden field's value.
*/
test.describe( "Person Attribute Forms Advanced: field visibility rules", () => {
    test.beforeEach( async ( { pafa } ) => {
        await pafa.configure( {
            Forms: pafa.buildCustomFormJson( [
                { field: "attribute" },
                { field: "dependentAttribute", showWhenAttributeEquals: showValue }
            ] )
        } );
    } );

    test( "PageLoad_SavedValueMatchesRule_ShowsDependentField", async ( { page, pafa, block } ) => {
        await pafa.api.setAttributeValue( pafa.personAttributeId, pafa.person.id, showValue );

        await openPageAsPerson( page, pafa.api, { pageId: pafa.blockPage.pageId, personId: pafa.person.id } );

        await expect( block.attributeInput( pafa.personAttributeId ) ).toHaveValue( showValue );
        await expect( block.attributeInput( pafa.dependentAttributeId ) ).toBeVisible();
    } );

    test( "PageLoad_SavedValueDoesNotMatchRule_HidesDependentField", async ( { page, pafa, block } ) => {
        await openPageAsPerson( page, pafa.api, { pageId: pafa.blockPage.pageId, personId: pafa.person.id } );

        await expect( block.attributeInput( pafa.personAttributeId ) ).toBeVisible();
        await expect( block.attributeInput( pafa.dependentAttributeId ) ).toHaveCount( 0 );
    } );

    test( "Edit_ValueChangedToMatchRule_ShowsDependentFieldAndSavesIt", async ( { page, pafa, block } ) => {
        const dependentValue = `Dep ${Date.now()}`;

        await openPageAsPerson( page, pafa.api, { pageId: pafa.blockPage.pageId, personId: pafa.person.id } );
        await expect( block.attributeInput( pafa.dependentAttributeId ) ).toHaveCount( 0 );

        await changeTestAttribute( page, block, pafa.personAttributeId, showValue );

        await expect( block.attributeInput( pafa.dependentAttributeId ), "The dependent field should appear once the rule matches" ).toBeVisible();
        await block.attributeInput( pafa.dependentAttributeId ).fill( dependentValue );
        await block.finish();

        expect( await pafa.api.getAttributeValue( pafa.personAttributeId, pafa.person.id ) ).toBe( showValue );
        expect( await pafa.api.getAttributeValue( pafa.dependentAttributeId, pafa.person.id ) ).toBe( dependentValue );
    } );

    test( "Edit_ValueChangedAwayFromRule_HidesDependentFieldAndDoesNotSaveIt", async ( { page, pafa, block } ) => {
        test.info().annotations.push( { type: "known-issue", description: "PAFA saves a value typed into a field that a visibility rule then hides (block's FieldVisibilityWrapper; since 2022)." } );
        await pafa.api.setAttributeValue( pafa.personAttributeId, pafa.person.id, showValue );

        await openPageAsPerson( page, pafa.api, { pageId: pafa.blockPage.pageId, personId: pafa.person.id } );
        await block.attributeInput( pafa.dependentAttributeId ).fill( "Should not save" );
        await changeTestAttribute( page, block, pafa.personAttributeId, "hide" );

        await expect( block.attributeInput( pafa.dependentAttributeId ), "The dependent field should disappear once the rule no longer matches" ).toHaveCount( 0 );
        await block.finish();

        expect( await pafa.api.getAttributeValue( pafa.dependentAttributeId, pafa.person.id ) ?? "", "A hidden field's value must not be saved" ).toBe( "" );
    } );

    /*
        10/13/2026 - CLAUDE

        Fails on rockbeta as of v2.7: hiding a *person* field (here Middle Name) by a rule
        crashes the page. The block's own FieldVisibilityWrapper (end of
        PersonAttributeForms.ascx.cs) is keyed by FormFieldId, which for a person field is
        the PersonFieldType number (Middle Name = 14). Its EditValue then reads and writes
        the control through AttributeCache.Get( 14 ), whatever unrelated attribute has that
        Id on the site, and throws or leaves the value in place. Present since the wrapper
        was added in 2022, so it is not a Rock 20 regression. Attribute fields (above) are
        keyed by their real attribute Id and work.

        Reason: Pins a real plugin bug; it should pass once the block handles person fields.
    */
    test( "PageLoad_PersonFieldHiddenByRule_RendersFormWithoutTheField", async ( { page, pafa, block } ) => {
        test.info().annotations.push( { type: "known-issue", description: "PAFA crashes when a visibility rule hides a person field (block's FieldVisibilityWrapper keys person fields by field-type number; since 2022)." } );
        await pafa.configure( {
            Forms: pafa.buildCustomFormJson( [
                { field: "attribute" },
                { field: PersonFieldType.MiddleName, showWhenAttributeEquals: showValue }
            ] )
        } );

        await openPageAsPerson( page, pafa.api, { pageId: pafa.blockPage.pageId, personId: pafa.person.id } );

        await expect( block.attributeInput( pafa.personAttributeId ) ).toBeVisible();
        await expect( block.middleNameInput ).toHaveCount( 0 );
    } );
} );

/** Types a new value into a test attribute's text box and leaves it, which posts back and re-runs the rules. */
async function changeTestAttribute( page: import( "@playwright/test" ).Page, block: import( "./personAttributeFormsBlock" ).PersonAttributeFormsBlock, attributeId: number, value: string ): Promise<void> {
    await block.attributeInput( attributeId ).fill( value );
    await block.attributeInput( attributeId ).press( "Tab" );
    await waitForPostback( page );
    await expectNoRockError( page );
}
