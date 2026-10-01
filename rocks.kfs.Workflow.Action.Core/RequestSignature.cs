using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Data.Entity;
using System.Linq;

using Rock;
using Rock.Attribute;
using Rock.Communication;
using Rock.Data;
using Rock.Model;
using Rock.Web;
using Rock.Web.Cache;
using Rock.Web.UI.Controls;
using Rock.Workflow;

namespace rocks.kfs.Workflow.Action.Core
{
    #region Action Attributes

    [ActionCategory( "KFS: Core" )]
    [Description( "Emails a person a link to sign a document with Rock's electronic signatures. Place Rock's Electronic Signature action in an activity assigned to the signer; the link opens that activity on the Workflow Entry page." )]
    [Export( typeof( ActionComponent ) )]
    [ExportMetadata( "ComponentName", "Request Signature" )]

    #endregion

    #region Action Settings

    [CustomDropdownListField( "Signature Document Template", "The Rock electronic signature template the person will sign. Templates that use a legacy signature provider are not supported.", "SELECT Id AS Value, Name AS Text FROM SignatureDocumentTemplate WHERE ProviderEntityTypeId IS NULL ORDER BY Name", true, order: 0, key: AttributeKeys.DocumentTemplate )]
    [WorkflowAttribute( "Signature Document", "The attribute to store the signed document in. If the person has already signed this template, the document is stored here and no request is sent.", true, "", "", 1, AttributeKeys.SignatureDocument, new string[] { "Rock.Field.Types.FileFieldType" } )]
    [WorkflowAttribute( "Signer", "The workflow attribute of the person the signature request will go to.", true, "", "", 2, AttributeKeys.Person, new string[] { "Rock.Field.Types.PersonFieldType" } )]
    [LinkedPage( "Workflow Entry Page", "The page with a Workflow Entry block where the person signs. The link includes this workflow's type and Guid.", true, "", "", 3, AttributeKeys.WorkflowEntryPage )]
    /*
        9/30/2026 - CLAUDE

        The CodeEditorField constructor below is obsolete from Rock 18.1 in favor of the
        name-only constructor, but Rock 17 has only this one. Switch to the name-only form
        when Rock 17 support is dropped.

        Reason: Must compile on Rock 17 (source) and Rock 20 (target).
    */
    [TextField( "Email Subject","The subject of the signature request email. <span class='tip tip-lava'></span>", true, "Signature requested: {{ DocumentTemplate.Name }}", "", 4, AttributeKeys.EmailSubject )]
    [CodeEditorField( "Email Body", "The body of the signature request email. Merge fields: Person, Workflow, DocumentTemplate, SignatureUrl. <span class='tip tip-lava'></span>", CodeEditorMode.Lava, CodeEditorTheme.Rock, 300, true, DefaultEmailBody, "", 5, AttributeKeys.EmailBody )]

    #endregion

    /// <summary>
    /// Requests an electronic signature by emailing the signer a link to the workflow's
    /// Workflow Entry page, where Rock's Electronic Signature action collects it.
    /// </summary>
    public class RequestSignature : ActionComponent
    {
        #region Attribute Keys

        private static class AttributeKeys
        {
            public const string SignatureDocument = "SignatureDocument";
            public const string DocumentTemplate = "SignatureDocumentTemplate";
            public const string Person = "Person";
            public const string WorkflowEntryPage = "WorkflowEntryPage";
            public const string EmailSubject = "EmailSubject";
            public const string EmailBody = "EmailBody";
        }

        #endregion

        private const string DefaultEmailBody = @"{{ 'Global' | Attribute:'EmailHeader' }}
<p>{{ Person.NickName }},</p>
<p>Please review and sign <strong>{{ DocumentTemplate.Name }}</strong>.</p>
<p><a href=""{{ SignatureUrl }}&{{ Person | PersonTokenCreate:4320,1 }}"">Review and sign</a></p>
{{ 'Global' | Attribute:'EmailFooter' }}";

        /*
            9/30/2026 - CLAUDE

            Rewritten for Rock's built-in electronic signatures. The previous version sent
            requests through a legacy signature provider (e.g. SignNow) with
            DigitalSignatureContainer and SendDigitalSignatureRequestTransaction, which Rock
            20 removed. The signature itself is now collected by Rock's Electronic Signature
            action on the Workflow Entry page; this action only sends the request. Everything
            used here exists from Rock 17 through 20, so one version ships for all.

            Reason: Rock 20 removed legacy digital signature providers.
        */

        /// <summary>
        /// Executes the action.
        /// </summary>
        /// <param name="rockContext">The rock context.</param>
        /// <param name="action">The workflow action.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="errorMessages">The error messages.</param>
        /// <returns><c>true</c> when the request was sent or the document was already signed.</returns>
        public override bool Execute( RockContext rockContext, WorkflowAction action, Object entity, out List<string> errorMessages )
        {
            errorMessages = new List<string>();

            var person = GetSigner( rockContext, action );
            if ( person == null )
            {
                errorMessages.Add( "There is no person set on the attribute. Please try again." );
                return false;
            }

            if ( string.IsNullOrWhiteSpace( person.Email ) )
            {
                errorMessages.Add( "There is no valid email address set on the person." );
                return false;
            }

            var documentTemplateId = GetAttributeValue( action, AttributeKeys.DocumentTemplate ).AsIntegerOrNull();
            var documentTemplate = documentTemplateId.HasValue ? new SignatureDocumentTemplateService( rockContext ).Get( documentTemplateId.Value ) : null;
            if ( documentTemplate == null )
            {
                errorMessages.Add( "There was no valid document template id set on this action." );
                return false;
            }

            if ( documentTemplate.ProviderEntityTypeId.HasValue )
            {
                errorMessages.Add( $"The '{documentTemplate.Name}' template uses a legacy signature provider, which is no longer supported. Choose a template that uses Rock's electronic signatures." );
                return false;
            }

            // Already signed: store the document and move on without sending a request.
            var signedDocument = new SignatureDocumentService( rockContext )
                .Queryable().AsNoTracking()
                .Where( d =>
                    d.SignatureDocumentTemplateId == documentTemplate.Id &&
                    d.AppliesToPersonAlias != null &&
                    d.AppliesToPersonAlias.PersonId == person.Id &&
                    d.LastStatusDate.HasValue &&
                    d.Status == SignatureDocumentStatus.Signed &&
                    d.BinaryFile != null )
                .OrderByDescending( d => d.LastStatusDate.Value )
                .FirstOrDefault();

            if ( signedDocument != null )
            {
                return StoreSignedDocument( rockContext, action, signedDocument, errorMessages );
            }

            return SendRequest( action, person, documentTemplate, errorMessages );
        }

        private Person GetSigner( RockContext rockContext, WorkflowAction action )
        {
            var personAttributeGuid = GetAttributeValue( action, AttributeKeys.Person ).AsGuidOrNull();
            if ( !personAttributeGuid.HasValue )
            {
                return null;
            }

            var personAliasGuid = action.GetWorkflowAttributeValue( personAttributeGuid.Value ).AsGuidOrNull();
            if ( !personAliasGuid.HasValue )
            {
                return null;
            }

            return new PersonAliasService( rockContext ).Get( personAliasGuid.Value )?.Person;
        }

        private bool StoreSignedDocument( RockContext rockContext, WorkflowAction action, SignatureDocument signedDocument, List<string> errorMessages )
        {
            var signatureDocumentAttributeGuid = GetAttributeValue( action, AttributeKeys.SignatureDocument ).AsGuidOrNull();
            if ( !signatureDocumentAttributeGuid.HasValue )
            {
                errorMessages.Add( "Signature document attribute must be set." );
                return false;
            }

            var signatureDocumentAttribute = AttributeCache.Get( signatureDocumentAttributeGuid.Value, rockContext );
            if ( signatureDocumentAttribute == null )
            {
                errorMessages.Add( "Invalid signature document attribute set." );
                return false;
            }

            if ( signatureDocumentAttribute.FieldTypeId != FieldTypeCache.Get( Rock.SystemGuid.FieldType.FILE.AsGuid(), rockContext ).Id )
            {
                errorMessages.Add( "Invalid field type for signature document attribute set." );
                return false;
            }

            SetWorkflowAttributeValue( action, signatureDocumentAttributeGuid.Value, signedDocument.BinaryFile.Guid.ToString() );
            return true;
        }

        private bool SendRequest( WorkflowAction action, Person person, SignatureDocumentTemplate documentTemplate, List<string> errorMessages )
        {
            var workflow = action.Activity.Workflow;

            // The link reopens this workflow, so it must be saved to the database.
            if ( !workflow.IsPersisted && !( workflow.WorkflowTypeCache?.IsPersisted ?? false ) )
            {
                errorMessages.Add( "The workflow must be persisted so the signer can reopen it from the email link. Enable 'Automatically Persisted' on the workflow type, or persist it before this action." );
                return false;
            }

            var pageReference = new PageReference( GetAttributeValue( action, AttributeKeys.WorkflowEntryPage ), new Dictionary<string, string>
            {
                { "WorkflowTypeId", workflow.WorkflowTypeId.ToString() },
                { "WorkflowGuid", workflow.Guid.ToString() }
            } );

            if ( pageReference.PageId <= 0 )
            {
                errorMessages.Add( "A valid Workflow Entry Page must be set on this action." );
                return false;
            }

            var appRoot = GlobalAttributesCache.Get().GetValue( "PublicApplicationRoot" ).EnsureTrailingForwardslash();
            var signatureUrl = appRoot + pageReference.BuildUrl().TrimStart( '/' );

            var mergeFields = GetMergeFields( action );
            mergeFields.AddOrReplace( "Person", person );
            mergeFields.AddOrReplace( "DocumentTemplate", documentTemplate );
            mergeFields.AddOrReplace( "SignatureUrl", signatureUrl );

            var emailMessage = new RockEmailMessage
            {
                Subject = GetAttributeValue( action, AttributeKeys.EmailSubject ).ResolveMergeFields( mergeFields ),
                Message = GetAttributeValue( action, AttributeKeys.EmailBody ),
                AppRoot = appRoot,
                CreateCommunicationRecord = true
            };
            emailMessage.AddRecipient( new RockEmailMessageRecipient( person, mergeFields ) );

            if ( !emailMessage.Send( out var sendErrors ) )
            {
                errorMessages.AddRange( sendErrors );
                return false;
            }

            action.AddLogEntry( $"Signature request for '{documentTemplate.Name}' sent to {person.FullName}." );
            return true;
        }
    }
}
