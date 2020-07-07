using System;

namespace EventBriteDotNetFramework.Entities
{
  public class HybridDate
  {
    public string Timezone { get; set; }

    public DateTime Local { get; set; }

    public DateTime Utc { get; set; }
  }
}