using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rock.Plugin;

namespace rocks.kfs.ShelbyBatchExport.Migrations
{
    [MigrationNumber( 3, "1.6.1" )]
    public class BatchExportDetailsBlock : Migration
    {
        /// <summary>
        /// The commands to run to migrate plugin to the specific version
        /// </summary>
        public override void Up()
        {
            // block type
            RockMigrationHelper.UpdateBlockType( "Batch Export Details", "Shows date exported and allows for quick access to Export Page", "~/Plugins/rocks_kfs/Finance/BatchExportDetails.ascx", "KFS > Finance", "6A8AAA9A-67BD-47CA-8FF7-400BDA9FEB2E" );
            
            // block type attribute to assign static guid
            RockMigrationHelper.UpdateBlockTypeAttribute( "6A8AAA9A-67BD-47CA-8FF7-400BDA9FEB2E", Rock.SystemGuid.FieldType.PAGE_REFERENCE, "Export Page", "ExportPage", "", "Page where export block is located. If not set, export shortcut will not be displayed.", 0, "", "DD9CD395-A4F0-4110-A349-C44EDFC0258B" );
            
            // block on the Financial Batch Details page
            RockMigrationHelper.AddBlock( Rock.SystemGuid.Page.FINANCIAL_BATCH_DETAIL, "", "6A8AAA9A-67BD-47CA-8FF7-400BDA9FEB2E", "Batch Export Details", "Main", "", "", 0, "AB2F7C43-74F3-46A4-A005-F690D0A612D2" );
            
            // block attribute for Shelby GL Export page
            RockMigrationHelper.AddBlockAttributeValue( "AB2F7C43-74F3-46A4-A005-F690D0A612D2", "DD9CD395-A4F0-4110-A349-C44EDFC0258B", "3F123421-E5DE-474E-9F56-AAFCEDE115EF" );
        }

        /// <summary>
        /// The commands to undo a migration from a specific version
        /// </summary>
        public override void Down()
        {
            //
            // remove block
            //
            RockMigrationHelper.DeleteBlockAttributeValue( "AB2F7C43-74F3-46A4-A005-F690D0A612D2", "DD9CD395-A4F0-4110-A349-C44EDFC0258B" );
            RockMigrationHelper.DeleteBlock( "AB2F7C43-74F3-46A4-A005-F690D0A612D2" );
        }
    }
}
