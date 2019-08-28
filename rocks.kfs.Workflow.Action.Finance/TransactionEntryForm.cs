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
using System.Web;
using Rock;
using Rock.Attribute;
using Rock.Data;
using Rock.Model;
using Rock.Workflow;

namespace rocks.kfs.Workflow.Action.Finance
{
    #region Action Attributes

    [ActionCategory( "KFS: Finance" )]
    [Description( "Tests for a transaction id created with the transaction entry workflow form block." )]
    [Export( typeof( ActionComponent ) )]
    [ExportMetadata( "ComponentName", "Transaction Entry Form" )]

    #endregion

    #region Action Settings

    [WorkflowAttribute( "Account", "The workflow attribute that contains the financial account.",
        true, "", "", 0, ACCOUNT_KEY, new string[] { "Rock.Field.Types.AccountFieldType", "Rock.Field.Types.TextFieldType" } )]
    [WorkflowAttribute( "Amount", "The workflow attribute that contains the decimal amount to process for the transaction.",
        true, "", "", 1, AMOUNT_KEY, new string[] { "Rock.Field.Types.DecimalFieldType", "Rock.Field.Types.TextFieldType" } )]
    [LinkedPage( "Transaction Entry Page", "The page with the transaction entry workflow form block.",
        true, "", "", 2, TRANSACTION_ENTRY_PAGE_KEY )]
    [WorkflowAttribute( "Person Attribute", "The person for the transaction that will be processed.",
        true, "", "", 3, PERSON_ATTRIBUTE_KEY, new string[] { "Rock.Field.Types.PersonFieldType" } )]
    [WorkflowAttribute( "Use Saved Payment Accounts", "The workflow attribute that contains indicates if the gateway saved payment accounts for the provided person should be displayed. Default is false.",
        false, "", "", 4, USE_SAVED_PAYMENT_ACCOUNTS_KEY, new string[] { "Rock.Field.Types.BooleanFieldType", "Rock.Field.Types.TextFieldType" } )]
    [WorkflowAttribute( "Transaction Id", "The workflow attribute to store the completed transaction id.",
        true, "", "", 5, TRANSACTION_ID_KEY, new string[] { "Rock.Field.Types.IntegerFieldType", "Rock.Field.Types.TextFieldType" } )]

    #endregion

    /// <summary>
    /// Tests for a transaction id created with the transaction entry workflow form block.
    /// </summary>
    public class TransactionEntryForm : ActionComponent
    {
        public const string ACCOUNT_KEY = "rocks.kfs.Workflow.Action.Finance.Account";
        public const string AMOUNT_KEY = "rocks.kfs.Workflow.Action.Finance.Amount";
        public const string TRANSACTION_ENTRY_PAGE_KEY = "rocks.kfs.Workflow.Action.Finance.TransactionEntryPage";
        public const string PERSON_ATTRIBUTE_KEY = "rocks.kfs.Workflow.Action.Finance.PersonAttribute";
        public const string USE_SAVED_PAYMENT_ACCOUNTS_KEY = "rocks.kfs.Workflow.Action.Finance.UseSavedPaymentAccounts";
        public const string TRANSACTION_ID_KEY = "rocks.kfs.Workflow.Action.Finance.TransactionId";

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

            var transactionId = GetAttributeValue( action, TRANSACTION_ID_KEY, true ).AsIntegerOrNull();

            if ( !action.CompletedDateTime.HasValue && transactionId.HasValue )
            {
                return true;
            }

            if ( action.Activity.Workflow.Guid != null && HttpContext.Current != null )
            {
                var workflowGuid = action.Activity.Workflow.Guid;
                var pageParams = new Dictionary<string, string>();
                pageParams.Add( "WorkflowGuid", workflowGuid.ToString() );
                var transactionEntryPage = GetAttributeValue( action, TRANSACTION_ENTRY_PAGE_KEY, true );
                var pageReference = new Rock.Web.PageReference( transactionEntryPage, pageParams );
                var url = pageReference.BuildUrl();
                if ( !string.IsNullOrWhiteSpace( url ) )
                {
                    HttpContext.Current.Response.Redirect( url, false );
                }
            }
            else
            {
                errorMessages.Add( "The current workflow is not persisted." );
            }

            return false;
        }
    }
}
