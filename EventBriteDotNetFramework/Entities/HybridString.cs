using RestSharp.Deserializers;

namespace EventBriteDotNetFramework.Entities
{
    public class HybridString
    {
        [DeserializeAs( Name = "text" )]
        public string Text { get; set; }

        [DeserializeAs( Name = "html" )]
        public string Html { get; set; }
    }
}
