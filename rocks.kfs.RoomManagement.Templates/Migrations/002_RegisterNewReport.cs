using System;
using System.Data.Entity;
using System.Linq;
using Rock;
using Rock.Data;
using Rock.Model;
using Rock.Plugin;

namespace rocks.kfs.RoomManagement.Migrations
{
    [MigrationNumber( 2, "1.9.0" )]
    public class RegisterNewReport : Migration
    {
        public override void Up()
        {
            // Report Templates
            RockMigrationHelper.UpdateEntityType( "rocks.kfs.RoomManagement.ReportTemplates.ResourceQuestionReportTemplate", "Resource Question Template", "rocks.kfs.RoomManagement.ReportTemplates.ResourceQuestionReportTemplate, rocks.kfs.RoomManagement.ReportTemplates, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", false, true, "2b2aff9a-c454-415c-ba68-01a4f18559ca" );

            RockMigrationHelper.AddEntityAttribute( "rocks.kfs.RoomManagement.ReportTemplates.ResourceQuestionReportTemplate", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "", "", "Active", "", "Should Service be used?", 0, "True", "CC6AB638-EDFD-4901-B73B-69D1CA29120C" );
            RockMigrationHelper.AddAttributeValue( "CC6AB638-EDFD-4901-B73B-69D1CA29120C", 0, "True", "D4FFBF1D-B7C0-4187-A654-3F56109083D8" );
        }

        public override void Down()
        {
            // Needs down migration?
        }

    }
}