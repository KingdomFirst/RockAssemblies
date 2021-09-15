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

namespace rocks.kfs.StepsToCare
{
    [MigrationNumber( 3, "1.12.3" )]
    public class CreateCommunications : Migration
    {
        public override void Up()
        {
            RockMigrationHelper.UpdateSystemCommunication( "Plugins", "Care Need Assigned", "", "", "", "", "", "You have been assigned a Care Need", @"{{ 'Global' | Attribute:'EmailHeader' }}
    <p>{{ Person.FullName }},</p>
    <p>You have been assigned a new Care Need:</p>
    <blockquote><b>Care Need ({{ CareNeed.Id }})</b><br>
    <b>Date:</b> {{ CareNeed.DateEntered }}<br>
    <b>Name:</b> {{ CareNeed.PersonAlias.Person.FullName }}<br>
    <b>Category:</b> {{ CareNeed.Category.Value }}<br>
    <b>Details:</b> {{ CareNeed.Details }}</blockquote>
    <p><a href='{{ 'Global' | Attribute:'PublicApplicationRoot' | ReplaceLast:'/','' }}{{ LinkedPages.CareDashboard }}'>View Care Dashboard</a></p>
{{ 'Global' | Attribute:'EmailFooter' }}
", SystemGuid.SystemCommunication.CARE_NEED_ASSIGNED );

            RockMigrationHelper.UpdateSystemCommunication( "Plugins", "Care Need Follow Up", "", "", "", "", "", "Care Need Follow Up Required", @"{{ 'Global' | Attribute:'EmailHeader' }}
    <p>{{ Person.FullName }},</p>
    <p>A Care Need has been flagged for follow up:</p>
    <blockquote><b>Care Need ({{ CareNeed.Id }})</b><br>
    <b>Date:</b> {{ CareNeed.DateEntered }} ({{CareNeed.DateEntered | HumanizeDateTime }}) <br>
    <b>Name:</b> {{ CareNeed.PersonAlias.Person.FullName }}<br>
    <b>Category:</b> {{ CareNeed.Category.Value }}<br>
    <b>Details:</b> {{ CareNeed.Details }}</blockquote>
    <p><a href='{{ 'Global' | Attribute:'PublicApplicationRoot' | ReplaceLast:'/','' }}{{ LinkedPages.CareDashboard }}'>View Care Dashboard</a></p>
{{ 'Global' | Attribute:'EmailFooter' }}
", SystemGuid.SystemCommunication.CARE_NEED_FOLLOWUP );

            RockMigrationHelper.UpdateSystemCommunication( "Plugins", "Outstanding Care Needs", "", "", "", "", "", "Outstanding Care Needs", @"{{ 'Global' | Attribute:'EmailHeader' }}
    <p>{{ Person.FullName }},</p>
    <p>You have the following care needs assigned to you:</p>
    <p><strong>Id | Date | Name | Category | Details</strong></p>
    <blockquote>
{%- for careNeed in CareNeeds -%}
    <b>({{ careNeed.Id }})</b> |
    {{ careNeed.DateEntered }} |
    {{ careNeed.PersonAlias.Person.FullName }}| 
    {{ careNeed.Category.Value }}
    {{ careNeed.Details }}<br>
{%- endfor -%}
    </blockquote>
    <p><a href='{{ 'Global' | Attribute:'PublicApplicationRoot' | ReplaceLast:'/','' }}{{ LinkedPages.CareDashboard }}'>View Care Dashboard</a></p>
{{ 'Global' | Attribute:'EmailFooter' }}
", SystemGuid.SystemCommunication.CARE_NEED_OUTSTANDING_NEEDS );

            RockMigrationHelper.UpdateSystemCommunication( "Plugins", "Care Touch Needed", "", "", "", "", "", "Your assigned Care Need requires attention", @"{{ 'Global' | Attribute:'EmailHeader' }}
    <p>{{ Person.FullName }},</p>
    <p>Your assigned Care Need requires attention:</p>
    <blockquote><b>Care Need ({{ CareNeed.Id }})</b><br>
    <b>Date:</b> {{ CareNeed.DateEntered }}<br>
    <b>Name:</b> {{ CareNeed.PersonAlias.Person.FullName }}<br>
    <b>Details:</b> {{ CareNeed.Details }}<br>
    <b>Care Touches:</b> {{ CareNeed.TouchCount }}</blockquote>
    <p><a href='{{ 'Global' | Attribute:'PublicApplicationRoot' | ReplaceLast:'/','' }}{{ LinkedPages.CareDashboard }}'>View Care Dashboard</a></p>
{{ 'Global' | Attribute:'EmailFooter' }}
", SystemGuid.SystemCommunication.CARE_NEED_TOUCH_NEEDED );
        }

        public override void Down()
        {
            RockMigrationHelper.DeleteSystemCommunication( SystemGuid.SystemCommunication.CARE_NEED_ASSIGNED );
            RockMigrationHelper.DeleteSystemCommunication( SystemGuid.SystemCommunication.CARE_NEED_FOLLOWUP );
            RockMigrationHelper.DeleteSystemCommunication( SystemGuid.SystemCommunication.CARE_NEED_OUTSTANDING_NEEDS );
            RockMigrationHelper.DeleteSystemCommunication( SystemGuid.SystemCommunication.CARE_NEED_TOUCH_NEEDED );
        }
    }
}