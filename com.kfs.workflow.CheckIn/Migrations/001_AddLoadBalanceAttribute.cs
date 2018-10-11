using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rock.Plugin;
using Rock.SystemGuid;

namespace com.kfs.Workflow.Action.CheckIn.Migrations
{
    [MigrationNumber( 1, "1.8.0" )]
    public class AddLoadBalanceAttribute : Migration
    {
        /// <summary>
        /// The commands to run to migrate plugin to the specific version
        /// </summary>
        public override void Up()
        {
            // add to checkin by age (inherited by ability and grade)
            RockMigrationHelper.AddGroupTypeGroupAttribute( "0572A5FE-20A4-4BF1-95CD-C71DB5281392", Rock.SystemGuid.FieldType.BOOLEAN, "Load Balance Locations", "Flag indicating if the locations for this group should be presented by current attendance when the person does not have a history.", 3, @"False", "14631054-CCEE-4033-98A6-F27144C943AD" );

            // add to checkin
            RockMigrationHelper.AddGroupTypeGroupAttribute( "6E7AD783-7614-4721-ABC1-35842113EF59", Rock.SystemGuid.FieldType.BOOLEAN, "Load Balance Locations", "Flag indicating if the locations for this group should be presented by current attendance when the person does not have a history.", 0, @"False", "D4B27829-494F-4A45-8A89-5389B3D84B14" );

            // set the attribute key
            Sql( @"
    UPDATE [Attribute]
    SET [Key] = 'com.kfs.LoadBalanceLocations'
    WHERE [Guid] = '14631054-CCEE-4033-98A6-F27144C943AD'
	OR [Guid] = 'D4B27829-494F-4A45-8A89-5389B3D84B14'
" );
        }

        /// <summary>
        /// The commands to undo a migration from a specific version
        /// </summary>
        public override void Down()
        {
            RockMigrationHelper.DeleteAttribute( "14631054-CCEE-4033-98A6-F27144C943AD" );
            RockMigrationHelper.DeleteAttribute( "D4B27829-494F-4A45-8A89-5389B3D84B14" );
        }
    }
}
