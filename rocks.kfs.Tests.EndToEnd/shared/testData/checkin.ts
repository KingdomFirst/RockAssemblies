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
import { randomUUID } from "crypto";
import { RockApi, RockEntity } from "../rockApi";
import { SystemGuid, TestNamePrefix } from "../systemGuids";
import { TestPerson, loadTestPerson } from "./people";

/*
    10/1/2026 - CLAUDE

    The check-in configuration is persistent: every entity is find-or-create by a
    fixed Guid and is never deleted by the tests. Check-in needs three
    many-to-many links (check-in type -> area, group location -> schedule,
    kiosk -> room) that the REST API cannot write, so those come from the
    one-time script setup/AttendedCheckin_Links.sql, which joins on these same
    Guids. Deleting and re-creating the entities each run would orphan those
    links and force the script to be re-run every time.

    Reason: REST cannot create check-in's join-table rows.
*/

/** Fixed Guids for the check-in configuration. Keep in sync with setup/AttendedCheckin_Links.sql. */
export const CheckinGuid = {
    checkinType: "C4EC0E2E-E2E0-4C0E-8C0E-0000000000A1",
    area: "C4EC0E2E-E2E0-4C0E-8C0E-0000000000A2",
    group: "C4EC0E2E-E2E0-4C0E-8C0E-0000000000A3",
    room: "C4EC0E2E-E2E0-4C0E-8C0E-0000000000A4",
    schedule: "C4EC0E2E-E2E0-4C0E-8C0E-0000000000A5",
    groupLocation: "C4EC0E2E-E2E0-4C0E-8C0E-0000000000A6",
    kiosk: "C4EC0E2E-E2E0-4C0E-8C0E-0000000000A7",
    /** A second room for Move tests. Not linked to the group, so check-in never offers it. */
    moveRoom: "C4EC0E2E-E2E0-4C0E-8C0E-0000000000A8",
    familyAdult: "C4EC0E2E-E2E0-4C0E-8C0E-0000000000B1",
    familyChild: "C4EC0E2E-E2E0-4C0E-8C0E-0000000000B2"
} as const;

export const CheckinName = {
    checkinType: `${TestNamePrefix} Attended Check-in`,
    area: `${TestNamePrefix} Check-in Area`,
    group: `${TestNamePrefix} Check-in Group`,
    room: `${TestNamePrefix} Check-in Room`,
    moveRoom: `${TestNamePrefix} Check-in Move Room`,
    schedule: `${TestNamePrefix} Always Open`,
    kiosk: `${TestNamePrefix} Kiosk`
} as const;

/** The persistent test family that the search tests look up by name and phone. */
export const CheckinFamily = {
    lastName: "Kfscheckin",
    adultFirstName: "Kirby",
    childFirstName: "Kody",
    mobilePhone: "6155550142"
} as const;

export type CheckinSetup = {
    checkinTypeId: number;
    areaId: number;
    groupId: number;
    roomId: number;
    scheduleId: number;
    groupLocationId: number;
    kioskId: number;
};

/** The one-time SQL script has not been run (or not since the entities were re-created). */
export class CheckinLinksMissingError extends Error {
}

/** Finds or creates every check-in entity the tests need. Does not create the join-table links. */
export async function ensureCheckinSetup( api: RockApi ): Promise<CheckinSetup> {
    const checkinTypeId = await findOrCreate( api, "GroupTypes", CheckinGuid.checkinType, async () => ( {
        Name: CheckinName.checkinType,
        Description: "Check-in type for the KFS end-to-end tests. Do not use.",
        GroupTerm: "Group",
        GroupMemberTerm: "Member",
        GroupTypePurposeValueId: await api.getIdByGuid( "DefinedValues", SystemGuid.DefinedValue.CheckinTemplatePurpose ),
        TakesAttendance: false,
        ShowInGroupList: false,
        ShowInNavigation: false,
        IsSystem: false,
        Order: 0
    } ) );

    // The Admin block only lists group types that take attendance.
    const areaId = await findOrCreate( api, "GroupTypes", CheckinGuid.area, async () => ( {
        Name: CheckinName.area,
        Description: "Check-in area for the KFS end-to-end tests. Do not use.",
        GroupTerm: "Group",
        GroupMemberTerm: "Member",
        TakesAttendance: true,
        AllowMultipleLocations: true,
        ShowInGroupList: false,
        ShowInNavigation: false,
        IsSystem: false,
        Order: 0
    } ) );

    // No age, grade or ability filters, so every test person is eligible.
    const groupId = await findOrCreate( api, "Groups", CheckinGuid.group, async () => ( {
        Name: CheckinName.group,
        GroupTypeId: areaId,
        IsActive: true,
        IsPublic: true,
        IsSystem: false,
        IsSecurityRole: false,
        Order: 0
    } ) );

    const roomId = await findOrCreate( api, "Locations", CheckinGuid.room, async () => ( {
        Name: CheckinName.room,
        LocationTypeValueId: await api.getIdByGuid( "DefinedValues", SystemGuid.DefinedValue.LocationTypeRoom ),
        IsActive: true
    } ) );

    // Daily 00:01 start; check-in opens 1 minute before and closes 1438 minutes after,
    // so it is open all day except 23:59-00:00.
    const scheduleId = await findOrCreate( api, "Schedules", CheckinGuid.schedule, async () => ( {
        Name: CheckinName.schedule,
        Description: "Always-open check-in schedule for the KFS end-to-end tests.",
        iCalendarContent: [
            "BEGIN:VCALENDAR",
            "VERSION:2.0",
            "PRODID:-//KFS//E2E//EN",
            "BEGIN:VEVENT",
            "DTSTART:20260101T000100",
            "DTEND:20260101T235900",
            "DTSTAMP:20260101T000000",
            "RRULE:FREQ=DAILY",
            "SEQUENCE:0",
            `UID:${CheckinGuid.schedule.toLowerCase()}`,
            "END:VEVENT",
            "END:VCALENDAR"
        ].join( "\r\n" ),
        CheckInStartOffsetMinutes: 1,
        CheckInEndOffsetMinutes: 1438,
        IsActive: true
    } ) );

    const groupLocationId = await findOrCreate( api, "GroupLocations", CheckinGuid.groupLocation, async () => ( {
        GroupId: groupId,
        LocationId: roomId,
        IsMailingLocation: false,
        IsMappedLocation: false,
        Order: 0
    } ) );

    // No printer: labels print from the client, which has none, so nothing is ever sent to a printer.
    const kioskId = await findOrCreate( api, "Devices", CheckinGuid.kiosk, async () => ( {
        Name: CheckinName.kiosk,
        Description: "Kiosk for the KFS end-to-end tests. No printer.",
        DeviceTypeValueId: await api.getIdByGuid( "DefinedValues", SystemGuid.DefinedValue.DeviceTypeCheckinKiosk ),
        PrintFrom: 0,
        PrintToOverride: 0,
        IsActive: true
    } ) );

    return { checkinTypeId, areaId, groupId, roomId, scheduleId, groupLocationId, kioskId };
}

/**
 * Checks the links the SQL script creates, as far as REST can see them, and throws
 * CheckinLinksMissingError if one is missing. The kiosk -> room link is not readable
 * through REST; the Admin page proves it by listing the test area.
 */
export async function verifyCheckinLinks( api: RockApi, setup: CheckinSetup ): Promise<void> {
    const missing: string[] = [];

    const checkinType = await api.first( "GroupTypes", `Id eq ${setup.checkinTypeId}`, "&$expand=ChildGroupTypes" );
    if ( !hasRelated( checkinType, "ChildGroupTypes", setup.areaId ) ) {
        missing.push( "check-in type -> area" );
    }

    const groupLocation = await api.first( "GroupLocations", `Id eq ${setup.groupLocationId}`, "&$expand=Schedules" );
    if ( !hasRelated( groupLocation, "Schedules", setup.scheduleId ) ) {
        missing.push( "group location -> schedule" );
    }

    if ( missing.length > 0 ) {
        throw new CheckinLinksMissingError( `Check-in links missing (${missing.join( ", " )}). Run rocks.kfs.Tests.EndToEnd/setup/AttendedCheckin_Links.sql against the site's database once, then run the tests again.` );
    }
}

/**
 * Re-saves the check-in type, area and kiosk. Rock caches check-in configuration
 * (GroupTypeCache, KioskDevice) and the SQL script's inserts bypass those caches;
 * saving through REST flushes them.
 */
export async function refreshCheckinCaches( api: RockApi, setup: CheckinSetup ): Promise<void> {
    const stamp = `Refreshed by the KFS end-to-end tests ${new Date().toISOString()}.`;
    await api.patch( "GroupTypes", setup.checkinTypeId, { Description: `Check-in type for the KFS end-to-end tests. Do not use. ${stamp}` } );
    await api.patch( "GroupTypes", setup.areaId, { Description: `Check-in area for the KFS end-to-end tests. Do not use. ${stamp}` } );
    await api.patch( "Devices", setup.kioskId, { Description: `Kiosk for the KFS end-to-end tests. No printer. ${stamp}` } );
}

/** Ensures a person has the given mobile number, so phone search can find them. */
export async function ensureMobilePhone( api: RockApi, personId: number, number: string ): Promise<void> {
    const existing = await api.first( "PhoneNumbers", `PersonId eq ${personId} and Number eq '${number}'` );
    if ( existing ) {
        return;
    }

    await api.post( "/api/PhoneNumbers", {
        PersonId: personId,
        Number: number,
        CountryCode: "1",
        NumberTypeValueId: await api.getIdByGuid( "DefinedValues", SystemGuid.DefinedValue.PhoneTypeMobile ),
        IsMessagingEnabled: false,
        IsUnlisted: false,
        IsSystem: false
    } );
}

/**
 * Finds or creates the persistent second room and keeps it beside the test room (same
 * parent location), so the Move dialog's location picker shows them together.
 * Returns its Id.
 */
export async function ensureMoveRoom( api: RockApi, setup: CheckinSetup ): Promise<number> {
    const moveRoomId = await findOrCreate( api, "Locations", CheckinGuid.moveRoom, async () => ( {
        Name: CheckinName.moveRoom,
        LocationTypeValueId: await api.getIdByGuid( "DefinedValues", SystemGuid.DefinedValue.LocationTypeRoom ),
        IsActive: true
    } ) );

    const room = await api.getById( "Locations", setup.roomId );
    const moveRoom = await api.getById( "Locations", moveRoomId );
    if ( moveRoom.ParentLocationId !== room.ParentLocationId ) {
        await api.patch( "Locations", moveRoomId, { ParentLocationId: room.ParentLocationId } );
    }

    return moveRoomId;
}

/**
 * Puts the test room back to its baseline: active, no thresholds. Room tests open/close it
 * and set thresholds; Attended Check-in shares the room and needs it open and unlimited.
 */
export async function resetRoom( api: RockApi, roomId: number ): Promise<void> {
    const room = await api.getById( "Locations", roomId );
    if ( room.IsActive !== true || room.SoftRoomThreshold != null || room.FirmRoomThreshold != null ) {
        await api.patch( "Locations", roomId, { IsActive: true, SoftRoomThreshold: null, FirmRoomThreshold: null } );
    }
}

// #region Labels

/** Key of the temporary check-in label attribute put on the test area by addAreaCheckinLabel. */
const areaLabelAttributeKey = "KFSE2ECheckinLabel";

export type AreaCheckinLabel = {
    attributeId: number;
    /** The label file's name, which the manager's Print Label dialog lists. */
    fileName: string;
};

/*
    10/13/2026 - CLAUDE

    Check-in labels are configured per area: a Label attribute on the area's group type
    whose value is a check-in label file. The test area normally has none, so Attended
    Check-in never prints, so this adds one only for the label tests and
    removeAreaCheckinLabel takes it off again. It reuses a label file the site already
    has (Rock installs its sample labels) rather than uploading ZPL, which REST cannot do.

    Reason: The test area must stay label-free for the Attended Check-in suite.
*/

/** Gives the test area a check-in label, using the site's first check-in label file. */
export async function addAreaCheckinLabel( api: RockApi, setup: CheckinSetup ): Promise<AreaCheckinLabel> {
    await removeAreaCheckinLabel( api, setup );

    const labelTypeId = await api.getIdByGuid( "BinaryFileTypes", SystemGuid.BinaryFileType.CheckinLabel );
    const labelFile = await api.first( "BinaryFiles", `BinaryFileTypeId eq ${labelTypeId} and IsTemporary eq false`, "&$orderby=Id" );
    if ( !labelFile ) {
        throw new Error( "The site has no check-in label files (Admin Tools > Check-in > Check-in Labels). Add one, e.g. Rock's sample Child Label, so the label tests have one to show." );
    }

    const attributeId = await api.post( "/api/Attributes", {
        Guid: randomUUID(),
        EntityTypeId: await api.getEntityTypeId( "Rock.Model.GroupType" ),
        EntityTypeQualifierColumn: "Id",
        EntityTypeQualifierValue: String( setup.areaId ),
        FieldTypeId: await api.getIdByGuid( "FieldTypes", SystemGuid.FieldType.Label ),
        Key: areaLabelAttributeKey,
        Name: `${TestNamePrefix} Check-in Label`,
        Description: "Created by rocks.kfs.Tests.EndToEnd for the label tests. Safe to delete.",
        Order: 0,
        IsGridColumn: false,
        IsMultiValue: false,
        IsRequired: false,
        IsSystem: false
    } );
    await api.post( "/api/AttributeQualifiers", {
        AttributeId: attributeId,
        Key: "binaryFileType",
        Value: SystemGuid.BinaryFileType.CheckinLabel,
        IsSystem: false
    } );
    // Re-save the attribute so Rock's attribute cache picks up the qualifier added after it.
    await api.patch( "Attributes", attributeId, { Description: "Created by rocks.kfs.Tests.EndToEnd for the label tests. Safe to delete." } );
    await api.setAttributeValue( attributeId, setup.areaId, String( labelFile.Guid ).toLowerCase() );

    return { attributeId, fileName: String( labelFile.FileName ) };
}

/** Removes the temporary label attribute (and its value) from the test area, if present. */
export async function removeAreaCheckinLabel( api: RockApi, setup: CheckinSetup ): Promise<void> {
    const groupTypeEntityTypeId = await api.getEntityTypeId( "Rock.Model.GroupType" );
    for ( const attribute of await api.query( "Attributes", `EntityTypeId eq ${groupTypeEntityTypeId} and Key eq '${areaLabelAttributeKey}' and EntityTypeQualifierValue eq '${setup.areaId}'` ) ) {
        for ( const value of await api.query( "AttributeValues", `AttributeId eq ${attribute.Id}` ) ) {
            await api.delete( "AttributeValues", value.Id );
        }
        for ( const qualifier of await api.query( "AttributeQualifiers", `AttributeId eq ${attribute.Id}` ) ) {
            await api.delete( "AttributeQualifiers", qualifier.Id );
        }
        await api.delete( "Attributes", attribute.Id );
    }
}

// #endregion

// #region Attendance

/** Gets attendance records in the test group, optionally for one person. */
export async function getGroupAttendance( api: RockApi, groupId: number, personId?: number ): Promise<RockEntity[]> {
    const results: RockEntity[] = [];
    const personAliasIds = personId ? ( await api.query( "PersonAlias", `PersonId eq ${personId}` ) ).map( alias => alias.Id ) : null;

    for ( const occurrence of await api.query( "AttendanceOccurrences", `GroupId eq ${groupId}` ) ) {
        for ( const attendance of await api.query( "Attendances", `OccurrenceId eq ${occurrence.Id}` ) ) {
            if ( !personAliasIds || personAliasIds.includes( attendance.PersonAliasId as number ) ) {
                results.push( attendance );
            }
        }
    }

    return results;
}

/** Deletes all attendance in the test group, so a person is never "already checked in" from an earlier test. */
export async function deleteGroupAttendance( api: RockApi, groupId: number ): Promise<void> {
    for ( const occurrence of await api.query( "AttendanceOccurrences", `GroupId eq ${groupId}` ) ) {
        for ( const attendance of await api.query( "Attendances", `OccurrenceId eq ${occurrence.Id}` ) ) {
            await api.delete( "Attendances", attendance.Id );
        }
        await api.delete( "AttendanceOccurrences", occurrence.Id );
    }
}

/*
    10/1/2026 - CLAUDE

    Check-in screens only count attendance that started today (in Rock's time
    zone) and is still open. Rather than guess the server's time zone, this
    saves the occurrence and reads back the ModifiedDateTime Rock stamped on it,
    which is RockDateTime.Now, and starts the attendance a minute before that.

    Reason: Attendance times must be in Rock's time zone, which the tests cannot see.
*/

/**
 * Finds the person's open attendance in the test group at a location, or null. Used to
 * check where a Move put someone.
 */
export async function getOpenAttendanceAt( api: RockApi, setup: CheckinSetup, person: TestPerson, locationId: number ): Promise<RockEntity | null> {
    for ( const occurrence of await api.query( "AttendanceOccurrences", `GroupId eq ${setup.groupId} and LocationId eq ${locationId}` ) ) {
        const open = await api.first( "Attendances", `OccurrenceId eq ${occurrence.Id} and PersonAliasId eq ${person.primaryAliasId} and EndDateTime eq null` );
        if ( open ) {
            return open;
        }
    }

    return null;
}

/** Checks a person into the test group, room and schedule right now (Rock's time), as a kiosk would. */
export async function createCurrentAttendance( api: RockApi, setup: CheckinSetup, person: TestPerson ): Promise<number> {
    const occurrenceFilter = `GroupId eq ${setup.groupId} and LocationId eq ${setup.roomId} and ScheduleId eq ${setup.scheduleId}`;
    const existing = await api.first( "AttendanceOccurrences", occurrenceFilter, "&$orderby=OccurrenceDate desc" );
    const occurrence = existing ?? await api.getById<RockEntity>( "AttendanceOccurrences", await api.post( "/api/AttendanceOccurrences", {
        GroupId: setup.groupId,
        LocationId: setup.roomId,
        ScheduleId: setup.scheduleId,
        OccurrenceDate: new Date().toISOString().substring( 0, 10 )
    } ) );

    await api.patch( "AttendanceOccurrences", occurrence.Id, { Notes: "KFS E2E" } );
    const rockNow = String( ( await api.getById( "AttendanceOccurrences", occurrence.Id ) ).ModifiedDateTime );
    const rockToday = rockNow.substring( 0, 10 );

    // The occurrence must be dated today in Rock's time zone; re-date one left over from another day.
    if ( !String( occurrence.OccurrenceDate ).startsWith( rockToday ) ) {
        await api.patch( "AttendanceOccurrences", occurrence.Id, { OccurrenceDate: rockToday } );
    }

    const start = new Date( `${rockNow.substring( 0, 19 )}Z` );
    start.setUTCMinutes( start.getUTCMinutes() - 1 );

    return await api.post( "/api/Attendances", {
        OccurrenceId: occurrence.Id,
        PersonAliasId: person.primaryAliasId,
        StartDateTime: start.toISOString().substring( 0, 19 ),
        DidAttend: true,
        DeviceId: setup.kioskId
    } );
}

/**
 * Puts the test room under the default campus's location. Check-in manager blocks only
 * list locations under the current campus, and with no campus context they use the
 * first campus by Order (inactive ones included), as CampusCache.All() does.
 * Returns that campus.
 */
export async function ensureRoomUnderDefaultCampus( api: RockApi, setup: CheckinSetup ): Promise<RockEntity> {
    const campus = await api.first( "Campuses", "Id gt 0", "&$orderby=Order,Id" );
    if ( !campus ) {
        throw new Error( "The site has no campus; check-in manager requires one." );
    }
    if ( !campus.LocationId ) {
        throw new Error( `Campus '${campus.Name}' (the default campus check-in manager uses) has no location. Set its location in Rock so check-in rooms can sit under it.` );
    }

    const room = await api.getById( "Locations", setup.roomId );
    if ( room.ParentLocationId !== campus.LocationId ) {
        await api.patch( "Locations", setup.roomId, { ParentLocationId: campus.LocationId } );
    }

    return campus;
}

// #endregion

// #region Per-run people

/*
    10/1/2026 - CLAUDE

    Add Person, Add Visitor and Add Family create real people through the plugin,
    and the REST API cannot delete people. Each run therefore uses a unique last
    name ("Kfsrun" + letters), each test that adds people works in its own family
    (run name + a tag letter), and teardown inactivates everyone whose last name
    starts with the run name. Letters only, because check-in treats a numeric
    search as a phone number.

    Reason: People cannot be deleted through REST; inactivating keeps searches clean.
*/

export const RunLastNamePrefix = "Kfsrun";

/** Builds a unique, letters-only last name for this run, e.g. "Kfsrunqbxmtd". */
export function createRunLastName(): string {
    let value = Date.now();
    let suffix = "";
    for ( let i = 0; i < 8; i++ ) {
        suffix = String.fromCharCode( 97 + ( value % 26 ) ) + suffix;
        value = Math.floor( value / 26 );
    }

    return `${RunLastNamePrefix}${suffix}`;
}

/** Creates an adult in a new family with the run's last name. */
export async function createRunPerson( api: RockApi, args: { firstName: string; lastName: string } ): Promise<TestPerson> {
    const guid = randomUUID();
    await api.post( "/api/People", {
        Guid: guid,
        FirstName: args.firstName,
        NickName: args.firstName,
        LastName: args.lastName,
        IsEmailActive: false,
        Gender: 0
    } );

    return await loadTestPerson( api, await api.getIdByGuid( "People", guid ) );
}

/** Finds active people with a given last name. */
export async function getActivePeopleByLastName( api: RockApi, lastName: string ): Promise<RockEntity[]> {
    const inactiveId = await api.getIdByGuid( "DefinedValues", SystemGuid.DefinedValue.RecordStatusInactive );
    return await api.query( "People", `LastName eq '${lastName}' and RecordStatusValueId ne ${inactiveId}` );
}

/** Inactivates everyone whose last name starts with this run's last name. */
export async function inactivateRunPeople( api: RockApi, runLastName: string ): Promise<void> {
    const inactiveId = await api.getIdByGuid( "DefinedValues", SystemGuid.DefinedValue.RecordStatusInactive );
    for ( const person of await api.query( "People", `startswith(LastName,'${runLastName}') and RecordStatusValueId ne ${inactiveId}` ) ) {
        await api.patch( "People", person.Id, { RecordStatusValueId: inactiveId } );
    }
}

/** Inactivates people left active by earlier runs that were killed before teardown. */
export async function inactivateLeftoverRunPeople( api: RockApi ): Promise<void> {
    const inactiveId = await api.getIdByGuid( "DefinedValues", SystemGuid.DefinedValue.RecordStatusInactive );
    for ( const person of await api.query( "People", `startswith(LastName,'${RunLastNamePrefix}') and RecordStatusValueId ne ${inactiveId}` ) ) {
        await api.patch( "People", person.Id, { RecordStatusValueId: inactiveId } );
    }
}

// #endregion

async function findOrCreate( api: RockApi, entitySet: string, guid: string, build: () => Promise<Record<string, unknown>> ): Promise<number> {
    const existing = await api.getByGuid( entitySet, guid );
    if ( existing ) {
        return existing.Id;
    }

    return await api.post( `/api/${entitySet}`, { Guid: guid, ...await build() } );
}

/**
 * True if the expanded collection contains the Id. If Rock did not return the
 * collection at all (no $expand support), the link cannot be checked here, so it
 * is assumed present and the browser tests will catch a missing link.
 */
function hasRelated( entity: RockEntity | null, property: string, id: number ): boolean {
    if ( !entity ) {
        return false;
    }

    const related = entity[ property ];
    if ( !Array.isArray( related ) ) {
        console.warn( `Rock did not return ${property}; cannot verify that check-in link through REST.` );
        return true;
    }

    return related.some( ( item: { Id?: number } ) => item.Id === id );
}
