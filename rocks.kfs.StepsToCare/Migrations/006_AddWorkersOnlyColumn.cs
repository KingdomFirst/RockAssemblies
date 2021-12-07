// <copyright>
// Copyright 2021 by Kingdom First Solutions
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

namespace rocks.kfs.StepsToCare
{
    [MigrationNumber( 6, "1.12.3" )]
    public class AddWorkersOnlyColumn : Migration
    {
        public override void Up()
        {
            Sql( @"
                ALTER TABLE [dbo].[_rocks_kfs_StepsToCare_CareNeed] ADD [WorkersOnly] BIT NOT NULL
                    CONSTRAINT DF__rocks_kfs_StepsToCare_CareNeed_WorkersOnly
                    DEFAULT (0)
            " );
        }

        public override void Down()
        {
            Sql( @"
                ALTER TABLE [dbo].[_rocks_kfs_StepsToCare_CareNeed] DROP CONSTRAINT [DF__rocks_kfs_StepsToCare_CareNeed_WorkersOnly]

                ALTER TABLE [dbo].[_rocks_kfs_StepsToCare_CareNeed] DROP COLUMN [WorkersOnly]
            " );
        }
    }
}