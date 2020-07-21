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
        public static Guid KFSEBCustomFieldModule = new Guid( "61A4DB64-9642-45BB-B3EC-4B83FB7CE18C" );
        public static Guid KFSEBoAuthTokenCustomField = new Guid( "E92C6D9C-CC5C-412D-BB8B-12C33B78A314" );
        public static Guid KFSEBEventNameCustomField = new Guid( "B47A6592-935F-43F9-B311-133D704E94A7" );
        public static Guid KFSEBEventSyncTimeCustomField = new Guid( "7F95AD8D-A58C-4E8B-B7F6-291571BBD7F6" );
        public static Guid KFSEBPersonOrderIDField = new Guid( "2D365D1B-3F74-45E2-A03A-AC2C219B3F52" );
        public static Guid KFSEBPersonTicketClassField = new Guid( "3C79F12F-7314-4536-A46D-EBA250738833" );
        public static Guid KFSEBPersonId = new Guid( "E6F108EB-066B-4628-9F75-21830B8ECC90" );
        public static Guid KFSEBProfileMemberFieldModules = new Guid( "261FAF99-E93C-4FD2-9817-634073C64B5E" );

        private static int recordTypePersonId = DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.PERSON_RECORD_TYPE_PERSON.AsGuid() ).Id;
        private static int recordStatusPendingId = DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.PERSON_RECORD_STATUS_PENDING.AsGuid() ).Id;
        private static int homePhoneValueId = DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.PERSON_PHONE_TYPE_HOME ).Id;
        private static int homeLocationValueId = DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.GROUP_LOCATION_TYPE_HOME ).Id;
        private static int mobilePhoneValueId = DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.PERSON_PHONE_TYPE_MOBILE ).Id;
        private static int workPhoneValueId = DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.PERSON_PHONE_TYPE_WORK ).Id;

        public static EBApi Api()
        {
            return new EBApi( Settings.GetAccessToken() );
        }

        public static EBApi Api( string oAuthToken )
        {
            return new EBApi( oAuthToken );
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
                    var attributeVal = new AttributeValueService( rockContext ).Queryable().FirstOrDefault( av => av.Attribute.FieldTypeId == ebFieldType.Id && av.Value.SplitDelimitedValues( "^" )[0].Equals( ebEventId.ToString() ) );
                    if ( attributeVal != null )
                    {
                        retVar = new GroupService( rockContext ).Get( attributeVal.Guid );
                    }
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
            int connectionStatusId = 66 )
        {
            //Setup
            var rockContext = new RockContext();
            var group = new GroupService( rockContext ).Get( groupid );
            var eb = new EBApi( Settings.GetAccessToken() );
            var groupEBEventIDAttr = GetGroupEBEventId( group );
            var evntid = long.Parse( groupEBEventIDAttr != null ? groupEBEventIDAttr.Value.SplitDelimitedValues( "^" )[0] : "0" );
            //var connectionStatus = DefinedValueCache.Get( GetAttributeValue( gateway, "PersonStatus" ) );
            var ebOrders = new List<Order>();
            var ebEvent = eb.GetEventById( evntid );

            ///* Refactor 6/27 notes:
            // * Eventbrite Event = Arena Event Tag
            // * Eventbrite Order = Arena Registration
            // * Eventbrite Attendee = Arena Registrant
            // *
            // * 1 Get Eventbrite event with all attendees
            // * 2 Get Arena Event
            // * 3 Get Arena Registrations
            // * 4 If Eventbrite/Arena event is RSVP, ignore attendees and just record the orders=registrations with count
            // * 4a Else for each eventbrite order find each attendee and make sure there is a registrant for each attendee
            // * 4b Create or match person as necessary.
            // * */

            //Get Event Brite Attendees
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

            //Get Arena Attendees
            var ArenaGetter = new KFSRegistrationCollection();
            var arRegistrations = ArenaGetter.GetEventbriteRegistrants( groupid );

            var groupMemberService = new GroupMemberService( rockContext );
            var occurrenceService = new AttendanceOccurrenceService( rockContext );
            var attendanceService = new AttendanceService( rockContext );
            var personAliasService = new PersonAliasService( rockContext );

            //if ( group.GroupType.EnableRSVP )
            //{
            foreach ( var order in ebOrders )
            {
                foreach ( var attendee in order.Attendees )
                {
                    if ( string.IsNullOrWhiteSpace( attendee.Profile.Email ) )
                    {
                        attendee.Profile.Email = order.Email;
                    }

                    if ( !string.IsNullOrWhiteSpace( attendee.Profile.First_Name ) && !string.IsNullOrWhiteSpace( attendee.Profile.Last_Name ) && !string.IsNullOrWhiteSpace( attendee.Profile.Email ) )
                    {
                        var person = MatchOrAddPerson( rockContext, attendee.Profile, connectionStatusId, updatePrimaryEmail );
                        var matchedRegistration = MatchRegistration( group.Members, order, person.Id, group );
                        if ( matchedRegistration == null && ( order.Status != "deleted" || order.Status != "abandoned" ) )
                        {
                            var member = new GroupMember
                            {
                                GroupId = group.Id,
                                Person = person,
                                GuestCount = order.Attendees.Count,
                                GroupRoleId = group.GroupType.DefaultGroupRoleId.Value,
                                GroupMemberStatus = GroupMemberStatus.Active
                            };

                            groupMemberService.Add( member );

                            SetPersonData( rockContext, person, attendee, group );
                        }
                        else if ( matchedRegistration != null && order.Status == "deleted" )
                        {
                            matchedRegistration.GroupMemberStatus = GroupMemberStatus.Inactive;
                            matchedRegistration.Note = "Eventbrite order deleted!";

                            //groupMemberService.Delete( matchedRegistration );
                        }
                        // Get all the occurrences for this group for the selected dates, location and schedule
                        var occurrences = new AttendanceOccurrenceService( rockContext )
                        .GetGroupOccurrences( group, ebEvent.Start.Local.Date, ebEvent.End.Local.Date, new List<int>(), new List<int>() )
                        .ToList();

                        //Record Attendance
                        if ( attendee.Checked_In )
                        {
                            var occ = occurrences.FirstOrDefault( o => o.OccurrenceDate.Date.Equals( ebEvent.Start.Local.Date ) );
                            if ( occ == null )
                            {
                                occ = new AttendanceOccurrence();
                                occ.GroupId = group.Id;
                                occ.OccurrenceDate = ebEvent.Start.Local.Date;
                                occurrenceService.Add( occ );
                            }

                            var existingAttendees = occ.Attendees.ToList();

                            var attendance = existingAttendees
                                .Where( a => a.PersonAlias.PersonId == person.Id )
                                .FirstOrDefault();

                            if ( attendance == null )
                            {
                                int? personAliasId = personAliasService.GetPrimaryAliasId( person.Id );
                                if ( personAliasId.HasValue )
                                {
                                    attendance = new Attendance();
                                    attendance.PersonAliasId = personAliasId;
                                    attendance.StartDateTime = occ.Schedule != null && occ.Schedule.HasSchedule() ? occ.OccurrenceDate.Date.Add( occ.Schedule.StartTimeOfDay ) : occ.OccurrenceDate;
                                    attendance.DidAttend = true;

                                    occ.Attendees.Add( attendance );
                                }
                            }
                            else
                            {
                                // Otherwise, only record that they attended -- don't change their attendance startDateTime 
                                attendance.DidAttend = true;
                            }

                        }
                    }
                }
            }

            rockContext.SaveChanges();
            //}
            //else
            //{
            //    foreach ( var order in ebOrders )
            //    {
            //        if ( !string.IsNullOrWhiteSpace( order.First_Name ) && !string.IsNullOrWhiteSpace( order.Last_Name ) && !string.IsNullOrWhiteSpace( order.Email ) )
            //        {
            //            var currentRegistration = new Registration();
            //            var person = MatchOrAddPerson( rockContext, order, connectionStatusId, updatePrimaryEmail );
            //            var matchedRegistration = MatchRegistration( arRegistrations, order, person.PersonID, profile.ProfileID );
            //            if ( matchedRegistration == null || matchedRegistration.Registrant.ProfileID != profile.ProfileID && order.Attendees.Count > 0 )
            //            {
            //                currentRegistration = new Registration
            //                {
            //                    ProfileId = profile.ProfileID,
            //                    Owner = person,
            //                    RSVPCount = -1,
            //                    RegistrantCount = order.Attendees.Count
            //                };
            //                currentRegistration.Save( userid );
            //            }
            //            else
            //            {
            //                currentRegistration = matchedRegistration.Registrant.Registration;
            //            }

            //            foreach ( var attendee in order.Attendees )
            //            {
            //                if ( string.IsNullOrWhiteSpace( attendee.Profile.Email ) )
            //                {
            //                    attendee.Profile.Email = order.Email;
            //                }

            //                if ( !string.IsNullOrWhiteSpace( attendee.Profile.First_Name ) && !string.IsNullOrWhiteSpace( attendee.Profile.Last_Name ) && !string.IsNullOrWhiteSpace( attendee.Profile.Email ) )
            //                {
            //                    var match = matcher.MatchOrAddPerson( attendee.Profile.First_Name,
            //                    attendee.Profile.First_Name,
            //                    attendee.Profile.Last_Name,
            //                    null,
            //                    attendee.Profile.Email,
            //                    MemberStatus,
            //                    RecordStatus,
            //                    organizationID,
            //                    DQProfileID,
            //                    DQProfileSource,
            //                    DQProfileStatus
            //                    );

            //                    var registrantTest = new Registrant( profile.ProfileID, match.PersonID );
            //                    if ( registrantTest.ProfileID == -1 )
            //                    {
            //                        var rgnt = new Registrant
            //                        {
            //                            PersonID = match.PersonID,
            //                            ProfileID = profile.ProfileID,
            //                            Registration = currentRegistration,
            //                            Status = new Lookup( attendee.ArenaStatus.First().Key ),
            //                            StatusReason = attendee.ArenaStatus.First().Value,
            //                            Notes = attendee.ArenaStatus.First().Value
            //                        };
            //                        rgnt.Save( userid );

            //                        SetPersonData( match, attendee, profile, organizationID );
            //                    }
            //                    else
            //                    {
            //                        var registrant = new Arena.Event.Registrant( profile.ProfileID, match.PersonID );
            //                        registrant.Status = new Lookup( attendee.ArenaStatus.First().Key );
            //                        registrant.StatusReason = attendee.ArenaStatus.First().Value;
            //                        registrant.Save( userid );
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}


            // Write the Sync Time
            group.SetAttributeValue( groupEBEventIDAttr.AttributeKey, string.Format( "{0}^{1}", groupEBEventIDAttr.Value.SplitDelimitedValues( "^" )[0], DateTime.Now.ToString( "g", CultureInfo.CreateSpecificCulture( "en-us" ) ) ) );
            group.SaveAttributeValue( groupEBEventIDAttr.AttributeKey, rockContext );
        }

        private static Person MatchOrAddPerson( RockContext rockContext, Profile profile, int connectionStatusId, bool updatePrimaryEmail )
        {
            var person = new PersonService( rockContext ).FindPerson( profile.First_Name, profile.Last_Name, profile.Email, updatePrimaryEmail, true );
            if ( person == null )
            {
                //// check by the search key
                //var existingSearchKey = new PersonSearchKeyService( rockContext ).Queryable().FirstOrDefault( k => k.SearchValue.Equals( searchKey, StringComparison.InvariantCultureIgnoreCase ) );
                //if ( existingSearchKey != null )
                //{
                //    person = existingSearchKey.PersonAlias.Person;
                //}
                //else
                //{
                // create the person since they don't exist
                using ( var newRockContext = new RockContext() )
                {
                    person = new Person
                    {
                        Guid = Guid.NewGuid(),
                        Gender = Gender.Unknown,
                        FirstName = profile.First_Name,
                        LastName = profile.Last_Name,
                        Email = profile.Email,
                        IsEmailActive = true,
                        EmailPreference = EmailPreference.EmailAllowed,
                        RecordStatusValueId = recordStatusPendingId,
                        RecordTypeValueId = recordTypePersonId,
                        ConnectionStatusValueId = connectionStatusId
                    };

                    // save so the person alias is attributed for the search key
                    PersonService.SaveNewPerson( person, newRockContext );

                    //// add the search key
                    //new PersonSearchKeyService( newRockContext ).Add( new PersonSearchKey
                    //{
                    //    SearchTypeValueId = searchKeyValueId,
                    //    SearchValue = reachSearchKey,
                    //    PersonAliasId = person.PrimaryAliasId
                    //} );
                    newRockContext.SaveChanges();

                }
                //} // If we ever enable searchKey capability
            }

            return person;
        }

        public static void LinkEvents( Group profile, Event evnt )
        {
            LinkEvents( profile.Id, evnt.Id );
        }

        public static void LinkEvents( int profile, long evnt )
        {
            //using ( var context = ContextHelper.GetArenaContext() )
            //{
            //    var cp = context.core_profile.FirstOrDefault( p => p.profile_id.Equals( profile ) );
            //    cp.foreign_key = "EB" + evnt.ToString();

            //    var fm = context.core_custom_field_module.FirstOrDefault( f => f.guid.Equals( KFSEBProfileMemberFieldModules ) );
            //    var cpfm = new ProfileMemberFieldModule( profile, fm.custom_field_module_id )
            //    {
            //        Active = true,
            //        ProfileId = profile,
            //        CustomFieldModuleId = fm.custom_field_module_id
            //    };
            //    cpfm.Save( "Eventbrite" );
            //    context.SaveChanges();
            //}
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

        private static void SetPersonData( RockContext rockContext, Person person, Attendee attendee, Group group )
        {
            //Address
            if ( attendee.Profile.Addresses.Home != null && attendee.Profile.Addresses.Home.Address_1 != null && !person.PrimaryFamily.GroupLocations.Any( gl => gl.GroupLocationTypeValueId == homeLocationValueId ) )
            {
                var familyGroup = person.GetFamily( rockContext );
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
            if ( ( person.BirthDate == null || person.BirthDate == DateTime.MinValue || person.BirthDate == new DateTime( 1900, 1, 1 ) ) && ( attendee.Profile.Birth_Date != null || attendee.Profile.Birth_Date != DateTime.MinValue ) )
            {
                person.SetBirthDate( attendee.Profile.Birth_Date );
            }
            rockContext.SaveChanges();

            // Attribute Values in our special eventBritePerson field type are stored as a delimitted string Eventbrite Id^Ticket Class^Eventbrite Order Id
            var ebPersonVal = string.Format( "{0}^{1}^{2}", attendee.Id, attendee.Ticket_Class_Name, attendee.Order_Id.ToString() );

            var groupMember = group.Members.FirstOrDefault( gm => gm.PersonId == person.Id );
            if ( groupMember != null )
            {
                var attributeKey = "";

                var ebPersonFieldType = FieldTypeCache.Get( EBGuid.FieldType.EVENTBRITE_PERSON.AsGuid() );
                if ( ebPersonFieldType != null )
                {
                    groupMember.LoadAttributes( rockContext );
                    var attribute = groupMember.Attributes.Select( a => a.Value ).FirstOrDefault( a => a.FieldTypeId == ebPersonFieldType.Id );
                    if ( attribute != null )
                    {
                        attributeKey = attribute.Key;
                    }

                }

                groupMember.SetAttributeValue( attributeKey, ebPersonVal );
                groupMember.SaveAttributeValue( attributeKey, rockContext );
            }

            //rockContext.SaveChanges();
        }

        private static GroupMember MatchRegistration( ICollection<GroupMember> groupMembers, Order order, int PersonId, Group group )
        {

            GroupMember retVar = null;
            var attributeKey = "";

            var ebPersonFieldType = FieldTypeCache.Get( EBGuid.FieldType.EVENTBRITE_PERSON.AsGuid() );
            if ( ebPersonFieldType != null )
            {
                var groupMember = group.Members.FirstOrDefault();
                if ( groupMember != null )
                {
                    groupMember.LoadAttributes();
                    var attribute = groupMember.Attributes.Select( a => a.Value ).FirstOrDefault( a => a.FieldTypeId == ebPersonFieldType.Id );
                    if ( attribute != null )
                    {
                        attributeKey = attribute.Key;
                        //retVar = group.AttributeValues.Select( av => av.Value ).FirstOrDefault( av => av.AttributeId == attribute.Id && av.Value != "" );
                    }
                }

            }

            // Attribute Values in our special eventBritePerson field type are stored as a delimitted string Eventbrite Id^Ticket Class^Eventbrite Order Id
            foreach ( var gm in groupMembers )
            {
                gm.LoadAttributes();
                var attributeVal = gm.GetAttributeValue( attributeKey );
                //if ( gm.AttributeValues != null && 
                //     gm.AttributeValues.Values != null && 
                //     gm.AttributeValues.Values.FirstOrDefault( av => av.AttributeId == attributeId 
                //                                                     && av.Value != null 
                //                                                     && av.Value != ""
                //                                                     && av.Value.SplitDelimitedValues( "^" )[2] == order.Id.ToString() // 2 = Eventbrite Order Id
                //     ) != null 
                //)
                if ( attributeVal.IsNotNullOrWhiteSpace() && attributeVal.SplitDelimitedValues( "^" )[2] == order.Id.ToString() )
                {
                    retVar = gm;
                    break;
                }
            }
            //retVar = arRegistrations.FirstOrDefault( ar => ar.MemberFieldValuesByGuid != null && ar.MemberFieldValuesByGuid[KFSEBPersonOrderIDField] == Order.Id.ToString() );

            if ( retVar == null )
            {
                retVar = groupMembers.FirstOrDefault( gm => gm.Person != null && gm.PersonId == PersonId );
                //retVar = arRegistrations.FirstOrDefault( ar => ar.Registrant != null && ar.Registrant.ProfileID == ProfileID && ar.Registrant.PersonID == PersonID );
            }

            return retVar;
        }
    }
}
