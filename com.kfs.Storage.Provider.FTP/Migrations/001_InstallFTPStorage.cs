using Rock.Data;
using Rock.Plugin;
using System;

namespace com.kfs.FTPStorageProvider.Migrations
{
    [MigrationNumber( 1, "1.6.9" )]
    public class InstallFTPStorage : Migration
    {
        /// <summary>
        /// The commands to run to migrate plugin to the specific version
        /// </summary>
        public override void Up()
        {
            RockMigrationHelper.UpdateEntityType( "com.kfs.FTPStorageProvider.FTPStorage", "DB311F90-0415-4F83-8E61-16BE12C1F89B", false, true );
            RockMigrationHelper.AddEntityAttribute( "Rock.Model.BinaryFileType", "9C204CD0-1233-41C5-818A-C5DA439445AA", "StorageEntityTypeId", "0", "FTP Address", "", "Folder to which selected file will be uploaded. Note, the folder must exist on the FTP server. Example: 'ftp://ftp.mysite.com/audio/weekend/'", 0, "", "CC215A28-869A-4713-9AE4-9282562F59FD", null );
            RockMigrationHelper.AddEntityAttribute( "Rock.Model.BinaryFileType", "36167F3E-8CB2-44F9-9022-102F171FBC9A", "StorageEntityTypeId", "0", "Username", "", "The Username to authenticate with.", 1, "", "6B78A006-C8CD-4420-B7FA-08800384402E", null );
            RockMigrationHelper.AddEntityAttribute( "Rock.Model.BinaryFileType", "36167F3E-8CB2-44F9-9022-102F171FBC9A", "StorageEntityTypeId", "0", "Password", "", "The Password to authenticate with.", 2, "", "9B1425A7-6C65-4C87-AB72-AF508A4FF173", null );
            RockMigrationHelper.AddEntityAttribute( "Rock.Model.BinaryFileType", "C0D0D7E2-C3B0-4004-ABEA-4BBFAD10D5D2", "StorageEntityTypeId", "0", "Public Access URL", "", "The public URL of the content folder. Example: 'https://www.mysite.com/ftp/audio/weekend/'", 3, "", "9F77C47E-D56A-498D-871D-D63A27E1E88C", null );

            Sql( @"
                DECLARE @FTPStorageId int = ( SELECT TOP 1 [Id] FROM [EntityType] WHERE [Guid] = 'DB311F90-0415-4F83-8E61-16BE12C1F89B' )
                UPDATE [Attribute] SET [EntityTypeQualifierValue] = CAST(@FTPStorageId as varchar)
                WHERE [Guid] IN ( 'CC215A28-869A-4713-9AE4-9282562F59FD', '6B78A006-C8CD-4420-B7FA-08800384402E', '9B1425A7-6C65-4C87-AB72-AF508A4FF173', '9F77C47E-D56A-498D-871D-D63A27E1E88C' )
            " );
        }

        /// <summary>
        /// The commands to undo a migration from a specific version
        /// </summary>
        public override void Down()
        {
        }
    }
 }