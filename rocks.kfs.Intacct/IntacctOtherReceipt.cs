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
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using Rock;
using Rock.Data;
using Rock.Model;
using rocks.kfs.Intacct.Enums;
using rocks.kfs.Intacct.Utils;

namespace rocks.kfs.Intacct
{
    public class IntacctOtherReceipt
    {
        /// <summary>
        /// Creates the XML to submit to Intacct for a new Other Receipt entry.
        /// </summary>
        /// <param name="AuthCreds">The IntacctAuth object with authentication. <see cref="IntacctAuth"/></param>
        /// <param name="BatchId">The BatchId of the Rock FinancialBatch that a Journal Entry will be created from.</param>
        /// <param name="debugLava">Boolean string indicating whether to display lava merge fields for debug purposes.</param>
        /// <param name="paymentMethod">The payment method to use for the other receipt. <see cref="PaymentMethod"/></param>
        /// <param name="groupingMode">The mode for handling grouping of GL accounts. NOTE: DebitLinesOnly mode is unsupported for Other Receipts. <see cref="GLAccountGroupingMode"/></param>
        /// <param name="bankAccountId">The GL bank account to use in the other receipt.</param>
        /// <param name="unDepGLAccountId">The GL undeposited funds account to use in the other receipt.</param>
        /// <param name="DescriptionLava">Lava code to use for the description of each line of the other receipt.</param>
        /// <returns>Returns the XML needed to create an Intacct Other Receipt.</returns>
        public XmlDocument CreateOtherReceiptXML( IntacctAuth AuthCreds, int BatchId, ref string debugLava, PaymentMethod paymentMethod, GLAccountGroupingMode groupingMode, string bankAccountId = null, string unDepGLAccountId = null, string DescriptionLava = "" )
        {
            var doc = new XmlDocument();
            var financialBatch = new FinancialBatchService( new RockContext() ).Get( BatchId );

            if ( financialBatch.Id > 0 )
            {
                var otherReceipt = BuildOtherReceipt( financialBatch, ref debugLava, paymentMethod, groupingMode, bankAccountId, unDepGLAccountId, DescriptionLava );
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
                            writer.WriteStartElement( "depositdate" );
                            writer.WriteElementString( "year", otherReceipt.DepositDate.Value.Year.ToString() );
                            writer.WriteElementString( "month", otherReceipt.DepositDate.Value.Month.ToString() );
                            writer.WriteElementString( "day", otherReceipt.DepositDate.Value.Day.ToString() );
                            writer.WriteEndElement();  // close depositdate
                        }
                        else if ( !string.IsNullOrWhiteSpace( otherReceipt.UnDepGLAccountNo ) )
                        {
                            writer.WriteElementString( "undepglaccountno", otherReceipt.UnDepGLAccountNo );
                        }
                        if ( !string.IsNullOrWhiteSpace( otherReceipt.RefId ) )
                        {
                            writer.WriteElementString( "refid", otherReceipt.RefId );
                        }
                        writer.WriteElementString( "description", otherReceipt.Description );
                        if ( !string.IsNullOrWhiteSpace( otherReceipt.Currency ) )
                        {
                            writer.WriteElementString( "currency", otherReceipt.Currency );
                        }
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
                            if ( !string.IsNullOrWhiteSpace( item.ProjectId ) )
                            {
                                writer.WriteElementString( "projectid", item.ProjectId );

                                // Task is a sub-dimension of Project in Intacct, so only include if Project is included.
                                if ( !string.IsNullOrWhiteSpace( item.TaskId ) )
                                {
                                    writer.WriteElementString( "taskid", item.TaskId );
                                }
                            }
                            if ( !string.IsNullOrWhiteSpace( item.CustomerId ) )
                            {
                                writer.WriteElementString( "customerid", item.CustomerId );
                            }
                            if ( !string.IsNullOrWhiteSpace( item.VendorId ) )
                            {
                                writer.WriteElementString( "vendorid", item.VendorId );
                            }
                            if ( !string.IsNullOrWhiteSpace( item.EmployeeId ) )
                            {
                                writer.WriteElementString( "employeeid", item.EmployeeId );
                            }
                            if ( !string.IsNullOrWhiteSpace( item.ItemId ) )
                            {
                                writer.WriteElementString( "itemid", item.ItemId );
                            }
                            if ( !string.IsNullOrWhiteSpace( item.ClassId ) )
                            {
                                writer.WriteElementString( "classid", item.ClassId );
                            }
                            if ( !string.IsNullOrWhiteSpace( item.ContractId ) )
                            {
                                writer.WriteElementString( "contractid", item.ContractId );
                            }
                            if ( !string.IsNullOrWhiteSpace( item.WarehouseId ) )
                            {
                                writer.WriteElementString( "warehouseid", item.WarehouseId );
                            }

                            // Intacct XML api documentation shows support for custom fields, but we are unable to get them to work. Disabling for now.
                            //if ( item.CustomFields.Count > 0 )
                            //{
                            //    writer.WriteStartElement( "customfields" );
                            //    foreach ( KeyValuePair<string, dynamic> customField in item.CustomFields )
                            //    {
                            //        writer.WriteStartElement( "customfield" );
                            //        writer.WriteElementString( "customfieldname", customField.Key );
                            //        writer.WriteElementString( "customfieldvalue", customField.Value ?? string.Empty );
                            //        writer.WriteEndElement();  // close customfield
                            //    }
                            //    writer.WriteEndElement();  // close customfields
                            //}
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

        private OtherReceipt BuildOtherReceipt( FinancialBatch financialBatch, ref string debugLava, PaymentMethod paymentMethod, GLAccountGroupingMode groupingMode, string bankAccountId = null, string unDepGLAccountId = null, string DescriptionLava = "" )
        {
            if ( string.IsNullOrWhiteSpace( DescriptionLava ) )
            {
                DescriptionLava = "{{ Batch.Id }}: {{ Batch.Name }}";
            }

            var rockContext = new RockContext();

            var batchDate = financialBatch.BatchStartDateTime == null ? RockDateTime.Now : ( ( System.DateTime ) financialBatch.BatchStartDateTime );
            var otherReceipt = new OtherReceipt
            {
                Payer = "Rock Batch Import",
                PaymentDate = batchDate,
                ReceivedDate = batchDate,
                PaymentMethod = paymentMethod,
                BankAccountId = bankAccountId,
                UnDepGLAccountNo = unDepGLAccountId,
                DepositDate = batchDate,
                Description = string.Format( "Imported From Rock batch {0}: {1}", financialBatch.Id, financialBatch.Name ),
                RefId = financialBatch.Id.ToString(),
                ReceiptItems = new List<ReceiptLineItem>()
            };
            List<RegistrationInstance> registrationLinks;
            List<GroupMember> groupMemberLinks;
            var receiptTransactions = TransactionHelpers.GetTransactionSummary( financialBatch, rockContext, out registrationLinks, out groupMemberLinks, groupingMode );

            //
            // Get the Dimensions from the Account since the Transaction Details have been Grouped already
            //
            var customDimensions = TransactionHelpers.GetCustomDimensions();
            var lineItemList = new List<ReceiptLineItem>();

            // Create Receipt Item for each entry within a grouping
            foreach ( var bTran in receiptTransactions )
            {
                var account = new FinancialAccountService( rockContext ).Get( bTran.FinancialAccountId );
                var customDimensionValues = new SortedDictionary<string, dynamic>();
                account.LoadAttributes();
                var mergeFieldObjects = new MergeFieldObjects
                {
                    Account = account,
                    Batch = financialBatch,
                    Registrations = registrationLinks,
                    GroupMembers = groupMemberLinks,
                    Summary = bTran,
                    CustomDimensions = customDimensions
                };
                Dictionary<string, object> mergeFields = TransactionHelpers.GetMergeFieldsAndDimensions( ref debugLava, customDimensionValues, mergeFieldObjects );

                // We want to include any attribute dimensions with "_credit" in the key, or neither "_debit" nor "_credit". It is cleanest to do this by just excluding "_debit". 
                var creditDimensions = TransactionHelpers.GetFilteredDimensions( customDimensionValues, "_debit", "_credit" );

                var classId = account.GetAttributeValue( "rocks.kfs.Intacct.CLASSID" );
                var departmentId = account.GetAttributeValue( "rocks.kfs.Intacct.DEPARTMENT" );
                var locationId = account.GetAttributeValue( "rocks.kfs.Intacct.LOCATION" );
                var taskId = account.GetAttributeValue( "rocks.kfs.Intacct.TASKID" );
                var customerId = account.GetAttributeValue( "rocks.kfs.Intacct.CUSTOMERID" );
                var itemId = account.GetAttributeValue( "rocks.kfs.Intacct.ITEMID" );
                var vendorId = account.GetAttributeValue( "rocks.kfs.Intacct.VENDORID" );
                var employeeId = account.GetAttributeValue( "rocks.kfs.Intacct.EMPLOYEEID" );
                var contractId = account.GetAttributeValue( "rocks.kfs.Intacct.CONTRACTID" );
                var warehouseId = account.GetAttributeValue( "rocks.kfs.Intacct.WAREHOUSEID" );

                var receiptItem = new ReceiptLineItem
                {
                    GlAccountNo = account.GetAttributeValue( "rocks.kfs.Intacct.ACCOUNTNO" ),
                    Amount = bTran.ProcessTransactionFees == 1 ? bTran.Amount - bTran.TransactionFeeAmount : bTran.Amount,
                    Memo = DescriptionLava.ResolveMergeFields( mergeFields ),
                    LocationId = locationId,
                    DepartmentId = departmentId,
                    ProjectId = bTran.CreditProject,
                    ClassId = classId,
                    TaskId = taskId,
                    CustomerId = customerId,
                    ItemId = itemId,
                    VendorId = vendorId,
                    EmployeeId = employeeId,
                    ContractId = contractId,
                    WarehouseId = warehouseId,
                    CustomFields = creditDimensions,
                    CustomFieldsString = string.Join( Environment.NewLine, new Dictionary<string, dynamic>( creditDimensions ) )
                };
                lineItemList.Add( receiptItem );

                if ( bTran.ProcessTransactionFees == 2 )
                {
                    var feeLineItem = new ReceiptLineItem
                    {
                        GlAccountNo = bTran.TransactionFeeAccount,
                        Amount = bTran.TransactionFeeAmount * -1,
                        Memo = "Transaction Fees",
                        LocationId = locationId,
                        DepartmentId = departmentId,
                        ProjectId = bTran.CreditProject,
                        ClassId = classId,
                        ContractId = contractId,
                        WarehouseId = warehouseId,
                        CustomFields = creditDimensions,
                        CustomFieldsString = string.Join( Environment.NewLine, new Dictionary<string, dynamic>( creditDimensions ) )
                    };
                    lineItemList.Add( feeLineItem );
                }
            }

            if ( groupingMode == GLAccountGroupingMode.DebitAndCreditLines || groupingMode == GLAccountGroupingMode.CreditLinesOnly )
            {
                lineItemList = lineItemList
                    .GroupBy( d => new { d.ClassId, d.DepartmentId, d.LocationId, d.ProjectId, d.GlAccountNo, d.CustomFieldsString } )
                    .Select( s => new ReceiptLineItem
                    {
                        Amount = s.Sum( f => f.Amount ),
                        GlAccountNo = s.Key.GlAccountNo,
                        ClassId = s.Key.ClassId,
                        DepartmentId = s.Key.DepartmentId,
                        LocationId = s.Key.LocationId,
                        ProjectId = s.Key.ProjectId,
                        Memo = s.First().Memo,
                        CustomFields = s.First().CustomFields
                    } )
                    .ToList();
            }

            otherReceipt.ReceiptItems.AddRange( lineItemList );

            return otherReceipt;
        }

        public List<GLReceiptCsvLine> GetOtherReceiptCsvLines( FinancialBatch financialBatch, ref string debugLava, PaymentMethod paymentMethod, GLAccountGroupingMode groupingMode, string bankAccountId = null, string unDepGLAccountId = null, string DescriptionLava = "" )
        {
            var glCsvLines = new List<GLReceiptCsvLine>();

            var otherReceipt = BuildOtherReceipt( financialBatch, ref debugLava, paymentMethod, groupingMode, bankAccountId, unDepGLAccountId, DescriptionLava );
            var entryLineNumber = 1;

            foreach ( var item in otherReceipt.ReceiptItems )
            {
                var csvLine = new GLReceiptCsvLine()
                {
                    LineNumber = entryLineNumber,
                    AccountNumber = item.GlAccountNo ?? string.Empty,
                    AccountLabel = item.GlAccountLabel ?? string.Empty,
                    TransactionAmount = item.Amount,
                    Amount = item.Amount,
                    DepartmentId = item.DepartmentId,
                    LocationId = item.LocationId,
                    Memo = item.Memo,
                    ProjectId = item.ProjectId,
                    CustomerId = item.CustomerId,
                    ItemId = item.ItemId,
                    VendorId = item.VendorId,
                    EmployeeId = item.EmployeeId,
                    ClassId = item.ClassId,
                    CustomFields = item.CustomFields
                };

                // Only add Batch/Receipt level info to first line of the receipt.
                if ( entryLineNumber == 1 )
                {
                    csvLine.ReceiptDate = otherReceipt.PaymentDate;
                    csvLine.Payer = otherReceipt.Payer;
                    csvLine.PaymentMethod = otherReceipt.PaymentMethod;
                    csvLine.TransactionDate = otherReceipt.ReceivedDate;
                    csvLine.TransactionNumber = otherReceipt.RefId;
                    csvLine.Description = otherReceipt.Description;
                    csvLine.DepositTo = otherReceipt.BankAccountId != null ? DepositTo.BankAccount : DepositTo.UndepositedFunds;
                    csvLine.BankAccountId = otherReceipt.BankAccountId;
                    csvLine.DepositDate = otherReceipt.DepositDate;
                    csvLine.UndepositedFundsAccountId = otherReceipt.UnDepGLAccountNo;
                    csvLine.Currency = otherReceipt.Currency;
                    csvLine.ExchRateDate = otherReceipt.ExchRateDate;
                    csvLine.ExchRateType = otherReceipt.ExchRateType;
                    csvLine.ExchRate = otherReceipt.ExchRate;
                }

                glCsvLines.Add( csvLine );
                entryLineNumber++;
            }

            return glCsvLines;
        }

        public void GLCsvExport( List<GLReceiptCsvLine> items, string fileId )
        {
            if ( HttpContext.Current.Session["IntacctCsvExport"] != null )
            {
                HttpContext.Current.Session["IntacctCsvExport"] = string.Empty;
            }
            if ( HttpContext.Current.Session["IntacctFileId"] != null )
            {
                HttpContext.Current.Session["IntacctFileId"] = string.Empty;
            }

            var customFieldCols = items.SelectMany( i => i.CustomFields.Keys ).Distinct().ToList().OrderBy( k => k );
            var exportColumns = new ExportColumns();
            exportColumns.CustomFieldKeys = customFieldCols.ToList();

            var output = new StringBuilder();
            output.Append( "Receipt_Date, Payer_Name, PayMethod, DocDate, DocNumber, Description" );
            if ( items.Any( i => i.DepositTo == DepositTo.BankAccount ) )
            {
                output.Append( ", BankAccountId, DepositDate" );
                exportColumns.BankAccountId = true;
                exportColumns.DepositDate = true;
            }
            if ( items.Any( i => i.DepositTo == DepositTo.UndepositedFunds ) )
            {
                output.Append( ", DepositTo, UndepAcctNo" );
                exportColumns.UndepAcctNo = true;
            }
            if ( items.Any( i => !i.Currency.IsNullOrWhiteSpace() ) )
            {
                output.Append( ", Currency" );
                exportColumns.Currency = true;
            }
            if ( items.Any( i => i.ExchRateDate.HasValue ) )
            {
                output.Append( ", Exch_Rate_Date" );
                exportColumns.ExchangeRateDate = true;
            }
            if ( items.Any( i => !i.ExchRateType.IsNullOrWhiteSpace() ) )
            {
                output.Append( ", Exch_Rate_Type_Id" );
                exportColumns.ExchangeRateTypeId = true;
            }
            if ( items.Any( i => i.ExchRate.HasValue ) )
            {
                output.Append( ", Exch_Rate" );
                exportColumns.ExchangeRate = true;
            }
            output.Append( ", Line_No, Acct_No" );
            if ( items.Any( i => !i.AccountLabel.IsNullOrWhiteSpace() ) )
            {
                output.Append( ", AccountLabel" );
                exportColumns.AccountLabel = true;
            }
            output.Append( ", Trx_Amount, Amount" );
            if ( items.Any( i => !i.DepartmentId.IsNullOrWhiteSpace() ) )
            {
                output.Append( ", Dept_Id" );
                exportColumns.DepartmentId = true;
            }
            output.Append( ", Location_Id, Item_Memo" );
            if ( items.Any( i => !i.ProjectId.IsNullOrWhiteSpace() ) )
            {
                output.Append( ", OtherReceiptsEntry_ProjectId" );
                exportColumns.ProjectId = true;
                if ( items.Any( i => !i.TaskId.IsNullOrWhiteSpace() ) )   // Task is a sub-dimension of Project in Intacct, so only include if Project is included.
                {
                    output.Append( ", OtherReceiptsEntry_TaskId" );
                    exportColumns.TaskId = true;
                }
            }
            if ( items.Any( i => !i.CustomerId.IsNullOrWhiteSpace() ) )
            {
                output.Append( ", OtherReceiptsEntry_CustomerId" );
                exportColumns.CustomerId = true;
            }
            if ( items.Any( i => !i.ItemId.IsNullOrWhiteSpace() ) )
            {
                output.Append( ", OtherReceiptsEntry_ItemId" );
                exportColumns.ItemId = true;
            }
            if ( items.Any( i => !i.VendorId.IsNullOrWhiteSpace() ) )
            {
                output.Append( ", OtherReceiptsEntry_VendorId" );
                exportColumns.VendorId = true;
            }
            if ( items.Any( i => !i.EmployeeId.IsNullOrWhiteSpace() ) )
            {
                output.Append( ", OtherReceiptsEntry_EmployeeId" );
                exportColumns.EmployeeId = true;
            }
            if ( items.Any( i => !i.ClassId.IsNullOrWhiteSpace() ) )
            {
                output.Append( ", OtherReceiptsEntry_ClassId" );
                exportColumns.ClassId = true;
            }
            foreach ( var customFieldCol in customFieldCols )
            {
                output.AppendFormat( ", {0}", customFieldCol );
            }

            foreach ( var item in items )
            {
                output.Append( Environment.NewLine );
                output.AppendFormat( "{0},{1},{2},{3},{4},{5}", item.ReceiptDate.HasValue ? item.ReceiptDate.Value.ToShortDateString() : string.Empty, item.Payer, item.PaymentMethod.HasValue ? item.PaymentMethod.GetDescription() : string.Empty, item.TransactionDate.HasValue ? item.TransactionDate.Value.ToShortDateString() : string.Empty, item.TransactionNumber, item.Description );
                if ( exportColumns.BankAccountId )
                {
                    output.AppendFormat( ",{0},{1}", item.BankAccountId ?? string.Empty, item.DepositDate.HasValue ? item.DepositDate.Value.ToShortDateString() : string.Empty );
                }
                if ( exportColumns.UndepAcctNo )
                {
                    output.AppendFormat( ",{0},{1}", item.DepositTo.HasValue ? item.DepositTo.GetDescription() : string.Empty, item.UndepositedFundsAccountId ?? string.Empty );
                }
                if ( exportColumns.Currency )
                {
                    output.AppendFormat( ",{0}", item.Currency ?? string.Empty );
                }
                if ( exportColumns.ExchangeRateDate )
                {
                    output.AppendFormat( ",{0}", item.ExchRateDate.HasValue ? item.ExchRateDate.Value.ToShortDateString() : string.Empty );
                }
                if ( exportColumns.ExchangeRateTypeId )
                {
                    output.AppendFormat( ",{0}", item.ExchRateType ?? string.Empty );
                }
                if ( exportColumns.ExchangeRate )
                {
                    output.AppendFormat( ",{0}", item.ExchRate.HasValue ? item.ExchRate.Value.ToString() : string.Empty );
                }
                output.AppendFormat( ",{0},{1}", item.LineNumber, item.AccountNumber );
                if ( exportColumns.AccountLabel )
                {
                    output.AppendFormat( ",{0}", item.AccountLabel ?? string.Empty );
                }
                output.AppendFormat( ",{0},{1}", item.TransactionAmount, item.Amount );
                if ( exportColumns.DepartmentId )
                {
                    output.AppendFormat( ",{0}", item.DepartmentId ?? string.Empty );
                }
                output.AppendFormat( ",{0},{1}", item.LocationId, item.Memo );
                if ( exportColumns.ProjectId )
                {
                    output.AppendFormat( ",{0}", item.ProjectId ?? string.Empty );

                    // Task is a sub-dimension of Project in Intacct, so only include if Project is included.
                    if ( exportColumns.TaskId )
                    {
                        output.AppendFormat( ",{0}", item.TaskId ?? string.Empty );
                    }
                }
                if ( exportColumns.CustomerId )
                {
                    output.AppendFormat( ",{0}", item.CustomerId ?? string.Empty );
                }
                if ( exportColumns.ItemId )
                {
                    output.AppendFormat( ",{0}", item.ItemId ?? string.Empty );
                }
                if ( exportColumns.VendorId )
                {
                    output.AppendFormat( ",{0}", item.VendorId ?? string.Empty );
                }
                if ( exportColumns.EmployeeId )
                {
                    output.AppendFormat( ",{0}", item.EmployeeId ?? string.Empty );
                }
                if ( exportColumns.ClassId )
                {
                    output.AppendFormat( ",{0}", item.ClassId ?? string.Empty );
                }
                foreach ( var customFieldCol in exportColumns.CustomFieldKeys )
                {
                    output.AppendFormat( ",{0}", item.CustomFields.ContainsKey( customFieldCol ) ? item.CustomFields[customFieldCol] : string.Empty );
                }
            }
            HttpContext.Current.Session["IntacctCsvExport"] = output.ToString();
            HttpContext.Current.Session["IntacctFileId"] = fileId;
        }

        public class ExportColumns
        {
            public bool BankAccountId = false;
            public bool DepositDate = false;
            public bool UndepAcctNo = false;
            public bool ExchangeRateDate = false;
            public bool ExchangeRateTypeId = false;
            public bool ExchangeRate = false;
            public bool AccountLabel = false;
            public bool Currency = false;
            public List<string> CustomFieldKeys = new List<string>();
            public bool DepartmentId = false;
            public bool ProjectId = false;
            public bool CustomerId = false;
            public bool VendorId = false;
            public bool EmployeeId = false;
            public bool ItemId = false;
            public bool ClassId = false;
            public bool TaskId = false;
        }
    }
}
