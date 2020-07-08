﻿// <copyright>
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
using System.Text;
using System.Threading.Tasks;

namespace EventBriteDotNetFramework.Entities
{
    public class Profile
    {
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public AttendeeAddress Addresses { get; set; }
        public string Gender { get; set; }
        public string Prefix { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Home_Phone { get; set; }
        public string Cell_Phone { get; set; }
        public string Work_Phone { get; set; }
        public DateTime Birth_Date { get; set; }
        public int Age { get; set; }
    }
}
