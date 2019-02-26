using System;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;
using RestSharp;
using Rock;

namespace com.kfs.MinistrySafe
{
    class Trainings
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
