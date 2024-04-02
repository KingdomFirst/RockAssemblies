using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rock.Lava;

namespace rocks.kfs.StepsToCare.Model
{
    // used to map Attribute Matrix Values to object for easier use.
    public class TouchTemplate : LavaDataObject
    {
        public NoteTemplate NoteTemplate { get; set; }
        public int MinimumCareTouches { get; set; }
        public int MinimumCareTouchHours { get; set; }
        public bool NotifyAll { get; set; }
        public bool Recurring { get; set; }
    }
}
