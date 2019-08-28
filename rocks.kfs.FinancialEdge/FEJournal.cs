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
using System.Linq;
using System.Text;
using System.Web;

using Rock;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;

namespace rocks.kfs.FinancialEdge
{
    public class FEJournal
    {
        public List<JournalEntryLine> GetGlEntries( RockContext rockContext, FinancialBatch financialBatch, string journal, string encumbranceStatus = "Regular" )
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

                    var detailProject = transactionDetail.GetAttributeValue( "rocks.kfs.FinancialEdge.PROJECTID" ).AsGuidOrNull();
                    var accountProject = transactionDetail.Account.GetAttributeValue( "rocks.kfs.FinancialEdge.PROJECTID" ).AsGuidOrNull();

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
                        CreditAccount = transactionDetail.Account.GetAttributeValue( "rocks.kfs.FinancialEdge.ACCOUNTNO" ),
                        DebitAccount = transactionDetail.Account.GetAttributeValue( "rocks.kfs.FinancialEdge.DEBITACCOUNTNO" ),
                        Project = projectCode,
                        FinancialAccountId = transactionDetail.Account.Id,
                        DebitProject = transactionDetail.Account.GetAttributeValue( "rocks.kfs.FinancialEdge.DEBITPROJECTID" ),
                    };

                    batchTransactions.Add( transactionItem );
                }
            }

            var creditLines = batchTransactions
                .GroupBy( d => new { d.CreditAccount, d.Project } )
                .Select( s => new GLTransactionLine
                {
                    Account = s.Key.CreditAccount,
                    Project = s.Key.Project,
                    Amount = s.Sum( f => ( decimal? ) f.Amount ) ?? 0.0M
                } )
                .ToList();

            var debitLines = batchTransactions
                .GroupBy( d => new { d.DebitAccount, d.DebitProject } )
                .Select( s => new GLTransactionLine
                {
                    Account = s.Key.DebitAccount,
                    Project = s.Key.DebitProject,
                    Amount = s.Sum( f => ( decimal? ) f.Amount ) ?? 0.0M,
                } )
                .ToList();

            var batchName = string.Format( "{0} ({1})", financialBatch.Name, financialBatch.Id );

            return GenerateLineItems( rockContext, creditLines, debitLines, financialBatch.BatchStartDateTime, journal, batchName, encumbranceStatus );
        }

        private List<JournalEntryLine> GenerateLineItems( RockContext rockContext, List<GLTransactionLine> creditLines, List<GLTransactionLine> debitLines, DateTime? batchDate, string journal, string batchName, string encumbranceStatus )
        {
            var journalReference = batchName;

            var returnList = new List<JournalEntryLine>();

            foreach ( var transaction in creditLines )
            {
                var creditLine = new JournalEntryLine()
                {
                    AccountNumber = transaction.Account,
                    PostDate = batchDate ?? RockDateTime.Now,
                    EncumbranceStatus = encumbranceStatus,
                    Type = transaction.Amount >= 0 ? "C" : "D",
                    Journal = journal,
                    JournalReference = journalReference,
                    Amount = transaction.Amount >= 0 ? transaction.Amount : ( transaction.Amount * -1 ),
                    ProjectId = DefinedValueCache.Get( transaction.Project.AsGuid() )?.Value
                };

                returnList.Add( creditLine );
            }

            foreach ( var transaction in debitLines )
            {
                var debitLine = new JournalEntryLine()
                {
                    AccountNumber = transaction.Account,
                    PostDate = batchDate ?? RockDateTime.Now,
                    EncumbranceStatus = encumbranceStatus,
                    Type = transaction.Amount >= 0 ? "D" : "C",
                    Journal = journal,
                    JournalReference = journalReference,
                    Amount = transaction.Amount >= 0 ? transaction.Amount : ( transaction.Amount * -1 ),
                    ProjectId = DefinedValueCache.Get( transaction.Project.AsGuid() )?.Value
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
        public string DebitProject;
    }

    public class GLTransactionLine
    {
        public decimal Amount;
        public string Account;
        public string Project;
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
}
