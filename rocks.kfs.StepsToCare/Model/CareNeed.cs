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
namespace rocks.kfs.StepsToCare.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Runtime.Serialization;
    using System.Data.Entity.ModelConfiguration;
    using Rock.Data;
    using Rock.Lava;
    using Rock.Model;
    using Rock.Web.Cache;
    using System.Linq;
    using System.Data.Entity;

    /// <summary>
    /// A Care Need
    /// </summary>
    [Table( "_rocks_kfs_StepsToCare_CareNeed" )]
    [DataContract]
    public class CareNeed : Rock.Data.Model<CareNeed>, Rock.Data.IRockEntity
    {
        private int _touchCount = -1;

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

        [DataMember]
        public bool WorkersOnly { get; set; }

        [DataMember]
        public int? ParentNeedId { get; set; }

        [DataMember]
        public bool CustomFollowUp { get; set; }

        [DataMember]
        public int? RenewPeriodDays { get; set; }

        [DataMember]
        public int? RenewMaxCount { get; set; }

        [DataMember]
        public int RenewCurrentCount { get; set; } = 0;

        [DataMember]
        public DateTime? SnoozeDate { get; set; }

        #endregion Entity Properties

        #region Virtual Properties

        [LavaVisible]
        public virtual PersonAlias PersonAlias { get; set; }

        [LavaVisible]
        public virtual PersonAlias SubmitterPersonAlias { get; set; }

        [LavaVisible]
        public virtual DefinedValue Status { get; set; }

        [LavaVisible]
        public virtual DefinedValue Category { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Campus"/> that this Care Need is associated with.
        /// </summary>
        /// <value>
        /// The <see cref="Campus"/> that this Care Need is associated with.
        /// </value>
        [DataMember]
        public virtual Campus Campus { get; set; }

        [LavaVisible]
        public virtual CareNeed ParentNeed { get; set; }

        [LavaVisible]
        public virtual ICollection<AssignedPerson> AssignedPersons { get; set; }

        [LavaVisible]
        public virtual ICollection<CareNeed> ChildNeeds { get; set; }

        #endregion Virtual Properties

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format( "{0} ({1})", Details, PersonAlias?.ToString() );
        }

        /// <summary>
        /// Returns a <see cref="System.Int32"/> count of <see cref="Note"/>'s attached to Care Need as Care Touches.
        /// </summary>
        [LavaVisible]
        public int TouchCount
        {
            get
            {
                if ( _touchCount == -1 )
                {
                    using ( var rockContext = new RockContext() )
                    {
                        var noteType = NoteTypeCache.GetByEntity( EntityTypeCache.Get( typeof( CareNeed ) ).Id, "", "", true ).FirstOrDefault();
                        if ( noteType != null )
                        {
                            var careNeedNotes = new NoteService( rockContext )
                                .GetByNoteTypeId( noteType.Id ).AsNoTracking()
                                .Where( n => n.EntityId == Id && n.Caption != "Action" );

                            _touchCount = careNeedNotes.Count();
                        }
                        else
                        {
                            _touchCount = 0;
                        }
                    }
                }
                return _touchCount;
            }
        }
    }

    #region Entity Configuration

    public partial class CareNeedConfiguration : EntityTypeConfiguration<CareNeed>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CareNeedConfiguration"/> class.
        /// </summary> 
        public CareNeedConfiguration()
        {
            this.HasRequired( cn => cn.PersonAlias ).WithMany().HasForeignKey( cn => cn.PersonAliasId ).WillCascadeOnDelete( false );
            this.HasRequired( cn => cn.SubmitterPersonAlias ).WithMany().HasForeignKey( cn => cn.SubmitterAliasId ).WillCascadeOnDelete( false );
            this.HasOptional( cn => cn.Status ).WithMany().HasForeignKey( cn => cn.StatusValueId ).WillCascadeOnDelete( false );
            this.HasOptional( cn => cn.Category ).WithMany().HasForeignKey( cn => cn.CategoryValueId ).WillCascadeOnDelete( false );
            this.HasOptional( cn => cn.Campus ).WithMany().HasForeignKey( cn => cn.CampusId ).WillCascadeOnDelete( false );
            // CascadeOnDelete is managed by a trigger that will set to null if parent need is deleted.
            this.HasOptional( cn => cn.ParentNeed ).WithMany( cn => cn.ChildNeeds ).HasForeignKey( cn => cn.ParentNeedId ).WillCascadeOnDelete( false );

            // IMPORTANT!!
            this.HasEntitySetName( "CareNeed" );
        }
    }

    #endregion

}