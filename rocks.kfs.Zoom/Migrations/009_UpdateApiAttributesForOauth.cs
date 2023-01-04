// <copyright>
// Copyright 2023 by Kingdom First Solutions
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
    [MigrationNumber( 9, "1.12.4" )]
    public partial class UpdateApiAttributesForOauth : Migration
    {
        /// <summary>
        /// Operations to be performed during the upgrade process.
        /// </summary>
        public override void Up()
        {
            // Create new Zoom OAuth App attributes
            RockMigrationHelper.UpdateEntityAttribute( "rocks.kfs.Zoom.ZoomApp", Rock.SystemGuid.FieldType.ENCRYPTED_TEXT, "", "", "Zoom OAuth App Account Id", "The Application Id for the Zoom Marketplace Server-to-Server OAuth app to use for KFS Zoom integration elements.", 0, "", "803991F1-9FA1-4AF7-94A7-D1748CBA764F", "rocks.kfs.ZoomAppAccountId" );
            RockMigrationHelper.UpdateEntityAttribute( "rocks.kfs.Zoom.ZoomApp", Rock.SystemGuid.FieldType.ENCRYPTED_TEXT, "", "", "Zoom OAuth App Client Id", "The Client Id for the Zoom Marketplace Server-to-Server OAuth app to use for KFS Zoom integration elements.", 0, "", "D66B02B7-8298-4371-B1FB-B02AC5C552D0", "rocks.kfs.ZoomAppClientId" );
            RockMigrationHelper.UpdateEntityAttribute( "rocks.kfs.Zoom.ZoomApp", Rock.SystemGuid.FieldType.ENCRYPTED_TEXT, "", "", "Zoom OAuth App Client Secret", "The Client Secret for the Zoom Marketplace Server-to-Server OAuth app to use for KFS Zoom integration elements.", 0, "", "9A1CC332-E7CE-4A0A-860E-513A73E8C2F8", "rocks.kfs.ZoomAppClientSecret" );
        }

        /// <summary>
        /// Operations to be performed during the downgrade process.
        /// </summary>
        public override void Down()
        {
            RockMigrationHelper.DeleteAttribute( "803991F1-9FA1-4AF7-94A7-D1748CBA764F" );
            RockMigrationHelper.DeleteAttribute( "D66B02B7-8298-4371-B1FB-B02AC5C552D0" );
            RockMigrationHelper.DeleteAttribute( "9A1CC332-E7CE-4A0A-860E-513A73E8C2F8" );
        }
    }
}