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
    [MigrationNumber( 1, "1.7.4" )]
    internal class AddSystemData : Migration
    {
        /// <summary>
        /// The commands to run to migrate plugin to the specific version
        /// </summary>
        public override void Up()
        {
            var migrateNamespaceEmail = false;
            var migrateNamespaceSMS = false;
            var oldNamespaceEmail = "com.kfs.GroupScheduledEmails";
            var oldNamespaceSMS = "com.kfs.GroupScheduledSMS";

            // check if migration has previously run
            using ( var rockContext = new RockContext() )
            {
                var migrationNumber = ( System.Attribute.GetCustomAttribute( this.GetType(), typeof( MigrationNumberAttribute ) ) as MigrationNumberAttribute ).Number;

                migrateNamespaceEmail = new PluginMigrationService( rockContext )
                    .Queryable()
                    .Where( m => m.PluginAssemblyName.Equals( oldNamespaceEmail, StringComparison.CurrentCultureIgnoreCase ) && m.MigrationNumber == migrationNumber )
                    .Any();

                migrateNamespaceSMS = new PluginMigrationService( rockContext )
                    .Queryable()
                    .Where( m => m.PluginAssemblyName.Equals( oldNamespaceSMS, StringComparison.CurrentCultureIgnoreCase ) && m.MigrationNumber == migrationNumber )
                    .Any();
            }

            #region Email Attributes

            if ( migrateNamespaceEmail )
            {
                var attributeMatrixTemplateEmailId = new AttributeMatrixTemplateService( new RockContext() ).Get( KFSConst.Attribute.ATTRIBUTE_MATRIX_TEMPLATE_SCHEDULED_EMAILS.AsGuid() ).Id.ToString();

                RockMigrationHelper.EnsureAttributeByGuid( KFSConst.Attribute.MATRIX_ATTRIBUTE_EMAIL_SEND_DATE, "rocks.kfs.ScheduledGroupCommunication.EmailSendDateTime", "3c9d5021-0484-4846-aef6-b6216d26c3c8", Rock.SystemGuid.FieldType.DATE_TIME, "AttributeMatrixTemplateId", attributeMatrixTemplateEmailId );
                RockMigrationHelper.EnsureAttributeByGuid( KFSConst.Attribute.MATRIX_ATTRIBUTE_EMAIL_FROM_EMAIL, "rocks.kfs.ScheduledGroupCommunication.EmailFromAddress", "3c9d5021-0484-4846-aef6-b6216d26c3c8", Rock.SystemGuid.FieldType.EMAIL, "AttributeMatrixTemplateId", attributeMatrixTemplateEmailId );
                RockMigrationHelper.EnsureAttributeByGuid( KFSConst.Attribute.MATRIX_ATTRIBUTE_EMAIL_FROM_NAME, "rocks.kfs.ScheduledGroupCommunication.EmailFromName", "3c9d5021-0484-4846-aef6-b6216d26c3c8", Rock.SystemGuid.FieldType.TEXT, "AttributeMatrixTemplateId", attributeMatrixTemplateEmailId );
                RockMigrationHelper.EnsureAttributeByGuid( KFSConst.Attribute.MATRIX_ATTRIBUTE_EMAIL_SUBJECT, "rocks.kfs.ScheduledGroupCommunication.EmailSubject", "3c9d5021-0484-4846-aef6-b6216d26c3c8", Rock.SystemGuid.FieldType.TEXT, "AttributeMatrixTemplateId", attributeMatrixTemplateEmailId );
                RockMigrationHelper.EnsureAttributeByGuid( KFSConst.Attribute.MATRIX_ATTRIBUTE_EMAIL_MESSAGE, "rocks.kfs.ScheduledGroupCommunication.EmailMessage", "3c9d5021-0484-4846-aef6-b6216d26c3c8", Rock.SystemGuid.FieldType.HTML, "AttributeMatrixTemplateId", attributeMatrixTemplateEmailId );

                using ( var rockContext = new RockContext() )
                {
                    // look for any configured jobs and change the class name
                    var emailJobs = new ServiceJobService( rockContext )
                        .Queryable()
                        .Where( j => j.Class.Equals( "com.kfs.GroupScheduledEmails.Jobs.SendScheduledGroupEmail", StringComparison.CurrentCultureIgnoreCase ) )
                        .ToList();

                    foreach ( var job in emailJobs )
                    {
                        job.Class = "rocks.kfs.ScheduledGroupCommunication.Jobs.SendScheduledGroupEmail";
                    }

                    // look for job attributes and change qualifier value
                    var attributes = new AttributeService( rockContext )
                        .Queryable()
                        .Where( a => a.EntityTypeQualifierValue.Equals( "com.kfs.GroupScheduledEmails.Jobs.SendScheduledGroupEmail", StringComparison.CurrentCultureIgnoreCase ) )
                        .ToList();

                    foreach ( var attribute in attributes )
                    {
                        attribute.EntityTypeQualifierValue = "rocks.kfs.ScheduledGroupCommunication.Jobs.SendScheduledGroupEmail";
                    }

                    rockContext.SaveChanges();
                }
            }
            else
            {
                // create the attribute matrix template and get it's id to assign to the attributes we'll create
                var attributeMatrixTemplateEmail = new AttributeMatrixTemplate
                {
                    Name = "Scheduled Emails",
                    Description = "Used to create scheduled emails.",
                    IsActive = true,
                    FormattedLava = AttributeMatrixTemplate.FormattedLavaDefault,
                    CreatedDateTime = RockDateTime.Now,
                    Guid = KFSConst.Attribute.ATTRIBUTE_MATRIX_TEMPLATE_SCHEDULED_EMAILS.AsGuid()
                };

                var rockContextEmail = new RockContext();
                rockContextEmail.WrapTransaction( () =>
                {
                    rockContextEmail.AttributeMatrixTemplates.Add( attributeMatrixTemplateEmail );
                    rockContextEmail.SaveChanges();
                } );

                var attributeMatrixTemplateEmailId = new AttributeMatrixTemplateService( rockContextEmail ).Get( KFSConst.Attribute.ATTRIBUTE_MATRIX_TEMPLATE_SCHEDULED_EMAILS.AsGuid() ).Id.ToString();

                // send date time
                RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.AttributeMatrixItem", Rock.SystemGuid.FieldType.DATE_TIME, "AttributeMatrixTemplateId", attributeMatrixTemplateEmailId, "Send Date Time", "", 0, "", KFSConst.Attribute.MATRIX_ATTRIBUTE_EMAIL_SEND_DATE, "rocks.kfs.ScheduledGroupCommunication.EmailSendDateTime" );
                RockMigrationHelper.AddAttributeQualifier( KFSConst.Attribute.MATRIX_ATTRIBUTE_EMAIL_SEND_DATE, "datePickerControlType", "Date Picker", "3C8A1A23-F2CD-42F6-BF1E-69AAE87A4701" );
                RockMigrationHelper.AddAttributeQualifier( KFSConst.Attribute.MATRIX_ATTRIBUTE_EMAIL_SEND_DATE, "displayCurrentOption", "False", "EE2C58B0-5A24-4957-A84D-94829985B2C1" );
                RockMigrationHelper.AddAttributeQualifier( KFSConst.Attribute.MATRIX_ATTRIBUTE_EMAIL_SEND_DATE, "displayDiff", "False", "B61EB1C8-D811-4D4A-9AC9-2E8B8381E468" );
                RockMigrationHelper.AddAttributeQualifier( KFSConst.Attribute.MATRIX_ATTRIBUTE_EMAIL_SEND_DATE, "format", "", "83C6F1A4-2514-4C62-A4EA-FD804ABC758C" );
                RockMigrationHelper.AddAttributeQualifier( KFSConst.Attribute.MATRIX_ATTRIBUTE_EMAIL_SEND_DATE, "futureYearCount", "", "1AFFC6EA-8C45-4125-94DC-25E0E73D8CC6" );

                // from email
                RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.AttributeMatrixItem", Rock.SystemGuid.FieldType.EMAIL, "AttributeMatrixTemplateId", attributeMatrixTemplateEmailId, "From Email", "", 1, "", KFSConst.Attribute.MATRIX_ATTRIBUTE_EMAIL_FROM_EMAIL, "rocks.kfs.ScheduledGroupCommunication.EmailFromAddress" );

                // from name
                RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.AttributeMatrixItem", Rock.SystemGuid.FieldType.TEXT, "AttributeMatrixTemplateId", attributeMatrixTemplateEmailId, "From Name", "", 2, "", KFSConst.Attribute.MATRIX_ATTRIBUTE_EMAIL_FROM_NAME, "rocks.kfs.ScheduledGroupCommunication.EmailFromName" );
                RockMigrationHelper.AddAttributeQualifier( KFSConst.Attribute.MATRIX_ATTRIBUTE_EMAIL_FROM_NAME, "ispassword", "false", "1EE77F52-992D-43CE-8574-306E95A7D740" );

                // subject
                RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.AttributeMatrixItem", Rock.SystemGuid.FieldType.TEXT, "AttributeMatrixTemplateId", attributeMatrixTemplateEmailId, "Subject", "", 3, "", KFSConst.Attribute.MATRIX_ATTRIBUTE_EMAIL_SUBJECT, "rocks.kfs.ScheduledGroupCommunication.EmailSubject" );
                RockMigrationHelper.AddAttributeQualifier( KFSConst.Attribute.MATRIX_ATTRIBUTE_EMAIL_SUBJECT, "ispassword", "false", "CEB3BEB8-20E9-4A8F-8530-2B68DA02B020" );

                // message
                RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.AttributeMatrixItem", Rock.SystemGuid.FieldType.HTML, "AttributeMatrixTemplateId", attributeMatrixTemplateEmailId, "Message", "", 4, "", KFSConst.Attribute.MATRIX_ATTRIBUTE_EMAIL_MESSAGE, "rocks.kfs.ScheduledGroupCommunication.EmailMessage" );
                RockMigrationHelper.AddAttributeQualifier( KFSConst.Attribute.MATRIX_ATTRIBUTE_EMAIL_MESSAGE, "documentfolderroot", "", "4B2FFA29-5B9C-4F74-ACE5-54C97B68E0B6" );
                RockMigrationHelper.AddAttributeQualifier( KFSConst.Attribute.MATRIX_ATTRIBUTE_EMAIL_MESSAGE, "imagefolderroot", "", "77C6B7A9-69DF-466F-AFEC-DC41B33E0DF0" );
                RockMigrationHelper.AddAttributeQualifier( KFSConst.Attribute.MATRIX_ATTRIBUTE_EMAIL_MESSAGE, "toolbar", "Light", "E1E5C3F2-F64C-4942-A5B3-B712125BF8CB" );
                RockMigrationHelper.AddAttributeQualifier( KFSConst.Attribute.MATRIX_ATTRIBUTE_EMAIL_MESSAGE, "userspecificroot", "False", "C3FF1EBB-7EFF-4747-A326-F6BCD669DD35" );

                // set all attributes to be required
                Sql( @"
                UPDATE [Attribute] SET [IsRequired] = 1
                WHERE (
                    [Guid] = '39A38B02-112C-4EDC-A30E-4BDB1B090EE4'
                    OR
                    [Guid] = 'F7C73002-6442-4756-BFDB-BC0BFE58EF15'
                    OR
                    [Guid] = '7BEE419A-8444-44E1-B7EB-451C038977B3'
                    OR
                    [Guid] = '9EC7C8A9-F4C9-421C-9129-2DD023E09D05'
                    OR
                    [Guid] = '8C4EE7A8-086D-42B7-908F-77A9A36E5342'
                )
                " );
            }

            #endregion

            #region SMS Attributes

            if ( migrateNamespaceSMS )
            {
                var attributeMatrixTemplateSMSId = new AttributeMatrixTemplateService( new RockContext() ).Get( KFSConst.Attribute.ATTRIBUTE_MATRIX_TEMPLATE_SCHEDULED_SMS.AsGuid() ).Id.ToString();

                RockMigrationHelper.EnsureAttributeByGuid( KFSConst.Attribute.MATRIX_ATTRIBUTE_SMS_SEND_DATE, "rocks.kfs.ScheduledGroupCommunication.SMSSendDateTime", "3c9d5021-0484-4846-aef6-b6216d26c3c8", Rock.SystemGuid.FieldType.DATE_TIME, "AttributeMatrixTemplateId", attributeMatrixTemplateSMSId );
                RockMigrationHelper.EnsureAttributeByGuid( KFSConst.Attribute.MATRIX_ATTRIBUTE_SMS_FROM_NUMBER, "rocks.kfs.ScheduledGroupCommunication.SMSFromNumber", "3c9d5021-0484-4846-aef6-b6216d26c3c8", Rock.SystemGuid.FieldType.DEFINED_VALUE, "AttributeMatrixTemplateId", attributeMatrixTemplateSMSId );
                RockMigrationHelper.EnsureAttributeByGuid( KFSConst.Attribute.MATRIX_ATTRIBUTE_SMS_MESSAGE, "rocks.kfs.ScheduledGroupCommunication.SMSMessage", "3c9d5021-0484-4846-aef6-b6216d26c3c8", Rock.SystemGuid.FieldType.MEMO, "AttributeMatrixTemplateId", attributeMatrixTemplateSMSId );

                using ( var rockContext = new RockContext() )
                {
                    // look for any configured jobs and change the class name
                    var smsJobs = new ServiceJobService( rockContext )
                        .Queryable()
                        .Where( j => j.Class.Equals( "com.kfs.GroupScheduledSMS.Jobs.SendScheduledGroupSMS", StringComparison.CurrentCultureIgnoreCase ) )
                        .ToList();

                    foreach ( var job in smsJobs )
                    {
                        job.Class = "rocks.kfs.ScheduledGroupCommunication.Jobs.SendScheduledGroupSMS";
                    }

                    // look for job attributes and change qualifier value
                    var attributes = new AttributeService( rockContext )
                        .Queryable()
                        .Where( a => a.EntityTypeQualifierValue.Equals( "com.kfs.GroupScheduledSMS.Jobs.SendScheduledGroupSMS", StringComparison.CurrentCultureIgnoreCase ) )
                        .ToList();

                    foreach ( var attribute in attributes )
                    {
                        attribute.EntityTypeQualifierValue = "rocks.kfs.ScheduledGroupCommunication.Jobs.SendScheduledGroupSMS";
                    }

                    rockContext.SaveChanges();
                }
            }
            else
            {
                var attributeMatrixTemplateSMS = new AttributeMatrixTemplate
                {
                    Name = "Scheduled SMS Messages",
                    Description = "Used to create scheduled SMS messages.",
                    IsActive = true,
                    FormattedLava = AttributeMatrixTemplate.FormattedLavaDefault,
                    CreatedDateTime = RockDateTime.Now,
                    Guid = KFSConst.Attribute.ATTRIBUTE_MATRIX_TEMPLATE_SCHEDULED_SMS.AsGuid()
                };

                var rockContextSMS = new RockContext();
                rockContextSMS.WrapTransaction( () =>
                {
                    rockContextSMS.AttributeMatrixTemplates.Add( attributeMatrixTemplateSMS );
                    rockContextSMS.SaveChanges();
                } );

                var attributeMatrixTemplateSMSId = new AttributeMatrixTemplateService( rockContextSMS ).Get( KFSConst.Attribute.ATTRIBUTE_MATRIX_TEMPLATE_SCHEDULED_SMS.AsGuid() ).Id.ToString();

                // send date time
                RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.AttributeMatrixItem", Rock.SystemGuid.FieldType.DATE_TIME, "AttributeMatrixTemplateId", attributeMatrixTemplateSMSId, "Send Date Time", "", 0, "", KFSConst.Attribute.MATRIX_ATTRIBUTE_SMS_SEND_DATE, "rocks.kfs.ScheduledGroupCommunication.SMSSendDateTime" );
                RockMigrationHelper.AddAttributeQualifier( KFSConst.Attribute.MATRIX_ATTRIBUTE_SMS_SEND_DATE, "datePickerControlType", "Date Picker", "3C8A1A23-F2CD-42F6-BF1E-69AAE87A4701" );
                RockMigrationHelper.AddAttributeQualifier( KFSConst.Attribute.MATRIX_ATTRIBUTE_SMS_SEND_DATE, "displayCurrentOption", "False", "EE2C58B0-5A24-4957-A84D-94829985B2C1" );
                RockMigrationHelper.AddAttributeQualifier( KFSConst.Attribute.MATRIX_ATTRIBUTE_SMS_SEND_DATE, "displayDiff", "False", "B61EB1C8-D811-4D4A-9AC9-2E8B8381E468" );
                RockMigrationHelper.AddAttributeQualifier( KFSConst.Attribute.MATRIX_ATTRIBUTE_SMS_SEND_DATE, "format", "", "83C6F1A4-2514-4C62-A4EA-FD804ABC758C" );
                RockMigrationHelper.AddAttributeQualifier( KFSConst.Attribute.MATRIX_ATTRIBUTE_SMS_SEND_DATE, "futureYearCount", "", "1AFFC6EA-8C45-4125-94DC-25E0E73D8CC6" );

                // from number
                RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.AttributeMatrixItem", Rock.SystemGuid.FieldType.DEFINED_VALUE, "AttributeMatrixTemplateId", attributeMatrixTemplateSMSId, "From Number", "", 1, "", KFSConst.Attribute.MATRIX_ATTRIBUTE_SMS_FROM_NUMBER, "rocks.kfs.ScheduledGroupCommunication.SMSFromNumber" );
                RockMigrationHelper.AddAttributeQualifier( KFSConst.Attribute.MATRIX_ATTRIBUTE_SMS_FROM_NUMBER, "allowmultiple", "False", "90051033-C881-42E8-A0CE-4152527D6FA3" );
                RockMigrationHelper.AddAttributeQualifier( KFSConst.Attribute.MATRIX_ATTRIBUTE_SMS_FROM_NUMBER, "definedtype", "", "26143779-5747-416C-98B9-EBC86E4B8EE6" );
                RockMigrationHelper.AddAttributeQualifier( KFSConst.Attribute.MATRIX_ATTRIBUTE_SMS_FROM_NUMBER, "displaydescription", "True", "8AABC104-CFE5-4DF1-A164-2DA305F46BBC" );
                RockMigrationHelper.AddAttributeQualifier( KFSConst.Attribute.MATRIX_ATTRIBUTE_SMS_FROM_NUMBER, "enhancedselection", "False", "B0008343-0033-4194-A1A8-F5AEF3B095AD" );

                // message
                RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.AttributeMatrixItem", Rock.SystemGuid.FieldType.MEMO, "AttributeMatrixTemplateId", attributeMatrixTemplateSMSId, "Message", "", 2, "", KFSConst.Attribute.MATRIX_ATTRIBUTE_SMS_MESSAGE, "rocks.kfs.ScheduledGroupCommunication.SMSMessage" );
                RockMigrationHelper.AddAttributeQualifier( KFSConst.Attribute.MATRIX_ATTRIBUTE_SMS_MESSAGE, "numberofrows", "3", "BB1F72CF-46E3-4ED5-B7C8-BAD274744435" );
                RockMigrationHelper.AddAttributeQualifier( KFSConst.Attribute.MATRIX_ATTRIBUTE_SMS_MESSAGE, "allowhtml", "False", "0C97ED08-FF8D-4BF3-ADD1-C688EFB999D1" );
                RockMigrationHelper.AddAttributeQualifier( KFSConst.Attribute.MATRIX_ATTRIBUTE_SMS_MESSAGE, "maxcharacters", "160", "A54E62A3-05BF-4458-8F56-408A2132BDA7" );
                RockMigrationHelper.AddAttributeQualifier( KFSConst.Attribute.MATRIX_ATTRIBUTE_SMS_MESSAGE, "showcountdown", "True", "8808BF22-2051-488F-83E5-7188D2941334" );

                // set all attributes to be required
                Sql( @"
                UPDATE [Attribute] SET [IsRequired] = 1
                WHERE (
                    [Guid] = 'B2125940-565B-42CE-82BE-CDA58FC65FDE'
                    OR
                    [Guid] = '1984A561-C4A9-4D4F-B366-23AD54BDCFE8'
                    OR
                    [Guid] = 'C57166D5-C0D3-4DA6-88DD-92AFA5126D69'
                )

                DECLARE @CommunicationSMSFromDefinedTypeId int = ( SELECT TOP 1 [Id] FROM [DefinedType] WHERE [Guid] = '611BDE1F-7405-4D16-8626-CCFEDB0E62BE' )

                UPDATE [AttributeQualifier] SET [Value] = CAST( @CommunicationSMSFromDefinedTypeId AS varchar )
                WHERE [Guid] = '26143779-5747-416C-98B9-EBC86E4B8EE6'
                " );
            }

            #endregion
        }

        /// <summary>
        /// The commands to undo a migration from a specific version
        /// </summary>
        public override void Down()
        {
            RockMigrationHelper.DeleteAttribute( KFSConst.Attribute.MATRIX_ATTRIBUTE_EMAIL_SEND_DATE );
            RockMigrationHelper.DeleteAttribute( KFSConst.Attribute.MATRIX_ATTRIBUTE_EMAIL_FROM_EMAIL );
            RockMigrationHelper.DeleteAttribute( KFSConst.Attribute.MATRIX_ATTRIBUTE_EMAIL_FROM_NAME );
            RockMigrationHelper.DeleteAttribute( KFSConst.Attribute.MATRIX_ATTRIBUTE_EMAIL_SUBJECT );
            RockMigrationHelper.DeleteAttribute( KFSConst.Attribute.MATRIX_ATTRIBUTE_EMAIL_MESSAGE );
            RockMigrationHelper.DeleteAttribute( KFSConst.Attribute.MATRIX_ATTRIBUTE_SMS_SEND_DATE );
            RockMigrationHelper.DeleteAttribute( KFSConst.Attribute.MATRIX_ATTRIBUTE_SMS_FROM_NUMBER );
            RockMigrationHelper.DeleteAttribute( KFSConst.Attribute.MATRIX_ATTRIBUTE_SMS_MESSAGE );

            var rockContext = new RockContext();
            var attributeMatrixTemplateEmail = new AttributeMatrixTemplateService( rockContext ).Get( KFSConst.Attribute.ATTRIBUTE_MATRIX_TEMPLATE_SCHEDULED_EMAILS.AsGuid() );
            var attributeMatrixTemplateSMS = new AttributeMatrixTemplateService( rockContext ).Get( KFSConst.Attribute.ATTRIBUTE_MATRIX_TEMPLATE_SCHEDULED_SMS.AsGuid() );
            rockContext.WrapTransaction( () =>
            {
                rockContext.AttributeMatrixTemplates.Remove( attributeMatrixTemplateEmail );
                rockContext.AttributeMatrixTemplates.Remove( attributeMatrixTemplateSMS );
                rockContext.SaveChanges();
            } );
        }
    }
}
