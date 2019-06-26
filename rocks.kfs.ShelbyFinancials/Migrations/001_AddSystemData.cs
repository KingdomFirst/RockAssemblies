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
using System;
using System.Linq;

using Rock.Data;
using Rock.Model;
using Rock.Plugin;

namespace rocks.kfs.ShelbyFinancials.Migrations
{
    [MigrationNumber( 1, "1.8.7" )]
    public class AddSystemData : Migration
    {
        /// <summary>
        /// The commands to run to migrate plugin to the specific version
        /// </summary>
        public override void Up()
        {
            var migrateNamespace = false;
            var oldNamespace = "com.kfs.ShelbyBatchExport";

            // check if migration has previously run
            using ( var rockContext = new RockContext() )
            {
                var migrationNumber = ( System.Attribute.GetCustomAttribute( this.GetType(), typeof( MigrationNumberAttribute ) ) as MigrationNumberAttribute ).Number;
                migrateNamespace = new PluginMigrationService( rockContext )
                    .Queryable()
                    .Where( m => m.PluginAssemblyName.Equals( oldNamespace, StringComparison.CurrentCultureIgnoreCase ) && m.MigrationNumber == migrationNumber )
                    .Any();
            }

            if ( migrateNamespace )
            {
                //
                // keeping from old 1
                //
                // batch export date
                RockMigrationHelper.EnsureAttributeByGuid( "4B6576DD-82F6-419F-8DF0-467D2636822D", "rocks.kfs.ShelbyFinancials.DateExported", Rock.SystemGuid.EntityType.FINANCIAL_BATCH, Rock.SystemGuid.FieldType.DATE_TIME, "", "" );

                // transaction project
                RockMigrationHelper.EnsureAttributeByGuid( "365134A6-D516-48E0-AC67-A011D5D59D99", "rocks.kfs.ShelbyFinancials.Project", Rock.SystemGuid.EntityType.FINANCIAL_TRANSACTION, Rock.SystemGuid.FieldType.DEFINED_VALUE, "", "" );

                // transaction detail project
                RockMigrationHelper.EnsureAttributeByGuid( "951FAFFD-0513-4E31-9271-87853469E85E", "rocks.kfs.ShelbyFinancials.Project", Rock.SystemGuid.EntityType.FINANCIAL_TRANSACTION_DETAIL, Rock.SystemGuid.FieldType.DEFINED_VALUE, "", "" );

                // account default project
                RockMigrationHelper.EnsureAttributeByGuid( "85422EA2-AC4E-44E5-99B9-30C131116734", "rocks.kfs.ShelbyFinancials.Project", Rock.SystemGuid.EntityType.FINANCIAL_ACCOUNT, Rock.SystemGuid.FieldType.DEFINED_VALUE, "", "" );

                // account gl attributes
                RockMigrationHelper.EnsureAttributeByGuid( "A211D2C9-249D-4D33-B120-A3EAB37C1EDF", "rocks.kfs.ShelbyFinancials.Company", Rock.SystemGuid.EntityType.FINANCIAL_ACCOUNT, Rock.SystemGuid.FieldType.TEXT, "", "" );
                RockMigrationHelper.EnsureAttributeByGuid( "B83D7934-F85A-42B7-AD0E-B4E16D63C189", "rocks.kfs.ShelbyFinancials.Fund", Rock.SystemGuid.EntityType.FINANCIAL_ACCOUNT, Rock.SystemGuid.FieldType.TEXT, "", "" );
                RockMigrationHelper.EnsureAttributeByGuid( "FD4EF8CC-DDB7-4DBD-9FD1-601A0119850B", "rocks.kfs.ShelbyFinancials.DebitAccount", Rock.SystemGuid.EntityType.FINANCIAL_ACCOUNT, Rock.SystemGuid.FieldType.TEXT, "", "" );
                RockMigrationHelper.EnsureAttributeByGuid( "2C1EE0CC-D329-453B-B4F0-29549E24ED05", "rocks.kfs.ShelbyFinancials.Department", Rock.SystemGuid.EntityType.FINANCIAL_ACCOUNT, Rock.SystemGuid.FieldType.TEXT, "", "" );
                RockMigrationHelper.EnsureAttributeByGuid( "0D114FB9-B1AA-4D6D-B0F3-9BB739710992", "rocks.kfs.ShelbyFinancials.CreditAccount", Rock.SystemGuid.EntityType.FINANCIAL_ACCOUNT, Rock.SystemGuid.FieldType.TEXT, "", "" );

                //
                // keeping from old 2
                //
                // create page for project defined type
                RockMigrationHelper.AddPage( true, Rock.SystemGuid.Page.ADMINISTRATION_FINANCE, "D65F783D-87A9-4CC9-8110-E83466A0EADB", "Projects", "", "01DAC445-9C4A-4469-9F39-A39549D75CBF", "fa fa-clipboard", "2B630A3B-E081-4204-A3E4-17BB3A5F063D" );

                // add defined value list block and set to projects defined type
                RockMigrationHelper.AddBlock( true, "01DAC445-9C4A-4469-9F39-A39549D75CBF", "", "0AB2D5E9-9272-47D5-90E4-4AA838D2D3EE", "Projects", "Main", "", "", 0, "23D1B7CE-D70C-4D81-B0D9-534A6D0542DC" );
                RockMigrationHelper.AddBlockAttributeValue( true, "23D1B7CE-D70C-4D81-B0D9-534A6D0542DC", "9280D61F-C4F3-4A3E-A9BB-BCD67FF78637", "2CE68D65-7EAC-4D5E-80B6-6FB903726961" );

                //
                // keeping from old 3
                //
                // nothing

                //
                // keeping from old 4
                //
                // transaction project
                RockMigrationHelper.UpdateAttributeQualifier( "365134A6-D516-48E0-AC67-A011D5D59D99", "displaydescription", "True", "4287FEEF-4AB5-4F16-A872-546A393F2DB8" );

                // transaction detail project
                RockMigrationHelper.UpdateAttributeQualifier( "951FAFFD-0513-4E31-9271-87853469E85E", "displaydescription", "True", "A0679A5F-A76A-4408-9900-C576CC20E18F" );

                // account default project
                RockMigrationHelper.UpdateAttributeQualifier( "85422EA2-AC4E-44E5-99B9-30C131116734", "displaydescription", "True", "DA83FED2-1E06-44A7-8E40-AAECE75169D4" );
                                             
                //
                // keeping from old 5
                //
                // Remove Batch Export Details Block from the Batch Details Page
                RockMigrationHelper.DeleteBlockAttributeValue( "AB2F7C43-74F3-46A4-A005-F690D0A612D2", "DD9CD395-A4F0-4110-A349-C44EDFC0258B" );
                RockMigrationHelper.DeleteBlock( "AB2F7C43-74F3-46A4-A005-F690D0A612D2" );

                // Remove the old export grid
                RockMigrationHelper.DeleteBlock( "E209EFB6-DE18-4DD3-8B15-F7BE418C4955" );

                // Change the Attribute name, and grid display
                Sql( @"
                    DECLARE @DateExportedAttributeId INT = (SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '4B6576DD-82F6-419F-8DF0-467D2636822D')

                    IF @DateExportedAttributeId IS NOT NULL
                    BEGIN
	                    UPDATE [Attribute]
	                    SET [Name] = 'Date Exported', [IsGridColumn] = 1
	                    WHERE [Id] = @DateExportedAttributeId
                    END
                " );

                // rename these attributes
                RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Debit Account", "", 3, "", "FD4EF8CC-DDB7-4DBD-9FD1-601A0119850B", "rocks.kfs.ShelbyFinancials.DebitAccount" );
                RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Revenue Department", "", 4, "", "2C1EE0CC-D329-453B-B4F0-29549E24ED05", "rocks.kfs.ShelbyFinancials.Department" );
                RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Revenue Account", "", 5, "", "0D114FB9-B1AA-4D6D-B0F3-9BB739710992", "rocks.kfs.ShelbyFinancials.CreditAccount" );

                // make sure the key gets updated properly because we're not sure if the old migration ran or not and can't UpdateEntityAttiribute until they're ensured
                RockMigrationHelper.EnsureAttributeByGuid( "9B67459C-3C61-491D-B072-9A9830FBB18F", "rocks.kfs.ShelbyFinancials.Region", Rock.SystemGuid.EntityType.FINANCIAL_ACCOUNT, Rock.SystemGuid.FieldType.TEXT, "", "" );
                RockMigrationHelper.EnsureAttributeByGuid( "8B8D8BBF-B763-4314-87D2-00B9B8BA5A0F", "rocks.kfs.ShelbyFinancials.SuperFund", Rock.SystemGuid.EntityType.FINANCIAL_ACCOUNT, Rock.SystemGuid.FieldType.TEXT, "", "" );
                RockMigrationHelper.EnsureAttributeByGuid( "22699ECA-BB71-4EFD-B416-17B41ED3DBEC", "rocks.kfs.ShelbyFinancials.Location", Rock.SystemGuid.EntityType.FINANCIAL_ACCOUNT, Rock.SystemGuid.FieldType.TEXT, "", "" );
                RockMigrationHelper.EnsureAttributeByGuid( "CD925E61-F87D-461F-9EFA-C1E14397FC4D", "rocks.kfs.ShelbyFinancials.CostCenter", Rock.SystemGuid.EntityType.FINANCIAL_ACCOUNT, Rock.SystemGuid.FieldType.TEXT, "", "" );
                RockMigrationHelper.EnsureAttributeByGuid( "65D935EC-3501-41A6-A2C5-CABC62AB9EF1", "rocks.kfs.ShelbyFinancials.AccountSub", Rock.SystemGuid.EntityType.FINANCIAL_ACCOUNT, Rock.SystemGuid.FieldType.TEXT, "", "" );

                // not all previous installs might have created these attributes.
                RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Region", "", 6, "", "9B67459C-3C61-491D-B072-9A9830FBB18F", "rocks.kfs.ShelbyFinancials.Region" );
                RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Super Fund", "", 7, "", "8B8D8BBF-B763-4314-87D2-00B9B8BA5A0F", "rocks.kfs.ShelbyFinancials.SuperFund" );
                RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Location", "", 8, "", "22699ECA-BB71-4EFD-B416-17B41ED3DBEC", "rocks.kfs.ShelbyFinancials.Location" );
                RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Cost Center", "", 9, "", "CD925E61-F87D-461F-9EFA-C1E14397FC4D", "rocks.kfs.ShelbyFinancials.CostCenter" );
                RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Account Sub", "", 10, "", "65D935EC-3501-41A6-A2C5-CABC62AB9EF1", "rocks.kfs.ShelbyFinancials.AccountSub" );
                                
                // delete old namespace block types from database by guid
                RockMigrationHelper.DeleteBlock( "00FC3A61-775D-4DE5-BC19-A1556FF465EA" );
                RockMigrationHelper.DeleteBlock( "2DC7BB64-EBDB-4973-85A8-E2AF82F76256" );
                RockMigrationHelper.DeleteBlockType( "235C370C-2CD7-4289-8B68-A8617F58B22B" );
                RockMigrationHelper.DeleteBlockType( "B6F19C75-9B23-4EC5-810B-E05A5E11033F" );
                RockMigrationHelper.DeleteAttribute( "CBC91780-4D02-47D5-B4AD-55AE2D5EBB49" );

                // Add new namespace blocks back
                RockMigrationHelper.UpdateBlockType( "Shelby Financials Batch to Journal", "Block used to create Journal Entries in Shelby Financials from a Rock Financial Batch.", "~/Plugins/rocks_kfs/ShelbyFinancials/BatchToJournal.ascx", "KFS > Shelby Financials", "235C370C-2CD7-4289-8B68-A8617F58B22B" );
                RockMigrationHelper.AddBlock( Rock.SystemGuid.Page.FINANCIAL_BATCH_DETAIL, "", "235C370C-2CD7-4289-8B68-A8617F58B22B", "Shelby Financials Batch To Journal", "Main", "", "", 0, "00FC3A61-775D-4DE5-BC19-A1556FF465EA" );

                RockMigrationHelper.UpdateBlockType( "Shelby Financials Batches to Journal", "Block used to create Journal Entries in Shelby Financials from multiple Rock Financial Batches.", "~/Plugins/rocks_kfs/ShelbyFinancials/BatchesToJournal.ascx", "KFS > Shelby Financials", "B6F19C75-9B23-4EC5-810B-E05A5E11033F" );
                RockMigrationHelper.UpdateBlockTypeAttribute( "B6F19C75-9B23-4EC5-810B-E05A5E11033F", Rock.SystemGuid.FieldType.PAGE_REFERENCE, "Detail Page", "DetailPage", "", "", 0, Rock.SystemGuid.Page.FINANCIAL_BATCH_DETAIL, "CBC91780-4D02-47D5-B4AD-55AE2D5EBB49" );
                RockMigrationHelper.AddBlock( true, "3F123421-E5DE-474E-9F56-AAFCEDE115EF", "", "B6F19C75-9B23-4EC5-810B-E05A5E11033F", "Shelby Financials Batches To Journal", "Main", "", "", 0, "2DC7BB64-EBDB-4973-85A8-E2AF82F76256" );
                
                //
                // keeping from old 6
                //
                RockMigrationHelper.UpdateCategory( "5997C8D3-8840-4591-99A5-552919F90CBD", "Shelby Financials Export", "fa fa-calculator", "", "C19D547F-CD02-45C1-9962-FA1DBCEC2897" ); // batch
                RockMigrationHelper.UpdateCategory( "5997C8D3-8840-4591-99A5-552919F90CBD", "Shelby Financials Export", "fa fa-calculator", "", "DD221639-4EFF-4C16-9E7B-BE318E9E9F55" ); // transaction
                RockMigrationHelper.UpdateCategory( "5997C8D3-8840-4591-99A5-552919F90CBD", "Shelby Financials Export", "fa fa-calculator", "", "B097F23D-00D2-4216-916F-DA14335DA9CE" ); // transaction detail
                RockMigrationHelper.UpdateCategory( "5997C8D3-8840-4591-99A5-552919F90CBD", "Shelby Financials Export", "fa fa-calculator", "", "F8893830-B331-4C9F-AA4C-470F0C9B0D18" ); // account

                Sql( @"
                    DECLARE @AccountCategoryId int = ( SELECT TOP 1 [Id] FROM [Category] WHERE [Guid] = 'F8893830-B331-4C9F-AA4C-470F0C9B0D18' )
                    DECLARE @RegionAttributeId int = ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '9B67459C-3C61-491D-B072-9A9830FBB18F' )
                    DECLARE @SuperFundAttributeId int = ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '8B8D8BBF-B763-4314-87D2-00B9B8BA5A0F' )
                    DECLARE @LocationAttributeId int = ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '22699ECA-BB71-4EFD-B416-17B41ED3DBEC' )
                    DECLARE @CostCenterAttributeId int = ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = 'CD925E61-F87D-461F-9EFA-C1E14397FC4D' )
                    DECLARE @AccountSubAttributeId int = ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '65D935EC-3501-41A6-A2C5-CABC62AB9EF1' )

                    IF NOT EXISTS (
                        SELECT *
                        FROM [AttributeCategory]
                        WHERE [AttributeId] = @RegionAttributeId
                        AND [CategoryId] = @AccountCategoryId )
                    BEGIN
                        INSERT INTO [AttributeCategory] ( [AttributeId], [CategoryId] )
                        VALUES( @RegionAttributeId, @AccountCategoryId )
                    END

                    IF NOT EXISTS (
                        SELECT *
                        FROM [AttributeCategory]
                        WHERE [AttributeId] = @SuperFundAttributeId
                        AND [CategoryId] = @AccountCategoryId )
                    BEGIN
                        INSERT INTO [AttributeCategory] ( [AttributeId], [CategoryId] )
                        VALUES( @SuperFundAttributeId, @AccountCategoryId )
                    END

                    IF NOT EXISTS (
                        SELECT *
                        FROM [AttributeCategory]
                        WHERE [AttributeId] = @LocationAttributeId
                        AND [CategoryId] = @AccountCategoryId )
                    BEGIN
                        INSERT INTO [AttributeCategory] ( [AttributeId], [CategoryId] )
                        VALUES( @LocationAttributeId, @AccountCategoryId )
                    END

                    IF NOT EXISTS (
                        SELECT *
                        FROM [AttributeCategory]
                        WHERE [AttributeId] = @CostCenterAttributeId
                        AND [CategoryId] = @AccountCategoryId )
                    BEGIN
                        INSERT INTO [AttributeCategory] ( [AttributeId], [CategoryId] )
                        VALUES( @CostCenterAttributeId, @AccountCategoryId )
                    END

                    IF NOT EXISTS (
                        SELECT *
                        FROM [AttributeCategory]
                        WHERE [AttributeId] = @AccountSubAttributeId
                        AND [CategoryId] = @AccountCategoryId )
                    BEGIN
                        INSERT INTO [AttributeCategory] ( [AttributeId], [CategoryId] )
                        VALUES( @AccountSubAttributeId, @AccountCategoryId )
                    END
                " );

                //
                // keeping from old 7
                //
                RockMigrationHelper.DeleteBlockType( "1F50D804-67A0-4348-8DBB-7C1B46280025" );
                RockMigrationHelper.DeleteBlockType( "6A8AAA9A-67BD-47CA-8FF7-400BDA9FEB2E" );
                RockMigrationHelper.DeleteAttribute( "B72C0356-63AB-4E8B-8F43-40E994B94008" );
            }
            else
            {
                // block and page
                RockMigrationHelper.AddPage( true, Rock.SystemGuid.Page.FUNCTIONS_FINANCE, "D65F783D-87A9-4CC9-8110-E83466A0EADB", "Shelby GL Export", "", "3F123421-E5DE-474E-9F56-AAFCEDE115EF", "fa fa-archive", "EF65EFF2-99AC-4081-8E09-32A04518683A" );

                // project defined type
                RockMigrationHelper.AddDefinedType( "Financial", "Financial Projects", "Used to designate what Project a Transaction should be associated with.", "2CE68D65-7EAC-4D5E-80B6-6FB903726961" );

                // batch export date
                RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialBatch", Rock.SystemGuid.FieldType.DATE_TIME, "", "", "Date Exported", "Date a batch was exported", 0, "", "4B6576DD-82F6-419F-8DF0-467D2636822D", "rocks.kfs.ShelbyFinancials.DateExported" );
                RockMigrationHelper.UpdateAttributeQualifier( "4B6576DD-82F6-419F-8DF0-467D2636822D", "displayDiff", "False", "B76CAB38-B1C8-4D81-B1F5-521A0B507053" );
                RockMigrationHelper.UpdateAttributeQualifier( "4B6576DD-82F6-419F-8DF0-467D2636822D", "format", "", "6AFC0F07-4706-4279-BDA4-204D57A4CC93" );

                // Change the grid display
                Sql( @"
                    DECLARE @DateExportedAttributeId INT = (SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '4B6576DD-82F6-419F-8DF0-467D2636822D')

                    IF @DateExportedAttributeId IS NOT NULL
                    BEGIN
	                    UPDATE [Attribute]
	                    SET [IsGridColumn] = 1
	                    WHERE [Id] = @DateExportedAttributeId
                    END
                " );

                // transaction project
                RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialTransaction", Rock.SystemGuid.FieldType.DEFINED_VALUE, "", "", "Transaction Project", "Designates the Project at the Financial Transaction Level", 0, "", "365134A6-D516-48E0-AC67-A011D5D59D99", "rocks.kfs.ShelbyFinancials.Project" );
                RockMigrationHelper.UpdateAttributeQualifier( "365134A6-D516-48E0-AC67-A011D5D59D99", "allowmultiple", "False", "B2205A7A-E11A-426C-9EF1-34CCD96F5047" );
                RockMigrationHelper.UpdateAttributeQualifier( "365134A6-D516-48E0-AC67-A011D5D59D99", "definedtype", "", "E88DAEFC-BEAE-43CE-8A0E-DF96AFB95FC7" );
                RockMigrationHelper.UpdateAttributeQualifier( "365134A6-D516-48E0-AC67-A011D5D59D99", "displaydescription", "True", "4287FEEF-4AB5-4F16-A872-546A393F2DB8" );

                // transaction detail project
                RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialTransactionDetail", Rock.SystemGuid.FieldType.DEFINED_VALUE, "", "", "Transaction Detail Project", "Designates the Project at the Financial Transaction Detail Level", 0, "", "951FAFFD-0513-4E31-9271-87853469E85E", "rocks.kfs.ShelbyFinancials.Project" );
                RockMigrationHelper.UpdateAttributeQualifier( "951FAFFD-0513-4E31-9271-87853469E85E", "allowmultiple", "False", "BA61B518-C7B7-4F33-8E23-8D2109DA49CB" );
                RockMigrationHelper.UpdateAttributeQualifier( "951FAFFD-0513-4E31-9271-87853469E85E", "definedtype", "", "408068EE-949F-41F5-8CC8-13C2DA6574FB" );
                RockMigrationHelper.UpdateAttributeQualifier( "951FAFFD-0513-4E31-9271-87853469E85E", "displaydescription", "True", "A0679A5F-A76A-4408-9900-C576CC20E18F" );

                // account default project
                RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.DEFINED_VALUE, "", "", "Default Project", "Designates the Project at the Financial Account Level", 0, "", "85422EA2-AC4E-44E5-99B9-30C131116734", "rocks.kfs.ShelbyFinancials.Project" );
                RockMigrationHelper.UpdateAttributeQualifier( "85422EA2-AC4E-44E5-99B9-30C131116734", "allowmultiple", "False", "7B25D6C9-7182-4617-A561-1A52F8140110" );
                RockMigrationHelper.UpdateAttributeQualifier( "85422EA2-AC4E-44E5-99B9-30C131116734", "definedtype", "", "354F659B-06C1-4FEF-A88E-B9C0B1E64C08" );
                RockMigrationHelper.UpdateAttributeQualifier( "85422EA2-AC4E-44E5-99B9-30C131116734", "displaydescription", "True", "DA83FED2-1E06-44A7-8E40-AAECE75169D4" );

                // set defined type qualifers
                Sql( @"
                    DECLARE @ProjectDefinedTypeId int = ( SELECT TOP 1 [Id] FROM [DefinedType] WHERE [Guid] = '2CE68D65-7EAC-4D5E-80B6-6FB903726961' )

                    UPDATE [AttributeQualifier] SET [Value] = CAST( @ProjectDefinedTypeId AS varchar )
                    WHERE [Guid] = 'E88DAEFC-BEAE-43CE-8A0E-DF96AFB95FC7'

                    UPDATE [AttributeQualifier] SET [Value] = CAST( @ProjectDefinedTypeId AS varchar )
                    WHERE [Guid] = '408068EE-949F-41F5-8CC8-13C2DA6574FB'

                    UPDATE [AttributeQualifier] SET [Value] = CAST( @ProjectDefinedTypeId AS varchar )
                    WHERE [Guid] = '354F659B-06C1-4FEF-A88E-B9C0B1E64C08'
                " );

                // account gl attributes
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

                RockMigrationHelper.UpdateCategory( "5997C8D3-8840-4591-99A5-552919F90CBD", "Shelby Financials Export", "fa fa-calculator", "", "C19D547F-CD02-45C1-9962-FA1DBCEC2897" ); // batch
                RockMigrationHelper.UpdateCategory( "5997C8D3-8840-4591-99A5-552919F90CBD", "Shelby Financials Export", "fa fa-calculator", "", "DD221639-4EFF-4C16-9E7B-BE318E9E9F55" ); // transaction
                RockMigrationHelper.UpdateCategory( "5997C8D3-8840-4591-99A5-552919F90CBD", "Shelby Financials Export", "fa fa-calculator", "", "B097F23D-00D2-4216-916F-DA14335DA9CE" ); // transaction detail
                RockMigrationHelper.UpdateCategory( "5997C8D3-8840-4591-99A5-552919F90CBD", "Shelby Financials Export", "fa fa-calculator", "", "F8893830-B331-4C9F-AA4C-470F0C9B0D18" ); // account

                Sql( @"
                    DECLARE @BatchEntityTypeId int = ( SELECT TOP 1 [Id] FROM [EntityType] WHERE [Guid] = 'BDD09C8E-2C52-4D08-9062-BE7D52D190C2' )
                    DECLARE @BatchCategoryId int = ( SELECT TOP 1 [Id] FROM [Category] WHERE [Guid] = 'C19D547F-CD02-45C1-9962-FA1DBCEC2897' )

                    UPDATE [Category]
                    SET [EntityTypeQualifierColumn] = 'EntityTypeId',  [EntityTypeQualifierValue] = @BatchEntityTypeId
                    WHERE [Id] = @BatchCategoryId

                    INSERT INTO [AttributeCategory]
                    SELECT [Id], @BatchCategoryId
                    FROM [Attribute]
                    WHERE [Guid] = '4B6576DD-82F6-419F-8DF0-467D2636822D'

                    DECLARE @TransactionEntityTypeId int = ( SELECT TOP 1 [Id] FROM [EntityType] WHERE [Guid] = '2C1CB26B-AB22-42D0-8164-AEDEE0DAE667' )
                    DECLARE @TransactionCategoryId int = ( SELECT TOP 1 [Id] FROM [Category] WHERE [Guid] = 'DD221639-4EFF-4C16-9E7B-BE318E9E9F55' )

                    UPDATE [Category]
                    SET [EntityTypeQualifierColumn] = 'EntityTypeId',  [EntityTypeQualifierValue] = @TransactionEntityTypeId
                    WHERE [Id] = @TransactionCategoryId

                    INSERT INTO [AttributeCategory]
                    SELECT [Id], @TransactionCategoryId
                    FROM [Attribute]
                    WHERE [Guid] = '365134A6-D516-48E0-AC67-A011D5D59D99'

                    DECLARE @TransactionDetailEntityTypeId int = ( SELECT TOP 1 [Id] FROM [EntityType] WHERE [Guid] = 'AC4AC28B-8E7E-4D7E-85DB-DFFB4F3ADCCE' )
                    DECLARE @TransactionDetailCategoryId int = ( SELECT TOP 1 [Id] FROM [Category] WHERE [Guid] = 'B097F23D-00D2-4216-916F-DA14335DA9CE' )

                    UPDATE [Category]
                    SET [EntityTypeQualifierColumn] = 'EntityTypeId',  [EntityTypeQualifierValue] = @TransactionDetailEntityTypeId
                    WHERE [Id] = @TransactionDetailCategoryId

                    INSERT INTO [AttributeCategory]
                    SELECT [Id], @TransactionDetailCategoryId
                    FROM [Attribute]
                    WHERE [Guid] = '951FAFFD-0513-4E31-9271-87853469E85E'

                    DECLARE @AccountEntityTypeId int = ( SELECT TOP 1 [Id] FROM [EntityType] WHERE [Guid] = '798BCE48-6AA7-4983-9214-F9BCEFB4521D' )
                    DECLARE @AccountCategoryId int = ( SELECT TOP 1 [Id] FROM [Category] WHERE [Guid] = 'F8893830-B331-4C9F-AA4C-470F0C9B0D18' )

                    UPDATE [Category]
                    SET [EntityTypeQualifierColumn] = 'EntityTypeId',  [EntityTypeQualifierValue] = @AccountEntityTypeId
                    WHERE [Id] = @AccountCategoryId

                    INSERT INTO [AttributeCategory]
                    SELECT [Id], @AccountCategoryId
                    FROM [Attribute]
                    WHERE [Guid] = '85422EA2-AC4E-44E5-99B9-30C131116734'
                       OR [Guid] = 'A211D2C9-249D-4D33-B120-A3EAB37C1EDF'
                       OR [Guid] = 'B83D7934-F85A-42B7-AD0E-B4E16D63C189'
                       OR [Guid] = 'FD4EF8CC-DDB7-4DBD-9FD1-601A0119850B'
                       OR [Guid] = '2C1EE0CC-D329-453B-B4F0-29549E24ED05'
                       OR [Guid] = '0D114FB9-B1AA-4D6D-B0F3-9BB739710992'
                       OR [Guid] = '9B67459C-3C61-491D-B072-9A9830FBB18F'
                       OR [Guid] = '8B8D8BBF-B763-4314-87D2-00B9B8BA5A0F'
                       OR [Guid] = '22699ECA-BB71-4EFD-B416-17B41ED3DBEC'
                       OR [Guid] = 'CD925E61-F87D-461F-9EFA-C1E14397FC4D'
                       OR [Guid] = '65D935EC-3501-41A6-A2C5-CABC62AB9EF1'
                " );

                // create page for project defined type
                RockMigrationHelper.AddPage( true, Rock.SystemGuid.Page.ADMINISTRATION_FINANCE, "D65F783D-87A9-4CC9-8110-E83466A0EADB", "Projects", "", "01DAC445-9C4A-4469-9F39-A39549D75CBF", "fa fa-clipboard", "2B630A3B-E081-4204-A3E4-17BB3A5F063D" );

                // add defined value list block and set to projects defined type
                RockMigrationHelper.AddBlock( true, "01DAC445-9C4A-4469-9F39-A39549D75CBF", "", "0AB2D5E9-9272-47D5-90E4-4AA838D2D3EE", "Projects", "Main", "", "", 0, "23D1B7CE-D70C-4D81-B0D9-534A6D0542DC" );
                RockMigrationHelper.AddBlockAttributeValue( true, "23D1B7CE-D70C-4D81-B0D9-534A6D0542DC", "9280D61F-C4F3-4A3E-A9BB-BCD67FF78637", "2CE68D65-7EAC-4D5E-80B6-6FB903726961" );

                // add new blocks
                RockMigrationHelper.UpdateBlockType( "Shelby Financials Batch to Journal", "Block used to create Journal Entries in Shelby Financials from a Rock Financial Batch.", "~/Plugins/rocks_kfs/ShelbyFinancials/BatchToJournal.ascx", "KFS > Shelby Financials", "235C370C-2CD7-4289-8B68-A8617F58B22B" );
                RockMigrationHelper.AddBlock(true, Rock.SystemGuid.Page.FINANCIAL_BATCH_DETAIL, "", "235C370C-2CD7-4289-8B68-A8617F58B22B", "Shelby Financial Batches To Journal", "Main", "", "", 0, "00FC3A61-775D-4DE5-BC19-A1556FF465EA" );

                // block type
                RockMigrationHelper.UpdateBlockType( "Shelby Financials Batches to Journal", "Block used to create Journal Entries in Shelby Financials from multiple Rock Financial Batches.", "~/Plugins/rocks_kfs/ShelbyFinancials/BatchesToJournal.ascx", "KFS > Shelby Financials", "B6F19C75-9B23-4EC5-810B-E05A5E11033F" );
                RockMigrationHelper.AddBlockTypeAttribute( "B6F19C75-9B23-4EC5-810B-E05A5E11033F", Rock.SystemGuid.FieldType.PAGE_REFERENCE, "Detail Page", "DetailPage", "", "", 0, Rock.SystemGuid.Page.FINANCIAL_BATCH_DETAIL, "CBC91780-4D02-47D5-B4AD-55AE2D5EBB49", true );

                // block on the Shelby batch export page
                RockMigrationHelper.AddBlock( "3F123421-E5DE-474E-9F56-AAFCEDE115EF", "", "B6F19C75-9B23-4EC5-810B-E05A5E11033F", "Shelby Financials Batches To Journal", "Main", "", "", 0, "2DC7BB64-EBDB-4973-85A8-E2AF82F76256" );
            }
        }

        /// <summary>
        /// The commands to undo a migration from a specific version
        /// </summary>
        public override void Down()
        {
            //
            // remove shelby financials blocks, page, and block types
            //
            RockMigrationHelper.DeleteBlock( "00FC3A61-775D-4DE5-BC19-A1556FF465EA" );
            RockMigrationHelper.DeleteBlock( "2DC7BB64-EBDB-4973-85A8-E2AF82F76256" );
            RockMigrationHelper.DeletePage( "3F123421-E5DE-474E-9F56-AAFCEDE115EF" );
            RockMigrationHelper.DeleteBlockType( "235C370C-2CD7-4289-8B68-A8617F58B22B" );
            RockMigrationHelper.DeleteBlockType( "B6F19C75-9B23-4EC5-810B-E05A5E11033F" );

            //
            // remove project block and page
            //
            RockMigrationHelper.DeleteBlockAttributeValue( "23D1B7CE-D70C-4D81-B0D9-534A6D0542DC", "9280D61F-C4F3-4A3E-A9BB-BCD67FF78637" );
            RockMigrationHelper.DeleteBlock( "23D1B7CE-D70C-4D81-B0D9-534A6D0542DC" );
            RockMigrationHelper.DeletePage( "01DAC445-9C4A-4469-9F39-A39549D75CBF" );


            //
            // remove account attributes
            //
            RockMigrationHelper.DeleteAttribute( "A211D2C9-249D-4D33-B120-A3EAB37C1EDF" );
            RockMigrationHelper.DeleteAttribute( "B83D7934-F85A-42B7-AD0E-B4E16D63C189" );
            RockMigrationHelper.DeleteAttribute( "FD4EF8CC-DDB7-4DBD-9FD1-601A0119850B" );
            RockMigrationHelper.DeleteAttribute( "2C1EE0CC-D329-453B-B4F0-29549E24ED05" );
            RockMigrationHelper.DeleteAttribute( "0D114FB9-B1AA-4D6D-B0F3-9BB739710992" );
            RockMigrationHelper.DeleteAttribute( "9B67459C-3C61-491D-B072-9A9830FBB18F" );
            RockMigrationHelper.DeleteAttribute( "8B8D8BBF-B763-4314-87D2-00B9B8BA5A0F" );
            RockMigrationHelper.DeleteAttribute( "22699ECA-BB71-4EFD-B416-17B41ED3DBEC" );
            RockMigrationHelper.DeleteAttribute( "CD925E61-F87D-461F-9EFA-C1E14397FC4D" );
            RockMigrationHelper.DeleteAttribute( "65D935EC-3501-41A6-A2C5-CABC62AB9EF1" );
        }
    }
}
