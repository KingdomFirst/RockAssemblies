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
import type { FullConfig, FullResult, Reporter, Suite, TestCase, TestResult } from "@playwright/test/reporter";
import { execSync } from "child_process";
import * as fs from "fs";
import * as path from "path";
import { redactSecrets } from "./config";
import { findUnknownPluginNames, getPluginForFolder } from "./plugins";
import { getRunDirectory, getRunId, runIdPattern, runsRoot } from "./runDirectory";

/*
    10/22/2026 - CLAUDE

    Playwright's HTML report is good for digging into one failure but poor as a record:
    it needs a local server to open, and it says nothing about the site or Rock version.
    This reporter writes a plain summary of every test (plugin, result, time, and the
    reason for any failure) next to the HTML report in the run's folder, as Markdown for
    pasting and HTML for reading, plus test-results/index.html listing every kept run.

    A test that fails because of a known plugin bug carries a "known-issue" annotation
    (see visibilityRules.spec.ts); it is reported apart from new failures so a run with
    only known issues reads as such.

    Reason: Each run needs a durable, readable record of what was tested and what failed.
*/

type Status = "passed" | "failed" | "known issue" | "flaky" | "skipped" | "timed out" | "interrupted";

type TestRow = {
    plugin: string;
    folder: string;
    describe: string;
    title: string;
    status: Status;
    durationMs: number;
    retries: number;
    error: string | null;
    knownIssue: string | null;
    attachments: Array<{ name: string; path: string }>;
    file: string;
    line: number;
};

export type RunSummary = {
    runId: string;
    site: string;
    rockVersions: string[];
    started: string;
    durationMs: number;
    overall: string;
    command: string;
    testCode: string;
    /** Problems with the report itself, e.g. a plugin name that is not a current KFS plugin. */
    warnings: string[];
    totals: Record<Status, number> & { total: number };
    plugins: Array<{ plugin: string; counts: Record<Status, number>; durationMs: number }>;
    tests: TestRow[];
};

const statuses: Status[] = [ "passed", "failed", "known issue", "timed out", "flaky", "skipped", "interrupted" ];

/** Runs to keep in test-results/runs; older run folders are deleted. Override with KFS_E2E_KEEP_RUNS. */
const defaultKeepRuns = 30;

export default class SummaryReporter implements Reporter {
    private rootSuite: Suite | null = null;
    private testDir = "";
    private started = new Date();

    public printsToStdio(): boolean {
        return false;
    }

    public onBegin( config: FullConfig, suite: Suite ): void {
        this.rootSuite = suite;
        this.testDir = config.projects[ 0 ]?.testDir ?? config.rootDir;
        this.started = new Date();
    }

    public async onEnd( result: FullResult ): Promise<void> {
        const tests = this.rootSuite?.allTests() ?? [];
        if ( tests.length === 0 ) {
            return;
        }

        const runDirectory = getRunDirectory();
        fs.mkdirSync( runDirectory, { recursive: true } );

        const rows = tests.map( test => this.toRow( test, runDirectory ) );
        const summary = this.summarize( rows, result );

        fs.writeFileSync( path.join( runDirectory, "summary.json" ), JSON.stringify( summary, null, 2 ) );
        fs.writeFileSync( path.join( runDirectory, "summary.md" ), renderMarkdown( summary ) );
        fs.writeFileSync( path.join( runDirectory, "summary.html" ), renderHtml( summary ) );

        pruneOldRuns();
        writeIndex();

        const t = summary.totals;
        console.log( "" );
        console.log( `Summary: ${t.passed} passed, ${t.failed + t[ "timed out" ]} failed, ${t[ "known issue" ]} known issue, ${t.flaky} flaky, ${t.skipped} skipped (of ${t.total}).` );
        console.log( `Report:  ${path.join( runDirectory, "summary.html" )}` );
        console.log( `All runs: ${path.join( runsRoot, "..", "index.html" )}` );
        for ( const warning of summary.warnings ) {
            console.log( `Warning: ${warning}` );
        }
    }

    private toRow( test: TestCase, runDirectory: string ): TestRow {
        const results = test.results;
        const last: TestResult | undefined = results[ results.length - 1 ];
        const describes = test.titlePath().slice( 3, -1 ).filter( title => title );
        const relativeFile = path.relative( this.testDir, test.location.file );
        const folder = path.dirname( relativeFile ).split( path.sep ).join( "/" );
        // The plugin is its Rock Packages name, from the test's folder (plugins.ts). Suites are
        // titled "<Plugin>: <area>"; the area alone is shown beside the test, since the report
        // already groups by plugin.
        const hasPluginPrefix = describes.length > 0 && describes[ 0 ].includes( ":" );
        const plugin = getPluginForFolder( folder ) ?? `${folder} (not in plugins.ts)`;
        if ( hasPluginPrefix ) {
            describes[ 0 ] = describes[ 0 ].substring( describes[ 0 ].indexOf( ":" ) + 1 ).trim();
        }
        const describe = describes.filter( title => title ).join( " › " );
        const knownIssue = test.annotations.concat( last?.annotations ?? [] ).find( a => a.type === "known-issue" )?.description ?? null;

        let status: Status;
        const outcome = test.outcome();
        if ( outcome === "skipped" || !last ) {
            status = "skipped";
        }
        else if ( outcome === "flaky" ) {
            status = "flaky";
        }
        else if ( outcome === "expected" ) {
            status = "passed";
        }
        else if ( last.status === "interrupted" ) {
            status = "interrupted";
        }
        else if ( knownIssue ) {
            status = "known issue";
        }
        else {
            status = last.status === "timedOut" ? "timed out" : "failed";
        }

        const rawError = status === "passed" || status === "skipped" ? null : ( last?.errors.map( e => e.message ?? e.value ?? "" ).join( "\n\n" ) || null );

        return {
            plugin,
            folder,
            describe,
            title: test.title,
            status,
            durationMs: results.reduce( ( sum, r ) => sum + r.duration, 0 ),
            retries: Math.max( 0, results.length - 1 ),
            error: rawError ? cleanError( rawError ) : null,
            knownIssue,
            // Playwright may still be writing the trace and error context, so do not require the files yet.
            attachments: ( last?.attachments ?? [] )
                .filter( a => a.path )
                .map( a => ( { name: a.name, path: path.relative( runDirectory, a.path as string ).split( path.sep ).join( "/" ) } ) ),
            file: relativeFile.split( path.sep ).join( "/" ),
            line: test.location.line
        };
    }

    private summarize( rows: TestRow[], result: FullResult ): RunSummary {
        const emptyCounts = (): Record<Status, number> => Object.fromEntries( statuses.map( s => [ s, 0 ] ) ) as Record<Status, number>;
        const totals = { ...emptyCounts(), total: rows.length };
        const byPlugin = new Map<string, { counts: Record<Status, number>; durationMs: number }>();

        for ( const row of rows ) {
            totals[ row.status ]++;
            const entry = byPlugin.get( row.plugin ) ?? { counts: emptyCounts(), durationMs: 0 };
            entry.counts[ row.status ]++;
            entry.durationMs += row.durationMs;
            byPlugin.set( row.plugin, entry );
        }

        const rockVersions = [ ...new Set( this.rootSuite?.allTests()
            .flatMap( test => test.results.flatMap( r => r.annotations ).concat( test.annotations ) )
            .filter( a => a.type === "rock-version" && a.description )
            .map( a => a.description as string ) ?? [] ) ];

        const newFailures = totals.failed + totals[ "timed out" ] + totals.interrupted;
        const overall = result.status === "interrupted" ? "Interrupted"
            : newFailures > 0 ? "Failed"
                : totals[ "known issue" ] > 0 ? "Passed except known issues"
                    : "Passed";

        return {
            runId: getRunId(),
            site: ( process.env.ROCK_BASE_URL ?? "" ).replace( /\/+$/, "" ) || "(not set)",
            rockVersions: rockVersions.length > 0 ? rockVersions : [ "unknown" ],
            started: this.started.toISOString(),
            durationMs: Date.now() - this.started.getTime(),
            overall,
            command: [ "playwright", ...process.argv.slice( 2 ) ].join( " " ),
            testCode: describeTestCode(),
            warnings: pluginNameWarnings( [ ...byPlugin.keys() ] ),
            totals,
            plugins: [ ...byPlugin.entries() ]
                .map( ( [ plugin, entry ] ) => ( { plugin, ...entry } ) )
                .sort( ( a, b ) => a.plugin.localeCompare( b.plugin ) ),
            tests: rows.sort( ( a, b ) => a.plugin.localeCompare( b.plugin ) || a.file.localeCompare( b.file ) || a.line - b.line )
        };
    }
}

// #region Rendering

const statusIcon: Record<Status, string> = {
    "passed": "✅",
    "failed": "❌",
    "known issue": "🐞",
    "flaky": "⚠️",
    "skipped": "⏭️",
    "timed out": "⏱️",
    "interrupted": "⛔"
};

export function renderMarkdown( s: RunSummary ): string {
    const lines: string[] = [];
    const cell = ( value: string ): string => value.replace( /\|/g, "\\|" ).replace( /\r?\n/g, " " );

    lines.push( `# KFS end-to-end test run ${s.runId}`, "" );
    lines.push( `**${s.overall}**: ${countsText( s.totals )} of ${s.totals.total} tests.`, "" );
    lines.push( "| | |", "|---|---|" );
    lines.push( `| Site | ${s.site} |` );
    lines.push( `| Rock version | ${s.rockVersions.join( ", " )} |` );
    lines.push( `| Started | ${new Date( s.started ).toLocaleString()} |` );
    lines.push( `| Duration | ${formatDuration( s.durationMs )} |` );
    lines.push( `| Command | \`${cell( s.command )}\` |` );
    lines.push( `| Test code | ${cell( s.testCode )} |` );
    for ( const warning of s.warnings ?? [] ) {
        lines.push( "", `> **Warning:** ${warning}` );
    }

    lines.push( "", "## By plugin", "", "| Plugin | Tests | Passed | Failed | Known issue | Flaky | Skipped | Time |", "|---|---|---|---|---|---|---|---|" );
    for ( const p of s.plugins ) {
        const c = p.counts;
        const total = statuses.reduce( ( sum, st ) => sum + c[ st ], 0 );
        lines.push( `| ${p.plugin} | ${total} | ${c.passed} | ${c.failed + c[ "timed out" ] + c.interrupted} | ${c[ "known issue" ]} | ${c.flaky} | ${c.skipped} | ${formatDuration( p.durationMs )} |` );
    }

    for ( const p of s.plugins ) {
        lines.push( "", `## ${p.plugin}`, "", "| Result | Test | Time | Notes |", "|---|---|---|---|" );
        for ( const t of s.tests.filter( row => row.plugin === p.plugin ) ) {
            const notes = t.knownIssue ? `Known issue: ${t.knownIssue}` : t.error ? firstLine( t.error ) : t.retries > 0 ? `Passed on retry ${t.retries}` : "";
            lines.push( `| ${statusIcon[ t.status ]} ${t.status} | ${cell( t.describe ? `${t.describe} › ${t.title}` : t.title )} | ${formatDuration( t.durationMs )} | ${cell( notes )} |` );
        }
    }

    const problems = s.tests.filter( t => t.error );
    if ( problems.length > 0 ) {
        lines.push( "", "## Failure details" );
        for ( const t of problems ) {
            lines.push( "", `### ${statusIcon[ t.status ]} ${t.plugin}: ${t.title}`, "", `\`${t.file}:${t.line}\``, "" );
            if ( t.knownIssue ) {
                lines.push( `Known issue: ${t.knownIssue}`, "" );
            }
            lines.push( "```", t.error as string, "```" );
            for ( const a of t.attachments ) {
                lines.push( `- ${a.name}: [${a.path}](${encodeURI( a.path )})` );
            }
        }
    }

    lines.push( "", "Full detail, including step-by-step traces: `npm run report` (Playwright's HTML report for this run, in `html/`)." );
    return lines.join( "\n" ) + "\n";
}

export function renderHtml( s: RunSummary ): string {
    const pluginRows = s.plugins.map( p => {
        const c = p.counts;
        const total = statuses.reduce( ( sum, st ) => sum + c[ st ], 0 );
        const failed = c.failed + c[ "timed out" ] + c.interrupted;
        return `<tr><td><a href="#${slug( p.plugin )}">${esc( p.plugin )}</a></td><td class="n">${total}</td><td class="n">${c.passed}</td>` +
            `<td class="n${failed ? " bad" : ""}">${failed}</td><td class="n${c[ "known issue" ] ? " warn" : ""}">${c[ "known issue" ]}</td>` +
            `<td class="n">${c.flaky}</td><td class="n">${c.skipped}</td><td class="n">${formatDuration( p.durationMs )}</td></tr>`;
    } ).join( "\n" );

    const pluginSections = s.plugins.map( p => {
        const rows = s.tests.filter( t => t.plugin === p.plugin ).map( t => {
            const note = t.knownIssue ? `<span class="tag">Known issue</span> ${esc( t.knownIssue )}` : t.error ? esc( firstLine( t.error ) ) : t.retries > 0 ? `Passed on retry ${t.retries}` : "";
            const links = t.attachments.map( a => `<a href="${esc( encodeURI( a.path ) )}">${esc( a.name )}</a>` ).join( " · " );
            const detail = t.error ? `<details><summary>Details</summary><pre>${esc( t.error )}</pre>${links ? `<p>${links}</p>` : ""}` +
                `${t.attachments.filter( a => /\.png$/i.test( a.path ) ).map( a => `<img src="${esc( encodeURI( a.path ) )}" alt="${esc( a.name )}">` ).join( "" )}</details>` : "";
            return `<tr class="s-${slug( t.status )}"><td><span class="pill p-${slug( t.status )}">${esc( t.status )}</span></td>` +
                `<td><div class="title">${esc( t.title )}</div>` +
                `<div class="muted">${esc( t.describe )}${t.describe ? " · " : ""}<span class="mono">${esc( t.file )}:${t.line}</span> · ${formatDuration( t.durationMs )}</div>` +
                `${note ? `<div class="note">${note}</div>` : ""}${detail}</td></tr>`;
        } ).join( "\n" );
        return `<section id="${slug( p.plugin )}"><h2>${esc( p.plugin )}</h2><div class="scroll"><table class="tests"><thead><tr><th>Result</th><th>Test</th></tr></thead><tbody>${rows}</tbody></table></div></section>`;
    } ).join( "\n" );

    const t = s.totals;
    const tiles = [
        [ "Passed", t.passed, "good" ],
        [ "Failed", t.failed + t[ "timed out" ] + t.interrupted, "bad" ],
        [ "Known issue", t[ "known issue" ], "warn" ],
        [ "Flaky", t.flaky, "" ],
        [ "Skipped", t.skipped, "" ]
    ].map( ( [ label, value, tone ] ) => `<div class="tile ${value ? tone : ""}"><div class="v">${value}</div><div class="l">${label}</div></div>` ).join( "" );

    return `<!doctype html>
<html lang="en"><head><meta charset="utf-8"><meta name="viewport" content="width=device-width, initial-scale=1">
<title>KFS E2E Run ${esc( s.runId )}</title>
<style>${css}</style></head>
<body><main>
<p class="crumb"><a href="../../index.html">All runs</a></p>
<h1>KFS end-to-end test run <span class="mono">${esc( s.runId )}</span></h1>
<p class="overall o-${slug( s.overall )}">${esc( s.overall )}: ${esc( countsText( t ) )} of ${t.total} tests</p>
<div class="tiles">${tiles}</div>
<table class="facts">
<tr><th>Site</th><td>${esc( s.site )}</td></tr>
<tr><th>Rock version</th><td>${esc( s.rockVersions.join( ", " ) )}</td></tr>
<tr><th>Started</th><td>${esc( new Date( s.started ).toLocaleString() )}</td></tr>
<tr><th>Duration</th><td>${formatDuration( s.durationMs )}</td></tr>
<tr><th>Command</th><td class="mono">${esc( s.command )}</td></tr>
<tr><th>Test code</th><td>${esc( s.testCode )}</td></tr>
</table>
${( s.warnings ?? [] ).map( w => `<p class="warning">Warning: ${esc( w )}</p>` ).join( "" )}
<h2>By plugin</h2>
<div class="scroll"><table><thead><tr><th>Plugin</th><th class="n">Tests</th><th class="n">Passed</th><th class="n">Failed</th><th class="n">Known issue</th><th class="n">Flaky</th><th class="n">Skipped</th><th class="n">Time</th></tr></thead>
<tbody>${pluginRows}</tbody></table></div>
${pluginSections}
<p class="muted">Step-by-step traces: run <code>npm run report</code> for Playwright's HTML report of this run (<code>html/</code> in this folder).</p>
</main></body></html>
`;
}

/** test-results/index.html: every kept run, newest first. */
export function writeIndex(): void {
    const runs = listRunIds().reverse().map( id => {
        try {
            return JSON.parse( fs.readFileSync( path.join( runsRoot, id, "summary.json" ), "utf8" ) ) as RunSummary;
        }
        catch {
            return null;
        }
    } ).filter( ( run ): run is RunSummary => run !== null );

    const rows = runs.map( r => `<tr><td><a class="mono nowrap" href="runs/${esc( r.runId )}/summary.html">${esc( r.runId )}</a></td>` +
        `<td><span class="pill p-${slug( r.overall )}">${esc( r.overall )}</span></td>` +
        `<td>${esc( r.plugins.map( p => p.plugin ).join( ", " ) )}</td><td>${esc( r.rockVersions.join( ", " ) )}</td><td>${esc( r.site.replace( /^https?:\/\//, "" ) )}</td>` +
        `<td class="n">${r.totals.passed}/${r.totals.total}</td><td class="n">${formatDuration( r.durationMs )}</td></tr>` ).join( "\n" );

    fs.writeFileSync( path.join( runsRoot, "..", "index.html" ), `<!doctype html>
<html lang="en"><head><meta charset="utf-8"><meta name="viewport" content="width=device-width, initial-scale=1">
<title>KFS E2E Runs</title><style>${css}</style></head>
<body><main><h1>KFS end-to-end test runs</h1>
<p class="muted">The last ${keepRuns()} runs are kept. Each folder under <code>test-results/runs</code> holds that run's summary, JSON results, HTML report and failure screenshots.</p>
<div class="scroll"><table><thead><tr><th>Run</th><th>Result</th><th>Plugins</th><th>Rock</th><th>Site</th><th class="n">Passed</th><th class="n">Time</th></tr></thead>
<tbody>${rows}</tbody></table></div></main></body></html>
` );
}

const css = `
:root { --bg:#fff; --fg:#1d2330; --muted:#5b6475; --line:#e3e6ec; --card:#f6f7f9; --good:#1f7a3f; --bad:#b42318; --warn:#a15c07; --goodbg:#e7f5ec; --badbg:#fdecea; --warnbg:#fdf3e1; }
@media (prefers-color-scheme: dark) { :root { --bg:#14171c; --fg:#e6e8ec; --muted:#9aa3b2; --line:#2a2f38; --card:#1c2027; --good:#5fd08a; --bad:#ff8a80; --warn:#f2b45a; --goodbg:#16301f; --badbg:#3a1a18; --warnbg:#3a2a12; } }
* { box-sizing:border-box; } body { margin:0; background:var(--bg); color:var(--fg); font:14px/1.45 system-ui, "Segoe UI", sans-serif; }
main { max-width:1200px; margin:0 auto; padding:24px 16px 48px; } h1 { font-size:22px; margin:4px 0 8px; } h2 { font-size:17px; margin:28px 0 8px; }
a { color:inherit; } .crumb { margin:0; font-size:13px; } .muted { color:var(--muted); font-size:12px; } .mono, code, pre { font-family:Consolas, ui-monospace, monospace; }
table { width:100%; border-collapse:collapse; } th, td { text-align:left; padding:6px 8px; border-bottom:1px solid var(--line); vertical-align:top; } th { font-size:12px; color:var(--muted); font-weight:600; }
.n { text-align:right; white-space:nowrap; } td.bad { color:var(--bad); font-weight:600; } td.warn { color:var(--warn); font-weight:600; }
.facts { width:auto; margin:12px 0; } .facts th { padding-right:24px; }
.tiles { display:flex; flex-wrap:wrap; gap:8px; margin:12px 0; } .tile { background:var(--card); border-radius:8px; padding:8px 14px; min-width:96px; }
.tile .v { font-size:22px; font-weight:700; } .tile .l { font-size:12px; color:var(--muted); }
.tile.good .v { color:var(--good); } .tile.bad .v { color:var(--bad); } .tile.warn .v { color:var(--warn); }
.overall { font-weight:600; margin:0; } .o-passed { color:var(--good); } .o-failed, .o-interrupted { color:var(--bad); } .o-passed-except-known-issues { color:var(--warn); }
.pill { display:inline-block; padding:1px 8px; border-radius:999px; font-size:12px; font-weight:600; white-space:nowrap; background:var(--card); }
.p-passed { background:var(--goodbg); color:var(--good); } .p-failed, .p-timed-out, .p-interrupted { background:var(--badbg); color:var(--bad); }
.p-known-issue, .p-flaky, .p-passed-except-known-issues { background:var(--warnbg); color:var(--warn); }
.warning { color:var(--warn); background:var(--warnbg); padding:6px 10px; border-radius:6px; }
.tag { font-size:11px; font-weight:700; color:var(--warn); text-transform:uppercase; }
.title { font-weight:600; } .title, .mono { overflow-wrap:anywhere; } .scroll { overflow-x:auto; }
.tests td:first-child { width:1%; } .mono.nowrap { white-space:nowrap; overflow-wrap:normal; } .note { margin-top:4px; } details { margin-top:4px; } pre { white-space:pre-wrap; word-break:break-word; background:var(--card); padding:8px; border-radius:6px; font-size:12px; max-height:360px; overflow:auto; }
img { max-width:100%; border:1px solid var(--line); border-radius:6px; margin-top:6px; }
`;

// #endregion

// #region Helpers

/** Warns when a reported plugin name is not a current KFS plugin, so plugins.ts is kept in step with Rock Packages. */
function pluginNameWarnings( names: string[] ): string[] {
    const unmapped = names.filter( name => name.endsWith( "(not in plugins.ts)" ) );
    const unknown = findUnknownPluginNames( names.filter( name => !unmapped.includes( name ) ) ) ?? [];
    const warnings: string[] = [];
    if ( unmapped.length > 0 ) {
        warnings.push( `Test folder(s) with no plugin in shared/plugins.ts: ${unmapped.map( name => name.replace( " (not in plugins.ts)", "" ) ).join( ", " )}.` );
    }
    if ( unknown.length > 0 ) {
        warnings.push( `Not a current KFS plugin in Rock Packages: ${unknown.join( ", " )}. Update shared/plugins.ts.` );
    }
    return warnings;
}

function keepRuns(): number {
    const value = Number( process.env.KFS_E2E_KEEP_RUNS );
    return Number.isInteger( value ) && value > 0 ? value : defaultKeepRuns;
}

function listRunIds(): string[] {
    if ( !fs.existsSync( runsRoot ) ) {
        return [];
    }

    return fs.readdirSync( runsRoot ).filter( name => runIdPattern.test( name ) ).sort();
}

/** Deletes the oldest run folders beyond the number kept. Only folders named like a run id are touched. */
function pruneOldRuns(): void {
    const ids = listRunIds();
    for ( const id of ids.slice( 0, Math.max( 0, ids.length - keepRuns() ) ) ) {
        fs.rmSync( path.join( runsRoot, id ), { recursive: true, force: true } );
    }
}

/** Branch and commit of the test code (RockAssemblies), marked if it has uncommitted changes. */
function describeTestCode(): string {
    try {
        const git = ( args: string ): string => execSync( `git ${args}`, { cwd: __dirname, stdio: [ "ignore", "pipe", "ignore" ] } ).toString().trim();
        const dirty = git( "status --porcelain -- ." ) ? ", with uncommitted changes" : "";
        return `${git( "rev-parse --abbrev-ref HEAD" )} @ ${git( "rev-parse --short HEAD" )}${dirty}`;
    }
    catch {
        return "unknown";
    }
}

function cleanError( message: string ): string {
    // eslint-disable-next-line no-control-regex
    const plain = message.replace( /\u001b\[[0-9;]*m/g, "" ).trim();
    const redacted = redactSecrets( new Error( plain ) ).message;
    return redacted.length > 4000 ? `${redacted.substring( 0, 4000 )}\n…` : redacted;
}

function firstLine( message: string ): string {
    const line = message.split( /\r?\n/ ).find( l => l.trim() ) ?? "";
    return line.length > 220 ? `${line.substring( 0, 220 )}…` : line;
}

function countsText( c: Record<Status, number> ): string {
    const parts = [ `${c.passed} passed` ];
    const failed = c.failed + c[ "timed out" ] + c.interrupted;
    if ( failed ) {
        parts.push( `${failed} failed` );
    }
    if ( c[ "known issue" ] ) {
        parts.push( `${c[ "known issue" ]} known issue` );
    }
    if ( c.flaky ) {
        parts.push( `${c.flaky} flaky` );
    }
    if ( c.skipped ) {
        parts.push( `${c.skipped} skipped` );
    }
    return parts.join( ", " );
}

function formatDuration( ms: number ): string {
    const seconds = Math.round( ms / 1000 );
    if ( seconds < 60 ) {
        return `${seconds}s`;
    }
    const minutes = Math.floor( seconds / 60 );
    return minutes < 60 ? `${minutes}m ${seconds % 60}s` : `${Math.floor( minutes / 60 )}h ${minutes % 60}m`;
}

function slug( value: string ): string {
    return value.toLowerCase().replace( /[^a-z0-9]+/g, "-" ).replace( /^-|-$/g, "" );
}

function esc( value: string ): string {
    return value.replace( /&/g, "&amp;" ).replace( /</g, "&lt;" ).replace( />/g, "&gt;" ).replace( /"/g, "&quot;" );
}

// #endregion
