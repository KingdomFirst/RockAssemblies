// <copyright>
// Copyright 2023 by Kingdom First Solutions
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
    [MigrationNumber( 4, "1.12.0" )]
    public class AddTransactionFeeAttributes : Migration
    {
        /// <summary>
        /// The commands to run to migrate plugin to the specific version
        /// </summary>
        public override void Up()
        {
            // Setup Financial Account fee account attribute
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Transaction Fee Account", "Expense account number for gateway transaction fees.", 13, "", KFSConst.Attribute.FINANCIAL_ACCOUNT_FEE_ACCOUNT, "rocks.kfs.ShelbyFinancials.FEEACCOUNTNO" );

            Sql( string.Format( @"
                --
                -- Set FinancialAccount attribute to category
                --
                DECLARE @AccountEntityTypeId int = ( SELECT TOP 1 [Id] FROM [EntityType] WHERE [Guid] = '{0}' )
                DECLARE @AccountCategoryId int = ( SELECT TOP 1 [Id] FROM [Category] WHERE [Guid] = '{1}' )
                DECLARE @AccountFeeAccountId int = ( SELECT [Id] FROM [Attribute] WHERE [Guid] = '{2}' )

                IF NOT EXISTS ( SELECT [AttributeId], [CategoryId] FROM [AttributeCategory] WHERE [AttributeId] = @AccountFeeAccountId AND [CategoryId] = @AccountCategoryId )

                BEGIN
                    INSERT INTO [AttributeCategory]
                    SELECT @AccountFeeAccountId, @AccountCategoryId
                END
            ", Rock.SystemGuid.EntityType.FINANCIAL_ACCOUNT, KFSConst.Attribute.FINANCIAL_ACCOUNT_ATTRIBUTE_CATEGORY, KFSConst.Attribute.FINANCIAL_ACCOUNT_FEE_ACCOUNT ) );

            // Setup Gateway Account fee processing attributes
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialGateway", Rock.SystemGuid.FieldType.SINGLE_SELECT, "", "", "Gateway Fee Processing", "How should the Shelby Financials Export plugin process transaction fees? DEFAULT: No special handling of transaction fees will be performed. NET DEBIT: Add credit entries for any transaction fees and use net amount (amount - transaction fees) for debit account entries. GROSS DEBIT: Debit account entries are left untouched (gross) and new debit and credit entries will be added for any transaction fees. NOTE: Both Net Debit and Gross Debit require a Fee Account attribute be set on either the financial gateway or financial account.", 0, "0", KFSConst.Attribute.FINANCIAL_GATEWAY_FEE_PROCESSING, "rocks.kfs.ShelbyFinancials.FEEPROCESSING", true );
            RockMigrationHelper.AddAttributeQualifier( KFSConst.Attribute.FINANCIAL_GATEWAY_FEE_PROCESSING, "values", "0^Default,1^Net Debit,2^Gross Debit", "EDD83B29-78ED-40DC-B8F9-9664EDB14884" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialGateway", Rock.SystemGuid.FieldType.TEXT, "", "", "Default Fee Account", "Default account number for transaction fees.", 1, "", KFSConst.Attribute.FINANCIAL_GATEWAY_DEFAULT_FEE_ACCOUNT, "rocks.kfs.ShelbyFinancials.DEFAULTFEEACCOUNTNO" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialGateway", Rock.SystemGuid.FieldType.SINGLE_SELECT, "", "", "Gateway Fee Calculation", "The method in which transaction fees are calculated.", 0, "0", KFSConst.Attribute.FINANCIAL_GATEWAY_FEE_CALCULATION, "rocks.kfs.ShelbyFinancials.FEECALCULATION", true );
            RockMigrationHelper.AddAttributeQualifier( KFSConst.Attribute.FINANCIAL_GATEWAY_FEE_CALCULATION, "values", "0^Fixed Amount,1^Percentage", "5027B9CD-4366-4CC8-917B-418025D9C158" );

            RockMigrationHelper.UpdateCategory( Rock.SystemGuid.EntityType.ATTRIBUTE, "Shelby Financials Export", "fa fa-calculator", "", KFSConst.Attribute.FINANCIAL_GATEWAY_ATTRIBUTE_CATEGORY );

            Sql( string.Format( @"
                --
                -- Set FinancialGateway attribute to category
                --
                DECLARE @GatewayEntityTypeId int = ( SELECT TOP 1 [Id] FROM [EntityType] WHERE [Guid] = '122EFE60-84A6-4C7A-A852-30E4BD89A662' )
                DECLARE @GatewayCategoryId int = ( SELECT TOP 1 [Id] FROM [Category] WHERE [Guid] = '{0}' )

                UPDATE [Category]
                SET [EntityTypeQualifierColumn] = 'EntityTypeId',  [EntityTypeQualifierValue] = @GatewayEntityTypeId
                WHERE [Id] = @GatewayCategoryId

                DECLARE @GatewayDefaultFeeAccountId int = ( SELECT [Id] FROM [Attribute] WHERE [Guid] = '{1}' )

                IF NOT EXISTS ( SELECT [AttributeId], [CategoryId] FROM [AttributeCategory] WHERE [AttributeId] = @GatewayDefaultFeeAccountId AND [CategoryId] = @GatewayCategoryId )

                BEGIN
                    INSERT INTO [AttributeCategory]
                    SELECT @GatewayDefaultFeeAccountId, @GatewayCategoryId
                END

                DECLARE @GatewayFeeProcessingAttributeId int = ( SELECT [Id] FROM [Attribute] WHERE [Guid] = '{2}' )

                IF NOT EXISTS ( SELECT [AttributeId], [CategoryId] FROM [AttributeCategory] WHERE [AttributeId] = @GatewayFeeProcessingAttributeId AND [CategoryId] = @GatewayCategoryId )

                BEGIN
                    INSERT INTO [AttributeCategory]
                    SELECT @GatewayFeeProcessingAttributeId, @GatewayCategoryId
                END

                DECLARE @GatewayFeeCalculationAttributeId int = ( SELECT [Id] FROM [Attribute] WHERE [Guid] = '{3}' )

                IF NOT EXISTS ( SELECT [AttributeId], [CategoryId] FROM [AttributeCategory] WHERE [AttributeId] = @GatewayFeeCalculationAttributeId AND [CategoryId] = @GatewayCategoryId )

                BEGIN
                    INSERT INTO [AttributeCategory]
                    SELECT @GatewayFeeCalculationAttributeId, @GatewayCategoryId
                END
            ", KFSConst.Attribute.FINANCIAL_GATEWAY_ATTRIBUTE_CATEGORY, KFSConst.Attribute.FINANCIAL_GATEWAY_DEFAULT_FEE_ACCOUNT, KFSConst.Attribute.FINANCIAL_GATEWAY_FEE_PROCESSING, KFSConst.Attribute.FINANCIAL_GATEWAY_FEE_CALCULATION ) );

        }

        /// <summary>
        /// The commands to undo a migration from a specific version
        /// </summary>
        public override void Down()
        {
            // remove attributes
            RockMigrationHelper.DeleteAttribute( KFSConst.Attribute.FINANCIAL_ACCOUNT_FEE_ACCOUNT );
            RockMigrationHelper.DeleteAttribute( KFSConst.Attribute.FINANCIAL_GATEWAY_DEFAULT_FEE_ACCOUNT );
            RockMigrationHelper.DeleteAttribute( KFSConst.Attribute.FINANCIAL_GATEWAY_FEE_PROCESSING );
            RockMigrationHelper.DeleteAttribute( KFSConst.Attribute.FINANCIAL_GATEWAY_FEE_CALCULATION );

            // remove category
            RockMigrationHelper.DeleteCategory( KFSConst.Attribute.FINANCIAL_GATEWAY_ATTRIBUTE_CATEGORY );
        }
    }
}
