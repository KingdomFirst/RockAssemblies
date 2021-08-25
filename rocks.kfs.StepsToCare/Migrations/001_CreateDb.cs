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
    [MigrationNumber( 1, "1.12.3" )]
    public class CreateDb : Migration
    {
        public override void Up()
        {
            Sql( @"
                CREATE TABLE [dbo].[_rocks_kfs_StepsToCare_CareNeed](
	                [Id] [int] IDENTITY(1,1) NOT NULL,
	                [PersonAliasId] [int] NULL,
	                [Details] [nvarchar](max) NULL,
	                [SubmitterAliasId] [int] NULL,
	                [CategoryValueId] [int] NULL,
	                [DateEntered] [datetime] NULL,
	                [FollowUpDate] [datetime] NULL,
	                [StatusValueId] [int] NULL,
	                [CampusId] [int] NULL,
	                [IsActive] [bit] NOT NULL,
	                [Guid] [uniqueidentifier] NOT NULL,
	                [CreatedDateTime] [datetime] NULL,
	                [ModifiedDateTime] [datetime] NULL,
	                [CreatedByPersonAliasId] [int] NULL,
	                [ModifiedByPersonAliasId] [int] NULL,
                    [ForeignKey] [nvarchar](50) NULL,
                    [ForeignGuid] [uniqueidentifier] NULL,
                    [ForeignId] [int] NULL,
                CONSTRAINT [PK__rocks_kfs_StepsToCare_CareNeed] PRIMARY KEY CLUSTERED
                (
	                [Id] ASC
                )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
                ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

                ALTER TABLE [dbo].[_rocks_kfs_StepsToCare_CareNeed]  WITH CHECK ADD  CONSTRAINT [FK__rocks_kfs_StepsToCare_CareNeed_CreatedPersonAlias] FOREIGN KEY([CreatedByPersonAliasId])
                REFERENCES [dbo].[PersonAlias] ([Id])

                ALTER TABLE [dbo].[_rocks_kfs_StepsToCare_CareNeed] CHECK CONSTRAINT [FK__rocks_kfs_StepsToCare_CareNeed_CreatedPersonAlias]

                ALTER TABLE [dbo].[_rocks_kfs_StepsToCare_CareNeed]  WITH CHECK ADD  CONSTRAINT [FK__rocks_kfs_StepsToCare_CareNeed_ModifiedPersonAlias] FOREIGN KEY([ModifiedByPersonAliasId])
                REFERENCES [dbo].[PersonAlias] ([Id])

                ALTER TABLE [dbo].[_rocks_kfs_StepsToCare_CareNeed] CHECK CONSTRAINT [FK__rocks_kfs_StepsToCare_CareNeed_ModifiedPersonAlias]

                ALTER TABLE [dbo].[_rocks_kfs_StepsToCare_CareNeed]  WITH CHECK ADD  CONSTRAINT [FK__rocks_kfs_StepsToCare_CareNeed_PersonAlias] FOREIGN KEY([PersonAliasId])
                REFERENCES [dbo].[PersonAlias] ([Id])

                ALTER TABLE [dbo].[_rocks_kfs_StepsToCare_CareNeed] CHECK CONSTRAINT [FK__rocks_kfs_StepsToCare_CareNeed_PersonAlias]

                ALTER TABLE [dbo].[_rocks_kfs_StepsToCare_CareNeed]  WITH CHECK ADD  CONSTRAINT [FK__rocks_kfs_StepsToCare_CareNeed_SubmitterPersonAlias] FOREIGN KEY([SubmitterAliasId])
                REFERENCES [dbo].[PersonAlias] ([Id])

                ALTER TABLE [dbo].[_rocks_kfs_StepsToCare_CareNeed] CHECK CONSTRAINT [FK__rocks_kfs_StepsToCare_CareNeed_SubmitterPersonAlias]

                ALTER TABLE [dbo].[_rocks_kfs_StepsToCare_CareNeed]  WITH CHECK ADD  CONSTRAINT [FK__rocks_kfs_StepsToCare_CareNeed_StatusDefinedValue] FOREIGN KEY([StatusValueId])
                REFERENCES [dbo].[DefinedValue] ([Id])

                ALTER TABLE [dbo].[_rocks_kfs_StepsToCare_CareNeed] CHECK CONSTRAINT [FK__rocks_kfs_StepsToCare_CareNeed_StatusDefinedValue]

                ALTER TABLE [dbo].[_rocks_kfs_StepsToCare_CareNeed]  WITH CHECK ADD  CONSTRAINT [FK__rocks_kfs_StepsToCare_CareNeed_CategoryDefinedValue] FOREIGN KEY([CategoryValueId])
                REFERENCES [dbo].[DefinedValue] ([Id])

                ALTER TABLE [dbo].[_rocks_kfs_StepsToCare_CareNeed] CHECK CONSTRAINT [FK__rocks_kfs_StepsToCare_CareNeed_CategoryDefinedValue]

                ALTER TABLE [dbo].[_rocks_kfs_StepsToCare_CareNeed]  WITH CHECK ADD  CONSTRAINT [FK__rocks_kfs_StepsToCare_CareNeed_CampusId] FOREIGN KEY([CampusId])
                REFERENCES [dbo].[Campus] ([Id])

                ALTER TABLE [dbo].[_rocks_kfs_StepsToCare_CareNeed] CHECK CONSTRAINT [FK__rocks_kfs_StepsToCare_CareNeed_CampusId]

                ALTER TABLE dbo._rocks_kfs_StepsToCare_CareNeed ADD CONSTRAINT
	                DF__rocks_kfs_StepsToCare_CareNeed_IsActive DEFAULT 1 FOR IsActive
            " );

            Sql( @"
                CREATE TABLE [dbo].[_rocks_kfs_StepsToCare_CareWorker](
	                [Id] [int] IDENTITY(1,1) NOT NULL,
	                [PersonAliasId] [int] NULL,
	                [CategoryValueId] [int] NULL,
	                [GeoFenceId] [int] NULL,
	                [CampusId] [int] NULL,
	                [IsActive] [bit] NOT NULL,
	                [Guid] [uniqueidentifier] NULL,
	                [CreatedDateTime] [datetime] NULL,
	                [ModifiedDateTime] [datetime] NULL,
	                [CreatedByPersonAliasId] [int] NULL,
	                [ModifiedByPersonAliasId] [int] NULL,
	                [ForeignKey] [nvarchar](50) NULL,
                    [ForeignGuid] [uniqueidentifier] NULL,
                    [ForeignId] [int] NULL,
                 CONSTRAINT [PK__rocks_kfs_StepsToCare_CareWorker] PRIMARY KEY CLUSTERED
                (
	                [Id] ASC
                )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
                ) ON [PRIMARY]

                ALTER TABLE [dbo].[_rocks_kfs_StepsToCare_CareWorker]  WITH CHECK ADD  CONSTRAINT [FK__rocks_kfs_StepsToCare_CareWorker_CreatedPersonAlias] FOREIGN KEY([CreatedByPersonAliasId])
                REFERENCES [dbo].[PersonAlias] ([Id])

                ALTER TABLE [dbo].[_rocks_kfs_StepsToCare_CareWorker] CHECK CONSTRAINT [FK__rocks_kfs_StepsToCare_CareWorker_CreatedPersonAlias]

                ALTER TABLE [dbo].[_rocks_kfs_StepsToCare_CareWorker]  WITH CHECK ADD  CONSTRAINT [FK__rocks_kfs_StepsToCare_CareWorker_ModifiedPersonAlias] FOREIGN KEY([ModifiedByPersonAliasId])
                REFERENCES [dbo].[PersonAlias] ([Id])

                ALTER TABLE [dbo].[_rocks_kfs_StepsToCare_CareWorker] CHECK CONSTRAINT [FK__rocks_kfs_StepsToCare_CareWorker_ModifiedPersonAlias]

                ALTER TABLE [dbo].[_rocks_kfs_StepsToCare_CareWorker]  WITH CHECK ADD  CONSTRAINT [FK__rocks_kfs_StepsToCare_CareWorker_PersonAlias] FOREIGN KEY([PersonAliasId])
                REFERENCES [dbo].[PersonAlias] ([Id])

                ALTER TABLE [dbo].[_rocks_kfs_StepsToCare_CareWorker] CHECK CONSTRAINT [FK__rocks_kfs_StepsToCare_CareWorker_PersonAlias]

                ALTER TABLE [dbo].[_rocks_kfs_StepsToCare_CareWorker]  WITH CHECK ADD  CONSTRAINT [FK__rocks_kfs_StepsToCare_CareWorker_DefinedValue] FOREIGN KEY([CategoryValueId])
                REFERENCES [dbo].[DefinedValue] ([Id])

                ALTER TABLE [dbo].[_rocks_kfs_StepsToCare_CareWorker] CHECK CONSTRAINT [FK__rocks_kfs_StepsToCare_CareWorker_DefinedValue]

                ALTER TABLE [dbo].[_rocks_kfs_StepsToCare_CareWorker]  WITH CHECK ADD  CONSTRAINT [FK__rocks_kfs_StepsToCare_CareWorker_CampusId] FOREIGN KEY([CampusId])
                REFERENCES [dbo].[Campus] ([Id])

                ALTER TABLE [dbo].[_rocks_kfs_StepsToCare_CareWorker] CHECK CONSTRAINT [FK__rocks_kfs_StepsToCare_CareWorker_CampusId]

                ALTER TABLE dbo._rocks_kfs_StepsToCare_CareWorker ADD CONSTRAINT
	                DF__rocks_kfs_StepsToCare_CareWorker_IsActive DEFAULT 1 FOR IsActive
            " );

            Sql( @"
                CREATE TABLE [dbo].[_rocks_kfs_StepsToCare_NoteTemplate](
	                [Id] [int] IDENTITY(1,1) NOT NULL,
	                [Icon] [nvarchar](250) NULL,
	                [Note] [nvarchar](max) NULL,
                	[Order] [int] NOT NULL,
	                [IsActive] [bit] NOT NULL,
	                [IsSystem] [bit] NOT NULL,
	                [Guid] [uniqueidentifier] NULL,
	                [CreatedDateTime] [datetime] NULL,
	                [ModifiedDateTime] [datetime] NULL,
	                [CreatedByPersonAliasId] [int] NULL,
	                [ModifiedByPersonAliasId] [int] NULL,
	                [ForeignKey] [nvarchar](50) NULL,
                    [ForeignGuid] [uniqueidentifier] NULL,
                    [ForeignId] [int] NULL,
                 CONSTRAINT [PK__rocks_kfs_StepsToCare_NoteTemplate] PRIMARY KEY CLUSTERED
                (
	                [Id] ASC
                )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
                ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

                ALTER TABLE [dbo].[_rocks_kfs_StepsToCare_NoteTemplate]  WITH CHECK ADD  CONSTRAINT [FK__rocks_kfs_StepsToCare_NoteTemplate_CreatedPersonAlias] FOREIGN KEY([CreatedByPersonAliasId])
                REFERENCES [dbo].[PersonAlias] ([Id])

                ALTER TABLE [dbo].[_rocks_kfs_StepsToCare_NoteTemplate] CHECK CONSTRAINT [FK__rocks_kfs_StepsToCare_NoteTemplate_CreatedPersonAlias]

                ALTER TABLE [dbo].[_rocks_kfs_StepsToCare_NoteTemplate]  WITH CHECK ADD  CONSTRAINT [FK__rocks_kfs_StepsToCare_NoteTemplate_ModifiedPersonAlias] FOREIGN KEY([ModifiedByPersonAliasId])
                REFERENCES [dbo].[PersonAlias] ([Id])

                ALTER TABLE [dbo].[_rocks_kfs_StepsToCare_NoteTemplate] CHECK CONSTRAINT [FK__rocks_kfs_StepsToCare_NoteTemplate_ModifiedPersonAlias]

                ALTER TABLE dbo._rocks_kfs_StepsToCare_NoteTemplate ADD CONSTRAINT
	                DF__rocks_kfs_StepsToCare_NoteTemplate_IsActive DEFAULT 1 FOR IsActive

                ALTER TABLE dbo._rocks_kfs_StepsToCare_NoteTemplate ADD CONSTRAINT
	                DF__rocks_kfs_StepsToCare_NoteTemplate_IsSystem DEFAULT 0 FOR IsSystem
            " );

            Sql( @"
                CREATE TABLE [dbo].[_rocks_kfs_StepsToCare_AssignedPerson](
	                [Id] [int] IDENTITY(1,1) NOT NULL,
	                [NeedId] [int] NULL,
	                [PersonAliasId] [int] NULL,
	                [WorkerAliasId] [int] NULL,
	                [FollowUpWorker] [bit] NULL,
	                [IsActive] [bit] NOT NULL,
	                [Guid] [uniqueidentifier] NULL,
	                [CreatedDateTime] [datetime] NULL,
	                [ModifiedDateTime] [datetime] NULL,
	                [CreatedByPersonAliasId] [int] NULL,
	                [ModifiedByPersonAliasId] [int] NULL,
	                [ForeignKey] [nvarchar](50) NULL,
                    [ForeignGuid] [uniqueidentifier] NULL,
                    [ForeignId] [int] NULL,
                 CONSTRAINT [PK__rocks_kfs_StepsToCare_AssignedPerson] PRIMARY KEY CLUSTERED
                (
	                [Id] ASC
                )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
                ) ON [PRIMARY]

                ALTER TABLE [dbo].[_rocks_kfs_StepsToCare_AssignedPerson]  WITH CHECK ADD  CONSTRAINT [FK__rocks_kfs_StepsToCare_AssignedPerson__rocks_kfs_StepsToCare_CareNeed] FOREIGN KEY([NeedId])
                REFERENCES [dbo].[_rocks_kfs_StepsToCare_CareNeed] ([Id])

                ALTER TABLE [dbo].[_rocks_kfs_StepsToCare_AssignedPerson] CHECK CONSTRAINT [FK__rocks_kfs_StepsToCare_AssignedPerson__rocks_kfs_StepsToCare_CareNeed]

                ALTER TABLE [dbo].[_rocks_kfs_StepsToCare_AssignedPerson]  WITH CHECK ADD  CONSTRAINT [FK__rocks_kfs_StepsToCare_AssignedPerson_CreatedPersonAlias] FOREIGN KEY([CreatedByPersonAliasId])
                REFERENCES [dbo].[PersonAlias] ([Id])

                ALTER TABLE [dbo].[_rocks_kfs_StepsToCare_AssignedPerson] CHECK CONSTRAINT [FK__rocks_kfs_StepsToCare_AssignedPerson_CreatedPersonAlias]

                ALTER TABLE [dbo].[_rocks_kfs_StepsToCare_AssignedPerson]  WITH CHECK ADD  CONSTRAINT [FK__rocks_kfs_StepsToCare_AssignedPerson_ModifiedPersonAlias] FOREIGN KEY([ModifiedByPersonAliasId])
                REFERENCES [dbo].[PersonAlias] ([Id])

                ALTER TABLE [dbo].[_rocks_kfs_StepsToCare_AssignedPerson] CHECK CONSTRAINT [FK__rocks_kfs_StepsToCare_AssignedPerson_ModifiedPersonAlias]

                ALTER TABLE [dbo].[_rocks_kfs_StepsToCare_AssignedPerson]  WITH CHECK ADD  CONSTRAINT [FK__rocks_kfs_StepsToCare_AssignedPerson_PersonAlias] FOREIGN KEY([PersonAliasId])
                REFERENCES [dbo].[PersonAlias] ([Id])

                ALTER TABLE [dbo].[_rocks_kfs_StepsToCare_AssignedPerson] CHECK CONSTRAINT [FK__rocks_kfs_StepsToCare_AssignedPerson_PersonAlias]

                ALTER TABLE [dbo].[_rocks_kfs_StepsToCare_AssignedPerson]  WITH CHECK ADD  CONSTRAINT [FK__rocks_kfs_StepsToCare_AssignedPerson_WorkerPersonAlias] FOREIGN KEY([WorkerAliasId])
                REFERENCES [dbo].[PersonAlias] ([Id])

                ALTER TABLE [dbo].[_rocks_kfs_StepsToCare_AssignedPerson] CHECK CONSTRAINT [FK__rocks_kfs_StepsToCare_AssignedPerson_WorkerPersonAlias]

                ALTER TABLE dbo._rocks_kfs_StepsToCare_AssignedPerson ADD CONSTRAINT
	                DF__rocks_kfs_StepsToCare_AssignedPerson_IsActive DEFAULT 1 FOR IsActive
            " );

            RockMigrationHelper.UpdateEntityType( "rocks.kfs.StepsToCare.Model.CareNeed", "87AC878D-6740-43EB-9389-B8440AC595C3", true, true );
            RockMigrationHelper.UpdateEntityType( "rocks.kfs.StepsToCare.Model.CareWorker", "E6C8DEF1-F51F-48F6-A2E9-749F86F89FFF", true, true );
            RockMigrationHelper.UpdateEntityType( "rocks.kfs.StepsToCare.Model.AssignedPerson", "BC88FC72-8C9A-45F3-B072-CA0EA8E88A2D", true, true );
            RockMigrationHelper.UpdateEntityType( "rocks.kfs.StepsToCare.Model.NoteTemplate", "4926FF37-0E30-401B-9AD7-A028745B5383", true, true );
        }

        public override void Down()
        {
            RockMigrationHelper.DeleteEntityType( "4926FF37-0E30-401B-9AD7-A028745B5383" );
            RockMigrationHelper.DeleteEntityType( "BC88FC72-8C9A-45F3-B072-CA0EA8E88A2D" );
            RockMigrationHelper.DeleteEntityType( "E6C8DEF1-F51F-48F6-A2E9-749F86F89FFF" );
            RockMigrationHelper.DeleteEntityType( "CF73C1F3-670C-4FBD-B042-A13298B6641E" );
            RockMigrationHelper.DeleteEntityType( "87AC878D-6740-43EB-9389-B8440AC595C3" );

            Sql( @"
                ALTER TABLE [dbo].[_rocks_kfs_StepsToCare_AssignedPerson] DROP CONSTRAINT [FK__rocks_kfs_StepsToCare_AssignedPerson_WorkerPersonAlias]
                ALTER TABLE [dbo].[_rocks_kfs_StepsToCare_AssignedPerson] DROP CONSTRAINT [FK__rocks_kfs_StepsToCare_AssignedPerson_PersonAlias]
                ALTER TABLE [dbo].[_rocks_kfs_StepsToCare_AssignedPerson] DROP CONSTRAINT [FK__rocks_kfs_StepsToCare_AssignedPerson_ModifiedPersonAlias]
                ALTER TABLE [dbo].[_rocks_kfs_StepsToCare_AssignedPerson] DROP CONSTRAINT [FK__rocks_kfs_StepsToCare_AssignedPerson_CreatedPersonAlias]
                ALTER TABLE [dbo].[_rocks_kfs_StepsToCare_AssignedPerson] DROP CONSTRAINT [FK__rocks_kfs_StepsToCare_AssignedPerson__rocks_kfs_StepsToCare_CareNeed]
                DROP TABLE [dbo].[_rocks_kfs_StepsToCare_AssignedPerson]

                ALTER TABLE [dbo].[_rocks_kfs_StepsToCare_NoteTemplate] DROP CONSTRAINT [FK__rocks_kfs_StepsToCare_NoteTemplate_ModifiedPersonAlias]
                ALTER TABLE [dbo].[_rocks_kfs_StepsToCare_NoteTemplate] DROP CONSTRAINT [FK__rocks_kfs_StepsToCare_NoteTemplate_CreatedPersonAlias]
                DROP TABLE [dbo].[_rocks_kfs_StepsToCare_NoteTemplate]

                ALTER TABLE [dbo].[_rocks_kfs_StepsToCare_CareWorker] DROP CONSTRAINT [FK__rocks_kfs_StepsToCare_CareWorker_CampusId]
                ALTER TABLE [dbo].[_rocks_kfs_StepsToCare_CareWorker] DROP CONSTRAINT [FK__rocks_kfs_StepsToCare_CareWorker_DefinedValue]
                ALTER TABLE [dbo].[_rocks_kfs_StepsToCare_CareWorker] DROP CONSTRAINT [FK__rocks_kfs_StepsToCare_CareWorker_PersonAlias]
                ALTER TABLE [dbo].[_rocks_kfs_StepsToCare_CareWorker] DROP CONSTRAINT [FK__rocks_kfs_StepsToCare_CareWorker_ModifiedPersonAlias]
                ALTER TABLE [dbo].[_rocks_kfs_StepsToCare_CareWorker] DROP CONSTRAINT [FK__rocks_kfs_StepsToCare_CareWorker_CreatedPersonAlias]
                DROP TABLE [dbo].[_rocks_kfs_StepsToCare_CareWorker]

                ALTER TABLE [dbo].[_rocks_kfs_StepsToCare_CareNeed] DROP CONSTRAINT [DF__rocks_kfs_StepsToCare_CareNeed_IsActive]
                ALTER TABLE [dbo].[_rocks_kfs_StepsToCare_CareNeed] DROP CONSTRAINT [FK__rocks_kfs_StepsToCare_CareNeed_CampusId]
                ALTER TABLE [dbo].[_rocks_kfs_StepsToCare_CareNeed] DROP CONSTRAINT [FK__rocks_kfs_StepsToCare_CareNeed_StatusDefinedValue]
                ALTER TABLE [dbo].[_rocks_kfs_StepsToCare_CareNeed] DROP CONSTRAINT [FK__rocks_kfs_StepsToCare_CareNeed_CategoryDefinedValue]
                ALTER TABLE [dbo].[_rocks_kfs_StepsToCare_CareNeed] DROP CONSTRAINT [FK__rocks_kfs_StepsToCare_CareNeed_SubmitterPersonAlias]
                ALTER TABLE [dbo].[_rocks_kfs_StepsToCare_CareNeed] DROP CONSTRAINT [FK__rocks_kfs_StepsToCare_CareNeed_PersonAlias]
                ALTER TABLE [dbo].[_rocks_kfs_StepsToCare_CareNeed] DROP CONSTRAINT [FK__rocks_kfs_StepsToCare_CareNeed_ModifiedPersonAlias]
                ALTER TABLE [dbo].[_rocks_kfs_StepsToCare_CareNeed] DROP CONSTRAINT [FK__rocks_kfs_StepsToCare_CareNeed_CreatedPersonAlias]
                DROP TABLE [dbo].[_rocks_kfs_StepsToCare_CareNeed]
            " );
        }
    }
}