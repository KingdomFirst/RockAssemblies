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
using Rock.Plugin;

namespace rocks.kfs.StepsToCare.Migrations
{
    [MigrationNumber( 12, "1.12.3" )]
    public class NewBlockSettings : Migration
    {
        public override void Up()
        {
            // Attribute for BlockType
            //   BlockType: Care Entry
            //   Category: KFS > Steps To Care
            //   Attribute: Verbose Logging
            RockMigrationHelper.AddOrUpdateBlockTypeAttribute( "4F0F9ED7-9F74-4152-B27F-D9B2A458AFBE", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Verbose Logging", "VerboseLogging", "Verbose Logging", @"Enable verbose Logging to help in determining issues with adding needs or auto assigning workers. Not recommended for normal use.", 0, @"False", "549C11F0-F532-4B3D-99D8-DC79ECB36498" );

            // Attribute for BlockType
            //   BlockType: Care Entry
            //   Category: KFS > Steps To Care
            //   Attribute: Enable Family Needs
            RockMigrationHelper.AddOrUpdateBlockTypeAttribute( "4F0F9ED7-9F74-4152-B27F-D9B2A458AFBE", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Enable Family Needs", "EnableFamilyNeeds", "Enable Family Needs", @"Show a checkbox to 'Include Family' which will create duplicate Care Needs for each family member with their own workers.", 0, @"False", "19C5157F-43C8-47E6-A757-A3507FC960CB" );

            // Attribute for BlockType
            //   BlockType: Care Entry
            //   Category: KFS > Steps To Care
            //   Attribute: Load Balanced Workers assignment type
            RockMigrationHelper.AddOrUpdateBlockTypeAttribute( "4F0F9ED7-9F74-4152-B27F-D9B2A458AFBE", "7525C4CB-EE6B-41D4-9B64-A08048D5A5C0", "Load Balanced Workers assignment type", "LoadBalanceWorkersType", "Load Balanced Workers assignment type", @"How should the auto assign worker load balancing work? Default: Exclusive. ""Prioritize"", it will prioritize the workers being assigned based on campus, category and any other parameters on the worker but still assign to any worker if their workload matches. ""Exclusive"", if there are workers with matching campus, category or other parameters it will only load balance between those workers.", 0, @"Exclusive", "CBC82754-CD05-4C45-873B-EC0689882A9F" );

            // Attribute for BlockType
            //   BlockType: Care Entry
            //   Category: KFS > Steps To Care
            //   Attribute: Adults in Family Worker Assignment
            RockMigrationHelper.AddOrUpdateBlockTypeAttribute( "4F0F9ED7-9F74-4152-B27F-D9B2A458AFBE", "7525C4CB-EE6B-41D4-9B64-A08048D5A5C0", "Adults in Family Worker Assignment", "AdultFamilyWorkers", "Adults in Family Workers", @"How should workers be assigned to spouses and other adults in the family when using 'Family Needs'. Normal behavior, use the same settings as a normal Care Need (Group Leader, Geofence and load balanced), or assign to Care Workers Only (load balanced).", 0, @"Normal", "6424654F-AF58-40A0-9AC0-331ACBABD4D5" );

            // Add Block Attribute Value
            //   Block: Care Entry
            //   BlockType: Care Entry
            //   Category: KFS > Steps To Care
            //   Block Location: Page=Care Entry, Site=Rock RMS
            //   Attribute: Verbose Logging
            /*   Attribute Value: False */
            RockMigrationHelper.AddBlockAttributeValue( "F953C5EF-6504-45F9-81A8-063518B7AB61", "549C11F0-F532-4B3D-99D8-DC79ECB36498", @"False" );

            // Add Block Attribute Value
            //   Block: Care Entry
            //   BlockType: Care Entry
            //   Category: KFS > Steps To Care
            //   Block Location: Page=Care Entry, Site=Rock RMS
            //   Attribute: Enable Family Needs
            /*   Attribute Value: False */
            RockMigrationHelper.AddBlockAttributeValue( "F953C5EF-6504-45F9-81A8-063518B7AB61", "19C5157F-43C8-47E6-A757-A3507FC960CB", @"False" );

            // Add Block Attribute Value
            //   Block: Care Entry
            //   BlockType: Care Entry
            //   Category: KFS > Steps To Care
            //   Block Location: Page=Care Entry, Site=Rock RMS
            //   Attribute: Load Balanced Workers assignment type
            /*   Attribute Value: Prioritize */
            RockMigrationHelper.AddBlockAttributeValue( "F953C5EF-6504-45F9-81A8-063518B7AB61", "CBC82754-CD05-4C45-873B-EC0689882A9F", @"Exclusive" );

            // Add Block Attribute Value
            //   Block: Care Entry
            //   BlockType: Care Entry
            //   Category: KFS > Steps To Care
            //   Block Location: Page=Care Entry, Site=Rock RMS
            //   Attribute: Adults in Family Worker Assignment
            /*   Attribute Value: Normal */
            RockMigrationHelper.AddBlockAttributeValue( "F953C5EF-6504-45F9-81A8-063518B7AB61", "6424654F-AF58-40A0-9AC0-331ACBABD4D5", @"Normal" );
        }

        public override void Down()
        {
            // Attribute for BlockType
            //   BlockType: Care Entry
            //   Category: KFS > Steps To Care
            //   Attribute: Adults in Family Worker Assignment
            RockMigrationHelper.DeleteAttribute( "6424654F-AF58-40A0-9AC0-331ACBABD4D5" );

            // Attribute for BlockType
            //   BlockType: Care Entry
            //   Category: KFS > Steps To Care
            //   Attribute: Load Balanced Workers assignment type
            RockMigrationHelper.DeleteAttribute( "CBC82754-CD05-4C45-873B-EC0689882A9F" );

            // Attribute for BlockType
            //   BlockType: Care Entry
            //   Category: KFS > Steps To Care
            //   Attribute: Enable Family Needs
            RockMigrationHelper.DeleteAttribute( "19C5157F-43C8-47E6-A757-A3507FC960CB" );

            // Attribute for BlockType
            //   BlockType: Care Entry
            //   Category: KFS > Steps To Care
            //   Attribute: Verbose Logging
            RockMigrationHelper.DeleteAttribute( "549C11F0-F532-4B3D-99D8-DC79ECB36498" );
        }
    }
}