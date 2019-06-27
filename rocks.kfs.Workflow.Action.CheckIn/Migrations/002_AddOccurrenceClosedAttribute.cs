// <copyright>
// Copyright 2019 by Kingdom First Solutions
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//
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
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.AttendanceOccurrence", FieldType.BOOLEAN, "", "", "Occurrence Closed", "", 0, "False", "B271037B-01AD-4270-B688-63DE29022915", "rocks.kfs.OccurrenceClosed" );
            RockMigrationHelper.UpdateAttributeQualifier( "B271037B-01AD-4270-B688-63DE29022915", "falsetext", "No", "04C24C40-85AF-421E-AFDE-6BA97ED085C3" );
            RockMigrationHelper.UpdateAttributeQualifier( "B271037B-01AD-4270-B688-63DE29022915", "truetext", "Yes", "4B0E9B0B-45FE-4BBC-B4FB-75D459D8281A" );
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
