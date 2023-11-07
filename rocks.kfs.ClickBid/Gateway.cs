// <copyright>
// Copyright 2023 by Kingdom First Solutions
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
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;

using Newtonsoft.Json;

using Rock;
using Rock.Attribute;
using Rock.Data;
using Rock.Financial;
using Rock.Model;
using Rock.Web.Cache;
using rocks.kfs.ClickBid.Reporting;

namespace rocks.kfs.ClickBid
{
    #region Assembly Attributes

    [Description( "ClickBid Gateway" )]
    [Export( typeof( GatewayComponent ) )]
    [ExportMetadata( "ComponentName", "ClickBid Gateway" )]

    #endregion

    #region Assembly Settings

    [TextField( "API Token",
        Description = "Enter the API token provided in your ClickBid Account",
        IsRequired = true,
        Order = 0,
        Key = AttributeKey.APIToken )]

    [TextField( "ClickBid Organization Id",
        Description = "Enter the Organization Id in your ClickBid Account",
        IsRequired = true,
        Order = 1,
        Key = AttributeKey.OrganizationId )]

    [TextField( "Batch Prefix",
        Description = "Enter a batch prefix to be used with downloading transactions. The date of the earliest transaction in the batch will be appended to the prefix.",
        IsRequired = true,
        Order = 3,
        DefaultValue = "ClickBid",
        Key = AttributeKey.BatchPrefix )]

    [DefinedTypeField( "Account Map",
        Description = "Select the defined type that specifies the EventId > FinancialAccount mappings.",
        IsRequired = true,
        DefaultValue = Guids.SystemGuid.ACCOUNT_MAP,
        Order = 4,
        Key = AttributeKey.AccountMap )]

    [AccountField( "Default Account",
        Description = "Select the default account transactions should post to if the ClickBid account does not exist",
        IsRequired = true,
        Order = 5,
        Key = AttributeKey.DefaultAccount )]

    [DefinedValueField( "Source Type",
        Description = "Select the defined value that new transactions should be attributed with.",
        DefinedTypeGuid = Rock.SystemGuid.DefinedType.FINANCIAL_SOURCE_TYPE,
        IsRequired = true,
        AllowMultiple = false,
        DefaultValue = "74650f5b-3e18-43e8-88db-1598deb2ffa0",
        Order = 6,
        Key = AttributeKey.SourceType )]

    [DefinedValueField( "Person Status",
        Description = "Select the defined value that new people should be created with.",
        DefinedTypeGuid = Rock.SystemGuid.DefinedType.PERSON_CONNECTION_STATUS,
        IsRequired = true,
        AllowMultiple = false,
        Order = 7,
        Key = AttributeKey.PersonStatus )]

    [BooleanField( "Update Primary Email",
        Description = "Should the ClickBid account update the primary Rock email on transaction download?",
        IsRequired = false,
        Order = 8,
        Key = AttributeKey.UpdatePrimaryEmail )]

    #endregion

    /// <summary>
    /// ClickBid Payment Gateway
    /// </summary>
    public class Gateway : GatewayComponent
    {
        private readonly string ApiUrl = "cbo.io/app/public";
        private readonly string ApiVersion = "api/v4";

        private static int contributionTypeId = DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.TRANSACTION_TYPE_CONTRIBUTION ).Id;

        /// <summary>
        /// Attribute Keys
        /// </summary>
        private static class AttributeKey
        {
            public const string APIToken = "APIToken";
            public const string OrganizationId = "OrganizationId";
            public const string BatchPrefix = "BatchPrefix";
            public const string AccountMap = "AccountMap";
            public const string DefaultAccount = "DefaultAccount";
            public const string SourceType = "SourceType";
            public const string PersonStatus = "PersonStatus";
            public const string UpdatePrimaryEmail = "UpdatePrimaryEmail";
        }

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
            errorMessage = "This gateway does not support authorizing transactions. Transactions should be created through the ClickBid interface.";
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
            errorMessage = "This gateway does not support charging transactions. Transactions should be created through the ClickBid interface.";
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
            errorMessage = "This gateway does not support crediting new transactions. Transactions should be credited through the ClickBid interface.";
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
            errorMessage = "This gateway does not support adding scheduled transactions. Transactions should be created through the ClickBid interface.";
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
            errorMessage = "This gateway does not support reactivating scheduled transactions. Transactions should be updated through the ClickBid interface.";
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
            errorMessage = "This gateway does not support updating scheduled transactions. Transactions should be updated through the ClickBid interface.";
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
            errorMessage = "This gateway does not support canceling scheduled transactions. Transactions should be cancelled through the ClickBid interface.";
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
            errorMessage = "This gateway does not support scheduled transactions. Transactions should be managed through the ClickBid interface.";
            return false;
        }

        /// <summary>
        /// Gets the payments that have been processed for any ClickBid Sales
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
            var eventsUrl = GetBaseUrl( gateway, "events", out errorMessage );
            var salesReportUrl = $"{eventsUrl}/##eventId##/sales-report";
            // Other endpoints for future expansion possibility
            //var categoryUrl = $"{eventsUrl}/##eventId##/categories";
            //var bidderUrl = $"{eventsUrl}/##eventId##/bidders";
            //var itemUrl = $"{eventsUrl}/##eventId##/items";

            if ( eventsUrl.IsNullOrWhiteSpace() || salesReportUrl.IsNullOrWhiteSpace() )
            {
                // errorMessage already set
                return null;
            }

            var token = GetToken( gateway, out errorMessage );
            if ( token == null )
            {
                // errorMessage already set
                return null;
            }

            var eventAccountMaps = DefinedTypeCache.Get( GetAttributeValue( gateway, AttributeKey.AccountMap ) ).DefinedValues;
            var connectionStatus = DefinedValueCache.Get( GetAttributeValue( gateway, AttributeKey.PersonStatus ) );
            var clickbidSourceType = DefinedValueCache.Get( GetAttributeValue( gateway, AttributeKey.SourceType ) );
            var defaultAccount = accountLookup.Get( GetAttributeValue( gateway, AttributeKey.DefaultAccount ).AsGuid() );
            var updatePrimaryEmail = GetAttributeValue( gateway, AttributeKey.UpdatePrimaryEmail ).AsBoolean();
            if ( connectionStatus == null || eventAccountMaps == null || clickbidSourceType == null || defaultAccount == null )
            {
                errorMessage = "The ClickBid Account Map, Person Status, Source Type, or Default Account is not configured correctly in gateway settings.";
                return null;
            }

            var skippedTransactionCount = 0;
            var errorMessages = new List<string>();
            var newTransactions = new List<FinancialTransaction>();

            foreach ( var eventValue in eventAccountMaps )
            {
                var currentPage = 1;
                var queryHasResults = true;
                var eventId = eventValue.Value;
                var eventAccounts = eventValue.GetAttributeValue( "Accounts" ).ToKeyValuePairList();
                var eventGroupMemberIds = eventValue.GetAttributeValue( "GroupMemberMap" ).ToKeyValuePairList();

                while ( queryHasResults )
                {
                    var parameters = new Dictionary<string, string>
                    {
                        { "checkout_time[gte]", startDate.ToString( "yyyy-MM-dd" ) },
                        { "checkout_time[lte]", endDate.ToString( "yyyy-MM-dd" ) },
                        { "per_page", "50" },
                        { "page", currentPage.ToString() }
                    };

                    var salesResult = Api.PostRequest( salesReportUrl.Replace( "##eventId##", eventId ), token, parameters, out errorMessage );
                    var salesResponse = JsonConvert.DeserializeObject<SalesResponse>( salesResult.ToStringSafe() );
                    if ( salesResponse != null && salesResponse.data.Any() && errorMessage.IsNullOrWhiteSpace() )
                    {
                        foreach ( var sale in salesResponse.data )
                        {
                            var generatedTransactionCode = $"bidder_{sale.bidder_number}_item_{sale.item_number}_amt_{sale.purchase_amount}".Left( 50 );
                            var transaction = transactionLookup.Queryable()
                                .FirstOrDefault( t => t.FinancialGatewayId.HasValue &&
                                    t.FinancialGatewayId.Value == gateway.Id &&
                                    t.TransactionCode == generatedTransactionCode );
                            if ( transaction == null )
                            {
                                // find or create this person asynchronously for performance
                                var personAlias = Api.FindPersonAsync( lookupContext, sale, connectionStatus.Id, updatePrimaryEmail );

                                int? rockAccountId = defaultAccount.Id;
                                GroupMember groupMember = null;
                                var accountMapping = eventAccounts.FirstOrDefault( kvp => kvp.Key.Equals( sale.item_type, StringComparison.CurrentCultureIgnoreCase ) );
                                if ( accountMapping.Key.IsNotNullOrWhiteSpace() )
                                {
                                    var accountId = accountMapping.Value;
                                    if ( accountId != null )
                                    {
                                        var accountIdInt = accountId.ToIntSafe( 0 );
                                        if ( accountIdInt != 0 )
                                        {
                                            rockAccountId = accountIdInt;
                                        }
                                    }
                                }

                                var groupMemberMapping = eventGroupMemberIds.FirstOrDefault( kvp => kvp.Key.Equals( sale.item_type, StringComparison.CurrentCultureIgnoreCase ) );
                                if ( groupMemberMapping.Key.IsNotNullOrWhiteSpace() )
                                {
                                    var groupMemberId = groupMemberMapping.Value;
                                    if ( groupMemberId != null )
                                    {
                                        var groupMemberIdInt = groupMemberId.ToIntSafe( 0 );
                                        if ( groupMemberIdInt != 0 )
                                        {
                                            groupMember = new GroupMemberService( lookupContext ).Get( groupMemberIdInt );
                                        }
                                    }
                                }

                                // verify person alias was found or created
                                personAlias.Wait();
                                if ( !personAlias.Result.HasValue )
                                {
                                    var infoMessage = string.Format( "{0} ClickBid import skipped {1} {2}'s donation {3} for {4} because their record could not be found or created",
                                        endDate.ToString( "d" ), sale.first_name, sale.last_name, generatedTransactionCode, sale.item_name );
                                    ExceptionLogService.LogException( new Exception( infoMessage ), null );
                                    continue;
                                }

                                var currencyTypes = DefinedTypeCache.Get( Rock.SystemGuid.DefinedType.FINANCIAL_CURRENCY_TYPE ).DefinedValues;
                                var selectedCurrencyType = currencyTypes.FirstOrDefault( dv => dv.Value.Equals( sale.pay_type, StringComparison.CurrentCultureIgnoreCase ) );
                                if ( selectedCurrencyType == null )
                                {
                                    switch ( sale.pay_type )
                                    {
                                        case "CREDIT":
                                            selectedCurrencyType = DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.CURRENCY_TYPE_CREDIT_CARD );
                                            break;
                                        default:
                                            selectedCurrencyType = DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.CURRENCY_TYPE_UNKNOWN );
                                            break;
                                    }
                                }
                                // create the transaction
                                var summary = string.Format( "ClickBid {0} for {1} from {2} {3} using {4} on {5}",
                                    sale.won_by, sale.item_name, sale.first_name, sale.last_name, sale.pay_type, sale.checkout_time_display );
                                transaction = new FinancialTransaction
                                {
                                    TransactionDateTime = sale.checkout_time,
                                    ProcessedDateTime = sale.checkout_time,
                                    TransactionCode = generatedTransactionCode,
                                    Summary = summary,
                                    SourceTypeValueId = clickbidSourceType.Id,
                                    TransactionTypeValueId = contributionTypeId,
                                    Guid = Guid.NewGuid(),
                                    CreatedDateTime = today,
                                    ModifiedDateTime = today,
                                    AuthorizedPersonAliasId = personAlias.Result.Value,
                                    FinancialGatewayId = gateway.Id,
                                    FinancialPaymentDetail = new FinancialPaymentDetail
                                    {
                                        CurrencyTypeValueId = selectedCurrencyType.Id
                                    },
                                    TransactionDetails = new List<FinancialTransactionDetail>
                                    {
                                        new FinancialTransactionDetail
                                        {
                                            AccountId = (int)rockAccountId,
                                            Amount = (decimal)sale.purchase_amount,
                                            Summary = summary,
                                            Guid = Guid.NewGuid(),
                                            CreatedDateTime = today,
                                            ModifiedDateTime = today,
                                            EntityTypeId = (groupMember != null) ? EntityTypeCache.GetId<GroupMember>() : null,
                                            EntityId = groupMember?.Id
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
            }

            if ( newTransactions.Any() )
            {
                using ( var rockContext = new RockContext() )
                {
                    // create batch and add transactions
                    var batchPrefix = GetAttributeValue( gateway, AttributeKey.BatchPrefix );
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

            if ( skippedTransactionCount > 0 )
            {
                var errorStr = string.Format( "{0} ClickBid import skipped downloading {1} transactions because they already exist. {2} transactions were downloaded.",
                    ( startDate.ToString( "d" ) != endDate.ToString( "d" ) ) ? startDate.ToString( "d" ) + " - " + endDate.ToString( "d" ) : startDate.ToString( "d" ), skippedTransactionCount, newTransactions.Count );
                ExceptionLogService.LogException( new Exception( errorStr ), null );
                errorMessages.Add( errorStr );
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
            var baseUrl = ApiUrl;
            baseUrl = string.Format( "https://{0}/{1}/{2}", baseUrl, ApiVersion, resource );

            return baseUrl;
        }

        /// <summary>
        /// Gets the Token.
        /// </summary>
        /// <param name="gateway">The gateway.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <returns></returns>
        private string GetToken( FinancialGateway gateway, out string errorMessage )
        {
            errorMessage = string.Empty;
            var apiToken = GetAttributeValue( gateway, AttributeKey.APIToken );
            if ( apiToken.IsNullOrWhiteSpace() )
            {
                errorMessage = "API Token is not valid";
                return null;
            }

            return apiToken;
        }

        #endregion
    }
}
