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
    Covers the manual plan's SMS steps: a One Time item on the group's Scheduled SMS
    matrix, with a System Phone Number to send from (Twilio on rockbeta), due now; run
    "Send Scheduled Group SMS (Plugin)"; the communication history shows it was sent. The
    plugin's 17.x change to System Phone Numbers is checked by the from-number assertion.
    Whether the text arrives on the phone stays a manual check ("manual-check" annotation).
*/
test.describe( "Scheduled Group Communication: SMS", () => {
    test( "SmsJob_OneTimeItemDueNow_SendsTextFromSystemPhoneNumber", async ( { sgc, cleanUpItems } ) => {
        const message = `KFS E2E scheduled SMS ${Date.now()}. Test message, no reply needed.`;
        const item = await sgc.schedule( "sms", { secondsFromNow: -30, recurrence: Recurrence.OneTime, message } );
        cleanUpItems.push( item );

        const run = await sgc.runJobFor( item );

        expect( String( run.LastStatus ), `Job result: ${run.LastStatusMessage}` ).toMatch( /Success/i );
        expect( String( run.LastStatusMessage ) ).toMatch( /Sent 1 communication/i );

        const communications = await sgc.api.query( "Communications", `SMSMessage eq '${message}'` );
        expect( communications, "The job should create exactly one communication" ).toHaveLength( 1 );
        const communication = communications[ 0 ];
        expect( communication.SmsFromSystemPhoneNumberId, "The text should go out from the chosen System Phone Number" ).toBe( sgc.smsFromNumber.Id );

        const recipients = await sgc.waitForDelivery( communication.Id );
        expect( recipients ).toHaveLength( 1 );
        const status = RecipientStatus[ asEnumValue( recipients[ 0 ].Status, RecipientStatus ) ];
        expect( [ "Delivered", "Opened" ], `Recipient status ${status}: ${recipients[ 0 ].StatusNote ?? ""}` ).toContain( status );

        test.info().annotations.push( { type: "manual-check", description: `Text "${message}" should arrive on the test phone ending ${sgc.recipientSmsHint}, from ${sgc.smsFromNumber.Number}.` } );
    } );

    test( "SmsJob_ItemScheduledTomorrow_IsNotSent", async ( { sgc, cleanUpItems } ) => {
        const message = `KFS E2E scheduled SMS future ${Date.now()}. Should not be sent.`;
        const item = await sgc.schedule( "sms", { secondsFromNow: 24 * 60 * 60, recurrence: Recurrence.OneTime, message } );
        cleanUpItems.push( item );

        const run = await sgc.runJobFor( item );

        expect( String( run.LastStatusMessage ) ).toMatch( /No communications to send/i );
        expect( await sgc.api.query( "Communications", `SMSMessage eq '${message}'` ) ).toHaveLength( 0 );
    } );
} );
