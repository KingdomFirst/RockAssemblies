// <copyright>
// Copyright 2022 by Kingdom First Solutions
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
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.ModelConfiguration;
    using System.Runtime.Serialization;
    using Rock.Data;
    using Rock.Lava;
    using Rock.Model;

    [Table( "_rocks_kfs_StepsToCare_CareWorker" )]
    [DataContract]
    public partial class CareWorker : Rock.Data.Model<CareWorker>, Rock.Data.IRockEntity
    {
        #region Entity Properties

        [DataMember]
        public int? PersonAliasId { get; set; }

        [DataMember]
        public string CategoryValues { get; set; }

        [DataMember]
        public int? GeoFenceId { get; set; }

        /// <summary>
        /// Gets or sets the campuses for the worker.
        /// </summary>
        /// <value>
        /// Comma separated list of campus ID's
        /// </value>
        [HideFromReporting]
        [DataMember]
        public string Campuses { get; set; }

        [DataMember]
        public bool IsActive { get; set; }

        [DataMember]
        public bool NotifyEmail { get; set; }

        [DataMember]
        public bool NotifySMS { get; set; }

        [DataMember]
        public bool NotifyPush { get; set; }

        [DataMember]
        public decimal? AgeRangeMin { get; set; }

        [DataMember]
        public decimal? AgeRangeMax { get; set; }

        /// <summary>
        /// Gets or sets the gender of the people this Care Worker will be assigned to.
        /// </summary>
        /// <value>
        /// A <see cref="Rock.Model.Gender"/> enum value representing the peoples gender assigned to this worker.  Valid values are <c>Gender.Unknown</c> if the gender should not be limited,
        /// <c>Gender.Male</c> if the gender should be limited to Male, <c>Gender.Female</c> if the gender should be limited to Female.
        /// </value>
        [DataMember]
        public Gender? Gender { get; set; }
        #endregion Entity Properties

        #region Virtual Properties

        [LavaVisible]
        public virtual PersonAlias PersonAlias { get; set; }

        [LavaVisible]
        public virtual ICollection<AssignedPerson> AssignedPersons { get; set; }

        #endregion Virtual Properties

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return PersonAlias?.ToString();
        }
    }

    #region Entity Configuration

    public partial class CareWorkerConfiguration : EntityTypeConfiguration<CareWorker>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CareWorkerConfiguration"/> class.
        /// </summary> 
        public CareWorkerConfiguration()
        {
            this.HasRequired( cw => cw.PersonAlias ).WithMany().HasForeignKey( cw => cw.PersonAliasId ).WillCascadeOnDelete( false );

            // IMPORTANT!!
            this.HasEntitySetName( "CareWorker" );
        }
    }

    #endregion
}