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

using System.ComponentModel;

namespace rocks.kfs.ZoomRoom.Enums
{
    public enum UserType
    {
        [Description( "Basic" )]
        Basic = 1,
        [Description( "Licensed" )]
        Licensed = 2,
        [Description( "OnPrem" )]
        OnPrem = 3,
        [Description( "None (this can only be set with ssoCreate)." )]
        None = 99
    }
}
