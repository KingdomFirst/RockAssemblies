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
using Newtonsoft.Json;
using RestSharp;

namespace rocks.kfs.ManagedMissions.API
{
    public class Contribution
    {
        #region Properties

        public int? Id { get; set; }
        public int? MissionTripId { get; set; }
        public string MissionTripImportExportKey { get; set; }
        public int? PersonId { get; set; }
        public string PersonImportExportKey { get; set; }
        public string PersonName { get; set; }
        public Decimal? ContributionAmount { get; set; }
        public bool? Anonymous { get; set; }
        public string ConfirmationCode { get; set; }
        public DateTime? DepositDate { get; set; }
        public string DonorName { get; set; }
        public string DonorOranization { get; set; }
        public string DonorSuffix { get; set; }
        public string DonorTitle { get; set; }
        public string ImportExportKey { get; set; }
        public string Notes { get; set; }
        public string ReferenceNumber { get; set; }
        public bool? RegularAttender { get; set; }
        public bool? TaxDeductible { get; set; }
        public string TransactionType { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }

        #endregion

        public int Save( ManagedMissionsClient client, out string msg )
        {
            msg = null;
            RestRequest request = new RestRequest( "ContributionAPI/Create", RestSharp.Method.POST ) { RequestFormat = DataFormat.Json };
            string json = JsonConvert.SerializeObject( this, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore } );

            request.AddParameter( "application/json", json, ParameterType.RequestBody );
            ResponseData<Contribution> response = client.SendRequest<Contribution>( request );

            if ( response.Messages != null && response.Messages.Length > 0 )
            {
                msg = string.Join( ", ", response.Messages );
            }

            if ( response.Data != null && response.Data.Id != null )
            {
                return response.Data.Id.Value;
            }
            else
            {
                string message = null;
                if ( response.Messages.Length > 0 )
                {
                    message = string.Join( ", ", response.Messages );
                }
                else if ( !string.IsNullOrWhiteSpace( response.Error ) )
                {
                    message = response.Error;
                }

                if ( !string.IsNullOrWhiteSpace( message ) )
                    throw new NotImplementedException( "An error occurred while saving contribution. Error: " + message );

                return -1;
            }
        }
    }
}
