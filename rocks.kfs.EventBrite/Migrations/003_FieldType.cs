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
    [MigrationNumber( 3, "1.9.0" )]
    public partial class FieldType : Migration
    {
        /// <summary>
        /// Operations to be performed during the upgrade process.
        /// </summary>
        public override void Up()
        {
            RockMigrationHelper.UpdateFieldType( "Eventbrite Person", "", "rocks.kfs.Eventbrite", "rocks.kfs.Eventbrite.Field.Types.EventbritePersonFieldType", EBGuid.FieldType.EVENTBRITE_PERSON );
        }

        /// <summary>
        /// Operations to be performed during the downgrade process.
        /// </summary>
        public override void Down()
        {
          
        }
    }
}