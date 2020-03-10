// <copyright>
// Copyright by the Central Christian Church
//
// Licensed under the Rock Community License (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.rockrms.com/license
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//
//
// <notice>
// This file contains modifications by Kingdom First Solutions
// and is a derivative work.
//
// Modification (including but not limited to):
// * Added Ministry and Contact Info to available lava fields.
// * Rotated document for landscape view
// </notice>
//
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using com.centralaz.RoomManagement.Model;
using com.centralaz.RoomManagement.ReportTemplates;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using Rock;
using Rock.Data;
using Rock.Model;

namespace rocks.kfs.RoomManagement.ReportTemplates
{
    /// <summary>
    /// 
    /// </summary>
    [System.ComponentModel.Description( "The lava landscape report template" )]
    [Export( typeof( ReportTemplate ) )]
    [ExportMetadata( "ComponentName", "Lava Landscape" )]
    public class LavaLandscapeReportTemplate : ReportTemplate
    {

        /// <summary>
        /// Gets or sets the exceptions.
        /// </summary>
        /// <value>
        /// The exceptions.
        /// </value>
        public override List<Exception> Exceptions { get; set; }

        /// <summary>
        /// Creates the document.
        /// </summary>
        /// <param name="mergeTemplate">The merge template.</param>
        /// <param name="mergeObjectList">The merge object list.</param>
        /// <param name="globalMergeFields">The global merge fields.</param>
        /// <returns></returns>
        public override byte[] GenerateReport( List<ReservationService.ReservationSummary> reservationSummaryList, string logoFileUrl, string font, DateTime? filterStartDate, DateTime? filterEndDate, string lavaTemplate = "" )
        {
            Font zapfdingbats = new Font( Font.ZAPFDINGBATS );

            // Date Ranges
            var today = RockDateTime.Today;
            var filterStartDateTime = filterStartDate.HasValue ? filterStartDate.Value : today;
            var filterEndDateTime = filterEndDate.HasValue ? filterEndDate.Value : today.AddMonths( 1 );

            // Build the Lava html
            var reservationSummaries = reservationSummaryList.Select( r => new
            {
                Id = r.Id,
                ReservationName = r.ReservationName,
                ReservationType = r.ReservationType,
                ApprovalState = r.ApprovalState.ConvertToString(),
                Locations = r.ReservationLocations.ToList(),
                Resources = r.ReservationResources.ToList(),
                CalendarDate = r.EventStartDateTime.ToLongDateString(),
                EventStartDateTime = r.EventStartDateTime,
                EventEndDateTime = r.EventEndDateTime,
                ReservationStartDateTime = r.ReservationStartDateTime,
                ReservationEndDateTime = r.ReservationEndDateTime,
                EventDateTimeDescription = r.EventTimeDescription,
                ReservationDateTimeDescription = r.ReservationTimeDescription,
                Ministry = r.ReservationMinistry,
                ContactInfo = String.Format( "{0} {1}", r.EventContactPersonAlias.Person.FullName, r.EventContactPhoneNumber ),
                SetupPhotoId = r.SetupPhotoId,
                Note = r.Note
            } )
            .OrderBy( r => r.EventStartDateTime )
            .GroupBy( r => r.EventStartDateTime.Date )
            .Select( r => r.ToList() )
            .ToList();

            var mergeFields = new Dictionary<string, object>();
            mergeFields.Add( "ReservationSummaries", reservationSummaries );
            mergeFields.Add( "FilterStartDate", filterStartDateTime );
            mergeFields.Add( "FilterEndDate", filterEndDateTime );
            mergeFields.Add( "ImageUrl", logoFileUrl.EncodeHtml() );
            mergeFields.Add( "ReportFont", font );
            mergeFields.Add( "CheckMark", new Phrase( "\u0034", zapfdingbats ).ToString() );

            string mergeHtml = lavaTemplate.ResolveMergeFields( mergeFields );

            //Setup the document
            StringReader stringReader = new StringReader( mergeHtml );
            var document = new Document( PageSize.A4.Rotate(), 25, 25, 25, 25 );
            HTMLWorker htmlWorker = new HTMLWorker( document );

            using ( var outputStream = new MemoryStream() )
            {
                var writer = PdfWriter.GetInstance( document, outputStream );

                document.Open();

                htmlWorker.Parse( stringReader );

                document.Close();

                return outputStream.ToArray();
            }
        }
    }
}
