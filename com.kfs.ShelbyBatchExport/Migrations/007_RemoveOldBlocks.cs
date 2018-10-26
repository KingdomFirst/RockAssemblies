using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rock.Plugin;

namespace com.kfs.ShelbyBatchExport.Migrations
{
    [MigrationNumber( 7, "1.8.0" )]
    class RemoveOldBlocks : Migration
    {
        /// <summary>
        /// The commands to run to migrate plugin to the specific version
        /// </summary>
        public override void Up()
        {
            RockMigrationHelper.DeleteBlockType( "1F50D804-67A0-4348-8DBB-7C1B46280025" );  // ~/Plugins/com_kfs/Finance/ShelbyGLExport.ascx
            RockMigrationHelper.DeleteBlockType( "6A8AAA9A-67BD-47CA-8FF7-400BDA9FEB2E" );  // ~/Plugins/com_kfs/Finance/BatchExportDetails.ascx

            RockMigrationHelper.DeleteAttribute( "B72C0356-63AB-4E8B-8F43-40E994B94008" ); // CODE attribute for the Project DV
        }

        /// <summary>
        /// The commands to undo a migration from a specific version
        /// </summary>
        public override void Down()
        {

        }
    }
}
