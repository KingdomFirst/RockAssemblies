SET NOCOUNT ON

-- =============================================
-- Description: One-time setup for the Attended Check-in end-to-end tests
--              (tests/CheckIn/AttendedCheckin).
--
--              The test fixture creates every check-in entity through the REST
--              API, by fixed Guid. REST cannot write the three many-to-many join
--              tables check-in needs, so this script links them:
--
--                GroupTypeAssociation   check-in type -> check-in area
--                GroupLocationSchedule  group location -> always-open schedule
--                DeviceLocation         test kiosk -> test room
--
--              Run order on a new site:
--                1. Run the tests once. The fixture creates the entities, then
--                   stops and asks for this script.
--                2. Run this script against the site's database.
--                3. Run the tests again.
--
--              Idempotent: safe to re-run. Touches only rows joined by the
--              fixture's fixed Guids. Works on Rock 17 through 20 (all three
--              tables are plain two-column join tables in each version).
-- Date: 2026-10-01
-- =============================================

-- Fixed Guids (keep in sync with shared/testData/checkin.ts)
DECLARE @CheckinTypeGuid   UNIQUEIDENTIFIER = 'C4EC0E2E-E2E0-4C0E-8C0E-0000000000A1'
DECLARE @AreaGuid          UNIQUEIDENTIFIER = 'C4EC0E2E-E2E0-4C0E-8C0E-0000000000A2'
DECLARE @RoomGuid          UNIQUEIDENTIFIER = 'C4EC0E2E-E2E0-4C0E-8C0E-0000000000A4'
DECLARE @ScheduleGuid      UNIQUEIDENTIFIER = 'C4EC0E2E-E2E0-4C0E-8C0E-0000000000A5'
DECLARE @GroupLocationGuid UNIQUEIDENTIFIER = 'C4EC0E2E-E2E0-4C0E-8C0E-0000000000A6'
DECLARE @KioskGuid         UNIQUEIDENTIFIER = 'C4EC0E2E-E2E0-4C0E-8C0E-0000000000A7'

-- Lookups
DECLARE @CheckinTypeId   INT = (SELECT TOP 1 [Id] FROM [GroupType]     WHERE [Guid] = @CheckinTypeGuid)
DECLARE @AreaId          INT = (SELECT TOP 1 [Id] FROM [GroupType]     WHERE [Guid] = @AreaGuid)
DECLARE @RoomId          INT = (SELECT TOP 1 [Id] FROM [Location]      WHERE [Guid] = @RoomGuid)
DECLARE @ScheduleId      INT = (SELECT TOP 1 [Id] FROM [Schedule]      WHERE [Guid] = @ScheduleGuid)
DECLARE @GroupLocationId INT = (SELECT TOP 1 [Id] FROM [GroupLocation] WHERE [Guid] = @GroupLocationGuid)
DECLARE @KioskId         INT = (SELECT TOP 1 [Id] FROM [Device]        WHERE [Guid] = @KioskGuid)

IF @CheckinTypeId IS NULL OR @AreaId IS NULL OR @RoomId IS NULL OR @ScheduleId IS NULL OR @GroupLocationId IS NULL OR @KioskId IS NULL
BEGIN
    RAISERROR( 'The KFS E2E check-in entities do not exist yet. Run the Attended Check-in tests once so the fixture creates them, then run this script again.', 16, 1 )
    RETURN
END

BEGIN TRANSACTION

IF NOT EXISTS (SELECT 1 FROM [GroupTypeAssociation] WHERE [GroupTypeId] = @CheckinTypeId AND [ChildGroupTypeId] = @AreaId)
BEGIN
    INSERT INTO [GroupTypeAssociation] ([GroupTypeId], [ChildGroupTypeId])
    VALUES (@CheckinTypeId, @AreaId)
END

IF NOT EXISTS (SELECT 1 FROM [GroupLocationSchedule] WHERE [GroupLocationId] = @GroupLocationId AND [ScheduleId] = @ScheduleId)
BEGIN
    INSERT INTO [GroupLocationSchedule] ([GroupLocationId], [ScheduleId])
    VALUES (@GroupLocationId, @ScheduleId)
END

IF NOT EXISTS (SELECT 1 FROM [DeviceLocation] WHERE [DeviceId] = @KioskId AND [LocationId] = @RoomId)
BEGIN
    INSERT INTO [DeviceLocation] ([DeviceId], [LocationId])
    VALUES (@KioskId, @RoomId)
END

IF @@ERROR <> 0
BEGIN
    ROLLBACK TRANSACTION
    RETURN
END

COMMIT TRANSACTION

-- Rock caches check-in configuration. The fixture re-saves the check-in type,
-- area and kiosk through REST at the start of every run, which flushes those
-- caches, so no restart is needed after this script.

-- Verify: expect three rows, each with Linked = 1
SELECT 'Check-in type -> area' AS [Link], CASE WHEN EXISTS (SELECT 1 FROM [GroupTypeAssociation] WHERE [GroupTypeId] = @CheckinTypeId AND [ChildGroupTypeId] = @AreaId) THEN 1 ELSE 0 END AS [Linked]
UNION ALL
SELECT 'Group location -> schedule', CASE WHEN EXISTS (SELECT 1 FROM [GroupLocationSchedule] WHERE [GroupLocationId] = @GroupLocationId AND [ScheduleId] = @ScheduleId) THEN 1 ELSE 0 END
UNION ALL
SELECT 'Kiosk -> room', CASE WHEN EXISTS (SELECT 1 FROM [DeviceLocation] WHERE [DeviceId] = @KioskId AND [LocationId] = @RoomId) THEN 1 ELSE 0 END

/*
-- =============================================
-- Teardown: removes the links only. Run this before deleting the KFS E2E
-- check-in entities (Rock will not delete a group type, location, schedule
-- or device that is still linked).
-- =============================================

DECLARE @CheckinTypeId   INT = (SELECT TOP 1 [Id] FROM [GroupType]     WHERE [Guid] = 'C4EC0E2E-E2E0-4C0E-8C0E-0000000000A1')
DECLARE @AreaId          INT = (SELECT TOP 1 [Id] FROM [GroupType]     WHERE [Guid] = 'C4EC0E2E-E2E0-4C0E-8C0E-0000000000A2')
DECLARE @RoomId          INT = (SELECT TOP 1 [Id] FROM [Location]      WHERE [Guid] = 'C4EC0E2E-E2E0-4C0E-8C0E-0000000000A4')
DECLARE @ScheduleId      INT = (SELECT TOP 1 [Id] FROM [Schedule]      WHERE [Guid] = 'C4EC0E2E-E2E0-4C0E-8C0E-0000000000A5')
DECLARE @GroupLocationId INT = (SELECT TOP 1 [Id] FROM [GroupLocation] WHERE [Guid] = 'C4EC0E2E-E2E0-4C0E-8C0E-0000000000A6')
DECLARE @KioskId         INT = (SELECT TOP 1 [Id] FROM [Device]        WHERE [Guid] = 'C4EC0E2E-E2E0-4C0E-8C0E-0000000000A7')

DELETE FROM [GroupTypeAssociation]  WHERE [GroupTypeId] = @CheckinTypeId AND [ChildGroupTypeId] = @AreaId
DELETE FROM [GroupLocationSchedule] WHERE [GroupLocationId] = @GroupLocationId AND [ScheduleId] = @ScheduleId
DELETE FROM [DeviceLocation]        WHERE [DeviceId] = @KioskId AND [LocationId] = @RoomId
*/
