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
using Rock;
using Rock.Plugin;

namespace rocks.kfs.StepsToCare.Migrations
{
    [MigrationNumber( 4, "1.12.3" )]
    public class CreatePages : Migration
    {
        public override void Up()
        {
            // Add Page Steps to Care to Site:Rock RMS
            RockMigrationHelper.AddPage( true, "5B6DBC42-8B03-4D15-8D92-AAFA28FD8616", "D65F783D-87A9-4CC9-8110-E83466A0EADB", "Steps to Care", "", "1F93E9AA-ECCA-42A2-8C91-73D991DBCD9F", "fa fa-hand-holding-heart" );

            // Add Page Care Entry to Site:Rock RMS
            RockMigrationHelper.AddPage( true, "1F93E9AA-ECCA-42A2-8C91-73D991DBCD9F", "D65F783D-87A9-4CC9-8110-E83466A0EADB", "Care Entry", "", "27953B65-21E2-4CA9-8461-3AFAD46D9BC8", "" );

            // Add Page Care Configuration to Site:Rock RMS
            RockMigrationHelper.AddPage( true, "1F93E9AA-ECCA-42A2-8C91-73D991DBCD9F", "D65F783D-87A9-4CC9-8110-E83466A0EADB", "Care Configuration", "", "39F72E9D-22B7-4F1E-8633-6C3745AC6F34", "" );

            // Add Page Route for Steps to Care
            RockMigrationHelper.AddPageRoute( "1F93E9AA-ECCA-42A2-8C91-73D991DBCD9F", "StepsToCare", "9FA7606C-13BC-4049-A1A1-0389C98C3743" );

            // Add Page Route for Care Entry
            RockMigrationHelper.AddPageRoute( "27953B65-21E2-4CA9-8461-3AFAD46D9BC8", "StepsToCareDetail", "7DA0C653-88E4-4997-89BA-C58B33A2AA32" );

            // Add Page Route for Care Entry
            RockMigrationHelper.AddPageRoute( "27953B65-21E2-4CA9-8461-3AFAD46D9BC8", "StepsToCareDetail/{CareNeedId}", "32CDFBC1-F3EB-405B-9462-F7E2DD722006" );

            // Add/Update BlockType Care Entry
            RockMigrationHelper.UpdateBlockType( "Care Entry", "Care entry page for KFS Steps to Care package. Used for adding and editing care needs ", "~/Plugins/rocks_kfs/StepsToCare/CareEntry.ascx", "KFS > Steps To Care", "4F0F9ED7-9F74-4152-B27F-D9B2A458AFBE" );

            // Add/Update BlockType Care Dashboard
            RockMigrationHelper.UpdateBlockType( "Care Dashboard", "Care dashboard block for KFS Steps to Care package. ", "~/Plugins/rocks_kfs/StepsToCare/CareDashboard.ascx", "KFS > Steps To Care", "AF14CB6C-F915-4449-9CB7-7C44B624B051" );

            // Add/Update BlockType Care Note Templates
            RockMigrationHelper.UpdateBlockType( "Care Note Templates", "Care Note Templates block for KFS Steps to Care package. Used for adding and editing care Note Templates for adding quick notes to needs.", "~/Plugins/rocks_kfs/StepsToCare/CareNoteTemplates.ascx", "KFS > Steps To Care", "561E0D77-12F9-4863-B5E3-4C5F36FB2DB1" );

            // Add/Update BlockType Care Workers
            RockMigrationHelper.UpdateBlockType( "Care Workers", "Care workers block for KFS Steps to Care package. Used for adding and editing care workers for assignment.", "~/Plugins/rocks_kfs/StepsToCare/CareWorkers.ascx", "KFS > Steps To Care", "FC3E03F3-80A0-42BF-AAFB-DD2095B7BE86" );

            // Add Block Care Dashboard to Page: Steps to Care, Site: Rock RMS
            RockMigrationHelper.AddBlock( true, "1F93E9AA-ECCA-42A2-8C91-73D991DBCD9F".AsGuid(), null, "C2D29296-6A87-47A9-A753-EE4E9159C4C4".AsGuid(), "AF14CB6C-F915-4449-9CB7-7C44B624B051".AsGuid(), "Care Dashboard", "Main", @"", @"", 0, "EADBE3F0-F64B-4583-B49D-F0031BBC929F" );

            // Add Block Care Entry to Page: Care Entry, Site: Rock RMS
            RockMigrationHelper.AddBlock( true, "27953B65-21E2-4CA9-8461-3AFAD46D9BC8".AsGuid(), null, "C2D29296-6A87-47A9-A753-EE4E9159C4C4".AsGuid(), "4F0F9ED7-9F74-4152-B27F-D9B2A458AFBE".AsGuid(), "Care Entry", "Main", @"", @"", 0, "F953C5EF-6504-45F9-81A8-063518B7AB61" );

            // Add Block Care Notes to Page: Care Entry, Site: Rock RMS
            RockMigrationHelper.AddBlock( true, "27953B65-21E2-4CA9-8461-3AFAD46D9BC8".AsGuid(), null, "C2D29296-6A87-47A9-A753-EE4E9159C4C4".AsGuid(), "2E9F32D4-B4FC-4A5F-9BE1-B2E3EA624DD3".AsGuid(), "Care Notes", "SectionA", @"", @"", 0, "A9F1F1C1-031B-4D87-BF52-0E5BC5423AC6" );

            // Add Block Care Workers to Page: Care Configuration, Site: Rock RMS
            RockMigrationHelper.AddBlock( true, "39F72E9D-22B7-4F1E-8633-6C3745AC6F34".AsGuid(), null, "C2D29296-6A87-47A9-A753-EE4E9159C4C4".AsGuid(), "FC3E03F3-80A0-42BF-AAFB-DD2095B7BE86".AsGuid(), "Care Workers", "Main", @"", @"", 0, "8B0BBF03-2BAE-4B7D-9A4B-1CAE23F8E02E" );

            // Add Block Care Note Templates to Page: Care Configuration, Site: Rock RMS
            RockMigrationHelper.AddBlock( true, "39F72E9D-22B7-4F1E-8633-6C3745AC6F34".AsGuid(), null, "C2D29296-6A87-47A9-A753-EE4E9159C4C4".AsGuid(), "561E0D77-12F9-4863-B5E3-4C5F36FB2DB1".AsGuid(), "Care Note Templates", "SectionB", @"", @"", 1, "E4108097-F10B-4404-BD3C-D36D8DEB1A8B" );

            // Add Block Defined Value List - Categories to Page: Care Configuration, Site: Rock RMS
            RockMigrationHelper.AddBlock( true, "39F72E9D-22B7-4F1E-8633-6C3745AC6F34".AsGuid(), null, "C2D29296-6A87-47A9-A753-EE4E9159C4C4".AsGuid(), "0AB2D5E9-9272-47D5-90E4-4AA838D2D3EE".AsGuid(), "Defined Value List - Categories", "SectionC", @"", @"", 0, "F588AEE4-3BFE-4025-AFF0-5C1569434924" );

            // Add Block Defined Value List - Status to Page: Care Configuration, Site: Rock RMS
            RockMigrationHelper.AddBlock( true, "39F72E9D-22B7-4F1E-8633-6C3745AC6F34".AsGuid(), null, "C2D29296-6A87-47A9-A753-EE4E9159C4C4".AsGuid(), "0AB2D5E9-9272-47D5-90E4-4AA838D2D3EE".AsGuid(), "Defined Value List - Status", "SectionD", @"", @"", 1, "362581DC-6224-40FA-B351-FC2572022166" );

            // update block order for pages with new blocks if the page,zone has multiple blocks

            // Update Order for Page: Care Configuration,  Zone: Main,  Block: Care Note Templates
            Sql( @"UPDATE [Block] SET [Order] = 1 WHERE [Guid] = 'E4108097-F10B-4404-BD3C-D36D8DEB1A8B'" );

            // Update Order for Page: Care Configuration,  Zone: Main,  Block: Care Workers
            Sql( @"UPDATE [Block] SET [Order] = 0 WHERE [Guid] = '8B0BBF03-2BAE-4B7D-9A4B-1CAE23F8E02E'" );

            // Update Order for Page: Care Configuration,  Zone: SectionA,  Block: Defined Value List - Categories
            Sql( @"UPDATE [Block] SET [Order] = 0 WHERE [Guid] = 'F588AEE4-3BFE-4025-AFF0-5C1569434924'" );

            // Update Order for Page: Care Configuration,  Zone: SectionA,  Block: Defined Value List - Status
            Sql( @"UPDATE [Block] SET [Order] = 1 WHERE [Guid] = '362581DC-6224-40FA-B351-FC2572022166'" );

            // Update Order for Page: Steps to Care,  Zone: Main,  Block: Care Dashboard
            Sql( @"UPDATE [Block] SET [Order] = 0 WHERE [Guid] = 'EADBE3F0-F64B-4583-B49D-F0031BBC929F'" );

            // Attribute for BlockType: Care Entry:Allow New Person Entry
            RockMigrationHelper.AddOrUpdateBlockTypeAttribute( "4F0F9ED7-9F74-4152-B27F-D9B2A458AFBE", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Allow New Person Entry", "AllowNewPerson", "Allow New Person Entry", @"Should you be able to enter a new person from the care entry form and use person matching?", 0, @"False", "E6D1F331-1FCA-4DA7-8C55-A7A40C94B906" );

            // Attribute for BlockType: Care Entry:Group Type and Role
            RockMigrationHelper.AddOrUpdateBlockTypeAttribute( "4F0F9ED7-9F74-4152-B27F-D9B2A458AFBE", "3BB25568-E793-4D12-AE80-AC3FDA6FD8A8", "Group Type and Role", "GroupTypeAndRole", "Group Type and Role", @"Select the group Type and Role of the leader you would like auto assigned to care need. If none are selected it will not auto assign the small group member to the need. ", 0, @"", "664F6632-A438-4249-ADB0-D94B891BF089" );

            // Attribute for BlockType: Care Entry:Auto Assign Worker with Geofence
            RockMigrationHelper.AddOrUpdateBlockTypeAttribute( "4F0F9ED7-9F74-4152-B27F-D9B2A458AFBE", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Auto Assign Worker with Geofence", "AutoAssignWorkerGeofence", "Auto Assign Worker with Geofence", @"Care Need Workers can have Geofence locations assigned to them, if there are workers with geofences and this block setting is enabled it will auto assign workers to this need on new entries based on the requester home being in the geofence.", 0, @"True", "C5DE9EAE-BB28-4D81-AAB0-B7F338D030A3" );

            // Attribute for BlockType: Care Entry:Auto Assign Worker (load balanced)
            RockMigrationHelper.AddOrUpdateBlockTypeAttribute( "4F0F9ED7-9F74-4152-B27F-D9B2A458AFBE", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Auto Assign Worker (load balanced)", "AutoAssignWorker", "Auto Assign Worker in Round Robin", @"Use intelligent load balancing to auto assign care workers to a care need based on their workload and other parameters?", 0, @"True", "F7135F9C-8D06-4E6D-A493-2B1B29CE9AB6" );

            // Attribute for BlockType: Care Entry:Newly Assigned Need Notification
            RockMigrationHelper.AddOrUpdateBlockTypeAttribute( "4F0F9ED7-9F74-4152-B27F-D9B2A458AFBE", "72ED40C7-4D64-4D60-9411-4FFB2B9E833E", "Newly Assigned Need Notification", "NewAssignmentNotification", "Newly Assigned Need Notification", @"Select the system communication template for the new assignment notification.", 0, SystemGuid.SystemCommunication.CARE_NEED_ASSIGNED, "E3604688-ACC2-4360-976B-44EB5170F685" );

            // Attribute for BlockType: Care Dashboard:Prayer Detail Page
            RockMigrationHelper.AddOrUpdateBlockTypeAttribute( "AF14CB6C-F915-4449-9CB7-7C44B624B051", "BD53F9C9-EBA9-4D3F-82EA-DE5DD34A8108", "Prayer Detail Page", "PrayerDetailPage", "Prayer Detail Page", @"Page used to convert needs to prayer requests. (if not set the action will not show)", 7, @"", "611CB530-08A9-4D29-8067-7BCB4FE4F300" );

            // Attribute for BlockType: Care Dashboard:Benevolence Detail Page
            RockMigrationHelper.AddOrUpdateBlockTypeAttribute( "AF14CB6C-F915-4449-9CB7-7C44B624B051", "BD53F9C9-EBA9-4D3F-82EA-DE5DD34A8108", "Benevolence Detail Page", "BenevolenceDetailPage", "Benevolence Detail Page", @"Page used to convert needs to benevolence requests. (if not set the action will not show)", 8, @"", "74BA310B-20FB-4856-819E-5606291B7ADA" );

            // Attribute for BlockType: Care Dashboard:Filter Connection Types
            RockMigrationHelper.AddOrUpdateBlockTypeAttribute( "AF14CB6C-F915-4449-9CB7-7C44B624B051", "E4E72958-4604-498F-956B-BA095976A60B", "Filter Connection Types", "IncludeConnectionTypes", "Include Connection Types", @"Filter down the connection types to include only these selected types.", 10, @"", "387CA195-C41F-4EC6-AD5D-69FABAC68AF9" );

            // Attribute for BlockType: Care Dashboard:Enable Convert to Connection Request
            RockMigrationHelper.AddOrUpdateBlockTypeAttribute( "AF14CB6C-F915-4449-9CB7-7C44B624B051", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Enable Convert to Connection Request", "ConnectionRequestEnable", "Convert to Connection Request", @"Enable Convert to Connection Request Action", 9, @"False", "9D1C58A6-FBD8-454B-8383-1060A7629D3E" );

            // Attribute for BlockType: Care Dashboard:Enable Launch Workflow
            RockMigrationHelper.AddOrUpdateBlockTypeAttribute( "AF14CB6C-F915-4449-9CB7-7C44B624B051", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Enable Launch Workflow", "WorkflowEnable", "Enable Launch Workflow", @"Enable Launch Workflow Action", 6, @"True", "7FEA3B48-CEA9-4902-B2BC-579D64221355" );

            // Attribute for BlockType: Care Dashboard:Minimum Care Touches
            RockMigrationHelper.AddOrUpdateBlockTypeAttribute( "AF14CB6C-F915-4449-9CB7-7C44B624B051", "A75DFC58-7A1B-4799-BF31-451B2BBE38FF", "Minimum Care Touches", "MinimumCareTouches", "Minimum Care Touches", @"Minimum care touches in 'Minimum Care Touch Hours' before the need gets 'flagged'.", 3, @"2", "8291F010-FFFE-4806-8E19-E701FAC62E10" );

            // Attribute for BlockType: Care Dashboard:Minimum Care Touch Hours
            RockMigrationHelper.AddOrUpdateBlockTypeAttribute( "AF14CB6C-F915-4449-9CB7-7C44B624B051", "A75DFC58-7A1B-4799-BF31-451B2BBE38FF", "Minimum Care Touch Givers", "MinimumCareTouchHours", "Minimum Care Touch Hours", @"Minimum care touches in this time period before the need gets 'flagged'.", 3, @"24", "8945BE62-D065-4A19-89A8-B06CE51FFBFF" );

            // Attribute for BlockType: Care Dashboard:Detail Page
            RockMigrationHelper.AddOrUpdateBlockTypeAttribute( "AF14CB6C-F915-4449-9CB7-7C44B624B051", "BD53F9C9-EBA9-4D3F-82EA-DE5DD34A8108", "Detail Page", "DetailPage", "Detail Page", @"Page used to modify and create care needs.", 1, @"", "83441829-9BAA-4C9A-921B-E3DCED54BB20" );

            // Attribute for BlockType: Care Dashboard:Categories Template
            RockMigrationHelper.AddOrUpdateBlockTypeAttribute( "AF14CB6C-F915-4449-9CB7-7C44B624B051", "1D0D3794-C210-48A8-8C68-3FBEC08A6BA5", "Categories Template", "CategoriesTemplate", "Categories Template", @"Lava Template that can be used to customize what is displayed in the last status section. Includes common merge fields plus Care Need Categories.", 5, @"
<div class="""">
{% for category in Categories %}
    <span class=""badge p-2 mb-2"" style=""background-color: {{ category | Attribute:'Color' }}"">{{ category.Value }}</span>
{% endfor %}
<br><span class=""badge p-2 mb-2 text-color"" style=""background-color: oldlace"">Assigned to You</span>
</div>", "057D28FD-8D16-4A35-BF42-389264A9B3B4" );

            // Attribute for BlockType: Care Dashboard:Outstanding Care Needs Statuses
            RockMigrationHelper.AddOrUpdateBlockTypeAttribute( "AF14CB6C-F915-4449-9CB7-7C44B624B051", "59D5A94C-94A0-4630-B80A-BB25697D74C7", "Outstanding Care Needs Statuses", "OutstandingCareNeedsStatuses", "Outstanding Care Needs Statuses", @"Select the status values that count towards the 'Outstanding Care Needs' total.", 4, @"811ECA2D-2B74-469A-9CFB-AB47B9643A02", "DCE15052-4333-4571-85BF-8803689C56D3" );

            // Attribute for BlockType: Care Dashboard:Configuration Page
            RockMigrationHelper.AddOrUpdateBlockTypeAttribute( "AF14CB6C-F915-4449-9CB7-7C44B624B051", "BD53F9C9-EBA9-4D3F-82EA-DE5DD34A8108", "Configuration Page", "ConfigurationPage", "Configuration Page", @"Page used to configure care workers and note templates.", 2, @"", "C886210E-BD1B-4187-9C04-B035447B004B" );

            // Attribute for BlockType: Care Dashboard:Display Type
            RockMigrationHelper.AddOrUpdateBlockTypeAttribute( "AF14CB6C-F915-4449-9CB7-7C44B624B051", "7525C4CB-EE6B-41D4-9B64-A08048D5A5C0", "Display Type", "DisplayType", "Display Type", @"The format to use for displaying notes.", 11, @"Full", "16067243-7FBC-4D35-B6A9-CBAABB155905" );

            // Attribute for BlockType: Care Dashboard:Use Person Icon
            RockMigrationHelper.AddOrUpdateBlockTypeAttribute( "AF14CB6C-F915-4449-9CB7-7C44B624B051", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Use Person Icon", "UsePersonIcon", "Use Person Icon", @"", 12, @"False", "7AFE5354-773D-4E47-AFF8-E155F1880695" );

            // Attribute for BlockType: Care Dashboard:Show Alert Checkbox
            RockMigrationHelper.AddOrUpdateBlockTypeAttribute( "AF14CB6C-F915-4449-9CB7-7C44B624B051", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Show Alert Checkbox", "ShowAlertCheckbox", "Show Alert Checkbox", @"", 13, @"True", "997A80B0-6F5A-4F3A-8094-6CA1AE750433" );

            // Attribute for BlockType: Care Dashboard:Show Private Checkbox
            RockMigrationHelper.AddOrUpdateBlockTypeAttribute( "AF14CB6C-F915-4449-9CB7-7C44B624B051", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Show Private Checkbox", "ShowPrivateCheckbox", "Show Private Checkbox", @"", 14, @"True", "AA734D17-955C-49A0-819E-581280FAD1C6" );

            // Attribute for BlockType: Care Dashboard:Show Security Button
            RockMigrationHelper.AddOrUpdateBlockTypeAttribute( "AF14CB6C-F915-4449-9CB7-7C44B624B051", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Show Security Button", "ShowSecurityButton", "Show Security Button", @"", 15, @"True", "9E5E061F-18B3-4D7F-BB35-D6C5E481A424" );

            // Attribute for BlockType: Care Dashboard:Allow Backdated Notes
            RockMigrationHelper.AddOrUpdateBlockTypeAttribute( "AF14CB6C-F915-4449-9CB7-7C44B624B051", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Allow Backdated Notes", "AllowBackdatedNotes", "Allow Backdated Notes", @"", 16, @"False", "F938CB7D-8EAD-4627-A042-F299FEB6642C" );

            // Attribute for BlockType: Care Dashboard:Note View Lava Template
            RockMigrationHelper.AddOrUpdateBlockTypeAttribute( "AF14CB6C-F915-4449-9CB7-7C44B624B051", "1D0D3794-C210-48A8-8C68-3FBEC08A6BA5", "Note View Lava Template", "NoteViewLavaTemplate", "Note View Lava Template", @"The Lava Template to use when rendering the view of the notes.", 18, @"{% include '~~/Assets/Lava/NoteViewList.lava' %}", "351EEAD6-6ACC-4B4D-9EE3-0B35309A2C97" );

            // Attribute for BlockType: Care Dashboard:Close Dialog on Save
            RockMigrationHelper.AddOrUpdateBlockTypeAttribute( "AF14CB6C-F915-4449-9CB7-7C44B624B051", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Close Dialog on Save", "CloseDialogOnSave", "Close Dialog on Save", @"", 17, @"True", "1A39DB8D-199D-4A3E-941C-C459F75D303E" );

            // Add Block Attribute Value
            //   Block: Care Entry
            //   BlockType: Care Entry
            //   Block Location: Page=Care Entry, Site=Rock RMS
            //   Attribute: Allow New Person Entry
            //   Attribute Value: False
            RockMigrationHelper.AddBlockAttributeValue( "F953C5EF-6504-45F9-81A8-063518B7AB61", "E6D1F331-1FCA-4DA7-8C55-A7A40C94B906", @"False" );

            // Add Block Attribute Value
            //   Block: Care Entry
            //   BlockType: Care Entry
            //   Block Location: Page=Care Entry, Site=Rock RMS
            //   Attribute: Group Type and Role
            //   Attribute Value: 6d798efa-0110-41d5-bce4-30acefe4317e
            RockMigrationHelper.AddBlockAttributeValue( "F953C5EF-6504-45F9-81A8-063518B7AB61", "664F6632-A438-4249-ADB0-D94B891BF089", @"6d798efa-0110-41d5-bce4-30acefe4317e" );

            // Add Block Attribute Value
            //   Block: Care Entry
            //   BlockType: Care Entry
            //   Block Location: Page=Care Entry, Site=Rock RMS
            //   Attribute: Auto Assign Worker with Geofence
            //   Attribute Value: True
            RockMigrationHelper.AddBlockAttributeValue( "F953C5EF-6504-45F9-81A8-063518B7AB61", "C5DE9EAE-BB28-4D81-AAB0-B7F338D030A3", @"True" );

            // Add Block Attribute Value
            //   Block: Care Entry
            //   BlockType: Care Entry
            //   Block Location: Page=Care Entry, Site=Rock RMS
            //   Attribute: Auto Assign Worker (load balanced)
            //   Attribute Value: True
            RockMigrationHelper.AddBlockAttributeValue( "F953C5EF-6504-45F9-81A8-063518B7AB61", "F7135F9C-8D06-4E6D-A493-2B1B29CE9AB6", @"True" );

            // Add Block Attribute Value
            //   Block: Care Dashboard
            //   BlockType: Care Dashboard
            //   Block Location: Page=Steps to Care, Site=Rock RMS
            //   Attribute: Minimum Care Touches
            //   Attribute Value: 2
            RockMigrationHelper.AddBlockAttributeValue( "EADBE3F0-F64B-4583-B49D-F0031BBC929F", "8291F010-FFFE-4806-8E19-E701FAC62E10", @"2" );

            // Add Block Attribute Value
            //   Block: Care Dashboard
            //   BlockType: Care Dashboard
            //   Block Location: Page=Steps to Care, Site=Rock RMS
            //   Attribute: Minimum Care Touch Hours
            //   Attribute Value: 24
            RockMigrationHelper.AddBlockAttributeValue( "EADBE3F0-F64B-4583-B49D-F0031BBC929F", "8945BE62-D065-4A19-89A8-B06CE51FFBFF", @"24" );

            // Add Block Attribute Value
            //   Block: Care Dashboard
            //   BlockType: Care Dashboard
            //   Block Location: Page=Steps to Care, Site=Rock RMS
            //   Attribute: Prayer Detail Page
            //   Attribute Value: 36e22c5d-fc31-4754-8583-b63079217528
            RockMigrationHelper.AddBlockAttributeValue( "EADBE3F0-F64B-4583-B49D-F0031BBC929F", "611CB530-08A9-4D29-8067-7BCB4FE4F300", @"36e22c5d-fc31-4754-8583-b63079217528" );

            // Add Block Attribute Value
            //   Block: Care Dashboard
            //   BlockType: Care Dashboard
            //   Block Location: Page=Steps to Care, Site=Rock RMS
            //   Attribute: Benevolence Detail Page
            //   Attribute Value: 6dc7baed-ca01-4703-b679-ec81143cdedd
            RockMigrationHelper.AddBlockAttributeValue( "EADBE3F0-F64B-4583-B49D-F0031BBC929F", "74BA310B-20FB-4856-819E-5606291B7ADA", @"6dc7baed-ca01-4703-b679-ec81143cdedd" );

            // Add Block Attribute Value
            //   Block: Care Dashboard
            //   BlockType: Care Dashboard
            //   Block Location: Page=Steps to Care, Site=Rock RMS
            //   Attribute: Enable Convert to Connection Request
            //   Attribute Value: True
            RockMigrationHelper.AddBlockAttributeValue( "EADBE3F0-F64B-4583-B49D-F0031BBC929F", "9D1C58A6-FBD8-454B-8383-1060A7629D3E", @"True" );

            // Add Block Attribute Value
            //   Block: Care Dashboard
            //   BlockType: Care Dashboard
            //   Block Location: Page=Steps to Care, Site=Rock RMS
            //   Attribute: Detail Page
            //   Attribute Value: 27953b65-21e2-4ca9-8461-3afad46d9bc8
            RockMigrationHelper.AddBlockAttributeValue( "EADBE3F0-F64B-4583-B49D-F0031BBC929F", "83441829-9BAA-4C9A-921B-E3DCED54BB20", @"27953b65-21e2-4ca9-8461-3afad46d9bc8" );

            // Add Block Attribute Value
            //   Block: Care Dashboard
            //   BlockType: Care Dashboard
            //   Block Location: Page=Steps to Care, Site=Rock RMS
            //   Attribute: Categories Template
            //   Attribute Value: <SomeLava>
            RockMigrationHelper.AddBlockAttributeValue( "EADBE3F0-F64B-4583-B49D-F0031BBC929F", "057D28FD-8D16-4A35-BF42-389264A9B3B4", @"<div class="""">
{% for category in Categories %}
    <span class=""badge p-2 mb-2"" style=""background-color: {{ category | Attribute:'Color' }}"">{{ category.Value }}</span>
{% endfor %}
<br>
<span class=""badge p-2 mb-2 text-color"" style=""background-color: oldlace"">Assigned to You</span>
</div>" );

            // Add Block Attribute Value
            //   Block: Care Dashboard
            //   BlockType: Care Dashboard
            //   Block Location: Page=Steps to Care, Site=Rock RMS
            //   Attribute: Outstanding Care Needs Statuses
            //   Attribute Value: 811eca2d-2b74-469a-9cfb-ab47b9643a02,1b48e766-3b9b-4a32-ab1a-80dc0c2dcc63
            RockMigrationHelper.AddBlockAttributeValue( "EADBE3F0-F64B-4583-B49D-F0031BBC929F", "DCE15052-4333-4571-85BF-8803689C56D3", @"811eca2d-2b74-469a-9cfb-ab47b9643a02,1b48e766-3b9b-4a32-ab1a-80dc0c2dcc63" );

            // Add Block Attribute Value
            //   Block: Care Dashboard
            //   BlockType: Care Dashboard
            //   Block Location: Page=Steps to Care, Site=Rock RMS
            //   Attribute: Configuration Page
            //   Attribute Value: 39f72e9d-22b7-4f1e-8633-6c3745ac6f34
            RockMigrationHelper.AddBlockAttributeValue( "EADBE3F0-F64B-4583-B49D-F0031BBC929F", "C886210E-BD1B-4187-9C04-B035447B004B", @"39f72e9d-22b7-4f1e-8633-6c3745ac6f34" );

            // Add Block Attribute Value
            //   Block: Care Dashboard
            //   BlockType: Care Dashboard
            //   Block Location: Page=Steps to Care, Site=Rock RMS
            //   Attribute: Display Type
            //   Attribute Value: Full
            RockMigrationHelper.AddBlockAttributeValue( "EADBE3F0-F64B-4583-B49D-F0031BBC929F", "16067243-7FBC-4D35-B6A9-CBAABB155905", @"Full" );

            // Add Block Attribute Value
            //   Block: Care Dashboard
            //   BlockType: Care Dashboard
            //   Block Location: Page=Steps to Care, Site=Rock RMS
            //   Attribute: Use Person Icon
            //   Attribute Value: False
            RockMigrationHelper.AddBlockAttributeValue( "EADBE3F0-F64B-4583-B49D-F0031BBC929F", "7AFE5354-773D-4E47-AFF8-E155F1880695", @"False" );

            // Add Block Attribute Value
            //   Block: Care Dashboard
            //   BlockType: Care Dashboard
            //   Block Location: Page=Steps to Care, Site=Rock RMS
            //   Attribute: Show Alert Checkbox
            //   Attribute Value: True
            RockMigrationHelper.AddBlockAttributeValue( "EADBE3F0-F64B-4583-B49D-F0031BBC929F", "997A80B0-6F5A-4F3A-8094-6CA1AE750433", @"True" );

            // Add Block Attribute Value
            //   Block: Care Dashboard
            //   BlockType: Care Dashboard
            //   Block Location: Page=Steps to Care, Site=Rock RMS
            //   Attribute: Show Private Checkbox
            //   Attribute Value: True
            RockMigrationHelper.AddBlockAttributeValue( "EADBE3F0-F64B-4583-B49D-F0031BBC929F", "AA734D17-955C-49A0-819E-581280FAD1C6", @"True" );

            // Add Block Attribute Value
            //   Block: Care Dashboard
            //   BlockType: Care Dashboard
            //   Block Location: Page=Steps to Care, Site=Rock RMS
            //   Attribute: Show Security Button
            //   Attribute Value: True
            RockMigrationHelper.AddBlockAttributeValue( "EADBE3F0-F64B-4583-B49D-F0031BBC929F", "9E5E061F-18B3-4D7F-BB35-D6C5E481A424", @"True" );

            // Add Block Attribute Value
            //   Block: Care Dashboard
            //   BlockType: Care Dashboard
            //   Block Location: Page=Steps to Care, Site=Rock RMS
            //   Attribute: Allow Backdated Notes
            //   Attribute Value: False
            RockMigrationHelper.AddBlockAttributeValue( "EADBE3F0-F64B-4583-B49D-F0031BBC929F", "F938CB7D-8EAD-4627-A042-F299FEB6642C", @"False" );

            // Add Block Attribute Value
            //   Block: Care Dashboard
            //   BlockType: Care Dashboard
            //   Block Location: Page=Steps to Care, Site=Rock RMS
            //   Attribute: Note View Lava Template
            //   Attribute Value: {% include '~~/Assets/Lava/NoteViewList.lava' %}
            RockMigrationHelper.AddBlockAttributeValue( "EADBE3F0-F64B-4583-B49D-F0031BBC929F", "351EEAD6-6ACC-4B4D-9EE3-0B35309A2C97", @"{% include '~~/Assets/Lava/NoteViewList.lava' %}" );

            // Add Block Attribute Value
            //   Block: Care Dashboard
            //   BlockType: Care Dashboard
            //   Block Location: Page=Steps to Care, Site=Rock RMS
            //   Attribute: Close Dialog on Save
            //   Attribute Value: True
            RockMigrationHelper.AddBlockAttributeValue( "EADBE3F0-F64B-4583-B49D-F0031BBC929F", "1A39DB8D-199D-4A3E-941C-C459F75D303E", @"True" );

            // Add Block Attribute Value
            //   Block: Defined Value List - Categories
            //   BlockType: Defined Value List
            //   Block Location: Page=Care Configuration, Site=Rock RMS
            //   Attribute: core.EnableDefaultWorkflowLauncher
            //   Attribute Value: True
            RockMigrationHelper.AddBlockAttributeValue( "F588AEE4-3BFE-4025-AFF0-5C1569434924", "80765648-83B0-4B75-A296-851384C41CAB", @"True" );

            // Add Block Attribute Value
            //   Block: Defined Value List - Categories
            //   BlockType: Defined Value List
            //   Block Location: Page=Care Configuration, Site=Rock RMS
            //   Attribute: core.CustomGridEnableStickyHeaders
            //   Attribute Value: False
            RockMigrationHelper.AddBlockAttributeValue( "F588AEE4-3BFE-4025-AFF0-5C1569434924", "2CD75CE0-D3C8-470D-8DE1-A2964AB98887", @"False" );

            // Add Block Attribute Value
            //   Block: Defined Value List - Categories
            //   BlockType: Defined Value List
            //   Block Location: Page=Care Configuration, Site=Rock RMS
            //   Attribute: Defined Type
            //   Attribute Value: 4915FF6B-4E8E-40FF-B853-EFF6B643611B
            RockMigrationHelper.AddBlockAttributeValue( "F588AEE4-3BFE-4025-AFF0-5C1569434924", "9280D61F-C4F3-4A3E-A9BB-BCD67FF78637", @"4915FF6B-4E8E-40FF-B853-EFF6B643611B" );

            // Add Block Attribute Value
            //   Block: Defined Value List - Status
            //   BlockType: Defined Value List
            //   Block Location: Page=Care Configuration, Site=Rock RMS
            //   Attribute: Defined Type
            //   Attribute Value: F965CB2C-23D0-42D6-8BAF-10F552249B7A
            RockMigrationHelper.AddBlockAttributeValue( "362581DC-6224-40FA-B351-FC2572022166", "9280D61F-C4F3-4A3E-A9BB-BCD67FF78637", @"F965CB2C-23D0-42D6-8BAF-10F552249B7A" );

            // Add Block Attribute Value
            //   Block: Defined Value List - Status
            //   BlockType: Defined Value List
            //   Block Location: Page=Care Configuration, Site=Rock RMS
            //   Attribute: core.CustomGridEnableStickyHeaders
            //   Attribute Value: False
            RockMigrationHelper.AddBlockAttributeValue( "362581DC-6224-40FA-B351-FC2572022166", "2CD75CE0-D3C8-470D-8DE1-A2964AB98887", @"False" );

            // Add Block Attribute Value
            //   Block: Defined Value List - Status
            //   BlockType: Defined Value List
            //   Block Location: Page=Care Configuration, Site=Rock RMS
            //   Attribute: core.EnableDefaultWorkflowLauncher
            //   Attribute Value: True
            RockMigrationHelper.AddBlockAttributeValue( "362581DC-6224-40FA-B351-FC2572022166", "80765648-83B0-4B75-A296-851384C41CAB", @"True" );

            // Add Block Attribute Value
            //   Block: Care Notes
            //   BlockType: Notes
            //   Block Location: Page=Care Entry, Site=Rock RMS
            //   Attribute: Note View Lava Template
            //   Attribute Value: {% include '~~/Assets/Lava/NoteViewList.lava' %}
            RockMigrationHelper.AddBlockAttributeValue( "A9F1F1C1-031B-4D87-BF52-0E5BC5423AC6", "328DDE3F-6FFF-4CA4-B6D0-C1BD4D643307", @"{% include '~~/Assets/Lava/NoteViewList.lava' %}" );

            // Add Block Attribute Value
            //   Block: Care Notes
            //   BlockType: Notes
            //   Block Location: Page=Care Entry, Site=Rock RMS
            //   Attribute: Expand Replies
            //   Attribute Value: False
            RockMigrationHelper.AddBlockAttributeValue( "A9F1F1C1-031B-4D87-BF52-0E5BC5423AC6", "84E53A88-32D2-432C-8BB5-600BDBA10949", @"False" );

            // Add Block Attribute Value
            //   Block: Care Notes
            //   BlockType: Notes
            //   Block Location: Page=Care Entry, Site=Rock RMS
            //   Attribute: Heading
            //   Attribute Value: Care Notes
            RockMigrationHelper.AddBlockAttributeValue( "A9F1F1C1-031B-4D87-BF52-0E5BC5423AC6", "3CB0A7DF-996B-4D6C-B3B6-9BBCC40BDC69", @"Care Notes" );

            // Add Block Attribute Value
            //   Block: Care Notes
            //   BlockType: Notes
            //   Block Location: Page=Care Entry, Site=Rock RMS
            //   Attribute: Heading Icon CSS Class
            //   Attribute Value: fas fa-notes-medical
            RockMigrationHelper.AddBlockAttributeValue( "A9F1F1C1-031B-4D87-BF52-0E5BC5423AC6", "B69937BE-000A-4B94-852F-16DE92344392", @"fas fa-notes-medical" );

            // Add Block Attribute Value
            //   Block: Care Notes
            //   BlockType: Notes
            //   Block Location: Page=Care Entry, Site=Rock RMS
            //   Attribute: Note Term
            //   Attribute Value: Note
            RockMigrationHelper.AddBlockAttributeValue( "A9F1F1C1-031B-4D87-BF52-0E5BC5423AC6", "FD0727DC-92F4-4765-82CB-3A08B7D864F8", @"Note" );

            // Add Block Attribute Value
            //   Block: Care Notes
            //   BlockType: Notes
            //   Block Location: Page=Care Entry, Site=Rock RMS
            //   Attribute: Display Type
            //   Attribute Value: Full
            RockMigrationHelper.AddBlockAttributeValue( "A9F1F1C1-031B-4D87-BF52-0E5BC5423AC6", "5232BFAE-4DC8-4270-B38F-D29E1B00AB5E", @"Full" );

            // Add Block Attribute Value
            //   Block: Care Notes
            //   BlockType: Notes
            //   Block Location: Page=Care Entry, Site=Rock RMS
            //   Attribute: Use Person Icon
            //   Attribute Value: True
            RockMigrationHelper.AddBlockAttributeValue( "A9F1F1C1-031B-4D87-BF52-0E5BC5423AC6", "C05757C0-E83E-4170-8CBF-C4E1ABEC36E1", @"True" );

            // Add Block Attribute Value
            //   Block: Care Notes
            //   BlockType: Notes
            //   Block Location: Page=Care Entry, Site=Rock RMS
            //   Attribute: Allow Anonymous
            //   Attribute Value: False
            RockMigrationHelper.AddBlockAttributeValue( "A9F1F1C1-031B-4D87-BF52-0E5BC5423AC6", "EB9CBD02-2B0F-4BA3-9112-BC73D54159E7", @"False" );

            // Add Block Attribute Value
            //   Block: Care Notes
            //   BlockType: Notes
            //   Block Location: Page=Care Entry, Site=Rock RMS
            //   Attribute: Add Always Visible
            //   Attribute Value: False
            RockMigrationHelper.AddBlockAttributeValue( "A9F1F1C1-031B-4D87-BF52-0E5BC5423AC6", "8E0BDD15-6B92-4BB0-9138-E9382B60F3A9", @"False" );

            // Add Block Attribute Value
            //   Block: Care Notes
            //   BlockType: Notes
            //   Block Location: Page=Care Entry, Site=Rock RMS
            //   Attribute: Display Order
            //   Attribute Value: Descending
            RockMigrationHelper.AddBlockAttributeValue( "A9F1F1C1-031B-4D87-BF52-0E5BC5423AC6", "C9FC2C09-1BF5-4711-8F97-0B96633C46B1", @"Descending" );

            // Add Block Attribute Value
            //   Block: Care Notes
            //   BlockType: Notes
            //   Block Location: Page=Care Entry, Site=Rock RMS
            //   Attribute: Entity Type
            //   Attribute Value: 87ac878d-6740-43eb-9389-b8440ac595c3
            RockMigrationHelper.AddBlockAttributeValue( "A9F1F1C1-031B-4D87-BF52-0E5BC5423AC6", "F1BCF615-FBCA-4BC2-A912-C35C0DC04174", @"87ac878d-6740-43eb-9389-b8440ac595c3" );

            // Add Block Attribute Value
            //   Block: Care Notes
            //   BlockType: Notes
            //   Block Location: Page=Care Entry, Site=Rock RMS
            //   Attribute: Show Private Checkbox
            //   Attribute Value: True
            RockMigrationHelper.AddBlockAttributeValue( "A9F1F1C1-031B-4D87-BF52-0E5BC5423AC6", "D68EE1F5-D29F-404B-945D-AD0BE76594C3", @"True" );

            // Add Block Attribute Value
            //   Block: Care Notes
            //   BlockType: Notes
            //   Block Location: Page=Care Entry, Site=Rock RMS
            //   Attribute: Show Security Button
            //   Attribute Value: True
            RockMigrationHelper.AddBlockAttributeValue( "A9F1F1C1-031B-4D87-BF52-0E5BC5423AC6", "00B6EBFF-786D-453E-8746-119D0B45CB3E", @"True" );

            // Add Block Attribute Value
            //   Block: Care Notes
            //   BlockType: Notes
            //   Block Location: Page=Care Entry, Site=Rock RMS
            //   Attribute: Show Alert Checkbox
            //   Attribute Value: True
            RockMigrationHelper.AddBlockAttributeValue( "A9F1F1C1-031B-4D87-BF52-0E5BC5423AC6", "20243A98-4802-48E2-AF61-83956056AC65", @"True" );

            // Add Block Attribute Value
            //   Block: Care Notes
            //   BlockType: Notes
            //   Block Location: Page=Care Entry, Site=Rock RMS
            //   Attribute: Allow Backdated Notes
            //   Attribute Value: False
            RockMigrationHelper.AddBlockAttributeValue( "A9F1F1C1-031B-4D87-BF52-0E5BC5423AC6", "6184511D-CC68-4FF2-90CB-3AD0AFD59D61", @"False" );

            // Add Block Attribute Value
            //   Block: Care Notes
            //   BlockType: Notes
            //   Block Location: Page=Care Entry, Site=Rock RMS
            //   Attribute: Display Note Type Heading
            //   Attribute Value: False
            RockMigrationHelper.AddBlockAttributeValue( "A9F1F1C1-031B-4D87-BF52-0E5BC5423AC6", "C5FD0719-1E03-4C17-BE31-E02A3637C39A", @"False" );

            // Add/Update PageContext for Page:Care Entry, Entity: rocks.kfs.StepsToCare.Model.CareNeed, Parameter: CareNeedId
            RockMigrationHelper.UpdatePageContext( "27953B65-21E2-4CA9-8461-3AFAD46D9BC8", "rocks.kfs.StepsToCare.Model.CareNeed", "CareNeedId", "6AE31281-2175-4BB9-B5A2-DF29092E85EC" );

        }

        public override void Down()
        {

            // Enable Launch Workflow Attribute for BlockType: Care Dashboard
            RockMigrationHelper.DeleteAttribute( "7FEA3B48-CEA9-4902-B2BC-579D64221355" );

            // Enable Convert to Connection Request Attribute for BlockType: Care Dashboard
            RockMigrationHelper.DeleteAttribute( "9D1C58A6-FBD8-454B-8383-1060A7629D3E" );

            // Filter Connection Types Attribute for BlockType: Care Dashboard
            RockMigrationHelper.DeleteAttribute( "387CA195-C41F-4EC6-AD5D-69FABAC68AF9" );

            // Benevolence Detail Page Attribute for BlockType: Care Dashboard
            RockMigrationHelper.DeleteAttribute( "74BA310B-20FB-4856-819E-5606291B7ADA" );

            // Prayer Detail Page Attribute for BlockType: Care Dashboard
            RockMigrationHelper.DeleteAttribute( "611CB530-08A9-4D29-8067-7BCB4FE4F300" );

            // Newly Assigned Need Notification Attribute for BlockType: Care Entry
            RockMigrationHelper.DeleteAttribute( "E3604688-ACC2-4360-976B-44EB5170F685" );

            // Minimum Care Touches Attribute for BlockType: Care Dashboard
            RockMigrationHelper.DeleteAttribute( "8291F010-FFFE-4806-8E19-E701FAC62E10" );

            // Minimum Care Touch Hours Attribute for BlockType: Care Dashboard
            RockMigrationHelper.DeleteAttribute( "8945BE62-D065-4A19-89A8-B06CE51FFBFF" );

            // Auto Assign Worker (load balanced) Attribute for BlockType: Care Entry
            RockMigrationHelper.DeleteAttribute( "F7135F9C-8D06-4E6D-A493-2B1B29CE9AB6" );

            // Auto Assign Worker with Geofence Attribute for BlockType: Care Entry
            RockMigrationHelper.DeleteAttribute( "C5DE9EAE-BB28-4D81-AAB0-B7F338D030A3" );

            // Group Type and Role Attribute for BlockType: Care Entry
            RockMigrationHelper.DeleteAttribute( "664F6632-A438-4249-ADB0-D94B891BF089" );

            // Close Dialog on Save Attribute for BlockType: Care Dashboard
            RockMigrationHelper.DeleteAttribute( "1A39DB8D-199D-4A3E-941C-C459F75D303E" );

            // Note View Lava Template Attribute for BlockType: Care Dashboard
            RockMigrationHelper.DeleteAttribute( "351EEAD6-6ACC-4B4D-9EE3-0B35309A2C97" );

            // Allow Backdated Notes Attribute for BlockType: Care Dashboard
            RockMigrationHelper.DeleteAttribute( "F938CB7D-8EAD-4627-A042-F299FEB6642C" );

            // Show Security Button Attribute for BlockType: Care Dashboard
            RockMigrationHelper.DeleteAttribute( "9E5E061F-18B3-4D7F-BB35-D6C5E481A424" );

            // Show Private Checkbox Attribute for BlockType: Care Dashboard
            RockMigrationHelper.DeleteAttribute( "AA734D17-955C-49A0-819E-581280FAD1C6" );

            // Show Alert Checkbox Attribute for BlockType: Care Dashboard
            RockMigrationHelper.DeleteAttribute( "997A80B0-6F5A-4F3A-8094-6CA1AE750433" );

            // Use Person Icon Attribute for BlockType: Care Dashboard
            RockMigrationHelper.DeleteAttribute( "7AFE5354-773D-4E47-AFF8-E155F1880695" );

            // Display Type Attribute for BlockType: Care Dashboard
            RockMigrationHelper.DeleteAttribute( "16067243-7FBC-4D35-B6A9-CBAABB155905" );

            // Configuration Page Attribute for BlockType: Care Dashboard
            RockMigrationHelper.DeleteAttribute( "C886210E-BD1B-4187-9C04-B035447B004B" );

            // Outstanding Care Needs Statuses Attribute for BlockType: Care Dashboard
            RockMigrationHelper.DeleteAttribute( "DCE15052-4333-4571-85BF-8803689C56D3" );

            // Categories Template Attribute for BlockType: Care Dashboard
            RockMigrationHelper.DeleteAttribute( "057D28FD-8D16-4A35-BF42-389264A9B3B4" );

            // Detail Page Attribute for BlockType: Care Dashboard
            RockMigrationHelper.DeleteAttribute( "83441829-9BAA-4C9A-921B-E3DCED54BB20" );

            // Allow New Person Entry Attribute for BlockType: Care Entry
            RockMigrationHelper.DeleteAttribute( "E6D1F331-1FCA-4DA7-8C55-A7A40C94B906" );

            // Remove Block: Care Notes, from Page: Care Entry, Site: Rock RMS
            RockMigrationHelper.DeleteBlock( "A9F1F1C1-031B-4D87-BF52-0E5BC5423AC6" );

            // Remove Block: Defined Value List - Status, from Page: Care Configuration, Site: Rock RMS
            RockMigrationHelper.DeleteBlock( "362581DC-6224-40FA-B351-FC2572022166" );

            // Remove Block: Defined Value List - Categories, from Page: Care Configuration, Site: Rock RMS
            RockMigrationHelper.DeleteBlock( "F588AEE4-3BFE-4025-AFF0-5C1569434924" );

            // Remove Block: Care Note Templates, from Page: Care Configuration, Site: Rock RMS
            RockMigrationHelper.DeleteBlock( "E4108097-F10B-4404-BD3C-D36D8DEB1A8B" );

            // Remove Block: Care Workers, from Page: Care Configuration, Site: Rock RMS
            RockMigrationHelper.DeleteBlock( "8B0BBF03-2BAE-4B7D-9A4B-1CAE23F8E02E" );

            // Remove Block: Care Dashboard, from Page: Steps to Care, Site: Rock RMS
            RockMigrationHelper.DeleteBlock( "EADBE3F0-F64B-4583-B49D-F0031BBC929F" );

            // Remove Block: Care Entry, from Page: Care Entry, Site: Rock RMS
            RockMigrationHelper.DeleteBlock( "F953C5EF-6504-45F9-81A8-063518B7AB61" );

            // Delete BlockType Care Workers
            RockMigrationHelper.DeleteBlockType( "FC3E03F3-80A0-42BF-AAFB-DD2095B7BE86" ); // Care Workers

            // Delete BlockType Care Note Templates
            RockMigrationHelper.DeleteBlockType( "561E0D77-12F9-4863-B5E3-4C5F36FB2DB1" ); // Care Note Templates

            // Delete BlockType Care Dashboard
            RockMigrationHelper.DeleteBlockType( "AF14CB6C-F915-4449-9CB7-7C44B624B051" ); // Care Dashboard

            // Delete BlockType Care Entry
            RockMigrationHelper.DeleteBlockType( "4F0F9ED7-9F74-4152-B27F-D9B2A458AFBE" ); // Care Entry

            // Delete Page Steps to Care from Site:Rock RMS
            RockMigrationHelper.DeletePage( "1F93E9AA-ECCA-42A2-8C91-73D991DBCD9F" ); //  Page: Steps to Care, Layout: Full Width, Site: Rock RMS
        }
    }
}