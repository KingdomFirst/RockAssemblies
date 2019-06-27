using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;
namespace com.kfs.AdobeSign.RestAPI
{
    public class RawResponse
    {
        public RawResponse( HttpStatusCode statusCode, string responseString )
        {
            StatusCode = statusCode;
            jsonItem = responseString;
        }

        public HttpStatusCode StatusCode { get; set; }
        public string jsonItem { get; set; }
    }
}
