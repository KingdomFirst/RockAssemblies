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
import * as fs from "fs";
import * as path from "path";
import { getOptionalSetting, getTestSettings } from "../../../shared/config";
import { ExchangeClient } from "../../../shared/exchange";
import { guardFixtureSetup } from "../../../shared/runGuard";
import { RockApi, RockEntity } from "../../../shared/rockApi";
import { TestNamePrefix } from "../../../shared/systemGuids";
import { getGlobalAttributeValue } from "../../../shared/testData/attributes";
import { TestJob, ensureTestJob, getJobSettingIds, runJobNow } from "../../../shared/testData/jobs";
import { createAttributeOnlyWorkflowType, deleteLeftoverWorkflowTypes, deleteWorkflowType, deleteWorkflowsOfType } from "../../../shared/testData/workflows";

export const JobClass = "rocks.kfs.Microsoft365Utilities.Jobs.LaunchWorkflowFromEWSAccount";

/** The plugin's three global attributes (Migrations/001_CreateEWSAttributes.cs), all Encrypted Text. */
export const EwsGlobalAttribute = {
    applicationId: { guid: "467C0F95-BAB9-49E9-B675-6AD59EB221D8", key: "rocks.kfs.EWSAppApplicationId", name: "EWS Azure Application ID" },
    tenantId: { guid: "12D2D842-8806-402D-BCD7-E9E81D9FDEAB", key: "rocks.kfs.EWSAppTenantId", name: "EWS Azure Tenant ID" },
    secret: { guid: "8CDE7476-506B-4F39-B2BE-5E4102F3C9F3", key: "rocks.kfs.EWSAppSecret", name: "EWS Azure Secret" }
} as const;

/** Values of the job's MarkEmailBy enum. */
export const MarkEmailBy = { Read: "0", AddFlag: "1", RemoveFlag: "2" } as const;

const TestJobGuid = "B3651E2E-E2E0-4C0E-8C0E-0000000003A1";

const workflowTypeName = "M365 Email Workflow";

/** The email properties the job can map, each to the workflow attribute of the same key. */
export const EmailProperties = [ "DateReceived", "FromEmail", "FromName", "Subject", "Body" ] as const;

/** The one calendar item a render prints. Subjects are personal data: compare, never print. */
export type RenderedCalendarItem = { subject: string; start: string; end: string; isRecurring: boolean };

/** The result of rendering a template. */
export type LavaRender = {
    output: string;
    /**
     * Microsoft errors (sign-in, Exchange) Rock logged while rendering. Before the compat
     * fix the shortcode showed only "One or more errors occurred."; the reason is always
     * in Rock's exception log.
     */
    loggedErrors: string[];
};

/** The result of rendering the shortcode with the tests' item template. */
export type ShortcodeRender = LavaRender & {
    /** The first calendar item; the template prints only one. */
    item: RenderedCalendarItem | null;
    /** How many items the shortcode returned (a count only). */
    itemCount: number;
    /** Rock's date ("yyyy-MM-dd") when the template ran, from the same render. */
    rockToday: string;
};

/** Shortcode parameters a test can set; the credential parameters always come from the global attributes. */
export type ShortcodeParameters = {
    calendarmailbox?: string;
    impersonate?: string;
    serverurl?: string;
    order?: string;
    daysback?: number;
    daysforward?: number;
    /** Replaces the global secret, to test a bad credential. */
    appsecret?: string;
};

/** Everything the shortcode tests need. Nothing is created on the site. */
export type ShortcodeFixture = {
    api: RockApi;
    rockVersion: string;
    mailbox: string;
    impersonate: string;

    /** The usage example from the shortcode's documentation, as shipped in the plugin source. */
    documentationExample: string;

    /** Renders a template through Rock's Lava engine. */
    render( template: string ): Promise<LavaRender>;

    /** Renders the shortcode with the given parameters and the tests' one-item template. */
    renderItem( parameters?: ShortcodeParameters ): Promise<ShortcodeRender>;
};

/** Everything the EWS job tests need, built once per run. */
export type EwsJobFixture = {
    api: RockApi;
    job: TestJob;
    mailbox: string;
    /** The test's own connection to the mailbox, to read and restore the email the job touches. */
    exchange: ExchangeClient;
    workflowType: { id: number; guid: string; attributeIds: Record<string, number> };

    /** Sets how the job marks the email it processes. */
    setMarkEmailsBy( value: string ): Promise<void>;

    /** Runs the job once and returns the run and the workflows it launched. */
    runOnce(): Promise<{ run: RockEntity; workflowsInRun: RockEntity[] }>;

    /** Gets a workflow attribute value by key. */
    getValue( workflow: RockEntity, key: typeof EmailProperties[ number ] ): Promise<string>;

    /** Deletes the test workflows: they hold a copy of a real email. */
    deleteWorkflows(): Promise<void>;
};

/*
    10/6/2026 - CLAUDE

    KFS tests against its live calendar@ mailbox (configuring a separate Microsoft 365
    mailbox and calendar is not practical). So the tests touch as little as possible:
    the shortcode tests print one calendar item at most, and the job tests run with
    Max Emails 1, check which single email was processed, and put that email's read and
    flag state back (see ewsJob.spec.ts).

    Reason: The mailbox under test is a live one.
*/

export const test = base.extend<{}, { shortcode: ShortcodeFixture; ewsJob: EwsJobFixture }>( {
    shortcode: [ async ( { }, use ) => guardFixtureSetup( "Microsoft 365 Utilities shortcode", async markSetupDone => {
        const settings = getTestSettings();
        const api = await RockApi.create();

        try {
            const rockVersion = await api.getRockVersion();
            console.log( `Rock version under test: ${rockVersion} at ${settings.baseUrl}` );
            const { mailbox, impersonate } = getMailboxSettings();

            const render = async ( template: string ): Promise<LavaRender> => {
                const lastLogId = ( await api.first( "ExceptionLogs", "Id gt 0", "&$orderby=Id desc" ) )?.Id ?? 0;
                const output = await api.renderLava( template );
                const logged = await api.query( "ExceptionLogs", `Id gt ${lastLogId}`, "&$orderby=Id" );
                const loggedErrors = logged
                    .filter( entry => String( entry.ExceptionType ?? "" ).startsWith( "Microsoft." ) )
                    .map( entry => `${entry.ExceptionType}: ${String( entry.Description ?? "" ).replace( /\s+/g, " " ).slice( 0, 600 )}` );
                return { output, loggedErrors };
            };

            const renderItem = async ( parameters: ShortcodeParameters = {} ): Promise<ShortcodeRender> => {
                const { output, loggedErrors } = await render( buildItemTemplate( {
                    calendarmailbox: mailbox,
                    impersonate,
                    ...parameters
                } ) );

                const itemText = output.match( /<kfs-item>([\s\S]*?)<\/kfs-item>/ )?.[ 1 ];
                let item: RenderedCalendarItem | null = null;
                if ( itemText !== undefined ) {
                    // The subject may contain the separator, so take the last three fields from the end.
                    const fields = itemText.split( "~|~" );
                    const [ start, end, isRecurring ] = fields.slice( -3 );
                    item = { subject: fields.slice( 0, -3 ).join( "~|~" ), start, end, isRecurring: isRecurring.trim().toLowerCase() === "true" };
                }

                return {
                    output,
                    loggedErrors,
                    item,
                    itemCount: parseInt( output.match( /<kfs-count>(\d+)<\/kfs-count>/ )?.[ 1 ] ?? "0", 10 ),
                    rockToday: output.match( /<kfs-today>(.*?)<\/kfs-today>/ )?.[ 1 ] ?? ""
                };
            };

            markSetupDone();
            await use( {
                api,
                rockVersion,
                mailbox,
                impersonate,
                documentationExample: readDocumentationExample(),
                render,
                renderItem
            } );
        }
        finally {
            await api.dispose();
        }
    } ), { scope: "worker", timeout: 300_000 } ],

    ewsJob: [ async ( { }, use ) => guardFixtureSetup( "Microsoft 365 Utilities EWS job", async markSetupDone => {
        const api = await RockApi.create();
        let workflowTypeId: number | null = null;

        try {
            const { mailbox, impersonate } = getMailboxSettings();

            // The test's own Exchange connection. Rock keeps the plugin's credentials encrypted, so the test needs its own copy.
            const applicationId = getOptionalSetting( "M365_APP_ID" );
            const tenantId = getOptionalSetting( "M365_TENANT_ID" );
            const secret = getOptionalSetting( "M365_APP_SECRET" );
            if ( !applicationId || !tenantId || !secret ) {
                throw new Error( "Set M365_APP_ID, M365_TENANT_ID and M365_APP_SECRET in .env (the same Azure app as the EWS global attributes). " +
                    "The job tests use them to see which email the job processed and to put its read and flag state back afterwards." );
            }
            const exchange = new ExchangeClient( { applicationId, tenantId, secret }, mailbox, impersonate );

            const settingIds = await getJobSettingIds( api, JobClass );
            const requiredSettings = [ "ApplicationId", "TenantId", "ApplicationSecret", "EmailAddress", "ImpersonateUser", "ServerUrl", "MaxEmails", "LaunchWorkflowsWith", "MarkEmailsBy", "OneWorkflowPerConversation", "WorkflowType", "WorkflowAttributes" ];
            const missing = requiredSettings.filter( key => !settingIds[ key ] );
            if ( missing.length > 0 ) {
                throw new Error( `The 'Launch Workflow From EWS Account' job type has no setting(s) ${missing.join( ", " )} on this site. Is Microsoft 365 Utilities installed?` );
            }
            const job = await ensureTestJob( api, { guid: TestJobGuid, name: `${TestNamePrefix} Launch Workflow From EWS Account`, jobClass: JobClass } );

            await deleteLeftoverWorkflowTypes( api, workflowTypeName );
            const workflowType = await createAttributeOnlyWorkflowType( api, { name: workflowTypeName, attributeKeys: [ ...EmailProperties ] } );
            workflowTypeId = workflowType.id;

            // The job's credentials are copied as stored (encrypted); the job decrypts them with the same site key.
            const copyGlobal = async ( attribute: { key: string; name: string } ): Promise<string> => {
                const value = await getGlobalAttributeValue( api, attribute.key );
                if ( !value ) {
                    throw new Error( `The global attribute '${attribute.name}' has no value.` );
                }
                return value;
            };

            const values: Record<string, string> = {
                ApplicationId: await copyGlobal( EwsGlobalAttribute.applicationId ),
                TenantId: await copyGlobal( EwsGlobalAttribute.tenantId ),
                ApplicationSecret: await copyGlobal( EwsGlobalAttribute.secret ),
                EmailAddress: mailbox,
                ImpersonateUser: impersonate,
                ServerUrl: "https://outlook.office365.com/EWS/Exchange.asmx",
                // Exactly one email per run: the newest in the inbox, whatever its state.
                MaxEmails: "1",
                LaunchWorkflowsWith: "",
                MarkEmailsBy: MarkEmailBy.Read,
                OneWorkflowPerConversation: "False",
                WorkflowType: workflowType.guid.toLowerCase(),
                WorkflowAttributes: EmailProperties.map( key => `${key}^${key}` ).join( "|" )
            };
            for ( const [ key, value ] of Object.entries( values ) ) {
                await api.setAttributeValue( settingIds[ key ], job.id, value );
            }

            const getValue = async ( workflow: RockEntity, key: typeof EmailProperties[ number ] ): Promise<string> =>
                await api.getAttributeValue( workflowType.attributeIds[ key ], workflow.Id ) ?? "";

            markSetupDone();
            await use( {
                api,
                job,
                mailbox,
                exchange,
                workflowType,
                setMarkEmailsBy: async value => {
                    await api.setAttributeValue( settingIds.MarkEmailsBy, job.id, value );
                },
                runOnce: async () => {
                    const before = ( await api.first( "Workflows", `WorkflowTypeId eq ${workflowType.id}`, "&$orderby=Id desc" ) )?.Id ?? 0;
                    const run = await runJobNow( api, job );
                    if ( String( run.LastStatus ) !== "Success" ) {
                        throw new Error( `The job run ended with status ${run.LastStatus}: ${String( run.LastStatusMessage ?? "" ).slice( 0, 1000 )}` );
                    }
                    return { run, workflowsInRun: await api.query( "Workflows", `WorkflowTypeId eq ${workflowType.id} and Id gt ${before}`, "&$orderby=Id" ) };
                },
                getValue,
                deleteWorkflows: async () => {
                    await deleteWorkflowsOfType( api, workflowType.id );
                }
            } );
        }
        finally {
            if ( workflowTypeId ) {
                await deleteWorkflowType( api, workflowTypeId ).catch( error => console.warn( `Cleanup of the workflow type failed; the next run will retry it. ${( error as Error ).message}` ) );
            }
            await api.dispose();
        }
    } ), { scope: "worker", timeout: 300_000 } ]
} );

export { expect } from "@playwright/test";

/** The mailbox under test (calendar and inbox) and the account to impersonate, from .env. */
function getMailboxSettings(): { mailbox: string; impersonate: string } {
    const mailbox = getOptionalSetting( "M365_MAILBOX" );
    if ( !mailbox ) {
        throw new Error( "Set M365_MAILBOX in .env to the Microsoft 365 mailbox whose calendar and inbox the tests read (KFS: the calendar@ mailbox), and M365_IMPERSONATE to the account with access to it." );
    }

    return { mailbox, impersonate: getOptionalSetting( "M365_IMPERSONATE" ) ?? "" };
}

/**
 * Reads the usage example from the shortcode's Documentation in the plugin source: the
 * same text a tester copies from CMS Configuration > Lava Shortcodes.
 */
function readDocumentationExample(): string {
    const sourcePath = path.join( __dirname, "..", "..", "..", "..", "rocks.kfs.Microsoft365Utilities", "Shortcodes", "CalendarItemsShortcode.cs" );
    const source = fs.readFileSync( sourcePath, "utf8" );
    const example = source.match( /<pre>([\s\S]*?)<\/pre>/ )?.[ 1 ];
    if ( !example ) {
        throw new Error( `No <pre> usage example in the shortcode documentation (${sourcePath}).` );
    }

    // The documentation is a C# verbatim string, where "" is one quote.
    return example.replace( /""/g, "\"" );
}

/** Builds a template that calls the shortcode the documented way and prints the count and the first item only. */
function buildItemTemplate( parameters: ShortcodeParameters ): string {
    const shortcodeParameters = [
        "applicationid:'{{ applicationid }}'",
        "tenantid:'{{ tenantid }}'",
        parameters.appsecret ? `appsecret:'${parameters.appsecret}'` : "appsecret:'{{ appsecret }}'"
    ];
    for ( const key of [ "calendarmailbox", "impersonate", "serverurl", "order", "daysback", "daysforward" ] as const ) {
        const value = parameters[ key ];
        // The shortcode ignores empty values (key:''), so leave those out.
        if ( value !== undefined && value !== "" ) {
            shortcodeParameters.push( `${key}:'${value}'` );
        }
    }

    return [
        `{% assign applicationid = 'Global' | Attribute:'${EwsGlobalAttribute.applicationId.key}' %}`,
        `{% assign tenantid = 'Global' | Attribute:'${EwsGlobalAttribute.tenantId.key}' %}`,
        `{% assign appsecret = 'Global' | Attribute:'${EwsGlobalAttribute.secret.key}', 'RawValue' %}`,
        `{[ ewscalendaritems ${shortcodeParameters.join( " " )} ]}`,
        "<kfs-count>{{ CalendarItems | Size }}</kfs-count>",
        "{% for calItem in CalendarItems limit:1 %}<kfs-item>{{ calItem.Subject | Escape }}~|~{{ calItem.Start | Date:'yyyy-MM-ddTHH:mm:ss' }}~|~{{ calItem.End | Date:'yyyy-MM-ddTHH:mm:ss' }}~|~{{ calItem.IsRecurring }}</kfs-item>{% endfor %}",
        "{[ endewscalendaritems ]}",
        "<kfs-today>{{ 'Now' | Date:'yyyy-MM-dd' }}</kfs-today>"
    ].join( "\n" );
}
