using Rock.Plugin;

namespace com.kfs.Intacct.Migrations
{
    [MigrationNumber( 3, "1.7.4" )]
    public class AddBlockToBatchDetails : Migration
    {
        /// <summary>
        /// The commands to run to migrate plugin to the specific version
        /// </summary>
        public override void Up()
        {
            // block type
            RockMigrationHelper.UpdateBlockType( "Intacct Batch to Journal", "Block used to create Journal Entries in Intacct from a Rock Financial Batch.", "~/Plugins/com_kfs/Intacct/BatchToJournal.ascx", "com_kfs > Intacct", "5F859264-2E47-41FF-AA63-B57FE400BBC2" );

            // block on the Financial Batch Details page
            RockMigrationHelper.AddBlock( Rock.SystemGuid.Page.FINANCIAL_BATCH_DETAIL, "", "5F859264-2E47-41FF-AA63-B57FE400BBC2", "Intacct Batch To Journal", "Main", "", "", 0, "328FD600-A246-4326-A00C-943A4DF7DC39" );
        }

        /// <summary>
        /// The commands to undo a migration from a specific version
        /// </summary>
        public override void Down()
        {
            RockMigrationHelper.DeleteBlock( "328FD600-A246-4326-A00C-943A4DF7DC39" );
        }
    }
}