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
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

using Rock.Data;
using Rock.Model;
using Rock.Security;
using Rock.Web.Cache;

using rocks.kfs.Reporting.SQLReportingServices.API;

namespace rocks.kfs.Reporting.SQLReportingServices
{
    /// <summary>
    /// The Reporting Service Provider
    /// </summary>
    public class ReportingServicesProvider
    {
        #region Properties

        /// <summary>
        /// Gets or sets the server URL.
        /// </summary>
        /// <value>
        /// The server URL.
        /// </value>
        public string ServerUrl
        {
            get
            {
                return mServerUrl;
            }
            set
            {
                Uri result;
                if ( Uri.TryCreate( value, UriKind.Absolute, out result ) )
                {
                    mServerUrl = result.AbsoluteUri;
                }
                else
                {
                    mServerUrl = null;
                }
            }
        }

        public string ReportPath { get; set; }
        public string ContentManagerUser { get; set; }
        public string ContentManagerPassword { get; set; }
        public string BrowserUser { get; set; }
        public string BrowserPassword { get; set; }
        public bool CredentialsStored { get; set; }

        private string mServerUrl;
        private Guid ReportingServicesCategoryGuid = new Guid( "BE54A3EB-98F9-4BBE-86FD-A3F503CDADF6" );
        private const string SERVER_URL_KEY = "ReportingServiceURL";
        private const string SERVER_ROOT_PATH_KEY = "ReportingServiceRootPath";
        private const string CONTENT_MANAGER_USER_KEY = "ReportingServiceContentManagerUser";
        private const string CONTENT_MANAGER_PWD_KEY = "ReportingServiceContentMangerPwd";
        private const string BROWSER_USER_KEY = "ReportingServiceBrowserUser";
        private const string BROWSER_PWD_KEY = "ReportingServiceBrowserPwd";

        private ReportingService2010SoapClient rsClient = null;
        private UserType rsUserType;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportingServicesProvider"/> class.
        /// </summary>
        public ReportingServicesProvider()
        {
            LoadCredentials( new RockContext() );
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the browser credentials.
        /// </summary>
        /// <returns></returns>
        public ReportingServicesCredentials GetBrowserCredentials()
        {
            return new ReportingServicesCredentials( BrowserUser, BrowserPassword );
        }

        /// <summary>
        /// Gets the path.
        /// </summary>
        /// <param name="basePath">The base path.</param>
        /// <param name="listChildren">if set to <c>true</c> [list children].</param>
        /// <returns></returns>
        public Dictionary<string, string> GetPath( string basePath, bool listChildren )
        {
            var client = GetClient( UserType.Browser );
            CatalogItem[] catalogItems;

            if ( basePath == null )
            {
                basePath = ReportPath;
            }
            else if ( !basePath.StartsWith( ReportPath ) )
            {
                basePath = string.Concat( ReportPath, basePath );
            }

            client.ListChildren( null, basePath, listChildren, out catalogItems );

            return catalogItems.Where( c => c.TypeName == "Report" ).Where( c => !c.Hidden ).ToDictionary( f => f.Path, f => f.Name );
        }

        /// <summary>
        /// Saves the credentials.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public bool SaveCredentials( out string message )
        {
            if ( !TestConnection( out message, UserType.ContentManager ) )
            {
                return false;
            }

            if ( !String.IsNullOrWhiteSpace( BrowserUser ) && !BrowserUser.Equals( ContentManagerUser ) )
            {
                if ( !TestConnection( out message, UserType.Browser ) )
                {
                    return false;
                }
            }

            using ( RockContext context = new RockContext() )
            {
                int categoryId = GetReportingServicesCategory( context );
                if ( categoryId <= 0 )
                {
                    categoryId = CreateReportingServicesCategory( context );
                }

                VerifyAttributes( context, categoryId );

                SaveAttributeValue( context, SERVER_URL_KEY, ServerUrl );
                SaveAttributeValue( context, SERVER_ROOT_PATH_KEY, ReportPath );
                SaveAttributeValue( context, CONTENT_MANAGER_USER_KEY, ContentManagerUser );
                SaveAttributeValue( context, CONTENT_MANAGER_PWD_KEY, Encryption.EncryptString( ContentManagerPassword ) );
                SaveAttributeValue( context, BROWSER_USER_KEY, BrowserUser );
                SaveAttributeValue( context, BROWSER_PWD_KEY, Encryption.EncryptString( BrowserPassword ) );
            }
            GlobalAttributesCache.Clear();
            return true;
        }

        /// <summary>
        /// Tests the connection.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public bool TestConnection( out string message, UserType type )
        {
            bool isSuccessful = false;
            message = string.Empty;
            try
            {
                ReportingService2010SoapClient client = GetClient( UserType.ContentManager );

                Property[] userProperties = null;

                if ( TestPath() )
                {
                    message = "Succesfully connected.";
                    isSuccessful = true;
                }
                else
                {
                    message = "Connection Error";
                }
            }
            catch ( Exception ex )
            {
                bool caught = false;
                if ( ex.InnerException != null && ex.InnerException.GetType() == typeof( System.Net.WebException ) )
                {
                    System.Net.WebException webEx = ( System.Net.WebException ) ex.InnerException;
                    if ( ( ( System.Net.HttpWebResponse ) webEx.Response ).StatusCode == System.Net.HttpStatusCode.Unauthorized )
                    {
                        message = "User is not authorized.";
                        caught = true;
                    }
                }
                if ( !caught )
                {
                    message = string.Format( "An error has occurred when Loading Reporting Services. {0}", ex.Message );
                }
                isSuccessful = false;
            }

            return isSuccessful;
        }

        /// <summary>
        /// Tests the data source.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public bool TestDataSource( out string message )
        {
            message = String.Empty;
            string pathEnd = ReportPath.EndsWith( "/" ) ? String.Empty : "/";
            string dsPath = string.Concat( ReportPath, pathEnd, "DataSources/RockContext" );
            string itemType = String.Empty;
            var client = GetClient( UserType.ContentManager );

            client.GetItemType( null, dsPath, out itemType );

            if ( !itemType.Equals( "datasource", StringComparison.InvariantCultureIgnoreCase ) )
            {
                message = "Rock Context Data Source not found.";
                return false;
            }
            return true;
        }

        /// <summary>
        /// Tests the path.
        /// </summary>
        /// <returns></returns>
        public bool TestPath()
        {
            try
            {
                var client = GetClient( UserType.ContentManager );
                string itemType = String.Empty;

                client.GetItemType( null, ReportPath, out itemType );

                if ( itemType.Equals( "folder", StringComparison.InvariantCultureIgnoreCase ) )
                {
                    return true;
                }
                return false;
            }
            catch ( Exception ex )
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the folder path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public string GetFolderPath( string path )
        {
            if ( path.EndsWith( "/" ) )
            {
                path = path.Substring( 0, path.Length - 1 );
            }
            //Aready includes root path
            if ( path.StartsWith( ReportPath ) )
            {
                return path;
            }

            // no path provided
            if ( string.IsNullOrWhiteSpace( path ) )
            {
                return ReportPath;
            }

            return string.Concat( ReportPath, path );
        }

        #endregion

        /// <summary>
        /// Gets the API client.
        /// </summary>
        /// <param name="userType">Type of the user.</param>
        /// <returns></returns>
        internal ReportingService2010SoapClient GetAPIClient( UserType userType )
        {
            return GetClient( userType );
        }

        #region Private Methods

        /// <summary>
        /// Clears the properties.
        /// </summary>
        private void ClearProperties()
        {
            ServerUrl = null;
            ReportPath = null;
            ContentManagerUser = null;
            ContentManagerPassword = null;
            BrowserUser = null;
            BrowserPassword = null;
        }

        /// <summary>
        /// Gets the client.
        /// </summary>
        /// <param name="ut">The ut.</param>
        /// <returns></returns>
        private ReportingService2010SoapClient GetClient( UserType ut )
        {
            if ( rsClient != null && ut == rsUserType )
            {
                return rsClient;
            }
            else
            {
                rsClient = null;
            }
            var binding = new BasicHttpBinding();
            binding.Name = "ReportingServicesBinding";
            binding.Security.Mode = BasicHttpSecurityMode.TransportCredentialOnly;
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Ntlm;
            binding.Security.Transport.ProxyCredentialType = HttpProxyCredentialType.None;
            binding.Security.Transport.Realm = "";
            binding.Security.Message.ClientCredentialType = BasicHttpMessageCredentialType.UserName;
            binding.Security.Message.AlgorithmSuite = System.ServiceModel.Security.SecurityAlgorithmSuite.Default;

            string url = string.Format( "{0}/ReportService2010.asmx", ServerUrl );
            var endpoint = new EndpointAddress( url );

            rsClient = new ReportingService2010SoapClient( binding, endpoint );
            rsClient.ClientCredentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation;
            if ( ut == UserType.ContentManager )
            {
                rsClient.ClientCredentials.Windows.ClientCredential = new System.Net.NetworkCredential( ContentManagerUser, ContentManagerPassword );
            }
            else
            {
                rsClient.ClientCredentials.Windows.ClientCredential = new System.Net.NetworkCredential( BrowserUser, BrowserPassword );
            }
            rsUserType = ut;
            return rsClient;
        }

        /// <summary>
        /// Loads the credentials.
        /// </summary>
        /// <param name="context">The context.</param>
        private void LoadCredentials( RockContext context )
        {
            ClearProperties();
            GlobalAttributesCache cache = GlobalAttributesCache.Get();

            ServerUrl = cache.GetValue( SERVER_URL_KEY, context );
            ReportPath = cache.GetValue( SERVER_ROOT_PATH_KEY, context );
            ContentManagerUser = cache.GetValue( CONTENT_MANAGER_USER_KEY, context );
            ContentManagerPassword = Encryption.DecryptString( cache.GetValue( CONTENT_MANAGER_PWD_KEY, context ) );
            BrowserUser = cache.GetValue( BROWSER_USER_KEY, context );
            BrowserPassword = Encryption.DecryptString( cache.GetValue( BROWSER_PWD_KEY, context ) );

            if ( !String.IsNullOrWhiteSpace( ServerUrl ) && !String.IsNullOrWhiteSpace( ReportPath ) && !String.IsNullOrWhiteSpace( BrowserUser ) && !String.IsNullOrWhiteSpace( BrowserPassword ) )
            {
                CredentialsStored = true;
            }
        }

        /// <summary>
        /// Creates the reporting services category.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        private int CreateReportingServicesCategory( RockContext context )
        {
            var service = new CategoryService( context );
            Category category = new Category();
            category.EntityTypeId = EntityTypeCache.Get( typeof( Rock.Model.Attribute ) ).Id;
            category.EntityTypeQualifierColumn = "EntityTypeId";

            category.Order = new CategoryService( context ).Queryable()
                .Where( c => c.EntityTypeId == category.EntityTypeId )
                .Where( c => c.EntityTypeQualifierColumn == category.EntityTypeQualifierColumn )
                .OrderByDescending( c => c.Order )
                .Select( c => c.Order )
                .FirstOrDefault() + 1;
            category.Name = "Reporting Services";
            category.Guid = ReportingServicesCategoryGuid;
            category.Description = "Reporting Service configuration Settings";
            service.Add( category );
            context.SaveChanges();

            return category.Id;
        }

        /// <summary>
        /// Gets the reporting services category.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        private int GetReportingServicesCategory( RockContext context )
        {
            Category cat = new CategoryService( context ).Get( ReportingServicesCategoryGuid );

            if ( cat == null )
            {
                return -1;
            }
            else
            {
                return cat.Id;
            }
        }

        /// <summary>
        /// Saves the attribute value.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        private void SaveAttributeValue( RockContext context, string key, string value )
        {
            var attributeSvc = new AttributeService( context );
            var attribute = attributeSvc.GetGlobalAttribute( key );
            var attributeValueSvc = new AttributeValueService( context );
            var attributeValue = attributeValueSvc.GetByAttributeIdAndEntityId( attribute.Id, null );

            if ( attributeValue == null && !String.IsNullOrWhiteSpace( value ) )
            {
                attributeValue = new AttributeValue();
                attributeValue.AttributeId = attribute.Id;
                attributeValue.EntityId = null;
                attributeValueSvc.Add( attributeValue );
            }

            if ( attributeValue == null || value.Equals( attributeValue.Value ) )
            {
                return;
            }

            if ( String.IsNullOrWhiteSpace( value ) )
            {
                attributeValueSvc.Delete( attributeValue );
            }
            else
            {
                attributeValue.Value = value;
            }

            context.SaveChanges();
        }

        /// <summary>
        /// Verifies the attributes.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="categoryId">The category identifier.</param>
        private void VerifyAttributes( RockContext context, int categoryId )
        {
            var attributeService = new AttributeService( context );
            var category = new CategoryService( context ).Get( categoryId );
            bool hasChanges = false;

            var serverUrl = attributeService.GetGlobalAttribute( SERVER_URL_KEY );
            var serverRootPath = attributeService.GetGlobalAttribute( SERVER_ROOT_PATH_KEY );
            var contentManagerUser = attributeService.GetGlobalAttribute( CONTENT_MANAGER_USER_KEY );
            var contentManagerPwd = attributeService.GetGlobalAttribute( CONTENT_MANAGER_PWD_KEY );
            var browserUser = attributeService.GetGlobalAttribute( BROWSER_USER_KEY );
            var browserPwd = attributeService.GetGlobalAttribute( BROWSER_PWD_KEY );

            if ( serverUrl == null )
            {
                serverUrl = new Rock.Model.Attribute();
                serverUrl.FieldTypeId = FieldTypeCache.Get( Rock.SystemGuid.FieldType.URL_LINK ).Id;
                serverUrl.IsSystem = false;
                serverUrl.Name = "Reporting Service URL";
                serverUrl.Description = "URL to the SQL Reporting Services Reporting Server endpoint.";
                serverUrl.Key = SERVER_URL_KEY;
                serverUrl.IsRequired = false;
                serverUrl.AllowSearch = false;
                serverUrl.Categories.Add( category );
                attributeService.Add( serverUrl );
                hasChanges = true;
            }

            if ( serverRootPath == null )
            {
                serverRootPath = new Rock.Model.Attribute();
                serverRootPath.FieldTypeId = FieldTypeCache.Get( Rock.SystemGuid.FieldType.TEXT ).Id;
                serverRootPath.IsSystem = false;
                serverRootPath.Name = "Reporting Service Root Folder";
                serverRootPath.Key = SERVER_ROOT_PATH_KEY;
                serverRootPath.Description = "Root/Base folder for Rock reports in reporting services.";
                serverRootPath.IsRequired = false;
                serverRootPath.AllowSearch = false;
                serverRootPath.Categories.Add( category );
                attributeService.Add( serverRootPath );
                hasChanges = true;
            }

            if ( contentManagerUser == null )
            {
                contentManagerUser = new Rock.Model.Attribute();
                contentManagerUser.FieldTypeId = FieldTypeCache.Get( Rock.SystemGuid.FieldType.TEXT ).Id;
                contentManagerUser.Name = "Reporting Service - Content Manager Username";
                contentManagerUser.Key = CONTENT_MANAGER_USER_KEY;
                contentManagerUser.Description = "The Reporting Server Content Manager (Report Administrator) User Name. (i.e. domain\\user format)";
                contentManagerUser.IsRequired = false;
                contentManagerUser.AllowSearch = false;
                contentManagerUser.Categories.Add( category );
                attributeService.Add( contentManagerUser );
                hasChanges = true;
            }

            if ( contentManagerPwd == null )
            {
                contentManagerPwd = new Rock.Model.Attribute();
                contentManagerPwd.FieldTypeId = FieldTypeCache.Get( Rock.SystemGuid.FieldType.ENCRYPTED_TEXT ).Id;
                contentManagerPwd.Name = "Reporting Service - Content Manager Password";
                contentManagerPwd.Key = CONTENT_MANAGER_PWD_KEY;
                contentManagerPwd.Description = "The Content Manager Password.";
                contentManagerPwd.IsRequired = false;
                contentManagerPwd.AllowSearch = false;
                contentManagerPwd.Categories.Add( category );
                contentManagerPwd.AttributeQualifiers.Add( new AttributeQualifier { IsSystem = false, Key = "ispassword", Value = bool.TrueString } );
                attributeService.Add( contentManagerPwd );
                hasChanges = true;
            }

            if ( browserUser == null )
            {
                browserUser = new Rock.Model.Attribute();
                browserUser.FieldTypeId = FieldTypeCache.Get( Rock.SystemGuid.FieldType.ENCRYPTED_TEXT ).Id;
                browserUser.Name = "Reporting Service - Browser User";
                browserUser.Key = BROWSER_USER_KEY;
                browserUser.Description = "The Reporting Server Browser (Report Viewer) User Name. (i.e. domain\\user format)";
                browserUser.IsRequired = false;
                browserUser.AllowSearch = false;
                browserUser.Categories.Add( category );
                attributeService.Add( browserUser );
                hasChanges = true;
            }

            if ( browserPwd == null )
            {
                browserPwd = new Rock.Model.Attribute();
                browserPwd.FieldTypeId = FieldTypeCache.Get( Rock.SystemGuid.FieldType.ENCRYPTED_TEXT ).Id;
                browserPwd.Name = "Reporting Service - Browser Password";
                browserPwd.Key = BROWSER_PWD_KEY;
                browserPwd.Description = "The Reporting Server Browser Password.";
                browserPwd.IsRequired = false;
                browserPwd.AllowSearch = false;
                browserPwd.Categories.Add( category );
                browserPwd.AttributeQualifiers.Add( new AttributeQualifier { IsSystem = false, Key = "ispassword", Value = bool.TrueString } );
                attributeService.Add( browserPwd );
                hasChanges = true;
            }

            if ( hasChanges )
            {
                context.SaveChanges();
                GlobalAttributesCache.Clear();
            }
        }

        #endregion
    }
}
