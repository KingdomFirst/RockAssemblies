// <copyright>
// Copyright 2020 by Kingdom First Solutions
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

namespace rocks.kfs.Eventbrite.Migrations
{
    [MigrationNumber( 2, "1.9.0" )]
    public partial class PageAndBlocks : Migration
    {
        /// <summary>
        /// Operations to be performed during the upgrade process.
        /// </summary>
        public override void Up()
        {
            RockMigrationHelper.AddPage( true, "5B6DBC42-8B03-4D15-8D92-AAFA28FD8616", "D65F783D-87A9-4CC9-8110-E83466A0EADB", "Eventbrite Settings", "", "B23A7712-54FB-4BA1-BBE7-F0B6077166FD", "fas fa-calendar-alt" ); // Site:Rock RMS

            RockMigrationHelper.UpdateBlockType( "Eventbrite Settings", "Allows you to configure any necessary system settings for Eventbrite integration", "~/Plugins/rocks_kfs/Eventbrite/EventbriteSettings.ascx", "KFS > Eventbrite", "7B62C3FA-8AA9-4DFE-B9A9-F7CA86AE99C2" );
            RockMigrationHelper.UpdateBlockType( "Eventbrite Sync Button", "Allows a sync button to be placed on any group aware page to be able to sync this group with Eventbrite.", "~/Plugins/rocks_kfs/Eventbrite/EventbriteSync.ascx", "KFS > Eventbrite", "4B8AD808-1378-456D-A568-A9C844B2151D" );

            RockMigrationHelper.AddBlock( true, "4E237286-B715-4109-A578-C1445EC02707".AsGuid(), null, "C2D29296-6A87-47A9-A753-EE4E9159C4C4".AsGuid(), "4B8AD808-1378-456D-A568-A9C844B2151D".AsGuid(), "Eventbrite Sync", "Feature", @"", @"", 0, "C0EB93B7-3E8B-4750-A47B-A4DB47E35F8E" );
            RockMigrationHelper.AddBlock( true, "B23A7712-54FB-4BA1-BBE7-F0B6077166FD".AsGuid(), null, "C2D29296-6A87-47A9-A753-EE4E9159C4C4".AsGuid(), "7B62C3FA-8AA9-4DFE-B9A9-F7CA86AE99C2".AsGuid(), "Eventbrite Settings", "Main", @"", @"", 0, "9EEC9E8B-6153-4313-9250-6A8AF0D348A0" );

            RockMigrationHelper.AddOrUpdateBlockTypeAttribute( "7B62C3FA-8AA9-4DFE-B9A9-F7CA86AE99C2", "BD53F9C9-EBA9-4D3F-82EA-DE5DD34A8108", "Group Detail", "GroupDetail", "Group Detail", @"", 0, @"", "4AD0CADF-9733-4CE1-B405-0B81AE7426EB" );
            RockMigrationHelper.AddOrUpdateBlockTypeAttribute( "7B62C3FA-8AA9-4DFE-B9A9-F7CA86AE99C2", "18E29E23-B43B-4CF7-AE41-C85672C09F50", "New Group Type", "NewGroupType", "New Group Type", @"Group type to be used when creating new groups", 0, @"", "1A2A8938-FB9D-4434-8135-3535EA2A016D" );
            RockMigrationHelper.AddOrUpdateBlockTypeAttribute( "7B62C3FA-8AA9-4DFE-B9A9-F7CA86AE99C2", "F4399CEF-827B-48B2-A735-F7806FCFE8E8", "New Group Parent", "NewGroupParent", "New Group Parent", @"Where new groups, if created, will be placed under.", 0, @"", "10164E92-1898-4ABC-928F-8E5728BE4B9C" );

            RockMigrationHelper.AddBlockAttributeValue( "9EEC9E8B-6153-4313-9250-6A8AF0D348A0", "4AD0CADF-9733-4CE1-B405-0B81AE7426EB", @"4e237286-b715-4109-a578-c1445ec02707,2bc75af5-44ad-4ba3-90d3-15d936f722e8" );
        }

        /// <summary>
        /// Operations to be performed during the downgrade process.
        /// </summary>
        public override void Down()
        {
            RockMigrationHelper.DeleteBlockAttributeValue( "9EEC9E8B-6153-4313-9250-6A8AF0D348A0", "4AD0CADF-9733-4CE1-B405-0B81AE7426EB" );
            RockMigrationHelper.DeleteBlockAttribute( "10164E92-1898-4ABC-928F-8E5728BE4B9C" );
            RockMigrationHelper.DeleteBlockAttribute( "1A2A8938-FB9D-4434-8135-3535EA2A016D" );
            RockMigrationHelper.DeleteBlockAttribute( "4AD0CADF-9733-4CE1-B405-0B81AE7426EB" );
            RockMigrationHelper.DeleteBlock( "9EEC9E8B-6153-4313-9250-6A8AF0D348A0" );
            RockMigrationHelper.DeleteBlock( "C0EB93B7-3E8B-4750-A47B-A4DB47E35F8E" );
            RockMigrationHelper.DeleteBlockType( "4B8AD808-1378-456D-A568-A9C844B2151D" );
            RockMigrationHelper.DeleteBlockType( "7B62C3FA-8AA9-4DFE-B9A9-F7CA86AE99C2" );
            RockMigrationHelper.DeletePage( "B23A7712-54FB-4BA1-BBE7-F0B6077166FD" );
        }
    }
}