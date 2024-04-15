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
using System.Linq;
using Rock.Data;

namespace rocks.kfs.StepsToCare.Model
{
    public class AssignedPersonService : Service<AssignedPerson>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AssignedPersonService"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public AssignedPersonService( RockContext context ) : base( context ) { }

        public AssignedPerson GetByPersonAliasId( int? PersonAliasId )
        {
            return this.Queryable().Where( ap => ap.PersonAliasId == PersonAliasId ).FirstOrDefault();
        }

        public AssignedPerson GetByPersonAliasAndCareNeed( int? PersonAliasId, int? needId )
        {
            return this.Queryable().Where( ap => ap.PersonAliasId == PersonAliasId && ap.NeedId == needId ).FirstOrDefault();
        }
    }
}