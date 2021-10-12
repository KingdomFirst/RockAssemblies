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
    using System.Data.Entity.ModelConfiguration;
    using Rock.Data;
    using Rock.Model;

    /// <summary>
    /// A Zoom Room Occurrence
    /// </summary>
    public class RoomOccurrence : Rock.Data.Model<RoomOccurrence>, Rock.Data.IRockEntity
    {
        #region Entity Properties

        public int ScheduleId { get; set; }

        public int LocationId { get; set; }

        public string Topic { get; set; }

        public DateTime StartTime { get; set; }

        public string TimeZone { get; set; }

        public string Password { get; set; }

        public int Duration { get; set; }

        public DateTime? SendAt { get; set; }

        public bool IsOccurring { get; set; } = true;

        public bool IsCompleted { get; set; } = false;

        public int? EntityTypeId { get; set; }

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
            this.HasRequired( ro => ro.EntityType ).WithMany().HasForeignKey( cn => cn.EntityTypeId ).WillCascadeOnDelete( false );


            // IMPORTANT!!
            this.HasEntitySetName( "RoomOccurrence" );
        }
    }

    #endregion Entity Configuration
}
