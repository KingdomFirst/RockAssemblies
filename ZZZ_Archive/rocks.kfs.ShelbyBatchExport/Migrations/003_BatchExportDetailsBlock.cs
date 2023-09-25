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
            RockMigrationHelper.UpdateBlockType( "Batch Export Details", "Shows date exported and allows for quick access to Export Page", "~/Plugins/rocks_kfs/Finance/BatchExportDetails.ascx", "KFS > Finance", "73DDFCE9-F539-4397-9945-1EB17CDF711A" );
            
            // block type attribute to assign static guid
            RockMigrationHelper.UpdateBlockTypeAttribute( "73DDFCE9-F539-4397-9945-1EB17CDF711A", Rock.SystemGuid.FieldType.PAGE_REFERENCE, "Export Page", "ExportPage", "", "Page where export block is located. If not set, export shortcut will not be displayed.", 0, "", "C6340594-856D-4BA2-91D9-746A77AFBFAD" );
            
            // block on the Financial Batch Details page
            RockMigrationHelper.AddBlock( Rock.SystemGuid.Page.FINANCIAL_BATCH_DETAIL, "", "73DDFCE9-F539-4397-9945-1EB17CDF711A", "Batch Export Details", "Main", "", "", 0, "95D047FF-6CCE-4B8B-838C-3F18767901FE" );
            
            // block attribute for Shelby GL Export page
            RockMigrationHelper.AddBlockAttributeValue( "95D047FF-6CCE-4B8B-838C-3F18767901FE", "C6340594-856D-4BA2-91D9-746A77AFBFAD", "408AAC12-B303-4565-AB97-0199F9F69A1B" );
        }

        /// <summary>
        /// The commands to undo a migration from a specific version
        /// </summary>
        public override void Down()
        {
            //
            // remove block
            //
            RockMigrationHelper.DeleteBlockAttributeValue( "95D047FF-6CCE-4B8B-838C-3F18767901FE", "C6340594-856D-4BA2-91D9-746A77AFBFAD" );
            RockMigrationHelper.DeleteBlock( "95D047FF-6CCE-4B8B-838C-3F18767901FE" );
        }
    }
}
