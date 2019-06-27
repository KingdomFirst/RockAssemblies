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
using System.Net;

using Newtonsoft.Json;
using RestSharp;

namespace rocks.kfs.MinistrySafe
{
    public class Users
    {
        /// <summary>
        /// This endpoint creates a new user.
        /// http://developers.abusepreventionsystems.com/#create-a-user
        /// </summary>
        /// <param name="APIKey">The API Key.</param>
        /// <param name="Person">The person who will be created as a Ministry Safe User.</param>
        /// <param name="ExternalId">Optional Id to use as the External Id (i.e. WorkflowId).  If left blank, Person.PrimaryAliasId will be used (i.e. 'P1234').</param>
        /// <param name="StagingMode">Optional flag indicating if the Staging API should be used.</param>
        /// <returns>Returns true or error.</returns>
        public static dynamic CreateUser( string APIKey, Rock.Model.Person Person, string msUserType = "volunteer", string ExternalId = "", bool StagingMode = false )
        {
            var externalId = string.IsNullOrWhiteSpace( ExternalId ) ? $"P{ Person.PrimaryAliasId.ToString() }" : ExternalId;
            var body = $"user[first_name]={ Person.FirstName }&user[last_name]={ Person.LastName }&user[email]={ Person.Email }&user[external_id]={ externalId }&user[user_type]={ msUserType }";

            var client = new RestClient();
            client.BaseUrl = MinistrySafe.BaseUrl( StagingMode );

            var path = "/v2/users";

            var request = new RestRequest( path, Method.POST )
                .AddHeader( "Authorization", "Token token=" + APIKey );

            request.AddHeader( "content-type", "application/x-www-form-urlencoded" );
            request.AddParameter( "application/x-www-form-urlencoded", body, ParameterType.RequestBody );

            var response = client.Execute( request );

            dynamic results = "";

            if ( response.StatusCode == HttpStatusCode.Created )
            {
                results = response.Content;
            }
            else
            {
                Console.WriteLine( response.Content.ToString() );
                results = response.Content.ToString();
            }

            results = JsonConvert.DeserializeObject( results );

            return results;
        }

        /// <summary>
        /// This endpoint retrieves a specific user.
        /// http://developers.abusepreventionsystems.com/#get-a-user
        /// </summary>
        /// <param name="APIKey">The API Key.</param>
        /// <param name="UserId">The Ministry Safe User Id.</param>
        /// <param name="StagingMode">Optional flag indicating if the Staging API should be used.</param>
        /// <returns>Returns true or error.</returns>
        public static dynamic GetUser( string APIKey, string UserId, bool StagingMode = false )
        {
            var client = new RestClient();
            client.BaseUrl = MinistrySafe.BaseUrl( StagingMode );

            var path = $"/v2/users/{ UserId }";

            var request = new RestRequest( path, Method.GET )
                .AddHeader( "Authorization", "Token token=" + APIKey );

            var response = client.Execute( request );

            dynamic results = "";

            if ( response.StatusCode == HttpStatusCode.OK )
            {
                results = response.Content;
            }
            else
            {
                Console.WriteLine( response.Content.ToString() );
                results = response.Content.ToString();
            }

            results = JsonConvert.DeserializeObject( results );

            return results;
        }
    }
}
