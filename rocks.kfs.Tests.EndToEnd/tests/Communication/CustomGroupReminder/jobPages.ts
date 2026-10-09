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
import { expectNoRockError } from "../../../shared/rockBrowser";

/**
 * Page object for Rock's Scheduled Job Detail block (Obsidian), which opens directly in its
 * edit form. Fields, including the job type's own settings, are found by their label.
 */
export class JobDetailPage {
    public readonly root: Locator;

    public constructor( private readonly page: Page, blockId: number ) {
        this.root = page.locator( `#bid_${blockId}` );
    }

    /** The form group of a field, by its label text (labels can also hold a help icon). */
    public field( label: string ): Locator {
        return this.root.locator( ".form-group" ).filter( { has: this.page.locator( "label.control-label", { hasText: new RegExp( `^\\s*${label}\\s*$` ) } ) } ).first();
    }

    /** Waits for the Obsidian block to render its form. */
    public async waitForForm(): Promise<void> {
        await expect( this.root.getByRole( "button", { name: "Save" } ) ).toBeVisible();
        await expectNoRockError( this.page );
    }

    /** The text shown in a drop down field (e.g. "Email"). */
    public dropDownValue( label: string ): Locator {
        return this.field( label ).locator( ".ant-select-selection-item" );
    }

    /** Picks an option in a drop down field, the way a user does. */
    public async chooseOption( label: string, option: string ): Promise<void> {
        await this.field( label ).locator( ".ant-select-selector" ).click();
        await this.page.locator( ".ant-select-dropdown:visible .ant-select-item-option", { hasText: new RegExp( `^${option}$` ) } ).click();
        await expect( this.dropDownValue( label ) ).toHaveText( option );
    }

    /** A text box by its accessible name (the label, followed by any help icon's text). */
    public textBox( label: string ): Locator {
        return this.root.getByRole( "textbox", { name: new RegExp( `^\\s*${label}(\\s|$)` ) } ).first();
    }

    public async save(): Promise<void> {
        const saved = this.page.waitForResponse( response => response.url().includes( "/api/v2/BlockActions/" ) && response.request().method() === "POST" && /save/i.test( response.url() ) );
        await this.root.getByRole( "button", { name: "Save" } ).click();
        const response = await saved;
        expect( response.ok(), `Saving the job failed (HTTP ${response.status()})` ).toBe( true );
        await this.page.waitForLoadState( "load" );
        await expectNoRockError( this.page );
    }
}

/** Page object for Rock's Scheduled Job List block (Obsidian) on Jobs Administration. */
export class JobListPage {
    public readonly root: Locator;

    public constructor( private readonly page: Page, blockId: number ) {
        this.root = page.locator( `#bid_${blockId}` );
    }

    public row( jobName: string ): Locator {
        return this.root.locator( ".grid-row" ).filter( { has: this.page.locator( ".grid-cell", { hasText: new RegExp( `^${jobName}$` ) } ) } ).first();
    }

    /*
        10/7/2026 - CLAUDE

        The Obsidian grid only draws the rows near the visible part of the list (24 of 68
        jobs on rockbeta), ordered by last run. Once other jobs had run more recently, the
        test job's row was not on the page at all. So, like a user, type the job's name in
        the grid's quick-filter Search box first.

        Reason: Rows outside the rendered window do not exist in the page.
    */
    /*
        10/8/2026 - CLAUDE

        Rock 20's grid dropped the grid-quick-filter class and the toggle button's title. Both
        versions still have a text box named "Search" next to the toggle button that reveals
        it, so the box is found by its accessible name and the toggle as the nearest button
        around it.

        Reason: Rock 20 changed the Obsidian grid's quick-filter markup.
    */
    /** Narrows the list to the job with the grid's Search box and returns its row. */
    public async find( jobName: string ): Promise<Locator> {
        await this.root.locator( ".grid-row" ).first().waitFor();
        const search = this.root.getByRole( "textbox", { name: "Search", exact: true } ).first();
        if ( !await search.isVisible() ) {
            await search.locator( "xpath=ancestor::*[.//button][1]//button" ).first().click();
        }
        await search.fill( jobName );
        const row = this.row( jobName );
        await expect( row, `Job '${jobName}' is not in the job list` ).toBeVisible();
        return row;
    }

    /** Clicks the job's Run Now button and waits for Rock to accept the request. */
    public async runNow( jobName: string ): Promise<void> {
        const row = await this.find( jobName );
        const accepted = this.page.waitForResponse( response => /\/RunNow(\?|$)/i.test( response.url() ) && response.request().method() === "POST" );
        await row.locator( "button[title='Run Now']" ).click();
        const response = await accepted;
        expect( response.ok(), `Run Now failed (HTTP ${response.status()})` ).toBe( true );
    }
}
