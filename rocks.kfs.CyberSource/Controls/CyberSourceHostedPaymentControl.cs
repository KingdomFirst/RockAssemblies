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
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Rock;
using Rock.Model;
using Rock.Web.UI;
using Rock.Web.UI.Controls;
using Rock.Web.Cache;

using CyberSource.Api;
using CyberSource.Model;
using CyberSourceSDK = CyberSource;

namespace rocks.kfs.CyberSource.Controls
{
    /// <summary>
    /// Control for hosting the CyberSource Gateway Payment Info HTML and scripts
    /// </summary>
    /// <seealso cref="System.Web.UI.WebControls.CompositeControl" />
    /// <seealso cref="System.Web.UI.INamingContainer" />
    public class CyberSourceHostedPaymentControl : CompositeControl,
        INamingContainer,
        Rock.Financial.IHostedGatewayPaymentControlTokenEvent
    {

        #region Controls

        private HiddenFieldWithClass _hfPaymentInfoToken;
        private HiddenFieldWithClass _hfCollectJSRawResponse;

        private HtmlGenericControl _divCreditCardNumber;
        private HtmlGenericControl _divCreditCardBreak;
        private MonthYearPicker _mypCreditCardExp;
        private HtmlGenericControl _divCreditCardCVV;
        private HtmlGenericControl _divValidationMessage;

        private Panel _gatewayCreditCardContainer;

        #endregion

        private FinancialGateway _cyberSourceGateway;

        #region Rock.Financial.IHostedGatewayPaymentControlTokenEvent

        /// <summary>
        /// Occurs when a payment token is received from the hosted gateway
        /// </summary>
        public event EventHandler<Rock.Financial.HostedGatewayPaymentControlTokenEventArgs> TokenReceived;

        #endregion Rock.Financial.IHostedGatewayPaymentControlTokenEvent

        /// <summary>
        /// Gets or sets the CyberSource gateway.
        /// </summary>
        /// <value>
        /// The CyberSource gateway.
        /// </value>
        public FinancialGateway CyberSourceGateway
        {
            private get
            {
                return _cyberSourceGateway;
            }

            set
            {
                _cyberSourceGateway = value;
            }
        }

        /// <summary>
        /// Gets the payment information token.
        /// </summary>
        /// <value>
        /// The payment information token.
        /// </value>
        public string PaymentInfoToken
        {
            get
            {
                EnsureChildControls();
                return _hfPaymentInfoToken.Value;
            }
        }

        /// <summary>
        /// Gets the payment information token raw.
        /// </summary>
        /// <value>
        /// The payment information token raw.
        /// </value>
        public string PaymentInfoTokenRaw
        {
            get
            {
                EnsureChildControls();
                return _hfCollectJSRawResponse.Value;
            }
        }

        public string MicroformJS = @"
   // the capture context that was requested server-side for this transaction
    var captureContext = ""{0}"";

    // custom styles that will be applied to each field we create using Microform
    var myStyles = {{
        //'input': {{
        //    'font-size': '14px',
        //    'font-family': 'helvetica, tahoma, calibri, sans-serif',
        //    'color': '#555'
        //}},
        //':focus': {{ 'color': 'blue' }},
        ':disabled': {{ 'cursor': 'not-allowed' }},
        'valid': {{ 'color': '#3c763d' }},
        'invalid': {{ 'color': '#a94442' }}
    }};

    // setup
    var flex = new Flex(captureContext);
    var microform = flex.microform({{ styles: myStyles }});

    function initCyberSourceMicroFormFields() {{
        if ($('.cybersource-payment-inputs .js-credit-card-input').length == 0) {{
            // control hasn't been rendered so skip
            return;
        }}


        var number = microform.createField('number', {{ placeholder: '0000 0000 0000 0000' }});
        var securityCode = microform.createField('securityCode', {{ placeholder: 'CVV' }});

        number.load('.cybersource-payment-inputs .js-credit-card-input');
        securityCode.load('.cybersource-payment-inputs .js-credit-card-cvv-input');
    }}

    function submitCyberSourceMicroFormInfo() {{
        var tokenizerPostbackScript = $('#{1}').attr('data-tokenizer-postback-script');
        var flexResponse = $('#{2}');
        var expMonth = $('.cybersource-payment-inputs .js-monthyear-month');
        var expYear = $('.cybersource-payment-inputs .js-monthyear-year');
        var $errorsOutput = $('.js-validation-message');

        var options = {{
            expirationMonth: ('00'+expMonth.val()).slice(-2),
            expirationYear: expYear.val()
        }};

        microform.createToken(options, function (err, token) {{
            if (err) {{
                // handle error
                console.error(err);
                $errorsOutput.text(err.message)
                $errorsOutput.parent().show();
            }} else {{
                // At this point you may pass the token back to your server as you wish.
                // In this example we append a hidden input to the form and submit it.
                console.log(JSON.stringify(token));
                flexResponse.val(token);

                if (tokenizerPostbackScript) {{
                    window.location = tokenizerPostbackScript;
                }}
            }}
        }});
    }}
//# sourceURL=CyberSourceMicroformJS.js
";
        private string microformJWK = "{\"kid\":\"HKJHKJ\"}";

        private void InitializeCyberSourceAPI()
        {
            List<String> targetOrigins = new List<String>()
            {
                GlobalAttributesCache.Get().GetValue( "PublicApplicationRoot" ).ReplaceIfEndsWith("/",""),
                GlobalAttributesCache.Get().GetValue( "InternalApplicationRoot" ).ReplaceIfEndsWith("/","")
            };

            List<String> allowedCardNetworks = new List<String>()
            {
                "VISA",
                "MAESTRO",
                "MASTERCARD",
                "AMEX",
                "DISCOVER",
                "DINERSCLUB",
                "JCB",
                "CUP",
                "CARTESBANCAIRES"
            };

            string clientVersion = "v2.0";

            var requestObj = new GenerateCaptureContextRequest(
                TargetOrigins: targetOrigins,
                AllowedCardNetworks: allowedCardNetworks,
                ClientVersion: clientVersion
            );

            try
            {
                var configDictionary = new Configuration().GetConfiguration( _cyberSourceGateway );
                var clientConfig = new CyberSourceSDK.Client.Configuration( merchConfigDictObj: configDictionary );

                var apiInstance = new MicroformIntegrationApi( clientConfig );
                String result = apiInstance.GenerateCaptureContext( requestObj );

                microformJWK = result;
            }
            catch ( Exception e )
            {
                ExceptionLogService.LogException( "Exception on calling the CyberSource API : " + e.Message );
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnInit( EventArgs e )
        {

            RockPage.AddScriptSrcToHead( this.Page, "MicroformJSV2", $"https://testflex.cybersource.com/microform/bundle/v2/flex-microform.min.js" );

            InitializeCyberSourceAPI();

            base.OnInit( e );
        }

        /// <summary>
        /// Writes the <see cref="T:System.Web.UI.WebControls.CompositeControl" /> content to the specified <see cref="T:System.Web.UI.HtmlTextWriter" /> object, for display on the client.
        /// </summary>
        /// <param name="writer">An <see cref="T:System.Web.UI.HtmlTextWriter" /> that represents the output stream to render HTML content on the client.</param>
        protected override void Render( HtmlTextWriter writer )
        {
            var updatePanel = this.ParentUpdatePanel();
            string postbackControlId;
            if ( updatePanel != null )
            {
                postbackControlId = updatePanel.ClientID;
            }
            else
            {
                postbackControlId = this.ID;
            }

            if ( TokenReceived != null )
            {
                this.Attributes["data-tokenizer-postback-script"] = $"javascript:__doPostBack('{postbackControlId}', '{this.ID}=TokenizerPostback')";
            }

            base.Render( writer );
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnLoad( EventArgs e )
        {
            base.OnLoad( e );

            if ( !Page.IsPostBack )
            {
                ScriptManager.RegisterClientScriptBlock( this, this.GetType(), "microformJSBlock", string.Format( MicroformJS, microformJWK, this.ClientID, _hfPaymentInfoToken.ClientID ), true );
            }

            ScriptManager.RegisterStartupScript( this, this.GetType(), "microformJSStartup", "initCyberSourceMicroFormFields();", true );

            if ( this.Page.IsPostBack )
            {
                HandleCustomPostbackEvents();
            }
        }

        /// <summary>
        /// Handles the custom postback events.
        /// </summary>
        private void HandleCustomPostbackEvents()
        {
            string[] eventArgs = ( this.Page.Request.Form["__EVENTARGUMENT"] ?? string.Empty ).Split( new[] { "=" }, StringSplitOptions.RemoveEmptyEntries );

            if ( eventArgs.Length < 2 )
            {
                // Nothing custom is in postback.
                return;
            }

            if ( eventArgs[0] != this.ID )
            {
                // Not from this control.
                return;
            }

            // The gatewayCollect script will pass back '{this.ID}=TokenizerPostback' in a postback. If so, we know this is a postback from that
            if ( eventArgs[1] == "TokenizerPostback" )
            {
                HandleTokenizerPostback();
            }
        }

        /// <summary>
        /// Handles the tokenizer postback.
        /// </summary>
        private void HandleTokenizerPostback()
        {
            Rock.Financial.HostedGatewayPaymentControlTokenEventArgs hostedGatewayPaymentControlTokenEventArgs = new Rock.Financial.HostedGatewayPaymentControlTokenEventArgs();

            if ( _hfPaymentInfoToken.Value.IsNullOrWhiteSpace() )
            {
                hostedGatewayPaymentControlTokenEventArgs.IsValid = false;
                hostedGatewayPaymentControlTokenEventArgs.ErrorMessage = _divValidationMessage.InnerText;
            }
            else
            {
                hostedGatewayPaymentControlTokenEventArgs.IsValid = true;
                hostedGatewayPaymentControlTokenEventArgs.ErrorMessage = null;
            }

            hostedGatewayPaymentControlTokenEventArgs.Token = _hfPaymentInfoToken.Value;

            TokenReceived?.Invoke( this, hostedGatewayPaymentControlTokenEventArgs );
        }

        /// <summary>
        /// Called by the ASP.NET page framework to notify server controls that use composition-based implementation to create any child controls they contain in preparation for posting back or rendering.
        /// </summary>
        protected override void CreateChildControls()
        {
            Controls.Clear();
            _hfPaymentInfoToken = new HiddenFieldWithClass() { ID = "_hfPaymentInfoToken", CssClass = "js-response-token" };
            Controls.Add( _hfPaymentInfoToken );

            _hfCollectJSRawResponse = new HiddenFieldWithClass() { ID = "_hfTokenizerRawResponse", CssClass = "js-tokenizer-raw-response" };
            Controls.Add( _hfCollectJSRawResponse );

            var pnlPaymentInputs = new Panel { ID = "pnlPaymentInputs", CssClass = "js-cybersource-payment-inputs cybersource-payment-inputs" };

            _gatewayCreditCardContainer = new Panel() { ID = "_gatewayCreditCardContainer", CssClass = "gateway-creditcard-container gateway-payment-container js-gateway-creditcard-container" };
            pnlPaymentInputs.Controls.Add( _gatewayCreditCardContainer );

            _divCreditCardNumber = new HtmlGenericControl( "div" );
            _divCreditCardNumber.AddCssClass( "form-control js-credit-card-input iframe-input credit-card-input" );
            _gatewayCreditCardContainer.Controls.Add( _divCreditCardNumber );

            _divCreditCardBreak = new HtmlGenericControl( "div" );
            _divCreditCardBreak.AddCssClass( "break" );
            _gatewayCreditCardContainer.Controls.Add( _divCreditCardBreak );

            _mypCreditCardExp = new MonthYearPicker();
            _mypCreditCardExp.MinimumYear = RockDateTime.Now.Year;
            _mypCreditCardExp.MaximumYear = _mypCreditCardExp.MinimumYear + 15;
            //_mypCreditCardExp.Label = "Expiration Date";
            _gatewayCreditCardContainer.Controls.Add( _mypCreditCardExp );

            _divCreditCardCVV = new HtmlGenericControl( "div" );
            _divCreditCardCVV.AddCssClass( "form-control js-credit-card-cvv-input iframe-input credit-card-cvv-input" );
            _gatewayCreditCardContainer.Controls.Add( _divCreditCardCVV );

            Controls.Add( pnlPaymentInputs );

            _divValidationMessage = new HtmlGenericControl( "div" );
            _divValidationMessage.AddCssClass( "alert alert-validation js-payment-input-validation" );
            _divValidationMessage.InnerHtml =
@"<span class='js-validation-message'></span>";
            _divValidationMessage.Style[HtmlTextWriterStyle.Display] = "none";
            this.Controls.Add( _divValidationMessage );
        }
    }
}