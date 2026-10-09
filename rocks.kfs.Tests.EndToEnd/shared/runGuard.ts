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
import * as fs from "fs";
import * as path from "path";

/*
    10/5/2026 - CLAUDE

    Playwright starts a new worker after every failed test, and a new worker
    builds its worker-scoped fixture again. When a fixture's setup itself fails
    (a slow site, a missing one-time setup step), every test in the suite
    rebuilt and tore down the test data against the site, and failed cleanups
    left data behind. The first setup failure is now recorded for the rest of
    the run, and later attempts fail immediately with the same message.

    Reason: One broken setup must not hit the site once per test.
*/

const markerDirectory = path.join( __dirname, "..", "test-results", ".setup-failures" );

/** Removes failure markers from an earlier run. Called from globalSetup. */
export function clearSetupFailures(): void {
    fs.rmSync( markerDirectory, { recursive: true, force: true } );
}

/**
 * Runs a worker fixture's body. If an earlier attempt in this run already failed during
 * setup, fails at once with that error instead of touching the site again.
 */
export async function guardFixtureSetup( suiteName: string, body: ( markSetupDone: () => void ) => Promise<void> ): Promise<void> {
    const marker = path.join( markerDirectory, `${suiteName.replace( /[^A-Za-z0-9]+/g, "-" )}.txt` );
    if ( fs.existsSync( marker ) ) {
        throw new Error( `${suiteName} setup already failed earlier in this run; not retrying. ${fs.readFileSync( marker, "utf8" )}` );
    }

    let setupDone = false;
    try {
        await body( () => {
            setupDone = true;
        } );
    }
    catch ( error ) {
        if ( !setupDone ) {
            fs.mkdirSync( markerDirectory, { recursive: true } );
            fs.writeFileSync( marker, ( error as Error ).message ?? String( error ) );
        }
        throw error;
    }
}
