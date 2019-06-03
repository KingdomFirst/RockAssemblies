using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Net;
using Newtonsoft.Json;

using Rock;
using Rock.Model;
using Rock.Security;
using Rock.Storage;
using rocks.kfs.FTPHelper;

namespace rocks.kfs.FTPStorageProvider
{
    /// <summary>
    /// Storage provider for saving binary files to a FTP server
    /// </summary>
    [Description( "FTP file storage" )]
    [Export( typeof( ProviderComponent ) )]
    [ExportMetadata( "ComponentName", "FTP Storage" )]
    public class FTPStorage : ProviderComponent
    {
        /// <summary>
        /// Saves the binary file contents to the external storage medium associated with the provider.
        /// </summary>
        /// <param name="binaryFile">The binary file.</param>
        /// <exception cref="System.ArgumentException">File Data must not be null.</exception>
        public override void SaveContent( BinaryFile binaryFile )
        {
            var ftp = GetFTPHelper( binaryFile );
            var remoteFileName = GetRemoteFileName( binaryFile );

            // Write the contents to file
            using ( var inputStream = binaryFile.ContentStream )
            {
                if ( inputStream != null )
                {
                    ftp.Upload( remoteFileName, inputStream );
                }
            }
        }

        /// <summary>
        /// Deletes the content from the external storage medium associated with the provider.
        /// </summary>
        /// <param name="binaryFile">The binary file.</param>
        public override void DeleteContent( BinaryFile binaryFile )
        {
            var ftp = GetFTPHelper( binaryFile );
            var remoteFileName = GetRemoteFileName( binaryFile );
            var fileInfo = ftp.GetFileSize( remoteFileName );
            if ( fileInfo > 0 )
            {
                ftp.Delete( remoteFileName );
            }
        }

        /// <summary>
        /// Gets the contents from the external storage medium associated with the provider
        /// </summary>
        /// <param name="binaryFile">The binary file.</param>
        /// <returns></returns>
        public override Stream GetContentStream( BinaryFile binaryFile )
        {
            var filePath = GetFilePath( binaryFile );

            var request = ( HttpWebRequest ) WebRequest.Create( filePath );
            var response = ( HttpWebResponse ) request.GetResponse();
            var responseStream = response.GetResponseStream();
            var memoryStream = new MemoryStream();

            byte[] buffer = new byte[16 * 1024];
            int read;
            while ( ( read = responseStream.Read( buffer, 0, buffer.Length ) ) > 0 )
            {
                memoryStream.Write( buffer, 0, read );
            }

            return memoryStream;
        }

        /// <summary>
        /// Gets the path.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns></returns>
        public override string GetPath( BinaryFile file )
        {
            return GetFilePath( file );
        }

        /// <summary>
        /// Gets the url.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns></returns>
        public override string GetUrl( BinaryFile file )
        {
            return GetFilePath( file );
        }

        /// <summary>
        /// Gets the file path.
        /// </summary>
        /// <param name="binaryFile">The binary file.</param>
        /// <returns></returns>
        private string GetFilePath( BinaryFile binaryFile )
        {
            if ( binaryFile != null && !string.IsNullOrWhiteSpace( binaryFile.FileName ) )
            {
                string publicAccessUrl = string.Empty;

                try
                {
                    if ( binaryFile.StorageEntitySettings != null )
                    {
                        var settings = JsonConvert.DeserializeObject<Dictionary<string, string>>( binaryFile.StorageEntitySettings );
                        if ( settings != null && settings.ContainsKey( "PublicAccessURL" ) )
                        {
                            publicAccessUrl = settings["PublicAccessURL"];
                        }
                    }
                }
                catch { }

                if ( !string.IsNullOrWhiteSpace( publicAccessUrl ) )
                {
                    publicAccessUrl = publicAccessUrl.EnsureTrailingForwardslash();

                    return string.Format( "{0}{1}", publicAccessUrl, GetRemoteFileName( binaryFile ) );
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Gets the remote file name.
        /// </summary>
        /// <param name="binaryFile">The binary file.</param>
        /// <returns></returns>
        private string GetRemoteFileName( BinaryFile binaryFile )
        {
            if ( binaryFile != null && !string.IsNullOrWhiteSpace( binaryFile.FileName ) )
            {
                return string.Format( "{0}_{1}", binaryFile.Guid, binaryFile.FileName.RemoveSpaces().RemoveSpecialCharacters() );
            }
            return string.Empty;
        }

        /// <summary>
        /// Gets the ftp helper.
        /// </summary>
        /// <param name="binaryFile">The binary file.</param>
        /// <returns></returns>
        private FTP GetFTPHelper( BinaryFile binaryFile )
        {
            var ftpAddress = string.Empty;
            var username = string.Empty;
            var password = string.Empty;

            try
            {
                if ( binaryFile.StorageEntitySettings != null )
                {
                    var settings = JsonConvert.DeserializeObject<Dictionary<string, string>>( binaryFile.StorageEntitySettings );
                    if ( settings != null )
                    {
                        if ( settings.ContainsKey( "FTPAddress" ) )
                        {
                            ftpAddress = settings["FTPAddress"];
                        }

                        if ( settings.ContainsKey( "Username" ) )
                        {
                            username = Encryption.DecryptString( settings["Username"] );
                        }

                        if ( settings.ContainsKey( "Password" ) )
                        {
                            password = Encryption.DecryptString( settings["Password"] );
                        }
                    }
                }
            }
            catch { }

            var ftp = new FTP( ftpAddress.EnsureTrailingForwardslash(), username, password );

            return ftp;
        }
    }
}
