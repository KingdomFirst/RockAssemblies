using System;
using System.Collections.Generic;


namespace EventBriteDotNetFramework.Entities
{
    public class Order
    {
        public DateTime Created { get; set; }
        public DateTime Changed { get; set; }
        public string Name { get; set; }
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public string Email { get; set; }
        public OrderCosts Costs { get; set; }
        public string Event_Id { get; set; }
        public string Time_Remaining { get; set; }
        public string Resource_Uri { get; set; }
        public long Id { get; set; }
        public string Status { get; set; }
        public List<Attendee> Attendees { get; set; }
    }
}
