using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rock.Plugin;

namespace com.kfs.ShelbyBatchExport.Migrations
{
    [MigrationNumber( 5, "1.8.0" )]
    class ResetToExcelFormat : Migration
    {
        /// <summary>
        /// The commands to run to migrate plugin to the specific version
        /// </summary>
        public override void Up()
        {
            //
            // Remove Batch Export Details Block from the Batch Details Page
            //
            RockMigrationHelper.DeleteBlockAttributeValue( "AB2F7C43-74F3-46A4-A005-F690D0A612D2", "DD9CD395-A4F0-4110-A349-C44EDFC0258B" );
            RockMigrationHelper.DeleteBlock( "AB2F7C43-74F3-46A4-A005-F690D0A612D2" );

            //
            // Remove the old export grid
            //
            RockMigrationHelper.DeleteBlock( "E209EFB6-DE18-4DD3-8B15-F7BE418C4955" );

            //
            // Change the Attribute Keys, names, and grid display
            //
            Sql( @"
                DECLARE @DateExportedAttributeId INT = (SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '4B6576DD-82F6-419F-8DF0-467D2636822D')
                DECLARE @TransactionProjectAttributeId INT = (SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '365134A6-D516-48E0-AC67-A011D5D59D99')
                DECLARE @TransactionDetailProjectAttributeId INT = (SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '951FAFFD-0513-4E31-9271-87853469E85E')
                DECLARE @FinancialAccountProjectAttributeId INT = (SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '85422EA2-AC4E-44E5-99B9-30C131116734')
                DECLARE @CompanyAttributeId INT = (SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = 'A211D2C9-249D-4D33-B120-A3EAB37C1EDF')
                DECLARE @FundAttributeId INT = (SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = 'B83D7934-F85A-42B7-AD0E-B4E16D63C189')
                DECLARE @DepartmentAttributeId INT = (SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '2C1EE0CC-D329-453B-B4F0-29549E24ED05')
                DECLARE @CreditAccountAttributeId INT = (SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '0D114FB9-B1AA-4D6D-B0F3-9BB739710992')
                DECLARE @DebitAccountAttributeId INT = (SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = 'FD4EF8CC-DDB7-4DBD-9FD1-601A0119850B')

                IF @DateExportedAttributeId IS NOT NULL
                BEGIN
	                UPDATE [Attribute]
	                SET [Name] = 'Date Exported', [Key] = 'com.kfs.ShelbyFinancials.DateExported', [IsGridColumn] = 1
	                WHERE [Id] = @DateExportedAttributeId
                END

                IF @TransactionProjectAttributeId IS NOT NULL
                BEGIN
	                UPDATE [Attribute]
	                SET [Key] = 'com.kfs.ShelbyFinancials.Project'
	                WHERE [Id] = @TransactionProjectAttributeId
                END

                IF @TransactionDetailProjectAttributeId IS NOT NULL
                BEGIN
	                UPDATE [Attribute]
	                SET [Key] = 'com.kfs.ShelbyFinancials.Project'
	                WHERE [Id] = @TransactionDetailProjectAttributeId
                END

                IF @FinancialAccountProjectAttributeId IS NOT NULL
                BEGIN
	                UPDATE [Attribute]
	                SET [Key] = 'com.kfs.ShelbyFinancials.Project'
	                WHERE [Id] = @FinancialAccountProjectAttributeId
                END

                IF @CompanyAttributeId IS NOT NULL
                BEGIN
	                UPDATE [Attribute]
	                SET [Key] = 'com.kfs.ShelbyFinancials.Company'
	                WHERE [Id] = @CompanyAttributeId
                END

                IF @FundAttributeId IS NOT NULL
                BEGIN
	                UPDATE [Attribute]
	                SET [Key] = 'com.kfs.ShelbyFinancials.Fund'
	                WHERE [Id] = @FundAttributeId
                END

                IF @DepartmentAttributeId IS NOT NULL
                BEGIN
	                UPDATE [Attribute]
	                SET [Name] = 'Department', [Key] = 'com.kfs.ShelbyFinancials.Department'
	                WHERE [Id] = @DepartmentAttributeId
                END

                IF @CreditAccountAttributeId IS NOT NULL
                BEGIN
	                UPDATE [Attribute]
	                SET [Name] = 'Credit Account', [Key] = 'com.kfs.ShelbyFinancials.CreditAccount'
	                WHERE [Id] = @CreditAccountAttributeId
                END

                IF @DebitAccountAttributeId IS NOT NULL
                BEGIN
	                UPDATE [Attribute]
	                SET [Name] = 'Debit Account', [Key] = 'com.kfs.ShelbyFinancials.DebitAccount'
	                WHERE [Id] = @DebitAccountAttributeId
                END
            " );

            RockMigrationHelper.AddEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Region", "", "", 6, "", "9B67459C-3C61-491D-B072-9A9830FBB18F", "com.kfs.ShelbyFinancials.Region" );
            RockMigrationHelper.AddEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Super Fund", "", "", 7, "", "8B8D8BBF-B763-4314-87D2-00B9B8BA5A0F", "com.kfs.ShelbyFinancials.SuperFund" );
            RockMigrationHelper.AddEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Location", "", "", 8, "", "22699ECA-BB71-4EFD-B416-17B41ED3DBEC", "com.kfs.ShelbyFinancials.Location" );
            RockMigrationHelper.AddEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Cost Center", "", "", 9, "", "CD925E61-F87D-461F-9EFA-C1E14397FC4D", "com.kfs.ShelbyFinancials.CostCenter" );
            RockMigrationHelper.AddEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Account Sub", "", "", 10, "", "65D935EC-3501-41A6-A2C5-CABC62AB9EF1", "com.kfs.ShelbyFinancials.AccountSub" );

            //
            // Add new blocks
            //
            RockMigrationHelper.UpdateBlockType( "Shelby Financials Batch to Journal", "Block used to create Journal Entries in Shelby Financials from a Rock Financial Batch.", "~/Plugins/com_kfs/ShelbyFinancials/BatchToJournal.ascx", "KFS > Shelby Financials", "235C370C-2CD7-4289-8B68-A8617F58B22B" );
            RockMigrationHelper.AddBlock( Rock.SystemGuid.Page.FINANCIAL_BATCH_DETAIL, "", "235C370C-2CD7-4289-8B68-A8617F58B22B", "Shelby GL Export", "Main", "", "", 0, "00FC3A61-775D-4DE5-BC19-A1556FF465EA" );

            // block type
            RockMigrationHelper.UpdateBlockType( "Shelby Financials Batches to Journal", "Block used to create Journal Entries in Shelby Financials from multiple Rock Financial Batches.", "~/Plugins/com_kfs/ShelbyFinancials/BatchesToJournal.ascx", "KFS > Shelby Financials", "B6F19C75-9B23-4EC5-810B-E05A5E11033F" );
            RockMigrationHelper.AddBlockTypeAttribute( "B6F19C75-9B23-4EC5-810B-E05A5E11033F", Rock.SystemGuid.FieldType.PAGE_REFERENCE, "Detail Page", "DetailPage", "", "", 0, Rock.SystemGuid.Page.FINANCIAL_BATCH_DETAIL, "CBC91780-4D02-47D5-B4AD-55AE2D5EBB49", true );

            // block on the Shelby batch export page
            RockMigrationHelper.AddBlock( "3F123421-E5DE-474E-9F56-AAFCEDE115EF", "", "B6F19C75-9B23-4EC5-810B-E05A5E11033F", "Shelby Financials Batches To Journal", "Main", "", "", 0, "2DC7BB64-EBDB-4973-85A8-E2AF82F76256" );

            // Clear all cached items
            Rock.Web.Cache.RockCache.ClearAllCachedItems();
        }

        /// <summary>
        /// The commands to undo a migration from a specific version
        /// </summary>
        public override void Down()
        {
            RockMigrationHelper.DeleteAttribute( "9B67459C-3C61-491D-B072-9A9830FBB18F" );
            RockMigrationHelper.DeleteAttribute( "8B8D8BBF-B763-4314-87D2-00B9B8BA5A0F" );
            RockMigrationHelper.DeleteAttribute( "22699ECA-BB71-4EFD-B416-17B41ED3DBEC" );
            RockMigrationHelper.DeleteAttribute( "CD925E61-F87D-461F-9EFA-C1E14397FC4D" );
            RockMigrationHelper.DeleteAttribute( "65D935EC-3501-41A6-A2C5-CABC62AB9EF1" );

            RockMigrationHelper.DeleteBlock( "00FC3A61-775D-4DE5-BC19-A1556FF465EA" );
            RockMigrationHelper.DeleteBlock( "2DC7BB64-EBDB-4973-85A8-E2AF82F76256" );
        }
    }
}
