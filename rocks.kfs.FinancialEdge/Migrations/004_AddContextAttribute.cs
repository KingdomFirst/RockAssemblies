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
using Rock.Plugin;

namespace rocks.kfs.FinancialEdge.Migrations
{
    [MigrationNumber( 4, "1.13.0" )]
    public class AddContextAttribute : Migration
    {
        /// <summary>
        /// The commands to run to migrate plugin to the specific version
        /// </summary>
        public override void Up()
        {
            // Attribute for BlockType
            //   BlockType: Financial Edge Batch to Journal
            //   Category: KFS > Financial Edge
            //   Attribute: Entity Type
            RockMigrationHelper.AddOrUpdateBlockTypeAttribute( "CC716B06-4674-4CBE-9C66-E7DCB42153CB", "3549BAB6-FE1B-4333-AFC4-C5ACA01BB8EB", "Entity Type", "ContextEntityType", "Entity Type", @"The type of entity that will provide context for this block", 0, @"", "D90B23E8-3C5B-4CCA-A8F8-025A53D8931E" );

            // Add Block Attribute Value
            //   Block: Financial Edge Batch To Journal
            //   BlockType: Financial Edge Batch to Journal
            //   Category: KFS > Financial Edge
            //   Block Location: Page=Financial Batch Detail, Site=Rock RMS
            //   Attribute: Entity Type
            /*   Attribute Value: bdd09c8e-2c52-4d08-9062-be7d52d190c2 */
            //   Skip If Already Exists: true
            RockMigrationHelper.AddBlockAttributeValue( true, "EAB705EF-22AF-4A6E-9531-A094AB913DC3", "D90B23E8-3C5B-4CCA-A8F8-025A53D8931E", @"bdd09c8e-2c52-4d08-9062-be7d52d190c2" );
        }

        /// <summary>
        /// The commands to undo a migration from a specific version
        /// </summary>
        public override void Down()
        {
            RockMigrationHelper.DeleteAttribute( "D90B23E8-3C5B-4CCA-A8F8-025A53D8931E" );
        }
    }
}
