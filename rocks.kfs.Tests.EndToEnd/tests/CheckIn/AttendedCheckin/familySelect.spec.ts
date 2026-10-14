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
import { CheckinFamily, getActivePeopleByLastName } from "../../../shared/testData/checkin";
import { expect, test } from "./attendedCheckin.fixture";

/*
    Covers the manual plan's "Add Visitor / Add Person / Add Family" plus the Family page
    selection rules. The add buttons create real people through the plugin, so each of
    these tests works in a new family of its own (createRunFamily) and never touches the
    persistent test family; teardown inactivates everyone they create.
*/
test.describe( "Attended Check-in: Family Select", () => {
    test.beforeEach( async ( { checkin, checkinPages } ) => {
        await checkinPages.startCheckin( { kioskId: checkin.setup.kioskId, areaId: checkin.setup.areaId } );
    } );

    test( "FamilySelect_Default_PreselectsEligibleMembers", async ( { checkin, checkinPages } ) => {
        await checkinPages.searchForFamily( CheckinFamily.lastName );

        await expect( checkinPages.personButton( checkin.familyAdult.id ) ).toHaveClass( /\bactive\b/ );
        await expect( checkinPages.personButton( checkin.familyChild.id ) ).toHaveClass( /\bactive\b/ );
    } );

    test( "FamilySelect_NoPersonSelected_WarnsToPickPerson", async ( { checkin, checkinPages } ) => {
        await checkinPages.searchForFamily( CheckinFamily.lastName );
        await checkinPages.togglePerson( checkin.familyAdult.id );
        await checkinPages.togglePerson( checkin.familyChild.id );

        await checkinPages.familyNext();

        await expect( checkinPages.alert ).toContainText( "Please pick at least one person." );
        await expect( checkinPages.page ).toHaveURL( /\/attendedcheckin\/family/i );
    } );

    test( "FamilySelect_OnePersonSelected_ConfirmsOnlyThatPerson", async ( { checkin, checkinPages } ) => {
        await checkinPages.searchForFamily( CheckinFamily.lastName );
        await checkinPages.togglePerson( checkin.familyChild.id );

        await checkinPages.familyNext();

        await expect( checkinPages.page ).toHaveURL( /\/attendedcheckin\/confirm/i );
        await expect( checkinPages.confirmRow( CheckinFamily.adultFirstName ) ).toHaveCount( 1 );
        await expect( checkinPages.confirmRow( CheckinFamily.childFirstName ) ).toHaveCount( 0 );
    } );

    test( "AddPerson_NewPerson_AddedToFamilyAndSelected", async ( { checkin, checkinPages } ) => {
        const firstName = "Addie";
        const family = await checkin.createRunFamily( "p" );
        const lastName = `${checkin.runLastName}p`;
        await checkinPages.searchForFamily( lastName );

        await checkinPages.addNewPersonInModal( checkinPages.addPersonButton, { firstName, lastName, gender: "Female" } );

        const button = checkinPages.memberButtons.filter( { hasText: firstName } );
        await expect( button ).toHaveCount( 1 );
        await expect( button ).toHaveClass( /\bactive\b/ );

        const added = ( await getActivePeopleByLastName( checkin.api, lastName ) ).filter( person => person.FirstName === firstName );
        expect( added, "Add Person did not create exactly one person" ).toHaveLength( 1 );
        const membership = await checkin.api.query( "GroupMembers", `GroupId eq ${family.familyId} and PersonId eq ${added[ 0 ].Id}` );
        expect( membership, "The new person was not added to the selected family" ).toHaveLength( 1 );
    } );

    test( "AddVisitor_NewChild_ListedAsVisitorNotFamilyMember", async ( { checkin, checkinPages } ) => {
        const firstName = "Vinny";
        const family = await checkin.createRunFamily( "v" );
        const lastName = `${checkin.runLastName}v`;
        await checkinPages.searchForFamily( lastName );

        // A child visitor gets a "can check in" relationship to the family and a family of their own.
        await checkinPages.addNewPersonInModal( checkinPages.addVisitorButton, { firstName, lastName, gender: "Male", birthDate: "1/1/2020" } );

        await expect( checkinPages.visitorButtons.filter( { hasText: firstName } ) ).toHaveCount( 1 );
        await expect( checkinPages.memberButtons.filter( { hasText: firstName } ) ).toHaveCount( 0 );

        const added = ( await getActivePeopleByLastName( checkin.api, lastName ) ).filter( person => person.FirstName === firstName );
        expect( added, "Add Visitor did not create exactly one person" ).toHaveLength( 1 );
        const membership = await checkin.api.query( "GroupMembers", `GroupId eq ${family.familyId} and PersonId eq ${added[ 0 ].Id}` );
        expect( membership, "A visitor must not join the selected family" ).toHaveLength( 0 );
    } );

    test( "AddFamily_NewFamily_SelectedAndConfirmable", async ( { checkin, checkinPages } ) => {
        const family = await checkin.createRunFamily( "f" );
        const lastName = `${checkin.runLastName}f`;
        await checkinPages.searchForFamily( lastName );

        await checkinPages.addNewFamily( [
            { firstName: "Fenna", lastName, gender: "Female" },
            { firstName: "Felix", lastName, gender: "Male" }
        ] );

        await expect( checkinPages.memberButtons.filter( { hasText: "Fenna" } ) ).toHaveCount( 1 );
        await expect( checkinPages.memberButtons.filter( { hasText: "Felix" } ) ).toHaveCount( 1 );

        const people = await getActivePeopleByLastName( checkin.api, lastName );
        const fenna = people.filter( person => person.FirstName === "Fenna" );
        const felix = people.filter( person => person.FirstName === "Felix" );
        expect( fenna, "Add Family did not create Fenna exactly once" ).toHaveLength( 1 );
        expect( felix, "Add Family did not create Felix exactly once" ).toHaveLength( 1 );
        expect( fenna[ 0 ].PrimaryFamilyId, "Add Family members should share a family" ).toBe( felix[ 0 ].PrimaryFamilyId );
        expect( fenna[ 0 ].PrimaryFamilyId, "Add Family should create a new family" ).not.toBe( family.familyId );

        await checkinPages.familyNext();
        await expect( checkinPages.page ).toHaveURL( /\/attendedcheckin\/confirm/i );
        await expect( checkinPages.confirmRow( "Fenna" ) ).toHaveCount( 1 );
        await expect( checkinPages.confirmRow( "Felix" ) ).toHaveCount( 1 );
    } );
} );
