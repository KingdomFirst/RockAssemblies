using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
namespace EventBriteDotNetFramework.Requests
{
  public interface IRequest<TResponse>
  {
    HttpMethod HttpMethod { get; }
    string RequestUri { get; }
  }
}
