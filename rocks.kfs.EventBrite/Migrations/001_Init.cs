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
    [MigrationNumber( 1, "1.9.0" )]
    public partial class Init : Migration
    {
        /// <summary>
        /// Operations to be performed during the upgrade process.
        /// </summary>
        public override void Up()
        {
            // Assign GUID to Eventbrite and add security
            RockMigrationHelper.UpdateEntityType( "rocks.kfs.Eventbrite.Eventbrite", EBGuid.EntityType.EVENTBRITE, false, true );
            RockMigrationHelper.AddSecurityAuthForEntityType( "rocks.kfs.Eventbrite.Eventbrite", 0, Rock.Security.Authorization.VIEW, true, Rock.SystemGuid.Group.GROUP_ADMINISTRATORS, 0, "C0B244D2-F2F4-4AFE-9081-4AB119BB7EB3" );
            RockMigrationHelper.AddSecurityAuthForEntityType( "rocks.kfs.Eventbrite.Eventbrite", 1, Rock.Security.Authorization.VIEW, true, Rock.SystemGuid.Group.GROUP_EVENT_REGISTRATION_ADMINISTRATORS, 0, "760E19ED-B2E1-46B8-8004-25777B1AF4E0" );
            RockMigrationHelper.AddSecurityAuthForEntityType( "rocks.kfs.Eventbrite.Eventbrite", 2, Rock.Security.Authorization.VIEW, false, null, Rock.Model.SpecialRole.AllUsers.ConvertToInt(), "DEA223C7-3691-464F-8B74-6408C7A0ACD1" );

            RockMigrationHelper.UpdateFieldType( "Eventbrite Event", "", "rocks.kfs.Eventbrite", "rocks.kfs.Eventbrite.Field.Types.EventbriteEventFieldType", EBGuid.FieldType.EVENTBRITE_EVENT );
            RockMigrationHelper.UpdateFieldType( "Eventbrite Person", "", "rocks.kfs.Eventbrite", "rocks.kfs.Eventbrite.Field.Types.EventbritePersonFieldType", EBGuid.FieldType.EVENTBRITE_PERSON );

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
            RockMigrationHelper.DeleteFieldType( EBGuid.FieldType.EVENTBRITE_PERSON );
            RockMigrationHelper.DeleteFieldType( EBGuid.FieldType.EVENTBRITE_EVENT );
            RockMigrationHelper.DeleteSecurityAuth( "DEA223C7-3691-464F-8B74-6408C7A0ACD1" );
            RockMigrationHelper.DeleteSecurityAuth( "760E19ED-B2E1-46B8-8004-25777B1AF4E0" );
            RockMigrationHelper.DeleteSecurityAuth( "C0B244D2-F2F4-4AFE-9081-4AB119BB7EB3" );
            RockMigrationHelper.DeleteEntityType( EBGuid.EntityType.EVENTBRITE );
        }
    }
}