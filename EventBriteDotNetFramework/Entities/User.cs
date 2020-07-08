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
using System.Collections.Generic;

namespace EventBriteDotNetFramework.Entities
{
  public class User
  {
    /// <summary>
    ///   The user's id
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    ///   The user’s name. Use this in preference to first_name/last_name if possible for forward compatibility with
    ///   non-Western names.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    ///   The user’s first name.
    /// </summary>
    public string FirstName { get; set; }

    /// <summary>
    ///   The user’s last name.
    /// </summary>
    public string LastName { get; set; }

    /// <summary>
    ///   A list of user emails.
    /// </summary>
    public List<UserEmail> Emails { get; set; }
  }
}