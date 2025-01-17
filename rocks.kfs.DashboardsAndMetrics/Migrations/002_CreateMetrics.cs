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
    [MigrationNumber( 2, "1.12.8" )]
    public partial class CreateMetrics : Migration
    {
        /// <summary>
        /// Operations to be performed during the upgrade process.
        /// </summary>
        public override void Up()
        {
            #region Categories

            // Create Dashboards category
            RockMigrationHelper.UpdateCategory( "3D35C859-DF37-433F-A20A-0FFD0FCB9862", "KFS Dashboards", "", "Metrics used for Kingdom First Solutions dashboards.", "3E650C53-9D8F-4946-8F6E-432B36CDC9E1" );

            // Create sub-categories
            RockMigrationHelper.UpdateCategory( "3D35C859-DF37-433F-A20A-0FFD0FCB9862", "KFS Groups Dashboard Metrics", "", "Group metrics for KFS dashboards.", "5A7A99D2-33C7-4684-9500-E200503C311F", parentCategoryGuid: "3E650C53-9D8F-4946-8F6E-432B36CDC9E1" );
            RockMigrationHelper.UpdateCategory( "3D35C859-DF37-433F-A20A-0FFD0FCB9862", "KFS Financial Dashboard Metrics", "", "Financial metrics for KFS dashboards.", "B6F5E7C3-4AF0-490E-8634-5405A1E84D88", parentCategoryGuid: "3E650C53-9D8F-4946-8F6E-432B36CDC9E1" );

            #endregion Categories

            #region Giving Metrics

            // Contributions Last Week: Create Schedule
            Sql( @"IF NOT EXISTS (SELECT * FROM Schedule where [Guid] = '077AB9CB-842D-4B78-8D95-83E3ED5B59B0')
                    BEGIN
                    DECLARE @CategoryId int = (select Id from Category where [Guid] = '5A794741-5444-43F0-90D7-48E47276D426')

                    INSERT INTO [Schedule] (
                        Name,
                        Description,
                        iCalendarContent,
                        EffectiveStartDate,
                        CategoryId,
                        [Guid],
                        IsActive,
                        [Order],
                        AutoInactivateWhenComplete)
                    VALUES (
                        'Monday Metrics - 1AM',
                        '',
                        'BEGIN:VCALENDAR
PRODID:-//github.com/rianjs/ical.net//NONSGML ical.net 2.2//EN
VERSION:2.0
BEGIN:VEVENT
DTEND:20220901T010001
DTSTAMP:20220901T133602
DTSTART:20220901T010000
RRULE:FREQ=WEEKLY;BYDAY=MO
SEQUENCE:0
UID:54a91b6c-d4b0-4c5b-9125-98aa1b00377d
END:VEVENT
END:VCALENDAR
',
                        GETDATE(),
                        @CategoryId,
                        '077AB9CB-842D-4B78-8D95-83E3ED5B59B0',
                        1,
                        0,
                        0)
                    END" );

            // Contributions Last Week: Create Metric
            Sql( @"IF NOT EXISTS (SELECT * FROM Metric where [Guid] = '56EF54EA-715B-4CF2-B401-60E3D3AF1389')
                    BEGIN
                    DECLARE @ScheduleId int = (select Id from Schedule where Guid = '077AB9CB-842D-4B78-8D95-83E3ED5B59B0'),
                            @SQLMetricValueSourceDVId int = (select Id from DefinedValue where [Guid] = '6A1E1A1B-A636-4E12-B90C-D7FD1BDAE764')

                    INSERT INTO [Metric] (
                        IsSystem,
                        Title,
                        Subtitle,
                        Description,
                        IsCumulative,
                        SourceValueTypeId,
                        SourceSql,
                        YAxisLabel,
                        ScheduleId,
                        [Guid],
                        NumericDataType,
                        EnableAnalytics,
                        AutoPartitionOnPrimaryCampus)
                    VALUES (
                        0,
                        'Contributions Last Week',
                        'All Contributions Accounts',
                        '',
                        0,
                        @SQLMetricValueSourceDVId,
                        'SELECT SUM(Amount)
  FROM [AnalyticsFactFinancialTransaction]
  WHERE TransactionTypeValueId = 53
  AND TransactionDateTime <= DATEADD(wk, DATEDIFF(wk, 6, GETDATE()), 6)
  AND TransactionDateTime > DateAdd(wk, -1, DateAdd(wk, DateDiff(wk, 6, GETDATE()), 6))',
                        'Dollars',
                        @ScheduleId,
                        '56EF54EA-715B-4CF2-B401-60E3D3AF1389',
                        0,
                        0,
                        0)
                    END" );

            // Contributions Last Week: Create MetricCategory
            Sql( @"IF NOT EXISTS (SELECT * FROM MetricCategory where [Guid] = '070D8296-3D8F-47DD-A79A-D482CAB47349')
                    BEGIN
                    DECLARE @MetricId int = (select Id from Metric where Guid = '56EF54EA-715B-4CF2-B401-60E3D3AF1389'),
                            @CategoryId int = (select Id from Category where Guid = 'B6F5E7C3-4AF0-490E-8634-5405A1E84D88')

                    INSERT INTO [MetricCategory] (
                        MetricId,
                        CategoryId,
                        [Order],
                        [Guid])
                    VALUES (
                        @MetricId,
                        @CategoryId,
                        0,
                        '070D8296-3D8F-47DD-A79A-D482CAB47349')
                    END" );

            // Contributions Last Week: Create MetricPartition
            Sql( @"IF NOT EXISTS (SELECT * FROM MetricPartition where [Guid] = '3575D7C5-CFC2-4017-98CB-E172DB1602B7')
                    BEGIN
                    DECLARE @MetricId int = (select Id from Metric where Guid = '56EF54EA-715B-4CF2-B401-60E3D3AF1389')

                    INSERT INTO [MetricPartition] (
                        MetricId,
                        IsRequired,
                        [Order],
                        [Guid])
                    VALUES (
                        @MetricId,
                        1,
                        0,
                        '3575D7C5-CFC2-4017-98CB-E172DB1602B7')
                    END" );

            // First Time Givers Previous Week: Create Metric
            Sql( @"IF NOT EXISTS (SELECT * FROM Metric where [Guid] = '7C85C112-2D1E-4F34-9BA3-FE9357CD570A')
                    BEGIN
                    DECLARE @ScheduleId int = (select Id from Schedule where Guid = '077AB9CB-842D-4B78-8D95-83E3ED5B59B0'),
                            @DataViewMetricValueSourceDVId int = (select Id from DefinedValue where [Guid] = '2EC60BCF-EF63-4CCC-A970-F152292765D0'),
                            @DataViewId int = (select Id from DataView where [Guid] = '82D0EF26-3234-49C1-94DE-B03740830BBC')

                    INSERT INTO [Metric] (
                        IsSystem,
                        Title,
                        Subtitle,
                        Description,
                        IsCumulative,
                        SourceValueTypeId,
                        DataViewId,
                        YAxisLabel,
                        ScheduleId,
                        [Guid],
                        NumericDataType,
                        EnableAnalytics,
                        AutoPartitionOnPrimaryCampus)
                    VALUES (
                        0,
                        'First Time Givers Previous Week',
                        '',
                        '',
                        0,
                        @DataViewMetricValueSourceDVId,
                        @DataViewId,
                        'Givers',
                        @ScheduleId,
                        '7C85C112-2D1E-4F34-9BA3-FE9357CD570A',
                        0,
                        0,
                        0)
                    END" );

            // First Time Givers Previous Week: Create MetricCategory
            Sql( @"IF NOT EXISTS (SELECT * FROM MetricCategory where [Guid] = 'EBFB1A17-1A23-4563-8E8C-34C2413FC0CD')
                    BEGIN
                    DECLARE @MetricId int = (select Id from Metric where Guid = '7C85C112-2D1E-4F34-9BA3-FE9357CD570A'),
                            @CategoryId int = (select Id from Category where Guid = 'B6F5E7C3-4AF0-490E-8634-5405A1E84D88')

                    INSERT INTO [MetricCategory] (
                        MetricId,
                        CategoryId,
                        [Order],
                        [Guid])
                    VALUES (
                        @MetricId,
                        @CategoryId,
                        0,
                        'EBFB1A17-1A23-4563-8E8C-34C2413FC0CD')
                    END" );

            // First Time Givers Previous Week: Create MetricPartition
            Sql( @"IF NOT EXISTS (SELECT * FROM MetricPartition where [Guid] = '0BC8577F-4300-459C-96A0-DC96DF797138')
                    BEGIN
                    DECLARE @MetricId int = (select Id from Metric where Guid = '7C85C112-2D1E-4F34-9BA3-FE9357CD570A')

                    INSERT INTO [MetricPartition] (
                        MetricId,
                        IsRequired,
                        [Order],
                        [Guid])
                    VALUES (
                        @MetricId,
                        1,
                        0,
                        '0BC8577F-4300-459C-96A0-DC96DF797138')
                    END" );

            // Giving Units Giving Last Week: Create Metric
            Sql( @"IF NOT EXISTS (SELECT * FROM Metric where [Guid] = '62D95833-346B-4244-91A0-89B8CB72B561')
                    BEGIN
                    DECLARE @ScheduleId int = (select Id from Schedule where Guid = '077AB9CB-842D-4B78-8D95-83E3ED5B59B0'),
                            @SQLMetricValueSourceDVId int = (select Id from DefinedValue where [Guid] = '6A1E1A1B-A636-4E12-B90C-D7FD1BDAE764')

                    INSERT INTO [Metric] (
                        IsSystem,
                        Title,
                        Subtitle,
                        Description,
                        IsCumulative,
                        SourceValueTypeId,
                        SourceSql,
                        YAxisLabel,
                        ScheduleId,
                        [Guid],
                        NumericDataType,
                        EnableAnalytics,
                        AutoPartitionOnPrimaryCampus)
                    VALUES (
                        0,
                        'Giving Units Giving Last Week',
                        '',
                        '',
                        0,
                        @SQLMetricValueSourceDVId,
                        'SELECT COUNT(DISTINCT GivingGroupId)
  FROM [AnalyticsFactFinancialTransaction]
  WHERE TransactionTypeValueId = 53
  AND TransactionDateTime <= DATEADD(wk, DATEDIFF(wk, 6, GETDATE()), 6)
  AND TransactionDateTime > DateAdd(wk, -1, DateAdd(wk, DateDiff(wk, 6, GETDATE()), 6))',
                        'Giving Units',
                        @ScheduleId,
                        '62D95833-346B-4244-91A0-89B8CB72B561',
                        0,
                        0,
                        0)
                    END" );

            // Giving Units Giving Last Week: Create MetricCategory
            Sql( @"IF NOT EXISTS (SELECT * FROM MetricCategory where [Guid] = 'BE45522A-101F-4B57-A7E6-BB19AE80C0D3')
                    BEGIN
                    DECLARE @MetricId int = (select Id from Metric where Guid = '62D95833-346B-4244-91A0-89B8CB72B561'),
                            @CategoryId int = (select Id from Category where Guid = 'B6F5E7C3-4AF0-490E-8634-5405A1E84D88')

                    INSERT INTO [MetricCategory] (
                        MetricId,
                        CategoryId,
                        [Order],
                        [Guid])
                    VALUES (
                        @MetricId,
                        @CategoryId,
                        0,
                        'BE45522A-101F-4B57-A7E6-BB19AE80C0D3')
                    END" );

            // Giving Units Giving Last Week: Create MetricPartition
            Sql( @"IF NOT EXISTS (SELECT * FROM MetricPartition where [Guid] = '518918E1-410A-4E3F-81BA-432F2FD041DD')
                    BEGIN
                    DECLARE @MetricId int = (select Id from Metric where Guid = '62D95833-346B-4244-91A0-89B8CB72B561')

                    INSERT INTO [MetricPartition] (
                        MetricId,
                        IsRequired,
                        [Order],
                        [Guid])
                    VALUES (
                        @MetricId,
                        1,
                        0,
                        '518918E1-410A-4E3F-81BA-432F2FD041DD')
                    END" );

            // Large Gifts Last Week: Create Metric
            Sql( @"IF NOT EXISTS (SELECT * FROM Metric where [Guid] = '1DB4EBB4-8B92-4A0F-86AE-CC6442E7CCA4')
                    BEGIN
                    DECLARE @ScheduleId int = (select Id from Schedule where Guid = '077AB9CB-842D-4B78-8D95-83E3ED5B59B0'),
                            @DataViewMetricValueSourceDVId int = (select Id from DefinedValue where [Guid] = '2EC60BCF-EF63-4CCC-A970-F152292765D0'),
                            @DataViewId int = (select Id from DataView where [Guid] = '928099C8-6851-40BA-9D02-7832A60BB9A2')

                    INSERT INTO [Metric] (
                        IsSystem,
                        Title,
                        Subtitle,
                        Description,
                        IsCumulative,
                        SourceValueTypeId,
                        DataViewId,
                        YAxisLabel,
                        ScheduleId,
                        [Guid],
                        NumericDataType,
                        EnableAnalytics,
                        AutoPartitionOnPrimaryCampus)
                    VALUES (
                        0,
                        'Large Gifts Last Week',
                        '',
                        '',
                        0,
                        @DataViewMetricValueSourceDVId,
                        @DataViewId,
                        'Gifts',
                        @ScheduleId,
                        '1DB4EBB4-8B92-4A0F-86AE-CC6442E7CCA4',
                        0,
                        0,
                        0)
                    END" );

            // Large Gifts Last Week: Create MetricCategory
            Sql( @"IF NOT EXISTS (SELECT * FROM MetricCategory where [Guid] = '6C111FC6-2400-4452-BC6C-32E08C33260C')
                    BEGIN
                    DECLARE @MetricId int = (select Id from Metric where Guid = '1DB4EBB4-8B92-4A0F-86AE-CC6442E7CCA4'),
                            @CategoryId int = (select Id from Category where Guid = 'B6F5E7C3-4AF0-490E-8634-5405A1E84D88')

                    INSERT INTO [MetricCategory] (
                        MetricId,
                        CategoryId,
                        [Order],
                        [Guid])
                    VALUES (
                        @MetricId,
                        @CategoryId,
                        0,
                        '6C111FC6-2400-4452-BC6C-32E08C33260C')
                    END" );

            // Large Gifts Last Week: Create MetricPartition
            Sql( @"IF NOT EXISTS (SELECT * FROM MetricPartition where [Guid] = '19F5B406-47E1-4A5C-B744-722D48F788ED')
                    BEGIN
                    DECLARE @MetricId int = (select Id from Metric where Guid = '1DB4EBB4-8B92-4A0F-86AE-CC6442E7CCA4')

                    INSERT INTO [MetricPartition] (
                        MetricId,
                        IsRequired,
                        [Order],
                        [Guid])
                    VALUES (
                        @MetricId,
                        1,
                        0,
                        '19F5B406-47E1-4A5C-B744-722D48F788ED')
                    END" );

            #endregion Giving Metrics

            #region Groups Metrics

            // % of Era Active in Small Groups: Create Metric
            Sql( @"IF NOT EXISTS (SELECT * FROM Metric where [Guid] = '848416B7-5095-4342-AD4E-94FBCA5BB171')
                    BEGIN
                    DECLARE @ScheduleId int = (select Id from Schedule where Guid = '077AB9CB-842D-4B78-8D95-83E3ED5B59B0'),
                            @SQLMetricValueSourceDVId int = (select Id from DefinedValue where [Guid] = '6A1E1A1B-A636-4E12-B90C-D7FD1BDAE764')

                    INSERT INTO [Metric] (
                        IsSystem,
                        Title,
                        Subtitle,
                        Description,
                        IsCumulative,
                        SourceValueTypeId,
                        SourceSql,
                        YAxisLabel,
                        ScheduleId,
                        [Guid],
                        NumericDataType,
                        EnableAnalytics,
                        AutoPartitionOnPrimaryCampus)
                    VALUES (
                        0,
                        '% of Era Active in Small Groups',
                        '',
                        '',
                        0,
                        @SQLMetricValueSourceDVId,
                        'WITH SmallGroup AS (
    SELECT g.[Id]
    FROM [Group] g
    WHERE g.[GroupTypeId] = 25
        AND g.[IsActive] = 1
        AND g.[IsArchived] = 0
),
ActiveMember AS (
    SELECT gm.[PersonId], gm.[GroupId]
    FROM [GroupMember] gm
        JOIN [SmallGroup] sm ON gm.[GroupId] = sm.[Id]
    WHERE gm.[GroupMemberStatus] = 1
        AND gm.[IsArchived] = 0
)

SELECT CAST((CAST(COUNT(DISTINCT am.[PersonId]) AS DECIMAL(5,2))/CAST(COUNT(DISTINCT adp.[PersonId])AS DECIMAL(5,2)))*100 AS DECIMAL(5,2))
FROM AnalyticsDimPersonCurrent adp
	LEFT JOIN [ActiveMember] am ON adp.[PersonId] = am.[PersonId]
WHERE adp.core_CurrentlyAnEra = 1',
                        'Percent',
                        @ScheduleId,
                        '848416B7-5095-4342-AD4E-94FBCA5BB171',
                        0,
                        0,
                        0)
                    END" );

            // % of Era Active in Small Groups: Create MetricCategory
            Sql( @"IF NOT EXISTS (SELECT * FROM MetricCategory where [Guid] = 'A483F3C3-6724-4B4C-BD19-C41690CF2FF8')
                    BEGIN
                    DECLARE @MetricId int = (select Id from Metric where Guid = '848416B7-5095-4342-AD4E-94FBCA5BB171'),
                            @CategoryId int = (select Id from Category where Guid = '5A7A99D2-33C7-4684-9500-E200503C311F')

                    INSERT INTO [MetricCategory] (
                        MetricId,
                        CategoryId,
                        [Order],
                        [Guid])
                    VALUES (
                        @MetricId,
                        @CategoryId,
                        0,
                        'A483F3C3-6724-4B4C-BD19-C41690CF2FF8')
                    END" );

            // % of Era Active in Small Groups: Create MetricPartition
            Sql( @"IF NOT EXISTS (SELECT * FROM MetricPartition where [Guid] = '316D2D4B-BC4A-4178-8512-BC7DDCE5111D')
                    BEGIN
                    DECLARE @MetricId int = (select Id from Metric where Guid = '848416B7-5095-4342-AD4E-94FBCA5BB171')

                    INSERT INTO [MetricPartition] (
                        MetricId,
                        IsRequired,
                        [Order],
                        [Guid])
                    VALUES (
                        @MetricId,
                        1,
                        0,
                        '316D2D4B-BC4A-4178-8512-BC7DDCE5111D')
                    END" );

            // Active Small Group Members: Create Metric
            Sql( @"IF NOT EXISTS (SELECT * FROM Metric where [Guid] = 'D4E7CF72-B001-4AF4-B52B-B3711FEF7D1B')
                    BEGIN
                    DECLARE @ScheduleId int = (select Id from Schedule where Guid = '077AB9CB-842D-4B78-8D95-83E3ED5B59B0'),
                            @DataViewMetricValueSourceDVId int = (select Id from DefinedValue where [Guid] = '2EC60BCF-EF63-4CCC-A970-F152292765D0'),
                            @DataViewId int = (select Id from DataView where [Guid] = 'D341C232-92A0-498D-9994-D3B2BA3419C9')

                    INSERT INTO [Metric] (
                        IsSystem,
                        Title,
                        Subtitle,
                        Description,
                        IsCumulative,
                        SourceValueTypeId,
                        DataViewId,
                        YAxisLabel,
                        ScheduleId,
                        [Guid],
                        NumericDataType,
                        EnableAnalytics,
                        AutoPartitionOnPrimaryCampus)
                    VALUES (
                        0,
                        'Active Small Group Members',
                        '',
                        '',
                        0,
                        @DataViewMetricValueSourceDVId,
                        @DataViewId,
                        'People',
                        @ScheduleId,
                        'D4E7CF72-B001-4AF4-B52B-B3711FEF7D1B',
                        0,
                        0,
                        0)
                    END" );

            // Active Small Group Members: Create MetricCategory
            Sql( @"IF NOT EXISTS (SELECT * FROM MetricCategory where [Guid] = 'BD237CEC-9189-4EC7-A10D-D869CBE238F1')
                    BEGIN
                    DECLARE @MetricId int = (select Id from Metric where Guid = 'D4E7CF72-B001-4AF4-B52B-B3711FEF7D1B'),
                            @CategoryId int = (select Id from Category where Guid = '5A7A99D2-33C7-4684-9500-E200503C311F')

                    INSERT INTO [MetricCategory] (
                        MetricId,
                        CategoryId,
                        [Order],
                        [Guid])
                    VALUES (
                        @MetricId,
                        @CategoryId,
                        0,
                        'BD237CEC-9189-4EC7-A10D-D869CBE238F1')
                    END" );

            // Active Small Group Members: Create MetricPartition
            Sql( @"IF NOT EXISTS (SELECT * FROM MetricPartition where [Guid] = '95ACDF23-AE7D-4CF9-85CE-6A835BEC6E95')
                    BEGIN
                    DECLARE @MetricId int = (select Id from Metric where Guid = 'D4E7CF72-B001-4AF4-B52B-B3711FEF7D1B')

                    INSERT INTO [MetricPartition] (
                        MetricId,
                        IsRequired,
                        [Order],
                        [Guid])
                    VALUES (
                        @MetricId,
                        1,
                        0,
                        '95ACDF23-AE7D-4CF9-85CE-6A835BEC6E95')
                    END" );

            // Active Small Groups: Create Metric
            Sql( @"IF NOT EXISTS (SELECT * FROM Metric where [Guid] = 'FA9DB967-39E3-4A77-85DB-3041A272D78F')
                    BEGIN
                    DECLARE @ScheduleId int = (select Id from Schedule where Guid = '077AB9CB-842D-4B78-8D95-83E3ED5B59B0'),
                            @DataViewMetricValueSourceDVId int = (select Id from DefinedValue where [Guid] = '2EC60BCF-EF63-4CCC-A970-F152292765D0'),
                            @DataViewId int = (select Id from DataView where [Guid] = '29F360C7-BB4D-4E9B-ADD6-2E73FE64F52D')

                    INSERT INTO [Metric] (
                        IsSystem,
                        Title,
                        Subtitle,
                        Description,
                        IsCumulative,
                        SourceValueTypeId,
                        DataViewId,
                        YAxisLabel,
                        ScheduleId,
                        [Guid],
                        NumericDataType,
                        EnableAnalytics,
                        AutoPartitionOnPrimaryCampus)
                    VALUES (
                        0,
                        'Active Small Groups',
                        '',
                        '',
                        0,
                        @DataViewMetricValueSourceDVId,
                        @DataViewId,
                        'Groups',
                        @ScheduleId,
                        'FA9DB967-39E3-4A77-85DB-3041A272D78F',
                        0,
                        0,
                        0)
                    END" );

            // Active Small Groups: Create MetricCategory
            Sql( @"IF NOT EXISTS (SELECT * FROM MetricCategory where [Guid] = '87E415F9-D9AC-4261-BFAA-AAAFE4FA585C')
                    BEGIN
                    DECLARE @MetricId int = (select Id from Metric where Guid = 'FA9DB967-39E3-4A77-85DB-3041A272D78F'),
                            @CategoryId int = (select Id from Category where Guid = '5A7A99D2-33C7-4684-9500-E200503C311F')

                    INSERT INTO [MetricCategory] (
                        MetricId,
                        CategoryId,
                        [Order],
                        [Guid])
                    VALUES (
                        @MetricId,
                        @CategoryId,
                        0,
                        '87E415F9-D9AC-4261-BFAA-AAAFE4FA585C')
                    END" );

            // Active Small Groups: Create MetricPartition
            Sql( @"IF NOT EXISTS (SELECT * FROM MetricPartition where [Guid] = '1551AA2E-7838-448F-A4C8-7259154ECFDC')
                    BEGIN
                    DECLARE @MetricId int = (select Id from Metric where Guid = 'FA9DB967-39E3-4A77-85DB-3041A272D78F')

                    INSERT INTO [MetricPartition] (
                        MetricId,
                        IsRequired,
                        [Order],
                        [Guid])
                    VALUES (
                        @MetricId,
                        1,
                        0,
                        '1551AA2E-7838-448F-A4C8-7259154ECFDC')
                    END" );

            // Attendance Last Week: Create Metric
            Sql( @"IF NOT EXISTS (SELECT * FROM Metric where [Guid] = '8B0CE73B-5557-4F93-939B-BBA640BE760E')
                    BEGIN
                    DECLARE @ScheduleId int = (select Id from Schedule where Guid = '077AB9CB-842D-4B78-8D95-83E3ED5B59B0'),
                            @DataViewMetricValueSourceDVId int = (select Id from DefinedValue where [Guid] = '2EC60BCF-EF63-4CCC-A970-F152292765D0'),
                            @DataViewId int = (select Id from DataView where [Guid] = '41882678-4668-4B52-89FA-ABB4718321F5')

                    INSERT INTO [Metric] (
                        IsSystem,
                        Title,
                        Subtitle,
                        Description,
                        IsCumulative,
                        SourceValueTypeId,
                        DataViewId,
                        YAxisLabel,
                        ScheduleId,
                        [Guid],
                        NumericDataType,
                        EnableAnalytics,
                        AutoPartitionOnPrimaryCampus)
                    VALUES (
                        0,
                        'Attendance Last Week',
                        '',
                        '',
                        0,
                        @DataViewMetricValueSourceDVId,
                        @DataViewId,
                        'People',
                        @ScheduleId,
                        '8B0CE73B-5557-4F93-939B-BBA640BE760E',
                        0,
                        0,
                        0)
                    END" );

            // Attendance Last Week: Create MetricCategory
            Sql( @"IF NOT EXISTS (SELECT * FROM MetricCategory where [Guid] = '82F0CA83-778C-41D0-B9EA-FC84F837825E')
                    BEGIN
                    DECLARE @MetricId int = (select Id from Metric where Guid = '8B0CE73B-5557-4F93-939B-BBA640BE760E'),
                            @CategoryId int = (select Id from Category where Guid = '5A7A99D2-33C7-4684-9500-E200503C311F')

                    INSERT INTO [MetricCategory] (
                        MetricId,
                        CategoryId,
                        [Order],
                        [Guid])
                    VALUES (
                        @MetricId,
                        @CategoryId,
                        0,
                        '82F0CA83-778C-41D0-B9EA-FC84F837825E')
                    END" );

            // Attendance Last Week: Create MetricPartition
            Sql( @"IF NOT EXISTS (SELECT * FROM MetricPartition where [Guid] = 'B898CC91-3040-483F-B30E-D5F5CFEE6F76')
                    BEGIN
                    DECLARE @MetricId int = (select Id from Metric where Guid = '8B0CE73B-5557-4F93-939B-BBA640BE760E')

                    INSERT INTO [MetricPartition] (
                        MetricId,
                        IsRequired,
                        [Order],
                        [Guid])
                    VALUES (
                        @MetricId,
                        1,
                        0,
                        'B898CC91-3040-483F-B30E-D5F5CFEE6F76')
                    END" );

            #endregion Groups Metrics
        }

        /// <summary>
        /// Operations to be performed during the downgrade process.
        /// </summary>
        public override void Down()
        {
            #region Groups Metrics

            // Attendance Last Week: Delete MetricPartition
            Sql( @"DELETE FROM MetricPartition where [Guid] = 'B898CC91-3040-483F-B30E-D5F5CFEE6F76'" );

            // Attendance Last Week: Delete MetricCategory
            Sql( @"DELETE FROM MetricCategory where [Guid] = '82F0CA83-778C-41D0-B9EA-FC84F837825E'" );

            // Attendance Last Week: Delete Metric
            Sql( @"DELETE FROM Metric where [Guid] = '8B0CE73B-5557-4F93-939B-BBA640BE760E'" );

            // Active Small Groups: Delete MetricPartition
            Sql( @"DELETE FROM MetricPartition where [Guid] = '1551AA2E-7838-448F-A4C8-7259154ECFDC'" );

            // Active Small Groups: Delete MetricCategory
            Sql( @"DELETE FROM MetricCategory where [Guid] = '87E415F9-D9AC-4261-BFAA-AAAFE4FA585C'" );

            // Active Small Groups: Delete Metric
            Sql( @"DELETE FROM Metric where [Guid] = 'FA9DB967-39E3-4A77-85DB-3041A272D78F'" );

            // Active Small Group Members: Delete MetricPartition
            Sql( @"DELETE FROM MetricPartition where [Guid] = '95ACDF23-AE7D-4CF9-85CE-6A835BEC6E95'" );

            // Active Small Group Members: Delete MetricCategory
            Sql( @"DELETE FROM MetricCategory where [Guid] = 'BD237CEC-9189-4EC7-A10D-D869CBE238F1'" );

            // Active Small Group Members: Delete Metric
            Sql( @"DELETE FROM Metric where [Guid] = 'D4E7CF72-B001-4AF4-B52B-B3711FEF7D1B'" );

            // % of Era Active in Small Groups: Delete MetricPartition
            Sql( @"DELETE FROM MetricPartition where [Guid] = '316D2D4B-BC4A-4178-8512-BC7DDCE5111D'" );

            // % of Era Active in Small Groups: Delete MetricCategory
            Sql( @"DELETE FROM MetricCategory where [Guid] = '6C111FC6-2400-4452-BC6C-32E08C33260C'" );

            // % of Era Active in Small Groups: Delete Metric
            Sql( @"DELETE FROM Metric where [Guid] = '848416B7-5095-4342-AD4E-94FBCA5BB171'" );

            #endregion Groups Metrics

            #region Giving Metrics

            // Large Gifts Last Week: Delete MetricPartition
            Sql( @"DELETE FROM MetricPartition where [Guid] = '19F5B406-47E1-4A5C-B744-722D48F788ED'" );

            // Large Gifts Last Week: Delete MetricCategory
            Sql( @"DELETE FROM MetricCategory where [Guid] = 'A483F3C3-6724-4B4C-BD19-C41690CF2FF8'" );

            // Large Gifts Last Week: Delete Metric
            Sql( @"DELETE FROM Metric where [Guid] = '1DB4EBB4-8B92-4A0F-86AE-CC6442E7CCA4'" );

            // Giving Units Giving Last Week: Delete MetricPartition
            Sql( @"DELETE FROM MetricPartition where [Guid] = '518918E1-410A-4E3F-81BA-432F2FD041DD'" );

            // Giving Units Giving Last Week: Delete MetricCategory
            Sql( @"DELETE FROM MetricCategory where [Guid] = 'BE45522A-101F-4B57-A7E6-BB19AE80C0D3'" );

            // Giving Units Giving Last Week: Delete Metric
            Sql( @"DELETE FROM Metric where [Guid] = '62D95833-346B-4244-91A0-89B8CB72B561'" );

            // First Time Givers Previous Week: Delete MetricPartition
            Sql( @"DELETE FROM MetricPartition where [Guid] = '0BC8577F-4300-459C-96A0-DC96DF797138'" );

            // First Time Givers Previous Week: Delete MetricCategory
            Sql( @"DELETE FROM MetricCategory where [Guid] = 'EBFB1A17-1A23-4563-8E8C-34C2413FC0CD'" );

            // First Time Givers Previous Week: Delete Metric
            Sql( @"DELETE FROM Metric where [Guid] = '7C85C112-2D1E-4F34-9BA3-FE9357CD570A'" );

            // Contributions Last Week: Delete MetricPartition
            Sql( @"DELETE FROM MetricPartition where [Guid] = '3575D7C5-CFC2-4017-98CB-E172DB1602B7'" );

            // Contributions Last Week: Delete MetricCategory
            Sql( @"DELETE FROM MetricCategory where [Guid] = '070D8296-3D8F-47DD-A79A-D482CAB47349'" );

            // Contributions Last Week: Delete Metric
            Sql( @"DELETE FROM Metric where [Guid] = '56EF54EA-715B-4CF2-B401-60E3D3AF1389'" );

            // Contributions Last Week: Delete Schedule
            Sql( @"DELETE FROM Schedule where [Guid] = '077AB9CB-842D-4B78-8D95-83E3ED5B59B0'" );

            #endregion Giving Metrics

            #region Categories

            RockMigrationHelper.DeleteCategory( "B6F5E7C3-4AF0-490E-8634-5405A1E84D88" );
            RockMigrationHelper.DeleteCategory( "5A7A99D2-33C7-4684-9500-E200503C311F" );
            RockMigrationHelper.DeleteCategory( "3E650C53-9D8F-4946-8F6E-432B36CDC9E1" );

            #endregion Categories
        }
    }
}