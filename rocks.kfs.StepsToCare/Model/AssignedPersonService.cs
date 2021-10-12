using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
