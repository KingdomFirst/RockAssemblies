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
import { expect, test } from "./stepsToCare.fixture";
import { CareEntryBlock } from "./stepsToCarePages";

// Traces are off: see actions.spec.ts.
test.use( { trace: "off" } );

/*
    Covers the manual plan's "Enter a Care Need" steps, the family "+" rows, and
    "Complete Need" (documentation scenarios 1 and 3: entering a need with an assigned
    worker; a Birth-style need with Include Family creating needs for family members).
*/
test.describe( "Steps to Care: entering care needs", () => {
    test( "EnterCareNeed_SelfAsWorkerAndCustomFollowUp_ShowsAssignedNeedOnDashboard", async ( { page, stc } ) => {
        const details = stc.uniqueDetails( "Hospital visit" );
        const dashboard = await stc.openDashboard( page );

        await dashboard.enterNeedButton.click();
        await page.waitForURL( new RegExp( `/page/${stc.pages.entry.pageId}` ) );
        const entry = new CareEntryBlock( page, stc.pages.entry.blockId );
        await entry.pickRequestor( stc.requestor.fullName, stc.requestor.id );
        await entry.chooseCategory( stc.category.id );
        await entry.details.fill( details );
        await entry.setCustomFollowUp( 7, 2 );
        await entry.assignPerson( stc.tester.fullName, stc.tester.id );
        await entry.save();

        const row = dashboard.row( details );
        await expect( row ).toBeVisible();
        await expect( row ).toContainText( stc.requestor.fullName );
        await expect( row.locator( `td[title='${stc.category.name}']` ), "Category color cell" ).toHaveCount( 1 );
        await expect( row.locator( `[personid='${stc.tester.id}'].photo-icon` ), "The tester is shown as assigned" ).toHaveCount( 1 );
        await expect( row, "Rows assigned to the current person are highlighted" ).toHaveClass( /\bassigned\b/ );

        // The custom follow up was saved with the need.
        await dashboard.openNeed( row );
        await expect( entry.customFollowUp ).toBeChecked();
        await expect( entry.followUpAfterDays ).toHaveValue( "7" );
        await expect( entry.followUpRepeatTimes ).toHaveValue( "2" );
        await expect( entry.assignedGrid ).toContainText( stc.tester.fullName );
    } );

    test( "EnterCareNeed_IncludeFamily_PlusShowsFamilyMembersNeeds", async ( { page, stc } ) => {
        const details = stc.uniqueDetails( "Birth" );

        const dashboard = await stc.enterNeed( page, { details, includeFamily: true } );

        const row = dashboard.row( details );
        const needId = await dashboard.needId( row );
        const toggle = dashboard.familyToggle( row );
        await expect( toggle, "No '+' before the person's name" ).toBeVisible();
        await expect( dashboard.familyRows( needId ).first() ).toBeHidden();

        await toggle.click();

        await expect( dashboard.familyRows( needId ) ).toHaveCount( 1 );
        await expect( dashboard.familyRows( needId ).first() ).toBeVisible();
        await expect( dashboard.familyRows( needId ).first() ).toContainText( stc.requestorChild.fullName );
    } );

    test( "FamilyNeed_EditFromFamilyRow_SavesDetailsAndWorker", async ( { page, stc } ) => {
        const details = stc.uniqueDetails( "Family need" );
        const childDetails = stc.uniqueDetails( "Child follow up" );
        const dashboard = await stc.enterNeed( page, { details, includeFamily: true } );
        const row = dashboard.row( details );
        const needId = await dashboard.needId( row );
        await dashboard.familyToggle( row ).click();

        await dashboard.openNeed( dashboard.familyRows( needId ).first() );
        const entry = new CareEntryBlock( page, stc.pages.entry.blockId );
        await expect( entry.requestorPicker ).toContainText( stc.requestorChild.fullName );
        await entry.details.fill( childDetails );
        await entry.assignPerson( stc.tester.fullName, stc.tester.id );
        await entry.save();

        // Assigned to the tester now, the family member's need shows as its own row.
        await expect( dashboard.row( childDetails ) ).toContainText( stc.requestorChild.fullName );
    } );

    test( "CompleteNeed_FromActions_RemovesNeedFromDashboard", async ( { page, stc } ) => {
        const details = stc.uniqueDetails( "Complete" );
        const dashboard = await stc.enterNeed( page, { details } );
        await expect( dashboard.row( details ) ).toBeVisible();

        await dashboard.action( dashboard.row( details ), "Complete Need" );

        await expect( dashboard.root.locator( "table tbody tr", { hasText: details } ) ).toHaveCount( 0 );
    } );
} );
