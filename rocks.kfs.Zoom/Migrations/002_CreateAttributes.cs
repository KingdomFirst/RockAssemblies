﻿// <copyright>
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
using Rock.Plugin;

namespace rocks.kfs.Zoom.Migrations
{
    [MigrationNumber( 2, "1.13" )]
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

            RockMigrationHelper.AddOrUpdateEntityAttribute( "rocks.kfs.Zoom.ZoomApp", Rock.SystemGuid.FieldType.ENCRYPTED_TEXT, "", "", "API Key", "", "The API Key for the Zoom Marketplace JWT app to use for KFS Zoom integration elements.", 0, "", "D53A2B48-C8B4-4481-B2AB-139CAF90C78C", "rocks.kfs.ZoomApiKey" );
            RockMigrationHelper.AddOrUpdateEntityAttribute( "rocks.kfs.Zoom.ZoomApp", Rock.SystemGuid.FieldType.ENCRYPTED_TEXT, "", "", "API Secret", "", "The API Secret for the Zoom Marketplace JWT app to use for KFS Zoom integration elements.", 0, "", "CB89B071-4866-41BD-ACB1-1810F5224A82", "rocks.kfs.ZoomApiSecret" );

            // Only add the Reservation attribute if the Reservation table exists
            try
            {
                Sql( @"SELECT TOP 1 Id FROM _com_bemaservices_RoomManagement_Reservation" );
                RockMigrationHelper.AddOrUpdateEntityAttribute( "com.bemaservices.RoomManagement.Model.Reservation", Rock.SystemGuid.FieldType.GROUP_TYPE_GROUP, "", "", "Zoom Notify Group", "", "The Group to notify when a Zoom Room meeting is connected with this reservation.", 0, "", ZoomGuid.Attribute.ROOM_RESERVATION_GROUP_ATTRIBUTE, "rocks.kfs.ZoomNotifyGroup" );
            }
            catch { }

            RockMigrationHelper.AddDefinedType( "Zoom", "Zoom Rooms", "Zoom Rooms available for linking to Rock Locations.", ZoomGuid.DefinedType.ZOOM_ROOM );
            RockMigrationHelper.AddDefinedTypeAttribute( ZoomGuid.DefinedType.ZOOM_ROOM, Rock.SystemGuid.FieldType.TEXT, "Meeting Password", "rocks.kfs.ZoomMeetingPassword", "Zoom meeting password.", 1, "", ZoomGuid.Attribute.ZOOM_MEETING_PASSWORD );
            RockMigrationHelper.AddDefinedTypeAttribute( ZoomGuid.DefinedType.ZOOM_ROOM, Rock.SystemGuid.FieldType.BOOLEAN, "Join Before Host", "rocks.kfs.ZoomJoinBeforeHost", "Zoom meeting's Join Before Host setting.", 2, "false", ZoomGuid.Attribute.ZOOM_MEETING_JOIN_BEFORE_HOST );

            RockMigrationHelper.AddOrUpdateEntityAttribute( "Rock.Model.Location", Rock.SystemGuid.FieldType.DEFINED_VALUE, "", "", "Zoom Room", "", "The Zoom Room associated with this location.", 0, "", ZoomGuid.Attribute.ZOOM_ROOM_LOCATION_ENTITY_ATTRIBUTE, "rocks.kfs.ZoomRoom" );
        }

        /// <summary>
        /// Operations to be performed during the downgrade process.
        /// </summary>
        public override void Down()
        {
            RockMigrationHelper.DeleteAttribute( ZoomGuid.Attribute.ZOOM_ROOM_LOCATION_ENTITY_ATTRIBUTE );
            RockMigrationHelper.DeleteAttribute( ZoomGuid.Attribute.ZOOM_MEETING_PASSWORD );
            RockMigrationHelper.DeleteAttribute( ZoomGuid.Attribute.ZOOM_MEETING_JOIN_BEFORE_HOST );
            RockMigrationHelper.DeleteDefinedType( ZoomGuid.DefinedType.ZOOM_ROOM );
            RockMigrationHelper.DeleteAttribute( "CB89B071-4866-41BD-ACB1-1810F5224A82" );
            RockMigrationHelper.DeleteAttribute( "D53A2B48-C8B4-4481-B2AB-139CAF90C78C" );
            RockMigrationHelper.DeleteAttribute( ZoomGuid.Attribute.ROOM_RESERVATION_GROUP_ATTRIBUTE );
            RockMigrationHelper.DeleteSecurityAuth( "3921542F-476D-48B4-97F5-788709A1F4B4" );
            RockMigrationHelper.DeleteSecurityAuth( "6BC0A6D7-844F-49DF-BE40-5AD45EAB3CEE" );
            RockMigrationHelper.DeleteEntityType( ZoomGuid.EntityType.ZOOM );
        }
    }
}