using System.Net;
using System.Security.Principal;
using Microsoft.Reporting.WebForms;

namespace com.kfs.Reporting.SQLReportingServices
{
    /// <summary>
    /// The Report Services Credentials
    /// </summary>
    /// <seealso cref="Microsoft.Reporting.WebForms.IReportServerCredentials" />
    public class ReportingServicesCredentials : IReportServerCredentials
    {
        #region Fields

        private string mUserName;
        private string mPassword;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the impersonation user.
        /// </summary>
        /// <value>
        /// The impersonation user.
        /// </value>
        public WindowsIdentity ImpersonationUser
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the network credentials.
        /// </summary>
        /// <value>
        /// The network credentials.
        /// </value>
        public ICredentials NetworkCredentials
        {
            get
            {
                return new NetworkCredential( this.StrippedUsername, this.Password, this.UsernameDomain );
            }
        }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
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

        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>
        /// The name of the user.
        /// </value>
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

        /// <summary>
        /// Gets the stripped username.
        /// </summary>
        /// <value>
        /// The stripped username.
        /// </value>
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

        /// <summary>
        /// Gets the username domain.
        /// </summary>
        /// <value>
        /// The username domain.
        /// </value>
        public string UsernameDomain
        {
            get
            {
                if ( this.mUserName.IndexOf( "\\" ) > 0 )
                {
                    string[] usernameValues = mUserName.Split( "\\".ToCharArray() );
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

        /// <summary>
        /// Prevents a default instance of the <see cref="ReportingServicesCredentials"/> class from being created.
        /// </summary>
        private ReportingServicesCredentials() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportingServicesCredentials"/> class.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        public ReportingServicesCredentials( string userName, string password )
        {
            UserName = userName;
            Password = password;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the forms credentials.
        /// </summary>
        /// <param name="authCookie">The authentication cookie.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <param name="authority">The authority.</param>
        /// <returns></returns>
        public bool GetFormsCredentials( out Cookie authCookie, out string userName, out string password, out string authority )
        {
            authCookie = null;
            object obj = null;
            string str = (string)obj;
            authority = (string)obj;
            string str1 = str;
            string str2 = str1;
            password = str1;
            userName = str2;
            return false;
        }

        #endregion
    }
}
