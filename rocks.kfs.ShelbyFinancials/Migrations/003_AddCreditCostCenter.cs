// <copyright>
// Copyright 2023 by Kingdom First Solutions
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
    [MigrationNumber( 3, "1.13.0" )]
    public class AddCreditCostCenter : Migration
    {
        /// <summary>
        /// The commands to run to migrate plugin to the specific version
        /// </summary>
        public override void Up()
        {
            // Relabel Cost Center attribute
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Cost Center Default/Debit", "Cost Center Default will be used on both Credit/Debit lines if Cost Center Credit does not contain a value, to preserve existing functionality.", 11, "", "CD925E61-F87D-461F-9EFA-C1E14397FC4D", "rocks.kfs.ShelbyFinancials.CostCenter" );

            // New Cost Center Credit attribute
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Cost Center Credit", "", 12, "", "94D96FEB-73AD-4A07-9F03-257A988994D4", "rocks.kfs.ShelbyFinancials.CostCenterCredit" );

            // set attribute category for new attribute

            Sql( @"
                    DECLARE @AccountCategoryId int = ( SELECT TOP 1 [Id] FROM [Category] WHERE [Guid] = 'F8893830-B331-4C9F-AA4C-470F0C9B0D18' )
                    DECLARE @CostCenterCreditAttributeId int = ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '94D96FEB-73AD-4A07-9F03-257A988994D4' )

                    IF NOT EXISTS (
                        SELECT *
                        FROM [AttributeCategory]
                        WHERE [AttributeId] = @CostCenterCreditAttributeId
                        AND [CategoryId] = @AccountCategoryId )
                    BEGIN
                        INSERT INTO [AttributeCategory] ( [AttributeId], [CategoryId] )
                        VALUES( @CostCenterCreditAttributeId, @AccountCategoryId )
                    END
                " );

        }

        /// <summary>
        /// The commands to undo a migration from a specific version
        /// </summary>
        public override void Down()
        {
            RockMigrationHelper.DeleteAttribute( "94D96FEB-73AD-4A07-9F03-257A988994D4" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.TEXT, "", "", "Cost Center", "", 11, "", "CD925E61-F87D-461F-9EFA-C1E14397FC4D", "rocks.kfs.ShelbyFinancials.CostCenter" );
        }
    }
}
