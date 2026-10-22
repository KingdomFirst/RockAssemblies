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
import * as path from "path";

/*
    10/22/2026 - CLAUDE

    Every run writes to its own folder, test-results/runs/<run id>, so a run's report,
    screenshots and traces are never overwritten by the next run. Playwright loads the
    config in the main process and again in every worker; the run id is fixed in an
    environment variable the first time, and the workers inherit it, so all of them
    agree on one folder.

    Reason: Reports must survive later runs to serve as a record (e.g. a RockShop submission).
*/

const runIdVariable = "KFS_E2E_RUN_ID";

/** Folder that holds every run's folder. */
export const runsRoot = path.join( __dirname, "..", "test-results", "runs" );

/** Run folder names look like 2026-10-22_143005. Only these are ever pruned. */
export const runIdPattern = /^\d{4}-\d{2}-\d{2}_\d{6}$/;

/** This run's id, created on first use (local time, sortable). */
export function getRunId(): string {
    if ( !process.env[ runIdVariable ] ) {
        const now = new Date();
        const pad = ( value: number ): string => String( value ).padStart( 2, "0" );
        process.env[ runIdVariable ] = `${now.getFullYear()}-${pad( now.getMonth() + 1 )}-${pad( now.getDate() )}_${pad( now.getHours() )}${pad( now.getMinutes() )}${pad( now.getSeconds() )}`;
    }

    return process.env[ runIdVariable ] as string;
}

/** This run's folder: summary, JSON results, HTML report and failure artifacts. */
export function getRunDirectory(): string {
    return path.join( runsRoot, getRunId() );
}
