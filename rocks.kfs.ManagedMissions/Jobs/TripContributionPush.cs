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

using Quartz;
using Rock;
using Rock.Attribute;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;
using API = rocks.kfs.ManagedMissions.API;

namespace rocks.kfs.Jobs.ManagedMissions
{
    [DisallowConcurrentExecution]
    [TextField( "Managed Missions API URL", "The url path to the Managed Missions API.", true, "https://app.managedmissions.com/API/", "ManagedMissions", 0, "ManagedMissionsAPIUrl", false )]
    [TextField( "Managed Missions API Key", "The API key/access token for the Managed Missions API", true, "", "ManagedMissions", 1, "ManagedMissionsAPIKey", false )]
    [DefinedValueField( "2E6540EA-63F0-40FE-BE50-F2A84735E600", "Donor Connection Status Map", "The connection statuses that will be counted as a 'RegularAttender' in Managed Missions", true, true, "", "", 3 )]
    public class TripContributionPush : IJob
    {
        public void Execute( IJobExecutionContext context )
        {
            JobDataMap jobMap = context.JobDetail.JobDataMap;
            string ApiUrl = jobMap.GetString( "ManagedMissionsAPIUrl" );
            string ApiKey = jobMap.GetString( "ManagedMissionsAPIKey" );
            int successfulTx = 0;

            List<string> unsuccessfulTxList = new List<string>();

            var mmClient = new API.ManagedMissionsClient( ApiUrl, ApiKey );

            using ( var rockContext = new RockContext() )
            {
                var cashDefinedValueId = new DefinedValueService( rockContext ).GetByGuid( new Guid( Rock.SystemGuid.DefinedValue.CURRENCY_TYPE_CASH ) ).Id;
                var homeDefinedValueId = new DefinedValueService( rockContext ).GetByGuid( new Guid( Rock.SystemGuid.DefinedValue.GROUP_LOCATION_TYPE_HOME ) ).Id;
                var workLocationDefinedValueId = new DefinedValueService( rockContext ).GetByGuid( new Guid( Rock.SystemGuid.DefinedValue.GROUP_LOCATION_TYPE_WORK ) ).Id;
                var buisnessDefinedValueId = new DefinedValueService( rockContext ).GetByGuid( new Guid( Rock.SystemGuid.DefinedValue.PERSON_RECORD_TYPE_BUSINESS ) ).Id;

                var connectionStatusGuids = ( jobMap.GetString( "ConnectionStatuses" ) ?? string.Empty ).Split( ',' ).AsGuidList();
                ServiceLogService logService = new ServiceLogService( rockContext );

                var transactionQry = new FinancialTransactionService( rockContext )
                    .Queryable( "TransactionDetails,FinancialPaymentDetail,AuthorizedPersonAlias.Person" )
                    .WhereAttributeValue( rockContext, a => a.Attribute.Key == "kfs_mm_tripkey" && ( a.Value != null && a.Value != "" ) )
                    .WhereAttributeValue( rockContext, a => a.Attribute.Key == "kfs_mm_personkey" && ( a.Value != null && a.Value != "" ) )
                    .WhereAttributeValue( rockContext, a => a.Attribute.Key == "kfs_mm_ManagedMissionsSyncDate" && ( a.Value != null && a.Value != "" ) );

                foreach ( var transaction in transactionQry.ToList() )
                {
                    if ( transaction.SourceTypeValue != null )
                    {
                        transaction.LoadAttributes();

                        var mmContribution = new API.Contribution();
                        mmContribution.MissionTripImportExportKey = transaction.GetAttributeValue( "kfs_mm_tripkey" );
                        mmContribution.PersonImportExportKey = transaction.GetAttributeValue( "kfs_mm_personkey" );
                        mmContribution.ContributionAmount = transaction.TotalAmount;
                        mmContribution.Anonymous = transaction.ShowAsAnonymous;
                        mmContribution.ConfirmationCode = transaction.Id.ToString();
                        mmContribution.DepositDate = transaction.TransactionDateTime;
                        mmContribution.DonorName = transaction.AuthorizedPersonAlias.Person.FullName;

                        if ( transaction.AuthorizedPersonAlias.Person.SuffixValueId.HasValue )
                        {
                            mmContribution.DonorSuffix = DefinedValueCache.Get( transaction.AuthorizedPersonAlias.Person.SuffixValueId.Value ).Value;
                        }

                        if ( transaction.AuthorizedPersonAlias.Person.TitleValueId.HasValue )
                        {
                            mmContribution.DonorTitle = DefinedValueCache.Get( transaction.AuthorizedPersonAlias.Person.TitleValueId.Value ).Value;
                        }

                        mmContribution.ImportExportKey = string.Concat( "Rock_", transaction.Id );

                        string refNumber = String.Empty;
                        if ( transaction.FinancialPaymentDetail.CurrencyTypeValueId == cashDefinedValueId )
                        {
                            refNumber = "C";
                        }
                        else
                        {
                            refNumber = transaction.TransactionCode;
                        }

                        mmContribution.ReferenceNumber = refNumber;
                        mmContribution.TransactionType = DefinedValueCache.Get( transaction.FinancialPaymentDetail.CurrencyTypeValueId.Value ).Value;
                        mmContribution.EmailAddress = transaction.AuthorizedPersonAlias.Person.Email;

                        var personService = new PersonService( rockContext );
                        PhoneNumber phoneNumber = null;
                        GroupLocation address = null;

                        if ( transaction.AuthorizedPersonAlias.Person.RecordTypeValueId == buisnessDefinedValueId )
                        {
                            mmContribution.RegularAttender = false;
                            phoneNumber = personService.GetPhoneNumber( transaction.AuthorizedPersonAlias.Person, DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.PERSON_PHONE_TYPE_WORK ) );
                            address = personService.GetFirstLocation( transaction.AuthorizedPersonAlias.Person.Id, workLocationDefinedValueId );
                        }
                        else
                        {
                            mmContribution.RegularAttender = connectionStatusGuids.Contains( transaction.AuthorizedPersonAlias.Person.ConnectionStatusValue.Guid );

                            phoneNumber = personService.GetPhoneNumber( transaction.AuthorizedPersonAlias.Person, DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.PERSON_PHONE_TYPE_HOME ) );

                            if ( phoneNumber == null )
                            {
                                phoneNumber = personService.GetPhoneNumber( transaction.AuthorizedPersonAlias.Person, DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.PERSON_PHONE_TYPE_MOBILE ) );
                            }

                            address = personService.GetFirstLocation( transaction.AuthorizedPersonAlias.Person.Id, homeDefinedValueId );
                        }

                        if ( phoneNumber != null && !phoneNumber.IsUnlisted )
                        {
                            mmContribution.PhoneNumber = phoneNumber.Number;
                        }

                        if ( address != null )
                        {
                            mmContribution.Address1 = address.Location.Street1;
                            mmContribution.Address2 = address.Location.Street2;
                            mmContribution.City = address.Location.City;
                            mmContribution.State = address.Location.State;
                            mmContribution.PostalCode = address.Location.PostalCode;
                        }
                        if ( transaction.TransactionDetails.Where( td => td.Account.IsTaxDeductible == false ).Count() > 0 )
                        {
                            mmContribution.TaxDeductible = false;
                        }
                        else
                        {
                            mmContribution.TaxDeductible = true;
                        }
                        string msg = null;
                        int mmId = mmContribution.Save( mmClient, out msg );

                        if ( mmId > 0 )
                        {
                            successfulTx++;
                            transaction.SetAttributeValue( "kfs_mm_ManagedMissionsSyncDate", RockDateTime.Now );
                            transaction.SaveAttributeValues( rockContext );
                        }
                        else
                        {
                            unsuccessfulTxList.Add( string.Format( "Transaction ID: {0} - Giver: {1} - Messages: {2}", transaction.Id, transaction.AuthorizedPersonAlias.Person.FullName, msg ) );
                            ServiceLog log = new ServiceLog();
                            log.LogDateTime = RockDateTime.Now;
                            log.Type = "Managed Mission Transaction Sync";
                            log.Name = "KFS Managed Missions Transaction Sync";
                            log.Input = string.Format( "Transaction ID: {0} - Giver: {1}", transaction.Id, transaction.AuthorizedPersonAlias.Person.FullName );
                            log.Input = string.Format( "Messages: {0}", msg );
                            log.Success = false;
                            logService.Add( log );
                        }
                    }
                    else
                    {
                        unsuccessfulTxList.Add( string.Format( "Transaction ID: {0} - Giver: {1} - Messages: {2}", transaction.Id, transaction.AuthorizedPersonAlias.Person.FullName, "Transaction Source cannot be null" ) );
                        ServiceLog log = new ServiceLog();
                        log.LogDateTime = RockDateTime.Now;
                        log.Type = "Managed Mission Transaction Sync";
                        log.Name = "KFS Managed Missions Transaction Sync";
                        log.Input = string.Format( "Transaction ID: {0} - Giver: {1}", transaction.Id, transaction.AuthorizedPersonAlias.Person.FullName );
                        log.Input = string.Format( "Messages: {0}", "Transaction source cannot be null" );
                        log.Success = false;
                        logService.Add( log );
                    }
                }
                StringBuilder resultSB = new StringBuilder();
                resultSB.AppendFormat( "{0} contributions processed.", ( successfulTx + unsuccessfulTxList.Count ) );
                resultSB.AppendFormat( "{0} contributions pushed to Managed Missions.\n", successfulTx );

                if ( unsuccessfulTxList.Count > 0 )
                {
                    resultSB.AppendFormat( "{0} transactions were unsuccessful.\n\n", unsuccessfulTxList.Count );
                    resultSB.AppendLine( "Unsuccessful Transactions:" );
                    foreach ( var item in unsuccessfulTxList )
                    {
                        resultSB.AppendLine( item.ToString() );
                    }
                }

                context.Result = resultSB.ToString();
            }
        }
    }
}
