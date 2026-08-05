// <copyright>
// Copyright 2025 by Kingdom First Solutions
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
using System;
using Rock;
using Rock.Data;
using Rock.Model;
using Rock.Plugin;

namespace rocks.kfs.StepsToCare.Migrations
{
    [MigrationNumber( 24, "1.16.0" )]
    public class AddHistory : Migration
    {
        public override void Up()
        {
            RockMigrationHelper.UpdateCategory( Rock.SystemGuid.EntityType.HISTORY, "Steps to Care - Care Need", "fa fa-hand-holding", "Care Need entity history entries for the Steps to Care plugin.", SystemGuid.Category.HISTORY_CARE_NEED );
            RockMigrationHelper.UpdateCategory( Rock.SystemGuid.EntityType.HISTORY, "Steps to Care", "fa fa-hand-holding", "Person History entries from Steps to Care plugin (assign workers or other history directly related to a person).", SystemGuid.Category.HISTORY_PERSON_STEPS_TO_CARE, 0, Rock.SystemGuid.Category.HISTORY_PERSON );
            Sql( $@"
                DECLARE 
                    @AttributeId int,
                    @EntityId int

                SET @AttributeId = (SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '0C405062-72BB-4362-9738-90C9ED5ACDDE')
                SET @EntityId = (SELECT TOP 1 [ID] FROM [Category] where [Guid] = '{SystemGuid.Category.HISTORY_CARE_NEED}')

                DELETE FROM [AttributeValue] WHERE [Guid] = 'B825A151-9DF6-473A-AA56-B7714AA58900'

                INSERT INTO [AttributeValue] ([IsSystem],[AttributeId],[EntityId],[Value],[Guid])
                VALUES(
                    1,@AttributeId,@EntityId,'~/StepsToCareDetail/{{0}}','B825A151-9DF6-473A-AA56-B7714AA58900')" );

            Sql( $@"
                DECLARE 
                    @AttributeId int,
                    @EntityId int

                SET @AttributeId = (SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '0C405062-72BB-4362-9738-90C9ED5ACDDE')
                SET @EntityId = (SELECT TOP 1 [ID] FROM [Category] where [Guid] = '{SystemGuid.Category.HISTORY_PERSON_STEPS_TO_CARE}')

                DELETE FROM [AttributeValue] WHERE [Guid] = '34C7CE36-192E-4BE9-A895-220B99AA09F6'

                INSERT INTO [AttributeValue] ([IsSystem],[AttributeId],[EntityId],[Value],[Guid])
                VALUES(
                    1,@AttributeId,@EntityId,'~/StepsToCareDetail/{{0}}','34C7CE36-192E-4BE9-A895-220B99AA09F6')" );

            RockMigrationHelper.AddPage( true, "27953B65-21E2-4CA9-8461-3AFAD46D9BC8", "D65F783D-87A9-4CC9-8110-E83466A0EADB", "Care Need History", "", "653F2B13-E828-4263-B12B-C419A1833197" );
            RockMigrationHelper.AddBlock( "653F2B13-E828-4263-B12B-C419A1833197", "", "C6C2DF41-A50D-4975-B21C-4EFD6FF3E8D0", "History Log", "Main", "", "", 0, "C5E4F1D7-0B2A-4F3C-8D6E-9B5A0F1D7E3F" );

            //   Attribute: Heading
            RockMigrationHelper.AddBlockAttributeValue( "C5E4F1D7-0B2A-4F3C-8D6E-9B5A0F1D7E3F", "EAAE646D-69CD-41AC-9B8A-7EC5A446B379", @"{{ Entity.EntityStringValue }} (ID:{{ Entity.Id }})" );
            //   Attribute: Entity Type
            RockMigrationHelper.AddBlockAttributeValue( "C5E4F1D7-0B2A-4F3C-8D6E-9B5A0F1D7E3F", "E9BB2534-D54E-4D50-A241-FCD45EFADE32", @"87ac878d-6740-43eb-9389-b8440ac595c3" );
            //   Attribute: Category
            RockMigrationHelper.AddBlockAttributeValue( "C5E4F1D7-0B2A-4F3C-8D6E-9B5A0F1D7E3F", "44092D4B-213D-4572-A005-C2B35E0B4082", @"4c4b37d6-6966-4864-ab06-ff92ece501bb" );

            RockMigrationHelper.UpdatePageContext( "653F2B13-E828-4263-B12B-C419A1833197", "rocks.kfs.StepsToCare.Model.CareNeed", "CareNeedId", "81EE561B-C87F-46DE-AAF4-8B6476E2AA79" );
            RockMigrationHelper.AddOrUpdatePageRoute( "653F2B13-E828-4263-B12B-C419A1833197", "StepsToCareDetail/{CareNeedId}/History", "73EDDBA6-737D-423F-B513-6E08113DA346" );

            // Attribute for  BlockType
            //   BlockType: Care Entry
            //   Category: KFS > Steps To Care
            //   Attribute: Care Need History Page
            RockMigrationHelper.AddOrUpdateBlockTypeAttribute( "4F0F9ED7-9F74-4152-B27F-D9B2A458AFBE", "BD53F9C9-EBA9-4D3F-82EA-DE5DD34A8108", "Care Need History Page", "CareNeedHistoryPage", "Care Need History Page", @"Page used to display history details.", 0, @"", "9C990D50-769F-4BFD-8AFE-02092C76B68C" );
            RockMigrationHelper.AddBlockAttributeValue( "F953C5EF-6504-45F9-81A8-063518B7AB61", "9C990D50-769F-4BFD-8AFE-02092C76B68C", @"653f2b13-e828-4263-b12b-c419a1833197,73eddba6-737d-423f-b513-6e08113da346" );

            // Attribute for  BlockType
            //   BlockType: Care Dashboard
            //   Category: KFS > Steps To Care
            //   Attribute: Care Need History Page
            RockMigrationHelper.AddOrUpdateBlockTypeAttribute( "AF14CB6C-F915-4449-9CB7-7C44B624B051", "BD53F9C9-EBA9-4D3F-82EA-DE5DD34A8108", "Care Need History Page", "CareNeedHistoryPage", "Care Need History Page", @"Page used to view history of Care Needs (if not set the action will not show)", 11, @"", "69F3EC39-E754-4E94-85F9-3DC2ED6099C4" );
            RockMigrationHelper.AddBlockAttributeValue( "EADBE3F0-F64B-4583-B49D-F0031BBC929F", "69F3EC39-E754-4E94-85F9-3DC2ED6099C4", @"653f2b13-e828-4263-b12b-c419a1833197,73eddba6-737d-423f-b513-6e08113da346" );
        }

        public override void Down()
        {
            RockMigrationHelper.DeleteCategory( SystemGuid.Category.HISTORY_CARE_NEED );
            RockMigrationHelper.DeleteCategory( SystemGuid.Category.HISTORY_PERSON_STEPS_TO_CARE );

            RockMigrationHelper.DeletePageContext( "81EE561B-C87F-46DE-AAF4-8B6476E2AA79" );
            RockMigrationHelper.DeletePageRoute( "73EDDBA6-737D-423F-B513-6E08113DA346" );
            RockMigrationHelper.DeleteBlock( "C5E4F1D7-0B2A-4F3C-8D6E-9B5A0F1D7E3F" );
            RockMigrationHelper.DeletePage( "653F2B13-E828-4263-B12B-C419A1833197" );

            RockMigrationHelper.DeleteAttribute( "9C990D50-769F-4BFD-8AFE-02092C76B68C" );
            RockMigrationHelper.DeleteAttribute( "69F3EC39-E754-4E94-85F9-3DC2ED6099C4" );
        }
    }
}