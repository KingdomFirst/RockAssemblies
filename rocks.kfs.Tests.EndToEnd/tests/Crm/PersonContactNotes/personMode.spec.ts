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
import { getLastCapturedEntity } from "../../../shared/testData/workflows";
import { defaultNoteText, expect, test } from "./personContactNotes.fixture";

/*
    Covers the manual plan's "Person Mode": with Workflow set to a Person workflow and
    Workflow Entity "Person", pick a person with the person picker, save a note, and
    verify the note saved and the workflow ran for that person.
*/
test.describe( "Person Contact Notes: person mode", () => {
    test( "PageLoad_NoParameters_ShowsPersonPickerWithoutNoteEditor", async ( { page, pcn, block } ) => {
        await pcn.open( page );

        await expect( block.selectionHeader ).toHaveText( "Select a Person" );
        await expect( block.personPicker ).toBeVisible();
        await expect( block.groupMemberDropDown ).toHaveCount( 0 );
        await expect( block.noteEditor ).toHaveCount( 0 );
    } );

    test( "SelectPerson_PersonPicker_ReloadsForPersonWithDefaultNoteText", async ( { page, pcn, block } ) => {
        await pcn.open( page );

        await block.selectPerson( pcn.subject.fullName, pcn.subject.id );

        await expect( block.personPicker ).toContainText( pcn.subject.fullName );
        await expect( block.noteText ).toHaveValue( defaultNoteText );
    } );

    test( "SaveNote_PersonWorkflowEntity_SavesNoteAndLaunchesWorkflowForPerson", async ( { page, pcn, block } ) => {
        const text = `KFS E2E person note ${Date.now()}`;

        await pcn.open( page );
        await block.selectPerson( pcn.subject.fullName, pcn.subject.id );
        await block.saveNote( text );

        const [ note ] = await pcn.getNotes( pcn.noteTypeIds.person, pcn.subject.id );
        expect( note?.Text, "The note was not saved on the selected person" ).toBe( text );
        expect( note.CreatedByPersonAliasId ).toBe( pcn.tester.primaryAliasId );
        await expect( block.root ).toContainText( text );

        // Rock hands a Person to a workflow's Entity attribute as the person's primary alias.
        const captured = await getLastCapturedEntity( pcn.api, pcn.captureWorkflowType );
        expect( captured, "Saving the note did not launch the workflow" ).not.toBeNull();
        expect( captured ).toMatchObject( { entityTypeGuid: pcn.entityTypeGuids.personAlias, entityId: pcn.subject.primaryAliasId } );
    } );

    // Not in the manual plan: the third Workflow Entity option.
    test( "SaveNote_NoteWorkflowEntity_LaunchesWorkflowForNote", async ( { page, pcn, block } ) => {
        await pcn.configure( { WorkflowEntity: "Note" } );
        const text = `KFS E2E note entity ${Date.now()}`;

        await pcn.open( page, { PersonId: pcn.subject.id } );
        await block.saveNote( text );

        const [ note ] = await pcn.getNotes( pcn.noteTypeIds.person, pcn.subject.id );
        expect( note?.Text ).toBe( text );

        const captured = await getLastCapturedEntity( pcn.api, pcn.captureWorkflowType );
        expect( captured, "Saving the note did not launch the workflow" ).not.toBeNull();
        expect( captured ).toMatchObject( { entityTypeGuid: pcn.entityTypeGuids.note, entityId: note.Id } );
    } );
} );
