using RestSharp.Deserializers;

namespace EventBriteDotNetFramework.Entities
{
    public class Venue
    {
        [DeserializeAs( Name = "id" )]
        public long Id { get; set; }

        [DeserializeAs( Name = "address" )]
        public Address Address { get; set; }

        [DeserializeAs( Name = "latitude" )]
        public string Latitude { get; set; }

        [DeserializeAs( Name = "longitude" )]
        public string Longitude { get; set; }

        [DeserializeAs( Name = "name" )]
        public string Name { get; set; }
    }
}
