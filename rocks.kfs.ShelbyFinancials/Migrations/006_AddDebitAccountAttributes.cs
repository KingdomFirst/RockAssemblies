// <copyright>
// Copyright 2025 by Kingdom First Solutions
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
using Rock.Plugin;
using KFSConst = rocks.kfs.ShelbyFinancials.SystemGuid;

namespace rocks.kfs.ShelbyFinancials.Migrations
{
    [MigrationNumber( 6, "1.13.0" )]
    public class AddDebitAccountAttributes : Migration
    {
        /// <summary>
        /// The commands to run to migrate plugin to the specific version
        /// </summary>
        public override void Up()
        {
            // Add new Project Debit Account Attribute for financial accounts
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.DEFINED_VALUE, "", "", "Default Debit Project", "Designates the Project for the assigned Debit Account at the Financial Account Level.", 0, "", KFSConst.Attribute.FINANCIAL_ACCOUNT_PROJECT_DEBIT, "rocks.kfs.ShelbyFinancials.DebitProject" );
            RockMigrationHelper.UpdateAttributeQualifier( KFSConst.Attribute.FINANCIAL_ACCOUNT_PROJECT_DEBIT, "allowmultiple", "False", "79689F90-35C5-4911-BDAE-5EE267208CEF" );
            RockMigrationHelper.UpdateAttributeQualifier( KFSConst.Attribute.FINANCIAL_ACCOUNT_PROJECT_DEBIT, "definedtype", "", "106BAE59-100A-4C4F-B43D-444FAD66302F" );
            RockMigrationHelper.UpdateAttributeQualifier( KFSConst.Attribute.FINANCIAL_ACCOUNT_PROJECT_DEBIT, "displaydescription", "True", "2C131398-61A1-4C87-8585-974C1BDBF18D" );
            RockMigrationHelper.UpdateAttributeQualifier( KFSConst.Attribute.FINANCIAL_ACCOUNT_PROJECT_DEBIT, "enhancedselection", "True", "D6EB0B90-C067-4A66-8823-5BADE64A5737" );

            // set defined type qualifiers
            Sql( @"
                DECLARE @ProjectDefinedTypeId int = ( SELECT TOP 1 [Id] FROM [DefinedType] WHERE [Guid] = '2CE68D65-7EAC-4D5E-80B6-6FB903726961' )

                UPDATE [AttributeQualifier] SET [Value] = CAST( @ProjectDefinedTypeId AS varchar )
                WHERE [Guid] = '106BAE59-100A-4C4F-B43D-444FAD66302F'
            " );

            // set attribute category for new attributes
            Sql( string.Format( @"
                --
                -- Set FinancialAccount attribute to category
                --

                DECLARE @AccountCategoryId int = ( SELECT TOP 1 [Id] FROM [Category] WHERE [Guid] = '{0}' )
                DECLARE @AccountDefaultProject int = ( SELECT [Id] FROM [Attribute] WHERE [Guid] = '{1}' )

                IF NOT EXISTS ( SELECT [AttributeId], [CategoryId] FROM [AttributeCategory] WHERE [AttributeId] = @AccountDefaultProject AND [CategoryId] = @AccountCategoryId )

                BEGIN
                    INSERT INTO [AttributeCategory]
                    SELECT @AccountDefaultProject, @AccountCategoryId
                END
            ", KFSConst.Attribute.FINANCIAL_ACCOUNT_ATTRIBUTE_CATEGORY, KFSConst.Attribute.FINANCIAL_ACCOUNT_PROJECT_DEBIT ) );

            // rename existing project attribute
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.DEFINED_VALUE, "", "", "Default Credit Project", "Designates the Project for the assigned Credit Account at the Financial Account Level.", 1, "", KFSConst.Attribute.FINANCIAL_ACCOUNT_PROJECT_CREDIT, "rocks.kfs.ShelbyFinancials.Project" );

            // reorder remaining attributes
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Company", "", 2, "", "A211D2C9-249D-4D33-B120-A3EAB37C1EDF", "rocks.kfs.ShelbyFinancials.Company" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Fund", "", 3, "", "B83D7934-F85A-42B7-AD0E-B4E16D63C189", "rocks.kfs.ShelbyFinancials.Fund" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Debit Account", "", 4, "", "FD4EF8CC-DDB7-4DBD-9FD1-601A0119850B", "rocks.kfs.ShelbyFinancials.DebitAccount" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Revenue Department", "", 5, "", "2C1EE0CC-D329-453B-B4F0-29549E24ED05", "rocks.kfs.ShelbyFinancials.Department" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Revenue Account", "", 6, "", "0D114FB9-B1AA-4D6D-B0F3-9BB739710992", "rocks.kfs.ShelbyFinancials.CreditAccount" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Region", "", 7, "", "9B67459C-3C61-491D-B072-9A9830FBB18F", "rocks.kfs.ShelbyFinancials.Region" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Super Fund", "", 8, "", "8B8D8BBF-B763-4314-87D2-00B9B8BA5A0F", "rocks.kfs.ShelbyFinancials.SuperFund" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Location", "", 9, "", "22699ECA-BB71-4EFD-B416-17B41ED3DBEC", "rocks.kfs.ShelbyFinancials.Location" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Cost Center", "", 10, "", "CD925E61-F87D-461F-9EFA-C1E14397FC4D", "rocks.kfs.ShelbyFinancials.CostCenter" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Account Sub", "", 11, "", "65D935EC-3501-41A6-A2C5-CABC62AB9EF1", "rocks.kfs.ShelbyFinancials.AccountSub" );

            // copy values already provided in existing Default Project (now Default Credit Project) attributes into new Default Debit Project attribute to keep previous functionality in tact
            Sql( string.Format( @"
                --
                -- Default Project
                --

                DECLARE @CreditProjectAttrId int = ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '{0}' )
                DECLARE @DebitProjectAttrId int = ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '{1}' )

                INSERT INTO [AttributeValue]
                   ([IsSystem]
                   ,[AttributeId]
                   ,[EntityId]
                   ,[Value]
                   ,[Guid]
                   ,[CreatedDateTime]
                   ,[ModifiedDateTime]
                   ,[CreatedByPersonAliasId]
                   ,[ModifiedByPersonAliasId]
                   ,[ValueAsNumeric])

                SELECT AV.[IsSystem], 
                    @DebitProjectAttrId, 
                    AV.[EntityId], 
                    AV.[Value],
                    NEWID(), 
                    AV.[CreatedDateTime], 
                    AV.[ModifiedDateTime], 
                    AV.[CreatedByPersonAliasId],
                    AV.[ModifiedByPersonAliasId],
                    AV.[ValueAsNumeric]
                FROM AttributeValue AV WITH(NOLOCK)
                LEFT JOIN AttributeValue AV2 WITH(NOLOCK) ON AV.EntityId = AV2.EntityId AND AV2.AttributeId = @DebitProjectAttrId 
                WHERE AV.AttributeId = @CreditProjectAttrId AND AV2.Id IS NULL;
            ", KFSConst.Attribute.FINANCIAL_ACCOUNT_PROJECT_CREDIT, KFSConst.Attribute.FINANCIAL_ACCOUNT_PROJECT_DEBIT ) );
        }

        /// <summary>
        /// The commands to undo a migration from a specific version
        /// </summary>
        public override void Down()
        {
            // remove debit project account attribute
            RockMigrationHelper.DeleteAttribute( KFSConst.Attribute.FINANCIAL_ACCOUNT_PROJECT_DEBIT );

            // revert name of original project attribute
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.DEFINED_VALUE, "", "", "Default Project", "Designates the Project at the Financial Account Level", 0, "", KFSConst.Attribute.FINANCIAL_ACCOUNT_PROJECT_CREDIT, "rocks.kfs.ShelbyFinancials.Project" );

            // reorder remaining attributes
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Company", "", 1, "", "A211D2C9-249D-4D33-B120-A3EAB37C1EDF", "rocks.kfs.ShelbyFinancials.Company" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Fund", "", 2, "", "B83D7934-F85A-42B7-AD0E-B4E16D63C189", "rocks.kfs.ShelbyFinancials.Fund" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Debit Account", "", 3, "", "FD4EF8CC-DDB7-4DBD-9FD1-601A0119850B", "rocks.kfs.ShelbyFinancials.DebitAccount" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Revenue Department", "", 4, "", "2C1EE0CC-D329-453B-B4F0-29549E24ED05", "rocks.kfs.ShelbyFinancials.Department" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Revenue Account", "", 5, "", "0D114FB9-B1AA-4D6D-B0F3-9BB739710992", "rocks.kfs.ShelbyFinancials.CreditAccount" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Region", "", 6, "", "9B67459C-3C61-491D-B072-9A9830FBB18F", "rocks.kfs.ShelbyFinancials.Region" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Super Fund", "", 7, "", "8B8D8BBF-B763-4314-87D2-00B9B8BA5A0F", "rocks.kfs.ShelbyFinancials.SuperFund" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Location", "", 8, "", "22699ECA-BB71-4EFD-B416-17B41ED3DBEC", "rocks.kfs.ShelbyFinancials.Location" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Cost Center", "", 9, "", "CD925E61-F87D-461F-9EFA-C1E14397FC4D", "rocks.kfs.ShelbyFinancials.CostCenter" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Account Sub", "", 10, "", "65D935EC-3501-41A6-A2C5-CABC62AB9EF1", "rocks.kfs.ShelbyFinancials.AccountSub" );
        }
    }
}
