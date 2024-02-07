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
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.ModelConfiguration;
    using System.Runtime.Serialization;
    using Rock.Data;
    using Rock.Model;

    /// <summary>
    /// Assigned Persons for Care Needs
    /// </summary>
    [Table( "_rocks_kfs_StepsToCare_AssignedPerson" )]
    [DataContract]
    public class AssignedPerson : Rock.Data.Model<AssignedPerson>, Rock.Data.IRockEntity
    {
        #region Entity Properties

        [DataMember]
        public int? NeedId { get; set; }

        [Required]
        [DataMember]
        public int? PersonAliasId { get; set; }

        [DataMember]
        public int? WorkerId { get; set; }

        [DataMember]
        public bool? FollowUpWorker { get; set; }

        [DataMember]
        public AssignedType? Type { get; set; }

        [DataMember]
        public string TypeQualifier { get; set; }

        #endregion Entity Properties

        #region Virtual Properties

        [LavaInclude]
        public virtual CareNeed CareNeed { get; set; }

        [LavaInclude]
        public virtual PersonAlias PersonAlias { get; set; }

        [LavaInclude]
        public virtual CareWorker CareWorker { get; set; }

        #endregion Virtual Properties
    }

    public enum AssignedType
    {
        Worker,
        [Description("Group Role")]
        GroupRole,
        Geofence,
        Manual
    }

    #region Entity Configuration

    public partial class AssignedPersonConfiguration : EntityTypeConfiguration<AssignedPerson>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AssignedPersonConfiguration"/> class.
        /// </summary> 
        public AssignedPersonConfiguration()
        {
            this.HasRequired( ap => ap.CareNeed ).WithMany( cn => cn.AssignedPersons ).HasForeignKey( ap => ap.NeedId ).WillCascadeOnDelete( true );
            this.HasRequired( ap => ap.CareWorker ).WithMany( cn => cn.AssignedPersons ).HasForeignKey( ap => ap.WorkerId ).WillCascadeOnDelete( true );
            this.HasRequired( ap => ap.PersonAlias ).WithMany().HasForeignKey( ap => ap.PersonAliasId ).WillCascadeOnDelete( false );

            // IMPORTANT!!
            this.HasEntitySetName( "AssignedPerson" );
        }
    }

    #endregion
}