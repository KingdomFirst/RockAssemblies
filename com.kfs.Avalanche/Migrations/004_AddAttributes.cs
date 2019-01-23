using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rock.Plugin;

namespace com.kfs.Avalanche.Migrations
{
    [MigrationNumber( 4, "1.8.0" )]
    public class AddAttributes : Migration
    {
        /// <summary>
        /// The commands to run to migrate plugin to the specific version
        /// </summary>
        public override void Up()
        {

            // Avalanche Page GUID's
            //"Sermons", "", "9711DB54-0FB0-4722-AB45-1DFB6158F922"
            //"Visit", "", "CD8A05F8-24FF-4D38-8ED0-FE2BF07C0CDE"
            //"Connect", "", "DE6D125E-892E-4F10-A33D-F84942582B1E"
            //"Give", "", "78EDFA36-FEC9-4D74-A34B-07C76A4FC071"
            //"Footer", "", "FF495C30-29C5-420C-A35B-E9E808EEBCEF"
            //"Visit Detail", "", "5F1E1759-0A3A-4A57-896B-7B8B8BD892B3"
            //"Series Detail", "", "4F8A1FFA-05A7-4522-8C23-74468FAFD6BC"
            //"Sermon Detail", "", "1CDE2032-482B-483E-B273-A1A3B421E04C"
            //"Sermon Detail Audio", "", "344BABC7-5C30-4B53-87F9-A00F0DDA38E7"
            //"Groups", "", "D943417B-F167-4CD0-8321-C779B1C9E92B"
            //"Baptism", "", "8E94B8E8-171D-4A81-9A69-D34667510231"
            //"Events", "", "B95062A4-770C-47DF-B88F-0626F7BFDF2F"
            //"Contact Us", "", "7817298D-0A76-4039-A5D7-AF273AC05952"
            //"Event Detail", "", "14A25533-186B-49C4-A949-0F9DA864A87E"
            //"Serve", "", "C90B1E1B-EC24-49E0-9562-7F92BB2D24AB"
            //"Other Blocks", "", "D4A5C0AB-0A90-416F-9B3F-C0ACFED92D43"
            //"Login", "", "F53E2612-3E8B-4D26-87ED-468D4C9C02E6"
            //"Prayer Request", "", "BB61CCD2-9E50-45A3-9D94-A37F3D7313FF"
            //"Person Card", "", "79EF0279 - 9800 - 4BAF - A8D9 - DD42E6868BAA"
            //"Group Attendance", "", "2FB9C8A1-B3E6-4608-8CE7-411AA27945BA"
            //"Group List", "", "0EC21B17-EA03-450E-A0B4-594C09340458"
            //"Group Member List", "", "C4A65B7E-D57C-44C9-93AD-9E2E51145481"
            //"Note Block", "", "F522A717-2D7A-405B-87BB-CCB16C15BB44"
            //"Webview Block", "", "0FBC5CE7-CF91-45EC-B28A-A5E486764B9A"

            // Attrib for BlockType: Audio Player Block:AutoPlay
            RockMigrationHelper.UpdateBlockTypeAttribute( "7515DA74-D55F-44C0-ABC0-A6D9DBFEF76B", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "AutoPlay", "AutoPlay", "", @"Start playing audio on load", 0, @"False", "38F36F45-EDEE-4DEE-8DE2-377BBD029D76" );
            // Attrib for BlockType: Audio Player Block:Source
            RockMigrationHelper.UpdateBlockTypeAttribute( "7515DA74-D55F-44C0-ABC0-A6D9DBFEF76B", "9C204CD0-1233-41C5-818A-C5DA439445AA", "Source", "Source", "", @"Audio file to be presented. Data is parsed through Lava with the request {{parameter}}.", 0, @"", "CA288975-9E8C-4612-96A0-23E1B7C3FDF9" );
            // Attrib for BlockType: Audio Player Block:Title
            RockMigrationHelper.UpdateBlockTypeAttribute( "7515DA74-D55F-44C0-ABC0-A6D9DBFEF76B", "9C204CD0-1233-41C5-818A-C5DA439445AA", "Title", "Title", "", @"Title of the audio file. Data is parsed through Lava with the request {{parameter}}.", 0, @"", "79211D59-E0D9-4C7C-919F-B5B084F9D953" );
            // Attrib for BlockType: Audio Player Block:Artist
            RockMigrationHelper.UpdateBlockTypeAttribute( "7515DA74-D55F-44C0-ABC0-A6D9DBFEF76B", "9C204CD0-1233-41C5-818A-C5DA439445AA", "Artist", "Artist", "", @"Artist or speaker's name. Data is parsed through Lava with the request {{parameter}}.", 0, @"", "614313D7-9634-4959-B117-6A2E7FA76272" );
            // Attrib for BlockType: Audio Player Block:Custom Attributes
            RockMigrationHelper.UpdateBlockTypeAttribute( "7515DA74-D55F-44C0-ABC0-A6D9DBFEF76B", "73B02051-0D38-4AD9-BF81-A2D477DE4F70", "Custom Attributes", "CustomAttributes", "", @"Custom attributes to set on block.", 0, @"", "B5C561AE-4926-42E3-8FF3-D231F0B7FAEC" );
            // Attrib for BlockType: Audio Player Block:Enabled Lava Commands
            RockMigrationHelper.UpdateBlockTypeAttribute( "7515DA74-D55F-44C0-ABC0-A6D9DBFEF76B", "4BD9088F-5CC6-89B1-45FC-A2AAFFC7CC0D", "Enabled Lava Commands", "EnabledLavaCommands", "", @"The Lava commands that should be enabled for this block.", 0, @"", "2DF42069-76B8-4414-B602-9828F1FDC2B2" );
            // Attrib for BlockType: Button:Action Item
            RockMigrationHelper.UpdateBlockTypeAttribute( "65CE4075-6A52-40F4-8ED0-6540F387BE76", "E5A6D6C7-DAB4-4EFA-B76F-E22AFEC5158D", "Action Item", "ActionItem", "", @"", 0, @"", "A9D5A12D-F60D-4CAA-A46B-1A605032586D" );
            // Attrib for BlockType: Button:Enabled Lava Commands
            RockMigrationHelper.UpdateBlockTypeAttribute( "65CE4075-6A52-40F4-8ED0-6540F387BE76", "4BD9088F-5CC6-89B1-45FC-A2AAFFC7CC0D", "Enabled Lava Commands", "EnabledLavaCommands", "", @"The Lava commands that should be enabled for this block.", 0, @"", "BEF7E221-837D-4DD5-BC3E-49E6E3BDEBF9" );
            // Attrib for BlockType: Button:Custom Attributes
            RockMigrationHelper.UpdateBlockTypeAttribute( "65CE4075-6A52-40F4-8ED0-6540F387BE76", "73B02051-0D38-4AD9-BF81-A2D477DE4F70", "Custom Attributes", "CustomAttributes", "", @"Custom attributes to set on block.", 0, @"", "FED19184-4169-4EEF-9437-592545A79461" );
            // Attrib for BlockType: Button:Text
            RockMigrationHelper.UpdateBlockTypeAttribute( "65CE4075-6A52-40F4-8ED0-6540F387BE76", "9C204CD0-1233-41C5-818A-C5DA439445AA", "Text", "Text", "", @"The text of the label to be displayed. Lava enabled with the {{parameter}} available.", 0, @"", "B60F03CA-B109-48F6-ACE4-1C365DC4E908" );
            // Attrib for BlockType: Avalanche Event Calendar Lava:Cache Duration
            RockMigrationHelper.UpdateBlockTypeAttribute( "A5129AF8-9744-4974-9EC3-F5D898DC7B77", "9C204CD0-1233-41C5-818A-C5DA439445AA", "Cache Duration", "CacheDuration", "", @"Ammount of time in minutes to cache the output of this block.", 14, @"3600", "4D9B40B4-14E2-4B00-9725-0D33B6606BD1" );
            // Attrib for BlockType: Avalanche Event Calendar Lava:Campus Parameter Name
            RockMigrationHelper.UpdateBlockTypeAttribute( "A5129AF8-9744-4974-9EC3-F5D898DC7B77", "9C204CD0-1233-41C5-818A-C5DA439445AA", "Campus Parameter Name", "CampusParameterName", "", @"The page parameter name that contains the id of the campus entity.", 18, @"campusId", "32ADBCD2-7858-4566-9AAE-4C5DC5C43246" );
            // Attrib for BlockType: Avalanche Event Calendar Lava:Category Parameter Name
            RockMigrationHelper.UpdateBlockTypeAttribute( "A5129AF8-9744-4974-9EC3-F5D898DC7B77", "9C204CD0-1233-41C5-818A-C5DA439445AA", "Category Parameter Name", "CategoryParameterName", "", @"The page parameter name that contains the id of the category entity.", 19, @"categoryId", "EC03AFD7-A544-4BEB-9CD8-5A87FC90B6B0" );
            // Attrib for BlockType: Avalanche Event Calendar Lava:Show Date Range Filter
            RockMigrationHelper.UpdateBlockTypeAttribute( "A5129AF8-9744-4974-9EC3-F5D898DC7B77", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Show Date Range Filter", "ShowDateRangeFilter", "", @"Determines whether the date range filters are shown", 7, @"False", "04992B21-617D-48A1-ADF3-951CB633CC93" );
            // Attrib for BlockType: Avalanche Event Calendar Lava:Show Small Calendar
            RockMigrationHelper.UpdateBlockTypeAttribute( "A5129AF8-9744-4974-9EC3-F5D898DC7B77", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Show Small Calendar", "ShowSmallCalendar", "", @"Determines whether the calendar widget is shown", 8, @"True", "197DA2B8-F798-44BF-B2C2-99723A3E2E45" );
            // Attrib for BlockType: Avalanche Event Calendar Lava:Show Day View
            RockMigrationHelper.UpdateBlockTypeAttribute( "A5129AF8-9744-4974-9EC3-F5D898DC7B77", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Show Day View", "ShowDayView", "", @"Determines whether the day view option is shown", 9, @"False", "3692C511-DFBF-4F5A-8BC0-16EAFC1B4BAC" );
            // Attrib for BlockType: Avalanche Event Calendar Lava:Show Week View
            RockMigrationHelper.UpdateBlockTypeAttribute( "A5129AF8-9744-4974-9EC3-F5D898DC7B77", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Show Week View", "ShowWeekView", "", @"Determines whether the week view option is shown", 10, @"True", "15084637-2667-4260-8E50-9B01EA852ADF" );
            // Attrib for BlockType: Avalanche Event Calendar Lava:Show Month View
            RockMigrationHelper.UpdateBlockTypeAttribute( "A5129AF8-9744-4974-9EC3-F5D898DC7B77", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Show Month View", "ShowMonthView", "", @"Determines whether the month view option is shown", 11, @"True", "42744616-A525-4252-8CEE-9CDE6250DD31" );
            // Attrib for BlockType: Avalanche Event Calendar Lava:Show Year View
            RockMigrationHelper.UpdateBlockTypeAttribute( "A5129AF8-9744-4974-9EC3-F5D898DC7B77", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Show Year View", "ShowYearView", "", @"Determines whether the year view option is shown", 12, @"True", "B7A1B651-9CC9-4961-AB8C-5E89FA7F19B2" );
            // Attrib for BlockType: Avalanche Event Calendar Lava:Enable Campus Context
            RockMigrationHelper.UpdateBlockTypeAttribute( "A5129AF8-9744-4974-9EC3-F5D898DC7B77", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Enable Campus Context", "EnableCampusContext", "", @"If the page has a campus context it's value will be used as a filter", 13, @"False", "13564317-66A2-4EE2-9078-5B314E8EB249" );
            // Attrib for BlockType: Avalanche Event Calendar Lava:Set Page Title
            RockMigrationHelper.UpdateBlockTypeAttribute( "A5129AF8-9744-4974-9EC3-F5D898DC7B77", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Set Page Title", "SetPageTitle", "", @"Determines if the block should set the page title with the calendar name.", 17, @"False", "B5EA1BE1-DE1D-4C9E-B565-A3050869CD58" );
            // Attrib for BlockType: Avalanche Event Calendar Lava:Default View Option
            RockMigrationHelper.UpdateBlockTypeAttribute( "A5129AF8-9744-4974-9EC3-F5D898DC7B77", "7525C4CB-EE6B-41D4-9B64-A08048D5A5C0", "Default View Option", "DefaultViewOption", "", @"Determines the default view option", 1, @"Week", "4BC3868B-23BB-4A46-970F-BD16447F705E" );
            // Attrib for BlockType: Avalanche Event Calendar Lava:Campus Filter Display Mode
            RockMigrationHelper.UpdateBlockTypeAttribute( "A5129AF8-9744-4974-9EC3-F5D898DC7B77", "7525C4CB-EE6B-41D4-9B64-A08048D5A5C0", "Campus Filter Display Mode", "CampusFilterDisplayMode", "", @"", 4, @"1", "7FBB51A6-B2C7-4E1D-ABAF-F4D01ABF1D8F" );
            // Attrib for BlockType: Avalanche Event Calendar Lava:Audience Filter Display Mode
            RockMigrationHelper.UpdateBlockTypeAttribute( "A5129AF8-9744-4974-9EC3-F5D898DC7B77", "7525C4CB-EE6B-41D4-9B64-A08048D5A5C0", "Audience Filter Display Mode", "CategoryFilterDisplayMode", "", @"", 5, @"1", "95AF549B-D25C-49BE-8B0F-59C3F3D1B4D4" );
            // Attrib for BlockType: Avalanche Event Calendar Lava:Filter Audiences
            RockMigrationHelper.UpdateBlockTypeAttribute( "A5129AF8-9744-4974-9EC3-F5D898DC7B77", "59D5A94C-94A0-4630-B80A-BB25697D74C7", "Filter Audiences", "FilterCategories", "", @"Determines which audiences should be displayed in the filter.", 6, @"", "9F67474F-B6DF-4549-884D-F89E56AA954F" );
            // Attrib for BlockType: Avalanche Event Calendar Lava:Component
            RockMigrationHelper.UpdateBlockTypeAttribute( "A5129AF8-9744-4974-9EC3-F5D898DC7B77", "59D5A94C-94A0-4630-B80A-BB25697D74C7", "Component", "Component", "", @"Different components will display your list in different ways.", 0, @"", "2A037292-80ED-4EF6-95E6-A3518EFFDC2C" );
            // Attrib for BlockType: Avalanche Event Calendar Lava:Custom Attributes
            RockMigrationHelper.UpdateBlockTypeAttribute( "A5129AF8-9744-4974-9EC3-F5D898DC7B77", "73B02051-0D38-4AD9-BF81-A2D477DE4F70", "Custom Attributes", "CustomAttributes", "", @"Custom attributes to set on block.", 0, @"", "0B61EA06-A713-4359-BCA4-1D0602E8C6CA" );
            // Attrib for BlockType: Avalanche Event Calendar Lava:Event Calendar
            RockMigrationHelper.UpdateBlockTypeAttribute( "A5129AF8-9744-4974-9EC3-F5D898DC7B77", "EC0D9528-1A22-404E-A776-566404987363", "Event Calendar", "EventCalendar", "", @"The event calendar to be displayed", 0, @"1", "29E37C6C-CCD8-4631-A2B5-200D93678D7A" );
            // Attrib for BlockType: Avalanche Event Calendar Lava:Lava Template
            RockMigrationHelper.UpdateBlockTypeAttribute( "A5129AF8-9744-4974-9EC3-F5D898DC7B77", "1D0D3794-C210-48A8-8C68-3FBEC08A6BA5", "Lava Template", "LavaTemplate", "", @"Lava template to use to display the list of events.", 15, @"{% include '~~/Assets/Lava/Calendar.lava' %}", "71BFE3A7-41CD-46E8-B95F-456735104DA2" );
            // Attrib for BlockType: Avalanche Event Calendar Lava:Start of Week Day
            RockMigrationHelper.UpdateBlockTypeAttribute( "A5129AF8-9744-4974-9EC3-F5D898DC7B77", "7EDFA2DE-FDD3-4AC1-B356-1F5BFC231DAE", "Start of Week Day", "StartofWeekDay", "", @"Determines what day is the start of a week.", 16, @"0", "A016AF17-6C91-49BD-AC92-C4B83A9BFE46" );
            // Attrib for BlockType: Avalanche Event Calendar Lava:Action Item
            RockMigrationHelper.UpdateBlockTypeAttribute( "A5129AF8-9744-4974-9EC3-F5D898DC7B77", "E5A6D6C7-DAB4-4EFA-B76F-E22AFEC5158D", "Action Item", "ActionItem", "", @"Action to take upon press of item in list.", 0, @"", "FD94A70E-EAC2-4851-9D3C-84629A93F62E" );
            // Attrib for BlockType: Avalanche Event Calendar Lava:Enabled Lava Commands
            RockMigrationHelper.UpdateBlockTypeAttribute( "A5129AF8-9744-4974-9EC3-F5D898DC7B77", "4BD9088F-5CC6-89B1-45FC-A2AAFFC7CC0D", "Enabled Lava Commands", "EnabledLavaCommands", "", @"The Lava commands that should be enabled for this HTML block.", 3, @"", "1AD0C6A7-16D4-4DBA-9584-342137D3E051" );
            // Attrib for BlockType: Content Channel Mobile List:Channel
            RockMigrationHelper.UpdateBlockTypeAttribute( "641A5E85-F2B4-46A3-8B9F-4AA759E33804", "D835A0EC-C8DB-483A-A37C-E8FB6E956C3D", "Channel", "Channel", "", @"The channel to display items from.", 0, @"", "25941CC2-1C51-46F6-AB82-7A9FA85ECFBA" );
            // Attrib for BlockType: Content Channel Mobile List:Component
            RockMigrationHelper.UpdateBlockTypeAttribute( "641A5E85-F2B4-46A3-8B9F-4AA759E33804", "59D5A94C-94A0-4630-B80A-BB25697D74C7", "Component", "Component", "", @"Different components will display your list in different ways.", 0, @"", "CF2A9EB3-8F1B-4562-979C-CB59D447BCCF" );
            // Attrib for BlockType: Content Channel Mobile List:Custom Attributes
            RockMigrationHelper.UpdateBlockTypeAttribute( "641A5E85-F2B4-46A3-8B9F-4AA759E33804", "73B02051-0D38-4AD9-BF81-A2D477DE4F70", "Custom Attributes", "CustomAttributes", "", @"Custom attributes to set on block.", 0, @"", "A88DA132-AA2B-47BD-8C04-F1A14D4B12A8" );
            // Attrib for BlockType: Content Channel Mobile List:Item Cache Duration
            RockMigrationHelper.UpdateBlockTypeAttribute( "641A5E85-F2B4-46A3-8B9F-4AA759E33804", "A75DFC58-7A1B-4799-BF31-451B2BBE38FF", "Item Cache Duration", "CacheDuration", "", @"Number of seconds to cache the content items returned by the selected filter.", 0, @"3600", "7A361636-F72A-40CA-91E2-20AA7496CDBC" );
            // Attrib for BlockType: Content Channel Mobile List:Output Cache Duration
            RockMigrationHelper.UpdateBlockTypeAttribute( "641A5E85-F2B4-46A3-8B9F-4AA759E33804", "A75DFC58-7A1B-4799-BF31-451B2BBE38FF", "Output Cache Duration", "OutputCacheDuration", "", @"Number of seconds to cache the resolved output. Only cache the output if you are not personalizing the output based on current user, current page, or any other merge field value.", 0, @"0", "0F110CCF-188D-4061-998C-7368A1BE171B" );
            // Attrib for BlockType: Content Channel Mobile List:Count
            RockMigrationHelper.UpdateBlockTypeAttribute( "641A5E85-F2B4-46A3-8B9F-4AA759E33804", "A75DFC58-7A1B-4799-BF31-451B2BBE38FF", "Count", "Count", "", @"The maximum number of items to display.", 0, @"10", "CE2FCBB8-22C5-4522-893C-DC63F5AD9E1F" );
            // Attrib for BlockType: Content Channel Mobile List:Filter Id
            RockMigrationHelper.UpdateBlockTypeAttribute( "641A5E85-F2B4-46A3-8B9F-4AA759E33804", "A75DFC58-7A1B-4799-BF31-451B2BBE38FF", "Filter Id", "FilterId", "", @"The data filter that is used to filter items", 0, @"0", "378D03E2-16C8-42AB-89FF-A90DE0FDA232" );
            // Attrib for BlockType: Content Channel Mobile List:Detail Page
            RockMigrationHelper.UpdateBlockTypeAttribute( "641A5E85-F2B4-46A3-8B9F-4AA759E33804", "BD53F9C9-EBA9-4D3F-82EA-DE5DD34A8108", "Detail Page", "DetailPage", "", @"The page to navigate to for details.", 1, @"", "46D432B9-7F18-4BF5-B874-54347A87A2C2" );
            // Attrib for BlockType: Content Channel Mobile List:Order
            RockMigrationHelper.UpdateBlockTypeAttribute( "641A5E85-F2B4-46A3-8B9F-4AA759E33804", "9C204CD0-1233-41C5-818A-C5DA439445AA", "Order", "Order", "", @"The specifics of how items should be ordered. This value is set through configuration and should not be modified here.", 0, @"", "956EFB28-F562-4A65-AEBB-AFACD44DB659" );
            // Attrib for BlockType: Content Channel Mobile List:Title Lava
            RockMigrationHelper.UpdateBlockTypeAttribute( "641A5E85-F2B4-46A3-8B9F-4AA759E33804", "9C204CD0-1233-41C5-818A-C5DA439445AA", "Title Lava", "TitleLava", "", @"Lava to display the details of each {{Item}}", 0, @"{{Item.Title}}", "DCD91C59-5152-4043-A7E2-A79301AF6A18" );
            // Attrib for BlockType: Content Channel Mobile List:Description Lava
            RockMigrationHelper.UpdateBlockTypeAttribute( "641A5E85-F2B4-46A3-8B9F-4AA759E33804", "9C204CD0-1233-41C5-818A-C5DA439445AA", "Description Lava", "DescriptionLava", "", @"The attribute key of the descriptionch formatted for mobile.", 0, @"", "7B7BF120-464E-45A6-9398-4E7F82C6B5F2" );
            // Attrib for BlockType: Content Channel Mobile List:Image Lava
            RockMigrationHelper.UpdateBlockTypeAttribute( "641A5E85-F2B4-46A3-8B9F-4AA759E33804", "9C204CD0-1233-41C5-818A-C5DA439445AA", "Image Lava", "ImageLava", "", @"The attribute key of the image to show on the list view will hide icon if not blank.", 0, @"", "4BC02D93-4ECB-44C0-9218-445815394C2E" );
            // Attrib for BlockType: Content Channel Mobile List:Icon Lava
            RockMigrationHelper.UpdateBlockTypeAttribute( "641A5E85-F2B4-46A3-8B9F-4AA759E33804", "9C204CD0-1233-41C5-818A-C5DA439445AA", "Icon Lava", "IconLava", "", @"The attribute key of the icon to show on the list view.", 0, @"", "1D0A5A0B-CD92-4120-93D1-4A5F698A33FC" );
            // Attrib for BlockType: Content Channel Mobile List:Order Lava
            RockMigrationHelper.UpdateBlockTypeAttribute( "641A5E85-F2B4-46A3-8B9F-4AA759E33804", "9C204CD0-1233-41C5-818A-C5DA439445AA", "Order Lava", "OrderLava", "", @"Lava to help order the items in the list {{Item}}", 0, @"", "DF036BE3-5D4F-4B56-B59B-1C323FAE7806" );
            // Attrib for BlockType: Content Channel Mobile List:Status
            RockMigrationHelper.UpdateBlockTypeAttribute( "641A5E85-F2B4-46A3-8B9F-4AA759E33804", "BD0D9B57-2A41-4490-89FF-F01DAB7D4904", "Status", "Status", "", @"Include items with the following status.", 0, @"2", "4B294624-5BB8-4457-9621-FC471BCD5823" );
            // Attrib for BlockType: Content Channel Mobile List:Query Parameter Filtering
            RockMigrationHelper.UpdateBlockTypeAttribute( "641A5E85-F2B4-46A3-8B9F-4AA759E33804", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Query Parameter Filtering", "QueryParameterFiltering", "", @"Determines if block should evaluate the query string parameters for additional filter criteria.", 0, @"False", "3BDF891A-3B7D-4839-A0B5-E83E966C5F5F" );
            // Attrib for BlockType: Group Attendance Block:Custom Attributes
            RockMigrationHelper.UpdateBlockTypeAttribute( "06A2663C-9292-4202-ACEE-A5F884671E55", "73B02051-0D38-4AD9-BF81-A2D477DE4F70", "Custom Attributes", "CustomAttributes", "", @"Custom attributes to set on block.", 0, @"", "151CEBB9-9BFF-43F3-9C73-672F6B645C7D" );
            // Attrib for BlockType: Group List:Custom Attributes
            RockMigrationHelper.UpdateBlockTypeAttribute( "55CB76A9-74F8-4F31-97B5-2567EBB25DFF", "73B02051-0D38-4AD9-BF81-A2D477DE4F70", "Custom Attributes", "CustomAttributes", "", @"Custom attributes to set on block.", 0, @"", "727903A8-FD3B-4E41-AFC4-309756390E34" );
            // Attrib for BlockType: Group List:Component
            RockMigrationHelper.UpdateBlockTypeAttribute( "55CB76A9-74F8-4F31-97B5-2567EBB25DFF", "59D5A94C-94A0-4630-B80A-BB25697D74C7", "Component", "Component", "", @"Different components will display your list in different ways.", 0, @"", "9B306F24-7D3A-4EF8-A1A1-C767211C92AB" );
            // Attrib for BlockType: Group List:Lava
            RockMigrationHelper.UpdateBlockTypeAttribute( "55CB76A9-74F8-4F31-97B5-2567EBB25DFF", "1D0D3794-C210-48A8-8C68-3FBEC08A6BA5", "Lava", "Lava", "", @"Lava to display list items.", 0, @"[
{% for group in Groups -%}
  { ""Id"":""{{group.Id}}"", ""Title"":""{{group.Name}}"", ""Description"":""{{group.Description}}"", ""Icon"":""{{group.GroupType.IconCssClass}}"" },
{% endfor -%}
]", "C9A3774F-21D3-49B1-8EFD-4A7807ED5A8A" );
            // Attrib for BlockType: Group List:Enabled Lava Commands
            RockMigrationHelper.UpdateBlockTypeAttribute( "55CB76A9-74F8-4F31-97B5-2567EBB25DFF", "4BD9088F-5CC6-89B1-45FC-A2AAFFC7CC0D", "Enabled Lava Commands", "EnabledLavaCommands", "", @"The Lava commands that should be enabled for this block.", 0, @"", "6451C0FE-D830-46C2-9C4E-F6CCA72C8A89" );
            // Attrib for BlockType: Group List:Action Item
            RockMigrationHelper.UpdateBlockTypeAttribute( "55CB76A9-74F8-4F31-97B5-2567EBB25DFF", "E5A6D6C7-DAB4-4EFA-B76F-E22AFEC5158D", "Action Item", "ActionItem", "", @"Action to take upon press of item in list.", 0, @"", "0CB6C7EC-FB8B-4C0A-805B-108075AA72BE" );
            // Attrib for BlockType: Group List:Only Show If Leader
            RockMigrationHelper.UpdateBlockTypeAttribute( "55CB76A9-74F8-4F31-97B5-2567EBB25DFF", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Only Show If Leader", "OnlyShowIfLeader", "", @"Should groups be hidden from all users except leaders?", 0, @"True", "16F74DEE-D2CF-459B-9E7B-0BE66F6A47AE" );
            // Attrib for BlockType: Group List:Parent Group Ids
            RockMigrationHelper.UpdateBlockTypeAttribute( "55CB76A9-74F8-4F31-97B5-2567EBB25DFF", "9C204CD0-1233-41C5-818A-C5DA439445AA", "Parent Group Ids", "ParentGroupIds", "", @"Comma separated list of id's of parent groups to display.", 0, @"", "36595C20-0B19-44D8-ADE9-446595557E80" );
            // Attrib for BlockType: Group Member List Block:Members Per Request
            RockMigrationHelper.UpdateBlockTypeAttribute( "E6BAE8D3-B244-400F-8092-AFD27156F6BD", "A75DFC58-7A1B-4799-BF31-451B2BBE38FF", "Members Per Request", "MembersPerRequest", "", @"The number of members to get per request. All group members will be loaded, but in multiple requests.", 0, @"20", "D40889E8-9468-40B2-8776-B15C7D9120E9" );
            // Attrib for BlockType: Group Member List Block:Action Item
            RockMigrationHelper.UpdateBlockTypeAttribute( "E6BAE8D3-B244-400F-8092-AFD27156F6BD", "E5A6D6C7-DAB4-4EFA-B76F-E22AFEC5158D", "Action Item", "ActionItem", "", @"Action to take upon press of item in list.", 0, @"", "0B073274-28E6-499F-A408-8CA05644F2EF" );
            // Attrib for BlockType: Group Member List Block:Enabled Lava Commands
            RockMigrationHelper.UpdateBlockTypeAttribute( "E6BAE8D3-B244-400F-8092-AFD27156F6BD", "4BD9088F-5CC6-89B1-45FC-A2AAFFC7CC0D", "Enabled Lava Commands", "EnabledLavaCommands", "", @"The Lava commands that should be enabled for this block.", 0, @"", "FB550261-8164-41BF-A2C8-554E161CB617" );
            // Attrib for BlockType: Group Member List Block:Component
            RockMigrationHelper.UpdateBlockTypeAttribute( "E6BAE8D3-B244-400F-8092-AFD27156F6BD", "59D5A94C-94A0-4630-B80A-BB25697D74C7", "Component", "Component", "", @"Different components will display your list in different ways.", 0, @"", "4D97403F-2540-4B22-9EC4-6BEF1DC283C4" );
            // Attrib for BlockType: Group Member List Block:Custom Attributes
            RockMigrationHelper.UpdateBlockTypeAttribute( "E6BAE8D3-B244-400F-8092-AFD27156F6BD", "73B02051-0D38-4AD9-BF81-A2D477DE4F70", "Custom Attributes", "CustomAttributes", "", @"Custom attributes to set on block.", 0, @"", "56498C7F-E638-4E45-A11F-644D6AC20242" );
            // Attrib for BlockType: Icon Block:Custom Attributes
            RockMigrationHelper.UpdateBlockTypeAttribute( "1112BA70-CE0D-4158-8A2B-D5F2FD217BAE", "73B02051-0D38-4AD9-BF81-A2D477DE4F70", "Custom Attributes", "CustomAttributes", "", @"Custom attributes to set on block.", 0, @"", "BE7D91EF-5C79-4940-9DA6-608933781419" );
            // Attrib for BlockType: Icon Block:Action Item
            RockMigrationHelper.UpdateBlockTypeAttribute( "1112BA70-CE0D-4158-8A2B-D5F2FD217BAE", "E5A6D6C7-DAB4-4EFA-B76F-E22AFEC5158D", "Action Item", "ActionItem", "", @"", 0, @"", "7B4B9D30-2BED-4F39-B897-10C7C561CFAE" );
            // Attrib for BlockType: Icon Block:Enabled Lava Commands
            RockMigrationHelper.UpdateBlockTypeAttribute( "1112BA70-CE0D-4158-8A2B-D5F2FD217BAE", "4BD9088F-5CC6-89B1-45FC-A2AAFFC7CC0D", "Enabled Lava Commands", "EnabledLavaCommands", "", @"The Lava commands that should be enabled for this block.", 0, @"", "0910B50C-23D8-46AC-BBF2-F54F074E35DC" );
            // Attrib for BlockType: Icon Block:Text
            RockMigrationHelper.UpdateBlockTypeAttribute( "1112BA70-CE0D-4158-8A2B-D5F2FD217BAE", "9C204CD0-1233-41C5-818A-C5DA439445AA", "Text", "Text", "", @"The text of the label to be displayed.", 0, @"", "4F4142E2-E6C0-4878-953F-521FC7A8A4C9" );
            // Attrib for BlockType: Icon Button:Text
            RockMigrationHelper.UpdateBlockTypeAttribute( "4CCA5B0C-63A6-40C1-B981-648663029092", "9C204CD0-1233-41C5-818A-C5DA439445AA", "Text", "Text", "", @"The text of the label to be displayed. Lava enabled with the {{parameter}} available.", 0, @"", "9FFA6301-04B4-4D1D-907C-D345BE825B13" );
            // Attrib for BlockType: Icon Button:Icon
            RockMigrationHelper.UpdateBlockTypeAttribute( "4CCA5B0C-63A6-40C1-B981-648663029092", "9C204CD0-1233-41C5-818A-C5DA439445AA", "Icon", "Icon", "", @"Icon to use on the button. Lava enabled with the {{parameter}} available.", 0, @"", "FCE07A51-64D8-4FC8-BFB2-8CDBCED449F7" );
            // Attrib for BlockType: Icon Button:Enabled Lava Commands
            RockMigrationHelper.UpdateBlockTypeAttribute( "4CCA5B0C-63A6-40C1-B981-648663029092", "4BD9088F-5CC6-89B1-45FC-A2AAFFC7CC0D", "Enabled Lava Commands", "EnabledLavaCommands", "", @"The Lava commands that should be enabled for this block.", 0, @"", "2FCA15F3-D621-4E7C-8F0C-FF9222AB3447" );
            // Attrib for BlockType: Icon Button:Action Item
            RockMigrationHelper.UpdateBlockTypeAttribute( "4CCA5B0C-63A6-40C1-B981-648663029092", "E5A6D6C7-DAB4-4EFA-B76F-E22AFEC5158D", "Action Item", "ActionItem", "", @"", 0, @"", "1AD33D1F-BDE9-4AFB-B6E8-E2CD271A5EF3" );
            // Attrib for BlockType: Icon Button:Custom Attributes
            RockMigrationHelper.UpdateBlockTypeAttribute( "4CCA5B0C-63A6-40C1-B981-648663029092", "73B02051-0D38-4AD9-BF81-A2D477DE4F70", "Custom Attributes", "CustomAttributes", "", @"Custom attributes to set on block.", 0, @"", "6D8A7116-BB0F-4F3A-9C24-5C39F2364E42" );
            // Attrib for BlockType: Image Block:Custom Attributes
            RockMigrationHelper.UpdateBlockTypeAttribute( "47C24453-10F9-4A11-9BD0-3D9B1CC943D3", "73B02051-0D38-4AD9-BF81-A2D477DE4F70", "Custom Attributes", "CustomAttributes", "", @"Custom attributes to set on block.", 0, @"", "B9523EB6-EEE5-4DC4-B2B0-EF7FF0E73F51" );
            // Attrib for BlockType: Image Block:Action Item
            RockMigrationHelper.UpdateBlockTypeAttribute( "47C24453-10F9-4A11-9BD0-3D9B1CC943D3", "E5A6D6C7-DAB4-4EFA-B76F-E22AFEC5158D", "Action Item", "ActionItem", "", @"Action to take on touch.", 0, @"", "68EF73D8-3C6D-4524-A3A4-9C06D95B164A" );
            // Attrib for BlockType: Image Block:Enabled Lava Commands
            RockMigrationHelper.UpdateBlockTypeAttribute( "47C24453-10F9-4A11-9BD0-3D9B1CC943D3", "4BD9088F-5CC6-89B1-45FC-A2AAFFC7CC0D", "Enabled Lava Commands", "EnabledLavaCommands", "", @"The Lava commands that should be enabled for this block.", 0, @"", "155B8623-7723-4FD2-A321-B9D24B1611CE" );
            // Attrib for BlockType: Image Block:Image
            RockMigrationHelper.UpdateBlockTypeAttribute( "47C24453-10F9-4A11-9BD0-3D9B1CC943D3", "9C204CD0-1233-41C5-818A-C5DA439445AA", "Image", "Image", "", @"Image to be displayed. Data is parsed through Lava with the request {{parameter}}.", 0, @"", "2E9389F8-0E3C-485D-B722-762D89C8EB2E" );
            // Attrib for BlockType: Image Block:Aspect
            RockMigrationHelper.UpdateBlockTypeAttribute( "47C24453-10F9-4A11-9BD0-3D9B1CC943D3", "7525C4CB-EE6B-41D4-9B64-A08048D5A5C0", "Aspect", "Aspect", "", @"Aspect type", 0, @"", "1900D9DD-EFA4-4278-8B23-0FEEF1894610" );
            // Attrib for BlockType: Label Block:Text
            RockMigrationHelper.UpdateBlockTypeAttribute( "A42B3143-E970-4BEC-A694-9BCB37B9B737", "9C204CD0-1233-41C5-818A-C5DA439445AA", "Text", "Text", "", @"The text of the label to be displayed.", 0, @"", "95980EF8-47FA-40C9-9372-938F46746458" );
            // Attrib for BlockType: Label Block:Enabled Lava Commands
            RockMigrationHelper.UpdateBlockTypeAttribute( "A42B3143-E970-4BEC-A694-9BCB37B9B737", "4BD9088F-5CC6-89B1-45FC-A2AAFFC7CC0D", "Enabled Lava Commands", "EnabledLavaCommands", "", @"The Lava commands that should be enabled for this block.", 0, @"", "7F860DC6-7478-4F88-A878-FAF3D75CEB9C" );
            // Attrib for BlockType: Label Block:Action Item
            RockMigrationHelper.UpdateBlockTypeAttribute( "A42B3143-E970-4BEC-A694-9BCB37B9B737", "E5A6D6C7-DAB4-4EFA-B76F-E22AFEC5158D", "Action Item", "ActionItem", "", @"", 0, @"", "0D9816B1-E6CF-46C3-A3C7-4099359B2857" );
            // Attrib for BlockType: Label Block:Custom Attributes
            RockMigrationHelper.UpdateBlockTypeAttribute( "A42B3143-E970-4BEC-A694-9BCB37B9B737", "73B02051-0D38-4AD9-BF81-A2D477DE4F70", "Custom Attributes", "CustomAttributes", "", @"Custom attributes to set on block.", 0, @"", "A59E551E-6B04-4C70-B1F6-B8EAEAC27D94" );
            // Attrib for BlockType: Login App:Custom Attributes
            RockMigrationHelper.UpdateBlockTypeAttribute( "857ABAF2-1F35-404E-827D-F4ADD629CBDF", "73B02051-0D38-4AD9-BF81-A2D477DE4F70", "Custom Attributes", "CustomAttributes", "", @"Custom attributes to set on block.", 0, @"", "3021F2AE-08B6-4EC0-B18F-F18C51D4DC7A" );
            // Attrib for BlockType: Markdown Detail:Custom Attributes
            RockMigrationHelper.UpdateBlockTypeAttribute( "35B0B2B4-BE73-4C03-8F56-DD8D28EBD7FA", "73B02051-0D38-4AD9-BF81-A2D477DE4F70", "Custom Attributes", "CustomAttributes", "", @"Custom attributes to set on block.", 0, @"", "538EE4EC-33C5-4997-ABD6-1F105F78EF38" );
            // Attrib for BlockType: Markdown Detail:Enabled Lava Commands
            RockMigrationHelper.UpdateBlockTypeAttribute( "35B0B2B4-BE73-4C03-8F56-DD8D28EBD7FA", "4BD9088F-5CC6-89B1-45FC-A2AAFFC7CC0D", "Enabled Lava Commands", "EnabledLavaCommands", "", @"The Lava commands that should be enabled for this block.", 0, @"", "7A9950AC-B9E7-4200-9B7A-634BF90DDB2B" );
            // Attrib for BlockType: Markdown Detail:Markdown
            RockMigrationHelper.UpdateBlockTypeAttribute( "35B0B2B4-BE73-4C03-8F56-DD8D28EBD7FA", "1D0D3794-C210-48A8-8C68-3FBEC08A6BA5", "Markdown", "Markdown", "", @"Markdown code to be rendered in app using the {{parameter}}.", 0, @"", "9CB3C25B-815D-44A4-9171-698136CC0988" );
            // Attrib for BlockType: Mobile ListView Lava:Lava
            RockMigrationHelper.UpdateBlockTypeAttribute( "755D550E-FB64-4DBE-A054-9D0141A18001", "1D0D3794-C210-48A8-8C68-3FBEC08A6BA5", "Lava", "Lava", "", @"Lava to display list items.", 0, @"", "2DF1245C-33A6-4C0B-9CFB-30047103BD05" );
            // Attrib for BlockType: Mobile ListView Lava:Enabled Lava Commands
            RockMigrationHelper.UpdateBlockTypeAttribute( "755D550E-FB64-4DBE-A054-9D0141A18001", "4BD9088F-5CC6-89B1-45FC-A2AAFFC7CC0D", "Enabled Lava Commands", "EnabledLavaCommands", "", @"The Lava commands that should be enabled for this block.", 0, @"", "1E87BAD4-50AB-474B-955B-E44FA10C0ADE" );
            // Attrib for BlockType: Mobile ListView Lava:Action Item
            RockMigrationHelper.UpdateBlockTypeAttribute( "755D550E-FB64-4DBE-A054-9D0141A18001", "E5A6D6C7-DAB4-4EFA-B76F-E22AFEC5158D", "Action Item", "ActionItem", "", @"Action to take upon press of item in list.", 0, @"", "9D4D1596-B680-4AFF-AF57-6C8867FC7D6B" );
            // Attrib for BlockType: Mobile ListView Lava:Custom Attributes
            RockMigrationHelper.UpdateBlockTypeAttribute( "755D550E-FB64-4DBE-A054-9D0141A18001", "73B02051-0D38-4AD9-BF81-A2D477DE4F70", "Custom Attributes", "CustomAttributes", "", @"Custom attributes to set on block.", 0, @"", "100D227F-518B-4AF4-9451-DB5663433C14" );
            // Attrib for BlockType: Mobile ListView Lava:Component
            RockMigrationHelper.UpdateBlockTypeAttribute( "755D550E-FB64-4DBE-A054-9D0141A18001", "59D5A94C-94A0-4630-B80A-BB25697D74C7", "Component", "Component", "", @"Different components will display your list in different ways.", 0, @"", "710306EF-D570-41D9-A806-DD38DC14FEDC" );
            // Attrib for BlockType: Mobile Workflow:Custom Attributes
            RockMigrationHelper.UpdateBlockTypeAttribute( "6B9DDC12-D7B3-4521-9D49-B79BE6578CB1", "73B02051-0D38-4AD9-BF81-A2D477DE4F70", "Custom Attributes", "CustomAttributes", "", @"Custom attributes to set on block.", 0, @"", "96FBBEC4-EDDE-4D98-8B3D-BF656CB4B563" );
            // Attrib for BlockType: Mobile Workflow:Workflow Type
            RockMigrationHelper.UpdateBlockTypeAttribute( "6B9DDC12-D7B3-4521-9D49-B79BE6578CB1", "46A03F59-55D3-4ACE-ADD5-B4642225DD20", "Workflow Type", "WorkflowType", "", @"Type of workflow to start.", 0, @"", "3667DECB-4576-46A5-B2CE-ACC2D43D0D69" );
            // Attrib for BlockType: Note Block:Custom Attributes
            RockMigrationHelper.UpdateBlockTypeAttribute( "275D5C9B-7D0B-42EE-A0D1-198B5C1A8A3B", "73B02051-0D38-4AD9-BF81-A2D477DE4F70", "Custom Attributes", "CustomAttributes", "", @"Custom attributes to set on block.", 0, @"", "D0EC7BA9-C9A3-48B3-A291-202571ED7AD4" );
            // Attrib for BlockType: Note Block:Note Type
            RockMigrationHelper.UpdateBlockTypeAttribute( "275D5C9B-7D0B-42EE-A0D1-198B5C1A8A3B", "E3FF88AC-13F6-4DF8-8371-FC0D7FD9A571", "Note Type", "NoteType", "", @"Type of note.", 0, @"", "E1E76347-70A9-464C-BA51-A4D938538A27" );
            // Attrib for BlockType: Note Block:Note Field Height
            RockMigrationHelper.UpdateBlockTypeAttribute( "275D5C9B-7D0B-42EE-A0D1-198B5C1A8A3B", "A75DFC58-7A1B-4799-BF31-451B2BBE38FF", "Note Field Height", "NoteFieldHeight", "", @"The height of the editor form element.", 0, @"200", "ED27D786-60AA-45E0-A57C-9893206F9A3E" );
            // Attrib for BlockType: Note Block:Note Field Label
            RockMigrationHelper.UpdateBlockTypeAttribute( "275D5C9B-7D0B-42EE-A0D1-198B5C1A8A3B", "9C204CD0-1233-41C5-818A-C5DA439445AA", "Note Field Label", "NoteFieldLabel", "", @"The height of the editor form element.", 0, @"Notes:", "34C4CC92-BC30-4835-9E36-71247FB2AC2D" );
            // Attrib for BlockType: Person Card:Accent Color
            RockMigrationHelper.UpdateBlockTypeAttribute( "9610B4D8-EB3B-45ED-B8DF-9E97FA1DF7AF", "9C204CD0-1233-41C5-818A-C5DA439445AA", "Accent Color", "AccentColor", "", @"Optional color to accent the member detail.", 0, @"", "3E3A6CB1-CAA4-4B7E-ABE3-4C58D870F912" );
            // Attrib for BlockType: Person Card:EntityType
            RockMigrationHelper.UpdateBlockTypeAttribute( "9610B4D8-EB3B-45ED-B8DF-9E97FA1DF7AF", "7525C4CB-EE6B-41D4-9B64-A08048D5A5C0", "EntityType", "EntityType", "", @"The entity type to get the person from.", 0, @"", "AF34D0D9-76BA-4D34-8236-929C71BC2CEB" );
            // Attrib for BlockType: Person Card:Custom Attributes
            RockMigrationHelper.UpdateBlockTypeAttribute( "9610B4D8-EB3B-45ED-B8DF-9E97FA1DF7AF", "73B02051-0D38-4AD9-BF81-A2D477DE4F70", "Custom Attributes", "CustomAttributes", "", @"Custom attributes to set on block.", 0, @"", "27CE0D5E-7E9A-4F63-B940-27F89017E267" );
            // Attrib for BlockType: Prayer Request Entry:Workflow
            RockMigrationHelper.UpdateBlockTypeAttribute( "8189E73E-7E5C-4D37-BD62-FD7FBDEEC9FD", "46A03F59-55D3-4ACE-ADD5-B4642225DD20", "Workflow", "Workflow", "", @"An optional workflow to start when prayer request is created. The PrayerRequest will be set as the workflow 'Entity' attribute when processing is started.", 17, @"", "9B8D5D1C-257C-403A-8B89-CC879373D1C6" );
            // Attrib for BlockType: Prayer Request Entry:Category Selection
            RockMigrationHelper.UpdateBlockTypeAttribute( "8189E73E-7E5C-4D37-BD62-FD7FBDEEC9FD", "309460EF-0CC5-41C6-9161-B3837BA3D374", "Category Selection", "CategorySelection", "", @"A top level category. This controls which categories the person can choose from when entering their prayer request.", 1, @"", "8F4AA029-6CC4-4826-8809-94EE0D97301C" );
            // Attrib for BlockType: Prayer Request Entry:Default Category
            RockMigrationHelper.UpdateBlockTypeAttribute( "8189E73E-7E5C-4D37-BD62-FD7FBDEEC9FD", "309460EF-0CC5-41C6-9161-B3837BA3D374", "Default Category", "DefaultCategory", "", @"If categories are not being shown, choose a default category to use for all new prayer requests.", 2, @"4B2D88F5-6E45-4B4B-8776-11118C8E8269", "06A82B98-4655-490D-BFC7-DE26BDD5EE3A" );
            // Attrib for BlockType: Prayer Request Entry:Custom Attributes
            RockMigrationHelper.UpdateBlockTypeAttribute( "8189E73E-7E5C-4D37-BD62-FD7FBDEEC9FD", "73B02051-0D38-4AD9-BF81-A2D477DE4F70", "Custom Attributes", "CustomAttributes", "", @"Custom attributes to set on block.", 0, @"", "CE8D0755-35A1-43B6-A0D8-9F6DA009E9A9" );
            // Attrib for BlockType: Prayer Request Entry:Expires After (Days)
            RockMigrationHelper.UpdateBlockTypeAttribute( "8189E73E-7E5C-4D37-BD62-FD7FBDEEC9FD", "A75DFC58-7A1B-4799-BF31-451B2BBE38FF", "Expires After (Days)", "ExpireDays", "", @"Number of days until the request will expire (only applies when auto-approved is enabled).", 4, @"14", "8503F519-CE9D-4CEE-B8B7-EDC5CB642896" );
            // Attrib for BlockType: Prayer Request Entry:Default Allow Comments Setting
            RockMigrationHelper.UpdateBlockTypeAttribute( "8189E73E-7E5C-4D37-BD62-FD7FBDEEC9FD", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Default Allow Comments Setting", "DefaultAllowCommentsSetting", "", @"This is the default setting for the 'Allow Comments' on prayer requests. If you enable the 'Comments Flag' below, the requestor can override this default setting.", 5, @"True", "7048A6C4-C76C-44C1-B227-B64D956BFC98" );
            // Attrib for BlockType: Prayer Request Entry:Enable Urgent Flag
            RockMigrationHelper.UpdateBlockTypeAttribute( "8189E73E-7E5C-4D37-BD62-FD7FBDEEC9FD", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Enable Urgent Flag", "EnableUrgentFlag", "", @"If enabled, requestors will be able to flag prayer requests as urgent.", 6, @"False", "CA49B740-8BB9-48B7-8A79-A7B49046EBB9" );
            // Attrib for BlockType: Prayer Request Entry:Default To Public
            RockMigrationHelper.UpdateBlockTypeAttribute( "8189E73E-7E5C-4D37-BD62-FD7FBDEEC9FD", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Default To Public", "DefaultToPublic", "", @"If enabled, all prayers will be set to public by default", 9, @"False", "DDDADB42-A9DD-4F43-AAE1-5EA53BC9BAC0" );
            // Attrib for BlockType: Prayer Request Entry:Require Last Name
            RockMigrationHelper.UpdateBlockTypeAttribute( "8189E73E-7E5C-4D37-BD62-FD7FBDEEC9FD", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Require Last Name", "RequireLastName", "", @"Require that a last name be entered", 11, @"True", "CD0A6E47-936F-4B75-8AAE-ACCB1A73107A" );
            // Attrib for BlockType: Prayer Request Entry:Enable Auto Approve
            RockMigrationHelper.UpdateBlockTypeAttribute( "8189E73E-7E5C-4D37-BD62-FD7FBDEEC9FD", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Enable Auto Approve", "EnableAutoApprove", "", @"If enabled, prayer requests are automatically approved; otherwise they must be approved by an admin before they can be seen by the prayer team.", 3, @"True", "8E427F57-420E-4775-94C7-747BC6C25EB5" );
            // Attrib for BlockType: Prayer Request Entry:Enable Comments Flag
            RockMigrationHelper.UpdateBlockTypeAttribute( "8189E73E-7E5C-4D37-BD62-FD7FBDEEC9FD", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Enable Comments Flag", "EnableCommentsFlag", "", @"If enabled, requestors will be able set whether or not they want to allow comments on their requests.", 7, @"False", "26A590AA-3E48-458E-9755-F4511609D24F" );
            // Attrib for BlockType: Prayer Request Entry:Enable Public Display Flag
            RockMigrationHelper.UpdateBlockTypeAttribute( "8189E73E-7E5C-4D37-BD62-FD7FBDEEC9FD", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Enable Public Display Flag", "EnablePublicDisplayFlag", "", @"If enabled, requestors will be able set whether or not they want their request displayed on the public website.", 8, @"False", "1EC69301-3C1A-40AB-A1D3-7FCC84358E45" );
            // Attrib for BlockType: Prayer Request Entry:Show Campus
            RockMigrationHelper.UpdateBlockTypeAttribute( "8189E73E-7E5C-4D37-BD62-FD7FBDEEC9FD", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Show Campus", "ShowCampus", "", @"Show a campus picker", 12, @"True", "5A4F4745-F7E7-4E0E-91CC-AD21190DEF54" );
            // Attrib for BlockType: Prayer Request Entry:Require Campus
            RockMigrationHelper.UpdateBlockTypeAttribute( "8189E73E-7E5C-4D37-BD62-FD7FBDEEC9FD", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Require Campus", "RequireCampus", "", @"Require that a campus be selected", 13, @"False", "58CAABD8-850F-4C4E-8B09-0CD424C424FD" );
            // Attrib for BlockType: Prayer Request Entry:Save Success Text
            RockMigrationHelper.UpdateBlockTypeAttribute( "8189E73E-7E5C-4D37-BD62-FD7FBDEEC9FD", "9C204CD0-1233-41C5-818A-C5DA439445AA", "Save Success Text", "SaveSuccessText", "", @"Text the user sees when the prayer is saved.", 16, @"Thank you for allowing us to pray for you.", "2EB23222-A5CD-4425-BA3E-F19495F53351" );
            // Attrib for BlockType: Prayer Request Entry:Action Item
            RockMigrationHelper.UpdateBlockTypeAttribute( "8189E73E-7E5C-4D37-BD62-FD7FBDEEC9FD", "E5A6D6C7-DAB4-4EFA-B76F-E22AFEC5158D", "Action Item", "ActionItem", "", @"Action to take upon submittin the prayer request.", 0, @"", "5EA0CDA9-449F-4661-A16A-55245C1BDEFE" );
            // Attrib for BlockType: Preload Block:Pages To Preload
            RockMigrationHelper.UpdateBlockTypeAttribute( "01CFA67B-429A-432E-BA56-4F4289917D06", "73B02051-0D38-4AD9-BF81-A2D477DE4F70", "Pages To Preload", "PagesToPreload", "", @"Pages with parameters to preload upon launching current page.", 0, @"", "73006B75-6B97-4203-8182-0944455BB213" );
            // Attrib for BlockType: Preload Block:Custom Attributes
            RockMigrationHelper.UpdateBlockTypeAttribute( "01CFA67B-429A-432E-BA56-4F4289917D06", "73B02051-0D38-4AD9-BF81-A2D477DE4F70", "Custom Attributes", "CustomAttributes", "", @"Custom attributes to set on block.", 0, @"", "E28D8CD0-91E8-453F-B221-C8A9E60DEE46" );
            // Attrib for BlockType: Preload Block:Enabled Lava Commands
            RockMigrationHelper.UpdateBlockTypeAttribute( "01CFA67B-429A-432E-BA56-4F4289917D06", "4BD9088F-5CC6-89B1-45FC-A2AAFFC7CC0D", "Enabled Lava Commands", "EnabledLavaCommands", "", @"The Lava commands that should be enabled for this block.", 0, @"", "2811B163-E681-4EBA-9280-A82742FEBE3C" );
            // Attrib for BlockType: Text Over Image Block:Action Item
            RockMigrationHelper.UpdateBlockTypeAttribute( "207ABAA7-8C45-472E-8649-3E4E895184B7", "E5A6D6C7-DAB4-4EFA-B76F-E22AFEC5158D", "Action Item", "ActionItem", "", @"Action to take on touch.", 0, @"", "0BF7D627-4BD3-4AE3-9AD7-CFABBA6C9799" );
            // Attrib for BlockType: Text Over Image Block:Enabled Lava Commands
            RockMigrationHelper.UpdateBlockTypeAttribute( "207ABAA7-8C45-472E-8649-3E4E895184B7", "4BD9088F-5CC6-89B1-45FC-A2AAFFC7CC0D", "Enabled Lava Commands", "EnabledLavaCommands", "", @"The Lava commands that should be enabled for this block.", 0, @"", "3B56BD7B-DFFA-4C07-A936-8B79029C01C5" );
            // Attrib for BlockType: Text Over Image Block:Custom Attributes
            RockMigrationHelper.UpdateBlockTypeAttribute( "207ABAA7-8C45-472E-8649-3E4E895184B7", "73B02051-0D38-4AD9-BF81-A2D477DE4F70", "Custom Attributes", "CustomAttributes", "", @"Custom attributes to set on block.", 0, @"", "B0FCF405-607A-43C1-A2DB-1AA2C066EABB" );
            // Attrib for BlockType: Text Over Image Block:Image
            RockMigrationHelper.UpdateBlockTypeAttribute( "207ABAA7-8C45-472E-8649-3E4E895184B7", "9C204CD0-1233-41C5-818A-C5DA439445AA", "Image", "Image", "", @"Image to be displayed. Data is parsed through Lava with the request {{parameter}}.", 0, @"", "B1DD109B-F099-4C88-AEF4-FDA0959B5530" );
            // Attrib for BlockType: Text Over Image Block:Text
            RockMigrationHelper.UpdateBlockTypeAttribute( "207ABAA7-8C45-472E-8649-3E4E895184B7", "9C204CD0-1233-41C5-818A-C5DA439445AA", "Text", "Text", "", @"Text to display over the image.", 0, @"", "3C535AE2-5D1D-4634-AFC2-658B4D55A3A8" );
            // Attrib for BlockType: Text Over Image Block:Aspect Ratio
            RockMigrationHelper.UpdateBlockTypeAttribute( "207ABAA7-8C45-472E-8649-3E4E895184B7", "C757A554-3009-4214-B05D-CEA2B2EA6B8F", "Aspect Ratio", "AspectRatio", "", @"The ratio of height to width. For example 0.5 would mean the image is half the height of the width.", 0, @"0.45", "BD4BC1F9-9C20-4A6E-A67D-8D5EF56A619D" );
            // Attrib for BlockType: Video Player Block:Aspect Ratio
            RockMigrationHelper.UpdateBlockTypeAttribute( "6FA66F35-5D80-4BD5-AD39-71618C1FFEAD", "9C204CD0-1233-41C5-818A-C5DA439445AA", "Aspect Ratio", "AspectRatio", "", @"The aspect ratio to display the video. Common ratios: 0.5625 = HD Video, 1.7778 = Vertical Video, 0.75 = SD Video", 0, @"0.5625", "0AF93DA7-6355-4EEB-AC12-D1C4AD8F0118" );
            // Attrib for BlockType: Video Player Block:Source
            RockMigrationHelper.UpdateBlockTypeAttribute( "6FA66F35-5D80-4BD5-AD39-71618C1FFEAD", "9C204CD0-1233-41C5-818A-C5DA439445AA", "Source", "Source", "", @"Video to be displayed. Data is parsed through Lava with the request {{parameter}}.", 0, @"", "8E4398DD-5BE5-4148-A87B-0C72E649CE6E" );
            // Attrib for BlockType: Video Player Block:AutoPlay
            RockMigrationHelper.UpdateBlockTypeAttribute( "6FA66F35-5D80-4BD5-AD39-71618C1FFEAD", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "AutoPlay", "AutoPlay", "", @"Start playing video on load", 0, @"False", "FB9755DA-5078-4CF1-826D-93CADE127EAA" );
            // Attrib for BlockType: Video Player Block:Custom Attributes
            RockMigrationHelper.UpdateBlockTypeAttribute( "6FA66F35-5D80-4BD5-AD39-71618C1FFEAD", "73B02051-0D38-4AD9-BF81-A2D477DE4F70", "Custom Attributes", "CustomAttributes", "", @"Custom attributes to set on block.", 0, @"", "8E6A53D3-AD8B-4993-86B6-981A909F66A2" );
            // Attrib for BlockType: Video Player Block:Enabled Lava Commands
            RockMigrationHelper.UpdateBlockTypeAttribute( "6FA66F35-5D80-4BD5-AD39-71618C1FFEAD", "4BD9088F-5CC6-89B1-45FC-A2AAFFC7CC0D", "Enabled Lava Commands", "EnabledLavaCommands", "", @"The Lava commands that should be enabled for this block.", 0, @"", "F860FBE4-55E7-4541-9C91-CC01463B3496" );
            // Attrib for BlockType: WebViewBlock:Custom Attributes
            RockMigrationHelper.UpdateBlockTypeAttribute( "78FC6291-B753-4782-8AED-DB04681F1D0E", "73B02051-0D38-4AD9-BF81-A2D477DE4F70", "Custom Attributes", "CustomAttributes", "", @"Custom attributes to set on block.", 0, @"", "4E1E3555-76B6-44C3-9F5E-56DFFB87C8E0" );
            // Attrib for BlockType: WebViewBlock:Url
            RockMigrationHelper.UpdateBlockTypeAttribute( "78FC6291-B753-4782-8AED-DB04681F1D0E", "9C204CD0-1233-41C5-818A-C5DA439445AA", "Url", "Url", "", @"Webpage to display.", 0, @"", "924B7E50-5C8B-4455-A0F7-B82364EBC905" );
            // Attrib for BlockType: WebViewBlock:Regex Limit
            RockMigrationHelper.UpdateBlockTypeAttribute( "78FC6291-B753-4782-8AED-DB04681F1D0E", "9C204CD0-1233-41C5-818A-C5DA439445AA", "Regex Limit", "RegexLimit", "", @"If a page is opened that doesn't match this Regex. A request to open in external browser will appear.", 0, @"", "D98A8E24-D980-4FB6-B5D4-00CB8287C851" );
            // Attrib for BlockType: Event Filter:Custom Attributes
            RockMigrationHelper.UpdateBlockTypeAttribute( "DE3997BC-AE8E-43B0-A41B-1F52C1BFE3B0", "73B02051-0D38-4AD9-BF81-A2D477DE4F70", "Custom Attributes", "CustomAttributes", "", @"Custom attributes to set on block.", 0, @"", "92E11239-D329-4E43-9847-72358122D052" );
            // Attrib for BlockType: Event Filter:Action Item
            RockMigrationHelper.UpdateBlockTypeAttribute( "DE3997BC-AE8E-43B0-A41B-1F52C1BFE3B0", "E5A6D6C7-DAB4-4EFA-B76F-E22AFEC5158D", "Action Item", "ActionItem", "", @"Action to take upon changing the filter", 0, @"", "CBB79612-83F9-4FC5-94EC-450DED06B30B" );
            // Attrib for BlockType: Public Profile Edit Block:Custom Attributes
            RockMigrationHelper.UpdateBlockTypeAttribute( "41D83AAD-F2B8-43DA-978E-1391B4427280", "73B02051-0D38-4AD9-BF81-A2D477DE4F70", "Custom Attributes", "CustomAttributes", "", @"Custom attributes to set on block.", 0, @"", "CE5435AF-C9E4-437B-B0D4-870B88DA514D" );
            // Attrib for BlockType: Public Profile Edit Block:Next Page
            RockMigrationHelper.UpdateBlockTypeAttribute( "41D83AAD-F2B8-43DA-978E-1391B4427280", "BD53F9C9-EBA9-4D3F-82EA-DE5DD34A8108", "Next Page", "NextPage", "", @"Page to forward to after completion.", 0, @"", "5EC718EB-8262-4A98-91AD-71807750863B" );
            // Attrib for BlockType: Person Profile Family:Additional Changes Link
            RockMigrationHelper.UpdateBlockTypeAttribute( "36569642-F6D7-4EAC-AF0E-C8238BEEAF7E", "BD53F9C9-EBA9-4D3F-82EA-DE5DD34A8108", "Additional Changes Link", "AdditionalChangesLink", "", @"Optional page to request aditional changes not permitted here.", 0, @"", "8A00043B-EDA0-4020-9915-46A008087928" );
            // Attrib for BlockType: Person Profile Family:Custom Attributes
            RockMigrationHelper.UpdateBlockTypeAttribute( "36569642-F6D7-4EAC-AF0E-C8238BEEAF7E", "73B02051-0D38-4AD9-BF81-A2D477DE4F70", "Custom Attributes", "CustomAttributes", "", @"Custom attributes to set on block.", 0, @"", "A928AE35-E9AC-45DE-9D7D-50A0548E8063" );
            // Attrib for BlockType: Person Profile Family:Component
            RockMigrationHelper.UpdateBlockTypeAttribute( "36569642-F6D7-4EAC-AF0E-C8238BEEAF7E", "59D5A94C-94A0-4630-B80A-BB25697D74C7", "Component", "Component", "", @"Different components will display your list in different ways.", 0, @"", "CFEB82A4-02FA-4F22-A38E-08E1B6FBD4ED" );
            // Attrib for BlockType: Person Profile Family:Action Item
            RockMigrationHelper.UpdateBlockTypeAttribute( "36569642-F6D7-4EAC-AF0E-C8238BEEAF7E", "E5A6D6C7-DAB4-4EFA-B76F-E22AFEC5158D", "Action Item", "ActionItem", "", @"Action to take upon press of item in list.", 0, @"", "508BD5E9-D674-46AD-83DE-1BF620C85B29" );

            // Attrib Value for Block:Text Over Image Block - Sermons, Attribute:Action Item Page: Avalanche Home Page, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "A2487BA5-843D-4D7E-92A6-36A52027EB26", "0BF7D627-4BD3-4AE3-9AD7-CFABBA6C9799", @"1^519^" ); // NEED SQL, 519
            Sql( @"DECLARE @PageId int = ( SELECT TOP 1 [Id] FROM [Page] WHERE [Guid] = '9711DB54-0FB0-4722-AB45-1DFB6158F922' )
                UPDATE av SET av.[Value] = Replace([Value],519,@PageId)
                FROM [AttributeValue] av 
				INNER JOIN Attribute a ON a.Id = av.AttributeId 
				INNER JOIN Block b ON a.EntityTypeQualifierColumn = 'BlockTypeId' AND a.EntityTypeQualifierValue = b.BlockTypeId
                WHERE a.[Guid] = '0BF7D627-4BD3-4AE3-9AD7-CFABBA6C9799' AND b.[Guid] = 'A2487BA5-843D-4D7E-92A6-36A52027EB26'" ); // Set AttributeValue to correct page id         
            // Attrib Value for Block:Text Over Image Block - Sermons, Attribute:Image Page: Avalanche Home Page, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "A2487BA5-843D-4D7E-92A6-36A52027EB26", "B1DD109B-F099-4C88-AEF4-FDA0959B5530", @"{{ 'Global' | Attribute:'PublicApplicationRoot' }}/Content/app/sermons.jpg" );
            // Attrib Value for Block:Text Over Image Block - Sermons, Attribute:Text Page: Avalanche Home Page, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "A2487BA5-843D-4D7E-92A6-36A52027EB26", "3C535AE2-5D1D-4634-AFC2-658B4D55A3A8", @"Sermons" );
            // Attrib Value for Block:Text Over Image Block - Sermons, Attribute:Aspect Ratio Page: Avalanche Home Page, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "A2487BA5-843D-4D7E-92A6-36A52027EB26", "BD4BC1F9-9C20-4A6E-A67D-8D5EF56A619D", @"0.45" );
            // Attrib Value for Block:Text Over Image Block - Sermons, Attribute:Enabled Lava Commands Page: Avalanche Home Page, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "A2487BA5-843D-4D7E-92A6-36A52027EB26", "3B56BD7B-DFFA-4C07-A936-8B79029C01C5", @"" );
            // Attrib Value for Block:Text Over Image Block - Sermons, Attribute:Custom Attributes Page: Avalanche Home Page, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "A2487BA5-843D-4D7E-92A6-36A52027EB26", "B0FCF405-607A-43C1-A2DB-1AA2C066EABB", @"TextColor^White|FontSize^55" );
            // Attrib Value for Block:Text Over Image Block - Events, Attribute:Custom Attributes Page: Avalanche Home Page, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "7E1A52B6-68A9-423F-88DB-4E402360FED0", "B0FCF405-607A-43C1-A2DB-1AA2C066EABB", @"TextColor^White|FontSize^55|HorizontalOptions^Center" );
            // Attrib Value for Block:Text Over Image Block - Events, Attribute:Aspect Ratio Page: Avalanche Home Page, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "7E1A52B6-68A9-423F-88DB-4E402360FED0", "BD4BC1F9-9C20-4A6E-A67D-8D5EF56A619D", @"0.45" );
            // Attrib Value for Block:Text Over Image Block - Events, Attribute:Enabled Lava Commands Page: Avalanche Home Page, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "7E1A52B6-68A9-423F-88DB-4E402360FED0", "3B56BD7B-DFFA-4C07-A936-8B79029C01C5", @"" );
            // Attrib Value for Block:Text Over Image Block - Events, Attribute:Image Page: Avalanche Home Page, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "7E1A52B6-68A9-423F-88DB-4E402360FED0", "B1DD109B-F099-4C88-AEF4-FDA0959B5530", @"{{ 'Global' | Attribute:'PublicApplicationRoot' }}/Content/app/events.jpg" );
            // Attrib Value for Block:Text Over Image Block - Events, Attribute:Text Page: Avalanche Home Page, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "7E1A52B6-68A9-423F-88DB-4E402360FED0", "3C535AE2-5D1D-4634-AFC2-658B4D55A3A8", @"Events" );
            // Attrib Value for Block:Text Over Image Block - Events, Attribute:Action Item Page: Avalanche Home Page, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "7E1A52B6-68A9-423F-88DB-4E402360FED0", "0BF7D627-4BD3-4AE3-9AD7-CFABBA6C9799", @"1^531^" ); // NEED SQL, 531
            Sql( @"DECLARE @PageId int = ( SELECT TOP 1 [Id] FROM [Page] WHERE [Guid] = 'B95062A4-770C-47DF-B88F-0626F7BFDF2F' )
                UPDATE av SET av.[Value] = Replace([Value],531,@PageId)
                FROM [AttributeValue] av 
				INNER JOIN Attribute a ON a.Id = av.AttributeId 
				INNER JOIN Block b ON a.EntityTypeQualifierColumn = 'BlockTypeId' AND a.EntityTypeQualifierValue = b.BlockTypeId
                WHERE a.[Guid] = '0BF7D627-4BD3-4AE3-9AD7-CFABBA6C9799' AND b.[Guid] = '7E1A52B6-68A9-423F-88DB-4E402360FED0'" ); // Set AttributeValue to correct page id         
            // Attrib Value for Block:Text Over Image Block - Visit, Attribute:Image Page: Avalanche Home Page, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "99865F0D-BD8A-4AA7-88A2-D7D7859E1992", "B1DD109B-F099-4C88-AEF4-FDA0959B5530", @"{{ 'Global' | Attribute:'PublicApplicationRoot' }}/Content/app/visit.jpg" );
            // Attrib Value for Block:Text Over Image Block - Visit, Attribute:Text Page: Avalanche Home Page, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "99865F0D-BD8A-4AA7-88A2-D7D7859E1992", "3C535AE2-5D1D-4634-AFC2-658B4D55A3A8", @"Visit" );
            // Attrib Value for Block:Text Over Image Block - Visit, Attribute:Action Item Page: Avalanche Home Page, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "99865F0D-BD8A-4AA7-88A2-D7D7859E1992", "0BF7D627-4BD3-4AE3-9AD7-CFABBA6C9799", @"1^520^" ); // NEED SQL, 520
            Sql( @"DECLARE @PageId int = ( SELECT TOP 1 [Id] FROM [Page] WHERE [Guid] = 'CD8A05F8-24FF-4D38-8ED0-FE2BF07C0CDE' )
                UPDATE av SET av.[Value] = Replace([Value],520,@PageId)
                FROM [AttributeValue] av 
				INNER JOIN Attribute a ON a.Id = av.AttributeId 
				INNER JOIN Block b ON a.EntityTypeQualifierColumn = 'BlockTypeId' AND a.EntityTypeQualifierValue = b.BlockTypeId
                WHERE a.[Guid] = '0BF7D627-4BD3-4AE3-9AD7-CFABBA6C9799' AND b.[Guid] = '99865F0D-BD8A-4AA7-88A2-D7D7859E1992'" ); // Set AttributeValue to correct page id         
            // Attrib Value for Block:Text Over Image Block - Visit, Attribute:Aspect Ratio Page: Avalanche Home Page, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "99865F0D-BD8A-4AA7-88A2-D7D7859E1992", "BD4BC1F9-9C20-4A6E-A67D-8D5EF56A619D", @"0.45" );
            // Attrib Value for Block:Text Over Image Block - Visit, Attribute:Enabled Lava Commands Page: Avalanche Home Page, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "99865F0D-BD8A-4AA7-88A2-D7D7859E1992", "3B56BD7B-DFFA-4C07-A936-8B79029C01C5", @"" );
            // Attrib Value for Block:Text Over Image Block - Visit, Attribute:Custom Attributes Page: Avalanche Home Page, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "99865F0D-BD8A-4AA7-88A2-D7D7859E1992", "B0FCF405-607A-43C1-A2DB-1AA2C066EABB", @"TextColor^White|FontSize^55" );
            // Attrib Value for Block:Listview Footer, Attribute:Lava Page: Footer, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "0DBA9AAB-FA2E-4C53-B820-A4C0B3FF29D0", "2DF1245C-33A6-4C0B-9CFB-30047103BD05", @"{% page id:'{{ 'Global' | Attribute:'AvalancheHomePage' }}' %}{% capture pagestr %}{% for childpage in page.Pages %}{% if childpage.DisplayInNavWhen != 'Never' %}{""Id"":""{{ forloop.index0 }}"",""Title"":""{{ childpage.PageTitle }}"",""Description"":""{{ childpage.PageDescription }}"",""Icon"":""{{ childpage.IconCssClass }}"",""Image"":"""",""Resource"":""{% assign resource = childpage | Attribute:""Resource"" %}{% if resource != '' %}{{ resource }}{% else %}{{ childpage.Id }}{% endif %}"",""ActionType"":""{{ childpage | Attribute:""ActionType"",""RawValue"" }}""}, {% endif %}{% endfor %}{% endcapture %}{% endpage -%}
[{{ pagestr | ReplaceLast:"", "" }}]" );
            // Attrib Value for Block:Listview Footer, Attribute:Component Page: Footer, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "0DBA9AAB-FA2E-4C53-B820-A4C0B3FF29D0", "710306EF-D570-41D9-A806-DD38DC14FEDC", @"673b7db5-2200-41d6-8857-9a7663b56c47" );
            // Attrib Value for Block:Listview Footer, Attribute:Enabled Lava Commands Page: Footer, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "0DBA9AAB-FA2E-4C53-B820-A4C0B3FF29D0", "1E87BAD4-50AB-474B-955B-E44FA10C0ADE", @"RockEntity" );
            // Attrib Value for Block:Listview Footer, Attribute:Action Item Page: Footer, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "0DBA9AAB-FA2E-4C53-B820-A4C0B3FF29D0", "9D4D1596-B680-4AFF-AF57-6C8867FC7D6B", @"0" );
            // Attrib Value for Block:Listview Footer, Attribute:Custom Attributes Page: Footer, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "0DBA9AAB-FA2E-4C53-B820-A4C0B3FF29D0", "100D227F-518B-4AF4-9451-DB5663433C14", @"Columns^4|FontSize^10|IconFontSize^25|HeightRequest^70|BackgroundColor^#0094d9|Margin^0%2C5%2C0%2C0|TextColor^White|IconTextColor^White" );
            // Attrib Value for Block:Icon Block - Back, Attribute:Enabled Lava Commands , Layout: Simple, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "D2DFBCB4-894C-4E5C-A534-110B21A00FEB", "0910B50C-23D8-46AC-BBF2-F54F074E35DC", @"" );
            // Attrib Value for Block:Icon Block - Back, Attribute:Text , Layout: Simple, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "D2DFBCB4-894C-4E5C-A534-110B21A00FEB", "4F4142E2-E6C0-4878-953F-521FC7A8A4C9", @"fa fa-chevron-left" );
            // Attrib Value for Block:Icon Block - Back, Attribute:Action Item , Layout: Simple, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "D2DFBCB4-894C-4E5C-A534-110B21A00FEB", "7B4B9D30-2BED-4F39-B897-10C7C561CFAE", @"3" );
            // Attrib Value for Block:Icon Block - Back, Attribute:Custom Attributes , Layout: Simple, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "D2DFBCB4-894C-4E5C-A534-110B21A00FEB", "BE7D91EF-5C79-4940-9DA6-608933781419", @"TextColor^White|FontSize^25|Margin^15%2C19%2C10%2C16" );
            // Attrib Value for Block:Text Over Image Block - Plan a visit, Attribute:Custom Attributes Page: Visit, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "C730CAF4-F17F-4D9C-B2F3-9A6A29491FD7", "B0FCF405-607A-43C1-A2DB-1AA2C066EABB", @"TextColor^White|FontSize^48|HorizontalOptions^Center" );
            // Attrib Value for Block:Text Over Image Block - Plan a visit, Attribute:Enabled Lava Commands Page: Visit, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "C730CAF4-F17F-4D9C-B2F3-9A6A29491FD7", "3B56BD7B-DFFA-4C07-A936-8B79029C01C5", @"" );
            // Attrib Value for Block:Text Over Image Block - Plan a visit, Attribute:Text Page: Visit, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "C730CAF4-F17F-4D9C-B2F3-9A6A29491FD7", "3C535AE2-5D1D-4634-AFC2-658B4D55A3A8", @"Plan A Visit" );
            // Attrib Value for Block:Text Over Image Block - Plan a visit, Attribute:Aspect Ratio Page: Visit, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "C730CAF4-F17F-4D9C-B2F3-9A6A29491FD7", "BD4BC1F9-9C20-4A6E-A67D-8D5EF56A619D", @"0.35" );
            // Attrib Value for Block:Text Over Image Block - Plan a visit, Attribute:Image Page: Visit, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "C730CAF4-F17F-4D9C-B2F3-9A6A29491FD7", "B1DD109B-F099-4C88-AEF4-FDA0959B5530", @"{{ 'Global' | Attribute:'PublicApplicationRoot' }}/Content/app/planavisit.jpg" );
            // Attrib Value for Block:Text Over Image Block - Plan a visit, Attribute:Action Item Page: Visit, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "C730CAF4-F17F-4D9C-B2F3-9A6A29491FD7", "0BF7D627-4BD3-4AE3-9AD7-CFABBA6C9799", @"0" );
            // Attrib Value for Block:Label Block - Welcome, Attribute:Enabled Lava Commands Page: Visit, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "57096CBB-21E8-4EF5-ABD3-05B2414225D0", "7F860DC6-7478-4F88-A878-FAF3D75CEB9C", @"" );
            // Attrib Value for Block:Label Block - Welcome, Attribute:Action Item Page: Visit, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "57096CBB-21E8-4EF5-ABD3-05B2414225D0", "0D9816B1-E6CF-46C3-A3C7-4099359B2857", @"0" );
            // Attrib Value for Block:Label Block - Welcome, Attribute:Text Page: Visit, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "57096CBB-21E8-4EF5-ABD3-05B2414225D0", "95980EF8-47FA-40C9-9372-938F46746458", @"WELCOME" );
            // Attrib Value for Block:Label Block - Welcome, Attribute:Custom Attributes Page: Visit, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "57096CBB-21E8-4EF5-ABD3-05B2414225D0", "A59E551E-6B04-4C70-B1F6-B8EAEAC27D94", @"TextColor^#0094d9|FontSize^25|HorizontalOptions^Center|FontFamily^Open Sans Light" );
            // Attrib Value for Block:Welcome Text, Attribute:Markdown Page: Visit, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "0B08E530-6763-473F-A338-C7E2B2F753BA", "9CB3C25B-815D-44A4-9171-698136CC0988", @"Traders Point is one church gathering in multiple locations around the Indianapolis area to worship, learn more about Jesus and the Bible, and serve our local communities and partners around the world." );
            // Attrib Value for Block:Welcome Text, Attribute:Enabled Lava Commands Page: Visit, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "0B08E530-6763-473F-A338-C7E2B2F753BA", "7A9950AC-B9E7-4200-9B7A-634BF90DDB2B", @"" );
            // Attrib Value for Block:Welcome Text, Attribute:Custom Attributes Page: Visit, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "0B08E530-6763-473F-A338-C7E2B2F753BA", "538EE4EC-33C5-4997-ABD6-1F105F78EF38", @"Margin^15%2C10" );
            // Attrib Value for Block:Campus Listview, Attribute:Action Item Page: Visit, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "14EFA755-D8F8-4BC2-A199-E67B2E960A95", "9D4D1596-B680-4AFF-AF57-6C8867FC7D6B", @"0" );
            // Attrib Value for Block:Campus Listview, Attribute:Custom Attributes Page: Visit, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "14EFA755-D8F8-4BC2-A199-E67B2E960A95", "100D227F-518B-4AF4-9451-DB5663433C14", @"Columns^1|Margin^10%2C0" );
            // Attrib Value for Block:Campus Listview, Attribute:Component Page: Visit, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "14EFA755-D8F8-4BC2-A199-E67B2E960A95", "710306EF-D570-41D9-A806-DD38DC14FEDC", @"1a637b48-35fb-43b2-9822-88af2fd1d333" );
            // Attrib Value for Block:Campus Listview, Attribute:Enabled Lava Commands Page: Visit, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "14EFA755-D8F8-4BC2-A199-E67B2E960A95", "1E87BAD4-50AB-474B-955B-E44FA10C0ADE", @"" );
            // Attrib Value for Block:Campus Listview, Attribute:Lava Page: Visit, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "14EFA755-D8F8-4BC2-A199-E67B2E960A95", "2DF1245C-33A6-4C0B-9CFB-30047103BD05", @"{% capture campusjson -%}
 {% for campus in Campuses -%}
  {% if campus.IsActive -%}
{""Id"":""{{ campus.Id }}"",""Title"":"""",""Description"":"""",""Icon"":"""",""Image"":""{% assign imgurl = campus | Attribute:'AppImage','Url' %}{{ imgurl }}"", ""Resource"":""524"", ""ActionType"":""1"" },{% endif -%}
 {% endfor -%}
{% endcapture -%}
[{{ campusjson | ReplaceLast:', ' }}]" ); // NEED SQL
            Sql( @"DECLARE @PageId int = ( SELECT TOP 1 [Id] FROM [Page] WHERE [Guid] = '5F1E1759-0A3A-4A57-896B-7B8B8BD892B3' )
                UPDATE av SET av.[Value] = Replace([Value],524,@PageId)
                FROM [AttributeValue] av 
				INNER JOIN Attribute a ON a.Id = av.AttributeId 
				INNER JOIN Block b ON a.EntityTypeQualifierColumn = 'BlockTypeId' AND a.EntityTypeQualifierValue = b.BlockTypeId
                WHERE a.[Guid] = '2DF1245C-33A6-4C0B-9CFB-30047103BD05' AND b.[Guid] = '14EFA755-D8F8-4BC2-A199-E67B2E960A95'" ); // Set AttributeValue to correct page id         

            // Attrib Value for Block:Image Block, Attribute:Aspect Page: Visit Detail, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "941F3F7B-2922-4D29-9B8E-FE22A5337486", "1900D9DD-EFA4-4278-8B23-0FEEF1894610", @"" );
            // Attrib Value for Block:Image Block, Attribute:Custom Attributes Page: Visit Detail, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "941F3F7B-2922-4D29-9B8E-FE22A5337486", "B9523EB6-EEE5-4DC4-B2B0-EF7FF0E73F51", @"" );
            // Attrib Value for Block:Image Block, Attribute:Custom Attributes Page: Visit Detail, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "941F3F7B-2922-4D29-9B8E-FE22A5337486", "B0FCF405-607A-43C1-A2DB-1AA2C066EABB", @"" );
            // Attrib Value for Block:Image Block, Attribute:Enabled Lava Commands Page: Visit Detail, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "941F3F7B-2922-4D29-9B8E-FE22A5337486", "155B8623-7723-4FD2-A321-B9D24B1611CE", @"RockEntity" );
            // Attrib Value for Block:Image Block, Attribute:Action Item Page: Visit Detail, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "941F3F7B-2922-4D29-9B8E-FE22A5337486", "68EF73D8-3C6D-4524-A3A4-9C06D95B164A", @"0" );
            // Attrib Value for Block:Image Block, Attribute:Image Page: Visit Detail, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "941F3F7B-2922-4D29-9B8E-FE22A5337486", "2E9389F8-0E3C-485D-B722-762D89C8EB2E", @"{% campus id:'{{parameter}}' %}{% assign imgurl = campus | Attribute:'AppHeaderImage','Url' %}{{ imgurl }}{% endcampus %}" );
            // Attrib Value for Block:Image Block, Attribute:Text Page: Visit Detail, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "941F3F7B-2922-4D29-9B8E-FE22A5337486", "3C535AE2-5D1D-4634-AFC2-658B4D55A3A8", @"{% campus id:'{{parameter}}' %}{{ campus.Name | Upcase }}{% endcampus %}" );
            // Attrib Value for Block:Image Block, Attribute:Enabled Lava Commands Page: Visit Detail, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "941F3F7B-2922-4D29-9B8E-FE22A5337486", "3B56BD7B-DFFA-4C07-A936-8B79029C01C5", @"RockEntity" );
            // Attrib Value for Block:Image Block, Attribute:Aspect Ratio Page: Visit Detail, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "941F3F7B-2922-4D29-9B8E-FE22A5337486", "BD4BC1F9-9C20-4A6E-A67D-8D5EF56A619D", @"0.45" );
            // Attrib Value for Block:Image Block, Attribute:Action Item Page: Visit Detail, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "941F3F7B-2922-4D29-9B8E-FE22A5337486", "0BF7D627-4BD3-4AE3-9AD7-CFABBA6C9799", @"0" );
            // Attrib Value for Block:Image Block, Attribute:Image Page: Visit Detail, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "941F3F7B-2922-4D29-9B8E-FE22A5337486", "B1DD109B-F099-4C88-AEF4-FDA0959B5530", @"{% campus id:'{{parameter}}' %}{% assign imgurl = campus | Attribute:'AppHeaderImage','Url' %}{{ imgurl }}{% endcampus %}" );
            // Attrib Value for Block:Header Title, Attribute:Custom Attributes Page: Visit Detail, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "089C9B48-AE89-416C-98FB-32304206DB20", "A59E551E-6B04-4C70-B1F6-B8EAEAC27D94", @"TextColor^#0094d9|HorizontalOptions^Center|FontSize^25|FontFamily^Open Sans Light|Margin^10" );
            // Attrib Value for Block:Header Title, Attribute:Action Item Page: Visit Detail, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "089C9B48-AE89-416C-98FB-32304206DB20", "0D9816B1-E6CF-46C3-A3C7-4099359B2857", @"0" );
            // Attrib Value for Block:Header Title, Attribute:Text Page: Visit Detail, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "089C9B48-AE89-416C-98FB-32304206DB20", "95980EF8-47FA-40C9-9372-938F46746458", @"{% campus id:'{{parameter}}' %}{{ campus.Name | Upcase }}{% endcampus %}" );
            // Attrib Value for Block:Header Title, Attribute:Enabled Lava Commands Page: Visit Detail, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "089C9B48-AE89-416C-98FB-32304206DB20", "7F860DC6-7478-4F88-A878-FAF3D75CEB9C", @"RockEntity" );
            // Attrib Value for Block:Description/Location summary text, Attribute:Enabled Lava Commands Page: Visit Detail, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "BDAAFA41-836E-4C27-9696-847D685D3980", "7A9950AC-B9E7-4200-9B7A-634BF90DDB2B", @"RockEntity" );
            // Attrib Value for Block:Description/Location summary text, Attribute:Custom Attributes Page: Visit Detail, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "BDAAFA41-836E-4C27-9696-847D685D3980", "538EE4EC-33C5-4997-ABD6-1F105F78EF38", @"Margin^10" );
            // Attrib Value for Block:Description/Location summary text, Attribute:Markdown Page: Visit Detail, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "BDAAFA41-836E-4C27-9696-847D685D3980", "9CB3C25B-815D-44A4-9171-698136CC0988", @"{% campus id:'{{parameter}}' %}{% assign campusinfo = campus | Attribute:'LocationInfoSummary' -%}
{{ campusinfo | HtmlToMarkdown }}{% endcampus %}" );
            // Attrib Value for Block:Parking Map Button, Attribute:Text Page: Visit Detail, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "41BD57CC-71E3-4DC0-957C-F9265061FA65", "B60F03CA-B109-48F6-ACE4-1C365DC4E908", @"{% campus id:'{{parameter}}' %}{% assign imgurl = campus | Attribute:'ParkingMap','Url' %}{% if imgurl != '' %}Parking Map{% endif %}{% endcampus %}" );
            // Attrib Value for Block:Parking Map Button, Attribute:Action Item Page: Visit Detail, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "41BD57CC-71E3-4DC0-957C-F9265061FA65", "A9D5A12D-F60D-4CAA-A46B-1A605032586D", @"4^{% campus id:'{{parameter}}' %}{% assign imgurl = campus | Attribute:'ParkingMap','Url' %}{{ imgurl }}{% endcampus %}^0" );
            // Attrib Value for Block:Parking Map Button, Attribute:Enabled Lava Commands Page: Visit Detail, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "41BD57CC-71E3-4DC0-957C-F9265061FA65", "BEF7E221-837D-4DD5-BC3E-49E6E3BDEBF9", @"RockEntity" );
            // Attrib Value for Block:Parking Map Button, Attribute:Custom Attributes Page: Visit Detail, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "41BD57CC-71E3-4DC0-957C-F9265061FA65", "FED19184-4169-4EEF-9437-592545A79461", @"WidthRequest^100|BackgroundColor^White|TextColor^#0094d9" );
            // Attrib Value for Block:Parking Map Button, Attribute:Text Page: Visit Detail, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "41BD57CC-71E3-4DC0-957C-F9265061FA65", "9FFA6301-04B4-4D1D-907C-D345BE825B13", @"{% campus id:'{{parameter}}' %}{% assign imgurl = campus | Attribute:'ParkingMap','Url' %}{% if imgurl != '' %}Parking Map{% endif %}{% endcampus %}" );
            // Attrib Value for Block:Parking Map Button, Attribute:Action Item Page: Visit Detail, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "41BD57CC-71E3-4DC0-957C-F9265061FA65", "1AD33D1F-BDE9-4AFB-B6E8-E2CD271A5EF3", @"4^{% campus id:'{{parameter}}' %}{% assign imgurl = campus | Attribute:'ParkingMap','Url' %}{{ imgurl }}{% endcampus %}^0" );
            // Attrib Value for Block:Parking Map Button, Attribute:Custom Attributes Page: Visit Detail, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "41BD57CC-71E3-4DC0-957C-F9265061FA65", "6D8A7116-BB0F-4F3A-9C24-5C39F2364E42", @"TextColor^#0094d9|BackgroundColor^White|HorizontalOptions^Center" );
            // Attrib Value for Block:Parking Map Button, Attribute:Icon Page: Visit Detail, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "41BD57CC-71E3-4DC0-957C-F9265061FA65", "FCE07A51-64D8-4FC8-BFB2-8CDBCED449F7", @"fa fa-map" );
            // Attrib Value for Block:Parking Map Button, Attribute:Enabled Lava Commands Page: Visit Detail, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "41BD57CC-71E3-4DC0-957C-F9265061FA65", "2FCA15F3-D621-4E7C-8F0C-FF9222AB3447", @"RockEntity" );
            // Attrib Value for Block:More Info Button, Attribute:Enabled Lava Commands Page: Visit Detail, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "20E578B0-BAB2-40DA-944D-9189465C6A96", "2FCA15F3-D621-4E7C-8F0C-FF9222AB3447", @"RockEntity" );
            // Attrib Value for Block:More Info Button, Attribute:Text Page: Visit Detail, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "20E578B0-BAB2-40DA-944D-9189465C6A96", "9FFA6301-04B4-4D1D-907C-D345BE825B13", @"{% campus id:'{{parameter}}' %}{% if campus.Url != '' %}More Info{% endif %}{% endcampus %}" );
            // Attrib Value for Block:More Info Button, Attribute:Icon Page: Visit Detail, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "20E578B0-BAB2-40DA-944D-9189465C6A96", "FCE07A51-64D8-4FC8-BFB2-8CDBCED449F7", @"fa fa-plus" );
            // Attrib Value for Block:More Info Button, Attribute:Action Item Page: Visit Detail, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "20E578B0-BAB2-40DA-944D-9189465C6A96", "1AD33D1F-BDE9-4AFB-B6E8-E2CD271A5EF3", @"4^{% campus id:'{{parameter}}' %}{{ campus.Url }}{% endcampus %}^0" );
            // Attrib Value for Block:More Info Button, Attribute:Custom Attributes Page: Visit Detail, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "20E578B0-BAB2-40DA-944D-9189465C6A96", "6D8A7116-BB0F-4F3A-9C24-5C39F2364E42", @"TextColor^#0094d9|HorizontalOptions^Center|BackgroundColor^White" );
            // Attrib Value for Block:Content Channel Mobile List, Attribute:Item Cache Duration Page: Sermons, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "724E2C2F-213B-4959-9F01-1957C77733DD", "7A361636-F72A-40CA-91E2-20AA7496CDBC", @"0" );
            // Attrib Value for Block:Content Channel Mobile List, Attribute:Custom Attributes Page: Sermons, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "724E2C2F-213B-4959-9F01-1957C77733DD", "A88DA132-AA2B-47BD-8C04-F1A14D4B12A8", @"Columns^1|VerticalOptions^Fill" );
            // Attrib Value for Block:Content Channel Mobile List, Attribute:Output Cache Duration Page: Sermons, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "724E2C2F-213B-4959-9F01-1957C77733DD", "0F110CCF-188D-4061-998C-7368A1BE171B", @"0" );
            // Attrib Value for Block:Content Channel Mobile List, Attribute:Component Page: Sermons, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "724E2C2F-213B-4959-9F01-1957C77733DD", "CF2A9EB3-8F1B-4562-979C-CB59D447BCCF", @"1a637b48-35fb-43b2-9822-88af2fd1d333" );
            // Attrib Value for Block:Content Channel Mobile List, Attribute:Detail Page Page: Sermons, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "724E2C2F-213B-4959-9F01-1957C77733DD", "46D432B9-7F18-4BF5-B874-54347A87A2C2", @"4f8a1ffa-05a7-4522-8c23-74468fafd6bc" );
            // Attrib Value for Block:Content Channel Mobile List, Attribute:Channel Page: Sermons, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "724E2C2F-213B-4959-9F01-1957C77733DD", "25941CC2-1C51-46F6-AB82-7A9FA85ECFBA", @"e2c598f1-d299-1baa-4873-8b679e3c1998" );
            // Attrib Value for Block:Content Channel Mobile List, Attribute:Status Page: Sermons, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "724E2C2F-213B-4959-9F01-1957C77733DD", "4B294624-5BB8-4457-9621-FC471BCD5823", @"2" );
            // Attrib Value for Block:Content Channel Mobile List, Attribute:Count Page: Sermons, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "724E2C2F-213B-4959-9F01-1957C77733DD", "CE2FCBB8-22C5-4522-893C-DC63F5AD9E1F", @"10" );
            // Attrib Value for Block:Content Channel Mobile List, Attribute:Filter Id Page: Sermons, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "724E2C2F-213B-4959-9F01-1957C77733DD", "378D03E2-16C8-42AB-89FF-A90DE0FDA232", @"92" );
            // Attrib Value for Block:Content Channel Mobile List, Attribute:Query Parameter Filtering Page: Sermons, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "724E2C2F-213B-4959-9F01-1957C77733DD", "3BDF891A-3B7D-4839-A0B5-E83E966C5F5F", @"False" );
            // Attrib Value for Block:Content Channel Mobile List, Attribute:Order Page: Sermons, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "724E2C2F-213B-4959-9F01-1957C77733DD", "956EFB28-F562-4A65-AEBB-AFACD44DB659", @"" );
            // Attrib Value for Block:Content Channel Mobile List, Attribute:Title Lava Page: Sermons, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "724E2C2F-213B-4959-9F01-1957C77733DD", "DCD91C59-5152-4043-A7E2-A79301AF6A18", @"{{ Item.Title }}" );
            // Attrib Value for Block:Content Channel Mobile List, Attribute:Description Lava Page: Sermons, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "724E2C2F-213B-4959-9F01-1957C77733DD", "7B7BF120-464E-45A6-9398-4E7F82C6B5F2", @"" );
            // Attrib Value for Block:Content Channel Mobile List, Attribute:Image Lava Page: Sermons, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "724E2C2F-213B-4959-9F01-1957C77733DD", "4BC02D93-4ECB-44C0-9218-445815394C2E", @"{{ 'Global' | Attribute:'PublicApplicationRoot' }}GetImage.ashx?guid={{ Item | Attribute:'SeriesImage','RawValue' }}" );
            // Attrib Value for Block:Content Channel Mobile List, Attribute:Icon Lava Page: Sermons, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "724E2C2F-213B-4959-9F01-1957C77733DD", "1D0A5A0B-CD92-4120-93D1-4A5F698A33FC", @"" );
            // Attrib Value for Block:Mobile ListView Lava - Sermon Content, Attribute:Enabled Lava Commands Page: Series Detail, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "7AA9738F-BA87-4761-9FD3-FF8A38703473", "1E87BAD4-50AB-474B-955B-E44FA10C0ADE", @"RockEntity" );
            // Attrib Value for Block:Mobile ListView Lava - Sermon Content, Attribute:Custom Attributes Page: Series Detail, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "7AA9738F-BA87-4761-9FD3-FF8A38703473", "100D227F-518B-4AF4-9451-DB5663433C14", @"Margin^5%2C0|BackgroundColor^#f2f6f8|VerticalOptions^Start" );
            // Attrib Value for Block:Mobile ListView Lava - Sermon Content, Attribute:Action Item Page: Series Detail, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "7AA9738F-BA87-4761-9FD3-FF8A38703473", "9D4D1596-B680-4AFF-AF57-6C8867FC7D6B", @"0" );
            // Attrib Value for Block:Mobile ListView Lava - Sermon Content, Attribute:Lava Page: Series Detail, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "7AA9738F-BA87-4761-9FD3-FF8A38703473", "2DF1245C-33A6-4C0B-9CFB-30047103BD05", @"{% contentchannelitem id:'{{parameter}}' %}{% assign seriesItem = contentchannelitemItems | First -%}{% capture seriesart %}{{ 'Global' | Attribute:'PublicApplicationRoot' }}GetImage.ashx?guid={{ seriesItem | Attribute:'SeriesImage','RawValue' }}{% endcapture %}
{% capture childjson %}{% for childitem in seriesItem.ChildItems %}{""Id"":""{{ childitem.ChildContentChannelItem.Id }}"",""Image"":""{{ seriesart }}"",""Title"":""{{ childitem.ChildContentChannelItem.Title | HtmlDecode | Replace:'""','\""' }}"",""Description"":""{{ childitem.ChildContentChannelItem.Content | StripHtml | HtmlDecode | Replace:'""','\""' }}"",""Resource"":""527"",""ActionType"":""1""},{% endfor -%}{% endcapture %}
{% endcontentchannelitem -%}
[{{ childjson | ReplaceLast:"","" }}]" ); // NEED SQL, 527
            Sql( @"DECLARE @PageId int = ( SELECT TOP 1 [Id] FROM [Page] WHERE [Guid] = '1CDE2032-482B-483E-B273-A1A3B421E04C' )
                UPDATE av SET av.[Value] = Replace([Value],527,@PageId)
                FROM [AttributeValue] av 
				INNER JOIN Attribute a ON a.Id = av.AttributeId 
				INNER JOIN Block b ON a.EntityTypeQualifierColumn = 'BlockTypeId' AND a.EntityTypeQualifierValue = b.BlockTypeId
                WHERE a.[Guid] = '2DF1245C-33A6-4C0B-9CFB-30047103BD05' AND b.[Guid] = '7AA9738F-BA87-4761-9FD3-FF8A38703473'" ); // Set AttributeValue to correct page id         
            // Attrib Value for Block:Mobile ListView Lava - Sermon Content, Attribute:Component Page: Series Detail, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "7AA9738F-BA87-4761-9FD3-FF8A38703473", "710306EF-D570-41D9-A806-DD38DC14FEDC", @"d9ea2c97-68e1-4d94-b881-f3ac4f2883a3" );
            // Attrib Value for Block:Image Block, Attribute:Custom Attributes Page: Series Detail, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "A47A5177-CD5B-4A1F-9F6D-C1718DB0BA16", "B9523EB6-EEE5-4DC4-B2B0-EF7FF0E73F51", @"" );
            // Attrib Value for Block:Image Block, Attribute:Image Page: Series Detail, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "A47A5177-CD5B-4A1F-9F6D-C1718DB0BA16", "2E9389F8-0E3C-485D-B722-762D89C8EB2E", @"{% contentchannelitem id:'{{parameter}}' %}{% assign seriesItem = contentchannelitemItems | First -%}{{ 'Global' | Attribute:'PublicApplicationRoot' }}GetImage.ashx?guid={{ seriesItem | Attribute:'SeriesImage','RawValue' }}{% endcontentchannelitem %}" );
            // Attrib Value for Block:Image Block, Attribute:Action Item Page: Series Detail, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "A47A5177-CD5B-4A1F-9F6D-C1718DB0BA16", "68EF73D8-3C6D-4524-A3A4-9C06D95B164A", @"0" );
            // Attrib Value for Block:Image Block, Attribute:Aspect Page: Series Detail, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "A47A5177-CD5B-4A1F-9F6D-C1718DB0BA16", "1900D9DD-EFA4-4278-8B23-0FEEF1894610", @"" );
            // Attrib Value for Block:Image Block, Attribute:Enabled Lava Commands Page: Series Detail, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "A47A5177-CD5B-4A1F-9F6D-C1718DB0BA16", "155B8623-7723-4FD2-A321-B9D24B1611CE", @"RockEntity" );
            // Attrib Value for Block:Markdown Detail, Attribute:Markdown Page: Series Detail, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "EDC51B07-CA23-4415-8997-E03A60D5FA5A", "9CB3C25B-815D-44A4-9171-698136CC0988", @"{% contentchannelitem id:'{{parameter}}' %}{% assign seriesItem = contentchannelitemItems | First -%}{{ seriesItem.Content | HtmlToMarkdown }}{% endcontentchannelitem %}" );
            // Attrib Value for Block:Markdown Detail, Attribute:Custom Attributes Page: Series Detail, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "EDC51B07-CA23-4415-8997-E03A60D5FA5A", "538EE4EC-33C5-4997-ABD6-1F105F78EF38", @"Margin^20%2C10" );
            // Attrib Value for Block:Markdown Detail, Attribute:Enabled Lava Commands Page: Series Detail, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "EDC51B07-CA23-4415-8997-E03A60D5FA5A", "7A9950AC-B9E7-4200-9B7A-634BF90DDB2B", @"RockEntity" );
            // Attrib Value for Block:Video Player Block, Attribute:Aspect Ratio Page: Sermon Detail, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "7E110550-159D-463A-A532-673564806896", "0AF93DA7-6355-4EEB-AC12-D1C4AD8F0118", @"0.5625" );
            // Attrib Value for Block:Video Player Block, Attribute:Source Page: Sermon Detail, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "7E110550-159D-463A-A532-673564806896", "8E4398DD-5BE5-4148-A87B-0C72E649CE6E", @"{% contentchannelitem id:'{{parameter}}' %}{% assign sermonItem = contentchannelitemItems | First -%}{{ sermonItem | Attribute:'VideoLink' }}{% endcontentchannelitem %}" );
            // Attrib Value for Block:Video Player Block, Attribute:Enabled Lava Commands Page: Sermon Detail, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "7E110550-159D-463A-A532-673564806896", "F860FBE4-55E7-4541-9C91-CC01463B3496", @"RockEntity" );
            // Attrib Value for Block:Video Player Block, Attribute:AutoPlay Page: Sermon Detail, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "7E110550-159D-463A-A532-673564806896", "FB9755DA-5078-4CF1-826D-93CADE127EAA", @"True" );
            // Attrib Value for Block:Video Player Block, Attribute:Custom Attributes Page: Sermon Detail, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "7E110550-159D-463A-A532-673564806896", "8E6A53D3-AD8B-4993-86B6-981A909F66A2", @"" );
            // Attrib Value for Block:Markdown Detail, Attribute:Enabled Lava Commands Page: Sermon Detail, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "11955742-2F20-40BB-B304-7865C992C7B4", "7A9950AC-B9E7-4200-9B7A-634BF90DDB2B", @"RockEntity" );
            // Attrib Value for Block:Markdown Detail, Attribute:Custom Attributes Page: Sermon Detail, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "11955742-2F20-40BB-B304-7865C992C7B4", "538EE4EC-33C5-4997-ABD6-1F105F78EF38", @"Margin^20%2C10" );
            // Attrib Value for Block:Markdown Detail, Attribute:Markdown Page: Sermon Detail, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "11955742-2F20-40BB-B304-7865C992C7B4", "9CB3C25B-815D-44A4-9171-698136CC0988", @"{% contentchannelitem id:'{{parameter}}' %}{% assign sermonItem = contentchannelitemItems | First -%}**{{ sermonItem.Title }}**
{{ sermonItem.Content | HtmlToMarkdown }}{% endcontentchannelitem %}" );
            // Attrib Value for Block:Markdown Detail, Attribute:Markdown Page: Sermon Detail Audio, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "8F975583-92F5-4827-909B-97D3D09E4BEC", "9CB3C25B-815D-44A4-9171-698136CC0988", @"{% contentchannelitem id:'{{parameter}}' %}{% assign sermonItem = contentchannelitemItems | First -%}**{{ sermonItem.Title }}**
{{ sermonItem.Content | HtmlToMarkdown }}{% endcontentchannelitem %}" );
            // Attrib Value for Block:Markdown Detail, Attribute:Enabled Lava Commands Page: Sermon Detail Audio, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "8F975583-92F5-4827-909B-97D3D09E4BEC", "7A9950AC-B9E7-4200-9B7A-634BF90DDB2B", @"RockEntity" );
            // Attrib Value for Block:Markdown Detail, Attribute:Custom Attributes Page: Sermon Detail Audio, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "8F975583-92F5-4827-909B-97D3D09E4BEC", "538EE4EC-33C5-4997-ABD6-1F105F78EF38", @"Margin^20%2C10" );
            // Attrib Value for Block:Audio Player Block, Attribute:Source Page: Sermon Detail Audio, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "7761BC7C-DDE7-4DCB-81C9-720339A24477", "CA288975-9E8C-4612-96A0-23E1B7C3FDF9", @"{% contentchannelitem id:'{{parameter}}' %}{% assign sermonItem = contentchannelitemItems | First -%}{{ sermonItem | Attribute:'AudioLink' }}{% endcontentchannelitem %}" );
            // Attrib Value for Block:Audio Player Block, Attribute:Enabled Lava Commands Page: Sermon Detail Audio, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "7761BC7C-DDE7-4DCB-81C9-720339A24477", "2DF42069-76B8-4414-B602-9828F1FDC2B2", @"RockEntity" );
            // Attrib Value for Block:Audio Player Block, Attribute:AutoPlay Page: Sermon Detail Audio, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "7761BC7C-DDE7-4DCB-81C9-720339A24477", "38F36F45-EDEE-4DEE-8DE2-377BBD029D76", @"True" );
            // Attrib Value for Block:Audio Player Block, Attribute:Title Page: Sermon Detail Audio, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "7761BC7C-DDE7-4DCB-81C9-720339A24477", "79211D59-E0D9-4C7C-919F-B5B084F9D953", @"{% contentchannelitem id:'{{parameter}}' %}{% assign sermonItem = contentchannelitemItems | First -%}{{ sermonItem.Title }}{% endcontentchannelitem %}" );
            // Attrib Value for Block:Audio Player Block, Attribute:Artist Page: Sermon Detail Audio, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "7761BC7C-DDE7-4DCB-81C9-720339A24477", "614313D7-9634-4959-B117-6A2E7FA76272", @"{% contentchannelitem id:'{{parameter}}' %}{% assign sermonItem = contentchannelitemItems | First -%}{{ sermonItem | Attribute:'Speaker' }}{% endcontentchannelitem %}" );
            // Attrib Value for Block:Audio Player Block, Attribute:Custom Attributes Page: Sermon Detail Audio, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "7761BC7C-DDE7-4DCB-81C9-720339A24477", "B5C561AE-4926-42E3-8FF3-D231F0B7FAEC", @"BackgroundColor^#0094d9|TextColor^White" );
            // Attrib Value for Block:Icon Button - Listen, Attribute:Text Page: Sermon Detail, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "BA50E899-CEAB-4816-8AD0-A650889882A4", "9FFA6301-04B4-4D1D-907C-D345BE825B13", @"Listen" );
            // Attrib Value for Block:Icon Button - Listen, Attribute:Action Item Page: Sermon Detail, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "BA50E899-CEAB-4816-8AD0-A650889882A4", "1AD33D1F-BDE9-4AFB-B6E8-E2CD271A5EF3", @"2^528^{{ parameter }}" ); // NEED SQL, 528
            Sql( @"DECLARE @PageId int = ( SELECT TOP 1 [Id] FROM [Page] WHERE [Guid] = '344BABC7-5C30-4B53-87F9-A00F0DDA38E7' )
                UPDATE av SET av.[Value] = Replace([Value],528,@PageId)
                FROM [AttributeValue] av 
				INNER JOIN Attribute a ON a.Id = av.AttributeId 
				INNER JOIN Block b ON a.EntityTypeQualifierColumn = 'BlockTypeId' AND a.EntityTypeQualifierValue = b.BlockTypeId
                WHERE a.[Guid] = '1AD33D1F-BDE9-4AFB-B6E8-E2CD271A5EF3' AND b.[Guid] = 'BA50E899-CEAB-4816-8AD0-A650889882A4'" ); // Set AttributeValue to correct page id         
            // Attrib Value for Block:Icon Button - Listen, Attribute:Icon Page: Sermon Detail, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "BA50E899-CEAB-4816-8AD0-A650889882A4", "FCE07A51-64D8-4FC8-BFB2-8CDBCED449F7", @"fa fa-headphones" );
            // Attrib Value for Block:Icon Button - Listen, Attribute:Enabled Lava Commands Page: Sermon Detail, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "BA50E899-CEAB-4816-8AD0-A650889882A4", "2FCA15F3-D621-4E7C-8F0C-FF9222AB3447", @"" );
            // Attrib Value for Block:Icon Button - Listen, Attribute:Custom Attributes Page: Sermon Detail, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "BA50E899-CEAB-4816-8AD0-A650889882A4", "6D8A7116-BB0F-4F3A-9C24-5C39F2364E42", @"TextColor^#0094d9|BackgroundColor^White" );
            // Attrib Value for Block:Icon Button - Watch, Attribute:Custom Attributes Page: Sermon Detail Audio, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "F915EB5A-9260-4681-9A49-0C6778D2F084", "6D8A7116-BB0F-4F3A-9C24-5C39F2364E42", @"TextColor^#0094d9|BackgroundColor^White" );
            // Attrib Value for Block:Icon Button - Watch, Attribute:Enabled Lava Commands Page: Sermon Detail Audio, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "F915EB5A-9260-4681-9A49-0C6778D2F084", "2FCA15F3-D621-4E7C-8F0C-FF9222AB3447", @"" );
            // Attrib Value for Block:Icon Button - Watch, Attribute:Icon Page: Sermon Detail Audio, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "F915EB5A-9260-4681-9A49-0C6778D2F084", "FCE07A51-64D8-4FC8-BFB2-8CDBCED449F7", @"fa fa-television" );
            // Attrib Value for Block:Icon Button - Watch, Attribute:Text Page: Sermon Detail Audio, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "F915EB5A-9260-4681-9A49-0C6778D2F084", "9FFA6301-04B4-4D1D-907C-D345BE825B13", @"Watch" );
            // Attrib Value for Block:Icon Button - Watch, Attribute:Action Item Page: Sermon Detail Audio, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "F915EB5A-9260-4681-9A49-0C6778D2F084", "1AD33D1F-BDE9-4AFB-B6E8-E2CD271A5EF3", @"2^527^{{ parameter }}" ); // NEED SQL, 527
            Sql( @"DECLARE @PageId int = ( SELECT TOP 1 [Id] FROM [Page] WHERE [Guid] = '1CDE2032-482B-483E-B273-A1A3B421E04C' )
                UPDATE av SET av.[Value] = Replace([Value],527,@PageId)
                FROM [AttributeValue] av 
				INNER JOIN Attribute a ON a.Id = av.AttributeId 
				INNER JOIN Block b ON a.EntityTypeQualifierColumn = 'BlockTypeId' AND a.EntityTypeQualifierValue = b.BlockTypeId
                WHERE a.[Guid] = '1AD33D1F-BDE9-4AFB-B6E8-E2CD271A5EF3' AND b.[Guid] = 'F915EB5A-9260-4681-9A49-0C6778D2F084'" ); // Set AttributeValue to correct page id         
            // Attrib Value for Block:Image Block, Attribute:Enabled Lava Commands Page: Sermon Detail Audio, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "9735B56B-32A3-45CE-9298-5C1F7EFB3DEA", "155B8623-7723-4FD2-A321-B9D24B1611CE", @"RockEntity" );
            // Attrib Value for Block:Image Block, Attribute:Aspect Page: Sermon Detail Audio, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "9735B56B-32A3-45CE-9298-5C1F7EFB3DEA", "1900D9DD-EFA4-4278-8B23-0FEEF1894610", @"" );
            // Attrib Value for Block:Image Block, Attribute:Action Item Page: Sermon Detail Audio, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "9735B56B-32A3-45CE-9298-5C1F7EFB3DEA", "68EF73D8-3C6D-4524-A3A4-9C06D95B164A", @"0" );
            // Attrib Value for Block:Image Block, Attribute:Image Page: Sermon Detail Audio, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "9735B56B-32A3-45CE-9298-5C1F7EFB3DEA", "2E9389F8-0E3C-485D-B722-762D89C8EB2E", @"{% contentchannelitem id:'{{parameter}}' %}{% assign sermonItem = contentchannelitemItems | First -%}{{ 'Global' | Attribute:'PublicApplicationRoot'}}GetImage.ashx?guid={{ sermonItem | Attribute:'Image','RawValue' }}{% endcontentchannelitem %}" );
            // Attrib Value for Block:Image Block, Attribute:Custom Attributes Page: Sermon Detail Audio, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "9735B56B-32A3-45CE-9298-5C1F7EFB3DEA", "B9523EB6-EEE5-4DC4-B2B0-EF7FF0E73F51", @"" );
            // Attrib Value for Block:Preload Block, Attribute:Enabled Lava Commands Page: Avalanche Home Page, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "FCB9F034-0036-48DC-B588-6FBE969938C9", "2811B163-E681-4EBA-9280-A82742FEBE3C", @"" );
            // Attrib Value for Block:Preload Block, Attribute:Pages To Preload Page: Avalanche Home Page, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "FCB9F034-0036-48DC-B588-6FBE969938C9", "73006B75-6B97-4203-8182-0944455BB213", @"519^|520^|521^" ); // NEED SQL
            Sql( @"DECLARE @PageId1 int = ( SELECT TOP 1 [Id] FROM [Page] WHERE [Guid] = '9711DB54-0FB0-4722-AB45-1DFB6158F922' )
                DECLARE @PageId2 int = ( SELECT TOP 1 [Id] FROM [Page] WHERE [Guid] = 'CD8A05F8-24FF-4D38-8ED0-FE2BF07C0CDE' )
                DECLARE @PageId3 int = ( SELECT TOP 1 [Id] FROM [Page] WHERE [Guid] = 'DE6D125E-892E-4F10-A33D-F84942582B1E' )
                UPDATE av SET av.[Value] = Replace(Replace(Replace([Value],521,@PageId3),520,@PageId2),519,@PageId1) 
                FROM [AttributeValue] av 
				INNER JOIN Attribute a ON a.Id = av.AttributeId 
				INNER JOIN Block b ON a.EntityTypeQualifierColumn = 'BlockTypeId' AND a.EntityTypeQualifierValue = b.BlockTypeId
                WHERE a.[Guid] = '73006B75-6B97-4203-8182-0944455BB213' AND b.[Guid] = 'FCB9F034-0036-48DC-B588-6FBE969938C9'" ); // Set AttributeValue to correct page id         
            // Attrib Value for Block:Preload Block, Attribute:Custom Attributes Page: Avalanche Home Page, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "FCB9F034-0036-48DC-B588-6FBE969938C9", "E28D8CD0-91E8-453F-B221-C8A9E60DEE46", @"" );
            // Attrib Value for Block:Preload Block, Attribute:Custom Attributes Page: Connect, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "C6A448AB-B39B-48D1-9EDC-06062BF98766", "E28D8CD0-91E8-453F-B221-C8A9E60DEE46", @"" );
            // Attrib Value for Block:Preload Block, Attribute:Enabled Lava Commands Page: Connect, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "C6A448AB-B39B-48D1-9EDC-06062BF98766", "2811B163-E681-4EBA-9280-A82742FEBE3C", @"" );
            // Attrib Value for Block:Preload Block, Attribute:Pages To Preload Page: Connect, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "C6A448AB-B39B-48D1-9EDC-06062BF98766", "73006B75-6B97-4203-8182-0944455BB213", @"529^|530^|531^" ); // NEED SQL
            Sql( @"DECLARE @PageId1 int = ( SELECT TOP 1 [Id] FROM [Page] WHERE [Guid] = 'D943417B-F167-4CD0-8321-C779B1C9E92B' )
                DECLARE @PageId2 int = ( SELECT TOP 1 [Id] FROM [Page] WHERE [Guid] = '8E94B8E8-171D-4A81-9A69-D34667510231' )
                DECLARE @PageId3 int = ( SELECT TOP 1 [Id] FROM [Page] WHERE [Guid] = 'B95062A4-770C-47DF-B88F-0626F7BFDF2F' )
                UPDATE av SET av.[Value] = Replace(Replace(Replace([Value],531,@PageId3),530,@PageId2),529,@PageId1)
                FROM [AttributeValue] av 
				INNER JOIN Attribute a ON a.Id = av.AttributeId 
				INNER JOIN Block b ON a.EntityTypeQualifierColumn = 'BlockTypeId' AND a.EntityTypeQualifierValue = b.BlockTypeId
                WHERE a.[Guid] = '73006B75-6B97-4203-8182-0944455BB213' AND b.[Guid] = 'C6A448AB-B39B-48D1-9EDC-06062BF98766'" ); // Set AttributeValue to correct page id         
            // Attrib Value for Block:Text Over Image Block - Baptism, Attribute:Custom Attributes Page: Connect, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "DC43E7F1-A294-4D99-B37A-CC04F18BD32C", "B0FCF405-607A-43C1-A2DB-1AA2C066EABB", @"TextColor^White|FontSize^45" );
            // Attrib Value for Block:Text Over Image Block - Baptism, Attribute:Aspect Ratio Page: Connect, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "DC43E7F1-A294-4D99-B37A-CC04F18BD32C", "BD4BC1F9-9C20-4A6E-A67D-8D5EF56A619D", @"0.45" );
            // Attrib Value for Block:Text Over Image Block - Baptism, Attribute:Enabled Lava Commands Page: Connect, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "DC43E7F1-A294-4D99-B37A-CC04F18BD32C", "3B56BD7B-DFFA-4C07-A936-8B79029C01C5", @"" );
            // Attrib Value for Block:Text Over Image Block - Baptism, Attribute:Image Page: Connect, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "DC43E7F1-A294-4D99-B37A-CC04F18BD32C", "B1DD109B-F099-4C88-AEF4-FDA0959B5530", @"{{ 'Global' | Attribute:'PublicApplicationRoot' }}/Content/app/connect_baptism.jpg" );
            // Attrib Value for Block:Text Over Image Block - Baptism, Attribute:Text Page: Connect, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "DC43E7F1-A294-4D99-B37A-CC04F18BD32C", "3C535AE2-5D1D-4634-AFC2-658B4D55A3A8", @"Baptism" );
            // Attrib Value for Block:Text Over Image Block - Baptism, Attribute:Action Item Page: Connect, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "DC43E7F1-A294-4D99-B37A-CC04F18BD32C", "0BF7D627-4BD3-4AE3-9AD7-CFABBA6C9799", @"1^530^" ); // NEED SQL, 530
            Sql( @"DECLARE @PageId int = ( SELECT TOP 1 [Id] FROM [Page] WHERE [Guid] = '8E94B8E8-171D-4A81-9A69-D34667510231' )
                UPDATE av SET av.[Value] = Replace([Value],530,@PageId)
                FROM [AttributeValue] av 
				INNER JOIN Attribute a ON a.Id = av.AttributeId 
				INNER JOIN Block b ON a.EntityTypeQualifierColumn = 'BlockTypeId' AND a.EntityTypeQualifierValue = b.BlockTypeId
                WHERE a.[Guid] = '0BF7D627-4BD3-4AE3-9AD7-CFABBA6C9799' AND b.[Guid] = 'DC43E7F1-A294-4D99-B37A-CC04F18BD32C'" ); // Set AttributeValue to correct page id         
            // Attrib Value for Block:Text Over Image Block - Groups, Attribute:Action Item Page: Connect, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "44D7ED68-22F4-4D1F-B443-75FF6B9F791B", "0BF7D627-4BD3-4AE3-9AD7-CFABBA6C9799", @"1^529^" ); // NEED SQL, 529
            Sql( @"DECLARE @PageId int = ( SELECT TOP 1 [Id] FROM [Page] WHERE [Guid] = 'D943417B-F167-4CD0-8321-C779B1C9E92B' )
                UPDATE av SET av.[Value] = Replace([Value],529,@PageId)
                FROM [AttributeValue] av 
				INNER JOIN Attribute a ON a.Id = av.AttributeId 
				INNER JOIN Block b ON a.EntityTypeQualifierColumn = 'BlockTypeId' AND a.EntityTypeQualifierValue = b.BlockTypeId
                WHERE a.[Guid] = '0BF7D627-4BD3-4AE3-9AD7-CFABBA6C9799' AND b.[Guid] = '44D7ED68-22F4-4D1F-B443-75FF6B9F791B'" ); // Set AttributeValue to correct page id         
            // Attrib Value for Block:Text Over Image Block - Groups, Attribute:Text Page: Connect, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "44D7ED68-22F4-4D1F-B443-75FF6B9F791B", "3C535AE2-5D1D-4634-AFC2-658B4D55A3A8", @"Groups" );
            // Attrib Value for Block:Text Over Image Block - Groups, Attribute:Image Page: Connect, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "44D7ED68-22F4-4D1F-B443-75FF6B9F791B", "B1DD109B-F099-4C88-AEF4-FDA0959B5530", @"{{ 'Global' | Attribute:'PublicApplicationRoot' }}/Content/app/connect_groups.jpg" );
            // Attrib Value for Block:Text Over Image Block - Groups, Attribute:Enabled Lava Commands Page: Connect, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "44D7ED68-22F4-4D1F-B443-75FF6B9F791B", "3B56BD7B-DFFA-4C07-A936-8B79029C01C5", @"" );
            // Attrib Value for Block:Text Over Image Block - Groups, Attribute:Custom Attributes Page: Connect, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "44D7ED68-22F4-4D1F-B443-75FF6B9F791B", "B0FCF405-607A-43C1-A2DB-1AA2C066EABB", @"TextColor^White|FontSize^45" );
            // Attrib Value for Block:Text Over Image Block - Groups, Attribute:Aspect Ratio Page: Connect, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "44D7ED68-22F4-4D1F-B443-75FF6B9F791B", "BD4BC1F9-9C20-4A6E-A67D-8D5EF56A619D", @"0.45" );
            // Attrib Value for Block:Text Over Image Block - Events, Attribute:Aspect Ratio Page: Connect, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "E7E0ADD1-0863-492E-B7D7-A32B5680816F", "BD4BC1F9-9C20-4A6E-A67D-8D5EF56A619D", @"0.45" );
            // Attrib Value for Block:Text Over Image Block - Events, Attribute:Custom Attributes Page: Connect, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "E7E0ADD1-0863-492E-B7D7-A32B5680816F", "B0FCF405-607A-43C1-A2DB-1AA2C066EABB", @"TextColor^White|FontSize^45" );
            // Attrib Value for Block:Text Over Image Block - Events, Attribute:Enabled Lava Commands Page: Connect, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "E7E0ADD1-0863-492E-B7D7-A32B5680816F", "3B56BD7B-DFFA-4C07-A936-8B79029C01C5", @"" );
            // Attrib Value for Block:Text Over Image Block - Events, Attribute:Image Page: Connect, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "E7E0ADD1-0863-492E-B7D7-A32B5680816F", "B1DD109B-F099-4C88-AEF4-FDA0959B5530", @"{{ 'Global' | Attribute:'PublicApplicationRoot' }}/Content/app/connect_events.jpg" );
            // Attrib Value for Block:Text Over Image Block - Events, Attribute:Text Page: Connect, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "E7E0ADD1-0863-492E-B7D7-A32B5680816F", "3C535AE2-5D1D-4634-AFC2-658B4D55A3A8", @"Events" );
            // Attrib Value for Block:Text Over Image Block - Events, Attribute:Action Item Page: Connect, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "E7E0ADD1-0863-492E-B7D7-A32B5680816F", "0BF7D627-4BD3-4AE3-9AD7-CFABBA6C9799", @"1^531^" ); // NEED SQL, 531
            Sql( @"DECLARE @PageId int = ( SELECT TOP 1 [Id] FROM [Page] WHERE [Guid] = 'B95062A4-770C-47DF-B88F-0626F7BFDF2F' )
                UPDATE av SET av.[Value] = Replace([Value],531,@PageId)
                FROM [AttributeValue] av 
				INNER JOIN Attribute a ON a.Id = av.AttributeId 
				INNER JOIN Block b ON a.EntityTypeQualifierColumn = 'BlockTypeId' AND a.EntityTypeQualifierValue = b.BlockTypeId
                WHERE a.[Guid] = '0BF7D627-4BD3-4AE3-9AD7-CFABBA6C9799' AND b.[Guid] = 'E7E0ADD1-0863-492E-B7D7-A32B5680816F'" ); // Set AttributeValue to correct page id         
            // Attrib Value for Block:Text Over Image Block, Attribute:Action Item Page: Groups, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "5C6A4458-56A3-4F60-8792-5271326DED00", "0BF7D627-4BD3-4AE3-9AD7-CFABBA6C9799", @"0" );
            // Attrib Value for Block:Text Over Image Block, Attribute:Image Page: Groups, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "5C6A4458-56A3-4F60-8792-5271326DED00", "B1DD109B-F099-4C88-AEF4-FDA0959B5530", @"{{ 'Global' | Attribute:'PublicApplicationRoot' }}/Content/app/groups.jpg" );
            // Attrib Value for Block:Text Over Image Block, Attribute:Text Page: Groups, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "5C6A4458-56A3-4F60-8792-5271326DED00", "3C535AE2-5D1D-4634-AFC2-658B4D55A3A8", @"Groups" );
            // Attrib Value for Block:Text Over Image Block, Attribute:Enabled Lava Commands Page: Groups, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "5C6A4458-56A3-4F60-8792-5271326DED00", "3B56BD7B-DFFA-4C07-A936-8B79029C01C5", @"" );
            // Attrib Value for Block:Text Over Image Block, Attribute:Custom Attributes Page: Groups, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "5C6A4458-56A3-4F60-8792-5271326DED00", "B0FCF405-607A-43C1-A2DB-1AA2C066EABB", @"TextColor^White|FontSize^45" );
            // Attrib Value for Block:Text Over Image Block, Attribute:Aspect Ratio Page: Groups, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "5C6A4458-56A3-4F60-8792-5271326DED00", "BD4BC1F9-9C20-4A6E-A67D-8D5EF56A619D", @"0.35" );
            // Attrib Value for Block:Markdown Detail, Attribute:Enabled Lava Commands Page: Groups, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "898CD5D8-AB1E-4267-8084-161FFBAA7820", "7A9950AC-B9E7-4200-9B7A-634BF90DDB2B", @"RockEntity" );
            // Attrib Value for Block:Markdown Detail, Attribute:Custom Attributes Page: Groups, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "898CD5D8-AB1E-4267-8084-161FFBAA7820", "538EE4EC-33C5-4997-ABD6-1F105F78EF38", @"Margin^10" );
            // Attrib Value for Block:Markdown Detail, Attribute:Markdown Page: Groups, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "898CD5D8-AB1E-4267-8084-161FFBAA7820", "9CB3C25B-815D-44A4-9171-698136CC0988", @"## Lorem ipsum dolor sit amet
Lorem ipsum dolor sit amet, consectetur adipiscing elit. Quisque mattis leo a eros accumsan elementum. Vestibulum iaculis ligula et molestie tempus. Nunc vel elit et orci vulputate ornare. Phasellus eleifend dapibus quam, vitae mollis tortor molestie eget. Vestibulum urna est, condimentum nec pulvinar at, semper malesuada elit. 

Praesent ornare sapien vel nibh viverra, ac aliquet nibh sagittis. Nunc congue commodo tortor, sed interdum tortor viverra vel. In enim risus, volutpat ut augue non, condimentum suscipit mi. Proin condimentum, nisi ac placerat maximus, erat felis varius sapien, in fringilla enim metus vel sem. Pellentesque et fringilla ex, vel semper sapien. Vivamus condimentum lectus nec tincidunt pulvinar. Quisque mattis erat vel eleifend luctus. Sed vitae bibendum neque. Cras consectetur gravida leo, facilisis posuere purus tristique nec. Ut vitae diam et ipsum venenatis venenatis eu sed mauris. Cras fermentum purus eu suscipit tempor." );
            // Attrib Value for Block:Button - Attend Growth Track, Attribute:Text Page: Groups, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "6B2D9BCB-6229-4859-BA77-9E4BF34AE181", "B60F03CA-B109-48F6-ACE4-1C365DC4E908", @"Learn More" );
            // Attrib Value for Block:Button - Attend Growth Track, Attribute:Action Item Page: Groups, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "6B2D9BCB-6229-4859-BA77-9E4BF34AE181", "A9D5A12D-F60D-4CAA-A46B-1A605032586D", @"4^{{ 'Global' | Attribute:'PublicApplicationRoot' }}/Connect^0" );
            // Attrib Value for Block:Button - Attend Growth Track, Attribute:Enabled Lava Commands Page: Groups, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "6B2D9BCB-6229-4859-BA77-9E4BF34AE181", "BEF7E221-837D-4DD5-BC3E-49E6E3BDEBF9", @"" );
            // Attrib Value for Block:Button - Attend Growth Track, Attribute:Custom Attributes Page: Groups, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "6B2D9BCB-6229-4859-BA77-9E4BF34AE181", "FED19184-4169-4EEF-9437-592545A79461", @"BackgroundColor^#0094d9|TextColor^White|BorderRadius^20|FontSize^24|Margin^30%2C10" );
            // Attrib Value for Block:Next Steps detail, Attribute:Custom Attributes Page: Groups, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "87B3FD61-73E7-4BEF-AC56-1A96E497E916", "538EE4EC-33C5-4997-ABD6-1F105F78EF38", @"Margin^10" );
            // Attrib Value for Block:Next Steps detail, Attribute:Enabled Lava Commands Page: Groups, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "87B3FD61-73E7-4BEF-AC56-1A96E497E916", "7A9950AC-B9E7-4200-9B7A-634BF90DDB2B", @"" );
            // Attrib Value for Block:Next Steps detail, Attribute:Markdown Page: Groups, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "87B3FD61-73E7-4BEF-AC56-1A96E497E916", "9CB3C25B-815D-44A4-9171-698136CC0988", @"Curabitur imperdiet bibendum dui, gravida sollicitudin mauris pharetra eget. Quisque vestibulum arcu sed nulla porttitor aliquet. Fusce vel interdum diam [Next Steps page]({{ 'Global' | Attribute:'PublicApplicationRoot' }}/Connect). Nullam purus dolor, mattis at pellentesque quis, consectetur in arcu. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Fusce vel tellus nibh.

### Nam semper facilisis ligula ut consequat
Sed quis tincidunt ex, ut sagittis erat. Donec congue ultrices mauris ut consectetur. Nulla urna nisl, pretium et mauris ac, tempus tristique augue. Morbi sodales egestas magna quis convallis. Pellentesque interdum tincidunt sollicitudin. Cras mattis eu arcu ultrices porta. Etiam eleifend viverra nulla, eget porta turpis mattis nec. Nullam faucibus maximus est, a rutrum metus fringilla vulputate." );
            // Attrib Value for Block:Next Steps Button, Attribute:Action Item Page: Groups, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "D0B758C4-18E1-45DE-82B0-70906E45B99A", "A9D5A12D-F60D-4CAA-A46B-1A605032586D", @"1^532^0" ); // NEED SQL, workflow form page
            Sql( @"DECLARE @PageId int = ( SELECT TOP 1 [Id] FROM [Page] WHERE [Guid] = '7817298D-0A76-4039-A5D7-AF273AC05952' )
                UPDATE av SET av.[Value] = Replace([Value],'532',@PageId)
                FROM [AttributeValue] av 
				INNER JOIN Attribute a ON a.Id = av.AttributeId 
				INNER JOIN Block b ON a.EntityTypeQualifierColumn = 'BlockTypeId' AND a.EntityTypeQualifierValue = b.BlockTypeId
                WHERE a.[Guid] = 'A9D5A12D-F60D-4CAA-A46B-1A605032586D' AND b.[Guid] = 'D0B758C4-18E1-45DE-82B0-70906E45B99A'" ); // Set AttributeValue to correct page id         
            // Attrib Value for Block:Next Steps Button, Attribute:Text Page: Groups, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "D0B758C4-18E1-45DE-82B0-70906E45B99A", "B60F03CA-B109-48F6-ACE4-1C365DC4E908", @"Form a group" );
            // Attrib Value for Block:Next Steps Button, Attribute:Custom Attributes Page: Groups, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "D0B758C4-18E1-45DE-82B0-70906E45B99A", "FED19184-4169-4EEF-9437-592545A79461", @"TextColor^White|BackgroundColor^#0094d9|BorderRadius^20|Margin^30%2C10|FontSize^24" );
            // Attrib Value for Block:Next Steps Button, Attribute:Enabled Lava Commands Page: Groups, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "D0B758C4-18E1-45DE-82B0-70906E45B99A", "BEF7E221-837D-4DD5-BC3E-49E6E3BDEBF9", @"" );
            // Attrib Value for Block:Mobile Workflow, Attribute:Workflow Type Page: Contact Us, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "F05B2A93-CEF8-4EAE-9358-EACC73C31173", "3667DECB-4576-46A5-B2CE-ACC2D43D0D69", @"236ab611-ede8-42b5-b559-6b6a88adddcb" );
            // Attrib Value for Block:Mobile Workflow, Attribute:Custom Attributes Page: Contact Us, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "F05B2A93-CEF8-4EAE-9358-EACC73C31173", "96FBBEC4-EDDE-4D98-8B3D-BF656CB4B563", @"" );
            // Attrib Value for Block:Avalanche Event Calendar Lava, Attribute:Cache Duration Page: Events, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "713083B7-B5A8-416C-9603-594589A67B8D", "4D9B40B4-14E2-4B00-9725-0D33B6606BD1", @"0" );
            // Attrib Value for Block:Avalanche Event Calendar Lava, Attribute:Event Calendar Page: Events, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "713083B7-B5A8-416C-9603-594589A67B8D", "29E37C6C-CCD8-4631-A2B5-200D93678D7A", @"8a444668-19af-4417-9c74-09f842572974" );
            // Attrib Value for Block:Avalanche Event Calendar Lava, Attribute:Default View Option Page: Events, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "713083B7-B5A8-416C-9603-594589A67B8D", "4BC3868B-23BB-4A46-970F-BD16447F705E", @"Year" );
            // Attrib Value for Block:Avalanche Event Calendar Lava, Attribute:Enabled Lava Commands Page: Events, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "713083B7-B5A8-416C-9603-594589A67B8D", "1AD0C6A7-16D4-4DBA-9584-342137D3E051", @"" );
            // Attrib Value for Block:Avalanche Event Calendar Lava, Attribute:Campus Filter Display Mode Page: Events, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "713083B7-B5A8-416C-9603-594589A67B8D", "7FBB51A6-B2C7-4E1D-ABAF-F4D01ABF1D8F", @"2" );
            // Attrib Value for Block:Avalanche Event Calendar Lava, Attribute:Audience Filter Display Mode Page: Events, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "713083B7-B5A8-416C-9603-594589A67B8D", "95AF549B-D25C-49BE-8B0F-59C3F3D1B4D4", @"2" );
            // Attrib Value for Block:Avalanche Event Calendar Lava, Attribute:Filter Audiences Page: Events, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "713083B7-B5A8-416C-9603-594589A67B8D", "9F67474F-B6DF-4549-884D-F89E56AA954F", @"" );
            // Attrib Value for Block:Avalanche Event Calendar Lava, Attribute:Show Date Range Filter Page: Events, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "713083B7-B5A8-416C-9603-594589A67B8D", "04992B21-617D-48A1-ADF3-951CB633CC93", @"False" );
            // Attrib Value for Block:Avalanche Event Calendar Lava, Attribute:Show Small Calendar Page: Events, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "713083B7-B5A8-416C-9603-594589A67B8D", "197DA2B8-F798-44BF-B2C2-99723A3E2E45", @"True" );
            // Attrib Value for Block:Avalanche Event Calendar Lava, Attribute:Show Day View Page: Events, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "713083B7-B5A8-416C-9603-594589A67B8D", "3692C511-DFBF-4F5A-8BC0-16EAFC1B4BAC", @"False" );
            // Attrib Value for Block:Avalanche Event Calendar Lava, Attribute:Show Week View Page: Events, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "713083B7-B5A8-416C-9603-594589A67B8D", "15084637-2667-4260-8E50-9B01EA852ADF", @"True" );
            // Attrib Value for Block:Avalanche Event Calendar Lava, Attribute:Show Month View Page: Events, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "713083B7-B5A8-416C-9603-594589A67B8D", "42744616-A525-4252-8CEE-9CDE6250DD31", @"True" );
            // Attrib Value for Block:Avalanche Event Calendar Lava, Attribute:Show Year View Page: Events, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "713083B7-B5A8-416C-9603-594589A67B8D", "B7A1B651-9CC9-4961-AB8C-5E89FA7F19B2", @"True" );
            // Attrib Value for Block:Avalanche Event Calendar Lava, Attribute:Enable Campus Context Page: Events, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "713083B7-B5A8-416C-9603-594589A67B8D", "13564317-66A2-4EE2-9078-5B314E8EB249", @"False" );
            // Attrib Value for Block:Avalanche Event Calendar Lava, Attribute:Action Item Page: Events, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "713083B7-B5A8-416C-9603-594589A67B8D", "FD94A70E-EAC2-4851-9D3C-84629A93F62E", @"0" );
            // Attrib Value for Block:Avalanche Event Calendar Lava, Attribute:Campus Parameter Name Page: Events, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "713083B7-B5A8-416C-9603-594589A67B8D", "32ADBCD2-7858-4566-9AAE-4C5DC5C43246", @"campusId" );
            // Attrib Value for Block:Avalanche Event Calendar Lava, Attribute:Start of Week Day Page: Events, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "713083B7-B5A8-416C-9603-594589A67B8D", "A016AF17-6C91-49BD-AC92-C4B83A9BFE46", @"0" );
            // Attrib Value for Block:Avalanche Event Calendar Lava, Attribute:Set Page Title Page: Events, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "713083B7-B5A8-416C-9603-594589A67B8D", "B5EA1BE1-DE1D-4C9E-B565-A3050869CD58", @"False" );
            // Attrib Value for Block:Avalanche Event Calendar Lava, Attribute:Component Page: Events, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "713083B7-B5A8-416C-9603-594589A67B8D", "2A037292-80ED-4EF6-95E6-A3518EFFDC2C", @"a6efb571-56c8-44c2-8f87-b7f4db4e1991" );
            // Attrib Value for Block:Avalanche Event Calendar Lava, Attribute:Category Parameter Name Page: Events, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "713083B7-B5A8-416C-9603-594589A67B8D", "EC03AFD7-A544-4BEB-9CD8-5A87FC90B6B0", @"categoryId" );
            // Attrib Value for Block:Avalanche Event Calendar Lava, Attribute:Lava Template Page: Events, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "713083B7-B5A8-416C-9603-594589A67B8D", "71BFE3A7-41CD-46E8-B95F-456735104DA2", @"[{% assign eventItemOccurrenceCount = EventItemOccurrences | Size -%}
{% if eventItemOccurrenceCount == 0 -%}
{""Id"":""-1"", ""Title"":""There are no events in this time frame."", ""Description"":"""", ""Resource"":"""", ""ActionType"":""0"", ""Image"":""""}
{% endif -%}{% for eventItemOccurrence in EventItemOccurrences -%}
{""Id"":""{{ eventItemOccurrence.EventItemOccurrence.Id }}"", ""Title"":""{{ eventItemOccurrence.Name | Replace:'""','\""' }}"", ""Description"":""{{ eventItemOccurrence.Date | Date:'MMM d' }} - {{ eventItemOccurrence.Summary | Replace:'""','\""' }}"", ""Resource"":""533"", ""ActionType"":""1"", ""Image"":""{{ 'Global' | Attribute:'PublicApplicationRoot' }}/GetImage.ashx?id={{ eventItemOccurrence.EventItemOccurrence.EventItem.PhotoId }}&w=720""},
{% endfor -%}]" ); // NEED SQL, 533
            Sql( @"DECLARE @PageId int = ( SELECT TOP 1 [Id] FROM [Page] WHERE [Guid] = '14A25533-186B-49C4-A949-0F9DA864A87E' )
                UPDATE av SET av.[Value] = Replace([Value],533,@PageId)
                FROM [AttributeValue] av 
				INNER JOIN Attribute a ON a.Id = av.AttributeId 
				INNER JOIN Block b ON a.EntityTypeQualifierColumn = 'BlockTypeId' AND a.EntityTypeQualifierValue = b.BlockTypeId
                WHERE a.[Guid] = '71BFE3A7-41CD-46E8-B95F-456735104DA2' AND b.[Guid] = '713083B7-B5A8-416C-9603-594589A67B8D'" ); // Set AttributeValue to correct page id         
            // Attrib Value for Block:Avalanche Event Calendar Lava, Attribute:Custom Attributes Page: Events, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "713083B7-B5A8-416C-9603-594589A67B8D", "0B61EA06-A713-4359-BCA4-1D0602E8C6CA", @"Columns^1|Margin^20%2C0|WidthRequest^440" );
            // Attrib Value for Block:Text Over Image Block, Attribute:Aspect Ratio Page: Events, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "FB586E92-764C-4321-871F-47C38D768E94", "BD4BC1F9-9C20-4A6E-A67D-8D5EF56A619D", @"0.45" );
            // Attrib Value for Block:Text Over Image Block, Attribute:Custom Attributes Page: Events, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "FB586E92-764C-4321-871F-47C38D768E94", "B0FCF405-607A-43C1-A2DB-1AA2C066EABB", @"TextColor^White|FontSize^45" );
            // Attrib Value for Block:Text Over Image Block, Attribute:Enabled Lava Commands Page: Events, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "FB586E92-764C-4321-871F-47C38D768E94", "3B56BD7B-DFFA-4C07-A936-8B79029C01C5", @"" );
            // Attrib Value for Block:Text Over Image Block, Attribute:Text Page: Events, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "FB586E92-764C-4321-871F-47C38D768E94", "3C535AE2-5D1D-4634-AFC2-658B4D55A3A8", @"Events" );
            // Attrib Value for Block:Text Over Image Block, Attribute:Image Page: Events, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "FB586E92-764C-4321-871F-47C38D768E94", "B1DD109B-F099-4C88-AEF4-FDA0959B5530", @"{{ 'Global' | Attribute:'PublicApplicationRoot' }}/Content/app/events.jpg" );
            // Attrib Value for Block:Text Over Image Block, Attribute:Action Item Page: Events, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "FB586E92-764C-4321-871F-47C38D768E94", "0BF7D627-4BD3-4AE3-9AD7-CFABBA6C9799", @"0" );
            // Attrib Value for Block:Event Filter, Attribute:Action Item Page: Events, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "A9D36E34-4D02-4179-B6DE-EF30B341D1D9", "CBB79612-83F9-4FC5-94EC-450DED06B30B", @"2^531^" ); // NEED SQL, 531
            Sql( @"DECLARE @PageId int = ( SELECT TOP 1 [Id] FROM [Page] WHERE [Guid] = 'B95062A4-770C-47DF-B88F-0626F7BFDF2F' )
                UPDATE av SET av.[Value] = Replace([Value],531,@PageId)
                FROM [AttributeValue] av 
				INNER JOIN Attribute a ON a.Id = av.AttributeId 
				INNER JOIN Block b ON a.EntityTypeQualifierColumn = 'BlockTypeId' AND a.EntityTypeQualifierValue = b.BlockTypeId
                WHERE a.[Guid] = 'CBB79612-83F9-4FC5-94EC-450DED06B30B' AND b.[Guid] = 'A9D36E34-4D02-4179-B6DE-EF30B341D1D9'" ); // Set AttributeValue to correct page id         
            // Attrib Value for Block:Event Filter, Attribute:Custom Attributes Page: Events, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "A9D36E34-4D02-4179-B6DE-EF30B341D1D9", "92E11239-D329-4E43-9847-72358122D052", @"Margin^10%2C0" );
            // Attrib Value for Block:Image Block, Attribute:Enabled Lava Commands Page: Event Detail, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "BAAA075C-AD3F-4A0B-AF0C-D9462760D16F", "155B8623-7723-4FD2-A321-B9D24B1611CE", @"RockEntity" );
            // Attrib Value for Block:Image Block, Attribute:Action Item Page: Event Detail, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "BAAA075C-AD3F-4A0B-AF0C-D9462760D16F", "68EF73D8-3C6D-4524-A3A4-9C06D95B164A", @"0" );
            // Attrib Value for Block:Image Block, Attribute:Image Page: Event Detail, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "BAAA075C-AD3F-4A0B-AF0C-D9462760D16F", "2E9389F8-0E3C-485D-B722-762D89C8EB2E", @"{% eventitemoccurrence id:'{{parameter}}' %}{{ 'Global' | Attribute:'PublicApplicationRoot' }}/GetImage.ashx?id={{ eventitemoccurrence.EventItem.PhotoId }}{% endeventitemoccurrence %}" );
            // Attrib Value for Block:Image Block, Attribute:Aspect Page: Event Detail, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "BAAA075C-AD3F-4A0B-AF0C-D9462760D16F", "1900D9DD-EFA4-4278-8B23-0FEEF1894610", @"" );
            // Attrib Value for Block:Image Block, Attribute:Custom Attributes Page: Event Detail, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "BAAA075C-AD3F-4A0B-AF0C-D9462760D16F", "B9523EB6-EEE5-4DC4-B2B0-EF7FF0E73F51", @"" );
            // Attrib Value for Block:Label Block, Attribute:Text Page: Event Detail, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "897FE435-F9E8-4755-AC87-5B273A415DCD", "95980EF8-47FA-40C9-9372-938F46746458", @"{% eventitemoccurrence id:'{{ parameter }}' %}{{ eventitemoccurrence.EventItem.Name }}{% endeventitemoccurrence %}" );
            // Attrib Value for Block:Label Block, Attribute:Custom Attributes Page: Event Detail, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "897FE435-F9E8-4755-AC87-5B273A415DCD", "A59E551E-6B04-4C70-B1F6-B8EAEAC27D94", @"FontSize^24|TextColor^#0094d9|FontFamily^Open Sans Light|Margin^10" );
            // Attrib Value for Block:Label Block, Attribute:Enabled Lava Commands Page: Event Detail, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "897FE435-F9E8-4755-AC87-5B273A415DCD", "7F860DC6-7478-4F88-A878-FAF3D75CEB9C", @"RockEntity" );
            // Attrib Value for Block:Label Block, Attribute:Action Item Page: Event Detail, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "897FE435-F9E8-4755-AC87-5B273A415DCD", "0D9816B1-E6CF-46C3-A3C7-4099359B2857", @"0" );
            // Attrib Value for Block:Markdown Detail, Attribute:Markdown Page: Event Detail, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "10F85187-7796-45FC-A65D-4EC1314E3609", "9CB3C25B-815D-44A4-9171-698136CC0988", @"{% eventitemoccurrence id:'{{ parameter }}' -%}{% assign icaldate = eventitemoccurrence.Schedule.iCalendarContent | DatesFromICal:'all' | First -%}
{% capture datestr %}{% if eventitemoccurrence.Schedule.EffectiveStartDate != eventitemoccurrence.Schedule.EffectiveEndDate -%}
{{ eventitemoccurrence.Schedule.EffectiveStartDate  | Date: 'MMMM d' }} - {{ eventitemoccurrence.Schedule.EffectiveEndDate  | Date: 'MMMM d'}}
{% else -%}
{{ icaldate | Date:'dddd, MMMM d, h:mm tt' }}
{% endif %}{% endcapture %}### {{ datestr }}  
**  ** 
{{ eventitemoccurrence.EventItem.Description }}
{% if eventitemoccurrence.Location != '' -%}

**Location:** {{ eventitemoccurrence.Location }}  
{% endif -%}
{% if eventitemoccurrence.Note != '' -%}   
**Note:**   
{{ eventitemoccurrence.Note | HtmlToMarkdown }}  
{% endif -%}{% endeventitemoccurrence -%}" );
            // Attrib Value for Block:Markdown Detail, Attribute:Custom Attributes Page: Event Detail, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "10F85187-7796-45FC-A65D-4EC1314E3609", "538EE4EC-33C5-4997-ABD6-1F105F78EF38", @"Margin^10%2C0" );
            // Attrib Value for Block:Markdown Detail, Attribute:Enabled Lava Commands Page: Event Detail, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "10F85187-7796-45FC-A65D-4EC1314E3609", "7A9950AC-B9E7-4200-9B7A-634BF90DDB2B", @"RockEntity" );
            // Attrib Value for Block:Mobile Button, Attribute:Text Page: Event Detail, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "C454F889-AB57-4D38-A18F-195D60445EDF", "B60F03CA-B109-48F6-ACE4-1C365DC4E908", @"More Info" );
            // Attrib Value for Block:Mobile Button, Attribute:Action Item Page: Event Detail, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "C454F889-AB57-4D38-A18F-195D60445EDF", "A9D5A12D-F60D-4CAA-A46B-1A605032586D", @"4^{{ 'Global' | Attribute:'PublicApplicationRoot' }}/page/414?EventOccurrenceID={{ parameter }}^0" ); 
            // Attrib Value for Block:Mobile Button, Attribute:Enabled Lava Commands Page: Event Detail, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "C454F889-AB57-4D38-A18F-195D60445EDF", "BEF7E221-837D-4DD5-BC3E-49E6E3BDEBF9", @"RockEntity" );
            // Attrib Value for Block:Mobile Button, Attribute:Custom Attributes Page: Event Detail, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "C454F889-AB57-4D38-A18F-195D60445EDF", "FED19184-4169-4EEF-9437-592545A79461", @"BorderRadius^20|TextColor^White|FontSize^24|BackgroundColor^#0094d9|Margin^20" );
            // Attrib Value for Block:Text Over Image Block, Attribute:Enabled Lava Commands Page: Baptism, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "F6B935B9-6DA2-421A-9645-F9EF7B432E8C", "3B56BD7B-DFFA-4C07-A936-8B79029C01C5", @"" );
            // Attrib Value for Block:Text Over Image Block, Attribute:Custom Attributes Page: Baptism, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "F6B935B9-6DA2-421A-9645-F9EF7B432E8C", "B0FCF405-607A-43C1-A2DB-1AA2C066EABB", @"TextColor^White|FontSize^45" );
            // Attrib Value for Block:Text Over Image Block, Attribute:Aspect Ratio Page: Baptism, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "F6B935B9-6DA2-421A-9645-F9EF7B432E8C", "BD4BC1F9-9C20-4A6E-A67D-8D5EF56A619D", @"0.35" );
            // Attrib Value for Block:Text Over Image Block, Attribute:Action Item Page: Baptism, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "F6B935B9-6DA2-421A-9645-F9EF7B432E8C", "0BF7D627-4BD3-4AE3-9AD7-CFABBA6C9799", @"0" );
            // Attrib Value for Block:Text Over Image Block, Attribute:Image Page: Baptism, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "F6B935B9-6DA2-421A-9645-F9EF7B432E8C", "B1DD109B-F099-4C88-AEF4-FDA0959B5530", @"{{ 'Global' | Attribute:'PublicApplicationRoot' }}/Content/app/parallax-baptism.jpg" );
            // Attrib Value for Block:Text Over Image Block, Attribute:Text Page: Baptism, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "F6B935B9-6DA2-421A-9645-F9EF7B432E8C", "3C535AE2-5D1D-4634-AFC2-658B4D55A3A8", @"Baptism" );
            // Attrib Value for Block:Markdown Detail, Attribute:Custom Attributes Page: Baptism, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "010B3F1F-2149-4DBA-ABB3-1DCF795C4C89", "538EE4EC-33C5-4997-ABD6-1F105F78EF38", @"Margin^10" );
            // Attrib Value for Block:Markdown Detail, Attribute:Markdown Page: Baptism, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "010B3F1F-2149-4DBA-ABB3-1DCF795C4C89", "9CB3C25B-815D-44A4-9171-698136CC0988", @"### Pellentesque habitant morbi tristique senectus

Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Suspendisse in arcu sodales felis dignissim accumsan. Sed interdum laoreet nulla. Duis laoreet scelerisque elit eu venenatis. 

Donec ultricies consequat nibh vitae vulputate. Sed vitae nunc in erat maximus feugiat sed et diam. Morbi risus nunc, placerat id ligula nec, bibendum volutpat quam. Maecenas faucibus orci ante, vel sollicitudin enim finibus quis. Suspendisse potenti. 

Aliquam massa eros, tincidunt vel enim eu, imperdiet tristique felis. Phasellus vel erat ullamcorper, fringilla urna a, maximus odio. Donec ornare arcu ut ligula cursus sagittis. Vivamus id sollicitudin ex, et rhoncus massa. Sed urna metus, ultricies venenatis nibh ac, consequat finibus eros." );
            // Attrib Value for Block:Markdown Detail, Attribute:Enabled Lava Commands Page: Baptism, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "010B3F1F-2149-4DBA-ABB3-1DCF795C4C89", "7A9950AC-B9E7-4200-9B7A-634BF90DDB2B", @"" );
            // Attrib Value for Block:Button, Attribute:Text Page: Baptism, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "9B44C218-D744-43AC-B4A0-9DBD7C78911D", "B60F03CA-B109-48F6-ACE4-1C365DC4E908", @"Learn More" );
            // Attrib Value for Block:Button, Attribute:Action Item Page: Baptism, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "9B44C218-D744-43AC-B4A0-9DBD7C78911D", "A9D5A12D-F60D-4CAA-A46B-1A605032586D", @"4^{{ 'Global' | Attribute:'PublicApplicationRoot' }}/Baptism^0" );
            // Attrib Value for Block:Button, Attribute:Enabled Lava Commands Page: Baptism, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "9B44C218-D744-43AC-B4A0-9DBD7C78911D", "BEF7E221-837D-4DD5-BC3E-49E6E3BDEBF9", @"" );
            // Attrib Value for Block:Button, Attribute:Custom Attributes Page: Baptism, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "9B44C218-D744-43AC-B4A0-9DBD7C78911D", "FED19184-4169-4EEF-9437-592545A79461", @"TextColor^White|FontSize^24|BorderRadius^20|BackgroundColor^#0094d9|Margin^20%2C0" );
            // Attrib Value for Block:Markdown Detail, Attribute:Custom Attributes Page: Baptism, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "6AA0CB59-E4BA-4E74-8198-749EEB553835", "538EE4EC-33C5-4997-ABD6-1F105F78EF38", @"Margin^10" );
            // Attrib Value for Block:Markdown Detail, Attribute:Markdown Page: Baptism, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "6AA0CB59-E4BA-4E74-8198-749EEB553835", "9CB3C25B-815D-44A4-9171-698136CC0988", @"If you attended week one of _Growth Track_ and weren't ready to be baptized, but would like to do so now—visit our [Next Steps page]({{ 'Global' | Attribute:'PublicApplicationRoot' }}/NextSteps). Someone from our team would love to help you out!" );
            // Attrib Value for Block:Markdown Detail, Attribute:Enabled Lava Commands Page: Baptism, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "6AA0CB59-E4BA-4E74-8198-749EEB553835", "7A9950AC-B9E7-4200-9B7A-634BF90DDB2B", @"" );
            // Attrib Value for Block:Text Over Image Block - Serve, Attribute:Action Item Page: Connect, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "2901482E-A8C5-4A47-8EEB-2D3C9E0EA407", "0BF7D627-4BD3-4AE3-9AD7-CFABBA6C9799", @"1^534^" ); // NEED SQL, 534
            Sql( @"DECLARE @PageId int = ( SELECT TOP 1 [Id] FROM [Page] WHERE [Guid] = 'C90B1E1B-EC24-49E0-9562-7F92BB2D24AB' )
                UPDATE av SET av.[Value] = Replace([Value],534,@PageId)
                FROM [AttributeValue] av 
				INNER JOIN Attribute a ON a.Id = av.AttributeId 
				INNER JOIN Block b ON a.EntityTypeQualifierColumn = 'BlockTypeId' AND a.EntityTypeQualifierValue = b.BlockTypeId
                WHERE a.[Guid] = '0BF7D627-4BD3-4AE3-9AD7-CFABBA6C9799' AND b.[Guid] = '2901482E-A8C5-4A47-8EEB-2D3C9E0EA407'" ); // Set AttributeValue to correct page id         

            // Attrib Value for Block:Text Over Image Block - Serve, Attribute:Image Page: Connect, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "2901482E-A8C5-4A47-8EEB-2D3C9E0EA407", "B1DD109B-F099-4C88-AEF4-FDA0959B5530", @"{{ 'Global' | Attribute:'PublicApplicationRoot' }}/Content/app/connect_serve.jpg" );
            // Attrib Value for Block:Text Over Image Block - Serve, Attribute:Text Page: Connect, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "2901482E-A8C5-4A47-8EEB-2D3C9E0EA407", "3C535AE2-5D1D-4634-AFC2-658B4D55A3A8", @"Serve" );
            // Attrib Value for Block:Text Over Image Block - Serve, Attribute:Aspect Ratio Page: Connect, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "2901482E-A8C5-4A47-8EEB-2D3C9E0EA407", "BD4BC1F9-9C20-4A6E-A67D-8D5EF56A619D", @"0.45" );
            // Attrib Value for Block:Text Over Image Block - Serve, Attribute:Enabled Lava Commands Page: Connect, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "2901482E-A8C5-4A47-8EEB-2D3C9E0EA407", "3B56BD7B-DFFA-4C07-A936-8B79029C01C5", @"" );
            // Attrib Value for Block:Text Over Image Block - Serve, Attribute:Custom Attributes Page: Connect, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "2901482E-A8C5-4A47-8EEB-2D3C9E0EA407", "B0FCF405-607A-43C1-A2DB-1AA2C066EABB", @"TextColor^White|FontSize^45" );
            // Attrib Value for Block:Text Over Image Block, Attribute:Custom Attributes Page: Serve, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "454646F6-C317-4764-B514-4AF84C1E7FEC", "B0FCF405-607A-43C1-A2DB-1AA2C066EABB", @"TextColor^White|FontSize^45" );
            // Attrib Value for Block:Text Over Image Block, Attribute:Enabled Lava Commands Page: Serve, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "454646F6-C317-4764-B514-4AF84C1E7FEC", "3B56BD7B-DFFA-4C07-A936-8B79029C01C5", @"" );
            // Attrib Value for Block:Text Over Image Block, Attribute:Aspect Ratio Page: Serve, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "454646F6-C317-4764-B514-4AF84C1E7FEC", "BD4BC1F9-9C20-4A6E-A67D-8D5EF56A619D", @"0.35" );
            // Attrib Value for Block:Text Over Image Block, Attribute:Image Page: Serve, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "454646F6-C317-4764-B514-4AF84C1E7FEC", "B1DD109B-F099-4C88-AEF4-FDA0959B5530", @"{{ 'Global' | Attribute:'PublicApplicationRoot' }}/Content/app/serve.jpg" );
            // Attrib Value for Block:Text Over Image Block, Attribute:Action Item Page: Serve, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "454646F6-C317-4764-B514-4AF84C1E7FEC", "0BF7D627-4BD3-4AE3-9AD7-CFABBA6C9799", @"0" );
            // Attrib Value for Block:Text Over Image Block, Attribute:Text Page: Serve, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "454646F6-C317-4764-B514-4AF84C1E7FEC", "3C535AE2-5D1D-4634-AFC2-658B4D55A3A8", @"Serve" );
            // Attrib Value for Block:Markdown Detail, Attribute:Enabled Lava Commands Page: Serve, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "897EB15F-66BB-44F6-BF89-C9012CA36BCA", "7A9950AC-B9E7-4200-9B7A-634BF90DDB2B", @"" );
            // Attrib Value for Block:Markdown Detail, Attribute:Custom Attributes Page: Serve, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "897EB15F-66BB-44F6-BF89-C9012CA36BCA", "538EE4EC-33C5-4997-ABD6-1F105F78EF38", @"Margin^10" );
            // Attrib Value for Block:Markdown Detail, Attribute:Markdown Page: Serve, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "897EB15F-66BB-44F6-BF89-C9012CA36BCA", "9CB3C25B-815D-44A4-9171-698136CC0988", @"Serving changes lives and helps you connect with others at Traders Point and in our community. Choose the area below that’s right for you." );
            // Attrib Value for Block:Mobile ListView Lava, Attribute:Component Page: Serve, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "77E64C5A-7F2A-4634-BEA4-46ECBAF4E56D", "710306EF-D570-41D9-A806-DD38DC14FEDC", @"a6efb571-56c8-44c2-8f87-b7f4db4e1991" );
            // Attrib Value for Block:Mobile ListView Lava, Attribute:Action Item Page: Serve, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "77E64C5A-7F2A-4634-BEA4-46ECBAF4E56D", "9D4D1596-B680-4AFF-AF57-6C8867FC7D6B", @"4^^0" );
            // Attrib Value for Block:Mobile ListView Lava, Attribute:Custom Attributes Page: Serve, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "77E64C5A-7F2A-4634-BEA4-46ECBAF4E56D", "100D227F-518B-4AF4-9451-DB5663433C14", @"Columns^2" );
            // Attrib Value for Block:Mobile ListView Lava, Attribute:Lava Page: Serve, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "77E64C5A-7F2A-4634-BEA4-46ECBAF4E56D", "2DF1245C-33A6-4C0B-9CFB-30047103BD05", @"[{""Id"":""0"",""Title"":""Serve the Church"",""Description"":"""",""Image"":""{{ 'Global' | Attribute:'PublicApplicationRoot' }}/Content/app/serve_thechurch.jpg"",""Resource"":""{{ 'Global' | Attribute:'PublicApplicationRoot' }}/Serve"",""ActionType"":""4""},
{""Id"":""1"",""Title"":""Serve the Community"",""Description"":"""",""Image"":""{{ 'Global' | Attribute:'PublicApplicationRoot' }}/Content/app/serve_thecommunity.jpg"",""Resource"":""{{ 'Global' | Attribute:'PublicApplicationRoot' }}/Serve"",""ActionType"":""4""},
{""Id"":""2"",""Title"":""Serve the World"",""Description"":"""",""Image"":""{{ 'Global' | Attribute:'PublicApplicationRoot' }}/Content/app/serve_theworld.jpg"",""Resource"":""{{ 'Global' | Attribute:'PublicApplicationRoot' }}/Serve"",""ActionType"":""4""}]" );
            // Attrib Value for Block:Mobile ListView Lava, Attribute:Enabled Lava Commands Page: Serve, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "77E64C5A-7F2A-4634-BEA4-46ECBAF4E56D", "1E87BAD4-50AB-474B-955B-E44FA10C0ADE", @"" );
            // Attrib Value for Block:WebViewBlock, Attribute:Url Page: Give, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "FAF7BF20-4F53-47CC-B7E7-A5223BED5188", "924B7E50-5C8B-4455-A0F7-B82364EBC905", @"{{ 'Global' | Attribute:'PublicApplicationRoot' }}/Give" );
            // Attrib Value for Block:WebViewBlock, Attribute:Regex Limit Page: Give, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "FAF7BF20-4F53-47CC-B7E7-A5223BED5188", "D98A8E24-D980-4FB6-B5D4-00CB8287C851", @"^{{ 'Global' | Attribute:'PublicApplicationRoot' }}/.*$" );
            // Attrib Value for Block:WebViewBlock, Attribute:Custom Attributes Page: Give, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "FAF7BF20-4F53-47CC-B7E7-A5223BED5188", "4E1E3555-76B6-44C3-9F5E-56DFFB87C8E0", @"" );
            // Attrib Value for Block:Mobile ListView Lava - Child Pages, Attribute:Custom Attributes Page: Other Blocks, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "253974FD-3F00-4677-86A4-F4F8C84F275C", "100D227F-518B-4AF4-9451-DB5663433C14", @"Columns^1|Margin^5%2C0" );
            // Attrib Value for Block:Mobile ListView Lava - Child Pages, Attribute:Enabled Lava Commands Page: Other Blocks, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "253974FD-3F00-4677-86A4-F4F8C84F275C", "1E87BAD4-50AB-474B-955B-E44FA10C0ADE", @"RockEntity" );
            // Attrib Value for Block:Mobile ListView Lava - Child Pages, Attribute:Action Item Page: Other Blocks, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "253974FD-3F00-4677-86A4-F4F8C84F275C", "9D4D1596-B680-4AFF-AF57-6C8867FC7D6B", @"0" );
            // Attrib Value for Block:Mobile ListView Lava - Child Pages, Attribute:Lava Page: Other Blocks, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "253974FD-3F00-4677-86A4-F4F8C84F275C", "2DF1245C-33A6-4C0B-9CFB-30047103BD05", @"{% page id:'535' %}{% capture pagestr %}{% for childpage in page.Pages %}{% if childpage.DisplayInNavWhen != 'Never' %}{""Id"":""{{ forloop.index0 }}"",""Title"":""{{ childpage.PageTitle }}"",""Description"":""{{ childpage.PageDescription }}"",""Icon"":""{{ childpage.IconCssClass }}"",""Image"":"""",""Resource"":""{% assign resource = childpage | Attribute:""Resource"" %}{% if resource != '' %}{{ resource }}{% else %}{{ childpage.Id }}{% endif %}"",""ActionType"":""{{ childpage | Attribute:""ActionType"",""RawValue"" }}""}, {% endif %}{% endfor %}{% endcapture %}{% endpage -%}
[{{ pagestr | ReplaceLast:"", "" }}]" ); // NEED SQL, 535
            Sql( @"DECLARE @PageId int = ( SELECT TOP 1 [Id] FROM [Page] WHERE [Guid] = 'D4A5C0AB-0A90-416F-9B3F-C0ACFED92D43' )
                UPDATE av SET av.[Value] = Replace([Value],535,@PageId)
                FROM [AttributeValue] av 
				INNER JOIN Attribute a ON a.Id = av.AttributeId 
				INNER JOIN Block b ON a.EntityTypeQualifierColumn = 'BlockTypeId' AND a.EntityTypeQualifierValue = b.BlockTypeId
                WHERE a.[Guid] = '2DF1245C-33A6-4C0B-9CFB-30047103BD05' AND b.[Guid] = '253974FD-3F00-4677-86A4-F4F8C84F275C'" ); // Set AttributeValue to correct page id         

            // Attrib Value for Block:Mobile ListView Lava - Child Pages, Attribute:Component Page: Other Blocks, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "253974FD-3F00-4677-86A4-F4F8C84F275C", "710306EF-D570-41D9-A806-DD38DC14FEDC", @"1a637b48-35fb-43b2-9822-88af2fd1d333" );
            // Attrib Value for Block:Login App, Attribute:Custom Attributes Page: Login, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "0DB6C226-836D-4CEB-B3CC-9F50ED499DD5", "3021F2AE-08B6-4EC0-B18F-F18C51D4DC7A", @"Margin^10" );
            // Attrib Value for Block:Person Card, Attribute:EntityType Page: Person Card, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "86FF3353-4FEE-4272-BD9B-78B5C373051A", "AF34D0D9-76BA-4D34-8236-929C71BC2CEB", @"GroupMember" );
            // Attrib Value for Block:Person Card, Attribute:Accent Color Page: Person Card, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "86FF3353-4FEE-4272-BD9B-78B5C373051A", "3E3A6CB1-CAA4-4B7E-ABE3-4C58D870F912", @"" );
            // Attrib Value for Block:Person Card, Attribute:Custom Attributes Page: Person Card, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "86FF3353-4FEE-4272-BD9B-78B5C373051A", "27CE0D5E-7E9A-4F63-B940-27F89017E267", @"" );
            // Attrib Value for Block:Group List, Attribute:Enabled Lava Commands Page: Group List, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "13472A3E-A5FD-47C1-A382-4243BBE0BC98", "6451C0FE-D830-46C2-9C4E-F6CCA72C8A89", @"" );
            // Attrib Value for Block:Group List, Attribute:Lava Page: Group List, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "13472A3E-A5FD-47C1-A382-4243BBE0BC98", "C9A3774F-21D3-49B1-8EFD-4A7807ED5A8A", @"[
{% for group in Groups -%}
  { ""Id"":""{{group.Id}}"", ""Title"":""{{group.Name}}"", ""Description"":""{{group.Description}}"", ""Icon"":""{{group.GroupType.IconCssClass}}"" },
{% endfor -%}
]" );
            // Attrib Value for Block:Group List, Attribute:Only Show If Leader Page: Group List, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "13472A3E-A5FD-47C1-A382-4243BBE0BC98", "16F74DEE-D2CF-459B-9E7B-0BE66F6A47AE", @"True" );
            // Attrib Value for Block:Group List, Attribute:Parent Group Ids Page: Group List, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "13472A3E-A5FD-47C1-A382-4243BBE0BC98", "36595C20-0B19-44D8-ADE9-446595557E80", @"56" );
            // Attrib Value for Block:Group List, Attribute:Component Page: Group List, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "13472A3E-A5FD-47C1-A382-4243BBE0BC98", "9B306F24-7D3A-4EF8-A1A1-C767211C92AB", @"1a637b48-35fb-43b2-9822-88af2fd1d333" );
            // Attrib Value for Block:Group List, Attribute:Action Item Page: Group List, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "13472A3E-A5FD-47C1-A382-4243BBE0BC98", "0CB6C7EC-FB8B-4C0A-805B-108075AA72BE", @"1^541^parameter" );
            // Attrib Value for Block:Group List, Attribute:Custom Attributes Page: Group List, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "13472A3E-A5FD-47C1-A382-4243BBE0BC98", "727903A8-FD3B-4E41-AFC4-309756390E34", @"" );
            // Attrib Value for Block:Group Member List Block, Attribute:Action Item Page: Group Member List, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "D53AE8B1-9F7B-457C-A2AB-297D7E0B0C71", "0B073274-28E6-499F-A408-8CA05644F2EF", @"1^538^parameter" ); // NEED SQL, 538
            Sql( @"DECLARE @PageId int = ( SELECT TOP 1 [Id] FROM [Page] WHERE [Guid] = '79EF0279-9800-4BAF-A8D9-DD42E6868BAA' )
                UPDATE av SET av.[Value] = Replace([Value],538,@PageId)
                FROM [AttributeValue] av 
				INNER JOIN Attribute a ON a.Id = av.AttributeId 
				INNER JOIN Block b ON a.EntityTypeQualifierColumn = 'BlockTypeId' AND a.EntityTypeQualifierValue = b.BlockTypeId
                WHERE a.[Guid] = '0B073274-28E6-499F-A408-8CA05644F2EF' AND b.[Guid] = 'D53AE8B1-9F7B-457C-A2AB-297D7E0B0C71'" ); // Set AttributeValue to correct page id         
            // Attrib Value for Block:Group Member List Block, Attribute:Members Per Request Page: Group Member List, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "D53AE8B1-9F7B-457C-A2AB-297D7E0B0C71", "D40889E8-9468-40B2-8776-B15C7D9120E9", @"20" );
            // Attrib Value for Block:Group Member List Block, Attribute:Component Page: Group Member List, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "D53AE8B1-9F7B-457C-A2AB-297D7E0B0C71", "4D97403F-2540-4B22-9EC4-6BEF1DC283C4", @"d9ea2c97-68e1-4d94-b881-f3ac4f2883a3" );
            // Attrib Value for Block:Group Member List Block, Attribute:Enabled Lava Commands Page: Group Member List, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "D53AE8B1-9F7B-457C-A2AB-297D7E0B0C71", "FB550261-8164-41BF-A2C8-554E161CB617", @"" );
            // Attrib Value for Block:Group Member List Block, Attribute:Custom Attributes Page: Group Member List, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "D53AE8B1-9F7B-457C-A2AB-297D7E0B0C71", "56498C7F-E638-4E45-A11F-644D6AC20242", @"" );
            // Attrib Value for Block:Note Block, Attribute:Note Field Label Page: Note Block, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "FB213CB9-C91F-4546-9371-73839FBD447A", "34C4CC92-BC30-4835-9E36-71247FB2AC2D", @"Notes:" );
            // Attrib Value for Block:Note Block, Attribute:Note Type Page: Note Block, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "FB213CB9-C91F-4546-9371-73839FBD447A", "E1E76347-70A9-464C-BA51-A4D938538A27", @"66A1B9D7-7EFA-40F3-9415-E54437977D60" );
            // Attrib Value for Block:Note Block, Attribute:Note Field Height Page: Note Block, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "FB213CB9-C91F-4546-9371-73839FBD447A", "ED27D786-60AA-45E0-A57C-9893206F9A3E", @"200" );
            // Attrib Value for Block:Note Block, Attribute:Custom Attributes Page: Note Block, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "FB213CB9-C91F-4546-9371-73839FBD447A", "D0EC7BA9-C9A3-48B3-A291-202571ED7AD4", @"" );
            // Attrib Value for Block:Group Attendance Block, Attribute:Custom Attributes Page: Group Attendance, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "BD824150-F1B1-4A7D-93FA-5A4AA7AE565D", "151CEBB9-9BFF-43F3-9C73-672F6B645C7D", @"" );
            // Attrib Value for Block:Button, Attribute:Custom Attributes Page: Connect, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "99CAEBAF-9E80-410E-BB33-9DCD6B4ECF9B", "FED19184-4169-4EEF-9437-592545A79461", @"Margin^10|BorderRadius^10" );
            // Attrib Value for Block:Button, Attribute:Text Page: Connect, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "99CAEBAF-9E80-410E-BB33-9DCD6B4ECF9B", "B60F03CA-B109-48F6-ACE4-1C365DC4E908", @"More pages" );
            // Attrib Value for Block:Button, Attribute:Enabled Lava Commands Page: Connect, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "99CAEBAF-9E80-410E-BB33-9DCD6B4ECF9B", "BEF7E221-837D-4DD5-BC3E-49E6E3BDEBF9", @"" );
            // Attrib Value for Block:Button, Attribute:Action Item Page: Connect, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "99CAEBAF-9E80-410E-BB33-9DCD6B4ECF9B", "A9D5A12D-F60D-4CAA-A46B-1A605032586D", @"1^535^" ); // NEED SQL, 535
            Sql( @"DECLARE @PageId int = ( SELECT TOP 1 [Id] FROM [Page] WHERE [Guid] = 'D4A5C0AB-0A90-416F-9B3F-C0ACFED92D43' )
                UPDATE av SET av.[Value] = Replace([Value],535,@PageId)
                FROM [AttributeValue] av 
				INNER JOIN Attribute a ON a.Id = av.AttributeId 
				INNER JOIN Block b ON a.EntityTypeQualifierColumn = 'BlockTypeId' AND a.EntityTypeQualifierValue = b.BlockTypeId
                WHERE a.[Guid] = 'A9D5A12D-F60D-4CAA-A46B-1A605032586D' AND b.[Guid] = '99CAEBAF-9E80-410E-BB33-9DCD6B4ECF9B'" ); // Set AttributeValue to correct page id
            // Attrib Value for Block:WebViewBlock, Attribute:Url Page: Webview Block, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "E43CF3E1-D8FA-41B3-AED9-E09FAA594F7B", "924B7E50-5C8B-4455-A0F7-B82364EBC905", @"{{ 'Global' | Attribute:'PublicApplicationRoot' }}/connect/" );
            // Attrib Value for Block:WebViewBlock, Attribute:Regex Limit Page: Webview Block, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "E43CF3E1-D8FA-41B3-AED9-E09FAA594F7B", "D98A8E24-D980-4FB6-B5D4-00CB8287C851", @"^{{ 'Global' | Attribute:'PublicApplicationRoot' }}/.*$" );
            // Attrib Value for Block:WebViewBlock, Attribute:Custom Attributes Page: Webview Block, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "E43CF3E1-D8FA-41B3-AED9-E09FAA594F7B", "4E1E3555-76B6-44C3-9F5E-56DFFB87C8E0", @"" );
            // Attrib Value for Block:Icon Block, Attribute:Enabled Lava Commands , Layout: No Scroll, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "962317D3-42D0-4152-839E-35AAF2BD66E2", "0910B50C-23D8-46AC-BBF2-F54F074E35DC", @"" );
            // Attrib Value for Block:Icon Block, Attribute:Text , Layout: No Scroll, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "962317D3-42D0-4152-839E-35AAF2BD66E2", "4F4142E2-E6C0-4878-953F-521FC7A8A4C9", @"fa fa-chevron-left" );
            // Attrib Value for Block:Icon Block, Attribute:Action Item , Layout: No Scroll, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "962317D3-42D0-4152-839E-35AAF2BD66E2", "7B4B9D30-2BED-4F39-B897-10C7C561CFAE", @"3" );
            // Attrib Value for Block:Icon Block, Attribute:Custom Attributes , Layout: No Scroll, Site: Avalanche
            RockMigrationHelper.AddBlockAttributeValue( "962317D3-42D0-4152-839E-35AAF2BD66E2", "BE7D91EF-5C79-4940-9DA6-608933781419", @"TextColor^White|FontSize^25|Margin^15%2C19%2C10%2C16" );

            //RockMigrationHelper.UpdateFieldType( "Action Item", "", "Avalanche", "Avalanche.Field.Types.ActionItemFieldType", "E5A6D6C7-DAB4-4EFA-B76F-E22AFEC5158D" );
        }

        /// <summary>
        /// The commands to undo a migration from a specific version
        /// </summary>
        public override void Down()
        {
            //
            // remove pages
            //

            // Attrib for BlockType: Add Group:Generate Alternate Identifier
            RockMigrationHelper.DeleteAttribute( "A730E61E-88D0-418C-B93D-AB66AA3DF9DB" );
            // Attrib for BlockType: Person Profile Family:Custom Attributes
            RockMigrationHelper.DeleteAttribute( "A928AE35-E9AC-45DE-9D7D-50A0548E8063" );
            // Attrib for BlockType: Person Profile Family:Additional Changes Link
            RockMigrationHelper.DeleteAttribute( "8A00043B-EDA0-4020-9915-46A008087928" );
            // Attrib for BlockType: Person Profile Family:Component
            RockMigrationHelper.DeleteAttribute( "CFEB82A4-02FA-4F22-A38E-08E1B6FBD4ED" );
            // Attrib for BlockType: Person Profile Family:Action Item
            RockMigrationHelper.DeleteAttribute( "508BD5E9-D674-46AD-83DE-1BF620C85B29" );
            // Attrib for BlockType: Public Profile Edit Block:Custom Attributes
            RockMigrationHelper.DeleteAttribute( "CE5435AF-C9E4-437B-B0D4-870B88DA514D" );
            // Attrib for BlockType: Public Profile Edit Block:Next Page
            RockMigrationHelper.DeleteAttribute( "5EC718EB-8262-4A98-91AD-71807750863B" );
            // Attrib for BlockType: Content Channel Mobile List:Order Lava
            RockMigrationHelper.DeleteAttribute( "DF036BE3-5D4F-4B56-B59B-1C323FAE7806" );
            // Attrib for BlockType: Group Attendance Block:Custom Attributes
            //RockMigrationHelper.DeleteAttribute( "151CEBB9-9BFF-43F3-9C73-672F6B645C7D" );
            // Attrib for BlockType: Note Block:Custom Attributes
            //RockMigrationHelper.DeleteAttribute( "D0EC7BA9-C9A3-48B3-A291-202571ED7AD4" );
            // Attrib for BlockType: Group Member List Block:Custom Attributes
            RockMigrationHelper.DeleteAttribute( "56498C7F-E638-4E45-A11F-644D6AC20242" );
            // Attrib for BlockType: Group Member List Block:Enabled Lava Commands
            RockMigrationHelper.DeleteAttribute( "FB550261-8164-41BF-A2C8-554E161CB617" );
            // Attrib for BlockType: Group Member List Block:Component
            RockMigrationHelper.DeleteAttribute( "4D97403F-2540-4B22-9EC4-6BEF1DC283C4" );
            // Attrib for BlockType: Group Member List Block:Members Per Request
            RockMigrationHelper.DeleteAttribute( "D40889E8-9468-40B2-8776-B15C7D9120E9" );
            // Attrib for BlockType: Group Member List Block:Action Item
            RockMigrationHelper.DeleteAttribute( "0B073274-28E6-499F-A408-8CA05644F2EF" );
            // Attrib for BlockType: Person Card:Custom Attributes
            RockMigrationHelper.DeleteAttribute( "27CE0D5E-7E9A-4F63-B940-27F89017E267" );
            // Attrib for BlockType: Person Card:Accent Color
            RockMigrationHelper.DeleteAttribute( "3E3A6CB1-CAA4-4B7E-ABE3-4C58D870F912" );
            // Attrib for BlockType: Person Card:EntityType
            RockMigrationHelper.DeleteAttribute( "AF34D0D9-76BA-4D34-8236-929C71BC2CEB" );
            // Attrib for BlockType: Prayer Request Entry:Custom Attributes
            RockMigrationHelper.DeleteAttribute( "CE8D0755-35A1-43B6-A0D8-9F6DA009E9A9" );
            // Attrib for BlockType: Prayer Request Entry:Save Success Text
            RockMigrationHelper.DeleteAttribute( "2EB23222-A5CD-4425-BA3E-F19495F53351" );
            // Attrib for BlockType: Prayer Request Entry:Action Item
            RockMigrationHelper.DeleteAttribute( "5EA0CDA9-449F-4661-A16A-55245C1BDEFE" );
            // Attrib for BlockType: Prayer Request Entry:Require Campus
            RockMigrationHelper.DeleteAttribute( "58CAABD8-850F-4C4E-8B09-0CD424C424FD" );
            // Attrib for BlockType: Prayer Request Entry:Show Campus
            RockMigrationHelper.DeleteAttribute( "5A4F4745-F7E7-4E0E-91CC-AD21190DEF54" );
            // Attrib for BlockType: Prayer Request Entry:Enable Public Display Flag
            RockMigrationHelper.DeleteAttribute( "1EC69301-3C1A-40AB-A1D3-7FCC84358E45" );
            // Attrib for BlockType: Prayer Request Entry:Enable Comments Flag
            RockMigrationHelper.DeleteAttribute( "26A590AA-3E48-458E-9755-F4511609D24F" );
            // Attrib for BlockType: Prayer Request Entry:Expires After (Days)
            RockMigrationHelper.DeleteAttribute( "8503F519-CE9D-4CEE-B8B7-EDC5CB642896" );
            // Attrib for BlockType: Prayer Request Entry:Enable Auto Approve
            RockMigrationHelper.DeleteAttribute( "8E427F57-420E-4775-94C7-747BC6C25EB5" );
            // Attrib for BlockType: Prayer Request Entry:Default Category
            RockMigrationHelper.DeleteAttribute( "06A82B98-4655-490D-BFC7-DE26BDD5EE3A" );
            // Attrib for BlockType: Prayer Request Entry:Workflow
            RockMigrationHelper.DeleteAttribute( "9B8D5D1C-257C-403A-8B89-CC879373D1C6" );
            // Attrib for BlockType: Prayer Request Entry:Require Last Name
            RockMigrationHelper.DeleteAttribute( "CD0A6E47-936F-4B75-8AAE-ACCB1A73107A" );
            // Attrib for BlockType: Prayer Request Entry:Default To Public
            RockMigrationHelper.DeleteAttribute( "DDDADB42-A9DD-4F43-AAE1-5EA53BC9BAC0" );
            // Attrib for BlockType: Prayer Request Entry:Enable Urgent Flag
            RockMigrationHelper.DeleteAttribute( "CA49B740-8BB9-48B7-8A79-A7B49046EBB9" );
            // Attrib for BlockType: Prayer Request Entry:Default Allow Comments Setting
            RockMigrationHelper.DeleteAttribute( "7048A6C4-C76C-44C1-B227-B64D956BFC98" );
            // Attrib for BlockType: Prayer Request Entry:Category Selection
            RockMigrationHelper.DeleteAttribute( "8F4AA029-6CC4-4826-8809-94EE0D97301C" );
            // Attrib for BlockType: Login App:Custom Attributes
            RockMigrationHelper.DeleteAttribute( "3021F2AE-08B6-4EC0-B18F-F18C51D4DC7A" );
            // Attrib for BlockType: WebViewBlock:Custom Attributes
            RockMigrationHelper.DeleteAttribute( "4E1E3555-76B6-44C3-9F5E-56DFFB87C8E0" );
            // Attrib for BlockType: WebViewBlock:Regex Limit
            RockMigrationHelper.DeleteAttribute( "D98A8E24-D980-4FB6-B5D4-00CB8287C851" );
            // Attrib for BlockType: WebViewBlock:Url
            RockMigrationHelper.DeleteAttribute( "924B7E50-5C8B-4455-A0F7-B82364EBC905" );
            // Attrib for BlockType: Event Filter:Custom Attributes
            RockMigrationHelper.DeleteAttribute( "92E11239-D329-4E43-9847-72358122D052" );
            // Attrib for BlockType: Event Filter:Action Item
            RockMigrationHelper.DeleteAttribute( "CBB79612-83F9-4FC5-94EC-450DED06B30B" );
            // Attrib for BlockType: Calendar Event Item Occurrence List:core.CustomGridEnableStickerHeaders
            RockMigrationHelper.DeleteAttribute( "33349C8D-F73D-46AF-93F2-DA252AD78A7F" );
            // Attrib for BlockType: Calendar Event Item List:core.CustomGridEnableStickerHeaders
            RockMigrationHelper.DeleteAttribute( "3EFEBA22-F3A9-4CB2-A1F6-2E23FEF30AFC" );
            // Attrib for BlockType: Avalanche Event Calendar Lava:Custom Attributes
            RockMigrationHelper.DeleteAttribute( "0B61EA06-A713-4359-BCA4-1D0602E8C6CA" );
            // Attrib for BlockType: Avalanche Event Calendar Lava:Lava Template
            RockMigrationHelper.DeleteAttribute( "71BFE3A7-41CD-46E8-B95F-456735104DA2" );
            // Attrib for BlockType: Avalanche Event Calendar Lava:Category Parameter Name
            RockMigrationHelper.DeleteAttribute( "EC03AFD7-A544-4BEB-9CD8-5A87FC90B6B0" );
            // Attrib for BlockType: Avalanche Event Calendar Lava:Component
            RockMigrationHelper.DeleteAttribute( "2A037292-80ED-4EF6-95E6-A3518EFFDC2C" );
            // Attrib for BlockType: Avalanche Event Calendar Lava:Set Page Title
            RockMigrationHelper.DeleteAttribute( "B5EA1BE1-DE1D-4C9E-B565-A3050869CD58" );
            // Attrib for BlockType: Avalanche Event Calendar Lava:Start of Week Day
            RockMigrationHelper.DeleteAttribute( "A016AF17-6C91-49BD-AC92-C4B83A9BFE46" );
            // Attrib for BlockType: Avalanche Event Calendar Lava:Campus Parameter Name
            RockMigrationHelper.DeleteAttribute( "32ADBCD2-7858-4566-9AAE-4C5DC5C43246" );
            // Attrib for BlockType: Avalanche Event Calendar Lava:Action Item
            RockMigrationHelper.DeleteAttribute( "FD94A70E-EAC2-4851-9D3C-84629A93F62E" );
            // Attrib for BlockType: Avalanche Event Calendar Lava:Enable Campus Context
            RockMigrationHelper.DeleteAttribute( "13564317-66A2-4EE2-9078-5B314E8EB249" );
            // Attrib for BlockType: Avalanche Event Calendar Lava:Show Year View
            RockMigrationHelper.DeleteAttribute( "B7A1B651-9CC9-4961-AB8C-5E89FA7F19B2" );
            // Attrib for BlockType: Avalanche Event Calendar Lava:Show Month View
            RockMigrationHelper.DeleteAttribute( "42744616-A525-4252-8CEE-9CDE6250DD31" );
            // Attrib for BlockType: Avalanche Event Calendar Lava:Show Week View
            RockMigrationHelper.DeleteAttribute( "15084637-2667-4260-8E50-9B01EA852ADF" );
            // Attrib for BlockType: Avalanche Event Calendar Lava:Show Day View
            RockMigrationHelper.DeleteAttribute( "3692C511-DFBF-4F5A-8BC0-16EAFC1B4BAC" );
            // Attrib for BlockType: Avalanche Event Calendar Lava:Show Small Calendar
            RockMigrationHelper.DeleteAttribute( "197DA2B8-F798-44BF-B2C2-99723A3E2E45" );
            // Attrib for BlockType: Avalanche Event Calendar Lava:Show Date Range Filter
            RockMigrationHelper.DeleteAttribute( "04992B21-617D-48A1-ADF3-951CB633CC93" );
            // Attrib for BlockType: Avalanche Event Calendar Lava:Filter Audiences
            RockMigrationHelper.DeleteAttribute( "9F67474F-B6DF-4549-884D-F89E56AA954F" );
            // Attrib for BlockType: Avalanche Event Calendar Lava:Audience Filter Display Mode
            RockMigrationHelper.DeleteAttribute( "95AF549B-D25C-49BE-8B0F-59C3F3D1B4D4" );
            // Attrib for BlockType: Avalanche Event Calendar Lava:Campus Filter Display Mode
            RockMigrationHelper.DeleteAttribute( "7FBB51A6-B2C7-4E1D-ABAF-F4D01ABF1D8F" );
            // Attrib for BlockType: Avalanche Event Calendar Lava:Enabled Lava Commands
            RockMigrationHelper.DeleteAttribute( "1AD0C6A7-16D4-4DBA-9584-342137D3E051" );
            // Attrib for BlockType: Avalanche Event Calendar Lava:Default View Option
            RockMigrationHelper.DeleteAttribute( "4BC3868B-23BB-4A46-970F-BD16447F705E" );
            // Attrib for BlockType: Avalanche Event Calendar Lava:Event Calendar
            RockMigrationHelper.DeleteAttribute( "29E37C6C-CCD8-4631-A2B5-200D93678D7A" );
            // Attrib for BlockType: Avalanche Event Calendar Lava:Cache Duration
            RockMigrationHelper.DeleteAttribute( "4D9B40B4-14E2-4B00-9725-0D33B6606BD1" );
            // Attrib for BlockType: Mobile Workflow:Custom Attributes
            RockMigrationHelper.DeleteAttribute( "96FBBEC4-EDDE-4D98-8B3D-BF656CB4B563" );
            // Attrib for BlockType: Mobile Workflow:Workflow Type
            RockMigrationHelper.DeleteAttribute( "3667DECB-4576-46A5-B2CE-ACC2D43D0D69" );
            // Attrib for BlockType: Group List:Custom Attributes
            RockMigrationHelper.DeleteAttribute( "727903A8-FD3B-4E41-AFC4-309756390E34" );
            // Attrib for BlockType: Group List:Action Item
            RockMigrationHelper.DeleteAttribute( "0CB6C7EC-FB8B-4C0A-805B-108075AA72BE" );
            // Attrib for BlockType: Group List:Component
            RockMigrationHelper.DeleteAttribute( "9B306F24-7D3A-4EF8-A1A1-C767211C92AB" );
            // Attrib for BlockType: Group List:Parent Group Ids
            RockMigrationHelper.DeleteAttribute( "36595C20-0B19-44D8-ADE9-446595557E80" );
            // Attrib for BlockType: Group List:Only Show If Leader
            RockMigrationHelper.DeleteAttribute( "16F74DEE-D2CF-459B-9E7B-0BE66F6A47AE" );
            // Attrib for BlockType: Group List:Lava
            RockMigrationHelper.DeleteAttribute( "C9A3774F-21D3-49B1-8EFD-4A7807ED5A8A" );
            // Attrib for BlockType: Group List:Enabled Lava Commands
            RockMigrationHelper.DeleteAttribute( "6451C0FE-D830-46C2-9C4E-F6CCA72C8A89" );
            // Attrib for BlockType: Preload Block:Custom Attributes
            RockMigrationHelper.DeleteAttribute( "E28D8CD0-91E8-453F-B221-C8A9E60DEE46" );
            // Attrib for BlockType: Preload Block:Pages To Preload
            RockMigrationHelper.DeleteAttribute( "73006B75-6B97-4203-8182-0944455BB213" );
            // Attrib for BlockType: Preload Block:Enabled Lava Commands
            RockMigrationHelper.DeleteAttribute( "2811B163-E681-4EBA-9280-A82742FEBE3C" );
            // Attrib for BlockType: Audio Player Block:Custom Attributes
            RockMigrationHelper.DeleteAttribute( "B5C561AE-4926-42E3-8FF3-D231F0B7FAEC" );
            // Attrib for BlockType: Audio Player Block:Artist
            RockMigrationHelper.DeleteAttribute( "614313D7-9634-4959-B117-6A2E7FA76272" );
            // Attrib for BlockType: Audio Player Block:Title
            RockMigrationHelper.DeleteAttribute( "79211D59-E0D9-4C7C-919F-B5B084F9D953" );
            // Attrib for BlockType: Audio Player Block:AutoPlay
            RockMigrationHelper.DeleteAttribute( "38F36F45-EDEE-4DEE-8DE2-377BBD029D76" );
            // Attrib for BlockType: Audio Player Block:Enabled Lava Commands
            RockMigrationHelper.DeleteAttribute( "2DF42069-76B8-4414-B602-9828F1FDC2B2" );
            // Attrib for BlockType: Audio Player Block:Source
            RockMigrationHelper.DeleteAttribute( "CA288975-9E8C-4612-96A0-23E1B7C3FDF9" );
            // Attrib for BlockType: Video Player Block:Custom Attributes
            RockMigrationHelper.DeleteAttribute( "8E6A53D3-AD8B-4993-86B6-981A909F66A2" );
            // Attrib for BlockType: Video Player Block:AutoPlay
            RockMigrationHelper.DeleteAttribute( "FB9755DA-5078-4CF1-826D-93CADE127EAA" );
            // Attrib for BlockType: Video Player Block:Enabled Lava Commands
            RockMigrationHelper.DeleteAttribute( "F860FBE4-55E7-4541-9C91-CC01463B3496" );
            // Attrib for BlockType: Video Player Block:Source
            RockMigrationHelper.DeleteAttribute( "8E4398DD-5BE5-4148-A87B-0C72E649CE6E" );
            // Attrib for BlockType: Video Player Block:Aspect Ratio
            RockMigrationHelper.DeleteAttribute( "0AF93DA7-6355-4EEB-AC12-D1C4AD8F0118" );
            // Attrib for BlockType: Content Channel Mobile List:Icon Lava
            RockMigrationHelper.DeleteAttribute( "1D0A5A0B-CD92-4120-93D1-4A5F698A33FC" );
            // Attrib for BlockType: Content Channel Mobile List:Image Lava
            RockMigrationHelper.DeleteAttribute( "4BC02D93-4ECB-44C0-9218-445815394C2E" );
            // Attrib for BlockType: Content Channel Mobile List:Description Lava
            RockMigrationHelper.DeleteAttribute( "7B7BF120-464E-45A6-9398-4E7F82C6B5F2" );
            // Attrib for BlockType: Content Channel Mobile List:Title Lava
            RockMigrationHelper.DeleteAttribute( "DCD91C59-5152-4043-A7E2-A79301AF6A18" );
            // Attrib for BlockType: Content Channel Mobile List:Order
            RockMigrationHelper.DeleteAttribute( "956EFB28-F562-4A65-AEBB-AFACD44DB659" );
            // Attrib for BlockType: Content Channel Mobile List:Query Parameter Filtering
            RockMigrationHelper.DeleteAttribute( "3BDF891A-3B7D-4839-A0B5-E83E966C5F5F" );
            // Attrib for BlockType: Content Channel Mobile List:Filter Id
            RockMigrationHelper.DeleteAttribute( "378D03E2-16C8-42AB-89FF-A90DE0FDA232" );
            // Attrib for BlockType: Content Channel Mobile List:Count
            RockMigrationHelper.DeleteAttribute( "CE2FCBB8-22C5-4522-893C-DC63F5AD9E1F" );
            // Attrib for BlockType: Content Channel Mobile List:Status
            RockMigrationHelper.DeleteAttribute( "4B294624-5BB8-4457-9621-FC471BCD5823" );
            // Attrib for BlockType: Content Channel Mobile List:Channel
            RockMigrationHelper.DeleteAttribute( "25941CC2-1C51-46F6-AB82-7A9FA85ECFBA" );
            // Attrib for BlockType: Content Channel Mobile List:Detail Page
            RockMigrationHelper.DeleteAttribute( "46D432B9-7F18-4BF5-B874-54347A87A2C2" );
            // Attrib for BlockType: Content Channel Mobile List:Component
            RockMigrationHelper.DeleteAttribute( "CF2A9EB3-8F1B-4562-979C-CB59D447BCCF" );
            // Attrib for BlockType: Content Channel Mobile List:Output Cache Duration
            RockMigrationHelper.DeleteAttribute( "0F110CCF-188D-4061-998C-7368A1BE171B" );
            // Attrib for BlockType: Content Channel Mobile List:Custom Attributes
            RockMigrationHelper.DeleteAttribute( "A88DA132-AA2B-47BD-8C04-F1A14D4B12A8" );
            // Attrib for BlockType: Content Channel Mobile List:Item Cache Duration
            RockMigrationHelper.DeleteAttribute( "7A361636-F72A-40CA-91E2-20AA7496CDBC" );
            // Attrib for BlockType: Icon Button:Custom Attributes
            RockMigrationHelper.DeleteAttribute( "6D8A7116-BB0F-4F3A-9C24-5C39F2364E42" );
            // Attrib for BlockType: Icon Button:Enabled Lava Commands
            RockMigrationHelper.DeleteAttribute( "2FCA15F3-D621-4E7C-8F0C-FF9222AB3447" );
            // Attrib for BlockType: Icon Button:Icon
            RockMigrationHelper.DeleteAttribute( "FCE07A51-64D8-4FC8-BFB2-8CDBCED449F7" );
            // Attrib for BlockType: Icon Button:Action Item
            RockMigrationHelper.DeleteAttribute( "1AD33D1F-BDE9-4AFB-B6E8-E2CD271A5EF3" );
            // Attrib for BlockType: Icon Button:Text
            RockMigrationHelper.DeleteAttribute( "9FFA6301-04B4-4D1D-907C-D345BE825B13" );
            // Attrib for BlockType: Button:Custom Attributes
            RockMigrationHelper.DeleteAttribute( "FED19184-4169-4EEF-9437-592545A79461" );
            // Attrib for BlockType: Button:Enabled Lava Commands
            RockMigrationHelper.DeleteAttribute( "BEF7E221-837D-4DD5-BC3E-49E6E3BDEBF9" );
            // Attrib for BlockType: Button:Action Item
            RockMigrationHelper.DeleteAttribute( "A9D5A12D-F60D-4CAA-A46B-1A605032586D" );
            // Attrib for BlockType: Button:Text
            RockMigrationHelper.DeleteAttribute( "B60F03CA-B109-48F6-ACE4-1C365DC4E908" );
            // Attrib for BlockType: Campus List:core.CustomGridEnableStickerHeaders
            RockMigrationHelper.DeleteAttribute( "3412FD2B-E75B-4F61-8B0B-750D9BB046CF" );
            // Attrib for BlockType: Markdown Detail:Custom Attributes
            RockMigrationHelper.DeleteAttribute( "538EE4EC-33C5-4997-ABD6-1F105F78EF38" );
            // Attrib for BlockType: Markdown Detail:Enabled Lava Commands
            RockMigrationHelper.DeleteAttribute( "7A9950AC-B9E7-4200-9B7A-634BF90DDB2B" );
            // Attrib for BlockType: Markdown Detail:Markdown
            RockMigrationHelper.DeleteAttribute( "9CB3C25B-815D-44A4-9171-698136CC0988" );
            // Attrib for BlockType: Label Block:Custom Attributes
            RockMigrationHelper.DeleteAttribute( "A59E551E-6B04-4C70-B1F6-B8EAEAC27D94" );
            // Attrib for BlockType: Label Block:Text
            RockMigrationHelper.DeleteAttribute( "95980EF8-47FA-40C9-9372-938F46746458" );
            // Attrib for BlockType: Label Block:Action Item
            RockMigrationHelper.DeleteAttribute( "0D9816B1-E6CF-46C3-A3C7-4099359B2857" );
            // Attrib for BlockType: Label Block:Enabled Lava Commands
            RockMigrationHelper.DeleteAttribute( "7F860DC6-7478-4F88-A878-FAF3D75CEB9C" );
            // Attrib for BlockType: Icon Block:Custom Attributes
            RockMigrationHelper.DeleteAttribute( "BE7D91EF-5C79-4940-9DA6-608933781419" );
            // Attrib for BlockType: Icon Block:Action Item
            RockMigrationHelper.DeleteAttribute( "7B4B9D30-2BED-4F39-B897-10C7C561CFAE" );
            // Attrib for BlockType: Icon Block:Text
            RockMigrationHelper.DeleteAttribute( "4F4142E2-E6C0-4878-953F-521FC7A8A4C9" );
            // Attrib for BlockType: Icon Block:Enabled Lava Commands
            RockMigrationHelper.DeleteAttribute( "0910B50C-23D8-46AC-BBF2-F54F074E35DC" );
            // Attrib for BlockType: Mobile ListView Lava:Custom Attributes
            RockMigrationHelper.DeleteAttribute( "100D227F-518B-4AF4-9451-DB5663433C14" );
            // Attrib for BlockType: Mobile ListView Lava:Action Item
            RockMigrationHelper.DeleteAttribute( "9D4D1596-B680-4AFF-AF57-6C8867FC7D6B" );
            // Attrib for BlockType: Mobile ListView Lava:Enabled Lava Commands
            RockMigrationHelper.DeleteAttribute( "1E87BAD4-50AB-474B-955B-E44FA10C0ADE" );
            // Attrib for BlockType: Mobile ListView Lava:Component
            RockMigrationHelper.DeleteAttribute( "710306EF-D570-41D9-A806-DD38DC14FEDC" );
            // Attrib for BlockType: Mobile ListView Lava:Lava
            RockMigrationHelper.DeleteAttribute( "2DF1245C-33A6-4C0B-9CFB-30047103BD05" );
            // Attrib for BlockType: Image Block:Custom Attributes
            RockMigrationHelper.DeleteAttribute( "B9523EB6-EEE5-4DC4-B2B0-EF7FF0E73F51" );
            // Attrib for BlockType: Image Block:Aspect
            RockMigrationHelper.DeleteAttribute( "1900D9DD-EFA4-4278-8B23-0FEEF1894610" );
            // Attrib for BlockType: Image Block:Image
            RockMigrationHelper.DeleteAttribute( "2E9389F8-0E3C-485D-B722-762D89C8EB2E" );
            // Attrib for BlockType: Image Block:Action Item
            RockMigrationHelper.DeleteAttribute( "68EF73D8-3C6D-4524-A3A4-9C06D95B164A" );
            // Attrib for BlockType: Image Block:Enabled Lava Commands
            RockMigrationHelper.DeleteAttribute( "155B8623-7723-4FD2-A321-B9D24B1611CE" );
            // Attrib for BlockType: Text Over Image Block:Custom Attributes
            RockMigrationHelper.DeleteAttribute( "B0FCF405-607A-43C1-A2DB-1AA2C066EABB" );
            // Attrib for BlockType: Text Over Image Block:Enabled Lava Commands
            RockMigrationHelper.DeleteAttribute( "3B56BD7B-DFFA-4C07-A936-8B79029C01C5" );
            // Attrib for BlockType: Text Over Image Block:Aspect Ratio
            RockMigrationHelper.DeleteAttribute( "BD4BC1F9-9C20-4A6E-A67D-8D5EF56A619D" );
            // Attrib for BlockType: Text Over Image Block:Text
            RockMigrationHelper.DeleteAttribute( "3C535AE2-5D1D-4634-AFC2-658B4D55A3A8" );
            // Attrib for BlockType: Text Over Image Block:Image
            RockMigrationHelper.DeleteAttribute( "B1DD109B-F099-4C88-AEF4-FDA0959B5530" );
            // Attrib for BlockType: Text Over Image Block:Action Item
            RockMigrationHelper.DeleteAttribute( "0BF7D627-4BD3-4AE3-9AD7-CFABBA6C9799" );

        }
    }
}
