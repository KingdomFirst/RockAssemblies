// <copyright>
// Copyright 2024 by Kingdom First Solutions
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

namespace rocks.kfs.StepsToCare.Migrations
{
    [MigrationNumber( 23, "1.16.0" )]
    public class UpdateAttributes : Migration
    {
        public override void Up()
        {

            var rockContext = new RockContext();
            var attributeMatrixTemplateId = new AttributeMatrixTemplateService( rockContext ).Get( SystemGuid.Attribute.CATEGORY_MATRIX_CARE_TOUCHES.AsGuid() ).Id.ToString();

            var assignToGroupDesc = "If groups are selected for this touch template, an \"X\" number of people will be assigned in a round-robin fashion for any Care Need added under this category. If multiple groups are chosen, someone will be assigned to each of them in the same fashion. (X will match the number of minimum care touches).";

            var groupSelectSql = @"SELECT 
        CASE WHEN ggpg.Name IS NOT NULL THEN
	        CONCAT(ggpg.name, ' > ',gpg.Name,' > ',pg.Name,' > ', g.Name)
        WHEN gpg.Name IS NOT NULL THEN
	        CONCAT(gpg.Name,' > ',pg.Name,' > ', g.Name)
        WHEN pg.Name IS NOT NULL THEN
	        CONCAT(pg.Name,' > ', g.Name)
        ELSE
	        g.Name 
        END as Text, g.Id as Value
        FROM [Group] g
            LEFT JOIN [Group] pg ON g.ParentGroupId = pg.Id
            LEFT JOIN [Group] gpg ON pg.ParentGroupId = gpg.Id
            LEFT JOIN [Group] ggpg ON gpg.ParentGroupId = ggpg.Id
        WHERE g.GroupTypeId NOT IN (1,10,11,12,19,20,21,22) 
        ORDER BY 
            CASE WHEN ggpg.Name IS NOT NULL THEN
	            CONCAT(ggpg.name, ' > ',gpg.Name,' > ',pg.Name,' > ', g.Name)
            WHEN gpg.Name IS NOT NULL THEN
	            CONCAT(gpg.Name,' > ',pg.Name,' > ', g.Name)
            WHEN pg.Name IS NOT NULL THEN
	            CONCAT(pg.Name,' > ', g.Name)
            ELSE
                g.Name 
        END";

            RockMigrationHelper.AddOrUpdateEntityAttribute( "Rock.Model.AttributeMatrixItem", Rock.SystemGuid.FieldType.MULTI_SELECT, "AttributeMatrixTemplateId", attributeMatrixTemplateId, "Assign to Group(s)", "Assign to Group(s)", assignToGroupDesc, 5, "", SystemGuid.Attribute.MATRIX_ATTRIBUTE_ASSIGNTOGROUPS, "AssignToGroups" );
            RockMigrationHelper.AddAttributeQualifier( SystemGuid.Attribute.MATRIX_ATTRIBUTE_ASSIGNTOGROUPS, "enhancedselection", "True", "C09BDE05-0635-4726-8A4F-5882C4705462" );
            RockMigrationHelper.AddAttributeQualifier( SystemGuid.Attribute.MATRIX_ATTRIBUTE_ASSIGNTOGROUPS, "repeatColumns", "", "D05D6809-4438-4381-892F-090E17B04FD3" );
            RockMigrationHelper.AddAttributeQualifier( SystemGuid.Attribute.MATRIX_ATTRIBUTE_ASSIGNTOGROUPS, "repeatDirection", "0", "F1A6D5C2-4021-4BF6-ABC4-CE5A4B04F691" );
            RockMigrationHelper.AddAttributeQualifier( SystemGuid.Attribute.MATRIX_ATTRIBUTE_ASSIGNTOGROUPS, "values", groupSelectSql, "73149EF2-195E-4E8F-87EA-044A0C4E3F80" );

            RockMigrationHelper.AddDefinedTypeAttribute( SystemGuid.DefinedType.CARE_NEED_CATEGORY, Rock.SystemGuid.FieldType.MULTI_SELECT, "Assign to Group(s)", "AssignToGroups", assignToGroupDesc, 3, "", SystemGuid.Attribute.CATEGORY_ATTRIBUTE_ASSIGNTOGROUPS );
            RockMigrationHelper.AddAttributeQualifier( SystemGuid.Attribute.CATEGORY_ATTRIBUTE_ASSIGNTOGROUPS, "enhancedselection", "True", "C299C97E-B3EF-45F4-AA12-924FC9BCADA9" );
            RockMigrationHelper.AddAttributeQualifier( SystemGuid.Attribute.CATEGORY_ATTRIBUTE_ASSIGNTOGROUPS, "repeatColumns", "", "18F97937-8A3C-4099-9F02-491B1ACD684B" );
            RockMigrationHelper.AddAttributeQualifier( SystemGuid.Attribute.CATEGORY_ATTRIBUTE_ASSIGNTOGROUPS, "repeatDirection", "0", "0B1F2E6A-E9E6-479A-8D77-667CD17E1315" );
            RockMigrationHelper.AddAttributeQualifier( SystemGuid.Attribute.CATEGORY_ATTRIBUTE_ASSIGNTOGROUPS, "values", groupSelectSql, "B544C383-CCBA-4D80-B86E-F843F4AF0453" );
        }

        public override void Down()
        {
            RockMigrationHelper.DeleteAttribute( SystemGuid.Attribute.CATEGORY_ATTRIBUTE_ASSIGNTOGROUPS );

            RockMigrationHelper.DeleteAttribute( SystemGuid.Attribute.MATRIX_ATTRIBUTE_ASSIGNTOGROUPS );
        }
    }
}