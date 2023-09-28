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

using CyberSource.Api;
using CyberSource.Model;
using CyberSourceSDK = CyberSource;

using rocks.kfs.CyberSource.Controls;
using System.Text;
using System.Xml.Linq;

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
        /// Charges the specified payment info using the DirectPost API
        /// </summary>
        /// <param name="financialGateway">The financial gateway.</param>
        /// <param name="paymentInfo">The payment info.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <returns></returns>
        public override FinancialTransaction Charge( FinancialGateway financialGateway, PaymentInfo paymentInfo, out string errorMessage )
        {
            errorMessage = string.Empty;
            var referencedPaymentInfo = paymentInfo as ReferencePaymentInfo;
            if ( referencedPaymentInfo == null )
            {
                throw new ReferencePaymentInfoRequired();
            }

            if ( financialGateway == null )
            {
                throw new NullFinancialGatewayException();
            }

            var customerId = referencedPaymentInfo.GatewayPersonIdentifier;
            var tokenizerToken = referencedPaymentInfo.ReferenceNumber;
            var amount = referencedPaymentInfo.Amount;

            StringBuilder stringBuilderDescription = new StringBuilder();
            if ( referencedPaymentInfo.Description.IsNotNullOrWhiteSpace() )
            {
                stringBuilderDescription.AppendLine( referencedPaymentInfo.Description );
            }

            if ( referencedPaymentInfo.Comment1.IsNotNullOrWhiteSpace() )
            {
                stringBuilderDescription.AppendLine( referencedPaymentInfo.Comment1 );
            }

            if ( referencedPaymentInfo.Comment2.IsNotNullOrWhiteSpace() )
            {
                stringBuilderDescription.AppendLine( referencedPaymentInfo.Comment2 );
            }

            var description = stringBuilderDescription.ToString().Truncate( 600 );

            Ptsv2paymentsClientReferenceInformation clientReferenceInformation = new Ptsv2paymentsClientReferenceInformation(
                //Code: clientReferenceInformationCode,
                Comments: description
           );

            string defaultCurrency = "USD";
            Ptsv2paymentsOrderInformationAmountDetails orderInformationAmountDetails = new Ptsv2paymentsOrderInformationAmountDetails(
                TotalAmount: amount.ToString( "0.00" ),
                Currency: DefinedValueCache.Get( referencedPaymentInfo.AmountCurrencyCodeValueId.ToIntSafe( -1 ) )?.Value ?? defaultCurrency
           );

            Ptsv2paymentsOrderInformationBillTo orderInformationBillTo = new Ptsv2paymentsOrderInformationBillTo(
                FirstName: paymentInfo.FirstName,
                LastName: paymentInfo.LastName.IsNullOrWhiteSpace() ? paymentInfo.BusinessName : paymentInfo.LastName,
                Address1: paymentInfo.Street1,
                Locality: paymentInfo.City,
                AdministrativeArea: paymentInfo.State,
                PostalCode: paymentInfo.PostalCode,
                Country: paymentInfo.Country,
                Email: paymentInfo.Email,
                PhoneNumber: paymentInfo.Phone
           );

            Ptsv2paymentsOrderInformation orderInformation = new Ptsv2paymentsOrderInformation(
                AmountDetails: orderInformationAmountDetails,
                BillTo: orderInformationBillTo
           );

            Ptsv2paymentsTokenInformation tokenInformation = new Ptsv2paymentsTokenInformation(
                TransientTokenJwt: tokenizerToken
           );

            if ( customerId.IsNotNullOrWhiteSpace() )
            {
                //queryParameters.Add( "customer_vault_id", customerId );
                //when customers are setup do something different here
            }
            else
            {
                // queryParameters.Add( "payment_token", tokenizerToken );
            }

            Ptsv2paymentsDeviceInformation deviceInformation = new Ptsv2paymentsDeviceInformation(
                IpAddress: paymentInfo.IPAddress
                );

            var requestObj = new CreatePaymentRequest(
                ClientReferenceInformation: clientReferenceInformation,
                OrderInformation: orderInformation,
                TokenInformation: tokenInformation,
                DeviceInformation: deviceInformation
           );

            PtsV2PaymentsPost201Response chargeResult = null;
            try
            {
                var configDictionary = new Configuration().GetConfiguration( financialGateway );
                var clientConfig = new CyberSourceSDK.Client.Configuration( merchConfigDictObj: configDictionary );

                var apiInstance = new PaymentsApi( clientConfig );
                chargeResult = apiInstance.CreatePayment( requestObj );
                //return result;
            }
            catch ( Exception e )
            {
                errorMessage += "Exception on calling the API : " + e.Message;
                //return null;
            }

            if ( chargeResult == null || chargeResult.ErrorInformation != null )
            {
                if ( chargeResult == null )
                {
                    return null;
                }

                errorMessage += string.Format( "{0} ({1})", chargeResult.ErrorInformation.Message, chargeResult.ErrorInformation.Reason );
                ExceptionLogService.LogException( $"Error processing CyberSource transaction. Result Code:  {chargeResult.ErrorInformation.Message} ({chargeResult.ErrorInformation.Reason})." );
            }

            var transaction = new FinancialTransaction();
            transaction.TransactionCode = chargeResult.ProcessorInformation.TransactionId;
            transaction.ForeignKey = chargeResult.BuyerInformation?.MerchantCustomerId;

            //Customer customerInfo = this.GetCustomerVaultQueryResponse( financialGateway, customerId )?.CustomerVault.Customer;
            TssV2TransactionsGet200Response transactionDetail = GetTransactionDetailResponse( financialGateway, chargeResult.Id );
            transaction.FinancialPaymentDetail = CreatePaymentPaymentDetail( transactionDetail );

            transaction.AdditionalLavaFields = GetAdditionalLavaFields( chargeResult );

            return transaction;
        }

        private TssV2TransactionsGet200Response GetTransactionDetailResponse( FinancialGateway financialGateway, string id )
        {
            try
            {
                var configDictionary = new Configuration().GetConfiguration( financialGateway );
                var clientConfig = new CyberSourceSDK.Client.Configuration( merchConfigDictObj: configDictionary );

                var apiInstance = new TransactionDetailsApi( clientConfig );
                var result = apiInstance.GetTransaction( id );
                return result;
            }
            catch ( Exception e )
            {
                var errorMessage = "Exception on calling the API : " + e.Message;
                ExceptionLogService.LogException( errorMessage );
                return null;
            }
        }

        /// <summary>
        /// Populates the payment information.
        /// </summary>
        /// <param name="transactionDetail">The customer information.</param>
        /// <returns></returns>
        private FinancialPaymentDetail CreatePaymentPaymentDetail( TssV2TransactionsGet200Response transactionDetail )
        {
            var financialPaymentDetail = new FinancialPaymentDetail();
            UpdateFinancialPaymentDetail( transactionDetail, financialPaymentDetail );
            return financialPaymentDetail;
        }

        /// <summary>
        /// Updates the financial payment detail fields from the information in transactionDetail
        /// </summary>
        /// <param name="transactionDetail">The customer information.</param>
        /// <param name="financialPaymentDetail">The financial payment detail.</param>
        private void UpdateFinancialPaymentDetail( TssV2TransactionsGet200Response transactionDetail, FinancialPaymentDetail financialPaymentDetail )
        {
            financialPaymentDetail.GatewayPersonIdentifier = transactionDetail.BuyerInformation.MerchantCustomerId;

            string paymentType = transactionDetail.PaymentInformation.PaymentType.Type;
            if ( paymentType == "credit card" )
            {
                // cc payment
                var curType = DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.CURRENCY_TYPE_CREDIT_CARD );
                financialPaymentDetail.NameOnCard = $"{transactionDetail.OrderInformation.BillTo.FirstName} {transactionDetail.OrderInformation.BillTo.LastName}";
                financialPaymentDetail.CurrencyTypeValueId = curType != null ? curType.Id : ( int? ) null;

                //// The gateway tells us what the CreditCardType is since it was selected using their hosted payment entry frame.
                //// So, first see if we can determine CreditCardTypeValueId using the CardType response from the gateway

                // See if we can figure it out from the CC Type (Amex, Visa, etc)
                var creditCardTypeValue = CreditCardPaymentInfo.GetCreditCardTypeFromName( transactionDetail.ProcessingInformation.PaymentSolution );
                if ( creditCardTypeValue == null )
                {
                    // GetCreditCardTypeFromName should have worked, but just in case, see if we can figure it out from the MaskedCard using RegEx
                    creditCardTypeValue = CreditCardPaymentInfo.GetCreditCardTypeFromCreditCardNumber( transactionDetail.PaymentInformation.Card.Prefix );
                }

                financialPaymentDetail.CreditCardTypeValueId = creditCardTypeValue?.Id;
                financialPaymentDetail.AccountNumberMasked = transactionDetail.PaymentInformation.Card.Suffix;

                financialPaymentDetail.ExpirationMonth = transactionDetail.PaymentInformation.Card.ExpirationMonth.AsIntegerOrNull();
                financialPaymentDetail.ExpirationYear = transactionDetail.PaymentInformation.Card.ExpirationYear.AsIntegerOrNull();
            }
            //else
            //{
            //    // ach payment
            //    var curType = DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.CURRENCY_TYPE_ACH );
            //    financialPaymentDetail.CurrencyTypeValueId = curType != null ? curType.Id : ( int? ) null;
            //    financialPaymentDetail.AccountNumberMasked = customerInfo.CheckAccount;
            //}
        }

        /// <summary>
        /// Gets the additional lava fields in the form of a flatted out dictionary where child properties are delimited with '_' (Billing.Street1 becomes Billing_Street1)
        /// </summary>
        /// <param name="xdocResult">The xdoc result.</param>
        /// <returns></returns>
        private static Dictionary<string, object> GetAdditionalLavaFields<T>( T obj )
        {
            var xdocResult = JsonConvert.DeserializeXNode( obj.ToJson(), "root" );
            var additionalLavaFields = new Dictionary<string, object>();
            foreach ( XElement element in xdocResult.Root.Elements() )
            {
                if ( element.HasElements )
                {
                    string prefix = element.Name.LocalName;
                    foreach ( XElement childElement in element.Elements() )
                    {
                        additionalLavaFields.AddOrIgnore( prefix + "_" + childElement.Name.LocalName, childElement.Value.Trim() );
                    }
                }
                else
                {
                    additionalLavaFields.AddOrIgnore( element.Name.LocalName, element.Value.Trim() );
                }
            }

            return additionalLavaFields;
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

        /// <summary>
        /// Populates the properties of the referencePaymentInfo from this gateway's <seealso cref="M:Rock.Financial.IHostedGatewayComponent.GetHostedPaymentInfoControl(Rock.Model.FinancialGateway,System.String)" >hostedPaymentInfoControl</seealso>
        /// This includes the ReferenceNumber, plus any other fields that the gateway wants to set
        /// </summary>
        /// <param name="financialGateway">The financial gateway.</param>
        /// <param name="hostedPaymentInfoControl">The hosted payment information control.</param>
        /// <param name="referencePaymentInfo">The reference payment information.</param>
        /// <param name="errorMessage">The error message.</param>
        public void UpdatePaymentInfoFromPaymentControl( FinancialGateway financialGateway, Control hostedPaymentInfoControl, ReferencePaymentInfo referencePaymentInfo, out string errorMessage )
        {
            var cyberSourcePaymentControl = hostedPaymentInfoControl as CyberSourceHostedPaymentControl;
            errorMessage = null;

            if ( cyberSourcePaymentControl.PaymentInfoToken.IsNullOrWhiteSpace() )
            {
                errorMessage = "null response from GetHostedPaymentInfoToken";
                referencePaymentInfo.ReferenceNumber = cyberSourcePaymentControl.PaymentInfoToken;
                // referencePaymentInfo.InitialCurrencyTypeValue = cyberSourcePaymentControl.CurrencyTypeValue;
            }
            else
            {
                referencePaymentInfo.ReferenceNumber = cyberSourcePaymentControl.PaymentInfoToken;
                //referencePaymentInfo.InitialCurrencyTypeValue = cyberSourcePaymentControl.CurrencyTypeValue;
            }
        }

        public string CreateCustomerAccount( FinancialGateway financialGateway, ReferencePaymentInfo paymentInfo, out string errorMessage )
        {
            errorMessage = "";
            return "12345";
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

        #region Exceptions

        /// <summary>
        ///
        /// </summary>
        /// <seealso cref="System.Exception" />
        public class ReferencePaymentInfoRequired : Exception
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ReferencePaymentInfoRequired"/> class.
            /// </summary>
            public ReferencePaymentInfoRequired()
                : base( "CyberSource gateway requires a token or customer reference" )
            {
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <seealso cref="System.ArgumentNullException" />
        public class NullFinancialGatewayException : ArgumentNullException
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="NullFinancialGatewayException"/> class.
            /// </summary>
            public NullFinancialGatewayException()
                : base( "Unable to determine financial gateway" )
            {
            }
        }

        #endregion
    }
}
