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
namespace rocks.kfs.ShelbyFinancials.SystemGuid
{
    /// <summary>
    /// Custom KFS Attributes for Shelby Financials Export plugin
    /// </summary>
    internal class Attribute
    {
        #region Financial Account Attributes

        /// <summary>
        /// The category for all Shelby Financials financials account attributes.
        /// </summary>
        public const string FINANCIAL_ACCOUNT_ATTRIBUTE_CATEGORY = "F8893830-B331-4C9F-AA4C-470F0C9B0D18";

        /// <summary>
        /// The account number that transaction fees will be split into.
        /// </summary>
        public const string FINANCIAL_ACCOUNT_FEE_ACCOUNT = "0E114DBD-F23C-4C03-99C0-896ACECE2054";

        #endregion

        #region Financial Gateway Attributes

        /// <summary>
        /// The category for all Shelby Financials gateway account attributes.
        /// </summary>
        public const string FINANCIAL_GATEWAY_ATTRIBUTE_CATEGORY = "64C83F8E-A6BD-4944-AC4D-01DB83948752";

        /// <summary>
        /// The default gateway fee account number.
        /// </summary>
        public const string FINANCIAL_GATEWAY_DEFAULT_FEE_ACCOUNT = "A2745D7F-0F9D-4FB8-9FC2-191AA4F3BF62";

        /// <summary>
        /// The gateway fee processing attribute.
        /// </summary>
        public const string FINANCIAL_GATEWAY_FEE_PROCESSING = "055FEEBC-B5D8-4138-B161-058A19DF7125";

        /// <summary>
        /// The gateway fee calculation attribute.
        /// </summary>
        public const string FINANCIAL_GATEWAY_FEE_CALCULATION = "D2302708-DD11-44EB-A56F-8ABD04B86295";

        #endregion
    }
}
