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

namespace rocks.kfs.ManagedMissions.Migrations
{
    [MigrationNumber( 1, "1.8.0" )]
    public class CreateManagedMissionsTransactionAttributes : Migration
    {
        /// <summary>
        /// The commands to run to migrate plugin to the specific version
        /// </summary>
        public override void Up()
        {
            // mm url fields
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialTransaction", Rock.SystemGuid.FieldType.TEXT, "", "", "Person First Name", "", 100, "", "3DF6068C-AFD2-4D59-8FCE-9BE9D4930E0D", "kfs_mm_firstname" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialTransaction", Rock.SystemGuid.FieldType.TEXT, "", "", "Person Last Name", "", 101, "", "D9A20523-2979-41E3-B6E2-C4C75EC079FE", "kfs_mm_lastname" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialTransaction", Rock.SystemGuid.FieldType.TEXT, "", "", "Person Import/Export Key", "", 102, "", "3E99FD9F-87DB-429D-9DDA-09ACC73C2911", "kfs_mm_personkey" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialTransaction", Rock.SystemGuid.FieldType.TEXT, "", "", "Trip Name", "", 103, "", "C500B2C3-79A0-4EBC-9E6A-0B356A66B970", "kfs_mm_tripname" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialTransaction", Rock.SystemGuid.FieldType.TEXT, "", "", "Trip Purpose Code", "", 104, "", "3025ACDB-40D1-4A52-AF0A-547D7126D90D", "kfs_mm_purposecode" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialTransaction", Rock.SystemGuid.FieldType.TEXT, "", "", "Trip Import/Exort Key", "", 105, "", "6A5F63FA-23D6-4BA4-84A1-3D10951646DB", "kfs_mm_tripkey" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialTransaction", Rock.SystemGuid.FieldType.TEXT, "", "", "Income Account Number", "", 106, "", "A30C07EB-AE07-4AF4-9831-B32F9B073B31", "kfs_mm_incomeaccount" );

            // sync date
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialTransaction", Rock.SystemGuid.FieldType.DATE_TIME, "", "", "Managed Missions Sync Date", "Date this Transaction was synced", 107, "", "BFC71CD0-2A4C-4491-847E-A5DCB1A1E876", "kfs_mm_ManagedMissionsSyncDate" );
            RockMigrationHelper.AddAttributeQualifier( "BFC71CD0-2A4C-4491-847E-A5DCB1A1E876", "displayDiff", "False", "7257C567-E916-4439-8A3E-BAEBC5BE82CA" );
            RockMigrationHelper.AddAttributeQualifier( "BFC71CD0-2A4C-4491-847E-A5DCB1A1E876", "format", "", "E490A27C-A995-44D7-BC79-5772AAD59F91" );

            RockMigrationHelper.UpdateCategory( "5997C8D3-8840-4591-99A5-552919F90CBD", "Managed Missions Transactions", "fa fa-plane", "", "0A7C03CF-8204-4F25-9156-30A91045CC52" );

            Sql( @"
                DECLARE @TransactionEntityTypeId int = ( SELECT TOP 1 [Id] FROM [EntityType] WHERE [Guid] = '2C1CB26B-AB22-42D0-8164-AEDEE0DAE667' )
                DECLARE @CategoryId int = ( SELECT TOP 1 [Id] FROM [Category] WHERE [Guid] = '0A7C03CF-8204-4F25-9156-30A91045CC52' )

                UPDATE [Category]
                SET [EntityTypeQualifierColumn] = 'EntityTypeId',  [EntityTypeQualifierValue] = @TransactionEntityTypeId
                WHERE [Id] = @CategoryId

                INSERT INTO [AttributeCategory]
                SELECT [Id], @CategoryId
                FROM [Attribute]
                WHERE [Guid] = '3DF6068C-AFD2-4D59-8FCE-9BE9D4930E0D'
                   OR [Guid] = 'D9A20523-2979-41E3-B6E2-C4C75EC079FE'
                   OR [Guid] = '3E99FD9F-87DB-429D-9DDA-09ACC73C2911'
                   OR [Guid] = 'C500B2C3-79A0-4EBC-9E6A-0B356A66B970'
                   OR [Guid] = '3025ACDB-40D1-4A52-AF0A-547D7126D90D'
                   OR [Guid] = '6A5F63FA-23D6-4BA4-84A1-3D10951646DB'
                   OR [Guid] = 'A30C07EB-AE07-4AF4-9831-B32F9B073B31'
                   OR [Guid] = 'BFC71CD0-2A4C-4491-847E-A5DCB1A1E876'
            " );
        }

        /// <summary>
        /// The commands to undo a migration from a specific version
        /// </summary>
        public override void Down()
        {
            //
            // Nothing to delete
            //
        }
    }
}
