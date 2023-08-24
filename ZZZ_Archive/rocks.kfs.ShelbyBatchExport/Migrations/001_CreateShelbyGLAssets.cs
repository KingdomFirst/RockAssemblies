using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rock.Plugin;

namespace rocks.kfs.ShelbyBatchExport.Migrations
{
    [MigrationNumber( 1, "1.6.1" )]
    public class CreateShelbyGLAssets : Migration
    {
        /// <summary>
        /// The commands to run to migrate plugin to the specific version
        /// </summary>
        public override void Up()
        {
            // block and page
            RockMigrationHelper.AddBlockType( "Shelby GL Export", "Lists all financial batches and provides Shelby GL Export capability", "~/Plugins/rocks_kfs/Finance/ShelbyGLExport.ascx", "KFS > Finance", "4A55A21B-F174-4012-AD6D-00BA9D76B1CA" );
            RockMigrationHelper.AddPage( Rock.SystemGuid.Page.FUNCTIONS_FINANCE, "D65F783D-87A9-4CC9-8110-E83466A0EADB", "Shelby GL Export", "", "408AAC12-B303-4565-AB97-0199F9F69A1B", "fa fa-archive", "EF65EFF2-99AC-4081-8E09-32A04518683A" );
            RockMigrationHelper.AddBlock( "408AAC12-B303-4565-AB97-0199F9F69A1B", "", "4A55A21B-F174-4012-AD6D-00BA9D76B1CA", "Shelby GL Export", "Main", "", "", 0, "7ED41FAE-BEE1-4307-AD4F-DEC77D455553" );

            // project defined type
            RockMigrationHelper.AddDefinedType( "Financial", "Financial Projects", "Used to designate what Project a Transaction should be associated with.", "1C9E0068-6840-4551-86F0-E12691CEC063" );
            RockMigrationHelper.AddDefinedTypeAttribute( "1C9E0068-6840-4551-86F0-E12691CEC063", Rock.SystemGuid.FieldType.TEXT, "GL Code", "Code", "", 0, "", "B85AA55C-ED3E-4E67-B022-81A7FC8C1E94" );

            // batch export date
            RockMigrationHelper.AddEntityAttribute( "Rock.Model.FinancialBatch", Rock.SystemGuid.FieldType.DATE_TIME, "", "", "Batch Exported", "", "Date a batch was exported", 0, "", "ADFC01F7-B446-479F-9BFE-30E3D68BCB83", "GLExport_BatchExported" );
            RockMigrationHelper.AddAttributeQualifier( "ADFC01F7-B446-479F-9BFE-30E3D68BCB83", "displayDiff", "False", "79D5F0E2-5A5F-46B6-B521-ADD2AB290ADB" );
            RockMigrationHelper.AddAttributeQualifier( "ADFC01F7-B446-479F-9BFE-30E3D68BCB83", "format", "", "06AC3DE6-BF7B-4DC4-AED4-9AFA5AB0A623" );

            // transaction project
            RockMigrationHelper.AddEntityAttribute( "Rock.Model.FinancialTransaction", Rock.SystemGuid.FieldType.DEFINED_VALUE, "", "", "Transaction Project", "", "Designates the Project at the Financial Transaction Level", 0, "", "D15927A2-C163-4286-AC8E-5FD081E753FA", "Project" );
            RockMigrationHelper.AddAttributeQualifier( "D15927A2-C163-4286-AC8E-5FD081E753FA", "allowmultiple", "False", "E537D256-DD39-4324-982D-E9B71C6901F1" );
            RockMigrationHelper.AddAttributeQualifier( "D15927A2-C163-4286-AC8E-5FD081E753FA", "definedtype", "", "A50F2C57-32D8-4B34-8DFC-F2C47EBBFB4E" );

            // transaction detail project
            RockMigrationHelper.AddEntityAttribute( "Rock.Model.FinancialTransactionDetail", Rock.SystemGuid.FieldType.DEFINED_VALUE, "", "", "Transaction Detail Project", "", "Designates the Project at the Financial Transaction Detail Level", 0, "", "C02C812E-AEEE-4D22-A13C-796558CD429D", "Project" );
            RockMigrationHelper.AddAttributeQualifier( "C02C812E-AEEE-4D22-A13C-796558CD429D", "allowmultiple", "False", "C02C812E-AEEE-4D22-A13C-796558CD429D" );
            RockMigrationHelper.AddAttributeQualifier( "C02C812E-AEEE-4D22-A13C-796558CD429D", "definedtype", "", "C02C812E-AEEE-4D22-A13C-796558CD429D" );

            // account default project
            RockMigrationHelper.AddEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.DEFINED_VALUE, "", "", "Default Project", "", "Designates the Project at the Financial Account Level", 0, "", "94E413D7-C6EB-4041-9E86-44269FFB9858", "Project" );
            RockMigrationHelper.AddAttributeQualifier( "94E413D7-C6EB-4041-9E86-44269FFB9858", "allowmultiple", "False", "518C3573-C2CB-4D10-BFD9-32A5E4D98CC5" );
            RockMigrationHelper.AddAttributeQualifier( "94E413D7-C6EB-4041-9E86-44269FFB9858", "definedtype", "", "6D84B878-66EC-462F-BC22-D9F2E41A4CF3" );

            // set defined type qualifers
            Sql( @"
                DECLARE @ProjectDefinedTypeId int = ( SELECT TOP 1 [Id] FROM [DefinedType] WHERE [Guid] = '1C9E0068-6840-4551-86F0-E12691CEC063' )
                
                UPDATE [AttributeQualifier] SET [Value] = CAST( @ProjectDefinedTypeId AS varchar )
                WHERE [Guid] = 'A50F2C57-32D8-4B34-8DFC-F2C47EBBFB4E'

                UPDATE [AttributeQualifier] SET [Value] = CAST( @ProjectDefinedTypeId AS varchar )
                WHERE [Guid] = 'C02C812E-AEEE-4D22-A13C-796558CD429D'

                UPDATE [AttributeQualifier] SET [Value] = CAST( @ProjectDefinedTypeId AS varchar )
                WHERE [Guid] = '6D84B878-66EC-462F-BC22-D9F2E41A4CF3'

                UPDATE [Attribute] SET [IsGridColumn] = 1
                WHERE [Guid] = 'B85AA55C-ED3E-4E67-B022-81A7FC8C1E94'
            " );

            // account gl attributes
            RockMigrationHelper.AddEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Company", "", "", 1, "", "A6FBC708-AD97-4638-95B5-11E49B8277AF", "GeneralLedgerExport_Company" );
            RockMigrationHelper.AddEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Fund", "", "", 2, "", "4F0D43CC-0EA7-4972-9ACA-F0849E892A16", "GeneralLedgerExport_Fund" );
            RockMigrationHelper.AddEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Bank Account", "", "", 3, "", "C4B71AE6-6621-4A6C-9A8D-3F23CEA1EBF5", "GeneralLedgerExport_BankAccount" );
            RockMigrationHelper.AddEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Revenue Department", "", "", 4, "", "1DB4ED40-7F7A-4A2C-A140-93B2909EB437", "GeneralLedgerExport_RevenueDepartment" );
            RockMigrationHelper.AddEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Revenue Account", "", "", 5, "", "FF7119BC-8387-4F76-865A-2CA10C3BE308", "GeneralLedgerExport_RevenueAccount" );

            RockMigrationHelper.UpdateCategory( "5997C8D3-8840-4591-99A5-552919F90CBD", "GL Export", "fa fa-calculator", "", "7D825118-C581-41EA-933E-1FE194E0FC46" ); // batch
            RockMigrationHelper.UpdateCategory( "5997C8D3-8840-4591-99A5-552919F90CBD", "GL Export", "fa fa-calculator", "", "37CF80DC-1F95-4109-9359-30C424B62051" ); // transaction
            RockMigrationHelper.UpdateCategory( "5997C8D3-8840-4591-99A5-552919F90CBD", "GL Export", "fa fa-calculator", "", "E2885276-7F8B-4283-B8A4-933EBB4BD083" ); // transaction detail
            RockMigrationHelper.UpdateCategory( "5997C8D3-8840-4591-99A5-552919F90CBD", "GL Export", "fa fa-calculator", "", "8E7631B3-987E-4374-8697-EDEEC53B1AC4" ); // account

            Sql( @"
                DECLARE @BatchEntityTypeId int = ( SELECT TOP 1 [Id] FROM [EntityType] WHERE [Guid] = 'BDD09C8E-2C52-4D08-9062-BE7D52D190C2' )
                DECLARE @BatchCategoryId int = ( SELECT TOP 1 [Id] FROM [Category] WHERE [Guid] = '7D825118-C581-41EA-933E-1FE194E0FC46' )

                UPDATE [Category]
                SET [EntityTypeQualifierColumn] = 'EntityTypeId',  [EntityTypeQualifierValue] = @BatchEntityTypeId
                WHERE [Id] = @BatchCategoryId
                
                INSERT INTO [AttributeCategory]
                SELECT [Id], @BatchCategoryId
                FROM [Attribute]
                WHERE [Guid] = 'ADFC01F7-B446-479F-9BFE-30E3D68BCB83'


                DECLARE @TransactionEntityTypeId int = ( SELECT TOP 1 [Id] FROM [EntityType] WHERE [Guid] = '2C1CB26B-AB22-42D0-8164-AEDEE0DAE667' )
                DECLARE @TransactionCategoryId int = ( SELECT TOP 1 [Id] FROM [Category] WHERE [Guid] = '37CF80DC-1F95-4109-9359-30C424B62051' )

                UPDATE [Category]
                SET [EntityTypeQualifierColumn] = 'EntityTypeId',  [EntityTypeQualifierValue] = @TransactionEntityTypeId
                WHERE [Id] = @TransactionCategoryId
                
                INSERT INTO [AttributeCategory]
                SELECT [Id], @TransactionCategoryId
                FROM [Attribute]
                WHERE [Guid] = 'D15927A2-C163-4286-AC8E-5FD081E753FA'


                DECLARE @TransactionDetailEntityTypeId int = ( SELECT TOP 1 [Id] FROM [EntityType] WHERE [Guid] = 'AC4AC28B-8E7E-4D7E-85DB-DFFB4F3ADCCE' )
                DECLARE @TransactionDetailCategoryId int = ( SELECT TOP 1 [Id] FROM [Category] WHERE [Guid] = 'E2885276-7F8B-4283-B8A4-933EBB4BD083' )

                UPDATE [Category]
                SET [EntityTypeQualifierColumn] = 'EntityTypeId',  [EntityTypeQualifierValue] = @TransactionDetailEntityTypeId
                WHERE [Id] = @TransactionDetailCategoryId

                INSERT INTO [AttributeCategory]
                SELECT [Id], @TransactionDetailCategoryId
                FROM [Attribute]
                WHERE [Guid] = 'C02C812E-AEEE-4D22-A13C-796558CD429D'

                
                DECLARE @AccountEntityTypeId int = ( SELECT TOP 1 [Id] FROM [EntityType] WHERE [Guid] = '798BCE48-6AA7-4983-9214-F9BCEFB4521D' )
                DECLARE @AccountCategoryId int = ( SELECT TOP 1 [Id] FROM [Category] WHERE [Guid] = '8E7631B3-987E-4374-8697-EDEEC53B1AC4' )

                UPDATE [Category]
                SET [EntityTypeQualifierColumn] = 'EntityTypeId',  [EntityTypeQualifierValue] = @AccountEntityTypeId
                WHERE [Id] = @AccountCategoryId

                INSERT INTO [AttributeCategory]
                SELECT [Id], @AccountCategoryId
                FROM [Attribute]
                WHERE [Guid] = '94E413D7-C6EB-4041-9E86-44269FFB9858'
                   OR [Guid] = 'A6FBC708-AD97-4638-95B5-11E49B8277AF'
                   OR [Guid] = '4F0D43CC-0EA7-4972-9ACA-F0849E892A16'
                   OR [Guid] = 'C4B71AE6-6621-4A6C-9A8D-3F23CEA1EBF5'
                   OR [Guid] = '1DB4ED40-7F7A-4A2C-A140-93B2909EB437'
                   OR [Guid] = 'FF7119BC-8387-4F76-865A-2CA10C3BE308'
            " );
        }

        /// <summary>
        /// The commands to undo a migration from a specific version
        /// </summary>
        public override void Down()
        {
            //
            // remove block, page, and block type
            // do not remove defined type and entity attribute 
            //
            RockMigrationHelper.DeleteBlock( "7ED41FAE-BEE1-4307-AD4F-DEC77D455553" );
            RockMigrationHelper.DeletePage( "408AAC12-B303-4565-AB97-0199F9F69A1B" );
            RockMigrationHelper.DeleteBlockType( "4A55A21B-F174-4012-AD6D-00BA9D76B1CA" );
        }
    }
}
