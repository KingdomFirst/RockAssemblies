using Rock.Data;
using Rock.Plugin;
using System;

namespace com.kfs.FTPStorageProvider.Migrations
{
    [MigrationNumber( 2, "1.6.9" )]
    public class SetPasswordQualifier : Migration
    {
        /// <summary>
        /// The commands to run to migrate plugin to the specific version
        /// </summary>
        public override void Up()
        {
            RockMigrationHelper.UpdateAttributeQualifier( "9B1425A7-6C65-4C87-AB72-AF508A4FF173", "ispassword", "True", "BCAD2656-AB88-4120-954D-0C96142309F7" );

            // Flush the static entity attributes cache
            Rock.Web.Cache.AttributeCache.FlushEntityAttributes();
        }

        /// <summary>
        /// The commands to undo a migration from a specific version
        /// </summary>
        public override void Down()
        {
        }
    }
 }