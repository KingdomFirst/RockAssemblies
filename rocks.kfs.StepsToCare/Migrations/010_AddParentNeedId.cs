// <copyright>
// Copyright 2022 by Kingdom First Solutions
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

            Sql( @"
               -- Due to error of 'may cause cycles or multiple cascade paths.'
                CREATE TRIGGER SetParentNeedNull 
                   ON  _rocks_kfs_StepsToCare_CareNeed 
                   INSTEAD OF DELETE
                AS 
                BEGIN
	                SET NOCOUNT ON;
	                DECLARE @Id int;
                    SELECT @Id = Id FROM deleted;
	
	                IF EXISTS (SELECT Id FROM _rocks_kfs_StepsToCare_CareNeed WHERE ParentNeedId = @Id) 
	                BEGIN
		                UPDATE _rocks_kfs_StepsToCare_CareNeed SET ParentNeedId = null WHERE ParentNeedId = @Id;
	                END

	                DELETE _rocks_kfs_StepsToCare_CareNeed WHERE Id = @Id;
                END
            " );

            Sql( @"
                -- Fix to add Delete cascade on CareWorker/AssignedPerson constraint 
                ALTER TABLE [dbo].[_rocks_kfs_StepsToCare_AssignedPerson] DROP CONSTRAINT [FK__rocks_kfs_StepsToCare_AssignedPerson_Worker]

                ALTER TABLE [dbo].[_rocks_kfs_StepsToCare_AssignedPerson]  WITH CHECK ADD  CONSTRAINT [FK__rocks_kfs_StepsToCare_AssignedPerson_Worker] FOREIGN KEY([WorkerId])
                REFERENCES [dbo].[_rocks_kfs_StepsToCare_CareWorker] ([Id])
                ON DELETE CASCADE
            " );
        }

        public override void Down()
        {
            Sql( @"
                ALTER TABLE [dbo].[_rocks_kfs_StepsToCare_AssignedPerson] DROP CONSTRAINT [FK__rocks_kfs_StepsToCare_AssignedPerson_Worker]

                ALTER TABLE [dbo].[_rocks_kfs_StepsToCare_AssignedPerson]  WITH CHECK ADD  CONSTRAINT [FK__rocks_kfs_StepsToCare_AssignedPerson_Worker] FOREIGN KEY([WorkerId])
                REFERENCES [dbo].[_rocks_kfs_StepsToCare_CareWorker] ([Id])

                DROP TRIGGER [dbo].[SetParentNeedNull]

                ALTER TABLE [dbo].[_rocks_kfs_StepsToCare_CareNeed] DROP CONSTRAINT [FK__rocks_kfs_StepsToCare_CareNeed_ParentNeed]

                ALTER TABLE [dbo].[_rocks_kfs_StepsToCare_CareNeed] DROP COLUMN [ParentNeedId]
            " );
        }
    }
}