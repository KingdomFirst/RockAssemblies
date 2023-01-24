// <copyright>
// Copyright 2023 by Kingdom First Solutions
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
    using System;
    using System.Collections.Generic;
    using System.Text;
    using ZoomDotNetFramework.Enums;
    using ZoomDotNetFramework.Responses;

    public class ZoomApi
    {
        private const string BaseUrl = "https://api.zoom.us/v2/";
        private string _oAuthToken;
        private string _jsonRpc = string.Empty;

        //Initializer
        public ZoomApi( string oAuthToken, string jsonRpcVersion = "" )
        {
            _oAuthToken = oAuthToken;
            _jsonRpc = jsonRpcVersion;
        }

        /// <summary>
        /// Gets an Oauth token for a Zoom app.
        /// </summary>
        /// <returns></returns>
        public static string GetOauthToken( string zoomAppAccountId, string zoomAppClientId, string zoomAppClientSecret )
        {
            string tokenString = null;
            var client = new RestClient
            {
                BaseUrl = new System.Uri( string.Format( "https://zoom.us/oauth/token?grant_type=account_credentials&account_id={0}", zoomAppAccountId ) )
            };
            var request = new RestRequest
            {
                Method = Method.POST
            };
            request.AddHeader( "Authorization", "Basic " + Convert.ToBase64String( Encoding.ASCII.GetBytes( string.Format( "{0}:{1}", zoomAppClientId, zoomAppClientSecret ) ) ) );
            request.AddHeader( "Host", "zoom.us" );
            var response = client.Execute<OauthTokenResponse>( request );

            if ( response.ErrorException != null )
            {
                const string message = "Error retrieving Oauth token.  Check inner details for more info.";
                var ZoomException = new ApplicationException( message, response.ErrorException );
                throw ZoomException;
            }
            else if ( response.Content.Contains( "error" ) )
            {
                var errorResponse = JsonConvert.DeserializeObject<OauthErrorResponse>( response.Content );
                var message = string.Format( "Error retrieving Oauth token. {0}: {1}", errorResponse.Error, errorResponse.Reason );
                var exception = new ApplicationException( message );
                throw exception;
            }
            tokenString = response.Data.Access_Token;
            return tokenString;
        }

        //Base Execute
        public T Execute<T>( RestRequest request ) where T : new()
        {
            var client = new RestClient
            {
                BaseUrl = new System.Uri( BaseUrl )
            };
            request.AddHeader( "content-type", "application/json" );
            request.AddHeader( "authorization", "Bearer " + _oAuthToken );
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
            if ( pageSize.HasValue )
            {
                request.AddParameter( "page_size", pageSize.Value );
            }
            var result = Execute<ListMeetingsResponse>( request );

            return result.Meetings;
        }

        public Meeting CreateMeeting( string userId, Meeting meetingData )
        {
            var request = new RestRequest
            {
                Resource = string.Format( "users/{0}/meetings", userId ),
                Method = Method.POST
            };
            AddRequestJsonBody( request, meetingData );
            var result = Execute<Meeting>( request );
            return result;
        }

        public bool UpdateMeeting( Meeting meeting, string occurrenceId = null )
        {
            var request = new RestRequest
            {
                Resource = string.Format( "meetings/{0}", meeting.Id ),
                Method = Method.PATCH
            };
            if ( !string.IsNullOrWhiteSpace( occurrenceId ) )
            {
                request.AddParameter( "occurrence_id", occurrenceId );
            }
            AddRequestJsonBody( request, meeting );
            var result = Execute<ZoomBaseResponse>( request );
            return result != null;
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

        public List<ZoomRoom> GetZoomRoomList( RoomStatus? status = null, RoomType? type = null, string locationId = null, string roomName = null, bool unassignedOnly = false, int? pageSize = null, string nextPageToken = null )
        {
            var request = new RestRequest
            {
                Resource = "rooms",
                Method = Method.GET
            };

            if ( status.HasValue )
            {
                request.AddParameter( "status", status.ToString() );
            }
            if ( type.HasValue )
            {
                request.AddParameter( "type", type.ToString() );
            }
            if ( !string.IsNullOrWhiteSpace( locationId ) )
            {
                request.AddParameter( "location_id", locationId );
            }
            if ( !string.IsNullOrWhiteSpace( roomName ) )
            {
                request.AddParameter( "query_name", roomName );
            }
            if ( unassignedOnly )
            {
                request.AddParameter( "unassigned_rooms", bool.TrueString );
            }
            if ( pageSize.HasValue && pageSize.Value > 0 )
            {
                var pageSizeInt = pageSize.Value < 300 ? pageSize.Value : 300;  // Max for api is 300
                request.AddParameter( "page_size", pageSizeInt );
            }
            if ( !string.IsNullOrWhiteSpace( nextPageToken ) )
            {
                request.AddParameter( "next_page_token", nextPageToken );
            }

            var result = Execute<ListRoomsResponse>( request );
            return result.Rooms;
        }

        #endregion Zoom API

        #region Zoom Room API

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

        public bool CancelZoomRoomMeeting( string roomId, string topic, DateTimeOffset startTime, string timezone, int duration, long? meetingNumber = null )
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
            if ( meetingNumber.HasValue )
            {
                reqBody.Params.Meeting_Number = meetingNumber.Value;
            }
            AddRequestJsonBody( request, reqBody );
            var result = Execute<ZRScheduleResponse>( request );
            return result != null;
        }

        #endregion Zoom Room API

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
