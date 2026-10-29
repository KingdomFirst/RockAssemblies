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
import { asEnumValue } from "../../../shared/rockApi";
import { Recurrence, RecipientStatus, expect, test } from "./scheduledGroupCommunication.fixture";

/*
    Covers the manual plan's email steps: a One Time item on the group's Scheduled Emails
    matrix, due now; run "Send Scheduled Group Email (Plugin)"; the communication history
    shows it was sent. Each test uses its own inactive job (never scheduled) and removes
    its item right after the run. The Edify outgoing-messages check stays manual: each
    sending test records what to look for as a "manual-check" annotation in the report.
*/
test.describe( "Scheduled Group Communication: Email", () => {
    test( "EmailJob_OneTimeItemDueNow_SendsEmailToGroupMember", async ( { sgc, cleanUpItems } ) => {
        const stamp = `${Date.now()}`;
        const subject = `KFS E2E Scheduled Email ${stamp}`;
        const item = await sgc.schedule( "email", { secondsFromNow: -30, recurrence: Recurrence.OneTime, subject, message: `<p>KFS end-to-end test ${stamp}. No action needed.</p>` } );
        cleanUpItems.push( item );

        const run = await sgc.runJobFor( item );

        expect( String( run.LastStatus ), `Job result: ${run.LastStatusMessage}` ).toMatch( /Success/i );
        expect( String( run.LastStatusMessage ) ).toMatch( /Sent 1 communication/i );

        const communications = await sgc.api.query( "Communications", `Subject eq '${subject}'` );
        expect( communications, "The job should create exactly one communication" ).toHaveLength( 1 );
        const communication = communications[ 0 ];
        expect( communication.FromEmail ).toBe( sgc.fromEmail );
        expect( communication.FromName ).toBe( sgc.fromName );

        const recipients = await sgc.waitForDelivery( communication.Id );
        expect( recipients ).toHaveLength( 1 );
        const status = RecipientStatus[ asEnumValue( recipients[ 0 ].Status, RecipientStatus ) ];
        expect( [ "Delivered", "Opened" ], `Recipient status ${status}: ${recipients[ 0 ].StatusNote ?? ""}` ).toContain( status );

        test.info().annotations.push( { type: "manual-check", description: `Edify (Dev domain) outgoing messages: subject '${subject}' to ${sgc.recipientEmail}, sent ${item.sendDate} (Rock time).` } );
    } );

    test( "EmailJob_ItemScheduledTomorrow_IsNotSent", async ( { sgc, cleanUpItems } ) => {
        const subject = `KFS E2E Scheduled Email future ${Date.now()}`;
        const item = await sgc.schedule( "email", { secondsFromNow: 24 * 60 * 60, recurrence: Recurrence.OneTime, subject, message: "<p>Should not be sent.</p>" } );
        cleanUpItems.push( item );

        const run = await sgc.runJobFor( item );

        expect( String( run.LastStatusMessage ) ).toMatch( /No communications to send/i );
        expect( await sgc.api.query( "Communications", `Subject eq '${subject}'` ) ).toHaveLength( 0 );
    } );

    test( "EmailJob_WeeklyItem_SendsAndMovesSendDateOneWeek", async ( { sgc, cleanUpItems } ) => {
        const stamp = `${Date.now()}`;
        const subject = `KFS E2E Scheduled Email weekly ${stamp}`;
        const item = await sgc.schedule( "email", { secondsFromNow: -30, recurrence: Recurrence.Weekly, subject, message: `<p>KFS end-to-end test ${stamp} (weekly). No action needed.</p>` } );
        cleanUpItems.push( item );

        const run = await sgc.runJobFor( item, { keepItem: true } );
        const sendDate = await sgc.getSendDate( item );
        await sgc.unschedule( item );

        expect( String( run.LastStatusMessage ) ).toMatch( /Sent 1 communication/i );
        expect( await sgc.api.query( "Communications", `Subject eq '${subject}'` ) ).toHaveLength( 1 );

        const expected = new Date( `${item.sendDate}Z` );
        expected.setUTCDate( expected.getUTCDate() + 7 );
        expect( sendDate, "Weekly items should move a week ahead after sending" ).toBe( expected.toISOString().substring( 0, 19 ) );

        test.info().annotations.push( { type: "manual-check", description: `Edify (Dev domain) outgoing messages: subject '${subject}' to ${sgc.recipientEmail}.` } );
    } );
} );
