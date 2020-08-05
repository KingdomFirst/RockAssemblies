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
using RestSharp.Deserializers;

namespace EventbriteDotNetFramework.Entities
{
    public class Venue
    {
        [DeserializeAs( Name = "id" )]
        public long Id { get; set; }

        [DeserializeAs( Name = "address" )]
        public Address Address { get; set; }

        [DeserializeAs( Name = "latitude" )]
        public string Latitude { get; set; }

        [DeserializeAs( Name = "longitude" )]
        public string Longitude { get; set; }

        [DeserializeAs( Name = "name" )]
        public string Name { get; set; }
    }
}
