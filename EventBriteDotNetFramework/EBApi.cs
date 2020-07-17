// <copyright>
// Copyright 2020 by Kingdom First Solutions
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
namespace EventbriteDotNetFramework
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Globalization;
    using System.Linq;
    using Entities;
    using EventbriteDotNetFramework.Responses;
    using RestSharp;

    public class EBApi
    {
        private const string BaseUrl = "https://www.eventbriteapi.com/v3/";
        private string _oauthtoken;
        private long _me;
        private User _user;
        private long? _org;

        public long OrganizationId
        {
            get
            {
                if ( !_org.HasValue )
                {
                    var organizations = GetOrganizations();
                    if ( organizations != null && organizations.Organizations.Any() )
                    {
                        _org = organizations.Organizations.Select( o => o.Id ).FirstOrDefault();
                    }
                }
                return _org.Value;
            }
            set
            {
                _org = value;
            }
        }
        //Initializer
        public EBApi( string oAuthToken )
        {
            _oauthtoken = oAuthToken;
            _user = GetUser();
            _me = _user.Id;
        }
        public EBApi( string oAuthToken, long orgId )
        {
            _oauthtoken = oAuthToken;
            _user = GetUser();
            _me = _user.Id;
            _org = orgId;
        }

        //Base Execute
        public T Execute<T>( RestRequest request ) where T : new()
        {
            var client = new RestClient
            {
                BaseUrl = new System.Uri( BaseUrl )
            };
            request.AddHeader( "Authorization", "Bearer " + _oauthtoken ); // used on every request
            var response = client.Execute<T>( request );

            if ( response.ErrorException != null )
            {
                const string message = "Error retrieving response.  Check inner details for more info.";
                var EventBriteException = new ApplicationException( message, response.ErrorException );
                throw EventBriteException;
            }
            return response.Data;
        }

        #region Pure Rest Calls

        public User GetUser()
        {
            var request = new RestRequest
            {
                Resource = "/users/me/"
            };

            return Execute<User>( request );
        }

        public OrganizationEventsResponse GetOrganizationEvents()
        {
            var request = new RestRequest
            {
                Resource = string.Format( "/organizations/{0}/events/", OrganizationId )
            };

            return Execute<OrganizationEventsResponse>( request );
        }
        public UserOrganizationsResponse GetOrganizations()
        {
            var request = new RestRequest
            {
                Resource = "/users/me/organizations/"
            };

            return Execute<UserOrganizationsResponse>( request );
        }

        public Event GetEventById( long id )
        {
            var request = new RestRequest
            {
                Resource = string.Format( "/events/{0}/", id.ToString() )
            };

            return Execute<Event>( request );
        }

        public EventOrders GetOrdersById( long id )
        {
            return GetOrdersById( id, 1 );
        }

        public EventOrders GetOrdersById( long id, int page )
        {
            var request = new RestRequest
            {
                Resource = string.Format( "events/{0}/orders/?page={1}", id.ToString(), page.ToString() )
            };

            return Execute<EventOrders>( request );
        }

        public EventOrders GetExpandedOrdersById( long id )
        {
            return GetExpandedOrdersById( id, 1 );
        }

        public EventOrders GetExpandedOrdersById( long id, int page )
        {
            var request = new RestRequest
            {
                Resource = string.Format( "events/{0}/orders/?page={1}&expand=attendees", id.ToString(), page.ToString() )
            };

            return Execute<EventOrders>( request );
        }

        public EventTicketClasses GetTicketsById( long id )
        {
            var request = new RestRequest
            {
                Resource = string.Format( "events/{0}/ticket_classes/", id.ToString() )
            };

            return Execute<EventTicketClasses>( request );
        }

        public EventCannedQuestions GetEventCannedQuestionsById( long id )
        {
            return GetEventCannedQuestionsbyId( id, 1 );
        }

        public EventCannedQuestions GetEventCannedQuestionsbyId( long id, int page )
        {
            var request = new RestRequest
            {
                Resource = string.Format( "events/{0}/canned_questions/?page={1}", id.ToString(), page.ToString() )
            };

            return Execute<EventCannedQuestions>( request );
        }

        #endregion

        #region Mixed Methods

        public OrganizationEventsResponse GetOrganizationEvents( bool RSVP = false )
        {
            var evnts = GetOrganizationEvents();
            var retVar = new OrganizationEventsResponse
            {
                Pagination = evnts.Pagination,
                Events = new List<Event>()
            };
            if ( evnts.Events != null )
            {
                foreach ( var evnt in evnts.Events )
                {
                    var cq = GetEventCannedQuestionsById( evnt.Id );
                    var test = cq.Questions.FirstOrDefault( q => q.Respondent.Equals( "attendee", StringComparison.CurrentCultureIgnoreCase ) );
                    if ( ( test == null && RSVP ) || ( test != null && !RSVP ) )
                    {
                        retVar.Events.Add( evnt );
                    }
                }
            }
            else
            {
                retVar.Events = evnts.Events;
            }

            return retVar;
        }

        #endregion
    }
}
