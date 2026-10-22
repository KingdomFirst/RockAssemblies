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
import * as os from "os";
import * as path from "path";

/*
    10/22/2026 - CLAUDE

    A KFS plugin is a RockShop package in the shared Rock Packages folder, and the
    compatibility preflight (rocks.kfs.Tests.Compile) and /kfs-package name plugins by
    that package's folder. Reports from this suite use the same names, mapped here from
    each plugin's test folder, so the two test reports line up plugin for plugin.
    Folders are organized by Rock domain, like Rock's own test projects, so a folder name
    is not always the package name.

    Reason: One name per plugin across packaging, the preflight and functional tests.
*/

/** Test folder (relative to tests/) -> Rock Packages folder name, without "(Unreleased)". */
export const PluginPackages: Record<string, string> = {
    "CheckIn/AttendedCheckin": "Attended Checkin",
    "CheckIn/CheckinManager": "Advanced Check-in Monitor",
    "Communication/CustomGroupReminder": "Custom Group Reminder Emails",
    "Communication/Microsoft365Utilities": "Microsoft 365 Utilities",
    "Communication/ScheduledGroupCommunication": "Scheduled Group Communication",
    "Crm/PersonAttributeForms": "Person Attributes Form Advanced",
    "Crm/PersonContactNotes": "Person Contact Notes",
    "Crm/StepsToCare": "Steps to Care",
    "Event/EventbriteSync": "Eventbrite",
    "Fundraising/FundraisingParticipantSummary": "Fundraising Participant Summary",
    "Fundraising/FundraisingProgress": "KFS Fundraising Progress",
    "Groups/GroupFinder": "Advanced Group Finder",
    "Groups/RoundRobinGroupAssignment": "Round Robin Group Assignment"
};

/** The plugin (package name) a test folder belongs to, or null if the folder is not mapped. */
export function getPluginForFolder( folder: string ): string | null {
    return PluginPackages[ folder.split( path.sep ).join( "/" ) ] ?? null;
}

/** The shared Rock Packages folder (ROCK_PACKAGES_ROOT, or the usual OneDrive path). */
export function getRockPackagesRoot(): string {
    return process.env.ROCK_PACKAGES_ROOT
        || path.join( os.homedir(), "OneDrive - Kingdom First Solutions Inc", "Shared Documents - Kingdom First Solutions Inc. Team Site", "Rock Packages" );
}

/**
 * Plugin names used by this suite that are not a current KFS plugin: no folder of that name
 * in Rock Packages, or a folder listed as not ours in the kfs-package skill's
 * excluded-plugins.txt. Returns null when Rock Packages is not available to check against.
 */
export function findUnknownPluginNames( names: string[] ): string[] | null {
    const root = getRockPackagesRoot();
    if ( !fs.existsSync( root ) ) {
        return null;
    }

    const folders = fs.readdirSync( root, { withFileTypes: true } )
        .filter( entry => entry.isDirectory() && !entry.name.startsWith( "_" ) )
        .map( entry => entry.name );

    const excludedList = path.join( __dirname, "..", "..", ".guardrails", "skills", "kfs-package", "excluded-plugins.txt" );
    const excluded = fs.existsSync( excludedList )
        ? fs.readFileSync( excludedList, "utf8" ).split( /\r?\n/ )
            .filter( line => line.trim() && !line.trim().startsWith( "#" ) )
            .map( line => line.split( "|" )[ 0 ].trim() )
        : [];

    const plugins = new Set( folders
        .filter( name => !excluded.includes( name ) )
        .map( name => name.replace( /\s*\(Unreleased\)\s*$/, "" ) ) );

    return [ ...new Set( names ) ].filter( name => !plugins.has( name ) );
}
