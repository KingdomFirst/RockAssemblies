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

namespace rocks.kfs.CustomGroupCommunication.Migrations
{
    [MigrationNumber( 1, "1.12.4" )]
    public partial class CreateCommunications : Migration
    {
        public override void Up()
        {
            RockMigrationHelper.UpdateSystemCommunication( "Plugins"
                , "Group Meeting Reminder", "", "", "", "", ""
                , "You have group meeting(s) starting soon", @"{{ 'Global' | Attribute:'EmailHeader' }}
                    <p>{{ Person.FullName }},</p>
                    <p>You have a scheduled group meeting coming up:</p>
                    <blockquote><b>Meeting for {{ Group.Name }}</b><br>
                    <b>Date(s):</b> {{ NextMeetingDates }}</blockquote>
                    {{ 'Global' | Attribute:'EmailFooter' }}"
                , Guid.SystemComunication.CUSTOM_GROUP_MEETING_REMINDER
                , isActive: true
                , smsMessage: @"Upcoming group meeting(s) for {{ Group.Name }} ( {{ NextMeetingDates }} )."
                , pushTitle: "Upcoming Group Meeting"
                , pushMessage: @"Upcoming group meeting(s) for {{ Group.Name }} ( {{ NextMeetingDates }} )." );

            Sql( string.Format( "UPDATE [SystemCommunication] SET [IsSystem] = 0 WHERE [Guid] = '{0}'", Guid.SystemComunication.CUSTOM_GROUP_MEETING_REMINDER ) );
        }

        /// <summary>
        /// Operations to be performed during the downgrade process.
        /// </summary>
        public override void Down()
        {
            RockMigrationHelper.DeleteSystemCommunication( Guid.SystemComunication.CUSTOM_GROUP_MEETING_REMINDER );
        }
    }
}