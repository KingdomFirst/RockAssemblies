// <copyright>
// Copyright 2024 by Kingdom First Solutions
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
using Rock.Plugin;

namespace rocks.kfs.MobileAppMigration.Migrations
{
    [MigrationNumber( 1, "1.16.0" )]
    public class AddPagesAndBlocks : Migration
    {
        public override void Up()
        {
            RockMigrationHelper.AddSite( "Kingdom First Solutions Mobile App", "", "", "1DEF6B4D-F796-45E5-9535-BC1A5A1EA327" );

            Sql( @"UPDATE [Site] SET [SiteType] = 1, [IsSystem] = 0 WHERE [Guid] = '1DEF6B4D-F796-45E5-9535-BC1A5A1EA327'" );

            // Site:Kingdom First Solutions Mobile App
            RockMigrationHelper.AddLayout( "1DEF6B4D-F796-45E5-9535-BC1A5A1EA327", "Homepage.xaml", "Homepage", "", "10D01240-BFBD-43FF-89EC-8AB0C837360E" );

            // Site:Kingdom First Solutions Mobile App
            RockMigrationHelper.AddLayout( "1DEF6B4D-F796-45E5-9535-BC1A5A1EA327", "No Scrollview.xaml", "No Scrollview", "", "A7C77AD7-69CB-4FE8-A3F6-AE7E48C53457" );

            // Add Page 
            //  Internal Name: Kingdom First Solutions Mobile App Homepage
            //  Site: Kingdom First Solutions Mobile App
            RockMigrationHelper.AddPage( true, null, "10D01240-BFBD-43FF-89EC-8AB0C837360E", "Kingdom First Solutions Mobile App Homepage", "", "7665A012-33D3-4990-8614-B8274A4221C7", "" );

            Sql( @"DECLARE @PageId int;
                   SELECT @PageId = Id FROM [Page] WHERE [Guid] = '7665A012-33D3-4990-8614-B8274A4221C7';
                   UPDATE [Site] SET [DefaultPageId] = @PageId WHERE [Guid] = '1DEF6B4D-F796-45E5-9535-BC1A5A1EA327'" );

            // Add Page 
            //  Internal Name: Communication View
            //  Site: Kingdom First Solutions Mobile App
            RockMigrationHelper.AddPage( true, "7665A012-33D3-4990-8614-B8274A4221C7", "10D01240-BFBD-43FF-89EC-8AB0C837360E", "Communication View", "", "341B07E0-E710-4C9A-8DAA-A5121917C330", "" );

            // Add Page 
            //  Internal Name: Edit Profile
            //  Site: Kingdom First Solutions Mobile App
            RockMigrationHelper.AddPage( true, "7665A012-33D3-4990-8614-B8274A4221C7", "10D01240-BFBD-43FF-89EC-8AB0C837360E", "Edit Profile", "", "A3929533-0055-44BE-9752-38B9DF024468", "" );

            // Add Page 
            //  Internal Name: C3 Home
            //  Site: Kingdom First Solutions Mobile App
            RockMigrationHelper.AddPage( true, "7665A012-33D3-4990-8614-B8274A4221C7", "10D01240-BFBD-43FF-89EC-8AB0C837360E", "C3 Home", "", "C244F574-D08A-424A-BBE4-9A63F98B18C3", "" );

            // Add Page 
            //  Internal Name: Sunday
            //  Site: Kingdom First Solutions Mobile App
            RockMigrationHelper.AddPage( true, "7665A012-33D3-4990-8614-B8274A4221C7", "10D01240-BFBD-43FF-89EC-8AB0C837360E", "Sunday", "", "CEECD2DD-2AE9-42B8-AA75-0C762444BAF1", "" );

            // Add Page 
            //  Internal Name: Bible
            //  Site: Kingdom First Solutions Mobile App
            RockMigrationHelper.AddPage( true, "7665A012-33D3-4990-8614-B8274A4221C7", "10D01240-BFBD-43FF-89EC-8AB0C837360E", "Bible", "", "8FC1504B-9D6B-4375-822E-4F44C9B939DD", "" );

            // Add Page 
            //  Internal Name: Give
            //  Site: Kingdom First Solutions Mobile App
            RockMigrationHelper.AddPage( true, "7665A012-33D3-4990-8614-B8274A4221C7", "10D01240-BFBD-43FF-89EC-8AB0C837360E", "Give", "", "D71681C4-3662-4E3C-8A8B-9A72844DEA43", "" );

            // Add Page 
            //  Internal Name: Webview - Any URL
            //  Site: Kingdom First Solutions Mobile App
            RockMigrationHelper.AddPage( true, "7665A012-33D3-4990-8614-B8274A4221C7", "A7C77AD7-69CB-4FE8-A3F6-AE7E48C53457", "Webview - Any URL", "", "743CF21F-C3C1-4AF1-8552-97A62A2B609B", "" );

            // Add Page 
            //  Internal Name: Login
            //  Site: Kingdom First Solutions Mobile App
            RockMigrationHelper.AddPage( true, "7665A012-33D3-4990-8614-B8274A4221C7", "10D01240-BFBD-43FF-89EC-8AB0C837360E", "Login", "", "23DCB577-AD0C-4DC6-97A3-2E1F0834D005", "" );

            // Add Page 
            //  Internal Name: Onboard
            //  Site: Kingdom First Solutions Mobile App
            RockMigrationHelper.AddPage( true, "7665A012-33D3-4990-8614-B8274A4221C7", "10D01240-BFBD-43FF-89EC-8AB0C837360E", "Onboard", "", "641C23AE-C04E-4EA6-85E3-B33F2062F36D", "" );

            // Add Page 
            //  Internal Name: Register
            //  Site: Kingdom First Solutions Mobile App
            RockMigrationHelper.AddPage( true, "7665A012-33D3-4990-8614-B8274A4221C7", "10D01240-BFBD-43FF-89EC-8AB0C837360E", "Register", "", "8DFDE07D-E9E0-4C38-99A8-DAC5A7C566DC", "" );

            // Add Page 
            //  Internal Name: My Account/Push Notifications
            //  Site: Kingdom First Solutions Mobile App
            RockMigrationHelper.AddPage( true, "7665A012-33D3-4990-8614-B8274A4221C7", "10D01240-BFBD-43FF-89EC-8AB0C837360E", "My Account/Push Notifications", "", "058F59CD-86D7-420B-81F0-2EFA2543CDDC", "" );

            // Add Page 
            //  Internal Name: My Account/Giving History
            //  Site: Kingdom First Solutions Mobile App
            RockMigrationHelper.AddPage( true, "7665A012-33D3-4990-8614-B8274A4221C7", "10D01240-BFBD-43FF-89EC-8AB0C837360E", "My Account/Giving History", "", "0A152319-16B7-4899-B803-5780130E96A8", "" );

            // Add Page 
            //  Internal Name: Push Notifications
            //  Site: Kingdom First Solutions Mobile App
            RockMigrationHelper.AddPage( true, "7665A012-33D3-4990-8614-B8274A4221C7", "A7C77AD7-69CB-4FE8-A3F6-AE7E48C53457", "Push Notifications", "", "751D63CC-1E10-4199-BB99-D6F411AF3BA6", "" );

            // Add Page 
            //  Internal Name: Push Notification - Detail
            //  Site: Kingdom First Solutions Mobile App
            RockMigrationHelper.AddPage( true, "7665A012-33D3-4990-8614-B8274A4221C7", "10D01240-BFBD-43FF-89EC-8AB0C837360E", "Push Notification - Detail", "", "4D7C4316-56F7-4BE7-A182-0074F47081EE", "" );

            // Add Page 
            //  Internal Name: Giving History - All Transactions
            //  Site: Kingdom First Solutions Mobile App
            RockMigrationHelper.AddPage( true, "7665A012-33D3-4990-8614-B8274A4221C7", "10D01240-BFBD-43FF-89EC-8AB0C837360E", "Giving History - All Transactions", "", "1E0EE431-671D-4AD4-9A31-83C12CDE4D19", "" );

            // Add Page 
            //  Internal Name: Prayer
            //  Site: Kingdom First Solutions Mobile App
            RockMigrationHelper.AddPage( true, "7665A012-33D3-4990-8614-B8274A4221C7", "10D01240-BFBD-43FF-89EC-8AB0C837360E", "Prayer", "", "6D328172-D7EE-434B-BD79-A4D7ECA14684", "" );

            // Add Page 
            //  Internal Name: Prayer - Prayer Wall
            //  Site: Kingdom First Solutions Mobile App
            RockMigrationHelper.AddPage( true, "7665A012-33D3-4990-8614-B8274A4221C7", "10D01240-BFBD-43FF-89EC-8AB0C837360E", "Prayer - Prayer Wall", "", "77F98E18-2E27-4D4A-B4CF-2FFA285A7014", "" );

            // Add Page 
            //  Internal Name: Prayer - Submit Prayer
            //  Site: Kingdom First Solutions Mobile App
            RockMigrationHelper.AddPage( true, "7665A012-33D3-4990-8614-B8274A4221C7", "10D01240-BFBD-43FF-89EC-8AB0C837360E", "Prayer - Submit Prayer", "", "2A94E058-189A-49CD-AC22-99677CBDB351", "" );

            // Add Page 
            //  Internal Name: Prayer - Private Entry
            //  Site: Kingdom First Solutions Mobile App
            RockMigrationHelper.AddPage( true, "7665A012-33D3-4990-8614-B8274A4221C7", "10D01240-BFBD-43FF-89EC-8AB0C837360E", "Prayer - Private Entry", "", "9A40B634-0E80-4207-BF0B-3050EFD49516", "" );

            // Add Page 
            //  Internal Name: Prayer Session
            //  Site: Kingdom First Solutions Mobile App
            RockMigrationHelper.AddPage( true, "7665A012-33D3-4990-8614-B8274A4221C7", "10D01240-BFBD-43FF-89EC-8AB0C837360E", "Prayer Session", "", "68EAF2D8-688C-44C3-9222-340047E07095", "" );

            // Add Page 
            //  Internal Name: Communication Preferences
            //  Site: Kingdom First Solutions Mobile App
            RockMigrationHelper.AddPage( true, "7665A012-33D3-4990-8614-B8274A4221C7", "10D01240-BFBD-43FF-89EC-8AB0C837360E", "Communication Preferences", "", "D426B3D7-ADCD-4516-B539-FDD6426DD230", "" );

            // Add Page 
            //  Internal Name: Content Item View
            //  Site: Kingdom First Solutions Mobile App
            RockMigrationHelper.AddPage( true, "7665A012-33D3-4990-8614-B8274A4221C7", "10D01240-BFBD-43FF-89EC-8AB0C837360E", "Content Item View", "", "FDF02B2C-F252-4F17-A947-C34957F1E2CC", "" );

            // Add Page 
            //  Internal Name: Group Detail
            //  Site: Kingdom First Solutions Mobile App
            RockMigrationHelper.AddPage( true, "7665A012-33D3-4990-8614-B8274A4221C7", "10D01240-BFBD-43FF-89EC-8AB0C837360E", "Group Detail", "", "EFB5029C-0BEA-4ADE-BFED-2E75B99A46D4", "" );

            // Add Page 
            //  Internal Name: C3 Home - Ministries
            //  Site: Kingdom First Solutions Mobile App
            RockMigrationHelper.AddPage( true, "7665A012-33D3-4990-8614-B8274A4221C7", "10D01240-BFBD-43FF-89EC-8AB0C837360E", "C3 Home - Ministries", "", "A95A3FF3-9287-4B50-99C7-ED01D00BAB2B", "" );

            // Add Page 
            //  Internal Name: C3 Home - About
            //  Site: Kingdom First Solutions Mobile App
            RockMigrationHelper.AddPage( true, "7665A012-33D3-4990-8614-B8274A4221C7", "10D01240-BFBD-43FF-89EC-8AB0C837360E", "C3 Home - About", "", "17025FF4-2717-44F6-A4B7-B42A24600576", "" );

            // Add Page 
            //  Internal Name: C3 Home - Welcome
            //  Site: Kingdom First Solutions Mobile App
            RockMigrationHelper.AddPage( true, "7665A012-33D3-4990-8614-B8274A4221C7", "10D01240-BFBD-43FF-89EC-8AB0C837360E", "C3 Home - Welcome", "", "649DDAB5-0872-4051-9E57-BE986884A0EC", "" );

            // Add Page 
            //  Internal Name: C3 Home - Ministries
            //  Site: Kingdom First Solutions Mobile App
            RockMigrationHelper.AddPage( true, "7665A012-33D3-4990-8614-B8274A4221C7", "10D01240-BFBD-43FF-89EC-8AB0C837360E", "C3 Home - Ministries", "", "78F86495-2973-4173-B9B7-5A7D32D6DBBB", "" );

            // Add Page 
            //  Internal Name: C3 Home - Coming Up
            //  Site: Kingdom First Solutions Mobile App
            RockMigrationHelper.AddPage( true, "7665A012-33D3-4990-8614-B8274A4221C7", "10D01240-BFBD-43FF-89EC-8AB0C837360E", "C3 Home - Coming Up", "", "81DEF3F4-FE28-433B-9B16-C1462545EBA3", "" );

            // Add Page 
            //  Internal Name: C3 Home - What's Next
            //  Site: Kingdom First Solutions Mobile App
            RockMigrationHelper.AddPage( true, "7665A012-33D3-4990-8614-B8274A4221C7", "10D01240-BFBD-43FF-89EC-8AB0C837360E", "C3 Home - Whats Next", "", "FC76C90A-D72B-4772-9DCE-DBC3CB11C545", "" );

            // Add Page 
            //  Internal Name: My Groups
            //  Site: Kingdom First Solutions Mobile App
            RockMigrationHelper.AddPage( true, "7665A012-33D3-4990-8614-B8274A4221C7", "10D01240-BFBD-43FF-89EC-8AB0C837360E", "My Groups", "", "6944F938-3379-4E0C-8FB4-188FCB383BB6", "" );

            // Add Page 
            //  Internal Name: Group Finder
            //  Site: Kingdom First Solutions Mobile App
            RockMigrationHelper.AddPage( true, "7665A012-33D3-4990-8614-B8274A4221C7", "10D01240-BFBD-43FF-89EC-8AB0C837360E", "Group Finder", "", "3696C320-3C61-476F-94A8-11E934891D67", "" );

            // Add Page 
            //  Internal Name: Group Edit
            //  Site: Kingdom First Solutions Mobile App
            RockMigrationHelper.AddPage( true, "7665A012-33D3-4990-8614-B8274A4221C7", "10D01240-BFBD-43FF-89EC-8AB0C837360E", "Group Edit", "", "367B9E49-0330-4890-B554-BA7F034ADC9B", "" );

            // Add Page 
            //  Internal Name: Group Member View
            //  Site: Kingdom First Solutions Mobile App
            RockMigrationHelper.AddPage( true, "7665A012-33D3-4990-8614-B8274A4221C7", "10D01240-BFBD-43FF-89EC-8AB0C837360E", "Group Member View", "", "78671DCD-A1E0-461F-8F28-B0C10467BE57", "" );

            // Add Page 
            //  Internal Name: Group Registration
            //  Site: Kingdom First Solutions Mobile App
            RockMigrationHelper.AddPage( true, "7665A012-33D3-4990-8614-B8274A4221C7", "10D01240-BFBD-43FF-89EC-8AB0C837360E", "Group Registration", "", "B8C9AF1D-7EEE-45C0-8619-93ABBF528F9B", "" );

            // Add Page 
            //  Internal Name: Watch
            //  Site: Kingdom First Solutions Mobile App
            RockMigrationHelper.AddPage( true, "7665A012-33D3-4990-8614-B8274A4221C7", "A7C77AD7-69CB-4FE8-A3F6-AE7E48C53457", "Watch", "", "4D192169-934E-491D-A1B1-5D4B759BA94E", "" );

            // Add Page 
            //  Internal Name: Series Detail
            //  Site: Kingdom First Solutions Mobile App
            RockMigrationHelper.AddPage( true, "7665A012-33D3-4990-8614-B8274A4221C7", "10D01240-BFBD-43FF-89EC-8AB0C837360E", "Series Detail", "", "D035D65C-EB4F-4DAD-8EAC-47478042CA8C", "" );

            // Add Page 
            //  Internal Name: Watch - Alternate
            //  Site: Kingdom First Solutions Mobile App
            RockMigrationHelper.AddPage( true, "7665A012-33D3-4990-8614-B8274A4221C7", "10D01240-BFBD-43FF-89EC-8AB0C837360E", "Watch - Alternate", "", "5F5D7552-1093-4E40-AF21-28AFA489D0A8", "" );

            // Add Page 
            //  Internal Name: Message Detail
            //  Site: Kingdom First Solutions Mobile App
            RockMigrationHelper.AddPage( true, "7665A012-33D3-4990-8614-B8274A4221C7", "10D01240-BFBD-43FF-89EC-8AB0C837360E", "Message Detail", "", "A5733C5C-7329-4A64-8FDB-228EACA7E601", "" );

            // Add Page 
            //  Internal Name: Take Notes
            //  Site: Kingdom First Solutions Mobile App
            RockMigrationHelper.AddPage( true, "7665A012-33D3-4990-8614-B8274A4221C7", "10D01240-BFBD-43FF-89EC-8AB0C837360E", "Take Notes", "", "AA18A142-B85A-4373-AF92-A2E3BD329197", "" );

            // Add Page 
            //  Internal Name: Listen
            //  Site: Kingdom First Solutions Mobile App
            RockMigrationHelper.AddPage( true, "7665A012-33D3-4990-8614-B8274A4221C7", "10D01240-BFBD-43FF-89EC-8AB0C837360E", "Listen", "", "B84CDC78-0C4E-4D59-A68F-157F01F6B9E7", "" );


            Sql( @"DECLARE @ApiKeyId int, @ProfilePageId int, @CommunicationViewPageId int;
                   SELECT @ProfilePageId = Id FROM [Page] WHERE [Guid] = 'A3929533-0055-44BE-9752-38B9DF024468';
                   SELECT @CommunicationViewPageId = Id FROM [Page] WHERE [Guid] = '341B07E0-E710-4C9A-8DAA-A5121917C330';
                   UPDATE [Site] SET [AdditionalSettings] = '{""LastDeploymentDate"":null,""LastDeploymentVersionId"":null,""PhoneUpdatePackageUrl"":null,""TabletUpdatePackageUrl"":null,""ShellType"":2,""TabLocation"":1,""CssStyle"":"".tab-bar {\n -xf-bar-background-color: #709bb5;\n -rock-selected-tab-color: #ffffff;\n -rock-unselected-tab-color: #99ffffff;\n}"",""ApiKeyId"":null,""ProfilePageId"":'+ @ProfilePageId + ',""PersonAttributeCategories"":[0],""BarBackgroundColor"":""#709bb5"",""MenuButtonColor"":""#ffffff"",""ActivityIndicatorColor"":""#709bb5"",""FlyoutXaml"":""<ListView SeparatorVisibility=\""None\"" \n    HasUnevenRows=\""true\"" \n    ItemsSource=\""{Binding MenuItems}\"">\n\n    <ListView.Header>\n        <StackLayout VerticalOptions=\""FillAndExpand\""\n            Orientation=\""Vertical\"">\n\n            <Rock:LoginStatus Padding=\""20, 70, 20, 50\"" \n                ImageSize=\""120\"" \n                ImageBorderColor=\""rgba(255, 255, 255, 0.4)\"" \n                ImageBorderSize=\""5\"" />\n\n            <BoxView HeightRequest=\""1\"" BackgroundColor=\""rgba(255, 255, 255, 0.2)\""\n                HorizontalOptions=\""FillAndExpand\""/>\n\n        </StackLayout>\n    </ListView.Header>\n\n    <ListView.ItemTemplate>\n        <DataTemplate>\n            <Rock:ViewCell SelectedBackgroundColor=\""rgba(255, 255, 255, 0.2)\"">\n            \n                <StackLayout VerticalOptions=\""FillAndExpand\"" \n                    Orientation=\""Vertical\"">\n\n                    <ContentView StyleClass=\""pt-16, pb-12\"">\n                        <Label StyleClass=\""text-white, ml-32, flyout-menu-item\""\n                            Text=\""{Binding Title}\"" \n                            VerticalOptions=\""Center\"" \n                            HorizontalOptions=\""FillAndExpand\"" />\n                    </ContentView>\n\n                    <BoxView HeightRequest=\""1\""\n                        BackgroundColor=\""rgba(255, 255, 255, 0.4)\""\n                        HorizontalOptions=\""FillAndExpand\"" />\n\n                </StackLayout>\n\n            </Rock:ViewCell>\n        </DataTemplate>\n    </ListView.ItemTemplate>\n\n</ListView>"",""ToastXaml"":""<StackLayout>\n <Frame HasShadow=\""False\"">\n <StackLayout>\n {% if ToastTitle != '' %}\n <Label StyleClass=\""title\"" Text=\""{Binding ToastTitle}\"" />\n {% endif %}\n <Label Text=\""{Binding ToastMessage}\"" />\n </StackLayout>\n </Frame>\n</StackLayout>"",""LockedPhoneOrientation"":1,""LockedTabletOrientation"":0,""DownhillSettings"":{""SpacingValues"":{""0"":""0"",""1"":""1"",""2"":""2"",""4"":""4"",""8"":""8"",""12"":""12"",""16"":""16"",""24"":""24"",""32"":""32"",""64"":""64""},""SpacingUnits"":"""",""FontSizes"":{""xs"":0.75,""sm"":0.875,""base"":1.0,""lg"":1.125,""xl"":1.25,""2xl"":1.5,""3xl"":1.875,""4xl"":2.25,""5xl"":3.0,""6xl"":4.0},""BorderWidths"":[0,1,2,4,8],""Platform"":0,""BorderUnits"":"""",""FontUnits"":"""",""FontSizeDefault"":16.0,""ApplicationColors"":{""Primary"":""#6f9bb4"",""Secondary"":""#6c757d"",""Success"":""#28a745"",""Danger"":""#dc3545"",""Warning"":""#ffc107"",""Info"":""#17a2b8"",""Light"":""#f8f9fa"",""Dark"":""#343a40"",""White"":""#ffffff"",""Brand"":""#6f9bb4""},""RadiusBase"":0.0,""TextColor"":""#676767"",""HeadingColor"":""#709bb5"",""BackgroundColor"":""#ffffff"",""AdditionalCssToParse"":{}},""NavigationBarActionXaml"":""<Rock:LoginStatusPhoto StyleClass=\""p-8\"" NotLoggedInCommand=\""{Binding PushPage}\""\n NotLoggedInCommandParameter=\""641c23ae-c04e-4ea6-85e3-b33f2062f36d\"" LoggedInCommand=\""{Binding PushPage}\""\n LoggedInCommandParameter=\""058f59cd-86d7-420b-81f0-2efa2543cddc\"" ProfilePhotoCircle=\""true\"" ProfilePhotoStrokeWidth=\""1\"" />"",""HomepageRoutingLogic"":"""",""CampusFilterDataViewId"":null,""CommunicationViewPageId"":'+ @CommunicationViewPageId + ',""EnableNotificationsAutomatically"":true,""PushTokenUpdateValue"":"""",""IsDeepLinkingEnabled"":false,""BundleIdentifier"":null,""TeamIdentifier"":null,""PackageName"":null,""CertificateFingerprint"":null,""DeepLinkPathPrefix"":null,""DeepLinkRoutes"":[],""DeepLinkDomains"":null,""IsPackageCompressionEnabled"":true,""Auth0Domain"":"""",""Auth0ClientId"":"""",""EntraClientId"":"""",""EntraTenantId"":"""",""EntraAuthenticationComponent"":null}' WHERE [Guid] = '1DEF6B4D-F796-45E5-9535-BC1A5A1EA327'" );

            // Add Block 
            //  Block Name: Content
            //  Page Name: Kingdom First Solutions Mobile App Homepage
            //  Layout: -
            //  Site: Kingdom First Solutions Mobile App
            RockMigrationHelper.AddBlock( true, "7665A012-33D3-4990-8614-B8274A4221C7".AsGuid(), null, "1DEF6B4D-F796-45E5-9535-BC1A5A1EA327".AsGuid(), "7258A210-E936-4260-B573-9FA1193AD9E2".AsGuid(), "Content", "Main", @"", @"", 0, "B404A4C5-B9E8-4721-B14D-9AD560EC2E6B" );

            // Add Block 
            //  Block Name: Content - Device Info
            //  Page Name: Kingdom First Solutions Mobile App Homepage
            //  Layout: -
            //  Site: Kingdom First Solutions Mobile App
            RockMigrationHelper.AddBlock( true, "7665A012-33D3-4990-8614-B8274A4221C7".AsGuid(), null, "1DEF6B4D-F796-45E5-9535-BC1A5A1EA327".AsGuid(), "7258A210-E936-4260-B573-9FA1193AD9E2".AsGuid(), "Content - Device Info", "Main", @"", @"", 2, "7D8962A6-94D0-48CE-9B25-9495DFA3409F" );

            // Add Block 
            //  Block Name: Static Card Content
            //  Page Name: Kingdom First Solutions Mobile App Homepage
            //  Layout: -
            //  Site: Kingdom First Solutions Mobile App
            RockMigrationHelper.AddBlock( true, "7665A012-33D3-4990-8614-B8274A4221C7".AsGuid(), null, "1DEF6B4D-F796-45E5-9535-BC1A5A1EA327".AsGuid(), "7258A210-E936-4260-B573-9FA1193AD9E2".AsGuid(), "Static Card Content", "Main", @"", @"", 1, "31AFE825-7065-4A67-8C7B-2CDABC44FBDD" );

            // Add Block 
            //  Block Name: Communication View
            //  Page Name: Communication View
            //  Layout: -
            //  Site: Kingdom First Solutions Mobile App
            RockMigrationHelper.AddBlock( true, "341B07E0-E710-4C9A-8DAA-A5121917C330".AsGuid(), null, "1DEF6B4D-F796-45E5-9535-BC1A5A1EA327".AsGuid(), "863E5638-B310-407E-A54E-2C069979881D".AsGuid(), "Communication View", "Main", @"", @"", 0, "43F9DF25-984E-43F9-BF99-5DE58D98D457" );

            // Add Block 
            //  Block Name: Profile Details
            //  Page Name: Edit Profile
            //  Layout: -
            //  Site: Kingdom First Solutions Mobile App
            RockMigrationHelper.AddBlock( true, "A3929533-0055-44BE-9752-38B9DF024468".AsGuid(), null, "1DEF6B4D-F796-45E5-9535-BC1A5A1EA327".AsGuid(), "66B2B513-1C71-4E6B-B4BE-C4EF90E1899C".AsGuid(), "Profile Details", "Main", @"", @"", 0, "1314F629-6204-48AB-8422-5B0EB55FEE23" );

            // Add Block 
            //  Block Name: Content
            //  Page Name: C3 Home
            //  Layout: -
            //  Site: Kingdom First Solutions Mobile App
            RockMigrationHelper.AddBlock( true, "C244F574-D08A-424A-BBE4-9A63F98B18C3".AsGuid(), null, "1DEF6B4D-F796-45E5-9535-BC1A5A1EA327".AsGuid(), "7258A210-E936-4260-B573-9FA1193AD9E2".AsGuid(), "Content", "Main", @"", @"", 0, "CFAD5C83-1A6A-4EA9-B664-19007724C3FF" );

            // Add Block 
            //  Block Name: Content
            //  Page Name: Sunday
            //  Layout: -
            //  Site: Kingdom First Solutions Mobile App
            RockMigrationHelper.AddBlock( true, "CEECD2DD-2AE9-42B8-AA75-0C762444BAF1".AsGuid(), null, "1DEF6B4D-F796-45E5-9535-BC1A5A1EA327".AsGuid(), "7258A210-E936-4260-B573-9FA1193AD9E2".AsGuid(), "Content", "Main", @"", @"", 0, "7D1B6CEA-34E1-4214-B772-9B8FD5CA2B5A" );

            // Add Block 
            //  Block Name: Content
            //  Page Name: Bible
            //  Layout: -
            //  Site: Kingdom First Solutions Mobile App
            RockMigrationHelper.AddBlock( true, "8FC1504B-9D6B-4375-822E-4F44C9B939DD".AsGuid(), null, "1DEF6B4D-F796-45E5-9535-BC1A5A1EA327".AsGuid(), "7258A210-E936-4260-B573-9FA1193AD9E2".AsGuid(), "Content", "Main", @"", @"", 1, "E183F13B-6745-4F5E-A529-F680E3BD2C91" );

            // Add Block 
            //  Block Name: Hero
            //  Page Name: Bible
            //  Layout: -
            //  Site: Kingdom First Solutions Mobile App
            RockMigrationHelper.AddBlock( true, "8FC1504B-9D6B-4375-822E-4F44C9B939DD".AsGuid(), null, "1DEF6B4D-F796-45E5-9535-BC1A5A1EA327".AsGuid(), "A8597994-BD47-4A15-8BB1-4B508977665F".AsGuid(), "Hero", "Main", @"", @"", 0, "5E44F0B2-307D-4659-AA00-2EFC9BF27DE9" );

            // Add Block 
            //  Block Name: Content
            //  Page Name: Give
            //  Layout: -
            //  Site: Kingdom First Solutions Mobile App
            RockMigrationHelper.AddBlock( true, "D71681C4-3662-4E3C-8A8B-9A72844DEA43".AsGuid(), null, "1DEF6B4D-F796-45E5-9535-BC1A5A1EA327".AsGuid(), "7258A210-E936-4260-B573-9FA1193AD9E2".AsGuid(), "Content", "Main", @"", @"", 0, "E9ECA833-8D2D-4A2D-AA00-B57178435285" );

            // Add Block 
            //  Block Name: Content Channel Item View
            //  Page Name: Series Detail
            //  Layout: -
            //  Site: Kingdom First Solutions Mobile App
            RockMigrationHelper.AddBlock( true, "D035D65C-EB4F-4DAD-8EAC-47478042CA8C".AsGuid(), null, "1DEF6B4D-F796-45E5-9535-BC1A5A1EA327".AsGuid(), "B76B5F10-D2D6-4C60-B6FB-F913A62442E0".AsGuid(), "Content Channel Item View", "Main", @"", @"", 0, "C65485AB-BB67-43E2-9C2A-CAFEA3BD2E14" );

            // Add Block 
            //  Block Name: Content
            //  Page Name: Watch - Alternate
            //  Layout: -
            //  Site: Kingdom First Solutions Mobile App
            RockMigrationHelper.AddBlock( true, "5F5D7552-1093-4E40-AF21-28AFA489D0A8".AsGuid(), null, "1DEF6B4D-F796-45E5-9535-BC1A5A1EA327".AsGuid(), "7258A210-E936-4260-B573-9FA1193AD9E2".AsGuid(), "Content", "Main", @"", @"", 0, "37708AD6-4902-4CEA-BECE-A7FF5517999E" );

            // Add Block 
            //  Block Name: Content Channel Item View
            //  Page Name: Message Detail
            //  Layout: -
            //  Site: Kingdom First Solutions Mobile App
            RockMigrationHelper.AddBlock( true, "A5733C5C-7329-4A64-8FDB-228EACA7E601".AsGuid(), null, "1DEF6B4D-F796-45E5-9535-BC1A5A1EA327".AsGuid(), "B76B5F10-D2D6-4C60-B6FB-F913A62442E0".AsGuid(), "Content Channel Item View", "Main", @"", @"", 0, "287ADEF9-787B-43E8-A6AE-7616E86866A5" );

            // Add Block 
            //  Block Name: Structured Content View
            //  Page Name: Take Notes
            //  Layout: -
            //  Site: Kingdom First Solutions Mobile App
            RockMigrationHelper.AddBlock( true, "AA18A142-B85A-4373-AF92-A2E3BD329197".AsGuid(), null, "1DEF6B4D-F796-45E5-9535-BC1A5A1EA327".AsGuid(), "A8BBE3F8-F3CC-4C0A-AB2F-5085F5BF59E7".AsGuid(), "Structured Content View", "Main", @"", @"", 0, "8800E9AF-E501-4DA3-B634-4E5A0B28C104" );

            // Add Block 
            //  Block Name: Content Channel Item View
            //  Page Name: Listen
            //  Layout: -
            //  Site: Kingdom First Solutions Mobile App
            RockMigrationHelper.AddBlock( true, "B84CDC78-0C4E-4D59-A68F-157F01F6B9E7".AsGuid(), null, "1DEF6B4D-F796-45E5-9535-BC1A5A1EA327".AsGuid(), "B76B5F10-D2D6-4C60-B6FB-F913A62442E0".AsGuid(), "Content Channel Item View", "Main", @"", @"", 0, "D1B4F2D2-6760-4B02-8488-4EA0B06FFFAD" );

            // Add Block 
            //  Block Name: Login
            //  Page Name: Login
            //  Layout: -
            //  Site: Kingdom First Solutions Mobile App
            RockMigrationHelper.AddBlock( true, "23DCB577-AD0C-4DC6-97A3-2E1F0834D005".AsGuid(), null, "1DEF6B4D-F796-45E5-9535-BC1A5A1EA327".AsGuid(), "6006FE32-DC01-4B1C-A9B8-EE172451F4C5".AsGuid(), "Login", "Main", @"", @"", 1, "4B26BE7A-A7A3-44D8-BD2B-B87506383D58" );

            // Add Block 
            //  Block Name: Content
            //  Page Name: Login
            //  Layout: -
            //  Site: Kingdom First Solutions Mobile App
            RockMigrationHelper.AddBlock( true, "23DCB577-AD0C-4DC6-97A3-2E1F0834D005".AsGuid(), null, "1DEF6B4D-F796-45E5-9535-BC1A5A1EA327".AsGuid(), "7258A210-E936-4260-B573-9FA1193AD9E2".AsGuid(), "Content", "Main", @"", @"", 0, "3245F127-A3B7-41A8-B004-945C726A1B94" );

            // Add Block 
            //  Block Name: Onboard Person
            //  Page Name: Onboard
            //  Layout: -
            //  Site: Kingdom First Solutions Mobile App
            RockMigrationHelper.AddBlock( true, "641C23AE-C04E-4EA6-85E3-B33F2062F36D".AsGuid(), null, "1DEF6B4D-F796-45E5-9535-BC1A5A1EA327".AsGuid(), "9544EE9E-07C2-4F14-9C93-3B16EBF0CC47".AsGuid(), "Onboard Person", "Main", @"", @"", 0, "41D6C985-C794-46E2-A2F4-10745072C68D" );

            // Add Block 
            //  Block Name: Register
            //  Page Name: Register
            //  Layout: -
            //  Site: Kingdom First Solutions Mobile App
            RockMigrationHelper.AddBlock( true, "8DFDE07D-E9E0-4C38-99A8-DAC5A7C566DC".AsGuid(), null, "1DEF6B4D-F796-45E5-9535-BC1A5A1EA327".AsGuid(), "2A71FDA2-5204-418F-858E-693A1F4E9A49".AsGuid(), "Register", "Main", @"", @"", 0, "A6C0DE83-D710-4062-8C4D-BDF6900C36B8" );

            // Add Block 
            //  Block Name: Push Notification Content - Recipients
            //  Page Name: My Account/Push Notifications
            //  Layout: -
            //  Site: Kingdom First Solutions Mobile App
            RockMigrationHelper.AddBlock( true, "058F59CD-86D7-420B-81F0-2EFA2543CDDC".AsGuid(), null, "1DEF6B4D-F796-45E5-9535-BC1A5A1EA327".AsGuid(), "7258A210-E936-4260-B573-9FA1193AD9E2".AsGuid(), "Push Notification Content - Recipients", "Main", @"", @"", 1, "72D2F125-C190-427D-B96E-03C1F59A9CB7" );

            // Add Block 
            //  Block Name: ContentPush Notification - Recipients Shell 2.0
            //  Page Name: My Account/Push Notifications
            //  Layout: -
            //  Site: Kingdom First Solutions Mobile App
            RockMigrationHelper.AddBlock( true, "058F59CD-86D7-420B-81F0-2EFA2543CDDC".AsGuid(), null, "1DEF6B4D-F796-45E5-9535-BC1A5A1EA327".AsGuid(), "7258A210-E936-4260-B573-9FA1193AD9E2".AsGuid(), "ContentPush Notification - Recipients Shell 2.0", "Main", @"", @"", 2, "3EF8CA12-457A-40B5-856B-D8CA240D2CDC" );

            // Add Block 
            //  Block Name: Content
            //  Page Name: My Account/Push Notifications
            //  Layout: -
            //  Site: Kingdom First Solutions Mobile App
            RockMigrationHelper.AddBlock( true, "058F59CD-86D7-420B-81F0-2EFA2543CDDC".AsGuid(), null, "1DEF6B4D-F796-45E5-9535-BC1A5A1EA327".AsGuid(), "7258A210-E936-4260-B573-9FA1193AD9E2".AsGuid(), "Content", "Main", @"", @"", 0, "48A47402-C0B4-4820-8017-BB7EF63A60B5" );

            // Add Block 
            //  Block Name: Push Notification Content - All Communications to Devices, Lists and existing notifications?
            //  Page Name: My Account/Push Notifications
            //  Layout: -
            //  Site: Kingdom First Solutions Mobile App
            RockMigrationHelper.AddBlock( true, "058F59CD-86D7-420B-81F0-2EFA2543CDDC".AsGuid(), null, "1DEF6B4D-F796-45E5-9535-BC1A5A1EA327".AsGuid(), "7258A210-E936-4260-B573-9FA1193AD9E2".AsGuid(), "Push Notification Content - All Communications to Devices, Lists and existing notifications?", "Main", @"", @"", 3, "D9E72CCB-C386-403E-A44E-BC36EA56227E" );

            // Add Block 
            //  Block Name: Content
            //  Page Name: My Account/Giving History
            //  Layout: -
            //  Site: Kingdom First Solutions Mobile App
            RockMigrationHelper.AddBlock( true, "0A152319-16B7-4899-B803-5780130E96A8".AsGuid(), null, "1DEF6B4D-F796-45E5-9535-BC1A5A1EA327".AsGuid(), "7258A210-E936-4260-B573-9FA1193AD9E2".AsGuid(), "Content", "Main", @"", @"", 0, "C8E2F42C-322F-4FD2-B886-571E9A202175" );

            // Add Block 
            //  Block Name: Content - Contribution List
            //  Page Name: My Account/Giving History
            //  Layout: -
            //  Site: Kingdom First Solutions Mobile App
            RockMigrationHelper.AddBlock( true, "0A152319-16B7-4899-B803-5780130E96A8".AsGuid(), null, "1DEF6B4D-F796-45E5-9535-BC1A5A1EA327".AsGuid(), "7258A210-E936-4260-B573-9FA1193AD9E2".AsGuid(), "Content - Contribution List", "Main", @"", @"", 1, "8BCF8A77-51AD-4797-8CE8-43EBC4B84E41" );

            // Add Block 
            //  Block Name: Content
            //  Page Name: Push Notification - Detail
            //  Layout: -
            //  Site: Kingdom First Solutions Mobile App
            RockMigrationHelper.AddBlock( true, "4D7C4316-56F7-4BE7-A182-0074F47081EE".AsGuid(), null, "1DEF6B4D-F796-45E5-9535-BC1A5A1EA327".AsGuid(), "7258A210-E936-4260-B573-9FA1193AD9E2".AsGuid(), "Content", "Main", @"", @"", 0, "3279A50D-8065-434B-90A6-2A17D0579ED6" );

            // Add Block 
            //  Block Name: Content
            //  Page Name: Giving History - All Transactions
            //  Layout: -
            //  Site: Kingdom First Solutions Mobile App
            RockMigrationHelper.AddBlock( true, "1E0EE431-671D-4AD4-9A31-83C12CDE4D19".AsGuid(), null, "1DEF6B4D-F796-45E5-9535-BC1A5A1EA327".AsGuid(), "7258A210-E936-4260-B573-9FA1193AD9E2".AsGuid(), "Content", "Main", @"", @"", 0, "4AA61BDD-7BE9-4910-89E7-9895B711C1EF" );

            // Add Block 
            //  Block Name: Content
            //  Page Name: Prayer
            //  Layout: -
            //  Site: Kingdom First Solutions Mobile App
            RockMigrationHelper.AddBlock( true, "6D328172-D7EE-434B-BD79-A4D7ECA14684".AsGuid(), null, "1DEF6B4D-F796-45E5-9535-BC1A5A1EA327".AsGuid(), "7258A210-E936-4260-B573-9FA1193AD9E2".AsGuid(), "Content", "Main", @"", @"", 1, "C2B854DD-646A-4ED8-8C99-818F19521E70" );

            // Add Block 
            //  Block Name: Hero
            //  Page Name: Prayer
            //  Layout: -
            //  Site: Kingdom First Solutions Mobile App
            RockMigrationHelper.AddBlock( true, "6D328172-D7EE-434B-BD79-A4D7ECA14684".AsGuid(), null, "1DEF6B4D-F796-45E5-9535-BC1A5A1EA327".AsGuid(), "A8597994-BD47-4A15-8BB1-4B508977665F".AsGuid(), "Hero", "Main", @"", @"", 0, "A95B3556-75EC-4C56-89D4-0BE7A25B1F71" );

            // Add Block 
            //  Block Name: Prayer Session Setup
            //  Page Name: Prayer
            //  Layout: -
            //  Site: Kingdom First Solutions Mobile App
            RockMigrationHelper.AddBlock( true, "6D328172-D7EE-434B-BD79-A4D7ECA14684".AsGuid(), null, "1DEF6B4D-F796-45E5-9535-BC1A5A1EA327".AsGuid(), "4A3B0D13-FC32-4354-A224-9D450F860BE9".AsGuid(), "Prayer Session Setup", "Main", @"", @"", 2, "AE875F67-394C-4BED-8C2D-8937F713F5BA" );

            // Add Block 
            //  Block Name: Hero
            //  Page Name: Prayer - Prayer Wall
            //  Layout: -
            //  Site: Kingdom First Solutions Mobile App
            RockMigrationHelper.AddBlock( true, "77F98E18-2E27-4D4A-B4CF-2FFA285A7014".AsGuid(), null, "1DEF6B4D-F796-45E5-9535-BC1A5A1EA327".AsGuid(), "A8597994-BD47-4A15-8BB1-4B508977665F".AsGuid(), "Hero", "Main", @"", @"", 0, "FFA5E7C3-8086-4D35-B2CB-B0964C496F16" );

            // Add Block 
            //  Block Name: Prayer Card View
            //  Page Name: Prayer - Prayer Wall
            //  Layout: -
            //  Site: Kingdom First Solutions Mobile App
            RockMigrationHelper.AddBlock( true, "77F98E18-2E27-4D4A-B4CF-2FFA285A7014".AsGuid(), null, "1DEF6B4D-F796-45E5-9535-BC1A5A1EA327".AsGuid(), "CA75C558-9345-47E7-99AF-D8191D31D00D".AsGuid(), "Prayer Card View", "Main", @"", @"", 2, "AABA341F-A6EF-45F9-B54D-362CDC892CA2" );

            // Add Block 
            //  Block Name: Content
            //  Page Name: Prayer - Prayer Wall
            //  Layout: -
            //  Site: Kingdom First Solutions Mobile App
            RockMigrationHelper.AddBlock( true, "77F98E18-2E27-4D4A-B4CF-2FFA285A7014".AsGuid(), null, "1DEF6B4D-F796-45E5-9535-BC1A5A1EA327".AsGuid(), "7258A210-E936-4260-B573-9FA1193AD9E2".AsGuid(), "Content", "Main", @"", @"", 1, "2A651146-7897-4E33-AB1E-70A53B38449B" );

            // Add Block 
            //  Block Name: Prayer Request Details
            //  Page Name: Prayer - Submit Prayer
            //  Layout: -
            //  Site: Kingdom First Solutions Mobile App
            RockMigrationHelper.AddBlock( true, "2A94E058-189A-49CD-AC22-99677CBDB351".AsGuid(), null, "1DEF6B4D-F796-45E5-9535-BC1A5A1EA327".AsGuid(), "EBB91B46-292E-4784-9E37-38781C714008".AsGuid(), "Prayer Request Details", "Main", @"", @"", 0, "423C035E-9F3A-40F9-ABA6-F07474A375AF" );

            // Add Block 
            //  Block Name: Prayer Request Details
            //  Page Name: Prayer - Private Entry
            //  Layout: -
            //  Site: Kingdom First Solutions Mobile App
            RockMigrationHelper.AddBlock( true, "9A40B634-0E80-4207-BF0B-3050EFD49516".AsGuid(), null, "1DEF6B4D-F796-45E5-9535-BC1A5A1EA327".AsGuid(), "EBB91B46-292E-4784-9E37-38781C714008".AsGuid(), "Prayer Request Details", "Main", @"", @"", 1, "F89758C2-F1B2-4E21-8539-6106C44E76CF" );

            // Add Block 
            //  Block Name: Content
            //  Page Name: Prayer - Private Entry
            //  Layout: -
            //  Site: Kingdom First Solutions Mobile App
            RockMigrationHelper.AddBlock( true, "9A40B634-0E80-4207-BF0B-3050EFD49516".AsGuid(), null, "1DEF6B4D-F796-45E5-9535-BC1A5A1EA327".AsGuid(), "7258A210-E936-4260-B573-9FA1193AD9E2".AsGuid(), "Content", "Main", @"", @"", 0, "7CA630C6-4B1B-4D4E-9088-A4572EFE73DD" );

            // Add Block 
            //  Block Name: Prayer Session
            //  Page Name: Prayer Session
            //  Layout: -
            //  Site: Kingdom First Solutions Mobile App
            RockMigrationHelper.AddBlock( true, "68EAF2D8-688C-44C3-9222-340047E07095".AsGuid(), null, "1DEF6B4D-F796-45E5-9535-BC1A5A1EA327".AsGuid(), "420DEA5F-9ABC-4E59-A9BD-DCA972657B84".AsGuid(), "Prayer Session", "Main", @"", @"", 0, "73A98506-DBBA-408E-8053-F022C6C0B5B9" );

            // Add Block 
            //  Block Name: Communication List Subscribe
            //  Page Name: Communication Preferences
            //  Layout: -
            //  Site: Kingdom First Solutions Mobile App
            RockMigrationHelper.AddBlock( true, "D426B3D7-ADCD-4516-B539-FDD6426DD230".AsGuid(), null, "1DEF6B4D-F796-45E5-9535-BC1A5A1EA327".AsGuid(), "D0C51784-71ED-46F3-86AB-972148B78BE8".AsGuid(), "Communication List Subscribe", "Main", @"<Label Text=""Select your preferences"" StyleClass=""h1,p-16"" />", @"", 0, "875449F2-B0DF-449D-837D-169DB71D038C" );

            // Add Block 
            //  Block Name: Content Channel Item View
            //  Page Name: Content Item View
            //  Layout: -
            //  Site: Kingdom First Solutions Mobile App
            RockMigrationHelper.AddBlock( true, "FDF02B2C-F252-4F17-A947-C34957F1E2CC".AsGuid(), null, "1DEF6B4D-F796-45E5-9535-BC1A5A1EA327".AsGuid(), "B76B5F10-D2D6-4C60-B6FB-F913A62442E0".AsGuid(), "Content Channel Item View", "Main", @"", @"", 0, "CAD44F15-9400-47DD-B32D-1A105959D230" );

            // Add Block 
            //  Block Name: Group View
            //  Page Name: Group Detail
            //  Layout: -
            //  Site: Kingdom First Solutions Mobile App
            RockMigrationHelper.AddBlock( true, "EFB5029C-0BEA-4ADE-BFED-2E75B99A46D4".AsGuid(), null, "1DEF6B4D-F796-45E5-9535-BC1A5A1EA327".AsGuid(), "3F34AE03-9378-4363-A232-0318139C3BD3".AsGuid(), "Group View", "Main", @"", @"", 0, "376C8FED-8875-4628-92D8-8AA5F944BBF1" );

            // Add Block 
            //  Block Name: Group Member List
            //  Page Name: Group Detail
            //  Layout: -
            //  Site: Kingdom First Solutions Mobile App
            RockMigrationHelper.AddBlock( true, "EFB5029C-0BEA-4ADE-BFED-2E75B99A46D4".AsGuid(), null, "1DEF6B4D-F796-45E5-9535-BC1A5A1EA327".AsGuid(), "5A6D2ADB-03A7-4B55-8EAA-26A37116BFF1".AsGuid(), "Group Member List", "Main", @"", @"", 1, "75E364B8-1439-491A-9CA0-87E843C6AE7C" );

            // Add Block 
            //  Block Name: Content
            //  Page Name: C3 Home - Ministries
            //  Layout: -
            //  Site: Kingdom First Solutions Mobile App
            RockMigrationHelper.AddBlock( true, "A95A3FF3-9287-4B50-99C7-ED01D00BAB2B".AsGuid(), null, "1DEF6B4D-F796-45E5-9535-BC1A5A1EA327".AsGuid(), "7258A210-E936-4260-B573-9FA1193AD9E2".AsGuid(), "Content", "Main", @"", @"", 0, "C25B8D3F-2435-49FA-843B-C802678CE91D" );

            // Add Block 
            //  Block Name: Content
            //  Page Name: C3 Home - About
            //  Layout: -
            //  Site: Kingdom First Solutions Mobile App
            RockMigrationHelper.AddBlock( true, "17025FF4-2717-44F6-A4B7-B42A24600576".AsGuid(), null, "1DEF6B4D-F796-45E5-9535-BC1A5A1EA327".AsGuid(), "7258A210-E936-4260-B573-9FA1193AD9E2".AsGuid(), "Content", "Main", @"", @"", 0, "C9A845B7-C2CE-49D4-BA41-5DA0D16762F8" );

            // Add Block 
            //  Block Name: Content
            //  Page Name: C3 Home - Welcome
            //  Layout: -
            //  Site: Kingdom First Solutions Mobile App
            RockMigrationHelper.AddBlock( true, "649DDAB5-0872-4051-9E57-BE986884A0EC".AsGuid(), null, "1DEF6B4D-F796-45E5-9535-BC1A5A1EA327".AsGuid(), "7258A210-E936-4260-B573-9FA1193AD9E2".AsGuid(), "Content", "Main", @"", @"", 0, "8D4A6291-2B50-40E9-B446-D35A5D510EEF" );

            // Add Block 
            //  Block Name: Content
            //  Page Name: C3 Home - Ministries
            //  Layout: -
            //  Site: Kingdom First Solutions Mobile App
            RockMigrationHelper.AddBlock( true, "78F86495-2973-4173-B9B7-5A7D32D6DBBB".AsGuid(), null, "1DEF6B4D-F796-45E5-9535-BC1A5A1EA327".AsGuid(), "7258A210-E936-4260-B573-9FA1193AD9E2".AsGuid(), "Content", "Main", @"", @"", 0, "4769FD72-8993-497D-B6F1-6DC90BA47559" );

            // Add Block 
            //  Block Name: Content
            //  Page Name: C3 Home - Coming Up
            //  Layout: -
            //  Site: Kingdom First Solutions Mobile App
            RockMigrationHelper.AddBlock( true, "81DEF3F4-FE28-433B-9B16-C1462545EBA3".AsGuid(), null, "1DEF6B4D-F796-45E5-9535-BC1A5A1EA327".AsGuid(), "7258A210-E936-4260-B573-9FA1193AD9E2".AsGuid(), "Content", "Main", @"", @"", 0, "50629D11-CA39-4B1B-B0DD-307CC9CC17AF" );

            // Add Block 
            //  Block Name: Content
            //  Page Name: C3 Home - What's Next
            //  Layout: -
            //  Site: Kingdom First Solutions Mobile App
            RockMigrationHelper.AddBlock( true, "FC76C90A-D72B-4772-9DCE-DBC3CB11C545".AsGuid(), null, "1DEF6B4D-F796-45E5-9535-BC1A5A1EA327".AsGuid(), "7258A210-E936-4260-B573-9FA1193AD9E2".AsGuid(), "Content", "Main", @"", @"", 0, "6857C9E8-3EE6-42B0-8878-33DE5E44E41D" );

            // Add Block 
            //  Block Name: Content
            //  Page Name: My Groups
            //  Layout: -
            //  Site: Kingdom First Solutions Mobile App
            RockMigrationHelper.AddBlock( true, "6944F938-3379-4E0C-8FB4-188FCB383BB6".AsGuid(), null, "1DEF6B4D-F796-45E5-9535-BC1A5A1EA327".AsGuid(), "7258A210-E936-4260-B573-9FA1193AD9E2".AsGuid(), "Content", "Main", @"", @"", 0, "D8295512-7FF1-43D8-9DE1-6CAA59D2BFB7" );

            // Add Block 
            //  Block Name: Group Finder
            //  Page Name: Group Finder
            //  Layout: -
            //  Site: Kingdom First Solutions Mobile App
            RockMigrationHelper.AddBlock( true, "3696C320-3C61-476F-94A8-11E934891D67".AsGuid(), null, "1DEF6B4D-F796-45E5-9535-BC1A5A1EA327".AsGuid(), "BAC6671E-4D6F-4428-A6FA-69B8BEADF55C".AsGuid(), "Group Finder", "Main", @"", @"", 0, "5976C5DF-5D71-4FF5-9FB7-F3C6DF4AC40E" );

            // Add Block 
            //  Block Name: Group Edit
            //  Page Name: Group Edit
            //  Layout: -
            //  Site: Kingdom First Solutions Mobile App
            RockMigrationHelper.AddBlock( true, "367B9E49-0330-4890-B554-BA7F034ADC9B".AsGuid(), null, "1DEF6B4D-F796-45E5-9535-BC1A5A1EA327".AsGuid(), "FEC66374-E38F-4651-BAA6-AC658409D9BD".AsGuid(), "Group Edit", "Main", @"", @"", 0, "6353052C-7321-4BA2-97B8-3D19CF077FAC" );

            // Add Block 
            //  Block Name: Group Member View
            //  Page Name: Group Member View
            //  Layout: -
            //  Site: Kingdom First Solutions Mobile App
            RockMigrationHelper.AddBlock( true, "78671DCD-A1E0-461F-8F28-B0C10467BE57".AsGuid(), null, "1DEF6B4D-F796-45E5-9535-BC1A5A1EA327".AsGuid(), "6B3C23EA-A1C2-46FA-9F04-5B0BD004ED8B".AsGuid(), "Group Member View", "Main", @"", @"", 0, "B502E4F3-D936-41A8-81EC-25966A8FC081" );

            // Add Block 
            //  Block Name: Group View
            //  Page Name: Group Registration
            //  Layout: -
            //  Site: Kingdom First Solutions Mobile App
            RockMigrationHelper.AddBlock( true, "B8C9AF1D-7EEE-45C0-8619-93ABBF528F9B".AsGuid(), null, "1DEF6B4D-F796-45E5-9535-BC1A5A1EA327".AsGuid(), "3F34AE03-9378-4363-A232-0318139C3BD3".AsGuid(), "Group View", "Main", @"", @"", 0, "B96C7C9F-1345-4AFD-A5E4-015EA2428203" );

            // Add Block 
            //  Block Name: Group Registration
            //  Page Name: Group Registration
            //  Layout: -
            //  Site: Kingdom First Solutions Mobile App
            RockMigrationHelper.AddBlock( true, "B8C9AF1D-7EEE-45C0-8619-93ABBF528F9B".AsGuid(), null, "1DEF6B4D-F796-45E5-9535-BC1A5A1EA327".AsGuid(), "8A42E4FA-9FE1-493C-B6D8-7A766D96E912".AsGuid(), "Group Registration", "Main", @"", @"", 1, "F0C889DB-DCEA-476A-82E6-B56B3718D5AE" );

            // Add Block 
            //  Block Name: Lava Item List
            //  Page Name: Watch
            //  Layout: -
            //  Site: Kingdom First Solutions Mobile App
            RockMigrationHelper.AddBlock( true, "4D192169-934E-491D-A1B1-5D4B759BA94E".AsGuid(), null, "1DEF6B4D-F796-45E5-9535-BC1A5A1EA327".AsGuid(), "42B9ADBA-AE3E-4AC6-BE4C-7D3714ADF48D".AsGuid(), "Lava Item List", "Main", @"", @"", 1, "138B8249-EEE9-4879-B3AA-C0DC30204926" );

            // Add Block 
            //  Block Name: Content
            //  Page Name: Webview - Any URL
            //  Layout: -
            //  Site: Kingdom First Solutions Mobile App
            RockMigrationHelper.AddBlock( true, "743CF21F-C3C1-4AF1-8552-97A62A2B609B".AsGuid(), null, "1DEF6B4D-F796-45E5-9535-BC1A5A1EA327".AsGuid(), "7258A210-E936-4260-B573-9FA1193AD9E2".AsGuid(), "Content", "Main", @"", @"", 0, "74B8A175-1FE5-41A0-9881-2FD369713A49" );

            // update block order for pages with new blocks if the page,zone has multiple blocks"

            // Update Order for Page: Bible,  Zone: Main,  Block: Content
            Sql( @"UPDATE [Block] SET [Order] = 1 WHERE [Guid] = 'E183F13B-6745-4F5E-A529-F680E3BD2C91'" );

            // Update Order for Page: Bible,  Zone: Main,  Block: Hero
            Sql( @"UPDATE [Block] SET [Order] = 0 WHERE [Guid] = '5E44F0B2-307D-4659-AA00-2EFC9BF27DE9'" );

            // Update Order for Page: Kingdom First Solutions Mobile App Homepage,  Zone: Main,  Block: Content - Device Info
            Sql( @"UPDATE [Block] SET [Order] = 2 WHERE [Guid] = '7D8962A6-94D0-48CE-9B25-9495DFA3409F'" );

            // Update Order for Page: Kingdom First Solutions Mobile App Homepage,  Zone: Main,  Block: Content
            Sql( @"UPDATE [Block] SET [Order] = 0 WHERE [Guid] = 'B404A4C5-B9E8-4721-B14D-9AD560EC2E6B'" );

            // Update Order for Page: Kingdom First Solutions Mobile App Homepage,  Zone: Main,  Block: Static Card Content
            Sql( @"UPDATE [Block] SET [Order] = 1 WHERE [Guid] = '31AFE825-7065-4A67-8C7B-2CDABC44FBDD'" );

            // Update Order for Page: Login,  Zone: Main,  Block: Content
            Sql( @"UPDATE [Block] SET [Order] = 0 WHERE [Guid] = '3245F127-A3B7-41A8-B004-945C726A1B94'" );

            // Update Order for Page: Login,  Zone: Main,  Block: Login
            Sql( @"UPDATE [Block] SET [Order] = 1 WHERE [Guid] = '4B26BE7A-A7A3-44D8-BD2B-B87506383D58'" );

            // Update Order for Page: My Account/Giving History,  Zone: Main,  Block: Content - Contribution List
            Sql( @"UPDATE [Block] SET [Order] = 1 WHERE [Guid] = '8BCF8A77-51AD-4797-8CE8-43EBC4B84E41'" );

            // Update Order for Page: My Account/Giving History,  Zone: Main,  Block: Content
            Sql( @"UPDATE [Block] SET [Order] = 0 WHERE [Guid] = 'C8E2F42C-322F-4FD2-B886-571E9A202175'" );

            // Update Order for Page: My Account/Push Notifications,  Zone: Main,  Block: Content
            Sql( @"UPDATE [Block] SET [Order] = 0 WHERE [Guid] = '48A47402-C0B4-4820-8017-BB7EF63A60B5'" );

            // Update Order for Page: My Account/Push Notifications,  Zone: Main,  Block: ContentPush Notification - Recipients Shell 2.0
            Sql( @"UPDATE [Block] SET [Order] = 2 WHERE [Guid] = '3EF8CA12-457A-40B5-856B-D8CA240D2CDC'" );

            // Update Order for Page: My Account/Push Notifications,  Zone: Main,  Block: Push Notification Content - All Communications to Devices, Lists and existing notifications?
            Sql( @"UPDATE [Block] SET [Order] = 3 WHERE [Guid] = 'D9E72CCB-C386-403E-A44E-BC36EA56227E'" );

            // Update Order for Page: My Account/Push Notifications,  Zone: Main,  Block: Push Notification Content - Recipients
            Sql( @"UPDATE [Block] SET [Order] = 1 WHERE [Guid] = '72D2F125-C190-427D-B96E-03C1F59A9CB7'" );

            // Update Order for Page: Prayer - Prayer Wall,  Zone: Main,  Block: Content
            Sql( @"UPDATE [Block] SET [Order] = 1 WHERE [Guid] = '2A651146-7897-4E33-AB1E-70A53B38449B'" );

            // Update Order for Page: Prayer - Prayer Wall,  Zone: Main,  Block: Hero
            Sql( @"UPDATE [Block] SET [Order] = 0 WHERE [Guid] = 'FFA5E7C3-8086-4D35-B2CB-B0964C496F16'" );

            // Update Order for Page: Prayer - Prayer Wall,  Zone: Main,  Block: Prayer Card View
            Sql( @"UPDATE [Block] SET [Order] = 2 WHERE [Guid] = 'AABA341F-A6EF-45F9-B54D-362CDC892CA2'" );

            // Update Order for Page: Prayer - Private Entry,  Zone: Main,  Block: Content
            Sql( @"UPDATE [Block] SET [Order] = 0 WHERE [Guid] = '7CA630C6-4B1B-4D4E-9088-A4572EFE73DD'" );

            // Update Order for Page: Prayer - Private Entry,  Zone: Main,  Block: Prayer Request Details
            Sql( @"UPDATE [Block] SET [Order] = 1 WHERE [Guid] = 'F89758C2-F1B2-4E21-8539-6106C44E76CF'" );

            // Update Order for Page: Prayer,  Zone: Main,  Block: Content
            Sql( @"UPDATE [Block] SET [Order] = 1 WHERE [Guid] = 'C2B854DD-646A-4ED8-8C99-818F19521E70'" );

            // Update Order for Page: Prayer,  Zone: Main,  Block: Hero
            Sql( @"UPDATE [Block] SET [Order] = 0 WHERE [Guid] = 'A95B3556-75EC-4C56-89D4-0BE7A25B1F71'" );

            // Update Order for Page: Prayer,  Zone: Main,  Block: Prayer Session Setup
            Sql( @"UPDATE [Block] SET [Order] = 2 WHERE [Guid] = 'AE875F67-394C-4BED-8C2D-8937F713F5BA'" );

            // Add Block Attribute Value
            //   Block: Communication View
            //   BlockType: Communication View
            //   Category: Mobile > Communication
            //   Block Location: Page=Communication View, Site=Kingdom First Solutions Mobile App
            //   Attribute: Template
            /*   Attribute Value: ... */
            RockMigrationHelper.AddBlockAttributeValue( "43F9DF25-984E-43F9-BF99-5DE58D98D457", "20C706F6-D690-401B-83A6-9BD41661AAD2", @"ffffffff-ffff-ffff-ffff-ffffffffffff|<StackLayout StyleClass=""p-16"">{% assign pushData = Communication.PushData | FromJSON %}{% assign pushDataQS = pushData.MobilePageQueryString -%}
{%- capture mobilePageQS %}{% for qs in pushDataQS %}{% assign parts = qs | PropertyToKeyValue -%}{{- parts.Key }}={{ parts.Value }}&{%- endfor %}{% endcapture -%}
    <Label Text=""Message Sent: {{ Communication.SendDateTime | Date:'ddd, MMM d, yyyy, h:mm tt' }}"" StyleClass=""text-sm,font-weight-bold"" />
    <Label Text=""{{ Communication.PushTitle | Escape }}"" StyleClass=""h2"" />
{% if Content and Content != empty and Content != """" %}
    <Rock:Html>
        <![CDATA[{{ Content }}]]>
    </Rock:Html>
{% else %}
    <Label Text=""{{ Communication.PushMessage | RunLava | Escape }}"" StyleClass=""p"" />
{% endif %}
{% if pushData.MobilePageId and pushData.MobilePageId != empty %}{% page id:'{{ pushData.MobilePageId }}' %}<Button Text=""View Item"" StyleClass=""mt-16,btn,btn-primary"" Command=""{Binding PushPage}"" CommandParameter=""{{ page.Guid }}{% if mobilePageQS and mobilePageQS != """" %}?{{ mobilePageQS | ReplaceLast:""&"","""" | Escape }}{% endif %}"" />{% endpage %}{% endif %}
</StackLayout>" );

            // Add Block Attribute Value
            //   Block: Communication View
            //   BlockType: Communication View
            //   Category: Mobile > Communication
            //   Block Location: Page=Communication View, Site=Kingdom First Solutions Mobile App
            //   Attribute: Enabled Lava Commands
            /*   Attribute Value: RockEntity */
            RockMigrationHelper.AddBlockAttributeValue( "43F9DF25-984E-43F9-BF99-5DE58D98D457", "8F90D46F-3420-40FD-8E63-923295326C22", @"RockEntity" );

            // Add Block Attribute Value
            //   Block: Profile Details
            //   BlockType: Profile Details
            //   Category: Mobile > Cms
            //   Block Location: Page=Edit Profile, Site=Kingdom First Solutions Mobile App
            //   Attribute: Connection Status
            /*   Attribute Value: 368dd475-242c-49c4-a42c-7278be690cc2 */
            RockMigrationHelper.AddBlockAttributeValue( "1314F629-6204-48AB-8422-5B0EB55FEE23", "E4709745-E420-425D-82FB-E7EA9B8C89E2", @"368dd475-242c-49c4-a42c-7278be690cc2" );

            // Add Block Attribute Value
            //   Block: Profile Details
            //   BlockType: Profile Details
            //   Category: Mobile > Cms
            //   Block Location: Page=Edit Profile, Site=Kingdom First Solutions Mobile App
            //   Attribute: Record Status
            /*   Attribute Value: 283999ec-7346-42e3-b807-bce9b2babb49 */
            RockMigrationHelper.AddBlockAttributeValue( "1314F629-6204-48AB-8422-5B0EB55FEE23", "13AED2C1-BC58-4B5C-B711-CEA71A52ECC4", @"283999ec-7346-42e3-b807-bce9b2babb49" );

            // Add Block Attribute Value
            //   Block: Profile Details
            //   BlockType: Profile Details
            //   Category: Mobile > Cms
            //   Block Location: Page=Edit Profile, Site=Kingdom First Solutions Mobile App
            //   Attribute: Birthdate Show
            /*   Attribute Value: True */
            RockMigrationHelper.AddBlockAttributeValue( "1314F629-6204-48AB-8422-5B0EB55FEE23", "F8A627F5-D23B-4F09-BF68-C2E7D5279C4D", @"True" );

            // Add Block Attribute Value
            //   Block: Profile Details
            //   BlockType: Profile Details
            //   Category: Mobile > Cms
            //   Block Location: Page=Edit Profile, Site=Kingdom First Solutions Mobile App
            //   Attribute: BirthDate Required
            /*   Attribute Value: True */
            RockMigrationHelper.AddBlockAttributeValue( "1314F629-6204-48AB-8422-5B0EB55FEE23", "98A9C8D3-777D-4744-9346-811A9829CB47", @"True" );

            // Add Block Attribute Value
            //   Block: Profile Details
            //   BlockType: Profile Details
            //   Category: Mobile > Cms
            //   Block Location: Page=Edit Profile, Site=Kingdom First Solutions Mobile App
            //   Attribute: Campus Show
            /*   Attribute Value: True */
            RockMigrationHelper.AddBlockAttributeValue( "1314F629-6204-48AB-8422-5B0EB55FEE23", "E35DCBCE-A2F6-46BD-87D5-04FF6A59BAB8", @"True" );

            // Add Block Attribute Value
            //   Block: Profile Details
            //   BlockType: Profile Details
            //   Category: Mobile > Cms
            //   Block Location: Page=Edit Profile, Site=Kingdom First Solutions Mobile App
            //   Attribute: Campus Required
            /*   Attribute Value: True */
            RockMigrationHelper.AddBlockAttributeValue( "1314F629-6204-48AB-8422-5B0EB55FEE23", "22289290-D87F-418F-997C-5FDF986379A1", @"True" );

            // Add Block Attribute Value
            //   Block: Profile Details
            //   BlockType: Profile Details
            //   Category: Mobile > Cms
            //   Block Location: Page=Edit Profile, Site=Kingdom First Solutions Mobile App
            //   Attribute: Email Show
            /*   Attribute Value: True */
            RockMigrationHelper.AddBlockAttributeValue( "1314F629-6204-48AB-8422-5B0EB55FEE23", "FE8F714E-3077-4EB6-87A8-001AF221DA1E", @"True" );

            // Add Block Attribute Value
            //   Block: Profile Details
            //   BlockType: Profile Details
            //   Category: Mobile > Cms
            //   Block Location: Page=Edit Profile, Site=Kingdom First Solutions Mobile App
            //   Attribute: Email Required
            /*   Attribute Value: True */
            RockMigrationHelper.AddBlockAttributeValue( "1314F629-6204-48AB-8422-5B0EB55FEE23", "BE3AAA2A-5D08-46CF-B7DE-DB8F19502463", @"True" );

            // Add Block Attribute Value
            //   Block: Profile Details
            //   BlockType: Profile Details
            //   Category: Mobile > Cms
            //   Block Location: Page=Edit Profile, Site=Kingdom First Solutions Mobile App
            //   Attribute: Mobile Phone Show
            /*   Attribute Value: True */
            RockMigrationHelper.AddBlockAttributeValue( "1314F629-6204-48AB-8422-5B0EB55FEE23", "1FAA53DE-7F6E-481A-A8C0-D977271F0B6E", @"True" );

            // Add Block Attribute Value
            //   Block: Profile Details
            //   BlockType: Profile Details
            //   Category: Mobile > Cms
            //   Block Location: Page=Edit Profile, Site=Kingdom First Solutions Mobile App
            //   Attribute: Mobile Phone Required
            /*   Attribute Value: True */
            RockMigrationHelper.AddBlockAttributeValue( "1314F629-6204-48AB-8422-5B0EB55FEE23", "7DAC89DE-BF2E-47E7-9D83-A4056A681D9B", @"True" );

            // Add Block Attribute Value
            //   Block: Profile Details
            //   BlockType: Profile Details
            //   Category: Mobile > Cms
            //   Block Location: Page=Edit Profile, Site=Kingdom First Solutions Mobile App
            //   Attribute: Address Show
            /*   Attribute Value: True */
            RockMigrationHelper.AddBlockAttributeValue( "1314F629-6204-48AB-8422-5B0EB55FEE23", "CC6CC403-423B-48CA-9761-6114C34C7FDE", @"True" );

            // Add Block Attribute Value
            //   Block: Profile Details
            //   BlockType: Profile Details
            //   Category: Mobile > Cms
            //   Block Location: Page=Edit Profile, Site=Kingdom First Solutions Mobile App
            //   Attribute: Address Required
            /*   Attribute Value: True */
            RockMigrationHelper.AddBlockAttributeValue( "1314F629-6204-48AB-8422-5B0EB55FEE23", "1759420A-18DA-44B0-98B9-54F55CD644B6", @"True" );

            // Add Block Attribute Value
            //   Block: Profile Details
            //   BlockType: Profile Details
            //   Category: Mobile > Cms
            //   Block Location: Page=Edit Profile, Site=Kingdom First Solutions Mobile App
            //   Attribute: Gender
            /*   Attribute Value: 2 */
            RockMigrationHelper.AddBlockAttributeValue( "1314F629-6204-48AB-8422-5B0EB55FEE23", "D9AAF055-24B9-4BF5-A2A3-2405993D9010", @"2" );

            // Add Block Attribute Value
            //   Block: Content
            //   BlockType: Content
            //   Category: Mobile > Cms
            //   Block Location: Page=Kingdom First Solutions Mobile App Homepage, Site=Kingdom First Solutions Mobile App
            //   Attribute: Content
            /*   Attribute Value: ... */
            RockMigrationHelper.AddBlockAttributeValue( "B404A4C5-B9E8-4721-B14D-9AD560EC2E6B", "5682EDBF-68DA-4B43-A593-6C2B936C2839", @"<StackLayout>
{%- contentchannelitem where:'ContentChannelId == ""10"" && StartDateTime < ""{{ 'Now' | Date }}"" && ExpireDateTime > ""{{ 'Now' | Date }}"" || ExpireDateTime _= """" && ContentChannelId == ""10"" && StartDateTime < ""{{ 'Now' | Date }}""' sort:'Order' -%}
{%- for item in contentchannelitemItems -%}
{%- assign linkUrlType = item | Attribute:""LinktoURLType"" -%}
{%- assign linkToUrl = item | Attribute:""LinktoURL"" -%}{% assign authenticatedUser = item | Attribute:'PassUserAuthentication' | AsBoolean %}
{%- assign appPage = item | Attribute:""LinktoAppPage"",""RawValue"" -%}{% assign contentStripped = item.Content | StripHtml | Trim %} 
{%- capture command -%}
{%- if linkUrlType == 'External Browser' and appPage == '' -%}
{Binding OpenExternalBrowser}{%- elseif linkUrlType == 'Internal Browser' and appPage == '' -%}
{Binding OpenBrowser}{%- elseif linkToURL != '' or appPage != '' or contentStripped != '' -%}
{Binding PushPage}{%- endif -%}{%- endcapture -%}
{% if authenticatedUser %}{%- capture personToken %}{% if linkToUrl contains '?' %}&{% else %}?{% endif %}rckipid={{ CurrentPerson | PersonTokenCreate }}{% endcapture -%}{% endif %}
{%- capture linkUrl -%}
{%- if linkToUrl != '' and linkUrlType == 'Webview' -%}
743cf21f-c3c1-4af1-8552-97a62a2b609b?url={{ linkToUrl | Append:personToken | UrlEncode -}}
{%- elseif linkToUrl != '' -%}
{{- linkToUrl | Append:personToken | Escape -}}
{%- elseif appPage != '' -%}
{{- appPage -}}
{%- elseif contentStripped != '' %}
fdf02b2c-f252-4f17-a947-c34957f1e2cc?Item={{ item.Id -}}
{%- endif -%}
{%- endcapture -%}
{% assign image = item | Attribute:'Image','Url' %}{% assign imageUrl = item | Attribute:'ImageUrl' %}{% assign subtitle = item | Attribute:'Subtitle' %}{% assign cardWidth = item | Attribute:'CardWidth' %}{% assign elevation = item | Attribute:'ShadowDepth' %}{% assign showDetailsButton = item | Attribute:'DisplayDetailsButton' | AsBoolean %}{% assign showTitle = item | Attribute:'ShowTitle' | AsBoolean %}
<Rock:ContainedCard Image=""{% if imageUrl != '' %}{{ imageUrl | Escape }}{% elseif image != '' %}{{ image | Escape }}{% endif %}""
        Tagline=""{{ item | Attribute:'Tagline' | Escape }}""
        {% if showTitle %}Title=""{{ item.Title | Escape }}""{% endif %}
        ImageRatio=""{{ item | Attribute:'ImageAspectRatio' }}""
        Command=""{{ command }}""
        CommandParameter=""{{ linkUrl }}""
        Elevation=""{{ elevation }}""
        {% if cardWidth == 'Full' %}CornerRadius=""0""{% else %}StyleClass=""mx-16""{% endif %}>
        {%- if subtitle != """" -%}
            <Rock:ContainedCard.DescriptionLeft>
                <Label Text=""{{ item | Attribute:'Subtitle' | Escape }}"" StyleClass=""text-gray-500"" />
            </Rock:ContainedCard.DescriptionLeft>{%- endif -%}
       {%- if showDetailsButton %}
       <StackLayout>
           <Label Text=""{{ item.Content | StripHtml | TruncateWords:30 | Escape }}"" StyleClass=""my-16"" />
           <Button Text=""SEE DETAILS"" StyleClass=""btn,btn-primary"" HorizontalOptions=""Center"" Command=""{{ command }}"" CommandParameter=""{{ linkUrl }}"" />
        </StackLayout>{% else %}{{ item.Content | StripHtml | TruncateWords:50 | Escape }}{% endif %}
    </Rock:ContainedCard>
{% endfor %}
{% endcontentchannelitem %}
</StackLayout>" );

            // Add Block Attribute Value
            //   Block: Content
            //   BlockType: Content
            //   Category: Mobile > Cms
            //   Block Location: Page=Kingdom First Solutions Mobile App Homepage, Site=Kingdom First Solutions Mobile App
            //   Attribute: Enabled Lava Commands
            /*   Attribute Value: RockEntity */
            RockMigrationHelper.AddBlockAttributeValue( "B404A4C5-B9E8-4721-B14D-9AD560EC2E6B", "24516448-3F1F-4F27-97A1-CFB4F8B277B5", @"RockEntity" );

            // Add Block Attribute Value
            //   Block: Content
            //   BlockType: Content
            //   Category: Mobile > Cms
            //   Block Location: Page=Kingdom First Solutions Mobile App Homepage, Site=Kingdom First Solutions Mobile App
            //   Attribute: Dynamic Content
            /*   Attribute Value: True */
            RockMigrationHelper.AddBlockAttributeValue( "B404A4C5-B9E8-4721-B14D-9AD560EC2E6B", "B31D29A0-3725-4AEB-8360-7D91B9CDFE47", @"True" );

            // Add Block Attribute Value
            //   Block: Content - Device Info
            //   BlockType: Content
            //   Category: Mobile > Cms
            //   Block Location: Page=Kingdom First Solutions Mobile App Homepage, Site=Kingdom First Solutions Mobile App
            //   Attribute: Dynamic Content
            /*   Attribute Value: True */
            RockMigrationHelper.AddBlockAttributeValue( "7D8962A6-94D0-48CE-9B25-9495DFA3409F", "B31D29A0-3725-4AEB-8360-7D91B9CDFE47", @"True" );

            // Add Block Attribute Value
            //   Block: Content - Device Info
            //   BlockType: Content
            //   Category: Mobile > Cms
            //   Block Location: Page=Kingdom First Solutions Mobile App Homepage, Site=Kingdom First Solutions Mobile App
            //   Attribute: Enabled Lava Commands
            /*   Attribute Value: RockEntity */
            RockMigrationHelper.AddBlockAttributeValue( "7D8962A6-94D0-48CE-9B25-9495DFA3409F", "24516448-3F1F-4F27-97A1-CFB4F8B277B5", @"RockEntity" );

            // Add Block Attribute Value
            //   Block: Content - Device Info
            //   BlockType: Content
            //   Category: Mobile > Cms
            //   Block Location: Page=Kingdom First Solutions Mobile App Homepage, Site=Kingdom First Solutions Mobile App
            //   Attribute: Content
            /*   Attribute Value: ... */
            RockMigrationHelper.AddBlockAttributeValue( "7D8962A6-94D0-48CE-9B25-9495DFA3409F", "5682EDBF-68DA-4B43-A593-6C2B936C2839", @"<StackLayout StyleClass=""p-16""
    Spacing=""16"">
    
        <Rock:ContainedCard
        Title=""Device Info"">
            {% personaldevice where:'PersonAliasId == {{ CurrentPerson.PrimaryAliasId }}' %}{% for device in personaldeviceItems %}{% assign deviceid = device.Id %}{% endfor %}{% endpersonaldevice %}{{ deviceid }}
{% capture communicationIds %}{% communicationrecipient where:'PersonalDeviceId == {{ deviceid }}' %}{% for recp in communicationrecipientItems %}{{ recp.CommunicationId }},{% endfor %}{% endcommunicationrecipient %}{% endcapture %}{{ communicationIds | ReplaceLast:"","","""" }}
{% communication ids:'{{ communicationIds | ReplaceLast:"","","""" }}' where:'PushMessage _!""""' %}{% for comm in communicationItems %}{{ comm.PushMessage }},{% endfor %}{% endcommunication %}
    </Rock:ContainedCard>
    <Rock:StyledView BackgroundColor=""#f5f5f5""
    CornerRadius=""6""
    HorizontalOptions=""FillAndExpand""
    HeightRequest=""50""
    HasShadow=""true""
    Elevation=""1"">
        <StackLayout Orientation=""Horizontal"">
            <Label Text=""Test Text"" HorizontalOptions=""StartAndExpand"" VerticalOptions=""Center"" StyleClass=""p-16"" />
            <Rock:Icon IconClass=""angle-right"" FontSize=""24"" HorizontalOptions=""End"" VerticalOptions=""Center"" StyleClass=""p-16"" />
        </StackLayout>
    </Rock:StyledView>
</StackLayout>" );

            // Add Block Attribute Value
            //   Block: Static Card Content
            //   BlockType: Content
            //   Category: Mobile > Cms
            //   Block Location: Page=Kingdom First Solutions Mobile App Homepage, Site=Kingdom First Solutions Mobile App
            //   Attribute: Content
            /*   Attribute Value: ... */
            RockMigrationHelper.AddBlockAttributeValue( "31AFE825-7065-4A67-8C7B-2CDABC44FBDD", "5682EDBF-68DA-4B43-A593-6C2B936C2839", @"<StackLayout StyleClass=""p-16""
    Spacing=""16"">
 
    <Label Text=""News Feed""
        StyleClass=""h4"" />
    
    <Rock:ContainedCard Image=""https://images.squarespace-cdn.com/content/v1/5c6f1d46bfba3e388c3ec659/1559925855350-8QMRWEG4P065MT9ZV3X3/IMG_4869+2.JPG?format=2500w""
        Tagline=""Watch/Listen LIVE on Sunday!""
        Title=""WATCH LIVE""
        ImageRatio="".75:1""
        Command=""{Binding OpenExternalBrowser}""
        CommandParameter=""https://cartervillechristian.churchonline.org/"">
       Sunday @ 9:00 or 10:45 am
    </Rock:ContainedCard>
    
    <Rock:ContainedCard Image=""https://images.squarespace-cdn.com/content/v1/5c6f1d46bfba3e388c3ec659/1648060685390-WYBP3FVPT441XMRVAHU2/CelebrateEaster_SlideInfo.jpg?format=1500w""
        Title=""Easter Week at C3""
        ImageRatio="".66:1""
        Command=""{Binding OpenBrowser}""
        CommandParameter=""https://www.cartervillechristian.com/comingup/easter-weekend"">
    </Rock:ContainedCard>
    
    <Rock:ContainedCard Image=""https://images.squarespace-cdn.com/content/v1/5c6f1d46bfba3e388c3ec659/1647375310830-JNK1N4WG2YPQCHYRSJGV/God+is+in+Control+-+Esther_+image.png?format=1500w""
        Title=""God is in Control: A Women's Study in Esther""
        ImageRatio="".66:1""
        Command=""{Binding PushPage}""
        CommandParameter=""25a63940-8196-4ea3-b943-d3caac43072d?url={{ 'https://www.cartervillechristian.com/comingup/esther' | UrlEncode }}"">
    </Rock:ContainedCard>
{% if CurrentPerson %} 
<Rock:ContainedCard
        Title=""My Groups""
        ImageRatio="".5:1""
        Command=""{Binding PushPage}""
        CommandParameter=""6944f938-3379-4e0c-8fb4-188fcb383bb6"">
    
    </Rock:ContainedCard>
{% endif %}
    <Rock:ContainedCard Image=""https://rock.cartervillechristian.com/Content/C3App/AppImages/C3Home/NewAppNavigation-09.jpg""
        Title=""Prayer""
        ImageRatio="".5:1""
        Command=""{Binding PushPage}""
        CommandParameter=""6d328172-d7ee-434b-bd79-a4d7eca14684"">
    </Rock:ContainedCard>
    
        <Rock:ContainedCard Image=""https://images.squarespace-cdn.com/content/v1/5c6f1d46bfba3e388c3ec659/1647375310830-JNK1N4WG2YPQCHYRSJGV/God+is+in+Control+-+Esther_+image.png?format=1500w""
        Title=""Device""
        ImageRatio="".66:1"">
            {{ Device.Name }}
    </Rock:ContainedCard>
</StackLayout>" );

            // Add Block Attribute Value
            //   Block: Static Card Content
            //   BlockType: Content
            //   Category: Mobile > Cms
            //   Block Location: Page=Kingdom First Solutions Mobile App Homepage, Site=Kingdom First Solutions Mobile App
            //   Attribute: Dynamic Content
            /*   Attribute Value: True */
            RockMigrationHelper.AddBlockAttributeValue( "31AFE825-7065-4A67-8C7B-2CDABC44FBDD", "B31D29A0-3725-4AEB-8360-7D91B9CDFE47", @"True" );

            // Add Block Attribute Value
            //   Block: Content
            //   BlockType: Content
            //   Category: Mobile > Cms
            //   Block Location: Page=Bible, Site=Kingdom First Solutions Mobile App
            //   Attribute: Dynamic Content
            /*   Attribute Value: False */
            RockMigrationHelper.AddBlockAttributeValue( "E183F13B-6745-4F5E-A529-F680E3BD2C91", "B31D29A0-3725-4AEB-8360-7D91B9CDFE47", @"False" );

            // Add Block Attribute Value
            //   Block: Content
            //   BlockType: Content
            //   Category: Mobile > Cms
            //   Block Location: Page=Bible, Site=Kingdom First Solutions Mobile App
            //   Attribute: Content
            /*   Attribute Value: ... */
            RockMigrationHelper.AddBlockAttributeValue( "E183F13B-6745-4F5E-A529-F680E3BD2C91", "5682EDBF-68DA-4B43-A593-6C2B936C2839", @"<StackLayout>
    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition Height=""75"" />
        </Grid.RowDefinitions>
        <Grid.ColumnDefinitions>
            <ColumnDefinition />
            <ColumnDefinition />
        </Grid.ColumnDefinitions>
        <StackLayout VerticalOptions=""CenterAndExpand"">
            <Rock:Icon IconClass=""mdi-logout"" IconFamily=""MaterialDesignIcons"" FontSize=""24"" HorizontalOptions=""Center"" />
            <Label Text=""OPEN BIBLE APP"" HorizontalOptions=""Center"" StyleClass=""text-xs"" />
        </StackLayout>
        <Button Command=""{Binding OpenExternalBrowser}"" CommandParameter=""youversion://"" StyleClass=""btn,btn-link"" />
        <StackLayout VerticalOptions=""CenterAndExpand"" Grid.Column=""1"">
            <Rock:Icon IconClass=""mdi-cloud-outline"" IconFamily=""MaterialDesignIcons"" FontSize=""24"" HorizontalOptions=""Center"" />
            <Label Text=""OPEN BIBLE WEB"" HorizontalOptions=""Center"" StyleClass=""text-xs""  />
        </StackLayout>
        <Button Grid.Column=""1"" Command=""{Binding OpenExternalBrowser}"" CommandParameter=""https://www.bible.com/bible/"" StyleClass=""btn,btn-link"" />
    </Grid>
    <BoxView HeightRequest=""1"" Color=""{Rock:PaletteColor gray-200}"" />
</StackLayout>" );

            // Add Block Attribute Value
            //   Block: Hero
            //   BlockType: Hero
            //   Category: Mobile > Cms
            //   Block Location: Page=Bible, Site=Kingdom First Solutions Mobile App
            //   Attribute: Subtitle Color
            /*   Attribute Value: #ffffff */
            RockMigrationHelper.AddBlockAttributeValue( "5E44F0B2-307D-4659-AA00-2EFC9BF27DE9", "5374CF14-6A4F-4CA8-9561-0A1003F28504", @"#ffffff" );

            // Add Block Attribute Value
            //   Block: Hero
            //   BlockType: Hero
            //   Category: Mobile > Cms
            //   Block Location: Page=Bible, Site=Kingdom First Solutions Mobile App
            //   Attribute: Title
            /*   Attribute Value: BIBLE */
            RockMigrationHelper.AddBlockAttributeValue( "5E44F0B2-307D-4659-AA00-2EFC9BF27DE9", "D9132EAB-91FC-4061-8881-1D2618E84F52", @"BIBLE" );

            // Add Block Attribute Value
            //   Block: Hero
            //   BlockType: Hero
            //   Category: Mobile > Cms
            //   Block Location: Page=Bible, Site=Kingdom First Solutions Mobile App
            //   Attribute: Title Color
            /*   Attribute Value: #ffffff */
            RockMigrationHelper.AddBlockAttributeValue( "5E44F0B2-307D-4659-AA00-2EFC9BF27DE9", "9C95201E-64AC-4A36-8957-571C7BCA8F5B", @"#ffffff" );

            // Add Block Attribute Value
            //   Block: Hero
            //   BlockType: Hero
            //   Category: Mobile > Cms
            //   Block Location: Page=Bible, Site=Kingdom First Solutions Mobile App
            //   Attribute: Text Align
            /*   Attribute Value: Left */
            RockMigrationHelper.AddBlockAttributeValue( "5E44F0B2-307D-4659-AA00-2EFC9BF27DE9", "F3F7B68A-6926-4D89-BA28-0FB99E940A8E", @"Left" );

            // Add Block Attribute Value
            //   Block: Hero
            //   BlockType: Hero
            //   Category: Mobile > Cms
            //   Block Location: Page=Bible, Site=Kingdom First Solutions Mobile App
            //   Attribute: Height - Phone
            /*   Attribute Value: 220 */
            RockMigrationHelper.AddBlockAttributeValue( "5E44F0B2-307D-4659-AA00-2EFC9BF27DE9", "6527442E-539A-4486-B0C8-6D8C48F77FE8", @"220" );

            // Add Block Attribute Value
            //   Block: Hero
            //   BlockType: Hero
            //   Category: Mobile > Cms
            //   Block Location: Page=Bible, Site=Kingdom First Solutions Mobile App
            //   Attribute: Height - Tablet
            /*   Attribute Value: 600 */
            RockMigrationHelper.AddBlockAttributeValue( "5E44F0B2-307D-4659-AA00-2EFC9BF27DE9", "BF0B4848-9193-45EA-9A13-E0DE5573A64F", @"600" );

            // Add Block Attribute Value
            //   Block: Hero
            //   BlockType: Hero
            //   Category: Mobile > Cms
            //   Block Location: Page=Bible, Site=Kingdom First Solutions Mobile App
            //   Attribute: Padding
            /*   Attribute Value: 20 */
            RockMigrationHelper.AddBlockAttributeValue( "5E44F0B2-307D-4659-AA00-2EFC9BF27DE9", "92DBA471-CFBC-4BC2-BD17-022663670328", @"20" );

            // Add Block Attribute Value
            //   Block: Hero
            //   BlockType: Hero
            //   Category: Mobile > Cms
            //   Block Location: Page=Bible, Site=Kingdom First Solutions Mobile App
            //   Attribute: Background Image - Phone
            /*   Attribute Value: 025bc00b-b21b-43b7-b6e7-e2d33a31df55 */
            RockMigrationHelper.AddBlockAttributeValue( "5E44F0B2-307D-4659-AA00-2EFC9BF27DE9", "D807B220-49A0-49EE-B4F0-F0A92ED3160F", @"025bc00b-b21b-43b7-b6e7-e2d33a31df55" );

            // Add Block Attribute Value
            //   Block: Hero
            //   BlockType: Hero
            //   Category: Mobile > Cms
            //   Block Location: Page=Bible, Site=Kingdom First Solutions Mobile App
            //   Attribute: Background Image - Tablet
            /*   Attribute Value: 3fae643c-5ad6-4cca-bc6a-5c9bcebc954e */
            RockMigrationHelper.AddBlockAttributeValue( "5E44F0B2-307D-4659-AA00-2EFC9BF27DE9", "B7A56FAF-90C6-4F79-898C-6F1FAEDF19B5", @"3fae643c-5ad6-4cca-bc6a-5c9bcebc954e" );

            // Add Block Attribute Value
            //   Block: Content
            //   BlockType: Content
            //   Category: Mobile > Cms
            //   Block Location: Page=Give, Site=Kingdom First Solutions Mobile App
            //   Attribute: Dynamic Content
            /*   Attribute Value: False */
            RockMigrationHelper.AddBlockAttributeValue( "E9ECA833-8D2D-4A2D-AA00-B57178435285", "B31D29A0-3725-4AEB-8360-7D91B9CDFE47", @"False" );

            // Add Block Attribute Value
            //   Block: Content
            //   BlockType: Content
            //   Category: Mobile > Cms
            //   Block Location: Page=Give, Site=Kingdom First Solutions Mobile App
            //   Attribute: Content
            /*   Attribute Value: ... */
            RockMigrationHelper.AddBlockAttributeValue( "E9ECA833-8D2D-4A2D-AA00-B57178435285", "5682EDBF-68DA-4B43-A593-6C2B936C2839", @"<StackLayout StyleClass=""p-16"">
    <Label Text=""C3 GIVING"" StyleClass=""h1,mb-16"" />
    <StackLayout Orientation=""Horizontal"" HeightRequest=""50"">
        <StackLayout.GestureRecognizers>
            <TapGestureRecognizer Command=""{Binding OpenExternalBrowser}"" CommandParameter=""https://c3.mywell.org/"" />
        </StackLayout.GestureRecognizers>
        <Rock:Icon IconClass=""mdi-heart-circle-outline"" IconFamily=""MaterialDesignIcons"" FontSize=""24"" VerticalOptions=""Center"" HorizontalTextAlignment=""Center"" WidthRequest=""60"" />
        <Label Text=""Give Online"" StyleClass=""font-weight-bold"" VerticalOptions=""Center"" />
    </StackLayout>
    <BoxView HeightRequest=""1"" Color=""{Rock:PaletteColor gray-200}"" />
    <StackLayout Orientation=""Horizontal"" HeightRequest=""50"">
        <StackLayout.GestureRecognizers>
            <TapGestureRecognizer Command=""{Binding PushPage}"" CommandParameter=""743cf21f-c3c1-4af1-8552-97a62a2b609b?url={{ 'https://www.emailmeform.com/builder/form/4svX9JknEabouO1UdclaQfT' | UrlEncode }}"" />
        </StackLayout.GestureRecognizers>
        <Rock:Icon IconClass=""mdi-file-edit-outline"" IconFamily=""MaterialDesignIcons"" FontSize=""24"" VerticalOptions=""Center"" HorizontalTextAlignment=""Center"" WidthRequest=""60"" />
        <Label Text=""ACH Recurring Form"" StyleClass=""font-weight-bold"" VerticalOptions=""Center"" />
    </StackLayout>
    <BoxView HeightRequest=""1"" Color=""{Rock:PaletteColor gray-200}"" />
    <StackLayout Orientation=""Horizontal"" HeightRequest=""60"">
        <StackLayout.GestureRecognizers>
            <TapGestureRecognizer Command=""{Binding SendEmail}"" CommandParameter=""office@cartervillechristian.com"" />
        </StackLayout.GestureRecognizers>
        <Rock:Icon IconClass=""mdi-help-circle-outline"" IconFamily=""MaterialDesignIcons"" FontSize=""24"" VerticalOptions=""Center"" HorizontalTextAlignment=""Center"" WidthRequest=""60"" />
        <StackLayout VerticalOptions=""Center"">
            <Label Text=""Questions"" />
            <Label StyleClass=""font-weight-bold"" Text=""office@cartervillechristrian.com"" />
        </StackLayout>
    </StackLayout>
    <BoxView HeightRequest=""1"" Color=""{Rock:PaletteColor gray-200}"" />
</StackLayout>" );

            // Add Block Attribute Value
            //   Block: Content
            //   BlockType: Content
            //   Category: Mobile > Cms
            //   Block Location: Page=C3 Home, Site=Kingdom First Solutions Mobile App
            //   Attribute: Content
            /*   Attribute Value: ... */
            RockMigrationHelper.AddBlockAttributeValue( "CFAD5C83-1A6A-4EA9-B664-19007724C3FF", "5682EDBF-68DA-4B43-A593-6C2B936C2839", @"<StackLayout Spacing=""0"">
    <Rock:Image Source=""https://rock13.kingdomfirstsolutions.com/Content/C3App/NewAppNavigation-01.jpg"" Command=""{Binding PushPage}"" CommandParameter=""649ddab5-0872-4051-9e57-be986884a0ec"" />
    <Rock:Image Source=""https://rock13.kingdomfirstsolutions.com/Content/C3App/NewAppNavigation-02.jpg"" Command=""{Binding PushPage}"" CommandParameter=""649ddab5-0872-4051-9e57-be986884a0ec"" />
    <Rock:Image Source=""https://rock13.kingdomfirstsolutions.com/Content/C3App/NewAppNavigation-03.jpg"" Command=""{Binding PushPage}"" CommandParameter=""fc76c90a-d72b-4772-9dce-dbc3cb11c545"" />
    <Rock:Image Source=""https://rock13.kingdomfirstsolutions.com/Content/C3App/NewAppNavigation-04.jpg"" Command=""{Binding PushPage}"" CommandParameter=""81def3f4-fe28-433b-9b16-c1462545eba3"" />
    <Rock:Image Source=""https://rock13.kingdomfirstsolutions.com/Content/C3App/NewAppNavigation-05.jpg"" Command=""{Binding PushPage}"" CommandParameter=""d71681c4-3662-4e3c-8a8b-9a72844dea43"" />
    <Rock:Image Source=""https://rock13.kingdomfirstsolutions.com/Content/C3App/NewAppNavigation-06.jpg"" Command=""{Binding PushPage}"" CommandParameter=""78f86495-2973-4173-b9b7-5a7d32d6dbbb"" />
    <Rock:Image Source=""https://rock13.kingdomfirstsolutions.com/Content/C3App/NewAppNavigation-07.jpg"" Command=""{Binding PushPage}"" CommandParameter=""743cf21f-c3c1-4af1-8552-97a62a2b609b?url={{ 'https://www.cartervillechristian.com/easter-bible' | UrlEncode }}"" />
    <Rock:Image Source=""https://rock13.kingdomfirstsolutions.com/Content/C3App/NewAppNavigation-08.jpg"" Command=""{Binding PushPage}"" CommandParameter=""743cf21f-c3c1-4af1-8552-97a62a2b609b?url={{ 'https://www.cartervillechristian.com/staff' | UrlEncode }}"" />
    <Rock:Image Source=""https://rock13.kingdomfirstsolutions.com/Content/C3App/NewAppNavigation-09.jpg"" Command=""{Binding PushPage}"" CommandParameter=""6d328172-d7ee-434b-bd79-a4d7eca14684"" />
    <Rock:Image Source=""https://rock13.kingdomfirstsolutions.com/Content/C3App/NewAppNavigation-10.jpg"" Command=""{Binding PushPage}"" CommandParameter=""17025ff4-2717-44f6-a4b7-b42a24600576"" />
    <Rock:Image Source=""https://rock13.kingdomfirstsolutions.com/Content/C3App/NewAppNavigation-11.jpg"" Command=""{Binding PushPage}"" CommandParameter=""17025ff4-2717-44f6-a4b7-b42a24600576"" />
</StackLayout>" );

            // Add Block Attribute Value
            //   Block: Content
            //   BlockType: Content
            //   Category: Mobile > Cms
            //   Block Location: Page=C3 Home, Site=Kingdom First Solutions Mobile App
            //   Attribute: Dynamic Content
            /*   Attribute Value: False */
            RockMigrationHelper.AddBlockAttributeValue( "CFAD5C83-1A6A-4EA9-B664-19007724C3FF", "B31D29A0-3725-4AEB-8360-7D91B9CDFE47", @"False" );

            // Add Block Attribute Value
            //   Block: Content
            //   BlockType: Content
            //   Category: Mobile > Cms
            //   Block Location: Page=Sunday, Site=Kingdom First Solutions Mobile App
            //   Attribute: Dynamic Content
            /*   Attribute Value: False */
            RockMigrationHelper.AddBlockAttributeValue( "7D1B6CEA-34E1-4214-B772-9B8FD5CA2B5A", "B31D29A0-3725-4AEB-8360-7D91B9CDFE47", @"False" );

            // Add Block Attribute Value
            //   Block: Content
            //   BlockType: Content
            //   Category: Mobile > Cms
            //   Block Location: Page=Sunday, Site=Kingdom First Solutions Mobile App
            //   Attribute: Content
            /*   Attribute Value: ... */
            RockMigrationHelper.AddBlockAttributeValue( "7D1B6CEA-34E1-4214-B772-9B8FD5CA2B5A", "5682EDBF-68DA-4B43-A593-6C2B936C2839", @"<StackLayout StyleClass=""px-8,py-16"">
    <Rock:StyledView StyleClass=""bg-white""
        CornerRadius=""4""
        HorizontalOptions=""FillAndExpand""
        HasShadow=""true""
        Elevation=""2""
        HeightRequest=""75"">
        <StackLayout Orientation=""Horizontal"" StyleClass=""py-8,px-16"">
            <StackLayout.GestureRecognizers>
                <TapGestureRecognizer Command=""{Binding OpenExternalBrowser}"" CommandParameter=""https://cartervillechristian.online.church/"" />
            </StackLayout.GestureRecognizers>
            <Label Text=""WATCH LIVE"" StyleClass=""font-weight-bold"" LineBreakMode=""TailTruncation"" VerticalOptions=""Center"" HorizontalOptions=""StartAndExpand"" />
            <Rock:Icon IconClass=""angle-right"" FontSize=""24"" HorizontalOptions=""End"" VerticalOptions=""Center"" />
        </StackLayout>
    </Rock:StyledView>
    <Rock:StyledView StyleClass=""bg-white""
        CornerRadius=""4""
        HorizontalOptions=""FillAndExpand""
        HasShadow=""true""
        Elevation=""2""
        HeightRequest=""75"">
        <StackLayout Orientation=""Horizontal"" StyleClass=""py-8,px-16"">
            <StackLayout.GestureRecognizers>
                <TapGestureRecognizer Command=""{Binding PushPage}"" CommandParameter=""743cf21f-c3c1-4af1-8552-97a62a2b609b?url={{ 'https://www.cartervillechristian.com/c3notes' | UrlEncode }}"" />
            </StackLayout.GestureRecognizers>
            <Label Text=""C3 MESSAGE NOTES"" StyleClass=""font-weight-bold"" LineBreakMode=""TailTruncation"" VerticalOptions=""Center"" HorizontalOptions=""StartAndExpand"" />
            <Rock:Icon IconClass=""angle-right"" FontSize=""24"" HorizontalOptions=""End"" VerticalOptions=""Center"" />
        </StackLayout>
    </Rock:StyledView>
    <Rock:StyledView StyleClass=""bg-white""
        CornerRadius=""4""
        HorizontalOptions=""FillAndExpand""
        HasShadow=""true""
        Elevation=""2""
        HeightRequest=""75"">
        <StackLayout Orientation=""Horizontal"" StyleClass=""py-8,px-16"">
            <StackLayout.GestureRecognizers>
                <TapGestureRecognizer Command=""{Binding PushPage}"" CommandParameter=""743cf21f-c3c1-4af1-8552-97a62a2b609b?url={{ 'https://www.cartervillechristian.com/worshipsetlist' | UrlEncode }}"" />
            </StackLayout.GestureRecognizers>
            <Label Text=""WORSHIP SET LIST"" StyleClass=""font-weight-bold"" LineBreakMode=""TailTruncation"" VerticalOptions=""Center"" HorizontalOptions=""StartAndExpand"" />
            <Rock:Icon IconClass=""angle-right"" FontSize=""24"" HorizontalOptions=""End"" VerticalOptions=""Center"" />
        </StackLayout>
    </Rock:StyledView>
    <Rock:StyledView StyleClass=""bg-white""
        CornerRadius=""4""
        HorizontalOptions=""FillAndExpand""
        HasShadow=""true""
        Elevation=""2""
        HeightRequest=""75"">
        <StackLayout Orientation=""Horizontal"" StyleClass=""py-8,px-16"">
            <StackLayout.GestureRecognizers>
                <TapGestureRecognizer Command=""{Binding OpenExternalBrowser}"" CommandParameter=""https://www.youtube.com/user/CartervilleChristian"" />
            </StackLayout.GestureRecognizers>
            <Label Text=""C3 MESSAGE ARCHIVE"" StyleClass=""font-weight-bold"" LineBreakMode=""TailTruncation"" VerticalOptions=""Center"" HorizontalOptions=""StartAndExpand"" />
            <Rock:Icon IconClass=""angle-right"" FontSize=""24"" HorizontalOptions=""End"" VerticalOptions=""Center"" />
        </StackLayout>
    </Rock:StyledView>
    <Rock:StyledView StyleClass=""bg-white""
        CornerRadius=""4""
        HorizontalOptions=""FillAndExpand""
        HasShadow=""true""
        Elevation=""2""
        HeightRequest=""75"">
        <StackLayout Orientation=""Horizontal"" StyleClass=""py-8,px-16"">
            <StackLayout.GestureRecognizers>
                <TapGestureRecognizer Command=""{Binding PushPage}"" CommandParameter=""4d192169-934e-491d-a1b1-5d4b759ba94e"" />
            </StackLayout.GestureRecognizers>
            <Label Text=""LOCAL MESSAGE ARCHIVE"" StyleClass=""font-weight-bold"" LineBreakMode=""TailTruncation"" VerticalOptions=""Center"" HorizontalOptions=""StartAndExpand"" />
            <Rock:Icon IconClass=""angle-right"" FontSize=""24"" HorizontalOptions=""End"" VerticalOptions=""Center"" />
        </StackLayout>
    </Rock:StyledView>
</StackLayout>

    " );

            // Add Block Attribute Value
            //   Block: Content
            //   BlockType: Content
            //   Category: Mobile > Cms
            //   Block Location: Page=Webview - Any URL, Site=Kingdom First Solutions Mobile App
            //   Attribute: Content
            /*   Attribute Value: <Rock:WebView Source="{{ PageParameter.url | UrlDecode }}" /> */
            RockMigrationHelper.AddBlockAttributeValue( "74B8A175-1FE5-41A0-9881-2FD369713A49", "5682EDBF-68DA-4B43-A593-6C2B936C2839", @"<Rock:WebView Source=""{{ PageParameter.url | UrlDecode }}"" />" );

            // Add Block Attribute Value
            //   Block: Content
            //   BlockType: Content
            //   Category: Mobile > Cms
            //   Block Location: Page=Webview - Any URL, Site=Kingdom First Solutions Mobile App
            //   Attribute: Dynamic Content
            /*   Attribute Value: True */
            RockMigrationHelper.AddBlockAttributeValue( "74B8A175-1FE5-41A0-9881-2FD369713A49", "B31D29A0-3725-4AEB-8360-7D91B9CDFE47", @"True" );

            // Add Block Attribute Value
            //   Block: Group View
            //   BlockType: Group View
            //   Category: Mobile > Groups
            //   Block Location: Page=Group Detail, Site=Kingdom First Solutions Mobile App
            //   Attribute: Group Edit Page
            /*   Attribute Value: 367b9e49-0330-4890-b554-ba7f034adc9b */
            RockMigrationHelper.AddBlockAttributeValue( "376C8FED-8875-4628-92D8-8AA5F944BBF1", "386360FF-9E79-4E0C-92D5-7285484FFADF", @"367b9e49-0330-4890-b554-ba7f034adc9b" );

            // Add Block Attribute Value
            //   Block: Group View
            //   BlockType: Group View
            //   Category: Mobile > Groups
            //   Block Location: Page=Group Detail, Site=Kingdom First Solutions Mobile App
            //   Attribute: Template
            /*   Attribute Value: ... */
            RockMigrationHelper.AddBlockAttributeValue( "376C8FED-8875-4628-92D8-8AA5F944BBF1", "A3826811-395A-4564-8101-EB95936065FB", @"ffffffff-ffff-ffff-ffff-ffffffffffff|<StackLayout Spacing=""0"" StyleClass=""p-16"">{% assign groupIdStr = Group.Id | AsString %}
{%- assign groupMembers = CurrentPerson | Group:groupIdStr -%}
{%- assign isGroupLeader = false -%}
{%- for groupMember in groupMembers -%}
    {%- if groupMember.GroupRole.IsLeader -%}
        {%- assign isGroupLeader = true -%}
    {%- endif -%}
{%- endfor -%}
    <StackLayout Orientation=""Horizontal"" Spacing=""20"">
        <StackLayout Orientation=""Vertical"" Spacing=""0"" HorizontalOptions=""FillAndExpand"">
            <Label StyleClass=""h1"" Text=""{{ Group.Name | Escape }}"" />
        </StackLayout>
        {% if GroupEditPage != '' and AllowedActions.Edit == true and isGroupLeader == true %}
        <Rock:Icon IconClass=""Ellipsis-v"" FontSize=""24"" TextColor=""#ccc"" Command=""{Binding ShowActionPanel}"">
            <Rock:Icon.CommandParameter>
                <Rock:ShowActionPanelParameters Title=""Group Actions"" CancelTitle=""Cancel"">
                    <Rock:ActionPanelButton Title=""Edit Group"" Command=""{Binding PushPage}"" CommandParameter=""{{ GroupEditPage }}?GroupGuid={{ Group.Guid }}"" />
                </Rock:ShowActionPanelParameters>
            </Rock:Icon.CommandParameter>
        </Rock:Icon>
        {% endif %}
    </StackLayout>

    <BoxView Color=""#ccc"" HeightRequest=""1"" Margin=""0, 30, 0, 10"" />

    <!-- Handle Group Attributes -->
    {% if VisibleAttributes != empty %}
        <Rock:ResponsiveLayout>
        {% for attribute in VisibleAttributes %}
            <Rock:ResponsiveColumn ExtraSmall=""6"">
                <Rock:FieldContainer>
                    <Rock:Literal Label=""{{ attribute.Name | Escape }}"" Text=""{{ attribute.FormattedValue }}"" />
                </Rock:FieldContainer>
            </Rock:ResponsiveColumn>
        {% endfor %}
        </Rock:ResponsiveLayout>
    {% endif %}

    <!-- Handle displaying of leaders -->
    {% if ShowLeaderList == true %}
        <Label Text=""Leaders"" StyleClass=""field-title,font-weight-bold"" Margin=""0, 40, 0, 0"" />
        <Grid RowSpacing=""0"" ColumnSpacing=""20"">
            <Grid.ColumnDefinitions>
                <ColumnDefinition Width=""Auto"" />
                <ColumnDefinition Width=""*"" />
            </Grid.ColumnDefinitions>
        {% assign row = 0 %}
        {% assign members = Group.Members | OrderBy:'Person.FullName' %}
        {% for member in members %}
            {% if member.GroupRole.IsLeader == false %}{% continue %}{% endif %}
            <Label Grid.Row=""{{ row }}"" Grid.Column=""0"" Text=""{{ member.Person.FullName }}"" />
            <Label Grid.Row=""{{ row }}"" Grid.Column=""1"" Text=""{{ member.GroupRole.Name }}"" />
            {% assign row = row | Plus:1 %}
        {% endfor %}
        </Grid>
    {% endif %}
</StackLayout>" );

            // Add Block Attribute Value
            //   Block: Group View
            //   BlockType: Group View
            //   Category: Mobile > Groups
            //   Block Location: Page=Group Detail, Site=Kingdom First Solutions Mobile App
            //   Attribute: Show Leader List
            /*   Attribute Value: True */
            RockMigrationHelper.AddBlockAttributeValue( "376C8FED-8875-4628-92D8-8AA5F944BBF1", "808D607F-D097-48C5-BC3A-988141A1C69C", @"True" );

            // Add Block Attribute Value
            //   Block: Group Member List
            //   BlockType: Group Member List
            //   Category: Mobile > Groups
            //   Block Location: Page=Group Detail, Site=Kingdom First Solutions Mobile App
            //   Attribute: Title Template
            /*   Attribute Value: Group Roster */
            RockMigrationHelper.AddBlockAttributeValue( "75E364B8-1439-491A-9CA0-87E843C6AE7C", "FB6FA5A4-74C7-4E17-8764-118C01FCD192", @"Group Roster" );

            // Add Block Attribute Value
            //   Block: Group Member List
            //   BlockType: Group Member List
            //   Category: Mobile > Groups
            //   Block Location: Page=Group Detail, Site=Kingdom First Solutions Mobile App
            //   Attribute: Additional Fields
            /*   Attribute Value: [{"Id":290198256,"Key":"GroupId","Value":"{{ item.GroupId }}","FieldName":"GroupId","FieldSource":0,"AttributeFormat":0,"FieldFormat":1},{"Id":-1926235312,"Key":"CurrentPersonIsGroupLeader","Value":"{%- assign isGroupLeader = false -%}\n{%- assign groupIdStr = Group.Id | AsString -%}\n{%- assign groupMembers = CurrentPerson | Group:groupIdStr -%}\n{%- for groupMember in groupMembers -%}\n    {%- if groupMember.GroupRole.IsLeader -%}\n        {%- assign isGroupLeader = true -%}\n    {%- endif -%}\n{%- endfor -%}\n{{ isGroupLeader }}","FieldName":"","FieldSource":2,"AttributeFormat":0,"FieldFormat":3}] */
            RockMigrationHelper.AddBlockAttributeValue( "75E364B8-1439-491A-9CA0-87E843C6AE7C", "D5942AD0-5EA5-4ACA-8D5F-66FDE86E2E61", @"[{""Id"":290198256,""Key"":""GroupId"",""Value"":""{{ item.GroupId }}"",""FieldName"":""GroupId"",""FieldSource"":0,""AttributeFormat"":0,""FieldFormat"":1},{""Id"":-1926235312,""Key"":""CurrentPersonIsGroupLeader"",""Value"":""{%- assign isGroupLeader = false -%}\n{%- assign groupIdStr = Group.Id | AsString -%}\n{%- assign groupMembers = CurrentPerson | Group:groupIdStr -%}\n{%- for groupMember in groupMembers -%}\n    {%- if groupMember.GroupRole.IsLeader -%}\n        {%- assign isGroupLeader = true -%}\n    {%- endif -%}\n{%- endfor -%}\n{{ isGroupLeader }}"",""FieldName"":"""",""FieldSource"":2,""AttributeFormat"":0,""FieldFormat"":3}]" );

            // Add Block Attribute Value
            //   Block: Group Member List
            //   BlockType: Group Member List
            //   Category: Mobile > Groups
            //   Block Location: Page=Group Detail, Site=Kingdom First Solutions Mobile App
            //   Attribute: Template
            /*   Attribute Value: ... */
            RockMigrationHelper.AddBlockAttributeValue( "75E364B8-1439-491A-9CA0-87E843C6AE7C", "4B1F3DAE-180E-45E6-BC36-2BEA0A03C674", @"ffffffff-ffff-ffff-ffff-ffffffffffff|<StackLayout StyleClass=""p-16"">
    {% assign groupMemberCount = Members | Size %}
    
    <Label StyleClass=""h1"" Text=""{{ Title | Escape }}"" />
    <Label StyleClass=""text"" Text=""{{ 'member' | ToQuantity:groupMemberCount }}"" />

    {% if Members != empty %}
        <StackLayout Spacing=""0"" Margin=""0,20,0,0"">
            <Rock:Divider />
            {% for member in Members %}
                <StackLayout Orientation=""Horizontal"" Padding=""0,16"" Spacing=""16"">
					{%- if member.CurrentPersonIsGroupLeader %}<StackLayout.GestureRecognizers>
						<TapGestureRecognizer Command=""{Binding PushPage}"" CommandParameter=""{{ DetailPage }}?GroupMemberGuid={{ member.Guid }}"" />
					</StackLayout.GestureRecognizers>
					{% endif %}
					
					<Rock:Image Source=""{{ member.PhotoUrl | Append:'&width=400' | Escape }}"" HeightRequest=""64"" WidthRequest=""64"" Aspect=""AspectFill"" BackgroundColor=""#ccc"">
						<Rock:RoundedTransformation CornerRadius=""8"" />
					</Rock:Image>
					
		
		            <StackLayout Spacing=""0"" HorizontalOptions=""FillAndExpand"" VerticalOptions=""Center"">
						<Label StyleClass=""h4"" Text=""{{ member.FullName | Escape }}"" />
						<Label StyleClass=""text, o-60"" Text=""{{ groupIdStr }} {{ member.GroupRole | Escape }} {{ groupIdStr }}"" />
					</StackLayout>
					{% if member.CurrentPersonIsGroupLeader %}<Rock:Icon IconClass=""chevron-right"" Margin=""0,0,20,0"" VerticalOptions=""Center"" />{% endif %}
				</StackLayout>
				<Rock:Divider />	
			{% endfor %}
        </StackLayout>
    {% endif %}
</StackLayout>" );

            // Add Block Attribute Value
            //   Block: Group Member List
            //   BlockType: Group Member List
            //   Category: Mobile > Groups
            //   Block Location: Page=Group Detail, Site=Kingdom First Solutions Mobile App
            //   Attribute: Group Member Detail Page
            /*   Attribute Value: 78671dcd-a1e0-461f-8f28-b0c10467be57 */
            RockMigrationHelper.AddBlockAttributeValue( "75E364B8-1439-491A-9CA0-87E843C6AE7C", "D5629B9E-59EE-40D4-B4BA-E23DFAC33D61", @"78671dcd-a1e0-461f-8f28-b0c10467be57" );

            // Add Block Attribute Value
            //   Block: Content
            //   BlockType: Content
            //   Category: Mobile > Cms
            //   Block Location: Page=Login, Site=Kingdom First Solutions Mobile App
            //   Attribute: Dynamic Content
            /*   Attribute Value: False */
            RockMigrationHelper.AddBlockAttributeValue( "3245F127-A3B7-41A8-B004-945C726A1B94", "B31D29A0-3725-4AEB-8360-7D91B9CDFE47", @"False" );

            // Add Block Attribute Value
            //   Block: Content
            //   BlockType: Content
            //   Category: Mobile > Cms
            //   Block Location: Page=Login, Site=Kingdom First Solutions Mobile App
            //   Attribute: Content
            /*   Attribute Value: <Label Text="Login" StyleClass="p-16,h3" /> */
            RockMigrationHelper.AddBlockAttributeValue( "3245F127-A3B7-41A8-B004-945C726A1B94", "5682EDBF-68DA-4B43-A593-6C2B936C2839", @"<Label Text=""Login"" StyleClass=""p-16,h3"" />" );

            // Add Block Attribute Value
            //   Block: Login
            //   BlockType: Login
            //   Category: Mobile > Cms
            //   Block Location: Page=Login, Site=Kingdom First Solutions Mobile App
            //   Attribute: Registration Page
            /*   Attribute Value: 641c23ae-c04e-4ea6-85e3-b33f2062f36d */
            RockMigrationHelper.AddBlockAttributeValue( "4B26BE7A-A7A3-44D8-BD2B-B87506383D58", "61B98E57-B508-4384-9606-8A4D6E827658", @"641c23ae-c04e-4ea6-85e3-b33f2062f36d" );

            // Add Block Attribute Value
            //   Block: Login
            //   BlockType: Login
            //   Category: Mobile > Cms
            //   Block Location: Page=Login, Site=Kingdom First Solutions Mobile App
            //   Attribute: Forgot Password URL
            /*   Attribute Value: https://rock.cartervillechristian.com/ForgotUserName */
            RockMigrationHelper.AddBlockAttributeValue( "4B26BE7A-A7A3-44D8-BD2B-B87506383D58", "0036807C-7742-48DE-BAD4-E025DE37A215", @"https://rock.cartervillechristian.com/ForgotUserName" );

            // Add Block Attribute Value
            //   Block: Login
            //   BlockType: Login
            //   Category: Mobile > Cms
            //   Block Location: Page=Login, Site=Kingdom First Solutions Mobile App
            //   Attribute: Confirm Account Template
            /*   Attribute Value: 17aaceef-15ca-4c30-9a3a-11e6cf7e6411 */
            RockMigrationHelper.AddBlockAttributeValue( "4B26BE7A-A7A3-44D8-BD2B-B87506383D58", "A425BCFB-F882-4094-B41B-66A79FA4C902", @"17aaceef-15ca-4c30-9a3a-11e6cf7e6411" );

            // Add Block Attribute Value
            //   Block: Onboard Person
            //   BlockType: Onboard Person
            //   Category: Mobile > Security
            //   Block Location: Page=Onboard, Site=Kingdom First Solutions Mobile App
            //   Attribute: Disable Matching for the Following Protection Profiles
            /*   Attribute Value: 2,3 */
            RockMigrationHelper.AddBlockAttributeValue( "41D6C985-C794-46E2-A2F4-10745072C68D", "3D8F0BF8-7C5D-4043-9DB9-E053413914D9", @"2,3" );

            // Add Block Attribute Value
            //   Block: Onboard Person
            //   BlockType: Onboard Person
            //   Category: Mobile > Security
            //   Block Location: Page=Onboard, Site=Kingdom First Solutions Mobile App
            //   Attribute: Completed Page
            /*   Attribute Value: 058f59cd-86d7-420b-81f0-2efa2543cddc */
            RockMigrationHelper.AddBlockAttributeValue( "41D6C985-C794-46E2-A2F4-10745072C68D", "D09D1B5F-A208-48AB-9E80-64E032C071B4", @"058f59cd-86d7-420b-81f0-2efa2543cddc" );

            // Add Block Attribute Value
            //   Block: Onboard Person
            //   BlockType: Onboard Person
            //   Category: Mobile > Security
            //   Block Location: Page=Onboard, Site=Kingdom First Solutions Mobile App
            //   Attribute: Login Page
            /*   Attribute Value: 23dcb577-ad0c-4dc6-97a3-2e1f0834d005 */
            RockMigrationHelper.AddBlockAttributeValue( "41D6C985-C794-46E2-A2F4-10745072C68D", "41B67475-F8D4-42EB-991D-71A327A08077", @"23dcb577-ad0c-4dc6-97a3-2e1f0834d005" );

            // Add Block Attribute Value
            //   Block: Onboard Person
            //   BlockType: Onboard Person
            //   Category: Mobile > Security
            //   Block Location: Page=Onboard, Site=Kingdom First Solutions Mobile App
            //   Attribute: Default Connection Status
            /*   Attribute Value: 368dd475-242c-49c4-a42c-7278be690cc2 */
            RockMigrationHelper.AddBlockAttributeValue( "41D6C985-C794-46E2-A2F4-10745072C68D", "487C6E9E-BF4F-4111-9C17-DEF87EADB213", @"368dd475-242c-49c4-a42c-7278be690cc2" );

            // Add Block Attribute Value
            //   Block: Onboard Person
            //   BlockType: Onboard Person
            //   Category: Mobile > Security
            //   Block Location: Page=Onboard, Site=Kingdom First Solutions Mobile App
            //   Attribute: Default Record Status
            /*   Attribute Value: 283999ec-7346-42e3-b807-bce9b2babb49 */
            RockMigrationHelper.AddBlockAttributeValue( "41D6C985-C794-46E2-A2F4-10745072C68D", "EFF020AA-E9FD-4FB0-9B1E-0446E751844F", @"283999ec-7346-42e3-b807-bce9b2babb49" );

            // Add Block Attribute Value
            //   Block: Onboard Person
            //   BlockType: Onboard Person
            //   Category: Mobile > Security
            //   Block Location: Page=Onboard, Site=Kingdom First Solutions Mobile App
            //   Attribute: Display Campus Types
            /*   Attribute Value: 5a61507b-79cb-4da2-af43-6f82260203b3 */
            RockMigrationHelper.AddBlockAttributeValue( "41D6C985-C794-46E2-A2F4-10745072C68D", "5F1F626E-65F1-44AC-BF49-40FD21E3EF64", @"5a61507b-79cb-4da2-af43-6f82260203b3" );

            // Add Block Attribute Value
            //   Block: Onboard Person
            //   BlockType: Onboard Person
            //   Category: Mobile > Security
            //   Block Location: Page=Onboard, Site=Kingdom First Solutions Mobile App
            //   Attribute: Display Campus Statuses
            /*   Attribute Value: 10696fd8-d0c7-486f-b736-5fb3f5d69f1a */
            RockMigrationHelper.AddBlockAttributeValue( "41D6C985-C794-46E2-A2F4-10745072C68D", "42B6A2C0-37FF-4BA8-8378-791937AECA86", @"10696fd8-d0c7-486f-b736-5fb3f5d69f1a" );

            // Add Block Attribute Value
            //   Block: Onboard Person
            //   BlockType: Onboard Person
            //   Category: Mobile > Security
            //   Block Location: Page=Onboard, Site=Kingdom First Solutions Mobile App
            //   Attribute: Communication List Categories
            /*   Attribute Value: a0889e77-67d9-418c-b301-1b3924692058 */
            RockMigrationHelper.AddBlockAttributeValue( "41D6C985-C794-46E2-A2F4-10745072C68D", "A27FADBA-B1AD-4D5B-A11A-B63AF67BD2D5", @"a0889e77-67d9-418c-b301-1b3924692058" );

            // Add Block Attribute Value
            //   Block: Onboard Person
            //   BlockType: Onboard Person
            //   Category: Mobile > Security
            //   Block Location: Page=Onboard, Site=Kingdom First Solutions Mobile App
            //   Attribute: System Communication
            /*   Attribute Value: 478cce9f-8489-47fa-9e94-217fc36cde01 */
            RockMigrationHelper.AddBlockAttributeValue( "41D6C985-C794-46E2-A2F4-10745072C68D", "CA1F541C-3C37-438B-8D7A-3841E31E05F8", @"478cce9f-8489-47fa-9e94-217fc36cde01" );

            // Add Block Attribute Value
            //   Block: Onboard Person
            //   BlockType: Onboard Person
            //   Category: Mobile > Security
            //   Block Location: Page=Onboard, Site=Kingdom First Solutions Mobile App
            //   Attribute: Verification Time Limit
            /*   Attribute Value: 5 */
            RockMigrationHelper.AddBlockAttributeValue( "41D6C985-C794-46E2-A2F4-10745072C68D", "4FE016DB-C77C-4A09-933B-1AAC649CAC95", @"5" );

            // Add Block Attribute Value
            //   Block: Onboard Person
            //   BlockType: Onboard Person
            //   Category: Mobile > Security
            //   Block Location: Page=Onboard, Site=Kingdom First Solutions Mobile App
            //   Attribute: IP Throttle Limit
            /*   Attribute Value: 5000 */
            RockMigrationHelper.AddBlockAttributeValue( "41D6C985-C794-46E2-A2F4-10745072C68D", "293A5476-C910-407A-8EAA-CA7AB12A1F55", @"5000" );

            // Add Block Attribute Value
            //   Block: Onboard Person
            //   BlockType: Onboard Person
            //   Category: Mobile > Security
            //   Block Location: Page=Onboard, Site=Kingdom First Solutions Mobile App
            //   Attribute: Validation Code Attempts
            /*   Attribute Value: 10 */
            RockMigrationHelper.AddBlockAttributeValue( "41D6C985-C794-46E2-A2F4-10745072C68D", "9E421C90-F505-47DC-BC93-CEE75076D653", @"10" );

            // Add Block Attribute Value
            //   Block: Onboard Person
            //   BlockType: Onboard Person
            //   Category: Mobile > Security
            //   Block Location: Page=Onboard, Site=Kingdom First Solutions Mobile App
            //   Attribute: Allow Skip of Onboarding
            /*   Attribute Value: False */
            RockMigrationHelper.AddBlockAttributeValue( "41D6C985-C794-46E2-A2F4-10745072C68D", "ED3671C9-D962-4584-8FD9-D7C2FEBFACE0", @"False" );

            // Add Block Attribute Value
            //   Block: Onboard Person
            //   BlockType: Onboard Person
            //   Category: Mobile > Security
            //   Block Location: Page=Onboard, Site=Kingdom First Solutions Mobile App
            //   Attribute: Hide Gender if Known
            /*   Attribute Value: True */
            RockMigrationHelper.AddBlockAttributeValue( "41D6C985-C794-46E2-A2F4-10745072C68D", "D55F9A7C-CEB0-468A-86B2-A1B34B09B302", @"True" );

            // Add Block Attribute Value
            //   Block: Onboard Person
            //   BlockType: Onboard Person
            //   Category: Mobile > Security
            //   Block Location: Page=Onboard, Site=Kingdom First Solutions Mobile App
            //   Attribute: Hide Birth Date if Known
            /*   Attribute Value: True */
            RockMigrationHelper.AddBlockAttributeValue( "41D6C985-C794-46E2-A2F4-10745072C68D", "FD94209A-76D6-4D41-B398-96745F546D5B", @"True" );

            // Add Block Attribute Value
            //   Block: Onboard Person
            //   BlockType: Onboard Person
            //   Category: Mobile > Security
            //   Block Location: Page=Onboard, Site=Kingdom First Solutions Mobile App
            //   Attribute: Hide Mobile Phone if Known
            /*   Attribute Value: True */
            RockMigrationHelper.AddBlockAttributeValue( "41D6C985-C794-46E2-A2F4-10745072C68D", "4AA3A746-C705-418A-8FB5-FB5690D1429D", @"True" );

            // Add Block Attribute Value
            //   Block: Onboard Person
            //   BlockType: Onboard Person
            //   Category: Mobile > Security
            //   Block Location: Page=Onboard, Site=Kingdom First Solutions Mobile App
            //   Attribute: Hide Email if Known
            /*   Attribute Value: True */
            RockMigrationHelper.AddBlockAttributeValue( "41D6C985-C794-46E2-A2F4-10745072C68D", "1A4321DF-3AAD-4A2C-8070-A068820BE827", @"True" );

            // Add Block Attribute Value
            //   Block: Onboard Person
            //   BlockType: Onboard Person
            //   Category: Mobile > Security
            //   Block Location: Page=Onboard, Site=Kingdom First Solutions Mobile App
            //   Attribute: Show Notifications Request
            /*   Attribute Value: True */
            RockMigrationHelper.AddBlockAttributeValue( "41D6C985-C794-46E2-A2F4-10745072C68D", "8458FD9D-C6C1-405B-9A93-CB713CD9A953", @"True" );

            // Add Block Attribute Value
            //   Block: Onboard Person
            //   BlockType: Onboard Person
            //   Category: Mobile > Security
            //   Block Location: Page=Onboard, Site=Kingdom First Solutions Mobile App
            //   Attribute: Hide Campus if Known
            /*   Attribute Value: True */
            RockMigrationHelper.AddBlockAttributeValue( "41D6C985-C794-46E2-A2F4-10745072C68D", "D00B6A18-D302-4A8C-ADF7-CD46ABF884C7", @"True" );

            // Add Block Attribute Value
            //   Block: Onboard Person
            //   BlockType: Onboard Person
            //   Category: Mobile > Security
            //   Block Location: Page=Onboard, Site=Kingdom First Solutions Mobile App
            //   Attribute: Gender
            /*   Attribute Value: 1 */
            RockMigrationHelper.AddBlockAttributeValue( "41D6C985-C794-46E2-A2F4-10745072C68D", "2D218849-F96F-4B0B-9B27-5236E8A6F8CB", @"1" );

            // Add Block Attribute Value
            //   Block: Onboard Person
            //   BlockType: Onboard Person
            //   Category: Mobile > Security
            //   Block Location: Page=Onboard, Site=Kingdom First Solutions Mobile App
            //   Attribute: Birth Date
            /*   Attribute Value: 1 */
            RockMigrationHelper.AddBlockAttributeValue( "41D6C985-C794-46E2-A2F4-10745072C68D", "3E362C81-EB06-444A-8E8D-92A763177B8E", @"1" );

            // Add Block Attribute Value
            //   Block: Onboard Person
            //   BlockType: Onboard Person
            //   Category: Mobile > Security
            //   Block Location: Page=Onboard, Site=Kingdom First Solutions Mobile App
            //   Attribute: Mobile Phone
            /*   Attribute Value: 1 */
            RockMigrationHelper.AddBlockAttributeValue( "41D6C985-C794-46E2-A2F4-10745072C68D", "E88A11E9-2118-4D70-B8CD-9E7B58AD07D3", @"1" );

            // Add Block Attribute Value
            //   Block: Onboard Person
            //   BlockType: Onboard Person
            //   Category: Mobile > Security
            //   Block Location: Page=Onboard, Site=Kingdom First Solutions Mobile App
            //   Attribute: Email
            /*   Attribute Value: 1 */
            RockMigrationHelper.AddBlockAttributeValue( "41D6C985-C794-46E2-A2F4-10745072C68D", "386C39D8-1D77-47F4-94F1-129F03AD7B88", @"1" );

            // Add Block Attribute Value
            //   Block: Onboard Person
            //   BlockType: Onboard Person
            //   Category: Mobile > Security
            //   Block Location: Page=Onboard, Site=Kingdom First Solutions Mobile App
            //   Attribute: Create Login
            /*   Attribute Value: 1 */
            RockMigrationHelper.AddBlockAttributeValue( "41D6C985-C794-46E2-A2F4-10745072C68D", "9106D3D4-20B0-47D4-81F3-A4FFF4D01D17", @"1" );

            // Add Block Attribute Value
            //   Block: Onboard Person
            //   BlockType: Onboard Person
            //   Category: Mobile > Security
            //   Block Location: Page=Onboard, Site=Kingdom First Solutions Mobile App
            //   Attribute: Hello Screen Title
            /*   Attribute Value: Hello! */
            RockMigrationHelper.AddBlockAttributeValue( "41D6C985-C794-46E2-A2F4-10745072C68D", "03152B49-B22B-478E-8EB2-BAD959A6E39A", @"Hello!" );

            // Add Block Attribute Value
            //   Block: Onboard Person
            //   BlockType: Onboard Person
            //   Category: Mobile > Security
            //   Block Location: Page=Onboard, Site=Kingdom First Solutions Mobile App
            //   Attribute: Hello Screen Subtitle
            /*   Attribute Value: Welcome to the C3 Test mobile app. Please sign-in so we can personalize your experience. */
            RockMigrationHelper.AddBlockAttributeValue( "41D6C985-C794-46E2-A2F4-10745072C68D", "C93E9926-0010-4B0E-A4D7-AA30B41F0BBF", @"Welcome to the C3 Test mobile app. Please sign-in so we can personalize your experience." );

            // Add Block Attribute Value
            //   Block: Onboard Person
            //   BlockType: Onboard Person
            //   Category: Mobile > Security
            //   Block Location: Page=Onboard, Site=Kingdom First Solutions Mobile App
            //   Attribute: Code Sent Screen Title
            /*   Attribute Value: Code Sent... */
            RockMigrationHelper.AddBlockAttributeValue( "41D6C985-C794-46E2-A2F4-10745072C68D", "18D9FB2D-1898-4957-95F0-2A5D2A55526A", @"Code Sent..." );

            // Add Block Attribute Value
            //   Block: Onboard Person
            //   BlockType: Onboard Person
            //   Category: Mobile > Security
            //   Block Location: Page=Onboard, Site=Kingdom First Solutions Mobile App
            //   Attribute: Code Sent Screen Subtitle
            /*   Attribute Value: You should be recieving a verification code from us shortly. When it arrives type or paste it below. */
            RockMigrationHelper.AddBlockAttributeValue( "41D6C985-C794-46E2-A2F4-10745072C68D", "A89FC144-DB98-4129-BAB7-EADA64F8FFCA", @"You should be recieving a verification code from us shortly. When it arrives type or paste it below." );

            // Add Block Attribute Value
            //   Block: Onboard Person
            //   BlockType: Onboard Person
            //   Category: Mobile > Security
            //   Block Location: Page=Onboard, Site=Kingdom First Solutions Mobile App
            //   Attribute: Name Screen Title
            /*   Attribute Value: Let’s Get to Know You */
            RockMigrationHelper.AddBlockAttributeValue( "41D6C985-C794-46E2-A2F4-10745072C68D", "FEC42D9A-A95C-4FEC-95DB-400B738BB250", @"Let’s Get to Know You" );

            // Add Block Attribute Value
            //   Block: Onboard Person
            //   BlockType: Onboard Person
            //   Category: Mobile > Security
            //   Block Location: Page=Onboard, Site=Kingdom First Solutions Mobile App
            //   Attribute: Name Screen Subtitle
            /*   Attribute Value: To maximize your experience we’d like to know a little about you. */
            RockMigrationHelper.AddBlockAttributeValue( "41D6C985-C794-46E2-A2F4-10745072C68D", "6B463687-EDCF-4C44-9BE6-96B00B6C23B0", @"To maximize your experience we’d like to know a little about you." );

            // Add Block Attribute Value
            //   Block: Onboard Person
            //   BlockType: Onboard Person
            //   Category: Mobile > Security
            //   Block Location: Page=Onboard, Site=Kingdom First Solutions Mobile App
            //   Attribute: Personal Information Screen Title
            /*   Attribute Value: Tell Us More */
            RockMigrationHelper.AddBlockAttributeValue( "41D6C985-C794-46E2-A2F4-10745072C68D", "BBF73E74-45DF-4E41-812D-A704E6409B14", @"Tell Us More" );

            // Add Block Attribute Value
            //   Block: Onboard Person
            //   BlockType: Onboard Person
            //   Category: Mobile > Security
            //   Block Location: Page=Onboard, Site=Kingdom First Solutions Mobile App
            //   Attribute: Personal Information Screen Subtitle
            /*   Attribute Value: The more we know the more we can tailor our ministry to you. */
            RockMigrationHelper.AddBlockAttributeValue( "41D6C985-C794-46E2-A2F4-10745072C68D", "29F7B8FD-1DAF-4062-9720-A735F60843AB", @"The more we know the more we can tailor our ministry to you." );

            // Add Block Attribute Value
            //   Block: Onboard Person
            //   BlockType: Onboard Person
            //   Category: Mobile > Security
            //   Block Location: Page=Onboard, Site=Kingdom First Solutions Mobile App
            //   Attribute: Contact Information Screen Title
            /*   Attribute Value: Stay Connected */
            RockMigrationHelper.AddBlockAttributeValue( "41D6C985-C794-46E2-A2F4-10745072C68D", "77D080E2-1CCD-4A1D-A641-20D9F0734ABF", @"Stay Connected" );

            // Add Block Attribute Value
            //   Block: Onboard Person
            //   BlockType: Onboard Person
            //   Category: Mobile > Security
            //   Block Location: Page=Onboard, Site=Kingdom First Solutions Mobile App
            //   Attribute: Contact Information Screen Subtitle
            /*   Attribute Value: Help us keep you in the loop by providing your contact information. */
            RockMigrationHelper.AddBlockAttributeValue( "41D6C985-C794-46E2-A2F4-10745072C68D", "2DF996A3-002D-4C89-8AE4-9FC49F846107", @"Help us keep you in the loop by providing your contact information." );

            // Add Block Attribute Value
            //   Block: Onboard Person
            //   BlockType: Onboard Person
            //   Category: Mobile > Security
            //   Block Location: Page=Onboard, Site=Kingdom First Solutions Mobile App
            //   Attribute: Interests Screen Title
            /*   Attribute Value: Topics Of Interest */
            RockMigrationHelper.AddBlockAttributeValue( "41D6C985-C794-46E2-A2F4-10745072C68D", "7C3F512A-7092-41C8-A6AA-F9427EFB313B", @"Topics Of Interest" );

            // Add Block Attribute Value
            //   Block: Onboard Person
            //   BlockType: Onboard Person
            //   Category: Mobile > Security
            //   Block Location: Page=Onboard, Site=Kingdom First Solutions Mobile App
            //   Attribute: Interests Screen Subtitle
            /*   Attribute Value: What topics are you most interested in. */
            RockMigrationHelper.AddBlockAttributeValue( "41D6C985-C794-46E2-A2F4-10745072C68D", "7F13BCFE-A16A-4688-AC74-30848B3DD1B2", @"What topics are you most interested in." );

            // Add Block Attribute Value
            //   Block: Onboard Person
            //   BlockType: Onboard Person
            //   Category: Mobile > Security
            //   Block Location: Page=Onboard, Site=Kingdom First Solutions Mobile App
            //   Attribute: Notifications Screen Title
            /*   Attribute Value: Enable Notifications */
            RockMigrationHelper.AddBlockAttributeValue( "41D6C985-C794-46E2-A2F4-10745072C68D", "3C3F5F53-D303-4465-A6BF-58C5F1365134", @"Enable Notifications" );

            // Add Block Attribute Value
            //   Block: Onboard Person
            //   BlockType: Onboard Person
            //   Category: Mobile > Security
            //   Block Location: Page=Onboard, Site=Kingdom First Solutions Mobile App
            //   Attribute: Notifications Screen Subtitle
            /*   Attribute Value: We’d like to keep you in the loop with important alerts and notifications. */
            RockMigrationHelper.AddBlockAttributeValue( "41D6C985-C794-46E2-A2F4-10745072C68D", "98D17BDF-C2F9-4F78-BFD8-7BE160787A24", @"We’d like to keep you in the loop with important alerts and notifications." );

            // Add Block Attribute Value
            //   Block: Onboard Person
            //   BlockType: Onboard Person
            //   Category: Mobile > Security
            //   Block Location: Page=Onboard, Site=Kingdom First Solutions Mobile App
            //   Attribute: Campus Screen Title
            /*   Attribute Value: Find Your Campus */
            RockMigrationHelper.AddBlockAttributeValue( "41D6C985-C794-46E2-A2F4-10745072C68D", "6E725307-72A3-4D2E-AA7E-B9DDE5187A67", @"Find Your Campus" );

            // Add Block Attribute Value
            //   Block: Onboard Person
            //   BlockType: Onboard Person
            //   Category: Mobile > Security
            //   Block Location: Page=Onboard, Site=Kingdom First Solutions Mobile App
            //   Attribute: Campus Screen Subtitle
            /*   Attribute Value: Select the campus you attend to get targets news and information about events. */
            RockMigrationHelper.AddBlockAttributeValue( "41D6C985-C794-46E2-A2F4-10745072C68D", "025306AB-40BA-490F-A9C5-BD1FC21E801D", @"Select the campus you attend to get targets news and information about events." );

            // Add Block Attribute Value
            //   Block: Onboard Person
            //   BlockType: Onboard Person
            //   Category: Mobile > Security
            //   Block Location: Page=Onboard, Site=Kingdom First Solutions Mobile App
            //   Attribute: Create Login Screen Title
            /*   Attribute Value: Create Login */
            RockMigrationHelper.AddBlockAttributeValue( "41D6C985-C794-46E2-A2F4-10745072C68D", "DF658448-7F79-473B-AE1E-F3B2DA7EBBF8", @"Create Login" );

            // Add Block Attribute Value
            //   Block: Onboard Person
            //   BlockType: Onboard Person
            //   Category: Mobile > Security
            //   Block Location: Page=Onboard, Site=Kingdom First Solutions Mobile App
            //   Attribute: Create Login Screen Subtitle
            /*   Attribute Value: Create a login to help signing in quicker in the future. */
            RockMigrationHelper.AddBlockAttributeValue( "41D6C985-C794-46E2-A2F4-10745072C68D", "EAED4835-A87E-4EEE-8345-D01DF6FF1DD7", @"Create a login to help signing in quicker in the future." );

            // Add Block Attribute Value
            //   Block: Register
            //   BlockType: Register
            //   Category: Mobile > Cms
            //   Block Location: Page=Register, Site=Kingdom First Solutions Mobile App
            //   Attribute: Gender
            /*   Attribute Value: 1 */
            RockMigrationHelper.AddBlockAttributeValue( "A6C0DE83-D710-4062-8C4D-BDF6900C36B8", "AAB94EB6-AAD8-4D68-8B56-22F192DAA7E1", @"1" );

            // Add Block Attribute Value
            //   Block: Register
            //   BlockType: Register
            //   Category: Mobile > Cms
            //   Block Location: Page=Register, Site=Kingdom First Solutions Mobile App
            //   Attribute: Connection Status
            /*   Attribute Value: 368dd475-242c-49c4-a42c-7278be690cc2 */
            RockMigrationHelper.AddBlockAttributeValue( "A6C0DE83-D710-4062-8C4D-BDF6900C36B8", "AF36BCE7-62F7-461F-80FC-77343925FE3E", @"368dd475-242c-49c4-a42c-7278be690cc2" );

            // Add Block Attribute Value
            //   Block: Register
            //   BlockType: Register
            //   Category: Mobile > Cms
            //   Block Location: Page=Register, Site=Kingdom First Solutions Mobile App
            //   Attribute: Record Status
            /*   Attribute Value: 283999ec-7346-42e3-b807-bce9b2babb49 */
            RockMigrationHelper.AddBlockAttributeValue( "A6C0DE83-D710-4062-8C4D-BDF6900C36B8", "BC9E9B0F-0097-4032-B348-8EA1C5B56E6D", @"283999ec-7346-42e3-b807-bce9b2babb49" );

            // Add Block Attribute Value
            //   Block: Register
            //   BlockType: Register
            //   Category: Mobile > Cms
            //   Block Location: Page=Register, Site=Kingdom First Solutions Mobile App
            //   Attribute: Birthdate Show
            /*   Attribute Value: True */
            RockMigrationHelper.AddBlockAttributeValue( "A6C0DE83-D710-4062-8C4D-BDF6900C36B8", "70992EE5-D8AF-420F-AA19-E5A0743F679F", @"True" );

            // Add Block Attribute Value
            //   Block: Register
            //   BlockType: Register
            //   Category: Mobile > Cms
            //   Block Location: Page=Register, Site=Kingdom First Solutions Mobile App
            //   Attribute: BirthDate Required
            /*   Attribute Value: False */
            RockMigrationHelper.AddBlockAttributeValue( "A6C0DE83-D710-4062-8C4D-BDF6900C36B8", "7C30E10D-3254-43C5-B0FF-36CB5012B708", @"False" );

            // Add Block Attribute Value
            //   Block: Register
            //   BlockType: Register
            //   Category: Mobile > Cms
            //   Block Location: Page=Register, Site=Kingdom First Solutions Mobile App
            //   Attribute: Campus Show
            /*   Attribute Value: True */
            RockMigrationHelper.AddBlockAttributeValue( "A6C0DE83-D710-4062-8C4D-BDF6900C36B8", "9410EFC3-43DE-48C7-92BC-09ECD4F8E63F", @"True" );

            // Add Block Attribute Value
            //   Block: Register
            //   BlockType: Register
            //   Category: Mobile > Cms
            //   Block Location: Page=Register, Site=Kingdom First Solutions Mobile App
            //   Attribute: Campus Required
            /*   Attribute Value: True */
            RockMigrationHelper.AddBlockAttributeValue( "A6C0DE83-D710-4062-8C4D-BDF6900C36B8", "B9F4F69B-86FE-47BE-8AC4-5C90C939F93D", @"True" );

            // Add Block Attribute Value
            //   Block: Register
            //   BlockType: Register
            //   Category: Mobile > Cms
            //   Block Location: Page=Register, Site=Kingdom First Solutions Mobile App
            //   Attribute: Email Show
            /*   Attribute Value: True */
            RockMigrationHelper.AddBlockAttributeValue( "A6C0DE83-D710-4062-8C4D-BDF6900C36B8", "B6045ACD-F384-4CEE-93C7-EA6C87024601", @"True" );

            // Add Block Attribute Value
            //   Block: Register
            //   BlockType: Register
            //   Category: Mobile > Cms
            //   Block Location: Page=Register, Site=Kingdom First Solutions Mobile App
            //   Attribute: Email Required
            /*   Attribute Value: True */
            RockMigrationHelper.AddBlockAttributeValue( "A6C0DE83-D710-4062-8C4D-BDF6900C36B8", "104CE646-73E5-46FA-B007-C4BF0FCC68B3", @"True" );

            // Add Block Attribute Value
            //   Block: Register
            //   BlockType: Register
            //   Category: Mobile > Cms
            //   Block Location: Page=Register, Site=Kingdom First Solutions Mobile App
            //   Attribute: Mobile Phone Show
            /*   Attribute Value: True */
            RockMigrationHelper.AddBlockAttributeValue( "A6C0DE83-D710-4062-8C4D-BDF6900C36B8", "65C84E5A-4D7E-4207-B019-75AA3458487F", @"True" );

            // Add Block Attribute Value
            //   Block: Register
            //   BlockType: Register
            //   Category: Mobile > Cms
            //   Block Location: Page=Register, Site=Kingdom First Solutions Mobile App
            //   Attribute: Mobile Phone Required
            /*   Attribute Value: True */
            RockMigrationHelper.AddBlockAttributeValue( "A6C0DE83-D710-4062-8C4D-BDF6900C36B8", "A5EE3A3C-0455-4966-90FF-5DC1000EDC67", @"True" );

            // Add Block Attribute Value
            //   Block: Register
            //   BlockType: Register
            //   Category: Mobile > Cms
            //   Block Location: Page=Register, Site=Kingdom First Solutions Mobile App
            //   Attribute: Check For Duplicates
            /*   Attribute Value: True */
            RockMigrationHelper.AddBlockAttributeValue( "A6C0DE83-D710-4062-8C4D-BDF6900C36B8", "ECC43369-9E44-4570-81C5-8E670BDBFE50", @"True" );

            // Add Block Attribute Value
            //   Block: Register
            //   BlockType: Register
            //   Category: Mobile > Cms
            //   Block Location: Page=Register, Site=Kingdom First Solutions Mobile App
            //   Attribute: Confirm Account Template
            /*   Attribute Value: 17aaceef-15ca-4c30-9a3a-11e6cf7e6411 */
            RockMigrationHelper.AddBlockAttributeValue( "A6C0DE83-D710-4062-8C4D-BDF6900C36B8", "4B3D4D4D-CA87-4B9F-85AD-CAB5AE5E6C32", @"17aaceef-15ca-4c30-9a3a-11e6cf7e6411" );

            // Add Block Attribute Value
            //   Block: Content
            //   BlockType: Content
            //   Category: Mobile > Cms
            //   Block Location: Page=My Account/Push Notifications, Site=Kingdom First Solutions Mobile App
            //   Attribute: Content
            /*   Attribute Value: ... */
            RockMigrationHelper.AddBlockAttributeValue( "48A47402-C0B4-4820-8017-BB7EF63A60B5", "5682EDBF-68DA-4B43-A593-6C2B936C2839", @"<StackLayout StyleClass=""pt-16,pb-0,bg-white"" Spacing=""16"">
    <StackLayout Orientation=""Horizontal"" HorizontalOptions=""FillAndExpand"" StyleClass=""px-8"">
        <Rock:LoginStatus HorizontalOptions=""FillAndExpand"" ProfilePageGuid=""a3929533-0055-44be-9752-38b9df024468"" ImageBorderSize=""1"" ImageBorderColor=""#f5f5f5"" TitleTextColor=""#000"" SubTitleTextColor=""#00F"" NoProfileIconColor=""{Rock:PaletteColor gray-400}"" NotLoggedInColor=""{Rock:PaletteColor gray-400}"" />
        <StackLayout HorizontalOptions=""FillAndExpand"" Spacing=""1"">
            <Button Text=""My Groups"" Command=""{Binding PushPage}"" CommandParameter=""6944f938-3379-4e0c-8fb4-188fcb383bb6"" />
{%- capture mobileCheckinUrl -%}{{ 'Global' | Attribute:'PublicApplicationRoot' | Append:'mobilecheckin' | Append:'?rckipid=' }}{{ CurrentPerson | PersonTokenCreate }}{%- endcapture -%}
            <Button Text=""Check-in"" Command=""{Binding PushPage}"" CommandParameter=""743cf21f-c3c1-4af1-8552-97a62a2b609b?url={{ mobileCheckinUrl | UrlEncode }}"" />
            <Button Text=""Logout"" Command=""{Binding Logout}"" />
        </StackLayout>
    </StackLayout>
    <Rock:Divider />
    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition />
        </Grid.RowDefinitions>
        <Grid.ColumnDefinitions>
            <ColumnDefinition />
            <ColumnDefinition />
        </Grid.ColumnDefinitions>
        <StackLayout>
            <Rock:Icon IconClass=""bell"" FontSize=""24"" HorizontalOptions=""Center"" />
            <Label Text=""Notifications"" HorizontalOptions=""Center"" StyleClass=""text-sm"" />
            <BoxView Color=""#000"" HeightRequest=""2"" />
        </StackLayout>
        <StackLayout Grid.Column=""1"">
            <Rock:Icon IconClass=""hand-holding-usd"" FontSize=""24"" HorizontalOptions=""Center"" />
            <Label Text=""Giving History"" HorizontalOptions=""Center"" StyleClass=""text-sm""  />
        </StackLayout>
        <Button Grid.Column=""1"" Command=""{Binding ReplacePage}"" CommandParameter=""0a152319-16b7-4899-b803-5780130e96a8"" StyleClass=""btn,btn-link"" />
    </Grid>
</StackLayout>" );

            // Add Block Attribute Value
            //   Block: Content
            //   BlockType: Content
            //   Category: Mobile > Cms
            //   Block Location: Page=My Account/Push Notifications, Site=Kingdom First Solutions Mobile App
            //   Attribute: Dynamic Content
            /*   Attribute Value: True */
            RockMigrationHelper.AddBlockAttributeValue( "48A47402-C0B4-4820-8017-BB7EF63A60B5", "B31D29A0-3725-4AEB-8360-7D91B9CDFE47", @"True" );

            // Add Block Attribute Value
            //   Block: Push Notification Content - Recipients
            //   BlockType: Content
            //   Category: Mobile > Cms
            //   Block Location: Page=My Account/Push Notifications, Site=Kingdom First Solutions Mobile App
            //   Attribute: Dynamic Content
            /*   Attribute Value: True */
            RockMigrationHelper.AddBlockAttributeValue( "72D2F125-C190-427D-B96E-03C1F59A9CB7", "B31D29A0-3725-4AEB-8360-7D91B9CDFE47", @"True" );

            // Add Block Attribute Value
            //   Block: Push Notification Content - Recipients
            //   BlockType: Content
            //   Category: Mobile > Cms
            //   Block Location: Page=My Account/Push Notifications, Site=Kingdom First Solutions Mobile App
            //   Attribute: Enabled Lava Commands
            /*   Attribute Value: RockEntity,Sql */
            RockMigrationHelper.AddBlockAttributeValue( "72D2F125-C190-427D-B96E-03C1F59A9CB7", "24516448-3F1F-4F27-97A1-CFB4F8B277B5", @"RockEntity,Sql" );

            // Add Block Attribute Value
            //   Block: Push Notification Content - Recipients
            //   BlockType: Content
            //   Category: Mobile > Cms
            //   Block Location: Page=My Account/Push Notifications, Site=Kingdom First Solutions Mobile App
            //   Attribute: Content
            /*   Attribute Value: ... */
            RockMigrationHelper.AddBlockAttributeValue( "72D2F125-C190-427D-B96E-03C1F59A9CB7", "5682EDBF-68DA-4B43-A593-6C2B936C2839", @"{%- assign paramLimit = PageParameter.Limit | Default:'25' | AsInteger  %}{%- assign paramPage = PageParameter.pg | Default:'1' | AsInteger -%}
{%- personaldevice where:'PersonAliasId == {{ CurrentPerson.PrimaryAliasId }}' -%}{% capture deviceids %}0,{% for device in personaldeviceItems %}{{ device.Id }},{% endfor %}{% endcapture %}{%- endpersonaldevice -%}
{%- sql -%}SELECT MIN(cr.Id) as Id FROM [CommunicationRecipient] cr JOIN EntityType et ON et.Id = cr.MediumEntityTypeId AND et.Guid = '3638c6df-4ff3-4a52-b4b8-afb754991597' WHERE PersonAliasId = {{ CurrentPerson.PrimaryAliasId }} OR PersonalDeviceId IN ({{ deviceids | ReplaceLast:"","","""" }}) GROUP BY cr.CommunicationId{% endsql %}{% capture communicationRecipientIds %}0,{% for recp in results %}{{ recp.Id }},{% endfor %}{%- endcapture -%}
<StackLayout StyleClass=""p-8"">
    <Label Text=""Mobile Shell v3"" />
    <Rock:Icon IconClass=""cog"" HorizontalOptions=""End"" Command=""{Binding PushPage}"" CommandParameter=""d426b3d7-adcd-4516-b539-fdd6426dd230"" StyleClass=""mr-16"" />
{%- communicationrecipient ids:'{{ communicationRecipientIds | ReplaceLast:"","","""" }}' sort:'CreatedDateTime DESC' limit:'{{ paramLimit }}' offset:'{{ paramPage | Minus:1 | Times:paramLimit }}' -%}{%- assign communicationRecipientItemSize = communicationrecipientItems | Size -%}
  {%- for commRecip in communicationrecipientItems %}
  <Rock:StyledView StyleClass=""bg-white,mt-8""
    CornerRadius=""4""
    HorizontalOptions=""FillAndExpand""
    HasShadow=""true""
    Elevation=""2"">
        <Rock:StyledView.GestureRecognizers>
            <TapGestureRecognizer Command=""{Binding AggregateCommand}"">
                <TapGestureRecognizer.CommandParameter>
                    <Rock:AggregateCommandParameters>
                        <Rock:CommandReference Command=""{Binding SetViewProperty}""
                            CommandParameter=""{Rock:SetViewPropertyParameters View={x:Reference UnreadMarker{{ forloop.index }}}, Name=IsVisible, Value=true}"" />

                        <Rock:CommandReference Command=""{Binding SetViewProperty}""
                            CommandParameter=""{Rock:SetViewPropertyParameters View={x:Reference UnreadMarker{{ forloop.index }}}, Name=IsVisible, Value=false}"" />
                            
                        <Rock:CommandReference Command=""{Binding SetViewProperty}""
                            CommandParameter=""{Rock:SetViewPropertyParameters View={x:Reference PushRow{{ forloop.index }}}, Name=StyleClass, Value='py-8,px-16'}"" />
                            
                        <Rock:CommandReference Command=""{Binding PushPage}""
                            CommandParameter=""341b07e0-e710-4c9a-8daa-a5121917c330?CommunicationRecipientGuid={{ commRecip.Guid }}"" />
                    </Rock:AggregateCommandParameters>
                </TapGestureRecognizer.CommandParameter>
            </TapGestureRecognizer>
        </Rock:StyledView.GestureRecognizers>
        <StackLayout x:Name=""PushRow{{ forloop.index }}"" Orientation=""Horizontal"" StyleClass=""py-8,{% if commRecip.Status != 'Opened'%}pl-8,pr-16{% else %}px-16{% endif %}"">
            <BoxView x:Name=""UnreadMarker{{ forloop.index }}"" Color=""{Rock:PaletteColor App-Primary}"" WidthRequest=""4"" HeightRequest=""4"" IsVisible=""{% if commRecip.Status != 'Opened'%}true{% else %}false{% endif %}"" />
            <StackLayout Padding=""0,5,0,5"" HorizontalOptions=""StartAndExpand"" VerticalOptions=""Center"">
                <Label Text=""{{ commRecip.Communication.PushTitle }}"" StyleClass=""font-weight-bold"" LineBreakMode=""TailTruncation"" />
                <Label Text=""{{ commRecip.Communication.SendDateTime | Date:'MMM dd' }}"" StyleClass=""text-sm"" />
            </StackLayout>
            <Rock:Icon IconClass=""angle-right"" FontSize=""24"" HorizontalOptions=""End"" VerticalOptions=""Center"" />
        </StackLayout>
    </Rock:StyledView>
{%- endfor -%}
<StackLayout Orientation=""Horizontal"">{% assign paramPageCount = paramPage | Times:paramLimit %}
    {% if paramPage > 1 %}
    <Button Text=""Previous {{ paramLimit }}"" StyleClass=""btn,btn-default,mt-16"" Command=""{Binding PopPage}"" />
    {%- endif -%}
    {%- if communicationRecipientItemSize == paramLimit -%}
    <Button Text=""Next {{ paramLimit }}"" StyleClass=""btn,btn-default,mt-16"" Command=""{Binding PushPage}"" CommandParameter=""{{ CurrentPage.Guid }}?pg={{ paramPage | Plus:1 }}"" HorizontalOptions=""EndAndExpand"" />
    {%- endif -%}
</StackLayout>
{%- if communicationRecipientItemSize < 1 -%} 
  <Rock:StyledView StyleClass=""bg-white""
    CornerRadius=""4""
    HorizontalOptions=""FillAndExpand""
    HasShadow=""true""
    Elevation=""2"">
        <StackLayout Orientation=""Horizontal"" StyleClass=""py-8,px-16"">
            <StackLayout Padding=""0,5,0,5"" HorizontalOptions=""StartAndExpand"" VerticalOptions=""Center"">
                <Label Text=""No Current Notifications"" StyleClass=""font-weight-bold"" LineBreakMode=""TailTruncation"" />
            </StackLayout>
        </StackLayout>
    </Rock:StyledView>
{%- endif -%}
{%- endcommunicationrecipient -%}
</StackLayout>" );

            // Add Block Attribute Value
            //   Block: ContentPush Notification - Recipients Shell 2.0
            //   BlockType: Content
            //   Category: Mobile > Cms
            //   Block Location: Page=My Account/Push Notifications, Site=Kingdom First Solutions Mobile App
            //   Attribute: Content
            /*   Attribute Value: ... */
            RockMigrationHelper.AddBlockAttributeValue( "3EF8CA12-457A-40B5-856B-D8CA240D2CDC", "5682EDBF-68DA-4B43-A593-6C2B936C2839", @"{%- assign paramLimit = PageParameter.Limit | Default:'25' | AsInteger  %}{%- assign paramPage = PageParameter.pg | Default:'1' | AsInteger -%}
{%- personaldevice where:'PersonAliasId == {{ CurrentPerson.PrimaryAliasId }}' -%}{% capture deviceids %}0,{% for device in personaldeviceItems %}{{ device.Id }},{% endfor %}{% endcapture %}{%- endpersonaldevice -%}
{%- sql -%}SELECT MIN(cr.Id) as Id FROM [CommunicationRecipient] cr JOIN EntityType et ON et.Id = cr.MediumEntityTypeId AND et.Guid = '3638c6df-4ff3-4a52-b4b8-afb754991597' WHERE PersonAliasId = {{ CurrentPerson.PrimaryAliasId }} OR PersonalDeviceId IN ({{ deviceids | ReplaceLast:"","","""" }}) GROUP BY cr.CommunicationId{% endsql %}{% capture communicationRecipientIds %}0,{% for recp in results %}{{ recp.Id }},{% endfor %}{%- endcapture -%}
<StackLayout StyleClass=""p-8"">
    <Label Text=""Mobile Shell v2"" />
    <Rock:Icon IconClass=""cog"" HorizontalOptions=""End"" Command=""{Binding PushPage}"" CommandParameter=""d426b3d7-adcd-4516-b539-fdd6426dd230"" StyleClass=""mr-16"" />
{%- communicationrecipient ids:'{{ communicationRecipientIds | ReplaceLast:"","","""" }}' sort:'CreatedDateTime DESC' limit:'{{ paramLimit }}' offset:'{{ paramPage | Minus:1 | Times:paramLimit }}' -%}{%- assign communicationRecipientItemSize = communicationrecipientItems | Size -%}
  {%- for commRecip in communicationrecipientItems %}
  <Rock:StyledView StyleClass=""bg-white,mt-8""
    CornerRadius=""4""
    HorizontalOptions=""FillAndExpand""
    HasShadow=""true""
    Elevation=""2"">
        <Rock:StyledView.GestureRecognizers>
            <TapGestureRecognizer Command=""{Binding PushPage}"" CommandParameter=""341b07e0-e710-4c9a-8daa-a5121917c330?CommunicationRecipientGuid={{ commRecip.Guid }}"" />
        </Rock:StyledView.GestureRecognizers>
        <StackLayout x:Name=""PushRow{{ forloop.index }}"" Orientation=""Horizontal"" StyleClass=""py-8,{% if commRecip.Status != 'Opened'%}pl-8,pr-16{% else %}px-16{% endif %}"">
            <BoxView x:Name=""UnreadMarker{{ forloop.index }}"" Color=""{Rock:PaletteColor App-Primary}"" WidthRequest=""4"" HeightRequest=""4"" IsVisible=""{% if commRecip.Status != 'Opened'%}true{% else %}false{% endif %}"" />
            <StackLayout Padding=""0,5,0,5"" HorizontalOptions=""StartAndExpand"" VerticalOptions=""Center"">
                <Label Text=""{{ commRecip.Communication.PushTitle }}"" StyleClass=""font-weight-bold"" LineBreakMode=""TailTruncation"" />
                <Label Text=""{{ commRecip.Communication.SendDateTime | Date:'MMM dd' }}"" StyleClass=""text-sm"" />
            </StackLayout>
            <Rock:Icon IconClass=""angle-right"" FontSize=""24"" HorizontalOptions=""End"" VerticalOptions=""Center"" />
        </StackLayout>
    </Rock:StyledView>
{%- endfor -%}
<StackLayout Orientation=""Horizontal"">{% assign paramPageCount = paramPage | Times:paramLimit %}
    {% if paramPage > 1 %}
    <Button Text=""Previous {{ paramLimit }}"" StyleClass=""btn,btn-default,mt-16"" Command=""{Binding PopPage}"" />
    {%- endif -%}
    {%- if communicationRecipientItemSize == paramLimit -%}
    <Button Text=""Next {{ paramLimit }}"" StyleClass=""btn,btn-default,mt-16"" Command=""{Binding PushPage}"" CommandParameter=""{{ CurrentPage.Guid }}?pg={{ paramPage | Plus:1 }}"" HorizontalOptions=""EndAndExpand"" />
    {%- endif -%}
</StackLayout>
{%- if communicationRecipientItemSize < 1 -%} 
  <Rock:StyledView StyleClass=""bg-white""
    CornerRadius=""4""
    HorizontalOptions=""FillAndExpand""
    HasShadow=""true""
    Elevation=""2"">
        <StackLayout Orientation=""Horizontal"" StyleClass=""py-8,px-16"">
            <StackLayout Padding=""0,5,0,5"" HorizontalOptions=""StartAndExpand"" VerticalOptions=""Center"">
                <Label Text=""No Current Notifications"" StyleClass=""font-weight-bold"" LineBreakMode=""TailTruncation"" />
            </StackLayout>
        </StackLayout>
    </Rock:StyledView>
{%- endif -%}
{%- endcommunicationrecipient -%}
</StackLayout>" );

            // Add Block Attribute Value
            //   Block: ContentPush Notification - Recipients Shell 2.0
            //   BlockType: Content
            //   Category: Mobile > Cms
            //   Block Location: Page=My Account/Push Notifications, Site=Kingdom First Solutions Mobile App
            //   Attribute: Enabled Lava Commands
            /*   Attribute Value: RockEntity,Sql */
            RockMigrationHelper.AddBlockAttributeValue( "3EF8CA12-457A-40B5-856B-D8CA240D2CDC", "24516448-3F1F-4F27-97A1-CFB4F8B277B5", @"RockEntity,Sql" );

            // Add Block Attribute Value
            //   Block: ContentPush Notification - Recipients Shell 2.0
            //   BlockType: Content
            //   Category: Mobile > Cms
            //   Block Location: Page=My Account/Push Notifications, Site=Kingdom First Solutions Mobile App
            //   Attribute: Dynamic Content
            /*   Attribute Value: True */
            RockMigrationHelper.AddBlockAttributeValue( "3EF8CA12-457A-40B5-856B-D8CA240D2CDC", "B31D29A0-3725-4AEB-8360-7D91B9CDFE47", @"True" );

            // Add Block Attribute Value
            //   Block: Push Notification Content - All Communications to Devices, Lists and existing notifications?
            //   BlockType: Content
            //   Category: Mobile > Cms
            //   Block Location: Page=My Account/Push Notifications, Site=Kingdom First Solutions Mobile App
            //   Attribute: Dynamic Content
            /*   Attribute Value: True */
            RockMigrationHelper.AddBlockAttributeValue( "D9E72CCB-C386-403E-A44E-BC36EA56227E", "B31D29A0-3725-4AEB-8360-7D91B9CDFE47", @"True" );

            // Add Block Attribute Value
            //   Block: Push Notification Content - All Communications to Devices, Lists and existing notifications?
            //   BlockType: Content
            //   Category: Mobile > Cms
            //   Block Location: Page=My Account/Push Notifications, Site=Kingdom First Solutions Mobile App
            //   Attribute: Enabled Lava Commands
            /*   Attribute Value: RockEntity,Sql */
            RockMigrationHelper.AddBlockAttributeValue( "D9E72CCB-C386-403E-A44E-BC36EA56227E", "24516448-3F1F-4F27-97A1-CFB4F8B277B5", @"RockEntity,Sql" );

            // Add Block Attribute Value
            //   Block: Push Notification Content - All Communications to Devices, Lists and existing notifications?
            //   BlockType: Content
            //   Category: Mobile > Cms
            //   Block Location: Page=My Account/Push Notifications, Site=Kingdom First Solutions Mobile App
            //   Attribute: Content
            /*   Attribute Value: ... */
            RockMigrationHelper.AddBlockAttributeValue( "D9E72CCB-C386-403E-A44E-BC36EA56227E", "5682EDBF-68DA-4B43-A593-6C2B936C2839", @"{%- assign paramLimit = PageParameter.Limit | Default:'25' | AsInteger  %}{%- assign paramPage = PageParameter.pg | Default:'1' | AsInteger -%}
{%- personaldevice where:'PersonAliasId == {{ CurrentPerson.PrimaryAliasId }}' -%}{% capture deviceids %}0,{% for device in personaldeviceItems %}{{ device.Id }},{% endfor %}{% endcapture %}{%- endpersonaldevice -%}
{%- sql -%}SELECT Id FROM Communication WHERE CommunicationType = '3' AND (ListGroupId IN (SELECT DISTINCT GroupId FROM GroupMember gm JOIN [Group] g ON g.Id = gm.GroupId JOIN [GroupType] gt ON gt.Id = g.GroupTypeId AND gt.Guid = 'd1d95777-ffa3-cbb3-4a6d-658706daed33' WHERE gm.PersonId = {{ CurrentPerson.Id }}) OR (UrlReferrer IS NULL AND ListGroupId IS NULL)){% endsql %}{% capture groupCommIds %}{% for comm in results %}{{ comm.Id }},{% endfor %}{%- endcapture -%}
{%- sql -%}SELECT DISTINCT CommunicationId FROM [CommunicationRecipient] cr JOIN EntityType et ON et.Id = cr.MediumEntityTypeId AND et.Guid = '3638c6df-4ff3-4a52-b4b8-afb754991597' WHERE PersonAliasId = {{ CurrentPerson.PrimaryAliasId }} OR PersonalDeviceId IN ({{ deviceids | ReplaceLast:"","","""" }}){% endsql %}{% capture communicationIds %}0,{{ groupCommIds }}{% for recp in results %}{{ recp.CommunicationId }},{% endfor %}{%- endcapture -%}
<StackLayout StyleClass=""p-8"">
    <Rock:Icon IconClass=""cog"" HorizontalOptions=""End"" Command=""{Binding PushPage}"" CommandParameter=""d426b3d7-adcd-4516-b539-fdd6426dd230"" StyleClass=""mr-16"" />
{%- communication ids:'{{ communicationIds | ReplaceLast:"","","""" }}' where:'PushMessage _!""""' sort:'SendDateTime DESC' limit:'{{ paramLimit }}' offset:'{{ paramPage | Minus:1 | Times:paramLimit }}' -%}{%- assign communicationItemSize = communicationItems | Size -%}
  {%- for comm in communicationItems %}
    <Rock:StyledView StyleClass=""bg-white,mt-8""
    CornerRadius=""4""
    HorizontalOptions=""FillAndExpand""
    HasShadow=""true""
    Elevation=""2"">
        <Rock:StyledView.GestureRecognizers>
            <TapGestureRecognizer Command=""{Binding PushPage}"" CommandParameter=""4d7c4316-56f7-4be7-a182-0074f47081ee?ItemId={{ comm.Id }}"" />
        </Rock:StyledView.GestureRecognizers>
        <StackLayout Orientation=""Horizontal"" StyleClass=""py-8,px-16"">
            <StackLayout Padding=""0,5,0,5"" HorizontalOptions=""StartAndExpand"" VerticalOptions=""Center"">
                <Label Text=""{{ comm.PushTitle }}"" StyleClass=""font-weight-bold"" LineBreakMode=""TailTruncation"" />
                <Label Text=""{{ comm.SendDateTime | Date:'MMM dd' }}"" StyleClass=""text-sm"" />
            </StackLayout>
            <Rock:Icon IconClass=""angle-right"" FontSize=""24"" HorizontalOptions=""End"" VerticalOptions=""Center"" />
        </StackLayout>
    </Rock:StyledView>
{%- endfor -%}
<StackLayout Orientation=""Horizontal"">{% assign paramPageCount = paramPage | Times:paramLimit %}
    {% if paramPage > 1 %}
    <Button Text=""Previous {{ paramLimit }}"" StyleClass=""btn,btn-default,mt-16"" Command=""{Binding PopPage}"" />
    {%- endif -%}
    {%- if communicationItemSize == paramLimit -%}
    <Button Text=""Next {{ paramLimit }}"" StyleClass=""btn,btn-default,mt-16"" Command=""{Binding PushPage}"" CommandParameter=""{{ CurrentPage.Guid }}?pg={{ paramPage | Plus:1 }}"" HorizontalOptions=""EndAndExpand"" />
    {%- endif -%}
</StackLayout>
{%- if communicationItemSize < 1 -%} 
  <Rock:StyledView StyleClass=""bg-white""
    CornerRadius=""4""
    HorizontalOptions=""FillAndExpand""
    HasShadow=""true""
    Elevation=""2"">
        <StackLayout Orientation=""Horizontal"" StyleClass=""py-8,px-16"">
            <StackLayout Padding=""0,5,0,5"" HorizontalOptions=""StartAndExpand"" VerticalOptions=""Center"">
                <Label Text=""No Current Notifications"" StyleClass=""font-weight-bold"" LineBreakMode=""TailTruncation"" />
            </StackLayout>
        </StackLayout>
    </Rock:StyledView>
{%- endif -%}
{%- endcommunication -%}
</StackLayout>" );

            // Add Block Attribute Value
            //   Block: Content
            //   BlockType: Content
            //   Category: Mobile > Cms
            //   Block Location: Page=My Account/Giving History, Site=Kingdom First Solutions Mobile App
            //   Attribute: Content
            /*   Attribute Value: ... */
            RockMigrationHelper.AddBlockAttributeValue( "C8E2F42C-322F-4FD2-B886-571E9A202175", "5682EDBF-68DA-4B43-A593-6C2B936C2839", @"<StackLayout StyleClass=""pt-16,pb-0,bg-white"" Spacing=""16"">
    <StackLayout Orientation=""Horizontal"" HorizontalOptions=""FillAndExpand"" StyleClass=""px-8"">
        <Rock:LoginStatus HorizontalOptions=""FillAndExpand"" ProfilePageGuid=""a3929533-0055-44be-9752-38b9df024468"" ImageBorderSize=""1"" ImageBorderColor=""#f5f5f5"" TitleTextColor=""#000"" SubTitleTextColor=""#00F"" NoProfileIconColor=""{Rock:PaletteColor gray-400}"" NotLoggedInColor=""{Rock:PaletteColor gray-400}"" />
        <StackLayout HorizontalOptions=""FillAndExpand"" Spacing=""1"">
            <Button Text=""My Groups"" Command=""{Binding PushPage}"" CommandParameter=""6944f938-3379-4e0c-8fb4-188fcb383bb6"" />
{%- capture mobileCheckinUrl -%}{{ 'Global' | Attribute:'PublicApplicationRoot' | Append:'mobilecheckin' | Append:'?rckipid=' }}{{ CurrentPerson | PersonTokenCreate }}{%- endcapture -%}
            <Button Text=""Check-in"" Command=""{Binding PushPage}"" CommandParameter=""743cf21f-c3c1-4af1-8552-97a62a2b609b?url={{ mobileCheckinUrl | UrlEncode }}"" />
            <Button Text=""Logout"" Command=""{Binding Logout}"" />
        </StackLayout>
    </StackLayout>
    <Rock:Divider />
    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition />
        </Grid.RowDefinitions>
        <Grid.ColumnDefinitions>
            <ColumnDefinition />
            <ColumnDefinition />
        </Grid.ColumnDefinitions>
        <StackLayout>
            <Rock:Icon IconClass=""bell"" FontSize=""24"" HorizontalOptions=""Center"" />
            <Label Text=""Notifications"" HorizontalOptions=""Center"" StyleClass=""text-sm"" />
        </StackLayout>
        <Button Command=""{Binding ReplacePage}"" CommandParameter=""058f59cd-86d7-420b-81f0-2efa2543cddc"" StyleClass=""btn,btn-link"" />
        <StackLayout Grid.Column=""1"">
            <Rock:Icon IconClass=""hand-holding-usd"" FontSize=""24"" HorizontalOptions=""Center"" />
            <Label Text=""Giving History"" HorizontalOptions=""Center"" StyleClass=""text-sm""  />
            <BoxView Color=""#000"" HeightRequest=""2"" />
        </StackLayout>
    </Grid>
</StackLayout>" );

            // Add Block Attribute Value
            //   Block: Content
            //   BlockType: Content
            //   Category: Mobile > Cms
            //   Block Location: Page=My Account/Giving History, Site=Kingdom First Solutions Mobile App
            //   Attribute: Dynamic Content
            /*   Attribute Value: True */
            RockMigrationHelper.AddBlockAttributeValue( "C8E2F42C-322F-4FD2-B886-571E9A202175", "B31D29A0-3725-4AEB-8360-7D91B9CDFE47", @"True" );

            // Add Block Attribute Value
            //   Block: Content - Contribution List
            //   BlockType: Content
            //   Category: Mobile > Cms
            //   Block Location: Page=My Account/Giving History, Site=Kingdom First Solutions Mobile App
            //   Attribute: Enabled Lava Commands
            /*   Attribute Value: Sql */
            RockMigrationHelper.AddBlockAttributeValue( "8BCF8A77-51AD-4797-8CE8-43EBC4B84E41", "24516448-3F1F-4F27-97A1-CFB4F8B277B5", @"Sql" );

            // Add Block Attribute Value
            //   Block: Content - Contribution List
            //   BlockType: Content
            //   Category: Mobile > Cms
            //   Block Location: Page=My Account/Giving History, Site=Kingdom First Solutions Mobile App
            //   Attribute: Dynamic Content
            /*   Attribute Value: True */
            RockMigrationHelper.AddBlockAttributeValue( "8BCF8A77-51AD-4797-8CE8-43EBC4B84E41", "B31D29A0-3725-4AEB-8360-7D91B9CDFE47", @"True" );

            // Add Block Attribute Value
            //   Block: Content - Contribution List
            //   BlockType: Content
            //   Category: Mobile > Cms
            //   Block Location: Page=My Account/Giving History, Site=Kingdom First Solutions Mobile App
            //   Attribute: Content
            /*   Attribute Value: ... */
            RockMigrationHelper.AddBlockAttributeValue( "8BCF8A77-51AD-4797-8CE8-43EBC4B84E41", "5682EDBF-68DA-4B43-A593-6C2B936C2839", @"{%- sql return:'transactionlist' -%}
SELECT TOP 10 '{{ ""Global"" | Attribute:""OrganizationName"" }}' as [Name], ftd.TransactionId, ftd.AccountId, fa.PublicName as AccountName, ftd.Amount, ft.TransactionDateTime 
FROM FinancialTransactionDetail ftd
JOIN FinancialTransaction ft ON ftd.TransactionId = ft.Id
JOIN FinancialAccount fa ON ftd.AccountId = fa.Id
WHERE ft.AuthorizedPersonAliasId IN (SELECT pa.Id FROM PersonAlias pa JOIN Person p ON p.Id = pa.PersonId WHERE p.GivingId = '{{ CurrentPerson.GivingId }}')
AND fa.IsTaxDeductible = 1
ORDER BY ft.TransactionDateTime DESC
{%- endsql -%}
{%- sql return:'currentyear' -%}
SELECT SUM(Amount) as Amount 
FROM FinancialTransactionDetail ftd
JOIN FinancialTransaction ft ON ftd.TransactionId = ft.Id
JOIN FinancialAccount fa ON ftd.AccountId = fa.Id
WHERE ft.AuthorizedPersonAliasId IN (SELECT pa.Id FROM PersonAlias pa JOIN Person p ON p.Id = pa.PersonId WHERE p.GivingId = '{{ CurrentPerson.GivingId }}')
AND fa.IsTaxDeductible = 1
AND DATEPART(YEAR,ft.TransactionDateTime) = DATEPART(YEAR,GETDATE())
--AND DATEPART(YEAR,ft.TransactionDateTime) = 2014
{%- endsql -%}
{%- sql return:'previousyear' -%}
SELECT SUM(Amount) as Amount 
FROM FinancialTransactionDetail ftd
JOIN FinancialTransaction ft ON ftd.TransactionId = ft.Id
JOIN FinancialAccount fa ON ftd.AccountId = fa.Id
WHERE ft.AuthorizedPersonAliasId IN (SELECT pa.Id FROM PersonAlias pa JOIN Person p ON p.Id = pa.PersonId WHERE p.GivingId = '{{ CurrentPerson.GivingId }}')
AND fa.IsTaxDeductible = 1
AND DATEPART(YEAR,ft.TransactionDateTime) = DATEPART(YEAR,DATEADD(YEAR,-1,GETDATE()))
--AND DATEPART(YEAR,ft.TransactionDateTime) = 2013
{%- endsql -%}{% assign transactionListSize = transactionlist | Size %}{% assign previousYearTotal = 0 %}{% assign currentYearTotal = 0 %}
{%- for total in previousyear %}{% assign previousYearTotal = total.Amount | AsDouble %}{% endfor -%}
{%- for total in currentyear %}{% assign currentYearTotal = total.Amount | AsDouble %}{% endfor -%}
{%- assign loopTransactionDate = """" -%}{%- assign transactionDates = """" -%}
<StackLayout StyleClass=""px-8,py-16,bg-gray-100"">
    <Label Text=""Overview"" StyleClass=""h4"" />
    <Rock:ContainedCard 
        Elevation=""1""
        TitleJustification=""Center""
        TaglineJustification=""Center""
        Tagline=""Total Amount in {{ 'Now' | Date:'yyyy' }}""
        Title=""{{ currentYearTotal | Default:'0' | FormatAsCurrency  }}"">
        <Label HorizontalOptions=""Center"" Text=""{{ previousYearTotal | Default:'0' | FormatAsCurrency  }} in {{ 'Now' | DateAdd:-1,'y' | Date:'yyyy' }}"" StyleClass=""text-xs,text-gray-500"" FontFamily=""Baskerville"" />
    </Rock:ContainedCard>
    <StackLayout Orientation=""Horizontal"" StyleClass=""px-8,mb-16"">
        <Rock:Icon IconClass=""heart"" VerticalOptions=""Center"" IconFamily=""FontAwesomeSolid"" WidthRequest=""20"" />
        <Label Text=""Thank you for your generosity!"" StyleClass=""text-sm"" VerticalOptions=""Center"" HorizontalOptions=""FillAndExpand"" LineBreakMode=""TailTruncation"" />
        <Button Text=""Give Again"" StyleClass=""btn,btn-outline-dark,btn-sm"" Command=""{Binding PushPage}"" CommandParameter=""d71681c4-3662-4e3c-8a8b-9a72844dea43"" HorizontalOptions=""End"" />
    </StackLayout>
{%- if transactionListSize > 0 %}
    <StackLayout Orientation=""Horizontal"">
        <Label Text=""Latest Transactions"" StyleClass=""h4,mt-12"" />
        <Button Text=""View All >"" StyleClass=""btn,btn-link,font-weight-bold"" HorizontalOptions=""EndAndExpand"" Command=""{Binding PushPage}"" CommandParameter=""1e0ee431-671d-4ad4-9a31-83c12cde4d19"" />
    </StackLayout>
    <StackLayout>
    {%- for transaction in transactionlist -%}
      {% if loopTransactionDate != transaction.TransactionDateTime %}
        <Label Text=""{{ transaction.TransactionDateTime | Date:'d MMM yyyy' | Upcase }}"" StyleClass=""text-sm,font-weight-bold,color-gray-400,mt-8"" />{% endif %}{% assign loopTransactionDate = transaction.TransactionDateTime %} 
        <Rock:StyledView CornerRadius=""4"" StyleClass=""bg-white"" HasShadow=""true"" Elevation=""1"">
        <Grid Padding=""10"">
            <Grid.RowDefinitions>
                <RowDefinition Height=""Auto"" />
                <RowDefinition Height=""Auto"" />
            </Grid.RowDefinitions>
            <Grid.ColumnDefinitions>
                <ColumnDefinition Width=""Auto"" />
                <ColumnDefinition Width=""1*"" />
            </Grid.ColumnDefinitions>
            <Label
                   Text=""{{ transaction.Name }}""
                   FontAttributes=""Bold"" />
            <Label Grid.Row=""1""
                   Text=""{{ transaction.AccountName }}""
                   FontAttributes=""Italic""
                   VerticalOptions=""End"" />
            <Label Grid.Column=""1""
                   Grid.RowSpan=""2""
                   Text=""{{ transaction.Amount | FormatAsCurrency }}""
                   FontAttributes=""Bold""
                   HorizontalTextAlignment=""End"" 
                   VerticalOptions=""Center"" />
        </Grid>
        </Rock:StyledView>
    {%- endfor -%}
    </StackLayout>
{%- endif -%}
</StackLayout>" );

            // Add Block Attribute Value
            //   Block: Content
            //   BlockType: Content
            //   Category: Mobile > Cms
            //   Block Location: Page=Giving History - All Transactions, Site=Kingdom First Solutions Mobile App
            //   Attribute: Content
            /*   Attribute Value: ... */
            RockMigrationHelper.AddBlockAttributeValue( "4AA61BDD-7BE9-4910-89E7-9895B711C1EF", "5682EDBF-68DA-4B43-A593-6C2B936C2839", @"{%- assign paramLimit = PageParameter.Limit | Default:'25' | AsInteger  %}{%- assign paramPage = PageParameter.pg | Default:'1' | AsInteger -%}
{%- sql return:'transactionlist' page:'{{ paramPage }}' limit:'{{ paramLimit }}' -%}
DECLARE @PageNumber AS INT, @RowsOfPage AS INT
SET @PageNumber=CAST(@page AS INT)
SET @RowsOfPage=CAST(@limit AS INT)
SELECT '{{ ""Global"" | Attribute:""OrganizationName"" }}' as [Name], ftd.TransactionId, ftd.AccountId, fa.PublicName as AccountName, ftd.Amount, ft.TransactionDateTime 
FROM FinancialTransactionDetail ftd
JOIN FinancialTransaction ft ON ftd.TransactionId = ft.Id
JOIN FinancialAccount fa ON ftd.AccountId = fa.Id
WHERE ft.AuthorizedPersonAliasId IN (SELECT pa.Id FROM PersonAlias pa JOIN Person p ON p.Id = pa.PersonId WHERE p.GivingId = '{{ CurrentPerson.GivingId }}')
AND fa.IsTaxDeductible = 1
--AND DATEPART(YEAR,ft.TransactionDateTime) = DATEPART(YEAR,GETDATE())
--AND DATEPART(YEAR,ft.TransactionDateTime) = 2013
ORDER BY ft.TransactionDateTime DESC
OFFSET (@PageNumber-1)*@RowsOfPage ROWS
FETCH NEXT @RowsOfPage ROWS ONLY
{%- endsql -%}
{%- assign loopTransactionDate = '' -%}{%- assign transactionListSize = transactionlist | Size -%}
<StackLayout StyleClass=""p-16"">
{%- if transactionListSize > 0 -%}
{%- for transaction in transactionlist -%}
  {% if loopTransactionDate != transaction.TransactionDateTime %}
    <Label Text=""{{ transaction.TransactionDateTime | Date:'d MMM yyyy' | Upcase }}"" StyleClass=""text-sm,font-weight-bold,color-gray-400,mt-8"" />{% endif %}{% assign loopTransactionDate = transaction.TransactionDateTime %} 
    <Rock:StyledView CornerRadius=""4"" StyleClass=""bg-white"" HasShadow=""true"" Elevation=""1"">
        <Grid Padding=""10"">
            <Grid.RowDefinitions>
                <RowDefinition Height=""Auto"" />
                <RowDefinition Height=""Auto"" />
            </Grid.RowDefinitions>
            <Grid.ColumnDefinitions>
                <ColumnDefinition Width=""Auto"" />
                <ColumnDefinition Width=""1*"" />
            </Grid.ColumnDefinitions>
            <Label
                   Text=""{{ transaction.Name }}""
                   FontAttributes=""Bold"" />
            <Label Grid.Row=""1""
                   Text=""{{ transaction.AccountName }}""
                   FontAttributes=""Italic""
                   VerticalOptions=""End"" />
            <Label Grid.Column=""1""
                   Grid.RowSpan=""2""
                   Text=""{{ transaction.Amount | FormatAsCurrency }}""
                   FontAttributes=""Bold""
                   HorizontalTextAlignment=""End"" 
                   VerticalOptions=""Center"" />
        </Grid>
    </Rock:StyledView>
{%- endfor -%}
<StackLayout Orientation=""Horizontal"">{% assign paramPageCount = paramPage | Times:paramLimit %}
    {% if paramPage > 1 %}
    <Button Text=""Previous 25"" StyleClass=""btn,btn-default,mt-16"" Command=""{Binding PopPage}"" />
    {%- endif -%}
    {%- if transactionListSize == paramLimit -%}
    <Button Text=""Next 25 {% if paramPage == 1 %}Transactions{% endif %}"" StyleClass=""btn,btn-default,mt-16"" Command=""{Binding PushPage}"" CommandParameter=""{{ CurrentPage.Guid }}?pg={{ paramPage | Plus:1 }}"" HorizontalOptions=""EndAndExpand"" />
    {%- endif -%}
</StackLayout>
{% else %}
    <Label Text=""We're sorry, you currently have no transactions available."" HorizontalOptions=""Center"" HorizontalTextAlignment=""Center""  />
{%- endif -%}
</StackLayout>" );

            // Add Block Attribute Value
            //   Block: Content
            //   BlockType: Content
            //   Category: Mobile > Cms
            //   Block Location: Page=Giving History - All Transactions, Site=Kingdom First Solutions Mobile App
            //   Attribute: Dynamic Content
            /*   Attribute Value: True */
            RockMigrationHelper.AddBlockAttributeValue( "4AA61BDD-7BE9-4910-89E7-9895B711C1EF", "B31D29A0-3725-4AEB-8360-7D91B9CDFE47", @"True" );

            // Add Block Attribute Value
            //   Block: Content
            //   BlockType: Content
            //   Category: Mobile > Cms
            //   Block Location: Page=Giving History - All Transactions, Site=Kingdom First Solutions Mobile App
            //   Attribute: Enabled Lava Commands
            /*   Attribute Value: Sql */
            RockMigrationHelper.AddBlockAttributeValue( "4AA61BDD-7BE9-4910-89E7-9895B711C1EF", "24516448-3F1F-4F27-97A1-CFB4F8B277B5", @"Sql" );

            // Add Block Attribute Value
            //   Block: Content
            //   BlockType: Content
            //   Category: Mobile > Cms
            //   Block Location: Page=Push Notification - Detail, Site=Kingdom First Solutions Mobile App
            //   Attribute: Dynamic Content
            /*   Attribute Value: True */
            RockMigrationHelper.AddBlockAttributeValue( "3279A50D-8065-434B-90A6-2A17D0579ED6", "B31D29A0-3725-4AEB-8360-7D91B9CDFE47", @"True" );

            // Add Block Attribute Value
            //   Block: Content
            //   BlockType: Content
            //   Category: Mobile > Cms
            //   Block Location: Page=Push Notification - Detail, Site=Kingdom First Solutions Mobile App
            //   Attribute: Enabled Lava Commands
            /*   Attribute Value: RockEntity */
            RockMigrationHelper.AddBlockAttributeValue( "3279A50D-8065-434B-90A6-2A17D0579ED6", "24516448-3F1F-4F27-97A1-CFB4F8B277B5", @"RockEntity" );

            // Add Block Attribute Value
            //   Block: Content
            //   BlockType: Content
            //   Category: Mobile > Cms
            //   Block Location: Page=Push Notification - Detail, Site=Kingdom First Solutions Mobile App
            //   Attribute: Content
            /*   Attribute Value: ... */
            RockMigrationHelper.AddBlockAttributeValue( "3279A50D-8065-434B-90A6-2A17D0579ED6", "5682EDBF-68DA-4B43-A593-6C2B936C2839", @"{%- if PageParameter.ItemId == ""-1"" || PageParameter.ItemId == """" || PageParameter.ItemId == empty -%}
<StackLayout StyleClass=""p-16"">
    <Label Text=""Sorry, you currently have no push notifications to view."" />
</StackLayout>
{%- else -%}
{%- communication id:'{{ PageParameter.ItemId }}' securityenabled:'false' %}{% assign pushData = communication.PushData | FromJSON -%}
<StackLayout StyleClass=""p-16"">
    <Label Text=""Message Sent: {{ communication.SendDateTime | Date:'ddd, MMM d, yyyy, h:mm tt' }}"" />
    <Label Text=""{{ communication.PushTitle }}"" StyleClass=""h3"" />
{%- if communication.PushOpenMessage and communication.PushOpenMessage != '' -%}
    <Rock:Html>
        <![CDATA[
        {{ communication.PushOpenMessage | RunLava }}
        ]]>
    </Rock:Html>
{%- else -%}
    <Label Text=""{{ communication.PushMessage | RunLava }}"" StyleClass=""p"" />
{%- endif -%}
</StackLayout>
{%- endcommunication -%}
{%- endif -%}" );

            // Add Block Attribute Value
            //   Block: Hero
            //   BlockType: Hero
            //   Category: Mobile > Cms
            //   Block Location: Page=Prayer, Site=Kingdom First Solutions Mobile App
            //   Attribute: Padding
            /*   Attribute Value: 20 */
            RockMigrationHelper.AddBlockAttributeValue( "A95B3556-75EC-4C56-89D4-0BE7A25B1F71", "92DBA471-CFBC-4BC2-BD17-022663670328", @"20" );

            // Add Block Attribute Value
            //   Block: Hero
            //   BlockType: Hero
            //   Category: Mobile > Cms
            //   Block Location: Page=Prayer, Site=Kingdom First Solutions Mobile App
            //   Attribute: Background Image - Tablet
            /*   Attribute Value: e0f2b0ae-6aac-4413-bf52-5d1f186b8d24 */
            RockMigrationHelper.AddBlockAttributeValue( "A95B3556-75EC-4C56-89D4-0BE7A25B1F71", "B7A56FAF-90C6-4F79-898C-6F1FAEDF19B5", @"e0f2b0ae-6aac-4413-bf52-5d1f186b8d24" );

            // Add Block Attribute Value
            //   Block: Hero
            //   BlockType: Hero
            //   Category: Mobile > Cms
            //   Block Location: Page=Prayer, Site=Kingdom First Solutions Mobile App
            //   Attribute: Background Image - Phone
            /*   Attribute Value: 85d204b4-231e-474a-b8a2-15f54615fb39 */
            RockMigrationHelper.AddBlockAttributeValue( "A95B3556-75EC-4C56-89D4-0BE7A25B1F71", "D807B220-49A0-49EE-B4F0-F0A92ED3160F", @"85d204b4-231e-474a-b8a2-15f54615fb39" );

            // Add Block Attribute Value
            //   Block: Hero
            //   BlockType: Hero
            //   Category: Mobile > Cms
            //   Block Location: Page=Prayer, Site=Kingdom First Solutions Mobile App
            //   Attribute: Subtitle Color
            /*   Attribute Value: #ffffff */
            RockMigrationHelper.AddBlockAttributeValue( "A95B3556-75EC-4C56-89D4-0BE7A25B1F71", "5374CF14-6A4F-4CA8-9561-0A1003F28504", @"#ffffff" );

            // Add Block Attribute Value
            //   Block: Hero
            //   BlockType: Hero
            //   Category: Mobile > Cms
            //   Block Location: Page=Prayer, Site=Kingdom First Solutions Mobile App
            //   Attribute: Title Color
            /*   Attribute Value: #ffffff */
            RockMigrationHelper.AddBlockAttributeValue( "A95B3556-75EC-4C56-89D4-0BE7A25B1F71", "9C95201E-64AC-4A36-8957-571C7BCA8F5B", @"#ffffff" );

            // Add Block Attribute Value
            //   Block: Hero
            //   BlockType: Hero
            //   Category: Mobile > Cms
            //   Block Location: Page=Prayer, Site=Kingdom First Solutions Mobile App
            //   Attribute: Height - Tablet
            /*   Attribute Value: 350 */
            RockMigrationHelper.AddBlockAttributeValue( "A95B3556-75EC-4C56-89D4-0BE7A25B1F71", "BF0B4848-9193-45EA-9A13-E0DE5573A64F", @"350" );

            // Add Block Attribute Value
            //   Block: Hero
            //   BlockType: Hero
            //   Category: Mobile > Cms
            //   Block Location: Page=Prayer, Site=Kingdom First Solutions Mobile App
            //   Attribute: Height - Phone
            /*   Attribute Value: 150 */
            RockMigrationHelper.AddBlockAttributeValue( "A95B3556-75EC-4C56-89D4-0BE7A25B1F71", "6527442E-539A-4486-B0C8-6D8C48F77FE8", @"150" );

            // Add Block Attribute Value
            //   Block: Hero
            //   BlockType: Hero
            //   Category: Mobile > Cms
            //   Block Location: Page=Prayer, Site=Kingdom First Solutions Mobile App
            //   Attribute: Text Align
            /*   Attribute Value: Center */
            RockMigrationHelper.AddBlockAttributeValue( "A95B3556-75EC-4C56-89D4-0BE7A25B1F71", "F3F7B68A-6926-4D89-BA28-0FB99E940A8E", @"Center" );

            // Add Block Attribute Value
            //   Block: Content
            //   BlockType: Content
            //   Category: Mobile > Cms
            //   Block Location: Page=Prayer, Site=Kingdom First Solutions Mobile App
            //   Attribute: Content
            /*   Attribute Value: ... */
            RockMigrationHelper.AddBlockAttributeValue( "C2B854DD-646A-4ED8-8C99-818F19521E70", "5682EDBF-68DA-4B43-A593-6C2B936C2839", @"<StackLayout StyleClass=""p-16"">
    <Label HorizontalTextAlignment=""Center"">
        <Label.Text>
Engage in community together by praying and encouraging each other through our Public Prayer Wall
Public Prayers will also be sent to the C3 Staff, Elders, and our C3 Prayer Team
        </Label.Text>
    </Label>
    <Button Text=""PUBLIC PRAYER"" Command=""{Binding PushPage}"" CommandParameter=""77f98e18-2e27-4d4a-b4cf-2ffa285a7014"" StyleClass=""btn,btn-primary"" HorizontalOptions=""Center"" />
    <Label HorizontalTextAlignment=""Center"" StyleClass=""mt-16"">
        <Label.Text>
Submit private prayers to be sent only to the C3 Staff, Elders, and our C3 Prayer Team
(For sensitive matters, please let us know in your request if you only want it sent to C3 Pastors)
        </Label.Text>
    </Label>
    <Button Text=""PRIVATE PRAYER"" Command=""{Binding PushPage}"" CommandParameter=""9a40b634-0e80-4207-bf0b-3050efd49516"" StyleClass=""btn,btn-primary"" HorizontalOptions=""Center"" />
</StackLayout>" );

            // Add Block Attribute Value
            //   Block: Content
            //   BlockType: Content
            //   Category: Mobile > Cms
            //   Block Location: Page=Prayer, Site=Kingdom First Solutions Mobile App
            //   Attribute: Dynamic Content
            /*   Attribute Value: False */
            RockMigrationHelper.AddBlockAttributeValue( "C2B854DD-646A-4ED8-8C99-818F19521E70", "B31D29A0-3725-4AEB-8360-7D91B9CDFE47", @"False" );

            // Add Block Attribute Value
            //   Block: Hero
            //   BlockType: Hero
            //   Category: Mobile > Cms
            //   Block Location: Page=Prayer - Prayer Wall, Site=Kingdom First Solutions Mobile App
            //   Attribute: Text Align
            /*   Attribute Value: Center */
            RockMigrationHelper.AddBlockAttributeValue( "FFA5E7C3-8086-4D35-B2CB-B0964C496F16", "F3F7B68A-6926-4D89-BA28-0FB99E940A8E", @"Center" );

            // Add Block Attribute Value
            //   Block: Hero
            //   BlockType: Hero
            //   Category: Mobile > Cms
            //   Block Location: Page=Prayer - Prayer Wall, Site=Kingdom First Solutions Mobile App
            //   Attribute: Height - Phone
            /*   Attribute Value: 150 */
            RockMigrationHelper.AddBlockAttributeValue( "FFA5E7C3-8086-4D35-B2CB-B0964C496F16", "6527442E-539A-4486-B0C8-6D8C48F77FE8", @"150" );

            // Add Block Attribute Value
            //   Block: Hero
            //   BlockType: Hero
            //   Category: Mobile > Cms
            //   Block Location: Page=Prayer - Prayer Wall, Site=Kingdom First Solutions Mobile App
            //   Attribute: Height - Tablet
            /*   Attribute Value: 350 */
            RockMigrationHelper.AddBlockAttributeValue( "FFA5E7C3-8086-4D35-B2CB-B0964C496F16", "BF0B4848-9193-45EA-9A13-E0DE5573A64F", @"350" );

            // Add Block Attribute Value
            //   Block: Hero
            //   BlockType: Hero
            //   Category: Mobile > Cms
            //   Block Location: Page=Prayer - Prayer Wall, Site=Kingdom First Solutions Mobile App
            //   Attribute: Padding
            /*   Attribute Value: 20 */
            RockMigrationHelper.AddBlockAttributeValue( "FFA5E7C3-8086-4D35-B2CB-B0964C496F16", "92DBA471-CFBC-4BC2-BD17-022663670328", @"20" );

            // Add Block Attribute Value
            //   Block: Hero
            //   BlockType: Hero
            //   Category: Mobile > Cms
            //   Block Location: Page=Prayer - Prayer Wall, Site=Kingdom First Solutions Mobile App
            //   Attribute: Title Color
            /*   Attribute Value: #ffffff */
            RockMigrationHelper.AddBlockAttributeValue( "FFA5E7C3-8086-4D35-B2CB-B0964C496F16", "9C95201E-64AC-4A36-8957-571C7BCA8F5B", @"#ffffff" );

            // Add Block Attribute Value
            //   Block: Hero
            //   BlockType: Hero
            //   Category: Mobile > Cms
            //   Block Location: Page=Prayer - Prayer Wall, Site=Kingdom First Solutions Mobile App
            //   Attribute: Subtitle Color
            /*   Attribute Value: #ffffff */
            RockMigrationHelper.AddBlockAttributeValue( "FFA5E7C3-8086-4D35-B2CB-B0964C496F16", "5374CF14-6A4F-4CA8-9561-0A1003F28504", @"#ffffff" );

            // Add Block Attribute Value
            //   Block: Hero
            //   BlockType: Hero
            //   Category: Mobile > Cms
            //   Block Location: Page=Prayer - Prayer Wall, Site=Kingdom First Solutions Mobile App
            //   Attribute: Background Image - Phone
            /*   Attribute Value: 3fba1290-9bdc-4cf2-a4f8-d1c2e1dcb90a */
            RockMigrationHelper.AddBlockAttributeValue( "FFA5E7C3-8086-4D35-B2CB-B0964C496F16", "D807B220-49A0-49EE-B4F0-F0A92ED3160F", @"3fba1290-9bdc-4cf2-a4f8-d1c2e1dcb90a" );

            // Add Block Attribute Value
            //   Block: Hero
            //   BlockType: Hero
            //   Category: Mobile > Cms
            //   Block Location: Page=Prayer - Prayer Wall, Site=Kingdom First Solutions Mobile App
            //   Attribute: Background Image - Tablet
            /*   Attribute Value: 50dc67de-d83e-4045-be20-6b364a65e4e7 */
            RockMigrationHelper.AddBlockAttributeValue( "FFA5E7C3-8086-4D35-B2CB-B0964C496F16", "B7A56FAF-90C6-4F79-898C-6F1FAEDF19B5", @"50dc67de-d83e-4045-be20-6b364a65e4e7" );

            // Add Block Attribute Value
            //   Block: Prayer Card View
            //   BlockType: Prayer Card View
            //   Category: Mobile > Prayer
            //   Block Location: Page=Prayer - Prayer Wall, Site=Kingdom First Solutions Mobile App
            //   Attribute: Template
            /*   Attribute Value: ... */
            RockMigrationHelper.AddBlockAttributeValue( "AABA341F-A6EF-45F9-B54D-362CDC892CA2", "A8B76BEB-0F6F-4F0B-968C-49CB3CEBB77F", @"ffffffff-ffff-ffff-ffff-ffffffffffff|<Rock:ResponsiveLayout>
{% for item in PrayerRequestItems %}
    <Rock:ResponsiveColumn Medium=""6"">
        <Frame StyleClass=""prayer-card-container"" HasShadow=""false"">
            <StackLayout>
                <Label Text=""{{ item.FirstName | Escape }} {{ item.LastName | Escape }}"" StyleClass=""prayer-card-name"" />

                <ContentView StyleClass=""prayer-card-category"" HorizontalOptions=""Start"">
                    <Label Text=""{{ item.Category.Name | Escape }}"" />
                </ContentView>

                <Label StyleClass=""prayer-card-text"">{{ item.Text | XamlWrap }}</Label>

                <Button x:Name=""PrayedBtn{{ forloop.index }}""
                    IsVisible=""false""
                    StyleClass=""btn,btn-primary,prayer-card-prayed-button""
                    HorizontalOptions=""End""
                    Text=""Prayed""
                    IsEnabled=""false"" />
                <Button x:Name=""PrayBtn{{ forloop.index }}""
                    StyleClass=""btn,btn-primary,prayer-card-pray-button""
                    HorizontalOptions=""End""
                    Text=""Pray""
                    Command=""{Binding AggregateCommand}"">
                    <Button.CommandParameter>
                        <Rock:AggregateCommandParameters>
                            <Rock:CommandReference Command=""{Binding PrayForRequest}""
                                CommandParameter=""{Rock:PrayForRequestParameters Guid={{ item.Guid }}, WorkflowTypeGuid='{{ PrayedWorkflowType }}'}"" />

                            <Rock:CommandReference Command=""{Binding SetViewProperty}""
                                CommandParameter=""{Rock:SetViewPropertyParameters View={x:Reference PrayedBtn{{ forloop.index }}}, Name=IsVisible, Value=true}"" />

                            <Rock:CommandReference Command=""{Binding SetViewProperty}""
                                CommandParameter=""{Rock:SetViewPropertyParameters View={x:Reference PrayBtn{{ forloop.index }}}, Name=IsVisible, Value=false}"" />
                        </Rock:AggregateCommandParameters>
                    </Button.CommandParameter>
                </Button>
            </StackLayout>
        </Frame>
    </Rock:ResponsiveColumn>
{% endfor %}
</Rock:ResponsiveLayout>" );

            // Add Block Attribute Value
            //   Block: Prayer Card View
            //   BlockType: Prayer Card View
            //   Category: Mobile > Prayer
            //   Block Location: Page=Prayer - Prayer Wall, Site=Kingdom First Solutions Mobile App
            //   Attribute: Title Content
            /*   Attribute Value: <Label StyleClass="h1" Text="Prayer Requests" /> */
            RockMigrationHelper.AddBlockAttributeValue( "AABA341F-A6EF-45F9-B54D-362CDC892CA2", "CD77FAA9-1852-407E-BE14-C8F92A1E7111", @"<Label StyleClass=""h1"" Text=""Prayer Requests"" />" );

            // Add Block Attribute Value
            //   Block: Prayer Card View
            //   BlockType: Prayer Card View
            //   Category: Mobile > Prayer
            //   Block Location: Page=Prayer - Prayer Wall, Site=Kingdom First Solutions Mobile App
            //   Attribute: Hide Campus When Known
            /*   Attribute Value: False */
            RockMigrationHelper.AddBlockAttributeValue( "AABA341F-A6EF-45F9-B54D-362CDC892CA2", "26CD1F8C-ABA4-48CC-812E-C43520C546B4", @"False" );

            // Add Block Attribute Value
            //   Block: Prayer Card View
            //   BlockType: Prayer Card View
            //   Category: Mobile > Prayer
            //   Block Location: Page=Prayer - Prayer Wall, Site=Kingdom First Solutions Mobile App
            //   Attribute: Category
            /*   Attribute Value: 5a94e584-35f0-4214-91f1-d72531cc6325 */
            RockMigrationHelper.AddBlockAttributeValue( "AABA341F-A6EF-45F9-B54D-362CDC892CA2", "4B794B17-84AB-4D83-9CBF-6ACB327297F2", @"5a94e584-35f0-4214-91f1-d72531cc6325" );

            // Add Block Attribute Value
            //   Block: Prayer Card View
            //   BlockType: Prayer Card View
            //   Category: Mobile > Prayer
            //   Block Location: Page=Prayer - Prayer Wall, Site=Kingdom First Solutions Mobile App
            //   Attribute: Public Only
            /*   Attribute Value: False */
            RockMigrationHelper.AddBlockAttributeValue( "AABA341F-A6EF-45F9-B54D-362CDC892CA2", "1BACC328-1913-4C1B-AA05-18CD56F357F8", @"False" );

            // Add Block Attribute Value
            //   Block: Prayer Card View
            //   BlockType: Prayer Card View
            //   Category: Mobile > Prayer
            //   Block Location: Page=Prayer - Prayer Wall, Site=Kingdom First Solutions Mobile App
            //   Attribute: Order
            /*   Attribute Value: 0 */
            RockMigrationHelper.AddBlockAttributeValue( "AABA341F-A6EF-45F9-B54D-362CDC892CA2", "6D362614-769B-4C0E-A305-B567FFE50F7E", @"0" );

            // Add Block Attribute Value
            //   Block: Prayer Card View
            //   BlockType: Prayer Card View
            //   Category: Mobile > Prayer
            //   Block Location: Page=Prayer - Prayer Wall, Site=Kingdom First Solutions Mobile App
            //   Attribute: Load Last Prayed Collection
            /*   Attribute Value: False */
            RockMigrationHelper.AddBlockAttributeValue( "AABA341F-A6EF-45F9-B54D-362CDC892CA2", "A39D70B2-2AF9-42B8-8C18-CC671E299CE0", @"False" );

            // Add Block Attribute Value
            //   Block: Prayer Card View
            //   BlockType: Prayer Card View
            //   Category: Mobile > Prayer
            //   Block Location: Page=Prayer - Prayer Wall, Site=Kingdom First Solutions Mobile App
            //   Attribute: Always Hide Campus
            /*   Attribute Value: True */
            RockMigrationHelper.AddBlockAttributeValue( "AABA341F-A6EF-45F9-B54D-362CDC892CA2", "54C2C92F-07F3-43B6-804A-47D7C9ACE4AA", @"True" );

            // Add Block Attribute Value
            //   Block: Prayer Card View
            //   BlockType: Prayer Card View
            //   Category: Mobile > Prayer
            //   Block Location: Page=Prayer - Prayer Wall, Site=Kingdom First Solutions Mobile App
            //   Attribute: Include Group Requests
            /*   Attribute Value: False */
            RockMigrationHelper.AddBlockAttributeValue( "AABA341F-A6EF-45F9-B54D-362CDC892CA2", "63A63454-1DAA-46F4-AB8C-16D454C25A32", @"False" );

            // Add Block Attribute Value
            //   Block: Content
            //   BlockType: Content
            //   Category: Mobile > Cms
            //   Block Location: Page=Prayer - Prayer Wall, Site=Kingdom First Solutions Mobile App
            //   Attribute: Dynamic Content
            /*   Attribute Value: False */
            RockMigrationHelper.AddBlockAttributeValue( "2A651146-7897-4E33-AB1E-70A53B38449B", "B31D29A0-3725-4AEB-8360-7D91B9CDFE47", @"False" );

            // Add Block Attribute Value
            //   Block: Content
            //   BlockType: Content
            //   Category: Mobile > Cms
            //   Block Location: Page=Prayer - Prayer Wall, Site=Kingdom First Solutions Mobile App
            //   Attribute: Content
            /*   Attribute Value: ... */
            RockMigrationHelper.AddBlockAttributeValue( "2A651146-7897-4E33-AB1E-70A53B38449B", "5682EDBF-68DA-4B43-A593-6C2B936C2839", @"<StackLayout StyleClass=""p-16"">
    <Label Text=""How can we pray for you? Share your prayer request (and read and pray for other requests) below."" StyleClass=""text-lg"" />
    <Label Text=""Please keep in mind that all prayers submitted here are public. For more private matters, for our staff or prayer team to pray for, please use the private request button."" />
    <StackLayout Orientation=""Horizontal"" StyleClass=""pt-16"">
        <Button Text=""Public Request"" HorizontalOptions=""StartAndExpand"" StyleClass=""mr-8,btn,btn-primary"" Command=""{Binding PushPage}"" CommandParameter=""2a94e058-189a-49cd-ac22-99677cbdb351"" />
        <Button Text=""Private Request"" HorizontalOptions=""EndAndExpand"" StyleClass=""ml-8,btn,btn-primary"" Command=""{Binding PushPage}"" CommandParameter=""9a40b634-0e80-4207-bf0b-3050efd49516"" />
    </StackLayout>
</StackLayout>" );

            // Add Block Attribute Value
            //   Block: Prayer Request Details
            //   BlockType: Prayer Request Details
            //   Category: Mobile > Prayer
            //   Block Location: Page=Prayer - Submit Prayer, Site=Kingdom First Solutions Mobile App
            //   Attribute: Character Limit
            /*   Attribute Value: 500 */
            RockMigrationHelper.AddBlockAttributeValue( "423C035E-9F3A-40F9-ABA6-F07474A375AF", "54273807-6E5F-47DF-A803-6251C6E2480A", @"500" );

            // Add Block Attribute Value
            //   Block: Prayer Request Details
            //   BlockType: Prayer Request Details
            //   Category: Mobile > Prayer
            //   Block Location: Page=Prayer - Submit Prayer, Site=Kingdom First Solutions Mobile App
            //   Attribute: Parent Category
            /*   Attribute Value: 5a94e584-35f0-4214-91f1-d72531cc6325 */
            RockMigrationHelper.AddBlockAttributeValue( "423C035E-9F3A-40F9-ABA6-F07474A375AF", "26EB9054-E675-4DD6-80EF-D5481F23CF51", @"5a94e584-35f0-4214-91f1-d72531cc6325" );

            // Add Block Attribute Value
            //   Block: Prayer Request Details
            //   BlockType: Prayer Request Details
            //   Category: Mobile > Prayer
            //   Block Location: Page=Prayer - Submit Prayer, Site=Kingdom First Solutions Mobile App
            //   Attribute: Default Category
            /*   Attribute Value: 4b2d88f5-6e45-4b4b-8776-11118c8e8269 */
            RockMigrationHelper.AddBlockAttributeValue( "423C035E-9F3A-40F9-ABA6-F07474A375AF", "80ABC48F-99E7-404F-AAB3-0EA8551010CF", @"4b2d88f5-6e45-4b4b-8776-11118c8e8269" );

            // Add Block Attribute Value
            //   Block: Prayer Request Details
            //   BlockType: Prayer Request Details
            //   Category: Mobile > Prayer
            //   Block Location: Page=Prayer - Submit Prayer, Site=Kingdom First Solutions Mobile App
            //   Attribute: Show Header
            /*   Attribute Value: True */
            RockMigrationHelper.AddBlockAttributeValue( "423C035E-9F3A-40F9-ABA6-F07474A375AF", "D847C303-4FF4-4A84-A1EB-4734DCA3F886", @"True" );

            // Add Block Attribute Value
            //   Block: Prayer Request Details
            //   BlockType: Prayer Request Details
            //   Category: Mobile > Prayer
            //   Block Location: Page=Prayer - Submit Prayer, Site=Kingdom First Solutions Mobile App
            //   Attribute: Completion Xaml
            /*   Attribute Value: ... */
            RockMigrationHelper.AddBlockAttributeValue( "423C035E-9F3A-40F9-ABA6-F07474A375AF", "949F7A76-8947-4457-B742-1AC1F2EC2486", @"<Rock:NotificationBox NotificationType=""Success"">
    Thank you for allowing us to pray for you.
</Rock:NotificationBox>" );

            // Add Block Attribute Value
            //   Block: Prayer Request Details
            //   BlockType: Prayer Request Details
            //   Category: Mobile > Prayer
            //   Block Location: Page=Prayer - Submit Prayer, Site=Kingdom First Solutions Mobile App
            //   Attribute: Expires After (Days)
            /*   Attribute Value: 14 */
            RockMigrationHelper.AddBlockAttributeValue( "423C035E-9F3A-40F9-ABA6-F07474A375AF", "4389C4A7-CF8D-4750-812A-F4C2BD602E96", @"14" );

            // Add Block Attribute Value
            //   Block: Prayer Request Details
            //   BlockType: Prayer Request Details
            //   Category: Mobile > Prayer
            //   Block Location: Page=Prayer - Submit Prayer, Site=Kingdom First Solutions Mobile App
            //   Attribute: Completion Action
            /*   Attribute Value: 0 */
            RockMigrationHelper.AddBlockAttributeValue( "423C035E-9F3A-40F9-ABA6-F07474A375AF", "22BCE7BA-ED45-480B-9833-03A503B825ED", @"0" );

            // Add Block Attribute Value
            //   Block: Prayer Request Details
            //   BlockType: Prayer Request Details
            //   Category: Mobile > Prayer
            //   Block Location: Page=Prayer - Submit Prayer, Site=Kingdom First Solutions Mobile App
            //   Attribute: Require Last Name
            /*   Attribute Value: True */
            RockMigrationHelper.AddBlockAttributeValue( "423C035E-9F3A-40F9-ABA6-F07474A375AF", "E5681E4D-7CD7-475F-9F74-7E0C295BE538", @"True" );

            // Add Block Attribute Value
            //   Block: Prayer Request Details
            //   BlockType: Prayer Request Details
            //   Category: Mobile > Prayer
            //   Block Location: Page=Prayer - Submit Prayer, Site=Kingdom First Solutions Mobile App
            //   Attribute: Enable Person Matching
            /*   Attribute Value: True */
            RockMigrationHelper.AddBlockAttributeValue( "423C035E-9F3A-40F9-ABA6-F07474A375AF", "238CFE61-56D7-47E2-AC5D-66B3B645D3FE", @"True" );

            // Add Block Attribute Value
            //   Block: Prayer Request Details
            //   BlockType: Prayer Request Details
            //   Category: Mobile > Prayer
            //   Block Location: Page=Prayer - Submit Prayer, Site=Kingdom First Solutions Mobile App
            //   Attribute: Show Category
            /*   Attribute Value: True */
            RockMigrationHelper.AddBlockAttributeValue( "423C035E-9F3A-40F9-ABA6-F07474A375AF", "1D63D8F5-A810-477D-B9F7-24B33546990F", @"True" );

            // Add Block Attribute Value
            //   Block: Prayer Request Details
            //   BlockType: Prayer Request Details
            //   Category: Mobile > Prayer
            //   Block Location: Page=Prayer - Submit Prayer, Site=Kingdom First Solutions Mobile App
            //   Attribute: Enable Auto Approve
            /*   Attribute Value: True */
            RockMigrationHelper.AddBlockAttributeValue( "423C035E-9F3A-40F9-ABA6-F07474A375AF", "42F19547-E84A-4CD8-9787-7A74255E73E4", @"True" );

            // Add Block Attribute Value
            //   Block: Prayer Request Details
            //   BlockType: Prayer Request Details
            //   Category: Mobile > Prayer
            //   Block Location: Page=Prayer - Submit Prayer, Site=Kingdom First Solutions Mobile App
            //   Attribute: Show Urgent Flag
            /*   Attribute Value: False */
            RockMigrationHelper.AddBlockAttributeValue( "423C035E-9F3A-40F9-ABA6-F07474A375AF", "50F41A0F-858E-46D9-AF3B-506E8AB2F9E3", @"False" );

            // Add Block Attribute Value
            //   Block: Prayer Request Details
            //   BlockType: Prayer Request Details
            //   Category: Mobile > Prayer
            //   Block Location: Page=Prayer - Submit Prayer, Site=Kingdom First Solutions Mobile App
            //   Attribute: Show Public Display Flag
            /*   Attribute Value: True */
            RockMigrationHelper.AddBlockAttributeValue( "423C035E-9F3A-40F9-ABA6-F07474A375AF", "0C0898A4-0554-420E-BA3D-BD22AB8C44CA", @"True" );

            // Add Block Attribute Value
            //   Block: Prayer Request Details
            //   BlockType: Prayer Request Details
            //   Category: Mobile > Prayer
            //   Block Location: Page=Prayer - Submit Prayer, Site=Kingdom First Solutions Mobile App
            //   Attribute: Default To Public
            /*   Attribute Value: True */
            RockMigrationHelper.AddBlockAttributeValue( "423C035E-9F3A-40F9-ABA6-F07474A375AF", "28332C3D-E2B2-4D6D-B8B3-9BE11FF07504", @"True" );

            // Add Block Attribute Value
            //   Block: Prayer Request Details
            //   BlockType: Prayer Request Details
            //   Category: Mobile > Prayer
            //   Block Location: Page=Prayer - Submit Prayer, Site=Kingdom First Solutions Mobile App
            //   Attribute: Show Campus
            /*   Attribute Value: True */
            RockMigrationHelper.AddBlockAttributeValue( "423C035E-9F3A-40F9-ABA6-F07474A375AF", "C91F9877-5605-48F8-B6CA-A797F998826D", @"True" );

            // Add Block Attribute Value
            //   Block: Prayer Request Details
            //   BlockType: Prayer Request Details
            //   Category: Mobile > Prayer
            //   Block Location: Page=Prayer - Submit Prayer, Site=Kingdom First Solutions Mobile App
            //   Attribute: Require Campus
            /*   Attribute Value: False */
            RockMigrationHelper.AddBlockAttributeValue( "423C035E-9F3A-40F9-ABA6-F07474A375AF", "92258CC6-9E8E-4DF3-AB5B-48114BE28BA7", @"False" );

            // Add Block Attribute Value
            //   Block: Prayer Request Details
            //   BlockType: Prayer Request Details
            //   Category: Mobile > Prayer
            //   Block Location: Page=Prayer - Private Entry, Site=Kingdom First Solutions Mobile App
            //   Attribute: Require Campus
            /*   Attribute Value: False */
            RockMigrationHelper.AddBlockAttributeValue( "F89758C2-F1B2-4E21-8539-6106C44E76CF", "92258CC6-9E8E-4DF3-AB5B-48114BE28BA7", @"False" );

            // Add Block Attribute Value
            //   Block: Prayer Request Details
            //   BlockType: Prayer Request Details
            //   Category: Mobile > Prayer
            //   Block Location: Page=Prayer - Private Entry, Site=Kingdom First Solutions Mobile App
            //   Attribute: Require Last Name
            /*   Attribute Value: True */
            RockMigrationHelper.AddBlockAttributeValue( "F89758C2-F1B2-4E21-8539-6106C44E76CF", "E5681E4D-7CD7-475F-9F74-7E0C295BE538", @"True" );

            // Add Block Attribute Value
            //   Block: Prayer Request Details
            //   BlockType: Prayer Request Details
            //   Category: Mobile > Prayer
            //   Block Location: Page=Prayer - Private Entry, Site=Kingdom First Solutions Mobile App
            //   Attribute: Default To Public
            /*   Attribute Value: False */
            RockMigrationHelper.AddBlockAttributeValue( "F89758C2-F1B2-4E21-8539-6106C44E76CF", "28332C3D-E2B2-4D6D-B8B3-9BE11FF07504", @"False" );

            // Add Block Attribute Value
            //   Block: Prayer Request Details
            //   BlockType: Prayer Request Details
            //   Category: Mobile > Prayer
            //   Block Location: Page=Prayer - Private Entry, Site=Kingdom First Solutions Mobile App
            //   Attribute: Show Campus
            /*   Attribute Value: False */
            RockMigrationHelper.AddBlockAttributeValue( "F89758C2-F1B2-4E21-8539-6106C44E76CF", "C91F9877-5605-48F8-B6CA-A797F998826D", @"False" );

            // Add Block Attribute Value
            //   Block: Prayer Request Details
            //   BlockType: Prayer Request Details
            //   Category: Mobile > Prayer
            //   Block Location: Page=Prayer - Private Entry, Site=Kingdom First Solutions Mobile App
            //   Attribute: Show Urgent Flag
            /*   Attribute Value: False */
            RockMigrationHelper.AddBlockAttributeValue( "F89758C2-F1B2-4E21-8539-6106C44E76CF", "50F41A0F-858E-46D9-AF3B-506E8AB2F9E3", @"False" );

            // Add Block Attribute Value
            //   Block: Prayer Request Details
            //   BlockType: Prayer Request Details
            //   Category: Mobile > Prayer
            //   Block Location: Page=Prayer - Private Entry, Site=Kingdom First Solutions Mobile App
            //   Attribute: Show Public Display Flag
            /*   Attribute Value: False */
            RockMigrationHelper.AddBlockAttributeValue( "F89758C2-F1B2-4E21-8539-6106C44E76CF", "0C0898A4-0554-420E-BA3D-BD22AB8C44CA", @"False" );

            // Add Block Attribute Value
            //   Block: Prayer Request Details
            //   BlockType: Prayer Request Details
            //   Category: Mobile > Prayer
            //   Block Location: Page=Prayer - Private Entry, Site=Kingdom First Solutions Mobile App
            //   Attribute: Show Category
            /*   Attribute Value: True */
            RockMigrationHelper.AddBlockAttributeValue( "F89758C2-F1B2-4E21-8539-6106C44E76CF", "1D63D8F5-A810-477D-B9F7-24B33546990F", @"True" );

            // Add Block Attribute Value
            //   Block: Prayer Request Details
            //   BlockType: Prayer Request Details
            //   Category: Mobile > Prayer
            //   Block Location: Page=Prayer - Private Entry, Site=Kingdom First Solutions Mobile App
            //   Attribute: Enable Auto Approve
            /*   Attribute Value: False */
            RockMigrationHelper.AddBlockAttributeValue( "F89758C2-F1B2-4E21-8539-6106C44E76CF", "42F19547-E84A-4CD8-9787-7A74255E73E4", @"False" );

            // Add Block Attribute Value
            //   Block: Prayer Request Details
            //   BlockType: Prayer Request Details
            //   Category: Mobile > Prayer
            //   Block Location: Page=Prayer - Private Entry, Site=Kingdom First Solutions Mobile App
            //   Attribute: Enable Person Matching
            /*   Attribute Value: True */
            RockMigrationHelper.AddBlockAttributeValue( "F89758C2-F1B2-4E21-8539-6106C44E76CF", "238CFE61-56D7-47E2-AC5D-66B3B645D3FE", @"True" );

            // Add Block Attribute Value
            //   Block: Prayer Request Details
            //   BlockType: Prayer Request Details
            //   Category: Mobile > Prayer
            //   Block Location: Page=Prayer - Private Entry, Site=Kingdom First Solutions Mobile App
            //   Attribute: Completion Action
            /*   Attribute Value: 0 */
            RockMigrationHelper.AddBlockAttributeValue( "F89758C2-F1B2-4E21-8539-6106C44E76CF", "22BCE7BA-ED45-480B-9833-03A503B825ED", @"0" );

            // Add Block Attribute Value
            //   Block: Prayer Request Details
            //   BlockType: Prayer Request Details
            //   Category: Mobile > Prayer
            //   Block Location: Page=Prayer - Private Entry, Site=Kingdom First Solutions Mobile App
            //   Attribute: Expires After (Days)
            /*   Attribute Value: 14 */
            RockMigrationHelper.AddBlockAttributeValue( "F89758C2-F1B2-4E21-8539-6106C44E76CF", "4389C4A7-CF8D-4750-812A-F4C2BD602E96", @"14" );

            // Add Block Attribute Value
            //   Block: Prayer Request Details
            //   BlockType: Prayer Request Details
            //   Category: Mobile > Prayer
            //   Block Location: Page=Prayer - Private Entry, Site=Kingdom First Solutions Mobile App
            //   Attribute: Parent Category
            /*   Attribute Value: 5a94e584-35f0-4214-91f1-d72531cc6325 */
            RockMigrationHelper.AddBlockAttributeValue( "F89758C2-F1B2-4E21-8539-6106C44E76CF", "26EB9054-E675-4DD6-80EF-D5481F23CF51", @"5a94e584-35f0-4214-91f1-d72531cc6325" );

            // Add Block Attribute Value
            //   Block: Prayer Request Details
            //   BlockType: Prayer Request Details
            //   Category: Mobile > Prayer
            //   Block Location: Page=Prayer - Private Entry, Site=Kingdom First Solutions Mobile App
            //   Attribute: Show Header
            /*   Attribute Value: False */
            RockMigrationHelper.AddBlockAttributeValue( "F89758C2-F1B2-4E21-8539-6106C44E76CF", "D847C303-4FF4-4A84-A1EB-4734DCA3F886", @"False" );

            // Add Block Attribute Value
            //   Block: Prayer Request Details
            //   BlockType: Prayer Request Details
            //   Category: Mobile > Prayer
            //   Block Location: Page=Prayer - Private Entry, Site=Kingdom First Solutions Mobile App
            //   Attribute: Default Category
            /*   Attribute Value: 4b2d88f5-6e45-4b4b-8776-11118c8e8269 */
            RockMigrationHelper.AddBlockAttributeValue( "F89758C2-F1B2-4E21-8539-6106C44E76CF", "80ABC48F-99E7-404F-AAB3-0EA8551010CF", @"4b2d88f5-6e45-4b4b-8776-11118c8e8269" );

            // Add Block Attribute Value
            //   Block: Prayer Request Details
            //   BlockType: Prayer Request Details
            //   Category: Mobile > Prayer
            //   Block Location: Page=Prayer - Private Entry, Site=Kingdom First Solutions Mobile App
            //   Attribute: Completion Xaml
            /*   Attribute Value: ... */
            RockMigrationHelper.AddBlockAttributeValue( "F89758C2-F1B2-4E21-8539-6106C44E76CF", "949F7A76-8947-4457-B742-1AC1F2EC2486", @"<Rock:NotificationBox NotificationType=""Success"">
    Thank you for allowing us to pray for you.
</Rock:NotificationBox>" );

            // Add Block Attribute Value
            //   Block: Content
            //   BlockType: Content
            //   Category: Mobile > Cms
            //   Block Location: Page=Prayer - Private Entry, Site=Kingdom First Solutions Mobile App
            //   Attribute: Content
            /*   Attribute Value: ... */
            RockMigrationHelper.AddBlockAttributeValue( "7CA630C6-4B1B-4D4E-9088-A4572EFE73DD", "5682EDBF-68DA-4B43-A593-6C2B936C2839", @"<Rock:Html>
    <![CDATA[
    <h1>We Believe in the Power of Prayer</h1>
    <p>Our Prayer Team is composed of people who have a passion for prayer and pray diligently, specifically for the needs within and outside the church, locally and globally.</p>
    <p></p>
    <h1>Need Prayer?</h1>
    <p>Let us pray for you. Just fill out this form and let us know your needs. (Please let us know in your request if you only want this sent to C3 Pastors.)</p>
    ]]>
</Rock:Html>" );

            // Add Block Attribute Value
            //   Block: Content
            //   BlockType: Content
            //   Category: Mobile > Cms
            //   Block Location: Page=Prayer - Private Entry, Site=Kingdom First Solutions Mobile App
            //   Attribute: Dynamic Content
            /*   Attribute Value: False */
            RockMigrationHelper.AddBlockAttributeValue( "7CA630C6-4B1B-4D4E-9088-A4572EFE73DD", "B31D29A0-3725-4AEB-8360-7D91B9CDFE47", @"False" );

            // Add Block Attribute Value
            //   Block: Prayer Session Setup
            //   BlockType: Prayer Session Setup
            //   Category: Mobile > Prayer
            //   Block Location: Page=Prayer, Site=Kingdom First Solutions Mobile App
            //   Attribute: Parent Category
            /*   Attribute Value: 5a94e584-35f0-4214-91f1-d72531cc6325 */
            RockMigrationHelper.AddBlockAttributeValue( "AE875F67-394C-4BED-8C2D-8937F713F5BA", "E177F02C-B1BF-4139-B354-E3C4EFFF9A0B", @"5a94e584-35f0-4214-91f1-d72531cc6325" );

            // Add Block Attribute Value
            //   Block: Prayer Session Setup
            //   BlockType: Prayer Session Setup
            //   Category: Mobile > Prayer
            //   Block Location: Page=Prayer, Site=Kingdom First Solutions Mobile App
            //   Attribute: Prayer Page
            /*   Attribute Value: 68eaf2d8-688c-44c3-9222-340047e07095 */
            RockMigrationHelper.AddBlockAttributeValue( "AE875F67-394C-4BED-8C2D-8937F713F5BA", "6EEED035-2E74-44EF-8567-016C2CBC3E6D", @"68eaf2d8-688c-44c3-9222-340047e07095" );

            // Add Block Attribute Value
            //   Block: Prayer Session Setup
            //   BlockType: Prayer Session Setup
            //   Category: Mobile > Prayer
            //   Block Location: Page=Prayer, Site=Kingdom First Solutions Mobile App
            //   Attribute: Title Text
            /*   Attribute Value: Prayer Session */
            RockMigrationHelper.AddBlockAttributeValue( "AE875F67-394C-4BED-8C2D-8937F713F5BA", "3D65A824-5C8C-4580-9E9B-5224A03D82FD", @"Prayer Session" );

            // Add Block Attribute Value
            //   Block: Prayer Session Setup
            //   BlockType: Prayer Session Setup
            //   Category: Mobile > Prayer
            //   Block Location: Page=Prayer, Site=Kingdom First Solutions Mobile App
            //   Attribute: Instruction Text
            /*   Attribute Value: Select your categories to start a prayer session. */
            RockMigrationHelper.AddBlockAttributeValue( "AE875F67-394C-4BED-8C2D-8937F713F5BA", "010BF18D-FA2E-4110-B393-96FBB6DFF02F", @"Select your categories to start a prayer session." );

            // Add Block Attribute Value
            //   Block: Prayer Session Setup
            //   BlockType: Prayer Session Setup
            //   Category: Mobile > Prayer
            //   Block Location: Page=Prayer, Site=Kingdom First Solutions Mobile App
            //   Attribute: Show Campus Filter
            /*   Attribute Value: False */
            RockMigrationHelper.AddBlockAttributeValue( "AE875F67-394C-4BED-8C2D-8937F713F5BA", "B0B72E6D-6D4B-4C29-A3B4-2B01A8E756B1", @"False" );

            // Add Block Attribute Value
            //   Block: Prayer Session
            //   BlockType: Prayer Session
            //   Category: Mobile > Prayer
            //   Block Location: Page=Prayer Session, Site=Kingdom First Solutions Mobile App
            //   Attribute: Prayed Button Text
            /*   Attribute Value: I've Prayed */
            RockMigrationHelper.AddBlockAttributeValue( "73A98506-DBBA-408E-8053-F022C6C0B5B9", "7D3AE194-069E-4CE8-97C9-EB7A9CADA7B9", @"I've Prayed" );

            // Add Block Attribute Value
            //   Block: Prayer Session
            //   BlockType: Prayer Session
            //   Category: Mobile > Prayer
            //   Block Location: Page=Prayer Session, Site=Kingdom First Solutions Mobile App
            //   Attribute: Show Follow Button
            /*   Attribute Value: True */
            RockMigrationHelper.AddBlockAttributeValue( "73A98506-DBBA-408E-8053-F022C6C0B5B9", "2E37A96E-9BF5-484A-8146-6E54629080CC", @"True" );

            // Add Block Attribute Value
            //   Block: Prayer Session
            //   BlockType: Prayer Session
            //   Category: Mobile > Prayer
            //   Block Location: Page=Prayer Session, Site=Kingdom First Solutions Mobile App
            //   Attribute: Show Inappropriate Button
            /*   Attribute Value: True */
            RockMigrationHelper.AddBlockAttributeValue( "73A98506-DBBA-408E-8053-F022C6C0B5B9", "52E1A831-4039-4185-8261-B39083B39C46", @"True" );

            // Add Block Attribute Value
            //   Block: Prayer Session
            //   BlockType: Prayer Session
            //   Category: Mobile > Prayer
            //   Block Location: Page=Prayer Session, Site=Kingdom First Solutions Mobile App
            //   Attribute: Public Only
            /*   Attribute Value: False */
            RockMigrationHelper.AddBlockAttributeValue( "73A98506-DBBA-408E-8053-F022C6C0B5B9", "B10629F9-DCBC-4601-A717-89B34F7ED5DE", @"False" );

            // Add Block Attribute Value
            //   Block: Prayer Session
            //   BlockType: Prayer Session
            //   Category: Mobile > Prayer
            //   Block Location: Page=Prayer Session, Site=Kingdom First Solutions Mobile App
            //   Attribute: Template
            /*   Attribute Value: 2b0f4548-8da7-4236-9bf9-5fa3c07d762f| */
            RockMigrationHelper.AddBlockAttributeValue( "73A98506-DBBA-408E-8053-F022C6C0B5B9", "AF310A62-C62A-427A-A88E-48AA885CCCE7", @"2b0f4548-8da7-4236-9bf9-5fa3c07d762f|" );

            // Add Block Attribute Value
            //   Block: Prayer Session
            //   BlockType: Prayer Session
            //   Category: Mobile > Prayer
            //   Block Location: Page=Prayer Session, Site=Kingdom First Solutions Mobile App
            //   Attribute: Include Group Requests
            /*   Attribute Value: True */
            RockMigrationHelper.AddBlockAttributeValue( "73A98506-DBBA-408E-8053-F022C6C0B5B9", "69DF4D6F-730B-4F16-BF47-7FFEAFC2EB2D", @"True" );

            // Add Block Attribute Value
            //   Block: Prayer Session
            //   BlockType: Prayer Session
            //   Category: Mobile > Prayer
            //   Block Location: Page=Prayer Session, Site=Kingdom First Solutions Mobile App
            //   Attribute: Create Interactions for Prayers
            /*   Attribute Value: True */
            RockMigrationHelper.AddBlockAttributeValue( "73A98506-DBBA-408E-8053-F022C6C0B5B9", "3382895E-1422-417E-84F2-A4750AF2AD70", @"True" );

            // Add Block Attribute Value
            //   Block: Communication List Subscribe
            //   BlockType: Communication List Subscribe
            //   Category: Mobile > Communication
            //   Block Location: Page=Communication Preferences, Site=Kingdom First Solutions Mobile App
            //   Attribute: Communication List Categories
            /*   Attribute Value: a0889e77-67d9-418c-b301-1b3924692058 */
            RockMigrationHelper.AddBlockAttributeValue( "875449F2-B0DF-449D-837D-169DB71D038C", "1E983A67-A9DC-4F91-8862-3E55613A95DC", @"a0889e77-67d9-418c-b301-1b3924692058" );

            // Add Block Attribute Value
            //   Block: Communication List Subscribe
            //   BlockType: Communication List Subscribe
            //   Category: Mobile > Communication
            //   Block Location: Page=Communication Preferences, Site=Kingdom First Solutions Mobile App
            //   Attribute: Show Description
            /*   Attribute Value: False */
            RockMigrationHelper.AddBlockAttributeValue( "875449F2-B0DF-449D-837D-169DB71D038C", "E12CE061-2475-41A2-BDB7-12F03853B18B", @"False" );

            // Add Block Attribute Value
            //   Block: Content
            //   BlockType: Content
            //   Category: Mobile > Cms
            //   Block Location: Page=C3 Home - Coming Up, Site=Kingdom First Solutions Mobile App
            //   Attribute: Dynamic Content
            /*   Attribute Value: True */
            RockMigrationHelper.AddBlockAttributeValue( "50629D11-CA39-4B1B-B0DD-307CC9CC17AF", "B31D29A0-3725-4AEB-8360-7D91B9CDFE47", @"True" );

            // Add Block Attribute Value
            //   Block: Content
            //   BlockType: Content
            //   Category: Mobile > Cms
            //   Block Location: Page=C3 Home - Coming Up, Site=Kingdom First Solutions Mobile App
            //   Attribute: Enabled Lava Commands
            /*   Attribute Value: RockEntity */
            RockMigrationHelper.AddBlockAttributeValue( "50629D11-CA39-4B1B-B0DD-307CC9CC17AF", "24516448-3F1F-4F27-97A1-CFB4F8B277B5", @"RockEntity" );

            // Add Block Attribute Value
            //   Block: Content
            //   BlockType: Content
            //   Category: Mobile > Cms
            //   Block Location: Page=C3 Home - Coming Up, Site=Kingdom First Solutions Mobile App
            //   Attribute: Content
            /*   Attribute Value: ... */
            RockMigrationHelper.AddBlockAttributeValue( "50629D11-CA39-4B1B-B0DD-307CC9CC17AF", "5682EDBF-68DA-4B43-A593-6C2B936C2839", @"<StackLayout Spacing=""0"">{%- contentchannelitem where:'ContentChannelId == ""8"" && StartDateTime < ""{{ 'Now' | Date }}"" && ExpireDateTime > ""{{ 'Now' | Date }}"" || ExpireDateTime _= """" && ContentChannelId == ""8"" && StartDateTime < ""{{ 'Now' | Date }}""' sort:'Order' -%}
{%- for item in contentchannelitemItems -%}
{%- assign linkUrlType = item | Attribute:""LinktoURLType"" -%}
{%- assign linkToUrl = item | Attribute:""LinktoURL"" -%}{% assign authenticatedUser = item | Attribute:'PassUserAuthentication' | AsBoolean %}
{%- assign appPage = item | Attribute:""LinktoAppPage"",""RawValue"" -%}{% assign contentStripped = item.Content | StripHtml | Trim %} 
{%- capture command -%}
{%- if linkUrlType == 'External Browser' and appPage == '' -%}
{Binding OpenExternalBrowser}{%- elseif linkUrlType == 'Internal Browser' and appPage == '' -%}
{Binding OpenBrowser}{%- elseif linkToURL != '' or appPage != '' or contentStripped != '' -%}
{Binding PushPage}{%- endif -%}{%- endcapture -%}
{%- if authenticatedUser %}{%- capture personToken %}{% if linkToUrl contains '?' %}&{% else %}?{% endif %}rckipid={{ CurrentPerson | PersonTokenCreate }}{% endcapture -%}{% endif -%}
{%- capture linkUrl -%}
{%- if linkToUrl != '' and linkUrlType == 'Webview' -%}
743cf21f-c3c1-4af1-8552-97a62a2b609b?url={{ linkToUrl | Append:personToken | UrlEncode -}}
{%- elseif linkToUrl != '' -%}
{{- linkToUrl | Append:personToken | Escape -}}
{%- elseif appPage != '' -%}
{{- appPage -}}
{%- elseif contentStripped != '' %}
fdf02b2c-f252-4f17-a947-c34957f1e2cc?Item={{ item.Id -}}
{%- endif -%}
{%- endcapture -%}
{% assign image = item | Attribute:'Image','Url' %}{% assign imageUrl = item | Attribute:'ImageUrl' %}{% assign subtitle = item | Attribute:'Subtitle' %}{% assign cardWidth = item | Attribute:'CardWidth' %}{% assign elevation = item | Attribute:'ShadowDepth' %}{% assign showDetailsButton = item | Attribute:'DisplayDetailsButton' | AsBoolean %}{% assign showTitle = item | Attribute:'ShowTitle' | AsBoolean %}{% assign tagline = item | Attribute:'Tagline' %}
{%- if elevation == 0 or showTitle == false and tagline == """" and subtitle == """" -%}
    {%- if cardWidth == ""Full"" %}{%- assign cardTagUsed = ""InlineCard"" -%}{%- assign yStyle = ""mt-0,mb-8"" -%}{% else %}{% assign cardTagUsed = ""BlockCard"" %}{% assign Style = ""my-0"" %}{% endif %}
{%- else -%}
    {%- assign cardTagUsed = ""ContainedCard"" -%}{%- assign yStyle = ""my-8"" -%}
{%- endif -%}
<Rock:{{ cardTagUsed }} Image=""{% if imageUrl != '' %}{{ imageUrl | Escape }}{% elseif image != '' %}{{ image | Escape }}{% endif %}""
        {% if tagline != """" %}Tagline=""{{ tagline | Escape }}""{% endif %}
        {% if showTitle %}Title=""{{ item.Title | Escape }}""{% endif %}
        Command=""{{ command }}""
        CommandParameter=""{{ linkUrl }}""
        Elevation=""{{ elevation }}""
        {% if cardTagUsed == 'InlineCard' %}CardRatio=""9:16""{% else %}ImageRatio=""{{ item | Attribute:'ImageAspectRatio' }}""{% endif %}
        {% if cardWidth == 'Full' %}CornerRadius=""0"" StyleClass=""{{ yStyle }}""{% else %}StyleClass=""mx-8,{{ yStyle }}""{% endif %}>
        {%- if subtitle != """" -%}
            <Rock:{{ cardTagUsed }}.DescriptionLeft>
                <Label Text=""{{ item | Attribute:'Subtitle' | Escape }}"" StyleClass=""text-gray-500"" />
            </Rock:{{ cardTagUsed }}.DescriptionLeft>{%- endif -%}
       {%- if showDetailsButton %}
       <StackLayout>
           <Label Text=""{{ item.Content | StripHtml | TruncateWords:50 | Escape }}"" StyleClass=""my-16"" />
           <Button Text=""SEE DETAILS"" StyleClass=""btn,btn-primary"" HorizontalOptions=""Center"" Command=""{{ command }}"" CommandParameter=""{{ linkUrl }}"" />
        </StackLayout>{% else %}{{ item.Content | StripHtml | TruncateWords:50 | Escape }}{% endif %}
    </Rock:{{ cardTagUsed }}>
{% endfor %}
{% endcontentchannelitem %}
</StackLayout>" );

            // Add Block Attribute Value
            //   Block: Content
            //   BlockType: Content
            //   Category: Mobile > Cms
            //   Block Location: Page=C3 Home - Ministries, Site=Kingdom First Solutions Mobile App
            //   Attribute: Content
            /*   Attribute Value: ... */
            RockMigrationHelper.AddBlockAttributeValue( "C25B8D3F-2435-49FA-843B-C802678CE91D", "5682EDBF-68DA-4B43-A593-6C2B936C2839", @"<StackLayout>
    <Rock:Image Source=""https://rock.cartervillechristian.com/Content/C3App/AppImages/Ministries/NewAppNavigation-12.jpg"" Command=""{Binding PushPage}"" CommandParameter=""743cf21f-c3c1-4af1-8552-97a62a2b609b?url={{ 'https://www.cartervillechristian.com/c3kids' | UrlEncode }}"" />
    <Rock:Image Source=""https://rock.cartervillechristian.com/Content/C3App/AppImages/Ministries/NewAppNavigation-13.jpg"" Command=""{Binding PushPage}"" CommandParameter=""743cf21f-c3c1-4af1-8552-97a62a2b609b?url={{ 'https://www.cartervillechristian.com/c3youth' | UrlEncode }}"" />
    <Rock:Image Source=""https://rock.cartervillechristian.com/Content/C3App/AppImages/Ministries/NewAppNavigation-14.jpg"" Command=""{Binding PushPage}"" CommandParameter=""743cf21f-c3c1-4af1-8552-97a62a2b609b?url={{ 'https://www.cartervillechristian.com/c3adults' | UrlEncode }}"" />
    <Rock:Image Source=""https://rock.cartervillechristian.com/Content/C3App/AppImages/Ministries/NewAppNavigation-15.jpg"" Command=""{Binding PushPage}"" CommandParameter=""743cf21f-c3c1-4af1-8552-97a62a2b609b?url={{ 'https://www.cartervillechristian.com/outreach' | UrlEncode }}"" />
</StackLayout>" );

            // Add Block Attribute Value
            //   Block: Content
            //   BlockType: Content
            //   Category: Mobile > Cms
            //   Block Location: Page=C3 Home - Ministries, Site=Kingdom First Solutions Mobile App
            //   Attribute: Dynamic Content
            /*   Attribute Value: False */
            RockMigrationHelper.AddBlockAttributeValue( "C25B8D3F-2435-49FA-843B-C802678CE91D", "B31D29A0-3725-4AEB-8360-7D91B9CDFE47", @"False" );

            // Add Block Attribute Value
            //   Block: Content
            //   BlockType: Content
            //   Category: Mobile > Cms
            //   Block Location: Page=C3 Home - Welcome, Site=Kingdom First Solutions Mobile App
            //   Attribute: Dynamic Content
            /*   Attribute Value: False */
            RockMigrationHelper.AddBlockAttributeValue( "8D4A6291-2B50-40E9-B446-D35A5D510EEF", "B31D29A0-3725-4AEB-8360-7D91B9CDFE47", @"False" );

            // Add Block Attribute Value
            //   Block: Content
            //   BlockType: Content
            //   Category: Mobile > Cms
            //   Block Location: Page=C3 Home - Welcome, Site=Kingdom First Solutions Mobile App
            //   Attribute: Content
            /*   Attribute Value: ... */
            RockMigrationHelper.AddBlockAttributeValue( "8D4A6291-2B50-40E9-B446-D35A5D510EEF", "5682EDBF-68DA-4B43-A593-6C2B936C2839", @"<StackLayout StyleClass=""p-16"">
    <Label StyleClass=""h1,mb-24"" Text=""WELCOME"" />
    <Rock:Html>
        <![CDATA[
            <p><strong>What You'll Find in the App</strong></p>
            <ul>
                <li>Upcoming events and activities at C3</li>
                <li>Ability to give and watch the latest message</li>
                <li>C3 location, service times, and ministry details</li>
            </ul>
            <p></p>
            <p><strong>Understanding Notifications</strong></p>
            <ul>
                <li>A notification is a message sent to all app users who have opted into receive notifications.</li>
                <li>Be notified about an important announcement, upcoming event or adjusted/canceled activities due to adverse weather.</li>
                <li>Filter your notifications... Choose your categories: Use C3 Church for all church announcement/cancellations and Reading Plan to Receive a daily notification for our C3 Reading Plan when available.</li>
                <li>Go to your personal profile on the C3 App at any time to view notifications or update your notification category settings within the app.</li>
                <li>You may also need to adjust your phone settings to receive/turn off push notifications for the C3 App based on your preference.</li>
            </ul>
            <p></p>
            <p><strong>We're so glad you're a part of the C3 Family! Stay updated and connected with the C3 App!</strong></p>
        ]]>
    </Rock:Html>
</StackLayout>" );

            // Add Block Attribute Value
            //   Block: Content
            //   BlockType: Content
            //   Category: Mobile > Cms
            //   Block Location: Page=C3 Home - About, Site=Kingdom First Solutions Mobile App
            //   Attribute: Content
            /*   Attribute Value: ... */
            RockMigrationHelper.AddBlockAttributeValue( "C9A845B7-C2CE-49D4-BA41-5DA0D16762F8", "5682EDBF-68DA-4B43-A593-6C2B936C2839", @"<StackLayout Spacing=""16"">
    <Label Text=""C3 HOME"" StyleClass=""h1,m-16"" />
    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition />
        </Grid.RowDefinitions>
        <Grid.ColumnDefinitions>
            <ColumnDefinition />
            <ColumnDefinition />
        </Grid.ColumnDefinitions>
        <StackLayout>
            <Rock:Icon IconClass=""phone"" FontSize=""24"" HorizontalOptions=""Center"" />
            <Label Text=""PHONE"" HorizontalOptions=""Center"" StyleClass=""text-sm"" />
        </StackLayout>
        <Button Command=""{Binding CallPhoneNumber}"" CommandParameter=""14176731245"" StyleClass=""btn,btn-link"" />
        <StackLayout Grid.Column=""1"">
            <Rock:Icon IconClass=""at"" FontSize=""24"" HorizontalOptions=""Center"" />
            <Label Text=""EMAIL"" HorizontalOptions=""Center"" StyleClass=""text-sm""  />
        </StackLayout>
        <Button Grid.Column=""1"" Command=""{Binding SendEmail}"" CommandParameter=""office@cartervillechristian.com"" StyleClass=""btn,btn-link"" />
    </Grid>
    <BoxView HeightRequest=""1"" Color=""{Rock:PaletteColor gray-200}"" />
    <StackLayout Orientation=""Horizontal"">
        <StackLayout.GestureRecognizers>
            <TapGestureRecognizer Command=""{Binding OpenExternalBrowser}"" CommandParameter=""https://goo.gl/maps/zPRFDSM9bavNTn2h6"" />
        </StackLayout.GestureRecognizers>
        <Rock:Icon IconClass=""map-marked-alt"" FontSize=""24"" VerticalOptions=""Center"" HorizontalTextAlignment=""Center"" WidthRequest=""60"" />
        <StackLayout>
            <Label Text=""Directions"" />
            <Label StyleClass=""font-weight-bold"">
                <Label.Text>
20123 Gravel Rd
Joplin, MO 64801
                </Label.Text>
            </Label>
        </StackLayout>
    </StackLayout>
    <BoxView HeightRequest=""1"" Color=""{Rock:PaletteColor gray-200}"" />
    <StackLayout StyleClass=""p-16"">
        <Label Text=""SUNDAY | 9:00 AM &amp; 10:45 AM"" StyleClass=""font-weight-bold"" />
        <Label StyleClass=""leading-loose"">
            <Label.Text>
Childcare for Birth - PreK
Programming for K-5th Grade
Sunday School for 6th - 12th Grade @ 9:00 AM
            </Label.Text>
        </Label>
        <Label Text=""WEDNESDAY"" StyleClass=""font-weight-bold,mt-24"" />
        <Label StyleClass=""leading-loose"">
            <Label.Text>
C3 Kids • 6:15 - 8:00 PM
C3 Youth • 6:15 - 8:00 PM
C3 Adult Electives • Check ""Coming Up"" for current elective times
            </Label.Text>
        </Label>
    </StackLayout>
</StackLayout>" );

            // Add Block Attribute Value
            //   Block: Content
            //   BlockType: Content
            //   Category: Mobile > Cms
            //   Block Location: Page=C3 Home - About, Site=Kingdom First Solutions Mobile App
            //   Attribute: Dynamic Content
            /*   Attribute Value: False */
            RockMigrationHelper.AddBlockAttributeValue( "C9A845B7-C2CE-49D4-BA41-5DA0D16762F8", "B31D29A0-3725-4AEB-8360-7D91B9CDFE47", @"False" );

            // Add Block Attribute Value
            //   Block: Content
            //   BlockType: Content
            //   Category: Mobile > Cms
            //   Block Location: Page=C3 Home - Ministries, Site=Kingdom First Solutions Mobile App
            //   Attribute: Content
            /*   Attribute Value: ... */
            RockMigrationHelper.AddBlockAttributeValue( "4769FD72-8993-497D-B6F1-6DC90BA47559", "5682EDBF-68DA-4B43-A593-6C2B936C2839", @"<StackLayout>
    <Rock:Image Source=""https://rock.cartervillechristian.com/Content/C3App/AppImages/Ministries/NewAppNavigation-12.jpg"" Command=""{Binding PushPage}"" CommandParameter=""25a63940-8196-4ea3-b943-d3caac43072d?url={{ 'https://www.cartervillechristian.com/c3kids' | UrlEncode }}"" />
    <Rock:Image Source=""https://rock.cartervillechristian.com/Content/C3App/AppImages/Ministries/NewAppNavigation-13.jpg"" Command=""{Binding PushPage}"" CommandParameter=""25a63940-8196-4ea3-b943-d3caac43072d?url={{ 'https://www.cartervillechristian.com/c3youth' | UrlEncode }}"" />
    <Rock:Image Source=""https://rock.cartervillechristian.com/Content/C3App/AppImages/Ministries/NewAppNavigation-14.jpg"" Command=""{Binding PushPage}"" CommandParameter=""25a63940-8196-4ea3-b943-d3caac43072d?url={{ 'https://www.cartervillechristian.com/c3adults' | UrlEncode }}"" />
    <Rock:Image Source=""https://rock.cartervillechristian.com/Content/C3App/AppImages/Ministries/NewAppNavigation-15.jpg"" Command=""{Binding PushPage}"" CommandParameter=""25a63940-8196-4ea3-b943-d3caac43072d?url={{ 'https://www.cartervillechristian.com/outreach' | UrlEncode }}"" />
</StackLayout>" );

            // Add Block Attribute Value
            //   Block: Content
            //   BlockType: Content
            //   Category: Mobile > Cms
            //   Block Location: Page=C3 Home - Ministries, Site=Kingdom First Solutions Mobile App
            //   Attribute: Dynamic Content
            /*   Attribute Value: False */
            RockMigrationHelper.AddBlockAttributeValue( "4769FD72-8993-497D-B6F1-6DC90BA47559", "B31D29A0-3725-4AEB-8360-7D91B9CDFE47", @"False" );

            // Add Block Attribute Value
            //   Block: Content
            //   BlockType: Content
            //   Category: Mobile > Cms
            //   Block Location: Page=C3 Home - What's Next, Site=Kingdom First Solutions Mobile App
            //   Attribute: Content
            /*   Attribute Value: ... */
            RockMigrationHelper.AddBlockAttributeValue( "6857C9E8-3EE6-42B0-8878-33DE5E44E41D", "5682EDBF-68DA-4B43-A593-6C2B936C2839", @"<StackLayout>
    <Rock:Image Source=""https://rock.cartervillechristian.com/Content/C3App/AppImages/WhatNext/NewAppNavigation-16.jpg"" Command=""{Binding PushPage}"" CommandParameter=""25a63940-8196-4ea3-b943-d3caac43072d?url={{ 'https://www.cartervillechristian.com/Connect' | UrlEncode }}"" />
    <Rock:Image Source=""https://rock.cartervillechristian.com/Content/C3App/AppImages/WhatNext/NewAppNavigation-17.jpg"" Command=""{Binding PushPage}"" CommandParameter=""25a63940-8196-4ea3-b943-d3caac43072d?url={{ 'https://www.cartervillechristian.com/comingup/interestedinbaptism' | UrlEncode }}"" />
    <Rock:Image Source=""https://rock.cartervillechristian.com/Content/C3App/AppImages/WhatNext/NewAppNavigation-23.jpg"" Command=""{Binding PushPage}"" CommandParameter=""d71681c4-3662-4e3c-8a8b-9a72844dea43"" />
    <Rock:Image Source=""https://rock.cartervillechristian.com/Content/C3App/AppImages/WhatNext/NewAppNavigation-19.jpg"" Command=""{Binding PushPage}"" CommandParameter=""3696c320-3c61-476f-94a8-11e934891d67"" />
    <Rock:Image Source=""https://rock.cartervillechristian.com/Content/C3App/AppImages/WhatNext/NewAppNavigation-20.jpg"" Command=""{Binding PushPage}"" CommandParameter=""25a63940-8196-4ea3-b943-d3caac43072d?url={{ 'https://www.cartervillechristian.com/serve' | UrlEncode }}"" />
    <Rock:Image Source=""https://rock.cartervillechristian.com/Content/C3App/AppImages/WhatNext/NewAppNavigation-22.jpg"" Command=""{Binding PushPage}"" CommandParameter=""25a63940-8196-4ea3-b943-d3caac43072d?url={{ 'https://www.cartervillechristian.com/discipleship' | UrlEncode }}"" />
</StackLayout>" );

            // Add Block Attribute Value
            //   Block: Content
            //   BlockType: Content
            //   Category: Mobile > Cms
            //   Block Location: Page=C3 Home - What's Next, Site=Kingdom First Solutions Mobile App
            //   Attribute: Dynamic Content
            /*   Attribute Value: False */
            RockMigrationHelper.AddBlockAttributeValue( "6857C9E8-3EE6-42B0-8878-33DE5E44E41D", "B31D29A0-3725-4AEB-8360-7D91B9CDFE47", @"False" );

            // Add Block Attribute Value
            //   Block: Content
            //   BlockType: Content
            //   Category: Mobile > Cms
            //   Block Location: Page=My Groups, Site=Kingdom First Solutions Mobile App
            //   Attribute: Dynamic Content
            /*   Attribute Value: True */
            RockMigrationHelper.AddBlockAttributeValue( "D8295512-7FF1-43D8-9DE1-6CAA59D2BFB7", "B31D29A0-3725-4AEB-8360-7D91B9CDFE47", @"True" );

            // Add Block Attribute Value
            //   Block: Content
            //   BlockType: Content
            //   Category: Mobile > Cms
            //   Block Location: Page=My Groups, Site=Kingdom First Solutions Mobile App
            //   Attribute: Enabled Lava Commands
            /*   Attribute Value: RockEntity */
            RockMigrationHelper.AddBlockAttributeValue( "D8295512-7FF1-43D8-9DE1-6CAA59D2BFB7", "24516448-3F1F-4F27-97A1-CFB4F8B277B5", @"RockEntity" );

            // Add Block Attribute Value
            //   Block: Content
            //   BlockType: Content
            //   Category: Mobile > Cms
            //   Block Location: Page=My Groups, Site=Kingdom First Solutions Mobile App
            //   Attribute: Content
            /*   Attribute Value: ... */
            RockMigrationHelper.AddBlockAttributeValue( "D8295512-7FF1-43D8-9DE1-6CAA59D2BFB7", "5682EDBF-68DA-4B43-A593-6C2B936C2839", @"{%- assign groupMembers = CurrentPerson | Groups:""25"" -%}{%- assign DetailPage = ""efb5029c-0bea-4ade-bfed-2e75b99a46d4"" -%}
<StackLayout>
    <Label Text=""My Groups"" StyleClass=""h1,mb-16"" />
        <Rock:Divider />
        {% for member in groupMembers %}{% assign group = member.Group %}
        <Grid ColumnDefinitions=""1*, 15"" ColumnSpacing=""12"" StyleClass=""group-content"">
            {% if DetailPage != null %}
                <Grid.GestureRecognizers>
                    <TapGestureRecognizer Command=""{Binding PushPage}"" CommandParameter=""{{ DetailPage }}?GroupGuid={{ group.Guid }}"" />
                </Grid.GestureRecognizers>
            {% endif %}
            <StackLayout Grid.Column=""0"" StyleClass=""group-primary-content"">
                {% if group.Schedule.WeeklyDayOfWeek != null %}
                    <Label Text=""{{ group.Schedule.WeeklyDayOfWeek }}"" StyleClass=""group-meeting-day"" />
                {% endif %}
                <Label Text=""{{ group.Name | Escape }}"" StyleClass=""group-name"" />
                <StackLayout Orientation=""Horizontal"">
                    {% if group.Schedule.WeeklyTimeOfDay != null %}
                        <Label Text=""Weekly at {{ group.Schedule.WeeklyTimeOfDayText }}"" HorizontalOptions=""Start"" StyleClass=""group-meeting-time"" />
                    {% elsif group.Schedule != null %}
                        <Label Text=""{{ group.Schedule.FriendlyScheduleText }}"" HorizontalOptions=""Start"" StyleClass=""group-meeting-time"" />
                    {% endif %}
                <Label Text=""{{ member.GroupRole.Name }}"" HorizontalTextAlignment=""End"" HorizontalOptions=""EndAndExpand"" StyleClass=""group-topic"" />
                </StackLayout>
            </StackLayout>

            <Rock:Icon IconClass=""chevron-right"" Grid.Column=""1"" HorizontalOptions=""End"" VerticalOptions=""Center"" StyleClass=""group-more-icon"" />
        </Grid>

        <Rock:Divider />
        {% endfor %}
    </StackLayout>" );

            // Add Block Attribute Value
            //   Block: Group Finder
            //   BlockType: Group Finder
            //   Category: Mobile > Groups
            //   Block Location: Page=Group Finder, Site=Kingdom First Solutions Mobile App
            //   Attribute: Location Types
            /*   Attribute Value: 8c52e53c-2a66-435a-ae6e-5ee307d9a0dc */
            RockMigrationHelper.AddBlockAttributeValue( "5976C5DF-5D71-4FF5-9FB7-F3C6DF4AC40E", "7564A0E0-A592-4910-AF8C-0428794DBF0F", @"8c52e53c-2a66-435a-ae6e-5ee307d9a0dc" );

            // Add Block Attribute Value
            //   Block: Group Finder
            //   BlockType: Group Finder
            //   Category: Mobile > Groups
            //   Block Location: Page=Group Finder, Site=Kingdom First Solutions Mobile App
            //   Attribute: Hide Overcapacity Groups
            /*   Attribute Value: True */
            RockMigrationHelper.AddBlockAttributeValue( "5976C5DF-5D71-4FF5-9FB7-F3C6DF4AC40E", "87724BAC-1E57-409D-8885-5318267BE5AF", @"True" );

            // Add Block Attribute Value
            //   Block: Group Finder
            //   BlockType: Group Finder
            //   Category: Mobile > Groups
            //   Block Location: Page=Group Finder, Site=Kingdom First Solutions Mobile App
            //   Attribute: Show Results on Initial Page Load
            /*   Attribute Value: False */
            RockMigrationHelper.AddBlockAttributeValue( "5976C5DF-5D71-4FF5-9FB7-F3C6DF4AC40E", "11B3600F-48D8-4064-A6A9-53B696E1C8FB", @"False" );

            // Add Block Attribute Value
            //   Block: Group Finder
            //   BlockType: Group Finder
            //   Category: Mobile > Groups
            //   Block Location: Page=Group Finder, Site=Kingdom First Solutions Mobile App
            //   Attribute: Template
            /*   Attribute Value: cc117dbb-5c3c-4a32-8aba-88a7493c7f70| */
            RockMigrationHelper.AddBlockAttributeValue( "5976C5DF-5D71-4FF5-9FB7-F3C6DF4AC40E", "92F92A63-7577-43CD-999F-DC51CA728659", @"cc117dbb-5c3c-4a32-8aba-88a7493c7f70|" );

            // Add Block Attribute Value
            //   Block: Group Finder
            //   BlockType: Group Finder
            //   Category: Mobile > Groups
            //   Block Location: Page=Group Finder, Site=Kingdom First Solutions Mobile App
            //   Attribute: Max Results
            /*   Attribute Value: 25 */
            RockMigrationHelper.AddBlockAttributeValue( "5976C5DF-5D71-4FF5-9FB7-F3C6DF4AC40E", "0F1D09D7-FA63-4B89-85E9-D4FF914D4C56", @"25" );

            // Add Block Attribute Value
            //   Block: Group Finder
            //   BlockType: Group Finder
            //   Category: Mobile > Groups
            //   Block Location: Page=Group Finder, Site=Kingdom First Solutions Mobile App
            //   Attribute: Show Location Filter
            /*   Attribute Value: True */
            RockMigrationHelper.AddBlockAttributeValue( "5976C5DF-5D71-4FF5-9FB7-F3C6DF4AC40E", "52567D6D-FE95-455E-BA3C-646E2491389F", @"True" );

            // Add Block Attribute Value
            //   Block: Group Finder
            //   BlockType: Group Finder
            //   Category: Mobile > Groups
            //   Block Location: Page=Group Finder, Site=Kingdom First Solutions Mobile App
            //   Attribute: Show Campus Filter
            /*   Attribute Value: True */
            RockMigrationHelper.AddBlockAttributeValue( "5976C5DF-5D71-4FF5-9FB7-F3C6DF4AC40E", "6EF61B27-9F15-4F85-BE3D-999CFF1C0491", @"True" );

            // Add Block Attribute Value
            //   Block: Group Finder
            //   BlockType: Group Finder
            //   Category: Mobile > Groups
            //   Block Location: Page=Group Finder, Site=Kingdom First Solutions Mobile App
            //   Attribute: Show Day of Week Filter
            /*   Attribute Value: True */
            RockMigrationHelper.AddBlockAttributeValue( "5976C5DF-5D71-4FF5-9FB7-F3C6DF4AC40E", "A041F92C-E9EA-4D1F-AED8-A39CA1CD04BE", @"True" );

            // Add Block Attribute Value
            //   Block: Group Finder
            //   BlockType: Group Finder
            //   Category: Mobile > Groups
            //   Block Location: Page=Group Finder, Site=Kingdom First Solutions Mobile App
            //   Attribute: Show Time Period Filter
            /*   Attribute Value: True */
            RockMigrationHelper.AddBlockAttributeValue( "5976C5DF-5D71-4FF5-9FB7-F3C6DF4AC40E", "B3DBED1D-92A7-442D-9F47-C6DC2BB4CF7C", @"True" );

            // Add Block Attribute Value
            //   Block: Group Finder
            //   BlockType: Group Finder
            //   Category: Mobile > Groups
            //   Block Location: Page=Group Finder, Site=Kingdom First Solutions Mobile App
            //   Attribute: Campus Context Enabled
            /*   Attribute Value: False */
            RockMigrationHelper.AddBlockAttributeValue( "5976C5DF-5D71-4FF5-9FB7-F3C6DF4AC40E", "29253485-695E-418F-881A-C27E942B634E", @"False" );

            // Add Block Attribute Value
            //   Block: Group Finder
            //   BlockType: Group Finder
            //   Category: Mobile > Groups
            //   Block Location: Page=Group Finder, Site=Kingdom First Solutions Mobile App
            //   Attribute: Detail Page
            /*   Attribute Value: b8c9af1d-7eee-45c0-8619-93abbf528f9b */
            RockMigrationHelper.AddBlockAttributeValue( "5976C5DF-5D71-4FF5-9FB7-F3C6DF4AC40E", "F321149D-4471-4069-9DCA-683A8AE4F199", @"b8c9af1d-7eee-45c0-8619-93abbf528f9b" );

            // Add Block Attribute Value
            //   Block: Group Finder
            //   BlockType: Group Finder
            //   Category: Mobile > Groups
            //   Block Location: Page=Group Finder, Site=Kingdom First Solutions Mobile App
            //   Attribute: Results Transition
            /*   Attribute Value: 1 */
            RockMigrationHelper.AddBlockAttributeValue( "5976C5DF-5D71-4FF5-9FB7-F3C6DF4AC40E", "2344DDD9-8C2E-4C92-B995-EDED842927E1", @"1" );

            // Add Block Attribute Value
            //   Block: Group Finder
            //   BlockType: Group Finder
            //   Category: Mobile > Groups
            //   Block Location: Page=Group Finder, Site=Kingdom First Solutions Mobile App
            //   Attribute: Group Types
            /*   Attribute Value: 50fcfb30-f51a-49df-86f4-2b176ea1820b */
            RockMigrationHelper.AddBlockAttributeValue( "5976C5DF-5D71-4FF5-9FB7-F3C6DF4AC40E", "9123F1E9-DC6C-421D-95A2-AF98D1F159FC", @"50fcfb30-f51a-49df-86f4-2b176ea1820b" );

            // Add Block Attribute Value
            //   Block: Group Finder
            //   BlockType: Group Finder
            //   Category: Mobile > Groups
            //   Block Location: Page=Group Finder, Site=Kingdom First Solutions Mobile App
            //   Attribute: Group Types Location Type
            /*   Attribute Value: {} */
            RockMigrationHelper.AddBlockAttributeValue( "5976C5DF-5D71-4FF5-9FB7-F3C6DF4AC40E", "B9E4AC96-6547-4E74-8FD4-28F3004F98B1", @"{}" );

            // Add Block Attribute Value
            //   Block: Group Finder
            //   BlockType: Group Finder
            //   Category: Mobile > Groups
            //   Block Location: Page=Group Finder, Site=Kingdom First Solutions Mobile App
            //   Attribute: Attribute Filters
            /*   Attribute Value: 04bfac9b-6d1a-e089-446a-b2c604c76764 */
            RockMigrationHelper.AddBlockAttributeValue( "5976C5DF-5D71-4FF5-9FB7-F3C6DF4AC40E", "E100682A-E5B7-4BF6-A39B-15EC6D01D6CB", @"04bfac9b-6d1a-e089-446a-b2c604c76764" );

            // Add Block Attribute Value
            //   Block: Group Edit
            //   BlockType: Group Edit
            //   Category: Mobile > Groups
            //   Block Location: Page=Group Edit, Site=Kingdom First Solutions Mobile App
            //   Attribute: Show Group Name
            /*   Attribute Value: True */
            RockMigrationHelper.AddBlockAttributeValue( "6353052C-7321-4BA2-97B8-3D19CF077FAC", "EB6741F0-22E6-4B90-BEC9-1929CE215B45", @"True" );

            // Add Block Attribute Value
            //   Block: Group Edit
            //   BlockType: Group Edit
            //   Category: Mobile > Groups
            //   Block Location: Page=Group Edit, Site=Kingdom First Solutions Mobile App
            //   Attribute: Enable Group Name Edit
            /*   Attribute Value: True */
            RockMigrationHelper.AddBlockAttributeValue( "6353052C-7321-4BA2-97B8-3D19CF077FAC", "1149943B-BF00-4338-AB46-B62933FDB557", @"True" );

            // Add Block Attribute Value
            //   Block: Group Edit
            //   BlockType: Group Edit
            //   Category: Mobile > Groups
            //   Block Location: Page=Group Edit, Site=Kingdom First Solutions Mobile App
            //   Attribute: Show Description
            /*   Attribute Value: True */
            RockMigrationHelper.AddBlockAttributeValue( "6353052C-7321-4BA2-97B8-3D19CF077FAC", "C894DB28-D08E-4649-9D41-54938D350950", @"True" );

            // Add Block Attribute Value
            //   Block: Group Edit
            //   BlockType: Group Edit
            //   Category: Mobile > Groups
            //   Block Location: Page=Group Edit, Site=Kingdom First Solutions Mobile App
            //   Attribute: Enable Description Edit
            /*   Attribute Value: True */
            RockMigrationHelper.AddBlockAttributeValue( "6353052C-7321-4BA2-97B8-3D19CF077FAC", "86CAFA5C-086A-4993-ADCE-B82024F90959", @"True" );

            // Add Block Attribute Value
            //   Block: Group Edit
            //   BlockType: Group Edit
            //   Category: Mobile > Groups
            //   Block Location: Page=Group Edit, Site=Kingdom First Solutions Mobile App
            //   Attribute: Show Campus
            /*   Attribute Value: True */
            RockMigrationHelper.AddBlockAttributeValue( "6353052C-7321-4BA2-97B8-3D19CF077FAC", "20BB29E2-45FA-469C-9266-88A30EA4D13A", @"True" );

            // Add Block Attribute Value
            //   Block: Group Edit
            //   BlockType: Group Edit
            //   Category: Mobile > Groups
            //   Block Location: Page=Group Edit, Site=Kingdom First Solutions Mobile App
            //   Attribute: Enable Campus Edit
            /*   Attribute Value: True */
            RockMigrationHelper.AddBlockAttributeValue( "6353052C-7321-4BA2-97B8-3D19CF077FAC", "4875251A-8B4B-4C5D-B630-E8D809260945", @"True" );

            // Add Block Attribute Value
            //   Block: Group Edit
            //   BlockType: Group Edit
            //   Category: Mobile > Groups
            //   Block Location: Page=Group Edit, Site=Kingdom First Solutions Mobile App
            //   Attribute: Show Group Capacity
            /*   Attribute Value: True */
            RockMigrationHelper.AddBlockAttributeValue( "6353052C-7321-4BA2-97B8-3D19CF077FAC", "DE4B5956-CB26-48BB-9C40-488A944B3701", @"True" );

            // Add Block Attribute Value
            //   Block: Group Edit
            //   BlockType: Group Edit
            //   Category: Mobile > Groups
            //   Block Location: Page=Group Edit, Site=Kingdom First Solutions Mobile App
            //   Attribute: Enable Group Capacity Edit
            /*   Attribute Value: True */
            RockMigrationHelper.AddBlockAttributeValue( "6353052C-7321-4BA2-97B8-3D19CF077FAC", "24DD3040-6CE5-46A1-BC3C-5E4F1782436F", @"True" );

            // Add Block Attribute Value
            //   Block: Group Edit
            //   BlockType: Group Edit
            //   Category: Mobile > Groups
            //   Block Location: Page=Group Edit, Site=Kingdom First Solutions Mobile App
            //   Attribute: Show Active Status
            /*   Attribute Value: True */
            RockMigrationHelper.AddBlockAttributeValue( "6353052C-7321-4BA2-97B8-3D19CF077FAC", "C0BAF201-4D44-47E2-91C8-9FD89CCFC558", @"True" );

            // Add Block Attribute Value
            //   Block: Group Edit
            //   BlockType: Group Edit
            //   Category: Mobile > Groups
            //   Block Location: Page=Group Edit, Site=Kingdom First Solutions Mobile App
            //   Attribute: Enable Active Status Edit
            /*   Attribute Value: True */
            RockMigrationHelper.AddBlockAttributeValue( "6353052C-7321-4BA2-97B8-3D19CF077FAC", "F4C2FAB6-33FB-4760-9721-7A1EBA714CEA", @"True" );

            // Add Block Attribute Value
            //   Block: Group Edit
            //   BlockType: Group Edit
            //   Category: Mobile > Groups
            //   Block Location: Page=Group Edit, Site=Kingdom First Solutions Mobile App
            //   Attribute: Show Public Status
            /*   Attribute Value: True */
            RockMigrationHelper.AddBlockAttributeValue( "6353052C-7321-4BA2-97B8-3D19CF077FAC", "5043F4B3-1465-4BAD-8375-67CF0F2BC928", @"True" );

            // Add Block Attribute Value
            //   Block: Group Edit
            //   BlockType: Group Edit
            //   Category: Mobile > Groups
            //   Block Location: Page=Group Edit, Site=Kingdom First Solutions Mobile App
            //   Attribute: Enable Public Status Edit
            /*   Attribute Value: True */
            RockMigrationHelper.AddBlockAttributeValue( "6353052C-7321-4BA2-97B8-3D19CF077FAC", "DB9BFD83-5B62-4B07-9747-15A46656F23A", @"True" );

            // Add Block Attribute Value
            //   Block: Group Edit
            //   BlockType: Group Edit
            //   Category: Mobile > Groups
            //   Block Location: Page=Group Edit, Site=Kingdom First Solutions Mobile App
            //   Attribute: Show Header
            /*   Attribute Value: True */
            RockMigrationHelper.AddBlockAttributeValue( "6353052C-7321-4BA2-97B8-3D19CF077FAC", "E5000EFA-D3A4-48C3-AB7D-7161AFA18FC7", @"True" );

            // Add Block Attribute Value
            //   Block: Group Member View
            //   BlockType: Group Member View
            //   Category: Mobile > Groups
            //   Block Location: Page=Group Member View, Site=Kingdom First Solutions Mobile App
            //   Attribute: Template
            /*   Attribute Value: ... */
            RockMigrationHelper.AddBlockAttributeValue( "B502E4F3-D936-41A8-81EC-25966A8FC081", "AD4CCF58-A93D-49C7-860D-02BC0F966724", @"ffffffff-ffff-ffff-ffff-ffffffffffff|<StackLayout Spacing=""0"">
    <Label StyleClass=""h1"" Text=""{{ Member.Group.Name | Escape }}"" />
    <Label Text=""{{ Member.Group.Members | Size }} members"" LineHeight=""0.8"" />

    <StackLayout Orientation=""Horizontal"" Spacing=""20"" Margin=""0, 20, 0, 40"">
            <Rock:Image Source=""{{ 'Global' | Attribute:'PublicApplicationRoot' }}{% if Member.Person.PhotoId != null %}{{ Member.Person.PhotoUrl | Append:'&width=120' | Escape }}{% else %}{{ Member.Person.PhotoUrl | Escape }}{% endif %}"" WidthRequest=""80"">
                <Rock:CircleTransformation />
            </Rock:Image>
            <StackLayout Spacing=""0"" VerticalOptions=""Center"">
                <Label FontSize=""20"" FontAttributes=""Bold"" Text=""{{ Member.Person.FullName | Escape }}"" />
                {% if Member.Person.BirthDate != null %}
                    <Label LineHeight=""0.85"" TextColor=""#888"" Text=""Age: {{ Member.Person.AgePrecise | Floor }}"" />
                    <Label LineHeight=""0.85"" TextColor=""#888"" Text=""Birthdate: {{ Member.Person.BirthDate | Date:'MMMM' }} {{ Member.Person.BirthDate | Date:'d' | NumberToOrdinal }}"" />
                {% endif %}
            </StackLayout>
    </StackLayout>

    <!-- Handle Member Attributes -->
    {% if VisibleAttributes != empty %}
        {% for attribute in VisibleAttributes %}
        <Rock:FieldContainer Margin=""0, 0, 0, {% if forloop.last %}40{% else %}10{% endif %}"">
            <Rock:Literal Label=""{{ attribute.Name | Escape }}"" Text=""{{ attribute.FormattedValue }}"" />
        </Rock:FieldContainer>
        {% endfor %}
    {% endif %}

    <!-- Contact options -->
    {% assign hasContact = false %}
    {% if Member.Person.Email != '' %}
        {% assign hasContact = true %}
        <BoxView Color=""#ccc"" HeightRequest=""1"" />
        <StackLayout Orientation=""Horizontal"" Padding=""12"">
            <StackLayout Spacing=""0"" VerticalOptions=""Center"" HorizontalOptions=""FillAndExpand"">
                <Label FontSize=""16"" FontAttributes=""Bold"" Text=""{{ Member.Person.Email | Escape }}"" />
                <Label Text=""Email"" />
            </StackLayout>
            <Rock:Icon IconClass=""Envelope"" FontSize=""36"" Command=""{Binding SendEmail}"" CommandParameter=""{{ Member.Person.Email | Escape }}"" VerticalOptions=""Center"" />
        </StackLayout>
    {% endif %}

    {% assign phoneNumber = Member.Person | PhoneNumber:'Mobile' %}
    {% assign phoneNumberLong = Member.Person | PhoneNumber:'Mobile',true %}
    {% if phoneNumber != '' and phoneNumber != null %}
        {% assign hasContact = true %}
        <BoxView Color=""#ccc"" HeightRequest=""1"" />
        <StackLayout Orientation=""Horizontal"" Padding=""12"" Spacing=""20"">
            <StackLayout Spacing=""0"" VerticalOptions=""Center"" HorizontalOptions=""FillAndExpand"">
                <Label FontSize=""16"" FontAttributes=""Bold"" Text=""{{ phoneNumber }}"" />
                <Label Text=""Mobile"" />
            </StackLayout>
            <Rock:Icon IconClass=""Comment"" FontSize=""36"" Command=""{Binding SendSms}"" CommandParameter=""{{ phoneNumberLong }}"" VerticalOptions=""Center"" />
            <Rock:Icon IconClass=""Phone"" FontSize=""36"" Command=""{Binding CallPhoneNumber}"" CommandParameter=""{{ phoneNumberLong }}"" VerticalOptions=""Center"" />
        </StackLayout>
    {% endif %}

    {% assign phoneNumber = Member.Person | PhoneNumber:'Home' %}
    {% assign phoneNumberLong = Member.Person | PhoneNumber:'Home',true %}
    {% if phoneNumber != '' and phoneNumber != null %}
        {% assign hasContact = true %}
        <BoxView Color=""#ccc"" HeightRequest=""1"" />
        <StackLayout Orientation=""Horizontal"" Padding=""12"" Spacing=""20"">
            <StackLayout Spacing=""0"" VerticalOptions=""Center"" HorizontalOptions=""FillAndExpand"">
                <Label FontSize=""16"" FontAttributes=""Bold"" Text=""{{ phoneNumber }}"" />
                <Label Text=""Home"" />
            </StackLayout>
            <Rock:Icon IconClass=""Phone"" FontSize=""36"" Command=""{Binding CallPhoneNumber}"" CommandParameter=""{{ phoneNumberLong }}"" VerticalOptions=""Center"" />
        </StackLayout>
    {% endif %}

    {% if hasContact == true %}
        <BoxView Color=""#ccc"" HeightRequest=""1"" />
    {% endif %}

    {% if GroupMemberEditPage != '' %}
        <Button StyleClass=""btn,btn-primary"" Text=""Edit"" Margin=""0, 40, 0, 0"" WidthRequest=""200"" HorizontalOptions=""Center"" Command=""{Binding PushPage}"" CommandParameter=""{{ GroupMemberEditPage }}?GroupMemberGuid={{ Member.Guid }}"" />
    {% endif %}
</StackLayout>" );

            // Add Block Attribute Value
            //   Block: Content Channel Item View
            //   BlockType: Content Channel Item View
            //   Category: Mobile > Cms
            //   Block Location: Page=Content Item View, Site=Kingdom First Solutions Mobile App
            //   Attribute: Content Template
            /*   Attribute Value: ... */
            RockMigrationHelper.AddBlockAttributeValue( "CAD44F15-9400-47DD-B32D-1A105959D230", "D80CF7C7-F6F4-4E77-97A8-B0842E4AF7FB", @"{%- assign imageUrl = Item | Attribute:'ImageUrl' %}{% assign image = Item | Attribute:'Image','Url' -%}
<StackLayout>
    {% if imageUrl != """" or image != """" %}<Rock:Image Source=""{% if imageUrl != '' %}{{ imageUrl }}{% else %}{{ image }}{% endif %}"" />{% endif %}
    <Label Text=""{{ Item.Title | Escape }}"" StyleClass=""h1,mx-16"" />
    <Rock:Html StyleClass=""py-8,px-16"">
        {{ Item.Content | XamlWrap }}
    </Rock:Html>
</StackLayout>" );

            // Add Block Attribute Value
            //   Block: Content Channel Item View
            //   BlockType: Content Channel Item View
            //   Category: Mobile > Cms
            //   Block Location: Page=Content Item View, Site=Kingdom First Solutions Mobile App
            //   Attribute: Log Interactions
            /*   Attribute Value: False */
            RockMigrationHelper.AddBlockAttributeValue( "CAD44F15-9400-47DD-B32D-1A105959D230", "616351D9-41FD-4E84-9378-78140BE30605", @"False" );

            // Add Block Attribute Value
            //   Block: Group Registration
            //   BlockType: Group Registration
            //   Category: Mobile > Groups
            //   Block Location: Page=Group Registration, Site=Kingdom First Solutions Mobile App
            //   Attribute: Group Member Status
            /*   Attribute Value: 2 */
            RockMigrationHelper.AddBlockAttributeValue( "F0C889DB-DCEA-476A-82E6-B56B3718D5AE", "FC8CA904-3E5D-4797-8C95-E032E1E6134E", @"2" );

            // Add Block Attribute Value
            //   Block: Group Registration
            //   BlockType: Group Registration
            //   Category: Mobile > Groups
            //   Block Location: Page=Group Registration, Site=Kingdom First Solutions Mobile App
            //   Attribute: Family Options
            /*   Attribute Value: 3 */
            RockMigrationHelper.AddBlockAttributeValue( "F0C889DB-DCEA-476A-82E6-B56B3718D5AE", "C5DFE24D-62EC-46E5-A23F-2789EA95A450", @"3" );

            // Add Block Attribute Value
            //   Block: Group Registration
            //   BlockType: Group Registration
            //   Category: Mobile > Groups
            //   Block Location: Page=Group Registration, Site=Kingdom First Solutions Mobile App
            //   Attribute: Prevent Overcapacity Registrations
            /*   Attribute Value: False */
            RockMigrationHelper.AddBlockAttributeValue( "F0C889DB-DCEA-476A-82E6-B56B3718D5AE", "3190D83F-6EAE-4B77-B2C1-0B0FAD5EC846", @"False" );

            // Add Block Attribute Value
            //   Block: Group Registration
            //   BlockType: Group Registration
            //   Category: Mobile > Groups
            //   Block Location: Page=Group Registration, Site=Kingdom First Solutions Mobile App
            //   Attribute: Autofill Form
            /*   Attribute Value: True */
            RockMigrationHelper.AddBlockAttributeValue( "F0C889DB-DCEA-476A-82E6-B56B3718D5AE", "E75E6654-0A4F-4833-8051-A6E96A36A784", @"True" );

            // Add Block Attribute Value
            //   Block: Group Registration
            //   BlockType: Group Registration
            //   Category: Mobile > Groups
            //   Block Location: Page=Group Registration, Site=Kingdom First Solutions Mobile App
            //   Attribute: Register Button Text
            /*   Attribute Value: Register */
            RockMigrationHelper.AddBlockAttributeValue( "F0C889DB-DCEA-476A-82E6-B56B3718D5AE", "A7BC77F1-41AF-4360-B08C-3F0E5B2D047A", @"Register" );

            // Add Block Attribute Value
            //   Block: Group Registration
            //   BlockType: Group Registration
            //   Category: Mobile > Groups
            //   Block Location: Page=Group Registration, Site=Kingdom First Solutions Mobile App
            //   Attribute: Mobile Phone
            /*   Attribute Value: 1 */
            RockMigrationHelper.AddBlockAttributeValue( "F0C889DB-DCEA-476A-82E6-B56B3718D5AE", "A0B73F41-364C-41B7-9D3D-5401D457C2C3", @"1" );

            // Add Block Attribute Value
            //   Block: Group Registration
            //   BlockType: Group Registration
            //   Category: Mobile > Groups
            //   Block Location: Page=Group Registration, Site=Kingdom First Solutions Mobile App
            //   Attribute: Email Address
            /*   Attribute Value: 1 */
            RockMigrationHelper.AddBlockAttributeValue( "F0C889DB-DCEA-476A-82E6-B56B3718D5AE", "2BC9774F-9CA1-4DA7-ADD5-A90F8588F814", @"1" );

            // Add Block Attribute Value
            //   Block: Group Registration
            //   BlockType: Group Registration
            //   Category: Mobile > Groups
            //   Block Location: Page=Group Registration, Site=Kingdom First Solutions Mobile App
            //   Attribute: Connection Status
            /*   Attribute Value: 368dd475-242c-49c4-a42c-7278be690cc2 */
            RockMigrationHelper.AddBlockAttributeValue( "F0C889DB-DCEA-476A-82E6-B56B3718D5AE", "39DF8349-D49A-4FDA-9D9A-81F421AE0168", @"368dd475-242c-49c4-a42c-7278be690cc2" );

            // Add Block Attribute Value
            //   Block: Group Registration
            //   BlockType: Group Registration
            //   Category: Mobile > Groups
            //   Block Location: Page=Group Registration, Site=Kingdom First Solutions Mobile App
            //   Attribute: Record Status
            /*   Attribute Value: 283999ec-7346-42e3-b807-bce9b2babb49 */
            RockMigrationHelper.AddBlockAttributeValue( "F0C889DB-DCEA-476A-82E6-B56B3718D5AE", "DA816A40-84DC-4DDD-BA23-572336C164FC", @"283999ec-7346-42e3-b807-bce9b2babb49" );

            // Add Block Attribute Value
            //   Block: Group Registration
            //   BlockType: Group Registration
            //   Category: Mobile > Groups
            //   Block Location: Page=Group Registration, Site=Kingdom First Solutions Mobile App
            //   Attribute: Allowed Group Types
            /*   Attribute Value: 50fcfb30-f51a-49df-86f4-2b176ea1820b */
            RockMigrationHelper.AddBlockAttributeValue( "F0C889DB-DCEA-476A-82E6-B56B3718D5AE", "331E7628-20CF-4979-8FB9-0F28DD91BD63", @"50fcfb30-f51a-49df-86f4-2b176ea1820b" );

            // Add Block Attribute Value
            //   Block: Group Registration
            //   BlockType: Group Registration
            //   Category: Mobile > Groups
            //   Block Location: Page=Group Registration, Site=Kingdom First Solutions Mobile App
            //   Attribute: Registration Completion Message
            /*   Attribute Value: Thank you for the group registration! The group leader will be in touch with you soon. */
            RockMigrationHelper.AddBlockAttributeValue( "F0C889DB-DCEA-476A-82E6-B56B3718D5AE", "CA828A3D-FD7B-42CD-951D-5A834B77517C", @"Thank you for the group registration! The group leader will be in touch with you soon." );

            // Add Block Attribute Value
            //   Block: Group View
            //   BlockType: Group View
            //   Category: Mobile > Groups
            //   Block Location: Page=Group Registration, Site=Kingdom First Solutions Mobile App
            //   Attribute: Template
            /*   Attribute Value: ... */
            RockMigrationHelper.AddBlockAttributeValue( "B96C7C9F-1345-4AFD-A5E4-015EA2428203", "A3826811-395A-4564-8101-EB95936065FB", @"ffffffff-ffff-ffff-ffff-ffffffffffff|<StackLayout Spacing=""0"">
    <StackLayout Orientation=""Horizontal"" Spacing=""20"">
        <StackLayout Orientation=""Vertical"" Spacing=""0"" HorizontalOptions=""FillAndExpand"">
            <Label StyleClass=""h1"" Text=""{{ Group.Name | Escape }}"" />
            <Label Text=""{{ Group.Members | Size }} members"" LineHeight=""0.8"" />
        </StackLayout>
        {% if GroupEditPage != '' and AllowedActions.Edit == true %}
        <Rock:Icon IconClass=""Ellipsis-v"" FontSize=""24"" TextColor=""#ccc"" Command=""{Binding ShowActionPanel}"">
            <Rock:Icon.CommandParameter>
                <Rock:ShowActionPanelParameters Title=""Group Actions"" CancelTitle=""Cancel"">
                    <Rock:ActionPanelButton Title=""Edit Group"" Command=""{Binding PushPage}"" CommandParameter=""{{ GroupEditPage }}?GroupGuid={{ Group.Guid }}"" />
                </Rock:ShowActionPanelParameters>
            </Rock:Icon.CommandParameter>
        </Rock:Icon>
        {% endif %}
    </StackLayout>

    <BoxView Color=""#ccc"" HeightRequest=""1"" Margin=""0, 30, 0, 10"" />

    <!-- Handle Group Attributes -->
    {% if VisibleAttributes != empty %}
        <Rock:ResponsiveLayout>
        {% for attribute in VisibleAttributes %}
            <Rock:ResponsiveColumn ExtraSmall=""6"">
                <Rock:FieldContainer>
                    <Rock:Literal Label=""{{ attribute.Name | Escape }}"" Text=""{{ attribute.FormattedValue }}"" />
                </Rock:FieldContainer>
            </Rock:ResponsiveColumn>
        </Rock:ResponsiveLayout>
        {% endfor %}
    {% endif %}

    <!-- Handle displaying of leaders -->
    {% if ShowLeaderList == true %}
        <Label Text=""Leaders"" StyleClass=""field-title,font-weight-bold"" Margin=""0, 40, 0, 0"" />
        <Grid RowSpacing=""0"" ColumnSpacing=""20"">
            <Grid.ColumnDefinitions>
                <ColumnDefinition Width=""Auto"" />
                <ColumnDefinition Width=""*"" />
            </Grid.ColumnDefinitions>
        {% assign row = 0 %}
        {% assign members = Group.Members | OrderBy:'Person.FullName' %}
        {% for member in members %}
            {% if member.GroupRole.IsLeader == false %}{% continue %}{% endif %}
            <Label Grid.Row=""{{ row }}"" Grid.Column=""0"" Text=""{{ member.Person.FullName }}"" />
            <Label Grid.Row=""{{ row }}"" Grid.Column=""1"" Text=""{{ member.GroupRole.Name }}"" />
            {% assign row = row | Plus:1 %}
        {% endfor %}
        </Grid>
    {% endif %}
    <BoxView Color=""#ccc"" HeightRequest=""1"" Margin=""0, 30, 0, 10"" StyleClass=""my-16"" />
    <Label StyleClass=""h1,mt-16"" Text=""Register for Group"" />
</StackLayout>" );

            // Add Block Attribute Value
            //   Block: Group View
            //   BlockType: Group View
            //   Category: Mobile > Groups
            //   Block Location: Page=Group Registration, Site=Kingdom First Solutions Mobile App
            //   Attribute: Show Leader List
            /*   Attribute Value: True */
            RockMigrationHelper.AddBlockAttributeValue( "B96C7C9F-1345-4AFD-A5E4-015EA2428203", "808D607F-D097-48C5-BC3A-988141A1C69C", @"True" );

            // Add Block Attribute Value
            //   Block: Content Channel Item View
            //   BlockType: Content Channel Item View
            //   Category: Mobile > Cms
            //   Block Location: Page=Series Detail, Site=Kingdom First Solutions Mobile App
            //   Attribute: Content Template
            /*   Attribute Value: ... */
            RockMigrationHelper.AddBlockAttributeValue( "C65485AB-BB67-43E2-9C2A-CAFEA3BD2E14", "D80CF7C7-F6F4-4E77-97A8-B0842E4AF7FB", @"<StackLayout>{% assign rootImageUrl = 'Global' | Attribute:'PublicApplicationRoot' | Append:'GetImage.ashx?Guid=' %}
    <Rock:Image Source=""{{ rootImageUrl }}{{ Item | Attribute:'SeriesImage','RawValue' }}"" />
    <Label Text=""{{ Item.Title | Escape }}"" StyleClass=""h3,mx-8"" />
    <BoxView HeightRequest=""1"" Color=""{Rock:PaletteColor gray-200}"" />
{%- for childItem in Item.ChildItems -%}
    <StackLayout Orientation=""Horizontal"" HeightRequest=""60"" StyleClass=""py-8"">
        <StackLayout.GestureRecognizers>
            <TapGestureRecognizer Command=""{Binding PushPage}"" CommandParameter=""a5733c5c-7329-4a64-8fdb-228eaca7e601?ItemId={{ childItem.ChildContentChannelItem.Id }}"" />
        </StackLayout.GestureRecognizers>
                <Rock:Image Source=""{{ rootImageUrl }}{{ Item | Attribute:'SeriesImage','RawValue' }}"" VerticalOptions=""Center"" WidthRequest=""60"" HeightRequest=""60"" StyleClass=""px-16"" />
        <StackLayout VerticalOptions=""Center"" StyleClass=""pr-8"">
            <Label StyleClass=""font-weight-bold"" FontSize=""16"" Text=""{{ childItem.ChildContentChannelItem.Title }}"" />
            <Label FontSize=""14"" Text=""{{ childItem.ChildContentChannelItem.StartDateTime | Date:'MMMM d, yyyy'}} - {{ childItem.ChildContentChannelItem | Attribute:'Speaker' }}"" />
        </StackLayout>
    </StackLayout>
    <BoxView HeightRequest=""1"" Color=""{Rock:PaletteColor gray-200}"" />
{%- endfor -%}
</StackLayout>" );

            // Add Block Attribute Value
            //   Block: Content Channel Item View
            //   BlockType: Content Channel Item View
            //   Category: Mobile > Cms
            //   Block Location: Page=Series Detail, Site=Kingdom First Solutions Mobile App
            //   Attribute: Content Channel
            /*   Attribute Value: e2c598f1-d299-1baa-4873-8b679e3c1998 */
            RockMigrationHelper.AddBlockAttributeValue( "C65485AB-BB67-43E2-9C2A-CAFEA3BD2E14", "49913217-BF13-4270-8023-C56BDA52C790", @"e2c598f1-d299-1baa-4873-8b679e3c1998" );

            // Add Block Attribute Value
            //   Block: Content Channel Item View
            //   BlockType: Content Channel Item View
            //   Category: Mobile > Cms
            //   Block Location: Page=Series Detail, Site=Kingdom First Solutions Mobile App
            //   Attribute: Log Interactions
            /*   Attribute Value: False */
            RockMigrationHelper.AddBlockAttributeValue( "C65485AB-BB67-43E2-9C2A-CAFEA3BD2E14", "616351D9-41FD-4E84-9378-78140BE30605", @"False" );

            // Add Block Attribute Value
            //   Block: Lava Item List
            //   BlockType: Lava Item List
            //   Category: Mobile > Cms
            //   Block Location: Page=Watch, Site=Kingdom First Solutions Mobile App
            //   Attribute: Page Size
            /*   Attribute Value: 10 */
            RockMigrationHelper.AddBlockAttributeValue( "138B8249-EEE9-4879-B3AA-C0DC30204926", "AFFD987C-99DF-43F5-B95E-636EBFB4F463", @"10" );

            // Add Block Attribute Value
            //   Block: Lava Item List
            //   BlockType: Lava Item List
            //   Category: Mobile > Cms
            //   Block Location: Page=Watch, Site=Kingdom First Solutions Mobile App
            //   Attribute: Detail Page
            /*   Attribute Value: d035d65c-eb4f-4dad-8eac-47478042ca8c */
            RockMigrationHelper.AddBlockAttributeValue( "138B8249-EEE9-4879-B3AA-C0DC30204926", "A644F995-DDF5-438B-8E11-B382874C872B", @"d035d65c-eb4f-4dad-8eac-47478042ca8c" );

            // Add Block Attribute Value
            //   Block: Lava Item List
            //   BlockType: Lava Item List
            //   Category: Mobile > Cms
            //   Block Location: Page=Watch, Site=Kingdom First Solutions Mobile App
            //   Attribute: List Template
            /*   Attribute Value: ... */
            RockMigrationHelper.AddBlockAttributeValue( "138B8249-EEE9-4879-B3AA-C0DC30204926", "2B736AB8-107E-412D-87CC-271F9C813911", @"[
{%- assign rootImageUrl = 'Global' | Attribute:'PublicApplicationRoot' | Append:'GetImage.ashx?Guid=' -%}
{%- contentchannelitem where:'ContentChannelId == ""4"" && StartDateTime <= ""{{ 'Now' | Date }}""' sort:'StartDateTime desc' -%}
{%- for item in contentchannelitemItems -%}
{% assign image = item | Attribute:'SeriesImage','RawValue' | Prepend:rootImageUrl %}{% assign imageUrl = item | Attribute:'SeriesImageUrl' %}{% assign summary = item | Attribute:'Summary' %}
  {
    ""Id"": {{ item.Id }},
    ""Title"": ""{{ item.Title | Replace:'""','\""' }}"",
    ""Image"": ""{% if imageUrl != '' %}{{ imageUrl | Escape }}{% elseif image != '' %}{{ image | Escape }}{% endif %}""
  },{% endfor %}
{% endcontentchannelitem %}
]" );

            // Add Block Attribute Value
            //   Block: Lava Item List
            //   BlockType: Lava Item List
            //   Category: Mobile > Cms
            //   Block Location: Page=Watch, Site=Kingdom First Solutions Mobile App
            //   Attribute: List Data Template
            /*   Attribute Value: ... */
            RockMigrationHelper.AddBlockAttributeValue( "138B8249-EEE9-4879-B3AA-C0DC30204926", "E92051A2-E18B-4599-B9DD-4F43B6D578D5", @"<StackLayout StyleClass=""mb-16"">
    <Rock:Image Source=""{Binding Image}"" />
    <Label Text=""{Binding Title}"" StyleClass=""h2,px-8"" />
</StackLayout>" );

            // Add Block Attribute Value
            //   Block: Content
            //   BlockType: Content
            //   Category: Mobile > Cms
            //   Block Location: Page=Watch - Alternate, Site=Kingdom First Solutions Mobile App
            //   Attribute: Dynamic Content
            /*   Attribute Value: True */
            RockMigrationHelper.AddBlockAttributeValue( "37708AD6-4902-4CEA-BECE-A7FF5517999E", "B31D29A0-3725-4AEB-8360-7D91B9CDFE47", @"True" );

            // Add Block Attribute Value
            //   Block: Content
            //   BlockType: Content
            //   Category: Mobile > Cms
            //   Block Location: Page=Watch - Alternate, Site=Kingdom First Solutions Mobile App
            //   Attribute: Content
            /*   Attribute Value: ... */
            RockMigrationHelper.AddBlockAttributeValue( "37708AD6-4902-4CEA-BECE-A7FF5517999E", "5682EDBF-68DA-4B43-A593-6C2B936C2839", @"<StackLayout>{% assign rootImageUrl = 'Global' | Attribute:'PublicApplicationRoot' | Append:'GetImage.ashx?Guid=' %}
{%- contentchannelitem where:'ContentChannelId == ""4"" && StartDateTime <= ""{{ 'Now' | Date }}""' sort:'StartDateTime desc' -%}
{%- for item in contentchannelitemItems -%}
{% assign image = item | Attribute:'SeriesImage','RawValue' | Prepend:rootImageUrl %}{% assign imageUrl = item | Attribute:'SeriesImageUrl' %}{% assign summary = item | Attribute:'Summary' %}
<Rock:ContainedCard Image=""{% if imageUrl != '' %}{{ imageUrl | Escape }}{% elseif image != '' %}{{ image | Escape }}{% endif %}""
    Title=""{{ item.Title | Escape }}""
    Command=""{Binding PushPage}""
    CommandParameter=""d035d65c-eb4f-4dad-8eac-47478042ca8c?ItemId={{ item.Id }}""
    CornerRadius=""0""></Rock:ContainedCard>
{% endfor %}
{% endcontentchannelitem %}
</StackLayout>" );

            // Add Block Attribute Value
            //   Block: Content
            //   BlockType: Content
            //   Category: Mobile > Cms
            //   Block Location: Page=Watch - Alternate, Site=Kingdom First Solutions Mobile App
            //   Attribute: Enabled Lava Commands
            /*   Attribute Value: RockEntity */
            RockMigrationHelper.AddBlockAttributeValue( "37708AD6-4902-4CEA-BECE-A7FF5517999E", "24516448-3F1F-4F27-97A1-CFB4F8B277B5", @"RockEntity" );

            // Add Block Attribute Value
            //   Block: Content Channel Item View
            //   BlockType: Content Channel Item View
            //   Category: Mobile > Cms
            //   Block Location: Page=Message Detail, Site=Kingdom First Solutions Mobile App
            //   Attribute: Content Template
            /*   Attribute Value: ... */
            RockMigrationHelper.AddBlockAttributeValue( "287ADEF9-787B-43E8-A6AE-7616E86866A5", "D80CF7C7-F6F4-4E77-97A8-B0842E4AF7FB", @"<StackLayout>{% assign rootImageUrl = 'Global' | Attribute:'PublicApplicationRoot' | Append:'GetImage.ashx?Guid=' %}{% assign series = Item.ParentItems | First %}{% assign seriesImageSrc = series.ContentChannelItem | Attribute:'SeriesImage','RawValue' %}
    <Rock:MediaPlayer Source=""{{ Item | Attribute:'VideoLink' }}"" />
    <Label Text=""{{ Item.Title }}"" StyleClass=""h3,pt-8,px-16"" />
    <Label Text=""{{ Item.StartDateTime | Date:'MMMM dd, yyyy' }} - {{ Item | Attribute:'Speaker'}}"" StyleClass=""py-4,px-16"" />
    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition Height=""75"" />
        </Grid.RowDefinitions>
        <Grid.ColumnDefinitions>
            <ColumnDefinition />
            <ColumnDefinition />
            <ColumnDefinition />
        </Grid.ColumnDefinitions>
        <StackLayout VerticalOptions=""CenterAndExpand"">
            <Rock:Icon IconClass=""headphones"" FontSize=""24"" HorizontalOptions=""Center"" />
            <Label Text=""Listen"" HorizontalOptions=""Center"" StyleClass=""text-sm"" />
        </StackLayout>
        <Button Command=""{Binding PlayAudio}"" StyleClass=""btn,btn-link"">
            <Button.CommandParameter>
                <Rock:PlayAudioParameters
                    Source=""{{ Item | Attribute:'AudioLink' }}""
                    ThumbnailSource=""{{ rootImageUrl }}{{ seriesImageSrc }}"" />
            </Button.CommandParameter>
        </Button>
{% assign typeGuid = '48951e97-0e45-4494-b87c-4eb9fca067eb' %}
{% assign noteItem = '' %}
{% for childItem in Item.ChildItems %}
    {% if childItem.ChildContentChannelItem.ContentChannelType.Guid == typeGuid %}
        {% assign noteItem = childItem.ChildContentChannelItem %}
    {% endif %}
{% endfor %}
{% if noteItem != '' %}
    {% assign noteGuid = noteItem.Guid %}
        <StackLayout VerticalOptions=""CenterAndExpand"" Grid.Column=""1"">
            <Rock:Icon IconClass=""sticky-note"" FontSize=""24"" HorizontalOptions=""Center"" />
            <Label Text=""Take Notes"" HorizontalOptions=""Center"" StyleClass=""text-sm""  />
        </StackLayout>
        <Button Grid.Column=""1"" Command=""{Binding PushPage}"" CommandParameter=""aa18a142-b85a-4373-af92-a2e3bd329197?ItemGuid={{ noteGuid }}"" StyleClass=""btn,btn-link"" />
{% endif %}
<StackLayout VerticalOptions=""CenterAndExpand"" Grid.Column=""2"">
            <Rock:Icon IconClass=""share"" FontSize=""24"" HorizontalOptions=""Center"" />
            <Label Text=""Share"" HorizontalOptions=""Center"" StyleClass=""text-sm""  />
        </StackLayout>
        <Button Grid.Column=""2"" StyleClass=""btn,btn-link"" Command=""{Binding ShareContent}"">
            <Button.CommandParameter>{%- assign summaryText = Item | Attribute:""Summary"" | StripNewlines | Trim %}{% if summaryText == """" %}{% assign summaryText = Item.Content | StripHtml | StripNewlines %}{% endif -%}
                <Rock:ShareContentParameters Title=""{{ Item.Title | Escape }}""
                                             Text=""{{ summaryText | TruncateWords:20 | Escape }}""
                                             Uri=""https://members.victoryatl.com/watch/{{ Item.Id }}"" />
            </Button.CommandParameter>
        </Button>
    </Grid>
    <Label Text=""{{ Item.Content | HtmlDecode | StripHtml | StripNewlines | Escape }}"" MaxLines=""3"" LineBreakMode=""TailTruncation"" x:Name=""ExpandText"" StyleClass=""py-4,px-16"" />
    <Button x:Name=""ExpandBtn""
                    StyleClass=""btn,btn-link,mr-16""
                    HorizontalOptions=""End""
                    Text=""More...""
                    Command=""{Binding AggregateCommand}"">
                    <Button.CommandParameter>
                        <Rock:AggregateCommandParameters>
                            <Rock:CommandReference Command=""{Binding SetViewProperty}"">
                                <Rock:SetViewPropertyParameters View=""{x:Reference ExpandText}""
                                    Name=""MaxLines""
                                    Value=""9999"" />
                            </Rock:CommandReference>
                            <Rock:CommandReference Command=""{Binding SetViewProperty}""
                                CommandParameter=""{Rock:SetViewPropertyParameters View={x:Reference ExpandBtn}, Name=IsVisible, Value=false}"" />
                            <Rock:CommandReference Command=""{Binding SetViewProperty}""
                                CommandParameter=""{Rock:SetViewPropertyParameters View={x:Reference HideBtn}, Name=IsVisible, Value=true}"" />
                        </Rock:AggregateCommandParameters>
                    </Button.CommandParameter>
                </Button>
                
                <Button x:Name=""HideBtn""
                    StyleClass=""btn,btn-link,mr-16""
                    HorizontalOptions=""End""
                    Text=""Less...""
                    Command=""{Binding AggregateCommand}""
                    IsVisible=""False"">
                    <Button.CommandParameter>
                        <Rock:AggregateCommandParameters>
                            <Rock:CommandReference Command=""{Binding SetViewProperty}"">
                                <Rock:SetViewPropertyParameters View=""{x:Reference ExpandText}""
                                    Name=""MaxLines""
                                    Value=""3"" />
                            </Rock:CommandReference>
                            <Rock:CommandReference Command=""{Binding SetViewProperty}""
                                CommandParameter=""{Rock:SetViewPropertyParameters View={x:Reference ExpandBtn}, Name=IsVisible, Value=true}"" />
                                <Rock:CommandReference Command=""{Binding SetViewProperty}""
                                CommandParameter=""{Rock:SetViewPropertyParameters View={x:Reference HideBtn}, Name=IsVisible, Value=false}"" />
                        </Rock:AggregateCommandParameters>
                    </Button.CommandParameter>
                </Button>
</StackLayout>" );

            // Add Block Attribute Value
            //   Block: Content Channel Item View
            //   BlockType: Content Channel Item View
            //   Category: Mobile > Cms
            //   Block Location: Page=Message Detail, Site=Kingdom First Solutions Mobile App
            //   Attribute: Log Interactions
            /*   Attribute Value: False */
            RockMigrationHelper.AddBlockAttributeValue( "287ADEF9-787B-43E8-A6AE-7616E86866A5", "616351D9-41FD-4E84-9378-78140BE30605", @"False" );
        }
        public override void Down()
        {

        }
    }
}
