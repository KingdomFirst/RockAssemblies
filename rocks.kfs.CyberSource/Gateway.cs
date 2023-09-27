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
using System.Web.UI;
using Newtonsoft.Json;
using RestSharp.Authenticators;

using Rock;
using Rock.Attribute;
using Rock.Data;
using Rock.Financial;
using Rock.Model;
using Rock.Web.Cache;
using rocks.kfs.CyberSource.Controls;

namespace rocks.kfs.CyberSource
{
    #region Assembly Attributes

    [Description( "CyberSource Gateway" )]
    [Export( typeof( GatewayComponent ) )]
    [ExportMetadata( "ComponentName", "CyberSource Gateway" )]

    #endregion

    #region Assembly Settings

    [TextField(
        "Organization Id",
        Key = AttributeKey.OrganizationId,
        Description = "Enter the Organization Id (or merchant id) provided in your CyberSource Account",
        IsRequired = true,
        Order = 1 )]

    [TextField(
        "API Key",
        Key = AttributeKey.APIKey,
        Description = "Enter the API key (or public key) provided in your CyberSource Account",
        IsPassword = true,
        IsRequired = true,
        Order = 2 )]

    [TextField(
        "API Secret",
        Key = AttributeKey.APISecret,
        Description = "Enter the API secret (or private key) provided in your CyberSource account",
        IsPassword = true,
        IsRequired = true,
        Order = 3 )]

    [TextField(
        "Batch Prefix",
        Key = AttributeKey.BatchPrefix,
        Description = "Enter a batch prefix to be used with downloading transactions. The date of the earliest transaction in the batch will be appended to the prefix.",
        IsRequired = true,
        DefaultValue = "CyberSource",
        Order = 4 )]

    [CustomRadioListField(
        "Mode",
        Key = AttributeKey.Mode,
        Description = "Mode to use for transactions",
        ListSource = "Live,Test",
        IsRequired = true,
        DefaultValue = "Test",
        Order = 5 )]

    [DecimalField(
        "Credit Card Fee Coverage Percentage (Future)",
        Key = AttributeKey.CreditCardFeeCoveragePercentage,
        Description = @"The credit card fee percentage that will be used to determine what to add to the person's donation, if they want to cover the fee.",
        IsRequired = false,
        DefaultValue = null,
        Order = 6 )]

    [CurrencyField(
        "ACH Transaction Fee Coverage Amount (Future)",
        Key = AttributeKey.ACHTransactionFeeCoverageAmount,
        Description = "The dollar amount to add to an ACH transaction, if they want to cover the fee.",
        IsRequired = false,
        DefaultValue = null,
        Order = 7 )]

    #endregion

    /// <summary>
    /// CyberSource Payment Gateway
    /// </summary>
    public class Gateway : GatewayComponent, IHostedGatewayComponent
    {
        private static readonly string DemoUrl = "apitest.cybersource.com";
        private static readonly string ProductionUrl = "api.cybersource.com";

        private static int recordTypePersonId = DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.PERSON_RECORD_TYPE_PERSON.AsGuid() ).Id;
        private static int recordStatusPendingId = DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.PERSON_RECORD_STATUS_PENDING.AsGuid() ).Id;
        private static int homePhoneValueId = DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.PERSON_PHONE_TYPE_HOME ).Id;
        private static int homeLocationValueId = DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.GROUP_LOCATION_TYPE_HOME ).Id;
        private static int searchKeyValueId = DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.PERSON_SEARCH_KEYS_ALTERNATE_ID.AsGuid() ).Id;
        private static int contributionTypeId = DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.TRANSACTION_TYPE_CONTRIBUTION ).Id;

        #region Attribute Keys

        /// <summary>;
        /// Keys to use for Component Attributes
        /// </summary>
        public static class AttributeKey
        {
            public const string OrganizationId = "OrganizationId";
            public const string APIKey = "APIKey";
            public const string APISecret = "APISecret";
            public const string BatchPrefix = "BatchPrefix";
            public const string Mode = "Mode";

            /// <summary>
            /// The credit card fee coverage percentage
            /// </summary>
            public const string CreditCardFeeCoveragePercentage = "CreditCardFeeCoveragePercentage";

            /// <summary>
            /// The ach transaction fee coverage amount
            /// </summary>
            public const string ACHTransactionFeeCoverageAmount = "ACHTransactionFeeCoverageAmount";
        }

        #endregion Attribute Keys

        /// <summary>
        /// Gets the gateway URL.
        /// </summary>
        /// <value>
        /// The gateway URL.
        /// </value>
        [System.Diagnostics.DebuggerStepThrough]
        public static string GetGatewayUrl( FinancialGateway financialGateway )
        {
            bool testMode = financialGateway.GetAttributeValue( AttributeKey.Mode ).Equals( "Test" );
            if ( testMode )
            {
                return DemoUrl;
            }
            else
            {
                return ProductionUrl;
            }
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
        /// Gets the URL that the Gateway Information UI will navigate to when they click the 'Configure' link
        /// </summary>
        /// <value>
        /// The configure URL.
        /// </value>
        public string ConfigureURL => "https://ubctest.cybersource.com/ebc2/";

        /// <summary>
        /// Gets the URL that the Gateway Information UI will navigate to when they click the 'Learn More' link
        /// </summary>
        /// <value>
        /// The learn more URL.
        /// </value>
        public string LearnMoreURL => "https://www.cybersource.com/";

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
            errorMessage = "This gateway does not support authorizing transactions. Transactions should be created through the CyberSource interface.";
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
            errorMessage = "This gateway does not support charging transactions. Transactions should be created through the CyberSource interface.";
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
            errorMessage = "This gateway does not support crediting new transactions. Transactions should be credited through the CyberSource interface.";
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
            errorMessage = "This gateway does not support adding scheduled transactions. Transactions should be created through the CyberSource interface.";
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
            errorMessage = "This gateway does not support reactivating scheduled transactions. Transactions should be updated through the CyberSource interface.";
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
            errorMessage = "This gateway does not support updating scheduled transactions. Transactions should be updated through the CyberSource interface.";
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
            errorMessage = "This gateway does not support canceling scheduled transactions. Transactions should be cancelled through the CyberSource interface.";
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
            errorMessage = "This gateway does not support scheduled transactions. Transactions should be managed through the CyberSource interface.";
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
            var errorMessages = new List<string>();

            if ( donationUrl.IsNullOrWhiteSpace() )
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

        /// <summary>
        /// Gets the hosted payment information control which will be used to collect CreditCard, ACH fields
        /// Note: A HostedPaymentInfoControl can optionally implement <seealso cref="T:Rock.Financial.IHostedGatewayPaymentControlTokenEvent" />
        /// </summary>
        /// <param name="financialGateway">The financial gateway.</param>
        /// <param name="controlId">The control identifier.</param>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        public Control GetHostedPaymentInfoControl( FinancialGateway financialGateway, string controlId, HostedPaymentInfoControlOptions options )
        {
            CyberSourceHostedPaymentControl hostedPaymentControl = new CyberSourceHostedPaymentControl { ID = controlId };
            hostedPaymentControl.CyberSourceGateway = financialGateway;

            return hostedPaymentControl;

        }

        public string GetHostPaymentInfoSubmitScript( FinancialGateway financialGateway, Control hostedPaymentInfoControl )
        {
            return $"submitCyberSourceMicroFormInfo();";
        }

        public void UpdatePaymentInfoFromPaymentControl( FinancialGateway financialGateway, Control hostedPaymentInfoControl, ReferencePaymentInfo referencePaymentInfo, out string errorMessage )
        {
            throw new NotImplementedException();
        }

        public string CreateCustomerAccount( FinancialGateway financialGateway, ReferencePaymentInfo paymentInfo, out string errorMessage )
        {
            throw new NotImplementedException();
        }

        public DateTime GetEarliestScheduledStartDate( FinancialGateway financialGateway )
        {
            return RockDateTime.Today.AddDays( 1 ).Date;
        }

        public HostedGatewayMode[] GetSupportedHostedGatewayModes( FinancialGateway financialGateway )
        {
            return new HostedGatewayMode[1] { HostedGatewayMode.Hosted };
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
            var baseUrl = GetGatewayUrl( gateway );
            baseUrl = string.Format( "https://{0}/{2}", baseUrl, resource );

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
            var apiKey = GetAttributeValue( gateway, AttributeKey.APIKey );
            var apiSecret = GetAttributeValue( gateway, AttributeKey.APISecret );
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
