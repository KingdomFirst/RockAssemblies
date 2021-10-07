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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoomDotNetFramework.Entities
{
    public class MeetingSetting
    {
        public bool Host_Video { get; set; }
        public bool Participant_Video { get; set; }
        public bool Cn_Meeting { get; set; }
        public bool In_Meeting { get; set; }
        public bool Join_Before_Host { get; set; }
        public int Jbh_Time { get; set; }
        public bool Mute_Upon_Entry { get; set; }
        public bool Watermark { get; set; }
        public bool Use_Pmi { get; set; }
        public int Approval_Type { get; set; }
        public int Registration_Type { get; set; }
        public string Audio { get; set; }
        public string Auto_Recording { get; set; }
        public string Alternative_Hosts { get; set; }
        public bool Close_Registration { get; set; }
        public bool Waiting_Room { get; set; }
        public List<string> Global_Dial_In_Countries { get; set; }
        public List<GlobalDialInNumber> Global_Dial_In_Numbers { get; set; }
        public string Contact_Name { get; set; }
        public string Contact_Email { get; set; }
        public bool Registrants_Email_Notification { get; set; }
        public bool Registrants_Confirmation_Email { get; set; }
        public bool Meeting_Authentication { get; set; }
        public string Authentication_Option { get; set; }
        public string Authentication_Domains { get; set; }
        public string Authentication_Name { get; set; }
        public bool Show_Share_Button { get; set; }
        public bool Allow_Multiple_Devices { get; set; }
        public string Encryption_Type { get; set; }
        public ApprovedOrDeniedCountriesRegions Approved_Or_Denied_Countries_Or_Regions { get; set; }
        public List<AuthenticationException> Authentication_Exception { get; set; }
        public BreakoutRoomSetting Breakout_Room { get; set; }
        public LanguageInterpretationSetting Language_Interpretation { get; set; }
        public List<CustomKey> Custom_Keys { get; set; }
        public bool Alternative_Hosts_Email_Notification { get; set; }
    }

    public class GlobalDialInNumber
    {
        public string Country { get; set; }
        public string Country_Name { get; set; }
        public string City { get; set; }
        public string Number { get; set; }
        public string Type { get; set; }
    }

    public class ApprovedOrDeniedCountriesRegions
    {
        public bool Enable { get; set; }
        public string Method { get; set; }
        public List<string> Approved_List { get; set; }
        public List<string> Denied_List { get; set; }
    }

    public class AuthenticationException
    {
        public string Name { get; set; }
        public string Email { get; set; }
    }

    public class BreakoutRoomSetting
    {
        public bool Enable { get; set; }
        public List<BreakoutRoom> Rooms { get; set; }
    }

    public class BreakoutRoom
    {
        public string Name { get; set; }
        public List<string> Participants { get; set; }
    }

    public class LanguageInterpretationSetting
    {
        public bool Enable { get; set; }
        public List<Interpreter> Interpreters { get; set; }
    }

    public class Interpreter
    {
        public string Email { get; set; }
        public string Languages { get; set; }
    }

    public class CustomKey
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
