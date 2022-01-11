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
using Rock;
using Rock.Plugin;

namespace rocks.kfs.StepsToCare.Migrations
{
    [MigrationNumber( 5, "1.12.3" )]
    public class CreateJob : Migration
    {
        public override void Up()
        {
            Sql( @"IF NOT EXISTS( SELECT [Id] FROM [ServiceJob] WHERE [Class] = 'rocks.kfs.StepsToCare.Jobs.CareNeedAutomatedNotifications' AND [Guid] = '895C301C-02D1-4D9C-9FC4-DA7257368208' )
            BEGIN
               INSERT INTO [ServiceJob] (
                  [IsSystem]
                  ,[IsActive]
                  ,[Name]
                  ,[Description]
                  ,[Class]
                  ,[CronExpression]
                  ,[NotificationStatus]
                  ,[Guid] )
               VALUES ( 
                  0
                  ,1
                  ,'Care Need Automated Notifications'
                  ,''
                  ,'rocks.kfs.StepsToCare.Jobs.CareNeedAutomatedNotifications'
                  ,'0 15 2 1/1 * ? *'
                  ,1
                  ,'895C301C-02D1-4D9C-9FC4-DA7257368208'
                  );
            END" );
            // Attribute: rocks.kfs.StepsToCare.Jobs.CareNeedAutomatedNotifications: Follow Up Notification Template
            RockMigrationHelper.AddOrUpdateEntityAttribute( "Rock.Model.ServiceJob", "72ED40C7-4D64-4D60-9411-4FFB2B9E833E", "Class", "rocks.kfs.StepsToCare.Jobs.CareNeedAutomatedNotifications", "Follow Up Notification Template", "Follow Up Notification Template", @"The system communication to use when sending the Care Need Follow Up.", 0, "", "125F5EB3-8223-40B9-8FD6-4EED93602D72", "FollowUpSystemCommunication" );
            // Attribute: rocks.kfs.StepsToCare.Jobs.CareNeedAutomatedNotifications: Care Touch Needed Notification Template
            RockMigrationHelper.AddOrUpdateEntityAttribute( "Rock.Model.ServiceJob", "72ED40C7-4D64-4D60-9411-4FFB2B9E833E", "Class", "rocks.kfs.StepsToCare.Jobs.CareNeedAutomatedNotifications", "Care Touch Needed Notification Template", "Care Touch Needed Notification Template", @"The system communication to use when sending the Care Touch needed notification.", 0, "", "8D050C0F-105B-4A8B-8879-A15F78F5913E", "CareTouchNeededCommunication" );
            // Attribute: rocks.kfs.StepsToCare.Jobs.CareNeedAutomatedNotifications: Outstanding Needs Notification Template
            RockMigrationHelper.AddOrUpdateEntityAttribute( "Rock.Model.ServiceJob", "72ED40C7-4D64-4D60-9411-4FFB2B9E833E", "Class", "rocks.kfs.StepsToCare.Jobs.CareNeedAutomatedNotifications", "Outstanding Needs Notification Template", "Outstanding Needs Notification Template", @"The system communication to use when sending the Outstanding Care Needs notification.", 0, "", "08D1BB81-9FCA-488B-9383-9440D1DCCFC6", "OutstandingNeedsCommunication" );
            // Attribute: rocks.kfs.StepsToCare.Jobs.CareNeedAutomatedNotifications: Minimum Care Touches
            RockMigrationHelper.AddOrUpdateEntityAttribute( "Rock.Model.ServiceJob", "A75DFC58-7A1B-4799-BF31-451B2BBE38FF", "Class", "rocks.kfs.StepsToCare.Jobs.CareNeedAutomatedNotifications", "Minimum Care Touches", "Minimum Care Touches", @"Minimum care touches in 24 hours before the Care Touch needed notification gets sent out.", 0, @"2", "A29278C1-9A80-4BEA-81A9-1B7FCB6CA305", "MinimumCareTouches" );
            // Attribute: rocks.kfs.StepsToCare.Jobs.CareNeedAutomatedNotifications: Follow Up Days
            RockMigrationHelper.AddOrUpdateEntityAttribute( "Rock.Model.ServiceJob", "A75DFC58-7A1B-4799-BF31-451B2BBE38FF", "Class", "rocks.kfs.StepsToCare.Jobs.CareNeedAutomatedNotifications", "Follow Up Days", "Follow Up Days", @"Days after a Care Need has been entered before it changes status to Follow Up.", 0, @"10", "F64479EE-A595-4AA3-97D6-CAA768464284", "FollowUpDays" );
            // Attribute: rocks.kfs.StepsToCare.Jobs.CareNeedAutomatedNotifications: Care Dashboard Page
            RockMigrationHelper.AddOrUpdateEntityAttribute( "Rock.Model.ServiceJob", "BD53F9C9-EBA9-4D3F-82EA-DE5DD34A8108", "Class", "rocks.kfs.StepsToCare.Jobs.CareNeedAutomatedNotifications", "Care Dashboard Page", "Care Dashboard Page", @"Page used to populate 'LinkedPages.CareDashboard' lava field in notification.", 0, @"", "5D8548E7-9570-4337-888B-A51868A57CD0", "CareDashboardPage" );
            // Attribute: rocks.kfs.StepsToCare.Jobs.CareNeedAutomatedNotifications: Care Detail Page
            RockMigrationHelper.AddOrUpdateEntityAttribute( "Rock.Model.ServiceJob", "BD53F9C9-EBA9-4D3F-82EA-DE5DD34A8108", "Class", "rocks.kfs.StepsToCare.Jobs.CareNeedAutomatedNotifications", "Care Detail Page", "Care Detail Page", @"Page used to populate 'LinkedPages.CareDetail' lava field in notification.", 0, @"", "245DFE71-FF8D-41FE-91A8-E4D29B1AF016", "CareDetailPage" );
            // Attribute: rocks.kfs.StepsToCare.Jobs.CareNeedAutomatedNotifications: Minimum Care Touch Hours
            RockMigrationHelper.AddOrUpdateEntityAttribute( "Rock.Model.ServiceJob", "A75DFC58-7A1B-4799-BF31-451B2BBE38FF", "Class", "rocks.kfs.StepsToCare.Jobs.CareNeedAutomatedNotifications", "Minimum Care Touch Hours", "Minimum Care Touch Hours", @"Minimum care touches in this time period before the Care Touch needed notification gets sent out.", 0, @"24", "AF3ECA88-C715-4E2D-9D9B-C4CC3141C6EC", "MinimumCareTouchesHours" );

            RockMigrationHelper.AddAttributeValue( "125F5EB3-8223-40B9-8FD6-4EED93602D72", -1, SystemGuid.SystemCommunication.CARE_NEED_FOLLOWUP, "DEF6F4F3-B4EC-4F2F-A996-C30212D48DBA" ); // Care Need Automated Notifications: Follow Up Notification Template
            RockMigrationHelper.AddAttributeValue( "8D050C0F-105B-4A8B-8879-A15F78F5913E", -1, SystemGuid.SystemCommunication.CARE_NEED_TOUCH_NEEDED, "D4C73B39-5658-4B93-9A62-31DBF1A89EF4" ); // Care Need Automated Notifications: Care Touch Needed Notification Template
            RockMigrationHelper.AddAttributeValue( "08D1BB81-9FCA-488B-9383-9440D1DCCFC6", -1, SystemGuid.SystemCommunication.CARE_NEED_OUTSTANDING_NEEDS, "EF10B5FA-3164-4BC4-8750-E33B9CAA75A8" ); // Care Need Automated Notifications: Outstanding Needs Notification Template
            RockMigrationHelper.AddAttributeValue( "A29278C1-9A80-4BEA-81A9-1B7FCB6CA305", -1, @"2", "71D37170-2B1B-4C36-A853-0B64737C2518" ); // Care Need Automated Notifications: Minimum Care Touches
            RockMigrationHelper.AddAttributeValue( "F64479EE-A595-4AA3-97D6-CAA768464284", -1, @"10", "E471BFBB-4E50-4F36-801E-7F2F6FC7C417" ); // Care Need Automated Notifications: Follow Up Days
            RockMigrationHelper.AddAttributeValue( "5D8548E7-9570-4337-888B-A51868A57CD0", -1, @"1f93e9aa-ecca-42a2-8c91-73d991dbcd9f", "F7218A61-0324-467B-AD5B-C9C58030EA95" ); // Care Need Automated Notifications: Care Dashboard Page
            RockMigrationHelper.AddAttributeValue( "245DFE71-FF8D-41FE-91A8-E4D29B1AF016", -1, @"27953b65-21e2-4ca9-8461-3afad46d9bc8", "78C13C31-23A4-46F4-B9B1-1815E78A5016" ); // Care Need Automated Notifications: Care Detail Page
            RockMigrationHelper.AddAttributeValue( "AF3ECA88-C715-4E2D-9D9B-C4CC3141C6EC", -1, @"24", "79A2352F-D7FB-4BDA-957B-F32D1A10D1D1" ); // Care Need Automated Notifications: Minimum Care Touch Hours

            Sql( @"DECLARE @JobId int = ( SELECT TOP 1 [Id] FROM [ServiceJob] WHERE [Guid] = '895C301C-02D1-4D9C-9FC4-DA7257368208' )
                 UPDATE [AttributeValue] SET EntityId = @JobId WHERE [Guid] = 'DEF6F4F3-B4EC-4F2F-A996-C30212D48DBA'
                 UPDATE [AttributeValue] SET EntityId = @JobId WHERE [Guid] = 'D4C73B39-5658-4B93-9A62-31DBF1A89EF4'
                 UPDATE [AttributeValue] SET EntityId = @JobId WHERE [Guid] = 'EF10B5FA-3164-4BC4-8750-E33B9CAA75A8'
                 UPDATE [AttributeValue] SET EntityId = @JobId WHERE [Guid] = '71D37170-2B1B-4C36-A853-0B64737C2518'
                 UPDATE [AttributeValue] SET EntityId = @JobId WHERE [Guid] = 'E471BFBB-4E50-4F36-801E-7F2F6FC7C417'
                 UPDATE [AttributeValue] SET EntityId = @JobId WHERE [Guid] = 'F7218A61-0324-467B-AD5B-C9C58030EA95'
                 UPDATE [AttributeValue] SET EntityId = @JobId WHERE [Guid] = '78C13C31-23A4-46F4-B9B1-1815E78A5016'
                 UPDATE [AttributeValue] SET EntityId = @JobId WHERE [Guid] = '79A2352F-D7FB-4BDA-957B-F32D1A10D1D1'" ); // Set EntityId to proper Job id

        }

        public override void Down()
        {

            // Code Generated using Rock\Dev Tools\Sql\CodeGen_ServiceJobWithAttributes_ForAJob.sql
            RockMigrationHelper.DeleteAttribute( "125F5EB3-8223-40B9-8FD6-4EED93602D72" ); // rocks.kfs.StepsToCare.Jobs.CareNeedAutomatedNotifications: Follow Up Notification Template
            RockMigrationHelper.DeleteAttribute( "8D050C0F-105B-4A8B-8879-A15F78F5913E" ); // rocks.kfs.StepsToCare.Jobs.CareNeedAutomatedNotifications: Care Touch Needed Notification Template
            RockMigrationHelper.DeleteAttribute( "08D1BB81-9FCA-488B-9383-9440D1DCCFC6" ); // rocks.kfs.StepsToCare.Jobs.CareNeedAutomatedNotifications: Outstanding Needs Notification Template
            RockMigrationHelper.DeleteAttribute( "A29278C1-9A80-4BEA-81A9-1B7FCB6CA305" ); // rocks.kfs.StepsToCare.Jobs.CareNeedAutomatedNotifications: Minimum Care Touches
            RockMigrationHelper.DeleteAttribute( "F64479EE-A595-4AA3-97D6-CAA768464284" ); // rocks.kfs.StepsToCare.Jobs.CareNeedAutomatedNotifications: Follow Up Days
            RockMigrationHelper.DeleteAttribute( "5D8548E7-9570-4337-888B-A51868A57CD0" ); // rocks.kfs.StepsToCare.Jobs.CareNeedAutomatedNotifications: Care Dashboard Page
            RockMigrationHelper.DeleteAttribute( "245DFE71-FF8D-41FE-91A8-E4D29B1AF016" ); // rocks.kfs.StepsToCare.Jobs.CareNeedAutomatedNotifications: Care Detail Page
            RockMigrationHelper.DeleteAttribute( "AF3ECA88-C715-4E2D-9D9B-C4CC3141C6EC" ); // rocks.kfs.StepsToCare.Jobs.CareNeedAutomatedNotifications: Minimum Care Touch Hours

            // remove ServiceJob: Care Need Automated Notifications
            Sql( @"IF EXISTS( SELECT [Id] FROM [ServiceJob] WHERE [Class] = 'rocks.kfs.StepsToCare.Jobs.CareNeedAutomatedNotifications' AND [Guid] = '895C301C-02D1-4D9C-9FC4-DA7257368208' )
            BEGIN
               DELETE [ServiceJob]  WHERE [Guid] = '895C301C-02D1-4D9C-9FC4-DA7257368208';
            END" );

        }
    }
}