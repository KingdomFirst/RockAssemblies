using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rock.Plugin;

namespace com.kfs.ShelbyBatchExport.Migrations
{
    [MigrationNumber( 4, "1.6.9" )]
    public class TransitionFromCodeAttribute : Migration
    {
        /// <summary>
        /// The commands to run to migrate plugin to the specific version
        /// </summary>
        public override void Up()
        {
            // transaction project
            RockMigrationHelper.UpdateAttributeQualifier( "365134A6-D516-48E0-AC67-A011D5D59D99", "displaydescription", "True", "4287FEEF-4AB5-4F16-A872-546A393F2DB8" );

            // transaction detail project
            RockMigrationHelper.UpdateAttributeQualifier( "951FAFFD-0513-4E31-9271-87853469E85E", "displaydescription", "True", "A0679A5F-A76A-4408-9900-C576CC20E18F" );

            // account default project
            RockMigrationHelper.UpdateAttributeQualifier( "85422EA2-AC4E-44E5-99B9-30C131116734", "displaydescription", "True", "DA83FED2-1E06-44A7-8E40-AAECE75169D4" );

            // Clear all cached items
            Rock.Web.Cache.RockCache.ClearAllCachedItems();
        }

        /// <summary>
        /// The commands to undo a migration from a specific version
        /// </summary>
        public override void Down()
        {
        }
    }
}
