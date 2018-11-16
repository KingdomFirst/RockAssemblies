using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using RestSharp;
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
    [CustomRadioListField( "Mode", "Mode to use for transactions", "Live,Test", true, "Test", "", 3 )]
    [DefinedTypeField( "Account Map", "Select the defined type used to specify the FinancialAccount mappings.", true, "80cdad25-a120-4a30-bb6a-21f1ccdb9e65", "", 4 )]
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
            var parameters = new Dictionary<string, string>
            {
                { "from_date", startDate.ToString( "yyyy-MM-dd" ) },
                { "end_date", endDate.ToString( "yyyy-MM-dd" ) }
            };

            var requestResult = PostRequest( gateway, "donations", parameters, out errorMessage );
            if ( requestResult != null )
            {
                var results = JsonConvert.DeserializeObject<List<ReachDonation>>( requestResult.ToString() );
                foreach ( var transaction in results )
                {
                    // only sync completed transactions
                    if ( transaction.status.Equals( "complete" ) )
                    {

                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Posts the request to the gateway endpoint.
        /// </summary>
        /// <param name="gateway">The gateway.</param>
        /// <param name="resource">The query resource.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <returns></returns>
        /// <exception cref="Exception">
        /// </exception>
        /// <exception cref="System.Exception"></exception>
        private object PostRequest( FinancialGateway gateway, string resource, Dictionary<string, string> parameters, out string errorMessage )
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
                baseUrl = string.Format( "https://{0}/{1}/{2}.json", baseUrl, ApiVersion, resource );
            }

            var restClient = new RestClient( baseUrl )
            {
                Authenticator = new HttpBasicAuthenticator( GetAttributeValue( gateway, "APIKey" ), GetAttributeValue( gateway, "APISecret" ) )
            };

            var restRequest = new RestRequest( Method.GET )
            {
                RequestFormat = DataFormat.Json
            };

            foreach ( var param in parameters )
            {
                restRequest.AddParameter( param.Key, param.Value );
            }

            try
            {
                var response = restClient.Execute( restRequest );
                if ( response != null && response.StatusCode == HttpStatusCode.OK )
                {
                    return response.Content;
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
        /// Gets the response message.
        /// </summary>
        /// <param name="responseStream">The response stream.</param>
        /// <returns></returns>
        private string GetResponseMessage( Stream responseStream )
        {
            var receiveStream = responseStream;
            var encode = Encoding.GetEncoding( "utf-8" );
            var readStream = new StreamReader( receiveStream, encode );

            var sb = new StringBuilder();
            var read = new Char[8192];
            var count = 0;
            do
            {
                count = readStream.Read( read, 0, 8192 );
                var str = new string( read, 0, count );
                sb.Append( str );
            }
            while ( count > 0 );

            return sb.ToString();
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

        #endregion
    }

    #region Object Declarations

    public class ReachDonation
    {
        public int account_id;
        public string address1;
        public string address2;
        public string admin_notes;
        public string alpha2_country_code;
        public decimal amount;
        public string ancestry;
        public string check_number;
        public string city;
        public string confirmation;
        public string country;
        public DateTime created_at;
        public string currency;
        public DateTime date;
        public string email;
        public string first_name;
        public int id;
        public string ip_address;
        public string last_name;
        public string name;
        public DateTime next_donation;
        public string note;
        public string payment_method;
        public string payment_type;
        public string paypal_profile_id;
        public string phone;
        public string post_body;
        public string postal;
        public string purpose;
        public bool recurring;
        public string recurring_period;
        public string referral_id;
        public string state;
        public string status;
        public int? supporter_id;
        public string token;
        public string transaction_token;
        public DateTime updated_at;
        public int? user_id;
    }

    #endregion
}
