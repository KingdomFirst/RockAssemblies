using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Arena.Custom.KFS.Eventbrite.Entities;

namespace EventBriteDotNetFramework.Requests
{
  public class GetOrganizerByIdRequest : IRequest<Organizer>
  {
    public GetOrganizerByIdRequest(long organizerId)
    {
      OrganizerId = organizerId;
    }

    public long OrganizerId { get; private set; }
    public HttpMethod HttpMethod { get { return HttpMethod.Get; } }
    public string RequestUri { get { return string.Format("/v3/organizers/{0}", OrganizerId); } }
  }
}
