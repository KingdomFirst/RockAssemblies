// <copyright>
// Copyright 2026 by Kingdom First Solutions
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

namespace rocks.kfs.MobileAppMigration.Migrations
{
    /// <summary>
    /// Workflows and binary assets for the Nfluence Church mobile application.
    ///
    /// Workflows generated with Dev Tools/Sql/CodeGen_WorkflowTypeMigration.sql
    /// Binary file generated with Dev Tools/Sql/CodeGen_BinaryFileMigration.sql
    ///
    /// Run AFTER migration 2 (the app itself) - the Workflow Entry blocks on the
    /// mobile pages reference these workflow types by Guid.
    /// </summary>
    [MigrationNumber( 2, "1.16.0" )]
    public class NfluenceMobileAppWorkflows : Migration
    {
        public override void Up()
        {

            //
            // Workflow: Post-Service Survey
            //
            #region FieldTypes
            #endregion
            #region EntityTypes
            RockMigrationHelper.UpdateEntityType("Rock.Model.Workflow", "3540E9A7-FE30-43A9-8B0A-A372B63DFC93", true, true);
            RockMigrationHelper.UpdateEntityType("Rock.Model.WorkflowActivity", "2CB52ED0-CB06-4D62-9E2C-73B60AFA4C9F", true, true);
            RockMigrationHelper.UpdateEntityType("Rock.Model.WorkflowActionType", "23E3273A-B137-48A3-9AFF-C8DC832DDCA6", true, true);
            RockMigrationHelper.UpdateEntityType("Rock.Workflow.Action.CompleteWorkflow","EEDA4318-F014-4A46-9C76-4C052EF81AA1",false,true);
            RockMigrationHelper.UpdateEntityType("Rock.Workflow.Action.PersistWorkflow","F1A39347-6FE0-43D4-89FB-544195088ECF",false,true);
            RockMigrationHelper.UpdateEntityType("Rock.Workflow.Action.SendEmail","66197B01-D1F0-4924-A315-47AD54E030DE",false,true);
            RockMigrationHelper.UpdateEntityType("Rock.Workflow.Action.SetWorkflowName","36005473-BD5D-470B-B28D-98E6D7ED808D",false,true);
            RockMigrationHelper.UpdateEntityType("Rock.Workflow.Action.UserEntryForm","486DC4FA-FCBC-425F-90B0-E606DA8A9F68",false,true);
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("36005473-BD5D-470B-B28D-98E6D7ED808D","1EDAFDED-DFE6-4334-B019-6EECBA89E05A","Active","Active","Should Service be used?",0,@"False","0A800013-51F7-4902-885A-5BE215D67D3D"); // Rock.Workflow.Action.SetWorkflowName:Active
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("36005473-BD5D-470B-B28D-98E6D7ED808D","3B1D93D7-9414-48F9-80E5-6A3FC8F94C20","Text Value|Attribute Value","NameValue","The value to use for the workflow's name. <span class='tip tip-lava'></span>",1,@"","93852244-A667-4749-961A-D47F88675BE4"); // Rock.Workflow.Action.SetWorkflowName:Text Value|Attribute Value
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("36005473-BD5D-470B-B28D-98E6D7ED808D","A75DFC58-7A1B-4799-BF31-451B2BBE38FF","Order","Order","The order that this service should be used (priority)",0,@"","5D95C15A-CCAE-40AD-A9DD-F929DA587115"); // Rock.Workflow.Action.SetWorkflowName:Order
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("486DC4FA-FCBC-425F-90B0-E606DA8A9F68","1EDAFDED-DFE6-4334-B019-6EECBA89E05A","Active","Active","Should Service be used?",0,@"False","234910F2-A0DB-4D7D-BAF7-83C880EF30AE"); // Rock.Workflow.Action.UserEntryForm:Active
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("486DC4FA-FCBC-425F-90B0-E606DA8A9F68","A75DFC58-7A1B-4799-BF31-451B2BBE38FF","Order","Order","The order that this service should be used (priority)",0,@"","C178113D-7C86-4229-8424-C6D0CF4A7E23"); // Rock.Workflow.Action.UserEntryForm:Order
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("66197B01-D1F0-4924-A315-47AD54E030DE","1D0D3794-C210-48A8-8C68-3FBEC08A6BA5","Body","Body","The body of the email that should be sent. <span class='tip tip-lava'></span> <span class='tip tip-html'></span>",6,@"","4D245B9E-6B03-46E7-8482-A51FBA190E4D"); // Rock.Workflow.Action.SendEmail:Body
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("66197B01-D1F0-4924-A315-47AD54E030DE","1EDAFDED-DFE6-4334-B019-6EECBA89E05A","Active","Active","Should Service be used?",0,@"False","36197160-7D3D-490D-AB42-7E29105AFE91"); // Rock.Workflow.Action.SendEmail:Active
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("66197B01-D1F0-4924-A315-47AD54E030DE","1EDAFDED-DFE6-4334-B019-6EECBA89E05A","Save Communication History","SaveCommunicationHistory","Should a record of this communication be saved to the recipient's profile?",12,@"False","1BDC7ACA-9A0B-4C8A-909E-8B4143D9C2A3"); // Rock.Workflow.Action.SendEmail:Save Communication History
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("66197B01-D1F0-4924-A315-47AD54E030DE","33E6DF69-BDFA-407A-9744-C175B60643AE","Attachment One","AttachmentOne","Workflow attribute that contains the email attachment. Note file size that can be sent is limited by both the sending and receiving email services typically 10 - 25 MB.",9,@"","C2C7DA55-3018-4645-B9EE-4BCD11855F2C"); // Rock.Workflow.Action.SendEmail:Attachment One
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("66197B01-D1F0-4924-A315-47AD54E030DE","33E6DF69-BDFA-407A-9744-C175B60643AE","Attachment Three","AttachmentThree","Workflow attribute that contains the email attachment. Note file size that can be sent is limited by both the sending and receiving email services typically 10 - 25 MB.",11,@"","A059767A-5592-4926-948A-1065AF4E9748"); // Rock.Workflow.Action.SendEmail:Attachment Three
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("66197B01-D1F0-4924-A315-47AD54E030DE","33E6DF69-BDFA-407A-9744-C175B60643AE","Attachment Two","AttachmentTwo","Workflow attribute that contains the email attachment. Note file size that can be sent is limited by both the sending and receiving email services typically 10 - 25 MB.",10,@"","FFD9193A-451F-40E6-9776-74D5DCAC1450"); // Rock.Workflow.Action.SendEmail:Attachment Two
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("66197B01-D1F0-4924-A315-47AD54E030DE","33E6DF69-BDFA-407A-9744-C175B60643AE","Send to Group Role","GroupRole","An optional Group Role attribute to limit recipients to if the 'Send to Email Addresses' is a group or security role.",4,@"","E3667110-339F-4FE3-B6B7-084CF9633580"); // Rock.Workflow.Action.SendEmail:Send to Group Role
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("66197B01-D1F0-4924-A315-47AD54E030DE","3B1D93D7-9414-48F9-80E5-6A3FC8F94C20","BCC Email Addresses|BCC Attribute","BCC","The email addresses or an attribute that contains the person, email address, group or security role that the email should be BCC'd (blind carbon copied) to. Any address in this field will be copied on the email sent to every recipient. <span class='tip tip-lava'></span>",8,@"","3A131021-CB73-44A8-A142-B42832B77F60"); // Rock.Workflow.Action.SendEmail:BCC Email Addresses|BCC Attribute
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("66197B01-D1F0-4924-A315-47AD54E030DE","3B1D93D7-9414-48F9-80E5-6A3FC8F94C20","CC Email Addresses|CC Attribute","CC","The email addresses or an attribute that contains the person, email address, group or security role that the email should be CC'd (carbon copied) to. Any address in this field will be copied on the email sent to every recipient. <span class='tip tip-lava'></span>",7,@"","99FFD423-2AB6-481B-8749-B4793A16B620"); // Rock.Workflow.Action.SendEmail:CC Email Addresses|CC Attribute
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("66197B01-D1F0-4924-A315-47AD54E030DE","3B1D93D7-9414-48F9-80E5-6A3FC8F94C20","From Email Address|From Attribute","From","The email address or an attribute that contains the person or email address that email should be sent from (will default to organization email). <span class='tip tip-lava'></span>",1,@"","9F5F7CEC-F369-4FDF-802A-99074CE7A7FC"); // Rock.Workflow.Action.SendEmail:From Email Address|From Attribute
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("66197B01-D1F0-4924-A315-47AD54E030DE","3B1D93D7-9414-48F9-80E5-6A3FC8F94C20","From Name|From Name Attribute","FromName","The name or an attribute that contains the person or name that email should be sent from. <span class='tip tip-lava'></span>",0,@"","E37C6F14-0D82-4E29-A735-2276BA94986A"); // Rock.Workflow.Action.SendEmail:From Name|From Name Attribute
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("66197B01-D1F0-4924-A315-47AD54E030DE","3B1D93D7-9414-48F9-80E5-6A3FC8F94C20","Reply To Address|Reply To Attribute","ReplyTo","The email address or an attribute that contains the person or email address that email replies should be sent to (will default to 'From' email). <span class='tip tip-lava'></span>",2,@"","1937BC8A-195E-48E7-9602-B8901D315CF2"); // Rock.Workflow.Action.SendEmail:Reply To Address|Reply To Attribute
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("66197B01-D1F0-4924-A315-47AD54E030DE","3B1D93D7-9414-48F9-80E5-6A3FC8F94C20","Send To Email Addresses|To Attribute","To","The email addresses or an attribute that contains the person, email address, group or security role that the email should be sent to. <span class='tip tip-lava'></span>",3,@"","0C4C13B8-7076-4872-925A-F950886B5E16"); // Rock.Workflow.Action.SendEmail:Send To Email Addresses|To Attribute
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("66197B01-D1F0-4924-A315-47AD54E030DE","9C204CD0-1233-41C5-818A-C5DA439445AA","Subject","Subject","The subject that should be used when sending email. <span class='tip tip-lava'></span>",5,@"","5D9B13B6-CD96-4C7C-86FA-4512B9D28386"); // Rock.Workflow.Action.SendEmail:Subject
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("66197B01-D1F0-4924-A315-47AD54E030DE","A75DFC58-7A1B-4799-BF31-451B2BBE38FF","Order","Order","The order that this service should be used (priority)",0,@"","D1269254-C15A-40BD-B784-ADCC231D3950"); // Rock.Workflow.Action.SendEmail:Order
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("EEDA4318-F014-4A46-9C76-4C052EF81AA1","1EDAFDED-DFE6-4334-B019-6EECBA89E05A","Active","Active","Should Service be used?",0,@"False","0CA0DDEF-48EF-4ABC-9822-A05E225DE26C"); // Rock.Workflow.Action.CompleteWorkflow:Active
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("EEDA4318-F014-4A46-9C76-4C052EF81AA1","3B1D93D7-9414-48F9-80E5-6A3FC8F94C20","Status|Status Attribute","Status","The status to set the workflow to when marking the workflow complete. <span class='tip tip-lava'></span>",0,@"Completed","385A255B-9F48-4625-862B-26231DBAC53A"); // Rock.Workflow.Action.CompleteWorkflow:Status|Status Attribute
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("EEDA4318-F014-4A46-9C76-4C052EF81AA1","A75DFC58-7A1B-4799-BF31-451B2BBE38FF","Order","Order","The order that this service should be used (priority)",0,@"","25CAD4BE-5A00-409D-9BAB-E32518D89956"); // Rock.Workflow.Action.CompleteWorkflow:Order
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("F1A39347-6FE0-43D4-89FB-544195088ECF","1EDAFDED-DFE6-4334-B019-6EECBA89E05A","Active","Active","Should Service be used?",0,@"False","50B01639-4938-40D2-A791-AA0EB4F86847"); // Rock.Workflow.Action.PersistWorkflow:Active
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("F1A39347-6FE0-43D4-89FB-544195088ECF","1EDAFDED-DFE6-4334-B019-6EECBA89E05A","Persist Immediately","PersistImmediately","This action will normally cause the workflow to be persisted (saved) once all the current activities/actions have completed processing. Set this flag to true, if the workflow should be persisted immediately. This is only required if a subsequent action needs a persisted workflow with a valid id.",0,@"False","E22BE348-18B1-4420-83A8-6319B35416D2"); // Rock.Workflow.Action.PersistWorkflow:Persist Immediately
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("F1A39347-6FE0-43D4-89FB-544195088ECF","A75DFC58-7A1B-4799-BF31-451B2BBE38FF","Order","Order","The order that this service should be used (priority)",0,@"","86F795B0-0CB6-4DA4-9CE4-B11D0922F361"); // Rock.Workflow.Action.PersistWorkflow:Order
            #endregion
            #region Categories
            RockMigrationHelper.UpdateCategory("C9F3C4A5-1526-474D-803F-D6C7A45CBBAE","Requests","fa fa-question-circle","","78E38655-D951-41DB-A0FF-D6474775CFA1",0); // Requests
            #endregion
            #region Post-Service Survey
            RockMigrationHelper.UpdateWorkflowType(false,true,"Post-Service Survey","","78E38655-D951-41DB-A0FF-D6474775CFA1","Survey","fa fa-list-ol",28800,false,0,"766B7660-857E-4C6B-80D1-1A34002D657B",0); // Post-Service Survey
            RockMigrationHelper.UpdateWorkflowTypeAttribute("766B7660-857E-4C6B-80D1-1A34002D657B","E4EAB7B2-0B76-429B-AFE4-AD86D7428C70","Person","Person","",0,@"","86319AFF-090E-41D1-BAAD-CF5FE2DCC0A7", false); // Post-Service Survey:Person
            RockMigrationHelper.UpdateWorkflowTypeAttribute("766B7660-857E-4C6B-80D1-1A34002D657B","7525C4CB-EE6B-41D4-9B64-A08048D5A5C0","Did you attend service in person or via our live stream platform?","Attendance","",1,@"","56BE12AD-D18D-4C16-98B4-6667A1674348", false); // Post-Service Survey:Did you attend service in person or via our live stream platform?
            RockMigrationHelper.UpdateWorkflowTypeAttribute("766B7660-857E-4C6B-80D1-1A34002D657B","C28C7BF3-A552-4D77-9408-DEDCF760CED0","How did today''s service impact you personally or spiritually?","ServiceImpact","",2,@"","A3C3BF28-91F3-430C-8FE3-9C800A40082B", false); // Post-Service Survey:How did today's service impact you personally or spiritually?
            RockMigrationHelper.UpdateWorkflowTypeAttribute("766B7660-857E-4C6B-80D1-1A34002D657B","7525C4CB-EE6B-41D4-9B64-A08048D5A5C0","Did you sense the presence of God during today''s service?","SensedPresence","",3,@"","CF1D1C54-8FDF-4E9C-8339-19A6FFE9318C", false); // Post-Service Survey:Did you sense the presence of God during today's service?
            RockMigrationHelper.UpdateWorkflowTypeAttribute("766B7660-857E-4C6B-80D1-1A34002D657B","7525C4CB-EE6B-41D4-9B64-A08048D5A5C0","Welcome & Hospitality","RateWelcome","",4,@"","E0CC0A1E-A5DB-41F4-B655-C4C7A6273D31", false); // Post-Service Survey:Welcome & Hospitality
            RockMigrationHelper.UpdateWorkflowTypeAttribute("766B7660-857E-4C6B-80D1-1A34002D657B","7525C4CB-EE6B-41D4-9B64-A08048D5A5C0","Worship & Atmosphere","RateWorship","",5,@"","10302E7B-6CFB-4B29-908C-EE4B9E7883AC", false); // Post-Service Survey:Worship & Atmosphere
            RockMigrationHelper.UpdateWorkflowTypeAttribute("766B7660-857E-4C6B-80D1-1A34002D657B","7525C4CB-EE6B-41D4-9B64-A08048D5A5C0","Sermon / Message","RateSermon","",6,@"","20BBAF4E-30EB-46EF-8B8A-03FC6724F658", false); // Post-Service Survey:Sermon / Message
            RockMigrationHelper.UpdateWorkflowTypeAttribute("766B7660-857E-4C6B-80D1-1A34002D657B","7525C4CB-EE6B-41D4-9B64-A08048D5A5C0","Kids Ministry (if applicable)","RateKids","",7,@"","3D388184-8EFA-43FC-A1EE-F780752898D0", false); // Post-Service Survey:Kids Ministry (if applicable)
            RockMigrationHelper.UpdateWorkflowTypeAttribute("766B7660-857E-4C6B-80D1-1A34002D657B","7525C4CB-EE6B-41D4-9B64-A08048D5A5C0","Clarity of Communication (signage, screens, announcements)","RateClarity","",8,@"","E44C2076-BDD3-4661-ACFD-5148D56D9B74", false); // Post-Service Survey:Clarity of Communication (signage, screens, announcements)
            RockMigrationHelper.UpdateWorkflowTypeAttribute("766B7660-857E-4C6B-80D1-1A34002D657B","7525C4CB-EE6B-41D4-9B64-A08048D5A5C0","Overall Service Experience","RateOverall","",9,@"","F2A3DC83-D3B4-4F2C-BE8E-BF7D9906F851", false); // Post-Service Survey:Overall Service Experience
            RockMigrationHelper.UpdateWorkflowTypeAttribute("766B7660-857E-4C6B-80D1-1A34002D657B","C28C7BF3-A552-4D77-9408-DEDCF760CED0","Do you have a testimony, story or God moment you''d like to share? Or explanation to any of your responses?","Testimony","",10,@"","FACF18A7-86DC-4827-BD29-8B4820D8A81F", false); // Post-Service Survey:Do you have a testimony, story or God moment you'd like to share? Or explanation to any of your responses?
            RockMigrationHelper.AddAttributeQualifier("86319AFF-090E-41D1-BAAD-CF5FE2DCC0A7","EnableSelfSelection",@"False","14A179C9-5364-4FA0-B1C0-0F4DBD797E4E"); // Post-Service Survey:Person:EnableSelfSelection
            RockMigrationHelper.AddAttributeQualifier("86319AFF-090E-41D1-BAAD-CF5FE2DCC0A7","includeBusinesses",@"False","FD1A3514-1D94-4D1B-B1A5-4D568EA11EE7"); // Post-Service Survey:Person:includeBusinesses
            RockMigrationHelper.AddAttributeQualifier("56BE12AD-D18D-4C16-98B4-6667A1674348","fieldtype",@"ddl","69AE288E-FEA8-40BA-9234-7F5DFCC173C0"); // Post-Service Survey:Did you attend service in person or via our live stream platform?:fieldtype
            RockMigrationHelper.AddAttributeQualifier("56BE12AD-D18D-4C16-98B4-6667A1674348","repeatColumns",@"","D9E1F8C4-14DE-4969-9D55-1ED672BD0A79"); // Post-Service Survey:Did you attend service in person or via our live stream platform?:repeatColumns
            RockMigrationHelper.AddAttributeQualifier("56BE12AD-D18D-4C16-98B4-6667A1674348","values",@"In Person, Online","0CD3DE75-DAE1-4037-8C43-13EC74CE438F"); // Post-Service Survey:Did you attend service in person or via our live stream platform?:values
            RockMigrationHelper.AddAttributeQualifier("A3C3BF28-91F3-430C-8FE3-9C800A40082B","allowhtml",@"False","89DBD5F6-A063-4F7F-9D66-D591FE78FD89"); // Post-Service Survey:How did today's service impact you personally or spiritually?:allowhtml
            RockMigrationHelper.AddAttributeQualifier("A3C3BF28-91F3-430C-8FE3-9C800A40082B","maxcharacters",@"","0BB9D10E-D592-4457-8793-4C25BD11B920"); // Post-Service Survey:How did today's service impact you personally or spiritually?:maxcharacters
            RockMigrationHelper.AddAttributeQualifier("A3C3BF28-91F3-430C-8FE3-9C800A40082B","numberofrows",@"","856644B2-A057-483F-8D0E-9CB607E3D746"); // Post-Service Survey:How did today's service impact you personally or spiritually?:numberofrows
            RockMigrationHelper.AddAttributeQualifier("A3C3BF28-91F3-430C-8FE3-9C800A40082B","showcountdown",@"False","EF8F182D-01C6-40F4-BE22-948C12EA2D41"); // Post-Service Survey:How did today's service impact you personally or spiritually?:showcountdown
            RockMigrationHelper.AddAttributeQualifier("CF1D1C54-8FDF-4E9C-8339-19A6FFE9318C","fieldtype",@"ddl","F541EE08-B00E-49F7-B24A-1138676EC206"); // Post-Service Survey:Did you sense the presence of God during today's service?:fieldtype
            RockMigrationHelper.AddAttributeQualifier("CF1D1C54-8FDF-4E9C-8339-19A6FFE9318C","repeatColumns",@"","C74D57B2-8758-469B-BE78-8E85C91013A8"); // Post-Service Survey:Did you sense the presence of God during today's service?:repeatColumns
            RockMigrationHelper.AddAttributeQualifier("CF1D1C54-8FDF-4E9C-8339-19A6FFE9318C","values",@"Strongly, Somewhat, Not Really, Not Sure","3D4DEF94-4D44-40C4-9055-00041C34FF56"); // Post-Service Survey:Did you sense the presence of God during today's service?:values
            RockMigrationHelper.AddAttributeQualifier("E0CC0A1E-A5DB-41F4-B655-C4C7A6273D31","fieldtype",@"rb","4CA5ED37-2CFD-44E1-8DF8-7DF78B7A9597"); // Post-Service Survey:Welcome & Hospitality:fieldtype
            RockMigrationHelper.AddAttributeQualifier("E0CC0A1E-A5DB-41F4-B655-C4C7A6273D31","repeatColumns",@"","7C805F8D-BCD5-4061-8E77-A1370E668204"); // Post-Service Survey:Welcome & Hospitality:repeatColumns
            RockMigrationHelper.AddAttributeQualifier("E0CC0A1E-A5DB-41F4-B655-C4C7A6273D31","values",@"1^★ Poor,2^★★ Fair,3^★★★ Good,4^★★★★ Great,5^★★★★★ Excellent","9F10DEBA-E437-46D4-882A-51603D7D88B4"); // Post-Service Survey:Welcome & Hospitality:values
            RockMigrationHelper.AddAttributeQualifier("10302E7B-6CFB-4B29-908C-EE4B9E7883AC","fieldtype",@"rb","D78CCB1C-4A06-4426-A0D9-6713B358974D"); // Post-Service Survey:Worship & Atmosphere:fieldtype
            RockMigrationHelper.AddAttributeQualifier("10302E7B-6CFB-4B29-908C-EE4B9E7883AC","repeatColumns",@"","522DF7B1-D6F6-4CC6-BA43-9A5BAE97C532"); // Post-Service Survey:Worship & Atmosphere:repeatColumns
            RockMigrationHelper.AddAttributeQualifier("10302E7B-6CFB-4B29-908C-EE4B9E7883AC","values",@"1^★ Poor,2^★★ Fair,3^★★★ Good,4^★★★★ Great,5^★★★★★ Excellent","E40479FD-5AF3-43E3-879C-6B7DA0F07B79"); // Post-Service Survey:Worship & Atmosphere:values
            RockMigrationHelper.AddAttributeQualifier("20BBAF4E-30EB-46EF-8B8A-03FC6724F658","fieldtype",@"rb","723DA2A4-07E1-4329-815D-9DD7C9784C9F"); // Post-Service Survey:Sermon / Message:fieldtype
            RockMigrationHelper.AddAttributeQualifier("20BBAF4E-30EB-46EF-8B8A-03FC6724F658","repeatColumns",@"","8C83122D-2C1E-49AC-B6C3-CC1DB1980A38"); // Post-Service Survey:Sermon / Message:repeatColumns
            RockMigrationHelper.AddAttributeQualifier("20BBAF4E-30EB-46EF-8B8A-03FC6724F658","values",@"1^★ Poor,2^★★ Fair,3^★★★ Good,4^★★★★ Great,5^★★★★★ Excellent","F386B0E9-4DA9-4340-AAB5-DE3C0B7209C3"); // Post-Service Survey:Sermon / Message:values
            RockMigrationHelper.AddAttributeQualifier("3D388184-8EFA-43FC-A1EE-F780752898D0","fieldtype",@"rb","60DF8470-25A6-4C11-912C-CE5153EF912C"); // Post-Service Survey:Kids Ministry (if applicable):fieldtype
            RockMigrationHelper.AddAttributeQualifier("3D388184-8EFA-43FC-A1EE-F780752898D0","repeatColumns",@"","87FC3FA0-F181-4C97-95BE-2C8D91F1D7FA"); // Post-Service Survey:Kids Ministry (if applicable):repeatColumns
            RockMigrationHelper.AddAttributeQualifier("3D388184-8EFA-43FC-A1EE-F780752898D0","values",@"1^★ Poor,2^★★ Fair,3^★★★ Good,4^★★★★ Great,5^★★★★★ Excellent","D723751A-C928-47D0-BDB3-969181EDCD40"); // Post-Service Survey:Kids Ministry (if applicable):values
            RockMigrationHelper.AddAttributeQualifier("E44C2076-BDD3-4661-ACFD-5148D56D9B74","fieldtype",@"rb","4174F967-C3F0-493B-BE08-5D75606ADB91"); // Post-Service Survey:Clarity of Communication (signage, screens, announcements):fieldtype
            RockMigrationHelper.AddAttributeQualifier("E44C2076-BDD3-4661-ACFD-5148D56D9B74","repeatColumns",@"","047C73DF-4825-4312-96AE-0B2967AFE196"); // Post-Service Survey:Clarity of Communication (signage, screens, announcements):repeatColumns
            RockMigrationHelper.AddAttributeQualifier("E44C2076-BDD3-4661-ACFD-5148D56D9B74","values",@"1^★ Poor,2^★★ Fair,3^★★★ Good,4^★★★★ Great,5^★★★★★ Excellent","8538F5BA-79C2-4251-84FD-E04023CE0B06"); // Post-Service Survey:Clarity of Communication (signage, screens, announcements):values
            RockMigrationHelper.AddAttributeQualifier("F2A3DC83-D3B4-4F2C-BE8E-BF7D9906F851","fieldtype",@"rb","8681A7EB-D435-42DD-8CF7-F03AD915DE32"); // Post-Service Survey:Overall Service Experience:fieldtype
            RockMigrationHelper.AddAttributeQualifier("F2A3DC83-D3B4-4F2C-BE8E-BF7D9906F851","repeatColumns",@"","46219D8D-0B2C-4DA4-972D-00DDC2CBC91A"); // Post-Service Survey:Overall Service Experience:repeatColumns
            RockMigrationHelper.AddAttributeQualifier("F2A3DC83-D3B4-4F2C-BE8E-BF7D9906F851","values",@"1^★ Poor,2^★★ Fair,3^★★★ Good,4^★★★★ Great,5^★★★★★ Excellent","62C4D6A0-70E9-4C9A-8D6D-04C8F6923915"); // Post-Service Survey:Overall Service Experience:values
            RockMigrationHelper.AddAttributeQualifier("FACF18A7-86DC-4827-BD29-8B4820D8A81F","allowhtml",@"False","A5BD3F01-D73F-455D-B24E-23E879EBBB52"); // Post-Service Survey:Do you have a testimony, story or God moment you'd like to share? Or explanation to any of your responses?:allowhtml
            RockMigrationHelper.AddAttributeQualifier("FACF18A7-86DC-4827-BD29-8B4820D8A81F","maxcharacters",@"","9967B431-CDDF-49B9-BC53-4B519E6C11C5"); // Post-Service Survey:Do you have a testimony, story or God moment you'd like to share? Or explanation to any of your responses?:maxcharacters
            RockMigrationHelper.AddAttributeQualifier("FACF18A7-86DC-4827-BD29-8B4820D8A81F","numberofrows",@"5","DF5DB996-7BAF-4A18-8B04-E8BC9E8DF72C"); // Post-Service Survey:Do you have a testimony, story or God moment you'd like to share? Or explanation to any of your responses?:numberofrows
            RockMigrationHelper.AddAttributeQualifier("FACF18A7-86DC-4827-BD29-8B4820D8A81F","showcountdown",@"False","349151DE-E65D-44AC-8989-262B50D3240C"); // Post-Service Survey:Do you have a testimony, story or God moment you'd like to share? Or explanation to any of your responses?:showcountdown
            RockMigrationHelper.UpdateWorkflowActivityType("766B7660-857E-4C6B-80D1-1A34002D657B",true,"Start","",true,0,"88B2D2EF-DE0F-4927-BEB8-3A1CD7574B79"); // Post-Service Survey:Start
            RockMigrationHelper.UpdateWorkflowActivityType("766B7660-857E-4C6B-80D1-1A34002D657B",true,"Send Email and Complete","",false,1,"CC137BA3-5C68-4CF2-B665-D2EBC8B2F282"); // Post-Service Survey:Send Email and Complete
            RockMigrationHelper.UpdateWorkflowActionForm(@"<p>Your feedback helps us grow and serve with excellence. Thank you for sharing your experience today!</p>",@"","Submit^fdc397cd-8b4a-436e-bea1-bce2e6717c03^CC137BA3-5C68-4CF2-B665-D2EBC8B2F282^Thank you! Your feedback has been received.|","",false,"","4C6FE585-DE93-4DC4-B17D-B0E772B6826F"); // Post-Service Survey:Start:Survey Form
            RockMigrationHelper.UpdateWorkflowActionFormAttribute("4C6FE585-DE93-4DC4-B17D-B0E772B6826F","86319AFF-090E-41D1-BAAD-CF5FE2DCC0A7",0,false,true,false,false,@"",@"","C83E51C8-6735-4248-8F0A-A74B19574D72"); // Post-Service Survey:Start:Survey Form:Person
            RockMigrationHelper.UpdateWorkflowActionFormAttribute("4C6FE585-DE93-4DC4-B17D-B0E772B6826F","56BE12AD-D18D-4C16-98B4-6667A1674348",1,true,false,true,false,@"",@"","DD604123-A317-421B-9C8D-91FAE335FC36"); // Post-Service Survey:Start:Survey Form:Did you attend service in person or via our live stream platform?
            RockMigrationHelper.UpdateWorkflowActionFormAttribute("4C6FE585-DE93-4DC4-B17D-B0E772B6826F","A3C3BF28-91F3-430C-8FE3-9C800A40082B",2,true,false,true,false,@"",@"","6F2A27B7-6B00-46E7-845A-A4ED5ABAFAF6"); // Post-Service Survey:Start:Survey Form:How did today's service impact you personally or spiritually?
            RockMigrationHelper.UpdateWorkflowActionFormAttribute("4C6FE585-DE93-4DC4-B17D-B0E772B6826F","CF1D1C54-8FDF-4E9C-8339-19A6FFE9318C",3,true,false,true,false,@"",@"","E2444106-7C0B-49EF-B876-FDAEEE349618"); // Post-Service Survey:Start:Survey Form:Did you sense the presence of God during today's service?
            RockMigrationHelper.UpdateWorkflowActionFormAttribute("4C6FE585-DE93-4DC4-B17D-B0E772B6826F","E0CC0A1E-A5DB-41F4-B655-C4C7A6273D31",4,true,false,true,false,@"<h3>How would you rate the following?</h3>",@"","124AFA67-4480-433B-B7A8-681CDC19B99A"); // Post-Service Survey:Start:Survey Form:Welcome & Hospitality
            RockMigrationHelper.UpdateWorkflowActionFormAttribute("4C6FE585-DE93-4DC4-B17D-B0E772B6826F","10302E7B-6CFB-4B29-908C-EE4B9E7883AC",5,true,false,true,false,@"",@"","F7BCA9D5-0FA2-4FD5-B47F-31F1CB57C2AC"); // Post-Service Survey:Start:Survey Form:Worship & Atmosphere
            RockMigrationHelper.UpdateWorkflowActionFormAttribute("4C6FE585-DE93-4DC4-B17D-B0E772B6826F","20BBAF4E-30EB-46EF-8B8A-03FC6724F658",6,true,false,true,false,@"",@"","93C02459-A8BB-411B-9424-C632E83B0BA0"); // Post-Service Survey:Start:Survey Form:Sermon / Message
            RockMigrationHelper.UpdateWorkflowActionFormAttribute("4C6FE585-DE93-4DC4-B17D-B0E772B6826F","3D388184-8EFA-43FC-A1EE-F780752898D0",7,true,false,false,false,@"",@"","F8115B12-511E-4C48-B104-933DEC7E6338"); // Post-Service Survey:Start:Survey Form:Kids Ministry (if applicable)
            RockMigrationHelper.UpdateWorkflowActionFormAttribute("4C6FE585-DE93-4DC4-B17D-B0E772B6826F","E44C2076-BDD3-4661-ACFD-5148D56D9B74",8,true,false,true,false,@"",@"","5154B8F7-C77A-448B-BC68-3EAABB816FDE"); // Post-Service Survey:Start:Survey Form:Clarity of Communication (signage, screens, announcements)
            RockMigrationHelper.UpdateWorkflowActionFormAttribute("4C6FE585-DE93-4DC4-B17D-B0E772B6826F","F2A3DC83-D3B4-4F2C-BE8E-BF7D9906F851",9,true,false,true,false,@"",@"","DF7E7EF7-5673-4477-8606-6B449B14ED4D"); // Post-Service Survey:Start:Survey Form:Overall Service Experience
            RockMigrationHelper.UpdateWorkflowActionFormAttribute("4C6FE585-DE93-4DC4-B17D-B0E772B6826F","FACF18A7-86DC-4827-BD29-8B4820D8A81F",10,true,false,true,false,@"",@"","9577E7C1-8CFE-4E1F-B097-2C9DD13A4060"); // Post-Service Survey:Start:Survey Form:Do you have a testimony, story or God moment you'd like to share? Or explanation to any of your responses?
            RockMigrationHelper.UpdateWorkflowActionType("88B2D2EF-DE0F-4927-BEB8-3A1CD7574B79","Survey Form",0,"486DC4FA-FCBC-425F-90B0-E606DA8A9F68",true,false,"4C6FE585-DE93-4DC4-B17D-B0E772B6826F","",1,"","E2E71D1B-8B40-4EF6-8394-29A5DEC7813F"); // Post-Service Survey:Start:Survey Form
            RockMigrationHelper.UpdateWorkflowActionType("CC137BA3-5C68-4CF2-B665-D2EBC8B2F282","Persist Workflow",0,"F1A39347-6FE0-43D4-89FB-544195088ECF",true,false,"","",1,"","3121BA94-28A2-444D-A7D7-1D23405366DC"); // Post-Service Survey:Send Email and Complete:Persist Workflow
            RockMigrationHelper.UpdateWorkflowActionType("CC137BA3-5C68-4CF2-B665-D2EBC8B2F282","Rename Workflow",1,"36005473-BD5D-470B-B28D-98E6D7ED808D",true,false,"","",1,"","F7D2B755-5D41-4A33-9307-44595833EA44"); // Post-Service Survey:Send Email and Complete:Rename Workflow
            RockMigrationHelper.UpdateWorkflowActionType("CC137BA3-5C68-4CF2-B665-D2EBC8B2F282","Email Staff",2,"66197B01-D1F0-4924-A315-47AD54E030DE",true,false,"","",1,"","42D286D6-72D3-42D2-9ED4-6B49718BE228"); // Post-Service Survey:Send Email and Complete:Email Staff
            RockMigrationHelper.UpdateWorkflowActionType("CC137BA3-5C68-4CF2-B665-D2EBC8B2F282","Complete Workflow",3,"EEDA4318-F014-4A46-9C76-4C052EF81AA1",true,false,"","",1,"","ECBB69BE-50EB-4DCD-A357-03A38866BB8C"); // Post-Service Survey:Send Email and Complete:Complete Workflow
            RockMigrationHelper.AddActionTypeAttributeValue("E2E71D1B-8B40-4EF6-8394-29A5DEC7813F","234910F2-A0DB-4D7D-BAF7-83C880EF30AE",@"False"); // Post-Service Survey:Start:Survey Form:Active
            RockMigrationHelper.AddActionTypeAttributeValue("3121BA94-28A2-444D-A7D7-1D23405366DC","50B01639-4938-40D2-A791-AA0EB4F86847",@"False"); // Post-Service Survey:Send Email and Complete:Persist Workflow:Active
            RockMigrationHelper.AddActionTypeAttributeValue("3121BA94-28A2-444D-A7D7-1D23405366DC","E22BE348-18B1-4420-83A8-6319B35416D2",@"False"); // Post-Service Survey:Send Email and Complete:Persist Workflow:Persist Immediately
            RockMigrationHelper.AddActionTypeAttributeValue("F7D2B755-5D41-4A33-9307-44595833EA44","0A800013-51F7-4902-885A-5BE215D67D3D",@"False"); // Post-Service Survey:Send Email and Complete:Rename Workflow:Active
            RockMigrationHelper.AddActionTypeAttributeValue("F7D2B755-5D41-4A33-9307-44595833EA44","93852244-A667-4749-961A-D47F88675BE4",@"Survey – {{ Workflow | Attribute:'FirstName' }} {{ Workflow | Attribute:'LastName' }} – {{ 'Now' | Date:'M/d' }}"); // Post-Service Survey:Send Email and Complete:Rename Workflow:Text Value|Attribute Value
            RockMigrationHelper.AddActionTypeAttributeValue("42D286D6-72D3-42D2-9ED4-6B49718BE228","36197160-7D3D-490D-AB42-7E29105AFE91",@"False"); // Post-Service Survey:Send Email and Complete:Email Staff:Active
            RockMigrationHelper.AddActionTypeAttributeValue("42D286D6-72D3-42D2-9ED4-6B49718BE228","0C4C13B8-7076-4872-925A-F950886B5E16",@"nateh@kingdomfirstsolutions.com"); // Post-Service Survey:Send Email and Complete:Email Staff:Send To Email Addresses|To Attribute
            RockMigrationHelper.AddActionTypeAttributeValue("42D286D6-72D3-42D2-9ED4-6B49718BE228","5D9B13B6-CD96-4C7C-86FA-4512B9D28386",@"Post-Service Survey — {{ Workflow | Attribute:'FirstName' }} {{ Workflow | Attribute:'LastName' }}"); // Post-Service Survey:Send Email and Complete:Email Staff:Subject
            RockMigrationHelper.AddActionTypeAttributeValue("42D286D6-72D3-42D2-9ED4-6B49718BE228","4D245B9E-6B03-46E7-8482-A51FBA190E4D",@"{{ 'Global' | Attribute:'EmailHeader' }}
{%- assign brand = '#DE5A25' -%}
{%- assign ratings = 'Welcome & Hospitality^RateWelcome,Worship & Atmosphere^RateWorship,Sermon / Message^RateSermon,Kids Ministry^RateKids,Clarity of Communication^RateClarity,Overall Service Experience^RateOverall' | Split:',' -%}
    <table width=""600"" cellpadding=""0"" cellspacing=""0"" style=""max-width:600px;background:#ffffff;border-radius:12px;overflow:hidden;"">
      <tr><td style=""background:{{ brand }};padding:20px 28px;"">
        <div style=""color:#ffffff;font-size:20px;font-weight:bold;"">Post-Service Survey</div>
        <div style=""color:#ffe6da;font-size:13px;margin-top:2px;"">Submitted {{ 'Now' | Date:'dddd, MMM d, yyyy • h:mm tt' }}</div>
      </td></tr>
      <tr><td style=""padding:24px 28px 8px;"">
        <div style=""font-size:11px;letter-spacing:.06em;text-transform:uppercase;color:#a1a1aa;font-weight:bold;"">From</div>
        <div style=""font-size:16px;color:#18181b;font-weight:bold;margin-top:4px;"">{{ Workflow | Attribute:'FirstName' }} {{ Workflow | Attribute:'LastName' }}</div>
        <div style=""font-size:14px;color:#52525b;margin-top:2px;"">
          <a href=""mailto:{{ Workflow | Attribute:'Email' }}"" style=""color:{{ brand }};text-decoration:none;"">{{ Workflow | Attribute:'Email' }}</a>
          &nbsp;•&nbsp; {{ Workflow | Attribute:'MobilePhone' }}
        </div>
      </td></tr>
      <tr><td style=""padding:16px 28px 0;"">
        <table width=""100%"" cellpadding=""0"" cellspacing=""0"">
          <tr>
            <td style=""font-size:14px;color:#52525b;padding:6px 0;"">Attended</td>
            <td align=""right"" style=""font-size:14px;color:#18181b;font-weight:bold;"">{{ Workflow | Attribute:'Attendance' | Default:'—' }}</td>
          </tr>
          <tr>
            <td style=""font-size:14px;color:#52525b;padding:6px 0;border-top:1px solid #f4f4f5;"">Sensed God's presence</td>
            <td align=""right"" style=""font-size:14px;color:#18181b;font-weight:bold;border-top:1px solid #f4f4f5;"">{{ Workflow | Attribute:'SensedPresence' | Default:'—' }}</td>
          </tr>
        </table>
      </td></tr>
      <tr><td style=""padding:24px 28px 4px;"">
        <div style=""font-size:11px;letter-spacing:.06em;text-transform:uppercase;color:#a1a1aa;font-weight:bold;"">Ratings</div>
      </td></tr>
      <tr><td style=""padding:0 28px 8px;"">
        <table width=""100%"" cellpadding=""0"" cellspacing=""0"">
          {%- for r in ratings -%}
            {%- assign parts = r | Split:'^' -%}
            {%- assign label = parts | First -%}
            {%- assign key = parts | Last -%}
            {%- assign score = Workflow | Attribute:key | AsInteger -%}
          <tr>
            <td style=""font-size:14px;color:#3f3f46;padding:8px 0;border-top:1px solid #f4f4f5;"">{{ label }}</td>
            <td align=""right"" style=""padding:8px 0;border-top:1px solid #f4f4f5;white-space:nowrap;"">
              {%- if score >= 1 -%}
                {% for i in (1..5) %}<span style=""font-size:16px;color:{% if i <= score %}{{ brand }}{% else %}#d4d4d8{% endif %};"">&#9733;</span>{% endfor %}
                <span style=""font-size:13px;color:#71717a;"">&nbsp;{{ score }}/5</span>
              {%- else -%}
                <span style=""font-size:13px;color:#a1a1aa;"">Not rated</span>
              {%- endif -%}
            </td>
          </tr>
          {%- endfor -%}
        </table>
      </td></tr>
      <tr><td style=""padding:20px 28px 0;"">
        <div style=""font-size:11px;letter-spacing:.06em;text-transform:uppercase;color:#a1a1aa;font-weight:bold;margin-bottom:6px;"">How the service impacted them</div>
        <div style=""font-size:14px;color:#3f3f46;line-height:1.5;background:#fafafa;border-left:3px solid {{ brand }};padding:12px 14px;border-radius:4px;"">{{ Workflow | Attribute:'ServiceImpact' | Default:'—' | NewlineToBr }}</div>
      </td></tr>
      <tr><td style=""padding:16px 28px 28px;"">
        <div style=""font-size:11px;letter-spacing:.06em;text-transform:uppercase;color:#a1a1aa;font-weight:bold;margin-bottom:6px;"">Testimony / notes</div>
        <div style=""font-size:14px;color:#3f3f46;line-height:1.5;background:#fafafa;border-left:3px solid {{ brand }};padding:12px 14px;border-radius:4px;"">{{ Workflow | Attribute:'Testimony' | Default:'—' | NewlineToBr }}</div>
      </td></tr>
    </table>
{{ 'Global' | Attribute:'EmailFooter' }}"); // Post-Service Survey:Send Email and Complete:Email Staff:Body
            RockMigrationHelper.AddActionTypeAttributeValue("42D286D6-72D3-42D2-9ED4-6B49718BE228","1BDC7ACA-9A0B-4C8A-909E-8B4143D9C2A3",@"False"); // Post-Service Survey:Send Email and Complete:Email Staff:Save Communication History
            RockMigrationHelper.AddActionTypeAttributeValue("ECBB69BE-50EB-4DCD-A357-03A38866BB8C","0CA0DDEF-48EF-4ABC-9822-A05E225DE26C",@"False"); // Post-Service Survey:Send Email and Complete:Complete Workflow:Active
            RockMigrationHelper.AddActionTypeAttributeValue("ECBB69BE-50EB-4DCD-A357-03A38866BB8C","385A255B-9F48-4625-862B-26231DBAC53A",@"Completed"); // Post-Service Survey:Send Email and Complete:Complete Workflow:Status|Status Attribute
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

            //
            // Workflow: Add Note (group messages / needs)
            //
            #region FieldTypes
            #endregion
            #region EntityTypes
            RockMigrationHelper.UpdateEntityType("Rock.Model.Workflow", "3540E9A7-FE30-43A9-8B0A-A372B63DFC93", true, true);
            RockMigrationHelper.UpdateEntityType("Rock.Model.WorkflowActivity", "2CB52ED0-CB06-4D62-9E2C-73B60AFA4C9F", true, true);
            RockMigrationHelper.UpdateEntityType("Rock.Model.WorkflowActionType", "23E3273A-B137-48A3-9AFF-C8DC832DDCA6", true, true);
            RockMigrationHelper.UpdateEntityType("Rock.Workflow.Action.CompleteWorkflow","EEDA4318-F014-4A46-9C76-4C052EF81AA1",false,true);
            RockMigrationHelper.UpdateEntityType("Rock.Workflow.Action.PersistWorkflow","F1A39347-6FE0-43D4-89FB-544195088ECF",false,true);
            RockMigrationHelper.UpdateEntityType("Rock.Workflow.Action.RunSQL","A41216D6-6FB0-4019-B222-2C29B4519CF4",false,true);
            RockMigrationHelper.UpdateEntityType("Rock.Workflow.Action.UserEntryForm","486DC4FA-FCBC-425F-90B0-E606DA8A9F68",false,true);
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("486DC4FA-FCBC-425F-90B0-E606DA8A9F68","1EDAFDED-DFE6-4334-B019-6EECBA89E05A","Active","Active","Should Service be used?",0,@"False","234910F2-A0DB-4D7D-BAF7-83C880EF30AE"); // Rock.Workflow.Action.UserEntryForm:Active
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("486DC4FA-FCBC-425F-90B0-E606DA8A9F68","A75DFC58-7A1B-4799-BF31-451B2BBE38FF","Order","Order","The order that this service should be used (priority)",0,@"","C178113D-7C86-4229-8424-C6D0CF4A7E23"); // Rock.Workflow.Action.UserEntryForm:Order
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("A41216D6-6FB0-4019-B222-2C29B4519CF4","1D0D3794-C210-48A8-8C68-3FBEC08A6BA5","SQLQuery","SQLQuery","The SQL query to run. <span class='tip tip-lava'></span>",0,@"","F3B9908B-096F-460B-8320-122CF046D1F9"); // Rock.Workflow.Action.RunSQL:SQLQuery
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("A41216D6-6FB0-4019-B222-2C29B4519CF4","1EDAFDED-DFE6-4334-B019-6EECBA89E05A","Active","Active","Should Service be used?",0,@"False","A18C3143-0586-4565-9F36-E603BC674B4E"); // Rock.Workflow.Action.RunSQL:Active
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("A41216D6-6FB0-4019-B222-2C29B4519CF4","1EDAFDED-DFE6-4334-B019-6EECBA89E05A","Continue On Error","ContinueOnError","Should processing continue even if SQL Error occurs?",3,@"False","9A567F6A-2A77-4ECD-80F7-BBD7D54E843C"); // Rock.Workflow.Action.RunSQL:Continue On Error
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("A41216D6-6FB0-4019-B222-2C29B4519CF4","33E6DF69-BDFA-407A-9744-C175B60643AE","Result Attribute","ResultAttribute","An optional attribute to set to the scaler result of SQL query.",2,@"","56997192-2545-4EA1-B5B2-313B04588984"); // Rock.Workflow.Action.RunSQL:Result Attribute
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("A41216D6-6FB0-4019-B222-2C29B4519CF4","73B02051-0D38-4AD9-BF81-A2D477DE4F70","Parameters","Parameters","The parameters to supply to the SQL query. <span class='tip tip-lava'></span>",1,@"","EA9A026A-934F-4920-97B1-9734795127ED"); // Rock.Workflow.Action.RunSQL:Parameters
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("A41216D6-6FB0-4019-B222-2C29B4519CF4","A75DFC58-7A1B-4799-BF31-451B2BBE38FF","Order","Order","The order that this service should be used (priority)",0,@"","FA7C685D-8636-41EF-9998-90FFF3998F76"); // Rock.Workflow.Action.RunSQL:Order
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("EEDA4318-F014-4A46-9C76-4C052EF81AA1","1EDAFDED-DFE6-4334-B019-6EECBA89E05A","Active","Active","Should Service be used?",0,@"False","0CA0DDEF-48EF-4ABC-9822-A05E225DE26C"); // Rock.Workflow.Action.CompleteWorkflow:Active
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("EEDA4318-F014-4A46-9C76-4C052EF81AA1","3B1D93D7-9414-48F9-80E5-6A3FC8F94C20","Status|Status Attribute","Status","The status to set the workflow to when marking the workflow complete. <span class='tip tip-lava'></span>",0,@"Completed","385A255B-9F48-4625-862B-26231DBAC53A"); // Rock.Workflow.Action.CompleteWorkflow:Status|Status Attribute
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("EEDA4318-F014-4A46-9C76-4C052EF81AA1","A75DFC58-7A1B-4799-BF31-451B2BBE38FF","Order","Order","The order that this service should be used (priority)",0,@"","25CAD4BE-5A00-409D-9BAB-E32518D89956"); // Rock.Workflow.Action.CompleteWorkflow:Order
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("F1A39347-6FE0-43D4-89FB-544195088ECF","1EDAFDED-DFE6-4334-B019-6EECBA89E05A","Active","Active","Should Service be used?",0,@"False","50B01639-4938-40D2-A791-AA0EB4F86847"); // Rock.Workflow.Action.PersistWorkflow:Active
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("F1A39347-6FE0-43D4-89FB-544195088ECF","1EDAFDED-DFE6-4334-B019-6EECBA89E05A","Persist Immediately","PersistImmediately","This action will normally cause the workflow to be persisted (saved) once all the current activities/actions have completed processing. Set this flag to true, if the workflow should be persisted immediately. This is only required if a subsequent action needs a persisted workflow with a valid id.",0,@"False","E22BE348-18B1-4420-83A8-6319B35416D2"); // Rock.Workflow.Action.PersistWorkflow:Persist Immediately
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("F1A39347-6FE0-43D4-89FB-544195088ECF","A75DFC58-7A1B-4799-BF31-451B2BBE38FF","Order","Order","The order that this service should be used (priority)",0,@"","86F795B0-0CB6-4DA4-9CE4-B11D0922F361"); // Rock.Workflow.Action.PersistWorkflow:Order
            #endregion
            #region Categories
            RockMigrationHelper.UpdateCategory("C9F3C4A5-1526-474D-803F-D6C7A45CBBAE","Requests","fa fa-question-circle","","78E38655-D951-41DB-A0FF-D6474775CFA1",0); // Requests
            #endregion
            #region Add Note
            RockMigrationHelper.UpdateWorkflowType(false,true,"Add Note","","78E38655-D951-41DB-A0FF-D6474775CFA1","Note","fa fa-list-ol",28800,false,0,"D7719F5C-96B3-4994-8A6D-0985AC7521E7",0); // Add Note
            RockMigrationHelper.UpdateWorkflowTypeAttribute("D7719F5C-96B3-4994-8A6D-0985AC7521E7","F4399CEF-827B-48B2-A735-F7806FCFE8E8","Group Guid","GroupGuid","",0,@"","D7F3F180-1E7F-428E-AB32-2613FBE96C86", false); // Add Note:Group Guid
            RockMigrationHelper.UpdateWorkflowTypeAttribute("D7719F5C-96B3-4994-8A6D-0985AC7521E7","E3FF88AC-13F6-4DF8-8371-FC0D7FD9A571","Note Type Guid","NoteTypeGuid","",1,@"","B570D107-1D42-4890-A138-B818B2DBD34C", false); // Add Note:Note Type Guid
            RockMigrationHelper.UpdateWorkflowTypeAttribute("D7719F5C-96B3-4994-8A6D-0985AC7521E7","9C204CD0-1233-41C5-818A-C5DA439445AA","Subject","Subject","",2,@"","B4AFB835-8374-4C90-A499-C88107A68D85", false); // Add Note:Subject
            RockMigrationHelper.UpdateWorkflowTypeAttribute("D7719F5C-96B3-4994-8A6D-0985AC7521E7","C28C7BF3-A552-4D77-9408-DEDCF760CED0","Body","Body","",3,@"","8C9051DC-0473-463C-AD24-366DD0EC0C93", false); // Add Note:Body
            RockMigrationHelper.UpdateWorkflowTypeAttribute("D7719F5C-96B3-4994-8A6D-0985AC7521E7","1EDAFDED-DFE6-4334-B019-6EECBA89E05A","Pin to Top","Pin","",4,@"False","7158F312-D035-4051-9834-976FF9D12132", false); // Add Note:Pin to Top
            RockMigrationHelper.AddAttributeQualifier("B570D107-1D42-4890-A138-B818B2DBD34C","entityTypeName",@"Rock.Model.Group","85A47788-24A1-4AD6-A903-ADB9AA19416C"); // Add Note:Note Type Guid:entityTypeName
            RockMigrationHelper.AddAttributeQualifier("B570D107-1D42-4890-A138-B818B2DBD34C","qualifierColumn",@"","42405541-FCDB-4822-81AC-908D059C3417"); // Add Note:Note Type Guid:qualifierColumn
            RockMigrationHelper.AddAttributeQualifier("B570D107-1D42-4890-A138-B818B2DBD34C","qualifierValue",@"","67EC06D2-31AB-4106-9FDC-469D52A8AA0A"); // Add Note:Note Type Guid:qualifierValue
            RockMigrationHelper.AddAttributeQualifier("B4AFB835-8374-4C90-A499-C88107A68D85","ispassword",@"False","26301FFB-CC24-4042-8664-F0B5AEA9B857"); // Add Note:Subject:ispassword
            RockMigrationHelper.AddAttributeQualifier("B4AFB835-8374-4C90-A499-C88107A68D85","maxcharacters",@"","FCC4A069-6AAD-4290-BEF7-FA531B8E0470"); // Add Note:Subject:maxcharacters
            RockMigrationHelper.AddAttributeQualifier("B4AFB835-8374-4C90-A499-C88107A68D85","showcountdown",@"False","E6DC2523-8CC8-47A0-B924-98AE439A4C6D"); // Add Note:Subject:showcountdown
            RockMigrationHelper.AddAttributeQualifier("8C9051DC-0473-463C-AD24-366DD0EC0C93","allowhtml",@"False","48ECF75A-B364-4327-8148-09232BA6974F"); // Add Note:Body:allowhtml
            RockMigrationHelper.AddAttributeQualifier("8C9051DC-0473-463C-AD24-366DD0EC0C93","maxcharacters",@"","E8DF7535-0D20-41E6-9236-31137D8AD90C"); // Add Note:Body:maxcharacters
            RockMigrationHelper.AddAttributeQualifier("8C9051DC-0473-463C-AD24-366DD0EC0C93","numberofrows",@"","96F36683-A61B-4232-B87F-CB6B254C6262"); // Add Note:Body:numberofrows
            RockMigrationHelper.AddAttributeQualifier("8C9051DC-0473-463C-AD24-366DD0EC0C93","showcountdown",@"False","96E64C7B-223B-439F-91B4-4A2DA43A8107"); // Add Note:Body:showcountdown
            RockMigrationHelper.AddAttributeQualifier("7158F312-D035-4051-9834-976FF9D12132","BooleanControlType",@"1","2F435AE6-044C-48F3-8A5B-CABEBFD9C5E0"); // Add Note:Pin to Top:BooleanControlType
            RockMigrationHelper.AddAttributeQualifier("7158F312-D035-4051-9834-976FF9D12132","falsetext",@"No","1AE21EFE-7EDA-496B-85F7-9AF6A19C9EAF"); // Add Note:Pin to Top:falsetext
            RockMigrationHelper.AddAttributeQualifier("7158F312-D035-4051-9834-976FF9D12132","truetext",@"Yes","03E3898F-3AE0-4D8A-A5D7-D3C3289798DD"); // Add Note:Pin to Top:truetext
            RockMigrationHelper.UpdateWorkflowActivityType("D7719F5C-96B3-4994-8A6D-0985AC7521E7",true,"Start","",true,0,"4D8C8846-1828-43A9-A660-496E9E9349FC"); // Add Note:Start
            RockMigrationHelper.UpdateWorkflowActionForm(@"",@"","Submit^fdc397cd-8b4a-436e-bea1-bce2e6717c03^^Message posted!|","",false,"","30AE8256-D1C6-470B-BB1C-293D265863DE"); // Add Note:Start:Display Form
            RockMigrationHelper.UpdateWorkflowActionFormAttribute("30AE8256-D1C6-470B-BB1C-293D265863DE","D7F3F180-1E7F-428E-AB32-2613FBE96C86",0,false,true,false,false,@"",@"","C6155F91-220F-44C4-8ED0-3F6E474BDFE3"); // Add Note:Start:Display Form:Group Guid
            RockMigrationHelper.UpdateWorkflowActionFormAttribute("30AE8256-D1C6-470B-BB1C-293D265863DE","B570D107-1D42-4890-A138-B818B2DBD34C",1,false,true,false,false,@"",@"","DA61FE55-0DF2-49D7-8DDA-BC7D5D46E971"); // Add Note:Start:Display Form:Note Type Guid
            RockMigrationHelper.UpdateWorkflowActionFormAttribute("30AE8256-D1C6-470B-BB1C-293D265863DE","B4AFB835-8374-4C90-A499-C88107A68D85",2,true,false,true,false,@"",@"","EA6355FE-29A5-44E9-8B80-1CCACBAF2EE0"); // Add Note:Start:Display Form:Subject
            RockMigrationHelper.UpdateWorkflowActionFormAttribute("30AE8256-D1C6-470B-BB1C-293D265863DE","8C9051DC-0473-463C-AD24-366DD0EC0C93",3,true,false,true,false,@"",@"","57B2FD4E-806F-467D-8AB2-7431E543BBA3"); // Add Note:Start:Display Form:Body
            RockMigrationHelper.UpdateWorkflowActionFormAttribute("30AE8256-D1C6-470B-BB1C-293D265863DE","7158F312-D035-4051-9834-976FF9D12132",4,true,false,false,false,@"",@"","37483359-5881-400D-A8D2-6705E84B17C5"); // Add Note:Start:Display Form:Pin to Top
            RockMigrationHelper.UpdateWorkflowActionType("4D8C8846-1828-43A9-A660-496E9E9349FC","Display Form",0,"486DC4FA-FCBC-425F-90B0-E606DA8A9F68",true,false,"30AE8256-D1C6-470B-BB1C-293D265863DE","",1,"","BE549FAF-523D-4A35-ADB0-235B9174FCA3"); // Add Note:Start:Display Form
            RockMigrationHelper.UpdateWorkflowActionType("4D8C8846-1828-43A9-A660-496E9E9349FC","Persist",1,"F1A39347-6FE0-43D4-89FB-544195088ECF",true,false,"","",1,"","1E22E8F2-22EE-4943-966B-A88D048D4A62"); // Add Note:Start:Persist
            RockMigrationHelper.UpdateWorkflowActionType("4D8C8846-1828-43A9-A660-496E9E9349FC","Save Note",2,"A41216D6-6FB0-4019-B222-2C29B4519CF4",true,false,"","",1,"","FBC75A8D-52FC-49F6-BAEC-ED58FE57E38D"); // Add Note:Start:Save Note
            RockMigrationHelper.UpdateWorkflowActionType("4D8C8846-1828-43A9-A660-496E9E9349FC","Complete",3,"EEDA4318-F014-4A46-9C76-4C052EF81AA1",true,true,"","",1,"","FF825B85-A0EC-4401-8CB2-1DF12DFA40F8"); // Add Note:Start:Complete
            RockMigrationHelper.AddActionTypeAttributeValue("BE549FAF-523D-4A35-ADB0-235B9174FCA3","234910F2-A0DB-4D7D-BAF7-83C880EF30AE",@"False"); // Add Note:Start:Display Form:Active
            RockMigrationHelper.AddActionTypeAttributeValue("1E22E8F2-22EE-4943-966B-A88D048D4A62","50B01639-4938-40D2-A791-AA0EB4F86847",@"False"); // Add Note:Start:Persist:Active
            RockMigrationHelper.AddActionTypeAttributeValue("1E22E8F2-22EE-4943-966B-A88D048D4A62","E22BE348-18B1-4420-83A8-6319B35416D2",@"True"); // Add Note:Start:Persist:Persist Immediately
            RockMigrationHelper.AddActionTypeAttributeValue("FBC75A8D-52FC-49F6-BAEC-ED58FE57E38D","F3B9908B-096F-460B-8320-122CF046D1F9",@"INSERT INTO [Note]
    (IsSystem, NoteTypeId, EntityId, Caption, [Text], IsAlert,
     IsPrivateNote, ApprovalStatus, CreatedByPersonAliasId, CreatedDateTime, [Guid])
SELECT 0, nt.Id, g.Id, @Subject, @Body,
       CASE WHEN @Pin = 'True' THEN 1 ELSE 0 END,
       0, 1, @AliasId, GETDATE(), NEWID()
FROM [Group] g
CROSS JOIN NoteType nt
WHERE g.[Guid] = @GroupGuid AND nt.[Guid] = @NoteTypeGuid"); // Add Note:Start:Save Note:SQLQuery
            RockMigrationHelper.AddActionTypeAttributeValue("FBC75A8D-52FC-49F6-BAEC-ED58FE57E38D","A18C3143-0586-4565-9F36-E603BC674B4E",@"False"); // Add Note:Start:Save Note:Active
            RockMigrationHelper.AddActionTypeAttributeValue("FBC75A8D-52FC-49F6-BAEC-ED58FE57E38D","EA9A026A-934F-4920-97B1-9734795127ED",@"Subject^{{ Workflow %7C Attribute:'Subject' }}|Body^{{ Workflow %7C Attribute:'Body' }}|Pin^{{ Workflow %7C Attribute:'Pin'%2C'RawValue' }}|GroupGuid^{{ Workflow %7C Attribute:'GroupGuid'%2C'RawValue' }}|NoteTypeGuid^{{ Workflow %7C Attribute:'NoteTypeGuid'%2C'RawValue' }}|AliasId^{{ Workflow.InitiatorPersonAliasId }}"); // Add Note:Start:Save Note:Parameters
            RockMigrationHelper.AddActionTypeAttributeValue("FBC75A8D-52FC-49F6-BAEC-ED58FE57E38D","9A567F6A-2A77-4ECD-80F7-BBD7D54E843C",@"False"); // Add Note:Start:Save Note:Continue On Error
            RockMigrationHelper.AddActionTypeAttributeValue("FF825B85-A0EC-4401-8CB2-1DF12DFA40F8","0CA0DDEF-48EF-4ABC-9822-A05E225DE26C",@"False"); // Add Note:Start:Complete:Active
            RockMigrationHelper.AddActionTypeAttributeValue("FF825B85-A0EC-4401-8CB2-1DF12DFA40F8","385A255B-9F48-4625-862B-26231DBAC53A",@"Completed"); // Add Note:Start:Complete:Status|Status Attribute
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

            //
            // Workflow: Account Deletion Request
            //
            #region FieldTypes
            #endregion
            #region EntityTypes
            RockMigrationHelper.UpdateEntityType("Rock.Model.Workflow", "3540E9A7-FE30-43A9-8B0A-A372B63DFC93", true, true);
            RockMigrationHelper.UpdateEntityType("Rock.Model.WorkflowActivity", "2CB52ED0-CB06-4D62-9E2C-73B60AFA4C9F", true, true);
            RockMigrationHelper.UpdateEntityType("Rock.Model.WorkflowActionType", "23E3273A-B137-48A3-9AFF-C8DC832DDCA6", true, true);
            RockMigrationHelper.UpdateEntityType("Rock.Workflow.Action.AssignActivityFromAttributeValue","F100A31F-E93A-4C7A-9E55-0FAF41A101C4",false,true);
            RockMigrationHelper.UpdateEntityType("Rock.Workflow.Action.CompleteWorkflow","EEDA4318-F014-4A46-9C76-4C052EF81AA1",false,true);
            RockMigrationHelper.UpdateEntityType("Rock.Workflow.Action.PersistWorkflow","F1A39347-6FE0-43D4-89FB-544195088ECF",false,true);
            RockMigrationHelper.UpdateEntityType("Rock.Workflow.Action.SendEmail","66197B01-D1F0-4924-A315-47AD54E030DE",false,true);
            RockMigrationHelper.UpdateEntityType("Rock.Workflow.Action.ShowHtml","FDDAE78D-B7B3-4DA2-9A92-CC129AAF15DE",false,true);
            RockMigrationHelper.UpdateEntityType("Rock.Workflow.Action.UserEntryForm","486DC4FA-FCBC-425F-90B0-E606DA8A9F68",false,true);
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("486DC4FA-FCBC-425F-90B0-E606DA8A9F68","1EDAFDED-DFE6-4334-B019-6EECBA89E05A","Active","Active","Should Service be used?",0,@"False","234910F2-A0DB-4D7D-BAF7-83C880EF30AE"); // Rock.Workflow.Action.UserEntryForm:Active
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("486DC4FA-FCBC-425F-90B0-E606DA8A9F68","A75DFC58-7A1B-4799-BF31-451B2BBE38FF","Order","Order","The order that this service should be used (priority)",0,@"","C178113D-7C86-4229-8424-C6D0CF4A7E23"); // Rock.Workflow.Action.UserEntryForm:Order
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("66197B01-D1F0-4924-A315-47AD54E030DE","1D0D3794-C210-48A8-8C68-3FBEC08A6BA5","Body","Body","The body of the email that should be sent. <span class='tip tip-lava'></span> <span class='tip tip-html'></span>",6,@"","4D245B9E-6B03-46E7-8482-A51FBA190E4D"); // Rock.Workflow.Action.SendEmail:Body
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("66197B01-D1F0-4924-A315-47AD54E030DE","1EDAFDED-DFE6-4334-B019-6EECBA89E05A","Active","Active","Should Service be used?",0,@"False","36197160-7D3D-490D-AB42-7E29105AFE91"); // Rock.Workflow.Action.SendEmail:Active
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("66197B01-D1F0-4924-A315-47AD54E030DE","1EDAFDED-DFE6-4334-B019-6EECBA89E05A","Save Communication History","SaveCommunicationHistory","Should a record of this communication be saved to the recipient's profile?",12,@"False","1BDC7ACA-9A0B-4C8A-909E-8B4143D9C2A3"); // Rock.Workflow.Action.SendEmail:Save Communication History
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("66197B01-D1F0-4924-A315-47AD54E030DE","33E6DF69-BDFA-407A-9744-C175B60643AE","Attachment One","AttachmentOne","Workflow attribute that contains the email attachment. Note file size that can be sent is limited by both the sending and receiving email services typically 10 - 25 MB.",9,@"","C2C7DA55-3018-4645-B9EE-4BCD11855F2C"); // Rock.Workflow.Action.SendEmail:Attachment One
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("66197B01-D1F0-4924-A315-47AD54E030DE","33E6DF69-BDFA-407A-9744-C175B60643AE","Attachment Three","AttachmentThree","Workflow attribute that contains the email attachment. Note file size that can be sent is limited by both the sending and receiving email services typically 10 - 25 MB.",11,@"","A059767A-5592-4926-948A-1065AF4E9748"); // Rock.Workflow.Action.SendEmail:Attachment Three
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("66197B01-D1F0-4924-A315-47AD54E030DE","33E6DF69-BDFA-407A-9744-C175B60643AE","Attachment Two","AttachmentTwo","Workflow attribute that contains the email attachment. Note file size that can be sent is limited by both the sending and receiving email services typically 10 - 25 MB.",10,@"","FFD9193A-451F-40E6-9776-74D5DCAC1450"); // Rock.Workflow.Action.SendEmail:Attachment Two
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("66197B01-D1F0-4924-A315-47AD54E030DE","33E6DF69-BDFA-407A-9744-C175B60643AE","Send to Group Role","GroupRole","An optional Group Role attribute to limit recipients to if the 'Send to Email Addresses' is a group or security role.",4,@"","E3667110-339F-4FE3-B6B7-084CF9633580"); // Rock.Workflow.Action.SendEmail:Send to Group Role
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("66197B01-D1F0-4924-A315-47AD54E030DE","3B1D93D7-9414-48F9-80E5-6A3FC8F94C20","BCC Email Addresses|BCC Attribute","BCC","The email addresses or an attribute that contains the person, email address, group or security role that the email should be BCC'd (blind carbon copied) to. Any address in this field will be copied on the email sent to every recipient. <span class='tip tip-lava'></span>",8,@"","3A131021-CB73-44A8-A142-B42832B77F60"); // Rock.Workflow.Action.SendEmail:BCC Email Addresses|BCC Attribute
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("66197B01-D1F0-4924-A315-47AD54E030DE","3B1D93D7-9414-48F9-80E5-6A3FC8F94C20","CC Email Addresses|CC Attribute","CC","The email addresses or an attribute that contains the person, email address, group or security role that the email should be CC'd (carbon copied) to. Any address in this field will be copied on the email sent to every recipient. <span class='tip tip-lava'></span>",7,@"","99FFD423-2AB6-481B-8749-B4793A16B620"); // Rock.Workflow.Action.SendEmail:CC Email Addresses|CC Attribute
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("66197B01-D1F0-4924-A315-47AD54E030DE","3B1D93D7-9414-48F9-80E5-6A3FC8F94C20","From Email Address|From Attribute","From","The email address or an attribute that contains the person or email address that email should be sent from (will default to organization email). <span class='tip tip-lava'></span>",1,@"","9F5F7CEC-F369-4FDF-802A-99074CE7A7FC"); // Rock.Workflow.Action.SendEmail:From Email Address|From Attribute
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("66197B01-D1F0-4924-A315-47AD54E030DE","3B1D93D7-9414-48F9-80E5-6A3FC8F94C20","From Name|From Name Attribute","FromName","The name or an attribute that contains the person or name that email should be sent from. <span class='tip tip-lava'></span>",0,@"","E37C6F14-0D82-4E29-A735-2276BA94986A"); // Rock.Workflow.Action.SendEmail:From Name|From Name Attribute
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("66197B01-D1F0-4924-A315-47AD54E030DE","3B1D93D7-9414-48F9-80E5-6A3FC8F94C20","Reply To Address|Reply To Attribute","ReplyTo","The email address or an attribute that contains the person or email address that email replies should be sent to (will default to 'From' email). <span class='tip tip-lava'></span>",2,@"","1937BC8A-195E-48E7-9602-B8901D315CF2"); // Rock.Workflow.Action.SendEmail:Reply To Address|Reply To Attribute
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("66197B01-D1F0-4924-A315-47AD54E030DE","3B1D93D7-9414-48F9-80E5-6A3FC8F94C20","Send To Email Addresses|To Attribute","To","The email addresses or an attribute that contains the person, email address, group or security role that the email should be sent to. <span class='tip tip-lava'></span>",3,@"","0C4C13B8-7076-4872-925A-F950886B5E16"); // Rock.Workflow.Action.SendEmail:Send To Email Addresses|To Attribute
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("66197B01-D1F0-4924-A315-47AD54E030DE","9C204CD0-1233-41C5-818A-C5DA439445AA","Subject","Subject","The subject that should be used when sending email. <span class='tip tip-lava'></span>",5,@"","5D9B13B6-CD96-4C7C-86FA-4512B9D28386"); // Rock.Workflow.Action.SendEmail:Subject
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("66197B01-D1F0-4924-A315-47AD54E030DE","A75DFC58-7A1B-4799-BF31-451B2BBE38FF","Order","Order","The order that this service should be used (priority)",0,@"","D1269254-C15A-40BD-B784-ADCC231D3950"); // Rock.Workflow.Action.SendEmail:Order
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("EEDA4318-F014-4A46-9C76-4C052EF81AA1","1EDAFDED-DFE6-4334-B019-6EECBA89E05A","Active","Active","Should Service be used?",0,@"False","0CA0DDEF-48EF-4ABC-9822-A05E225DE26C"); // Rock.Workflow.Action.CompleteWorkflow:Active
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("EEDA4318-F014-4A46-9C76-4C052EF81AA1","3B1D93D7-9414-48F9-80E5-6A3FC8F94C20","Status|Status Attribute","Status","The status to set the workflow to when marking the workflow complete. <span class='tip tip-lava'></span>",0,@"Completed","385A255B-9F48-4625-862B-26231DBAC53A"); // Rock.Workflow.Action.CompleteWorkflow:Status|Status Attribute
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("EEDA4318-F014-4A46-9C76-4C052EF81AA1","A75DFC58-7A1B-4799-BF31-451B2BBE38FF","Order","Order","The order that this service should be used (priority)",0,@"","25CAD4BE-5A00-409D-9BAB-E32518D89956"); // Rock.Workflow.Action.CompleteWorkflow:Order
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("F100A31F-E93A-4C7A-9E55-0FAF41A101C4","1EDAFDED-DFE6-4334-B019-6EECBA89E05A","Active","Active","Should Service be used?",0,@"False","E0F7AB7E-7761-4600-A099-CB14ACDBF6EF"); // Rock.Workflow.Action.AssignActivityFromAttributeValue:Active
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("F100A31F-E93A-4C7A-9E55-0FAF41A101C4","33E6DF69-BDFA-407A-9744-C175B60643AE","Attribute","Attribute","The person or group attribute value to assign this activity to.",0,@"","FBADD25F-D309-4512-8430-3CC8615DD60E"); // Rock.Workflow.Action.AssignActivityFromAttributeValue:Attribute
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("F100A31F-E93A-4C7A-9E55-0FAF41A101C4","A75DFC58-7A1B-4799-BF31-451B2BBE38FF","Order","Order","The order that this service should be used (priority)",0,@"","7A6B605D-7FB1-4F48-AF35-5A0683FB1CDA"); // Rock.Workflow.Action.AssignActivityFromAttributeValue:Order
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("F1A39347-6FE0-43D4-89FB-544195088ECF","1EDAFDED-DFE6-4334-B019-6EECBA89E05A","Active","Active","Should Service be used?",0,@"False","50B01639-4938-40D2-A791-AA0EB4F86847"); // Rock.Workflow.Action.PersistWorkflow:Active
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("F1A39347-6FE0-43D4-89FB-544195088ECF","1EDAFDED-DFE6-4334-B019-6EECBA89E05A","Persist Immediately","PersistImmediately","This action will normally cause the workflow to be persisted (saved) once all the current activities/actions have completed processing. Set this flag to true, if the workflow should be persisted immediately. This is only required if a subsequent action needs a persisted workflow with a valid id.",0,@"False","E22BE348-18B1-4420-83A8-6319B35416D2"); // Rock.Workflow.Action.PersistWorkflow:Persist Immediately
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("F1A39347-6FE0-43D4-89FB-544195088ECF","A75DFC58-7A1B-4799-BF31-451B2BBE38FF","Order","Order","The order that this service should be used (priority)",0,@"","86F795B0-0CB6-4DA4-9CE4-B11D0922F361"); // Rock.Workflow.Action.PersistWorkflow:Order
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("FDDAE78D-B7B3-4DA2-9A92-CC129AAF15DE","1D0D3794-C210-48A8-8C68-3FBEC08A6BA5","HTML","HTML","The HTML to show. <span class='tip tip-lava'></span>",0,@"","B3E08E2D-7CD3-42C8-A3AD-F60BFD07CFC0"); // Rock.Workflow.Action.ShowHtml:HTML
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("FDDAE78D-B7B3-4DA2-9A92-CC129AAF15DE","1EDAFDED-DFE6-4334-B019-6EECBA89E05A","Active","Active","Should Service be used?",0,@"False","666EAEA1-27B6-41AF-9896-4F8DDE87E2ED"); // Rock.Workflow.Action.ShowHtml:Active
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("FDDAE78D-B7B3-4DA2-9A92-CC129AAF15DE","1EDAFDED-DFE6-4334-B019-6EECBA89E05A","Hide Status Message","HideStatusMessage","Whether or not to hide the built-in status message.",1,@"False","7C6F75BE-B58C-4835-9B1D-2A0F496878F9"); // Rock.Workflow.Action.ShowHtml:Hide Status Message
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("FDDAE78D-B7B3-4DA2-9A92-CC129AAF15DE","A75DFC58-7A1B-4799-BF31-451B2BBE38FF","Order","Order","The order that this service should be used (priority)",0,@"","2B14EC82-B9A8-43B1-9298-4A8FF0FAF378"); // Rock.Workflow.Action.ShowHtml:Order
            #endregion
            #region Categories
            RockMigrationHelper.UpdateCategory("C9F3C4A5-1526-474D-803F-D6C7A45CBBAE","Requests","fa fa-question-circle","","78E38655-D951-41DB-A0FF-D6474775CFA1",0); // Requests
            #endregion
            #region Account Deletion Request
            RockMigrationHelper.UpdateWorkflowType(false,true,"Account Deletion Request","","78E38655-D951-41DB-A0FF-D6474775CFA1","Work","fa fa-user-slash",28800,false,0,"35A494CC-15DB-46A3-B28B-571D16DDDFF1",0); // Account Deletion Request
            RockMigrationHelper.UpdateWorkflowTypeAttribute("35A494CC-15DB-46A3-B28B-571D16DDDFF1","7525C4CB-EE6B-41D4-9B64-A08048D5A5C0","I understand my account will be permanently deleted.","Confirmed","",0,@"","C4686C9E-A904-4645-A0E0-FC5ED6F99314", false); // Account Deletion Request:I understand my account will be permanently deleted.
            RockMigrationHelper.UpdateWorkflowTypeAttribute("35A494CC-15DB-46A3-B28B-571D16DDDFF1","F4399CEF-827B-48B2-A735-F7806FCFE8E8","Group to Notify","GrouptoNotify","Select the group you would like to notify about this deletion request.",1,@"5b6be24f-349b-4630-b99c-20bb62bc8bd2","57125EE2-87C6-42EC-8B0C-CE35B26F09DE", false); // Account Deletion Request:Group to Notify
            RockMigrationHelper.AddAttributeQualifier("C4686C9E-A904-4645-A0E0-FC5ED6F99314","fieldtype",@"ddl","E52AB783-C4B5-444C-A170-8543BBA84157"); // Account Deletion Request:I understand my account will be permanently deleted.:fieldtype
            RockMigrationHelper.AddAttributeQualifier("C4686C9E-A904-4645-A0E0-FC5ED6F99314","repeatColumns",@"","8A6B540F-FDE3-4D89-9485-91E155492571"); // Account Deletion Request:I understand my account will be permanently deleted.:repeatColumns
            RockMigrationHelper.AddAttributeQualifier("C4686C9E-A904-4645-A0E0-FC5ED6F99314","values",@"Yes","2A657F51-1E2F-4A00-9636-2F0FCC8B2EC9"); // Account Deletion Request:I understand my account will be permanently deleted.:values
            RockMigrationHelper.UpdateWorkflowActivityType("35A494CC-15DB-46A3-B28B-571D16DDDFF1",true,"Confirm Request","",true,0,"B5AC62F8-6EC8-48B8-9B1E-C6429650BE20"); // Account Deletion Request:Confirm Request
            RockMigrationHelper.UpdateWorkflowActivityType("35A494CC-15DB-46A3-B28B-571D16DDDFF1",true,"Notify of Request","",false,1,"58DEA9EA-B6EF-42A2-8433-F3DFD7218C35"); // Account Deletion Request:Notify of Request
            RockMigrationHelper.UpdateWorkflowActionForm(@"<h3>Delete Your Account</h3>
<p>This submits a request to permanently delete your {{ 'Global' | Attribute:'OrganizationName' }} account and profile. A staff member will process it, and you'll lose access to your giving history, saved items, and profile. This can't be undone once processed.</p>",@"","Submit^9b329020-e074-4326-8831-9dd534f491df^58DEA9EA-B6EF-42A2-8433-F3DFD7218C35^Your request has been submitted — we''ll process it within 5 business days|","",false,"","D25883C1-0C3D-4AAF-B541-6777ED4B9287"); // Account Deletion Request:Confirm Request:User Entry Form
            RockMigrationHelper.UpdateWorkflowActionForm(@"<p>{{ Workflow.InitiatorPersonAlias.Person.FullName }} (Email: {{ Workflow.InitiatorPersonAlias.Person.Email }}, Person ID: {{ Workflow.InitiatorPersonAlias.Person.Id }}) has requested that their account be deleted via the mobile app.</p>
<p>Please review and process the deletion (handle any giving statements / family merges first).</p>",@"","Complete Workflow^fdc397cd-8b4a-436e-bea1-bce2e6717c03^^Your information has been submitted successfully.|","",false,"","D4949E01-5281-43F6-9A00-88AFC9CF76D2"); // Account Deletion Request:Notify of Request:Confirm Complete
            RockMigrationHelper.UpdateWorkflowActionFormAttribute("D25883C1-0C3D-4AAF-B541-6777ED4B9287","C4686C9E-A904-4645-A0E0-FC5ED6F99314",0,true,false,true,false,@"",@"","0CFB486E-A9E0-4D24-A679-E57DC97ACA93"); // Account Deletion Request:Confirm Request:User Entry Form:I understand my account will be permanently deleted.
            RockMigrationHelper.UpdateWorkflowActionFormAttribute("D25883C1-0C3D-4AAF-B541-6777ED4B9287","57125EE2-87C6-42EC-8B0C-CE35B26F09DE",1,false,true,false,false,@"",@"","7BEF4DAE-844B-4AC2-8DE0-FAAB6A7B2C63"); // Account Deletion Request:Confirm Request:User Entry Form:Group to Notify
            RockMigrationHelper.UpdateWorkflowActionFormAttribute("D4949E01-5281-43F6-9A00-88AFC9CF76D2","C4686C9E-A904-4645-A0E0-FC5ED6F99314",1,false,true,false,false,@"",@"","956719F1-6434-47F8-9C2B-5D039541A80B"); // Account Deletion Request:Notify of Request:Confirm Complete:I understand my account will be permanently deleted.
            RockMigrationHelper.UpdateWorkflowActionFormAttribute("D4949E01-5281-43F6-9A00-88AFC9CF76D2","57125EE2-87C6-42EC-8B0C-CE35B26F09DE",0,false,true,false,false,@"",@"","D9B8FBD2-FEBF-46E9-B53B-0241E585E458"); // Account Deletion Request:Notify of Request:Confirm Complete:Group to Notify
            RockMigrationHelper.UpdateWorkflowActionType("B5AC62F8-6EC8-48B8-9B1E-C6429650BE20","User Entry Form",0,"486DC4FA-FCBC-425F-90B0-E606DA8A9F68",true,true,"D25883C1-0C3D-4AAF-B541-6777ED4B9287","",1,"","C66A1D4B-83AD-43C6-929E-DA6A59101224"); // Account Deletion Request:Confirm Request:User Entry Form
            RockMigrationHelper.UpdateWorkflowActionType("58DEA9EA-B6EF-42A2-8433-F3DFD7218C35","Confirmed = no",0,"FDDAE78D-B7B3-4DA2-9A92-CC129AAF15DE",true,false,"","C4686C9E-A904-4645-A0E0-FC5ED6F99314",2,"Yes","D935EDBA-E66C-476F-AD55-C4097BD9B267"); // Account Deletion Request:Notify of Request:Confirmed = no
            RockMigrationHelper.UpdateWorkflowActionType("58DEA9EA-B6EF-42A2-8433-F3DFD7218C35","Ignore and complete if no confirmation",1,"EEDA4318-F014-4A46-9C76-4C052EF81AA1",true,true,"","C4686C9E-A904-4645-A0E0-FC5ED6F99314",2,"Yes","1629A23F-716C-4A4B-82B2-94A04DB82280"); // Account Deletion Request:Notify of Request:Ignore and complete if no confirmation
            RockMigrationHelper.UpdateWorkflowActionType("58DEA9EA-B6EF-42A2-8433-F3DFD7218C35","Persist",2,"F1A39347-6FE0-43D4-89FB-544195088ECF",true,false,"","",1,"","E8211F05-343E-4A81-9265-E82FC02B2172"); // Account Deletion Request:Notify of Request:Persist
            RockMigrationHelper.UpdateWorkflowActionType("58DEA9EA-B6EF-42A2-8433-F3DFD7218C35","Notify Group",3,"66197B01-D1F0-4924-A315-47AD54E030DE",true,false,"","",1,"","09C1EA55-C2C4-480F-A860-F68E2F7449E3"); // Account Deletion Request:Notify of Request:Notify Group
            RockMigrationHelper.UpdateWorkflowActionType("58DEA9EA-B6EF-42A2-8433-F3DFD7218C35","Assign activity",4,"F100A31F-E93A-4C7A-9E55-0FAF41A101C4",true,false,"","",1,"","1B6B386D-420C-486D-BECA-50918916F626"); // Account Deletion Request:Notify of Request:Assign activity
            RockMigrationHelper.UpdateWorkflowActionType("58DEA9EA-B6EF-42A2-8433-F3DFD7218C35","Confirm Complete",5,"486DC4FA-FCBC-425F-90B0-E606DA8A9F68",true,false,"D4949E01-5281-43F6-9A00-88AFC9CF76D2","",1,"","972F995E-9AA8-4FDC-8678-D9009A83E7A0"); // Account Deletion Request:Notify of Request:Confirm Complete
            RockMigrationHelper.UpdateWorkflowActionType("58DEA9EA-B6EF-42A2-8433-F3DFD7218C35","Complete workflow",6,"EEDA4318-F014-4A46-9C76-4C052EF81AA1",true,true,"","",1,"","4A831249-E9C1-49C4-8246-46A0B06BD29B"); // Account Deletion Request:Notify of Request:Complete workflow
            RockMigrationHelper.AddActionTypeAttributeValue("C66A1D4B-83AD-43C6-929E-DA6A59101224","234910F2-A0DB-4D7D-BAF7-83C880EF30AE",@"False"); // Account Deletion Request:Confirm Request:User Entry Form:Active
            RockMigrationHelper.AddActionTypeAttributeValue("D935EDBA-E66C-476F-AD55-C4097BD9B267","B3E08E2D-7CD3-42C8-A3AD-F60BFD07CFC0",@"<div class=""alert alert-danger""><p>You must agree that you understand everything will be deleted to continue.</p></div>"); // Account Deletion Request:Notify of Request:Confirmed = no:HTML
            RockMigrationHelper.AddActionTypeAttributeValue("D935EDBA-E66C-476F-AD55-C4097BD9B267","666EAEA1-27B6-41AF-9896-4F8DDE87E2ED",@"False"); // Account Deletion Request:Notify of Request:Confirmed = no:Active
            RockMigrationHelper.AddActionTypeAttributeValue("D935EDBA-E66C-476F-AD55-C4097BD9B267","7C6F75BE-B58C-4835-9B1D-2A0F496878F9",@"True"); // Account Deletion Request:Notify of Request:Confirmed = no:Hide Status Message
            RockMigrationHelper.AddActionTypeAttributeValue("1629A23F-716C-4A4B-82B2-94A04DB82280","0CA0DDEF-48EF-4ABC-9822-A05E225DE26C",@"False"); // Account Deletion Request:Notify of Request:Ignore and complete if no confirmation:Active
            RockMigrationHelper.AddActionTypeAttributeValue("1629A23F-716C-4A4B-82B2-94A04DB82280","385A255B-9F48-4625-862B-26231DBAC53A",@"Completed"); // Account Deletion Request:Notify of Request:Ignore and complete if no confirmation:Status|Status Attribute
            RockMigrationHelper.AddActionTypeAttributeValue("E8211F05-343E-4A81-9265-E82FC02B2172","50B01639-4938-40D2-A791-AA0EB4F86847",@"False"); // Account Deletion Request:Notify of Request:Persist:Active
            RockMigrationHelper.AddActionTypeAttributeValue("E8211F05-343E-4A81-9265-E82FC02B2172","E22BE348-18B1-4420-83A8-6319B35416D2",@"False"); // Account Deletion Request:Notify of Request:Persist:Persist Immediately
            RockMigrationHelper.AddActionTypeAttributeValue("09C1EA55-C2C4-480F-A860-F68E2F7449E3","36197160-7D3D-490D-AB42-7E29105AFE91",@"False"); // Account Deletion Request:Notify of Request:Notify Group:Active
            RockMigrationHelper.AddActionTypeAttributeValue("09C1EA55-C2C4-480F-A860-F68E2F7449E3","0C4C13B8-7076-4872-925A-F950886B5E16",@"57125ee2-87c6-42ec-8b0c-ce35b26f09de"); // Account Deletion Request:Notify of Request:Notify Group:Send To Email Addresses|To Attribute
            RockMigrationHelper.AddActionTypeAttributeValue("09C1EA55-C2C4-480F-A860-F68E2F7449E3","5D9B13B6-CD96-4C7C-86FA-4512B9D28386",@"Account Deletion Request — {{ Workflow.InitiatorPersonAlias.Person.FullName }}"); // Account Deletion Request:Notify of Request:Notify Group:Subject
            RockMigrationHelper.AddActionTypeAttributeValue("09C1EA55-C2C4-480F-A860-F68E2F7449E3","4D245B9E-6B03-46E7-8482-A51FBA190E4D",@"{{ 'Global' | Attribute:'EmailHeader' }}
<p>{{ Workflow.InitiatorPersonAlias.Person.FullName }} (Email: {{ Workflow.InitiatorPersonAlias.Person.Email }}, Person ID: {{ Workflow.InitiatorPersonAlias.Person.Id }}) has requested that their account be deleted via the mobile app.</p>
<p>Please review and process the deletion (handle any giving statements / family merges first).</p>
{{ 'Global' | Attribute:'EmailFooter' }}"); // Account Deletion Request:Notify of Request:Notify Group:Body
            RockMigrationHelper.AddActionTypeAttributeValue("09C1EA55-C2C4-480F-A860-F68E2F7449E3","1BDC7ACA-9A0B-4C8A-909E-8B4143D9C2A3",@"False"); // Account Deletion Request:Notify of Request:Notify Group:Save Communication History
            RockMigrationHelper.AddActionTypeAttributeValue("1B6B386D-420C-486D-BECA-50918916F626","E0F7AB7E-7761-4600-A099-CB14ACDBF6EF",@"False"); // Account Deletion Request:Notify of Request:Assign activity:Active
            RockMigrationHelper.AddActionTypeAttributeValue("1B6B386D-420C-486D-BECA-50918916F626","FBADD25F-D309-4512-8430-3CC8615DD60E",@"57125ee2-87c6-42ec-8b0c-ce35b26f09de"); // Account Deletion Request:Notify of Request:Assign activity:Attribute
            RockMigrationHelper.AddActionTypeAttributeValue("972F995E-9AA8-4FDC-8678-D9009A83E7A0","234910F2-A0DB-4D7D-BAF7-83C880EF30AE",@"False"); // Account Deletion Request:Notify of Request:Confirm Complete:Active
            RockMigrationHelper.AddActionTypeAttributeValue("4A831249-E9C1-49C4-8246-46A0B06BD29B","0CA0DDEF-48EF-4ABC-9822-A05E225DE26C",@"False"); // Account Deletion Request:Notify of Request:Complete workflow:Active
            RockMigrationHelper.AddActionTypeAttributeValue("4A831249-E9C1-49C4-8246-46A0B06BD29B","385A255B-9F48-4625-862B-26231DBAC53A",@"Completed"); // Account Deletion Request:Notify of Request:Complete workflow:Status|Status Attribute
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

            //
            // Site logo binary file (AppHeader3.png) + wire it to the mobile site
            //
            Sql( @"DECLARE
	@BinaryFileId int
	,@BinaryFileTypeIdDefault int = (SELECT TOP 1 Id from [BinaryFileType] where [Guid] = 'C1142570-8CD6-4A20-83B1-ACB47C1CD377')
	,@StorageEntityTypeIdDatabase int = (SELECT TOP 1 Id FROM [EntityType] WHERE [Guid] = '0AA42802-04FD-4AEC-B011-FEB127FC85CD')

-- Add AppHeader3.png
IF NOT EXISTS (SELECT * FROM [BinaryFile] WHERE [Guid] = 'BC08E46C-5493-48F7-BDDA-0DC305ED0836' )
BEGIN
INSERT INTO [BinaryFile] ([IsTemporary], [IsSystem], [BinaryFileTypeId], [FileName], [MimeType], [StorageEntityTypeId], [Guid])
			VALUES (0,0, @BinaryFileTypeIdDefault, 'AppHeader3.png', 'image/png', @StorageEntityTypeIdDatabase, 'BC08E46C-5493-48F7-BDDA-0DC305ED0836')

SET @BinaryFileId = SCOPE_IDENTITY()

INSERT INTO [BinaryFileData] ([Id], [Content], [Guid])
  VALUES ( 
    @BinaryFileId
    ,0x89504E470D0A1A0A0000000D4948445200000077000000760806000000F2650C2F0000000467414D410000B18F0BFC6105000000097048597300000B1200000B1201D2DD7EFC000009CA49444154785EED9D7BAC5D4515C61145C4173E107C248A20C2D298B65040482D043556C547B4AE3F844A456909582050092D1692269848894683BC22260846541A6914048322E5E145C442A1BCA38864DD0B45DAA2F4DE16E836DFB9739B9E75E69CEE73F69ED93373E64B7E4993B3EF7EACAF679FD96BD6ACBDCB2E914B985E234C7B0BD34784E9F3C274AA305D204C5708D375C2F40761BA43984684E95E61FAABF9F7ADC2F45B61BA5A987E204CE708D33C619A2D4CFB0AD31BF5B1B21C4B98F610A60F0BD371C2749131EF29617A45988A9AF89F30AD31C62F16A6A384691F617A953E9FAC0A424085692F61FA9430AD10A6BF08D3568B21AE795A987E214C270BD38784E9B5FA5CB34A0AB745613A46987E2E4CFFB104BB49F09FEB7E615A244CEFCFDFE89212A6F79A5BE1BF2C410D91CDC2F41B61FAB830BD4E5FCFD0CBDC7AA70BD32AF37BA7031803DB84E92161FA9630EDA9AF71E8644C9D264C3708D3842560B1F2A4309D2E4CAFD7D73C1412A619668012EB37B50C8F996FF2700CBE84E92DC2B45C989EB3042345F088F667613A5AC72219995BF027CDF3237E9F741052E74561BA5098DEAA6313B584E9CDE61935E55B7059FE2E4C1FD3318A5266C08451A4BEC8610683C7EF08D3EE3A5E5148987615A6E38569A3E5E23293DC88EC9B8E5DD032C65E66B9984C27486B4ED7310C5278B613A69B2D1791E90EC6225FD0B10C4A26C1BFCE72F2999D837CF5A93AA6414898DE234C8F5B4E3A539E97856949501311C2748030FDD372B299FE41D2639930EDA6E3EC5DC2F43E617AD0729299C1C12DFADBA832D1F1F626539D80B2157D7299EAE05978A18EB91709D39B84E936CB4965EA03067F46C7DEA94C515A7E8EF5C3F3A819D31E3893309D614676FA44326E78D84B35266E13C2F45FCB0964DC82529E5DB51FB509D355C2F4A8E5C0193F2CD29ED422617AB5305D6B3960C61F28C69BA1BDA92C538DFF92E58019BFACADB5CAD22CD74055BF3E50A619CED01E0D24531E7389E50095195B30ABD870F1D9C5C6CBCE4D9AF54BBFD271ED15D9224C1FD05EF52D619A6976A60F5099F1BB6E2C86422F6F2DC64E3CBCE3FA2BB2527BD59750926956C4E91DD7C2E65B57EA3024AB674F9BD371FD35F069ED596909D3E75C562A8ECE3FB498587BA78E43927264EE1FF114A37DDBA984E90D665DABDE61AD8CCE9F594C3C90BEC18ECC4596F078EDDD4E254C732D3B73C23018ECC85C70675F5383788E328B9AF58E9C0183B73E719F8E493272682E720F5FD61E769530CDA979A57A2952FE0D76682EF87DA9D21C936644BB00BD032FA46AB06373513DF941ED658784E9A0A60BC961F09687EFD1F1895A8ECD05DFD75E76C87475D17FE89DD1AF1D5C6C5937A26314AD3C988B4E04BDD7039B8622FA0F1BA165F04377EB3845290FE6221F3147FBB95DC857363190EAC5D8378E2826D6DCA663159D3C980BBAA72485E97CCB1F344E0A067B325750B8A87D6D4998EEB2FC4110C46EB02773716B3E52FB0A6351421374D391980DF6642E384F7B0B73BF6AD9303860708C832C8FE6A2BF65FB64826984A9370C92181F933C9A8BCAD4776973EFB16C182CB119ECD15C70D48EC622E5886E2B7AA3A069191C4926CBB3B9A7EF682E9E6FF50651104B2EDAB3B9D7ED68EEB1960DA22106833D9BFBC4F6395E0C9F2D1B4445E813FE9ECDDD204CEF9832F7D7960DA22364833D9B8B14F20153E626B380BA657080B768CFE682C92E75A9F5B308F137B80173E74E99FB82E5C3A8096DC2BF0173174C3DE3EA0F9220A4444703E62E85B9E86DA13F4886500C6EC0DCEFC1DCB75B3E488AD17907373E9BD480B997C05C747ED31F2447D3D385D95CC734697053E6267F5BDE91A60C6EC0DC1F273FA0B2D184C10D98DB1A50E151C8D932CD50F15DD1D180B94B924D6294C1E7635203E62E4832FDD80FBE26FC1B30777BFAF16ECB8743838F5C7403E6CE9A32F757960F870AD7067B3617537E93DD6ECC3B6EF4064307A60BC7FF768BF6A5167936179D5E275F69D35499CDFA25738BCDB7AF2AC6476EAAC48B375D5D8C9DF8D18EFD0F82AB097FCFE6B695D9EC67D9C039E32337EB180CAC8D972FEBD8FFA0B8F8067B36F7972D638DB9688EEDBDB47562CD6A1D8381B5E9A71774ECBF0A7557747836F7B4EDE61A83BD97DA846C2EA87390E5D9DCD9DADCCB2D1B39257473415D151D1ECDB52E27F1BE102C0673411D992C8FE6E215AEED1DD5CD3B6E9D34F0EC462CE682AA2D1C3C9ABBACCDD82909D36ACBC6CE88C95C506536C993B998003A5CFBDA92EFB609B1990B0635D893B93DDB26ECEBB3D57D8CE602188CE4493FF264EEB5DAD336F96C5514ABB9A0DF6FB00773714BEEDD7FD96793B198CD05FD18ECC1DC27D14659FBD926613AD0D7E47DECE60294CD96794CF2606EA9F68078618597C69E29980BCA3C073B3617A9E3FDB5975609D3313E3AC9A5622E183D6E5ACF54A563737FA73DEC2A61DADD4733ED94CC05BD72D10ECD452BFC2F690F7B0ADDB72D3BAA95D4CC05DD0C76686E7F6DF02161DA4398FE64D9596DA4682EB04DF83B32173909D6DE9592EB0A8DCDABAF6F0B40156DF8D1E28EFD37499BC1DBB6156327CDEAD8A606F0EA99DDB46FA5645E1A851DE89DD6C2330B67179BAE5C5EBC70CD8A4A6CF8E159AD018DDE7FD38C9E30B3D874D5778BE72F3CA5E3B31A40D2E213DAB3BE244C8709D3B865E799665959EAA515BD84B9412C2AB2EC3CD31C98903F507B35905026294C4F5B0E926986F61AA9AAC26BC57CCE1865BAB26EE041543799DBF3CF2C07CBF803639F7A6EC75AA69BFAA3968366FC708AF6A45699BC33962BE80367DC82BB667BE19B0BA19FAF306DB59C40C60D0F0AD39EDA0727C20FBAABF7D8673A785698A66B0F9C0AFF935C66AF322D304F7BAC8EBD1709D3DB84E93ECB4965AA839FBD9374CCBD4A98DE9D47D0B58339DAB32AA717EB9030ED2D4C4F594E32D33F981080B1EE47C665254CFB08D33F2C279B290F4A9B160765EC944C9223AAF7130504D669CD0BE256DC4D6641D90D9693CF7407A5C45FD4B10C5266A5FE8AD05FFC1808F70BD3213A8641CBB41DFCBA308D5A2E283339225E254CEFD4B18B46C274A87933A4BEB86166A35951D9FB1DF3310817214C17E5729D16988F3D4CC7286A99652A479A8BD3173C0C60FC81FFE0BD176BC52C5C9C302D37AF24D30148112425468469A68E45B2328DCDAE4CFC568D4E6E73FB5E119082CCAD7A9A1935A6343FFC6F61FA66D71606C324539F3543982E3523491DAC18C0EDF776619A8FD9327D8D432FF34DC68B9AB1BAFF014B00430435C4D708D367931E2CD529530C8080FD24C09A69E481D1D6E94C613A28C8447F2C32D9AEA385E962531C80EC8E0EB86B9E13A6EB856951D459A5D065A6174F30CB5CF03BB7DE62461530B87B04EF7E474736B35E2ADCD99AD4654A7E0E318BC6970AD3156827204C7798F4E75A63182A0991ACC7B3E72DE8DD8477F008D34253B6BB7F2A8F2EFF076091D9AE53989A340000000049454E44AE426082
    ,'4F730BB7-543B-41C6-9FA5-B514E0F705BF'
    )
END
" );

            Sql( @"
                DECLARE @LogoId INT = ( SELECT TOP 1 [Id] FROM [BinaryFile] WHERE [Guid] = 'BC08E46C-5493-48F7-BDDA-0DC305ED0836' );
                IF @LogoId IS NOT NULL
                    UPDATE [Site] SET [SiteLogoBinaryFileId] = @LogoId
                    WHERE [Guid] = ( SELECT [Guid] FROM [Site] WHERE [SiteType] = 1 AND [Name] = 'Nfluence Church App' );
                " );
        }

        public override void Down()
        {
            // Intentionally left blank.
        }
    }
}