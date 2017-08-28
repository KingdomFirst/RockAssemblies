using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;

using Rock.Security;
using Rock.Attribute;
using Rock.Model;
using Rock.Web.Cache;

using Newtonsoft.Json.Linq;
using System.Web;
using Rock.Communication;
using Rock;
using Rock.Data;
using com.kfs.AdobeSign.RestAPI;

namespace com.kfs.AdobeSign
{
    /// <summary>
    /// Adobe Sign Digital Signature Provider
    /// </summary>
    [Description( "Adobe Sign Digital Signature Provider" )]
    [Export( typeof( DigitalSignatureComponent ) )]
    [ExportMetadata( "ComponentName", "Adobe Sign" )]

    [TextField( "Cookie Initialization Url", "The URL of the Adobe Sign page to use for setting an initial cookie.", true, "https://mw.signnow.com/setcookie", "Advanced", 0 )]
    
    public class AdobeSign : DigitalSignatureComponent
    {
        /// <summary>
        /// Gets the Cookie Initialization URL. If specified, Rock will first redirect
        /// user to this url in order to set a cookie prior to submitting any requests.
        /// </summary>
        /// <value>
        /// The set cookie URL.
        /// </value>
        public override string CookieInitializationUrl
        {
            get
            {
                return GetAttributeValue( "CookieInitializationUrl" );
            }
        }

        public override bool CancelDocument( SignatureDocument document, out List<string> errors )
        {
            throw new NotImplementedException();
        }

        public override string CreateDocument( SignatureDocumentTemplate documentType, Person appliesTo, Person assignedTo, string documentName, out List<string> errors, bool sendInvite )
        {
            throw new NotImplementedException();
        }

        public override string GetDocument( SignatureDocument document, string folderPath, out List<string> errors )
        {
            throw new NotImplementedException();
        }

        public override string GetInviteLink( string documentId, out List<string> errors )
        {
            throw new NotImplementedException();
        }

        public override string GetInviteLink( SignatureDocument document, Person recipient, out List<string> errors )
        {
            throw new NotImplementedException();
        }

        public override Dictionary<string, string> GetTemplates( out List<string> errors )
        {
            throw new NotImplementedException();
        }

        public override bool ResendDocument( SignatureDocument document, out List<string> errors )
        {
            throw new NotImplementedException();
        }

        public override bool UpdateDocumentStatus( SignatureDocument document, out List<string> errors )
        {
            throw new NotImplementedException();
        }
    }
}