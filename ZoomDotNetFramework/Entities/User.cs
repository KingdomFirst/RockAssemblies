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
    public class User
    {
        public int Id { get; set; }
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public string Email { get; set; }
        public int Type { get; set; }
        public string Role_Name { get; set; }
        public int Pmi { get; set; }
        public bool Use_Pmi { get; set; }
        public string Timezone { get; set; }
        public string Dept { get; set; }
        public DateTime Created_At { get; set; }
        public DateTime Last_Login_Time { get; set; }
        public string Last_Client_Version { get; set; }
        public string Language { get; set; }
        public List<UserPhoneNumber> Phone_Numbers { get; set; }
        public string Vanity_Url { get; set; }
        public string Personal_Meeting_Url { get; set; }
        public int Verified { get; set; }
        public string Pic_Url { get; set; }
        public string Cms_User_Id { get; set; }
        public string Account_id { get; set; }
        public string Host_Key { get; set; }
        public string Status { get; set; }
        public List<string> Group_ids { get; set; }
        public List<string> Im_Group_Ids { get; set; }
        public string Jid { get; set; }
        public string Job_Title { get; set; }
        public string Company { get; set; }
        public string Location { get; set; }
        public UserCustomAttribute Custom_Attributes { get; set; }
        public int Login_Type { get; set; }
        public string Role_Id { get; set; }
        public string Plan_United_Type { get; set; }
        public string Employee_Unique_Id { get; set; }
        public int Account_Number { get; set; }
        public string Manager { get; set; }
        public string Pronouns { get; set; }
        public int Pronouns_Option { get; set; }
    }

    public class UserPhoneNumber
    {
        public string Country { get; set; }
        public string Code { get; set; }
        public string Number { get; set; }
        public bool Verified { get; set; }
        public string Label { get; set; }
    }

    public class UserCustomAttribute
    {
        public string Key { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
