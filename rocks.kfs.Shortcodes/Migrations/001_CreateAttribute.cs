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

namespace rocks.kfs.StepsToCare.Migrations
{
    [MigrationNumber( 1, "1.12.6" )]
    public class CreateAttribute : Migration
    {
        public override void Up()
        {
            RockMigrationHelper.AddGlobalAttribute( Rock.SystemGuid.FieldType.ENCRYPTED_TEXT, null, null, "EWS Username", "The Microsoft Exchange server username to use with EWS managed API.", 0, null, "EDEEE121-BE8F-4B80-A567-80E8BB344346", "rocks.kfs.EWSUsername", true );
            RockMigrationHelper.AddGlobalAttribute( Rock.SystemGuid.FieldType.ENCRYPTED_TEXT, null, null, "EWS Password", "The Microsoft Exchange server password to use with EWS managed API.", 0, null, "0C278C32-C179-4EBE-AF40-F9AD509B8450", "rocks.kfs.EWSPassword", true );
        }

        public override void Down()
        {
            RockMigrationHelper.DeleteAttribute( "EDEEE121-BE8F-4B80-A567-80E8BB344346" );
            RockMigrationHelper.DeleteAttribute( "0C278C32-C179-4EBE-AF40-F9AD509B8450" );
        }
    }
}