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

namespace rocks.kfs.ClickBid.Reporting
{
    /// <summary>
    /// Reach Sponsorship Supporter Object
    /// </summary>
    public class Supporter
    {
        public int? account_id;
        public DateTime? created_at;
        public int? id;
        public string payment_type;
        public string recurring_period;
        public string share_type_id;
        public Sponsorship sponsorship;
        public int? sponsorship_id;
        public int? supporter_id;
        public decimal? total_given;
        public decimal? total_needed;
    }

    /// <summary>
    /// Reach Sponsorship Object
    /// </summary>
    public class Sponsorship
    {
        public string full_url;
        public int? id;
        public string permalink;
        public Place place;
        public int? sponsorship_type_id;
        public string sponsorship_type_permalink;
        public string sponsorship_type_title;
    }

    /// <summary>
    /// Reach Place Object
    /// </summary>
    public class Place
    {
        public DateTime? created_at;
        public string description;
        public int? id;
        public string permalink;
        public string title;
    }
}
