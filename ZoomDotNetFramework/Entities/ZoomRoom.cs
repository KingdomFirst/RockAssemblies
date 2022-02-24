// <copyright>
// Copyright 2021 by Kingdom First Solutions
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
using ZoomDotNetFramework.Enums;

namespace ZoomDotNetFramework.Entities
{
    public class ZoomRoom
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public RoomType Type { get; set; }
        public string Activation_Code { get; set; }
        public string Status { get; set; }
        public string Room_Id { get; set; }
        public string Location_Id { get; set; }
    }

    public class ZRRoom
    {
        public string Zr_Name { get; set; }
        public string Zr_Id { get; set; }
    }
}
