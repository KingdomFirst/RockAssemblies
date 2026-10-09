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

/*
    10/6/2026 - CLAUDE

    The Eventbrite Settings block decrypts the Eventbrite private token and prints it
    in plain text ("Private Token" in its details list) on every load and partial
    postback. A failure screenshot, trace or page snapshot of that page would carry
    the live token into test-results. So every response for the settings test page
    is rewritten before the browser sees it: the token's characters become '*' (same
    length, so ASP.NET's partial-postback length prefixes stay valid). A response that
    mentions the token but does not match the expected markup is refused outright
    rather than passed through. Traces are also off for this suite, since they keep
    __VIEWSTATE, which holds the token too.

    Reason: Keep the Eventbrite private token out of every test artifact.
*/
const tokenPattern = /(<dt>\s*Private Token\s*<\/dt>\s*<dd>)([^<]*)(<\/dd>)/gi;

/** Rewrites every response for the page so the private token never reaches the browser. */
export async function protectEventbriteToken( page: Page, pageId: number ): Promise<void> {
    await page.route( url => new RegExp( `/page/${pageId}(\\?|$)`, "i" ).test( url.pathname + url.search ), async route => {
        // The page calls Eventbrite while it renders (and compiles after a deploy), so allow
        // more than the 20s action timeout. A failure is reported by its first line only:
        // Playwright's call log lists the request headers, which hold the session cookie.
        let response;
        try {
            response = await route.fetch( { timeout: 120_000 } );
        }
        catch ( error ) {
            console.warn( `Eventbrite Settings test page request failed: ${String( ( error as Error ).message ).split( "\n" )[ 0 ]}` );
            await route.abort( "failed" );
            return;
        }
        const body = await response.text();

        if ( !/Private Token/i.test( body ) ) {
            await route.fulfill( { response, body } );
            return;
        }

        const redacted = body.replace( tokenPattern, ( _match, open: string, token: string, close: string ) => `${open}${"*".repeat( token.length )}${close}` );
        if ( redacted === body && /<dt>\s*Private Token/i.test( body ) ) {
            await route.fulfill( { status: 500, contentType: "text/plain", body: "KFS E2E: the Eventbrite Settings markup changed and the private token could not be redacted, so the page was blocked. Update tokenPattern in eventbritePages.ts." } );
            return;
        }

        await route.fulfill( { response, body: redacted } );
    } );
}

/** Page object for RockWeb/Plugins/rocks_kfs/Eventbrite/EventbriteSettings.ascx. */
export class EventbriteSettingsBlock {
    public readonly root: Locator;

    public constructor( private readonly page: Page, blockId: number ) {
        this.root = page.locator( `#bid_${blockId}` );
    }

    /** "Authenticated" or "Not authenticated". */
    public get loginStatus(): Locator {
        return this.root.locator( "[id$='_lblLoginStatus']" );
    }

    /** The token as shown, already redacted to '*' by protectEventbriteToken. */
    public get privateToken(): Locator {
        return this.root.locator( "dt", { hasText: "Private Token" } ).locator( "xpath=following-sibling::dd[1]" );
    }

    public get organization(): Locator {
        return this.root.locator( "dt", { hasText: "Organization" } ).locator( "xpath=following-sibling::dd[1]" );
    }

    public get linkedGroupsGrid(): Locator {
        return this.root.locator( "table[id$='_gEBLinkedGroups']" );
    }

    public get eventDropDown(): Locator {
        return this.root.locator( "select[id$='_ddlEventbriteEvents']" );
    }

    public get createGroupButton(): Locator {
        return this.root.locator( "[id$='_lbCreateNewRockGroup']" );
    }

    public get createNotice(): Locator {
        return this.root.locator( "[id$='_nbLinkNew']" );
    }

    /** Value (Eventbrite event Id) of the drop down option for the event, or null. */
    public async findEventId( eventName: string ): Promise<string | null> {
        const options = await this.eventDropDown.locator( "option" ).evaluateAll( elements => elements.map( e => ( { value: ( e as HTMLOptionElement ).value, text: e.textContent ?? "" } ) ) );
        return options.find( option => option.text.startsWith( `${eventName} - ` ) )?.value ?? null;
    }

    public async createGroupFor( eventId: string ): Promise<void> {
        await this.eventDropDown.selectOption( eventId );
        await this.createGroupButton.click();
        await waitForPostback( this.page );
        await expectNoRockError( this.page );
    }
}

/** The copy of Rock's Group Viewer page: Group Detail, the Eventbrite Sync Button and Group Member List. */
export class GroupViewerPage {
    public constructor( private readonly page: Page, private readonly blockIds: { sync: number; groupDetail: number; memberList: number } ) {
    }

    public get syncButton(): Locator {
        return this.page.locator( `#bid_${this.blockIds.sync} [id$='_lbSyncButton']` );
    }

    public get unlinkButton(): Locator {
        return this.page.locator( `#bid_${this.blockIds.sync} [id$='_lbUnlink']` );
    }

    private get groupDetail(): Locator {
        return this.page.locator( `#bid_${this.blockIds.groupDetail}` );
    }

    /** The Eventbrite Event field's edit control (a drop down of the organization's events). */
    public eventAttributeDropDown( attributeId: number ): Locator {
        return this.groupDetail.locator( `select[id$='attribute_field_${attributeId}']` );
    }

    public get memberRows(): Locator {
        // Rock's empty-grid row is a single cell spanning every column.
        return this.page.locator( `#bid_${this.blockIds.memberList} table[id$='_gGroupMembers'] tbody tr:not(:has(td[colspan]))` );
    }

    /*
        10/8/2026 - CLAUDE

        Rock 20 replaced the WebForms Group Detail block with an Obsidian block under the
        same block type Guid. The block's markup decides which steps are used, so the test
        runs on Rock 17 (WebForms) and Rock 20 (Obsidian). In the Obsidian editor a field
        type without an Obsidian edit control is shown as a plain text box; the Eventbrite
        Event field type has none yet, so on Rock 20 this fails with that message until the
        plugin gives the field a drop down there.

        Reason: Rock 20 converted the core Group Detail block to Obsidian.
    */
    /** Clicks Edit on Group Detail, picks the event in the Eventbrite attribute and saves. */
    public async linkEventInGroupEditor( attributeId: number, event: { id: string; name: string } ): Promise<void> {
        const webFormsEdit = this.groupDetail.locator( "[id$='_btnEdit']" );
        const obsidianEdit = this.groupDetail.getByRole( "button", { name: "Edit", exact: true } );
        await expect( webFormsEdit.or( obsidianEdit ).first(), "No Edit button on Group Detail" ).toBeVisible();

        if ( await webFormsEdit.count() > 0 ) {
            await this.linkEventInWebFormsEditor( attributeId, event.id );
        }
        else {
            await this.linkEventInObsidianEditor( event.name );
        }
    }

    /** Rock 17: the WebForms Group Detail editor, where the field is a native drop down. */
    private async linkEventInWebFormsEditor( attributeId: number, eventId: string ): Promise<void> {
        await this.groupDetail.locator( "[id$='_btnEdit']" ).click();
        await waitForPostback( this.page );
        const dropDown = this.eventAttributeDropDown( attributeId );
        await expect( dropDown, "No Eventbrite Event attribute in the group editor" ).toHaveCount( 1 );
        // Group attributes sit in a collapsed panel ("General"); expand it as a user would.
        if ( !await dropDown.isVisible() ) {
            await this.groupDetail.locator( "[id$='_wpGroupAttributes'] .panel-heading" ).first().click();
        }
        await expect( dropDown ).toBeVisible();
        await this.eventAttributeDropDown( attributeId ).selectOption( eventId );
        await this.groupDetail.locator( "[id$='_btnSave']" ).click();
        await waitForPostback( this.page );
        await this.page.waitForLoadState( "load" );
        await expectNoRockError( this.page );
    }

    /** Rock 20: the Obsidian Group Detail editor, where the field is found by its label. */
    private async linkEventInObsidianEditor( eventName: string ): Promise<void> {
        await this.groupDetail.getByRole( "button", { name: "Edit", exact: true } ).click();
        const saveButton = this.groupDetail.getByRole( "button", { name: "Save", exact: true } );
        await expect( saveButton ).toBeVisible();

        const field = this.groupDetail.locator( ".form-group" )
            .filter( { has: this.page.locator( "label.control-label", { hasText: /^\s*Eventbrite Event\s*$/ } ) } ).first();
        await expect( field, "No Eventbrite Event attribute in the group editor" ).toHaveCount( 1 );
        await field.scrollIntoViewIfNeeded();

        const dropDown = field.locator( ".ant-select-selector" );
        if ( await dropDown.count() === 0 ) {
            const textBox = await field.getByRole( "textbox" ).count() > 0;
            throw new Error( textBox
                ? "The Eventbrite Event attribute is a plain text box in Rock's Obsidian Group Detail editor, not a drop down of events: "
                    + "the plugin's EventbriteEventFieldType has no Obsidian edit control, so Rock falls back to its text field."
                : "The Eventbrite Event attribute in Rock's Obsidian Group Detail editor has no drop down of events." );
        }

        await dropDown.click();
        await this.page.locator( ".ant-select-dropdown:visible .ant-select-item-option", { hasText: eventName } ).first().click();

        const saved = this.page.waitForResponse( response => response.url().includes( "/api/v2/BlockActions/" ) && response.request().method() === "POST" && /save/i.test( response.url() ) );
        await saveButton.click();
        const response = await saved;
        expect( response.ok(), `Saving the group failed (HTTP ${response.status()})` ).toBe( true );
        await this.page.waitForLoadState( "load" );
        await expectNoRockError( this.page );
    }

    /** Clicks a button that syncs or unlinks; the block then reloads the page (a sync calls Eventbrite first). */
    public async clickAndWaitForReload( button: Locator ): Promise<void> {
        await Promise.all( [
            this.page.waitForEvent( "load", { timeout: 180_000 } ),
            button.click()
        ] );
        await expectNoRockError( this.page );
    }
}
