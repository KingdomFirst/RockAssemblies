using System;
using System.Net;
using Newtonsoft.Json;
using RestSharp;

namespace com.kfs.Checkr
{
    class Document
    {
        /// <summary>
        /// This request is used to retrieve existing documents.
        /// https://docs.checkr.com/#document
        /// </summary>
        /// <param name="APIKey">The Checkr API Key.</param>
        /// <param name="DocumentId">The document Id.</param>
        /// <returns>Returns a representation of the package.</returns>
        public static dynamic RetrieveDocument( string APIKey, string DocumentId )
        {
            var client = new RestClient();
            client.BaseUrl = Checkr.checkrBaseUrl;

            var path = $"/v1/documents/{ DocumentId }";

            var request = new RestRequest( path, Method.GET )
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
