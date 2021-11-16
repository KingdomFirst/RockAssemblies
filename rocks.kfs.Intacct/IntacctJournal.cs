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
using rocks.kfs.Intacct.Utils;
using KFSConst = rocks.kfs.Intacct.SystemGuid;

namespace rocks.kfs.Intacct
{
    public class IntacctJournal
    {
        /// <summary>
        /// Creates the XML to submit to Intacct for a new Journal
        /// </summary>
        /// <param name="AuthCreds">The IntacctAuth object with authentication. <see cref="IntacctAuth"/></param>
        /// <param name="Batch">The Rock FinancialBatch that a Journal Entry will be created from. <see cref="FinancialBatch"/></param>
        /// <param name="JournalId">The Intacct Symbol of the Journal that the Entry should be posted to. For example: GJ</param>
        /// <returns>Returns the XML needed to create an Intacct Journal Entry.</returns>
        public XmlDocument CreateJournalEntryXML( IntacctAuth AuthCreds, int BatchId, string JournalId, ref string debugLava, string DescriptionLava = "" )
        {
            var doc = new XmlDocument();
            var financialBatch = new FinancialBatchService( new RockContext() ).Get( BatchId );

            if ( financialBatch.Id > 0 )
            {
                var lines = GetGlEntries( financialBatch, ref debugLava, DescriptionLava );
                if ( lines.Any() )
                {
                    var batchDate = financialBatch.BatchStartDateTime == null ? RockDateTime.Now.ToShortDateString() : ( ( System.DateTime ) financialBatch.BatchStartDateTime ).ToShortDateString();

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
                        writer.WriteStartElement( "create" );
                        writer.WriteStartElement( "GLBATCH" );
                        writer.WriteElementString( "JOURNAL", JournalId );
                        writer.WriteElementString( "REFERENCENO", financialBatch.Id.ToString() );
                        writer.WriteElementString( "BATCH_DATE", batchDate );
                        writer.WriteElementString( "BATCH_TITLE", financialBatch.Name );
                        writer.WriteElementString( "HISTORY_COMMENT", "Imported from RockRMS" );
                        writer.WriteStartElement( "ENTRIES" );

                        if ( lines.Count > 0 )
                        {
                            foreach ( var line in lines )
                            {
                                writer.WriteStartElement( "GLENTRY" );
                                writer.WriteElementString( "DOCUMENT", line.DocumentNumber ?? string.Empty );
                                writer.WriteElementString( "ACCOUNTNO", line.GlAccountNumber ?? string.Empty );
                                decimal? transactionAmount = line.TransactionAmount;
                                decimal num = new decimal();
                                if ( ( transactionAmount.GetValueOrDefault() < num ? !transactionAmount.HasValue : true ) )
                                {
                                    writer.WriteElementString( "TR_TYPE", "1" );
                                    writer.WriteElementString( "TRX_AMOUNT", line.TransactionAmount.ToString() );
                                }
                                else
                                {
                                    writer.WriteElementString( "TR_TYPE", "-1" );
                                    writer.WriteElementString( "TRX_AMOUNT", new decimal?( Math.Abs( line.TransactionAmount.Value ) ).ToString() );
                                }
                                writer.WriteElementString( "CURRENCY", line.TransactionCurrency ?? string.Empty );
                                if ( line.ExchangeRateDate.HasValue )
                                {
                                    writer.WriteElementString( "EXCH_RATE_DATE", ( ( DateTime ) line.ExchangeRateDate ).ToShortDateString() );
                                }
                                if ( !string.IsNullOrWhiteSpace( line.ExchangeRateType ) )
                                {
                                    writer.WriteElementString( "EXCH_RATE_TYPE_ID", line.ExchangeRateType );
                                }
                                else if ( line.ExchangeRateValue.HasValue )
                                {
                                    writer.WriteElementString( "EXCHANGE_RATE", line.ExchangeRateValue.ToString() );
                                }
                                else if ( !string.IsNullOrWhiteSpace( line.TransactionCurrency ) )
                                {
                                    writer.WriteElementString( "EXCH_RATE_TYPE_ID", line.ExchangeRateType );
                                }
                                if ( string.IsNullOrWhiteSpace( line.AllocationId ) )
                                {
                                    writer.WriteElementString( "LOCATION", line.LocationId ?? string.Empty );
                                    writer.WriteElementString( "DEPARTMENT", line.DepartmentId ?? string.Empty );
                                    writer.WriteElementString( "PROJECTID", line.ProjectId ?? string.Empty );
                                    writer.WriteElementString( "CUSTOMERID", line.CustomerId ?? string.Empty );
                                    writer.WriteElementString( "VENDORID", line.VendorId ?? string.Empty );
                                    writer.WriteElementString( "EMPLOYEEID", line.EmployeeId ?? string.Empty );
                                    writer.WriteElementString( "ITEMID", line.ItemId ?? string.Empty );
                                    writer.WriteElementString( "CLASSID", line.ClassId ?? string.Empty );
                                    writer.WriteElementString( "CONTRACTID", line.ContractId ?? string.Empty );
                                    writer.WriteElementString( "WAREHOUSEID", line.WarehouseId ?? string.Empty );
                                }
                                else
                                {
                                    writer.WriteElementString( "ALLOCATION", line.AllocationId );
                                    if ( line.AllocationId == "Custom" )
                                    {
                                        foreach ( var split in line.CustomAllocationSplits )
                                        {
                                            writer.WriteStartElement( "SPLIT" );
                                            writer.WriteElementString( "AMOUNT", split.Amount.ToString() );
                                            writer.WriteElementString( "LOCATIONID", split.LocationId ?? string.Empty );
                                            writer.WriteElementString( "DEPARTMENTID", split.DepartmentId ?? string.Empty );
                                            writer.WriteElementString( "PROJECTID", split.ProjectId ?? string.Empty );
                                            writer.WriteElementString( "CUSTOMERID", split.CustomerId ?? string.Empty );
                                            writer.WriteElementString( "VENDORID", split.VendorId ?? string.Empty );
                                            writer.WriteElementString( "EMPLOYEEID", split.EmployeeId ?? string.Empty );
                                            writer.WriteElementString( "ITEMID", split.ItemId ?? string.Empty );
                                            writer.WriteElementString( "CLASSID", split.ClassId ?? string.Empty );
                                            writer.WriteElementString( "CONTRACTID", split.ContractId ?? string.Empty );
                                            writer.WriteElementString( "WAREHOUSEID", split.WarehouseId ?? string.Empty );
                                            writer.WriteEndElement();
                                        }
                                    }
                                }
                                writer.WriteElementString( "DESCRIPTION", line.Memo );
                                if ( line.CustomFields.Count > 0 )
                                {
                                    foreach ( KeyValuePair<string, dynamic> customField in line.CustomFields )
                                    {
                                        writer.WriteElementString( customField.Key, customField.Value ?? string.Empty );
                                    }
                                }
                                writer.WriteEndElement();
                            }
                        }

                        writer.WriteEndElement();  // close ENTRIES
                        writer.WriteEndElement();  // close GLBATCH
                        writer.WriteEndElement();  // close create
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

        private List<JournalEntryLine> GetGlEntries( FinancialBatch financialBatch, ref string debugLava, string DescriptionLava = "" )
        {
            if ( string.IsNullOrWhiteSpace( DescriptionLava ) )
            {
                DescriptionLava = "{{ Batch.Id }}: {{ Batch.Name }}";
            }

            var rockContext = new RockContext();

            //
            // Group/Sum Transactions by Account and Project since Project can come from Account or Transaction Details
            //
            List<RegistrationInstance> registrationLinks;
            List<GroupMember> groupMemberLinks;
            var batchTransactionsSummary = TransactionHelpers.GetTransactionSummary( financialBatch, rockContext, out registrationLinks, out groupMemberLinks );

            //
            // Get the Dimensions from the Account since the Transaction Details have been Grouped already
            //
            var customDimensions = TransactionHelpers.GetCustomDimensions();
            var batchSummary = new List<GLBatchTotals>();
            foreach ( var summary in batchTransactionsSummary )
            {
                var account = new FinancialAccountService( rockContext ).Get( summary.FinancialAccountId );
                var customDimensionValues = new Dictionary<string, dynamic>();
                account.LoadAttributes();
                var mergeFieldObjects = new MergeFieldObjects
                {
                    Account = account,
                    Batch = financialBatch,
                    Registrations = registrationLinks,
                    GroupMembers = groupMemberLinks,
                    Summary = summary,
                    CustomDimensions = customDimensions
                };
                Dictionary<string, object> mergeFields = TransactionHelpers.GetMergeFieldsAndDimensions(  ref debugLava, customDimensionValues, mergeFieldObjects );

                var batchSummaryItem = new GLBatchTotals()
                {
                    Amount = summary.Amount,
                    CreditAccount = account.GetAttributeValue( "rocks.kfs.Intacct.ACCOUNTNO" ),
                    DebitAccount = account.GetAttributeValue( "rocks.kfs.Intacct.DEBITACCOUNTNO" ),
                    TransactionFeeAmount = summary.TransactionFeeAmount,
                    TransactionFeeAccount = summary.TransactionFeeAccount,
                    Class = account.GetAttributeValue( "rocks.kfs.Intacct.CLASSID" ),
                    Department = account.GetAttributeValue( "rocks.kfs.Intacct.DEPARTMENT" ),
                    Location = account.GetAttributeValue( "rocks.kfs.Intacct.LOCATION" ),
                    Project = summary.Project,
                    Description = DescriptionLava.ResolveMergeFields( mergeFields ),
                    CustomDimensions = customDimensionValues,
                    ProcessTransactionFees = summary.ProcessTransactionFees
                };

                batchSummary.Add( batchSummaryItem );
            }

            return GenerateLineItems( batchSummary );
        }

        private List<JournalEntryLine> GenerateLineItems( List<GLBatchTotals> transactionItems )
        {
            var returnList = new List<JournalEntryLine>();
            foreach ( var transaction in transactionItems )
            {
                var processTransactionFees = 0;
                if ( transaction.ProcessTransactionFees > 0 && !string.IsNullOrWhiteSpace( transaction.TransactionFeeAccount ) && transaction.TransactionFeeAmount > 0 )
                {
                    processTransactionFees = transaction.ProcessTransactionFees;
                }
                var creditLine = new JournalEntryLine()
                {
                    GlAccountNumber = transaction.CreditAccount,
                    TransactionAmount = transaction.Amount * -1,
                    ClassId = transaction.Class,
                    DepartmentId = transaction.Department,
                    LocationId = transaction.Location,
                    ProjectId = transaction.Project,
                    Memo = transaction.Description,
                    CustomFields = transaction.CustomDimensions
                };

                returnList.Add( creditLine );

                var debitLine = new JournalEntryLine()
                {
                    GlAccountNumber = transaction.DebitAccount,
                    TransactionAmount = processTransactionFees == 1 ? transaction.Amount - transaction.TransactionFeeAmount : transaction.Amount,
                    ClassId = transaction.Class,
                    DepartmentId = transaction.Department,
                    LocationId = transaction.Location,
                    ProjectId = transaction.Project,
                    Memo = transaction.Description,
                    CustomFields = transaction.CustomDimensions
                };

                returnList.Add( debitLine );

                if ( processTransactionFees == 2 )
                {
                    var feeCreditLine = new JournalEntryLine()
                    {
                        GlAccountNumber = transaction.DebitAccount,  // Credit the Bank Account (DebitAccount)
                        TransactionAmount = transaction.TransactionFeeAmount * -1,
                        ClassId = transaction.Class,
                        DepartmentId = transaction.Department,
                        LocationId = transaction.Location,
                        ProjectId = transaction.Project,
                        Memo = transaction.Description + " Transaction Fees",
                        CustomFields = transaction.CustomDimensions
                    };

                    returnList.Add( feeCreditLine );
                }

                if ( processTransactionFees > 0 )
                {
                    var feeDebitLine = new JournalEntryLine()
                    {
                        GlAccountNumber = transaction.TransactionFeeAccount,
                        TransactionAmount = transaction.TransactionFeeAmount,
                        ClassId = transaction.Class,
                        DepartmentId = transaction.Department,
                        LocationId = transaction.Location,
                        ProjectId = transaction.Project,
                        Memo = transaction.Description + " Transaction Fees",
                        CustomFields = transaction.CustomDimensions
                    };

                    returnList.Add( feeDebitLine );
                }
            }

            return returnList;
        }
    }
}
