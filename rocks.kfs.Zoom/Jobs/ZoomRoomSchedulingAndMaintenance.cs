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
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Web;
using com.bemaservices.RoomManagement.Model;
using Quartz;
using Rock;
using Rock.Attribute;
using Rock.Communication;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;
using rocks.kfs.Zoom;
using rocks.kfs.Zoom.Model;

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
        Key = AttributeKey.SyncDaysOut )]

    [BooleanField(
        "Enable Verbose Logging",
        Description = "Turn on extra logging points in addition to the standard job logging points. This is only recommended for testing/troubleshooting purposes.",
        DefaultBooleanValue = false,
        IsRequired = false,
        Key = AttributeKey.VerboseLogging )]

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
        }

        private int errorCount = 0;
        private int reservationLocationEntityTypeId;

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
            var dataMap = context.JobDetail.JobDataMap;
            var JobStartDateTime = RockDateTime.Now;
            var daysOut = dataMap.GetIntegerFromString( AttributeKey.SyncDaysOut );
            var verboseLogging = dataMap.GetBooleanFromString( AttributeKey.VerboseLogging );

            using ( var rockContext = new RockContext() )
            {
                DateTime? lastSuccessRunDateTime = null;
                ServiceLogService logService = new ServiceLogService( rockContext );

                // get job type id
                int jobId = context.JobDetail.Description.AsInteger();

                // load job
                var job = new ServiceJobService( rockContext )
                    .GetNoTracking( jobId );

                if ( job != null && job.Guid != Rock.SystemGuid.ServiceJob.JOB_PULSE.AsGuid() )
                {
                    lastSuccessRunDateTime = job.LastSuccessfulRunDateTime;
                }

                // get the last run date or yesterday
                var beginDateTime = lastSuccessRunDateTime ?? JobStartDateTime.AddDays( -1 );

                var locationService = new LocationService( rockContext );
                reservationLocationEntityTypeId = new EntityTypeService( rockContext ).GetNoTracking( com.bemaservices.RoomManagement.SystemGuid.EntityType.RESERVATION_LOCATION.AsGuid() ).Id;
                var zrLocationIds = locationService.Queryable()
                                                 .AsNoTracking()
                                                 .WhereAttributeValue( rockContext, a => a.Attribute.Key == "KFSZoomRoom" && a.Value != null && a.Value != "" )
                                                 .Select( l => l.Id )
                                                 .ToList();

                var reservationService = new ReservationService( rockContext );
                var reservations = reservationService.Queryable( "Schedule,ReservationLocations" )
                                                     .AsNoTracking()
                                                     .Where( r => r.ModifiedDateTime >= beginDateTime
                                                         && r.ApprovalState == ReservationApprovalState.Approved
                                                         && r.ReservationLocations.Any( rl => zrLocationIds.Contains( rl.LocationId ) )
                                                         && r.Schedule != null
                                                         && ( ( r.Schedule.EffectiveEndDate != null && r.Schedule.EffectiveEndDate > DbFunctions.AddDays( RockDateTime.Today, -1 ) )
                                                             || ( r.Schedule.EffectiveEndDate == null && r.Schedule.EffectiveStartDate != null && r.Schedule.EffectiveStartDate > DbFunctions.AddDays( RockDateTime.Today, -1 ) ) ) )
                                                     .ToList();                
                var reservationLocationIds = new List<int>();
                var zrOccurrenceService = new RoomOccurrenceService( rockContext );
                var zrOccurrencesCancel = new List<RoomOccurrence>();
                var zrOccurrencesAdded = 0;

                if ( verboseLogging )
                {
                    AddLogEntry( logService, "Zoom Room Reservation Sync", string.Format( "{0} Room Reservation(s) to be processed", reservations.Count() ), true );
                }

                // Create new Zoom Room Occurrences and Zoom Meetings where needed
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
                                var success = new Zoom().ScheduleZoomRoomMeeting( rockContext, zoomRoomDT.Value, occurrence.Password, occurrence.Topic, occurrence.StartTime.ToRockDateTimeOffset(), occurrence.Duration, joinBeforeHost, enableLogging: verboseLogging );
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
                                endDate = RockDateTime.Today.AddDays( daysOut + 1 ).AddSeconds( -1 ); // Set to last second of days out date since we are working with DateTimes
                            }
                            var reservationDateTimes = res.GetReservationTimes( beginDateTime, endDate );
                            zrOccurrencesAdded += CreateZoomRoomOccurrences( rockContext, zrOccurrenceService, zoomRoomDT.Value, res.Name, zrPassword, res.Schedule, joinBeforeHost, rl, reservationDateTimes, zrOccurrences, logService, verboseLogging );

                            // Capture existing Zoom Room Occurrences where the target Reservation "occurrence" no longer exists. This would happen if the Room Reservation schedule has been changed.
                            zrOccurrencesCancel.AddRange( zrOccurrences.Where( o => o.IsOccurring && !reservationDateTimes.Any( rdt => rdt.StartDateTime == o.StartTime ) ) );
                        }
                    }
                }

                AddLogEntry( logService, "Zoom Room Reservation Sync", string.Format( "{0} Zoom Room Occurrence(s) Created", zrOccurrencesAdded ), true );

                // Collect all Zoom Room Occurrences tied to non-approved Room Reservations
                zrOccurrencesCancel.AddRange( zrOccurrenceService.Queryable().Where( ro => ro.EntityTypeId == reservationLocationEntityTypeId
                                                                            && ro.StartTime > RockDateTime.Now
                                                                            && !reservationLocationIds.Contains( ro.EntityId.Value )
                                                                            && ro.IsOccurring ) );
                if ( verboseLogging )
                {
                    AddLogEntry( logService, "Zoom Room Reservation Sync", string.Format( "{0} Zoom Room Occurrence(s) to cancel", zrOccurrencesCancel.Count() ), true );
                }
                if ( zrOccurrencesCancel.Count() > 0 )
                {
                    var apiAuthorized = Zoom.ZoomAuthCheck();
                    if ( !apiAuthorized )
                    {
                        AddLogEntry( logService, "Zoom Room Reservation Sync", "Zoom Api not able to authenticate. Please check your Zoom Api settings in Rock. Unable to cancel Zoom Room Occurrences.", false );
                    }
                    else
                    {
                        var zoom = Zoom.Api();
                        var errors = new List<string>();
                        foreach ( var roomOcc in zrOccurrencesCancel )
                        {
                            roomOcc.Location.LoadAttributes();
                            var zoomRoomDT = DefinedValueCache.Get( roomOcc.Location.AttributeValues.FirstOrDefault( v => v.Key == "KFSZoomRoom" ).Value.Value.AsGuid() );
                            roomOcc.IsOccurring = false;
                            if ( !zoom.CancelZoomRoomMeeting( zoomRoomDT.Value, roomOcc.Topic, roomOcc.StartTime.ToRockDateTimeOffset(), roomOcc.TimeZone, roomOcc.Duration ) )
                            {
                                errors.Add( string.Format( "Unable to cancel Zoom Room Meeting tied to RoomReservation {0}", roomOcc.EntityId ) );
                            }
                        }
                        rockContext.SaveChanges();
                        if ( verboseLogging && errors.Count > 0 )
                        {
                            AddLogEntry( logService, "Zoom Room Reservation Sync", "An error was encountered while trying to cancel Zoom Meeting(s).", false, errors );
                        }
                        AddLogEntry( logService, "Zoom Room Reservation Sync", string.Format( "{0} Zoom Meetings successfully canceled.", zrOccurrencesCancel.Count() - errors.Count ), true );
                    }
                }
                //StringBuilder resultSB = new StringBuilder();
                //resultSB.AppendFormat( "{0} contributions processed.", ( successfulTx + unsuccessfulTxList.Count ) );
                //resultSB.AppendFormat( "{0} contributions pushed to Managed Missions.\n", successfulTx );

                //if ( unsuccessfulTxList.Count > 0 )
                //{
                //    resultSB.AppendFormat( "{0} transactions were unsuccessful.\n\n", unsuccessfulTxList.Count );
                //    resultSB.AppendLine( "Unsuccessful Transactions:" );
                //    foreach ( var item in unsuccessfulTxList )
                //    {
                //        resultSB.AppendLine( item.ToString() );
                //    }
                //}

                //context.Result = resultSB.ToString();
            }
        }

        private int CreateZoomRoomOccurrences( RockContext rockContext, RoomOccurrenceService occService, string roomId, string occurrenceTopic, string passsword, Schedule schedule, bool joinBeforeHost, ReservationLocation reservationLocation, List<ReservationDateTime> reservationDateTimes, IQueryable<RoomOccurrence> existingRoomOccurrences, ServiceLogService logService, bool verboseLogging = false )
        {
            var occurrencesAdded = 0;
            foreach ( var dateTime in reservationDateTimes )
            {
                var existingRoomOccurrencce = existingRoomOccurrences.Where( o => o.StartTime == dateTime.StartDateTime );
                if ( existingRoomOccurrencce.Count() == 0 )
                {
                    var occurrence = new RoomOccurrence
                    {
                        EntityTypeId = reservationLocationEntityTypeId,
                        EntityId = reservationLocation.Id,
                        ScheduleId = schedule.Id,
                        LocationId = reservationLocation.LocationId,
                        Topic = occurrenceTopic,
                        StartTime = dateTime.StartDateTime,
                        Password = passsword,
                        Duration = schedule.DurationInMinutes
                    };
                    occService.Add( occurrence );
                    rockContext.SaveChanges();
                    occurrencesAdded++;
                    if ( verboseLogging )
                    {
                        AddLogEntry( logService, "Zoom Room Reservation Sync", string.Format( "Begin create new Zoom Meeting: ZoomRoom {0} - {1}", roomId, occurrence.StartTime.ToShortDateTimeString() ), true );
                    }
                    var success = new Zoom().ScheduleZoomRoomMeeting( rockContext, roomId, occurrence.Password, occurrence.Topic, occurrence.StartTime.ToRockDateTimeOffset(), occurrence.Duration, joinBeforeHost, enableLogging: verboseLogging ); if ( verboseLogging )
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