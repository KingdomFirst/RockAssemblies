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
using Rock.Plugin;

namespace rocks.kfs.Zoom.Migrations
{
    [MigrationNumber( 2, "1.12.4" )]
    public partial class Init : Migration
    {
        /// <summary>
        /// Operations to be performed during the upgrade process.
        /// </summary>
        public override void Up()
        {
            // Assign GUID to Zoom and add security
            RockMigrationHelper.UpdateEntityType( "rocks.kfs.Zoom.Zoom", ZoomGuid.EntityType.ZOOM, false, true );
            RockMigrationHelper.AddSecurityAuthForEntityType( "rocks.kfs.Zoom.Zoom", 0, Rock.Security.Authorization.VIEW, true, Rock.SystemGuid.Group.GROUP_ADMINISTRATORS, 0, "6BC0A6D7-844F-49DF-BE40-5AD45EAB3CEE" );
            RockMigrationHelper.AddSecurityAuthForEntityType( "rocks.kfs.Zoom.Zoom", 1, Rock.Security.Authorization.VIEW, true, Rock.SystemGuid.Group.GROUP_EVENT_REGISTRATION_ADMINISTRATORS, 0, "827CD9FB-FA00-4C99-B193-C531EF0F1B0E" );
            RockMigrationHelper.AddSecurityAuthForEntityType( "rocks.kfs.Zoom.Zoom", 2, Rock.Security.Authorization.VIEW, false, null, Rock.Model.SpecialRole.AllUsers.ConvertToInt(), "3921542F-476D-48B4-97F5-788709A1F4B4" );

            RockMigrationHelper.UpdateFieldType( "Zoom Room", "", "rocks.kfs.Zoom", "rocks.kfs.Zoom.Field.Types.ZoomRoomFieldType", ZoomGuid.FieldType.ZOOM_ROOM );
            RockMigrationHelper.UpdateFieldType( "Zoom Room", "", "rocks.kfs.Zoom", "rocks.kfs.Zoom.Field.Types.ZoomMeetingFieldType", ZoomGuid.FieldType.ZOOM_MEETING );

            RockMigrationHelper.AddNewEntityAttribute( "rocks.kfs.Zoom.Zoom", Rock.SystemGuid.FieldType.ENCRYPTED_TEXT, "", "", "Zoom API Key", "", "The API Key for the Zoom Marketplace JWT app to use for KFS Zoom integration elements.", 0, "", "D53A2B48-C8B4-4481-B2AB-139CAF90C78C", "KFSZoomApiKey" );
            RockMigrationHelper.AddNewEntityAttribute( "rocks.kfs.Zoom.Zoom", Rock.SystemGuid.FieldType.ENCRYPTED_TEXT, "", "", "Zoom API Secret", "", "The API Secret for the Zoom Marketplace JWT app to use for KFS Zoom integration elements.", 0, "", "CB89B071-4866-41BD-ACB1-1810F5224A82", "KFSZoomApiSecret" );

            RockMigrationHelper.AddDefinedType( "", "Zoom Rooms", "Zoom Rooms available for linking to Rock Locations.", ZoomGuid.DefinedType.ZOOM_ROOM );
        }

        /// <summary>
        /// Operations to be performed during the downgrade process.
        /// </summary>
        public override void Down()
        {
            RockMigrationHelper.DeleteDefinedType( ZoomGuid.DefinedType.ZOOM_ROOM );
            RockMigrationHelper.DeleteAttribute( "CB89B071-4866-41BD-ACB1-1810F5224A82" );
            RockMigrationHelper.DeleteAttribute( "D53A2B48-C8B4-4481-B2AB-139CAF90C78C" );
            RockMigrationHelper.DeleteFieldType( ZoomGuid.FieldType.ZOOM_MEETING );
            RockMigrationHelper.DeleteFieldType( ZoomGuid.FieldType.ZOOM_ROOM );
            RockMigrationHelper.DeleteSecurityAuth( "3921542F-476D-48B4-97F5-788709A1F4B4" );
            RockMigrationHelper.DeleteSecurityAuth( "827CD9FB-FA00-4C99-B193-C531EF0F1B0E" );
            RockMigrationHelper.DeleteSecurityAuth( "6BC0A6D7-844F-49DF-BE40-5AD45EAB3CEE" );
            RockMigrationHelper.DeleteEntityType( ZoomGuid.EntityType.ZOOM );
        }
    }
}