using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Web;

using Rock;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;
using Rock.Utility;
using OfficeOpenXml;
using System.Data;
using System.ComponentModel;

namespace com.kfs.ShelbyBatchExport
{
    public class SFJournal
    {
        public List<GLExcelLine> GetGLExcelLines( RockContext rockContext, FinancialBatch financialBatch, string journalCode, int period )
        {
            var glExcelLines = new List<GLExcelLine>();
            var glEntries = GetGlEntries( rockContext, financialBatch, journalCode, period );
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

        private List<JournalEntryLine> GetGlEntries( RockContext rockContext, FinancialBatch financialBatch, string journalCode, int period )
        {
            //
            // Group/Sum Transactions by Account and Project since Project can come from Account or Transaction Details
            //
            var batchTransactions = new List<GLTransaction>();
            foreach ( var transaction in financialBatch.Transactions )
            {
                transaction.LoadAttributes();
                foreach ( var transactionDetail in transaction.TransactionDetails )
                {
                    transactionDetail.LoadAttributes();
                    transactionDetail.Account.LoadAttributes();

                    var detailProject = transactionDetail.GetAttributeValue( "com.kfs.ShelbyFinancials.Project" ).AsGuidOrNull();
                    var transactionProject = transaction.GetAttributeValue( "com.kfs.ShelbyFinancials.Project" ).AsGuidOrNull();
                    var accountProject = transactionDetail.Account.GetAttributeValue( "com.kfs.ShelbyFinancials.Project" ).AsGuidOrNull();

                    var projectCode = string.Empty;
                    if ( detailProject != null )
                    {
                        projectCode = DefinedValueCache.Get( ( Guid ) detailProject ).Value;
                    }
                    else if ( transactionProject != null )
                    {
                        projectCode = DefinedValueCache.Get( ( Guid ) transactionProject ).Value;
                    }
                    else if ( accountProject != null )
                    {
                        projectCode = DefinedValueCache.Get( ( Guid ) accountProject ).Value;
                    }

                    var transactionItem = new GLTransaction()
                    {
                        Amount = transactionDetail.Amount,
                        FinancialAccountId = transactionDetail.AccountId,
                        Project = projectCode
                    };

                    batchTransactions.Add( transactionItem );
                }
            }

            var batchTransactionsSummary = batchTransactions
                .GroupBy( d => new { d.FinancialAccountId, d.Project } )
                .Select( s => new GLTransaction
                {
                    FinancialAccountId = s.Key.FinancialAccountId,
                    Project = s.Key.Project,
                    Amount = s.Sum( f => ( decimal? ) f.Amount ) ?? 0.0M
                } )
                .ToList();

            var batchSummary = new List<GLBatchTotals>();
            foreach ( var summary in batchTransactionsSummary )
            {
                var account = new FinancialAccountService( rockContext ).Get( summary.FinancialAccountId );
                account.LoadAttributes();

                var batchSummaryItem = new GLBatchTotals()
                {
                    CompanyNumber = account.GetAttributeValue( "com.kfs.ShelbyFinancials.Company" ),
                    RegionNumber = account.GetAttributeValue( "com.kfs.ShelbyFinancials.Region" ),
                    SuperFundNumber = account.GetAttributeValue( "com.kfs.ShelbyFinancials.SuperFund" ),
                    FundNumber = account.GetAttributeValue( "com.kfs.ShelbyFinancials.Fund" ),
                    LocationNumber = account.GetAttributeValue( "com.kfs.ShelbyFinancials.Location" ),
                    CostCenterNumber = account.GetAttributeValue( "com.kfs.ShelbyFinancials.CostCenter" ),
                    DepartmentNumber = account.GetAttributeValue( "com.kfs.ShelbyFinancials.Department" ),
                    CreditAccountNumber = account.GetAttributeValue( "com.kfs.ShelbyFinancials.CreditAccount" ),
                    DebitAccountNumber = account.GetAttributeValue( "com.kfs.ShelbyFinancials.DebitAccount" ),
                    AccountSub = account.GetAttributeValue( "com.kfs.ShelbyFinancials.AccountSub" ),
                    Amount = summary.Amount,
                    Project = summary.Project,
                    JournalNumber = financialBatch.Id,
                    JournalDescription = string.Format("{0}: {1}", financialBatch.Id, financialBatch.Name),
                    Date = financialBatch.BatchStartDateTime ?? RockDateTime.Now,
                    Note = financialBatch.Note
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
                var creditLine = new JournalEntryLine()
                {
                    CompanyNumber = transaction.CompanyNumber,
                    RegionNumber = transaction.RegionNumber,
                    SuperFundNumber = transaction.SuperFundNumber,
                    FundNumber = transaction.FundNumber,
                    LocationNumber = transaction.LocationNumber,
                    CostCenterNumber = transaction.CostCenterNumber,
                    DepartmentNumber = transaction.DepartmentNumber,
                    AccountNumber = transaction.CreditAccountNumber,
                    AccountSub = transaction.AccountSub,
                    Amount = transaction.Amount * -1,
                    Project = transaction.Project,
                    JournalNumber = transaction.JournalNumber,
                    JournalDescription = transaction.JournalDescription,
                    Date = transaction.Date,
                    Note = transaction.Note
                };

                returnList.Add( creditLine );

                var debitLine = new JournalEntryLine()
                {
                    CompanyNumber = transaction.CompanyNumber,
                    RegionNumber = transaction.RegionNumber,
                    SuperFundNumber = transaction.SuperFundNumber,
                    FundNumber = transaction.FundNumber,
                    LocationNumber = transaction.LocationNumber,
                    CostCenterNumber = transaction.CostCenterNumber,
                    DepartmentNumber = "0",
                    AccountNumber = transaction.DebitAccountNumber,
                    AccountSub = transaction.AccountSub,
                    Amount = transaction.Amount,
                    Project = transaction.Project,
                    JournalNumber = transaction.JournalNumber,
                    JournalDescription = transaction.JournalDescription,
                    Date = transaction.Date,
                    Note = transaction.Note
                };

                returnList.Add( debitLine );
            }

            return returnList;
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

            //ShelbyFormatWorksheet( worksheet, headerRows, rowCounter, columnCounter );
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

        public class GLTransaction
        {
            public decimal Amount;
            public int FinancialAccountId;
            public string Project;
        }

        public class GLBatchTotals
        {
            public string CompanyNumber;
            public string RegionNumber;
            public string SuperFundNumber;
            public string FundNumber;
            public string LocationNumber;
            public string CostCenterNumber;
            public string DepartmentNumber;
            public string CreditAccountNumber;
            public string DebitAccountNumber;
            public string AccountSub;
            public decimal Amount;
            public string Project;
            public int JournalNumber;
            public string JournalDescription;
            public DateTime Date;
            public string Note;
        }

        public class JournalEntryLine
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
}
