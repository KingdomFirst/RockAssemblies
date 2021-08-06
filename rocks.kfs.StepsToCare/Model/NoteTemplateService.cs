using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rock.Data;

namespace rocks.kfs.StepsToCare.Model
{
    public class NoteTemplateService : Service<NoteTemplate>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NoteTemplateService"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public NoteTemplateService( RockContext context ) : base( context ) { }

    }
}
