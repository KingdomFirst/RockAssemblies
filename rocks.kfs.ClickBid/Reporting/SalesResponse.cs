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

    public class SalesResponse
    {
        public List<Sale> data;

        // Pagination
        public int? current_page;
        public int? from;
        public int? last_page;
        public string path;
        public int? per_page;
        public int? to;
        public int? total;
        public DateTime? timestamp;
    }

    #endregion
}
