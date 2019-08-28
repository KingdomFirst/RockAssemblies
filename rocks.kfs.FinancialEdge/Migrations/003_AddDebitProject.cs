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
    [MigrationNumber( 3, "1.7.4" )]
    public class AddDebitProject : Migration
    {
        /// <summary>
        /// The commands to run to migrate plugin to the specific version
        /// </summary>
        public override void Up()
        {
            // reset the account project attribute key
            Sql( @"
                UPDATE [Attribute]
                SET [Key] = 'DebitProject'
                WHERE [Guid] = 'E717E7DF-FDC9-41C4-AC5E-3693E11B55DE'
            " );

            // account project
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.DEFINED_VALUE, "", "", "Debit Project", "Designates the Project for the Debit Account", 3, "", "E717E7DF-FDC9-41C4-AC5E-3693E11B55DE" );
            RockMigrationHelper.UpdateAttributeQualifier( "E717E7DF-FDC9-41C4-AC5E-3693E11B55DE", "allowmultiple", "False", "FA5C55AA-5824-4FFE-8011-5CC23AF62680" );
            RockMigrationHelper.UpdateAttributeQualifier( "E717E7DF-FDC9-41C4-AC5E-3693E11B55DE", "definedtype", "", "30CE0CF6-A330-4D5E-8C46-30A1A1578325" );
            RockMigrationHelper.UpdateAttributeQualifier( "E717E7DF-FDC9-41C4-AC5E-3693E11B55DE", "displaydescription", "True", "E4376AA7-8BAB-4698-83BB-BE944AF52332" );
            RockMigrationHelper.UpdateAttributeQualifier( "E717E7DF-FDC9-41C4-AC5E-3693E11B55DE", "enhancedselection", "True", "4766585C-563E-4669-98F8-0E806D55C436" );

            // set the account project attribute key
            Sql( @"
                UPDATE [Attribute]
                SET [Key] = 'rocks.kfs.FinancialEdge.DEBITPROJECTID'
                WHERE [Guid] = 'E717E7DF-FDC9-41C4-AC5E-3693E11B55DE'
            " );

            // set defined type qualifers
            Sql( @"
                DECLARE @ProjectDefinedTypeId int = ( SELECT TOP 1 [Id] FROM [DefinedType] WHERE [Guid] = '35178732-932A-472F-8D39-526B8E45625E' )

                UPDATE [AttributeQualifier] SET [Value] = CAST( @ProjectDefinedTypeId AS varchar )
                WHERE [Guid] = '30CE0CF6-A330-4D5E-8C46-30A1A1578325'
            " );

            Sql( @"
                --
                -- Set FinancialAccount attributes to category
                --
                DECLARE @AccountEntityTypeId int = ( SELECT TOP 1 [Id] FROM [EntityType] WHERE [Guid] = '798BCE48-6AA7-4983-9214-F9BCEFB4521D' )
                DECLARE @AccountCategoryId int = ( SELECT TOP 1 [Id] FROM [Category] WHERE [Guid] = 'FC9B74D6-ABB7-4D01-836C-C811859F0F62' )

                UPDATE [Category]
                SET [EntityTypeQualifierColumn] = 'EntityTypeId',  [EntityTypeQualifierValue] = @AccountEntityTypeId
                WHERE [Id] = @AccountCategoryId

                DECLARE @AccountDefaultProject int = ( SELECT [Id] FROM [Attribute] WHERE [Guid] = 'E717E7DF-FDC9-41C4-AC5E-3693E11B55DE' )

				IF NOT EXISTS ( SELECT [AttributeId], [CategoryId] FROM [AttributeCategory] WHERE [AttributeId] = @AccountDefaultProject AND [CategoryId] = @AccountCategoryId )

                BEGIN
                    INSERT INTO [AttributeCategory]
                    SELECT [Id], @AccountCategoryId
                    FROM [Attribute]
                    WHERE [Guid] = 'E717E7DF-FDC9-41C4-AC5E-3693E11B55DE'
                END
            " );
        }

        /// <summary>
        /// The commands to undo a migration from a specific version
        /// </summary>
        public override void Down()
        {
        }
    }
}
