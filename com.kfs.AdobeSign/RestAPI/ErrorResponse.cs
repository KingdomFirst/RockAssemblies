using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json;
namespace com.kfs.AdobeSign.RestAPI
{
    public class ErrorResponse
    {
        [JsonProperty( "code" )]
        public string ErrorCode { get; set; }
        [JsonProperty( "message" )]
        public string ErrorMessage { get; set; }
    }
}
