// <copyright>
// Copyright 2023 by Kingdom First Solutions
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
using Rock.Data;
using Rock.Model;
using Rock.Plugin;

namespace rocks.kfs.StepsToCare.Migrations
{
    [MigrationNumber( 20, "1.14.0" )]
    public class NewCommunications : Migration
    {
        public override void Up()
        {
            RockMigrationHelper.UpdateSystemCommunication( "Plugins", "Care Need Follow Up", "", "", "", "", "", "Care Need Follow Up Required", @"{{ 'Global' | Attribute:'EmailHeader' }}
    <p>{{ Person.FullName }},</p>
    <p>A Care Need has been flagged for follow up:</p>
    <blockquote><b>Care Need ({{ CareNeed.Id }})</b><br>
    <b>Date:</b> {{ CareNeed.DateEntered }} ({{CareNeed.DateEntered | HumanizeDateTime }}) <br>
    <b>Name:</b> {{ CareNeed.PersonAlias.Person.FullName }}<br>
    <b>Category:</b> {{ CareNeed.Category.Value }}<br>
    <b>Details:</b> {{ CareNeed.Details }}</blockquote>
    <p><a href='{{ 'Global' | Attribute:'InternalApplicationRoot' | ReplaceLast:'/','' }}{{ LinkedPages.CareDashboard }}'>View Care Dashboard</a></p>
{{ 'Global' | Attribute:'EmailFooter' }}
", SystemGuid.SystemCommunication.CARE_NEED_FOLLOWUP_WITH_ACTIONS, true, @"A Care Need has been flagged for follow up
""{{ CareNeed.Details | Truncate:20,'...' }}""
{{ 'Global' | Attribute:'InternalApplicationRoot' | ReplaceLast:'/','' }}{{ LinkedPages.CareDashboard }}" );

            RockMigrationHelper.UpdateSystemCommunication( "Plugins", "Care Touch Needed", "", "", "", "", "", "Your assigned Care Need requires attention", @"{{ 'Global' | Attribute:'EmailHeader' }}
    <p>{{ Person.FullName }},</p>
    <p>Your assigned Care Need requires attention:</p>
    <blockquote><b>Care Need ({{ CareNeed.Id }})</b><br>
    <b>Date:</b> {{ CareNeed.DateEntered }}<br>
    <b>Name:</b> {{ CareNeed.PersonAlias.Person.FullName }}<br>
    <b>Details:</b> {{ CareNeed.Details }}<br>
    <b>Care Touches:</b> {{ TouchCount }}</blockquote>
    <p><a href='{{ 'Global' | Attribute:'InternalApplicationRoot' | ReplaceLast:'/','' }}{{ LinkedPages.CareDashboard }}'>View Care Dashboard</a></p>
{{ 'Global' | Attribute:'EmailFooter' }}
", SystemGuid.SystemCommunication.CARE_NEED_TOUCH_NEEDED_WITH_ACTIONS, true, @"Your assigned Care Need requires attention: 
""{{ CareNeed.Details | Truncate:20,'...' }}""
{{ 'Global' | Attribute:'InternalApplicationRoot' | ReplaceLast:'/','' }}{{ LinkedPages.CareDashboard }}" );

        }

        public override void Down()
        {
            RockMigrationHelper.DeleteSystemCommunication( SystemGuid.SystemCommunication.CARE_NEED_FOLLOWUP_WITH_ACTIONS );
            RockMigrationHelper.DeleteSystemCommunication( SystemGuid.SystemCommunication.CARE_NEED_TOUCH_NEEDED_WITH_ACTIONS );

        }
    }
}