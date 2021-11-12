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
using System.Data.Entity;
using System.Linq;
using Rock;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;
using KFSConst = rocks.kfs.Intacct.SystemGuid;

namespace rocks.kfs.Intacct.Utils
{
    public class TransactionHelpers
    {
        public static List<GLTransaction> GetTransactionSummary( FinancialBatch financialBatch, RockContext rockContext, out List<RegistrationInstance> registrationLinks, out List<GroupMember> groupMemberLinks )
        {
            //
            // Group/Sum Transactions by Debit/Bank Account and Project since Project can come from Account or Transaction Details
            //
            var batchTransactions = new List<GLTransaction>();
            registrationLinks = new List<RegistrationInstance>();
            groupMemberLinks = new List<GroupMember>();
            foreach ( var transaction in financialBatch.Transactions )
            {
                transaction.LoadAttributes();
                var gateway = transaction.FinancialGateway;
                var gatewayDefaultFeeAccount = string.Empty;
                var processTransactionFees = 0;
                if ( gateway != null )
                {
                    gateway.LoadAttributes();
                    gatewayDefaultFeeAccount = transaction.FinancialGateway.GetAttributeValue( "rocks.kfs.Intacct.DEFAULTFEEACCOUNTNO" );
                    var gatewayFeeProcessing = transaction.FinancialGateway.GetAttributeValue( "rocks.kfs.Intacct.FEEPROCESSING" ).AsIntegerOrNull();
                    if ( gatewayFeeProcessing != null )
                    {
                        processTransactionFees = gatewayFeeProcessing.Value;
                    }
                }

                foreach ( var transactionDetail in transaction.TransactionDetails )
                {
                    transactionDetail.LoadAttributes();
                    transactionDetail.Account.LoadAttributes();

                    var detailProject = transactionDetail.GetAttributeValue( "rocks.kfs.Intacct.PROJECTID" ).AsGuidOrNull();
                    var accountProject = transactionDetail.Account.GetAttributeValue( "rocks.kfs.Intacct.PROJECTID" ).AsGuidOrNull();
                    var transactionFeeAccount = transactionDetail.Account.GetAttributeValue( "rocks.kfs.Intacct.FEEACCOUNTNO" );

                    if ( string.IsNullOrWhiteSpace( transactionFeeAccount ) )
                    {
                        transactionFeeAccount = gatewayDefaultFeeAccount;
                    }

                    var projectCode = string.Empty;
                    if ( detailProject != null )
                    {
                        projectCode = DefinedValueCache.Get( ( Guid ) detailProject ).Value;
                    }
                    else if ( accountProject != null )
                    {
                        projectCode = DefinedValueCache.Get( ( Guid ) accountProject ).Value;
                    }

                    if ( transactionDetail.EntityTypeId.HasValue )
                    {
                        var registrationEntityType = EntityTypeCache.Get( typeof( Rock.Model.Registration ) );
                        var groupMemberEntityType = EntityTypeCache.Get( typeof( Rock.Model.GroupMember ) );

                        if ( transactionDetail.EntityId.HasValue && transactionDetail.EntityTypeId == registrationEntityType.Id )
                        {
                            foreach ( var registration in new RegistrationService( rockContext )
                                .Queryable().AsNoTracking()
                                .Where( r =>
                                    r.RegistrationInstance != null &&
                                    r.RegistrationInstance.RegistrationTemplate != null &&
                                    r.Id == transactionDetail.EntityId ) )
                            {
                                registrationLinks.Add( registration.RegistrationInstance );
                            }
                        }
                        if ( transactionDetail.EntityId.HasValue && transactionDetail.EntityTypeId == groupMemberEntityType.Id )
                        {
                            foreach ( var groupMember in new GroupMemberService( rockContext )
                                .Queryable().AsNoTracking()
                                .Where( gm =>
                                    gm.Group != null &&
                                    gm.Id == transactionDetail.EntityId ) )
                            {
                                groupMemberLinks.Add( groupMember );
                            }
                        }
                    }

                    var transactionItem = new GLTransaction()
                    {
                        Payer = transaction.AuthorizedPersonAlias.Person.FullName,
                        Amount = transactionDetail.Amount,
                        FinancialAccountId = transactionDetail.AccountId,
                        Project = projectCode,
                        TransactionFeeAmount = transactionDetail.FeeAmount != null && transactionDetail.FeeAmount.Value > 0 ? transactionDetail.FeeAmount.Value : 0.0M,
                        TransactionFeeAccount = transactionFeeAccount,
                        ProcessTransactionFees = processTransactionFees
                    };

                    batchTransactions.Add( transactionItem );
                }
            }

            var batchTransactionList = batchTransactions
            .GroupBy( d => new { d.FinancialAccountId, d.Project, d.TransactionFeeAccount, d.ProcessTransactionFees } )
            .Select( s => new GLTransaction
            {
                Payer = "Rock Import",
                FinancialAccountId = s.Key.FinancialAccountId,
                Project = s.Key.Project,
                Amount = s.Sum( f => ( decimal? ) f.Amount ) ?? 0.0M,
                TransactionFeeAmount = s.Sum( f => ( decimal? ) f.TransactionFeeAmount ) ?? 0.0M,
                TransactionFeeAccount = s.Key.TransactionFeeAccount,
                ProcessTransactionFees = s.Key.ProcessTransactionFees
            } )
            .ToList();

            return batchTransactionList;
        }

        public static List<string> GetCustomDimensions()
        {
            var knownDimensions = new List<string>();
            knownDimensions.Add( "rocks.kfs.Intacct.ACCOUNTNO" );
            knownDimensions.Add( "rocks.kfs.Intacct.DEBITACCOUNTNO" );
            knownDimensions.Add( "rocks.kfs.Intacct.FEEACCOUNTNO" );
            knownDimensions.Add( "rocks.kfs.Intacct.DEFAULTFEEACCOUNTNO" );
            knownDimensions.Add( "rocks.kfs.Intacct.FEEPROCESSING" );
            knownDimensions.Add( "rocks.kfs.Intacct.CLASSID" );
            knownDimensions.Add( "rocks.kfs.Intacct.DEPARTMENT" );
            knownDimensions.Add( "rocks.kfs.Intacct.LOCATION" );
            knownDimensions.Add( "rocks.kfs.Intacct.PROJECTID" );

            var rockContext = new RockContext();
            var accountCategoryId = new CategoryService( rockContext ).Queryable().FirstOrDefault( c => c.Guid.Equals( new System.Guid( KFSConst.Attribute.FINANCIAL_ACCOUNT_ATTRIBUTE_CATEGORY ) ) ).Id;
            var gatewayCategoryId = new CategoryService( rockContext ).Queryable().FirstOrDefault( c => c.Guid.Equals( new System.Guid( KFSConst.Attribute.FINANCIAL_GATEWAY_ATTRIBUTE_CATEGORY ) ) ).Id;
            var attributeService = new AttributeService( rockContext );
            var accountAttributes = attributeService.Queryable().Where( a => a.Categories.Select( c => c.Id ).Contains( accountCategoryId ) ).ToList();

            var customDimensions = new List<string>();

            foreach ( var attribute in accountAttributes )
            {
                if ( !knownDimensions.Contains( attribute.Key ) )
                {
                    customDimensions.Add( attribute.Key );
                }
            }

            return customDimensions;
        }

        public static Dictionary<string, object> GetMergeFieldsAndDimensions( ref string debugLava, Dictionary<string, dynamic> customDimensionValues, MergeFieldObjects mergeFieldObjects )
        {
            var mergeFields = new Dictionary<string, object>();
            var account = mergeFieldObjects.Account;
            account.LoadAttributes();

            if ( mergeFieldObjects.CustomDimensions.Count > 0 )
            {
                foreach ( var rockKey in mergeFieldObjects.CustomDimensions )
                {
                    var dimension = rockKey.Split( '.' ).Last();
                    customDimensionValues.Add( dimension, account.GetAttributeValue( rockKey ) );
                }
            }

            mergeFields.Add( "Account", account );
            mergeFields.Add( "Summary", mergeFieldObjects.Summary );
            mergeFields.Add( "Batch", mergeFieldObjects.Batch );
            mergeFields.Add( "Registrations", mergeFieldObjects.Registrations );
            mergeFields.Add( "GroupMembers", mergeFieldObjects.GroupMembers );
            mergeFields.Add( "CustomDimensions", customDimensionValues );

            if ( debugLava.Length < 6 && debugLava.AsBoolean() )
            {
                debugLava = mergeFields.lavaDebugInfo();
            }

            return mergeFields;
        }
    }

    public class MergeFieldObjects
    {
        public FinancialAccount Account { get; set; }
        public GLTransaction Summary { get; set; }
        public FinancialBatch Batch { get; set; }
        public List<RegistrationInstance> Registrations { get; set; }
        public List<GroupMember> GroupMembers { get; set; }
        public List<string> CustomDimensions { get; set; }
    }
}
