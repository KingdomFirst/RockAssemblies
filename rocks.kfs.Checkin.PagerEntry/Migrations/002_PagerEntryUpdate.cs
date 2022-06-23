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

namespace rocks.kfs.Checkin.PagerEntry.Migrations
{
    [MigrationNumber( 2, "1.12.0" )]
    public class PagerEntryUpdate : Migration
    {
        /// <summary>
        /// The commands to run to migrate plugin to the specific version
        /// </summary>
        public override void Up()
        {
            // Attribute for BlockType
            //   BlockType: Pager Entry
            //   Category: KFS > Check-in
            //   Attribute: Groups
            RockMigrationHelper.AddOrUpdateBlockTypeAttribute( "176095E9-3BEB-44DC-AADB-B5CCA8F479DB", "BD0D9B57-2A41-4490-89FF-F01DAB7D4904", "Groups", "Groups", "Groups", @"Select the check-in group(s) to utilize this pager entry capability. This capability will be displayed for all groups by default.", 9, @"", "5E4FD9A0-E3DC-4C0E-8FEE-8F15AE47362F" );

            // Add/Update HtmlContent for Block: Family Attributes
            RockMigrationHelper.UpdateHtmlContentBlock( "83763A73-794F-49D9-BC1E-DD34AC628BFB", @"<div class=""panel panel-block"">
    <div class=""panel-heading""><strong>Family Attributes</strong></div>
    <div class=""panel-body"">
        <dl class=""attribute-value-container-display mb-1"">
            {% assign person = PageParameter.PersonId | PersonById %}
{%- attribute where:'Key == ""rocks.kfs.PagerNumber"" && EntityTypeId == 16' -%}
                {%- assign attributeItem = attributeItems | First -%}
                {%- if attributeItem and attributeItem != empty -%}
                {%- assign midnightToday = ""Now"" | Date:""M/dd/yyyy 12:00 A\M"" %}{% assign endToday = ""Now"" | Date:""M/dd/yyyy 11:59 P\M"" -%}
                {%- attributevalue where:'AttributeId == {{ attributeItem.Id }} && EntityId == {{ person.PrimaryFamily.Id }} && ModifiedDateTime >= ""{{ midnightToday }}"" && ModifiedDateTime <= ""{{ endToday }}""' -%}
                    {% assign pagerNumber = attributevalueItems | First -%}
                {%- endattributevalue -%}
                {%- endif -%}
            {%- endattribute -%}
            <dt>Pager Number</dt>
            <dd>{% if pagerNumber and pagerNumber != empty %}{{ pagerNumber.Value }} ({{ pagerNumber.ModifiedDateTime | Date:""M/dd/yyyy hh:mm tt"" }}){% else %}Not Found{% endif %}</dd>
        </dl>
    </div>
</div>
", "AB4CA531-9095-40AA-A245-A6454C0A6CC7" );

            // Add/Update HtmlContent for Block: Family Attributes
            RockMigrationHelper.UpdateHtmlContentBlock( "AC41EA0D-2385-4797-9EAB-B8E1990B224E", @"<div class=""panel panel-block"">
    <div class=""panel-heading""><strong>Family Attributes</strong></div>
    <div class=""panel-body"">
        <dl class=""attribute-value-container-display mb-1"">
            {% assign person = PageParameter.Person | PersonByGuid %}
            {%- attribute where:'Key == ""rocks.kfs.PagerNumber"" && EntityTypeId == 16' -%}
                {%- assign attributeItem = attributeItems | First -%}
                {%- if attributeItem and attributeItem != empty -%}
                {%- assign midnightToday = ""Now"" | Date:""M/dd/yyyy 12:00 A\M"" %}{% assign endToday = ""Now"" | Date:""M/dd/yyyy 11:59 P\M"" -%}
                {%- attributevalue where:'AttributeId == {{ attributeItem.Id }} && EntityId == {{ person.PrimaryFamily.Id }} && ModifiedDateTime >= ""{{ midnightToday }}"" && ModifiedDateTime <= ""{{ endToday }}""' -%}
                    {% assign pagerNumber = attributevalueItems | First -%}
                {%- endattributevalue -%}
                {%- endif -%}
            {%- endattribute -%}
            <dt>Pager Number</dt>
            <dd>{% if pagerNumber and pagerNumber != empty %}{{ pagerNumber.Value }} ({{ pagerNumber.ModifiedDateTime | Date:""M/dd/yyyy hh:mm tt"" }}){% else %}Not Found{% endif %}</dd>
        </dl>
    </div>
</div>", "279D56AC-4399-4B91-8187-47D82C6C5CB9" );
        }

        /// <summary>
        /// The commands to undo a migration from a specific version
        /// </summary>
        public override void Down()
        {
            // Add/Update HtmlContent for Block: Family Attributes
            RockMigrationHelper.UpdateHtmlContentBlock( "83763A73-794F-49D9-BC1E-DD34AC628BFB", @"<div class=""panel panel-block"">
    <div class=""panel-heading""><strong>Family Attributes</strong></div>
    <div class=""panel-body"">
        <dl class=""attribute-value-container-display mb-1"">
            {% assign person = PageParameter.PersonId | PersonById %}
{%- attribute where:'Key == ""rocks.kfs.PagerNumber"" && EntityTypeId == 16' -%}
                {%- assign attributeItem = attributeItems | First -%}
                {%- if attributeItem and attributeItem != empty -%}
                {%- attributevalue where:'AttributeId == {{ attributeItem.Id }} && EntityId == {{ person.PrimaryFamily.Id }}' -%}
                    {% assign pagerNumber = attributevalueItems | First -%}
                {%- endattributevalue -%}
                {%- endif -%}
            {%- endattribute -%}
            <dt>Pager Number</dt>
            <dd>{% if pagerNumber and pagerNumber != empty %}{{ pagerNumber.Value }} ({{ pagerNumber.ModifiedDateTime | Date:""M/dd/yyyy hh:mm tt"" }}){% else %}Not Found{% endif %}</dd>
        </dl>
    </div>
</div>", "AB4CA531-9095-40AA-A245-A6454C0A6CC7" );

            // Add/Update HtmlContent for Block: Family Attributes
            RockMigrationHelper.UpdateHtmlContentBlock( "AC41EA0D-2385-4797-9EAB-B8E1990B224E", @"<div class=""panel panel-block"">
    <div class=""panel-heading""><strong>Family Attributes</strong></div>
    <div class=""panel-body"">
        <dl class=""attribute-value-container-display mb-1"">
            {% assign person = PageParameter.Person | PersonByGuid %}
            {%- attribute where:'Key == ""rocks.kfs.PagerNumber"" && EntityTypeId == 16' -%}
                {%- assign attributeItem = attributeItems | First -%}
                {%- if attributeItem and attributeItem != empty -%}
                {%- attributevalue where:'AttributeId == {{ attributeItem.Id }} && EntityId == {{ person.PrimaryFamily.Id }}' -%}
                    {% assign pagerNumber = attributevalueItems | First -%}
                {%- endattributevalue -%}
                {%- endif -%}
            {%- endattribute -%}
            <dt>Pager Number</dt>
            <dd>{% if pagerNumber and pagerNumber != empty %}{{ pagerNumber.Value }} ({{ pagerNumber.ModifiedDateTime | Date:""M/dd/yyyy hh:mm tt"" }}){% else %}Not Found{% endif %}</dd>
        </dl>
    </div>
</div>", "279D56AC-4399-4B91-8187-47D82C6C5CB9" );

            // Attribute for BlockType
            //   BlockType: Pager Entry
            //   Category: KFS > Check-in
            //   Attribute: Groups
            RockMigrationHelper.DeleteAttribute( "5E4FD9A0-E3DC-4C0E-8FEE-8F15AE47362F" );
        }
    }
}