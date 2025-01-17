// <copyright>
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
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Rock;
using Rock.Model;
using Rock.Web.UI;
using Rock.Web.UI.Controls;

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
        ':disabled': {{ 'cursor': 'not-allowed' }},
        'valid': {{ 'color': '#3c763d' }},
        'invalid': {{ 'color': '#a94442' }}
    }};

    var cardIcons = {{
      visa: 'fab fa-cc-visa', 
      mastercard: 'fab fa-cc-mastercard', 
      amex: 'fab fa-cc-amex',
      discover: 'fab fa-cc-discover',
      dinersclub: 'fab fa-cc-diners-club',
      jcb: 'fab fa-cc-jcb'
    }};

    // setup
    var flex = new Flex(captureContext);
    var microform = flex.microform({{ styles: myStyles }});
    var number, securityCode;

    var loadCheckInterval = setInterval(checkCybersourceFieldsLoaded,1000);
    var flexTimeout = 900000;
    let flexTimeLoaded = new Date().getTime();

    function checkCybersourceFieldsLoaded(clearTimer = true) {{
        var currentTime = new Date().getTime();
        var timeDiff = currentTime - flexTimeLoaded;
        if (timeDiff >= flexTimeout) {{
            if (clearTimer) {{
                clearInterval(loadCheckInterval);
            }}

            var $errorsOutput = $('.js-validation-message');
            if ($errorsOutput.length == 0) {{
                $errorsOutput = $('<div class=""alert alert-warning mt-3""></div>').appendTo(($('.btn-give-now').length > 0) ? $('.btn-give-now').parent() : '.navigation.actions');
            }}

            $errorsOutput.text('We\'re sorry your session has timed out. Please reload the page to try again.');
            $errorsOutput.append('<p><a href=""javascript:location.reload();"" class=""btn btn-warning mt-3"" onclick=""Rock.controls.bootstrapButton.showLoading(this);"" data-loading-text=""Reloading..."">Reload Page</a></p>');
            $errorsOutput.parent().show();
            $('.btn-give-now, .js-submit-hostedpaymentinfo, .navigation.actions .btn').addClass('disabled').removeAttr('href');
        }}
    }}

    function initCyberSourceMicroFormFields() {{
        if ($('.cybersource-payment-inputs .js-credit-card-input').length == 0) {{
            // control hasn't been rendered so skip
            if (number != undefined && securityCode != undefined) {{
                number.unload();
                securityCode.unload();
            }}
            return;
        }}
        if (number == undefined && securityCode == undefined) {{
            number = microform.createField('number', {{ placeholder: '0000 0000 0000 0000' }});
            securityCode = microform.createField('securityCode');
        }}

        number.load('.cybersource-payment-inputs .js-credit-card-input');
        securityCode.load('.cybersource-payment-inputs .js-credit-card-cvv-input');

        number.on('error', function(data) {{
            var $errorsOutput = $('.js-validation-message');

            console.error(data);
            $errorsOutput.text(data.message);
            $errorsOutput.parent().show();
        }});

        var cardIcon = document.querySelector('#cardDisplay');
        var cardSecurityCodeLabel = document.querySelector('label.credit-card-cvv-label');

        number.on('change', function(data) {{
          if (data.card.length === 1) {{
            cardIcon.className = 'fa-2x ' + cardIcons[data.card[0].name];
            cardSecurityCodeLabel.textContent = data.card[0].securityCode.name;
          }} else {{
            cardIcon.className = 'fa-2x fas fa-credit-card';
          }}
        }});

        checkCybersourceFieldsLoaded(false);
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
                flexResponse.val(token);

                if (tokenizerPostbackScript) {{
                    window.location = tokenizerPostbackScript;
                }}
            }}
        }});
    }}
//# sourceURL=CyberSourceMicroformJS.js
";
        private string microformJWK = "{'kid':'HKJHKJ'}";

        private void InitializeCyberSourceAPI()
        {
            var microFormJsPath = "https://flex.cybersource.com/microform/bundle/v2/flex-microform.min.js";

            microFormJsPath = Configuration.GetMicroFormJWK( CyberSourceGateway, out microformJWK );

            RockPage.AddScriptSrcToHead( this.Page, "MicroformJSV2", microFormJsPath );
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnInit( EventArgs e )
        {
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

            var divFormGroupCC = new HtmlGenericControl( "div" );
            divFormGroupCC.AddCssClass( "form-group position-relative" );
            _gatewayCreditCardContainer.Controls.Add( divFormGroupCC );

            var lblCreditCardNumber = new HtmlGenericControl( "label" );
            lblCreditCardNumber.InnerText = "Card Number";
            lblCreditCardNumber.AddCssClass( "control-label" );
            divFormGroupCC.Controls.Add( lblCreditCardNumber );

            _divCreditCardNumber = new HtmlGenericControl( "div" );
            _divCreditCardNumber.ID = "divCC";
            _divCreditCardNumber.AddCssClass( "form-control js-credit-card-input iframe-input credit-card-input" );
            divFormGroupCC.Controls.Add( _divCreditCardNumber );

            var iconHtml = new HtmlGenericControl( "i" );
            iconHtml.ID = "cardDisplay";
            iconHtml.ClientIDMode = ClientIDMode.Static;
            iconHtml.EnableViewState = false;
            iconHtml.Attributes["style"] = "position: absolute; right: 10px; bottom: 3px;";
            divFormGroupCC.Controls.Add( iconHtml );

            _divCreditCardBreak = new HtmlGenericControl( "div" );
            _divCreditCardBreak.AddCssClass( "break" );
            _gatewayCreditCardContainer.Controls.Add( _divCreditCardBreak );

            var divRow = new HtmlGenericControl( "div" );
            divRow.AddCssClass( "row" );
            _gatewayCreditCardContainer.Controls.Add( divRow );

            var divRowCol1 = new HtmlGenericControl( "div" );
            divRowCol1.AddCssClass( "col-xs-6 exp-col" );
            divRow.Controls.Add( divRowCol1 );

            _mypCreditCardExp = new MonthYearPicker();
            _mypCreditCardExp.MinimumYear = RockDateTime.Now.Year;
            _mypCreditCardExp.MaximumYear = _mypCreditCardExp.MinimumYear + 15;
            _mypCreditCardExp.Label = "Expiration Date";
            divRowCol1.Controls.Add( _mypCreditCardExp );

            var divRowCol2 = new HtmlGenericControl( "div" );
            divRowCol2.AddCssClass( "col-xs-6 cvv-col" );
            divRow.Controls.Add( divRowCol2 );

            var divFormGroupCVV = new HtmlGenericControl( "div" );
            divFormGroupCVV.AddCssClass( "form-group" );
            divRowCol2.Controls.Add( divFormGroupCVV );

            var lblCreditCardCvv = new HtmlGenericControl( "label" );
            lblCreditCardCvv.InnerText = "Security Code";
            lblCreditCardCvv.AddCssClass( "control-label credit-card-cvv-label" );
            divFormGroupCVV.Controls.Add( lblCreditCardCvv );

            _divCreditCardCVV = new HtmlGenericControl( "div" );
            _divCreditCardCVV.ID = "divCVV";
            _divCreditCardCVV.AddCssClass( "form-control js-credit-card-cvv-input iframe-input credit-card-cvv-input input-width-sm" );
            divFormGroupCVV.Controls.Add( _divCreditCardCVV );

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