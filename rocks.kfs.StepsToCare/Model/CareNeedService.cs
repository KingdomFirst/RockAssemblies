// <copyright>
// Copyright 2024 by Kingdom First Solutions
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
using Rock.Data;

namespace rocks.kfs.StepsToCare.Model
{
    public class CareNeedService : Service<CareNeed>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CareNeedService"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public CareNeedService( RockContext context ) : base( context ) { }

        /// <summary>
        /// Determines whether this instance can delete the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <returns>
        ///   <c>true</c> if this instance can delete the specified item; otherwise, <c>false</c>.
        /// </returns>
        public bool CanDelete( CareNeed item, out string errorMessage )
        {
            errorMessage = string.Empty;
            return true;
        }
    }
}