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

    }
}
