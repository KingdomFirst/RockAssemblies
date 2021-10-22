// <copyright>
// Copyright 2020 by Kingdom First Solutions
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
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using EventbriteDotNetFramework;
using EventbriteDotNetFramework.Entities;
using EventbriteDotNetFramework.Responses;
using Rock;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;
using rocks.kfs.Eventbrite.Utility.ExtensionMethods;

namespace rocks.kfs.Eventbrite
{
    public class Eventbrite
    {
        private static int recordTypePersonId = DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.PERSON_RECORD_TYPE_PERSON.AsGuid() ).Id;
        private static int recordStatusPendingId = DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.PERSON_RECORD_STATUS_PENDING.AsGuid() ).Id;
        private static int homePhoneValueId = DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.PERSON_PHONE_TYPE_HOME ).Id;
        private static int homeLocationValueId = DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.GROUP_LOCATION_TYPE_HOME ).Id;
        private static int mobilePhoneValueId = DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.PERSON_PHONE_TYPE_MOBILE ).Id;
        private static int workPhoneValueId = DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.PERSON_PHONE_TYPE_WORK ).Id;

        public static EBApi Api()
        {
            return new EBApi( Settings.GetAccessToken(), Settings.GetOrganizationId().ToLong( 0 ) );
        }

        public static EBApi Api( string oAuthToken )
        {
            return new EBApi( oAuthToken );
        }
        public static EBApi Api( string oAuthToken, long orgId )
        {
            return new EBApi( oAuthToken, orgId );
        }

        public static bool EBUsageCheck( int id )
        {
            var mockQuerystring = new NameValueCollection();
            mockQuerystring.Add( "GroupId", id.ToString() );
            return EBUsageCheck( mockQuerystring );
        }

        public static bool EBUsageCheck( NameValueCollection queryString )
        {
            var retVar = false;
            var group = GetGroupFromQS( queryString );
            retVar = GetGroupEBEventId( group ) != null;
            return retVar;
        }

        private static AttributeValueCache GetGroupEBEventId( Group group )
        {
            AttributeValueCache retVar = null;
            var ebFieldType = FieldTypeCache.Get( EBGuid.FieldType.EVENTBRITE_EVENT.AsGuid() );
            if ( ebFieldType != null )
            {
                group.LoadAttributes();
                var attribute = group.Attributes.Select( a => a.Value ).FirstOrDefault( a => a.FieldTypeId == ebFieldType.Id );
                if ( attribute != null )
                {
                    retVar = group.AttributeValues.Select( av => av.Value ).FirstOrDefault( av => av.AttributeId == attribute.Id && av.Value != "" );
                }
            }

            return retVar;
        }

        public static Group GetGroupByEBEventId( string ebEventId )
        {
            return GetGroupByEBEventId( ebEventId.AsLong() );
        }

        public static Group GetGroupByEBEventId( long ebEventId )
        {
            Group retVar = null;
            var ebFieldType = FieldTypeCache.Get( EBGuid.FieldType.EVENTBRITE_EVENT.AsGuid() );
            if ( ebFieldType != null )
            {
                using ( var rockContext = new RockContext() )
                {
                    var attributeVal = new AttributeValueService( rockContext ).Queryable().FirstOrDefault( av => av.Attribute.FieldTypeId == ebFieldType.Id && av.Value.Contains( ebEventId.ToString() ) );
                    if ( attributeVal != null )
                    {
                        retVar = new GroupService( rockContext ).Get( attributeVal.EntityId ?? 0 );
                    }
                }
            }

            return retVar;
        }
        public static Group GetGroupByEBEventId( long ebEventId, RockContext rockContext )
        {
            Group retVar = null;
            var ebFieldType = FieldTypeCache.Get( EBGuid.FieldType.EVENTBRITE_EVENT.AsGuid() );
            if ( ebFieldType != null )
            {
                var attributeVal = new AttributeValueService( rockContext ).Queryable().FirstOrDefault( av => av.Attribute.FieldTypeId == ebFieldType.Id && av.Value.Contains( ebEventId.ToString() ) );
                if ( attributeVal != null )
                {
                    retVar = new GroupService( rockContext ).Get( attributeVal.EntityId ?? 0 );
                }
            }

            return retVar;
        }

        public static bool EBoAuthCheck()
        {
            var retVar = false;
            var getUser = new EBApi( Settings.GetAccessToken() ).GetUser();
            if ( getUser != null && getUser.Id != 0 )
            {
                retVar = true;
            }
            return retVar;
        }

        private static Group GetGroup( int id )
        {
            var mockQuerystring = new NameValueCollection();
            mockQuerystring.Add( "GroupId", id.ToString() );
            return GetGroupFromQS( mockQuerystring );
        }

        private static Group GetGroupFromQS( NameValueCollection qs )
        {
            if ( qs["GroupId"] != null && int.Parse( qs["GroupId"] ) > 0 )
            {
                return new GroupService( new RockContext() ).Get( int.Parse( qs["GroupId"] ) );
            }
            else
            {
                return new Group();
            }
        }

        public static void SyncEvent(
            int groupid,
            bool updatePrimaryEmail = false,
            string userid = "Eventbrite",
            int recordStatusId = 5,
            int connectionStatusId = 66,
            bool EnableLogging = false,
            bool ThrottleSync = false )
        {
            //Setup
            var rockContext = new RockContext();

            if ( EnableLogging )
            {
                LogEvent( rockContext, "SyncEvent", string.Format( "GroupId:{0}", groupid ), "Started" );
            }

            var group = new GroupService( rockContext ).Get( groupid );
            var eb = new EBApi( Settings.GetAccessToken(), Settings.GetOrganizationId().ToLong( 0 ) );
            var groupEBEventIDAttr = GetGroupEBEventId( group );
            var groupEBEventAttrSplit = groupEBEventIDAttr.Value.SplitDelimitedValues( "^" );
            var evntid = long.Parse( groupEBEventIDAttr != null ? groupEBEventAttrSplit[0] : "0" );

            if ( ThrottleSync && groupEBEventAttrSplit.Length > 1 && groupEBEventAttrSplit[1].AsDateTime() > RockDateTime.Now.Date.AddMinutes( -30 ) )
            {
                return;
            }

            if ( EnableLogging )
            {
                LogEvent( rockContext, "SyncEvent", string.Format( "Group: {0}", group ), "Got Group and EBEventId from Group." );
            }

            var ebOrders = new List<Order>();
            var ebEvent = eb.GetEventById( evntid );
            var IsRSVPEvent = ebEvent.IsRSVPEvent( eb );
            var gmPersonAttributeKey = GetPersonAttributeKey( rockContext, group );

            if ( EnableLogging )
            {
                LogEvent( rockContext, "SyncEvent", string.Format( "eb.GetEventById({0})", evntid ), "eb.GetEventById and get Person Attribute Key" );
            }

            //Get Eventbrite Attendees
            var ebOrderGet = eb.GetExpandedOrdersById( evntid );
            ebOrders.AddRange( ebOrderGet.Orders );
            if ( ebOrderGet.Pagination.PageCount > 1 )
            {
                var looper = new EventOrders();
                for ( int i = 2; i <= ebOrderGet.Pagination.PageCount; i++ )
                {
                    looper = eb.GetExpandedOrdersById( evntid, i );
                    ebOrders.AddRange( looper.Orders );
                }
            }

            if ( EnableLogging )
            {
                LogEvent( rockContext, "SyncEvent", string.Format( "GetExpandedOrdersById:{0}", evntid ), string.Format( "Result count: {0}", ebOrders.Count ) );
            }

            var groupMemberService = new GroupMemberService( rockContext );
            var personAliasService = new PersonAliasService( rockContext );

            AttendanceOccurrence occ = GetOrAddOccurrence( rockContext, group, ebEvent );

            if ( EnableLogging )
            {
                LogEvent( rockContext, "SyncEvent", string.Format( "GroupId: {0} Evntid: {1}", groupid, evntid ), "Begin For each order in ebOrders" );
            }
            foreach ( var order in ebOrders )
            {
                foreach ( var attendee in order.Attendees )
                {
                    HttpContext.Current.Server.ScriptTimeout = HttpContext.Current.Server.ScriptTimeout + 2;
                    SyncAttendee( rockContext, attendee, order, group, groupMemberService, personAliasService, occ, evntid, IsRSVPEvent, gmPersonAttributeKey, updatePrimaryEmail, recordStatusId, connectionStatusId, EnableLogging );
                }
            }

            if ( EnableLogging )
            {
                LogEvent( rockContext, "SyncEvent", string.Format( "End Sync for Group: {0}", groupid ), "End Sync and Write SyncTime to Group" );
            }
            rockContext.SaveChanges();

            // Write the Sync Time
            group.SetAttributeValue( groupEBEventIDAttr.AttributeKey, string.Format( "{0}^{1}", groupEBEventIDAttr.Value.SplitDelimitedValues( "^" )[0], RockDateTime.Now.ToString( "g", CultureInfo.CreateSpecificCulture( "en-us" ) ) ) );
            group.SaveAttributeValue( groupEBEventIDAttr.AttributeKey, rockContext );
        }
        public static void SyncOrder( string apiUrl )
        {
            var eb = new EBApi( Settings.GetAccessToken(), Settings.GetOrganizationId().ToLong( 0 ) );
            var order = eb.GetOrder( apiUrl, "event,attendees" );
            var rockContext = new RockContext();
            var group = GetGroupByEBEventId( order.Event_Id, rockContext );
            var ebEvent = order.Event;

            if ( group != null && ebEvent != null )
            {
                var gmPersonAttributeKey = GetPersonAttributeKey( rockContext, group );

                var groupMemberService = new GroupMemberService( rockContext );
                var personAliasService = new PersonAliasService( rockContext );

                AttendanceOccurrence occ = GetOrAddOccurrence( rockContext, group, ebEvent );

                foreach ( var attendee in order.Attendees )
                {
                    SyncAttendee( rockContext, attendee, order, group, groupMemberService, personAliasService, occ, order.Event_Id, ebEvent.IsRSVPEvent( eb ), gmPersonAttributeKey, false );
                }
                rockContext.SaveChanges();
            }
            else
            {
                HttpContext httpContext = HttpContext.Current;
                var logException = new Exception( string.Format( "Eventbrite SyncOrder, Group not found for Event: {0}", order.Event_Id ), new Exception( string.Format( "Sync api request: {0}", apiUrl ) ) );
                ExceptionLogService.LogException( logException, httpContext );
            }
        }

        public static void SyncAttendee( string apiUrl )
        {
            var eb = new EBApi( Settings.GetAccessToken(), Settings.GetOrganizationId().ToLong( 0 ) );
            var attendee = eb.GetAttendee( apiUrl, "event,order" );
            var order = attendee.Order;
            var rockContext = new RockContext();
            var group = GetGroupByEBEventId( attendee.Event_Id, rockContext );
            var ebEvent = attendee.Event;

            if ( group != null && ebEvent != null )
            {
                var gmPersonAttributeKey = GetPersonAttributeKey( rockContext, group );

                var groupMemberService = new GroupMemberService( rockContext );
                var personAliasService = new PersonAliasService( rockContext );

                AttendanceOccurrence occ = GetOrAddOccurrence( rockContext, group, ebEvent );

                SyncAttendee( rockContext, attendee, order, group, groupMemberService, personAliasService, occ, attendee.Event_Id, ebEvent.IsRSVPEvent( eb ), gmPersonAttributeKey, false );
                rockContext.SaveChanges();
            }
            else
            {
                HttpContext httpContext = HttpContext.Current;
                var logException = new Exception( string.Format( "Eventbrite SyncAttendee, Group not found for Event: {0}", attendee.Event_Id ), new Exception( string.Format( "Sync api request: {0}", apiUrl ) ) );
                ExceptionLogService.LogException( logException, httpContext );
            }

        }

        private static void SyncAttendee( RockContext rockContext, Attendee attendee, Order order, Group group, GroupMemberService groupMemberService, PersonAliasService personAliasService, AttendanceOccurrence occ, long evntid, bool IsRSVPEvent, string gmPersonAttributeKey, bool updatePrimaryEmail, int recordStatusId = 5, int connectionStatusId = 66, bool EnableLogging = false )
        {
            if ( string.IsNullOrWhiteSpace( attendee.Profile.Email ) )
            {
                attendee.Profile.Email = order.Email;
            }

            if ( !string.IsNullOrWhiteSpace( attendee.Profile.First_Name ) && !string.IsNullOrWhiteSpace( attendee.Profile.Last_Name ) && !string.IsNullOrWhiteSpace( attendee.Profile.Email ) )
            {
                var person = MatchOrAddPerson( rockContext, attendee.Profile, connectionStatusId, updatePrimaryEmail, EnableLogging );
                var matchedGroupMember = MatchGroupMember( group.Members, order, person.Id, gmPersonAttributeKey, rockContext, EnableLogging );
                if ( matchedGroupMember == null && ( order.Status != "deleted" || order.Status != "abandoned" ) )
                {
                    var member = new GroupMember
                    {
                        GroupId = group.Id,
                        Person = person,
                        GuestCount = ( order.Attendees != null ) ? order.Attendees.Count : 1,
                        GroupRoleId = group.GroupType.DefaultGroupRoleId.Value,
                        GroupMemberStatus = GroupMemberStatus.Active
                    };

                    groupMemberService.Add( member );

                    SetPersonData( rockContext, person, attendee, group, IsRSVPEvent && order.Attendees != null ? order.Attendees.Count : attendee.Quantity, EnableLogging );
                }
                else if ( matchedGroupMember != null && order.Status == "deleted" )
                {
                    matchedGroupMember.GroupMemberStatus = GroupMemberStatus.Inactive;
                    matchedGroupMember.Note = "Eventbrite order deleted!";

                    groupMemberService.Delete( matchedGroupMember );
                }
                else if ( matchedGroupMember != null && order.Status == "refunded" )
                {
                    matchedGroupMember.GroupMemberStatus = GroupMemberStatus.Inactive;
                    matchedGroupMember.Note = "Eventbrite order canceled/refunded!";
                }
                else if ( matchedGroupMember != null )
                {
                    if ( matchedGroupMember.Attributes == null )
                    {
                        matchedGroupMember.LoadAttributes( rockContext );
                    }
                    var attributeVal = matchedGroupMember.GetAttributeValue( gmPersonAttributeKey );
                    if ( attributeVal.IsNotNullOrWhiteSpace() )
                    {
                        var splitVal = attributeVal.SplitDelimitedValues( "^" );
                        var ticketClasses = splitVal[1].SplitDelimitedValues( "||" );
                        var ticketQtys = splitVal[3].SplitDelimitedValues( "||" );
                        if ( !ticketClasses.Contains( attendee.Ticket_Class_Name ) )
                        {
                            splitVal[1] += "||" + attendee.Ticket_Class_Name;
                            splitVal[3] += "||" + order.Attendees?.Count( a => a.Profile.First_Name == attendee.Profile.First_Name && a.Profile.Last_Name == attendee.Profile.Last_Name && a.Profile.Email == attendee.Profile.Email && a.Ticket_Class_Id == attendee.Ticket_Class_Id ).ToString();
                        }
                        else
                        {
                            ticketQtys[Array.IndexOf( ticketClasses, attendee.Ticket_Class_Name )] = order.Attendees?.Count( a => a.Profile.First_Name == attendee.Profile.First_Name && a.Profile.Last_Name == attendee.Profile.Last_Name && a.Profile.Email == attendee.Profile.Email && a.Ticket_Class_Id == attendee.Ticket_Class_Id ).ToString();
                            splitVal[3] = ticketQtys.JoinStrings( "||" );
                        }
                        matchedGroupMember.SetAttributeValue( gmPersonAttributeKey, splitVal.JoinStrings( "^" ) );
                        matchedGroupMember.SaveAttributeValue( gmPersonAttributeKey );
                    }
                }

                //Record Attendance
                if ( attendee.Checked_In )
                {
                    if ( EnableLogging )
                    {
                        LogEvent( rockContext, "SyncAttendee", string.Format( "Attendee.Checked_in", evntid ), "True" );
                    }

                    var attendance = occ.Attendees
                        .Where( a => a.PersonAlias != null && a.PersonAlias.PersonId == person.Id )
                        .FirstOrDefault();

                    if ( attendance == null )
                    {
                        int? personAliasId = personAliasService.GetPrimaryAliasId( person.Id );
                        if ( personAliasId.HasValue )
                        {
                            attendance = new Attendance
                            {
                                PersonAliasId = personAliasId,
                                StartDateTime = occ.Schedule != null && occ.Schedule.HasSchedule() ? occ.OccurrenceDate.Date.Add( occ.Schedule.StartTimeOfDay ) : occ.OccurrenceDate,
                                DidAttend = true
                            };

                            occ.Attendees.Add( attendance );
                        }
                    }
                    else
                    {
                        // Otherwise, only record that they attended -- don't change their attendance startDateTime 
                        attendance.DidAttend = true;
                    }
                    rockContext.SaveChanges();
                }
            }
        }

        private static Person MatchOrAddPerson( RockContext rockContext, Profile profile, int connectionStatusId, bool updatePrimaryEmail, bool EnableLogging = false )
        {
            var logString = string.Format( "Match or Add Person Profile: {0} {1} ({2})", profile.First_Name, profile.Last_Name, profile.Email );
            if ( EnableLogging )
            {
                LogEvent( rockContext, "MatchOrAddPerson", logString, "Start" );
            }

            var person = new PersonService( rockContext ).FindPerson( profile.First_Name, profile.Last_Name, profile.Email, updatePrimaryEmail );
            if ( person == null )
            {
                var email = ( profile.Email.Length <= 75 && System.Text.RegularExpressions.Regex.IsMatch( profile.Email, @"^[\w\.\'_%-]+(\+[\w-]*)?@([\w-]+\.)+[\w-]+" ) ) ? profile.Email : "";
                if ( profile.First_Name.Length <= 50 && profile.Last_Name.Length <= 50 )
                {
                    person = new Person
                    {
                        Guid = Guid.NewGuid(),
                        Gender = Gender.Unknown,
                        FirstName = profile.First_Name,
                        LastName = profile.Last_Name,
                        Email = email,
                        IsEmailActive = true,
                        EmailPreference = EmailPreference.EmailAllowed,
                        RecordStatusValueId = recordStatusPendingId,
                        RecordTypeValueId = recordTypePersonId,
                        ConnectionStatusValueId = connectionStatusId
                    };
                    PersonService.SaveNewPerson( person, rockContext );

                    if ( email.IsNullOrWhiteSpace() )
                    {
                        HttpContext httpContext = HttpContext.Current;
                        var e = new Exception( string.Format( "Person saved without Email address due to a validation error: {0} PersonId: {1}", profile.Email, person.Id ) );
                        ExceptionLogService.LogException( e, httpContext );
                    }
                }
                else
                {
                    var errorMsg = string.Format( "Error with profile information, unable to save. FirstName: {0} LastName: {1} Email: {0}", profile.First_Name, profile.Last_Name, profile.Email );
                    if ( EnableLogging )
                    {
                        LogEvent( rockContext, "MatchOrAddPerson", logString, errorMsg );
                    }
                    HttpContext httpContext = HttpContext.Current;
                    var e = new Exception( errorMsg );
                    ExceptionLogService.LogException( e, httpContext );
                }

            }

            if ( EnableLogging )
            {
                LogEvent( rockContext, "MatchOrAddPerson", logString, "End" );
            }
            return person;
        }

        private static ServiceLog LogEvent( RockContext rockContext, string type, string input, string result )
        {
            var rockLogger = new ServiceLogService( rockContext );
            ServiceLog serviceLog = new ServiceLog
            {
                Name = "Eventbrite",
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

        public static bool LinkEvents( Group group, long evnt )
        {
            var retVar = false;
            var ebFieldType = FieldTypeCache.Get( EBGuid.FieldType.EVENTBRITE_EVENT.AsGuid() );
            if ( ebFieldType != null )
            {
                group.LoadAttributes();
                var attribute = group.Attributes.Select( a => a.Value ).FirstOrDefault( a => a.FieldTypeId == ebFieldType.Id );
                if ( attribute != null )
                {
                    using ( var rockContext = new RockContext() )
                    {
                        group.SetAttributeValue( attribute.Key, evnt );
                        group.SaveAttributeValue( attribute.Key, rockContext );
                        retVar = true;
                    }
                }
            }
            return retVar;
        }

        public static bool LinkEvents( int groupId, long evnt )
        {
            var group = GetGroup( groupId );
            return LinkEvents( group, evnt );
        }

        public static void UnlinkEvents( int id )
        {
            var group = GetGroup( id );
            UnlinkEvents( group );
        }

        public static void UnlinkEvents( Group group )
        {
            var groupEBEventIDAttr = GetGroupEBEventId( group );

            using ( var rockContext = new RockContext() )
            {
                group.SetAttributeValue( groupEBEventIDAttr.AttributeKey, string.Empty );
                group.SaveAttributeValue( groupEBEventIDAttr.AttributeKey, rockContext );
            }
        }

        private static void SetPersonData( RockContext rockContext, Person person, Attendee attendee, Group group, int attendeeCount, bool EnableLogging = false )
        {
            if ( EnableLogging )
            {
                LogEvent( rockContext, "SetPersonData", string.Format( "Person: {0} Attendee: {1} Group: {2}", person, attendee.Id, group ), "Start" );
            }

            var familyGroup = person.GetFamily( rockContext );

            //Address
            if ( attendee.Profile.Addresses.Home != null && attendee.Profile.Addresses.Home.Address_1 != null && familyGroup != null && familyGroup.GroupLocations != null && !familyGroup.GroupLocations.Any( gl => gl.GroupLocationTypeValueId == homeLocationValueId ) )
            {
                var location = new LocationService( rockContext ).Get( attendee.Profile.Addresses.Home.Address_1, attendee.Profile.Addresses.Home.Address_2, attendee.Profile.Addresses.Home.City, attendee.Profile.Addresses.Home.Region, attendee.Profile.Addresses.Home.Postal_Code, attendee.Profile.Addresses.Home.Country );
                if ( familyGroup != null && location != null )
                {
                    familyGroup.GroupLocations.Add( new GroupLocation
                    {
                        GroupLocationTypeValueId = homeLocationValueId,
                        LocationId = location.Id,
                        IsMailingLocation = true,
                        IsMappedLocation = true
                    } );
                }
            }
            //Phone
            var homePhone = person.PhoneNumbers.FirstOrDefault( hp => hp.NumberTypeValueId.Equals( homePhoneValueId ) );
            var mobilePhone = person.PhoneNumbers.FirstOrDefault( mp => mp.NumberTypeValueId.Equals( mobilePhoneValueId ) );
            var workPhone = person.PhoneNumbers.FirstOrDefault( wp => wp.NumberTypeValueId.Equals( workPhoneValueId ) );
            if ( attendee.Profile.Home_Phone != null && homePhone == null )
            {
                person.PhoneNumbers.Add( new PhoneNumber
                {
                    Number = attendee.Profile.Home_Phone,
                    NumberTypeValueId = homePhoneValueId,
                    Guid = Guid.NewGuid(),
                    CreatedDateTime = attendee.Created,
                    ModifiedDateTime = attendee.Changed
                } );

            }
            if ( attendee.Profile.Cell_Phone != null && mobilePhone == null )
            {
                person.PhoneNumbers.Add( new PhoneNumber
                {
                    Number = attendee.Profile.Cell_Phone,
                    NumberTypeValueId = mobilePhoneValueId,
                    Guid = Guid.NewGuid(),
                    CreatedDateTime = attendee.Created,
                    ModifiedDateTime = attendee.Changed
                } );
            }
            if ( attendee.Profile.Work_Phone != null && workPhone == null )
            {
                person.PhoneNumbers.Add( new PhoneNumber
                {
                    Number = attendee.Profile.Work_Phone,
                    NumberTypeValueId = workPhoneValueId,
                    Guid = Guid.NewGuid(),
                    CreatedDateTime = attendee.Created,
                    ModifiedDateTime = attendee.Changed
                } );
            }
            //Gender
            if ( person.Gender == Gender.Unknown && attendee.Profile.Gender != null )
            {
                switch ( attendee.Profile.Gender.ToLower() )
                {
                    case "male":
                        person.Gender = Gender.Male;
                        break;
                    case "female":
                        person.Gender = Gender.Female;
                        break;
                    default:
                        person.Gender = Gender.Unknown;
                        break;
                }
            }
            //Birthdate
            if ( ( person.BirthDate == null || person.BirthDate == DateTime.MinValue || person.BirthDate == new DateTime( 1900, 1, 1 ) ) && ( attendee.Profile.Birth_Date != null && attendee.Profile.Birth_Date != DateTime.MinValue ) )
            {
                person.SetBirthDate( attendee.Profile.Birth_Date );
            }
            if ( EnableLogging )
            {
                LogEvent( rockContext, "SetPersonData", string.Format( "Person: {0} Attendee: {1} Group: {2}", person, attendee.Id, group ), "Set person info complete. Start Attribute check/update." );
            }
            rockContext.SaveChanges();

            // Attribute Values in our special eventBritePerson field type are stored as a delimited string Eventbrite Id^Ticket Class(es)^Eventbrite Order Id^RSVP/Ticket Count(s)
            var ebPersonVal = string.Format( "{0}^{1}^{2}^{3}", attendee.Id, attendee.Ticket_Class_Name, attendee.Order_Id.ToString(), attendeeCount );

            var groupMember = group.Members.FirstOrDefault( gm => gm.PersonId == person.Id );
            if ( groupMember != null )
            {
                var attributeKey = "";

                var ebPersonFieldType = FieldTypeCache.Get( EBGuid.FieldType.EVENTBRITE_PERSON.AsGuid() );
                if ( ebPersonFieldType != null )
                {
                    if ( groupMember.Attributes == null )
                    {
                        groupMember.LoadAttributes( rockContext );
                    }
                    var attribute = groupMember.Attributes.Select( a => a.Value ).FirstOrDefault( a => a.FieldTypeId == ebPersonFieldType.Id );
                    if ( attribute != null )
                    {
                        attributeKey = attribute.Key;
                    }

                }

                groupMember.SetAttributeValue( attributeKey, ebPersonVal );
                groupMember.SaveAttributeValue( attributeKey, rockContext );

                if ( EnableLogging )
                {
                    LogEvent( rockContext, "SetPersonData", string.Format( "Person: {0} Attendee: {1} Group: {2}", person, attendee.Id, group ), "Updated person attribute." );
                }
            }
        }

        private static GroupMember MatchGroupMember( ICollection<GroupMember> groupMembers, Order order, int PersonId, string attributeKey, RockContext rockContext = null, bool EnableLogging = false )
        {
            if ( EnableLogging )
            {
                LogEvent( rockContext, "MatchGroupMember", string.Format( "GroupMembers: {0} Order: {1} PersonId: {2}", groupMembers.Count, order.Id, PersonId ), "Start Match" );
            }

            if ( rockContext == null )
            {
                rockContext = new RockContext();
            }
            GroupMember retVar = null;

            // Attribute Values in our special eventBritePerson field type are stored as a delimited string Eventbrite Id^Ticket Class(es)^Eventbrite Order Id^RSVP/Ticket Count(s)
            foreach ( var gm in groupMembers )
            {
                if ( gm.Attributes == null )
                {
                    gm.LoadAttributes( rockContext );
                }
                var attributeVal = gm.GetAttributeValue( attributeKey );
                if ( attributeVal.IsNotNullOrWhiteSpace() && attributeVal.SplitDelimitedValues( "^" )[2] == order.Id.ToString() && gm.Person != null && gm.PersonId == PersonId )
                {
                    retVar = gm;
                    break;
                }
            }

            if ( retVar == null )
            {
                retVar = groupMembers.FirstOrDefault( gm => gm.Person != null && gm.PersonId == PersonId );
            }

            if ( EnableLogging )
            {
                LogEvent( rockContext, "MatchGroupMember", string.Format( "GroupMembers: {0} Order: {1} PersonId: {2} Matched: {3}", groupMembers.Count, order.Id, PersonId, retVar ), "End Match" );
            }

            return retVar;
        }

        private static string GetPersonAttributeKey( RockContext rockContext, Group group )
        {
            var retVal = "";
            var ebPersonFieldType = FieldTypeCache.Get( EBGuid.FieldType.EVENTBRITE_PERSON.AsGuid() );
            if ( ebPersonFieldType != null )
            {
                var groupMember = group.Members.FirstOrDefault();
                if ( groupMember != null )
                {
                    groupMember.LoadAttributes( rockContext );
                    var attribute = groupMember.Attributes.Select( a => a.Value ).FirstOrDefault( a => a.FieldTypeId == ebPersonFieldType.Id );
                    if ( attribute != null )
                    {
                        retVal = attribute.Key;
                    }
                }
            }

            return retVal;
        }

        private static AttendanceOccurrence GetOrAddOccurrence( RockContext rockContext, Group group, Event ebEvent )
        {
            var occurrenceService = new AttendanceOccurrenceService( rockContext );

            // Get all the occurrences for this group for the selected dates, location and schedule
            var occurrences = occurrenceService.GetGroupOccurrences( group, ebEvent.Start.Local.Date, null, new List<int>(), new List<int>() ).ToList();

            var occ = occurrences.FirstOrDefault( o => o.OccurrenceDate.Date.Equals( ebEvent.Start.Local.Date ) );
            if ( occ == null )
            {
                occ = new AttendanceOccurrence
                {
                    GroupId = group.Id,
                    OccurrenceDate = ebEvent.Start.Local.Date
                };
                occurrenceService.Add( occ );
            }

            return occ;
        }
    }
}
