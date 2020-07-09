// <copyright>
// Copyright 2020 by Kingdom First Solutions
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using EventbriteDotNetFramework.Entities;

namespace EventbriteDotNetFramework.Requests
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
