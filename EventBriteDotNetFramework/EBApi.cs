namespace EventBriteDotNetFramework
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Globalization;
    using System.Linq;
    using Core;
    using Entities;
    using Enums;
    using Event;
    using RestSharp;

    public class EBApi
    {
        private const string BaseUrl = "https://www.eventbriteapi.com/v3/";

        public static Guid KFSEBCustomFieldModule = new Guid( "61A4DB64-9642-45BB-B3EC-4B83FB7CE18C" );
        public static Guid KFSEBoAuthTokenCustomField = new Guid( "E92C6D9C-CC5C-412D-BB8B-12C33B78A314" );
        public static Guid KFSEBEventNameCustomField = new Guid( "B47A6592-935F-43F9-B311-133D704E94A7" );
        public static Guid KFSEBEventSyncTimeCustomField = new Guid( "7F95AD8D-A58C-4E8B-B7F6-291571BBD7F6" );
        public static Guid KFSEBPersonOrderIDField = new Guid( "2D365D1B-3F74-45E2-A03A-AC2C219B3F52" );
        public static Guid KFSEBPersonTicketClassField = new Guid( "3C79F12F-7314-4536-A46D-EBA250738833" );
        public static Guid KFSEBPersonId = new Guid( "E6F108EB-066B-4628-9F75-21830B8ECC90" );
        public static Guid KFSEBProfileMemberFieldModules = new Guid( "261FAF99-E93C-4FD2-9817-634073C64B5E" );
        private string _oauthtoken;
        private long _me;
        private User _user;

        //Initializer
        public EBApi( string oAuthToken )
        {
            _oauthtoken = oAuthToken;
            _user = GetUser();
            _me = _user.Id;
        }

        //Base Execute
        public T Execute<T>( RestRequest request ) where T : new()
        {
            var client = new RestClient
            {
                BaseUrl = new System.Uri( BaseUrl )
            };
            request.AddHeader( "Authorization", "Bearer " + _oauthtoken ); // used on every request
            var response = client.Execute<T>( request );

            if ( response.ErrorException != null )
            {
                const string message = "Error retrieving response.  Check inner details for more info.";
                var EventBriteException = new ApplicationException( message, response.ErrorException );
                throw EventBriteException;
            }
            return response.Data;
        }

        #region Pure Rest Calls

        public User GetUser()
        {
            var request = new RestRequest
            {
                Resource = "/users/me/",
                RootElement = "User"
            };

            return Execute<User>( request );
        }

        public OwnedEventsResponse GetOwnedEvents()
        {
            var request = new RestRequest
            {
                Resource = string.Format( "/users/{0}/owned_events/", _me ),
                RootElement = "OwnedEventsRespoonse"
            };

            return Execute<OwnedEventsResponse>( request );
        }

        public Event GetEventById( long id )
        {
            var request = new RestRequest
            {
                Resource = string.Format( "/events/{0}/", id.ToString() ),
                RootElement = "Event"
            };

            return Execute<Event>( request );
        }

        public EventOrders GetOrdersById( long id )
        {
            return GetOrdersById( id, 1 );
        }

        public EventOrders GetOrdersById( long id, int page )
        {
            var request = new RestRequest
            {
                Resource = string.Format( "events/{0}/orders/?page={1}", id.ToString(), page.ToString() ),
                RootElement = "EventOrders"
            };

            return Execute<EventOrders>( request );
        }

        public EventOrders GetExpandedOrdersById( long id )
        {
            return GetExpandedOrdersById( id, 1 );
        }

        public EventOrders GetExpandedOrdersById( long id, int page )
        {
            var request = new RestRequest
            {
                Resource = string.Format( "events/{0}/orders/?page={1}&expand=attendees", id.ToString(), page.ToString() ),
                RootElement = "EventOrders"
            };

            return Execute<EventOrders>( request );
        }

        public EventTicketClasses GetTicketsById( long id )
        {
            var request = new RestRequest
            {
                Resource = string.Format( "events/{0}/ticket_classes/", id.ToString() ),
                RootElement = "EventTicketClasses"
            };

            return Execute<EventTicketClasses>( request );
        }

        public EventCannedQuestions GetEventCannedQuestionsById( long id )
        {
            return GetEventCannedQuestionsbyId( id, 1 );
        }

        public EventCannedQuestions GetEventCannedQuestionsbyId( long id, int page )
        {
            var request = new RestRequest
            {
                Resource = string.Format( "events/{0}/canned_questions/?page={1}", id.ToString(), page.ToString() ),
                RootElement = "EventCannedQuestions"
            };

            return Execute<EventCannedQuestions>( request );
        }

        #endregion

        #region Mixed Methods

        public static bool EBUsageCheck( NameValueCollection queryString )
        {
            var retVar = false;
            var evnt = GetEventFromQS( queryString );
            retVar = evnt.CustomFieldModules.FirstOrDefault( fm => fm.Guid.Equals( KFSEBCustomFieldModule ) ) != null;
            return retVar;
        }

        public static bool EBLinkCheck( NameValueCollection queryString )
        {
            var retVar = false;
            var evnt = GetEventFromQS( queryString );
            retVar = evnt.ForiegnKey != "";
            return retVar;
        }

        public static bool EBLinkCheck( int ArenaEventId )
        {
            var crazyTalk = new NameValueCollection();
            crazyTalk.Add( "profile", ArenaEventId.ToString() );
            return EBLinkCheck( crazyTalk );
        }

        public static bool EBoAuthCheck( NameValueCollection queryString )
        {
            var retVar = false;
            retVar = GetEBoAuth( queryString ) != "";
            return retVar;
        }

        public static string GetEBoAuth( NameValueCollection queryString )
        {
            var retVar = "";
            var evnt = GetEventFromQS( queryString );
            var cf = evnt.CustomFieldValues.FirstOrDefault( fv => fv.Guid.Equals( KFSEBoAuthTokenCustomField ) );
            if ( cf != null )
            {
                retVar = cf.SelectedValue;
            }
            return retVar;
        }

        public static string GetEBoAuth( int profile_id )
        {
            var ThisIsKindOfStupid = new NameValueCollection();
            ThisIsKindOfStupid.Add( "profile", profile_id.ToString() );
            return GetEBoAuth( ThisIsKindOfStupid );
        }

        public static void SyncEvent(
            int profileid,
            int ProfileSource = 272,
            int ProfileStatus = 255,
            int organizationID = 1,
            int DQProfileSource = 272,
            int DQProfileStatus = 255,
            int DQProfileID = -1,
            string userid = "Eventbrite",
            RecordStatus RecordStatus = RecordStatus.Pending,
            int MemberStatus = 957 )
        {
            //Setup
            var profile = new Core.Profile( profileid );
            var eb = new EBApi( GetEBoAuth( profileid ) );
            var evntid = long.Parse( profile.ForiegnKey.Substring( 2 ) );
            var ebOrders = new List<Order>();
            var matcher = new KFSPersonMatch();
            var ebEvent = eb.GetEventById( evntid );

            /* Refactor 6/27 notes:
             * Eventbrite Event = Arena Event Tag
             * Eventbrite Order = Arena Registration
             * Eventbrite Attendee = Arena Registrant
             *
             * 1 Get Eventbrite event with all attendees
             * 2 Get Arena Event
             * 3 Get Arena Registrations
             * 4 If Eventbrite/Arena event is RSVP, ignore attendees and just record the orders=registrations with count
             * 4a Else for each eventbrite order find each attendee and make sure there is a registrant for each attendee
             * 4b Create or match person as necessary.
             * */

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
            var arRegistrations = ArenaGetter.GetEventbriteRegistrants( profileid );

            switch ( profile.ProfileType )
            {
                // Only syncing event profile types is currently supported.
                case ProfileType.Event:

                    var arenaEvent = new EventProfile( profile.ProfileID );
                    if ( arenaEvent.IsRSVPEvent )
                    {
                        foreach ( var order in ebOrders )
                        {
                            if ( !string.IsNullOrWhiteSpace( order.First_Name ) && !string.IsNullOrWhiteSpace( order.Last_Name ) && !string.IsNullOrWhiteSpace( order.Email ) )
                            {
                                var person = matcher.MatchOrAddPerson( order.First_Name,
                                            order.First_Name,
                                            order.Last_Name,
                                            null,
                                            order.Email,
                                            MemberStatus,
                                            RecordStatus,
                                            organizationID,
                                            DQProfileID,
                                            DQProfileSource,
                                            DQProfileStatus
                                            );
                                var matchedRegistration = MatchRegistration( arRegistrations, order, person.PersonID, profile.ProfileID );
                                if ( matchedRegistration == null || matchedRegistration.Registrant.ProfileID != profile.ProfileID )
                                {
                                    var reg = new Registration
                                    {
                                        ProfileId = profile.ProfileID,
                                        Owner = person,
                                        RSVPCount = order.Attendees.Count,
                                        RegistrantCount = -1
                                    };
                                    reg.Save( userid );
                                    var rgnt = new Registrant
                                    {
                                        PersonID = person.PersonID,
                                        ProfileID = profile.ProfileID,
                                        Registration = reg
                                    };
                                    rgnt.Save( userid );

                                    SetPersonData( person, order.Attendees[0], profile, organizationID );
                                }
                                //Record Attendance
                                if ( order.Attendees[0].Checked_In && profile.Occurrences.Count != 0 )
                                {
                                    var occ = profile.Occurrences.FirstOrDefault( o => o.StartTime.Date.Equals( ebEvent.Start.Local.Date ) );
                                    var occAtt = new OccurrenceAttendance( occ.OccurrenceID, person.PersonID )
                                    {
                                        Attended = true,
                                        CheckInTime = ebEvent.Start.Local
                                    };
                                    if ( occAtt.OccurrenceID == -1 )
                                    {
                                        occAtt.OccurrenceID = occ.OccurrenceID;
                                    }
                                    occAtt.Save( "Eventbrite" );
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach ( var order in ebOrders )
                        {
                            if ( !string.IsNullOrWhiteSpace( order.First_Name ) && !string.IsNullOrWhiteSpace( order.Last_Name ) && !string.IsNullOrWhiteSpace( order.Email ) )
                            {
                                var currentRegistration = new Registration();
                                var person = matcher.MatchOrAddPerson( order.First_Name,
                                            order.First_Name,
                                            order.Last_Name,
                                            null,
                                            order.Email,
                                            MemberStatus,
                                            RecordStatus,
                                            organizationID,
                                            DQProfileID,
                                            DQProfileSource,
                                            DQProfileStatus
                                            );
                                var matchedRegistration = MatchRegistration( arRegistrations, order, person.PersonID, profile.ProfileID );
                                if ( matchedRegistration == null || matchedRegistration.Registrant.ProfileID != profile.ProfileID && order.Attendees.Count > 0 )
                                {
                                    currentRegistration = new Registration
                                    {
                                        ProfileId = profile.ProfileID,
                                        Owner = person,
                                        RSVPCount = -1,
                                        RegistrantCount = order.Attendees.Count
                                    };
                                    currentRegistration.Save( userid );
                                }
                                else
                                {
                                    currentRegistration = matchedRegistration.Registrant.Registration;
                                }

                                foreach ( var attendee in order.Attendees )
                                {
                                    if ( string.IsNullOrWhiteSpace( attendee.Profile.Email ) )
                                    {
                                        attendee.Profile.Email = order.Email;
                                    }

                                    if ( !string.IsNullOrWhiteSpace( attendee.Profile.First_Name ) && !string.IsNullOrWhiteSpace( attendee.Profile.Last_Name ) && !string.IsNullOrWhiteSpace( attendee.Profile.Email ) )
                                    {
                                        var match = matcher.MatchOrAddPerson( attendee.Profile.First_Name,
                                        attendee.Profile.First_Name,
                                        attendee.Profile.Last_Name,
                                        null,
                                        attendee.Profile.Email,
                                        MemberStatus,
                                        RecordStatus,
                                        organizationID,
                                        DQProfileID,
                                        DQProfileSource,
                                        DQProfileStatus
                                        );

                                        var registrantTest = new Registrant( profile.ProfileID, match.PersonID );
                                        if ( registrantTest.ProfileID == -1 )
                                        {
                                            var rgnt = new Registrant
                                            {
                                                PersonID = match.PersonID,
                                                ProfileID = profile.ProfileID,
                                                Registration = currentRegistration,
                                                Status = new Lookup( attendee.ArenaStatus.First().Key ),
                                                StatusReason = attendee.ArenaStatus.First().Value,
                                                Notes = attendee.ArenaStatus.First().Value
                                            };
                                            rgnt.Save( userid );

                                            SetPersonData( match, attendee, profile, organizationID );
                                        }
                                        else
                                        {
                                            var registrant = new Arena.Event.Registrant( profile.ProfileID, match.PersonID );
                                            registrant.Status = new Lookup( attendee.ArenaStatus.First().Key );
                                            registrant.StatusReason = attendee.ArenaStatus.First().Value;
                                            registrant.Save( userid );
                                        }
                                    }
                                }
                            }
                        }
                    }

                    break;
            }

            // Write the Sync Time
            using ( var context = ContextHelper.GetArenaContext() )
            {
                var cf = context.core_custom_field.FirstOrDefault( ccf => ccf.guid.Equals( KFSEBEventSyncTimeCustomField ) );
                var syncTime = new ProfileCustomFieldValue( profile.ProfileID, cf.custom_field_id )
                {
                    SelectedValue = DateTime.Now.ToString( "g", CultureInfo.CreateSpecificCulture( "en-us" ) )
                };
                if ( syncTime.ProfileId == -1 )
                {
                    syncTime.ProfileId = profile.ProfileID;
                }
                syncTime.Save( "Eventbrite" );
            }
        }

        private static EventProfile GetEventFromQS( NameValueCollection qs )
        {
            if ( qs["profile"] != null && int.Parse( qs["profile"] ) > 0 )
            {
                return new EventProfile( int.Parse( qs["profile"] ) );
            }
            else
            {
                return new EventProfile();
            }
        }

        private static Core.Address GetArenaAddress( Entities.Address EBAddress )
        {
            var retVar = new Core.Address
            {
                StreetLine1 = EBAddress.Address_1 != null ? EBAddress.Address_1 : "",
                StreetLine2 = EBAddress.Address_2 != null ? EBAddress.Address_2 : "",
                City = EBAddress.City != null ? EBAddress.City : "",
                State = EBAddress.Region != null ? EBAddress.Region : "",
                PostalCode = EBAddress.Postal_Code != null ? EBAddress.Postal_Code : "",
                Country = EBAddress.Country != null ? EBAddress.Country : ""
            };
            retVar.Save( "Eventbrite", true, true );
            return retVar;
        }

        public static void LinkEvents( EventProfile profile, Event evnt )
        {
            LinkEvents( profile.ProfileID, evnt.Id );
        }

        public static void LinkEvents( int profile, long evnt )
        {
            using ( var context = ContextHelper.GetArenaContext() )
            {
                var cp = context.core_profile.FirstOrDefault( p => p.profile_id.Equals( profile ) );
                cp.foreign_key = "EB" + evnt.ToString();

                var fm = context.core_custom_field_module.FirstOrDefault( f => f.guid.Equals( KFSEBProfileMemberFieldModules ) );
                var cpfm = new ProfileMemberFieldModule( profile, fm.custom_field_module_id )
                {
                    Active = true,
                    ProfileId = profile,
                    CustomFieldModuleId = fm.custom_field_module_id
                };
                cpfm.Save( "Eventbrite" );
                context.SaveChanges();
            }
        }

        public static void UnlinkEvents( int v )
        {
            using ( var context = ContextHelper.GetArenaContext() )
            {
                var cp = context.core_profile.FirstOrDefault( p => p.profile_id.Equals( v ) );
                cp.foreign_key = "";
                var fm = context.core_custom_field_module.FirstOrDefault( f => f.guid.Equals( KFSEBProfileMemberFieldModules ) );
                var cpfm = new ProfileMemberFieldModule( v, fm.custom_field_module_id );
                cpfm.Active = false;
                cpfm.ProfileId = v;
                cpfm.Save( "Eventbrite" );
                context.SaveChanges();
            }
        }

        public static void UnlinkEvents( EventProfile ep )
        {
            UnlinkEvents( ep.ProfileID );
        }

        public OwnedEventsResponse GetOwnedEvents( bool RSVP = false )
        {
            var evnts = GetOwnedEvents();
            var retVar = new OwnedEventsResponse
            {
                Pagination = evnts.Pagination,
                Events = new List<Event>()
            };
            if ( evnts.Events != null )
            {
                foreach ( var evnt in evnts.Events )
                {
                    var cq = GetEventCannedQuestionsById( evnt.Id );
                    var test = cq.Questions.FirstOrDefault( q => q.Respondent.Equals( "attendee", StringComparison.CurrentCultureIgnoreCase ) );
                    if ( ( test == null && RSVP ) || ( test != null && !RSVP ) )
                    {
                        retVar.Events.Add( evnt );
                    }
                }
            }
            else
            {
                retVar.Events = evnts.Events;
            }

            return retVar;
        }

        private static void SetPersonData( Person person, Attendee attendee, Arena.Core.Profile profile, int organizationID )
        {
            //Address
            if ( attendee.Profile.Addresses.Home != null && attendee.Profile.Addresses.Home.Address_1 != null && person.Addresses.PrimaryAddress() == null )
            {
                var pa = new PersonAddress
                {
                    PersonID = person.PersonID,
                    Address = GetArenaAddress( attendee.Profile.Addresses.Home ),
                    AddressType = new Lookup( SystemLookup.AddressType_Home ), //Home/Main address type
                    Primary = true
                };
                pa.save();
                person.Addresses.Add( pa );
                person.Addresses.Save( person.PersonID, "Eventbrite" );
            }
            //Phone
            var homePhone = person.Phones.FirstOrDefault( hp => hp.PhoneType.Equals( new Lookup( SystemLookup.PhoneType_Home ) ) );
            var cellPhone = person.Phones.FirstOrDefault( cp => cp.PhoneType.Equals( new Lookup( SystemLookup.PhoneType_Cell ) ) );
            var workPhone = person.Phones.FirstOrDefault( wp => wp.PhoneType.Equals( new Lookup( SystemLookup.PhoneType_Business ) ) );
            if ( attendee.Profile.Home_Phone != null && homePhone == null )
            {
                var pp = new PersonPhone( person.PersonID, new Lookup( SystemLookup.PhoneType_Home ), attendee.Profile.Home_Phone );
                pp.Save( organizationID );
            }
            if ( attendee.Profile.Cell_Phone != null && cellPhone == null )
            {
                var pp = new PersonPhone( person.PersonID, new Lookup( SystemLookup.PhoneType_Cell ), attendee.Profile.Cell_Phone );
                pp.Save( organizationID );
            }
            if ( attendee.Profile.Work_Phone != null && workPhone == null )
            {
                var pp = new PersonPhone( person.PersonID, new Lookup( SystemLookup.PhoneType_Business ), attendee.Profile.Work_Phone );
                pp.Save( organizationID );
            }
            //Gender
            if ( ( person.Gender == Gender.Unknown || person.Gender == Gender.Undefined ) && attendee.Profile.Gender != null )
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
                person.Save( organizationID, "Eventbrite", true );
            }
            //Birthdate
            if ( ( person.BirthDate == null || person.BirthDate == DateTime.MinValue || person.BirthDate == new DateTime( 1900, 1, 1 ) ) && (attendee.Profile.Birth_Date != null || attendee.Profile.Birth_Date != DateTime.MinValue ) )
            {
                person.BirthDate = attendee.Profile.Birth_Date;
                person.Save( organizationID, "Eventbrite", true );
            }

            //Set Custom Field Value
            var ebidcfid = -1;
            var eboidcfid = -1;
            var ebtccfid = -1;
            using ( var context = ContextHelper.GetArenaContext() )
            {
                ebidcfid = context.core_custom_field.FirstOrDefault( cf => cf.guid.Equals( KFSEBPersonId ) ).custom_field_id;
                eboidcfid = context.core_custom_field.FirstOrDefault( cf => cf.guid.Equals( KFSEBPersonOrderIDField ) ).custom_field_id;
                ebtccfid = context.core_custom_field.FirstOrDefault( cf => cf.guid.Equals( KFSEBPersonTicketClassField ) ).custom_field_id;
            }
            var ebid = new ProfileMemberFieldValue( profile.ProfileID, person.PersonID, ebidcfid );
            if ( ebid.PersonId == -1 )
            {
                ebid.PersonId = person.PersonID;
                ebid.ProfileId = profile.ProfileID;
            }
            ebid.SelectedValue = attendee.Id;
            ebid.Save( "Eventbrite" );

            var eboid = new ProfileMemberFieldValue( profile.ProfileID, person.PersonID, eboidcfid );
            if ( eboid.PersonId == -1 )
            {
                eboid.PersonId = person.PersonID;
                eboid.ProfileId = profile.ProfileID;
            }
            eboid.SelectedValue = attendee.Order_Id.ToString();
            eboid.Save( "Eventbrite" );

            var ebtcid = new ProfileMemberFieldValue( profile.ProfileID, person.PersonID, ebtccfid );
            if ( ebtcid.PersonId == -1 )
            {
                ebtcid.PersonId = person.PersonID;
                ebtcid.ProfileId = profile.ProfileID;
            }
            ebtcid.SelectedValue = attendee.Ticket_Class_Name;
            ebtcid.Save( "Eventbrite" );
        }

        private static KFSEventbriteRegistrant MatchRegistration(List<KFSEventbriteRegistrant> arRegistrations, Order Order, int PersonID, int ProfileID)
        {
            var retVar = new KFSEventbriteRegistrant();

            retVar = arRegistrations.FirstOrDefault( ar => ar.MemberFieldValuesByGuid != null && ar.MemberFieldValuesByGuid[KFSEBPersonOrderIDField] == Order.Id.ToString() );

            if (retVar == null)
            {
                retVar = arRegistrations.FirstOrDefault( ar => ar.Registrant != null && ar.Registrant.ProfileID == ProfileID && ar.Registrant.PersonID == PersonID );
            }

            return retVar;
        }

        #endregion
    }
}
