// <copyright>
// Copyright 2023 by Kingdom First Solutions
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

namespace rocks.kfs.ClickBid.Reporting
{
    #region Object Declarations

    /// <summary>
    /// ClickBid Sale Object from Sales Report API Call
    /// </summary>
    public class Sale
    {
        public string address;
        public string address2;
        public int? bidder_number;
        public string bidder_tags;
        public string category;
        public decimal? cc_fees;
        public DateTime? checkout_time;
        public string checkout_time_display;
        public string city;
        public DateTime? closing_time;
        public string closing_time_display;
        public string discount_amount;
        public string discount_code;
        public string donor;
        public string emails;
        public string first_name;
        public string fmv;
        public string fmv_percent;
        public string item_name;
        public int? item_number;
        public string item_tags;
        public string item_type;
        public string last_name;
        public string pay_type;
        public string payout_date;
        public string phones;
        public decimal? purchase_amount;
        public int? qty;
        public string refund;
        public string refund_cc_fees;
        public string starting_bid;
        public string state;
        public string table_name;
        public decimal? tax;
        public DateTime? time_bid;
        public string time_bid_display;
        public string won_by;
        public string zip;
    }

    #endregion
}
