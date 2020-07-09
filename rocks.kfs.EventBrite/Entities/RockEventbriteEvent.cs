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


namespace EventbriteDotNetFramework.Entities
{
    public class RockEventbriteEvent
    {
        //private Event _ebevent;
        //private EventProfile _arenaevent;
        //private int _arenaeventid;
        //private EBApi _eb;

        public RockEventbriteEvent( int ArenaEventId )
        {
            //LoadArenaEvent( ArenaEventId );
            //if ( EBApi.EBLinkCheck( ArenaEventId ) )
            //{
            //    LoadEventbriteEvent();
            //}
        }

        //public Event EventbriteEvent
        //{
        //    get
        //    {
        //        return _ebevent;
        //    }
        //}
        //public EventProfile ArenaEvent
        //{
        //    get
        //    {
        //        return _arenaevent;
        //    }
        //}
        //public string oAuthToken
        //{
        //    get
        //    {
        //        return EBApi.GetEBoAuth( _arenaevent.ProfileID );
        //    }
        //}
        //public string ArenaEventName
        //{
        //    get
        //    {
        //        return _arenaevent.Title;
        //    }
        //}
        //public string EventbriteEventName
        //{
        //    get
        //    {
        //        return _ebevent.Name.Text;
        //    }
        //}
        //public int ArenaEventId
        //{
        //    get
        //    {
        //        return _arenaevent.ProfileID;
        //    }
        //}
        //public string LastSynced
        //{
        //    get
        //    {
        //        var sync = _arenaevent.CustomFieldValues.FirstOrDefault( cf => cf.Guid.Equals( new Guid( "7F95AD8D-A58C-4E8B-B7F6-291571BBD7F6" ) ) );
        //        if ( sync != null && sync.SelectedValue != null )
        //        {
        //            return _arenaevent.CustomFieldValues.FirstOrDefault( cf => cf.Guid.Equals( new Guid( "7F95AD8D-A58C-4E8B-B7F6-291571BBD7F6" ) ) ).SelectedValue;
        //        }
        //        else
        //        {
        //            return "Never";
        //        }


        //    }
        //}
        //private void LoadArenaEvent( int id )
        //{
        //    _arenaevent = new EventProfile( id );
        //    _arenaeventid = _arenaevent.ProfileID;
        //}
        //private void LoadEventbriteEvent()
        //{
        //    if ( _arenaevent == null )
        //    {
        //        return;
        //    }
        //    _eb = new EBApi( EBApi.GetEBoAuth( _arenaevent.ProfileID ) );
        //    var ebevents = _eb.GetOwnedEvents();
        //    var testId = new long();
        //    long.TryParse( _arenaevent.ForiegnKey.Substring( 2 ), out testId );
        //    foreach ( var evnt in ebevents.Events )
        //    {
        //        if ( evnt.Id == testId )
        //        {
        //            _ebevent = evnt;
        //        }
        //    }
        //}
    }
}
