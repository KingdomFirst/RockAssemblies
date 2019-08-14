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
using Rock;
using Rock.Data;
using Rock.Model;
using Rock.Plugin;

namespace rocks.kfs.PersonAudience.Migrations
{
    [MigrationNumber( 1, "1.8.0" )]
    internal class AddSystemData : Migration
    {
        /// <summary>
        /// The commands to run to migrate plugin to the specific version
        /// </summary>
        public override void Up()
        {
            var definedTypeId = new DefinedTypeService( new RockContext() ).GetByGuid( Rock.SystemGuid.DefinedType.MARKETING_CAMPAIGN_AUDIENCE_TYPE.AsGuid() ).Id;

            // Person Attribute
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.Person", Rock.SystemGuid.FieldType.DEFINED_VALUE, "", "", "Audiences", "The target audiences for this person.", 0, "", "67CD39C1-4AEA-4ED1-AB57-608AD6F7DB8B" );
            RockMigrationHelper.UpdateAttributeQualifier( "67CD39C1-4AEA-4ED1-AB57-608AD6F7DB8B", "allowmultiple", "True", "DFBA5F28-774B-45AB-9952-B8BAAB7940D9" );
            RockMigrationHelper.UpdateAttributeQualifier( "67CD39C1-4AEA-4ED1-AB57-608AD6F7DB8B", "definedtype", definedTypeId.ToString(), "041202B3-C6DB-4061-A2C7-FFC31C9637AD" );

            // Defined Value Attribute
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.DefinedValue", "BD72BBF1-0269-407E-BDBE-EEED4F1F207F", "DefinedTypeId", definedTypeId.ToString(), "Audience Data View", "The person data view that is associated to this audience.", 0, "", "3E856C6D-0FB4-472F-BCB5-E83D1E45249B" );
            RockMigrationHelper.UpdateAttributeQualifier( "3E856C6D-0FB4-472F-BCB5-E83D1E45249B", "entityTypeName", "Rock.Model.Person", "B1C75C98-322B-41A6-A5E7-F106EF22F06B" );

            Sql( @"
                    DECLARE @AudienceDataViewAttributeId INT = (SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '3E856C6D-0FB4-472F-BCB5-E83D1E45249B')

                    IF @AudienceDataViewAttributeId IS NOT NULL
                    BEGIN
	                    UPDATE [Attribute]
	                    SET [IsGridColumn] = 1
	                    WHERE [Id] = @AudienceDataViewAttributeId
                    END
                " );
        }

        /// <summary>
        /// The commands to undo a migration from a specific version
        /// </summary>
        public override void Down()
        {
            RockMigrationHelper.DeleteAttribute( "67CD39C1-4AEA-4ED1-AB57-608AD6F7DB8B" );
            RockMigrationHelper.DeleteAttribute( "3E856C6D-0FB4-472F-BCB5-E83D1E45249B" );
        }
    }
}
