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
using Rock;
using Rock.Plugin;

namespace rocks.kfs.StepsToCare.Migrations
{
    [MigrationNumber( 15, "1.12.3" )]
    public class UpdateJob : Migration
    {
        public override void Up()
        {
            Sql( @"IF EXISTS( SELECT [Id] FROM [ServiceJob] WHERE [Class] = 'rocks.kfs.StepsToCare.Jobs.CareNeedAutomatedNotifications' AND [Guid] = '895C301C-02D1-4D9C-9FC4-DA7257368208' )
            BEGIN
               UPDATE [ServiceJob]
                SET
                    [Name] = 'Care Need Automated Processes'
                  , [Class] = 'rocks.kfs.StepsToCare.Jobs.CareNeedAutomatedProcesses'
                WHERE
                    [Guid] = '895C301C-02D1-4D9C-9FC4-DA7257368208'
            END" );

            Sql( @"UPDATE [Attribute] SET [EntityTypeQualifierValue] = 'rocks.kfs.StepsToCare.Jobs.CareNeedAutomatedProcesses' WHERE [Guid] = '125F5EB3-8223-40B9-8FD6-4EED93602D72'
                   UPDATE [Attribute] SET [EntityTypeQualifierValue] = 'rocks.kfs.StepsToCare.Jobs.CareNeedAutomatedProcesses' WHERE [Guid] = '8D050C0F-105B-4A8B-8879-A15F78F5913E'
                   UPDATE [Attribute] SET [EntityTypeQualifierValue] = 'rocks.kfs.StepsToCare.Jobs.CareNeedAutomatedProcesses' WHERE [Guid] = '08D1BB81-9FCA-488B-9383-9440D1DCCFC6'
                   UPDATE [Attribute] SET [EntityTypeQualifierValue] = 'rocks.kfs.StepsToCare.Jobs.CareNeedAutomatedProcesses' WHERE [Guid] = 'A29278C1-9A80-4BEA-81A9-1B7FCB6CA305'
                   UPDATE [Attribute] SET [EntityTypeQualifierValue] = 'rocks.kfs.StepsToCare.Jobs.CareNeedAutomatedProcesses' WHERE [Guid] = 'F64479EE-A595-4AA3-97D6-CAA768464284'
                   UPDATE [Attribute] SET [EntityTypeQualifierValue] = 'rocks.kfs.StepsToCare.Jobs.CareNeedAutomatedProcesses' WHERE [Guid] = '5D8548E7-9570-4337-888B-A51868A57CD0'
                   UPDATE [Attribute] SET [EntityTypeQualifierValue] = 'rocks.kfs.StepsToCare.Jobs.CareNeedAutomatedProcesses' WHERE [Guid] = '245DFE71-FF8D-41FE-91A8-E4D29B1AF016'
                   UPDATE [Attribute] SET [EntityTypeQualifierValue] = 'rocks.kfs.StepsToCare.Jobs.CareNeedAutomatedProcesses' WHERE [Guid] = 'AF3ECA88-C715-4E2D-9D9B-C4CC3141C6EC'" );

            // Attribute: rocks.kfs.StepsToCare.Jobs.CareNeedAutomatedNotifications: Group Type and Role
            RockMigrationHelper.AddOrUpdateEntityAttribute( "Rock.Model.ServiceJob", "3BB25568-E793-4D12-AE80-AC3FDA6FD8A8", "Class", "rocks.kfs.StepsToCare.Jobs.CareNeedAutomatedProcesses", "Group Type and Role", "Group Type and Role", @"Select the group Type and Role of the leader you would like auto assigned to care need. If none are selected it will not auto assign the small group member to the need. ", 0, @"", "D3F19DE8-B8A0-4B87-AB4D-DE827D79C780", "GroupTypeAndRole" );
            // Attribute: rocks.kfs.StepsToCare.Jobs.CareNeedAutomatedNotifications: Auto Assign Worker with Geofence
            RockMigrationHelper.AddOrUpdateEntityAttribute( "Rock.Model.ServiceJob", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Class", "rocks.kfs.StepsToCare.Jobs.CareNeedAutomatedProcesses", "Auto Assign Worker with Geofence", "Auto Assign Worker with Geofence", @"Care Need Workers can have Geofence locations assigned to them, if there are workers with geofences and this block setting is enabled it will auto assign workers to this need on new entries based on the requester home being in the geofence.", 0, @"True", "54E062CE-BBA1-4C65-A12D-1774A145930D", "AutoAssignWorkerGeofence" );
            // Attribute: rocks.kfs.StepsToCare.Jobs.CareNeedAutomatedNotifications: Auto Assign Worker (load balanced)
            RockMigrationHelper.AddOrUpdateEntityAttribute( "Rock.Model.ServiceJob", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Class", "rocks.kfs.StepsToCare.Jobs.CareNeedAutomatedProcesses", "Auto Assign Worker (load balanced)", "Auto Assign Worker (load balanced)", @"Use intelligent load balancing to auto assign care workers to a care need based on their workload and other parameters?", 0, @"True", "1FDE9E0C-D98A-4CB1-BABD-EA294F76653A", "AutoAssignWorker" );
            // Attribute: rocks.kfs.StepsToCare.Jobs.CareNeedAutomatedNotifications: Newly Assigned Need Notification
            RockMigrationHelper.AddOrUpdateEntityAttribute( "Rock.Model.ServiceJob", "72ED40C7-4D64-4D60-9411-4FFB2B9E833E", "Class", "rocks.kfs.StepsToCare.Jobs.CareNeedAutomatedProcesses", "Newly Assigned Need Notification", "Newly Assigned Need Notification", @"Select the system communication template for the new assignment notification.", 0, @"70CF2049-A443-4C87-82C3-0A8A22D8DAC8", "AC8DB5F1-1BF8-4AC0-B1CD-D0A28E850343", "NewAssignmentNotification" );
            // Attribute: rocks.kfs.StepsToCare.Jobs.CareNeedAutomatedNotifications: Load Balanced Workers assignment type
            RockMigrationHelper.AddOrUpdateEntityAttribute( "Rock.Model.ServiceJob", "7525C4CB-EE6B-41D4-9B64-A08048D5A5C0", "Class", "rocks.kfs.StepsToCare.Jobs.CareNeedAutomatedProcesses", "Load Balanced Workers assignment type", "Load Balanced Workers assignment type", @"How should the auto assign worker load balancing work? Default: Exclusive. ""Prioritize"", it will prioritize the workers being assigned based on campus, category and any other parameters on the worker but still assign to any worker if their workload matches. ""Exclusive"", if there are workers with matching campus, category or other parameters it will only load balance between those workers.", 0, @"Exclusive", "9A6C8A35-5197-4CBA-9BE6-5A9CAFF6A469", "LoadBalanceWorkersType" );
            // Attribute: rocks.kfs.StepsToCare.Jobs.CareNeedAutomatedNotifications: Adults in Family Worker Assignment
            RockMigrationHelper.AddOrUpdateEntityAttribute( "Rock.Model.ServiceJob", "7525C4CB-EE6B-41D4-9B64-A08048D5A5C0", "Class", "rocks.kfs.StepsToCare.Jobs.CareNeedAutomatedProcesses", "Adults in Family Worker Assignment", "Adults in Family Worker Assignment", @"How should workers be assigned to spouses and other adults in the family when using 'Family Needs'. Normal behavior, use the same settings as a normal Care Need (Group Leader, Geofence and load balanced), or assign to Care Workers Only (load balanced).", 0, @"Normal", "33649C3E-F221-4562-BCA1-B775127097C8", "AdultFamilyWorkers" );
            // Attribute: rocks.kfs.StepsToCare.Jobs.CareNeedAutomatedNotifications: Threshold of Days before Assignment
            RockMigrationHelper.AddOrUpdateEntityAttribute( "Rock.Model.ServiceJob", "A75DFC58-7A1B-4799-BF31-451B2BBE38FF", "Class", "rocks.kfs.StepsToCare.Jobs.CareNeedAutomatedProcesses", "Threshold of Days before Assignment", "Threshold of Days before Assignment", @"The number of days you can schedule a need in the future before a need will be assigned to workers.", 0, @"3", "D98AECCE-07D9-46A6-8401-6DE8615143F2", "FutureThresholdDays" );
            // Attribute: rocks.kfs.StepsToCare.Jobs.CareNeedAutomatedNotifications: Verbose Logging
            RockMigrationHelper.AddOrUpdateEntityAttribute( "Rock.Model.ServiceJob", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Class", "rocks.kfs.StepsToCare.Jobs.CareNeedAutomatedProcesses", "Verbose Logging", "Verbose Logging", @"Enable verbose Logging to help in determining issues with adding needs or auto assigning workers. Not recommended for normal use.", 0, @"False", "37CEC870-6B09-4231-ACC8-DEE0CAB55466", "VerboseLogging" );

            RockMigrationHelper.AddAttributeValue( "AC8DB5F1-1BF8-4AC0-B1CD-D0A28E850343", -1, SystemGuid.SystemCommunication.CARE_NEED_ASSIGNED, "AC8DB5F1-1BF8-4AC0-B1CD-D0A28E850343" ); // Care Need Automated Notifications: Newly Assigned Need Notification
            RockMigrationHelper.AddAttributeValue( "D3F19DE8-B8A0-4B87-AB4D-DE827D79C780", -1, @"6d798efa-0110-41d5-bce4-30acefe4317e", "D3F19DE8-B8A0-4B87-AB4D-DE827D79C780" ); // Care Need Automated Notifications: Group Type and Role
            RockMigrationHelper.AddAttributeValue( "54E062CE-BBA1-4C65-A12D-1774A145930D", -1, @"True", "54E062CE-BBA1-4C65-A12D-1774A145930D" ); // Care Need Automated Notifications: Auto Assign Worker with Geofence
            RockMigrationHelper.AddAttributeValue( "1FDE9E0C-D98A-4CB1-BABD-EA294F76653A", -1, @"True", "1FDE9E0C-D98A-4CB1-BABD-EA294F76653A" ); // Care Need Automated Notifications: Auto Assign Worker (load balanced)
            RockMigrationHelper.AddAttributeValue( "9A6C8A35-5197-4CBA-9BE6-5A9CAFF6A469", -1, @"Exclusive", "9A6C8A35-5197-4CBA-9BE6-5A9CAFF6A469" ); // Care Need Automated Notifications: Load Balanced Workers assignment type
            RockMigrationHelper.AddAttributeValue( "33649C3E-F221-4562-BCA1-B775127097C8", -1, @"Normal", "33649C3E-F221-4562-BCA1-B775127097C8" ); // Care Need Automated Notifications: Adults in Family Worker Assignment
            RockMigrationHelper.AddAttributeValue( "D98AECCE-07D9-46A6-8401-6DE8615143F2", -1, @"3", "D98AECCE-07D9-46A6-8401-6DE8615143F2" ); // Care Need Automated Notifications: Threshold of Days before Assignment
            RockMigrationHelper.AddAttributeValue( "37CEC870-6B09-4231-ACC8-DEE0CAB55466", -1, @"False", "37CEC870-6B09-4231-ACC8-DEE0CAB55466" ); // Care Need Automated Notifications: Verbose Logging

            Sql( @"DECLARE @JobId int = ( SELECT TOP 1 [Id] FROM [ServiceJob] WHERE [Guid] = '895C301C-02D1-4D9C-9FC4-DA7257368208' )
                 UPDATE [AttributeValue] SET EntityId = @JobId WHERE [Guid] = 'AC8DB5F1-1BF8-4AC0-B1CD-D0A28E850343'
                 UPDATE [AttributeValue] SET EntityId = @JobId WHERE [Guid] = 'D3F19DE8-B8A0-4B87-AB4D-DE827D79C780'
                 UPDATE [AttributeValue] SET EntityId = @JobId WHERE [Guid] = '54E062CE-BBA1-4C65-A12D-1774A145930D'
                 UPDATE [AttributeValue] SET EntityId = @JobId WHERE [Guid] = '1FDE9E0C-D98A-4CB1-BABD-EA294F76653A'
                 UPDATE [AttributeValue] SET EntityId = @JobId WHERE [Guid] = '9A6C8A35-5197-4CBA-9BE6-5A9CAFF6A469'
                 UPDATE [AttributeValue] SET EntityId = @JobId WHERE [Guid] = '33649C3E-F221-4562-BCA1-B775127097C8'
                 UPDATE [AttributeValue] SET EntityId = @JobId WHERE [Guid] = 'D98AECCE-07D9-46A6-8401-6DE8615143F2'
                 UPDATE [AttributeValue] SET EntityId = @JobId WHERE [Guid] = '37CEC870-6B09-4231-ACC8-DEE0CAB55466'" ); // Set EntityId to proper Job id

        }

        public override void Down()
        {
            Sql( @"IF EXISTS( SELECT [Id] FROM [ServiceJob] WHERE [Class] = 'rocks.kfs.StepsToCare.Jobs.CareNeedAutomatedProcesses' AND [Guid] = '895C301C-02D1-4D9C-9FC4-DA7257368208' )
            BEGIN
               UPDATE [ServiceJob]
                SET
                    [Name] = 'Care Need Automated Notifications'
                  , [Class] = 'rocks.kfs.StepsToCare.Jobs.CareNeedAutomatedNotifications'
                WHERE
                    [Guid] = '895C301C-02D1-4D9C-9FC4-DA7257368208'
            END" );

            RockMigrationHelper.DeleteAttribute( "D3F19DE8-B8A0-4B87-AB4D-DE827D79C780" ); // rocks.kfs.StepsToCare.Jobs.CareNeedAutomatedNotifications: Group Type and Role
            RockMigrationHelper.DeleteAttribute( "54E062CE-BBA1-4C65-A12D-1774A145930D" ); // rocks.kfs.StepsToCare.Jobs.CareNeedAutomatedNotifications: Auto Assign Worker with Geofence
            RockMigrationHelper.DeleteAttribute( "1FDE9E0C-D98A-4CB1-BABD-EA294F76653A" ); // rocks.kfs.StepsToCare.Jobs.CareNeedAutomatedNotifications: Auto Assign Worker (load balanced)
            RockMigrationHelper.DeleteAttribute( "AC8DB5F1-1BF8-4AC0-B1CD-D0A28E850343" ); // rocks.kfs.StepsToCare.Jobs.CareNeedAutomatedNotifications: Newly Assigned Need Notification
            RockMigrationHelper.DeleteAttribute( "9A6C8A35-5197-4CBA-9BE6-5A9CAFF6A469" ); // rocks.kfs.StepsToCare.Jobs.CareNeedAutomatedNotifications: Load Balanced Workers assignment type
            RockMigrationHelper.DeleteAttribute( "33649C3E-F221-4562-BCA1-B775127097C8" ); // rocks.kfs.StepsToCare.Jobs.CareNeedAutomatedNotifications: Adults in Family Worker Assignment
            RockMigrationHelper.DeleteAttribute( "D98AECCE-07D9-46A6-8401-6DE8615143F2" ); // rocks.kfs.StepsToCare.Jobs.CareNeedAutomatedNotifications: Threshold of Days before Assignment
            RockMigrationHelper.DeleteAttribute( "37CEC870-6B09-4231-ACC8-DEE0CAB55466" ); // rocks.kfs.StepsToCare.Jobs.CareNeedAutomatedNotifications: Verbose Logging

            Sql( @"UPDATE [Attribute] SET [EntityTypeQualifierValue] = 'rocks.kfs.StepsToCare.Jobs.CareNeedAutomatedNotifications' WHERE [Guid] = '125F5EB3-8223-40B9-8FD6-4EED93602D72'
                   UPDATE [Attribute] SET [EntityTypeQualifierValue] = 'rocks.kfs.StepsToCare.Jobs.CareNeedAutomatedNotifications' WHERE [Guid] = '8D050C0F-105B-4A8B-8879-A15F78F5913E'
                   UPDATE [Attribute] SET [EntityTypeQualifierValue] = 'rocks.kfs.StepsToCare.Jobs.CareNeedAutomatedNotifications' WHERE [Guid] = '08D1BB81-9FCA-488B-9383-9440D1DCCFC6'
                   UPDATE [Attribute] SET [EntityTypeQualifierValue] = 'rocks.kfs.StepsToCare.Jobs.CareNeedAutomatedNotifications' WHERE [Guid] = 'A29278C1-9A80-4BEA-81A9-1B7FCB6CA305'
                   UPDATE [Attribute] SET [EntityTypeQualifierValue] = 'rocks.kfs.StepsToCare.Jobs.CareNeedAutomatedNotifications' WHERE [Guid] = 'F64479EE-A595-4AA3-97D6-CAA768464284'
                   UPDATE [Attribute] SET [EntityTypeQualifierValue] = 'rocks.kfs.StepsToCare.Jobs.CareNeedAutomatedNotifications' WHERE [Guid] = '5D8548E7-9570-4337-888B-A51868A57CD0'
                   UPDATE [Attribute] SET [EntityTypeQualifierValue] = 'rocks.kfs.StepsToCare.Jobs.CareNeedAutomatedNotifications' WHERE [Guid] = '245DFE71-FF8D-41FE-91A8-E4D29B1AF016'
                   UPDATE [Attribute] SET [EntityTypeQualifierValue] = 'rocks.kfs.StepsToCare.Jobs.CareNeedAutomatedNotifications' WHERE [Guid] = 'AF3ECA88-C715-4E2D-9D9B-C4CC3141C6EC'" );
        }
    }
}