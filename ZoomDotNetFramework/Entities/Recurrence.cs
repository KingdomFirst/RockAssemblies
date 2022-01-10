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
using System;
using ZoomDotNetFramework.Enums;

namespace ZoomDotNetFramework.Entities
{
    public class Recurrence
    {
        public RecurrenceType Type { get; set; }
        public int Repeat_Interval { get; set; }
        public string Weekly_Days { get; set; }
        public int Monthly_Day { get; set; }
        public int Monthly_Week { get; set; }
        public int Monthly_Week_Day { get; set; }
        public int End_Times { get; set; }
        public DateTime End_Date_Time { get; set; }  // Must be UTC
    }
}
