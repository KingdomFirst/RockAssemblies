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
using Rock;
using Rock.Attribute;
using Rock.Communication;
using Rock.Communication.Transport;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;
using SimpleTextingDotNet.v2;
using SimpleTextingDotNet.v2.Model.Response;

namespace rocks.kfs.SimpleTexting.Communications.Transport
{
    /// <summary>
    /// Used to send communication through Simple Texting's API.
    /// </summary>
    /// <seealso cref="TransportComponent" />
    [Description( "Sends a communication through Simple Texting API" )]
    [Export( typeof( TransportComponent ) )]
    [ExportMetadata( "ComponentName", "Simple Texting" )]
    [TextField( "API Key",
        Description = "The API Key provided by Simple Texting.",
        IsRequired = true,
        Order = 1,
        Key = AttributeKey.ApiKey )]
    public class SimpleTexting : TransportComponent, ISmsPipelineWebhook
    {
        /// <summary>
        /// Gets the sms pipeline webhook path that should be used by this transport.
        /// </summary>
        /// <value>
        /// The sms pipeline webhook path.
        /// </value>
        /// <note>
        /// This should be from the application root (https://www.kingdomfirstsolutions.com/).
        /// </note>
        public string SmsPipelineWebhookPath => "Plugins/rocks_kfs/Webhooks/SimpleTexting.ashx";

        /// <summary>
        /// Class for storing attribute keys.
        /// </summary>
        public class AttributeKey
        {
            /// <summary>
            /// The API key
            /// </summary>
            public const string ApiKey = "APIKey";
        }

        /// <summary>
        /// Sends the specified rock message.
        /// </summary>
        /// <param name="rockMessage">The rock message.</param>
        /// <param name="mediumEntityTypeId">The medium entity type identifier.</param>
        /// <param name="mediumAttributes">The medium attributes.</param>
        /// <param name="errorMessages">The error messages.</param>
        /// <returns></returns>
        public override bool Send( RockMessage rockMessage, int mediumEntityTypeId, Dictionary<string, string> mediumAttributes, out List<string> errorMessages )
        {
            errorMessages = new List<string>();

            var smsMessage = rockMessage as RockSMSMessage;
            if ( smsMessage != null )
            {
                var simpleTextClient = new Client( GetAttributeValue( AttributeKey.ApiKey ) );

                var mergeFields = Rock.Lava.LavaHelper.GetCommonMergeFields( null, rockMessage.CurrentPerson );
                foreach ( var mergeField in rockMessage.AdditionalMergeFields )
                {
                    mergeFields.AddOrReplace( mergeField.Key, mergeField.Value );
                }

                var attachmentUris = GetAttachmentMediaUrls( smsMessage.Attachments.AsQueryable() );
                foreach ( var recipient in smsMessage.GetRecipients() )
                {
                    try
                    {
                        foreach ( var mergeField in mergeFields )
                        {
                            recipient.MergeFields.AddOrIgnore( mergeField.Key, mergeField.Value );
                        }

                        CommunicationRecipient communicationRecipient = null;

                        using ( var rockContext = new RockContext() )
                        {
                            CommunicationRecipientService communicationRecipientService = new CommunicationRecipientService( rockContext );
                            int? recipientId = recipient.CommunicationRecipientId;
                            if ( recipientId.HasValue )
                            {
                                communicationRecipient = communicationRecipientService.Get( recipientId.Value );
                            }

                            string message = ResolveText( smsMessage.Message, smsMessage.CurrentPerson, communicationRecipient, smsMessage.EnabledLavaCommands, recipient.MergeFields, smsMessage.AppRoot, smsMessage.ThemeRoot );
                            Person recipientPerson = ( Person ) recipient.MergeFields.GetValueOrNull( "Person" );

                            if ( rockMessage.CreateCommunicationRecord && recipientPerson != null )
                            {
                                var communicationService = new CommunicationService( rockContext );

                                var createSMSCommunicationArgs = new CommunicationService.CreateSMSCommunicationArgs
                                {
                                    FromPerson = smsMessage.CurrentPerson,
                                    ToPersonAliasId = recipientPerson?.PrimaryAliasId,
                                    Message = message,
                                    FromPhone = smsMessage.FromNumber,
                                    CommunicationName = smsMessage.CommunicationName,
                                    ResponseCode = string.Empty,
                                    SystemCommunicationId = smsMessage.SystemCommunicationId
                                };

                                Communication communication = communicationService.CreateSMSCommunication( createSMSCommunicationArgs );

                                if ( attachmentUris.Any() )
                                {
                                    foreach ( var attachment in rockMessage.Attachments.AsQueryable() )
                                    {
                                        communication.AddAttachment( new CommunicationAttachment { BinaryFileId = attachment.Id }, CommunicationType.SMS );
                                    }
                                }

                                rockContext.SaveChanges();
                                Send( communication, mediumEntityTypeId, mediumAttributes );
                                continue;
                            }
                            else
                            {
                                Object response = simpleTextClient.SendMessage( recipient.To, message, accountPhone: smsMessage.FromNumber?.Value, mediaItems: attachmentUris );

                                var responseType = response.GetType();

                                if ( responseType != typeof( SendResponse ) )
                                {
                                    var errorResponse = ( ErrorResponse ) response;
                                    if ( errorResponse != null && errorResponse.Message.IsNotNullOrWhiteSpace() )
                                    {
                                        errorMessages.Add( errorResponse.Message );
                                    }

                                    var errorConflictResponse = ( ErrorConflictResponse ) response;
                                    if ( errorConflictResponse != null && errorConflictResponse.Message.IsNotNullOrWhiteSpace() )
                                    {
                                        errorMessages.Add( errorConflictResponse.Message );
                                    }
                                }

                                if ( communicationRecipient != null )
                                {
                                    rockContext.SaveChanges();
                                }
                            }
                        }
                    }
                    catch ( Exception ex )
                    {
                        errorMessages.Add( ex.Message );
                        ExceptionLogService.LogException( ex );
                    }
                }
            }

            return !errorMessages.Any();
        }

        /// <summary>
        /// Sends the specified communication.
        /// </summary>
        /// <param name="communication">The communication.</param>
        /// <param name="mediumEntityTypeId">The medium entity type identifier.</param>
        /// <param name="mediumAttributes">The medium attributes.</param>
        public override void Send( Communication communication, int mediumEntityTypeId, Dictionary<string, string> mediumAttributes )
        {
            using ( var communicationRockContext = new RockContext() )
            {
                communication = new CommunicationService( communicationRockContext ).Get( communication.Id );

                bool hasPendingRecipients;
                if ( communication != null &&
                    communication.Status == Rock.Model.CommunicationStatus.Approved &&
                    ( !communication.FutureSendDateTime.HasValue || communication.FutureSendDateTime.Value.CompareTo( RockDateTime.Now ) <= 0 ) )
                {
                    var qryRecipients = new CommunicationRecipientService( communicationRockContext ).Queryable();
                    hasPendingRecipients = qryRecipients
                        .Where( r =>
                            r.CommunicationId == communication.Id &&
                            r.Status == CommunicationRecipientStatus.Pending &&
                            r.MediumEntityTypeId.HasValue &&
                            r.MediumEntityTypeId.Value == mediumEntityTypeId )
                        .Any();
                }
                else
                {
                    hasPendingRecipients = false;
                }

                if ( hasPendingRecipients )
                {
                    var currentPerson = communication.CreatedByPersonAlias?.Person;
                    var globalAttributes = GlobalAttributesCache.Get();
                    string publicAppRoot = globalAttributes.GetValue( "PublicApplicationRoot" );
                    var mergeFields = Rock.Lava.LavaHelper.GetCommonMergeFields( null, currentPerson );

                    string fromPhone = communication.SMSFromDefinedValue?.Value;

                    var personEntityTypeId = EntityTypeCache.Get( "Rock.Model.Person" ).Id;
                    var communicationEntityTypeId = EntityTypeCache.Get( "Rock.Model.Communication" ).Id;
                    var communicationCategoryId = CategoryCache.Get( Rock.SystemGuid.Category.HISTORY_PERSON_COMMUNICATIONS.AsGuid(), communicationRockContext ).Id;

                    var simpleTextingClient = new Client( GetAttributeValue( AttributeKey.ApiKey ) );

                    var smsAttachmentsBinaryFileIdList = communication.GetAttachmentBinaryFileIds( CommunicationType.SMS );
                    List<string> attachmentMediaUrls = new List<string>();
                    if ( smsAttachmentsBinaryFileIdList.Any() )
                    {
                        attachmentMediaUrls = this.GetAttachmentMediaUrls( new BinaryFileService( communicationRockContext ).GetByIds( smsAttachmentsBinaryFileIdList ) );
                    }

                    bool recipientFound = true;
                    while ( recipientFound )
                    {
                        var recipientRockContext = new RockContext();
                        var recipient = Rock.Model.Communication.GetNextPending( communication.Id, mediumEntityTypeId, recipientRockContext );
                        if ( recipient != null )
                        {
                            if ( ValidRecipient( recipient, communication.IsBulkCommunication ) )
                            {
                                try
                                {
                                    var toNumber = recipient.PersonAlias.Person.PhoneNumbers.GetFirstSmsNumber();
                                    if ( !string.IsNullOrWhiteSpace( toNumber ) )
                                    {
                                        var mergeObjects = recipient.CommunicationMergeValues( mergeFields );

                                        string message = ResolveText( communication.SMSMessage, currentPerson, recipient, communication.EnabledLavaCommands, mergeObjects, publicAppRoot );

                                        var response = simpleTextingClient.SendMessage( toNumber, message, accountPhone: fromPhone, mediaItems: attachmentMediaUrls );

                                        recipient.Status = CommunicationRecipientStatus.Delivered;
                                        recipient.SendDateTime = RockDateTime.Now;
                                        recipient.TransportEntityTypeName = this.GetType().FullName;
                                        recipient.UniqueMessageId = response.Id;

                                        try
                                        {
                                            var historyService = new HistoryService( recipientRockContext );
                                            historyService.Add( new History
                                            {
                                                CreatedByPersonAliasId = communication.SenderPersonAliasId,
                                                EntityTypeId = personEntityTypeId,
                                                CategoryId = communicationCategoryId,
                                                EntityId = recipient.PersonAlias.PersonId,
                                                Verb = History.HistoryVerb.Sent.ConvertToString().ToUpper(),
                                                ChangeType = History.HistoryChangeType.Record.ToString(),
                                                ValueName = "SMS message",
                                                Caption = message.Truncate( 200 ),
                                                RelatedEntityTypeId = communicationEntityTypeId,
                                                RelatedEntityId = communication.Id
                                            } );
                                        }
                                        catch ( Exception ex )
                                        {
                                            ExceptionLogService.LogException( ex, null );
                                        }

                                    }
                                    else
                                    {
                                        recipient.Status = CommunicationRecipientStatus.Failed;
                                        recipient.StatusNote = "No Phone Number with Messaging Enabled";
                                    }
                                }
                                catch ( Exception ex )
                                {
                                    recipient.Status = CommunicationRecipientStatus.Failed;
                                    recipient.StatusNote = "Simple Texting Exception: " + ex.Message;
                                }
                            }

                            recipientRockContext.SaveChanges();

                        }
                        else
                        {
                            recipientFound = false;
                        }
                    }
                }
            }
        }

        #region private shared methods

        /// <summary>
        /// Gets the attachment media urls.
        /// </summary>
        /// <param name="attachments">The attachments.</param>
        /// <returns></returns>
        private List<string> GetAttachmentMediaUrls( IQueryable<BinaryFile> attachments )
        {
            var binaryFilesInfo = attachments.Select( a => new
            {
                a.Id,
                a.MimeType
            } ).ToList();

            List<string> attachmentMediaUrls = new List<string>();
            if ( binaryFilesInfo.Any() )
            {
                string publicAppRoot = GlobalAttributesCache.Get().GetValue( "PublicApplicationRoot" );
                attachmentMediaUrls = binaryFilesInfo.Select( b =>
                {
                    if ( b.MimeType.StartsWith( "image/", StringComparison.OrdinalIgnoreCase ) )
                    {
                        return $"{publicAppRoot}GetImage.ashx?id={b.Id}";
                    }
                    else
                    {
                        return $"{publicAppRoot}GetFile.ashx?id={b.Id}";
                    }
                } ).ToList();
            }

            return attachmentMediaUrls;
        }

        #endregion
    }
}