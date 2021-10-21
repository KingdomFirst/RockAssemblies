using Rock.Data;

namespace rocks.kfs.Zoom.Model
{
    public class RoomOccurrenceService : Service<RoomOccurrence>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RoomOccurrenceService"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public RoomOccurrenceService( RockContext context ) : base( context ) { }
    }
}
