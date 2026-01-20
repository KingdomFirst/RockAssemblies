// <copyright>
// Copyright 2022 by Kingdom First Solutions
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
using Rock;
using Rock.Plugin;

namespace rocks.kfs.Zoom.Migrations
{
    [MigrationNumber( 6, "1.13" )]
    public partial class PageAndBlocks : Migration
    {
        /// <summary>
        /// Operations to be performed during the upgrade process.
        /// </summary>
        public override void Up()
        {
            RockMigrationHelper.AddPage( true, "5B6DBC42-8B03-4D15-8D92-AAFA28FD8616", "D65F783D-87A9-4CC9-8110-E83466A0EADB", "Zoom Room Settings", "Configuration settings for Zoom Room integration.", "6653BC97-06D0-4AF4-803B-21724AF78E93", "fas fa-video" ); // Site:Rock RMS

            RockMigrationHelper.UpdateBlockType( "Zoom Settings", "Allows you to configure any necessary system settings for Zoom integration", "~/Plugins/rocks_kfs/Zoom/ZoomSettings.ascx", "KFS > Zoom", "00F5DA7A-3696-4AC5-BDBF-6835FBBEAC11" );

            // Add KFS Zoom Settings block
            RockMigrationHelper.AddBlock( true, "6653BC97-06D0-4AF4-803B-21724AF78E93".AsGuid(), null, "C2D29296-6A87-47A9-A753-EE4E9159C4C4".AsGuid(), "00F5DA7A-3696-4AC5-BDBF-6835FBBEAC11".AsGuid(), "Zoom Settings", "Main", @"", @"", 1, "6681D49B-1A90-455D-9DB5-70DF7B650B7D" );
            RockMigrationHelper.AddOrUpdateBlockTypeAttribute( "00F5DA7A-3696-4AC5-BDBF-6835FBBEAC11", Rock.SystemGuid.FieldType.BOOLEAN, "Enable Logging", "EnableLogging", "Enable Logging", @"Enable logging for Zoom sync methods from this block.", 1, @"False", "07B86320-D3E2-4BDD-853D-479C97067C75" );
            RockMigrationHelper.AddBlockAttributeValue( "6681D49B-1A90-455D-9DB5-70DF7B650B7D", "07B86320-D3E2-4BDD-853D-479C97067C75", @"False" );

            // Add Defined Type block
            RockMigrationHelper.AddBlock( true, "6653BC97-06D0-4AF4-803B-21724AF78E93".AsGuid(), null, "C2D29296-6A87-47A9-A753-EE4E9159C4C4".AsGuid(), "0AB2D5E9-9272-47D5-90E4-4AA838D2D3EE".AsGuid(), "Zoom Rooms", "Main", @"", @"", 2, "5278E423-758A-475E-9A62-B58C346949A5" ); // Core Defined Type List block
            RockMigrationHelper.AddBlockAttributeValue( "5278E423-758A-475E-9A62-B58C346949A5", "9280D61F-C4F3-4A3E-A9BB-BCD67FF78637", ZoomGuid.DefinedType.ZOOM_ROOM ); // Set Defined Type setting of block
        }

        /// <summary>
        /// Operations to be performed during the downgrade process.
        /// </summary>
        public override void Down()
        {
            // Delete Defined Type block
            RockMigrationHelper.DeleteBlockAttributeValue( "5278E423-758A-475E-9A62-B58C346949A5", "9280D61F-C4F3-4A3E-A9BB-BCD67FF78637" );
            RockMigrationHelper.DeleteBlock( "5278E423-758A-475E-9A62-B58C346949A5" );

            // Delete KFS Zoom Settings block and block type
            RockMigrationHelper.DeleteBlockAttributeValue( "6681D49B-1A90-455D-9DB5-70DF7B650B7D", "07B86320-D3E2-4BDD-853D-479C97067C75" );
            RockMigrationHelper.DeleteBlockAttribute( "07B86320-D3E2-4BDD-853D-479C97067C75" );
            RockMigrationHelper.DeleteBlock( "6681D49B-1A90-455D-9DB5-70DF7B650B7D" );
            RockMigrationHelper.DeleteBlockType( "00F5DA7A-3696-4AC5-BDBF-6835FBBEAC11" );

            // Delete page
            RockMigrationHelper.DeletePage( "6653BC97-06D0-4AF4-803B-21724AF78E93" );
        }
    }
}