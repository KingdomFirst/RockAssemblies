using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

using Rock;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;

namespace com.kfs.FinancialEdge
{
    public class FEJournal
    {
        public List<JournalEntryLine> GetGlEntries( RockContext rockContext, FinancialBatch financialBatch, string journal, ReferenceStyle referenceStyle, string encumbranceStatus = "Regular" )
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

                    var detailProject = transactionDetail.GetAttributeValue( "com.kfs.FinancialEdge.PROJECTID" ).AsGuidOrNull();
                    var accountProject = transactionDetail.Account.GetAttributeValue( "com.kfs.FinancialEdge.PROJECTID" ).AsGuidOrNull();

                    var projectCode = string.Empty;
                    if ( detailProject != null )
                    {
                        projectCode = detailProject.ToString();
                    }
                    else if ( accountProject != null )
                    {
                        projectCode = accountProject.ToString();
                    }

                    var transactionItem = new GLTransaction()
                    {
                        Amount = transactionDetail.Amount,
                        CreditAccount = transactionDetail.Account.GetAttributeValue( "com.kfs.FinancialEdge.ACCOUNTNO" ),
                        DebitAccount = transactionDetail.Account.GetAttributeValue( "com.kfs.FinancialEdge.DEBITACCOUNTNO" ),
                        Project = projectCode,
                        FinancialAccountId = transactionDetail.Account.Id
                    };

                    batchTransactions.Add( transactionItem );
                }
            }

            var batchSummary = batchTransactions
                .GroupBy( d => new { d.FinancialAccountId, d.CreditAccount, d.DebitAccount, d.Project } )
                .Select( s => new GLTransaction
                {
                    FinancialAccountId = s.Key.FinancialAccountId,
                    CreditAccount = s.Key.CreditAccount,
                    DebitAccount = s.Key.DebitAccount,
                    Project = s.Key.Project,
                    Amount = s.Sum( f => ( decimal? ) f.Amount ) ?? 0.0M
                } )
                .ToList();

            var batchName = string.Format( "{0} ({1})", financialBatch.Name, financialBatch.Id );

            return GenerateLineItems( rockContext, batchSummary, financialBatch.BatchStartDateTime, journal, batchName, referenceStyle, encumbranceStatus );
        }

        private List<JournalEntryLine> GenerateLineItems( RockContext rockContext, List<GLTransaction> transactionItems, DateTime? batchDate, string journal, string batchName, ReferenceStyle referenceStyle, string encumbranceStatus )
        {
            var journalReference = batchName;

            var returnList = new List<JournalEntryLine>();
            foreach ( var transaction in transactionItems )
            {
                if ( referenceStyle.Equals( ReferenceStyle.AccountName ) )
                {
                    var accountName = new FinancialAccountService( rockContext ).Get( transaction.FinancialAccountId ).Name;
                    var project = DefinedValueCache.Read( transaction.Project.AsGuid() );
                    var projectName = string.Empty;
                    if ( project != null )
                    {
                        if ( !string.IsNullOrWhiteSpace( project.Description ) )
                        {
                            projectName = string.Format( " - {0}", project.Description );
                        }
                        else
                        {
                            projectName = string.Format( " - {0}", project.Value );
                        }
                    }
                    journalReference = string.Format( "{0}{1}", accountName, projectName );
                }
                var creditLine = new JournalEntryLine()
                {
                    AccountNumber = transaction.CreditAccount,
                    PostDate = batchDate ?? RockDateTime.Now,
                    EncumbranceStatus = encumbranceStatus,
                    Type = "C",
                    Journal = journal,
                    JournalReference = journalReference,
                    Amount = transaction.Amount,
                    ProjectId = DefinedValueCache.Read( transaction.Project.AsGuid() ).Value
                };

                returnList.Add( creditLine );

                var debitLine = new JournalEntryLine()
                {
                    AccountNumber = transaction.DebitAccount,
                    PostDate = batchDate ?? RockDateTime.Now,
                    EncumbranceStatus = encumbranceStatus,
                    Type = "D",
                    Journal = journal,
                    JournalReference = journalReference,
                    Amount = transaction.Amount,
                    ProjectId = DefinedValueCache.Read( transaction.Project.AsGuid() ).Value
                };

                returnList.Add( debitLine );
            }

            return returnList;
        }

        public void SetFinancialEdgeSessions( List<JournalEntryLine> items, string fileId )
        {
            if ( HttpContext.Current.Session["FinancialEdgeCsvExport"] != null )
            {
                HttpContext.Current.Session["FinancialEdgeCsvExport"] = string.Empty;
            }
            if ( HttpContext.Current.Session["FinancialEdgeFileId"] != null )
            {
                HttpContext.Current.Session["FinancialEdgeFileId"] = string.Empty;
            }

            var output = new StringBuilder();
            output.Append( "Account number, Post Date, Encumbrance Status, Type, Journal, Journal Reference, Amount, Project ID" );
            var num = 0;
            foreach ( var item in items )
            {
                output.Append( Environment.NewLine );
                output.Append( string.Format( "{0},{1},{2},{3},{4},\"{5}\",{6},{7}", item.AccountNumber, item.PostDate.ToString( "MM/dd/yyyy" ), item.EncumbranceStatus, item.Type, item.Journal, item.JournalReference, item.Amount, item.ProjectId ) );
                num++;
            }
            HttpContext.Current.Session["FinancialEdgeCsvExport"] = output.ToString();
            HttpContext.Current.Session["FinancialEdgeFileId"] = fileId;
        }
    }

    public class GLTransaction
    {
        public decimal Amount;
        public string CreditAccount;
        public string DebitAccount;
        public string Project;
        public int FinancialAccountId;
    }

    public class JournalEntryLine
    {
        public string AccountNumber;
        public DateTime PostDate;
        public string EncumbranceStatus;
        public string Type;
        public string Journal;
        public string JournalReference;
        public decimal? Amount;
        public string ProjectId;
    }

    public enum ReferenceStyle
    {
        BatchName,
        AccountName
    }
}