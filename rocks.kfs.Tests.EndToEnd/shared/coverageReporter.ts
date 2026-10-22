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
import type { FullConfig, Reporter, Suite } from "@playwright/test/reporter";
import * as fs from "fs";
import * as path from "path";
import { CoverageNotes } from "./coverageNotes";
import { findUnknownPluginNames, getPluginForFolder } from "./plugins";

/*
    10/22/2026 - CLAUDE

    Writes TEST-COVERAGE.md, a plain-language list of what the functional tests cover for
    each KFS plugin. Run it with `npm run coverage`, which lists the suite without running
    it (playwright test --list) and hands the test tree to this reporter. The tests are
    read from the suite itself, so the document cannot drift from them; what the code
    cannot say (what a plugin does, what is not covered) comes from coverageNotes.ts.
    Compile coverage comes from the newest compatibility preflight report, if there is one.

    The output has no dates or run-specific values, so it only changes when coverage does.

    Reason: Stakeholders need to know what is tested without reading test code.
*/

type ListedTest = { area: string; title: string; file: string; line: number };

type PreflightPlugin = { plugin: string; dlls?: unknown[]; pages?: unknown[] };
type PreflightReport = { rockVersion?: string; plugins?: PreflightPlugin[] | PreflightPlugin; notTested?: Array<{ plugin: string; status: string }> | { plugin: string; status: string } };

const outputFile = path.join( __dirname, "..", "TEST-COVERAGE.md" );
const preflightResults = path.join( __dirname, "..", "..", "rocks.kfs.Tests.Compile", "results" );

export default class CoverageReporter implements Reporter {
    private rootSuite: Suite | null = null;
    private testDir = "";

    public printsToStdio(): boolean {
        return true;
    }

    public onBegin( config: FullConfig, suite: Suite ): void {
        this.rootSuite = suite;
        this.testDir = config.projects[ 0 ]?.testDir ?? config.rootDir;
    }

    public onEnd(): void {
        const byPlugin = new Map<string, ListedTest[]>();
        const warnings: string[] = [];

        for ( const test of this.rootSuite?.allTests() ?? [] ) {
            const relativeFile = path.relative( this.testDir, test.location.file );
            const folder = path.dirname( relativeFile ).split( path.sep ).join( "/" );
            const plugin = getPluginForFolder( folder );
            if ( !plugin ) {
                warnings.push( `Tests in ${folder} have no plugin in shared/plugins.ts; they are left out.` );
                continue;
            }

            const describes = test.titlePath().slice( 3, -1 ).filter( title => title );
            if ( describes.length > 0 && describes[ 0 ].includes( ":" ) ) {
                describes[ 0 ] = describes[ 0 ].substring( describes[ 0 ].indexOf( ":" ) + 1 ).trim();
            }

            const tests = byPlugin.get( plugin ) ?? [];
            tests.push( { area: describes.filter( d => d ).join( " › " ) || "general", title: test.title, file: relativeFile.split( path.sep ).join( "/" ), line: test.location.line } );
            byPlugin.set( plugin, tests );
        }

        const preflight = readNewestPreflight();
        const compiled = new Map<string, string>();
        for ( const p of asArray( preflight?.plugins ) ) {
            const parts = [ countText( asArray( p.dlls ).length, "DLL" ), countText( asArray( p.pages ).length, "page" ) ].filter( part => part );
            compiled.set( p.plugin, `Yes (${parts.join( ", " )})` );
        }
        for ( const p of asArray( preflight?.notTested ) ) {
            compiled.set( p.plugin, `No (${p.status === "Obsidian only" ? "Obsidian files only" : p.status.toLowerCase()})` );
        }
        if ( !preflight ) {
            warnings.push( "No compatibility preflight report found (rocks.kfs.Tests.Compile/results); the compile column is empty. Run the preflight, then regenerate." );
        }

        const plugins = [ ...new Set( [ ...byPlugin.keys(), ...Object.keys( CoverageNotes ), ...compiled.keys() ] ) ].sort( ( a, b ) => a.localeCompare( b ) );
        for ( const plugin of plugins.filter( p => !CoverageNotes[ p ] ) ) {
            warnings.push( `${plugin} has no entry in shared/coverageNotes.ts.` );
        }
        const unknown = findUnknownPluginNames( plugins );
        if ( unknown && unknown.length > 0 ) {
            warnings.push( `Not a current KFS plugin in Rock Packages: ${unknown.join( ", " )}. Update shared/plugins.ts or shared/coverageNotes.ts.` );
        }

        const tested = plugins.filter( p => byPlugin.has( p ) );
        const untested = plugins.filter( p => !byPlugin.has( p ) );
        const total = [ ...byPlugin.values() ].reduce( ( sum, tests ) => sum + tests.length, 0 );

        const lines: string[] = [];
        lines.push( "# KFS plugin test coverage", "" );
        lines.push( "<!-- Generated by `npm run coverage` from the test suite and shared/coverageNotes.ts. Do not edit by hand. -->", "" );
        lines.push( "What the automated tests check for each KFS RockShop plugin, in plain language. Two suites cover the plugins:", "" );
        lines.push( "- **Functional tests** (`rocks.kfs.Tests.EndToEnd`) use each plugin on a running Rock test site, the way a person would, and check the result. They are listed below, plugin by plugin." );
        lines.push( "- **The compatibility preflight** (`rocks.kfs.Tests.Compile`) checks that each plugin's code still builds and compiles against a new Rock version. It does not run the plugin." );
        lines.push( "" );
        lines.push( `${total} functional tests cover ${tested.length} of ${plugins.length} plugins.${preflight?.rockVersion ? ` Compile coverage is from the newest preflight report (${cleanRockVersion( preflight.rockVersion )}).` : ""}`, "" );
        lines.push( "Plugins are named as in the shared Rock Packages folder. Setup steps for each suite are in its README." );

        lines.push( "", "## Summary", "", "| Plugin | Functional tests | Areas tested | Compile check | Known issues |", "|---|---|---|---|---|" );
        for ( const plugin of plugins ) {
            const tests = byPlugin.get( plugin ) ?? [];
            const areas = [ ...new Set( tests.map( t => t.area ) ) ];
            const issues = CoverageNotes[ plugin ]?.knownIssues?.length ?? 0;
            lines.push( `| [${plugin}](#${anchor( plugin )}) | ${tests.length || "none"} | ${areas.map( capitalize ).join( ", " ) || "-"} | ${compiled.get( plugin ) ?? "-"} | ${issues || "-"} |` );
        }

        for ( const plugin of tested ) {
            lines.push( ...renderPlugin( plugin, byPlugin.get( plugin ) ?? [], compiled.get( plugin ) ) );
        }

        if ( untested.length > 0 ) {
            lines.push( "", "## Plugins without functional tests" );
            for ( const plugin of untested ) {
                lines.push( ...renderPlugin( plugin, [], compiled.get( plugin ), 3 ) );
            }
        }

        if ( warnings.length > 0 ) {
            lines.push( "", "## Generator warnings", "" );
            warnings.forEach( w => lines.push( `- ${w}` ) );
        }

        const content = lines.join( "\n" ) + "\n";
        const changed = !fs.existsSync( outputFile ) || fs.readFileSync( outputFile, "utf8" ).replace( /\r\n/g, "\n" ) !== content;
        fs.writeFileSync( outputFile, content );

        console.log( `${changed ? "Wrote" : "Unchanged:"} ${outputFile}` );
        console.log( `${total} tests, ${tested.length} plugins with functional tests, ${untested.length} without.` );
        warnings.forEach( w => console.log( `Warning: ${w}` ) );
    }
}

function renderPlugin( plugin: string, tests: ListedTest[], compile: string | undefined, level = 2 ): string[] {
    const notes = CoverageNotes[ plugin ];
    const heading = "#".repeat( level );
    const sub = "#".repeat( level + 1 );
    const lines: string[] = [ "", `${heading} ${plugin}`, "" ];

    if ( notes?.about ) {
        lines.push( notes.about, "" );
    }
    lines.push( `Functional tests: **${tests.length || "none"}**. Compile check: ${compile ?? "not in the preflight report"}.` );

    if ( tests.length > 0 ) {
        lines.push( "", `${sub} What is tested` );
        const areas = [ ...new Set( tests.map( t => t.area ) ) ];
        for ( const area of areas ) {
            lines.push( "", `**${capitalize( area )}**`, "" );
            for ( const test of tests.filter( t => t.area === area ) ) {
                lines.push( `- ${describeTest( test.title )}` );
            }
        }
    }

    if ( notes?.notCovered?.length && tests.length > 0 ) {
        lines.push( "", `${sub} Not tested`, "" );
        notes.notCovered.forEach( n => lines.push( `- ${n}` ) );
    }
    else if ( notes?.notCovered?.length ) {
        lines.push( "" );
        notes.notCovered.forEach( n => lines.push( n ) );
    }

    if ( notes?.knownIssues?.length ) {
        lines.push( "", `${sub} Known issues`, "", "Tests that fail because of these plugin bugs are reported as *known issue* rather than as new failures.", "" );
        notes.knownIssues.forEach( n => lines.push( `- ${n}` ) );
    }

    if ( notes?.manual?.length ) {
        lines.push( "", `${sub} Manual checks and one-time setup`, "" );
        notes.manual.forEach( n => lines.push( `- ${n}` ) );
    }

    return lines;
}

// #region Plain-language test names

/*
    Test names follow Rock's convention, Action_Scenario_ExpectedResult, in PascalCase
    (e.g. Submit_ConnectionStatusLeftBlank_KeepsExistingStatus). They become
    "Keeps existing status (submit, connection status left blank)."
*/

/** Words kept in their own capitalization when a name is turned into a sentence. */
const keepCase: Record<string, string> = {
    id: "ID", guid: "GUID", sms: "SMS", ok: "OK", checkin: "check-in", pdf: "PDF", url: "URL", ews: "EWS", api: "API", zpl: "ZPL",
    rock: "Rock", eventbrite: "Eventbrite", excel: "Excel", lava: "Lava", microsoft: "Microsoft"
};

function humanize( token: string ): string {
    return token
        .replace( /([a-z0-9])([A-Z])/g, "$1 $2" )
        .replace( /([A-Z]+)([A-Z][a-z])/g, "$1 $2" )
        .split( " " )
        .map( word => keepCase[ word.toLowerCase() ] ?? word.toLowerCase() )
        .join( " " );
}

export function describeTest( title: string ): string {
    const parts = title.split( "_" ).filter( p => p );
    if ( parts.length < 2 ) {
        return `${capitalize( humanize( title ) )}.`;
    }

    const expected = humanize( parts[ parts.length - 1 ] );
    const context = parts.slice( 0, -1 ).map( humanize );

    // "GlobalAttributes_EwsCredentials_AreSetAndEncrypted": the result needs its subject first.
    if ( /^(is|are) /.test( expected ) && context.length > 0 ) {
        const subject = context[ context.length - 1 ];
        const rest = context.slice( 0, -1 );
        return `${capitalize( subject )} ${expected}${rest.length ? ` (${rest.join( ", " )})` : ""}.`;
    }

    return `${capitalize( expected )} (${context.join( ", " )}).`;
}

// #endregion

// #region Helpers

function readNewestPreflight(): PreflightReport | null {
    if ( !fs.existsSync( preflightResults ) ) {
        return null;
    }

    const reports = fs.readdirSync( preflightResults )
        .map( version => path.join( preflightResults, version, "preflight.json" ) )
        .filter( file => fs.existsSync( file ) )
        .sort( ( a, b ) => fs.statSync( b ).mtimeMs - fs.statSync( a ).mtimeMs );
    if ( reports.length === 0 ) {
        return null;
    }

    try {
        return JSON.parse( fs.readFileSync( reports[ 0 ], "utf8" ).replace( /^﻿/, "" ) ) as PreflightReport;
    }
    catch {
        return null;
    }
}

/** PowerShell's ConvertTo-Json writes a one-item array as a bare object. */
function asArray<T>( value: T[] | T | undefined | null ): T[] {
    if ( value === undefined || value === null ) {
        return [];
    }
    return Array.isArray( value ) ? value : [ value ];
}

function cleanRockVersion( value: string ): string {
    return value.replace( /\+.*$/, "" ).replace( /^Rock\s+\w+\s+/, "Rock " );
}

/** "1 page", "3 pages", or "" for none. */
function countText( count: number, noun: string ): string {
    return count === 0 ? "" : `${count} ${noun}${count === 1 ? "" : "s"}`;
}

function capitalize( value: string ): string {
    return value.charAt( 0 ).toUpperCase() + value.slice( 1 );
}

function anchor( value: string ): string {
    return value.toLowerCase().replace( /[^a-z0-9 -]/g, "" ).replace( / /g, "-" );
}

// #endregion
