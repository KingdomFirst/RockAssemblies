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
 * Page object for RockWeb/Plugins/rocks_kfs/Crm/PersonContactNotes.ascx.
 * Selectors match on server control ID suffixes and Rock's own js- classes, which are
 * stable across Rock versions even when the generated ClientID prefix changes.
 */
export class PersonContactNotesBlock {
    public readonly root: Locator;

    public constructor( private readonly page: Page, blockId: number ) {
        this.root = page.locator( `#bid_${blockId}` );
    }

    // #region Selection panel

    /** "Select a Person" or "Select a Group Member". */
    public get selectionHeader(): Locator {
        return this.root.locator( "[id$='_lblPersonInfoHdr']" );
    }

    public get personPicker(): Locator {
        return this.root.locator( ".picker-person" );
    }

    public get groupMemberDropDown(): Locator {
        return this.root.locator( "select[id$='_gmpGroupMember']" );
    }

    /**
     * Picks a person the way a user does: opens the picker, types a name, chooses the
     * result and clicks Select. The block then reloads the page with ?PersonId=.
     */
    public async selectPerson( searchName: string, personId: number ): Promise<void> {
        await this.personPicker.locator( ".js-personpicker-toggle" ).click();
        await this.personPicker.locator( "input[id$='_tbSearchName']" ).fill( searchName );

        const result = this.personPicker.locator( `li.js-picker-select-item[data-person-id='${personId}']` );
        const unauthorized = this.personPicker.locator( ".js-personpicker-searchresults .text-danger" );
        await expect( result.or( unauthorized ), `No search result for '${searchName}' (person ${personId})` ).toBeVisible();
        await expect( unauthorized, "The person picker's search (api/People/Search) refused the signed-in tester" ).toHaveCount( 0 );

        await result.locator( "input[type='radio']" ).check();
        await this.personPicker.locator( ".js-personpicker-select" ).click();
        await this.page.waitForURL( new RegExp( `[?&]PersonId=${personId}(&|$)`, "i" ) );
        await this.page.waitForLoadState( "load" );
        await expectNoRockError( this.page );
    }

    /** Chooses a member from the drop down. The block then reloads the page with ?GroupMember=. */
    public async selectGroupMember( groupMemberId: number ): Promise<void> {
        await this.groupMemberDropDown.selectOption( groupMemberId.toString() );
        await this.page.waitForURL( new RegExp( `[?&]GroupMember=${groupMemberId}(&|$)`, "i" ) );
        await this.page.waitForLoadState( "load" );
        await expectNoRockError( this.page );
    }

    // #endregion

    // #region Notes column

    /** The add-note editor; only rendered once a person or group member is selected. */
    public get noteEditor(): Locator {
        return this.root.locator( ".note-new-kfs" );
    }

    public get noteText(): Locator {
        return this.noteEditor.locator( "textarea" ).first();
    }

    public get saveNoteButton(): Locator {
        return this.root.locator( "[id$='_lbSaveNote']" );
    }

    /** Types the note and clicks "Save Note". */
    public async saveNote( text: string ): Promise<void> {
        await expect( this.noteEditor, "No note editor; does the tester have Edit on the configured note type?" ).toBeVisible();
        await this.noteText.fill( text );
        await this.saveNoteButton.click();
        await waitForPostback( this.page );
        await this.page.waitForLoadState( "load" );
        await expectNoRockError( this.page );
    }

    // #endregion
}
