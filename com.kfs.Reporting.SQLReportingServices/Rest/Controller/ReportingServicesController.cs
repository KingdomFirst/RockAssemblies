using Rock;
using Rock.Security;
using Rock.Web.Cache;
using Rock.Web.UI.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;


namespace com.kfs.Reporting.SQLReportingServices.Rest.Controller
{
    public class ReportingServicesController : ApiController
    {
        public ReportingServicesController() { }

        [HttpGet]
        [Route( "api/com.kfs/ReportingServices/GetFolderList" )]
        public ReportingServiceItem GetFolderTree()
        {
            return ReportingServiceItem.GetFoldersTree( "", true, true );
        }

        [HttpGet]
        [Route( "api/com.kfs/ReportingServices/GetFolderList" )]
        public ReportingServiceItem GetFolderTree( string rootPath, bool getChildren, bool includeHidden )
        {
            return ReportingServiceItem.GetFoldersTree( rootPath, getChildren, includeHidden );
        }

        [HttpGet]
        [Route( "api/com.kfs/ReportingServices/GetReportTree" )]
        public ReportingServiceItem GetReportTree()
        {
            return ReportingServiceItem.GetReportTree( "", true, true );
        }

        [HttpGet]
        [Route( "api/com.kfs/ReportingServices/GetReportTree" )]
        public ReportingServiceItem GetReportTree( string rootPath, bool getChildren, bool includeHidden )
        {
            return ReportingServiceItem.GetReportTree( rootPath, getChildren, includeHidden );

        }



    }
}
