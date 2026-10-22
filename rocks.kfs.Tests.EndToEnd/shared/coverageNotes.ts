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

/*
    10/22/2026 - CLAUDE

    The parts of TEST-COVERAGE.md that cannot be read from the tests: what each plugin
    does, what its tests do not cover, known plugin bugs and steps that stay manual. The
    list of tests itself is generated from the suite (npm run coverage), so it never
    drifts; keep these notes current when a plugin or its tests change, then regenerate.

    Keys are Rock Packages folder names without "(Unreleased)", as in plugins.ts.

    Reason: A plain-language coverage record needs facts the test code does not hold.
*/

export type PluginNotes = {
    /** One or two sentences on what the plugin does, for someone who has not used it. */
    about: string;
    /** What the functional tests do not exercise. */
    notCovered: string[];
    /** Plugin bugs a test is known to fail on (tests tagged "known-issue"). */
    knownIssues?: string[];
    /** Checks that stay manual, or setup a person must do once. */
    manual?: string[];
};

export const CoverageNotes: Record<string, PluginNotes> = {
    "Advanced Check-in Monitor": {
        about: "Replaces the Live Metrics block of Rock's Check-in Manager with a room-by-room view of who is checked in, where staff can move, check out or remove people, view and reprint labels, open and close rooms, and set room thresholds.",
        notCovered: [
            "Close Occurrence mode (closes a group's occurrence instead of the room; needs the KFS Load Balance Locations check-in workflow).",
            "The Include Group Move, Advanced Print Options and Search By Code settings.",
            "Sending a label to a printer (the plugin's print button is turned off in its code; the tests check the label preview).",
            "Rock's location tree in the Move dialog: the tests choose the new room directly, because Rock's REST API refuses the test sign-in the tree uses."
        ],
        manual: [ "The open/close tests need a one-time Administrate rule for the test role on the Check-in Manager site (README)." ]
    },
    "Advanced Group Finder": {
        about: "KFS's version of Rock's Group Finder: lists groups people can join, with filters for campus, day, keyword, postal code and group attributes, and extra display settings.",
        notCovered: [
            "Postal-code proximity search (needs geocoded group locations).",
            "The map.",
            "Keyword search across group attributes.",
            "Collapsible filters and custom attribute sort.",
            "PersonGuid mode and Group Opportunities."
        ]
    },
    "Attended Checkin": {
        about: "NewSpring's attended (staff-run) check-in, maintained by KFS: staff search for a family, pick who is checking in and confirm rooms and schedules.",
        notCovered: [
            "Printing labels (needs a printer; the test kiosk has none).",
            "Changing a person's group, room or schedule on the Activity Select page.",
            "The Override button and the on-screen keypad.",
            "Matching a kiosk by geolocation."
        ]
    },
    "Custom Group Reminder Emails": {
        about: "A job that reminds members of flagged groups about an upcoming meeting, by email or text, a set number of days before it.",
        notCovered: [
            "Choosing a different system communication for the reminder.",
            "Groups on schedules other than a simple weekly one."
        ],
        manual: [ "Each sending test notes what to check: the email in Edify's outgoing messages and the text on the phone." ]
    },
    "Eventbrite": {
        about: "Links Rock groups to Eventbrite events and brings each event's attendees into the group as members, with their ticket details.",
        notCovered: [
            "Automatic syncing through the Eventbrite webhook (Sync.ashx); the tests sync with the Sync button.",
            "The Ticket Class data view filter.",
            "The New Event Statuses and Display Eventbrite Event Name settings."
        ],
        knownIssues: [
            "On Rock 20 the group editor shows a plain text box for the Eventbrite Event attribute instead of a list of events (Rock 20's Group Detail is an Obsidian block, and the plugin's field type has no Obsidian editor), so linking a group from the group editor fails there."
        ],
        manual: [ "Creating the Eventbrite test event and registering an attendee (Eventbrite has no API for registering attendees)." ]
    },
    "Fundraising Participant Summary": {
        about: "A job that emails each fundraising trip participant a summary of the gifts made toward their goal since the last run.",
        notCovered: [
            "The Show Address and Show Amount settings.",
            "Choosing groups by Group Types instead of a single group.",
            "A custom system communication."
        ],
        manual: [ "Each sending test notes what to check in Edify's outgoing messages." ]
    },
    "KFS Fundraising Progress": {
        about: "A block that shows a fundraising group's progress toward its goal, overall and for each participant, with an Excel export.",
        notCovered: [
            "Turning display settings off one at a time: the tests turn them all off together, and Show Group Member Goals on its own."
        ]
    },
    "KFS Workflow Actions": {
        about: "A workflow action that creates a prayer request (KFS: Prayer > Add Prayer Request).",
        notCovered: [ "No functional tests yet. The compatibility preflight still checks that it builds." ]
    },
    "Microsoft 365 Utilities": {
        about: "A Lava shortcode that lists items from a Microsoft 365 calendar, and a job that launches a workflow for each new email in a Microsoft 365 mailbox.",
        notCovered: [
            "The job's One Workflow Per Conversation and Workflow Attributes settings.",
            "Marking processed emails in ways other than Read or Add Flag."
        ],
        manual: [ "When the Azure app's secret expires, the tests fail with AADSTS7000222; create a new secret and update the EWS Azure Secret global attribute and the test settings." ]
    },
    "Person Attributes Form Advanced": {
        about: "A multi-page form that updates a person's details and attributes, and can add them to a group, create a connection request, launch a workflow and ask for an electronic signature.",
        notCovered: [
            "Defaulting a blank Connection Status to Prospect: the form never saves a blank person field, so this fallback cannot be reached.",
            "Saving after each page (Save Values = each page); the tests save at the end.",
            "Drawn signatures, signature templates that stay valid for a period, and signature completion emails.",
            "Visibility rules on fields other than text attributes."
        ],
        knownIssues: [
            "A visibility rule that hides a person field (for example Middle Name) crashes the form.",
            "A value typed into a field that a visibility rule then hides is still saved."
        ]
    },
    "Person Contact Notes": {
        about: "A block for quickly adding a note to a person, or to a group member, that can launch a workflow and change the member's status.",
        notCovered: [
            "The Allow Backdated Notes, Show Alert Checkbox, Show Private Checkbox and Show Security Button settings.",
            "The Note View and Person Info Lava templates, Display Note Type Heading, Expand Replies and Display Type.",
            "Start New Search After Adding a Note."
        ],
        manual: [ "One-time Edit rules for the test role on the Personal Note and Group Member Note types (README)." ]
    },
    "Round Robin Group Assignment": {
        about: "A job that spreads the people in a data view across a set of groups in turn, optionally keeping families together.",
        notCovered: [
            "The Campus, Use Group Campus, Include Selected Groups and Output Errors settings."
        ]
    },
    "Scheduled Group Communication": {
        about: "Jobs that send a group's scheduled emails and texts, entered as dated items on the group.",
        notCovered: [
            "Recurrences other than One Time and Weekly.",
            "The Last Run Buffer setting."
        ],
        manual: [ "Each sending test notes what to check: the email in Edify's outgoing messages and the text on the phone." ]
    },
    "Steps to Care": {
        about: "A care-ministry tool: record care needs, assign care workers, add notes and follow-ups, and track each need on a dashboard.",
        notCovered: [
            "The time-driven scenarios run by the Care Need Automated Processes job: scheduled future needs, touch-template flags, automatic follow-up and snooze expiry.",
            "Automatic worker assignment (turned off on the test pages so no real worker is notified).",
            "The core Rock pages the plugin links to (Add Prayer Request, Launch Workflow, View History): the tests check the plugin opens the right page, not the page itself."
        ]
    },
    "Batch to Financial Edge": {
        about: "Exports Rock financial batches to Blackbaud Financial Edge as journal entries (CSV).",
        notCovered: [ "No functional tests yet. The compatibility preflight still checks that it builds and its pages compile." ]
    },
    "Batch to Intacct": {
        about: "Exports Rock financial batches to Sage Intacct as journal entries or other receipts, by API or file.",
        notCovered: [ "No functional tests yet. The compatibility preflight still checks that it builds and its pages compile." ]
    },
    "Batch to Shelby Financials": {
        about: "Exports Rock financial batches to Shelby Financials as journal entries (Excel).",
        notCovered: [ "No functional tests yet. The compatibility preflight still checks that it builds and its pages compile." ]
    },
    "Vimeo Sync": {
        about: "Syncs video details from Vimeo into Rock content channel items, one item or a whole channel at a time.",
        notCovered: [ "No functional tests yet. The compatibility preflight still checks that it builds and its pages compile." ]
    },
    "Cybersource Gateway": {
        about: "A CyberSource payment gateway for Rock giving and registrations (unreleased).",
        notCovered: [ "No functional tests yet. The compatibility preflight checks that it builds; its Obsidian control is not compiled by either suite." ]
    },
    "Zoom Room Management": {
        about: "Connects Rock room reservations with Zoom meetings (unreleased).",
        notCovered: [ "No functional tests yet. The compatibility preflight still checks that it builds and its page compiles." ]
    },
    "Canopy": {
        about: "Exports Rock financial batches to Canopy (unreleased).",
        notCovered: [ "Not tested by either suite: its package ships only Obsidian files, and its DLL is not built on this branch." ]
    }
};
