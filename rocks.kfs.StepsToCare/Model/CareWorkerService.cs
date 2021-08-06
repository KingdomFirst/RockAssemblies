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

        /// <summary>
        /// Returns a <see cref="rocks.kfs.StepsToCare.Model.CareWorker"/> by Id and PersonId
        /// </summary>
        /// <param name="categoryId">A <see cref="System.Int32"/> representing the Category Id of the <see cref="rocks.kfs.StepsToCare.Model.CareWorker" />.</param>
        /// <param name="personId">A <see cref="System.Guid"/> representing the person identifier of an <see cref="rocks.kfs.StepsToCare.Model.CareWorker">CareWorker's</see> PersonAlias.</param>
        /// <returns>The <see cref="rocks.kfs.StepsToCare.Model.CareWorker"/> that matches the provided criteria. If a match is not found, null will be returned.</returns>
        public CareWorker Get( int? categoryId, int personId )
        {
            return Queryable()
               .Where( cw => ( categoryId == null || cw.CategoryValueId == categoryId ) && cw.PersonAlias.Person.Id == personId )
               .FirstOrDefault();
        }
    }
}
