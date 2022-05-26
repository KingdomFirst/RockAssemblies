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
using System;
using System.Linq;
using Rock;
using Rock.Data;
using Rock.Model;
using Rock.Plugin;

namespace rocks.kfs.Checkin.PagerEntry.Migrations
{
    [MigrationNumber( 1, "1.12.0" )]
    public class PagerEntryInstall : Migration
    {
        /// <summary>
        /// The commands to run to migrate plugin to the specific version
        /// </summary>
        public override void Up()
        {

            // Add/Update BlockType 
            //   Name: Pager Entry
            //   Category: KFS > Check-in
            //   Path: ~/Plugins/rocks_kfs/CheckIn/PagerEntry.ascx
            //   EntityType: -
            RockMigrationHelper.UpdateBlockType( "Pager Entry", "Displays a prompt for pager number entry.", "~/Plugins/rocks_kfs/CheckIn/PagerEntry.ascx", "KFS > Check-in", "176095E9-3BEB-44DC-AADB-B5CCA8F479DB" );

            // Add/Update BlockType 
            //   Name: Pager Entry Setup
            //   Category: KFS > Check-in
            //   Path: ~/Plugins/rocks_kfs/CheckIn/PagerEntry_Setup.ascx
            //   EntityType: -
            RockMigrationHelper.UpdateBlockType( "Pager Entry Setup", "Block that sets up Pager Entry page and block settings.", "~/Plugins/rocks_kfs/CheckIn/PagerEntry_Setup.ascx", "KFS > Check-in", "E5A2E9E8-712D-4D68-A1A7-1CEC632B9359" );

            RockMigrationHelper.AddGroupTypeGroupAttribute( Rock.SystemGuid.GroupType.GROUPTYPE_FAMILY, Rock.SystemGuid.FieldType.TEXT, "Pager Number", "", 0, "", "67BABAC3-7263-4230-A528-CC103C3CB5FD" );

            Sql( @"
                UPDATE [Attribute]
                SET [Key] = 'rocks.kfs.PagerNumber', [IsSystem] = 0
                WHERE [Guid] = '67BABAC3-7263-4230-A528-CC103C3CB5FD'
            " );

            // Add Page 
            //  Internal Name: Pager Entry Setup
            //  Site: Rock RMS
            RockMigrationHelper.AddPage( true, "5B6DBC42-8B03-4D15-8D92-AAFA28FD8616", "D65F783D-87A9-4CC9-8110-E83466A0EADB", "Pager Entry Setup", "", "9668D7D9-EB92-4BAE-961C-C796882DCDD2", "fas fa-pager" );

            // Add Page 
            //  Internal Name: Pager Number
            //  Site: Rock Check-in
            RockMigrationHelper.AddPage( true, "CDF2C599-D341-42FD-B7DC-CD402EA96050", "66FA0143-F04C-4447-A67A-2A10A6BB1A2B", "Pager Number", "", "50A1708F-D751-40C5-BE99-492C4E81AED0", "" );

            // Add Block 
            //  Block Name: Pager Entry Setup
            //  Page Name: Pager Entry Setup
            //  Layout: -
            //  Site: Rock RMS
            RockMigrationHelper.AddBlock( true, "9668D7D9-EB92-4BAE-961C-C796882DCDD2".AsGuid(), null, "C2D29296-6A87-47A9-A753-EE4E9159C4C4".AsGuid(), "E5A2E9E8-712D-4D68-A1A7-1CEC632B9359".AsGuid(), "Pager Entry Setup", "Main", @"", @"", 0, "3786332E-03EC-4CA7-B513-8EF0001725F7" );

            // Add Block 
            //  Block Name: Pager Entry
            //  Page Name: Pager Number
            //  Layout: -
            //  Site: Rock Check-in
            RockMigrationHelper.AddBlock( true, "50A1708F-D751-40C5-BE99-492C4E81AED0".AsGuid(), null, "15AEFC01-ACB3-4F5D-B83E-AB3AB7F2A54A".AsGuid(), "176095E9-3BEB-44DC-AADB-B5CCA8F479DB".AsGuid(), "Pager Entry", "Main", @"", @"", 0, "A6FE5ABF-25BD-49E2-AB9B-1820FB1AC8B9" );

            // Add Block 
            //  Block Name: Idle Redirect
            //  Page Name: Pager Number
            //  Layout: -
            //  Site: Rock Check-in
            RockMigrationHelper.AddBlock( true, "50A1708F-D751-40C5-BE99-492C4E81AED0".AsGuid(), null, "15AEFC01-ACB3-4F5D-B83E-AB3AB7F2A54A".AsGuid(), "49FC4B38-741E-4B0B-B395-7C1929340D88".AsGuid(), "Idle Redirect", "Main", @"", @"", 1, "F56ADEDA-45D5-4597-8387-633B2B1A9DCC" );

            // Add Block 
            //  Block Name: Family Attributes
            //  Page Name: Person Profile
            //  Layout: -
            //  Site: Rock Check-in Manager
            RockMigrationHelper.AddBlock( true, "F3062622-C6AD-48F3-ADD7-7F58E4BD4EF3".AsGuid(), null, "A5FA7C3C-A238-4E0B-95DE-B540144321EC".AsGuid(), "19B61D65-37E3-459F-A44F-DEF0089118A3".AsGuid(), "Family Attributes", "Sidebar1", @"", @"", 1, "AC41EA0D-2385-4797-9EAB-B8E1990B224E" );

            // Add Block 
            //  Block Name: Family Attributes
            //  Page Name: Attendance Detail
            //  Layout: -
            //  Site: Rock Check-in Manager
            RockMigrationHelper.AddBlock( true, "758ECFCD-9E20-48B5-827B-973492E39C0D".AsGuid(), null, "A5FA7C3C-A238-4E0B-95DE-B540144321EC".AsGuid(), "19B61D65-37E3-459F-A44F-DEF0089118A3".AsGuid(), "Family Attributes", "Sidebar1", @"", @"", 1, "83763A73-794F-49D9-BC1E-DD34AC628BFB" );

            // Attribute for BlockType
            //   BlockType: Pager Entry
            //   Category: KFS > Check-in
            //   Attribute: Title Template
            RockMigrationHelper.AddOrUpdateBlockTypeAttribute( "176095E9-3BEB-44DC-AADB-B5CCA8F479DB", "27718256-C1EB-4B1F-9B4B-AC53249F78DF", "Title Template", "TitleTemplate", "Title Template", @"", 5, @"{{ Family.Name }}", "0DDB5F00-F3E6-498D-8BE3-965B0BBDC9BD" );

            // Attribute for BlockType
            //   BlockType: Pager Entry
            //   Category: KFS > Check-in
            //   Attribute: Caption
            RockMigrationHelper.AddOrUpdateBlockTypeAttribute( "176095E9-3BEB-44DC-AADB-B5CCA8F479DB", "9C204CD0-1233-41C5-818A-C5DA439445AA", "Caption", "Caption", "Caption", @"", 6, @"Please enter the pager number", "F7DAF664-1538-49CE-B009-FE70421DD1AC" );

            // Attribute for BlockType
            //   BlockType: Pager Entry
            //   Category: KFS > Check-in
            //   Attribute: Workflow Type
            RockMigrationHelper.AddOrUpdateBlockTypeAttribute( "176095E9-3BEB-44DC-AADB-B5CCA8F479DB", "46A03F59-55D3-4ACE-ADD5-B4642225DD20", "Workflow Type", "WorkflowType", "Workflow Type", @"The workflow type to activate for check-in", 0, @"", "343396C2-6ADC-48E7-9DAA-0AC7FBAC1024" );

            // Attribute for BlockType
            //   BlockType: Pager Entry
            //   Category: KFS > Check-in
            //   Attribute: Workflow Activity
            RockMigrationHelper.AddOrUpdateBlockTypeAttribute( "176095E9-3BEB-44DC-AADB-B5CCA8F479DB", "9C204CD0-1233-41C5-818A-C5DA439445AA", "Workflow Activity", "WorkflowActivity", "Workflow Activity", @"The name of the workflow activity to run on selection.", 1, @"", "5E2512EA-C543-452C-B242-238693CE230B" );

            // Attribute for BlockType
            //   BlockType: Pager Entry
            //   Category: KFS > Check-in
            //   Attribute: Home Page
            RockMigrationHelper.AddOrUpdateBlockTypeAttribute( "176095E9-3BEB-44DC-AADB-B5CCA8F479DB", "BD53F9C9-EBA9-4D3F-82EA-DE5DD34A8108", "Home Page", "HomePage", "Home Page", @"", 2, @"", "BE422E2F-EC4F-4DED-A512-AB51F198A97B" );

            // Attribute for BlockType
            //   BlockType: Pager Entry
            //   Category: KFS > Check-in
            //   Attribute: Previous Page
            RockMigrationHelper.AddOrUpdateBlockTypeAttribute( "176095E9-3BEB-44DC-AADB-B5CCA8F479DB", "BD53F9C9-EBA9-4D3F-82EA-DE5DD34A8108", "Previous Page", "PreviousPage", "Previous Page", @"", 3, @"", "D2856DFC-2BD3-4752-8F44-1F253195521D" );

            // Attribute for BlockType
            //   BlockType: Pager Entry
            //   Category: KFS > Check-in
            //   Attribute: Next Page
            RockMigrationHelper.AddOrUpdateBlockTypeAttribute( "176095E9-3BEB-44DC-AADB-B5CCA8F479DB", "BD53F9C9-EBA9-4D3F-82EA-DE5DD34A8108", "Next Page", "NextPage", "Next Page", @"", 4, @"", "A09221A7-564C-47EB-9FE2-DD7D012A3527" );

            // Attribute for BlockType
            //   BlockType: Pager Entry
            //   Category: KFS > Check-in
            //   Attribute: Multi-Person First Page (Family Check-in)
            RockMigrationHelper.AddOrUpdateBlockTypeAttribute( "176095E9-3BEB-44DC-AADB-B5CCA8F479DB", "BD53F9C9-EBA9-4D3F-82EA-DE5DD34A8108", "Multi-Person First Page (Family Check-in)", "MultiPersonFirstPage", "Multi-Person First Page (Family Check-in)", @"The first page for each person during family check-in.", 5, @"", "641CDA04-E310-4440-A5BB-6DFBBD614AED" );

            // Attribute for BlockType
            //   BlockType: Pager Entry
            //   Category: KFS > Check-in
            //   Attribute: Multi-Person Last Page  (Family Check-in)
            RockMigrationHelper.AddOrUpdateBlockTypeAttribute( "176095E9-3BEB-44DC-AADB-B5CCA8F479DB", "BD53F9C9-EBA9-4D3F-82EA-DE5DD34A8108", "Multi-Person Last Page  (Family Check-in)", "MultiPersonLastPage", "Multi-Person Last Page  (Family Check-in)", @"The last page for each person during family check-in.", 6, @"", "47013CF8-A969-480A-BEA5-F8E9497EF2B3" );

            // Attribute for BlockType
            //   BlockType: Pager Entry
            //   Category: KFS > Check-in
            //   Attribute: Multi-Person Done Page (Family Check-in)
            RockMigrationHelper.AddOrUpdateBlockTypeAttribute( "176095E9-3BEB-44DC-AADB-B5CCA8F479DB", "BD53F9C9-EBA9-4D3F-82EA-DE5DD34A8108", "Multi-Person Done Page (Family Check-in)", "MultiPersonDonePage", "Multi-Person Done Page (Family Check-in)", @"The page to navigate to once all people have checked in during family check-in.", 7, @"", "1881C018-BEA2-40BA-A6E0-C4654500B970" );

            // Attribute for BlockType
            //   BlockType: Pager Entry
            //   Category: KFS > Check-in
            //   Attribute: Display Keypad
            RockMigrationHelper.AddOrUpdateBlockTypeAttribute( "176095E9-3BEB-44DC-AADB-B5CCA8F479DB", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Display Keypad", "DisplayKeypad", "Display Keypad", @"If your pager id's are numbers only and you have touch screen kiosks you can enable a touch screen keypad.", 7, @"False", "5BE8E0C1-CEFA-4C51-8889-5305E3CF4F3D" );

            // Attribute for BlockType
            //   BlockType: Pager Entry
            //   Category: KFS > Check-in
            //   Attribute: Pager Attribute Key
            RockMigrationHelper.AddOrUpdateBlockTypeAttribute( "176095E9-3BEB-44DC-AADB-B5CCA8F479DB", "9C204CD0-1233-41C5-818A-C5DA439445AA", "Pager Attribute Key", "PagerAttribute", "Pager Attribute Key", @"Attribute Key on Family Group type for Pager.", 8, @"rocks.kfs.PagerNumber", "9462972C-B6DA-4741-B707-2DBB23ADFCF8" );

            // Add Block Attribute Value
            //   Block: Pager Entry
            //   BlockType: Pager Entry
            //   Category: KFS > Check-in
            //   Block Location: Page=Pager Number, Site=Rock Check-in
            //   Attribute: Title Template
            /*   Attribute Value: {{ Family.Name }} */
            RockMigrationHelper.AddBlockAttributeValue( "A6FE5ABF-25BD-49E2-AB9B-1820FB1AC8B9", "0DDB5F00-F3E6-498D-8BE3-965B0BBDC9BD", @"{{ Family.Name }}" );

            // Add Block Attribute Value
            //   Block: Pager Entry
            //   BlockType: Pager Entry
            //   Category: KFS > Check-in
            //   Block Location: Page=Pager Number, Site=Rock Check-in
            //   Attribute: Caption
            /*   Attribute Value: Please enter the pager number */
            RockMigrationHelper.AddBlockAttributeValue( "A6FE5ABF-25BD-49E2-AB9B-1820FB1AC8B9", "F7DAF664-1538-49CE-B009-FE70421DD1AC", @"Please enter the pager number" );

            // Add Block Attribute Value
            //   Block: Pager Entry
            //   BlockType: Pager Entry
            //   Category: KFS > Check-in
            //   Block Location: Page=Pager Number, Site=Rock Check-in
            //   Attribute: Workflow Type
            /*   Attribute Value: 011e9f5a-60d4-4ff5-912a-290881e37eaf */
            RockMigrationHelper.AddBlockAttributeValue( "A6FE5ABF-25BD-49E2-AB9B-1820FB1AC8B9", "343396C2-6ADC-48E7-9DAA-0AC7FBAC1024", @"011e9f5a-60d4-4ff5-912a-290881e37eaf" );

            // Add Block Attribute Value
            //   Block: Pager Entry
            //   BlockType: Pager Entry
            //   Category: KFS > Check-in
            //   Block Location: Page=Pager Number, Site=Rock Check-in
            //   Attribute: Home Page
            /*   Attribute Value: 432b615a-75ff-4b14-9c99-3e769f866950 */
            RockMigrationHelper.AddBlockAttributeValue( "A6FE5ABF-25BD-49E2-AB9B-1820FB1AC8B9", "BE422E2F-EC4F-4DED-A512-AB51F198A97B", @"432b615a-75ff-4b14-9c99-3e769f866950" );

            // Add Block Attribute Value
            //   Block: Pager Entry
            //   BlockType: Pager Entry
            //   Category: KFS > Check-in
            //   Block Location: Page=Pager Number, Site=Rock Check-in
            //   Attribute: Previous Page
            /*   Attribute Value: c0afa081-b64e-4006-bffc-a350a51ae4cc */
            RockMigrationHelper.AddBlockAttributeValue( "A6FE5ABF-25BD-49E2-AB9B-1820FB1AC8B9", "D2856DFC-2BD3-4752-8F44-1F253195521D", @"c0afa081-b64e-4006-bffc-a350a51ae4cc" );

            // Add Block Attribute Value
            //   Block: Pager Entry
            //   BlockType: Pager Entry
            //   Category: KFS > Check-in
            //   Block Location: Page=Pager Number, Site=Rock Check-in
            //   Attribute: Next Page
            /*   Attribute Value: e08230b8-35a4-40d6-a0bb-521418314da9 */
            RockMigrationHelper.AddBlockAttributeValue( "A6FE5ABF-25BD-49E2-AB9B-1820FB1AC8B9", "A09221A7-564C-47EB-9FE2-DD7D012A3527", @"e08230b8-35a4-40d6-a0bb-521418314da9" );

            // Add Block Attribute Value
            //   Block: Pager Entry
            //   BlockType: Pager Entry
            //   Category: KFS > Check-in
            //   Block Location: Page=Pager Number, Site=Rock Check-in
            //   Attribute: Multi-Person First Page (Family Check-in)
            /*   Attribute Value: d14154ba-2f2c-41c3-b380-f833252cbb13 */
            RockMigrationHelper.AddBlockAttributeValue( "A6FE5ABF-25BD-49E2-AB9B-1820FB1AC8B9", "641CDA04-E310-4440-A5BB-6DFBBD614AED", @"d14154ba-2f2c-41c3-b380-f833252cbb13" );

            // Add Block Attribute Value
            //   Block: Pager Entry
            //   BlockType: Pager Entry
            //   Category: KFS > Check-in
            //   Block Location: Page=Pager Number, Site=Rock Check-in
            //   Attribute: Multi-Person Done Page (Family Check-in)
            /*   Attribute Value: 4af7a0e1-e991-4ae5-a2b5-c440f67a2e6a */
            RockMigrationHelper.AddBlockAttributeValue( "A6FE5ABF-25BD-49E2-AB9B-1820FB1AC8B9", "1881C018-BEA2-40BA-A6E0-C4654500B970", @"4af7a0e1-e991-4ae5-a2b5-c440f67a2e6a" );

            // Add Block Attribute Value
            //   Block: Pager Entry
            //   BlockType: Pager Entry
            //   Category: KFS > Check-in
            //   Block Location: Page=Pager Number, Site=Rock Check-in
            //   Attribute: Display Keypad
            /*   Attribute Value: False */
            RockMigrationHelper.AddBlockAttributeValue( "A6FE5ABF-25BD-49E2-AB9B-1820FB1AC8B9", "5BE8E0C1-CEFA-4C51-8889-5305E3CF4F3D", @"False" );

            // Add Block Attribute Value
            //   Block: Pager Entry
            //   BlockType: Pager Entry
            //   Category: KFS > Check-in
            //   Block Location: Page=Pager Number, Site=Rock Check-in
            //   Attribute: Pager Attribute Key
            /*   Attribute Value: rocks.kfs.PagerNumber */
            RockMigrationHelper.AddBlockAttributeValue( "A6FE5ABF-25BD-49E2-AB9B-1820FB1AC8B9", "9462972C-B6DA-4741-B707-2DBB23ADFCF8", @"rocks.kfs.PagerNumber" );

            // Add Block Attribute Value
            //   Block: Family Attributes
            //   BlockType: HTML Content
            //   Category: CMS
            //   Block Location: Page=Attendance Detail, Site=Rock Check-in Manager
            //   Attribute: Require Approval
            /*   Attribute Value: False */
            RockMigrationHelper.AddBlockAttributeValue( "83763A73-794F-49D9-BC1E-DD34AC628BFB", "EC2B701B-4C1D-4F3F-9C77-A73C75D7FF7A", @"False" );

            // Add Block Attribute Value
            //   Block: Family Attributes
            //   BlockType: HTML Content
            //   Category: CMS
            //   Block Location: Page=Attendance Detail, Site=Rock Check-in Manager
            //   Attribute: Enable Versioning
            /*   Attribute Value: False */
            RockMigrationHelper.AddBlockAttributeValue( "83763A73-794F-49D9-BC1E-DD34AC628BFB", "7C1CE199-86CF-4EAE-8AB3-848416A72C58", @"False" );

            // Add Block Attribute Value
            //   Block: Family Attributes
            //   BlockType: HTML Content
            //   Category: CMS
            //   Block Location: Page=Attendance Detail, Site=Rock Check-in Manager
            //   Attribute: Start in Code Editor mode
            /*   Attribute Value: True */
            RockMigrationHelper.AddBlockAttributeValue( "83763A73-794F-49D9-BC1E-DD34AC628BFB", "0673E015-F8DD-4A52-B380-C758011331B2", @"True" );

            // Add Block Attribute Value
            //   Block: Family Attributes
            //   BlockType: HTML Content
            //   Category: CMS
            //   Block Location: Page=Attendance Detail, Site=Rock Check-in Manager
            //   Attribute: Document Root Folder
            /*   Attribute Value: ~/Content */
            RockMigrationHelper.AddBlockAttributeValue( "83763A73-794F-49D9-BC1E-DD34AC628BFB", "3BDB8AED-32C5-4879-B1CB-8FC7C8336534", @"~/Content" );

            // Add Block Attribute Value
            //   Block: Family Attributes
            //   BlockType: HTML Content
            //   Category: CMS
            //   Block Location: Page=Attendance Detail, Site=Rock Check-in Manager
            //   Attribute: Image Root Folder
            /*   Attribute Value: ~/Content */
            RockMigrationHelper.AddBlockAttributeValue( "83763A73-794F-49D9-BC1E-DD34AC628BFB", "26F3AFC6-C05B-44A4-8593-AFE1D9969B0E", @"~/Content" );

            // Add Block Attribute Value
            //   Block: Family Attributes
            //   BlockType: HTML Content
            //   Category: CMS
            //   Block Location: Page=Attendance Detail, Site=Rock Check-in Manager
            //   Attribute: User Specific Folders
            /*   Attribute Value: False */
            RockMigrationHelper.AddBlockAttributeValue( "83763A73-794F-49D9-BC1E-DD34AC628BFB", "9D3E4ED9-1BEF-4547-B6B0-CE29FE3835EE", @"False" );

            // Add Block Attribute Value
            //   Block: Family Attributes
            //   BlockType: HTML Content
            //   Category: CMS
            //   Block Location: Page=Attendance Detail, Site=Rock Check-in Manager
            //   Attribute: Cache Duration
            /*   Attribute Value: 0 */
            RockMigrationHelper.AddBlockAttributeValue( "83763A73-794F-49D9-BC1E-DD34AC628BFB", "4DFDB295-6D0F-40A1-BEF9-7B70C56F66C4", @"0" );

            // Add Block Attribute Value
            //   Block: Family Attributes
            //   BlockType: HTML Content
            //   Category: CMS
            //   Block Location: Page=Attendance Detail, Site=Rock Check-in Manager
            //   Attribute: Is Secondary Block
            /*   Attribute Value: False */
            RockMigrationHelper.AddBlockAttributeValue( "83763A73-794F-49D9-BC1E-DD34AC628BFB", "04C15DC1-DFB6-4D63-A7BC-0507D0E33EF4", @"False" );

            // Add Block Attribute Value
            //   Block: Family Attributes
            //   BlockType: HTML Content
            //   Category: CMS
            //   Block Location: Page=Attendance Detail, Site=Rock Check-in Manager
            //   Attribute: Enabled Lava Commands
            /*   Attribute Value: RockEntity */
            RockMigrationHelper.AddBlockAttributeValue( "83763A73-794F-49D9-BC1E-DD34AC628BFB", "7146AC24-9250-4FC4-9DF2-9803B9A84299", @"RockEntity" );

            // Add Block Attribute Value
            //   Block: Family Attributes
            //   BlockType: HTML Content
            //   Category: CMS
            //   Block Location: Page=Person Profile, Site=Rock Check-in Manager
            //   Attribute: Enabled Lava Commands
            /*   Attribute Value: RockEntity */
            RockMigrationHelper.AddBlockAttributeValue( "AC41EA0D-2385-4797-9EAB-B8E1990B224E", "7146AC24-9250-4FC4-9DF2-9803B9A84299", @"RockEntity" );

            // Add Block Attribute Value
            //   Block: Family Attributes
            //   BlockType: HTML Content
            //   Category: CMS
            //   Block Location: Page=Person Profile, Site=Rock Check-in Manager
            //   Attribute: Is Secondary Block
            /*   Attribute Value: False */
            RockMigrationHelper.AddBlockAttributeValue( "AC41EA0D-2385-4797-9EAB-B8E1990B224E", "04C15DC1-DFB6-4D63-A7BC-0507D0E33EF4", @"False" );

            // Add Block Attribute Value
            //   Block: Family Attributes
            //   BlockType: HTML Content
            //   Category: CMS
            //   Block Location: Page=Person Profile, Site=Rock Check-in Manager
            //   Attribute: User Specific Folders
            /*   Attribute Value: False */
            RockMigrationHelper.AddBlockAttributeValue( "AC41EA0D-2385-4797-9EAB-B8E1990B224E", "9D3E4ED9-1BEF-4547-B6B0-CE29FE3835EE", @"False" );

            // Add Block Attribute Value
            //   Block: Family Attributes
            //   BlockType: HTML Content
            //   Category: CMS
            //   Block Location: Page=Person Profile, Site=Rock Check-in Manager
            //   Attribute: Cache Duration
            /*   Attribute Value: 0 */
            RockMigrationHelper.AddBlockAttributeValue( "AC41EA0D-2385-4797-9EAB-B8E1990B224E", "4DFDB295-6D0F-40A1-BEF9-7B70C56F66C4", @"0" );

            // Add Block Attribute Value
            //   Block: Family Attributes
            //   BlockType: HTML Content
            //   Category: CMS
            //   Block Location: Page=Person Profile, Site=Rock Check-in Manager
            //   Attribute: Document Root Folder
            /*   Attribute Value: ~/Content */
            RockMigrationHelper.AddBlockAttributeValue( "AC41EA0D-2385-4797-9EAB-B8E1990B224E", "3BDB8AED-32C5-4879-B1CB-8FC7C8336534", @"~/Content" );

            // Add Block Attribute Value
            //   Block: Family Attributes
            //   BlockType: HTML Content
            //   Category: CMS
            //   Block Location: Page=Person Profile, Site=Rock Check-in Manager
            //   Attribute: Image Root Folder
            /*   Attribute Value: ~/Content */
            RockMigrationHelper.AddBlockAttributeValue( "AC41EA0D-2385-4797-9EAB-B8E1990B224E", "26F3AFC6-C05B-44A4-8593-AFE1D9969B0E", @"~/Content" );

            // Add Block Attribute Value
            //   Block: Family Attributes
            //   BlockType: HTML Content
            //   Category: CMS
            //   Block Location: Page=Person Profile, Site=Rock Check-in Manager
            //   Attribute: Start in Code Editor mode
            /*   Attribute Value: True */
            RockMigrationHelper.AddBlockAttributeValue( "AC41EA0D-2385-4797-9EAB-B8E1990B224E", "0673E015-F8DD-4A52-B380-C758011331B2", @"True" );

            // Add Block Attribute Value
            //   Block: Family Attributes
            //   BlockType: HTML Content
            //   Category: CMS
            //   Block Location: Page=Person Profile, Site=Rock Check-in Manager
            //   Attribute: Enable Versioning
            /*   Attribute Value: False */
            RockMigrationHelper.AddBlockAttributeValue( "AC41EA0D-2385-4797-9EAB-B8E1990B224E", "7C1CE199-86CF-4EAE-8AB3-848416A72C58", @"False" );

            // Add Block Attribute Value
            //   Block: Family Attributes
            //   BlockType: HTML Content
            //   Category: CMS
            //   Block Location: Page=Person Profile, Site=Rock Check-in Manager
            //   Attribute: Require Approval
            /*   Attribute Value: False */
            RockMigrationHelper.AddBlockAttributeValue( "AC41EA0D-2385-4797-9EAB-B8E1990B224E", "EC2B701B-4C1D-4F3F-9C77-A73C75D7FF7A", @"False" );

            // Add Block Attribute Value
            //   Block: Idle Redirect
            //   BlockType: Idle Redirect
            //   Category: Utility
            //   Block Location: Page=Pager Number, Site=Rock Check-in
            //   Attribute: Idle Seconds
            /*   Attribute Value: 60 */
            RockMigrationHelper.AddBlockAttributeValue( "F56ADEDA-45D5-4597-8387-633B2B1A9DCC", "1CAC7B16-041A-4F40-8AEE-A39DFA076C14", @"60" );

            // Add Block Attribute Value
            //   Block: Idle Redirect
            //   BlockType: Idle Redirect
            //   Category: Utility
            //   Block Location: Page=Pager Number, Site=Rock Check-in
            //   Attribute: New Location
            /*   Attribute Value: /checkin/welcome */
            RockMigrationHelper.AddBlockAttributeValue( "F56ADEDA-45D5-4597-8387-633B2B1A9DCC", "2254B67B-9CB1-47DE-A63D-D0B56051ECD4", @"/checkin/welcome" );

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
        }

        /// <summary>
        /// The commands to undo a migration from a specific version
        /// </summary>
        public override void Down()
        {
            // Attribute for BlockType
            //   BlockType: Pager Entry
            //   Category: KFS > Check-in
            //   Attribute: Pager Attribute Key
            RockMigrationHelper.DeleteAttribute( "9462972C-B6DA-4741-B707-2DBB23ADFCF8" );

            // Attribute for BlockType
            //   BlockType: Pager Entry
            //   Category: KFS > Check-in
            //   Attribute: Display Keypad
            RockMigrationHelper.DeleteAttribute( "5BE8E0C1-CEFA-4C51-8889-5305E3CF4F3D" );

            // Attribute for BlockType
            //   BlockType: Pager Entry
            //   Category: KFS > Check-in
            //   Attribute: Multi-Person Done Page (Family Check-in)
            RockMigrationHelper.DeleteAttribute( "1881C018-BEA2-40BA-A6E0-C4654500B970" );

            // Attribute for BlockType
            //   BlockType: Pager Entry
            //   Category: KFS > Check-in
            //   Attribute: Multi-Person Last Page  (Family Check-in)
            RockMigrationHelper.DeleteAttribute( "47013CF8-A969-480A-BEA5-F8E9497EF2B3" );

            // Attribute for BlockType
            //   BlockType: Pager Entry
            //   Category: KFS > Check-in
            //   Attribute: Multi-Person First Page (Family Check-in)
            RockMigrationHelper.DeleteAttribute( "641CDA04-E310-4440-A5BB-6DFBBD614AED" );

            // Attribute for BlockType
            //   BlockType: Pager Entry
            //   Category: KFS > Check-in
            //   Attribute: Next Page
            RockMigrationHelper.DeleteAttribute( "A09221A7-564C-47EB-9FE2-DD7D012A3527" );

            // Attribute for BlockType
            //   BlockType: Pager Entry
            //   Category: KFS > Check-in
            //   Attribute: Previous Page
            RockMigrationHelper.DeleteAttribute( "D2856DFC-2BD3-4752-8F44-1F253195521D" );

            // Attribute for BlockType
            //   BlockType: Pager Entry
            //   Category: KFS > Check-in
            //   Attribute: Home Page
            RockMigrationHelper.DeleteAttribute( "BE422E2F-EC4F-4DED-A512-AB51F198A97B" );

            // Attribute for BlockType
            //   BlockType: Pager Entry
            //   Category: KFS > Check-in
            //   Attribute: Workflow Activity
            RockMigrationHelper.DeleteAttribute( "5E2512EA-C543-452C-B242-238693CE230B" );

            // Attribute for BlockType
            //   BlockType: Pager Entry
            //   Category: KFS > Check-in
            //   Attribute: Workflow Type
            RockMigrationHelper.DeleteAttribute( "343396C2-6ADC-48E7-9DAA-0AC7FBAC1024" );

            // Attribute for BlockType
            //   BlockType: Pager Entry
            //   Category: KFS > Check-in
            //   Attribute: Caption
            RockMigrationHelper.DeleteAttribute( "F7DAF664-1538-49CE-B009-FE70421DD1AC" );

            // Attribute for BlockType
            //   BlockType: Pager Entry
            //   Category: KFS > Check-in
            //   Attribute: Title Template
            RockMigrationHelper.DeleteAttribute( "0DDB5F00-F3E6-498D-8BE3-965B0BBDC9BD" );

            // Remove Block
            //  Name: Family Attributes, from Page: Person Profile, Site: Rock Check-in Manager
            //  from Page: Person Profile, Site: Rock Check-in Manager
            RockMigrationHelper.DeleteBlock( "AC41EA0D-2385-4797-9EAB-B8E1990B224E" );

            // Remove Block
            //  Name: Family Attributes, from Page: Attendance Detail, Site: Rock Check-in Manager
            //  from Page: Attendance Detail, Site: Rock Check-in Manager
            RockMigrationHelper.DeleteBlock( "83763A73-794F-49D9-BC1E-DD34AC628BFB" );

            // Remove Block
            //  Name: Pager Entry, from Page: Pager Number, Site: Rock Check-in
            //  from Page: Pager Number, Site: Rock Check-in
            RockMigrationHelper.DeleteBlock( "A6FE5ABF-25BD-49E2-AB9B-1820FB1AC8B9" );

            // Delete BlockType 
            //   Name: Pager Entry
            //   Category: KFS > Check-in
            //   Path: ~/Plugins/rocks_kfs/CheckIn/PagerEntry.ascx
            //   EntityType: -
            RockMigrationHelper.DeleteBlockType( "176095E9-3BEB-44DC-AADB-B5CCA8F479DB" );

            // Delete Page 
            //  Internal Name: Pager Number
            //  Site: Rock Check-in
            //  Layout: Checkin
            RockMigrationHelper.DeletePage( "50A1708F-D751-40C5-BE99-492C4E81AED0" );
        }
    }
}
