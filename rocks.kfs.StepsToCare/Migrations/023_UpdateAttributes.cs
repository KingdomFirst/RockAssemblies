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
using System;
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

            var assignToGroupDesc = "If groups are selected for this touch template, \"Minimum Care Touches\" number of people will be assigned in a round-robin fashion for any new Care Need in this category. If multiple groups are chosen, that same number of members from each group will be assigned. ";

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

            RockMigrationHelper.AddDefinedTypeAttribute( SystemGuid.DefinedType.CARE_NEED_CATEGORY, Rock.SystemGuid.FieldType.MULTI_SELECT, "Assign to Group(s)", "AssignToGroups", "If groups are selected, one member from each group will be assigned in a round-robin fashion for any new Care Need in this category. ", 3, "", SystemGuid.Attribute.CATEGORY_ATTRIBUTE_ASSIGNTOGROUPS );
            RockMigrationHelper.AddAttributeQualifier( SystemGuid.Attribute.CATEGORY_ATTRIBUTE_ASSIGNTOGROUPS, "enhancedselection", "True", "C299C97E-B3EF-45F4-AA12-924FC9BCADA9" );
            RockMigrationHelper.AddAttributeQualifier( SystemGuid.Attribute.CATEGORY_ATTRIBUTE_ASSIGNTOGROUPS, "repeatColumns", "", "18F97937-8A3C-4099-9F02-491B1ACD684B" );
            RockMigrationHelper.AddAttributeQualifier( SystemGuid.Attribute.CATEGORY_ATTRIBUTE_ASSIGNTOGROUPS, "repeatDirection", "0", "0B1F2E6A-E9E6-479A-8D77-667CD17E1315" );
            RockMigrationHelper.AddAttributeQualifier( SystemGuid.Attribute.CATEGORY_ATTRIBUTE_ASSIGNTOGROUPS, "values", groupSelectSql, "B544C383-CCBA-4D80-B86E-F843F4AF0453" );

            RockMigrationHelper.AddOrUpdateBlockTypeAttribute( "AF14CB6C-F915-4449-9CB7-7C44B624B051", "A75DFC58-7A1B-4799-BF31-451B2BBE38FF", "Minimum Follow Up Care Touch Hours", "MinimumFollowUpTouchHours", "Minimum Follow Up Care Touch Hours", @"Minimum hours for the follow up worker to add a care touch before the need gets 'flagged'.", 4, @"24", "3D5545C1-29AF-4DF4-8C5C-8330651F4FEE" );
            RockMigrationHelper.AddBlockAttributeValue( "3D5545C1-29AF-4DF4-8C5C-8330651F4FEE", "8945BE62-D065-4A19-89A8-B06CE51FFBFF", @"24" );

            var attributeGuid = "A9B39207-F075-4208-B97E-43C8773F706D";
            var attributeValueGuid = "5CC0532F-55A0-4329-A560-ECC6417F49BB";
            RockMigrationHelper.AddOrUpdateEntityAttribute( "Rock.Model.ServiceJob", "A75DFC58-7A1B-4799-BF31-451B2BBE38FF", "Class", "rocks.kfs.StepsToCare.Jobs.CareNeedAutomatedNotifications", "Minimum Follow Up Care Touch Hours", "Minimum Follow Up Care Touch Hours", @"Minimum hours for the follow up worker to add a care touch before the Care Touch Needed notification gets sent out.", 0, @"24", attributeGuid, "MinimumFollowUpTouchHours" );

            // copied and modified from RockMigrationHelper.AddAttributeValue method to use the value from another attribute.
            Sql( $@"
                DECLARE @AttributeId INT = (SELECT [Id] FROM [Attribute] WHERE [Guid] = '{attributeGuid}')
                DECLARE @AttributeValueGuid UNIQUEIDENTIFIER = NEWID()
                DECLARE @JobId int = ( SELECT TOP 1 [Id] FROM [ServiceJob] WHERE [Guid] = '895C301C-02D1-4D9C-9FC4-DA7257368208' )
                DECLARE @DefaultValue NVARCHAR(MAX) = ( SELECT TOP 1 [Value] FROM [AttributeValue] av JOIN [Attribute] a ON av.AttributeId = a.Id WHERE a.[Guid] = 'AF3ECA88-C715-4E2D-9D9B-C4CC3141C6EC' AND EntityId = @JobId)

                -- A GUID was provided, try to use it if it is available
                IF NOT EXISTS(SELECT * FROM [AttributeValue] WHERE [Guid] = '{attributeValueGuid}')
                BEGIN
	                SET @AttributeValueGuid = '{attributeValueGuid}'
                END

                -- Now check if the attribute/entity pair already has a row and insert it if not
                IF NOT EXISTS(SELECT [Id] FROM [dbo].[AttributeValue] WHERE [AttributeId] = @AttributeId AND [EntityId] = @JobId)
                BEGIN
                    INSERT INTO [AttributeValue] (
                          [IsSystem]
		                , [AttributeId]
		                , [EntityId]
		                , [Value]
		                , [Guid])
                    VALUES(
                          1
		                , @AttributeId
		                , @JobId
		                , @DefaultValue
		                , @AttributeValueGuid)
                END" );
        }

        public override void Down()
        {
            RockMigrationHelper.DeleteAttribute( "A9B39207-F075-4208-B97E-43C8773F706D" );

            RockMigrationHelper.DeleteAttribute( "3D5545C1-29AF-4DF4-8C5C-8330651F4FEE" );

            RockMigrationHelper.DeleteAttribute( SystemGuid.Attribute.CATEGORY_ATTRIBUTE_ASSIGNTOGROUPS );

            RockMigrationHelper.DeleteAttribute( SystemGuid.Attribute.MATRIX_ATTRIBUTE_ASSIGNTOGROUPS );
        }
    }
}