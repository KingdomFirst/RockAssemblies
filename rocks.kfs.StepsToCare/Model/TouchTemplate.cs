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
using System.Collections.Generic;
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
        public List<int> AssignToGroups { get; set; }
        public int Order { get; set; } = 0;

        public override string ToString()
        {
            return NoteTemplate.Note;
        }
    }
}