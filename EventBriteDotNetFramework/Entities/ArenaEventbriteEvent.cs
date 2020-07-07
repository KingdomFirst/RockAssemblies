using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arena.Event;


namespace EventBriteDotNetFramework.Entities
{
    public class ArenaEventbriteEvent
    {
        private Event _ebevent;
        private EventProfile _arenaevent;
        private int _arenaeventid;
        private EBApi _eb;

        public ArenaEventbriteEvent(int ArenaEventId)
        {
            LoadArenaEvent(ArenaEventId);
            if(EBApi.EBLinkCheck(ArenaEventId))
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
        public EventProfile ArenaEvent
        {
            get
            {
                return _arenaevent;
            }
        }
        public string oAuthToken
        {
            get
            {
                return EBApi.GetEBoAuth( _arenaevent.ProfileID );
            }
        }
        public string ArenaEventName
        {
            get
            {
                return _arenaevent.Title;
            }
        }
        public string EventbriteEventName
        {
            get
            {
                return _ebevent.Name.Text;
            }
        }
        public int ArenaEventId
        {
            get
            {
                return _arenaevent.ProfileID;
            }
        }
        public string LastSynced
        {
            get
            {
                var sync = _arenaevent.CustomFieldValues.FirstOrDefault( cf => cf.Guid.Equals( new Guid( "7F95AD8D-A58C-4E8B-B7F6-291571BBD7F6" ) ) );
                if (sync != null && sync.SelectedValue != null )
                {
                    return _arenaevent.CustomFieldValues.FirstOrDefault( cf => cf.Guid.Equals( new Guid( "7F95AD8D-A58C-4E8B-B7F6-291571BBD7F6" ) ) ).SelectedValue;
                }
                else
                {
                    return "Never";
                }
                    
            
            }
        }
        private void LoadArenaEvent(int id)
        {
            _arenaevent = new EventProfile( id );
            _arenaeventid = _arenaevent.ProfileID;
        }
        private void LoadEventbriteEvent()
        {
            if(_arenaevent==null)
            {
                return;
            }
            _eb = new EBApi( EBApi.GetEBoAuth( _arenaevent.ProfileID ) );
            var ebevents = _eb.GetOwnedEvents();
            var testId = new long();
            long.TryParse(_arenaevent.ForiegnKey.Substring( 2 ),out testId);
            foreach(var evnt in ebevents.Events)
            {
                if(evnt.Id == testId)
                {
                    _ebevent = evnt;
                }
            }
        }
    }
}
