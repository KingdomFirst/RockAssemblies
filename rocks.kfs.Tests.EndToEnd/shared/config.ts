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
import * as dotenv from "dotenv";
import * as path from "path";

dotenv.config( { path: path.join( __dirname, "..", ".env" ) } );

/** Settings for the site under test, read from .env (see .env.example). */
export type TestSettings = {
    baseUrl: string;
    apiKey: string;
    parentPageId: number;
    zone: string;
};

let settings: TestSettings | null = null;

/**
 * Gets the test settings, failing with a message that says exactly what is missing.
 * The API key itself is never logged.
 */
export function getTestSettings(): TestSettings {
    if ( settings ) {
        return settings;
    }

    const missing: string[] = [];
    const baseUrl = ( process.env.ROCK_BASE_URL ?? "" ).replace( /\/+$/, "" );
    const apiKey = process.env.ROCK_API_KEY ?? "";
    const parentPageId = parseInt( process.env.ROCK_TEST_PARENT_PAGE_ID ?? "", 10 );

    if ( !baseUrl ) {
        missing.push( "ROCK_BASE_URL" );
    }
    if ( !apiKey ) {
        missing.push( "ROCK_API_KEY" );
    }
    if ( !Number.isInteger( parentPageId ) || parentPageId <= 0 ) {
        missing.push( "ROCK_TEST_PARENT_PAGE_ID" );
    }

    if ( missing.length > 0 ) {
        throw new Error( `Test settings missing from rocks.kfs.Tests.EndToEnd/.env: ${missing.join( ", " )}. Copy .env.example to .env and fill it in.` );
    }

    settings = {
        baseUrl,
        apiKey,
        parentPageId,
        zone: process.env.ROCK_TEST_ZONE || "Main"
    };

    return settings;
}

/**
 * Reads a plugin-specific setting from .env. Returns null when it is not set, so a suite
 * can stop with a message naming the setting it needs.
 */
export function getOptionalSetting( name: string ): string | null {
    getTestSettings();
    const value = ( process.env[ name ] ?? "" ).trim();
    return value === "" ? null : value;
}

/*
    10/5/2026 - CLAUDE

    Playwright's request errors (timeouts, connection failures) include a call
    log that lists every request header, so the REST key appeared in plain text
    in the console and the test reports. Every API error is passed through here
    before it is rethrown, so the key never leaves this process.

    Reason: The REST key must never be printed or written to reports.
*/

/** Rethrows an error with the REST key masked in its message and stack. */
export function redactSecrets( error: unknown ): Error {
    const apiKey = getTestSettings().apiKey;
    const mask = ( text: string | undefined ): string => ( text ?? "" ).split( apiKey ).join( "[REST key redacted]" );

    if ( error instanceof Error ) {
        error.message = mask( error.message );
        error.stack = mask( error.stack );
        return error;
    }

    return new Error( mask( String( error ) ) );
}
