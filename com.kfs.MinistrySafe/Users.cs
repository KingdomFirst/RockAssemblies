using System;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;
using RestSharp;
using Rock;

namespace com.kfs.MinistrySafe
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
