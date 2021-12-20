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
    using Entities;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using RestSharp;
    using rocks.kfs.ZoomRoom.Enums;
    using System;
    using System.Collections.Generic;
    using ZoomDotNetFramework.Responses;

    public class ZoomApi
    {
        private const string BaseUrl = "https://api.zoom.us/v2/";
        private string _jwtToken;
        private string _jsonRpc = string.Empty;

        //Initializer
        public ZoomApi( string jwtToken, string jsonRpcVersion = "" )
        {
            _jwtToken = jwtToken;
            _jsonRpc = jsonRpcVersion;
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
                var ZoomException = new ApplicationException( message, response.ErrorException );
                throw ZoomException;
            }
            else if ( response.Content.Contains( "error" ) )
            {
                var errorResponse = JsonConvert.DeserializeObject<ZRErrorResponse>( response.Content );
                var message = string.Format( "Error Code {0}. {1}", errorResponse.Error.Code, errorResponse.Error.Message );
                if ( errorResponse.Error.Data != null )
                {
                    foreach ( var error in errorResponse.Error.Data )
                    {
                        message += string.Format( " {0}: {1};", error.Field, error.Message );
                    }
                }
                var ZoomException = new ApplicationException( message );
                throw ZoomException;
            }
            return response.Data;
        }

        #region Pure Rest Calls

        #region Zoom API

        public User GetUser()
        {
            var request = new RestRequest
            {
                Resource = "users/me/"
            };

            return Execute<User>( request );
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

        public Meeting GetZoomMeeting( long meetingId )
        {
            var request = new RestRequest
            {
                Resource = string.Format( "meetings/{0}", meetingId ),
            };
            var result = Execute<Meeting>( request );

            return result;
        }

        public List<Meeting> GetZoomMeetings( string userId, MeetingListType? type = null, int? pageSize = null )
        {
            var request = new RestRequest
            {
                Resource = string.Format( "users/{0}/meetings", userId ),
            };
            if ( type.HasValue )
            {
                request.AddParameter( "type", type.ToString() );
            }
            var result = Execute<ListMeetingsResponse>( request );

            return result.Meetings;
        }

        public bool DeleteMeeting( long meetingId, string occurrenceId = null, bool notifyHosts = false, bool notifyRegistrants = false )
        {
            var request = new RestRequest
            {
                Resource = string.Format( "meetings/{0}", meetingId ),
                Method = Method.DELETE
            };
            if ( !string.IsNullOrWhiteSpace( occurrenceId ) )
            {
                request.AddParameter( "occurrence_id", occurrenceId );
            }
            request.AddParameter( "schedule_for_reminder", notifyHosts );
            request.AddParameter( "cancel_meeting_reminder", notifyRegistrants.ToString().ToLower() );
            var result = Execute<ZoomBaseResponse>( request );
            return result != null;
        }

        #endregion Zoom API

        #region Zoom Room API

        public List<ZRRoom> GetZoomRoomList()
        {
            var request = new RestRequest
            {
                Resource = "rooms/zrlist",
                Method = Method.POST
            };
            var reqBody = new ZRRequestBodyBase
            {
                Method = "list",
                JsonRPC = _jsonRpc
            };
            AddRequestJsonBody( request, reqBody );
            var result = Execute<ZRListResponse>( request );
            return result.Result.Data;
        }

        public bool ScheduleZoomRoomMeeting( string roomId, string password, string callbackUrl, string topic, DateTimeOffset startTime, string timezone, int duration, bool joinBeforeHost = false )
        {
            var request = new RestRequest
            {
                Resource = string.Format( "rooms/{0}/meetings", roomId ),
                Method = Method.POST
            };
            var reqBody = new ZRScheduleRequest
            {
                Method = "schedule",
                JsonRPC = _jsonRpc,
                Params = new ZRScheduleRequestBody
                {
                    Password = password,
                    Callback_Url = callbackUrl,
                    Meeting_Info = new ZRScheduleMeetingInfo
                    {
                        Topic = topic,
                        Start_Time = startTime,
                        Duration = duration,
                        Timezone = timezone,
                        Settings = new ZRScheduleMeetingInfoSetting
                        {
                            Join_Before_Host = joinBeforeHost
                        }
                    }
                }
            };
            AddRequestJsonBody( request, reqBody );
            var result = Execute<ZRScheduleResponse>( request );
            return result != null;
        }

        public bool CancelZoomRoomMeeting( string roomId, string topic, DateTimeOffset startTime, string timezone, int duration )
        {
            var request = new RestRequest
            {
                Resource = string.Format( "rooms/{0}/meetings", roomId ),
                Method = Method.POST
            };
            var reqBody = new ZRCancelMeetingRequest
            {
                Method = "cancel",
                JsonRPC = _jsonRpc,
                Params = new ZRCancelMeetingRequestBody
                {
                    Meeting_Info = new ZRScheduleMeetingInfo
                    {
                        Topic = topic,
                        Start_Time = startTime,
                        Timezone = timezone,
                        Duration = duration
                    }
                }
            };
            AddRequestJsonBody( request, reqBody );
            var result = Execute<ZRScheduleResponse>( request );
            return result != null;
        }

        #endregion Zoom Room API

        #endregion Pure Rest Calls

        public RestRequest AddRequestJsonBody( RestRequest request, object bodyObject )
        {
            var settings = new JsonSerializerSettings();
            settings.ContractResolver = new LowercaseContractResolver();
            var jsonBody = JsonConvert.SerializeObject( bodyObject, Formatting.Indented, settings );
            request.AddParameter( "application/json", jsonBody, ParameterType.RequestBody );
            return request;
        }

        public class LowercaseContractResolver : DefaultContractResolver
        {
            protected override string ResolvePropertyName( string propertyName )
            {
                return propertyName.ToLower();
            }
        }
    }

}
