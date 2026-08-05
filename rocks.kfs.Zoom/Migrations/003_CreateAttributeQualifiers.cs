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
    [MigrationNumber( 3, "1.13" )]
    public partial class CreateAttributeQualifiers : Migration
    {
        /// <summary>
        /// Operations to be performed during the upgrade process.
        /// </summary>
        public override void Up()
        {
            var zrDefinedTypeId = new DefinedTypeService( new Rock.Data.RockContext() ).Get( ZoomGuid.DefinedType.ZOOM_ROOM.AsGuid() ).Id;
            RockMigrationHelper.AddAttributeQualifier( ZoomGuid.Attribute.ZOOM_ROOM_LOCATION_ENTITY_ATTRIBUTE, "definedtype", zrDefinedTypeId.ToString(), "D210E7BF-7C3D-4B8E-A8AB-423C97DA6FACf" );
            RockMigrationHelper.AddAttributeQualifier( ZoomGuid.Attribute.ZOOM_ROOM_LOCATION_ENTITY_ATTRIBUTE, "displaydescription", "True", "85D4A488-3F71-49EF-9EBC-28927D4C15A4" );
        }

        /// <summary>
        /// Operations to be performed during the downgrade process.
        /// </summary>
        public override void Down()
        {
        }
    }
}