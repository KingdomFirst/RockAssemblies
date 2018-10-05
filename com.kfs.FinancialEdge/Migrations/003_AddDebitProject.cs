using Rock.Plugin;

namespace com.kfs.FinancialEdge.Migrations
{
    [MigrationNumber( 3, "1.7.4" )]
    public class AddDebitProject : Migration
    {
        /// <summary>
        /// The commands to run to migrate plugin to the specific version
        /// </summary>
        public override void Up()
        {
            // account project
            RockMigrationHelper.AddEntityAttribute( "Rock.Model.FinancialAccount", Rock.SystemGuid.FieldType.DEFINED_VALUE, "", "", "Debit Project", "", "Designates the Project for the Debit Account", 3, "", "E717E7DF-FDC9-41C4-AC5E-3693E11B55DE", "com.kfs.FinancialEdge.DEBITPROJECTID" );
            RockMigrationHelper.AddAttributeQualifier( "E717E7DF-FDC9-41C4-AC5E-3693E11B55DE", "allowmultiple", "False", "FA5C55AA-5824-4FFE-8011-5CC23AF62680" );
            RockMigrationHelper.AddAttributeQualifier( "E717E7DF-FDC9-41C4-AC5E-3693E11B55DE", "definedtype", "", "30CE0CF6-A330-4D5E-8C46-30A1A1578325" );
            RockMigrationHelper.AddAttributeQualifier( "E717E7DF-FDC9-41C4-AC5E-3693E11B55DE", "displaydescription", "True", "E4376AA7-8BAB-4698-83BB-BE944AF52332" );
            RockMigrationHelper.AddAttributeQualifier( "E717E7DF-FDC9-41C4-AC5E-3693E11B55DE", "enhancedselection", "True", "4766585C-563E-4669-98F8-0E806D55C436" );

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

                INSERT INTO [AttributeCategory]
                SELECT [Id], @AccountCategoryId
                FROM [Attribute]
                WHERE [Guid] = 'E717E7DF-FDC9-41C4-AC5E-3693E11B55DE'
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