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
using Rock;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;
using rocks.kfs.ZoomRoom.Enums;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using ZoomDotNetFramework;
using ZoomDotNetFramework.Entities;
using ZoomDotNetFramework.Responses;
//using rocks.kfs.Eventbrite.Utility.ExtensionMethods;

namespace rocks.kfs.Zoom
{
    public class Zoom
    {
        public static ZoomApi Api()
        {
            return new ZoomApi( Settings.GetJwtString() );
        }

        public static ZoomApi Api( string jwtString )
        {
            return new ZoomApi( jwtString );
        }

        public static bool ZoomAuthCheck()
        {
            var retVar = false;
            var zmUser = new ZoomApi( Settings.GetJwtString() ).GetUser();
            if ( zmUser != null && zmUser.Id.IsNotNullOrWhiteSpace() )
            {
                retVar = true;
            }
            return retVar;
        }

        public Meeting GetZoomMeeting( long meetingId )
        {
            var zoom = Api();
            var meeting = zoom.GetZoomMeeting( meetingId );
            return meeting;
        }

        public Meeting CreateZoomMeeting( string userId, Meeting meetingInfo )
        {
            var zoom = Api();
            var meeting = zoom.CreateMeeting( userId, meetingInfo );
            return meeting;
        }

        public bool UpdateZoomMeeting( Meeting meetingInfo )
        {
            var zoom = Api();
            var success = zoom.UpdateMeeting( meetingInfo );
            return success;
        }

        public bool DeleteZoomMeeting( long meetingId )
        {
            var zoom = Api();
            var success = zoom.DeleteMeeting( meetingId );
            return success;
        }

        public List<Meeting> GetZoomRoomMeetings( string userId, MeetingListType meetingType = MeetingListType.Upcoming  )
        {
            var zoom = Api();
            var meeting = zoom.GetZoomMeetings( userId, meetingType );
            return meeting;
        }

        public static void SyncZoomRoomDT( RockContext rockContext, bool enableLogging = false )
        {
            if ( enableLogging )
            {
                LogEvent( rockContext, "SyncZoom", "Sync Zoom Rooms Defined Type", "Started" );
            }
            var zoom = Api();
            var zrList = zoom.GetZoomRoomList().OrderBy( r => r.Zr_Name );
            if ( enableLogging )
            {
                LogEvent( rockContext, "SyncZoom", "Get Zoom Rooms using Zoom API", string.Format( "zoom.GetZoomRoomList() found {0} rooms.", zrList.Count() ) );
            }

            var zoomRoomDT = new DefinedTypeService( rockContext ).Get( ZoomGuid.DefinedType.ZOOM_ROOM.AsGuid() );

            // Delete DefinedValue for any rooms that no longer exist
            var zoomRoomsToDelete = zoomRoomDT.DefinedValues.Where( v => !zrList.Select( zr => zr.Zr_Id ).Contains( v.Value ) );
            foreach ( var zrDV in zoomRoomsToDelete )
            {
                zoomRoomDT.DefinedValues.Remove( zrDV );
            }

            if ( enableLogging )
            {
                LogEvent( rockContext, "SyncZoom", "Remove Defined Values for Zoom Rooms that no longer exist.", string.Format( "Deleted {0} Zoom Room Defined Value(s).", zoomRoomsToDelete.Count() ) );
            }

            // Add DefinedValue for any new Zoom Rooms
            var newZoomRooms = zrList.Where( r => !zoomRoomDT.DefinedValues.Select( v => v.Value )
                                        .Contains( r.Zr_Id ) )
                                        .Select( r => new DefinedValue
                                        {
                                            Value = r.Zr_Id,
                                            Description = r.Zr_Name
                                        } );
            foreach ( var roomDV in newZoomRooms )
            {
                zoomRoomDT.DefinedValues.Add( roomDV );
            }
            rockContext.SaveChanges();

            if ( enableLogging )
            {
                LogEvent( rockContext, "SyncZoom", "Add Defined Values for new Zoom Rooms.", string.Format( "Added {0} Zoom Room Defined Value(s).", newZoomRooms.Count() ) );
            }
            if ( enableLogging )
            {
                LogEvent( rockContext, "SyncZoom", "Sync Zoom Rooms Defined Type", "Finished" );
            }
        }

        public bool ScheduleZoomRoomMeeting( RockContext rockContext, string roomId, string password, string topic, DateTimeOffset startTime, int duration, bool joinBeforeHost, string timezone = "", bool enableLogging = false, string callbackUrl = "" )
        {

            if ( enableLogging )
            {
                LogEvent( rockContext, "SyncZoom", "Schedule Zoom Room Meeting", "Started" );
            }
            var zoom = Api();
            var success = zoom.ScheduleZoomRoomMeeting( roomId, password, callbackUrl, topic, startTime, timezone, duration, joinBeforeHost: joinBeforeHost );
            if ( enableLogging )
            {
                LogEvent( rockContext, "SyncZoom", "Schedule Zoom Room Meeting", string.Format( "Meeting schedule {0}.", success ? "succeeded" : "failed" ) );
            }
            return success;
        }

        public bool CancelZoomRoomMeeting( RockContext rockContext, string roomId, string topic, DateTimeOffset startTime, string timezone, int duration, bool enableLogging = false )
        {
            if ( enableLogging )
            {
                LogEvent( rockContext, "SyncZoom", "Cancel Zoom Room Meeting", "Started" );
            }
            var zoom = Api();
            var success = zoom.CancelZoomRoomMeeting( roomId, topic, startTime, timezone, duration );
            if ( enableLogging )
            {
                LogEvent( rockContext, "SyncZoom", "Cancel Zoom Room Meeting", string.Format( "Cancel of meeting {0}.", success ? "succeeded" : "failed" ) );
            }
            return success;
        }

        private static AttributeValueCache GetLocationZoomRoomId( Location location )
        {
            AttributeValueCache retVar = null;
            var zoomFieldType = FieldTypeCache.Get( ZoomGuid.FieldType.ZOOM_ROOM.AsGuid() );
            if ( zoomFieldType != null )
            {
                location.LoadAttributes();
                var attribute = location.Attributes.Select( a => a.Value ).FirstOrDefault( a => a.FieldTypeId == zoomFieldType.Id );
                if ( attribute != null )
                {
                    retVar = location.AttributeValues.Select( av => av.Value ).FirstOrDefault( av => av.AttributeId == attribute.Id && av.Value != "" );
                }
            }

            return retVar;
        }

        public static Location GetLocationByZoomRoomId( string zoomRoomId, RockContext rockContext = null )
        {
            Location retVar = null;
            var zoomFieldType = FieldTypeCache.Get( ZoomGuid.FieldType.ZOOM_ROOM.AsGuid() );
            if ( zoomFieldType != null )
            {
                using ( var context = rockContext ?? new RockContext() )
                {
                    var attributeVal = new AttributeValueService( context ).Queryable().FirstOrDefault( av => av.Attribute.FieldTypeId == zoomFieldType.Id && av.Value.Contains( zoomRoomId ) );
                    if ( attributeVal != null )
                    {
                        retVar = new LocationService( context ).Get( attributeVal.EntityId ?? 0 );
                    }
                }
            }

            return retVar;
        }

        private static Location GetLocation( int id )
        {
            var mockQuerystring = new NameValueCollection();
            mockQuerystring.Add( "LocationId", id.ToString() );
            return GetLocationpFromQS( mockQuerystring );
        }

        private static Location GetLocationpFromQS( NameValueCollection qs )
        {
            if ( qs["LocationId"] != null && int.Parse( qs["LocationId"] ) > 0 )
            {
                return new LocationService( new RockContext() ).Get( int.Parse( qs["LocationId"] ) );
            }
            else
            {
                return new Location();
            }
        }

        private static ServiceLog LogEvent( RockContext rockContext, string type, string input, string result )
        {
            var rockLogger = new ServiceLogService( rockContext );
            ServiceLog serviceLog = new ServiceLog
            {
                Name = "Zoom",
                Type = type,
                LogDateTime = RockDateTime.Now,
                Input = input,
                Result = result,
                Success = true
            };
            rockLogger.Add( serviceLog );
            rockContext.SaveChanges();
            return serviceLog;
        }

        public static bool LinkZoomRoomLocation( Location location, string zoomRoomId )
        {
            var retVar = false;
            var zoomFieldType = FieldTypeCache.Get( ZoomGuid.FieldType.ZOOM_ROOM.AsGuid() );
            if ( zoomFieldType != null )
            {
                location.LoadAttributes();
                var attribute = location.Attributes.Select( a => a.Value ).FirstOrDefault( a => a.FieldTypeId == zoomFieldType.Id );
                if ( attribute != null )
                {
                    using ( var rockContext = new RockContext() )
                    {
                        location.SetAttributeValue( attribute.Key, zoomRoomId );
                        location.SaveAttributeValue( attribute.Key, rockContext );
                        retVar = true;
                    }
                }
            }
            return retVar;
        }

        public static bool LinkZoomRoomLocation( int locationId, string zoomRoomId )
        {
            var location = GetLocation( locationId );
            return LinkZoomRoomLocation( location, zoomRoomId );
        }

        public static void UnlinkZoomRoomLocation( int locationId )
        {
            var location = GetLocation( locationId );
            UnlinkZoomRoomLocation( location );
        }

        public static void UnlinkZoomRoomLocation( Location location )
        {
            var locationZoomRoomIDAttr = GetLocationZoomRoomId( location );

            using ( var rockContext = new RockContext() )
            {
                location.SetAttributeValue( locationZoomRoomIDAttr.AttributeKey, string.Empty );
                location.SaveAttributeValue( locationZoomRoomIDAttr.AttributeKey, rockContext );
            }
        }
    }
}