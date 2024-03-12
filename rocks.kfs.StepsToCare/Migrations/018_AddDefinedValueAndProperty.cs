// <copyright>
// Copyright 2024 by Kingdom First Solutions
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
            RockMigrationHelper.AddDefinedTypeAttribute( SystemGuid.DefinedType.CARE_NEED_CATEGORY, "A75DFC58-7A1B-4799-BF31-451B2BBE38FF", "Number of Times to Repeat", "TimesToRepeat", "The number of times to repeat.  Leave blank to repeat indefinitely. Only applicable if Repeat Every is set.", 6564, "", "39CF47AD-8A04-41D2-B629-D4E3822D4A1D" );
            RockMigrationHelper.AddDefinedTypeAttribute( SystemGuid.DefinedType.CARE_NEED_CATEGORY, "A75DFC58-7A1B-4799-BF31-451B2BBE38FF", "Repeat Every", "RepeatEvery", "Will repeat the notification and change to follow up status the provided number of days after the snooze. If you have a value set in this attribute, by default when this category is selected it will \"Enable Recurrence\" and set the Repeat Every value to this on the Care Need.", 6563, "", "CF59CDFF-8C98-487B-B2E7-A2CA496ACB87" );
            Sql( @"IF COL_LENGTH('_rocks_kfs_StepsToCare_CareNeed', 'CustomFollowUp') IS NULL
                BEGIN
	                ALTER TABLE _rocks_kfs_StepsToCare_CareNeed ADD [CustomFollowUp] bit NOT NULL DEFAULT (0), [RenewPeriodDays] int NULL, [RenewMaxCount] int NULL, [RenewCurrentCount] int NOT NULL DEFAULT (0), [SnoozeDate] [datetime] NULL
                END" );
        }

        public override void Down()
        {
            RockMigrationHelper.DeleteAttribute( "CF59CDFF-8C98-487B-B2E7-A2CA496ACB87" ); // Number of Times to Repeat
            RockMigrationHelper.DeleteAttribute( "39CF47AD-8A04-41D2-B629-D4E3822D4A1D" ); // Repeat Every
            RockMigrationHelper.DeleteDefinedValue( SystemGuid.DefinedValue.CARE_NEED_STATUS_SNOOZED );
            Sql( @"IF COL_LENGTH('_rocks_kfs_StepsToCare_CareNeed', 'CustomFollowUp') IS NOT NULL
                BEGIN
	                ALTER TABLE _rocks_kfs_StepsToCare_CareNeed DROP COLUMN [CustomFollowUp], [RenewPeriodDays], [RenewMaxCount], [RenewCurrentCount], [SnoozeDate]
                END" );
        }
    }
}