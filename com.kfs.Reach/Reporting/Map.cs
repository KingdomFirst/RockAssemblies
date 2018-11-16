using System;
using System.Linq;
using Rock;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;

namespace com.kfs.Reach.Reporting
{
    /// <summary>
    /// Helper Class to
    /// </summary>
    public static class Map
    {
        private static int recordTypePersonId = DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.PERSON_RECORD_TYPE_PERSON.AsGuid() ).Id;
        private static int recordStatusPendingId = DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.PERSON_RECORD_STATUS_PENDING.AsGuid() ).Id;
        private static int homePhoneValueId = DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.PERSON_PHONE_TYPE_HOME ).Id;
        private static int homeLocationValueId = DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.GROUP_LOCATION_TYPE_HOME ).Id;
        private static int searchKeyValueId = DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.PERSON_SEARCH_KEYS_ALTERNATE_ID.AsGuid() ).Id;
        private static int contributionTypeId = DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.TRANSACTION_TYPE_CONTRIBUTION ).Id;

        public static int? FindPerson( RockContext lookupContext, Donation donation, int connectionStatusId )
        {
            // get a single existing person by person fields
            var reachSearchKey = string.Format( "{0}_{1}", donation.supporter_id, "reach" );
            var person = new PersonService( lookupContext ).FindPerson( donation.first_name, donation.last_name, donation.email, true, true );
            if ( person == null )
            {
                // check by the search key
                var existingSearchKey = new PersonSearchKeyService( lookupContext ).Queryable().FirstOrDefault( k => k.SearchValue.Equals( reachSearchKey, StringComparison.InvariantCultureIgnoreCase ) );
                if ( existingSearchKey != null )
                {
                    person = existingSearchKey.PersonAlias.Person;
                }
                else
                {
                    // create the person since they don't exist
                    using ( var rockContext = new RockContext() )
                    {
                        person = new Person
                        {
                            Guid = Guid.NewGuid(),
                            FirstName = donation.first_name.FixCase(),
                            LastName = donation.last_name.FixCase(),
                            Email = donation.email,
                            IsEmailActive = true,
                            EmailPreference = EmailPreference.EmailAllowed,
                            RecordStatusValueId = recordStatusPendingId,
                            RecordTypeValueId = recordTypePersonId,
                            Gender = Gender.Unknown,
                            ConnectionStatusValueId = connectionStatusId,
                            ForeignId = donation.supporter_id
                        };

                        // save so the person alias is attributed for the search key
                        PersonService.SaveNewPerson( person, rockContext );

                        // add the person phone number
                        if ( donation.phone.IsNotNullOrWhiteSpace() )
                        {
                            person.PhoneNumbers.Add( new PhoneNumber
                            {
                                Number = donation.phone,
                                NumberTypeValueId = homePhoneValueId,
                                Guid = Guid.NewGuid(),
                                CreatedDateTime = donation.date,
                                ModifiedDateTime = donation.updated_at
                            } );
                        }

                        // add the person address
                        if ( donation.address1.IsNotNullOrWhiteSpace() )
                        {
                            var familyGroup = person.GetFamily();
                            var location = new LocationService( rockContext ).Get( donation.address1, donation.address2, donation.city, donation.state, donation.postal, donation.country );
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

                        // add the search key
                        new PersonSearchKeyService( rockContext ).Add( new PersonSearchKey
                        {
                            SearchTypeValueId = searchKeyValueId,
                            SearchValue = reachSearchKey,
                            PersonAliasId = person.PrimaryAliasId
                        } );
                        rockContext.SaveChanges();
                    }
                }
            }
            return person.PrimaryAliasId;
        }
    }
}
