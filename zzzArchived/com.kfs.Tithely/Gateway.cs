using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using Rock;
using Rock.Attribute;
using Rock.Data;
using Rock.Financial;
using Rock.Model;
using Rock.Security;
using Rock.Web.Cache;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Security;
using System.Xml.Linq;

namespace com.kfs.Tithely
{
    /// <summary>
    /// Tithely Payment Gateway
    /// </summary>
    [Description( "Tithely Gateway" )]
    [Export( typeof( GatewayComponent ) )]
    [ExportMetadata( "ComponentName", "Tithely Gateway" )]
    [TextField( "Organization Id", "The organization ID for your Tithely account", true, "", "", 0, "", false )]
    [TextField( "Public Key", "The public key for your Tithely account", true, "", "", 1, "", false )]
    [TextField( "Secret Key", "The secret key for your Tithely account", true, "", "", 2, "", true )]
    [BooleanField( "Use Live Mode", "By default the gateway will be in test mode. Toggle this to go live.", false, "", 3, "" )]
    [BooleanField( "Prompt for Billing Name", "Should users be prompted to enter name on the card", false, "", 4 )]
    [BooleanField( "Prompt for Billing Address", "Should users be prompted to enter billing address", false, "", 5 )]
    public class Gateway : ThreeStepGatewayComponent
    {
        #region Gateway Component Implementation

        private const string LiveURL = "https://tithe.ly/api/v1/";
        private const string TestURL = "https://tithelydev.com/api/v1/";

        /// <summary>
        /// Gets the step2 form URL.
        /// </summary>
        /// <value>
        /// The step2 form URL.
        /// </value>
        public override string Step2FormUrl
        {
            get
            {
                return string.Format( "~/Plugins/com_kfs/Tithely/BillingInfo.html?timestamp={0}", RockDateTime.Now.Ticks );
            }
        }

        /// <summary>
        /// Gets a value indicating whether the gateway requires the name on card for CC processing
        /// </summary>
        /// <param name="financialGateway">The financial gateway.</param>
        /// <returns></returns>
        /// <value>
        ///   <c>true</c> if [name on card required]; otherwise, <c>false</c>.
        /// </value>
        public override bool PromptForNameOnCard( FinancialGateway financialGateway )
        {
            return GetAttributeValue( financialGateway, "PromptForBillingName" ).AsBoolean();
        }

        /// <summary>
        /// Gets a value indicating whether gateway provider needs first and last name on credit card as two distinct fields.
        /// </summary>
        /// <value>
        /// <c>true</c> if [split name on card]; otherwise, <c>false</c>.
        /// </value>
        public override bool SplitNameOnCard
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Prompts for the person name associated with a bank account.
        /// </summary>
        /// <param name="financialGateway">The financial gateway.</param>
        /// <returns></returns>
        public override bool PromptForBankAccountName( FinancialGateway financialGateway )
        {
            return true;
        }

        /// <summary>
        /// Gets a value indicating whether [address required].
        /// </summary>
        /// <param name="financialGateway">The financial gateway.</param>
        /// <returns></returns>
        /// <value>
        ///   <c>true</c> if [address required]; otherwise, <c>false</c>.
        /// </value>
        public override bool PromptForBillingAddress( FinancialGateway financialGateway )
        {
            return GetAttributeValue( financialGateway, "PromptForBillingAddress" ).AsBoolean();
        }

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
                var values = new List<DefinedValueCache>();
                values.Add( DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.TRANSACTION_FREQUENCY_ONE_TIME ) );
                values.Add( DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.TRANSACTION_FREQUENCY_WEEKLY ) );
                values.Add( DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.TRANSACTION_FREQUENCY_BIWEEKLY ) );
                values.Add( DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.TRANSACTION_FREQUENCY_TWICEMONTHLY ) );
                values.Add( DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.TRANSACTION_FREQUENCY_MONTHLY ) );
                return values;
            }
        }

        /// <summary>
        /// Returns a boolean value indicating if 'Saved Account' functionality is supported for frequency (i.e. one-time vs repeating )
        /// </summary>
        /// <param name="isRepeating">if set to <c>true</c> [is repeating].</param>
        /// <returns></returns>
        public override bool SupportsSavedAccount( bool isRepeating )
        {
            return true;
        }

        /// <summary>
        /// Gets the financial transaction parameters that are passed to step 1
        /// </summary>
        /// <param name="redirectUrl">The redirect URL.</param>
        /// <returns></returns>
        public override Dictionary<string, string> GetStep1Parameters( string redirectUrl )
        {
            // For this gateway, returns the signed value of parameters that are passed in.
            // If a transaction doesn't pass parameters to this method, return null
            // When creating an actual billable transaction, ChargeStep1 should be used.
            Dictionary<string, string> parameters = Utility.ParseQueryString( redirectUrl );
            if ( parameters == null )
            {
                return null;
            }

            return parameters;
        }

        /// <summary>
        /// Charges the specified payment info.
        /// </summary>
        /// <param name="financialGateway">The financial gateway.</param>
        /// <param name="paymentInfo">The payment info.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <returns></returns>
        public override FinancialTransaction Charge( FinancialGateway financialGateway, PaymentInfo paymentInfo, out string errorMessage )
        {
            errorMessage = "The Payment Gateway only supports a three-step charge.";
            return null;
        }

        /// <summary>
        /// Performs the first step of a three-step charge
        /// </summary>
        /// <param name="financialGateway">The financial gateway.</param>
        /// <param name="paymentInfo">The payment information.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <returns>
        /// Url to post the Step2 request to
        /// </returns>
        /// <exception cref="System.ArgumentNullException">paymentInfo</exception>
        public override string ChargeStep1( FinancialGateway gateway, PaymentInfo paymentInfo, out string errorMessage )
        {
            errorMessage = string.Empty;

            try
            {
                if ( paymentInfo == null )
                {
                    throw new ArgumentNullException( "paymentInfo" );
                }

                var payment = new Dictionary<string, string>
                {
                    { "first_name", paymentInfo.FirstName },
                    { "last_name", paymentInfo.LastName },
                    { "email", paymentInfo.Email },
                    { "password", string.Format( "{0}1!", Membership.GeneratePassword( 8, 0 ) ) },
                    { "pin", paymentInfo.Phone.IsNotNullOrWhiteSpace() ? paymentInfo.Phone.Right( 4 ) : "1234" }
                };

                dynamic result = PostToGateway( gateway, GetRoot( gateway, "accounts" ), payment );

                if ( result == null )
                {
                    errorMessage = "Invalid Response from Tithely!";
                    return null;
                }

                if ( result.status != "success" )
                {
                    errorMessage = result.reason;
                    return null;
                }

                string tithelyAccountToken = result.account_id;
                using ( var rockContext = new RockContext() )
                {
                    // find or create the contributor record
                    var personService = new PersonService( rockContext );
                    var contributor = personService.FindPerson( paymentInfo.FirstName, paymentInfo.LastName, paymentInfo.Email, false, true, true );
                    if ( contributor == null )
                    {
                        var recordTypePersonId = DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.PERSON_RECORD_TYPE_PERSON.AsGuid() ).Id;
                        var recordStatusActiveId = DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.PERSON_RECORD_STATUS_ACTIVE.AsGuid() ).Id;
                        var connectionStatusWebId = DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.PERSON_CONNECTION_STATUS_WEB_PROSPECT ).Id;
                        contributor = new Person
                        {
                            FirstName = paymentInfo.FirstName.FixCase(),
                            LastName = paymentInfo.LastName.FixCase(),
                            Email = paymentInfo.Email,
                            IsEmailActive = true,
                            EmailPreference = EmailPreference.EmailAllowed,
                            RecordStatusValueId = recordStatusActiveId,
                            RecordTypeValueId = recordTypePersonId,
                            Gender = Gender.Unknown,
                            ConnectionStatusValueId = connectionStatusWebId
                        };

                        personService.Add( contributor );
                        rockContext.SaveChanges();
                    }

                    // store the Tithely account id as a person search key
                    var searchKey = contributor.GetPersonSearchKeys( rockContext ).FirstOrDefault( k => k.SearchValue.Equals( tithelyAccountToken ) );
                    if ( searchKey == null )
                    {
                        var alternateValueId = DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.PERSON_SEARCH_KEYS_ALTERNATE_ID.AsGuid() ).Id;
                        searchKey = new PersonSearchKey
                        {
                            SearchTypeValueId = alternateValueId,
                            SearchValue = tithelyAccountToken,
                            PersonAliasId = contributor.PrimaryAliasId
                        };
                        new PersonSearchKeyService( rockContext ).Add( searchKey );
                        rockContext.SaveChanges();
                    }
                }

                return string.Format( "{0}?public_key={1}&account_id={2}", GetRoot( gateway, "charges" ), GetAttributeValue( gateway, "PublicKey" ), tithelyAccountToken );
            }
            catch ( WebException webException )
            {
                var message = GetResponseMessage( webException.Response.GetResponseStream() );
                errorMessage = webException.Message + " - " + message;
                return null;
            }
            catch ( Exception ex )
            {
                errorMessage = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// Performs the final step of a three-step charge.
        /// </summary>
        /// <param name="financialGateway">The financial gateway.</param>
        /// <param name="resultQueryString">The result query string from step 2.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <returns></returns>
        public override FinancialTransaction ChargeStep3( FinancialGateway financialGateway, string resultQueryString, out string errorMessage )
        {
            errorMessage = string.Empty;

            try
            {
                //rootElement.Add( new XElement( "token-id", resultQueryString.Substring( 10 ) ) );

                var functionPath = "charges";
                // functionPath = "charge-once";

                dynamic result = PostToGateway( financialGateway, functionPath, null );

                if ( result == null )
                {
                    errorMessage = "Invalid Response from Tithely!";
                    return null;
                }

                if ( result.GetValueOrNull( "result" ) != "1" )
                {
                    errorMessage = result.GetValueOrNull( "result-text" );

                    string resultCodeMessage = GetResultCodeMessage( result );
                    if ( resultCodeMessage.IsNotNullOrWhiteSpace() )
                    {
                        errorMessage += string.Format( " ({0})", resultCodeMessage );
                    }

                    // write result error as an exception
                    ExceptionLogService.LogException( new Exception( $"Error processing Tithely transaction. Result Code:  {result.GetValueOrNull( "result-code" )} ({resultCodeMessage}). Result text: {result.GetValueOrNull( "result-text" )}. Card Holder Name: {result.GetValueOrNull( "first-name" )} {result.GetValueOrNull( "last-name" )}. Amount: {result.GetValueOrNull( "total-amount" )}. Transaction id: {result.GetValueOrNull( "transaction-id" )}. Descriptor: {result.GetValueOrNull( "descriptor" )}. Order description: {result.GetValueOrNull( "order-description" )}." ) );

                    return null;
                }

                var transaction = new FinancialTransaction();
                transaction.TransactionCode = result.GetValueOrNull( "transaction-id" );
                transaction.ForeignKey = result.GetValueOrNull( "customer-vault-id" );
                transaction.FinancialPaymentDetail = new FinancialPaymentDetail();

                string ccNumber = result.GetValueOrNull( "billing_cc-number" );
                if ( !string.IsNullOrWhiteSpace( ccNumber ) )
                {
                    // cc payment
                    var curType = DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.CURRENCY_TYPE_CREDIT_CARD );
                    transaction.FinancialPaymentDetail.NameOnCardEncrypted = Encryption.EncryptString( $"{result.GetValueOrNull( "billing_first-name" )} {result.GetValueOrNull( "billing_last-name" )}" );
                    transaction.FinancialPaymentDetail.CurrencyTypeValueId = curType != null ? curType.Id : (int?)null;
                    transaction.FinancialPaymentDetail.CreditCardTypeValueId = CreditCardPaymentInfo.GetCreditCardType( ccNumber.Replace( '*', '1' ).AsNumeric() )?.Id;
                    transaction.FinancialPaymentDetail.AccountNumberMasked = ccNumber.Masked( true );

                    string mmyy = result.GetValueOrNull( "billing_cc-exp" );
                    if ( !string.IsNullOrWhiteSpace( mmyy ) && mmyy.Length == 4 )
                    {
                        transaction.FinancialPaymentDetail.ExpirationMonthEncrypted = Encryption.EncryptString( mmyy.Substring( 0, 2 ) );
                        transaction.FinancialPaymentDetail.ExpirationYearEncrypted = Encryption.EncryptString( mmyy.Substring( 2, 2 ) );
                    }
                }
                else
                {
                    // ach payment
                    var curType = DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.CURRENCY_TYPE_ACH );
                    transaction.FinancialPaymentDetail.CurrencyTypeValueId = curType != null ? curType.Id : (int?)null;
                    transaction.FinancialPaymentDetail.AccountNumberMasked = result.GetValueOrNull( "billing_account-number" ).Masked( true );
                }

                transaction.AdditionalLavaFields = new Dictionary<string, object>();
                foreach ( var keyVal in result )
                {
                    transaction.AdditionalLavaFields.Add( keyVal.Key, keyVal.Value );
                }

                return transaction;
            }
            catch ( WebException webException )
            {
                string message = GetResponseMessage( webException.Response.GetResponseStream() );
                errorMessage = webException.Message + " - " + message;
                return null;
            }
            catch ( Exception ex )
            {
                errorMessage = ex.Message;
                return null;
            }
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

            if ( origTransaction != null &&
                !string.IsNullOrWhiteSpace( origTransaction.TransactionCode ) &&
                origTransaction.FinancialGateway != null )
            {
                //rootElement.Add( new XElement( "transaction-id", origTransaction.TransactionCode ) );
                //rootElement.Add( new XElement( "amount", amount.ToString( "0.00" ) ) );

                dynamic result = PostToGateway( origTransaction.FinancialGateway, "refunds", null );

                if ( result == null )
                {
                    errorMessage = "Invalid Response from Tithely!";
                    return null;
                }

                if ( result.GetValueOrNull( "result" ) != "1" )
                {
                    errorMessage = result.GetValueOrNull( "result-text" );
                    return null;
                }

                var transaction = new FinancialTransaction();
                transaction.TransactionCode = result.GetValueOrNull( "transaction-id" );
                return transaction;
            }
            else
            {
                errorMessage = "Invalid original transaction, transaction code, or gateway.";
            }

            return null;
        }

        /// <summary>
        /// Adds the scheduled payment.
        /// </summary>
        /// <param name="financialGateway">The financial gateway.</param>
        /// <param name="schedule">The schedule.</param>
        /// <param name="paymentInfo">The payment info.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <returns></returns>
        public override FinancialScheduledTransaction AddScheduledPayment( FinancialGateway financialGateway, PaymentSchedule schedule, PaymentInfo paymentInfo, out string errorMessage )
        {
            errorMessage = "The Payment Gateway only supports adding scheduled payment using a three-step process.";
            return null;
        }

        /// <summary>
        /// Performs the first step of adding a new payment schedule
        /// </summary>
        /// <param name="financialGateway">The financial gateway.</param>
        /// <param name="schedule">The schedule.</param>
        /// <param name="paymentInfo">The payment information.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">paymentInfo</exception>
        public override string AddScheduledPaymentStep1( FinancialGateway financialGateway, PaymentSchedule schedule, PaymentInfo paymentInfo, out string errorMessage )
        {
            errorMessage = string.Empty;

            try
            {
                if ( paymentInfo == null )
                {
                    throw new ArgumentNullException( "paymentInfo" );
                }

                //new XElement( "start-date", schedule.StartDate.ToString( "yyyyMMdd" ) ),
                //new XElement( "order-description", paymentInfo.Description ),
                //new XElement( "currency", "USD" ),
                //new XElement( "tax-amount", "0.00" ) );

                bool isReferencePayment = ( paymentInfo is ReferencePaymentInfo );
                if ( isReferencePayment )
                {
                    var reference = paymentInfo as ReferencePaymentInfo;
                    //rootElement.Add( new XElement( "customer-vault-id", reference.ReferenceNumber ) );
                }

                if ( paymentInfo.AdditionalParameters != null )
                {
                    foreach ( var keyValue in paymentInfo.AdditionalParameters )
                    {
                        var xElement = new XElement( keyValue.Key, keyValue.Value );
                        //rootElement.Add( xElement );
                    }
                }

                var selectedFrequencyGuid = schedule.TransactionFrequencyValue.Guid.ToString().ToUpper();
                switch ( selectedFrequencyGuid )
                {
                    case Rock.SystemGuid.DefinedValue.TRANSACTION_FREQUENCY_ONE_TIME:
                        {
                            schedule.NumberOfPayments = 1;
                            break;
                        }
                    case Rock.SystemGuid.DefinedValue.TRANSACTION_FREQUENCY_WEEKLY:
                        {
                            break;
                        }
                    case Rock.SystemGuid.DefinedValue.TRANSACTION_FREQUENCY_BIWEEKLY:
                        {
                            //planElement.Add( new XElement( "day-frequency", "14" ) );
                            break;
                        }
                    case Rock.SystemGuid.DefinedValue.TRANSACTION_FREQUENCY_MONTHLY:
                        {
                            //planElement.Add( new XElement( "month-frequency", "1" ) );

                            break;
                        }

                    default:
                        break;
                }

                //rootElement.Add( GetBilling( paymentInfo ) );

                dynamic result = PostToGateway( financialGateway, "recurring", null );

                if ( result == null )
                {
                    errorMessage = "Invalid Response from Tithely!";
                    return null;
                }

                if ( result.GetValueOrNull( "result" ) != "1" )
                {
                    errorMessage = result.GetValueOrNull( "result-text" );
                    return null;
                }

                return result.GetValueOrNull( "form-url" );
            }
            catch ( WebException webException )
            {
                string message = GetResponseMessage( webException.Response.GetResponseStream() );
                errorMessage = webException.Message + " - " + message;
                return null;
            }
            catch ( Exception ex )
            {
                errorMessage = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// Performs the third step of adding a new payment schedule
        /// </summary>
        /// <param name="financialGateway">The financial gateway.</param>
        /// <param name="resultQueryString">The result query string from step 2.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">tokenId</exception>
        public override FinancialScheduledTransaction AddScheduledPaymentStep3( FinancialGateway financialGateway, string resultQueryString, out string errorMessage )
        {
            errorMessage = string.Empty;

            try
            {
                //rootElement.Add( new XElement( "token-id", resultQueryString.Substring( 10 ) ) );

                dynamic result = PostToGateway( financialGateway, "recurring", null );

                if ( result == null )
                {
                    errorMessage = "Invalid Response from Tithely!";
                    return null;
                }

                if ( result.GetValueOrNull( "result" ) != "1" )
                {
                    errorMessage = result.GetValueOrNull( "result-text" );
                    return null;
                }

                var scheduledTransaction = new FinancialScheduledTransaction();
                scheduledTransaction.IsActive = true;
                scheduledTransaction.GatewayScheduleId = result.GetValueOrNull( "subscription-id" );
                scheduledTransaction.FinancialGatewayId = financialGateway.Id;

                scheduledTransaction.FinancialPaymentDetail = new FinancialPaymentDetail();
                string ccNumber = result.GetValueOrNull( "billing_cc-number" );
                if ( !string.IsNullOrWhiteSpace( ccNumber ) )
                {
                    // cc payment
                    var curType = DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.CURRENCY_TYPE_CREDIT_CARD );
                    scheduledTransaction.FinancialPaymentDetail.CurrencyTypeValueId = curType != null ? curType.Id : (int?)null;
                    scheduledTransaction.FinancialPaymentDetail.CreditCardTypeValueId = CreditCardPaymentInfo.GetCreditCardType( ccNumber.Replace( '*', '1' ).AsNumeric() )?.Id;
                    scheduledTransaction.FinancialPaymentDetail.AccountNumberMasked = ccNumber.Masked( true );

                    string mmyy = result.GetValueOrNull( "billing_cc-exp" );
                    if ( !string.IsNullOrWhiteSpace( mmyy ) && mmyy.Length == 4 )
                    {
                        scheduledTransaction.FinancialPaymentDetail.ExpirationMonthEncrypted = Encryption.EncryptString( mmyy.Substring( 0, 2 ) );
                        scheduledTransaction.FinancialPaymentDetail.ExpirationYearEncrypted = Encryption.EncryptString( mmyy.Substring( 2, 2 ) );
                    }
                }
                else
                {
                    // ach payment
                    var curType = DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.CURRENCY_TYPE_ACH );
                    scheduledTransaction.FinancialPaymentDetail.CurrencyTypeValueId = curType != null ? curType.Id : (int?)null;
                    scheduledTransaction.FinancialPaymentDetail.AccountNumberMasked = result.GetValueOrNull( "billing_account_number" ).Masked( true );
                }

                GetScheduledPaymentStatus( scheduledTransaction, out errorMessage );

                return scheduledTransaction;
            }
            catch ( WebException webException )
            {
                string message = GetResponseMessage( webException.Response.GetResponseStream() );
                errorMessage = webException.Message + " - " + message;
                return null;
            }
            catch ( Exception ex )
            {
                errorMessage = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// Flag indicating if gateway supports reactivating a scheduled payment.
        /// </summary>
        /// <returns></returns>
        public override bool ReactivateScheduledPaymentSupported
        {
            get { return false; }
        }

        /// <summary>
        /// Reactivates the scheduled payment.
        /// </summary>
        /// <param name="transaction">The transaction.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <returns></returns>
        public override bool ReactivateScheduledPayment( FinancialScheduledTransaction transaction, out string errorMessage )
        {
            errorMessage = "The Tithely gateway associated with this scheduled tranaction does not support reactivating scheduled transactions. A new scheduled transaction should be created instead.";
            return false;
        }

        /// <summary>
        /// Updates the scheduled payment supported.
        /// </summary>
        /// <returns></returns>
        public override bool UpdateScheduledPaymentSupported
        {
            get { return false; }
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
            errorMessage = "The Tithely gateway associated with this scheduled transaction does not support updating an existing scheduled transaction. A new scheduled transaction should be created instead.";
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
            errorMessage = string.Empty;

            if ( transaction != null &&
                !string.IsNullOrWhiteSpace( transaction.GatewayScheduleId ) &&
                transaction.FinancialGateway != null )
            {
                //rootElement.Add( new XElement( "subscription-id", transaction.GatewayScheduleId ) );

                dynamic result = PostToGateway( transaction.FinancialGateway, "recurring", null );

                if ( result == null )
                {
                    errorMessage = "Invalid Response from Tithely!";
                    return false;
                }

                if ( result.GetValueOrNull( "result" ) != "1" )
                {
                    errorMessage = result.GetValueOrNull( "result-text" );
                    return false;
                }

                transaction.IsActive = false;
                return true;
            }
            else
            {
                errorMessage = "Invalid original transaction, transaction code, or gateway.";
            }

            return false;
        }

        /// <summary>
        /// Gets the scheduled payment status supported.
        /// </summary>
        /// <returns></returns>
        public override bool GetScheduledPaymentStatusSupported
        {
            get { return true; }
        }

        /// <summary>
        /// Gets the scheduled payment status.
        /// </summary>
        /// <param name="transaction">The transaction.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <returns></returns>
        public override bool GetScheduledPaymentStatus( FinancialScheduledTransaction transaction, out string errorMessage )
        {
            errorMessage = string.Empty;
            return true;
        }

        /// <summary>
        /// Gets the payments that have been processed for any scheduled transactions
        /// </summary>
        /// <param name="financialGateway">The financial gateway.</param>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <returns></returns>
        public override List<Payment> GetPayments( FinancialGateway financialGateway, DateTime startDate, DateTime endDate, out string errorMessage )
        {
            errorMessage = string.Empty;

            var txns = new List<Payment>();

            //var restClient = new RestClient( GetAttributeValue( financialGateway, "QueryUrl" ) );
            //var restRequest = new RestRequest( Method.GET );

            //restRequest.AddParameter( "username", GetAttributeValue( financialGateway, "AdminUsername" ) );
            //restRequest.AddParameter( "password", GetAttributeValue( financialGateway, "AdminPassword" ) );
            //restRequest.AddParameter( "start_date", startDate.ToString( "yyyyMMddHHmmss" ) );
            //restRequest.AddParameter( "end_date", endDate.ToString( "yyyyMMddHHmmss" ) );

            //try
            //{
            //    var response = restClient.Execute( restRequest );
            //    if ( response != null )
            //    {
            //        if ( response.StatusCode == HttpStatusCode.OK )
            //        {
            //            var xdocResult = GetXmlResponse( response );
            //            if ( xdocResult != null )
            //            {
            //                var errorResponse = xdocResult.Root.Element( "error_response" );
            //                if ( errorResponse != null )
            //                {
            //                    errorMessage = errorResponse.Value;
            //                }
            //                else
            //                {
            //                    foreach ( var xTxn in xdocResult.Root.Elements( "transaction" ) )
            //                    {
            //                        Payment payment = new Payment();
            //                        //payment.TransactionCode = GetXElementValue( xTxn, "transaction_id" );
            //                        //payment.Status = GetXElementValue( xTxn, "condition" ).FixCase();
            //                        //payment.IsFailure =
            //                        //    payment.Status == "Failed" ||
            //                        //    payment.Status == "Abandoned" ||
            //                        //    payment.Status == "Canceled";
            //                        //payment.TransactionCode = GetXElementValue( xTxn, "transaction_id" );
            //                        //payment.GatewayScheduleId = GetXElementValue( xTxn, "original_transaction_id" ).Trim();

            //                        //var statusMessage = new StringBuilder();
            //                        //DateTime? txnDateTime = null;

            //                        //foreach ( var xAction in xTxn.Elements( "action" ) )
            //                        //{
            //                        //    DateTime? actionDate = ParseDateValue( GetXElementValue( xAction, "date" ) );
            //                        //    string actionType = GetXElementValue( xAction, "action_type" );
            //                        //    string responseText = GetXElementValue( xAction, "response_text" );

            //                        //    if ( actionDate.HasValue )
            //                        //    {
            //                        //        statusMessage.AppendFormat( "{0} {1}: {2}; Status: {3}",
            //                        //            actionDate.Value.ToShortDateString(), actionDate.Value.ToShortTimeString(),
            //                        //            actionType.FixCase(), responseText );
            //                        //        statusMessage.AppendLine();
            //                        //    }

            //                        //    decimal? txnAmount = GetXElementValue( xAction, "amount" ).AsDecimalOrNull();
            //                        //    if ( txnAmount.HasValue && actionDate.HasValue )
            //                        //    {
            //                        //        payment.Amount = txnAmount.Value;
            //                        //    }

            //                        //    if ( actionType == "sale" )
            //                        //    {
            //                        //        txnDateTime = actionDate.Value;
            //                        //    }

            //                        //    if ( actionType == "settle" )
            //                        //    {
            //                        //        payment.IsSettled = true;
            //                        //        payment.SettledGroupId = GetXElementValue( xAction, "processor_batch_id" ).Trim();
            //                        //        payment.SettledDate = actionDate;
            //                        //        txnDateTime = txnDateTime.HasValue ? txnDateTime.Value : actionDate.Value;
            //                        //    }
            //                        //}

            //                        //if ( txnDateTime.HasValue )
            //                        //{
            //                        //    payment.TransactionDateTime = txnDateTime.Value;
            //                        //    payment.StatusMessage = statusMessage.ToString();
            //                        //    txns.Add( payment );
            //                        //}
            //                    }
            //                }
            //            }
            //            else
            //            {
            //                errorMessage = "Invalid XML Document Returned From Gateway!";
            //            }
            //        }
            //        else
            //        {
            //            errorMessage = string.Format( "Invalid Response from Gateway: [{0}] {1}", response.StatusCode.ConvertToString(), response.ErrorMessage );
            //        }
            //    }
            //    else
            //    {
            //        errorMessage = "Null Response From Gateway!";
            //    }
            //}
            //catch ( WebException webException )
            //{
            //    string message = GetResponseMessage( webException.Response.GetResponseStream() );
            //    throw new Exception( webException.Message + " - " + message );
            //}

            return txns;
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
            return transaction.ForeignKey;
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
        /// Gets the root url and adds the function path.
        /// </summary>
        /// <param name="gateway">The gateway.</param>
        /// <param name="elementName">Name of the element.</param>
        /// <returns></returns>
        private string GetRoot( FinancialGateway gateway, string elementName )
        {
            var urlPrefix = GetAttributeValue( gateway, "UseLiveMode" ).AsBoolean() ? LiveURL : TestURL;
            return string.Format( "{0}{1}", urlPrefix.EnsureTrailingForwardslash(), elementName );
        }

        /// <summary>
        /// Posts to gateway.
        /// </summary>
        /// <param name="gateway">The gateway.</param>
        /// <param name="queryUrl">The query URL.</param>
        /// <param name="tithelyParams">The tithely parameters.</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        /// <exception cref="System.Exception"></exception>
        private object PostToGateway( FinancialGateway gateway, string queryUrl, Dictionary<string, string> tithelyParams )
        {
            var restClient = new RestClient( queryUrl )
            {
                Authenticator = new HttpBasicAuthenticator( GetAttributeValue( gateway, "PublicKey" ), GetAttributeValue( gateway, "SecretKey" ) )
            };

            var restRequest = new RestRequest( Method.POST )
            {
                RequestFormat = DataFormat.Json
            };

            foreach ( var param in tithelyParams )
            {
                restRequest.AddParameter( param.Key, param.Value );
            }

            try
            {
                var response = restClient.Execute( restRequest );
                if ( response != null && response.StatusCode == HttpStatusCode.OK )
                {
                    //dynamic result = SimpleJson.DeserializeObject( response.Content );
                    dynamic result = JsonConvert.DeserializeObject<dynamic>( response.Content );
                    return result;
                }
            }
            catch ( WebException webException )
            {
                var message = GetResponseMessage( webException.Response.GetResponseStream() );
                throw new Exception( webException.Message + " - " + message );
            }
            catch ( Exception ex )
            {
                throw new Exception( ex?.InnerException?.Message, ex );
            }

            return null;
        }

        /// <summary>
        /// Gets the response as an XDocument
        /// </summary>
        /// <param name="response">The response.</param>
        /// <returns></returns>
        private XDocument GetXmlResponse( IRestResponse response )
        {
            if ( response.StatusCode == HttpStatusCode.OK &&
                response.Content.Trim().Length > 0 &&
                response.Content.Contains( "<?xml" ) )
            {
                return XDocument.Parse( response.Content );
            }

            return null;
        }

        /// <summary>
        /// Gets the result code message.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        private string GetResultCodeMessage( Dictionary<string, string> result )
        {
            switch ( result.GetValueOrNull( "result-code" ).AsInteger() )
            {
                case 100:
                    {
                        return "Transaction was approved.";
                    }
                case 200:
                    {
                        return "Transaction was declined by processor.";
                    }
                case 201:
                    {
                        return "Do not honor.";
                    }
                case 202:
                    {
                        return "Insufficient funds.";
                    }
                case 203:
                    {
                        return "Over limit.";
                    }
                case 204:
                    {
                        return "Transaction not allowed.";
                    }
                case 220:
                    {
                        return "Incorrect payment information.";
                    }
                case 221:
                    {
                        return "No such card issuer.";
                    }
                case 222:
                    {
                        return "No card number on file with issuer.";
                    }
                case 223:
                    {
                        return "Expired card.";
                    }
                case 224:
                    {
                        return "Invalid expiration date.";
                    }
                case 225:
                    {
                        return "Invalid card security code.";
                    }
                case 240:
                    {
                        return "Call issuer for further information.";
                    }
                case 250: // pickup card
                case 251: // lost card
                case 252: // stolen card
                case 253: // fradulent card
                    {
                        // these are more sensitive declines so sanitize them a bit but provide a code for later lookup
                        return string.Format( "This card was declined (code: {0}).", result.GetValueOrNull( "result-code" ) );
                    }
                case 260:
                    {
                        return string.Format( "Declined with further instructions available. ({0})", result.GetValueOrNull( "result-text" ) );
                    }
                case 261:
                    {
                        return "Declined-Stop all recurring payments.";
                    }
                case 262:
                    {
                        return "Declined-Stop this recurring program.";
                    }
                case 263:
                    {
                        return "Declined-Update cardholder data available.";
                    }
                case 264:
                    {
                        return "Declined-Retry in a few days.";
                    }
                case 300:
                    {
                        return "Transaction was rejected by gateway.";
                    }
                case 400:
                    {
                        return "Transaction error returned by processor.";
                    }
                case 410:
                    {
                        return "Invalid merchant configuration.";
                    }
                case 411:
                    {
                        return "Merchant account is inactive.";
                    }
                case 420:
                    {
                        return "Communication error.";
                    }
                case 421:
                    {
                        return "Communication error with issuer.";
                    }
                case 430:
                    {
                        return "Duplicate transaction at processor.";
                    }
                case 440:
                    {
                        return "Processor format error.";
                    }
                case 441:
                    {
                        return "Invalid transaction information.";
                    }
                case 460:
                    {
                        return "Processor feature not available.";
                    }
                case 461:
                    {
                        return "Unsupported card type.";
                    }
            }

            return string.Empty;
        }

        /// <summary>
        /// Gets the response message.
        /// </summary>
        /// <param name="responseStream">The response stream.</param>
        /// <returns></returns>
        private string GetResponseMessage( Stream responseStream )
        {
            Stream receiveStream = responseStream;
            Encoding encode = System.Text.Encoding.GetEncoding( "utf-8" );
            StreamReader readStream = new StreamReader( receiveStream, encode );

            StringBuilder sb = new StringBuilder();
            Char[] read = new Char[8192];
            int count = 0;
            do
            {
                count = readStream.Read( read, 0, 8192 );
                String str = new String( read, 0, count );
                sb.Append( str );
            }
            while ( count > 0 );

            return sb.ToString();
        }

        private DateTime? ParseDateValue( string dateString )
        {
            if ( !string.IsNullOrWhiteSpace( dateString ) && dateString.Length >= 14 )
            {
                int year = dateString.Substring( 0, 4 ).AsInteger();
                int month = dateString.Substring( 4, 2 ).AsInteger();
                int day = dateString.Substring( 6, 2 ).AsInteger();
                int hour = dateString.Substring( 8, 2 ).AsInteger();
                int min = dateString.Substring( 10, 2 ).AsInteger();
                int sec = dateString.Substring( 12, 2 ).AsInteger();

                return new DateTime( year, month, day, hour, min, sec );
            }

            return DateTime.MinValue;
        }

        #endregion
    }

    public class TithelyResponse
    {
        private string Status;
        private string Account_Id;
        private string Type;
        private object JSON;
    }

    public class TithelyAccount
    {
        private string Account_Id;
        private string Type;
        private string Email;
        private string First_Name;
        private string Last_Name;
        private string Phone_Number;
    }

    public class TithelyPayment
    {
        public string AccountId;
        public string PIN;
        public int AmountCents;
        public string City;
        public string Country;
        public string Email;
        public string FirstName;
        public string GivingType;
        public string LastName;
        public string Memo;
        public string OrganizationId;
        public string PaymentTerm;
        public string PaymentToken;
        public string PhoneNumber;
        public string Postal;
        public int StartDate;
        public string State;
        public string StreetAddress;
    }

    /// <summary>
    /// Provides utility methods to parse query strings
    /// </summary>
    public class Utility
    {
        /// <summary>
        /// Parses the query string.
        /// </summary>
        /// <param name="queryString">The query string.</param>
        /// <returns></returns>
        public static Dictionary<string, string> ParseQueryString( string queryString )
        {
            Dictionary<string, string> result = queryString.Split( '&' )
                .Select( x => x.Split( '=' ) )
                .Where( x => x.Length == 2 )
                .ToDictionary( x => x.First(), x => WebUtility.UrlDecode( x.Last() ) );

            if ( !result.Any() )
            {
                return null;
            }

            return result;
        }

        /// <summary>
        /// Creates the query string.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public static string CreateQueryString( Dictionary<string, string> parameters )
        {
            var queryString = string.Empty;
            if ( parameters.Any() )
            {
                queryString = parameters
                    .Select( p => string.Format( "{0}={1}", p.Key, WebUtility.UrlEncode( p.Value ).Replace( "+", "%20" ) ) )
                    .ToList()
                    .AsDelimited( "&" );
            }

            return queryString;
        }
    }
}
