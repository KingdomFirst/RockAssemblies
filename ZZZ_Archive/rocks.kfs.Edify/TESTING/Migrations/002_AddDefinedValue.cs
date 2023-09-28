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
using Rock.Plugin;

namespace rocks.kfs.Edify.Migrations
{
    [MigrationNumber( 2, "1.12.3" )]
    public class AddDefinedValue : Migration
    {
        public override void Up()
        {
            RockMigrationHelper.UpdateDefinedValue( SystemGuid.BASE_URL_DEFINED_TYPE, "e3", "Edify Server 3 (e3)", SystemGuid.BASE_URL_SERVER3, true );
        }

        public override void Down()
        {
            RockMigrationHelper.DeleteDefinedValue( SystemGuid.BASE_URL_SERVER3 ); // Server 2
        }
    }
}