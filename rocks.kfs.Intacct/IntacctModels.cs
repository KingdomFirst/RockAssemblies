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

namespace rocks.kfs.Intacct
{
    public class IntacctAuth
    {
        public string SenderId;
        public string SenderPassword;
        public string CompanyId;
        public string UserId;
        public string UserPassword;
        public string LocationId;
    }

    public class JournalEntryLine : GlEntry
    {
        public string GlAccountNumber;
        public decimal? TransactionAmount;
        public string TransactionCurrency;
        public DateTime? ExchangeRateDate;
        public decimal? ExchangeRateValue;
        public string ExchangeRateType;
    }

    public class GlEntry
    {
        public const string CustomAllocationId = "Custom";
        public string DocumentNumber;
        public string AllocationId;
        public string Memo;
        public string DepartmentId;
        public string LocationId;
        public string ProjectId;
        public string CustomerId;
        public string VendorId;
        public string EmployeeId;
        public string ItemId;
        public string ClassId;
        public string ContractId;
        public string WarehouseId;
        public List<AllocationLine> CustomAllocationSplits = new List<AllocationLine>();
        public Dictionary<string, dynamic> CustomFields = new Dictionary<string, object>();
    }

    public class AllocationLine
    {
        public decimal? Amount;
        public string DepartmentId;
        public string LocationId;
        public string ProjectId;
        public string CustomerId;
        public string VendorId;
        public string EmployeeId;
        public string ItemId;
        public string ClassId;
        public string ContractId;
        public string WarehouseId;
    }

    public class GLTransaction
    {
        public decimal Amount;
        public int FinancialAccountId;
        public string Project;
    }

    public class GLBatchTotals
    {
        public decimal Amount;
        public string CreditAccount;
        public string DebitAccount;
        public string Class;
        public string Department;
        public string Location;
        public string Project;
        public string Description;
        public Dictionary<string, dynamic> CustomDimensions;
    }
}
