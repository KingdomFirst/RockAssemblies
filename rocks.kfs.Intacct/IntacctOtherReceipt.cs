// <copyright>
// Copyright 2021 by Kingdom First Solutions
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
using System.Xml;

using Rock;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;
using rocks.kfs.Intacct.Enums;
using KFSConst = rocks.kfs.Intacct.SystemGuid;

namespace rocks.kfs.Intacct
{
    public class IntacctOtherReceipt
    {
        /// <summary>
        /// Creates the XML to submit to Intacct for a new Other Receipt entry.
        /// </summary>
        /// <param name="AuthCreds">The IntacctAuth object with authentication. <see cref="IntacctAuth"/></param>
        /// <param name="Batch">The Rock FinancialBatch that a Journal Entry will be created from. <see cref="FinancialBatch"/></param>
        /// <returns>Returns the XML needed to create an Intacct Other Receipt.</returns>
        public XmlDocument CreateOtherReceiptXML( IntacctAuth AuthCreds, int BatchId, ref string debugLava, PaymentMethod paymentMethod, string bankAccountId = null, string unDepGLAccountId = null , string DescriptionLava = "" )
        {
            var doc = new XmlDocument();
            var financialBatch = new FinancialBatchService( new RockContext() ).Get( BatchId );

            if ( financialBatch.Id > 0 )
            {
                var otherReceipt = BuildOtherReceipt( financialBatch, ref debugLava, paymentMethod, bankAccountId, DescriptionLava );
                if ( otherReceipt.ReceiptItems.Any() )
                {
                    using ( var writer = doc.CreateNavigator().AppendChild() )
                    {
                        writer.WriteStartDocument();
                        writer.WriteStartElement( "request" );
                        writer.WriteStartElement( "control" );
                        writer.WriteElementString( "senderid", AuthCreds.SenderId );
                        writer.WriteElementString( "password", AuthCreds.SenderPassword );
                        writer.WriteElementString( "controlid", $"RequestControl_{financialBatch.Id}" );
                        writer.WriteElementString( "uniqueid", "false" );
                        writer.WriteElementString( "dtdversion", "3.0" );
                        writer.WriteElementString( "includewhitespace", "false" );
                        writer.WriteEndElement();  // close control
                        writer.WriteStartElement( "operation" );
                        writer.WriteStartElement( "authentication" );
                        writer.WriteStartElement( "login" );
                        writer.WriteElementString( "userid", AuthCreds.UserId );
                        writer.WriteElementString( "companyid", AuthCreds.CompanyId );
                        writer.WriteElementString( "password", AuthCreds.UserPassword );
                        if ( !string.IsNullOrWhiteSpace( AuthCreds.LocationId ) )
                        {
                            writer.WriteElementString( "locationid", AuthCreds.LocationId );
                        }
                        writer.WriteEndElement();  // close login
                        writer.WriteEndElement();  // close authentication
                        writer.WriteStartElement( "content" );
                        writer.WriteStartElement( "function" );
                        writer.WriteAttributeString( "controlid", $"Batch_{financialBatch.Id}" );
                        writer.WriteStartElement( "record_otherreceipt" );
                        writer.WriteStartElement( "paymentdate" );
                        writer.WriteElementString( "year", otherReceipt.PaymentDate.Year.ToString() );
                        writer.WriteElementString( "month", otherReceipt.PaymentDate.Month.ToString() );
                        writer.WriteElementString( "day", otherReceipt.PaymentDate.Day.ToString() );
                        writer.WriteEndElement();  // close paymentdate
                        writer.WriteElementString( "payee", otherReceipt.Payer );
                        writer.WriteStartElement( "receiveddate" );
                        writer.WriteElementString( "year", otherReceipt.ReceivedDate.Year.ToString() );
                        writer.WriteElementString( "month", otherReceipt.ReceivedDate.Month.ToString() );
                        writer.WriteElementString( "day", otherReceipt.ReceivedDate.Day.ToString() );
                        writer.WriteEndElement();  // close receiveddate
                        writer.WriteElementString( "paymentmethod", otherReceipt.PaymentMethod.GetDescription() );
                        if ( !string.IsNullOrWhiteSpace( otherReceipt.BankAccountId ) )
                        {
                            writer.WriteElementString( "bankaccountid", otherReceipt.BankAccountId );
                        }
                        else if ( !string.IsNullOrWhiteSpace( otherReceipt.UnDepGLAccountNo ) )
                        {
                            writer.WriteElementString( "undepglaccountn", otherReceipt.UnDepGLAccountNo );
                        }
                        writer.WriteStartElement( "depositdate" );
                        writer.WriteElementString( "year", otherReceipt.DepositDate.Value.Year.ToString() );
                        writer.WriteElementString( "month", otherReceipt.DepositDate.Value.Month.ToString() );
                        writer.WriteElementString( "day", otherReceipt.DepositDate.Value.Day.ToString() );
                        writer.WriteEndElement();  // close depositdate
                        writer.WriteElementString( "refid", otherReceipt.RefId );
                        writer.WriteElementString( "currency", otherReceipt.Currency ?? string.Empty );
                        if ( otherReceipt.ExchRateDate.HasValue )
                        {
                            writer.WriteElementString( "exchratedate", ( ( DateTime ) otherReceipt.ExchRateDate ).ToShortDateString() );
                        }
                        if ( !string.IsNullOrWhiteSpace( otherReceipt.ExchRateType ) )
                        {
                            writer.WriteElementString( "exchratetype", otherReceipt.ExchRateType );
                        }
                        else if ( otherReceipt.ExchRate.HasValue )
                        {
                            writer.WriteElementString( "exchrate", otherReceipt.ExchRate.Value.ToString() );
                        }
                        else if ( !string.IsNullOrWhiteSpace( otherReceipt.Currency ) )
                        {
                            writer.WriteElementString( "exchratetype", otherReceipt.ExchRateType );
                        }
                        writer.WriteStartElement( "receiptitems" );

                        // Add Receipt Items
                        foreach ( var item in otherReceipt.ReceiptItems )
                        {
                            writer.WriteStartElement( "lineitem" );
                            writer.WriteElementString( "glaccountno", item.GlAccountNo ?? string.Empty );
                            writer.WriteElementString( "amount", item.Amount.ToString() );
                            writer.WriteElementString( "memo", item.Memo ?? string.Empty );
                            writer.WriteElementString( "locationid", item.LocationId ?? string.Empty );
                            writer.WriteElementString( "departmentid", item.DepartmentId ?? string.Empty );
                            writer.WriteElementString( "projectid", item.ProjectId ?? string.Empty );
                            writer.WriteElementString( "taskid", item.TaskId ?? string.Empty );
                            writer.WriteElementString( "customerid", item.CustomerId ?? string.Empty );
                            writer.WriteElementString( "vendorid", item.VendorId ?? string.Empty );
                            writer.WriteElementString( "employeeid", item.EmployeeId ?? string.Empty );
                            writer.WriteElementString( "itemid", item.ItemId ?? string.Empty );
                            writer.WriteElementString( "classid", item.ClassId ?? string.Empty );
                            if ( item.CustomFields.Count > 0 )
                            {
                                foreach ( KeyValuePair<string, dynamic> customField in item.CustomFields )
                                {
                                    writer.WriteElementString( customField.Key, customField.Value ?? string.Empty );
                                }
                            }
                            writer.WriteEndElement();  // close lineitem
                        }

                        writer.WriteEndElement();  // close receiptitems
                        writer.WriteEndElement();  // close record_otherreceipt
                        writer.WriteEndElement();  // close function
                        writer.WriteEndElement();  // close content
                        writer.WriteEndElement();  // close operation
                        writer.WriteEndElement();  // close request
                        writer.WriteEndDocument(); // close document
                    }
                }
            }

            XmlDeclaration xmldecl;
            xmldecl = doc.CreateXmlDeclaration( "1.0", null, null );
            xmldecl.Encoding = "UTF-8";
            xmldecl.Standalone = "yes";

            XmlElement root = doc.DocumentElement;
            doc.InsertBefore( xmldecl, root );

            return doc;
        }

        private OtherReceipt BuildOtherReceipt( FinancialBatch financialBatch, ref string debugLava, PaymentMethod paymentMethod, string bankAccountId = null, string unDepGLAccountId = null, string DescriptionLava = "" )
        {
            if ( string.IsNullOrWhiteSpace( DescriptionLava ) )
            {
                DescriptionLava = "{{ Batch.Id }}: {{ Batch.Name }}";
            }

            var rockContext = new RockContext();

            var batchDate = financialBatch.BatchStartDateTime == null ? RockDateTime.Now : ( ( System.DateTime ) financialBatch.BatchStartDateTime );
            var otherReceipt = new OtherReceipt
            {
                Payer = string.Format( "Rock Batch - {0} ({1})", financialBatch.Name, financialBatch.Id ),
                PaymentDate = batchDate,
                ReceivedDate = batchDate,
                PaymentMethod = paymentMethod,
                BankAccountId = bankAccountId,
                UnDepGLAccountNo = unDepGLAccountId,
                DepositDate = batchDate,
                Description = string.Format( "Imported From Rock Batch {0} ({1})", financialBatch.Name, financialBatch.Id ),
                RefId = financialBatch.Id.ToString()
            };

            //
            // Group/Sum Transactions by Debit/Bank Account and Project since Project can come from Account or Transaction Details
            //
            var batchTransactions = new List<OtherReceiptTransaction>();
            var registrationLinks = new List<RegistrationInstance>();
            var groupMemberLinks = new List<GroupMember>();
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

                    var transactionItem = new OtherReceiptTransaction()
                    {
                        TransactionId = transactionDetail.TransactionId,
                        Payer = transaction.AuthorizedPersonAlias.Person.FullName,
                        Amount = transactionDetail.Amount,
                        FinancialAccountId = transactionDetail.AccountId,
                        Project = projectCode,
                        PaymentDate = transaction.TransactionDateTime.Value,
                        TransactionFeeAmount = transactionDetail.FeeAmount != null && transactionDetail.FeeAmount.Value > 0 ? transactionDetail.FeeAmount.Value : 0.0M,
                        TransactionFeeAccount = transactionFeeAccount,
                        ProcessTransactionFees = processTransactionFees
                    };

                    batchTransactions.Add( transactionItem );
                }
            }
            
            var receiptTransactions = batchTransactions
            .GroupBy( d => new { d.FinancialAccountId, d.Project, d.TransactionFeeAccount, d.ProcessTransactionFees } )
            .Select( s => new OtherReceiptTransaction
            {
                Payer = "Rock Import",
                TransactionId = -1,
                FinancialAccountId = s.Key.FinancialAccountId,
                Project = s.Key.Project,
                Amount = s.Sum( f => ( decimal? ) f.Amount ) ?? 0.0M,
                TransactionFeeAmount = s.Sum( f => ( decimal? ) f.TransactionFeeAmount ) ?? 0.0M,
                TransactionFeeAccount = s.Key.TransactionFeeAccount,
                ProcessTransactionFees = s.Key.ProcessTransactionFees
            } )
            .ToList();

            //
            // Get the Dimensions from the Account since the Transaction Details have been Grouped already
            //
            var customDimensions = GetCustomDimensions();

            // Create Receipt Item for each entry within a grouping
            foreach ( var bTran in receiptTransactions )
            {
                var account = new FinancialAccountService( rockContext ).Get( bTran.FinancialAccountId );
                var customDimensionValues = new Dictionary<string, dynamic>();
                var mergeFields = new Dictionary<string, object>();
                account.LoadAttributes();

                if ( customDimensions.Count > 0 )
                {
                    foreach ( var rockKey in customDimensions )
                    {
                        var dimension = rockKey.Split( '.' ).Last();
                        customDimensionValues.Add( dimension, account.GetAttributeValue( rockKey ) );
                    }
                }

                mergeFields.Add( "Account", account );
                mergeFields.Add( "Batch", financialBatch );
                mergeFields.Add( "Registrations", registrationLinks );
                mergeFields.Add( "GroupMembers", groupMemberLinks );
                mergeFields.Add( "CustomDimensions", customDimensionValues );

                if ( debugLava.Length < 6 && debugLava.AsBoolean() )
                {
                    debugLava = mergeFields.lavaDebugInfo();
                }

                var bankAccount = account.GetAttributeValue( "rocks.kfs.Intacct.DEBITACCOUNTNO" );
                var glAccount = account.GetAttributeValue( "rocks.kfs.Intacct.ACCOUNTNO" );
                var classId = account.GetAttributeValue( "rocks.kfs.Intacct.CLASSID" );
                var departmentId = account.GetAttributeValue( "rocks.kfs.Intacct.DEPARTMENT" );
                var locationId = account.GetAttributeValue( "rocks.kfs.Intacct.LOCATION" );

                var receiptItem = new ReceiptLineItem
                {
                    GlAccountNo = glAccount,
                    Amount = bTran.ProcessTransactionFees == 1 ? bTran.Amount - bTran.TransactionFeeAmount : bTran.Amount,
                    Memo = DescriptionLava.ResolveMergeFields( mergeFields ),
                    LocationId = locationId,
                    DepartmentId = departmentId,
                    ProjectId = bTran.Project,
                    ClassId = classId
                };
                otherReceipt.ReceiptItems.Add( receiptItem );

                if ( bTran.ProcessTransactionFees == 2 )
                {
                    var feeLineItem = new ReceiptLineItem
                    {
                        GlAccountNo = bTran.TransactionFeeAccount,
                        Amount = bTran.TransactionFeeAmount * -1,
                        Memo = "Transaction Fee",
                        LocationId = locationId,
                        DepartmentId = departmentId,
                        ProjectId = bTran.Project,
                        ClassId = classId
                    };
                    otherReceipt.ReceiptItems.Add( feeLineItem );
                }
            }

            return otherReceipt;
        }

        private List<string> GetCustomDimensions()
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
    }
}
