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
namespace EventbriteDotNetFramework.Responses
{
    public class WebhookResponse
    {
        public string Api_Url { get; set; }
        public WebhookConfig Config { get; set; }
    }

    public class WebhookConfig
    {
        public long Webhook_Id { get; set; }
        public string Action { get; set; }
        public string Endpoint_Url { get; set; }
        public long User_Id { get; set; }

    }
}
