// <copyright>
// Copyright 2019 by Kingdom First Solutions
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
using Rock.SystemGuid;

namespace rocks.kfs.MinistrySafe.Migrations
{
    [MigrationNumber( 1, "1.6.9" )]
    public class AddSystemData : Migration
    {
        /// <summary>
        /// The commands to run to migrate plugin to the specific version
        /// </summary>
        public override void Up()
        {
            // Global Attributes
            RockMigrationHelper.AddGlobalAttribute( FieldType.ENCRYPTED_TEXT, "", "", "Ministry Safe API Key", "", 0, "", "E246DBBA-3092-4B26-A3A0-292AD3F68C33" );

            // Training Types
            RockMigrationHelper.AddDefinedType( "Global", "Ministry Safe Training Types", "The type of Ministry Safe Trainings available.", SystemGuid.DefinedType.MINISTRY_SAFE_TRAINING_TYPES );
            RockMigrationHelper.UpdateDefinedValue( SystemGuid.DefinedType.MINISTRY_SAFE_TRAINING_TYPES, "standard", "Standard Sexual Abuse Awareness Training", "16A04162-2396-4A64-937D-E222ADA2CBB4", true );
            RockMigrationHelper.UpdateDefinedValue( SystemGuid.DefinedType.MINISTRY_SAFE_TRAINING_TYPES, "youth", "Youth Sports Sexual Abuse Awareness Training", "F422B142-7621-4FF4-B143-FA6C9833824C", true );
            RockMigrationHelper.UpdateDefinedValue( SystemGuid.DefinedType.MINISTRY_SAFE_TRAINING_TYPES, "camp", "Camp-Focused Sexual Abuse Awareness Training", "41946C59-A7FC-4FDD-AF8F-A68F1A0F4C34", true );
            RockMigrationHelper.UpdateDefinedValue( SystemGuid.DefinedType.MINISTRY_SAFE_TRAINING_TYPES, "spanish", "Spanish Sexual Abuse Awareness Training", "B3F7014A-2433-4889-BA9B-ECB7701A99C5", true );

            // Person Attribute Category
            RockMigrationHelper.UpdatePersonAttributeCategory( "Ministry Safe", "fa fa-clipboard", "", SystemGuid.Category.MINISTRY_SAFE_PERSON_ATTRIBUTE_CATEGORY );

            // Person Attribute Training Type
            RockMigrationHelper.UpdatePersonAttribute( FieldType.DEFINED_VALUE, SystemGuid.Category.MINISTRY_SAFE_PERSON_ATTRIBUTE_CATEGORY, "Ministry Safe Training Type", "MSTrainingType", "", "The Ministry Safe training type that this person completed.", 0, "", SystemGuid.Attribute.MINISTRY_SAFE_PERSON_ATTRIBUTE_TEST_TYPE );
            RockMigrationHelper.UpdateAttributeQualifier( SystemGuid.Attribute.MINISTRY_SAFE_PERSON_ATTRIBUTE_TEST_TYPE, "allowmultiple", "False", "13C4E727-21E2-4A7D-968A-2BA55B5AA567" );
            RockMigrationHelper.UpdateAttributeQualifier( SystemGuid.Attribute.MINISTRY_SAFE_PERSON_ATTRIBUTE_TEST_TYPE, "definedtype", "", "110572F2-A7EA-40C3-8C93-732313CEACBC" );
            RockMigrationHelper.UpdateAttributeQualifier( SystemGuid.Attribute.MINISTRY_SAFE_PERSON_ATTRIBUTE_TEST_TYPE, "displaydescription", "True", "61DF6B3C-06B6-488D-ADCC-769A46FBE1EA" );

            // Person Attribute Test Date
            RockMigrationHelper.UpdatePersonAttribute( FieldType.DATE, SystemGuid.Category.MINISTRY_SAFE_PERSON_ATTRIBUTE_CATEGORY, "Ministry Safe Training Date", "MSTrainingDate", "", "Date the user took the Ministry Safe training.", 1, "", SystemGuid.Attribute.MINISTRY_SAFE_PERSON_ATTRIBUTE_TEST_DATE );
            RockMigrationHelper.UpdateAttributeQualifier( SystemGuid.Attribute.MINISTRY_SAFE_PERSON_ATTRIBUTE_TEST_DATE, "displayDiff", "False", "133DEB74-6412-4DC6-AE3C-EB2E36BD9AC7" );
            RockMigrationHelper.UpdateAttributeQualifier( SystemGuid.Attribute.MINISTRY_SAFE_PERSON_ATTRIBUTE_TEST_DATE, "format", "", "652B01D4-DF27-4DA8-9D28-9BFFDC3D543C" );

            // Person Attribute Score
            RockMigrationHelper.UpdatePersonAttribute( FieldType.INTEGER, SystemGuid.Category.MINISTRY_SAFE_PERSON_ATTRIBUTE_CATEGORY, "Ministry Safe Training Score", "MSTrainingScore", "", "The user's score from the Ministry Safe training.", 2, "", SystemGuid.Attribute.MINISTRY_SAFE_PERSON_ATTRIBUTE_TEST_SCORE );

            // Set the Test Type Defined Type Id as the Qualifer Value for the Person Attribute
            Sql( @"
                DECLARE @DefinedTypeId INT = ( SELECT Id FROM [DefinedType] WHERE [Guid] = '56DA549B-EE02-4F6C-9CE4-60C2270751F2' )

                UPDATE [AttributeQualifier]
                SET [Value] = @DefinedTypeId
                WHERE [Guid] = '110572F2-A7EA-40C3-8C93-732313CEACBC'
            " );

            // Put person attribute block on Extended Attributes page
            RockMigrationHelper.AddBlock( true, "1C737278-4CBA-404B-B6B3-E3F0E05AB5FE", "", "D70A59DC-16BE-43BE-9880-59598FA7A94C", "Ministry Safe", "SectionB2", "", "", 0, "63FF2A72-B3B7-4CD9-A652-91B8230773A6" );
            RockMigrationHelper.AddBlockAttributeValue( true, "63FF2A72-B3B7-4CD9-A652-91B8230773A6", "EC43CF32-3BDF-4544-8B6A-CE9208DD7C81", SystemGuid.Category.MINISTRY_SAFE_PERSON_ATTRIBUTE_CATEGORY );

            // Person Attribute Result
            RockMigrationHelper.UpdatePersonAttribute( FieldType.BOOLEAN, SystemGuid.Category.MINISTRY_SAFE_PERSON_ATTRIBUTE_CATEGORY, "Ministry Safe Training Result", "MSTrainingResult", "", "The user's pass/fail status from the Ministry Safe training.", 3, "", SystemGuid.Attribute.MINISTRY_SAFE_PERSON_ATTRIBUTE_TEST_RESULT );
            RockMigrationHelper.UpdateAttributeQualifier( SystemGuid.Attribute.MINISTRY_SAFE_PERSON_ATTRIBUTE_TEST_RESULT, "truetext", "Pass", "9FCEE8FD-12C2-4602-AB32-9AC9590F2156" );
            RockMigrationHelper.UpdateAttributeQualifier( SystemGuid.Attribute.MINISTRY_SAFE_PERSON_ATTRIBUTE_TEST_RESULT, "falsetext", "Fail", "6F4FA3B6-8958-4DDA-AC5B-73DB8500E0C4" );

            Sql( @"
                DECLARE @MSResultId INT = ( SELECT [Id] FROM [Attribute] WHERE [Guid] = 'DB9D8876-4550-424D-A019-1AA71F8ADC1D' )
                DECLARE @SingleSelectFieldType INT = ( SELECT [Id] FROM [FieldType] WHERE [Guid] = '7525C4CB-EE6B-41D4-9B64-A08048D5A5C0' )

                UPDATE AttributeValue
                SET [Value] = 'Pass'
                WHERE [AttributeId] = @MSResultId
                AND [Value] like 'True'

                UPDATE AttributeValue
                SET [Value] = 'Fail'
                WHERE [AttributeId] = @MSResultId
                AND [Value] like 'False'

                DELETE FROM [AttributeQualifier]
                WHERE [Guid] = '9FCEE8FD-12C2-4602-AB32-9AC9590F2156'

                DELETE FROM [AttributeQualifier]
                WHERE [Guid] = '6F4FA3B6-8958-4DDA-AC5B-73DB8500E0C4'

                UPDATE Attribute
                SET FieldTypeId = @SingleSelectFieldType
                WHERE [Id] = @MSResultId
            " );

            RockMigrationHelper.UpdateAttributeQualifier( SystemGuid.Attribute.MINISTRY_SAFE_PERSON_ATTRIBUTE_TEST_RESULT, "fieldtype", "ddl", "6325EF63-CC10-4173-A69B-21BD22A8E75B" );
            RockMigrationHelper.UpdateAttributeQualifier( SystemGuid.Attribute.MINISTRY_SAFE_PERSON_ATTRIBUTE_TEST_RESULT, "values", "Pass,Fail,Pending", "62F5D81B-BF3F-4349-858F-9158DE58F268" );

            // Register the Ministry Safe Badge
            RockMigrationHelper.UpdatePersonBadge( "Ministry Safe", "Shows the person's Ministry Safe Results.", "Rock.PersonProfile.Badge.Liquid", 0, SystemGuid.PersonBadge.MINISTRY_SAFE_PERSON_BADGE );
            RockMigrationHelper.AddPersonBadgeAttributeValue( SystemGuid.PersonBadge.MINISTRY_SAFE_PERSON_BADGE, "01C9BA59-D8D4-4137-90A6-B3C06C70BBC3", "<style>     .badge-ms .badge-content {        padding:5px;     border-radius: 5px;     }      .badge-ms .badge-content {         position: relative;         opacity: .6;     }      .badge-ms .badge-content i {         font-size: 36px;     }      .badge-ms .badge-content span.score {         position: absolute;         top: 20px;         right: 50%;         width: 35px;         margin-right: -18px;         font-size: 10px;         font-weight: 700;         text-align: center;     } </style>  {% assign trainingDate = Person | Attribute:'MSTrainingDate' -%} {% assign daysSinceTraining = 'Now' | DateDiff:trainingDate,'d' -%} {% assign trainingScore = Person | Attribute:'MSTrainingScore' -%} {% assign trainingResult = Person | Attribute:'MSTrainingResult' -%} {% if trainingResult == 'Pending' -%}         <div class=\"badge badge-ms\" data-toggle=\"tooltip\" data-original-title=\"{{ Person.NickName }} has been sent the Ministry Safe Training and has not finished.\">             <div class=\"badge-score\">                 <div class=\"badge-content label-info\">                     <i class=\"fa fa-clipboard badge-icon\"></i>                     <span class=\"score\">P</span>                 </div>             </div>         </div> {% elseif trainingResult == 'Fail' -%}         <div class=\"badge badge-ms\" data-toggle=\"tooltip\" data-original-title=\"{{ Person.NickName }} has failed the Ministry Safe Training.\">             <div class=\"badge-score\">                 <div class=\"badge-content label-danger\">                     <i class=\"fa fa-clipboard badge-icon\"></i>                     <span class=\"score\">{{ trainingScore }}</span>                 </div>             </div>         </div> {% else -%}     {% if daysSinceTraining < -1095 -%}         <div class=\"badge badge-ms\" data-toggle=\"tooltip\" data-original-title=\"{{ Person.NickName }}'s Ministry Safe Training has expired.\">             <div class=\"badge-score\">                 <div class=\"badge-content label-warning\">                     <i class=\"fa fa-clipboard badge-icon\"></i>                     <span class=\"score\">{{ trainingScore }}</span>                 </div>             </div>         </div>     {% elseif trainingDate == blank -%}         <div class=\"badge badge-ms badge-disabled\" data-toggle=\"tooltip\" data-original-title=\"{{ Person.NickName }} has not completed a Ministry Safe Training.\">             <i class=\"badge-icon badge-disabled fa fa-clipboard\"></i>         </div>     {% else -%}         <div class=\"badge badge-ms\" data-toggle=\"tooltip\" data-original-title=\"{{ Person.NickName }} is current with Ministry Safe Training.<br /> Score: {{ trainingScore }}.\">             <div class=\"badge-score\">                 <div class=\"badge-content label-success\">                     <i class=\"fa fa-clipboard badge-icon\"></i>                     <span class=\"score\">{{ trainingScore }}</span>                 </div>             </div>         </div>     {% endif -%} {% endif -%}" );

            // Add to Person Details right badge bar
            Sql( string.Format( @"
                UPDATE AV
    	            SET AV.[Value] = AV.[Value] + CASE WHEN AV.[Value] != '' THEN ',' END + '{0}'
                    FROM AttributeValue AS AV
                    LEFT JOIN [Block] AS B ON B.Id = AV.EntityId
                    LEFT JOIN [Attribute] AS A ON A.Id = AV.[AttributeId]
                    WHERE B.[Guid] = 'F3E6CC14-C540-4FFC-A5A9-48AD9CC0A61B'
                      AND A.[Guid] = 'F5AB231E-3836-4D52-BD03-BF79773C237A'
    	              AND AV.[Value] NOT LIKE '%{0}%'
            ", SystemGuid.PersonBadge.MINISTRY_SAFE_PERSON_BADGE ) );

            #region Workflows

            // Create Ministry Safe Person Training Workflow

            #region EntityTypes

            Sql( @"
                    DELETE FROM [EntityType]
                    WHERE [Name] = 'rocks.kfs.MinistrySafe.Workflow.Action.MinistrySafe.MinistrySafeCreateUser'
                    
                    DELETE FROM [EntityType]
                    WHERE [Name] = 'rocks.kfs.MinistrySafe.Workflow.Action.MinistrySafe.MinistrySafeGetUserResults'
                    
                    IF EXISTS ( SELECT [Id] FROM [EntityType] WHERE [Guid] = '8A1F13F8-53F6-4108-BFC5-F2131EDFE8E2' )
                    BEGIN
                        UPDATE [EntityType]
                        SET [Name] = 'rocks.kfs.MinistrySafe.Workflow.Action.MinistrySafe.MinistrySafeCreateUser'
                        WHERE [Guid] = '8A1F13F8-53F6-4108-BFC5-F2131EDFE8E2'
                    END
                    
                    IF EXISTS ( SELECT [Id] FROM [EntityType] WHERE [Guid] = '481197B7-50AD-4E7D-BD16-F9B2858891A5' )
                    BEGIN
                        UPDATE [EntityType]
                        SET [Name] = 'rocks.kfs.MinistrySafe.Workflow.Action.MinistrySafe.MinistrySafeGetUserResults'
                        WHERE [Guid] = '481197B7-50AD-4E7D-BD16-F9B2858891A5'
                    END
                " );

            RockMigrationHelper.UpdateEntityType( "rocks.kfs.MinistrySafe.Workflow.Action.MinistrySafe.MinistrySafeCreateUser", "8A1F13F8-53F6-4108-BFC5-F2131EDFE8E2", false, true );
            RockMigrationHelper.UpdateEntityType( "rocks.kfs.MinistrySafe.Workflow.Action.MinistrySafe.MinistrySafeGetUserResults", "481197B7-50AD-4E7D-BD16-F9B2858891A5", false, true );
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "481197B7-50AD-4E7D-BD16-F9B2858891A5", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Active", "Active", "Should Service be used?", 0, @"False", "3167CFBB-E14D-4CFE-9627-B2F366BD0CB9" ); // rocks.kfs.MinistrySafe.Workflow.Action.MinistrySafe.MinistrySafeGetUserResults:Active
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "481197B7-50AD-4E7D-BD16-F9B2858891A5", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Staging Mode", "StagingMode", "Flag indicating if Ministry Safe Staging Mode should be used.", 1, @"False", "227E8888-AFB9-4745-9EFD-4F6CAF487996" ); // rocks.kfs.MinistrySafe.Workflow.Action.MinistrySafe.MinistrySafeGetUserResults:Staging Mode
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "481197B7-50AD-4E7D-BD16-F9B2858891A5", "33E6DF69-BDFA-407A-9744-C175B60643AE", "Date Completed", "DateCompleted", "Workflow attribute to store the Date Completed.", 1, @"", "0FA480DA-C75F-43E4-94F1-76921670B798" ); // rocks.kfs.MinistrySafe.Workflow.Action.MinistrySafe.MinistrySafeGetUserResults:Date Completed
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "481197B7-50AD-4E7D-BD16-F9B2858891A5", "33E6DF69-BDFA-407A-9744-C175B60643AE", "Ministry Safe Id", "MinistrySafeId", "The User Id in the Ministry Safe system.", 0, @"", "71E73409-2A9A-469B-BE40-74379D7DCD2D" ); // rocks.kfs.MinistrySafe.Workflow.Action.MinistrySafe.MinistrySafeGetUserResults:Ministry Safe Id
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "481197B7-50AD-4E7D-BD16-F9B2858891A5", "33E6DF69-BDFA-407A-9744-C175B60643AE", "Result", "Result", "Workflow attribute to store the Result.", 4, @"", "A443807B-07A4-4FB2-AA28-84954B455218" ); // rocks.kfs.MinistrySafe.Workflow.Action.MinistrySafe.MinistrySafeGetUserResults:Result
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "481197B7-50AD-4E7D-BD16-F9B2858891A5", "33E6DF69-BDFA-407A-9744-C175B60643AE", "Score", "Score", "Workflow attribute to store the Score.", 2, @"", "572FE7DC-50D7-4C28-9CA9-D89F8A8E12B5" ); // rocks.kfs.MinistrySafe.Workflow.Action.MinistrySafe.MinistrySafeGetUserResults:Score
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "481197B7-50AD-4E7D-BD16-F9B2858891A5", "36167F3E-8CB2-44F9-9022-102F171FBC9A", "API Key", "APIKey", "Optional API Key to override Global Attribute.", 0, @"", "656B34CE-79AF-4FEE-952D-A129A433C063" ); // rocks.kfs.MinistrySafe.Workflow.Action.MinistrySafe.MinistrySafeGetUserResults:API Key
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "481197B7-50AD-4E7D-BD16-F9B2858891A5", "A75DFC58-7A1B-4799-BF31-451B2BBE38FF", "Order", "Order", "The order that this service should be used (priority)", 0, @"", "2694605F-69D1-4403-B5FD-F5234500FD4D" ); // rocks.kfs.MinistrySafe.Workflow.Action.MinistrySafe.MinistrySafeGetUserResults:Order
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "481197B7-50AD-4E7D-BD16-F9B2858891A5", "A75DFC58-7A1B-4799-BF31-451B2BBE38FF", "Pass Score", "PassScore", "The minimum score to consider a pass.", 3, @"80", "92BCAFAE-075B-4BA9-BE5F-A941A0EC3F0C" ); // rocks.kfs.MinistrySafe.Workflow.Action.MinistrySafe.MinistrySafeGetUserResults:Pass Score
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "8A1F13F8-53F6-4108-BFC5-F2131EDFE8E2", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Active", "Active", "Should Service be used?", 0, @"False", "09B52E96-E13C-4989-8E42-5F9C3E8AF31A" ); // rocks.kfs.MinistrySafe.Workflow.Action.MinistrySafe.MinistrySafeCreateUser:Active
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "8A1F13F8-53F6-4108-BFC5-F2131EDFE8E2", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Staging Mode", "StagingMode", "Flag indicating if Ministry Safe Staging Mode should be used.", 1, @"False", "834BD487-ADE7-4C3C-9AC6-BF2EC9548CD0" ); // rocks.kfs.MinistrySafe.Workflow.Action.MinistrySafe.MinistrySafeCreateUser:Staging Mode
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "8A1F13F8-53F6-4108-BFC5-F2131EDFE8E2", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Use Workflow Id", "UseWorkflowId", "Flag indicating if the Workflow Id should be used as the Ministry Safe External Id.", 2, @"True", "092B8D61-6D25-4811-9F78-8E6FCAF2D2C9" ); // rocks.kfs.MinistrySafe.Workflow.Action.MinistrySafe.MinistrySafeCreateUser:Use Workflow Id
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "8A1F13F8-53F6-4108-BFC5-F2131EDFE8E2", "33E6DF69-BDFA-407A-9744-C175B60643AE", "Direct Login Url", "DirectLoginUrl", "Workflow attribute to store the Ministry Safe Direct Login Url.", 2, @"", "F677B2FF-BBB4-4BDE-8F9C-CF474CEBF52B" ); // rocks.kfs.MinistrySafe.Workflow.Action.MinistrySafe.MinistrySafeCreateUser:Direct Login Url
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "8A1F13F8-53F6-4108-BFC5-F2131EDFE8E2", "33E6DF69-BDFA-407A-9744-C175B60643AE", "Ministry Safe Id", "MinistrySafeId", "Workflow attribute to store the Ministry Safe User Id.", 1, @"", "80180219-80F7-4D45-BF4F-006CC2AD5639" ); // rocks.kfs.MinistrySafe.Workflow.Action.MinistrySafe.MinistrySafeCreateUser:Ministry Safe Id
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "8A1F13F8-53F6-4108-BFC5-F2131EDFE8E2", "33E6DF69-BDFA-407A-9744-C175B60643AE", "Person", "Person", "Workflow attribute that contains the person to update.", 0, @"", "7B93916A-F91D-49AC-965E-BF78A584E248" ); // rocks.kfs.MinistrySafe.Workflow.Action.MinistrySafe.MinistrySafeCreateUser:Person
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "8A1F13F8-53F6-4108-BFC5-F2131EDFE8E2", "36167F3E-8CB2-44F9-9022-102F171FBC9A", "API Key", "APIKey", "Optional API Key to override Global Attribute.", 0, @"", "C369F3F1-C34B-433B-88EA-B318DDB47181" ); // rocks.kfs.MinistrySafe.Workflow.Action.MinistrySafe.MinistrySafeCreateUser:API Key
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "8A1F13F8-53F6-4108-BFC5-F2131EDFE8E2", "3B1D93D7-9414-48F9-80E5-6A3FC8F94C20", "Training Type|Attribute Value", "TType", "The training type that should be assigned to the User.  If left blank or none selected, the Standard Survey will be assigned.", 3, @"", "43C7560D-02B7-4ADF-8720-2CC8923F9C05" ); // rocks.kfs.MinistrySafe.Workflow.Action.MinistrySafe.MinistrySafeCreateUser:Training Type|Attribute Value
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "8A1F13F8-53F6-4108-BFC5-F2131EDFE8E2", "A75DFC58-7A1B-4799-BF31-451B2BBE38FF", "Order", "Order", "The order that this service should be used (priority)", 0, @"", "CEFB2D83-187E-4C42-B0D2-B2133CB8E713" ); // rocks.kfs.MinistrySafe.Workflow.Action.MinistrySafe.MinistrySafeCreateUser:Order

            #endregion

            #region Categories

            RockMigrationHelper.UpdateCategory( "C9F3C4A5-1526-474D-803F-D6C7A45CBBAE", "Safety & Security", "fa fa-medkit", "", "6F8A431C-BEBD-4D33-AAD6-1D70870329C2", 0 ); // Safety & Security

            #endregion

            #region Ministry Safe Training

            RockMigrationHelper.UpdateWorkflowType( false, true, "Ministry Safe Training", "Used to request a Ministry Safe Training be sent to a person", "6F8A431C-BEBD-4D33-AAD6-1D70870329C2", "Training", "fa fa-clipboard", 28800, true, 0, "AE617426-E1E3-4E06-B3D8-B7AB1A7B4892", 0 ); // Ministry Safe Training
            RockMigrationHelper.UpdateWorkflowTypeAttribute( "AE617426-E1E3-4E06-B3D8-B7AB1A7B4892", "E4EAB7B2-0B76-429B-AFE4-AD86D7428C70", "Requester", "Requester", "The person initiating the request", 0, @"", "1C23302E-49BD-432B-9440-E42D289E5D69", false ); // Ministry Safe Training:Requester
            RockMigrationHelper.UpdateWorkflowTypeAttribute( "AE617426-E1E3-4E06-B3D8-B7AB1A7B4892", "E4EAB7B2-0B76-429B-AFE4-AD86D7428C70", "Person", "Person", "The person who request should be initiated for", 1, @"", "6691E18E-D164-4875-9F10-137180C02D27", false ); // Ministry Safe Training:Person
            RockMigrationHelper.UpdateWorkflowTypeAttribute( "AE617426-E1E3-4E06-B3D8-B7AB1A7B4892", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Warn Of Recent", "WarnOfRecent", "Flag indicating if user should be warned that person has a recent background check already.", 2, @"False", "EB0A5B98-FAFD-45A4-9990-FBA732253351", false ); // Ministry Safe Training:Warn Of Recent
            RockMigrationHelper.UpdateWorkflowTypeAttribute( "AE617426-E1E3-4E06-B3D8-B7AB1A7B4892", "1B71FEF4-201F-4D53-8C60-2DF21F1985ED", "Campus", "Campus", "If included, the campus name will be used as the Billing Reference Code for the request (optional)", 3, @"", "427B6949-AE7E-4EEC-8DBA-13A883A1D189", false ); // Ministry Safe Training:Campus
            RockMigrationHelper.UpdateWorkflowTypeAttribute( "AE617426-E1E3-4E06-B3D8-B7AB1A7B4892", "C28C7BF3-A552-4D77-9408-DEDCF760CED0", "Reason", "Reason", "A brief description of the reason that a background check is being requested", 4, @"", "AB581448-3CB4-4FA9-B50A-60A42E4ADD71", false ); // Ministry Safe Training:Reason
            RockMigrationHelper.UpdateWorkflowTypeAttribute( "AE617426-E1E3-4E06-B3D8-B7AB1A7B4892", "6B6AA175-4758-453F-8D83-FCD8044B5F36", "Training Date", "TrainingDate", "The date of the training", 5, @"", "C259E954-3FDD-4CAD-AC43-E0A0D68225EF", false ); // Ministry Safe Training:Training Date
            RockMigrationHelper.UpdateWorkflowTypeAttribute( "AE617426-E1E3-4E06-B3D8-B7AB1A7B4892", "59D5A94C-94A0-4630-B80A-BB25697D74C7", "Training Type", "TrainingType", "The Training Type to administer to the person", 6, @"16a04162-2396-4a64-937d-e222ada2cbb4", "FAE024DA-3908-449A-A4F5-882CB7277CC1", false ); // Ministry Safe Training:Training Type
            RockMigrationHelper.UpdateWorkflowTypeAttribute( "AE617426-E1E3-4E06-B3D8-B7AB1A7B4892", "A75DFC58-7A1B-4799-BF31-451B2BBE38FF", "Training Score", "TrainingScore", "The score of the training", 7, @"", "B23AB632-B01C-445A-8362-68BA226A7D4E", false ); // Ministry Safe Training:Training Score
            RockMigrationHelper.UpdateWorkflowTypeAttribute( "AE617426-E1E3-4E06-B3D8-B7AB1A7B4892", "7525C4CB-EE6B-41D4-9B64-A08048D5A5C0", "Training Result", "TrainingResult", "", 8, @"", "AA0CE3D6-1FEC-48A6-AEE0-97410BBDBC78", false ); // Ministry Safe Training:Training Result
            RockMigrationHelper.UpdateWorkflowTypeAttribute( "AE617426-E1E3-4E06-B3D8-B7AB1A7B4892", "9C204CD0-1233-41C5-818A-C5DA439445AA", "Ministry Safe Id", "MinistrySafeId", "The Id of the User in the Ministry Safe system", 9, @"", "A0C65BC0-C61D-419C-881B-2C6AAB5CE5E3", false ); // Ministry Safe Training:Ministry Safe Id
            RockMigrationHelper.UpdateWorkflowTypeAttribute( "AE617426-E1E3-4E06-B3D8-B7AB1A7B4892", "C0D0D7E2-C3B0-4004-ABEA-4BBFAD10D5D2", "Direct Login Url", "DirectLoginUrl", "The Login Url to send to the person to view training videos", 10, @"", "C9911445-C2AA-47D4-B051-93B42128E35D", false ); // Ministry Safe Training:Direct Login Url
            RockMigrationHelper.UpdateWorkflowTypeAttribute( "AE617426-E1E3-4E06-B3D8-B7AB1A7B4892", "99B090AA-4D7E-46D8-B393-BF945EA1BA8B", "Training Date Attribute", "TrainingDateAttribute", "", 11, @"a5df28ec-04dc-44d5-9a7b-1c6bbc87037c", "A7F8EB85-024C-4F75-874F-78E03CD7EEBA", false ); // Ministry Safe Training:Training Date Attribute
            RockMigrationHelper.UpdateWorkflowTypeAttribute( "AE617426-E1E3-4E06-B3D8-B7AB1A7B4892", "A75DFC58-7A1B-4799-BF31-451B2BBE38FF", "ConnectionRequestId", "ConnectionRequestId", "", 12, @"0", "5FC958C7-98F2-4AC5-ADC3-7A822BA66437", false ); // Ministry Safe Training:ConnectionRequestId
            RockMigrationHelper.UpdateAttributeQualifier( "AA0CE3D6-1FEC-48A6-AEE0-97410BBDBC78", "fieldtype", @"ddl", "40EB5429-32DB-48A4-8E17-3E5BE6C24C6D" ); // Ministry Safe Training:Training Result:fieldtype
            RockMigrationHelper.UpdateAttributeQualifier( "AA0CE3D6-1FEC-48A6-AEE0-97410BBDBC78", "values", @"Pass,Fail,Pending", "656FD54C-935B-4F80-9729-D55E2C4721AB" ); // Ministry Safe Training:Training Result:values
            RockMigrationHelper.UpdateWorkflowActivityType( "AE617426-E1E3-4E06-B3D8-B7AB1A7B4892", true, "Initial Request", "Saves the person and requester and prompts for additional information needed.", true, 0, "CD9029C2-D420-4A0B-AE7C-90D26EA080BF" ); // Ministry Safe Training:Initial Request
            RockMigrationHelper.UpdateWorkflowActivityType( "AE617426-E1E3-4E06-B3D8-B7AB1A7B4892", true, "Approve Request", "Assigns the activity to security team and waits for their approval before submitting the request.", false, 1, "3EC65FD2-A676-4DD5-AA5B-1794662607C3" ); // Ministry Safe Training:Approve Request
            RockMigrationHelper.UpdateWorkflowActivityType( "AE617426-E1E3-4E06-B3D8-B7AB1A7B4892", true, "Review Denial", "Provides the requester a way to add additional information for the security team to approve request.", false, 2, "25B9CF22-C437-40C8-A88B-6C44AD22639B" ); // Ministry Safe Training:Review Denial
            RockMigrationHelper.UpdateWorkflowActivityType( "AE617426-E1E3-4E06-B3D8-B7AB1A7B4892", true, "Submit Request", "Submits the Ministry Safe training request.", false, 3, "7AACF619-9882-4AA1-9522-ECF872482CFD" ); // Ministry Safe Training:Submit Request
            RockMigrationHelper.UpdateWorkflowActivityType( "AE617426-E1E3-4E06-B3D8-B7AB1A7B4892", true, "Request Error", "Prompts when there is an error submitting to Ministry Safe.", false, 4, "B14EF83F-CECD-4864-9C38-2D3218103CB0" ); // Ministry Safe Training:Request Error
            RockMigrationHelper.UpdateWorkflowActivityType( "AE617426-E1E3-4E06-B3D8-B7AB1A7B4892", true, "Retrieve Results", "Checks Ministry Safe to see if training is completed.", false, 5, "DEFEECE4-CBFF-42E5-BD56-D54217A1E2E4" ); // Ministry Safe Training:Retrieve Results
            RockMigrationHelper.UpdateWorkflowActivityType( "AE617426-E1E3-4E06-B3D8-B7AB1A7B4892", true, "Complete Request", "Notifies requester of result and updates person's record with result", false, 6, "4E3D92C6-C186-4D9A-9888-D49F607E5F79" ); // Ministry Safe Training:Complete Request
            RockMigrationHelper.UpdateWorkflowActivityType( "AE617426-E1E3-4E06-B3D8-B7AB1A7B4892", true, "Cancel Request", "Cancels the request prior to submitting to provider and deletes the workflow.", false, 7, "120D9EB5-7A0C-4416-86DE-F3FC62856824" ); // Ministry Safe Training:Cancel Request
            RockMigrationHelper.UpdateWorkflowActivityTypeAttribute( "3EC65FD2-A676-4DD5-AA5B-1794662607C3", "C28C7BF3-A552-4D77-9408-DEDCF760CED0", "Note", "Note", "Any notes that approver wants to provide to submitter for review", 0, @"", "8D2F0CE6-25AA-407B-83CD-A4A50B3461ED" ); // Ministry Safe Training:Approve Request:Note
            RockMigrationHelper.UpdateWorkflowActivityTypeAttribute( "3EC65FD2-A676-4DD5-AA5B-1794662607C3", "E4EAB7B2-0B76-429B-AFE4-AD86D7428C70", "Approver", "Approver", "Person who approved or denied this request", 1, @"", "083B9E1C-DEDD-4FA2-AF11-AD2A160B523C" ); // Ministry Safe Training:Approve Request:Approver
            RockMigrationHelper.UpdateWorkflowActivityTypeAttribute( "3EC65FD2-A676-4DD5-AA5B-1794662607C3", "9C204CD0-1233-41C5-818A-C5DA439445AA", "Approval Status", "ApprovalStatus", "The status of the appoval (Approve,Deny)", 2, @"", "C3DFC12B-8091-4B80-8BF7-71167EF1A86B" ); // Ministry Safe Training:Approve Request:Approval Status
            RockMigrationHelper.UpdateWorkflowActivityTypeAttribute( "DEFEECE4-CBFF-42E5-BD56-D54217A1E2E4", "9C204CD0-1233-41C5-818A-C5DA439445AA", "Delay Activated", "2432fabb-eaed-4145-b45b-c41bea53668d", "", 0, @"", "E4624D32-ADA4-471A-9FC8-C40E14F1C88D" ); // Ministry Safe Training:Retrieve Results:Delay Activated
            RockMigrationHelper.UpdateWorkflowActionForm( @"<h1>Ministry Safe Training Request Details</h1> <p>     {{CurrentPerson.NickName}}, please complete the form below to start the Ministry Safe training process. </p> {% if Workflow.WarnOfRecent == 'Yes' %}     <div class='alert alert-warning'>         Notice: It's been less than a year since this person's last training was processed.         Please make sure you want to continue with this request!     </div> {% endif %} <hr />", @"", "Submit^fdc397cd-8b4a-436e-bea1-bce2e6717c03^3ec65fd2-a676-4dd5-aa5b-1794662607c3^Your request has been submitted successfully.|Cancel^5683E775-B9F3-408C-80AC-94DE0E51CF3A^120d9eb5-7a0c-4416-86de-f3fc62856824^The request has been cancelled.", "", false, "", "C885B724-FFD8-47F5-930B-81E4BF6DA8F1" ); // Ministry Safe Training:Initial Request:Get Details
            RockMigrationHelper.UpdateWorkflowActionForm( @"<h1>Ministry Safe Training Request Details</h1> <div class='alert alert-info'>     {{CurrentPerson.NickName}}, the following Ministry Safe training request has been submitted for your review.     If you approve the request it will be sent to Ministry Safe for processing. If you     deny the request, it will be sent back to the requester. If you deny the request, please add notes     explaining why the request was denied. </div>", @"", "Approve^c88fef94-95b9-444a-bc93-58e983f3c047^^The request has been submitted to provider for processing.|Deny^d6b809a9-c1cc-4ebb-816e-33d8c1e53ea4^^The requester will be notified that this request has been denied (along with the reason why).|", "88C7D1CC-3478-4562-A301-AE7D4D7FFF6D", true, "C3DFC12B-8091-4B80-8BF7-71167EF1A86B", "06E54EA2-7C46-491A-9A43-92D2D07D6795" ); // Ministry Safe Training:Approve Request:Approve or Deny
            RockMigrationHelper.UpdateWorkflowActionForm( @"<h1>Ministry Safe Training Request Details</h1> <p>     {{CurrentPerson.NickName}}, this request has come back from the approval process with the following results. </p>  <div class=""well"">     <strong>Summary of Security Notes:</strong><br />     <table class="" table table-condensed table-light margin-b-md"">      {% for activity in Workflow.Activities %}       {% if activity.ActivityType.Name == 'Approve Request' %}        <tr>         <td width=""220"">{{activity.CompletedDateTime}}</td>         <td width=""220"">{{activity.Approver}}</td>         <td>{{activity.Note}}</td>        </tr>       {% endif %}      {% endfor %}     </table>      </div> <hr />", @"", "Submit^fdc397cd-8b4a-436e-bea1-bce2e6717c03^3ec65fd2-a676-4dd5-aa5b-1794662607c3^The request has been submitted again to the security team for approval.|Cancel Request^5683E775-B9F3-408C-80AC-94DE0E51CF3A^120d9eb5-7a0c-4416-86de-f3fc62856824^The request has been cancelled.", "88C7D1CC-3478-4562-A301-AE7D4D7FFF6D", true, "", "74D737C9-1275-4B56-8E59-A3D85116DB5C" ); // Ministry Safe Training:Review Denial:Review
            RockMigrationHelper.UpdateWorkflowActionForm( @"<h1>Ministry Safe Training Request Details</h1> <div class='alert alert-danger'>     An error occurred when submitting this request to Ministry Safe. Please check the workflow logs for more details, then resubmit when resolved. </div>", @"", "Re-Submit^fdc397cd-8b4a-436e-bea1-bce2e6717c03^7aacf619-9882-4aa1-9522-ecf872482cfd^Your information has been submitted successfully.", "", true, "", "AE2BC2CC-37C1-4D68-B429-228690EC81B1" ); // Ministry Safe Training:Request Error:Display Error Message
            RockMigrationHelper.UpdateWorkflowActionFormAttribute( "C885B724-FFD8-47F5-930B-81E4BF6DA8F1", "1C23302E-49BD-432B-9440-E42D289E5D69", 0, false, true, false, false, @"", @"", "CBF471B7-669E-4E7A-856F-347080DCCD62" ); // Ministry Safe Training:Initial Request:Get Details:Requester
            RockMigrationHelper.UpdateWorkflowActionFormAttribute( "C885B724-FFD8-47F5-930B-81E4BF6DA8F1", "6691E18E-D164-4875-9F10-137180C02D27", 1, true, true, false, false, @"", @"", "50B78215-02BC-4387-AA4E-B2E72FE1B3F4" ); // Ministry Safe Training:Initial Request:Get Details:Person
            RockMigrationHelper.UpdateWorkflowActionFormAttribute( "C885B724-FFD8-47F5-930B-81E4BF6DA8F1", "EB0A5B98-FAFD-45A4-9990-FBA732253351", 2, false, true, false, false, @"", @"", "3C756FB0-59E5-43E4-BDAE-BF80E264BE6E" ); // Ministry Safe Training:Initial Request:Get Details:Warn Of Recent
            RockMigrationHelper.UpdateWorkflowActionFormAttribute( "C885B724-FFD8-47F5-930B-81E4BF6DA8F1", "427B6949-AE7E-4EEC-8DBA-13A883A1D189", 3, true, false, false, false, @"", @"", "78D309D9-3047-41A2-80BF-ABE83AF9C78C" ); // Ministry Safe Training:Initial Request:Get Details:Campus
            RockMigrationHelper.UpdateWorkflowActionFormAttribute( "C885B724-FFD8-47F5-930B-81E4BF6DA8F1", "AB581448-3CB4-4FA9-B50A-60A42E4ADD71", 5, true, false, false, false, @"", @"", "C51327EF-F1D2-4F1C-B282-013F6EC55F4A" ); // Ministry Safe Training:Initial Request:Get Details:Reason
            RockMigrationHelper.UpdateWorkflowActionFormAttribute( "C885B724-FFD8-47F5-930B-81E4BF6DA8F1", "C259E954-3FDD-4CAD-AC43-E0A0D68225EF", 8, false, true, false, false, @"", @"", "48F02395-EE2E-42E8-99A6-9E15E0D2E573" ); // Ministry Safe Training:Initial Request:Get Details:Training Date
            RockMigrationHelper.UpdateWorkflowActionFormAttribute( "C885B724-FFD8-47F5-930B-81E4BF6DA8F1", "FAE024DA-3908-449A-A4F5-882CB7277CC1", 4, true, false, true, false, @"", @"", "7B2A24EA-3D34-493F-9022-69ED28270B3C" ); // Ministry Safe Training:Initial Request:Get Details:Training Type
            RockMigrationHelper.UpdateWorkflowActionFormAttribute( "C885B724-FFD8-47F5-930B-81E4BF6DA8F1", "B23AB632-B01C-445A-8362-68BA226A7D4E", 9, false, true, false, false, @"", @"", "9FA6ACD5-7B0D-4290-B4E8-B5DDABBC06CA" ); // Ministry Safe Training:Initial Request:Get Details:Training Score
            RockMigrationHelper.UpdateWorkflowActionFormAttribute( "C885B724-FFD8-47F5-930B-81E4BF6DA8F1", "AA0CE3D6-1FEC-48A6-AEE0-97410BBDBC78", 12, false, true, false, false, @"", @"", "8153F497-2974-457B-AFB3-5DCDC47EC55B" ); // Ministry Safe Training:Initial Request:Get Details:Training Result
            RockMigrationHelper.UpdateWorkflowActionFormAttribute( "C885B724-FFD8-47F5-930B-81E4BF6DA8F1", "A0C65BC0-C61D-419C-881B-2C6AAB5CE5E3", 6, false, true, false, false, @"", @"", "1359FDC4-29DA-47D6-83BF-593C2DC60C15" ); // Ministry Safe Training:Initial Request:Get Details:Ministry Safe Id
            RockMigrationHelper.UpdateWorkflowActionFormAttribute( "C885B724-FFD8-47F5-930B-81E4BF6DA8F1", "C9911445-C2AA-47D4-B051-93B42128E35D", 7, false, true, false, false, @"", @"", "2051A04E-F415-4788-9CE2-96619D5902A0" ); // Ministry Safe Training:Initial Request:Get Details:Direct Login Url
            RockMigrationHelper.UpdateWorkflowActionFormAttribute( "C885B724-FFD8-47F5-930B-81E4BF6DA8F1", "A7F8EB85-024C-4F75-874F-78E03CD7EEBA", 10, false, true, false, false, @"", @"", "6D91B3CF-81A9-448C-BE01-B35CD6B85994" ); // Ministry Safe Training:Initial Request:Get Details:Training Date Attribute
            RockMigrationHelper.UpdateWorkflowActionFormAttribute( "C885B724-FFD8-47F5-930B-81E4BF6DA8F1", "5FC958C7-98F2-4AC5-ADC3-7A822BA66437", 11, false, true, false, false, @"", @"", "9C93307F-527A-43E2-B820-B01921CA4A25" ); // Ministry Safe Training:Initial Request:Get Details:ConnectionRequestId
            RockMigrationHelper.UpdateWorkflowActionFormAttribute( "06E54EA2-7C46-491A-9A43-92D2D07D6795", "8D2F0CE6-25AA-407B-83CD-A4A50B3461ED", 6, true, false, false, false, @"", @"", "0AD1A6F4-7045-4F9D-9033-3597385B19E3" ); // Ministry Safe Training:Approve Request:Approve or Deny:Note
            RockMigrationHelper.UpdateWorkflowActionFormAttribute( "06E54EA2-7C46-491A-9A43-92D2D07D6795", "1C23302E-49BD-432B-9440-E42D289E5D69", 0, true, true, false, false, @"", @"", "8CD97FB8-E7B3-48B6-B57D-F3E89940FB71" ); // Ministry Safe Training:Approve Request:Approve or Deny:Requester
            RockMigrationHelper.UpdateWorkflowActionFormAttribute( "06E54EA2-7C46-491A-9A43-92D2D07D6795", "6691E18E-D164-4875-9F10-137180C02D27", 1, true, true, false, false, @"", @"", "45E16C32-044B-4ADA-BD0B-7D00E9C7969A" ); // Ministry Safe Training:Approve Request:Approve or Deny:Person
            RockMigrationHelper.UpdateWorkflowActionFormAttribute( "06E54EA2-7C46-491A-9A43-92D2D07D6795", "083B9E1C-DEDD-4FA2-AF11-AD2A160B523C", 7, false, true, false, false, @"", @"", "C97E8582-7A53-40DD-90B4-B9BF3B0F27F9" ); // Ministry Safe Training:Approve Request:Approve or Deny:Approver
            RockMigrationHelper.UpdateWorkflowActionFormAttribute( "06E54EA2-7C46-491A-9A43-92D2D07D6795", "C3DFC12B-8091-4B80-8BF7-71167EF1A86B", 8, false, true, false, false, @"", @"", "44CD0C69-0331-42CB-8984-24413D6EB3C7" ); // Ministry Safe Training:Approve Request:Approve or Deny:Approval Status
            RockMigrationHelper.UpdateWorkflowActionFormAttribute( "06E54EA2-7C46-491A-9A43-92D2D07D6795", "EB0A5B98-FAFD-45A4-9990-FBA732253351", 2, false, true, false, false, @"", @"", "B83749B0-2AE8-4F15-A5A7-F489B001D242" ); // Ministry Safe Training:Approve Request:Approve or Deny:Warn Of Recent
            RockMigrationHelper.UpdateWorkflowActionFormAttribute( "06E54EA2-7C46-491A-9A43-92D2D07D6795", "427B6949-AE7E-4EEC-8DBA-13A883A1D189", 3, true, true, false, false, @"", @"", "FB5D305A-0990-4463-A720-4FF7E6E8AF6E" ); // Ministry Safe Training:Approve Request:Approve or Deny:Campus
            RockMigrationHelper.UpdateWorkflowActionFormAttribute( "06E54EA2-7C46-491A-9A43-92D2D07D6795", "AB581448-3CB4-4FA9-B50A-60A42E4ADD71", 4, true, true, false, false, @"", @"", "9D434F83-CD3F-4937-B983-B6B7BABE5C78" ); // Ministry Safe Training:Approve Request:Approve or Deny:Reason
            RockMigrationHelper.UpdateWorkflowActionFormAttribute( "06E54EA2-7C46-491A-9A43-92D2D07D6795", "C259E954-3FDD-4CAD-AC43-E0A0D68225EF", 11, false, true, false, false, @"", @"", "1A1B59A9-A799-4D4D-B664-71F53DFD383B" ); // Ministry Safe Training:Approve Request:Approve or Deny:Training Date
            RockMigrationHelper.UpdateWorkflowActionFormAttribute( "06E54EA2-7C46-491A-9A43-92D2D07D6795", "FAE024DA-3908-449A-A4F5-882CB7277CC1", 5, true, false, true, false, @"", @"", "6134BEB5-023C-446F-B10C-BB2D5A8EFA26" ); // Ministry Safe Training:Approve Request:Approve or Deny:Training Type
            RockMigrationHelper.UpdateWorkflowActionFormAttribute( "06E54EA2-7C46-491A-9A43-92D2D07D6795", "B23AB632-B01C-445A-8362-68BA226A7D4E", 12, false, true, false, false, @"", @"", "22322762-64AD-4B26-A907-E5D1197975CA" ); // Ministry Safe Training:Approve Request:Approve or Deny:Training Score
            RockMigrationHelper.UpdateWorkflowActionFormAttribute( "06E54EA2-7C46-491A-9A43-92D2D07D6795", "AA0CE3D6-1FEC-48A6-AEE0-97410BBDBC78", 15, false, true, false, false, @"", @"", "9363A054-EF92-4DFF-9AAB-40419B81D1DB" ); // Ministry Safe Training:Approve Request:Approve or Deny:Training Result
            RockMigrationHelper.UpdateWorkflowActionFormAttribute( "06E54EA2-7C46-491A-9A43-92D2D07D6795", "A0C65BC0-C61D-419C-881B-2C6AAB5CE5E3", 9, false, true, false, false, @"", @"", "A1C92714-111E-427E-B9BD-DA16C831F45A" ); // Ministry Safe Training:Approve Request:Approve or Deny:Ministry Safe Id
            RockMigrationHelper.UpdateWorkflowActionFormAttribute( "06E54EA2-7C46-491A-9A43-92D2D07D6795", "C9911445-C2AA-47D4-B051-93B42128E35D", 10, false, true, false, false, @"", @"", "90862A06-C956-471F-8F1B-59569B400E98" ); // Ministry Safe Training:Approve Request:Approve or Deny:Direct Login Url
            RockMigrationHelper.UpdateWorkflowActionFormAttribute( "06E54EA2-7C46-491A-9A43-92D2D07D6795", "A7F8EB85-024C-4F75-874F-78E03CD7EEBA", 13, false, true, false, false, @"", @"", "CE2D7813-312A-4F09-8416-8C87B9D16AD1" ); // Ministry Safe Training:Approve Request:Approve or Deny:Training Date Attribute
            RockMigrationHelper.UpdateWorkflowActionFormAttribute( "06E54EA2-7C46-491A-9A43-92D2D07D6795", "5FC958C7-98F2-4AC5-ADC3-7A822BA66437", 14, false, true, false, false, @"", @"", "1F7D6DBC-77C4-469D-ACE4-AB0CD7EB4E11" ); // Ministry Safe Training:Approve Request:Approve or Deny:ConnectionRequestId
            RockMigrationHelper.UpdateWorkflowActionFormAttribute( "74D737C9-1275-4B56-8E59-A3D85116DB5C", "1C23302E-49BD-432B-9440-E42D289E5D69", 0, false, true, false, false, @"", @"", "06327B55-CBA4-464F-B6A1-5479C322C7CB" ); // Ministry Safe Training:Review Denial:Review:Requester
            RockMigrationHelper.UpdateWorkflowActionFormAttribute( "74D737C9-1275-4B56-8E59-A3D85116DB5C", "6691E18E-D164-4875-9F10-137180C02D27", 2, true, true, false, false, @"", @"", "0C585014-2F4F-4346-8F47-4316C133A280" ); // Ministry Safe Training:Review Denial:Review:Person
            RockMigrationHelper.UpdateWorkflowActionFormAttribute( "74D737C9-1275-4B56-8E59-A3D85116DB5C", "EB0A5B98-FAFD-45A4-9990-FBA732253351", 1, false, true, false, false, @"", @"", "A3B131DC-0E32-4401-B5C3-58990D1C2E9A" ); // Ministry Safe Training:Review Denial:Review:Warn Of Recent
            RockMigrationHelper.UpdateWorkflowActionFormAttribute( "74D737C9-1275-4B56-8E59-A3D85116DB5C", "427B6949-AE7E-4EEC-8DBA-13A883A1D189", 3, true, false, true, false, @"", @"", "A090845A-CE60-4389-8F21-920985B7E6E3" ); // Ministry Safe Training:Review Denial:Review:Campus
            RockMigrationHelper.UpdateWorkflowActionFormAttribute( "74D737C9-1275-4B56-8E59-A3D85116DB5C", "AB581448-3CB4-4FA9-B50A-60A42E4ADD71", 4, true, false, false, false, @"", @"", "7027964B-E0DB-4BB8-B938-B5B5F08B14DD" ); // Ministry Safe Training:Review Denial:Review:Reason
            RockMigrationHelper.UpdateWorkflowActionFormAttribute( "74D737C9-1275-4B56-8E59-A3D85116DB5C", "C259E954-3FDD-4CAD-AC43-E0A0D68225EF", 8, false, true, false, false, @"", @"", "F07F6F4D-514E-420C-8814-2D638E299BF1" ); // Ministry Safe Training:Review Denial:Review:Training Date
            RockMigrationHelper.UpdateWorkflowActionFormAttribute( "74D737C9-1275-4B56-8E59-A3D85116DB5C", "FAE024DA-3908-449A-A4F5-882CB7277CC1", 7, false, true, false, false, @"", @"", "76EB9B21-6257-4B3D-9D9F-4F664C0DD8C4" ); // Ministry Safe Training:Review Denial:Review:Training Type
            RockMigrationHelper.UpdateWorkflowActionFormAttribute( "74D737C9-1275-4B56-8E59-A3D85116DB5C", "B23AB632-B01C-445A-8362-68BA226A7D4E", 9, false, true, false, false, @"", @"", "4BCC1934-3BC2-4730-846E-273474FD8F49" ); // Ministry Safe Training:Review Denial:Review:Training Score
            RockMigrationHelper.UpdateWorkflowActionFormAttribute( "74D737C9-1275-4B56-8E59-A3D85116DB5C", "AA0CE3D6-1FEC-48A6-AEE0-97410BBDBC78", 12, false, true, false, false, @"", @"", "2E72CC84-0651-4E18-B18A-F1556A9E73BE" ); // Ministry Safe Training:Review Denial:Review:Training Result
            RockMigrationHelper.UpdateWorkflowActionFormAttribute( "74D737C9-1275-4B56-8E59-A3D85116DB5C", "A0C65BC0-C61D-419C-881B-2C6AAB5CE5E3", 5, false, true, false, false, @"", @"", "34B49EEE-0E7B-4F7A-AF48-CE49737DB7A3" ); // Ministry Safe Training:Review Denial:Review:Ministry Safe Id
            RockMigrationHelper.UpdateWorkflowActionFormAttribute( "74D737C9-1275-4B56-8E59-A3D85116DB5C", "C9911445-C2AA-47D4-B051-93B42128E35D", 6, false, true, false, false, @"", @"", "BED7F5EE-EC53-47AB-B2A6-729B09FDCD93" ); // Ministry Safe Training:Review Denial:Review:Direct Login Url
            RockMigrationHelper.UpdateWorkflowActionFormAttribute( "74D737C9-1275-4B56-8E59-A3D85116DB5C", "A7F8EB85-024C-4F75-874F-78E03CD7EEBA", 10, false, true, false, false, @"", @"", "718643F1-9627-43F0-B79C-F32B1E492F76" ); // Ministry Safe Training:Review Denial:Review:Training Date Attribute
            RockMigrationHelper.UpdateWorkflowActionFormAttribute( "74D737C9-1275-4B56-8E59-A3D85116DB5C", "5FC958C7-98F2-4AC5-ADC3-7A822BA66437", 11, false, true, false, false, @"", @"", "ED034EE2-38AF-41AA-B5D8-75B3A74015B3" ); // Ministry Safe Training:Review Denial:Review:ConnectionRequestId
            RockMigrationHelper.UpdateWorkflowActionFormAttribute( "AE2BC2CC-37C1-4D68-B429-228690EC81B1", "1C23302E-49BD-432B-9440-E42D289E5D69", 0, true, true, false, false, @"", @"", "054C36BA-CCF6-4291-A207-3D344589F7CD" ); // Ministry Safe Training:Request Error:Display Error Message:Requester
            RockMigrationHelper.UpdateWorkflowActionFormAttribute( "AE2BC2CC-37C1-4D68-B429-228690EC81B1", "6691E18E-D164-4875-9F10-137180C02D27", 1, true, true, false, false, @"", @"", "8FDF19D0-21EB-4492-BAB4-90AC3103E4FF" ); // Ministry Safe Training:Request Error:Display Error Message:Person
            RockMigrationHelper.UpdateWorkflowActionFormAttribute( "AE2BC2CC-37C1-4D68-B429-228690EC81B1", "EB0A5B98-FAFD-45A4-9990-FBA732253351", 2, false, true, false, false, @"", @"", "DC485704-8222-4D61-AF85-2E2842D889A7" ); // Ministry Safe Training:Request Error:Display Error Message:Warn Of Recent
            RockMigrationHelper.UpdateWorkflowActionFormAttribute( "AE2BC2CC-37C1-4D68-B429-228690EC81B1", "427B6949-AE7E-4EEC-8DBA-13A883A1D189", 3, true, true, false, false, @"", @"", "BB3B102F-D92D-4A7C-9508-2F1178BCC24D" ); // Ministry Safe Training:Request Error:Display Error Message:Campus
            RockMigrationHelper.UpdateWorkflowActionFormAttribute( "AE2BC2CC-37C1-4D68-B429-228690EC81B1", "AB581448-3CB4-4FA9-B50A-60A42E4ADD71", 4, false, true, false, false, @"", @"", "A7C48F91-7E05-4706-A3BE-D10BC99BA0D0" ); // Ministry Safe Training:Request Error:Display Error Message:Reason
            RockMigrationHelper.UpdateWorkflowActionFormAttribute( "AE2BC2CC-37C1-4D68-B429-228690EC81B1", "C259E954-3FDD-4CAD-AC43-E0A0D68225EF", 8, false, true, false, false, @"", @"", "7DCEE8AB-0195-456F-92CE-C77E6C93BCC4" ); // Ministry Safe Training:Request Error:Display Error Message:Training Date
            RockMigrationHelper.UpdateWorkflowActionFormAttribute( "AE2BC2CC-37C1-4D68-B429-228690EC81B1", "FAE024DA-3908-449A-A4F5-882CB7277CC1", 7, true, false, true, false, @"", @"", "0FA3297A-9BFF-45E0-8AEC-C3503FC484D0" ); // Ministry Safe Training:Request Error:Display Error Message:Training Type
            RockMigrationHelper.UpdateWorkflowActionFormAttribute( "AE2BC2CC-37C1-4D68-B429-228690EC81B1", "B23AB632-B01C-445A-8362-68BA226A7D4E", 9, false, true, false, false, @"", @"", "3BBA8F9E-E236-430A-88A8-500B2CB692F4" ); // Ministry Safe Training:Request Error:Display Error Message:Training Score
            RockMigrationHelper.UpdateWorkflowActionFormAttribute( "AE2BC2CC-37C1-4D68-B429-228690EC81B1", "AA0CE3D6-1FEC-48A6-AEE0-97410BBDBC78", 12, false, true, false, false, @"", @"", "12546AB1-4BC9-45DE-9291-D2619BFD3EE0" ); // Ministry Safe Training:Request Error:Display Error Message:Training Result
            RockMigrationHelper.UpdateWorkflowActionFormAttribute( "AE2BC2CC-37C1-4D68-B429-228690EC81B1", "A0C65BC0-C61D-419C-881B-2C6AAB5CE5E3", 5, false, true, false, false, @"", @"", "72397345-183E-40BC-96B7-7BC7DE467550" ); // Ministry Safe Training:Request Error:Display Error Message:Ministry Safe Id
            RockMigrationHelper.UpdateWorkflowActionFormAttribute( "AE2BC2CC-37C1-4D68-B429-228690EC81B1", "C9911445-C2AA-47D4-B051-93B42128E35D", 6, false, true, false, false, @"", @"", "14883254-275F-4C27-87BE-1281D0A1E6A4" ); // Ministry Safe Training:Request Error:Display Error Message:Direct Login Url
            RockMigrationHelper.UpdateWorkflowActionFormAttribute( "AE2BC2CC-37C1-4D68-B429-228690EC81B1", "A7F8EB85-024C-4F75-874F-78E03CD7EEBA", 10, false, true, false, false, @"", @"", "4C6A42E2-F64A-4482-9CD4-7B55EA7E75F9" ); // Ministry Safe Training:Request Error:Display Error Message:Training Date Attribute
            RockMigrationHelper.UpdateWorkflowActionFormAttribute( "AE2BC2CC-37C1-4D68-B429-228690EC81B1", "5FC958C7-98F2-4AC5-ADC3-7A822BA66437", 11, false, true, false, false, @"", @"", "4195FBDE-8153-4148-8BBE-94F0502D68F4" ); // Ministry Safe Training:Request Error:Display Error Message:ConnectionRequestId
            RockMigrationHelper.UpdateWorkflowActionType( "CD9029C2-D420-4A0B-AE7C-90D26EA080BF", "Set Status", 0, "96D371A7-A291-4F8F-8B38-B8F72CE5407E", true, false, "", "", 1, "", "EC0DABEB-32A8-4338-9A07-AF4AEE4C95C2" ); // Ministry Safe Training:Initial Request:Set Status
            RockMigrationHelper.UpdateWorkflowActionType( "CD9029C2-D420-4A0B-AE7C-90D26EA080BF", "Set Person", 1, "972F19B9-598B-474B-97A4-50E56E7B59D2", true, false, "", "6691E18E-D164-4875-9F10-137180C02D27", 32, "", "AE894258-89DB-4890-A0A0-4D0E1743D50C" ); // Ministry Safe Training:Initial Request:Set Person
            RockMigrationHelper.UpdateWorkflowActionType( "CD9029C2-D420-4A0B-AE7C-90D26EA080BF", "Set Name", 2, "36005473-BD5D-470B-B28D-98E6D7ED808D", true, false, "", "", 1, "", "B33C6899-59F3-4C8A-BDB7-4553C8FB03A9" ); // Ministry Safe Training:Initial Request:Set Name
            RockMigrationHelper.UpdateWorkflowActionType( "CD9029C2-D420-4A0B-AE7C-90D26EA080BF", "Set Requester", 3, "24B7D5E6-C30F-48F4-9D7E-AF45A342CF3A", true, false, "", "", 1, "", "32C29676-F7DE-40CB-A3F3-09E35F87BE4E" ); // Ministry Safe Training:Initial Request:Set Requester
            RockMigrationHelper.UpdateWorkflowActionType( "CD9029C2-D420-4A0B-AE7C-90D26EA080BF", "Set Warning", 4, "A41216D6-6FB0-4019-B222-2C29B4519CF4", true, false, "", "", 1, "", "DFEFF167-F0E1-498F-A2CD-7DDEF9C752C3" ); // Ministry Safe Training:Initial Request:Set Warning
            RockMigrationHelper.UpdateWorkflowActionType( "CD9029C2-D420-4A0B-AE7C-90D26EA080BF", "Move to Approve Request from Connection", 5, "38907A90-1634-4A93-8017-619326A4A582", true, true, "", "5FC958C7-98F2-4AC5-ADC3-7A822BA66437", 128, "0", "5030F5FD-BC20-46D0-A19F-CEAD3FFF3B22" ); // Ministry Safe Training:Initial Request:Move to Approve Request from Connection
            RockMigrationHelper.UpdateWorkflowActionType( "CD9029C2-D420-4A0B-AE7C-90D26EA080BF", "Get Details", 6, "486DC4FA-FCBC-425F-90B0-E606DA8A9F68", true, true, "C885B724-FFD8-47F5-930B-81E4BF6DA8F1", "", 1, "", "AD11FE97-FB1D-4EAC-AD72-B1AF19DDEE75" ); // Ministry Safe Training:Initial Request:Get Details
            RockMigrationHelper.UpdateWorkflowActionType( "3EC65FD2-A676-4DD5-AA5B-1794662607C3", "Set Status", 0, "96D371A7-A291-4F8F-8B38-B8F72CE5407E", true, false, "", "", 1, "", "EFD8C68B-E372-47B9-9A85-4E87A0D62BF3" ); // Ministry Safe Training:Approve Request:Set Status
            RockMigrationHelper.UpdateWorkflowActionType( "3EC65FD2-A676-4DD5-AA5B-1794662607C3", "Set Pending Training Result Person Attribute Value", 1, "320622DA-52E0-41AE-AF90-2BF78B488552", true, false, "", "", 1, "", "BDA8B012-FB7D-4139-BD2B-AAB7B4322825" ); // Ministry Safe Training:Approve Request:Set Pending Training Result Person Attribute Value
            RockMigrationHelper.UpdateWorkflowActionType( "3EC65FD2-A676-4DD5-AA5B-1794662607C3", "Assign to Security", 2, "08189B3F-B506-45E8-AA68-99EC51085CF3", true, false, "", "", 1, "", "FD90EF54-7443-4784-BB4A-D3C6F7D96D13" ); // Ministry Safe Training:Approve Request:Assign to Security
            RockMigrationHelper.UpdateWorkflowActionType( "3EC65FD2-A676-4DD5-AA5B-1794662607C3", "Approve or Deny", 3, "486DC4FA-FCBC-425F-90B0-E606DA8A9F68", true, false, "06E54EA2-7C46-491A-9A43-92D2D07D6795", "", 1, "", "A2BE8E3A-BE01-4679-9B10-519FA72E1870" ); // Ministry Safe Training:Approve Request:Approve or Deny
            RockMigrationHelper.UpdateWorkflowActionType( "3EC65FD2-A676-4DD5-AA5B-1794662607C3", "Set Approver", 4, "24B7D5E6-C30F-48F4-9D7E-AF45A342CF3A", true, false, "", "", 1, "", "E49E0381-791E-4AF0-BA57-431EFF75E38B" ); // Ministry Safe Training:Approve Request:Set Approver
            RockMigrationHelper.UpdateWorkflowActionType( "3EC65FD2-A676-4DD5-AA5B-1794662607C3", "Submit Request", 5, "38907A90-1634-4A93-8017-619326A4A582", true, true, "", "C3DFC12B-8091-4B80-8BF7-71167EF1A86B", 1, "Approve", "CFC5D3D0-9E4A-4240-894D-EF356996285C" ); // Ministry Safe Training:Approve Request:Submit Request
            RockMigrationHelper.UpdateWorkflowActionType( "3EC65FD2-A676-4DD5-AA5B-1794662607C3", "Deny Request", 6, "38907A90-1634-4A93-8017-619326A4A582", true, true, "", "C3DFC12B-8091-4B80-8BF7-71167EF1A86B", 1, "Deny", "8B789AD2-0CC1-49A1-9684-D3986E74404A" ); // Ministry Safe Training:Approve Request:Deny Request
            RockMigrationHelper.UpdateWorkflowActionType( "25B9CF22-C437-40C8-A88B-6C44AD22639B", "Set Status", 0, "96D371A7-A291-4F8F-8B38-B8F72CE5407E", true, false, "", "", 1, "", "F9C3639B-2284-414C-A05A-69BFF4E8420A" ); // Ministry Safe Training:Review Denial:Set Status
            RockMigrationHelper.UpdateWorkflowActionType( "25B9CF22-C437-40C8-A88B-6C44AD22639B", "Assign to Requester", 1, "F100A31F-E93A-4C7A-9E55-0FAF41A101C4", true, false, "", "", 1, "", "D3CD296E-3524-4F19-97D1-A846D9B6D019" ); // Ministry Safe Training:Review Denial:Assign to Requester
            RockMigrationHelper.UpdateWorkflowActionType( "25B9CF22-C437-40C8-A88B-6C44AD22639B", "Review", 2, "486DC4FA-FCBC-425F-90B0-E606DA8A9F68", true, false, "74D737C9-1275-4B56-8E59-A3D85116DB5C", "", 1, "", "3BDB0435-01B8-468F-8BE6-BCF0DC037166" ); // Ministry Safe Training:Review Denial:Review
            RockMigrationHelper.UpdateWorkflowActionType( "7AACF619-9882-4AA1-9522-ECF872482CFD", "Set Status", 0, "96D371A7-A291-4F8F-8B38-B8F72CE5407E", true, false, "", "", 1, "", "9483267F-A8FE-4AC9-8943-7158128AB85C" ); // Ministry Safe Training:Submit Request:Set Status
            RockMigrationHelper.UpdateWorkflowActionType( "7AACF619-9882-4AA1-9522-ECF872482CFD", "Submit Training", 1, "8A1F13F8-53F6-4108-BFC5-F2131EDFE8E2", true, false, "", "", 1, "", "A6232D45-AFF4-4A34-AE14-35383BA6C6C9" ); // Ministry Safe Training:Submit Request:Submit Training
            RockMigrationHelper.UpdateWorkflowActionType( "7AACF619-9882-4AA1-9522-ECF872482CFD", "Process Request Error", 2, "38907A90-1634-4A93-8017-619326A4A582", true, true, "", "A0C65BC0-C61D-419C-881B-2C6AAB5CE5E3", 32, "", "AFE931F6-973D-4E2F-991F-1AC7FC0713A9" ); // Ministry Safe Training:Submit Request:Process Request Error
            RockMigrationHelper.UpdateWorkflowActionType( "7AACF619-9882-4AA1-9522-ECF872482CFD", "Notify Person of Training", 3, "66197B01-D1F0-4924-A315-47AD54E030DE", true, false, "", "", 1, "", "CB64702D-0BBA-48FE-88E8-F1EAA1DED411" ); // Ministry Safe Training:Submit Request:Notify Person of Training
            RockMigrationHelper.UpdateWorkflowActionType( "7AACF619-9882-4AA1-9522-ECF872482CFD", "Retrieve Results", 4, "38907A90-1634-4A93-8017-619326A4A582", true, true, "", "", 1, "", "049D2EB6-023F-4B2A-AF43-ED8B7079E94B" ); // Ministry Safe Training:Submit Request:Retrieve Results
            RockMigrationHelper.UpdateWorkflowActionType( "B14EF83F-CECD-4864-9C38-2D3218103CB0", "Set Status", 0, "96D371A7-A291-4F8F-8B38-B8F72CE5407E", true, false, "", "", 1, "", "6CADE9FC-7D2E-4386-BD46-43187A515E18" ); // Ministry Safe Training:Request Error:Set Status
            RockMigrationHelper.UpdateWorkflowActionType( "B14EF83F-CECD-4864-9C38-2D3218103CB0", "Assign to Security", 1, "DB2D8C44-6E57-4B45-8973-5DE327D61554", true, false, "", "", 1, "", "0F0F1782-A3F1-494D-BE5E-C06EC68ECB0E" ); // Ministry Safe Training:Request Error:Assign to Security
            RockMigrationHelper.UpdateWorkflowActionType( "B14EF83F-CECD-4864-9C38-2D3218103CB0", "Display Error Message", 2, "486DC4FA-FCBC-425F-90B0-E606DA8A9F68", true, false, "AE2BC2CC-37C1-4D68-B429-228690EC81B1", "", 1, "", "EF5EF53B-C171-4897-AD96-858B7BEC7185" ); // Ministry Safe Training:Request Error:Display Error Message
            RockMigrationHelper.UpdateWorkflowActionType( "DEFEECE4-CBFF-42E5-BD56-D54217A1E2E4", "Wait", 0, "D22E73F7-86E2-46CA-AD5B-7770A866726B", true, false, "", "", 1, "", "2432FABB-EAED-4145-B45B-C41BEA53668D" ); // Ministry Safe Training:Retrieve Results:Wait
            RockMigrationHelper.UpdateWorkflowActionType( "DEFEECE4-CBFF-42E5-BD56-D54217A1E2E4", "Get Training Results", 1, "481197B7-50AD-4E7D-BD16-F9B2858891A5", true, false, "", "", 1, "", "457C69D7-0D0E-44BF-B260-813E6DC6733B" ); // Ministry Safe Training:Retrieve Results:Get Training Results
            RockMigrationHelper.UpdateWorkflowActionType( "DEFEECE4-CBFF-42E5-BD56-D54217A1E2E4", "Activate Complete", 2, "38907A90-1634-4A93-8017-619326A4A582", true, true, "", "B23AB632-B01C-445A-8362-68BA226A7D4E", 64, "", "6C1CDD26-0DF3-42D1-A600-A6C545E52F1E" ); // Ministry Safe Training:Retrieve Results:Activate Complete
            RockMigrationHelper.UpdateWorkflowActionType( "DEFEECE4-CBFF-42E5-BD56-D54217A1E2E4", "Reactivate Activity", 3, "699756EF-28EB-444B-BD28-15F0A167E614", false, false, "", "", 1, "", "ECBDA381-0624-4EC6-8801-1B2DABCDC86E" ); // Ministry Safe Training:Retrieve Results:Reactivate Activity
            RockMigrationHelper.UpdateWorkflowActionType( "4E3D92C6-C186-4D9A-9888-D49F607E5F79", "Update Training Date", 0, "320622DA-52E0-41AE-AF90-2BF78B488552", true, false, "", "", 1, "", "0162FBEA-E5A9-4DC7-95D0-97E9D2D0959D" ); // Ministry Safe Training:Complete Request:Update Training Date
            RockMigrationHelper.UpdateWorkflowActionType( "4E3D92C6-C186-4D9A-9888-D49F607E5F79", "Update Training Type", 1, "320622DA-52E0-41AE-AF90-2BF78B488552", true, false, "", "", 1, "", "6609D32A-1933-424E-B9A3-808AE6D2D138" ); // Ministry Safe Training:Complete Request:Update Training Type
            RockMigrationHelper.UpdateWorkflowActionType( "4E3D92C6-C186-4D9A-9888-D49F607E5F79", "Update Training Score", 2, "320622DA-52E0-41AE-AF90-2BF78B488552", true, false, "", "", 1, "", "D1F18CF7-99EF-4780-BB9D-94A31559ED7C" ); // Ministry Safe Training:Complete Request:Update Training Score
            RockMigrationHelper.UpdateWorkflowActionType( "4E3D92C6-C186-4D9A-9888-D49F607E5F79", "Update Training Result", 3, "320622DA-52E0-41AE-AF90-2BF78B488552", true, false, "", "", 1, "", "5B283518-A45F-4847-B9C8-1264A232F995" ); // Ministry Safe Training:Complete Request:Update Training Result
            RockMigrationHelper.UpdateWorkflowActionType( "4E3D92C6-C186-4D9A-9888-D49F607E5F79", "Notify Requester", 4, "66197B01-D1F0-4924-A315-47AD54E030DE", true, false, "", "", 1, "", "06ECCE5A-4E73-47B7-882C-30AEFB665A1C" ); // Ministry Safe Training:Complete Request:Notify Requester
            RockMigrationHelper.UpdateWorkflowActionType( "4E3D92C6-C186-4D9A-9888-D49F607E5F79", "Complete Workflow", 5, "EEDA4318-F014-4A46-9C76-4C052EF81AA1", true, false, "", "", 1, "", "6DF84C34-012B-43EE-BFC9-65071E4794A2" ); // Ministry Safe Training:Complete Request:Complete Workflow
            RockMigrationHelper.UpdateWorkflowActionType( "120D9EB5-7A0C-4416-86DE-F3FC62856824", "Delete Workflow", 0, "0E79AF40-4FB0-49D7-AB0E-E95BD828C62D", true, false, "", "", 1, "", "3B9173EC-7038-4AFC-BC69-324C4EBDBC43" ); // Ministry Safe Training:Cancel Request:Delete Workflow
            RockMigrationHelper.AddActionTypeAttributeValue( "EC0DABEB-32A8-4338-9A07-AF4AEE4C95C2", "36CE41F4-4C87-4096-B0C6-8269163BCC0A", @"False" ); // Ministry Safe Training:Initial Request:Set Status:Active
            RockMigrationHelper.AddActionTypeAttributeValue( "EC0DABEB-32A8-4338-9A07-AF4AEE4C95C2", "91A9F4BE-4A8E-430A-B466-A88DB2D33B34", @"Initial Entry" ); // Ministry Safe Training:Initial Request:Set Status:Status
            RockMigrationHelper.AddActionTypeAttributeValue( "EC0DABEB-32A8-4338-9A07-AF4AEE4C95C2", "AE8C180C-E370-414A-B10D-97891B95D105", @"" ); // Ministry Safe Training:Initial Request:Set Status:Order
            RockMigrationHelper.AddActionTypeAttributeValue( "AE894258-89DB-4890-A0A0-4D0E1743D50C", "9392E3D7-A28B-4CD8-8B03-5E147B102EF1", @"False" ); // Ministry Safe Training:Initial Request:Set Person:Active
            RockMigrationHelper.AddActionTypeAttributeValue( "AE894258-89DB-4890-A0A0-4D0E1743D50C", "AD4EFAC4-E687-43DF-832F-0DC3856ABABB", @"" ); // Ministry Safe Training:Initial Request:Set Person:Order
            RockMigrationHelper.AddActionTypeAttributeValue( "AE894258-89DB-4890-A0A0-4D0E1743D50C", "61E6E1BC-E657-4F00-B2E9-769AAA25B9F7", @"6691e18e-d164-4875-9f10-137180c02d27" ); // Ministry Safe Training:Initial Request:Set Person:Attribute
            RockMigrationHelper.AddActionTypeAttributeValue( "AE894258-89DB-4890-A0A0-4D0E1743D50C", "B524B00C-29CB-49E9-9896-8BB60F209783", @"True" ); // Ministry Safe Training:Initial Request:Set Person:Entity Is Required
            RockMigrationHelper.AddActionTypeAttributeValue( "AE894258-89DB-4890-A0A0-4D0E1743D50C", "1246C53A-FD92-4E08-ABDE-9A6C37E70C7B", @"True" ); // Ministry Safe Training:Initial Request:Set Person:Use Id instead of Guid
            RockMigrationHelper.AddActionTypeAttributeValue( "AE894258-89DB-4890-A0A0-4D0E1743D50C", "7D79FC31-D0ED-4DB0-AB7D-60F4F98A1199", @"" ); // Ministry Safe Training:Initial Request:Set Person:Lava Template
            RockMigrationHelper.AddActionTypeAttributeValue( "B33C6899-59F3-4C8A-BDB7-4553C8FB03A9", "0A800013-51F7-4902-885A-5BE215D67D3D", @"False" ); // Ministry Safe Training:Initial Request:Set Name:Active
            RockMigrationHelper.AddActionTypeAttributeValue( "B33C6899-59F3-4C8A-BDB7-4553C8FB03A9", "5D95C15A-CCAE-40AD-A9DD-F929DA587115", @"" ); // Ministry Safe Training:Initial Request:Set Name:Order
            RockMigrationHelper.AddActionTypeAttributeValue( "B33C6899-59F3-4C8A-BDB7-4553C8FB03A9", "93852244-A667-4749-961A-D47F88675BE4", @"6691e18e-d164-4875-9f10-137180c02d27" ); // Ministry Safe Training:Initial Request:Set Name:Text Value|Attribute Value
            RockMigrationHelper.AddActionTypeAttributeValue( "32C29676-F7DE-40CB-A3F3-09E35F87BE4E", "DE9CB292-4785-4EA3-976D-3826F91E9E98", @"False" ); // Ministry Safe Training:Initial Request:Set Requester:Active
            RockMigrationHelper.AddActionTypeAttributeValue( "32C29676-F7DE-40CB-A3F3-09E35F87BE4E", "BBED8A83-8BB2-4D35-BAFB-05F67DCAD112", @"1c23302e-49bd-432b-9440-e42d289e5d69" ); // Ministry Safe Training:Initial Request:Set Requester:Person Attribute
            RockMigrationHelper.AddActionTypeAttributeValue( "32C29676-F7DE-40CB-A3F3-09E35F87BE4E", "89E9BCED-91AB-47B0-AD52-D78B0B7CB9E8", @"" ); // Ministry Safe Training:Initial Request:Set Requester:Order
            RockMigrationHelper.AddActionTypeAttributeValue( "DFEFF167-F0E1-498F-A2CD-7DDEF9C752C3", "F3B9908B-096F-460B-8320-122CF046D1F9", @"SELECT ISNULL( (     SELECT          CASE WHEN DATEADD(year, 1, AV.[ValueAsDateTime]) > GETDATE() THEN 'True' ELSE 'False' END     FROM [AttributeValue] AV         INNER JOIN [Attribute] A ON A.[Id] = AV.[AttributeId]         INNER JOIN [PersonAlias] P ON P.[PersonId] = AV.[EntityId]     WHERE AV.[ValueAsDateTime] IS NOT NULL         AND A.[Guid] = '{{ Workflow | Attribute:'TrainingDateAttribute','RawValue' }}'         AND P.[Guid] = '{{ Workflow | Attribute:'Person','RawValue' }}' ), 'False')" ); // Ministry Safe Training:Initial Request:Set Warning:SQLQuery
            RockMigrationHelper.AddActionTypeAttributeValue( "DFEFF167-F0E1-498F-A2CD-7DDEF9C752C3", "A18C3143-0586-4565-9F36-E603BC674B4E", @"False" ); // Ministry Safe Training:Initial Request:Set Warning:Active
            RockMigrationHelper.AddActionTypeAttributeValue( "DFEFF167-F0E1-498F-A2CD-7DDEF9C752C3", "FA7C685D-8636-41EF-9998-90FFF3998F76", @"" ); // Ministry Safe Training:Initial Request:Set Warning:Order
            RockMigrationHelper.AddActionTypeAttributeValue( "DFEFF167-F0E1-498F-A2CD-7DDEF9C752C3", "56997192-2545-4EA1-B5B2-313B04588984", @"eb0a5b98-fafd-45a4-9990-fba732253351" ); // Ministry Safe Training:Initial Request:Set Warning:Result Attribute
            RockMigrationHelper.AddActionTypeAttributeValue( "DFEFF167-F0E1-498F-A2CD-7DDEF9C752C3", "9BCD10A8-FDCE-44A9-9CC8-72D935B5E974", @"False" ); // Ministry Safe Training:Initial Request:Set Warning:Continue On Error
            RockMigrationHelper.AddActionTypeAttributeValue( "5030F5FD-BC20-46D0-A19F-CEAD3FFF3B22", "E8ABD802-372C-47BE-82B1-96F50DB5169E", @"False" ); // Ministry Safe Training:Initial Request:Move to Approve Request from Connection:Active
            RockMigrationHelper.AddActionTypeAttributeValue( "5030F5FD-BC20-46D0-A19F-CEAD3FFF3B22", "3809A78C-B773-440C-8E3F-A8E81D0DAE08", @"" ); // Ministry Safe Training:Initial Request:Move to Approve Request from Connection:Order
            RockMigrationHelper.AddActionTypeAttributeValue( "5030F5FD-BC20-46D0-A19F-CEAD3FFF3B22", "02D5A7A5-8781-46B4-B9FC-AF816829D240", @"3EC65FD2-A676-4DD5-AA5B-1794662607C3" ); // Ministry Safe Training:Initial Request:Move to Approve Request from Connection:Activity
            RockMigrationHelper.AddActionTypeAttributeValue( "AD11FE97-FB1D-4EAC-AD72-B1AF19DDEE75", "234910F2-A0DB-4D7D-BAF7-83C880EF30AE", @"False" ); // Ministry Safe Training:Initial Request:Get Details:Active
            RockMigrationHelper.AddActionTypeAttributeValue( "AD11FE97-FB1D-4EAC-AD72-B1AF19DDEE75", "C178113D-7C86-4229-8424-C6D0CF4A7E23", @"" ); // Ministry Safe Training:Initial Request:Get Details:Order
            RockMigrationHelper.AddActionTypeAttributeValue( "EFD8C68B-E372-47B9-9A85-4E87A0D62BF3", "36CE41F4-4C87-4096-B0C6-8269163BCC0A", @"False" ); // Ministry Safe Training:Approve Request:Set Status:Active
            RockMigrationHelper.AddActionTypeAttributeValue( "EFD8C68B-E372-47B9-9A85-4E87A0D62BF3", "91A9F4BE-4A8E-430A-B466-A88DB2D33B34", @"Waiting for Submit Approval" ); // Ministry Safe Training:Approve Request:Set Status:Status
            RockMigrationHelper.AddActionTypeAttributeValue( "EFD8C68B-E372-47B9-9A85-4E87A0D62BF3", "AE8C180C-E370-414A-B10D-97891B95D105", @"" ); // Ministry Safe Training:Approve Request:Set Status:Order
            RockMigrationHelper.AddActionTypeAttributeValue( "BDA8B012-FB7D-4139-BD2B-AAB7B4322825", "E5BAC4A6-FF7F-4016-BA9C-72D16CB60184", @"False" ); // Ministry Safe Training:Approve Request:Set Pending Training Result Person Attribute Value:Active
            RockMigrationHelper.AddActionTypeAttributeValue( "BDA8B012-FB7D-4139-BD2B-AAB7B4322825", "E456FB6F-05DB-4826-A612-5B704BC4EA13", @"6691e18e-d164-4875-9f10-137180c02d27" ); // Ministry Safe Training:Approve Request:Set Pending Training Result Person Attribute Value:Person
            RockMigrationHelper.AddActionTypeAttributeValue( "BDA8B012-FB7D-4139-BD2B-AAB7B4322825", "3F3BF3E6-AD53-491E-A40F-441F2AFCBB5B", @"" ); // Ministry Safe Training:Approve Request:Set Pending Training Result Person Attribute Value:Order
            RockMigrationHelper.AddActionTypeAttributeValue( "BDA8B012-FB7D-4139-BD2B-AAB7B4322825", "8F4BB00F-7FA2-41AD-8E90-81F4DFE2C762", @"db9d8876-4550-424d-a019-1aa71f8adc1d" ); // Ministry Safe Training:Approve Request:Set Pending Training Result Person Attribute Value:Person Attribute
            RockMigrationHelper.AddActionTypeAttributeValue( "BDA8B012-FB7D-4139-BD2B-AAB7B4322825", "94689BDE-493E-4869-A614-2D54822D747C", @"Pending" ); // Ministry Safe Training:Approve Request:Set Pending Training Result Person Attribute Value:Value|Attribute Value
            RockMigrationHelper.AddActionTypeAttributeValue( "FD90EF54-7443-4784-BB4A-D3C6F7D96D13", "27BAC9C8-2BF7-405A-AA01-845A3D374295", @"False" ); // Ministry Safe Training:Approve Request:Assign to Security:Active
            RockMigrationHelper.AddActionTypeAttributeValue( "FD90EF54-7443-4784-BB4A-D3C6F7D96D13", "D53823A1-28CB-4BA0-A24C-873ECF4079C5", @"a6bcc49e-103f-46b0-8bac-84ea03ff04d5" ); // Ministry Safe Training:Approve Request:Assign to Security:Security Role
            RockMigrationHelper.AddActionTypeAttributeValue( "FD90EF54-7443-4784-BB4A-D3C6F7D96D13", "120D39B5-8D2A-4B96-9419-C73BE0F2451A", @"" ); // Ministry Safe Training:Approve Request:Assign to Security:Order
            RockMigrationHelper.AddActionTypeAttributeValue( "A2BE8E3A-BE01-4679-9B10-519FA72E1870", "234910F2-A0DB-4D7D-BAF7-83C880EF30AE", @"False" ); // Ministry Safe Training:Approve Request:Approve or Deny:Active
            RockMigrationHelper.AddActionTypeAttributeValue( "A2BE8E3A-BE01-4679-9B10-519FA72E1870", "C178113D-7C86-4229-8424-C6D0CF4A7E23", @"" ); // Ministry Safe Training:Approve Request:Approve or Deny:Order
            RockMigrationHelper.AddActionTypeAttributeValue( "E49E0381-791E-4AF0-BA57-431EFF75E38B", "DE9CB292-4785-4EA3-976D-3826F91E9E98", @"False" ); // Ministry Safe Training:Approve Request:Set Approver:Active
            RockMigrationHelper.AddActionTypeAttributeValue( "E49E0381-791E-4AF0-BA57-431EFF75E38B", "BBED8A83-8BB2-4D35-BAFB-05F67DCAD112", @"083b9e1c-dedd-4fa2-af11-ad2a160b523c" ); // Ministry Safe Training:Approve Request:Set Approver:Person Attribute
            RockMigrationHelper.AddActionTypeAttributeValue( "E49E0381-791E-4AF0-BA57-431EFF75E38B", "89E9BCED-91AB-47B0-AD52-D78B0B7CB9E8", @"" ); // Ministry Safe Training:Approve Request:Set Approver:Order
            RockMigrationHelper.AddActionTypeAttributeValue( "CFC5D3D0-9E4A-4240-894D-EF356996285C", "E8ABD802-372C-47BE-82B1-96F50DB5169E", @"False" ); // Ministry Safe Training:Approve Request:Submit Request:Active
            RockMigrationHelper.AddActionTypeAttributeValue( "CFC5D3D0-9E4A-4240-894D-EF356996285C", "3809A78C-B773-440C-8E3F-A8E81D0DAE08", @"" ); // Ministry Safe Training:Approve Request:Submit Request:Order
            RockMigrationHelper.AddActionTypeAttributeValue( "CFC5D3D0-9E4A-4240-894D-EF356996285C", "02D5A7A5-8781-46B4-B9FC-AF816829D240", @"7AACF619-9882-4AA1-9522-ECF872482CFD" ); // Ministry Safe Training:Approve Request:Submit Request:Activity
            RockMigrationHelper.AddActionTypeAttributeValue( "8B789AD2-0CC1-49A1-9684-D3986E74404A", "E8ABD802-372C-47BE-82B1-96F50DB5169E", @"False" ); // Ministry Safe Training:Approve Request:Deny Request:Active
            RockMigrationHelper.AddActionTypeAttributeValue( "8B789AD2-0CC1-49A1-9684-D3986E74404A", "3809A78C-B773-440C-8E3F-A8E81D0DAE08", @"" ); // Ministry Safe Training:Approve Request:Deny Request:Order
            RockMigrationHelper.AddActionTypeAttributeValue( "8B789AD2-0CC1-49A1-9684-D3986E74404A", "02D5A7A5-8781-46B4-B9FC-AF816829D240", @"25B9CF22-C437-40C8-A88B-6C44AD22639B" ); // Ministry Safe Training:Approve Request:Deny Request:Activity
            RockMigrationHelper.AddActionTypeAttributeValue( "F9C3639B-2284-414C-A05A-69BFF4E8420A", "36CE41F4-4C87-4096-B0C6-8269163BCC0A", @"False" ); // Ministry Safe Training:Review Denial:Set Status:Active
            RockMigrationHelper.AddActionTypeAttributeValue( "F9C3639B-2284-414C-A05A-69BFF4E8420A", "91A9F4BE-4A8E-430A-B466-A88DB2D33B34", @"Waiting for More Details" ); // Ministry Safe Training:Review Denial:Set Status:Status
            RockMigrationHelper.AddActionTypeAttributeValue( "F9C3639B-2284-414C-A05A-69BFF4E8420A", "AE8C180C-E370-414A-B10D-97891B95D105", @"" ); // Ministry Safe Training:Review Denial:Set Status:Order
            RockMigrationHelper.AddActionTypeAttributeValue( "D3CD296E-3524-4F19-97D1-A846D9B6D019", "E0F7AB7E-7761-4600-A099-CB14ACDBF6EF", @"False" ); // Ministry Safe Training:Review Denial:Assign to Requester:Active
            RockMigrationHelper.AddActionTypeAttributeValue( "D3CD296E-3524-4F19-97D1-A846D9B6D019", "FBADD25F-D309-4512-8430-3CC8615DD60E", @"1c23302e-49bd-432b-9440-e42d289e5d69" ); // Ministry Safe Training:Review Denial:Assign to Requester:Attribute
            RockMigrationHelper.AddActionTypeAttributeValue( "D3CD296E-3524-4F19-97D1-A846D9B6D019", "7A6B605D-7FB1-4F48-AF35-5A0683FB1CDA", @"" ); // Ministry Safe Training:Review Denial:Assign to Requester:Order
            RockMigrationHelper.AddActionTypeAttributeValue( "3BDB0435-01B8-468F-8BE6-BCF0DC037166", "234910F2-A0DB-4D7D-BAF7-83C880EF30AE", @"False" ); // Ministry Safe Training:Review Denial:Review:Active
            RockMigrationHelper.AddActionTypeAttributeValue( "3BDB0435-01B8-468F-8BE6-BCF0DC037166", "C178113D-7C86-4229-8424-C6D0CF4A7E23", @"" ); // Ministry Safe Training:Review Denial:Review:Order
            RockMigrationHelper.AddActionTypeAttributeValue( "9483267F-A8FE-4AC9-8943-7158128AB85C", "36CE41F4-4C87-4096-B0C6-8269163BCC0A", @"False" ); // Ministry Safe Training:Submit Request:Set Status:Active
            RockMigrationHelper.AddActionTypeAttributeValue( "9483267F-A8FE-4AC9-8943-7158128AB85C", "91A9F4BE-4A8E-430A-B466-A88DB2D33B34", @"Waiting for Result" ); // Ministry Safe Training:Submit Request:Set Status:Status
            RockMigrationHelper.AddActionTypeAttributeValue( "9483267F-A8FE-4AC9-8943-7158128AB85C", "AE8C180C-E370-414A-B10D-97891B95D105", @"" ); // Ministry Safe Training:Submit Request:Set Status:Order
            RockMigrationHelper.AddActionTypeAttributeValue( "A6232D45-AFF4-4A34-AE14-35383BA6C6C9", "7B93916A-F91D-49AC-965E-BF78A584E248", @"6691e18e-d164-4875-9f10-137180c02d27" ); // Ministry Safe Training:Submit Request:Submit Training:Person
            RockMigrationHelper.AddActionTypeAttributeValue( "A6232D45-AFF4-4A34-AE14-35383BA6C6C9", "C369F3F1-C34B-433B-88EA-B318DDB47181", @"" ); // Ministry Safe Training:Submit Request:Submit Training:API Key
            RockMigrationHelper.AddActionTypeAttributeValue( "A6232D45-AFF4-4A34-AE14-35383BA6C6C9", "CEFB2D83-187E-4C42-B0D2-B2133CB8E713", @"" ); // Ministry Safe Training:Submit Request:Submit Training:Order
            RockMigrationHelper.AddActionTypeAttributeValue( "A6232D45-AFF4-4A34-AE14-35383BA6C6C9", "09B52E96-E13C-4989-8E42-5F9C3E8AF31A", @"False" ); // Ministry Safe Training:Submit Request:Submit Training:Active
            RockMigrationHelper.AddActionTypeAttributeValue( "A6232D45-AFF4-4A34-AE14-35383BA6C6C9", "834BD487-ADE7-4C3C-9AC6-BF2EC9548CD0", @"True" ); // Ministry Safe Training:Submit Request:Submit Training:Staging Mode
            RockMigrationHelper.AddActionTypeAttributeValue( "A6232D45-AFF4-4A34-AE14-35383BA6C6C9", "80180219-80F7-4D45-BF4F-006CC2AD5639", @"a0c65bc0-c61d-419c-881b-2c6aab5ce5e3" ); // Ministry Safe Training:Submit Request:Submit Training:Ministry Safe Id
            RockMigrationHelper.AddActionTypeAttributeValue( "A6232D45-AFF4-4A34-AE14-35383BA6C6C9", "F677B2FF-BBB4-4BDE-8F9C-CF474CEBF52B", @"c9911445-c2aa-47d4-b051-93b42128e35d" ); // Ministry Safe Training:Submit Request:Submit Training:Direct Login Url
            RockMigrationHelper.AddActionTypeAttributeValue( "A6232D45-AFF4-4A34-AE14-35383BA6C6C9", "092B8D61-6D25-4811-9F78-8E6FCAF2D2C9", @"True" ); // Ministry Safe Training:Submit Request:Submit Training:Use Workflow Id
            RockMigrationHelper.AddActionTypeAttributeValue( "A6232D45-AFF4-4A34-AE14-35383BA6C6C9", "43C7560D-02B7-4ADF-8720-2CC8923F9C05", @"fae024da-3908-449a-a4f5-882cb7277cc1" ); // Ministry Safe Training:Submit Request:Submit Training:Training Type|Attribute Value
            RockMigrationHelper.AddActionTypeAttributeValue( "AFE931F6-973D-4E2F-991F-1AC7FC0713A9", "E8ABD802-372C-47BE-82B1-96F50DB5169E", @"False" ); // Ministry Safe Training:Submit Request:Process Request Error:Active
            RockMigrationHelper.AddActionTypeAttributeValue( "AFE931F6-973D-4E2F-991F-1AC7FC0713A9", "3809A78C-B773-440C-8E3F-A8E81D0DAE08", @"" ); // Ministry Safe Training:Submit Request:Process Request Error:Order
            RockMigrationHelper.AddActionTypeAttributeValue( "AFE931F6-973D-4E2F-991F-1AC7FC0713A9", "02D5A7A5-8781-46B4-B9FC-AF816829D240", @"B14EF83F-CECD-4864-9C38-2D3218103CB0" ); // Ministry Safe Training:Submit Request:Process Request Error:Activity
            RockMigrationHelper.AddActionTypeAttributeValue( "CB64702D-0BBA-48FE-88E8-F1EAA1DED411", "36197160-7D3D-490D-AB42-7E29105AFE91", @"False" ); // Ministry Safe Training:Submit Request:Notify Person of Training:Active
            RockMigrationHelper.AddActionTypeAttributeValue( "CB64702D-0BBA-48FE-88E8-F1EAA1DED411", "D1269254-C15A-40BD-B784-ADCC231D3950", @"" ); // Ministry Safe Training:Submit Request:Notify Person of Training:Order
            RockMigrationHelper.AddActionTypeAttributeValue( "CB64702D-0BBA-48FE-88E8-F1EAA1DED411", "9F5F7CEC-F369-4FDF-802A-99074CE7A7FC", @"" ); // Ministry Safe Training:Submit Request:Notify Person of Training:From Email Address|Attribute Value
            RockMigrationHelper.AddActionTypeAttributeValue( "CB64702D-0BBA-48FE-88E8-F1EAA1DED411", "0C4C13B8-7076-4872-925A-F950886B5E16", @"6691e18e-d164-4875-9f10-137180c02d27" ); // Ministry Safe Training:Submit Request:Notify Person of Training:Send To Email Addresses|Attribute Value
            RockMigrationHelper.AddActionTypeAttributeValue( "CB64702D-0BBA-48FE-88E8-F1EAA1DED411", "E3667110-339F-4FE3-B6B7-084CF9633580", @"" ); // Ministry Safe Training:Submit Request:Notify Person of Training:Send to Group Role
            RockMigrationHelper.AddActionTypeAttributeValue( "CB64702D-0BBA-48FE-88E8-F1EAA1DED411", "5D9B13B6-CD96-4C7C-86FA-4512B9D28386", @"Ministry Safe Training Request" ); // Ministry Safe Training:Submit Request:Notify Person of Training:Subject
            RockMigrationHelper.AddActionTypeAttributeValue( "CB64702D-0BBA-48FE-88E8-F1EAA1DED411", "4D245B9E-6B03-46E7-8482-A51FBA190E4D", @"{{ 'Global' | Attribute:'EmailHeader' }}  <p>{{ Person.FirstName }},</p> <p>{{ 'Global' | Attribute:'OrganizationName' }} has requested you to take a {{ Workflow.TrainingType }}.<p/> <p>Please click the following link to begin training:><br> <a href=""{{ Workflow.DirectLoginUrl }}"">{{ Workflow.DirectLoginUrl }}</a></p>  {{ 'Global' | Attribute:'EmailFooter' }}" ); // Ministry Safe Training:Submit Request:Notify Person of Training:Body
            RockMigrationHelper.AddActionTypeAttributeValue( "CB64702D-0BBA-48FE-88E8-F1EAA1DED411", "1BDC7ACA-9A0B-4C8A-909E-8B4143D9C2A3", @"False" ); // Ministry Safe Training:Submit Request:Notify Person of Training:Save Communication History
            RockMigrationHelper.AddActionTypeAttributeValue( "049D2EB6-023F-4B2A-AF43-ED8B7079E94B", "E8ABD802-372C-47BE-82B1-96F50DB5169E", @"False" ); // Ministry Safe Training:Submit Request:Retrieve Results:Active
            RockMigrationHelper.AddActionTypeAttributeValue( "049D2EB6-023F-4B2A-AF43-ED8B7079E94B", "3809A78C-B773-440C-8E3F-A8E81D0DAE08", @"" ); // Ministry Safe Training:Submit Request:Retrieve Results:Order
            RockMigrationHelper.AddActionTypeAttributeValue( "049D2EB6-023F-4B2A-AF43-ED8B7079E94B", "02D5A7A5-8781-46B4-B9FC-AF816829D240", @"DEFEECE4-CBFF-42E5-BD56-D54217A1E2E4" ); // Ministry Safe Training:Submit Request:Retrieve Results:Activity
            RockMigrationHelper.AddActionTypeAttributeValue( "6CADE9FC-7D2E-4386-BD46-43187A515E18", "36CE41F4-4C87-4096-B0C6-8269163BCC0A", @"False" ); // Ministry Safe Training:Request Error:Set Status:Active
            RockMigrationHelper.AddActionTypeAttributeValue( "6CADE9FC-7D2E-4386-BD46-43187A515E18", "91A9F4BE-4A8E-430A-B466-A88DB2D33B34", @"Request Error" ); // Ministry Safe Training:Request Error:Set Status:Status
            RockMigrationHelper.AddActionTypeAttributeValue( "6CADE9FC-7D2E-4386-BD46-43187A515E18", "AE8C180C-E370-414A-B10D-97891B95D105", @"" ); // Ministry Safe Training:Request Error:Set Status:Order
            RockMigrationHelper.AddActionTypeAttributeValue( "0F0F1782-A3F1-494D-BE5E-C06EC68ECB0E", "C0D75D1A-16C5-4786-A1E0-25669BEE8FE9", @"False" ); // Ministry Safe Training:Request Error:Assign to Security:Active
            RockMigrationHelper.AddActionTypeAttributeValue( "0F0F1782-A3F1-494D-BE5E-C06EC68ECB0E", "041B7B51-A694-4AF5-B455-64D0DE7160A2", @"" ); // Ministry Safe Training:Request Error:Assign to Security:Order
            RockMigrationHelper.AddActionTypeAttributeValue( "0F0F1782-A3F1-494D-BE5E-C06EC68ECB0E", "BBFAD050-5968-4D11-8887-2FF877D8C8AB", @"aece949f-704c-483e-a4fb-93d5e4720c4c|a6bcc49e-103f-46b0-8bac-84ea03ff04d5" ); // Ministry Safe Training:Request Error:Assign to Security:Group
            RockMigrationHelper.AddActionTypeAttributeValue( "EF5EF53B-C171-4897-AD96-858B7BEC7185", "234910F2-A0DB-4D7D-BAF7-83C880EF30AE", @"False" ); // Ministry Safe Training:Request Error:Display Error Message:Active
            RockMigrationHelper.AddActionTypeAttributeValue( "EF5EF53B-C171-4897-AD96-858B7BEC7185", "C178113D-7C86-4229-8424-C6D0CF4A7E23", @"" ); // Ministry Safe Training:Request Error:Display Error Message:Order
            RockMigrationHelper.AddActionTypeAttributeValue( "2432FABB-EAED-4145-B45B-C41BEA53668D", "97AF3FEA-C4C0-4A1C-A366-4AC0DF1E414A", @"1" ); // Ministry Safe Training:Retrieve Results:Wait:Minutes To Delay
            RockMigrationHelper.AddActionTypeAttributeValue( "2432FABB-EAED-4145-B45B-C41BEA53668D", "A7EC0FF2-E30A-4122-840E-2361EEB4894F", @"" ); // Ministry Safe Training:Retrieve Results:Wait:Order
            RockMigrationHelper.AddActionTypeAttributeValue( "2432FABB-EAED-4145-B45B-C41BEA53668D", "2B67B855-BFC7-4AAF-B083-8BA06E9CD9CC", @"False" ); // Ministry Safe Training:Retrieve Results:Wait:Active
            RockMigrationHelper.AddActionTypeAttributeValue( "2432FABB-EAED-4145-B45B-C41BEA53668D", "979680E3-E7AF-493E-8C65-EE53D3C81820", @"" ); // Ministry Safe Training:Retrieve Results:Wait:Date In Attribute
            RockMigrationHelper.AddActionTypeAttributeValue( "2432FABB-EAED-4145-B45B-C41BEA53668D", "6DEE2D98-D09C-4F0D-BA05-2F28121F88CF", @"" ); // Ministry Safe Training:Retrieve Results:Wait:Next Weekday
            RockMigrationHelper.AddActionTypeAttributeValue( "457C69D7-0D0E-44BF-B260-813E6DC6733B", "71E73409-2A9A-469B-BE40-74379D7DCD2D", @"a0c65bc0-c61d-419c-881b-2c6aab5ce5e3" ); // Ministry Safe Training:Retrieve Results:Get Training Results:Ministry Safe Id
            RockMigrationHelper.AddActionTypeAttributeValue( "457C69D7-0D0E-44BF-B260-813E6DC6733B", "656B34CE-79AF-4FEE-952D-A129A433C063", @"" ); // Ministry Safe Training:Retrieve Results:Get Training Results:API Key
            RockMigrationHelper.AddActionTypeAttributeValue( "457C69D7-0D0E-44BF-B260-813E6DC6733B", "2694605F-69D1-4403-B5FD-F5234500FD4D", @"" ); // Ministry Safe Training:Retrieve Results:Get Training Results:Order
            RockMigrationHelper.AddActionTypeAttributeValue( "457C69D7-0D0E-44BF-B260-813E6DC6733B", "3167CFBB-E14D-4CFE-9627-B2F366BD0CB9", @"False" ); // Ministry Safe Training:Retrieve Results:Get Training Results:Active
            RockMigrationHelper.AddActionTypeAttributeValue( "457C69D7-0D0E-44BF-B260-813E6DC6733B", "227E8888-AFB9-4745-9EFD-4F6CAF487996", @"True" ); // Ministry Safe Training:Retrieve Results:Get Training Results:Staging Mode
            RockMigrationHelper.AddActionTypeAttributeValue( "457C69D7-0D0E-44BF-B260-813E6DC6733B", "0FA480DA-C75F-43E4-94F1-76921670B798", @"c259e954-3fdd-4cad-ac43-e0a0d68225ef" ); // Ministry Safe Training:Retrieve Results:Get Training Results:Date Completed
            RockMigrationHelper.AddActionTypeAttributeValue( "457C69D7-0D0E-44BF-B260-813E6DC6733B", "572FE7DC-50D7-4C28-9CA9-D89F8A8E12B5", @"b23ab632-b01c-445a-8362-68ba226a7d4e" ); // Ministry Safe Training:Retrieve Results:Get Training Results:Score
            RockMigrationHelper.AddActionTypeAttributeValue( "457C69D7-0D0E-44BF-B260-813E6DC6733B", "92BCAFAE-075B-4BA9-BE5F-A941A0EC3F0C", @"70" ); // Ministry Safe Training:Retrieve Results:Get Training Results:Pass Score
            RockMigrationHelper.AddActionTypeAttributeValue( "457C69D7-0D0E-44BF-B260-813E6DC6733B", "A443807B-07A4-4FB2-AA28-84954B455218", @"aa0ce3d6-1fec-48a6-aee0-97410bbdbc78" ); // Ministry Safe Training:Retrieve Results:Get Training Results:Result
            RockMigrationHelper.AddActionTypeAttributeValue( "6C1CDD26-0DF3-42D1-A600-A6C545E52F1E", "E8ABD802-372C-47BE-82B1-96F50DB5169E", @"False" ); // Ministry Safe Training:Retrieve Results:Activate Complete:Active
            RockMigrationHelper.AddActionTypeAttributeValue( "6C1CDD26-0DF3-42D1-A600-A6C545E52F1E", "3809A78C-B773-440C-8E3F-A8E81D0DAE08", @"" ); // Ministry Safe Training:Retrieve Results:Activate Complete:Order
            RockMigrationHelper.AddActionTypeAttributeValue( "6C1CDD26-0DF3-42D1-A600-A6C545E52F1E", "02D5A7A5-8781-46B4-B9FC-AF816829D240", @"4E3D92C6-C186-4D9A-9888-D49F607E5F79" ); // Ministry Safe Training:Retrieve Results:Activate Complete:Activity
            RockMigrationHelper.AddActionTypeAttributeValue( "ECBDA381-0624-4EC6-8801-1B2DABCDC86E", "A134F1A7-3824-43E0-9EB1-22C899B795BD", @"False" ); // Ministry Safe Training:Retrieve Results:Reactivate Activity:Active
            RockMigrationHelper.AddActionTypeAttributeValue( "ECBDA381-0624-4EC6-8801-1B2DABCDC86E", "5DA71523-E8B0-4C4D-89A4-B47945A22A0C", @"" ); // Ministry Safe Training:Retrieve Results:Reactivate Activity:Order
            RockMigrationHelper.AddActionTypeAttributeValue( "0162FBEA-E5A9-4DC7-95D0-97E9D2D0959D", "E5BAC4A6-FF7F-4016-BA9C-72D16CB60184", @"False" ); // Ministry Safe Training:Complete Request:Update Training Date:Active
            RockMigrationHelper.AddActionTypeAttributeValue( "0162FBEA-E5A9-4DC7-95D0-97E9D2D0959D", "E456FB6F-05DB-4826-A612-5B704BC4EA13", @"6691e18e-d164-4875-9f10-137180c02d27" ); // Ministry Safe Training:Complete Request:Update Training Date:Person
            RockMigrationHelper.AddActionTypeAttributeValue( "0162FBEA-E5A9-4DC7-95D0-97E9D2D0959D", "3F3BF3E6-AD53-491E-A40F-441F2AFCBB5B", @"" ); // Ministry Safe Training:Complete Request:Update Training Date:Order
            RockMigrationHelper.AddActionTypeAttributeValue( "0162FBEA-E5A9-4DC7-95D0-97E9D2D0959D", "8F4BB00F-7FA2-41AD-8E90-81F4DFE2C762", @"a5df28ec-04dc-44d5-9a7b-1c6bbc87037c" ); // Ministry Safe Training:Complete Request:Update Training Date:Person Attribute
            RockMigrationHelper.AddActionTypeAttributeValue( "0162FBEA-E5A9-4DC7-95D0-97E9D2D0959D", "94689BDE-493E-4869-A614-2D54822D747C", @"c259e954-3fdd-4cad-ac43-e0a0d68225ef" ); // Ministry Safe Training:Complete Request:Update Training Date:Value|Attribute Value
            RockMigrationHelper.AddActionTypeAttributeValue( "6609D32A-1933-424E-B9A3-808AE6D2D138", "E5BAC4A6-FF7F-4016-BA9C-72D16CB60184", @"False" ); // Ministry Safe Training:Complete Request:Update Training Type:Active
            RockMigrationHelper.AddActionTypeAttributeValue( "6609D32A-1933-424E-B9A3-808AE6D2D138", "E456FB6F-05DB-4826-A612-5B704BC4EA13", @"6691e18e-d164-4875-9f10-137180c02d27" ); // Ministry Safe Training:Complete Request:Update Training Type:Person
            RockMigrationHelper.AddActionTypeAttributeValue( "6609D32A-1933-424E-B9A3-808AE6D2D138", "3F3BF3E6-AD53-491E-A40F-441F2AFCBB5B", @"" ); // Ministry Safe Training:Complete Request:Update Training Type:Order
            RockMigrationHelper.AddActionTypeAttributeValue( "6609D32A-1933-424E-B9A3-808AE6D2D138", "8F4BB00F-7FA2-41AD-8E90-81F4DFE2C762", @"8d4a22ea-10ed-4b62-bf42-d47024bc8602" ); // Ministry Safe Training:Complete Request:Update Training Type:Person Attribute
            RockMigrationHelper.AddActionTypeAttributeValue( "6609D32A-1933-424E-B9A3-808AE6D2D138", "94689BDE-493E-4869-A614-2D54822D747C", @"fae024da-3908-449a-a4f5-882cb7277cc1" ); // Ministry Safe Training:Complete Request:Update Training Type:Value|Attribute Value
            RockMigrationHelper.AddActionTypeAttributeValue( "D1F18CF7-99EF-4780-BB9D-94A31559ED7C", "E5BAC4A6-FF7F-4016-BA9C-72D16CB60184", @"False" ); // Ministry Safe Training:Complete Request:Update Training Score:Active
            RockMigrationHelper.AddActionTypeAttributeValue( "D1F18CF7-99EF-4780-BB9D-94A31559ED7C", "E456FB6F-05DB-4826-A612-5B704BC4EA13", @"6691e18e-d164-4875-9f10-137180c02d27" ); // Ministry Safe Training:Complete Request:Update Training Score:Person
            RockMigrationHelper.AddActionTypeAttributeValue( "D1F18CF7-99EF-4780-BB9D-94A31559ED7C", "3F3BF3E6-AD53-491E-A40F-441F2AFCBB5B", @"" ); // Ministry Safe Training:Complete Request:Update Training Score:Order
            RockMigrationHelper.AddActionTypeAttributeValue( "D1F18CF7-99EF-4780-BB9D-94A31559ED7C", "8F4BB00F-7FA2-41AD-8E90-81F4DFE2C762", @"e04b15e3-bebd-4b98-a919-80fcb0e479d1" ); // Ministry Safe Training:Complete Request:Update Training Score:Person Attribute
            RockMigrationHelper.AddActionTypeAttributeValue( "D1F18CF7-99EF-4780-BB9D-94A31559ED7C", "94689BDE-493E-4869-A614-2D54822D747C", @"b23ab632-b01c-445a-8362-68ba226a7d4e" ); // Ministry Safe Training:Complete Request:Update Training Score:Value|Attribute Value
            RockMigrationHelper.AddActionTypeAttributeValue( "5B283518-A45F-4847-B9C8-1264A232F995", "3F3BF3E6-AD53-491E-A40F-441F2AFCBB5B", @"" ); // Ministry Safe Training:Complete Request:Update Training Result:Order
            RockMigrationHelper.AddActionTypeAttributeValue( "5B283518-A45F-4847-B9C8-1264A232F995", "E5BAC4A6-FF7F-4016-BA9C-72D16CB60184", @"False" ); // Ministry Safe Training:Complete Request:Update Training Result:Active
            RockMigrationHelper.AddActionTypeAttributeValue( "5B283518-A45F-4847-B9C8-1264A232F995", "E456FB6F-05DB-4826-A612-5B704BC4EA13", @"6691e18e-d164-4875-9f10-137180c02d27" ); // Ministry Safe Training:Complete Request:Update Training Result:Person
            RockMigrationHelper.AddActionTypeAttributeValue( "5B283518-A45F-4847-B9C8-1264A232F995", "8F4BB00F-7FA2-41AD-8E90-81F4DFE2C762", @"db9d8876-4550-424d-a019-1aa71f8adc1d" ); // Ministry Safe Training:Complete Request:Update Training Result:Person Attribute
            RockMigrationHelper.AddActionTypeAttributeValue( "5B283518-A45F-4847-B9C8-1264A232F995", "94689BDE-493E-4869-A614-2D54822D747C", @"aa0ce3d6-1fec-48a6-aee0-97410bbdbc78" ); // Ministry Safe Training:Complete Request:Update Training Result:Value|Attribute Value
            RockMigrationHelper.AddActionTypeAttributeValue( "06ECCE5A-4E73-47B7-882C-30AEFB665A1C", "9F5F7CEC-F369-4FDF-802A-99074CE7A7FC", @"" ); // Ministry Safe Training:Complete Request:Notify Requester:From Email Address|Attribute Value
            RockMigrationHelper.AddActionTypeAttributeValue( "06ECCE5A-4E73-47B7-882C-30AEFB665A1C", "36197160-7D3D-490D-AB42-7E29105AFE91", @"False" ); // Ministry Safe Training:Complete Request:Notify Requester:Active
            RockMigrationHelper.AddActionTypeAttributeValue( "06ECCE5A-4E73-47B7-882C-30AEFB665A1C", "D1269254-C15A-40BD-B784-ADCC231D3950", @"" ); // Ministry Safe Training:Complete Request:Notify Requester:Order
            RockMigrationHelper.AddActionTypeAttributeValue( "06ECCE5A-4E73-47B7-882C-30AEFB665A1C", "0C4C13B8-7076-4872-925A-F950886B5E16", @"1c23302e-49bd-432b-9440-e42d289e5d69" ); // Ministry Safe Training:Complete Request:Notify Requester:Send To Email Addresses|Attribute Value
            RockMigrationHelper.AddActionTypeAttributeValue( "06ECCE5A-4E73-47B7-882C-30AEFB665A1C", "E3667110-339F-4FE3-B6B7-084CF9633580", @"" ); // Ministry Safe Training:Complete Request:Notify Requester:Send to Group Role
            RockMigrationHelper.AddActionTypeAttributeValue( "06ECCE5A-4E73-47B7-882C-30AEFB665A1C", "5D9B13B6-CD96-4C7C-86FA-4512B9D28386", @"Ministry Safe Training for {{ Workflow.Person }}" ); // Ministry Safe Training:Complete Request:Notify Requester:Subject
            RockMigrationHelper.AddActionTypeAttributeValue( "06ECCE5A-4E73-47B7-882C-30AEFB665A1C", "4D245B9E-6B03-46E7-8482-A51FBA190E4D", @"{{ 'Global' | Attribute:'EmailHeader' }}  <p>{{ Person.FirstName }},</p> <p>The Ministry Safe Training for {{ Workflow.Person }} has been completed.</p> <p>Result: {{ Workflow.TrainingScore }}<p/>  {{ 'Global' | Attribute:'EmailFooter' }}" ); // Ministry Safe Training:Complete Request:Notify Requester:Body
            RockMigrationHelper.AddActionTypeAttributeValue( "06ECCE5A-4E73-47B7-882C-30AEFB665A1C", "1BDC7ACA-9A0B-4C8A-909E-8B4143D9C2A3", @"False" ); // Ministry Safe Training:Complete Request:Notify Requester:Save Communication History
            RockMigrationHelper.AddActionTypeAttributeValue( "6DF84C34-012B-43EE-BFC9-65071E4794A2", "0CA0DDEF-48EF-4ABC-9822-A05E225DE26C", @"False" ); // Ministry Safe Training:Complete Request:Complete Workflow:Active
            RockMigrationHelper.AddActionTypeAttributeValue( "6DF84C34-012B-43EE-BFC9-65071E4794A2", "25CAD4BE-5A00-409D-9BAB-E32518D89956", @"" ); // Ministry Safe Training:Complete Request:Complete Workflow:Order
            RockMigrationHelper.AddActionTypeAttributeValue( "6DF84C34-012B-43EE-BFC9-65071E4794A2", "385A255B-9F48-4625-862B-26231DBAC53A", @"Completed" ); // Ministry Safe Training:Complete Request:Complete Workflow:Status|Status Attribute
            RockMigrationHelper.AddActionTypeAttributeValue( "3B9173EC-7038-4AFC-BC69-324C4EBDBC43", "361A1EC8-FFD0-4880-AF68-91DC0E0D7CDC", @"False" ); // Ministry Safe Training:Cancel Request:Delete Workflow:Active
            RockMigrationHelper.AddActionTypeAttributeValue( "3B9173EC-7038-4AFC-BC69-324C4EBDBC43", "79D23F8B-0DC8-4B48-8A86-AEA48B396C82", @"" ); // Ministry Safe Training:Cancel Request:Delete Workflow:Order

            #endregion

            #region Ministry Safe Connection

            RockMigrationHelper.UpdateWorkflowType( false, true, "Ministry Safe Connection", "Used to request a Ministry Safe Training be sent to a person from a Connection Request", "6F8A431C-BEBD-4D33-AAD6-1D70870329C2", "Training", "fa fa-clipboard", 28800, true, 0, "E7D5B3F9-45FD-4091-971A-BD5D4BD80538", 0 ); // Ministry Safe Connection
            RockMigrationHelper.UpdateWorkflowTypeAttribute( "E7D5B3F9-45FD-4091-971A-BD5D4BD80538", "E4EAB7B2-0B76-429B-AFE4-AD86D7428C70", "Person", "Person", "", 0, @"", "AE8F3513-6B55-486D-AA3E-5E84613607F0", false ); // Ministry Safe Connection:Person
            RockMigrationHelper.UpdateWorkflowTypeAttribute( "E7D5B3F9-45FD-4091-971A-BD5D4BD80538", "1B71FEF4-201F-4D53-8C60-2DF21F1985ED", "Campus", "Campus", "", 1, @"", "C93D7901-A1A6-4DA8-BA7E-F4054DC44C1B", false ); // Ministry Safe Connection:Campus
            RockMigrationHelper.UpdateWorkflowTypeAttribute( "E7D5B3F9-45FD-4091-971A-BD5D4BD80538", "9C204CD0-1233-41C5-818A-C5DA439445AA", "Reason", "Reason", "", 2, @"Ministry Safe Training requested via Connection Request", "EE2F92D8-FDD0-4AD4-8E80-9C4D1EBBAE85", false ); // Ministry Safe Connection:Reason
            RockMigrationHelper.UpdateWorkflowTypeAttribute( "E7D5B3F9-45FD-4091-971A-BD5D4BD80538", "A75DFC58-7A1B-4799-BF31-451B2BBE38FF", "Connection Request Id", "ConnectionRequestId", "", 3, @"0", "04F0BFBA-F5E6-46E4-8F1B-711E5E8BAA35", false ); // Ministry Safe Connection:Connection Request Id
            RockMigrationHelper.UpdateAttributeQualifier( "AE8F3513-6B55-486D-AA3E-5E84613607F0", "EnableSelfSelection", @"False", "41D5F2C7-DEB3-43B0-B894-7D314E750A0F" ); // Ministry Safe Connection:Person:EnableSelfSelection
            RockMigrationHelper.UpdateAttributeQualifier( "C93D7901-A1A6-4DA8-BA7E-F4054DC44C1B", "includeInactive", @"False", "FAFEC056-54FD-4951-8DB8-2DBC30795F3A" ); // Ministry Safe Connection:Campus:includeInactive
            RockMigrationHelper.UpdateAttributeQualifier( "EE2F92D8-FDD0-4AD4-8E80-9C4D1EBBAE85", "ispassword", @"False", "714616BD-9003-460A-B1F6-EA320CCF4293" ); // Ministry Safe Connection:Reason:ispassword
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

            #region DefinedValue AttributeType qualifier helper

            Sql( @"     UPDATE [aq] SET [key] = 'definedtype', [Value] = CAST( [dt].[Id] as varchar(5) )     FROM [AttributeQualifier] [aq]     INNER JOIN [Attribute] [a] ON [a].[Id] = [aq].[AttributeId]     INNER JOIN [FieldType] [ft] ON [ft].[Id] = [a].[FieldTypeId]     INNER JOIN [DefinedType] [dt] ON CAST([dt].[guid] AS varchar(50) ) = [aq].[value]     WHERE [ft].[class] = 'Rock.Field.Types.DefinedValueFieldType'     AND [aq].[key] = 'definedtypeguid'    " );

            #endregion

            #endregion

            // Delete the workflow guid from the setting in case it already exists
            Sql( @"
                DECLARE @BlockId int
                SET @BlockId = (SELECT [Id] FROM [Block] WHERE [Guid] = 'B5C1FDB6-0224-43E4-8E26-6B2EAF86253A')
                DECLARE @AttributeId int
                SET @AttributeId = (SELECT [Id] FROM [Attribute] WHERE [Guid] = '7197A0FB-B330-43C4-8E62-F3C14F649813')

                DECLARE @OldValue NVARCHAR(MAX) = ''
                DECLARE @NewValue NVARCHAR(MAX) = ''

                IF EXISTS (SELECT 1 FROM [AttributeValue] WHERE [AttributeId] = @AttributeId AND [EntityId] = @BlockId )
                BEGIN
                    SET @OldValue = (SELECT [Value] FROM [AttributeValue] WHERE [AttributeId] = @AttributeId AND [EntityId] = @BlockId )

                    IF CHARINDEX( 'AE617426-E1E3-4E06-B3D8-B7AB1A7B4892', @OldValue ) != 0
                    BEGIN
                        SET @NewValue = REPLACE(@OldValue, ',AE617426-E1E3-4E06-B3D8-B7AB1A7B4892', '') -- if not first item
                        SET @NewValue = REPLACE(@NewValue, 'AE617426-E1E3-4E06-B3D8-B7AB1A7B4892,', '') -- if first item
                        SET @NewValue = REPLACE(@NewValue, 'AE617426-E1E3-4E06-B3D8-B7AB1A7B4892', '') -- if only item

	                   UPDATE [AttributeValue]
	                   SET [Value] = @NewValue
	                   WHERE [AttributeId] = @AttributeId AND [EntityId] = @BlockId
                    END
                END
            " );

            // Add Action to person bio block
            RockMigrationHelper.AddBlockAttributeValue( "B5C1FDB6-0224-43E4-8E26-6B2EAF86253A", "7197A0FB-B330-43C4-8E62-F3C14F649813", "AE617426-E1E3-4E06-B3D8-B7AB1A7B4892", appendToExisting: true );
        }

        /// <summary>
        /// The commands to undo a migration from a specific version
        /// </summary>
        public override void Down()
        {
            RockMigrationHelper.DeleteBlock( "63FF2A72-B3B7-4CD9-A652-91B8230773A6" );
            RockMigrationHelper.DeleteAttribute( SystemGuid.Attribute.MINISTRY_SAFE_PERSON_ATTRIBUTE_TEST_TYPE );
            RockMigrationHelper.DeleteAttribute( SystemGuid.Attribute.MINISTRY_SAFE_PERSON_ATTRIBUTE_TEST_DATE );
            RockMigrationHelper.DeleteAttribute( SystemGuid.Attribute.MINISTRY_SAFE_PERSON_ATTRIBUTE_TEST_SCORE );
            RockMigrationHelper.DeleteCategory( SystemGuid.Category.MINISTRY_SAFE_PERSON_ATTRIBUTE_CATEGORY );
            RockMigrationHelper.DeleteDefinedType( SystemGuid.DefinedType.MINISTRY_SAFE_TRAINING_TYPES );

            Sql( @"
                DECLARE @BlockId int
                SET @BlockId = (SELECT [Id] FROM [Block] WHERE [Guid] = 'B5C1FDB6-0224-43E4-8E26-6B2EAF86253A')
                DECLARE @AttributeId int
                SET @AttributeId = (SELECT [Id] FROM [Attribute] WHERE [Guid] = '7197A0FB-B330-43C4-8E62-F3C14F649813')

                DECLARE @OldValue NVARCHAR(MAX) = ''
                DECLARE @NewValue NVARCHAR(MAX) = ''

                IF EXISTS (SELECT 1 FROM [AttributeValue] WHERE [AttributeId] = @AttributeId AND [EntityId] = @BlockId )
                BEGIN
                    SET @OldValue = (SELECT [Value] FROM [AttributeValue] WHERE [AttributeId] = @AttributeId AND [EntityId] = @BlockId )

                    IF CHARINDEX( 'AE617426-E1E3-4E06-B3D8-B7AB1A7B4892', @OldValue ) != 0
                    BEGIN
                        SET @NewValue = REPLACE(@OldValue, ',AE617426-E1E3-4E06-B3D8-B7AB1A7B4892', '') -- if not first item
                        SET @NewValue = REPLACE(@NewValue, 'AE617426-E1E3-4E06-B3D8-B7AB1A7B4892,', '') -- if first item
                        SET @NewValue = REPLACE(@NewValue, 'AE617426-E1E3-4E06-B3D8-B7AB1A7B4892', '') -- if only item

	                   UPDATE [AttributeValue]
	                   SET [Value] = @NewValue
	                   WHERE [AttributeId] = @AttributeId AND [EntityId] = @BlockId
                    END
                END
            " );

            RockMigrationHelper.DeleteAttribute( SystemGuid.Attribute.MINISTRY_SAFE_PERSON_ATTRIBUTE_TEST_RESULT );

            RockMigrationHelper.DeleteWorkflowType( "E7D5B3F9-45FD-4091-971A-BD5D4BD80538" );
            RockMigrationHelper.DeleteWorkflowType( "AE617426-E1E3-4E06-B3D8-B7AB1A7B4892" );
        }
    }
}
