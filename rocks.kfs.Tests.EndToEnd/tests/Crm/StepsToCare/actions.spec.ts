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
import { waitForPostback } from "../../../shared/rockBrowser";
import { pickRadio } from "../../../shared/rockControls";
import { PluginGuid, expect, test } from "./stepsToCare.fixture";

// Traces are off: the shared REST client is created during the first test's trace recording
// and later fails writing to it (ENOENT), failing tests that passed.
test.use( { trace: "off" } );

/*
    Covers the manual plan's "Action" and "Add Note" steps (documentation scenarios 1, 2
    and 6: care touches through Quick Notes, Add Connection Request, Launch Workflow,
    Snooze and Re-Open). Each test enters its own need through Care Entry first.
*/
test.describe( "Steps to Care: actions and notes", () => {
    test( "Actions_Menu_OffersEachConfiguredAction", async ( { page, stc } ) => {
        const details = stc.uniqueDetails( "Actions" );
        const dashboard = await stc.enterNeed( page, { details } );

        expect( await dashboard.actionNames( dashboard.row( details ) ) ).toEqual( expect.arrayContaining( [ "Complete Need", "Snooze", "Launch Workflow", "Add Prayer Request", "Add Connection Request", "View History" ] ) );
    } );

    test( "QuickNote_Called_AddsCareTouch", async ( { page, stc } ) => {
        const details = stc.uniqueDetails( "Quick note" );
        const dashboard = await stc.enterNeed( page, { details } );
        await expect( dashboard.careTouches( dashboard.row( details ) ) ).toHaveText( "0" );

        await dashboard.quickNote( dashboard.row( details ), "Called" );

        await expect( dashboard.careTouches( dashboard.row( details ) ) ).toHaveText( "1" );
    } );

    test( "MakeNote_QuickNoteWithText_SavesNoteShownInDialog", async ( { page, stc } ) => {
        const details = stc.uniqueDetails( "Make note" );
        const noteText = `KFS E2E note ${Date.now()}`;
        const dashboard = await stc.enterNeed( page, { details } );

        await dashboard.openMakeNote( dashboard.row( details ) );
        const dialog = dashboard.dialog;
        await pickRadio( page, dialog.locator( "input[type='radio'][id*='_rrblQuickNotes_']" ).first() );
        await dialog.locator( "textarea[id$='_rtbNote']" ).fill( noteText );
        await dialog.locator( "a[id$='_rbBtnQuickNoteSave'], button[id$='_rbBtnQuickNoteSave']" ).click();
        await waitForPostback( page );

        // The note is listed in the dialog (or, with Close Dialog on Save, on reopening it) and counted as a care touch.
        await stc.openDashboard( page );
        await expect( dashboard.careTouches( dashboard.row( details ) ) ).toHaveText( "1" );
        await dashboard.openMakeNote( dashboard.row( details ) );
        await expect( dashboard.dialog ).toContainText( noteText );
    } );

    test( "Snooze_ThenReOpen_SwitchesBetweenSnoozedAndOpen", async ( { page, stc } ) => {
        const details = stc.uniqueDetails( "Snooze" );
        const dashboard = await stc.enterNeed( page, { details } );

        await dashboard.action( dashboard.row( details ), "Snooze" );
        const snoozeDialog = dashboard.dialog;
        await expect( snoozeDialog ).toContainText( "Snooze" );
        const until = new Date( Date.now() + 14 * 24 * 60 * 60 * 1000 );
        const untilBox = snoozeDialog.locator( "input[id*='_dpSnoozeUntil']" ).first();
        // Typed key by key: the date picker's script clears a value set all at once.
        await untilBox.click();
        await untilBox.pressSequentially( `${until.getMonth() + 1}/${until.getDate()}/${until.getFullYear()}`, { delay: 30 } );
        // Click the dialog's title so the pop-up calendar closes and uncovers the Snooze button.
        await snoozeDialog.locator( ".modal-header" ).click();
        await expect( untilBox ).not.toHaveValue( "" );
        await snoozeDialog.locator( "a[id$='_serverSaveLink']" ).click();
        await waitForPostback( page );
        await page.waitForLoadState( "load" );
        await expect( snoozeDialog, "The Snooze dialog closes when saved" ).toBeHidden();

        // The dashboard lists Open needs unless filtered, so the snoozed need drops off; a care
        // worker filters for Snoozed to find it and re-open it.
        await expect( dashboard.row( details ) ).toHaveCount( 0 );
        const snoozedId = await stc.api.getIdByGuid( "DefinedValues", PluginGuid.statusSnoozed );
        try {
            await dashboard.showOnlyStatus( snoozedId );
            const row = dashboard.row( details );
            await expect( row ).toBeVisible();
            expect( await dashboard.actionNames( row ) ).toContain( "Re-Open" );
            expect( await dashboard.actionNames( row ) ).not.toContain( "Snooze" );

            await dashboard.action( row, "Re-Open" );
        }
        finally {
            await dashboard.clearFilter();
        }

        // Open again: back on the unfiltered dashboard, with Snooze offered again.
        await expect( dashboard.row( details ) ).toBeVisible();
        expect( await dashboard.actionNames( dashboard.row( details ) ) ).toContain( "Snooze" );
    } );

    test( "AddConnectionRequest_TestOpportunity_CreatesRequestWithNeedDetails", async ( { page, stc } ) => {
        const details = stc.uniqueDetails( "Connection" );
        const dashboard = await stc.enterNeed( page, { details } );

        await dashboard.action( dashboard.row( details ), "Add Connection Request" );
        const dialog = dashboard.dialog;
        await expect( dialog ).toContainText( "Add Connection Request" );
        const opportunity = dialog.locator( `input[type='checkbox'][value='${stc.connection.opportunityId}']` );
        await expect( opportunity, "The test connection opportunity is offered" ).toHaveCount( 1 );
        await opportunity.locator( "xpath=ancestor::label[1]" ).click();
        await expect( dialog.locator( "textarea[id$='_tbComments']" ), "The need's details fill the comments" ).toHaveValue( new RegExp( details ) );
        await dialog.locator( "a[id$='_serverSaveLink']" ).click();
        await waitForPostback( page );

        await expect( dialog.locator( "[id$='_nbSuccess']" ) ).toBeVisible();
        const requests = await stc.api.query( "ConnectionRequests", `ConnectionOpportunityId eq ${stc.connection.opportunityId}` );
        expect( requests, "One connection request in the test opportunity" ).toHaveLength( 1 );
        expect( requests[ 0 ].PersonAliasId ).toBe( stc.requestor.primaryAliasId );
        expect( String( requests[ 0 ].Comments ) ).toContain( details );
    } );

    /*
        The next three actions hand off to core Rock pages (Prayer Request Detail, Launch
        Workflow, history). Those pages are not part of the plugin and the test role is not
        given access to them, so the tests check where the plugin sends the browser.
    */
    test( "AddPrayerRequest_CreatesRequestAndOpensItsDetailPage", async ( { page, stc } ) => {
        const details = stc.uniqueDetails( "Prayer" );
        const dashboard = await stc.enterNeed( page, { details } );

        const target = await dashboard.actionNavigation( dashboard.row( details ), "Add Prayer Request" );

        const requests = await stc.api.query( "PrayerRequests", `RequestedByPersonAliasId eq ${stc.requestor.primaryAliasId}`, "&$orderby=Id desc" );
        try {
            const request = requests.find( r => String( r.Text ?? "" ).includes( details ) );
            expect( request, "A prayer request with the need's details, for the requestor" ).toBeDefined();
            expect( target ).toMatch( new RegExp( `PrayerRequestId=${request!.Id}(&|$)`, "i" ) );
        }
        finally {
            for ( const r of requests.filter( r => String( r.Text ?? "" ).startsWith( "KFS E2E" ) ) ) {
                await stc.api.delete( "PrayerRequests", r.Id );
            }
        }
    } );

    test( "LaunchWorkflow_SendsNeedToWorkflowLauncher", async ( { page, stc } ) => {
        const details = stc.uniqueDetails( "Workflow" );
        const dashboard = await stc.enterNeed( page, { details } );

        const target = await dashboard.actionNavigation( dashboard.row( details ), "Launch Workflow" );

        // Rock's default launcher route, with an entity set holding this care need.
        expect( target ).toMatch( /\/LaunchWorkflows\/\d+/i );
    } );

    test( "ViewHistory_OpensHistoryPageForNeed", async ( { page, stc } ) => {
        const details = stc.uniqueDetails( "History" );
        const dashboard = await stc.enterNeed( page, { details } );
        const needId = await dashboard.needId( dashboard.row( details ) );

        const target = await dashboard.actionNavigation( dashboard.row( details ), "View History" );

        // The installed history page's route carries the need in its path (/StepsToCareDetail/{id}/History).
        expect( target ).toMatch( new RegExp( `/${needId}/History|CareNeedId=${needId}(&|$)`, "i" ) );
    } );
} );
