// <copyright>
// Copyright 2022 by Kingdom First Solutions
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
using System.Linq;
using Rock;
using Rock.Data;
using Rock.Model;
using Rock.Plugin;
using KFSConst = rocks.kfs.ScheduledGroupCommunication.SystemGuid;

namespace rocks.kfs.ScheduledGroupCommunication.Migrations
{
    [MigrationNumber( 3, "1.7.4" )]
    internal class AddDailyRecurrence : Migration
    {
        /// <summary>
        /// The commands to run to migrate plugin to the specific version
        /// </summary>
        public override void Up()
        {
            #region Email Attribute

            // recurrence attribute
            RockMigrationHelper.UpdateAttributeQualifier( KFSConst.Attribute.MATRIX_ATTRIBUTE_EMAIL_SEND_RECURRENCE, "values", "0^OneTime,4^Daily,1^Weekly,2^BiWeekly,3^Monthly", "F1F0873E-CA36-4935-90F5-DFEFD79083C8" );

            #endregion

            #region SMS Attributes

            var rockContextSMS = new RockContext();
            var attributeMatrixTemplateSMSId = new AttributeMatrixTemplateService( rockContextSMS ).Get( KFSConst.Attribute.ATTRIBUTE_MATRIX_TEMPLATE_SCHEDULED_SMS.AsGuid() ).Id.ToString();

            // recurrence attribute
            RockMigrationHelper.UpdateAttributeQualifier( KFSConst.Attribute.MATRIX_ATTRIBUTE_SMS_SEND_RECURRENCE, "values", "0^OneTime,4^Daily,1^Weekly,2^BiWeekly,3^Monthly", "4FAE15C5-C1AB-4FDB-B21D-CBF467B1D395" );

            #endregion
        }

        /// <summary>
        /// The commands to undo a migration from a specific version
        /// </summary>
        public override void Down()
        {
            RockMigrationHelper.UpdateAttributeQualifier( KFSConst.Attribute.MATRIX_ATTRIBUTE_EMAIL_SEND_RECURRENCE, "values", "0^OneTime,1^Weekly,2^BiWeekly,3^Monthly", "F1F0873E-CA36-4935-90F5-DFEFD79083C8" );
            RockMigrationHelper.UpdateAttributeQualifier( KFSConst.Attribute.MATRIX_ATTRIBUTE_SMS_SEND_RECURRENCE, "values", "0^OneTime,1^Weekly,2^BiWeekly,3^Monthly", "4FAE15C5-C1AB-4FDB-B21D-CBF467B1D395" );

        }
    }
}
