// <copyright>
// Copyright 2024 by Kingdom First Solutions
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
    [MigrationNumber( 22, "1.15.0" )]
    public class UpdateCommunications : Migration
    {
        public override void Up()
        {
            RockMigrationHelper.UpdateSystemCommunication( "Plugins", "Care Need Follow Up with Actions", "", "", "", "", "", "Care Need Follow Up Required", @"{{ 'Global' | Attribute:'EmailHeader' }}
 {% assign noteTemplateSize = NoteTemplates | Size | Plus:1 %}{% assign personToken = Person | PersonTokenCreate:2880,noteTemplateSize %}
   <p>{{ Person.FullName }},</p>
    <p>A Care Need has been flagged for follow up:</p>
    <blockquote><b>Care Need ({{ CareNeed.Id }})</b><br>
    <b>Date:</b> {{ CareNeed.DateEntered }} ({{CareNeed.DateEntered | HumanizeDateTime }}) <br>
    <b>Name:</b> {{ CareNeed.PersonAlias.Person.FullName }}<br>
    <b>Category:</b> {{ CareNeed.Category.Value }}<br>
    <b>Details:</b> {{ CareNeed.Details }}</blockquote>

<table width=""100%"">
<tr>
    <td>
    <table align=""left"" style=""width: 29%; min-width: 190px; margin-bottom: 12px;"" cellpadding=""0"" cellspacing=""0"">
     <tr>
       <td>
    		<div><!--[if mso]>
    		  <v:roundrect xmlns:v=""urn:schemas-microsoft-com:vml"" xmlns:w=""urn:schemas-microsoft-com:office:word"" href=""{{ 'Global' | Attribute:'InternalApplicationRoot' | ReplaceLast:'/','' }}{{ LinkedPages.CareDashboard }}?rckipid={{ personToken }}"" style=""height:38px;v-text-anchor:middle;width:175px;"" arcsize=""11%"" strokecolor=""#269abc"" fillcolor=""#31b0d5"">
    			<w:anchorlock/>
    			<center style=""color:#ffffff;font-family:sans-serif;font-size:14px;font-weight:normal;"">View Care Dashboard</center>
    		  </v:roundrect>
    		<![endif]--><a href=""{{ 'Global' | Attribute:'InternalApplicationRoot' | ReplaceLast:'/','' }}{{ LinkedPages.CareDashboard }}?rckipid={{ personToken }}""
    		style=""background-color:#31b0d5;border:1px solid #269abc;border-radius:4px;color:#ffffff;display:inline-block;font-family:sans-serif;font-size:14px;font-weight:normal;line-height:38px;text-align:center;text-decoration:none;width:175px;-webkit-text-size-adjust:none;mso-hide:all;"">View Care Dashboard</a></div>

    	</td>
     </tr>
    </table>
    <table align=""left"" style=""width: 29%; min-width: 190px; margin-bottom: 12px;"" cellpadding=""0"" cellspacing=""0"">
     <tr>
       <td>
    		<div><!--[if mso]>
    		  <v:roundrect xmlns:v=""urn:schemas-microsoft-com:vml"" xmlns:w=""urn:schemas-microsoft-com:office:word"" href=""{{ 'Global' | Attribute:'InternalApplicationRoot' | ReplaceLast:'/','' }}{{ LinkedPages.CareDashboard }}?QuickNote=-1&CareNeed={{ CareNeed.Id }}&rckipid={{ personToken }}"" style=""height:38px;v-text-anchor:middle;width:175px;"" arcsize=""11%"" strokecolor=""#269abc"" fillcolor=""#31b0d5"">
    			<w:anchorlock/>
    			<center style=""color:#ffffff;font-family:sans-serif;font-size:14px;font-weight:normal;"">Take Action</center>
    		  </v:roundrect>
    		<![endif]--><a href=""{{ 'Global' | Attribute:'InternalApplicationRoot' | ReplaceLast:'/','' }}{{ LinkedPages.CareDashboard }}?QuickNote=-1&CareNeed={{ CareNeed.Id }}&rckipid={{ personToken }}""
    		style=""background-color:#31b0d5;border:1px solid #269abc;border-radius:4px;color:#ffffff;display:inline-block;font-family:sans-serif;font-size:14px;font-weight:normal;line-height:38px;text-align:center;text-decoration:none;width:175px;-webkit-text-size-adjust:none;mso-hide:all;"">Take Action</a></div>

    	</td>
     </tr>
    </table>

</td>
</tr>
</table>
    <p><b>Quick Notes:</b></p>
<table width=""100%"">
<tr>
    <td>
{% for template in NoteTemplates %}
<table align=""left"" style=""width: 20%; min-width: 114px; margin-bottom: 12px;"" cellpadding=""0"" cellspacing=""0"">
 <tr>
   <td>
		<div><!--[if mso]>
		  <v:roundrect xmlns:v=""urn:schemas-microsoft-com:vml"" xmlns:w=""urn:schemas-microsoft-com:office:word"" href=""{{ 'Global' | Attribute:'InternalApplicationRoot' | ReplaceLast:'/','' }}{{ LinkedPages.CareDashboard }}?QuickNote={{ template.Id }}&CareNeed={{ CareNeed.Id }}&rckipid={{ personToken }}"" style=""height:38px;v-text-anchor:middle;width:95px;padding-left:5px;padding-right:5px;"" arcsize=""11%"" strokecolor=""#269abc"" fillcolor=""#31b0d5"">
			<w:anchorlock/>
			<center style=""color:#ffffff;font-family:sans-serif;font-size:13px;font-weight:normal;"">{{ template.Note }}</center>
		  </v:roundrect>
		<![endif]--><a href=""{{ 'Global' | Attribute:'InternalApplicationRoot' | ReplaceLast:'/','' }}{{ LinkedPages.CareDashboard }}?QuickNote={{ template.Id }}&CareNeed={{ CareNeed.Id }}&rckipid={{ personToken }}""
		style=""background-color: #31b0d5;border: 1px solid #269abc;border-radius: 4px;color: #ffffff;display: inline-block;font-family: sans-serif;font-size: 13px;font-weight: normal;line-height: 38px;text-align: center;text-decoration: none;width: 95px;-webkit-text-size-adjust: none;mso-hide: all;white-space: nowrap;text-overflow: ellipsis;overflow: hidden;padding-left: 5px;padding-right: 5px;"">{{ template.Note }}</a></div>

	</td>
 </tr>
</table>
{% endfor %}
	</td>
 </tr>
</table>
{{ 'Global' | Attribute:'EmailFooter' }}
", SystemGuid.SystemCommunication.CARE_NEED_FOLLOWUP_WITH_ACTIONS, true, @"{% assign noteTemplateSize = NoteTemplates | Size | Plus:1 %}{% assign personToken = Person | PersonTokenCreate:2880,noteTemplateSize -%}
A Care Need has been flagged for follow up
""{{ CareNeed.Details | Truncate:20,'...' }}""
{% capture dashboardLink %}{{ 'Global' | Attribute:'InternalApplicationRoot' | ReplaceLast:'/','' }}{{ LinkedPages.CareDashboard }}{% endcapture -%}
{% capture genericLink %}{{ dashboardLink }}?QuickNote=-1&CareNeed={{ CareNeed.Id }}&rckipid={{ personToken }}{% endcapture %}{{ genericLink | CreateShortLink }}

Quick Notes:
{% for template in NoteTemplates %} {{ template.Note }}: {% capture templateLink %}{{ dashboardLink }}?QuickNote={{ template.Id }}&CareNeed={{ CareNeed.Id }}&rckipid={{ personToken }}{% endcapture %}{{ templateLink | CreateShortLink }}
{% endfor %}" );

            RockMigrationHelper.UpdateSystemCommunication( "Plugins", "Care Touch Needed with Actions", "", "", "", "", "", "Your assigned Care Need requires attention", @"{{ 'Global' | Attribute:'EmailHeader' }}
{% assign noteTemplateSize = NoteTemplates | Size | Plus:1 %}{% assign personToken = Person | PersonTokenCreate:2880,noteTemplateSize %}
    <p>{{ Person.FullName }},</p>
    <p>Your assigned Care Need requires attention{% if TouchTemplate and TouchTemplate.NoteTemplate.Note != '' %}, specifically there are not enough ""{{ TouchTemplate.NoteTemplate.Note }}"" Care Touches{% endif %}:</p>
    <blockquote><b>Care Need ({{ CareNeed.Id }})</b><br>
    <b>Date:</b> {{ CareNeed.DateEntered }}<br>
    <b>Name:</b> {{ CareNeed.PersonAlias.Person.FullName }}<br>
    <b>Category:</b> {{ CareNeed.Category.Value }}<br>
    <b>Details:</b> {{ CareNeed.Details }}<br>
    {% if TouchTemplate and TouchTemplate.NoteTemplate.Note != '' %}<b>""{{ TouchTemplate.NoteTemplate.Note }}"" Care Touches:</b> {{ NoteTouchCount }}<br>{% endif %}
    <b>Total Care Touches:</b> {{ TouchCount }}</blockquote>
<table width=""100%"">
<tr>
    <td>
    <table align=""left"" style=""width: 29%; min-width: 190px; margin-bottom: 12px;"" cellpadding=""0"" cellspacing=""0"">
     <tr>
       <td>
    		<div><!--[if mso]>
    		  <v:roundrect xmlns:v=""urn:schemas-microsoft-com:vml"" xmlns:w=""urn:schemas-microsoft-com:office:word"" href=""{{ 'Global' | Attribute:'InternalApplicationRoot' | ReplaceLast:'/','' }}{{ LinkedPages.CareDashboard }}?rckipid={{ personToken }}"" style=""height:38px;v-text-anchor:middle;width:175px;"" arcsize=""11%"" strokecolor=""#269abc"" fillcolor=""#31b0d5"">
    			<w:anchorlock/>
    			<center style=""color:#ffffff;font-family:sans-serif;font-size:14px;font-weight:normal;"">View Care Dashboard</center>
    		  </v:roundrect>
    		<![endif]--><a href=""{{ 'Global' | Attribute:'InternalApplicationRoot' | ReplaceLast:'/','' }}{{ LinkedPages.CareDashboard }}?rckipid={{ personToken }}""
    		style=""background-color:#31b0d5;border:1px solid #269abc;border-radius:4px;color:#ffffff;display:inline-block;font-family:sans-serif;font-size:14px;font-weight:normal;line-height:38px;text-align:center;text-decoration:none;width:175px;-webkit-text-size-adjust:none;mso-hide:all;"">View Care Dashboard</a></div>

    	</td>
     </tr>
    </table>
    <table align=""left"" style=""width: 29%; min-width: 190px; margin-bottom: 12px;"" cellpadding=""0"" cellspacing=""0"">
     <tr>
       <td>
    		<div><!--[if mso]>
    		  <v:roundrect xmlns:v=""urn:schemas-microsoft-com:vml"" xmlns:w=""urn:schemas-microsoft-com:office:word"" href=""{{ 'Global' | Attribute:'InternalApplicationRoot' | ReplaceLast:'/','' }}{{ LinkedPages.CareDashboard }}?QuickNote=-1&CareNeed={{ CareNeed.Id }}&rckipid={{ personToken }}"" style=""height:38px;v-text-anchor:middle;width:175px;"" arcsize=""11%"" strokecolor=""#269abc"" fillcolor=""#31b0d5"">
    			<w:anchorlock/>
    			<center style=""color:#ffffff;font-family:sans-serif;font-size:14px;font-weight:normal;"">Take Action</center>
    		  </v:roundrect>
    		<![endif]--><a href=""{{ 'Global' | Attribute:'InternalApplicationRoot' | ReplaceLast:'/','' }}{{ LinkedPages.CareDashboard }}?QuickNote=-1&CareNeed={{ CareNeed.Id }}&rckipid={{ personToken }}""
    		style=""background-color:#31b0d5;border:1px solid #269abc;border-radius:4px;color:#ffffff;display:inline-block;font-family:sans-serif;font-size:14px;font-weight:normal;line-height:38px;text-align:center;text-decoration:none;width:175px;-webkit-text-size-adjust:none;mso-hide:all;"">Take Action</a></div>

    	</td>
     </tr>
    </table>
</td>
</tr>
</table>
    <p><b>Quick Notes:</b></p>
<table width=""100%"">
<tr>
    <td>
{% for template in NoteTemplates %}
<table align=""left"" style=""width: 20%; min-width: 114px; margin-bottom: 12px;"" cellpadding=""0"" cellspacing=""0"">
 <tr>
   <td>
		<div><!--[if mso]>
		  <v:roundrect xmlns:v=""urn:schemas-microsoft-com:vml"" xmlns:w=""urn:schemas-microsoft-com:office:word"" href=""{{ 'Global' | Attribute:'InternalApplicationRoot' | ReplaceLast:'/','' }}{{ LinkedPages.CareDashboard }}?QuickNote={{ template.Id }}&CareNeed={{ CareNeed.Id }}&rckipid={{ personToken }}"" style=""height:38px;v-text-anchor:middle;width:95px;padding-left:5px;padding-right:5px;"" arcsize=""11%"" strokecolor=""#269abc"" fillcolor=""#31b0d5"">
			<w:anchorlock/>
			<center style=""color:#ffffff;font-family:sans-serif;font-size:13px;font-weight:normal;"">{{ template.Note }}</center>
		  </v:roundrect>
		<![endif]--><a href=""{{ 'Global' | Attribute:'InternalApplicationRoot' | ReplaceLast:'/','' }}{{ LinkedPages.CareDashboard }}?QuickNote={{ template.Id }}&CareNeed={{ CareNeed.Id }}&rckipid={{ personToken }}""
		style=""background-color: #31b0d5;border: 1px solid #269abc;border-radius: 4px;color: #ffffff;display: inline-block;font-family: sans-serif;font-size: 13px;font-weight: normal;line-height: 38px;text-align: center;text-decoration: none;width: 95px;-webkit-text-size-adjust: none;mso-hide: all;white-space: nowrap;text-overflow: ellipsis;overflow: hidden;padding-left: 5px;padding-right: 5px;"">{{ template.Note }}</a></div>

	</td>
 </tr>
</table>
{% endfor %}
	</td>
 </tr>
</table>
{{ 'Global' | Attribute:'EmailFooter' }}
", SystemGuid.SystemCommunication.CARE_NEED_TOUCH_NEEDED_WITH_ACTIONS, true, @"{% assign noteTemplateSize = NoteTemplates | Size | Plus:1 %}{% assign personToken = Person | PersonTokenCreate:2880,noteTemplateSize -%}
Your assigned Care Need requires attention:
""{{ CareNeed.Details | Truncate:20,'...' }}""
{% capture dashboardLink %}{{ 'Global' | Attribute:'PublicApplicationRoot' | ReplaceLast:'/','' }}{{ LinkedPages.CareDashboard }}{% endcapture -%}
{%- capture genericLink -%}{{ dashboardLink }}?QuickNote=-1&CareNeed={{ CareNeed.Id }}&rckipid={{ personToken }}{% endcapture %}{{ genericLink | CreateShortLink }}

Quick Notes:
{% for template in NoteTemplates %} {{ template.Note }}: {% capture templateLink %}{{ dashboardLink }}?QuickNote={{ template.Id }}&CareNeed={{ CareNeed.Id }}&rckipid={{ personToken }}{% endcapture %}{{ templateLink | CreateShortLink }}
{% endfor %}" );

            RockMigrationHelper.UpdateSystemCommunication( "Plugins", "Outstanding Care Needs", "", "", "", "", "", "Outstanding Care Needs", @"{{ 'Global' | Attribute:'EmailHeader' }}
{% assign careNeedsSize = CareNeeds | Size %}{% assign personToken = Person | PersonTokenCreate:2880,careNeedsSize %}
<table class='row' style='border-collapse: collapse; border-spacing: 0; display: table; padding: 0; position: relative; text-align: left; vertical-align: top; width: 100%;'>
   <tbody>
      <tr style='padding: 0; text-align: left; vertical-align: top;'>
         <th class='small-12 large-12 columns first last' style='Margin: 0 auto; color: #0a0a0a; font-family: Helvetica, Arial, sans-serif; font-size: 16px; font-weight: normal; line-height: 1.3; margin: 0 auto; padding: 0; padding-bottom: 16px; padding-left: 16px; padding-right: 16px; text-align: left; width: 564px;'>
            <div class='structure-dropzone'>
               <div class='dropzone'>
                  <table class='component component-text' data-state='component' style='border-collapse: collapse; border-spacing: 0px; display: table; padding: 0px; position: relative; text-align: left; vertical-align: top; width: 100%; background-color: rgba(0, 0, 0, 0);'>
                     <tbody>
                        <tr>
                           <td class='js-component-text-wrapper' style='color: #0a0a0a;font-family: Helvetica, Arial, sans-serif;font-size: 16px;font-weight: normal;line-height: 1.3;border-color: rgb(0, 0, 0)'>
                              <p>{{ Person.FullName }},</p>
                              <p>You have the following care needs assigned to you:</p>
                           </td>
                        </tr>
                     </tbody>
                  </table>
                  <div class='component component-section' data-state='component'>
                     <table class='row' width='100%'>
                        <tbody>
                           <tr>
                              {%- for careNeed in CareNeeds -%}
                              <td class='dropzone columns large-6 small-12 first' width='50%' valign='top' style='background-color: rgba(0, 0, 0, 0);'>
                                 <table class='component component-text' data-state='component' style='border-collapse: collapse; border-spacing: 0px; display: table; padding: 0px; position: relative; text-align: left; vertical-align: top; width: 100%; background-color: rgba(0, 0, 0, 0);'>
                                    <tbody>
                                       <tr>
                                          <td class='js-component-text-wrapper' style='color: #0a0a0a;font-family: Helvetica, Arial, sans-serif;font-size: 16px;font-weight: normal;line-height: 1.3;border-color: transparent'>
                                             <span style='float:right; padding-right: 1em; margin-top: 1.5em;'>#{{ careNeed.Id }}</span>
                                             <h2 style='line-height: normal; margin-top: .83em;'>{{ careNeed.PersonAlias.Person.FullName }}<br><span style='font-weight: normal; font-size: 14px'>{{ careNeed.DateEntered }} | {{ careNeed.Category.Value }}</span></h2>
                                             <p>{{ careNeed.Details }}</p>
                                             <p style='font-size: 14px'><strong>Status:</strong> {{ careNeed.Status.Value }}<br>
                                             <strong>Care Touches:</strong> {{ careNeed.TouchCount }}</p>
                                          </td>
                                       </tr>
                                    </tbody>
                                 </table>
                                 <div class='component component-button v2' data-state='component'>
                                    <table class='button-outerwrap' border='0' cellpadding='0' cellspacing='0' width='100%' style='min-width:100%;'>
                                       <tbody>
                                          <tr>
                                             <td valign='top' align='center' class='button-innerwrap'>
                                                <table border='0' cellpadding='0' cellspacing='0' class='button-shell'>
                                                   <tbody>
                                                      <tr>
                                                         <td align='center' valign='middle' class='button-content' style='border-radius: 3px; background-color: rgb(0, 0, 0);'><a class='button-link' title='View Detail' href='{{ 'Global' | Attribute:'InternalApplicationRoot' | ReplaceLast:'/','' }}{{ LinkedPages.CareDashboard }}?QuickNote=-1&CareNeed={{ careNeed.Id }}&rckipid={{ personToken }}' target='_blank' style='color: rgb(255, 255, 255);display: inline-block;font-weight: normal;letter-spacing: normal;line-height: 100%;text-align: center;text-decoration: none;background-color: rgb(0, 0, 0);padding: 6px;border: 1px solid rgb(0, 0, 0);border-radius: 3px;font-size: 14px'>View Detail</a></td>
                                                      </tr>
                                                   </tbody>
                                                </table>
                                             </td>
                                          </tr>
                                       </tbody>
                                    </table>
                                 </div>
                              </td>
                            {% capture breakNow %}{{ forloop.index | Modulo:2 }}{% endcapture %}
                            {% if breakNow == 0 -%}
                            </tr>
                            <tr>
                            {% endif -%}
                            {%- endfor -%}
                           </tr>
                        </tbody>
                     </table>
                  </div>
               </div>
            </div>
         </th>
      </tr>
   </tbody>
</table>
<table class='button-outerwrap' border='0' cellpadding='0' cellspacing='0' width='100%' style='min-width:100%;'>
   <tbody>
      <tr>
         <td valign='top' align='center' class='button-innerwrap'>
            <table border='0' cellpadding='0' cellspacing='0' class='button-shell'>
               <tbody>
                  <tr>
                     <td align='center' valign='middle' class='button-content' style='border-radius: 3px; background-color: rgb(0, 0, 0);'><a class='button-link' title='View Care Dashboard' href='{{ 'Global' | Attribute:'InternalApplicationRoot' | ReplaceLast:'/','' }}{{ LinkedPages.CareDashboard }}?rckipid={{ personToken }}' target='_blank' style='color: rgb(255, 255, 255);display: inline-block;font-weight: bold;font-size: 16px;letter-spacing: normal;line-height: 100%;text-align: center;text-decoration: none;background-color: rgb(0, 0, 0);padding: 15px;border: 1px solid rgb(0, 0, 0);border-radius: 3px;'>View Care Dashboard</a></td>
                  </tr>
               </tbody>
            </table>
         </td>
      </tr>
   </tbody>
</table>
{{ 'Global' | Attribute:'EmailFooter' }}", SystemGuid.SystemCommunication.CARE_NEED_OUTSTANDING_NEEDS, true, @"{{ Person.NickName }},
You currently have {{ CareNeeds | Size }} care needs assigned to you.
View them at {% assign personToken = Person | PersonTokenCreate:2880,2 %}{% capture dashboardLink %}{{ 'Global' | Attribute:'InternalApplicationRoot' | ReplaceLast:'/','' }}{{ LinkedPages.CareDashboard }}?rckipid={{ personToken }}{% endcapture %}{{ dashboardLink | CreateShortLink }}" );
        }

        public override void Down()
        {
            RockMigrationHelper.UpdateSystemCommunication( "Plugins", "Care Need Follow Up with Actions", "", "", "", "", "", "Care Need Follow Up Required", @"{{ 'Global' | Attribute:'EmailHeader' }}
    <p>{{ Person.FullName }},</p>
    <p>A Care Need has been flagged for follow up:</p>
    <blockquote><b>Care Need ({{ CareNeed.Id }})</b><br>
    <b>Date:</b> {{ CareNeed.DateEntered }} ({{CareNeed.DateEntered | HumanizeDateTime }}) <br>
    <b>Name:</b> {{ CareNeed.PersonAlias.Person.FullName }}<br>
    <b>Category:</b> {{ CareNeed.Category.Value }}<br>
    <b>Details:</b> {{ CareNeed.Details }}</blockquote>

<table width=""100%"">
<tr>
    <td>
    <table style=""width: 29%; min-width: 190px; margin-bottom: 12px;"" cellpadding=""0"" cellspacing=""0"">
     <tr>
       <td>

    		<div><!--[if mso]>
    		  <v:roundrect xmlns:v=""urn:schemas-microsoft-com:vml"" xmlns:w=""urn:schemas-microsoft-com:office:word"" href=""{{ 'Global' | Attribute:'PublicApplicationRoot' | ReplaceLast:'/','' }}{{ LinkedPages.CareDashboard }}"" style=""height:38px;v-text-anchor:middle;width:175px;"" arcsize=""11%"" strokecolor=""#269abc"" fillcolor=""#31b0d5"">
    			<w:anchorlock/>
    			<center style=""color:#ffffff;font-family:sans-serif;font-size:14px;font-weight:normal;"">View Details</center>
    		  </v:roundrect>
    		<![endif]--><a href=""{{ 'Global' | Attribute:'PublicApplicationRoot' | ReplaceLast:'/','' }}{{ LinkedPages.CareDashboard }}""
    		style=""background-color:#31b0d5;border:1px solid #269abc;border-radius:4px;color:#ffffff;display:inline-block;font-family:sans-serif;font-size:14px;font-weight:normal;line-height:38px;text-align:center;text-decoration:none;width:175px;-webkit-text-size-adjust:none;mso-hide:all;"">View Care Dashboard</a></div>

    	</td>
     </tr>
    </table>
</td>
</tr>
</table>
    <p><b>Quick Notes:</b></p>
<table width=""100%"">
<tr>
    <td>
{% assign noteTemplateSize = NoteTemplates | Size %}{% assign personToken = Person | PersonTokenCreate:2880,noteTemplateSize %}
{% for template in NoteTemplates %}
<table align=""left"" style=""width: 20%; min-width: 114px; margin-bottom: 12px;"" cellpadding=""0"" cellspacing=""0"">
 <tr>
   <td>
		<div><!--[if mso]>
		  <v:roundrect xmlns:v=""urn:schemas-microsoft-com:vml"" xmlns:w=""urn:schemas-microsoft-com:office:word"" href=""{{ 'Global' | Attribute:'PublicApplicationRoot' | ReplaceLast:'/','' }}{{ LinkedPages.CareDashboard }}?QuickNote={{ template.Id }}&CareNeed={{ CareNeed.Id }}&rckipid={{ personToken }}"" style=""height:38px;v-text-anchor:middle;width:95px;padding-left:5px;padding-right:5px;"" arcsize=""11%"" strokecolor=""#269abc"" fillcolor=""#31b0d5"">
			<w:anchorlock/>
			<center style=""color:#ffffff;font-family:sans-serif;font-size:13px;font-weight:normal;"">{{ template.Note }}</center>
		  </v:roundrect>
		<![endif]--><a href=""{{ 'Global' | Attribute:'PublicApplicationRoot' | ReplaceLast:'/','' }}{{ LinkedPages.CareDashboard }}?QuickNote={{ template.Id }}&CareNeed={{ CareNeed.Id }}&rckipid={{ personToken }}""
		style=""background-color: #31b0d5;border: 1px solid #269abc;border-radius: 4px;color: #ffffff;display: inline-block;font-family: sans-serif;font-size: 13px;font-weight: normal;line-height: 38px;text-align: center;text-decoration: none;width: 95px;-webkit-text-size-adjust: none;mso-hide: all;white-space: nowrap;text-overflow: ellipsis;overflow: hidden;padding-left: 5px;padding-right: 5px;"">{{ template.Note }}</a></div>

	</td>
 </tr>
</table>
{% endfor %}
	</td>
 </tr>
</table>
{{ 'Global' | Attribute:'EmailFooter' }}
", SystemGuid.SystemCommunication.CARE_NEED_FOLLOWUP_WITH_ACTIONS, true, @"A Care Need has been flagged for follow up
""{{ CareNeed.Details | Truncate:20,'...' }}""
{% capture dashboardLink %}{{ 'Global' | Attribute:'PublicApplicationRoot' | ReplaceLast:'/','' }}{{ LinkedPages.CareDashboard }}{% endcapture -%}{{ dashboardLink }}

Quick Notes:
{% for template in NoteTemplates %} {{ template.Note }}: {% capture templateLink %}{{ dashboardLink }}?QuickNote={{ template.Id }}&CareNeed={{ CareNeed.Id }}&rckipid={{ Person | PersonTokenCreate:2880,2 }}{% endcapture %}{{ templateLink | CreateShortLink }}
{% endfor %}" );

            RockMigrationHelper.UpdateSystemCommunication( "Plugins", "Care Touch Needed with Actions", "", "", "", "", "", "Your assigned Care Need requires attention", @"{{ 'Global' | Attribute:'EmailHeader' }}
    <p>{{ Person.FullName }},</p>
    <p>Your assigned Care Need requires attention{% if TouchTemplate and TouchTemplate.NoteTemplate.Note != '' %}, specifically there are not enough ""{{ TouchTemplate.NoteTemplate.Note }}"" Care Touches{% endif %}:</p>
    <blockquote><b>Care Need ({{ CareNeed.Id }})</b><br>
    <b>Date:</b> {{ CareNeed.DateEntered }}<br>
    <b>Name:</b> {{ CareNeed.PersonAlias.Person.FullName }}<br>
    <b>Category:</b> {{ CareNeed.Category.Value }}<br>
    <b>Details:</b> {{ CareNeed.Details }}<br>
    {% if TouchTemplate and TouchTemplate.NoteTemplate.Note != '' %}<b>""{{ TouchTemplate.NoteTemplate.Note }}"" Care Touches:</b> {{ NoteTouchCount }}<br>{% endif %}
    <b>Total Care Touches:</b> {{ TouchCount }}</blockquote>
<table width=""100%"">
<tr>
    <td>
    <table style=""width: 29%; min-width: 190px; margin-bottom: 12px;"" cellpadding=""0"" cellspacing=""0"">
     <tr>
       <td>

    		<div><!--[if mso]>
    		  <v:roundrect xmlns:v=""urn:schemas-microsoft-com:vml"" xmlns:w=""urn:schemas-microsoft-com:office:word"" href=""{{ 'Global' | Attribute:'PublicApplicationRoot' | ReplaceLast:'/','' }}{{ LinkedPages.CareDashboard }}"" style=""height:38px;v-text-anchor:middle;width:175px;"" arcsize=""11%"" strokecolor=""#269abc"" fillcolor=""#31b0d5"">
    			<w:anchorlock/>
    			<center style=""color:#ffffff;font-family:sans-serif;font-size:14px;font-weight:normal;"">View Details</center>
    		  </v:roundrect>
    		<![endif]--><a href=""{{ 'Global' | Attribute:'PublicApplicationRoot' | ReplaceLast:'/','' }}{{ LinkedPages.CareDashboard }}""
    		style=""background-color:#31b0d5;border:1px solid #269abc;border-radius:4px;color:#ffffff;display:inline-block;font-family:sans-serif;font-size:14px;font-weight:normal;line-height:38px;text-align:center;text-decoration:none;width:175px;-webkit-text-size-adjust:none;mso-hide:all;"">View Care Dashboard</a></div>

    	</td>
     </tr>
    </table>
</td>
</tr>
</table>
    <p><b>Quick Notes:</b></p>
<table width=""100%"">
<tr>
    <td>
{% assign noteTemplateSize = NoteTemplates | Size %}{% assign personToken = Person | PersonTokenCreate:2880,noteTemplateSize %}
{% for template in NoteTemplates %}
<table align=""left"" style=""width: 20%; min-width: 114px; margin-bottom: 12px;"" cellpadding=""0"" cellspacing=""0"">
 <tr>
   <td>
		<div><!--[if mso]>
		  <v:roundrect xmlns:v=""urn:schemas-microsoft-com:vml"" xmlns:w=""urn:schemas-microsoft-com:office:word"" href=""{{ 'Global' | Attribute:'PublicApplicationRoot' | ReplaceLast:'/','' }}{{ LinkedPages.CareDashboard }}?QuickNote={{ template.Id }}&CareNeed={{ CareNeed.Id }}&rckipid={{ personToken }}"" style=""height:38px;v-text-anchor:middle;width:95px;padding-left:5px;padding-right:5px;"" arcsize=""11%"" strokecolor=""#269abc"" fillcolor=""#31b0d5"">
			<w:anchorlock/>
			<center style=""color:#ffffff;font-family:sans-serif;font-size:13px;font-weight:normal;"">{{ template.Note }}</center>
		  </v:roundrect>
		<![endif]--><a href=""{{ 'Global' | Attribute:'PublicApplicationRoot' | ReplaceLast:'/','' }}{{ LinkedPages.CareDashboard }}?QuickNote={{ template.Id }}&CareNeed={{ CareNeed.Id }}&rckipid={{ personToken }}""
		style=""background-color: #31b0d5;border: 1px solid #269abc;border-radius: 4px;color: #ffffff;display: inline-block;font-family: sans-serif;font-size: 13px;font-weight: normal;line-height: 38px;text-align: center;text-decoration: none;width: 95px;-webkit-text-size-adjust: none;mso-hide: all;white-space: nowrap;text-overflow: ellipsis;overflow: hidden;padding-left: 5px;padding-right: 5px;"">{{ template.Note }}</a></div>

	</td>
 </tr>
</table>
{% endfor %}
	</td>
 </tr>
</table>
{{ 'Global' | Attribute:'EmailFooter' }}
", SystemGuid.SystemCommunication.CARE_NEED_TOUCH_NEEDED_WITH_ACTIONS, true, @"Your assigned Care Need requires attention:
""{{ CareNeed.Details | Truncate:20,'...' }}""
{% capture dashboardLink %}{{ 'Global' | Attribute:'PublicApplicationRoot' | ReplaceLast:'/','' }}{{ LinkedPages.CareDashboard }}{% endcapture -%}{{ dashboardLink }}

Quick Notes:
{% for template in NoteTemplates %} {{ template.Note }}: {% capture templateLink %}{{ dashboardLink }}?QuickNote={{ template.Id }}&CareNeed={{ CareNeed.Id }}&rckipid={{ Person | PersonTokenCreate:2880,2 }}{% endcapture %}{{ templateLink | CreateShortLink }}
{% endfor %}" );

            RockMigrationHelper.UpdateSystemCommunication( "Plugins", "Outstanding Care Needs", "", "", "", "", "", "Outstanding Care Needs", @"{{ 'Global' | Attribute:'EmailHeader' }}
<table class='row' style='border-collapse: collapse; border-spacing: 0; display: table; padding: 0; position: relative; text-align: left; vertical-align: top; width: 100%;'>
   <tbody>
      <tr style='padding: 0; text-align: left; vertical-align: top;'>
         <th class='small-12 large-12 columns first last' style='Margin: 0 auto; color: #0a0a0a; font-family: Helvetica, Arial, sans-serif; font-size: 16px; font-weight: normal; line-height: 1.3; margin: 0 auto; padding: 0; padding-bottom: 16px; padding-left: 16px; padding-right: 16px; text-align: left; width: 564px;'>
            <div class='structure-dropzone'>
               <div class='dropzone'>
                  <table class='component component-text' data-state='component' style='border-collapse: collapse; border-spacing: 0px; display: table; padding: 0px; position: relative; text-align: left; vertical-align: top; width: 100%; background-color: rgba(0, 0, 0, 0);'>
                     <tbody>
                        <tr>
                           <td class='js-component-text-wrapper' style='color: #0a0a0a;font-family: Helvetica, Arial, sans-serif;font-size: 16px;font-weight: normal;line-height: 1.3;border-color: rgb(0, 0, 0)'>
                              <p>{{ Person.FullName }},</p>
                              <p>You have the following care needs assigned to you:</p>
                           </td>
                        </tr>
                     </tbody>
                  </table>
                  <div class='component component-section' data-state='component'>
                     <table class='row' width='100%'>
                        <tbody>
                           <tr>
                              {%- for careNeed in CareNeeds -%}
                              <td class='dropzone columns large-6 small-12 first' width='50%' valign='top' style='background-color: rgba(0, 0, 0, 0);'>
                                 <table class='component component-text' data-state='component' style='border-collapse: collapse; border-spacing: 0px; display: table; padding: 0px; position: relative; text-align: left; vertical-align: top; width: 100%; background-color: rgba(0, 0, 0, 0);'>
                                    <tbody>
                                       <tr>
                                          <td class='js-component-text-wrapper' style='color: #0a0a0a;font-family: Helvetica, Arial, sans-serif;font-size: 16px;font-weight: normal;line-height: 1.3;border-color: transparent'>
                                             <span style='float:right; padding-right: 1em; margin-top: 1.5em;'>#{{ careNeed.Id }}</span>
                                             <h2 style='line-height: normal; margin-top: .83em;'>{{ careNeed.PersonAlias.Person.FullName }}<br><span style='font-weight: normal; font-size: 14px'>{{ careNeed.DateEntered }} | {{ careNeed.Category.Value }}</span></h2>
                                             <p>{{ careNeed.Details }}</p>
                                             <p style='font-size: 14px'><strong>Status:</strong> {{ careNeed.Status.Value }}<br>
                                            <strong>Care Touches:</strong> {{ careNeed.TouchCount }}</p>
                                          </td>
                                       </tr>
                                    </tbody>
                                 </table>
                                 <div class='component component-button v2' data-state='component'>
                                    <table class='button-outerwrap' border='0' cellpadding='0' cellspacing='0' width='100%' style='min-width:100%;'>
                                       <tbody>
                                          <tr>
                                             <td valign='top' align='center' class='button-innerwrap'>
                                                <table border='0' cellpadding='0' cellspacing='0' class='button-shell'>
                                                   <tbody>
                                                      <tr>
                                                         <td align='center' valign='middle' class='button-content' style='border-radius: 3px; background-color: rgb(0, 0, 0);'><a class='button-link' title='View Detail' href='{{ 'Global' | Attribute:'InternalApplicationRoot' | ReplaceLast:'/','' }}{{ LinkedPages.CareDetail }}?CareNeedId={{ careNeed.Id }}' target='_blank' style='color: rgb(255, 255, 255);display: inline-block;font-weight: normal;letter-spacing: normal;line-height: 100%;text-align: center;text-decoration: none;background-color: rgb(0, 0, 0);padding: 6px;border: 1px solid rgb(0, 0, 0);border-radius: 3px;font-size: 14px'>View Detail</a></td>
                                                      </tr>
                                                   </tbody>
                                                </table>
                                             </td>
                                          </tr>
                                       </tbody>
                                    </table>
                                 </div>
                              </td>
                            {% capture breakNow %}{{ forloop.index | Modulo:2 }}{% endcapture %}
                            {% if breakNow == 0 -%}
                            </tr>
                            <tr>
                            {% endif -%}
                            {%- endfor -%}
                           </tr>
                        </tbody>
                     </table>
                  </div>
               </div>
            </div>
         </th>
      </tr>
   </tbody>
</table>
<table class='button-outerwrap' border='0' cellpadding='0' cellspacing='0' width='100%' style='min-width:100%;'>
   <tbody>
      <tr>
         <td valign='top' align='center' class='button-innerwrap'>
            <table border='0' cellpadding='0' cellspacing='0' class='button-shell'>
               <tbody>
                  <tr>
                     <td align='center' valign='middle' class='button-content' style='border-radius: 3px; background-color: rgb(0, 0, 0);'><a class='button-link' title='View Care Dashboard' href='{{ 'Global' | Attribute:'InternalApplicationRoot' | ReplaceLast:'/','' }}{{ LinkedPages.CareDashboard }}' target='_blank' style='color: rgb(255, 255, 255);display: inline-block;font-weight: bold;font-size: 16px;letter-spacing: normal;line-height: 100%;text-align: center;text-decoration: none;background-color: rgb(0, 0, 0);padding: 15px;border: 1px solid rgb(0, 0, 0);border-radius: 3px;'>View Care Dashboard</a></td>
                  </tr>
               </tbody>
            </table>
         </td>
      </tr>
   </tbody>
</table>
{{ 'Global' | Attribute:'EmailFooter' }}", SystemGuid.SystemCommunication.CARE_NEED_OUTSTANDING_NEEDS, true, @"{{ Person.NickName }},
You currently have {{ CareNeeds | Size }} care needs assigned to you.
View them at {{ 'Global' | Attribute:'InternalApplicationRoot' | ReplaceLast:'/','' }}{{ LinkedPages.CareDashboard }}" );
        }
    }
}