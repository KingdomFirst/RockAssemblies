using System;
using System.Data.Entity;
using System.Linq;
using Rock;
using Rock.Data;
using Rock.Model;
using Rock.Plugin;

namespace rocks.kfs.RoomManagement.Migrations
{
    [MigrationNumber( 1, "1.8.0" )]
    public class RegisterReports : Migration
    {
        public override void Up()
        {
            // Report Templates
            RockMigrationHelper.UpdateEntityType( "rocks.kfs.RoomManagement.ReportTemplates.LavaLandscapeReportTemplate", "KFS - Lava Landscape Template", "rocks.kfs.RoomManagement.ReportTemplates.LavaLandscapeReportTemplate, rocks.kfs.RoomManagement.ReportTemplates, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", false, true, "1676d165-61e9-4a3f-abd6-01904afa2ceb" );
            RockMigrationHelper.UpdateEntityType( "rocks.kfs.RoomManagement.ReportTemplates.SPACReportTemplate", "KFS - SPAC Template", "rocks.kfs.RoomManagement.ReportTemplates.SPACReportTemplate, rocks.kfs.RoomManagement.ReportTemplates, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", false, true, "56288405-fb95-41c2-a2e4-775265c15f19" );
            RockMigrationHelper.UpdateEntityType( "rocks.kfs.RoomManagement.ReportTemplates.ResourceQuestionReportTemplate", "KFS - Resource Question Template", "rocks.kfs.RoomManagement.ReportTemplates.ResourceQuestionReportTemplate, rocks.kfs.RoomManagement.ReportTemplates, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", false, true, "2b2aff9a-c454-415c-ba68-01a4f18559ca" );

            RockMigrationHelper.AddEntityAttribute( "rocks.kfs.RoomManagement.ReportTemplates.LavaLandscapeReportTemplate", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "", "", "Active", "", "Should Service be used?", 0, "True", "5914141D-0DA5-47E1-B86D-697A2F09AD8F" );
            RockMigrationHelper.AddAttributeValue( "5914141D-0DA5-47E1-B86D-697A2F09AD8F", 0, "True", "5D1C0EA8-67B6-45EE-ACAB-4A58AD0587D6" );

            RockMigrationHelper.AddEntityAttribute( "rocks.kfs.RoomManagement.ReportTemplates.SPACReportTemplate", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "", "", "Active", "", "Should Service be used?", 0, "True", "5D02961E-3320-4087-9CBB-1501DDEDBC80" );
            RockMigrationHelper.AddAttributeValue( "5D02961E-3320-4087-9CBB-1501DDEDBC80", 0, "True", "D70BF65C-CABC-45C7-9FAF-DF310C57FBFE" );

            RockMigrationHelper.AddEntityAttribute( "rocks.kfs.RoomManagement.ReportTemplates.ResourceQuestionReportTemplate", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "", "", "Active", "", "Should Service be used?", 0, "True", "CC6AB638-EDFD-4901-B73B-69D1CA29120C" );
            RockMigrationHelper.AddAttributeValue( "CC6AB638-EDFD-4901-B73B-69D1CA29120C", 0, "True", "D4FFBF1D-B7C0-4187-A654-3F56109083D8" );
        }

        public override void Down()
        {
            // Needs down migration?
        }

    }
}