﻿// <copyright>
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
using Rock.Plugin;

namespace rocks.kfs.Zoom.Migrations
{
    [MigrationNumber( 2, "1.12.4" )]
    public partial class CreateAttributes : Migration
    {
        /// <summary>
        /// Operations to be performed during the upgrade process.
        /// </summary>
        public override void Up()
        {
            // Assign GUID to Zoom and add security
            RockMigrationHelper.UpdateEntityType( "rocks.kfs.Zoom.ZoomApp", ZoomGuid.EntityType.ZOOM, false, true );
            RockMigrationHelper.AddSecurityAuthForEntityType( "rocks.kfs.Zoom.ZoomApp", 0, Rock.Security.Authorization.VIEW, true, Rock.SystemGuid.Group.GROUP_ADMINISTRATORS, 0, "6BC0A6D7-844F-49DF-BE40-5AD45EAB3CEE" );
            RockMigrationHelper.AddSecurityAuthForEntityType( "rocks.kfs.Zoom.ZoomApp", 1, Rock.Security.Authorization.VIEW, false, null, Rock.Model.SpecialRole.AllUsers.ConvertToInt(), "3921542F-476D-48B4-97F5-788709A1F4B4" );

            RockMigrationHelper.UpdateFieldType( "Zoom Room", "", "rocks.kfs.Zoom", "rocks.kfs.Zoom.Field.Types.ZoomRoomFieldType", ZoomGuid.FieldType.ZOOM_ROOM );
            RockMigrationHelper.UpdateFieldType( "Zoom Room", "", "rocks.kfs.Zoom", "rocks.kfs.Zoom.Field.Types.ZoomMeetingFieldType", ZoomGuid.FieldType.ZOOM_MEETING );

            RockMigrationHelper.AddNewEntityAttribute( "rocks.kfs.Zoom.ZoomApp", Rock.SystemGuid.FieldType.ENCRYPTED_TEXT, "", "", "API Key", "", "The API Key for the Zoom Marketplace JWT app to use for KFS Zoom integration elements.", 0, "", "D53A2B48-C8B4-4481-B2AB-139CAF90C78C", "KFSZoomApiKey" );
            RockMigrationHelper.AddNewEntityAttribute( "rocks.kfs.Zoom.ZoomApp", Rock.SystemGuid.FieldType.ENCRYPTED_TEXT, "", "", "API Secret", "", "The API Secret for the Zoom Marketplace JWT app to use for KFS Zoom integration elements.", 0, "", "CB89B071-4866-41BD-ACB1-1810F5224A82", "KFSZoomApiSecret" );
            RockMigrationHelper.AddNewEntityAttribute( "rocks.kfs.Zoom.ZoomApp", Rock.SystemGuid.FieldType.TEXT, "", "", "Webhook URL", "", "The URL for the webhook to handle callbacks from Zoom api.", 0, "", "4B6E0AF2-9D9D-4801-B8CE-F2B424F970FE", "KFSZoomWebhookURL" );

            RockMigrationHelper.AddDefinedType( "Zoom", "Zoom Rooms", "Zoom Rooms available for linking to Rock Locations.", ZoomGuid.DefinedType.ZOOM_ROOM );
            RockMigrationHelper.AddDefinedTypeAttribute( ZoomGuid.DefinedType.ZOOM_ROOM, Rock.SystemGuid.FieldType.TEXT, "User Name", "ZoomUsersName", "Zoom user's name.", 0, "", ZoomGuid.Attribute.ZOOM_USER_NAME );
            RockMigrationHelper.AddDefinedTypeAttribute( ZoomGuid.DefinedType.ZOOM_ROOM, Rock.SystemGuid.FieldType.INTEGER, "User PMI", "ZoomUsersPMI", "Zoom user's personal meeting ID.", 1, "", ZoomGuid.Attribute.ZOOM_USER_PMI );
            RockMigrationHelper.AddDefinedTypeAttribute( ZoomGuid.DefinedType.ZOOM_ROOM, Rock.SystemGuid.FieldType.TEXT, "Meeting Password", "ZoomMeetingPassword", "Zoom meeting password.", 2, "", ZoomGuid.Attribute.ZOOM_MEETING_PASSWORD );
            RockMigrationHelper.AddDefinedTypeAttribute( ZoomGuid.DefinedType.ZOOM_ROOM, Rock.SystemGuid.FieldType.BOOLEAN, "Join Before Host", "ZoomJoinBeforeHost", "Zoom meeting's Join Before Host setting.", 3, "false", ZoomGuid.Attribute.ZOOM_MEETING_JOIN_BEFORE_HOST );
            RockMigrationHelper.AddDefinedTypeAttribute( ZoomGuid.DefinedType.ZOOM_ROOM, Rock.SystemGuid.FieldType.TEXT, "Time Zone", "ZoomMeetingTimeZone", "Zoom meeting time zone. A list of valid time zone strings can be found <a href='https://marketplace.zoom.us/docs/api-reference/other-references/abbreviation-lists#timezones' target='_blank'>here</a>. Example: America/New_York", 4, "", ZoomGuid.Attribute.ZOOM_MEETING_TIME_ZONE );

            RockMigrationHelper.AddOrUpdateEntityAttribute( "Rock.Model.Location", Rock.SystemGuid.FieldType.DEFINED_VALUE, "", "", "Zoom Room", "", "The Zoom Room associated with this location.", 0, "", ZoomGuid.Attribute.ZOOR_ROOM_LOCATION_ENTITY_ATTRIBUTE, "KFSZoomRoom" );
        }

        /// <summary>
        /// Operations to be performed during the downgrade process.
        /// </summary>
        public override void Down()
        {
            RockMigrationHelper.DeleteAttribute( ZoomGuid.Attribute.ZOOR_ROOM_LOCATION_ENTITY_ATTRIBUTE );
            RockMigrationHelper.DeleteAttribute( ZoomGuid.Attribute.ZOOM_USER_NAME );
            RockMigrationHelper.DeleteAttribute( ZoomGuid.Attribute.ZOOM_USER_PMI );
            RockMigrationHelper.DeleteAttribute( ZoomGuid.Attribute.ZOOM_MEETING_PASSWORD );
            RockMigrationHelper.DeleteAttribute( ZoomGuid.Attribute.ZOOM_MEETING_JOIN_BEFORE_HOST );
            RockMigrationHelper.DeleteAttribute( ZoomGuid.Attribute.ZOOM_MEETING_TIME_ZONE );
            RockMigrationHelper.DeleteDefinedType( ZoomGuid.DefinedType.ZOOM_ROOM );
            RockMigrationHelper.DeleteAttribute( "CB89B071-4866-41BD-ACB1-1810F5224A82" );
            RockMigrationHelper.DeleteAttribute( "D53A2B48-C8B4-4481-B2AB-139CAF90C78C" );
            RockMigrationHelper.DeleteAttribute( "4B6E0AF2-9D9D-4801-B8CE-F2B424F970FE" );
            RockMigrationHelper.DeleteFieldType( ZoomGuid.FieldType.ZOOM_MEETING );
            RockMigrationHelper.DeleteFieldType( ZoomGuid.FieldType.ZOOM_ROOM );
            RockMigrationHelper.DeleteSecurityAuth( "3921542F-476D-48B4-97F5-788709A1F4B4" );
            RockMigrationHelper.DeleteSecurityAuth( "6BC0A6D7-844F-49DF-BE40-5AD45EAB3CEE" );
            RockMigrationHelper.DeleteEntityType( ZoomGuid.EntityType.ZOOM );
        }
    }
}