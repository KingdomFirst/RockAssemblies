using System.Web.Http;
using Rock.Rest.Filters;

namespace com.kfs.Reporting.SQLReportingServices.Rest.Controller
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
        [Route( "api/com.kfs/ReportingServices/GetFolderList" )]
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
        [Route( "api/com.kfs/ReportingServices/GetFolderList" )]
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
        [Route( "api/com.kfs/ReportingServices/GetReportTree" )]
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
        [Route( "api/com.kfs/ReportingServices/GetReportTree" )]
        public ReportingServiceItem GetReportTree( string rootPath, bool getChildren, bool includeHidden )
        {
            return ReportingServiceItem.GetReportTree( rootPath, getChildren, includeHidden );
        }
    }
}
