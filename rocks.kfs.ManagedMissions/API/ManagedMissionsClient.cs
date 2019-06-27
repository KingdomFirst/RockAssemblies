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

using Newtonsoft.Json;
using RestSharp;

namespace rocks.kfs.ManagedMissions.API
{
    public class ManagedMissionsClient
    {
        #region Fields

        private string mBaseUrl = "";
        private string mAPIKey = "";

        #endregion

        #region Properties

        public string BaseUrl
        {
            get
            {
                return mBaseUrl;
            }
            set
            {
                mBaseUrl = value;
            }
        }

        public string APIKey
        {
            get
            {
                return mAPIKey;
            }
            set
            {
                mAPIKey = value;
            }
        }

        #endregion

        public ManagedMissionsClient()
        {
        }

        public ManagedMissionsClient( string url, string key )
        {
            BaseUrl = url;
            APIKey = key;
        }

        public ResponseData<T> SendRequest<T>( RestRequest request )
        {
            RestClient client = new RestClient( mBaseUrl );
            request.AddQueryParameter( "apiKey", APIKey );
            var response = client.Execute( request );

            var content = response.Content;

            ResponseData<T> data = JsonConvert.DeserializeObject<ResponseData<T>>( content );

            data.StatusCode = response.StatusCode;

            if ( response.ErrorException != null )
            {
                throw new Exception( "An error has occurred while processing Managed Missions Request. See Inner Exception for details.", response.ErrorException );
            }

            return data;
        }
    }
}
