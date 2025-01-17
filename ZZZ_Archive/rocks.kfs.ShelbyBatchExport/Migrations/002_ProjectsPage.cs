using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rock.Plugin;

namespace rocks.kfs.ShelbyBatchExport.Migrations
{
    [MigrationNumber( 2, "1.6.1" )]
    public class ProjectsPage : Migration
    {
        /// <summary>
        /// The commands to run to migrate plugin to the specific version
        /// </summary>
        public override void Up()
        {
            // create page for project defined type
            RockMigrationHelper.AddPage( Rock.SystemGuid.Page.ADMINISTRATION_FINANCE, "D65F783D-87A9-4CC9-8110-E83466A0EADB", "Shelby GL Projects", "", "1DF1C485-565A-4D20-822A-490772CC7DCB", "fa fa-clipboard", "2B630A3B-E081-4204-A3E4-17BB3A5F063D" );

            // add defined value list block and set to projects defined type
            RockMigrationHelper.AddBlock( "1DF1C485-565A-4D20-822A-490772CC7DCB", "", "0AB2D5E9-9272-47D5-90E4-4AA838D2D3EE", "Projects", "Main", "", "", 0, "16D21428-35A5-4EEF-B156-C76A692031AE" );
            RockMigrationHelper.AddBlockAttributeValue( "16D21428-35A5-4EEF-B156-C76A692031AE", "9280D61F-C4F3-4A3E-A9BB-BCD67FF78637", "1C9E0068-6840-4551-86F0-E12691CEC063" );
        }

        /// <summary>
        /// The commands to undo a migration from a specific version
        /// </summary>
        public override void Down()
        {
            //
            // remove block and page
            //
            RockMigrationHelper.DeleteBlockAttributeValue( "16D21428-35A5-4EEF-B156-C76A692031AE", "9280D61F-C4F3-4A3E-A9BB-BCD67FF78637" );
            RockMigrationHelper.DeleteBlock( "16D21428-35A5-4EEF-B156-C76A692031AE" );
            RockMigrationHelper.DeletePage( "1DF1C485-565A-4D20-822A-490772CC7DCB" );
        }
    }
}
