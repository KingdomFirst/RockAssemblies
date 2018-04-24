using System;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;
using RestSharp;

namespace com.kfs.Checkr
{
    class Invitation
    {
        /// <summary>
        /// This request is used to retrieve existing Invitations.
        /// https://docs.checkr.com/#invitation
        /// </summary>
        /// <param name="APIKey">The Checkr API Key.</param>
        /// <param name="CandidateId">The person's Checkr CandidateId.</param>
        /// <param name="Status">The Status of the Invitation.</param>
        /// <returns>Returns a list of Invitations.</returns>
        public static dynamic RetrieveInvitations( string APIKey, string CandidateId, InvitationStatus Status = InvitationStatus.All )
        {
            var client = new RestClient();
            client.BaseUrl = Checkr.checkrBaseUrl;

            var path = $"/v1/invitations/";
            
            var invitationKeyMap = new Dictionary<string, string>();
            invitationKeyMap.Add( "candidate_id", CandidateId );

            switch ( Status )
            {
                case InvitationStatus.Pending:
                    invitationKeyMap.Add( "status", "pending" );
                    break;
                case InvitationStatus.Complete:
                    invitationKeyMap.Add( "status", "complete" );
                    break;
                case InvitationStatus.Expired:
                    invitationKeyMap.Add( "status", "expired" );
                    break;
                default:
                    break;
            }

            var request = new RestRequest( path, Method.POST )
                .AddHeader( "Accept", "application/json" )
                .AddHeader( "Authorization", $"Basic { Checkr.encodeClientCredentials( APIKey ) }" );

            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody( invitationKeyMap );

            var response = client.Execute( request );

            dynamic results = "";

            if ( response.StatusCode == HttpStatusCode.OK )  // TODO VERIFY HTTPSTATUS CODE
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
        /// This request is used to retrieve existing Invitations.
        /// https://docs.checkr.com/#invitation
        /// </summary>
        /// <param name="APIKey">The Checkr API Key.</param>
        /// <param name="InvitationId">The Id of the Invitation to retrieve.</param>
        /// <returns>Returns a representation of the Invitation.</returns>
        public static dynamic RetrieveInvitation( string APIKey, string InvitationId )
        {
            var client = new RestClient();
            client.BaseUrl = Checkr.checkrBaseUrl;

            var path = $"/v1/invitations/";

            var invitationKeyMap = new Dictionary<string, string>();
            invitationKeyMap.Add( "id", InvitationId );

            var request = new RestRequest( path, Method.POST )
                .AddHeader( "Accept", "application/json" )
                .AddHeader( "Authorization", $"Basic { Checkr.encodeClientCredentials( APIKey ) }" );

            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody( invitationKeyMap );

            var response = client.Execute( request );

            dynamic results = "";

            if ( response.StatusCode == HttpStatusCode.OK )  // TODO VERIFY HTTPSTATUS CODE
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
        /// This request is used to create a new Invitation.
        /// https://docs.checkr.com/#invitation
        /// </summary>
        /// <param name="APIKey">The Checkr API Key.</param>
        /// <param name="PackageSlug">The slug of the package to create for the Candidate.</param>
        /// <param name="CandidateId">The person's Checkr CandidateId.</param>
        /// <returns>Returns a representation of the Invitation.</returns>
        public static dynamic CreateInvitation( string APIKey, string PackageSlug, string CandidateId )
        {
            dynamic results = "";

            if ( !string.IsNullOrWhiteSpace( PackageSlug ) )
            {
                var client = new RestClient();
                client.BaseUrl = Checkr.checkrBaseUrl;

                var path = $"/v1/invitations/";

                var invitationKeyMap = new Dictionary<string, string>();
                invitationKeyMap.Add( "candidate_id", CandidateId );
                invitationKeyMap.Add( "package", PackageSlug );

                var request = new RestRequest( path, Method.POST )
                    .AddHeader( "Accept", "application/json" )
                    .AddHeader( "Authorization", $"Basic { Checkr.encodeClientCredentials( APIKey ) }" );

                request.RequestFormat = DataFormat.Json;
                request.AddJsonBody( invitationKeyMap );

                var response = client.Execute( request );
                
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
            }

            return results;
        }

        /// <summary>
        /// This request is used to cancel an existing Invitation.
        /// https://docs.checkr.com/#invitation
        /// </summary>
        /// <param name="APIKey">The Checkr API Key.</param>
        /// <param name="InvitationId">The Id of the Invitation to cancel.</param>
        /// <returns>Returns a representation of the Invitation.</returns>
        public static dynamic CancelInvitation( string APIKey, string InvitationId )
        {
            var client = new RestClient();
            client.BaseUrl = Checkr.checkrBaseUrl;

            var path = $"/v1/invitations/";

            var invitationKeyMap = new Dictionary<string, string>();
            invitationKeyMap.Add( "id", InvitationId );
            
            var request = new RestRequest( path, Method.POST )
                .AddHeader( "Accept", "application/json" )
                .AddHeader( "Authorization", $"Basic { Checkr.encodeClientCredentials( APIKey ) }" );

            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody( invitationKeyMap );

            var response = client.Execute( request );

            dynamic results = "";

            if ( response.StatusCode == HttpStatusCode.OK )  // TODO VERIFY HTTPSTATUS CODE
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

        public enum InvitationStatus
        {
            All,
            Pending,
            Complete,
            Expired
        }
    }
}
