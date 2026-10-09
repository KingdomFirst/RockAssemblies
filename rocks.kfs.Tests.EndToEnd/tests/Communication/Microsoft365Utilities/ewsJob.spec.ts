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
import { InboxItem } from "../../../shared/exchange";
import { MarkEmailBy, expect, test } from "./microsoft365Utilities.fixture";

/** The number of workflows a run reports ("Started 1 workflow", "No workflows started"). */
function startedCount( statusMessage: string ): number {
    const started = statusMessage.match( /Started (\d+) workflow/ );
    return started ? parseInt( started[ 1 ], 10 ) : /No workflows started/.test( statusMessage ) ? 0 : -1;
}

/** True when two times are the same instant, allowing for a whole-hour time zone difference in how they were written. */
function isSameReceivedTime( a: string, b: string ): boolean {
    const difference = Math.abs( Date.parse( a ) - Date.parse( b ) );
    return !Number.isNaN( difference ) && difference < 15 * 3_600_000 && difference % 3_600_000 < 60_000;
}

/*
    Covers the manual plan's "Test EWS Job": a "Launch Workflow From EWS Account" job with
    every setting filled in, run, then check the job history count, the workflow and that
    the email was marked as configured.

    The mailbox is the live calendar@ inbox, so the test touches exactly one email and puts
    it back:
      - Max Emails is 1 with no filter, so the job processes only the newest inbox email.
      - Before the run the test reads that email's read and flag state from Exchange, and
        picks a marking that will visibly change it (Read if unread, else Add Flag).
      - After the run it identifies the processed email by the workflow's ForeignKey (the
        email's Exchange id), checks the marking, and restores the original read and flag
        state in a finally block, then re-reads it to prove the restore.
    Email subjects and senders are compared, never printed.
*/
test.describe( "Microsoft 365 Utilities: Launch Workflow From EWS Account job", () => {
    test( "RunJob_MaxEmailsOne_LaunchesOneWorkflowForNewestEmailAndMarksIt", async ( { ewsJob } ) => {
        const { exchange } = ewsJob;

        const snapshot = await exchange.getNewestInboxItems( 5 );
        expect( snapshot.length, `The ${ewsJob.mailbox} inbox is empty; the job has nothing to process.` ).toBeGreaterThan( 0 );
        const newest = snapshot[ 0 ];

        let markEmailsBy: string = MarkEmailBy.Read;
        let expectedChange: ( item: InboxItem ) => boolean = item => item.isRead === true;
        if ( newest.isRead === true && newest.flagStatus === "NotFlagged" ) {
            markEmailsBy = MarkEmailBy.AddFlag;
            expectedChange = item => item.flagStatus === "Flagged";
        }
        const markingIsVisible = newest.isRead === false || markEmailsBy === MarkEmailBy.AddFlag;
        await ewsJob.setMarkEmailsBy( markEmailsBy );
        test.info().annotations.push( { type: "marking", description: markEmailsBy === MarkEmailBy.AddFlag ? "Add Flag (newest email was already read)" : "Read" } );

        // The email to put back, and its state before the run.
        let processed: InboxItem = newest;
        let original: InboxItem = newest;

        try {
            const { run, workflowsInRun } = await ewsJob.runOnce();

            // Job history: one workflow, and the count it reports matches.
            expect( workflowsInRun, `Job result: ${run.LastStatusMessage}` ).toHaveLength( 1 );
            expect( startedCount( String( run.LastStatusMessage ?? "" ) ), `Job result: ${run.LastStatusMessage}` ).toBe( 1 );

            const workflow = workflowsInRun[ 0 ];
            const foreignKey = String( workflow.ForeignKey ?? "" );
            expect( foreignKey, "The workflow has no ForeignKey (the email's Exchange id)" ).not.toBe( "" );

            const fromSnapshot = snapshot.find( item => item.id.endsWith( foreignKey ) );
            if ( fromSnapshot ) {
                processed = fromSnapshot;
                original = fromSnapshot;
            }
            else {
                // Arrived between the snapshot and the run: a new email starts unread and unflagged.
                const arrived = ( await exchange.getNewestInboxItems( 5 ) ).find( item => item.id.endsWith( foreignKey ) );
                expect( arrived, "Could not find the processed email in the inbox" ).toBeDefined();
                processed = arrived!;
                original = { ...arrived!, isRead: false, flagStatus: arrived!.flagStatus === null ? null : "NotFlagged" };
                test.info().annotations.push( { type: "warning", description: "A new email arrived during the test and was processed instead; it was restored to unread and unflagged." } );
            }
            expect( processed.id === newest.id || !fromSnapshot, "The job processed an older email, not the newest" ).toBe( true );

            // The workflow holds the email's properties (compared, not printed).
            expect( await ewsJob.getValue( workflow, "Subject" ) === processed.subject, "Workflow Subject does not match the email" ).toBe( true );
            expect( ( await ewsJob.getValue( workflow, "FromEmail" ) ).toLowerCase() === processed.fromEmail.toLowerCase(), "Workflow FromEmail does not match the email" ).toBe( true );
            expect( isSameReceivedTime( await ewsJob.getValue( workflow, "DateReceived" ), processed.received ), "Workflow DateReceived does not match the email" ).toBe( true );
            expect( String( workflow.Name ).toLowerCase().includes( `<${processed.fromEmail.toLowerCase()}>` ), "Workflow name does not include the sender" ).toBe( true );

            // The email was marked as configured.
            if ( markingIsVisible ) {
                expect( expectedChange( await exchange.getItem( processed.id ) ), "The job did not mark the email as configured" ).toBe( true );
            }
            else {
                test.info().annotations.push( { type: "marking", description: "The newest email was already read and flagged, so the marking could not be seen." } );
            }
        }
        finally {
            await ewsJob.deleteWorkflows();
            await restore( exchange, processed, original );
        }
    } );
} );

/** Puts the email's read and flag state back, changing only what differs, then proves it. */
async function restore( exchange: import( "../../../shared/exchange" ).ExchangeClient, item: InboxItem, original: InboxItem ): Promise<void> {
    const current = await exchange.getItem( item.id );
    await exchange.setState( item, {
        isRead: original.isRead !== null && current.isRead !== original.isRead ? original.isRead : null,
        flagStatus: original.flagStatus !== null && current.flagStatus !== original.flagStatus ? original.flagStatus : null
    } );

    const restored = await exchange.getItem( item.id );
    if ( restored.isRead !== original.isRead || restored.flagStatus !== original.flagStatus ) {
        throw new Error( `Could not restore the email received ${original.received}: it should be ${original.isRead ? "read" : "unread"} and ${original.flagStatus}, ` +
            `but is ${restored.isRead ? "read" : "unread"} and ${restored.flagStatus}. Fix it by hand in Outlook.` );
    }
}
