using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rock.Plugin;
using Rock.SystemGuid;

namespace com.kfs.MinistrySafe.Migrations
{
    [MigrationNumber( 6, "1.6.9" )]
    public class AddTestResultPersonAttribute : Migration
    {
        /// <summary>
        /// The commands to run to migrate plugin to the specific version
        /// </summary>
        public override void Up()
        {
            // Person Attribute Result
            RockMigrationHelper.UpdatePersonAttribute( FieldType.BOOLEAN, SystemGuid.Category.MINISTRY_SAFE_PERSON_ATTRIBUTE_CATEGORY, "Ministry Safe Training Result", "MSTrainingResult", "", "The user's pass/fail status from the Ministry Safe training.", 3, "", SystemGuid.Attribute.MINISTRY_SAFE_PERSON_ATTRIBUTE_TEST_RESULT );
            RockMigrationHelper.AddAttributeQualifier( SystemGuid.Attribute.MINISTRY_SAFE_PERSON_ATTRIBUTE_TEST_RESULT, "truetext", "Pass", "9FCEE8FD-12C2-4602-AB32-9AC9590F2156" );
            RockMigrationHelper.AddAttributeQualifier( SystemGuid.Attribute.MINISTRY_SAFE_PERSON_ATTRIBUTE_TEST_RESULT, "falsetext", "Fail", "6F4FA3B6-8958-4DDA-AC5B-73DB8500E0C4" );
        }

        /// <summary>
        /// The commands to undo a migration from a specific version
        /// </summary>
        public override void Down()
        {
            RockMigrationHelper.DeleteAttribute( SystemGuid.Attribute.MINISTRY_SAFE_PERSON_ATTRIBUTE_TEST_RESULT );
        }
    }
}
