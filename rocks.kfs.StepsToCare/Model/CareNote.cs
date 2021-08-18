// <copyright>
// Copyright 2021 by Kingdom First Solutions
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
namespace rocks.kfs.StepsToCare.Model
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.ModelConfiguration;
    using System.Runtime.Serialization;
    using Rock.Data;

    [Table( "_rocks_kfs_StepsToCare_CareNote" )]
    [DataContract]
    public class CareNote : Rock.Data.Model<CareNote>, Rock.Data.IRockEntity
    {
        #region Entity Properties

        [Required]
        [DataMember]
        public int? NeedId { get; set; }

        [DataMember]
        public string Note { get; set; }

        [DataMember]
        public bool IsPrivateNote { get; set; }

        #endregion Entity Properties

        #region Virtual Properties

        [LavaInclude]
        public virtual CareNeed CareNeed { get; set; }

        #endregion Virtual Properties
    }
    #region Entity Configuration

    public partial class CareNoteConfiguration : EntityTypeConfiguration<CareNote>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CareNoteConfiguration"/> class.
        /// </summary> 
        public CareNoteConfiguration()
        {
            this.HasRequired( cn => cn.CareNeed ).WithMany( cn => cn.CareNotes ).HasForeignKey( cn => cn.NeedId ).WillCascadeOnDelete( true );

            // IMPORTANT!!
            this.HasEntitySetName( "CareNote" );
        }
    }

    #endregion
}