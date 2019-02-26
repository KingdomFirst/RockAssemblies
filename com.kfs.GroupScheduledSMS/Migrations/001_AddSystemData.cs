using Rock;
using Rock.Data;
using Rock.Model;
using Rock.Plugin;
using KFSConst = com.kfs.GroupScheduledSMS.SystemGuid;

namespace com.kfs.GroupScheduledSMS.Migrations
{
    [MigrationNumber( 1, "1.8.0" )]
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
                Name = "Scheduled SMS Messages",
                Description = "Used to create scheduled SMS messages.",
                IsActive = true,
                FormattedLava = AttributeMatrixTemplate.FormattedLavaDefault,
                CreatedDateTime = RockDateTime.Now,
                Guid = KFSConst.Attribute.ATTRIBUTE_MATRIX_TEMPLATE_SCHEDULED_SMS.AsGuid()
            };

            var rockContext = new RockContext();
            rockContext.WrapTransaction( () =>
            {
                rockContext.AttributeMatrixTemplates.Add( attributeMatrixTemplate );
                rockContext.SaveChanges();
            } );

            var attributeMatrixTemplateId = new AttributeMatrixTemplateService( rockContext ).Get( KFSConst.Attribute.ATTRIBUTE_MATRIX_TEMPLATE_SCHEDULED_SMS.AsGuid() ).Id.ToString();

            // send date time
            RockMigrationHelper.AddEntityAttribute( "Rock.Model.AttributeMatrixItem", Rock.SystemGuid.FieldType.DATE_TIME, "AttributeMatrixTemplateId", attributeMatrixTemplateId, "Send Date Time", "", "", 0, "", KFSConst.Attribute.MATRIX_ATTRIBUTE_SMS_SEND_DATE, "com.kfs.GroupScheduledSMS.SendDateTime" );
            RockMigrationHelper.AddAttributeQualifier( KFSConst.Attribute.MATRIX_ATTRIBUTE_SMS_SEND_DATE, "datePickerControlType", "Date Picker", "3C8A1A23-F2CD-42F6-BF1E-69AAE87A4701" );
            RockMigrationHelper.AddAttributeQualifier( KFSConst.Attribute.MATRIX_ATTRIBUTE_SMS_SEND_DATE, "displayCurrentOption", "False", "EE2C58B0-5A24-4957-A84D-94829985B2C1" );
            RockMigrationHelper.AddAttributeQualifier( KFSConst.Attribute.MATRIX_ATTRIBUTE_SMS_SEND_DATE, "displayDiff", "False", "B61EB1C8-D811-4D4A-9AC9-2E8B8381E468" );
            RockMigrationHelper.AddAttributeQualifier( KFSConst.Attribute.MATRIX_ATTRIBUTE_SMS_SEND_DATE, "format", "", "83C6F1A4-2514-4C62-A4EA-FD804ABC758C" );
            RockMigrationHelper.AddAttributeQualifier( KFSConst.Attribute.MATRIX_ATTRIBUTE_SMS_SEND_DATE, "futureYearCount", "", "1AFFC6EA-8C45-4125-94DC-25E0E73D8CC6" );

            // from number
            RockMigrationHelper.AddEntityAttribute( "Rock.Model.AttributeMatrixItem", Rock.SystemGuid.FieldType.DEFINED_VALUE, "AttributeMatrixTemplateId", attributeMatrixTemplateId, "From Number", "", "", 1, "", KFSConst.Attribute.MATRIX_ATTRIBUTE_SMS_FROM_NUMBER, "com.kfs.GroupScheduledSMS.FromNumber" );
            RockMigrationHelper.AddAttributeQualifier( KFSConst.Attribute.MATRIX_ATTRIBUTE_SMS_FROM_NUMBER, "allowmultiple", "False", "90051033-C881-42E8-A0CE-4152527D6FA3" );
            RockMigrationHelper.AddAttributeQualifier( KFSConst.Attribute.MATRIX_ATTRIBUTE_SMS_FROM_NUMBER, "definedtype", "", "26143779-5747-416C-98B9-EBC86E4B8EE6" );
            RockMigrationHelper.AddAttributeQualifier( KFSConst.Attribute.MATRIX_ATTRIBUTE_SMS_FROM_NUMBER, "displaydescription", "True", "8AABC104-CFE5-4DF1-A164-2DA305F46BBC" );
            RockMigrationHelper.AddAttributeQualifier( KFSConst.Attribute.MATRIX_ATTRIBUTE_SMS_FROM_NUMBER, "enhancedselection", "False", "B0008343-0033-4194-A1A8-F5AEF3B095AD" );
            
            // message
            RockMigrationHelper.AddEntityAttribute( "Rock.Model.AttributeMatrixItem", Rock.SystemGuid.FieldType.MEMO, "AttributeMatrixTemplateId", attributeMatrixTemplateId, "Message", "", "", 2, "", KFSConst.Attribute.MATRIX_ATTRIBUTE_SMS_MESSAGE, "com.kfs.GroupScheduledSMS.Message" );
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

        /// <summary>
        /// The commands to undo a migration from a specific version
        /// </summary>
        public override void Down()
        {
            RockMigrationHelper.DeleteAttribute( KFSConst.Attribute.MATRIX_ATTRIBUTE_SMS_SEND_DATE );
            RockMigrationHelper.DeleteAttribute( KFSConst.Attribute.MATRIX_ATTRIBUTE_SMS_FROM_NUMBER );
            RockMigrationHelper.DeleteAttribute( KFSConst.Attribute.MATRIX_ATTRIBUTE_SMS_MESSAGE );

            var rockContext = new RockContext();
            var attributeMatrixTemplate = new AttributeMatrixTemplateService( rockContext ).Get( KFSConst.Attribute.ATTRIBUTE_MATRIX_TEMPLATE_SCHEDULED_SMS.AsGuid() );
            rockContext.WrapTransaction( () =>
            {
                rockContext.AttributeMatrixTemplates.Remove( attributeMatrixTemplate );
                rockContext.SaveChanges();
            } );
        }
    }
}
