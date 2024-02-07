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
using Rock.Plugin;

namespace rocks.kfs.StepsToCare.Migrations
{
    [MigrationNumber( 17, "1.14.0" )]
    public class AddColumn : Migration
    {
        public override void Up()
        {
            Sql( @"IF COL_LENGTH('_rocks_kfs_StepsToCare_AssignedPerson', 'Type') IS NULL
                BEGIN
	                ALTER TABLE _rocks_kfs_StepsToCare_AssignedPerson ADD [Type] int NULL 
                END" );
        }

        public override void Down()
        {
            Sql( @"IF COL_LENGTH('_rocks_kfs_StepsToCare_AssignedPerson', 'Type') IS NOT NULL
                BEGIN
	                ALTER TABLE _rocks_kfs_StepsToCare_AssignedPerson DROP COLUMN [Type]
                END" );
        }
    }
}