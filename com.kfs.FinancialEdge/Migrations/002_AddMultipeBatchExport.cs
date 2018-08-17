using Rock.Plugin;

namespace com.kfs.FinancialEdge.Migrations
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
                DECLARE @BatchExportDateAttributeId INT = (SELECT TOP 1 [Id] FROM [Attribute] WHERE [Key] = 'com.kfs.FinancialEdge.DateExported')

                UPDATE [Attribute]
                SET [IsGridColumn] = 1
                WHERE [Id] = @BatchExportDateAttributeId
            " );

            // create page for project defined type
            RockMigrationHelper.AddPage( Rock.SystemGuid.Page.FUNCTIONS_FINANCE, "D65F783D-87A9-4CC9-8110-E83466A0EADB", "Financial Edge Batch Export", "", "2DD5ECDF-AF5B-4CB5-B747-490E47BBDE8E", "fa fa-archive", Rock.SystemGuid.Page.BATCHES );

            // block type
            RockMigrationHelper.UpdateBlockType( "Financial Edge Batches to Journal", "Block used to create Journal Entries in Financial Edge from multiple Rock Financial Batches.", "~/Plugins/com_kfs/FinancialEdge/BatchesToJournal.ascx", "com_kfs > Financial Edge", "880C5926-CC80-41E6-BC0C-CE680019A9C4" );
            RockMigrationHelper.AddBlockTypeAttribute( "880C5926-CC80-41E6-BC0C-CE680019A9C4", Rock.SystemGuid.FieldType.PAGE_REFERENCE, "Detail Page", "DetailPage", "", "", 0, Rock.SystemGuid.Page.FINANCIAL_BATCH_DETAIL, "85F6ED9C-1972-42CF-9402-9E94CE15F3B8", true );

            // block on the FE batch export page
            RockMigrationHelper.AddBlock( "2DD5ECDF-AF5B-4CB5-B747-490E47BBDE8E", "", "880C5926-CC80-41E6-BC0C-CE680019A9C4", "Financial Edge Batches To Journal", "Main", "", "", 0, "824100EF-3C0D-4842-8494-EB02F2CB4859" );
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