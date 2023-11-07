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
using Rock;
using Rock.Model;
using Rock.Plugin;

namespace rocks.kfs.ClickBid.Migrations
{
    [MigrationNumber( 1, "1.13.0" )]
    public partial class CreateDefinedType : Migration
    {
        public override void Up()
        {
            RockMigrationHelper.AddDefinedType( "Financial", "ClickBid Event Account Map", "List of ClickBid events to download sales reports for from the ClickBid gateway with their appropriate item type to Financial Account map to associate transactions into the proper accounts.", Guids.SystemGuid.ACCOUNT_MAP, @"Please specify the EventId provided by ClickBid in the value and the appropriate account map(s) per item type." );
            RockMigrationHelper.AddDefinedTypeAttribute( Guids.SystemGuid.ACCOUNT_MAP, "73B02051-0D38-4AD9-BF81-A2D477DE4F70", "Accounts", "Accounts", "Supported Item Types: Silent, Live, Donation, Quantity, Blind", 6560, "", "441B3C84-FE8D-4C6C-8437-1B8036AC4A89" );
            RockMigrationHelper.AddDefinedTypeAttribute( Guids.SystemGuid.ACCOUNT_MAP, "73B02051-0D38-4AD9-BF81-A2D477DE4F70", "Group Member Map", "GroupMemberMap", "Map specific item types to group member ids primarily to associate donations to fundraising group members. Supported Item Types: Silent, Live, Donation, Quantity, Blind", 6561, "", "320FDA10-2118-4351-AD51-816AFE75AFDD" );
            RockMigrationHelper.AddAttributeQualifier( "320FDA10-2118-4351-AD51-816AFE75AFDD", "allowhtml", "False", "666F6D63-36B1-4F24-BCCE-AFA394175398" );
            RockMigrationHelper.AddAttributeQualifier( "320FDA10-2118-4351-AD51-816AFE75AFDD", "customvalues", "", "8B1047A9-2FC8-406C-89A8-808B2ADB3731" );
            RockMigrationHelper.AddAttributeQualifier( "320FDA10-2118-4351-AD51-816AFE75AFDD", "definedtype", "", "DC499E66-7013-475A-986D-AB41D0413DF1" );
            RockMigrationHelper.AddAttributeQualifier( "320FDA10-2118-4351-AD51-816AFE75AFDD", "displayvaluefirst", "False", "493D8C0F-4F42-4EB5-9267-75B6209BAD0F" );
            RockMigrationHelper.AddAttributeQualifier( "320FDA10-2118-4351-AD51-816AFE75AFDD", "keyprompt", "Item Type", "BD678398-9380-4212-A82C-A6DD07BD9E93" );
            RockMigrationHelper.AddAttributeQualifier( "320FDA10-2118-4351-AD51-816AFE75AFDD", "valueprompt", "Group Member Id", "4E89CD23-11C7-475F-93F6-EA34DB6F0DEF" );
            RockMigrationHelper.AddAttributeQualifier( "441B3C84-FE8D-4C6C-8437-1B8036AC4A89", "allowhtml", "False", "67309A58-C044-495C-9469-A421EEE5D5F6" );
            RockMigrationHelper.AddAttributeQualifier( "441B3C84-FE8D-4C6C-8437-1B8036AC4A89", "customvalues", @"SELECT fa.Id as Value, CASE WHEN ggpa.Name IS NOT NULL THEN
	        CONCAT(ggpa.name, '' > '',gpa.Name,'' > '',pa.Name,'' > '', fa.Name)
        WHEN gpa.Name IS NOT NULL THEN
	        CONCAT(gpa.Name,'' > '',pa.Name,'' > '', fa.Name)
        WHEN pa.Name IS NOT NULL THEN
	        CONCAT(pa.Name,'' > '', fa.Name)
        ELSE
	        fa.Name 
        END as Text 
FROM FinancialAccount fa 
	LEFT JOIN FinancialAccount pa ON fa.ParentAccountId = pa.Id 
	LEFT JOIN FinancialAccount gpa ON pa.ParentAccountId = gpa.Id 
	LEFT JOIN FinancialAccount ggpa ON gpa.ParentAccountId = ggpa.Id
ORDER BY CASE WHEN ggpa.Name IS NOT NULL THEN
	        CONCAT(ggpa.name, '' > '',gpa.Name,'' > '',pa.Name,'' > '', fa.Name)
        WHEN gpa.Name IS NOT NULL THEN
	        CONCAT(gpa.Name,'' > '',pa.Name,'' > '', fa.Name)
        WHEN pa.Name IS NOT NULL THEN
	        CONCAT(pa.Name,'' > '', fa.Name)
        ELSE
	        fa.Name 
        END", "471EFB41-142C-4092-BCDC-670B22649804" );
            RockMigrationHelper.AddAttributeQualifier( "441B3C84-FE8D-4C6C-8437-1B8036AC4A89", "definedtype", "", "7C2B30D6-001B-4A92-8968-F9632F59B4F8" );
            RockMigrationHelper.AddAttributeQualifier( "441B3C84-FE8D-4C6C-8437-1B8036AC4A89", "displayvaluefirst", "False", "B7815DFE-FE5F-4D02-9B91-5E9F6AE2993C" );
            RockMigrationHelper.AddAttributeQualifier( "441B3C84-FE8D-4C6C-8437-1B8036AC4A89", "keyprompt", "Item Type", "DA9EE8A9-97C0-4F6F-9F12-DB9608507531" );
            RockMigrationHelper.AddAttributeQualifier( "441B3C84-FE8D-4C6C-8437-1B8036AC4A89", "valueprompt", "Account", "C865048B-7340-4FEF-9B26-0DF51B20B8E0" );
        }

        /// <summary>
        /// Operations to be performed during the downgrade process.
        /// </summary>
        public override void Down()
        {
            RockMigrationHelper.DeleteAttribute( "320FDA10-2118-4351-AD51-816AFE75AFDD" ); // GroupMemberMap
            RockMigrationHelper.DeleteAttribute( "441B3C84-FE8D-4C6C-8437-1B8036AC4A89" ); // Accounts
            RockMigrationHelper.DeleteDefinedType( Guids.SystemGuid.ACCOUNT_MAP ); // ClickBid Event Account Map
        }
    }
}