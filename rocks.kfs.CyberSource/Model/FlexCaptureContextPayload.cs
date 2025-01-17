using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rocks.kfs.CyberSource.Model
{
    internal class FlexCaptureContextPayload
    {
        public FlexPayloadFlx flx { get; set; }

        public List<FlexPayloadCtx> ctx { get; set; }

        public string iss { get; set; }

        public long exp { get; set; }

        public long iat { get; set; }

        public string jti { get; set; }

    }

    public class FlexPayloadFlx
    {
        public string path { get; set; }

        public string data { get; set; }

        public string origin { get; set; }

        public FlexPayloadJwk jwk { get; set; }

    }

    public class FlexPayloadJwk
    {
        public string kty { get; set; }

        public string e { get; set; }

        public string use { get; set; }

        public string n { get; set; }

        public string kid { get; set; }

    }
    public class FlexPayloadCtx
    {
        public FlexPayloadData data { get; set; }

        public string type { get; set; }
    }

    public class FlexPayloadData
    {
        public string clientLibrary { get; set; }

        public string mfOrigin { get; set; }

        public string[] allowedCardNetworks { get; set; }

        public string[] targetOrigins { get; set; }
    }
}
