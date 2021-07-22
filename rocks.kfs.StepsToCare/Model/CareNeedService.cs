using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

    }
}
