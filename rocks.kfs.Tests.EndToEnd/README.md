# rocks.kfs.Tests.EndToEnd

Automated compatibility tests for KFS plugins. When a new Rock version drops, install it on a
test site with our plugins, point this suite at it, and run it. Each plugin's manual test plan
is encoded here as repeatable tests.

The suite is modeled on Rock's own test projects (`Rock.Tests`, `Rock.Tests.Integration`,
`Rock.Tests.Shared`). It uses the same naming convention, test rules and setup/teardown
discipline. It differs in one way: it drives a **running Rock site** through a browser and
Rock's REST API rather than calling code in-process.

## Why end-to-end rather than MSTest

Rock's integration tests call services directly against a test database. That works for
Rock's model and service code. Most KFS plugins are WebForms blocks, and their logic lives in
the `.ascx.cs` code-behind, tied to `RockPage`, page parameters, ViewState and UpdatePanels.
It cannot run outside a web request without refactoring the plugin. Rendering the block in a
real Rock instance also catches the most common kind of break between Rock versions: a block
that no longer compiles or renders against the new Rock assemblies.

## How it works

For each plugin, a **worker-scoped fixture** builds everything its tests need on the target
site, uses it for the whole run, then deletes it:

- A hidden test page (under a parent page you choose) holding one instance of the block.
- Test people. These are created once by fixed Guid and reused on every run, because Rock
  does not cleanly delete people.
- Whatever else the plugin touches (groups, a connection type, workflow types, attributes).

Tests then:

1. **Arrange**: set the block's settings through the REST API (every setting is reset to a
   baseline before each test, so tests do not depend on order).
2. **Act**: open the page in a browser signed in as a test person. Sign-in uses a Rock
   impersonation token (`rckipid`), so no password is ever handled. The test fills in the
   block and submits it.
3. **Assert**: read the result back through the REST API: person values, group members,
   connection requests, and the workflows that were launched.

**Workflow verification.** Rather than depending on real workflows (such as "Second Visit"),
the fixture creates tiny workflow types whose only action is *Set Attribute From Entity*. That
action records exactly which entity the workflow was launched with. So a test can prove both
that the workflow ran and that it received the right Person, Group Member or Connection
Request. That is stricter than the manual check.

Every entity the suite creates is named with the prefix `KFS E2E`. If a run is killed before
cleanup, the next run finds those leftovers by name and deletes them first.

## Setup

Prerequisites: Node 18+ and Microsoft Edge or Google Chrome (the installed browser is used,
so there is nothing else to download).

```bash
cd KFSRockAssemblies/rocks.kfs.Tests.EndToEnd
npm install
cp .env.example .env
```

Fill in `.env`:

| Setting | Meaning |
|---|---|
| `ROCK_BASE_URL` | Site under test: `https://admin-rockbeta.kingdomfirstsolutions.com` |
| `ROCK_API_KEY` | A REST key whose REST user can create, edit and delete the entities listed in `.env.example`. Putting the REST user in *RSR - Rock Administration* on the **test** site is simplest. |
| `ROCK_TEST_PARENT_PAGE_ID` | An existing page the tests put their persistent, hidden `KFS E2E Test Pages` page under; test pages are created below that. Its layout must have a `Main` zone (or set `ROCK_TEST_ZONE`). On rockbeta: 206 (Support Pages). |
| `BROWSER_CHANNEL` | `msedge` (default) or `chrome` |

> **Only point this at a beta/test site.** The suite creates and deletes data. `.env` is
> git-ignored; never commit a real key.

The plugin under test must be installed on the site. For Person Attribute Forms Advanced, that
means `~/Plugins/rocks_kfs/Crm/PersonAttributeForms.ascx` must be deployed and registered.

## Running

```bash
npm test                                   # everything
npx playwright test tests/Crm/PersonAttributeForms
npx playwright test tests/CheckIn/AttendedCheckin
npx playwright test -g "Submit_GroupId"    # by name
npm run test:headed                        # watch it in a browser window
npm run report                             # open the HTML report of the last run
```

Output:

- `test-results/results.json`: machine-readable results. Each test carries a `rock-version`
  annotation with the version it ran against.
- `test-results/html/`: HTML report.
- `test-results/artifacts/`: a screenshot and a Playwright trace for every failure
  (`npx playwright show-trace <zip>`).

`npm run typecheck` and `npm run test:list` need no site and no `.env`.

## Layout

```
rocks.kfs.Tests.EndToEnd/
  shared/                     # like Rock.Tests.Shared
    config.ts                 # .env settings
    rockApi.ts                # Rock v1 REST client (query, create, patch, delete, attribute values)
    rockBrowser.ts            # sign in by impersonation, detect Rock error pages, wait for UpdatePanels
    systemGuids.ts            # Rock system Guids / type names used by tests
    testData/                 # like Rock.Tests.Integration/TestData: builders + cleanup
      cms.ts people.ts groups.ts connections.ts workflows.ts attributes.ts checkin.ts
  setup/                      # one-time SQL a plugin needs that REST cannot create
  tests/
    {Domain}/{Plugin}/        # organized by Rock domain, like Rock's test projects
      {plugin}.fixture.ts     # builds and tears down the plugin's test environment
      {plugin}Block.ts        # page object: selectors for the block's controls
      *.spec.ts               # the tests
```

## Person Attribute Forms Advanced

`tests/Crm/PersonAttributeForms/`: 32 tests.

**One-time setup per site.** The test page is created under `KFS E2E Test Pages` (the fixture
creates that page on its first run) and inherits its security. Only the adult test person
*Pafa KFS E2E Tester* opens the page; View comes from the site (rockbeta's External Website
allows all users), and the *Edit Forms and Fields* test needs Edit. In Rock:

1. Create a security role `KFS E2E Plugin Testers` with **Elevated Security Level: None**, and
   add Pafa KFS E2E Tester to it.
2. On the `KFS E2E Test Pages` page's security, add an **Edit** Allow rule for that role.

3. Build the capture workflow type once, in **Admin Tools > Workflow Configuration**:
   - name `KFS E2E Entity Capture`, any category, **Automatically Persisted** on;
   - a workflow attribute *Launched Entity* (key `LaunchedEntity`, field type **Entity**);
   - in the first activity (activated with the workflow), one **Set Attribute From Entity**
     action whose *Attribute* setting is *Launched Entity*.

   The tests point the block's Workflow setting at it and check which entity (person, group
   member or connection request) the workflow received. It is built by hand because creating
   a workflow action type through the REST API (v1 or v2) hangs and pegs the server's CPU on
   Rock 17.9; see `shared/testData/workflows.ts`.

The fixture checks all of this before creating any test data and stops with the missing step.

First passing run: 32 of 32 on Rock 17.9.0 (rockbeta, 2026-10-05). The first page load after an
app pool recycle compiles the block and can exceed a test's 90-second limit; rerun if only the
first test times out. (The
fixture used to grant these rights through REST, which Rock 17.9 ignores; see
`shared/testData/security.ts`.)

How the manual test plan maps to the tests:

| Manual step | Test(s) |
|---|---|
| Form with Person Field + Person Attribute items; values saved | `fields › Submit_PersonFieldsAndPersonAttribute_SavesValuesToPerson`, `Submit_TwoForms_SavesValuesFromBothForms` |
| Confirmation Text / Done Page | `fields › Submit_Finished_ShowsConfirmationText`, `Submit_DonePageSet_RedirectsToDonePage` |
| Workflow Entity = Person runs | `fields › Submit_PersonWorkflowEntity_LaunchesWorkflowWithPerson` |
| `&GroupId=` of an allowed type adds member with configured status | `groups › Submit_GroupIdOfAllowedGroupType_AddsGroupMemberWithConfiguredStatus`, `Submit_GroupMemberStatusPending_AddsPendingGroupMember` |
| Group guid mode | `groups › Submit_GroupGuidOfAllowedGroupType_AddsGroupMember` |
| Workflow Entity = Group Member runs | `groups › Submit_GroupMemberWorkflowEntity_LaunchesWorkflowWithGroupMember` |
| GroupId of a type not allowed is not added | `groups › Submit_GroupIdOfDisallowedGroupType_DoesNotAddGroupMember` |
| Group block setting overrides Allowed Group Types | `groups › Submit_GroupSettingOfDisallowedGroupType_AddsGroupMemberWithoutPageParameter` |
| *(beyond the manual plan)* Enable Passing Group Id off, Allow Group Membership off, no duplicate members | `groups › …PassingGroupIdDisabled…`, `…GroupMembershipDisallowed…`, `…AlreadyGroupMember…` |
| `&OpportunityId=` creates a connection request | `connections › Submit_OpportunityId_CreatesConnectionRequestInDefaultStatus` |
| Workflow Entity = Connection Request with new request | `connections › Submit_OpportunityIdWithConnectionRequestWorkflowEntity_LaunchesWorkflowWithNewRequest` |
| `&ConnectionRequestId=` launches workflow with existing request | `connections › Submit_ConnectionRequestId_LaunchesWorkflowWithExistingRequest` |
| Family Member Picker × Person Mode, every combination | `person mode › PageLoad_*` (6 tests), `SelectFamilyMember_…` |
| Person Mode rules for `?Person=` | `person mode › Submit_*Mode*` (4 tests) |
| "Edit Forms and Fields" dialog | `settings › EditFormsAndFields_OpenAndSave_PreservesConfiguration` |

Not covered yet: the signature document settings (they need an electronic signature
template), the SMS checkbox, and field visibility rules.

## Attended Check-in

`tests/CheckIn/AttendedCheckin/`: 21 tests, run against the plugin's own pages
(`/attendedcheckin/admin`, `/search`, `/family`, `/confirm`, `/activity`) on the site under test.

**One-time setup per site.** Unlike the other suites, the check-in configuration persists
between runs: a check-in type, area, group, room, always-open schedule and kiosk, all named
`KFS E2E …` with fixed Guids (`shared/testData/checkin.ts`). REST cannot create check-in's
three link tables, so a SQL script does that once:

1. Run the suite. The fixture creates the entities, then stops with "Check-in links missing".
2. Run `setup/AttendedCheckin_Links.sql` against the site's database (idempotent; teardown at
   the bottom of the file).
3. Make sure **Allow Manual Setup** is on in the *Check-in Administration* block on
   `/attendedcheckin/admin`. The tests pick the test kiosk from its device list; the fixture
   checks this and stops with a message, but never changes it.
4. Run the suite again.

The test kiosk has no printer (labels print from the client) and the test area has no labels,
so nothing is ever printed. Each test starts a fresh browser session and the fixture deletes
the test group's attendance before every test.

Add Person, Add Visitor and Add Family create real people, which REST cannot delete. Each
run uses a unique letters-only last name (`Kfsrun…`), each add test works in a new family of
its own, and teardown inactivates everyone whose last name starts with the run's name (and
any left active by an aborted run).

| Manual step | Test(s) |
|---|---|
| Launch the Admin screen; configuration works smoothly | `admin › Admin_Load_ListsTestKiosk`, `Admin_SelectKiosk_ListsTestArea`, `Admin_KioskAndArea_OpensSearch` |
| *(Admin validation)* | `admin › Admin_OkWithoutDevice_WarnsToSelectDevice`, `Admin_OkWithoutArea_WarnsToSelectArea`, `Admin_AreaToggledTwice_IsDeselected` |
| Search / Search Results | `search › Search_ByLastName_FindsFamilyAndMembers`, `Search_ByPhone_FindsFamily`, `Search_PhoneTooShort_WarnsMinimumLength`, `Search_NoMatch_ShowsNoResults` |
| *(Family page selection)* | `family select › FamilySelect_Default_PreselectsEligibleMembers`, `FamilySelect_NoPersonSelected_WarnsToPickPerson`, `FamilySelect_OnePersonSelected_ConfirmsOnlyThatPerson` |
| Add Person | `family select › AddPerson_NewPerson_AddedToFamilyAndSelected` |
| Add Visitor | `family select › AddVisitor_NewChild_ListedAsVisitorNotFamilyMember` |
| Add Family | `family select › AddFamily_NewFamily_SelectedAndConfirmable` |
| Confirm | `confirm › Confirm_SelectedFamily_ListsPeopleWithRoomAndSchedule`, `Confirm_PrintAll_SavesAttendanceWithoutPrinterError`, `Confirm_Done_SavesAttendanceAndReturnsToSearch`, `Confirm_DeleteRow_RemovesPerson`, `Confirm_EditRow_OpensActivitySelectAndReturns` |

First passing run: 21 of 21 on Rock 17.9.0 (rockbeta, 2026-10-05). The pages move between each
other through the `/attendedcheckin/*` routes, so the site's page routes must be loaded (if every
route, even `/login`, returns 404 while `/page/{id}` works, recycle the app pool).

Not covered yet: label printing (needs a printer), changing a person's group, room or schedule
on Activity Select, the Override button, the keypad, and geolocation kiosk matching.

## Advanced Check-in Manager

`tests/CheckIn/CheckinManager/`: 11 tests for the *KFS > Check-in > Manager > Locations* block
(RockShop: Advanced Check-in Monitor).

**Setup the fixture does each run**, matching the manual plan's first step: it copies Rock's
Check-in Manager *Live Metrics* page (same parent, layout and other blocks, hidden from
navigation), swaps the core Live Metrics block for the KFS Locations block, and copies the
core block's settings across, as an admin would. The copy is deleted after the run. It is
pointed at the same persistent `KFS E2E` check-in configuration as Attended Check-in, so
`setup/AttendedCheckin_Links.sql` must have been run on the site. The fixture also:

- puts the test room under the default campus's location (the block only lists locations
  under the current campus; with no campus context that is the first campus by Order), and
  stops with a message if that campus has no location;
- checks the test child into the test room before every test, using Rock's own clock (see
  `createCurrentAttendance`) so the attendance is "now" in Rock's time zone;
- signs in as the staff test person *Manny KFS E2E Tester* by impersonation token.

**One-time setup per site** (besides `setup/AttendedCheckin_Links.sql`). The copied page has no
security of its own; it inherits the Check-in Manager site's, which allow View only to certain
roles. The tests cannot grant access themselves: Rock 17.9 refuses security role membership
changes through REST, and a security rule saved through REST is ignored until Rock's security
cache is cleared. So, in Rock:

1. Create a security role `KFS E2E Plugin Testers` (shared by all suites) with **Elevated Security Level:
   None**, and add Manny KFS E2E Tester to it. (The fixture creates Manny on its first run.)
2. On the *Rock Check-in Manager* site's security, add a View **Allow** rule for that role
   *above* the "All Users: Deny" rule.
3. If Manny was ever in an Extreme role (RSR - Staff Workers, Staff Like Workers, Rock
   Administration are Extreme on rockbeta), remove him and run the **Process Elevated
   Security** job. Rock refuses token sign-in for Extreme accounts, and only that job lowers a
   person's account protection profile.

The fixture checks the role membership and the protection profile before opening any page,
and stops with a message naming the step that is missing.

First passing run: 11 of 11 on Rock 17.9.0 (rockbeta, 2026-10-05).

| Manual step | Test(s) |
|---|---|
| Set up a copy of the Check-in Manager page with the KFS Locations block | fixture, plus `launch › Setup_CopiedPage_CarriesCoreBlockSettings` |
| Launch the Check-in Manager page | `launch › Launch_ManagerPage_ShowsCheckinTypeWithoutWarning`, `Launch_ManagerPage_DrawsAttendanceChart`, `Launch_AreaCount_IncludesCheckedInPerson` |
| *(Plugin features)* | `room › Navigate_AreaGroupRoom_ListsCheckedInPersonWithActions`, `Navigate_HeadingClicked_GoesUpALevel`, `Search_ByName_FindsCheckedInPerson`, `Checkout_Person_EndsAttendanceAndLeavesRoom`, `Delete_Person_RemovesAttendance`, `Move_Person_OpensLocationDialog`, `Settings_AttendeeActionsOff_HidesButtons` |

The chart test is the one most likely to catch a Rock upgrade break: Rock 20 deleted the chart
options class the block used, and the block now carries its own copy.

Not covered yet: completing a Move (the location picker), Move All, View Labels / reprint
(needs label data from a real check-in), opening/closing rooms, thresholds, and Location
navigation mode.

## Scheduled Group Communication

`tests/Communication/ScheduledGroupCommunication/`: 5 tests for the *Send Scheduled Group Email
(Plugin)* and *Send Scheduled Group SMS (Plugin)* jobs. **They send real messages**, to the
email address and mobile number set in `.env` (`SGC_TEST_EMAIL`, `SGC_TEST_SMS_NUMBER`; see
`.env.example`).

The fixture finds or creates, by fixed Guid (persistent between runs):

- a test group type with the two Matrix group attributes the plugin needs, one on the
  *Scheduled Emails* template and one on *Scheduled SMS Messages*;
- a test group whose only active member is a test person with that email and number;
- two **inactive** test jobs, one of each job type. Rock's scheduler never runs them; the tests
  press Run Now through Rock's own Jobs Administration block action
  (`api/v2/BlockActions/.../RunNow`), since Rock has no REST endpoint that runs a job.

Each sending test adds a One Time matrix item due 30 seconds ago, sets its job's last run to a
minute before that (the jobs send items dated from their last run to now; a job that never ran
looks back a whole day and would resend real groups' messages), runs the job, and deletes the
item at once so the site's own scheduled jobs never send it a second time.

| Manual step | Test(s) |
|---|---|
| Test group with the two matrix attributes; email item One Time, today | fixture, `email › EmailJob_OneTimeItemDueNow_SendsEmailToGroupMember` |
| SMS item One Time, today, from a Twilio number; member with an SMS-enabled number | fixture, `sms › SmsJob_OneTimeItemDueNow_SendsTextFromSystemPhoneNumber` |
| Configure and run the email job; history shows it was sent | `email › EmailJob_OneTimeItemDueNow_…` (job result, communication, recipient Delivered) |
| Configure and run the SMS job; history shows it was sent | `sms › SmsJob_OneTimeItemDueNow_…` (job result, from-number, recipient Delivered) |
| *(beyond the manual plan)* | `email › EmailJob_ItemScheduledTomorrow_IsNotSent`, `EmailJob_WeeklyItem_SendsAndMovesSendDateOneWeek`, `sms › SmsJob_ItemScheduledTomorrow_IsNotSent` |
| Check Edify outgoing messages; check the text arrived on the phone | **manual**: each sending test adds a `manual-check` annotation to the report saying exactly what to look for |

No security setup is needed: nothing is opened in a browser, and the REST key's user (RSR -
Rock Administration) can run jobs.

## Person Contact Notes

`tests/Crm/PersonContactNotes/`: 8 tests for the *Person Contact Note* block (KFS > CRM).

The fixture creates a hidden test page under *KFS E2E Test Pages* with the block, configured
like page 821 on the KFS beta site but limited to one note type per entity (Personal Note,
Group Member Note). It also creates a test Small Group with two members: an Active one and a
Pending one. The test people (`Pcnsubject` and `Pcnpending KFS E2E Tester`) persist between
runs. The workflow is the persistent *KFS E2E Entity Capture* type, which records the entity
it was launched with. Each test deletes the notes and workflows the previous one created.

The notes are entered by Pafa KFS E2E Tester, who is already in **KFS E2E Plugin Testers**.

**One-time setup:** the note editor only offers note types the signed-in person may Edit. In
Admin Tools > Settings > Note Types, give **KFS E2E Plugin Testers** an Allow rule for Edit
on **Personal Note** and on **Group Member Note**. The fixture checks for both rules.

| Manual step | Test(s) |
|---|---|
| Page with the block; Workflow a Person workflow, Workflow Entity Person | fixture |
| Launch page; select a person with the person picker | `personMode › PageLoad_NoParameters_ShowsPersonPickerWithoutNoteEditor`, `SelectPerson_PersonPicker_ReloadsForPersonWithDefaultNoteText` |
| Type a note, Save Note; note saved, workflow ran | `personMode › SaveNote_PersonWorkflowEntity_SavesNoteAndLaunchesWorkflowForPerson` (note on the person, created by the tester; workflow received the person) |
| Workflow a GroupMember workflow, Workflow Entity Group Member; launch with `&GroupId=` | `groupMemberMode › PageLoad_WithGroupId_ShowsGroupMemberDropDownInsteadOfPersonPicker` |
| Picker becomes "Select a Group Member" drop down; select a member | `groupMemberMode › SelectGroupMember_DropDown_ReloadsForGroupMember` |
| Type a note, Save Note; note saved, workflow ran | `groupMemberMode › SaveNote_GroupMemberWorkflowEntity_SavesNoteAndLaunchesWorkflowForGroupMember`, `SaveNote_SetStatusActiveWorkflow_ActivatesPendingMember` (uses the beta site's *Group Member - Set Status Active*; skipped if absent) |
| *(beyond the manual plan)* | `personMode › SaveNote_NoteWorkflowEntity_LaunchesWorkflowForNote` |

## Advanced Fundraising Progress

`tests/Fundraising/FundraisingProgress/`: 8 tests for the *Fundraising Progress* block
(KFS > Fundraising).

The fixture creates a hidden test page under *KFS E2E Test Pages* holding the block. Its
settings match the block on the KFS beta site's Fundraising page (page 486), with every
display option on.

It also creates two Fundraising Opportunity groups:
- **KFS E2E AFP Trip**, with a default goal of 1,000 and two participants, *Fundtesta* and
  *Fundtestb KFS E2E Tester*. These two test people persist between runs.
- **KFS E2E AFP Other Trip**, with no members.

The participants get test contribution transactions. Fundtesta has their own goal of 500 and
two gifts totalling 200, so 40%. Fundtestb uses the group default and gave 1,000, so 100%.
The group shows 1,200 of 1,500, so 80%. Every amount, percentage and bar color is checked
against these numbers. The fixture deletes the transactions, groups and page afterwards.

No security setup is needed: Pafa views the page with the site's View rights.

**Show Specific Group needs the current block.** RockShop package *KFS Fundraising Progress*
v1.0 predates that setting, which was added to the source on 2020-06-09. If the installed
block lacks the setting, the two Show Specific Group tests fail with that explanation. To
test it, deploy the current `Fundraising/FundraisingProgress.ascx` and `.ascx.cs` from
KFSRockBlocks.

| Manual step | Test(s) |
|---|---|
| Page with the block | fixture |
| Launch for a fundraising group with contributions; no errors | `groupParameters › PageLoad_GroupIdParameter_ShowsGroupTotalsAndEachMembersProgress` (no Rock errors; title, total raised, group and member amounts, percentages, bar colors) |
| Set Show Specific Group; launch with no group in the URL; block shows that group | `showSpecificGroup › PageLoad_ShowSpecificGroupSettingWithoutParameters_ShowsThatGroup` |
| *(beyond the manual plan)* | `showSpecificGroup › PageLoad_GroupIdParameterAndShowSpecificGroupSetting_ParameterWins`, `groupParameters › PageLoad_GroupMemberIdParameter_ShowsOnlyThatMember`, `PageLoad_NoGroupParameterOrSetting_ShowsNoProgress`, `settings › PageLoad_DisplaySettingsOff_…`, `PageLoad_GroupMemberGoalsOff_HidesMemberList`, `ExportToExcel_GroupIdParameter_DownloadsWorkbookNamedForGroup` (file name, headers, each participant) |

## Fundraising Participant Summary

`tests/Fundraising/FundraisingParticipantSummary/`: 3 tests for the *Fundraising Participant
Summary* job. **They send real emails** to `FPS_TEST_EMAIL` (default `SGC_TEST_EMAIL`; see
`.env.example`).

The fixture creates its own data each run and deletes it after (rockbeta's job 122 and real
groups are not touched):

- an **inactive** test job of that job type, kept between runs by fixed Guid. The tests set its
  Group setting to the test group and press Run Now, as Scheduled Group Communication does;
- a Fundraising Opportunity group, **KFS E2E FPS Trip**, with three participants. They are
  test people kept between runs, all with the test email address:
  - *Fpsdonor*: a $150 gift an hour before setup;
  - *Fpsquiet*: an $80 gift two days before;
  - *Fpsoptout*: a $60 gift an hour before, but with *Disable Public Contribution Requests* on.

Each test sets the job's last run, which starts the window the job reports on, then runs the
job. It then checks the job result and each summary email in communication history: who it
went to, the subject, the greeting, the group, the gift amount and the delivery status.

| Manual step | Test(s) |
|---|---|
| Configure a Fundraising Participant Summary job and run it | fixture, every test |
| History shows the summary emails were sent | `summaryJob › Run_GiftSinceLastRun_EmailsOnlyTheParticipantWithTheNewGift` (only the donor; the email lists the $150 gift), `Run_SendEmailsWithZeroDonationsOn_EmailsEveryParticipantExceptOptedOut` (donor and quiet, not the opted-out participant) |
| *(beyond the manual plan)* | `summaryJob › Run_NoGiftsSinceLastRun_SendsNoEmails` |
| Check Edify outgoing messages | **manual**: each sending test adds a `manual-check` annotation to the report saying what to look for |

No security setup is needed: nothing is opened in a browser.

## Eventbrite Sync

`tests/Event/EventbriteSync/`: 5 tests for the *Eventbrite Settings* and *Eventbrite Sync
Button* blocks and the plugin's two field types. They call the live Eventbrite API through the
plugin, using the token and organization already saved on the site. The tests only read from
Eventbrite; linking a group does not register a webhook.

**The private token stays out of the test output.** The Eventbrite Settings block shows the
decrypted private token in plain text. The suite rewrites every response for its settings test
page so the token's characters become `*` before the browser sees them, and blocks the page if
the markup no longer matches. Traces are off for this suite, because they keep `__VIEWSTATE`,
which holds the token.

The fixture builds, by fixed Guid (persistent between runs):

- a group type, **KFS E2E Eventbrite**, with an *Eventbrite Event* group attribute and an
  *Eventbrite Person* member attribute shown in the grid;
- a parent group, **KFS E2E Eventbrite Groups**, of that type.

Each run it also creates two hidden pages under *KFS E2E Test Pages*:

- a settings page holding the Eventbrite Settings block, with the settings installed on the
  site, pointed at the test group type and parent;
- a copy of Rock's Group Viewer page, which already holds the Eventbrite Sync Button.

Test groups are created under the parent and deleted before each test and after the run,
together with their members and attendance occurrences. People a sync creates for Eventbrite
attendees stay, like other test people.

**One-time setup** (the fixture checks each step and lists any that are missing):

1. In the site's Eventbrite organization, create an event named **KFS E2E Test Event** (or set
   `EVENTBRITE_TEST_EVENT_NAME`) with a free ticket. Publish it, and register at least one
   attendee at an address you control. Don't link it to a real Rock group.
2. Admin Tools > Settings > Group Types > **KFS E2E Eventbrite** > Edit: add *KFS E2E
   Eventbrite* under Child Group Types. The settings block only creates groups whose type is
   allowed as a child of the parent's type, and Rock's REST API cannot set that list.
3. On the same group type's Security, give **KFS E2E Plugin Testers** Allow rules for View and
   Edit. Group Detail only saves for people with Edit on the group.
4. On the same group type, under Group Attributes, click the lock icon on **Eventbrite Event** and
   give **KFS E2E Plugin Testers** an Allow rule for Edit. Group Detail only shows attributes the
   person may edit, and it checks each attribute's own security.

| Manual step | Test(s) |
|---|---|
| Settings page: Private Token and Organization set | `settingsPage › PageLoad_TokenAndOrganizationSet_ShowsAuthenticatedWithLinkedGroupsAndEvents` ("Authenticated", token present but redacted, organization, both panels, test event offered) |
| Choose the event, Create New Rock Group; the group is created and in Linked Groups | `settingsPage › CreateNewRockGroup_TestEventSelected_CreatesGroupLinkedToEvent` |
| Group of a type with the Eventbrite attributes; select the event under Group Attribute Values, save; Sync and Unlink buttons appear | `groupViewer › SaveGroup_EventbriteEventSelected_LinksGroupAndShowsEventbriteButtons` |
| Eventbrite Sync Attendees; members sync in with ticket details in the member grid | `groupViewer › SyncAttendees_LinkedGroup_AddsAttendeesWithTicketDetails` |
| Create the Eventbrite event | one-time setup (Eventbrite has no API for registering attendees) |
| *(beyond the manual plan)* | `groupViewer › Unlink_LinkedGroup_ClearsEventAndHidesButtons` |

**On Rock 20, `SaveGroup_EventbriteEventSelected_…` is expected to fail until the plugin is
fixed.** Rock 20 replaced the Group Detail block with an Obsidian block, and the *Eventbrite
Event* field type has no Obsidian edit control, so the group editor shows a plain text box
instead of a drop down of events. The test says so in its failure message. On Rock 17 it uses
the WebForms editor as before.

## Advanced Group Finder

`tests/Groups/GroupFinder/`: 12 tests for the *Group Finder KFS* block (KFS > Groups).

The manual plan says to set up a page with the block and exercise several elements. The fixture
creates a hidden test page under *KFS E2E Test Pages* with the settings of the block on the KFS
beta site's page 800. Those settings are:

- Auto Load and Auto Filter on;
- the Campus, Day of Week, Keyword and Zip Code filters;
- a group attribute filter;
- full groups hidden.

The page points the block at a test group type and uses a plain Lava template, so the results
can be read exactly.

**Test data:** the group type, **KFS E2E Group Finder**, is kept between runs; it has a
*Topic* single-select attribute (Bible Study, Prayer, Service). Each run creates four groups,
each with a weekly schedule, and deletes them afterwards:

| Group | Campus | Day | Topic | Other |
|---|---|---|---|---|
| Monday Bible Study | first active campus | Monday | Bible Study | |
| Wednesday Prayer | second active campus | Wednesday | Prayer | "potluck" only in its description |
| Full Group | first | Monday | Service | capacity 1, one member |
| Private Group | first | Monday | Service | not public |

The campus filter needs at least two active campuses on the site.

| Manual step | Test(s) |
|---|---|
| Page with the block; launch it | fixture, `filters › PageLoad_AutoLoad_ListsPublicGroupsWithRoom` (only the public groups with room, sorted by name) |
| Exercise several elements | `filters › Keyword_WordInDescription_…`, `Keyword_WordInName_…`, `Campus_SecondCampusChecked_…`, `DayOfWeek_MondayChecked_…`, `AttributeFilter_TopicPrayer_…`, `Filters_DayAndCampusThatDoNotMatch_ShowsNoGroups`, `UrlParameters_TopicAndDay_PrefillFiltersAndShowMatches` |
| *(beyond the manual plan: settings the KFS version adds or changes)* | `settings › ShowAllGroups_On_ListsNonPublicGroup`, `HideOvercapacityGroups_Off_ListsFullGroup`, `AutoFilterOff_SearchThenClear_FiltersOnlyOnSearchAndClearResets`, `SingleSelectFilters_On_CampusFilterIsDropDown` |

Not covered yet:
- postal-code proximity, which needs geocoded group locations;
- the map;
- keyword search across attributes;
- collapsible filters;
- custom attribute sort;
- PersonGuid mode;
- Group Opportunities.

No security setup is needed.

## Custom Group Reminder Emails

`tests/Communication/CustomGroupReminder/`: 3 tests for the *Send Custom Meeting Reminders*
job (job type "Custom Meeting Group Reminder"). **They send a real email and a real text** to
`CGR_TEST_EMAIL` / `CGR_TEST_SMS_NUMBER` (default `SGC_TEST_EMAIL` / `SGC_TEST_SMS_NUMBER`).

The job sends a reminder to the active members of every active group that has a schedule and
has the chosen Group Attribute set to Yes, for meetings exactly *Days Prior* days away. The
fixture keeps rockbeta's job 104 and real groups out of it.

Persistent, by fixed Guid:
- a test group type, **KFS E2E Meeting Reminder**, with its own Boolean **Group Meeting
  Reminder** group attribute;
- an **inactive** test job of that job type, whose Group Attribute is that test attribute. Only
  test groups can ever match;
- a test member, *Cgrmember KFS E2E Tester*, with the test email and an SMS-enabled mobile
  number.

Each run creates:
- two groups that meet weekly on tomorrow's weekday, using Rock's own date. One has Group
  Meeting Reminder = Yes and the other No; both have the test member;
- copies of Rock's **Jobs Administration** and **Scheduled Job Detail** pages under *KFS E2E
  Test Pages*. The test role has Edit there, which is all both blocks require; the list's
  detail link points at the detail copy.

All of it is deleted afterwards. The fixture sets the job's starting settings through the API
(the plan's setup step). **Editing the job and running it happen in the browser**, signed in
as Pafa:
1. Open Scheduled Job Detail.
2. Change Send Using in its drop-down, and Days Prior.
3. Save.
4. Open Jobs Administration and click the job's Run Now.

Each test then checks the result shown in the job list and in the job record, plus the
reminder in communication history: recipient, subject or text, and group name.

**What "Delivered" means here:** the job sends through a system communication. Rock hands the
message straight to the transport and writes the history record afterwards, marked Delivered.
For email that record names no transport and has no status note, so it does not confirm the
message reached Edify. The SMS record does name the Twilio transport. The Edify and phone
checks stay manual.

**One-time setup (done on rockbeta):** the plugin's *Group Meeting Reminder* system
communication must have an SMS From number. Rock's Twilio transport refuses texts without
one. The SMS test checks for it first.

| Manual step | Test(s) |
|---|---|
| Group with a predictable schedule and Group Meeting Reminder = Yes; job with Days Prior, Send Using Email, Group Attribute | fixture |
| Run job; history shows reminder emails sent | `reminderJob › Run_SendUsingEmailMeetingTomorrow_EmailsMembersOfReminderGroupOnly` (Run Now clicked in Jobs Administration; one reminder, to the Yes group's member only; names that group) |
| Open Job Detail, set Send Using to SMS; run; history shows communications sent | `reminderJob › EditSendUsingSmsAndRun_TextsMemberOfReminderGroup` (edited and run in the browser) |
| *(beyond the manual plan)* | `reminderJob › Run_DaysPriorNotMatchingMeeting_SendsNothing` (Days Prior edited to 2 in Job Detail) |
| Check Edify outgoing messages; check the text arrived | **manual**: each sending test adds a `manual-check` annotation |

## Microsoft 365 Utilities

`tests/Communication/Microsoft365Utilities/`: 8 tests for the *KFS - EWS Calendar Items* Lava
shortcode and the *Launch Workflow From EWS Account* job. Both sign in to Microsoft 365 with the
Azure app in the three EWS global attributes, and call Exchange Web Services.

**They run against the live KFS calendar@ mailbox** (`M365_MAILBOX`, `M365_IMPERSONATE` in
`.env`), so they touch as little as possible:

- **Shortcode** (`ewsCalendarShortcode`): renders Lava through Rock's own engine
  (`api/Lava/RenderTemplate`, as the Lava Tester does). Reading the calendar changes nothing,
  and every template prints at most one calendar item plus a count. Subjects are compared,
  never printed. The example is read from the shortcode's documentation in the plugin source,
  which is the text CMS Configuration > Lava Shortcodes shows. Before the compat fix the
  shortcode showed only "One or more errors occurred.", so a failure also gives the real
  Microsoft error from Rock's exception log.
- **Job** (`ewsJob`): one test, one email. The inactive test job runs with Max Emails 1 and
  no filter, so it processes only the newest inbox email. Before the run, the test reads that
  email's read and flag state straight from Exchange, and picks a marking that will visibly
  change it: Read if it is unread, otherwise Add Flag. After the run it finds the processed
  email by the workflow's ForeignKey (the email's Exchange id), checks the workflow and the
  marking, and then, in a `finally`, puts the original read and flag state back and re-reads
  the email to prove it. The test workflow holds a copy of the email, so it is deleted
  straight away. The job gets its credentials from the global attributes (copied encrypted).
  The test's own Exchange calls need the same app's ID, tenant and secret in plain text in
  `.env` (`M365_APP_ID`, `M365_TENANT_ID`, `M365_APP_SECRET`), because Rock cannot give them
  out decrypted. The secret and access token are masked in any error.

| Manual step | Test(s) |
|---|---|
| EWS Azure Application ID, Tenant ID and Secret are set | `ewsCalendarShortcode › GlobalAttributes_EwsCredentials_AreSetAndEncrypted` (set and encrypted; that they are *valid* is shown by the shortcode tests signing in) |
| Copy the shortcode example, set calendarmailbox and impersonate, run it | `ewsCalendarShortcode › Render_DocumentationExample_RendersWithoutErrors` |
| Verify data comes through | `ewsCalendarShortcode › Render_ThirtyDaysBackSixtyForward_ReturnsItemInWindow` |
| *(beyond the manual plan)* | `Render_OrderDesc_ReturnsLatestItemFirst`, `Render_NoCalendarMailbox_RendersNothing`, `Render_NoImpersonate_ReadsCalendarAsCalendarMailbox` (documented as optional: falls back to the calendar mailbox), `Render_InvalidSecret_ShowsWarningAndLogsMicrosoftSignInError` (also proves MSAL loads on this Rock version) |
| Configure a job with every setting; run it; history shows the right number; workflows created; email marked as configured | `ewsJob › RunJob_MaxEmailsOne_LaunchesOneWorkflowForNewestEmailAndMarksIt` (then restores the email) |
| Expired secret: create a new one in the Entra app registration | **manual**: the failure message names AADSTS7000222. Update the 'EWS Azure Secret' global attribute and `M365_APP_SECRET` |

## Steps to Care

`tests/Crm/StepsToCare/`: 18 tests for the Care Dashboard, Care Entry, Care Workers and Care
Note Templates blocks, and the Care Configuration page.

**Everything is done in the browser.** The plugin has no REST endpoints for care needs,
workers or note templates, so the tests also check their results on the plugin's own pages.

**The test pages are kept between runs.** These are copies of the installed Steps to Care,
Care Entry and Care Configuration pages, nested the same way (Care Entry returns to its parent
page after Save):

| Page | Id | Guid |
|---|---|---|
| KFS E2E Care Dashboard | 1003 | `9A0B4C33-E2E0-4C0E-8C0E-0000000000B1` |
| KFS E2E Care Entry | 1004 | `…B2` |
| KFS E2E Care Configuration | | `…B3` |

They're kept because the blocks' own security actions can only be granted on the block, by
hand. Their settings are rockbeta's, with auto-assignment off, so no real care worker is
assigned or notified.

The tests sign in with a token that works on every page, because they move between the three
pages.

**Test data:**
- *Stcparent KFS E2E Tester* (requestor) and *Stckid* (a child in the same family);
- *Stcworker* (added as a care worker);
- a *KFS E2E Care Category*;
- a connection type and opportunity, created each run.

**Cleanup:** before and after each test, needs whose description matches "KFS E2E … <13-digit
timestamp>" are deleted with the dashboard's Delete column, family needs first. This also
clears the tester's saved grid filter. Prayer requests, connection requests, configuration
items and the notification choice the tests create are removed or restored afterwards.

**One-time setup** (done on rockbeta), all for **KFS E2E Plugin Testers**:
1. Administrate on page KFS E2E Care Dashboard. This shows the Assigned panel in Care Entry and
   the dashboard's Delete column.
2. On the Care Dashboard block: CompleteNeeds, and ViewAll. Family members' needs get no
   worker, so only View All shows them under the "+".
3. On the Care Entry block: CompleteNeeds and UpdateStatus.

The fixture checks each rule.

| Manual step / documented scenario | Test(s) |
|---|---|
| Enter a Care Need; add yourself as Care Worker; Custom Follow Up days (scenarios 1, 6) | `careNeed › EnterCareNeed_SelfAsWorkerAndCustomFollowUp_ShowsAssignedNeedOnDashboard` |
| Include Family; "+" lists family members' needs (scenario 3) | `careNeed › EnterCareNeed_IncludeFamily_PlusShowsFamilyMembersNeeds` |
| Enter further needs for family members (edit a family need) | `careNeed › FamilyNeed_EditFromFamilyRow_SavesDetailsAndWorker` |
| Action button: each option | `actions › Actions_Menu_OffersEachConfiguredAction`, `Snooze_ThenReOpen_SwitchesBetweenSnoozedAndOpen` (scenario 6), `AddConnectionRequest_TestOpportunity_CreatesRequestWithNeedDetails` (scenario 1), `AddPrayerRequest_CreatesRequestAndOpensItsDetailPage`, `LaunchWorkflow_SendsNeedToWorkflowLauncher` (scenario 2), `ViewHistory_OpensHistoryPageForNeed` |
| Add Note; care touches (scenarios 1, 5) | `actions › QuickNote_Called_AddsCareTouch`, `MakeNote_QuickNoteWithText_SavesNoteShownInDialog` |
| Notification icon: None / Email / SMS / Email & SMS, value saved | `notification › NotificationType_AllFourOptions_ChoiceIsSaved` |
| Gear opens Care Configuration; "+" adds and clicking edits in each block | `configuration › Gear_OnDashboard_OpensCareConfiguration`, `CareWorkers_AddThenEdit_SavesWorker`, `NoteTemplates_AddThenEdit_SavesTemplate`, `Category_AddThenEdit_SavesDefinedValue`, `Status_AddThenEdit_SavesDefinedValue` |
| Complete Need removes it from the grid | `careNeed › CompleteNeed_FromActions_RemovesNeedFromDashboard` |

**Core Rock pages are not tested.** Add Prayer Request, Launch Workflow and View History send
the user to core Rock pages, which the test role cannot view. The tests check that the plugin
sends the browser to the right page with the right Id, and that it creates the prayer request.

**Not covered yet:** the time-driven scenarios (4: scheduled future needs; 5: touch-template
flags; 6: automatic follow-up and snooze). They depend on the Care Need Automated Processes job
and elapsed time; they would be job tests that set dates and run the job, like Scheduled Group
Communication.

Traces are off for this suite. The worker's shared REST client is created inside the first
test's trace recording and later fails writing to it (ENOENT), failing tests that passed.

## Round Robin Group Assignment

`tests/Groups/RoundRobinGroupAssignment/`: 4 tests for the *Round Robin Group Assignment*
job. There is no browser part: the job is run with Run Now, and the results are read through
REST.

**Kept between runs** (by fixed Guid):
- an **inactive** test job (rockbeta's job 123 is not touched);
- a data view, **KFS E2E Round Robin People**, built through REST: people whose last name is
  exactly "KFS E2E Rrtester";
- four test people with that last name, each in their own family;
- *Rrkid*, a child in the first person's family, and *Rroutsider*. Neither is in the data view.

**Each run:** a parent Small Group, *KFS E2E RR Parent*, with two child groups, A and B. It is
deleted after the run, and the child groups are emptied before each test.

| Manual step | Test(s) |
|---|---|
| Data view of people not yet in the target groups; job with "Groups to Assign People to" = a parent group and "People Data View" = that data view; run; history shows people assigned; each person is in one of the target groups | `roundRobin › Run_FourPeopleTwoChildGroups_AssignsEachPersonRoundRobin` ("4 people added to groups."; each person in exactly one child group; 2 and 2) |
| *(beyond the manual plan)* | `Run_PeopleAlreadyAssigned_AddsNoOne`, `Run_IncludeAllFamilyMembers_AddsFamilyToSameGroup` (the child joins the parent's group), `Run_RemoveMembersNotInDataView_RemovesOthersFromTargetGroups` |

No security setup is needed.

## Adding tests for another plugin

Never add tests for anything under a `ZZZ_Archive` folder (in `KFSRockAssemblies` or
`KFSRockBlocks`). Those plugins are retired and obsolete by KFS practice.

1. Create `tests/{Domain}/{Plugin}/` using the Rock domain the plugin belongs to.
2. Write a fixture that builds everything the tests need through `shared/testData` (add
   builders there when a new entity type is needed), and tears it down in `finally`.
   Name everything with `TestNamePrefix`, and delete leftovers by name before creating.
   Do not grant security through REST: on Rock 17.9 a REST-saved rule is ignored and security
   role membership cannot be changed. Give test people access once, by hand, through a
   test-only role (Elevated Security Level None, so token sign-in works), and check it in the
   fixture with `verifyRoleAccess` from `shared/testData/security.ts`.
3. Write a page object that selects controls by server ID suffix (`[id$='_tbEmail']`), which
   stays stable across Rock versions.
4. Turn each step of the manual test plan into a test, and record the mapping in this README.

## Test rules

These are Rock's, from `Rock.Tests/README.md`, applied here:

- **Naming:** `MethodOrComponentName_TestConditionOrScenario_ExpectedResult`.
- Every test has at least one assertion and tests one thing (multiple assertions about that
  thing are fine).
- Tests must not depend on the order they run in. The auto `resetState` fixture resets block
  settings and removes group members, requests and workflows before each test.
- Tests leave the site the way they found it. The fixture deletes what it created; test
  people are the one deliberate exception.
- Tests run one at a time (`workers: 1`) because they share one block instance.
