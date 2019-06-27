using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rock.Plugin;

namespace com.kfs.Workflow.Action.Groups.Migrations
{
    [MigrationNumber( 1, "1.7.0" )]
    public class AddCreateGroupWorkflowType : Migration
    {
        /// <summary>
        /// The commands to run to migrate plugin to the specific version
        /// </summary>
        public override void Up()
        {

            #region EntityTypes

            RockMigrationHelper.UpdateEntityType( "Rock.Model.Workflow", "3540E9A7-FE30-43A9-8B0A-A372B63DFC93", true, true );
            RockMigrationHelper.UpdateEntityType( "Rock.Model.WorkflowActivity", "2CB52ED0-CB06-4D62-9E2C-73B60AFA4C9F", true, true );
            RockMigrationHelper.UpdateEntityType( "Rock.Model.WorkflowActionType", "23E3273A-B137-48A3-9AFF-C8DC832DDCA6", true, true );
            RockMigrationHelper.UpdateEntityType( "Rock.Workflow.Action.ActivateActivity", "38907A90-1634-4A93-8017-619326A4A582", false, true );
            RockMigrationHelper.UpdateEntityType( "Rock.Workflow.Action.AddPersonToGroupWFAttribute", "BD53F375-78A2-4A54-B1D1-2D805F3FCD44", false, true );
            RockMigrationHelper.UpdateEntityType( "Rock.Workflow.Action.AssignActivityFromAttributeValue", "F100A31F-E93A-4C7A-9E55-0FAF41A101C4", false, true );
            RockMigrationHelper.UpdateEntityType( "Rock.Workflow.Action.CompleteWorkflow", "EEDA4318-F014-4A46-9C76-4C052EF81AA1", false, true );
            RockMigrationHelper.UpdateEntityType( "Rock.Workflow.Action.PersistWorkflow", "F1A39347-6FE0-43D4-89FB-544195088ECF", false, true );
            RockMigrationHelper.UpdateEntityType( "Rock.Workflow.Action.SetAttributeFromPerson", "17962C23-2E94-4E06-8461-0FB8B94E2FEA", false, true );
            RockMigrationHelper.UpdateEntityType( "Rock.Workflow.Action.SetAttributeToCurrentPerson", "24B7D5E6-C30F-48F4-9D7E-AF45A342CF3A", false, true );
            RockMigrationHelper.UpdateEntityType( "Rock.Workflow.Action.SetWorkflowName", "36005473-BD5D-470B-B28D-98E6D7ED808D", false, true );
            RockMigrationHelper.UpdateEntityType( "Rock.Workflow.Action.UserEntryForm", "486DC4FA-FCBC-425F-90B0-E606DA8A9F68", false, true );
            RockMigrationHelper.UpdateEntityType( "Rock.Workflow.Action.WriteToLog", "B442940A-0C8B-4F44-8359-1E0AF3AAAB4C", false, true );
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "17962C23-2E94-4E06-8461-0FB8B94E2FEA", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Active", "Active", "Should Service be used?", 0, @"False", "CE28B79D-FBC2-4894-9198-D923D0217549" ); // Rock.Workflow.Action.SetAttributeFromPerson:Active
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "17962C23-2E94-4E06-8461-0FB8B94E2FEA", "33E6DF69-BDFA-407A-9744-C175B60643AE", "Attribute", "Attribute", "The person attribute to set the value of.", 0, @"", "7AC47975-71AC-4A2F-BF1F-115CF5578D6F" ); // Rock.Workflow.Action.SetAttributeFromPerson:Attribute
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "17962C23-2E94-4E06-8461-0FB8B94E2FEA", "A75DFC58-7A1B-4799-BF31-451B2BBE38FF", "Order", "Order", "The order that this service should be used (priority)", 0, @"", "18EF907D-607E-4891-B034-7AA379D77854" ); // Rock.Workflow.Action.SetAttributeFromPerson:Order
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "17962C23-2E94-4E06-8461-0FB8B94E2FEA", "E4EAB7B2-0B76-429B-AFE4-AD86D7428C70", "Person", "Person", "The person to set attribute value to. Leave blank to set person to nobody.", 1, @"", "5C803BD1-40FA-49B1-AE7E-68F43D3687BB" ); // Rock.Workflow.Action.SetAttributeFromPerson:Person
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "24B7D5E6-C30F-48F4-9D7E-AF45A342CF3A", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Active", "Active", "Should Service be used?", 0, @"False", "DE9CB292-4785-4EA3-976D-3826F91E9E98" ); // Rock.Workflow.Action.SetAttributeToCurrentPerson:Active
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "24B7D5E6-C30F-48F4-9D7E-AF45A342CF3A", "33E6DF69-BDFA-407A-9744-C175B60643AE", "Person Attribute", "PersonAttribute", "The attribute to set to the currently logged in person.", 0, @"", "BBED8A83-8BB2-4D35-BAFB-05F67DCAD112" ); // Rock.Workflow.Action.SetAttributeToCurrentPerson:Person Attribute
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "24B7D5E6-C30F-48F4-9D7E-AF45A342CF3A", "A75DFC58-7A1B-4799-BF31-451B2BBE38FF", "Order", "Order", "The order that this service should be used (priority)", 0, @"", "89E9BCED-91AB-47B0-AD52-D78B0B7CB9E8" ); // Rock.Workflow.Action.SetAttributeToCurrentPerson:Order
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "36005473-BD5D-470B-B28D-98E6D7ED808D", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Active", "Active", "Should Service be used?", 0, @"False", "0A800013-51F7-4902-885A-5BE215D67D3D" ); // Rock.Workflow.Action.SetWorkflowName:Active
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "36005473-BD5D-470B-B28D-98E6D7ED808D", "3B1D93D7-9414-48F9-80E5-6A3FC8F94C20", "Text Value|Attribute Value", "NameValue", "The value to use for the workflow's name. <span class='tip tip-lava'></span>", 1, @"", "93852244-A667-4749-961A-D47F88675BE4" ); // Rock.Workflow.Action.SetWorkflowName:Text Value|Attribute Value
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "36005473-BD5D-470B-B28D-98E6D7ED808D", "A75DFC58-7A1B-4799-BF31-451B2BBE38FF", "Order", "Order", "The order that this service should be used (priority)", 0, @"", "5D95C15A-CCAE-40AD-A9DD-F929DA587115" ); // Rock.Workflow.Action.SetWorkflowName:Order
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "38907A90-1634-4A93-8017-619326A4A582", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Active", "Active", "Should Service be used?", 0, @"False", "E8ABD802-372C-47BE-82B1-96F50DB5169E" ); // Rock.Workflow.Action.ActivateActivity:Active
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "38907A90-1634-4A93-8017-619326A4A582", "739FD425-5B8C-4605-B775-7E4D9D4C11DB", "Activity", "Activity", "The activity type to activate", 0, @"", "02D5A7A5-8781-46B4-B9FC-AF816829D240" ); // Rock.Workflow.Action.ActivateActivity:Activity
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "38907A90-1634-4A93-8017-619326A4A582", "A75DFC58-7A1B-4799-BF31-451B2BBE38FF", "Order", "Order", "The order that this service should be used (priority)", 0, @"", "3809A78C-B773-440C-8E3F-A8E81D0DAE08" ); // Rock.Workflow.Action.ActivateActivity:Order
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "486DC4FA-FCBC-425F-90B0-E606DA8A9F68", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Active", "Active", "Should Service be used?", 0, @"False", "234910F2-A0DB-4D7D-BAF7-83C880EF30AE" ); // Rock.Workflow.Action.UserEntryForm:Active
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "486DC4FA-FCBC-425F-90B0-E606DA8A9F68", "A75DFC58-7A1B-4799-BF31-451B2BBE38FF", "Order", "Order", "The order that this service should be used (priority)", 0, @"", "C178113D-7C86-4229-8424-C6D0CF4A7E23" ); // Rock.Workflow.Action.UserEntryForm:Order
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "B442940A-0C8B-4F44-8359-1E0AF3AAAB4C", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Active", "Active", "Should Service be used?", 0, @"False", "C969ED7A-10FB-4655-A9D2-94E197B44603" ); // Rock.Workflow.Action.WriteToLog:Active
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "B442940A-0C8B-4F44-8359-1E0AF3AAAB4C", "A75DFC58-7A1B-4799-BF31-451B2BBE38FF", "Order", "Order", "The order that this service should be used (priority)", 0, @"", "0DDD5B24-455B-4989-9C70-F31D78C7AC8F" ); // Rock.Workflow.Action.WriteToLog:Order
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "B442940A-0C8B-4F44-8359-1E0AF3AAAB4C", "C28C7BF3-A552-4D77-9408-DEDCF760CED0", "Message", "Message", "The message to write to the log. <span class='tip tip-lava'></span>", 0, @"", "1242890B-DC92-48B2-8DC3-F6A20B920FAD" ); // Rock.Workflow.Action.WriteToLog:Message
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "BD53F375-78A2-4A54-B1D1-2D805F3FCD44", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Active", "Active", "Should Service be used?", 0, @"False", "8D8F9DD3-2CE6-43C7-8995-399E9F985B84" ); // Rock.Workflow.Action.AddPersonToGroupWFAttribute:Active
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "BD53F375-78A2-4A54-B1D1-2D805F3FCD44", "33E6DF69-BDFA-407A-9744-C175B60643AE", "Group", "Group", "Workflow Attribute that contains the group to add the person to.", 0, @"", "EE74986E-1368-4A68-BF96-9B5B621FA7F9" ); // Rock.Workflow.Action.AddPersonToGroupWFAttribute:Group
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "BD53F375-78A2-4A54-B1D1-2D805F3FCD44", "33E6DF69-BDFA-407A-9744-C175B60643AE", "Person", "Person", "Workflow attribute that contains the person to add to the group.", 0, @"", "3BCC80E3-68B9-47AA-8A04-40BC83D54289" ); // Rock.Workflow.Action.AddPersonToGroupWFAttribute:Person
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "BD53F375-78A2-4A54-B1D1-2D805F3FCD44", "A75DFC58-7A1B-4799-BF31-451B2BBE38FF", "Order", "Order", "The order that this service should be used (priority)", 0, @"", "91EFE616-FF86-4A23-A16A-D5C2458782B2" ); // Rock.Workflow.Action.AddPersonToGroupWFAttribute:Order
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "EEDA4318-F014-4A46-9C76-4C052EF81AA1", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Active", "Active", "Should Service be used?", 0, @"False", "0CA0DDEF-48EF-4ABC-9822-A05E225DE26C" ); // Rock.Workflow.Action.CompleteWorkflow:Active
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "EEDA4318-F014-4A46-9C76-4C052EF81AA1", "3B1D93D7-9414-48F9-80E5-6A3FC8F94C20", "Status|Status Attribute", "Status", "The status to set the workflow to when marking the workflow complete. <span class='tip tip-lava'></span>", 0, @"Completed", "B9325645-E512-4CB9-8E9E-730A7858A146" ); // Rock.Workflow.Action.CompleteWorkflow:Status|Status Attribute
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "EEDA4318-F014-4A46-9C76-4C052EF81AA1", "A75DFC58-7A1B-4799-BF31-451B2BBE38FF", "Order", "Order", "The order that this service should be used (priority)", 0, @"", "25CAD4BE-5A00-409D-9BAB-E32518D89956" ); // Rock.Workflow.Action.CompleteWorkflow:Order
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "F100A31F-E93A-4C7A-9E55-0FAF41A101C4", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Active", "Active", "Should Service be used?", 0, @"False", "E0F7AB7E-7761-4600-A099-CB14ACDBF6EF" ); // Rock.Workflow.Action.AssignActivityFromAttributeValue:Active
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "F100A31F-E93A-4C7A-9E55-0FAF41A101C4", "33E6DF69-BDFA-407A-9744-C175B60643AE", "Attribute", "Attribute", "The person or group attribute value to assign this activity to.", 0, @"", "FBADD25F-D309-4512-8430-3CC8615DD60E" ); // Rock.Workflow.Action.AssignActivityFromAttributeValue:Attribute
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "F100A31F-E93A-4C7A-9E55-0FAF41A101C4", "A75DFC58-7A1B-4799-BF31-451B2BBE38FF", "Order", "Order", "The order that this service should be used (priority)", 0, @"", "7A6B605D-7FB1-4F48-AF35-5A0683FB1CDA" ); // Rock.Workflow.Action.AssignActivityFromAttributeValue:Order
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "F1A39347-6FE0-43D4-89FB-544195088ECF", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Active", "Active", "Should Service be used?", 0, @"False", "50B01639-4938-40D2-A791-AA0EB4F86847" ); // Rock.Workflow.Action.PersistWorkflow:Active
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "F1A39347-6FE0-43D4-89FB-544195088ECF", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Persist Immediately", "PersistImmediately", "This action will normally cause the workflow to be persisted (saved) once all the current activites/actions have completed processing. Set this flag to true, if the workflow should be persisted immediately. This is only required if a subsequent action needs a persisted workflow with a valid id.", 0, @"False", "84E159D4-BB84-40CD-9B73-FE8C9C609F80" ); // Rock.Workflow.Action.PersistWorkflow:Persist Immediately
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "F1A39347-6FE0-43D4-89FB-544195088ECF", "A75DFC58-7A1B-4799-BF31-451B2BBE38FF", "Order", "Order", "The order that this service should be used (priority)", 0, @"", "86F795B0-0CB6-4DA4-9CE4-B11D0922F361" ); // Rock.Workflow.Action.PersistWorkflow:Order

            #endregion

            #region Categories

            RockMigrationHelper.UpdateCategory( "C9F3C4A5-1526-474D-803F-D6C7A45CBBAE", "Requests", "fa fa-question-circle", "", "78E38655-D951-41DB-A0FF-D6474775CFA1", 0 ); // Requests

            #endregion

            #region Create Group

            RockMigrationHelper.UpdateWorkflowType( false, true, "Create Group", "Workflow used to create a group from a group leader request", "78E38655-D951-41DB-A0FF-D6474775CFA1", "Group", "fa fa-list-ol", 28800, false, 0, "3D736B06-45B3-4A1A-BDE5-5A46ED7DDC74", 0 ); // Create Group
            RockMigrationHelper.UpdateWorkflowTypeAttribute( "3D736B06-45B3-4A1A-BDE5-5A46ED7DDC74", "E4EAB7B2-0B76-429B-AFE4-AD86D7428C70", "Requester", "Requester", "Person making the request", 0, @"", "03217B23-04CF-4FEC-AB69-962C3D72196D", false ); // Create Group:Requester
            RockMigrationHelper.UpdateWorkflowTypeAttribute( "3D736B06-45B3-4A1A-BDE5-5A46ED7DDC74", "9C204CD0-1233-41C5-818A-C5DA439445AA", "Proposed Group Name", "ProposedGroupName", "", 1, @"", "ABF83FE3-D4AB-48A7-AD46-D34368B34401", false ); // Create Group:Proposed Group Name
            RockMigrationHelper.UpdateWorkflowTypeAttribute( "3D736B06-45B3-4A1A-BDE5-5A46ED7DDC74", "C28C7BF3-A552-4D77-9408-DEDCF760CED0", "Description", "Description1", "", 2, @"", "B8118D49-5A48-4C55-BFC9-540A607762F1", false ); // Create Group:Description
            RockMigrationHelper.UpdateWorkflowTypeAttribute( "3D736B06-45B3-4A1A-BDE5-5A46ED7DDC74", "1B71FEF4-201F-4D53-8C60-2DF21F1985ED", "Campus", "Campus", "", 3, @"", "7250259C-C763-4344-8D9C-5C93BA66192E", false ); // Create Group:Campus
            RockMigrationHelper.UpdateWorkflowTypeAttribute( "3D736B06-45B3-4A1A-BDE5-5A46ED7DDC74", "B0B9EFE3-F09F-4604-AD1B-76B298A85D83", "Location", "Location", "", 4, @"00000000-0000-0000-0000-000000000000", "AE2B5FB2-68B1-4EA2-8793-687EC396FD68", false ); // Create Group:Location
            RockMigrationHelper.UpdateWorkflowTypeAttribute( "3D736B06-45B3-4A1A-BDE5-5A46ED7DDC74", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Show Map to Location", "ShowMaptoLocation", "", 5, @"", "D6D1F80C-851E-4CC0-93BF-A548D02A0CAF", false ); // Create Group:Show Map to Location
            RockMigrationHelper.UpdateWorkflowTypeAttribute( "3D736B06-45B3-4A1A-BDE5-5A46ED7DDC74", "18E29E23-B43B-4CF7-AE41-C85672C09F50", "Group Type", "GroupType", "", 6, @"", "EAD808C7-2D9E-4E8E-8841-71A74B3D6FAB", false ); // Create Group:Group Type
            RockMigrationHelper.UpdateWorkflowTypeAttribute( "3D736B06-45B3-4A1A-BDE5-5A46ED7DDC74", "F4399CEF-827B-48B2-A735-F7806FCFE8E8", "Parent Group/Category", "ParentGroup", "Select a Parent Group/Category for this group", 7, @"", "C989B26F-4F4E-472E-BDF9-CDAFD197C66E", false ); // Create Group:Parent Group/Category
            RockMigrationHelper.UpdateWorkflowTypeAttribute( "3D736B06-45B3-4A1A-BDE5-5A46ED7DDC74", "E4EAB7B2-0B76-429B-AFE4-AD86D7428C70", "Worker", "Worker", "", 8, @"", "A6782894-B6AE-4260-B361-E15BE17BA443", true ); // Create Group:Worker
            RockMigrationHelper.UpdateWorkflowTypeAttribute( "3D736B06-45B3-4A1A-BDE5-5A46ED7DDC74", "9C204CD0-1233-41C5-818A-C5DA439445AA", "Review Result", "ReviewResult", "Used to keep track of what the result of this create was.", 9, @"", "83528E87-B330-4FD7-AC2D-A54D85AFFDB1", true ); // Create Group:Review Result
            RockMigrationHelper.UpdateWorkflowTypeAttribute( "3D736B06-45B3-4A1A-BDE5-5A46ED7DDC74", "F4399CEF-827B-48B2-A735-F7806FCFE8E8", "New Group", "NewGroup", "", 10, @"", "7ECBBF12-5AA1-4594-9B43-DD835C272C39", false ); // Create Group:New Group
            RockMigrationHelper.AddAttributeQualifier( "03217B23-04CF-4FEC-AB69-962C3D72196D", "EnableSelfSelection", @"False", "265CB6EF-ED36-4E8B-8970-209EEEFF9A05" ); // Create Group:Requester:EnableSelfSelection
            RockMigrationHelper.AddAttributeQualifier( "ABF83FE3-D4AB-48A7-AD46-D34368B34401", "ispassword", @"False", "460C3DAA-7433-419E-89A6-86C160CD1282" ); // Create Group:Proposed Group Name:ispassword
            RockMigrationHelper.AddAttributeQualifier( "B8118D49-5A48-4C55-BFC9-540A607762F1", "allowhtml", @"False", "AD32D38E-F9B2-4E0C-B45A-85AA77D27636" ); // Create Group:Description:allowhtml
            RockMigrationHelper.AddAttributeQualifier( "B8118D49-5A48-4C55-BFC9-540A607762F1", "numberofrows", @"4", "6B5365F1-BBE6-46A1-8069-F3D1A871ADE4" ); // Create Group:Description:numberofrows
            RockMigrationHelper.AddAttributeQualifier( "7250259C-C763-4344-8D9C-5C93BA66192E", "includeInactive", @"False", "6BEF9ABD-3B23-4F18-AA58-F7BF61542858" ); // Create Group:Campus:includeInactive
            RockMigrationHelper.AddAttributeQualifier( "D6D1F80C-851E-4CC0-93BF-A548D02A0CAF", "falsetext", @"No", "F4FDE367-F9B7-40B5-9BCA-A53CFF3C637F" ); // Create Group:Show Map to Location:falsetext
            RockMigrationHelper.AddAttributeQualifier( "D6D1F80C-851E-4CC0-93BF-A548D02A0CAF", "truetext", @"Yes", "95563983-DE6D-40FB-A705-C1DF4513A010" ); // Create Group:Show Map to Location:truetext
            RockMigrationHelper.AddAttributeQualifier( "EAD808C7-2D9E-4E8E-8841-71A74B3D6FAB", "groupTypePurposeValueGuid", @"", "75707171-3FD2-4564-9B56-940C05887E55" ); // Create Group:Group Type:groupTypePurposeValueGuid
            RockMigrationHelper.AddAttributeQualifier( "A6782894-B6AE-4260-B361-E15BE17BA443", "EnableSelfSelection", @"False", "B5B78013-739A-4169-A06C-9CC5F1FFA681" ); // Create Group:Worker:EnableSelfSelection
            RockMigrationHelper.AddAttributeQualifier( "83528E87-B330-4FD7-AC2D-A54D85AFFDB1", "ispassword", @"False", "91045793-AD50-4D24-B4FC-710D3383B4E4" ); // Create Group:Review Result:ispassword
            RockMigrationHelper.UpdateWorkflowActivityType( "3D736B06-45B3-4A1A-BDE5-5A46ED7DDC74", true, "Start", "", true, 0, "FBE45F3A-D848-4474-AD57-D3DE590DADCA" ); // Create Group:Start
            RockMigrationHelper.UpdateWorkflowActivityType( "3D736B06-45B3-4A1A-BDE5-5A46ED7DDC74", true, "Open", "", false, 1, "DFB5FA9F-7072-4533-90B6-5995E16D0D47" ); // Create Group:Open
            RockMigrationHelper.UpdateWorkflowActivityType( "3D736B06-45B3-4A1A-BDE5-5A46ED7DDC74", true, "Create Group", "", false, 2, "155D13EC-C38F-4375-B35A-3707578DA7F8" ); // Create Group:Create Group
            RockMigrationHelper.UpdateWorkflowActivityType( "3D736B06-45B3-4A1A-BDE5-5A46ED7DDC74", true, "Complete", "Complete the workflow", false, 3, "21AC97B4-22EE-4835-B8A4-4B02BB2D0F53" ); // Create Group:Complete
            RockMigrationHelper.UpdateWorkflowActivityType( "3D736B06-45B3-4A1A-BDE5-5A46ED7DDC74", true, "Deny", "", false, 4, "60D1FB14-816D-4292-985C-67EA87FE6EF4" ); // Create Group:Deny
            RockMigrationHelper.UpdateWorkflowActionForm( @"", @"", "Submit^^^Your information has been submitted successfully.", "", false, "", "4560C191-DC55-4E3D-B4F2-608993EDCD97" ); // Create Group:Start:Prompt User
            RockMigrationHelper.UpdateWorkflowActionForm( @"<h4>Create Group Request from {{ Workflow.Requester }}</h4>
<p>The following create group request has been submitted by a group leader.</p>", @"", "Deny^9b329020-e074-4326-8831-9dd534f491df^60D1FB14-816D-4292-985C-67EA87FE6EF4^The create group request has been denied.|Approve and Create^fdc397cd-8b4a-436e-bea1-bce2e6717c03^155D13EC-C38F-4375-B35A-3707578DA7F8^The group will now be created.|", "88C7D1CC-3478-4562-A301-AE7D4D7FFF6D", false, "83528E87-B330-4FD7-AC2D-A54D85AFFDB1", "AEEC6DA6-A75E-44D5-8E06-2FC1936D8986" ); // Create Group:Open:Admin Approval
            RockMigrationHelper.UpdateWorkflowActionFormAttribute( "4560C191-DC55-4E3D-B4F2-608993EDCD97", "03217B23-04CF-4FEC-AB69-962C3D72196D", 0, false, true, false, false, @"", @"", "0B113BA8-2362-4188-80BB-803BBAECC1F1" ); // Create Group:Start:Prompt User:Requester
            RockMigrationHelper.UpdateWorkflowActionFormAttribute( "4560C191-DC55-4E3D-B4F2-608993EDCD97", "ABF83FE3-D4AB-48A7-AD46-D34368B34401", 1, true, false, true, false, @"", @"", "376A48C4-BA26-4921-8DE1-9F64495B594A" ); // Create Group:Start:Prompt User:Proposed Group Name
            RockMigrationHelper.UpdateWorkflowActionFormAttribute( "4560C191-DC55-4E3D-B4F2-608993EDCD97", "B8118D49-5A48-4C55-BFC9-540A607762F1", 2, true, false, false, false, @"", @"", "937E219C-2954-45EE-A7B6-4450A29BE047" ); // Create Group:Start:Prompt User:Description
            RockMigrationHelper.UpdateWorkflowActionFormAttribute( "4560C191-DC55-4E3D-B4F2-608993EDCD97", "7250259C-C763-4344-8D9C-5C93BA66192E", 3, true, false, true, false, @"", @"", "48B1D50C-CD6C-44F5-8C7A-B448B028D378" ); // Create Group:Start:Prompt User:Campus
            RockMigrationHelper.UpdateWorkflowActionFormAttribute( "4560C191-DC55-4E3D-B4F2-608993EDCD97", "AE2B5FB2-68B1-4EA2-8793-687EC396FD68", 4, true, false, true, false, @"", @"", "E061E0CC-16BB-4756-BB01-FB0DF948A764" ); // Create Group:Start:Prompt User:Location
            RockMigrationHelper.UpdateWorkflowActionFormAttribute( "4560C191-DC55-4E3D-B4F2-608993EDCD97", "D6D1F80C-851E-4CC0-93BF-A548D02A0CAF", 5, true, false, false, false, @"", @"", "17173955-CE2F-4CD7-862D-B248F951A7E0" ); // Create Group:Start:Prompt User:Show Map to Location
            RockMigrationHelper.UpdateWorkflowActionFormAttribute( "4560C191-DC55-4E3D-B4F2-608993EDCD97", "EAD808C7-2D9E-4E8E-8841-71A74B3D6FAB", 6, false, true, false, false, @"", @"", "F263AF56-38E6-4CF2-8C97-C3EB8CF9A23F" ); // Create Group:Start:Prompt User:Group Type
            RockMigrationHelper.UpdateWorkflowActionFormAttribute( "4560C191-DC55-4E3D-B4F2-608993EDCD97", "C989B26F-4F4E-472E-BDF9-CDAFD197C66E", 7, false, true, false, false, @"", @"", "D0ADB004-3D5D-4506-9078-A2F737E99F23" ); // Create Group:Start:Prompt User:Parent Group/Category
            RockMigrationHelper.UpdateWorkflowActionFormAttribute( "4560C191-DC55-4E3D-B4F2-608993EDCD97", "A6782894-B6AE-4260-B361-E15BE17BA443", 8, false, true, false, false, @"", @"", "738AD4A2-A096-47D0-B518-088FB6D11E33" ); // Create Group:Start:Prompt User:Worker
            RockMigrationHelper.UpdateWorkflowActionFormAttribute( "4560C191-DC55-4E3D-B4F2-608993EDCD97", "83528E87-B330-4FD7-AC2D-A54D85AFFDB1", 9, false, true, false, false, @"", @"", "4D13B1E9-DC13-488E-9D77-7837CB83CD42" ); // Create Group:Start:Prompt User:Review Result
            RockMigrationHelper.UpdateWorkflowActionFormAttribute( "4560C191-DC55-4E3D-B4F2-608993EDCD97", "7ECBBF12-5AA1-4594-9B43-DD835C272C39", 10, false, true, false, false, @"", @"", "066059E7-99E9-47CB-88EF-3DBE4761E81D" ); // Create Group:Start:Prompt User:New Group
            RockMigrationHelper.UpdateWorkflowActionFormAttribute( "AEEC6DA6-A75E-44D5-8E06-2FC1936D8986", "03217B23-04CF-4FEC-AB69-962C3D72196D", 0, false, true, false, false, @"", @"", "FEE8012C-F75E-4B4E-9845-59962896ECC8" ); // Create Group:Open:Admin Approval:Requester
            RockMigrationHelper.UpdateWorkflowActionFormAttribute( "AEEC6DA6-A75E-44D5-8E06-2FC1936D8986", "ABF83FE3-D4AB-48A7-AD46-D34368B34401", 1, true, false, false, false, @"", @"", "5DF41F5E-B363-4F36-9907-83FC91B12384" ); // Create Group:Open:Admin Approval:Proposed Group Name
            RockMigrationHelper.UpdateWorkflowActionFormAttribute( "AEEC6DA6-A75E-44D5-8E06-2FC1936D8986", "B8118D49-5A48-4C55-BFC9-540A607762F1", 2, true, false, false, false, @"", @"", "39BCE663-B273-4284-9BA2-89853B133678" ); // Create Group:Open:Admin Approval:Description
            RockMigrationHelper.UpdateWorkflowActionFormAttribute( "AEEC6DA6-A75E-44D5-8E06-2FC1936D8986", "7250259C-C763-4344-8D9C-5C93BA66192E", 3, true, false, false, false, @"", @"", "8D27343E-FD8B-4C43-842C-0469982283C6" ); // Create Group:Open:Admin Approval:Campus
            RockMigrationHelper.UpdateWorkflowActionFormAttribute( "AEEC6DA6-A75E-44D5-8E06-2FC1936D8986", "AE2B5FB2-68B1-4EA2-8793-687EC396FD68", 4, true, false, false, false, @"", @"", "EBD50CD4-0F70-4F8D-99AD-ACB39862C323" ); // Create Group:Open:Admin Approval:Location
            RockMigrationHelper.UpdateWorkflowActionFormAttribute( "AEEC6DA6-A75E-44D5-8E06-2FC1936D8986", "D6D1F80C-851E-4CC0-93BF-A548D02A0CAF", 5, true, false, false, false, @"", @"", "D422AE7C-287D-408C-BA89-BFD47BA60FC7" ); // Create Group:Open:Admin Approval:Show Map to Location
            RockMigrationHelper.UpdateWorkflowActionFormAttribute( "AEEC6DA6-A75E-44D5-8E06-2FC1936D8986", "EAD808C7-2D9E-4E8E-8841-71A74B3D6FAB", 6, true, false, true, false, @"", @"", "D3B3507B-CCEB-43CB-B21D-6142D6BC810A" ); // Create Group:Open:Admin Approval:Group Type
            RockMigrationHelper.UpdateWorkflowActionFormAttribute( "AEEC6DA6-A75E-44D5-8E06-2FC1936D8986", "C989B26F-4F4E-472E-BDF9-CDAFD197C66E", 7, true, false, false, false, @"", @"", "D3496762-BDD5-4F72-B0D9-A5CE0FAB1255" ); // Create Group:Open:Admin Approval:Parent Group/Category
            RockMigrationHelper.UpdateWorkflowActionFormAttribute( "AEEC6DA6-A75E-44D5-8E06-2FC1936D8986", "A6782894-B6AE-4260-B361-E15BE17BA443", 8, false, true, false, false, @"", @"", "223B6F4D-B9DA-426A-9029-D358BEC067F5" ); // Create Group:Open:Admin Approval:Worker
            RockMigrationHelper.UpdateWorkflowActionFormAttribute( "AEEC6DA6-A75E-44D5-8E06-2FC1936D8986", "83528E87-B330-4FD7-AC2D-A54D85AFFDB1", 9, false, true, false, false, @"", @"", "60B08061-E4FB-4773-AC9C-5D7BB858F97A" ); // Create Group:Open:Admin Approval:Review Result
            RockMigrationHelper.UpdateWorkflowActionFormAttribute( "AEEC6DA6-A75E-44D5-8E06-2FC1936D8986", "7ECBBF12-5AA1-4594-9B43-DD835C272C39", 10, false, true, false, false, @"", @"", "C0239FC6-2272-4073-A303-AC05C97EEC76" ); // Create Group:Open:Admin Approval:New Group
            RockMigrationHelper.UpdateWorkflowActionType( "FBE45F3A-D848-4474-AD57-D3DE590DADCA", "Prompt User", 0, "486DC4FA-FCBC-425F-90B0-E606DA8A9F68", true, false, "4560C191-DC55-4E3D-B4F2-608993EDCD97", "", 1, "", "B6F019F0-38C4-4AC7-94C9-30A966C3A9E2" ); // Create Group:Start:Prompt User
            RockMigrationHelper.UpdateWorkflowActionType( "FBE45F3A-D848-4474-AD57-D3DE590DADCA", "Set Requester", 1, "24B7D5E6-C30F-48F4-9D7E-AF45A342CF3A", true, false, "", "", 1, "", "A8BBC717-598A-4A5B-BF1E-6F4D209C9E87" ); // Create Group:Start:Set Requester
            RockMigrationHelper.UpdateWorkflowActionType( "FBE45F3A-D848-4474-AD57-D3DE590DADCA", "Set Worker", 2, "17962C23-2E94-4E06-8461-0FB8B94E2FEA", true, false, "", "", 1, "", "9E0A7327-619C-431F-8FFB-DC4AE1F5ED9C" ); // Create Group:Start:Set Worker
            RockMigrationHelper.UpdateWorkflowActionType( "FBE45F3A-D848-4474-AD57-D3DE590DADCA", "Set Name", 3, "36005473-BD5D-470B-B28D-98E6D7ED808D", true, false, "", "", 1, "", "B263AC98-37C0-484A-9DB1-8BACE2D8A82C" ); // Create Group:Start:Set Name
            RockMigrationHelper.UpdateWorkflowActionType( "FBE45F3A-D848-4474-AD57-D3DE590DADCA", "Persist the Workflow", 4, "F1A39347-6FE0-43D4-89FB-544195088ECF", true, false, "", "", 1, "", "67BF45B3-A4B7-4AD5-85F6-04BAADEDFC89" ); // Create Group:Start:Persist the Workflow
            RockMigrationHelper.UpdateWorkflowActionType( "FBE45F3A-D848-4474-AD57-D3DE590DADCA", "Open", 5, "38907A90-1634-4A93-8017-619326A4A582", true, true, "", "", 1, "", "E8A4DD41-5232-4C68-A234-B0DDD39197BF" ); // Create Group:Start:Open
            RockMigrationHelper.UpdateWorkflowActionType( "DFB5FA9F-7072-4533-90B6-5995E16D0D47", "Assign to Worker", 0, "F100A31F-E93A-4C7A-9E55-0FAF41A101C4", true, false, "", "", 1, "", "A00FDB8E-A8F8-4DC0-BD0B-C06AC1F5DE50" ); // Create Group:Open:Assign to Worker
            RockMigrationHelper.UpdateWorkflowActionType( "DFB5FA9F-7072-4533-90B6-5995E16D0D47", "Admin Approval", 1, "486DC4FA-FCBC-425F-90B0-E606DA8A9F68", true, false, "AEEC6DA6-A75E-44D5-8E06-2FC1936D8986", "", 1, "", "8E139021-A4E3-4703-A450-B825DB7B0071" ); // Create Group:Open:Admin Approval
            RockMigrationHelper.UpdateWorkflowActionType( "155D13EC-C38F-4375-B35A-3707578DA7F8", "Create Group", 0, "B442940A-0C8B-4F44-8359-1E0AF3AAAB4C", true, false, "", "", 1, "", "51B0C6EA-A0A8-415C-9CCB-6CAB3A24DBF4" ); // Create Group:Create Group:Create Group
            RockMigrationHelper.UpdateWorkflowActionType( "155D13EC-C38F-4375-B35A-3707578DA7F8", "Add Requester to Group", 1, "BD53F375-78A2-4A54-B1D1-2D805F3FCD44", true, false, "", "7ECBBF12-5AA1-4594-9B43-DD835C272C39", 64, "", "3AAFF030-A0CF-4A2F-8DC2-46C8F8CFC918" ); // Create Group:Create Group:Add Requester to Group
            RockMigrationHelper.UpdateWorkflowActionType( "155D13EC-C38F-4375-B35A-3707578DA7F8", "Complete", 2, "38907A90-1634-4A93-8017-619326A4A582", true, true, "", "", 1, "", "9C42944F-B865-4D68-8AF0-AC015594B0CF" ); // Create Group:Create Group:Complete
            RockMigrationHelper.UpdateWorkflowActionType( "21AC97B4-22EE-4835-B8A4-4B02BB2D0F53", "Complete the workflow", 0, "EEDA4318-F014-4A46-9C76-4C052EF81AA1", true, true, "", "", 1, "", "58F15946-C370-4327-97C8-F730F465D041" ); // Create Group:Complete:Complete the workflow
            RockMigrationHelper.UpdateWorkflowActionType( "60D1FB14-816D-4292-985C-67EA87FE6EF4", "Activate Complete", 0, "38907A90-1634-4A93-8017-619326A4A582", true, true, "", "", 1, "", "6E184EA2-0B19-4DE1-B2F2-A24B7E35C3E3" ); // Create Group:Deny:Activate Complete
            RockMigrationHelper.AddActionTypeAttributeValue( "B6F019F0-38C4-4AC7-94C9-30A966C3A9E2", "234910F2-A0DB-4D7D-BAF7-83C880EF30AE", @"False" ); // Create Group:Start:Prompt User:Active
            RockMigrationHelper.AddActionTypeAttributeValue( "B6F019F0-38C4-4AC7-94C9-30A966C3A9E2", "C178113D-7C86-4229-8424-C6D0CF4A7E23", @"" ); // Create Group:Start:Prompt User:Order
            RockMigrationHelper.AddActionTypeAttributeValue( "A8BBC717-598A-4A5B-BF1E-6F4D209C9E87", "DE9CB292-4785-4EA3-976D-3826F91E9E98", @"False" ); // Create Group:Start:Set Requester:Active
            RockMigrationHelper.AddActionTypeAttributeValue( "A8BBC717-598A-4A5B-BF1E-6F4D209C9E87", "BBED8A83-8BB2-4D35-BAFB-05F67DCAD112", @"03217b23-04cf-4fec-ab69-962c3d72196d" ); // Create Group:Start:Set Requester:Person Attribute
            RockMigrationHelper.AddActionTypeAttributeValue( "A8BBC717-598A-4A5B-BF1E-6F4D209C9E87", "89E9BCED-91AB-47B0-AD52-D78B0B7CB9E8", @"" ); // Create Group:Start:Set Requester:Order
            RockMigrationHelper.AddActionTypeAttributeValue( "9E0A7327-619C-431F-8FFB-DC4AE1F5ED9C", "CE28B79D-FBC2-4894-9198-D923D0217549", @"False" ); // Create Group:Start:Set Worker:Active
            RockMigrationHelper.AddActionTypeAttributeValue( "9E0A7327-619C-431F-8FFB-DC4AE1F5ED9C", "7AC47975-71AC-4A2F-BF1F-115CF5578D6F", @"a6782894-b6ae-4260-b361-e15be17ba443" ); // Create Group:Start:Set Worker:Attribute
            RockMigrationHelper.AddActionTypeAttributeValue( "9E0A7327-619C-431F-8FFB-DC4AE1F5ED9C", "18EF907D-607E-4891-B034-7AA379D77854", @"" ); // Create Group:Start:Set Worker:Order
            RockMigrationHelper.AddActionTypePersonAttributeValue( "9E0A7327-619C-431F-8FFB-DC4AE1F5ED9C", "5C803BD1-40FA-49B1-AE7E-68F43D3687BB", @"42292aa8-c7da-404f-8181-348142fc9454" ); // Create Group:Start:Set Worker:Person
            RockMigrationHelper.AddActionTypeAttributeValue( "B263AC98-37C0-484A-9DB1-8BACE2D8A82C", "0A800013-51F7-4902-885A-5BE215D67D3D", @"False" ); // Create Group:Start:Set Name:Active
            RockMigrationHelper.AddActionTypeAttributeValue( "B263AC98-37C0-484A-9DB1-8BACE2D8A82C", "5D95C15A-CCAE-40AD-A9DD-F929DA587115", @"" ); // Create Group:Start:Set Name:Order
            RockMigrationHelper.AddActionTypeAttributeValue( "B263AC98-37C0-484A-9DB1-8BACE2D8A82C", "93852244-A667-4749-961A-D47F88675BE4", @"{{ Workflow.ProposedGroupName }} ( {{ Workflow.Requester }} )" ); // Create Group:Start:Set Name:Text Value|Attribute Value
            RockMigrationHelper.AddActionTypeAttributeValue( "67BF45B3-A4B7-4AD5-85F6-04BAADEDFC89", "50B01639-4938-40D2-A791-AA0EB4F86847", @"False" ); // Create Group:Start:Persist the Workflow:Active
            RockMigrationHelper.AddActionTypeAttributeValue( "67BF45B3-A4B7-4AD5-85F6-04BAADEDFC89", "86F795B0-0CB6-4DA4-9CE4-B11D0922F361", @"" ); // Create Group:Start:Persist the Workflow:Order
            RockMigrationHelper.AddActionTypeAttributeValue( "67BF45B3-A4B7-4AD5-85F6-04BAADEDFC89", "84E159D4-BB84-40CD-9B73-FE8C9C609F80", @"False" ); // Create Group:Start:Persist the Workflow:Persist Immediately
            RockMigrationHelper.AddActionTypeAttributeValue( "E8A4DD41-5232-4C68-A234-B0DDD39197BF", "E8ABD802-372C-47BE-82B1-96F50DB5169E", @"False" ); // Create Group:Start:Open:Active
            RockMigrationHelper.AddActionTypeAttributeValue( "E8A4DD41-5232-4C68-A234-B0DDD39197BF", "3809A78C-B773-440C-8E3F-A8E81D0DAE08", @"" ); // Create Group:Start:Open:Order
            RockMigrationHelper.AddActionTypeAttributeValue( "E8A4DD41-5232-4C68-A234-B0DDD39197BF", "02D5A7A5-8781-46B4-B9FC-AF816829D240", @"DFB5FA9F-7072-4533-90B6-5995E16D0D47" ); // Create Group:Start:Open:Activity
            RockMigrationHelper.AddActionTypeAttributeValue( "A00FDB8E-A8F8-4DC0-BD0B-C06AC1F5DE50", "E0F7AB7E-7761-4600-A099-CB14ACDBF6EF", @"False" ); // Create Group:Open:Assign to Worker:Active
            RockMigrationHelper.AddActionTypeAttributeValue( "A00FDB8E-A8F8-4DC0-BD0B-C06AC1F5DE50", "FBADD25F-D309-4512-8430-3CC8615DD60E", @"a6782894-b6ae-4260-b361-e15be17ba443" ); // Create Group:Open:Assign to Worker:Attribute
            RockMigrationHelper.AddActionTypeAttributeValue( "A00FDB8E-A8F8-4DC0-BD0B-C06AC1F5DE50", "7A6B605D-7FB1-4F48-AF35-5A0683FB1CDA", @"" ); // Create Group:Open:Assign to Worker:Order
            RockMigrationHelper.AddActionTypeAttributeValue( "8E139021-A4E3-4703-A450-B825DB7B0071", "234910F2-A0DB-4D7D-BAF7-83C880EF30AE", @"False" ); // Create Group:Open:Admin Approval:Active
            RockMigrationHelper.AddActionTypeAttributeValue( "8E139021-A4E3-4703-A450-B825DB7B0071", "C178113D-7C86-4229-8424-C6D0CF4A7E23", @"" ); // Create Group:Open:Admin Approval:Order
            RockMigrationHelper.AddActionTypeAttributeValue( "51B0C6EA-A0A8-415C-9CCB-6CAB3A24DBF4", "1242890B-DC92-48B2-8DC3-F6A20B920FAD", @"This is where the workflow should create the group.
GroupType: {{ Workflow | Attribute:'GroupType','Id' }}
ParentGroupId: {{ Workflow | Attribute:'ParentGroup','Id' }}
Name: {{ Workflow | Attribute:'ProposedGroupName' }}" ); // Create Group:Create Group:Create Group:Message
            RockMigrationHelper.AddActionTypeAttributeValue( "51B0C6EA-A0A8-415C-9CCB-6CAB3A24DBF4", "0DDD5B24-455B-4989-9C70-F31D78C7AC8F", @"" ); // Create Group:Create Group:Create Group:Order
            RockMigrationHelper.AddActionTypeAttributeValue( "51B0C6EA-A0A8-415C-9CCB-6CAB3A24DBF4", "C969ED7A-10FB-4655-A9D2-94E197B44603", @"False" ); // Create Group:Create Group:Create Group:Active
            RockMigrationHelper.AddActionTypeAttributeValue( "3AAFF030-A0CF-4A2F-8DC2-46C8F8CFC918", "3BCC80E3-68B9-47AA-8A04-40BC83D54289", @"03217b23-04cf-4fec-ab69-962c3d72196d" ); // Create Group:Create Group:Add Requester to Group:Person
            RockMigrationHelper.AddActionTypeAttributeValue( "3AAFF030-A0CF-4A2F-8DC2-46C8F8CFC918", "EE74986E-1368-4A68-BF96-9B5B621FA7F9", @"7ecbbf12-5aa1-4594-9b43-dd835c272c39" ); // Create Group:Create Group:Add Requester to Group:Group
            RockMigrationHelper.AddActionTypeAttributeValue( "3AAFF030-A0CF-4A2F-8DC2-46C8F8CFC918", "91EFE616-FF86-4A23-A16A-D5C2458782B2", @"" ); // Create Group:Create Group:Add Requester to Group:Order
            RockMigrationHelper.AddActionTypeAttributeValue( "3AAFF030-A0CF-4A2F-8DC2-46C8F8CFC918", "8D8F9DD3-2CE6-43C7-8995-399E9F985B84", @"False" ); // Create Group:Create Group:Add Requester to Group:Active
            RockMigrationHelper.AddActionTypeAttributeValue( "9C42944F-B865-4D68-8AF0-AC015594B0CF", "E8ABD802-372C-47BE-82B1-96F50DB5169E", @"False" ); // Create Group:Create Group:Complete:Active
            RockMigrationHelper.AddActionTypeAttributeValue( "9C42944F-B865-4D68-8AF0-AC015594B0CF", "3809A78C-B773-440C-8E3F-A8E81D0DAE08", @"" ); // Create Group:Create Group:Complete:Order
            RockMigrationHelper.AddActionTypeAttributeValue( "9C42944F-B865-4D68-8AF0-AC015594B0CF", "02D5A7A5-8781-46B4-B9FC-AF816829D240", @"21AC97B4-22EE-4835-B8A4-4B02BB2D0F53" ); // Create Group:Create Group:Complete:Activity
            RockMigrationHelper.AddActionTypeAttributeValue( "58F15946-C370-4327-97C8-F730F465D041", "0CA0DDEF-48EF-4ABC-9822-A05E225DE26C", @"False" ); // Create Group:Complete:Complete the workflow:Active
            RockMigrationHelper.AddActionTypeAttributeValue( "58F15946-C370-4327-97C8-F730F465D041", "25CAD4BE-5A00-409D-9BAB-E32518D89956", @"" ); // Create Group:Complete:Complete the workflow:Order
            RockMigrationHelper.AddActionTypeAttributeValue( "58F15946-C370-4327-97C8-F730F465D041", "B9325645-E512-4CB9-8E9E-730A7858A146", @"Completed" ); // Create Group:Complete:Complete the workflow:Status|Status Attribute
            RockMigrationHelper.AddActionTypeAttributeValue( "6E184EA2-0B19-4DE1-B2F2-A24B7E35C3E3", "E8ABD802-372C-47BE-82B1-96F50DB5169E", @"False" ); // Create Group:Deny:Activate Complete:Active
            RockMigrationHelper.AddActionTypeAttributeValue( "6E184EA2-0B19-4DE1-B2F2-A24B7E35C3E3", "3809A78C-B773-440C-8E3F-A8E81D0DAE08", @"" ); // Create Group:Deny:Activate Complete:Order
            RockMigrationHelper.AddActionTypeAttributeValue( "6E184EA2-0B19-4DE1-B2F2-A24B7E35C3E3", "02D5A7A5-8781-46B4-B9FC-AF816829D240", @"21AC97B4-22EE-4835-B8A4-4B02BB2D0F53" ); // Create Group:Deny:Activate Complete:Activity

            #endregion

            #region DefinedValue AttributeType qualifier helper

            Sql( @"
			    UPDATE [aq] SET [key] = 'definedtype', [Value] = CAST( [dt].[Id] as varchar(5) )
			    FROM [AttributeQualifier] [aq]
			    INNER JOIN [Attribute] [a] ON [a].[Id] = [aq].[AttributeId]
			    INNER JOIN [FieldType] [ft] ON [ft].[Id] = [a].[FieldTypeId]
			    INNER JOIN [DefinedType] [dt] ON CAST([dt].[guid] AS varchar(50) ) = [aq].[value]
			    WHERE [ft].[class] = 'Rock.Field.Types.DefinedValueFieldType'
			    AND [aq].[key] = 'definedtypeguid'
		    " );

            #endregion

        }

        /// <summary>
        /// The commands to undo a migration from a specific version
        /// </summary>
        public override void Down()
        {
            RockMigrationHelper.DeleteWorkflowType( "3D736B06-45B3-4A1A-BDE5-5A46ED7DDC74" );
        }

    }
}
