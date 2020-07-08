using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventBriteDotNetFramework.Entities;
using EventBriteDotNetFramework.Responses;


namespace EventBriteDotNetFramework.Responses
{
    public class EventOrders
    {
        public Pagination Pagination { get; set;}
        public List<Order> Orders { get; set; }
    }
}
