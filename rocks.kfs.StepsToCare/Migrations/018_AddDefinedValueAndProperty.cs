// <copyright>
// Copyright 2023 by Kingdom First Solutions
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
using Rock;
using Rock.Plugin;

namespace rocks.kfs.StepsToCare.Migrations
{
    [MigrationNumber( 18, "1.14.0" )]
    public class AddDefinedValueAndProperty : Migration
    {
        public override void Up()
        {
            RockMigrationHelper.UpdateDefinedValue( SystemGuid.DefinedType.CARE_NEED_STATUS, "Snoozed", "", SystemGuid.DefinedValue.CARE_NEED_STATUS_SNOOZED, false );
            RockMigrationHelper.AddDefinedValueAttributeValue( SystemGuid.DefinedValue.CARE_NEED_STATUS_SNOOZED, "ACD00438-3E25-4E17-8875-742C27EC7B30", @"label label-primary" );
            Sql( @"IF COL_LENGTH('_rocks_kfs_StepsToCare_CareNeed', 'EnableRecurrence') IS NULL
                BEGIN
	                ALTER TABLE _rocks_kfs_StepsToCare_CareNeed ADD [EnableRecurrence] bit NOT NULL DEFAULT (0), [RenewPeriodDays] int NULL, [RenewMaxCount] int NULL, [RenewCurrentCount] int NULL, [SnoozeDate] [datetime] NULL
                END" );
        }

        public override void Down()
        {
            RockMigrationHelper.DeleteDefinedValue( SystemGuid.DefinedValue.CARE_NEED_STATUS_SNOOZED );
            Sql( @"IF COL_LENGTH('_rocks_kfs_StepsToCare_CareNeed', 'EnableRecurrence') IS NOT NULL
                BEGIN
	                ALTER TABLE _rocks_kfs_StepsToCare_CareNeed DROP COLUMN [EnableRecurrence], [RenewPeriodDays], [RenewMaxCount], [RenewCurrentCount], [SnoozeDate]
                END" );
        }
    }
}