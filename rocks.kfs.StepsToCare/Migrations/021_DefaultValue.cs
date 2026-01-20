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
using Rock.Plugin;

namespace rocks.kfs.StepsToCare.Migrations
{
    [MigrationNumber( 21, "1.14.0" )]
    public class DefaultValue : Migration
    {
        public override void Up()
        {
            Sql( @"UPDATE _rocks_kfs_StepsToCare_AssignedPerson SET FollowUpWorker = 0 WHERE FollowUpWorker IS NULL

                ALTER TABLE _rocks_kfs_StepsToCare_AssignedPerson ALTER COLUMN FollowUpWorker BIT NOT NULL

                ALTER TABLE _rocks_kfs_StepsToCare_AssignedPerson ADD CONSTRAINT
	                DF__rocks_kfs_StepsToCare_AssignedPerson_FollowUpWorker DEFAULT 0 FOR FollowUpWorker" );
        }

        public override void Down()
        {
            Sql( @"ALTER TABLE _rocks_kfs_StepsToCare_AssignedPerson DROP CONSTRAINT DF__rocks_kfs_StepsToCare_AssignedPerson_FollowUpWorker

                ALTER TABLE _rocks_kfs_StepsToCare_AssignedPerson ALTER COLUMN FollowUpWorker BIT NULL

                UPDATE _rocks_kfs_StepsToCare_AssignedPerson SET FollowUpWorker = NULL WHERE FollowUpWorker = 0" );
        }
    }
}