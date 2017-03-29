using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

using com.kfs.Reporting.SQLReportingServices.API;

using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;

namespace com.kfs.Reporting.SQLReportingServices
{
    public class ReportingServicesProvider
    {
        #region Properties
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
        public ReportingServicesProvider()
        {
            LoadCredentials( new RockContext() );
        }
        #endregion

        #region Public Methods



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
                SaveAttributeValue( context, CONTENT_MANAGER_PWD_KEY, ContentManagerPassword );
                SaveAttributeValue( context, BROWSER_USER_KEY, BrowserUser );
                SaveAttributeValue( context, BROWSER_PWD_KEY, BrowserPassword );


            }

            return true;

        }

        public bool TestConnection( out string message, UserType type )
        {
            bool isSuccessful = false;
            message = string.Empty;
            try
            {
                ReportingService2010SoapClient client = GetClient( UserType.ContentManager );
  
                Property[] userProperties = null;
                var header = client.GetUserSettings( null, null, out userProperties );

                if ( header != null && DateTime.Parse( header.ReportServerDateTime ) > DateTime.MinValue )
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
                    System.Net.WebException webEx = ( System.Net.WebException )ex.InnerException;
                    if ( ( ( System.Net.HttpWebResponse )webEx.Response ).StatusCode == System.Net.HttpStatusCode.Unauthorized )
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

        public bool TestDataSource(out string message)
        {
            message = String.Empty;
            string pathEnd = ReportPath.EndsWith("/") ? String.Empty : "/";
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
            catch ( Exception ex  )
            {
                return false;
            }
        }

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

        internal ReportingService2010SoapClient GetAPIClient( UserType userType )
        {
            return GetClient( userType );
        }

        #region Private Methods

        private void ClearProperties()
        {
            ServerUrl = null;
            ReportPath = null;
            ContentManagerUser = null;
            ContentManagerPassword = null;
            BrowserUser = null;
            BrowserPassword = null;
        }

        private ReportingService2010SoapClient GetClient( UserType ut )
        {

            if ( rsClient != null && ut ==  rsUserType)
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

        private void LoadCredentials( RockContext context )
        {

            ClearProperties();
            GlobalAttributesCache cache = GlobalAttributesCache.Read();

            ServerUrl = cache.GetValue( SERVER_URL_KEY, context );
            ReportPath = cache.GetValue( SERVER_ROOT_PATH_KEY, context );
            ContentManagerUser = cache.GetValue( CONTENT_MANAGER_USER_KEY, context );
            ContentManagerPassword = cache.GetValue( CONTENT_MANAGER_PWD_KEY, context );
            BrowserUser = cache.GetValue( BROWSER_USER_KEY, context );
            BrowserPassword = cache.GetValue( BROWSER_PWD_KEY, context );

            if ( !String.IsNullOrWhiteSpace( ServerUrl ) && !String.IsNullOrWhiteSpace( ReportPath ) && !String.IsNullOrWhiteSpace( BrowserUser ) && !String.IsNullOrWhiteSpace( BrowserPassword ) )
            {
                CredentialsStored = true;
            }

        }

        private int CreateReportingServicesCategory( RockContext context )
        {
            var service = new CategoryService( context );
            Category category = new Category();
            category.EntityTypeId = EntityTypeCache.Read( typeof( Rock.Model.Attribute ) ).Id;
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
                serverUrl.FieldTypeId = FieldTypeCache.Read( Rock.SystemGuid.FieldType.URL_LINK ).Id;
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
                serverRootPath.FieldTypeId = FieldTypeCache.Read( Rock.SystemGuid.FieldType.TEXT ).Id;
                serverRootPath.IsSystem = false;
                serverRootPath.Name = "Reporting Service Root Folder.";
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
                contentManagerUser.FieldTypeId = FieldTypeCache.Read( Rock.SystemGuid.FieldType.TEXT ).Id;
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
                contentManagerPwd.FieldTypeId = FieldTypeCache.Read( Rock.SystemGuid.FieldType.ENCRYPTED_TEXT ).Id;
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
                browserUser.FieldTypeId = FieldTypeCache.Read( Rock.SystemGuid.FieldType.TEXT ).Id;
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
                browserPwd.FieldTypeId = FieldTypeCache.Read( Rock.SystemGuid.FieldType.ENCRYPTED_TEXT ).Id;
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
                GlobalAttributesCache.Flush();
            }

        }

        #endregion


    }
}
