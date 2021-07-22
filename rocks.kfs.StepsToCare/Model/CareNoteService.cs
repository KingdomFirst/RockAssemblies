using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rock.Data;

namespace rocks.kfs.StepsToCare.Model
{
    public class CareNoteService : Service<CareNeed>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CareNoteService"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public CareNoteService( RockContext context ) : base( context ) { }

    }
}
