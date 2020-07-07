using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arena.Custom.KFS.Eventbrite.Entities;
using Arena.Custom.KFS.Eventbrite.Responses;


namespace EventBriteDotNetFramework.Responses
{
    public class EventOrders
    {
        public Pagination Pagination { get; set;}
        public List<Order> Orders { get; set; }
    }
}
