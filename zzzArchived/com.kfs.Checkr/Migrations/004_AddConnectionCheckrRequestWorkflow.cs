using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rock.Plugin;

namespace com.kfs.Checkr.Migrations
{
    [MigrationNumber( 4, "1.6.9" )]
    public class AddConnectionCheckrRequestWorkflow : Migration
    {
        /// <summary>
        /// The commands to run to migrate plugin to the specific version
        /// </summary>
        public override void Up()
        {
            // Create Checkr Background Check Connection Workflow Type
            #region EntityTypes

            RockMigrationHelper.UpdateEntityType( "Rock.Model.Workflow", "3540E9A7-FE30-43A9-8B0A-A372B63DFC93", true, true );
            RockMigrationHelper.UpdateEntityType( "Rock.Model.WorkflowActivity", "2CB52ED0-CB06-4D62-9E2C-73B60AFA4C9F", true, true );
            RockMigrationHelper.UpdateEntityType( "Rock.Model.WorkflowActionType", "23E3273A-B137-48A3-9AFF-C8DC832DDCA6", true, true );
            RockMigrationHelper.UpdateEntityType( "Rock.Workflow.Action.ActivateWorkflow", "EE0981F6-DC5B-401B-B8AF-A863CD48A38C", false, true );
            RockMigrationHelper.UpdateEntityType( "Rock.Workflow.Action.SetAttributeFromEntity", "972F19B9-598B-474B-97A4-50E56E7B59D2", false, true );
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "972F19B9-598B-474B-97A4-50E56E7B59D2", "1D0D3794-C210-48A8-8C68-3FBEC08A6BA5", "Lava Template", "LavaTemplate", "By default this action will set the attribute value equal to the guid (or id) of the entity that was passed in for processing. If you include a lava template here, the action will instead set the attribute value to the output of this template. The mergefield to use for the entity is 'Entity.' For example, use {{ Entity.Name }} if the entity has a Name property. <span class='tip tip-lava'></span>", 4, @"", "7D79FC31-D0ED-4DB0-AB7D-60F4F98A1199" ); // Rock.Workflow.Action.SetAttributeFromEntity:Lava Template
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "972F19B9-598B-474B-97A4-50E56E7B59D2", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Active", "Active", "Should Service be used?", 0, @"False", "9392E3D7-A28B-4CD8-8B03-5E147B102EF1" ); // Rock.Workflow.Action.SetAttributeFromEntity:Active
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "972F19B9-598B-474B-97A4-50E56E7B59D2", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Entity Is Required", "EntityIsRequired", "Should an error be returned if the entity is missing or not a valid entity type?", 2, @"True", "B524B00C-29CB-49E9-9896-8BB60F209783" ); // Rock.Workflow.Action.SetAttributeFromEntity:Entity Is Required
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "972F19B9-598B-474B-97A4-50E56E7B59D2", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Use Id instead of Guid", "UseId", "Most entity attribute field types expect the Guid of the entity (which is used by default). Select this option if the entity's Id should be used instead (should be rare).", 3, @"False", "1246C53A-FD92-4E08-ABDE-9A6C37E70C7B" ); // Rock.Workflow.Action.SetAttributeFromEntity:Use Id instead of Guid
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "972F19B9-598B-474B-97A4-50E56E7B59D2", "33E6DF69-BDFA-407A-9744-C175B60643AE", "Attribute", "Attribute", "The attribute to set the value of.", 1, @"", "61E6E1BC-E657-4F00-B2E9-769AAA25B9F7" ); // Rock.Workflow.Action.SetAttributeFromEntity:Attribute
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "972F19B9-598B-474B-97A4-50E56E7B59D2", "A75DFC58-7A1B-4799-BF31-451B2BBE38FF", "Order", "Order", "The order that this service should be used (priority)", 0, @"", "AD4EFAC4-E687-43DF-832F-0DC3856ABABB" ); // Rock.Workflow.Action.SetAttributeFromEntity:Order
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "EE0981F6-DC5B-401B-B8AF-A863CD48A38C", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Active", "Active", "Should Service be used?", 0, @"False", "742C0566-6CBB-4C42-8DCC-E7AA5EDA49EB" ); // Rock.Workflow.Action.ActivateWorkflow:Active
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "EE0981F6-DC5B-401B-B8AF-A863CD48A38C", "46A03F59-55D3-4ACE-ADD5-B4642225DD20", "Workflow Type", "WorkflowType", "The workflow type to activate", 0, @"", "317F9E07-F3DE-4F5B-AAFF-2C1D6876407E" ); // Rock.Workflow.Action.ActivateWorkflow:Workflow Type
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "EE0981F6-DC5B-401B-B8AF-A863CD48A38C", "73B02051-0D38-4AD9-BF81-A2D477DE4F70", "Workflow Attribute Key", "WorkflowAttributeKey", "Used to match the current workflow's attribute keys to the keys of the new workflow. The new workflow will inherit the attribute values of the keys provided.", 0, @"", "111C1C46-1FE2-44EB-89DE-806F0F13659B" ); // Rock.Workflow.Action.ActivateWorkflow:Workflow Attribute Key
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "EE0981F6-DC5B-401B-B8AF-A863CD48A38C", "9C204CD0-1233-41C5-818A-C5DA439445AA", "Workflow Name", "WorkflowName", "The name of your new workflow", 0, @"", "A16AF9BF-5969-4E77-AF8D-ED833638672B" ); // Rock.Workflow.Action.ActivateWorkflow:Workflow Name
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "EE0981F6-DC5B-401B-B8AF-A863CD48A38C", "A75DFC58-7A1B-4799-BF31-451B2BBE38FF", "Order", "Order", "The order that this service should be used (priority)", 0, @"", "150499F8-74E7-4362-8B6F-E1A4F25693BA" ); // Rock.Workflow.Action.ActivateWorkflow:Order

            #endregion

            #region Categories

            RockMigrationHelper.UpdateCategory( "C9F3C4A5-1526-474D-803F-D6C7A45CBBAE", "Safety & Security", "fa fa-medkit", "", "6F8A431C-BEBD-4D33-AAD6-1D70870329C2", 0 ); // Safety & Security

            #endregion

            #region Checkr Background Check Connection

            RockMigrationHelper.UpdateWorkflowType( false, true, "Checkr Background Check Connection", "Used to request a Checkr Background Check be sent to a person from a Connection Request", "6F8A431C-BEBD-4D33-AAD6-1D70870329C2", "Request", "fa fa-check-square-o", 28800, true, 0, "1588751E-4ECC-4A60-92DF-1CE2107A6331", 0 ); // Checkr Background Check Connection
            RockMigrationHelper.UpdateWorkflowTypeAttribute( "1588751E-4ECC-4A60-92DF-1CE2107A6331", "E4EAB7B2-0B76-429B-AFE4-AD86D7428C70", "Person", "Person", "", 0, @"", "ED053AF1-1BD8-4EC2-A7EE-E4AE94A8D4F7", false ); // Checkr Background Check Connection:Person
            RockMigrationHelper.UpdateWorkflowTypeAttribute( "1588751E-4ECC-4A60-92DF-1CE2107A6331", "1B71FEF4-201F-4D53-8C60-2DF21F1985ED", "Campus", "Campus", "", 1, @"", "7FB63197-1C3D-47A6-983E-FCB5C5AA5422", false ); // Checkr Background Check Connection:Campus
            RockMigrationHelper.UpdateWorkflowTypeAttribute( "1588751E-4ECC-4A60-92DF-1CE2107A6331", "9C204CD0-1233-41C5-818A-C5DA439445AA", "Reason", "Reason", "", 2, @"Checkr Background Check requested via Connection Request", "CBE80482-D568-4600-AC46-B1AF53B9A250", false ); // Checkr Background Check Connection:Reason
            RockMigrationHelper.UpdateWorkflowTypeAttribute( "1588751E-4ECC-4A60-92DF-1CE2107A6331", "A75DFC58-7A1B-4799-BF31-451B2BBE38FF", "Connection Request Id", "ConnectionRequestId", "", 3, @"0", "990AAC58-82EF-49A1-B7B2-147C2840DCF6", false ); // Checkr Background Check Connection:Connection Request Id
            RockMigrationHelper.AddAttributeQualifier( "ED053AF1-1BD8-4EC2-A7EE-E4AE94A8D4F7", "EnableSelfSelection", @"False", "BE7F2481-038B-4C08-806D-2A026579ABDC" ); // Checkr Background Check Connection:Person:EnableSelfSelection
            RockMigrationHelper.AddAttributeQualifier( "7FB63197-1C3D-47A6-983E-FCB5C5AA5422", "includeInactive", @"False", "7A7047A3-DD93-4FCB-B0A2-EE1254213B0C" ); // Checkr Background Check Connection:Campus:includeInactive
            RockMigrationHelper.AddAttributeQualifier( "CBE80482-D568-4600-AC46-B1AF53B9A250", "ispassword", @"False", "C056780C-C05C-46A7-8760-ABDCF78AFA0D" ); // Checkr Background Check Connection:Reason:ispassword
            RockMigrationHelper.UpdateWorkflowActivityType( "1588751E-4ECC-4A60-92DF-1CE2107A6331", true, "Start", "", true, 0, "E37B7067-B9D7-4642-B901-EB310FB57B30" ); // Checkr Background Check Connection:Start
            RockMigrationHelper.UpdateWorkflowActionType( "E37B7067-B9D7-4642-B901-EB310FB57B30", "Set Person", 0, "972F19B9-598B-474B-97A4-50E56E7B59D2", true, false, "", "", 1, "", "B925FF5E-6040-4270-82AF-583EAE458B83" ); // Checkr Background Check Connection:Start:Set Person
            RockMigrationHelper.UpdateWorkflowActionType( "E37B7067-B9D7-4642-B901-EB310FB57B30", "Set Campus", 1, "972F19B9-598B-474B-97A4-50E56E7B59D2", true, false, "", "", 1, "", "4C4E1043-5907-4B8C-9A7D-EE3FCEDFEA06" ); // Checkr Background Check Connection:Start:Set Campus
            RockMigrationHelper.UpdateWorkflowActionType( "E37B7067-B9D7-4642-B901-EB310FB57B30", "Set Connection Request Id", 2, "972F19B9-598B-474B-97A4-50E56E7B59D2", true, false, "", "", 1, "", "F932EB1B-3C5F-4C04-B672-C449CF7C7117" ); // Checkr Background Check Connection:Start:Set Connection Request Id
            RockMigrationHelper.UpdateWorkflowActionType( "E37B7067-B9D7-4642-B901-EB310FB57B30", "Initiate Checkr Background Check Workflow", 3, "EE0981F6-DC5B-401B-B8AF-A863CD48A38C", true, false, "", "", 1, "", "0EC5897B-7237-448F-BD9E-3BADB617C86D" ); // Checkr Background Check Connection:Start:Initiate Checkr Background Check Workflow
            RockMigrationHelper.AddActionTypeAttributeValue( "B925FF5E-6040-4270-82AF-583EAE458B83", "9392E3D7-A28B-4CD8-8B03-5E147B102EF1", @"False" ); // Checkr Background Check Connection:Start:Set Person:Active
            RockMigrationHelper.AddActionTypeAttributeValue( "B925FF5E-6040-4270-82AF-583EAE458B83", "AD4EFAC4-E687-43DF-832F-0DC3856ABABB", @"" ); // Checkr Background Check Connection:Start:Set Person:Order
            RockMigrationHelper.AddActionTypeAttributeValue( "B925FF5E-6040-4270-82AF-583EAE458B83", "61E6E1BC-E657-4F00-B2E9-769AAA25B9F7", @"ed053af1-1bd8-4ec2-a7ee-e4ae94a8d4f7" ); // Checkr Background Check Connection:Start:Set Person:Attribute
            RockMigrationHelper.AddActionTypeAttributeValue( "B925FF5E-6040-4270-82AF-583EAE458B83", "B524B00C-29CB-49E9-9896-8BB60F209783", @"True" ); // Checkr Background Check Connection:Start:Set Person:Entity Is Required
            RockMigrationHelper.AddActionTypeAttributeValue( "B925FF5E-6040-4270-82AF-583EAE458B83", "1246C53A-FD92-4E08-ABDE-9A6C37E70C7B", @"False" ); // Checkr Background Check Connection:Start:Set Person:Use Id instead of Guid
            RockMigrationHelper.AddActionTypeAttributeValue( "B925FF5E-6040-4270-82AF-583EAE458B83", "7D79FC31-D0ED-4DB0-AB7D-60F4F98A1199", @"{{ Entity.PersonAlias.Guid }}" ); // Checkr Background Check Connection:Start:Set Person:Lava Template
            RockMigrationHelper.AddActionTypeAttributeValue( "4C4E1043-5907-4B8C-9A7D-EE3FCEDFEA06", "9392E3D7-A28B-4CD8-8B03-5E147B102EF1", @"False" ); // Checkr Background Check Connection:Start:Set Campus:Active
            RockMigrationHelper.AddActionTypeAttributeValue( "4C4E1043-5907-4B8C-9A7D-EE3FCEDFEA06", "AD4EFAC4-E687-43DF-832F-0DC3856ABABB", @"" ); // Checkr Background Check Connection:Start:Set Campus:Order
            RockMigrationHelper.AddActionTypeAttributeValue( "4C4E1043-5907-4B8C-9A7D-EE3FCEDFEA06", "61E6E1BC-E657-4F00-B2E9-769AAA25B9F7", @"7fb63197-1c3d-47a6-983e-fcb5c5aa5422" ); // Checkr Background Check Connection:Start:Set Campus:Attribute
            RockMigrationHelper.AddActionTypeAttributeValue( "4C4E1043-5907-4B8C-9A7D-EE3FCEDFEA06", "B524B00C-29CB-49E9-9896-8BB60F209783", @"True" ); // Checkr Background Check Connection:Start:Set Campus:Entity Is Required
            RockMigrationHelper.AddActionTypeAttributeValue( "4C4E1043-5907-4B8C-9A7D-EE3FCEDFEA06", "1246C53A-FD92-4E08-ABDE-9A6C37E70C7B", @"False" ); // Checkr Background Check Connection:Start:Set Campus:Use Id instead of Guid
            RockMigrationHelper.AddActionTypeAttributeValue( "4C4E1043-5907-4B8C-9A7D-EE3FCEDFEA06", "7D79FC31-D0ED-4DB0-AB7D-60F4F98A1199", @"{{ Entity.Campus.Guid }}" ); // Checkr Background Check Connection:Start:Set Campus:Lava Template
            RockMigrationHelper.AddActionTypeAttributeValue( "F932EB1B-3C5F-4C04-B672-C449CF7C7117", "9392E3D7-A28B-4CD8-8B03-5E147B102EF1", @"False" ); // Checkr Background Check Connection:Start:Set Connection Request Id:Active
            RockMigrationHelper.AddActionTypeAttributeValue( "F932EB1B-3C5F-4C04-B672-C449CF7C7117", "AD4EFAC4-E687-43DF-832F-0DC3856ABABB", @"" ); // Checkr Background Check Connection:Start:Set Connection Request Id:Order
            RockMigrationHelper.AddActionTypeAttributeValue( "F932EB1B-3C5F-4C04-B672-C449CF7C7117", "61E6E1BC-E657-4F00-B2E9-769AAA25B9F7", @"990aac58-82ef-49a1-b7b2-147c2840dcf6" ); // Checkr Background Check Connection:Start:Set Connection Request Id:Attribute
            RockMigrationHelper.AddActionTypeAttributeValue( "F932EB1B-3C5F-4C04-B672-C449CF7C7117", "B524B00C-29CB-49E9-9896-8BB60F209783", @"True" ); // Checkr Background Check Connection:Start:Set Connection Request Id:Entity Is Required
            RockMigrationHelper.AddActionTypeAttributeValue( "F932EB1B-3C5F-4C04-B672-C449CF7C7117", "1246C53A-FD92-4E08-ABDE-9A6C37E70C7B", @"False" ); // Checkr Background Check Connection:Start:Set Connection Request Id:Use Id instead of Guid
            RockMigrationHelper.AddActionTypeAttributeValue( "F932EB1B-3C5F-4C04-B672-C449CF7C7117", "7D79FC31-D0ED-4DB0-AB7D-60F4F98A1199", @"{{ Entity.Id }}" ); // Checkr Background Check Connection:Start:Set Connection Request Id:Lava Template
            RockMigrationHelper.AddActionTypeAttributeValue( "0EC5897B-7237-448F-BD9E-3BADB617C86D", "A16AF9BF-5969-4E77-AF8D-ED833638672B", @"Checkr Background Check from Connection Request" ); // Checkr Background Check Connection:Start:Initiate Checkr Background Check Workflow:Workflow Name
            RockMigrationHelper.AddActionTypeAttributeValue( "0EC5897B-7237-448F-BD9E-3BADB617C86D", "317F9E07-F3DE-4F5B-AAFF-2C1D6876407E", @"18729d36-b352-4676-87e7-44651fa7f76a" ); // Checkr Background Check Connection:Start:Initiate Checkr Background Check Workflow:Workflow Type
            RockMigrationHelper.AddActionTypeAttributeValue( "0EC5897B-7237-448F-BD9E-3BADB617C86D", "111C1C46-1FE2-44EB-89DE-806F0F13659B", @"Person^Person|Reason^Reason|Campus^Campus|ConnectionRequestId^ConnectionRequestId" ); // Checkr Background Check Connection:Start:Initiate Checkr Background Check Workflow:Workflow Attribute Key
            RockMigrationHelper.AddActionTypeAttributeValue( "0EC5897B-7237-448F-BD9E-3BADB617C86D", "150499F8-74E7-4362-8B6F-E1A4F25693BA", @"" ); // Checkr Background Check Connection:Start:Initiate Checkr Background Check Workflow:Order
            RockMigrationHelper.AddActionTypeAttributeValue( "0EC5897B-7237-448F-BD9E-3BADB617C86D", "742C0566-6CBB-4C42-8DCC-E7AA5EDA49EB", @"False" ); // Checkr Background Check Connection:Start:Initiate Checkr Background Check Workflow:Active

            #endregion

            #region DefinedValue AttributeType qualifier helper

            Sql( @"     UPDATE [aq] SET [key] = 'definedtype', [Value] = CAST( [dt].[Id] as varchar(5) )     FROM [AttributeQualifier] [aq]     INNER JOIN [Attribute] [a] ON [a].[Id] = [aq].[AttributeId]     INNER JOIN [FieldType] [ft] ON [ft].[Id] = [a].[FieldTypeId]     INNER JOIN [DefinedType] [dt] ON CAST([dt].[guid] AS varchar(50) ) = [aq].[value]     WHERE [ft].[class] = 'Rock.Field.Types.DefinedValueFieldType'     AND [aq].[key] = 'definedtypeguid'    " );

            #endregion
        }

        /// <summary>
        /// The commands to undo a migration from a specific version
        /// </summary>
        public override void Down()
        {
            RockMigrationHelper.DeleteWorkflowType( "1588751E-4ECC-4A60-92DF-1CE2107A6331" );
        }
    }
}
