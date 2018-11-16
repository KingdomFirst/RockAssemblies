using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using com.kfs.Reach.Reporting;
using Newtonsoft.Json;
using RestSharp.Authenticators;
using Rock;
using Rock.Attribute;
using Rock.Data;
using Rock.Financial;
using Rock.Model;
using Rock.Web.Cache;

namespace com.kfs.Reach
{
    /// <summary>
    /// Reach Payment Gateway
    /// </summary>
    [Description( "Reach Gateway" )]
    [Export( typeof( GatewayComponent ) )]
    [ExportMetadata( "ComponentName", "Reach Gateway" )]
    [TextField( "Reach Domain", "This is your organization's custom domain, typically [CHURCH].reachapp.co.", true, "", "", 0 )]
    [TextField( "API Key", "Enter the API key provided in your Reach Account", true, "", "", 1 )]
    [TextField( "API Secret", "Enter the API secret provided in your Reach account", true, "", "", 2 )]
    [TextField( "Batch Prefix", "Enter a batch prefix to be used with downloading transactions.  The date of the earliest transaction in the batch will be appended to the prefix.", true, "Reach", "", 3 )]
    [DefinedTypeField( "Account Map", "Select the defined type that specifies the FinancialAccount mappings.", true, "80cdad25-a120-4a30-bb6a-21f1ccdb9e65", "", 4 )]
    [DefinedValueField( Rock.SystemGuid.DefinedType.FINANCIAL_SOURCE_TYPE, "Source Type", "Select the defined value that new transactions should be attributed with.", true, false, "74650f5b-3e18-43e8-88db-1598deb2ffa0", "", 6 )]
    [CustomRadioListField( "Mode", "Mode to use for transactions", "Live,Test", true, "Test", "", 7 )]
    public class Gateway : GatewayComponent
    {
        private readonly string DemoUrl = "demo.reachapp.co";
        private readonly string ApiVersion = "api/v1";

        #region Gateway Component Implementation

        /// <summary>
        /// Gets the supported payment schedules.
        /// </summary>
        /// <value>
        /// The supported payment schedules.
        /// </value>
        public override List<DefinedValueCache> SupportedPaymentSchedules
        {
            get
            {
                return new List<DefinedValueCache>();
            }
        }

        /// <summary>
        /// Returns a boolean value indicating if 'Saved Account' functionality is supported for the given currency type.
        /// </summary>
        /// <param name="currencyType">Type of the currency.</param>
        /// <returns></returns>
        public override bool SupportsSavedAccount( DefinedValueCache currencyType )
        {
            return false;
        }

        /// <summary>
        /// Authorizes the specified payment info.
        /// </summary>
        /// <param name="financialGateway"></param>
        /// <param name="paymentInfo">The payment info.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <returns></returns>
        public override FinancialTransaction Authorize( FinancialGateway financialGateway, PaymentInfo paymentInfo, out string errorMessage )
        {
            errorMessage = "This gateway does not support authorizing transactions. Transactions should be created through the Reach interface.";
            return null;
        }

        /// <summary>
        /// Charges the specified payment info.
        /// </summary>
        /// <param name="financialGateway"></param>
        /// <param name="paymentInfo">The payment info.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <returns></returns>
        public override FinancialTransaction Charge( FinancialGateway financialGateway, PaymentInfo paymentInfo, out string errorMessage )
        {
            errorMessage = "This gateway does not support charging transactions. Transactions should be created through the Reach interface.";
            return null;
        }

        /// <summary>
        /// Credits (Refunds) the specified transaction.
        /// </summary>
        /// <param name="origTransaction">The original transaction.</param>
        /// <param name="amount">The amount.</param>
        /// <param name="comment">The comment.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <returns></returns>
        public override FinancialTransaction Credit( FinancialTransaction origTransaction, decimal amount, string comment, out string errorMessage )
        {
            errorMessage = "This gateway does not support crediting new transactions. Transactions should be credited through the Reach interface.";
            return null;
        }

        /// <summary>
        /// Adds the scheduled payment.
        /// </summary>
        /// <param name="financialGateway"></param>
        /// <param name="schedule">The schedule.</param>
        /// <param name="paymentInfo">The payment info.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <returns></returns>
        public override FinancialScheduledTransaction AddScheduledPayment( FinancialGateway financialGateway, PaymentSchedule schedule, PaymentInfo paymentInfo, out string errorMessage )
        {
            errorMessage = "This gateway does not support adding scheduled transactions. Transactions should be created through the Reach interface.";
            return null;
        }

        /// <summary>
        /// Reactivates the scheduled payment.
        /// </summary>
        /// <param name="transaction">The transaction.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <returns></returns>
        public override bool ReactivateScheduledPayment( FinancialScheduledTransaction transaction, out string errorMessage )
        {
            errorMessage = "This gateway does not support reactivating scheduled transactions. Transactions should be updated through the Reach interface.";
            return false;
        }

        /// <summary>
        /// Updates the scheduled payment.
        /// </summary>
        /// <param name="transaction">The transaction.</param>
        /// <param name="paymentInfo">The payment info.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <returns></returns>
        public override bool UpdateScheduledPayment( FinancialScheduledTransaction transaction, PaymentInfo paymentInfo, out string errorMessage )
        {
            errorMessage = "This gateway does not support updating scheduled transactions. Transactions should be updated through the Reach interface.";
            return false;
        }

        /// <summary>
        /// Cancels the scheduled payment.
        /// </summary>
        /// <param name="transaction">The transaction.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <returns></returns>
        public override bool CancelScheduledPayment( FinancialScheduledTransaction transaction, out string errorMessage )
        {
            errorMessage = "This gateway does not support cancelling scheduled transactions. Transactions should be cancelled through the Reach interface.";
            return false;
        }

        /// <summary>
        /// Gets the scheduled payment status.
        /// </summary>
        /// <param name="transaction">The transaction.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <returns></returns>
        public override bool GetScheduledPaymentStatus( FinancialScheduledTransaction transaction, out string errorMessage )
        {
            errorMessage = "This gateway does not support scheduled transactions. Transactions should be managed through the Reach interface.";
            return false;
        }

        /// <summary>
        /// Gets the payments that have been processed for any scheduled transactions
        /// </summary>
        /// <param name="gateway">The gateway.</param>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <returns></returns>
        public override List<Payment> GetPayments( FinancialGateway gateway, DateTime startDate, DateTime endDate, out string errorMessage )
        {
            var donationUrl = GetBaseUrl( gateway, "donations", out errorMessage );
            if ( donationUrl.IsNullOrWhiteSpace() )
            {
                return null;
            }

            var authenticator = GetAuthenticator( gateway, out errorMessage );
            if ( authenticator == null )
            {
                return null;
            }

            // get gateway values
            var recordTypePersonId = DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.PERSON_RECORD_TYPE_PERSON.AsGuid() ).Id;
            var recordStatusPendingId = DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.PERSON_RECORD_STATUS_PENDING.AsGuid() ).Id;
            var connectionStatusProspectId = DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.PERSON_CONNECTION_STATUS_WEB_PROSPECT ).Id;
            var homePhoneValueId = DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.PERSON_PHONE_TYPE_HOME ).Id;
            var homeLocationValueId = DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.GROUP_LOCATION_TYPE_HOME ).Id;
            var searchKeyValueId = DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.PERSON_SEARCH_KEYS_ALTERNATE_ID.AsGuid() ).Id;
            var contributionTypeId = DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.TRANSACTION_TYPE_CONTRIBUTION ).Id;
            var reachAccountMaps = DefinedTypeCache.Get( GetAttributeValue( gateway, "AccountMap" ) ).DefinedValues;
            var reachSourceType = DefinedValueCache.Get( GetAttributeValue( gateway, "SourceType" ) );
            if ( reachAccountMaps == null || reachSourceType == null )
            {
                errorMessage = "Reach Account Map or Source Type is not configured correctly in gateway settings.";
                return null;
            }

            var today = RockDateTime.Now;
            var lookupContext = new RockContext();
            var personLookup = new PersonService( lookupContext );
            var searchKeyLookup = new PersonSearchKeyService( lookupContext );
            var accountLookup = new FinancialAccountService( lookupContext );
            var transactionLookup = new FinancialTransactionService( lookupContext );

            var currentPage = 1;
            var queryHasResults = true;
            var infoMessages = new List<string>();
            var newTransactions = new List<FinancialTransaction>();

            while ( queryHasResults )
            {
                var parameters = new Dictionary<string, string>
                {
                    { "from_date", startDate.ToString( "yyyy-MM-dd" ) },
                    { "end_date", endDate.ToString( "yyyy-MM-dd" ) },
                    { "per_page", "100" },
                    { "page", currentPage.ToString() }
                };

                var donationResults = Api.PostRequest( donationUrl, authenticator, parameters, out errorMessage );
                if ( donationResults != null && errorMessage.IsNullOrWhiteSpace() )
                {
                    var donations = JsonConvert.DeserializeObject<List<Donation>>( donationResults.ToString() );
                    if ( donations == null )
                    {
                        errorMessage = "Unable to parse donation objects, the API object format may have changed.";
                        return null;
                    }

                    // process transactions
                    var supporterUrl = GetBaseUrl( gateway, "sponsorship_supporters", out errorMessage );
                    foreach ( var donation in donations )
                    {
                        // only sync completed transactions with confirmation codes
                        if ( donation.status.Equals( "complete" ) && donation.confirmation.IsNotNullOrWhiteSpace() && donation.supporter_id.HasValue )
                        {
                            // get a single existing person by person fields
                            var reachSearchKey = string.Format( "{0}_{1}", donation.supporter_id, "reach" );
                            var person = personLookup.FindPerson( donation.first_name, donation.last_name, donation.email, true, true );
                            if ( person == null )
                            {
                                // check by the search key
                                var existingSearchKey = searchKeyLookup.Queryable().FirstOrDefault( k => k.SearchValue.Equals( reachSearchKey, StringComparison.InvariantCultureIgnoreCase ) );
                                if ( existingSearchKey != null )
                                {
                                    person = person ?? existingSearchKey.PersonAlias.Person;
                                }
                            }

                            // create the person if they don't exist
                            if ( person == null )
                            {
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
                                        ConnectionStatusValueId = connectionStatusProspectId,
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
                                                LocationId = location.Id
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

                            // find financial account from the account map based on the sponsorship
                            var reachAccountName = string.Empty;
                            var supporterId = donation.referral_id ?? donation.line_items.Select( i => i.referral_id ).FirstOrDefault();
                            if ( donation.purpose.EndsWith( "Sponsorship" ) && supporterId.HasValue )
                            {
                                // get the supporter results to find the account name
                                var supporterResults = Api.PostRequest( string.Format( "{0}/{1}", supporterUrl, supporterId ), authenticator, null, out errorMessage );
                                var supporter = JsonConvert.DeserializeObject<Supporter>( supporterResults.ToStringSafe() );
                                if ( supporter != null )
                                {
                                    var place = supporter.sponsorship?.place?.title;
                                    var sponsorshipType = supporter.sponsorship?.sponsorship_type_title;
                                    var shareType = supporter.share_type_id;

                                    string shareTypeName;
                                    switch ( shareType )
                                    {
                                        case "668":
                                            shareTypeName = "Primary";
                                            break;
                                        case "669":
                                            shareTypeName = "Secondary";
                                            break;
                                        default:
                                            shareTypeName = string.Empty;
                                            break;
                                    }

                                    reachAccountName = string.Format( "{0} {1} {2}", place, sponsorshipType, shareTypeName ).Trim();
                                }
                            }
                            else
                            {
                                reachAccountName = donation.purpose.Trim();
                            }

                            int? rockAccountId = null;
                            var accountMapping = reachAccountMaps.FirstOrDefault( v => v.Value.Equals( reachAccountName, StringComparison.CurrentCultureIgnoreCase ) );
                            if ( accountMapping == null )
                            {
                                infoMessages.Add( string.Format( "Unable to find account \"{1}\" for donation {0}", donation.confirmation, reachAccountName ) );
                                continue;
                            }
                            else
                            {
                                var accountGuid = accountMapping.GetAttributeValue( "RockAccount" ).AsGuidOrNull();
                                if ( accountGuid.HasValue )
                                {
                                    rockAccountId = accountLookup.Get( (Guid)accountGuid ).Id;
                                }
                            }

                            // find/create financial transaction
                            var transaction = transactionLookup.Queryable( "TransactionDetails" )
                                .FirstOrDefault( t => t.FinancialGatewayId.HasValue &&
                                    t.FinancialGatewayId.Value == gateway.Id &&
                                    t.TransactionCode == donation.confirmation );
                            if ( transaction == null && rockAccountId.HasValue && donation.amount.HasValue )
                            {
                                var summary = string.Format( "Reach Donation for {0} from {1} using {2} on {3} ({4})", reachAccountName, donation.name, donation.payment_method, donation.date, donation.token );
                                transaction = new FinancialTransaction
                                {
                                    TransactionDateTime = donation.date,
                                    TransactionCode = donation.confirmation,
                                    Summary = summary,
                                    SourceTypeValueId = reachSourceType.Id,
                                    TransactionTypeValueId = contributionTypeId,
                                    Guid = Guid.NewGuid(),
                                    CreatedDateTime = today,
                                    ModifiedDateTime = today,
                                    AuthorizedPersonAliasId = person.PrimaryAliasId,
                                    FinancialGatewayId = gateway.Id,
                                    ForeignId = donation.id,
                                    FinancialPaymentDetail = new FinancialPaymentDetail(),
                                    TransactionDetails = new List<FinancialTransactionDetail>
                                {
                                    new FinancialTransactionDetail
                                    {
                                        AccountId = (int)rockAccountId,
                                        Amount = (decimal)donation.amount,
                                        Summary = summary,
                                        Guid = Guid.NewGuid(),
                                        CreatedDateTime = today,
                                        ModifiedDateTime = today
                                    }
                                }
                                };

                                newTransactions.Add( transaction );
                            }
                            else
                            {
                                infoMessages.Add( string.Format( "Skipped donation {0} for {1} because it already exists", donation.confirmation, reachAccountName ) );
                            }
                        }
                    }
                }
                else
                {
                    queryHasResults = false;
                }

                currentPage++;
            }

            if ( !newTransactions.Any() )
            {
                errorMessage = "The donations API query did not return any data.";
                return null;
            }

            // get current batch and add transactions to it
            using ( var rockContext = new RockContext() )
            {
                var batch = new FinancialBatchService( rockContext ).GetByNameAndDate( string.Format( "{0} {1}", GetAttributeValue( gateway, "BatchPrefix" ), today.ToString( "d" ) ), today, gateway.GetBatchTimeOffset() );
                batch.BatchStartDateTime = newTransactions.Min( t => t.TransactionDateTime );
                batch.BatchEndDateTime = newTransactions.Max( t => t.TransactionDateTime );
                batch.ControlAmount += newTransactions.Select( t => t.TotalAmount ).Sum();

                foreach ( var transaction in newTransactions )
                {
                    batch.Transactions.Add( transaction );
                }
                rockContext.SaveChanges();
            }

            // CallStack for processing transactions:
            // Job.GetScheduledPayments
            // Gateway.GetPayments (this)
            // FinancialScheduledTransactionService.ProcessPayments

            if ( infoMessages.Any() )
            {
                errorMessage = string.Join( "<br>", infoMessages );
            }

            return null;
        }

        /// <summary>
        /// Gets an optional reference identifier needed to process future transaction from saved account.
        /// </summary>
        /// <param name="transaction">The transaction.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <returns></returns>
        public override string GetReferenceNumber( FinancialTransaction transaction, out string errorMessage )
        {
            errorMessage = string.Empty;
            return string.Empty;
        }

        /// <summary>
        /// Gets an optional reference identifier needed to process future transaction from saved account.
        /// </summary>
        /// <param name="scheduledTransaction">The scheduled transaction.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <returns></returns>
        public override string GetReferenceNumber( FinancialScheduledTransaction scheduledTransaction, out string errorMessage )
        {
            errorMessage = string.Empty;
            return string.Empty;
        }

        #endregion

        #region Static Helpers

        /// <summary>
        /// Gets the financial gateway.
        /// </summary>
        /// <param name="transaction">The transaction.</param>
        /// <returns></returns>
        private static FinancialGateway GetFinancialGateway( FinancialTransaction transaction )
        {
            return transaction != null ? GetFinancialGateway( transaction.FinancialGateway, transaction.FinancialGatewayId ) : null;
        }

        /// <summary>
        /// Gets the financial gateway.
        /// </summary>
        /// <param name="scheduledTransaction">The scheduled transaction.</param>
        /// <returns></returns>
        private static FinancialGateway GetFinancialGateway( FinancialScheduledTransaction scheduledTransaction )
        {
            return scheduledTransaction != null ? GetFinancialGateway( scheduledTransaction.FinancialGateway, scheduledTransaction.FinancialGatewayId ) : null;
        }

        /// <summary>
        /// Gets the financial gateway.
        /// </summary>
        /// <param name="financialGateway">The financial gateway.</param>
        /// <param name="financialGatewayId">The financial gateway identifier.</param>
        /// <returns></returns>
        private static FinancialGateway GetFinancialGateway( FinancialGateway financialGateway, int? financialGatewayId )
        {
            if ( financialGateway != null )
            {
                if ( financialGateway.Attributes == null )
                {
                    financialGateway.LoadAttributes();
                }
                return financialGateway;
            }

            if ( financialGatewayId.HasValue )
            {
                using ( var rockContext = new RockContext() )
                {
                    var gateway = new FinancialGatewayService( rockContext ).Get( financialGatewayId.Value );
                    gateway.LoadAttributes( rockContext );
                    return gateway;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the base URL.
        /// </summary>
        /// <param name="gateway">The gateway.</param>
        /// <param name="resource">The resource.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <returns></returns>
        private string GetBaseUrl( FinancialGateway gateway, string resource, out string errorMessage )
        {
            errorMessage = string.Empty;
            var baseUrl = GetAttributeValue( gateway, "Mode" ) == "Live" ? GetAttributeValue( gateway, "ReachDomain" ) : DemoUrl;
            if ( baseUrl.IsNullOrWhiteSpace() || !baseUrl.EndsWith( "reachapp.co" ) )
            {
                errorMessage = "The Domain URL was not provided or is not valid";
                return null;
            }
            else
            {
                baseUrl = string.Format( "https://{0}/{1}/{2}", baseUrl, ApiVersion, resource );
            }

            return baseUrl;
        }

        /// <summary>
        /// Gets the authenticator.
        /// </summary>
        /// <param name="gateway">The gateway.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <returns></returns>
        private HttpBasicAuthenticator GetAuthenticator( FinancialGateway gateway, out string errorMessage )
        {
            errorMessage = string.Empty;
            var apiKey = GetAttributeValue( gateway, "APIKey" );
            var apiSecret = GetAttributeValue( gateway, "APISecret" );
            if ( apiKey.IsNullOrWhiteSpace() || apiSecret.IsNullOrWhiteSpace() )
            {
                errorMessage = "API Key or API Secret is not valid";
                return null;
            }

            return new HttpBasicAuthenticator( apiKey, apiSecret );
        }

        #endregion
    }
}
