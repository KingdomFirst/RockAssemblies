﻿// <copyright>
// Copyright 2024 by Kingdom First Solutions
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
using System.Text;
using System.Threading;
using System.Web.UI;
using System.Xml.Linq;
using Newtonsoft.Json;

using Rock;
using Rock.Attribute;
using Rock.Data;
using Rock.Financial;
using Rock.Model;
using Rock.Web.Cache;

using CyberSource.Api;
using CyberSource.Model;
using rocks.kfs.CyberSource.Controls;
using static rocks.kfs.CyberSource.CyberSourceTypes;
using CyberSourceSDK = CyberSource;
using rocks.kfs.CyberSource.Model;

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
        Description = @"The credit card fee percentage that will be used to determine what to add to the person's donation when they choose to cover the fee.",
        IsRequired = false,
        DefaultValue = null,
        Order = 6 )]

    [BooleanField(
        "Capture Payment",
        Key = AttributeKey.CapturePayment,
        Description = "Indicates whether to also include a capture in the submitted authorization request or not. If not, transactions will have to be settled.",
        IsRequired = true,
        DefaultBooleanValue = false,
        Order = 7 )]

    [CustomDropdownListField(
        "Event Registration Address Mode",
        Key = AttributeKey.EventRegistrationAddressMode,
        Description = @"The mode for address input on the Gateway with event registrations. In order to use 'Hide' or 'Optional' you must have Relaxed AVS enabled by the processor.",
        ListSource = "Hide,Optional,Required",
        DefaultValue = "Required",
        Order = 8 )]

    [ValueListField(
        "Allowed Card Networks",
        customValues: "VISA,MASTERCARD,AMEX,CARNET,CARTESBANCAIRES,CUP,DINERSCLUB,DISCOVER,EFTPOS,ELO,JCB,JCREW,MADA,MAESTRO,MEEZA",
        Key = AttributeKey.AllowedCardNetworks,
        Description = "Select the card networks to allow for this gateway. Selections must be supported by your Cybersource configuration.",
        IsRequired = true,
        DefaultValue = "VISA|MASTERCARD",
        Order = 9 )]

    #endregion

    /// <summary>
    /// CyberSource Payment Gateway
    /// </summary>
    public class Gateway : GatewayComponent, IHostedGatewayComponent, IObsidianHostedGatewayComponent
    {
        private static readonly string DemoUrl = "apitest.cybersource.com";
        private static readonly string ProductionUrl = "api.cybersource.com";

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
            public const string CapturePayment = "CapturePayment";
            public const string EventRegistrationAddressMode = "EventRegistrationAddressMode";
            public const string AllowedCardNetworks = "AllowedCardNetworks";

            /// <summary>
            /// The credit card fee coverage percentage
            /// </summary>
            public const string CreditCardFeeCoveragePercentage = "CreditCardFeeCoveragePercentage";
        }

        #endregion Attribute Keys

        /// <summary>
        /// Gets the gateway URL.
        /// </summary>
        /// <param name="financialGateway">The financial gateway.</param>
        /// <returns>string</returns>
        /// <value>
        /// The gateway URL.
        /// </value>
        [System.Diagnostics.DebuggerStepThrough]
        public static string GetGatewayUrl( FinancialGateway financialGateway )
        {
            financialGateway.LoadAttributes();
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
                // In the event we wish to check whether recurring billing is truly supported this is where the code would have to be. For now I am not going to.
                var values = new List<DefinedValueCache>();
                values.Add( DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.TRANSACTION_FREQUENCY_WEEKLY ) );
                values.Add( DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.TRANSACTION_FREQUENCY_BIWEEKLY ) );
                values.Add( DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.TRANSACTION_FREQUENCY_MONTHLY ) );
                values.Add( DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.TRANSACTION_FREQUENCY_QUARTERLY ) );
                values.Add( DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.TRANSACTION_FREQUENCY_YEARLY ) );
                return values;
            }
        }

        /// <summary>
        /// Gets the URL that the Gateway Information UI will navigate to when they click the 'Configure' link
        /// </summary>
        /// <value>
        /// The configure URL.
        /// </value>
        public string ConfigureURL => "https://businesscenter.cybersource.com/ebc2/";

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
        /// <returns>boolean</returns>
        public override bool SupportsSavedAccount( DefinedValueCache currencyType )
        {
            return true;
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
            errorMessage = "This gateway does not support authorization transactions. Authorization transactions should be created through the CyberSource interface.";
            return null;
        }

        /// <summary>
        /// Charges the specified payment info using the Payments API
        /// </summary>
        /// <param name="financialGateway">The Rock financial gateway.</param>
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

            var customerId = referencedPaymentInfo.GatewayPersonIdentifier.Replace( "null", "" );
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
                Comments: description
            );

            string definedCurrencyCode = GetCurrencyCode( referencedPaymentInfo );
            Ptsv2paymentsOrderInformationAmountDetails orderInformationAmountDetails = new Ptsv2paymentsOrderInformationAmountDetails(
                TotalAmount: amount.ToString( "0.00" ),
                Currency: definedCurrencyCode
            );

            bool processingInformationCapture = financialGateway.GetAttributeValue( AttributeKey.CapturePayment ).AsBoolean();
            Ptsv2paymentsProcessingInformation paymentProcessingInformation = new Ptsv2paymentsProcessingInformation(
                 Capture: processingInformationCapture
             );

            Ptsv2paymentsOrderInformationBillTo orderInformationBillTo = new Ptsv2paymentsOrderInformationBillTo(
                FirstName: paymentInfo.FirstName,
                LastName: paymentInfo.LastName.IsNullOrWhiteSpace() ? paymentInfo.BusinessName : paymentInfo.LastName,
                Address1: paymentInfo.Street1,
                Address2: paymentInfo.Street2,
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

            Ptsv2paymentsPaymentInformationCustomer paymentInformationCustomer = new Ptsv2paymentsPaymentInformationCustomer(
                Id: customerId
            );

            Ptsv2paymentsPaymentInformation paymentInformation = new Ptsv2paymentsPaymentInformation(
                Customer: paymentInformationCustomer
            );

            Ptsv2paymentsTokenInformation tokenInformation = new Ptsv2paymentsTokenInformation(
                TransientTokenJwt: tokenizerToken
            );

            Ptsv2paymentsDeviceInformation deviceInformation = new Ptsv2paymentsDeviceInformation(
                IpAddress: paymentInfo.IPAddress
            );

            CreatePaymentRequest requestObj = null;

            if ( customerId.IsNotNullOrWhiteSpace() )
            {
                requestObj = new CreatePaymentRequest(
                    ProcessingInformation: paymentProcessingInformation,
                    PaymentInformation: paymentInformation,
                    ClientReferenceInformation: clientReferenceInformation,
                    OrderInformation: orderInformation,
                    DeviceInformation: deviceInformation
                );
            }
            else
            {
                paymentProcessingInformation.ActionList = new List<string> { "TOKEN_CREATE" };
                paymentProcessingInformation.ActionTokenTypes = new List<string> { "customer", "paymentInstrument" };

                requestObj = new CreatePaymentRequest(
                    ProcessingInformation: paymentProcessingInformation,
                    ClientReferenceInformation: clientReferenceInformation,
                    OrderInformation: orderInformation,
                    TokenInformation: tokenInformation,
                    DeviceInformation: deviceInformation
                );
            }

            PtsV2PaymentsPost201Response chargeResult = null;
            try
            {
                var clientConfig = Configuration.GetClientConfig( financialGateway );

                var apiInstance = new PaymentsApi( clientConfig );
                chargeResult = apiInstance.CreatePayment( requestObj );
            }
            catch ( Exception e )
            {
                ExceptionLogService.LogException( $"Exception on calling the Cybersource API: {e.Message}" );
                errorMessage += "We encountered an error while calling the Gateway provider API. Kindly double-check the payment information you entered and try again.";
            }

            if ( chargeResult == null || chargeResult.ErrorInformation != null )
            {
                if ( chargeResult == null )
                {
                    return null;
                }

                errorMessage += string.Format( "{0} ({1})", chargeResult.ErrorInformation.Message, chargeResult.ErrorInformation.Reason );
                ExceptionLogService.LogException( $"Error processing CyberSource transaction. Result Code:  {chargeResult.ErrorInformation.Message} ({chargeResult.ErrorInformation.Reason})." );
                return null;
            }

            var transaction = new FinancialTransaction();
            transaction.TransactionCode = chargeResult.Id;
            transaction.ForeignKey = chargeResult.PaymentInformation?.Customer?.Id;
            if ( transaction.ForeignKey.IsNullOrWhiteSpace() )
            {
                transaction.ForeignKey = chargeResult.TokenInformation?.Customer?.Id;
            }

            TssV2TransactionsGet200Response transactionDetail = null;
            var requestCount = 0;
            while ( ( transactionDetail == null || transactionDetail.PaymentInformation == null ) && requestCount < 10 )
            {
                transactionDetail = GetTransactionDetailResponse( financialGateway, chargeResult.Id );
                requestCount += 1;
                Thread.Sleep( 250 );
            }
            transaction.FinancialPaymentDetail = CreatePaymentPaymentDetail( transactionDetail );

            transaction.AdditionalLavaFields = GetAdditionalLavaFields( chargeResult );

            return transaction;
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
            errorMessage = string.Empty;

            var financialGateway = origTransaction?.FinancialGateway ?? new FinancialGatewayService( new RockContext() ).Get( origTransaction.FinancialGatewayId ?? 0 );

            if ( financialGateway == null )
            {
                throw new NullFinancialGatewayException();
            }

            if ( origTransaction.TransactionCode.IsNullOrWhiteSpace() )
            {
                errorMessage = "Invalid transaction code";
                return null;
            }

            string defaultCurrency = GetCurrencyCode( null );
            var currencyCode = origTransaction.ForeignCurrencyCodeValueId.HasValue ?
                            DefinedValueCache.Get( origTransaction.ForeignCurrencyCodeValueId.Value ).Value :
                            defaultCurrency;
            Ptsv2paymentsidcapturesOrderInformationAmountDetails orderInformationAmountDetails = new Ptsv2paymentsidcapturesOrderInformationAmountDetails(
                TotalAmount: amount.ToString(),
                Currency: currencyCode
           );

            Ptsv2paymentsidrefundsOrderInformation orderInformation = new Ptsv2paymentsidrefundsOrderInformation(
                AmountDetails: orderInformationAmountDetails
           );

            var refundRequestObj = new RefundPaymentRequest(
                OrderInformation: orderInformation
           );

            Ptsv2paymentsidreversalsReversalInformationAmountDetails reversalInformationAmountDetails = new Ptsv2paymentsidreversalsReversalInformationAmountDetails(
                TotalAmount: origTransaction.TotalAmount.ToString(),
                Currency: currencyCode
            );

            Ptsv2paymentsidreversalsReversalInformation reversalInformation = new Ptsv2paymentsidreversalsReversalInformation(
                AmountDetails: reversalInformationAmountDetails,
                Reason: comment
           );

            var reversalRequestObj = new AuthReversalRequest(
                ReversalInformation: reversalInformation
           );

            var applications = new List<TssV2TransactionsGet200ResponseApplicationInformationApplications>();
            PtsV2PaymentsRefundPost201Response refundResponse = null;
            PtsV2PaymentsReversalsPost201Response reversalResponse = null;
            TssV2TransactionsGet200Response transactionDetail = GetTransactionDetailResponse( financialGateway, origTransaction.TransactionCode );
            var clientConfig = Configuration.GetClientConfig( financialGateway );
            if ( transactionDetail != null && transactionDetail.ApplicationInformation != null )
            {
                applications.AddRange( transactionDetail.ApplicationInformation.Applications );
                if ( transactionDetail.Links != null && transactionDetail.Links.RelatedTransactions != null && transactionDetail.Links.RelatedTransactions.Any() )
                {
                    foreach ( var relatedTransactionLink in transactionDetail.Links.RelatedTransactions )
                    {
                        var relatedTransaction = GetTransactionDetailResponse( financialGateway, relatedTransactionLink.Href.Substring( relatedTransactionLink.Href.LastIndexOf( "/" ) + 1 ) );
                        if ( relatedTransaction != null && relatedTransaction.ApplicationInformation != null )
                        {
                            applications.AddRange( relatedTransaction.ApplicationInformation.Applications );
                        }
                    }
                }
            }
            var attemptType = "Refund";
            var alreadyHandledErrors = new List<string> { "TRANSACTION_ALREADY_REVERSED_OR_SETTLED", "AUTH_ALREADY_REVERSED", "DUPLICATE_REQUEST", "CAPTURE_ALREADY_VOIDED" };
            try
            {
                if ( applications.Any( a => a.Name == "ics_bill" && a.ReasonCode == "100" ) )
                {
                    var apiInstance = new RefundApi( clientConfig );
                    refundResponse = apiInstance.RefundPayment( refundRequestObj, origTransaction.TransactionCode );
                }
                else
                {
                    attemptType = "Reversal";
                    var apiInstance = new ReversalApi( clientConfig );
                    reversalResponse = apiInstance.AuthReversal( origTransaction.TransactionCode, reversalRequestObj );
                }
            }
            catch ( Exception e )
            {
                var apiException = ( CyberSourceSDK.Client.ApiException ) e;
                ExceptionLogService.LogException( $"Exception on calling the Cybersource API: {e.Message}" );
                errorMessage += "We encountered an error while calling the Gateway provider API. Kindly double-check the payment information you entered and try again.";

                PtsV2PaymentsRefundPost201Response alreadyHandled = null;

                if ( apiException != null && apiException.ErrorContent != null )
                {
                    PushFunds401Response parsedError = JsonConvert.DeserializeObject<PushFunds401Response>( apiException.ErrorContent );
                    if ( parsedError != null && alreadyHandledErrors.Contains( parsedError.Reason ) )
                    {
                        errorMessage = $"The refund/reversal has already been handled, please check with your Gateway Provider for more info. ({parsedError.Reason}) ";
                        if ( origTransaction.Refunds == null || !origTransaction.Refunds.Any() )
                        {
                            refundResponse = new PtsV2PaymentsRefundPost201Response( Id: parsedError.Id );
                            alreadyHandled = refundResponse;
                        }
                    }
                }

                try
                {
                    if ( attemptType == "Refund" )
                    {
                        attemptType = "Reversal";
                        var apiInstance = new ReversalApi( clientConfig );
                        reversalResponse = apiInstance.AuthReversal( origTransaction.TransactionCode, reversalRequestObj );
                    }
                    else
                    {
                        attemptType = "Refund";
                        var apiInstance = new RefundApi( clientConfig );
                        refundResponse = apiInstance.RefundPayment( refundRequestObj, origTransaction.TransactionCode );
                    }
                }
                catch ( Exception ie )
                {
                    ExceptionLogService.LogException( $"Exception on calling the Cybersource API: {e.Message}" );
                    errorMessage += "We encountered an error while calling the Gateway provider API. Kindly double-check the refund information entered and try again.";
                    if ( alreadyHandled != null )
                    {
                        refundResponse = alreadyHandled;
                    }
                }
            }

            if ( reversalResponse == null && ( refundResponse == null || refundResponse.Status != "PENDING" ) || refundResponse == null && ( reversalResponse == null || ( reversalResponse.Status != "REVERSED" && reversalResponse.Status != "PARTIALLY_REVERSED" ) ) )
            {
                if ( refundResponse == null && reversalResponse == null )
                {
                    return null;
                }
                ExceptionLogService.LogException( $"Error processing CyberSource transaction. Result Code:  {refundResponse?.Status ?? reversalResponse?.Status})." );
            }

            // return a refund transaction
            var transaction = new FinancialTransaction();
            transaction.TransactionCode = refundResponse?.Id ?? reversalResponse?.Id;
            if ( attemptType == "Reversal" && origTransaction.TotalAmount != amount )
            {
                transaction.Summary += "Due to Authorization Reversal partial amounts are not supported.";
            }
            return transaction;
        }

        /// <summary>
        /// Adds the scheduled payment.
        /// </summary>
        /// <param name="financialGateway">The financial gateway.</param>
        /// <param name="schedule">The schedule.</param>
        /// <param name="paymentInfo">The payment info.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <returns></returns>
        /// <exception cref="ReferencePaymentInfoRequired"></exception>
        public override FinancialScheduledTransaction AddScheduledPayment( FinancialGateway financialGateway, PaymentSchedule schedule, PaymentInfo paymentInfo, out string errorMessage )
        {
            var descriptionGuid = Guid.NewGuid();
            var descriptionCode = descriptionGuid.ToString().RemoveSpecialCharacters().Left( 8 );

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
            if ( customerId == "null" )
            {
                errorMessage = "There was an error creating the customer on the Gateway. Please try again later.";
                return null;
            }

            string subscriptionId = "";
            string subscriptionDescription = $"{referencedPaymentInfo.Description} - Subscription Ref: {descriptionGuid}";

            var clientConfig = Configuration.GetClientConfig( financialGateway );

            try
            {
                errorMessage = string.Empty;

                Riskv1decisionsClientReferenceInformationPartner clientPartnerInformation = new Riskv1decisionsClientReferenceInformationPartner(
                    SolutionId: descriptionCode
                );

                Rbsv1subscriptionsClientReferenceInformation clientReferenceInformation = new Rbsv1subscriptionsClientReferenceInformation(
                    Code: descriptionCode,
                    Partner: clientPartnerInformation
                );

                var startDateUTC = schedule.StartDate.ToUniversalTime();
                Rbsv1subscriptionsSubscriptionInformation subscriptionInformation = new Rbsv1subscriptionsSubscriptionInformation(
                    Code: descriptionCode,
                    Name: subscriptionDescription,
                    StartDate: startDateUTC.ToString( "yyyy-MM-ddTHH:mm:ssZ" )
                );
                string definedCurrencyCode = GetCurrencyCode( referencedPaymentInfo );
                GetAllPlansResponseOrderInformationAmountDetails orderInformationAmountDetails = new GetAllPlansResponseOrderInformationAmountDetails(
                    BillingAmount: paymentInfo.Amount.ToString( "0.00" ),
                    Currency: definedCurrencyCode
                );

                GetAllPlansResponseOrderInformation orderInformation = new GetAllPlansResponseOrderInformation(
                    AmountDetails: orderInformationAmountDetails
                );

                string processingInformationCommerceIndicator = "recurring";
                string processingInformationAuthorizationOptionsInitiatorType = "merchant";
                Rbsv1subscriptionsProcessingInformationAuthorizationOptionsInitiator processingInformationAuthorizationOptionsInitiator = new Rbsv1subscriptionsProcessingInformationAuthorizationOptionsInitiator(
                    Type: processingInformationAuthorizationOptionsInitiatorType
                );

                Rbsv1subscriptionsProcessingInformationAuthorizationOptions processingInformationAuthorizationOptions = new Rbsv1subscriptionsProcessingInformationAuthorizationOptions(
                    Initiator: processingInformationAuthorizationOptionsInitiator
                );

                Rbsv1subscriptionsProcessingInformation processingInformation = new Rbsv1subscriptionsProcessingInformation(
                    CommerceIndicator: processingInformationCommerceIndicator,
                    AuthorizationOptions: processingInformationAuthorizationOptions
                );

                Rbsv1subscriptionsPaymentInformationCustomer paymentInformationCustomer = new Rbsv1subscriptionsPaymentInformationCustomer(
                    Id: customerId
                );

                Rbsv1subscriptionsPaymentInformation paymentInformation = new Rbsv1subscriptionsPaymentInformation(
                    Customer: paymentInformationCustomer
                );

                Rbsv1subscriptionsPlanInformation planInformation = new Rbsv1subscriptionsPlanInformation();

                CreateSubscriptionResponse subscriptionResult = null;

                if ( SetSubscriptionPlanParams( planInformation, subscriptionInformation, schedule.TransactionFrequencyValue.Guid, schedule.StartDate, out errorMessage ) )
                {

                    var requestObj = new CreateSubscriptionRequest(
                        ClientReferenceInformation: clientReferenceInformation,
                        ProcessingInformation: processingInformation,
                        PlanInformation: planInformation,
                        SubscriptionInformation: subscriptionInformation,
                        OrderInformation: orderInformation,
                        PaymentInformation: paymentInformation
                    );

                    try
                    {
                        var apiInstance = new SubscriptionsApi( clientConfig );
                        subscriptionResult = apiInstance.CreateSubscription( requestObj );
                    }
                    catch ( Exception e )
                    {
                        ExceptionLogService.LogException( $"Exception on calling the Cybersource Subscriptions API: {e.Message}" );
                        errorMessage += "We encountered an error while calling the Gateway provider API. Kindly double-check the payment information you entered and try again.";
                        return null;
                    }

                    subscriptionId = subscriptionResult.Id;

                    if ( subscriptionId.IsNullOrWhiteSpace() )
                    {
                        // Error from CreateSubscription.
                        errorMessage += "Subscription Id is missing.";
                        return null;
                    }
                }
                else
                {
                    // Error from SetSubscriptionPlanParams.
                    return null;
                }

                // Set the paymentInfo.TransactionCode to the subscriptionId so that we know what CreateSubsciption created.
                // This might be handy in case we have an exception and need to know what the subscriptionId is.
                referencedPaymentInfo.TransactionCode = subscriptionId;

                var scheduledTransaction = new FinancialScheduledTransaction();
                scheduledTransaction.TransactionCode = subscriptionResult.SubscriptionInformation?.Code ?? customerId;
                scheduledTransaction.GatewayScheduleId = subscriptionId;
                scheduledTransaction.FinancialGatewayId = financialGateway.Id;

                PaymentInstrumentList customerInfo;
                try
                {
                    var apiInstance = new CustomerPaymentInstrumentApi( clientConfig );
                    customerInfo = apiInstance.GetCustomerPaymentInstrumentsList( customerId, null, null, null );
                }
                catch ( Exception e )
                {
                    errorMessage += "Exception getting Customer Information for Scheduled Payment: " + e.Message;
                    return null;
                }

                scheduledTransaction.FinancialPaymentDetail = PopulatePaymentInfo( paymentInfo, customerInfo.Embedded.PaymentInstruments.LastOrDefault() );

                try
                {
                    GetScheduledPaymentStatus( scheduledTransaction, out errorMessage );
                }
                catch ( Exception ex )
                {
                    throw new Exception( $"Exception getting Scheduled Payment Status. {errorMessage}", ex );
                }

                return scheduledTransaction;
            }
            catch ( Exception )
            {
                // If there is an exception, Rock won't save this as a scheduled transaction, so make sure the subscription didn't get created so mystery scheduled transactions don't happen.
                var apiInstance = new SubscriptionsApi( clientConfig );

                GetSubscriptionResponse subscriptionResult = null;
                try
                {
                    subscriptionResult = apiInstance.GetSubscription( subscriptionId );
                }
                catch ( Exception ) { }

                GetAllSubscriptionsResponseSubscriptions orphanedSubscription = null;
                if ( subscriptionResult == null )
                {
                    var subscriptionSearchResult = apiInstance.GetAllSubscriptions( null, null, descriptionCode, null );
                    orphanedSubscription = subscriptionSearchResult.Subscriptions.FirstOrDefault();

                    if ( orphanedSubscription != null )
                    {
                        subscriptionId = orphanedSubscription.Id;
                    }
                }

                if ( subscriptionResult != null || orphanedSubscription != null )
                {
                    apiInstance.CancelSubscription( subscriptionId );
                }

                throw;
            }
        }

        /// <summary>
        /// Reactivates the scheduled payment.
        /// </summary>
        /// <param name="transaction">The transaction.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <returns></returns>
        public override bool ReactivateScheduledPayment( FinancialScheduledTransaction transaction, out string errorMessage )
        {
            errorMessage = "This gateway does not support reactivating scheduled transactions. Inactive transactions should be managed through the CyberSource interface.";
            return false;
        }

        /// <summary>
        /// Updates the scheduled payment.
        /// </summary>
        /// <param name="transaction">The transaction.</param>
        /// <param name="paymentInfo">The payment info.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <returns></returns>
        public override bool UpdateScheduledPayment( FinancialScheduledTransaction scheduledTransaction, PaymentInfo paymentInfo, out string errorMessage )
        {
            errorMessage = string.Empty;

            var referencedPaymentInfo = paymentInfo as ReferencePaymentInfo;
            if ( referencedPaymentInfo == null )
            {
                throw new ReferencePaymentInfoRequired();
            }

            var subscriptionId = scheduledTransaction.GatewayScheduleId;
            var descriptionGuid = Guid.NewGuid();
            string subscriptionDescription = $"{referencedPaymentInfo.Description} - Subscription Ref: {descriptionGuid}";

            var startDateUTC = scheduledTransaction.StartDate.ToUniversalTime();
            Rbsv1subscriptionsSubscriptionInformation subscriptionInformation = new Rbsv1subscriptionsSubscriptionInformation(
                Code: scheduledTransaction.TransactionCode,
                Name: subscriptionDescription,
                StartDate: startDateUTC.ToString( "yyyy-MM-ddTHH:mm:ssZ" )
            );

            Rbsv1subscriptionsidOrderInformationAmountDetails orderInformationAmountDetails = new Rbsv1subscriptionsidOrderInformationAmountDetails(
                BillingAmount: paymentInfo.Amount.ToString( "0.00" )
            );

            Rbsv1subscriptionsidOrderInformation orderInformation = new Rbsv1subscriptionsidOrderInformation(
                AmountDetails: orderInformationAmountDetails
            );

            Rbsv1subscriptionsPaymentInformationCustomer paymentInformationCustomer = null;
            if ( referencedPaymentInfo.GatewayPersonIdentifier.IsNotNullOrWhiteSpace() )
            {
                paymentInformationCustomer = new Rbsv1subscriptionsPaymentInformationCustomer(
                    Id: referencedPaymentInfo.GatewayPersonIdentifier
                );
            }

            Rbsv1subscriptionsPaymentInformation paymentInformation = new Rbsv1subscriptionsPaymentInformation(
                Customer: paymentInformationCustomer
            );

            Rbsv1subscriptionsPlanInformation planInformation = new Rbsv1subscriptionsPlanInformation();

            UpdateSubscriptionResponse subscriptionResult = null;

            var transactionFrequencyGuid = DefinedValueCache.Get( scheduledTransaction.TransactionFrequencyValueId ).Guid;

            var clientConfig = Configuration.GetClientConfig( scheduledTransaction.FinancialGateway );

            GetSubscriptionResponse subscriptionStatusResult = null;
            DateTime? nextPaymentDate = RockDateTime.Now;

            if ( SetSubscriptionPlanParams( planInformation, subscriptionInformation, transactionFrequencyGuid, scheduledTransaction.StartDate, out errorMessage ) )
            {
                var apiInstance = new SubscriptionsApi( clientConfig );

                if ( paymentInformationCustomer == null || referencedPaymentInfo.GatewayPersonIdentifier.IsNullOrWhiteSpace() )
                {
                    // If the GatewayPersonIdentifier wasn't known to Rock, get the CustomerId from Gateway.
                    var subscription = apiInstance.GetSubscription( subscriptionId );
                    referencedPaymentInfo.GatewayPersonIdentifier = subscription.PaymentInformation?.Customer?.Id;
                    paymentInformationCustomer = new Rbsv1subscriptionsPaymentInformationCustomer( Id: referencedPaymentInfo.GatewayPersonIdentifier );
                }

                subscriptionStatusResult = apiInstance.GetSubscription( subscriptionId );
                if ( subscriptionStatusResult.SubscriptionInformation.Status == "SUSPENDED" )
                {
                    // If subscription isn't active (it might be suspended due to expired card),
                    // change the status back to active
                    try
                    {
                        var setSubscriptionStatusResult = apiInstance.ActivateSubscription( subscriptionId );

                        if ( apiInstance.GetStatusCode() != 200 || setSubscriptionStatusResult.Status != "COMPLETED" )
                        {
                            // Write decline/error as an exception.
                            var exception = new Exception( $"Error re-activating Cybersource subscription. Message:  {setSubscriptionStatusResult.Status} " );

                            ExceptionLogService.LogException( exception );

                            errorMessage = setSubscriptionStatusResult.Status;

                            return false;
                        }
                    }
                    catch ( Exception e )
                    {
                        errorMessage += $"Error re-activating Cybersource subscription. Message:  {e.Message} ";

                        var exception = new Exception( errorMessage );
                        ExceptionLogService.LogException( exception );
                        return false;

                    }
                }
                else
                {
                    nextPaymentDate = GetNextPaymentDate( subscriptionStatusResult, out errorMessage );
                }

                var nextPaymentDateUTC = nextPaymentDate.Value.ToUniversalTime();
                try
                {
                    if ( nextPaymentDateUTC != startDateUTC || subscriptionStatusResult.PaymentInformation.Customer.Id != paymentInformation.Customer.Id || subscriptionStatusResult.PlanInformation.BillingPeriod.Unit != planInformation.BillingPeriod.Unit || subscriptionStatusResult.PlanInformation.BillingPeriod.Length != planInformation.BillingPeriod.Length || subscriptionStatusResult.SubscriptionInformation.Status == "CANCELLED" )
                    {
                        // Cancel and add new subscription if they change the gift date or billing period.
                        var deletedGatewayScheduleId = scheduledTransaction.GatewayScheduleId;

                        PaymentSchedule paymentSchedule = new PaymentSchedule
                        {
                            TransactionFrequencyValue = DefinedValueCache.Get( scheduledTransaction.TransactionFrequencyValueId ),
                            StartDate = scheduledTransaction.StartDate,
                            EndDate = scheduledTransaction.EndDate,
                            NumberOfPayments = scheduledTransaction.NumberOfPayments,
                            PersonId = scheduledTransaction.AuthorizedPersonAlias.PersonId
                        };

                        if ( subscriptionStatusResult.SubscriptionInformation.Status != "CANCELLED" )
                        {
                            apiInstance.CancelSubscription( subscriptionId );
                        }

                        var dummyFinancialScheduledTransaction = AddScheduledPayment( scheduledTransaction.FinancialGateway, paymentSchedule, paymentInfo, out errorMessage );

                        if ( dummyFinancialScheduledTransaction != null )
                        {
                            subscriptionId = dummyFinancialScheduledTransaction.GatewayScheduleId;
                            subscriptionInformation.Code = dummyFinancialScheduledTransaction.TransactionCode;

                            // keep track of the deleted schedule id in case some have been processed but not downloaded yet.
                            if ( scheduledTransaction.PreviousGatewayScheduleIds == null )
                            {
                                scheduledTransaction.PreviousGatewayScheduleIds = new List<string>();
                            }

                            scheduledTransaction.PreviousGatewayScheduleIds.Add( deletedGatewayScheduleId );

                            scheduledTransaction.GatewayScheduleId = subscriptionId;

                            scheduledTransaction.IsActive = true;

                            errorMessage = string.Empty;

                            try
                            {
                                GetScheduledPaymentStatus( scheduledTransaction, out errorMessage );
                            }
                            catch ( Exception ex )
                            {
                                throw new Exception( $"Exception getting Scheduled Payment Status. {errorMessage}", ex );
                            }
                        }
                    }
                    else
                    {
                        var updatePlanInformation = new Rbsv1subscriptionsidPlanInformation( planInformation.BillingCycles );
                        var updateSubscriptionInformation = new Rbsv1subscriptionsidSubscriptionInformation( subscriptionInformation.Code, subscriptionInformation.PlanId, subscriptionInformation.Name, subscriptionInformation.StartDate );
                        subscriptionResult = apiInstance.UpdateSubscription( subscriptionId, new UpdateSubscription( PlanInformation: updatePlanInformation, OrderInformation: orderInformation ) );
                        subscriptionId = subscriptionResult?.Id;
                    }
                }
                catch ( Exception e )
                {
                    // Write decline/error as an exception.
                    var exception = new Exception( $"Error processing CyberSource subscription. Message:  {e.Message} " );

                    ExceptionLogService.LogException( exception );
                    errorMessage = "We encountered an error while calling the Gateway provider API. Kindly double-check the subscription information you entered and try again.";


                    return false;
                }

                if ( subscriptionId != scheduledTransaction.GatewayScheduleId )
                {
                    // Shouldn't happen, but just in case...
                    if ( scheduledTransaction.PreviousGatewayScheduleIds == null )
                    {
                        scheduledTransaction.PreviousGatewayScheduleIds = new List<string>();
                    }

                    scheduledTransaction.PreviousGatewayScheduleIds.Add( scheduledTransaction.GatewayScheduleId );

                    referencedPaymentInfo.TransactionCode = subscriptionId;
                    scheduledTransaction.GatewayScheduleId = subscriptionId;
                }
            }
            else
            {
                // Error from SetSubscriptionBillingPlanParameters
                return false;
            }

            var customerId = referencedPaymentInfo.GatewayPersonIdentifier;

            if ( referencedPaymentInfo.IncludesAddressData() )
            {
                PatchCustomerPaymentInstrumentRequest updatePaymentInstrumentResponse = null;
                var customerPaymentApiInstance = new CustomerPaymentInstrumentApi( clientConfig );

                try
                {
                    var customerPaymentInstrumentList = customerPaymentApiInstance.GetCustomerPaymentInstrumentsList( customerId );
                    if ( customerPaymentApiInstance.GetStatusCode() == 200 && customerPaymentInstrumentList != null )
                    {
                        var paymentInstrument = customerPaymentInstrumentList.Embedded?.PaymentInstruments?.FirstOrDefault( pi => pi.Default.HasValue && pi.Default.Value );
                        if ( paymentInstrument != null )
                        {
                            var newBillTo = new Tmsv2customersEmbeddedDefaultPaymentInstrumentBillTo(
                                FirstName: referencedPaymentInfo.FirstName,
                                LastName: referencedPaymentInfo.LastName.IsNullOrWhiteSpace() ? referencedPaymentInfo.BusinessName : referencedPaymentInfo.LastName,
                                Address1: referencedPaymentInfo.Street1,
                                Address2: referencedPaymentInfo.Street2,
                                Locality: referencedPaymentInfo.City,
                                AdministrativeArea: referencedPaymentInfo.State,
                                PostalCode: referencedPaymentInfo.PostalCode,
                                Country: referencedPaymentInfo.Country,
                                Email: ( referencedPaymentInfo.Email != "update@invalid.email" && referencedPaymentInfo.Email.IsNotNullOrWhiteSpace() ) ? referencedPaymentInfo.Email : scheduledTransaction.AuthorizedPersonAlias.Person.Email,
                                PhoneNumber: referencedPaymentInfo.Phone
                            );

                            PatchCustomerPaymentInstrumentRequest patchCustomerPaymentInstrumentRequest = new PatchCustomerPaymentInstrumentRequest( BillTo: newBillTo );

                            updatePaymentInstrumentResponse = customerPaymentApiInstance.PatchCustomersPaymentInstrument( referencedPaymentInfo.GatewayPersonIdentifier, paymentInstrument.Id, patchCustomerPaymentInstrumentRequest );
                        }
                    }
                    if ( customerPaymentApiInstance.GetStatusCode() != 200 )
                    {
                        errorMessage = customerPaymentApiInstance.ToString();
                        return false;
                    }
                }
                catch ( Exception e )
                {
                    // Write decline/error as an exception.
                    var exception = new Exception( $"Error processing CyberSource Address Update. Message:  {e.Message} " );

                    ExceptionLogService.LogException( exception );
                    errorMessage += "We encountered an error while calling the Gateway provider API. Kindly double-check the address information you entered and try again.";

                    return false;
                }

            }

            PaymentInstrumentList customerInfo;
            try
            {
                var customerPaymentApiInstance = new CustomerPaymentInstrumentApi( clientConfig );
                customerInfo = customerPaymentApiInstance.GetCustomerPaymentInstrumentsList( customerId, null, null, null );
            }
            catch ( Exception e )
            {
                errorMessage += "Exception getting Customer Information for Scheduled Payment: " + e.Message;
                return false;
            }

            scheduledTransaction.FinancialPaymentDetail = PopulatePaymentInfo( paymentInfo, customerInfo.Embedded.PaymentInstruments.LastOrDefault( pi => pi.Default.HasValue && pi.Default.Value ) );
            scheduledTransaction.TransactionCode = subscriptionInformation.Code;

            try
            {
                GetScheduledPaymentStatus( scheduledTransaction, out errorMessage );
            }
            catch ( Exception ex )
            {
                throw new Exception( $"Exception getting Scheduled Payment Status. {errorMessage}", ex );
            }

            errorMessage = null;
            return true;
        }

        /// <summary>
        /// Cancels the scheduled payment.
        /// </summary>
        /// <param name="transaction">The scheduled transaction.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <returns></returns>
        public override bool CancelScheduledPayment( FinancialScheduledTransaction transaction, out string errorMessage )
        {
            var subscriptionId = transaction.GatewayScheduleId;

            var clientConfig = Configuration.GetClientConfig( transaction.FinancialGateway );

            var apiInstance = new SubscriptionsApi( clientConfig );

            CancelSubscriptionResponse cancelResult = null;

            try
            {
                cancelResult = apiInstance.CancelSubscription( subscriptionId );
                transaction.IsActive = false;

                errorMessage = string.Empty;
                return true;
            }
            catch ( Exception e )
            {
                var apiException = ( CyberSourceSDK.Client.ApiException ) e;
                if ( ( apiException != null && apiException.ErrorCode == 400 ) || apiInstance.GetStatusCode() == ( int ) System.Net.HttpStatusCode.NotFound || apiInstance.GetStatusCode() == ( int ) System.Net.HttpStatusCode.Forbidden )
                {
                    // Assume that status code of Forbidden or NonFound indicates that the schedule doesn't exist, or was deleted.
                    errorMessage = string.Empty;
                    return true;
                }

                ExceptionLogService.LogException( $"Exception on calling the Cybersource API: {e.Message}" );
                errorMessage = "We encountered an error while calling the Gateway provider API. Kindly double-check the payment information you entered and try again.";
                return false;
            }
        }

        /// <summary>
        /// Gets the scheduled payment status.
        /// </summary>
        /// <param name="transaction">The transaction.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <returns></returns>
        public override bool GetScheduledPaymentStatus( FinancialScheduledTransaction scheduledTransaction, out string errorMessage )
        {
            errorMessage = string.Empty;

            var subscriptionId = scheduledTransaction.GatewayScheduleId;

            FinancialGateway financialGateway = scheduledTransaction.FinancialGateway;
            if ( financialGateway == null && scheduledTransaction.FinancialGatewayId.HasValue )
            {
                financialGateway = new FinancialGatewayService( new RockContext() ).GetNoTracking( scheduledTransaction.FinancialGatewayId.Value );
            }

            SubscriptionsApi apiInstance = null;
            GetSubscriptionResponse subscriptionResponse = null;
            try
            {
                var clientConfig = Configuration.GetClientConfig( financialGateway );

                apiInstance = new SubscriptionsApi( clientConfig );
                subscriptionResponse = apiInstance.GetSubscription( subscriptionId );
            }
            catch ( Exception e )
            {
                ExceptionLogService.LogException( $"Exception on calling the Cybersource API: {e.Message}" );
                errorMessage += "We encountered an error while calling the Gateway provider API. Kindly double-check the subscription information you entered and try again.";
            }

            if ( apiInstance.GetStatusCode() == ( int ) System.Net.HttpStatusCode.OK )
            {
                if ( subscriptionResponse != null )
                {
                    scheduledTransaction.NextPaymentDate = GetNextPaymentDate( subscriptionResponse, out errorMessage );
                    scheduledTransaction.FinancialPaymentDetail.GatewayPersonIdentifier = subscriptionResponse.PaymentInformation?.Customer?.Id;
                    scheduledTransaction.StatusMessage = subscriptionResponse.SubscriptionInformation.Status;
                    scheduledTransaction.Status = GetFinancialScheduledTransactionStatus( subscriptionResponse.SubscriptionInformation.Status );
                    scheduledTransaction.TransactionFrequencyValueId = GetFinancialScheduledTransactionFrequency( subscriptionResponse.PlanInformation.BillingPeriod );
                }

                scheduledTransaction.LastStatusUpdateDateTime = RockDateTime.Now;

                if ( scheduledTransaction.Status == FinancialScheduledTransactionStatus.Canceled )
                {
                    scheduledTransaction.IsActive = false;
                }

                return true;
            }
            else
            {
                if ( apiInstance.GetStatusCode() == ( int ) System.Net.HttpStatusCode.NotFound || apiInstance.GetStatusCode() == ( int ) System.Net.HttpStatusCode.Forbidden )
                {
                    // Assume that a status code of Forbidden or NonFound indicates that the schedule doesn't exist, or was deleted.
                    scheduledTransaction.IsActive = false;
                    return true;
                }

                errorMessage += subscriptionResponse?.ToString();
                return false;
            }
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
            errorMessage = string.Empty;

            var today = RockDateTime.Now;
            var errorMessages = new List<string>();
            var paymentList = new List<Payment>();

            string query = $"submitTimeUtc:[{( ( DateTimeOffset ) startDate.ToUniversalTime() ).ToUnixTimeMilliseconds()} TO {( ( DateTimeOffset ) endDate.ToUniversalTime() ).ToUnixTimeMilliseconds()}] AND orderInformation.amountDetails.totalAmount:[0.01 TO 999999999.99]";
            int offset = 0;
            // maximum limit of the CyberSource API is 2500
            int limit = 2500;
            string sort = "submitTimeUtc:asc";
            var requestObj = new CreateSearchRequest(
                Query: query,
                Offset: offset,
                Limit: limit,
                Sort: sort
            );
            var clientConfig = Configuration.GetClientConfig( gateway );
            var allSubscriptions = new List<GetAllSubscriptionsResponseSubscriptions>();

            TssV2TransactionsPost201Response searchResult = null;
            try
            {
                var apiInstance = new SearchTransactionsApi( clientConfig );
                searchResult = apiInstance.CreateSearch( requestObj );
            }
            catch ( Exception e )
            {
                ExceptionLogService.LogException( $"Exception on calling the Cybersource API: {e.Message}" );
                errorMessage += "We encountered an error while calling the Gateway provider API. Kindly double-check the payment information you entered and try again.";
                return paymentList;
            }

            if ( searchResult == null || searchResult.Embedded == null || !searchResult.Embedded.TransactionSummaries.Any() )
            {
                // empty result
                errorMessage = "Empty response returned From gateway.";
                return paymentList;
            }

            foreach ( var transaction in searchResult.Embedded.TransactionSummaries )
            {
                Payment payment = new Payment();
                payment.TransactionCode = transaction.Id;

                payment.Status = transaction.ApplicationInformation.RFlag;

                payment.IsFailure = transaction.ApplicationInformation.ReasonCode != "100";

                payment.GatewayScheduleId = transaction.ClientReferenceInformation.Partner.SolutionId;

                try
                {
                    if ( payment.GatewayScheduleId == null )
                    {
                        throw new Exception( $"Transaction {payment.TransactionCode} SolutionId is null" );
                    }
                    var apiInstance = new SubscriptionsApi( clientConfig );
                    var subscriptionSearchResult = apiInstance.GetAllSubscriptions( null, null, payment.GatewayScheduleId, null );

                    if ( subscriptionSearchResult.Subscriptions.Count() == 1 )
                    {
                        var subscriptionResult = subscriptionSearchResult.Subscriptions.FirstOrDefault();

                        if ( subscriptionResult != null )
                        {
                            payment.GatewayScheduleId = subscriptionResult.Id;
                        }
                    }
                    else
                    {
                        throw new Exception( $"No subscription or more than one found with Subscription Code {payment.GatewayScheduleId}." );
                    }
                }
                catch ( Exception e )
                {
                    if ( !allSubscriptions.Any() )
                    {
                        allSubscriptions = GetAllSubscriptions( clientConfig );
                    }

                    var getSubscriptionFromList = allSubscriptions.Where( s => s.SubscriptionInformation != null
                        && ( s.SubscriptionInformation.Status == "ACTIVE" || s.SubscriptionInformation.Status == "PENDING" )
                        && s.PaymentInformation != null
                        && s.PaymentInformation.Customer != null
                        && s.PaymentInformation.Customer.Id == transaction.PaymentInformation.Customer.CustomerId
                        && s.OrderInformation != null
                        && s.OrderInformation.AmountDetails != null
                        && s.OrderInformation.AmountDetails.BillingAmount == transaction.OrderInformation.AmountDetails.TotalAmount );

                    if ( getSubscriptionFromList.Count() == 1 )
                    {
                        payment.GatewayScheduleId = getSubscriptionFromList.FirstOrDefault().Id;
                    }
                    else if ( getSubscriptionFromList.Count() != 0 )
                    {
                        errorMessages.Add( $"Failed to retrieve subscription via code {payment.GatewayScheduleId} and multiple matching subscriptions found. Error message: {e.Message}" );
                    }
                }

                payment.GatewayPersonIdentifier = transaction.PaymentInformation?.Customer?.CustomerId;

                payment.Amount = transaction.OrderInformation.AmountDetails.TotalAmount.AsDecimal();

                var statusMessage = new StringBuilder();
                DateTime? transactionDateTime = transaction.SubmitTimeUtc.AsDateTime();
                foreach ( var transactionAction in transaction.ApplicationInformation.Applications )
                {
                    string actionType = transactionAction.Name;

                    string responseText = transactionAction.RMessage;

                    statusMessage.AppendFormat(
                        "{0} ({1}): {2}; Status: {3}",
                        transactionAction.ReconciliationId, ( transactionAction.RFlag.IsNotNullOrWhiteSpace() ) ? transactionAction.RFlag : transactionAction.RCode,
                        actionType,
                        responseText );

                    statusMessage.AppendLine();

                    if ( actionType == "ics_bill" )
                    {
                        payment.IsSettled = true;
                        payment.SettledGroupId = transactionAction.ReconciliationId.Trim();
                        payment.SettledDate = transactionDateTime.HasValue ? transactionDateTime.Value.ToLocalTime() : today;
                    }

                    if ( transactionDateTime.HasValue && ( actionType == "ics_bill" || actionType == "ics_auth" ) )
                    {
                        payment.TransactionDateTime = transactionDateTime.Value.ToLocalTime();
                        payment.StatusMessage = statusMessage.ToString().Left( 200 );
                        paymentList.Add( payment );
                    }
                }
            }

            if ( errorMessages.Any() )
            {
                errorMessage = string.Join( "<br>", errorMessages );
            }

            return paymentList;
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
            return transaction?.FinancialPaymentDetail?.GatewayPersonIdentifier;
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
            return scheduledTransaction?.FinancialPaymentDetail?.GatewayPersonIdentifier ?? scheduledTransaction.TransactionCode;
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

        /// <summary>
        /// Gets the JavaScript needed to tell the hostedPaymentInfoControl to get send the paymentInfo and get a token.
        /// Have your 'Next' or 'Submit' call this so that the hostedPaymentInfoControl will fetch the token/response
        /// </summary>
        /// <param name="financialGateway">The financial gateway.</param>
        /// <param name="hostedPaymentInfoControl">The hosted payment information control.</param>
        /// <returns></returns>
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
            }
            else
            {
                referencePaymentInfo.ReferenceNumber = cyberSourcePaymentControl.PaymentInfoToken;
            }
            // no ach support for now, so hard coded to CC instead of from control.
            referencePaymentInfo.InitialCurrencyTypeValue = DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.CURRENCY_TYPE_CREDIT_CARD );
        }

        /// <summary>
        /// Creates the customer account using a token received from the HostedPaymentInfoControl <seealso cref="M:Rock.Financial.IHostedGatewayComponent.GetHostedPaymentInfoControl(Rock.Model.FinancialGateway,System.String,Rock.Financial.HostedPaymentInfoControlOptions)" />
        /// and returns a customer account token that can be used for future transactions.
        /// </summary>
        /// <param name="financialGateway">The financial gateway.</param>
        /// <param name="paymentInfo">The payment information.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <returns></returns>
        /// <exception cref="rocks.kfs.CyberSource.Gateway.ReferencePaymentInfoRequired"></exception>
        /// <exception cref="rocks.kfs.CyberSource.Gateway.NullFinancialGatewayException"></exception>
        public string CreateCustomerAccount( FinancialGateway financialGateway, ReferencePaymentInfo paymentInfo, out string errorMessage )
        {
            errorMessage = string.Empty;

            if ( paymentInfo == null )
            {
                throw new ReferencePaymentInfoRequired();
            }

            if ( financialGateway == null )
            {
                throw new NullFinancialGatewayException();
            }

            var tokenizerToken = paymentInfo.ReferenceNumber;

            if ( tokenizerToken.StartsWith( "{" ) && tokenizerToken.EndsWith( "}" ) )
            {
                var parseToken = JsonConvert.DeserializeObject<AddressToken>( tokenizerToken );
                if ( parseToken != null )
                {
                    paymentInfo.Street1 = parseToken.BillingAddress.Street1;
                    paymentInfo.Street2 = parseToken.BillingAddress.Street2;
                    paymentInfo.City = parseToken.BillingAddress.City;
                    paymentInfo.State = parseToken.BillingAddress.State;
                    paymentInfo.PostalCode = parseToken.BillingAddress.PostalCode;
                    paymentInfo.Country = parseToken.BillingAddress.Country;

                    tokenizerToken = parseToken.OriginalToken;
                }
            }

            Ptsv2paymentsClientReferenceInformation clientReferenceInformation = new Ptsv2paymentsClientReferenceInformation(
                Comments: paymentInfo.FullName
            );

            string definedCurrencyCode = GetCurrencyCode( paymentInfo );

            Ptsv2paymentsOrderInformationAmountDetails orderInformationAmountDetails = new Ptsv2paymentsOrderInformationAmountDetails(
                TotalAmount: "0.00",
                Currency: definedCurrencyCode
            );

            Ptsv2paymentsProcessingInformation paymentProcessingInformation = new Ptsv2paymentsProcessingInformation(
                 ActionList: new List<string> { "TOKEN_CREATE" },
                 ActionTokenTypes: new List<string> { "customer", "paymentInstrument" }
             );

            Ptsv2paymentsOrderInformationBillTo orderInformationBillTo = new Ptsv2paymentsOrderInformationBillTo(
                FirstName: paymentInfo.FirstName,
                LastName: paymentInfo.LastName.IsNullOrWhiteSpace() ? paymentInfo.BusinessName : paymentInfo.LastName,
                Address1: paymentInfo.Street1,
                Address2: paymentInfo.Street2,
                Locality: paymentInfo.City,
                AdministrativeArea: paymentInfo.State,
                PostalCode: paymentInfo.PostalCode,
                Country: paymentInfo.Country,
                Email: paymentInfo.Email ?? "update@invalid.email",
                PhoneNumber: paymentInfo.Phone
            );

            Ptsv2paymentsOrderInformation orderInformation = new Ptsv2paymentsOrderInformation(
                AmountDetails: orderInformationAmountDetails,
                BillTo: orderInformationBillTo
            );

            Ptsv2paymentsTokenInformation tokenInformation = new Ptsv2paymentsTokenInformation(
                TransientTokenJwt: tokenizerToken
            );

            Ptsv2paymentsDeviceInformation deviceInformation = new Ptsv2paymentsDeviceInformation(
                IpAddress: paymentInfo.IPAddress
            );

            CreatePaymentRequest requestObj = new CreatePaymentRequest(
                    ProcessingInformation: paymentProcessingInformation,
                    ClientReferenceInformation: clientReferenceInformation,
                    OrderInformation: orderInformation,
                    TokenInformation: tokenInformation,
                    DeviceInformation: deviceInformation
                );

            PtsV2PaymentsPost201Response tokenResult = null;
            try
            {
                var clientConfig = Configuration.GetClientConfig( financialGateway );

                var apiInstance = new PaymentsApi( clientConfig );
                tokenResult = apiInstance.CreatePayment( requestObj );
            }
            catch ( Exception e )
            {
                ExceptionLogService.LogException( $"Exception on calling the Cybersource API: {e.Message}" );
                errorMessage += "We encountered an error while calling the Gateway provider API. Kindly double-check the payment information you entered and try again.";
            }

            if ( tokenResult == null || tokenResult.ErrorInformation != null )
            {
                if ( tokenResult == null )
                {
                    return null;
                }

                errorMessage += string.Format( "{0} ({1})", tokenResult.ErrorInformation.Message, tokenResult.ErrorInformation.Reason );
                ExceptionLogService.LogException( $"Error processing CyberSource Customer Token Creation. Result Code:  {tokenResult.ErrorInformation.Message} ({tokenResult.ErrorInformation.Reason})." );
            }

            var customerId = tokenResult.PaymentInformation?.Customer?.Id;
            if ( customerId.IsNullOrWhiteSpace() )
            {
                customerId = tokenResult.TokenInformation?.Customer?.Id;
            }
            if ( customerId.IsNullOrWhiteSpace() )
            {
                customerId = "null";
            }
            return customerId;
        }

        /// <summary>
        /// Gets the earliest scheduled start date that the gateway will accept for the start date, based on the current local time.
        /// </summary>
        /// <param name="financialGateway">The financial gateway.</param>
        /// <returns></returns>
        public DateTime GetEarliestScheduledStartDate( FinancialGateway financialGateway )
        {
            return RockDateTime.Today.Date;
        }

        /// <summary>
        /// Gets the hosted gateway modes that this gateway has configured/supports. Use this to determine which mode to use (in cases where both are supported, like Scheduled Payments lists ).
        /// If the Gateway supports both hosted and unhosted (and has Hosted mode configured), hosted mode should be preferred.
        /// </summary>
        /// <param name="financialGateway"></param>
        /// <returns></returns>
        /// <value>
        /// The hosted gateway modes that this gateway supports
        /// </value>
        public HostedGatewayMode[] GetSupportedHostedGatewayModes( FinancialGateway financialGateway )
        {
            return new HostedGatewayMode[1] { HostedGatewayMode.Hosted };
        }

        #endregion

        #region IObsidianFinancialGateway

        /// <inheritdoc/>
        public string GetObsidianControlFileUrl( FinancialGateway financialGateway )
        {
            return "/Plugins/rocks_kfs/Obsidian/Controls/cyberSourceGatewayControl.obs.js";
        }

        /// <inheritdoc/>
        public object GetObsidianControlSettings( FinancialGateway financialGateway, HostedPaymentInfoControlOptions options )
        {
            var microFormJWK = "";
            var microFormJsPath = Configuration.GetMicroFormJWK( financialGateway, out microFormJWK );
            var clientIntegrity = "";

            var microFormParameters = microFormJsPath.Split( '|' );

            if ( microFormParameters.Length > 1 && microFormParameters[1].IsNotNullOrWhiteSpace() )
            {
                clientIntegrity = microFormParameters[1];
            }

            microFormJsPath = microFormParameters[0];

            DateTime epoch = new DateTime( 1970, 1, 1, 0, 0, 0, DateTimeKind.Utc );
            DateTime now = DateTime.UtcNow;
            long millisecondsSinceEpoch = ( long ) ( now - epoch ).TotalMilliseconds;

            return new
            {
                gatewayUrl = GetGatewayUrl( financialGateway ),
                addressMode = financialGateway.GetAttributeValue( AttributeKey.EventRegistrationAddressMode ),
                microFormJsPath,
                microFormJWK,
                jwkGeneratedTime = millisecondsSinceEpoch,
                integrity = clientIntegrity
            };
        }

        /// <inheritdoc/>
        public bool TryGetPaymentTokenFromParameters( FinancialGateway financialGateway, IDictionary<string, string> parameters, out string paymentToken )
        {
            paymentToken = null;

            return false;
        }

        /// <inheritdoc/>
        public bool IsPaymentTokenCharged( FinancialGateway financialGateway, string paymentToken )
        {
            return false;
        }

        /// <inheritdoc/>
        public FinancialTransaction FetchPaymentTokenTransaction( RockContext rockContext, FinancialGateway financialGateway, int? fundId, string paymentToken )
        {
            // This method is not required in our implementation.
            throw new NotImplementedException();
        }

        #endregion

        #region Private Helpers

        /// <summary>
        /// Gets the transaction detail response from the CyberSource API.
        /// </summary>
        /// <param name="financialGateway">The financial gateway.</param>
        /// <param name="id">The transaction identifier.</param>
        /// <returns></returns>
        private TssV2TransactionsGet200Response GetTransactionDetailResponse( FinancialGateway financialGateway, string id )
        {
            try
            {
                var clientConfig = Configuration.GetClientConfig( financialGateway );

                var apiInstance = new TransactionDetailsApi( clientConfig );
                var result = apiInstance.GetTransaction( id );
                return result;
            }
            catch ( Exception e )
            {
                ExceptionLogService.LogException( $"Exception on calling the Cybersource API: {e.Message}" );
                return null;
            }
        }

        /// <summary>
        /// Populates the payment information from the CyberSource API Response.
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
        /// Updates the financial payment detail fields from the information in transactionDetail response from the CyberSource API
        /// </summary>
        /// <param name="transactionDetail">The customer information.</param>
        /// <param name="financialPaymentDetail">The financial payment detail.</param>
        private void UpdateFinancialPaymentDetail( TssV2TransactionsGet200Response transactionDetail, FinancialPaymentDetail financialPaymentDetail )
        {
            financialPaymentDetail.GatewayPersonIdentifier = transactionDetail.PaymentInformation?.Customer?.Id;

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
                financialPaymentDetail.AccountNumberMasked = $"{transactionDetail.PaymentInformation.Card.Prefix}XXXXXX{transactionDetail.PaymentInformation.Card.Suffix}";

                financialPaymentDetail.ExpirationMonth = transactionDetail.PaymentInformation.Card.ExpirationMonth.AsIntegerOrNull();
                financialPaymentDetail.ExpirationYear = transactionDetail.PaymentInformation.Card.ExpirationYear.AsIntegerOrNull();
            }
            // ACH not supported by majority of CyberSource Payment Processors.
            //else
            //{
            //    // ach payment
            //    var curType = DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.CURRENCY_TYPE_ACH );
            //    financialPaymentDetail.CurrencyTypeValueId = curType != null ? curType.Id : ( int? ) null;
            //    financialPaymentDetail.AccountNumberMasked = customerInfo.CheckAccount;
            //}
        }

        /// <summary>
        /// Gets the additional lava fields in the form of a flattened dictionary where child properties are delimited with '_' (Billing.Street1 becomes Billing_Street1)
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
        /// Sets the subscription plan parameters in a reusable method before adding or updating a scheduled payment.
        /// </summary>
        /// <param name="planInformation">The plan information.</param>
        /// <param name="subscriptionInformation">The subscription information.</param>
        /// <param name="scheduleTransactionFrequencyValueGuid">The schedule transaction frequency value unique identifier.</param>
        /// <param name="startDate">The start date.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <returns></returns>
        private bool SetSubscriptionPlanParams( Rbsv1subscriptionsPlanInformation planInformation, Rbsv1subscriptionsSubscriptionInformation subscriptionInformation, Guid scheduleTransactionFrequencyValueGuid, DateTime startDate, out string errorMessage )
        {
            errorMessage = string.Empty;

            var startDateUTC = startDate.ToUniversalTime();
            subscriptionInformation.StartDate = startDateUTC.ToString( "yyyy-MM-ddTHH:mm:ssZ" );
            BillingPeriodUnit? billingPeriodUnit = null;
            int billingDuration = 0;
            int billingCycleInterval = 1;

            if ( scheduleTransactionFrequencyValueGuid == Rock.SystemGuid.DefinedValue.TRANSACTION_FREQUENCY_MONTHLY.AsGuid() )
            {
                billingPeriodUnit = BillingPeriodUnit.M;
            }
            else if ( scheduleTransactionFrequencyValueGuid == Rock.SystemGuid.DefinedValue.TRANSACTION_FREQUENCY_WEEKLY.AsGuid() )
            {
                billingPeriodUnit = BillingPeriodUnit.W;
            }
            else if ( scheduleTransactionFrequencyValueGuid == Rock.SystemGuid.DefinedValue.TRANSACTION_FREQUENCY_BIWEEKLY.AsGuid() )
            {
                billingCycleInterval = 2;
                billingPeriodUnit = BillingPeriodUnit.W;
            }
            // DAILY does not exist, but the gateway supports it.
            //else if ( scheduleTransactionFrequencyValueGuid == Rock.SystemGuid.DefinedValue.TRANSACTION_FREQUENCY_DAILY.AsGuid() )
            //{
            //    billingPeriodUnit = BillingPeriodUnit.D;
            //}
            else if ( scheduleTransactionFrequencyValueGuid == Rock.SystemGuid.DefinedValue.TRANSACTION_FREQUENCY_QUARTERLY.AsGuid() )
            {
                billingCycleInterval = 3;
                billingPeriodUnit = BillingPeriodUnit.M;
            }
            else if ( scheduleTransactionFrequencyValueGuid == Rock.SystemGuid.DefinedValue.TRANSACTION_FREQUENCY_YEARLY.AsGuid() )
            {
                billingPeriodUnit = BillingPeriodUnit.Y;
            }
            else
            {
                errorMessage = $"Unsupported Schedule Frequency {DefinedValueCache.Get( scheduleTransactionFrequencyValueGuid )?.Value}";
                return false;
            }

            planInformation.BillingPeriod = new GetAllPlansResponsePlanInformationBillingPeriod( billingCycleInterval.ToString(), billingPeriodUnit.ToString() );
            if ( billingDuration != 0 )
            {
                planInformation.BillingCycles = new Rbsv1plansPlanInformationBillingCycles( billingDuration.ToString() );
            }
            return true;
        }

        /// <summary>
        /// Gets the next payment date by generating it off of the StartDate, Billing Period Unit and Length
        /// </summary>
        /// <param name="subscriptionResponse">The subscription response from the CyberSource API.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <returns></returns>
        private DateTime? GetNextPaymentDate( GetSubscriptionResponse subscriptionResponse, out string errorMessage )
        {
            errorMessage = string.Empty;

            var gatewayStartDate = subscriptionResponse.SubscriptionInformation?.StartDate?.AsDateTime();
            var billingUnit = subscriptionResponse.PlanInformation?.BillingPeriod?.Unit;
            var billingLength = subscriptionResponse.PlanInformation?.BillingPeriod?.Length.AsIntegerOrNull();
            var billingCyclesCurrent = subscriptionResponse.PlanInformation?.BillingCycles?.Current.AsInteger();
            var billingCyclesTotal = subscriptionResponse.PlanInformation?.BillingCycles?.Total.AsInteger();
            if ( gatewayStartDate == null || billingUnit == null || billingLength == null )
            {
                errorMessage = "One of the required fields is null, StartDate, Billing.Unit or Billing.Length";
                return gatewayStartDate;
            }
            var nextPaymentDate = gatewayStartDate;

            if ( ( billingCyclesCurrent == null || billingCyclesTotal == null || billingCyclesTotal == 0 || billingCyclesCurrent <= billingCyclesTotal ) && gatewayStartDate < RockDateTime.Now )
            {
                if ( billingUnit == "W" )
                {
                    var weekCalc = ( 7 * billingLength.Value );
                    var calcDifference = ( RockDateTime.Now - gatewayStartDate.Value ).TotalDays / weekCalc;
                    nextPaymentDate = gatewayStartDate.Value.AddDays( Math.Ceiling( calcDifference ) * weekCalc );
                }
                else if ( billingUnit == "M" )
                {
                    var monthDifference = ( RockDateTime.Now.Year - gatewayStartDate.Value.Year ) * 12 + RockDateTime.Now.Month - gatewayStartDate.Value.Month;
                    if ( billingLength > 1 )
                    {
                        var calcMonthDifference = ( double ) ( monthDifference / billingLength.Value );
                        nextPaymentDate = gatewayStartDate.Value.AddMonths( Math.Ceiling( calcMonthDifference ).ToIntSafe() * billingLength.Value );
                    }
                    else
                    {
                        nextPaymentDate = gatewayStartDate.Value.AddMonths( monthDifference );
                    }

                    if ( nextPaymentDate < RockDateTime.Now )
                    {
                        nextPaymentDate = nextPaymentDate.Value.AddMonths( billingLength.Value );
                    }
                }
                else if ( billingUnit == "Y" )
                {
                    var yearDifference = ( RockDateTime.Now.Year - gatewayStartDate.Value.Year );
                    nextPaymentDate.Value.AddYears( yearDifference );
                    if ( nextPaymentDate < RockDateTime.Now )
                    {
                        nextPaymentDate = nextPaymentDate.Value.AddYears( billingLength.Value );
                    }
                }
                else
                {
                    var calcDifference = ( RockDateTime.Now - gatewayStartDate.Value ).TotalDays;
                    nextPaymentDate = gatewayStartDate.Value.AddDays( calcDifference + 1 );
                }
            }
            else
            {
                errorMessage = "Your billing cycle is complete. There will be no future gifts without setting up a new subscription.";
            }
            return nextPaymentDate;
        }

        /// <summary>
        /// Gets the financial scheduled transaction frequency defined value id based off of the CyberSource API Billing Period Unit.
        /// </summary>
        /// <param name="billingPeriod">The billing period.</param>
        /// <returns></returns>
        private int GetFinancialScheduledTransactionFrequency( GetAllPlansResponsePlanInformationBillingPeriod billingPeriod )
        {
            switch ( billingPeriod.Unit )
            {
                case "W":
                    if ( billingPeriod.Length == "2" )
                    {
                        return DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.TRANSACTION_FREQUENCY_BIWEEKLY.AsGuid() ).Id;
                    }
                    else
                    {
                        return DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.TRANSACTION_FREQUENCY_WEEKLY.AsGuid() ).Id;
                    }
                case "M":
                    if ( billingPeriod.Length == "3" )
                    {
                        return DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.TRANSACTION_FREQUENCY_QUARTERLY.AsGuid() ).Id;
                    }
                    else
                    {
                        return DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.TRANSACTION_FREQUENCY_MONTHLY.AsGuid() ).Id;
                    }
                case "Y":
                    return DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.TRANSACTION_FREQUENCY_YEARLY.AsGuid() ).Id;
                default:
                    return -1;
            }
        }

        /// <summary>
        /// Gets all subscriptions from the CyberSource API, multiple calls can happen to the API to get ALL the subscriptions due to paging limits.
        /// </summary>
        /// <param name="clientConfig">The client configuration.</param>
        /// <returns></returns>
        private List<GetAllSubscriptionsResponseSubscriptions> GetAllSubscriptions( CyberSourceSDK.Client.Configuration clientConfig )
        {
            var allSubscriptions = new List<GetAllSubscriptionsResponseSubscriptions>();
            try
            {
                var apiInstance = new SubscriptionsApi( clientConfig );
                var page = 0;
                var totalCount = 100;
                var pageSize = 100;
                while ( totalCount > pageSize * page )
                {
                    var subscriptionSearchResult = apiInstance.GetAllSubscriptions( page * pageSize, pageSize, null, null );
                    allSubscriptions.AddRange( subscriptionSearchResult.Subscriptions );
                    totalCount = subscriptionSearchResult.TotalCount ?? 100;
                    page += 1;
                }
            }
            catch ( Exception e )
            {
                ExceptionLogService.LogException( $"Error getting CyberSource subscriptions:  {e.Message}." );
            }
            return allSubscriptions;
        }

        /// <summary>
        /// Gets the currency code from the Payment Info or Global attribute if not set.
        /// </summary>
        /// <param name="paymentInfo">The payment information.</param>
        /// <returns></returns>
        private string GetCurrencyCode( ReferencePaymentInfo paymentInfo )
        {
            if ( paymentInfo == null )
            {
                paymentInfo = new ReferencePaymentInfo();
            }

            string defaultCurrency = "USD";
            var globalAttributesCache = GlobalAttributesCache.Get();
            var organizationCurrencyCodeGuid = globalAttributesCache.GetValue( Rock.SystemKey.SystemSetting.ORGANIZATION_CURRENCY_CODE ).AsGuidOrNull();
            var organizationCurrencyCode = DefinedValueCache.Get( organizationCurrencyCodeGuid.Value )?.Value;

            var definedCurrencyCode = DefinedValueCache.Get( paymentInfo.AmountCurrencyCodeValueId.ToIntSafe( -1 ) )?.Value;
            if ( definedCurrencyCode.IsNullOrWhiteSpace() )
            {
                definedCurrencyCode = organizationCurrencyCode ?? defaultCurrency;
            }

            return definedCurrencyCode;
        }

        /// <summary>
        /// Populates the FinancialPaymentDetail record for a FinancialTransaction or FinancialScheduledTransaction
        /// </summary>
        /// <param name="paymentInfo">The payment information.</param>
        /// <param name="paymentInstrument">The payment instrument response, contains billing address and card info.</param>
        /// <returns></returns>
        private FinancialPaymentDetail PopulatePaymentInfo( PaymentInfo paymentInfo, Tmsv2customersEmbeddedDefaultPaymentInstrument paymentInstrument )
        {
            FinancialPaymentDetail financialPaymentDetail = new FinancialPaymentDetail();
            if ( paymentInstrument != null )
            {
                // Since we are using a token for payment, it is possible that the Gateway has a different address associated with the payment method.
                financialPaymentDetail.NameOnCard = $"{paymentInstrument.BillTo.FirstName} {paymentInstrument.BillTo.LastName}";

                // If address wasn't collected when entering the transaction, set the address to the billing info returned from the gateway (if any).
                if ( paymentInfo.Street1.IsNullOrWhiteSpace() )
                {
                    if ( paymentInstrument.BillTo.Address1.IsNotNullOrWhiteSpace() )
                    {
                        paymentInfo.Street1 = paymentInstrument.BillTo.Address1;
                        paymentInfo.Street2 = paymentInstrument.BillTo.Address2;
                        paymentInfo.City = paymentInstrument.BillTo.Locality;
                        paymentInfo.State = paymentInstrument.BillTo.AdministrativeArea;
                        paymentInfo.PostalCode = paymentInstrument.BillTo.PostalCode;
                        paymentInfo.Country = paymentInstrument.BillTo.Country;
                    }
                }
            }

            var creditCardResponse = paymentInstrument.Card;
            var achResponse = paymentInstrument.BankAccount;
            financialPaymentDetail.GatewayPersonIdentifier = ( paymentInfo as ReferencePaymentInfo )?.GatewayPersonIdentifier;
            financialPaymentDetail.FinancialPersonSavedAccountId = ( paymentInfo as ReferencePaymentInfo )?.FinancialPersonSavedAccountId;

            if ( creditCardResponse != null )
            {
                financialPaymentDetail.CurrencyTypeValueId = DefinedValueCache.GetId( Rock.SystemGuid.DefinedValue.CURRENCY_TYPE_CREDIT_CARD.AsGuid() );
                financialPaymentDetail.AccountNumberMasked = paymentInstrument.Embedded?.InstrumentIdentifier?.Card?.Number;

                financialPaymentDetail.ExpirationMonth = creditCardResponse.ExpirationMonth.AsIntegerOrNull();
                financialPaymentDetail.ExpirationYear = creditCardResponse.ExpirationYear.AsIntegerOrNull();

                //// The gateway tells us what the CreditCardType is since it was selected using their hosted payment entry frame.
                //// So, first see if we can determine CreditCardTypeValueId using the CardType response from the gateway

                // See if we can figure it out from the CC Type (Amex, Visa, etc)
                var creditCardTypeValue = CreditCardPaymentInfo.GetCreditCardTypeFromName( creditCardResponse.Type );
                if ( creditCardTypeValue == null )
                {
                    // GetCreditCardTypeFromName should have worked, but just in case, see if we can figure it out from the MaskedCard using RegEx
                    creditCardTypeValue = CreditCardPaymentInfo.GetCreditCardTypeFromCreditCardNumber( financialPaymentDetail.AccountNumberMasked );
                }

                financialPaymentDetail.CreditCardTypeValueId = creditCardTypeValue?.Id;
            }
            else if ( achResponse != null )
            {
                financialPaymentDetail.CurrencyTypeValueId = DefinedValueCache.GetId( Rock.SystemGuid.DefinedValue.CURRENCY_TYPE_ACH.AsGuid() );
                financialPaymentDetail.AccountNumberMasked = achResponse.Type;
            }

            return financialPaymentDetail;
        }
        #endregion Private Helpers

        #region Static Helpers

        /// <summary>
        /// Gets the Rock financial scheduled transaction status.
        /// </summary>
        /// <param name="subscriptionStatus">The subscription status from CyberSource.</param>
        /// <returns></returns>
        internal static Rock.Model.FinancialScheduledTransactionStatus? GetFinancialScheduledTransactionStatus( string subscriptionStatus )
        {
            if ( subscriptionStatus == null )
            {
                return null;
            }

            switch ( subscriptionStatus )
            {
                case "PENDING":
                case "ACTIVE":
                    return FinancialScheduledTransactionStatus.Active;
                case "CANCELLED":
                    return FinancialScheduledTransactionStatus.Canceled;
                case "COMPLETED":
                    return FinancialScheduledTransactionStatus.Completed;
                case "FAILED":
                    return FinancialScheduledTransactionStatus.Failed;
                case "DELINQUENT":
                    return FinancialScheduledTransactionStatus.PastDue;
                case "SUSPENDED":
                    return FinancialScheduledTransactionStatus.Paused;
                default:
                    return subscriptionStatus.ConvertToEnumOrNull<FinancialScheduledTransactionStatus>();
            }
        }


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
