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
import { APIRequestContext, APIResponse, request } from "@playwright/test";
import { getTestSettings, redactSecrets } from "./config";

/** Any Rock entity as returned by the v1 REST API. */
export type RockEntity = {
    Id: number;
    Guid: string;
    [ key: string ]: unknown;
};

/**
 * Normalizes an enum property from the REST API to its numeric value. Depending on the
 * serializer settings Rock returns either the number or the member name.
 * @param names Enum member names in numeric order (index = value).
 */
export function asEnumValue( value: unknown, names: readonly string[] ): number {
    if ( typeof value === "number" ) {
        return value;
    }

    const index = names.findIndex( name => name.toLowerCase() === String( value ).toLowerCase() );
    return index >= 0 ? index : parseInt( String( value ), 10 );
}

/**
 * A thin client over Rock's v1 REST API (the OData-style /api/{EntitySet} endpoints).
 * Used for arranging test data and asserting results, so tests do not depend on
 * pre-existing data on the site under test.
 */
export class RockApi {
    private constructor( private readonly http: APIRequestContext ) {
    }

    /** Creates a client authenticated with the REST key from .env. */
    public static async create(): Promise<RockApi> {
        const settings = getTestSettings();
        const http = await request.newContext( {
            baseURL: settings.baseUrl,
            // Rock can take well over 30s to delete a group or attribute on a busy site.
            timeout: 120_000,
            extraHTTPHeaders: {
                "Authorization-Token": settings.apiKey,
                "Accept": "application/json"
            }
        } );

        return new RockApi( http );
    }

    public async dispose(): Promise<void> {
        await this.http.dispose();
    }

    // #region Raw verbs

    public async get<T>( url: string ): Promise<T> {
        const response = await this.send( () => this.http.get( url ) );
        await this.ensureSuccess( "GET", url, response );

        return await response.json() as T;
    }

    /** POSTs a new entity and returns the new Id (Rock returns the Id as the body). */
    public async post( url: string, body: unknown ): Promise<number> {
        const response = await this.send( () => this.http.post( url, { data: body } ) );
        await this.ensureSuccess( "POST", url, response );

        const text = await response.text();
        return parseInt( text.replace( /"/g, "" ), 10 );
    }

    /** POSTs JSON to a non-entity endpoint (e.g. a v2 block action) and returns the parsed response, if any. */
    public async postJson<T = unknown>( url: string, body: unknown ): Promise<T | null> {
        const response = await this.send( () => this.http.post( url, { data: body } ) );
        await this.ensureSuccess( "POST", url, response );

        const text = await response.text();
        return text ? JSON.parse( text ) as T : null;
    }

    /**
     * Renders a Lava template on the site as the REST user (api/Lava/RenderTemplate, the
     * same engine the Lava Tester uses). Rock reads the body as raw text.
     */
    public async renderLava( template: string ): Promise<string> {
        const url = "/api/Lava/RenderTemplate";
        const response = await this.send( () => this.http.post( url, { data: template, headers: { "Content-Type": "text/plain" } } ) );
        await this.ensureSuccess( "POST", url, response );

        const text = await response.text();
        return text.startsWith( "\"" ) ? JSON.parse( text ) as string : text;
    }

    public async patch( entitySet: string, id: number, values: Record<string, unknown> ): Promise<void> {
        const url = `/api/${entitySet}/${id}`;
        const response = await this.send( () => this.http.patch( url, { data: values } ) );
        await this.ensureSuccess( "PATCH", url, response );
    }

    /** Deletes an entity. A 404 is not an error, so cleanup can run repeatedly. */
    public async delete( entitySet: string, id: number ): Promise<void> {
        const url = `/api/${entitySet}/${id}`;
        const response = await this.send( () => this.http.delete( url ) );
        if ( response.status() === 404 ) {
            return;
        }
        await this.ensureSuccess( "DELETE", url, response );
    }

    // #endregion

    // #region Query helpers

    /** Runs an OData $filter against an entity set. */
    public async query<T = RockEntity>( entitySet: string, filter: string, extra = "" ): Promise<T[]> {
        const url = `/api/${entitySet}?$filter=${encodeURIComponent( filter )}${extra}`;
        return await this.get<T[]>( url );
    }

    public async first<T = RockEntity>( entitySet: string, filter: string, extra = "" ): Promise<T | null> {
        const results = await this.query<T>( entitySet, filter, `&$top=1${extra}` );
        return results.length > 0 ? results[0] : null;
    }

    public async single<T = RockEntity>( entitySet: string, filter: string ): Promise<T> {
        const result = await this.first<T>( entitySet, filter );
        if ( !result ) {
            throw new Error( `No ${entitySet} found matching: ${filter}` );
        }

        return result;
    }

    public async getById<T = RockEntity>( entitySet: string, id: number ): Promise<T> {
        return await this.get<T>( `/api/${entitySet}/${id}` );
    }

    public async getByGuid<T = RockEntity>( entitySet: string, guid: string ): Promise<T | null> {
        return await this.first<T>( entitySet, `Guid eq guid'${guid}'` );
    }

    /** Deletes the entity with this Guid if it exists. Used to clear leftovers from an aborted run. */
    public async deleteByGuid( entitySet: string, guid: string ): Promise<void> {
        const existing = await this.getByGuid( entitySet, guid );
        if ( existing ) {
            await this.delete( entitySet, existing.Id );
        }
    }

    // #endregion

    // #region Lookups

    public async getEntityTypeId( name: string ): Promise<number> {
        return ( await this.single( "EntityTypes", `Name eq '${name}'` ) ).Id;
    }

    public async getFieldTypeId( fieldTypeClass: string ): Promise<number> {
        return ( await this.single( "FieldTypes", `Class eq '${fieldTypeClass}'` ) ).Id;
    }

    public async getIdByGuid( entitySet: string, guid: string ): Promise<number> {
        const entity = await this.getByGuid( entitySet, guid );
        if ( !entity ) {
            throw new Error( `No ${entitySet} with Guid ${guid}.` );
        }

        return entity.Id;
    }

    /** Gets the version string of the Rock instance under test, e.g. "20.0.7". */
    public async getRockVersion(): Promise<string> {
        const response = await this.send( () => this.http.get( "/api/Utility/GetRockSemanticVersionNumber" ) );
        return response.ok() ? ( await response.text() ).replace( /"/g, "" ) : "unknown";
    }

    // #endregion

    // #region Attribute values

    /**
     * Sets an attribute value for an entity (insert or update). Saving through the API
     * flushes the owning entity's cache item, so cached Blocks and workflow action types
     * see the new value on their next use.
     */
    public async setAttributeValue( attributeId: number, entityId: number, value: string ): Promise<void> {
        const existing = await this.first( "AttributeValues", `AttributeId eq ${attributeId} and EntityId eq ${entityId}` );
        if ( existing ) {
            await this.patch( "AttributeValues", existing.Id, { Value: value } );
        }
        else {
            await this.post( "/api/AttributeValues", {
                AttributeId: attributeId,
                EntityId: entityId,
                Value: value,
                IsSystem: false
            } );
        }
    }

    public async getAttributeValue( attributeId: number, entityId: number ): Promise<string | null> {
        const existing = await this.first( "AttributeValues", `AttributeId eq ${attributeId} and EntityId eq ${entityId}` );
        return existing ? existing.Value as string : null;
    }

    // #endregion

    /** Sends a request, masking the REST key in any error (Playwright's call log lists the request headers). */
    private async send( request: () => Promise<APIResponse> ): Promise<APIResponse> {
        try {
            return await request();
        }
        catch ( error ) {
            throw redactSecrets( error );
        }
    }

    private async ensureSuccess( verb: string, url: string, response: { ok(): boolean; status(): number; text(): Promise<string> } ): Promise<void> {
        if ( response.ok() ) {
            return;
        }

        const body = ( await response.text() ).substring( 0, 1000 );
        throw new Error( `Rock API ${verb} ${url} failed with ${response.status()}: ${body}` );
    }
}
