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
/*
    Opens a run's report. Usage:
        node scripts/openReport.js summary [run id]   the run's summary.html (default: latest run)
        node scripts/openReport.js html [run id]      Playwright's HTML report for the run
        node scripts/openReport.js index              test-results/index.html, every kept run
*/
const { spawnSync } = require( "child_process" );
const fs = require( "fs" );
const path = require( "path" );

const resultsRoot = path.join( __dirname, "..", "test-results" );
const runsRoot = path.join( resultsRoot, "runs" );
const [ kind = "summary", requestedRun ] = process.argv.slice( 2 );

function openFile( file ) {
    if ( !fs.existsSync( file ) ) {
        console.error( `Not found: ${file}` );
        process.exit( 1 );
    }
    const command = process.platform === "win32" ? "cmd" : process.platform === "darwin" ? "open" : "xdg-open";
    const args = process.platform === "win32" ? [ "/c", "start", "", file ] : [ file ];
    spawnSync( command, args, { stdio: "inherit" } );
    console.log( file );
}

if ( kind === "index" ) {
    openFile( path.join( resultsRoot, "index.html" ) );
    process.exit( 0 );
}

const runs = fs.existsSync( runsRoot ) ? fs.readdirSync( runsRoot ).filter( name => /^\d{4}-\d{2}-\d{2}_\d{6}$/.test( name ) ).sort() : [];
const run = requestedRun ?? runs[ runs.length - 1 ];
if ( !run ) {
    console.error( "No test runs found under test-results/runs. Run the tests first." );
    process.exit( 1 );
}

if ( kind === "html" ) {
    const result = spawnSync( "npx", [ "playwright", "show-report", path.join( runsRoot, run, "html" ) ], { stdio: "inherit", shell: true } );
    process.exit( result.status ?? 0 );
}

openFile( path.join( runsRoot, run, "summary.html" ) );
