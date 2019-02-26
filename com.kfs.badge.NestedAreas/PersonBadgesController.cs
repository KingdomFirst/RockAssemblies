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