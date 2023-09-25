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
using KFSConst = rocks.kfs.Intacct.SystemGuid;

namespace rocks.kfs.Intacct
{
    public class IntacctCheckingAccountList
    {
        /// <summary>
        /// Creates the XML to submit to Intacct to capture a list of Bank Accounts
        /// </summary>
        /// <param name="AuthCreds">The IntacctAuth object with authentication. <see cref="IntacctAuth"/></param>
        /// <returns>Returns the XML needed to request a list of Bank Accounts.</returns>
        public XmlDocument GetBankAccountsXML( IntacctAuth AuthCreds, int batchId, List<string> fields = null )
        {
            var doc = new XmlDocument();
            using ( var writer = doc.CreateNavigator().AppendChild() )
            {
                writer.WriteStartDocument();
                writer.WriteStartElement( "request" );
                writer.WriteStartElement( "control" );
                writer.WriteElementString( "senderid", AuthCreds.SenderId );
                writer.WriteElementString( "password", AuthCreds.SenderPassword );
                writer.WriteElementString( "controlid", $"RequestControl_{batchId}" );
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
                writer.WriteAttributeString( "controlid", $"Batch_{batchId}" );
                writer.WriteStartElement( "query" );
                writer.WriteElementString( "object", "CHECKINGACCOUNT" );
                writer.WriteStartElement( "select" );
                if ( fields == null )
                {
                    fields = new List<string>();
                    fields.Add( "BANKACCOUNTID" );
                    fields.Add( "BANKNAME" );
                    fields.Add( "BANKACCOUNTNO" );
                    fields.Add( "GLACCOUNTNO" );
                }
                foreach( var field in fields )
                {
                    writer.WriteElementString( "field", field );
                }
                writer.WriteEndElement();  // close select
                writer.WriteEndElement();  // close query
                writer.WriteEndElement();  // close function
                writer.WriteEndElement();  // close content
                writer.WriteEndElement();  // close operation
                writer.WriteEndElement();  // close request
                writer.WriteEndDocument(); // close document
            }

            XmlDeclaration xmldecl;
            xmldecl = doc.CreateXmlDeclaration( "1.0", null, null );
            xmldecl.Encoding = "UTF-8";
            xmldecl.Standalone = "yes";

            XmlElement root = doc.DocumentElement;
            doc.InsertBefore( xmldecl, root );

            return doc;
        }
    }
}
