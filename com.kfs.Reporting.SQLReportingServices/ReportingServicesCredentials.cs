using System.Net;
using System.Security.Principal;
using Microsoft.Reporting.WebForms;

namespace com.kfs.Reporting.SQLReportingServices
{
    public class ReportingServicesCredentials : IReportServerCredentials
    {
        #region Fields
        private string mUserName;
        private string mPassword;
        #endregion

        #region Properties
        public WindowsIdentity ImpersonationUser
        {
            get
            {
                return null;
            }
        }

        public ICredentials NetworkCredentials
        {
            get
            {
                return new NetworkCredential( this.StrippedUsername, this.Password, this.UsernameDomain );
            }
        }
        public string Password
        {
            get
            {
                return mPassword;
            }
            set
            {
                mPassword = value;
            }
        }
        public string UserName
        {
            get
            {
                return mUserName;
            }
            set
            {
                mUserName = value;
            }

        }

        public string StrippedUsername
        {
            get
            {
                if ( this.mUserName.IndexOf( "\\" ) > 0 )
                {
                    string[] splitUserName = this.mUserName.Split( "\\".ToCharArray() );
                    if ( splitUserName.Length == 2 )
                    {
                        return splitUserName[1];
                    }
                }
                return this.mUserName;
            }
        }

        public string UsernameDomain
        {
            get
            {
                if ( this.mUserName.IndexOf( "\\" ) > 0 )
                {
                    string[] usernameValues = mUserName.Split( "\\".ToCharArray()  );
                    if ( usernameValues.Length == 2 )
                    {
                        return usernameValues[0];
                    }
                }
                return "";
            }
        }
        #endregion

        #region Constructors
        private ReportingServicesCredentials() { }

        public ReportingServicesCredentials( string userName, string password )
        {
            UserName = userName;
            Password = password;
        }
        #endregion

        #region Public Methods
        public bool GetFormsCredentials( out Cookie authCookie, out string userName, out string password, out string authority )
        {
            authCookie = null;
            object obj = null;
            string str = ( string )obj;
            authority = ( string )obj;
            string str1 = str;
            string str2 = str1;
            password = str1;
            userName = str2;
            return false;
        }
        #endregion
    }
}
