﻿// <copyright>
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

namespace rocks.kfs.Reach.Reporting
{
    #region Object Declarations

    /// <summary>
    /// Reach Donation Object
    /// </summary>
    public class Donation
    {
        public int? account_id;
        public string address1;
        public string address2;
        public string admin_notes;
        public string alpha2_country_code;
        public decimal? amount;
        public string ancestry;
        public decimal? base_amount;
        public string base_currency;
        public decimal? base_deductible_amount;
        public decimal? base_gift_aid_amount;
        public decimal? base_total_amount;
        public decimal? base_transaction_fees;
        public string check_number;
        public string city;
        public string confirmation;
        public string country;
        public DateTime? created_at;
        public string currency;
        public DateTime? date;
        public decimal? deductible_amount;
        public string email;
        public string first_name;
        public decimal? gift_aid_amount;
        public int? id;
        public bool? include_fees;
        public string ip_address;
        public string last_name;
        public List<LineItem> line_items;
        public string name;
        public DateTime? next_donation;
        public string note;
        public string payment_type;
        public string paypal_profile_id;
        public string phone;
        public string post_body;
        public string postal;
        public string purpose;
        public bool? recurring;
        public string recurring_period;
        public string reference_number;
        public int? referral_id;
        public string referral_type;
        public string state;
        public string status;
        public int? supporter_id;
        public bool? taxable;
        public string token;
        public decimal? total_amount;
        public decimal? transaction_fees;
        public string transaction_token;
        public DateTime? updated_at;
        public int? user_id;
    }

    public class LineItem
    {
        public string id;
        public int? referral_id;
        public string referral_type;
    }

    #endregion
}
