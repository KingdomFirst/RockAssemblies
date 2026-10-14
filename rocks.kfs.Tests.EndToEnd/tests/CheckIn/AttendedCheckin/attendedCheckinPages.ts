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
 * Routes of the Attended Check-in pages on the site under test. These are the routes
 * the plugin's install creates; if a site moved them, change them here.
 */
export const AttendedCheckinRoute = {
    admin: "/attendedcheckin/admin",
    search: "/attendedcheckin/search",
    family: "/attendedcheckin/family",
    confirm: "/attendedcheckin/confirm",
    activity: "/attendedcheckin/activity"
} as const;

/** Gender option labels from Rock's Gender enum (the add-person modals bind to it). */
export type GenderLabel = "Male" | "Female";

/**
 * Page object for the Attended Check-in plugin (RockWeb/Plugins/cc_newspring/AttendedCheckin).
 * One object covers all five pages, since a test walks through them in one browser tab.
 * Selectors match on server control ID suffixes, which are stable across Rock versions.
 */
export class AttendedCheckinPages {
    public constructor( public readonly page: Page ) {
    }

    // #region Shared

    /** The ModalAlert (bootbox) the blocks use for warnings and validation messages. */
    public get alert(): Locator {
        return this.page.locator( ".bootbox .bootbox-body" );
    }

    public async dismissAlert(): Promise<void> {
        await this.page.locator( ".bootbox .modal-footer button" ).first().click();
        await expect( this.alert ).toBeHidden();
    }

    /** Clicks a control that posts back (partial or full) and waits for Rock to finish. */
    private async clickAndWait( locator: Locator ): Promise<void> {
        await locator.click();
        await waitForPostback( this.page );
        await expectNoRockError( this.page );
    }

    // #endregion

    // #region Admin

    public get kioskDropDown(): Locator {
        return this.page.locator( "select[id$='_ddlKiosk']" );
    }

    public areaButton( areaId: number ): Locator {
        return this.page.locator( `input[id$='_btnGroupType'][data-id='${areaId}']` );
    }

    public get okButton(): Locator {
        return this.page.locator( "[id$='_lbOk']" );
    }

    /** Opens the Admin page in a fresh session and waits for its startup postback. */
    public async openAdmin(): Promise<void> {
        const response = await this.page.goto( AttendedCheckinRoute.admin );
        expect( response?.status() ?? 0, "Rock returned an HTTP error for the Admin page" ).toBeLessThan( 500 );
        expect( this.page.url(), "The Admin page redirected to sign-in. The tests expect the check-in site to allow anonymous access." ).not.toMatch( /login/i );

        // The block posts back once on load to restore kiosk settings from localStorage.
        await this.page.waitForLoadState( "networkidle" );
        await waitForPostback( this.page );
        await expectNoRockError( this.page );
    }

    public async selectKiosk( kioskId: number ): Promise<void> {
        await this.kioskDropDown.selectOption( String( kioskId ) );
        await waitForPostback( this.page );
        await expectNoRockError( this.page );
    }

    /** Toggles an area button (client side only; the selection is sent on OK). */
    public async toggleArea( areaId: number ): Promise<void> {
        await this.areaButton( areaId ).click();
    }

    public async clickOk(): Promise<void> {
        await this.clickAndWait( this.okButton );
    }

    /** Configures the kiosk from a fresh session and lands on the Search page. */
    public async startCheckin( args: { kioskId: number; areaId: number } ): Promise<void> {
        await this.openAdmin();
        await this.selectKiosk( args.kioskId );
        await expect( this.areaButton( args.areaId ), "The test area is not listed for the test kiosk. Has setup/AttendedCheckin_Links.sql been run?" ).toBeVisible();
        await this.toggleArea( args.areaId );
        await this.okButton.click();
        await this.page.waitForURL( url => url.pathname.toLowerCase().startsWith( AttendedCheckinRoute.search ) );
        await expectNoRockError( this.page );
    }

    // #endregion

    // #region Search

    public get searchBox(): Locator {
        return this.page.locator( "input[id$='_tbSearchBox']" );
    }

    public get searchButton(): Locator {
        return this.page.locator( "[id$='_lbSearch']" );
    }

    /** Searches and waits for either the Family page or a warning. */
    public async search( value: string ): Promise<void> {
        await this.searchBox.fill( value );
        await this.searchButton.click();
        await Promise.race( [
            this.page.waitForURL( url => url.pathname.toLowerCase().startsWith( AttendedCheckinRoute.family ) ),
            this.alert.waitFor( { state: "visible" } )
        ] );
        await waitForPostback( this.page );
        await expectNoRockError( this.page );
    }

    /** Searches and expects to land on the Family page. */
    public async searchForFamily( value: string ): Promise<void> {
        await this.search( value );
        await expect( this.page, `Search for '${value}' did not reach the Family page` ).toHaveURL( /\/attendedcheckin\/family/i );
    }

    // #endregion

    // #region Family

    public get familyButtons(): Locator {
        return this.page.locator( "[id$='_lbSelectFamily']" );
    }

    public familyButton( name: string | RegExp ): Locator {
        return this.familyButtons.filter( { hasText: name } );
    }

    /** People buttons in the family member list. */
    public get memberButtons(): Locator {
        return this.page.locator( "[id$='_pnlPerson'] [id$='_lbSelectPerson']" );
    }

    /** People buttons in the visitor list. */
    public get visitorButtons(): Locator {
        return this.page.locator( "[id$='_pnlVisitor'] [id$='_lbSelectPerson']" );
    }

    public personButton( personId: number ): Locator {
        return this.page.locator( `[id$='_lbSelectPerson'][data-id='${personId}']` );
    }

    public get addVisitorButton(): Locator {
        return this.page.locator( "[id$='_lbAddVisitor']" );
    }

    public get addPersonButton(): Locator {
        return this.page.locator( "[id$='_lbAddFamilyMember']" );
    }

    public get addFamilyButton(): Locator {
        return this.page.locator( "[id$='_lbNewFamily']" );
    }

    public get familyNextButton(): Locator {
        return this.page.locator( "[id$='_pnlSelections'] [id$='_lbNext']" );
    }

    public async selectFamily( name: string | RegExp ): Promise<void> {
        await this.clickAndWait( this.familyButton( name ) );
    }

    /** Toggles a person (client side only; the selection is sent on Next). */
    public async togglePerson( personId: number ): Promise<void> {
        await this.personButton( personId ).click();
    }

    /** Clicks Next on the Family page and waits for the Confirm page or a warning. */
    public async familyNext(): Promise<void> {
        await this.familyNextButton.click();
        await Promise.race( [
            this.page.waitForURL( url => url.pathname.toLowerCase().startsWith( AttendedCheckinRoute.confirm ) ),
            this.alert.waitFor( { state: "visible" } )
        ] );
        await waitForPostback( this.page );
        await expectNoRockError( this.page );
    }

    // #endregion

    // #region Add Person / Add Visitor modal

    public get addPersonModal(): Locator {
        return this.page.locator( "[id$='_mdlAddPerson_modal_dialog_panel']" );
    }

    /**
     * Adds a new person through the Add Person or Add Visitor modal: fills the
     * search fields, searches, then clicks "None of these, add a new person".
     */
    public async addNewPersonInModal( opener: Locator, person: { firstName: string; lastName: string; gender: GenderLabel; birthDate?: string } ): Promise<void> {
        await this.clickAndWait( opener );
        await expect( this.addPersonModal ).toBeVisible();

        await this.addPersonModal.locator( "input[id$='_tbFirstName']" ).fill( person.firstName );
        await this.addPersonModal.locator( "input[id$='_tbLastName']" ).fill( person.lastName );
        await this.addPersonModal.locator( "select[id$='_ddlPersonGender']" ).selectOption( { label: person.gender } );
        if ( person.birthDate ) {
            await this.addPersonModal.locator( "input[id$='_dpPersonDOB']" ).fill( person.birthDate );
        }

        await this.clickAndWait( this.addPersonModal.locator( "[id$='_lbPersonSearch']" ) );
        await this.clickAndWait( this.addPersonModal.locator( "[id$='_lbNewPerson']" ) );
        await expect( this.addPersonModal ).toBeHidden();
    }

    // #endregion

    // #region Add Family modal

    public get addFamilyModal(): Locator {
        return this.page.locator( "[id$='_mdlNewFamily_modal_dialog_panel']" );
    }

    /** Adds a new family through the Add Family modal, one row per person. */
    public async addNewFamily( people: Array<{ firstName: string; lastName: string; gender: GenderLabel }> ): Promise<void> {
        await this.clickAndWait( this.addFamilyButton );
        await expect( this.addFamilyModal ).toBeVisible();

        for ( const [ index, person ] of people.entries() ) {
            await this.addFamilyModal.locator( "input[id$='_tbFirstName']" ).nth( index ).fill( person.firstName );
            await this.addFamilyModal.locator( "input[id$='_tbLastName']" ).nth( index ).fill( person.lastName );
            await this.addFamilyModal.locator( "select[id$='_ddlGender']" ).nth( index ).selectOption( { label: person.gender } );
        }

        await this.clickAndWait( this.addFamilyModal.locator( "[id$='_lbSaveFamily']" ) );
        await expect( this.addFamilyModal ).toBeHidden();
    }

    // #endregion

    // #region Confirm

    public get confirmRows(): Locator {
        return this.page.locator( "[id$='_gPersonList'] tbody tr" );
    }

    public confirmRow( name: string | RegExp ): Locator {
        return this.confirmRows.filter( { hasText: name } );
    }

    public get printAllButton(): Locator {
        return this.page.locator( "[id$='_lbPrintAll']" );
    }

    public get confirmDoneButton(): Locator {
        return this.page.locator( "[id$='_pnlConfirm'] [id$='_lbNext']" );
    }

    /** True once the row's Checked In column shows the check mark. */
    public checkedInMark( name: string | RegExp ): Locator {
        return this.confirmRow( name ).locator( ".fa-check" );
    }

    public async printAll(): Promise<void> {
        await this.clickAndWait( this.printAllButton );
    }

    /*
        10/8/2026 - CLAUDE

        The Edit and Delete buttons are Rock's grid EditField and DeleteField, whose default
        icons changed in Rock 20 from Font Awesome (fa-pencil, fa-times) to Tabler (ti-pencil,
        ti-x). The selectors match either, so the suite runs on Rock 17 and Rock 20.

        Reason: Rock 20 changed the default grid button icons.
    */

    /** Clicks the row's Delete button, accepting Rock's confirmation if it asks. */
    public async deleteRow( name: string | RegExp ): Promise<void> {
        await this.confirmRow( name ).locator( "a:has(.fa-times, .ti-x)" ).click();

        const confirmButton = this.page.locator( ".bootbox .modal-footer .btn-primary" );
        const asked = await confirmButton.waitFor( { state: "visible", timeout: 2_000 } ).then( () => true, () => false );
        if ( asked ) {
            await confirmButton.click();
        }

        await waitForPostback( this.page );
        await expectNoRockError( this.page );
    }

    /** Clicks the row's Edit button and waits for the Activity Select page. */
    public async editRow( name: string | RegExp ): Promise<void> {
        await this.confirmRow( name ).locator( "a:has(.fa-pencil, .ti-pencil)" ).click();
        await this.page.waitForURL( url => url.pathname.toLowerCase().startsWith( AttendedCheckinRoute.activity ) );
        await expectNoRockError( this.page );
    }

    // #endregion

    // #region Activity Select

    public get activityPersonName(): Locator {
        return this.page.locator( "[id$='_pnlActivities'] h1" ).first();
    }

    public get activityBackButton(): Locator {
        return this.page.locator( "[id$='_pnlActivities'] [id$='_lbBack']" );
    }

    // #endregion
}
