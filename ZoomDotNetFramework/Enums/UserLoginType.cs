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

using System.ComponentModel;

namespace rocks.kfs.ZoomRoom.Enums
{
    public enum UserLoginType
    {
        [Description( "Facebook OAuth" )]
        FacebookOAuth = 0,
        [Description( "Google OAuth" )]
        GoogleOAuth = 1,
        [Description( "Apple OAuth" )]
        AppleOAuth = 24,
        [Description( "Microsoft OAuth" )]
        MicrosoftOAuth = 27,
        [Description( "Mobile Device" )]
        MobileDevice = 97,
        [Description( "RingCentral OAuth" )]
        RingCentralOAuth = 98,
        [Description( "API User" )]
        APIUser = 99,
        [Description( "Zoom Work Email" )]
        ZoomWorkEmail = 100,
        [Description( "Single Sign-On (SSO)" )]
        SingleSignOn = 101,
        [Description( "Phone Number - China Only" )]
        ChinaPhoneNumber = 11,
        [Description( "WeChat - China Only" )]
        ChinaWeChat = 21,
        [Description( "Alipay - China Only" )]
        ChinaAlipay = 23
    }
}
