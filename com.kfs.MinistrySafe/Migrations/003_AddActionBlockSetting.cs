using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rock.Plugin;

namespace com.kfs.MinistrySafe.Migrations
{
    [MigrationNumber( 3, "1.6.9" )]
    class AddActionBlockSetting : Migration
    {
        /// <summary>
        /// The commands to run to migrate plugin to the specific version
        /// </summary>
        public override void Up()
        {
            // Add Action to person bio block
            RockMigrationHelper.AddBlockAttributeValue( "B5C1FDB6-0224-43E4-8E26-6B2EAF86253A", "7197A0FB-B330-43C4-8E62-F3C14F649813", "AE617426-E1E3-4E06-B3D8-B7AB1A7B4892", appendToExisting: true );
        }

        /// <summary>
        /// The commands to undo a migration from a specific version
        /// </summary>
        public override void Down()
        {
            Sql( @"
    DECLARE @BlockId int
    SET @BlockId = (SELECT [Id] FROM [Block] WHERE [Guid] = 'B5C1FDB6-0224-43E4-8E26-6B2EAF86253A')
    DECLARE @AttributeId int
    SET @AttributeId = (SELECT [Id] FROM [Attribute] WHERE [Guid] = '7197A0FB-B330-43C4-8E62-F3C14F649813')

    DECLARE @OldValue NVARCHAR(MAX) = ''
    DECLARE @NewValue NVARCHAR(MAX) = ''

    IF EXISTS (SELECT 1 FROM [AttributeValue] WHERE [AttributeId] = @AttributeId AND [EntityId] = @BlockId )
    BEGIN
        SET @OldValue = (SELECT [Value] FROM [AttributeValue] WHERE [AttributeId] = @AttributeId AND [EntityId] = @BlockId )

        IF CHARINDEX( 'AE617426-E1E3-4E06-B3D8-B7AB1A7B4892', @OldValue ) != 0
        BEGIN
            SET @NewValue = REPLACE(@OldValue, ',AE617426-E1E3-4E06-B3D8-B7AB1A7B4892', '') -- if not first item
            SET @NewValue = REPLACE(@NewValue, 'AE617426-E1E3-4E06-B3D8-B7AB1A7B4892,', '') -- if first item
            SET @NewValue = REPLACE(@NewValue, 'AE617426-E1E3-4E06-B3D8-B7AB1A7B4892', '') -- if only item

	       UPDATE [AttributeValue]
	       SET [Value] = @NewValue
	       WHERE [AttributeId] = @AttributeId AND [EntityId] = @BlockId
        END
    END
" );
        }
    }
}
