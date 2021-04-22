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
using System.Linq;
using Rock;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;
using rocks.kfs.Eventbrite;
using rocks.kfs.Eventbrite.Utility.ExtensionMethods;

namespace EventbriteDotNetFramework.Entities
{
    public class RockEventbriteEvent
    {
        private Event _ebevent;
        private Group _rockGroup;
        private int _rockGroupId;
        private EBApi _eb;
        private RockContext _rockContext;
        private long _evntId;
        private string _synced;

        public RockEventbriteEvent( int GroupId, bool loadEventbriteEvent = false )
        {
            _rockContext = new RockContext();
            LoadRockGroup( GroupId );
            if ( loadEventbriteEvent && Eventbrite.EBUsageCheck( GroupId ) )
            {
                LoadEventbriteEvent();
            }
        }
        public RockEventbriteEvent( Group group, bool loadEventbriteEvent = false )
        {
            _rockContext = new RockContext();
            LoadRockGroup( group );
            if ( loadEventbriteEvent && Eventbrite.EBUsageCheck( group.Id ) )
            {
                LoadEventbriteEvent();
            }
        }

        public Event EventbriteEvent
        {
            get
            {
                return _ebevent;
            }
        }
        public Group RockGroup
        {
            get
            {
                return _rockGroup;
            }
        }

        public string RockGroupName
        {
            get
            {
                return _rockGroup.Name;
            }
        }
        public string EventbriteEventName
        {
            get
            {
                if ( _ebevent != null )
                {
                    return _ebevent.Name.Text;
                }
                return string.Empty;
            }
        }
        public int RockGroupId
        {
            get
            {
                return _rockGroup.Id;
            }
        }
        public long EventbriteEventId
        {
            get
            {
                return _evntId;
            }
        }
        public string LastSynced
        {
            get
            {
                if ( _synced.IsNotNullOrWhiteSpace() )
                {
                    return _synced;
                }
                else
                {
                    return "Never";
                }


            }
        }
        private void LoadRockGroup( Group group )
        {
            _rockGroup = group;
            _rockGroupId = _rockGroup.Id;

            var ebFieldType = FieldTypeCache.Get( rocks.kfs.Eventbrite.EBGuid.FieldType.EVENTBRITE_EVENT.AsGuid() );
            if ( ebFieldType != null )
            {
                _rockGroup.LoadAttributes( _rockContext );
                var attribute = _rockGroup.Attributes.Select( a => a.Value ).FirstOrDefault( a => a.FieldTypeId == ebFieldType.Id );
                if ( attribute != null )
                {
                    var attributeVal = _rockGroup.AttributeValues.Select( av => av.Value ).FirstOrDefault( av => av.AttributeId == attribute.Id && av.Value != "" );
                    if ( attributeVal != null )
                    {
                        var splitVal = attributeVal.Value.SplitDelimitedValues( "^" );
                        if ( splitVal.Length > 0 )
                        {
                            _evntId = splitVal[0].AsLong();
                        }
                        if ( splitVal.Length > 1 )
                        {
                            _synced = splitVal[1];
                        }
                    }
                }
            }
        }
        private void LoadRockGroup( int id )
        {
            var group = new GroupService( _rockContext ).Get( id );
            LoadRockGroup( group );
        }
        private void LoadEventbriteEvent()
        {
            if ( _rockGroup == null )
            {
                return;
            }
            if ( _evntId > 0 )
            {
                _eb = new EBApi( Settings.GetAccessToken(), Settings.GetOrganizationId().ToLong( 0 ) );
                _ebevent = _eb.GetEventById( _evntId );
            }
        }
    }
}
