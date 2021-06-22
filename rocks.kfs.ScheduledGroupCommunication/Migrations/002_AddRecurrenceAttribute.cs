// <copyright>
// Copyright 2021 by Kingdom First Solutions
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
    [MigrationNumber( 2, "1.7.4" )]
    internal class AddRecurrenceAttribute : Migration
    {
        /// <summary>
        /// The commands to run to migrate plugin to the specific version
        /// </summary>
        public override void Up()
        {
            #region Email Attribute

            var attributeMatrixTemplateEmailId = new AttributeMatrixTemplateService( new RockContext() ).Get( KFSConst.Attribute.ATTRIBUTE_MATRIX_TEMPLATE_SCHEDULED_EMAILS.AsGuid() ).Id.ToString();

            // recurrence attribute
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.AttributeMatrixItem", Rock.SystemGuid.FieldType.SINGLE_SELECT, "AttributeMatrixTemplateId", attributeMatrixTemplateEmailId, "Recurrence", "", 1, "0", KFSConst.Attribute.MATRIX_ATTRIBUTE_EMAIL_SEND_RECURRENCE, "rocks.kfs.ScheduledGroupCommunication.EmailSendRecurrence" );
            RockMigrationHelper.AddAttributeQualifier( KFSConst.Attribute.MATRIX_ATTRIBUTE_EMAIL_SEND_RECURRENCE, "values", "0^OneTime,1^Weekly,2^BiWeekly,3^Monthly", "F1F0873E-CA36-4935-90F5-DFEFD79083C8" );

            // reorder existing matrix attributes

            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.AttributeMatrixItem", Rock.SystemGuid.FieldType.EMAIL, "AttributeMatrixTemplateId", attributeMatrixTemplateEmailId, "From Email", "", 2, "", KFSConst.Attribute.MATRIX_ATTRIBUTE_EMAIL_FROM_EMAIL, "rocks.kfs.ScheduledGroupCommunication.EmailFromAddress" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.AttributeMatrixItem", Rock.SystemGuid.FieldType.TEXT, "AttributeMatrixTemplateId", attributeMatrixTemplateEmailId, "From Name", "", 3, "", KFSConst.Attribute.MATRIX_ATTRIBUTE_EMAIL_FROM_NAME, "rocks.kfs.ScheduledGroupCommunication.EmailFromName" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.AttributeMatrixItem", Rock.SystemGuid.FieldType.TEXT, "AttributeMatrixTemplateId", attributeMatrixTemplateEmailId, "Subject", "", 4, "", KFSConst.Attribute.MATRIX_ATTRIBUTE_EMAIL_SUBJECT, "rocks.kfs.ScheduledGroupCommunication.EmailSubject" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.AttributeMatrixItem", Rock.SystemGuid.FieldType.HTML, "AttributeMatrixTemplateId", attributeMatrixTemplateEmailId, "Message", "", 5, "", KFSConst.Attribute.MATRIX_ATTRIBUTE_EMAIL_MESSAGE, "rocks.kfs.ScheduledGroupCommunication.EmailMessage" );

            // set attribute to be required
            Sql( @"
            UPDATE [Attribute] SET [IsRequired] = 1
            WHERE (
                [Guid] = '9BC4F790-A9F7-4822-AEEE-91095A3E3D4C'
            )
            " );

            #endregion

            #region SMS Attributes

            var rockContextSMS = new RockContext();
            var attributeMatrixTemplateSMSId = new AttributeMatrixTemplateService( rockContextSMS ).Get( KFSConst.Attribute.ATTRIBUTE_MATRIX_TEMPLATE_SCHEDULED_SMS.AsGuid() ).Id.ToString();

            // recurrence attribute
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.AttributeMatrixItem", Rock.SystemGuid.FieldType.SINGLE_SELECT, "AttributeMatrixTemplateId", attributeMatrixTemplateSMSId, "Recurrence", "", 1, "0", KFSConst.Attribute.MATRIX_ATTRIBUTE_SMS_SEND_RECURRENCE, "rocks.kfs.ScheduledGroupCommunication.SMSSendRecurrence" );
            RockMigrationHelper.AddAttributeQualifier( KFSConst.Attribute.MATRIX_ATTRIBUTE_SMS_SEND_RECURRENCE, "values", "0^OneTime,1^Weekly,2^BiWeekly,3^Monthly", "4FAE15C5-C1AB-4FDB-B21D-CBF467B1D395" );

            // reorder existing matrix attributes
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.AttributeMatrixItem", Rock.SystemGuid.FieldType.DEFINED_VALUE, "AttributeMatrixTemplateId", attributeMatrixTemplateSMSId, "From Number", "", 2, "", KFSConst.Attribute.MATRIX_ATTRIBUTE_SMS_FROM_NUMBER, "rocks.kfs.ScheduledGroupCommunication.SMSFromNumber" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.AttributeMatrixItem", Rock.SystemGuid.FieldType.MEMO, "AttributeMatrixTemplateId", attributeMatrixTemplateSMSId, "Message", "", 3, "", KFSConst.Attribute.MATRIX_ATTRIBUTE_SMS_MESSAGE, "rocks.kfs.ScheduledGroupCommunication.SMSMessage" );

            // set attribute to be required
            Sql( @"
            UPDATE [Attribute] SET [IsRequired] = 1
            WHERE (
                [Guid] = 'EC6D13F7-A256-4B03-A94B-3B713F26E62D'
            )
            " );

            #endregion
        }

        /// <summary>
        /// The commands to undo a migration from a specific version
        /// </summary>
        public override void Down()
        {
            RockMigrationHelper.DeleteAttribute( KFSConst.Attribute.MATRIX_ATTRIBUTE_EMAIL_SEND_RECURRENCE );
            RockMigrationHelper.DeleteAttribute( KFSConst.Attribute.MATRIX_ATTRIBUTE_SMS_SEND_RECURRENCE );
        }
    }
}
