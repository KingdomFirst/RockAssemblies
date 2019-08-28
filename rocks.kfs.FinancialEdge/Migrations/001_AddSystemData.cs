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

namespace rocks.kfs.FinancialEdge.Migrations
{
    [MigrationNumber( 1, "1.7.4" )]
    public class AddSystemData : Migration
    {
        /// <summary>
        /// The commands to run to migrate plugin to the specific version
        /// </summary>
        public override void Up()
        {
            // project defined type
            RockMigrationHelper.AddDefinedType( "Financial", "Financial Edge Projects", "Used to designate what Project a Transaction should be associated with. Value: Journal Id, Description: Friendly Name", "35178732-932A-472F-8D39-526B8E45625E" );

            // reset the batch export date attribute key
            Sql( @"
                UPDATE [Attribute]
                SET [Key] = 'DateExported'
                WHERE [Guid] = '16EFE0B4-E607-4960-BC92-8D66854E827A'
            " );

            // batch export date
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialBatch", Rock.SystemGuid.FieldType.DATE_TIME, "", "", "Date Exported", "Date the batch was exported to Financial Edge.", 0, "", "16EFE0B4-E607-4960-BC92-8D66854E827A" );
            RockMigrationHelper.UpdateAttributeQualifier( "16EFE0B4-E607-4960-BC92-8D66854E827A", "displayDiff", "False", "B707D9D4-1697-42A2-985A-CCFC93414365" );
            RockMigrationHelper.UpdateAttributeQualifier( "16EFE0B4-E607-4960-BC92-8D66854E827A", "format", "", "F0326D79-8C42-4D16-9E71-1E03DA0B1711" );

            // set the batch export date attribute key
            Sql( @"
                UPDATE [Attribute]
                SET [Key] = 'rocks.kfs.FinancialEdge.DateExported'
                WHERE [Guid] = '16EFE0B4-E607-4960-BC92-8D66854E827A'
            " );

            // reset the transaction detail project attribute key
            Sql( @"
                UPDATE [Attribute]
                SET [Key] = 'TransactionDetailProject'
                WHERE [Guid] = '0450CE22-2C04-453F-B688-5FC687FA59B0'
            " );

            // transaction detail project
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialTransactionDetail", Rock.SystemGuid.FieldType.DEFINED_VALUE, "", "", "Transaction Detail Project", "Designates the Project at the Financial Transaction Detail Level", 0, "", "0450CE22-2C04-453F-B688-5FC687FA59B0" );
            RockMigrationHelper.UpdateAttributeQualifier( "0450CE22-2C04-453F-B688-5FC687FA59B0", "allowmultiple", "False", "66C3ED8D-59EA-4013-80C9-7262FBEB8ADF" );
            RockMigrationHelper.UpdateAttributeQualifier( "0450CE22-2C04-453F-B688-5FC687FA59B0", "definedtype", "", "242EF4D2-0F55-481D-9065-A642E65736DE" );
            RockMigrationHelper.UpdateAttributeQualifier( "0450CE22-2C04-453F-B688-5FC687FA59B0", "displaydescription", "True", "6ED65431-6151-438F-BB7A-AC4AE953D08D" );
            RockMigrationHelper.UpdateAttributeQualifier( "0450CE22-2C04-453F-B688-5FC687FA59B0", "enhancedselection", "True", "DDE8F66C-15C5-44A1-90BD-87D5BE3FD6FC" );

            // set the transaction detail project attribute key
            Sql( @"
                UPDATE [Attribute]
                SET [Key] = 'rocks.kfs.FinancialEdge.PROJECTID'
                WHERE [Guid] = '0450CE22-2C04-453F-B688-5FC687FA59B0'
            " );

            // reset the credit account attribute key
            Sql( @"
                UPDATE [Attribute]
                SET [Key] = 'CreditAccount'
                WHERE [Guid] = '82688734-FEAF-48B2-9948-80EA225EC938'
            " );

            // reset the debit account attribute key
            Sql( @"
                UPDATE [Attribute]
                SET [Key] = 'DebitAccount'
                WHERE [Guid] = '0726BE0F-0B41-4FB5-A3CC-352B2B46A2CD'
            " );

            // account gl attributes
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Credit Account", "Account number to use for credit column. Required by FE.", 1, "", "82688734-FEAF-48B2-9948-80EA225EC938" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Debit Account", "Account number to use for debit column. Required by FE.", 2, "", "0726BE0F-0B41-4FB5-A3CC-352B2B46A2CD" );

            // set the credit account attribute key
            Sql( @"
                UPDATE [Attribute]
                SET [Key] = 'rocks.kfs.FinancialEdge.ACCOUNTNO'
                WHERE [Guid] = '82688734-FEAF-48B2-9948-80EA225EC938'
            " );

            // set the debit account attribute key
            Sql( @"
                UPDATE [Attribute]
                SET [Key] = 'rocks.kfs.FinancialEdge.DEBITACCOUNTNO'
                WHERE [Guid] = '0726BE0F-0B41-4FB5-A3CC-352B2B46A2CD'
            " );

            // reset the account project attribute key
            Sql( @"
                UPDATE [Attribute]
                SET [Key] = 'DefaultProject'
                WHERE [Guid] = '8F47F40F-01EF-44F6-BA4E-E1E75E9E4DAC'
            " );

            // account project
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.DEFINED_VALUE, "", "", "Default Project", "Designates the Project at the Financial Account Level", 0, "", "8F47F40F-01EF-44F6-BA4E-E1E75E9E4DAC" );
            RockMigrationHelper.UpdateAttributeQualifier( "8F47F40F-01EF-44F6-BA4E-E1E75E9E4DAC", "allowmultiple", "False", "5DB1B3FA-5A9E-4DCA-8730-7922838EF21F" );
            RockMigrationHelper.UpdateAttributeQualifier( "8F47F40F-01EF-44F6-BA4E-E1E75E9E4DAC", "definedtype", "", "E5635789-1F3D-4EE5-9230-80A61FC3788C" );
            RockMigrationHelper.UpdateAttributeQualifier( "8F47F40F-01EF-44F6-BA4E-E1E75E9E4DAC", "displaydescription", "True", "29A2EC92-E6E5-4399-8C89-B206264C6ABB" );
            RockMigrationHelper.UpdateAttributeQualifier( "8F47F40F-01EF-44F6-BA4E-E1E75E9E4DAC", "enhancedselection", "True", "C945C6C0-81AE-4C8E-A08D-210C61B7AC06" );

            // reset the account project attribute key
            Sql( @"
                UPDATE [Attribute]
                SET [Key] = 'rocks.kfs.FinancialEdge.PROJECTID'
                WHERE [Guid] = '8F47F40F-01EF-44F6-BA4E-E1E75E9E4DAC'
            " );

            // set defined type qualifers
            Sql( @"
                DECLARE @ProjectDefinedTypeId int = ( SELECT TOP 1 [Id] FROM [DefinedType] WHERE [Guid] = '35178732-932A-472F-8D39-526B8E45625E' )

                UPDATE [AttributeQualifier] SET [Value] = CAST( @ProjectDefinedTypeId AS varchar )
                WHERE [Guid] = '242EF4D2-0F55-481D-9065-A642E65736DE'

                UPDATE [AttributeQualifier] SET [Value] = CAST( @ProjectDefinedTypeId AS varchar )
                WHERE [Guid] = 'E5635789-1F3D-4EE5-9230-80A61FC3788C'
            " );

            RockMigrationHelper.UpdateCategory( "5997C8D3-8840-4591-99A5-552919F90CBD", "Financial Edge Export", "fa fa-calculator", "", "7EBDA9FC-667D-4A5A-88D9-894CEE5F0DF6" ); // batch
            RockMigrationHelper.UpdateCategory( "5997C8D3-8840-4591-99A5-552919F90CBD", "Financial Edge Export", "fa fa-calculator", "", "13EFF4FB-7FEE-4071-9DA8-FE911A1C8456" ); // transaction detail
            RockMigrationHelper.UpdateCategory( "5997C8D3-8840-4591-99A5-552919F90CBD", "Financial Edge Export", "fa fa-calculator", "", "FC9B74D6-ABB7-4D01-836C-C811859F0F62" ); // account

            Sql( @"
                --
                -- Set FinancialBatch Date Exported attribute to category
                --
                DECLARE @BatchEntityTypeId int = ( SELECT TOP 1 [Id] FROM [EntityType] WHERE [Guid] = 'BDD09C8E-2C52-4D08-9062-BE7D52D190C2' )
                DECLARE @BatchCategoryId int = ( SELECT TOP 1 [Id] FROM [Category] WHERE [Guid] = '7EBDA9FC-667D-4A5A-88D9-894CEE5F0DF6' )

                UPDATE [Category]
                SET [EntityTypeQualifierColumn] = 'EntityTypeId',  [EntityTypeQualifierValue] = @BatchEntityTypeId
                WHERE [Id] = @BatchCategoryId

				DECLARE @BatchDateExportedId int = ( SELECT [Id] FROM [Attribute] WHERE [Guid] = '16EFE0B4-E607-4960-BC92-8D66854E827A' )

				IF NOT EXISTS ( SELECT [AttributeId], [CategoryId] FROM [AttributeCategory] WHERE [AttributeId] = @BatchDateExportedId AND [CategoryId] = @BatchCategoryId )

                BEGIN
                    INSERT INTO [AttributeCategory]
                    SELECT [Id], @BatchCategoryId
                    FROM [Attribute]
                    WHERE [Guid] = '16EFE0B4-E607-4960-BC92-8D66854E827A'
                END

                --
                -- Set FinancialTransactionDetail Project attribute to category
                --
                DECLARE @TransactionDetailEntityTypeId int = ( SELECT TOP 1 [Id] FROM [EntityType] WHERE [Guid] = 'AC4AC28B-8E7E-4D7E-85DB-DFFB4F3ADCCE' )
                DECLARE @TransactionDetailCategoryId int = ( SELECT TOP 1 [Id] FROM [Category] WHERE [Guid] = '13EFF4FB-7FEE-4071-9DA8-FE911A1C8456' )

                UPDATE [Category]
                SET [EntityTypeQualifierColumn] = 'EntityTypeId',  [EntityTypeQualifierValue] = @TransactionDetailEntityTypeId
                WHERE [Id] = @TransactionDetailCategoryId

				DECLARE @TransactionDetailProjectId int = ( SELECT [Id] FROM [Attribute] WHERE [Guid] = '0450CE22-2C04-453F-B688-5FC687FA59B0' )

				IF NOT EXISTS ( SELECT [AttributeId], [CategoryId] FROM [AttributeCategory] WHERE [AttributeId] = @TransactionDetailProjectId AND [CategoryId] = @TransactionDetailCategoryId )

                BEGIN
                    INSERT INTO [AttributeCategory]
                    SELECT [Id], @TransactionDetailCategoryId
                    FROM [Attribute]
                    WHERE [Guid] = '0450CE22-2C04-453F-B688-5FC687FA59B0'
                END

                --
                -- Set FinancialAccount attributes to category
                --
                DECLARE @AccountEntityTypeId int = ( SELECT TOP 1 [Id] FROM [EntityType] WHERE [Guid] = '798BCE48-6AA7-4983-9214-F9BCEFB4521D' )
                DECLARE @AccountCategoryId int = ( SELECT TOP 1 [Id] FROM [Category] WHERE [Guid] = 'FC9B74D6-ABB7-4D01-836C-C811859F0F62' )

                UPDATE [Category]
                SET [EntityTypeQualifierColumn] = 'EntityTypeId',  [EntityTypeQualifierValue] = @AccountEntityTypeId
                WHERE [Id] = @AccountCategoryId

				DECLARE @AccountDefaultProject int = ( SELECT [Id] FROM [Attribute] WHERE [Guid] = '8F47F40F-01EF-44F6-BA4E-E1E75E9E4DAC' )

				IF NOT EXISTS ( SELECT [AttributeId], [CategoryId] FROM [AttributeCategory] WHERE [AttributeId] = @AccountDefaultProject AND [CategoryId] = @AccountCategoryId )

                BEGIN
                    INSERT INTO [AttributeCategory]
                    SELECT [Id], @AccountCategoryId
                    FROM [Attribute]
                    WHERE [Guid] = '8F47F40F-01EF-44F6-BA4E-E1E75E9E4DAC'
                END

				DECLARE @AccountCredit int = ( SELECT [Id] FROM [Attribute] WHERE [Guid] = '82688734-FEAF-48B2-9948-80EA225EC938' )

				IF NOT EXISTS ( SELECT [AttributeId], [CategoryId] FROM [AttributeCategory] WHERE [AttributeId] = @AccountCredit AND [CategoryId] = @AccountCategoryId )

                BEGIN
                    INSERT INTO [AttributeCategory]
                    SELECT [Id], @AccountCategoryId
                    FROM [Attribute]
                    WHERE [Guid] = '82688734-FEAF-48B2-9948-80EA225EC938'
                END

				DECLARE @AccountDebit int = ( SELECT [Id] FROM [Attribute] WHERE [Guid] = '0726BE0F-0B41-4FB5-A3CC-352B2B46A2CD' )

				IF NOT EXISTS ( SELECT [AttributeId], [CategoryId] FROM [AttributeCategory] WHERE [AttributeId] = @AccountDebit AND [CategoryId] = @AccountCategoryId )

                BEGIN
                    INSERT INTO [AttributeCategory]
                    SELECT [Id], @AccountCategoryId
                    FROM [Attribute]
                    WHERE [Guid] = '0726BE0F-0B41-4FB5-A3CC-352B2B46A2CD'
                END
            " );

            // create page for project defined type
            RockMigrationHelper.AddPage( true, Rock.SystemGuid.Page.ADMINISTRATION_FINANCE, "D65F783D-87A9-4CC9-8110-E83466A0EADB", "Financial Edge Projects", "", "D42C767A-A388-4565-AA08-77FAFF01CE5B", "fa fa-clipboard", "2B630A3B-E081-4204-A3E4-17BB3A5F063D" );

            // add defined type detail to Financial Edge project page
            RockMigrationHelper.AddBlock( true, "D42C767A-A388-4565-AA08-77FAFF01CE5B", "", "08C35F15-9AF7-468F-9D50-CDFD3D21220C", "Defined Type Detail", "Main", "", "", 0, "B2A6E042-A8B6-4911-A5E5-6860C3DED96A" );
            RockMigrationHelper.AddBlockAttributeValue( true, "B2A6E042-A8B6-4911-A5E5-6860C3DED96A", "0305EF98-C791-4626-9996-F189B9BB674C", @"35178732-932A-472F-8D39-526B8E45625E" );

            // add defined value list block and set to projects defined type
            RockMigrationHelper.AddBlock( true, "D42C767A-A388-4565-AA08-77FAFF01CE5B", "", "0AB2D5E9-9272-47D5-90E4-4AA838D2D3EE", "Financial Edge Projects", "Main", "", "", 1, "5F882086-AE21-4736-B908-BB2BF58ECA60" );
            RockMigrationHelper.AddBlockAttributeValue( true, "5F882086-AE21-4736-B908-BB2BF58ECA60", "9280D61F-C4F3-4A3E-A9BB-BCD67FF78637", "35178732-932A-472F-8D39-526B8E45625E" );

            // block type
            RockMigrationHelper.RenameBlockType( "~/Plugins/com_kfs/FinancialEdge/BatchToJournal.ascx", "~/Plugins/rocks_kfs/FinancialEdge/BatchToJournal.ascx" );
            RockMigrationHelper.UpdateBlockType( "Financial Edge Batch to Journal", "Block used to create Journal Entries in Financial Edge from a Rock Financial Batch.", "~/Plugins/rocks_kfs/FinancialEdge/BatchToJournal.ascx", "KFS > Financial Edge", "CC716B06-4674-4CBE-9C66-E7DCB42153CB" );

            // block on the Financial Batch Details page
            RockMigrationHelper.AddBlock( true, Rock.SystemGuid.Page.FINANCIAL_BATCH_DETAIL, "", "CC716B06-4674-4CBE-9C66-E7DCB42153CB", "Financial Edge Batch To Journal", "Main", "", "", 0, "EAB705EF-22AF-4A6E-9531-A094AB913DC3" );
        }

        /// <summary>
        /// The commands to undo a migration from a specific version
        /// </summary>
        public override void Down()
        {
            //
            // remove block and page
            //
            RockMigrationHelper.DeleteBlockAttributeValue( "B2A6E042-A8B6-4911-A5E5-6860C3DED96A", "0305EF98-C791-4626-9996-F189B9BB674C" );
            RockMigrationHelper.DeleteBlock( "B2A6E042-A8B6-4911-A5E5-6860C3DED96A" );
            RockMigrationHelper.DeleteBlockAttributeValue( "5F882086-AE21-4736-B908-BB2BF58ECA60", "9280D61F-C4F3-4A3E-A9BB-BCD67FF78637" );
            RockMigrationHelper.DeleteBlock( "5F882086-AE21-4736-B908-BB2BF58ECA60" );
            RockMigrationHelper.DeletePage( "D42C767A-A388-4565-AA08-77FAFF01CE5B" );

            //
            // remove block from batch details
            //
            RockMigrationHelper.DeleteBlock( "EAB705EF-22AF-4A6E-9531-A094AB913DC3" );
        }
    }
}
