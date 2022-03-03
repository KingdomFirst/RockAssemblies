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
using System.Web.Http;
using Rock.Rest.Filters;

namespace rocks.kfs.Reporting.SQLReportingServices.Rest.Controller
{
    /// <summary>
    /// The API endpoints for the report treeview
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController" />
    public class ReportingServicesController : ApiController
    {
        public ReportingServicesController()
        {
        }

        /// <summary>
        /// Gets the folder tree.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authenticate, Secured]
        [Route( "api/rocks.kfs/ReportingServices/GetFolderList" )]
        public ReportingServiceItem GetFolderTree()
        {
            return ReportingServiceItem.GetFoldersTree( "", true, true );
        }

        /// <summary>
        /// Gets the folder tree.
        /// </summary>
        /// <param name="rootPath">The root path.</param>
        /// <param name="getChildren">if set to <c>true</c> [get children].</param>
        /// <param name="includeHidden">if set to <c>true</c> [include hidden].</param>
        /// <returns></returns>
        [HttpGet]
        [Authenticate, Secured]
        [Route( "api/rocks.kfs/ReportingServices/GetFolderList" )]
        public ReportingServiceItem GetFolderTree( string rootPath, bool getChildren, bool includeHidden )
        {
            return ReportingServiceItem.GetFoldersTree( rootPath, getChildren, includeHidden );
        }

        /// <summary>
        /// Gets the report tree.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authenticate, Secured]
        [Route( "api/rocks.kfs/ReportingServices/GetReportTree" )]
        public ReportingServiceItem GetReportTree()
        {
            return ReportingServiceItem.GetReportTree( "", true, true );
        }

        /// <summary>
        /// Gets the report tree.
        /// </summary>
        /// <param name="rootPath">The root path.</param>
        /// <param name="getChildren">if set to <c>true</c> [get children].</param>
        /// <param name="includeHidden">if set to <c>true</c> [include hidden].</param>
        /// <returns></returns>
        [HttpGet]
        [Authenticate, Secured]
        [Route( "api/rocks.kfs/ReportingServices/GetReportTree" )]
        public ReportingServiceItem GetReportTree( string rootPath, bool getChildren, bool includeHidden )
        {
            return ReportingServiceItem.GetReportTree( rootPath, getChildren, includeHidden );
        }
    }
}
