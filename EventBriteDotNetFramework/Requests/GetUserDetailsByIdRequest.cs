using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Arena.Custom.KFS.Eventbrite.Entities;

namespace EventBriteDotNetFramework.Requests
{
  public class GetUserDetailsByIdRequest : IRequest<User>
  {
    public GetUserDetailsByIdRequest(long userId)
    {
      UserId = userId;
    }

    public long UserId { get; private set; }

    public HttpMethod HttpMethod { get { return HttpMethod.Get; }}

    public string RequestUri
    {
      get { return string.Format("/v3/users/{0}/", UserId); }
    }
  }
}
