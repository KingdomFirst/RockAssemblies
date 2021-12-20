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
using Ical.Net;
using Ical.Net.DataTypes;
using Ical.Net.Serialization.iCalendar.Serializers;
using Quartz;
using Rock;
using Rock.Attribute;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

using com.bemaservices.RoomManagement.Model;
using rocks.kfs.Zoom.Model;
using rocks.kfs.Zoom.ZoomGuid;
using rocks.kfs.ZoomRoom.Enums;
using ZoomDotNetFramework;

namespace rocks.kfs.Zoom.Jobs
{
    /// <summary>
    /// Job to send generate Zoom Room occurrences and sync Zoom Meeting schedules.
    /// </summary>

    [IntegerField(
        "Sync Days Out",
        Description = "Number of days into the future to sync Locations to Zoom Rooms if the Schedule does not have an effective date.",
        DefaultIntegerValue = 30,
        IsRequired = true,
        Order = 1,
        Key = AttributeKey.SyncDaysOut )]

    [BooleanField(
        "Enable Verbose Logging",
        Description = "Turn on extra logging points in addition to the standard job logging points. This is only recommended for testing/troubleshooting purposes.",
        DefaultBooleanValue = false,
        IsRequired = false,
        Order = 2,
        Key = AttributeKey.VerboseLogging )]

    //[TextField(
    //    "Webhook Base URL",
    //    Description = "The base URL to target the Zoom Webhook plugin to process new meetings being created.",
    //    DefaultValue = "",
    //    IsRequired = true,
    //    Key = AttributeKey.WebhookBaseUrl )]

    [BooleanField(
        "Import Zoom Room Meetings",
        Description = "Create Room Reservations for any Zoom Room meetings scheduled outside of Rock. This will help reduce the chances of the Room Reservation plugin scheduling a conflict with other Zoom Room meetings.",
        DefaultBooleanValue = false,
        IsRequired = false,
        Order = 3,
        Key = AttributeKey.ImportMeetings )]

    //[IntegerField(
    //    "Imported Reservation Type Id",
    //    Description = "The Reservation Type Id to use for reservations created by imported Zoom Room meetings.",
    //    DefaultIntegerValue = 1,
    //    IsRequired = true,
    //    Key = AttributeKey.ReservationTypeId )]

    //[IntegerField(
    //    "Imported Reservation Setup Time",
    //    Description = "The Reservation Setup Time ( in minutes ) to use for reservations created by imported Zoom Room meetings.",
    //    DefaultIntegerValue = 30,
    //    IsRequired = true,
    //    Key = AttributeKey.ReservationSetupTime )]

    //[IntegerField(
    //    "Imported Reservation Setup Time",
    //    Description = "The Reservation Cleanup Time ( in minutes ) to use for reservations created by imported Zoom Room meetings.",
    //    DefaultIntegerValue = 30,
    //    IsRequired = true,
    //    Key = AttributeKey.ReservationCleanupTime )]

    //[PersonField(
    //    "Imported Reservation Person",
    //    Description = "The person record to use for all elements requiring person or person alias properties on reservations created by imported Zoom Room meetings.",
    //    IsRequired = true,
    //    Key = AttributeKey.ReservationPerson )]

    [DisallowConcurrentExecution]
    public class ZoomRoomSchedulingAndMaintenance : IJob
    {
        /// <summary>
        /// Attribute Keys
        /// </summary>
        private static class AttributeKey
        {
            public const string SyncDaysOut = "SyncDaysOut";
            public const string VerboseLogging = "VerboseLogging";
            //public const string WebhookBaseUrl = "WebhookBaseUrl";
            //public const string ReservationTypeId = "ReservationTypeId";
            //public const string ReservationSetupTime = "ReservationSetupTime";
            //public const string ReservationCleanupTime = "ReservationCleanupTime";
            //public const string ReservationPerson = "ReservationPerson";
            public const string ImportMeetings = "ImportMeetings";
        }

        private int errorCount = 0;
        private int reservationLocationEntityTypeId;
        private string webhookBaseUrl;

        private List<string> errorMessages = new List<string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ZoomRoomSchedulingAndMaintenance"/> class.
        /// </summary>
        public ZoomRoomSchedulingAndMaintenance()
        {
        }

        /// <summary>
        /// Executes the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        public virtual void Execute( IJobExecutionContext context )
        {
            // Check Api connection first.
            if ( !Zoom.ZoomAuthCheck() )
            {
                var strng = "What do I do here?";
                return;
            }

            var JobStartDateTime = RockDateTime.Now;
            DateTime? lastSuccessRunDateTime = null;
            DateTime beginDateTime;
            int jobId = context.JobDetail.Description.AsInteger();
            var dataMap = context.JobDetail.JobDataMap;
            var daysOut = dataMap.GetIntegerFromString( AttributeKey.SyncDaysOut );
            webhookBaseUrl = Settings.GetWebhookUrl();
            var importMeetings = dataMap.GetBooleanFromString( AttributeKey.ImportMeetings );
            var verboseLogging = dataMap.GetBooleanFromString( AttributeKey.VerboseLogging );

            var zrLocationIds = new List<int>();
            var linkedZoomRoomLocations = new Dictionary<int, string>();
            var zrOccurrencesCancel = new List<RoomOccurrence>();
            ZoomApi zoom = null;

            // Start with cleanup

            using ( var rockContext = new RockContext() )
            {
                // load job
                var job = new ServiceJobService( rockContext )
                    .GetNoTracking( jobId );

                if ( job != null && job.Guid != Rock.SystemGuid.ServiceJob.JOB_PULSE.AsGuid() )
                {
                    lastSuccessRunDateTime = job.LastSuccessfulRunDateTime;
                }

                // get the last run date or yesterday
                beginDateTime = lastSuccessRunDateTime ?? JobStartDateTime.AddDays( -1 );

                // Start collecting any orphaned RoomOccurrences

                reservationLocationEntityTypeId = new EntityTypeService( rockContext ).GetNoTracking( com.bemaservices.RoomManagement.SystemGuid.EntityType.RESERVATION_LOCATION.AsGuid() ).Id;
                var zrOccurrenceService = new RoomOccurrenceService( rockContext );
                var reservationLocationService = new ReservationLocationService( rockContext );
                var locationService = new LocationService( rockContext );
                var zrLocations = locationService.Queryable()
                                                 .AsNoTracking()
                                                 .WhereAttributeValue( rockContext, a => a.Attribute.Key == "KFSZoomRoom" && a.Value != null && a.Value != "" )
                                                 .ToList();
                zrLocationIds = zrLocations.Select( l => l.Id ).ToList();
                foreach ( var loc in zrLocations )
                {
                    loc.LoadAttributes();
                    var zoomRoomDT = DefinedValueCache.Get( loc.AttributeValues.FirstOrDefault( v => v.Key == "KFSZoomRoom" ).Value.Value.AsGuid() );
                    linkedZoomRoomLocations.Add( loc.Id, zoomRoomDT.Value );
                }
                var reservationLocationIds = reservationLocationService.Queryable().AsNoTracking().Select( rl => rl.Id );
                var occsForDelete = zrOccurrenceService.Queryable().Where( ro => ro.EntityTypeId == reservationLocationEntityTypeId
                                                                    && !reservationLocationIds.Any( id => id == ro.EntityId ) )
                                                                   .ToList();
                //var occsForDelete = orphanedOccs.Select( ro => ro.Id ).ToList();

                // Now collect any Zoom Room Occurrences tied to Reservations that no longer meet the criteria to have active RoomOccurrences

                //var linkedResLocations = zrOccurrenceService.Queryable().AsNoTracking()
                //                                     .Where( ro => ro.EntityTypeId == reservationLocationEntityTypeId && ro.StartTime > beginDateTime )
                //                                     .Select( ro => new
                //                                     {
                //                                         RoomLocationId = ro.EntityId,
                //                                         OccurrenceStartTime = ro.StartTime
                //                                     } )
                //                                     .ToList();
                //var reservationService = new ReservationService( rockContext );
                //var reservationsToClean = reservationService.Queryable( "Schedule,ReservationLocations,ReservationType" )
                //                                     .AsNoTracking()
                //                                     .Where( r => r.ModifiedDateTime >= beginDateTime
                //                                        && r.ReservationLocations.Any( rl => linkedResLocations.Any( ll => ll.RoomLocationId == rl.Id ) )
                //                                        && (
                //                                            !( r.ApprovalState == ReservationApprovalState.Approved
                //                                                || ( !r.ReservationType.IsReservationBookedOnApproval && r.ApprovalState != ReservationApprovalState.Cancelled && r.ApprovalState != ReservationApprovalState.Denied ) )
                //                                            || r.Schedule == null
                //                                            || r.ReservationLocations.Any( rl => linkedResLocations.Any( ll => ll.RoomLocationId == rl.Id && r.Schedule.EffectiveStartDate != ll.OccurrenceStartTime ) )
                //                                            )
                //                                        );
                //var resLocIdsForDelete = new List<int>();
                //foreach ( var res in reservationsToClean )
                //{
                //    resLocIdsForDelete.AddRange( res.ReservationLocations.Select( rl => rl.Id ).ToList() );
                //}
                //occsForDelete.AddRange( zrOccurrenceService.Queryable().AsNoTracking()
                //                                                        .Where( ro => ro.EntityTypeId == reservationLocationEntityTypeId
                //                                                            && resLocIdsForDelete.Contains( ro.EntityId.Value )
                //                                                            && ro.StartTime > RockDateTime.Now )
                //                                                       .ToList() );

                // Now delete any occurrences that were collected for cleanup

                if ( occsForDelete.Count > 0 )
                {
                    var logService = new ServiceLogService( rockContext );
                    DeleteRoomOccurrences( verboseLogging, zrOccurrencesCancel, zoom, logService );
                    zrOccurrenceService.DeleteRange( occsForDelete );

                    zoom = Zoom.Api();
                    var errors = new List<string>();
                    AddLogEntry( logService, "Cleanup RoomOccurrences", string.Format( "{0} orphaned RoomOccurrence(s) deleted.", occsForDelete.Count ), true );
                    if ( verboseLogging )
                    {
                        AddLogEntry( logService, "Cleanup RoomOccurrences", "Deleting related Zoom Meetings.", true );
                    }
                    foreach ( var roomOcc in occsForDelete )
                    {
                        //var loc = locationService.Get( roomOcc.LocationId );
                        //loc.LoadAttributes();
                        //var zoomRoomDT = DefinedValueCache.Get( loc.AttributeValues.FirstOrDefault( v => v.Key == "KFSZoomRoom" ).Value.Value.AsGuid() );
                        roomOcc.IsOccurring = false;
                        if ( !zoom.DeleteMeeting( roomOcc.ZoomMeetingId ) )
                        {
                            errors.Add( string.Format( "Unable to delete Zoom Room Meeting ({0}).", roomOcc.ZoomMeetingId ) );
                        }
                        //if ( !zoom.CancelZoomRoomMeeting( zoomRoomDT.Value, roomOcc.Topic, roomOcc.StartTime.ToRockDateTimeOffset(), roomOcc.TimeZone, roomOcc.Duration ) )
                        //{
                        //    errors.Add( string.Format( "Unable to cancel Zoom Room Meeting ({0}) tied to RoomReservation {1}", roomOcc.ZoomMeetingId, roomOcc.EntityId ) );
                        //}
                    }
                    if ( errors.Count > 0 )
                    {
                        AddLogEntry( logService, "Cleanup RoomOccurrences", "An error was encountered while trying to delete Zoom Meeting(s).", false, errors );
                    }
                    AddLogEntry( logService, "Cleanup RoomOccurrences", string.Format( "{0} Zoom Meeting(s) deleted successfully.{1}", occsForDelete.Count - errors.Count, errors.Count > 0 ? errors.Count + " Meeting(s) failed to cancel." : string.Empty ), true );
                }                                             
                rockContext.SaveChanges();
            }

            // Create RoomOccurrences for any Zoom Room meetings created outside of Rock
            if ( importMeetings && linkedZoomRoomLocations.Count > 0 )
            {
                if ( zoom == null )
                {
                    zoom = Zoom.Api();
                }
                using ( var rockContext = new RockContext() )
                {
                    // Check for custom Zoom Room Reservation Type and add if it does not already exist.
                    var reservationTypeService = new ReservationTypeService( rockContext );
                    var zoomReservationType = reservationTypeService.Get( RoomReservationType.ZOOMROOMIMPORT.AsGuid() );
                    if ( zoomReservationType == null )
                    {
                        zoomReservationType = new ReservationType
                        {
                            Guid = RoomReservationType.ZOOMROOMIMPORT.AsGuid(),
                            Name = "Zoom Room Import",
                            Description = "For use with Reservations created from Zoom Room meetings imported from Zoom api. WARNING: Making any changes to the configuration of this type could result in undesired behavior of synchronization with external Zoom Room resources.",
                            IconCssClass = "fa fa-video",
                            IsSetupTimeRequired = false,
                            IsNumberAttendingRequired = false,
                            IsContactDetailsRequired = false,
                            IsReservationBookedOnApproval = false,
                            IsSystem = true,
                            IsActive = true
                        };
                        reservationTypeService.Add( zoomReservationType );
                    }
                    var zrOccurrenceService = new RoomOccurrenceService( rockContext );
                    var linkedMeetings = zrOccurrenceService
                                            .Queryable()
                                            .AsNoTracking()
                                            .Where( ro => ro.EntityTypeId == reservationLocationEntityTypeId && ro.ZoomMeetingId > 0 )
                                            .Select( ro => ro.ZoomMeetingId )
                                            .ToList();
                    foreach ( var zrl in linkedZoomRoomLocations )
                    {
                        var zoomRoomMeetings = zoom.GetZoomMeetings( zrl.Value, MeetingListType.Upcoming );
                        var missingMeetings = zoomRoomMeetings.Where( m => !linkedMeetings.Any( mid => mid == m.Id ) );
                        if ( missingMeetings.Count() > 0 )
                        {
                            var locationService = new LocationService( rockContext );
                            var reservationService = new ReservationService( rockContext );
                            var reservationLocationService = new ReservationLocationService( rockContext );
                            var scheduleService = new ScheduleService( rockContext );
                            var personAliasService = new PersonAliasService( rockContext );
                            var resNameString = "Imported from ZoomRooms";
                            //var reservationTypeId = dataMap.GetIntegerFromString( AttributeKey.ReservationTypeId );
                            //var reservationSetupTime = dataMap.GetIntegerFromString( AttributeKey.ReservationSetupTime );
                            //var reservationCleanupTime = dataMap.GetIntegerFromString( AttributeKey.ReservationCleanupTime );
                            //var reservationPerson = personAliasService.Get( dataMap.GetString( AttributeKey.ReservationPerson ).AsGuid() ).Person;
                            foreach ( var meeting in missingMeetings )
                            {
                                // Build the iCal string as it is a required property on the Schedule for Room Reservation block to display the Reservation
                                var calendarEvent = new Event
                                {
                                    DtStart = new CalDateTime( meeting.Start_Time ),
                                    DtEnd = new CalDateTime( meeting.Start_Time.AddMinutes( meeting.Duration ) ),
                                    DtStamp = new CalDateTime( meeting.Start_Time.Year, meeting.Start_Time.Month, meeting.Start_Time.Day )
                                };
                                var calendar = new Calendar();
                                calendar.Events.Add( calendarEvent );
                                var serializer = new CalendarSerializer( calendar );

                                var schedule = new Schedule
                                {
                                    Guid = Guid.NewGuid(),
                                    EffectiveStartDate = meeting.Start_Time,
                                    EffectiveEndDate = meeting.Start_Time.AddMinutes( meeting.Duration ),
                                    IsActive = true,
                                    iCalendarContent = serializer.SerializeToString()
                                };
                                scheduleService.Add( schedule );
                                var newReservation = new Reservation
                                {
                                    Name = !string.IsNullOrWhiteSpace( meeting.Topic ) ? string.Format( "{0} ({1})", meeting.Topic, resNameString ) : resNameString,
                                    ReservationTypeId = zoomReservationType.Id,
                                    Guid = Guid.NewGuid(),
                                    Schedule = schedule,
                                    //SetupTime = reservationSetupTime,
                                    //CleanupTime = reservationCleanupTime,
                                    //RequesterAliasId = reservationPerson.PrimaryAliasId,
                                    NumberAttending = 0,
                                    Note = string.Format( "Created from import of \"{0}\" meeting ({1}) from Zoom Room \"{2}\".", meeting.Topic, meeting.Id, zrl.Value ),
                                    //CreatedByPersonAliasId = reservationPerson.PrimaryAliasId,
                                    //ModifiedByPersonAliasId = reservationPerson.PrimaryAliasId,
                                    //EventContactPersonAliasId = reservationPerson.PrimaryAliasId,
                                    //AdministrativeContactPersonAliasId = reservationPerson.PrimaryAliasId,
                                    //AdministrativeContactEmail = reservationPerson.Email,
                                    ApprovalState = ReservationApprovalState.Approved
                                };
                                reservationService.Add( newReservation );

                                var reservationLocation = new ReservationLocation
                                {
                                    Reservation = newReservation,
                                    LocationId = zrl.Key,
                                    ApprovalState = ReservationLocationApprovalState.Approved
                                };
                                reservationLocationService.Add( reservationLocation );

                                var occurrence = new RoomOccurrence
                                {
                                    ZoomMeetingId = meeting.Id,
                                    EntityTypeId = reservationLocationEntityTypeId,
                                    EntityId = reservationLocation.Id,
                                    ScheduleId = schedule.Id,
                                    LocationId = reservationLocation.LocationId,
                                    Topic = meeting.Topic,
                                    StartTime = meeting.Start_Time,
                                    Password = meeting.Password,
                                    Duration = meeting.Duration,
                                    TimeZone = meeting.Timezone
                                };
                                zrOccurrenceService.Add( occurrence );
                            }
                            rockContext.SaveChanges();
                        }
                    }
                }
            }

            // Create or Cancel/Delete Zoom Room Occurrences and Zoom Meetings where needed
            using ( var rockContext = new RockContext() )
            {
                var logService = new ServiceLogService( rockContext );

                var reservationService = new ReservationService( rockContext );
                var reservations = reservationService.Queryable( "Schedule,ReservationLocations,ReservationType" )
                                                     .AsNoTracking()
                                                     .Where( r => r.ModifiedDateTime >= beginDateTime
                                                         && ( r.ApprovalState == ReservationApprovalState.Approved
                                                            || ( !r.ReservationType.IsReservationBookedOnApproval && r.ApprovalState != ReservationApprovalState.Cancelled && r.ApprovalState != ReservationApprovalState.Denied ) )
                                                         && r.ReservationLocations.Any( rl => zrLocationIds.Contains( rl.LocationId ) )
                                                         && r.Schedule != null
                                                         && ( ( r.Schedule.EffectiveEndDate != null && r.Schedule.EffectiveEndDate > DbFunctions.AddDays( RockDateTime.Today, -1 ) )
                                                             || ( r.Schedule.EffectiveEndDate == null && r.Schedule.EffectiveStartDate != null && r.Schedule.EffectiveStartDate > DbFunctions.AddDays( RockDateTime.Today, -1 ) ) ) )
                                                     .ToList();
                var reservationLocationIds = new List<int>();
                var zrOccurrenceService = new RoomOccurrenceService( rockContext );
                var zrOccurrencesAdded = 0;

                if ( verboseLogging )
                {
                    AddLogEntry( logService, "Zoom Room Reservation Sync", string.Format( "{0} Room Reservation(s) to be processed", reservations.Count() ), true );
                }

                foreach ( var res in reservations )
                {
                    foreach ( var rl in res.ReservationLocations.Where( rl => zrLocationIds.Contains( rl.LocationId ) ).ToList() )
                    {
                        rl.Location.LoadAttributes();
                        var zoomRoomDT = DefinedValueCache.Get( rl.Location.AttributeValues.FirstOrDefault( v => v.Key == "KFSZoomRoom" ).Value.Value.AsGuid() );
                        var zrPassword = zoomRoomDT.AttributeValues.FirstOrDefault( v => v.Key == "ZoomMeetingPassword" ).Value.Value;
                        var joinBeforeHost = zoomRoomDT.AttributeValues.FirstOrDefault( v => v.Key == "ZoomJoinBeforeHost" ).Value.Value.AsBoolean();
                        reservationLocationIds.Add( rl.Id );
                        var zrOccurrences = zrOccurrenceService.Queryable().AsNoTracking().Where( ro => ro.EntityTypeId == reservationLocationEntityTypeId && ro.EntityId == rl.Id );

                        // One-Time Schedule
                        if ( res.Schedule.EffectiveEndDate is null || res.Schedule.EffectiveStartDate.Value == res.Schedule.EffectiveEndDate.Value )
                        {
                            if ( zrOccurrences.Count() == 0 )
                            {
                                var occurrence = new RoomOccurrence
                                {
                                    Id = 0,
                                    EntityTypeId = reservationLocationEntityTypeId,
                                    EntityId = rl.Id,
                                    ScheduleId = res.ScheduleId,
                                    LocationId = rl.LocationId,
                                    Topic = res.Name,
                                    StartTime = res.Schedule.FirstStartDateTime.Value,
                                    Password = zrPassword,
                                    Duration = res.Schedule.DurationInMinutes
                                };
                                zrOccurrenceService.Add( occurrence );
                                rockContext.SaveChanges();
                                zrOccurrencesAdded++;
                                if ( verboseLogging )
                                {
                                    AddLogEntry( logService, "Zoom Room Reservation Sync", string.Format( "Begin create new Zoom Meeting: ZoomRoom {0} - {1}", zoomRoomDT.Value, occurrence.StartTime.ToShortDateTimeString() ), true );
                                }
                                var callbackUrl = string.Format( "{0}?token={1}", webhookBaseUrl, occurrence.Id );
                                var success = new Zoom().ScheduleZoomRoomMeeting( rockContext, zoomRoomDT.Value, occurrence.Password, occurrence.Topic, occurrence.StartTime.ToRockDateTimeOffset(), occurrence.Duration, joinBeforeHost, enableLogging: verboseLogging, callbackUrl: callbackUrl );
                                if ( verboseLogging )
                                {
                                    AddLogEntry( logService, "Zoom Room Reservation Sync", string.Format( "Create new Zoom Meeting {0}: ZoomRoom {1} - {2}", success ? "succeeded" : "failed", zoomRoomDT.Value, occurrence.StartTime.ToShortDateTimeString() ), success );
                                }
                            }
                        }
                        // Recurring Schedule
                        else
                        {
                            DateTime endDate;

                            // Has valid end date - fixed duration
                            if ( res.Schedule.EffectiveEndDate < "12/31/9999".AsDateTime() )
                            {
                                endDate = res.Schedule.EffectiveEndDate.Value;
                            }
                            // No valid end date - continuous duration
                            else
                            {
                                endDate = RockDateTime.Today.AddDays( daysOut + 1 ).AddMilliseconds( -1 ); // Set to last millisecond of days out date since we are working with DateTimes
                            }
                            var reservationDateTimes = res.GetReservationTimes( beginDateTime, endDate );
                            zrOccurrencesAdded += CreateZoomRoomOccurrences( rockContext, zrOccurrenceService, zoomRoomDT.Value, res.Name, zrPassword, res.Schedule, joinBeforeHost, rl, reservationDateTimes, zrOccurrences, logService, verboseLogging );

                            // Capture existing Zoom Room Occurrences where the target Reservation "occurrence" no longer exists. This would happen if the Room Reservation schedule has been changed.
                            zrOccurrencesCancel.AddRange( zrOccurrences.Where( o => o.IsOccurring && !reservationDateTimes.Any( rdt => rdt.StartDateTime == o.StartTime ) ) );
                        }
                    }
                }

                AddLogEntry( logService, "Zoom Room Reservation Sync", string.Format( "{0} Zoom Room Occurrence(s) Created", zrOccurrencesAdded ), true );

                

                // Deal with meetings that need canceled on Zoom Room side 
                if ( zrOccurrencesCancel.Count() > 0 )
                {
                    if ( zoom == null )
                    {
                        zoom = Zoom.Api();
                    }
                    DeleteRoomOccurrences( verboseLogging, zrOccurrencesCancel, zoom, logService );
                }
                rockContext.SaveChanges();
            }
        }

        private void DeleteRoomOccurrences( bool verboseLogging, List<RoomOccurrence> occurrencesToCancel, ZoomApi zoom, ServiceLogService logService )
        {
            var errors = new List<string>();
            if ( verboseLogging )
            {
                AddLogEntry( logService, "Zoom Room Reservation Sync", string.Format( "{0} Zoom Room Occurrence(s) to cancel", occurrencesToCancel.Count() ), true );
            }
            foreach ( var roomOcc in occurrencesToCancel )
            {
                //roomOcc.Location.LoadAttributes();
                //var zoomRoomDT = DefinedValueCache.Get( roomOcc.Location.AttributeValues.FirstOrDefault( v => v.Key == "KFSZoomRoom" ).Value.Value.AsGuid() );
                roomOcc.IsOccurring = false;
                if ( !zoom.DeleteMeeting( roomOcc.ZoomMeetingId ) )
                {
                    errors.Add( string.Format( "Unable to delete Zoom meeting ({0}).", roomOcc.ZoomMeetingId ) );
                }
                //if ( !zoom.CancelZoomRoomMeeting( zoomRoomDT.Value, roomOcc.Topic, roomOcc.StartTime.ToRockDateTimeOffset(), roomOcc.TimeZone, roomOcc.Duration ) )
                //{
                //    errors.Add( string.Format( "Unable to cancel Zoom Room Meeting tied to RoomReservation {0}", roomOcc.EntityId ) );
                //}
            }
            if ( verboseLogging && errors.Count > 0 )
            {
                AddLogEntry( logService, "Zoom Room Reservation Sync", "An error was encountered while trying to delete Zoom Meeting(s).", false, errors );
            }
            AddLogEntry( logService, "Zoom Room Reservation Sync", string.Format( "{0} Zoom Meetings successfully deleted.", occurrencesToCancel.Count() - errors.Count ), true );
        }

        private int CreateZoomRoomOccurrences( RockContext rockContext, RoomOccurrenceService occService, string roomId, string occurrenceTopic, string password, Schedule schedule, bool joinBeforeHost, ReservationLocation reservationLocation, List<ReservationDateTime> reservationDateTimes, IQueryable<RoomOccurrence> existingRoomOccurrences, ServiceLogService logService, bool verboseLogging = false )
        {
            var occurrencesAdded = 0;
            foreach ( var dateTime in reservationDateTimes )
            {
                var existingRoomOccurrence = existingRoomOccurrences.Where( o => o.StartTime == dateTime.StartDateTime );
                if ( existingRoomOccurrence.Count() == 0 )
                {
                    var occurrence = new RoomOccurrence
                    {
                        EntityTypeId = reservationLocationEntityTypeId,
                        EntityId = reservationLocation.Id,
                        ScheduleId = schedule.Id,
                        LocationId = reservationLocation.LocationId,
                        Topic = occurrenceTopic,
                        StartTime = dateTime.StartDateTime,
                        Password = password,
                        Duration = schedule.DurationInMinutes
                    };
                    occService.Add( occurrence );
                    //rockContext.SaveChanges();
                    occurrencesAdded++;
                    if ( verboseLogging )
                    {
                        AddLogEntry( logService, "Zoom Room Reservation Sync", string.Format( "Begin create new Zoom Meeting: ZoomRoom {0} - {1}", roomId, occurrence.StartTime.ToShortDateTimeString() ), true );
                    }
                    var callbackUrl = string.Format( "{0}?token={1}", webhookBaseUrl, occurrence.Id );
                    var success = new Zoom().ScheduleZoomRoomMeeting( rockContext, roomId, occurrence.Password, occurrence.Topic, occurrence.StartTime.ToRockDateTimeOffset(), occurrence.Duration, joinBeforeHost, enableLogging: verboseLogging, callbackUrl: callbackUrl );
                    if ( verboseLogging )
                    {
                        AddLogEntry( logService, "Zoom Room Reservation Sync", string.Format( "Create new Zoom Meeting {0}: ZoomRoom {1} - {2}", success ? "succeeded" : "failed", roomId, occurrence.StartTime.ToShortDateTimeString() ), success );
                    }
                }
            }
            return occurrencesAdded;
        }

        private void AddLogEntry( ServiceLogService logService, string logType, string logName, bool logSuccess, List<string> logInputs = null )
        {
            if ( logInputs == null )
            {
                logInputs = new List<string>();
            }

            ServiceLog log = new ServiceLog
            {
                LogDateTime = RockDateTime.Now,
                Type = logType,
                Name = logName,
                Success = logSuccess
            };
            foreach ( var input in logInputs )
            {
                log.Input = input;
            }
            logService.Add( log );
        }
    }
}