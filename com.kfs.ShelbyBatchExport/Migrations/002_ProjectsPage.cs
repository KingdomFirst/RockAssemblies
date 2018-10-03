using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rock.Plugin;

namespace com.kfs.ShelbyBatchExport.Migrations
{
    [MigrationNumber( 2, "1.8.0" )]
    public class ProjectsPage : Migration
    {
        /// <summary>
        /// The commands to run to migrate plugin to the specific version
        /// </summary>
        public override void Up()
        {
            // create page for project defined type
            RockMigrationHelper.AddPage( Rock.SystemGuid.Page.ADMINISTRATION_FINANCE, "D65F783D-87A9-4CC9-8110-E83466A0EADB", "Projects", "", "01DAC445-9C4A-4469-9F39-A39549D75CBF", "fa fa-clipboard", "2B630A3B-E081-4204-A3E4-17BB3A5F063D" );

            // add defined value list block and set to projects defined type
            RockMigrationHelper.AddBlock( "01DAC445-9C4A-4469-9F39-A39549D75CBF", "", "0AB2D5E9-9272-47D5-90E4-4AA838D2D3EE", "Projects", "Main", "", "", 0, "23D1B7CE-D70C-4D81-B0D9-534A6D0542DC" );
            RockMigrationHelper.AddBlockAttributeValue( "23D1B7CE-D70C-4D81-B0D9-534A6D0542DC", "9280D61F-C4F3-4A3E-A9BB-BCD67FF78637", "2CE68D65-7EAC-4D5E-80B6-6FB903726961" );
        }

        /// <summary>
        /// The commands to undo a migration from a specific version
        /// </summary>
        public override void Down()
        {
            //
            // remove block and page
            //
            RockMigrationHelper.DeleteBlockAttributeValue( "23D1B7CE-D70C-4D81-B0D9-534A6D0542DC", "9280D61F-C4F3-4A3E-A9BB-BCD67FF78637" );
            RockMigrationHelper.DeleteBlock( "23D1B7CE-D70C-4D81-B0D9-534A6D0542DC" );
            RockMigrationHelper.DeletePage( "01DAC445-9C4A-4469-9F39-A39549D75CBF" );
        }
    }
}
