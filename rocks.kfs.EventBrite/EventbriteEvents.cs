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
using EventbriteDotNetFramework.Entities;
using Rock;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;

namespace EventbriteDotNetFramework
{
    public class EventbriteEvents
    {
        private Person _person = null;
        private string _oauthtoken = null;

        public EventbriteEvents() { }

        public EventbriteEvents( Person owner )
        {
            _person = owner;
        }

        public List<RockEventbriteEvent> Events()
        {
            var retVar = new List<RockEventbriteEvent>();
            var ebFieldType = FieldTypeCache.Get( rocks.kfs.Eventbrite.EBGuid.FieldType.EVENTBRITE_EVENT.AsGuid() );
            if ( ebFieldType != null )
            {
                using ( RockContext rockContext = new RockContext() )
                {
                    var attributeValues = new AttributeValueService( rockContext ).Queryable().Where( av => av.Attribute.FieldTypeId == ebFieldType.Id && av.EntityId.HasValue && av.Value != "" ).Select( av => av.EntityId.Value );
                    var groups = new GroupService( rockContext ).GetListByIds( attributeValues.ToList() );
                    foreach ( var group in groups )
                    {
                        retVar.Add( new RockEventbriteEvent( group ) );
                    }
                }
            }
            return retVar;
        }

    }
}
