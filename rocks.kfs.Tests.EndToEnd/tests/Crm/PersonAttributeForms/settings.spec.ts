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
import { expect, test } from "./personAttributeForms.fixture";

type SavedForm = {
    Guid: string;
    Fields: Array<{ Guid: string; FieldSource: number; PersonFieldType: number; AttributeId: number | null }>;
};

/*
    Covers the manual plan's "Edit Forms and Fields" setup step. The other specs set the
    Forms setting directly, so this is the one place the settings dialog itself is driven:
    it must open, show the configured forms, and save them back without loss. A change in
    Rock's pickers, grids or JSON handling usually shows up here first.
*/
test.describe( "Person Attribute Forms Advanced: Edit Forms and Fields", () => {
    test( "EditFormsAndFields_OpenAndSave_PreservesConfiguration", async ( { page, pafa, block } ) => {
        await pafa.configure( {
            Workflow: pafa.workflows.groupMember.guid,
            WorkflowEntity: "GroupMember"
        } );
        const formsAttributeId = pafa.blockPage.attributeIds[ "Forms" ];
        const workflowAttributeId = pafa.blockPage.attributeIds[ "Workflow" ];
        const formsBefore = JSON.parse( await pafa.api.getAttributeValue( formsAttributeId, pafa.blockPage.blockId ) ?? "[]" ) as SavedForm[];

        await openPageAsPerson( page, pafa.api, { pageId: pafa.blockPage.pageId, personId: pafa.person.id } );
        await block.openEditFormsAndFields();

        await expect( block.editModal.locator( "input[id$='_tbFormName']" ).first() ).toHaveValue( "KFS E2E Form 1" );
        await expect( block.editModal ).toContainText( "KFS E2E PAFA Text" );
        await expect( block.workflowEntityDropDown ).toHaveValue( "GroupMember" );

        await block.saveEditFormsAndFields();
        await expect( block.editModal ).toBeHidden();

        const formsAfter = JSON.parse( await pafa.api.getAttributeValue( formsAttributeId, pafa.blockPage.blockId ) ?? "[]" ) as SavedForm[];
        const summarize = ( forms: SavedForm[] ): unknown => forms.map( form => ( {
            guid: form.Guid.toLowerCase(),
            fields: form.Fields.map( field => ( {
                guid: field.Guid.toLowerCase(),
                source: field.FieldSource,
                personFieldType: field.FieldSource === 1 ? field.PersonFieldType : null,
                attributeId: field.AttributeId ?? null
            } ) )
        } ) );

        expect( summarize( formsAfter ) ).toEqual( summarize( formsBefore ) );
        expect( ( await pafa.api.getAttributeValue( workflowAttributeId, pafa.blockPage.blockId ) ?? "" ).toLowerCase() )
            .toBe( pafa.workflows.groupMember.guid.toLowerCase() );
    } );
} );
