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
import { readXlsxStrings } from "../../../shared/xlsx";
import { expect, test } from "./fundraisingProgress.fixture";

/*
    Not in the manual plan: the show/hide settings and the Excel export that this
    advanced block adds to Rock's core Fundraising Progress block.
*/
test.describe( "Fundraising Progress: display settings and export", () => {
    test( "PageLoad_DisplaySettingsOff_HidesTitleTotalsMemberDetailsAndExport", async ( { page, afp, block } ) => {
        await afp.configure( {
            ShowGroupTitle: "False",
            ShowTotalAmountRaised: "False",
            ShowGroupTotalGoals: "False",
            ShowGroupMemberGoalAmounts: "False",
            ShowGroupMemberGoalProgressBars: "False",
            ShowExcelExportButton: "False"
        } );

        await afp.open( page, { GroupId: afp.group.id } );

        await expect( block.title ).toHaveCount( 0 );
        await expect( block.totalRaised ).toHaveCount( 0 );
        await expect( block.groupTotals ).toHaveCount( 0 );
        await expect( block.exportButton ).toHaveCount( 0 );

        // Member names still listed, without amounts or bars.
        await expect( block.memberRows ).toHaveCount( afp.participants.length );
        const name = afp.participants[ 0 ].person.fullName;
        await expect( block.memberAmounts( name ) ).toHaveCount( 0 );
        await expect( block.memberProgressBar( name ) ).toHaveCount( 0 );
    } );

    test( "PageLoad_GroupMemberGoalsOff_HidesMemberList", async ( { page, afp, block } ) => {
        await afp.configure( { ShowGroupMemberGoals: "False" } );

        await afp.open( page, { GroupId: afp.group.id } );

        await expect( block.memberRows ).toHaveCount( 0 );
        await expect( block.groupTotalAmounts ).toHaveText( `$${afp.expectedTotals.raised}/$${afp.expectedTotals.goal}` );
    } );

    test( "ExportToExcel_GroupIdParameter_DownloadsWorkbookNamedForGroup", async ( { page, afp, block } ) => {
        await afp.open( page, { GroupId: afp.group.id } );

        const download = await block.exportToExcel();

        expect( download.suggestedFilename() ).toBe( `FinancialProgress_${afp.groupName.replace( /[^A-Za-z0-9_\- ]/g, "" )}.xlsx` );
        const content = fs.readFileSync( await download.path() );
        const strings = readXlsxStrings( content );
        expect( strings ).toEqual( expect.arrayContaining( [ "NickName", "LastName", "IndividualGoal", "TotalRaised", "PercentageRaised" ] ) );
        for ( const participant of afp.participants ) {
            expect( strings, `${participant.person.fullName} is missing from the export` ).toContain( participant.person.fullName.split( " " )[ 0 ] );
        }
    } );
} );
