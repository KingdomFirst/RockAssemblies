using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json;
namespace com.kfs.AdobeSign.RestAPI
{
    public class LibraryDocument
    {
        AdobeSignClient mClient = null;

        private LibraryDocument() { }

        public LibraryDocument( AdobeSignClient client )
        {
            mClient = client;
        }

        public List<DocumentLibraryItem> GetUserDocumentLibaryItemsByEmail( string email )
        {
            return GetUserDocumentLibaryItems( string.Format( "email:{0}", email ) );
        }
        public List<DocumentLibraryItem> GetUserDocumentLibraryItemsByUserId( string userId )
        {
            return GetUserDocumentLibaryItems( string.Format( "userid:{0}", userId ) );
        }

        private List<DocumentLibraryItem> GetUserDocumentLibaryItems( string apiUserValue )
        {
            string path = "libraryDocuments";

            Dictionary<string, string> headerItems = new Dictionary<string, string>();
            if ( !string.IsNullOrWhiteSpace( apiUserValue ) )
            {

                headerItems.Add( "x-api-user", apiUserValue );
            }

            RawResponse resp = mClient.SendRequest( path, "Get", null, headerItems, null );

            if ( resp.StatusCode != System.Net.HttpStatusCode.OK )
            {
                ErrorResponse eResp = JsonConvert.DeserializeObject<ErrorResponse>( resp.jsonItem );
                throw new Exception( string.Format( "An error has occurred while retrieving Library Items. Code: {0}. Message:{1}", eResp.ErrorCode, eResp.ErrorMessage ) );
            }

            return JsonConvert.DeserializeObject<DocumentLibaryListResponse>( resp.jsonItem ).DocumentLibraryList;
        }


    }

    public class DocumentLibaryListResponse
    {
        [JsonProperty( "libraryDocumentList" )]
        public List<DocumentLibraryItem> DocumentLibraryList { get; set; }
    }
    public class DocumentLibraryItem
    {
        public string libraryDocumentId { get; set; }
        public List<string> libraryTemplateTypes { get; set; }
        public string modifiedDate { get; set; }
        public string name { get; set; }
        public string scope { get; set; }
    }
}
