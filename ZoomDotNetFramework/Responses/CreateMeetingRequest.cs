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
using rocks.kfs.ZoomRoom.Enums;
using System;
using System.Collections.Generic;
using ZoomDotNetFramework.Entities;

namespace ZoomDotNetFramework.Responses
{
    public class CreateMeetingRequest
    {
        public CreateMeetingRequestBody Request { get; set; }
    }

    public class CreateMeetingRequestBody
    {
        public string Topic { get; set; }
        public MeetingType Type { get; set; }
        public bool Pre_Schedule { get; set; }
        public DateTimeOffset Start_Time { get; set; }
        public int Duration { get; set; }
        public string Schedule_For { get; set; }
        public string Timezone { get; set; }
        public string Password { get; set; }
        public bool Default_Password { get; set; }
        public string Agenda { get; set; }
        public List<TrackingField> Tracking_Fields { get; set; }
        public Recurrence Recurrence { get; set; }
        public MeetingSetting Settings { get; set; }
        public string Template_Id { get; set; }
    }
}
