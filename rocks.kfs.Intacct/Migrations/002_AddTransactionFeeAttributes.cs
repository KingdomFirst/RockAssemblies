﻿// <copyright>
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
using System;
using Rock.Plugin;
using KFSConst = rocks.kfs.Intacct.SystemGuid;

namespace com.kfs.Intacct.Migrations
{
    [MigrationNumber( 2, "1.7.4" )]
    public class AddTransactionFeeAttributes : Migration
    {
        /// <summary>
        /// The commands to run to migrate plugin to the specific version
        /// </summary>
        public override void Up()
        {
            // Setup Financial Account fee account attribute
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Transaction Fee Account", "Expense account number for gateway transaction fees.", 3, "", KFSConst.Attribute.FINANCIAL_ACCOUNT_FEE_ACCOUNT, "rocks.kfs.Intacct.FEEACCOUNTNO" );

            // re-order Financial Account attributes
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Class", "The Intacct dimension for Class Id.", 4, "", KFSConst.Attribute.FINANCIAL_ACCOUNT_CLASS, "rocks.kfs.Intacct.CLASSID" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Department", "The Intacct dimension for Department Id.", 5, "", KFSConst.Attribute.FINANCIAL_ACCOUNT_DEPARTMENT, "rocks.kfs.Intacct.DEPARTMENT" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Location", "The Intacct dimension for Location Id. Required if multi-entity enabled.", 6, "", KFSConst.Attribute.FINANCIAL_ACCOUNT_LOCATION, "rocks.kfs.Intacct.LOCATION" );

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
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialGateway", Rock.SystemGuid.FieldType.SINGLE_SELECT, "", "", "Gateway Fee Processing", "Set how the Intacct Export plugin will process gateway transaction fees. Combined: Transaction fees will remain included in the total amount of the financial transaction; Separate: If a Fee Account attribute on either the financial gateway or financial account is set, transactions fees will be subtracted from the total amount of the financial transaction and added as a separate line item toward the Fee Account provided.", 0, "0", KFSConst.Attribute.FINANCIAL_GATEWAY_FEE_PROCESSING, "rocks.kfs.Intacct.FEEPROCESSING", true );
            RockMigrationHelper.AddAttributeQualifier( KFSConst.Attribute.FINANCIAL_GATEWAY_FEE_PROCESSING, "values", "0^Combined,1^Separate", "BA5F38C9-78D4-43D9-86ED-5248A8AADFA5" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialGateway", Rock.SystemGuid.FieldType.TEXT, "", "", "Default Fee Account", "Default account number for transaction fees.", 1, "", KFSConst.Attribute.FINANCIAL_GATEWAY_DEFAULT_FEE_ACCOUNT, "rocks.kfs.Intacct.DEFAULTFEEACCOUNTNO" );

            RockMigrationHelper.UpdateCategory( Rock.SystemGuid.EntityType.ATTRIBUTE, "Intacct Export", "fa fa-calculator", "", KFSConst.Attribute.FINANCIAL_GATEWAY_ATTRIBUTE_CATEGORY );

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
            ", KFSConst.Attribute.FINANCIAL_GATEWAY_ATTRIBUTE_CATEGORY, KFSConst.Attribute.FINANCIAL_GATEWAY_DEFAULT_FEE_ACCOUNT, KFSConst.Attribute.FINANCIAL_GATEWAY_FEE_PROCESSING ) );

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

            // remove category
            RockMigrationHelper.DeleteCategory( KFSConst.Attribute.FINANCIAL_GATEWAY_ATTRIBUTE_CATEGORY );

            // re-order Financial Account attributes
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Class", "The Intacct dimension for Class Id.", 3, "", KFSConst.Attribute.FINANCIAL_ACCOUNT_CLASS, "rocks.kfs.Intacct.CLASSID" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Department", "The Intacct dimension for Department Id.", 4, "", KFSConst.Attribute.FINANCIAL_ACCOUNT_DEPARTMENT, "rocks.kfs.Intacct.DEPARTMENT" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Location", "The Intacct dimension for Location Id. Required if multi-entity enabled.", 5, "", KFSConst.Attribute.FINANCIAL_ACCOUNT_LOCATION, "rocks.kfs.Intacct.LOCATION" );
        }
    }
}
