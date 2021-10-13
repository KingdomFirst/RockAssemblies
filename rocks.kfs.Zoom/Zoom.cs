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
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ZoomDotNetFramework;
using ZoomDotNetFramework.Entities;
using ZoomDotNetFramework.Responses;
using Rock;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;
using rocks.kfs.Zoom.Utility.ExtensionMethods;
//using rocks.kfs.Eventbrite.Utility.ExtensionMethods;

namespace rocks.kfs.Zoom
{
    public class Zoom
    {
        private static int recordTypePersonId = DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.PERSON_RECORD_TYPE_PERSON.AsGuid() ).Id;
        private static int recordStatusPendingId = DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.PERSON_RECORD_STATUS_PENDING.AsGuid() ).Id;
        private static int homePhoneValueId = DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.PERSON_PHONE_TYPE_HOME ).Id;
        private static int homeLocationValueId = DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.GROUP_LOCATION_TYPE_HOME ).Id;
        private static int mobilePhoneValueId = DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.PERSON_PHONE_TYPE_MOBILE ).Id;
        private static int workPhoneValueId = DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.PERSON_PHONE_TYPE_WORK ).Id;

        public static ZoomApi Api()
        {
            return new ZoomApi( Settings.GetJwtString() );
        }

        public static ZoomApi Api( string jwtString )
        {
            return new ZoomApi( jwtString );
        }

        public static void SyncZoomRoomDT()
        {
            var rockContext = new RockContext();
            var zoomRoomDT = new DefinedTypeService( rockContext ).Get( ZoomGuid.DefinedType.ZOOM_ROOM.AsGuid() );
            zoomRoomDT.DefinedValues.Clear();
            var zoom = Api();
            var zrList = zoom.GetZoomRooms().OrderBy( r => r.Name );
            foreach( var room in zrList )
            {
                zoomRoomDT.DefinedValues.Add( new DefinedValue
                {
                    Value = room.Id,
                    Description = room.Name
                } );
            }
            rockContext.SaveChanges();
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

        //public static bool EBoAuthCheck()
        //{
        //    var retVar = false;
        //    var getUser = new EBApi( Settings.GetAccessToken() ).GetUser();
        //    if ( getUser != null && getUser.Id != 0 )
        //    {
        //        retVar = true;
        //    }
        //    return retVar;
        //}

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