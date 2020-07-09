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
using System.Text;
using System.Threading.Tasks;

namespace rocks.kfs.Eventbrite.Entities
{
    public class Attendee : EventbriteDotNetFramework.Entities.Attendee
    {
        public Dictionary<int, string> ArenaStatus
        {
            get
            {
                var retVar = new Dictionary<int, string>();
                var lookupid = -1;
                var statusnote = Status;
                //var lookups = new Core.LookupCollection( Guid.Parse( "705F785D-36DB-4BF2-9C35-2A7F72A55731" ) );

                //switch ( Status )
                //{
                //    case "Attending":
                //        lookupid = lookups.FirstOrDefault( l => l.Qualifier == "A" ).LookupID;
                //        break;
                //    case "Deleted":
                //        lookupid = lookups.FirstOrDefault( l => l.Qualifier == "D" ).LookupID;
                //        statusnote = "Deleted on Eventbrite";
                //        break;
                //    case "Not Attending":
                //    case "Refunded":
                //        lookupid = lookups.FirstOrDefault( l => l.Qualifier == "D" ).LookupID;
                //        statusnote = "Refunded on Eventbrite";
                //        break;
                //    default:
                //        lookupid = lookups.FirstOrDefault( l => l.Qualifier == "D" ).LookupID;
                //        statusnote = "Unknown";
                //        break;
                //}

                //retVar.Add( lookupid, statusnote );

                return retVar;
            }
        }
    }
}
