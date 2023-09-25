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
namespace rocks.kfs.Intacct.SystemGuid
{
    /// <summary>
    /// Custom KFS Attributes for Intacct Export plugin
    /// </summary>
    internal class Attribute
    {
        #region Financial Account Attributes

        /// <summary>
        /// The category for all Intacct financial account attributes.
        /// </summary>
        public const string FINANCIAL_ACCOUNT_ATTRIBUTE_CATEGORY = "7361A954-350A-41F1-9D94-AD2CF4030CA5";

        /// <summary>
        /// The account number to use for credit entries.
        /// </summary>
        public const string FINANCIAL_ACCOUNT_CREDIT_ACCOUNT = "8D790DD0-D84F-4DE2-9C7A-356D4590439E";

        /// <summary>
        /// The account number to use for debit entries.
        /// </summary>
        public const string FINANCIAL_ACCOUNT_DEBIT_ACCOUNT = "48E1B80E-5E8D-4016-B64E-F2527F328EA7";

        /// <summary>
        /// The Intacct dimension for Project.
        /// </summary>
        public const string FINANCIAL_ACCOUNT_PROJECT = "115519C9-EDFA-4BB5-A512-102C798F17F4";

        /// <summary>
        /// The account number that transaction fees will be split into.
        /// </summary>
        public const string FINANCIAL_ACCOUNT_FEE_ACCOUNT = "AB69D108-59FF-4D07-94AA-ECD14FC7B2BD";

        /// <summary>
        /// The Intacct dimension for Class Id.
        /// </summary>
        public const string FINANCIAL_ACCOUNT_CLASS = "76EFF760-B5B5-4053-8EBD-C329ECB85032";

        /// <summary>
        /// The Intacct dimension for Department.
        /// </summary>
        public const string FINANCIAL_ACCOUNT_DEPARTMENT = "CD56CBD9-CA3B-49F2-BCA5-73A7C3F19328";

        /// <summary>
        /// The Intacct dimension for Location.
        /// </summary>
        public const string FINANCIAL_ACCOUNT_LOCATION = "CFA818F9-3163-45FE-AA03-AAF8DEDCF48D";

        /// <summary>
        /// The Intacct dimension for Projects of debit account.
        /// </summary>
        public const string FINANCIAL_ACCOUNT_PROJECT_DEBIT = "9DF341B7-2E07-44BA-94B1-F637BE551961";

        /// <summary>
        /// The Intacct dimension for Class Id of debit account.
        /// </summary>
        public const string FINANCIAL_ACCOUNT_CLASS_DEBIT = "58B0B754-0B1C-4E4E-B396-3F8F046267E3";

        /// <summary>
        /// The Intacct dimension for Department of debit account.
        /// </summary>
        public const string FINANCIAL_ACCOUNT_DEPARTMENT_DEBIT = "5705FC91-0937-4F84-812F-730899E0114E";

        /// <summary>
        /// The Intacct dimension for Location of debit account.
        /// </summary>
        public const string FINANCIAL_ACCOUNT_LOCATION_DEBIT = "0F247FC0-ECF0-407F-A042-F8E3638ED5B4";

        #endregion

        #region Financial Gateway Attributes

        /// <summary>
        /// The category for all Intacct gateway account attributes.
        /// </summary>
        public const string FINANCIAL_GATEWAY_ATTRIBUTE_CATEGORY = "69E2D60D-C5D4-4804-9A48-2B2A7A5E64DE";

        /// <summary>
        /// The default gateway fee account number.
        /// </summary>
        public const string FINANCIAL_GATEWAY_DEFAULT_FEE_ACCOUNT = "D6AE2F14-4B4B-4E28-A79D-8C28F9DDBED3";

        /// <summary>
        /// The gateway fee processing attribute.
        /// </summary>
        public const string FINANCIAL_GATEWAY_FEE_PROCESSING = "F6CE6FDF-DC54-414A-BD76-7FE75B084A1B";

        #endregion
    }
}
