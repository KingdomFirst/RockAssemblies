// <copyright>
// Copyright 2021 by Kingdom First Solutions
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
namespace ZoomDotNetFramework
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Globalization;
    using System.Linq;
    using System.IdentityModel.Tokens;
    using System.Security.Claims;
    using Entities;
    //using EventbriteDotNetFramework.Responses;
    using RestSharp;
    using JWT.Builder;
    using JWT.Algorithms;
    using ZoomDotNetFramework.Responses;

    public class ZoomApi
    {
        private const string BaseUrl = "https://api.zoom.us/v2/";
        private string _jwtToken;
        private long _me;
        private User _user;
        //private long? _org;

        //public long OrganizationId
        //{
        //    get
        //    {
        //        if ( !_org.HasValue )
        //        {
        //            var organizations = GetOrganizations();
        //            if ( organizations != null && organizations.Organizations != null && organizations.Organizations.Any() )
        //            {
        //                _org = organizations.Organizations.Select( o => o.Id ).FirstOrDefault();
        //            } 
        //            else
        //            {
        //                return 0;
        //            }
        //        }
        //        return _org.Value;
        //    }
        //    set
        //    {
        //        _org = value;
        //    }
        //}
        //Initializer
        public ZoomApi( string jwtToken )
        {
            _jwtToken = jwtToken;
        }
        //public ZoomApi( string jwtToken, long orgId )
        //{
        //    _jwtToken = jwtToken;
        //    _user = GetUser();
        //    _me = _user.Id;
        //    _org = orgId;
        //}

        //Base Execute
        public T Execute<T>( RestRequest request ) where T : new()
        {
            var client = new RestClient
            {
                BaseUrl = new System.Uri( BaseUrl )
            };
            request.AddHeader( "content-type", "application/json" );
            request.AddHeader( "authorization", "Bearer " + _jwtToken ); // used on every request
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

        public List<ZRRoom> GetZoomRoomList()
        {
            var request = new RestRequest
            {
                Resource = "rooms/zrlist"
            };
            var result = Execute<ZRListResponse>( request );

            return result.Result.Data;
        }

        public List<ZoomRoom> GetZoomRooms()
        {
            var request = new RestRequest
            {
                Resource = "rooms"
            };
            var result = Execute<RoomsResponse>( request );

            return result.Rooms;
        }

        public ZoomRoom GetZoomRoomById( string zoomRoomId )
        {
            var request = new RestRequest
            {
                Resource = string.Format( "events/{0}/", id.ToString() )
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
        public Order GetOrderById( long id )
        {
            var request = new RestRequest
            {
                Resource = string.Format( "orders/{0}/", id.ToString() )
            };

            return Execute<Order>( request );
        }
        public Order GetExpandedOrderById( long id )
        {
            var request = new RestRequest
            {
                Resource = string.Format( "orders/{0}/?expand=attendees", id.ToString() )
            };

            return Execute<Order>( request );
        }
        public Order GetOrder( string apiUrl )
        {
            var request = new RestRequest
            {
                Resource = string.Format( "{0}", apiUrl.Replace( BaseUrl, "" ) )
            };

            return Execute<Order>( request );
        }

        public Order GetOrder( string apiUrl, string expand )
        {
            var request = new RestRequest
            {
                Resource = string.Format( "{0}?expand={1}", apiUrl.Replace( BaseUrl, "" ), expand )
            };

            return Execute<Order>( request );
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

        public Attendee GetAttendee( string apiUrl )
        {
            var request = new RestRequest
            {
                Resource = string.Format( "{0}", apiUrl.Replace( BaseUrl, "" ) )
            };

            return Execute<Attendee>( request );
        }

        public Attendee GetAttendee( string apiUrl, string expand )
        {
            var request = new RestRequest
            {
                Resource = string.Format( "{0}?expand={1}", apiUrl.Replace( BaseUrl, "" ), expand )
            };

            return Execute<Attendee>( request );
        }

        public Attendee GetAttendee( long evntid, long id )
        {
            var request = new RestRequest
            {
                Resource = string.Format( "events/{0}/attendees/{1}", evntid.ToString(), id.ToString() )
            };

            return Execute<Attendee>( request );
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
