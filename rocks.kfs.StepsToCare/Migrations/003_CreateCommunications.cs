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
    <p><a href='{{ 'Global' | Attribute:'InternalApplicationRoot' | ReplaceLast:'/','' }}{{ LinkedPages.CareDashboard }}'>View Care Dashboard</a></p>
{{ 'Global' | Attribute:'EmailFooter' }}
", SystemGuid.SystemCommunication.CARE_NEED_ASSIGNED, true, @"You have been assigned a new Care Need for {{ CareNeed.PersonAlias.Person.FullName }}.
""{{ CareNeed.Details | Truncate:20,'...' }}""
{{ 'Global' | Attribute:'InternalApplicationRoot' | ReplaceLast:'/','' }}{{ LinkedPages.CareDashboard }}" );

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
", SystemGuid.SystemCommunication.CARE_NEED_FOLLOWUP, true, @"A Care Need has been flagged for follow up
""{{ CareNeed.Details | Truncate:20,'...' }}""
{{ 'Global' | Attribute:'InternalApplicationRoot' | ReplaceLast:'/','' }}{{ LinkedPages.CareDashboard }}" );

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
", SystemGuid.SystemCommunication.CARE_NEED_TOUCH_NEEDED, true, @"Your assigned Care Need requires attention: 
""{{ CareNeed.Details | Truncate:20,'...' }}""
{{ 'Global' | Attribute:'InternalApplicationRoot' | ReplaceLast:'/','' }}{{ LinkedPages.CareDashboard }}" );

            Sql( string.Format( "UPDATE [SystemCommunication] SET [IsSystem] = 0 WHERE [Guid] = '{0}'", SystemGuid.SystemCommunication.CARE_NEED_ASSIGNED ) );
            Sql( string.Format( "UPDATE [SystemCommunication] SET [IsSystem] = 0 WHERE [Guid] = '{0}'", SystemGuid.SystemCommunication.CARE_NEED_FOLLOWUP ) );
            Sql( string.Format( "UPDATE [SystemCommunication] SET [IsSystem] = 0 WHERE [Guid] = '{0}'", SystemGuid.SystemCommunication.CARE_NEED_OUTSTANDING_NEEDS ) );
            Sql( string.Format( "UPDATE [SystemCommunication] SET [IsSystem] = 0 WHERE [Guid] = '{0}'", SystemGuid.SystemCommunication.CARE_NEED_TOUCH_NEEDED ) );
        }

        public override void Down()
        {
            RockMigrationHelper.DeleteSystemCommunication( SystemGuid.SystemCommunication.CARE_NEED_TOUCH_NEEDED );
            RockMigrationHelper.DeleteSystemCommunication( SystemGuid.SystemCommunication.CARE_NEED_OUTSTANDING_NEEDS );
            RockMigrationHelper.DeleteSystemCommunication( SystemGuid.SystemCommunication.CARE_NEED_FOLLOWUP );
            RockMigrationHelper.DeleteSystemCommunication( SystemGuid.SystemCommunication.CARE_NEED_ASSIGNED );
        }
    }
}