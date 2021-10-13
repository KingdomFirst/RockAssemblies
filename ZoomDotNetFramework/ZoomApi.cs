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
    using RestSharp;
    using JWT.Builder;
    using JWT.Algorithms;
    using ZoomDotNetFramework.Responses;

    public class ZoomApi
    {
        private const string BaseUrl = "https://api.zoom.us/v2/";
        private string _jwtToken;
        private long _me;

        //Initializer
        public ZoomApi( string jwtToken )
        {
            _jwtToken = jwtToken;
        }

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

        public List<ZoomRoom> ScheduleZoomRoomMeeting( ZoomRoom room, string topic, string password )
        {
            var request = new RestRequest
            {
                Resource = string.Format( "rooms/{0}/meetings", room.Id )
            };
            if ( !string.IsNullOrWhiteSpace( password ) )
            {
                request.AddParameter( "password", password );
            }
            var result = Execute<RoomsResponse>( request );

            return result.Rooms;
        }

        #endregion

        #region Mixed Methods

        #endregion

    }
}
