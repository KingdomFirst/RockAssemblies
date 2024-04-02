// <copyright>
// Copyright 2024 by Kingdom First Solutions
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
using Rock.Plugin;

namespace rocks.kfs.StepsToCare.Migrations
{
    [MigrationNumber( 16, "1.14.0" )]
    public class NewAttribute : Migration
    {
        public override void Up()
        {

            // Attribute for BlockType
            //   BlockType: Care Dashboard
            //   Category: KFS > Steps To Care
            //   Attribute: Benevolence Type
            RockMigrationHelper.AddOrUpdateBlockTypeAttribute( "AF14CB6C-F915-4449-9CB7-7C44B624B051", "7BD3C3A3-DF4A-41EB-BF13-29EDB166078B", "Benevolence Type", "BenevolenceType", "Benevolence Type", @"The Benevolence type used when creating benevolence requests from Steps to Care 'Actions'", 12, @"B4A7C50B-E399-452E-BA37-1ABD6B15482C", "026DDC0A-F633-4D22-A9F0-B062B80CD17D" );

            // Attribute for BlockType
            //   BlockType: Care Entry
            //   Category: KFS > Steps To Care
            //   Attribute: Group Type Roles
            RockMigrationHelper.AddOrUpdateBlockTypeAttribute( "4F0F9ED7-9F74-4152-B27F-D9B2A458AFBE", "BD0D9B57-2A41-4490-89FF-F01DAB7D4904", "Group Type Roles", "GroupTypeAndRole", "Group Type and Role", @"Select the Group Type Roles of the leaders you would like auto assigned to care need when the Person is a member of this type of group. If none are selected it will not auto assign the group member with the appropriate role to the need. ", 0, @"", "664F6632-A438-4249-ADB0-D94B891BF089" );

            // Add Block Attribute Value
            //   Block: Care Dashboard
            //   BlockType: Care Dashboard
            //   Category: KFS > Steps To Care
            //   Block Location: Page=Steps to Care, Site=Rock RMS
            //   Attribute: Benevolence Type
            /*   Attribute Value: B4A7C50B-E399-452E-BA37-1ABD6B15482C */
            RockMigrationHelper.AddBlockAttributeValue( "EADBE3F0-F64B-4583-B49D-F0031BBC929F", "026DDC0A-F633-4D22-A9F0-B062B80CD17D", @"B4A7C50B-E399-452E-BA37-1ABD6B15482C" );

            // Add Block Attribute Value
            //   Block: Care Entry
            //   BlockType: Care Entry
            //   Category: KFS > Steps To Care
            //   Block Location: Page=Care Entry, Site=Rock RMS
            //   Attribute: Group Type Roles
            /*   Attribute Value: f6cecb48-52c1-4d25-9411-f1465755eb70,6d798efa-0110-41d5-bce4-30acefe4317e */
            RockMigrationHelper.AddBlockAttributeValue( "F953C5EF-6504-45F9-81A8-063518B7AB61", "664F6632-A438-4249-ADB0-D94B891BF089", @"f6cecb48-52c1-4d25-9411-f1465755eb70,6d798efa-0110-41d5-bce4-30acefe4317e" );

            // Attribute: rocks.kfs.StepsToCare.Jobs.CareNeedAutomatedProcesses: Group Type Roles
            RockMigrationHelper.AddOrUpdateEntityAttribute( "Rock.Model.ServiceJob", "BD0D9B57-2A41-4490-89FF-F01DAB7D4904", "Class", "rocks.kfs.StepsToCare.Jobs.CareNeedAutomatedProcesses", "Group Type Roles", "Group Type Roles", @"Select the Group Type Roles of the leaders you would like auto assigned to care need when the Person is a member of this type of group. If none are selected it will not auto assign the group member with the appropriate role to the need. ", 0, @"", "D3F19DE8-B8A0-4B87-AB4D-DE827D79C780", "GroupTypeAndRole" );

            RockMigrationHelper.AddAttributeValue( "D3F19DE8-B8A0-4B87-AB4D-DE827D79C780", 1079, @"", "D3F19DE8-B8A0-4B87-AB4D-DE827D79C780" ); // Care Need Automated Processes: Group Type Roles

            RockMigrationHelper.UpdatePageLayout( "ABA4CE73-28DC-42DE-BE70-33F09287C116", "6AD84AFC-B3A1-4E30-B53B-C6E57B513839" );
        }

        public override void Down()
        {
            RockMigrationHelper.UpdatePageLayout( "ABA4CE73-28DC-42DE-BE70-33F09287C116", "F66758C6-3E3D-4598-AF4C-B317047B5987" );

            // Attribute: rocks.kfs.StepsToCare.Jobs.CareNeedAutomatedNotifications: Group Type and Role
            RockMigrationHelper.AddOrUpdateEntityAttribute( "Rock.Model.ServiceJob", "3BB25568-E793-4D12-AE80-AC3FDA6FD8A8", "Class", "rocks.kfs.StepsToCare.Jobs.CareNeedAutomatedProcesses", "Group Type and Role", "Group Type and Role", @"Select the group Type and Role of the leader you would like auto assigned to care need. If none are selected it will not auto assign the small group member to the need. ", 0, @"", "D3F19DE8-B8A0-4B87-AB4D-DE827D79C780", "GroupTypeAndRole" );

            RockMigrationHelper.DeleteAttribute( "664F6632-A438-4249-ADB0-D94B891BF089" );
            RockMigrationHelper.DeleteAttribute( "026DDC0A-F633-4D22-A9F0-B062B80CD17D" );
        }
    }
}