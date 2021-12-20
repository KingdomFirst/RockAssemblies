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
using Rock;
using Rock.Model;
using Rock.Plugin;

namespace rocks.kfs.Zoom.Migrations
{
    [MigrationNumber( 4, "1.12.4" )]
    public partial class CreateZoomRoomSyncJob : Migration
    {
        /// <summary>
        /// Operations to be performed during the upgrade process.
        /// </summary>
        public override void Up()
        {
            Sql( @"IF NOT EXISTS( SELECT [Id] FROM [ServiceJob] WHERE [Class] = 'rocks.kfs.Zoom.Jobs.ZoomRoomSchedulingAndMaintenance' AND [Guid] = 'B23BD7D6-046D-411D-8F06-64EE237FA770' )
            BEGIN
               INSERT INTO [ServiceJob] (
                  [IsSystem]
                  ,[IsActive]
                  ,[Name]
                  ,[Description]
                  ,[Class]
                  ,[CronExpression]
                  ,[NotificationStatus]
                  ,[Guid] )
               VALUES ( 
                  0
                  ,1
                  ,'Zoom Room Reservation Scheduling and Maintenance'
                  ,'Generates Zoom Room occurrences and synchronizes meeting schedules for Room Reservations tied to Locations linked to a Zoom Room.'
                  ,'rocks.kfs.Zoom.Jobs.ZoomRoomSchedulingAndMaintenance'
                  ,'0 0/1 * 1/1 * ? *'
                  ,1
                  ,'B23BD7D6-046D-411D-8F06-64EE237FA770'
                  );
            END" );

            // Attribute: rocks.kfs.Zoom.Jobs.ZoomRoomSchedulingAndMaintenance: Sync Days Out
            RockMigrationHelper.AddOrUpdateEntityAttribute( "Rock.Model.ServiceJob", Rock.SystemGuid.FieldType.INTEGER, "Class", "rocks.kfs.Zoom.Jobs.ZoomRoomSchedulingAndMaintenance", "Sync Days Out", "SyncDaysOut", @"Number of days into the future to sync Locations to Zoom Rooms if the Schedule does not have an effective date.", 0, @"30", "4504E3F4-E2E1-44DB-B6D7-E2CC1F498695", "SyncDaysOut" );
            RockMigrationHelper.AddAttributeValue( "4504E3F4-E2E1-44DB-B6D7-E2CC1F498695", -1, @"30", "43FF6707-E0C9-4303-804D-48587EDD20D0" );

            // Attribute: rocks.kfs.Zoom.Jobs.ZoomRoomSchedulingAndMaintenance: Import Zoom Room Meetings 
            RockMigrationHelper.AddOrUpdateEntityAttribute( "Rock.Model.ServiceJob", Rock.SystemGuid.FieldType.BOOLEAN, "Class", "rocks.kfs.Zoom.Jobs.ZoomRoomSchedulingAndMaintenance", "Import Zoom Room Meetings", "Import Zoom Room Meetings", @"Create Room Reservations for any Zoom Room meetings scheduled outside of Rock. This will help reduce the chances of the Room Reservation plugin scheduling a conflict with other Zoom Room meetings.", 0, bool.FalseString, "E512BF8F-EBD0-40D9-B310-3C3B3B895629", "ImportMeetings" );
            RockMigrationHelper.AddAttributeValue( "E512BF8F-EBD0-40D9-B310-3C3B3B895629", -1, bool.FalseString, "7D96AD5A-29A6-40F0-BF1D-1D6EEEA26A8C" );

            Sql( @"DECLARE @JobId int = ( SELECT TOP 1 [Id] FROM [ServiceJob] WHERE [Guid] = 'B23BD7D6-046D-411D-8F06-64EE237FA770' )
                 UPDATE [AttributeValue] SET EntityId = @JobId WHERE [Guid] = '43FF6707-E0C9-4303-804D-48587EDD20D0'" ); // Set EntityId to proper Job id
        }

        /// <summary>
        /// Operations to be performed during the downgrade process.
        /// </summary>
        public override void Down()
        {
            RockMigrationHelper.DeleteAttribute( "4504E3F4-E2E1-44DB-B6D7-E2CC1F498695" ); // rocks.kfs.Zoom.Jobs.ZoomRoomSchedulingAndMaintenance: Sync Days Out
            RockMigrationHelper.DeleteAttribute( "E512BF8F-EBD0-40D9-B310-3C3B3B895629" ); // rocks.kfs.Zoom.Jobs.ZoomRoomSchedulingAndMaintenance: Import Zoom Room Meetings

            // remove ServiceJob: Zoom Room Scheduling And Maintenance
            Sql( @"IF EXISTS( SELECT [Id] FROM [ServiceJob] WHERE [Class] = 'rocks.kfs.Zoom.Jobs.ZoomRoomSchedulingAndMaintenance' AND [Guid] = 'B23BD7D6-046D-411D-8F06-64EE237FA770' )
            BEGIN
               DELETE [ServiceJob]  WHERE [Guid] = 'B23BD7D6-046D-411D-8F06-64EE237FA770';
            END" );
        }
    }
}