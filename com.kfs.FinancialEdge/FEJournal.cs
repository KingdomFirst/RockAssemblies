using System;
using System.Collections.Generic;
using System.Linq;

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

            return GenerateLineItems( rockContext, batchSummary, financialBatch.BatchStartDateTime, journal, financialBatch.Name, referenceStyle, encumbranceStatus );
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
                    var projectName = string.IsNullOrWhiteSpace( project.Description ) ? string.Empty : string.Format( " - {0}", project.Description );
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