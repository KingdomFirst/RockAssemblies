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
import { Page, test as base } from "@playwright/test";
import { randomUUID } from "crypto";
import { getOptionalSetting, getTestSettings } from "../../../shared/config";
import { guardFixtureSetup } from "../../../shared/runGuard";
import { RockApi, RockEntity, asEnumValue } from "../../../shared/rockApi";
import { EntityTypeName, SystemGuid, TestNamePrefix } from "../../../shared/systemGuids";
import { GroupMemberStatus, addGroupMember, deleteGroup } from "../../../shared/testData/groups";
import { CopiedPage, createPageCopy, deleteBlockPage, deleteLeftoverTestPages, ensureTestPagesParent } from "../../../shared/testData/cms";
import { TestJob, ensureTestJob } from "../../../shared/testData/jobs";
import { openPageAsPerson } from "../../../shared/rockBrowser";
import { JobDetailPage, JobListPage } from "./jobPages";
import { TestPerson, ensurePerson } from "../../../shared/testData/people";

/** The plugin's system communication (rocks.kfs.CustomGroupCommunication/Guids/SystemCommunication.cs). */
export const SystemCommunicationGuid = "F2B18235-1BA3-421D-A62B-1713B38B7112";

const JobClass = "rocks.kfs.CustomGroupCommunication.Jobs.CustomMeetingGroupReminder";

/** The job's Send Using values ("1^Email,2^SMS,0^Recipient Preference"). */
export const SendUsing = { Email: "1", Sms: "2" } as const;

export const jobName = `${TestNamePrefix} Custom Meeting Group Reminder`;

/** Rock's Jobs Administration and Scheduled Job Detail pages and their (Obsidian) blocks. */
const CoreGuid = {
    jobListPage: "C58ADA1A-6322-4998-8FED-C3565DE87EFA",
    jobDetailPage: "E18AC09D-45CD-49CF-8874-157B32556B7D",
    jobListBlockType: "9B90F2D1-0C7B-4F08-A808-8BA4C9A70A20",
    jobDetailBlockType: "762F09EA-0A11-4BC7-9A68-13F0E44217C1"
} as const;

/** Rock's CommunicationRecipientStatus enum, in value order. */
export const RecipientStatus = [ "Pending", "Delivered", "Failed", "Cancelled", "Opened", "Sending" ] as const;

/** Fixed Guids for this suite's persistent test data. */
const TestGuid = {
    groupType: "B3C4D5E6-E2E0-4C0E-8C0E-0000000000A1",
    reminderAttribute: "B3C4D5E6-E2E0-4C0E-8C0E-0000000000A2",
    job: "B3C4D5E6-E2E0-4C0E-8C0E-0000000000A3",
    member: "B3C4D5E6-E2E0-4C0E-8C0E-0000000000A4"
} as const;

const names = {
    groupType: `${TestNamePrefix} Meeting Reminder`,
    groupPrefix: `${TestNamePrefix} CGR`,
    jobListPage: "CGR Jobs Administration",
    jobDetailPage: "CGR Scheduled Job Detail"
};

/** A reminder the job sent, as recorded in communication history. */
export type SentReminder = { communication: RockEntity; recipient: RockEntity; personId: number };

export type CustomGroupReminderFixture = {
    api: RockApi;
    rockVersion: string;
    member: TestPerson;
    recipientEmail: string;
    /** Last four digits of the SMS test number, for reports. */
    recipientSmsHint: string;
    /** Rock's date for "today" on the server ("yyyy-MM-dd"). */
    rockToday: string;

    /** Meets tomorrow; "Group Meeting Reminder" = Yes. */
    reminderGroup: { id: number; name: string };
    /** Same schedule and member; "Group Meeting Reminder" = No. */
    noReminderGroup: { id: number; name: string };

    systemCommunication: RockEntity;
    job: TestJob;

    /**
     * Copies of Rock's Jobs Administration and Scheduled Job Detail pages under the test pages,
     * where the test role has Edit (all both blocks require). The list links to the detail copy.
     */
    jobPages: { list: CopiedPage; detail: CopiedPage };

    /** Staff member who edits and runs the job in the browser. */
    tester: TestPerson;

    /**
     * Opens the test job in Scheduled Job Detail, changes the given settings the way a user
     * does, and saves.
     */
    editJob( page: Page, changes: { sendUsing?: "Email" | "SMS"; daysPrior?: string } ): Promise<void>;
    /**
     * Opens Jobs Administration, clicks Run Now on the test job and waits for the run to
     * finish. Returns the job record and where this run's communications start.
     */
    runJob( page: Page ): Promise<{ job: RockEntity; firstCommunicationId: number; list: JobListPage }>;
    /** Waits for the run's reminders to appear in communication history, then for delivery. */
    getReminders( firstCommunicationId: number, expectedCount: number ): Promise<SentReminder[]>;
};

export const test = base.extend<{ annotateVersion: void }, { cgr: CustomGroupReminderFixture }>( {
    cgr: [ async ( {}, use ) => guardFixtureSetup( "Custom Group Reminder", async markSetupDone => {
        const settings = getTestSettings();
        const api = await RockApi.create();
        const rockVersion = await api.getRockVersion();
        console.log( `Rock version under test: ${rockVersion} at ${settings.baseUrl}` );

        const createdGroups: number[] = [];
        const createdPages: number[] = [];
        try {
            const recipientEmail = getOptionalSetting( "CGR_TEST_EMAIL" ) ?? getOptionalSetting( "SGC_TEST_EMAIL" );
            const smsNumber = ( getOptionalSetting( "CGR_TEST_SMS_NUMBER" ) ?? getOptionalSetting( "SGC_TEST_SMS_NUMBER" ) ?? "" ).replace( /\D/g, "" );
            if ( !recipientEmail || !smsNumber ) {
                throw new Error( "Set SGC_TEST_EMAIL and SGC_TEST_SMS_NUMBER (or CGR_TEST_EMAIL / CGR_TEST_SMS_NUMBER) in rocks.kfs.Tests.EndToEnd/.env: the reminder tests send a real email and text to them." );
            }

            const systemCommunication = await api.getByGuid( "SystemCommunications", SystemCommunicationGuid );
            if ( !systemCommunication ) {
                throw new Error( "The 'Group Meeting Reminder' system communication is missing. Is the Custom Group Communication plugin installed on the site?" );
            }

            const { groupTypeId, reminderAttributeId } = await ensureGroupType( api );
            await deleteLeftoverGroups( api, groupTypeId );
            const member = await ensureMember( api, recipientEmail, smsNumber );
            const job = await ensureTestJob( api, { guid: TestGuid.job, name: jobName, jobClass: JobClass } );
            const tester = await ensurePerson( api, {
                guid: "6B0A9E43-2C5A-4E0B-9D0C-6F1F2B8A7E01",
                firstName: "Pafa",
                lastName: "KFS E2E Tester",
                email: "pafa.adult@kfs-e2e.invalid"
            } );
            const jobSettingIds = await getJobSettingIds( api );

            // Rock's own date (the server's time zone), read from a record it just stamped.
            const stampGroupId = await createReminderGroup( api, { groupTypeId, reminderAttributeId, label: "Reminder", reminder: true, weekday: 0 } );
            createdGroups.push( stampGroupId );
            const rockToday = String( ( await api.getById( "Groups", stampGroupId ) ).ModifiedDateTime ).substring( 0, 10 );
            const tomorrow = new Date( `${rockToday}T00:00:00Z` );
            tomorrow.setUTCDate( tomorrow.getUTCDate() + 1 );
            const tomorrowWeekday = tomorrow.getUTCDay();

            // Both groups meet weekly on tomorrow's weekday; only the first says Yes to reminders.
            await setWeeklySchedule( api, stampGroupId, tomorrowWeekday );
            const noReminderGroupId = await createReminderGroup( api, { groupTypeId, reminderAttributeId, label: "No Reminder", reminder: false, weekday: tomorrowWeekday } );
            createdGroups.push( noReminderGroupId );
            for ( const groupId of createdGroups ) {
                await addGroupMember( api, { groupId, personId: member.id, status: GroupMemberStatus.Active } );
            }

            const groupName = async ( id: number ): Promise<string> => String( ( await api.getById( "Groups", id ) ).Name );

            await deleteLeftoverTestPages( api, names.jobDetailPage );
            await deleteLeftoverTestPages( api, names.jobListPage );
            const testPagesParentId = await ensureTestPagesParent( api, settings.parentPageId );
            const list = await createPageCopy( api, { sourcePageGuid: CoreGuid.jobListPage, name: names.jobListPage, blockTypeId: await api.getIdByGuid( "BlockTypes", CoreGuid.jobListBlockType ), parentPageId: testPagesParentId } );
            createdPages.push( list.pageId );
            const detail = await createPageCopy( api, { sourcePageGuid: CoreGuid.jobDetailPage, name: names.jobDetailPage, blockTypeId: await api.getIdByGuid( "BlockTypes", CoreGuid.jobDetailBlockType ), parentPageId: list.pageId } );
            createdPages.unshift( detail.pageId );
            const detailPageGuid = String( ( await api.getById( "Pages", detail.pageId ) ).Guid ).toLowerCase();
            for ( const key of Object.keys( list.attributeIds ).filter( k => /detailpage/i.test( k ) ) ) {
                await api.setAttributeValue( list.attributeIds[ key ], list.blockId, detailPageGuid );
            }

            // The setup step of the manual plan: the job aimed at the test attribute, Email, one day prior.
            for ( const [ key, value ] of Object.entries( {
                SystemCommunication: SystemCommunicationGuid.toLowerCase(),
                DaysPrior: "1",
                SendUsing: SendUsing.Email,
                GroupAttributeSetting: TestGuid.reminderAttribute.toLowerCase()
            } ) ) {
                await api.setAttributeValue( jobSettingIds[ key ], job.id, value );
            }

            const fixture: CustomGroupReminderFixture = {
                api,
                rockVersion,
                member,
                recipientEmail,
                recipientSmsHint: smsNumber.slice( -4 ),
                rockToday,
                reminderGroup: { id: stampGroupId, name: await groupName( stampGroupId ) },
                noReminderGroup: { id: noReminderGroupId, name: await groupName( noReminderGroupId ) },
                systemCommunication,
                job,
                jobPages: { list, detail },
                tester,
                editJob: async ( page, changes ) => {
                    await openPageAsPerson( page, api, { pageId: detail.pageId, personId: tester.id, parameters: { ServiceJobId: job.id } } );
                    const form = new JobDetailPage( page, detail.blockId );
                    await form.waitForForm();
                    if ( changes.sendUsing ) {
                        await form.chooseOption( "Send Using", changes.sendUsing );
                    }
                    if ( changes.daysPrior !== undefined ) {
                        await form.textBox( "Days Prior" ).fill( changes.daysPrior );
                    }
                    await form.save();
                },
                runJob: async page => {
                    const before = await api.getById( "ServiceJobs", job.id );
                    const previousRun = String( before.LastRunDateTime ?? "" );
                    const latest = await api.first( "Communications", "Id gt 0", "&$orderby=Id desc" );

                    await openPageAsPerson( page, api, { pageId: list.pageId, personId: tester.id } );
                    const listPage = new JobListPage( page, list.blockId );
                    await listPage.runNow( jobName );

                    // Rock runs the job in the background; wait until it records a finished run.
                    const deadline = Date.now() + 180_000;
                    while ( Date.now() < deadline ) {
                        await new Promise( resolve => setTimeout( resolve, 2_000 ) );
                        const current = await api.getById( "ServiceJobs", job.id );
                        if ( String( current.LastRunDateTime ?? "" ) !== previousRun && current.LastStatus ) {
                            // Show the result as the list does after a refresh.
                            await openPageAsPerson( page, api, { pageId: list.pageId, personId: tester.id } );
                            await listPage.find( jobName );
                            return { job: current, firstCommunicationId: ( latest?.Id ?? 0 ) + 1, list: listPage };
                        }
                    }
                    throw new Error( `The job did not finish a run within 180s of clicking Run Now.` );
                },
                getReminders: ( firstCommunicationId, expectedCount ) => getReminders( api, systemCommunication.Id, firstCommunicationId, expectedCount )
            };

            markSetupDone();
            await use( fixture );
        }
        finally {
            for ( const groupId of createdGroups ) {
                await deleteReminderGroup( api, groupId ).catch( error => console.warn( `Cleanup of group ${groupId} failed; the next run will retry it. ${( error as Error ).message}` ) );
            }
            for ( const pageId of createdPages ) {
                await deleteBlockPage( api, { pageId } ).catch( error => console.warn( `Cleanup of page ${pageId} failed; the next run will retry it. ${( error as Error ).message}` ) );
            }
            await api.dispose();
        }
    } ), { scope: "worker", timeout: 600_000 } ],

    annotateVersion: [ async ( { cgr }, use ) => {
        test.info().annotations.push( { type: "rock-version", description: cgr.rockVersion } );
        await use();
    }, { auto: true } ]
} );

export { expect } from "@playwright/test";

/**
 * The test group type with its own Boolean "Group Meeting Reminder" group attribute. The
 * job only processes groups whose chosen attribute is Yes, so aiming it at this attribute
 * keeps real groups out of every run.
 */
async function ensureGroupType( api: RockApi ): Promise<{ groupTypeId: number; reminderAttributeId: number }> {
    let groupType = await api.getByGuid( "GroupTypes", TestGuid.groupType );
    if ( !groupType ) {
        await api.post( "/api/GroupTypes", {
            Guid: TestGuid.groupType,
            Name: names.groupType,
            Description: "Group type for the KFS end-to-end tests of Custom Group Reminder. Do not use.",
            GroupTerm: "Group",
            GroupMemberTerm: "Member",
            ShowInGroupList: true,
            ShowInNavigation: false,
            TakesAttendance: false,
            IsSystem: false,
            Order: 0
        } );
        groupType = await api.getByGuid( "GroupTypes", TestGuid.groupType );
    }
    const groupTypeId = groupType!.Id;

    let role = await api.first( "GroupTypeRoles", `GroupTypeId eq ${groupTypeId}`, "&$orderby=Order" );
    if ( !role ) {
        await api.post( "/api/GroupTypeRoles", { Guid: randomUUID(), GroupTypeId: groupTypeId, Name: "Member", Order: 0, IsLeader: false, IsSystem: false } );
        role = await api.first( "GroupTypeRoles", `GroupTypeId eq ${groupTypeId}`, "&$orderby=Order" );
    }
    if ( groupType!.DefaultGroupRoleId !== role!.Id ) {
        await api.patch( "GroupTypes", groupTypeId, { DefaultGroupRoleId: role!.Id } );
    }

    let attribute = await api.getByGuid( "Attributes", TestGuid.reminderAttribute );
    if ( !attribute ) {
        await api.post( "/api/Attributes", {
            Guid: TestGuid.reminderAttribute,
            EntityTypeId: await api.getEntityTypeId( EntityTypeName.Group ),
            EntityTypeQualifierColumn: "GroupTypeId",
            EntityTypeQualifierValue: String( groupTypeId ),
            FieldTypeId: await api.getFieldTypeId( "Rock.Field.Types.BooleanFieldType" ),
            Key: "KFSE2EGroupMeetingReminder",
            Name: "Group Meeting Reminder",
            Description: "KFS end-to-end tests. Do not use.",
            DefaultValue: "False",
            IsGridColumn: false,
            IsMultiValue: false,
            IsRequired: false,
            IsSystem: false,
            Order: 0
        } );
        attribute = await api.getByGuid( "Attributes", TestGuid.reminderAttribute );
    }

    return { groupTypeId, reminderAttributeId: attribute!.Id };
}

/** The persistent group member: a test person with the .env email and an SMS-enabled mobile number. */
async function ensureMember( api: RockApi, email: string, smsNumber: string ): Promise<TestPerson> {
    const member = await ensurePerson( api, { guid: TestGuid.member, firstName: "Cgrmember", lastName: "KFS E2E Tester", email } );
    await api.patch( "People", member.id, { Email: email, IsEmailActive: true, EmailPreference: 0 } );

    const mobileTypeId = await api.getIdByGuid( "DefinedValues", SystemGuid.DefinedValue.PhoneTypeMobile );
    const phones = await api.query( "PhoneNumbers", `PersonId eq ${member.id} and NumberTypeValueId eq ${mobileTypeId}` );
    if ( phones.length === 0 ) {
        await api.post( "/api/PhoneNumbers", { PersonId: member.id, Number: smsNumber, CountryCode: "1", NumberTypeValueId: mobileTypeId, IsMessagingEnabled: true, IsUnlisted: false, IsSystem: false } );
    }
    else if ( phones[ 0 ].Number !== smsNumber || !phones[ 0 ].IsMessagingEnabled ) {
        await api.patch( "PhoneNumbers", phones[ 0 ].Id, { Number: smsNumber, IsMessagingEnabled: true } );
    }

    return member;
}

/** Creates a test group with a weekly schedule and the reminder attribute set to Yes or No. */
async function createReminderGroup( api: RockApi, args: { groupTypeId: number; reminderAttributeId: number; label: string; reminder: boolean; weekday: number } ): Promise<number> {
    const name = `${names.groupPrefix} ${args.label} ${Date.now()}`;
    const scheduleId = await api.post( "/api/Schedules", {
        Guid: randomUUID(),
        Name: name,
        Description: "KFS end-to-end tests.",
        iCalendarContent: "",
        WeeklyDayOfWeek: args.weekday,
        WeeklyTimeOfDay: "19:00:00",
        IsActive: true
    } );
    const groupId = await api.post( "/api/Groups", {
        Guid: randomUUID(),
        Name: name,
        Description: "KFS end-to-end tests. Safe to delete.",
        GroupTypeId: args.groupTypeId,
        ScheduleId: scheduleId,
        IsActive: true,
        IsPublic: false,
        IsSystem: false,
        IsSecurityRole: false,
        Order: 0
    } );
    await api.setAttributeValue( args.reminderAttributeId, groupId, args.reminder ? "True" : "False" );
    return groupId;
}

async function setWeeklySchedule( api: RockApi, groupId: number, weekday: number ): Promise<void> {
    const scheduleId = ( await api.getById( "Groups", groupId ) ).ScheduleId as number;
    await api.patch( "Schedules", scheduleId, { WeeklyDayOfWeek: weekday } );
}

async function deleteReminderGroup( api: RockApi, groupId: number ): Promise<void> {
    const scheduleId = ( await api.getById( "Groups", groupId ) ).ScheduleId as number | null;
    await deleteGroup( api, groupId );
    if ( scheduleId ) {
        await api.delete( "Schedules", scheduleId );
    }
}

async function deleteLeftoverGroups( api: RockApi, groupTypeId: number ): Promise<void> {
    for ( const group of await api.query( "Groups", `GroupTypeId eq ${groupTypeId}` ) ) {
        await deleteReminderGroup( api, group.Id );
    }
}

/** The job type's settings (attributes on ServiceJob qualified by its class), keyed by Key. */
async function getJobSettingIds( api: RockApi ): Promise<Record<string, number>> {
    const jobEntityTypeId = await api.getEntityTypeId( "Rock.Model.ServiceJob" );
    const ids: Record<string, number> = {};
    for ( const attribute of await api.query( "Attributes", `EntityTypeId eq ${jobEntityTypeId} and EntityTypeQualifierColumn eq 'Class' and EntityTypeQualifierValue eq '${JobClass}'` ) ) {
        ids[ String( attribute.Key ) ] = attribute.Id;
    }
    for ( const key of [ "SystemCommunication", "DaysPrior", "SendUsing", "GroupAttributeSetting" ] ) {
        if ( !ids[ key ] ) {
            throw new Error( `Job setting '${key}' is not registered for ${JobClass}. Known settings: ${Object.keys( ids ).join( ", " ) || "(none)"}. Is the Custom Group Communication plugin installed?` );
        }
    }
    return ids;
}

async function getReminders( api: RockApi, systemCommunicationId: number, firstCommunicationId: number, expectedCount: number ): Promise<SentReminder[]> {
    const query = (): Promise<RockEntity[]> => api.query( "Communications", `SystemCommunicationId eq ${systemCommunicationId} and Id ge ${firstCommunicationId}`, "&$orderby=Id" );

    // Rock writes communication records a few seconds after sending.
    let deadline = Date.now() + 120_000;
    let communications = await query();
    while ( communications.length < expectedCount && Date.now() < deadline ) {
        await new Promise( resolve => setTimeout( resolve, 3_000 ) );
        communications = await query();
    }
    // Give any unexpected extra reminders from the same run time to show up too.
    await new Promise( resolve => setTimeout( resolve, 10_000 ) );
    communications = await query();

    const reminders: SentReminder[] = [];
    for ( const communication of communications ) {
        deadline = Date.now() + 180_000;
        let recipients = await api.query( "CommunicationRecipients", `CommunicationId eq ${communication.Id}` );
        while ( recipients.some( r => [ 0, 5 ].includes( asEnumValue( r.Status, RecipientStatus ) ) ) && Date.now() < deadline ) {
            await new Promise( resolve => setTimeout( resolve, 3_000 ) );
            recipients = await api.query( "CommunicationRecipients", `CommunicationId eq ${communication.Id}` );
        }
        for ( const recipient of recipients ) {
            const alias = await api.getById( "PersonAlias", recipient.PersonAliasId as number );
            reminders.push( { communication, recipient, personId: alias.PersonId as number } );
        }
    }
    return reminders;
}
