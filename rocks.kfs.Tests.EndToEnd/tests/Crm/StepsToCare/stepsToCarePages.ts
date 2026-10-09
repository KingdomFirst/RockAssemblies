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
import { pickPerson, setCheckBox } from "../../../shared/rockControls";

/** Page object for RockWeb/Plugins/rocks_kfs/StepsToCare/CareDashboard.ascx. */
export class CareDashboardBlock {
    public readonly root: Locator;

    public constructor( public readonly page: Page, blockId: number ) {
        this.root = page.locator( `#bid_${blockId}` );
    }

    /** Filters the main grid to one status with its filter panel (Rock GridFilter, opened by "Filter Options"). */
    public async showOnlyStatus( statusValueId: number ): Promise<void> {
        await this.openFilter();
        for ( const box of await this.root.locator( "input[type='checkbox'][id*='_dvpStatus_']" ).all() ) {
            const wanted = await box.getAttribute( "value" ) === String( statusValueId );
            if ( await box.isChecked() !== wanted ) {
                await box.locator( "xpath=ancestor::label[1]" ).click();
            }
        }
        await this.root.locator( "a[id$='_lbFilter']" ).first().click();
        await waitForPostback( this.page );
        await this.page.waitForLoadState( "load" );
    }

    /** Clears the main grid's filter, which Rock keeps as the person's preference between visits. */
    public async clearFilter(): Promise<void> {
        await this.openFilter();
        await this.root.locator( "a[id$='__lbClearFilter']" ).first().click();
        await waitForPostback( this.page );
        await this.page.waitForLoadState( "load" );
    }

    // The main grid's filter is the first on the block (the Follow Up grid has its own).
    private async openFilter(): Promise<void> {
        await this.waitForAnimations();
        const applyButton = this.root.locator( "a[id$='_lbFilter']" ).first();
        if ( !await applyButton.isVisible() ) {
            await this.root.locator( "button.btn-filter-toggle" ).first().click();
            await expect( applyButton ).toBeVisible();
        }
        // The panel slides open; its buttons are not clickable until it stops.
        await this.waitForAnimations();
    }

    private async waitForAnimations(): Promise<void> {
        await this.page.waitForFunction( () => {
            const jq = ( window as unknown as { jQuery?: ( selector: string ) => { length: number } } ).jQuery;
            return !jq || jq( ":animated" ).length === 0;
        } );
    }

    public get enterNeedButton(): Locator {
        return this.root.locator( "a[id$='_btnAdd']" );
    }

    public get notificationButton(): Locator {
        return this.root.locator( "a[id$='_lbNotificationType']" );
    }

    public get configurationButton(): Locator {
        return this.root.locator( "a[id$='_lbCareConfigure']" );
    }

    /** The need's row in the main grid, found by its unique description. */
    public row( details: string ): Locator {
        return this.root.locator( "table[id$='_gList'] tbody tr", { hasText: details } ).first();
    }

    /** The need's row in the Follow Up grid (needs in Follow Up or Snoozed status). */
    public followUpRow( details: string ): Locator {
        return this.root.locator( "table[id$='_gFollowUp'] tbody tr", { hasText: details } ).first();
    }

    /** Care need Id of a grid row (the grid's data key). */
    public async needId( row: Locator ): Promise<number> {
        return parseInt( await row.getAttribute( "datakey" ) ?? "0", 10 );
    }

    /** The "+" before the person's name that shows the family members' needs. */
    public familyToggle( row: Locator ): Locator {
        return row.locator( "a[id^='toggleLink']" );
    }

    /** Rows of the family members' needs under a parent need (hidden until the "+" is clicked). */
    public familyRows( parentNeedId: number ): Locator {
        return this.root.locator( `table[id$='_gList'] tbody tr.hasParentNeed${parentNeedId}` );
    }

    public careTouches( row: Locator ): Locator {
        return row.locator( "td" ).nth( 5 );
    }

    /** Opens the row's Actions menu and clicks the action. */
    public async action( row: Locator, name: string ): Promise<void> {
        await row.locator( "a.dropdown-toggle", { hasText: "Actions" } ).click();
        await row.locator( ".dropdown-menu a", { hasText: name } ).click();
        await waitForPostback( this.page );
        await this.page.waitForLoadState( "load" );
        await expectNoRockError( this.page );
    }

    /**
     * Clicks an action that sends the browser to another page (a core Rock page such as
     * Prayer Request Detail) and returns that page's address. The core page itself is not
     * under test, and the test role may not be allowed to view it.
     */
    public async actionNavigation( row: Locator, name: string ): Promise<string> {
        const dashboardPath = new URL( this.page.url() ).pathname;
        const navigation = this.page.waitForRequest( request => request.isNavigationRequest() && new URL( request.url() ).pathname.toLowerCase() !== dashboardPath.toLowerCase(), { timeout: 60_000 } );
        await row.locator( "a.dropdown-toggle", { hasText: "Actions" } ).click();
        await row.locator( ".dropdown-menu a", { hasText: name } ).click();
        return ( await navigation ).url();
    }

    public async actionNames( row: Locator ): Promise<string[]> {
        return ( await row.locator( ".dropdown-menu a" ).allTextContents() ).map( text => text.trim() );
    }

    /** Clicks a Quick Note button (e.g. "Called") on the row. */
    public async quickNote( row: Locator, title: string ): Promise<void> {
        await row.locator( `a.btn[title='${title}']` ).click();
        await waitForPostback( this.page );
        await expectNoRockError( this.page );
    }

    /** The open dialog (Rock modal) on the page. */
    public get dialog(): Locator {
        return this.page.locator( ".modal:visible" ).last();
    }

    public async openMakeNote( row: Locator ): Promise<void> {
        await row.locator( "a.btn-make-note" ).click();
        await waitForPostback( this.page );
        await expect( this.dialog ).toContainText( "Quick Notes" );
    }

    /** Clicks a row (not a button in it), which opens the need in Care Entry. */
    public async openNeed( row: Locator ): Promise<void> {
        await row.locator( "td" ).nth( 2 ).click();
        await this.page.waitForURL( /CareNeedId=/i );
        await this.page.waitForLoadState( "load" );
        await expectNoRockError( this.page );
    }
}

/** Page object for RockWeb/Plugins/rocks_kfs/StepsToCare/CareEntry.ascx. */
export class CareEntryBlock {
    public readonly root: Locator;

    public constructor( public readonly page: Page, blockId: number ) {
        this.root = page.locator( `#bid_${blockId}` );
    }

    public get requestorPicker(): Locator {
        return this.root.locator( "[id$='_ppPerson']" );
    }

    public get category(): Locator {
        return this.root.locator( "select[id$='_dvpCategory']" );
    }

    public get status(): Locator {
        return this.root.locator( "select[id$='_dvpStatus']" );
    }

    public get details(): Locator {
        return this.root.locator( "textarea[id$='_dtbDetailsText']" );
    }

    public get customFollowUp(): Locator {
        return this.root.locator( "input[id$='_cbCustomFollowUp']" );
    }

    public get followUpAfterDays(): Locator {
        return this.root.locator( "input[id$='_numbRepeatDays']" );
    }

    public get followUpRepeatTimes(): Locator {
        return this.root.locator( "input[id$='_numbRepeatTimes']" );
    }

    public get includeFamily(): Locator {
        return this.root.locator( "input[id$='_cbIncludeFamily']" );
    }

    public get addPersonPicker(): Locator {
        return this.root.locator( "[id$='_ppAddPerson']" );
    }

    public get assignedGrid(): Locator {
        return this.root.locator( "table[id$='_gAssignedPersons']" );
    }

    public async pickRequestor( searchName: string, personId: number ): Promise<void> {
        await pickPerson( this.page, this.requestorPicker, searchName, personId );
    }

    public async chooseCategory( categoryId: number ): Promise<void> {
        await this.category.selectOption( String( categoryId ) );
        await waitForPostback( this.page );
    }

    /** Adds a person to the assigned care workers with the Assigned panel's "Add Person" picker. */
    public async assignPerson( searchName: string, personId: number ): Promise<void> {
        await pickPerson( this.page, this.addPersonPicker, searchName, personId );
        await expect( this.assignedGrid ).toContainText( searchName );
    }

    public async setCustomFollowUp( days: number, times: number ): Promise<void> {
        await setCheckBox( this.page, this.customFollowUp, true );
        await this.followUpAfterDays.fill( String( days ) );
        await this.followUpRepeatTimes.fill( String( times ) );
    }

    public async setIncludeFamily( include: boolean ): Promise<void> {
        await setCheckBox( this.page, this.includeFamily, include );
    }

    /** Saves; Care Entry then returns to its parent page, the Care Dashboard. */
    public async save(): Promise<void> {
        const entryUrl = this.page.url();
        await this.root.locator( "a[id$='_lbSave']" ).click();
        await this.page.waitForURL( url => url.toString() !== entryUrl, { timeout: 60_000 } );
        await this.page.waitForLoadState( "load" );
        await expectNoRockError( this.page );
    }
}
