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
import { waitForPostback } from "./rockBrowser";

/*
    Helpers for Rock's WebForms controls. Some themes draw their own check box and radio
    graphics over the real inputs, so Playwright cannot click the input itself; these click
    the control's label, which is also what a user does.
*/

/** Checks or unchecks a check box by clicking its label, if it is not already in that state. */
export async function setCheckBox( page: Page, checkBox: Locator, checked: boolean ): Promise<void> {
    if ( await checkBox.isChecked() === checked ) {
        return;
    }
    await checkBox.locator( "xpath=ancestor::label[1]" ).click();
    await waitForPostback( page );
    await expect( checkBox ).toBeChecked( { checked } );
}

/** Picks a radio button by clicking its label. */
export async function pickRadio( page: Page, radio: Locator ): Promise<void> {
    await radio.locator( "xpath=ancestor::label[1]" ).click();
    await waitForPostback( page );
    await expect( radio ).toBeChecked();
}

/**
 * Picks a person in a Rock person picker the way a user does: opens it, types the name,
 * chooses the result and clicks Select. Waits for the partial postback that follows.
 */
export async function pickPerson( page: Page, picker: Locator, searchName: string, personId: number ): Promise<void> {
    await picker.locator( ".js-personpicker-toggle" ).click();
    await picker.locator( "input[id$='_tbSearchName']" ).fill( searchName );

    const result = picker.locator( `li.js-picker-select-item[data-person-id='${personId}']` );
    const unauthorized = picker.locator( ".js-personpicker-searchresults .text-danger" );
    await expect( result.or( unauthorized ), `No search result for '${searchName}' (person ${personId})` ).toBeVisible();
    await expect( unauthorized, "The person picker's search (api/People/Search) refused the signed-in person" ).toHaveCount( 0 );

    await result.locator( "label" ).first().click();
    await picker.locator( ".js-personpicker-select" ).click();
    await waitForPostback( page );
    await page.waitForLoadState( "load" );
}
