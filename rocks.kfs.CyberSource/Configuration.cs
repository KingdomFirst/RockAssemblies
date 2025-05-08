// <copyright>
// Copyright 2024 by Kingdom First Solutions
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
using System.Text;

using Rock;
using Rock.Model;
using Rock.Web.Cache;

using CyberSource.Api;
using CyberSource.Model;
using CyberSourceSDK = CyberSource;

using rocks.kfs.CyberSource.Model;

namespace rocks.kfs.CyberSource
{
    public class Configuration
    {
        private readonly Dictionary<string, string> _configurationDictionary = new Dictionary<string, string>();

        private static CyberSourceSDK.Client.Configuration _clientConfig;

        public Dictionary<string, string> GetConfiguration( FinancialGateway cyberSourceGateway )
        {
            var orgId = cyberSourceGateway.GetAttributeValue( Gateway.AttributeKey.OrganizationId );
            _configurationDictionary.Add( "authenticationType", "HTTP_SIGNATURE" );
            _configurationDictionary.Add( "merchantID", orgId );
            _configurationDictionary.Add( "merchantsecretKey", cyberSourceGateway.GetAttributeValue( Gateway.AttributeKey.APISecret ) );
            _configurationDictionary.Add( "merchantKeyId", cyberSourceGateway.GetAttributeValue( Gateway.AttributeKey.APIKey ) );
            _configurationDictionary.Add( "keysDirectory", "Resource" );
            _configurationDictionary.Add( "keyFilename", orgId );
            _configurationDictionary.Add( "runEnvironment", Gateway.GetGatewayUrl( cyberSourceGateway ) );
            _configurationDictionary.Add( "keyAlias", orgId );
            _configurationDictionary.Add( "keyPass", orgId );
            _configurationDictionary.Add( "enableLog", "FALSE" );
            _configurationDictionary.Add( "logDirectory", string.Empty );
            _configurationDictionary.Add( "logFileName", string.Empty );
            _configurationDictionary.Add( "logFileMaxSize", "5242880" );
            _configurationDictionary.Add( "timeout", "10000" );
            _configurationDictionary.Add( "proxyAddress", string.Empty );
            _configurationDictionary.Add( "proxyPort", string.Empty );
            _configurationDictionary.Add( "rockGatewayGuid", cyberSourceGateway.Guid.ToString() );

            return _configurationDictionary;
        }

        public static CyberSourceSDK.Client.Configuration GetClientConfig( FinancialGateway gateway )
        {
            if ( _clientConfig == null || !_clientConfig.MerchantConfigDictionaryObj.Contains( new KeyValuePair<string, string>( "rockGatewayGuid", gateway.Guid.ToString() ) ) )
            {
                var configDictionary = new Configuration().GetConfiguration( gateway );
                _clientConfig = new CyberSourceSDK.Client.Configuration( merchConfigDictObj: configDictionary );
            }

            return _clientConfig;
        }

        public static string GetMicroFormJWK( FinancialGateway gateway, out string microformJWK )
        {
            List<String> targetOrigins = new List<String>()
            {
                GlobalAttributesCache.Get().GetValue( "PublicApplicationRoot" ).ReplaceIfEndsWith("/",""),
                GlobalAttributesCache.Get().GetValue( "InternalApplicationRoot" ).ReplaceIfEndsWith("/","")
            };

            gateway.LoadAttributes();

            var allowedCardNetworks = gateway.GetAttributeValues( Gateway.AttributeKey.AllowedCardNetworks );

            string clientVersion = "v2.0";

            var requestObj = new GenerateCaptureContextRequest(
                TargetOrigins: targetOrigins,
                AllowedCardNetworks: allowedCardNetworks,
                ClientVersion: clientVersion
            );

            try
            {
                var clientConfig = GetClientConfig( gateway );

                var apiInstance = new MicroformIntegrationApi( clientConfig );
                String result = apiInstance.GenerateCaptureContext( requestObj );
                microformJWK = result;

                var microFormJsPath = "https://flex.cybersource.com/microform/bundle/v2/flex-microform.min.js";

                try
                {
                    var splitResult = result.Split( '.' );
                    var parseJwtResponse = Base64UrlDecode( splitResult[1] );
                    var parseToObject = parseJwtResponse.FromJsonOrNull<FlexCaptureContextPayload>();

                    if ( parseToObject != null && parseToObject.ctx?.FirstOrDefault().data != null )
                    {
                        var microFormData = parseToObject.ctx.FirstOrDefault().data;
                        microFormJsPath = $"{microFormData.clientLibrary}|{microFormData.clientLibraryIntegrity}";

                    }
                }
                catch ( Exception ex )
                {
                    ExceptionLogService.LogException( ex );

                }
                return microFormJsPath;
            }
            catch ( Exception e )
            {
                ExceptionLogService.LogException( "Exception on calling the CyberSource API : " + e.Message );
                throw e;
            }
        }

        public static string Base64UrlDecode( string text )
        {
            text = text.Replace( '_', '/' ).Replace( '-', '+' );
            switch ( text.Length % 4 )
            {
                case 2:
                    text += "==";
                    break;
                case 3:
                    text += "=";
                    break;
            }
            return Encoding.UTF8.GetString( Convert.FromBase64String( text ) );
        }
    }
}
