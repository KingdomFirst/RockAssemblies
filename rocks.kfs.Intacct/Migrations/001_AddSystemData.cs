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

namespace com.kfs.Intacct.Migrations
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
            RockMigrationHelper.AddDefinedType( "Financial", "Financial Projects", "Used to designate what Project a Transaction should be associated with. Value: Journal Id, Description: Friendly Name", "C244D4C4-636F-4BCA-8E7C-1907933ABB74" );

            // reset the batch export date attribute key
            Sql( @"
                UPDATE [Attribute]
                SET [Key] = 'DateExported'
                WHERE [Guid] = '1C85E090-3DAB-4929-957E-A6140633724A'
            " );

            // batch export date
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialBatch", Rock.SystemGuid.FieldType.DATE_TIME, "", "", "Date Exported", "Date the batch was exported to Intacct.", 0, "", "1C85E090-3DAB-4929-957E-A6140633724A" );
            RockMigrationHelper.UpdateAttributeQualifier( "1C85E090-3DAB-4929-957E-A6140633724A", "displayDiff", "False", "D7E4CBA8-6772-4C61-9980-D88BFB3AE53D" );
            RockMigrationHelper.UpdateAttributeQualifier( "1C85E090-3DAB-4929-957E-A6140633724A", "format", "", "0F4ECB28-94F4-40A1-B074-61B80025452A" );

            // set the batch export date attribute key
            Sql( @"
                UPDATE [Attribute]
                SET [Key] = 'rocks.kfs.Intacct.DateExported'
                WHERE [Guid] = '1C85E090-3DAB-4929-957E-A6140633724A'
            " );

            // reset the transaction detail project attribute key
            Sql( @"
                UPDATE [Attribute]
                SET [Key] = 'TransactionDetailProject'
                WHERE [Guid] = '1615E093-51DF-4857-AED7-4F80DD36BE8B'
            " );

            // transaction detail project
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialTransactionDetail", Rock.SystemGuid.FieldType.DEFINED_VALUE, "", "", "Transaction Detail Project", "Designates the Project at the Financial Transaction Detail Level", 0, "", "1615E093-51DF-4857-AED7-4F80DD36BE8B" );
            RockMigrationHelper.UpdateAttributeQualifier( "1615E093-51DF-4857-AED7-4F80DD36BE8B", "allowmultiple", "False", "9D39DBC5-E9EF-4AD6-995A-3512403A4AF6" );
            RockMigrationHelper.UpdateAttributeQualifier( "1615E093-51DF-4857-AED7-4F80DD36BE8B", "definedtype", "", "02BAC8C6-D45B-4CC1-9DE2-7D4B3DBABF64" );
            RockMigrationHelper.UpdateAttributeQualifier( "1615E093-51DF-4857-AED7-4F80DD36BE8B", "displaydescription", "True", "8955E6D0-9905-4923-9501-5BEEC69DFA49" );
            RockMigrationHelper.UpdateAttributeQualifier( "1615E093-51DF-4857-AED7-4F80DD36BE8B", "enhancedselection", "True", "D99AF6BA-C6E6-40AA-90A3-4C30E1A76DAE" );

            // set the transaction detail project attribute key
            Sql( @"
                UPDATE [Attribute]
                SET [Key] = 'rocks.kfs.Intacct.PROJECTID'
                WHERE [Guid] = '1615E093-51DF-4857-AED7-4F80DD36BE8B'
            " );

            // reset the account project attribute key
            Sql( @"
                UPDATE [Attribute]
                SET [Key] = 'DefaultProject'
                WHERE [Guid] = '115519C9-EDFA-4BB5-A512-102C798F17F4'
            " );

            // account project
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.DEFINED_VALUE, "", "", "Default Project", "Designates the Project at the Financial Account Level", 0, "", "115519C9-EDFA-4BB5-A512-102C798F17F4" );
            RockMigrationHelper.UpdateAttributeQualifier( "115519C9-EDFA-4BB5-A512-102C798F17F4", "allowmultiple", "False", "C87D26C4-DB81-4CDE-8890-5AB739424331" );
            RockMigrationHelper.UpdateAttributeQualifier( "115519C9-EDFA-4BB5-A512-102C798F17F4", "definedtype", "", "E2702E6F-0C1E-4EE4-9F0D-991877E75754" );
            RockMigrationHelper.UpdateAttributeQualifier( "115519C9-EDFA-4BB5-A512-102C798F17F4", "displaydescription", "True", "4BF01113-77B8-4628-B3A9-8286654DE65E" );
            RockMigrationHelper.UpdateAttributeQualifier( "115519C9-EDFA-4BB5-A512-102C798F17F4", "enhancedselection", "True", "40BC40C5-91B1-41E3-87F2-D51610FA8CCE" );

            // set the account project attribute key
            Sql( @"
                UPDATE [Attribute]
                SET [Key] = 'rocks.kfs.Intacct.PROJECTID'
                WHERE [Guid] = '115519C9-EDFA-4BB5-A512-102C798F17F4'
            " );

            // set defined type qualifers
            Sql( @"
                DECLARE @ProjectDefinedTypeId int = ( SELECT TOP 1 [Id] FROM [DefinedType] WHERE [Guid] = 'C244D4C4-636F-4BCA-8E7C-1907933ABB74' )

                UPDATE [AttributeQualifier] SET [Value] = CAST( @ProjectDefinedTypeId AS varchar )
                WHERE [Guid] = '02BAC8C6-D45B-4CC1-9DE2-7D4B3DBABF64'

                UPDATE [AttributeQualifier] SET [Value] = CAST( @ProjectDefinedTypeId AS varchar )
                WHERE [Guid] = 'E2702E6F-0C1E-4EE4-9F0D-991877E75754'
            " );

            // reset the gl credit account attribute key
            Sql( @"
                UPDATE [Attribute]
                SET [Key] = 'CreditAccount'
                WHERE [Guid] = '8D790DD0-D84F-4DE2-9C7A-356D4590439E'
            " );

            // reset the gl debit account attribute key
            Sql( @"
                UPDATE [Attribute]
                SET [Key] = 'DebitAccount'
                WHERE [Guid] = '48E1B80E-5E8D-4016-B64E-F2527F328EA7'
            " );

            // reset the gl class attribute key
            Sql( @"
                UPDATE [Attribute]
                SET [Key] = 'Class'
                WHERE [Guid] = '76EFF760-B5B5-4053-8EBD-C329ECB85032'
            " );

            // reset the gl department attribute key
            Sql( @"
                UPDATE [Attribute]
                SET [Key] = 'Department'
                WHERE [Guid] = 'CD56CBD9-CA3B-49F2-BCA5-73A7C3F19328'
            " );

            // reset the gl location attribute key
            Sql( @"
                UPDATE [Attribute]
                SET [Key] = 'Location'
                WHERE [Guid] = 'CFA818F9-3163-45FE-AA03-AAF8DEDCF48D'
            " );

            // account gl attributes
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Credit Account", "Account number to use for credit column. Required by Intacct.", 1, "", "8D790DD0-D84F-4DE2-9C7A-356D4590439E" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Debit Account", "Account number to use for debit column. Required by Intacct.", 2, "", "48E1B80E-5E8D-4016-B64E-F2527F328EA7" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Class", "The Intacct dimension for Class Id.", 3, "", "76EFF760-B5B5-4053-8EBD-C329ECB85032" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Department", "The Intacct dimension for Department Id.", 4, "", "CD56CBD9-CA3B-49F2-BCA5-73A7C3F19328" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Location", "The Intacct dimension for Location Id. Required if multi-entity enabled.", 5, "", "CFA818F9-3163-45FE-AA03-AAF8DEDCF48D" );

            // set the gl credit account attribute key
            Sql( @"
                UPDATE [Attribute]
                SET [Key] = 'rocks.kfs.Intacct.ACCOUNTNO'
                WHERE [Guid] = '8D790DD0-D84F-4DE2-9C7A-356D4590439E'
            " );

            // set the gl debit account attribute key
            Sql( @"
                UPDATE [Attribute]
                SET [Key] = 'rocks.kfs.Intacct.DEBITACCOUNTNO'
                WHERE [Guid] = '48E1B80E-5E8D-4016-B64E-F2527F328EA7'
            " );

            // set the gl class attribute key
            Sql( @"
                UPDATE [Attribute]
                SET [Key] = 'rocks.kfs.Intacct.CLASSID'
                WHERE [Guid] = '76EFF760-B5B5-4053-8EBD-C329ECB85032'
            " );

            // set the gl department attribute key
            Sql( @"
                UPDATE [Attribute]
                SET [Key] = 'rocks.kfs.Intacct.DEPARTMENT'
                WHERE [Guid] = 'CD56CBD9-CA3B-49F2-BCA5-73A7C3F19328'
            " );

            // set the gl location attribute key
            Sql( @"
                UPDATE [Attribute]
                SET [Key] = 'rocks.kfs.Intacct.LOCATION'
                WHERE [Guid] = 'CFA818F9-3163-45FE-AA03-AAF8DEDCF48D'
            " );

            RockMigrationHelper.UpdateCategory( "5997C8D3-8840-4591-99A5-552919F90CBD", "Intacct Export", "fa fa-calculator", "", "BD629E8D-10D9-4C9F-9796-6358F2127483" ); // batch
            RockMigrationHelper.UpdateCategory( "5997C8D3-8840-4591-99A5-552919F90CBD", "Intacct Export", "fa fa-calculator", "", "48CC8F06-B299-434A-9D2A-8D83CC5B2EE5" ); // transaction detail
            RockMigrationHelper.UpdateCategory( "5997C8D3-8840-4591-99A5-552919F90CBD", "Intacct Export", "fa fa-calculator", "", "7361A954-350A-41F1-9D94-AD2CF4030CA5" ); // account

            Sql( @"
                --
                -- Set FinancialBatch Date Exported attribute to category
                --
                DECLARE @BatchEntityTypeId int = ( SELECT TOP 1 [Id] FROM [EntityType] WHERE [Guid] = 'BDD09C8E-2C52-4D08-9062-BE7D52D190C2' )
                DECLARE @BatchCategoryId int = ( SELECT TOP 1 [Id] FROM [Category] WHERE [Guid] = 'BD629E8D-10D9-4C9F-9796-6358F2127483' )

                UPDATE [Category]
                SET [EntityTypeQualifierColumn] = 'EntityTypeId',  [EntityTypeQualifierValue] = @BatchEntityTypeId
                WHERE [Id] = @BatchCategoryId

                DECLARE @BatchDateExportedId int = ( SELECT [Id] FROM [Attribute] WHERE [Guid] = '1C85E090-3DAB-4929-957E-A6140633724A' )

                IF NOT EXISTS ( SELECT [AttributeId], [CategoryId] FROM [AttributeCategory] WHERE [AttributeId] = @BatchDateExportedId AND [CategoryId] = @BatchCategoryId )

                BEGIN
                    INSERT INTO [AttributeCategory]
                    SELECT [Id], @BatchCategoryId
                    FROM [Attribute]
                    WHERE [Guid] = '1C85E090-3DAB-4929-957E-A6140633724A'
                END

                --
                -- Set FinancialTransactionDetail Project attribute to category
                --
                DECLARE @TransactionDetailEntityTypeId int = ( SELECT TOP 1 [Id] FROM [EntityType] WHERE [Guid] = 'AC4AC28B-8E7E-4D7E-85DB-DFFB4F3ADCCE' )
                DECLARE @TransactionDetailCategoryId int = ( SELECT TOP 1 [Id] FROM [Category] WHERE [Guid] = '48CC8F06-B299-434A-9D2A-8D83CC5B2EE5' )

                UPDATE [Category]
                SET [EntityTypeQualifierColumn] = 'EntityTypeId',  [EntityTypeQualifierValue] = @TransactionDetailEntityTypeId
                WHERE [Id] = @TransactionDetailCategoryId

                DECLARE @TransactionDetailProjectId int = ( SELECT [Id] FROM [Attribute] WHERE [Guid] = '1615E093-51DF-4857-AED7-4F80DD36BE8B' )

                IF NOT EXISTS ( SELECT [AttributeId], [CategoryId] FROM [AttributeCategory] WHERE [AttributeId] = @TransactionDetailProjectId AND [CategoryId] = @TransactionDetailCategoryId )

                BEGIN
                    INSERT INTO [AttributeCategory]
                    SELECT [Id], @TransactionDetailCategoryId
                    FROM [Attribute]
                    WHERE [Guid] = '1615E093-51DF-4857-AED7-4F80DD36BE8B'
                END

                --
                -- Set FinancialAccount attributes to category
                --
                DECLARE @AccountEntityTypeId int = ( SELECT TOP 1 [Id] FROM [EntityType] WHERE [Guid] = '798BCE48-6AA7-4983-9214-F9BCEFB4521D' )
                DECLARE @AccountCategoryId int = ( SELECT TOP 1 [Id] FROM [Category] WHERE [Guid] = '7361A954-350A-41F1-9D94-AD2CF4030CA5' )

                UPDATE [Category]
                SET [EntityTypeQualifierColumn] = 'EntityTypeId',  [EntityTypeQualifierValue] = @AccountEntityTypeId
                WHERE [Id] = @AccountCategoryId

                DECLARE @AccountDefaultProject int = ( SELECT [Id] FROM [Attribute] WHERE [Guid] = '115519C9-EDFA-4BB5-A512-102C798F17F4' )

                IF NOT EXISTS ( SELECT [AttributeId], [CategoryId] FROM [AttributeCategory] WHERE [AttributeId] = @AccountDefaultProject AND [CategoryId] = @AccountCategoryId )

                BEGIN
                    INSERT INTO [AttributeCategory]
                    SELECT [Id], @AccountCategoryId
                    FROM [Attribute]
                    WHERE [Guid] = '115519C9-EDFA-4BB5-A512-102C798F17F4'
                END

                DECLARE @AccountCredit int = ( SELECT [Id] FROM [Attribute] WHERE [Guid] = '8D790DD0-D84F-4DE2-9C7A-356D4590439E' )

                IF NOT EXISTS ( SELECT [AttributeId], [CategoryId] FROM [AttributeCategory] WHERE [AttributeId] = @AccountCredit AND [CategoryId] = @AccountCategoryId )

                BEGIN
                    INSERT INTO [AttributeCategory]
                    SELECT [Id], @AccountCategoryId
                    FROM [Attribute]
                    WHERE [Guid] = '8D790DD0-D84F-4DE2-9C7A-356D4590439E'
                END

                DECLARE @AccountDebit int = ( SELECT [Id] FROM [Attribute] WHERE [Guid] = '48E1B80E-5E8D-4016-B64E-F2527F328EA7' )

                IF NOT EXISTS ( SELECT [AttributeId], [CategoryId] FROM [AttributeCategory] WHERE [AttributeId] = @AccountDebit AND [CategoryId] = @AccountCategoryId )

                BEGIN
                    INSERT INTO [AttributeCategory]
                    SELECT [Id], @AccountCategoryId
                    FROM [Attribute]
                    WHERE [Guid] = '48E1B80E-5E8D-4016-B64E-F2527F328EA7'
                END

                DECLARE @AccountClass int = ( SELECT [Id] FROM [Attribute] WHERE [Guid] = '76EFF760-B5B5-4053-8EBD-C329ECB85032' )

                IF NOT EXISTS ( SELECT [AttributeId], [CategoryId] FROM [AttributeCategory] WHERE [AttributeId] = @AccountClass AND [CategoryId] = @AccountCategoryId )

                BEGIN
                    INSERT INTO [AttributeCategory]
                    SELECT [Id], @AccountCategoryId
                    FROM [Attribute]
                    WHERE [Guid] = '76EFF760-B5B5-4053-8EBD-C329ECB85032'
                END

                DECLARE @AccountDepartment int = ( SELECT [Id] FROM [Attribute] WHERE [Guid] = 'CD56CBD9-CA3B-49F2-BCA5-73A7C3F19328' )

                IF NOT EXISTS ( SELECT [AttributeId], [CategoryId] FROM [AttributeCategory] WHERE [AttributeId] = @AccountDepartment AND [CategoryId] = @AccountCategoryId )

                BEGIN
                    INSERT INTO [AttributeCategory]
                    SELECT [Id], @AccountCategoryId
                    FROM [Attribute]
                    WHERE [Guid] = 'CD56CBD9-CA3B-49F2-BCA5-73A7C3F19328'
                END

                DECLARE @AccountLocation int = ( SELECT [Id] FROM [Attribute] WHERE [Guid] = 'CFA818F9-3163-45FE-AA03-AAF8DEDCF48D' )

                IF NOT EXISTS ( SELECT [AttributeId], [CategoryId] FROM [AttributeCategory] WHERE [AttributeId] = @AccountLocation AND [CategoryId] = @AccountCategoryId )

                BEGIN
                    INSERT INTO [AttributeCategory]
                    SELECT [Id], @AccountCategoryId
                    FROM [Attribute]
                    WHERE [Guid] = 'CFA818F9-3163-45FE-AA03-AAF8DEDCF48D'
                END
            " );

            // create page for project defined type
            RockMigrationHelper.AddPage( true, Rock.SystemGuid.Page.ADMINISTRATION_FINANCE, "D65F783D-87A9-4CC9-8110-E83466A0EADB", "Intacct Projects", "", "06EB0D2D-A864-48F7-86E3-D72687E3A03C", "fa fa-clipboard", "2B630A3B-E081-4204-A3E4-17BB3A5F063D" );

            // add defined type detail to intacct project page
            RockMigrationHelper.AddBlock( true, "06EB0D2D-A864-48F7-86E3-D72687E3A03C", "", "08C35F15-9AF7-468F-9D50-CDFD3D21220C", "Defined Type Detail", "Main", "", "", 0, "81882390-0BAF-4DAC-ADDF-2637C47ECD7B" );
            RockMigrationHelper.AddBlockAttributeValue( true, "81882390-0BAF-4DAC-ADDF-2637C47ECD7B", "0305EF98-C791-4626-9996-F189B9BB674C", @"C244D4C4-636F-4BCA-8E7C-1907933ABB74" );

            // add defined value list block and set to projects defined type
            RockMigrationHelper.AddBlock( true, "06EB0D2D-A864-48F7-86E3-D72687E3A03C", "", "0AB2D5E9-9272-47D5-90E4-4AA838D2D3EE", "Intacct Projects", "Main", "", "", 0, "B9A1B5EA-3402-4F68-9190-5EA10B166569" );
            RockMigrationHelper.AddBlockAttributeValue( true, "B9A1B5EA-3402-4F68-9190-5EA10B166569", "9280D61F-C4F3-4A3E-A9BB-BCD67FF78637", "C244D4C4-636F-4BCA-8E7C-1907933ABB74" );

            // block type
            RockMigrationHelper.RenameBlockType( "~/Plugins/com_kfs/Intacct/BatchToJournal.ascx", "~/Plugins/rocks_kfs/Intacct/BatchToJournal.ascx" );
            RockMigrationHelper.UpdateBlockType( "Intacct Batch to Journal", "Block used to create Journal Entries in Intacct from a Rock Financial Batch.", "~/Plugins/rocks_kfs/Intacct/BatchToJournal.ascx", "KFS > Intacct", "5F859264-2E47-41FF-AA63-B57FE400BBC2" );

            // block on the Financial Batch Details page
            RockMigrationHelper.AddBlock( true, Rock.SystemGuid.Page.FINANCIAL_BATCH_DETAIL, "", "5F859264-2E47-41FF-AA63-B57FE400BBC2", "Intacct Batch To Journal", "Main", "", "", 0, "328FD600-A246-4326-A00C-943A4DF7DC39" );
        }

        /// <summary>
        /// The commands to undo a migration from a specific version
        /// </summary>
        public override void Down()
        {
            RockMigrationHelper.DeleteBlock( "328FD600-A246-4326-A00C-943A4DF7DC39" );
            RockMigrationHelper.DeleteBlockAttributeValue( "81882390-0BAF-4DAC-ADDF-2637C47ECD7B", "0305EF98-C791-4626-9996-F189B9BB674C" );
            RockMigrationHelper.DeleteBlock( "81882390-0BAF-4DAC-ADDF-2637C47ECD7B" );
            RockMigrationHelper.DeleteBlockAttributeValue( "B9A1B5EA-3402-4F68-9190-5EA10B166569", "9280D61F-C4F3-4A3E-A9BB-BCD67FF78637" );
            RockMigrationHelper.DeleteBlock( "B9A1B5EA-3402-4F68-9190-5EA10B166569" );
            RockMigrationHelper.DeletePage( "06EB0D2D-A864-48F7-86E3-D72687E3A03C" );
        }
    }
}
