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
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.ModelConfiguration;
    using System.Runtime.Serialization;
    using Rock.Data;
    using Rock.Model;

    [Table( "_rocks_kfs_StepsToCare_CareWorker" )]
    [DataContract]
    public partial class CareWorker : Rock.Data.Model<CareNote>, Rock.Data.IRockEntity
    {
        #region Entity Properties

        [DataMember]
        public int? PersonAliasId { get; set; }

        [DataMember]
        [DefinedValue( SystemGuid.DefinedType.CARE_NEED_CATEGORY )]
        public int? CategoryValueId { get; set; }

        [DataMember]
        public int? GeoFenceId { get; set; }

        #endregion Entity Properties

        #region Virtual Properties

        [LavaInclude]
        public virtual PersonAlias PersonAlias { get; set; }

        [LavaInclude]
        public virtual DefinedValue Category { get; set; }

        #endregion Virtual Properties
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
            this.HasRequired( cw => cw.Category ).WithMany().HasForeignKey( cw => cw.CategoryValueId ).WillCascadeOnDelete( false );

            // IMPORTANT!!
            this.HasEntitySetName( "CareWorker" );
        }
    }

    #endregion
}