// <copyright>
// Copyright 2024 by Kingdom First Solutions
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//
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
        public bool HasNoteOlderThanHours { get; set; } = false;
        public bool HasFollowUpWorkerNote { get; set; } = false;
    }
}