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
using System.Collections.Generic;
using Rock;
using Rock.Plugin;

namespace rocks.kfs.StepsToCare
{
    [MigrationNumber( 7, "1.12.3" )]
    public class CreateAttribute : Migration
    {
        public override void Up()
        {
            RockMigrationHelper.AddOrUpdatePersonAttributeByGuid( Rock.SystemGuid.FieldType.SINGLE_SELECT, new List<string>(), "Steps to Care Notification", "Steps to Care Notification", "rocks_kfs_StepsToCare_Notification", "", "Choose how you would like to receive Steps to Care notifications. Default: Email", 0, "Email", SystemGuid.PersonAttribute.NOTIFICATION );
            RockMigrationHelper.AddAttributeQualifier( SystemGuid.PersonAttribute.NOTIFICATION, "values", "None^None,Email^Email,SMS^SMS,Both^Email & SMS", "A8E56BDB-8523-4BEA-9707-5C1463750BBB" );
            RockMigrationHelper.AddAttributeQualifier( SystemGuid.PersonAttribute.NOTIFICATION, "fieldtype", "rb", "C56B40EA-1594-448C-9742-50B3477EC04A" );

            Sql( string.Format( "UPDATE [Attribute] SET IsSystem = 1 WHERE [Guid] = '{0}'", SystemGuid.PersonAttribute.NOTIFICATION ) );
        }

        public override void Down()
        {
            RockMigrationHelper.DeleteAttribute( SystemGuid.PersonAttribute.NOTIFICATION );
        }
    }
}