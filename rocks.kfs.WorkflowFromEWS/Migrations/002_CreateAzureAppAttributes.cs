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
using System.Collections.Generic;
using Rock;
using Rock.Plugin;

namespace rocks.kfs.Shortcodes.Migrations
{
    [MigrationNumber( 2, "1.12.6" )]
    public class CreateAzureAppAttributes : Migration
    {
        public override void Up()
        {
            RockMigrationHelper.AddGlobalAttribute( Rock.SystemGuid.FieldType.ENCRYPTED_TEXT, null, null, "EWS Azure Application ID", "The Application (client) ID in Microsoft Azure for the registered application to use for accessing the EWS managed API. Value should be in the format of a guid.", 0, null, "467C0F95-BAB9-49E9-B675-6AD59EB221D8", "rocks.kfs.EWSAppApplicationId", true );
            RockMigrationHelper.AddGlobalAttribute( Rock.SystemGuid.FieldType.ENCRYPTED_TEXT, null, null, "EWS Azure Tenant ID", "The Directory (tenant) ID in Microsoft Azure for the registered application to use for accessing the EWS managed API. Value should be in the format of a guid.", 0, null, "12D2D842-8806-402D-BCD7-E9E81D9FDEAB", "rocks.kfs.EWSAppTenantId", true );
            RockMigrationHelper.AddGlobalAttribute( Rock.SystemGuid.FieldType.ENCRYPTED_TEXT, null, null, "EWS Azure Secret", "The Secret Value in Microsoft Azure for the registered application to use for accessing the EWS managed API.", 0, null, "8CDE7476-506B-4F39-B2BE-5E4102F3C9F3", "rocks.kfs.EWSAppSecret", true );
            RockMigrationHelper.AddAttributeQualifier( "8CDE7476-506B-4F39-B2BE-5E4102F3C9F3","ispassword","true", "B70BC64F-3DCD-402C-A24D-38A8CD0428BD" );
        }

        public override void Down()
        {
            RockMigrationHelper.DeleteAttribute( "467C0F95-BAB9-49E9-B675-6AD59EB221D8" );
            RockMigrationHelper.DeleteAttribute( "12D2D842-8806-402D-BCD7-E9E81D9FDEAB" );
            RockMigrationHelper.DeleteAttribute( "8CDE7476-506B-4F39-B2BE-5E4102F3C9F3" );
        }
    }
}