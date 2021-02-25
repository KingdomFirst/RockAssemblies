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
using System.Linq;
using System.Collections.Generic;

namespace EventbriteDotNetFramework.Entities
{
    public class Event
    {
        public string ResourceUri { get; set; }

        public HybridString Name { get; set; }

        public string Summary { get; set; }

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

        public bool IsRSVPEvent( EBApi eb )
        {
            var retVar = false;
            var cq = eb.GetEventCannedQuestionsById( Id );
            var test = cq.Questions.FirstOrDefault( q => q.Respondent.Equals( "attendee", StringComparison.CurrentCultureIgnoreCase ) );
            if ( test == null )
            {
                retVar = true;
            }
            return retVar;
        }

        public bool IsRSVPEvent( string oAuthToken )
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