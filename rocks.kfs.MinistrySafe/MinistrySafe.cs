// <copyright>
// Copyright 2019 by Kingdom First Solutions
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

namespace rocks.kfs.MinistrySafe
{
    public class MinistrySafe
    {
        /// <summary>
        /// The base URL.
        /// </summary>
        /// <param name="StagingMode">if set to <c>true</c> [staging mode].</param>
        /// <returns></returns>
        public static Uri BaseUrl( bool StagingMode = false )
        {
            if ( StagingMode )
            {
                return new Uri( "https://staging.ministrysafe.com/api/" );
            }
            else
            {
                return new Uri( "https://safetysystem.ministrysafe.com/api/" );
            }
        }
    }
}
