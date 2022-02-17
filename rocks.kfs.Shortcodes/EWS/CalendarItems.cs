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
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using DotLiquid;
using DotLiquid.Util;
using Microsoft.Exchange.WebServices.Data;
using Rock;
using Rock.Data;
using Rock.Lava.Shortcodes;
using Rock.Model;
using Rock.Security;

namespace rocks.kfs.Shortcodes.EWS
{
    /// <summary>
    /// Lava shortcode for displaying calendar items from a Microsoft Exchange mailbox.
    /// </summary>
    [LavaShortcodeMetadata(
        "KFS - EWS Calendar Items",
        "ewscalendaritems",
        "Retrieves a list of calendar items from a specified Microsoft Exchange shared calendar mailbox.",
        @"<p>The KFS - EWS calendar items shortcode allows you to access calendar items from a specified Microsoft Exchange shared mailbox using the EWS managed API. Below is an example of how to use it.
            </p>
            <pre>{% assign username = 'Global' | Attribute:'rocks.kfs.EWSUsername' %}
{% assign password = 'Global' | Attribute:'rocks.kfs.EWSPassword' %}
{[ ewscalendaritems username:'{{ username }}' password:'{{ password }}' calendarmailbox:'kfscalendar@kingdomfirstsolutions.com' ]}
    {% for calItem in CalendarItems %}
        {{ calItem.Subject }}
        {{ calItem.TextBody }}
        {{ calItem.Location }}
        {{ calItem.Start }}
        {{ calItem.End }}
    {% endfor %}
 {[ endewscalendaritems ]}</pre>
        <p>Below is a list of available parameters.</p>
        <ul>
            <li><strong>username</strong> REQUIRED - The username of an account on the target Exchange server that has access to the target mailbox. This value can be either encrypted or non-encrypted. It is recommended this value be stored as a global attribute of type EncryptedText and passed to the shortcode directly from the global attribute value for higher security.</li>
            <li><strong>password</strong> REQUIRED - The password of an account on the target Exchange server that has access to the target mailbox. This value can be either encrypted or non-encrypted. It is recommended this value be stored as a global attribute of type EncryptedText and passed to the shortcode directly from the global attribute value for higher security.</li>
            <li><strong>calendarmailbox</strong> REQUIRED - The address of the mailbox for the target calendar.</li>
            <li><strong>serverurl</strong> - The url for the Microsoft Exchange server. Default: https://outlook.office365.com/EWS/Exchange.asmx</li>
            <li><strong>order</strong> - An optional parameter to change the ordering of the returned items based on their Start value. By default items are ordered by Start ascending. Set value to 'desc' will cause the results to be orded by Start descending.</li>
            <li><strong>daysback</strong> (0)- The number of days to look back to find calendar items.</li>
            <li><strong>daysforward</strong> (7)- The number of days to look forward to find calendar items.</li>
        </ul>
        <p>The shortcode returns a liquid object named CalendarItems that contains a list of calendar items pulled from the Exchange server that meet the requested parameters.</p>
        <p>Each calendar item in the list has the following merge fields available.</p>
        <ul>
            <li><strong>Subject</strong> - The subject.</li>
            <li><strong>Body</strong> - The full body.</li>
            <li><strong>TextBody</strong> - The plain text representation of the body.</li>
            <li><strong>Location</strong> - The name of the location</li>
            <li><strong>Start</strong> - The start dateTime.</li>
            <li><strong>End</strong> - The end dateTime.</li>
            <li><strong>DisplayTo</strong> - A text summarization of the To recipients.</li>
            <li><strong>DisplayCc</strong> - A text summarization of the CC recipients.</li>
            <li><strong>IsRecurring</strong> - A boolean indicating if the calendar item is part of a recurring series.</li>
        </ul>",
        "username,password,calendarmailbox,serverurl,order,daysback,daysforward",
        "" )]
    public class EWSCalendarItems : RockLavaShortcodeBlockBase
    {
        private static readonly Regex Syntax = new Regex( @"(\w+)" );

        string _markup = string.Empty;
        string _enabledSecurityCommands = string.Empty;
        StringBuilder _blockMarkup = new StringBuilder();
        string _tagName = "ewscalendaritems";

        // Keys
        const string EWS_USERNAME = "username";
        const string EWS_PASSWORD = "password";
        const string SERVER_URL = "serverurl";
        const string CALENDAR_MAILBOX = "calendarmailbox";
        const string ORDER = "order";
        const string DAYS_BACK = "daysback";
        const string DAYS_FORWARD = "daysforward";

        /// <summary>
        /// Method that will be run at Rock startup
        /// </summary>
        public override void OnStartup()
        {
            Template.RegisterShortcode<EWSCalendarItems>( _tagName );
        }

        /// <summary>
        /// Initializes the specified tag name.
        /// </summary>
        /// <param name="tagName">Name of the tag.</param>
        /// <param name="markup">The markup.</param>
        /// <param name="tokens">The tokens.</param>
        /// <exception cref="System.Exception">Could not find the variable to place results in.</exception>
        public override void Initialize( string tagName, string markup, List<string> tokens )
        {
            _markup = markup;
            base.Initialize( tagName, markup, tokens );
        }

        /// <summary>
        /// Parses the specified tokens.
        /// </summary>
        /// <param name="tokens">The tokens.</param>
        protected override void Parse( List<string> tokens )
        {
            // Get the block markup. The list of tokens contains all of the lava from the start tag to
            // the end of the template. This will pull out just the internals of the block.

            // We must take into consideration nested tags of the same type

            var endTagFound = false;

            var startTag = $@"{{\[\s*{ _tagName }\s*\]}}";
            var endTag = $@"{{\[\s*end{ _tagName }\s*\]}}";

            var childTags = 0;

            Regex regExStart = new Regex( startTag );
            Regex regExEnd = new Regex( endTag );

            NodeList = NodeList ?? new List<object>();
            NodeList.Clear();

            string token;
            while ( ( token = tokens.Shift() ) != null )
            {

                Match startTagMatch = regExStart.Match( token );
                if ( startTagMatch.Success )
                {
                    childTags++; // increment the child tag counter
                    _blockMarkup.Append( token );
                }
                else
                {
                    Match endTagMatch = regExEnd.Match( token );

                    if ( endTagMatch.Success )
                    {
                        if ( childTags > 0 )
                        {
                            childTags--; // decrement the child tag counter
                            _blockMarkup.Append( token );
                        }
                        else
                        {
                            endTagFound = true;
                            break;
                        }
                    }
                    else
                    {
                        _blockMarkup.Append( token );
                    }
                }
            }

            if ( !endTagFound )
            {
                AssertMissingDelimitation();
            }
        }

        /// <summary>
        /// Renders the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="result">The result.</param>
        public override void Render( Context context, TextWriter result )
        {
            var rockContext = new RockContext();

            // Get enabled security commands
            if ( context.Registers.ContainsKey( "EnabledCommands" ) )
            {
                _enabledSecurityCommands = context.Registers["EnabledCommands"].ToString();
            }

            using ( TextWriter writer = new StringWriter() )
            {
                var now = RockDateTime.Now;

                base.Render( context, writer );

                var parms = ParseMarkup( _markup, context );
                string username;
                string password;
                var rawUsername = username = parms[EWS_USERNAME];
                var rawPassword = password = parms[EWS_PASSWORD];
                var decryptedUn = Encryption.DecryptString( rawUsername );
                var decryptedPw = Encryption.DecryptString( rawPassword );
                if ( !string.IsNullOrWhiteSpace( decryptedUn ) )
                {
                    username = decryptedUn;
                }
                if ( !string.IsNullOrWhiteSpace( decryptedPw ) )
                {
                    password = decryptedPw;
                }
                var order = parms[ORDER];
                var mailbox = parms[CALENDAR_MAILBOX];
                var url = parms[SERVER_URL];
                var daysBack = parms[DAYS_BACK].AsIntegerOrNull();
                var daysForward = parms[DAYS_FORWARD].AsIntegerOrNull();
                var startDate = now.Date;
                var endDate = now.Date.AddDays( 8 ).AddMilliseconds( -1 );
                if ( daysBack.HasValue )
                {
                    startDate = startDate.AddDays( -daysBack.Value );
                }
                if ( daysForward.HasValue )
                {
                    endDate = now.Date.AddDays( daysForward.Value + 1 ).AddMilliseconds( -1 );
                }

                if ( string.IsNullOrWhiteSpace( username ) || string.IsNullOrWhiteSpace( password ) || string.IsNullOrWhiteSpace( mailbox ) )
                {
                    return;
                }

                var calendarItems = new List<CalendarItemResult>();
                var service = new ExchangeService();

                try
                {
                    service.Credentials = new WebCredentials( username, password );
                    service.TraceEnabled = true;
                    service.TraceFlags = TraceFlags.All;
                    service.Url = new Uri( url );

                    var folderId = new FolderId( WellKnownFolderName.Calendar, mailbox );
                    CalendarView calView = new CalendarView( startDate, endDate );
                    calView.PropertySet = new PropertySet( PropertySet.IdOnly );
                    var addlPropSet = new PropertySet(
                                                                AppointmentSchema.Subject,
                                                                AppointmentSchema.Start,
                                                                AppointmentSchema.End,
                                                                AppointmentSchema.Location,
                                                                AppointmentSchema.Body,
                                                                AppointmentSchema.TextBody,
                                                                AppointmentSchema.IsRecurring,
                                                                AppointmentSchema.AppointmentType,
                                                                AppointmentSchema.DisplayTo,
                                                                AppointmentSchema.DisplayCc
                                                            );
                    var appointments = service.FindAppointments( folderId, calView );
                    if ( appointments.Items.Count > 0 )
                    {
                        service.LoadPropertiesForItems( appointments.Items, addlPropSet );
                        calendarItems.AddRange( appointments.Where( a => a.AppointmentType != AppointmentType.RecurringMaster )
                                                            .OrderBy( a => a.Start )
                                                            .Select( a => new CalendarItemResult
                                                            {
                                                                Subject = a.Subject,
                                                                Body = a.Body,
                                                                TextBody = a.TextBody,
                                                                Start = a.Start,
                                                                End = a.End,
                                                                Location = a.Location,
                                                                DisplayTo = a.DisplayTo,
                                                                DisplayCc = a.DisplayCc,
                                                                IsRecurring = a.IsRecurring
                                                            } ) );
                    }
                }
                catch ( Exception ex )
                {
                    return;
                }
                if ( calendarItems.Count() == 0 )
                {
                    return;
                }

                if ( order.ToLower() == "desc" )
                {
                    calendarItems = calendarItems.OrderByDescending( ci => ci.Start ).ToList();
                }

                var mergeFields = LoadBlockMergeFields( context );
                mergeFields.Add( "CalendarItems", calendarItems );

                var results = _blockMarkup.ToString().ResolveMergeFields( mergeFields, _enabledSecurityCommands );
                result.Write( results.Trim() );
                base.Render( context, result );
            }
        }

        /// <summary>
        /// Parses the markup.
        /// </summary>
        /// <param name="markup">The markup.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        private Dictionary<string, string> ParseMarkup( string markup, Context context )
        {
            // first run lava across the inputted markup
            var internalMergeFields = new Dictionary<string, object>();

            // get variables defined in the lava source
            foreach ( var scope in context.Scopes )
            {
                foreach ( var item in scope )
                {
                    internalMergeFields.AddOrReplace( item.Key, item.Value );
                }
            }

            // get merge fields loaded by the block or container
            if ( context.Environments.Count > 0 )
            {
                foreach ( var item in context.Environments[0] )
                {
                    internalMergeFields.AddOrReplace( item.Key, item.Value );
                }
            }
            var resolvedMarkup = markup.ResolveMergeFields( internalMergeFields );

            var parms = new Dictionary<string, string>();
            parms.Add( EWS_USERNAME, "" );
            parms.Add( EWS_PASSWORD, "" );
            parms.Add( SERVER_URL, "https://outlook.office365.com/EWS/Exchange.asmx" );
            parms.Add( CALENDAR_MAILBOX, "" );
            parms.Add( ORDER, "" );
            parms.Add( DAYS_BACK, "0" );
            parms.Add( DAYS_FORWARD, "7" );

            var markupItems = Regex.Matches( resolvedMarkup, @"(\S*?:'[^']+')" )
                .Cast<Match>()
                .Select( m => m.Value )
                .ToList();

            foreach ( var item in markupItems )
            {
                var itemParts = item.ToString().Split( new char[] { ':' }, 2 );
                if ( itemParts.Length > 1 )
                {
                    parms.AddOrReplace( itemParts[0].Trim().ToLower(), itemParts[1].Trim().Substring( 1, itemParts[1].Length - 2 ) );
                }
            }
            return parms;
        }


        /// <summary>
        /// Loads the block merge fields.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        private Dictionary<string, object> LoadBlockMergeFields( Context context )
        {
            var _internalMergeFields = new Dictionary<string, object>();

            // Get merge fields loaded by the block or container
            if ( context.Environments.Count > 0 )
            {
                foreach ( var item in context.Environments[0] )
                {
                    _internalMergeFields.AddOrReplace( item.Key, item.Value );
                }
            }

            return _internalMergeFields;
        }

        private class CalendarItemResult : Model<CalendarItemResult>
        {
            [LavaInclude]
            public string Subject { get; set; }

            [LavaInclude]
            public string Body { get; set; }

            [LavaInclude]
            public string TextBody { get; set; }

            [LavaInclude]
            public DateTime Start { get; set; }

            [LavaInclude]
            public DateTime End { get; set; }

            [LavaInclude]
            public string Location { get; set; }

            [LavaInclude]
            public string DisplayTo { get; set; }

            [LavaInclude]
            public string DisplayCc { get; set; }

            [LavaInclude]
            public bool IsRecurring { get; set; }
        }
    }
}
