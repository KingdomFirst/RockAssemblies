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
import { Locator, Page, expect } from "@playwright/test";
import { expectNoRockError, waitForPostback } from "../../../shared/rockBrowser";

/** CSS class on the confirmation text the fixture configures, so the tests can find it. */
export const confirmationCssClass = "kfs-e2e-confirmation";

/**
 * Page object for RockWeb/Plugins/rocks_kfs/Crm/PersonAttributeForms.ascx.
 * Selectors match on the server control ID suffix (e.g. "_tbMiddleName"), which is
 * stable across Rock versions even when the generated ClientID prefix changes.
 */
export class PersonAttributeFormsBlock {
    public readonly root: Locator;

    public constructor( private readonly page: Page, blockId: number ) {
        this.root = page.locator( `#bid_${blockId}` );
    }

    // #region View mode

    public get middleNameInput(): Locator {
        return this.root.locator( "input[id$='_tbMiddleName']" );
    }

    public get emailInput(): Locator {
        return this.root.locator( "input[id$='_tbEmail']" );
    }

    public attributeInput( attributeId: number ): Locator {
        return this.root.locator( `input[id$='_attribute_field_${attributeId}']` );
    }

    public get familyMemberPicker(): Locator {
        return this.root.locator( "select[id$='_ddlFamilyMembers']" );
    }

    public get notification(): Locator {
        return this.root.locator( "[id$='_nbMain']" );
    }

    public get confirmation(): Locator {
        return this.root.locator( `.${confirmationCssClass}` );
    }

    public get nextButton(): Locator {
        return this.root.locator( "[id$='_lbNext']" );
    }

    public get previousButton(): Locator {
        return this.root.locator( "[id$='_lbPrev']" );
    }

    public get title(): Locator {
        return this.root.locator( "h1, h2, h3, .panel-title" ).first();
    }

    /** Clicks Next and waits for the next form to render. */
    public async next(): Promise<void> {
        await this.nextButton.click();
        await waitForPostback( this.page );
        await expectNoRockError( this.page );
    }

    /** Clicks Finish and waits for the confirmation text (or, with a Done Page, the redirect). */
    public async finish( options: { expectRedirect?: boolean } = {} ): Promise<void> {
        await expect( this.nextButton ).toHaveText( /Finish/i );
        const startUrl = this.page.url();
        await this.nextButton.click();

        if ( options.expectRedirect ) {
            await this.page.waitForURL( url => url.toString() !== startUrl );
            await this.page.waitForLoadState( "load" );
        }
        else {
            await waitForPostback( this.page );
            await expect( this.confirmation, "Confirmation text did not appear after Finish" ).toBeVisible();
        }

        await expectNoRockError( this.page );
    }

    // #endregion

    // #region Edit mode ("Edit Forms and Fields")

    /** Opens the block's custom settings dialog through its block configuration link. */
    public async openEditFormsAndFields(): Promise<void> {
        const editLink = this.root.locator( ".block-configuration a.edit" ).first();
        await expect( editLink, "No 'Edit Forms and Fields' link; does the test person have Edit on the block?" ).toHaveCount( 1 );

        // The configuration bar is only shown on hover/toggle; clicking the element directly fires its postback.
        await editLink.evaluate( ( element: HTMLElement ) => element.click() );
        await waitForPostback( this.page );
        await expect( this.editModal ).toBeVisible();
        await expectNoRockError( this.page );
    }

    public get editModal(): Locator {
        return this.page.locator( "[id$='_mdEdit_modal_dialog_panel']" );
    }

    public get workflowEntityDropDown(): Locator {
        return this.editModal.locator( "select[id$='_ddlWorkflowEntity']" );
    }

    public async saveEditFormsAndFields(): Promise<void> {
        await this.editModal.locator( "[id$='_mdEdit_serverSaveLink']" ).click();
        await waitForPostback( this.page );
        await expectNoRockError( this.page );
    }

    // #endregion
}
