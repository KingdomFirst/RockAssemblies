using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rock.Plugin;

namespace com.kfs.MinistrySafe.Migrations
{
    [MigrationNumber( 5, "1.6.9" )]
    class CreateConnectionTrainingWorkflow : Migration
    {
        /// <summary>
        /// The commands to run to migrate plugin to the specific version
        /// </summary>
        public override void Up()
        {
            // Create Ministry Safe Connection Training Workflow
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

            #region Ministry Safe Connection

            RockMigrationHelper.UpdateWorkflowType( false, true, "Ministry Safe Connection", "Used to request a Ministry Safe Training be sent to a person from a Connection Request", "6F8A431C-BEBD-4D33-AAD6-1D70870329C2", "Training", "fa fa-clipboard", 28800, true, 0, "E7D5B3F9-45FD-4091-971A-BD5D4BD80538", 0 ); // Ministry Safe Connection
            RockMigrationHelper.UpdateWorkflowTypeAttribute( "E7D5B3F9-45FD-4091-971A-BD5D4BD80538", "E4EAB7B2-0B76-429B-AFE4-AD86D7428C70", "Person", "Person", "", 0, @"", "AE8F3513-6B55-486D-AA3E-5E84613607F0", false ); // Ministry Safe Connection:Person
            RockMigrationHelper.UpdateWorkflowTypeAttribute( "E7D5B3F9-45FD-4091-971A-BD5D4BD80538", "1B71FEF4-201F-4D53-8C60-2DF21F1985ED", "Campus", "Campus", "", 1, @"", "C93D7901-A1A6-4DA8-BA7E-F4054DC44C1B", false ); // Ministry Safe Connection:Campus
            RockMigrationHelper.UpdateWorkflowTypeAttribute( "E7D5B3F9-45FD-4091-971A-BD5D4BD80538", "9C204CD0-1233-41C5-818A-C5DA439445AA", "Reason", "Reason", "", 2, @"Ministry Safe Training requested via Connection Request", "EE2F92D8-FDD0-4AD4-8E80-9C4D1EBBAE85", false ); // Ministry Safe Connection:Reason
            RockMigrationHelper.UpdateWorkflowTypeAttribute( "E7D5B3F9-45FD-4091-971A-BD5D4BD80538", "A75DFC58-7A1B-4799-BF31-451B2BBE38FF", "Connection Request Id", "ConnectionRequestId", "", 3, @"0", "04F0BFBA-F5E6-46E4-8F1B-711E5E8BAA35", false ); // Ministry Safe Connection:Connection Request Id
            RockMigrationHelper.AddAttributeQualifier( "AE8F3513-6B55-486D-AA3E-5E84613607F0", "EnableSelfSelection", @"False", "41D5F2C7-DEB3-43B0-B894-7D314E750A0F" ); // Ministry Safe Connection:Person:EnableSelfSelection
            RockMigrationHelper.AddAttributeQualifier( "C93D7901-A1A6-4DA8-BA7E-F4054DC44C1B", "includeInactive", @"False", "FAFEC056-54FD-4951-8DB8-2DBC30795F3A" ); // Ministry Safe Connection:Campus:includeInactive
            RockMigrationHelper.AddAttributeQualifier( "EE2F92D8-FDD0-4AD4-8E80-9C4D1EBBAE85", "ispassword", @"False", "714616BD-9003-460A-B1F6-EA320CCF4293" ); // Ministry Safe Connection:Reason:ispassword
            RockMigrationHelper.UpdateWorkflowActivityType( "E7D5B3F9-45FD-4091-971A-BD5D4BD80538", true, "Start", "", true, 0, "2AB25A27-6097-4B27-A414-49975A0FC740" ); // Ministry Safe Connection:Start
            RockMigrationHelper.UpdateWorkflowActionType( "2AB25A27-6097-4B27-A414-49975A0FC740", "Set Person", 0, "972F19B9-598B-474B-97A4-50E56E7B59D2", true, false, "", "", 1, "", "2F6D1E4E-925A-4DEB-8C66-F0859A19D35A" ); // Ministry Safe Connection:Start:Set Person
            RockMigrationHelper.UpdateWorkflowActionType( "2AB25A27-6097-4B27-A414-49975A0FC740", "Set Campus", 1, "972F19B9-598B-474B-97A4-50E56E7B59D2", true, false, "", "", 1, "", "BDFE1843-FB92-4E55-A86D-6497C2457DE6" ); // Ministry Safe Connection:Start:Set Campus
            RockMigrationHelper.UpdateWorkflowActionType( "2AB25A27-6097-4B27-A414-49975A0FC740", "Set Connection Request Id", 2, "972F19B9-598B-474B-97A4-50E56E7B59D2", true, false, "", "", 1, "", "4563750D-5465-4AAC-8550-DAF277273667" ); // Ministry Safe Connection:Start:Set Connection Request Id
            RockMigrationHelper.UpdateWorkflowActionType( "2AB25A27-6097-4B27-A414-49975A0FC740", "Initiate Ministry Safe Training Workflow", 3, "EE0981F6-DC5B-401B-B8AF-A863CD48A38C", true, false, "", "", 1, "", "BEEA2051-6799-463A-9F65-9EB517D08554" ); // Ministry Safe Connection:Start:Initiate Ministry Safe Training Workflow
            RockMigrationHelper.AddActionTypeAttributeValue( "2F6D1E4E-925A-4DEB-8C66-F0859A19D35A", "9392E3D7-A28B-4CD8-8B03-5E147B102EF1", @"False" ); // Ministry Safe Connection:Start:Set Person:Active
            RockMigrationHelper.AddActionTypeAttributeValue( "2F6D1E4E-925A-4DEB-8C66-F0859A19D35A", "AD4EFAC4-E687-43DF-832F-0DC3856ABABB", @"" ); // Ministry Safe Connection:Start:Set Person:Order
            RockMigrationHelper.AddActionTypeAttributeValue( "2F6D1E4E-925A-4DEB-8C66-F0859A19D35A", "61E6E1BC-E657-4F00-B2E9-769AAA25B9F7", @"ae8f3513-6b55-486d-aa3e-5e84613607f0" ); // Ministry Safe Connection:Start:Set Person:Attribute
            RockMigrationHelper.AddActionTypeAttributeValue( "2F6D1E4E-925A-4DEB-8C66-F0859A19D35A", "B524B00C-29CB-49E9-9896-8BB60F209783", @"True" ); // Ministry Safe Connection:Start:Set Person:Entity Is Required
            RockMigrationHelper.AddActionTypeAttributeValue( "2F6D1E4E-925A-4DEB-8C66-F0859A19D35A", "1246C53A-FD92-4E08-ABDE-9A6C37E70C7B", @"False" ); // Ministry Safe Connection:Start:Set Person:Use Id instead of Guid
            RockMigrationHelper.AddActionTypeAttributeValue( "2F6D1E4E-925A-4DEB-8C66-F0859A19D35A", "7D79FC31-D0ED-4DB0-AB7D-60F4F98A1199", @"{{ Entity.PersonAlias.Guid }}" ); // Ministry Safe Connection:Start:Set Person:Lava Template
            RockMigrationHelper.AddActionTypeAttributeValue( "BDFE1843-FB92-4E55-A86D-6497C2457DE6", "9392E3D7-A28B-4CD8-8B03-5E147B102EF1", @"False" ); // Ministry Safe Connection:Start:Set Campus:Active
            RockMigrationHelper.AddActionTypeAttributeValue( "BDFE1843-FB92-4E55-A86D-6497C2457DE6", "AD4EFAC4-E687-43DF-832F-0DC3856ABABB", @"" ); // Ministry Safe Connection:Start:Set Campus:Order
            RockMigrationHelper.AddActionTypeAttributeValue( "BDFE1843-FB92-4E55-A86D-6497C2457DE6", "61E6E1BC-E657-4F00-B2E9-769AAA25B9F7", @"c93d7901-a1a6-4da8-ba7e-f4054dc44c1b" ); // Ministry Safe Connection:Start:Set Campus:Attribute
            RockMigrationHelper.AddActionTypeAttributeValue( "BDFE1843-FB92-4E55-A86D-6497C2457DE6", "B524B00C-29CB-49E9-9896-8BB60F209783", @"True" ); // Ministry Safe Connection:Start:Set Campus:Entity Is Required
            RockMigrationHelper.AddActionTypeAttributeValue( "BDFE1843-FB92-4E55-A86D-6497C2457DE6", "1246C53A-FD92-4E08-ABDE-9A6C37E70C7B", @"False" ); // Ministry Safe Connection:Start:Set Campus:Use Id instead of Guid
            RockMigrationHelper.AddActionTypeAttributeValue( "BDFE1843-FB92-4E55-A86D-6497C2457DE6", "7D79FC31-D0ED-4DB0-AB7D-60F4F98A1199", @"{{ Entity.Campus.Guid }}" ); // Ministry Safe Connection:Start:Set Campus:Lava Template
            RockMigrationHelper.AddActionTypeAttributeValue( "4563750D-5465-4AAC-8550-DAF277273667", "9392E3D7-A28B-4CD8-8B03-5E147B102EF1", @"False" ); // Ministry Safe Connection:Start:Set Connection Request Id:Active
            RockMigrationHelper.AddActionTypeAttributeValue( "4563750D-5465-4AAC-8550-DAF277273667", "AD4EFAC4-E687-43DF-832F-0DC3856ABABB", @"" ); // Ministry Safe Connection:Start:Set Connection Request Id:Order
            RockMigrationHelper.AddActionTypeAttributeValue( "4563750D-5465-4AAC-8550-DAF277273667", "61E6E1BC-E657-4F00-B2E9-769AAA25B9F7", @"04f0bfba-f5e6-46e4-8f1b-711e5e8baa35" ); // Ministry Safe Connection:Start:Set Connection Request Id:Attribute
            RockMigrationHelper.AddActionTypeAttributeValue( "4563750D-5465-4AAC-8550-DAF277273667", "B524B00C-29CB-49E9-9896-8BB60F209783", @"True" ); // Ministry Safe Connection:Start:Set Connection Request Id:Entity Is Required
            RockMigrationHelper.AddActionTypeAttributeValue( "4563750D-5465-4AAC-8550-DAF277273667", "1246C53A-FD92-4E08-ABDE-9A6C37E70C7B", @"False" ); // Ministry Safe Connection:Start:Set Connection Request Id:Use Id instead of Guid
            RockMigrationHelper.AddActionTypeAttributeValue( "4563750D-5465-4AAC-8550-DAF277273667", "7D79FC31-D0ED-4DB0-AB7D-60F4F98A1199", @"{{ Entity.Id }}" ); // Ministry Safe Connection:Start:Set Connection Request Id:Lava Template
            RockMigrationHelper.AddActionTypeAttributeValue( "BEEA2051-6799-463A-9F65-9EB517D08554", "A16AF9BF-5969-4E77-AF8D-ED833638672B", @"Ministry Safe Training from Connection Request" ); // Ministry Safe Connection:Start:Initiate Ministry Safe Training Workflow:Workflow Name
            RockMigrationHelper.AddActionTypeAttributeValue( "BEEA2051-6799-463A-9F65-9EB517D08554", "317F9E07-F3DE-4F5B-AAFF-2C1D6876407E", @"ae617426-e1e3-4e06-b3d8-b7ab1a7b4892" ); // Ministry Safe Connection:Start:Initiate Ministry Safe Training Workflow:Workflow Type
            RockMigrationHelper.AddActionTypeAttributeValue( "BEEA2051-6799-463A-9F65-9EB517D08554", "111C1C46-1FE2-44EB-89DE-806F0F13659B", @"Person^Person|Reason^Reason|Campus^Campus|ConnectionRequestId^ConnectionRequestId" ); // Ministry Safe Connection:Start:Initiate Ministry Safe Training Workflow:Workflow Attribute Key
            RockMigrationHelper.AddActionTypeAttributeValue( "BEEA2051-6799-463A-9F65-9EB517D08554", "150499F8-74E7-4362-8B6F-E1A4F25693BA", @"" ); // Ministry Safe Connection:Start:Initiate Ministry Safe Training Workflow:Order
            RockMigrationHelper.AddActionTypeAttributeValue( "BEEA2051-6799-463A-9F65-9EB517D08554", "742C0566-6CBB-4C42-8DCC-E7AA5EDA49EB", @"False" ); // Ministry Safe Connection:Start:Initiate Ministry Safe Training Workflow:Active

            #endregion
        }

        /// <summary>
        /// The commands to undo a migration from a specific version
        /// </summary>
        public override void Down()
        {
            RockMigrationHelper.DeleteWorkflowType( "E7D5B3F9-45FD-4091-971A-BD5D4BD80538" );
        }
    }
}
