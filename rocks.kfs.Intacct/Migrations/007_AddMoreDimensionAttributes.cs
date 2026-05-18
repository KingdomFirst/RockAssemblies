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
using Rock.Plugin;
using rocks.kfs.Intacct;

namespace rocks.kfs.Intacct.Migrations
{
    [MigrationNumber( 7, "1.16.0" )]
    public class AddMoreDimensionAttributes : Migration
    {
        /// <summary>
        /// The commands to run to migrate plugin to the specific version
        /// </summary>
        public override void Up()
        {
            // Add new dimension attributes for Financial Accounts

            RockMigrationHelper.AddOrUpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Credit Task", "Credit Task", "The Intacct dimension for Task to be used for assigned Credit Account. A valid Default Credit Project value is required for Credit Task value to be used.", 2, "", SystemGuid.Attribute.FINANCIAL_ACCOUNT_TASK, "rocks.kfs.Intacct.TASKID" );
            RockMigrationHelper.AddOrUpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Credit Customer", "Credit Customer", "The Intacct dimension for Customer to be used for assigned Credit Account.", 6, "", SystemGuid.Attribute.FINANCIAL_ACCOUNT_CUSTOMER, "rocks.kfs.Intacct.CUSTOMERID" );
            RockMigrationHelper.AddOrUpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Credit Item", "Credit Item", "The Intacct dimension for Item to be used for assigned Credit Account.", 7, "", SystemGuid.Attribute.FINANCIAL_ACCOUNT_ITEM, "rocks.kfs.Intacct.ITEMID" );
            RockMigrationHelper.AddOrUpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Credit Vendor", "Credit Vendor", "The Intacct dimension for Vendor to be used for assigned Credit Account.", 8, "", SystemGuid.Attribute.FINANCIAL_ACCOUNT_VENDOR, "rocks.kfs.Intacct.VENDORID" );
            RockMigrationHelper.AddOrUpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Credit Employee", "Credit Employee", "The Intacct dimension for Employee to be used for assigned Credit Account.", 9, "", SystemGuid.Attribute.FINANCIAL_ACCOUNT_EMPLOYEE, "rocks.kfs.Intacct.EMPLOYEEID" );
            RockMigrationHelper.AddOrUpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Credit Contract Id", "Credit Contract Id", "The Intacct dimension for Contract Id to be used for assigned Credit Account. Not supported in Other Receipts mode.", 10, "", SystemGuid.Attribute.FINANCIAL_ACCOUNT_CONTRACTID, "rocks.kfs.Intacct.CONTRACTID" );
            RockMigrationHelper.AddOrUpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Credit Warehouse Id", "Credit Warehouse Id", "The Intacct dimension for Warehouse Id to be used for assigned Credit Account. Not supported in Other Receipts mode.", 11, "", SystemGuid.Attribute.FINANCIAL_ACCOUNT_WAREHOUSEID, "rocks.kfs.Intacct.WAREHOUSEID" );

            RockMigrationHelper.AddOrUpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Debit Task", "Debit Task", "The Intacct dimension for Task to be used for assigned Debit Account. A valid Default Debit Project value is required for Credit Task value to be used.", 14, "", SystemGuid.Attribute.FINANCIAL_ACCOUNT_TASK_DEBIT, "rocks.kfs.Intacct.DEBITTASKID" );
            RockMigrationHelper.AddOrUpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Debit Customer", "Debit Customer", "The Intacct dimension for Customer to be used for assigned Debit Account.", 18, "", SystemGuid.Attribute.FINANCIAL_ACCOUNT_CUSTOMER_DEBIT, "rocks.kfs.Intacct.DEBITCUSTOMERID" );
            RockMigrationHelper.AddOrUpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Debit Item", "Debit Item", "The Intacct dimension for Item to be used for assigned Debit Account.", 19, "", SystemGuid.Attribute.FINANCIAL_ACCOUNT_ITEM_DEBIT, "rocks.kfs.Intacct.DEBITITEMID" );
            RockMigrationHelper.AddOrUpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Debit Vendor", "Debit Vendor", "The Intacct dimension for Vendor to be used for assigned Debit Account.", 20, "", SystemGuid.Attribute.FINANCIAL_ACCOUNT_VENDOR_DEBIT, "rocks.kfs.Intacct.DEBITVENDORID" );
            RockMigrationHelper.AddOrUpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Debit Employee", "Debit Employee", "The Intacct dimension for Employee to be used for assigned Debit Account.", 21, "", SystemGuid.Attribute.FINANCIAL_ACCOUNT_EMPLOYEE_DEBIT, "rocks.kfs.Intacct.DEBITEMPLOYEEID" );
            RockMigrationHelper.AddOrUpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Credit Contract Id", "Credit Contract Id", "The Intacct dimension for Contract Id to be used for assigned Debit Account. Not supported in Other Receipts mode.", 22, "", SystemGuid.Attribute.FINANCIAL_ACCOUNT_CONTRACTID_DEBIT, "rocks.kfs.Intacct.DEBITCONTRACTID" );
            RockMigrationHelper.AddOrUpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Credit Warehouse Id", "Credit Warehouse Id", "The Intacct dimension for Warehouse Id to be used for assigned Debit Account. Not supported in Other Receipts mode.", 23, "", SystemGuid.Attribute.FINANCIAL_ACCOUNT_WAREHOUSEID_DEBIT, "rocks.kfs.Intacct.DEBITWAREHOUSEID" );

            // set attribute category for new attributes
            Sql( string.Format( @"
                --
                -- Set FinancialAccount attributes to category
                --

                DECLARE @AccountCategoryId int = ( SELECT TOP 1 [Id] FROM [Category] WHERE [Guid] = '{0}' )

                DECLARE @AccountCreditCustomer int = ( SELECT [Id] FROM [Attribute] WHERE [Guid] = '{1}' )

                IF NOT EXISTS ( SELECT [AttributeId], [CategoryId] FROM [AttributeCategory] WHERE [AttributeId] = @AccountCreditCustomer AND [CategoryId] = @AccountCategoryId )

                BEGIN
                    INSERT INTO [AttributeCategory]
                    SELECT @AccountCreditCustomer, @AccountCategoryId
                END

                DECLARE @AccountCreditItem int = ( SELECT [Id] FROM [Attribute] WHERE [Guid] = '{2}' )

                IF NOT EXISTS ( SELECT [AttributeId], [CategoryId] FROM [AttributeCategory] WHERE [AttributeId] = @AccountCreditItem AND [CategoryId] = @AccountCategoryId )

                BEGIN
                    INSERT INTO [AttributeCategory]
                    SELECT @AccountCreditItem, @AccountCategoryId
                END

                DECLARE @AccountCreditTask int = ( SELECT [Id] FROM [Attribute] WHERE [Guid] = '{3}' )

                IF NOT EXISTS ( SELECT [AttributeId], [CategoryId] FROM [AttributeCategory] WHERE [AttributeId] = @AccountCreditTask AND [CategoryId] = @AccountCategoryId )

                BEGIN
                    INSERT INTO [AttributeCategory]
                    SELECT @AccountCreditTask, @AccountCategoryId
                END

                DECLARE @AccountCreditVendor int = ( SELECT [Id] FROM [Attribute] WHERE [Guid] = '{4}' )

                IF NOT EXISTS ( SELECT [AttributeId], [CategoryId] FROM [AttributeCategory] WHERE [AttributeId] = @AccountCreditVendor AND [CategoryId] = @AccountCategoryId )

                BEGIN
                    INSERT INTO [AttributeCategory]
                    SELECT @AccountCreditVendor, @AccountCategoryId
                END

                DECLARE @AccountCreditEmployee int = ( SELECT [Id] FROM [Attribute] WHERE [Guid] = '{5}' )

                IF NOT EXISTS ( SELECT [AttributeId], [CategoryId] FROM [AttributeCategory] WHERE [AttributeId] = @AccountCreditEmployee AND [CategoryId] = @AccountCategoryId )

                BEGIN
                    INSERT INTO [AttributeCategory]
                    SELECT @AccountCreditEmployee, @AccountCategoryId
                END

                DECLARE @AccountDebitCustomer int = ( SELECT [Id] FROM [Attribute] WHERE [Guid] = '{6}' )

                IF NOT EXISTS ( SELECT [AttributeId], [CategoryId] FROM [AttributeCategory] WHERE [AttributeId] = @AccountDebitCustomer AND [CategoryId] = @AccountCategoryId )

                BEGIN
                    INSERT INTO [AttributeCategory]
                    SELECT @AccountDebitCustomer, @AccountCategoryId
                END

                DECLARE @AccountDebitItem int = ( SELECT [Id] FROM [Attribute] WHERE [Guid] = '{7}' )

                IF NOT EXISTS ( SELECT [AttributeId], [CategoryId] FROM [AttributeCategory] WHERE [AttributeId] = @AccountDebitItem AND [CategoryId] = @AccountCategoryId )

                BEGIN
                    INSERT INTO [AttributeCategory]
                    SELECT @AccountDebitItem, @AccountCategoryId
                END

                DECLARE @AccountDebitTask int = ( SELECT [Id] FROM [Attribute] WHERE [Guid] = '{8}' )

                IF NOT EXISTS ( SELECT [AttributeId], [CategoryId] FROM [AttributeCategory] WHERE [AttributeId] = @AccountDebitTask AND [CategoryId] = @AccountCategoryId )

                BEGIN
                    INSERT INTO [AttributeCategory]
                    SELECT @AccountDebitTask, @AccountCategoryId
                END

                DECLARE @AccountDebitVendor int = ( SELECT [Id] FROM [Attribute] WHERE [Guid] = '{9}' )

                IF NOT EXISTS ( SELECT [AttributeId], [CategoryId] FROM [AttributeCategory] WHERE [AttributeId] = @AccountDebitVendor AND [CategoryId] = @AccountCategoryId )

                BEGIN
                    INSERT INTO [AttributeCategory]
                    SELECT @AccountDebitVendor, @AccountCategoryId
                END

                DECLARE @AccountDebitEmployee int = ( SELECT [Id] FROM [Attribute] WHERE [Guid] = '{10}' )

                IF NOT EXISTS ( SELECT [AttributeId], [CategoryId] FROM [AttributeCategory] WHERE [AttributeId] = @AccountDebitEmployee AND [CategoryId] = @AccountCategoryId )

                BEGIN
                    INSERT INTO [AttributeCategory]
                    SELECT @AccountDebitEmployee, @AccountCategoryId
                END
            ", SystemGuid.Attribute.FINANCIAL_ACCOUNT_ATTRIBUTE_CATEGORY, SystemGuid.Attribute.FINANCIAL_ACCOUNT_CUSTOMER, SystemGuid.Attribute.FINANCIAL_ACCOUNT_ITEM, SystemGuid.Attribute.FINANCIAL_ACCOUNT_TASK, SystemGuid.Attribute.FINANCIAL_ACCOUNT_VENDOR, SystemGuid.Attribute.FINANCIAL_ACCOUNT_EMPLOYEE, SystemGuid.Attribute.FINANCIAL_ACCOUNT_CUSTOMER_DEBIT, SystemGuid.Attribute.FINANCIAL_ACCOUNT_ITEM_DEBIT, SystemGuid.Attribute.FINANCIAL_ACCOUNT_TASK_DEBIT, SystemGuid.Attribute.FINANCIAL_ACCOUNT_VENDOR_DEBIT, SystemGuid.Attribute.FINANCIAL_ACCOUNT_EMPLOYEE_DEBIT ) );

            // set dimension attributes to inactive so they don't show on the financial account edit screen until activated as needed.
            Sql( string.Format( @"

                UPDATE [Attribute] SET [IsActive] = 0
                WHERE [Guid] IN ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}')
            ", SystemGuid.Attribute.FINANCIAL_ACCOUNT_CUSTOMER, SystemGuid.Attribute.FINANCIAL_ACCOUNT_ITEM, SystemGuid.Attribute.FINANCIAL_ACCOUNT_TASK, SystemGuid.Attribute.FINANCIAL_ACCOUNT_VENDOR, SystemGuid.Attribute.FINANCIAL_ACCOUNT_EMPLOYEE, SystemGuid.Attribute.FINANCIAL_ACCOUNT_CUSTOMER_DEBIT, SystemGuid.Attribute.FINANCIAL_ACCOUNT_ITEM_DEBIT, SystemGuid.Attribute.FINANCIAL_ACCOUNT_TASK_DEBIT, SystemGuid.Attribute.FINANCIAL_ACCOUNT_VENDOR_DEBIT, SystemGuid.Attribute.FINANCIAL_ACCOUNT_EMPLOYEE_DEBIT ) );

            // reorder remaining attributes
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Credit Class", "The Intacct dimension for Class Id to be used for assigned Credit Account.", 3, "", SystemGuid.Attribute.FINANCIAL_ACCOUNT_CLASS, "rocks.kfs.Intacct.CLASSID" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Credit Department", "The Intacct dimension for Department Id to be used for assigned Credit Account.", 4, "", SystemGuid.Attribute.FINANCIAL_ACCOUNT_DEPARTMENT, "rocks.kfs.Intacct.DEPARTMENT" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Credit Location", "The Intacct dimension for Location Id to be used for assigned Credit Account. Required if multi-entity enabled.", 5, "", SystemGuid.Attribute.FINANCIAL_ACCOUNT_LOCATION, "rocks.kfs.Intacct.LOCATION" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Debit Account", "Account number to use for debit column. Required by Intacct.", 12, "", SystemGuid.Attribute.FINANCIAL_ACCOUNT_DEBIT_ACCOUNT, "rocks.kfs.Intacct.DEBITACCOUNTNO" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.DEFINED_VALUE, "", "", "Default Debit Project", "Designates the Project for the assigned Debit Account at the Financial Account Level.", 13, "", SystemGuid.Attribute.FINANCIAL_ACCOUNT_PROJECT_DEBIT, "rocks.kfs.Intacct.DEBITPROJECTID" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Debit Class", "The Intacct dimension for Class Id to be used for assigned Debit Account.", 15, "", SystemGuid.Attribute.FINANCIAL_ACCOUNT_CLASS_DEBIT, "rocks.kfs.Intacct.DEBITCLASSID" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Debit Department", "The Intacct dimension for Department Id to be used for assigned Debit Account.", 16, "", SystemGuid.Attribute.FINANCIAL_ACCOUNT_DEPARTMENT_DEBIT, "rocks.kfs.Intacct.DEBITDEPARTMENT" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Debit Location", "The Intacct dimension for Location Id to be used for assigned Debit Account. Required if multi-entity enabled.", 17, "", SystemGuid.Attribute.FINANCIAL_ACCOUNT_LOCATION_DEBIT, "rocks.kfs.Intacct.DEBITLOCATION" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Transaction Fee Account", "Expense account number for gateway transaction fees.", 24, "", SystemGuid.Attribute.FINANCIAL_ACCOUNT_FEE_ACCOUNT, "rocks.kfs.Intacct.FEEACCOUNTNO" );
        }

        /// <summary>
        /// The commands to undo a migration from a specific version
        /// </summary>
        public override void Down()
        {
            // remove attributes
            RockMigrationHelper.DeleteAttribute( SystemGuid.Attribute.FINANCIAL_ACCOUNT_CUSTOMER );
            RockMigrationHelper.DeleteAttribute( SystemGuid.Attribute.FINANCIAL_ACCOUNT_ITEM );
            RockMigrationHelper.DeleteAttribute( SystemGuid.Attribute.FINANCIAL_ACCOUNT_TASK );
            RockMigrationHelper.DeleteAttribute( SystemGuid.Attribute.FINANCIAL_ACCOUNT_VENDOR );
            RockMigrationHelper.DeleteAttribute( SystemGuid.Attribute.FINANCIAL_ACCOUNT_EMPLOYEE );
            RockMigrationHelper.DeleteAttribute( SystemGuid.Attribute.FINANCIAL_ACCOUNT_CUSTOMER_DEBIT );
            RockMigrationHelper.DeleteAttribute( SystemGuid.Attribute.FINANCIAL_ACCOUNT_ITEM );
            RockMigrationHelper.DeleteAttribute( SystemGuid.Attribute.FINANCIAL_ACCOUNT_TASK_DEBIT );
            RockMigrationHelper.DeleteAttribute( SystemGuid.Attribute.FINANCIAL_ACCOUNT_VENDOR_DEBIT );
            RockMigrationHelper.DeleteAttribute( SystemGuid.Attribute.FINANCIAL_ACCOUNT_EMPLOYEE_DEBIT );

            // update order of original attributes 
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Credit Class", "The Intacct dimension for Class Id to be used for assigned Credit Account.", 2, "", SystemGuid.Attribute.FINANCIAL_ACCOUNT_CLASS, "rocks.kfs.Intacct.CLASSID" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Credit Department", "The Intacct dimension for Department Id to be used for assigned Credit Account.", 3, "", SystemGuid.Attribute.FINANCIAL_ACCOUNT_DEPARTMENT, "rocks.kfs.Intacct.DEPARTMENT" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Credit Location", "The Intacct dimension for Location Id to be used for assigned Credit Account. Required if multi-entity enabled.", 4, "", SystemGuid.Attribute.FINANCIAL_ACCOUNT_LOCATION, "rocks.kfs.Intacct.LOCATION" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Debit Account", "Account number to use for debit column. Required by Intacct.", 5, "", SystemGuid.Attribute.FINANCIAL_ACCOUNT_DEBIT_ACCOUNT, "rocks.kfs.Intacct.DEBITACCOUNTNO" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.DEFINED_VALUE, "", "", "Default Debit Project", "Designates the Project for the assigned Debit Account at the Financial Account Level.", 6, "", SystemGuid.Attribute.FINANCIAL_ACCOUNT_PROJECT_DEBIT, "rocks.kfs.Intacct.DEBITPROJECTID" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Debit Class", "The Intacct dimension for Class Id to be used for assigned Debit Account.", 7, "", SystemGuid.Attribute.FINANCIAL_ACCOUNT_CLASS_DEBIT, "rocks.kfs.Intacct.DEBITCLASSID" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Debit Department", "The Intacct dimension for Department Id to be used for assigned Debit Account.", 8, "", SystemGuid.Attribute.FINANCIAL_ACCOUNT_DEPARTMENT_DEBIT, "rocks.kfs.Intacct.DEBITDEPARTMENT" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Debit Location", "The Intacct dimension for Location Id to be used for assigned Debit Account. Required if multi-entity enabled.", 9, "", SystemGuid.Attribute.FINANCIAL_ACCOUNT_LOCATION_DEBIT, "rocks.kfs.Intacct.DEBITLOCATION" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Transaction Fee Account", "Expense account number for gateway transaction fees.", 10, "", SystemGuid.Attribute.FINANCIAL_ACCOUNT_FEE_ACCOUNT, "rocks.kfs.Intacct.FEEACCOUNTNO" );
        }
    }
}
