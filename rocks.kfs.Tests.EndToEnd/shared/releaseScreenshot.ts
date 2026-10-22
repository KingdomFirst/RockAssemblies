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
import { Locator } from "@playwright/test";
import * as fs from "fs";
import * as path from "path";

/*
    10/9/2026 - CLAUDE

    RockShop screenshots for a plugin update are taken from the same tests that prove the
    change works, so they show the real block on a real Rock version and can be retaken on
    every release. Nothing is written unless RELEASE_SCREENSHOTS_DIR is set, so ordinary
    test runs are unaffected. The /kfs-package skill sets it and copies the results into
    the plugin's Screenshots folder.

    Reason: RockShop submissions need a screenshot of each UI change.
*/

/**
 * Saves a RockShop screenshot of the given element when RELEASE_SCREENSHOTS_DIR is set.
 * Call it from the test that covers a UI change, after the assertions pass, so a failing
 * test never produces a screenshot.
 *
 * @param target The part of the page that changed. Usually the block's root.
 * @param name File name without extension, as PluginName_WhatItShows (e.g.
 * "FundraisingProgress_GroupMemberView"). The package's version is added when it is
 * copied into the Screenshots folder.
 */
export async function releaseScreenshot( target: Locator, name: string ): Promise<void> {
    const directory = process.env.RELEASE_SCREENSHOTS_DIR;
    if ( !directory ) {
        return;
    }

    fs.mkdirSync( directory, { recursive: true } );
    await target.scrollIntoViewIfNeeded();
    await target.screenshot( { path: path.join( directory, `${name}.png` ), animations: "disabled" } );
}
