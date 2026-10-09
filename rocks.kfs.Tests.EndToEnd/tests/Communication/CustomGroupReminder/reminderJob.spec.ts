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
import { RecipientStatus, SentReminder, expect, jobName, test } from "./customGroupReminder.fixture";

/*
    Covers the manual plan: a group with a predictable schedule and "Group Meeting Reminder"
    = Yes; a "Custom Meeting Group Reminder" job with Days Prior covering the meeting and
    the Group Attribute set; run it with Send Using Email, then edit the job to SMS and run
    it again, and check the history shows the reminders were sent. Editing the job and Run
    Now happen in the browser, on copies of Rock's Scheduled Job Detail and Jobs
    Administration pages. The test job is inactive and its Group Attribute is the test
    group type's own attribute, so only the test groups are ever processed. The Edify and
    phone checks stay manual: each sending test adds a "manual-check" annotation.
*/
test.describe( "Custom Group Reminder: job", () => {
    test( "Run_SendUsingEmailMeetingTomorrow_EmailsMembersOfReminderGroupOnly", async ( { page, cgr } ) => {
        await cgr.editJob( page, { sendUsing: "Email", daysPrior: "1" } );
        const { job, firstCommunicationId, list } = await cgr.runJob( page );

        expect( String( job.LastStatus ), `Job result: ${job.LastStatusMessage}` ).toMatch( /Success/i );
        // One reminder per group per channel: the "No" group is left out.
        expect( String( job.LastStatusMessage ) ).toMatch( /\b1 meeting reminders sent\./ );
        expect( String( job.LastStatusMessage ) ).toMatch( /- 1 email\(s\)/ );
        await expect( list.row( jobName ), "Jobs Administration shows the result" ).toContainText( "1 meeting reminders sent." );

        const reminders = await cgr.getReminders( firstCommunicationId, 1 );
        expect( reminders.map( r => r.personId ), "Exactly one reminder, to the reminder group's member" ).toEqual( [ cgr.member.id ] );
        const [ reminder ] = reminders;
        expect( String( reminder.communication.Subject ) ).toBe( String( cgr.systemCommunication.Subject ) );
        expect( String( reminder.communication.Message ), "Reminder names the group" ).toContain( cgr.reminderGroup.name );
        expect( String( reminder.communication.Message ) ).not.toContain( cgr.noReminderGroup.name );
        expectDelivered( reminder );

        test.info().annotations.push( { type: "manual-check", description: `Edify (Dev domain) outgoing messages: '${reminder.communication.Subject}' to ${cgr.recipientEmail}, about '${cgr.reminderGroup.name}'.` } );
    } );

    // Not in the manual plan: Days Prior that does not reach the meeting.
    test( "Run_DaysPriorNotMatchingMeeting_SendsNothing", async ( { page, cgr } ) => {
        await cgr.editJob( page, { sendUsing: "Email", daysPrior: "2" } );
        const { job, firstCommunicationId, list } = await cgr.runJob( page );

        expect( String( job.LastStatus ), `Job result: ${job.LastStatusMessage}` ).toMatch( /Success/i );
        expect( String( job.LastStatusMessage ) ).toMatch( /\b0 meeting reminders sent\./ );
        await expect( list.row( jobName ) ).toContainText( "0 meeting reminders sent." );
        expect( await cgr.getReminders( firstCommunicationId, 0 ) ).toHaveLength( 0 );
    } );

    test( "EditSendUsingSmsAndRun_TextsMemberOfReminderGroup", async ( { page, cgr } ) => {
        expect( cgr.systemCommunication.SmsFromSystemPhoneNumberId,
            "One-time setup: the 'Group Meeting Reminder' system communication has no SMS From number, so Rock cannot send its texts " +
            "(Twilio: 'A From Number was not provided'). In Rock, Admin Tools > Communications > System Communications > Group Meeting Reminder: " +
            "choose an SMS-enabled number under SMS From, and save." ).not.toBeNull();

        await cgr.editJob( page, { sendUsing: "SMS", daysPrior: "1" } );
        const { job, firstCommunicationId, list } = await cgr.runJob( page );

        expect( String( job.LastStatus ), `Job result: ${job.LastStatusMessage}` ).toMatch( /Success/i );
        expect( String( job.LastStatusMessage ) ).toMatch( /\b1 meeting reminders sent\./ );
        expect( String( job.LastStatusMessage ) ).toMatch( /- 1 SMS message\(s\)/ );
        await expect( list.row( jobName ) ).toContainText( "1 SMS message(s)" );

        const reminders = await cgr.getReminders( firstCommunicationId, 1 );
        expect( reminders.map( r => r.personId ) ).toEqual( [ cgr.member.id ] );
        const [ reminder ] = reminders;
        expect( String( reminder.communication.SMSMessage ), "Text names the group" ).toContain( cgr.reminderGroup.name );
        expect( reminder.communication.SmsFromSystemPhoneNumberId ).toBe( cgr.systemCommunication.SmsFromSystemPhoneNumberId );
        expectDelivered( reminder );

        test.info().annotations.push( { type: "manual-check", description: `Phone ending ${cgr.recipientSmsHint}: text "Upcoming group meeting(s) for ${cgr.reminderGroup.name} ...".` } );
    } );
} );

/*
    10/6/2026 - CLAUDE

    The job sends through a system communication: the message goes straight to the email
    or SMS transport and Rock writes the communication record afterwards, marked Delivered
    whatever the transport did (no transport name or status note is recorded). So this
    check only proves Rock handed the message off; whether it reached Edify or the phone
    stays a manual check.

    Reason: History for system communications does not confirm transport delivery.
*/
function expectDelivered( reminder: SentReminder ): void {
    const status = RecipientStatus[ asEnumValue( reminder.recipient.Status, RecipientStatus ) ];
    expect( [ "Delivered", "Opened" ], `Recipient status ${status}: ${reminder.recipient.StatusNote ?? ""}` ).toContain( status );
}
