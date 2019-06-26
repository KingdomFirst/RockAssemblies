using System;
using System.Net;
using Newtonsoft.Json;
using RestSharp;

namespace com.kfs.Checkr
{
    class Package
    {
        /// <summary>
        /// This request is used to retrieve existing packages.
        /// https://docs.checkr.com/#package
        /// </summary>
        /// <param name="APIKey">The Checkr API Key.</param>
        /// <param name="PackageId">The package Id.</param>
        /// <returns>Returns a representation of the package.</returns>
        public static dynamic RetrievePackage( string APIKey, string PackageId )
        {
            var client = new RestClient();
            client.BaseUrl = Checkr.checkrBaseUrl;

            var path = $"/v1/packages/{ PackageId }";

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
