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
import { defineConfig } from "@playwright/test";
import * as dotenv from "dotenv";
import * as path from "path";

dotenv.config( { path: path.join( __dirname, ".env" ) } );

const channel = process.env.BROWSER_CHANNEL || "msedge";

export default defineConfig( {
    testDir: "./tests",
    globalSetup: "./shared/globalSetup.ts",
    outputDir: "./test-results/artifacts",

    /*
        9/23/2026 - CLAUDE

        Every test in a plugin suite reconfigures the same block instance on the same
        site, so tests must not run concurrently. One worker also means the worker-scoped
        fixture (test page, person, groups, workflows) is built exactly once per run.

        Reason: Tests share one block instance on a live site.
    */
    workers: 1,
    fullyParallel: false,
    retries: 0,

    timeout: 90_000,
    expect: { timeout: 15_000 },

    reporter: [
        [ "list" ],
        [ "json", { outputFile: "test-results/results.json" } ],
        [ "html", { outputFolder: "test-results/html", open: "never" } ]
    ],

    use: {
        baseURL: process.env.ROCK_BASE_URL,
        channel: channel === "chromium" ? undefined : channel,
        headless: true,
        viewport: { width: 1400, height: 1000 },
        actionTimeout: 20_000,
        navigationTimeout: 45_000,
        screenshot: "only-on-failure",
        trace: "retain-on-failure"
    }
} );
