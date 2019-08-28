// <copyright>
// Copyright 2019 by Kingdom First Solutions
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//
using System;
using System.IO;
using System.Net;
using Rock;

namespace rocks.kfs.FTPHelper
{
    public class FTP
    {
        private string host = null;
        private string user = null;
        private string pass = null;
        private FtpWebRequest ftpRequest = null;
        private FtpWebResponse ftpResponse = null;
        private Stream ftpStream = null;
        private int bufferSize = 2048;

        /* Construct Object */

        public FTP( string hostAddress, string userName, string password )
        {
            host = hostAddress;
            user = userName;
            pass = password;
        }

        /* Download File */

        public void Download( string remoteFile, string localFile )
        {
            try
            {
                /* Create an FTP Request */
                ftpRequest = ( FtpWebRequest ) FtpWebRequest.Create( host.EnsureTrailingForwardslash() + remoteFile );
                /* Log in to the FTP Server with the User Name and Password Provided */
                ftpRequest.Credentials = new NetworkCredential( user, pass );
                /* When in doubt, use these options */
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = false;
                /* Specify the Type of FTP Request */
                ftpRequest.Method = WebRequestMethods.Ftp.DownloadFile;
                /* Establish Return Communication with the FTP Server */
                ftpResponse = ( FtpWebResponse ) ftpRequest.GetResponse();
                /* Get the FTP Server's Response Stream */
                ftpStream = ftpResponse.GetResponseStream();
                /* Open a File Stream to Write the Downloaded File */
                FileStream localFileStream = new FileStream( localFile, FileMode.Create );
                /* Buffer for the Downloaded Data */
                byte[] byteBuffer = new byte[bufferSize];
                int bytesRead = ftpStream.Read( byteBuffer, 0, bufferSize );
                /* Download the File by Writing the Buffered Data Until the Transfer is Complete */
                try
                {
                    while ( bytesRead > 0 )
                    {
                        localFileStream.Write( byteBuffer, 0, bytesRead );
                        bytesRead = ftpStream.Read( byteBuffer, 0, bufferSize );
                    }
                }
                catch ( Exception ex ) { Console.WriteLine( ex.ToString() ); }
                /* Resource Cleanup */
                localFileStream.Close();
                ftpStream.Close();
                ftpResponse.Close();
                ftpRequest = null;
            }
            catch ( Exception ex ) { Console.WriteLine( ex.ToString() ); }
            return;
        }

        public Stream GetFTPResponseStream( string remoteFile )
        {
            try
            {
                /* Create an FTP Request */
                ftpRequest = ( FtpWebRequest ) FtpWebRequest.Create( host.EnsureTrailingForwardslash() + remoteFile );
                /* Log in to the FTP Server with the User Name and Password Provided */
                ftpRequest.Credentials = new NetworkCredential( user, pass );
                /* When in doubt, use these options */
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = false;
                /* Specify the Type of FTP Request */
                ftpRequest.Method = WebRequestMethods.Ftp.DownloadFile;
                /* Establish Return Communication with the FTP Server */
                ftpResponse = ( FtpWebResponse ) ftpRequest.GetResponse();
                /* Get the FTP Server's Response Stream */
                return ftpResponse.GetResponseStream();
            }
            catch ( Exception ex ) { Console.WriteLine( ex.ToString() ); }
            return new MemoryStream();
        }

        /* Upload File */

        public void Upload( string remoteFile, string localFile )
        {
            try
            {
                /* Create an FTP Request */
                ftpRequest = ( FtpWebRequest ) FtpWebRequest.Create( host.EnsureTrailingForwardslash() + remoteFile );
                /* Log in to the FTP Server with the User Name and Password Provided */
                ftpRequest.Credentials = new NetworkCredential( user, pass );
                /* When in doubt, use these options */
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = false;
                /* Specify the Type of FTP Request */
                ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;
                /* Establish Return Communication with the FTP Server */
                ftpStream = ftpRequest.GetRequestStream();
                /* Open a File Stream to Read the File for Upload */
                FileStream localFileStream = new FileStream( localFile, FileMode.Create );
                /* Buffer for the Downloaded Data */
                byte[] byteBuffer = new byte[bufferSize];
                int bytesSent = localFileStream.Read( byteBuffer, 0, bufferSize );
                /* Upload the File by Sending the Buffered Data Until the Transfer is Complete */
                try
                {
                    while ( bytesSent != 0 )
                    {
                        ftpStream.Write( byteBuffer, 0, bytesSent );
                        bytesSent = localFileStream.Read( byteBuffer, 0, bufferSize );
                    }
                }
                catch ( Exception ex ) { Console.WriteLine( ex.ToString() ); }
                /* Resource Cleanup */
                localFileStream.Close();
                ftpStream.Close();
                ftpRequest = null;
            }
            catch ( Exception ex ) { Console.WriteLine( ex.ToString() ); }
            return;
        }

        public void Upload( string remoteFile, Stream inputStream )
        {
            //try
            {
                /* Create an FTP Request */
                ftpRequest = ( FtpWebRequest ) FtpWebRequest.Create( host.EnsureTrailingForwardslash() + remoteFile );

                /* Log in to the FTP Server with the User Name and Password Provided */
                ftpRequest.Credentials = new NetworkCredential( user, pass );

                /* When in doubt, use these options */
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = false;

                /* Specify the Type of FTP Request */
                ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;

                /* Establish Return Communication with the FTP Server */
                ftpStream = ftpRequest.GetRequestStream();

                // Get size of uploaded file
                int nFileLen = inputStream.Length.ToString().AsInteger();

                // Allocate a buffer for reading of the file
                byte[] myData = new byte[nFileLen];

                // Read uploaded file from the Stream
                inputStream.Read( myData, 0, nFileLen );

                //Upload file
                ftpStream.Write( myData, 0, myData.Length );
                ftpStream.Close();

                ftpRequest = null;
            }
            //catch ( Exception ex ) { Console.WriteLine( ex.ToString() ); }
            return;
        }

        /* Delete File */

        public void Delete( string deleteFile )
        {
            //try
            {
                /* Create an FTP Request */
                ftpRequest = ( FtpWebRequest ) WebRequest.Create( host.EnsureTrailingForwardslash() + deleteFile );
                /* Log in to the FTP Server with the User Name and Password Provided */
                ftpRequest.Credentials = new NetworkCredential( user, pass );
                /* When in doubt, use these options */
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = false;
                /* Specify the Type of FTP Request */
                ftpRequest.Method = WebRequestMethods.Ftp.DeleteFile;
                /* Establish Return Communication with the FTP Server */
                ftpResponse = ( FtpWebResponse ) ftpRequest.GetResponse();
                /* Resource Cleanup */
                ftpResponse.Close();
                ftpRequest = null;
            }
            //catch ( Exception ex ) { Console.WriteLine( ex.ToString() ); }
            return;
        }

        /* Rename File */

        public void Rename( string currentFileNameAndPath, string newFileName )
        {
            try
            {
                /* Create an FTP Request */
                ftpRequest = ( FtpWebRequest ) WebRequest.Create( host.EnsureTrailingForwardslash() + currentFileNameAndPath );
                /* Log in to the FTP Server with the User Name and Password Provided */
                ftpRequest.Credentials = new NetworkCredential( user, pass );
                /* When in doubt, use these options */
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = false;
                /* Specify the Type of FTP Request */
                ftpRequest.Method = WebRequestMethods.Ftp.Rename;
                /* Rename the File */
                ftpRequest.RenameTo = newFileName;
                /* Establish Return Communication with the FTP Server */
                ftpResponse = ( FtpWebResponse ) ftpRequest.GetResponse();
                /* Resource Cleanup */
                ftpResponse.Close();
                ftpRequest = null;
            }
            catch ( Exception ex ) { Console.WriteLine( ex.ToString() ); }
            return;
        }

        /* Create a New Directory on the FTP Server */

        public void CreateDirectory( string newDirectory )
        {
            try
            {
                /* Create an FTP Request */
                ftpRequest = ( FtpWebRequest ) WebRequest.Create( host.EnsureTrailingForwardslash() + newDirectory );
                /* Log in to the FTP Server with the User Name and Password Provided */
                ftpRequest.Credentials = new NetworkCredential( user, pass );
                /* When in doubt, use these options */
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = false;
                /* Specify the Type of FTP Request */
                ftpRequest.Method = WebRequestMethods.Ftp.MakeDirectory;
                /* Establish Return Communication with the FTP Server */
                ftpResponse = ( FtpWebResponse ) ftpRequest.GetResponse();
                /* Resource Cleanup */
                ftpResponse.Close();
                ftpRequest = null;
            }
            catch ( Exception ex ) { Console.WriteLine( ex.ToString() ); }
            return;
        }

        /* Get the Date/Time a File was Created */

        public string GetFileCreatedDateTime( string fileName )
        {
            try
            {
                /* Create an FTP Request */
                ftpRequest = ( FtpWebRequest ) FtpWebRequest.Create( host.EnsureTrailingForwardslash() + fileName );
                /* Log in to the FTP Server with the User Name and Password Provided */
                ftpRequest.Credentials = new NetworkCredential( user, pass );
                /* When in doubt, use these options */
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = false;
                /* Specify the Type of FTP Request */
                ftpRequest.Method = WebRequestMethods.Ftp.GetDateTimestamp;
                /* Establish Return Communication with the FTP Server */
                ftpResponse = ( FtpWebResponse ) ftpRequest.GetResponse();
                /* Establish Return Communication with the FTP Server */
                ftpStream = ftpResponse.GetResponseStream();
                /* Get the FTP Server's Response Stream */
                StreamReader ftpReader = new StreamReader( ftpStream );
                /* Store the Raw Response */
                string fileInfo = null;
                /* Read the Full Response Stream */
                try
                { fileInfo = ftpReader.ReadToEnd(); }
                catch ( Exception ex ) { Console.WriteLine( ex.ToString() ); }
                /* Resource Cleanup */
                ftpReader.Close();
                ftpStream.Close();
                ftpResponse.Close();
                ftpRequest = null;
                /* Return File Created Date Time */
                return fileInfo;
            }
            catch ( Exception ex ) { Console.WriteLine( ex.ToString() ); }
            /* Return an Empty string Array if an Exception Occurs */
            return "";
        }

        /* Get the Size of a File */

        public long GetFileSize( string fileName )
        {
            try
            {
                /* Create an FTP Request */
                ftpRequest = ( FtpWebRequest ) FtpWebRequest.Create( host.EnsureTrailingForwardslash() + fileName );
                /* Log in to the FTP Server with the User Name and Password Provided */
                ftpRequest.Credentials = new NetworkCredential( user, pass );
                /* When in doubt, use these options */
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = false;
                /* Specify the Type of FTP Request */
                ftpRequest.Method = WebRequestMethods.Ftp.GetFileSize;
                /* Establish Return Communication with the FTP Server */
                ftpResponse = ( FtpWebResponse ) ftpRequest.GetResponse();

                ///* Establish Return Communication with the FTP Server */
                //ftpStream = ftpResponse.GetResponseStream();
                ///* Get the FTP Server's Response Stream */
                //StreamReader ftpReader = new StreamReader( ftpStream );
                ///* Store the Raw Response */

                long fileInfo = 0;
                if ( ftpResponse.ContentLength > 0 )
                {
                    fileInfo = ftpResponse.ContentLength;
                }
                //ftpReader.Close();
                //ftpStream.Close();
                ftpResponse.Close();
                ftpRequest = null;
                /* Return File Size */
                return fileInfo;
            }
            catch ( Exception ex ) { Console.WriteLine( ex.ToString() ); }
            /* Return an Empty string Array if an Exception Occurs */
            return 0;
        }

        /* List Directory Contents File/Folder Name Only */

        public string[] DirectoryListSimple( string directory )
        {
            try
            {
                /* Create an FTP Request */
                ftpRequest = ( FtpWebRequest ) FtpWebRequest.Create( host.EnsureTrailingForwardslash() + directory );
                /* Log in to the FTP Server with the User Name and Password Provided */
                ftpRequest.Credentials = new NetworkCredential( user, pass );
                /* When in doubt, use these options */
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = false;
                /* Specify the Type of FTP Request */
                ftpRequest.Method = WebRequestMethods.Ftp.ListDirectory;
                /* Establish Return Communication with the FTP Server */
                ftpResponse = ( FtpWebResponse ) ftpRequest.GetResponse();
                /* Establish Return Communication with the FTP Server */
                ftpStream = ftpResponse.GetResponseStream();
                /* Get the FTP Server's Response Stream */
                StreamReader ftpReader = new StreamReader( ftpStream );
                /* Store the Raw Response */
                string directoryRaw = null;
                /* Read Each Line of the Response and Append a Pipe to Each Line for Easy Parsing */
                try
                { while ( ftpReader.Peek() != -1 ) { directoryRaw += ftpReader.ReadLine() + "|"; } }
                catch ( Exception ex ) { Console.WriteLine( ex.ToString() ); }
                /* Resource Cleanup */
                ftpReader.Close();
                ftpStream.Close();
                ftpResponse.Close();
                ftpRequest = null;
                /* Return the Directory Listing as a string Array by Parsing 'directoryRaw' with the Delimiter you Append (I use | in This Example) */
                try
                { string[] directoryList = directoryRaw.Split( "|".ToCharArray() ); return directoryList; }
                catch ( Exception ex ) { Console.WriteLine( ex.ToString() ); }
            }
            catch ( Exception ex ) { Console.WriteLine( ex.ToString() ); }
            /* Return an Empty string Array if an Exception Occurs */
            return new string[] { "" };
        }

        /* List Directory Contents in Detail (Name, Size, Created, etc.) */

        public string[] DirectoryListDetailed( string directory )
        {
            try
            {
                /* Create an FTP Request */
                ftpRequest = ( FtpWebRequest ) FtpWebRequest.Create( host.EnsureTrailingForwardslash() + directory );
                /* Log in to the FTP Server with the User Name and Password Provided */
                ftpRequest.Credentials = new NetworkCredential( user, pass );
                /* When in doubt, use these options */
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = false;
                /* Specify the Type of FTP Request */
                ftpRequest.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                /* Establish Return Communication with the FTP Server */
                ftpResponse = ( FtpWebResponse ) ftpRequest.GetResponse();
                /* Establish Return Communication with the FTP Server */
                ftpStream = ftpResponse.GetResponseStream();
                /* Get the FTP Server's Response Stream */
                StreamReader ftpReader = new StreamReader( ftpStream );
                /* Store the Raw Response */
                string directoryRaw = null;
                /* Read Each Line of the Response and Append a Pipe to Each Line for Easy Parsing */
                try
                { while ( ftpReader.Peek() != -1 ) { directoryRaw += ftpReader.ReadLine() + "|"; } }
                catch ( Exception ex ) { Console.WriteLine( ex.ToString() ); }
                /* Resource Cleanup */
                ftpReader.Close();
                ftpStream.Close();
                ftpResponse.Close();
                ftpRequest = null;
                /* Return the Directory Listing as a string Array by Parsing 'directoryRaw' with the Delimiter you Append (I use | in This Example) */
                try
                { string[] directoryList = directoryRaw.Split( "|".ToCharArray() ); return directoryList; }
                catch ( Exception ex ) { Console.WriteLine( ex.ToString() ); }
            }
            catch ( Exception ex ) { Console.WriteLine( ex.ToString() ); }
            /* Return an Empty string Array if an Exception Occurs */
            return new string[] { "" };
        }
    }
}
