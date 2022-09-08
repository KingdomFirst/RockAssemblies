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

namespace rocks.kfs.DashboardsAndMetrics.Migrations
{
    [MigrationNumber( 3, "1.12.8" )]
    public class CreatePages : Migration
    {
        public override void Up()
        {
            #region Financial Dashboard

            // Add Page
            //  Internal Name: KFS Financial Dashboard
            //  Site: Rock RMS
            RockMigrationHelper.AddPage( true, "7F2581A1-941E-4D51-8A9D-5BE9B881B003", "D65F783D-87A9-4CC9-8110-E83466A0EADB", "KFS Financial Dashboard", "", "DAE58EFA-8315-4F0A-A7EE-DDD0B986C20E", "" );

            // Add Block
            //  Block Name: KFS Financial Dashboard Content
            //  Page Name: KFS Financial Dashboard
            //  Site: Rock RMS
            RockMigrationHelper.AddBlock( true, "DAE58EFA-8315-4F0A-A7EE-DDD0B986C20E".AsGuid(), null, "C2D29296-6A87-47A9-A753-EE4E9159C4C4".AsGuid(), "19B61D65-37E3-459F-A44F-DEF0089118A3".AsGuid(), "KFS Financial Dashboard Content", "Main", @"", @"", 0, "8CB0BFB1-8B29-4DCA-9373-E52B1FD4759A" );

            // Add/Update HtmlContent for Block: KFS Financial Dashboard Content
            RockMigrationHelper.UpdateHtmlContentBlock( "8CB0BFB1-8B29-4DCA-9373-E52B1FD4759A", 
                @"{% metric where:'Guid == "56EF54EA-715B-4CF2-B401-60E3D3AF1389"' limit:'1' securityenabled:'false' %}    
    {% metricvalue where:'MetricId == {{ metric.Id }}' sort:'MetricValueDateTime,desc' limit:'1' securityenabled:'false' %}
        {% assign total = metricvalue.YValue %}
    {% endmetricvalue %}
{% endmetric %}

{% metric where:'Guid == "7C85C112-2D1E-4F34-9BA3-FE9357CD570A"' limit:'1' securityenabled:'false' %}    
    {% metricvalue where:'MetricId == {{ metric.Id }}' sort:'MetricValueDateTime,desc' limit:'1' securityenabled:'false' %}
        {% assign firsttime = metricvalue.YValue | AsInteger %}
    {% endmetricvalue %}
{% endmetric %}

{% metric where:'Guid == "62D95833-346B-4244-91A0-89B8CB72B561"' limit:'1' securityenabled:'false' %}    
    {% metricvalue where:'MetricId == {{ metric.Id }}' sort:'MetricValueDateTime,desc' limit:'1' securityenabled:'false' %}
        {% assign units = metricvalue.YValue | AsInteger %}
    {% endmetricvalue %}
{% endmetric %}

{% metric where:'Guid == "1DB4EBB4-8B92-4A0F-86AE-CC6442E7CCA4"' limit:'1' securityenabled:'false' %}    
    {% metricvalue where:'MetricId == {{ metric.Id }}' sort:'MetricValueDateTime,desc' limit:'1' securityenabled:'false' %}
        {% assign largegifts = metricvalue.YValue | AsInteger %}
    {% endmetricvalue %}
{% endmetric %}

{[kpis size:'xl']}
    [[ kpi icon:'fa-dollar-sign' value:'{{ total | FormatAsCurrency }}' label:'Contributions Last Week' color:'green-500']][[endkpi]]
    [[ kpi icon:'fa-user' value:'{{ firsttime }}' label:'First Time Givers Last Week' color:'green-500']][[endkpi]]
    [[ kpi icon:'fa-users' value:'{{ units }}' label:'Giving Units that Gave Last Week' color:'green-500']][[endkpi]]
    [[ kpi icon:'fa-money-bill' value:'{{ largegifts }}' label:'Large Gifts Last Week' color:'green-500']][[endkpi]]
{[endkpis]}", 
                "D8AA5074-9AD8-4340-8E57-1E3D18C8BB5B" );

            // Add Block Attribute Value
            //   Block: KFS Financial Dashboard Content
            //   BlockType: HTML Content
            //   Category: CMS
            //   Block Location: Page = KFS Financial Dashboard, Site = Rock RMS
            //   Attribute: Enabled Lava Commands
            /*   Attribute Value: RockEntity */
            RockMigrationHelper.AddBlockAttributeValue( "8CB0BFB1-8B29-4DCA-9373-E52B1FD4759A", "7146AC24-9250-4FC4-9DF2-9803B9A84299", @"RockEntity" );

            // Add Block Attribute Value
            //   Block: KFS Financial Dashboard Content
            //   BlockType: HTML Content
            //   Category: CMS
            //   Block Location: Page = KFS Financial Dashboard, Site = Rock RMS
            //   Attribute: Is Secondary Block
            /*   Attribute Value: False */
            RockMigrationHelper.AddBlockAttributeValue( "8CB0BFB1-8B29-4DCA-9373-E52B1FD4759A", "04C15DC1-DFB6-4D63-A7BC-0507D0E33EF4", @"False" );

            // Add Block Attribute Value
            //   Block: KFS Financial Dashboard Content
            //   BlockType: HTML Content
            //   Category: CMS
            //   Block Location: Page = KFS Financial Dashboard, Site = Rock RMS
            //   Attribute: Validate Markup
            /*   Attribute Value: True */
            RockMigrationHelper.AddBlockAttributeValue( "8CB0BFB1-8B29-4DCA-9373-E52B1FD4759A", "6E71FE26-5628-4DDA-BDBC-8E4D47BE72CD", @"True" );

            // Add Block Attribute Value
            //   Block: KFS Financial Dashboard Content
            //   BlockType: HTML Content
            //   Category: CMS
            //   Block Location: Page = KFS Financial Dashboard, Site = Rock RMS
            //   Attribute: Require Approval
            /*   Attribute Value: False */
            RockMigrationHelper.AddBlockAttributeValue( "8CB0BFB1-8B29-4DCA-9373-E52B1FD4759A", "EC2B701B-4C1D-4F3F-9C77-A73C75D7FF7A", @"False" );

            // Add Block Attribute Value
            //   Block: KFS Financial Dashboard Content
            //   BlockType: HTML Content
            //   Category: CMS
            //   Block Location: Page = KFS Financial Dashboard, Site = Rock RMS
            //   Attribute: Enable Versioning
            /*   Attribute Value: False */
            RockMigrationHelper.AddBlockAttributeValue( "8CB0BFB1-8B29-4DCA-9373-E52B1FD4759A", "7C1CE199-86CF-4EAE-8AB3-848416A72C58", @"False" );

            // Add Block Attribute Value
            //   Block: KFS Financial Dashboard Content
            //   BlockType: HTML Content
            //   Category: CMS
            //   Block Location: Page = KFS Financial Dashboard, Site = Rock RMS
            //   Attribute: Start in Code Editor mode
            /*   Attribute Value: True */
            RockMigrationHelper.AddBlockAttributeValue( "8CB0BFB1-8B29-4DCA-9373-E52B1FD4759A", "0673E015-F8DD-4A52-B380-C758011331B2", @"True" );

            // Add Block Attribute Value
            //   Block: KFS Financial Dashboard Content
            //   BlockType: HTML Content
            //   Category: CMS
            //   Block Location: Page = KFS Financial Dashboard, Site = Rock RMS
            //   Attribute: Image Root Folder
            /*   Attribute Value: ~/Content */
            RockMigrationHelper.AddBlockAttributeValue( "8CB0BFB1-8B29-4DCA-9373-E52B1FD4759A", "26F3AFC6-C05B-44A4-8593-AFE1D9969B0E", @"~/Content" );

            // Add Block Attribute Value
            //   Block: KFS Financial Dashboard Content
            //   BlockType: HTML Content
            //   Category: CMS
            //   Block Location: Page = KFS Financial Dashboard, Site = Rock RMS
            //   Attribute: User Specific Folders
            /*   Attribute Value: False */
            RockMigrationHelper.AddBlockAttributeValue( "8CB0BFB1-8B29-4DCA-9373-E52B1FD4759A", "9D3E4ED9-1BEF-4547-B6B0-CE29FE3835EE", @"False" );

            // Add Block Attribute Value
            //   Block: KFS Financial Dashboard Content
            //   BlockType: HTML Content
            //   Category: CMS
            //   Block Location: Page = KFS Financial Dashboard, Site = Rock RMS
            //   Attribute: Document Root Folder
            /*   Attribute Value: ~/Content */
            RockMigrationHelper.AddBlockAttributeValue( "8CB0BFB1-8B29-4DCA-9373-E52B1FD4759A", "3BDB8AED-32C5-4879-B1CB-8FC7C8336534", @"~/Content" );

            // Add Block Attribute Value
            //   Block: KFS Financial Dashboard Content
            //   BlockType: HTML Content
            //   Category: CMS
            //   Block Location: Page = KFS Financial Dashboard, Site = Rock RMS
            //   Attribute: Cache Duration
            /*   Attribute Value: 0 */
            RockMigrationHelper.AddBlockAttributeValue( "8CB0BFB1-8B29-4DCA-9373-E52B1FD4759A", "4DFDB295-6D0F-40A1-BEF9-7B70C56F66C4", @"0" );

            #endregion Financial Dashboard

            #region Groups Dashboard

            // Add Page
            //  Internal Name: KFS Groups Dashboard
            //  Site: Rock RMS
            RockMigrationHelper.AddPage( true, "7F2581A1-941E-4D51-8A9D-5BE9B881B003", "D65F783D-87A9-4CC9-8110-E83466A0EADB", "KFS Groups Dashboard", "", "8D00C825-E78D-4F01-8F69-62BA2D168C76", "" );

            // Add Block
            //  Block Name: KFS Groups Dashboard Content
            //  Page Name: KFS Groups Dashboard
            //  Site: Rock RMS
            RockMigrationHelper.AddBlock( true, "8D00C825-E78D-4F01-8F69-62BA2D168C76".AsGuid(), null, "C2D29296-6A87-47A9-A753-EE4E9159C4C4".AsGuid(), "19B61D65-37E3-459F-A44F-DEF0089118A3".AsGuid(), "KFS Groups Dashboard Content", "Main", @"", @"", 0, "548E8070-13EB-4B26-BB1B-E5EC49919BD3" );

            // Add/Update HtmlContent for Block: KFS Groups Dashboard Content
            RockMigrationHelper.UpdateHtmlContentBlock( "548E8070-13EB-4B26-BB1B-E5EC49919BD3",
                @"{% metric where:'Guid == "FA9DB967-39E3-4A77-85DB-3041A272D78F"' limit:'1' securityenabled:'false' %}
    {% metricvalue where:'MetricId == {{ metric.Id }}' sort:'MetricValueDateTime,desc' limit:'1' securityenabled:'false' %}
        {% assign activegroups = metricvalue.YValue | AsInteger %}
    {% endmetricvalue %}
{% endmetric %}

{% metric where:'Guid == "8B0CE73B-5557-4F93-939B-BBA640BE760E"' limit:'1' securityenabled:'false' %}
    {% metricvalue where:'MetricId == {{ metric.Id }}' sort:'MetricValueDateTime,desc' limit:'1' securityenabled:'false' %}
        {% assign attendance = metricvalue.YValue | AsInteger %}
    {% endmetricvalue %}
{% endmetric %}

{% metric where:'Guid == "D4E7CF72-B001-4AF4-B52B-B3711FEF7D1B"' limit:'1' securityenabled:'false' %}
    {% metricvalue where:'MetricId == {{ metric.Id }}' sort:'MetricValueDateTime,desc' limit:'1' securityenabled:'false' %}
        {% assign members = metricvalue.YValue | AsInteger %}
    {% endmetricvalue %}
{% endmetric %}

{% metric where:'Guid == "848416B7-5095-4342-AD4E-94FBCA5BB171"' limit:'1' securityenabled:'false' %}
    {% metricvalue where:'MetricId == {{ metric.Id }}' sort:'MetricValueDateTime,desc' limit:'1' securityenabled:'false' %}
        {% assign percent = metricvalue.YValue | AsInteger %}
    {% endmetricvalue %}
{% endmetric %}

{[kpis size:'xl']}
    [[ kpi icon:'fa-users' value:'{{ activegroups }}' label:'Active Groups' color:'blue-500']][[endkpi]]
    [[ kpi icon:'fa-check-square-o' value:'{{ attendance }}' label:'Attendance Last Week' color:'blue-500']][[endkpi]]
    [[ kpi icon:'fa-user' value:'{{ members }}' label:'Active Small Group Members' color:'blue-500']][[endkpi]]
    [[ kpi icon:'fa-user' value:'{{ percent }}%' label:'% Era in a Small Group' color:'blue-500']][[endkpi]]
{[endkpis]}",
                "A9E681DF-3AFD-4E21-84C6-BD5231247F8E" );

            // Add Block Attribute Value
            //   Block: KFS Groups Dashboard Content
            //   BlockType: HTML Content
            //   Category: CMS
            //   Block Location: Page = KFS Groups Dashboard, Site = Rock RMS
            //   Attribute: Enabled Lava Commands
            /*   Attribute Value: RockEntity */
            RockMigrationHelper.AddBlockAttributeValue( "548E8070-13EB-4B26-BB1B-E5EC49919BD3", "7146AC24-9250-4FC4-9DF2-9803B9A84299", @"RockEntity" );

            // Add Block Attribute Value
            //   Block: KFS Groups Dashboard Content
            //   BlockType: HTML Content
            //   Category: CMS
            //   Block Location: Page = KFS Groups Dashboard, Site = Rock RMS
            //   Attribute: Is Secondary Block
            /*   Attribute Value: False */
            RockMigrationHelper.AddBlockAttributeValue( "548E8070-13EB-4B26-BB1B-E5EC49919BD3", "04C15DC1-DFB6-4D63-A7BC-0507D0E33EF4", @"False" );

            // Add Block Attribute Value
            //   Block: KFS Groups Dashboard Content
            //   BlockType: HTML Content
            //   Category: CMS
            //   Block Location: Page = KFS Groups Dashboard, Site = Rock RMS
            //   Attribute: Validate Markup
            /*   Attribute Value: True */
            RockMigrationHelper.AddBlockAttributeValue( "548E8070-13EB-4B26-BB1B-E5EC49919BD3", "6E71FE26-5628-4DDA-BDBC-8E4D47BE72CD", @"True" );

            // Add Block Attribute Value
            //   Block: KFS Groups Dashboard Content
            //   BlockType: HTML Content
            //   Category: CMS
            //   Block Location: Page = KFS Groups Dashboard, Site = Rock RMS
            //   Attribute: Require Approval
            /*   Attribute Value: False */
            RockMigrationHelper.AddBlockAttributeValue( "548E8070-13EB-4B26-BB1B-E5EC49919BD3", "EC2B701B-4C1D-4F3F-9C77-A73C75D7FF7A", @"False" );

            // Add Block Attribute Value
            //   Block: KFS Groups Dashboard Content
            //   BlockType: HTML Content
            //   Category: CMS
            //   Block Location: Page = KFS Groups Dashboard, Site = Rock RMS
            //   Attribute: Enable Versioning
            /*   Attribute Value: False */
            RockMigrationHelper.AddBlockAttributeValue( "548E8070-13EB-4B26-BB1B-E5EC49919BD3", "7C1CE199-86CF-4EAE-8AB3-848416A72C58", @"False" );

            // Add Block Attribute Value
            //   Block: KFS Groups Dashboard Content
            //   BlockType: HTML Content
            //   Category: CMS
            //   Block Location: Page = KFS Groups Dashboard, Site = Rock RMS
            //   Attribute: Start in Code Editor mode
            /*   Attribute Value: True */
            RockMigrationHelper.AddBlockAttributeValue( "548E8070-13EB-4B26-BB1B-E5EC49919BD3", "0673E015-F8DD-4A52-B380-C758011331B2", @"True" );

            // Add Block Attribute Value
            //   Block: KFS Groups Dashboard Content
            //   BlockType: HTML Content
            //   Category: CMS
            //   Block Location: Page = KFS Groups Dashboard, Site = Rock RMS
            //   Attribute: Image Root Folder
            /*   Attribute Value: ~/Content */
            RockMigrationHelper.AddBlockAttributeValue( "548E8070-13EB-4B26-BB1B-E5EC49919BD3", "26F3AFC6-C05B-44A4-8593-AFE1D9969B0E", @"~/Content" );

            // Add Block Attribute Value
            //   Block: KFS Groups Dashboard Content
            //   BlockType: HTML Content
            //   Category: CMS
            //   Block Location: Page = KFS Groups Dashboard, Site = Rock RMS
            //   Attribute: User Specific Folders
            /*   Attribute Value: False */
            RockMigrationHelper.AddBlockAttributeValue( "548E8070-13EB-4B26-BB1B-E5EC49919BD3", "9D3E4ED9-1BEF-4547-B6B0-CE29FE3835EE", @"False" );

            // Add Block Attribute Value
            //   Block: KFS Groups Dashboard Content
            //   BlockType: HTML Content
            //   Category: CMS
            //   Block Location: Page = KFS Groups Dashboard, Site = Rock RMS
            //   Attribute: Document Root Folder
            /*   Attribute Value: ~/Content */
            RockMigrationHelper.AddBlockAttributeValue( "548E8070-13EB-4B26-BB1B-E5EC49919BD3", "3BDB8AED-32C5-4879-B1CB-8FC7C8336534", @"~/Content" );

            // Add Block Attribute Value
            //   Block: KFS Groups Dashboard Content
            //   BlockType: HTML Content
            //   Category: CMS
            //   Block Location: Page = KFS Groups Dashboard, Site = Rock RMS
            //   Attribute: Cache Duration
            /*   Attribute Value: 0 */
            RockMigrationHelper.AddBlockAttributeValue( "548E8070-13EB-4B26-BB1B-E5EC49919BD3", "4DFDB295-6D0F-40A1-BEF9-7B70C56F66C4", @"0" );

            #endregion Groups Dashboard
        }

        public override void Down()
        {
            #region Financial Dashboard

            // Remove Block
            //  Name: KFS Financial Dashboard Content, from Page: KFS Financial Dashboard, Site: Rock RMS
            RockMigrationHelper.DeleteBlock( "8CB0BFB1-8B29-4DCA-9373-E52B1FD4759A" );

            // Delete Page
            //  Internal Name: KFS Financial Dashboard
            //  Site: Rock RMS
            //  Layout: Full Width
            RockMigrationHelper.DeletePage( "DAE58EFA-8315-4F0A-A7EE-DDD0B986C20E" );

            #endregion Financial Dashboard

            #region Groups Dashboard

            // Remove Block
            //  Name: KFS Groups Dashboard Content, from Page: KFS Groups Dashboard, Site: Rock RMS
            RockMigrationHelper.DeleteBlock( "548E8070-13EB-4B26-BB1B-E5EC49919BD3" );

            // Delete Page
            //  Internal Name: KFS Groups Dashboard
            //  Site: Rock RMS
            //  Layout: Full Width
            RockMigrationHelper.DeletePage( "8D00C825-E78D-4F01-8F69-62BA2D168C76" );

            #endregion Groups Dashboard
        }
    }
}