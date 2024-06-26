﻿// <copyright>
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
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using Rock.Data;

namespace rocks.kfs.StepsToCare.Model
{
    [Table( "_rocks_kfs_StepsToCare_NoteTemplate" )]
    [DataContract]
    public partial class NoteTemplate : Model<NoteTemplate>, IRockEntity, IOrdered
    {
        #region Entity Properties

        [DataMember]
        [StringLength( 250 )]
        public string Icon { get; set; }

        [DataMember]
        public string Note { get; set; }

        [Required]
        [DataMember( IsRequired = true )]
        public int Order { get; set; }

        [DataMember]
        public bool IsActive { get; set; }

        #endregion Entity Properties
    }
}