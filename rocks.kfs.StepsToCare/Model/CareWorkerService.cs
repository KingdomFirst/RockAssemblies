using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rock.Data;

namespace rocks.kfs.StepsToCare.Model
{
    public class CareWorkerService : Service<CareWorker>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CareWorkerService"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public CareWorkerService( RockContext context ) : base( context ) { }

    }
}
