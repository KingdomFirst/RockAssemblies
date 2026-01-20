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
    [MigrationNumber( 19, "1.14.0" )]
    public class AddFieldTypeAndAttributeMatrix : Migration
    {
        public override void Up()
        {
            RockMigrationHelper.UpdateFieldType( "Steps To Care - Note Template", "", "rocks.kfs.StepsToCare", "rocks.kfs.StepsToCare.Field.Types.NoteTemplateFieldType", SystemGuid.FieldType.NOTE_TEMPLATE );
            RockMigrationHelper.UpdateFieldType( "Steps To Care - Care Need", "", "rocks.kfs.StepsToCare", "rocks.kfs.StepsToCare.Field.Types.CareNeedFieldType", SystemGuid.FieldType.CARE_NEED );

            // create the attribute matrix template and get it's id to assign to the attributes we'll create
            var attributeMatrixTemplate = new AttributeMatrixTemplate
            {
                Name = "Care Touch Templates",
                Description = "This attribute matrix is used by Steps to Care to assign care touch templates to care need categories.",
                IsActive = true,
                FormattedLava = AttributeMatrixTemplate.FormattedLavaDefault,
                CreatedDateTime = RockDateTime.Now,
                Guid = SystemGuid.Category.MATRIX_CARE_TOUCHES.AsGuid()
            };

            var rockContext = new RockContext();
            var attributeMatrixTemplateService = new AttributeMatrixTemplateService( rockContext );
            rockContext.WrapTransaction( () =>
            {
                attributeMatrixTemplateService.Add( attributeMatrixTemplate );
                rockContext.SaveChanges();
            } );

            var attributeMatrixTemplateId = attributeMatrixTemplateService.Get( SystemGuid.Category.MATRIX_CARE_TOUCHES.AsGuid() ).Id.ToString();

            // Entity: Rock.Model.AttributeMatrixItem Attribute: Note Template
            RockMigrationHelper.AddOrUpdateEntityAttribute( "Rock.Model.AttributeMatrixItem", SystemGuid.FieldType.NOTE_TEMPLATE, "AttributeMatrixTemplateId", attributeMatrixTemplateId, "Note Template", "Note Template", @"", 0, @"", SystemGuid.Attribute.MATRIX_ATTRIBUTE_TOUCH_NOTE_TEMPLATE, "NoteTemplate" );
            // Entity: Rock.Model.AttributeMatrixItem Attribute: Minimum Care Touches
            RockMigrationHelper.AddOrUpdateEntityAttribute( "Rock.Model.AttributeMatrixItem", Rock.SystemGuid.FieldType.INTEGER, "AttributeMatrixTemplateId", attributeMatrixTemplateId, "Minimum Care Touches", "Minimum Care Touches", @"", 1, @"", SystemGuid.Attribute.MATRIX_ATTRIBUTE_TOUCH_MINIMUM_TOUCHES, "MinimumCareTouches" );
            // Entity: Rock.Model.AttributeMatrixItem Attribute: Minimum Care Touch Hours
            RockMigrationHelper.AddOrUpdateEntityAttribute( "Rock.Model.AttributeMatrixItem", Rock.SystemGuid.FieldType.INTEGER, "AttributeMatrixTemplateId", attributeMatrixTemplateId, "Minimum Care Touch Hours", "Minimum Care Touch Hours", @"", 2, @"", SystemGuid.Attribute.MATRIX_ATTRIBUTE_TOUCH_MINIMUM_HOURS, "MinimumCareTouchHours" );
            // Entity: Rock.Model.AttributeMatrixItem Attribute: Notify All Assigned
            RockMigrationHelper.AddOrUpdateEntityAttribute( "Rock.Model.AttributeMatrixItem", Rock.SystemGuid.FieldType.BOOLEAN, "AttributeMatrixTemplateId", attributeMatrixTemplateId, "Notify All Assigned", "Notify All Assigned", @"By default, only the ""follow up worker"" will get notified. If you choose this option anyone assigned to the need will receive this notification.", 3, @"False", SystemGuid.Attribute.MATRIX_ATTRIBUTE_TOUCH_NOTIFY_ALL, "NotifyAllAssigned" );
            RockMigrationHelper.UpdateAttributeQualifier( SystemGuid.Attribute.MATRIX_ATTRIBUTE_TOUCH_NOTIFY_ALL, "truetext", @"Yes", "FA07644B-73DD-4F0B-8525-9B2A406A98B3" );
            RockMigrationHelper.UpdateAttributeQualifier( SystemGuid.Attribute.MATRIX_ATTRIBUTE_TOUCH_NOTIFY_ALL, "falsetext", @"No", "A2B118DB-5773-44CB-B2A7-3DF6C9F589F8" );
            RockMigrationHelper.UpdateAttributeQualifier( SystemGuid.Attribute.MATRIX_ATTRIBUTE_TOUCH_NOTIFY_ALL, "BooleanControlType", @"1", "BB6C24F8-118C-469C-A63D-ECB56946012B" );
            // Entity: Rock.Model.AttributeMatrixItem Attribute: Recurring
            RockMigrationHelper.AddOrUpdateEntityAttribute( "Rock.Model.AttributeMatrixItem", Rock.SystemGuid.FieldType.BOOLEAN, "AttributeMatrixTemplateId", attributeMatrixTemplateId, "Recurring", "Recurring", @"Is this care touch requirement recurring? Each recurrence of this notification will happen in the set amount of time after the last ""touch"" using the set note template.", 4, @"False", SystemGuid.Attribute.MATRIX_ATTRIBUTE_TOUCH_RECURRING, "Recurring" );
            RockMigrationHelper.UpdateAttributeQualifier( SystemGuid.Attribute.MATRIX_ATTRIBUTE_TOUCH_RECURRING, "truetext", @"Yes", "1FB1E224-9A79-4E14-9F3C-B7E782FAF68D" );
            RockMigrationHelper.UpdateAttributeQualifier( SystemGuid.Attribute.MATRIX_ATTRIBUTE_TOUCH_RECURRING, "falsetext", @"No", "B1A68314-7619-4580-923A-BA22DF022ABE" );
            RockMigrationHelper.UpdateAttributeQualifier( SystemGuid.Attribute.MATRIX_ATTRIBUTE_TOUCH_RECURRING, "BooleanControlType", @"1", "FD768D79-629F-40F7-A5D7-EB246A8D7DB9" );

            // set all attributes to be required
            Sql( string.Format( @"
                UPDATE [Attribute] SET [IsRequired] = 1
                WHERE (
                    [Guid] = '{0}'
                    OR
                    [Guid] = '{1}'
                    OR
                    [Guid] = '{2}'
                )
                ", SystemGuid.Attribute.MATRIX_ATTRIBUTE_TOUCH_NOTE_TEMPLATE, SystemGuid.Attribute.MATRIX_ATTRIBUTE_TOUCH_MINIMUM_TOUCHES, SystemGuid.Attribute.MATRIX_ATTRIBUTE_TOUCH_MINIMUM_HOURS ) );

            RockMigrationHelper.AddDefinedTypeAttribute( SystemGuid.DefinedType.CARE_NEED_CATEGORY, "F16FC460-DC1E-4821-9012-5F21F974C677", "Care Touch Templates", "CareTouchTemplates", "", 6565, "", "50128254-2C44-421D-A53E-F67236B67A2F" );
            RockMigrationHelper.AddAttributeQualifier( "50128254-2C44-421D-A53E-F67236B67A2F", "attributematrixtemplate", attributeMatrixTemplateId, "AE2E6015-5281-4367-A5C3-DC278644B038" );
        }

        public override void Down()
        {
            RockMigrationHelper.DeleteAttribute( "50128254-2C44-421D-A53E-F67236B67A2F" );

            RockMigrationHelper.DeleteAttribute( SystemGuid.Attribute.MATRIX_ATTRIBUTE_TOUCH_NOTE_TEMPLATE ); // Rock.Model.AttributeMatrixItem: Note Template
            RockMigrationHelper.DeleteAttribute( SystemGuid.Attribute.MATRIX_ATTRIBUTE_TOUCH_MINIMUM_TOUCHES ); // Rock.Model.AttributeMatrixItem: Minimum Care Touches
            RockMigrationHelper.DeleteAttribute( SystemGuid.Attribute.MATRIX_ATTRIBUTE_TOUCH_MINIMUM_HOURS ); // Rock.Model.AttributeMatrixItem: Minimum Care Touch Hours
            RockMigrationHelper.DeleteAttribute( SystemGuid.Attribute.MATRIX_ATTRIBUTE_TOUCH_NOTIFY_ALL ); // Rock.Model.AttributeMatrixItem: Notify All Assigned
            RockMigrationHelper.DeleteAttribute( SystemGuid.Attribute.MATRIX_ATTRIBUTE_TOUCH_RECURRING ); // Rock.Model.AttributeMatrixItem: Recurring

            RockMigrationHelper.DeleteFieldType( SystemGuid.FieldType.NOTE_TEMPLATE );
            RockMigrationHelper.DeleteFieldType( SystemGuid.FieldType.CARE_NEED );

            var rockContext = new RockContext();
            var attributeMatrixTemplateService = new AttributeMatrixTemplateService( rockContext );
            var attributeMatrixTemplate = attributeMatrixTemplateService.Get( SystemGuid.Category.MATRIX_CARE_TOUCHES.AsGuid() );
            rockContext.WrapTransaction( () =>
            {
                attributeMatrixTemplateService.Delete( attributeMatrixTemplate );
                rockContext.SaveChanges();
            } );
        }
    }
}