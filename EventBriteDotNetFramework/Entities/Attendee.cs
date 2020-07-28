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

namespace EventbriteDotNetFramework.Entities
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
        public Event Event { get; set; }
        public long Order_Id { get; set; }
        public Order Order { get; set; }
        public long Ticket_Class_Id { get; set; }
    }
}
