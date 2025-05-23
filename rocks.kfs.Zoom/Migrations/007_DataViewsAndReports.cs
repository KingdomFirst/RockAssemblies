﻿// <copyright>
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

namespace rocks.kfs.Zoom.Migrations
{
    [MigrationNumber( 7, "1.13" )]
    public partial class DataViewsAndReports : Migration
    {
        /// <summary>
        /// Operations to be performed during the upgrade process.
        /// </summary>
        public override void Up()
        {
            #region DataView

            // Create Zoom Room Occurrence DataView Category
            RockMigrationHelper.UpdateCategory( "57F8FA29-DCF1-4F74-8553-87E90F234139", "Zoom Room Occurrences", "fa fa-video", "Dataviews related to Zoom Room Occurrences", "409D543C-9D39-49D5-BF1C-0F9BE442555F" );

            // Create [GroupAll] DataViewFilter for DataView: Upcoming Zoom Room Occurrences
            Sql( @"IF NOT EXISTS (SELECT * FROM DataViewFilter where [Guid] = '823D3C22-EC26-42DF-BB3A-4E675734D010')
                        BEGIN
                        INSERT INTO [DataViewFilter] (
                            ExpressionType,
                            [Guid])
                        values (
                            1,
                            '823D3C22-EC26-42DF-BB3A-4E675734D010')
                        END" );

            // Create Rock.Reporting.DataFilter.PropertyFilter DataViewFilter for DataView: Upcoming Zoom Room Occurrences
            Sql( @"IF NOT EXISTS (SELECT * FROM DataViewFilter where [Guid] = 'F1E656FA-FC12-410E-9EA7-8F48CFB9D85B')
                    BEGIN
                    DECLARE
                        @ParentDataViewFilterId int = (select Id from DataViewFilter where [Guid] = '823D3C22-EC26-42DF-BB3A-4E675734D010'),
                        @DataViewFilterEntityTypeId int = (select Id from EntityType where [Guid] = '03F0D6AC-D181-48B6-B4BC-1F2652B55323')
                    INSERT INTO [DataViewFilter] (
                        ExpressionType,
                        ParentId,
                        EntityTypeId,
                        Selection,
                        [Guid])
                    values (
                        0,
                        @ParentDataViewFilterId,
                        @DataViewFilterEntityTypeId,
                        '[""Property_IsCompleted"",""1"",""False""]',
                        'F1E656FA-FC12-410E-9EA7-8F48CFB9D85B')
                    END" );

            // Create DataView: Upcoming Zoom Room Occurrences
            Sql( @"IF NOT EXISTS (SELECT * FROM DataView where [Guid] = '45493469-8718-4C09-BB76-ED53AE3F2FA5')
                    BEGIN
                    DECLARE
                        @categoryId int = (select top 1 [Id] from [Category] where [Guid] = '409D543C-9D39-49D5-BF1C-0F9BE442555F'),
                        @entityTypeId int = (select top 1 [Id] from [EntityType] where [Guid] = '2A138B5B-3CD8-4F03-ACAD-4D544D257916'),
                        @dataViewFilterId int = (select top 1 [Id] from [DataViewFilter] where [Guid] = '823D3C22-EC26-42DF-BB3A-4E675734D010')

                    INSERT INTO [DataView] (
                        [IsSystem],
                        [Name],
                        [Description],
                        [CategoryId],
                        [EntityTypeId],
                        [DataViewFilterId],
                        [Guid])
                    VALUES(
                        0,
                        'Upcoming Zoom Room Occurrences',
                        '',
                        @categoryId,
                        @entityTypeId,
                        @dataViewFilterId,
                        '45493469-8718-4C09-BB76-ED53AE3F2FA5')
                    END" );

            #endregion DataView

            #region Report

            // Add GUID to Reporting DataSelect entity type for reporting: Location Name
            RockMigrationHelper.UpdateEntityType( "rocks.kfs.Zoom.Reporting.RoomOccurrence.DataSelect.LocationNameSelect", "Location Name Select", "rocks.kfs.Zoom.Reporting.RoomOccurrence.DataSelect.LocationNameSelect, rocks.kfs.Zoom, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", true, true, ZoomGuid.EntityType.REPORT_LOCATION_NAME );

            // Add GUID to Reporting DataSelect entity type for reporting: Zoom Room Name
            RockMigrationHelper.UpdateEntityType( "rocks.kfs.Zoom.Reporting.RoomOccurrence.DataSelect.ZoomRoomNameSelect", "Zoom Room Name Select", "rocks.kfs.Zoom.Reporting.RoomOccurrence.DataSelect.ZoomRoomNameSelect, rocks.kfs.Zoom, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", true, true, ZoomGuid.EntityType.REPORT_ZOOM_ROOM_NAME );

            // Create Zoom Room Occurrence Report Category
            RockMigrationHelper.UpdateCategory( "F1F22D3E-FEFA-4C84-9FFA-9E8ACE60FCE7", "Zoom Room Occurrences", "fa fa-video", "Reports related to Zoom Room Occurrences", "5C73C293-031A-4D91-8947-23A506120D22" );

            // Create Report: Upcoming Zoom Room Occurrences
            RockMigrationHelper.AddReport( "5C73C293-031A-4D91-8947-23A506120D22", "45493469-8718-4C09-BB76-ED53AE3F2FA5", "2A138B5B-3CD8-4F03-ACAD-4D544D257916", "Upcoming Zoom Room Occurrences", "Upcoming Zoom Room Occurrences", "E89ED73D-3D1D-40EB-BC6A-5631C6BC9834" );

            // Create ReportFields for Upcoming Zoom Room Occurrences
            // Topic
            Sql( @"IF NOT EXISTS (SELECT * FROM ReportField where [Guid] = '4A0F0FEB-780E-4D02-87E4-A908DCC41C2D')
                    BEGIN
                    DECLARE
                        @reportId int = (select Id from [Report] where [Guid] = 'E89ED73D-3D1D-40EB-BC6A-5631C6BC9834')
                    INSERT INTO [ReportField] (
                        ReportId,
                        ReportFieldType,
                        ShowInGrid,
                        DataSelectComponentEntityTypeId,
                        Selection,
                        [Guid],
                        ColumnHeaderText,
                        ColumnOrder,
                        SortOrder,
                        SortDirection )
                    values (
                        @reportId,
                        0,
                        1,
                        null,
                        'Topic',
                        '4A0F0FEB-780E-4D02-87E4-A908DCC41C2D',
                        'Topic',
                        0,
                        null,
                        0 )
                    END" );

            // StartTime
            Sql( @"IF NOT EXISTS (SELECT * FROM ReportField where [Guid] = 'EFB0EA72-DBD5-4440-B53C-7637A2C0F8E1')
                    BEGIN
                    DECLARE
                        @reportId int = (select Id from [Report] where [Guid] = 'E89ED73D-3D1D-40EB-BC6A-5631C6BC9834')
                    INSERT INTO [ReportField] (
                        ReportId,
                        ReportFieldType,
                        ShowInGrid,
                        DataSelectComponentEntityTypeId,
                        Selection,
                        [Guid],
                        ColumnHeaderText,
                        ColumnOrder,
                        SortOrder,
                        SortDirection )
                    values (
                        @reportId,
                        0,
                        1,
                        null,
                        'StartTime',
                        'EFB0EA72-DBD5-4440-B53C-7637A2C0F8E1',
                        'StartTime',
                        1,
                        0,
                        0 )
                    END" );

            // Duration
            Sql( @"IF NOT EXISTS (SELECT * FROM ReportField where [Guid] = 'F0E16384-17D0-4168-BE44-23859C088EA0')
                    BEGIN
                    DECLARE
                        @reportId int = (select Id from [Report] where [Guid] = 'E89ED73D-3D1D-40EB-BC6A-5631C6BC9834')
                    INSERT INTO [ReportField] (
                        ReportId,
                        ReportFieldType,
                        ShowInGrid,
                        DataSelectComponentEntityTypeId,
                        Selection,
                        [Guid],
                        ColumnHeaderText,
                        ColumnOrder,
                        SortOrder,
                        SortDirection )
                    values (
                        @reportId,
                        0,
                        1,
                        null,
                        'Duration',
                        'F0E16384-17D0-4168-BE44-23859C088EA0',
                        'Duration',
                        2,
                        null,
                        0 )
                    END" );

            // Password
            Sql( @"IF NOT EXISTS (SELECT * FROM ReportField where [Guid] = 'F04559AA-E192-4799-9434-B660BC097992')
                    BEGIN
                    DECLARE
                        @reportId int = (select Id from [Report] where [Guid] = 'E89ED73D-3D1D-40EB-BC6A-5631C6BC9834')
                    INSERT INTO [ReportField] (
                        ReportId,
                        ReportFieldType,
                        ShowInGrid,
                        DataSelectComponentEntityTypeId,
                        Selection,
                        [Guid],
                        ColumnHeaderText,
                        ColumnOrder,
                        SortOrder,
                        SortDirection )
                    values (
                        @reportId,
                        0,
                        1,
                        null,
                        'Password',
                        'F04559AA-E192-4799-9434-B660BC097992',
                        'Password',
                        3,
                        null,
                        0 )
                    END" );

            // Location Name
            RockMigrationHelper.AddReportField( "E89ED73D-3D1D-40EB-BC6A-5631C6BC9834", Rock.Model.ReportFieldType.DataSelectComponent, true, ZoomGuid.EntityType.REPORT_LOCATION_NAME, "{}", 4, "Location Name", "E855930E-5B2E-4311-B494-EA7FD56E6B57" );

            // Zoom Room Name
            RockMigrationHelper.AddReportField( "E89ED73D-3D1D-40EB-BC6A-5631C6BC9834", Rock.Model.ReportFieldType.DataSelectComponent, true, ZoomGuid.EntityType.REPORT_ZOOM_ROOM_NAME, "{}", 5, "Zoom Room Name", "F79941D9-8BC2-4812-A6A4-68143E5DBCBC" );

            #endregion Report
        }

        /// <summary>
        /// Operations to be performed during the downgrade process.
        /// </summary>
        public override void Down()
        {
            #region Report
            RockMigrationHelper.DeleteReportField( "F79941D9-8BC2-4812-A6A4-68143E5DBCBC" );
            RockMigrationHelper.DeleteReportField( "E855930E-5B2E-4311-B494-EA7FD56E6B57" );
            RockMigrationHelper.DeleteReportField( "F04559AA-E192-4799-9434-B660BC097992" );
            RockMigrationHelper.DeleteReportField( "F0E16384-17D0-4168-BE44-23859C088EA0" );
            RockMigrationHelper.DeleteReportField( "EFB0EA72-DBD5-4440-B53C-7637A2C0F8E1" );
            RockMigrationHelper.DeleteReportField( "4A0F0FEB-780E-4D02-87E4-A908DCC41C2D" );

            RockMigrationHelper.DeleteReport( "E89ED73D-3D1D-40EB-BC6A-5631C6BC9834" );
            RockMigrationHelper.DeleteCategory( "5C73C293-031A-4D91-8947-23A506120D22" );

            #endregion Report

            #region DataView

            // Delete DataView: Upcoming Zoom Room Occurrences
            Sql( @"DELETE FROM DataView where [Guid] = '45493469-8718-4C09-BB76-ED53AE3F2FA5'" );

            // Delete DataViewFilter for DataView: Upcoming Zoom Room Occurrences
            Sql( @"DELETE FROM DataViewFilter where [Guid] = 'F1E656FA-FC12-410E-9EA7-8F48CFB9D85B'" );

            // Delete DataViewFilter for DataView: Upcoming Zoom Room Occurrences
            Sql( @"DELETE FROM DataViewFilter where [Guid] = '823D3C22-EC26-42DF-BB3A-4E675734D010'" );

            RockMigrationHelper.DeleteCategory( "409D543C-9D39-49D5-BF1C-0F9BE442555F" );

            #endregion DataView
        }
    }
}