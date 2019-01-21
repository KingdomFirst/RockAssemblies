using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rock.Plugin;

namespace com.kfs.Avalanche.Migrations
{
    [MigrationNumber( 1, "1.8.0" )]
    public class CreateSiteAndLayouts : Migration
    {
        /// <summary>
        /// The commands to run to migrate plugin to the specific version
        /// </summary>
        public override void Up()
        {
            RockMigrationHelper.AddSite( "Avalanche", "This site is used for the KFS Avalanche Mobile Application", "Avalanche", "613631FF-D19C-4F9C-B163-E9331C4BA61B" );
            RockMigrationHelper.AddLayout( "613631FF-D19C-4F9C-B163-E9331C4BA61B", "Boxes", "Boxes", "", "B510AB94-04B0-48C0-BEDD-16812BEDECB1" ); // Site:Avalanche
            RockMigrationHelper.AddLayout( "613631FF-D19C-4F9C-B163-E9331C4BA61B", "Footer", "Footer", "", "60D99A36-8D00-467E-9993-3C2F0B249EBD" ); // Site:Avalanche
            RockMigrationHelper.AddLayout( "613631FF-D19C-4F9C-B163-E9331C4BA61B", "MainPage", "Main Page", "", "FC61CD1A-15DC-4FDD-9DDD-4A0BD8936E16" ); // Site:Avalanche
            RockMigrationHelper.AddLayout( "613631FF-D19C-4F9C-B163-E9331C4BA61B", "NoScroll", "No Scroll", "", "901926F9-AD81-41A4-9B1E-254F5B45E471" ); // Site:Avalanche
            RockMigrationHelper.AddLayout( "613631FF-D19C-4F9C-B163-E9331C4BA61B", "Simple", "Simple", "", "355F6C23-29B3-4976-AE43-30426BE12B99" ); // Site:Avalanche
        }

        /// <summary>
        /// The commands to undo a migration from a specific version
        /// </summary>
        public override void Down()
        {
            //
            // remove layouts and site
            //
            RockMigrationHelper.DeleteLayout( "355F6C23-29B3-4976-AE43-30426BE12B99" ); //  Layout: Simple, Site: Avalanche
            RockMigrationHelper.DeleteLayout( "901926F9-AD81-41A4-9B1E-254F5B45E471" ); //  Layout: No Scroll, Site: Avalanche
            RockMigrationHelper.DeleteLayout( "FC61CD1A-15DC-4FDD-9DDD-4A0BD8936E16" ); //  Layout: Main Page, Site: Avalanche
            RockMigrationHelper.DeleteLayout( "60D99A36-8D00-467E-9993-3C2F0B249EBD" ); //  Layout: Footer, Site: Avalanche
            RockMigrationHelper.DeleteLayout( "B510AB94-04B0-48C0-BEDD-16812BEDECB1" ); //  Layout: Boxes, Site: Avalanche
            RockMigrationHelper.DeleteSite( "613631FF-D19C-4F9C-B163-E9331C4BA61B" );
        }
    }
}
