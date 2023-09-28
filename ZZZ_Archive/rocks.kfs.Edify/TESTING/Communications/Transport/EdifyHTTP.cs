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
using System.IO;
using System.Linq;
using PostalServerDotNet.v1.Model.Object;
using Rock;
using Rock.Attribute;
using Rock.Communication;
using Rock.Communication.Transport;
using Rock.Data;
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
    [DefinedValueField( "Base URL 2",
        Description = "The API URL provided by Edify.",
        DefinedTypeGuid = SystemGuid.BASE_URL_DEFINED_TYPE,
        DisplayDescription = true,
        IsRequired = false,
        Order = 2,
        Key = AttributeKey.BaseUrl2 )]
    [TextField( "API Key 2",
        Description = "The API Key provided by Edify API Credentials.",
        IsRequired = false,
        Order = 3,
        Key = AttributeKey.ApiKey2 )]
    [IntegerField( "Threshold",
        Description = "The recipient count threshold for sending to server 2. If under this amount messages will be sent using Base URL 2 and API Key 2, if provided.",
        DefaultIntegerValue = 10,
        IsRequired = true,
        Order = 4,
        Key = AttributeKey.Threshold )]
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

            /// <summary>
            /// The API key for a different server
            /// </summary>
            public const string ApiKey2 = "APIKey2";

            /// <summary>
            /// The base URL
            /// </summary>
            public const string BaseUrl2 = "BaseURL2";

            /// <summary>
            /// The threshold for sending emails to a different server.
            /// </summary>
            public const string Threshold = "Threshold";

        }

        /// <summary>
        /// Send the Edify specific email. 
        /// </summary>
        /// <param name="rockEmailMessage">The rock email message.</param>
        /// <returns></returns>
        protected override EmailSendResponse SendEmail( RockEmailMessage rockEmailMessage )
        {
            PostalServerDotNet.v1.Client client = null;
            var apiKey1 = GetAttributeValue( AttributeKey.ApiKey );
            var apiKey2 = GetAttributeValue( AttributeKey.ApiKey2 );

            var selectedBaseUrlGuid = GetAttributeValue( AttributeKey.BaseUrl ).AsGuid();
            var selectedBaseUrl2Guid = GetAttributeValue( AttributeKey.BaseUrl2 ).AsGuid();
            var definedValueBaseUrl = DefinedValueCache.Get( selectedBaseUrlGuid );
            var definedValueBase2Url = DefinedValueCache.Get( selectedBaseUrl2Guid );

            var thresholdCount = GetAttributeValue( AttributeKey.Threshold ).AsInteger();
            var edifyCountKey = string.Format( "EdifyRecipientCount{0}", rockEmailMessage.Subject );

            var recipientCount = RockCache.Get( edifyCountKey )?.ToString().AsInteger();
            if ( ( recipientCount == null || recipientCount <= 0 ) && rockEmailMessage.MessageMetaData != null && rockEmailMessage.MessageMetaData.Count > 0 )
            {
                using ( var rockContext = new RockContext() )
                {
                    var communicationRecipientService = new CommunicationRecipientService( rockContext );
                    var communicationRecipient = communicationRecipientService.Get( rockEmailMessage.MessageMetaData["communication_recipient_guid"].AsGuid() );
                    if ( communicationRecipient != null )
                    {
                        recipientCount = communicationRecipient.Communication.Recipients.Count();
                        RockCache.AddOrUpdate( edifyCountKey, recipientCount );
                    }
                }
            }

            if ( apiKey2.IsNotNullOrWhiteSpace() && definedValueBase2Url != null && recipientCount < thresholdCount )
            {
                client = new PostalServerDotNet.v1.Client( $"https://{definedValueBase2Url.Value}.edify.press/api/v1", GetAttributeValue( AttributeKey.ApiKey2 ) );
            }
            else
            {
                client = new PostalServerDotNet.v1.Client( $"https://{definedValueBaseUrl.Value}.edify.press/api/v1", GetAttributeValue( AttributeKey.ApiKey ) );
            }

            var toEmailList = new List<string>();
            var ccEmailList = new List<string>();
            var bccEmailList = new List<string>();
            var attachments = new List<MessageAttachment>();

            string sender = rockEmailMessage.FromEmail;
            if ( rockEmailMessage.FromName.IsNotNullOrWhiteSpace() && rockEmailMessage.FromName != rockEmailMessage.FromEmail )
            {
                sender = $"{rockEmailMessage.FromName} <{rockEmailMessage.FromEmail}>";
            }
            string replyTo = null;
            string tag = null;

            if ( rockEmailMessage.ReplyToEmail.IsNotNullOrWhiteSpace() )
            {
                replyTo = rockEmailMessage.ReplyToEmail;
            }

            var toEmail = rockEmailMessage.GetRecipients();
            toEmail
                .Where( r => r.To.Trim() != string.Empty )
                .ToList()
                .ForEach( r => toEmailList.Add( r.To.Trim() ) );

            ccEmailList = rockEmailMessage
                .CCEmails
                .Where( cc => cc.Trim() != string.Empty )
                .Where( cc => !toEmailList.Contains( cc ) )
                .ToList();

            bccEmailList = rockEmailMessage
                .BCCEmails
                .Where( bcc => bcc.Trim() != string.Empty )
                .Where( bcc => !toEmailList.Contains( bcc ) )
                .Where( bcc => !ccEmailList.Contains( bcc ) )
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
                if ( toEmailList.Any( e => e.Trim() != "" ) || ccEmailList.Any( e => e.Trim() != "" ) || bccEmailList.Any( e => e.Trim() != "" ) )
                {
                    // Future enhancement possibility to convert Rock HTML Message to a readable Plain Text Message
                    var response = client.SendMessage( sender, toEmailList, ccEmailList, bccEmailList, sender, rockEmailMessage.Subject, tag, replyTo, ( !attachments.Any() ) ? rockEmailMessage.PlainTextMessage : "", rockEmailMessage.Message, attachments );
                    return new EmailSendResponse
                    {
                        Status = response.Status == "success" ? CommunicationRecipientStatus.Delivered : CommunicationRecipientStatus.Failed,
                        StatusNote = "Message Sent"
                    };
                }
                else
                {
                    throw new Exception( "No Recipients found for this message." );
                }
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