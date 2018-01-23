using System;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;
using RestSharp;

namespace com.kfs.Checkr
{
    class Candidate
    {
        /// <summary>
        /// This request is used to create a new Candidate.
        /// https://docs.checkr.com/#candidate
        /// </summary>
        /// <param name="APIKey">The Checkr API Key.</param>
        /// <param name="Person">The person who will be created as a Checkr Candidate.</param>
        /// <returns>Returns a representation of the Candidate.</returns>
        public static dynamic CreateCandidate( string APIKey, Rock.Model.Person Person )
        {
            var client = new RestClient();
            client.BaseUrl = Checkr.checkrBaseUrl;

            var path = "/v1/candidates";

            var candidateKeyMap = new Dictionary<string, string>();

            candidateKeyMap.Add( "email", Person.Email );
            candidateKeyMap.Add( "custom_id", Person.PrimaryAliasId.ToString() );

            if ( !string.IsNullOrWhiteSpace( Person.FirstName ) )
            {
                candidateKeyMap.Add( "first_name", Person.FirstName );
            }

            if ( !string.IsNullOrWhiteSpace( Person.MiddleName ) )
            {
                candidateKeyMap.Add( "middle_name", Person.MiddleName );
            }

            if ( !string.IsNullOrWhiteSpace( Person.LastName ) )
            {
                candidateKeyMap.Add( "last_name", Person.LastName );
            }

            if ( Person.BirthDate != null && Person.BirthDate > DateTime.MinValue )
            {
                candidateKeyMap.Add( "dob", ( ( DateTime ) Person.BirthDate ).ToString( "yyyy-MM-dd" ) );
            }

            dynamic json = new { candidateKeyMap };

            var request = new RestRequest( path, Method.POST )
                .AddHeader( "Accept", "application/json" )
                .AddHeader( "Authorization", $"Basic { Checkr.encodeClientCredentials( APIKey ) }" );

            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody( candidateKeyMap );

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
        /// This request is used to retrieve existing Candidates.
        /// https://docs.checkr.com/#candidate
        /// </summary>
        /// <param name="APIKey">The Checkr API Key.</param>
        /// <param name="CandidateId">The person's Checkr CandidateId.</param>
        /// <returns>Returns a representation of the Candidate.</returns>
        public static dynamic RetrieveCandidate( string APIKey, string CandidateId )
        {
            var client = new RestClient();
            client.BaseUrl = Checkr.checkrBaseUrl;

            var path = $"/v1/candidates/{ CandidateId }";

            var request = new RestRequest( path, Method.POST )
                .AddHeader( "Accept", "application/json" )
                .AddHeader( "Authorization", $"Basic { Checkr.encodeClientCredentials( APIKey ) }" );

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
