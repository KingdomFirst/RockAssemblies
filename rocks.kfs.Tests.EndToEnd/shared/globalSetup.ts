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
import { request } from "@playwright/test";
import { getTestSettings, redactSecrets } from "./config";
import { clearSetupFailures } from "./runGuard";

/*
    9/23/2026 - CLAUDE

    Checks the site and REST key once before any test runs. Without this, a bad key
    fails every plugin fixture separately, burying one configuration problem under
    dozens of identical failures.

    Reason: Fail fast with one clear message on environment problems.
*/
export default async function globalSetup(): Promise<void> {
    clearSetupFailures();
    const settings = getTestSettings();
    const http = await request.newContext( {
        baseURL: settings.baseUrl,
        extraHTTPHeaders: { "Authorization-Token": settings.apiKey, "Accept": "application/json" }
    } );

    try {
        // A Rock site that was just updated or restarted compiles on its first requests, which can take minutes.
        const response = await http.get( "/api/People/GetCurrentPerson", { timeout: 180_000 } ).catch( error => {
            const reason = /Timeout/i.test( String( ( error as Error ).message ) ) ? "did not answer within 3 minutes" : "could not be reached";
            throw redactSecrets( new Error( `The Rock site at ${settings.baseUrl} ${reason}. If it was just updated or restarted, open it in a browser until it loads, then run the tests again. (${( error as Error ).message.split( "\n" )[ 0 ]})` ) );
        } );

        if ( response.status() === 401 ) {
            throw new Error( `The REST key in .env was rejected by ${settings.baseUrl} (401). Check the key on the site under Admin Tools > Security > REST Keys: it must match exactly, and the REST key must be Active.` );
        }
        if ( !response.ok() ) {
            throw new Error( `Could not reach the Rock REST API at ${settings.baseUrl} (HTTP ${response.status()}).` );
        }

        const restUser = await response.json() as { FullName?: string };
        console.log( `Connected to ${settings.baseUrl} as REST user '${restUser.FullName ?? "unknown"}'.` );
    }
    finally {
        await http.dispose();
    }
}
