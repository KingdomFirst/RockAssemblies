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

namespace rocks.kfs.StepsToCare.Migrations
{
    [MigrationNumber( 13, "1.12.3" )]
    public class NewPermissions : Migration
    {
        public override void Up()
        {
            RockMigrationHelper.AddSecurityAuthForBlock( "EADBE3F0-F64B-4583-B49D-F0031BBC929F", 0, "CompleteNeeds", true, Rock.SystemGuid.Group.GROUP_ADMINISTRATORS, Rock.Model.SpecialRole.None, "C9919614-E7BD-4857-8594-312D358FF7C8" );
            RockMigrationHelper.AddSecurityAuthForBlock( "F953C5EF-6504-45F9-81A8-063518B7AB61", 0, "UpdateStatus", true, Rock.SystemGuid.Group.GROUP_ADMINISTRATORS, Rock.Model.SpecialRole.None, "B9AABE23-7B5F-49E2-AFC4-BA8F874E22B9" );
        }

        public override void Down()
        {
            RockMigrationHelper.DeleteSecurityAuth( "C9919614-E7BD-4857-8594-312D358FF7C8" );
            RockMigrationHelper.DeleteSecurityAuth( "B9AABE23-7B5F-49E2-AFC4-BA8F874E22B9" );
        }
    }
}