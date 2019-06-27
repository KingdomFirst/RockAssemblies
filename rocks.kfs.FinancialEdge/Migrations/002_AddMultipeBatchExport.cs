// <copyright>
// Copyright 2019 by Kingdom First Solutions
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
    [MigrationNumber( 2, "1.7.4" )]
    public class AddMultipleBatchExport : Migration
    {
        /// <summary>
        /// The commands to run to migrate plugin to the specific version
        /// </summary>
        public override void Up()
        {
            // Set FinancialBatch Date Exported attribute to display in grid
            Sql( @"
                DECLARE @BatchExportDateAttributeId INT = (SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '16EFE0B4-E607-4960-BC92-8D66854E827A')

                UPDATE [Attribute]
                SET [IsGridColumn] = 1
                WHERE [Id] = @BatchExportDateAttributeId
            " );

            // create page for FE batches export
            RockMigrationHelper.AddPage( true, Rock.SystemGuid.Page.FUNCTIONS_FINANCE, "D65F783D-87A9-4CC9-8110-E83466A0EADB", "Financial Edge Batch Export", "", "2DD5ECDF-AF5B-4CB5-B747-490E47BBDE8E", "fa fa-archive", Rock.SystemGuid.Page.BATCHES );

            // block type
            RockMigrationHelper.RenameBlockType( "~/Plugins/com_kfs/FinancialEdge/BatchesToJournal.ascx", "~/Plugins/rocks_kfs/FinancialEdge/BatchesToJournal.ascx" );
            RockMigrationHelper.UpdateBlockType( "Financial Edge Batches to Journal", "Block used to create Journal Entries in Financial Edge from multiple Rock Financial Batches.", "~/Plugins/rocks_kfs/FinancialEdge/BatchesToJournal.ascx", "KFS > Financial Edge", "880C5926-CC80-41E6-BC0C-CE680019A9C4" );
            RockMigrationHelper.AddBlockTypeAttribute( "880C5926-CC80-41E6-BC0C-CE680019A9C4", Rock.SystemGuid.FieldType.PAGE_REFERENCE, "Detail Page", "DetailPage", "", "", 0, Rock.SystemGuid.Page.FINANCIAL_BATCH_DETAIL, "85F6ED9C-1972-42CF-9402-9E94CE15F3B8", true );

            // block on the FE batch export page
            RockMigrationHelper.AddBlock( true, "2DD5ECDF-AF5B-4CB5-B747-490E47BBDE8E", "", "880C5926-CC80-41E6-BC0C-CE680019A9C4", "Financial Edge Batches To Journal", "Main", "", "", 0, "824100EF-3C0D-4842-8494-EB02F2CB4859" );
        }

        /// <summary>
        /// The commands to undo a migration from a specific version
        /// </summary>
        public override void Down()
        {
            //
            // remove fe batch export page and block
            //
            RockMigrationHelper.DeleteBlock( "824100EF-3C0D-4842-8494-EB02F2CB4859" );
            RockMigrationHelper.DeletePage( "2DD5ECDF-AF5B-4CB5-B747-490E47BBDE8E" );
        }
    }
}
