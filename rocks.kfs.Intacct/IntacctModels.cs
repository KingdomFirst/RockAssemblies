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
using Rock;
using Rock.Data;
using Rock.Lava;

using rocks.kfs.Intacct.Enums;

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
        public int ItemIndex;
        public int FeeItemIndex;
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
        public SortedDictionary<string, dynamic> CustomFields = new SortedDictionary<string, object>();
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

    public class OtherReceipt
    {
        public DateTime PaymentDate;
        public string Payer; // Incorrectly named "payee" in API.
        public DateTime ReceivedDate;
        public PaymentMethod PaymentMethod;
        public string UnDepGLAccountNo;
        public string BankAccountId;
        public DateTime? DepositDate;
        public string RefId;
        public string Description;
        public string SupDocId;
        public string Currency;
        public DateTime? ExchRateDate;
        public string ExchRateType;
        public decimal? ExchRate;
        public Dictionary<string, dynamic> CustomFields = new Dictionary<string, object>();
        public bool? InclusiveTax;
        public string TaxSolutionId;
        public List<ReceiptLineItem> ReceiptItems;

    }

    public class ReceiptLineItem
    {
        public string GlAccountNo;
        public string AccountLabel;
        public decimal Amount;
        public string Memo;
        public string LocationId;
        public string DepartmentId;
        public SortedDictionary<string, dynamic> CustomFields = new SortedDictionary<string, object>();
        public string CustomFieldsString;
        public string ProjectId;
        public string TaskId;
        public string CostTypeId;
        public string CustomerId;
        public string VendorId;
        public string EmployeeId;
        public string ItemId;
        public string ClassId;
        public decimal TotalTrxAmount;
    }

    public class GLTransaction : ILavaDataDictionary, ILiquidizable
    {
        [LavaVisible]
        public decimal Amount { get; set; }

        [LavaVisible]
        public int FinancialAccountId { get; set; }

        [LavaVisible]
        public string CreditProject { get; set; }

        [LavaVisible]
        public string DebitProject { get; set; }

        [LavaVisible]
        public decimal TransactionFeeAmount { get; set; }

        [LavaVisible]
        public string TransactionFeeAccount { get; set; }

        [LavaVisible]
        public int ProcessTransactionFees { get; set; }

        [LavaVisible]
        public string Payer { get; set; }

        #region ILavaDataDictionary implementation

        /// <summary>
        /// Gets the available keys (for debugging info).
        /// </summary>
        /// <value>
        /// The available keys.
        /// </value>
        [LavaHidden]
        public virtual List<string> AvailableKeys
        {
            get
            {
                var availableKeys = new List<string>();

                foreach ( var propInfo in GetType().GetProperties() )
                {
                    availableKeys.Add( propInfo.Name );
                }

                return availableKeys;
            }
        }

        /// <summary>
        /// Gets the <see cref="System.Object"/> with the specified key.
        /// </summary>
        /// <value>
        /// The <see cref="System.Object"/>.
        /// </value>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public object GetValue( string key )
        {
            return this[key];
        }

        /// <summary>
        /// Gets the <see cref="System.Object"/> with the specified key.
        /// </summary>
        /// <value>
        /// The <see cref="System.Object"/>.
        /// </value>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        [LavaHidden]
        public virtual object this[object key]
        {
            get
            {
                string propertyKey = key.ToStringSafe();
                var propInfo = GetType().GetProperty( propertyKey );

                try
                {
                    object propValue = null;
                    if ( propInfo != null )
                    {
                        propValue = propInfo.GetValue( this, null );
                    }

                    if ( propValue is Guid )
                    {
                        return ( ( Guid ) propValue ).ToString();
                    }
                    else
                    {
                        return propValue;
                    }
                }
                catch
                {
                    // intentionally ignore
                }

                return null;
            }
        }

        /// <summary>
        /// Determines whether the specified key contains key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public bool ContainsKey( string key )
        {
            string propertyKey = key.ToStringSafe();
            var propInfo = GetType().GetProperty( propertyKey );

            return propInfo != null;
        }

        #endregion

        #region ILiquidizable

        /// <summary>
        /// Determines whether the specified key contains key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public virtual bool ContainsKey( object key )
        {
            string propertyKey = key.ToStringSafe();
            var propInfo = GetType().GetProperty( propertyKey );

            return propInfo != null;
        }

        /// <summary>
        /// Gets the <see cref="System.Object"/> with the specified key.
        /// </summary>
        /// <value>
        /// The <see cref="System.Object"/>.
        /// </value>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public object GetValue( object key )
        {
            return this[key];
        }

        /// <summary>
        /// To the liquid.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public object ToLiquid()
        {
            return this;
        }

        #endregion
    }

    public class GLBatchTotals : ICloneable
    {
        public decimal Amount;
        public string CreditAccount;
        public string DebitAccount;
        public string TransactionFeeAccount;
        public decimal TransactionFeeAmount;
        public string CreditClass;
        public string CreditDepartment;
        public string CreditLocation;
        public string CreditProject;
        public string DebitClass;
        public string DebitDepartment;
        public string DebitLocation;
        public string DebitProject;
        public string Description;
        public SortedDictionary<string, dynamic> CustomDimensions;
        public string CustomDimensionString;
        public int ProcessTransactionFees;
        public int ItemIndex = -1;
        public int FeeItemIndex = -1;

        public object Clone()
        {
            return ( GLBatchTotals ) MemberwiseClone();
        }
    }

    public class CheckingAccount
    {
        public string BankAccountId { get; set; }
        public string BankAcountNo { get; set; }
        public string GLAccountNo { get; set; }
        public string BankName { get; set; }
    }
}
