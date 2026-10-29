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
import { RecipientStatus, SummaryEmail, expect, test } from "./fundraisingParticipantSummary.fixture";

/*
    Covers the manual plan: configure a "Fundraising Participant Summary" job, run it,
    and check the history shows the summary emails were sent. The test job is inactive
    and limited (Group setting) to a fundraising group the fixture creates, so it only
    ever emails the test participants. The Edify outgoing-messages check stays manual:
    each sending test records what to look for as a "manual-check" annotation.
*/
test.describe( "Fundraising Participant Summary: job", () => {
    test( "Run_GiftSinceLastRun_EmailsOnlyTheParticipantWithTheNewGift", async ( { fps } ) => {
        const { job, firstCommunicationId } = await fps.runJob( { lastRun: fps.addSeconds( fps.setupTime, -2 * 60 * 60 ), sendZeroDonations: false } );

        expect( String( job.LastStatus ), `Job result: ${job.LastStatusMessage}` ).toMatch( /Success/i );
        expect( String( job.LastStatusMessage ) ).toMatch( /\b1 Email sent\./ );

        const emails = await fps.getSummaryEmails( firstCommunicationId, 1 );
        expect( emails.map( email => email.personId ), "Only the participant with a gift since the last run should get a summary" ).toEqual( [ fps.donor.person.id ] );
        expectSummary( emails[ 0 ], { nickName: "Fpsdonor", groupName: fps.groupName, giftAmount: "$150.00" } );

        test.info().annotations.push( { type: "manual-check", description: `Edify (Dev domain) outgoing messages: '${emails[ 0 ].communication.Subject}' to ${fps.recipientEmail}.` } );
    } );

    test( "Run_SendEmailsWithZeroDonationsOn_EmailsEveryParticipantExceptOptedOut", async ( { fps } ) => {
        const { job, firstCommunicationId } = await fps.runJob( { lastRun: fps.addSeconds( fps.setupTime, -2 * 60 * 60 ), sendZeroDonations: true } );

        expect( String( job.LastStatus ), `Job result: ${job.LastStatusMessage}` ).toMatch( /Success/i );
        expect( String( job.LastStatusMessage ) ).toMatch( /\b2 Emails sent\./ );

        const emails = await fps.getSummaryEmails( firstCommunicationId, 2 );
        expect( emails.map( email => email.personId ).sort() ).toEqual( [ fps.donor.person.id, fps.quiet.person.id ].sort() );
        for ( const email of emails ) {
            const nickName = email.personId === fps.donor.person.id ? "Fpsdonor" : "Fpsquiet";
            expectSummary( email, { nickName, groupName: fps.groupName } );
        }

        test.info().annotations.push( { type: "manual-check", description: `Edify (Dev domain) outgoing messages: two '${emails[ 0 ].communication.Subject}' emails to ${fps.recipientEmail}.` } );
    } );

    // Not in the manual plan: the default, gifts-only mode with nothing new to report.
    test( "Run_NoGiftsSinceLastRun_SendsNoEmails", async ( { fps } ) => {
        const { job, firstCommunicationId } = await fps.runJob( { lastRun: fps.setupTime, sendZeroDonations: false } );

        expect( String( job.LastStatus ), `Job result: ${job.LastStatusMessage}` ).toMatch( /Success/i );
        expect( String( job.LastStatusMessage ) ).toMatch( /\b0 Emails sent\./ );
        expect( await fps.getSummaryEmails( firstCommunicationId, 0 ) ).toHaveLength( 0 );
    } );
} );

/** Checks one summary email: subject, greeting, group and (optionally) the gift listed, and delivery. */
function expectSummary( email: SummaryEmail, expected: { nickName: string; groupName: string; giftAmount?: string } ): void {
    const message = String( email.communication.Message ?? "" );

    expect( String( email.communication.Subject ) ).toMatch( /^Summary of Donations for \d{1,2}\/\d{1,2}\/\d{4} - \d{1,2}\/\d{1,2}\/\d{4}$/ );
    expect( message, "Summary should greet the participant" ).toContain( `${expected.nickName},` );
    expect( message, "Summary should name the fundraising group" ).toContain( expected.groupName );
    if ( expected.giftAmount ) {
        expect( message, "Summary should list the gift" ).toContain( expected.giftAmount );
    }

    const status = RecipientStatus[ asEnumValue( email.recipient.Status, RecipientStatus ) ];
    expect( [ "Delivered", "Opened" ], `Recipient status ${status}: ${email.recipient.StatusNote ?? ""}` ).toContain( status );
}
