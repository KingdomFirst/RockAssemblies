using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arena.Core;
using Arena.Custom.KFS.ArenaData;
using Arena.Custom.KFS.Eventbrite.Entities;

namespace EventBriteDotNetFramework
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

        public List<ArenaEventbriteEvent> Events()
        {
            var retVar = new List<ArenaEventbriteEvent>();
            IQueryable<core_profile> AllEBEvents;
            IQueryable<core_profile> FilteredEvents;

            using ( var context = ContextHelper.GetArenaContext() )
            {
                AllEBEvents = context.core_profile.Where( cp => cp.foreign_key.Substring( 0, 2 ).Equals( "EB" ) );


                FilteredEvents = AllEBEvents;
                if ( _person != null )
                {
                    FilteredEvents = FilteredEvents.Where( e => e.owner_id.Equals( _person.PersonID ) );
                }
                foreach ( var fe in FilteredEvents.ToList() )
                {
                    retVar.Add( new ArenaEventbriteEvent( fe.profile_id ) );
                }
            }
            return retVar;
        }

    }
}
