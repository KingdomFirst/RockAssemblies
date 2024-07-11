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
namespace rocks.kfs.ScheduledGroupCommunication.SystemGuid
{
    /// <summary>
    /// Custom KFS Attributes for Scheduled Group Communications
    /// </summary>
    internal class Attribute
    {
        #region Email Attributes

        /// <summary>
        /// The attribute matrix template that emails will be created and sent from.
        /// </summary>
        public const string ATTRIBUTE_MATRIX_TEMPLATE_SCHEDULED_EMAILS = "7CFE297A-CE79-45D3-B4E0-D6BDAE723929";

        /// <summary>
        /// The matrix attribute for email send date
        /// </summary>
        public const string MATRIX_ATTRIBUTE_EMAIL_SEND_DATE = "39A38B02-112C-4EDC-A30E-4BDB1B090EE4";

        /// <summary>
        /// The matrix attribute for email send recurrence
        /// </summary>
        public const string MATRIX_ATTRIBUTE_EMAIL_SEND_RECURRENCE = "9BC4F790-A9F7-4822-AEEE-91095A3E3D4C";

        /// <summary>
        /// The matrix attribute for email from address
        /// </summary>
        public const string MATRIX_ATTRIBUTE_EMAIL_FROM_EMAIL = "F7C73002-6442-4756-BFDB-BC0BFE58EF15";

        /// <summary>
        /// The matrix attribute for email from name
        /// </summary>
        public const string MATRIX_ATTRIBUTE_EMAIL_FROM_NAME = "7BEE419A-8444-44E1-B7EB-451C038977B3";

        /// <summary>
        /// The matrix attribute for email subject
        /// </summary>
        public const string MATRIX_ATTRIBUTE_EMAIL_SUBJECT = "9EC7C8A9-F4C9-421C-9129-2DD023E09D05";

        /// <summary>
        /// The matrix attribute for email message
        /// </summary>
        public const string MATRIX_ATTRIBUTE_EMAIL_MESSAGE = "8C4EE7A8-086D-42B7-908F-77A9A36E5342";

        #endregion

        #region SMS Attributes

        /// <summary>
        /// The attribute matrix template that SMS messages will be created and sent from.
        /// </summary>
        public const string ATTRIBUTE_MATRIX_TEMPLATE_SCHEDULED_SMS = "B40C318D-0EB5-49CD-B93C-1CDA0F5CB4BC";

        /// <summary>
        /// The matrix attribute for SMS send date
        /// </summary>
        public const string MATRIX_ATTRIBUTE_SMS_SEND_DATE = "B2125940-565B-42CE-82BE-CDA58FC65FDE";

        /// <summary>
        /// The matrix attribute for SMS send recurrence
        /// </summary>
        public const string MATRIX_ATTRIBUTE_SMS_SEND_RECURRENCE = "EC6D13F7-A256-4B03-A94B-3B713F26E62D";

        /// <summary>
        /// The matrix attribute for SMS from address
        /// </summary>
        public const string MATRIX_ATTRIBUTE_SMS_FROM_NUMBER = "1984A561-C4A9-4D4F-B366-23AD54BDCFE8";

        /// <summary>
        /// The matrix attribute for SMS message
        /// </summary>
        public const string MATRIX_ATTRIBUTE_SMS_MESSAGE = "C57166D5-C0D3-4DA6-88DD-92AFA5126D69";

        #endregion
    }
}
