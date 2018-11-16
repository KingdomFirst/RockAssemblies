using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using RestSharp;
using RestSharp.Authenticators;

namespace com.kfs.Reach.Reporting
{
    public static class Api
    {
        /// <summary>
        /// Posts the request to the gateway endpoint.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="authenticator">The authenticator.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <returns></returns>
        /// <exception cref="Exception">
        /// </exception>
        /// <exception cref="System.Exception"></exception>
        public static object PostRequest( string url, HttpBasicAuthenticator authenticator, Dictionary<string, string> parameters, out string errorMessage )
        {
            errorMessage = string.Empty;
            var restClient = new RestClient( url )
            {
                Authenticator = authenticator
            };

            var restRequest = new RestRequest( Method.GET )
            {
                RequestFormat = DataFormat.Json
            };

            if ( parameters != null )
            {
                foreach ( var param in parameters )
                {
                    restRequest.AddParameter( param.Key, param.Value );
                }
            }

            try
            {
                var response = restClient.Execute( restRequest );
                if ( response != null && response.StatusCode == HttpStatusCode.OK )
                {
                    return response.Content;
                }
            }
            catch ( WebException webException )
            {
                var message = GetResponseMessage( webException.Response.GetResponseStream() );
                throw new Exception( webException.Message + " - " + message );
            }
            catch ( Exception ex )
            {
                throw new Exception( ex?.InnerException?.Message, ex );
            }

            return null;
        }

        /// <summary>
        /// Gets the response message.
        /// </summary>
        /// <param name="responseStream">The response stream.</param>
        /// <returns></returns>
        private static string GetResponseMessage( Stream responseStream )
        {
            var receiveStream = responseStream;
            var encode = Encoding.GetEncoding( "utf-8" );
            var sb = new StringBuilder();
            using ( var readStream = new StreamReader( receiveStream, encode ) )
            {
                var read = new Char[8192];
                var count = 0;
                do
                {
                    count = readStream.Read( read, 0, 8192 );
                    var str = new string( read, 0, count );
                    sb.Append( str );
                }
                while ( count > 0 );
            }

            return sb.ToString();
        }
    }
}
