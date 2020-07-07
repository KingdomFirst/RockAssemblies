
namespace EventBriteDotNetFramework.Entities
{
    public class OrderCosts
    {
        public Cost Base_Price { get; set; }
        public Cost Eventbrite_Fee { get; set; }
        public Cost Gross { get; set; }
        public Cost Payment_Fee { get; set; }
        public Cost Tax { get; set; }
    }
}
