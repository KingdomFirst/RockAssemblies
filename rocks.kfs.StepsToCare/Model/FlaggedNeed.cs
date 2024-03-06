using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rock.Model;

namespace rocks.kfs.StepsToCare.Model
{
    public class FlaggedNeed
    {
        public CareNeed CareNeed { get; set; }
        public TouchTemplate TouchTemplate { get; set; }
        public Note Note { get; set; }
        public int NoteTouchCount { get; set; } = 0;
        public int TouchCount { get; set; } = 0;
        public bool HasNoteOlderThenHours { get; set; } = false;
        public bool HasFollowUpWorkerNote { get; set; } = false;

    }
}
