// <copyright>
// Copyright 2022 by Kingdom First Solutions
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