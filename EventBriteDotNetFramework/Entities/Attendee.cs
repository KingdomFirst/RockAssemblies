using System;
using System.Collections.Generic;
using System.Linq;

namespace EventBriteDotNetFramework.Entities
{
    public class Attendee
    {
        public string Team { get; set; }
        public List<OrderCosts> Costs { get; set; }
        public string Resource_Uri { get; set; }
        public string Id { get; set; }
        public DateTime Changed { get; set; }
        public DateTime Created { get; set; }
        public int Quantity { get; set; }
        public Profile Profile { get; set; }
        public bool Checked_In { get; set; }
        public bool Cancelled { get; set; }
        public bool Refunded { get; set; }
        public string Status { get; set; }
        public string Ticket_Class_Name { get; set; }
        public long Event_Id { get; set; }
        public long Order_Id { get; set; }
        public long Ticket_Class_Id { get; set; }
        public Dictionary<int, string> ArenaStatus
        {
            get
            {
                var retVar = new Dictionary<int, string>();
                var lookupid = -1;
                var statusnote = Status;
                var lookups = new Core.LookupCollection( Guid.Parse( "705F785D-36DB-4BF2-9C35-2A7F72A55731" ) );

                switch ( Status )
                {
                    case "Attending":
                        lookupid = lookups.FirstOrDefault( l => l.Qualifier == "A" ).LookupID;
                        break;
                    case "Deleted":
                        lookupid = lookups.FirstOrDefault( l => l.Qualifier == "D" ).LookupID;
                        statusnote = "Deleted on Eventbrite";
                        break;
                    case "Not Attending":
                    case "Refunded":
                        lookupid = lookups.FirstOrDefault( l => l.Qualifier == "D" ).LookupID;
                        statusnote = "Refunded on Eventbrite";
                        break;
                    default:
                        lookupid = lookups.FirstOrDefault( l => l.Qualifier == "D" ).LookupID;
                        statusnote = "Unknown";
                        break;
                }

                retVar.Add( lookupid, statusnote );

                return retVar;
            }
        }
    }
}
