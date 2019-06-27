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
    public class Trainings
    {
        /// <summary>
        /// Assigns the specified training to a user.
        /// http://developers.abusepreventionsystems.com/#assign-a-training-to-a-user
        /// </summary>
        /// <param name="APIKey">The API Key.</param>
        /// <param name="MinistrySafeId">The Ministry Safe Id of the User.</param>
        /// <param name="SurveyCode">Optional survey code to assign to the User.</param>
        /// <param name="StagingMode">Optional flag indicating if the Staging API should be used.</param>
        /// <returns>Returns true or error.</returns>
        public static dynamic AssignTraining( string APIKey, string MinistrySafeId, string SurveyCode = "standard", bool StagingMode = false )
        {
            var client = new RestClient();
            client.BaseUrl = MinistrySafe.BaseUrl( StagingMode );

            var path = $"/v2/users/{ MinistrySafeId }/assign_training";

            var request = new RestRequest( path, Method.POST )
                .AddHeader( "Authorization", "Token token=" + APIKey )
                .AddParameter( "survey_code", SurveyCode );

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
