using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using EventBriteDotNetFramework.Responses;

namespace EventBriteDotNetFramework.Requests
{
  public class GetUserOwnedEventsRequest : IRequest<OwnedEventsResponse>
  {
    public GetUserOwnedEventsRequest(long id)
    {
      Id = id;
    }

    public long Id { get; private set; }

    public HttpMethod HttpMethod
    {
      get
      {
        return HttpMethod.Get;
      }
    }

    public string RequestUri
    {
      get { return string.Format("/v3/users/{0}/owned_events/", Id); }
    }
  }
}
