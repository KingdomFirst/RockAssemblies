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

/**
 * Page object for RockWeb/Plugins/rocks_kfs/CheckIn/Manager/Locations.ascx (the
 * "Advanced Check-in Monitor" RockShop plugin). Selectors match on server control ID
 * suffixes, which are stable across Rock versions.
 *
 * Navigation items and people are <li> elements whose onclick posts the update panel
 * back, so clicking the row drives the block.
 */
export class CheckinManagerLocationsBlock {
    public readonly root: Locator;

    public constructor( private readonly page: Page, blockId: number ) {
        this.root = page.locator( `#bid_${blockId}` );
    }

    public get title(): Locator {
        return this.root.locator( ".checkin-manager .panel-title" );
    }

    public get warning(): Locator {
        return this.root.locator( "[id$='_nbWarning']" );
    }

    /** The attendance chart panel; Flot draws canvases into it. */
    public get chart(): Locator {
        return this.root.locator( "[id$='_pnlChart']" );
    }

    public get searchBox(): Locator {
        return this.root.locator( "input[id$='_tbSearch']" );
    }

    public get searchButton(): Locator {
        return this.root.locator( "[id$='_lbSearch']" );
    }

    /** The heading of the current navigation level; clicking it goes up a level. */
    public get navHeading(): Locator {
        return this.root.locator( "[id$='_pnlNavHeading']" );
    }

    public get moveAllButton(): Locator {
        return this.root.locator( "[id$='_lbMoveAll']" );
    }

    /** A navigation item (area, group or location) by its exact name. */
    public navItem( name: string ): Locator {
        return this.root.locator( "li.list-group-item.clickable" ).filter( { has: this.page.locator( ".content", { hasText: new RegExp( `^\\s*${escapeRegExp( name )}\\s*$` ) } ) } );
    }

    public currentCount( name: string ): Locator {
        return this.navItem( name ).locator( "[id$='_lblCurrentCount']" );
    }

    /** A checked-in person's row in a location. */
    public personRow( name: string | RegExp ): Locator {
        return this.root.locator( "li.list-group-item" ).filter( { has: this.page.locator( ".js-checkin-person-name", { hasText: name } ) } );
    }

    public get personRows(): Locator {
        return this.root.locator( "li.list-group-item:has(.js-checkin-person-name)" );
    }

    public get moveDialog(): Locator {
        return this.page.locator( "[id$='_dlgMoveLocation_modal_dialog_panel']" );
    }

    public get printLabelDialog(): Locator {
        return this.page.locator( "[id$='_dlgPrintLabel_modal_dialog_panel']" );
    }

    /** Clicks through navigation items by name, e.g. area, then group, then room. */
    public async navigate( ...names: string[] ): Promise<void> {
        for ( const name of names ) {
            const item = this.navItem( name );
            await expect( item, `Navigation item '${name}' not shown` ).toHaveCount( 1 );
            await item.locator( ".content" ).click();
            await waitForPostback( this.page );
            await expectNoRockError( this.page );
        }
    }

    public async search( value: string ): Promise<void> {
        await this.searchBox.fill( value );
        await this.searchButton.click();
        await waitForPostback( this.page );
        await expectNoRockError( this.page );
    }

    /** Clicks a person action (Checkout, Delete) and accepts Rock's confirmation. */
    public async personActionWithConfirm( name: string | RegExp, button: "_lbCheckOut" | "_lbRemoveAttendance" ): Promise<void> {
        await this.personRow( name ).locator( `[id$='${button}']` ).click();
        const confirm = this.page.locator( ".bootbox .modal-footer .btn-primary" );
        await expect( confirm, "Rock did not ask for confirmation" ).toBeVisible();
        await confirm.click();
        await waitForPostback( this.page );
        await expectNoRockError( this.page );
    }

    public async openMoveDialog( name: string | RegExp ): Promise<void> {
        await this.personRow( name ).locator( "[id$='_lbMovePerson']" ).click();
        await waitForPostback( this.page );
        await expect( this.moveDialog ).toBeVisible();
        await expectNoRockError( this.page );
    }

    public async openMoveAllDialog(): Promise<void> {
        await this.moveAllButton.click();
        await waitForPostback( this.page );
        await expect( this.moveDialog ).toBeVisible();
        await expectNoRockError( this.page );
    }

    /*
        10/13/2026 - CLAUDE

        The location picker loads its tree from Rock's v1 REST API (api/locations/
        getactivechildren), and Rock's v1 API refuses impersonated sessions: every call
        returns 401, even api/People/GetCurrentPerson. The tests sign in only by
        impersonation token (no passwords), so the tree cannot load here. completeMove
        therefore sets the picker's selection the way its tree does, in the picker's
        hidden id and name fields, and then uses the dialog's real Save. Everything the
        plugin does with the selection is still exercised; only Rock's own tree is not.

        Reason: Rock's v1 REST API rejects impersonated (rckipid) sessions.
    */

    /** In the open Move dialog, selects a location in the picker and saves the dialog. */
    public async completeMove( locationId: number, locationName: string ): Promise<void> {
        const picker = this.moveDialog.locator( ".picker" ).first();
        await expect( picker ).toBeVisible();
        await picker.evaluate( ( element, selection ) => {
            const setValue = ( selector: string, value: string ): void => {
                const input = element.querySelector<HTMLInputElement>( selector );
                if ( !input ) {
                    throw new Error( `Location picker has no ${selector}` );
                }
                input.value = value;
            };
            setValue( ".js-item-id-value", String( selection.id ) );
            setValue( ".js-item-name-value", selection.name );
            const label = element.querySelector( ".selected-names" );
            if ( label ) {
                label.textContent = selection.name;
            }
        }, { id: locationId, name: locationName } );

        await this.moveDialog.locator( "[id$='_dlgMoveLocation_serverSaveLink']" ).click();
        await waitForPostback( this.page );
        await expectNoRockError( this.page );
        await expect( this.moveDialog ).toBeHidden();
    }

    // #region Labels

    /** The label list in the Print Label dialog. */
    public get labelSelect(): Locator {
        return this.printLabelDialog.locator( "select[id$='_ddlLabelToPrint']" );
    }

    /** The label preview: an embedded PDF the server rendered (through labelary.com). */
    public get labelPreview(): Locator {
        return this.printLabelDialog.locator( ".pdfObject object" );
    }

    public async openPrintLabelDialog( name: string | RegExp ): Promise<void> {
        await this.personRow( name ).locator( "[id$='_lbPrintLabel']" ).click();
        await waitForPostback( this.page );
        await expect( this.printLabelDialog ).toBeVisible();
        await expectNoRockError( this.page );
    }

    // #endregion

    // #region Room settings (heading of a room)

    /** The heading's Open/Close toggle; shown to people who may close rooms. */
    public get headingToggle(): Locator {
        return this.root.locator( "[id$='_tglHeadingRoom']" );
    }

    /** The Open/Close toggle on a room in a navigation list. */
    public navToggle( name: string ): Locator {
        return this.navItem( name ).locator( "[id$='_tglRoom']" );
    }

    /** Clicks the Open or Close half of a Rock toggle and waits for the postback it fires. */
    public async setToggle( toggle: Locator, open: boolean ): Promise<void> {
        await toggle.locator( open ? ".js-toggle-on" : ".js-toggle-off" ).click();
        await waitForPostback( this.page );
        await expectNoRockError( this.page );
    }

    public get thresholdPanel(): Locator {
        return this.root.locator( "[id$='_pnlThreshold']" );
    }

    /** The threshold shown in the room heading. */
    public get thresholdLabel(): Locator {
        return this.thresholdPanel.locator( "[id$='_lThreshold']" );
    }

    /** Edits the room's threshold in its heading and saves it. */
    public async editThreshold( value: number ): Promise<void> {
        await this.thresholdPanel.locator( ".js-threshold-btn-edit" ).click();
        const input = this.thresholdPanel.locator( "input[id$='_nbThreshold']" );
        await expect( input ).toBeVisible();
        await input.fill( String( value ) );
        await this.thresholdPanel.locator( "[id$='_lbUpdateThreshold']" ).click();
        await waitForPostback( this.page );
        await expectNoRockError( this.page );
    }

    // #endregion
}

function escapeRegExp( value: string ): string {
    return value.replace( /[.*+?^${}()|[\]\\]/g, "\\$&" );
}
