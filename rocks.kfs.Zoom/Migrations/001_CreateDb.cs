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

namespace rocks.kfs.Zoom.Migrations
{
    [MigrationNumber( 1, "1.12.4" )]
    public partial class CreateDb : Migration
    {
        /// <summary>
        /// Operations to be performed during the upgrade process.
        /// </summary>
        public override void Up()
        {
            Sql( @"IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[_rocks_kfs_ZoomRoomOccurrence]') AND type in (N'U'))
            BEGIN
                CREATE TABLE [dbo].[_rocks_kfs_ZoomRoomOccurrence](
	                [Id] [int] IDENTITY(1,1) NOT NULL,
	                [ZoomMeetingId] [bigint] NULL,
	                [ZoomMeetingJoinUrl] [varchar](2000) NULL,
	                [ScheduleId] [int] NOT NULL,
	                [LocationId] [int] NOT NULL,
	                [Topic] [varchar](200) NOT NULL,
	                [StartTime] [datetime] NOT NULL,
	                [TimeZone] [varchar](50) NULL,
	                [Password] [varchar](10) NULL,
	                [Duration] [int] NOT NULL,
	                [SendAt] [datetime] NULL,
	                [IsOccurring] [bit] NOT NULL,
	                [IsCompleted] [bit] NOT NULL,
	                [EntityTypeId] [int] NULL,
	                [EntityId] [int] NULL,
	                [IsActive] [bit] NOT NULL,
	                [Guid] [uniqueidentifier] NOT NULL,
	                [CreatedDateTime] [datetime] NULL,
	                [ModifiedDateTime] [datetime] NULL,
	                [CreatedByPersonAliasId] [int] NULL,
	                [ModifiedByPersonAliasId] [int] NULL,
	                [ForeignKey] [nvarchar](50) NULL,
	                [ForeignGuid] [uniqueidentifier] NULL,
	                [ForeignId] [int] NULL,
                 CONSTRAINT [PK__rocks_kfs_ZoomRoomOccurrence] PRIMARY KEY CLUSTERED 
                (
	                [Id] ASC
                )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
                ) ON [PRIMARY]

                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[_rocks_kfs_ZoomRoomOccurrence]') AND name = N'IX_EntityTypeId')
                CREATE NONCLUSTERED INDEX [IX_EntityTypeId] ON [dbo].[_rocks_kfs_ZoomRoomOccurrence]
                (
	                [EntityTypeId] ASC
                )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]

                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[_rocks_kfs_ZoomRoomOccurrence]') AND name = N'IX_EntityTypeId_EntityId')
                CREATE NONCLUSTERED INDEX [IX_EntityTypeId_EntityId] ON [dbo].[_rocks_kfs_ZoomRoomOccurrence]
                (
	                [EntityTypeId] ASC,
	                [EntityId] ASC
                )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]

                ALTER TABLE [dbo].[_rocks_kfs_ZoomRoomOccurrence] ADD  CONSTRAINT [DF__rocks_kfs_ZoomRoomOccurrence_IsOccurring]  DEFAULT ((1)) FOR [IsOccurring]

                ALTER TABLE [dbo].[_rocks_kfs_ZoomRoomOccurrence] ADD  CONSTRAINT [DF__rocks_kfs_ZoomRoomOccurrence_IsCompleted]  DEFAULT ((0)) FOR [IsCompleted]

                ALTER TABLE [dbo].[_rocks_kfs_ZoomRoomOccurrence] ADD  CONSTRAINT [DF__rocks_kfs_ZoomRoomOccurrence_IsActive]  DEFAULT ((1)) FOR [IsActive]
                
                ALTER TABLE [dbo].[_rocks_kfs_ZoomRoomOccurrence]  WITH CHECK ADD  CONSTRAINT [FK__rocks_kfs_ZoomRoomOccurrence_Location] FOREIGN KEY([LocationId])
                REFERENCES [dbo].[Location] ([Id])
                ON DELETE CASCADE

                ALTER TABLE [dbo].[_rocks_kfs_ZoomRoomOccurrence] CHECK CONSTRAINT [FK__rocks_kfs_ZoomRoomOccurrence_Location]
                
                ALTER TABLE [dbo].[_rocks_kfs_ZoomRoomOccurrence]  WITH CHECK ADD  CONSTRAINT [FK__rocks_kfs_ZoomRoomOccurrence_Schedule] FOREIGN KEY([ScheduleId])
                REFERENCES [dbo].[Schedule] ([Id])

                ALTER TABLE [dbo].[_rocks_kfs_ZoomRoomOccurrence] CHECK CONSTRAINT [FK__rocks_kfs_ZoomRoomOccurrence_Schedule]
            END
                " );

            RockMigrationHelper.UpdateEntityType( "rocks.kfs.Zoom.Model.RoomOccurrence", "2A138B5B-3CD8-4F03-ACAD-4D544D257916", true, true );
        }

        /// <summary>
        /// Operations to be performed during the downgrade process.
        /// </summary>
        public override void Down()
        {
            RockMigrationHelper.DeleteEntityType( "2A138B5B-3CD8-4F03-ACAD-4D544D257916" );
            Sql( @"IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[_rocks_kfs_ZoomRoomOccurrence]') AND type in (N'U'))
            BEGIN
                DROP TABLE [dbo].[_rocks_kfs_ZoomRoomOccurrence]
            END
            " );
        }
    }
}