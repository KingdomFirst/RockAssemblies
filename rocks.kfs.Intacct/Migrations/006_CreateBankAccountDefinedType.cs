// <copyright>
// Copyright 2026 by Kingdom First Solutions
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
using Rock.Plugin;
using KFSConst = rocks.kfs.Intacct.SystemGuid;

namespace rocks.kfs.Intacct.Migrations
{
    [MigrationNumber( 6, "1.16.0" )]
    public class CreateBankAccountDefinedType : Migration
    {
        /// <summary>
        /// The commands to run to migrate plugin to the specific version
        /// </summary>
        public override void Up()
        {
            RockMigrationHelper.AddDefinedType( "Financial", "Intacct Bank Accounts", "Intacct checking accounts for use with exporting Other Receipts using Batch To Intacct plugin. Value should be set to the id of the checking account in Intacct.<br />Note: If using Batch (or Batches) to Intacct block(s) with Export Method set to \"Direct\", this list will be automatically generated and synced using the configured api.", KFSConst.DefinedType.INTACCT_OTHER_RECEIPT_BANK_ACCOUNT_DEFINED_TYPE );
        }

        /// <summary>
        /// The commands to undo a migration from a specific version
        /// </summary>
        public override void Down()
        {
            RockMigrationHelper.DeleteDefinedType( KFSConst.DefinedType.INTACCT_OTHER_RECEIPT_BANK_ACCOUNT_DEFINED_TYPE );
        }
    }
}
