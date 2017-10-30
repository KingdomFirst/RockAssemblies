using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Security;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using RestSharp;
using Rock;
using Rock.Attribute;
using Rock.Data;
using Rock.Model;
using Rock.Security;
using Rock.Web.Cache;

namespace com.kfs.Security.ExternalAuthentication
{
    /// <summary>
    /// Authenticates a user using a provided Doorkeeper OAuth Server
    /// </summary>
    [Description( "OAuth Authentication Provider for Doorkeeper" )]
    [Export( typeof( AuthenticationComponent ) )]
    [ExportMetadata( "ComponentName", "Doorkeeper OAuth" )]

    [TextField( "Client ID", "The Doorkeeper OAuth Client ID" )]
    [TextField( "Client Secret", "The Doorkeeper OAuth Client Secret" )]
    [UrlLinkField( "Doorkeeper Server Url", "The base Url of the local Doorkeeper server. Example: https://login.mychurch.org/" )]
    [UrlLinkField( "User Info Url", "The Url location of the 'me.json' data. Example: https://login.mychurch.org/api/v1/me.json" )]   /// https://www.googleapis.com/oauth2/v2/userinfo

    public class DoorkeeperOAuth : AuthenticationComponent
    {
        /// <summary>
        /// The _baseUrl
        /// </summary>
        private string _baseUrl = null;

        /// <summary>
        /// Gets the type of the service.
        /// </summary>
        /// <value>
        /// The type of the service.
        /// </value>
        public override AuthenticationServiceType ServiceType
        {
            get { return AuthenticationServiceType.External; }
        }

        /// <summary>
        /// Determines if user is directed to another site (i.e. Facebook, Gmail, Twitter, etc) to confirm approval of using
        /// that site's credentials for authentication.
        /// </summary>
        /// <value>
        /// The requires remote authentication.
        /// </value>
        public override bool RequiresRemoteAuthentication
        {
            get { return true; }
        }

        /// <summary>
        /// Tests the Http Request to determine if authentication should be tested by this
        /// authentication provider.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        public override Boolean IsReturningFromAuthentication( HttpRequest request )
        {
            return ( !String.IsNullOrWhiteSpace( request.QueryString["code"] ) &&
                !String.IsNullOrWhiteSpace( request.QueryString["state"] ) );
        }

        /// <summary>
        /// Generates the login URL.
        /// </summary>
        /// <param name="request">Forming the URL to obtain user consent</param>
        /// <returns></returns>
        public override Uri GenerateLoginUrl( HttpRequest request )
        {
            _baseUrl = GetAttributeValue( "DoorkeeperServerUrl" );
            if ( !_baseUrl.EndsWith( "/" ) && _baseUrl != "" )
            {
                _baseUrl = _baseUrl + "/";
            }

            string returnUrl = request.QueryString["returnurl"];
            string redirectUri = GetRedirectUrl( request );

            return new Uri( string.Format( "{0}oauth/authorize?client_id={1}&response_type=code&redirect_uri={2}&state={3}",
                _baseUrl,
                GetAttributeValue( "ClientID" ),
                HttpUtility.UrlEncode( redirectUri ),
                HttpUtility.UrlEncode( returnUrl ?? FormsAuthentication.DefaultUrl ) ) );
        }

        ///<summary>
        ///JSON Class for Access Token Response
        ///</summary>
        public class accesstokenresponse
        {
            /// <summary>
            /// Gets or sets the access_token.
            /// </summary>
            /// <value>
            /// The access_token.
            /// </value>
            public string access_token { get; set; }

            /// <summary>
            /// Gets or sets the expires_in.
            /// </summary>
            /// <value>
            /// The expires_in.
            /// </value>
            public int expires_in { get; set; }

            /// <summary>
            /// Gets or sets the token_type.
            /// </summary>
            /// <value>
            /// The token_type.
            /// </value>
            public string token_type { get; set; }
        }

        /// <summary>
        /// Authenticates the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="username">The username.</param>
        /// <param name="returnUrl">The return URL.</param>
        /// <returns></returns>
        public override Boolean Authenticate( HttpRequest request, out string username, out string returnUrl )
        {
            username = string.Empty;
            returnUrl = request.QueryString["State"];
            string redirectUri = GetRedirectUrl( request );

            try
            {
                // Get a new OAuth Access Token for the 'code' that was returned from the OAuth user consent redirect
                var restClient = new RestClient(
                    string.Format( "{0}oauth/token?grant_type=authorization_code&code={1}&redirect_uri={2}&client_id={3}&client_secret={4}",
                        _baseUrl,
                        request.QueryString["code"],
                        HttpUtility.UrlEncode( redirectUri ),
                        GetAttributeValue( "ClientID" ),
                        GetAttributeValue( "ClientSecret" ) ) );
                var restRequest = new RestRequest( Method.POST );
                var restResponse = restClient.Execute( restRequest );

                if ( restResponse.StatusCode == HttpStatusCode.OK )
                {
                    var accesstokenresponse = JsonConvert.DeserializeObject<accesstokenresponse>( restResponse.Content );
                    string accessToken = accesstokenresponse.access_token;

                    // Get information about the person who logged in using OAuth
                    restRequest = new RestRequest( Method.GET );
                    restRequest.AddParameter( "access_token", accessToken );
                    restRequest.AddParameter( "fields", "id,first_name,last_name,email" );
                    restRequest.AddParameter( "key", GetAttributeValue( "ClientID" ) );
                    restRequest.RequestFormat = DataFormat.Json;
                    restRequest.AddHeader( "Accept", "application/json" );
                    restClient = new RestClient( GetAttributeValue( "UserInfoUrl" ) );
                    restResponse = restClient.Execute( restRequest );

                    if ( restResponse.StatusCode == HttpStatusCode.OK )
                    {
                        OAuthUser oauthUser = JsonConvert.DeserializeObject<OAuthUser>( restResponse.Content );
                        username = GetOAuthUser( oauthUser, accessToken );
                    }
                }
            }

            catch ( Exception ex )
            {
                ExceptionLogService.LogException( ex, HttpContext.Current );
            }

            return !string.IsNullOrWhiteSpace( username );
        }

        /// <summary>
        /// Gets the URL of an image that should be displayed.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override String ImageUrl()
        {
            return ""; /*~/Assets/Images/facebook-login.png*/
        }

        private string GetRedirectUrl( HttpRequest request )
        {
            Uri uri = new Uri( request.Url.ToString() );
            return uri.Scheme + "://" + uri.GetComponents( UriComponents.HostAndPort, UriFormat.UriEscaped ) + uri.LocalPath;
        }

        /// <summary>
        /// Authenticates the user based on user name and password
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override bool Authenticate( UserLogin user, string password )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Encodes the password.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override string EncodePassword( UserLogin user, string password )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a value indicating whether [supports change password].
        /// </summary>
        /// <value>
        /// <c>true</c> if [supports change password]; otherwise, <c>false</c>.
        /// </value>
        public override bool SupportsChangePassword
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Changes the password.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="oldPassword">The old password.</param>
        /// <param name="newPassword">The new password.</param>
        /// <param name="warningMessage">The warning message.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override bool ChangePassword( UserLogin user, string oldPassword, string newPassword, out string warningMessage )
        {
            warningMessage = "not supported";
            return false;
        }

        /// <summary>
        /// Sets the password.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="password">The password.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public override void SetPassword( UserLogin user, string password )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// OAuth User Object
        /// </summary>
        public class OAuthUser
        {
            /// <summary>
            /// Gets or sets the identifier.
            /// </summary>
            /// <value>
            /// The identifier.
            /// </value>
            public string id { get; set; }

            /// <summary>
            /// Gets or sets the name.
            /// </summary>
            /// <value>
            /// The name.
            /// </value>
            public string first_name { get; set; }

            /// <summary>
            /// Gets or sets the family_name.
            /// </summary>
            /// <value>
            /// The family_name.
            /// </value>
            public string last_name { get; set; }
            
            /// <summary>
            /// Gets or sets the email.
            /// </summary>
            /// <value>
            /// The email.
            /// </value>
            public string email { get; set; }
        }

        /// <summary>
        /// Gets the name of the OAuth user.
        /// </summary>
        /// <param name="oauthUser">The OAuth user.</param>
        /// <param name="accessToken">The access token.</param>
        /// <returns></returns>
        public static string GetOAuthUser( OAuthUser oauthUser, string accessToken = "" )
        {
            string username = string.Empty;
            string oauthId = oauthUser.id;

            string userName = "OAuth_" + oauthId;
            UserLogin user = null;

            using ( var rockContext = new RockContext() )
            {

                // Query for an existing user 
                var userLoginService = new UserLoginService( rockContext );
                user = userLoginService.GetByUserName( userName );

                // If no user was found, see if we can find a match in the person table
                if ( user == null )
                {
                    // Get name/email from OAuth login
                    string lastName = oauthUser.last_name.ToString();
                    string firstName = oauthUser.first_name.ToString();
                    string email = string.Empty;
                    try
                    { email = oauthUser.email.ToString(); }
                    catch { }

                    Person person = null;

                    // If person had an email, get the first person with the same name and email address.
                    if ( !string.IsNullOrWhiteSpace( email ) )
                    {
                        var personService = new PersonService( rockContext );
                        var people = personService.GetByMatch( firstName, lastName, email );
                        if ( people.Count() == 1 )
                        {
                            person = people.First();
                        }
                    }

                    var personRecordTypeId = DefinedValueCache.Read( Rock.SystemGuid.DefinedValue.PERSON_RECORD_TYPE_PERSON.AsGuid() ).Id;
                    var personStatusPending = DefinedValueCache.Read( Rock.SystemGuid.DefinedValue.PERSON_RECORD_STATUS_PENDING.AsGuid() ).Id;

                    rockContext.WrapTransaction( () =>
                    {
                        if ( person == null )
                        {
                            person = new Person();
                            person.IsSystem = false;
                            person.RecordTypeValueId = personRecordTypeId;
                            person.RecordStatusValueId = personStatusPending;
                            person.FirstName = firstName;
                            person.LastName = lastName;
                            person.Email = email;
                            person.IsEmailActive = true;
                            person.EmailPreference = EmailPreference.EmailAllowed;
                            person.Gender = Gender.Unknown;
                            
                            if ( person != null )
                            {
                                PersonService.SaveNewPerson( person, rockContext, null, false );
                            }
                        }

                        if ( person != null )
                        {
                            int typeId = EntityTypeCache.Read( typeof( DoorkeeperOAuth ) ).Id;
                            user = UserLoginService.Create( rockContext, person, AuthenticationServiceType.External, typeId, userName, "goog", true );
                        }

                    } );
                }
                if ( user != null )
                {
                    username = user.UserName;

                    if ( user.PersonId.HasValue )
                    {
                        var converter = new ExpandoObjectConverter();

                        var personService = new PersonService( rockContext );
                        var person = personService.Get( user.PersonId.Value );
                    }
                }

                return username;
            }
        }
    }
}