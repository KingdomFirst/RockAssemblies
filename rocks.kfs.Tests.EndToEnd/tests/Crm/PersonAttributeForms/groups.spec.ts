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
import { openPageAsPerson } from "../../../shared/rockBrowser";
import { GroupMemberStatus, getGroupMembers, getMemberStatus } from "../../../shared/testData/groups";
import { getLastCapturedEntity } from "../../../shared/testData/workflows";
import { expect, test } from "./personAttributeForms.fixture";

/*
    Covers the manual plan's "Testing - Fields & Groups" group steps: adding the person
    to a group from the GroupId/GroupGuid page parameter or the Group block setting,
    honoring Allowed Group Types and Group Member Status, and the Group Member workflow
    entity.
*/
test.describe( "Person Attribute Forms Advanced: groups", () => {
    const groupSettings = {
        AllowGroupMembership: "True",
        EnablePassingGroupId: "True",
        GroupMemberStatus: String( GroupMemberStatus.Active )
    };

    test( "Submit_GroupIdOfAllowedGroupType_AddsGroupMemberWithConfiguredStatus", async ( { page, pafa, block } ) => {
        await pafa.configure( groupSettings );

        await openPageAsPerson( page, pafa.api, { pageId: pafa.blockPage.pageId, personId: pafa.person.id, parameters: { GroupId: pafa.allowedGroup.id } } );
        await block.finish();

        const members = await getGroupMembers( pafa.api, pafa.allowedGroup.id, pafa.person.id );
        expect( members ).toHaveLength( 1 );
        // Active is not the default (Pending), so this proves the setting was honored.
        expect( getMemberStatus( members[ 0 ] ) ).toBe( GroupMemberStatus.Active );
    } );

    test( "Submit_GroupGuidOfAllowedGroupType_AddsGroupMember", async ( { page, pafa, block } ) => {
        await pafa.configure( { ...groupSettings, EnablePassingGroupId: "False" } );

        await openPageAsPerson( page, pafa.api, { pageId: pafa.blockPage.pageId, personId: pafa.person.id, parameters: { GroupGuid: pafa.allowedGroup.guid } } );
        await block.finish();

        expect( await getGroupMembers( pafa.api, pafa.allowedGroup.id, pafa.person.id ) ).toHaveLength( 1 );
    } );

    test( "Submit_GroupMemberStatusPending_AddsPendingGroupMember", async ( { page, pafa, block } ) => {
        await pafa.configure( { ...groupSettings, GroupMemberStatus: String( GroupMemberStatus.Pending ) } );

        await openPageAsPerson( page, pafa.api, { pageId: pafa.blockPage.pageId, personId: pafa.person.id, parameters: { GroupId: pafa.allowedGroup.id } } );
        await block.finish();

        const members = await getGroupMembers( pafa.api, pafa.allowedGroup.id, pafa.person.id );
        expect( members ).toHaveLength( 1 );
        expect( getMemberStatus( members[ 0 ] ) ).toBe( GroupMemberStatus.Pending );
    } );

    test( "Submit_GroupMemberWorkflowEntity_LaunchesWorkflowWithGroupMember", async ( { page, pafa, block } ) => {
        await pafa.configure( {
            ...groupSettings,
            Workflow: pafa.workflows.groupMember.guid,
            WorkflowEntity: "GroupMember"
        } );

        await openPageAsPerson( page, pafa.api, { pageId: pafa.blockPage.pageId, personId: pafa.person.id, parameters: { GroupId: pafa.allowedGroup.id } } );
        await block.finish();

        const members = await getGroupMembers( pafa.api, pafa.allowedGroup.id, pafa.person.id );
        expect( members ).toHaveLength( 1 );

        const captured = await getLastCapturedEntity( pafa.api, pafa.workflows.groupMember );
        expect( captured, "Workflow was not launched" ).not.toBeNull();
        expect( captured?.entityTypeGuid ).toBe( pafa.entityTypeGuids.groupMember );
        expect( captured?.entityId ).toBe( members[ 0 ].Id );
    } );

    test( "Submit_GroupIdOfDisallowedGroupType_DoesNotAddGroupMember", async ( { page, pafa, block } ) => {
        await pafa.configure( groupSettings );

        await openPageAsPerson( page, pafa.api, { pageId: pafa.blockPage.pageId, personId: pafa.person.id, parameters: { GroupId: pafa.disallowedGroup.id } } );
        await block.finish();

        expect( await getGroupMembers( pafa.api, pafa.disallowedGroup.id, pafa.person.id ) ).toHaveLength( 0 );
    } );

    test( "Submit_GroupIdWhenPassingGroupIdDisabled_DoesNotAddGroupMember", async ( { page, pafa, block } ) => {
        await pafa.configure( { ...groupSettings, EnablePassingGroupId: "False" } );

        await openPageAsPerson( page, pafa.api, { pageId: pafa.blockPage.pageId, personId: pafa.person.id, parameters: { GroupId: pafa.allowedGroup.id } } );
        await block.finish();

        expect( await getGroupMembers( pafa.api, pafa.allowedGroup.id, pafa.person.id ) ).toHaveLength( 0 );
    } );

    test( "Submit_GroupMembershipDisallowed_DoesNotAddGroupMember", async ( { page, pafa, block } ) => {
        await pafa.configure( { ...groupSettings, AllowGroupMembership: "False" } );

        await openPageAsPerson( page, pafa.api, { pageId: pafa.blockPage.pageId, personId: pafa.person.id, parameters: { GroupId: pafa.allowedGroup.id } } );
        await block.finish();

        expect( await getGroupMembers( pafa.api, pafa.allowedGroup.id, pafa.person.id ) ).toHaveLength( 0 );
    } );

    test( "Submit_GroupSettingOfDisallowedGroupType_AddsGroupMemberWithoutPageParameter", async ( { page, pafa, block } ) => {
        // The Group setting overrides Allowed Group Types.
        await pafa.configure( { ...groupSettings, Group: pafa.disallowedGroup.guid } );

        await openPageAsPerson( page, pafa.api, { pageId: pafa.blockPage.pageId, personId: pafa.person.id } );
        await block.finish();

        expect( await getGroupMembers( pafa.api, pafa.disallowedGroup.id, pafa.person.id ) ).toHaveLength( 1 );
    } );

    test( "Submit_AlreadyGroupMember_DoesNotAddDuplicateMember", async ( { page, pafa, block } ) => {
        await pafa.configure( groupSettings );

        for ( let i = 0; i < 2; i++ ) {
            await openPageAsPerson( page, pafa.api, { pageId: pafa.blockPage.pageId, personId: pafa.person.id, parameters: { GroupId: pafa.allowedGroup.id } } );
            await block.finish();
        }

        expect( await getGroupMembers( pafa.api, pafa.allowedGroup.id, pafa.person.id ) ).toHaveLength( 1 );
    } );
} );
