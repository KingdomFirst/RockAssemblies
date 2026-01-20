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
// * Added new section to repeat through resource questions
// </notice>
//
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using com.bemaservices.RoomManagement.Attribute;
using com.bemaservices.RoomManagement.Model;
using com.bemaservices.RoomManagement.ReportTemplates;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Rock;

namespace rocks.kfs.RoomManagement.ReportTemplates
{
    /// <summary>
    ///
    /// </summary>
    [System.ComponentModel.Description( "Resource question report template" )]
    [Export( typeof( ReportTemplate ) )]
    [ExportMetadata( "ComponentName", "Resource Questions" )]
    public class ResourceQuestionReportTemplate : ReportTemplate
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
        /// <param name="reservationSummaryList"></param>
        /// <param name="logoFileUrl"></param>
        /// <param name="font"></param>
        /// <param name="filterStartDate"></param>
        /// <param name="filterEndDate"></param>
        /// <returns></returns>
        public override byte[] GenerateReport( List<ReservationService.ReservationSummary> reservationSummaryList, string logoFileUrl, string font, DateTime? filterStartDate, DateTime? filterEndDate, string lavaTemplate = "" )
        {
            //Fonts
            var titleFont = FontFactory.GetFont( font, 16, Font.BOLD );
            var listHeaderFont = FontFactory.GetFont( font, 12, Font.BOLD, Color.DARK_GRAY );
            var listSubHeaderFont = FontFactory.GetFont( font, 10, Font.BOLD, Color.DARK_GRAY );
            var listItemFontBold = FontFactory.GetFont( font, 8, Font.BOLD );
            var listItemFontNormal = FontFactory.GetFont( font, 8, Font.NORMAL );
            var listItemFontUnapproved = FontFactory.GetFont( font, 8, Font.ITALIC, Color.MAGENTA );
            var noteFont = FontFactory.GetFont( font, 8, Font.NORMAL, Color.GRAY );

            // Bind to Grid
            var reservationSummaries = reservationSummaryList.Select( r => new
            {
                Id = r.Id,
                ReservationName = r.ReservationName,
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

            //Setup the document
            var document = new Document( PageSize.LETTER.Rotate(), 25, 25, 25, 50 );

            var outputStream = new MemoryStream();
            var writer = PdfWriter.GetInstance( document, outputStream );

            // Our custom Header and Footer is done using Event Handler
            ResourceQuestionHeaderFooter PageEventHandler = new ResourceQuestionHeaderFooter();
            writer.PageEvent = PageEventHandler;

            // Define the page header
            PageEventHandler.HeaderFont = listHeaderFont;
            PageEventHandler.SubHeaderFont = listSubHeaderFont;
            PageEventHandler.HeaderLeft = "Group";
            PageEventHandler.HeaderRight = "1";
            document.Open();

            // Add logo
            try
            {
                iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance( logoFileUrl );

                logo.Alignment = iTextSharp.text.Image.RIGHT_ALIGN;
                logo.ScaleToFit( 300, 55 );
                document.Add( logo );
            }
            catch { }

            // Write the document
            var today = RockDateTime.Today;
            var filterStartDateTime = filterStartDate.HasValue ? filterStartDate.Value : today;
            var filterEndDateTime = filterEndDate.HasValue ? filterEndDate.Value : today.AddMonths( 1 );
            String title = String.Format( "Reservations for: {0} - {1}", filterStartDateTime.ToString( "MMMM d" ), filterEndDateTime.ToString( "MMMM d" ) );
            document.Add( new Paragraph( title, titleFont ) );

            Font zapfdingbats = new Font( Font.ZAPFDINGBATS );

            // Populate the Lists
            foreach ( var reservationDay in reservationSummaries )
            {
                var firstReservation = reservationDay.FirstOrDefault();
                if ( firstReservation != null )
                {
                    //Build Header
                    document.Add( Chunk.NEWLINE );
                    String listHeader = PageEventHandler.CalendarDate = firstReservation.CalendarDate;
                    document.Add( new Paragraph( listHeader, listHeaderFont ) );

                    //Build Subheaders
                    var listSubHeaderTable = new PdfPTable( 8 );
                    listSubHeaderTable.LockedWidth = true;
                    listSubHeaderTable.TotalWidth = PageSize.LETTER.Rotate().Width - document.LeftMargin - document.RightMargin;
                    listSubHeaderTable.HorizontalAlignment = 0;
                    listSubHeaderTable.SpacingBefore = 10;
                    listSubHeaderTable.SpacingAfter = 0;
                    listSubHeaderTable.DefaultCell.BorderWidth = 0;
                    listSubHeaderTable.DefaultCell.BorderWidthBottom = 1;
                    listSubHeaderTable.DefaultCell.BorderColorBottom = Color.DARK_GRAY;

                    listSubHeaderTable.AddCell( new Phrase( "Name", listSubHeaderFont ) );
                    listSubHeaderTable.AddCell( new Phrase( "Event Time", listSubHeaderFont ) );
                    listSubHeaderTable.AddCell( new Phrase( "Reservation Time", listSubHeaderFont ) );
                    listSubHeaderTable.AddCell( new Phrase( "Locations", listSubHeaderFont ) );
                    listSubHeaderTable.AddCell( new Phrase( "Resources", listSubHeaderFont ) );
                    listSubHeaderTable.AddCell( new Phrase( "Has Layout", listSubHeaderFont ) );
                    listSubHeaderTable.AddCell( new Phrase( "Status", listSubHeaderFont ) );
                    listSubHeaderTable.AddCell( new Phrase( "Contact Info", listSubHeaderFont ) );
                    PageEventHandler.IsHeaderShown = true;
                    document.Add( listSubHeaderTable );

                    foreach ( var reservationSummary in reservationDay )
                    {
                        if ( reservationSummary == reservationDay.Last() )
                        {
                            PageEventHandler.IsHeaderShown = false;
                        }
                        var keepTogetherEvent = new PdfPTable( 1 );
                        keepTogetherEvent.LockedWidth = true;
                        keepTogetherEvent.TotalWidth = PageSize.LETTER.Rotate().Width - document.LeftMargin - document.RightMargin;
                        keepTogetherEvent.KeepTogether = true;
                        keepTogetherEvent.HorizontalAlignment = 0;
                        keepTogetherEvent.SpacingBefore = 0;
                        keepTogetherEvent.SpacingAfter = 1;
                        keepTogetherEvent.DefaultCell.BorderWidth = 0;

                        //Build the list item table
                        var listItemTable = new PdfPTable( 8 );
                        listItemTable.LockedWidth = true;
                        listItemTable.TotalWidth = PageSize.LETTER.Rotate().Width - document.LeftMargin - document.RightMargin;
                        listItemTable.HorizontalAlignment = 0;
                        listItemTable.SpacingBefore = 0;
                        listItemTable.SpacingAfter = 1;
                        listItemTable.DefaultCell.BorderWidth = 0;
                        //if ( string.IsNullOrWhiteSpace( reservationSummary.Note ) ||
                        //    reservationLocation != reservationSummary.Locations.First() )
                        //{
                        //    listItemTable.DefaultCell.BorderWidthBottom = 1;
                        //    listItemTable.DefaultCell.BorderColorBottom = Color.DARK_GRAY;
                        //}

                        //Add the list items
                        listItemTable.AddCell( new Phrase( reservationSummary.ReservationName, listItemFontNormal ) );

                        listItemTable.AddCell( new Phrase( reservationSummary.EventDateTimeDescription, listItemFontNormal ) );
                        listItemTable.AddCell( new Phrase( reservationSummary.ReservationDateTimeDescription, listItemFontNormal ) );

                        List locationList = new List( List.UNORDERED, 8f );
                        locationList.SetListSymbol( "\u2022" );
                        foreach ( var reservationLocation in reservationSummary.Locations )
                        {
                            var listItem = new iTextSharp.text.ListItem( reservationLocation.Location.Name, listItemFontNormal );
                            if ( reservationLocation.ApprovalState == ReservationLocationApprovalState.Approved )
                            {
                                listItem.Add( new Phrase( "\u0034", zapfdingbats ) );
                            }
                            locationList.Add( listItem );
                        }

                        PdfPCell locationCell = new PdfPCell();
                        locationCell.Border = 0;
                        locationCell.PaddingTop = -2;
                        locationCell.PaddingBottom = 2;
                        locationCell.AddElement( locationList );
                        locationCell.BorderWidth = 0;
                        //if ( string.IsNullOrWhiteSpace( reservationSummary.Note ) ||
                        //    reservationLocation != reservationSummary.Locations.First() )
                        //{
                        //    locationCell.BorderWidthBottom = 1;
                        //    locationCell.BorderColorBottom = Color.DARK_GRAY;
                        //}
                        listItemTable.AddCell( locationCell );

                        List resourceList = new List( List.UNORDERED, 8f );
                        resourceList.SetListSymbol( "\u2022" );
                        foreach ( var reservationResource in reservationSummary.Resources )
                        {
                            var resourceListItem = new iTextSharp.text.ListItem( string.Format( "{0} ({1})", reservationResource.Resource.Name, reservationResource.Quantity ), listItemFontNormal );
                            resourceList.Add( resourceListItem );
                        }

                        PdfPCell resourceCell = new PdfPCell();
                        resourceCell.Border = 0;
                        resourceCell.PaddingTop = -2;
                        resourceCell.PaddingBottom = 2;
                        resourceCell.AddElement( resourceList );
                        resourceCell.BorderWidth = 0;
                        // resources
                        listItemTable.AddCell( resourceCell );

                        // Has Layout
                        listItemTable.AddCell( new Phrase( ( reservationSummary.SetupPhotoId.HasValue ) ? "Yes" : "No", listItemFontNormal ) );

                        // approval status
                        listItemTable.AddCell( new Phrase( reservationSummary.ApprovalState, listItemFontNormal ) );

                        PdfPCell contactCell = new PdfPCell();
                        contactCell.Border = 0;
                        contactCell.PaddingTop = -2;
                        contactCell.PaddingBottom = 2;
                        contactCell.AddElement( new Phrase( reservationSummary.Ministry != null ? reservationSummary.Ministry.Name : string.Empty, listItemFontNormal ) );
                        contactCell.AddElement( new Phrase( reservationSummary.ContactInfo, listItemFontNormal ) );
                        contactCell.BorderWidth = 0;
                        //if ( string.IsNullOrWhiteSpace( reservationSummary.Note ) ||
                        //    reservationLocation != reservationSummary.Locations.First() )
                        //{
                        //    contactCell.BorderWidthBottom = 1;
                        //    contactCell.BorderColorBottom = Color.DARK_GRAY;
                        //}
                        listItemTable.AddCell( contactCell );

                        //document.Add( listItemTable );

                        PdfPCell eventRow = new PdfPCell();
                        eventRow.Border = 0;
                        eventRow.AddElement( listItemTable );
                        keepTogetherEvent.AddCell( eventRow );

                        if ( !string.IsNullOrWhiteSpace( reservationSummary.Note ) )
                        {
                            //document.Add( Chunk.NEWLINE );
                            var listNoteTable = new PdfPTable( 8 );
                            listNoteTable.LockedWidth = true;
                            listNoteTable.TotalWidth = PageSize.LETTER.Rotate().Width - document.LeftMargin - document.RightMargin;
                            listNoteTable.HorizontalAlignment = 1;
                            listNoteTable.SpacingBefore = 0;
                            listNoteTable.SpacingAfter = 1;
                            listNoteTable.DefaultCell.BorderWidth = 0;
                            //listNoteTable.DefaultCell.BorderWidthBottom = 1;
                            //listNoteTable.DefaultCell.BorderColorBottom = Color.DARK_GRAY;
                            listNoteTable.AddCell( new Phrase( string.Empty, noteFont ) );
                            var noteCell = new PdfPCell( new Phrase( reservationSummary.Note, noteFont ) );
                            noteCell.Border = 0;
                            noteCell.Colspan = 7;
                            listNoteTable.AddCell( noteCell );

                            //document.Add( listNoteTable );

                            PdfPCell noteRow = new PdfPCell();
                            noteRow.Border = 0;
                            noteRow.AddElement( listNoteTable );
                            keepTogetherEvent.AddCell( noteRow );
                        }

                        if ( reservationSummary.Resources.Any() )
                        {
                            var listResourceTable = new PdfPTable( 8 );
                            listResourceTable.LockedWidth = true;
                            listResourceTable.TotalWidth = PageSize.LETTER.Rotate().Width - document.LeftMargin - document.RightMargin;
                            listResourceTable.HorizontalAlignment = 1;
                            listResourceTable.SpacingBefore = 0;
                            listResourceTable.SpacingAfter = 1;
                            listResourceTable.DefaultCell.BorderWidth = 0;
                            listResourceTable.DefaultCell.BorderWidthBottom = 1;
                            listResourceTable.DefaultCell.BorderColorBottom = Color.DARK_GRAY;
                            listResourceTable.KeepTogether = true;

                            var addResourceTable = false;
                            foreach ( var reservationResource in reservationSummary.Resources )
                            {
                                reservationResource.LoadReservationResourceAttributes();

                                if ( reservationResource.Attributes != null && reservationResource.Attributes.Any() )
                                {
                                    var headerCell = new PdfPCell( new Phrase( string.Format( "{0} ({1})", reservationResource.Resource.Name, reservationResource.Quantity ), listSubHeaderFont ) );
                                    headerCell.Border = 0;
                                    headerCell.Colspan = 8;
                                    listResourceTable.AddCell( headerCell );

                                    var columnCount = 0;

                                    foreach ( var attributeDict in reservationResource.Attributes )
                                    {
                                        var resourceTableCell = new PdfPCell();
                                        resourceTableCell.Border = 0;
                                        resourceTableCell.Colspan = 2;

                                        var attribute = attributeDict.Value;
                                        var value = reservationResource.GetAttributeValue( attribute.Key );
                                        var formattedValue = attribute.FieldType.Field.FormatValue( null, attribute.EntityTypeId, reservationResource.Id, value, attribute.QualifierValues, true );

                                        var resourceListItem = new Phrase( attribute.Name, listItemFontBold );
                                        var resourceListItemValue = new Phrase( formattedValue, listItemFontNormal );

                                        if ( !string.IsNullOrWhiteSpace( value ) )
                                        {
                                            if ( columnCount == 8 )
                                            {
                                                columnCount = 0;
                                            }
                                            columnCount += 2;
                                            addResourceTable = true;
                                            resourceTableCell.AddElement( resourceListItem );
                                            resourceTableCell.AddElement( resourceListItemValue );
                                            listResourceTable.AddCell( resourceTableCell );
                                        }
                                    }
                                    if ( columnCount > 0 && columnCount < 8 )
                                    {
                                        PdfPCell columnFillCell = new PdfPCell( new Phrase( Chunk.NEWLINE ) );
                                        columnFillCell.Border = PdfPCell.NO_BORDER;
                                        columnFillCell.Colspan = 8 - columnCount;
                                        columnFillCell.FixedHeight = 15;

                                        listResourceTable.AddCell( columnFillCell );
                                    }

                                    //For blank line
                                    PdfPCell blankCell = new PdfPCell( new Phrase( Chunk.NEWLINE ) );
                                    blankCell.Border = PdfPCell.NO_BORDER;
                                    blankCell.Colspan = 8;
                                    blankCell.FixedHeight = 15;

                                    listResourceTable.AddCell( blankCell );
                                }
                            }

                            if ( addResourceTable )
                            {
                                PdfPCell borderCell = new PdfPCell( new Phrase( Chunk.NEWLINE ) );
                                borderCell.Colspan = 8;
                                borderCell.FixedHeight = 5;
                                borderCell.BorderWidth = 0;
                                borderCell.BorderWidthBottom = 1;
                                borderCell.BorderColorBottom = Color.DARK_GRAY;

                                listResourceTable.AddCell( borderCell );

                                //document.Add( listResourceTable );

                                PdfPCell resourceRow = new PdfPCell();
                                resourceRow.Border = 0;
                                resourceRow.AddElement( listResourceTable );
                                keepTogetherEvent.AddCell( resourceRow );
                            }
                        }
                        document.Add( keepTogetherEvent );
                    }
                }
                document.NewPage();
            }

            document.Close();

            return outputStream.ToArray();
        }
    }
}

public class ResourceQuestionHeaderFooter : PdfPageEventHelper
{
    // This is the contentbyte object of the writer
    private PdfContentByte cb;

    // we will put the final number of pages in a template
    private PdfTemplate template;

    // this is the BaseFont we are going to use for the header / footer
    private BaseFont bf = null;

    // This keeps track of the creation time
    private DateTime PrintTime = DateTime.Now;

    #region Properties

    private string _Title;

    public string Title
    {
        get { return _Title; }
        set { _Title = value; }
    }

    private string _CalendarDate;

    public string CalendarDate
    {
        get { return _CalendarDate; }
        set { _CalendarDate = value; }
    }

    private string _HeaderLeft;

    public string HeaderLeft
    {
        get { return _HeaderLeft; }
        set { _HeaderLeft = value; }
    }

    private string _HeaderRight;

    public string HeaderRight
    {
        get { return _HeaderRight; }
        set { _HeaderRight = value; }
    }

    private Font _HeaderFont;

    public Font HeaderFont
    {
        get { return _HeaderFont; }
        set { _HeaderFont = value; }
    }

    private Font _SubHeaderFont;

    public Font SubHeaderFont
    {
        get { return _SubHeaderFont; }
        set { _SubHeaderFont = value; }
    }

    private bool _IsHeaderShown;

    public bool IsHeaderShown
    {
        get { return _IsHeaderShown; }
        set { _IsHeaderShown = value; }
    }

    #endregion Properties

    // we override the onOpenDocument method
    public override void OnOpenDocument( PdfWriter writer, Document document )
    {
        try
        {
            PrintTime = DateTime.Now;
            bf = BaseFont.CreateFont( BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED );
            cb = writer.DirectContent;
            template = cb.CreateTemplate( 50, 50 );
        }
        catch
        {
            // not implemented
        }
    }

    public override void OnStartPage( PdfWriter writer, Document document )
    {
        base.OnStartPage( writer, document );
        int pageN = writer.PageNumber;
        if ( pageN > 1 && IsHeaderShown )
        {
            document.Add( new Paragraph( CalendarDate, HeaderFont ) );

            //Build Subheaders
            var listSubHeaderTable = new PdfPTable( 8 );
            listSubHeaderTable.LockedWidth = true;
            listSubHeaderTable.TotalWidth = PageSize.LETTER.Rotate().Width - document.LeftMargin - document.RightMargin;
            listSubHeaderTable.HorizontalAlignment = 0;
            listSubHeaderTable.SpacingBefore = 10;
            listSubHeaderTable.SpacingAfter = 0;
            listSubHeaderTable.DefaultCell.BorderWidth = 0;
            listSubHeaderTable.DefaultCell.BorderWidthBottom = 1;
            listSubHeaderTable.DefaultCell.BorderColorBottom = Color.DARK_GRAY;

            listSubHeaderTable.AddCell( new Phrase( "Name", SubHeaderFont ) );
            listSubHeaderTable.AddCell( new Phrase( "Event Time", SubHeaderFont ) );
            listSubHeaderTable.AddCell( new Phrase( "Reservation Time", SubHeaderFont ) );
            listSubHeaderTable.AddCell( new Phrase( "Locations", SubHeaderFont ) );
            listSubHeaderTable.AddCell( new Phrase( "Resources", SubHeaderFont ) );
            listSubHeaderTable.AddCell( new Phrase( "Has Layout", SubHeaderFont ) );
            listSubHeaderTable.AddCell( new Phrase( "Status", SubHeaderFont ) );
            listSubHeaderTable.AddCell( new Phrase( "Contact Info", SubHeaderFont ) );

            document.Add( listSubHeaderTable );
        }
    }

    public override void OnEndPage( PdfWriter writer, Document document )
    {
        base.OnEndPage( writer, document );
        int pageN = writer.PageNumber;
        String text = "Page " + pageN + " of ";
        float len = bf.GetWidthPoint( text, 8 );
        Rectangle pageSize = document.PageSize;
        cb.SetRGBColorFill( 100, 100, 100 );
        cb.BeginText();
        cb.SetFontAndSize( bf, 8 );
        cb.SetTextMatrix( pageSize.GetLeft( 40 ), pageSize.GetBottom( 30 ) );
        cb.ShowText( text );
        cb.EndText();
        cb.AddTemplate( template, pageSize.GetLeft( 40 ) + len, pageSize.GetBottom( 30 ) );

        cb.BeginText();
        cb.SetFontAndSize( bf, 8 );
        cb.ShowTextAligned( PdfContentByte.ALIGN_RIGHT,
        "Printed On " + PrintTime.ToString(),
        pageSize.GetRight( 40 ),
        pageSize.GetBottom( 30 ), 0 );
        cb.EndText();
    }

    public override void OnCloseDocument( PdfWriter writer, Document document )
    {
        base.OnCloseDocument( writer, document );
        template.BeginText();
        template.SetFontAndSize( bf, 8 );
        template.SetTextMatrix( 0, 0 );
        template.ShowText( "" + ( writer.PageNumber - 1 ) );
        template.EndText();
    }
}