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
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Runtime.Serialization;
    using System.Data.Entity.ModelConfiguration;
    using Rock.Data;
    using Rock.Model;

    /// <summary>
    /// A Care Need
    /// </summary>
    [Table( "_rocks_kfs_StepsToCare_CareNeed" )]
    [DataContract]
    public class CareNeed : Rock.Data.Model<CareNeed>, Rock.Data.IRockEntity
    {
        #region Entity Properties

        [Required]
        [DataMember]
        public int? PersonAliasId { get; set; }

        [Required]
        [DataMember]
        public string Details { get; set; }

        [Required]
        [DataMember]
        public int? SubmitterAliasId { get; set; }

        [DataMember]
        [DefinedValue( SystemGuid.DefinedType.CARE_NEED_CATEGORY )]
        public int? CategoryValueId { get; set; }

        [DataMember]
        public DateTime? DateEntered { get; set; }

        [DataMember]
        public DateTime? FollowUpDate { get; set; }

        [DataMember]
        [DefinedValue( SystemGuid.DefinedType.CARE_NEED_STATUS )]
        public int? StatusValueId { get; set; }

        /// <summary>
        /// Gets or sets the campus identifier.
        /// </summary>
        /// <value>
        /// The campus identifier.
        /// </value>
        [HideFromReporting]
        [DataMember]
        [FieldType( Rock.SystemGuid.FieldType.CAMPUS )]
        public int? CampusId { get; set; }

        #endregion Entity Properties

        #region Virtual Properties

        [LavaInclude]
        public virtual PersonAlias PersonAlias { get; set; }

        [LavaInclude]
        public virtual PersonAlias SubmitterPersonAlias { get; set; }

        [LavaInclude]
        public virtual DefinedValue Status { get; set; }

        [LavaInclude]
        public virtual DefinedValue Category { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Campus"/> that this Care Need is associated with.
        /// </summary>
        /// <value>
        /// The <see cref="Campus"/> that this Care Need is associated with.
        /// </value>
        [DataMember]
        public virtual Campus Campus { get; set; }

        [LavaInclude]
        public virtual ICollection<AssignedPerson> AssignedPersons { get; set; }

        [LavaInclude]
        public virtual ICollection<CareNote> CareNotes { get; set; }

        #endregion Virtual Properties
    }

    #region Entity Configuration

    public partial class CareNeedConfiguration : EntityTypeConfiguration<CareNeed>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CareNeedConfiguration"/> class.
        /// </summary> 
        public CareNeedConfiguration()
        {
            //this.HasRequired( r => r.Reservation ).WithMany( r => r.ReservationLocations ).HasForeignKey( r => r.ReservationId ).WillCascadeOnDelete( true );

            this.HasRequired( cn => cn.PersonAlias ).WithMany().HasForeignKey( cn => cn.PersonAliasId ).WillCascadeOnDelete( false );
            this.HasRequired( cn => cn.SubmitterPersonAlias ).WithMany().HasForeignKey( cn => cn.SubmitterAliasId ).WillCascadeOnDelete( false );
            this.HasOptional( cn => cn.Status ).WithMany().HasForeignKey( cn => cn.StatusValueId ).WillCascadeOnDelete( false );
            this.HasOptional( cn => cn.Category ).WithMany().HasForeignKey( cn => cn.CategoryValueId ).WillCascadeOnDelete( false );
            this.HasOptional( p => p.Campus ).WithMany().HasForeignKey( p => p.CampusId ).WillCascadeOnDelete( false );


            // IMPORTANT!!
            this.HasEntitySetName( "CareNeed" );
        }
    }

    #endregion

}