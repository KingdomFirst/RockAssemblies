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
using rocks.kfs.Zoom.ZoomGuid;

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
            Sql( string.Format( @"IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[_rocks_kfs_ZoomRoomOccurrence]') AND type in (N'U'))
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
	                [IsCompleted] [bit] NOT NULL,
	                [EntityTypeId] [int] NULL,
	                [EntityId] [int] NULL,
	                [IsActive] [bit] NOT NULL,
	                [ZoomMeetingRequestStatus] [int] NULL,
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
                )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                ) ON [PRIMARY]

                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[_rocks_kfs_ZoomRoomOccurrence]') AND name = N'IX_EntityTypeId')
                CREATE NONCLUSTERED INDEX [IX_EntityTypeId] ON [dbo].[_rocks_kfs_ZoomRoomOccurrence]
                (
	                [EntityTypeId] ASC
                )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[_rocks_kfs_ZoomRoomOccurrence]') AND name = N'IX_EntityTypeId_EntityId')
                CREATE NONCLUSTERED INDEX [IX_EntityTypeId_EntityId] ON [dbo].[_rocks_kfs_ZoomRoomOccurrence]
                (
	                [EntityTypeId] ASC,
	                [EntityId] ASC
                )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

                ALTER TABLE [dbo].[_rocks_kfs_ZoomRoomOccurrence] ADD  CONSTRAINT [DF__rocks_kfs_ZoomRoomOccurrence_IsCompleted]  DEFAULT ((0)) FOR [IsCompleted]

                ALTER TABLE [dbo].[_rocks_kfs_ZoomRoomOccurrence] ADD  CONSTRAINT [DF__rocks_kfs_ZoomRoomOccurrence_IsActive]  DEFAULT ((1)) FOR [IsActive]
                
                ALTER TABLE [dbo].[_rocks_kfs_ZoomRoomOccurrence]  WITH CHECK ADD  CONSTRAINT [FK__rocks_kfs_ZoomRoomOccurrence_Location] FOREIGN KEY([LocationId])
                REFERENCES [dbo].[Location] ([Id])
                ON DELETE CASCADE

                ALTER TABLE [dbo].[_rocks_kfs_ZoomRoomOccurrence] CHECK CONSTRAINT [FK__rocks_kfs_ZoomRoomOccurrence_Location]
                
                ALTER TABLE [dbo].[_rocks_kfs_ZoomRoomOccurrence]  WITH CHECK ADD  CONSTRAINT [FK__rocks_kfs_ZoomRoomOccurrence_Schedule] FOREIGN KEY([ScheduleId])
                REFERENCES [dbo].[Schedule] ([Id])
                ON DELETE CASCADE

                ALTER TABLE [dbo].[_rocks_kfs_ZoomRoomOccurrence] CHECK CONSTRAINT [FK__rocks_kfs_ZoomRoomOccurrence_Schedule]
            END

            /* Add Zoom Room reservation type  */

            IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[_com_bemaservices_RoomManagement_ReservationType]') AND type in (N'U'))
            BEGIN
                IF NOT EXISTS ( SELECT * FROM [dbo].[_com_bemaservices_RoomManagement_ReservationType] WHERE [Guid] = '{0}' )
                BEGIN
                    INSERT INTO [dbo].[_com_bemaservices_RoomManagement_ReservationType] (
                        [IsSystem]
                        ,[Name]
                        ,[Description]
                        ,[IsActive]
                        ,[IconCssClass]
                        ,[IsNumberAttendingRequired]
                        ,[IsContactDetailsRequired]
                        ,[IsSetupTimeRequired]
                        ,[Guid]
                        ,[CreatedDateTime]
                        ,[ModifiedDateTime]
                        ,[IsReservationBookedOnApproval])
                        VALUES (
                        1
                        ,'Zoom Room Import'
                        ,'For use with Reservations created from Zoom Room meetings imported from Zoom api. WARNING: Making any changes to the configuration of this type could result in undesired behavior of synchronization with external Zoom Room resources.'
                        ,1
                        ,'fa fa-video'
                        ,0
                        ,0
                        ,0
                        ,'{0}'
                        ,GETDATE()
                        ,GETDATE()
                        ,0
                        )
                END

                /*   Add security for Zoom Room reservation type  */

                DECLARE @RockAdminsGroupId INT
                SET @RockAdminsGroupId = (SELECT [Id] FROM [Group] WHERE [Guid] = '{1}')
                DECLARE @entityTypeId int
                SET @entityTypeId = (SELECT [Id] FROM [EntityType] WHERE [Guid] = 'AC498297-D28C-47C0-B53B-4BF54D895DEB')    -- ReservationType entity type
                DECLARE @entityId int
                SET @entityId = (SELECT [Id] FROM [_com_bemaservices_RoomManagement_ReservationType] WHERE [Guid] = '{0}')

                IF NOT EXISTS ( SELECT [Id] FROM [dbo].[Auth] WHERE [EntityTypeId] = @entityTypeId AND [EntityId] = @entityId AND [Action] = '{2}' AND [SpecialRole] = 0 AND [GroupId] = @RockAdminsGroupId)
                BEGIN
                    INSERT INTO [dbo].[Auth] (
                        [EntityTypeId]
                        ,[EntityId]
                        ,[Order]
                        ,[Action]
                        ,[AllowOrDeny]
                        ,[SpecialRole]
                        ,[GroupId]
                        ,[Guid])
                        VALUES (
                        @entityTypeId
                        ,@entityId
                        ,0
                        ,'{2}'
                        ,'A'
                        ,0
                        ,@RockAdminsGroupId
                        ,'D32F44D8-22A0-4FEF-91C2-CF05833523DF')
                END

                IF NOT EXISTS ( SELECT [Id] FROM [dbo].[Auth] WHERE [EntityTypeId] = @entityTypeId AND [EntityId] = @entityId AND [Action] = '{2}' AND [SpecialRole] = 1 AND [GroupId] = NULL )
                BEGIN
                    INSERT INTO [dbo].[Auth] (
                        [EntityTypeId]
                        ,[EntityId]
                        ,[Order]
                        ,[Action]
                        ,[AllowOrDeny]
                        ,[SpecialRole]
                        ,[Guid])
                        VALUES (
                        @entityTypeId
                        ,@entityId
                        ,1
                        ,'{2}'
                        ,'D'
                        ,1
                        ,'46ACAA04-F415-4CB8-9D2F-88B7596F6409')
                END
            END", RoomReservationType.ZOOMROOMIMPORT, Rock.SystemGuid.Group.GROUP_ADMINISTRATORS, Rock.Security.Authorization.VIEW ) );

            RockMigrationHelper.UpdateEntityType( "rocks.kfs.Zoom.Model.RoomOccurrence", "2A138B5B-3CD8-4F03-ACAD-4D544D257916", true, true );
        }

        /// <summary>
        /// Operations to be performed during the downgrade process.
        /// </summary>
        public override void Down()
        {
            RockMigrationHelper.DeleteEntityType( "2A138B5B-3CD8-4F03-ACAD-4D544D257916" );
            Sql( string.Format( @"IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[_rocks_kfs_ZoomRoomOccurrence]') AND type in (N'U'))
            BEGIN
                DROP TABLE [dbo].[_rocks_kfs_ZoomRoomOccurrence]
            END
            IF EXISTS ( SELECT * FROM [dbo].[_com_bemaservices_RoomManagement_ReservationType] WHERE [Guid] = '{0}' )
            BEGIN
                DECLARE @KFSZoomReservationTypeId INT = ( SELECT Id FROM [_com_bemaservices_RoomManagement_ReservationType] WHERE [Guid] = '{0}' )
                IF NOT EXISTS ( SELECT * FROM [_com_bemaservices_RoomManagement_Reservation] WHERE [ReservationTypeId] = @KFSZoomReservationTypeId )
                BEGIN
                    DELETE FROM [dbo].[_com_bemaservices_RoomManagement_ReservationType] WHERE [Guid] = '{0}'
                END
            END
            ", RoomReservationType.ZOOMROOMIMPORT ) );
            RockMigrationHelper.DeleteSecurityAuth( "D32F44D8-22A0-4FEF-91C2-CF05833523DF" );
            RockMigrationHelper.DeleteSecurityAuth( "46ACAA04-F415-4CB8-9D2F-88B7596F6409" );
        }
    }
}