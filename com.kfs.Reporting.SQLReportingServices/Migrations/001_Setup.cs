using Rock.Plugin;

namespace com.kfs.Reporting.SQLReportingServices.Migrations
{
    [MigrationNumber( 1, "1.6.5" )]
    public class Setup : Migration
    {
        /// <summary>
        /// Adds the files for this plugins
        /// </summary>
        public override void Up()
        {
            RockMigrationHelper.AddPage( "5B6DBC42-8B03-4D15-8D92-AAFA28FD8616", "D65F783D-87A9-4CC9-8110-E83466A0EADB", "SQL Reporting Services", "", "1254EEFC-286D-44D7-B219-397368874AA9", "" ); // Site:Rock RMS
            RockMigrationHelper.AddPage( "BB0ACD18-24FB-42BA-B89A-2FFD80472F5B", "0CB60906-6B74-44FD-AB25-026050EF70EB", "Reporting Services", "", "64E41E3B-F49F-405A-9D7A-942F666B76A7", "" ); // Site:Rock RMS
            RockMigrationHelper.AddPage( "64E41E3B-F49F-405A-9D7A-942F666B76A7", "D65F783D-87A9-4CC9-8110-E83466A0EADB", "Report Viewer", "", "FEC43015-F506-46EA-912C-D8038B838A6F", "" ); // Site:Rock RMS
            RockMigrationHelper.UpdateBlockType( "Reporting Services Configuration", "SQL Server Reporting Services Setup and Configuration.", "~/Plugins/com_kfs/Reporting/ReportingServicesConfiguration.ascx", "KFS > Reporting", "6E2E032E-F6CC-4BC5-8B24-514AB7839813" );
            RockMigrationHelper.UpdateBlockType( "Reporting Services Tree", "SQL Server Reporting Services Tree View", "~/Plugins/com_kfs/Reporting/ReportingServicesFolderTree.ascx", "KFS > Reporting", "395808FF-05C9-43F8-B3B4-B65479E7BAF8" );
            RockMigrationHelper.UpdateBlockType( "Reporting Services PDF Viewer", "", "~/Plugins/com_kfs/Reporting/ReportingServicesPDFViewer.ascx", "KFS > Reporting", "14AD062A-9EF9-4661-B372-6A3DC8D2EC03" );
            RockMigrationHelper.UpdateBlockType( "Reporting Services Viewer", "", "~/Plugins/com_kfs/Reporting/ReportingServicesViewer.ascx", "KFS > Reporting", "8F3D25D7-6016-40A4-ADB7-AFA024FDF393" );

            Sql( "UPDATE [Page] SET [IconCSSClass] = 'fa fa-area-chart' WHERE [Guid] = '1254EEFC-286D-44D7-B219-397368874AA9'" );

            // Add Block to Page: Report Services, Site: Rock RMS
            RockMigrationHelper.AddBlock( "1254EEFC-286D-44D7-B219-397368874AA9", "", "6E2E032E-F6CC-4BC5-8B24-514AB7839813", "Reporting Services Configuration", "Main", "", "", 0, "18CD63D1-8FA8-4D34-81BE-37AA2DBA4767" );
            // Add Block to Page: Reporting Services, Site: Rock RMS
            RockMigrationHelper.AddBlock( "64E41E3B-F49F-405A-9D7A-942F666B76A7", "", "395808FF-05C9-43F8-B3B4-B65479E7BAF8", "Reporting Services Tree", "Sidebar1", "", "", 0, "60F24AEB-38C8-4430-932F-8F1A1F6D472E" );
            // Add Block to Page: Report Viewer, Site: Rock RMS
            RockMigrationHelper.AddBlock( "FEC43015-F506-46EA-912C-D8038B838A6F", "", "8F3D25D7-6016-40A4-ADB7-AFA024FDF393", "Reporting Services Viewer", "Main", "", "", 0, "4E991DE5-D54A-4F14-85B5-5E3CC4B05F5A" );
            // Add Block to Page: Reporting Services, Site: Rock RMS
            RockMigrationHelper.AddBlock( "64E41E3B-F49F-405A-9D7A-942F666B76A7", "", "8F3D25D7-6016-40A4-ADB7-AFA024FDF393", "Reporting Services Viewer", "Main", "", "", 0, "0FAC6AF1-FE73-4678-A9BC-DB8F67118887" );
            // Attrib for BlockType: Reporting Services Configuration:Use Separate Content Manager User
            RockMigrationHelper.AddBlockTypeAttribute( "6E2E032E-F6CC-4BC5-8B24-514AB7839813", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Use Separate Content Manager User", "UseCMUser", "", "Use separate Content Manager user and Browser user.", 0, @"False", "B33BA0F2-55E6-4495-84D8-CF748B17BC58" );
            // Attrib for BlockType: Reporting Services Tree:Report Viewer Page
            RockMigrationHelper.AddBlockTypeAttribute( "395808FF-05C9-43F8-B3B4-B65479E7BAF8", "BD53F9C9-EBA9-4D3F-82EA-DE5DD34A8108", "Report Viewer Page", "ReportViewerPage", "", "The page that contains the Reporting Services Report Viewer. If populated all report nodes will be clickable.", 4, @"", "73152A1E-258C-4E5B-81C4-487AD8F0C1DD" );
            // Attrib for BlockType: Reporting Services Tree:Standalone Mode
            RockMigrationHelper.AddBlockTypeAttribute( "395808FF-05C9-43F8-B3B4-B65479E7BAF8", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Standalone Mode", "StandaloneMode", "", "A flag indicating if this block is on a shared page with a report viewer or if it is on it's own page.", 4, @"False", "AD4C4AF1-A396-4AAF-8D6C-3D3336228D1F" );
            // Attrib for BlockType: Reporting Services Tree:Selection Mode
            RockMigrationHelper.AddBlockTypeAttribute( "395808FF-05C9-43F8-B3B4-B65479E7BAF8", "7525C4CB-EE6B-41D4-9B64-A08048D5A5C0", "Selection Mode", "SelectionMode", "", "Reporting Services Tree selection mode.", 2, @"Report", "737F8011-31E1-436F-9D1F-8182B65944E1" );
            // Attrib for BlockType: Reporting Services Tree:Header Text
            RockMigrationHelper.AddBlockTypeAttribute( "395808FF-05C9-43F8-B3B4-B65479E7BAF8", "9C204CD0-1233-41C5-818A-C5DA439445AA", "Header Text", "HeaderText", "", "The text to be displayed in the header when in Standalone Mode", 0, @"", "543260CF-D489-4C4E-9DCC-510E46828B3F" );
            // Attrib for BlockType: Reporting Services Tree:Show Child Items
            RockMigrationHelper.AddBlockTypeAttribute( "395808FF-05C9-43F8-B3B4-B65479E7BAF8", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Show Child Items", "ShowChildItems", "", "Determines if child items should be displayed. Default is true", 0, @"True", "8CFBD6B0-FC0A-46AC-BD4C-FB8B0820AA0F" );
            // Attrib for BlockType: Reporting Services Tree:Show Hidden Items
            RockMigrationHelper.AddBlockTypeAttribute( "395808FF-05C9-43F8-B3B4-B65479E7BAF8", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Show Hidden Items", "ShowHiddenItems", "", "Determines if hidden items should be displayed. Default is false.", 3, @"False", "B1C319AD-0849-4252-B1AC-911CF6F6E14B" );
            // Attrib for BlockType: Reporting Services Tree:Root Folder
            RockMigrationHelper.AddBlockTypeAttribute( "395808FF-05C9-43F8-B3B4-B65479E7BAF8", "9C204CD0-1233-41C5-818A-C5DA439445AA", "Root Folder", "RootFolder", "", "Root/Base Folder", 2, @"/", "0DB0C38B-0B0D-4274-9D84-0B0BEA00B6CE" );
            // Attrib for BlockType: Reporting Services Viewer:Report Path
            RockMigrationHelper.AddBlockTypeAttribute( "8F3D25D7-6016-40A4-ADB7-AFA024FDF393", "9C204CD0-1233-41C5-818A-C5DA439445AA", "Report Path", "ReportPath", "", "Relative Path to Reporting Services Report. Used in single report mode, and will overide ReportPath page parameter.", 0, @"", "093F1648-636F-4296-8E43-E13F3D533147" );
            // Attrib for BlockType: Reporting Services Viewer:Report Parameters
            RockMigrationHelper.AddBlockTypeAttribute( "8F3D25D7-6016-40A4-ADB7-AFA024FDF393", "73B02051-0D38-4AD9-BF81-A2D477DE4F70", "Report Parameters", "ReportParameters", "", "Report Parameters.", 1, @"", "909DCC55-4DB3-4A4E-B329-56356C699113" );
            // Attrib for BlockType: Reporting Services PDF Viewer:Report Parameters
            RockMigrationHelper.AddBlockTypeAttribute( "14AD062A-9EF9-4661-B372-6A3DC8D2EC03", "73B02051-0D38-4AD9-BF81-A2D477DE4F70", "Report Parameters", "ReportParameters", "", "Report Parameters.", 1, @"", "F0C9AA75-4F97-4471-9D8A-61B3E48A51EA" );
            // Attrib for BlockType: Reporting Services PDF Viewer:Show PDF Viewer
            RockMigrationHelper.AddBlockTypeAttribute( "14AD062A-9EF9-4661-B372-6A3DC8D2EC03", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Show PDF Viewer", "ShowReportViewer", "", "A flag that determines if the full PDF Viewer block should be rendered or only return the report pdf. Default is true.", 0, @"True", "E46B9C0F-2C62-4781-BEB2-1033AD3D8A79" );
            // Attrib for BlockType: Reporting Services PDF Viewer:Report Path
            RockMigrationHelper.AddBlockTypeAttribute( "14AD062A-9EF9-4661-B372-6A3DC8D2EC03", "9C204CD0-1233-41C5-818A-C5DA439445AA", "Report Path", "ReportPath", "", "Relative Path to Reporting Services Report. Used in single report mode, and will overide ReportPath page parameter.", 0, @"", "ABF47B67-3B40-4A9F-937D-90D53CD53307" );
            // Attrib Value for Block:Reporting Services Tree, Attribute:Report Viewer Page Page: Reporting Services, Site: Rock RMS
            RockMigrationHelper.AddBlockAttributeValue( "60F24AEB-38C8-4430-932F-8F1A1F6D472E", "73152A1E-258C-4E5B-81C4-487AD8F0C1DD", @"64e41e3b-f49f-405a-9d7a-942f666b76a7" );
            // Attrib Value for Block:Reporting Services Tree, Attribute:Standalone Mode Page: Reporting Services, Site: Rock RMS
            RockMigrationHelper.AddBlockAttributeValue( "60F24AEB-38C8-4430-932F-8F1A1F6D472E", "AD4C4AF1-A396-4AAF-8D6C-3D3336228D1F", @"False" );
            // Attrib Value for Block:Reporting Services Tree, Attribute:Selection Mode Page: Reporting Services, Site: Rock RMS
            RockMigrationHelper.AddBlockAttributeValue( "60F24AEB-38C8-4430-932F-8F1A1F6D472E", "737F8011-31E1-436F-9D1F-8182B65944E1", @"Report" );
            // Attrib Value for Block:Reporting Services Tree, Attribute:Header Text Page: Reporting Services, Site: Rock RMS
            RockMigrationHelper.AddBlockAttributeValue( "60F24AEB-38C8-4430-932F-8F1A1F6D472E", "543260CF-D489-4C4E-9DCC-510E46828B3F", @"" );
            // Attrib Value for Block:Reporting Services Tree, Attribute:Show Child Items Page: Reporting Services, Site: Rock RMS
            RockMigrationHelper.AddBlockAttributeValue( "60F24AEB-38C8-4430-932F-8F1A1F6D472E", "8CFBD6B0-FC0A-46AC-BD4C-FB8B0820AA0F", @"True" );
            // Attrib Value for Block:Reporting Services Tree, Attribute:Show Hidden Items Page: Reporting Services, Site: Rock RMS
            RockMigrationHelper.AddBlockAttributeValue( "60F24AEB-38C8-4430-932F-8F1A1F6D472E", "B1C319AD-0849-4252-B1AC-911CF6F6E14B", @"False" );
            // Attrib Value for Block:Reporting Services Tree, Attribute:Root Folder Page: Reporting Services, Site: Rock RMS
            RockMigrationHelper.AddBlockAttributeValue( "60F24AEB-38C8-4430-932F-8F1A1F6D472E", "0DB0C38B-0B0D-4274-9D84-0B0BEA00B6CE", @"/Rock" );
        }

        /// <summary>
        /// Removes the files for this plugin.
        /// </summary>
        public override void Down()
        {
            // Attrib for BlockType: Reporting Services PDF Viewer:Report Path
            RockMigrationHelper.DeleteAttribute( "ABF47B67-3B40-4A9F-937D-90D53CD53307" );
            // Attrib for BlockType: Reporting Services PDF Viewer:Show PDF Viewer
            RockMigrationHelper.DeleteAttribute( "E46B9C0F-2C62-4781-BEB2-1033AD3D8A79" );
            // Attrib for BlockType: Reporting Services PDF Viewer:Report Parameters
            RockMigrationHelper.DeleteAttribute( "F0C9AA75-4F97-4471-9D8A-61B3E48A51EA" );
            // Attrib for BlockType: Reporting Services Viewer:Report Parameters
            RockMigrationHelper.DeleteAttribute( "909DCC55-4DB3-4A4E-B329-56356C699113" );
            // Attrib for BlockType: Reporting Services Viewer:Report Path
            RockMigrationHelper.DeleteAttribute( "093F1648-636F-4296-8E43-E13F3D533147" );
            // Attrib for BlockType: Reporting Services Tree:Root Folder
            RockMigrationHelper.DeleteAttribute( "0DB0C38B-0B0D-4274-9D84-0B0BEA00B6CE" );
            // Attrib for BlockType: Reporting Services Tree:Show Hidden Items
            RockMigrationHelper.DeleteAttribute( "B1C319AD-0849-4252-B1AC-911CF6F6E14B" );
            // Attrib for BlockType: Reporting Services Tree:Show Child Items
            RockMigrationHelper.DeleteAttribute( "8CFBD6B0-FC0A-46AC-BD4C-FB8B0820AA0F" );
            // Attrib for BlockType: Reporting Services Tree:Header Text
            RockMigrationHelper.DeleteAttribute( "543260CF-D489-4C4E-9DCC-510E46828B3F" );
            // Attrib for BlockType: Reporting Services Tree:Selection Mode
            RockMigrationHelper.DeleteAttribute( "737F8011-31E1-436F-9D1F-8182B65944E1" );
            // Attrib for BlockType: Reporting Services Tree:Standalone Mode
            RockMigrationHelper.DeleteAttribute( "AD4C4AF1-A396-4AAF-8D6C-3D3336228D1F" );
            // Attrib for BlockType: Reporting Services Tree:Report Viewer Page
            RockMigrationHelper.DeleteAttribute( "73152A1E-258C-4E5B-81C4-487AD8F0C1DD" );
            // Attrib for BlockType: Reporting Services Configuration:Use Separate Content Manager User
            RockMigrationHelper.DeleteAttribute( "B33BA0F2-55E6-4495-84D8-CF748B17BC58" );
            // Attrib for BlockType: HTML Content:Enabled Lava Commands
            RockMigrationHelper.DeleteAttribute( "4ED8715A-5378-4BC2-B297-1B181C28F1D9" );
            // Attrib for BlockType: Login:New Account Text
            RockMigrationHelper.DeleteAttribute( "C125D182-F97B-4020-9AF3-1F6BD8884E88" );
            // Remove Block: Reporting Services Viewer, from Page: Reporting Services, Site: Rock RMS
            RockMigrationHelper.DeleteBlock( "0FAC6AF1-FE73-4678-A9BC-DB8F67118887" );
            // Remove Block: Reporting Services Viewer, from Page: Report Viewer, Site: Rock RMS
            RockMigrationHelper.DeleteBlock( "4E991DE5-D54A-4F14-85B5-5E3CC4B05F5A" );
            // Remove Block: Reporting Services Tree, from Page: Reporting Services, Site: Rock RMS
            RockMigrationHelper.DeleteBlock( "60F24AEB-38C8-4430-932F-8F1A1F6D472E" );
            // Remove Block: Reporting Services Configuration, from Page: Report Services, Site: Rock RMS
            RockMigrationHelper.DeleteBlock( "18CD63D1-8FA8-4D34-81BE-37AA2DBA4767" );
            RockMigrationHelper.DeleteBlockType( "8F3D25D7-6016-40A4-ADB7-AFA024FDF393" ); // Reporting Services Viewer
            RockMigrationHelper.DeleteBlockType( "14AD062A-9EF9-4661-B372-6A3DC8D2EC03" ); // Reporting Services PDF Viewer
            RockMigrationHelper.DeleteBlockType( "395808FF-05C9-43F8-B3B4-B65479E7BAF8" ); // Reporting Services Tree
            RockMigrationHelper.DeleteBlockType( "6E2E032E-F6CC-4BC5-8B24-514AB7839813" ); // Reporting Services Configuration
            RockMigrationHelper.DeletePage( "FEC43015-F506-46EA-912C-D8038B838A6F" ); //  Page: Report Viewer, Layout: Full Width, Site: Rock RMS
            RockMigrationHelper.DeletePage( "64E41E3B-F49F-405A-9D7A-942F666B76A7" ); //  Page: Reporting Services, Layout: Left Sidebar, Site: Rock RMS
            RockMigrationHelper.DeletePage( "1254EEFC-286D-44D7-B219-397368874AA9" ); //  Page: Report Services, Layout: Full Width, Site: Rock RMS
        }
    }
}
