using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rock.Plugin;
using Rock.SystemGuid;

namespace com.kfs.MinistrySafe.Migrations
{
    [MigrationNumber( 1, "1.6.9" )]
    public class AddMinistrySafeElements : Migration
    {
        /// <summary>
        /// The commands to run to migrate plugin to the specific version
        /// </summary>
        public override void Up()
        {
            // Global Attributes
            RockMigrationHelper.AddGlobalAttribute( FieldType.ENCRYPTED_TEXT, "", "", "Ministry Safe API Key", "", 0, "", "E246DBBA-3092-4B26-A3A0-292AD3F68C33" );

            // Training Types
            RockMigrationHelper.AddDefinedType( "Global", "Ministry Safe Training Types", "The type of Ministry Safe Trainings available.", SystemGuid.DefinedType.MINISTRY_SAFE_TRAINING_TYPES );
            RockMigrationHelper.AddDefinedValue( SystemGuid.DefinedType.MINISTRY_SAFE_TRAINING_TYPES, "standard", "Standard Sexual Abuse Awareness Training", "16A04162-2396-4A64-937D-E222ADA2CBB4", true );
            RockMigrationHelper.AddDefinedValue( SystemGuid.DefinedType.MINISTRY_SAFE_TRAINING_TYPES, "youth", "Youth Sports Sexual Abuse Awareness Training", "F422B142-7621-4FF4-B143-FA6C9833824C", true );
            RockMigrationHelper.AddDefinedValue( SystemGuid.DefinedType.MINISTRY_SAFE_TRAINING_TYPES, "camp", "Camp-Focused Sexual Abuse Awareness Training", "41946C59-A7FC-4FDD-AF8F-A68F1A0F4C34", true );
            RockMigrationHelper.AddDefinedValue( SystemGuid.DefinedType.MINISTRY_SAFE_TRAINING_TYPES, "spanish", "Spanish Sexual Abuse Awareness Training", "B3F7014A-2433-4889-BA9B-ECB7701A99C5", true );

            // Person Attribute Category
            RockMigrationHelper.UpdatePersonAttributeCategory( "Ministry Safe", "fa fa-clipboard", "", SystemGuid.Category.MINISTRY_SAFE_PERSON_ATTRIBUTE_CATEGORY );

            // Person Attribute Training Type
            RockMigrationHelper.UpdatePersonAttribute( FieldType.DEFINED_VALUE, SystemGuid.Category.MINISTRY_SAFE_PERSON_ATTRIBUTE_CATEGORY, "Ministry Safe Training Type", "MSTrainingType", "", "The Ministry Safe training type that this person completed.", 0, "", SystemGuid.Attribute.MINISTRY_SAFE_PERSON_ATTRIBUTE_TEST_TYPE );
            RockMigrationHelper.AddAttributeQualifier( SystemGuid.Attribute.MINISTRY_SAFE_PERSON_ATTRIBUTE_TEST_TYPE, "allowmultiple", "False", "13C4E727-21E2-4A7D-968A-2BA55B5AA567" );
            RockMigrationHelper.AddAttributeQualifier( SystemGuid.Attribute.MINISTRY_SAFE_PERSON_ATTRIBUTE_TEST_TYPE, "definedtype", "", "110572F2-A7EA-40C3-8C93-732313CEACBC" );
            RockMigrationHelper.AddAttributeQualifier( SystemGuid.Attribute.MINISTRY_SAFE_PERSON_ATTRIBUTE_TEST_TYPE, "displaydescription", "True", "61DF6B3C-06B6-488D-ADCC-769A46FBE1EA" );

            // Person Attribute Test Date
            RockMigrationHelper.UpdatePersonAttribute( FieldType.DATE, SystemGuid.Category.MINISTRY_SAFE_PERSON_ATTRIBUTE_CATEGORY, "Ministry Safe Training Date", "MSTrainingDate", "", "Date the user took the Ministry Safe training.", 1, "", SystemGuid.Attribute.MINISTRY_SAFE_PERSON_ATTRIBUTE_TEST_DATE );
            RockMigrationHelper.AddAttributeQualifier( SystemGuid.Attribute.MINISTRY_SAFE_PERSON_ATTRIBUTE_TEST_DATE, "displayDiff", "False", "133DEB74-6412-4DC6-AE3C-EB2E36BD9AC7" );
            RockMigrationHelper.AddAttributeQualifier( SystemGuid.Attribute.MINISTRY_SAFE_PERSON_ATTRIBUTE_TEST_DATE, "format", "", "652B01D4-DF27-4DA8-9D28-9BFFDC3D543C" );

            // Person Attribute Score
            RockMigrationHelper.UpdatePersonAttribute( FieldType.INTEGER, SystemGuid.Category.MINISTRY_SAFE_PERSON_ATTRIBUTE_CATEGORY, "Ministry Safe Training Score", "MSTrainingScore", "", "The user's score from the Ministry Safe training.", 2, "", SystemGuid.Attribute.MINISTRY_SAFE_PERSON_ATTRIBUTE_TEST_SCORE );

            // Set the Test Type Defined Type Id as the Qualifer Value for the Person Attribute
            Sql( @"
    DECLARE @DefinedTypeId INT = ( SELECT Id FROM [DefinedType] WHERE [Guid] = '56DA549B-EE02-4F6C-9CE4-60C2270751F2' )

    UPDATE [AttributeQualifier]
    SET [Value] = @DefinedTypeId
    WHERE [Guid] = '110572F2-A7EA-40C3-8C93-732313CEACBC'
" );

            // Put person attribute block on Extended Attributes page
            RockMigrationHelper.AddBlock( "1C737278-4CBA-404B-B6B3-E3F0E05AB5FE", "", "D70A59DC-16BE-43BE-9880-59598FA7A94C", "Ministry Safe", "SectionB2", "", "", 0, "63FF2A72-B3B7-4CD9-A652-91B8230773A6" );
            RockMigrationHelper.AddBlockAttributeValue( "63FF2A72-B3B7-4CD9-A652-91B8230773A6", "EC43CF32-3BDF-4544-8B6A-CE9208DD7C81", SystemGuid.Category.MINISTRY_SAFE_PERSON_ATTRIBUTE_CATEGORY );
        }

        /// <summary>
        /// The commands to undo a migration from a specific version
        /// </summary>
        public override void Down()
        {
            RockMigrationHelper.DeleteBlock( "63FF2A72-B3B7-4CD9-A652-91B8230773A6" );
            RockMigrationHelper.DeleteAttribute( SystemGuid.Attribute.MINISTRY_SAFE_PERSON_ATTRIBUTE_TEST_TYPE );
            RockMigrationHelper.DeleteAttribute( SystemGuid.Attribute.MINISTRY_SAFE_PERSON_ATTRIBUTE_TEST_DATE );
            RockMigrationHelper.DeleteAttribute( SystemGuid.Attribute.MINISTRY_SAFE_PERSON_ATTRIBUTE_TEST_SCORE );
            RockMigrationHelper.DeleteCategory( SystemGuid.Category.MINISTRY_SAFE_PERSON_ATTRIBUTE_CATEGORY );
            RockMigrationHelper.DeleteDefinedType( SystemGuid.DefinedType.MINISTRY_SAFE_TRAINING_TYPES );
        }
    }
}
