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
import { test as base } from "@playwright/test";
import { getOptionalSetting, getTestSettings } from "../../../shared/config";
import { guardFixtureSetup } from "../../../shared/runGuard";
import { RockApi, RockEntity, asEnumValue } from "../../../shared/rockApi";
import { EntityTypeName, SystemGuid, TestNamePrefix } from "../../../shared/systemGuids";
import { createGroupMemberContribution, deleteLeftoverTransactions, deleteTransaction } from "../../../shared/testData/finance";
import { GroupMemberStatus, TestGroup, addGroupMember, createGroup, deleteGroup, deleteLeftoverGroups } from "../../../shared/testData/groups";
import { ensureTestJob, runJobNow, setJobLastRun } from "../../../shared/testData/jobs";
import { TestPerson, ensurePerson } from "../../../shared/testData/people";

/** The plugin's system communication (rocks.kfs.FundraisingParticipantSummary/Migrations/001_AddSystemCommunication.cs). */
const SystemCommunicationGuid = "553B6FCA-9AFF-4618-BDE9-FF41A1EC689E";

const JobClass = "rocks.kfs.FundraisingParticipantSummary.Jobs.FundraisingParticipantSummary";

/** Fixed Guid of this suite's persistent, inactive test job. */
const TestJobGuid = "6D7D1F76-E2E0-4C0E-8C0E-0000000000E1";

/** Rock's CommunicationRecipientStatus enum, in value order. */
export const RecipientStatus = [ "Pending", "Delivered", "Failed", "Cancelled", "Opened", "Sending" ] as const;

const names = { group: "FPS Trip" };

/** A participant in the test group and the gift the fixture recorded for them. */
export type Participant = {
    person: TestPerson;
    groupMemberId: number;
    giftAmount: number;
    /** Rock local date-time of the gift. */
    giftDate: string;
};

/** A summary email the job sent, as recorded in communication history. */
export type SummaryEmail = {
    communication: RockEntity;
    personId: number;
    recipient: RockEntity;
};

export type FundraisingParticipantSummaryFixture = {
    api: RockApi;
    rockVersion: string;
    recipientEmail: string;
    groupName: string;

    /** Gave an hour before setup: inside a run that starts two hours back. */
    donor: Participant;
    /** Gave two days before setup: outside that run. */
    quiet: Participant;
    /** Gave an hour before setup but turned off contribution requests: never emailed. */
    optedOut: Participant;

    /** Rock's clock when the gifts were recorded ("yyyy-MM-ddTHH:mm:ss"). */
    setupTime: string;

    /**
     * Configures the test job for the test group, sets its last run (the start of the
     * window it reports on), presses Run Now and waits for the run to finish.
     */
    runJob( args: { lastRun: string; sendZeroDonations: boolean } ): Promise<{ job: RockEntity; firstCommunicationId: number }>;

    /**
     * Waits for the summary emails of a run to appear in communication history (Rock
     * records them shortly after sending), then waits for delivery.
     */
    getSummaryEmails( firstCommunicationId: number, expectedCount: number ): Promise<SummaryEmail[]>;

    addSeconds( rockDateTime: string, seconds: number ): string;
};

export const test = base.extend<{ annotateVersion: void }, { fps: FundraisingParticipantSummaryFixture }>( {
    fps: [ async ( {}, use ) => guardFixtureSetup( "Fundraising Participant Summary", async markSetupDone => {
        const settings = getTestSettings();
        const api = await RockApi.create();
        const rockVersion = await api.getRockVersion();
        console.log( `Rock version under test: ${rockVersion} at ${settings.baseUrl}` );

        // Clear leftovers from an earlier run that was killed before cleanup.
        await deleteLeftoverTransactions( api );
        await deleteLeftoverGroups( api, names.group );

        const recipientEmail = getOptionalSetting( "FPS_TEST_EMAIL" ) ?? getOptionalSetting( "SGC_TEST_EMAIL" );
        if ( !recipientEmail ) {
            throw new Error( "Set FPS_TEST_EMAIL (or SGC_TEST_EMAIL) in rocks.kfs.Tests.EndToEnd/.env: an address you can check in the Edify outgoing messages (Dev domain). The tests send real summary emails to it." );
        }

        const systemCommunication = await api.getByGuid( "SystemCommunications", SystemCommunicationGuid );
        if ( !systemCommunication ) {
            throw new Error( "The 'Fundraising Participant Summary' system communication is missing. Is the Fundraising Participant Summary plugin installed on the site?" );
        }

        const job = await ensureTestJob( api, { guid: TestJobGuid, name: `${TestNamePrefix} Fundraising Participant Summary`, jobClass: JobClass } );
        const jobSettingIds = await getJobSettingIds( api, JobClass );

        const people = {
            donor: await ensureRecipient( api, "9E3D2B76-5F8D-4B3E-A03F-9C4C5EBDAB31", "Fpsdonor", recipientEmail ),
            quiet: await ensureRecipient( api, "9E3D2B76-5F8D-4B3E-A03F-9C4C5EBDAB32", "Fpsquiet", recipientEmail ),
            optedOut: await ensureRecipient( api, "9E3D2B76-5F8D-4B3E-A03F-9C4C5EBDAB33", "Fpsoptout", recipientEmail )
        };

        const groupTypeId = await api.getIdByGuid( "GroupTypes", SystemGuid.GroupType.FundraisingOpportunity );
        const fundraisingAttributeId = async ( entityTypeName: string, key: string ): Promise<number> => ( await api.single( "Attributes",
            `EntityTypeId eq ${await api.getEntityTypeId( entityTypeName )} and EntityTypeQualifierColumn eq 'GroupTypeId' and EntityTypeQualifierValue eq '${groupTypeId}' and Key eq '${key}'` ) ).Id;

        const account = await api.first( "FinancialAccounts", "IsActive eq true", "&$orderby=Id" );
        if ( !account ) {
            throw new Error( "The site has no active financial account to record test contributions to." );
        }

        const created: { group?: TestGroup; transactionIds: number[] } = { transactionIds: [] };

        try {
            const group = await createGroup( api, { name: names.group, groupTypeGuid: SystemGuid.GroupType.FundraisingOpportunity } );
            created.group = group;
            await api.setAttributeValue( await fundraisingAttributeId( EntityTypeName.Group, "IndividualFundraisingGoal" ), group.id, "1000" );
            const groupName = String( ( await api.getById( "Groups", group.id ) ).Name );

            // Rock's clock, read from a record it just stamped, so gift dates and the job window
            // use the server's time zone rather than this machine's.
            await api.patch( "Groups", group.id, { Description: `KFS end-to-end tests. Created ${new Date().toISOString()}.` } );
            const setupTime = String( ( await api.getById( "Groups", group.id ) ).ModifiedDateTime ).substring( 0, 19 );

            const addParticipant = async ( person: TestPerson, giftAmount: number, giftDate: string ): Promise<Participant> => {
                const groupMemberId = await addGroupMember( api, { groupId: group.id, personId: person.id, status: GroupMemberStatus.Active } );
                created.transactionIds.push( await createGroupMemberContribution( api, {
                    groupMemberId,
                    authorizedPersonAliasId: person.primaryAliasId,
                    accountId: account.Id,
                    amount: giftAmount,
                    transactionDateTime: giftDate
                } ) );
                return { person, groupMemberId, giftAmount, giftDate };
            };

            const donor = await addParticipant( people.donor, 150, addSeconds( setupTime, -60 * 60 ) );
            const quiet = await addParticipant( people.quiet, 80, addSeconds( setupTime, -2 * 24 * 60 * 60 ) );
            const optedOut = await addParticipant( people.optedOut, 60, addSeconds( setupTime, -60 * 60 ) );
            await api.setAttributeValue( await fundraisingAttributeId( EntityTypeName.GroupMember, "DisablePublicContributionRequests" ), optedOut.groupMemberId, "True" );

            const configureJob = async ( sendZeroDonations: boolean ): Promise<void> => {
                const values: Record<string, string> = {
                    SystemCommunication: SystemCommunicationGuid.toLowerCase(),
                    GroupTypes: "",
                    Group: group.guid.toLowerCase(),
                    ShowAddress: "True",
                    ShowAmount: "True",
                    SendEmailswithZeroDonations: sendZeroDonations ? "True" : "False",
                    VerboseLogging: "False",
                    CommandTimeoutOverride: ""
                };
                for ( const [ key, value ] of Object.entries( values ) ) {
                    const attributeId = jobSettingIds[ key ];
                    if ( !attributeId ) {
                        throw new Error( `Job setting '${key}' does not exist for ${JobClass}. Known settings: ${Object.keys( jobSettingIds ).join( ", " )}` );
                    }
                    await api.setAttributeValue( attributeId, job.id, value );
                }
            };

            const fixture: FundraisingParticipantSummaryFixture = {
                api,
                rockVersion,
                recipientEmail,
                groupName,
                donor,
                quiet,
                optedOut,
                setupTime,
                runJob: async ( { lastRun, sendZeroDonations } ) => {
                    await configureJob( sendZeroDonations );
                    await setJobLastRun( api, job, lastRun );
                    const latest = await api.first( "Communications", "Id gt 0", "&$orderby=Id desc" );
                    const run = await runJobNow( api, job );
                    return { job: run, firstCommunicationId: ( latest?.Id ?? 0 ) + 1 };
                },
                getSummaryEmails: ( firstCommunicationId, expectedCount ) => getSummaryEmails( api, systemCommunication.Id, firstCommunicationId, expectedCount ),
                addSeconds
            };

            markSetupDone();
            await use( fixture );
        }
        finally {
            await cleanUp( api, created );
            await api.dispose();
        }
    } ), { scope: "worker", timeout: 600_000 } ],

    annotateVersion: [ async ( { fps }, use ) => {
        test.info().annotations.push( { type: "rock-version", description: fps.rockVersion } );
        await use();
    }, { auto: true } ]
} );

export { expect } from "@playwright/test";

/** Adds seconds to a Rock local date-time string without involving the machine's time zone. */
function addSeconds( rockDateTime: string, seconds: number ): string {
    const date = new Date( `${rockDateTime.substring( 0, 19 )}Z` );
    date.setUTCSeconds( date.getUTCSeconds() + seconds );
    return date.toISOString().substring( 0, 19 );
}

/** A persistent test person whose email is the address the tester checks in Edify. */
async function ensureRecipient( api: RockApi, guid: string, firstName: string, email: string ): Promise<TestPerson> {
    const person = await ensurePerson( api, { guid, firstName, lastName: "KFS E2E Tester", email } );
    await api.patch( "People", person.id, { Email: email, IsEmailActive: true, EmailPreference: 0 } );
    return person;
}

/** The job type's settings (attributes on ServiceJob qualified by its class), keyed by Key. */
async function getJobSettingIds( api: RockApi, jobClass: string ): Promise<Record<string, number>> {
    const jobEntityTypeId = await api.getEntityTypeId( "Rock.Model.ServiceJob" );
    const ids: Record<string, number> = {};
    for ( const attribute of await api.query( "Attributes", `EntityTypeId eq ${jobEntityTypeId} and EntityTypeQualifierColumn eq 'Class' and EntityTypeQualifierValue eq '${jobClass}'` ) ) {
        ids[ String( attribute.Key ) ] = attribute.Id;
    }
    if ( Object.keys( ids ).length === 0 ) {
        throw new Error( `No settings are registered for job type ${jobClass}. Is the Fundraising Participant Summary plugin installed (Rock registers a job type's settings when it loads the plugin)?` );
    }
    return ids;
}

async function getSummaryEmails( api: RockApi, systemCommunicationId: number, firstCommunicationId: number, expectedCount: number ): Promise<SummaryEmail[]> {
    const query = (): Promise<RockEntity[]> => api.query( "Communications", `SystemCommunicationId eq ${systemCommunicationId} and Id ge ${firstCommunicationId}`, "&$orderby=Id" );

    // Rock writes the communication records a few seconds after the job sends.
    let deadline = Date.now() + 120_000;
    let communications = await query();
    while ( communications.length < expectedCount && Date.now() < deadline ) {
        await new Promise( resolve => setTimeout( resolve, 3_000 ) );
        communications = await query();
    }
    // Give any unexpected extra emails from the same run time to show up too.
    await new Promise( resolve => setTimeout( resolve, 10_000 ) );
    communications = await query();

    const emails: SummaryEmail[] = [];
    for ( const communication of communications ) {
        deadline = Date.now() + 180_000;
        let recipients = await api.query( "CommunicationRecipients", `CommunicationId eq ${communication.Id}` );
        while ( recipients.some( r => [ 0, 5 ].includes( asEnumValue( r.Status, RecipientStatus ) ) ) && Date.now() < deadline ) {
            await new Promise( resolve => setTimeout( resolve, 3_000 ) );
            recipients = await api.query( "CommunicationRecipients", `CommunicationId eq ${communication.Id}` );
        }
        for ( const recipient of recipients ) {
            const alias = await api.getById( "PersonAlias", recipient.PersonAliasId as number );
            emails.push( { communication, personId: alias.PersonId as number, recipient } );
        }
    }

    return emails;
}

/** Best-effort teardown: keeps going past failures so one stuck record does not strand the rest. */
async function cleanUp( api: RockApi, created: { group?: TestGroup; transactionIds: number[] } ): Promise<void> {
    const steps: Array<[ string, () => Promise<void> ]> = [];

    for ( const transactionId of created.transactionIds ) {
        steps.push( [ `transaction ${transactionId}`, () => deleteTransaction( api, transactionId ) ] );
    }
    if ( created.group ) {
        const group = created.group;
        steps.push( [ `group ${group.id}`, () => deleteGroup( api, group.id ) ] );
    }

    for ( const [ name, step ] of steps ) {
        try {
            await step();
        }
        catch ( error ) {
            console.warn( `Cleanup of ${name} failed; the next run will retry it. ${( error as Error ).message}` );
        }
    }
}
