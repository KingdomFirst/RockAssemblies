using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Text;

using Rock.Data;
using Rock.Web.Cache;
using Rock.Model;
using Rock.Security;

namespace com.kfs.AdobeSign.RestAPI
{
    public class AdobeSignClient
    {
        public AdobeSignClient() { }

        #region Public Methods

        public bool RequestAccessToken( string authCode, string redirectUri )
        {
            var apiAccessPoint = string.Empty;
            var clientId = string.Empty;
            var clientSecret = string.Empty;

            using ( var rockContext = new RockContext() )
            {
                var settings = GetSettings( rockContext );
                if ( settings != null )
                {
                    apiAccessPoint = GetSettingValue( settings, "AdobeSignAPIAccessPoint" );
                    clientId = GetSettingValue( settings, "AdobeSignClientId" );
                    clientSecret = GetSettingValue( settings, "AdobeSignClientSecret" );
                }
            }

            if ( string.IsNullOrWhiteSpace( authCode ) )
            {
                throw new System.Exception( "Adobe Sign Authorizaton Code is requred request Access Tokens." );
            }

            if ( string.IsNullOrWhiteSpace( redirectUri ) )
            {
                throw new System.Exception( "Referring Redirect URI is required." );
            }

            string requestUrl = apiAccessPoint + "oauth/token";
            string jsonResponse = null;
            using ( WebClient client = new WebClient() )
            {
                byte[] response = client.UploadValues( requestUrl, new System.Collections.Specialized.NameValueCollection()
                {
                    { "grant_type", "authorization_code" },
                    { "client_id", clientId },
                    { "client_secret", clientSecret },
                    { "redirect_uri", redirectUri },
                    { "code", authCode }
                } );
                jsonResponse = System.Text.Encoding.UTF8.GetString( response );
            }

            AccessTokenResponse tokenResponse = new AccessTokenResponse( jsonResponse );

            if ( !string.IsNullOrWhiteSpace( jsonResponse ) )
            {
                using ( var rockContext = new RockContext() )
                {
                    var settings = GetSettings( rockContext );

                    SetSettingValue( rockContext, settings, "AdobeSignAccessToken", tokenResponse.AccessToken );
                    SetSettingValue( rockContext, settings, "AdobeSignRefreshToken", tokenResponse.RefreshToken );
                    SetSettingValue( rockContext, settings, "AdobeSignAccessTokenExpiration", DateTime.Now.AddSeconds( tokenResponse.ExpiresIn ).ToString() );

                    rockContext.SaveChanges();
                }
            }

            return true;
        }

        public bool RefreshAccessToken()
        {
            var apiAccessPoint = string.Empty;
            var clientId = string.Empty;
            var clientSecret = string.Empty;
            var refreshToken = string.Empty;

            using ( var rockContext = new RockContext() )
            {
                var settings = GetSettings( rockContext );
                if ( settings != null )
                {
                    apiAccessPoint = GetSettingValue( settings, "AdobeSignAPIAccessPoint" );
                    clientId = GetSettingValue( settings, "AdobeSignClientId" );
                    clientSecret = GetSettingValue( settings, "AdobeSignClientSecret" );
                    refreshToken = GetSettingValue( settings, "AdobeSignRefreshToken" );
                }
            }

            string url = apiAccessPoint + "oauth/refresh";
            string jsonResponse = null;
            using ( WebClient client = new WebClient() )
            {
                byte[] response = client.UploadValues( url, new System.Collections.Specialized.NameValueCollection()
                {
                    {"grant_type", "refresh_token" },
                    {"client_id", clientId },
                    {"client_secret", clientSecret },
                    {"refresh_token", refreshToken }
                } );

                jsonResponse = System.Text.Encoding.UTF8.GetString( response );
            }

            AccessTokenResponse tokenResponse = new AccessTokenResponse( jsonResponse );

            if ( !string.IsNullOrWhiteSpace( jsonResponse ) )
            {
                using ( var rockContext = new RockContext() )
                {
                    var settings = GetSettings( rockContext );

                    SetSettingValue( rockContext, settings, "AdobeSignAccessToken", tokenResponse.AccessToken );
                    SetSettingValue( rockContext, settings, "AdobeSignAccessTokenExpiration", DateTime.Now.AddSeconds( tokenResponse.ExpiresIn ).ToString() );

                    rockContext.SaveChanges();
                }
            }

            return false;
        }

        public bool RevokeToken()
        {
            var apiAccessPoint = string.Empty;
            var clientId = string.Empty;
            var clientSecret = string.Empty;
            var refreshToken = string.Empty;

            using ( var rockContext = new RockContext() )
            {
                var settings = GetSettings( rockContext );
                if ( settings != null )
                {
                    apiAccessPoint = GetSettingValue( settings, "AdobeSignAPIAccessPoint" );
                    clientId = GetSettingValue( settings, "AdobeSignClientId" );
                    clientSecret = GetSettingValue( settings, "AdobeSignClientSecret" );
                    refreshToken = GetSettingValue( settings, "AdobeSignRefreshToken" );
                }
            }

            string url = string.Format( "{0}oauth/revoke?token={1}", apiAccessPoint, System.Web.HttpUtility.UrlEncode( refreshToken ) );

            WebRequest request = WebRequest.Create( url );
            request.Method = "POST";
            request.ContentType = @"application/x-www-form-urlencoded";

            HttpWebResponse resp = ( HttpWebResponse ) request.GetResponse();
            if ( resp.StatusCode == HttpStatusCode.OK )
            {
                using ( var rockContext = new RockContext() )
                {
                    var settings = GetSettings( rockContext );

                    SetSettingValue( rockContext, settings, "AdobeSignAccessToken", string.Empty );
                    SetSettingValue( rockContext, settings, "AdobeSignRefreshToken", string.Empty );
                    SetSettingValue( rockContext, settings, "AdobeSignAccessTokenExpiration", DateTime.MinValue.ToString() );

                    rockContext.SaveChanges();
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        public RawResponse SendRequest( string path, string method )
        {
            return SendRequest( path, method, null, null, null );
        }

        public RawResponse SendRequest( string path, string method, Dictionary<string, string> qsValues, Dictionary<string, string> headerValues, string body )
        {
            var apiAccessPoint = string.Empty;
            var clientId = string.Empty;
            var clientSecret = string.Empty;
            var accessToken = string.Empty;
            var tokenExpiration = DateTime.MinValue;

            using ( var rockContext = new RockContext() )
            {
                var settings = GetSettings( rockContext );
                if ( settings != null )
                {
                    apiAccessPoint = GetSettingValue( settings, "AdobeSignAPIAccessPoint" );
                    clientId = GetSettingValue( settings, "AdobeSignClientId" );
                    clientSecret = GetSettingValue( settings, "AdobeSignClientSecret" );
                    accessToken = GetSettingValue( settings, "AdobeSignAccessToken" );
                    tokenExpiration = Convert.ToDateTime( GetSettingValue( settings, "AdobeSignAccessTokenExpiration" ) );
                }
            }

            if ( tokenExpiration > DateTime.MinValue && tokenExpiration < DateTime.Now.AddSeconds( 30 ) )
            {
                RefreshAccessToken();
            }
            StringBuilder queryStringBuilder = new StringBuilder();
            if ( qsValues != null )
            {
                foreach ( var qsKVP in qsValues )
                {
                    queryStringBuilder.AppendFormat( "&{0}={1}", qsKVP.Key, qsKVP.Value );
                }
            }

            string queryString = String.Empty;
            if ( !string.IsNullOrWhiteSpace( queryStringBuilder.ToString() ) )
            {
                queryString = "?" + queryStringBuilder.ToString().Substring( 1 );
            }
            string apiRoot = "api/rest/v5/";
            string url = string.Concat( apiAccessPoint, apiRoot, path, queryString );

            HttpWebRequest request = ( HttpWebRequest ) WebRequest.Create( url );
            request.Method = method;
            request.ContentType = "application/json";

            request.Headers.Add( "Access-Token", accessToken );
            if ( headerValues != null )
            {
                foreach ( var headerKVP in headerValues )
                {
                    request.Headers.Add( headerKVP.Key, headerKVP.Value );
                }
            }

            if ( !string.IsNullOrWhiteSpace( body ) )
            {
                Byte[] byteArray = System.Text.Encoding.UTF8.GetBytes( body );
                request.ContentLength = byteArray.Length;

                using ( var stream = request.GetRequestStream() )
                {
                    stream.Write( byteArray, 0, byteArray.Length );
                }
            }

            HttpWebResponse resp = null;

            try
            {
                resp = ( HttpWebResponse ) request.GetResponse();
            }
            catch ( WebException we )
            {
                if ( we.Response == null )
                {
                    throw;
                }
                resp = ( HttpWebResponse ) we.Response;

            }
            string responseString = string.Empty;

            using ( System.IO.StreamReader sr = new System.IO.StreamReader( resp.GetResponseStream(), System.Text.Encoding.UTF8 ) )
            {
                responseString = sr.ReadToEnd();
            }

            RawResponse rawResp = new RawResponse( resp.StatusCode, responseString );

            return rawResp;
        }

        #endregion

        #region Attributes
        /// <summary>
        /// Gets the settings.
        /// </summary>
        /// <param name="rockContext">The rock context.</param>
        /// <returns></returns>
        private List<AttributeValue> GetSettings( RockContext rockContext )
        {
            var adobeSignEntityType = EntityTypeCache.Read( typeof( com.kfs.AdobeSign.AdobeSign ) );
            if ( adobeSignEntityType != null )
            {
                var service = new AttributeValueService( rockContext );
                return service.Queryable( "Attribute" )
                    .Where( v => v.Attribute.EntityTypeId == adobeSignEntityType.Id )
                    .ToList();
            }

            return null;
        }

        /// <summary>
        /// Gets the setting value.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        private string GetSettingValue( List<AttributeValue> values, string key, bool encryptedValue = false )
        {
            string value = values
                .Where( v => v.AttributeKey == key )
                .Select( v => v.Value )
                .FirstOrDefault();
            if ( encryptedValue && !string.IsNullOrWhiteSpace( value ) )
            {
                try
                { value = Encryption.DecryptString( value ); }
                catch { }
            }

            return value;
        }

        /// <summary>
        /// Sets the setting value.
        /// </summary>
        /// <param name="rockContext">The rock context.</param>
        /// <param name="values">The values.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        private void SetSettingValue( RockContext rockContext, List<AttributeValue> values, string key, string value, bool encryptValue = false )
        {
            if ( encryptValue && !string.IsNullOrWhiteSpace( value ) )
            {
                try
                { value = Encryption.EncryptString( value ); }
                catch { }
            }

            var attributeValue = values
                .Where( v => v.AttributeKey == key )
                .FirstOrDefault();
            if ( attributeValue != null )
            {
                attributeValue.Value = value;
            }
            else
            {
                var adobeSignEntityType = EntityTypeCache.Read( typeof( com.kfs.AdobeSign.AdobeSign ) );
                if ( adobeSignEntityType != null )
                {
                    var attribute = new AttributeService( rockContext )
                        .Queryable()
                        .Where( a =>
                            a.EntityTypeId == adobeSignEntityType.Id &&
                            a.Key == key
                        )
                        .FirstOrDefault();

                    if ( attribute != null )
                    {
                        attributeValue = new AttributeValue();
                        new AttributeValueService( rockContext ).Add( attributeValue );
                        attributeValue.AttributeId = attribute.Id;
                        attributeValue.Value = value;
                        attributeValue.EntityId = 0;
                    }
                }
            }

        }

        #endregion
    }
}
