﻿// <copyright>
// Copyright 2021 by Kingdom First Solutions
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
using Rock.Plugin;

namespace rocks.kfs.StepsToCare
{
    [MigrationNumber( 2, "1.12.3" )]
    public class CreateDefinedTypes : Migration
    {
        public override void Up()
        {
            RockMigrationHelper.AddDefinedType( "Steps to Care", "Status", "The status of Care Needs.", SystemGuid.DefinedType.CARE_NEED_STATUS );
            RockMigrationHelper.UpdateDefinedValue( SystemGuid.DefinedType.CARE_NEED_STATUS, "Closed", "", "3613946D-9788-47E1-BFD3-5FD75B59F986", false );
            RockMigrationHelper.UpdateDefinedValue( SystemGuid.DefinedType.CARE_NEED_STATUS, "Follow Up", "", "1B48E766-3B9B-4A32-AB1A-80DC0C2DCC63", false );
            RockMigrationHelper.UpdateDefinedValue( SystemGuid.DefinedType.CARE_NEED_STATUS, "Long Term Care", "", "989A3B33-8230-4167-99F6-E1C21EE6950E", false );
            RockMigrationHelper.UpdateDefinedValue( SystemGuid.DefinedType.CARE_NEED_STATUS, "Open", "", "811ECA2D-2B74-469A-9CFB-AB47B9643A02", false );

            RockMigrationHelper.AddDefinedType( "Steps to Care", "Category", "Categories used for Care Needs and Care workers to color code and assign them.", SystemGuid.DefinedType.CARE_NEED_CATEGORY );
            RockMigrationHelper.AddDefinedTypeAttribute( SystemGuid.DefinedType.CARE_NEED_CATEGORY, "D747E6AE-C383-4E22-8846-71518E3DD06F", "Color", "Color", "", 1056, "", "FF84E9A7-9BD6-4987-A942-AD4703303B12" );
            RockMigrationHelper.AddAttributeQualifier( "FF84E9A7-9BD6-4987-A942-AD4703303B12", "selectiontype", "Color Picker", "E3E0FD56-AEDF-4A44-87E2-B2E5B74A32F9" );
            RockMigrationHelper.UpdateDefinedValue( SystemGuid.DefinedType.CARE_NEED_CATEGORY, "Birth", "", "EA93B6A9-F151-475D-89B4-C7C24AE603B8", false );
            RockMigrationHelper.UpdateDefinedValue( SystemGuid.DefinedType.CARE_NEED_CATEGORY, "Grief", "", "6E092B06-BD9C-400F-9426-75CED70FF93E", false );
            RockMigrationHelper.UpdateDefinedValue( SystemGuid.DefinedType.CARE_NEED_CATEGORY, "Hospital/Surgery", "", "167B982F-1C66-4228-B867-3A64AEFE37F1", false );
            RockMigrationHelper.UpdateDefinedValue( SystemGuid.DefinedType.CARE_NEED_CATEGORY, "Milestone", "", "087FF723-7C2A-46EE-83F6-F1EF7F0F33C7", false );
            RockMigrationHelper.UpdateDefinedValue( SystemGuid.DefinedType.CARE_NEED_CATEGORY, "Other", "", "6BD7404D-90DD-4C94-9B0C-B516DEACB306", false );
            RockMigrationHelper.AddDefinedValueAttributeValue( "087FF723-7C2A-46EE-83F6-F1EF7F0F33C7", "FF84E9A7-9BD6-4987-A942-AD4703303B12", @"rgb(76,175,80)" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "167B982F-1C66-4228-B867-3A64AEFE37F1", "FF84E9A7-9BD6-4987-A942-AD4703303B12", @"rgb(244,67,54)" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "6BD7404D-90DD-4C94-9B0C-B516DEACB306", "FF84E9A7-9BD6-4987-A942-AD4703303B12", @"rgb(156,39,176)" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "6E092B06-BD9C-400F-9426-75CED70FF93E", "FF84E9A7-9BD6-4987-A942-AD4703303B12", @"#757272" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "EA93B6A9-F151-475D-89B4-C7C24AE603B8", "FF84E9A7-9BD6-4987-A942-AD4703303B12", @"rgb(255,235,59)" );
        }

        public override void Down()
        {
            RockMigrationHelper.DeleteAttribute( "FF84E9A7-9BD6-4987-A942-AD4703303B12" ); // Color
            RockMigrationHelper.DeleteDefinedValue( "087FF723-7C2A-46EE-83F6-F1EF7F0F33C7" ); // Milestone
            RockMigrationHelper.DeleteDefinedValue( "167B982F-1C66-4228-B867-3A64AEFE37F1" ); // Hospital/Surgery
            RockMigrationHelper.DeleteDefinedValue( "6BD7404D-90DD-4C94-9B0C-B516DEACB306" ); // Other
            RockMigrationHelper.DeleteDefinedValue( "6E092B06-BD9C-400F-9426-75CED70FF93E" ); // Grief
            RockMigrationHelper.DeleteDefinedValue( "EA93B6A9-F151-475D-89B4-C7C24AE603B8" ); // Birth
            RockMigrationHelper.DeleteDefinedType( SystemGuid.DefinedType.CARE_NEED_CATEGORY ); // Category

            RockMigrationHelper.DeleteDefinedValue( "811ECA2D-2B74-469A-9CFB-AB47B9643A02" ); // Open
            RockMigrationHelper.DeleteDefinedValue( "989A3B33-8230-4167-99F6-E1C21EE6950E" ); // Long Term Care
            RockMigrationHelper.DeleteDefinedValue( "1B48E766-3B9B-4A32-AB1A-80DC0C2DCC63" ); // Follow Up
            RockMigrationHelper.DeleteDefinedValue( "3613946D-9788-47E1-BFD3-5FD75B59F986" ); // Closed
            RockMigrationHelper.DeleteDefinedType( SystemGuid.DefinedType.CARE_NEED_STATUS ); // Status
        }
    }
}