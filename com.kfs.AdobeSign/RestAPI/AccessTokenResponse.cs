using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json;
namespace com.kfs.AdobeSign.RestAPI
{
    public class AccessTokenResponse
    {
        [JsonProperty( "access_token" )]
        public string AccessToken { get; set; }
        [JsonProperty( "refresh_token", NullValueHandling = NullValueHandling.Include, Required = Required.Default )]
        public string RefreshToken { get; set; }
        [JsonProperty( "expires_in" )]
        public int ExpiresIn { get; set; }
        [JsonProperty( "token_type" )]
        public string TokenType { get; set; }

        public AccessTokenResponse() { }
        public AccessTokenResponse( string json )
        {
            DeserializeJson( json );
        }
        
        private void DeserializeJson( string json )
        {
            AccessTokenResponse atr = JsonConvert.DeserializeObject<AccessTokenResponse>( json );

            AccessToken = atr.AccessToken;
            RefreshToken = atr.RefreshToken;
            ExpiresIn = atr.ExpiresIn;
            TokenType = atr.TokenType;
        }
    }
}
