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
import { Page, expect } from "@playwright/test";
import { RockApi } from "./rockApi";
import { getImpersonationParameter } from "./testData/people";

/**
 * Text that means Rock failed to render the page or a block. A WebForms block that no
 * longer compiles against a new Rock version surfaces here first.
 */
const rockErrorPatterns = [
    /Server Error in '\/' Application/i,
    /Compilation Error/i,
    /Parser Error/i,
    /An error has occurred/i,
    /Exception Details:/i
];

/**
 * Opens a Rock page signed in as the given person (by impersonation token, not password)
 * with optional page parameters, and fails if Rock rendered an error.
 */
export async function openPageAsPerson( page: Page, api: RockApi, args: { pageId: number; personId: number; parameters?: Record<string, string | number>; allPages?: boolean } ): Promise<void> {
    // A page-limited token signs the person in on that page only; a block that moves the user
    // to another page (e.g. Save returning to a parent page) needs allPages.
    const impersonation = await getImpersonationParameter( api, args.personId, args.allPages ? null : args.pageId );
    const query = [ impersonation ];

    for ( const [ key, value ] of Object.entries( args.parameters ?? {} ) ) {
        query.push( `${encodeURIComponent( key )}=${encodeURIComponent( String( value ) )}` );
    }

    const response = await page.goto( `/page/${args.pageId}?${query.join( "&" )}` );
    expect( response?.status() ?? 0, "Rock returned an HTTP error for the page" ).toBeLessThan( 500 );

    await expectNoRockError( page );
}

/** Opens a Rock page with no one signed in. */
export async function openPageAnonymously( page: Page, args: { pageId: number } ): Promise<void> {
    await page.context().clearCookies();
    const response = await page.goto( `/page/${args.pageId}` );
    expect( response?.status() ?? 0, "Rock returned an HTTP error for the page" ).toBeLessThan( 500 );

    await expectNoRockError( page );
}

/** Fails if the current page is a Rock/ASP.NET error page or shows a block exception. */
export async function expectNoRockError( page: Page ): Promise<void> {
    expect( page.url(), "Rock redirected to its error page" ).not.toMatch( /\/error\.aspx/i );

    // Rock's "Error Loading Block" alert keeps the exception in a collapsed element, so read textContent (includes hidden text).
    const blockErrors = await page.locator( ".alert-danger, .alert-block" ).evaluateAll( elements => elements
        .map( element => ( element.textContent ?? "" ).replace( /\s+/g, " " ).trim() )
        .filter( text => /Error Loading Block|exception/i.test( text ) ) );
    expect( blockErrors, "Rock failed to load a block" ).toEqual( [] );

    const bodyText = await page.locator( "body" ).innerText();
    for ( const pattern of rockErrorPatterns ) {
        expect( bodyText, `Rock rendered an error (${pattern})` ).not.toMatch( pattern );
    }
}

/** Waits for any in-flight ASP.NET UpdatePanel (partial postback) request to finish. */
export async function waitForPostback( page: Page ): Promise<void> {
    await page.waitForFunction( () => {
        const prm = ( window as unknown as { Sys?: { WebForms?: { PageRequestManager?: { getInstance(): { get_isInAsyncPostBack(): boolean } } } } } )
            .Sys?.WebForms?.PageRequestManager?.getInstance();
        return !prm || !prm.get_isInAsyncPostBack();
    } );
    await page.waitForLoadState( "domcontentloaded" );
}
