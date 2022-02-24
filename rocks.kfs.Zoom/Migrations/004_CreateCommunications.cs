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
using Rock.Model;
using Rock.Plugin;

namespace rocks.kfs.Zoom.Migrations
{
    [MigrationNumber( 4, "1.12.4" )]
    public partial class CreateCommunications : Migration
    {
        public override void Up()
        {
            RockMigrationHelper.UpdateSystemCommunication( "Plugins"
                , "Zoom Meeting Reminder", "", "", "", "", ""
                , "You have a Zoom meeting starting soon", @"{{ 'Global' | Attribute:'EmailHeader' }}
                    <p>{{ Person.FullName }},</p>
                    <p>You have a scheduled Zoom meeting soon:</p>
                    <blockquote><b>Meeting for {{ Group.Name }}</b><br>
                    <b>Date:</b> {{ Occurrence.StartTime }}<br>
                    <b>Topic:</b> {{ Occurrence.Topic }}<br>
                    <b>Meeting Link:</b> <a href='{{ Occurrence.ZoomMeetingJoinUrl }}'>Join Meeting</a> <br>
                    <b>Zoom Meeting Id:</b> {{ Occurrence.ZoomMeetingId }}</blockquote>
                    {{ 'Global' | Attribute:'EmailFooter' }}"
                , ZoomGuid.SystemComunication.ZOOM_MEETING_REMINDER
                , isActive: true
                , smsMessage: @"Upcoming Zoom meeting for {{ Group.Name }} ( {{ Occurrence.StartTime }} ). {{ Occurrence.ZoomMeetingJoinUrl }}"
                , pushTitle: "Upcoming Zoom Meeting"
                , pushMessage: @"Upcoming Zoom meeting for {{ Group.Name }} ( {{ Occurrence.StartTime }} ). {{ Occurrence.ZoomMeetingJoinUrl }}" );
        }

        /// <summary>
        /// Operations to be performed during the downgrade process.
        /// </summary>
        public override void Down()
        {
            RockMigrationHelper.DeleteSystemCommunication( ZoomGuid.SystemComunication.ZOOM_MEETING_REMINDER );
        }
    }
}