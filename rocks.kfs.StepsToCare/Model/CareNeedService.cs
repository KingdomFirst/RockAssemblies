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
