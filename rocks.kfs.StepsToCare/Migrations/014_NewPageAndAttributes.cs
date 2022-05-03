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
using System.Collections.Generic;
using Rock;
using Rock.Plugin;

namespace rocks.kfs.StepsToCare.Migrations
{
    [MigrationNumber( 14, "1.12.3" )]
    public class NewPageAndAttributes : Migration
    {
        public override void Up()
        {
            // Attribute for BlockType
            //   BlockType: Care Dashboard
            //   Category: KFS > Steps To Care
            //   Attribute: Target Person Mode Include Family Members
            RockMigrationHelper.AddOrUpdateBlockTypeAttribute( "AF14CB6C-F915-4449-9CB7-7C44B624B051", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Target Person Mode Include Family Members", "TargetModeIncludeFamilyMembers", "Target Person Mode Include Family Members", @"When running in Target Person mode/a Person Detail tab, should we include family member needs in the list? Default: false.", 7, @"False", "B60B5D46-31A6-41BF-9A23-F15AE4525604" );

            // Add Block Attribute Value
            //   Block: Care Dashboard
            //   BlockType: Care Dashboard
            //   Category: KFS > Steps To Care
            //   Block Location: Page=Care Dashboard, Site=Rock RMS
            //   Attribute: Target Person Mode Include Family Members
            /*   Attribute Value: False */
            //   Skip If Already Exists: true
            RockMigrationHelper.AddBlockAttributeValue( true, "EADBE3F0-F64B-4583-B49D-F0031BBC929F", "B60B5D46-31A6-41BF-9A23-F15AE4525604", @"False" );

            RockMigrationHelper.AddDefinedTypeAttribute( "F965CB2C-23D0-42D6-8BAF-10F552249B7A", "9C204CD0-1233-41C5-818A-C5DA439445AA", "CSS Class", "CssClass", "", 1, "", "D7727F70-10B3-471E-86AE-5A2176ABCD0F" );
            RockMigrationHelper.AddAttributeQualifier( "D7727F70-10B3-471E-86AE-5A2176ABCD0F", "ispassword", "False", "13A0CDA5-70C7-4784-B96A-DE3FDB381E70" );
            RockMigrationHelper.AddAttributeQualifier( "D7727F70-10B3-471E-86AE-5A2176ABCD0F", "maxcharacters", "", "52D93628-0077-41E9-9691-213C20BED311" );
            RockMigrationHelper.AddAttributeQualifier( "D7727F70-10B3-471E-86AE-5A2176ABCD0F", "showcountdown", "False", "21B13EA6-18B3-475E-AB8B-5784FA78F356" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "1B48E766-3B9B-4A32-AB1A-80DC0C2DCC63", "D7727F70-10B3-471E-86AE-5A2176ABCD0F", @"label label-warning" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "3613946D-9788-47E1-BFD3-5FD75B59F986", "D7727F70-10B3-471E-86AE-5A2176ABCD0F", @"label label-default" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "811ECA2D-2B74-469A-9CFB-AB47B9643A02", "D7727F70-10B3-471E-86AE-5A2176ABCD0F", @"label label-success" );
            RockMigrationHelper.AddDefinedValueAttributeValue( "989A3B33-8230-4167-99F6-E1C21EE6950E", "D7727F70-10B3-471E-86AE-5A2176ABCD0F", @"label label-info" );

            // Add Page 
            //  Internal Name: Care Needs
            //  Site: Rock RMS
            RockMigrationHelper.AddPage( true, "BF04BB7E-BE3A-4A38-A37C-386B55496303", "F66758C6-3E3D-4598-AF4C-B317047B5987", "Care Needs", "", "ABA4CE73-28DC-42DE-BE70-33F09287C116", "" );
            // Add Page Route
            //   Page:Care Needs
            //   Route:Person/{PersonId}/CareNeeds
            RockMigrationHelper.AddPageRoute( "ABA4CE73-28DC-42DE-BE70-33F09287C116", "Person/{PersonId}/CareNeeds", "A243DDEE-1B40-4DAD-8A70-35116558E47F" );
            // Add Block 
            //  Block Name: Care Dashboard
            //  Page Name: Care Needs
            //  Layout: -
            //  Site: Rock RMS
            RockMigrationHelper.AddBlock( true, "ABA4CE73-28DC-42DE-BE70-33F09287C116".AsGuid(), null, "C2D29296-6A87-47A9-A753-EE4E9159C4C4".AsGuid(), "AF14CB6C-F915-4449-9CB7-7C44B624B051".AsGuid(), "Care Dashboard", "SectionC1", @"", @"", 0, "6AC9740A-EFAA-42E4-BD9B-D131FE4AE442" );

            // Add Block Attribute Value
            //   Block: Care Dashboard
            //   BlockType: Care Dashboard
            //   Category: KFS > Steps To Care
            //   Block Location: Page=Care Needs, Site=Rock RMS
            //   Attribute: Minimum Care Touch Hours
            /*   Attribute Value: 24 */
            //   Skip If Already Exists: true
            RockMigrationHelper.AddBlockAttributeValue( true, "6AC9740A-EFAA-42E4-BD9B-D131FE4AE442", "8945BE62-D065-4A19-89A8-B06CE51FFBFF", @"24" );

            // Add Block Attribute Value
            //   Block: Care Dashboard
            //   BlockType: Care Dashboard
            //   Category: KFS > Steps To Care
            //   Block Location: Page=Care Needs, Site=Rock RMS
            //   Attribute: Enable Add Connection Request
            /*   Attribute Value: False */
            //   Skip If Already Exists: true
            RockMigrationHelper.AddBlockAttributeValue( true, "6AC9740A-EFAA-42E4-BD9B-D131FE4AE442", "9D1C58A6-FBD8-454B-8383-1060A7629D3E", @"False" );

            // Add Block Attribute Value
            //   Block: Care Dashboard
            //   BlockType: Care Dashboard
            //   Category: KFS > Steps To Care
            //   Block Location: Page=Care Needs, Site=Rock RMS
            //   Attribute: Enable Launch Workflow
            /*   Attribute Value: True */
            //   Skip If Already Exists: true
            RockMigrationHelper.AddBlockAttributeValue( true, "6AC9740A-EFAA-42E4-BD9B-D131FE4AE442", "7FEA3B48-CEA9-4902-B2BC-579D64221355", @"True" );

            // Add Block Attribute Value
            //   Block: Care Dashboard
            //   BlockType: Care Dashboard
            //   Category: KFS > Steps To Care
            //   Block Location: Page=Care Needs, Site=Rock RMS
            //   Attribute: Note View Lava Template
            /*   Attribute Value: {% include '~~/Assets/Lava/NoteViewList.lava' %} */
            //   Skip If Already Exists: true
            RockMigrationHelper.AddBlockAttributeValue( true, "6AC9740A-EFAA-42E4-BD9B-D131FE4AE442", "351EEAD6-6ACC-4B4D-9EE3-0B35309A2C97", @"{% include '~~/Assets/Lava/NoteViewList.lava' %}" );

            // Add Block Attribute Value
            //   Block: Care Dashboard
            //   BlockType: Care Dashboard
            //   Category: KFS > Steps To Care
            //   Block Location: Page=Care Needs, Site=Rock RMS
            //   Attribute: Close Dialog on Save
            /*   Attribute Value: True */
            //   Skip If Already Exists: true
            RockMigrationHelper.AddBlockAttributeValue( true, "6AC9740A-EFAA-42E4-BD9B-D131FE4AE442", "1A39DB8D-199D-4A3E-941C-C459F75D303E", @"True" );

            // Add Block Attribute Value
            //   Block: Care Dashboard
            //   BlockType: Care Dashboard
            //   Category: KFS > Steps To Care
            //   Block Location: Page=Care Needs, Site=Rock RMS
            //   Attribute: Minimum Care Touches
            /*   Attribute Value: 2 */
            //   Skip If Already Exists: true
            RockMigrationHelper.AddBlockAttributeValue( true, "6AC9740A-EFAA-42E4-BD9B-D131FE4AE442", "8291F010-FFFE-4806-8E19-E701FAC62E10", @"2" );

            // Add Block Attribute Value
            //   Block: Care Dashboard
            //   BlockType: Care Dashboard
            //   Category: KFS > Steps To Care
            //   Block Location: Page=Care Needs, Site=Rock RMS
            //   Attribute: Detail Page
            /*   Attribute Value: 27953b65-21e2-4ca9-8461-3afad46d9bc8 */
            //   Skip If Already Exists: true
            RockMigrationHelper.AddBlockAttributeValue( true, "6AC9740A-EFAA-42E4-BD9B-D131FE4AE442", "83441829-9BAA-4C9A-921B-E3DCED54BB20", @"27953b65-21e2-4ca9-8461-3afad46d9bc8" );

            // Add Block Attribute Value
            //   Block: Care Dashboard
            //   BlockType: Care Dashboard
            //   Category: KFS > Steps To Care
            //   Block Location: Page=Care Needs, Site=Rock RMS
            //   Attribute: Categories Template
            /*   Attribute Value: ... */
            //   Skip If Already Exists: true
            RockMigrationHelper.AddBlockAttributeValue( true, "6AC9740A-EFAA-42E4-BD9B-D131FE4AE442", "057D28FD-8D16-4A35-BF42-389264A9B3B4", @"<div class=""mr-2"">
{% for category in Categories %}
    <span class=""badge rounded-0"" style=""background-color: {{ category | Attribute:'Color' }}"">{{ category.Value }}</span>
{% endfor %} <span class=""badge rounded-0 text-color"" style=""background-color: oldlace"">Assigned to You</span>
</div>" );

            // Add Block Attribute Value
            //   Block: Care Dashboard
            //   BlockType: Care Dashboard
            //   Category: KFS > Steps To Care
            //   Block Location: Page=Care Needs, Site=Rock RMS
            //   Attribute: Show Security Button
            /*   Attribute Value: True */
            //   Skip If Already Exists: true
            RockMigrationHelper.AddBlockAttributeValue( true, "6AC9740A-EFAA-42E4-BD9B-D131FE4AE442", "9E5E061F-18B3-4D7F-BB35-D6C5E481A424", @"True" );

            // Add Block Attribute Value
            //   Block: Care Dashboard
            //   BlockType: Care Dashboard
            //   Category: KFS > Steps To Care
            //   Block Location: Page=Care Needs, Site=Rock RMS
            //   Attribute: Allow Backdated Notes
            /*   Attribute Value: False */
            //   Skip If Already Exists: true
            RockMigrationHelper.AddBlockAttributeValue( true, "6AC9740A-EFAA-42E4-BD9B-D131FE4AE442", "F938CB7D-8EAD-4627-A042-F299FEB6642C", @"False" );

            // Add Block Attribute Value
            //   Block: Care Dashboard
            //   BlockType: Care Dashboard
            //   Category: KFS > Steps To Care
            //   Block Location: Page=Care Needs, Site=Rock RMS
            //   Attribute: Show Alert Checkbox
            /*   Attribute Value: True */
            //   Skip If Already Exists: true
            RockMigrationHelper.AddBlockAttributeValue( true, "6AC9740A-EFAA-42E4-BD9B-D131FE4AE442", "997A80B0-6F5A-4F3A-8094-6CA1AE750433", @"True" );

            // Add Block Attribute Value
            //   Block: Care Dashboard
            //   BlockType: Care Dashboard
            //   Category: KFS > Steps To Care
            //   Block Location: Page=Care Needs, Site=Rock RMS
            //   Attribute: Show Private Checkbox
            /*   Attribute Value: True */
            //   Skip If Already Exists: true
            RockMigrationHelper.AddBlockAttributeValue( true, "6AC9740A-EFAA-42E4-BD9B-D131FE4AE442", "AA734D17-955C-49A0-819E-581280FAD1C6", @"True" );

            // Add Block Attribute Value
            //   Block: Care Dashboard
            //   BlockType: Care Dashboard
            //   Category: KFS > Steps To Care
            //   Block Location: Page=Care Needs, Site=Rock RMS
            //   Attribute: Display Type
            /*   Attribute Value: Full */
            //   Skip If Already Exists: true
            RockMigrationHelper.AddBlockAttributeValue( true, "6AC9740A-EFAA-42E4-BD9B-D131FE4AE442", "16067243-7FBC-4D35-B6A9-CBAABB155905", @"Full" );

            // Add Block Attribute Value
            //   Block: Care Dashboard
            //   BlockType: Care Dashboard
            //   Category: KFS > Steps To Care
            //   Block Location: Page=Care Needs, Site=Rock RMS
            //   Attribute: Use Person Icon
            /*   Attribute Value: False */
            //   Skip If Already Exists: true
            RockMigrationHelper.AddBlockAttributeValue( true, "6AC9740A-EFAA-42E4-BD9B-D131FE4AE442", "7AFE5354-773D-4E47-AFF8-E155F1880695", @"False" );

            // Add Block Attribute Value
            //   Block: Care Dashboard
            //   BlockType: Care Dashboard
            //   Category: KFS > Steps To Care
            //   Block Location: Page=Care Needs, Site=Rock RMS
            //   Attribute: Outstanding Care Needs Statuses
            /*   Attribute Value: 811eca2d-2b74-469a-9cfb-ab47b9643a02 */
            //   Skip If Already Exists: true
            RockMigrationHelper.AddBlockAttributeValue( true, "6AC9740A-EFAA-42E4-BD9B-D131FE4AE442", "DCE15052-4333-4571-85BF-8803689C56D3", @"811eca2d-2b74-469a-9cfb-ab47b9643a02" );

            // Add Block Attribute Value
            //   Block: Care Dashboard
            //   BlockType: Care Dashboard
            //   Category: KFS > Steps To Care
            //   Block Location: Page=Care Needs, Site=Rock RMS
            //   Attribute: Configuration Page
            /*   Attribute Value: 39f72e9d-22b7-4f1e-8633-6c3745ac6f34 */
            //   Skip If Already Exists: true
            RockMigrationHelper.AddBlockAttributeValue( true, "6AC9740A-EFAA-42E4-BD9B-D131FE4AE442", "C886210E-BD1B-4187-9C04-B035447B004B", @"39f72e9d-22b7-4f1e-8633-6c3745ac6f34" );

            // Add Block Attribute Value
            //   Block: Care Dashboard
            //   BlockType: Care Dashboard
            //   Category: KFS > Steps To Care
            //   Block Location: Page=Care Needs, Site=Rock RMS
            //   Attribute: Complete Child Needs on Parent Completion
            /*   Attribute Value: True */
            //   Skip If Already Exists: true
            RockMigrationHelper.AddBlockAttributeValue( true, "6AC9740A-EFAA-42E4-BD9B-D131FE4AE442", "50254F24-78CC-458A-B07E-E43B4B97D0D3", @"True" );

            // Add Block Attribute Value
            //   Block: Care Dashboard
            //   BlockType: Care Dashboard
            //   Category: KFS > Steps To Care
            //   Block Location: Page=Care Needs, Site=Rock RMS
            //   Attribute: Target Person Mode Include Family Members
            /*   Attribute Value: False */
            //   Skip If Already Exists: true
            RockMigrationHelper.AddBlockAttributeValue( true, "6AC9740A-EFAA-42E4-BD9B-D131FE4AE442", "B60B5D46-31A6-41BF-9A23-F15AE4525604", @"False" );

            // Add/Update PageContext for Page:Care Needs, Entity: Rock.Model.Person, Parameter: PersonId
            RockMigrationHelper.UpdatePageContext( "ABA4CE73-28DC-42DE-BE70-33F09287C116", "Rock.Model.Person", "PersonId", "2D97D5C5-60E7-4EA3-9DC3-72710948C7AC" );

            RockMigrationHelper.AddSecurityAuthForBlock( "6AC9740A-EFAA-42E4-BD9B-D131FE4AE442", 0, "ViewAll", true, Rock.SystemGuid.Group.GROUP_ADMINISTRATORS, Rock.Model.SpecialRole.None, "AF0EABC0-135E-4737-A234-D2EC7322730A" );
            RockMigrationHelper.AddSecurityAuthForBlock( "6AC9740A-EFAA-42E4-BD9B-D131FE4AE442", 0, "CareWorkers", true, Rock.SystemGuid.Group.GROUP_ADMINISTRATORS, Rock.Model.SpecialRole.None, "C536A811-EE66-4F79-AE36-4EED95E9E244" );
            RockMigrationHelper.AddSecurityAuthForBlock( "6AC9740A-EFAA-42E4-BD9B-D131FE4AE442", 0, "CompleteNeeds", true, Rock.SystemGuid.Group.GROUP_ADMINISTRATORS, Rock.Model.SpecialRole.None, "E4726FD7-7BB7-4BFF-AA81-8C2D3D10C35E" );
        }

        public override void Down()
        {
            RockMigrationHelper.DeleteSecurityAuth( "AF0EABC0-135E-4737-A234-D2EC7322730A" );
            RockMigrationHelper.DeleteSecurityAuth( "C536A811-EE66-4F79-AE36-4EED95E9E244" );
            RockMigrationHelper.DeleteSecurityAuth( "E4726FD7-7BB7-4BFF-AA81-8C2D3D10C35E" );

            // Remove Block
            //  Name: Care Dashboard, from Page: Care Needs, Site: Rock RMS
            //  from Page: Care Needs, Site: Rock RMS
            RockMigrationHelper.DeleteBlock( "6AC9740A-EFAA-42E4-BD9B-D131FE4AE442" );

            RockMigrationHelper.DeletePageRoute( "A243DDEE-1B40-4DAD-8A70-35116558E47F" );
            // Delete Page 
            //  Internal Name: Care Needs
            //  Site: Rock RMS
            //  Layout: PersonDetail
            RockMigrationHelper.DeletePage( "ABA4CE73-28DC-42DE-BE70-33F09287C116" );

            // Delete PageContext for Page:Care Needs, Entity: Rock.Model.Person, Parameter: PersonId
            RockMigrationHelper.DeletePageContext( "2D97D5C5-60E7-4EA3-9DC3-72710948C7AC" );

            RockMigrationHelper.DeleteAttribute( "D7727F70-10B3-471E-86AE-5A2176ABCD0F" ); // CssClass	0

            // Attribute for BlockType
            //   BlockType: Care Dashboard
            //   Category: KFS > Steps To Care
            //   Attribute: Target Person Mode Include Family Members
            RockMigrationHelper.DeleteAttribute( "B60B5D46-31A6-41BF-9A23-F15AE4525604" );
        }
    }
}