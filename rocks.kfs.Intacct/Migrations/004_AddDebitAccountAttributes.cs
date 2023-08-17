// <copyright>
// Copyright 2023 by Kingdom First Solutions
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//9DF341B7-2E07-44BA-94B1-F637BE551961
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
using KFSConst = rocks.kfs.Intacct.SystemGuid;

namespace rocks.kfs.Intacct.Migrations
{
    [MigrationNumber( 4, "1.11.0" )]
    public class AddDebitAccountAttributes : Migration
    {
        /// <summary>
        /// The commands to run to migrate plugin to the specific version
        /// </summary>
        public override void Up()
        {
            // ADD NEW DEBIT ACCOUNT ATTRIBUTES FOR FINANCIAL ACCOUNTS
            // project
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.DEFINED_VALUE, "", "", "Default Debit Project", "Designates the Project for the assigned Debit Account at the Financial Account Level.", 6, "", KFSConst.Attribute.FINANCIAL_ACCOUNT_PROJECT_DEBIT, "rocks.kfs.Intacct.DEBITPROJECTID" );
            RockMigrationHelper.UpdateAttributeQualifier( KFSConst.Attribute.FINANCIAL_ACCOUNT_PROJECT_DEBIT, "allowmultiple", "False", "4A597C2F-034F-4855-B939-F44EA19B4194" );
            RockMigrationHelper.UpdateAttributeQualifier( KFSConst.Attribute.FINANCIAL_ACCOUNT_PROJECT_DEBIT, "definedtype", "", "07C034DB-AD00-4BB9-A0B0-3BC35BF7A994" );
            RockMigrationHelper.UpdateAttributeQualifier( KFSConst.Attribute.FINANCIAL_ACCOUNT_PROJECT_DEBIT, "displaydescription", "True", "B87B2631-5692-4346-BDD7-557FC224FF40" );
            RockMigrationHelper.UpdateAttributeQualifier( KFSConst.Attribute.FINANCIAL_ACCOUNT_PROJECT_DEBIT, "enhancedselection", "True", "88B9BF14-0756-45B1-9B7F-5BFE1A4DDF7D" );

            // set defined type qualifiers
            Sql( @"
                DECLARE @ProjectDefinedTypeId int = ( SELECT TOP 1 [Id] FROM [DefinedType] WHERE [Guid] = 'C244D4C4-636F-4BCA-8E7C-1907933ABB74' )

                UPDATE [AttributeQualifier] SET [Value] = CAST( @ProjectDefinedTypeId AS varchar )
                WHERE [Guid] = '07C034DB-AD00-4BB9-A0B0-3BC35BF7A994'
            " );

            // gl attributes
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Debit Class", "The Intacct dimension for Class Id to be used for assigned Debit Account.", 7, "", KFSConst.Attribute.FINANCIAL_ACCOUNT_CLASS_DEBIT, "rocks.kfs.Intacct.DEBITCLASSID" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Debit Department", "The Intacct dimension for Department Id to be used for assigned Debit Account.", 8, "", KFSConst.Attribute.FINANCIAL_ACCOUNT_DEPARTMENT_DEBIT, "rocks.kfs.Intacct.DEBITDEPARTMENT" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Debit Location", "The Intacct dimension for Location Id to be used for assigned Debit Account. Required if multi-entity enabled.", 9, "", KFSConst.Attribute.FINANCIAL_ACCOUNT_LOCATION_DEBIT, "rocks.kfs.Intacct.DEBITLOCATION" );

            // set attribute category for new attributes
            Sql( string.Format( @"
                --
                -- Set FinancialAccount attributes to category
                --

                DECLARE @AccountCategoryId int = ( SELECT TOP 1 [Id] FROM [Category] WHERE [Guid] = '{0}' )
                DECLARE @AccountDefaultProject int = ( SELECT [Id] FROM [Attribute] WHERE [Guid] = '{1}' )

                IF NOT EXISTS ( SELECT [AttributeId], [CategoryId] FROM [AttributeCategory] WHERE [AttributeId] = @AccountDefaultProject AND [CategoryId] = @AccountCategoryId )

                BEGIN
                    INSERT INTO [AttributeCategory]
                    SELECT @AccountDefaultProject, @AccountCategoryId
                END

                DECLARE @AccountDebitClass int = ( SELECT [Id] FROM [Attribute] WHERE [Guid] = '{2}' )

                IF NOT EXISTS ( SELECT [AttributeId], [CategoryId] FROM [AttributeCategory] WHERE [AttributeId] = @AccountDebitClass AND [CategoryId] = @AccountCategoryId )

                BEGIN
                    INSERT INTO [AttributeCategory]
                    SELECT @AccountDebitClass, @AccountCategoryId
                END

                DECLARE @AccountDebitDepartment int = ( SELECT [Id] FROM [Attribute] WHERE [Guid] = '{3}' )

                IF NOT EXISTS ( SELECT [AttributeId], [CategoryId] FROM [AttributeCategory] WHERE [AttributeId] = @AccountDebitDepartment AND [CategoryId] = @AccountCategoryId )

                BEGIN
                    INSERT INTO [AttributeCategory]
                    SELECT @AccountDebitDepartment, @AccountCategoryId
                END

                DECLARE @AccountDebitLocation int = ( SELECT [Id] FROM [Attribute] WHERE [Guid] = '{4}' )

                IF NOT EXISTS ( SELECT [AttributeId], [CategoryId] FROM [AttributeCategory] WHERE [AttributeId] = @AccountDebitLocation AND [CategoryId] = @AccountCategoryId )

                BEGIN
                    INSERT INTO [AttributeCategory]
                    SELECT @AccountDebitLocation, @AccountCategoryId
                END
            ", KFSConst.Attribute.FINANCIAL_ACCOUNT_ATTRIBUTE_CATEGORY, KFSConst.Attribute.FINANCIAL_ACCOUNT_PROJECT_DEBIT, KFSConst.Attribute.FINANCIAL_ACCOUNT_CLASS_DEBIT, KFSConst.Attribute.FINANCIAL_ACCOUNT_DEPARTMENT_DEBIT, KFSConst.Attribute.FINANCIAL_ACCOUNT_LOCATION_DEBIT ) );

            // rename and reorder of existing attributes
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.DEFINED_VALUE, "", "", "Default Credit Project", "Designates the Project for the assigned Credit Account at the Financial Account Level.", 1, "", KFSConst.Attribute.FINANCIAL_ACCOUNT_PROJECT, "rocks.kfs.Intacct.PROJECTID" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Credit Class", "The Intacct dimension for Class Id to be used for assigned Credit Account.", 2, "", KFSConst.Attribute.FINANCIAL_ACCOUNT_CLASS, "rocks.kfs.Intacct.CLASSID" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Credit Department", "The Intacct dimension for Department Id to be used for assigned Credit Account.", 3, "", KFSConst.Attribute.FINANCIAL_ACCOUNT_DEPARTMENT, "rocks.kfs.Intacct.DEPARTMENT" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Credit Location", "The Intacct dimension for Location Id to be used for assigned Credit Account. Required if multi-entity enabled.", 4, "", KFSConst.Attribute.FINANCIAL_ACCOUNT_LOCATION, "rocks.kfs.Intacct.LOCATION" );

            // reorder remaining attributes
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Credit Account", "Account number to use for credit column. Required by Intacct.", 0, "", KFSConst.Attribute.FINANCIAL_ACCOUNT_CREDIT_ACCOUNT, "rocks.kfs.Intacct.ACCOUNTNO" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Debit Account", "Account number to use for debit column. Required by Intacct.", 5, "", KFSConst.Attribute.FINANCIAL_ACCOUNT_DEBIT_ACCOUNT, "rocks.kfs.Intacct.DEBITACCOUNTNO" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Transaction Fee Account", "Expense account number for gateway transaction fees.", 10, "", KFSConst.Attribute.FINANCIAL_ACCOUNT_FEE_ACCOUNT, "rocks.kfs.Intacct.FEEACCOUNTNO" );

            // copy values already provided in existing (now credit) attributes into corresponding new debit attributes to keep previous functionality in tact
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

                --
                -- Class
                --

                DECLARE @CreditClassAttrId int = ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '{2}' )
                DECLARE @DebitClassAttrId int = ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '{3}' )

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
                    @DebitClassAttrId, 
                    AV.[EntityId], 
                    AV.[Value],
                    NEWID(), 
                    AV.[CreatedDateTime], 
                    AV.[ModifiedDateTime], 
                    AV.[CreatedByPersonAliasId],
                    AV.[ModifiedByPersonAliasId],
                    AV.[ValueAsNumeric]
                FROM AttributeValue AV WITH(NOLOCK)
                LEFT JOIN AttributeValue AV2 WITH(NOLOCK) ON AV.EntityId = AV2.EntityId AND AV2.AttributeId = @DebitClassAttrId 
                WHERE AV.AttributeId = @CreditClassAttrId AND AV2.Id IS NULL;

                --
                -- Department
                --

                DECLARE @CreditDepartmentAttrId int = ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '{4}' )
                DECLARE @DebitDepartmentAttrId int = ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '{5}' )

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
                    @DebitDepartmentAttrId, 
                    AV.[EntityId], 
                    AV.[Value],
                    NEWID(), 
                    AV.[CreatedDateTime], 
                    AV.[ModifiedDateTime], 
                    AV.[CreatedByPersonAliasId],
                    AV.[ModifiedByPersonAliasId],
                    AV.[ValueAsNumeric]
                FROM AttributeValue AV WITH(NOLOCK)
                LEFT JOIN AttributeValue AV2 WITH(NOLOCK) ON AV.EntityId = AV2.EntityId AND AV2.AttributeId = @DebitDepartmentAttrId 
                WHERE AV.AttributeId = @CreditDepartmentAttrId AND AV2.Id IS NULL;

                --
                -- Location
                --

                DECLARE @CreditLocationAttrId int = ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '{6}' )
                DECLARE @DebitLocationAttrId int = ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '{7}' )

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
                    @DebitLocationAttrId, 
                    AV.[EntityId], 
                    AV.[Value],
                    NEWID(), 
                    AV.[CreatedDateTime], 
                    AV.[ModifiedDateTime], 
                    AV.[CreatedByPersonAliasId],
                    AV.[ModifiedByPersonAliasId],
                    AV.[ValueAsNumeric]
                FROM AttributeValue AV WITH(NOLOCK)
                LEFT JOIN AttributeValue AV2 WITH(NOLOCK) ON AV.EntityId = AV2.EntityId AND AV2.AttributeId = @DebitLocationAttrId 
                WHERE AV.AttributeId = @CreditLocationAttrId AND AV2.Id IS NULL;
            ", KFSConst.Attribute.FINANCIAL_ACCOUNT_PROJECT, KFSConst.Attribute.FINANCIAL_ACCOUNT_PROJECT_DEBIT, KFSConst.Attribute.FINANCIAL_ACCOUNT_CLASS, KFSConst.Attribute.FINANCIAL_ACCOUNT_CLASS_DEBIT, KFSConst.Attribute.FINANCIAL_ACCOUNT_DEPARTMENT, KFSConst.Attribute.FINANCIAL_ACCOUNT_DEPARTMENT_DEBIT, KFSConst.Attribute.FINANCIAL_ACCOUNT_LOCATION, KFSConst.Attribute.FINANCIAL_ACCOUNT_LOCATION_DEBIT ) );
        }

        /// <summary>
        /// The commands to undo a migration from a specific version
        /// </summary>
        public override void Down()
        {
            // remove debit account related attributes
            RockMigrationHelper.DeleteAttribute( KFSConst.Attribute.FINANCIAL_ACCOUNT_PROJECT_DEBIT );
            RockMigrationHelper.DeleteAttribute( KFSConst.Attribute.FINANCIAL_ACCOUNT_CLASS_DEBIT );
            RockMigrationHelper.DeleteAttribute( KFSConst.Attribute.FINANCIAL_ACCOUNT_DEPARTMENT_DEBIT );
            RockMigrationHelper.DeleteAttribute( KFSConst.Attribute.FINANCIAL_ACCOUNT_LOCATION_DEBIT );

            // update name and order of original attributes 
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.DEFINED_VALUE, "", "", "Default Project", "Designates the Project at the Financial Account Level", 0, "", KFSConst.Attribute.FINANCIAL_ACCOUNT_PROJECT, "rocks.kfs.Intacct.PROJECTID" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Class", "The Intacct dimension for Class Id.", 4, "", KFSConst.Attribute.FINANCIAL_ACCOUNT_CLASS, "rocks.kfs.Intacct.CLASSID" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Department", "The Intacct dimension for Department Id.", 5, "", KFSConst.Attribute.FINANCIAL_ACCOUNT_DEPARTMENT, "rocks.kfs.Intacct.DEPARTMENT" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Location", "The Intacct dimension for Location Id. Required if multi-entity enabled.", 6, "", KFSConst.Attribute.FINANCIAL_ACCOUNT_LOCATION, "rocks.kfs.Intacct.LOCATION" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Credit Account", "Account number to use for credit column. Required by Intacct.", 1, "", KFSConst.Attribute.FINANCIAL_ACCOUNT_CREDIT_ACCOUNT, "rocks.kfs.Intacct.ACCOUNTNO" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Debit Account", "Account number to use for debit column. Required by Intacct.", 2, "", KFSConst.Attribute.FINANCIAL_ACCOUNT_DEBIT_ACCOUNT, "rocks.kfs.Intacct.DEBITACCOUNTNO" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Transaction Fee Account", "Expense account number for gateway transaction fees.", 3, "", KFSConst.Attribute.FINANCIAL_ACCOUNT_FEE_ACCOUNT, "rocks.kfs.Intacct.FEEACCOUNTNO" );
        }
    }
}
