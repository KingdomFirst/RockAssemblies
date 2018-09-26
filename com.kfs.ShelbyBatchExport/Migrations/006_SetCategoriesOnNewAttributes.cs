using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rock.Plugin;

namespace com.kfs.ShelbyBatchExport.Migrations
{
    [MigrationNumber( 6, "1.6.9" )]
    class SetCategoriesOnNewAttributes : Migration
    {
        /// <summary>
        /// The commands to run to migrate plugin to the specific version
        /// </summary>
        public override void Up()
        {
            RockMigrationHelper.UpdateCategory( "5997C8D3-8840-4591-99A5-552919F90CBD", "Shelby Financials Export", "fa fa-calculator", "", "C19D547F-CD02-45C1-9962-FA1DBCEC2897" ); // batch
            RockMigrationHelper.UpdateCategory( "5997C8D3-8840-4591-99A5-552919F90CBD", "Shelby Financials Export", "fa fa-calculator", "", "DD221639-4EFF-4C16-9E7B-BE318E9E9F55" ); // transaction
            RockMigrationHelper.UpdateCategory( "5997C8D3-8840-4591-99A5-552919F90CBD", "Shelby Financials Export", "fa fa-calculator", "", "B097F23D-00D2-4216-916F-DA14335DA9CE" ); // transaction detail
            RockMigrationHelper.UpdateCategory( "5997C8D3-8840-4591-99A5-552919F90CBD", "Shelby Financials Export", "fa fa-calculator", "", "F8893830-B331-4C9F-AA4C-470F0C9B0D18" ); // account

            Sql( @"
                DECLARE @AccountCategoryId int = ( SELECT TOP 1 [Id] FROM [Category] WHERE [Guid] = 'F8893830-B331-4C9F-AA4C-470F0C9B0D18' )

                INSERT INTO [AttributeCategory]
                SELECT [Id], @AccountCategoryId
                FROM [Attribute]
                WHERE [Guid] = '9B67459C-3C61-491D-B072-9A9830FBB18F'
                    OR [Guid] = '8B8D8BBF-B763-4314-87D2-00B9B8BA5A0F'
                    OR [Guid] = '22699ECA-BB71-4EFD-B416-17B41ED3DBEC'
                    OR [Guid] = 'CD925E61-F87D-461F-9EFA-C1E14397FC4D'
                    OR [Guid] = '65D935EC-3501-41A6-A2C5-CABC62AB9EF1'
" );

            // Clear all cached items
            Rock.Web.Cache.RockCache.ClearAllCachedItems();
        }

        /// <summary>
        /// The commands to undo a migration from a specific version
        /// </summary>
        public override void Down()
        {

        }
    }
}
