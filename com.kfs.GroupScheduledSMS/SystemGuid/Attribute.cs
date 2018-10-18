using System;

namespace com.kfs.GroupScheduledSMS.SystemGuid
{
    /// <summary>
    /// Custom KFS Attributes for Group Scheduled SMS
    /// </summary>
    class Attribute
    {
        /// <summary>
        /// The attribute matrix template that SMS messages will be created and sent from.
        /// </summary>
        public const string ATTRIBUTE_MATRIX_TEMPLATE_SCHEDULED_SMS = "B40C318D-0EB5-49CD-B93C-1CDA0F5CB4BC";

        /// <summary>
        /// The matrix attribute for email send date
        /// </summary>
        public const string MATRIX_ATTRIBUTE_SMS_SEND_DATE = "B2125940-565B-42CE-82BE-CDA58FC65FDE";

        /// <summary>
        /// The matrix attribute for email from address
        /// </summary>
        public const string MATRIX_ATTRIBUTE_SMS_FROM_NUMBER = "1984A561-C4A9-4D4F-B366-23AD54BDCFE8";

        /// <summary>
        /// The matrix attribute for email message
        /// </summary>
        public const string MATRIX_ATTRIBUTE_SMS_MESSAGE = "C57166D5-C0D3-4DA6-88DD-92AFA5126D69";
    }
}
