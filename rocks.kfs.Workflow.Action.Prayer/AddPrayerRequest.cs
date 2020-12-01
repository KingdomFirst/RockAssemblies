// <copyright>
// Copyright 2019 by Kingdom First Solutions
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

namespace rocks.kfs.Workflow.Action.Prayer
{
    #region Action Attributes

    [ActionCategory( "KFS: Prayer" )]
    [Description( "Creates a new prayer request." )]
    [Export( typeof( ActionComponent ) )]
    [ExportMetadata( "ComponentName", "Add Prayer Request" )]

    #endregion

    #region Action Settings

    [IntegerField( "Expires After (Days)", "Number of days until the request will expire (only applies when auto-approved is enabled).", false, 14, "", 0, "ExpireDays" )]
    [BooleanField( "Approved", "Flag indicating if the request should be marked as approved when submitted.", order: 1 )]
    [WorkflowAttribute( "Request", "The workflow attribute to use for the request.", false, "", "", 2, null, new string[] { "Rock.Field.Types.MemoFieldType", "Rock.Field.Types.TextFieldType" } )]
    [WorkflowAttribute( "Category", "The workflow attribute to use for the Category.", false, "", "", 3, null, new string[] { "Rock.Field.Types.CategoryFieldType", "Rock.Field.Types.SelectSingleFieldType", "Rock.Field.Types.TextFieldType" } )]
    [WorkflowAttribute( "Allow Comments", "The workflow attribute to use to indicate if comments are allowed.", false, "", "", 4, null, new string[] { "Rock.Field.Types.BooleanFieldType" } )]
    [WorkflowAttribute( "Is Urgent", "The workflow attribute to use to indicate if the request is urgent.", false, "", "", 5, null, new string[] { "Rock.Field.Types.BooleanFieldType" } )]
    [WorkflowAttribute( "Is Public", "The workflow attribute to use to indicate if the request is public.", false, "", "", 6, null, new string[] { "Rock.Field.Types.BooleanFieldType" } )]
    [WorkflowAttribute( "Person", "The workflow attribute of the person submitting the prayer request. If a person will always be provided, the following attributes are not necessary: First Name, Last Name, Email, and Campus", true, "", "", 7, null, new string[] { "Rock.Field.Types.PersonFieldType" } )]
    [WorkflowAttribute( "First Name", "The workflow attribute to use for the First Name.", false, "", "", 8, null, new string[] { "Rock.Field.Types.TextFieldType" } )]
    [WorkflowAttribute( "Last Name", "The workflow attribute to use for the Last Name.", false, "", "", 9, null, new string[] { "Rock.Field.Types.TextFieldType" } )]
    [WorkflowAttribute( "Email", "The workflow attribute to use for the Email.", false, "", "", 10, null, new string[] { "Rock.Field.Types.EmailFieldType", "Rock.Field.Types.TextFieldType" } )]
    [WorkflowAttribute( "Campus", "The workflow attribute to use for the Campus.", false, "", "", 11, null, new string[] { "Rock.Field.Types.CampusFieldType" } )]
    [WorkflowAttribute( "Prayer Request", "The attribute to store the created prayer request guid.", true, "", "", 12, null, new string[] { "Rock.Field.Types.TextFieldType" } )]

    #endregion

    /// <summary>
    /// Creates a new prayer request.
    /// </summary>
    public class AddPrayerRequest : ActionComponent
    {
        /// <summary>
        /// Executes the action.
        /// </summary>
        /// <param name="rockContext">The rock context.</param>
        /// <param name="action">The workflow action.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="errorMessages">The error messages.</param>
        /// <returns></returns>
        public override bool Execute( RockContext rockContext, WorkflowAction action, Object entity, out List<string> errorMessages )
        {
            errorMessages = new List<string>();

            // stand up all the vars
            var expireDays = GetAttributeValue( action, "ExpireDays" ).AsDouble();
            int? categoryId = null;
            bool? allowComments = null;
            bool? isUrgent = null;
            bool? isPublic = null;
            Person person = null;
            int? requestorPersonAliasId = null;
            var requestFirstName = "Anonymous";
            var requestLastName = string.Empty;
            var requestEmail = string.Empty;
            int? requestCampusId = null;
            var requestText = string.Empty;
            var isApproved = GetAttributeValue( action, "Approved" ).AsBoolean();
            var expirationDate = RockDateTime.Now.AddDays( expireDays );
            DateTime? dateApproved = null;
            int? approvedPersonAliasId = null;

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
                requestorPersonAliasId = person.PrimaryAliasId;
                requestFirstName = ( !string.IsNullOrWhiteSpace( person.NickName ) ) ? person.NickName : person.LastName; // in case it was a business
                requestLastName = person.LastName;
                requestEmail = person.Email;
                if ( person.GetCampusIds().Any() )
                {
                    requestCampusId = person.GetCampusIds().FirstOrDefault();
                }
            }
            else
            {
                Guid? firstNameAttributeGuid = GetAttributeValue( action, "FirstName" ).AsGuidOrNull();
                if ( firstNameAttributeGuid.HasValue )
                {
                    var firstName = action.GetWorkflowAttributeValue( firstNameAttributeGuid.Value );
                    if ( !string.IsNullOrWhiteSpace( firstName ) )
                    {
                        requestFirstName = firstName;
                    }
                }

                Guid? lastNameAttributeGuid = GetAttributeValue( action, "LastName" ).AsGuidOrNull();
                if ( lastNameAttributeGuid.HasValue )
                {
                    var lastName = action.GetWorkflowAttributeValue( lastNameAttributeGuid.Value );
                    if ( !string.IsNullOrWhiteSpace( lastName ) )
                    {
                        requestLastName = lastName;
                    }
                }

                Guid? emailAttributeGuid = GetAttributeValue( action, "Email" ).AsGuidOrNull();
                if ( emailAttributeGuid.HasValue )
                {
                    var email = action.GetWorkflowAttributeValue( emailAttributeGuid.Value );
                    if ( !string.IsNullOrWhiteSpace( email ) )
                    {
                        requestEmail = email;
                    }
                }

                Guid? campusAttributeGuid = GetAttributeValue( action, "Campus" ).AsGuidOrNull();
                if ( campusAttributeGuid.HasValue )
                {
                    var campus = action.GetWorkflowAttributeValue( campusAttributeGuid.Value ).AsGuidOrNull();
                    if ( campus.HasValue )
                    {
                        requestCampusId = CampusCache.Get( campus.Value ).Id;
                    }
                }
            }

            // get prayer request text
            Guid? requestTextAttributeGuid = GetAttributeValue( action, "Request" ).AsGuidOrNull();
            if ( requestTextAttributeGuid.HasValue )
            {
                var requestAttributeText = action.GetWorkflowAttributeValue( requestTextAttributeGuid.Value );
                if ( !string.IsNullOrWhiteSpace( requestAttributeText ) )
                {
                    requestText = requestAttributeText;
                }
            }

            // get category
            Guid? cateogryAttributeGuid = GetAttributeValue( action, "Category" ).AsGuidOrNull();
            if ( cateogryAttributeGuid.HasValue )
            {
                var categoryGuid = action.GetWorkflowAttributeValue( cateogryAttributeGuid.Value ).AsGuidOrNull();
                if ( categoryGuid.HasValue )
                {
                    categoryId = CategoryCache.Get( categoryGuid.Value ).Id;
                }
            }

            // get allow comments
            Guid? allowCommentsAttributeGuid = GetAttributeValue( action, "AllowComments" ).AsGuidOrNull();
            if ( allowCommentsAttributeGuid.HasValue )
            {
                var commentsAllowed = action.GetWorkflowAttributeValue( allowCommentsAttributeGuid.Value ).AsBooleanOrNull();
                if ( commentsAllowed.HasValue )
                {
                    allowComments = commentsAllowed;
                }
            }

            // get is urgent
            Guid? isUrgentAttributeGuid = GetAttributeValue( action, "IsUrgent" ).AsGuidOrNull();
            if ( isUrgentAttributeGuid.HasValue )
            {
                var urgent = action.GetWorkflowAttributeValue( isUrgentAttributeGuid.Value ).AsBooleanOrNull();
                if ( urgent.HasValue )
                {
                    isUrgent = urgent;
                }
            }

            // get is public
            Guid? isPublicAttributeGuid = GetAttributeValue( action, "IsPublic" ).AsGuidOrNull();
            if ( isPublicAttributeGuid.HasValue )
            {
                var publicValue = action.GetWorkflowAttributeValue( isPublicAttributeGuid.Value ).AsBooleanOrNull();
                if ( publicValue.HasValue )
                {
                    isPublic = publicValue;
                }
            }

            // mark as approved if needed
            if ( isApproved )
            {
                dateApproved = RockDateTime.Now;
                approvedPersonAliasId = requestorPersonAliasId;
            }

            // test for possible errors and fail gracefully
            if ( string.IsNullOrWhiteSpace( requestFirstName ) )
            {
                errorMessages.Add( "A first name is required." );
                return true;
            }

            if ( string.IsNullOrWhiteSpace( requestText ) )
            {
                errorMessages.Add( "The prayer request text is required." );
                return true;
            }

            // create the request
            var request = new PrayerRequest
            {
                RequestedByPersonAliasId = requestorPersonAliasId,
                FirstName = requestFirstName,
                LastName = requestLastName,
                Email = requestEmail,
                CampusId = requestCampusId,
                IsActive = true,
                IsApproved = isApproved,
                Text = requestText,
                EnteredDateTime = RockDateTime.Now,
                ExpirationDate = expirationDate,
                CreatedDateTime = RockDateTime.Now,
                ApprovedOnDateTime = dateApproved,
                CreatedByPersonAliasId = requestorPersonAliasId,
                ApprovedByPersonAliasId = approvedPersonAliasId,
                CategoryId = categoryId,
                AllowComments = allowComments,
                IsUrgent = isUrgent,
                IsPublic = isPublic,
                Answer = string.Empty
            };

            // add the request
            var prayerRequestService = new PrayerRequestService( rockContext );
            prayerRequestService.Add( request );

            // save the request
            rockContext.WrapTransaction( () =>
            {
                rockContext.SaveChanges();
            } );

            if ( request.Guid != null )
            {
                // get the attribute to store the request guid
                var prayerRequestAttributeGuid = GetAttributeValue( action, "PrayerRequest" ).AsGuidOrNull();
                if ( prayerRequestAttributeGuid.HasValue )
                {
                    var prayerRequestAttribute = AttributeCache.Get( prayerRequestAttributeGuid.Value, rockContext );
                    if ( prayerRequestAttribute != null )
                    {
                        if ( prayerRequestAttribute.FieldTypeId == FieldTypeCache.Get( Rock.SystemGuid.FieldType.TEXT.AsGuid(), rockContext ).Id )
                        {
                            SetWorkflowAttributeValue( action, prayerRequestAttributeGuid.Value, request.Guid.ToString() );
                        }
                    }
                }
            }

            return true;
        }
    }
}
