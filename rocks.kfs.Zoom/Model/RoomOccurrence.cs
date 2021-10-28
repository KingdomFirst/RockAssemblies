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
namespace rocks.kfs.Zoom.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.ModelConfiguration;
    using System.Runtime.Serialization;
    using Rock.Data;
    using Rock.Model;

    /// <summary>
    /// A Zoom Room Occurrence
    /// </summary>
    [Table( "_rocks_kfs_ZoomRoomOccurrence" )]
    [DataContract]
    public class RoomOccurrence : Rock.Data.Model<RoomOccurrence>, Rock.Data.IRockEntity
    {
        #region Entity Properties

        [DataMember]
        public Int64 MeetingId { get; set; }

        [Required]
        [DataMember]
        public int ScheduleId { get; set; }

        [Required]
        [DataMember]
        public int LocationId { get; set; }

        [Required]
        [DataMember]
        public string Topic { get; set; }

        [Required]
        [DataMember]
        public DateTime StartTime { get; set; }

        [DataMember]
        public string TimeZone { get; set; }

        [DataMember]
        public string Password { get; set; }

        [Required]
        [DataMember]
        public int Duration { get; set; }

        [DataMember]
        public DateTime? SendAt { get; set; }

        [Required]
        [DataMember]
        public bool IsOccurring { get; set; } = true;

        [Required]
        [DataMember]
        public bool IsCompleted { get; set; } = false;

        [DataMember]
        public int? EntityTypeId { get; set; }

        [DataMember]
        public int? EntityId { get; set; }

        #endregion Entity Properties

        #region Virtual Properties

        [LavaInclude]
        public virtual Schedule Schedule { get; set; }

        [LavaInclude]
        public virtual Location Location { get; set; }

        [LavaInclude]
        public virtual EntityType EntityType { get; set; }

        #endregion Virtual Properties

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format( "{0} ({1}): {2}", Location.Name, LocationId, Schedule.EffectiveEndDate?.ToString( "m/d/yy H:mm tt" ) );
        }
    }
    #region Entity Configuration

    public partial class RoomOccurrenceConfiguration : EntityTypeConfiguration<RoomOccurrence>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RoomOccurrenceConfiguration"/> class.
        /// </summary> 
        public RoomOccurrenceConfiguration()
        {
            this.HasRequired( ro => ro.Schedule ).WithMany().HasForeignKey( ro => ro.ScheduleId ).WillCascadeOnDelete( false );
            this.HasRequired( ro => ro.Location ).WithMany().HasForeignKey( cn => cn.LocationId ).WillCascadeOnDelete( false );


            // IMPORTANT!!
            this.HasEntitySetName( "RoomOccurrence" );
        }
    }

    #endregion Entity Configuration
}
