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
import { randomUUID } from "crypto";
import { getOptionalSetting, getTestSettings } from "../../../shared/config";
import { guardFixtureSetup } from "../../../shared/runGuard";
import { RockApi, RockEntity, asEnumValue } from "../../../shared/rockApi";
import { EntityTypeName, SystemGuid, TestNamePrefix } from "../../../shared/systemGuids";
import { TestJob, ensureTestJob, runJobNow, setJobLastRun } from "../../../shared/testData/jobs";
import { TestPerson, ensurePerson } from "../../../shared/testData/people";

/** Guids the plugin installs (rocks.kfs.ScheduledGroupCommunication/SystemGuid/Attribute.cs). */
export const PluginGuid = {
    emailTemplate: "7CFE297A-CE79-45D3-B4E0-D6BDAE723929",
    emailSendDate: "39A38B02-112C-4EDC-A30E-4BDB1B090EE4",
    emailRecurrence: "9BC4F790-A9F7-4822-AEEE-91095A3E3D4C",
    emailFromEmail: "F7C73002-6442-4756-BFDB-BC0BFE58EF15",
    emailFromName: "7BEE419A-8444-44E1-B7EB-451C038977B3",
    emailSubject: "9EC7C8A9-F4C9-421C-9129-2DD023E09D05",
    emailMessage: "8C4EE7A8-086D-42B7-908F-77A9A36E5342",
    smsTemplate: "B40C318D-0EB5-49CD-B93C-1CDA0F5CB4BC",
    smsSendDate: "B2125940-565B-42CE-82BE-CDA58FC65FDE",
    smsRecurrence: "EC6D13F7-A256-4B03-A94B-3B713F26E62D",
    smsFromSystemPhoneNumber: "A97CA985-53F5-4C0C-8827-FBB0B082162E",
    smsMessage: "C57166D5-C0D3-4DA6-88DD-92AFA5126D69"
} as const;

/** The plugin's Recurrence values ("0^OneTime,4^Daily,1^Weekly,2^BiWeekly,3^Monthly"). */
export const Recurrence = { OneTime: "0", Weekly: "1" } as const;

/** Fixed Guids for this suite's persistent test data. */
const TestGuid = {
    groupType: "5C6C0E2E-E2E0-4C0E-8C0E-0000000000F1",
    emailAttribute: "5C6C0E2E-E2E0-4C0E-8C0E-0000000000F2",
    smsAttribute: "5C6C0E2E-E2E0-4C0E-8C0E-0000000000F3",
    group: "5C6C0E2E-E2E0-4C0E-8C0E-0000000000F4",
    emailJob: "5C6C0E2E-E2E0-4C0E-8C0E-0000000000F5",
    smsJob: "5C6C0E2E-E2E0-4C0E-8C0E-0000000000F6",
    member: "5C6C0E2E-E2E0-4C0E-8C0E-0000000000F7"
} as const;

const JobClass = {
    email: "rocks.kfs.ScheduledGroupCommunication.Jobs.SendScheduledGroupEmail",
    sms: "rocks.kfs.ScheduledGroupCommunication.Jobs.SendScheduledGroupSMS"
} as const;

/** Rock's CommunicationRecipientStatus enum, in value order. */
export const RecipientStatus = [ "Pending", "Delivered", "Failed", "Cancelled", "Opened", "Sending" ] as const;

export type Channel = "email" | "sms";

/** One scheduled message on the test group, created by a test and removed right after. */
export type ScheduledItem = {
    channel: Channel;
    matrixId: number;
    itemId: number;
    /** Rock's clock when the item was created ("yyyy-MM-ddTHH:mm:ss"). */
    rockNow: string;
    sendDate: string;
};

export type ScheduledGroupCommunicationFixture = {
    api: RockApi;
    rockVersion: string;
    groupId: number;
    member: TestPerson;
    recipientEmail: string;
    /** Last four digits of the SMS test number, for messages and reports. */
    recipientSmsHint: string;
    fromEmail: string;
    fromName: string;
    smsFromNumber: RockEntity;
    jobs: Record<Channel, TestJob>;

    /** Adds a matrix item to the group's Scheduled Emails or Scheduled SMS matrix. */
    schedule( channel: Channel, args: { secondsFromNow: number; recurrence: string; subject?: string; message: string } ): Promise<ScheduledItem>;
    /** Removes the item (and its matrix) so no other Scheduled Group Communication job can send it. */
    unschedule( item: ScheduledItem ): Promise<void>;
    /**
     * Narrows the job's window to just before the item, presses Run Now and waits. Then
     * removes the item, unless keepItem is set (the cleanUpItems fixture removes it later).
     */
    runJobFor( item: ScheduledItem, options?: { keepItem?: boolean } ): Promise<RockEntity>;
    /**
     * Gets the item's Send Date Time as Rock parsed it ("yyyy-MM-ddTHH:mm:ss"). The job
     * rewrites the raw value in .NET's culture-specific format, so the raw text is not used.
     */
    getSendDate( item: ScheduledItem ): Promise<string | null>;
    /** Waits until Rock has finished sending to every recipient of the communication. */
    waitForDelivery( communicationId: number ): Promise<RockEntity[]>;
};

export const test = base.extend<{ cleanUpItems: ScheduledItem[] }, { sgc: ScheduledGroupCommunicationFixture }>( {
    sgc: [ async ( {}, use ) => guardFixtureSetup( "Scheduled Group Communication", async markSetupDone => {
        const settings = getTestSettings();
        const api = await RockApi.create();
        const rockVersion = await api.getRockVersion();
        console.log( `Rock version under test: ${rockVersion} at ${settings.baseUrl}` );

        try {
            const recipientEmail = requireSetting( "SGC_TEST_EMAIL", "an email address you can check in the Edify outgoing messages (Dev domain)" );
            const smsNumber = requireSetting( "SGC_TEST_SMS_NUMBER", "a 10-digit mobile number that can receive texts" ).replace( /\D/g, "" );

            const templates = await verifyPluginInstalled( api );
            const groupTypeId = await ensureGroupType( api, templates );
            const groupId = await ensureGroup( api, groupTypeId );
            const member = await ensureMember( api, groupId, recipientEmail, smsNumber );

            const fromEmail = getOptionalSetting( "SGC_FROM_EMAIL" ) ?? await getGlobalAttribute( api, "OrganizationEmail" );
            if ( !fromEmail ) {
                throw new Error( "No sender email: set SGC_FROM_EMAIL in .env, or the site's Organization Email global attribute." );
            }
            const fromName = getOptionalSetting( "SGC_FROM_NAME" ) ?? `${TestNamePrefix} Tests`;
            const smsFromNumber = await findSmsFromNumber( api );

            const jobs = {
                email: await ensureTestJob( api, { guid: TestGuid.emailJob, name: `${TestNamePrefix} Send Scheduled Group Email`, jobClass: JobClass.email } ),
                sms: await ensureTestJob( api, { guid: TestGuid.smsJob, name: `${TestNamePrefix} Send Scheduled Group SMS`, jobClass: JobClass.sms } )
            };

            const attributeIds = {
                email: await api.getIdByGuid( "Attributes", TestGuid.emailAttribute ),
                sms: await api.getIdByGuid( "Attributes", TestGuid.smsAttribute )
            };
            const matrixAttributeId = async ( guid: string ): Promise<number> => await api.getIdByGuid( "Attributes", guid );

            const schedule = async ( channel: Channel, args: { secondsFromNow: number; recurrence: string; subject?: string; message: string } ): Promise<ScheduledItem> => {
                const rockNow = await getRockNow( api, groupId );
                const sendDate = addSeconds( rockNow, args.secondsFromNow );

                const matrixGuid = randomUUID();
                const matrixId = await api.post( "/api/AttributeMatrices", { Guid: matrixGuid, AttributeMatrixTemplateId: templates[ channel ] } );
                const itemId = await api.post( "/api/AttributeMatrixItems", { Guid: randomUUID(), AttributeMatrixId: matrixId, Order: 0 } );

                const values: Array<[ string, string ]> = channel === "email"
                    ? [
                        [ PluginGuid.emailSendDate, sendDate ],
                        [ PluginGuid.emailRecurrence, args.recurrence ],
                        [ PluginGuid.emailFromEmail, fromEmail ],
                        [ PluginGuid.emailFromName, fromName ],
                        [ PluginGuid.emailSubject, args.subject ?? "" ],
                        [ PluginGuid.emailMessage, args.message ]
                    ]
                    : [
                        [ PluginGuid.smsSendDate, sendDate ],
                        [ PluginGuid.smsRecurrence, args.recurrence ],
                        [ PluginGuid.smsFromSystemPhoneNumber, String( smsFromNumber.Guid ).toLowerCase() ],
                        [ PluginGuid.smsMessage, args.message ]
                    ];
                for ( const [ guid, value ] of values ) {
                    await api.setAttributeValue( await matrixAttributeId( guid ), itemId, value );
                }

                // The group's matrix attribute holds the matrix Guid; that is how the job finds the group.
                await api.setAttributeValue( attributeIds[ channel ], groupId, matrixGuid );

                return { channel, matrixId, itemId, rockNow, sendDate };
            };

            const unschedule = async ( item: ScheduledItem ): Promise<void> => {
                await api.setAttributeValue( attributeIds[ item.channel ], groupId, "" );
                for ( const value of await api.query( "AttributeValues", `EntityId eq ${item.itemId}` ) ) {
                    const attribute = await api.getById( "Attributes", value.AttributeId as number );
                    if ( String( attribute.EntityTypeQualifierColumn ) === "AttributeMatrixTemplateId" ) {
                        await api.delete( "AttributeValues", value.Id );
                    }
                }
                await api.delete( "AttributeMatrixItems", item.itemId );
                await api.delete( "AttributeMatrices", item.matrixId );
            };

            const fixture: ScheduledGroupCommunicationFixture = {
                api,
                rockVersion,
                groupId,
                member,
                recipientEmail,
                recipientSmsHint: smsNumber.slice( -4 ),
                fromEmail,
                fromName,
                smsFromNumber,
                jobs,
                schedule,
                unschedule,
                runJobFor: async ( item, options = {} ) => {
                    // The job sends items dated from its last run to now; start the window a minute before the item.
                    await setJobLastRun( api, jobs[ item.channel ], addSeconds( item.sendDate, -60 ) );
                    try {
                        return await runJobNow( api, jobs[ item.channel ] );
                    }
                    finally {
                        // Remove the item at once, so the site's own scheduled jobs never send it a second time.
                        if ( !options.keepItem ) {
                            await unschedule( item );
                        }
                    }
                },
                getSendDate: async item => {
                    const guid = item.channel === "email" ? PluginGuid.emailSendDate : PluginGuid.smsSendDate;
                    const value = await api.first( "AttributeValues", `AttributeId eq ${await matrixAttributeId( guid )} and EntityId eq ${item.itemId}` );
                    return value?.ValueAsDateTime ? String( value.ValueAsDateTime ).substring( 0, 19 ) : null;
                },
                waitForDelivery: async communicationId => {
                    const deadline = Date.now() + 180_000;
                    let recipients: RockEntity[] = [];
                    while ( Date.now() < deadline ) {
                        recipients = await api.query( "CommunicationRecipients", `CommunicationId eq ${communicationId}` );
                        const busy = recipients.some( r => [ 0, 5 ].includes( asEnumValue( r.Status, RecipientStatus ) ) );
                        if ( recipients.length > 0 && !busy ) {
                            return recipients;
                        }
                        await new Promise( resolve => setTimeout( resolve, 3_000 ) );
                    }
                    return recipients;
                }
            };

            markSetupDone();
            await use( fixture );
        }
        finally {
            await api.dispose();
        }
    } ), { scope: "worker", timeout: 600_000 } ],

    /*
        Items a test schedules are normally removed by runJobFor right after the run. This
        removes any a test left behind (e.g. it failed before running the job), so no
        scheduled job on the site ever sends a test message later.
    */
    cleanUpItems: async ( { sgc }, use ) => {
        test.info().annotations.push( { type: "rock-version", description: sgc.rockVersion } );
        const items: ScheduledItem[] = [];
        await use( items );
        for ( const item of items ) {
            const stillThere = await sgc.api.query( "AttributeMatrixItems", `Id eq ${item.itemId}` );
            if ( stillThere.length > 0 ) {
                await sgc.unschedule( item ).catch( error => console.warn( `Could not remove scheduled test item ${item.itemId}: ${( error as Error ).message}` ) );
            }
        }
    }
} );

export { expect } from "@playwright/test";

function requireSetting( name: string, meaning: string ): string {
    const value = getOptionalSetting( name );
    if ( !value ) {
        throw new Error( `Set ${name} in rocks.kfs.Tests.EndToEnd/.env: ${meaning}. The Scheduled Group Communication tests send real messages to it.` );
    }
    return value;
}

/** Fails clearly if the plugin (or its 17.x System Phone Number migration) is not installed. */
async function verifyPluginInstalled( api: RockApi ): Promise<Record<Channel, number>> {
    const email = await api.getByGuid( "AttributeMatrixTemplates", PluginGuid.emailTemplate );
    const sms = await api.getByGuid( "AttributeMatrixTemplates", PluginGuid.smsTemplate );
    if ( !email || !sms ) {
        throw new Error( "The Scheduled Emails / Scheduled SMS Messages attribute matrix templates are missing. Is Scheduled Group Communication installed on the site?" );
    }
    for ( const guid of Object.values( PluginGuid ).filter( g => g !== PluginGuid.emailTemplate && g !== PluginGuid.smsTemplate ) ) {
        if ( !await api.getByGuid( "Attributes", guid ) ) {
            throw new Error( `Scheduled Group Communication matrix attribute ${guid} is missing; the plugin's migrations have not all run.` );
        }
    }
    return { email: email.Id, sms: sms.Id };
}

/** The test group type: the two Matrix group attributes the plugin needs, one per template. */
async function ensureGroupType( api: RockApi, templates: Record<Channel, number> ): Promise<number> {
    let groupType = await api.getByGuid( "GroupTypes", TestGuid.groupType );
    if ( !groupType ) {
        await api.post( "/api/GroupTypes", {
            Guid: TestGuid.groupType,
            Name: `${TestNamePrefix} Scheduled Communication`,
            Description: "Group type for the KFS end-to-end tests of Scheduled Group Communication. Do not use.",
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

    const groupEntityTypeId = await api.getEntityTypeId( EntityTypeName.Group );
    const matrixFieldTypeId = await api.getFieldTypeId( "Rock.Field.Types.MatrixFieldType" );
    for ( const [ guid, key, name, templateId ] of [
        [ TestGuid.emailAttribute, "KFSE2EScheduledEmails", "Scheduled Emails", templates.email ],
        [ TestGuid.smsAttribute, "KFSE2EScheduledSMS", "Scheduled SMS Messages", templates.sms ]
    ] as Array<[ string, string, string, number ]> ) {
        let attribute = await api.getByGuid( "Attributes", guid );
        if ( !attribute ) {
            await api.post( "/api/Attributes", {
                Guid: guid,
                EntityTypeId: groupEntityTypeId,
                EntityTypeQualifierColumn: "GroupTypeId",
                EntityTypeQualifierValue: String( groupTypeId ),
                FieldTypeId: matrixFieldTypeId,
                Key: key,
                Name: name,
                Description: "KFS end-to-end tests. Do not use.",
                IsGridColumn: false,
                IsMultiValue: false,
                IsRequired: false,
                IsSystem: false,
                Order: 0
            } );
            attribute = await api.getByGuid( "Attributes", guid );
        }

        const qualifier = await api.first( "AttributeQualifiers", `AttributeId eq ${attribute!.Id} and Key eq 'attributematrixtemplate'` );
        if ( !qualifier ) {
            await api.post( "/api/AttributeQualifiers", { AttributeId: attribute!.Id, Key: "attributematrixtemplate", Value: String( templateId ), IsSystem: false, Guid: randomUUID() } );
        }
    }

    return groupTypeId;
}

async function ensureGroup( api: RockApi, groupTypeId: number ): Promise<number> {
    const existing = await api.getByGuid( "Groups", TestGuid.group );
    if ( existing ) {
        return existing.Id;
    }

    return await api.post( "/api/Groups", {
        Guid: TestGuid.group,
        Name: `${TestNamePrefix} Scheduled Communication Group`,
        Description: "KFS end-to-end tests. Its only member receives the test emails and texts.",
        GroupTypeId: groupTypeId,
        IsActive: true,
        IsPublic: false,
        IsSystem: false,
        IsSecurityRole: false,
        Order: 0
    } );
}

/**
 * The group's only active member: a test person whose email and mobile number come from
 * .env, so the messages reach addresses the tester can check.
 */
async function ensureMember( api: RockApi, groupId: number, email: string, smsNumber: string ): Promise<TestPerson> {
    const member = await ensurePerson( api, {
        guid: TestGuid.member,
        firstName: "Sgc",
        lastName: "KFS E2E Tester",
        email
    } );
    await api.patch( "People", member.id, { Email: email, IsEmailActive: true, EmailPreference: 0 } );

    const mobileTypeId = await api.getIdByGuid( "DefinedValues", SystemGuid.DefinedValue.PhoneTypeMobile );
    const phones = await api.query( "PhoneNumbers", `PersonId eq ${member.id} and NumberTypeValueId eq ${mobileTypeId}` );
    if ( phones.length === 0 ) {
        await api.post( "/api/PhoneNumbers", { PersonId: member.id, Number: smsNumber, CountryCode: "1", NumberTypeValueId: mobileTypeId, IsMessagingEnabled: true, IsUnlisted: false, IsSystem: false } );
    }
    else if ( phones[ 0 ].Number !== smsNumber || !phones[ 0 ].IsMessagingEnabled ) {
        await api.patch( "PhoneNumbers", phones[ 0 ].Id, { Number: smsNumber, IsMessagingEnabled: true } );
    }

    const group = await api.getById( "Groups", groupId );
    const roleId = ( await api.getById( "GroupTypes", group.GroupTypeId as number ) ).DefaultGroupRoleId as number;
    for ( const other of await api.query( "GroupMembers", `GroupId eq ${groupId}` ) ) {
        if ( other.PersonId !== member.id ) {
            await api.delete( "GroupMembers", other.Id );
        }
    }
    const membership = await api.first( "GroupMembers", `GroupId eq ${groupId} and PersonId eq ${member.id}` );
    if ( !membership ) {
        await api.post( "/api/GroupMembers", { GroupId: groupId, PersonId: member.id, GroupRoleId: roleId, GroupMemberStatus: 1, IsSystem: false, Guid: randomUUID() } );
    }
    else if ( asEnumValue( membership.GroupMemberStatus, [ "Inactive", "Active", "Pending" ] ) !== 1 ) {
        await api.patch( "GroupMembers", membership.Id, { GroupMemberStatus: 1 } );
    }

    return member;
}

/** The System Phone Number to send from: SGC_SMS_FROM_NUMBER if set, else the first active, SMS-enabled one. */
async function findSmsFromNumber( api: RockApi ): Promise<RockEntity> {
    // The SMS transport (e.g. Twilio) is not exposed through REST; Active + SMS Enabled is what can be checked.
    const numbers = await api.query( "SystemPhoneNumbers", "IsActive eq true and IsSmsEnabled eq true", "&$orderby=Order" );
    const wanted = getOptionalSetting( "SGC_SMS_FROM_NUMBER" )?.replace( /\D/g, "" );
    const chosen = wanted ? numbers.find( n => String( n.Number ).replace( /\D/g, "" ).endsWith( wanted ) ) : numbers[ 0 ];
    if ( !chosen ) {
        throw new Error( wanted
            ? `No active, SMS-enabled System Phone Number matches SGC_SMS_FROM_NUMBER (${wanted}).`
            : "The site has no active, SMS-enabled System Phone Number to send from." );
    }
    return chosen;
}

async function getGlobalAttribute( api: RockApi, key: string ): Promise<string | null> {
    const attribute = await api.first( "Attributes", `Key eq '${key}' and EntityTypeId eq null` );
    if ( !attribute ) {
        return null;
    }
    // Global attribute values have no entity.
    const value = await api.first( "AttributeValues", `AttributeId eq ${attribute.Id} and EntityId eq null` );
    return ( value?.Value as string | undefined ) || ( attribute.DefaultValue as string | null ) || null;
}

/** Reads Rock's clock: saves the test group and returns the ModifiedDateTime Rock stamped ("yyyy-MM-ddTHH:mm:ss"). */
async function getRockNow( api: RockApi, groupId: number ): Promise<string> {
    await api.patch( "Groups", groupId, { Description: `KFS end-to-end tests. Its only member receives the test emails and texts. Last used ${new Date().toISOString()}.` } );
    return String( ( await api.getById( "Groups", groupId ) ).ModifiedDateTime ).substring( 0, 19 );
}

/** Adds seconds to a Rock local date-time string without involving the machine's time zone. */
function addSeconds( rockDateTime: string, seconds: number ): string {
    const date = new Date( `${rockDateTime.substring( 0, 19 )}Z` );
    date.setUTCSeconds( date.getUTCSeconds() + seconds );
    return date.toISOString().substring( 0, 19 );
}
