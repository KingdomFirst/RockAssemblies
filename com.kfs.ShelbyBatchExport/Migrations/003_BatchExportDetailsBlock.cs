using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rock.Plugin;

namespace com.kfs.ShelbyBatchExport.Migrations
{
    [MigrationNumber( 3, "1.6.1" )]
    public class BatchExportDetailsBlock : Migration
    {
        /// <summary>
        /// The commands to run to migrate plugin to the specific version
        /// </summary>
        public override void Up()
        {
            // block type and block
            RockMigrationHelper.UpdateBlockType( "Batch Export Details", "Shows date exported and allows for quick access to Export Page", "~/Plugins/com_kfs/Finance/BatchExportDetails.ascx", "KFS > Finance", "6A8AAA9A-67BD-47CA-8FF7-400BDA9FEB2E" );
            RockMigrationHelper.AddBlock( Rock.SystemGuid.Page.FINANCIAL_BATCH_DETAIL, "", "6A8AAA9A-67BD-47CA-8FF7-400BDA9FEB2E", "Batch Export Details", "Main", "", "", 0, "AB2F7C43-74F3-46A4-A005-F690D0A612D2" );
        }

        /// <summary>
        /// The commands to undo a migration from a specific version
        /// </summary>
        public override void Down()
        {
            //
            // remove block
            //
            RockMigrationHelper.DeleteBlock( "AB2F7C43-74F3-46A4-A005-F690D0A612D2" );
        }
    }
}
