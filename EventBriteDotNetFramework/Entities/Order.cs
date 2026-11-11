// <copyright>
// Copyright 2026 by Kingdom First Solutions
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
using RestSharp;
using RestSharp.Deserializers;

namespace EventbriteDotNetFramework.Entities
{
    public class Order
    {
        private long _eventId;
        private Event _event;

        public DateTime Created { get; set; }
        public DateTime Changed { get; set; }
        public string Name { get; set; }
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public string Email { get; set; }
        public OrderCosts Costs { get; set; }

        /// <summary>
        /// Gets or sets the Eventbrite id of the event this order belongs to. When the response has no
        /// "event_id", the id is read from <see cref="EventReference"/>.
        /// </summary>
        public long Event_Id
        {
            get
            {
                if ( _eventId != 0 )
                {
                    return _eventId;
                }

                return GetEventIdFromReference();
            }
            set
            {
                _eventId = value;
            }
        }

        /*
            11/9/2026

            Eventbrite now returns an order's "event" as the event id (a string) unless the request
            expands "event", and no longer returns "event_id". RestSharp throws an InvalidCastException
            when a class-typed property receives a string, which broke every order request. The raw
            value is kept as an object, which accepts either shape: Event_Id reads the id from it, and
            Event converts it to a typed Event when the request expanded "event". Event has no setter
            so that RestSharp never maps "event" into it directly.

            Reason: Eventbrite API change broke order deserialization.
        */

        /// <summary>
        /// Gets or sets the raw "event" value from Eventbrite: the event id as a string, or a dictionary
        /// of the event's fields when the request expands "event".
        /// </summary>
        [DeserializeAs( Name = "event" )]
        public object EventReference { get; set; }

        /// <summary>
        /// Gets the event this order belongs to when the request expanded "event"; otherwise null, and
        /// the event can be loaded with EBApi.GetEventById( Event_Id ).
        /// </summary>
        public Event Event
        {
            get
            {
                if ( _event == null )
                {
                    _event = GetEventFromReference();
                }

                return _event;
            }
        }

        public string Time_Remaining { get; set; }
        public string Resource_Uri { get; set; }
        public long Id { get; set; }
        public string Status { get; set; }
        public List<Attendee> Attendees { get; set; }

        /// <summary>
        /// Gets the event id from <see cref="EventReference"/>, whichever shape Eventbrite returned it in.
        /// </summary>
        /// <returns>The event id, or 0 when it is missing or not a number.</returns>
        private long GetEventIdFromReference()
        {
            var eventValue = EventReference;
            var eventFields = EventReference as IDictionary<string, object>;

            if ( eventFields != null )
            {
                eventValue = eventFields.ContainsKey( "id" ) ? eventFields["id"] : null;
            }

            long eventId;
            if ( eventValue == null || !long.TryParse( eventValue.ToString(), out eventId ) )
            {
                return 0;
            }

            return eventId;
        }

        /// <summary>
        /// Converts an expanded <see cref="EventReference"/> to a typed event, using the same RestSharp
        /// deserializer as every other Eventbrite response.
        /// </summary>
        /// <returns>The event, or null when "event" was not expanded.</returns>
        private Event GetEventFromReference()
        {
            var eventFields = EventReference as IDictionary<string, object>;

            if ( eventFields == null )
            {
                return null;
            }

            var eventResponse = new RestResponse { Content = SimpleJson.SerializeObject( eventFields ) };

            return new JsonDeserializer().Deserialize<Event>( eventResponse );
        }
    }
}
