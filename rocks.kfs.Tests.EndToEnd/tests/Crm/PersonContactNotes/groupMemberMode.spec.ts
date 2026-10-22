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
import { GroupMemberStatus, getMemberStatus } from "../../../shared/testData/groups";
import { getLastCapturedEntity } from "../../../shared/testData/workflows";
import { expect, setStatusActiveWorkflowTypeName, test } from "./personContactNotes.fixture";

/*
    Covers the manual plan's "Group Member Mode": with Workflow set to a GroupMember
    workflow and Workflow Entity "Group Member", open the page with &GroupId=, check the
    person picker becomes a "Select a Group Member" drop down, pick a member, save a
    note, and verify the note saved and the workflow ran for that member.
*/
test.describe( "Person Contact Notes: group member mode", () => {
    test( "PageLoad_WithGroupId_ShowsGroupMemberDropDownInsteadOfPersonPicker", async ( { page, pcn, block } ) => {
        await pcn.open( page, { GroupId: pcn.group.id } );

        await expect( block.selectionHeader ).toHaveText( "Select a Group Member" );
        await expect( block.personPicker ).toHaveCount( 0 );
        await expect( block.groupMemberDropDown ).toBeVisible();
        await expect( block.groupMemberDropDown.locator( "option" ).filter( { hasText: /\S/ } ) ).toHaveText( [ pcn.pendingSubject.fullName, pcn.subject.fullName ] );
        await expect( block.noteEditor ).toHaveCount( 0 );
    } );

    test( "SelectGroupMember_DropDown_ReloadsForGroupMember", async ( { page, pcn, block } ) => {
        await pcn.open( page, { GroupId: pcn.group.id } );

        await block.selectGroupMember( pcn.subjectMemberId );

        await expect( block.selectionHeader ).toHaveText( "Select a Group Member" );
        await expect( block.groupMemberDropDown ).toHaveValue( pcn.subjectMemberId.toString() );
        await expect( block.noteEditor ).toBeVisible();
    } );

    test( "SaveNote_GroupMemberWorkflowEntity_SavesNoteAndLaunchesWorkflowForGroupMember", async ( { page, pcn, block } ) => {
        await pcn.configure( { WorkflowEntity: "GroupMember" } );
        const text = `KFS E2E group member note ${Date.now()}`;

        await pcn.open( page, { GroupId: pcn.group.id } );
        await block.selectGroupMember( pcn.subjectMemberId );
        await block.saveNote( text );

        const [ note ] = await pcn.getNotes( pcn.noteTypeIds.groupMember, pcn.subjectMemberId );
        expect( note?.Text, "The note was not saved on the selected group member" ).toBe( text );
        expect( note.CreatedByPersonAliasId ).toBe( pcn.tester.primaryAliasId );
        await expect( block.root ).toContainText( text );

        const captured = await getLastCapturedEntity( pcn.api, pcn.captureWorkflowType );
        expect( captured, "Saving the note did not launch the workflow" ).not.toBeNull();
        expect( captured ).toMatchObject( { entityTypeGuid: pcn.entityTypeGuids.groupMember, entityId: pcn.subjectMemberId } );
    } );

    // The manual plan's suggested "simple verification" on the KFS beta site.
    test( "SaveNote_SetStatusActiveWorkflow_ActivatesPendingMember", async ( { page, pcn, block } ) => {
        const workflowType = await pcn.api.first( "WorkflowTypes", `Name eq '${setStatusActiveWorkflowTypeName}' and IsActive eq true` );
        test.skip( !workflowType, `No active '${setStatusActiveWorkflowTypeName}' workflow type on this site.` );

        const lastWorkflow = await pcn.api.first( "Workflows", `WorkflowTypeId eq ${workflowType!.Id}`, "&$orderby=Id desc" );
        await pcn.configure( { Workflow: String( workflowType!.Guid ).toLowerCase(), WorkflowEntity: "GroupMember" } );

        try {
            await pcn.open( page, { GroupId: pcn.group.id } );
            await block.selectGroupMember( pcn.pendingMemberId );
            await block.saveNote( `KFS E2E set status note ${Date.now()}` );

            const member = await pcn.api.getById( "GroupMembers", pcn.pendingMemberId );
            expect( getMemberStatus( member ), "The workflow did not set the member Active" ).toBe( GroupMemberStatus.Active );
        }
        finally {
            // Remove only the workflows this test launched; the type belongs to the site.
            for ( const workflow of await pcn.api.query( "Workflows", `WorkflowTypeId eq ${workflowType!.Id} and Id gt ${lastWorkflow?.Id ?? 0}` ) ) {
                await pcn.api.delete( "Workflows", workflow.Id );
            }
        }
    } );
} );
