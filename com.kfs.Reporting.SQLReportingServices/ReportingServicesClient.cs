using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.kfs.Reporting.SQLReportingServices
{
    public class ReportingServicesClient
    {
        #region Properties
        public string ServerUrl  { get; set; }
        public string ReportPath { get; set; }
        public string ContentManagerUser { get; set; }
        public string ContentManagerPassword { get; set; }
        public string BrowserUser { get; set; }
        public string BrowserPassword { get; set; }
        public bool Configured { get; set; }

        private Guid ReportingServicesCategoryGuid = new Guid( "BE54A3EB-98F9-4BBE-86FD-A3F503CDADF6" );
        #endregion

        #region Constructors
        public ReportingServicesClient()
        {
            LoadCredentials();
        }
        #endregion

        #region Public Methods
        public bool SaveCredentials(out string message)
        {
            throw new NotImplementedException();
        }

        public bool TestConnection(out string message)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Private Methods

        private void LoadCredentials()
        {
            throw new NotImplementedException();
        }

        private bool ReportingServicesCategoryExists()
        {
            throw new NotImplementedException();
        }

        #endregion


    }
}
