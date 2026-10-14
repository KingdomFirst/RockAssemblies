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

/*
    10/6/2026 - CLAUDE

    The EWS job tests run against a live inbox, so they must see exactly which email
    the job touched and put its read and flag state back afterwards. The job itself
    cannot undo its marking (it has no "mark unread"), so the tests talk to Exchange
    Web Services directly, signed in as the same Azure app the plugin uses. Only the
    few calls the tests need are here, as raw SOAP over Node's fetch: Playwright's
    request log would print the bearer token and client secret on an error.

    Reason: A test that marks a live email must restore it exactly.
*/

export type ExchangeCredentials = {
    applicationId: string;
    tenantId: string;
    secret: string;
};

export type FlagStatus = "NotFlagged" | "Flagged" | "Complete";

/** The state of one inbox item. Subject and sender are kept in memory for comparison only; never print them. */
export type InboxItem = {
    id: string;
    /** Element name, e.g. Message or MeetingRequest. */
    itemType: string;
    received: string;
    isRead: boolean | null;
    flagStatus: FlagStatus | null;
    subject: string;
    fromEmail: string;
};

const ServerUrl = "https://outlook.office365.com/EWS/Exchange.asmx";

export class ExchangeClient {
    private token: { value: string; expiresAt: number } | null = null;

    public constructor( private readonly credentials: ExchangeCredentials, private readonly mailbox: string, private readonly impersonate: string ) {
    }

    /** The newest items in the mailbox's Inbox, newest first: the same query the job runs. */
    public async getNewestInboxItems( count: number ): Promise<InboxItem[]> {
        const body = `
    <m:FindItem Traversal="Shallow">
      <m:ItemShape>
        <t:BaseShape>IdOnly</t:BaseShape>
        <t:AdditionalProperties>
          <t:FieldURI FieldURI="item:ItemClass" />
          <t:FieldURI FieldURI="item:Subject" />
          <t:FieldURI FieldURI="item:DateTimeReceived" />
          <t:FieldURI FieldURI="item:Flag" />
          <t:FieldURI FieldURI="message:IsRead" />
          <t:FieldURI FieldURI="message:From" />
        </t:AdditionalProperties>
      </m:ItemShape>
      <m:IndexedPageItemView MaxEntriesReturned="${count}" Offset="0" BasePoint="Beginning" />
      <m:SortOrder>
        <t:FieldOrder Order="Descending"><t:FieldURI FieldURI="item:DateTimeReceived" /></t:FieldOrder>
      </m:SortOrder>
      <m:ParentFolderIds>
        <t:DistinguishedFolderId Id="inbox"><t:Mailbox><t:EmailAddress>${xmlEscape( this.mailbox )}</t:EmailAddress></t:Mailbox></t:DistinguishedFolderId>
      </m:ParentFolderIds>
    </m:FindItem>`;

        return parseItems( await this.call( "FindItem", body ) );
    }

    /** Re-reads one item's state. */
    public async getItem( id: string ): Promise<InboxItem> {
        const body = `
    <m:GetItem>
      <m:ItemShape>
        <t:BaseShape>IdOnly</t:BaseShape>
        <t:AdditionalProperties>
          <t:FieldURI FieldURI="item:Subject" />
          <t:FieldURI FieldURI="item:DateTimeReceived" />
          <t:FieldURI FieldURI="item:Flag" />
          <t:FieldURI FieldURI="message:IsRead" />
          <t:FieldURI FieldURI="message:From" />
        </t:AdditionalProperties>
      </m:ItemShape>
      <m:ItemIds><t:ItemId Id="${xmlEscape( id )}" /></m:ItemIds>
    </m:GetItem>`;

        const [ item ] = parseItems( await this.call( "GetItem", body ) );
        if ( !item ) {
            throw new Error( "Exchange returned no item for the id." );
        }
        return item;
    }

    /** Sets an item's read and flag state, without sending a read receipt. */
    public async setState( item: { id: string; itemType: string }, state: { isRead: boolean | null; flagStatus: FlagStatus | null } ): Promise<void> {
        // Each change is wrapped in the item's own element type (Message, MeetingRequest, ...).
        const element = `t:${item.itemType}`;
        const updates: string[] = [];
        if ( state.isRead !== null ) {
            updates.push( `<t:SetItemField><t:FieldURI FieldURI="message:IsRead" /><${element}><t:IsRead>${state.isRead}</t:IsRead></${element}></t:SetItemField>` );
        }
        if ( state.flagStatus !== null ) {
            updates.push( `<t:SetItemField><t:FieldURI FieldURI="item:Flag" /><${element}><t:Flag><t:FlagStatus>${state.flagStatus}</t:FlagStatus></t:Flag></${element}></t:SetItemField>` );
        }
        if ( updates.length === 0 ) {
            return;
        }

        const body = `
    <m:UpdateItem MessageDisposition="SaveOnly" ConflictResolution="AlwaysOverwrite" SuppressReadReceipts="true">
      <m:ItemChanges>
        <t:ItemChange>
          <t:ItemId Id="${xmlEscape( item.id )}" />
          <t:Updates>${updates.join( "" )}</t:Updates>
        </t:ItemChange>
      </m:ItemChanges>
    </m:UpdateItem>`;

        await this.call( "UpdateItem", body );
    }

    private async call( operation: string, body: string ): Promise<string> {
        const envelope = `<?xml version="1.0" encoding="utf-8"?>
<soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/"
  xmlns:t="http://schemas.microsoft.com/exchange/services/2006/types"
  xmlns:m="http://schemas.microsoft.com/exchange/services/2006/messages">
  <soap:Header>
    <t:RequestServerVersion Version="Exchange2013_SP1" />
    <t:ExchangeImpersonation><t:ConnectingSID><t:SmtpAddress>${xmlEscape( this.impersonate || this.mailbox )}</t:SmtpAddress></t:ConnectingSID></t:ExchangeImpersonation>
  </soap:Header>
  <soap:Body>${body}
  </soap:Body>
</soap:Envelope>`;

        const response = await this.guard( async () => await fetch( ServerUrl, {
            method: "POST",
            headers: {
                "Authorization": `Bearer ${await this.getToken()}`,
                "Content-Type": "text/xml; charset=utf-8",
                "X-AnchorMailbox": this.impersonate || this.mailbox
            },
            body: envelope
        } ) );
        const text = await response.text();

        const responseCode = text.match( /<m:ResponseCode>([^<]*)<\/m:ResponseCode>/ )?.[ 1 ];
        if ( !response.ok || ( responseCode && responseCode !== "NoError" ) ) {
            const message = text.match( /<m:MessageText>([^<]*)<\/m:MessageText>/ )?.[ 1 ] ?? text.match( /<faultstring[^>]*>([^<]*)</ )?.[ 1 ] ?? `HTTP ${response.status}`;
            throw this.redact( new Error( `Exchange ${operation} failed: ${responseCode ?? response.status} ${xmlUnescape( message )}` ) );
        }

        return text;
    }

    private async getToken(): Promise<string> {
        if ( this.token && Date.now() < this.token.expiresAt ) {
            return this.token.value;
        }

        const response = await this.guard( async () => await fetch( `https://login.microsoftonline.com/${encodeURIComponent( this.credentials.tenantId )}/oauth2/v2.0/token`, {
            method: "POST",
            headers: { "Content-Type": "application/x-www-form-urlencoded" },
            body: new URLSearchParams( {
                grant_type: "client_credentials",
                client_id: this.credentials.applicationId,
                client_secret: this.credentials.secret,
                scope: "https://outlook.office365.com/.default"
            } ).toString()
        } ) );
        const json = await response.json() as { access_token?: string; expires_in?: number; error_description?: string };
        if ( !response.ok || !json.access_token ) {
            throw this.redact( new Error( `Microsoft sign-in for the test's Exchange checks failed (M365_APP_ID / M365_TENANT_ID / M365_APP_SECRET in .env): ${json.error_description ?? response.status}` ) );
        }

        this.token = { value: json.access_token, expiresAt: Date.now() + ( ( json.expires_in ?? 3600 ) - 120 ) * 1000 };
        return this.token.value;
    }

    private async guard<T>( action: () => Promise<T> ): Promise<T> {
        try {
            return await action();
        }
        catch ( error ) {
            throw this.redact( error );
        }
    }

    /** Masks the client secret and access token in an error before it can reach a report. */
    private redact( error: unknown ): Error {
        const secrets = [ this.credentials.secret, this.token?.value ].filter( ( value ): value is string => !!value );
        const mask = ( text: string | undefined ): string => secrets.reduce( ( result, secret ) => result.split( secret ).join( "[redacted]" ), text ?? "" );
        const result = error instanceof Error ? error : new Error( String( error ) );
        result.message = mask( result.message );
        result.stack = mask( result.stack );
        return result;
    }
}

function parseItems( xml: string ): InboxItem[] {
    const items: InboxItem[] = [];
    for ( const match of xml.matchAll( /<t:(\w+)>\s*<t:ItemId Id="([^"]+)"[^>]*\/>([\s\S]*?)<\/t:\1>/g ) ) {
        const [ , itemType, id, content ] = match;
        const field = ( pattern: RegExp ): string | null => {
            const value = content.match( pattern )?.[ 1 ];
            return value === undefined ? null : xmlUnescape( value );
        };
        const isRead = field( /<t:IsRead>([^<]*)<\/t:IsRead>/ );

        items.push( {
            id: xmlUnescape( id ),
            itemType,
            received: field( /<t:DateTimeReceived>([^<]*)<\/t:DateTimeReceived>/ ) ?? "",
            isRead: isRead === null ? null : isRead === "true",
            flagStatus: field( /<t:FlagStatus>([^<]*)<\/t:FlagStatus>/ ) as FlagStatus | null,
            subject: field( /<t:Subject>([^<]*)<\/t:Subject>/ ) ?? "",
            fromEmail: field( /<t:From>[\s\S]*?<t:EmailAddress>([^<]*)<\/t:EmailAddress>/ ) ?? ""
        } );
    }

    return items;
}

function xmlEscape( value: string ): string {
    return value.replace( /&/g, "&amp;" ).replace( /</g, "&lt;" ).replace( />/g, "&gt;" ).replace( /"/g, "&quot;" );
}

function xmlUnescape( value: string ): string {
    return value
        .replace( /&lt;/g, "<" ).replace( /&gt;/g, ">" ).replace( /&quot;/g, "\"" ).replace( /&apos;/g, "'" )
        .replace( /&#(\d+);/g, ( _match, code: string ) => String.fromCharCode( parseInt( code, 10 ) ) )
        .replace( /&amp;/g, "&" );
}
