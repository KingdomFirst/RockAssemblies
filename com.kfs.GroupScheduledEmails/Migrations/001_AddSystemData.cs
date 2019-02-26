using Rock;
using Rock.Data;
using Rock.Model;
using Rock.Plugin;
using KFSConst = com.kfs.GroupScheduledEmails.SystemGuid;

namespace com.kfs.GroupScheduledEmails.Migrations
{
    [MigrationNumber( 1, "1.7.4" )]
    class AddSystemData : Migration
    {
        /// <summary>
        /// The commands to run to migrate plugin to the specific version
        /// </summary>
        public override void Up()
        {
            // create the attirbute matrix template and get it's id to assign to the attributes we'll create
            var attributeMatrixTemplate = new AttributeMatrixTemplate
            {
                Name = "Scheduled Emails",
                Description = "Used to create scheduled emails.",
                IsActive = true,
                FormattedLava = AttributeMatrixTemplate.FormattedLavaDefault,
                CreatedDateTime = RockDateTime.Now,
                Guid = KFSConst.Attribute.ATTRIBUTE_MATRIX_TEMPLATE_SCHEDULED_EMAILS.AsGuid()
            };

            var rockContext = new RockContext();
            rockContext.WrapTransaction( () =>
            {
                rockContext.AttributeMatrixTemplates.Add( attributeMatrixTemplate );
                rockContext.SaveChanges();
            } );

            var attributeMatrixTemplateId = new AttributeMatrixTemplateService( rockContext ).Get( KFSConst.Attribute.ATTRIBUTE_MATRIX_TEMPLATE_SCHEDULED_EMAILS.AsGuid() ).Id.ToString();

            // send date time
            RockMigrationHelper.AddEntityAttribute( "Rock.Model.AttributeMatrixItem", Rock.SystemGuid.FieldType.DATE_TIME, "AttributeMatrixTemplateId", attributeMatrixTemplateId, "Send Date Time", "", "", 0, "", KFSConst.Attribute.MATRIX_ATTRIBUTE_EMAIL_SEND_DATE, "com.kfs.GroupScheduledEmail.SendDateTime" );
            RockMigrationHelper.AddAttributeQualifier( KFSConst.Attribute.MATRIX_ATTRIBUTE_EMAIL_SEND_DATE, "datePickerControlType", "Date Picker", "3C8A1A23-F2CD-42F6-BF1E-69AAE87A4701" );
            RockMigrationHelper.AddAttributeQualifier( KFSConst.Attribute.MATRIX_ATTRIBUTE_EMAIL_SEND_DATE, "displayCurrentOption", "False", "EE2C58B0-5A24-4957-A84D-94829985B2C1" );
            RockMigrationHelper.AddAttributeQualifier( KFSConst.Attribute.MATRIX_ATTRIBUTE_EMAIL_SEND_DATE, "displayDiff", "False", "B61EB1C8-D811-4D4A-9AC9-2E8B8381E468" );
            RockMigrationHelper.AddAttributeQualifier( KFSConst.Attribute.MATRIX_ATTRIBUTE_EMAIL_SEND_DATE, "format", "", "83C6F1A4-2514-4C62-A4EA-FD804ABC758C" );
            RockMigrationHelper.AddAttributeQualifier( KFSConst.Attribute.MATRIX_ATTRIBUTE_EMAIL_SEND_DATE, "futureYearCount", "", "1AFFC6EA-8C45-4125-94DC-25E0E73D8CC6" );

            // from email
            RockMigrationHelper.AddEntityAttribute( "Rock.Model.AttributeMatrixItem", Rock.SystemGuid.FieldType.EMAIL, "AttributeMatrixTemplateId", attributeMatrixTemplateId, "From Email", "", "", 1, "", KFSConst.Attribute.MATRIX_ATTRIBUTE_EMAIL_FROM_EMAIL, "com.kfs.GroupScheduledEmail.FromEmail" );

            // from name
            RockMigrationHelper.AddEntityAttribute( "Rock.Model.AttributeMatrixItem", Rock.SystemGuid.FieldType.TEXT, "AttributeMatrixTemplateId", attributeMatrixTemplateId, "From Name", "", "", 2, "", KFSConst.Attribute.MATRIX_ATTRIBUTE_EMAIL_FROM_NAME, "com.kfs.GroupScheduledEmail.FromName" );
            RockMigrationHelper.AddAttributeQualifier( KFSConst.Attribute.MATRIX_ATTRIBUTE_EMAIL_FROM_NAME, "ispassword", "false", "1EE77F52-992D-43CE-8574-306E95A7D740" );

            // subject
            RockMigrationHelper.AddEntityAttribute( "Rock.Model.AttributeMatrixItem", Rock.SystemGuid.FieldType.TEXT, "AttributeMatrixTemplateId", attributeMatrixTemplateId, "Subject", "", "", 3, "", KFSConst.Attribute.MATRIX_ATTRIBUTE_EMAIL_SUBJECT, "com.kfs.GroupScheduledEmail.Subject" );
            RockMigrationHelper.AddAttributeQualifier( KFSConst.Attribute.MATRIX_ATTRIBUTE_EMAIL_SUBJECT, "ispassword", "false", "CEB3BEB8-20E9-4A8F-8530-2B68DA02B020" );

            // message
            RockMigrationHelper.AddEntityAttribute( "Rock.Model.AttributeMatrixItem", Rock.SystemGuid.FieldType.HTML, "AttributeMatrixTemplateId", attributeMatrixTemplateId, "Message", "", "", 4, "", KFSConst.Attribute.MATRIX_ATTRIBUTE_EMAIL_MESSAGE, "com.kfs.GroupScheduledEmail.Message" );
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

            var rockContext = new RockContext();
            var attributeMatrixTemplate = new AttributeMatrixTemplateService( rockContext ).Get( KFSConst.Attribute.ATTRIBUTE_MATRIX_TEMPLATE_SCHEDULED_EMAILS.AsGuid() );
            rockContext.WrapTransaction( () =>
            {
                rockContext.AttributeMatrixTemplates.Remove( attributeMatrixTemplate );
                rockContext.SaveChanges();
            } );
        }
    }
}
