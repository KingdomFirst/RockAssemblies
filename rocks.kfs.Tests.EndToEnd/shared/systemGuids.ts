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

/** Rock core system Guids used by the tests. Copied from Rock/SystemGuid; stable across versions. */
export const SystemGuid = {
    GroupType: {
        Family: "790E3215-3B10-442B-AF69-616C0DCB998E",
        SmallGroup: "50FCFB30-F51A-49DF-86F4-2B176EA1820B",
        ServingTeam: "2C42B2D4-1C5F-4AD5-A9AD-08631B872AC4",
        FundraisingOpportunity: "4BE7FC44-332D-40A8-978E-47B7035D7A0C"
    },
    GroupRole: {
        FamilyMemberAdult: "2639F9A5-2AAE-4E48-A8C3-4FFE86681E42",
        FamilyMemberChild: "C8B1814F-6AA7-4055-B2D7-48FE20429CB9"
    },
    NoteType: {
        PersonTimelineNote: "66A1B9D7-7EFA-40F3-9415-E54437977D60",
        GroupMemberNote: "FFFC3644-60CD-4D14-A714-E8DCC202A0E1"
    },
    DefinedValue: {
        CheckinTemplatePurpose: "4A406CB0-495B-4795-B788-52BDFDE00B01",
        DeviceTypeCheckinKiosk: "BC809626-1389-4543-B8BB-6FAC79C27AFD",
        LocationTypeRoom: "107C6DA1-266D-4E1C-A443-1CD37064601D",
        PhoneTypeMobile: "407E7E45-7B2E-4FCD-9605-ECB1339F2453",
        RecordStatusActive: "618F906C-C33D-4FA3-8AEF-E58CB7B63F1E",
        RecordStatusInactive: "1DAD99D5-41A9-4865-8366-F269902B80A4",
        TransactionTypeContribution: "2D607262-52D6-4724-910D-5C6E8FB89ACC"
    }
} as const;

/** Rock entity type names, resolved to Ids at runtime with RockApi.getEntityTypeId(). */
export const EntityTypeName = {
    Block: "Rock.Model.Block",
    Page: "Rock.Model.Page",
    Person: "Rock.Model.Person",
    PersonAlias: "Rock.Model.PersonAlias",
    Group: "Rock.Model.Group",
    GroupMember: "Rock.Model.GroupMember",
    ConnectionRequest: "Rock.Model.ConnectionRequest",
    Note: "Rock.Model.Note",
    NoteType: "Rock.Model.NoteType",
    Workflow: "Rock.Model.Workflow",
    WorkflowActionType: "Rock.Model.WorkflowActionType",
    SetAttributeFromEntityAction: "Rock.Workflow.Action.SetAttributeFromEntity"
} as const;

/** Rock field type classes, resolved to Ids at runtime with RockApi.getFieldTypeId(). */
export const FieldTypeClass = {
    Text: "Rock.Field.Types.TextFieldType",
    Entity: "Rock.Field.Types.EntityFieldType"
} as const;

/**
 * Every entity the suite creates is named with this prefix, so a run can sweep up
 * leftovers from an earlier run that was killed before its cleanup ran.
 */
export const TestNamePrefix = "KFS E2E";
