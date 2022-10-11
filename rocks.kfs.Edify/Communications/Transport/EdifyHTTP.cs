// <copyright>
// Copyright 2022 by Kingdom First Solutions
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
using System.IO;
using System.Linq;
using PostalServerDotNet.v1.Model.Object;
using Rock;
using Rock.Attribute;
using Rock.Communication;
using Rock.Communication.Transport;
using Rock.Model;
using Rock.Web.Cache;

namespace rocks.kfs.Edify.Communications.Transport
{
    /// <summary>
    /// Used to send communication through Edify's HTTP API.
    /// </summary>
    /// <seealso cref="Rock.Communication.EmailTransportComponent" />
    [Description( "Sends a communication through Edify's HTTP API" )]
    [Export( typeof( TransportComponent ) )]
    [ExportMetadata( "ComponentName", "Edify HTTP" )]
    [DefinedValueField( "Base URL",
        Description = "The API URL provided by Edify.",
        DefinedTypeGuid = SystemGuid.BASE_URL_DEFINED_TYPE,
        DisplayDescription = true,
        IsRequired = true,
        DefaultValue = SystemGuid.BASE_URL_SERVER1,
        Order = 0,
        Key = AttributeKey.BaseUrl )]
    [TextField( "API Key",
        Description = "The API Key provided by Edify API Credentials.",
        IsRequired = true,
        Order = 1,
        Key = AttributeKey.ApiKey )]
    public class EdifyHTTP : EmailTransportComponent
    {
        /// <summary>
        /// Class for storing attribute keys.
        /// </summary>
        public class AttributeKey
        {
            /// <summary>
            /// The API key
            /// </summary>
            public const string ApiKey = "APIKey";

            /// <summary>
            /// The base URL
            /// </summary>
            public const string BaseUrl = "BaseURL";
        }

        /// <summary>
        /// Send the Edify specific email. 
        /// </summary>
        /// <param name="rockEmailMessage">The rock email message.</param>
        /// <returns></returns>
        protected override EmailSendResponse SendEmail( RockEmailMessage rockEmailMessage )
        {
            var selectedBaseUrlGuid = GetAttributeValue( AttributeKey.BaseUrl ).AsGuid();
            var definedValueBaseUrl = DefinedValueCache.Get( selectedBaseUrlGuid );
            var baseUrl = $"https://{definedValueBaseUrl.Value}.edify.press/api/v1";
            var client = new PostalServerDotNet.v1.Client( baseUrl, GetAttributeValue( AttributeKey.ApiKey ) );

            var toEmailList = new List<string>();
            var ccEmailList = new List<string>();
            var bccEmailList = new List<string>();
            var attachments = new List<MessageAttachment>();

            string sender = null;
            string replyTo = null;
            string tag = null;

            if ( rockEmailMessage.ReplyToEmail.IsNotNullOrWhiteSpace() )
            {
                replyTo = rockEmailMessage.ReplyToEmail;
            }

            var toEmail = rockEmailMessage.GetRecipients();
            toEmail.ForEach( r => toEmailList.Add( r.To ) );

            var ccEmailAddresses = rockEmailMessage
                .CCEmails
                .Where( cc => cc != string.Empty )
                .Where( cc => !toEmail.Any( te => te.To == cc ) )
                .ToList();

            var bccEmailAddresses = rockEmailMessage
                .BCCEmails
                .Where( bcc => bcc != string.Empty )
                .Where( bcc => !toEmail.Any( te => te.To == bcc ) )
                .Where( bcc => !ccEmailAddresses.Contains( bcc ) )
                .ToList();

            // Tag Communication record for tracking opens & clicks
            if ( rockEmailMessage.MessageMetaData != null && rockEmailMessage.MessageMetaData.Count > 0 )
            {
                tag = rockEmailMessage.MessageMetaData.Join( "|" );
            }

            if ( rockEmailMessage.Attachments.Any() )
            {
                foreach ( var attachment in rockEmailMessage.Attachments )
                {
                    if ( attachment != null )
                    {
                        MessageAttachment ma = new MessageAttachment();
                        MemoryStream ms = new MemoryStream();
                        attachment.ContentStream.CopyTo( ms );
                        ma.Data = Convert.ToBase64String( ms.ToArray() );
                        ma.Name = attachment.FileName;
                        ma.Content_Type = attachment.MimeType;
                        attachments.Add( ma );
                    }
                }
            }

            try
            {
                var response = client.SendMessage( rockEmailMessage.FromEmail, toEmailList, ccEmailList, bccEmailList, sender, rockEmailMessage.Subject, tag, replyTo, rockEmailMessage.PlainTextMessage, rockEmailMessage.Message, attachments );
                return new EmailSendResponse
                {
                    Status = response.Status == "success" ? CommunicationRecipientStatus.Delivered : CommunicationRecipientStatus.Failed,
                    StatusNote = "Message Sent"
                };
            }
            catch ( Exception e )
            {
                if ( e.InnerException != null )
                {
                    throw e;
                }
                return new EmailSendResponse
                {
                    Status = CommunicationRecipientStatus.Failed,
                    StatusNote = e.Message
                };
            }
        }
    }
}