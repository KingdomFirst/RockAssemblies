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
import { asEnumValue } from "../../../shared/rockApi";
import { openPageAsPerson, waitForPostback } from "../../../shared/rockBrowser";
import { SignatureDocumentText, getSignatureDocuments } from "../../../shared/testData/signatures";
import { defaultDisableFormWarningText, expect, test } from "./personAttributeForms.fixture";

/** Rock's AgeClassification enum, in value order. */
const AgeClassifications = [ "Unknown", "Adult", "Child" ] as const;

/*
    The signature document settings added in v2.6: Signature Document Template, Disable
    Form for Children and Disable Form Warning Text. The fixture's template uses Rock's own
    electronic signature (typed, no provider, no completion email), so signing sends
    nothing. Signing makes Rock render a PDF on the server.
*/
test.describe( "Person Attribute Forms Advanced: signature document", () => {
    test( "Submit_SignatureTemplate_ShowsDocumentThenSavesSignedDocument", async ( { page, pafa, block } ) => {
        const middleName = `Sig${Date.now()}`;
        await pafa.configure( { SignatureDocumentTemplate: String( pafa.signatureTemplateId ) } );

        await openPageAsPerson( page, pafa.api, { pageId: pafa.blockPage.pageId, personId: pafa.person.id } );
        await expect( block.nextButton, "With a signature template the last form leads to signing, not Finish" ).toHaveText( /Next/i );
        await block.middleNameInput.fill( middleName );
        await block.nextButton.click();
        await waitForPostback( page );

        await expect( block.signaturePanel ).toBeVisible();
        await expect( block.signatureDocumentFrame ).toHaveAttribute( "srcdoc", new RegExp( SignatureDocumentText.replace( /\./g, "\\." ) ) );
        await expect( block.signatureDocumentFrame ).toHaveAttribute( "srcdoc", new RegExp( pafa.person.fullName ) );

        // Values are held until the document is signed.
        expect( ( await pafa.api.getById( "People", pafa.person.id ) ).MiddleName ?? "" ).toBe( "" );

        await block.signAndComplete( pafa.person.fullName );

        const documents = await getSignatureDocuments( pafa.api, pafa.signatureTemplateId );
        expect( documents, "Signing should create one signature document" ).toHaveLength( 1 );
        expect( documents[ 0 ].AppliesToPersonAliasId ).toBe( pafa.person.primaryAliasId );
        expect( documents[ 0 ].SignedName ).toBe( pafa.person.fullName );
        expect( documents[ 0 ].BinaryFileId, "Signing should store the signed PDF" ).toBeTruthy();
        expect( ( await pafa.api.getById( "People", pafa.person.id ) ).MiddleName ).toBe( middleName );
    } );

    test( "PageLoad_ChildWithDisableFormForChildren_ShowsWarningInsteadOfForm", async ( { page, pafa, block } ) => {
        await expectChild( pafa );
        const warning = `KFS E2E child warning ${Date.now()}`;
        await pafa.configure( {
            SignatureDocumentTemplate: String( pafa.signatureTemplateId ),
            DisableFormforChildren: "True",
            DisableFormWarningText: `<p>${warning}</p>`
        } );

        await openPageAsPerson( page, pafa.api, { pageId: pafa.blockPage.pageId, personId: pafa.familyMember.id } );

        await expect( block.notification ).toContainText( warning );
        await expect( block.middleNameInput ).toHaveCount( 0 );
    } );

    test( "PageLoad_ChildWithDefaultWarningText_ShowsDefaultWarning", async ( { page, pafa, block } ) => {
        await expectChild( pafa );
        await pafa.configure( { SignatureDocumentTemplate: String( pafa.signatureTemplateId ), DisableFormforChildren: "True" } );

        await openPageAsPerson( page, pafa.api, { pageId: pafa.blockPage.pageId, personId: pafa.familyMember.id } );

        await expect( block.notification ).toContainText( defaultDisableFormWarningText.replace( /<\/?p>/g, "" ) );
        await expect( block.middleNameInput ).toHaveCount( 0 );
    } );

    test( "PageLoad_ChildWithDisableFormForChildrenOff_ShowsForm", async ( { page, pafa, block } ) => {
        await expectChild( pafa );
        await pafa.configure( { SignatureDocumentTemplate: String( pafa.signatureTemplateId ), DisableFormforChildren: "False" } );

        await openPageAsPerson( page, pafa.api, { pageId: pafa.blockPage.pageId, personId: pafa.familyMember.id } );

        await expect( block.middleNameInput ).toBeVisible();
    } );

    test( "PageLoad_AdultWithDisableFormForChildren_ShowsForm", async ( { page, pafa, block } ) => {
        await pafa.configure( { SignatureDocumentTemplate: String( pafa.signatureTemplateId ), DisableFormforChildren: "True" } );

        await openPageAsPerson( page, pafa.api, { pageId: pafa.blockPage.pageId, personId: pafa.person.id } );

        await expect( block.middleNameInput ).toBeVisible();
        await expect( block.notification ).toBeHidden();
    } );

    test( "PageLoad_ChildWithoutSignatureTemplate_ShowsForm", async ( { page, pafa, block } ) => {
        await expectChild( pafa );
        // Disable Form for Children applies only when the form needs a signature.
        await pafa.configure( { DisableFormforChildren: "True" } );

        await openPageAsPerson( page, pafa.api, { pageId: pafa.blockPage.pageId, personId: pafa.familyMember.id } );

        await expect( block.middleNameInput ).toBeVisible();
    } );
} );

/** The child tests rely on Rock classifying the fixture's family child as a Child. */
async function expectChild( pafa: { api: import( "../../../shared/rockApi" ).RockApi; familyMember: { id: number; fullName: string } } ): Promise<void> {
    const record = await pafa.api.getById( "People", pafa.familyMember.id );
    expect( AgeClassifications[ asEnumValue( record.AgeClassification, AgeClassifications ) ], `${pafa.familyMember.fullName} should be classified as a Child` ).toBe( "Child" );
}
