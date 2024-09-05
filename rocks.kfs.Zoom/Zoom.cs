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
using Rock;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using ZoomDotNetFramework;
using ZoomDotNetFramework.Entities;
using ZoomDotNetFramework.Enums;

namespace rocks.kfs.Zoom
{
    public class Zoom
    {
        public static ZoomApi Api()
        {
            return new ZoomApi( Settings.GetOauthString() );
        }

        public static ZoomApi Api( string oAuthString )
        {
            return new ZoomApi( oAuthString );
        }

        public static bool ZoomAuthCheck()
        {
            var retVar = false;
            var zmUser = new ZoomApi( Settings.GetOauthString() ).GetUser();
            if ( zmUser != null && zmUser.Id.IsNotNullOrWhiteSpace() )
            {
                retVar = true;
            }
            return retVar;
        }

        public static void SyncZoomRoomDT( RockContext rockContext = null, bool enableLogging = false )
        {
            if ( rockContext == null )
            {
                rockContext = new RockContext();
            }
            if ( enableLogging )
            {
                LogEvent( rockContext, "SyncZoom", "Sync Zoom Rooms Defined Type", "Started" );
            }
            var zoom = Api();
            var zrList = zoom.GetZoomRoomList( type: RoomType.ZoomRoom )?.OrderBy( r => r.Name )?.ToList();
            if ( zrList == null )
            {
                zrList = new List<ZoomRoom>();
            }
            if ( enableLogging )
            {
                LogEvent( rockContext, "SyncZoom", "Get Zoom Rooms using Zoom API", string.Format( "zoom.GetZoomRoomList() found {0} rooms.", zrList.Count() ) );
            }

            var zoomRoomDT = new DefinedTypeService( rockContext ).Get( ZoomGuid.DefinedType.ZOOM_ROOM.AsGuid() );
            var dvService = new DefinedValueService( rockContext );
            var zoomRoomDV = dvService.Queryable().Where( v => v.DefinedTypeId == zoomRoomDT.Id );

            // Delete DefinedValue for any rooms that no longer exist
            var zrIds = zrList.Select( zr => zr.Id ).ToList();
            var zoomRoomsToDelete = zoomRoomDV.Where( v => !zrIds.Contains( v.Value ) );
            dvService.DeleteRange( zoomRoomsToDelete );

            if ( enableLogging )
            {
                LogEvent( null, "SyncZoom", "Remove Defined Values for Zoom Rooms that no longer exist.", string.Format( "Deleted {0} Zoom Room Defined Value(s).", zoomRoomsToDelete.Count() ) );
            }

            // Add DefinedValue for any new Zoom Rooms
            var newZoomRooms = zrList.Where( r => !zoomRoomDT.DefinedValues.Select( v => v.Value ).Contains( r.Id ) )
                                        .Select( r => new DefinedValue
                                        {
                                            Value = r.Id,
                                            Description = r.Name
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

        private static ServiceLog LogEvent( RockContext rockContext, string type, string input, string result )
        {
            if ( rockContext == null )
            {
                rockContext = new RockContext();
            }
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
    }
}