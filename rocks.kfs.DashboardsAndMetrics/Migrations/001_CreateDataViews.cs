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

namespace rocks.kfs.DashboardsAndMetrics.Migrations
{
    [MigrationNumber( 1, "1.12.8" )]
    public partial class CreateDataViews : Migration
    {
        /// <summary>
        /// Operations to be performed during the upgrade process.
        /// </summary>
        public override void Up()
        {
            #region Categories

            // Create Metrics View category
            RockMigrationHelper.UpdateCategory( "57F8FA29-DCF1-4F74-8553-87E90F234139", "Metric Views", "", "Metric Data Views used for KFS dashboards.", "D900BCBC-59F4-4975-808B-0041F601D044" );

            // Create sub-categories
            RockMigrationHelper.UpdateCategory( "57F8FA29-DCF1-4F74-8553-87E90F234139", "KFS Financial Dashboard", "", "Financial metric Data Views for KFS dashboards.", "B229F71C-2A6A-46F0-8D4A-898673D94F68", parentCategoryGuid: "D900BCBC-59F4-4975-808B-0041F601D044" );
            RockMigrationHelper.UpdateCategory( "57F8FA29-DCF1-4F74-8553-87E90F234139", "KFS Group Metrics Dashboard", "", "Group metric Data Views for KFS dashboards.", "D206BCC0-C784-4E90-ADC4-0EB0BC74517A", parentCategoryGuid: "D900BCBC-59F4-4975-808B-0041F601D044" );

            #endregion Categories

            #region Giving DataViews

            // Create [GroupAll] DataViewFilter for DataView: First Time Givers Previous Week
            Sql( @"IF NOT EXISTS (SELECT * FROM DataViewFilter where [Guid] = 'DD4B4DC8-62B8-402E-A8AB-7539165699D8')
                    BEGIN
                    DECLARE @ParentDataViewFilterId int = (select Id from DataViewFilter where [Guid] = '00000000-0000-0000-0000-000000000000'),
                            @DataViewFilterEntityTypeId int = (select Id from EntityType where [Guid] = '00000000-0000-0000-0000-000000000000')

                    INSERT INTO [DataViewFilter] (
                        ExpressionType,
                        ParentId,
                        EntityTypeId,
                        Selection,
                        [Guid])
                    VALUES (
                        1,
                        @ParentDataViewFilterId,
                        @DataViewFilterEntityTypeId,
                        '',
                        'DD4B4DC8-62B8-402E-A8AB-7539165699D8')
                    END");

            // Create Rock.Reporting.DataFilter.Person.FirstContributionDateFilter DataViewFilter for DataView: First Time Givers Previous Week
            Sql( @"IF NOT EXISTS (SELECT * FROM DataViewFilter where [Guid] = '06C826D5-89C7-4A00-AC20-9D3655F81348')
                    BEGIN
                    DECLARE @ParentDataViewFilterId int = (select Id from DataViewFilter where [Guid] = 'DD4B4DC8-62B8-402E-A8AB-7539165699D8'),
                            @DataViewFilterEntityTypeId int = (select Id from EntityType where [Guid] = 'B4B70487-E620-4BC1-8983-124578118BC0')

                    INSERT INTO [DataViewFilter] (
                        ExpressionType,
                        ParentId,
                        EntityTypeId,
                        Selection,
                        [Guid])
                    VALUES (
                        0,
                        @ParentDataViewFilterId,
                        @DataViewFilterEntityTypeId,
                        '{""AccountGuids"":[""4410306f-3fb5-4a57-9a80-09a3f9d40d0c""],""DelimitedValues"":""Previous|1|Week||"",""DateRangeMode"":4,""NumberOfTimeUnits"":1,""TimeUnit"":2,""StartDate"":null,""EndDate"":null,""UseSundayDate"":true}',
                        '06C826D5-89C7-4A00-AC20-9D3655F81348')
                    END");

            // Create DataView: First Time Givers Previous Week
            Sql( @"IF NOT EXISTS (SELECT * FROM DataView where [Guid] = '82D0EF26-3234-49C1-94DE-B03740830BBC')
                    BEGIN
                    DECLARE @categoryId int = (select top 1 [Id] from [Category] where [Guid] = 'B229F71C-2A6A-46F0-8D4A-898673D94F68'),
                            @entityTypeId  int = (select top 1 [Id] from [EntityType] where [Guid] = '72657ED8-D16E-492E-AC12-144C5E7567E7'),
                            @dataViewFilterId  int = (select top 1 [Id] from [DataViewFilter] where [Guid] = 'DD4B4DC8-62B8-402E-A8AB-7539165699D8'),
                            @transformEntityTypeId  int = (select top 1 [Id] from [EntityType] where [Guid] = '00000000-0000-0000-0000-000000000000')

                    INSERT INTO [DataView] (
                        [IsSystem],
                        [Name],
                        [Description],
                        [CategoryId],
                        [EntityTypeId],
                        [DataViewFilterId],
                        [TransformEntityTypeId],
                        [Guid])
                    VALUES(
                        0,
                        'First Time Givers Previous Week',
                        '',
                        @categoryId,
                        @entityTypeId,
                        @dataViewFilterId,
                        @transformEntityTypeId,
                        '82D0EF26-3234-49C1-94DE-B03740830BBC')
                    END");

            // Create [GroupAll] DataViewFilter for DataView: Large Gifts Last Week
            Sql( @"IF NOT EXISTS (SELECT * FROM DataViewFilter where [Guid] = 'B75D02B7-EB92-4C90-AC84-A3AF708E836E')
                    BEGIN
                    DECLARE @ParentDataViewFilterId int = (select Id from DataViewFilter where [Guid] = '00000000-0000-0000-0000-000000000000'),
                            @DataViewFilterEntityTypeId int = (select Id from EntityType where [Guid] = '00000000-0000-0000-0000-000000000000')

                    INSERT INTO [DataViewFilter] (
                        ExpressionType,
                        ParentId,
                        EntityTypeId,
                        Selection,
                        [Guid])
                    VALUES (
                        1,
                        @ParentDataViewFilterId,
                        @DataViewFilterEntityTypeId,
                        '',
                        'B75D02B7-EB92-4C90-AC84-A3AF708E836E')
                    END");

            // Create Rock.Reporting.DataFilter.Person.GivingAmountFilter DataViewFilter for DataView: Large Gifts Last Week
            Sql( @"IF NOT EXISTS (SELECT * FROM DataViewFilter where [Guid] = '900B5D94-F44B-40D0-8A3F-190BAB9292D3')
                    BEGIN
                    DECLARE @ParentDataViewFilterId int = (select Id from DataViewFilter where [Guid] = 'B75D02B7-EB92-4C90-AC84-A3AF708E836E'),
                            @DataViewFilterEntityTypeId int = (select Id from EntityType where [Guid] = '1087DEE3-9932-4647-88A3-7CD87AB16B7D')

                    INSERT INTO [DataViewFilter] (
                        ExpressionType,
                        ParentId,
                        EntityTypeId,
                        Selection,
                        [Guid])
                    VALUES (
                        0,
                        @ParentDataViewFilterId,
                        @DataViewFilterEntityTypeId,
                        '{""ComparisonType"":256,""Amount"":1000.00,""SlidingDateRangePickerDelimitedValues"":""Previous|1|Week||"",""AccountGuids"":[""4410306f-3fb5-4a57-9a80-09a3f9d40d0c""],""CombineGiving"":false,""UseAnalyticsModels"":false,""IncludeChildAccounts"":false,""IgnoreInactiveAccounts"":false}',
                        '900B5D94-F44B-40D0-8A3F-190BAB9292D3')
                    END");

            // Create DataView: Large Gifts Last Week
            Sql( @"IF NOT EXISTS (SELECT * FROM DataView where [Guid] = '928099C8-6851-40BA-9D02-7832A60BB9A2')
                    BEGIN
                    DECLARE @categoryId int = (select top 1 [Id] from [Category] where [Guid] = 'B229F71C-2A6A-46F0-8D4A-898673D94F68'),
                            @entityTypeId int = (select top 1 [Id] from [EntityType] where [Guid] = '72657ED8-D16E-492E-AC12-144C5E7567E7'),
                            @dataViewFilterId int = (select top 1 [Id] from [DataViewFilter] where [Guid] = 'B75D02B7-EB92-4C90-AC84-A3AF708E836E'),
                            @transformEntityTypeId int = (select top 1 [Id] from [EntityType] where [Guid] = '00000000-0000-0000-0000-000000000000')

                    INSERT INTO [DataView] (
                        [IsSystem],
                        [Name],
                        [Description],
                        [CategoryId],
                        [EntityTypeId],
                        [DataViewFilterId],
                        [TransformEntityTypeId],
                        [Guid])
                    VALUES(
                        0,
                        'Large Gifts Last Week',
                        'Gifts over $1000',
                        @categoryId,
                        @entityTypeId,
                        @dataViewFilterId,
                        @transformEntityTypeId,
                        '928099C8-6851-40BA-9D02-7832A60BB9A2')
                    END");

            #endregion Giving DataViews

            #region Groups DataViews

            // Create [GroupAll] DataViewFilter for DataView: Active Small Group Members
            Sql( @"IF NOT EXISTS (SELECT * FROM DataViewFilter where [Guid] = '7FFFAC89-59D7-4EE1-A5FB-08539E417182')
                    BEGIN
                    DECLARE @ParentDataViewFilterId int = (select Id from DataViewFilter where [Guid] = '00000000-0000-0000-0000-000000000000'),
                            @DataViewFilterEntityTypeId int = (select Id from EntityType where [Guid] = '00000000-0000-0000-0000-000000000000')

                    INSERT INTO [DataViewFilter] (
                        ExpressionType,
                        ParentId,
                        EntityTypeId,
                        Selection,
                        [Guid])
                    VALUES (
                        1,
                        @ParentDataViewFilterId,
                        @DataViewFilterEntityTypeId,
                        '',
                        '7FFFAC89-59D7-4EE1-A5FB-08539E417182')
                    END");

            // Create Rock.Reporting.DataFilter.Person.InGroupGroupTypeFilter DataViewFilter for DataView: Active Small Group Members
            Sql( @"IF NOT EXISTS (SELECT * FROM DataViewFilter where [Guid] = '6AFB6BD2-5445-4FF8-8C16-FF81E9CA3CBA')
                    BEGIN
                    DECLARE @ParentDataViewFilterId int = (select Id from DataViewFilter where [Guid] = '7FFFAC89-59D7-4EE1-A5FB-08539E417182'),
                            @DataViewFilterEntityTypeId int = (select Id from EntityType where [Guid] = '0E239967-6D33-4205-B19F-08AD8FF6ED0B')

                    INSERT INTO [DataViewFilter] (
                        ExpressionType,
                        ParentId,
                        EntityTypeId,
                        Selection,
                        [Guid])
                    VALUES (
                        0,
                        @ParentDataViewFilterId,
                        @DataViewFilterEntityTypeId,
                        '50fcfb30-f51a-49df-86f4-2b176ea1820b||1|True',
                        '6AFB6BD2-5445-4FF8-8C16-FF81E9CA3CBA')
                    END");

            // Create DataView: Active Small Group Members
            Sql( @"IF NOT EXISTS (SELECT * FROM DataView where [Guid] = 'D341C232-92A0-498D-9994-D3B2BA3419C9')
                    BEGIN
                    DECLARE @categoryId int = (select top 1 [Id] from [Category] where [Guid] = 'D206BCC0-C784-4E90-ADC4-0EB0BC74517A'),
                            @entityTypeId int = (select top 1 [Id] from [EntityType] where [Guid] = '72657ED8-D16E-492E-AC12-144C5E7567E7'),
                            @dataViewFilterId int = (select top 1 [Id] from [DataViewFilter] where [Guid] = '7FFFAC89-59D7-4EE1-A5FB-08539E417182'),
                            @transformEntityTypeId  int = (select top 1 [Id] from [EntityType] where [Guid] = '00000000-0000-0000-0000-000000000000')

                    INSERT INTO [DataView] (
                        [IsSystem],
                        [Name],
                        [Description],
                        [CategoryId],
                        [EntityTypeId],
                        [DataViewFilterId],
                        [TransformEntityTypeId],
                        [Guid])
                    VALUES(
                        0,
                        'Active Small Group Members',
                        '',
                        @categoryId,
                        @entityTypeId,
                        @dataViewFilterId,
                        @transformEntityTypeId,
                        'D341C232-92A0-498D-9994-D3B2BA3419C9')
                    END");

            // Create [GroupAll] DataViewFilter for DataView: Active Small Groups
            Sql( @"IF NOT EXISTS (SELECT * FROM DataViewFilter where [Guid] = '12B10297-8366-45C0-9682-CEF35CE27875')
                    BEGIN
                    DECLARE @ParentDataViewFilterId int = (select Id from DataViewFilter where [Guid] = '00000000-0000-0000-0000-000000000000'),
                            @DataViewFilterEntityTypeId int = (select Id from EntityType where [Guid] = '00000000-0000-0000-0000-000000000000')

                    INSERT INTO [DataViewFilter] (
                        ExpressionType,
                        ParentId,
                        EntityTypeId,
                        Selection,
                        [Guid])
                    VALUES (
                        1,
                        @ParentDataViewFilterId,
                        @DataViewFilterEntityTypeId,
                        '',
                        '12B10297-8366-45C0-9682-CEF35CE27875')
                    END");

            // Create Rock.Reporting.DataFilter.Group.GroupTypeFilter DataViewFilter for DataView: Active Small Groups
            Sql( @"IF NOT EXISTS (SELECT * FROM DataViewFilter where [Guid] = '37F82F17-6082-4C0D-9E1C-937C368EE668')
                    BEGIN
                    DECLARE @ParentDataViewFilterId int = (select Id from DataViewFilter where [Guid] = '12B10297-8366-45C0-9682-CEF35CE27875'),
                            @DataViewFilterEntityTypeId int = (select Id from EntityType where [Guid] = '6880CE3A-366B-4D21-8CAC-DEC7D18173C3')

                    INSERT INTO [DataViewFilter] (
                        ExpressionType,
                        ParentId,
                        EntityTypeId,
                        Selection,
                        [Guid])
                    VALUES (
                        0,
                        @ParentDataViewFilterId,
                        @DataViewFilterEntityTypeId,
                        '50fcfb30-f51a-49df-86f4-2b176ea1820b',
                        '37F82F17-6082-4C0D-9E1C-937C368EE668')
                    END");

            // Create Rock.Reporting.DataFilter.PropertyFilter DataViewFilter for DataView: Active Small Groups
            Sql( @"IF NOT EXISTS (SELECT * FROM DataViewFilter where [Guid] = 'C04C0E0E-C481-4E76-B3A7-18A9B604EF2C')
                    BEGIN
                    DECLARE @ParentDataViewFilterId int = (select Id from DataViewFilter where [Guid] = '12B10297-8366-45C0-9682-CEF35CE27875'),
                            @DataViewFilterEntityTypeId int = (select Id from EntityType where [Guid] = '03F0D6AC-D181-48B6-B4BC-1F2652B55323')

                    INSERT INTO [DataViewFilter] (
                        ExpressionType,
                        ParentId,
                        EntityTypeId,
                        Selection,
                        [Guid])
                    VALUES (
                        0,
                        @ParentDataViewFilterId,
                        @DataViewFilterEntityTypeId,
                        '[""Property_IsActive"",""1"",""True""]',
                        'C04C0E0E-C481-4E76-B3A7-18A9B604EF2C')
                    END");

            // Create DataView: Active Small Groups
            Sql( @"IF NOT EXISTS (SELECT * FROM DataView where [Guid] = '29F360C7-BB4D-4E9B-ADD6-2E73FE64F52D')
                    BEGIN
                    DECLARE @categoryId int = (select top 1 [Id] from [Category] where [Guid] = 'D206BCC0-C784-4E90-ADC4-0EB0BC74517A'),
                            @entityTypeId  int = (select top 1 [Id] from [EntityType] where [Guid] = '9BBFDA11-0D22-40D5-902F-60ADFBC88987'),
                            @dataViewFilterId  int = (select top 1 [Id] from [DataViewFilter] where [Guid] = '12B10297-8366-45C0-9682-CEF35CE27875'),
                            @transformEntityTypeId  int = (select top 1 [Id] from [EntityType] where [Guid] = '00000000-0000-0000-0000-000000000000')
                    INSERT INTO [DataView] (
                        [IsSystem],
                        [Name],
                        [Description],
                        [CategoryId],
                        [EntityTypeId],
                        [DataViewFilterId],
                        [TransformEntityTypeId],
                        [Guid])
                    VALUES(
                        0,
                        'Active Small Groups',
                        '',
                        @categoryId,
                        @entityTypeId,
                        @dataViewFilterId,
                        @transformEntityTypeId,
                        '29F360C7-BB4D-4E9B-ADD6-2E73FE64F52D')
                    END");

            // Create [GroupAll] DataViewFilter for DataView: Attendance Previous Week
            Sql( @"IF NOT EXISTS (SELECT * FROM DataViewFilter where [Guid] = 'CBD76476-5BE0-4975-B6F6-0E10EEDF8A1F')
                    BEGIN
                    DECLARE @ParentDataViewFilterId int = (select Id from DataViewFilter where [Guid] = '00000000-0000-0000-0000-000000000000'),
                            @DataViewFilterEntityTypeId int = (select Id from EntityType where [Guid] = '00000000-0000-0000-0000-000000000000')

                    INSERT INTO [DataViewFilter] (
                        ExpressionType,
                        ParentId,
                        EntityTypeId,
                        Selection,
                        [Guid])
                    VALUES (
                        1,
                        @ParentDataViewFilterId,
                        @DataViewFilterEntityTypeId,
                        '',
                        'CBD76476-5BE0-4975-B6F6-0E10EEDF8A1F')
                    END");

            // Create Rock.Reporting.DataFilter.Person.GroupTypeAttendanceFilter DataViewFilter for DataView: Attendance Previous Week
            Sql( @"IF NOT EXISTS (SELECT * FROM DataViewFilter where [Guid] = '3FAE921A-865E-4FC4-97A1-AFF69B617B46')
                    BEGIN
                    DECLARE @ParentDataViewFilterId int = (select Id from DataViewFilter where [Guid] = 'CBD76476-5BE0-4975-B6F6-0E10EEDF8A1F'),
                            @DataViewFilterEntityTypeId int = (select Id from EntityType where [Guid] = '55E78F48-F849-444A-B285-FDA99E7236E7')

                    INSERT INTO [DataViewFilter] (
                        ExpressionType,
                        ParentId,
                        EntityTypeId,
                        Selection,
                        [Guid])
                    VALUES (
                        0,
                        @ParentDataViewFilterId,
                        @DataViewFilterEntityTypeId,
                        '50fcfb30-f51a-49df-86f4-2b176ea1820b|256|1|Previous,1,Week,,|False',
                        '3FAE921A-865E-4FC4-97A1-AFF69B617B46')
                    END");

            // Create DataView: Attendance Previous Week
            Sql( @"IF NOT EXISTS (SELECT * FROM DataView where [Guid] = '41882678-4668-4B52-89FA-ABB4718321F5')
                    BEGIN
                    DECLARE @categoryId int = (select top 1 [Id] from [Category] where [Guid] = 'D206BCC0-C784-4E90-ADC4-0EB0BC74517A'),
                            @entityTypeId  int = (select top 1 [Id] from [EntityType] where [Guid] = '72657ED8-D16E-492E-AC12-144C5E7567E7'),
                            @dataViewFilterId  int = (select top 1 [Id] from [DataViewFilter] where [Guid] = 'CBD76476-5BE0-4975-B6F6-0E10EEDF8A1F'),
                            @transformEntityTypeId  int = (select top 1 [Id] from [EntityType] where [Guid] = '00000000-0000-0000-0000-000000000000')

                    INSERT INTO [DataView] (
                        [IsSystem],
                        [Name],
                        [Description],
                        [CategoryId],
                        [EntityTypeId],
                        [DataViewFilterId],
                        [TransformEntityTypeId],
                        [Guid])
                    VALUES(
                        0,
                        'Attendance Previous Week',
                        '',
                        @categoryId,
                        @entityTypeId,
                        @dataViewFilterId,
                        @transformEntityTypeId,
                        '41882678-4668-4B52-89FA-ABB4718321F5')
                    END");

            #endregion Groups DataViews
        }

        /// <summary>
        /// Operations to be performed during the downgrade process.
        /// </summary>
        public override void Down()
        {
            #region Groups DataViews
            // Delete DataView: Attendance Previous Week
            Sql( @"DELETE FROM DataView where [Guid] = '41882678-4668-4B52-89FA-ABB4718321F5'");

            // Delete DataViewFilter for DataView: Attendance Previous Week
            Sql( @"DELETE FROM DataViewFilter where [Guid] = '3FAE921A-865E-4FC4-97A1-AFF69B617B46'");

            // Delete DataViewFilter for DataView: Attendance Previous Week
            Sql( @"DELETE FROM DataViewFilter where [Guid] = 'CBD76476-5BE0-4975-B6F6-0E10EEDF8A1F'");

            // Delete DataView: Active Small Groups
            Sql( @"DELETE FROM DataView where [Guid] = '29F360C7-BB4D-4E9B-ADD6-2E73FE64F52D'");

            // Delete DataViewFilter for DataView: Active Small Groups
            Sql( @"DELETE FROM DataViewFilter where [Guid] = 'C04C0E0E-C481-4E76-B3A7-18A9B604EF2C'");

            // Delete DataViewFilter for DataView: Active Small Groups
            Sql( @"DELETE FROM DataViewFilter where [Guid] = '37F82F17-6082-4C0D-9E1C-937C368EE668'");

            // Delete DataViewFilter for DataView: Active Small Groups
            Sql( @"DELETE FROM DataViewFilter where [Guid] = '12B10297-8366-45C0-9682-CEF35CE27875'");

            // Delete DataView: Active Small Group Members
            Sql( @"DELETE FROM DataView where [Guid] = 'D341C232-92A0-498D-9994-D3B2BA3419C9'");

            // Delete DataViewFilter for DataView: Active Small Group Members
            Sql( @"DELETE FROM DataViewFilter where [Guid] = '6AFB6BD2-5445-4FF8-8C16-FF81E9CA3CBA'");

            // Delete DataViewFilter for DataView: Active Small Group Members
            Sql( @"DELETE FROM DataViewFilter where [Guid] = '7FFFAC89-59D7-4EE1-A5FB-08539E417182'");

            #endregion Groups DataViews

            #region Giving DataViews

            // Delete DataView: Large Gifts Last Week
            Sql( @"DELETE FROM DataView where [Guid] = '928099C8-6851-40BA-9D02-7832A60BB9A2'");

            // Delete DataViewFilter for DataView: Large Gifts Last Week
            Sql( @"DELETE FROM DataViewFilter where [Guid] = '900B5D94-F44B-40D0-8A3F-190BAB9292D3'");

            // Delete DataViewFilter for DataView: Large Gifts Last Week
            Sql( @"DELETE FROM DataViewFilter where [Guid] = 'B75D02B7-EB92-4C90-AC84-A3AF708E836E'");

            // Delete DataView: First Time Givers Previous Week
            Sql( @"DELETE FROM DataView where [Guid] = '82D0EF26-3234-49C1-94DE-B03740830BBC'");

            // Delete DataViewFilter for DataView: First Time Givers Previous Week
            Sql( @"DELETE FROM DataViewFilter where [Guid] = '06C826D5-89C7-4A00-AC20-9D3655F81348'");

            // Delete DataViewFilter for DataView: First Time Givers Previous Week
            Sql( @"DELETE FROM DataViewFilter where [Guid] = 'DD4B4DC8-62B8-402E-A8AB-7539165699D8'");

            #endregion Giving DataViews

            #region Categories

            RockMigrationHelper.DeleteCategory( "D206BCC0-C784-4E90-ADC4-0EB0BC74517A" );
            RockMigrationHelper.DeleteCategory( "B229F71C-2A6A-46F0-8D4A-898673D94F68" );
            RockMigrationHelper.DeleteCategory( "D900BCBC-59F4-4975-808B-0041F601D044" );

            #endregion Categories
        }
    }
}