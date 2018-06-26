using Rock.Plugin;

namespace com.kfs.Intacct.Migrations
{
    [MigrationNumber( 2, "1.7.4" )]
    public class AddDefinedTypeDetail : Migration
    {
        /// <summary>
        /// The commands to run to migrate plugin to the specific version
        /// </summary>
        public override void Up()
        {
            // add defined type detail to intacct project page
            RockMigrationHelper.AddBlock( "06EB0D2D-A864-48F7-86E3-D72687E3A03C", "", "08C35F15-9AF7-468F-9D50-CDFD3D21220C", "Defined Type Detail", "Main", "", "", 0, "81882390-0BAF-4DAC-ADDF-2637C47ECD7B" );
            RockMigrationHelper.AddBlockAttributeValue( "81882390-0BAF-4DAC-ADDF-2637C47ECD7B", "0305EF98-C791-4626-9996-F189B9BB674C", @"C244D4C4-636F-4BCA-8E7C-1907933ABB74" );

            // set defined value list as secord order in zone
            Sql( @"
            UPDATE [Block]
            SET [Order] = 1
            WHERE [Guid] = 'B9A1B5EA-3402-4F68-9190-5EA10B166569'
            " );
        }

        /// <summary>
        /// The commands to undo a migration from a specific version
        /// </summary>
        public override void Down()
        {
            RockMigrationHelper.DeleteBlockAttributeValue( "81882390-0BAF-4DAC-ADDF-2637C47ECD7B", "0305EF98-C791-4626-9996-F189B9BB674C" );
            RockMigrationHelper.DeleteBlock( "81882390-0BAF-4DAC-ADDF-2637C47ECD7B" );
        }
    }
}