using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rock.Plugin;

namespace com.kfs.Checkr.Migrations
{
    [MigrationNumber( 2, "1.6.9" )]
    public class AddBasicCriminalPackage : Migration
    {
        /// <summary>
        /// The commands to run to migrate plugin to the specific version
        /// </summary>
        public override void Up()
        {
            RockMigrationHelper.AddDefinedValue( SystemGuid.DefinedType.CHECKR_PACKAGES, "basic_criminal", "Basic Criminal", "c8b6a461-5062-45ff-88c0-5f87663fe9db", false );
        }

        /// <summary>
        /// The commands to undo a migration from a specific version
        /// </summary>
        public override void Down()
        {
        }
    }
}
