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

namespace EventbriteDotNetFramework.Entities
{
    public class Organization
    {
        /// <summary>
        ///   The Organization's id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        ///   Organization Name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///   (Optional) ID of the image for an Organization.
        /// </summary>
        public string Image_Id { get; set; }

        /// <summary>
        ///   Type of business vertical within which this Organization operates. Currently, the only values are default and music. If not specified, the value is default.
        /// </summary>
        public string Vertical { get; set; }
    }
}