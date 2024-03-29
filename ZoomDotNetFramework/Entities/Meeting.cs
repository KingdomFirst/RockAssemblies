﻿// <copyright>
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
using System.Collections.Generic;
using ZoomDotNetFramework.Enums;

namespace ZoomDotNetFramework.Entities
{
    public class Meeting
    {
        public string Uuid { get; set; }
        public long Id { get; set; }
        public string Host_Id { get; set; }
        public string Assistant_Id { get; set; }
        public string Host_Email { get; set; }
        public string Topic { get; set; }
        public MeetingType Type { get; set; }
        public bool Pre_Schedule { get; set; }
        public string Status { get; set; }
        public DateTimeOffset Start_Time { get; set; }
        public int Duration { get; set; }
        public string Timezone { get; set; }
        public DateTime Created_At { get; set; }
        public string Agenda { get; set; }
        public string Start_Url { get; set; }
        public string Join_Url { get; set; }
        public string Password { get; set; }
        public string H323_Password { get; set; }
        public string Encrypted_Password { get; set; }
        public int Pmi { get; set; }
        public List<TrackingField> Tracking_Fields { get; set; }
        public List<Occurrence> Occurrences { get; set; }
        public MeetingSetting Settings { get; set; }
        public Recurrence Recurrence { get; set; }
        public string Template_Id { get; set; }
        public bool Join_Before_Host { get; set; }
        public int JBH_Time { get; set; }
    }
}
