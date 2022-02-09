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
using rocks.kfs.Zoom.Enums;
using System.Text;
using Rock.Jobs;
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

        private List<ZoomRoomOfflineResult> zoomRoomOfflineResultList = new List<ZoomRoomOfflineResult>();
        private int zrOfflineRequests = 0;
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
                context.Result = "Zoom API authentication error. Check API settings for Zoom Room plugin or try again later.";
                throw new Exception( "Authentication failed for Zoom API. Please verify the API settings configured in the Zoom Room plugin are valid and correct." );
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

                var zoom = Zoom.Api();
                var logService = new ServiceLogService( rockContext );
                var locationService = new LocationService( rockContext );
                var zrLocations = locationService.Queryable()
                                                 .AsNoTracking()
                                                 .WhereAttributeValue( rockContext, a => a.Attribute.Key == "rocks.kfs.ZoomRoom" && a.Value != null && a.Value != "" )
                                                 .ToList();
                var zrLocationIds = zrLocations.Select( l => l.Id ).ToList();
                var linkedZoomRoomLocations = new Dictionary<int, string>();
                foreach ( var loc in zrLocations )
                {
                    loc.LoadAttributes();
                    var zoomRoomDT = DefinedValueCache.Get( loc.AttributeValues.FirstOrDefault( v => v.Key == "rocks.kfs.ZoomRoom" ).Value.Value.AsGuid() );
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
                    var deleteCount = orphanedOccs.Count();
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
                    DeleteOccurrenceZoomMeetings( orphanedOccs, zoom );
                    rockContext.SaveChanges();
                }

                // Delete any active Room Occurrences tied to Zoom Meetings that no longer exist.
                var linkedOccurrences = zrOccurrenceService
                                        .Queryable()
                                        .AsNoTracking()
                                        .Where( ro => ro.EntityTypeId == reservationLocationEntityTypeId
                                            && ro.ZoomMeetingId > 0
                                            && !ro.IsCompleted
                                            && ro.StartTime >= beginDateTime );
                var zoomMeetings = new List<Meeting>();
                foreach ( var zrl in linkedZoomRoomLocations )
                {
                    zoomMeetings.AddRange( zoom.GetZoomMeetings( zrl.Value, MeetingListType.Upcoming ) );
                }
                var zoomMeetingIds = zoomMeetings.Select( m => m.Id ).ToList();
                var orphanedOccurrences = linkedOccurrences.Where( ro => !zoomMeetingIds.Any( mid => mid == ro.ZoomMeetingId ) );
                if ( orphanedOccurrences.Count() > 0 )
                {
                    var deleteCount = orphanedOccurrences.Count();
                    zrOccurrenceService.DeleteRange( orphanedOccurrences );
                    rockContext.SaveChanges();
                }

                // Attempt to create Zoom Room Meeting for any Room Occurrences that may have had previous issues.
                var unlinkedOccurrences = zrOccurrenceService
                                            .Queryable()
                                            .Where( ro => ro.EntityTypeId == reservationLocationEntityTypeId
                                                && ( !ro.ZoomMeetingId.HasValue || ro.ZoomMeetingId <= 0 )
                                                && !ro.IsCompleted
                                                && ro.StartTime >= beginDateTime
                                                && ( ro.ZoomMeetingRequestStatus == ZoomMeetingRequestStatus.Failed || ro.ZoomMeetingRequestStatus == ZoomMeetingRequestStatus.ZoomRoomOffline ) );

                foreach ( var rOcc in unlinkedOccurrences )
                {
                    var rLoc = reservationLocationService.Queryable( "Location" ).FirstOrDefault( rl => rl.Id == rOcc.EntityId );
                    rLoc.Location.LoadAttributes();
                    rOcc.ZoomMeetingRequestStatus = ZoomMeetingRequestStatus.Requested;
                    var zoomRoomDV = DefinedValueCache.Get( rLoc.Location.GetAttributeValue( "rocks.kfs.ZoomRoom" ).AsGuid() );
                    CreateOccurrenceZoomMeeting( rOcc, zoomRoomDV, zoom );
                }
                if ( unlinkedOccurrences.Count() > 0 )
                {
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
                        var zoomRoomDV = DefinedValueCache.Get( rl.Location.AttributeValues.FirstOrDefault( v => v.Key == "rocks.kfs.ZoomRoom" ).Value.Value.AsGuid() );
                        var zrPassword = zoomRoomDV.GetAttributeValue( "rocks.kfs.ZoomMeetingPassword" );
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
                                    IsCompleted = false,
                                    ZoomMeetingRequestStatus = ZoomMeetingRequestStatus.Requested
                                };
                                zrOccurrenceService.Add( occurrence );
                                rockContext.SaveChanges();
                                zrOccurrencesAdded++;

                                if ( CreateOccurrenceZoomMeeting( occurrence, zoomRoomDV, zoom ) )
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
                                    zoom.UpdateMeeting( connectedMeeting );
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
                            zrOccurrencesAdded += CreateZoomRoomOccurrences( zrOccurrenceService, zoomRoomDV, res.Name, zrPassword, res.Schedule, rl, reservationDateTimes, resLocOccurrences, zoom );

                            // Delete existing Zoom Room Occurrences where the target Reservation "occurrence" no longer exists. This would happen if the Room Reservation schedule has been changed.
                            var resStartDateTimes = reservationDateTimes.Select( rdt => rdt.StartDateTime ).ToList();
                            foreach ( var roomOcc in resLocOccurrences.Where( o => !resStartDateTimes.Any( st => st == o.StartTime ) ) )
                            {
                                zrOccurrenceService.Delete( roomOcc );
                                if ( roomOcc.ZoomMeetingId.HasValue && roomOcc.ZoomMeetingId.Value > 0 )
                                {
                                    zoom.DeleteMeeting( roomOcc.ZoomMeetingId.Value );
                                }
                            }
                        }
                    }
                }
                rockContext.SaveChanges();
                LogEvent( rockContext, "Zoom Room Reservation Sync", string.Format( "{0} Zoom Room Occurrence(s) Created", zrOccurrencesAdded ) );

                #endregion Process Reservations

                #region Final Summary Report

                StringBuilder errorSummaryBuilder = new StringBuilder();
                var resultList = zoomRoomOfflineResultList.OrderBy( r => r.Title )
                                                          .GroupBy( r => r.Title )
                                                          .Select( grp => new ZoomRoomOfflineResult
                                                          {
                                                              Title = string.Format( "{0} {1} {2} failed to schedule.", grp.Key, grp.Count(), "Zoom meeting".PluralizeIf( grp.Count() != 1 ) )
                                                          } );
                foreach ( var result in resultList )
                {
                    errorSummaryBuilder.AppendLine( GetFormattedResult( result ) );
                }
                context.Result = errorSummaryBuilder.ToString();

                var jobExceptions = zoomRoomOfflineResultList.Where( a => a.Exception != null ).Select( a => a.Exception ).ToList();

                if ( jobExceptions.Any() )
                {
                    var exceptionList = new AggregateException( "One or more exceptions occurred during Zoom Room Scheduling.", jobExceptions );
                    throw new RockJobWarningException( "Zoom Room Scheduling completed with warnings", exceptionList );
                }

                #endregion Final Summary Report

            }
        }

        /// <summary>
        /// Creates a Meeting for a Zoom Room using the Zoom Room api.
        /// </summary>
        /// <param name="occurrence"></param>
        /// <param name="zoomRoomId"></param>
        /// <param name="joinBeforeHost"></param>
        /// <returns>Boolean indicating if any changes were made to the provided occurrence object.</returns>
        private bool CreateOccurrenceZoomMeeting( RoomOccurrence occurrence, DefinedValueCache zoomRoomDV, ZoomApi zoomApi )
        {
            var joinBeforeHost = zoomRoomDV.GetAttributeValue( "rocks.kfs.ZoomJoinBeforeHost" ).AsBoolean();
            var changesMade = false;
            if ( verboseLogging )
            {
                LogEvent( null, "Zoom Room Reservation Sync", string.Format( "Begin create new Zoom Meeting Request: ZoomRoom {0} - {1}", zoomRoomDV.Value, occurrence.StartTime.ToShortDateTimeString() ) );
            }
            try
            {
                var callbackUrl = string.Format( "{0}?token={1}", webhookBaseUrl, occurrence.Id );

                if ( verboseLogging )
                {
                    LogEvent( null, "Zoom Room Reservation Sync", "Schedule Zoom Room Meeting", "Started" );
                }
                var success = zoomApi.ScheduleZoomRoomMeeting( zoomRoomDV.Value, occurrence.Password, callbackUrl, occurrence.Topic, occurrence.StartTime.ToRockDateTimeOffset(), string.Empty, occurrence.Duration, joinBeforeHost );
                if ( verboseLogging )
                {
                    LogEvent( null, "Zoom Room Reservation Sync", string.Format( "Create new Zoom Meeting {0}: ZoomRoom {1} - {2}", success ? "succeeded" : "failed", zoomRoomDV.Value, occurrence.StartTime.ToShortDateTimeString() ) );
                }
            }
            catch ( Exception ex )
            {
                if ( ex.Message.Contains( "Error Code 4008" ) )
                {
                    occurrence.ZoomMeetingRequestStatus = ZoomMeetingRequestStatus.ZoomRoomOffline;
                    changesMade = true;
                    zoomRoomOfflineResultList.Add( new ZoomRoomOfflineResult
                    {
                        Title = string.Format( "{0} ({1}) offline.", zoomRoomDV.Description, zoomRoomDV.Value ),
                        Exception = new Exception( string.Format( "{0} ({1}) is offline. Unable to schedule meeting for {2}.", zoomRoomDV.Description, zoomRoomDV.Value, occurrence.StartTime ) )
                    } );
                    LogEvent( null, "Zoom Room Reservation Sync", string.Format( "Create new Zoom Meeting failed for ZoomRoom {0} - {1}. Zoom Room client is offline.", zoomRoomDV.Value, occurrence.StartTime.ToShortDateTimeString() ) );
                }
                else
                {
                    throw ex;
                }
            }
            return changesMade;
        }

        private void DeleteOccurrenceZoomMeetings( IQueryable<RoomOccurrence> occurrencesToCancel, ZoomApi zoom )
        {
            var errors = new List<string>();
            if ( verboseLogging )
            {
                LogEvent( null, "Zoom Room Reservation Sync", string.Format( "{0} Zoom meeting(s) to delete.", occurrencesToCancel.Count() ) );
            }
            foreach ( var roomOcc in occurrencesToCancel )
            {
                if ( !zoom.DeleteMeeting( roomOcc.ZoomMeetingId.Value ) )
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

        private int CreateZoomRoomOccurrences( RoomOccurrenceService occService, DefinedValueCache zoomRoomDV, string occurrenceTopic, string password, Schedule schedule, ReservationLocation reservationLocation, List<ReservationDateTime> reservationDateTimes, IQueryable<RoomOccurrence> existingRoomOccurrences, ZoomApi zoomApi )
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
                    CreateOccurrenceZoomMeeting( occurrence, zoomRoomDV, zoomApi );
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

        /// <summary>
        /// Get a cleanup job result as a formatted string
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        private string GetFormattedResult( ZoomRoomOfflineResult result )
        {
            var icon = "<i class='fa fa-circle text-danger'></i>";
            return $"{icon} {result.Title}";
        }

        /// <summary>
        /// The result data from a cleanup task
        /// </summary>
        private class ZoomRoomOfflineResult
        {
            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            /// <value>
            /// The title.
            /// </value>
            public string Title { get; set; }

            public Exception Exception { get; set; }
        }
    }
}