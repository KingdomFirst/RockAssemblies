// <copyright>
// Copyright 2019 by Kingdom First Solutions
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
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using RestSharp;
using RestSharp.Authenticators;

using Rock;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;

namespace rocks.kfs.Reach.Reporting
{
    public static class Api
    {
        private static int recordTypePersonId = DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.PERSON_RECORD_TYPE_PERSON.AsGuid() ).Id;
        private static int recordStatusPendingId = DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.PERSON_RECORD_STATUS_PENDING.AsGuid() ).Id;
        private static int homePhoneValueId = DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.PERSON_PHONE_TYPE_HOME ).Id;
        private static int homeLocationValueId = DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.GROUP_LOCATION_TYPE_HOME ).Id;
        private static int searchKeyValueId = DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.PERSON_SEARCH_KEYS_ALTERNATE_ID.AsGuid() ).Id;
        private static int contributionTypeId = DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.TRANSACTION_TYPE_CONTRIBUTION ).Id;

        #region Reach API

        /// <summary>
        /// Posts the request to the gateway endpoint.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="authenticator">The authenticator.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <returns></returns>
        /// <exception cref="Exception">
        /// </exception>
        /// <exception cref="System.Exception"></exception>
        public static object PostRequest( string url, HttpBasicAuthenticator authenticator, Dictionary<string, string> parameters, out string errorMessage )
        {
            errorMessage = string.Empty;
            var restClient = new RestClient( url )
            {
                Authenticator = authenticator
            };

            var restRequest = new RestRequest( Method.GET )
            {
                RequestFormat = DataFormat.Json
            };

            if ( parameters != null )
            {
                foreach ( var param in parameters )
                {
                    restRequest.AddParameter( param.Key, param.Value );
                }
            }

            try
            {
                var response = restClient.Execute( restRequest );
                if ( response != null && response.StatusCode == HttpStatusCode.OK )
                {
                    return response.Content;
                }
                else
                {
                    errorMessage = $"Error with API request: {response.ErrorMessage} {response.StatusDescription}";
                    return null;
                }
            }
            catch ( WebException webException )
            {
                errorMessage = GetResponseMessage( webException.Response.GetResponseStream() );
                return null;
            }
            catch ( Exception ex )
            {
                errorMessage = ex?.InnerException?.Message;
                return null;
            }
        }

        /// <summary>
        /// Gets the response message.
        /// </summary>
        /// <param name="responseStream">The response stream.</param>
        /// <returns></returns>
        private static string GetResponseMessage( Stream responseStream )
        {
            var receiveStream = responseStream;
            var encode = Encoding.GetEncoding( "utf-8" );
            var sb = new StringBuilder();
            using ( var readStream = new StreamReader( receiveStream, encode ) )
            {
                var read = new Char[8192];
                var count = 0;
                do
                {
                    count = readStream.Read( read, 0, 8192 );
                    var str = new string( read, 0, count );
                    sb.Append( str );
                }
                while ( count > 0 );
            }

            return sb.ToString();
        }

        #endregion

        #region Rock Helpers

        /// <summary>
        /// Finds or Creates the Rock person asynchronously.
        /// </summary>
        /// <param name="lookupContext">The lookup context.</param>
        /// <param name="donation">The donation.</param>
        /// <param name="connectionStatusId">The connection status identifier.</param>
        /// <param name="updatePrimaryEmail">Whether or not this method should update the primary email address on the person.</param>
        /// <returns></returns>
        public static Task<int?> FindPersonAsync( RockContext lookupContext, Donation donation, int connectionStatusId, bool updatePrimaryEmail )
        {
            // specifically using Task.Run instead of Task.Factory.StartNew( longRunning)
            // see https://blog.stephencleary.com/2013/08/startnew-is-dangerous.html
            return Task.Run( () =>
            {
                // look for a single existing person by person fields
                int? primaryAliasId = null;
                var reachSearchKey = string.Format( "{0}_{1}", donation.supporter_id, "reach" );
                var person = new PersonService( lookupContext ).FindPerson( donation.first_name, donation.last_name, donation.email, updatePrimaryEmail, true );
                if ( person == null )
                {
                    // check by the search key
                    var existingSearchKey = new PersonSearchKeyService( lookupContext ).Queryable().FirstOrDefault( k => k.SearchValue.Equals( reachSearchKey, StringComparison.InvariantCultureIgnoreCase ) );
                    if ( existingSearchKey != null )
                    {
                        primaryAliasId = existingSearchKey.PersonAlias.Person.PrimaryAliasId;
                    }
                    else
                    {
                        // create the person since they don't exist
                        using ( var rockContext = new RockContext() )
                        {
                            person = new Person
                            {
                                Guid = Guid.NewGuid(),
                                Gender = Gender.Unknown,
                                FirstName = donation.first_name,
                                LastName = donation.last_name,
                                Email = donation.email,
                                IsEmailActive = true,
                                EmailPreference = EmailPreference.EmailAllowed,
                                RecordStatusValueId = recordStatusPendingId,
                                RecordTypeValueId = recordTypePersonId,
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
                                var familyGroup = person.GetFamily( rockContext );
                                var countryValue = donation.country;
                                var countryDV = DefinedTypeCache.Get( Rock.SystemGuid.DefinedType.LOCATION_COUNTRIES.AsGuid() )
                                                .GetDefinedValueFromValue( countryValue );
                                if ( countryDV == null )
                                {
                                    var countriesDT = DefinedTypeCache.Get( Rock.SystemGuid.DefinedType.LOCATION_COUNTRIES.AsGuid() );
                                    countryDV = countriesDT.DefinedValues.FirstOrDefault( dv => dv.Description.Contains( countryValue ) );
                                    if ( countryDV != null && countryDV.Value.IsNotNullOrWhiteSpace() )
                                    {
                                        countryValue = countryDV.Value;
                                    }
                                }

                                var location = new LocationService( rockContext ).Get( donation.address1, donation.address2, donation.city, donation.state, donation.postal, countryValue );
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

                            primaryAliasId = person.PrimaryAliasId;
                        }
                    }
                }
                else
                {
                    primaryAliasId = person.PrimaryAliasId;
                }

                return primaryAliasId;
            } );
        }

        #endregion
    }
}
