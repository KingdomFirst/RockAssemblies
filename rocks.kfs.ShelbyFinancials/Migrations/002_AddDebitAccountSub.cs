// <copyright>
// Copyright 2022 by Kingdom First Solutions
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
    [MigrationNumber( 2, "1.8.7" )]
    public class AddDebitAccountSub : Migration
    {
        /// <summary>
        /// The commands to run to migrate plugin to the specific version
        /// </summary>
        public override void Up()
        {
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Debit Account Sub", "The Sub Account number for the Debit Account", 4, "", "4F1BA61A-31C3-4FD6-9758-E3FE17A2D746", "rocks.kfs.ShelbyFinancials.DebitAccountSub" );

            // update name and order of Account Sub attribute 
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Revenue Account Sub", "The Sub Account number for the Revenue Account", 7, "", "65D935EC-3501-41A6-A2C5-CABC62AB9EF1", "rocks.kfs.ShelbyFinancials.AccountSub" );

            // reorder attributes
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Revenue Department", "", 5, "", "2C1EE0CC-D329-453B-B4F0-29549E24ED05", "rocks.kfs.ShelbyFinancials.Department" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Revenue Account", "", 6, "", "0D114FB9-B1AA-4D6D-B0F3-9BB739710992", "rocks.kfs.ShelbyFinancials.CreditAccount" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Region", "", 8, "", "9B67459C-3C61-491D-B072-9A9830FBB18F", "rocks.kfs.ShelbyFinancials.Region" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Super Fund", "", 9, "", "8B8D8BBF-B763-4314-87D2-00B9B8BA5A0F", "rocks.kfs.ShelbyFinancials.SuperFund" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Location", "", 10, "", "22699ECA-BB71-4EFD-B416-17B41ED3DBEC", "rocks.kfs.ShelbyFinancials.Location" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Cost Center", "", 11, "", "CD925E61-F87D-461F-9EFA-C1E14397FC4D", "rocks.kfs.ShelbyFinancials.CostCenter" );

            // set attribute category for new attribute

            Sql( @"
                    DECLARE @AccountCategoryId int = ( SELECT TOP 1 [Id] FROM [Category] WHERE [Guid] = 'F8893830-B331-4C9F-AA4C-470F0C9B0D18' )
                    DECLARE @DebitAccountSubAttributeId int = ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '4F1BA61A-31C3-4FD6-9758-E3FE17A2D746' )

                    IF NOT EXISTS (
                        SELECT *
                        FROM [AttributeCategory]
                        WHERE [AttributeId] = @DebitAccountSubAttributeId
                        AND [CategoryId] = @AccountCategoryId )
                    BEGIN
                        INSERT INTO [AttributeCategory] ( [AttributeId], [CategoryId] )
                        VALUES( @DebitAccountSubAttributeId, @AccountCategoryId )
                    END
                " );

            // copy values already provided in AccountSub into DebitAccountSub as well to keep previous functionality in tact
            Sql( @"
                DECLARE @RevAccSubAttrId int = ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '65D935EC-3501-41A6-A2C5-CABC62AB9EF1' )
                DECLARE @DebAccSubAttrId int = ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '4F1BA61A-31C3-4FD6-9758-E3FE17A2D746' )

                INSERT INTO [AttributeValue]
                   ([IsSystem]
                   ,[AttributeId]
                   ,[EntityId]
                   ,[Value]
                   ,[Guid]
                   ,[CreatedDateTime]
                   ,[ModifiedDateTime]
                   ,[CreatedByPersonAliasId]
                   ,[ModifiedByPersonAliasId]
                   ,[ValueAsNumeric])

                SELECT AV.[IsSystem], 
                    @DebAccSubAttrId, 
                    AV.[EntityId], 
                    AV.[Value],
                    NEWID(), 
                    AV.[CreatedDateTime], 
                    AV.[ModifiedDateTime], 
                    AV.[CreatedByPersonAliasId],
                    AV.[ModifiedByPersonAliasId],
                    AV.[ValueAsNumeric]
                FROM AttributeValue AV WITH(NOLOCK)
                LEFT JOIN AttributeValue AV2 WITH(NOLOCK) ON AV.EntityId = AV2.EntityId AND AV2.AttributeId = @DebAccSubAttrId 
                WHERE AV.AttributeId = @RevAccSubAttrId AND AV2.Id IS NULL
            " );
        }

        /// <summary>
        /// The commands to undo a migration from a specific version
        /// </summary>
        public override void Down()
        {
            // remove Debit Account Sub attribute
            RockMigrationHelper.DeleteAttribute( "4F1BA61A-31C3-4FD6-9758-E3FE17A2D746" );

            // update name and order of Account Sub attribute 
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Account Sub", "", 10, "", "65D935EC-3501-41A6-A2C5-CABC62AB9EF1", "rocks.kfs.ShelbyFinancials.AccountSub" );

            // reorder attributes
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Revenue Department", "", 4, "", "2C1EE0CC-D329-453B-B4F0-29549E24ED05", "rocks.kfs.ShelbyFinancials.Department" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Revenue Account", "", 5, "", "0D114FB9-B1AA-4D6D-B0F3-9BB739710992", "rocks.kfs.ShelbyFinancials.CreditAccount" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Region", "", 6, "", "9B67459C-3C61-491D-B072-9A9830FBB18F", "rocks.kfs.ShelbyFinancials.Region" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Super Fund", "", 7, "", "8B8D8BBF-B763-4314-87D2-00B9B8BA5A0F", "rocks.kfs.ShelbyFinancials.SuperFund" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Location", "", 8, "", "22699ECA-BB71-4EFD-B416-17B41ED3DBEC", "rocks.kfs.ShelbyFinancials.Location" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Cost Center", "", 9, "", "CD925E61-F87D-461F-9EFA-C1E14397FC4D", "rocks.kfs.ShelbyFinancials.CostCenter" );
        }
    }
}
