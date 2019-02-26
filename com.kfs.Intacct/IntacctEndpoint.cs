using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml;
using System.Xml.Linq;
using Rock;
using Rock.Data;
using Rock.Model;

namespace com.kfs.Intacct
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

        public XmlDocument PostToIntacct( XmlDocument xmlDoc )
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

                string sXml = xmlDoc.InnerXml;
                req.ContentLength = sXml.Length;
                var sw = new StreamWriter( req.GetRequestStream() );
                sw.Write( sXml );
                sw.Close();

                res = ( HttpWebResponse ) req.GetResponse();
                Stream responseStream = res.GetResponseStream();
                var streamReader = new StreamReader( responseStream );

                //Read the response into an xml document
                var xml = new XmlDocument();
                xml.LoadXml( streamReader.ReadToEnd() );

                //return only the xml representing the response details (inner request)
                response = xml;
            }
            catch ( Exception ex )
            {
            }

            return response;
        }

        public bool ParseEndpointResponse( XmlDocument xmlDocument, int BatchId, bool Log = false )
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

            return false;
        }
    }
}