using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

using Rock;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;

namespace com.kfs.Intacct
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
        public XmlDocument CreateJournalEntryXML( IntacctAuth AuthCreds, int BatchId, string JournalId )
        {
            var doc = new XmlDocument();
            var financialBatch = new FinancialBatchService( new RockContext() ).Get( BatchId );

            if ( financialBatch.Id > 0 )
            {
                var lines = GetGlEntries( financialBatch );
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

        private List<JournalEntryLine> GetGlEntries( FinancialBatch Batch )
        {
            var customDimensions = GetCustomDimensions();

            var batchTransactions = new List<GLTransaction>();

            Batch.LoadAttributes();
            foreach ( var transaction in Batch.Transactions )
            {
                transaction.LoadAttributes();
                foreach ( var transactionDetail in transaction.TransactionDetails )
                {
                    transactionDetail.LoadAttributes();
                    transactionDetail.Account.LoadAttributes();

                    var detailProject = transactionDetail.GetAttributeValue( "com.kfs.Intacct.PROJECTID" ).AsGuidOrNull();
                    var accountProject = transactionDetail.Account.GetAttributeValue( "com.kfs.Intacct.PROJECTID" ).AsGuidOrNull();

                    var projectCode = string.Empty;
                    if ( detailProject != null )
                    {
                        projectCode = DefinedValueCache.Read( ( Guid ) detailProject ).Value;
                    }
                    else if ( accountProject != null )
                    {
                        projectCode = DefinedValueCache.Read( ( Guid ) accountProject ).Value;
                    }

                    var customDimensionValues = new Dictionary<string, dynamic>();

                    if ( customDimensions.Count > 0 )
                    {
                        foreach ( var rockKey in customDimensions )
                        {
                            var dimension = rockKey.Split( '.' ).Last();
                            customDimensionValues.Add( dimension, transactionDetail.Account.GetAttributeValue( rockKey ) );
                        }
                    }

                    var transactionItem = new GLTransaction()
                    {
                        Amount = transactionDetail.Amount,
                        CreditAccount = transactionDetail.Account.GetAttributeValue( "com.kfs.Intacct.ACCOUNTNO" ),
                        DebitAccount = transactionDetail.Account.GetAttributeValue( "com.kfs.Intacct.DEBITACCOUNTNO" ),
                        Class = transactionDetail.Account.GetAttributeValue( "com.kfs.Intacct.CLASSID" ),
                        Department = transactionDetail.Account.GetAttributeValue( "com.kfs.Intacct.DEPARTMENT" ),
                        Location = transactionDetail.Account.GetAttributeValue( "com.kfs.Intacct.LOCATION" ),
                        Project = projectCode,
                        CustomDimensions = customDimensionValues
                    };

                    batchTransactions.Add( transactionItem );
                }
            }

            var groupedTransactions = batchTransactions
                .GroupBy( d => new { d.DebitAccount, d.Class, d.Department, d.CreditAccount, d.Location, d.Project, d.CustomDimensions } )
                .Select( t => new GLTransaction
                {
                    DebitAccount = t.Key.DebitAccount,
                    Class = t.Key.Class,
                    Department = t.Key.Department,
                    CreditAccount = t.Key.CreditAccount,
                    Location = t.Key.Location,
                    Project = t.Key.Project,
                    Amount = t.Sum( f => ( decimal? ) f.Amount ) ?? 0.0M,
                    CustomDimensions = t.Key.CustomDimensions
                } )
                .ToList();

            return GenerateLineItems( groupedTransactions );
        }

        private List<string> GetCustomDimensions()
        {
            var knownDimensions = new List<string>();
            knownDimensions.Add( "com.kfs.Intacct.ACCOUNTNO" );
            knownDimensions.Add( "com.kfs.Intacct.DEBITACCOUNTNO" );
            knownDimensions.Add( "com.kfs.Intacct.CLASSID" );
            knownDimensions.Add( "com.kfs.Intacct.DEPARTMENT" );
            knownDimensions.Add( "com.kfs.Intacct.LOCATION" );
            knownDimensions.Add( "com.kfs.Intacct.PROJECTID" );

            var rockContext = new RockContext();
            var accountCategoryId = new CategoryService( rockContext ).Queryable().FirstOrDefault( c => c.Guid.Equals( new System.Guid( "7361A954-350A-41F1-9D94-AD2CF4030CA5" ) ) ).Id;
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

        private List<JournalEntryLine> GenerateLineItems( List<GLTransaction> transactionItems )
        {
            var returnList = new List<JournalEntryLine>();
            foreach ( var transaction in transactionItems )
            {
                var creditLine = new JournalEntryLine()
                {
                    GlAccountNumber = transaction.CreditAccount,
                    TransactionAmount = transaction.Amount * -1,
                    ClassId = transaction.Class,
                    DepartmentId = transaction.Department,
                    LocationId = transaction.Location,
                    ProjectId = transaction.Project,
                    CustomFields = transaction.CustomDimensions
                };

                returnList.Add( creditLine );

                var debitLine = new JournalEntryLine()
                {
                    GlAccountNumber = transaction.DebitAccount,
                    TransactionAmount = transaction.Amount,
                    ClassId = transaction.Class,
                    DepartmentId = transaction.Department,
                    LocationId = transaction.Location,
                    ProjectId = transaction.Project,
                    CustomFields = transaction.CustomDimensions
                };

                returnList.Add( debitLine );
            }

            return returnList;
        }
    }
}