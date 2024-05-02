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
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

using Rock;
using Rock.Data;
using Rock.Model;

namespace rocks.kfs.Intacct
{
    public class IntacctEndpoint
    {
        public const string DefaultEndpoint = "https://api.intacct.com/ia/xml/xmlgw.phtml";

        public const string EndpointUrlEnvName = "INTACCT_ENDPOINT_URL";

        public const string DomainName = "intacct.com";

        private string _url;

        public string Url
        {
            get
            {
                return this._url;
            }
            set
            {
                Uri uri;
                if ( string.IsNullOrWhiteSpace( value ) )
                {
                    value = "https://api.intacct.com/ia/xml/xmlgw.phtml";
                }
                if ( !Uri.TryCreate( value, UriKind.Absolute, out uri ) )
                {
                    throw new ArgumentException( "Endpoint URL is not a valid URL" );
                }
                if ( !uri.Host.EndsWith( ".intacct.com" ) )
                {
                    throw new ArgumentException( "Endpoint URL is not a valid intacct.com domain name" );
                }
                this._url = value;
            }
        }

        public IntacctEndpoint()
        {
            this.Url = ( Environment.GetEnvironmentVariable( "INTACCT_ENDPOINT_URL" ) );
        }

        public override string ToString()
        {
            return this.Url;
        }

        public XmlDocument PostToIntacct( XmlDocument xmlDoc, bool Log = false )
        {
            var response = new XmlDocument();

            HttpWebRequest req = null;
            HttpWebResponse res = null;
            try
            {
                var endpoint = new IntacctEndpoint();
                req = ( HttpWebRequest ) WebRequest.Create( endpoint.Url );
                req.Method = "POST";
                req.ContentType = "application/xml; charset=utf-8";

                if ( Log )
                {
                    ExceptionLogService.LogException( $"Intacct Request: {Regex.Replace( Regex.Replace( xmlDoc.InnerXml, "<login>.*</login>", "<login>HIDDEN</login>" ), "<password>.*</password>", "<password>HIDDEN</password>" )}" );
                }

                string sXml = xmlDoc.InnerXml;
                req.ContentLength = sXml.Length;
                var sw = new StreamWriter( req.GetRequestStream() );
                sw.Write( sXml );
                sw.Close();

                res = ( HttpWebResponse ) req.GetResponse();
                Stream responseStream = res.GetResponseStream();
                var streamReader = new StreamReader( responseStream );
                var responseString = streamReader.ReadToEnd();

                //Read the response into an xml document
                var xml = new XmlDocument();
                xml.LoadXml( responseString );

                //return only the xml representing the response details (inner request)
                response = xml;

                if ( Log )
                {
                    ExceptionLogService.LogException( $"Intacct Response: {responseString}" );
                }
            }
            catch ( Exception ex )
            {
                ExceptionLogService.LogException( ex );

                if ( Log )
                {
                    ExceptionLogService.LogException( $"Intacct Exception generated with this request: {xmlDoc.InnerXml}" );
                }
            }

            return response;
        }

        public bool ParseEndpointResponse( XmlDocument xmlDocument, int BatchId, bool Log = false )
        {
            try
            {
                var resultX = XDocument.Load( new XmlNodeReader( xmlDocument ) );

                if ( Log )
                {
                    var financialBatch = new FinancialBatchService( new RockContext() ).Get( BatchId );
                    var changes = new History.HistoryChangeList();
                    var oldValue = string.Empty;
                    var newValue = resultX.ToString();

                    History.EvaluateChange( changes, "Intacct Response", oldValue, newValue );

                    var rockContext = new RockContext();
                    rockContext.WrapTransaction( () =>
                    {
                        if ( changes.Any() )
                        {
                            HistoryService.SaveChanges(
                                rockContext,
                                typeof( FinancialBatch ),
                                Rock.SystemGuid.Category.HISTORY_FINANCIAL_BATCH.AsGuid(),
                                BatchId,
                                changes );
                        }
                    } );
                }

                var xResponseXml = resultX.Elements( "response" ).FirstOrDefault();
                if ( xResponseXml != null )
                {
                    var xOperationXml = xResponseXml.Elements( "operation" ).FirstOrDefault();
                    if ( xOperationXml != null )
                    {
                        var xResultXml = xOperationXml.Elements( "result" ).FirstOrDefault();
                        if ( xResultXml != null )
                        {
                            var xStatusXml = xResultXml.Elements( "status" ).FirstOrDefault();
                            if ( xStatusXml != null && xStatusXml.Value == "success" )
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            catch ( Exception e )
            {
                ExceptionLogService.LogException( e );
            }

            return false;
        }

        public List<CheckingAccount> ParseListCheckingAccountsResponse( XmlDocument xmlDocument, int BatchId )
        {
            var bankAccountList = new List<CheckingAccount>();
            var resultX = XDocument.Load( new XmlNodeReader( xmlDocument ) );

            var xResponseXml = resultX.Elements( "response" ).FirstOrDefault();
            if ( xResponseXml != null )
            {
                var xOperationXml = xResponseXml.Elements( "operation" ).FirstOrDefault();
                if ( xOperationXml != null )
                {
                    var xResultXml = xOperationXml.Elements( "result" ).FirstOrDefault();
                    if ( xResultXml != null )
                    {
                        var xStatusXml = xResultXml.Elements( "status" ).FirstOrDefault();
                        if ( xStatusXml != null && xStatusXml.Value == "success" )
                        {
                            var xDataXml = xResultXml.Elements( "data" ).FirstOrDefault();
                            if ( xDataXml != null )
                            {
                                var xCheckingAccountsXml = xDataXml.Elements( "CHECKINGACCOUNT" );
                                if ( xCheckingAccountsXml != null )
                                {
                                    foreach ( var xAcctXml in xCheckingAccountsXml )
                                    {
                                        var bankAccount = new CheckingAccount();
                                        if ( xAcctXml.Elements( "BANKACCOUNTID" ).FirstOrDefault() != null )
                                        {
                                            bankAccount.BankAccountId = xAcctXml.Elements( "BANKACCOUNTID" ).FirstOrDefault().Value;
                                        }
                                        if ( xAcctXml.Elements( "BANKACCOUNTNO" ).FirstOrDefault() != null )
                                        {
                                            bankAccount.BankAcountNo = xAcctXml.Elements( "BANKACCOUNTNO" ).FirstOrDefault().Value;
                                        }
                                        if ( xAcctXml.Elements( "GLACCOUNTNO" ).FirstOrDefault() != null )
                                        {
                                            bankAccount.GLAccountNo = xAcctXml.Elements( "GLACCOUNTNO" ).FirstOrDefault().Value;
                                        }
                                        if ( xAcctXml.Elements( "BANKNAME" ).FirstOrDefault() != null )
                                        {
                                            bankAccount.BankName = xAcctXml.Elements( "BANKNAME" ).FirstOrDefault().Value;
                                        }
                                        bankAccountList.Add( bankAccount );
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return bankAccountList;
        }
    }
}
