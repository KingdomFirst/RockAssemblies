// <copyright>
// Copyright 2019 by Kingdom First Solutions
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

namespace rocks.kfs.FundraisingParticipantSummary.Migrations
{
    [MigrationNumber( 1, "1.8.0" )]
    internal class AddSystemEmail : Migration
    {
        /// <summary>
        /// The commands to run to migrate plugin to the specific version
        /// </summary>
        public override void Up()
        {
            // Fundraising Participant Summary
            RockMigrationHelper.UpdateSystemEmail( "Plugins", "Fundraising Participant Summary", "", "", "", "", "", "Summary of Donations for {{ BeginDateTime | Date:'M/d/yyyy' }} - {{ 'Now' | Date:'M/d/yyyy' }}", @"{{ 'Global' | Attribute:'EmailHeader' }}
{% assign percentWidth = PercentMet -%}
{% if percentWidth > 100 -%}
  {% assign percentWidth = 100 -%}
{% endif -%}
{% assign percentRounded = PercentMet | Floor -%}

<p>{{ CurrentPerson.NickName }},</p>
<p>Below you will find a summary of donations for {{ GroupMember.Group.Name }} from {{ BeginDateTime | Date:'M/d/yyyy' }} - {{ 'Now' | Date:'M/d/yyyy' }}.</p>

 <div class=""well margin-t-md"" style=""-webkit-box-sizing: border-box;-moz-box-sizing: border-box;box-sizing: border-box;min-height: 20px;padding: 19px;margin-bottom: 20px;background-color: #f2f2f2;border: 1px solid #e0e0e0;border-radius: 4px;-webkit-box-shadow: inset 0 1px 1px rgba(0,0,0,0.05);box-shadow: inset 0 1px 1px rgba(0,0,0,0.05);margin-top: 15px !important;"">
    <div class=""row"" style=""-webkit-box-sizing: border-box;-moz-box-sizing: border-box;box-sizing: border-box;margin-right: -15px;margin-left: -15px;"">
        <div class=""col-md-12"" style=""-webkit-box-sizing: border-box;-moz-box-sizing: border-box;box-sizing: border-box;position: relative;min-height: 1px;padding-right: 15px;padding-left: 15px;width: 100%;"">

            <label style=""-webkit-box-sizing: border-box;-moz-box-sizing: border-box;box-sizing: border-box;display: inline-block;max-width: 100%;margin-bottom: 5px;font-weight: 700;"">
                Fundraising Progress
            </label>
            <label class=""pull-right"" style=""-webkit-box-sizing: border-box;-moz-box-sizing: border-box;box-sizing: border-box;display: inline-block;max-width: 100%;margin-bottom: 5px;font-weight: 700;float: right !important;"">                    
                {% if AmountLeft >= 0 %}
                    {{ AmountLeft | FormatAsCurrency }}
                {% else %}{% assign amountLeftPositive = AmountLeft | Times:-1 %}
                    {{ FundraisingGoal | Plus:amountLeftPositive | FormatAsCurrency }} / {{ FundraisingGoal | FormatAsCurrency }}
                {% endif %}                               
            </label>
            <div class=""progress"" style=""-webkit-box-sizing: border-box;-moz-box-sizing: border-box;box-sizing: border-box;height: 20px;margin-bottom: 20px;overflow: hidden;background-color: #f5f5f5;border-radius: 4px;-webkit-box-shadow: inset 0 1px 2px rgba(0,0,0,0.1);box-shadow: inset 0 1px 2px rgba(0,0,0,0.1);"">
                <div class=""progress-bar"" role=""progressbar"" aria-valuenow=""{{ percentWidth }}"" aria-valuemin=""0"" aria-valuemax=""100"" style=""width: {{ percentWidth }}%;-webkit-box-sizing: border-box;-moz-box-sizing: border-box;box-sizing: border-box;float: left;height: 100%;font-size: 12px;line-height: 20px;color: #fff;text-align: center;background-color: #428bca;-webkit-box-shadow: inset 0 -1px 0 rgba(0,0,0,0.15);box-shadow: inset 0 -1px 0 rgba(0,0,0,0.15);-webkit-transition: width 0.6s ease;-o-transition: width 0.6s ease;transition: width 0.6s ease;"">
                <span class=""sr-only"" style=""-webkit-box-sizing: border-box;-moz-box-sizing: border-box;box-sizing: border-box;border: 0;clip: rect(0,0,0,0);height: 1px;margin: -1px;overflow: hidden;padding: 0;position: absolute;width: 1px;"">{{ percentRounded }}% Complete</span>
                </div>
            </div>
        </div>
    </div>
    <div class=""row"" style=""-webkit-box-sizing: border-box;-moz-box-sizing: border-box;box-sizing: border-box;margin-right: -15px;margin-left: -15px;"">
        <div class=""col-md-12"" style=""-webkit-box-sizing: border-box;-moz-box-sizing: border-box;box-sizing: border-box;position: relative;min-height: 1px;padding-right: 15px;padding-left: 15px;width: 100%; height: 30px;"">
            <div class=""actions pull-right"" style=""-webkit-box-sizing: border-box;-moz-box-sizing: border-box;box-sizing: border-box;float: right !important;"">
                <a href=""{{ 'Global' | Attribute:'PublicApplicationRoot' }}/page/472?GroupId={{ GroupMember.GroupId }}&amp;GroupMemberId={{ GroupMember.Id }}"" class=""btn btn-sm btn-primary"" style=""-webkit-box-sizing: border-box;-moz-box-sizing: border-box;box-sizing: border-box;background-color: #428bca;color: #fff;text-decoration: underline;display: inline-block;margin-bottom: 0;font-weight: normal;text-align: center;white-space: nowrap;vertical-align: middle;touch-action: manipulation;cursor: pointer;background-image: none;border: 1px solid transparent;padding: 5px 10px;font-size: 12px;line-height: 1.5;border-radius: 3px;-webkit-user-select: none;-moz-user-select: none;-ms-user-select: none;user-select: none;border-color: #357ebd;"">Make Payment</a>
            </div>
        </div>
    </div>
</div>


            <br style=""-webkit-box-sizing: border-box;-moz-box-sizing: border-box;box-sizing: border-box;"">
{%- assign contributionsSize = Contributions | Size %}{% assign colspan = 2 -%}
{%- if ShowAddress %}{% assign colspan = colspan | Plus:1 %}{% endif -%}
{%- if ShowAmount %}{% assign colspan = colspan | Plus:1 %}{% endif -%}
 <div class=""table-responsive table-no-border hide-for-small"" style=""-webkit-box-sizing: border-box;-moz-box-sizing: border-box;box-sizing: border-box;min-height: 0.01%;overflow-x: auto;width: 100%;overflow-y: hidden;-webkit-overflow-scrolling: touch;-ms-overflow-style: -ms-autohiding-scrollbar;border: 0;"">
    <table class=""grid-table table table-condensed table-light"" style=""-webkit-box-sizing: border-box;-moz-box-sizing: border-box;box-sizing: border-box;border-collapse: collapse !important;border-spacing: 0;background-color: #fff;width: 100%;max-width: 100%;margin-bottom: 0;"">
	    <thead style=""-webkit-box-sizing: border-box;-moz-box-sizing: border-box;box-sizing: border-box;display: table-header-group;"">
		    <tr align=""left"" data-original-title="""" title="""" style=""-webkit-box-sizing: border-box;-moz-box-sizing: border-box;box-sizing: border-box;page-break-inside: avoid;"">
			    <th data-priority=""1"" scope=""col"" style=""-webkit-box-sizing: border-box;-moz-box-sizing: border-box;box-sizing: border-box;padding: 12px;text-align: left;font-weight: 600;color: #333;background-color: #fff !important;line-height: 1.428571429;vertical-align: bottom;border-top: 1px solid #ddd;border-bottom: 2px solid #ddd;font-size: 14px;border-color: #999;display: table-cell;white-space: nowrap;"">Name</th>
                {% if ShowAddress %}<th data-priority=""1"" scope=""col"" style=""-webkit-box-sizing: border-box;-moz-box-sizing: border-box;box-sizing: border-box;padding: 12px;text-align: left;font-weight: 600;color: #333;background-color: #fff !important;line-height: 1.428571429;vertical-align: bottom;border-top: 1px solid #ddd;border-bottom: 2px solid #ddd;font-size: 14px;border-color: #999;display: table-cell;white-space: nowrap;"">Address</th>{% endif %}
                <th data-priority=""1"" scope=""col"" style=""-webkit-box-sizing: border-box;-moz-box-sizing: border-box;box-sizing: border-box;padding: 12px;text-align: left;font-weight: 600;color: #333;background-color: #fff !important;line-height: 1.428571429;vertical-align: bottom;border-top: 1px solid #ddd;border-bottom: 2px solid #ddd;font-size: 14px;border-color: #999;display: table-cell;white-space: nowrap;"">Date</th>
                {% if ShowAmount %}<th data-priority=""1"" align=""right"" scope=""col"" style=""-webkit-box-sizing: border-box;-moz-box-sizing: border-box;box-sizing: border-box;padding: 12px;text-align: left;font-weight: 600;color: #333;background-color: #fff !important;line-height: 1.428571429;vertical-align: bottom;border-top: 1px solid #ddd;border-bottom: 2px solid #ddd;font-size: 14px;border-color: #999;display: table-cell;white-space: nowrap;"">Amount</th>{% endif %}
			</tr>
		</thead>
        <tbody style=""-webkit-box-sizing: border-box;-moz-box-sizing: border-box;box-sizing: border-box;"">
{%- for contribution in Contributions -%}
		    <tr align=""left"" data-row-index=""0"" data-original-title="""" title="""" style=""-webkit-box-sizing: border-box;-moz-box-sizing: border-box;box-sizing: border-box;page-break-inside: avoid;"">
				<td data-priority=""1"" style=""-webkit-box-sizing: border-box;-moz-box-sizing: border-box;box-sizing: border-box;padding: 12px;line-height: 1.428571429;vertical-align: top;border-top: 1px solid #ddd;color: #333;display: table-cell;background-color: #fff !important;"">{% if contribution.Transaction.ShowAsAnonymous %}Anonymous{% else %}{{ contribution.Transaction.AuthorizedPersonAlias.Person.FullName }}{% endif %}</td>
                {% if ShowAddress %}<td data-priority=""1"" style=""-webkit-box-sizing: border-box;-moz-box-sizing: border-box;box-sizing: border-box;padding: 12px;line-height: 1.428571429;vertical-align: top;border-top: 1px solid #ddd;color: #333;display: table-cell;background-color: #fff !important;"">{% if contribution.Transaction.ShowAsAnonymous %}&nbsp;{% else %}{{ contribution.Transaction.AuthorizedPersonAlias.Person | Address:'Mailing' }}{% endif %}</td>{% endif %}
                <td align=""left"" data-priority=""1"" style=""-webkit-box-sizing: border-box;-moz-box-sizing: border-box;box-sizing: border-box;padding: 12px;line-height: 1.428571429;vertical-align: top;border-top: 1px solid #ddd;color: #333;display: table-cell;background-color: #fff !important;"">{{ contribution.Transaction.TransactionDateTime }}</td>
                {% if ShowAmount %}<td align=""right"" data-priority=""1"" style=""-webkit-box-sizing: border-box;-moz-box-sizing: border-box;box-sizing: border-box;padding: 12px;line-height: 1.428571429;vertical-align: top;border-top: 1px solid #ddd;color: #333;display: table-cell;background-color: #fff !important;"">{{ 'Global' | Attribute:'CurrencySymbol' }}{{ contribution.Amount }}</td>{% endif %}
			</tr>
{%- endfor -%}
{%- if contributionsSize == 0 %}<tr><td colspan=""{{ colspan }}"" style=""-webkit-box-sizing: border-box;-moz-box-sizing: border-box;box-sizing: border-box;padding: 12px;line-height: 1.428571429;vertical-align: top;border-top: 1px solid #ddd;color: #333;display: table-cell;background-color: #fff !important;"">No donations for {{ BeginDateTime | Date:'M/d/yyyy' }} - {{ 'Now' | Date:'M/d/yyyy' }}.</td></tr>{% endif -%}
		</tbody>
	</table>
</div>
<div class=""show-for-small"" style=""display: none"">
{%- for contribution in Contributions -%}
<div class=""panel panel-default"" style=""-webkit-box-sizing: border-box;-moz-box-sizing: border-box;box-sizing: border-box;margin-bottom: 20px;background-color: #fff;border: 1px solid transparent;border-radius: 4px;-webkit-box-shadow: 0 1px 1px rgba(0,0,0,0.05);box-shadow: 0 1px 1px rgba(0,0,0,0.05);border-color: #ddd;"">
		<div class=""panel-heading"" style=""-webkit-box-sizing: border-box;-moz-box-sizing: border-box;box-sizing: border-box;padding: 10px 15px;border-bottom: 1px solid transparent;border-top-left-radius: 3px;border-top-right-radius: 3px;color: #333;background-color: #f5f5f5;border-color: #ddd;"">
        {% if contribution.Transaction.ShowAsAnonymous %}Anonymous{% else %}{{ contribution.Transaction.AuthorizedPersonAlias.Person.FullName }}{% endif %}</div>
    <div class=""panel-body"" style=""-webkit-box-sizing: border-box;-moz-box-sizing: border-box;box-sizing: border-box;padding: 15px;"">
    {% if ShowAddress and contribution.Transaction.ShowAsAnonymous == false %}<p>{{ contribution.Transaction.AuthorizedPersonAlias.Person | Address:'Mailing' }}</p>{% endif %}
            <p>{{ contribution.Transaction.TransactionDateTime }}</p>
            {% if ShowAmount %}<p>{{ 'Global' | Attribute:'CurrencySymbol' }}{{ contribution.Amount }}</p>{% endif %}
        </div>
	</div>
{%- endfor -%}
</div>
{{ 'Global' | Attribute:'EmailFooter' }}", "DE953A2C-1AE5-406D-B36C-B8486147D77B" );

        }

        /// <summary>
        /// The commands to undo a migration from a specific version
        /// </summary>
        public override void Down()
        {
            // Fundraising Participant Summary
            RockMigrationHelper.DeleteSystemEmail( "DE953A2C-1AE5-406D-B36C-B8486147D77B" );
        }

    }
}
