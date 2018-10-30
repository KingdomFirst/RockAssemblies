using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rock.Plugin;
using Rock.SystemGuid;

namespace com.kfs.Workflow.Action.CheckIn.Migrations
{
    [MigrationNumber( 2, "1.8.0" )]
    public class AddOccurrenceClosedAttribute : Migration
    {
        /// <summary>
        /// The commands to run to migrate plugin to the specific version
        /// </summary>
        public override void Up()
        {
            RockMigrationHelper.AddEntityAttribute( "Rock.Model.AttendanceOccurrence", FieldType.BOOLEAN, "", "", "Occurrence Closed", "", "", 0, "False", "B271037B-01AD-4270-B688-63DE29022915", "com.kfs.OccurrenceClosed" );
            RockMigrationHelper.AddAttributeQualifier( "B271037B-01AD-4270-B688-63DE29022915", "falsetext", "No", "04C24C40-85AF-421E-AFDE-6BA97ED085C3" );
            RockMigrationHelper.AddAttributeQualifier( "B271037B-01AD-4270-B688-63DE29022915", "truetext", "Yes", "4B0E9B0B-45FE-4BBC-B4FB-75D459D8281A" );
        }

        /// <summary>
        /// The commands to undo a migration from a specific version
        /// </summary>
        public override void Down()
        {
            RockMigrationHelper.DeleteAttribute( "B271037B-01AD-4270-B688-63DE29022915" );
        }
    }
}
