// <copyright>
// Copyright by the Spark Development Network
//
// Licensed under the Rock Community License (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.rockrms.com/license
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
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;
using System.Web.Http.WebHost;

using Rock;
using Rock.Data;
using Rock.Model;
using Rock.Rest;
using Rock.Rest.Filters;
using Rock.Security;
using Rock.Web.Cache;

namespace com.kfs.PersonBadges.Rest
{
    /// <summary>
    /// 
    /// </summary>
    public class com_kfs_PersonBadges_Rest_PersonBadgesController : ApiControllerBase, IHasCustomRoutes
    {
        /// <summary>
        /// Add in the routes that are supported by this badge
        /// </summary>
        /// <param name="routes">The RouteCollection that we shyould add any additional routes to.</param>
        public void AddRoutes( System.Web.Routing.RouteCollection routes )
        {
            routes.MapHttpRoute(
                name: "com.kfs.PersonBadges.Rest.PersonBadgesController",
                routeTemplate: "api/com.kfs/PersonBadges/NestedGeofencingGroups/{personId}/{groupTypeGuid}",
                defaults: new
                {
                    controller = "com_kfs_PersonBadges_Rest_PersonBadges"
                } );
        }

        /// <summary>
        /// Returns groups that are a specified type and geofence a given person
        /// </summary>
        /// <param name="personId">The person id.</param>
        /// <param name="groupTypeGuid">The group type guid with geofences.</param>
        /// <returns></returns>
        [Authenticate, Secured]
        [HttpGet]
        [System.Web.Http.Route( "api/com.kfs/PersonBadges/NestedGeofencingGroups/{personId}/{groupTypeGuid}" )]
        public List<GroupAndLeaderInfo> GetGeofencingGroups( int personId, Guid groupTypeGuid )
        {
            var rockContext = new RockContext();
            var groupMemberService = new GroupMemberService( rockContext );

            var groups = new GroupService( rockContext ).GetGeofencingGroups( personId, groupTypeGuid ).AsNoTracking();

            var result = new List<GroupAndLeaderInfo>();
            foreach ( var group in groups.OrderBy( g => g.Order ) )
            {
                var info = new GroupAndLeaderInfo();
                info.GroupName = group.Name.Trim();
                info.GroupId = group.Id;
                info.LeaderNames = groupMemberService
                    .Queryable().AsNoTracking()
                    .Where( m =>
                        m.GroupId == group.Id &&
                        m.GroupRole.IsLeader )
                    .Select( m => m.Person.NickName + " " + m.Person.LastName + " (" + m.GroupRole.Name + ")" )
                    .ToList()
                    .AsDelimited( ", " );
                result.Add( info );
            }

            return result;
        }

        /// <summary>
        /// Group and Leader name info
        /// </summary>
        public class GroupAndLeaderInfo
        {
            /// <summary>
            /// Gets or sets the name of the group.
            /// </summary>
            /// <value>
            /// The name of the group.
            /// </value>
            public string GroupName { get; set; }

            /// <summary>
            /// Gets or sets the id of the group.
            /// </summary>
            /// <value>
            /// The id of the group.
            /// </value>
            public int GroupId { get; set; }

            /// <summary>
            /// Gets or sets the leader names.
            /// </summary>
            /// <value>
            /// The leader names.
            /// </value>
            public string LeaderNames { get; set; }
        }
    }
}