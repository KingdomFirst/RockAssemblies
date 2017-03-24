using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rock.Plugin;

namespace com.kfs.ShelbyBatchExport.Migrations
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
            RockMigrationHelper.AddBlockType( "Shelby GL Export", "Lists all financial batches and provides Shelby GL Export capability", "~/Plugins/com_kfs/Finance/ShelbyGLExport.ascx", "KFS > Finance", "1F50D804-67A0-4348-8DBB-7C1B46280025" );
            RockMigrationHelper.AddPage( Rock.SystemGuid.Page.FUNCTIONS_FINANCE, "D65F783D-87A9-4CC9-8110-E83466A0EADB", "Shelby GL Export", "", "3F123421-E5DE-474E-9F56-AAFCEDE115EF", "fa fa-archive", "EF65EFF2-99AC-4081-8E09-32A04518683A" );
            RockMigrationHelper.AddBlock( "3F123421-E5DE-474E-9F56-AAFCEDE115EF", "", "1F50D804-67A0-4348-8DBB-7C1B46280025", "Shelby GL Export", "Main", "", "", 0, "E209EFB6-DE18-4DD3-8B15-F7BE418C4955" );

            // project defined type
            RockMigrationHelper.AddDefinedType( "Financial", "Financial Projects", "Used to designate what Project a Transaction should be associated with.", "2CE68D65-7EAC-4D5E-80B6-6FB903726961" );
            RockMigrationHelper.AddDefinedTypeAttribute( "2CE68D65-7EAC-4D5E-80B6-6FB903726961", Rock.SystemGuid.FieldType.TEXT, "GL Code", "Code", "", 0, "", "B72C0356-63AB-4E8B-8F43-40E994B94008" );

            // batch export date
            RockMigrationHelper.AddEntityAttribute( "Rock.Model.FinancialBatch", Rock.SystemGuid.FieldType.DATE_TIME, "", "", "Batch Exported", "", "Date a batch was exported", 0, "", "4B6576DD-82F6-419F-8DF0-467D2636822D", "GLExport_BatchExported" );
            RockMigrationHelper.AddAttributeQualifier( "4B6576DD-82F6-419F-8DF0-467D2636822D", "displayDiff", "False", "B76CAB38-B1C8-4D81-B1F5-521A0B507053" );
            RockMigrationHelper.AddAttributeQualifier( "4B6576DD-82F6-419F-8DF0-467D2636822D", "format", "", "6AFC0F07-4706-4279-BDA4-204D57A4CC93" );

            // transaction project
            RockMigrationHelper.AddEntityAttribute( "Rock.Model.FinancialTransaction", Rock.SystemGuid.FieldType.DEFINED_VALUE, "", "", "Transaction Project", "", "Designates the Project at the Transaction Level", 0, "", "365134A6-D516-48E0-AC67-A011D5D59D99", "Project" );
            RockMigrationHelper.AddAttributeQualifier( "365134A6-D516-48E0-AC67-A011D5D59D99", "allowmultiple", "False", "B2205A7A-E11A-426C-9EF1-34CCD96F5047" );
            RockMigrationHelper.AddAttributeQualifier( "365134A6-D516-48E0-AC67-A011D5D59D99", "definedtype", "", "E88DAEFC-BEAE-43CE-8A0E-DF96AFB95FC7" );

            // transaction detail project
            RockMigrationHelper.AddEntityAttribute( "Rock.Model.FinancialTransactionDetail", Rock.SystemGuid.FieldType.DEFINED_VALUE, "", "", "Transaction Detail Project", "", "Designates the Project at the Transaction Detail Level", 0, "", "951FAFFD-0513-4E31-9271-87853469E85E", "Project" );
            RockMigrationHelper.AddAttributeQualifier( "951FAFFD-0513-4E31-9271-87853469E85E", "allowmultiple", "False", "BA61B518-C7B7-4F33-8E23-8D2109DA49CB" );
            RockMigrationHelper.AddAttributeQualifier( "951FAFFD-0513-4E31-9271-87853469E85E", "definedtype", "", "408068EE-949F-41F5-8CC8-13C2DA6574FB" );

            // account default project
            RockMigrationHelper.AddEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.DEFINED_VALUE, "", "", "Default Project", "", "Designates the Project at the Transaction Detail Level", 0, "", "85422EA2-AC4E-44E5-99B9-30C131116734", "Project" );
            RockMigrationHelper.AddAttributeQualifier( "85422EA2-AC4E-44E5-99B9-30C131116734", "allowmultiple", "False", "7B25D6C9-7182-4617-A561-1A52F8140110" );
            RockMigrationHelper.AddAttributeQualifier( "85422EA2-AC4E-44E5-99B9-30C131116734", "definedtype", "", "354F659B-06C1-4FEF-A88E-B9C0B1E64C08" );

            // set defined type qualifers
            Sql( @"
                DECLARE @ProjectDefinedTypeId int = ( SELECT TOP 1 [Id] FROM [DefinedType] WHERE [Guid] = '2CE68D65-7EAC-4D5E-80B6-6FB903726961' )
                
                UPDATE [AttributeQualifier] SET [Value] = CAST( @ProjectDefinedTypeId AS varchar )
                WHERE [Guid] = 'E88DAEFC-BEAE-43CE-8A0E-DF96AFB95FC7'

                UPDATE [AttributeQualifier] SET [Value] = CAST( @ProjectDefinedTypeId AS varchar )
                WHERE [Guid] = '408068EE-949F-41F5-8CC8-13C2DA6574FB'

                UPDATE [AttributeQualifier] SET [Value] = CAST( @ProjectDefinedTypeId AS varchar )
                WHERE [Guid] = '354F659B-06C1-4FEF-A88E-B9C0B1E64C08'

                UPDATE [Attribute] SET [IsGridColumn] = 1
                WHERE [Guid] = 'B72C0356-63AB-4E8B-8F43-40E994B94008'
            " );

            // account gl attributes
            RockMigrationHelper.AddEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Company", "", "", 1, "", "A211D2C9-249D-4D33-B120-A3EAB37C1EDF", "GeneralLedgerExport_Company" );
            RockMigrationHelper.AddEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Fund", "", "", 2, "", "B83D7934-F85A-42B7-AD0E-B4E16D63C189", "GeneralLedgerExport_Fund" );
            RockMigrationHelper.AddEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Bank Account", "", "", 3, "", "FD4EF8CC-DDB7-4DBD-9FD1-601A0119850B", "GeneralLedgerExport_BankAccount" );
            RockMigrationHelper.AddEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Revenue Department", "", "", 4, "", "2C1EE0CC-D329-453B-B4F0-29549E24ED05", "GeneralLedgerExport_RevenueDepartment" );
            RockMigrationHelper.AddEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Revenue Account", "", "", 5, "", "0D114FB9-B1AA-4D6D-B0F3-9BB739710992", "GeneralLedgerExport_RevenueAccount" );
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
            RockMigrationHelper.DeleteBlock( "E209EFB6-DE18-4DD3-8B15-F7BE418C4955" );
            RockMigrationHelper.DeletePage( "3F123421-E5DE-474E-9F56-AAFCEDE115EF" );
            RockMigrationHelper.DeleteBlockType( "1F50D804-67A0-4348-8DBB-7C1B46280025" );
        }
    }
}
