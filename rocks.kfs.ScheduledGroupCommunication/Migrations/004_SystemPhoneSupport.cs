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
using System;
using System.Linq;
using Rock;
using Rock.Data;
using Rock.Model;
using Rock.Plugin;
using KFSConst = rocks.kfs.ScheduledGroupCommunication.SystemGuid;

namespace rocks.kfs.ScheduledGroupCommunication.Migrations
{
    [MigrationNumber( 4, "1.15.0" )]
    internal class SystemPhoneSupport : Migration
    {
        /// <summary>
        /// The commands to run to migrate plugin to the specific version
        /// </summary>
        public override void Up()
        {
            var rockContextSMS = new RockContext();

            var attributeMatrixTemplateSMSId = new AttributeMatrixTemplateService( rockContextSMS ).Get( KFSConst.Attribute.ATTRIBUTE_MATRIX_TEMPLATE_SCHEDULED_SMS.AsGuid() ).Id.ToString();

            // Add new from number attribute to existing matrix
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.AttributeMatrixItem", Rock.SystemGuid.FieldType.SYSTEM_PHONE_NUMBER, "AttributeMatrixTemplateId", attributeMatrixTemplateSMSId, "From Number", "", 1, "", KFSConst.Attribute.MATRIX_ATTRIBUTE_SMS_FROM_NUMBER_SYSTEM_PHONE, "rocks.kfs.ScheduledGroupCommunication.SMSFromNumberSystemPhone" );
            RockMigrationHelper.AddAttributeQualifier( KFSConst.Attribute.MATRIX_ATTRIBUTE_SMS_FROM_NUMBER_SYSTEM_PHONE, "allowMultiple", "False", "E519CA4D-2E4E-4265-9DCD-2010647748AF" );
            RockMigrationHelper.AddAttributeQualifier( KFSConst.Attribute.MATRIX_ATTRIBUTE_SMS_FROM_NUMBER_SYSTEM_PHONE, "includeInactive", "False", "03B877A1-4822-4210-B739-8A83B747BE49" );
            RockMigrationHelper.AddAttributeQualifier( KFSConst.Attribute.MATRIX_ATTRIBUTE_SMS_FROM_NUMBER_SYSTEM_PHONE, "repeatColumns", "", "CA8E4252-E94C-4631-9705-8F1E33B8E215" );

            // set new attribute to be required
            Sql( string.Format( @"
            UPDATE [Attribute] SET [IsRequired] = 1
            WHERE [Guid] = '{0}'", KFSConst.Attribute.MATRIX_ATTRIBUTE_SMS_FROM_NUMBER_SYSTEM_PHONE ) );

            // Migrate existing attribute values to new attribute
            Sql( string.Format( @"
            DECLARE @OldSMSFromNumberAttributeId int = ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '{0}' )
            DECLARE @NewSMSFromNumberAttributeId int = ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '{1}' )
            UPDATE [AttributeValue] SET [AttributeId] = @NewSMSFromNumberAttributeId
            WHERE [AttributeId] = @OldSMSFromNumberAttributeId", KFSConst.Attribute.MATRIX_ATTRIBUTE_SMS_FROM_NUMBER, KFSConst.Attribute.MATRIX_ATTRIBUTE_SMS_FROM_NUMBER_SYSTEM_PHONE ) );

            // Remove old From Number attribute
            RockMigrationHelper.DeleteAttribute( KFSConst.Attribute.MATRIX_ATTRIBUTE_SMS_FROM_NUMBER );
        }

        /// <summary>
        /// The commands to undo a migration from a specific version
        /// </summary>
        public override void Down()
        {
            // Add back original From Number
            var rockContextSMS = new RockContext();
            var attributeMatrixTemplateSMSId = new AttributeMatrixTemplateService( rockContextSMS ).Get( KFSConst.Attribute.ATTRIBUTE_MATRIX_TEMPLATE_SCHEDULED_SMS.AsGuid() ).Id.ToString();

            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.AttributeMatrixItem", Rock.SystemGuid.FieldType.DEFINED_VALUE, "AttributeMatrixTemplateId", attributeMatrixTemplateSMSId, "From Number", "", 1, "", KFSConst.Attribute.MATRIX_ATTRIBUTE_SMS_FROM_NUMBER, "rocks.kfs.ScheduledGroupCommunication.SMSFromNumber" );
            RockMigrationHelper.AddAttributeQualifier( KFSConst.Attribute.MATRIX_ATTRIBUTE_SMS_FROM_NUMBER, "allowmultiple", "False", "90051033-C881-42E8-A0CE-4152527D6FA3" );
            RockMigrationHelper.AddAttributeQualifier( KFSConst.Attribute.MATRIX_ATTRIBUTE_SMS_FROM_NUMBER, "definedtype", "", "26143779-5747-416C-98B9-EBC86E4B8EE6" );
            RockMigrationHelper.AddAttributeQualifier( KFSConst.Attribute.MATRIX_ATTRIBUTE_SMS_FROM_NUMBER, "displaydescription", "True", "8AABC104-CFE5-4DF1-A164-2DA305F46BBC" );
            RockMigrationHelper.AddAttributeQualifier( KFSConst.Attribute.MATRIX_ATTRIBUTE_SMS_FROM_NUMBER, "enhancedselection", "False", "B0008343-0033-4194-A1A8-F5AEF3B095AD" );
            Sql( @"
            DECLARE @CommunicationSMSFromDefinedTypeId int = ( SELECT TOP 1 [Id] FROM [DefinedType] WHERE [Guid] = '611BDE1F-7405-4D16-8626-CCFEDB0E62BE' )
            UPDATE [AttributeQualifier] SET [Value] = CAST( @CommunicationSMSFromDefinedTypeId AS varchar )
            WHERE [Guid] = '26143779-5747-416C-98B9-EBC86E4B8EE6'" );

            // Reverse attribute value migration
            Sql( string.Format( @"
            DECLARE @OldSMSFromNumberAttributeId int = ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '{0}' )
            DECLARE @NewSMSFromNumberAttributeId int = ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '{1}' )
            UPDATE [AttributeValue] SET [AttributeId] = @OldSMSFromNumberAttributeId
            WHERE [AttributeId] = @NewSMSFromNumberAttributeId", KFSConst.Attribute.MATRIX_ATTRIBUTE_SMS_FROM_NUMBER, KFSConst.Attribute.MATRIX_ATTRIBUTE_SMS_FROM_NUMBER_SYSTEM_PHONE ) );

            // Delete attribute
            RockMigrationHelper.DeleteAttribute( KFSConst.Attribute.MATRIX_ATTRIBUTE_SMS_FROM_NUMBER_SYSTEM_PHONE );
        }
    }
}
