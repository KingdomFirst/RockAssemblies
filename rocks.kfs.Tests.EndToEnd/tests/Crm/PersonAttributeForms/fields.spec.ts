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
import { openPageAnonymously, openPageAsPerson } from "../../../shared/rockBrowser";
import { getLastCapturedEntity } from "../../../shared/testData/workflows";
import { expect, test } from "./personAttributeForms.fixture";

/*
    Covers the manual plan's "Testing - Fields & Groups" steps for form fields and the
    Person workflow entity: Person Field and Person Attribute values are saved, and the
    configured workflow runs.
*/
test.describe( "Person Attribute Forms Advanced: fields", () => {
    test( "PageLoad_LoggedInPerson_RendersPersonFieldsAndPersonAttribute", async ( { page, pafa, block } ) => {
        await openPageAsPerson( page, pafa.api, { pageId: pafa.blockPage.pageId, personId: pafa.person.id } );

        await expect( block.middleNameInput ).toBeVisible();
        await expect( block.emailInput ).toBeVisible();
        await expect( block.attributeInput( pafa.personAttributeId ) ).toBeVisible();
        await expect( block.nextButton ).toHaveText( /Finish/i );
    } );

    test( "PageLoad_Anonymous_ShowsLoginRequiredMessage", async ( { page, pafa, block } ) => {
        await openPageAnonymously( page, { pageId: pafa.blockPage.pageId } );

        /*
            Whether an anonymous visitor reaches the block depends on the site's View
            rules (rockbeta's External Website allows all users, so the block shows its
            login message); a site that denies anonymous View sends them to the login
            page. Either outcome proves no one can fill out the form without signing in.
        */
        const reachedBlock = await block.root.count() > 0;
        if ( reachedBlock ) {
            await expect( block.notification ).toContainText( "You need to login" );
            await expect( block.middleNameInput ).toHaveCount( 0 );
        }
        else {
            await expect( page ).toHaveURL( /login/i );
        }
    } );

    test( "Submit_PersonFieldsAndPersonAttribute_SavesValuesToPerson", async ( { page, pafa, block } ) => {
        const stamp = Date.now().toString();
        const middleName = `Mid${stamp}`;
        const email = `pafa.adult+${stamp}@kfs-e2e.invalid`;
        const attributeValue = `Attr ${stamp}`;

        await openPageAsPerson( page, pafa.api, { pageId: pafa.blockPage.pageId, personId: pafa.person.id } );
        await block.middleNameInput.fill( middleName );
        await block.emailInput.fill( email );
        await block.attributeInput( pafa.personAttributeId ).fill( attributeValue );
        await block.finish();

        const person = await pafa.api.getById( "People", pafa.person.id );
        expect( person.MiddleName ).toBe( middleName );
        expect( person.Email ).toBe( email );
        expect( await pafa.api.getAttributeValue( pafa.personAttributeId, pafa.person.id ) ).toBe( attributeValue );
    } );

    test( "Submit_Finished_ShowsConfirmationText", async ( { page, pafa, block } ) => {
        await openPageAsPerson( page, pafa.api, { pageId: pafa.blockPage.pageId, personId: pafa.person.id } );
        await block.finish();

        await expect( block.confirmation ).toContainText( "KFS E2E form complete." );
        await expect( block.middleNameInput ).toHaveCount( 0 );
    } );

    test( "Submit_PersonWorkflowEntity_LaunchesWorkflowWithPerson", async ( { page, pafa, block } ) => {
        await pafa.configure( {
            Workflow: pafa.workflows.person.guid,
            WorkflowEntity: "Person"
        } );

        await openPageAsPerson( page, pafa.api, { pageId: pafa.blockPage.pageId, personId: pafa.person.id } );
        await block.finish();

        const captured = await getLastCapturedEntity( pafa.api, pafa.workflows.person );
        expect( captured, "Workflow was not launched" ).not.toBeNull();
        // Set Attribute From Entity stores a Person as its primary PersonAlias.
        expect( captured?.entityTypeGuid ).toBe( pafa.entityTypeGuids.personAlias );
        expect( captured?.entityId ).toBe( pafa.person.primaryAliasId );
    } );

    test( "Submit_TwoForms_SavesValuesFromBothForms", async ( { page, pafa, block } ) => {
        await pafa.configure( { Forms: pafa.buildFormsJson( 2 ), DisplayProgressBar: "True" } );
        const stamp = Date.now().toString();

        await openPageAsPerson( page, pafa.api, { pageId: pafa.blockPage.pageId, personId: pafa.person.id } );

        // Form 1 gets fields 0 and 2 (Middle Name, attribute); form 2 gets field 1 (Email).
        await expect( block.nextButton ).toHaveText( /Next/i );
        await block.middleNameInput.fill( `Two${stamp}` );
        await block.attributeInput( pafa.personAttributeId ).fill( `Two ${stamp}` );
        await block.next();

        await expect( block.emailInput ).toBeVisible();
        await expect( block.previousButton ).toBeVisible();
        await block.emailInput.fill( `pafa.adult+two${stamp}@kfs-e2e.invalid` );
        await block.finish();

        const person = await pafa.api.getById( "People", pafa.person.id );
        expect( person.MiddleName ).toBe( `Two${stamp}` );
        expect( person.Email ).toBe( `pafa.adult+two${stamp}@kfs-e2e.invalid` );
        expect( await pafa.api.getAttributeValue( pafa.personAttributeId, pafa.person.id ) ).toBe( `Two ${stamp}` );
    } );

    test( "Submit_DonePageSet_RedirectsToDonePage", async ( { page, pafa, block } ) => {
        const parentPage = await pafa.api.getById( "Pages", ( await pafa.api.getById( "Pages", pafa.blockPage.pageId ) ).ParentPageId as number );
        await pafa.configure( { DonePage: ( parentPage.Guid as string ).toLowerCase() } );

        await openPageAsPerson( page, pafa.api, { pageId: pafa.blockPage.pageId, personId: pafa.person.id } );
        await block.finish( { expectRedirect: true } );

        await expect( page ).not.toHaveURL( new RegExp( `/page/${pafa.blockPage.pageId}(\\?|$)` ) );
    } );
} );
