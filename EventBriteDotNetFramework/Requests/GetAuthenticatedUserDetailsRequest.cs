using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using EventBriteDotNetFramework.Entities;

namespace EventBriteDotNetFramework.Requests
{
  public class GetAuthenticatedUserDetailsRequest : IRequest<User>
  {
    public HttpMethod HttpMethod
    {
      get { return HttpMethod.Get; }
    }

    public string RequestUri
    {
      get
      {
        return "/v3/users/me/";
      }
    }
  }
}
