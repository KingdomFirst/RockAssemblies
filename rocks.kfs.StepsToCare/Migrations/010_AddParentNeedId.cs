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

namespace rocks.kfs.StepsToCare.Migrations
{
    [MigrationNumber( 10, "1.12.3" )]
    public class AddParentNeedId : Migration
    {
        public override void Up()
        {
            Sql( @"
                ALTER TABLE [dbo].[_rocks_kfs_StepsToCare_CareNeed] ADD [ParentNeedId] int NULL

                ALTER TABLE [dbo].[_rocks_kfs_StepsToCare_CareNeed]  WITH CHECK ADD  CONSTRAINT [FK__rocks_kfs_StepsToCare_CareNeed_ParentNeed] FOREIGN KEY([ParentNeedId])
                REFERENCES [dbo].[_rocks_kfs_StepsToCare_CareNeed] ([Id])

                ALTER TABLE [dbo].[_rocks_kfs_StepsToCare_CareNeed] CHECK CONSTRAINT [FK__rocks_kfs_StepsToCare_CareNeed_ParentNeed]
            " );
        }

        public override void Down()
        {
            Sql( @"
                ALTER TABLE [dbo].[_rocks_kfs_StepsToCare_CareNeed] DROP CONSTRAINT [FK__rocks_kfs_StepsToCare_CareNeed_ParentNeed]

                ALTER TABLE [dbo].[_rocks_kfs_StepsToCare_CareNeed] DROP COLUMN [ParentNeedId]
            " );
        }
    }
}