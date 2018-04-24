﻿
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Data.Entity;
using System.Linq;

using Rock;
using Rock.Attribute;
using Rock.CheckIn;
using Rock.Data;
using Rock.Model;
using Rock.Web.UI.Controls;
using Rock.Workflow;

namespace Rock.Workflow.Action.CheckIn
{
    /// <summary>
    /// Saves the selected check-in data as attendance
    /// </summary>
    //[ActionCategory( "Check-In" )]
    [Description( "Saves the selected check-in data as attendance" )]
    [Export( typeof( ActionComponent ) )]
    [ExportMetadata( "ComponentName", "Save Attendance" )]
    [IntegerField( "Security Code Length", "The number of characters to use for the security code.", true, 3 )]
    [BooleanField( "Reuse Code For Family", "By default a unique security code is created for each person.  Select this option to use one security code per family.", false )]
    [BooleanField( "Checkout Other Groups", "By default a person can be checked into multiple groups at one time.  This will cancel other checkins for the day.", false )]
    [BooleanField( "First Checkin Not Transit", "Keep the first checkin of the day from being marked as In Transit.", false )]
    public class SaveAttendanceInTransit : CheckInActionComponent
    {
        /// <summary>
        /// Executes the specified workflow.
        /// </summary>
        /// <param name="rockContext">The rock context.</param>
        /// <param name="action">The workflow action.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="errorMessages">The error messages.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override bool Execute( RockContext rockContext, Model.WorkflowAction action, Object entity, out List<string> errorMessages )
        {
            var checkInState = GetCheckInState( entity, out errorMessages );
            if ( checkInState != null )
            {
                AttendanceCode attendanceCode = null;
                DateTime startDateTime = RockDateTime.Now;

                bool reuseCodeForFamily = GetAttributeValue( action, "ReuseCodeForFamily" ).AsBoolean();
                bool checkoutOtherGroups = GetAttributeValue( action, "CheckoutOtherGroups" ).AsBoolean();
                bool firstCheckinNotTransit = GetAttributeValue( action, "FirstCheckinNotTransit" ).AsBoolean();

                int securityCodeLength = 3;
                if ( !int.TryParse( GetAttributeValue( action, "SecurityCodeLength" ), out securityCodeLength ) )
                {
                    securityCodeLength = 3;
                }

                var attendanceCodeService = new AttendanceCodeService( rockContext );
                var attendanceService = new AttendanceService( rockContext );
                var groupMemberService = new GroupMemberService( rockContext );
                var personAliasService = new PersonAliasService( rockContext );

                foreach ( var family in checkInState.CheckIn.Families.Where( f => f.Selected ) )
                {
                    foreach ( var person in family.People.Where( p => p.Selected ) )
                    {
                        if ( reuseCodeForFamily && attendanceCode != null )
                        {
                            person.SecurityCode = attendanceCode.Code;
                        }
                        else
                        {
                            attendanceCode = AttendanceCodeService.GetNew( securityCodeLength );
                            person.SecurityCode = attendanceCode.Code;
                        }

                        foreach ( var groupType in person.GroupTypes.Where( g => g.Selected ) )
                        {
                            foreach ( var group in groupType.Groups.Where( g => g.Selected ) )
                            {
                                foreach ( var location in group.Locations.Where( l => l.Selected ) )
                                {
                                    if ( groupType.GroupType.AttendanceRule == AttendanceRule.AddOnCheckIn &&
                                        groupType.GroupType.DefaultGroupRoleId.HasValue &&
                                        !groupMemberService.GetByGroupIdAndPersonId( group.Group.Id, person.Person.Id, true ).Any() )
                                    {
                                        var groupMember = new GroupMember();
                                        groupMember.GroupId = group.Group.Id;
                                        groupMember.PersonId = person.Person.Id;
                                        groupMember.GroupRoleId = groupType.GroupType.DefaultGroupRoleId.Value;
                                        groupMemberService.Add( groupMember );
                                    }

                                    foreach ( var schedule in location.Schedules.Where( s => s.Selected ) )
                                    {
                                        // Only create one attendance record per day for each person/schedule/group/location
                                        var attendance = attendanceService.Get( startDateTime, location.Location.Id, schedule.Schedule.Id, group.Group.Id, person.Person.Id );
                                        if ( attendance == null )
                                        {
                                            var primaryAlias = personAliasService.GetPrimaryAlias( person.Person.Id );
                                            if ( primaryAlias != null )
                                            {
                                                attendance = rockContext.Attendances.Create();
                                                attendance.LocationId = location.Location.Id;
                                                attendance.CampusId = location.CampusId;
                                                attendance.ScheduleId = schedule.Schedule.Id;
                                                attendance.GroupId = group.Group.Id;
                                                attendance.PersonAlias = primaryAlias;
                                                attendance.PersonAliasId = primaryAlias.Id;
                                                attendance.DeviceId = checkInState.Kiosk.Device.Id;
                                                attendance.SearchTypeValueId = checkInState.CheckIn.SearchType.Id;
                                                attendanceService.Add( attendance );
                                                attendance.DidAttend = false;

                                                if ( firstCheckinNotTransit )
                                                {
                                                    List<Attendance> attendances = new List<Attendance>();
                                                    var attendancesService = new AttendanceService( rockContext );
                                                    var qryAttendance = attendancesService.Queryable();
                                                    qryAttendance = qryAttendance.Where( a => a.PersonAlias.PersonId == person.Person.Id );
                                                    qryAttendance = qryAttendance.Where( a => DbFunctions.TruncateTime( a.StartDateTime ) == RockDateTime.Today );
                                                    attendances.AddRange( qryAttendance );
                                                    if ( attendances.Count == 0 )
                                                    {
                                                        attendance.DidAttend = true;
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            attendance.DidAttend = true;
                                        }

                                        if ( checkoutOtherGroups )
                                        {
                                            List<Attendance> attendances = new List<Attendance>();
                                            var attendancesService = new AttendanceService( rockContext );
                                            var qryAttendance = attendancesService.Queryable();
                                            qryAttendance = qryAttendance.Where( a => a.PersonAlias.PersonId == person.Person.Id );
                                            qryAttendance = qryAttendance.Where( a => a.EndDateTime == null );
                                            qryAttendance = qryAttendance.Where( a => DbFunctions.TruncateTime( a.StartDateTime ) == RockDateTime.Today );
                                            attendances.AddRange( qryAttendance );
                                            foreach ( var otherAttendance in attendances )
                                            {
                                                if ( otherAttendance.Guid != attendance.Guid )
                                                {
                                                    otherAttendance.EndDateTime = RockDateTime.Now;
                                                    otherAttendance.DidAttend = false;
                                                }
                                            }
                                        }

                                        attendance.AttendanceCodeId = attendanceCode.Id;
                                        attendance.StartDateTime = startDateTime;
                                        if ( attendance.EndDateTime == null )
                                        {
                                            attendance.EndDateTime = null;
                                        }

                                        KioskLocationAttendance.AddAttendance( attendance );
                                    }
                                }
                            }
                        }
                    }
                }

                rockContext.SaveChanges();
                return true;
            }

            return false;
        }
    }
}