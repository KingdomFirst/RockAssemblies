// <copyright>
// Copyright 2020 by Kingdom First Solutions
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

namespace rocks.kfs.Eventbrite.Migrations
{
    [MigrationNumber( 2, "1.9.0" )]
    public partial class CreateAttributes : Migration
    {
        /// <summary>
        /// Operations to be performed during the upgrade process.
        /// </summary>
        public override void Up()
        {
            RockMigrationHelper.AddNewEntityAttribute( "rocks.kfs.Eventbrite.Eventbrite", Rock.SystemGuid.FieldType.ENCRYPTED_TEXT, "", "", "EB Access Token", "", "Eventbrite Access Token", 0, "", "BC105CFA-A663-47E3-9B5B-47591D0BC668", "EBAccessToken" );
            RockMigrationHelper.AddNewEntityAttribute( "rocks.kfs.Eventbrite.Eventbrite", Rock.SystemGuid.FieldType.INTEGER, "", "", "EB Organization Id", "", "Eventbrite Organization id", 0, "", "8C3D6321-B6D3-42FA-ABA2-36ECC4C21D9D", "EBOrganizationId" );
        }

        /// <summary>
        /// Operations to be performed during the downgrade process.
        /// </summary>
        public override void Down()
        {
            RockMigrationHelper.DeleteAttribute( "BC105CFA-A663-47E3-9B5B-47591D0BC668" );
            RockMigrationHelper.DeleteAttribute( "8C3D6321-B6D3-42FA-ABA2-36ECC4C21D9D" );
        }
    }
}