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
    [AccountField( "Default Account", "Select the default account transactions should post to if the Reach account does not exist", true, "", "", 5 )]
    [DefinedValueField( Rock.SystemGuid.DefinedType.FINANCIAL_SOURCE_TYPE, "Source Type", "Select the defined value that new transactions should be attributed with.", true, false, "74650f5b-3e18-43e8-88db-1598deb2ffa0", "", 6 )]
    [DefinedValueField( Rock.SystemGuid.DefinedType.PERSON_CONNECTION_STATUS, "Person Status", "Select the defined value that new people should be created with.", true, false, "", "", 7 )]
    [CustomRadioListField( "Mode", "Mode to use for transactions", "Live,Test", true, "Test", "", 7 )]
    public class Gateway : GatewayComponent
    {
        private readonly string DemoUrl = "demo.reachapp.co";
        private readonly string ApiVersion = "api/v1";

        private static int recordTypePersonId = DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.PERSON_RECORD_TYPE_PERSON.AsGuid() ).Id;
        private static int recordStatusPendingId = DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.PERSON_RECORD_STATUS_PENDING.AsGuid() ).Id;
        private static int homePhoneValueId = DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.PERSON_PHONE_TYPE_HOME ).Id;
        private static int homeLocationValueId = DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.GROUP_LOCATION_TYPE_HOME ).Id;
        private static int searchKeyValueId = DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.PERSON_SEARCH_KEYS_ALTERNATE_ID.AsGuid() ).Id;
        private static int contributionTypeId = DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.TRANSACTION_TYPE_CONTRIBUTION ).Id;

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
            var today = RockDateTime.Now;
            var lookupContext = new RockContext();
            var accountLookup = new FinancialAccountService( lookupContext );
            var transactionLookup = new FinancialTransactionService( lookupContext );
            var donationUrl = GetBaseUrl( gateway, "donations", out errorMessage );
            var supporterUrl = GetBaseUrl( gateway, "sponsorship_supporters", out errorMessage );
            var categoryUrl = GetBaseUrl( gateway, "donation_categories", out errorMessage );

            if ( donationUrl.IsNullOrWhiteSpace() || supporterUrl.IsNullOrWhiteSpace() )
            {
                // errorMessage already set
                return null;
            }

            var authenticator = GetAuthenticator( gateway, out errorMessage );
            if ( authenticator == null )
            {
                // errorMessage already set
                return null;
            }

            var reachAccountMaps = DefinedTypeCache.Get( GetAttributeValue( gateway, "AccountMap" ) ).DefinedValues;
            var connectionStatus = DefinedValueCache.Get( GetAttributeValue( gateway, "PersonStatus" ) );
            var reachSourceType = DefinedValueCache.Get( GetAttributeValue( gateway, "SourceType" ) );
            var defaultAccount = accountLookup.Get( GetAttributeValue( gateway, "DefaultAccount" ).AsGuid() );
            if ( connectionStatus == null || reachAccountMaps == null || reachSourceType == null || defaultAccount == null )
            {
                errorMessage = "The Reach Account Map, Person Status, Source Type, or Default Account is not configured correctly in gateway settings.";
                return null;
            }

            var currentPage = 1;
            var queryHasResults = true;
            var skippedTransactionCount = 0;
            var errorMessages = new List<string>();
            var newTransactions = new List<FinancialTransaction>();
            var categoryResult = Api.PostRequest( categoryUrl, authenticator, null, out errorMessage );
            var categories = JsonConvert.DeserializeObject<List<Reporting.Category>>( categoryResult.ToStringSafe() );
            if ( categories == null )
            {
                // errorMessage already set
                return null;
            }

            while ( queryHasResults )
            {
                var parameters = new Dictionary<string, string>
                {
                    { "from_date", startDate.ToString( "yyyy-MM-dd" ) },
                    { "to_date", endDate.ToString( "yyyy-MM-dd" ) },
                    { "per_page", "50" },
                    { "page", currentPage.ToString() }
                };

                // to_date doesn't have a timestamp, so it includes transactions posted after the cutoff
                var donationResult = Api.PostRequest( donationUrl, authenticator, parameters, out errorMessage );
                var donations = JsonConvert.DeserializeObject<List<Donation>>( donationResult.ToStringSafe() );
                if ( donations != null && donations.Any() && errorMessage.IsNullOrWhiteSpace() )
                {
                    // only process completed transactions with confirmation codes and within the date range
                    foreach ( var donation in donations.Where( d => d.updated_at >= startDate && d.updated_at < endDate && d.status.Equals( "complete" ) && d.confirmation.IsNotNullOrWhiteSpace() ) )
                    {
                        var transaction = transactionLookup.Queryable()
                            .FirstOrDefault( t => t.FinancialGatewayId.HasValue &&
                                t.FinancialGatewayId.Value == gateway.Id &&
                                t.TransactionCode == donation.confirmation );
                        if ( transaction == null )
                        {
                            // find or create this person asynchronously for performance
                            var personAlias = Api.FindPersonAsync( lookupContext, donation, connectionStatus.Id );

                            var reachAccountName = string.Empty;
                            var donationItem = donation.line_items.FirstOrDefault();
                            if ( donationItem != null && donationItem.referral_type.Equals( "DonationOption", StringComparison.InvariantCultureIgnoreCase ) )
                            {
                                // one-time gift, should match a known category
                                var category = categories.FirstOrDefault( c => c.id == donationItem.referral_id );
                                if ( category != null )
                                {
                                    reachAccountName = category.title;
                                }
                            }
                            else
                            {
                                // recurring gift, lookup the sponsor info
                                var referralId = donation.referral_id ?? donationItem.referral_id;
                                var supporterResults = Api.PostRequest( string.Format( "{0}/{1}", supporterUrl, referralId ), authenticator, null, out errorMessage );
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

                            int? rockAccountId = defaultAccount.Id;
                            var accountMapping = reachAccountMaps.FirstOrDefault( v => v.Value.Equals( reachAccountName, StringComparison.CurrentCultureIgnoreCase ) );
                            if ( accountMapping != null )
                            {
                                var accountGuid = accountMapping.GetAttributeValue( "RockAccount" ).AsGuidOrNull();
                                if ( accountGuid.HasValue )
                                {
                                    rockAccountId = accountLookup.Get( (Guid)accountGuid ).Id;
                                }
                            }

                            // verify person alias was found or created
                            personAlias.Wait();
                            if ( !personAlias.Result.HasValue )
                            {
                                var infoMessage = string.Format( "{0} Reach import skipped {1} {2}'s donation {3} for {4} because their record could not be found or created",
                                    endDate.ToString( "d" ), donation.first_name, donation.last_name, donation.confirmation, reachAccountName );
                                ExceptionLogService.LogException( new Exception( infoMessage ), null );
                                continue;
                            }

                            // create the transaction
                            var summary = string.Format( "Reach Donation for {0} from {1} using {2} on {3} ({4})",
                                reachAccountName, donation.name, donation.payment_method, donation.updated_at, donation.token );
                            transaction = new FinancialTransaction
                            {
                                TransactionDateTime = donation.updated_at,
                                ProcessedDateTime = donation.updated_at,
                                TransactionCode = donation.confirmation,
                                Summary = summary,
                                SourceTypeValueId = reachSourceType.Id,
                                TransactionTypeValueId = contributionTypeId,
                                Guid = Guid.NewGuid(),
                                CreatedDateTime = today,
                                ModifiedDateTime = today,
                                AuthorizedPersonAliasId = personAlias.Result.Value,
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
                        else if ( transaction != null )
                        {
                            skippedTransactionCount++;
                        }
                    }
                }
                else 
                {
                    queryHasResults = false;
                }

                currentPage++;
            }

            if ( skippedTransactionCount > 0 )
            {
                ExceptionLogService.LogException( new Exception( string.Format( "{0} Reach import skipped downloading {1} transactions because they already exist",
                    endDate.ToString( "d" ), skippedTransactionCount ) ), null );
            }

            if ( newTransactions.Any() )
            {
                using ( var rockContext = new RockContext() )
                {
                    // create batch and add transactions
                    var batchPrefix = GetAttributeValue( gateway, "BatchPrefix" );
                    var batchDate = newTransactions.GroupBy( t => t.TransactionDateTime.Value.Date ).OrderByDescending( t => t.Count() )
                        .Select( g => g.Key ).FirstOrDefault();
                    var batch = new FinancialBatchService( rockContext ).GetByNameAndDate( string.Format( "{0} {1}",
                        batchPrefix, batchDate.ToString( "d" ) ), endDate, gateway.GetBatchTimeOffset() );
                    batch.BatchStartDateTime = batchDate;
                    batch.BatchEndDateTime = endDate;
                    batch.Note = string.Format( "{0} transactions downloaded starting {1} to {2}", batchPrefix, startDate, endDate );
                    batch.ControlAmount += newTransactions.Select( t => t.TotalAmount ).Sum();

                    var currentChanges = 0;
                    foreach ( var transaction in newTransactions )
                    {
                        // save in large batches so it doesn't overload context
                        batch.Transactions.Add( transaction );
                        if ( currentChanges++ > 100 )
                        {
                            rockContext.SaveChanges( disablePrePostProcessing: true );
                            currentChanges = 0;
                        }
                    }

                    // by default Rock associates with the current person
                    rockContext.SaveChanges( disablePrePostProcessing: true );
                }
            }

            if ( errorMessages.Any() )
            {
                errorMessage = string.Join( "<br>", errorMessages );
            }

            return new List<Payment>();
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
