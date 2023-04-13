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
using System;
using Rock.Plugin;

namespace rocks.kfs.Intacct.Migrations
{
    [MigrationNumber( 3, "1.11.0" )]
    public class AddBatchesToIntacct : Migration
    {
        /// <summary>
        /// The commands to run to migrate plugin to the specific version
        /// </summary>
        public override void Up()
        {
            // Create new block type
            RockMigrationHelper.UpdateBlockType( "Intacct Batches to Journal", "Block used to create Journal Entries or Other Receipts in Intacct from multiple Rock Financial Batches.", "~/Plugins/rocks_kfs/Intacct/BatchesToJournal.ascx", "KFS > Intacct", "D01834F6-3405-4060-94C5-0D2D982214C1" );
            RockMigrationHelper.AddBlockTypeAttribute( "D01834F6-3405-4060-94C5-0D2D982214C1", Rock.SystemGuid.FieldType.PAGE_REFERENCE, "Detail Page", "DetailPage", "", "", 0, Rock.SystemGuid.Page.FINANCIAL_BATCH_DETAIL, "686C6889-9329-4409-9831-9D9C19B08F69", true );

            // Create new Intacct Export page
            RockMigrationHelper.AddPage( true, Rock.SystemGuid.Page.FUNCTIONS_FINANCE, "D65F783D-87A9-4CC9-8110-E83466A0EADB", "Intacct Export", "", "E775829C-36EB-45A5-895F-A4AA8589D61B", "fa fa-archive", "EF65EFF2-99AC-4081-8E09-32A04518683A" );

            // Add block on the Intacct batch export page
            RockMigrationHelper.AddBlock( "E775829C-36EB-45A5-895F-A4AA8589D61B", "", "D01834F6-3405-4060-94C5-0D2D982214C1", "Intacct Batches To Journal", "Main", "", "", 0, "509E0B55-49F7-499A-A3BA-353D4382E04F" );
        }

        /// <summary>
        /// The commands to undo a migration from a specific version
        /// </summary>
        public override void Down()
        {
            RockMigrationHelper.DeleteBlock( "509E0B55-49F7-499A-A3BA-353D4382E04F" );
            RockMigrationHelper.DeletePage( "E775829C-36EB-45A5-895F-A4AA8589D61B" );
            RockMigrationHelper.DeleteBlockType( "D01834F6-3405-4060-94C5-0D2D982214C1" );
        }
    }
}
