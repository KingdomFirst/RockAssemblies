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
using Rock.Plugin;

namespace rocks.kfs.StepsToCare.Migrations
{
    [MigrationNumber( 9, "1.12.3" )]
    public class CreatePermissions : Migration
    {
        public override void Up()
        {
            RockMigrationHelper.AddSecurityAuthForBlock( "EADBE3F0-F64B-4583-B49D-F0031BBC929F", 0, "ViewAll", true, Rock.SystemGuid.Group.GROUP_ADMINISTRATORS, Rock.Model.SpecialRole.None, "B841217C-327B-4753-BA76-C5ED1F2748EE" );
            RockMigrationHelper.AddSecurityAuthForBlock( "EADBE3F0-F64B-4583-B49D-F0031BBC929F", 0, "CareWorkers", true, Rock.SystemGuid.Group.GROUP_ADMINISTRATORS, Rock.Model.SpecialRole.None, "037202FE-5A6B-4D3B-9C1D-6F5AD65A5D06" );
        }

        public override void Down()
        {
            RockMigrationHelper.DeleteSecurityAuth( "037202FE-5A6B-4D3B-9C1D-6F5AD65A5D06" );
            RockMigrationHelper.DeleteSecurityAuth( "B841217C-327B-4753-BA76-C5ED1F2748EE" );
        }
    }
}