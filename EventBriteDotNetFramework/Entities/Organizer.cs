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
namespace EventBriteDotNetFramework.Entities
{
  public class Organizer
  {
    /// <summary>
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    ///   The organizer’s name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    ///   The URL to the organizer’s page on Eventbrite.
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// </summary>
    public int NumberOfPastEvents { get; set; }

    /// <summary>
    /// </summary>
    public int NumberOfFutureEvents { get; set; }
  }
}