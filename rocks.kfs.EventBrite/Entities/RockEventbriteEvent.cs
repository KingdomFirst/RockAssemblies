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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rock;
using Rock.Model;
using Rock.Web.Cache;
using rocks.kfs.Eventbrite;

namespace EventbriteDotNetFramework.Entities
{
    public class RockEventbriteEvent
    {
        private Event _ebevent;
        private Group _rockGroup;
        private int _rockGroupId;
        private EBApi _eb;

        public RockEventbriteEvent( int GroupId )
        {
            LoadRockGroup( GroupId );
            if ( Eventbrite.EBUsageCheck( GroupId ) )
            {
                LoadEventbriteEvent();
            }
        }
        public RockEventbriteEvent( Group group )
        {
            LoadRockGroup( group );
            if ( Eventbrite.EBUsageCheck( group.Id ) )
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
                if (_ebevent != null)
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
        public string LastSynced
        {
            get
            {
                var sync = "";
                var ebFieldType = FieldTypeCache.Get( rocks.kfs.Eventbrite.EBGuid.FieldType.EVENTBRITE_EVENT.AsGuid() );
                if ( ebFieldType != null )
                {
                    _rockGroup.LoadAttributes();
                    var attribute = _rockGroup.Attributes.Select( a => a.Value ).FirstOrDefault( a => a.FieldTypeId == ebFieldType.Id );
                    if ( attribute != null )
                    {
                        var attributeVal = _rockGroup.AttributeValues.Select( av => av.Value ).FirstOrDefault( av => av.AttributeId == attribute.Id && av.Value != "" );
                        if ( attributeVal != null )
                        {
                            var splitVal = attributeVal.Value.SplitDelimitedValues( "^" );
                            if ( splitVal.Length > 1 )
                            {
                                sync = splitVal[1];
                            }
                        }
                    }
                }
                if ( sync.IsNotNullOrWhiteSpace() )
                {
                    return sync;
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
        }
        private void LoadRockGroup( int id )
        {
            _rockGroup = new GroupService( new Rock.Data.RockContext() ).Get( id );
            _rockGroupId = _rockGroup.Id;
        }
        private void LoadEventbriteEvent()
        {
            if ( _rockGroup == null )
            {
                return;
            }
            _eb = new EBApi( Settings.GetAccessToken() );
            var ebevents = _eb.GetOrganizationEvents();
            var testId = new long();
            var ebFieldType = FieldTypeCache.Get( rocks.kfs.Eventbrite.EBGuid.FieldType.EVENTBRITE_EVENT.AsGuid() );
            if ( ebFieldType != null )
            {
                _rockGroup.LoadAttributes();
                var attribute = _rockGroup.Attributes.Select( a => a.Value ).FirstOrDefault( a => a.FieldTypeId == ebFieldType.Id );
                if ( attribute != null )
                {
                    var attributeVal = _rockGroup.AttributeValues.Select( av => av.Value ).FirstOrDefault( av => av.AttributeId == attribute.Id && av.Value != "" );
                    if ( attributeVal != null )
                    {
                        long.TryParse( attributeVal.Value.SplitDelimitedValues( "^" )[0], out testId );
                    }
                }
            }
            foreach ( var evnt in ebevents.Events )
            {
                if ( evnt.Id == testId )
                {
                    _ebevent = evnt;
                }
            }
        }
    }
}
