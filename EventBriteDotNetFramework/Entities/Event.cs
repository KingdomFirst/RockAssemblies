using System;
using System.Linq;
using System.Collections.Generic;

namespace EventBriteDotNetFramework.Entities
{
    public class Event
    {
        public string ResourceUri { get; set; }

        public HybridString Name { get; set; }

        public HybridString Description { get; set; }

        public Organizer Organizer { get; set; }

        public Venue Venue { get; set; }

        public List<TicketClass> TicketClasses { get; set; }

        public long Id { get; set; }

        public string Url { get; set; }

        public HybridDate Start { get; set; }

        public HybridDate End { get; set; }

        public DateTime Created { get; set; }

        public DateTime Changed { get; set; }

        public string Timezone { get; set; }

        public int Capacity { get; set; }

        public object[] Categories { get; set; }

        public string Status { get; set; }
        public bool ArenaRSVPEvent( string oAuthToken )
        {
            var retVar = false;
            var eb = new EBApi( oAuthToken );
            var cq = eb.GetEventCannedQuestionsById( Id );
            var test = cq.Questions.FirstOrDefault( q => q.Respondent.Equals( "attendee", StringComparison.CurrentCultureIgnoreCase ) );
            if ( test == null )
            {
                retVar = true;
            }
            return retVar;
        }
    }
}