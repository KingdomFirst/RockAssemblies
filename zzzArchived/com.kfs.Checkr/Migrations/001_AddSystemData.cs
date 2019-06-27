using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rock.Plugin;
using Rock.SystemGuid;

namespace com.kfs.Checkr.Migrations
{
    [MigrationNumber( 1, "1.6.9" )]
    public class AddSystemData : Migration
    {
        /// <summary>
        /// The commands to run to migrate plugin to the specific version
        /// </summary>
        public override void Up()
        {
            RockMigrationHelper.AddDefinedType( "Global", "Background Check Types (Checkr)", "The type of background checks that are available to be used with Checkr. Packages are specific per account, use the name provided on the Checkr Dashboard Home as a guide. i.e. 'Basic criminal' would be added here as Value: 'basic_criminal' Description: 'Basic Criminal'.", SystemGuid.DefinedType.CHECKR_PACKAGES );
            RockMigrationHelper.AddDefinedValue( SystemGuid.DefinedType.CHECKR_PACKAGES, "tasker_standard", "Tasker Standard", "50cf25d8-9409-4164-b426-480abdfe11b9", false );
            RockMigrationHelper.AddDefinedValue( SystemGuid.DefinedType.CHECKR_PACKAGES, "tasker_pro", "Tasker Pro", "fb84e91d-3485-4e26-af96-7404147805d8", false );
            RockMigrationHelper.AddDefinedValue( SystemGuid.DefinedType.CHECKR_PACKAGES, "driver_standard", "Driver Standard", "9782e533-4938-4f85-a027-daa26be51bc7", false );
            RockMigrationHelper.AddDefinedValue( SystemGuid.DefinedType.CHECKR_PACKAGES, "driver_pro", "Driver Pro", "a272c411-9a9b-43ae-810d-892ea8c1db95", false );
            RockMigrationHelper.AddDefinedValue( SystemGuid.DefinedType.CHECKR_PACKAGES, "tasker_basic", "Tasker Basic", "9cdb61e7-8083-4a6f-9606-c12cd24aaa10", false );
            RockMigrationHelper.AddDefinedValue( SystemGuid.DefinedType.CHECKR_PACKAGES, "employment_verification_package", "Employment Verification Package", "f5dfe151-dc3b-44e5-8e55-cdd52a7ec0cb", false );
            RockMigrationHelper.AddDefinedValue( SystemGuid.DefinedType.CHECKR_PACKAGES, "education_package", "Education Package", "b54dc0c4-bd34-4c91-bd1c-c70c3c482350", false );

            RockMigrationHelper.UpdatePersonAttribute( Rock.SystemGuid.FieldType.URL_LINK, "4D1E1EBA-ABF2-4A7C-8ADF-65CB5AAE94E2", "Checkr Candidate Link", "CheckrCandidateLink", "", "", 6, "", "72e7649e-04c1-4787-adf9-69fc12971bb0" );
        }

        /// <summary>
        /// The commands to undo a migration from a specific version
        /// </summary>
        public override void Down()
        {
            RockMigrationHelper.DeleteDefinedType( SystemGuid.DefinedType.CHECKR_PACKAGES );
        }
    }
}
