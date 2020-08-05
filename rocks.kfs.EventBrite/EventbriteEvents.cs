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
using System.Collections.Generic;
using System.Linq;
using EventbriteDotNetFramework.Entities;
using Rock;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;

namespace rocks.kfs.Eventbrite
{
    public class EventbriteEvents
    {
        public List<RockEventbriteEvent> Events( bool loadEventbriteEvent = false )
        {
            var retVar = new List<RockEventbriteEvent>();
            var ebFieldType = FieldTypeCache.Get( EBGuid.FieldType.EVENTBRITE_EVENT.AsGuid() );
            if ( ebFieldType != null )
            {
                using ( RockContext rockContext = new RockContext() )
                {
                    var attributes = new AttributeService( rockContext ).GetByFieldTypeId( ebFieldType.Id ).Select( a => a.Id );
                    var attributeValues = new AttributeValueService( rockContext ).Queryable().Where( av => attributes.Contains( av.AttributeId ) && av.EntityId.HasValue && av.Value != "" ).Select( av => av.EntityId.Value );
                    var groups = new GroupService( rockContext ).GetListByIds( attributeValues.ToList() );
                    foreach ( var group in groups )
                    {
                        retVar.Add( new RockEventbriteEvent( group, loadEventbriteEvent ) );
                    }
                }
            }
            return retVar;
        }
    }
}
