
namespace EventBriteDotNetFramework.Entities
{
    public class Address
    {
        public string City { get; set; }

        public string Country { get; set; }

        public string Region { get; set; }

        public string Address_1 { get; set; }

        public string Address_2 { get; set; }
        public string Postal_Code { get; set; }
    }
    public class AttendeeAddress
    {
        public Address Home { get; set; }
        public Address Ship { get; set; }
        public Address Work { get; set; }
        public Address Bill { get; set; }
    }
}