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
using System;

namespace rocks.kfs.StepsToCare.Model
{
    public partial class WorkerResult
    {
        public int Count { get; set; }
        public DateTime? LastAssignmentDate { get; set; }
        public CareWorker Worker { get; set; }
        public bool HasCategory { get; set; } = false;
        public bool HasCampus { get; set; } = false;
        public bool HasAgeRange { get; set; } = false;
        public bool HasGender { get; set; } = false;
    }
}