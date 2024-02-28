// <copyright>
// Copyright 2024 by Kingdom First Solutions
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
using Rock.Plugin;
using KFSConst = rocks.kfs.ShelbyFinancials.SystemGuid;

namespace rocks.kfs.ShelbyFinancials.Migrations
{
    [MigrationNumber( 5, "1.13.0" )]
    public class AddContextAttribute : Migration
    {
        /// <summary>
        /// The commands to run to migrate plugin to the specific version
        /// </summary>
        public override void Up()
        {
            // Attribute for BlockType
            //   BlockType: Shelby Financials Batch to Journal
            //   Category: KFS > Shelby Financials
            //   Attribute: Entity Type
            RockMigrationHelper.AddOrUpdateBlockTypeAttribute( "235C370C-2CD7-4289-8B68-A8617F58B22B", "3549BAB6-FE1B-4333-AFC4-C5ACA01BB8EB", "Entity Type", "ContextEntityType", "Entity Type", @"The type of entity that will provide context for this block", 0, @"", "4F7F5998-D969-4CF3-9BDD-DC8954624CB4" );

            // Add Block Attribute Value
            //   Block: Shelby Financials Batch To Journal
            //   BlockType: Shelby Financials Batch to Journal
            //   Category: KFS > Shelby Financials
            //   Block Location: Page=Financial Batch Detail, Site=Rock RMS
            //   Attribute: Entity Type
            /*   Attribute Value: bdd09c8e-2c52-4d08-9062-be7d52d190c2 */
            //   Skip If Already Exists: true
            RockMigrationHelper.AddBlockAttributeValue( true, "00FC3A61-775D-4DE5-BC19-A1556FF465EA", "4F7F5998-D969-4CF3-9BDD-DC8954624CB4", @"bdd09c8e-2c52-4d08-9062-be7d52d190c2" );

        }

        /// <summary>
        /// The commands to undo a migration from a specific version
        /// </summary>
        public override void Down()
        {
            RockMigrationHelper.DeleteAttribute( "4F7F5998-D969-4CF3-9BDD-DC8954624CB4" );

        }
    }
}
