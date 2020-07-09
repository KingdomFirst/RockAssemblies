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
using EventbriteDotNetFramework.Entities;
using Rock.Model;

namespace EventbriteDotNetFramework
{
    public class EventbriteEvents
    {
        private Person _person = null;
        private string _oauthtoken = null;

        public EventbriteEvents() { }

        public EventbriteEvents( Person owner )
        {
            _person = owner;
        }

        public List<RockEventbriteEvent> Events()
        {
            var retVar = new List<RockEventbriteEvent>();
            //IQueryable<core_profile> AllEBEvents;
            //IQueryable<core_profile> FilteredEvents;

            //using ( var context = ContextHelper.GetArenaContext() )
            //{
            //    AllEBEvents = context.core_profile.Where( cp => cp.foreign_key.Substring( 0, 2 ).Equals( "EB" ) );


            //    FilteredEvents = AllEBEvents;
            //    if ( _person != null )
            //    {
            //        FilteredEvents = FilteredEvents.Where( e => e.owner_id.Equals( _person.PersonID ) );
            //    }
            //    foreach ( var fe in FilteredEvents.ToList() )
            //    {
            //        retVar.Add( new ArenaEventbriteEvent( fe.profile_id ) );
            //    }
            //}
            return retVar;
        }

    }
}
