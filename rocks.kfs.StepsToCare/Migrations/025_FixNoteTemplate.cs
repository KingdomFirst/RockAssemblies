// <copyright>
// Copyright 2026 by Kingdom First Solutions
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
    [MigrationNumber( 25, "1.16.0" )]
    public class FixNoteTemplate : Migration
    {
        public override void Up()
        {
            Sql( $@"UPDATE _rocks_kfs_StepsToCare_NoteTemplate SET Icon = 'fa fa-comment' WHERE Icon = 'far fa-comment'" );
        }

        public override void Down()
        {
            Sql( $@"UPDATE _rocks_kfs_StepsToCare_NoteTemplate SET Icon = 'far fa-comment' WHERE Icon = 'fa fa-comment'" );
        }
    }
}