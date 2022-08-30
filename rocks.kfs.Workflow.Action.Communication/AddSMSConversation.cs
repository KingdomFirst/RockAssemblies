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
using System.Linq;

using Rock;
using Rock.Attribute;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;
using Rock.Workflow;

namespace rocks.kfs.Workflow.Action.Communication
{
    #region Action Attributes

    [ActionCategory( "KFS: Communication" )]
    [Description( "Creates a new SMS Conversation." )]
    [Export( typeof( ActionComponent ) )]
    [ExportMetadata( "ComponentName", "Add SMS Conversation" )]

    #endregion

    #region Action Settings

    [TextField( "Message in Conversation", "Message to display in conversation as though it was from the recipient. Default: Blank <span class='tip tip-lava'></span>", false, "" )]
    [BooleanField( "Mark as Read", "Flag indicating if the conversation should be marked as read when submitted. Default: false", defaultValue: false, order: 1 )]
    [WorkflowAttribute( "Person", "The workflow attribute of the person to be the recipient.", true, "", "", 7, null, new string[] { "Rock.Field.Types.PersonFieldType" } )]
    [DefinedValueField( Rock.SystemGuid.DefinedType.COMMUNICATION_SMS_FROM, "SMS From Number", "The SMS number the conversation will appear under.", false, false )]
    #endregion

    /// <summary>
    /// Creates a new prayer request.
    /// </summary>
    public class AddSMSConversation : ActionComponent
    {
        /// <summary>
        /// Executes the action.
        /// </summary>
        /// <param name="rockContext">The rock context.</param>
        /// <param name="action">The workflow action.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="errorMessages">The error messages.</param>
        /// <returns></returns>
        public override bool Execute(RockContext rockContext, WorkflowAction action, Object entity, out List<string> errorMessages)
        {
            errorMessages = new List<string>();

            Person person = null;
            int? conversationPersonAliasId = null;
            var personMessageKey = "";
            var isRead = GetAttributeValue( action, "MarkasRead" ).AsBoolean();
            var message = GetAttributeValue( action, "MessageinConversation" );
            var smsFromGuid = GetAttributeValue( action, "SMSFromNumber" ).AsGuidOrNull();

            DateTime dateRead;

            // get person
            Guid? personAttributeGuid = GetAttributeValue( action, "Person" ).AsGuidOrNull();
            if ( personAttributeGuid.HasValue )
            {
                Guid? personAliasGuid = action.GetWorkflowAttributeValue( personAttributeGuid.Value ).AsGuidOrNull();
                if ( personAliasGuid.HasValue )
                {
                    var personAlias = new PersonAliasService( rockContext ).Get( personAliasGuid.Value );
                    if ( personAlias != null )
                    {
                        person = personAlias.Person;
                    }
                }
            }

            // get name and campus info
            if ( person != null )
            {
                conversationPersonAliasId = person.PrimaryAliasId;
                personMessageKey = person.PhoneNumbers.GetFirstSmsNumber();

                if ( personMessageKey.IsNullOrWhiteSpace() )
                {
                    personMessageKey = person.PhoneNumbers.FirstOrDefault()?.Number;

                    if ( personMessageKey.IsNullOrWhiteSpace() )
                    {
                        personMessageKey = person.Email;
                    }
                }
            }
            else
            {
                // invalid conversation
                errorMessages.Add( "A valid person is required to use this action." );
                return true;
            }

            // mark as approved if needed
            if ( isRead )
            {
                dateRead = RockDateTime.Now;
            }
            var smsMediumEntityTypeId = EntityTypeCache.GetId( Rock.SystemGuid.EntityType.COMMUNICATION_MEDIUM_SMS ).Value;
            var smsDefinedValueId = DefinedValueCache.GetId( smsFromGuid ?? Guid.Empty );
            var smsTransport = new Rock.Communication.Medium.Sms().Transport.EntityType.Id;

            var mergeFields = GetMergeFields( action );

            // create the conversation
            var response = new CommunicationResponse
            {
                FromPersonAliasId = conversationPersonAliasId,
                MessageKey = personMessageKey.IsNullOrWhiteSpace() ? person.FullName : personMessageKey, // message key is required to have something when saved to the database, so if all else fails use the person's name.
                IsRead = isRead,
                CreatedDateTime = RockDateTime.Now,
                CreatedByPersonAliasId = conversationPersonAliasId,
                RelatedSmsFromDefinedValueId = smsDefinedValueId,
                RelatedTransportEntityTypeId = smsTransport,
                RelatedMediumEntityTypeId = smsMediumEntityTypeId,
                Response = message.ResolveMergeFields( mergeFields )
            };

            // add the SMS Conversation
            var communicationResponseService = new CommunicationResponseService( rockContext );
            communicationResponseService.Add( response );

            // save the Conversation
            rockContext.WrapTransaction( () =>
            {
                rockContext.SaveChanges();
            } );

            return true;
        }
    }
}
