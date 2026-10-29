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
// <notice>
// This file contains modifications by Kingdom First Solutions
// and is a derivative work.
//
// Modification (including but not limited to):
// * Adds setting to select a group detail page.
// * Adds any Leader Names as tool tip to the group name.
// </notice>
//
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;

using Rock;
using Rock.Attribute;
using Rock.Data;
using Rock.Model;
using Rock.Badge;
using Rock.Web;
using Rock.Web.Cache;

namespace rocks.kfs.PersonProfile.Badge
{
    /// <summary>
    /// Area Badge
    /// </summary>
    [Description( "Displays the group(s) of a particular type that have a geo-fence location around one or more of the the person's map locations." )]
    [Export( typeof( BadgeComponent ) )]
    [ExportMetadata( "ComponentName", "Nested Geofenced By Group" )]
    [GroupTypeField( "Group Type", "The type of group to use.", true )]
    [TextField( "Badge Color", "The color of the badge (#ffffff).", true, "#0ab4dd" )]
    [LinkedPage( "Group Detail Page", "The page that should be loaded when group name is clicked.", false )]
    public class NestedGeofencedByGroup : BadgeComponent
    {
        /// <summary>
        /// Renders the specified writer.
        /// </summary>
        /// <param name="badge">The badge.</param>
        /// <param name="writer">The writer.</param>
        /// <summary>
        /// Determines if this badge component applies to the given type.
        /// </summary>
        /// <param name="type">The entity type name.</param>
        /// <returns><c>true</c> if the badge applies to the type.</returns>
        public override bool DoesApplyToEntityType( string type )
        {
            return type.IsNullOrWhiteSpace() || typeof( Person ).FullName == type;
        }

        /*
            9/30/2026 - GEM

            Moved from the Render( BadgeCache, HtmlTextWriter ) override, which Rock
            obsoleted in 1.14 and removed in v20, to the Render( BadgeCache, IEntity,
            TextWriter ) + GetJavaScript pattern used in current Rock versions.
            Both exist since Rock 1.14, so this compiles on every version we ship.
            Rock wraps the returned JavaScript itself, so the <script> tag and
            Sys.Application.add_load (WebForms-only) wrapper are gone.

        */

        /// <inheritdoc/>
        public override void Render( BadgeCache badge, IEntity entity, TextWriter writer )
        {
            if ( !( entity is Person ) )
            {
                return;
            }

            var groupTypeGuid = GetAttributeValue( badge, "GroupType" ).AsGuidOrNull();
            var badgeColor = GetAttributeValue( badge, "BadgeColor" );

            if ( groupTypeGuid.HasValue && !string.IsNullOrWhiteSpace( badgeColor ) )
            {
                writer.Write( string.Format(
                    "<span class='label badge-geofencing-group badge-id-{0}' style='background-color:{1};display:none' ></span>",
                    badge.Id, badgeColor.EscapeQuotes() ) );
            }
        }

        /// <inheritdoc/>
        protected override string GetJavaScript( BadgeCache badge, IEntity entity )
        {
            var groupTypeGuid = GetAttributeValue( badge, "GroupType" ).AsGuidOrNull();
            var badgeColor = GetAttributeValue( badge, "BadgeColor" );

            if ( !( entity is Person person ) || !groupTypeGuid.HasValue || string.IsNullOrWhiteSpace( badgeColor ) )
            {
                return null;
            }

            var groupNameOrLink = LinkedPageUrl( badge, "GroupDetailPage" );
            if ( !string.IsNullOrWhiteSpace( groupNameOrLink ) )
            {
                groupNameOrLink = @"<a href=""" + groupNameOrLink + @"?GroupId=' + this.GroupId + '"">' + this.GroupName + '</a>";
            }
            else
            {
                groupNameOrLink = @"' + this.GroupName + '";
            }

            return string.Format( @"
$.ajax({{
    type: 'GET',
    url: Rock.settings.get('baseUrl') + 'api/rocks.kfs/PersonBadges/NestedGeofencingGroups/{0}/{1}' ,
    statusCode: {{
        200: function (data, status, xhr) {{
            var $badge = $('.badge-geofencing-group.badge-id-{2}');
            var badgeHtml = '';

            $.each(data, function() {{
                if ( badgeHtml != '' ) {{
                    badgeHtml += ' | ';
                }}
                badgeHtml += '<span title=""' + this.LeaderNames + '"" data-toggle=""tooltip"">{3}</span>';
            }});

            if (badgeHtml != '') {{
                $badge.show('fast');
            }} else {{
                $badge.hide();
            }}
            $badge.html(badgeHtml);
            $badge.find('span').tooltip();
        }}
    }},
}});", person.Id.ToString(), groupTypeGuid.ToString(), badge.Id, groupNameOrLink );
        }

        /// <summary>
        /// Builds and returns the URL for a linked <see cref="Rock.Model.Page"/> from a "linked page attribute" and any necessary query parameters.
        /// </summary>
        /// <param name="attributeKey">A <see cref="System.String"/> representing the name of the linked <see cref="Rock.Model.Page"/> attribute key.</param>
        /// <param name="queryParams">A <see cref="System.Collections.Generic.Dictionary{String,String}" /> containing the query string parameters to be added to the URL.
        /// In each <see cref="System.Collections.Generic.KeyValuePair{String,String}"/> the key value is a <see cref="System.String"/> that represents the name of the query string
        /// parameter, and the value is a <see cref="System.String"/> that represents the query string value..</param>
        /// <returns>A <see cref="System.String"/> representing the URL to the linked <see cref="Rock.Model.Page"/>. </returns>
        public virtual string LinkedPageUrl( BadgeCache badge, string attributeKey, Dictionary<string, string> queryParams = null )
        {
            var pageReference = new PageReference( GetAttributeValue( badge, attributeKey ), queryParams );
            if ( pageReference.PageId > 0 )
            {
                return pageReference.BuildUrl();
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
