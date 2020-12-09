using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rock;
using Rock.Attribute;
using Rock.Data;
using Rock.Model;
using Rock.Security;
using Rock.Web.Cache;
using Rock.Workflow;

namespace rocks.kfs.Workflow.Action.Core
{
    #region Action Attributes

    [ActionCategory( "KFS: Core" )]
    [Description( "Creates a new signature request for the workflow." )]
    [Export( typeof( ActionComponent ) )]
    [ExportMetadata( "ComponentName", "Request Signature" )]

    #endregion

    #region Action Settings
    [CustomDropdownListField( "Signature Document Template", "The signature document type to send out when this action runs.", "SELECT Id AS Value, Name AS Text FROM SignatureDocumentTemplate ORDER BY Name", true, order: 0, key: AttributeKeys.DocumentTemplate )]
    [WorkflowAttribute( "Signature Document", "The attribute to store the created digital signature document in.", true, "", "", 1, AttributeKeys.SignatureDocument, new string[] { "Rock.Field.Types.FileFieldType" } )]
    [WorkflowAttribute( "Person", "The workflow attribute of the person submitting the signature request. ", true, "", "", 2, AttributeKeys.Person, new string[] { "Rock.Field.Types.PersonFieldType" } )]
    #endregion

    /// <summary>
    /// Creates a new prayer request.
    /// </summary>
    public class RequestSignature : ActionComponent
    {
        #region Attribute Keys

        private static class AttributeKeys
        {
            public const string SignatureDocument = "SignatureDocument";
            public const string DocumentTemplate = "SignatureDocumentTemplate";
            public const string Person = "Person";
        }

        #endregion
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

            var documentTemplateId = GetAttributeValue( action, AttributeKeys.DocumentTemplate ).AsIntegerOrNull();
            Person person = null;
            DigitalSignatureComponent DigitalSignatureComponent = null;

            // get person
            Guid? personAttributeGuid = GetAttributeValue( action, AttributeKeys.Person ).AsGuidOrNull();
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

            if ( person == null )
            {
                errorMessages.Add( "There is no person set on the attribute. Please try again." );
                return false;
            }

            if ( documentTemplateId.HasValue )
            {
                var documentTemplate = new SignatureDocumentTemplateService( rockContext ).Get( documentTemplateId.Value );
                if ( documentTemplate.ProviderEntityType != null )
                {
                    var provider = DigitalSignatureContainer.GetComponent( documentTemplate.ProviderEntityType.Name );
                    if ( provider != null && provider.IsActive )
                    {
                        DigitalSignatureComponent = provider;
                    }
                }

                if ( DigitalSignatureComponent != null )
                {
                    var sendDocumentTxn = new Rock.Transactions.SendDigitalSignatureRequestTransaction();
                    sendDocumentTxn.SignatureDocumentTemplateId = documentTemplateId.Value;
                    sendDocumentTxn.AppliesToPersonAliasId = person.PrimaryAliasId ?? 0;
                    sendDocumentTxn.AssignedToPersonAliasId = person.PrimaryAliasId ?? 0;
                    sendDocumentTxn.DocumentName = string.Format( "{0}_{1}", action.Activity.Workflow.Name, person.FullName.RemoveSpecialCharacters() );
                    sendDocumentTxn.Email = person.Email;
                    Rock.Transactions.RockQueue.TransactionQueue.Enqueue( sendDocumentTxn );
                    return true;
                }
                else
                {
                    errorMessages.Add( "There was an error loading the Digital Signature component, please check your document template." );
                    return false;
                }
            }
            else
            {
                errorMessages.Add( "There was no valid document template id set on this action" );
                return false;
            }
        }
    }
}
