using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rock.Plugin;
using Rock.SystemGuid;

namespace com.kfs.MinistrySafe.Migrations
{
    [MigrationNumber( 8, "1.6.9" )]
    public class ChangeTestResultPersonAttribute : Migration
    {
        /// <summary>
        /// The commands to run to migrate plugin to the specific version
        /// </summary>
        public override void Up()
        {
            Sql( @"
DECLARE @MSResultId INT = ( SELECT [Id] FROM [Attribute] WHERE [Guid] = 'DB9D8876-4550-424D-A019-1AA71F8ADC1D' )
DECLARE @SingleSelectFieldType INT = ( SELECT [Id] FROM [FieldType] WHERE [Guid] = '7525C4CB-EE6B-41D4-9B64-A08048D5A5C0' )

UPDATE AttributeValue
SET [Value] = 'Pass'
WHERE [AttributeId] = @MSResultId
AND [Value] like 'True'

UPDATE AttributeValue
SET [Value] = 'Fail'
WHERE [AttributeId] = @MSResultId
AND [Value] like 'False'

DELETE FROM [AttributeQualifier]
WHERE [Guid] = '9FCEE8FD-12C2-4602-AB32-9AC9590F2156'

DELETE FROM [AttributeQualifier]
WHERE [Guid] = '6F4FA3B6-8958-4DDA-AC5B-73DB8500E0C4'

UPDATE Attribute
SET FieldTypeId = @SingleSelectFieldType
WHERE [Id] = @MSResultId
" );

            RockMigrationHelper.UpdateAttributeQualifier( SystemGuid.Attribute.MINISTRY_SAFE_PERSON_ATTRIBUTE_TEST_RESULT, "fieldtype", "ddl", "6325EF63-CC10-4173-A69B-21BD22A8E75B" );
            RockMigrationHelper.UpdateAttributeQualifier( SystemGuid.Attribute.MINISTRY_SAFE_PERSON_ATTRIBUTE_TEST_RESULT, "values", "Pass,Fail,Pending", "62F5D81B-BF3F-4349-858F-9158DE58F268" );
        }

        /// <summary>
        /// The commands to undo a migration from a specific version
        /// </summary>
        public override void Down()
        {
        }
    }
}
