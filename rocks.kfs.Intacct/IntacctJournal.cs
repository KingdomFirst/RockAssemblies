// <copyright>
// Copyright 2025 by Kingdom First Solutions
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
using System.Web;
using System.Xml;
using OfficeOpenXml;

using Rock;
using Rock.Data;
using Rock.Model;
using Rock.Utility;
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
        /// <param name="BatchId">The BatchId of the Rock FinancialBatch that a Journal Entry will be created from.</param>
        /// <param name="JournalId">The Intacct Symbol of the Journal that the Entry should be posted to. For example: GJ</param>
        /// <param name="debugLava">Boolean string indicating whether to display lava merge fields for debug purposes.</param>
        /// <param name="DescriptionLava">Lava code to use for the description of each line of the journal entry.</param>
        /// <param name="groupingMode">The mode for handling grouping of GL accounts. <see cref="GLAccountGroupingMode"/></param>
        /// <returns>Returns the XML needed to create an Intacct Journal Entry.</returns>
        public XmlDocument CreateJournalEntryXML( IntacctAuth AuthCreds, int BatchId, string JournalId, ref string debugLava, string DescriptionLava, GLAccountGroupingMode groupingMode, JournalState journalState = JournalState.Posted )
        {
            var doc = new XmlDocument();
            var financialBatch = new FinancialBatchService( new RockContext() ).Get( BatchId );

            if ( financialBatch.Id > 0 )
            {
                var lines = GetGlEntries( financialBatch, ref debugLava, DescriptionLava, groupingMode );
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
                        if ( journalState == JournalState.Draft )
                        {
                            writer.WriteElementString( "STATE", journalState.ToString() );
                        }
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

        private List<JournalEntryLine> GetGlEntries( FinancialBatch financialBatch, ref string debugLava, string DescriptionLava, GLAccountGroupingMode groupingMode )
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
            var batchTransactionsSummary = TransactionHelpers.GetTransactionSummary( financialBatch, rockContext, out registrationLinks, out groupMemberLinks, groupingMode );

            //
            // Get the Dimensions from the Account since the Transaction Details have been Grouped already
            //
            var customDimensions = TransactionHelpers.GetCustomDimensions();
            var batchSummary = new List<GLBatchTotals>();
            foreach ( var summary in batchTransactionsSummary )
            {
                var account = new FinancialAccountService( rockContext ).Get( summary.FinancialAccountId );
                var customDimensionValues = new SortedDictionary<string, dynamic>();
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
                Dictionary<string, object> mergeFields = TransactionHelpers.GetMergeFieldsAndDimensions( ref debugLava, customDimensionValues, mergeFieldObjects );

                var batchSummaryItem = new GLBatchTotals()
                {
                    Amount = summary.Amount,
                    CreditAccount = account.GetAttributeValue( "rocks.kfs.Intacct.ACCOUNTNO" ),
                    DebitAccount = account.GetAttributeValue( "rocks.kfs.Intacct.DEBITACCOUNTNO" ),
                    TransactionFeeAmount = summary.TransactionFeeAmount,
                    TransactionFeeAccount = summary.TransactionFeeAccount,
                    CreditClass = account.GetAttributeValue( "rocks.kfs.Intacct.CLASSID" ),
                    CreditDepartment = account.GetAttributeValue( "rocks.kfs.Intacct.DEPARTMENT" ),
                    CreditLocation = account.GetAttributeValue( "rocks.kfs.Intacct.LOCATION" ),
                    CreditProject = summary.CreditProject,
                    DebitClass = account.GetAttributeValue( "rocks.kfs.Intacct.DEBITCLASSID" ),
                    DebitDepartment = account.GetAttributeValue( "rocks.kfs.Intacct.DEBITDEPARTMENT" ),
                    DebitLocation = account.GetAttributeValue( "rocks.kfs.Intacct.DEBITLOCATION" ),
                    DebitProject = summary.DebitProject,
                    Description = DescriptionLava.ResolveMergeFields( mergeFields ),
                    CustomDimensions = new SortedDictionary<string, dynamic>( customDimensionValues ),
                    ProcessTransactionFees = summary.ProcessTransactionFees
                };

                batchSummary.Add( batchSummaryItem );
            }

            return GenerateLineItems( batchSummary, groupingMode );
        }

        private List<JournalEntryLine> GenerateLineItems( List<GLBatchTotals> transactionItems, GLAccountGroupingMode groupingMode )
        {
            var returnList = new List<JournalEntryLine>();

            var itemIndex = 0;
            foreach ( var transactionItem in transactionItems )
            {
                transactionItem.ItemIndex = itemIndex;
                itemIndex++;
            }

            var debitTransactions = transactionItems.Select( ti => ( GLBatchTotals ) ti.Clone() ).ToList();
            var creditTransactions = transactionItems.Select( ti => ( GLBatchTotals ) ti.Clone() ).ToList();
            var feeDebitTransactions = transactionItems.Where( f => ( f.TransactionFeeAmount > 0.0M || f.TransactionFeeAmount < 0.0M ) && !string.IsNullOrWhiteSpace( f.TransactionFeeAccount ) && f.ProcessTransactionFees > 0 ).Select( ti => ( GLBatchTotals ) ti.Clone() ).ToList();
            var feeCreditTransactions = transactionItems.Where( f => ( f.TransactionFeeAmount > 0.0M || f.TransactionFeeAmount < 0.0M ) && !string.IsNullOrWhiteSpace( f.TransactionFeeAccount ) && f.ProcessTransactionFees == 2 ).Select( ti => ( GLBatchTotals ) ti.Clone() ).ToList();

            // Condition and prepare debit entries
            foreach ( var t in debitTransactions )
            {
                t.Amount = ( ( decimal? ) t.Amount ?? 0.0M ) - ( t.ProcessTransactionFees == 1 ? t.TransactionFeeAmount : 0.0M );

                // We want to include any attribute dimensions with "debit" in the key, or neither "debit" nor "credit". It is cleanest to do this by just excluding "credit". 
                var debitDimensions = TransactionHelpers.GetFilteredDimensions( t.CustomDimensions, "_credit", "_debit" );
                t.CustomDimensions = debitDimensions;
                t.CustomDimensionString = string.Join( Environment.NewLine, new Dictionary<string, dynamic>( debitDimensions ) );
            }

            foreach ( var t in feeDebitTransactions )
            {
                t.Amount = ( decimal? ) t.TransactionFeeAmount ?? 0.0M;
                t.DebitAccount = t.TransactionFeeAccount;
                t.Description += " Transaction Fees";

                var debitDimensions = TransactionHelpers.GetFilteredDimensions( t.CustomDimensions, "_credit", "_debit" );
                t.CustomDimensions = debitDimensions;
                t.CustomDimensionString = string.Join( Environment.NewLine, new Dictionary<string, dynamic>( debitDimensions ) );
                t.FeeItemIndex = t.ItemIndex;
            }

            if ( groupingMode == GLAccountGroupingMode.DebitAndCreditLines || groupingMode == GLAccountGroupingMode.DebitLinesOnly )
            {
                debitTransactions = debitTransactions
                    .GroupBy( d => new { d.DebitClass, d.DebitDepartment, d.DebitLocation, d.DebitProject, d.DebitAccount, d.CustomDimensionString, d.ProcessTransactionFees } )
                    .Select( s => new GLBatchTotals()
                    {
                        Amount = s.Sum( f => f.Amount ),
                        DebitAccount = s.Key.DebitAccount,
                        DebitClass = s.Key.DebitClass,
                        DebitDepartment = s.Key.DebitDepartment,
                        DebitLocation = s.Key.DebitLocation,
                        DebitProject = s.Key.DebitProject,
                        Description = s.First().Description,
                        CustomDimensions = s.First().CustomDimensions,
                        ItemIndex = s.First().ItemIndex
                    } )
                    .ToList();

                feeDebitTransactions = feeDebitTransactions
                    .GroupBy( d => new { d.DebitClass, d.DebitDepartment, d.DebitLocation, d.DebitProject, d.DebitAccount, d.CustomDimensionString } )
                    .Select( s => new GLBatchTotals
                    {
                        Amount = s.Sum( f => f.Amount ),
                        DebitAccount = s.Key.DebitAccount,
                        DebitClass = s.Key.DebitClass,
                        DebitDepartment = s.Key.DebitDepartment,
                        DebitLocation = s.Key.DebitLocation,
                        DebitProject = s.Key.DebitProject,
                        Description = s.First().Description,
                        CustomDimensions = s.First().CustomDimensions,
                        ItemIndex = s.First().ItemIndex,
                        FeeItemIndex = s.First().FeeItemIndex
                    } )
                    .ToList();
            }

            // Condition and prepare credit entries
            foreach ( var t in creditTransactions )
            {
                t.Amount = ( ( decimal? ) t.Amount ?? 0.0M ) * -1;

                // We want to include any attribute dimensions with "_credit" in the key, or neither "_debit" nor "_credit". It is cleanest to do this by just excluding "_debit". 
                var creditDimensions = TransactionHelpers.GetFilteredDimensions( t.CustomDimensions, "_debit", "_credit" );
                t.CustomDimensions = creditDimensions;
                t.CustomDimensionString = string.Join( Environment.NewLine, new Dictionary<string, dynamic>( creditDimensions ) );
            }

            foreach ( var t in feeCreditTransactions )
            {
                t.Amount = ( ( decimal? ) t.TransactionFeeAmount ?? 0.0M ) * -1;
                t.CreditAccount = t.DebitAccount;

                var creditDimensions = TransactionHelpers.GetFilteredDimensions( t.CustomDimensions, "_debit", "_credit" );
                t.CustomDimensions = creditDimensions;
                t.CustomDimensionString = string.Join( Environment.NewLine, new Dictionary<string, dynamic>( creditDimensions ) );
                t.FeeItemIndex = t.ItemIndex;
            }

            if ( groupingMode == GLAccountGroupingMode.DebitAndCreditLines || groupingMode == GLAccountGroupingMode.CreditLinesOnly )
            {
                creditTransactions = creditTransactions
                    .GroupBy( d => new { d.CreditClass, d.CreditDepartment, d.CreditLocation, d.CreditProject, d.CreditAccount, d.CustomDimensionString } )
                    .Select( s => new GLBatchTotals
                    {
                        Amount = s.Sum( f => f.Amount ),
                        CreditAccount = s.Key.CreditAccount,
                        CreditClass = s.Key.CreditClass,
                        CreditDepartment = s.Key.CreditDepartment,
                        CreditLocation = s.Key.CreditLocation,
                        CreditProject = s.Key.CreditProject,
                        Description = s.First().Description,
                        CustomDimensions = s.First().CustomDimensions,
                        ItemIndex = s.First().ItemIndex
                    } )
                    .ToList();

                feeCreditTransactions = feeCreditTransactions
                    .GroupBy( d => new { d.CreditClass, d.CreditDepartment, d.CreditLocation, d.CreditProject, d.CreditAccount, d.CustomDimensionString } )
                    .Select( s => new GLBatchTotals
                    {
                        Amount = s.Sum( f => f.Amount ),
                        CreditAccount = s.Key.CreditAccount,
                        CreditClass = s.Key.CreditClass,
                        CreditDepartment = s.Key.CreditDepartment,
                        CreditLocation = s.Key.CreditLocation,
                        CreditProject = s.Key.CreditProject,
                        Description = s.First().Description,
                        CustomDimensions = s.First().CustomDimensions,
                        ItemIndex = s.First().ItemIndex,
                        FeeItemIndex = s.First().FeeItemIndex
                    } )
                    .ToList();
            }

            var allCreditTransactions = creditTransactions.Concat( feeCreditTransactions ).ToList();
            var allDebitTransactions = debitTransactions.Concat( feeDebitTransactions ).ToList();

            foreach ( var debitTransaction in allDebitTransactions )
            {
                var debitLine = new JournalEntryLine()
                {
                    GlAccountNumber = debitTransaction.DebitAccount,
                    TransactionAmount = debitTransaction.Amount,
                    ClassId = debitTransaction.DebitClass,
                    DepartmentId = debitTransaction.DebitDepartment,
                    LocationId = debitTransaction.DebitLocation,
                    ProjectId = debitTransaction.DebitProject,
                    Memo = debitTransaction.Description,
                    CustomFields = debitTransaction.CustomDimensions,
                    ItemIndex = debitTransaction.ItemIndex,
                    FeeItemIndex = debitTransaction.FeeItemIndex,
                };

                returnList.Add( debitLine );
            }

            foreach ( var creditTransaction in allCreditTransactions )
            {
                var creditLine = new JournalEntryLine()
                {
                    GlAccountNumber = creditTransaction.CreditAccount,
                    TransactionAmount = creditTransaction.Amount,
                    ClassId = creditTransaction.CreditClass,
                    DepartmentId = creditTransaction.CreditDepartment,
                    LocationId = creditTransaction.CreditLocation,
                    ProjectId = creditTransaction.CreditProject,
                    Memo = creditTransaction.Description,
                    CustomFields = creditTransaction.CustomDimensions,
                    ItemIndex = creditTransaction.ItemIndex,
                    FeeItemIndex = creditTransaction.FeeItemIndex,
                };

                returnList.Add( creditLine );
            }
            if ( groupingMode == GLAccountGroupingMode.NoGrouping || groupingMode == GLAccountGroupingMode.DebitAndCreditByFinancialAccount )
            {
                returnList = returnList.OrderBy( i => i.ItemIndex ).ThenBy( i => i.FeeItemIndex ).ThenByDescending( i => i.TransactionAmount ).ToList();
            }

            return returnList;
        }

        public List<GLExcelLine> GetGLExcelLines( RockContext rockContext, FinancialBatch financialBatch, string journalCode, int period, ref string debugLava )
        {
            var glExcelLines = new List<GLExcelLine>();
            var glEntries = GetGlEntries( rockContext, financialBatch, journalCode, period, ref debugLava );
            foreach ( var entry in glEntries )
            {
                glExcelLines.Add( new GLExcelLine()
                {
                    CompanyNumber = entry.CompanyNumber,
                    RegionNumber = entry.RegionNumber,
                    SuperFundNumber = entry.SuperFundNumber,
                    FundNumber = entry.FundNumber,
                    LocationNumber = entry.LocationNumber,
                    CostCenterNumber = entry.CostCenterNumber,
                    DepartmentNumber = entry.DepartmentNumber,
                    AccountNumber = entry.AccountNumber,
                    AccountSub = entry.AccountSub,
                    Amount = entry.Amount,
                    Project = entry.Project,
                    JournalNumber = entry.JournalNumber,
                    JournalDescription = entry.JournalDescription,
                    Date = entry.Date,
                    Note = entry.Note,
                    Period = period,
                    JournalCode = journalCode
                } );
            }

            return glExcelLines;
        }

        public ExcelPackage GLExcelExport( List<GLExcelLine> items )
        {
            var exportColumns = GetExportColumns( items );

            // create default settings
            string workSheetName = "Export";
            string title = "RockExport";

            ExcelPackage excel = new ExcelPackage();

            excel.Workbook.Properties.Title = title;

            // add author info
            Rock.Model.UserLogin userLogin = Rock.Model.UserLoginService.GetCurrentUser();
            if ( userLogin != null )
            {
                excel.Workbook.Properties.Author = userLogin.Person.FullName;
            }
            else
            {
                excel.Workbook.Properties.Author = "Rock";
            }

            // add the page that created this
            excel.Workbook.Properties.SetCustomPropertyValue( "Source", HttpContext.Current.Request.Url.OriginalString );

            ExcelWorksheet worksheet = excel.Workbook.Worksheets.Add( workSheetName );

            var headerRows = 1;
            int rowCounter = headerRows;
            int columnCounter = 1;

            worksheet.Cells[rowCounter, columnCounter].Value = "Amount";
            worksheet.Column( columnCounter ).Style.Numberformat.Format = ExcelHelper.FormattedNumberFormat;
            columnCounter++;
            worksheet.Cells[rowCounter, columnCounter].Value = "JournalNumber";
            worksheet.Column( columnCounter ).Style.Numberformat.Format = ExcelHelper.UnformattedNumberFormat;
            columnCounter++;
            worksheet.Cells[rowCounter, columnCounter].Value = "JournalDescription";
            worksheet.Column( columnCounter ).Style.Numberformat.Format = ExcelHelper.GeneralFormat;
            columnCounter++;
            worksheet.Cells[rowCounter, columnCounter].Value = "Date";
            worksheet.Column( columnCounter ).Style.Numberformat.Format = ExcelHelper.DateFormat;
            columnCounter++;
            worksheet.Cells[rowCounter, columnCounter].Value = "Period";
            worksheet.Column( columnCounter ).Style.Numberformat.Format = ExcelHelper.UnformattedNumberFormat;
            columnCounter++;
            worksheet.Cells[rowCounter, columnCounter].Value = "JournalCode";
            worksheet.Column( columnCounter ).Style.Numberformat.Format = ExcelHelper.GeneralFormat;

            if ( exportColumns.CompanyNumber )
            {
                columnCounter++;
                worksheet.Cells[rowCounter, columnCounter].Value = "CompanyNumber";
                worksheet.Column( columnCounter ).Style.Numberformat.Format = ExcelHelper.UnformattedNumberFormat;
            }

            if ( exportColumns.RegionNumber )
            {
                columnCounter++;
                worksheet.Cells[rowCounter, columnCounter].Value = "RegionNumber";
                worksheet.Column( columnCounter ).Style.Numberformat.Format = ExcelHelper.UnformattedNumberFormat;
            }

            if ( exportColumns.SuperFundNumber )
            {
                columnCounter++;
                worksheet.Cells[rowCounter, columnCounter].Value = "SuperFundNumber";
                worksheet.Column( columnCounter ).Style.Numberformat.Format = ExcelHelper.UnformattedNumberFormat;
            }

            if ( exportColumns.FundNumber )
            {
                columnCounter++;
                worksheet.Cells[rowCounter, columnCounter].Value = "FundNumber";
                worksheet.Column( columnCounter ).Style.Numberformat.Format = ExcelHelper.UnformattedNumberFormat;
            }

            if ( exportColumns.LocationNumber )
            {
                columnCounter++;
                worksheet.Cells[rowCounter, columnCounter].Value = "LocationNumber";
                worksheet.Column( columnCounter ).Style.Numberformat.Format = ExcelHelper.UnformattedNumberFormat;
            }

            if ( exportColumns.CostCenterNumber )
            {
                columnCounter++;
                worksheet.Cells[rowCounter, columnCounter].Value = "CostCenterNumber";
                worksheet.Column( columnCounter ).Style.Numberformat.Format = ExcelHelper.UnformattedNumberFormat;
            }

            if ( exportColumns.DepartmentNumber )
            {
                columnCounter++;
                worksheet.Cells[rowCounter, columnCounter].Value = "DepartmentNumber";
                worksheet.Column( columnCounter ).Style.Numberformat.Format = ExcelHelper.UnformattedNumberFormat;
            }

            if ( exportColumns.AccountNumber )
            {
                columnCounter++;
                worksheet.Cells[rowCounter, columnCounter].Value = "AccountNumber";
                worksheet.Column( columnCounter ).Style.Numberformat.Format = ExcelHelper.UnformattedNumberFormat;
            }

            if ( exportColumns.AccountSub )
            {
                columnCounter++;
                worksheet.Cells[rowCounter, columnCounter].Value = "AccountSub";
                worksheet.Column( columnCounter ).Style.Numberformat.Format = ExcelHelper.UnformattedNumberFormat;
            }

            if ( exportColumns.Project )
            {
                columnCounter++;
                worksheet.Cells[rowCounter, columnCounter].Value = "Project";
                worksheet.Column( columnCounter ).Style.Numberformat.Format = ExcelHelper.UnformattedNumberFormat;
            }

            if ( exportColumns.Note )
            {
                columnCounter++;
                worksheet.Cells[rowCounter, columnCounter].Value = "Note";
                worksheet.Column( columnCounter ).Style.Numberformat.Format = ExcelHelper.GeneralFormat;
            }

            // print data
            if ( items.Any() )
            {
                foreach ( var item in items )
                {
                    rowCounter++;

                    var columnIndex = 1;
                    ExcelHelper.SetExcelValue( worksheet.Cells[rowCounter, columnIndex], item.Amount );
                    ExcelHelper.FinalizeColumnFormat( worksheet, columnIndex, item.Amount );
                    columnIndex++;
                    ExcelHelper.SetExcelValue( worksheet.Cells[rowCounter, columnIndex], item.JournalNumber );
                    ExcelHelper.FinalizeColumnFormat( worksheet, columnIndex, item.JournalNumber );
                    columnIndex++;
                    ExcelHelper.SetExcelValue( worksheet.Cells[rowCounter, columnIndex], item.JournalDescription );
                    ExcelHelper.FinalizeColumnFormat( worksheet, columnIndex, item.JournalDescription );
                    columnIndex++;
                    ExcelHelper.SetExcelValue( worksheet.Cells[rowCounter, columnIndex], item.Date );
                    ExcelHelper.FinalizeColumnFormat( worksheet, columnIndex, item.Date );
                    columnIndex++;
                    ExcelHelper.SetExcelValue( worksheet.Cells[rowCounter, columnIndex], item.Period );
                    ExcelHelper.FinalizeColumnFormat( worksheet, columnIndex, item.Period );
                    columnIndex++;
                    ExcelHelper.SetExcelValue( worksheet.Cells[rowCounter, columnIndex], item.JournalCode );
                    ExcelHelper.FinalizeColumnFormat( worksheet, columnIndex, item.JournalCode );

                    if ( exportColumns.CompanyNumber )
                    {
                        columnIndex++;
                        ExcelHelper.SetExcelValue( worksheet.Cells[rowCounter, columnIndex], item.CompanyNumber );
                        ExcelHelper.FinalizeColumnFormat( worksheet, columnIndex, item.CompanyNumber );
                    }

                    if ( exportColumns.RegionNumber )
                    {
                        columnIndex++;
                        ExcelHelper.SetExcelValue( worksheet.Cells[rowCounter, columnIndex], item.RegionNumber );
                        ExcelHelper.FinalizeColumnFormat( worksheet, columnIndex, item.RegionNumber );
                    }

                    if ( exportColumns.SuperFundNumber )
                    {
                        columnIndex++;
                        ExcelHelper.SetExcelValue( worksheet.Cells[rowCounter, columnIndex], item.SuperFundNumber );
                        ExcelHelper.FinalizeColumnFormat( worksheet, columnIndex, item.SuperFundNumber );
                    }

                    if ( exportColumns.FundNumber )
                    {
                        columnIndex++;
                        ExcelHelper.SetExcelValue( worksheet.Cells[rowCounter, columnIndex], item.FundNumber );
                        ExcelHelper.FinalizeColumnFormat( worksheet, columnIndex, item.FundNumber );
                    }

                    if ( exportColumns.LocationNumber )
                    {
                        columnIndex++;
                        ExcelHelper.SetExcelValue( worksheet.Cells[rowCounter, columnIndex], item.LocationNumber );
                        ExcelHelper.FinalizeColumnFormat( worksheet, columnIndex, item.LocationNumber );
                    }

                    if ( exportColumns.CostCenterNumber )
                    {
                        columnIndex++;
                        ExcelHelper.SetExcelValue( worksheet.Cells[rowCounter, columnIndex], item.CostCenterNumber );
                        ExcelHelper.FinalizeColumnFormat( worksheet, columnIndex, item.CostCenterNumber );
                    }

                    if ( exportColumns.DepartmentNumber )
                    {
                        columnIndex++;
                        ExcelHelper.SetExcelValue( worksheet.Cells[rowCounter, columnIndex], item.DepartmentNumber );
                        ExcelHelper.FinalizeColumnFormat( worksheet, columnIndex, item.DepartmentNumber );
                    }

                    if ( exportColumns.AccountNumber )
                    {
                        columnIndex++;
                        ExcelHelper.SetExcelValue( worksheet.Cells[rowCounter, columnIndex], item.AccountNumber );
                        ExcelHelper.FinalizeColumnFormat( worksheet, columnIndex, item.AccountNumber );
                    }

                    if ( exportColumns.AccountSub )
                    {
                        columnIndex++;
                        ExcelHelper.SetExcelValue( worksheet.Cells[rowCounter, columnIndex], item.AccountSub );
                        ExcelHelper.FinalizeColumnFormat( worksheet, columnIndex, item.AccountSub );
                    }

                    if ( exportColumns.Project )
                    {
                        columnIndex++;
                        ExcelHelper.SetExcelValue( worksheet.Cells[rowCounter, columnIndex], item.Project );
                        ExcelHelper.FinalizeColumnFormat( worksheet, columnIndex, item.Project );
                    }

                    if ( exportColumns.Note )
                    {
                        columnIndex++;
                        ExcelHelper.SetExcelValue( worksheet.Cells[rowCounter, columnIndex], item.Note );
                        ExcelHelper.FinalizeColumnFormat( worksheet, columnIndex, item.Note );
                    }
                }
            }
            else
            {
                rowCounter++;

                var columnIndex = 1;
                ExcelHelper.SetExcelValue( worksheet.Cells[rowCounter, columnIndex], string.Empty );
                ExcelHelper.FinalizeColumnFormat( worksheet, columnIndex, string.Empty );
            }

            var range = worksheet.Cells[headerRows, 1, rowCounter, columnCounter];
            var table = worksheet.Tables.Add( range, title );

            // ensure each column in the table has a unique name
            var columnNames = worksheet.Cells[headerRows, 1, headerRows, columnCounter].Select( a => new { OrigColumnName = a.Text, Cell = a } ).ToList();
            columnNames.Reverse();
            foreach ( var col in columnNames )
            {
                int duplicateSuffix = 0;
                string uniqueName = col.OrigColumnName;

                // increment the suffix by 1 until there is only one column with that name
                while ( columnNames.Where( a => a.Cell.Text == uniqueName ).Count() > 1 )
                {
                    duplicateSuffix++;
                    uniqueName = col.OrigColumnName + duplicateSuffix.ToString();
                    col.Cell.Value = uniqueName;
                }
            }

            table.ShowHeader = true;
            table.ShowFilter = true;
            table.TableStyle = OfficeOpenXml.Table.TableStyles.None;

            // Format header range
            using ( ExcelRange r = worksheet.Cells[headerRows, 1, headerRows, columnCounter] )
            {
                r.Style.Font.Bold = true;
                r.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
            }

            // do AutoFitColumns on no more than the first 10000 rows (10000 can take 4-5 seconds, but could take several minutes if there are 100000+ rows )
            int autoFitRows = Math.Min( rowCounter, 10000 );
            var autoFitRange = worksheet.Cells[headerRows, 1, autoFitRows, columnCounter];

            autoFitRange.AutoFitColumns();

            // set some footer text
            worksheet.HeaderFooter.OddHeader.CenteredText = title;
            worksheet.HeaderFooter.OddFooter.RightAlignedText = string.Format( "Page {0} of {1}", ExcelHeaderFooter.PageNumber, ExcelHeaderFooter.NumberOfPages );

            return excel;
        }

        private ExportColumns GetExportColumns( List<GLExcelLine> items )
        {
            var exportColumns = new ExportColumns();

            foreach ( var item in items )
            {
                if ( !string.IsNullOrWhiteSpace( item.CompanyNumber ) )
                {
                    exportColumns.CompanyNumber = true;
                }

                if ( !string.IsNullOrWhiteSpace( item.RegionNumber ) )
                {
                    exportColumns.RegionNumber = true;
                }

                if ( !string.IsNullOrWhiteSpace( item.SuperFundNumber ) )
                {
                    exportColumns.SuperFundNumber = true;
                }

                if ( !string.IsNullOrWhiteSpace( item.FundNumber ) )
                {
                    exportColumns.FundNumber = true;
                }

                if ( !string.IsNullOrWhiteSpace( item.LocationNumber ) )
                {
                    exportColumns.LocationNumber = true;
                }

                if ( !string.IsNullOrWhiteSpace( item.CostCenterNumber ) )
                {
                    exportColumns.CostCenterNumber = true;
                }

                if ( !string.IsNullOrWhiteSpace( item.DepartmentNumber ) )
                {
                    exportColumns.DepartmentNumber = true;
                }

                if ( !string.IsNullOrWhiteSpace( item.AccountNumber ) )
                {
                    exportColumns.AccountNumber = true;
                }

                if ( !string.IsNullOrWhiteSpace( item.AccountSub ) )
                {
                    exportColumns.AccountSub = true;
                }

                if ( !string.IsNullOrWhiteSpace( item.Project ) )
                {
                    exportColumns.Project = true;
                }

                if ( !string.IsNullOrWhiteSpace( item.Note ) )
                {
                    exportColumns.Note = true;
                }
            }

            return exportColumns;
        }

        public class GLExcelLine
        {
            public string CompanyNumber;
            public string RegionNumber;
            public string SuperFundNumber;
            public string FundNumber;
            public string LocationNumber;
            public string CostCenterNumber;
            public string DepartmentNumber;
            public string AccountNumber;
            public string AccountSub;
            public decimal Amount;
            public string Project;
            public int JournalNumber;
            public string JournalDescription;
            public DateTime Date;
            public string Note;
            public int Period;
            public string JournalCode;
        }

        public class ExportColumns
        {
            public bool CompanyNumber;
            public bool RegionNumber;
            public bool SuperFundNumber;
            public bool FundNumber;
            public bool LocationNumber;
            public bool CostCenterNumber;
            public bool DepartmentNumber;
            public bool AccountNumber;
            public bool AccountSub;
            public bool Project;
            public bool Note;
        }
    }

    public enum GLAccountGroupingMode
    {
        DebitAndCreditLines = 0,
        DebitLinesOnly = 1,
        CreditLinesOnly = 2,
        DebitAndCreditByFinancialAccount = 3,
        NoGrouping = 4
    }
}
