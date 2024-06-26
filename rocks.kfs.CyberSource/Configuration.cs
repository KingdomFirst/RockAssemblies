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
using System.Collections.Generic;
using Rock;
using Rock.Model;

namespace rocks.kfs.CyberSource
{
    public class Configuration
    {
        // initialize dictionary object
        private readonly Dictionary<string, string> _configurationDictionary = new Dictionary<string, string>();

        public Dictionary<string, string> GetConfiguration( FinancialGateway cyberSourceGateway )
        {
            cyberSourceGateway.LoadAttributes();
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

            return _configurationDictionary;
        }
    }
}
