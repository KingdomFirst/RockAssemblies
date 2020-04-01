using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rock.Plugin;

namespace rocks.kfs.ShelbyBatchExport.Migrations
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
            RockMigrationHelper.UpdateAttributeQualifier( "D15927A2-C163-4286-AC8E-5FD081E753FA", "displaydescription", "True", "BF34ACD9-17AC-41A4-B1B0-7053F04DAC01" );

            // transaction detail project
            RockMigrationHelper.UpdateAttributeQualifier( "C02C812E-AEEE-4D22-A13C-796558CD429D", "displaydescription", "True", "D70E0ADC-06C1-4CB6-8330-85EBC7613D2A" );

            // account default project
            RockMigrationHelper.UpdateAttributeQualifier( "94E413D7-C6EB-4041-9E86-44269FFB9858", "displaydescription", "True", "F6B0784C-067B-4DF8-96E9-CA7EEE5D71D8" );

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
