using System;
using System.Collections.Generic;
using System.Linq;
using Arena.Custom.KFS.Eventbrite.Entities;

namespace EventBriteDotNetFramework.Responses
{
  public class OwnedEventsResponse
  {
    public Pagination Pagination { get; set; }

    public List<Entities.Event> Events { get; set; }
  }
}
