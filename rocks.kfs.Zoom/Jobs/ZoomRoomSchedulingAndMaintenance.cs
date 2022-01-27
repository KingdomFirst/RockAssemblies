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
using ZoomDotNetFramework.Entities;
using ZoomDotNetFramework.Enums;

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

    [BooleanField(
        "Import Zoom Room Meetings",
        Description = "Create Room Reservations for any Zoom Room meetings scheduled outside of Rock. This will help reduce the chances of the Room Reservation plugin scheduling a conflict with other Zoom Room meetings.",
        DefaultBooleanValue = false,
        IsRequired = false,
        Order = 3,
        Key = AttributeKey.ImportMeetings )]

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
            public const string ImportMeetings = "ImportMeetings";
        }

        private int errorCount = 0;
        private string webhookBaseUrl;
        private int reservationLocationEntityTypeId;
        private bool verboseLogging;

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

            using ( var rockContext = new RockContext() )
            {
                #region Setup Variables

                int jobId = context.JobDetail.Description.AsInteger();
                var job = new ServiceJobService( rockContext ).GetNoTracking( jobId );
                var JobStartDateTime = RockDateTime.Now;
                DateTime? lastSuccessRunDateTime = null;
                if ( job != null && job.Guid != Rock.SystemGuid.ServiceJob.JOB_PULSE.AsGuid() )
                {
                    lastSuccessRunDateTime = job.LastSuccessfulRunDateTime;
                }

                // get the last run date or yesterday
                var beginDateTime = lastSuccessRunDateTime ?? JobStartDateTime.AddDays( -1 );

                var dataMap = context.JobDetail.JobDataMap;
                var daysOut = dataMap.GetIntegerFromString( AttributeKey.SyncDaysOut );
                webhookBaseUrl = Settings.GetWebhookUrl();
                var importMeetings = dataMap.GetBooleanFromString( AttributeKey.ImportMeetings );
                verboseLogging = dataMap.GetBooleanFromString( AttributeKey.VerboseLogging );
                var zrOccurrencesCancel = new List<RoomOccurrence>();
                reservationLocationEntityTypeId = new EntityTypeService( rockContext ).GetNoTracking( com.bemaservices.RoomManagement.SystemGuid.EntityType.RESERVATION_LOCATION.AsGuid() ).Id;

                var zoom = new Zoom();
                var logService = new ServiceLogService( rockContext );
                var locationService = new LocationService( rockContext );
                var zrLocations = locationService.Queryable()
                                                 .AsNoTracking()
                                                 .WhereAttributeValue( rockContext, a => a.Attribute.Key == "rocks.kfs.KFSZoomRoom" && a.Value != null && a.Value != "" )
                                                 .ToList();
                var zrLocationIds = zrLocations.Select( l => l.Id ).ToList();
                var linkedZoomRoomLocations = new Dictionary<int, string>();
                foreach ( var loc in zrLocations )
                {
                    loc.LoadAttributes();
                    var zoomRoomDT = DefinedValueCache.Get( loc.AttributeValues.FirstOrDefault( v => v.Key == "rocks.kfs.KFSZoomRoom" ).Value.Value.AsGuid() );
                    linkedZoomRoomLocations.Add( loc.Id, zoomRoomDT.Value );
                }

                #endregion Setup Variables

                #region Mark Completed Occurrences

                var zrOccurrenceService = new RoomOccurrenceService( rockContext );
                var completedOccurrences = zrOccurrenceService.Queryable()
                                                              .Where( ro => ro.IsCompleted == false
                                                                && DbFunctions.AddMinutes( ro.StartTime, ro.Duration ) < beginDateTime );
                foreach ( var occ in completedOccurrences )
                {
                    occ.IsCompleted = true;
                }
                rockContext.SaveChanges();

                #endregion Mark Completed Occurrences

                #region Cleanup

                var reservationLocationService = new ReservationLocationService( rockContext );
                var reservationLocationIds = reservationLocationService.Queryable().AsNoTracking().Select( rl => rl.Id );

                // Delete any orphaned RoomOccurrences ( tied to invalid/deleted ReservationId )
                zrOccurrenceService = new RoomOccurrenceService( rockContext );
                var orphanedOccs = zrOccurrenceService.Queryable()
                                                      .Where( ro => ro.EntityTypeId == reservationLocationEntityTypeId
                                                        && !reservationLocationIds.Any( id => id == ro.EntityId ) );
                if ( orphanedOccs.Count() > 0 )
                {
                    if ( verboseLogging )
                    {
                        LogEvent( rockContext, "Zoom Room Reservation Sync", string.Format( "Preparing to delete {0} orphaned RoomOccurrence(s)." ) );
                    }
                    zrOccurrenceService.DeleteRange( orphanedOccs );
                    var errors = new List<string>();
                    LogEvent( null, "Zoom Room Reservation Sync", string.Format( "{0} orphaned RoomOccurrence(s) deleted.", orphanedOccs.Count() ) );
                    if ( verboseLogging )
                    {
                        LogEvent( null, "Zoom Room Reservation Sync", "Deleting related Zoom Meetings." );
                    }
                    DeleteOccurrenceZoomMeetings( rockContext, orphanedOccs, zoom );
                    rockContext.SaveChanges();
                }

                // Check for active Room Occurrences tied to Zoom Meetings that no longer exist.
                var linkedOccurrences = zrOccurrenceService
                                        .Queryable()
                                        .AsNoTracking()
                                        .Where( ro => ro.EntityTypeId == reservationLocationEntityTypeId
                                            && ro.ZoomMeetingId > 0
                                            && !ro.IsCompleted
                                            && ro.IsOccurring
                                            && ro.StartTime >= beginDateTime );
                var zoomMeetings = new List<ZoomDotNetFramework.Entities.Meeting>();
                foreach ( var zrl in linkedZoomRoomLocations )
                {
                    zoomMeetings.AddRange( zoom.GetZoomRoomMeetings( zrl.Value, MeetingListType.Upcoming ) );
                }
                var zoomMeetingIds = zoomMeetings.Select( m => m.Id ).ToList();
                var orphanedOccurrences = linkedOccurrences.Where( ro => !zoomMeetingIds.Any( mid => mid == ro.ZoomMeetingId ) );
                if ( orphanedOccurrences.Count() > 0 )
                {
                    zrOccurrenceService.DeleteRange( orphanedOccurrences );
                    rockContext.SaveChanges();
                }

                #endregion Cleanup

                #region External Zoom Meetings

                var scheduleService = new ScheduleService( rockContext );
                var reservationService = new ReservationService( rockContext );
                var reservationTypeService = new ReservationTypeService( rockContext );
                var zoomImportReservationType = reservationTypeService.Get( RoomReservationType.ZOOMROOMIMPORT.AsGuid() );

                // Create RoomOccurrences for any Zoom Room meetings created outside of Rock
                if ( importMeetings && linkedZoomRoomLocations.Count > 0 )
                {
                    var linkedMeetings = linkedOccurrences.Select( ro => ro.ZoomMeetingId ).ToList();
                    var zoomRoomMeetings = zoomMeetings.Where( m => m.Start_Time > beginDateTime );
                    var missingMeetings = zoomRoomMeetings.Where( m => !linkedMeetings.Any( mid => mid == m.Id ) );
                    if ( missingMeetings.Count() > 0 )
                    {
                        foreach ( var zrl in linkedZoomRoomLocations )
                        {
                            var resNameString = "Imported from ZoomRooms";
                            foreach ( var meeting in missingMeetings.Where( m => m.Host_Id == zrl.Value ) )
                            {
                                // Build the iCal string as it is a required property on the Schedule for Room Reservation block to display the Reservation
                                var meetingLocalTime = meeting.Start_Time.UtcDateTime.ToLocalTime();
                                var calendarEvent = new Event
                                {
                                    DtStart = new CalDateTime( meetingLocalTime ),
                                    DtEnd = new CalDateTime( meetingLocalTime.AddMinutes( meeting.Duration ) ),
                                    DtStamp = new CalDateTime( meetingLocalTime.Year, meetingLocalTime.Month, meetingLocalTime.Day )
                                };
                                var calendar = new Calendar();
                                calendar.Events.Add( calendarEvent );
                                var serializer = new CalendarSerializer( calendar );

                                var schedule = new Schedule
                                {
                                    Guid = Guid.NewGuid(),
                                    EffectiveStartDate = meetingLocalTime,
                                    EffectiveEndDate = meetingLocalTime.AddMinutes( meeting.Duration ),
                                    IsActive = true,
                                    iCalendarContent = serializer.SerializeToString()
                                };
                                scheduleService.Add( schedule );
                                var newReservation = new Reservation
                                {
                                    Name = !string.IsNullOrWhiteSpace( meeting.Topic ) ? string.Format( "{0} ({1})", meeting.Topic, resNameString ) : resNameString,
                                    ReservationTypeId = zoomImportReservationType.Id,
                                    Guid = Guid.NewGuid(),
                                    Schedule = schedule,
                                    NumberAttending = 0,
                                    Note = string.Format( "Created from import of \"{0}\" meeting ({1}) from Zoom Room \"{2}\".", meeting.Topic, meeting.Id, zrl.Value ),
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
                                    Schedule = schedule,
                                    LocationId = reservationLocation.LocationId,
                                    Topic = meeting.Topic,
                                    StartTime = meetingLocalTime,
                                    Password = meeting.Password,
                                    Duration = meeting.Duration,
                                    TimeZone = meeting.Timezone
                                };
                                zrOccurrenceService.Add( occurrence );
                                rockContext.SaveChanges();
                            }
                        }
                    }
                }

                #endregion External Zoom Meetings

                #region Process Reservations

                var reservations = reservationService.Queryable( "Schedule,ReservationLocations,ReservationType" )
                                                     .AsNoTracking()
                                                     .Where( r => r.ModifiedDateTime >= beginDateTime
                                                         && r.ReservationTypeId != zoomImportReservationType.Id
                                                         && ( r.ApprovalState == ReservationApprovalState.Approved
                                                            || ( !r.ReservationType.IsReservationBookedOnApproval && r.ApprovalState != ReservationApprovalState.Cancelled && r.ApprovalState != ReservationApprovalState.Denied && r.ApprovalState != ReservationApprovalState.Draft ) )
                                                         && r.ReservationLocations.Any( rl => zrLocationIds.Contains( rl.LocationId ) )
                                                         && r.Schedule != null
                                                         && ( ( r.Schedule.EffectiveEndDate != null && r.Schedule.EffectiveEndDate > DbFunctions.AddDays( RockDateTime.Today, -1 ) )
                                                             || ( r.Schedule.EffectiveEndDate == null && r.Schedule.EffectiveStartDate != null && r.Schedule.EffectiveStartDate > DbFunctions.AddDays( RockDateTime.Today, -1 ) ) ) )
                                                     .ToList();
                var resLocationIdsToProcess = new List<int>();
                var zrOccurrencesAdded = 0;
                if ( verboseLogging )
                {
                    LogEvent( rockContext, "Zoom Room Reservation Sync", string.Format( "{0} Room Reservation(s) to be processed", reservations.Count() ) );
                }
                foreach ( var res in reservations )
                {
                    foreach ( var rl in res.ReservationLocations.Where( rl => zrLocationIds.Contains( rl.LocationId ) ).ToList() )
                    {
                        rl.Location.LoadAttributes();
                        var zoomRoomDV = DefinedValueCache.Get( rl.Location.AttributeValues.FirstOrDefault( v => v.Key == "rocks.kfs.KFSZoomRoom" ).Value.Value.AsGuid() );
                        var zrPassword = zoomRoomDV.GetAttributeValue( "rocks.kfs.ZoomMeetingPassword" );
                        var joinBeforeHost = zoomRoomDV.GetAttributeValue( "rocks.kfs.ZoomJoinBeforeHost" ).AsBoolean();
                        resLocationIdsToProcess.Add( rl.Id );
                        var resLocOccurrences = zrOccurrenceService.Queryable().Where( ro => ro.EntityTypeId == reservationLocationEntityTypeId && ro.EntityId == rl.Id );

                        // One-Time Schedule
                        if ( res.Schedule.EffectiveEndDate is null || res.Schedule.EffectiveStartDate.Value == res.Schedule.EffectiveEndDate.Value )
                        {
                            var occurrence = new RoomOccurrence();
                            if ( resLocOccurrences.Count() == 0 )
                            {
                                occurrence = new RoomOccurrence
                                {
                                    Id = 0,
                                    EntityTypeId = reservationLocationEntityTypeId,
                                    EntityId = rl.Id,
                                    ScheduleId = res.ScheduleId,
                                    LocationId = rl.LocationId,
                                    Topic = res.Name,
                                    StartTime = res.Schedule.FirstStartDateTime.Value,
                                    Password = zrPassword,
                                    Duration = res.Schedule.DurationInMinutes,
                                    IsOccurring = true,
                                    IsCompleted = false
                                };
                                zrOccurrenceService.Add( occurrence );
                                rockContext.SaveChanges();
                                zrOccurrencesAdded++;
                                //var meetingSettings = new MeetingSetting
                                //{
                                //    Join_Before_Host = joinBeforeHost
                                //};
                                //var meetingToCreate = new Meeting
                                //{
                                //    Topic = occurrence.Topic,
                                //    Type = MeetingType.Scheduled,
                                //    Start_Time = occurrence.StartTime.ToRockDateTimeOffset(),
                                //    Duration = occurrence.Duration,
                                //    Password = occurrence.Password,
                                //    Settings = meetingSettings
                                //};
                                //var newMeeting = zoom.CreateZoomMeeting( zoomRoomDT.Value, meetingToCreate );
                                //var success = false;
                                //if ( newMeeting != null )
                                //{
                                //    success = true;
                                //    occurrence.ZoomMeetingId = newMeeting.Id;
                                //    occurrence.ZoomMeetingJoinUrl = newMeeting.Join_Url;
                                //}

                                saveChanges = false;
                                CreateOccurrenceZoomMeeting( occurrence, zoomRoomDV.Value, joinBeforeHost, out saveChanges );

                                if ( saveChanges )
                                {
                                    rockContext.SaveChanges();
                                }
                            }
                            else
                            {
                                Meeting connectedMeeting = null;
                                var updateMeeting = false;
                                occurrence = resLocOccurrences.FirstOrDefault();
                                if ( occurrence.ZoomMeetingId.HasValue && occurrence.ZoomMeetingId.Value > 0 )
                                {
                                    connectedMeeting = zoomMeetings.FirstOrDefault( m => m.Id == occurrence.ZoomMeetingId.Value );
                                    if ( connectedMeeting == null )
                                    {
                                        occurrence.ZoomMeetingId = null;
                                    }
                                }
                                if ( !occurrence.IsOccurring )
                                {
                                    occurrence.IsOccurring = true;
                                }
                                if ( occurrence.IsCompleted )
                                {
                                    occurrence.IsCompleted = false;
                                }
                                if ( occurrence.ScheduleId != res.ScheduleId )
                                {
                                    occurrence.ScheduleId = res.ScheduleId;
                                }
                                if ( occurrence.StartTime != res.Schedule.FirstStartDateTime.Value )
                                {
                                    occurrence.StartTime = res.Schedule.FirstStartDateTime.Value;
                                }
                                if ( connectedMeeting != null && connectedMeeting.Start_Time != occurrence.StartTime.ToRockDateTimeOffset() )
                                {
                                    connectedMeeting.Start_Time = occurrence.StartTime.ToRockDateTimeOffset();
                                    updateMeeting = true;
                                }
                                if ( occurrence.Duration != res.Schedule.DurationInMinutes )
                                {
                                    occurrence.Duration = res.Schedule.DurationInMinutes;
                                    if ( connectedMeeting != null )
                                    {
                                        connectedMeeting.Duration = res.Schedule.DurationInMinutes;
                                        updateMeeting = true;
                                    }
                                }
                                if ( occurrence.Topic != res.Name )
                                {
                                    occurrence.Topic = res.Name;
                                    if ( connectedMeeting != null )
                                    {
                                        connectedMeeting.Topic = res.Name;
                                        updateMeeting = true;
                                    }
                                }
                                if ( updateMeeting )
                                {
                                    zoom.UpdateZoomMeeting( connectedMeeting );
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
                            zrOccurrencesAdded += CreateZoomRoomOccurrences( zrOccurrenceService, zoomRoomDV.Value, res.Name, zrPassword, res.Schedule, joinBeforeHost, rl, reservationDateTimes, resLocOccurrences );

                            // Capture existing Zoom Room Occurrences where the target Reservation "occurrence" no longer exists. This would happen if the Room Reservation schedule has been changed.
                            var resStartDateTimes = reservationDateTimes.Select( rdt => rdt.StartDateTime ).ToList();
                            foreach ( var roomOcc in resLocOccurrences.Where( o => o.IsOccurring && !resStartDateTimes.Any( st => st == o.StartTime ) ) )
                            {
                                zrOccurrenceService.Delete( roomOcc );
                                if ( roomOcc.ZoomMeetingId.HasValue && roomOcc.ZoomMeetingId.Value > 0 )
                                {
                                    zoom.DeleteZoomMeeting( roomOcc.ZoomMeetingId.Value );
                                }
                            }
                        }
                    }
                }
                rockContext.SaveChanges();
                LogEvent( rockContext, "Zoom Room Reservation Sync", string.Format( "{0} Zoom Room Occurrence(s) Created", zrOccurrencesAdded ) );

                #endregion Process Reservations
            }
        }

        private void CreateOccurrenceZoomMeeting( RoomOccurrence occurrence, string zoomRoomId, bool joinBeforeHost, out bool changesMade )
        {
            changesMade = false;
            if ( verboseLogging )
            {
                LogEvent( null, "Zoom Room Reservation Sync", string.Format( "Begin create new Zoom Meeting Request: ZoomRoom {0} - {1}", zoomRoomId, occurrence.StartTime.ToShortDateTimeString() ) );
            }
            var callbackUrl = string.Format( "{0}?token={1}", webhookBaseUrl, occurrence.Id );
            var success = new Zoom().ScheduleZoomRoomMeeting( zoomRoomId, occurrence.Password, occurrence.Topic, occurrence.StartTime.ToRockDateTimeOffset(), occurrence.Duration, joinBeforeHost, enableLogging: verboseLogging, callbackUrl: callbackUrl );
            if ( "test for offline logic" == "true" )
            {
                occurrence.ZoomMeetingRequestStatus = ZoomMeetingRequestStatus.ZoomRoomOffline;
                changesMade = true;
            }
            if ( verboseLogging )
            {
                LogEvent( null, "Zoom Room Reservation Sync", string.Format( "Create new Zoom Meeting {0}: ZoomRoom {1} - {2}", success ? "succeeded" : "failed", zoomRoomId, occurrence.StartTime.ToShortDateTimeString() ) );
            }
        }

        private void DeleteOccurrenceZoomMeetings( RockContext rockContext, IQueryable<RoomOccurrence> occurrencesToCancel, Zoom zoom )
        {
            var errors = new List<string>();
            if ( verboseLogging )
            {
                LogEvent( null, "Zoom Room Reservation Sync", string.Format( "{0} Zoom meeting(s) to delete.", occurrencesToCancel.Count() ) );
            }
            foreach ( var roomOcc in occurrencesToCancel )
            {
                if ( !zoom.DeleteZoomMeeting( roomOcc.ZoomMeetingId.Value ) )
                {
                    errors.Add( string.Format( "Unable to delete Zoom meeting ({0}).", roomOcc.ZoomMeetingId ) );
                }
            }
            if ( verboseLogging && errors.Count > 0 )
            {
                foreach ( var error in errors )
                {
                    LogEvent( null, "Zoom Room Reservation Sync", "An error was encountered while trying to delete Zoom Meeting(s).", error );
                }
            }
            LogEvent( null, "Zoom Room Reservation Sync", string.Format( "{0} Zoom Meetings successfully deleted.", occurrencesToCancel.Count() - errors.Count ) );
        }

        private int CreateZoomRoomOccurrences( RoomOccurrenceService occService, string roomId, string occurrenceTopic, string password, Schedule schedule, bool joinBeforeHost, ReservationLocation reservationLocation, List<ReservationDateTime> reservationDateTimes, IQueryable<RoomOccurrence> existingRoomOccurrences )
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
                    occurrencesAdded++;
                    var meetingSettings = new MeetingSetting
                    {
                        Join_Before_Host = joinBeforeHost
                    };
                    //var meetingToCreate = new Meeting
                    //{
                    //    Topic = occurrence.Topic,
                    //    Type = MeetingType.Scheduled,
                    //    Start_Time = occurrence.StartTime.ToRockDateTimeOffset(),
                    //    Duration = occurrence.Duration,
                    //    Password = occurrence.Password,
                    //    Settings = meetingSettings
                    //};
                    //var zoom = new Zoom();
                    //var newMeeting = zoom.CreateZoomMeeting( roomId, meetingToCreate );
                    //var success = false;
                    //if ( newMeeting != null )
                    //{
                    //    success = true;
                    //    occurrence.ZoomMeetingId = newMeeting.Id;
                    //    occurrence.ZoomMeetingJoinUrl = newMeeting.Join_Url;
                    //}
                    var changesMade = false;
                    CreateOccurrenceZoomMeeting( occurrence, roomId, joinBeforeHost, out changesMade );
                }
            }
            return occurrencesAdded;
        }

        private static ServiceLog LogEvent( RockContext rockContext, string type, string input, string result = null )
        {
            if ( rockContext == null )
            {
                rockContext = new RockContext();
            }
            var rockLogger = new ServiceLogService( rockContext );
            ServiceLog serviceLog = new ServiceLog
            {
                Name = "ZoomRoom",
                Type = type,
                LogDateTime = RockDateTime.Now,
                Input = input,
                Result = result,
                Success = true
            };
            rockLogger.Add( serviceLog );
            rockContext.SaveChanges();
            return serviceLog;
        }
    }
}