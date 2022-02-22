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
using Rock;
using Rock.Model;
using Rock.Plugin;

namespace rocks.kfs.Zoom.Migrations
{
    [MigrationNumber( 5, "1.12.4" )]
    public partial class CreateJobs : Migration
    {
        /// <summary>
        /// Operations to be performed during the upgrade process.
        /// </summary>
        public override void Up()
        {
            #region Zoom Room Scheduling and Maintenance Job

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
                  ,0
                  ,'Zoom Room Reservation Scheduling and Maintenance'
                  ,'Generates Zoom Room occurrences and synchronizes meeting schedules for Room Reservations tied to Locations linked to a Zoom Room.'
                  ,'rocks.kfs.Zoom.Jobs.ZoomRoomSchedulingAndMaintenance'
                  ,'0 0 0/1 1/1 * ? *'  -- Every Hour
                  ,1
                  ,'B23BD7D6-046D-411D-8F06-64EE237FA770'
                  );
            END" );

            // Attribute: rocks.kfs.Zoom.Jobs.ZoomRoomSchedulingAndMaintenance: Sync Days Out
            RockMigrationHelper.AddOrUpdateEntityAttribute( "Rock.Model.ServiceJob", Rock.SystemGuid.FieldType.INTEGER, "Class", "rocks.kfs.Zoom.Jobs.ZoomRoomSchedulingAndMaintenance", "Sync Days Out", "SyncDaysOut", @"Number of days into the future to sync Locations to Zoom Rooms if the Schedule does not have an effective date.", 1, @"30", "4504E3F4-E2E1-44DB-B6D7-E2CC1F498695", "SyncDaysOut" );
            RockMigrationHelper.AddAttributeValue( "4504E3F4-E2E1-44DB-B6D7-E2CC1F498695", -1, @"30", "43FF6707-E0C9-4303-804D-48587EDD20D0" );

            // Attribute: rocks.kfs.Zoom.Jobs.ZoomRoomSchedulingAndMaintenance: Enable Verbose Logging 
            RockMigrationHelper.AddOrUpdateEntityAttribute( "Rock.Model.ServiceJob", Rock.SystemGuid.FieldType.BOOLEAN, "Class", "rocks.kfs.Zoom.Jobs.ZoomRoomSchedulingAndMaintenance", "Enable Verbose Logging", "Verbose Logging", @"Turn on extra logging points in addition to the standard job logging points. This is only recommended for testing/troubleshooting purposes.", 2, bool.FalseString, "6EE5573F-D3CE-4E04-A9D7-EF6F19343961", "VerboseLogging" );
            RockMigrationHelper.AddAttributeValue( "6EE5573F-D3CE-4E04-A9D7-EF6F19343961", -1, bool.FalseString, "D5CEA4A8-54A4-4790-926F-A93423471BA0" );

            // Attribute: rocks.kfs.Zoom.Jobs.ZoomRoomSchedulingAndMaintenance: Import Zoom Room Meetings 
            RockMigrationHelper.AddOrUpdateEntityAttribute( "Rock.Model.ServiceJob", Rock.SystemGuid.FieldType.BOOLEAN, "Class", "rocks.kfs.Zoom.Jobs.ZoomRoomSchedulingAndMaintenance", "Import Zoom Room Meetings", "Import Zoom Room Meetings", @"Create Room Reservations for any Zoom Room meetings scheduled outside of Rock. This will help reduce the chances of the Room Reservation plugin scheduling a conflict with other Zoom Room meetings.", 3, bool.FalseString, "E512BF8F-EBD0-40D9-B310-3C3B3B895629", "ImportMeetings" );
            RockMigrationHelper.AddAttributeValue( "E512BF8F-EBD0-40D9-B310-3C3B3B895629", -1, bool.FalseString, "7D96AD5A-29A6-40F0-BF1D-1D6EEEA26A8C" );

            Sql( @"DECLARE @JobId int = ( SELECT TOP 1 [Id] FROM [ServiceJob] WHERE [Guid] = 'B23BD7D6-046D-411D-8F06-64EE237FA770' )
                 UPDATE [AttributeValue] SET EntityId = @JobId WHERE [Guid] IN ( '43FF6707-E0C9-4303-804D-48587EDD20D0', 'D5CEA4A8-54A4-4790-926F-A93423471BA0', '7D96AD5A-29A6-40F0-BF1D-1D6EEEA26A8C' )" ); // Set EntityId to proper Job id

            #endregion Zoom Room Scheduling and Maintenance Job

            #region Zoom Meeting Reminder Job

            Sql( @"IF NOT EXISTS( SELECT [Id] FROM [ServiceJob] WHERE [Class] = 'rocks.kfs.Zoom.Jobs.ZoomMeetingGroupReminder' AND [Guid] = '07BCC7E6-0E02-4ECE-8948-422C4E4EF20D' )
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
                  ,0
                  ,'Zoom Meeting Group Reminder'
                  ,'Sends a reminder to members of a group connected with a room reservation that has a Zoom meeting attached to it.'
                  ,'rocks.kfs.Zoom.Jobs.ZoomMeetingGroupReminder'
                  ,'0 0 5 1/1 * ? *' -- Daily at 5:00 AM
                  ,1
                  ,'07BCC7E6-0E02-4ECE-8948-422C4E4EF20D'
                  );
            END" );

            // Attribute: rocks.kfs.Zoom.Jobs.ZoomMeetingGroupReminder: System Communication
            RockMigrationHelper.AddOrUpdateEntityAttribute( "Rock.Model.ServiceJob", Rock.SystemGuid.FieldType.SYSTEM_COMMUNICATION, "Class", "rocks.kfs.Zoom.Jobs.ZoomMeetingGroupReminder", "System Communication", "System Communication", @"The system communication to use when sending Zoom meeting reminders.", 1, ZoomGuid.SystemComunication.ZOOM_MEETING_REMINDER, "00D58FC1-E85B-4D6F-8788-3F9466A8A364", "SystemCommunication" );
            RockMigrationHelper.AddAttributeValue( "00D58FC1-E85B-4D6F-8788-3F9466A8A364", -1, ZoomGuid.SystemComunication.ZOOM_MEETING_REMINDER, "5AD2F8A7-B809-4479-A118-1B36623DBFC0" );

            // Attribute: rocks.kfs.Zoom.Jobs.ZoomMeetingGroupReminder: Days Prior 
            RockMigrationHelper.AddOrUpdateEntityAttribute( "Rock.Model.ServiceJob", Rock.SystemGuid.FieldType.TEXT, "Class", "rocks.kfs.Zoom.Jobs.ZoomMeetingGroupReminder", "Days Prior", "Days Prior", @"Comma delimited list of days prior to a scheduled Zoom meeting to send a reminder. For example, a value of '2,4' would result in reminders getting sent two and four days prior to the Zoom meeting's scheduled meeting date.", 2, "1", "BFE626AD-7141-4EA3-B5CE-73F03FE18622", "DaysPrior" );
            RockMigrationHelper.AddAttributeValue( "BFE626AD-7141-4EA3-B5CE-73F03FE18622", -1, "1", "DA386BCA-3EDC-4E79-98DB-009C994D2B58" );

            // Attribute: rocks.kfs.Zoom.Jobs.ZoomMeetingGroupReminder: Reminder Group Attribute 
            RockMigrationHelper.AddOrUpdateEntityAttribute( "Rock.Model.ServiceJob", Rock.SystemGuid.FieldType.ATTRIBUTE, "Class", "rocks.kfs.Zoom.Jobs.ZoomMeetingGroupReminder", "Reminder Group Attribute", "Reminder Group", "The \"Group Type Group\" type attribute on the Room Reservation entity to be used for sending reminders. This attribute is what connects a Room Reservation to a Group for Zoom meeting purposes.", 3, ZoomGuid.Attribute.ROOM_RESERVATION_GROUP_ATTRIBUTE, "866D68EC-835E-4B2A-A2AC-791973D3B4A1", "GroupAttributeSetting" );
            RockMigrationHelper.AddAttributeValue( "866D68EC-835E-4B2A-A2AC-791973D3B4A1", -1, ZoomGuid.Attribute.ROOM_RESERVATION_GROUP_ATTRIBUTE, "385C7944-9010-4DB1-9DA1-E4BD0F552C3A" );

            Sql( @"DECLARE @JobId int = ( SELECT TOP 1 [Id] FROM [ServiceJob] WHERE [Guid] = '07BCC7E6-0E02-4ECE-8948-422C4E4EF20D' )
                 UPDATE [AttributeValue] SET EntityId = @JobId WHERE [Guid] IN ( '5AD2F8A7-B809-4479-A118-1B36623DBFC0', 'DA386BCA-3EDC-4E79-98DB-009C994D2B58', '385C7944-9010-4DB1-9DA1-E4BD0F552C3A' )" ); // Set EntityId to proper Job id

            #endregion Zoom Meeting Reminder Job

        }

        /// <summary>
        /// Operations to be performed during the downgrade process.
        /// </summary>
        public override void Down()
        {
            #region Zoom Room Scheduling And Maintenance Job

            RockMigrationHelper.DeleteAttribute( "4504E3F4-E2E1-44DB-B6D7-E2CC1F498695" ); // rocks.kfs.Zoom.Jobs.ZoomRoomSchedulingAndMaintenance: Sync Days Out
            RockMigrationHelper.DeleteAttribute( "6EE5573F-D3CE-4E04-A9D7-EF6F19343961" ); // rocks.kfs.Zoom.Jobs.ZoomRoomSchedulingAndMaintenance: Enable Verbose Logging
            RockMigrationHelper.DeleteAttribute( "E512BF8F-EBD0-40D9-B310-3C3B3B895629" ); // rocks.kfs.Zoom.Jobs.ZoomRoomSchedulingAndMaintenance: Import Zoom Room Meetings

            // remove ServiceJob: Zoom Room Scheduling And Maintenance
            Sql( @"IF EXISTS( SELECT [Id] FROM [ServiceJob] WHERE [Class] = 'rocks.kfs.Zoom.Jobs.ZoomRoomSchedulingAndMaintenance' AND [Guid] = 'B23BD7D6-046D-411D-8F06-64EE237FA770' )
            BEGIN
               DELETE FROM [ServiceJob]  WHERE [Guid] = 'B23BD7D6-046D-411D-8F06-64EE237FA770';
            END" );

            #endregion Zoom Room Scheduling And Maintenance Job

            #region Zoom Meeting Reminder Job

            RockMigrationHelper.DeleteAttribute( "00D58FC1-E85B-4D6F-8788-3F9466A8A364" ); // rocks.kfs.Zoom.Jobs.ZoomMeetingGroupReminder: System Communication
            RockMigrationHelper.DeleteAttribute( "BFE626AD-7141-4EA3-B5CE-73F03FE18622" ); // rocks.kfs.Zoom.Jobs.ZoomMeetingGroupReminder: Days Prior
            RockMigrationHelper.DeleteAttribute( "866D68EC-835E-4B2A-A2AC-791973D3B4A1" ); // rocks.kfs.Zoom.Jobs.ZoomMeetingGroupReminder: Reminder Group Attribute

            // remove ServiceJob: Zoom Meeting Reminder
            Sql( @"IF EXISTS( SELECT [Id] FROM [ServiceJob] WHERE [Class] = 'rocks.kfs.Zoom.Jobs.ZoomMeetingGroupReminder' AND [Guid] = '07BCC7E6-0E02-4ECE-8948-422C4E4EF20D' )
            BEGIN
               DELETE FROM [ServiceJob]  WHERE [Guid] = '07BCC7E6-0E02-4ECE-8948-422C4E4EF20D';
            END" );

            #endregion Zoom Meeting Reminder Job
        }
    }
}