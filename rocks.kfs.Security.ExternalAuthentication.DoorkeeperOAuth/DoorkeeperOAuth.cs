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
using System.ComponentModel;
using System.ComponentModel.Composition;
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
using Rock.Field.Types;
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
    [UrlLinkField( "User Info Url", "The Url location of the 'me.json' data. Example: https://login.mychurch.org/api/v1/me.json" )]
    [DefinedValueField( "2E6540EA-63F0-40FE-BE50-F2A84735E600", "Connection Status", "The connection status to use for new individuals (default: 'Web Prospect'.)", true, false, "368DD475-242C-49C4-A42C-7278BE690CC2" )]
    [BooleanField( "Enable Logging", "Enable logging for Doorkeeper OAuth methods from this provider.", false )]
    [KeyValueListField( "Weglot Language Hosts", "Add support for Weglot specific language headers to host mapping. Key = language code, i.e. 'es', Value = Host, i.e. 'es.example.com'", false, "", "Language Code", "Host" )]
    public class DoorkeeperOAuth : AuthenticationComponent
    {
        /// <summary>
        /// The _baseUrl
        /// </summary>
        private string _baseUrl = null;

        /// <summary>
        /// The _enableLogging
        /// </summary>
        private bool _enableLogging = false;

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
            _enableLogging = GetAttributeValue( "EnableLogging" ).AsBoolean();
            _baseUrl = GetAttributeValue( "DoorkeeperServerUrl" );
            if ( !_baseUrl.EndsWith( "/" ) && _baseUrl != "" )
            {
                _baseUrl = _baseUrl + "/";
            }

            string returnUrl = request.QueryString["returnurl"];
            string redirectUri = GetRedirectUrl( request );

            if ( _enableLogging )
            {
                using ( var rockContext = new RockContext() )
                {
                    LogEvent( rockContext, "GenerateLoginUrl", "_baseUrl", _baseUrl );
                    LogEvent( rockContext, "GenerateLoginUrl", "returnUrl", returnUrl ?? FormsAuthentication.DefaultUrl );
                    LogEvent( rockContext, "GenerateLoginUrl", "redirectUri", redirectUri );

                    var serverVariableString = "";
                    foreach ( string x in request.ServerVariables )
                    {
                        serverVariableString += x.ToString() + " : ";
                        serverVariableString += request.ServerVariables[x].ToString() + "<br/>";
                    }
                    LogEvent( rockContext, "GenerateLoginUrl", serverVariableString, "ServerVariables" );
                }
            }

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
                    restRequest.AddParameter( "fields", "id,first_name,last_name,email,contact,address" );
                    restRequest.AddParameter( "key", GetAttributeValue( "ClientID" ) );
                    restRequest.RequestFormat = DataFormat.Json;
                    restRequest.AddHeader( "Accept", "application/json" );
                    restClient = new RestClient( GetAttributeValue( "UserInfoUrl" ) );
                    restResponse = restClient.Execute( restRequest );

                    if ( restResponse.StatusCode == HttpStatusCode.OK )
                    {
                        OAuthUser oauthUser = JsonConvert.DeserializeObject<OAuthUser>( restResponse.Content );
                        username = GetOAuthUser( GetAttributeValue( "ConnectionStatus" ).AsGuid(), oauthUser, accessToken );
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
            // ProxySafe method pulls host from x-forwarded-host and x-forwarded-proto
            Uri uri = new Uri( request.UrlProxySafe().ToString() );

            // Support for forwarding hosts such as ngrok adds x-original-host
            var originalRequest = request.ServerVariables["HTTP_X_ORIGINAL_HOST"];
            if ( originalRequest.IsNotNullOrWhiteSpace() && !uri.ToString().Contains( originalRequest ) )
            {
                if ( !originalRequest.Contains( "http" ) )
                {
                    originalRequest = uri.Scheme + "://" + originalRequest;
                }
                uri = new Uri( originalRequest );
            }

            // Currently Weglot only adds a single header to the request that we can identify. Find the language and map it to a setting to get the proper host.
            var weglotLanguage = request.ServerVariables["HTTP_WEGLOT_LANGUAGE"];
            var weglotLanguageHosts = new KeyValueListFieldType().GetValuesFromString( null, GetAttributeValue( "WeglotLanguageHosts" ), null, false );
            if ( weglotLanguageHosts.Any( l => l.Key == weglotLanguage ) )
            {
                uri = new Uri( weglotLanguageHosts.Where( l => l.Key == weglotLanguage ).Select( l => l.Value ).FirstOrDefault().ToString() );
            }

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

            public OAuthContact contact { get; set; }

            public OAuthAddress address { get; set; }
        }

        public class OAuthContact
        {
            public string first_name { get; set; }          // "first_name": "First",
            public string last_name { get; set; }           // "last_name": "Last",
            public string email { get; set; }               // "email": "user1@example.org",
            public string phone { get; set; }               // "phone": "123-456-7890",
            public string birthday { get; set; }            // "birthday": "2015-11-09",
            public string marital_status_id { get; set; }   // "marital_status_id": 1,
            public string gender_id { get; set; }           // "gender_id": 1                   [ 1 = male; 2 = female ]
        }

        public class OAuthAddress
        {
            public string street1 { get; set; }             // "street1": "123 MyAwesome St",
            public string street2 { get; set; }             // "street2": null,
            public string city { get; set; }                // "city": "Dallas",
            public string state { get; set; }               // "state": "TX",
            public string zip { get; set; }                 // "zip": "12345",
            public string country { get; set; } = "US";     // "country": "US"
            public string latitude { get; set; }            // "latitude": 1.5,
            public string longitude { get; set; }           // "longitude": 1.5
        }

        /// <summary>
        /// Gets the name of the OAuth user.
        /// </summary>
        /// <param name="oauthUser">The OAuth user.</param>
        /// <param name="accessToken">The access token.</param>
        /// <returns></returns>
        public static string GetOAuthUser( Guid connectionStatusGuid, OAuthUser oauthUser, string accessToken = "" )
        {
            // accessToken is required
            if ( accessToken.IsNullOrWhiteSpace() )
            {
                return null;
            }

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
                        person = personService.FindPerson( firstName, lastName, email, true );
                    }

                    var personRecordTypeId = DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.PERSON_RECORD_TYPE_PERSON.AsGuid() ).Id;
                    var personStatusPendingId = DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.PERSON_RECORD_STATUS_PENDING.AsGuid() ).Id;
                    var personConnectionStatusId = DefinedValueCache.Get( connectionStatusGuid ).Id;
                    var phoneNumberTypeId = DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.PERSON_PHONE_TYPE_HOME.AsGuid() ).Id;

                    rockContext.WrapTransaction( () =>
                    {
                        if ( person == null )
                        {
                            person = new Person();
                            person.IsSystem = false;
                            person.RecordTypeValueId = personRecordTypeId;
                            person.RecordStatusValueId = personStatusPendingId;
                            person.ConnectionStatusValueId = personConnectionStatusId;
                            person.FirstName = firstName;
                            person.LastName = lastName;
                            person.Email = email;
                            person.IsEmailActive = true;
                            person.EmailPreference = EmailPreference.EmailAllowed;
                            person.Gender = Gender.Unknown;

                            var phoneNumber = new PhoneNumber { NumberTypeValueId = phoneNumberTypeId };
                            person.PhoneNumbers.Add( phoneNumber );
                            phoneNumber.Number = PhoneNumber.CleanNumber( oauthUser.contact.phone.AsNumeric() );

                            var birthday = oauthUser.contact.birthday.Split( ( new char[] { '-' } ) );
                            if ( birthday.Length == 3 )
                            {
                                person.BirthYear = birthday[0].AsIntegerOrNull();
                                person.BirthMonth = birthday[1].AsIntegerOrNull();
                                person.BirthDay = birthday[2].AsIntegerOrNull();
                            }

                            var gender = oauthUser.contact.gender_id.AsIntegerOrNull();
                            if ( gender != null )
                            {
                                person.Gender = ( Gender ) gender;
                            }

                            if ( person != null )
                            {
                                PersonService.SaveNewPerson( person, rockContext, null, false );
                            }

                            // save address
                            var personLocation = new LocationService( rockContext )
                                                    .Get( oauthUser.address.street1, oauthUser.address.street2,
                                                        oauthUser.address.city, oauthUser.address.state, oauthUser.address.zip, oauthUser.address.country );
                            if ( personLocation != null )
                            {
                                Guid locationTypeGuid = Rock.SystemGuid.DefinedValue.GROUP_LOCATION_TYPE_HOME.AsGuid();
                                if ( locationTypeGuid != Guid.Empty )
                                {
                                    Guid familyGroupTypeGuid = Rock.SystemGuid.GroupType.GROUPTYPE_FAMILY.AsGuid();
                                    GroupService groupService = new GroupService( rockContext );
                                    GroupLocationService groupLocationService = new GroupLocationService( rockContext );
                                    var family = groupService.Queryable().Where( g => g.GroupType.Guid == familyGroupTypeGuid && g.Members.Any( m => m.PersonId == person.Id ) ).FirstOrDefault();

                                    var groupLocation = new GroupLocation();
                                    groupLocation.GroupId = family.Id;
                                    groupLocationService.Add( groupLocation );

                                    groupLocation.Location = personLocation;

                                    groupLocation.GroupLocationTypeValueId = DefinedValueCache.Get( locationTypeGuid ).Id;
                                    groupLocation.IsMailingLocation = true;
                                    groupLocation.IsMappedLocation = true;

                                    rockContext.SaveChanges();
                                }
                            }
                        }

                        if ( person != null )
                        {
                            int typeId = EntityTypeCache.Get( typeof( DoorkeeperOAuth ) ).Id;
                            user = UserLoginService.Create( rockContext, person, AuthenticationServiceType.External, typeId, userName, "KFSRocksRock", true );
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

        private static ServiceLog LogEvent( RockContext rockContext, string type, string input, string result )
        {
            if ( rockContext == null )
            {
                rockContext = new RockContext();
            }
            var rockLogger = new ServiceLogService( rockContext );
            ServiceLog serviceLog = new ServiceLog
            {
                Name = "DoorkeeperOAuth",
                Type = type,
                LogDateTime = RockDateTime.Now,
                Input = input,
                Result = result,
                Success = true
            };
            rockLogger.Add( serviceLog );
            rockContext.SaveChanges();
            return serviceLog;
        }
    }
}
