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
import { getGlobalAttributeValue } from "../../../shared/testData/attributes";
import { EwsGlobalAttribute, LavaRender, expect, test } from "./microsoft365Utilities.fixture";

/*
    Covers the manual plan's global attribute check and "Test Shortcode": copy the
    KFS - EWS Calendar Items example, set calendarmailbox and impersonate, run it in the
    Lava Tester and check calendar data comes through. The tests render through Rock's
    Lava engine (api/Lava/RenderTemplate), as the Lava Tester does.

    The calendar is the live calendar@ mailbox. Reading it changes nothing, and every
    template prints at most one calendar item (plus a count); subjects are compared,
    never printed.
*/

/** What Rock writes when a Lava template fails to render. */
const lavaErrorPattern = /Liquid (syntax )?error|Lava error|Error resolving Lava|Unknown tag|shortcode .* not found/i;

/** The shortcode reports an EWS or sign-in failure as a Bootstrap warning. */
const shortcodeWarningPattern = /<div class='alert alert-warning'>([\s\S]*?)<\/div>/;

function expectRenderedCleanly( render: LavaRender ): void {
    expect( render.output, "Rock could not render the Lava" ).not.toMatch( lavaErrorPattern );

    // Name the real cause from Rock's exception log (before the compat fix the warning said only "One or more errors occurred.").
    const warning = render.output.match( shortcodeWarningPattern )?.[ 1 ] ?? null;
    const cause = render.loggedErrors.join( " | " ) || "(nothing in Rock's exception log)";
    const expired = /AADSTS7000222/.test( cause )
        ? " The Azure app's client secret has expired: create a new secret in the Entra app registration and save it in the 'EWS Azure Secret' global attribute (and M365_APP_SECRET in .env)."
        : "";
    expect( warning, `The shortcode reported an error. Cause: ${cause}.${expired}` ).toBeNull();
}

/** Adds days to a "yyyy-MM-dd" date, without the machine's time zone. */
function addDays( date: string, days: number ): string {
    const value = new Date( `${date}T00:00:00Z` );
    value.setUTCDate( value.getUTCDate() + days );
    return value.toISOString().substring( 0, 10 );
}

test.describe( "Microsoft 365 Utilities: KFS - EWS Calendar Items shortcode", () => {
    test.beforeEach( ( { shortcode } ) => {
        test.info().annotations.push(
            { type: "rock-version", description: shortcode.rockVersion },
            { type: "calendar", description: shortcode.mailbox } );
    } );

    test( "GlobalAttributes_EwsCredentials_AreSetAndEncrypted", async ( { shortcode } ) => {
        for ( const attribute of Object.values( EwsGlobalAttribute ) ) {
            const record = await shortcode.api.getByGuid( "Attributes", attribute.guid );
            expect( record, `Global attribute '${attribute.name}' is missing` ).not.toBeNull();
            expect( record!.Key ).toBe( attribute.key );

            // Only the length and shape are checked; the value is never printed.
            const value = await getGlobalAttributeValue( shortcode.api, attribute.key ) ?? "";
            expect( value.length, `Global attribute '${attribute.name}' has no value` ).toBeGreaterThan( 0 );
            expect( /^EAAAA/.test( value ), `Global attribute '${attribute.name}' is not stored encrypted` ).toBe( true );
        }
    } );

    test( "Render_DocumentationExample_RendersWithoutErrors", async ( { shortcode } ) => {
        // The manual step: copy the documented example and set calendarmailbox and impersonate.
        let template = shortcode.documentationExample
            .replace( /calendarmailbox:'[^']*'/, `calendarmailbox:'${shortcode.mailbox}'` );
        template = shortcode.impersonate
            ? template.replace( /impersonate:'[^']*'/, `impersonate:'${shortcode.impersonate}'` )
            : template.replace( /\s*impersonate:'[^']*'/, "" );
        expect( template, "The documented example no longer uses the calendarmailbox parameter" ).toContain( `calendarmailbox:'${shortcode.mailbox}'` );
        // Print at most one item from the live calendar.
        template = template.replace( /\{%\s*for calItem in CalendarItems\s*%\}/, "{% for calItem in CalendarItems limit:1 %}" );

        const render = await shortcode.render( template );

        expectRenderedCleanly( render );
        // The example covers today and the next 7 days, which may be empty; the next test checks data.
        test.info().annotations.push( { type: "example-output", description: render.output.trim() === "" ? "no calendar items in the next 7 days" : "rendered a calendar item" } );
    } );

    test( "Render_ThirtyDaysBackSixtyForward_ReturnsItemInWindow", async ( { shortcode } ) => {
        const result = await shortcode.renderItem( { daysback: 30, daysforward: 60 } );

        expectRenderedCleanly( result );
        expect( result.item, `No calendar items in ${shortcode.mailbox} between 30 days ago and 60 days ahead; data cannot be checked` ).not.toBeNull();

        // Items overlapping the window come back, so check overlap (a day of slack for time zones).
        const item = result.item!;
        const windowStart = addDays( result.rockToday, -31 );
        const windowEnd = addDays( result.rockToday, 62 );
        expect( item.subject.trim().length, "The item came back without a subject" ).toBeGreaterThan( 0 );
        expect( item.start, "The item has no Start" ).toMatch( /^\d{4}-\d{2}-\d{2}T/ );
        expect( item.end >= item.start, "The item ends before it starts" ).toBe( true );
        expect( item.end >= windowStart && item.start <= windowEnd, `The item (${item.start} to ${item.end}) is outside the requested window` ).toBe( true );
        test.info().annotations.push( { type: "items", description: `${result.itemCount} calendar item(s) in the window` } );
    } );

    test( "Render_OrderDesc_ReturnsLatestItemFirst", async ( { shortcode } ) => {
        const ascending = await shortcode.renderItem( { daysback: 30, daysforward: 60 } );
        const descending = await shortcode.renderItem( { daysback: 30, daysforward: 60, order: "desc" } );

        expectRenderedCleanly( descending );
        test.skip( descending.itemCount < 2, "Fewer than two calendar items in the window; ordering cannot be checked." );
        // Each render prints only its first item: the earliest by default, the latest with order:'desc'.
        expect( descending.item!.start >= ascending.item!.start, "order:'desc' did not put the latest item first" ).toBe( true );
        const sameFirstItem = descending.item!.start === ascending.item!.start && descending.item!.subject === ascending.item!.subject;
        expect( sameFirstItem, "order:'desc' returned the same first item as the default order" ).toBe( false );
    } );

    test( "Render_NoCalendarMailbox_RendersNothing", async ( { shortcode } ) => {
        const result = await shortcode.renderItem( { calendarmailbox: "", impersonate: shortcode.impersonate || "nobody@kfs-e2e.invalid" } );

        expectRenderedCleanly( result );
        expect( result.item ).toBeNull();
    } );

    // The documentation lists impersonate as optional ("If not provided, the calendarmailbox address will be used").
    test( "Render_NoImpersonate_ReadsCalendarAsCalendarMailbox", async ( { shortcode } ) => {
        const withImpersonate = await shortcode.renderItem( { daysback: 30, daysforward: 60 } );
        const withoutImpersonate = await shortcode.renderItem( { daysback: 30, daysforward: 60, impersonate: "" } );

        expect( withoutImpersonate.output, "Leaving out the optional impersonate parameter breaks the shortcode" ).not.toMatch( lavaErrorPattern );
        // Signing in as the calendar mailbox itself must work and return the same calendar.
        expectRenderedCleanly( withoutImpersonate );
        expect( withoutImpersonate.itemCount, "Without impersonate the shortcode returned a different calendar" ).toBe( withImpersonate.itemCount );
        const sameFirstItem = withoutImpersonate.item?.start === withImpersonate.item?.start && withoutImpersonate.item?.subject === withImpersonate.item?.subject;
        expect( sameFirstItem, "Without impersonate the shortcode returned a different first item" ).toBe( true );
    } );

    test( "Render_InvalidSecret_ShowsWarningAndLogsMicrosoftSignInError", async ( { shortcode } ) => {
        // Also proves the Microsoft sign-in library (MSAL) loads and reaches Entra ID on this Rock version.
        const result = await shortcode.renderItem( { appsecret: "kfs-e2e-not-a-real-secret" } );

        expect( result.output ).not.toMatch( lavaErrorPattern );
        // The warning names Microsoft's reason, not just "One or more errors occurred." (fixed in the compat release).
        expect( result.output.match( shortcodeWarningPattern )?.[ 1 ] ?? "", "The warning does not show Microsoft's sign-in error" ).toMatch( /AADSTS7000215/ );
        expect( result.item ).toBeNull();
        expect( result.loggedErrors.join( " | " ), "Rock logged no Microsoft sign-in error for the bad secret" ).toMatch( /MsalServiceException: .*AADSTS7000215/ );
    } );
} );
