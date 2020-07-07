using System;

namespace EventBriteDotNetFramework.Entities
{
  public class TicketClass
  {
    public long Id { get; set; }

    public string Name { get; set; }

    public object Cost { get; set; }

    public object Fee { get; set; }

    public object Currency { get; set; }

    public bool Free { get; set; }

    public int MinimumQuantity { get; set; }

    public object MaximumQuantity { get; set; }

    public int QuantityTotal { get; set; }

    public int QuantitySold { get; set; }

    public object SalesStart { get; set; }

    public DateTime SalesEnd { get; set; }
  }
}