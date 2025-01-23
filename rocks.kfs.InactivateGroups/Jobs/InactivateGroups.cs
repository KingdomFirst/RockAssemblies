// <copyright>
// Copyright 2019 by Kingdom First Solutions
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
using System.Data.Entity;
using System.Linq;

using Quartz;

using Rock;
using Rock.Attribute;
using Rock.Data;
using Rock.Jobs;
using Rock.Model;
using Rock.Web.Cache;

namespace rocks.kfs.InactivateGroups.Jobs
{
    [GroupTypeField( "Group Type to look for attributes on", "Limit job to run on a specific group type.", true, order: 0, key: "LimitGroupType" )]
    [AttributeField( Rock.SystemGuid.EntityType.GROUP, "Start Date Attribute", "Group Attribute of the start date for group.", true, false, "", "Group Attributes", 1, "StartDateAttribute" )]
    [AttributeField( Rock.SystemGuid.EntityType.GROUP, "End Date Attribute", "Group Attribute of the end date for the group..", true, false, "", "Group Attributes", 2, "EndDateAttribute" )]

    /// <summary>
    /// Job to inactivate groups based on group attribute values.
    /// </summary>
    [DisallowConcurrentExecution]
    public class InactivateGroups : RockJob
    {
        /// <summary>
        /// Empty constructor for job initialization
        /// <para>
        /// Jobs require a public empty constructor so that the
        /// scheduler can instantiate the class whenever it needs.
        /// </para>
        /// </summary>
        public InactivateGroups()
        {
        }

        /// <summary>
        /// Job to inactivate groups based on group attribute values.
        /// </summary>
        public override void Execute()
        {
            var rockContext = new RockContext();
            var groupService = new GroupService( rockContext );
            var groupsInactivated = 0;
            var groupsActivated = 0;

            // Get Job Settings
            Guid? StartDateAttributeGuid = GetAttributeValue( "StartDateAttribute" ).AsGuidOrNull();
            if ( !StartDateAttributeGuid.HasValue )
            {
                return;
            }
            var StartDateAttribute = AttributeCache.Get( StartDateAttributeGuid.Value, rockContext );

            Guid? EndDateAttributeGuid = GetAttributeValue( "EndDateAttribute" ).AsGuidOrNull();
            if ( !EndDateAttributeGuid.HasValue )
            {
                return;
            }
            var EndDateAttribute = AttributeCache.Get( EndDateAttributeGuid.Value, rockContext );

            Guid groupTypeGuid = GetAttributeValue( "LimitGroupType" ).AsGuid();

            var groupQryActive = groupService
                .Queryable()
                .Where( g => g.IsActive && g.GroupType.Guid.Equals( groupTypeGuid ) );

            var groupQryNotActive = groupService
                .Queryable()
                .Where( g => !g.IsActive && g.GroupType.Guid.Equals( groupTypeGuid ) );

            var groupListActive = groupQryActive.AsNoTracking().ToList();
            var groupListNotActive = groupQryNotActive.AsNoTracking().ToList();

            var JobStartDateTime = RockDateTime.Now;

            foreach ( var group in groupListActive )
            {
                group.LoadAttributes();

                var startDateValue = DateTime.MinValue;
                DateTime.TryParse( group.AttributeValues[StartDateAttribute.Key].Value, out startDateValue );

                var endDateValue = DateTime.MinValue;
                DateTime.TryParse( group.AttributeValues[EndDateAttribute.Key].Value, out endDateValue );

                if ( ( startDateValue != DateTime.MinValue && startDateValue > JobStartDateTime ) || ( endDateValue != DateTime.MinValue && endDateValue <= JobStartDateTime ) )
                {
                    var _group = groupService.Get( group.Id );
                    _group.IsActive = false;
                    groupsInactivated++;
                }
            }

            foreach ( var group in groupListNotActive )
            {
                group.LoadAttributes();

                var startDateValue = DateTime.MinValue;
                DateTime.TryParse( group.AttributeValues[StartDateAttribute.Key].Value, out startDateValue );

                var endDateValue = DateTime.MinValue;
                DateTime.TryParse( group.AttributeValues[EndDateAttribute.Key].Value, out endDateValue );

                if ( ( startDateValue <= JobStartDateTime && endDateValue >= JobStartDateTime ) || ( endDateValue == DateTime.MinValue && startDateValue != DateTime.MinValue && startDateValue <= JobStartDateTime ) )
                {
                    var _group = groupService.Get( group.Id );
                    _group.IsActive = true;
                    groupsActivated++;
                }
            }

            rockContext.SaveChanges();

            if ( groupsInactivated > 0 || groupsActivated > 0 )
            {
                Result = string.Format( "Inactivated {0} {1}. Activated {2} {3}.", groupsInactivated, "group".PluralizeIf( groupsInactivated == 0 || groupsInactivated > 1 ), groupsActivated, "group".PluralizeIf( groupsActivated == 0 || groupsActivated > 1 ) );
            }
            else
            {
                Result = "No groups changed.";
            }
        }
    }
}
