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
import { Locator, Page } from "@playwright/test";
import { expectNoRockError, waitForPostback } from "../../../shared/rockBrowser";
import { pickPerson, setCheckBox } from "../../../shared/rockControls";
import { PluginGuid, expect, test } from "./stepsToCare.fixture";

// Traces are off: see actions.spec.ts.
test.use( { trace: "off" } );

/*
    Covers the manual plan's Care Configuration steps: the gear on the dashboard opens it;
    in each block (Care Workers, Care Note Templates, Category, Status) the "+" adds an item
    to the list and clicking an existing item edits it. Items the tests add are deleted
    afterwards. Note templates are added inactive: active ones appear as Quick Note buttons
    on every Care Dashboard on the site.
*/
test.describe( "Steps to Care: care configuration", () => {
    test( "Gear_OnDashboard_OpensCareConfiguration", async ( { page, stc } ) => {
        const dashboard = await stc.openDashboard( page );

        await dashboard.configurationButton.click();

        await page.waitForURL( new RegExp( `/page/${stc.pages.configuration.pageId}(\\?|$)` ) );
        await expectNoRockError( page );
        for ( const title of [ "Care Workers", "Care Note Templates", "Category", "Status" ] ) {
            await expect( page.locator( `${PanelTitleSelector}, ${ObsidianGridTitleSelector}`, { hasText: title } ).first() ).toBeVisible();
        }
    } );

    test( "CareWorkers_AddThenEdit_SavesWorker", async ( { page, stc } ) => {
        const block = page.locator( `#bid_${stc.pages.configuration.careWorkersBlockId}` );
        const workerRow = (): Locator => block.locator( "tbody tr", { hasText: stc.worker.fullName } );
        await stc.openConfiguration( page );
        await deleteRows( page, workerRow );

        try {
            const dialog = await openAdd( page, block );
            await pickPerson( page, dialog.locator( "[id$='_ppNewPerson']" ), stc.worker.fullName, stc.worker.id );
            await dialog.locator( "input[id$='_nreAgeRange_lower']" ).fill( "18" );
            await dialog.locator( "input[id$='_nreAgeRange_upper']" ).fill( "99" );
            await dialog.locator( "select[id$='_ddlGender']" ).selectOption( { label: "Female" } );
            await save( page, dialog );

            await expect( workerRow() ).toHaveCount( 1 );
            await expect( workerRow() ).toContainText( "Female" );

            const editDialog = await openEdit( page, workerRow() );
            await expect( editDialog.locator( "input[id$='_nreAgeRange_lower']" ) ).toHaveValue( "18" );
            await editDialog.locator( "select[id$='_ddlGender']" ).selectOption( { label: "Male" } );
            await save( page, editDialog );

            await expect( workerRow() ).toContainText( "Male" );
        }
        finally {
            await stc.openConfiguration( page );
            await deleteRows( page, workerRow );
        }
    } );

    test( "NoteTemplates_AddThenEdit_SavesTemplate", async ( { page, stc } ) => {
        const block = page.locator( `#bid_${stc.pages.configuration.noteTemplatesBlockId}` );
        const note = stc.uniqueDetails( "Template" );
        const edited = `${note} edited`;
        const templateRow = (): Locator => block.locator( "tbody tr", { hasText: note } );
        await stc.openConfiguration( page );

        try {
            const dialog = await openAdd( page, block );
            await dialog.locator( "input[id$='_tbIcon']" ).fill( "fa fa-flask" );
            await dialog.locator( "input[id$='_tbNote']" ).fill( note );
            await setCheckBox( page, dialog.locator( "input[id$='_cbActive']" ), false );
            await save( page, dialog );

            await expect( templateRow() ).toHaveCount( 1 );

            const editDialog = await openEdit( page, templateRow() );
            await expect( editDialog.locator( "input[id$='_tbNote']" ) ).toHaveValue( note );
            await editDialog.locator( "input[id$='_tbNote']" ).fill( edited );
            await save( page, editDialog );

            await expect( block.locator( "tbody tr", { hasText: edited } ) ).toHaveCount( 1 );
        }
        finally {
            await stc.openConfiguration( page );
            await deleteRows( page, () => block.locator( "tbody tr", { hasText: "KFS E2E Template" } ) );
        }
    } );

    for ( const list of [ { title: "Category", definedType: PluginGuid.categoryDefinedType }, { title: "Status", definedType: PluginGuid.statusDefinedType } ] ) {
        test( `${list.title}_AddThenEdit_SavesDefinedValue`, async ( { page, stc } ) => {
            const titleText = new RegExp( `^\\s*${list.title}\\s*$` );
            const block = page.locator( "[id^='bid_']", { has: page.locator( `${PanelTitleSelector}, ${ObsidianGridTitleSelector}`, { hasText: titleText } ) } ).first();
            const value = stc.uniqueDetails( `Cfg ${list.title}` );
            const definedTypeId = await stc.api.getIdByGuid( "DefinedTypes", list.definedType );
            await stc.openConfiguration( page );
            const ui = await definedValueListUi( page, block );
            const row = (): Locator => ui.row( value );

            try {
                const dialog = await ui.openAdd();
                await ui.valueBox( dialog ).fill( value );
                await ui.descriptionBox( dialog ).fill( "Added by the KFS end-to-end tests." );
                await ui.save( dialog );

                await expect( row() ).toHaveCount( 1 );

                const editDialog = await ui.openEdit( row() );
                await expect( ui.valueBox( editDialog ) ).toHaveValue( value );
                await ui.descriptionBox( editDialog ).fill( "Edited by the KFS end-to-end tests." );
                await ui.save( editDialog );

                await expect( row() ).toContainText( "Edited by the KFS end-to-end tests." );
            }
            finally {
                for ( const definedValue of await stc.api.query( "DefinedValues", `DefinedTypeId eq ${definedTypeId} and startswith(Value,'KFS E2E Cfg')` ) ) {
                    await stc.api.delete( "DefinedValues", definedValue.Id );
                }
            }
        } );
    }
} );

/** Title of a WebForms panel (the plugin's own blocks, and Rock 17's Defined Value List). */
const PanelTitleSelector = ".panel-title";

/** Title of an Obsidian grid (Rock 20's Defined Value List). */
const ObsidianGridTitleSelector = ".grid-obsidian .grid-title";

/** The steps to add and edit an item in one Defined Value List block. */
type DefinedValueListUi = {
    row( text: string ): Locator;
    openAdd(): Promise<Locator>;
    openEdit( row: Locator ): Promise<Locator>;
    valueBox( dialog: Locator ): Locator;
    descriptionBox( dialog: Locator ): Locator;
    save( dialog: Locator ): Promise<void>;
};

/*
    10/8/2026 - CLAUDE

    The Category and Status lists are Rock's own Defined Value List block, which the plugin
    places on its configuration page. Rock 20 replaced it with an Obsidian block under the
    same block type Guid: an Obsidian grid with a header Add button, and a modal opened by
    clicking a row. The block's markup decides which steps are used, so the tests run on
    Rock 17 (WebForms) and Rock 20 (Obsidian).

    Reason: Rock 20 converted the core Defined Value List block to Obsidian.
*/
/** Gets the add/edit steps for whichever version of the Defined Value List block is shown. */
async function definedValueListUi( page: Page, block: Locator ): Promise<DefinedValueListUi> {
    await expect( block, "Defined Value List block not found on the configuration page" ).toBeVisible();
    const isObsidian = await block.locator( ".grid-obsidian" ).count() > 0;

    if ( !isObsidian ) {
        return {
            row: text => block.locator( "tbody tr", { hasText: text } ),
            openAdd: () => openAdd( page, block ),
            openEdit: row => openEdit( page, row ),
            valueBox: dialog => dialog.locator( "input[id$='_tbValueName']" ),
            descriptionBox: dialog => dialog.locator( "textarea[id$='_tbValueDescription']" ),
            save: dialog => save( page, dialog )
        };
    }

    const openModal = async ( click: () => Promise<void> ): Promise<Locator> => {
        await click();
        const dialog = page.locator( ".modal:visible" ).last();
        await expect( dialog ).toBeVisible();
        return dialog;
    };

    return {
        row: text => block.locator( ".grid-row", { hasText: text } ),
        openAdd: () => openModal( () => block.locator( "button[title^='Add a new item']" ).first().click() ),
        openEdit: row => openModal( () => row.locator( ".grid-cell" ).nth( 1 ).click() ),
        valueBox: dialog => dialog.getByRole( "textbox", { name: "Value", exact: true } ),
        descriptionBox: dialog => dialog.getByRole( "textbox", { name: "Description", exact: true } ),
        save: async dialog => {
            await dialog.getByRole( "button", { name: "Save", exact: true } ).click();
            await expect( dialog ).toBeHidden();
            await expectNoRockError( page );
        }
    };
}

/** Clicks the block's "+" (grid footer Add) and returns the dialog it opens. */
async function openAdd( page: Page, block: Locator ): Promise<Locator> {
    await block.locator( "a[id$='_lbAdd']" ).first().click();
    await waitForPostback( page );
    const dialog = page.locator( ".modal:visible" ).last();
    await expect( dialog ).toBeVisible();
    return dialog;
}

/** Clicks an existing item (a grid row) and returns the edit dialog it opens. */
async function openEdit( page: Page, row: Locator ): Promise<Locator> {
    await row.locator( "td" ).nth( 1 ).click();
    await waitForPostback( page );
    const dialog = page.locator( ".modal:visible" ).last();
    await expect( dialog ).toBeVisible();
    return dialog;
}

async function save( page: Page, dialog: Locator ): Promise<void> {
    await dialog.locator( "a[id$='_serverSaveLink']" ).click();
    await waitForPostback( page );
    await expect( dialog ).toBeHidden();
    await expectNoRockError( page );
}

/** Deletes every row the locator matches with the grid's Delete button (confirming each). */
async function deleteRows( page: Page, rows: () => Locator ): Promise<void> {
    for ( let i = 0; i < 20 && await rows().count() > 0; i++ ) {
        await rows().first().locator( "a.grid-delete-button" ).click();
        await page.locator( ".bootbox .btn-primary, .modal-footer .btn-primary" ).filter( { visible: true } ).first().click();
        await waitForPostback( page );
        await page.waitForLoadState( "load" );
    }
}
