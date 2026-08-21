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
using Rock;
using Rock.Plugin;

namespace rocks.kfs.MobileAppMigration.Migrations
{
    /// <summary>
    /// Nfluence Church mobile application.
    ///
    /// Generated from the source install (SiteId 2036) with CodeGen_MobileAppMigration.sql.
    /// Includes the pieces RockMigrationHelper does not cover on its own:
    ///   * Layout XAML          (Layout.LayoutMobilePhone / LayoutMobileTablet)
    ///   * Mobile page settings (Page.AdditionalSettingsJson)
    ///   * Mobile block settings(Block.AdditionalSettings - ProcessLavaOnServer / ProcessLavaOnClient)
    ///
    /// POST-DEPLOY: see the notes at the bottom of Up() - the content channel
    /// references inside the block Lava are install specific.
    /// </summary>
    [MigrationNumber( 1, "1.16.0" )]
    public class NfluenceMobileApp : Migration
    {
        public override void Up()
        {
﻿            //
            // Site
            //
            RockMigrationHelper.AddSite( "Nfluence Church App", "Nfluence Church App for Rock", "", "1D501408-CA88-4565-8822-BD318F255A59" );
            Sql( @"UPDATE [Site] SET [SiteType] = 1, [IsSystem] = 0 WHERE [Guid] = '1D501408-CA88-4565-8822-BD318F255A59'" );
            Sql( @"UPDATE [Site] SET [AdditionalSettings] = N'{""LastDeploymentDate"":""2026-08-19T16:42:47.7027894-06:00"",""LastDeploymentVersionId"":1787179367,""PhoneUpdatePackageUrl"":""https://e392-2601-8c0-100-6e80-80d8-d059-a3fe-5e3f.ngrok-free.app:443/GetFile.ashx?id=3019"",""TabletUpdatePackageUrl"":""https://e392-2601-8c0-100-6e80-80d8-d059-a3fe-5e3f.ngrok-free.app:443/GetFile.ashx?id=3020"",""ShellType"":2,""TabLocation"":1,""CssStyle"":"""",""ApiKeyId"":1012,""ProfilePageId"":16822,""PersonAttributeCategories"":[0],""BarBackgroundColor"":"""",""IOSEnableBarTransparency"":false,""IOSBarBlurStyle"":0,""MenuButtonColor"":null,""ActivityIndicatorColor"":null,""FlyoutXaml"":""<ListView SeparatorVisibility=\""None\"" \n    HasUnevenRows=\""true\"" \n    ItemsSource=\""{Binding MenuItems}\"">\n\n    <ListView.Header>\n        <StackLayout VerticalOptions=\""FillAndExpand\""\n            Orientation=\""Vertical\"">\n\n            <Rock:LoginStatus Padding=\""20, 70, 20, 50\"" \n                ImageSize=\""120\"" \n                ImageBorderColor=\""rgba(255, 255, 255, 0.4)\"" \n                ImageBorderSize=\""5\"" />\n\n            <BoxView HeightRequest=\""1\"" BackgroundColor=\""rgba(255, 255, 255, 0.2)\""\n                HorizontalOptions=\""FillAndExpand\""/>\n\n        </StackLayout>\n    </ListView.Header>\n\n    <ListView.ItemTemplate>\n        <DataTemplate>\n            <Rock:ViewCell SelectedBackgroundColor=\""rgba(255, 255, 255, 0.2)\"">\n            \n                <StackLayout VerticalOptions=\""FillAndExpand\"" \n                    Orientation=\""Vertical\"">\n\n                    <ContentView StyleClass=\""pt-16, pb-12\"">\n                        <Label StyleClass=\""text-white, ml-32, flyout-menu-item\""\n                            Text=\""{Binding Title}\"" \n                            VerticalOptions=\""Center\"" \n                            HorizontalOptions=\""FillAndExpand\"" />\n                    </ContentView>\n\n                    <BoxView HeightRequest=\""1\""\n                        BackgroundColor=\""rgba(255, 255, 255, 0.4)\""\n                        HorizontalOptions=\""FillAndExpand\"" />\n\n                </StackLayout>\n\n            </Rock:ViewCell>\n        </DataTemplate>\n    </ListView.ItemTemplate>\n\n</ListView>"",""LockedPhoneOrientation"":1,""LockedTabletOrientation"":0,""DownhillSettings"":{""SpacingValues"":{""0"":""0"",""4"":""4"",""8"":""8"",""16"":""16"",""24"":""24"",""48"":""48"",""80"":""80"",""1"":""1"",""2"":""2"",""12"":""12"",""32"":""32"",""64"":""64""},""SpacingUnits"":"""",""FontSizes"":{""xs"":0.75,""sm"":0.875,""base"":1.0,""lg"":1.125,""xl"":1.25,""2xl"":1.5,""3xl"":1.875,""4xl"":2.25,""5xl"":3.0,""6xl"":4.0},""BorderWidths"":[0,1,2,4,8],""Platform"":0,""BorderUnits"":"""",""FontUnits"":"""",""FontSizeDefault"":16.0,""ApplicationColors"":{""Primary"":""#007bff"",""Secondary"":""#6c757d"",""Success"":""#28a745"",""Danger"":""#dc3545"",""Warning"":""#ffc107"",""Info"":""#17a2b8"",""Light"":""#f8f9fa"",""Dark"":""#343a40"",""White"":""#ffffff"",""Brand"":""#007bff"",""InterfaceStrongest"":""#000000"",""InterfaceStronger"":""#1c1c1e"",""InterfaceStrong"":""#5d5d6f"",""InterfaceMedium"":""#8b8ba7"",""InterfaceSoft"":""#d9d9e3"",""InterfaceSofter"":""#f2f2f7"",""InterfaceSoftest"":""#ffffff"",""PrimaryStrong"":""#de5a25"",""PrimarySoft"":""#eeab90"",""SecondaryStrong"":""#53b1fd"",""SecondarySoft"":""#eff8ff"",""BrandStrong"":""#de5a25"",""BrandSoft"":""#eeab90"",""SuccessStrong"":""#248a3d"",""SuccessSoft"":""#d7f4de"",""InfoStrong"":""#007aff"",""InfoSoft"":""#d6eaff"",""DangerStrong"":""#d70015"",""DangerSoft"":""#ffccd1"",""WarningStrong"":""#e58600"",""WarningSoft"":""#ffecd1""},""RadiusBase"":0.0,""TextColor"":""#676767"",""HeadingColor"":""#333333"",""BackgroundColor"":""#ffffff"",""AdditionalCssToParse"":{},""SupplyTailwindCss"":true,""MobileStyleFramework"":2},""NavigationBarActionXaml"":""<Rock:LoginStatusPhoto StyleClass=\""p-8\"" NotLoggedInCommand=\""{Binding PushPage}\"" NotLoggedInPhotoFillColor=\""{AppThemeBinding Light=#4B5563, Dark=#FFFFFF}\""\n NotLoggedInCommandParameter=\""9d8435bd-8583-4325-aefc-af073d0e9020\"" LoggedInCommand=\""{Binding PushPage}\""\n LoggedInCommandParameter=\""9d8435bd-8583-4325-aefc-af073d0e9020\"" ProfilePhotoCircle=\""true\"" ProfilePhotoStrokeWidth=\""1\"" HeightRequest=\""50\"" />"",""HomepageRoutingLogic"":"""",""CampusFilterDataViewId"":null,""CommunicationViewPageId"":16846,""InteractiveExperiencePageId"":null,""SmsConversationPageId"":null,""EnableNotificationsAutomatically"":true,""PushTokenUpdateValue"":"""",""IsDeepLinkingEnabled"":false,""BundleIdentifier"":null,""TeamIdentifier"":null,""PackageName"":null,""CertificateFingerprint"":null,""DeepLinkPathPrefix"":null,""DeepLinkRoutes"":[],""DeepLinkDomains"":null,""IsPackageCompressionEnabled"":true,""Auth0Domain"":"""",""Auth0ClientId"":"""",""Auth0ConnectionStatusValueId"":66,""Auth0RecordStatusValueId"":5,""EntraClientId"":"""",""EntraTenantId"":"""",""EntraAuthenticationComponent"":null}' WHERE [Guid] = '1D501408-CA88-4565-8822-BD318F255A59'" );

            //
            // Layouts
            //
            RockMigrationHelper.AddLayout( "1D501408-CA88-4565-8822-BD318F255A59", "Homepage.xaml", "Homepage", "", "0DB5128F-3D9E-4F0D-91BC-C9B29DAAFBFE" );
            RockMigrationHelper.AddLayout( "1D501408-CA88-4565-8822-BD318F255A59", "No Scrollview.xaml", "No Scrollview", "", "8983814A-6D5D-413C-9FA5-9D54ECA7A1B0" );
            RockMigrationHelper.AddLayout( "1D501408-CA88-4565-8822-BD318F255A59", "Custom Navbar Header.xaml", "Custom Navbar Header", "Used when you want a custom navbar.", "5BDEF023-C5DD-4A0E-BFE5-5F33A97E2537" );

            // Layout XAML (AddLayout does not carry this)
            Sql( @"UPDATE [Layout] SET [LayoutMobilePhone] = N'<?xml version=""1.0"" encoding=""utf-8"" ?>
<ContentPage xmlns=""http://xamarin.com/schemas/2014/forms""
             xmlns:x=""http://schemas.microsoft.com/winfx/2009/xaml""
             xmlns:Rock=""clr-namespace:Rock.Mobile.Cms;assembly=Rock.Mobile""
             xmlns:Common=""clr-namespace:Rock.Mobile.Common;assembly=Rock.Mobile.Common"">
    <ScrollView>
        <StackLayout>
            <Rock:Zone ZoneName=""Main"" />
        </StackLayout>
    </ScrollView>
</ContentPage>', [LayoutMobileTablet] = N'<?xml version=""1.0"" encoding=""utf-8"" ?>
<ContentPage xmlns=""http://xamarin.com/schemas/2014/forms""
             xmlns:x=""http://schemas.microsoft.com/winfx/2009/xaml""
             xmlns:Rock=""clr-namespace:Rock.Mobile.Cms;assembly=Rock.Mobile""
             xmlns:Common=""clr-namespace:Rock.Mobile.Common;assembly=Rock.Mobile.Common"">
    <ScrollView>
        <StackLayout>
            <Rock:Zone ZoneName=""Main"" />
        </StackLayout>
    </ScrollView>
</ContentPage>' WHERE [Guid] = '0DB5128F-3D9E-4F0D-91BC-C9B29DAAFBFE'" );   // Homepage
            Sql( @"UPDATE [Layout] SET [LayoutMobilePhone] = N'<?xml version=""1.0"" encoding=""utf-8"" ?>
<ContentPage xmlns=""http://xamarin.com/schemas/2014/forms""
             xmlns:x=""http://schemas.microsoft.com/winfx/2009/xaml""
             xmlns:Rock=""clr-namespace:Rock.Mobile.Cms;assembly=Rock.Mobile""
             xmlns:Common=""clr-namespace:Rock.Mobile.Common;assembly=Rock.Mobile.Common"">
        <StackLayout>
            <Rock:Zone ZoneName=""Main"" />
        </StackLayout>
</ContentPage>', [LayoutMobileTablet] = N'<?xml version=""1.0"" encoding=""utf-8"" ?>
<ContentPage xmlns=""http://xamarin.com/schemas/2014/forms""
             xmlns:x=""http://schemas.microsoft.com/winfx/2009/xaml""
             xmlns:Rock=""clr-namespace:Rock.Mobile.Cms;assembly=Rock.Mobile""
             xmlns:Common=""clr-namespace:Rock.Mobile.Common;assembly=Rock.Mobile.Common"">
        <StackLayout>
            <Rock:Zone ZoneName=""Main"" />
        </StackLayout>
</ContentPage>' WHERE [Guid] = '8983814A-6D5D-413C-9FA5-9D54ECA7A1B0'" );   // No Scrollview
            Sql( @"UPDATE [Layout] SET [LayoutMobilePhone] = N'<?xml version=""1.0"" encoding=""utf-8"" ?>
<ContentPage xmlns=""http://xamarin.com/schemas/2014/forms""
             xmlns:x=""http://schemas.microsoft.com/winfx/2009/xaml""
             xmlns:Rock=""clr-namespace:Rock.Mobile.Cms;assembly=Rock.Mobile""
             xmlns:Common=""clr-namespace:Rock.Mobile.Common;assembly=Rock.Mobile.Common"">
    <Grid RowDefinitions=""Auto, *"">
        <Rock:Zone ZoneName=""Header"" Grid.Row=""0"" />
        <ScrollView Grid.Row=""1"">
            <StackLayout>
                <Rock:Zone ZoneName=""Main"" />
            </StackLayout>
        </ScrollView>
    </Grid>
</ContentPage>', [LayoutMobileTablet] = N'<?xml version=""1.0"" encoding=""utf-8"" ?>
<ContentPage xmlns=""http://xamarin.com/schemas/2014/forms""
             xmlns:x=""http://schemas.microsoft.com/winfx/2009/xaml""
             xmlns:Rock=""clr-namespace:Rock.Mobile.Cms;assembly=Rock.Mobile""
             xmlns:Common=""clr-namespace:Rock.Mobile.Common;assembly=Rock.Mobile.Common"">
    <Grid RowDefinitions=""Auto, *"">
        <Rock:Zone ZoneName=""Header"" Grid.Row=""0"" />
        <ScrollView Grid.Row=""1"">
            <StackLayout>
                <Rock:Zone ZoneName=""Main"" />
            </StackLayout>
        </ScrollView>
    </Grid>
</ContentPage>' WHERE [Guid] = '5BDEF023-C5DD-4A0E-BFE5-5F33A97E2537'" );   // Custom Navbar Header

            //
            // Pages (parent-first)
            //
            RockMigrationHelper.AddPage( true, null, "0DB5128F-3D9E-4F0D-91BC-C9B29DAAFBFE", "Nfluence Church App Homepage", "", "EF0257AD-B5E4-4D53-B7D0-17561941EE1E", "" );
            RockMigrationHelper.AddPage( true, "EF0257AD-B5E4-4D53-B7D0-17561941EE1E", "0DB5128F-3D9E-4F0D-91BC-C9B29DAAFBFE", "Sundays", "", "AC3DD575-18A9-4ECD-8152-A69DF89A3E46", "" );
            RockMigrationHelper.AddPage( true, "EF0257AD-B5E4-4D53-B7D0-17561941EE1E", "0DB5128F-3D9E-4F0D-91BC-C9B29DAAFBFE", "Give", "", "72A04441-A8FF-44DF-AA70-7059D7C9B8F8", "" );
            RockMigrationHelper.AddPage( true, "EF0257AD-B5E4-4D53-B7D0-17561941EE1E", "0DB5128F-3D9E-4F0D-91BC-C9B29DAAFBFE", "Explore", "", "2E509A98-0FAD-43EC-860D-E83D3B2ACCA9", "" );
            RockMigrationHelper.AddPage( true, "EF0257AD-B5E4-4D53-B7D0-17561941EE1E", "0DB5128F-3D9E-4F0D-91BC-C9B29DAAFBFE", "Profile", "", "5FB03A05-B1FC-49D6-9CDA-2F57BA677671", "" );
            RockMigrationHelper.AddPage( true, "EF0257AD-B5E4-4D53-B7D0-17561941EE1E", "0DB5128F-3D9E-4F0D-91BC-C9B29DAAFBFE", "Item Detail", "", "C9D8BD2D-8F1E-42E6-A4C3-B71B0511E9C7", "" );
            RockMigrationHelper.AddPage( true, "EF0257AD-B5E4-4D53-B7D0-17561941EE1E", "8983814A-6D5D-413C-9FA5-9D54ECA7A1B0", "Webview", "", "C543EF2A-DF73-4C21-BB01-94F2A6CB6373", "" );
            RockMigrationHelper.AddPage( true, "EF0257AD-B5E4-4D53-B7D0-17561941EE1E", "0DB5128F-3D9E-4F0D-91BC-C9B29DAAFBFE", "Login", "", "9BB25932-4D56-417C-911B-DC915167E7BC", "" );
            RockMigrationHelper.AddPage( true, "EF0257AD-B5E4-4D53-B7D0-17561941EE1E", "0DB5128F-3D9E-4F0D-91BC-C9B29DAAFBFE", "Register", "", "16E97046-C04F-4388-8AEE-D5C1CF4A19C8", "" );
            RockMigrationHelper.AddPage( true, "EF0257AD-B5E4-4D53-B7D0-17561941EE1E", "5BDEF023-C5DD-4A0E-BFE5-5F33A97E2537", "User Profile - Notifications", "", "9D8435BD-8583-4325-AEFC-AF073D0E9020", "" );
            RockMigrationHelper.AddPage( true, "EF0257AD-B5E4-4D53-B7D0-17561941EE1E", "5BDEF023-C5DD-4A0E-BFE5-5F33A97E2537", "User Profile - My List", "", "A4FFB56B-938E-44B9-ADB2-A2529B0D8AF2", "" );
            RockMigrationHelper.AddPage( true, "EF0257AD-B5E4-4D53-B7D0-17561941EE1E", "5BDEF023-C5DD-4A0E-BFE5-5F33A97E2537", "User Profile - My Giving", "", "DF9A3772-CC1C-4CE3-885B-ABF828EF6065", "" );
            RockMigrationHelper.AddPage( true, "EF0257AD-B5E4-4D53-B7D0-17561941EE1E", "0DB5128F-3D9E-4F0D-91BC-C9B29DAAFBFE", "Edit Profile", "", "941096AE-FB51-4450-9DB9-6248B584D917", "" );
            RockMigrationHelper.AddPage( true, "EF0257AD-B5E4-4D53-B7D0-17561941EE1E", "0DB5128F-3D9E-4F0D-91BC-C9B29DAAFBFE", "Live Stream", "", "223193F9-9833-4A19-BA36-2B49D312D02A", "" );
            RockMigrationHelper.AddPage( true, "EF0257AD-B5E4-4D53-B7D0-17561941EE1E", "0DB5128F-3D9E-4F0D-91BC-C9B29DAAFBFE", "Menu", "", "A93CFC38-98F5-41DE-B68D-6E7EE97F2D46", "" );
            RockMigrationHelper.AddPage( true, "EF0257AD-B5E4-4D53-B7D0-17561941EE1E", "0DB5128F-3D9E-4F0D-91BC-C9B29DAAFBFE", "Bible", "", "6419FA93-A317-47FC-9C8B-A4265F7BC7EF", "" );
            RockMigrationHelper.AddPage( true, "EF0257AD-B5E4-4D53-B7D0-17561941EE1E", "0DB5128F-3D9E-4F0D-91BC-C9B29DAAFBFE", "Sermon Notes", "", "289D32AD-AD5F-431F-BCBE-7EBEE71D0F19", "" );
            RockMigrationHelper.AddPage( true, "EF0257AD-B5E4-4D53-B7D0-17561941EE1E", "0DB5128F-3D9E-4F0D-91BC-C9B29DAAFBFE", "Settings", "", "5F46D984-6597-4834-9B78-8F009AB1E1E7", "" );
            RockMigrationHelper.AddPage( true, "EF0257AD-B5E4-4D53-B7D0-17561941EE1E", "0DB5128F-3D9E-4F0D-91BC-C9B29DAAFBFE", "Delete Account", "", "5AC83965-D553-4255-B1ED-85F0D8742B6A", "" );
            RockMigrationHelper.AddPage( true, "EF0257AD-B5E4-4D53-B7D0-17561941EE1E", "0DB5128F-3D9E-4F0D-91BC-C9B29DAAFBFE", "Check-in", "", "B29E8335-21C0-4459-A1AD-D85537FC2C08", "" );
            RockMigrationHelper.AddPage( true, "EF0257AD-B5E4-4D53-B7D0-17561941EE1E", "0DB5128F-3D9E-4F0D-91BC-C9B29DAAFBFE", "Events", "", "CBB319A2-9C4E-40FE-829A-55D70842EFDC", "" );
            RockMigrationHelper.AddPage( true, "EF0257AD-B5E4-4D53-B7D0-17561941EE1E", "0DB5128F-3D9E-4F0D-91BC-C9B29DAAFBFE", "Event Detail", "", "A2156601-D477-465E-ADDF-E745DEE935F5", "" );
            RockMigrationHelper.AddPage( true, "EF0257AD-B5E4-4D53-B7D0-17561941EE1E", "0DB5128F-3D9E-4F0D-91BC-C9B29DAAFBFE", "My Serving Schedule", "", "56302B84-36E3-4E62-9E74-C5739D7DE977", "" );
            RockMigrationHelper.AddPage( true, "EF0257AD-B5E4-4D53-B7D0-17561941EE1E", "0DB5128F-3D9E-4F0D-91BC-C9B29DAAFBFE", "My Groups", "", "3430667F-D38C-4A9C-A65E-BC8D15B4FC51", "" );
            RockMigrationHelper.AddPage( true, "EF0257AD-B5E4-4D53-B7D0-17561941EE1E", "0DB5128F-3D9E-4F0D-91BC-C9B29DAAFBFE", "Group Detail", "", "73143E47-3C0E-44BC-8815-0021F88E9F72", "" );
            RockMigrationHelper.AddPage( true, "EF0257AD-B5E4-4D53-B7D0-17561941EE1E", "0DB5128F-3D9E-4F0D-91BC-C9B29DAAFBFE", "Staff Directory", "", "2B7F224A-EA02-4E60-98B0-BCBBA72CEB7C", "" );
            RockMigrationHelper.AddPage( true, "EF0257AD-B5E4-4D53-B7D0-17561941EE1E", "0DB5128F-3D9E-4F0D-91BC-C9B29DAAFBFE", "Sermon Library", "", "F42357CE-077B-4986-B602-CDBAF2EAEAAD", "" );
            RockMigrationHelper.AddPage( true, "EF0257AD-B5E4-4D53-B7D0-17561941EE1E", "0DB5128F-3D9E-4F0D-91BC-C9B29DAAFBFE", "Staff Detail", "", "CB293DA2-94C5-469D-9413-42D59F603B37", "" );
            RockMigrationHelper.AddPage( true, "EF0257AD-B5E4-4D53-B7D0-17561941EE1E", "0DB5128F-3D9E-4F0D-91BC-C9B29DAAFBFE", "Series Detail", "", "6D3762E4-0689-42FE-8535-A7B89C4FC028", "" );
            RockMigrationHelper.AddPage( true, "EF0257AD-B5E4-4D53-B7D0-17561941EE1E", "0DB5128F-3D9E-4F0D-91BC-C9B29DAAFBFE", "Sermon Detail", "", "4079B24C-D548-4CD0-A833-C5688BBEF052", "" );
            RockMigrationHelper.AddPage( true, "EF0257AD-B5E4-4D53-B7D0-17561941EE1E", "0DB5128F-3D9E-4F0D-91BC-C9B29DAAFBFE", "My Giving - All", "", "D77959E1-8A63-49D7-9FA1-A8720214D073", "" );
            RockMigrationHelper.AddPage( true, "EF0257AD-B5E4-4D53-B7D0-17561941EE1E", "0DB5128F-3D9E-4F0D-91BC-C9B29DAAFBFE", "Communication View", "", "E626FC0E-18F1-49DB-8A17-AA0AE375E4E8", "" );
            RockMigrationHelper.AddPage( true, "EF0257AD-B5E4-4D53-B7D0-17561941EE1E", "0DB5128F-3D9E-4F0D-91BC-C9B29DAAFBFE", "Communication Subscribe", "", "FA0ECB26-442E-446E-B78B-51B04B6ABB1D", "" );
            RockMigrationHelper.AddPage( true, "EF0257AD-B5E4-4D53-B7D0-17561941EE1E", "5BDEF023-C5DD-4A0E-BFE5-5F33A97E2537", "Push Notification Detail", "", "F77C9F5C-3AA5-4ED3-8390-DB086ABE7BF7", "" );
            RockMigrationHelper.AddPage( true, "EF0257AD-B5E4-4D53-B7D0-17561941EE1E", "0DB5128F-3D9E-4F0D-91BC-C9B29DAAFBFE", "Post-Service Survey", "", "1519A17F-8CB5-487E-87DD-30FD2E5CF0DA", "" );
            RockMigrationHelper.AddPage( true, "EF0257AD-B5E4-4D53-B7D0-17561941EE1E", "0DB5128F-3D9E-4F0D-91BC-C9B29DAAFBFE", "Post-Service Survey Form", "", "2A5CE7F5-1A5D-49C6-B06A-730CB6FD8ACE", "" );
            RockMigrationHelper.AddPage( true, "EF0257AD-B5E4-4D53-B7D0-17561941EE1E", "0DB5128F-3D9E-4F0D-91BC-C9B29DAAFBFE", "Group Members", "", "BD9535DD-DA9C-4CEC-9397-2E429BE4E6C0", "" );
            RockMigrationHelper.AddPage( true, "EF0257AD-B5E4-4D53-B7D0-17561941EE1E", "0DB5128F-3D9E-4F0D-91BC-C9B29DAAFBFE", "Group Attendance", "", "2826EB49-8A4D-42F7-AD4F-8538F9A2CB05", "" );
            RockMigrationHelper.AddPage( true, "EF0257AD-B5E4-4D53-B7D0-17561941EE1E", "0DB5128F-3D9E-4F0D-91BC-C9B29DAAFBFE", "Group Messages", "", "AAB4218E-E5B1-4728-8E52-CB7A19DCB124", "" );
            RockMigrationHelper.AddPage( true, "EF0257AD-B5E4-4D53-B7D0-17561941EE1E", "0DB5128F-3D9E-4F0D-91BC-C9B29DAAFBFE", "Group Message Detail", "", "C317451F-B45F-4C05-9068-A74265B0568C", "" );
            RockMigrationHelper.AddPage( true, "EF0257AD-B5E4-4D53-B7D0-17561941EE1E", "0DB5128F-3D9E-4F0D-91BC-C9B29DAAFBFE", "Group Needs", "", "DD64051F-EC4A-4AE0-87A1-CA5392DFEB3F", "" );
            RockMigrationHelper.AddPage( true, "EF0257AD-B5E4-4D53-B7D0-17561941EE1E", "0DB5128F-3D9E-4F0D-91BC-C9B29DAAFBFE", "Post Group Note", "", "80BD4600-EB25-4405-B798-57AC6590B390", "" );
            RockMigrationHelper.AddPage( true, "EF0257AD-B5E4-4D53-B7D0-17561941EE1E", "0DB5128F-3D9E-4F0D-91BC-C9B29DAAFBFE", "Structured Item Detail", "", "BB429C19-052F-4537-A40B-A157016B341B", "" );
            RockMigrationHelper.AddPage( true, "EF0257AD-B5E4-4D53-B7D0-17561941EE1E", "0DB5128F-3D9E-4F0D-91BC-C9B29DAAFBFE", "Podcasts", "", "8EAB3BB6-F327-420F-8FE7-00C78A2449C4", "" );
            RockMigrationHelper.AddPage( true, "EF0257AD-B5E4-4D53-B7D0-17561941EE1E", "0DB5128F-3D9E-4F0D-91BC-C9B29DAAFBFE", "Podcast Detail", "", "129CD3DF-28B5-44B0-B77C-1241041E2B50", "" );
            RockMigrationHelper.AddPage( true, "EF0257AD-B5E4-4D53-B7D0-17561941EE1E", "0DB5128F-3D9E-4F0D-91BC-C9B29DAAFBFE", "Sermon Detail (YouTube - archived)", "", "C7AA98DB-9032-4C96-AD1E-4A2A99817A32", "" );
            RockMigrationHelper.AddPage( true, "EF0257AD-B5E4-4D53-B7D0-17561941EE1E", "0DB5128F-3D9E-4F0D-91BC-C9B29DAAFBFE", "Inheritance Campaign", "", "C7F41A93-6E82-4D05-9B3C-1A5E8D07F264", "" );
            RockMigrationHelper.AddPage( true, "EF0257AD-B5E4-4D53-B7D0-17561941EE1E", "0DB5128F-3D9E-4F0D-91BC-C9B29DAAFBFE", "Schedule Preferences", "", "1E4C7A55-8B92-4D30-A6F1-3C08D5B72E41", "" );
            RockMigrationHelper.AddPage( true, "EF0257AD-B5E4-4D53-B7D0-17561941EE1E", "0DB5128F-3D9E-4F0D-91BC-C9B29DAAFBFE", "Sign Up for Serving", "", "2F7B3C88-4D61-4E29-B0A5-9E13F6C48D72", "" );
            RockMigrationHelper.AddPage( true, "EF0257AD-B5E4-4D53-B7D0-17561941EE1E", "0DB5128F-3D9E-4F0D-91BC-C9B29DAAFBFE", "Schedule Unavailability", "", "3A6D9E14-7C25-4F83-91B6-5D420AE7C193", "" );
            RockMigrationHelper.AddPage( true, "EF0257AD-B5E4-4D53-B7D0-17561941EE1E", "0DB5128F-3D9E-4F0D-91BC-C9B29DAAFBFE", "Podcast Feed", "", "A3F7C21D-8E64-4B09-95A2-7D0E1F63B458", "" );
            RockMigrationHelper.AddPage( true, "EF0257AD-B5E4-4D53-B7D0-17561941EE1E", "0DB5128F-3D9E-4F0D-91BC-C9B29DAAFBFE", "Podcast Feed Episode", "", "B4081D3E-9F75-4C1A-A6B3-8E1F2A74C569", "" );

            // Mobile page settings (AutoRefresh, HideNavigationBar, PageType, ...)
            Sql( @"UPDATE [Page] SET [AdditionalSettingsJson] = N'{""AdditionalPageSettings"":{""LavaEventHandler"":"""",""CssStyles"":"""",""HideNavigationBar"":false,""ShowFullScreen"":false,""AutoRefresh"":true,""PageType"":0,""WebPageUrl"":""""}}' WHERE [Guid] = 'EF0257AD-B5E4-4D53-B7D0-17561941EE1E'" );   // Nfluence Church App Homepage
            Sql( @"UPDATE [Page] SET [AdditionalSettingsJson] = N'{""AdditionalPageSettings"":{""LavaEventHandler"":"""",""CssStyles"":"""",""HideNavigationBar"":false,""ShowFullScreen"":false,""AutoRefresh"":true,""PageType"":0,""WebPageUrl"":""""}}' WHERE [Guid] = 'AC3DD575-18A9-4ECD-8152-A69DF89A3E46'" );   // Sundays
            Sql( @"UPDATE [Page] SET [AdditionalSettingsJson] = N'{""AdditionalPageSettings"":{""LavaEventHandler"":"""",""CssStyles"":"""",""HideNavigationBar"":false,""ShowFullScreen"":false,""AutoRefresh"":false,""PageType"":1,""WebPageUrl"":""https://app.securegive.com/nfluencenetwork/main/donate/category""}}' WHERE [Guid] = '72A04441-A8FF-44DF-AA70-7059D7C9B8F8'" );   // Give
            Sql( @"UPDATE [Page] SET [AdditionalSettingsJson] = N'{""AdditionalPageSettings"":{""LavaEventHandler"":"""",""CssStyles"":"""",""HideNavigationBar"":false,""ShowFullScreen"":false,""AutoRefresh"":false,""PageType"":0,""WebPageUrl"":""""}}' WHERE [Guid] = '2E509A98-0FAD-43EC-860D-E83D3B2ACCA9'" );   // Explore
            Sql( @"UPDATE [Page] SET [AdditionalSettingsJson] = N'{""AdditionalPageSettings"":{""LavaEventHandler"":"""",""CssStyles"":"""",""HideNavigationBar"":false,""ShowFullScreen"":false,""AutoRefresh"":false,""PageType"":0,""WebPageUrl"":""""}}' WHERE [Guid] = '5FB03A05-B1FC-49D6-9CDA-2F57BA677671'" );   // Profile
            Sql( @"UPDATE [Page] SET [AdditionalSettingsJson] = N'{""AdditionalPageSettings"":{""LavaEventHandler"":"""",""CssStyles"":"""",""HideNavigationBar"":true,""ShowFullScreen"":false,""AutoRefresh"":false,""PageType"":0,""WebPageUrl"":""""}}' WHERE [Guid] = 'C9D8BD2D-8F1E-42E6-A4C3-B71B0511E9C7'" );   // Item Detail
            Sql( @"UPDATE [Page] SET [AdditionalSettingsJson] = N'{""AdditionalPageSettings"":{""LavaEventHandler"":"""",""CssStyles"":"""",""HideNavigationBar"":false,""ShowFullScreen"":false,""AutoRefresh"":false,""PageType"":0,""WebPageUrl"":""""}}' WHERE [Guid] = 'C543EF2A-DF73-4C21-BB01-94F2A6CB6373'" );   // Webview
            Sql( @"UPDATE [Page] SET [AdditionalSettingsJson] = N'{""AdditionalPageSettings"":{""LavaEventHandler"":"""",""CssStyles"":"""",""HideNavigationBar"":false,""ShowFullScreen"":false,""AutoRefresh"":false,""PageType"":0,""WebPageUrl"":""""}}' WHERE [Guid] = '9BB25932-4D56-417C-911B-DC915167E7BC'" );   // Login
            Sql( @"UPDATE [Page] SET [AdditionalSettingsJson] = N'{""AdditionalPageSettings"":{""LavaEventHandler"":"""",""CssStyles"":"""",""HideNavigationBar"":false,""ShowFullScreen"":false,""AutoRefresh"":false,""PageType"":0,""WebPageUrl"":""""}}' WHERE [Guid] = '16E97046-C04F-4388-8AEE-D5C1CF4A19C8'" );   // Register
            Sql( @"UPDATE [Page] SET [AdditionalSettingsJson] = N'{""AdditionalPageSettings"":{""LavaEventHandler"":"""",""CssStyles"":"""",""HideNavigationBar"":false,""ShowFullScreen"":false,""AutoRefresh"":false,""PageType"":0,""WebPageUrl"":""""}}' WHERE [Guid] = '941096AE-FB51-4450-9DB9-6248B584D917'" );   // Edit Profile
            Sql( @"UPDATE [Page] SET [AdditionalSettingsJson] = N'{""AdditionalPageSettings"":{""LavaEventHandler"":"""",""CssStyles"":"""",""HideNavigationBar"":false,""ShowFullScreen"":false,""AutoRefresh"":false,""PageType"":0,""WebPageUrl"":""""}}' WHERE [Guid] = '223193F9-9833-4A19-BA36-2B49D312D02A'" );   // Live Stream
            Sql( @"UPDATE [Page] SET [AdditionalSettingsJson] = N'{""AdditionalPageSettings"":{""LavaEventHandler"":"""",""CssStyles"":"""",""HideNavigationBar"":false,""ShowFullScreen"":false,""AutoRefresh"":false,""PageType"":0,""WebPageUrl"":""""}}' WHERE [Guid] = 'A93CFC38-98F5-41DE-B68D-6E7EE97F2D46'" );   // Menu
            Sql( @"UPDATE [Page] SET [AdditionalSettingsJson] = N'{""AdditionalPageSettings"":{""LavaEventHandler"":"""",""CssStyles"":"""",""HideNavigationBar"":false,""ShowFullScreen"":false,""AutoRefresh"":false,""PageType"":0,""WebPageUrl"":""""}}' WHERE [Guid] = '6419FA93-A317-47FC-9C8B-A4265F7BC7EF'" );   // Bible
            Sql( @"UPDATE [Page] SET [AdditionalSettingsJson] = N'{""AdditionalPageSettings"":{""LavaEventHandler"":"""",""CssStyles"":"""",""HideNavigationBar"":false,""ShowFullScreen"":false,""AutoRefresh"":false,""PageType"":0,""WebPageUrl"":""""}}' WHERE [Guid] = '289D32AD-AD5F-431F-BCBE-7EBEE71D0F19'" );   // Sermon Notes
            Sql( @"UPDATE [Page] SET [AdditionalSettingsJson] = N'{""AdditionalPageSettings"":{""LavaEventHandler"":"""",""CssStyles"":"""",""HideNavigationBar"":true,""ShowFullScreen"":false,""AutoRefresh"":false,""PageType"":0,""WebPageUrl"":""""}}' WHERE [Guid] = '9D8435BD-8583-4325-AEFC-AF073D0E9020'" );   // User Profile - Notifications
            Sql( @"UPDATE [Page] SET [AdditionalSettingsJson] = N'{""AdditionalPageSettings"":{""LavaEventHandler"":"""",""CssStyles"":"""",""HideNavigationBar"":false,""ShowFullScreen"":false,""AutoRefresh"":false,""PageType"":0,""WebPageUrl"":""""}}' WHERE [Guid] = '5F46D984-6597-4834-9B78-8F009AB1E1E7'" );   // Settings
            Sql( @"UPDATE [Page] SET [AdditionalSettingsJson] = N'{""AdditionalPageSettings"":{""LavaEventHandler"":"""",""CssStyles"":"""",""HideNavigationBar"":false,""ShowFullScreen"":false,""AutoRefresh"":false,""PageType"":0,""WebPageUrl"":""""}}' WHERE [Guid] = '5AC83965-D553-4255-B1ED-85F0D8742B6A'" );   // Delete Account
            Sql( @"UPDATE [Page] SET [AdditionalSettingsJson] = N'{""AdditionalPageSettings"":{""LavaEventHandler"":"""",""CssStyles"":"""",""HideNavigationBar"":false,""ShowFullScreen"":false,""AutoRefresh"":false,""PageType"":0,""WebPageUrl"":""""}}' WHERE [Guid] = 'B29E8335-21C0-4459-A1AD-D85537FC2C08'" );   // Check-in
            Sql( @"UPDATE [Page] SET [AdditionalSettingsJson] = N'{""AdditionalPageSettings"":{""LavaEventHandler"":"""",""CssStyles"":"".calendar-events-day { color: #1F2937; margin-top: 20; margin-bottom: 4;  }\n.calendar-event      { margin-top: 8; }\n.calendar-monthcalendar { margin-bottom: 16; }"",""HideNavigationBar"":false,""ShowFullScreen"":false,""AutoRefresh"":false,""PageType"":0,""WebPageUrl"":""""}}' WHERE [Guid] = 'CBB319A2-9C4E-40FE-829A-55D70842EFDC'" );   // Events
            Sql( @"UPDATE [Page] SET [AdditionalSettingsJson] = N'{""AdditionalPageSettings"":{""LavaEventHandler"":"""",""CssStyles"":"""",""HideNavigationBar"":false,""ShowFullScreen"":false,""AutoRefresh"":false,""PageType"":0,""WebPageUrl"":""""}}' WHERE [Guid] = 'A2156601-D477-465E-ADDF-E745DEE935F5'" );   // Event Detail
            Sql( @"UPDATE [Page] SET [AdditionalSettingsJson] = N'{""AdditionalPageSettings"":{""LavaEventHandler"":"""",""CssStyles"":"""",""HideNavigationBar"":false,""ShowFullScreen"":false,""AutoRefresh"":false,""PageType"":0,""WebPageUrl"":""""}}' WHERE [Guid] = '56302B84-36E3-4E62-9E74-C5739D7DE977'" );   // My Serving Schedule
            Sql( @"UPDATE [Page] SET [AdditionalSettingsJson] = N'{""AdditionalPageSettings"":{""LavaEventHandler"":"""",""CssStyles"":"""",""HideNavigationBar"":false,""ShowFullScreen"":false,""AutoRefresh"":false,""PageType"":0,""WebPageUrl"":""""}}' WHERE [Guid] = '3430667F-D38C-4A9C-A65E-BC8D15B4FC51'" );   // My Groups
            Sql( @"UPDATE [Page] SET [AdditionalSettingsJson] = N'{""AdditionalPageSettings"":{""LavaEventHandler"":"""",""CssStyles"":"""",""HideNavigationBar"":false,""ShowFullScreen"":false,""AutoRefresh"":false,""PageType"":0,""WebPageUrl"":""""}}' WHERE [Guid] = '73143E47-3C0E-44BC-8815-0021F88E9F72'" );   // Group Detail
            Sql( @"UPDATE [Page] SET [AdditionalSettingsJson] = N'{""AdditionalPageSettings"":{""LavaEventHandler"":"""",""CssStyles"":"""",""HideNavigationBar"":false,""ShowFullScreen"":false,""AutoRefresh"":false,""PageType"":0,""WebPageUrl"":""""}}' WHERE [Guid] = '2B7F224A-EA02-4E60-98B0-BCBBA72CEB7C'" );   // Staff Directory
            Sql( @"UPDATE [Page] SET [AdditionalSettingsJson] = N'{""AdditionalPageSettings"":{""LavaEventHandler"":"""",""CssStyles"":"""",""HideNavigationBar"":false,""ShowFullScreen"":false,""AutoRefresh"":false,""PageType"":0,""WebPageUrl"":""""}}' WHERE [Guid] = 'F42357CE-077B-4986-B602-CDBAF2EAEAAD'" );   // Sermon Library
            Sql( @"UPDATE [Page] SET [AdditionalSettingsJson] = N'{""AdditionalPageSettings"":{""LavaEventHandler"":"""",""CssStyles"":"""",""HideNavigationBar"":true,""ShowFullScreen"":false,""AutoRefresh"":false,""PageType"":0,""WebPageUrl"":""""}}' WHERE [Guid] = 'CB293DA2-94C5-469D-9413-42D59F603B37'" );   // Staff Detail
            Sql( @"UPDATE [Page] SET [AdditionalSettingsJson] = N'{""AdditionalPageSettings"":{""LavaEventHandler"":"""",""CssStyles"":"""",""HideNavigationBar"":false,""ShowFullScreen"":false,""AutoRefresh"":false,""PageType"":0,""WebPageUrl"":""""}}' WHERE [Guid] = '6D3762E4-0689-42FE-8535-A7B89C4FC028'" );   // Series Detail
            Sql( @"UPDATE [Page] SET [AdditionalSettingsJson] = N'{""AdditionalPageSettings"":{""LavaEventHandler"":"""",""CssStyles"":"""",""HideNavigationBar"":false,""ShowFullScreen"":false,""AutoRefresh"":false,""PageType"":0,""WebPageUrl"":""""}}' WHERE [Guid] = '4079B24C-D548-4CD0-A833-C5688BBEF052'" );   // Sermon Detail
            Sql( @"UPDATE [Page] SET [AdditionalSettingsJson] = N'{""AdditionalPageSettings"":{""LavaEventHandler"":"""",""CssStyles"":"""",""HideNavigationBar"":false,""ShowFullScreen"":false,""AutoRefresh"":false,""PageType"":0,""WebPageUrl"":""""}}' WHERE [Guid] = 'D77959E1-8A63-49D7-9FA1-A8720214D073'" );   // My Giving - All
            Sql( @"UPDATE [Page] SET [AdditionalSettingsJson] = N'{""AdditionalPageSettings"":{""LavaEventHandler"":"""",""CssStyles"":"""",""HideNavigationBar"":true,""ShowFullScreen"":false,""AutoRefresh"":false,""PageType"":0,""WebPageUrl"":""""}}' WHERE [Guid] = 'A4FFB56B-938E-44B9-ADB2-A2529B0D8AF2'" );   // User Profile - My List
            Sql( @"UPDATE [Page] SET [AdditionalSettingsJson] = N'{""AdditionalPageSettings"":{""LavaEventHandler"":"""",""CssStyles"":"""",""HideNavigationBar"":true,""ShowFullScreen"":false,""AutoRefresh"":false,""PageType"":0,""WebPageUrl"":""""}}' WHERE [Guid] = 'DF9A3772-CC1C-4CE3-885B-ABF828EF6065'" );   // User Profile - My Giving
            Sql( @"UPDATE [Page] SET [AdditionalSettingsJson] = N'{""AdditionalPageSettings"":{""LavaEventHandler"":"""",""CssStyles"":"""",""HideNavigationBar"":false,""ShowFullScreen"":false,""AutoRefresh"":false,""PageType"":0,""WebPageUrl"":""""}}' WHERE [Guid] = 'E626FC0E-18F1-49DB-8A17-AA0AE375E4E8'" );   // Communication View
            Sql( @"UPDATE [Page] SET [AdditionalSettingsJson] = N'{""AdditionalPageSettings"":{""LavaEventHandler"":"""",""CssStyles"":"""",""HideNavigationBar"":false,""ShowFullScreen"":false,""AutoRefresh"":false,""PageType"":0,""WebPageUrl"":""""}}' WHERE [Guid] = 'FA0ECB26-442E-446E-B78B-51B04B6ABB1D'" );   // Communication Subscribe
            Sql( @"UPDATE [Page] SET [AdditionalSettingsJson] = N'{""AdditionalPageSettings"":{""LavaEventHandler"":"""",""CssStyles"":"""",""HideNavigationBar"":true,""ShowFullScreen"":false,""AutoRefresh"":false,""PageType"":0,""WebPageUrl"":""""}}' WHERE [Guid] = 'F77C9F5C-3AA5-4ED3-8390-DB086ABE7BF7'" );   // Push Notification Detail
            Sql( @"UPDATE [Page] SET [AdditionalSettingsJson] = N'{""AdditionalPageSettings"":{""LavaEventHandler"":"""",""CssStyles"":"""",""HideNavigationBar"":false,""ShowFullScreen"":false,""AutoRefresh"":false,""PageType"":0,""WebPageUrl"":""""}}' WHERE [Guid] = '1519A17F-8CB5-487E-87DD-30FD2E5CF0DA'" );   // Post-Service Survey
            Sql( @"UPDATE [Page] SET [AdditionalSettingsJson] = N'{""AdditionalPageSettings"":{""LavaEventHandler"":"""",""CssStyles"":"""",""HideNavigationBar"":false,""ShowFullScreen"":false,""AutoRefresh"":false,""PageType"":0,""WebPageUrl"":""""}}' WHERE [Guid] = '2A5CE7F5-1A5D-49C6-B06A-730CB6FD8ACE'" );   // Post-Service Survey Form
            Sql( @"UPDATE [Page] SET [AdditionalSettingsJson] = N'{""AdditionalPageSettings"":{""LavaEventHandler"":"""",""CssStyles"":"""",""HideNavigationBar"":false,""ShowFullScreen"":false,""AutoRefresh"":false,""PageType"":0,""WebPageUrl"":""""}}' WHERE [Guid] = 'BD9535DD-DA9C-4CEC-9397-2E429BE4E6C0'" );   // Group Members
            Sql( @"UPDATE [Page] SET [AdditionalSettingsJson] = N'{""AdditionalPageSettings"":{""LavaEventHandler"":"""",""CssStyles"":"""",""HideNavigationBar"":false,""ShowFullScreen"":false,""AutoRefresh"":false,""PageType"":0,""WebPageUrl"":""""}}' WHERE [Guid] = '2826EB49-8A4D-42F7-AD4F-8538F9A2CB05'" );   // Group Attendance
            Sql( @"UPDATE [Page] SET [AdditionalSettingsJson] = N'{""AdditionalPageSettings"":{""LavaEventHandler"":"""",""CssStyles"":"""",""HideNavigationBar"":false,""ShowFullScreen"":false,""AutoRefresh"":true,""PageType"":0,""WebPageUrl"":""""}}' WHERE [Guid] = 'AAB4218E-E5B1-4728-8E52-CB7A19DCB124'" );   // Group Messages
            Sql( @"UPDATE [Page] SET [AdditionalSettingsJson] = N'{""AdditionalPageSettings"":{""LavaEventHandler"":"""",""CssStyles"":"""",""HideNavigationBar"":false,""ShowFullScreen"":false,""AutoRefresh"":false,""PageType"":0,""WebPageUrl"":""""}}' WHERE [Guid] = 'C317451F-B45F-4C05-9068-A74265B0568C'" );   // Group Message Detail
            Sql( @"UPDATE [Page] SET [AdditionalSettingsJson] = N'{""AdditionalPageSettings"":{""LavaEventHandler"":"""",""CssStyles"":"""",""HideNavigationBar"":false,""ShowFullScreen"":false,""AutoRefresh"":true,""PageType"":0,""WebPageUrl"":""""}}' WHERE [Guid] = 'DD64051F-EC4A-4AE0-87A1-CA5392DFEB3F'" );   // Group Needs
            Sql( @"UPDATE [Page] SET [AdditionalSettingsJson] = N'{""AdditionalPageSettings"":{""LavaEventHandler"":"""",""CssStyles"":"""",""HideNavigationBar"":false,""ShowFullScreen"":false,""AutoRefresh"":false,""PageType"":0,""WebPageUrl"":""""}}' WHERE [Guid] = '80BD4600-EB25-4405-B798-57AC6590B390'" );   // Post Group Note
            Sql( @"UPDATE [Page] SET [AdditionalSettingsJson] = N'{""AdditionalPageSettings"":{""LavaEventHandler"":"""",""CssStyles"":"".noteeditor {\n    background-color: #1C1C1E;\n    border-color: #3A3A3C;\n    border-radius: 10;\n    margin-top: 16;\n    margin-bottom: 16;\n    padding: 12;\n}\n.noteeditor .noteeditor-label {\n    color: #9CA3AF;\n    font-size: 13;\n    margin-bottom: 8;\n}\n.inner-note-editor ^Editor {\n    -rock-placeholder-text-color: #5A5A60;\n}"",""HideNavigationBar"":true,""ShowFullScreen"":false,""AutoRefresh"":false,""PageType"":0,""WebPageUrl"":""""}}' WHERE [Guid] = 'BB429C19-052F-4537-A40B-A157016B341B'" );   // Structured Item Detail
            Sql( @"UPDATE [Page] SET [AdditionalSettingsJson] = N'{""AdditionalPageSettings"":{""LavaEventHandler"":"""",""CssStyles"":"""",""HideNavigationBar"":false,""ShowFullScreen"":false,""AutoRefresh"":false,""PageType"":0,""WebPageUrl"":""""}}' WHERE [Guid] = '8EAB3BB6-F327-420F-8FE7-00C78A2449C4'" );   // Podcasts
            Sql( @"UPDATE [Page] SET [AdditionalSettingsJson] = N'{""AdditionalPageSettings"":{""LavaEventHandler"":"""",""CssStyles"":"""",""HideNavigationBar"":false,""ShowFullScreen"":false,""AutoRefresh"":false,""PageType"":0,""WebPageUrl"":""""}}' WHERE [Guid] = '129CD3DF-28B5-44B0-B77C-1241041E2B50'" );   // Podcast Detail
            Sql( @"UPDATE [Page] SET [AdditionalSettingsJson] = N'{""AdditionalPageSettings"":{""LavaEventHandler"":"""",""CssStyles"":"""",""HideNavigationBar"":false,""ShowFullScreen"":false,""AutoRefresh"":false,""PageType"":0,""WebPageUrl"":""""}}' WHERE [Guid] = 'C7AA98DB-9032-4C96-AD1E-4A2A99817A32'" );   // Sermon Detail (YouTube - archived)
            Sql( @"UPDATE [Page] SET [AdditionalSettingsJson] = N'{""AdditionalPageSettings"":{""LavaEventHandler"":"""",""CssStyles"":"""",""HideNavigationBar"":false,""ShowFullScreen"":false,""AutoRefresh"":false,""PageType"":0,""WebPageUrl"":""""}}' WHERE [Guid] = 'C7F41A93-6E82-4D05-9B3C-1A5E8D07F264'" );   // Inheritance Campaign
            Sql( @"UPDATE [Page] SET [AdditionalSettingsJson] = N'{""AdditionalPageSettings"":{""LavaEventHandler"":"""",""CssStyles"":"""",""HideNavigationBar"":false,""ShowFullScreen"":false,""AutoRefresh"":false,""PageType"":0,""WebPageUrl"":""""}}' WHERE [Guid] = '1E4C7A55-8B92-4D30-A6F1-3C08D5B72E41'" );   // Schedule Preferences
            Sql( @"UPDATE [Page] SET [AdditionalSettingsJson] = N'{""AdditionalPageSettings"":{""LavaEventHandler"":"""",""CssStyles"":"""",""HideNavigationBar"":false,""ShowFullScreen"":false,""AutoRefresh"":false,""PageType"":0,""WebPageUrl"":""""}}' WHERE [Guid] = '2F7B3C88-4D61-4E29-B0A5-9E13F6C48D72'" );   // Sign Up for Serving
            Sql( @"UPDATE [Page] SET [AdditionalSettingsJson] = N'{""AdditionalPageSettings"":{""LavaEventHandler"":"""",""CssStyles"":"""",""HideNavigationBar"":false,""ShowFullScreen"":false,""AutoRefresh"":false,""PageType"":0,""WebPageUrl"":""""}}' WHERE [Guid] = '3A6D9E14-7C25-4F83-91B6-5D420AE7C193'" );   // Schedule Unavailability

            //
            // Blocks
            //
            RockMigrationHelper.AddBlock( true, "EF0257AD-B5E4-4D53-B7D0-17561941EE1E".AsGuid(), null, "1D501408-CA88-4565-8822-BD318F255A59".AsGuid(), "7258A210-E936-4260-B573-9FA1193AD9E2".AsGuid(), "Content", "Main", @"", @"", 0, "D3C3432F-78C9-4891-9AC4-0C0E6329DCB4" );
            RockMigrationHelper.AddBlock( true, "AC3DD575-18A9-4ECD-8152-A69DF89A3E46".AsGuid(), null, "1D501408-CA88-4565-8822-BD318F255A59".AsGuid(), "7258A210-E936-4260-B573-9FA1193AD9E2".AsGuid(), "Content", "Main", @"", @"", 0, "F8DA4BE8-D31D-479F-9306-F9E0CD450A86" );
            RockMigrationHelper.AddBlock( true, "72A04441-A8FF-44DF-AA70-7059D7C9B8F8".AsGuid(), null, "1D501408-CA88-4565-8822-BD318F255A59".AsGuid(), "7258A210-E936-4260-B573-9FA1193AD9E2".AsGuid(), "Content", "Main", @"", @"", 0, "85EF791C-9F1A-4EB0-B2FB-DA53256E9848" );
            RockMigrationHelper.AddBlock( true, "2E509A98-0FAD-43EC-860D-E83D3B2ACCA9".AsGuid(), null, "1D501408-CA88-4565-8822-BD318F255A59".AsGuid(), "7258A210-E936-4260-B573-9FA1193AD9E2".AsGuid(), "Content", "Main", @"", @"", 0, "F147C24C-E1A1-4C41-968E-0F4FABCD3DE6" );
            RockMigrationHelper.AddBlock( true, "5FB03A05-B1FC-49D6-9CDA-2F57BA677671".AsGuid(), null, "1D501408-CA88-4565-8822-BD318F255A59".AsGuid(), "7258A210-E936-4260-B573-9FA1193AD9E2".AsGuid(), "Content", "Main", @"", @"", 0, "0AF332B5-1F4C-4D17-B3C6-91C899A6C6FC" );
            RockMigrationHelper.AddBlock( true, "C9D8BD2D-8F1E-42E6-A4C3-B71B0511E9C7".AsGuid(), null, "1D501408-CA88-4565-8822-BD318F255A59".AsGuid(), "B76B5F10-D2D6-4C60-B6FB-F913A62442E0".AsGuid(), "Content Channel Item View", "Main", @"", @"", 0, "8B02B9CD-0474-4D79-AEEB-0F91407713CA" );
            RockMigrationHelper.AddBlock( true, "C543EF2A-DF73-4C21-BB01-94F2A6CB6373".AsGuid(), null, "1D501408-CA88-4565-8822-BD318F255A59".AsGuid(), "7258A210-E936-4260-B573-9FA1193AD9E2".AsGuid(), "Content", "Main", @"", @"", 0, "DBD2ABC9-945F-457B-8B0F-80F0A87792F0" );
            RockMigrationHelper.AddBlock( true, "9BB25932-4D56-417C-911B-DC915167E7BC".AsGuid(), null, "1D501408-CA88-4565-8822-BD318F255A59".AsGuid(), "6006FE32-DC01-4B1C-A9B8-EE172451F4C5".AsGuid(), "Login", "Main", @"", @"", 1, "783BB975-D313-4722-A444-D3FF6EE06B3B" );
            RockMigrationHelper.AddBlock( true, "16E97046-C04F-4388-8AEE-D5C1CF4A19C8".AsGuid(), null, "1D501408-CA88-4565-8822-BD318F255A59".AsGuid(), "9544EE9E-07C2-4F14-9C93-3B16EBF0CC47".AsGuid(), "Onboard Person", "Main", @"", @"", 0, "86FA86D1-936D-4AD3-908B-D07D9A874F1F" );
            RockMigrationHelper.AddBlock( true, "941096AE-FB51-4450-9DB9-6248B584D917".AsGuid(), null, "1D501408-CA88-4565-8822-BD318F255A59".AsGuid(), "66B2B513-1C71-4E6B-B4BE-C4EF90E1899C".AsGuid(), "Profile Details", "Main", @"", @"", 0, "9C074994-EE12-41C8-8072-49A3012A72E8" );
            RockMigrationHelper.AddBlock( true, "223193F9-9833-4A19-BA36-2B49D312D02A".AsGuid(), null, "1D501408-CA88-4565-8822-BD318F255A59".AsGuid(), "7258A210-E936-4260-B573-9FA1193AD9E2".AsGuid(), "Content", "Main", @"", @"", 0, "545B0BD2-51C8-48D6-A680-57B50D79454C" );
            RockMigrationHelper.AddBlock( true, "A93CFC38-98F5-41DE-B68D-6E7EE97F2D46".AsGuid(), null, "1D501408-CA88-4565-8822-BD318F255A59".AsGuid(), "7258A210-E936-4260-B573-9FA1193AD9E2".AsGuid(), "Content", "Main", @"", @"", 0, "7D8C58C0-0FE1-4F17-B651-3BDC3B306423" );
            RockMigrationHelper.AddBlock( true, "6419FA93-A317-47FC-9C8B-A4265F7BC7EF".AsGuid(), null, "1D501408-CA88-4565-8822-BD318F255A59".AsGuid(), "7258A210-E936-4260-B573-9FA1193AD9E2".AsGuid(), "Content", "Main", @"", @"", 0, "1C5A37BB-A556-4777-9E33-51A11D4DB8A8" );
            RockMigrationHelper.AddBlock( true, "289D32AD-AD5F-431F-BCBE-7EBEE71D0F19".AsGuid(), null, "1D501408-CA88-4565-8822-BD318F255A59".AsGuid(), "7258A210-E936-4260-B573-9FA1193AD9E2".AsGuid(), "Content", "Main", @"", @"", 0, "83D77AF7-E32C-4994-9CEA-9698E5F7BF25" );
            RockMigrationHelper.AddBlock( true, "9D8435BD-8583-4325-AEFC-AF073D0E9020".AsGuid(), null, "1D501408-CA88-4565-8822-BD318F255A59".AsGuid(), "7258A210-E936-4260-B573-9FA1193AD9E2".AsGuid(), "Content", "Main", @"", @"", 0, "5081D904-65B8-46D7-9BF4-602661982712" );
            RockMigrationHelper.AddBlock( true, "9D8435BD-8583-4325-AEFC-AF073D0E9020".AsGuid(), null, "1D501408-CA88-4565-8822-BD318F255A59".AsGuid(), "7258A210-E936-4260-B573-9FA1193AD9E2".AsGuid(), "Content", "Header", @"", @"", 0, "05137346-7182-43EC-B7BD-581237869417" );
            RockMigrationHelper.AddBlock( true, "9D8435BD-8583-4325-AEFC-AF073D0E9020".AsGuid(), null, "1D501408-CA88-4565-8822-BD318F255A59".AsGuid(), "7258A210-E936-4260-B573-9FA1193AD9E2".AsGuid(), "Notifications", "Main", @"", @"", 1, "2C7A4F19-6B83-4E05-9D24-A15C8E30B7F6" );
            RockMigrationHelper.AddBlock( true, "9D8435BD-8583-4325-AEFC-AF073D0E9020".AsGuid(), null, "1D501408-CA88-4565-8822-BD318F255A59".AsGuid(), "7258A210-E936-4260-B573-9FA1193AD9E2".AsGuid(), "Content - Not logged in", "Main", @"", @"", 2, "10E07AFD-C2EB-4E97-9121-027811648F4B" );
            RockMigrationHelper.AddBlock( true, "5F46D984-6597-4834-9B78-8F009AB1E1E7".AsGuid(), null, "1D501408-CA88-4565-8822-BD318F255A59".AsGuid(), "7258A210-E936-4260-B573-9FA1193AD9E2".AsGuid(), "Content", "Main", @"", @"", 0, "4CAF5978-5265-4633-8021-D53E30C318EE" );
            RockMigrationHelper.AddBlock( true, "5AC83965-D553-4255-B1ED-85F0D8742B6A".AsGuid(), null, "1D501408-CA88-4565-8822-BD318F255A59".AsGuid(), "9116AAD8-CF16-4BCE-B0CF-5B4D565710ED".AsGuid(), "Workflow Entry", "Main", @"", @"", 0, "E44BCD5E-AFA1-42D2-8371-0041ADB65CC5" );
            RockMigrationHelper.AddBlock( true, "B29E8335-21C0-4459-A1AD-D85537FC2C08".AsGuid(), null, "1D501408-CA88-4565-8822-BD318F255A59".AsGuid(), "85A9DDF0-D199-4D7B-887C-9AE8B3508444".AsGuid(), "Check-in", "Main", @"", @"", 0, "82791881-D9E4-48E1-9844-A84CFDC78955" );
            RockMigrationHelper.AddBlock( true, "CBB319A2-9C4E-40FE-829A-55D70842EFDC".AsGuid(), null, "1D501408-CA88-4565-8822-BD318F255A59".AsGuid(), "14B447B3-6117-4142-92E7-E3F289106140".AsGuid(), "Calendar View", "Main", @"", @"", 0, "BEBEC594-4C65-411E-8013-BAC2983D2DD8" );
            RockMigrationHelper.AddBlock( true, "A2156601-D477-465E-ADDF-E745DEE935F5".AsGuid(), null, "1D501408-CA88-4565-8822-BD318F255A59".AsGuid(), "15DD270A-A0BB-45BF-AA36-FE37856C60DE".AsGuid(), "Calendar Event Item Occurrence View", "Main", @"", @"", 0, "B36C1077-5312-44E3-ACFF-DCB32EC72B4A" );
            RockMigrationHelper.AddBlock( true, "56302B84-36E3-4E62-9E74-C5739D7DE977".AsGuid(), null, "1D501408-CA88-4565-8822-BD318F255A59".AsGuid(), "7258A210-E936-4260-B573-9FA1193AD9E2".AsGuid(), "Serving Nav", "Main", @"", @"", 0, "7EB4F509-816A-4D23-B247-9F53C6E0A8D1" );
            RockMigrationHelper.AddBlock( true, "56302B84-36E3-4E62-9E74-C5739D7DE977".AsGuid(), null, "1D501408-CA88-4565-8822-BD318F255A59".AsGuid(), "E00F3C6D-D007-4408-8A41-AD2A6AB29D6E".AsGuid(), "Schedule Toolbox", "Main", @"", @"", 1, "7B49DE66-B707-417F-8A65-A082508A548B" );
            RockMigrationHelper.AddBlock( true, "3430667F-D38C-4A9C-A65E-BC8D15B4FC51".AsGuid(), null, "1D501408-CA88-4565-8822-BD318F255A59".AsGuid(), "7258A210-E936-4260-B573-9FA1193AD9E2".AsGuid(), "Content", "Main", @"", @"", 0, "5E4FFC63-BAA6-4676-AD4C-3C7E0034E0BD" );
            RockMigrationHelper.AddBlock( true, "73143E47-3C0E-44BC-8815-0021F88E9F72".AsGuid(), null, "1D501408-CA88-4565-8822-BD318F255A59".AsGuid(), "7258A210-E936-4260-B573-9FA1193AD9E2".AsGuid(), "Group Header", "Main", @"", @"", 0, "8830E449-A6B7-42B4-8A24-3E847E750502" );
            RockMigrationHelper.AddBlock( true, "73143E47-3C0E-44BC-8815-0021F88E9F72".AsGuid(), null, "1D501408-CA88-4565-8822-BD318F255A59".AsGuid(), "3F34AE03-9378-4363-A232-0318139C3BD3".AsGuid(), "Group View", "Main", @"", @"", 1, "98963BBF-5F11-4B6B-ACCA-ECFFFDB96480" );
            RockMigrationHelper.AddBlock( true, "2B7F224A-EA02-4E60-98B0-BCBBA72CEB7C".AsGuid(), null, "1D501408-CA88-4565-8822-BD318F255A59".AsGuid(), "7258A210-E936-4260-B573-9FA1193AD9E2".AsGuid(), "Content", "Main", @"", @"", 0, "A5F5F565-6542-47B4-8FD8-642B3CC3E7C6" );
            RockMigrationHelper.AddBlock( true, "F42357CE-077B-4986-B602-CDBAF2EAEAAD".AsGuid(), null, "1D501408-CA88-4565-8822-BD318F255A59".AsGuid(), "7258A210-E936-4260-B573-9FA1193AD9E2".AsGuid(), "Content", "Main", @"", @"", 0, "E430DB8D-53B7-432A-BF3D-28D590295FE1" );
            RockMigrationHelper.AddBlock( true, "CB293DA2-94C5-469D-9413-42D59F603B37".AsGuid(), null, "1D501408-CA88-4565-8822-BD318F255A59".AsGuid(), "B76B5F10-D2D6-4C60-B6FB-F913A62442E0".AsGuid(), "Content Channel Item View", "Main", @"", @"", 0, "00AF4314-F34E-4AEF-ADF6-462182AF8D89" );
            RockMigrationHelper.AddBlock( true, "6D3762E4-0689-42FE-8535-A7B89C4FC028".AsGuid(), null, "1D501408-CA88-4565-8822-BD318F255A59".AsGuid(), "B76B5F10-D2D6-4C60-B6FB-F913A62442E0".AsGuid(), "Content Channel Item View", "Main", @"", @"", 0, "C63AD407-B2D3-4DA5-BC49-B34DF3554EE1" );
            RockMigrationHelper.AddBlock( true, "4079B24C-D548-4CD0-A833-C5688BBEF052".AsGuid(), null, "1D501408-CA88-4565-8822-BD318F255A59".AsGuid(), "B76B5F10-D2D6-4C60-B6FB-F913A62442E0".AsGuid(), "Content Channel Item View", "Main", @"", @"", 0, "A67C5D2C-1161-4EBA-8F92-93962EF739F6" );
            RockMigrationHelper.AddBlock( true, "D77959E1-8A63-49D7-9FA1-A8720214D073".AsGuid(), null, "1D501408-CA88-4565-8822-BD318F255A59".AsGuid(), "7258A210-E936-4260-B573-9FA1193AD9E2".AsGuid(), "Content", "Main", @"", @"", 0, "C3E6A9C2-EBA4-4AF9-A37B-3E7F8CFE0DF4" );
            RockMigrationHelper.AddBlock( true, "A4FFB56B-938E-44B9-ADB2-A2529B0D8AF2".AsGuid(), null, "1D501408-CA88-4565-8822-BD318F255A59".AsGuid(), "7258A210-E936-4260-B573-9FA1193AD9E2".AsGuid(), "Content", "Main", @"", @"", 0, "78C66DE9-AE93-4B11-9667-B87EF00A1C4C" );
            RockMigrationHelper.AddBlock( true, "A4FFB56B-938E-44B9-ADB2-A2529B0D8AF2".AsGuid(), null, "1D501408-CA88-4565-8822-BD318F255A59".AsGuid(), "7258A210-E936-4260-B573-9FA1193AD9E2".AsGuid(), "Content", "Header", @"", @"", 0, "81E84E8F-4FC6-4D89-997E-8BBF1A7B2E05" );
            RockMigrationHelper.AddBlock( true, "A4FFB56B-938E-44B9-ADB2-A2529B0D8AF2".AsGuid(), null, "1D501408-CA88-4565-8822-BD318F255A59".AsGuid(), "7258A210-E936-4260-B573-9FA1193AD9E2".AsGuid(), "Content", "Main", @"", @"", 1, "8380B1DB-4D13-47D5-B290-D8D98E6FB4BD" );
            RockMigrationHelper.AddBlock( true, "DF9A3772-CC1C-4CE3-885B-ABF828EF6065".AsGuid(), null, "1D501408-CA88-4565-8822-BD318F255A59".AsGuid(), "7258A210-E936-4260-B573-9FA1193AD9E2".AsGuid(), "Content", "Main", @"", @"", 0, "5166C7A4-C480-4F93-8A1F-634775438974" );
            RockMigrationHelper.AddBlock( true, "DF9A3772-CC1C-4CE3-885B-ABF828EF6065".AsGuid(), null, "1D501408-CA88-4565-8822-BD318F255A59".AsGuid(), "7258A210-E936-4260-B573-9FA1193AD9E2".AsGuid(), "Content", "Header", @"", @"", 0, "65350FBF-7EDA-4D89-9778-6020D53B785F" );
            RockMigrationHelper.AddBlock( true, "DF9A3772-CC1C-4CE3-885B-ABF828EF6065".AsGuid(), null, "1D501408-CA88-4565-8822-BD318F255A59".AsGuid(), "7258A210-E936-4260-B573-9FA1193AD9E2".AsGuid(), "Content - Not signed in", "Main", @"", @"", 1, "77A80F5F-CED1-4471-BCE3-1F405BE29C6B" );
            RockMigrationHelper.AddBlock( true, "DF9A3772-CC1C-4CE3-885B-ABF828EF6065".AsGuid(), null, "1D501408-CA88-4565-8822-BD318F255A59".AsGuid(), "7258A210-E936-4260-B573-9FA1193AD9E2".AsGuid(), "Content", "Main", @"", @"", 2, "FF2703A4-BF7D-4A53-A214-10AD2E850BAA" );
            RockMigrationHelper.AddBlock( true, "E626FC0E-18F1-49DB-8A17-AA0AE375E4E8".AsGuid(), null, "1D501408-CA88-4565-8822-BD318F255A59".AsGuid(), "863E5638-B310-407E-A54E-2C069979881D".AsGuid(), "Communication View", "Main", @"", @"", 0, "DDA29EBD-B8A0-44FD-A1A4-2E783F050005" );
            RockMigrationHelper.AddBlock( true, "FA0ECB26-442E-446E-B78B-51B04B6ABB1D".AsGuid(), null, "1D501408-CA88-4565-8822-BD318F255A59".AsGuid(), "D0C51784-71ED-46F3-86AB-972148B78BE8".AsGuid(), "Communication List Subscribe", "Main", @"", @"", 0, "DE95662D-6AD7-47F3-92F7-7851C3E9E6E8" );
            RockMigrationHelper.AddBlock( true, "F77C9F5C-3AA5-4ED3-8390-DB086ABE7BF7".AsGuid(), null, "1D501408-CA88-4565-8822-BD318F255A59".AsGuid(), "7258A210-E936-4260-B573-9FA1193AD9E2".AsGuid(), "Content", "Header", @"", @"", 0, "BD66F886-936E-4624-AEF5-00698C7C0BFC" );
            RockMigrationHelper.AddBlock( true, "F77C9F5C-3AA5-4ED3-8390-DB086ABE7BF7".AsGuid(), null, "1D501408-CA88-4565-8822-BD318F255A59".AsGuid(), "7258A210-E936-4260-B573-9FA1193AD9E2".AsGuid(), "Content", "Main", @"", @"", 0, "AC2DDABC-C82A-4827-8565-248724D1C324" );
            RockMigrationHelper.AddBlock( true, "1519A17F-8CB5-487E-87DD-30FD2E5CF0DA".AsGuid(), null, "1D501408-CA88-4565-8822-BD318F255A59".AsGuid(), "7258A210-E936-4260-B573-9FA1193AD9E2".AsGuid(), "Intro", "Main", @"", @"", 0, "3B3985B1-5A10-441C-94DD-C83FA15B5579" );
            RockMigrationHelper.AddBlock( true, "2A5CE7F5-1A5D-49C6-B06A-730CB6FD8ACE".AsGuid(), null, "1D501408-CA88-4565-8822-BD318F255A59".AsGuid(), "9116AAD8-CF16-4BCE-B0CF-5B4D565710ED".AsGuid(), "Workflow Entry", "Main", @"", @"", 0, "4687C5D1-BEAE-48F7-AB47-C5F723369EB6" );
            RockMigrationHelper.AddBlock( true, "BD9535DD-DA9C-4CEC-9397-2E429BE4E6C0".AsGuid(), null, "1D501408-CA88-4565-8822-BD318F255A59".AsGuid(), "5A6D2ADB-03A7-4B55-8EAA-26A37116BFF1".AsGuid(), "Group Member List", "Main", @"", @"", 0, "597C9DB9-24E6-4FBF-A97D-A21A32F3B81D" );
            RockMigrationHelper.AddBlock( true, "2826EB49-8A4D-42F7-AD4F-8538F9A2CB05".AsGuid(), null, "1D501408-CA88-4565-8822-BD318F255A59".AsGuid(), "08AE409C-9E4C-42D1-A93C-A554A3EEA0C3".AsGuid(), "Group Attendance Entry", "Main", @"", @"", 0, "E06F0FFF-4020-4B11-9F1E-038724978A34" );
            RockMigrationHelper.AddBlock( true, "AAB4218E-E5B1-4728-8E52-CB7A19DCB124".AsGuid(), null, "1D501408-CA88-4565-8822-BD318F255A59".AsGuid(), "7258A210-E936-4260-B573-9FA1193AD9E2".AsGuid(), "Messages List", "Main", @"", @"", 0, "F9B0195A-585F-4706-99E5-29DE2A392666" );
            RockMigrationHelper.AddBlock( true, "C317451F-B45F-4C05-9068-A74265B0568C".AsGuid(), null, "1D501408-CA88-4565-8822-BD318F255A59".AsGuid(), "7258A210-E936-4260-B573-9FA1193AD9E2".AsGuid(), "Message Detail", "Main", @"", @"", 0, "B3FC2629-4E9C-403F-AA52-4CDBBE9AC126" );
            RockMigrationHelper.AddBlock( true, "DD64051F-EC4A-4AE0-87A1-CA5392DFEB3F".AsGuid(), null, "1D501408-CA88-4565-8822-BD318F255A59".AsGuid(), "7258A210-E936-4260-B573-9FA1193AD9E2".AsGuid(), "Needs List", "Main", @"", @"", 0, "12A48122-F5C7-4731-8780-873E0FAADDAC" );
            RockMigrationHelper.AddBlock( true, "80BD4600-EB25-4405-B798-57AC6590B390".AsGuid(), null, "1D501408-CA88-4565-8822-BD318F255A59".AsGuid(), "9116AAD8-CF16-4BCE-B0CF-5B4D565710ED".AsGuid(), "Workflow Entry", "Main", @"", @"", 0, "43523BEC-BC89-448E-BE74-9DB32CF4BB3B" );
            RockMigrationHelper.AddBlock( true, "BB429C19-052F-4537-A40B-A157016B341B".AsGuid(), null, "1D501408-CA88-4565-8822-BD318F255A59".AsGuid(), "B76B5F10-D2D6-4C60-B6FB-F913A62442E0".AsGuid(), "Content Channel Item View", "Main", @"", @"", 0, "258628D3-224E-4CFA-856B-B9C3A8E097BD" );
            RockMigrationHelper.AddBlock( true, "BB429C19-052F-4537-A40B-A157016B341B".AsGuid(), null, "1D501408-CA88-4565-8822-BD318F255A59".AsGuid(), "7258A210-E936-4260-B573-9FA1193AD9E2".AsGuid(), "Scripture References", "Main", @"", @"", 1, "500D9BFF-47F3-4AEA-9601-6450337C9CE2" );
            RockMigrationHelper.AddBlock( true, "BB429C19-052F-4537-A40B-A157016B341B".AsGuid(), null, "1D501408-CA88-4565-8822-BD318F255A59".AsGuid(), "A8BBE3F8-F3CC-4C0A-AB2F-5085F5BF59E7".AsGuid(), "Structured Content View", "Main", @"", @"", 3, "B4E573CC-516E-4E32-A9B6-8E1475F8086F" );
            RockMigrationHelper.AddBlock( true, "8EAB3BB6-F327-420F-8FE7-00C78A2449C4".AsGuid(), null, "1D501408-CA88-4565-8822-BD318F255A59".AsGuid(), "7258A210-E936-4260-B573-9FA1193AD9E2".AsGuid(), "Podcast List", "Main", @"", @"", 0, "C82E63A0-0D0F-4EA7-A2B8-A53FF93C88F0" );
            RockMigrationHelper.AddBlock( true, "129CD3DF-28B5-44B0-B77C-1241041E2B50".AsGuid(), null, "1D501408-CA88-4565-8822-BD318F255A59".AsGuid(), "7258A210-E936-4260-B573-9FA1193AD9E2".AsGuid(), "Podcast Detail", "Main", @"", @"", 0, "B0264469-6A70-46F8-8A13-AC0B3375652A" );
            RockMigrationHelper.AddBlock( true, "C7AA98DB-9032-4C96-AD1E-4A2A99817A32".AsGuid(), null, "1D501408-CA88-4565-8822-BD318F255A59".AsGuid(), "B76B5F10-D2D6-4C60-B6FB-F913A62442E0".AsGuid(), "Content Channel Item View", "Main", @"", @"", 0, "633A561C-B5A3-44A6-97E5-5A422CCD1AA8" );
            RockMigrationHelper.AddBlock( true, "C7F41A93-6E82-4D05-9B3C-1A5E8D07F264".AsGuid(), null, "1D501408-CA88-4565-8822-BD318F255A59".AsGuid(), "7258A210-E936-4260-B573-9FA1193AD9E2".AsGuid(), "Inheritance Campaign", "Main", @"", @"", 0, "5D9A02B7-84C1-4E36-A7F8-B0629C4E1D53" );
            RockMigrationHelper.AddBlock( true, "1E4C7A55-8B92-4D30-A6F1-3C08D5B72E41".AsGuid(), null, "1D501408-CA88-4565-8822-BD318F255A59".AsGuid(), "7258A210-E936-4260-B573-9FA1193AD9E2".AsGuid(), "Serving Nav", "Main", @"", @"", 0, "8FC5061A-927B-4E34-C358-A064D7F1B9E2" );
            RockMigrationHelper.AddBlock( true, "1E4C7A55-8B92-4D30-A6F1-3C08D5B72E41".AsGuid(), null, "1D501408-CA88-4565-8822-BD318F255A59".AsGuid(), "F6D0A258-F97E-4561-B881-ACBF985F89DC".AsGuid(), "Schedule Preferences", "Main", @"", @"", 1, "4B81C2D6-5E37-4A90-8F14-6C2093BD75AE" );
            RockMigrationHelper.AddBlock( true, "2F7B3C88-4D61-4E29-B0A5-9E13F6C48D72".AsGuid(), null, "1D501408-CA88-4565-8822-BD318F255A59".AsGuid(), "7258A210-E936-4260-B573-9FA1193AD9E2".AsGuid(), "Serving Nav", "Main", @"", @"", 0, "90D6172B-A38C-4F45-D469-B175E8021CA3" );
            RockMigrationHelper.AddBlock( true, "2F7B3C88-4D61-4E29-B0A5-9E13F6C48D72".AsGuid(), null, "1D501408-CA88-4565-8822-BD318F255A59".AsGuid(), "CA27CB14-22FD-4DE6-9C3B-0EAA0AA84708".AsGuid(), "Sign Up for Serving", "Main", @"", @"", 1, "5C92D3E7-6F48-4B01-9025-7D31A4CE86BF" );
            RockMigrationHelper.AddBlock( true, "3A6D9E14-7C25-4F83-91B6-5D420AE7C193".AsGuid(), null, "1D501408-CA88-4565-8822-BD318F255A59".AsGuid(), "7258A210-E936-4260-B573-9FA1193AD9E2".AsGuid(), "Serving Nav", "Main", @"", @"", 0, "A1E7283C-B49D-4056-E570-C286F91302B4" );
            RockMigrationHelper.AddBlock( true, "3A6D9E14-7C25-4F83-91B6-5D420AE7C193".AsGuid(), null, "1D501408-CA88-4565-8822-BD318F255A59".AsGuid(), "AEFF246D-A514-4D46-801E-D717E1D1D209".AsGuid(), "Schedule Unavailability", "Main", @"", @"", 1, "6DA3E4F8-7059-4C12-A136-8E42B5DF97C0" );
            RockMigrationHelper.AddBlock( true, "A3F7C21D-8E64-4B09-95A2-7D0E1F63B458".AsGuid(), null, "1D501408-CA88-4565-8822-BD318F255A59".AsGuid(), "7258A210-E936-4260-B573-9FA1193AD9E2".AsGuid(), "Podcast Feed List", "Main", @"", @"", 0, "C519E24F-A086-4D2B-B7C4-9F203B85D67A" );
            RockMigrationHelper.AddBlock( true, "B4081D3E-9F75-4C1A-A6B3-8E1F2A74C569".AsGuid(), null, "1D501408-CA88-4565-8822-BD318F255A59".AsGuid(), "7258A210-E936-4260-B573-9FA1193AD9E2".AsGuid(), "Podcast Feed Episode", "Main", @"", @"", 0, "D62AF350-B197-4E3C-98D5-A0314C96E78B" );

            //
            // Block attribute values
            //
            RockMigrationHelper.AddBlockAttributeValue( "D3C3432F-78C9-4891-9AC4-0C0E6329DCB4", "5682EDBF-68DA-4B43-A593-6C2B936C2839", @"{%- assign homeChannelGuid = '122CAAAE-0698-4869-89FE-D818E109BAEA' -%}
{%- assign homeChannelId = 0 -%}
{% contentchannel where:'Guid == ""{{ homeChannelGuid }}""' securityenabled:'false' %}{%- for ch in contentchannelItems -%}{%- assign homeChannelId = ch.Id -%}{%- endfor -%}{% endcontentchannel %}
<VerticalStackLayout>
  <!-- Typed-card feed -->
  {%- contentchannelitem where:'ContentChannelId == ""{{ homeChannelId }}"" && StartDateTime < ""{{ 'Now' | Date }}"" && ExpireDateTime > ""{{ 'Now' | Date }}"" || ExpireDateTime _= """" && ContentChannelId == ""{{ homeChannelId }}"" && StartDateTime < ""{{ 'Now' | Date }}""' sort:'Order' -%}
  {%- for item in contentchannelitemItems -%}
  {%- comment -%} ===== If an Event Item is linked, drive the card from the event ===== {%- endcomment -%}
  {%- assign appRoot = 'Global' | Attribute:'PublicApplicationRoot' -%}
  {%- assign evGuid = item | Attribute:'EventItem','RawValue' -%}
  {%- assign isEvent = false -%}{%- assign evName = '' -%}{%- assign evImg = '' -%}
  {%- assign evStart = '' -%}{%- assign evEnd = '' -%}{%- assign occGuid = '' -%}
  {%- if evGuid != '' -%}
    {% eventitem where:'Guid == ""{{ evGuid }}""' securityenabled:'false' %}
      {%- for ev in eventitemItems -%}
        {%- assign isEvent = true -%}
        {%- assign evName = ev.Name -%}
        {%- if ev.PhotoId and ev.PhotoId != '' -%}{%- assign evImg = appRoot | Append:'GetImage.ashx?Id=' | Append:ev.PhotoId -%}{%- endif -%}
        {%- comment -%} Pick the soonest FUTURE occurrence. DatesFromICal returns dates from now
             onward, so a past-only schedule yields nothing and is skipped. Never trust
             NextStartDateTime for ordering - it is persisted and can be stale. {%- endcomment -%}
        {%- assign bestDay = 0 -%}{%- assign bestMin = 0 -%}{%- assign fallbackGuid = '' -%}
        {%- for o in ev.EventItemOccurrences -%}
          {%- if fallbackGuid == '' -%}{%- assign fallbackGuid = o.Guid -%}{%- endif -%}
          {%- assign ical = o.Schedule.iCalendarContent -%}
          {%- if ical and ical != '' -%}
            {%- assign oStart = ical | DatesFromICal:1 | First -%}
            {%- if oStart and oStart != '' -%}
              {%- assign oDay = oStart | Date:'yyyyMMdd' | AsInteger -%}
              {%- assign oMin = oStart | Date:'HHmm' | AsInteger -%}
              {%- assign isBetter = false -%}
              {%- if bestDay == 0 -%}{%- assign isBetter = true -%}
              {%- elsif oDay < bestDay -%}{%- assign isBetter = true -%}
              {%- elsif oDay == bestDay and oMin < bestMin -%}{%- assign isBetter = true -%}
              {%- endif -%}
              {%- if isBetter -%}
                {%- assign bestDay = oDay -%}{%- assign bestMin = oMin -%}
                {%- assign evStart = oStart -%}
                {%- assign evEnd = ical | DatesFromICal:1,'enddatetime' | First -%}
                {%- assign occGuid = o.Guid -%}
              {%- endif -%}
            {%- endif -%}
          {%- endif -%}
        {%- endfor -%}
        {%- comment -%} no future occurrence: still link somewhere sane, just show no date {%- endcomment -%}
        {%- if occGuid == '' -%}{%- assign occGuid = fallbackGuid -%}{%- endif -%}
      {%- endfor -%}
    {% endeventitem %}
  {%- endif -%}

  {%- comment -%} ===== Linked Content Item: build the card from another item ===== {%- endcomment -%}
  {%- assign appPageForLinked = item | Attribute:""LinktoAppPage"",""RawValue"" -%}
  {%- assign lnRaw = item | Attribute:'LinkedItem','RawValue' -%}
  {%- assign isLinked = false -%}{%- assign lnTitle = '' -%}{%- assign lnImg = '' -%}
  {%- assign lnSummary = '' -%}{%- assign lnId = '' -%}{%- assign lnPage = '' -%}
  {%- comment -%} ""unless isEvent"" rather than ""isEvent == false"": assign stores a
      string, so comparing against the boolean literal never matches {%- endcomment -%}
  {%- unless isEvent -%}
  {%- if lnRaw != '' and lnRaw != null -%}
    {%- comment -%} the picker stores a Guid; a bare Id typed by hand also works {%- endcomment -%}
    {%- if lnRaw contains '-' -%}{%- capture lnWhere -%}Guid == ""{{ lnRaw }}""{%- endcapture -%}
    {%- else -%}{%- capture lnWhere -%}Id == {{ lnRaw }}{%- endcapture -%}{%- endif -%}
    {% contentchannelitem where:'{{ lnWhere }}' securityenabled:'false' %}
      {%- comment -%} iterate INSIDE the block - the collection does not survive
          past endcontentchannelitem, which is why the event code does the same {%- endcomment -%}
      {%- for li in contentchannelitemItems -%}
        {%- assign isLinked = true -%}
        {%- assign lnId = li.Id -%}
        {%- assign lnTitle = li.Title -%}
        {%- assign lnSummary = li | Attribute:'Summary' -%}
        {%- if lnSummary == '' or lnSummary == null -%}
          {%- assign lnSummary = li.Content | StripHtml | Trim -%}
        {%- endif -%}
        {%- assign lnImg = li | Attribute:'Image','RawValue' -%}
        {%- if lnImg == '' or lnImg == null -%}
          {%- for lp in li.ParentItems limit:1 -%}
            {%- assign lnImg = lp.ContentChannelItem | Attribute:'SeriesImage','RawValue' -%}
            {%- if lnImg == '' or lnImg == null -%}
              {%- assign lnImg = lp.ContentChannelItem | Attribute:'SeriesImageLink','RawValue' -%}
            {%- endif -%}
          {%- endfor -%}
        {%- endif -%}
        {%- if lnImg != '' and lnImg != null -%}
          {%- unless lnImg contains 'http' -%}
            {%- assign lnImg = appRoot | Append:'GetImage.ashx?Guid=' | Append:lnImg -%}
          {%- endunless -%}
        {%- endif -%}
        {%- comment -%} destination comes from the linked item's own channel, so no
            install-specific channel-to-page map is baked in {%- endcomment -%}
        {%- if li.ContentChannel.IsStructuredContent -%}
          {%- assign lnPage = 'bb429c19-052f-4537-a40b-a157016b341b' -%}
        {%- else -%}
          {%- assign lnPage = 'c9d8bd2d-8f1e-42e6-a4c3-b71b0511e9c7' -%}
        {%- endif -%}
        {%- if appPageForLinked != '' and appPageForLinked != null -%}{%- assign lnPage = appPageForLinked -%}{%- endif -%}
      {%- endfor -%}
    {% endcontentchannelitem %}
  {%- endif -%}
  {%- endunless -%}
  {%- comment -%} friendly date line: multi-day / all day / same day range {%- endcomment -%}
  {%- capture evWhen -%}
    {%- assign startDay = evStart | Date:'MMM dd' -%}
    {%- assign endDay = evEnd | Date:'MMM dd' -%}
    {%- assign startTime = evStart | Date:'h:mm tt' -%}
    {%- assign endTime = evEnd | Date:'h:mm tt' -%}
    {%- if startTime == '12:00 AM' and endDay == startDay -%}
      {{ startDay }}, ALL DAY
    {%- elsif endDay != '' and endDay != startDay -%}
      {{ startDay }}, {{ startTime }} - {{ endDay }}, {{ endTime }}
    {%- elsif endTime != '' and endTime != startTime -%}
      {{ startDay }}, {{ startTime }} - {{ endTime }}
    {%- else -%}
      {{ startDay }}, {{ startTime }}
    {%- endif -%}
  {%- endcapture -%}

    {%- assign linkUrlType = item | Attribute:""LinktoURLType"" -%}
  {%- assign linkToUrl = item | Attribute:""LinktoURL"" -%}{% assign authenticatedUser = item | Attribute:'PassUserAuthentication' | AsBoolean %}
  {%- assign appPage = item | Attribute:""LinktoAppPage"",""RawValue"" -%}{% assign contentStripped = item.Content | StripHtml | Trim %}
  {%- assign pageParameters = item | Attribute:""PageParameters"",""RawValue"" -%}
  {%- capture command -%}
  {%- if linkUrlType == 'External Browser' and appPage == '' -%}
  {Binding OpenExternalBrowser}{%- elseif linkUrlType == 'Internal Browser' and appPage == '' -%}
  {Binding OpenBrowser}{%- elseif linkToUrl != '' or appPage != '' or contentStripped != '' -%}
  {Binding PushPage}{%- endif -%}{%- endcapture -%}
  {% if authenticatedUser %}{%- capture personToken %}{% if linkToUrl contains '?' %}&{% else %}?{% endif %}rckipid={{ CurrentPerson | PersonTokenCreate }}{% endcapture -%}{% endif %}
  {%- capture linkUrl -%}
  {%- if linkToUrl != '' and linkUrlType == 'Webview' -%}
  c543ef2a-df73-4c21-bb01-94f2a6cb6373?url={{ linkToUrl | Append:personToken | UrlEncode -}}
  {%- elseif linkToUrl != '' -%}
  {{- linkToUrl | Append:personToken | Escape -}}
  {%- elseif appPage != '' -%}
  {{- appPage -}}{% if pageParameters and pageParameters != '' %}{{ pageParameters }}{% endif %}
  {%- elseif contentStripped != '' %}
  c9d8bd2d-8f1e-42e6-a4c3-b71b0511e9c7?ContentChannelItemId={{ item.Id -}}
  {%- endif -%}
  {%- endcapture -%}
  {% assign image = item | Attribute:'Image','Url' %}{% assign imageUrl = item | Attribute:'ImageUrl' %}{% assign subtitle = item | Attribute:'Subtitle' %}{% assign cardWidth = item | Attribute:'CardWidth' %}{% assign elevation = item | Attribute:'ShadowDepth' %}{% assign showDetailsButton = item | Attribute:'DisplayDetailsButton' | AsBoolean %}{% assign showTitle = item | Attribute:'ShowTitle' | AsBoolean %}{% assign tagline = item | Attribute:'Tagline' %}{% assign imageAspectRatio = item | Attribute:'ImageAspectRatio' %}
  {%- if isEvent -%}
  <Rock:StyledBorder StrokeThickness=""0"" Padding=""0"" CornerRadius=""16"" StyleClass=""bg-interface-softest,mx-16,my-8"">
      <Rock:StyledBorder.Shadow><Shadow Brush=""#000000"" Offset=""0,4"" Radius=""12"" Opacity=""0.15"" /></Rock:StyledBorder.Shadow>
      <Rock:StyledBorder.GestureRecognizers>
          <TapGestureRecognizer Command=""{Binding PushPage}"" CommandParameter=""a2156601-d477-465e-addf-e745dee935f5?EventOccurrenceGuid={{ occGuid }}"" />
      </Rock:StyledBorder.GestureRecognizers>
      <VerticalStackLayout Spacing=""0"">
          <!-- orange upcoming-event tag -->
          <Rock:StyledBorder BackgroundColor=""{Rock:PaletteColor App-Primary-Strong}"" CornerRadius=""0"" StrokeThickness=""0"" Padding=""0"">
              <Label Text=""UPCOMING EVENT"" TextColor=""#FFFFFF"" StyleClass=""caption2, bold""
                  HorizontalOptions=""Center"" HorizontalTextAlignment=""Center"" Margin=""0,5"" />
          </Rock:StyledBorder>
          {%- if evImg != '' -%}
          <Rock:Image Source=""{{ evImg | Escape }}"" Aspect=""AspectFill"" Ratio=""16:9"" />
          {%- endif -%}
          <VerticalStackLayout StyleClass=""p-16"" Spacing=""4"">
              <Grid ColumnDefinitions=""Auto, *"" ColumnSpacing=""10"">
                  <Rock:Icon Grid.Column=""0"" IconClass=""calendar-alt"" IconFamily=""FontAwesomeSolid"" FontSize=""20""
                      StyleClass=""text-interface-strongest"" VerticalOptions=""Start"" Margin=""0,3,0,0"" />
                  <VerticalStackLayout Grid.Column=""1"" Spacing=""2"">
                      <Label Text=""{{ evName | Escape }}"" StyleClass=""title3,text-interface-strongest"" />
                      {%- if evStart != '' -%}
                      <Label Text=""{{ evWhen | Trim }}"" StyleClass=""footnote,text-interface-soft"" />
                      {%- endif -%}
                  </VerticalStackLayout>
              </Grid>
          </VerticalStackLayout>
      </VerticalStackLayout>
  </Rock:StyledBorder>
  {%- elsif isLinked -%}
  <Rock:StyledBorder StrokeThickness=""0"" Padding=""0""
          CornerRadius=""{% if cardWidth == 'Full' %}0{% else %}16{% endif %}""
          StyleClass=""bg-interface-softest{% if cardWidth != 'Full' %},mx-16,my-8{% endif %}"">
      {%- if elevation != '' and elevation != '0' -%}
      <Rock:StyledBorder.Shadow><Shadow Brush=""#000000"" Offset=""0,4"" Radius=""12"" Opacity=""0.15"" /></Rock:StyledBorder.Shadow>
      {%- endif -%}
      <Rock:StyledBorder.GestureRecognizers>
          <TapGestureRecognizer Command=""{Binding PushPage}"" CommandParameter=""{{ lnPage }}?ContentChannelItemId={{ lnId }}"" />
      </Rock:StyledBorder.GestureRecognizers>
      <VerticalStackLayout Spacing=""0"">
          {%- comment -%} the feed item's own image wins if one was set, so a card
              can be given custom art without breaking the link {%- endcomment -%}
          {%- assign cardImg = '' -%}
          {%- if imageUrl != '' -%}{%- assign cardImg = imageUrl -%}
          {%- elsif image != '' -%}{%- assign cardImg = image -%}
          {%- elsif lnImg != '' -%}{%- assign cardImg = lnImg -%}{%- endif -%}
          {%- if cardImg != '' -%}
          <Rock:Image Source=""{{ cardImg | Escape }}"" Aspect=""AspectFill"" Ratio=""{% if imageAspectRatio != '' %}{{ imageAspectRatio }}{% else %}16:9{% endif %}"" />
          {%- endif -%}
          <VerticalStackLayout StyleClass=""p-16"" Spacing=""4"">
              {%- if tagline != '' -%}
              <Label Text=""{{ tagline | Escape }}"" StyleClass=""caption1,font-weight-semi-bold,text-interface-strong"" />
              {%- endif -%}
              {%- if showTitle -%}
              <Label Text=""{% if item.Title != '' %}{{ item.Title | Escape }}{% else %}{{ lnTitle | Escape }}{% endif %}"" StyleClass=""title3,text-interface-strongest"" />
              {%- endif -%}
              {%- if subtitle != '' -%}
              <Label Text=""{{ subtitle | Escape }}"" StyleClass=""footnote,text-interface-soft"" />
              {%- endif -%}
              {%- if lnSummary != '' -%}
              <Label Text=""{{ lnSummary | TruncateWords:50 | Escape }}"" StyleClass=""subheadline,text-interface-strong,mt-4"" />
              {%- endif -%}
              {%- if showDetailsButton -%}
              <Button Text=""SEE DETAILS"" StyleClass=""btn,btn-primary,mt-12"" HorizontalOptions=""Start""
                  Command=""{Binding PushPage}"" CommandParameter=""{{ lnPage }}?ContentChannelItemId={{ lnId }}"" />
              {%- endif -%}
          </VerticalStackLayout>
      </VerticalStackLayout>
  </Rock:StyledBorder>
  {%- else -%}
  <Rock:StyledBorder StrokeThickness=""0"" Padding=""0""
          CornerRadius=""{% if cardWidth == 'Full' %}0{% else %}16{% endif %}""
          StyleClass=""bg-interface-softest{% if cardWidth != 'Full' %},mx-16,my-8{% endif %}"">
      {%- if elevation != '' and elevation != '0' -%}
      <Rock:StyledBorder.Shadow>
          <Shadow Brush=""#000000"" Offset=""0,4"" Radius=""12"" Opacity=""0.15"" />
      </Rock:StyledBorder.Shadow>
      {%- endif -%}
      <Rock:StyledBorder.GestureRecognizers>
          <TapGestureRecognizer Command=""{{ command }}"" CommandParameter=""{{ linkUrl }}"" />
      </Rock:StyledBorder.GestureRecognizers>
      <VerticalStackLayout Spacing=""0"">
          {%- if imageUrl != '' or image != '' -%}
          <Rock:Image Source=""{% if imageUrl != '' %}{{ imageUrl | Escape }}{% else %}{{ image | Escape }}{% endif %}"" Aspect=""AspectFill"" Ratio=""{% if imageAspectRatio != '' %}{{ imageAspectRatio }}{% else %}16:9{% endif %}"" />
          {%- endif -%}
          <VerticalStackLayout StyleClass=""p-16"" Spacing=""4"">
              {%- if tagline != '' -%}
              <Label Text=""{{ tagline | Escape }}"" StyleClass=""caption1,font-weight-semi-bold,text-interface-strong"" />
              {%- endif -%}
              {%- if showTitle -%}
              <Label Text=""{{ item.Title | Escape }}"" StyleClass=""title3,text-interface-strongest"" />
              {%- endif -%}
              {%- if subtitle != '' -%}
              <Label Text=""{{ subtitle | Escape }}"" StyleClass=""footnote,text-interface-soft"" />
              {%- endif -%}
              {%- if showDetailsButton -%}
              <Label Text=""{{ item.Content | StripHtml | TruncateWords:30 | Escape }}"" StyleClass=""subheadline,text-interface-strong,mt-8"" />
              <Button Text=""SEE DETAILS"" StyleClass=""btn,btn-primary,mt-12"" HorizontalOptions=""Start"" Command=""{{ command }}"" CommandParameter=""{{ linkUrl }}"" />
              {%- elseif contentStripped != '' -%}
              <Label Text=""{{ item.Content | StripHtml | TruncateWords:50 | Escape }}"" StyleClass=""subheadline,text-interface-strong,mt-4"" />
              {%- endif -%}
          </VerticalStackLayout>
      </VerticalStackLayout>
  </Rock:StyledBorder>
  {%- endif -%}
  {% endfor %}
  {% endcontentchannelitem %}

</VerticalStackLayout>" );   // Content
            RockMigrationHelper.AddBlockAttributeValue( "D3C3432F-78C9-4891-9AC4-0C0E6329DCB4", "24516448-3F1F-4F27-97A1-CFB4F8B277B5", @"RockEntity" );   // EnabledLavaCommands
            RockMigrationHelper.AddBlockAttributeValue( "D3C3432F-78C9-4891-9AC4-0C0E6329DCB4", "B31D29A0-3725-4AEB-8360-7D91B9CDFE47", @"True" );   // DynamicContent
            RockMigrationHelper.AddBlockAttributeValue( "85EF791C-9F1A-4EB0-B2FB-DA53256E9848", "5682EDBF-68DA-4B43-A593-6C2B936C2839", @"<StackLayout StyleClass=""p-16""
    Spacing=""16"">
 
    <Label Text=""A webview will be inserted here in the end, but it is causing an ANR with the emulator for now.""
        StyleClass=""h4"" />
    
</StackLayout>" );   // Content
            RockMigrationHelper.AddBlockAttributeValue( "85EF791C-9F1A-4EB0-B2FB-DA53256E9848", "B31D29A0-3725-4AEB-8360-7D91B9CDFE47", @"False" );   // DynamicContent
            RockMigrationHelper.AddBlockAttributeValue( "DBD2ABC9-945F-457B-8B0F-80F0A87792F0", "5682EDBF-68DA-4B43-A593-6C2B936C2839", @"<Rock:WebView Source=""{{ PageParameter.url | UrlDecode }}"" />" );   // Content
            RockMigrationHelper.AddBlockAttributeValue( "DBD2ABC9-945F-457B-8B0F-80F0A87792F0", "B31D29A0-3725-4AEB-8360-7D91B9CDFE47", @"True" );   // DynamicContent
            RockMigrationHelper.AddBlockAttributeValue( "8B02B9CD-0474-4D79-AEEB-0F91407713CA", "D80CF7C7-F6F4-4E77-97A8-B0842E4AF7FB", @"<VerticalStackLayout>
{%- comment -%}
    ===========================================================================
    ITEM DETAIL
    ---------------------------------------------------------------------------
    Every attribute below is optional and read BY KEY, so any content channel
    routing here can opt in just by defining the key. Undefined attributes come
    back empty, which is always the ""off"" state - nothing renders.

    Call to action
      LinkUrl              open a URL          -> OpenBrowser / OpenExternalBrowser
      LinkAppPage          open a page IN APP  -> PushPage
      LinkPageParameters   query string for LinkAppPage, with or without a leading ?
      LinkButtonText       button label, defaults to ""Learn More""
      LinkOpensExternally  URL mode only: hand off to the device browser

    LinkAppPage WINS when both it and LinkUrl are set - an in-app destination is
    the better experience, and having both set is a config mistake rather than a
    thing to render twice. It is a Page Reference, so it stores a bare page guid,
    which is exactly what PushPage expects. Point it at a page holding a WebView
    or Workflow Entry block and the button opens a web page or starts a workflow.

    Share (ShareMode)
      None / empty   no share affordance                    (default)
      Item           share this item's PUBLIC WEB page  -> {{ appRoot }}item/{Id}
      Link           share whatever LinkUrl points at

    ShareMode=Item depends on a public web route that does NOT exist yet - see
    section 2 of PRODUCTION-SETUP-CHECKLIST.md. Until it is built the share still
    fires, it just hands out a 404, so leave ShareMode empty on production items.
    ShareMode=Link renders nothing when the item uses LinkAppPage instead of
    LinkUrl, since there is no external URL to hand out.

    Display
      ShowDetailTitle  default on. Hides the LARGE in-body title only - the nav
                       bar keeps its title either way, so the page never loses
                       its label.
      ShowDetailDate   default on. Hides the date; the speaker still shows if
                       set. The whole meta line disappears only when both are
                       absent.

    NOT named ShowTitle/ShowDate on purpose. App Home Feed already has a
    ShowTitle attribute meaning ""show the title on the CARD"", consumed by the
    home feed block. Reusing that key would have made hiding the detail title
    also blank the card on the home feed - two different switches wired to one
    value. The Detail prefix keeps them independent.

    Both are ""hide only when explicitly False"" so that channels which never
    define them keep the original layout.
    ===========================================================================
{%- endcomment -%}
{%- assign appRoot = 'Global' | Attribute:'PublicApplicationRoot' -%}
{%- assign webItemRoute = 'item/' -%}

{%- assign linkAppPage = Item | Attribute:'LinkAppPage','RawValue' -%}
{%- assign linkUrl = Item | Attribute:'LinkUrl','RawValue' -%}
{%- assign ctaCommand = '' -%}
{%- assign ctaParam = '' -%}
{%- if linkAppPage != '' and linkAppPage != null -%}
    {%- assign ctaCommand = 'PushPage' -%}
    {%- assign linkParams = Item | Attribute:'LinkPageParameters','RawValue' -%}
    {%- comment -%} accept ""?a=1"" or ""a=1"": drop any ? then re-add exactly one {%- endcomment -%}
    {%- assign linkParams = linkParams | Replace:'?','' -%}
    {%- capture ctaParam -%}{{ linkAppPage }}{% if linkParams != '' and linkParams != null %}?{{ linkParams }}{% endif %}{%- endcapture -%}
{%- elsif linkUrl != '' and linkUrl != null -%}
    {%- assign linkExternal = Item | Attribute:'LinkOpensExternally','RawValue' -%}
    {%- if linkExternal == 'True' -%}
        {%- assign ctaCommand = 'OpenExternalBrowser' -%}
    {%- else -%}
        {%- assign ctaCommand = 'OpenBrowser' -%}
    {%- endif -%}
    {%- assign ctaParam = linkUrl -%}
{%- endif -%}

{%- assign shareMode = Item | Attribute:'ShareMode','RawValue' -%}
{%- assign shareUri = '' -%}
{%- if shareMode == 'Item' -%}
    {%- capture shareUri -%}{{ appRoot }}{{ webItemRoute }}{{ Item.Id }}{%- endcapture -%}
{%- elsif shareMode == 'Link' -%}
    {%- assign shareUri = linkUrl -%}
{%- endif -%}

{%- assign showTitle = Item | Attribute:'ShowDetailTitle','RawValue' -%}
{%- assign showDate = Item | Attribute:'ShowDetailDate','RawValue' -%}
{%- assign speaker = Item | Attribute:'Speaker' -%}

        <VerticalStackLayout StyleClass=""bg-interface-softest"" Spacing=""0"">
        <VerticalStackLayout.Behaviors>
            <Rock:SafeAreaPaddingBehavior Edges=""Top"" />
        </VerticalStackLayout.Behaviors>

        <Grid ColumnDefinitions=""56, *, 56"" ColumnSpacing=""0"" Padding=""16,16"">
            <Rock:Icon Grid.Column=""0""
                IconClass=""arrow-left""
                IconFamily=""MaterialDesignIcons""
                FontSize=""24""
                StyleClass=""text-interface-strongest""
                VerticalOptions=""Center""
                VerticalTextAlignment=""Center""
                HorizontalOptions=""Start"">
                <Rock:Icon.GestureRecognizers>
                    <TapGestureRecognizer Command=""{Binding PopPage}"" />
                </Rock:Icon.GestureRecognizers>
            </Rock:Icon>

            <Label Grid.Column=""1""
                StyleClass=""title3, font-weight-semi-bold, text-interface-strongest""
                Text=""{{ Item.Title | Escape }}""
                VerticalOptions=""Center""
                VerticalTextAlignment=""Center""
                LineBreakMode=""TailTruncation"" />
            {%- if shareUri != '' and shareUri != null -%}
            <Rock:Icon Grid.Column=""2""
                IconClass=""share-square""
                IconFamily=""FontAwesomeSolid""
                FontSize=""22""
                StyleClass=""text-interface-strongest""
                VerticalOptions=""Center""
                VerticalTextAlignment=""Center""
                HorizontalOptions=""End"">
                <Rock:Icon.GestureRecognizers>
                    <TapGestureRecognizer Command=""{Binding ShareContent}"">
                        <TapGestureRecognizer.CommandParameter>
                            <Rock:ShareContentParameters
                                Title=""{{ Item.Title | Escape }}""
                                Text=""{{ Item.Title | Escape }}{% if speaker != '' %} - {{ speaker | Escape }}{% endif %}""
                                Uri=""{{ shareUri | Escape }}"" />
                        </TapGestureRecognizer.CommandParameter>
                    </TapGestureRecognizer>
                </Rock:Icon.GestureRecognizers>
            </Rock:Icon>
            {%- endif -%}
        </Grid>
    </VerticalStackLayout>

    <VerticalStackLayout>
      {%- assign image = Item | Attribute:'Image','Url' -%}
      {%- assign imageUrl = Item | Attribute:'ImageUrl' -%}
      {%- if imageUrl != '' or image != '' -%}
      <Rock:Image Source=""{% if imageUrl != '' %}{{ imageUrl | Escape }}{% else %}{{ image | Escape }}{% endif %}"" Aspect=""AspectFill"" Ratio=""16:9"" />
      {%- endif -%}
      <VerticalStackLayout StyleClass=""p-16"" Spacing=""8"">
        {%- assign tagline = Item | Attribute:'Tagline' -%}
        {%- if tagline != '' -%}<Label Text=""{{ tagline | Escape }}"" StyleClass=""caption1,font-weight-semi-bold,text-interface-strong"" />{%- endif -%}
        {%- if showTitle != 'False' -%}
        <Label Text=""{{ Item.Title | Escape }}"" StyleClass=""title1,text-interface-strongest"" />
        {%- endif -%}
        {%- capture meta -%}{% if showDate != 'False' %}{{ Item.StartDateTime | Date:'MMMM d, yyyy' }}{% endif %}{% if speaker != '' %}{% if showDate != 'False' %} | {% endif %}{{ speaker | Escape }}{% endif %}{%- endcapture -%}
        {%- assign metaTrimmed = meta | Trim -%}
        {%- if metaTrimmed != '' -%}
        <Label Text=""{{ metaTrimmed }}"" StyleClass=""footnote,text-interface-medium,mb-8"" />
        {%- endif -%}
        {%- comment -%} CDATA rather than Text+Escape so the markup reaches Rock:Html intact,
            and FollowHyperlinks so anchors in the body are tappable (the property defaults
            to false on Rock.Mobile.Cms.Html, so links are inert without it). {%- endcomment -%}
        <Rock:Html FollowHyperlinks=""true"">
        <![CDATA[
        {{ Item.Content }}
        ]]>
        </Rock:Html>

        {%- if ctaCommand != '' -%}
            {%- assign linkText = Item | Attribute:'LinkButtonText' -%}
            {%- if linkText == '' or linkText == null -%}{%- assign linkText = 'Learn More' -%}{%- endif -%}
            <Button Text=""{{ linkText | Escape }}""
                Command=""{Binding {{ ctaCommand }}}""
                CommandParameter=""{{ ctaParam | Escape }}""
                StyleClass=""btn, btn-primary""
                HorizontalOptions=""Center""
                Margin=""0,16,0,0"" />
        {%- endif -%}
      </VerticalStackLayout>
    </VerticalStackLayout>
</VerticalStackLayout>
" );   // ContentTemplate
            RockMigrationHelper.AddBlockAttributeValue( "8B02B9CD-0474-4D79-AEEB-0F91407713CA", "616351D9-41FD-4E84-9378-78140BE30605", @"False" );   // LogInteractions
            RockMigrationHelper.AddBlockAttributeValue( "783BB975-D313-4722-A444-D3FF6EE06B3B", "61B98E57-B508-4384-9606-8A4D6E827658", @"16e97046-c04f-4388-8aee-d5c1cf4a19c8" );   // RegistrationPage
            RockMigrationHelper.AddBlockAttributeValue( "783BB975-D313-4722-A444-D3FF6EE06B3B", "0036807C-7742-48DE-BAD4-E025DE37A215", @"https://members.nfluencechurch.org/ForgotUserName" );   // ForgotPasswordUrl
            RockMigrationHelper.AddBlockAttributeValue( "545B0BD2-51C8-48D6-A680-57B50D79454C", "5682EDBF-68DA-4B43-A593-6C2B936C2839", @"<VerticalStackLayout>
  {% webrequest url:'https://webevents.resi.io/api/v1/eventprofiles/latest/fce4196f-82ed-49e8-b726-f290e7c02dab' %}
    {%- assign streamUrl = results.cloud.hlsUrl -%}
    {%- if streamUrl and streamUrl != '' -%}
    <Rock:RatioView Ratio=""16:9"">
      <Rock:MediaPlayer Source=""{{ streamUrl | Escape }}"" ShouldAutoPlay=""True"" ShouldShowPlaybackControls=""True"" Aspect=""AspectFill"" />
    </Rock:RatioView>
    {%- elseif results.offlineImageUrl and results.offlineImageUrl != '' -%}
    <Rock:Image Source=""{{ results.offlineImageUrl | Escape }}"" Aspect=""AspectFill"" Ratio=""16:9"" />
    {%- else -%}
    <Rock:StyledBorder StyleClass=""bg-interface-softer,m-16"" CornerRadius=""16"" Padding=""24"">
      <Label Text=""The live stream is currently offline."" StyleClass=""body,text-interface-strong"" HorizontalTextAlignment=""Center"" />
    </Rock:StyledBorder>
    {%- endif -%}
  {% endwebrequest %}
  <VerticalStackLayout StyleClass=""p-16"" Spacing=""8"">
    <Label Text=""Live Stream"" StyleClass=""title2,text-interface-strongest"" />
    <Label Text=""Join us LIVE every Sunday at 9 &amp; 11 AM. Can't make it in person? Worship with us online."" StyleClass=""body,text-interface-strong"" />

    <!-- ===== Invite someone: prominent share ===== -->
    {%- assign liveShareUrl = 'https://live.nfluencechurch.org' -%}
    <Rock:StyledBorder CornerRadius=""12"" Padding=""0"" BackgroundColor=""{Rock:PaletteColor App-Primary-Strong}""
        HorizontalOptions=""Fill"" Margin=""0,12,0,0"">
      <Rock:StyledBorder.GestureRecognizers>
        <TapGestureRecognizer Command=""{Binding ShareContent}"">
          <TapGestureRecognizer.CommandParameter>
            <Rock:ShareContentParameters
              Title=""Nfluence Church Live""
              Text=""Join me for church online at Nfluence Church!""
              Uri=""{{ liveShareUrl }}"" />
          </TapGestureRecognizer.CommandParameter>
        </TapGestureRecognizer>
      </Rock:StyledBorder.GestureRecognizers>
      <HorizontalStackLayout Spacing=""10"" HorizontalOptions=""Center"" Margin=""0,16"">
        <Rock:Icon IconClass=""share-square"" IconFamily=""FontAwesomeSolid"" FontSize=""20""
          TextColor=""#FFFFFF"" VerticalOptions=""Center"" />
        <Label Text=""INVITE SOMEONE TO WATCH"" TextColor=""#FFFFFF"" StyleClass=""body, bold"" VerticalOptions=""Center"" />
      </HorizontalStackLayout>
    </Rock:StyledBorder>
  </VerticalStackLayout>
</VerticalStackLayout>
" );   // Content
            RockMigrationHelper.AddBlockAttributeValue( "545B0BD2-51C8-48D6-A680-57B50D79454C", "24516448-3F1F-4F27-97A1-CFB4F8B277B5", @"WebRequest" );   // EnabledLavaCommands
            RockMigrationHelper.AddBlockAttributeValue( "545B0BD2-51C8-48D6-A680-57B50D79454C", "B31D29A0-3725-4AEB-8360-7D91B9CDFE47", @"True" );   // DynamicContent
            RockMigrationHelper.AddBlockAttributeValue( "F8DA4BE8-D31D-479F-9306-F9E0CD450A86", "5682EDBF-68DA-4B43-A593-6C2B936C2839", @"<!--
  Reusable Menu / List template (Nfluence Rock Mobile)
  ONE template drives: curated recursive menus (Sundays, Explore, sub-menus)
  AND data-driven lists (Sermon Notes, Staff) by pointing at a different channel.

  Config block below — set per placement:
    menuChannelId : App Menu channel Id (for a data-list, the data channel's Id)
    menuKey       : curated menu -> which menu to show (Sundays tab='sundays', Explore='explore');
                    sub-menu pages read it from PageParameter.MenuKey
    forcedStyle   : data-list -> force one style for every row (e.g. 'Meta Row', 'Avatar Row');
                    leave BLANK for a curated menu (per-item DisplayStyle)
    sortBy        : 'Order' for menus; 'StartDateTime desc' for sermons
  Block settings: Dynamic Content = Yes, Rock Entity command enabled.
-->
<VerticalStackLayout Spacing=""0"" StyleClass=""pt-12,pb-16"">

{%- assign menuChannelGuid = 'DF6276E8-B00E-4E77-88D8-AB1241A4A8C7' -%}
{%- assign menuChannelId = '' -%}
{% contentchannel where:'Guid == ""{{ menuChannelGuid }}""' securityenabled:'false' %}{%- for ch in contentchannelItems -%}{%- assign menuChannelId = ch.Id -%}{%- endfor -%}{% endcontentchannel %}
{%- assign menuKey = PageParameter.MenuKey | Default:'sundays' -%}
{%- assign forcedStyle = '' -%}
{%- assign sortBy = 'Order' -%}

{%- assign menuPageGuid = 'a93cfc38-98f5-41de-b68d-6e7ee97f2d46' -%}
{%- assign itemDetailGuid = 'c9d8bd2d-8f1e-42e6-a4c3-b71b0511e9c7' -%}
{%- assign webviewGuid = 'c543ef2a-df73-4c21-bb01-94f2a6cb6373' -%}
{%- assign isListMode = false -%}
{%- if forcedStyle != '' -%}{% assign isListMode = true %}{%- endif -%}

{%- contentchannelitem where:'ContentChannelId == ""{{ menuChannelId }}""' sort:'{{ sortBy }}' -%}
{%- for item in contentchannelitemItems -%}
  {%- assign itemMenu = item | Attribute:'Menu','RawValue' -%}
  {%- if isListMode or itemMenu == menuKey -%}

  {%- if isListMode -%}{% assign style = forcedStyle %}{%- else -%}{% assign style = item | Attribute:'DisplayStyle','RawValue' %}{%- endif -%}
  {%- assign icon = item | Attribute:'Icon' -%}
  {%- assign subtitle = item | Attribute:'Subtitle' -%}
  {%- assign img = item | Attribute:'Image','Url' -%}
  {%- assign imgUrl = item | Attribute:'ImageUrl' -%}
  {%- if imgUrl == '' -%}{% assign imgUrl = img %}{%- endif -%}

  {%- assign subMenu = item | Attribute:'OpensSubMenu','RawValue' -%}
  {%- assign linkType = item | Attribute:'LinktoURLType','RawValue' -%}
  {%- assign linkUrlVal = item | Attribute:'LinktoURL' -%}
  {%- assign appPage = item | Attribute:'LinktoAppPage','RawValue' -%}
  {%- assign pageParams = item | Attribute:'PageParameters' -%}
  {%- assign contentStripped = item.Content | StripHtml | Trim -%}

  {%- capture cmd -%}
  {%- if subMenu != '' -%}{Binding PushPage}
  {%- elsif linkType == 'External Browser' -%}{Binding OpenExternalBrowser}
  {%- elsif linkType == 'Internal Browser' -%}{Binding OpenBrowser}
  {%- elsif linkUrlVal != '' or appPage != '' or contentStripped != '' -%}{Binding PushPage}
  {%- endif -%}
  {%- endcapture -%}{% assign cmd = cmd | Trim %}

  {%- capture param -%}
  {%- if subMenu != '' -%}{{ menuPageGuid }}?MenuKey={{ subMenu }}&amp;Title={{ item.Title | UrlEncode }}
  {%- elsif linkUrlVal != '' and linkType == 'Webview' -%}{{ webviewGuid }}?url={{ linkUrlVal | UrlEncode }}
  {%- elsif linkUrlVal != '' -%}{{ linkUrlVal | Escape }}
  {%- elsif appPage != '' -%}{{ appPage | Split:',' | First }}{% if pageParams != '' %}{{ pageParams }}{% endif %}
  {%- elsif contentStripped != '' -%}{{ itemDetailGuid }}?ContentChannelItemId={{ item.Id }}
  {%- endif -%}
  {%- endcapture -%}{% assign param = param | Trim %}

  {%- if style == 'Section Header' -%}
  <Label Text=""{{ item.Title | Escape }}"" StyleClass=""caption1,font-weight-semi-bold,text-interface-soft,px-16,mt-16,mb-4"" />

  {%- elsif style == 'Hero Banner' -%}
  <Rock:Image Source=""{{ imgUrl | Escape }}"" Aspect=""AspectFill"" Ratio=""16:9"" StyleClass=""mb-8"">
    {%- if cmd != '' -%}<Rock:Image.GestureRecognizers><TapGestureRecognizer Command=""{{ cmd }}"" CommandParameter=""{{ param }}"" /></Rock:Image.GestureRecognizers>{%- endif -%}
  </Rock:Image>

  {%- elsif style == 'Image Card' -%}
  <Rock:StyledBorder StrokeThickness=""0"" Padding=""0"" CornerRadius=""16"" StyleClass=""bg-interface-softest,mx-16,my-8"">
    <Rock:StyledBorder.Shadow><Shadow Brush=""#000000"" Offset=""0,4"" Radius=""12"" Opacity=""0.15"" /></Rock:StyledBorder.Shadow>
    {%- if cmd != '' -%}<Rock:StyledBorder.GestureRecognizers><TapGestureRecognizer Command=""{{ cmd }}"" CommandParameter=""{{ param }}"" /></Rock:StyledBorder.GestureRecognizers>{%- endif -%}
    <VerticalStackLayout Spacing=""0"">
      <Rock:Image Source=""{{ imgUrl | Escape }}"" Aspect=""AspectFill"" Ratio=""16:9"" />
      <VerticalStackLayout StyleClass=""p-16"" Spacing=""4"">
        <Label Text=""{{ item.Title | Escape }}"" StyleClass=""title3,text-interface-strongest"" />
        {%- if subtitle != '' -%}<Label Text=""{{ subtitle | Escape }}"" StyleClass=""footnote,text-interface-soft"" />{%- endif -%}
      </VerticalStackLayout>
    </VerticalStackLayout>
  </Rock:StyledBorder>

  {%- else -%}
  <!-- Row styles: Icon Row, Plain Row, Thumbnail Row, Avatar Row, Meta Row -->
  <Rock:StyledBorder StrokeThickness=""0"" CornerRadius=""12"" Padding=""18,{% if subtitle == '' %}22{% else %}18{% endif %}"" StyleClass=""bg-interface-softest,mx-16,my-8"">
    <Rock:StyledBorder.Shadow><Shadow Brush=""#000000"" Offset=""0,2"" Radius=""8"" Opacity=""0.10"" /></Rock:StyledBorder.Shadow>
    {%- if cmd != '' -%}<Rock:StyledBorder.GestureRecognizers><TapGestureRecognizer Command=""{{ cmd }}"" CommandParameter=""{{ param }}"" /></Rock:StyledBorder.GestureRecognizers>{%- endif -%}
    <Grid ColumnDefinitions=""Auto,*,Auto"" ColumnSpacing=""14"" VerticalOptions=""Center"">
      {%- if icon != '' and style != 'Thumbnail Row' and style != 'Avatar Row' -%}
      <Rock:Icon Grid.Column=""0"" IconClass=""{{ icon }}"" FontSize=""22"" TextColor=""{Rock:PaletteColor Interface-Strong}"" VerticalOptions=""Center"" />
      {%- elsif style == 'Thumbnail Row' and imgUrl != '' -%}
      <Rock:Image Grid.Column=""0"" Source=""{{ imgUrl | Escape }}"" WidthRequest=""56"" HeightRequest=""56"" Aspect=""AspectFill"" StyleClass=""rounded"" />
      {%- elsif style == 'Avatar Row' and imgUrl != '' -%}
      <Rock:Image Grid.Column=""0"" Source=""{{ imgUrl | Escape }}"" WidthRequest=""52"" HeightRequest=""52"" Aspect=""AspectFill"" StyleClass=""rounded-full"" />
      {%- endif -%}
      <VerticalStackLayout Grid.Column=""1"" Spacing=""2"" VerticalOptions=""Center"">
        <Label Text=""{{ item.Title | Escape }}"" StyleClass=""body,font-weight-semi-bold,text-interface-strongest"" />
        {%- if subtitle != '' -%}<Label Text=""{{ subtitle | Escape }}"" StyleClass=""footnote,text-interface-soft"" />{%- endif -%}
      </VerticalStackLayout>
      <Rock:Icon Grid.Column=""2"" IconClass=""chevron-right"" IconFamily=""FontAwesomeSolid"" FontSize=""15"" TextColor=""{Rock:PaletteColor Interface-Soft}"" VerticalOptions=""Center"" />
    </Grid>
  </Rock:StyledBorder>
  {%- endif -%}

  {%- endif -%}
{%- endfor -%}
{%- endcontentchannelitem -%}
</VerticalStackLayout>" );   // Content
            RockMigrationHelper.AddBlockAttributeValue( "F8DA4BE8-D31D-479F-9306-F9E0CD450A86", "24516448-3F1F-4F27-97A1-CFB4F8B277B5", @"RockEntity" );   // EnabledLavaCommands
            RockMigrationHelper.AddBlockAttributeValue( "F8DA4BE8-D31D-479F-9306-F9E0CD450A86", "B31D29A0-3725-4AEB-8360-7D91B9CDFE47", @"True" );   // DynamicContent
            RockMigrationHelper.AddBlockAttributeValue( "F147C24C-E1A1-4C41-968E-0F4FABCD3DE6", "5682EDBF-68DA-4B43-A593-6C2B936C2839", @"<!--
  Reusable Menu / List template (Nfluence Rock Mobile)
  ONE template drives: curated recursive menus (Sundays, Explore, sub-menus)
  AND data-driven lists (Sermon Notes, Staff) by pointing at a different channel.

  Config block below — set per placement:
    menuChannelId : App Menu channel Id (for a data-list, the data channel's Id)
    menuKey       : curated menu -> which menu to show (Sundays tab='sundays', Explore='explore');
                    sub-menu pages read it from PageParameter.MenuKey
    forcedStyle   : data-list -> force one style for every row (e.g. 'Meta Row', 'Avatar Row');
                    leave BLANK for a curated menu (per-item DisplayStyle)
    sortBy        : 'Order' for menus; 'StartDateTime desc' for sermons
  Block settings: Dynamic Content = Yes, Rock Entity command enabled.
-->
<VerticalStackLayout Spacing=""0"" StyleClass=""pt-12,pb-16"">

{%- assign menuChannelGuid = 'DF6276E8-B00E-4E77-88D8-AB1241A4A8C7' -%}
{%- assign menuChannelId = '' -%}
{% contentchannel where:'Guid == ""{{ menuChannelGuid }}""' securityenabled:'false' %}{%- for ch in contentchannelItems -%}{%- assign menuChannelId = ch.Id -%}{%- endfor -%}{% endcontentchannel %}
{%- assign menuKey = PageParameter.MenuKey | Default:'explore' -%}
{%- assign forcedStyle = '' -%}
{%- assign sortBy = 'Order' -%}

{%- assign menuPageGuid = 'a93cfc38-98f5-41de-b68d-6e7ee97f2d46' -%}
{%- assign itemDetailGuid = 'c9d8bd2d-8f1e-42e6-a4c3-b71b0511e9c7' -%}
{%- assign webviewGuid = 'c543ef2a-df73-4c21-bb01-94f2a6cb6373' -%}
{%- assign isListMode = false -%}
{%- if forcedStyle != '' -%}{% assign isListMode = true %}{%- endif -%}

{%- contentchannelitem where:'ContentChannelId == ""{{ menuChannelId }}""' sort:'{{ sortBy }}' -%}
{%- for item in contentchannelitemItems -%}
  {%- assign itemMenu = item | Attribute:'Menu','RawValue' -%}
  {%- if isListMode or itemMenu == menuKey -%}

  {%- if isListMode -%}{% assign style = forcedStyle %}{%- else -%}{% assign style = item | Attribute:'DisplayStyle','RawValue' %}{%- endif -%}
  {%- assign icon = item | Attribute:'Icon' -%}
  {%- assign subtitle = item | Attribute:'Subtitle' -%}
  {%- assign img = item | Attribute:'Image','Url' -%}
  {%- assign imgUrl = item | Attribute:'ImageUrl' -%}
  {%- if imgUrl == '' -%}{% assign imgUrl = img %}{%- endif -%}

  {%- assign subMenu = item | Attribute:'OpensSubMenu','RawValue' -%}
  {%- assign linkType = item | Attribute:'LinktoURLType','RawValue' -%}
  {%- assign linkUrlVal = item | Attribute:'LinktoURL' -%}
  {%- assign appPage = item | Attribute:'LinktoAppPage','RawValue' -%}
  {%- assign pageParams = item | Attribute:'PageParameters' -%}
  {%- assign contentStripped = item.Content | StripHtml | Trim -%}

  {%- capture cmd -%}
  {%- if subMenu != '' -%}{Binding PushPage}
  {%- elsif linkType == 'External Browser' -%}{Binding OpenExternalBrowser}
  {%- elsif linkType == 'Internal Browser' -%}{Binding OpenBrowser}
  {%- elsif linkUrlVal != '' or appPage != '' or contentStripped != '' -%}{Binding PushPage}
  {%- endif -%}
  {%- endcapture -%}{% assign cmd = cmd | Trim %}

  {%- capture param -%}
  {%- if subMenu != '' -%}{{ menuPageGuid }}?MenuKey={{ subMenu }}&amp;Title={{ item.Title | UrlEncode }}
  {%- elsif linkUrlVal != '' and linkType == 'Webview' -%}{{ webviewGuid }}?url={{ linkUrlVal | UrlEncode }}
  {%- elsif linkUrlVal != '' -%}{{ linkUrlVal | Escape }}
  {%- elsif appPage != '' -%}{{ appPage | Split:',' | First }}{% if pageParams != '' %}{{ pageParams }}{% endif %}
  {%- elsif contentStripped != '' -%}{{ itemDetailGuid }}?ContentChannelItemId={{ item.Id }}
  {%- endif -%}
  {%- endcapture -%}{% assign param = param | Trim %}

  {%- if style == 'Section Header' -%}
  <Label Text=""{{ item.Title | Escape }}"" StyleClass=""caption1,font-weight-semi-bold,text-interface-soft,px-16,mt-16,mb-4"" />

  {%- elsif style == 'Hero Banner' -%}
  <Rock:Image Source=""{{ imgUrl | Escape }}"" Aspect=""AspectFill"" Ratio=""16:9"" StyleClass=""mb-8"">
    {%- if cmd != '' -%}<Rock:Image.GestureRecognizers><TapGestureRecognizer Command=""{{ cmd }}"" CommandParameter=""{{ param }}"" /></Rock:Image.GestureRecognizers>{%- endif -%}
  </Rock:Image>

  {%- elsif style == 'Image Card' -%}
  <Rock:StyledBorder StrokeThickness=""0"" Padding=""0"" CornerRadius=""16"" StyleClass=""bg-interface-softest,mx-16,my-8"">
    <Rock:StyledBorder.Shadow><Shadow Brush=""#000000"" Offset=""0,4"" Radius=""12"" Opacity=""0.15"" /></Rock:StyledBorder.Shadow>
    {%- if cmd != '' -%}<Rock:StyledBorder.GestureRecognizers><TapGestureRecognizer Command=""{{ cmd }}"" CommandParameter=""{{ param }}"" /></Rock:StyledBorder.GestureRecognizers>{%- endif -%}
    <VerticalStackLayout Spacing=""0"">
      <Rock:Image Source=""{{ imgUrl | Escape }}"" Aspect=""AspectFill"" Ratio=""16:9"" />
      <VerticalStackLayout StyleClass=""p-16"" Spacing=""4"">
        <Label Text=""{{ item.Title | Escape }}"" StyleClass=""title3,text-interface-strongest"" />
        {%- if subtitle != '' -%}<Label Text=""{{ subtitle | Escape }}"" StyleClass=""footnote,text-interface-soft"" />{%- endif -%}
      </VerticalStackLayout>
    </VerticalStackLayout>
  </Rock:StyledBorder>

  {%- else -%}
  <!-- Row styles: Icon Row, Plain Row, Thumbnail Row, Avatar Row, Meta Row -->
  <Rock:StyledBorder StrokeThickness=""0"" CornerRadius=""12"" Padding=""18,{% if subtitle == '' %}22{% else %}18{% endif %}"" StyleClass=""bg-interface-softest,mx-16,my-8"">
    <Rock:StyledBorder.Shadow><Shadow Brush=""#000000"" Offset=""0,2"" Radius=""8"" Opacity=""0.10"" /></Rock:StyledBorder.Shadow>
    {%- if cmd != '' -%}<Rock:StyledBorder.GestureRecognizers><TapGestureRecognizer Command=""{{ cmd }}"" CommandParameter=""{{ param }}"" /></Rock:StyledBorder.GestureRecognizers>{%- endif -%}
    <Grid ColumnDefinitions=""Auto,*,Auto"" ColumnSpacing=""14"" VerticalOptions=""Center"">
      {%- if icon != '' and style != 'Thumbnail Row' and style != 'Avatar Row' -%}
      <Rock:Icon Grid.Column=""0"" IconClass=""{{ icon }}"" FontSize=""22"" TextColor=""{Rock:PaletteColor Interface-Strong}"" VerticalOptions=""Center"" />
      {%- elsif style == 'Thumbnail Row' and imgUrl != '' -%}
      <Rock:Image Grid.Column=""0"" Source=""{{ imgUrl | Escape }}"" WidthRequest=""56"" HeightRequest=""56"" Aspect=""AspectFill"" StyleClass=""rounded"" />
      {%- elsif style == 'Avatar Row' and imgUrl != '' -%}
      <Rock:Image Grid.Column=""0"" Source=""{{ imgUrl | Escape }}"" WidthRequest=""52"" HeightRequest=""52"" Aspect=""AspectFill"" StyleClass=""rounded-full"" />
      {%- endif -%}
      <VerticalStackLayout Grid.Column=""1"" Spacing=""2"" VerticalOptions=""Center"">
        <Label Text=""{{ item.Title | Escape }}"" StyleClass=""body,font-weight-semi-bold,text-interface-strongest"" />
        {%- if subtitle != '' -%}<Label Text=""{{ subtitle | Escape }}"" StyleClass=""footnote,text-interface-soft"" />{%- endif -%}
      </VerticalStackLayout>
      <Rock:Icon Grid.Column=""2"" IconClass=""chevron-right"" IconFamily=""FontAwesomeSolid"" FontSize=""15"" TextColor=""{Rock:PaletteColor Interface-Soft}"" VerticalOptions=""Center"" />
    </Grid>
  </Rock:StyledBorder>
  {%- endif -%}

  {%- endif -%}
{%- endfor -%}
{%- endcontentchannelitem -%}
</VerticalStackLayout>" );   // Content
            RockMigrationHelper.AddBlockAttributeValue( "F147C24C-E1A1-4C41-968E-0F4FABCD3DE6", "24516448-3F1F-4F27-97A1-CFB4F8B277B5", @"RockEntity" );   // EnabledLavaCommands
            RockMigrationHelper.AddBlockAttributeValue( "F147C24C-E1A1-4C41-968E-0F4FABCD3DE6", "B31D29A0-3725-4AEB-8360-7D91B9CDFE47", @"True" );   // DynamicContent
            RockMigrationHelper.AddBlockAttributeValue( "7D8C58C0-0FE1-4F17-B651-3BDC3B306423", "5682EDBF-68DA-4B43-A593-6C2B936C2839", @"<!--
  Reusable Menu / List template (Nfluence Rock Mobile)
  ONE template drives: curated recursive menus (Sundays, Explore, sub-menus)
  AND data-driven lists (Sermon Notes, Staff) by pointing at a different channel.

  Config block below — set per placement:
    menuChannelId : App Menu channel Id (for a data-list, the data channel's Id)
    menuKey       : curated menu -> which menu to show (Sundays tab='sundays', Explore='explore');
                    sub-menu pages read it from PageParameter.MenuKey
    forcedStyle   : data-list -> force one style for every row (e.g. 'Meta Row', 'Avatar Row');
                    leave BLANK for a curated menu (per-item DisplayStyle)
    sortBy        : 'Order' for menus; 'StartDateTime desc' for sermons
  Block settings: Dynamic Content = Yes, Rock Entity command enabled.
-->
<VerticalStackLayout Spacing=""0"" StyleClass=""pt-12,pb-16"">
    
{%- assign menuChannelGuid = 'DF6276E8-B00E-4E77-88D8-AB1241A4A8C7' -%}
{%- assign menuChannelId = '' -%}
{% contentchannel where:'Guid == ""{{ menuChannelGuid }}""' securityenabled:'false' %}{%- for ch in contentchannelItems -%}{%- assign menuChannelId = ch.Id -%}{%- endfor -%}{% endcontentchannel %}
{%- assign menuKey = PageParameter.MenuKey | Default:'explore' -%}
{%- assign forcedStyle = '' -%}
{%- assign sortBy = 'Order' -%}

{%- assign menuPageGuid = 'a93cfc38-98f5-41de-b68d-6e7ee97f2d46' -%}
{%- assign itemDetailGuid = 'c9d8bd2d-8f1e-42e6-a4c3-b71b0511e9c7' -%}
{%- assign webviewGuid = 'c543ef2a-df73-4c21-bb01-94f2a6cb6373' -%}
{%- assign isListMode = false -%}
{%- if forcedStyle != '' -%}{% assign isListMode = true %}{%- endif -%}

{%- contentchannelitem where:'ContentChannelId == ""{{ menuChannelId }}""' sort:'{{ sortBy }}' -%}
{%- for item in contentchannelitemItems -%}
  {%- assign itemMenu = item | Attribute:'Menu','RawValue' -%}
  {%- if isListMode or itemMenu == menuKey -%}

  {%- if isListMode -%}{% assign style = forcedStyle %}{%- else -%}{% assign style = item | Attribute:'DisplayStyle','RawValue' %}{%- endif -%}
  {%- assign icon = item | Attribute:'Icon' -%}
  {%- assign subtitle = item | Attribute:'Subtitle' -%}
  {%- assign img = item | Attribute:'Image','Url' -%}
  {%- assign imgUrl = item | Attribute:'ImageUrl' -%}
  {%- if imgUrl == '' -%}{% assign imgUrl = img %}{%- endif -%}

  {%- assign subMenu = item | Attribute:'OpensSubMenu','RawValue' -%}
  {%- assign linkType = item | Attribute:'LinktoURLType','RawValue' -%}
  {%- assign linkUrlVal = item | Attribute:'LinktoURL' -%}
  {%- assign appPage = item | Attribute:'LinktoAppPage','RawValue' -%}
  {%- assign pageParams = item | Attribute:'PageParameters' -%}
  {%- assign contentStripped = item.Content | StripHtml | Trim -%}

  {%- capture cmd -%}
  {%- if subMenu != '' -%}{Binding PushPage}
  {%- elsif linkType == 'External Browser' -%}{Binding OpenExternalBrowser}
  {%- elsif linkType == 'Internal Browser' -%}{Binding OpenBrowser}
  {%- elsif linkUrlVal != '' or appPage != '' or contentStripped != '' -%}{Binding PushPage}
  {%- endif -%}
  {%- endcapture -%}{% assign cmd = cmd | Trim %}

  {%- capture param -%}
  {%- if subMenu != '' -%}{{ menuPageGuid }}?MenuKey={{ subMenu }}&amp;Title={{ item.Title | UrlEncode }}
  {%- elsif linkUrlVal != '' and linkType == 'Webview' -%}{{ webviewGuid }}?url={{ linkUrlVal | UrlEncode }}
  {%- elsif linkUrlVal != '' -%}{{ linkUrlVal | Escape }}
  {%- elsif appPage != '' -%}{{ appPage | Split:',' | First }}{% if pageParams != '' %}{{ pageParams }}{% endif %}
  {%- elsif contentStripped != '' -%}{{ itemDetailGuid }}?ContentChannelItemId={{ item.Id }}
  {%- endif -%}
  {%- endcapture -%}{% assign param = param | Trim %}

  {%- if style == 'Section Header' -%}
  <Label Text=""{{ item.Title | Escape }}"" StyleClass=""caption1,font-weight-semi-bold,text-interface-soft,px-16,mt-16,mb-4"" />

  {%- elsif style == 'Hero Banner' -%}
  <Rock:Image Source=""{{ imgUrl | Escape }}"" Aspect=""AspectFill"" Ratio=""16:9"" StyleClass=""mb-8"">
    {%- if cmd != '' -%}<Rock:Image.GestureRecognizers><TapGestureRecognizer Command=""{{ cmd }}"" CommandParameter=""{{ param }}"" /></Rock:Image.GestureRecognizers>{%- endif -%}
  </Rock:Image>

  {%- elsif style == 'Image Card' -%}
  <Rock:StyledBorder StrokeThickness=""0"" Padding=""0"" CornerRadius=""16"" StyleClass=""bg-interface-softest,mx-16,my-8"">
    <Rock:StyledBorder.Shadow><Shadow Brush=""#000000"" Offset=""0,4"" Radius=""12"" Opacity=""0.15"" /></Rock:StyledBorder.Shadow>
    {%- if cmd != '' -%}<Rock:StyledBorder.GestureRecognizers><TapGestureRecognizer Command=""{{ cmd }}"" CommandParameter=""{{ param }}"" /></Rock:StyledBorder.GestureRecognizers>{%- endif -%}
    <VerticalStackLayout Spacing=""0"">
      <Rock:Image Source=""{{ imgUrl | Escape }}"" Aspect=""AspectFill"" Ratio=""16:9"" />
      <VerticalStackLayout StyleClass=""p-16"" Spacing=""4"">
        <Label Text=""{{ item.Title | Escape }}"" StyleClass=""title3,text-interface-strongest"" />
        {%- if subtitle != '' -%}<Label Text=""{{ subtitle | Escape }}"" StyleClass=""footnote,text-interface-soft"" />{%- endif -%}
      </VerticalStackLayout>
    </VerticalStackLayout>
  </Rock:StyledBorder>

  {%- else -%}
  <!-- Row styles: Icon Row, Plain Row, Thumbnail Row, Avatar Row, Meta Row -->
  <Rock:StyledBorder StrokeThickness=""0"" CornerRadius=""12"" Padding=""18,{% if subtitle == '' %}22{% else %}18{% endif %}"" StyleClass=""bg-interface-softest,mx-16,my-8"">
    <Rock:StyledBorder.Shadow><Shadow Brush=""#000000"" Offset=""0,2"" Radius=""8"" Opacity=""0.10"" /></Rock:StyledBorder.Shadow>
    {%- if cmd != '' -%}<Rock:StyledBorder.GestureRecognizers><TapGestureRecognizer Command=""{{ cmd }}"" CommandParameter=""{{ param }}"" /></Rock:StyledBorder.GestureRecognizers>{%- endif -%}
    <Grid ColumnDefinitions=""Auto,*,Auto"" ColumnSpacing=""14"" VerticalOptions=""Center"">
      {%- if icon != '' and style != 'Thumbnail Row' and style != 'Avatar Row' -%}
      <Rock:Icon Grid.Column=""0"" IconClass=""{{ icon }}"" FontSize=""22"" TextColor=""{Rock:PaletteColor Interface-Strong}"" VerticalOptions=""Center"" />
      {%- elsif style == 'Thumbnail Row' and imgUrl != '' -%}
      <Rock:Image Grid.Column=""0"" Source=""{{ imgUrl | Escape }}"" WidthRequest=""56"" HeightRequest=""56"" Aspect=""AspectFill"" StyleClass=""rounded"" />
      {%- elsif style == 'Avatar Row' and imgUrl != '' -%}
      <Rock:Image Grid.Column=""0"" Source=""{{ imgUrl | Escape }}"" WidthRequest=""52"" HeightRequest=""52"" Aspect=""AspectFill"" StyleClass=""rounded-full"" />
      {%- endif -%}
      <VerticalStackLayout Grid.Column=""1"" Spacing=""2"" VerticalOptions=""Center"">
        <Label Text=""{{ item.Title | Escape }}"" StyleClass=""body,font-weight-semi-bold,text-interface-strongest"" />
        {%- if subtitle != '' -%}<Label Text=""{{ subtitle | Escape }}"" StyleClass=""footnote,text-interface-soft"" />{%- endif -%}
      </VerticalStackLayout>
      <Rock:Icon Grid.Column=""2"" IconClass=""chevron-right"" IconFamily=""FontAwesomeSolid"" FontSize=""15"" TextColor=""{Rock:PaletteColor Interface-Soft}"" VerticalOptions=""Center"" />
    </Grid>
  </Rock:StyledBorder>
  {%- endif -%}

  {%- endif -%}
{%- endfor -%}
{%- endcontentchannelitem -%}
</VerticalStackLayout>" );   // Content
            RockMigrationHelper.AddBlockAttributeValue( "7D8C58C0-0FE1-4F17-B651-3BDC3B306423", "24516448-3F1F-4F27-97A1-CFB4F8B277B5", @"RockEntity" );   // EnabledLavaCommands
            RockMigrationHelper.AddBlockAttributeValue( "7D8C58C0-0FE1-4F17-B651-3BDC3B306423", "B31D29A0-3725-4AEB-8360-7D91B9CDFE47", @"True" );   // DynamicContent
            RockMigrationHelper.AddBlockAttributeValue( "83D77AF7-E32C-4994-9CEA-9698E5F7BF25", "5682EDBF-68DA-4B43-A593-6C2B936C2839", @"<!--
  Reusable Menu / List template (Nfluence Rock Mobile)
  ONE template drives: curated recursive menus (Sundays, Explore, sub-menus)
  AND data-driven lists (Sermon Notes, Staff) by pointing at a different channel.

  Config block below — set per placement:
    menuChannelId : App Menu channel Id (for a data-list, the data channel's Id)
    menuKey       : curated menu -> which menu to show (Sundays tab='sundays', Explore='explore');
                    sub-menu pages read it from PageParameter.MenuKey
    forcedStyle   : data-list -> force one style for every row (e.g. 'Meta Row', 'Avatar Row');
                    leave BLANK for a curated menu (per-item DisplayStyle)
    sortBy        : 'Order' for menus; 'StartDateTime desc' for sermons
  Block settings: Dynamic Content = Yes, Rock Entity command enabled.
-->
<VerticalStackLayout Spacing=""0"" StyleClass=""pt-12,pb-16"">

{%- assign menuChannelId = '21' -%}
{%- assign menuKey = '' -%}
{%- assign forcedStyle = 'Meta Row' -%}
{%- assign sortBy = 'StartDateTime desc' -%}
{%- assign metaMode = 'sermon' -%}
{%- comment -%} Shown when the list renders no rows. Blank it to show nothing. {%- endcomment -%}
{%- assign emptyMessage = 'No sermon notes have been posted yet.' -%}
{%- assign emptyIcon = 'file-alt' -%}

{%- assign menuPageGuid = 'a93cfc38-98f5-41de-b68d-6e7ee97f2d46' -%}
{%- assign itemDetailGuid = 'bb429c19-052f-4537-a40b-a157016b341b' -%}
{%- assign webviewGuid = 'c543ef2a-df73-4c21-bb01-94f2a6cb6373' -%}
{%- assign isListMode = false -%}
{%- if forcedStyle != '' -%}{% assign isListMode = true %}{%- endif -%}

{%- assign renderedCount = 0 -%}
{%- capture listBody -%}
{%- contentchannelitem where:'ContentChannelId == ""{{ menuChannelId }}""' sort:'{{ sortBy }}' -%}
{%- for item in contentchannelitemItems -%}
  {%- assign itemMenu = item | Attribute:'Menu','RawValue' -%}
  {%- if isListMode or itemMenu == menuKey -%}
  {%- assign renderedCount = renderedCount | Plus:1 -%}

  {%- if isListMode -%}{% assign style = forcedStyle %}{%- else -%}{% assign style = item | Attribute:'DisplayStyle','RawValue' %}{%- endif -%}
  {%- assign icon = item | Attribute:'Icon' -%}
  {%- if metaMode == 'sermon' -%}{%- assign speaker = item | Attribute:'Speaker' -%}{%- capture subtitle -%}{{ item.StartDateTime | Date:'MMMM d, yyyy' }}{% if speaker != '' %} • {{ speaker }}{% endif %}{%- endcapture -%}{%- assign subtitle = subtitle | Trim | Upcase -%}{%- else -%}{%- assign subtitle = item | Attribute:'Subtitle' -%}{%- endif -%}
  {%- assign img = item | Attribute:'Image','Url' -%}
  {%- assign imgUrl = item | Attribute:'ImageUrl' -%}
  {%- if imgUrl == '' -%}{% assign imgUrl = img %}{%- endif -%}

  {%- assign subMenu = item | Attribute:'OpensSubMenu','RawValue' -%}
  {%- assign linkType = item | Attribute:'LinktoURLType','RawValue' -%}
  {%- assign linkUrlVal = item | Attribute:'LinktoURL' -%}
  {%- assign appPage = item | Attribute:'LinktoAppPage','RawValue' -%}
  {%- assign pageParams = item | Attribute:'PageParameters' -%}
  {%- assign contentStripped = item.Content | StripHtml | Trim -%}
  {%- comment -%}
      A structured-content document holding only a Notes field is entirely
      <textarea> markup with no text between the tags, so StripHtml returns ''
      and the detail link never renders. Treat a notes field as content in its
      own right. <img> has the same blind spot if image-only items ever appear.
  {%- endcomment -%}
  {%- if contentStripped == '' and item.Content contains '<textarea' -%}
    {%- assign contentStripped = 'notes' -%}
  {%- endif -%}

  {%- capture cmd -%}
  {%- if subMenu != '' -%}{Binding PushPage}
  {%- elsif linkType == 'External Browser' -%}{Binding OpenExternalBrowser}
  {%- elsif linkType == 'Internal Browser' -%}{Binding OpenBrowser}
  {%- elsif linkUrlVal != '' or appPage != '' or contentStripped != '' -%}{Binding PushPage}
  {%- endif -%}
  {%- endcapture -%}{% assign cmd = cmd | Trim %}

  {%- capture param -%}
  {%- if subMenu != '' -%}{{ menuPageGuid }}?MenuKey={{ subMenu }}&amp;Title={{ item.Title | UrlEncode }}
  {%- elsif linkUrlVal != '' and linkType == 'Webview' -%}{{ webviewGuid }}?url={{ linkUrlVal | UrlEncode }}
  {%- elsif linkUrlVal != '' -%}{{ linkUrlVal | Escape }}
  {%- elsif appPage != '' -%}{{ appPage | Split:',' | First }}{% if pageParams != '' %}{{ pageParams }}{% endif %}
  {%- elsif contentStripped != '' -%}{{ itemDetailGuid }}?ItemGuid={{ item.Guid }}
  {%- endif -%}
  {%- endcapture -%}{% assign param = param | Trim %}

  {%- if style == 'Section Header' -%}
  <Label Text=""{{ item.Title | Escape }}"" StyleClass=""caption1,font-weight-semi-bold,text-interface-soft,px-16,mt-16,mb-4"" />

  {%- elsif style == 'Hero Banner' -%}
  <Rock:Image Source=""{{ imgUrl | Escape }}"" Aspect=""AspectFill"" Ratio=""16:9"" StyleClass=""mb-8"">
    {%- if cmd != '' -%}<Rock:Image.GestureRecognizers><TapGestureRecognizer Command=""{{ cmd }}"" CommandParameter=""{{ param }}"" /></Rock:Image.GestureRecognizers>{%- endif -%}
  </Rock:Image>

  {%- elsif style == 'Image Card' -%}
  <Rock:StyledBorder StrokeThickness=""0"" Padding=""0"" CornerRadius=""16"" StyleClass=""bg-interface-softest,mx-16,my-8"">
    <Rock:StyledBorder.Shadow><Shadow Brush=""#000000"" Offset=""0,4"" Radius=""12"" Opacity=""0.15"" /></Rock:StyledBorder.Shadow>
    {%- if cmd != '' -%}<Rock:StyledBorder.GestureRecognizers><TapGestureRecognizer Command=""{{ cmd }}"" CommandParameter=""{{ param }}"" /></Rock:StyledBorder.GestureRecognizers>{%- endif -%}
    <VerticalStackLayout Spacing=""0"">
      <Rock:Image Source=""{{ imgUrl | Escape }}"" Aspect=""AspectFill"" Ratio=""16:9"" />
      <VerticalStackLayout StyleClass=""p-16"" Spacing=""4"">
        <Label Text=""{{ item.Title | Escape }}"" StyleClass=""title3,text-interface-strongest"" />
        {%- if subtitle != '' -%}<Label Text=""{{ subtitle | Escape }}"" StyleClass=""footnote,text-interface-soft"" />{%- endif -%}
      </VerticalStackLayout>
    </VerticalStackLayout>
  </Rock:StyledBorder>

  {%- else -%}
  <!-- Row styles: Icon Row, Plain Row, Thumbnail Row, Avatar Row, Meta Row -->
  <Rock:StyledBorder StrokeThickness=""0"" CornerRadius=""12"" Padding=""18,{% if subtitle == '' %}22{% else %}16{% endif %}"" StyleClass=""bg-interface-softest,mx-16,my-8"">
    <Rock:StyledBorder.Shadow><Shadow Brush=""#000000"" Offset=""0,2"" Radius=""8"" Opacity=""0.10"" /></Rock:StyledBorder.Shadow>
    {%- if cmd != '' -%}<Rock:StyledBorder.GestureRecognizers><TapGestureRecognizer Command=""{{ cmd }}"" CommandParameter=""{{ param }}"" /></Rock:StyledBorder.GestureRecognizers>{%- endif -%}
    <Grid ColumnDefinitions=""Auto,*,Auto"" ColumnSpacing=""14"" VerticalOptions=""Center"">
      {%- if icon != '' and style != 'Thumbnail Row' and style != 'Avatar Row' -%}
      <Rock:Icon Grid.Column=""0"" IconClass=""{{ icon }}"" FontSize=""22"" TextColor=""{Rock:PaletteColor Interface-Strong}"" VerticalOptions=""Center"" />
      {%- elsif style == 'Thumbnail Row' and imgUrl != '' -%}
      <Rock:Image Grid.Column=""0"" Source=""{{ imgUrl | Escape }}"" WidthRequest=""56"" HeightRequest=""56"" Aspect=""AspectFill"" StyleClass=""rounded"" />
      {%- elsif style == 'Avatar Row' and imgUrl != '' -%}
      <Rock:Image Grid.Column=""0"" Source=""{{ imgUrl | Escape }}"" WidthRequest=""52"" HeightRequest=""52"" Aspect=""AspectFill"" StyleClass=""rounded-full"" />
      {%- endif -%}
      <VerticalStackLayout Grid.Column=""1"" Spacing=""2"" VerticalOptions=""Center"">
        <Label Text=""{{ item.Title | Escape }}"" StyleClass=""body,font-weight-semi-bold,text-interface-strongest"" />
        {%- if subtitle != '' -%}<Label Text=""{{ subtitle | Escape }}"" StyleClass=""footnote,text-interface-soft"" />{%- endif -%}
      </VerticalStackLayout>
      {%- if cmd != '' -%}<Rock:Icon Grid.Column=""2"" IconClass=""chevron-right"" IconFamily=""FontAwesomeSolid"" FontSize=""15"" TextColor=""{Rock:PaletteColor Interface-Soft}"" VerticalOptions=""Center"" />{%- endif -%}
    </Grid>
  </Rock:StyledBorder>
  {%- endif -%}

  {%- endif -%}
{%- endfor -%}
{%- endcontentchannelitem -%}
{%- endcapture -%}

{%- if renderedCount > 0 -%}
{{ listBody }}
{%- elsif emptyMessage != '' -%}
    <VerticalStackLayout Spacing=""12"" StyleClass=""p-32"" HorizontalOptions=""Fill"">
        {%- if emptyIcon != '' -%}
        <Rock:Icon IconClass=""{{ emptyIcon }}"" IconFamily=""FontAwesomeSolid"" FontSize=""36""
            StyleClass=""text-interface-soft"" HorizontalOptions=""Center"" />
        {%- endif -%}
        <Label Text=""{{ emptyMessage | Escape }}"" StyleClass=""body, text-interface-medium""
            HorizontalTextAlignment=""Center"" HorizontalOptions=""Fill"" />
    </VerticalStackLayout>
{%- endif -%}
</VerticalStackLayout>" );   // Content
            RockMigrationHelper.AddBlockAttributeValue( "83D77AF7-E32C-4994-9CEA-9698E5F7BF25", "24516448-3F1F-4F27-97A1-CFB4F8B277B5", @"RockEntity" );   // EnabledLavaCommands
            RockMigrationHelper.AddBlockAttributeValue( "83D77AF7-E32C-4994-9CEA-9698E5F7BF25", "B31D29A0-3725-4AEB-8360-7D91B9CDFE47", @"True" );   // DynamicContent
            RockMigrationHelper.AddBlockAttributeValue( "9C074994-EE12-41C8-8072-49A3012A72E8", "E4709745-E420-425D-82FB-E7EA9B8C89E2", @"368dd475-242c-49c4-a42c-7278be690cc2" );   // ConnectionStatus
            RockMigrationHelper.AddBlockAttributeValue( "9C074994-EE12-41C8-8072-49A3012A72E8", "13AED2C1-BC58-4B5C-B711-CEA71A52ECC4", @"283999ec-7346-42e3-b807-bce9b2babb49" );   // RecordStatus
            RockMigrationHelper.AddBlockAttributeValue( "9C074994-EE12-41C8-8072-49A3012A72E8", "F8A627F5-D23B-4F09-BF68-C2E7D5279C4D", @"True" );   // BirthDateShow
            RockMigrationHelper.AddBlockAttributeValue( "9C074994-EE12-41C8-8072-49A3012A72E8", "98A9C8D3-777D-4744-9346-811A9829CB47", @"True" );   // BirthDateRequired
            RockMigrationHelper.AddBlockAttributeValue( "9C074994-EE12-41C8-8072-49A3012A72E8", "E35DCBCE-A2F6-46BD-87D5-04FF6A59BAB8", @"True" );   // CampusShow
            RockMigrationHelper.AddBlockAttributeValue( "9C074994-EE12-41C8-8072-49A3012A72E8", "22289290-D87F-418F-997C-5FDF986379A1", @"True" );   // CampusRequired
            RockMigrationHelper.AddBlockAttributeValue( "9C074994-EE12-41C8-8072-49A3012A72E8", "FE8F714E-3077-4EB6-87A8-001AF221DA1E", @"True" );   // EmailShow
            RockMigrationHelper.AddBlockAttributeValue( "9C074994-EE12-41C8-8072-49A3012A72E8", "BE3AAA2A-5D08-46CF-B7DE-DB8F19502463", @"True" );   // EmailRequired
            RockMigrationHelper.AddBlockAttributeValue( "9C074994-EE12-41C8-8072-49A3012A72E8", "1FAA53DE-7F6E-481A-A8C0-D977271F0B6E", @"True" );   // MobilePhoneShow
            RockMigrationHelper.AddBlockAttributeValue( "9C074994-EE12-41C8-8072-49A3012A72E8", "7DAC89DE-BF2E-47E7-9D83-A4056A681D9B", @"True" );   // MobilePhoneRequired
            RockMigrationHelper.AddBlockAttributeValue( "9C074994-EE12-41C8-8072-49A3012A72E8", "CC6CC403-423B-48CA-9761-6114C34C7FDE", @"True" );   // AddressShow
            RockMigrationHelper.AddBlockAttributeValue( "9C074994-EE12-41C8-8072-49A3012A72E8", "1759420A-18DA-44B0-98B9-54F55CD644B6", @"True" );   // AddressRequired
            RockMigrationHelper.AddBlockAttributeValue( "783BB975-D313-4722-A444-D3FF6EE06B3B", "A425BCFB-F882-4094-B41B-66A79FA4C902", @"17aaceef-15ca-4c30-9a3a-11e6cf7e6411" );   // ConfirmAccountTemplate
            RockMigrationHelper.AddBlockAttributeValue( "5081D904-65B8-46D7-9BF4-602661982712", "5682EDBF-68DA-4B43-A593-6C2B936C2839", @"{%- assign activeTab = 'notifications' -%}
{% comment %} ^ set to 'notifications' | 'mylist' | 'mygiving' on each page {% endcomment %}

{%- assign notificationsPageGuid = '9d8435bd-8583-4325-aefc-af073d0e9020' -%}
{%- assign myListPageGuid        = 'a4ffb56b-938e-44b9-adb2-a2529b0d8af2' -%}
{%- assign myGivingPageGuid      = 'df9a3772-cc1c-4ce3-885b-abf828ef6065' -%}
{%- assign editProfilePageGuid   = '941096ae-fb51-4450-9db9-6248b584d917' -%}
{%- assign loginPageGuid = '9bb25932-4d56-417c-911b-dc915167e7bc' -%}

{%- assign coverImageUrl = '' -%}
{% comment %} coverImageUrl: static brand banner or a person cover attribute; empty = solid band {% endcomment %}

<VerticalStackLayout Spacing=""0"" StyleClass=""bg-interface-softest"">
    
    <!-- ===== Banner + overlapping avatar ===== -->
    <Grid HorizontalOptions=""Fill"">
        {% if coverImageUrl != '' %}
            <Rock:Image Source=""{{ coverImageUrl | Escape }}"" Aspect=""AspectFill"" HeightRequest=""150"" VerticalOptions=""Start"" />
        {% else %}
            <Rock:StyledBorder HeightRequest=""150"" VerticalOptions=""Start"" StyleClass=""bg-interface-soft"" />
        {% endif %}

        <Grid WidthRequest=""92"" HeightRequest=""92"" HorizontalOptions=""Center"" VerticalOptions=""Start""
              Margin=""0,104,0,0"" BackgroundColor=""Transparent"">
        {% if CurrentPerson != null %}
            <Rock:StyledBorder WidthRequest=""92"" HeightRequest=""92"" CornerRadius=""46"" Padding=""0""
                StrokeThickness=""3"" Stroke=""{AppThemeBinding Light=#FFFFFF, Dark=#18181B}""
                StyleClass=""bg-interface-softer"">
                <Rock:Image x:Name=""PersonImage""
                    Source=""{{ 'Global' | Attribute:'PublicApplicationRoot' }}GetAvatar.ashx?PersonGuid={{ CurrentPerson.Guid }}&amp;w=184""
                    Aspect=""AspectFill"" WidthRequest=""92"" HeightRequest=""92"" />
            </Rock:StyledBorder>
            <Rock:StyledBorder WidthRequest=""30"" HeightRequest=""30"" CornerRadius=""15"" Padding=""0""
                HorizontalOptions=""End"" VerticalOptions=""End"" StrokeThickness=""2""
                BackgroundColor=""{Rock:PaletteColor App-Primary-Strong}"" Stroke=""{AppThemeBinding Light=#FFFFFF, Dark=#18181B}"">
                <Rock:Icon IconClass=""pen"" IconFamily=""FontAwesomeSolid"" FontSize=""13""
                    TextColor=""#FFFFFF"" HorizontalOptions=""Center"" VerticalOptions=""Center"" />
            </Rock:StyledBorder>
            <Grid.GestureRecognizers>
                <TapGestureRecognizer Command=""{Binding UpdatePersonProfilePhoto}"">
                    <TapGestureRecognizer.CommandParameter>
                        <Rock:UpdatePersonProfilePhotoCommandParameters PersonGuid=""{{ CurrentPerson.Guid }}"" Image=""{x:Reference PersonImage}"" />
                    </TapGestureRecognizer.CommandParameter>
                </TapGestureRecognizer>
            </Grid.GestureRecognizers>
        {% else %}
            <Rock:StyledBorder WidthRequest=""92"" HeightRequest=""92"" CornerRadius=""46"" Padding=""0""
                StrokeThickness=""3"" Stroke=""{AppThemeBinding Light=#FFFFFF, Dark=#18181B}""
                BackgroundColor=""{Rock:PaletteColor App-Primary-Strong}"">
                <Rock:Icon IconClass=""user"" IconFamily=""FontAwesomeSolid"" FontSize=""44""
                    TextColor=""#FFFFFF"" HorizontalOptions=""Center"" VerticalOptions=""Center"" />
            </Rock:StyledBorder>
            <Grid.GestureRecognizers>
                <TapGestureRecognizer Command=""{Binding PushPage}"" CommandParameter=""{{ loginPageGuid }}"" />
            </Grid.GestureRecognizers>
        {% endif %}
        </Grid>
    </Grid>

    <!-- ===== Name + action link ===== -->
    <VerticalStackLayout Spacing=""2"" StyleClass=""p-8"" HorizontalOptions=""Center"">
        {% if CurrentPerson != null %}
            <Label Text=""{{ CurrentPerson.FullName | Escape }}""
                StyleClass=""title2, bold, text-interface-strongest"" HorizontalOptions=""Center"" />
            <Label Text=""Edit Profile"" TextColor=""{Rock:PaletteColor App-Primary-Strong}""
                StyleClass=""body, font-weight-semi-bold"" HorizontalOptions=""Center"">
                <Label.GestureRecognizers>
                    <TapGestureRecognizer Command=""{Binding PushPage}"" CommandParameter=""{{ editProfilePageGuid }}"" />
                </Label.GestureRecognizers>
            </Label>
        {% else %}
            <Label Text=""Welcome""
                StyleClass=""title2, bold, text-interface-strongest"" HorizontalOptions=""Center"" />
            <Label Text=""Sign in or Register"" TextColor=""{Rock:PaletteColor App-Primary-Strong}""
                StyleClass=""body, font-weight-semi-bold"" HorizontalOptions=""Center"">
                <Label.GestureRecognizers>
                    <TapGestureRecognizer Command=""{Binding PushPage}"" CommandParameter=""{{ loginPageGuid }}"" />
                </Label.GestureRecognizers>
            </Label>
        {% endif %}
    </VerticalStackLayout>

    <!-- ===== Faux tab bar ===== -->
    <Grid ColumnDefinitions=""*, *, *"" StyleClass=""bg-interface-softest"">

        <!-- Notifications -->
        <VerticalStackLayout Grid.Column=""0"" Spacing=""6"" BackgroundColor=""Transparent"" StyleClass=""pt-8"">
            <Rock:Icon IconClass=""bell"" IconFamily=""FontAwesomeSolid"" FontSize=""22"" HorizontalOptions=""Center""
                {% if activeTab == 'notifications' %}TextColor=""{Rock:PaletteColor App-Primary-Strong}""{% else %}StyleClass=""text-interface-medium""{% endif %} />
            <Label Text=""Notifications"" HorizontalOptions=""Center""
                StyleClass=""caption1, font-weight-semi-bold{% unless activeTab == 'notifications' %}, text-interface-medium{% endunless %}""
                {% if activeTab == 'notifications' %}TextColor=""{Rock:PaletteColor App-Primary-Strong}""{% endif %} />
            <BoxView HeightRequest=""3"" Color=""{% if activeTab == 'notifications' %}{Rock:PaletteColor App-Primary-Strong}{% else %}Transparent{% endif %}"" />
            {% unless activeTab == 'notifications' %}
                <VerticalStackLayout.GestureRecognizers>
                    <TapGestureRecognizer Command=""{Binding ReplacePage}"">
                        <TapGestureRecognizer.CommandParameter>
                            <Rock:ReplacePageParameters PageGuid=""{{ notificationsPageGuid }}"" WaitForReady=""true"" />
                        </TapGestureRecognizer.CommandParameter>
                    </TapGestureRecognizer>
                </VerticalStackLayout.GestureRecognizers>
            {% endunless %}
        </VerticalStackLayout>

        <!-- My List -->
        <VerticalStackLayout Grid.Column=""1"" Spacing=""6"" BackgroundColor=""Transparent"" StyleClass=""pt-8"">
            <Rock:Icon IconClass=""bookmark"" IconFamily=""FontAwesomeSolid"" FontSize=""22"" HorizontalOptions=""Center""
                {% if activeTab == 'mylist' %}TextColor=""{Rock:PaletteColor App-Primary-Strong}""{% else %}StyleClass=""text-interface-medium""{% endif %} />
            <Label Text=""My List"" HorizontalOptions=""Center""
                StyleClass=""caption1, font-weight-semi-bold{% unless activeTab == 'mylist' %}, text-interface-medium{% endunless %}""
                {% if activeTab == 'mylist' %}TextColor=""{Rock:PaletteColor App-Primary-Strong}""{% endif %} />
            <BoxView HeightRequest=""3"" Color=""{% if activeTab == 'mylist' %}{Rock:PaletteColor App-Primary-Strong}{% else %}Transparent{% endif %}"" />
            {% unless activeTab == 'mylist' %}
                <VerticalStackLayout.GestureRecognizers>
                    <TapGestureRecognizer Command=""{Binding ReplacePage}"">
                        <TapGestureRecognizer.CommandParameter>
                            <Rock:ReplacePageParameters PageGuid=""{{ myListPageGuid }}"" WaitForReady=""true"" />
                        </TapGestureRecognizer.CommandParameter>
                    </TapGestureRecognizer>
                </VerticalStackLayout.GestureRecognizers>
            {% endunless %}
        </VerticalStackLayout>

        <!-- My Giving -->
        <VerticalStackLayout Grid.Column=""2"" Spacing=""6"" BackgroundColor=""Transparent"" StyleClass=""pt-8"">
            <Rock:Icon IconClass=""hand-holding-heart"" IconFamily=""FontAwesomeSolid"" FontSize=""22"" HorizontalOptions=""Center""
                {% if activeTab == 'mygiving' %}TextColor=""{Rock:PaletteColor App-Primary-Strong}""{% else %}StyleClass=""text-interface-medium""{% endif %} />
            <Label Text=""My Giving"" HorizontalOptions=""Center""
                StyleClass=""caption1, font-weight-semi-bold{% unless activeTab == 'mygiving' %}, text-interface-medium{% endunless %}""
                {% if activeTab == 'mygiving' %}TextColor=""{Rock:PaletteColor App-Primary-Strong}""{% endif %} />
            <BoxView HeightRequest=""3"" Color=""{% if activeTab == 'mygiving' %}{Rock:PaletteColor App-Primary-Strong}{% else %}Transparent{% endif %}"" />
            {% unless activeTab == 'mygiving' %}
                <VerticalStackLayout.GestureRecognizers>
                    <TapGestureRecognizer Command=""{Binding ReplacePage}"">
                        <TapGestureRecognizer.CommandParameter>
                            <Rock:ReplacePageParameters PageGuid=""{{ myGivingPageGuid }}"" WaitForReady=""true"" />
                        </TapGestureRecognizer.CommandParameter>
                    </TapGestureRecognizer>
                </VerticalStackLayout.GestureRecognizers>
            {% endunless %}
        </VerticalStackLayout>

    </Grid>

    <BoxView HeightRequest=""1"" StyleClass=""bg-interface-softer"" />

</VerticalStackLayout>" );   // Content
            RockMigrationHelper.AddBlockAttributeValue( "5081D904-65B8-46D7-9BF4-602661982712", "B31D29A0-3725-4AEB-8360-7D91B9CDFE47", @"True" );   // DynamicContent
            RockMigrationHelper.AddBlockAttributeValue( "86FA86D1-936D-4AD3-908B-D07D9A874F1F", "D09D1B5F-A208-48AB-9E80-64E032C071B4", @"ef0257ad-b5e4-4d53-b7d0-17561941ee1e" );   // CompletedPage
            RockMigrationHelper.AddBlockAttributeValue( "86FA86D1-936D-4AD3-908B-D07D9A874F1F", "41B67475-F8D4-42EB-991D-71A327A08077", @"9bb25932-4d56-417c-911b-dc915167e7bc" );   // LoginPage
            RockMigrationHelper.AddBlockAttributeValue( "86FA86D1-936D-4AD3-908B-D07D9A874F1F", "487C6E9E-BF4F-4111-9C17-DEF87EADB213", @"368dd475-242c-49c4-a42c-7278be690cc2" );   // DefaultConnectionStatus
            RockMigrationHelper.AddBlockAttributeValue( "86FA86D1-936D-4AD3-908B-D07D9A874F1F", "EFF020AA-E9FD-4FB0-9B1E-0446E751844F", @"283999ec-7346-42e3-b807-bce9b2babb49" );   // DefaultRecordStatus
            RockMigrationHelper.AddBlockAttributeValue( "86FA86D1-936D-4AD3-908B-D07D9A874F1F", "5F1F626E-65F1-44AC-BF49-40FD21E3EF64", @"10101010-2db4-4c95-b07d-c400e412289b,5a61507b-79cb-4da2-af43-6f82260203b3" );   // DisplayCampusTypes
            RockMigrationHelper.AddBlockAttributeValue( "86FA86D1-936D-4AD3-908B-D07D9A874F1F", "42B6A2C0-37FF-4BA8-8378-791937AECA86", @"10696fd8-d0c7-486f-b736-5fb3f5d69f1a" );   // DisplayCampusStatuses
            RockMigrationHelper.AddBlockAttributeValue( "86FA86D1-936D-4AD3-908B-D07D9A874F1F", "CA1F541C-3C37-438B-8D7A-3841E31E05F8", @"543b7c09-80c0-4dab-8487-10569474d9c7" );   // SystemCommunication
            RockMigrationHelper.AddBlockAttributeValue( "86FA86D1-936D-4AD3-908B-D07D9A874F1F", "4FE016DB-C77C-4A09-933B-1AAC649CAC95", @"5" );   // VerificationTimeLimit
            RockMigrationHelper.AddBlockAttributeValue( "86FA86D1-936D-4AD3-908B-D07D9A874F1F", "293A5476-C910-407A-8EAA-CA7AB12A1F55", @"5000" );   // IpThrottleLimit
            RockMigrationHelper.AddBlockAttributeValue( "86FA86D1-936D-4AD3-908B-D07D9A874F1F", "9E421C90-F505-47DC-BC93-CEE75076D653", @"10" );   // ValidationCodeAttempts
            RockMigrationHelper.AddBlockAttributeValue( "86FA86D1-936D-4AD3-908B-D07D9A874F1F", "ED3671C9-D962-4584-8FD9-D7C2FEBFACE0", @"False" );   // AllowSkipOfOnboarding
            RockMigrationHelper.AddBlockAttributeValue( "86FA86D1-936D-4AD3-908B-D07D9A874F1F", "D55F9A7C-CEB0-468A-86B2-A1B34B09B302", @"False" );   // HideGenderIfKnown
            RockMigrationHelper.AddBlockAttributeValue( "86FA86D1-936D-4AD3-908B-D07D9A874F1F", "FD94209A-76D6-4D41-B398-96745F546D5B", @"False" );   // HideBirthDateIfKnown
            RockMigrationHelper.AddBlockAttributeValue( "86FA86D1-936D-4AD3-908B-D07D9A874F1F", "4AA3A746-C705-418A-8FB5-FB5690D1429D", @"False" );   // HideMobilePhoneIfKnown
            RockMigrationHelper.AddBlockAttributeValue( "86FA86D1-936D-4AD3-908B-D07D9A874F1F", "1A4321DF-3AAD-4A2C-8070-A068820BE827", @"False" );   // HideEmailIfKnown
            RockMigrationHelper.AddBlockAttributeValue( "86FA86D1-936D-4AD3-908B-D07D9A874F1F", "8458FD9D-C6C1-405B-9A93-CB713CD9A953", @"True" );   // ShowNotificationsRequest
            RockMigrationHelper.AddBlockAttributeValue( "86FA86D1-936D-4AD3-908B-D07D9A874F1F", "D00B6A18-D302-4A8C-ADF7-CD46ABF884C7", @"False" );   // HideCampusIfKnown
            RockMigrationHelper.AddBlockAttributeValue( "86FA86D1-936D-4AD3-908B-D07D9A874F1F", "2D218849-F96F-4B0B-9B27-5236E8A6F8CB", @"1" );   // GenderVisibility
            RockMigrationHelper.AddBlockAttributeValue( "86FA86D1-936D-4AD3-908B-D07D9A874F1F", "3E362C81-EB06-444A-8E8D-92A763177B8E", @"1" );   // BirthDateVisibility
            RockMigrationHelper.AddBlockAttributeValue( "86FA86D1-936D-4AD3-908B-D07D9A874F1F", "E88A11E9-2118-4D70-B8CD-9E7B58AD07D3", @"1" );   // MobilePhoneVisibility
            RockMigrationHelper.AddBlockAttributeValue( "86FA86D1-936D-4AD3-908B-D07D9A874F1F", "386C39D8-1D77-47F4-94F1-129F03AD7B88", @"1" );   // EmailVisibility
            RockMigrationHelper.AddBlockAttributeValue( "86FA86D1-936D-4AD3-908B-D07D9A874F1F", "9106D3D4-20B0-47D4-81F3-A4FFF4D01D17", @"1" );   // CreateLoginVisibility
            RockMigrationHelper.AddBlockAttributeValue( "86FA86D1-936D-4AD3-908B-D07D9A874F1F", "03152B49-B22B-478E-8EB2-BAD959A6E39A", @"Hello!" );   // HelloScreenTitle
            RockMigrationHelper.AddBlockAttributeValue( "86FA86D1-936D-4AD3-908B-D07D9A874F1F", "C93E9926-0010-4B0E-A4D7-AA30B41F0BBF", @"Welcome to the {{ 'Global' | Attribute:'OrganizationName' }} mobile app. Please sign-in so we can personalize your experience." );   // HelloScreenSubtitle
            RockMigrationHelper.AddBlockAttributeValue( "86FA86D1-936D-4AD3-908B-D07D9A874F1F", "18D9FB2D-1898-4957-95F0-2A5D2A55526A", @"Code Sent..." );   // CodeSentScreenTitle
            RockMigrationHelper.AddBlockAttributeValue( "86FA86D1-936D-4AD3-908B-D07D9A874F1F", "A89FC144-DB98-4129-BAB7-EADA64F8FFCA", @"You should be receiving a verification code from us shortly. When it arrives type or paste it below." );   // CodeSentScreenSubtitle
            RockMigrationHelper.AddBlockAttributeValue( "86FA86D1-936D-4AD3-908B-D07D9A874F1F", "FEC42D9A-A95C-4FEC-95DB-400B738BB250", @"Let’s Get to Know You" );   // NameScreenTitle
            RockMigrationHelper.AddBlockAttributeValue( "86FA86D1-936D-4AD3-908B-D07D9A874F1F", "6B463687-EDCF-4C44-9BE6-96B00B6C23B0", @"To maximize your experience we’d like to know a little about you." );   // NameScreenSubtitle
            RockMigrationHelper.AddBlockAttributeValue( "86FA86D1-936D-4AD3-908B-D07D9A874F1F", "BBF73E74-45DF-4E41-812D-A704E6409B14", @"Tell Us More" );   // PersonalInformationScreenTitle
            RockMigrationHelper.AddBlockAttributeValue( "86FA86D1-936D-4AD3-908B-D07D9A874F1F", "29F7B8FD-1DAF-4062-9720-A735F60843AB", @"The more we know the more we can tailor our ministry to you." );   // PersonalInformationScreenSubtitle
            RockMigrationHelper.AddBlockAttributeValue( "86FA86D1-936D-4AD3-908B-D07D9A874F1F", "77D080E2-1CCD-4A1D-A641-20D9F0734ABF", @"Stay Connected" );   // ContactInformationScreenTitle
            RockMigrationHelper.AddBlockAttributeValue( "86FA86D1-936D-4AD3-908B-D07D9A874F1F", "2DF996A3-002D-4C89-8AE4-9FC49F846107", @"Help us keep you in the loop by providing your contact information." );   // ContactInformationScreenSubtitle
            RockMigrationHelper.AddBlockAttributeValue( "86FA86D1-936D-4AD3-908B-D07D9A874F1F", "7C3F512A-7092-41C8-A6AA-F9427EFB313B", @"Topics Of Interest" );   // InterestsScreenTitle
            RockMigrationHelper.AddBlockAttributeValue( "86FA86D1-936D-4AD3-908B-D07D9A874F1F", "7F13BCFE-A16A-4688-AC74-30848B3DD1B2", @"What topics are you most interested in." );   // InterestsScreenSubtitle
            RockMigrationHelper.AddBlockAttributeValue( "86FA86D1-936D-4AD3-908B-D07D9A874F1F", "3C3F5F53-D303-4465-A6BF-58C5F1365134", @"Enable Notifications" );   // NotificationsScreenTitle
            RockMigrationHelper.AddBlockAttributeValue( "86FA86D1-936D-4AD3-908B-D07D9A874F1F", "98D17BDF-C2F9-4F78-BFD8-7BE160787A24", @"We’d like to keep you in the loop with important alerts and notifications." );   // NotificationsScreenSubtitle
            RockMigrationHelper.AddBlockAttributeValue( "86FA86D1-936D-4AD3-908B-D07D9A874F1F", "6E725307-72A3-4D2E-AA7E-B9DDE5187A67", @"Find Your Campus" );   // CampusScreenTitle
            RockMigrationHelper.AddBlockAttributeValue( "86FA86D1-936D-4AD3-908B-D07D9A874F1F", "025306AB-40BA-490F-A9C5-BD1FC21E801D", @"Select the campus you attend to get targets news and information about events." );   // CampusScreenSubtitle
            RockMigrationHelper.AddBlockAttributeValue( "86FA86D1-936D-4AD3-908B-D07D9A874F1F", "DF658448-7F79-473B-AE1E-F3B2DA7EBBF8", @"Create Login" );   // CreateLoginScreenTitle
            RockMigrationHelper.AddBlockAttributeValue( "86FA86D1-936D-4AD3-908B-D07D9A874F1F", "EAED4835-A87E-4EEE-8345-D01DF6FF1DD7", @"Create a login to help signing in quicker in the future." );   // CreateLoginScreenSubtitle
            RockMigrationHelper.AddBlockAttributeValue( "4CAF5978-5265-4633-8021-D53E30C318EE", "5682EDBF-68DA-4B43-A593-6C2B936C2839", @"<!--
  Settings page (Nfluence Rock Mobile) - the store-compliance hub.
  Block: Content, on the Settings page.  Dynamic Content = Yes (needs CurrentPerson).
  FILL IN before deploy:
    DELETE_ACCOUNT_PAGE_GUID  -> the Delete Account page's GUID
    PRIVACY_POLICY_URL / TERMS_URL / ABOUT_URL -> your web URLs
  Fixed GUIDs: Edit Profile = 941096ae-fb51-4450-9db9-6248b584d917 ; Login = 9bb25932-4d56-417c-911b-dc915167e7bc
-->
<VerticalStackLayout Spacing=""0"">

  <!-- Person header (orange banner) -->
  <Rock:StyledBorder BackgroundColor=""{Rock:PaletteColor App-Primary-Strong}"" CornerRadius=""0"" StrokeThickness=""0"" Padding=""16,20"">
    <Grid ColumnDefinitions=""Auto,*,Auto"" ColumnSpacing=""14"" VerticalOptions=""Center"">
      <Rock:StyledBorder Grid.Column=""0"" WidthRequest=""60"" HeightRequest=""60"" CornerRadius=""30"" Padding=""0""
          BackgroundColor=""#FFFFFF"" StrokeThickness=""0"" VerticalOptions=""Center"">
        {%- if CurrentPerson -%}
        <Rock:Avatar PersonGuid=""{{ CurrentPerson.Guid }}"" />
        {%- else -%}
        <Rock:Icon IconClass=""user"" IconFamily=""FontAwesomeSolid"" FontSize=""30"" TextColor=""{Rock:PaletteColor App-Primary-Strong}"" HorizontalOptions=""Center"" VerticalOptions=""Center"" />
        {%- endif -%}
      </Rock:StyledBorder>
      <VerticalStackLayout Grid.Column=""1"" Spacing=""2"" VerticalOptions=""Center"">
        <Label Text=""{% if CurrentPerson %}{{ CurrentPerson.FullName | Escape }}{% else %}Guest{% endif %}"" StyleClass=""title3"" TextColor=""#FFFFFF"" />
        <Label Text=""{% if CurrentPerson %}Edit Profile{% else %}Sign in or Register{% endif %}"" StyleClass=""callout"" TextColor=""#FFFFFF"">
          <Label.GestureRecognizers>
            <TapGestureRecognizer Command=""{Binding PushPage}"" CommandParameter=""{% if CurrentPerson %}941096ae-fb51-4450-9db9-6248b584d917{% else %}9bb25932-4d56-417c-911b-dc915167e7bc{% endif %}"" />
          </Label.GestureRecognizers>
        </Label>
      </VerticalStackLayout>
      {%- if CurrentPerson -%}
      <Rock:Icon Grid.Column=""2"" IconClass=""pencil-alt"" IconFamily=""FontAwesomeSolid"" FontSize=""18"" TextColor=""#FFFFFF"" VerticalOptions=""Center"">
        <Rock:Icon.GestureRecognizers><TapGestureRecognizer Command=""{Binding PushPage}"" CommandParameter=""941096ae-fb51-4450-9db9-6248b584d917"" /></Rock:Icon.GestureRecognizers>
      </Rock:Icon>
      {%- endif -%}
    </Grid>
  </Rock:StyledBorder>

  <!-- APP SETTINGS -->
  <Label Text=""APP SETTINGS"" StyleClass=""caption1,font-weight-semi-bold,text-interface-soft"" Margin=""16,24,16,8"" />

  <Grid ColumnDefinitions=""*,Auto"" Padding=""16,16"">
    <Grid.GestureRecognizers><TapGestureRecognizer Command=""{Binding OpenBrowser}"" CommandParameter=""https://nfluencechurch.org/about/"" /></Grid.GestureRecognizers>
    <Label Grid.Column=""0"" Text=""About This App"" StyleClass=""body,text-interface-strongest"" VerticalOptions=""Center"" />
    <Rock:Icon Grid.Column=""1"" IconClass=""chevron-right"" IconFamily=""FontAwesomeSolid"" FontSize=""15"" TextColor=""{Rock:PaletteColor Interface-Soft}"" VerticalOptions=""Center"" />
  </Grid>
  <BoxView HeightRequest=""1"" Color=""{Rock:PaletteColor Interface-Softer}"" Margin=""16,0"" />

  <Grid ColumnDefinitions=""*,Auto"" Padding=""16,16"">
    <Grid.GestureRecognizers><TapGestureRecognizer Command=""{Binding OpenBrowser}"" CommandParameter=""https://nfluencechurch.org/privacy-policy/"" /></Grid.GestureRecognizers>
    <Label Grid.Column=""0"" Text=""Privacy Policy"" StyleClass=""body,text-interface-strongest"" VerticalOptions=""Center"" />
    <Rock:Icon Grid.Column=""1"" IconClass=""chevron-right"" IconFamily=""FontAwesomeSolid"" FontSize=""15"" TextColor=""{Rock:PaletteColor Interface-Soft}"" VerticalOptions=""Center"" />
  </Grid>

  {%- if CurrentPerson -%}
  <!-- PRIVACY SETTINGS (compliance) -->
  <Label Text=""PRIVACY SETTINGS"" StyleClass=""caption1,font-weight-semi-bold,text-interface-soft"" Margin=""16,28,16,8"" />

  <Grid ColumnDefinitions=""*,Auto"" Padding=""16,16"">
    <Grid.GestureRecognizers><TapGestureRecognizer Command=""{Binding Logout}"" /></Grid.GestureRecognizers>
    <Label Grid.Column=""0"" Text=""Sign Out"" StyleClass=""body,text-interface-strongest"" VerticalOptions=""Center"" />
  </Grid>
  <BoxView HeightRequest=""1"" Color=""{Rock:PaletteColor Interface-Softer}"" Margin=""16,0"" />

  <Grid ColumnDefinitions=""*,Auto"" Padding=""16,16"">
    <Grid.GestureRecognizers><TapGestureRecognizer Command=""{Binding PushPage}"" CommandParameter=""5ac83965-d553-4255-b1ed-85f0d8742b6a"" /></Grid.GestureRecognizers>
    <Label Grid.Column=""0"" Text=""Delete Profile"" StyleClass=""body,font-weight-semi-bold"" TextColor=""#DC3545"" VerticalOptions=""Center"" />
    <Rock:Icon Grid.Column=""1"" IconClass=""chevron-right"" IconFamily=""FontAwesomeSolid"" FontSize=""15"" TextColor=""#DC3545"" VerticalOptions=""Center"" />
  </Grid>
  {%- endif -%}

</VerticalStackLayout>" );   // Content
            RockMigrationHelper.AddBlockAttributeValue( "4CAF5978-5265-4633-8021-D53E30C318EE", "B31D29A0-3725-4AEB-8360-7D91B9CDFE47", @"True" );   // DynamicContent
            RockMigrationHelper.AddBlockAttributeValue( "783BB975-D313-4722-A444-D3FF6EE06B3B", "14525BC1-7CF3-4EAC-B889-0F80FC503535", @"False" );   // EnableAuth0Login
            RockMigrationHelper.AddBlockAttributeValue( "783BB975-D313-4722-A444-D3FF6EE06B3B", "661AE50E-9125-40CB-BFE8-A978DF17BC4A", @"True" );   // EnableDatabaseLogin
            RockMigrationHelper.AddBlockAttributeValue( "783BB975-D313-4722-A444-D3FF6EE06B3B", "446FD78A-1855-4ED8-8EE9-23DDDB1F4B7B", @"Login With Auth0" );   // Auth0LoginButtonText
            RockMigrationHelper.AddBlockAttributeValue( "783BB975-D313-4722-A444-D3FF6EE06B3B", "1D907103-CA35-4E16-BC3A-633CB3EA4B1C", @"False" );   // EnableEntraLogin
            RockMigrationHelper.AddBlockAttributeValue( "783BB975-D313-4722-A444-D3FF6EE06B3B", "D89CA35C-B607-49DD-A89B-46663E2300F7", @"Login With Entra" );   // EntraLoginButtonText
            RockMigrationHelper.AddBlockAttributeValue( "E44BCD5E-AFA1-42D2-8371-0041ADB65CC5", "D77299F8-37F8-4F3C-8747-A9F1C7C5CEF1", @"35a494cc-15db-46a3-b28b-571d16dddff1" );   // WorkflowType
            RockMigrationHelper.AddBlockAttributeValue( "E44BCD5E-AFA1-42D2-8371-0041ADB65CC5", "87BAB537-0EB1-4894-B72B-D70472C802D7", @"0" );   // CompletionAction
            RockMigrationHelper.AddBlockAttributeValue( "0AF332B5-1F4C-4D17-B3C6-91C899A6C6FC", "5682EDBF-68DA-4B43-A593-6C2B936C2839", @"<!--
  Reusable Menu / List template (Nfluence Rock Mobile)
  ONE template drives: curated recursive menus (Sundays, Explore, sub-menus)
  AND data-driven lists (Sermon Notes, Staff) by pointing at a different channel.

  Config block below — set per placement:
    menuChannelId : App Menu channel Id (for a data-list, the data channel's Id)
    menuKey       : curated menu -> which menu to show (Sundays tab='sundays', Explore='explore');
                    sub-menu pages read it from PageParameter.MenuKey
    forcedStyle   : data-list -> force one style for every row (e.g. 'Meta Row', 'Avatar Row');
                    leave BLANK for a curated menu (per-item DisplayStyle)
    sortBy        : 'Order' for menus; 'StartDateTime desc' for sermons
  Block settings: Dynamic Content = Yes, Rock Entity command enabled.
-->
<VerticalStackLayout Spacing=""0"" StyleClass=""pt-12,pb-16"">

{%- assign menuChannelGuid = 'DF6276E8-B00E-4E77-88D8-AB1241A4A8C7' -%}
{%- assign menuChannelId = '' -%}
{% contentchannel where:'Guid == ""{{ menuChannelGuid }}""' securityenabled:'false' %}{%- for ch in contentchannelItems -%}{%- assign menuChannelId = ch.Id -%}{%- endfor -%}{% endcontentchannel %}
{%- assign menuKey = PageParameter.MenuKey | Default:'profile' -%}
{%- assign forcedStyle = '' -%}
{%- assign sortBy = 'Order' -%}

{%- assign menuPageGuid = 'a93cfc38-98f5-41de-b68d-6e7ee97f2d46' -%}
{%- assign itemDetailGuid = 'c9d8bd2d-8f1e-42e6-a4c3-b71b0511e9c7' -%}
{%- assign webviewGuid = 'c543ef2a-df73-4c21-bb01-94f2a6cb6373' -%}
{%- assign isListMode = false -%}
{%- if forcedStyle != '' -%}{% assign isListMode = true %}{%- endif -%}

{%- contentchannelitem where:'ContentChannelId == ""{{ menuChannelId }}""' sort:'{{ sortBy }}' -%}
{%- for item in contentchannelitemItems -%}
  {%- assign itemMenu = item | Attribute:'Menu','RawValue' -%}
  {%- if isListMode or itemMenu == menuKey -%}

  {%- if isListMode -%}{% assign style = forcedStyle %}{%- else -%}{% assign style = item | Attribute:'DisplayStyle','RawValue' %}{%- endif -%}
  {%- assign icon = item | Attribute:'Icon' -%}
  {%- assign subtitle = item | Attribute:'Subtitle' -%}
  {%- assign img = item | Attribute:'Image','Url' -%}
  {%- assign imgUrl = item | Attribute:'ImageUrl' -%}
  {%- if imgUrl == '' -%}{% assign imgUrl = img %}{%- endif -%}

  {%- assign subMenu = item | Attribute:'OpensSubMenu','RawValue' -%}
  {%- assign linkType = item | Attribute:'LinktoURLType','RawValue' -%}
  {%- assign linkUrlVal = item | Attribute:'LinktoURL' -%}
  {%- assign appPage = item | Attribute:'LinktoAppPage','RawValue' -%}
  {%- assign pageParams = item | Attribute:'PageParameters' -%}
  {%- assign contentStripped = item.Content | StripHtml | Trim -%}

  {%- capture cmd -%}
  {%- if subMenu != '' -%}{Binding PushPage}
  {%- elsif linkType == 'External Browser' -%}{Binding OpenExternalBrowser}
  {%- elsif linkType == 'Internal Browser' -%}{Binding OpenBrowser}
  {%- elsif linkUrlVal != '' or appPage != '' or contentStripped != '' -%}{Binding PushPage}
  {%- endif -%}
  {%- endcapture -%}{% assign cmd = cmd | Trim %}

  {%- capture param -%}
  {%- if subMenu != '' -%}{{ menuPageGuid }}?MenuKey={{ subMenu }}&amp;Title={{ item.Title | UrlEncode }}
  {%- elsif linkUrlVal != '' and linkType == 'Webview' -%}{{ webviewGuid }}?url={{ linkUrlVal | UrlEncode }}
  {%- elsif linkUrlVal != '' -%}{{ linkUrlVal | Escape }}
  {%- elsif appPage != '' -%}{{ appPage | Split:',' | First }}{% if pageParams != '' %}{{ pageParams }}{% endif %}
  {%- elsif contentStripped != '' -%}{{ itemDetailGuid }}?ContentChannelItemId={{ item.Id }}
  {%- endif -%}
  {%- endcapture -%}{% assign param = param | Trim %}

  {%- if style == 'Section Header' -%}
  <Label Text=""{{ item.Title | Escape }}"" StyleClass=""caption1,font-weight-semi-bold,text-interface-soft,px-16,mt-16,mb-4"" />

  {%- elsif style == 'Hero Banner' -%}
  <Rock:Image Source=""{{ imgUrl | Escape }}"" Aspect=""AspectFill"" Ratio=""16:9"" StyleClass=""mb-8"">
    {%- if cmd != '' -%}<Rock:Image.GestureRecognizers><TapGestureRecognizer Command=""{{ cmd }}"" CommandParameter=""{{ param }}"" /></Rock:Image.GestureRecognizers>{%- endif -%}
  </Rock:Image>

  {%- elsif style == 'Image Card' -%}
  <Rock:StyledBorder StrokeThickness=""0"" Padding=""0"" CornerRadius=""16"" StyleClass=""bg-interface-softest,mx-16,my-8"">
    <Rock:StyledBorder.Shadow><Shadow Brush=""#000000"" Offset=""0,4"" Radius=""12"" Opacity=""0.15"" /></Rock:StyledBorder.Shadow>
    {%- if cmd != '' -%}<Rock:StyledBorder.GestureRecognizers><TapGestureRecognizer Command=""{{ cmd }}"" CommandParameter=""{{ param }}"" /></Rock:StyledBorder.GestureRecognizers>{%- endif -%}
    <VerticalStackLayout Spacing=""0"">
      <Rock:Image Source=""{{ imgUrl | Escape }}"" Aspect=""AspectFill"" Ratio=""16:9"" />
      <VerticalStackLayout StyleClass=""p-16"" Spacing=""4"">
        <Label Text=""{{ item.Title | Escape }}"" StyleClass=""title3,text-interface-strongest"" />
        {%- if subtitle != '' -%}<Label Text=""{{ subtitle | Escape }}"" StyleClass=""footnote,text-interface-soft"" />{%- endif -%}
      </VerticalStackLayout>
    </VerticalStackLayout>
  </Rock:StyledBorder>

  {%- else -%}
  <!-- Row styles: Icon Row, Plain Row, Thumbnail Row, Avatar Row, Meta Row -->
  <Rock:StyledBorder StrokeThickness=""0"" CornerRadius=""12"" Padding=""18,{% if subtitle == '' %}22{% else %}18{% endif %}"" StyleClass=""bg-interface-softest,mx-16,my-8"">
    <Rock:StyledBorder.Shadow><Shadow Brush=""#000000"" Offset=""0,2"" Radius=""8"" Opacity=""0.10"" /></Rock:StyledBorder.Shadow>
    {%- if cmd != '' -%}<Rock:StyledBorder.GestureRecognizers><TapGestureRecognizer Command=""{{ cmd }}"" CommandParameter=""{{ param }}"" /></Rock:StyledBorder.GestureRecognizers>{%- endif -%}
    <Grid ColumnDefinitions=""Auto,*,Auto"" ColumnSpacing=""14"" VerticalOptions=""Center"">
      {%- if icon != '' and style != 'Thumbnail Row' and style != 'Avatar Row' -%}
      <Rock:Icon Grid.Column=""0"" IconClass=""{{ icon }}"" FontSize=""22"" TextColor=""{Rock:PaletteColor Interface-Strong}"" VerticalOptions=""Center"" />
      {%- elsif style == 'Thumbnail Row' and imgUrl != '' -%}
      <Rock:Image Grid.Column=""0"" Source=""{{ imgUrl | Escape }}"" WidthRequest=""56"" HeightRequest=""56"" Aspect=""AspectFill"" StyleClass=""rounded"" />
      {%- elsif style == 'Avatar Row' and imgUrl != '' -%}
      <Rock:Image Grid.Column=""0"" Source=""{{ imgUrl | Escape }}"" WidthRequest=""52"" HeightRequest=""52"" Aspect=""AspectFill"" StyleClass=""rounded-full"" />
      {%- endif -%}
      <VerticalStackLayout Grid.Column=""1"" Spacing=""2"" VerticalOptions=""Center"">
        <Label Text=""{{ item.Title | Escape }}"" StyleClass=""body,font-weight-semi-bold,text-interface-strongest"" />
        {%- if subtitle != '' -%}<Label Text=""{{ subtitle | Escape }}"" StyleClass=""footnote,text-interface-soft"" />{%- endif -%}
      </VerticalStackLayout>
      <Rock:Icon Grid.Column=""2"" IconClass=""chevron-right"" IconFamily=""FontAwesomeSolid"" FontSize=""15"" TextColor=""{Rock:PaletteColor Interface-Soft}"" VerticalOptions=""Center"" />
    </Grid>
  </Rock:StyledBorder>
  {%- endif -%}

  {%- endif -%}
{%- endfor -%}
{%- endcontentchannelitem -%}
</VerticalStackLayout>" );   // Content
            RockMigrationHelper.AddBlockAttributeValue( "0AF332B5-1F4C-4D17-B3C6-91C899A6C6FC", "24516448-3F1F-4F27-97A1-CFB4F8B277B5", @"RockEntity" );   // EnabledLavaCommands
            RockMigrationHelper.AddBlockAttributeValue( "0AF332B5-1F4C-4D17-B3C6-91C899A6C6FC", "B31D29A0-3725-4AEB-8360-7D91B9CDFE47", @"True" );   // DynamicContent
            RockMigrationHelper.AddBlockAttributeValue( "BEBEC594-4C65-411E-8013-BAC2983D2DD8", "694244DC-5067-4A34-98F3-85FED7052E18", @"8a444668-19af-4417-9c74-09f842572974" );   // Calendar
            RockMigrationHelper.AddBlockAttributeValue( "BEBEC594-4C65-411E-8013-BAC2983D2DD8", "91A13A0A-2D7A-45AE-BF09-D897C280C4E1", @"a2156601-d477-465e-addf-e745dee935f5" );   // DetailPage
            RockMigrationHelper.AddBlockAttributeValue( "BEBEC594-4C65-411E-8013-BAC2983D2DD8", "497A6BD6-D36C-4AC8-AF83-9015EAF43C89", @"<Rock:StyledBorder CornerRadius=""12"" Padding=""16,12"">
    <Grid ColumnDefinitions=""*, Auto"" ColumnSpacing=""12"">
        <StackLayout Spacing=""2"" VerticalOptions=""Center"">
            <Label StyleClass=""headline, bold, text-interface-strongest""
                Text=""{Binding Name}"" />

            {% if Item.EndDateTime == null %}
                <Label StyleClass=""caption1, text-interface-medium""
                    Text=""{{ Item.StartDateTime | Date:'h:mm tt' | Upcase }}""
                    LineBreakMode=""NoWrap"" />
            {% else %}
                <Label StyleClass=""caption1, text-interface-medium""
                    Text=""{{ Item.StartDateTime | Date:'h:mm tt' | Upcase }} - {{ Item.EndDateTime | Date:'h:mm tt' | Upcase }}""
                    LineBreakMode=""NoWrap"" />
            {% endif %}
        </StackLayout>

        <Rock:Icon Grid.Column=""1""
            IconClass=""chevron-right""
            IconFamily=""FontAwesomeSolid""
            FontSize=""16""
            StyleClass=""text-interface-soft""
            VerticalOptions=""Center"" />
    </Grid>
</Rock:StyledBorder>" );   // EventSummary
            RockMigrationHelper.AddBlockAttributeValue( "BEBEC594-4C65-411E-8013-BAC2983D2DD8", "9E75484E-9F6B-4E36-A73A-C1AE06A48CE1", @"True" );   // ShowFilter
            RockMigrationHelper.AddBlockAttributeValue( "E44BCD5E-AFA1-42D2-8371-0041ADB65CC5", "370F3617-CE26-4FA8-96CA-26B82E4D4F15", @"0" );   // ScanMode
            RockMigrationHelper.AddBlockAttributeValue( "9C074994-EE12-41C8-8072-49A3012A72E8", "D9AAF055-24B9-4BF5-A2A3-2405993D9010", @"2" );   // Gender
            RockMigrationHelper.AddBlockAttributeValue( "5E4FFC63-BAA6-4676-AD4C-3C7E0034E0BD", "5682EDBF-68DA-4B43-A593-6C2B936C2839", @"{%- assign includeGroupTypeIds = '25,23' -%}
{%- assign groupViewPageGuid = '73143e47-3c0e-44bc-8815-0021f88e9f72' -%}

<VerticalStackLayout Spacing=""0"" StyleClass=""p-16"">

    <Label Text=""MY GROUPS""
        StyleClass=""caption1, font-weight-semi-bold, text-interface-medium""
        Margin=""4,0,0,8"" />

    {%- assign found = 0 -%}
    {% groupmember where:'PersonId == {{ CurrentPerson.Id }} && GroupMemberStatus == 1 && IsArchived == false' sort:'Group.Name' %}
        {% for gm in groupmemberItems %}
            {%- assign gtId = gm.Group.GroupTypeId | ToString -%}
            {%- assign matched = includeGroupTypeIds | Split:',' | Where:'.', gtId -%}
            {% if gm.Group.IsActive and matched != empty %}
                {%- assign found = found | Plus:1 -%}

                <Rock:StyledBorder CornerRadius=""12"" Padding=""16,14"" StyleClass=""bg-interface-softest, my-4"">
                    <Rock:StyledBorder.GestureRecognizers>
                        <TapGestureRecognizer Command=""{Binding PushPage}""
                            CommandParameter=""{{ groupViewPageGuid }}?GroupGuid={{ gm.Group.Guid }}"" />
                    </Rock:StyledBorder.GestureRecognizers>

                    <Grid ColumnDefinitions=""Auto, *, Auto"" ColumnSpacing=""14"">
                        <Rock:Icon Grid.Column=""0""
                            IconClass=""users""
                            IconFamily=""FontAwesomeSolid""
                            FontSize=""20""
                            StyleClass=""text-interface-stronger""
                            VerticalOptions=""Center"" />

                        <VerticalStackLayout Grid.Column=""1"" Spacing=""6"" VerticalOptions=""Center"">
                            <Label StyleClass=""headline, bold, text-interface-strongest""
                                Text=""{{ gm.Group.Name | Escape }}"" />

                            <Rock:StyledBorder CornerRadius=""10"" Padding=""8,3""
                                StyleClass=""bg-interface-softer""
                                HorizontalOptions=""Start"">
                                <Grid ColumnDefinitions=""Auto, Auto"" ColumnSpacing=""6"">
                                    <Rock:Icon Grid.Column=""0"" IconClass=""user""
                                        IconFamily=""FontAwesomeSolid"" FontSize=""11""
                                        TextColor=""{Rock:PaletteColor App-Primary-Strong}"" VerticalOptions=""Center"" />
                                    <Label Grid.Column=""1""
                                        StyleClass=""caption1, text-interface-medium""
                                        Text=""{{ gm.GroupRole.Name | Escape }}""
                                        VerticalOptions=""Center"" />
                                </Grid>
                            </Rock:StyledBorder>
                        </VerticalStackLayout>

                        <Rock:Icon Grid.Column=""2"" IconClass=""chevron-right""
                            IconFamily=""FontAwesomeSolid"" FontSize=""16""
                            StyleClass=""text-interface-soft"" VerticalOptions=""Center"" />
                    </Grid>
                </Rock:StyledBorder>
            {% endif %}
        {% endfor %}
    {% endgroupmember %}

    {% if found == 0 %}
        <Label StyleClass=""body, text-interface-medium""
            Text=""You're not in any groups yet.""
            Margin=""4,12,0,0"" />
    {% endif %}

</VerticalStackLayout>" );   // Content
            RockMigrationHelper.AddBlockAttributeValue( "5E4FFC63-BAA6-4676-AD4C-3C7E0034E0BD", "24516448-3F1F-4F27-97A1-CFB4F8B277B5", @"RockEntity" );   // EnabledLavaCommands
            RockMigrationHelper.AddBlockAttributeValue( "5E4FFC63-BAA6-4676-AD4C-3C7E0034E0BD", "B31D29A0-3725-4AEB-8360-7D91B9CDFE47", @"True" );   // DynamicContent
            RockMigrationHelper.AddBlockAttributeValue( "B36C1077-5312-44E3-ACFF-DCB32EC72B4A", "96CC7902-C81F-463F-A3A1-85D36ACE3618", @"ffffffff-ffff-ffff-ffff-ffffffffffff|{%- assign regAppRoot = 'Global' | Attribute:'PublicApplicationRoot' -%}
{%- assign regToken = '' -%}
{%- if CurrentPerson -%}{%- capture regToken -%}&rckipid={{ CurrentPerson | PersonTokenCreate }}{%- endcapture -%}{%- endif -%}
{%- comment -%}
    Off-site registration and the event's built-in Details Url.

    ""ItemLink"" is an EventCalendarItem attribute on the Public calendar, and
    EventItem inherits its calendars' attributes (EventItem.GetInheritedAttributes),
    so it usually resolves straight off Event. The EventCalendarItems loop is a
    fallback for when inheritance has not been primed.

    regToken is deliberately NOT appended to either of these - they point at
    third-party sites, and rckipid is an impersonation token.
{%- endcomment -%}
{%- assign itemLink = Event | Attribute:'ItemLink','RawValue' -%}
{%- if itemLink == '' or itemLink == null -%}
    {%- for eci in Event.EventCalendarItems -%}
        {%- assign eciLink = eci | Attribute:'ItemLink','RawValue' -%}
        {%- if eciLink != '' and itemLink == '' -%}{%- assign itemLink = eciLink -%}{%- endif -%}
    {%- endfor -%}
{%- endif -%}
{%- assign itemLinkText = Event | Attribute:'ItemLinkText' -%}
{%- if itemLinkText == '' or itemLinkText == null -%}{%- assign itemLinkText = 'Register' -%}{%- endif -%}
{%- assign detailsUrl = Event.DetailsUrl -%}
{%- assign detailsUrlText = Event | Attribute:'DetailsUrlButtonText' -%}
{%- if detailsUrlText == '' or detailsUrlText == null -%}{%- assign detailsUrlText = 'More Information' -%}{%- endif -%}
{%- assign scheduledDates = EventItemOccurrence.Schedule.iCalendarContent | DatesFromICal:'all' -%}
{%- assign dateCount = scheduledDates | Size -%}
{%- assign firstDate = scheduledDates | First -%}
{%- assign lastDate = scheduledDates | Last -%}
{%- assign spanDays = firstDate | DateDiff:lastDate,'d' -%}

{%- comment -%}
  Date summary logic:
    1 date              -> ""AUG 2, 2026""
    short span (<=14d)  -> ""OCT 2 - OCT 3, 2026""   (a real multi-day event)
    long span           -> ""NEXT: AUG 2, 2026""     (a recurring series; a first-to-last
                                                    range would read as e.g. Aug 2 - Jul 25)
{%- endcomment -%}
{%- capture dateSummary -%}
{%- if dateCount == 1 -%}
{{ firstDate | Date:'MMM d, yyyy' }}
{%- elseif spanDays <= 14 -%}
{{ firstDate | Date:'MMM d' }} - {{ lastDate | Date:'MMM d, yyyy' }}
{%- else -%}
NEXT: {{ firstDate | Date:'MMM d, yyyy' }}
{%- endif -%}
{%- endcapture -%}

<VerticalStackLayout Spacing=""20"">

    <!-- ===== Hero image ===== -->
    {% if Event.Photo.Guid %}
        <Rock:Image Source=""{{ 'Global' | Attribute:'PublicApplicationRoot' }}/GetImage.ashx?Guid={{ Event.Photo.Guid }}""
            Aspect=""AspectFit""
            MaximumHeightRequest=""420"">
            <Rock:RoundedTransformation CornerRadius=""12"" />
        </Rock:Image>
    {% endif %}

    <!-- ===== Title + orange date summary ===== -->
    <VerticalStackLayout Spacing=""4"">
        <Label StyleClass=""title1, bold, text-interface-strongest""
            Text=""{{ Event.Name | Escape }}"" />

        <Label StyleClass=""headline, bold""
            TextColor=""{Rock:PaletteColor App-Primary-Strong}""
            Text=""{{ dateSummary | Trim | Upcase }}""
            LineBreakMode=""NoWrap"" />
    </VerticalStackLayout>

    <!-- ===== Action row: SAVE EVENT | SHARE ===== -->
    <Grid ColumnDefinitions=""*, *"" ColumnSpacing=""8"">

        <VerticalStackLayout Grid.Column=""0"" Spacing=""6"" HorizontalOptions=""Center"">
            <Rock:Icon IconClass=""calendar-plus""
                IconFamily=""FontAwesomeSolid""
                FontSize=""22""
                StyleClass=""text-interface-stronger""
                HorizontalOptions=""Center"" />
            <Label StyleClass=""caption1, font-weight-semi-bold, text-interface-medium""
                Text=""SAVE EVENT""
                HorizontalOptions=""Center"" />
            <VerticalStackLayout.GestureRecognizers>
                <TapGestureRecognizer Command=""{Binding AddEventToCalendar}"">
                    <TapGestureRecognizer.CommandParameter>
                        <Rock:AddEventToCalendarParameters
                            Title=""{{ Event.Name | Escape }}""
                            StartDateTime=""{{ firstDate | Date:'yyyy-MM-dd HH:mm:ss' }}""
                            EndDateTime=""{{ firstDate | DateAdd:1,'h' | Date:'yyyy-MM-dd HH:mm:ss' }}""
                            TimeZoneId=""America/Indiana/Indianapolis"" />
                    </TapGestureRecognizer.CommandParameter>
                </TapGestureRecognizer>
            </VerticalStackLayout.GestureRecognizers>
        </VerticalStackLayout>

        <VerticalStackLayout Grid.Column=""1"" Spacing=""6"" HorizontalOptions=""Center"">
            <Rock:Icon IconClass=""share-square""
                IconFamily=""FontAwesomeSolid""
                FontSize=""22""
                StyleClass=""text-interface-stronger""
                HorizontalOptions=""Center"" />
            <Label StyleClass=""caption1, font-weight-semi-bold, text-interface-medium""
                Text=""SHARE""
                HorizontalOptions=""Center"" />
            <VerticalStackLayout.GestureRecognizers>
                <TapGestureRecognizer Command=""{Binding ShareContent}"">
                    <TapGestureRecognizer.CommandParameter>
                        <Rock:ShareContentParameters
                            Title=""{{ Event.Name | Escape }}""
                            Text=""{{ Event.Name | Escape }} - {{ dateSummary | Trim }}""
                            Uri=""{{ 'Global' | Attribute:'PublicApplicationRoot' }}event/{{ EventItemOccurrence.Id }}"" />
                    </TapGestureRecognizer.CommandParameter>
                </TapGestureRecognizer>
            </VerticalStackLayout.GestureRecognizers>
        </VerticalStackLayout>

    </Grid>

    <BoxView HeightRequest=""1"" StyleClass=""bg-interface-softer"" />

    <!-- ===== Date / Time, Location, Contact ===== -->
    <Rock:FieldContainer FieldLayout=""Individual"">
        {% assign scheduleListing = '' %}
        {% for scheduledDate in scheduledDates %}
            {% if forloop.index <= 5 %}
                {% assign scheduleDateTime = scheduledDate | Date:'dddd, MMMM d, yyyy @ h:mm tt' %}
                {% assign scheduleListing = scheduleListing | Append:scheduleDateTime | Append:'&#xa;' %}
            {% endif %}
        {% endfor %}

        <Rock:Literal Label=""Date / Time"" Text=""{{ scheduleListing | ReplaceLast:'&#xa;', '' }}"" />

        {% if EventItemOccurrence.Location != '' %}
            <Rock:Literal Label=""Location"" Text=""{{ EventItemOccurrence.Location }}"" />
        {% endif %}

        {% if EventItemOccurrence.ContactPersonAliasId != null or EventItemOccurrence.ContactEmail != '' or EventItemOccurrence.ContactPhone != '' %}
            {% if EventItemOccurrence.ContactPersonAliasId != null %}
                <Rock:Literal Label=""Contact"" Text=""{{ EventItemOccurrence.ContactPersonAlias.Person.FullName | Escape }}"" />
            {% endif %}
            {% if EventItemOccurrence.ContactEmail != '' %}
                <Rock:Literal Label=""Contact Email"" Text=""{{ EventItemOccurrence.ContactEmail | Escape }}"" />
            {% endif %}
            {% if EventItemOccurrence.ContactPhone != '' %}
                <Rock:Literal Label=""Contact Phone"" Text=""{{ EventItemOccurrence.ContactPhone | Escape }}"" />
            {% endif %}
        {% endif %}
    </Rock:FieldContainer>

    <BoxView HeightRequest=""1"" StyleClass=""bg-interface-softer"" />

    <!-- ===== Description ===== -->
    <Rock:Html StyleClass=""body, text-interface-stronger"">
        {{ Event.Description | Escape }}
    </Rock:Html>

    <!-- ===== Registration (unchanged from the stock template) ===== -->
{%- comment -%} An off-site ItemLink stands in for Rock registration entirely {%- endcomment -%}
{% if itemLink != '' and itemLink != null %}
    <Button Text=""{{ itemLinkText | Escape }}"" Command=""{Binding OpenBrowser}""
        CommandParameter=""{{ itemLink | Escape }}"" StyleClass=""btn, btn-primary"" />
{% endif %}

{% if detailsUrl != '' and detailsUrl != null %}
    <Button Text=""{{ detailsUrlText | Escape }}"" Command=""{Binding OpenBrowser}""
        CommandParameter=""{{ detailsUrl | Escape }}"" StyleClass=""btn, btn-secondary"" />
{% endif %}

    {% assign showRegistration = false %}
    {% assign eventItemOccurrenceLinkages = EventItemOccurrence.Linkages %}

    {% assign eventItemOccurrenceLinkagesCount = eventItemOccurrenceLinkages | Size %}
    {% if itemLink != '' and itemLink != null %}{% assign eventItemOccurrenceLinkagesCount = 0 %}{% endif %}
    {% if eventItemOccurrenceLinkagesCount > 0 %}
        {% for eventItemOccurrenceLinkage in eventItemOccurrenceLinkages %}
            {% assign daysTillStartDate = 'Now' | DateDiff:eventItemOccurrenceLinkage.RegistrationInstance.StartDateTime,'m' %}
            {% assign daysTillEndDate = 'Now' | DateDiff:eventItemOccurrenceLinkage.RegistrationInstance.EndDateTime,'m' %}
            {% assign showRegistration = true %}
            {% assign registrationMessage = '' %}

            {% if daysTillStartDate and daysTillStartDate > 0 %}
                {% assign showRegistration = false %}
                {% if eventItemOccurrenceLinkagesCount == 1 %}
                  {% capture registrationMessage %}Registration opens on {{ eventItemOccurrenceLinkage.RegistrationInstance.StartDateTime | Date:'dddd, MMMM d, yyyy' }}{% endcapture %}
                {% else %}
                  {% capture registrationMessage %}Registration for {{ eventItemOccurrenceLinkage.PublicName }} opens on {{ eventItemOccurrenceLinkage.RegistrationInstance.StartDateTime | Date:'dddd, MMMM d, yyyy' }}{% endcapture %}
                {% endif %}
            {% endif %}

            {% if daysTillEndDate and daysTillEndDate < 0 %}
                {% assign showRegistration = false %}
                {% if eventItemOccurrenceLinkagesCount == 1 %}
                  {% capture registrationMessage %}Registration closed on {{ eventItemOccurrenceLinkage.RegistrationInstance.EndDateTime | Date:'dddd, MMMM d, yyyy' }}{% endcapture %}
                {% else %}
                  {% capture registrationMessage %}Registration for {{ eventItemOccurrenceLinkage.PublicName }} closed on {{ eventItemOccurrenceLinkage.RegistrationInstance.EndDateTime | Date:'dddd, MMMM d, yyyy' }}{% endcapture %}
                {% endif %}
            {% endif %}

            {% if showRegistration == true %}
                {% assign statusLabel = RegistrationStatusLabels[eventItemOccurrenceLinkage.RegistrationInstanceId] %}
                {% if eventItemOccurrenceLinkagesCount == 1 %}
                  {% assign registrationButtonText = statusLabel %}
                {% else %}
                  {% assign registrationButtonText = statusLabel | Plus:' for ' | Plus:eventItemOccurrenceLinkage.PublicName %}
                {% endif %}

                {% if statusLabel == 'Full' %}
                    {% if eventItemOccurrenceLinkagesCount == 1 %}
                      {% assign registrationButtonText = 'Registration Full' %}
                    {% else %}
                      {% assign registrationButtonText = eventItemOccurrenceLinkage.PublicName | Plus: ' (Registration Full) ' %}
                    {% endif %}
                    <Label StyleClass=""body, bold, text-interface-stronger"">{{ registrationButtonText }}</Label>
                {% else %}
                    {% if eventItemOccurrenceLinkage.UrlSlug != '' %}
                        {%- capture regUrl -%}{{ regAppRoot }}Registration?RegistrationInstanceId={{ eventItemOccurrenceLinkage.RegistrationInstanceId }}&Slug={{ eventItemOccurrenceLinkage.UrlSlug }}{{ regToken }}{%- endcapture -%}
                        <Button Text=""{{ registrationButtonText | Escape }}"" Command=""{Binding OpenBrowser}""
                            CommandParameter=""{{ regUrl | Escape }}"" StyleClass=""btn, btn-primary"" />
                    {% else %}
                        {%- capture regUrl -%}{{ regAppRoot }}Registration?RegistrationInstanceId={{ eventItemOccurrenceLinkage.RegistrationInstanceId }}&EventOccurrenceId={{ eventItemOccurrenceLinkage.EventItemOccurrenceId }}{{ regToken }}{%- endcapture -%}
                        <Button Text=""{{ registrationButtonText | Escape }}"" Command=""{Binding OpenBrowser}""
                            CommandParameter=""{{ regUrl | Escape }}"" StyleClass=""btn, btn-primary"" />
                    {% endif %}
                {% endif %}
            {% else %}
              <Label StyleClass=""body, bold, text-interface-stronger"" Text=""Registration Information"" />
              <Label StyleClass=""body, text-interface-strong"" Text=""{{ registrationMessage | Escape }}"" />
            {% endif %}
        {% endfor %}
    {% endif %}
</VerticalStackLayout>
" );   // Template
            RockMigrationHelper.AddBlockAttributeValue( "98963BBF-5F11-4B6B-ACCA-ECFFFDB96480", "808D607F-D097-48C5-BC3A-988141A1C69C", @"True" );   // ShowLeaderList
            RockMigrationHelper.AddBlockAttributeValue( "98963BBF-5F11-4B6B-ACCA-ECFFFDB96480", "A3826811-395A-4564-8101-EB95936065FB", @"ffffffff-ffff-ffff-ffff-ffffffffffff|{%- assign groupMemberCount = Group.Members | Size -%}
{%- assign leaderCount = 0 -%}
{%- for m in Group.Members -%}{%- if m.GroupRole.IsLeader -%}{%- assign leaderCount = leaderCount | Plus:1 -%}{%- endif -%}{%- endfor -%}
{%- assign memberCount = groupMemberCount | Minus:leaderCount -%}
{%- assign showAttributeKeys = 'Arena_ChildcareProvided,Topic,SmallGroupTopicMulti,GroupTags,SharedInterest' -%}
{%- assign showKeyList = showAttributeKeys | Split:',' -%}
{%- assign rosterPageGuid = 'BD9535DD-DA9C-4CEC-9397-2E429BE4E6C0' -%}
{%- assign appRoot = 'Global' | Attribute:'PublicApplicationRoot' -%}
{%- assign carousel = Group.Members | OrderBy:'GroupRole.Order' -%}

<StackLayout StyleClass=""spacing-24"">

    <!-- ===== Member count + View/Edit Members ===== -->
    <Grid ColumnDefinitions=""*, Auto"" VerticalOptions=""Center"">
        <VerticalStackLayout Grid.Column=""0"" Spacing=""2"" VerticalOptions=""Center"">
            <Label StyleClass=""headline, bold, text-interface-strongest"" Text=""{{ groupMemberCount }} MEMBERS"" />
            <Label StyleClass=""caption1, text-interface-medium""
                Text=""{{ leaderCount }} Leader{% if leaderCount != 1 %}s{% endif %} &#8226; {{ memberCount }} Member{% if memberCount != 1 %}s{% endif %}"" />
        </VerticalStackLayout>
        <Label Grid.Column=""1"" StyleClass=""body, bold, text-primary-strong"" Text=""VIEW/EDIT MEMBERS"" VerticalOptions=""Center"">
            <Label.GestureRecognizers>
                <TapGestureRecognizer Command=""{Binding PushPage}"" CommandParameter=""{{ rosterPageGuid }}?GroupGuid={{ Group.Guid }}"" />
            </Label.GestureRecognizers>
        </Label>
    </Grid>

    <!-- ===== Member carousel ===== -->
    <ScrollView Orientation=""Horizontal"" HorizontalScrollBarVisibility=""Never"">
        <StackLayout Orientation=""Horizontal"" StyleClass=""spacing-16"">
            {% for member in carousel limit:20 %}
                <VerticalStackLayout WidthRequest=""64"" Spacing=""6"" HorizontalOptions=""Center"">
                    <Rock:Avatar Source=""{{ appRoot }}{{ member.Person.PhotoUrl | Escape }}""
                        HeightRequest=""56"" WidthRequest=""56"" ShowStroke=""false"" HorizontalOptions=""Center"" />
                    <Label StyleClass=""caption2, text-interface-medium"" Text=""{{ member.Person.NickName | Escape }}""
                        HorizontalTextAlignment=""Center"" MaxLines=""1"" LineBreakMode=""TailTruncation"" />
                </VerticalStackLayout>
            {% endfor %}
        </StackLayout>
    </ScrollView>

    <!-- ===== Description ===== -->
    {% if Group.Description and Group.Description != '' %}
        <StackLayout StyleClass=""spacing-8"">
            <Label StyleClass=""caption1, font-weight-semi-bold, text-interface-medium"" Text=""DESCRIPTION"" />
            <Label StyleClass=""body, text-interface-strong"" Text=""{{ Group.Description | Escape }}"" />
        </StackLayout>
    {% endif %}

    <!-- ===== Group details ===== -->
    {% if VisibleAttributes != empty %}
        <StackLayout StyleClass=""spacing-8"">
            <Label StyleClass=""caption1, font-weight-semi-bold, text-interface-medium"" Text=""GROUP DETAILS"" />
            {% for attribute in VisibleAttributes %}
                {%- assign keyMatch = showKeyList | Where:'.', attribute.Key -%}
                {% if attribute.FormattedValue != '' and keyMatch != empty %}
                    <Grid ColumnDefinitions=""Auto, *"" ColumnSpacing=""16"">
                        <Label Grid.Column=""0"" StyleClass=""body, text-interface-strongest"" Text=""{{ attribute.Name | Escape }}"" VerticalOptions=""Center"" />
                        <Label Grid.Column=""1"" StyleClass=""body, text-interface-medium"" Text=""{{ attribute.FormattedValue | StripHtml | Escape }}"" HorizontalTextAlignment=""End"" VerticalOptions=""Center"" />
                    </Grid>
                    <BoxView HeightRequest=""1"" StyleClass=""bg-interface-softer"" />
                {% endif %}
            {% endfor %}
        </StackLayout>
    {% endif %}

</StackLayout>
" );   // Template
            RockMigrationHelper.AddBlockAttributeValue( "BEBEC594-4C65-411E-8013-BAC2983D2DD8", "DD3BCE0E-BE48-4F68-B1E2-176426124FDB", @"True" );   // ShowAllEventsInDetail
            RockMigrationHelper.AddBlockAttributeValue( "BEBEC594-4C65-411E-8013-BAC2983D2DD8", "37BC8C51-F20E-4FFC-9C3B-42D515C6FA94", @"False" );   // ShowPerAudienceEventIndicators
            RockMigrationHelper.AddBlockAttributeValue( "597C9DB9-24E6-4FBF-A97D-A21A32F3B81D", "FB6FA5A4-74C7-4E17-8764-118C01FCD192", @"Group Roster" );   // TitleTemplate
            RockMigrationHelper.AddBlockAttributeValue( "597C9DB9-24E6-4FBF-A97D-A21A32F3B81D", "4B1F3DAE-180E-45E6-BC36-2BEA0A03C674", @"ffffffff-ffff-ffff-ffff-ffffffffffff|<Rock:StyledBorder StyleClass=""bg-interface-softest, border, border-interface-soft, rounded, p-16"">
    <VerticalStackLayout>
        {% for member in Members %}
            {%- assign phoneDigits = member.MobilePhone | Remove:' ' | Remove:'(' | Remove:')' | Remove:'-' | Remove:'.' | Remove:'+' -%}
            <Grid RowDefinitions=""48, Auto"" ColumnDefinitions=""Auto, *, Auto"" StyleClass=""gap-column-12"">

                <Rock:Avatar Source=""{{ member.PhotoUrl | Escape }}""
                    HeightRequest=""48"" ShowStroke=""false""
                    Grid.Row=""0"" Grid.Column=""0"" VerticalOptions=""Center"" />

                <StackLayout Grid.Column=""1"" VerticalOptions=""Center"">
                    <Label StyleClass=""body, bold, text-interface-stronger""
                        Text=""{{ member.FullName | Escape }}"" MaxLines=""1"" LineBreakMode=""TailTruncation"" />
                    <Label StyleClass=""footnote, text-interface-strong""
                        Text=""{{ member.GroupRole | Escape }}"" MaxLines=""1"" LineBreakMode=""TailTruncation"" />
                </StackLayout>

                <Rock:Icon Grid.Column=""2"" IconClass=""ellipsis-v"" IconFamily=""FontAwesomeSolid""
                    FontSize=""18"" VerticalOptions=""Center"" StyleClass=""text-interface-medium, px-8, py-4"">
                    <Rock:Icon.GestureRecognizers>
                        <TapGestureRecognizer Command=""{Binding ShowActionPanel}"">
                            <TapGestureRecognizer.CommandParameter>
                                <Rock:ShowActionPanelParameters Title=""{{ member.FullName | Escape }}"" CancelTitle=""Cancel"">
                                    {% if phoneDigits != '' %}
                                    <Rock:ActionPanelButton Title=""Voice Call"" Command=""{Binding CallPhoneNumber}"" CommandParameter=""{{ phoneDigits }}"" />
                                    <Rock:ActionPanelButton Title=""Text Message"" Command=""{Binding SendSms}"" CommandParameter=""{{ phoneDigits }}"" />
                                    {% endif %}
                                    {% if member.Email != '' %}
                                    <Rock:ActionPanelButton Title=""Email"" Command=""{Binding SendEmail}"" CommandParameter=""{{ member.Email }}"" />
                                    {% endif %}
                                </Rock:ShowActionPanelParameters>
                            </TapGestureRecognizer.CommandParameter>
                        </TapGestureRecognizer>
                    </Rock:Icon.GestureRecognizers>
                </Rock:Icon>

                {% unless forloop.last %}
                    <Rock:Divider Grid.Row=""1"" Grid.ColumnSpan=""3"" StyleClass=""my-8"" />
                {% endunless %}
            </Grid>
        {% endfor %}
    </VerticalStackLayout>
</Rock:StyledBorder>
" );   // Template
            RockMigrationHelper.AddBlockAttributeValue( "597C9DB9-24E6-4FBF-A97D-A21A32F3B81D", "D5942AD0-5EA5-4ACA-8D5F-66FDE86E2E61", @"[{""Key"":""Email"",""Value"":""{{ item.Person.Email }}"",""FieldFormat"":0},{""Key"":""MobilePhone"",""Value"":""{{ item.Person | PhoneNumber:'Mobile' }}"",""FieldFormat"":0}]" );   // AdditionalFields
            RockMigrationHelper.AddBlockAttributeValue( "A5F5F565-6542-47B4-8FD8-642B3CC3E7C6", "5682EDBF-68DA-4B43-A593-6C2B936C2839", @"{%- assign staffChannelGuid = '68A8458F-B632-46BA-A1F4-CB65C62DF18F' -%}
{%- assign staffChannelId = 0 -%}
{% contentchannel where:'Guid == ""{{ staffChannelGuid }}""' securityenabled:'false' %}{%- for ch in contentchannelItems -%}{%- assign staffChannelId = ch.Id -%}{%- endfor -%}{% endcontentchannel %}
{%- assign itemDetailPageGuid = 'cb293da2-94c5-469d-9413-42d59f603b37' -%}

<VerticalStackLayout Spacing=""0"" StyleClass=""p-16"">

    {% contentchannelitem where:'ContentChannelId == ""{{ staffChannelId }}""' sort:'Order' %}
        {% for item in contentchannelitemItems %}
            {%- assign photo = item | Attribute:'Image','Url' -%}
            {%- assign role = item | Attribute:'Role' -%}
            {%- assign displayName = item | Attribute:'DisplayName' -%}
            {%- if displayName == '' -%}{%- assign displayName = item.Title -%}{%- endif -%}

            <Rock:StyledBorder CornerRadius=""12"" Padding=""14""
                StyleClass=""bg-interface-softest, my-4"">
                <Rock:StyledBorder.GestureRecognizers>
                    <TapGestureRecognizer Command=""{Binding PushPage}""
                        CommandParameter=""{{ itemDetailPageGuid }}?ContentChannelItemId={{ item.Id }}"" />
                </Rock:StyledBorder.GestureRecognizers>

                <Grid ColumnDefinitions=""Auto, *, Auto"" ColumnSpacing=""14"">

                    <Rock:StyledBorder Grid.Column=""0""
                        CornerRadius=""28"" Padding=""0""
                        HeightRequest=""56"" WidthRequest=""56""
                        StyleClass=""bg-interface-softer""
                        VerticalOptions=""Center"">
                        {% if photo != '' %}
                            <Rock:Image Source=""{{ photo | Escape }}""
                                Aspect=""AspectFill""
                                HeightRequest=""56"" WidthRequest=""56"" />
                        {% else %}
                            <Rock:Icon IconClass=""user"" IconFamily=""FontAwesomeSolid""
                                FontSize=""22"" StyleClass=""text-interface-soft""
                                HorizontalOptions=""Center"" VerticalOptions=""Center"" />
                        {% endif %}
                    </Rock:StyledBorder>

                    <VerticalStackLayout Grid.Column=""1"" Spacing=""3"" VerticalOptions=""Center"">
                        <Label StyleClass=""headline, bold, text-interface-strongest""
                            Text=""{{ displayName | Escape }}"" />
                        {% if role != '' %}
                            <Label StyleClass=""caption1, font-weight-semi-bold, text-interface-medium""
                                Text=""{{ role | Upcase | Escape }}""
                                LineBreakMode=""TailTruncation"" />
                        {% endif %}
                    </VerticalStackLayout>

                    <Rock:Icon Grid.Column=""2"" IconClass=""chevron-right""
                        IconFamily=""FontAwesomeSolid"" FontSize=""16""
                        StyleClass=""text-interface-soft"" VerticalOptions=""Center"" />

                </Grid>
            </Rock:StyledBorder>
        {% endfor %}
    {% endcontentchannelitem %}

</VerticalStackLayout>" );   // Content
            RockMigrationHelper.AddBlockAttributeValue( "A5F5F565-6542-47B4-8FD8-642B3CC3E7C6", "24516448-3F1F-4F27-97A1-CFB4F8B277B5", @"RockEntity" );   // EnabledLavaCommands
            RockMigrationHelper.AddBlockAttributeValue( "A5F5F565-6542-47B4-8FD8-642B3CC3E7C6", "B31D29A0-3725-4AEB-8360-7D91B9CDFE47", @"True" );   // DynamicContent
            RockMigrationHelper.AddBlockAttributeValue( "00AF4314-F34E-4AEF-ADF6-462182AF8D89", "D80CF7C7-F6F4-4E77-97A8-B0842E4AF7FB", @"{%- assign photo = Item | Attribute:'Image','Url' -%}
{%- assign role = Item | Attribute:'Role' -%}
{%- assign subtitle = Item | Attribute:'Subtitle' -%}
{%- assign displayName = Item | Attribute:'DisplayName' -%}
{%- if displayName == '' -%}{%- assign displayName = Item.Title -%}{%- endif -%}

{%- comment -%}
    ===========================================================================
    ICON ROWS - shared by Contact Links and Social Media
    ---------------------------------------------------------------------------
    Both are Key Value List attributes stored as  key^value|key^value  where the
    KEY is an icon and the VALUE is the text shown to its right.

    The key accepts whatever someone naturally types:
        fa fa-linkedin      (what the existing Social Media data uses)
        fab fa-instagram
        envelope
    Everything down to the bare name is stripped off, then the family is picked
    from brandNames - Font Awesome puts company logos in Brands and everything
    else in Solid, and using the wrong family renders a blank glyph.

    An EMPTY key is deliberate and supported: it produces a text-only row that
    lines up under the ones with icons, which is how ""ext 6001"" sits under the
    phone number.

    Contact Links values are display text and are made tappable only when they
    are recognisably an email or a phone number. Social Media values are links,
    so they always open, and a missing scheme is filled in - people paste
    ""www.linkedin.com/in/x"" far more often than they paste a full URL.
    ===========================================================================
{%- endcomment -%}
{%- assign brandNames = 'facebook,facebook-f,facebook-square,twitter,twitter-square,x-twitter,instagram,instagram-square,linkedin,linkedin-in,youtube,youtube-square,tiktok,spotify,apple,apple-music,google,threads,snapchat,pinterest,pinterest-p,vimeo,vimeo-v,github,whatsapp,telegram,discord,twitch,soundcloud,podcast' | Split:',' -%}

{%- capture contactRows -%}
{%- assign contactRaw = Item | Attribute:'ContactLinks','RawValue' -%}
{%- if contactRaw != '' and contactRaw != null -%}
    {%- assign contactPairs = contactRaw | Split:'|' -%}
    {%- for pair in contactPairs -%}
        {%- comment -%} Split drops an empty leading entry, so ""^ext 6001"" would collapse
            to a single element and the icon-less row would vanish. A sentinel keeps the
            key slot occupied through the split, then comes straight back off. {%- endcomment -%}
        {%- assign safePair = 'ICONSLOT' | Append:pair -%}
        {%- assign bits = safePair | Split:'^' -%}
        {%- assign bitCount = bits | Size -%}
        {%- assign rawIcon = bits[0] | ReplaceFirst:'ICONSLOT','' | Trim -%}
        {%- assign rowText = '' -%}
        {%- if bitCount > 1 -%}{%- assign rowText = bits[1] | Trim -%}{%- endif -%}
        {%- comment -%} a pair with no text at all is a data entry slip, not a row {%- endcomment -%}
        {%- if rowText != '' -%}
            {%- assign icon = rawIcon | Replace:'fab ','' | Replace:'fas ','' | Replace:'far ','' | Replace:'fa ','' | Replace:'fa-','' | Trim -%}
            {%- assign family = 'FontAwesomeSolid' -%}
            {%- for b in brandNames -%}{%- if b == icon -%}{%- assign family = 'FontAwesomeBrands' -%}{%- endif -%}{%- endfor -%}
            {%- assign tapUri = '' -%}
            {%- if rowText contains '@' and rowText contains '.' -%}
                {%- assign tapUri = 'mailto:' | Append:rowText -%}
            {%- elsif rowText contains '(' or rowText contains '-' -%}
                {%- assign digits = rowText | Remove:'(' | Remove:')' | Remove:'-' | Remove:' ' | Remove:'.' -%}
                {%- assign digitCount = digits | Size -%}
                {%- if digitCount >= 7 -%}{%- assign tapUri = 'tel:' | Append:digits -%}{%- endif -%}
            {%- endif -%}
            <Grid ColumnDefinitions=""36, *"" ColumnSpacing=""12"" Padding=""0,14"">
                {%- if icon != '' -%}
                <Rock:Icon Grid.Column=""0"" IconClass=""{{ icon | Escape }}"" IconFamily=""{{ family }}""
                    FontSize=""20"" StyleClass=""text-interface-stronger""
                    VerticalOptions=""Center"" HorizontalOptions=""Start"" />
                {%- endif -%}
                <Label Grid.Column=""1"" Text=""{{ rowText | Escape }}""
                    StyleClass=""body, text-interface-stronger""
                    VerticalOptions=""Center"" LineBreakMode=""TailTruncation"" />
                {%- if tapUri != '' -%}
                <Grid.GestureRecognizers>
                    <TapGestureRecognizer Command=""{Binding OpenBrowser}"" CommandParameter=""{{ tapUri | Escape }}"" />
                </Grid.GestureRecognizers>
                {%- endif -%}
            </Grid>
            <BoxView HeightRequest=""1"" StyleClass=""bg-interface-softer"" />
        {%- endif -%}
    {%- endfor -%}
{%- endif -%}
{%- endcapture -%}

{%- capture socialRows -%}
{%- assign socialRaw = Item | Attribute:'SocialMedia','RawValue' -%}
{%- if socialRaw != '' and socialRaw != null -%}
    {%- assign socialPairs = socialRaw | Split:'|' -%}
    {%- for pair in socialPairs -%}
        {%- comment -%} Split drops an empty leading entry, so ""^ext 6001"" would collapse
            to a single element and the icon-less row would vanish. A sentinel keeps the
            key slot occupied through the split, then comes straight back off. {%- endcomment -%}
        {%- assign safePair = 'ICONSLOT' | Append:pair -%}
        {%- assign bits = safePair | Split:'^' -%}
        {%- assign bitCount = bits | Size -%}
        {%- assign rawIcon = bits[0] | ReplaceFirst:'ICONSLOT','' | Trim -%}
        {%- assign link = '' -%}
        {%- if bitCount > 1 -%}{%- assign link = bits[1] | Trim -%}{%- endif -%}
        {%- if link != '' -%}
            {%- assign icon = rawIcon | Replace:'fab ','' | Replace:'fas ','' | Replace:'far ','' | Replace:'fa ','' | Replace:'fa-','' | Trim -%}
            {%- assign family = 'FontAwesomeSolid' -%}
            {%- for b in brandNames -%}{%- if b == icon -%}{%- assign family = 'FontAwesomeBrands' -%}{%- endif -%}{%- endfor -%}
            {%- assign href = link -%}
            {%- unless href contains 'http' -%}{%- assign href = 'https://' | Append:href -%}{%- endunless -%}
            {%- assign shown = link | Remove:'https://' | Remove:'http://' -%}
            <Grid ColumnDefinitions=""36, *"" ColumnSpacing=""12"" Padding=""0,14"">
                {%- if icon != '' -%}
                <Rock:Icon Grid.Column=""0"" IconClass=""{{ icon | Escape }}"" IconFamily=""{{ family }}""
                    FontSize=""20"" StyleClass=""text-interface-stronger""
                    VerticalOptions=""Center"" HorizontalOptions=""Start"" />
                {%- endif -%}
                <Label Grid.Column=""1"" Text=""{{ shown | Escape }}""
                    StyleClass=""body, text-interface-stronger""
                    VerticalOptions=""Center"" LineBreakMode=""TailTruncation"" />
                <Grid.GestureRecognizers>
                    <TapGestureRecognizer Command=""{Binding OpenBrowser}"" CommandParameter=""{{ href | Escape }}"" />
                </Grid.GestureRecognizers>
            </Grid>
            <BoxView HeightRequest=""1"" StyleClass=""bg-interface-softer"" />
        {%- endif -%}
    {%- endfor -%}
{%- endif -%}
{%- endcapture -%}

<VerticalStackLayout Spacing=""0"">

    <!-- ===== Custom nav bar (page's HideNavigationBar must be ON) ===== -->
    <VerticalStackLayout StyleClass=""bg-interface-softest"" Spacing=""0"">
        <VerticalStackLayout.Behaviors>
            <Rock:SafeAreaPaddingBehavior Edges=""Top"" />
        </VerticalStackLayout.Behaviors>

        <Grid ColumnDefinitions=""56, *"" ColumnSpacing=""0"" Padding=""16,16"">
            <Rock:Icon Grid.Column=""0""
                IconClass=""arrow-left""
                IconFamily=""MaterialDesignIcons""
                FontSize=""24""
                StyleClass=""text-interface-strongest""
                VerticalOptions=""Center""
                VerticalTextAlignment=""Center""
                HorizontalOptions=""Start"">
                <Rock:Icon.GestureRecognizers>
                    <TapGestureRecognizer Command=""{Binding PopPage}"" />
                </Rock:Icon.GestureRecognizers>
            </Rock:Icon>

            <Label Grid.Column=""1""
                StyleClass=""title3, font-weight-semi-bold, text-interface-strongest""
                Text=""{{ displayName | Escape }}""
                VerticalOptions=""Center""
                VerticalTextAlignment=""Center""
                LineBreakMode=""TailTruncation"" />
        </Grid>
    </VerticalStackLayout>

    <!-- ===== Page content ===== -->
    <VerticalStackLayout Spacing=""0"" StyleClass=""p-24"">

        {% if photo != '' %}
            <Rock:StyledBorder CornerRadius=""80"" Padding=""0""
                HeightRequest=""160"" WidthRequest=""160""
                StyleClass=""bg-interface-softer""
                HorizontalOptions=""Center"">
                <Rock:Image Source=""{{ photo | Escape }}""
                    Aspect=""AspectFill""
                    HeightRequest=""160"" WidthRequest=""160"" />
            </Rock:StyledBorder>
        {% endif %}

        <Label StyleClass=""title1, bold, text-interface-strongest""
            Text=""{{ displayName | Escape }}""
            HorizontalTextAlignment=""Center""
            Margin=""0,20,0,0"" />

        {% if role != '' %}
            <Label StyleClass=""callout, font-weight-semi-bold""
                TextColor=""{Rock:PaletteColor App-Primary-Strong}""
                Text=""{{ role | Upcase | Escape }}""
                HorizontalTextAlignment=""Center""
                Margin=""0,6,0,0"" />
        {% endif %}

        {% if subtitle != '' %}
            <Label StyleClass=""footnote, text-interface-medium""
                Text=""{{ subtitle | Escape }}""
                HorizontalTextAlignment=""Center""
                Margin=""0,4,0,0"" />
        {% endif %}

        <BoxView HeightRequest=""1"" StyleClass=""bg-interface-softer"" Margin=""0,24,0,20"" />

        <Rock:Html FollowHyperlinks=""true"" StyleClass=""body"">
        <![CDATA[
        {{ Item.Content }}
        ]]>
        </Rock:Html>
        
        {%- assign contactTrimmed = contactRows | Trim -%}
        {%- if contactTrimmed != '' -%}
            <BoxView HeightRequest=""1"" StyleClass=""bg-interface-softer"" Margin=""0,20,0,0"" />
            {{ contactRows }}
        {%- endif -%}

        {%- assign socialTrimmed = socialRows | Trim -%}
        {%- if socialTrimmed != '' -%}
            {%- if contactTrimmed == '' -%}
            <BoxView HeightRequest=""1"" StyleClass=""bg-interface-softer"" Margin=""0,20,0,0"" />
            {%- endif -%}
            {{ socialRows }}
        {%- endif -%}

        {%- comment -%}
            Everything else the item carries, rendered automatically.

            Skips the attributes already laid out above, plus the ones that are
            plumbing rather than content (LinkedPerson is an entity reference,
            AllowEmail / AllowPhone are visibility flags). Those three are kept in
            the dev database but are excluded from the migration, so they will not
            exist on a migrated server - the skip list covers both cases.

            ValueFormatted is used rather than Value so defined values, booleans
            and the like render readably instead of as raw Guids.
        {%- endcomment -%}
        {%- assign skipKeys = 'Image,Role,DisplayName,Subtitle,LinkedPerson,AllowEmail,AllowPhone,SocialMedia,ContactLinks,Team' | Split:',' -%}
        {%- assign extraCount = 0 -%}
        {%- capture extraRows -%}
            {%- for av in Item.AttributeValues -%}
                {%- assign shown = false -%}
                {%- for k in skipKeys -%}
                    {%- if k == av.AttributeKey -%}{%- assign shown = true -%}{%- endif -%}
                {%- endfor -%}
                {%- assign val = av.ValueFormatted -%}
                {%- unless shown or val == '' or val == null -%}
                    {%- assign extraCount = extraCount | Plus:1 -%}
                    <Grid ColumnDefinitions=""Auto, *"" ColumnSpacing=""16"" Margin=""0,0,0,14"">
                        <Label Grid.Column=""0"" StyleClass=""footnote, font-weight-semi-bold, text-interface-medium""
                            Text=""{{ av.AttributeName | Escape }}"" VerticalOptions=""Center"" />
                        <Label Grid.Column=""1"" StyleClass=""body, text-interface-stronger""
                            Text=""{{ val | StripHtml | Trim | Escape }}""
                            HorizontalTextAlignment=""End"" VerticalOptions=""Center"" />
                    </Grid>
                {%- endunless -%}
            {%- endfor -%}
        {%- endcapture -%}

        {%- if extraCount > 0 -%}
            <BoxView HeightRequest=""1"" StyleClass=""bg-interface-softer"" Margin=""0,20,0,16"" />
            {{ extraRows }}
        {%- endif -%}

    </VerticalStackLayout>

</VerticalStackLayout>
" );   // ContentTemplate
            RockMigrationHelper.AddBlockAttributeValue( "00AF4314-F34E-4AEF-ADF6-462182AF8D89", "49913217-BF13-4270-8023-C56BDA52C790", @"68a8458f-b632-46ba-a1f4-cb65c62df18f" );   // ContentChannel
            RockMigrationHelper.AddBlockAttributeValue( "00AF4314-F34E-4AEF-ADF6-462182AF8D89", "616351D9-41FD-4E84-9378-78140BE30605", @"True" );   // LogInteractions
            RockMigrationHelper.AddBlockAttributeValue( "82791881-D9E4-48E1-9844-A84CFDC78955", "74FAF302-CE8C-4DF8-8DD2-85755A63B04B", @"<VerticalStackLayout Spacing=""24""
    VerticalOptions=""Center""
    HorizontalOptions=""Center"">
    
    <ActivityIndicator IsVisible=""true"" IsRunning=""true"" />
        
    <Label Text=""Hang on while we fetch your details!""
        StyleClass=""text-interface-strongest, bold, subheadline"" />
        
</VerticalStackLayout>" );   // LoadingScreenTemplate
            RockMigrationHelper.AddBlockAttributeValue( "82791881-D9E4-48E1-9844-A84CFDC78955", "DC375A74-280B-4BB8-8F8D-49A51A101152", @"{% assign hasNewAchievements = false %}

{% for attendance in RecordedAttendances %}
    {% assign newAchievementSize = attendance.JustCompletedAchievements | Size %}
    {% if newAchievementSize > 0 %}
        {% assign hasNewAchievements = true %}
    {% endif %}
{% endfor %}

<Grid>
    <StackLayout Spacing=""24"">
    
        //- Header Row
        <VerticalStackLayout>
            <Label Text=""Check-In Complete""
                StyleClass=""title1, bold, text-interface-strongest"" />
                
            <Label Text=""Below are the details of your check-in""
                StyleClass=""footnote, text-interface-strong"" />
        </VerticalStackLayout>
        
        //- Achievement Bar
        {% if hasNewAchievements %}
            <VerticalStackLayout>
                {% for attendance in RecordedAttendances.JustCompletedAchievements %}
                    <Grid ColumnDefinitions=""Auto, *"">
                        <Label Text=""test"" />
                    </Grid>        
                {% endfor %}
            </VerticalStackLayout>
        {% endif %}
    
        //- Attendance Details
        <VerticalStackLayout Spacing=""24"">
            {% for savedAttendance in RecordedAttendances %}
                <Rock:StyledBorder StyleClass=""p-16, bg-interface-softest, border, border-interface-soft, rounded"">
                    <VerticalStackLayout Spacing=""8"">
    
                        //- Avatar and person name
                        <HorizontalStackLayout Spacing=""16""
                            HorizontalOptions=""Center"">
                            <Rock:Avatar Source=""{{ 'Global' | Attribute:'PublicApplicationRoot' }}{{ savedAttendance.Attendance.Person.PhotoUrl | Escape }}"" 
                                HeightRequest=""32""
                                WidthRequest=""32"" />
        
                            <Label Text=""{{ savedAttendance.Attendance.Person.FullName | Escape }}""
                                StyleClass=""title3, bold, text-interface-stronger""
                                VerticalOptions=""Center"" />
                        </HorizontalStackLayout>
    
                        //- Checked into group
                        <Grid RowSpacing=""4""
                            RowDefinitions=""Auto, Auto"">
                            <Label Text=""Checked into""
                                StyleClass=""footnote, text-interface-strong"" />
                            
                            <Rock:StyledBorder StyleClass=""bg-primary-strong, px-8, py-4, rounded""
                                Grid.Row=""1"">
                                <Grid ColumnDefinitions=""*, Auto""
                                    VerticalOptions=""Center"">
                                    <Label Text=""{{ savedAttendance.Attendance.Location.Name | Escape }}""
                                        StyleClass=""body, bold, text-primary-soft"" />
                                    
                                    <Label Text=""{{ savedAttendance.Attendance.Schedule.Name | Escape }}""
                                        Grid.Column=""1""
                                        StyleClass=""body, text-primary-soft""/>
                                </Grid>
                            </Rock:StyledBorder>
                        </Grid>
                    </VerticalStackLayout>
                </Rock:StyledBorder>
            {% endfor %} 
        </VerticalStackLayout>
    </StackLayout>
    
    <Rock:ConfettiView IsAnimationEnabled=""True"" InputTransparent=""true"" />
</Grid>" );   // CompletionScreenTemplate
            RockMigrationHelper.AddBlockAttributeValue( "82791881-D9E4-48E1-9844-A84CFDC78955", "E018C9A6-96BD-4670-B10C-E06CD552D089", @"False" );   // AllowAddFamilyMember
            RockMigrationHelper.AddBlockAttributeValue( "82791881-D9E4-48E1-9844-A84CFDC78955", "5BB516A9-2512-479D-9979-50DFAA705960", @"q8Lln9mXyM" );   // ConfigurationTemplate
            RockMigrationHelper.AddBlockAttributeValue( "82791881-D9E4-48E1-9844-A84CFDC78955", "6F90A16E-BB57-47D0-9230-07B633CAA65F", @"xbKmXMlo7J,yKQmK0l8Gp,RGKBqjmW8b,pAZB8dBngD,d1VPVOB2MZ" );   // Areas
            RockMigrationHelper.AddBlockAttributeValue( "82791881-D9E4-48E1-9844-A84CFDC78955", "EBA83553-0D5C-487A-9CF5-48E88EA67F8B", @"OX9mQWPQo8" );   // Kiosk
            RockMigrationHelper.AddBlockAttributeValue( "E430DB8D-53B7-432A-BF3D-28D590295FE1", "5682EDBF-68DA-4B43-A593-6C2B936C2839", @"{%- assign appRoot = 'Global' | Attribute:'PublicApplicationRoot' -%}
{%- assign seriesChannelId = 4 -%}
{%- assign messageChannelId = 5 -%}
{%- assign sermonPageGuid = '4079b24c-d548-4cd0-a833-c5688bbef052' -%}
{%- assign seriesPageGuid = '6d3762e4-0689-42fe-8535-a7b89c4fc028' -%}
{%- assign appRoot = 'Global' | Attribute:'PublicApplicationRoot' -%}
{%- assign recentCount = 10 -%}
{%- assign seriesRowCount = 5 -%}

<VerticalStackLayout Spacing=""0"">

    {% contentchannelitem where:'ContentChannelId == ""{{ seriesChannelId }}""' sort:'StartDateTime desc' limit:'1' %}
        {% for hero in contentchannelitemItems %}
            {%- comment -%} hg ends up holding an absolute image URL, not a guid: SeriesImage (Image field, a guid) wrapped in GetImage, else SeriesImageLink (Text field, already a URL). {%- endcomment -%}
{%- assign hgGuid = hero | Attribute:'SeriesImage','RawValue' -%}
{%- assign hgLink = hero | Attribute:'SeriesImageLink','RawValue' -%}
{%- assign hg = '' -%}
{%- if hgGuid != '' and hgGuid != null -%}{%- assign hg = hgGuid -%}
{%- elsif hgLink != '' and hgLink != null -%}{%- assign hg = hgLink -%}{%- endif -%}{%- if hg != '' and hg != null -%}{%- unless hg contains 'http' -%}{%- assign hg = appRoot | Append:'GetImage.ashx?Guid=' | Append:hg -%}{%- endunless -%}{%- endif -%}
            {% if hg != '' %}
                <Rock:StyledBorder CornerRadius=""12"" Padding=""0"" StyleClass=""mx-16, mt-16"">
                    <Rock:Image Source=""{{ hg | Escape }}"" Aspect=""AspectFill"" Ratio=""16:9"" />
                    <Rock:StyledBorder.GestureRecognizers>
                        <TapGestureRecognizer Command=""{Binding PushPage}""
                            CommandParameter=""{{ seriesPageGuid }}?ContentChannelItemId={{ hero.Id }}"" />
                    </Rock:StyledBorder.GestureRecognizers>
                </Rock:StyledBorder>
            {% endif %}
        {% endfor %}
    {% endcontentchannelitem %}

    <Grid ColumnDefinitions=""*, Auto"" StyleClass=""mx-16, mt-24"">
        <Label Grid.Column=""0"" StyleClass=""title2, bold, text-interface-strongest"" Text=""Recent Sermons"" VerticalOptions=""Center"" />
    </Grid>

    <ScrollView Orientation=""Horizontal"" HorizontalScrollBarVisibility=""Never"" StyleClass=""mt-12"">
        <HorizontalStackLayout Spacing=""12"" Padding=""16,0"">
            {% contentchannelitem where:'ContentChannelId == ""{{ messageChannelId }}""' sort:'StartDateTime desc' limit:'{{ recentCount }}' %}
                {% for msg in contentchannelitemItems %}
{%- comment -%} thumbnail: Media File thumb -> Image -> Series Image {%- endcomment -%}
                    {%- assign img = msg | Attribute:'MediaFile','DefaultThumbnailUrl' -%}
                    {%- if img == '' or img == null -%}{%- assign img = msg | Attribute:'Image','RawValue' -%}{%- if img != '' and img != null -%}{%- unless img contains 'http' -%}{%- assign img = appRoot | Append:'GetImage.ashx?Guid=' | Append:img -%}{%- endunless -%}{%- endif -%}{%- endif -%}
                    {%- if img == '' -%}
                        {%- for p in msg.ParentItems limit:1 -%}
                            {%- comment -%} sg ends up holding an absolute image URL, not a guid: SeriesImage (Image field, a guid) wrapped in GetImage, else SeriesImageLink (Text field, already a URL). {%- endcomment -%}
{%- assign sgGuid = p.ContentChannelItem | Attribute:'SeriesImage','RawValue' -%}
{%- assign sgLink = p.ContentChannelItem | Attribute:'SeriesImageLink','RawValue' -%}
{%- assign sg = '' -%}
{%- if sgGuid != '' and sgGuid != null -%}{%- assign sg = sgGuid -%}
{%- elsif sgLink != '' and sgLink != null -%}{%- assign sg = sgLink -%}{%- endif -%}{%- if sg != '' and sg != null -%}{%- unless sg contains 'http' -%}{%- assign sg = appRoot | Append:'GetImage.ashx?Guid=' | Append:sg -%}{%- endunless -%}{%- endif -%}
                            {%- if sg != '' -%}{%- assign img = sg -%}{%- endif -%}
                        {%- endfor -%}
                    {%- endif -%}
                    <VerticalStackLayout WidthRequest=""240"" Spacing=""8"">
                        <Rock:StyledBorder CornerRadius=""10"" Padding=""0"" WidthRequest=""240"" HeightRequest=""135"" StyleClass=""bg-interface-softer"">{% if img != '' %}<Rock:Image Source=""{{ img | Escape }}"" Aspect=""AspectFill"" WidthRequest=""240"" HeightRequest=""135"" />{% else %}<Rock:Icon IconClass=""video"" IconFamily=""FontAwesomeSolid"" FontSize=""28"" StyleClass=""text-interface-soft"" HorizontalOptions=""Center"" VerticalOptions=""Center"" />{% endif %}</Rock:StyledBorder>
                        <Label StyleClass=""body, bold, text-interface-strongest"" Text=""{{ msg.Title | Escape }}"" MaxLines=""2"" LineBreakMode=""TailTruncation"" />
                        <VerticalStackLayout.GestureRecognizers>
                            <TapGestureRecognizer Command=""{Binding PushPage}"" CommandParameter=""{{ sermonPageGuid }}?ContentChannelItemId={{ msg.Id }}"" />
                        </VerticalStackLayout.GestureRecognizers>
                    </VerticalStackLayout>
                {% endfor %}
            {% endcontentchannelitem %}
        </HorizontalStackLayout>
    </ScrollView>

    {% contentchannelitem where:'ContentChannelId == ""{{ seriesChannelId }}""' sort:'StartDateTime desc' limit:'{{ seriesRowCount }}' %}
        {% for series in contentchannelitemItems %}
            {%- assign childCount = series.ChildItems | Size -%}
            {%- comment -%} seriesGuid ends up holding an absolute image URL, not a guid: SeriesImage (Image field, a guid) wrapped in GetImage, else SeriesImageLink (Text field, already a URL). {%- endcomment -%}
{%- assign seriesGuidGuid = series | Attribute:'SeriesImage','RawValue' -%}
{%- assign seriesGuidLink = series | Attribute:'SeriesImageLink','RawValue' -%}
{%- assign seriesGuid = '' -%}
{%- if seriesGuidGuid != '' and seriesGuidGuid != null -%}{%- assign seriesGuid = seriesGuidGuid -%}
{%- elsif seriesGuidLink != '' and seriesGuidLink != null -%}{%- assign seriesGuid = seriesGuidLink -%}{%- endif -%}{%- if seriesGuid != '' and seriesGuid != null -%}{%- unless seriesGuid contains 'http' -%}{%- assign seriesGuid = appRoot | Append:'GetImage.ashx?Guid=' | Append:seriesGuid -%}{%- endunless -%}{%- endif -%}
            {% if childCount > 0 %}
                <Grid ColumnDefinitions=""*, Auto"" StyleClass=""mx-16, mt-24"">
                    <Label Grid.Column=""0"" StyleClass=""title2, bold, text-interface-strongest"" Text=""{{ series.Title | Escape }}"" MaxLines=""2"" LineBreakMode=""TailTruncation"" VerticalOptions=""Center"" />
                    <Label Grid.Column=""1"" StyleClass=""body, text-interface-medium"" Text=""View All"" VerticalOptions=""Center"" />
                    <Grid.GestureRecognizers>
                        <TapGestureRecognizer Command=""{Binding PushPage}"" CommandParameter=""{{ seriesPageGuid }}?ContentChannelItemId={{ series.Id }}"" />
                    </Grid.GestureRecognizers>
                </Grid>

                <ScrollView Orientation=""Horizontal"" HorizontalScrollBarVisibility=""Never"" StyleClass=""mt-12"">
                    <HorizontalStackLayout Spacing=""12"" Padding=""16,0"">
                        {% for assoc in series.ChildItems %}
                            {%- assign msg = assoc.ChildContentChannelItem -%}
                            {%- assign img = msg | Attribute:'Image','RawValue' -%}{%- if img != '' and img != null -%}{%- unless img contains 'http' -%}{%- assign img = appRoot | Append:'GetImage.ashx?Guid=' | Append:img -%}{%- endunless -%}{%- endif -%}
                            {%- if img == '' and seriesGuid != '' -%}{%- assign img = seriesGuid -%}{%- endif -%}
                            <VerticalStackLayout WidthRequest=""240"" Spacing=""8"">
                                <Rock:StyledBorder CornerRadius=""10"" Padding=""0"" WidthRequest=""240"" HeightRequest=""135"" StyleClass=""bg-interface-softer"">{% if img != '' %}<Rock:Image Source=""{{ img | Escape }}"" Aspect=""AspectFill"" WidthRequest=""240"" HeightRequest=""135"" />{% else %}<Rock:Icon IconClass=""video"" IconFamily=""FontAwesomeSolid"" FontSize=""28"" StyleClass=""text-interface-soft"" HorizontalOptions=""Center"" VerticalOptions=""Center"" />{% endif %}</Rock:StyledBorder>
                                <Label StyleClass=""body, bold, text-interface-strongest"" Text=""{{ msg.Title | Escape }}"" MaxLines=""2"" LineBreakMode=""TailTruncation"" />
                                <VerticalStackLayout.GestureRecognizers>
                                    <TapGestureRecognizer Command=""{Binding PushPage}"" CommandParameter=""{{ sermonPageGuid }}?ContentChannelItemId={{ msg.Id }}"" />
                                </VerticalStackLayout.GestureRecognizers>
                            </VerticalStackLayout>
                        {% endfor %}
                    </HorizontalStackLayout>
                </ScrollView>
            {% endif %}
        {% endfor %}
    {% endcontentchannelitem %}

    <BoxView HeightRequest=""24"" Color=""Transparent"" />
</VerticalStackLayout>" );   // Content
            RockMigrationHelper.AddBlockAttributeValue( "E430DB8D-53B7-432A-BF3D-28D590295FE1", "24516448-3F1F-4F27-97A1-CFB4F8B277B5", @"RockEntity" );   // EnabledLavaCommands
            RockMigrationHelper.AddBlockAttributeValue( "E430DB8D-53B7-432A-BF3D-28D590295FE1", "B31D29A0-3725-4AEB-8360-7D91B9CDFE47", @"True" );   // DynamicContent
            RockMigrationHelper.AddBlockAttributeValue( "C63AD407-B2D3-4DA5-BC49-B34DF3554EE1", "D80CF7C7-F6F4-4E77-97A8-B0842E4AF7FB", @"{%- assign appRoot = 'Global' | Attribute:'PublicApplicationRoot' -%}
{%- assign publicPagesReady = true -%}
{% comment %} flip to true once /series/{Id} exists — see PRODUCTION-SETUP-CHECKLIST.md {% endcomment %}
{%- comment -%} bannerGuid ends up holding an absolute image URL, not a guid: SeriesImage (Image field, a guid) wrapped in GetImage, else SeriesImageLink (Text field, already a URL). {%- endcomment -%}
{%- assign bannerGuidGuid = Item | Attribute:'SeriesImage','RawValue' -%}
{%- assign bannerGuidLink = Item | Attribute:'SeriesImageLink','RawValue' -%}
{%- assign bannerGuid = '' -%}
{%- if bannerGuidGuid != '' and bannerGuidGuid != null -%}{%- assign bannerGuid = bannerGuidGuid -%}
{%- elsif bannerGuidLink != '' and bannerGuidLink != null -%}{%- assign bannerGuid = bannerGuidLink -%}{%- endif -%}{%- if bannerGuid != '' and bannerGuid != null -%}{%- unless bannerGuid contains 'http' -%}{%- assign bannerGuid = appRoot | Append:'GetImage.ashx?Guid=' | Append:bannerGuid -%}{%- endunless -%}{%- endif -%}
{%- assign appRoot = 'Global' | Attribute:'PublicApplicationRoot' -%}
{%- assign summary = Item | Attribute:'Summary' -%}
{%- assign sermonPageGuid = '4079b24c-d548-4cd0-a833-c5688bbef052' -%}
{%- assign episodes = Item.ChildItems | Select:'ChildContentChannelItem' | Sort:'StartDateTime' -%}
{%- if publicPagesReady -%}
    {%- capture shareUri -%}{{ appRoot }}series/{{ Item.Id }}{%- endcapture -%}
{%- else -%}
    {%- assign shareUri = '' -%}
{%- endif -%}

<VerticalStackLayout Spacing=""0"">
    {% if bannerGuid != '' %}
        <Rock:StyledBorder CornerRadius=""12"" Padding=""0"" StyleClass=""mx-16, mt-16"">
            <Rock:Image Source=""{{ bannerGuid | Escape }}"" Aspect=""AspectFill"" Ratio=""16:9"" />
        </Rock:StyledBorder>
    {% endif %}
    <VerticalStackLayout Spacing=""8"" StyleClass=""p-16"">
        <Label StyleClass=""title1, bold, text-interface-strongest"" Text=""{{ Item.Title | Escape }}"" />
        {% if summary != '' %}
            <Rock:Html Text=""{{ summary | Escape }}"" StyleClass=""body, text-interface-strong"" />
        {% endif %}
    </VerticalStackLayout>
    <Grid ColumnDefinitions=""*, *"" ColumnSpacing=""8"" StyleClass=""px-16, pb-8"">

        <VerticalStackLayout Grid.Column=""0"" Spacing=""6"" HorizontalOptions=""Center"">
            <Rock:FollowingIcon
                EntityTypeId=""{{ Item.TypeId }}""
                EntityId=""{{ Item.Id }}""
                IsFollowed=""{{ Item | IsFollowed }}""
                FontSize=""22""
                HorizontalOptions=""Center""
                FollowingIconClass=""heart""
                FollowingIconFamily=""FontAwesomeSolid""
                FollowingIconColor=""{Rock:PaletteColor App-Primary-Strong}""
                NotFollowingIconClass=""heart""
                NotFollowingIconFamily=""FontAwesomeRegular""
                NotFollowingIconColor=""{AppThemeBinding Light=#3F3F46, Dark=#E4E4E7}""
                NotLoggedInText=""Sign in to save this series to your list."" />
            <Label StyleClass=""caption1, font-weight-semi-bold, text-interface-medium""
                Text=""SAVE"" HorizontalOptions=""Center"" />
        </VerticalStackLayout>

        <VerticalStackLayout Grid.Column=""1"" Spacing=""6"" HorizontalOptions=""Center"">
            <Rock:Icon IconClass=""share-square"" IconFamily=""FontAwesomeSolid"" FontSize=""22""
                StyleClass=""text-interface-stronger"" HorizontalOptions=""Center"" />
            <Label StyleClass=""caption1, font-weight-semi-bold, text-interface-medium""
                Text=""SHARE"" HorizontalOptions=""Center"" />
            <VerticalStackLayout.GestureRecognizers>
                <TapGestureRecognizer Command=""{Binding ShareContent}"">
                    <TapGestureRecognizer.CommandParameter>
                        <Rock:ShareContentParameters
                            Title=""{{ Item.Title | Escape }}""
                            Text=""Check out the {{ Item.Title | Escape }} series from Nfluence Church.""
                            Uri=""{{ shareUri | Escape }}"" />
                    </TapGestureRecognizer.CommandParameter>
                </TapGestureRecognizer>
            </VerticalStackLayout.GestureRecognizers>
        </VerticalStackLayout>
    </Grid>
    <VerticalStackLayout Spacing=""0"" StyleClass=""px-16"">
        {% for msg in episodes %}
            {%- assign speaker = msg | Attribute:'Speaker' -%}
            <Rock:StyledBorder CornerRadius=""12"" Padding=""16,14"" StyleClass=""bg-interface-softest, my-4"">
                <Rock:StyledBorder.GestureRecognizers>
                    <TapGestureRecognizer Command=""{Binding PushPage}"" CommandParameter=""{{ sermonPageGuid }}?ContentChannelItemId={{ msg.Id }}"" />
                </Rock:StyledBorder.GestureRecognizers>
                <Grid ColumnDefinitions=""*, Auto"" ColumnSpacing=""12"">
                    <VerticalStackLayout Grid.Column=""0"" Spacing=""3"" VerticalOptions=""Center"">
                        <Label StyleClass=""body, bold, text-interface-strongest"" Text=""{{ msg.Title | Escape }}"" MaxLines=""2"" LineBreakMode=""TailTruncation"" />
                        <Label StyleClass=""caption1, text-interface-medium"" Text=""{{ msg.StartDateTime | Date:'MMMM d, yyyy' | Upcase }}{% if speaker != '' %} - {{ speaker | Upcase | Escape }}{% endif %}"" LineBreakMode=""TailTruncation"" />
                    </VerticalStackLayout>
                    <Rock:Icon Grid.Column=""1"" IconClass=""chevron-right"" IconFamily=""FontAwesomeSolid"" FontSize=""16"" StyleClass=""text-interface-soft"" VerticalOptions=""Center"" />
                </Grid>
            </Rock:StyledBorder>
        {% endfor %}
    </VerticalStackLayout>
    <BoxView HeightRequest=""24"" Color=""Transparent"" />
</VerticalStackLayout>" );   // ContentTemplate
            RockMigrationHelper.AddBlockAttributeValue( "C63AD407-B2D3-4DA5-BC49-B34DF3554EE1", "49913217-BF13-4270-8023-C56BDA52C790", @"e2c598f1-d299-1baa-4873-8b679e3c1998" );   // ContentChannel
            RockMigrationHelper.AddBlockAttributeValue( "C63AD407-B2D3-4DA5-BC49-B34DF3554EE1", "616351D9-41FD-4E84-9378-78140BE30605", @"False" );   // LogInteractions
            RockMigrationHelper.AddBlockAttributeValue( "7B49DE66-B707-417F-8A65-A082508A548B", "21CC1DAD-87A6-4F00-A324-281E6D7190D0", @"f04b6154-1543-4632-89a2-1792f6ced9d6|" );   // ToolboxTemplate
            RockMigrationHelper.AddBlockAttributeValue( "7B49DE66-B707-417F-8A65-A082508A548B", "3755EF28-B0FC-4039-8012-6EDDA4E10FFF", @"de3e57ac-e12b-4249-bb15-64c7a7780ac8|" );   // ConfirmDeclineTemplate
            RockMigrationHelper.AddBlockAttributeValue( "A67C5D2C-1161-4EBA-8F92-93962EF739F6", "D80CF7C7-F6F4-4E77-97A8-B0842E4AF7FB", @"{%- assign appRoot = 'Global' | Attribute:'PublicApplicationRoot' -%}
{%- comment -%}
    Sermon Detail 2.0 - Resi media via the ""MediaFile"" Media Element attribute.
    AppendWatches supplies the media urls AND the person's prior watch map (resume).
    Fallback order: MediaFile (Resi) -> legacy VideoEmbed/VideoLink web player -> item Image.
{%- endcomment -%}
{%- comment -%} AppendWatches is documented against a COLLECTION, so run it over a one-item set {%- endcomment -%}
{%- assign w = null -%}
{% contentchannelitem where:'Id == {{ Item.Id }}' securityenabled:'false' %}
    {%- assign w = contentchannelitemItems | AppendWatches:'MediaFile',365 | First -%}
{% endcontentchannelitem %}
{%- assign mediaUrl = w.MediaDefaultFileUrl -%}
{%- assign mediaThumb = w.MediaDefaultThumbnailUrl -%}
{%- assign mediaGuid = w.MediaGuid -%}
{%- assign watchMap = w.WatchMap -%}
{%- assign watchInteraction = w.WatchInteractionGuid -%}
{%- assign resumeSecs = w.ResumeLocationInSeconds -%}

{%- assign mode = PageParameter.Mode -%}
{%- assign speaker = Item | Attribute:'Speaker' -%}
{%- assign audio = Item | Attribute:'AudioLink','RawValue' -%}
{%- assign img = Item | Attribute:'Image','RawValue' -%}{%- if img != '' and img != null -%}{%- unless img contains 'http' -%}{%- assign img = appRoot | Append:'GetImage.ashx?Guid=' | Append:img -%}{%- endunless -%}{%- endif -%}
{%- assign appRoot = 'Global' | Attribute:'PublicApplicationRoot' -%}
{%- assign sermonPageGuid = '4079b24c-d548-4cd0-a833-c5688bbef052' -%}
{%- if mediaThumb == '' or mediaThumb == null -%}{%- assign mediaThumb = img -%}{%- endif -%}
{%- assign videoLink = Item | Attribute:'VideoLink','RawValue' -%}
{%- assign videoEmbed = Item | Attribute:'VideoEmbed','RawValue' -%}
{%- comment -%} ===== what media does this item actually have? =====
     MediaFile is the Resi element, VideoLink/VideoEmbed the legacy web player.
     Any one of them counts as ""watchable"". {%- endcomment -%}
{%- comment -%} Render the candidates into a string and test THAT. Comparing a
     possibly-null object property with != '' and != null is unreliable - w is
     null when the item has no Resi media, and w.MediaGuid then satisfied the
     test, which put a WATCH button on messages with no video at all. {%- endcomment -%}
{%- capture videoProbe -%}{{ mediaGuid }}{{ mediaUrl }}{{ videoLink }}{{ videoEmbed }}{%- endcapture -%}
{%- capture audioProbe -%}{{ audio }}{%- endcapture -%}
{%- comment -%} w.MediaGuid renders as the EMPTY guid, not blank, when an item has
     no Resi media - so it has to be stripped or every item looks watchable {%- endcomment -%}
{%- assign videoProbe = videoProbe | Remove:'00000000-0000-0000-0000-000000000000' | Trim -%}
{%- assign audioProbe = audioProbe | Trim -%}
{%- assign hasVideo = false -%}{%- if videoProbe != '' -%}{%- assign hasVideo = true -%}{%- endif -%}
{%- assign hasAudio = false -%}{%- if audioProbe != '' -%}{%- assign hasAudio = true -%}{%- endif -%}
{%- comment -%} playMode, not the raw Mode parameter. An audio-only message has no
     video branch to fall into, so without this the page renders with NO player at
     all until someone taps LISTEN. Force it into audio mode instead.
     The toggle then only appears when the item has BOTH - with one medium there
     is nowhere to toggle to. {%- endcomment -%}
{%- assign playMode = mode -%}
{%- unless hasVideo -%}{%- if hasAudio -%}{%- assign playMode = 'audio' -%}{%- endif -%}{%- endunless -%}
{%- assign showToggle = false -%}
{%- if hasAudio -%}{%- if hasVideo -%}{%- assign showToggle = true -%}{%- endif -%}{%- endif -%}

{%- if videoLink != '' -%}
    {%- assign shareUri = videoLink -%}
{%- else -%}
    {%- capture shareUri -%}{{ appRoot }}sermon/{{ Item.Id }}{%- endcapture -%}
{%- endif -%}

{%- assign siblings = '' -%}
{%- assign seriesGuid = '' -%}
{%- for p in Item.ParentItems limit:1 -%}
    {%- assign parentSeries = p.ContentChannelItem -%}
    {%- comment -%} seriesGuid ends up holding an absolute image URL, not a guid: SeriesImage (Image field, a guid) wrapped in GetImage, else SeriesImageLink (Text field, already a URL). {%- endcomment -%}
{%- assign seriesGuidGuid = parentSeries | Attribute:'SeriesImage','RawValue' -%}
{%- assign seriesGuidLink = parentSeries | Attribute:'SeriesImageLink','RawValue' -%}
{%- assign seriesGuid = '' -%}
{%- if seriesGuidGuid != '' and seriesGuidGuid != null -%}{%- assign seriesGuid = seriesGuidGuid -%}
{%- elsif seriesGuidLink != '' and seriesGuidLink != null -%}{%- assign seriesGuid = seriesGuidLink -%}{%- endif -%}{%- if seriesGuid != '' and seriesGuid != null -%}{%- unless seriesGuid contains 'http' -%}{%- assign seriesGuid = appRoot | Append:'GetImage.ashx?Guid=' | Append:seriesGuid -%}{%- endunless -%}{%- endif -%}
    {%- assign siblings = parentSeries.ChildItems | Select:'ChildContentChannelItem' | Sort:'StartDateTime' -%}
{%- endfor -%}
{%- assign siblingCount = siblings | Size -%}

{%- comment -%} player artwork: MediaFile thumb -> item Image -> parent Series Image {%- endcomment -%}
{%- if mediaThumb == '' or mediaThumb == null -%}
    {%- if seriesGuid != '' -%}{%- assign mediaThumb = seriesGuid -%}{%- endif -%}
{%- endif -%}

<VerticalStackLayout Spacing=""0"">

    <!-- ===== Player: audio mode swaps the mp3 in for the video ===== -->
    {% if playMode == 'audio' and audio != '' %}
        <Rock:MediaPlayer x:Name=""episodePlayer""
            Source=""{{ audio | Escape }}""
            Title=""{{ Item.Title | Escape }}""
            Subtitle=""{% if speaker != '' %}{{ speaker | Escape }}{% endif %}""
            ShowThumbnail=""false""
            IsCastEnabled=""true""
            MeasureWithAspectRatio=""false""
            HeightRequest=""260"">
            <Rock:MediaPlayer.OverlayContent>
                <Grid InputTransparent=""False"">
                    {% if mediaThumb != '' %}
                    <Rock:Image Source=""{{ mediaThumb | Escape }}"" Aspect=""AspectFill"" HorizontalOptions=""Fill"" VerticalOptions=""Fill"" />
                    {% endif %}
    
                    <!-- PLAY: shown unless playing -->
                    <Rock:StyledBorder WidthRequest=""70"" HeightRequest=""70"" CornerRadius=""35"" Padding=""0""
                        HorizontalOptions=""Center"" VerticalOptions=""Center"" InputTransparent=""False""
                        BackgroundColor=""{Rock:PaletteColor App-Primary-Strong}"">
                        <Rock:Icon IconClass=""play"" IconFamily=""FontAwesomeSolid"" FontSize=""28""
                            TextColor=""#FFFFFF"" HorizontalOptions=""Center"" VerticalOptions=""Center"" />
                        <Rock:StyledBorder.GestureRecognizers>
                            <TapGestureRecognizer Command=""{Binding PlayCommand}"" />
                        </Rock:StyledBorder.GestureRecognizers>
                        <Rock:StyledBorder.Triggers>
                            <DataTrigger TargetType=""Rock:StyledBorder""
                                Binding=""{Binding Source={x:Reference episodePlayer}, Path=CurrentState}"" Value=""Playing"">
                                <Setter Property=""IsVisible"" Value=""False"" />
                            </DataTrigger>
                        </Rock:StyledBorder.Triggers>
                    </Rock:StyledBorder>
    
                    <!-- PAUSE: shown only while playing -->
                    <Rock:StyledBorder WidthRequest=""70"" HeightRequest=""70"" CornerRadius=""35"" Padding=""0""
                        HorizontalOptions=""Center"" VerticalOptions=""Center"" IsVisible=""False"" InputTransparent=""False""
                        BackgroundColor=""{Rock:PaletteColor App-Primary-Strong}"">
                        <Rock:Icon IconClass=""pause"" IconFamily=""FontAwesomeSolid"" FontSize=""28""
                            TextColor=""#FFFFFF"" HorizontalOptions=""Center"" VerticalOptions=""Center"" />
                        <Rock:StyledBorder.GestureRecognizers>
                            <TapGestureRecognizer Command=""{Binding PauseCommand}"" />
                        </Rock:StyledBorder.GestureRecognizers>
                        <Rock:StyledBorder.Triggers>
                            <DataTrigger TargetType=""Rock:StyledBorder""
                                Binding=""{Binding Source={x:Reference episodePlayer}, Path=CurrentState}"" Value=""Playing"">
                                <Setter Property=""IsVisible"" Value=""True"" />
                            </DataTrigger>
                        </Rock:StyledBorder.Triggers>
                    </Rock:StyledBorder>
    
                </Grid>
            </Rock:MediaPlayer.OverlayContent>
        </Rock:MediaPlayer>
    {% elsif mediaUrl != '' and mediaUrl != null %}
        <Rock:MediaPlayer Source=""{{ mediaUrl | Escape }}""
            Title=""{{ Item.Title | Escape }}""
            Subtitle=""{% if speaker != '' %}{{ speaker | Escape }}{% endif %}""
            ThumbnailSource=""{{ mediaThumb | Escape }}""
            ShowThumbnail=""true""
            IsCastEnabled=""true""
            AllowsPictureInPicturePlayback=""true"">
            <Rock:MediaPlayer.WatchMap>
                <Rock:WatchMapParameters
                    MediaElementGuid=""{{ mediaGuid }}""
                    InteractionGuid=""{{ watchInteraction }}""
                    WatchMap=""{{ watchMap }}""
                    RelatedEntityTypeId=""{{ Item.TypeId }}""
                    RelatedEntityId=""{{ Item.Id }}"" />
            </Rock:MediaPlayer.WatchMap>
        </Rock:MediaPlayer>
    {% elsif videoLink != '' and videoLink contains '.mp4' %}
        <Rock:MediaPlayer Source=""{{ videoLink | Escape }}""
            Title=""{{ Item.Title | Escape }}""
            Subtitle=""{% if speaker != '' %}{{ speaker | Escape }}{% endif %}""
            ThumbnailSource=""{{ mediaThumb | Escape }}""
            ShowThumbnail=""true""
            IsCastEnabled=""true""
            AllowsPictureInPicturePlayback=""true"" />
    {% elsif videoEmbed != '' %}
        {%- comment -%} legacy YouTube/embed path - served by the web mediaplayer route {%- endcomment -%}
        <Rock:RatioView Ratio=""16:9"" BackgroundColor=""#000000"">
            <Rock:WebView Source=""{{ appRoot }}mediaplayer/{{ Item.Id }}"" />
        </Rock:RatioView>
    {% elsif img != '' %}
        <Rock:Image Source=""{{ img | Escape }}"" Aspect=""AspectFill"" Ratio=""16:9"" />
    {% endif %}

    <!-- ===== Title / meta ===== -->
    <VerticalStackLayout Spacing=""6"" StyleClass=""p-16"">
        <Label StyleClass=""title2, bold, text-interface-strongest"" Text=""{{ Item.Title | Escape }}"" />
        <Label StyleClass=""caption1, font-weight-semi-bold, text-interface-medium""
            Text=""{{ Item.StartDateTime | Date:'MMMM d, yyyy' | Upcase }}{% if speaker != '' %} - {{ speaker | Upcase | Escape }}{% endif %}"" />
        {% if resumeSecs > 0 %}
            <Label StyleClass=""caption1, text-primary-strong""
                Text=""Resuming where you left off"" />
        {% endif %}
    </VerticalStackLayout>

    <!-- ===== Actions: Save | Share | Listen ===== -->
    <Grid ColumnDefinitions=""*, *{% if showToggle %}, *{% endif %}"" ColumnSpacing=""8"" StyleClass=""px-16"">

        <VerticalStackLayout Grid.Column=""0"" Spacing=""6"" HorizontalOptions=""Center"">
            <Rock:FollowingIcon
                EntityTypeId=""{{ Item.TypeId }}""
                EntityId=""{{ Item.Id }}""
                IsFollowed=""{{ Item | IsFollowed }}""
                FontSize=""22""
                HorizontalOptions=""Center""
                FollowingIconClass=""heart""
                FollowingIconFamily=""FontAwesomeSolid""
                FollowingIconColor=""{Rock:PaletteColor App-Primary-Strong}""
                NotFollowingIconClass=""heart""
                NotFollowingIconFamily=""FontAwesomeRegular""
                NotFollowingIconColor=""{AppThemeBinding Light=#3F3F46, Dark=#E4E4E7}""
                NotLoggedInText=""Sign in to save this message to your list."" />
            <Label StyleClass=""caption1, font-weight-semi-bold, text-interface-medium"" Text=""SAVE"" HorizontalOptions=""Center"" />
        </VerticalStackLayout>

        <VerticalStackLayout Grid.Column=""1"" Spacing=""6"" HorizontalOptions=""Center"">
            <Rock:Icon IconClass=""share-square"" IconFamily=""FontAwesomeSolid"" FontSize=""22""
                StyleClass=""text-interface-stronger"" HorizontalOptions=""Center"" />
            <Label StyleClass=""caption1, font-weight-semi-bold, text-interface-medium"" Text=""SHARE"" HorizontalOptions=""Center"" />
            <VerticalStackLayout.GestureRecognizers>
                <TapGestureRecognizer Command=""{Binding ShareContent}"">
                    <TapGestureRecognizer.CommandParameter>
                        <Rock:ShareContentParameters
                            Title=""{{ Item.Title | Escape }}""
                            Text=""{{ Item.Title | Escape }}{% if speaker != '' %} - {{ speaker | Escape }}{% endif %}""
                            Uri=""{{ shareUri | Escape }}"" />
                    </TapGestureRecognizer.CommandParameter>
                </TapGestureRecognizer>
            </VerticalStackLayout.GestureRecognizers>
        </VerticalStackLayout>

        {% if showToggle %}
        <VerticalStackLayout Grid.Column=""2"" Spacing=""6"" HorizontalOptions=""Center"">
            {% if playMode == 'audio' %}
                <Rock:Icon IconClass=""video"" IconFamily=""FontAwesomeSolid"" FontSize=""22""
                    StyleClass=""text-interface-stronger"" HorizontalOptions=""Center"" />
                <Label StyleClass=""caption1, font-weight-semi-bold, text-interface-medium"" Text=""WATCH"" HorizontalOptions=""Center"" />
                <VerticalStackLayout.GestureRecognizers>
                    <TapGestureRecognizer Command=""{Binding ReplacePage}"" CommandParameter=""{{ sermonPageGuid }}?ContentChannelItemId={{ Item.Id }}"" />
                </VerticalStackLayout.GestureRecognizers>
            {% else %}
                <Rock:Icon IconClass=""headphones"" IconFamily=""FontAwesomeSolid"" FontSize=""22""
                    StyleClass=""text-interface-stronger"" HorizontalOptions=""Center"" />
                <Label StyleClass=""caption1, font-weight-semi-bold, text-interface-medium"" Text=""LISTEN"" HorizontalOptions=""Center"" />
                <VerticalStackLayout.GestureRecognizers>
                    <TapGestureRecognizer Command=""{Binding ReplacePage}"" CommandParameter=""{{ sermonPageGuid }}?ContentChannelItemId={{ Item.Id }}&amp;Mode=audio"" />
                </VerticalStackLayout.GestureRecognizers>
            {% endif %}
        </VerticalStackLayout>
        {% endif %}

    </Grid>

    <BoxView HeightRequest=""1"" StyleClass=""bg-interface-softer, mx-16, my-16"" />

    <Rock:Html Text=""{{ Item.Content | Escape }}"" StyleClass=""body, text-interface-stronger, px-16"" />

    <!-- ===== In This Series (includes the current sermon, badged NOW PLAYING) ===== -->
    {% if siblingCount > 1 %}
        <BoxView HeightRequest=""1"" StyleClass=""bg-interface-softer, mx-16, mt-16, mb-16"" />
        <Label Text=""In This Series"" StyleClass=""title3, bold, text-interface-strongest"" Margin=""16,0,16,12"" />
        <ScrollView Orientation=""Horizontal"" HorizontalScrollBarVisibility=""Never"">
            <HorizontalStackLayout Spacing=""12"" Padding=""16,0"">
                {% for sib in siblings %}
                    {%- comment -%}
                        The current sermon is included rather than skipped, so the row shows the
                        whole series and where you are in it. It keeps its natural date order
                        instead of being pinned first - position in the series is the useful bit.
                        It gets a scrim + NOW PLAYING badge and no gesture recogniser, so tapping
                        it cannot push a second copy of the page onto the stack.
                    {%- endcomment -%}
                    {%- assign isCurrent = false -%}
                    {%- if sib.Id == Item.Id -%}{%- assign isCurrent = true -%}{%- endif -%}

                    {%- comment -%} thumbnail: Media File thumb -> Image -> Series Image {%- endcomment -%}
                    {%- assign sibImg = sib | Attribute:'MediaFile','DefaultThumbnailUrl' -%}
                    {%- if sibImg == '' or sibImg == null -%}{%- assign sibImg = sib | Attribute:'Image','RawValue' -%}{%- if sibImg != '' and sibImg != null -%}{%- unless sibImg contains 'http' -%}{%- assign sibImg = appRoot | Append:'GetImage.ashx?Guid=' | Append:sibImg -%}{%- endunless -%}{%- endif -%}{%- endif -%}
                    {%- if sibImg == '' and seriesGuid != '' -%}{%- assign sibImg = seriesGuid -%}{%- endif -%}

                    <VerticalStackLayout WidthRequest=""240"" Spacing=""8"">
                        <Grid WidthRequest=""240"" HeightRequest=""135"">
                            <Rock:StyledBorder CornerRadius=""10"" Padding=""0"" WidthRequest=""240"" HeightRequest=""135"" StyleClass=""bg-interface-softer"">{% if sibImg != '' %}<Rock:Image Source=""{{ sibImg | Escape }}"" Aspect=""AspectFill"" WidthRequest=""240"" HeightRequest=""135"" />{% else %}<Rock:Icon IconClass=""video"" IconFamily=""FontAwesomeSolid"" FontSize=""28"" StyleClass=""text-interface-soft"" HorizontalOptions=""Center"" VerticalOptions=""Center"" />{% endif %}</Rock:StyledBorder>
                            {% if isCurrent %}
                            <Rock:StyledBorder CornerRadius=""10"" Padding=""0"" WidthRequest=""240"" HeightRequest=""135""
                                BackgroundColor=""#B3000000"" StrokeThickness=""2""
                                Stroke=""{Rock:PaletteColor App-Primary-Strong}"">
                                <VerticalStackLayout Spacing=""8"" HorizontalOptions=""Center"" VerticalOptions=""Center"">
                                    <Rock:StyledBorder WidthRequest=""40"" HeightRequest=""40"" CornerRadius=""20"" Padding=""0""
                                        StrokeThickness=""0"" HorizontalOptions=""Center""
                                        BackgroundColor=""{Rock:PaletteColor App-Primary-Strong}"">
                                        <Rock:Icon IconClass=""play"" IconFamily=""FontAwesomeSolid"" FontSize=""16""
                                            TextColor=""#FFFFFF"" HorizontalOptions=""Center"" VerticalOptions=""Center"" />
                                    </Rock:StyledBorder>
                                    <Label Text=""NOW PLAYING"" StyleClass=""caption2, bold"" TextColor=""#FFFFFF""
                                        HorizontalOptions=""Center"" HorizontalTextAlignment=""Center"" />
                                </VerticalStackLayout>
                            </Rock:StyledBorder>
                            {% endif %}
                        </Grid>
                        <Label Text=""{{ sib.Title | Escape }}"" MaxLines=""2"" LineBreakMode=""TailTruncation""
                            {% if isCurrent %}StyleClass=""body, bold"" TextColor=""{Rock:PaletteColor App-Primary-Strong}""{% else %}StyleClass=""body, bold, text-interface-strongest""{% endif %} />
                        {% unless isCurrent %}
                        <VerticalStackLayout.GestureRecognizers>
                            <TapGestureRecognizer Command=""{Binding PushPage}"" CommandParameter=""{{ sermonPageGuid }}?ContentChannelItemId={{ sib.Id }}"" />
                        </VerticalStackLayout.GestureRecognizers>
                        {% endunless %}
                    </VerticalStackLayout>
                {% endfor %}
            </HorizontalStackLayout>
        </ScrollView>
    {% endif %}

    <BoxView HeightRequest=""24"" Color=""Transparent"" />
</VerticalStackLayout>" );   // ContentTemplate
            RockMigrationHelper.AddBlockAttributeValue( "A67C5D2C-1161-4EBA-8F92-93962EF739F6", "45EC896A-6A9F-495E-8F88-1BC800612B2D", @"RockEntity" );   // EnabledLavaCommands
            RockMigrationHelper.AddBlockAttributeValue( "A67C5D2C-1161-4EBA-8F92-93962EF739F6", "49913217-BF13-4270-8023-C56BDA52C790", @"0a63a427-e6b5-2284-45b3-789b293c02ea" );   // ContentChannel
            RockMigrationHelper.AddBlockAttributeValue( "A67C5D2C-1161-4EBA-8F92-93962EF739F6", "616351D9-41FD-4E84-9378-78140BE30605", @"False" );   // LogInteractions
            RockMigrationHelper.AddBlockAttributeValue( "C3E6A9C2-EBA4-4AF9-A37B-3E7F8CFE0DF4", "5682EDBF-68DA-4B43-A593-6C2B936C2839", @"{%- assign paramLimit = PageParameter.Limit | Default:'25' | AsInteger %}
{%- assign paramPage = PageParameter.pg | Default:'1' | AsInteger -%}

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
ORDER BY ft.TransactionDateTime DESC
OFFSET (@PageNumber-1)*@RowsOfPage ROWS
FETCH NEXT @RowsOfPage ROWS ONLY
{%- endsql -%}

{%- assign loopTransactionDate = '' -%}
{%- assign transactionListSize = transactionlist | Size -%}

<VerticalStackLayout Spacing=""0"" StyleClass=""p-16, bg-interface-softest"">

{%- if transactionListSize > 0 -%}

    {%- for transaction in transactionlist -%}
        {%- assign txnDate = transaction.TransactionDateTime | Date:'d MMM yyyy' | Upcase -%}
        {% if loopTransactionDate != txnDate %}
            <Label Text=""{{ txnDate }}""
                StyleClass=""caption1, font-weight-semi-bold, text-interface-medium"" Margin=""4,12,0,6"" />
        {% endif %}
        {%- assign loopTransactionDate = txnDate -%}
        <Rock:StyledBorder CornerRadius=""12"" Padding=""14,12"" StyleClass=""bg-interface-softest, mb-8"">
            <Grid ColumnDefinitions=""*, Auto"" ColumnSpacing=""12"" VerticalOptions=""Center"">
                <VerticalStackLayout Grid.Column=""0"" Spacing=""2"" VerticalOptions=""Center"">
                    <Label Text=""{{ transaction.Name | Escape }}""
                        StyleClass=""body, bold, text-interface-strongest"" LineBreakMode=""TailTruncation"" />
                    <Label Text=""{{ transaction.AccountName | Escape }}""
                        StyleClass=""footnote, text-interface-medium"" LineBreakMode=""TailTruncation"" />
                </VerticalStackLayout>
                <Label Grid.Column=""1"" Text=""{{ transaction.Amount | FormatAsCurrency }}""
                    StyleClass=""body, bold, text-interface-strongest"" VerticalOptions=""Center"" />
            </Grid>
        </Rock:StyledBorder>
    {%- endfor -%}

    <!-- ===== Pagination ===== -->
    <Grid ColumnDefinitions=""Auto, *, Auto"" Margin=""0,12,0,0"" VerticalOptions=""Center"">
        {% if paramPage > 1 %}
            <Rock:StyledBorder Grid.Column=""0"" CornerRadius=""8"" Padding=""16,10"" StrokeThickness=""1.5""
                Stroke=""{AppThemeBinding Light=#D4D4D8, Dark=#3F3F46}"" BackgroundColor=""Transparent"">
                <Label Text=""Previous {{ paramLimit }}"" StyleClass=""footnote, bold, text-interface-strong"" />
                <Rock:StyledBorder.GestureRecognizers>
                    <TapGestureRecognizer Command=""{Binding PopPage}"" />
                </Rock:StyledBorder.GestureRecognizers>
            </Rock:StyledBorder>
        {% endif %}

        {%- if transactionListSize == paramLimit -%}
            <Rock:StyledBorder Grid.Column=""2"" CornerRadius=""8"" Padding=""16,10"" StrokeThickness=""1.5""
                Stroke=""{Rock:PaletteColor App-Primary-Strong}"" BackgroundColor=""Transparent"">
                <Label Text=""Next {{ paramLimit }}"" TextColor=""{Rock:PaletteColor App-Primary-Strong}"" StyleClass=""footnote, bold"" />
                <Rock:StyledBorder.GestureRecognizers>
                    <TapGestureRecognizer Command=""{Binding PushPage}"" CommandParameter=""{{ CurrentPage.Guid }}?pg={{ paramPage | Plus:1 }}"" />
                </Rock:StyledBorder.GestureRecognizers>
            </Rock:StyledBorder>
        {%- endif -%}
    </Grid>

{% else %}

    <VerticalStackLayout Spacing=""12"" StyleClass=""p-16"" HorizontalOptions=""Center"" VerticalOptions=""Center"">
        <Rock:Icon IconClass=""receipt"" IconFamily=""FontAwesomeSolid"" FontSize=""34""
            StyleClass=""text-interface-soft"" HorizontalOptions=""Center"" />
        <Label Text=""You currently have no transactions available.""
            StyleClass=""body, text-interface-medium"" HorizontalOptions=""Center"" HorizontalTextAlignment=""Center"" />
    </VerticalStackLayout>

{%- endif -%}

</VerticalStackLayout>" );   // Content
            RockMigrationHelper.AddBlockAttributeValue( "C3E6A9C2-EBA4-4AF9-A37B-3E7F8CFE0DF4", "24516448-3F1F-4F27-97A1-CFB4F8B277B5", @"Sql" );   // EnabledLavaCommands
            RockMigrationHelper.AddBlockAttributeValue( "C3E6A9C2-EBA4-4AF9-A37B-3E7F8CFE0DF4", "B31D29A0-3725-4AEB-8360-7D91B9CDFE47", @"True" );   // DynamicContent
            RockMigrationHelper.AddBlockAttributeValue( "5166C7A4-C480-4F93-8A1F-634775438974", "5682EDBF-68DA-4B43-A593-6C2B936C2839", @"{%- assign activeTab = 'mygiving' -%}
{% comment %} ^ set to 'notifications' | 'mylist' | 'mygiving' on each page {% endcomment %}

{%- assign notificationsPageGuid = '9d8435bd-8583-4325-aefc-af073d0e9020' -%}
{%- assign myListPageGuid        = 'a4ffb56b-938e-44b9-adb2-a2529b0d8af2' -%}
{%- assign myGivingPageGuid      = 'df9a3772-cc1c-4ce3-885b-abf828ef6065' -%}
{%- assign editProfilePageGuid   = '941096ae-fb51-4450-9db9-6248b584d917' -%}
{%- assign loginPageGuid = '9bb25932-4d56-417c-911b-dc915167e7bc' -%}

{%- assign coverImageUrl = '' -%}
{% comment %} coverImageUrl: static brand banner or a person cover attribute; empty = solid band {% endcomment %}

<VerticalStackLayout Spacing=""0"" StyleClass=""bg-interface-softest"">

    <!-- ===== Banner + overlapping avatar ===== -->
    <Grid HorizontalOptions=""Fill"">
        {% if coverImageUrl != '' %}
            <Rock:Image Source=""{{ coverImageUrl | Escape }}"" Aspect=""AspectFill"" HeightRequest=""150"" VerticalOptions=""Start"" />
        {% else %}
            <Rock:StyledBorder HeightRequest=""150"" VerticalOptions=""Start"" StyleClass=""bg-interface-soft"" />
        {% endif %}

        <Grid WidthRequest=""92"" HeightRequest=""92"" HorizontalOptions=""Center"" VerticalOptions=""Start""
              Margin=""0,104,0,0"" BackgroundColor=""Transparent"">
        {% if CurrentPerson != null %}
            <Rock:StyledBorder WidthRequest=""92"" HeightRequest=""92"" CornerRadius=""46"" Padding=""0""
                StrokeThickness=""3"" Stroke=""{AppThemeBinding Light=#FFFFFF, Dark=#18181B}""
                StyleClass=""bg-interface-softer"">
                <Rock:Image x:Name=""PersonImage""
                    Source=""{{ 'Global' | Attribute:'PublicApplicationRoot' }}GetAvatar.ashx?PersonGuid={{ CurrentPerson.Guid }}&amp;w=184&amp;v={{ CurrentPerson.PhotoId }}""
                    Aspect=""AspectFill"" WidthRequest=""92"" HeightRequest=""92"" />
            </Rock:StyledBorder>
            <Rock:StyledBorder WidthRequest=""30"" HeightRequest=""30"" CornerRadius=""15"" Padding=""0""
                HorizontalOptions=""End"" VerticalOptions=""End"" StrokeThickness=""2""
                BackgroundColor=""{Rock:PaletteColor App-Primary-Strong}"" Stroke=""{AppThemeBinding Light=#FFFFFF, Dark=#18181B}"">
                <Rock:Icon IconClass=""pen"" IconFamily=""FontAwesomeSolid"" FontSize=""13""
                    TextColor=""#FFFFFF"" HorizontalOptions=""Center"" VerticalOptions=""Center"" />
            </Rock:StyledBorder>
            <Grid.GestureRecognizers>
                <TapGestureRecognizer Command=""{Binding UpdatePersonProfilePhoto}"">
                    <TapGestureRecognizer.CommandParameter>
                        <Rock:UpdatePersonProfilePhotoCommandParameters PersonGuid=""{{ CurrentPerson.Guid }}"" Image=""{x:Reference PersonImage}"" />
                    </TapGestureRecognizer.CommandParameter>
                </TapGestureRecognizer>
            </Grid.GestureRecognizers>
        {% else %}
            <Rock:StyledBorder WidthRequest=""92"" HeightRequest=""92"" CornerRadius=""46"" Padding=""0""
                StrokeThickness=""3"" Stroke=""{AppThemeBinding Light=#FFFFFF, Dark=#18181B}""
                BackgroundColor=""{Rock:PaletteColor App-Primary-Strong}"">
                <Rock:Icon IconClass=""user"" IconFamily=""FontAwesomeSolid"" FontSize=""44""
                    TextColor=""#FFFFFF"" HorizontalOptions=""Center"" VerticalOptions=""Center"" />
            </Rock:StyledBorder>
            <Grid.GestureRecognizers>
                <TapGestureRecognizer Command=""{Binding PushPage}"" CommandParameter=""{{ loginPageGuid }}"" />
            </Grid.GestureRecognizers>
        {% endif %}
        </Grid>
    </Grid>

    <!-- ===== Name + action link ===== -->
    <VerticalStackLayout Spacing=""2"" StyleClass=""p-8"" HorizontalOptions=""Center"">
        {% if CurrentPerson != null %}
            <Label Text=""{{ CurrentPerson.FullName | Escape }}""
                StyleClass=""title2, bold, text-interface-strongest"" HorizontalOptions=""Center"" />
            <Label Text=""Edit Profile"" TextColor=""{Rock:PaletteColor App-Primary-Strong}""
                StyleClass=""body, font-weight-semi-bold"" HorizontalOptions=""Center"">
                <Label.GestureRecognizers>
                    <TapGestureRecognizer Command=""{Binding PushPage}"" CommandParameter=""{{ editProfilePageGuid }}"" />
                </Label.GestureRecognizers>
            </Label>
        {% else %}
            <Label Text=""Welcome""
                StyleClass=""title2, bold, text-interface-strongest"" HorizontalOptions=""Center"" />
            <Label Text=""Sign in or Register"" TextColor=""{Rock:PaletteColor App-Primary-Strong}""
                StyleClass=""body, font-weight-semi-bold"" HorizontalOptions=""Center"">
                <Label.GestureRecognizers>
                    <TapGestureRecognizer Command=""{Binding PushPage}"" CommandParameter=""{{ loginPageGuid }}"" />
                </Label.GestureRecognizers>
            </Label>
        {% endif %}
    </VerticalStackLayout>

    <!-- ===== Faux tab bar ===== -->
    <Grid ColumnDefinitions=""*, *, *"" StyleClass=""bg-interface-softest"">

        <!-- Notifications -->
        <VerticalStackLayout Grid.Column=""0"" Spacing=""6"" BackgroundColor=""Transparent"" StyleClass=""pt-8"">
            <Rock:Icon IconClass=""bell"" IconFamily=""FontAwesomeSolid"" FontSize=""22"" HorizontalOptions=""Center""
                {% if activeTab == 'notifications' %}TextColor=""{Rock:PaletteColor App-Primary-Strong}""{% else %}StyleClass=""text-interface-medium""{% endif %} />
            <Label Text=""Notifications"" HorizontalOptions=""Center""
                StyleClass=""caption1, font-weight-semi-bold{% unless activeTab == 'notifications' %}, text-interface-medium{% endunless %}""
                {% if activeTab == 'notifications' %}TextColor=""{Rock:PaletteColor App-Primary-Strong}""{% endif %} />
            <BoxView HeightRequest=""3"" Color=""{% if activeTab == 'notifications' %}{Rock:PaletteColor App-Primary-Strong}{% else %}Transparent{% endif %}"" />
            {% unless activeTab == 'notifications' %}
                <VerticalStackLayout.GestureRecognizers>
                    <TapGestureRecognizer Command=""{Binding ReplacePage}"">
                        <TapGestureRecognizer.CommandParameter>
                            <Rock:ReplacePageParameters PageGuid=""{{ notificationsPageGuid }}"" WaitForReady=""true"" />
                        </TapGestureRecognizer.CommandParameter>
                    </TapGestureRecognizer>
                </VerticalStackLayout.GestureRecognizers>
            {% endunless %}
        </VerticalStackLayout>

        <!-- My List -->
        <VerticalStackLayout Grid.Column=""1"" Spacing=""6"" BackgroundColor=""Transparent"" StyleClass=""pt-8"">
            <Rock:Icon IconClass=""bookmark"" IconFamily=""FontAwesomeSolid"" FontSize=""22"" HorizontalOptions=""Center""
                {% if activeTab == 'mylist' %}TextColor=""{Rock:PaletteColor App-Primary-Strong}""{% else %}StyleClass=""text-interface-medium""{% endif %} />
            <Label Text=""My List"" HorizontalOptions=""Center""
                StyleClass=""caption1, font-weight-semi-bold{% unless activeTab == 'mylist' %}, text-interface-medium{% endunless %}""
                {% if activeTab == 'mylist' %}TextColor=""{Rock:PaletteColor App-Primary-Strong}""{% endif %} />
            <BoxView HeightRequest=""3"" Color=""{% if activeTab == 'mylist' %}{Rock:PaletteColor App-Primary-Strong}{% else %}Transparent{% endif %}"" />
            {% unless activeTab == 'mylist' %}
                <VerticalStackLayout.GestureRecognizers>
                    <TapGestureRecognizer Command=""{Binding ReplacePage}"">
                        <TapGestureRecognizer.CommandParameter>
                            <Rock:ReplacePageParameters PageGuid=""{{ myListPageGuid }}"" WaitForReady=""true"" />
                        </TapGestureRecognizer.CommandParameter>
                    </TapGestureRecognizer>
                </VerticalStackLayout.GestureRecognizers>
            {% endunless %}
        </VerticalStackLayout>

        <!-- My Giving -->
        <VerticalStackLayout Grid.Column=""2"" Spacing=""6"" BackgroundColor=""Transparent"" StyleClass=""pt-8"">
            <Rock:Icon IconClass=""hand-holding-heart"" IconFamily=""FontAwesomeSolid"" FontSize=""22"" HorizontalOptions=""Center""
                {% if activeTab == 'mygiving' %}TextColor=""{Rock:PaletteColor App-Primary-Strong}""{% else %}StyleClass=""text-interface-medium""{% endif %} />
            <Label Text=""My Giving"" HorizontalOptions=""Center""
                StyleClass=""caption1, font-weight-semi-bold{% unless activeTab == 'mygiving' %}, text-interface-medium{% endunless %}""
                {% if activeTab == 'mygiving' %}TextColor=""{Rock:PaletteColor App-Primary-Strong}""{% endif %} />
            <BoxView HeightRequest=""3"" Color=""{% if activeTab == 'mygiving' %}{Rock:PaletteColor App-Primary-Strong}{% else %}Transparent{% endif %}"" />
            {% unless activeTab == 'mygiving' %}
                <VerticalStackLayout.GestureRecognizers>
                    <TapGestureRecognizer Command=""{Binding ReplacePage}"">
                        <TapGestureRecognizer.CommandParameter>
                            <Rock:ReplacePageParameters PageGuid=""{{ myGivingPageGuid }}"" WaitForReady=""true"" />
                        </TapGestureRecognizer.CommandParameter>
                    </TapGestureRecognizer>
                </VerticalStackLayout.GestureRecognizers>
            {% endunless %}
        </VerticalStackLayout>

    </Grid>

    <BoxView HeightRequest=""1"" StyleClass=""bg-interface-softer"" />

</VerticalStackLayout>" );   // Content
            RockMigrationHelper.AddBlockAttributeValue( "5166C7A4-C480-4F93-8A1F-634775438974", "B31D29A0-3725-4AEB-8360-7D91B9CDFE47", @"True" );   // DynamicContent
            RockMigrationHelper.AddBlockAttributeValue( "78C66DE9-AE93-4B11-9667-B87EF00A1C4C", "5682EDBF-68DA-4B43-A593-6C2B936C2839", @"{%- assign activeTab = 'mylist' -%}
{% comment %} ^ set to 'notifications' | 'mylist' | 'mygiving' on each page {% endcomment %}

{%- assign notificationsPageGuid = '9d8435bd-8583-4325-aefc-af073d0e9020' -%}
{%- assign myListPageGuid        = 'a4ffb56b-938e-44b9-adb2-a2529b0d8af2' -%}
{%- assign myGivingPageGuid      = 'df9a3772-cc1c-4ce3-885b-abf828ef6065' -%}
{%- assign editProfilePageGuid   = '941096ae-fb51-4450-9db9-6248b584d917' -%}
{%- assign loginPageGuid = '9bb25932-4d56-417c-911b-dc915167e7bc' -%}

{%- assign coverImageUrl = '' -%}
{% comment %} coverImageUrl: static brand banner or a person cover attribute; empty = solid band {% endcomment %}

<VerticalStackLayout Spacing=""0"" StyleClass=""bg-interface-softest"">

    <!-- ===== Banner + overlapping avatar ===== -->
    <Grid HorizontalOptions=""Fill"">
        {% if coverImageUrl != '' %}
            <Rock:Image Source=""{{ coverImageUrl | Escape }}"" Aspect=""AspectFill"" HeightRequest=""150"" VerticalOptions=""Start"" />
        {% else %}
            <Rock:StyledBorder HeightRequest=""150"" VerticalOptions=""Start"" StyleClass=""bg-interface-soft"" />
        {% endif %}

        <Grid WidthRequest=""92"" HeightRequest=""92"" HorizontalOptions=""Center"" VerticalOptions=""Start""
              Margin=""0,104,0,0"" BackgroundColor=""Transparent"">
        {% if CurrentPerson != null %}
            <Rock:StyledBorder WidthRequest=""92"" HeightRequest=""92"" CornerRadius=""46"" Padding=""0""
                StrokeThickness=""3"" Stroke=""{AppThemeBinding Light=#FFFFFF, Dark=#18181B}""
                StyleClass=""bg-interface-softer"">
                <Rock:Image x:Name=""PersonImage""
                    Source=""{{ 'Global' | Attribute:'PublicApplicationRoot' }}GetAvatar.ashx?PersonGuid={{ CurrentPerson.Guid }}&amp;w=184&amp;v={{ CurrentPerson.PhotoId }}""
                    Aspect=""AspectFill"" WidthRequest=""92"" HeightRequest=""92"" />
            </Rock:StyledBorder>
            <Rock:StyledBorder WidthRequest=""30"" HeightRequest=""30"" CornerRadius=""15"" Padding=""0""
                HorizontalOptions=""End"" VerticalOptions=""End"" StrokeThickness=""2""
                BackgroundColor=""{Rock:PaletteColor App-Primary-Strong}"" Stroke=""{AppThemeBinding Light=#FFFFFF, Dark=#18181B}"">
                <Rock:Icon IconClass=""pen"" IconFamily=""FontAwesomeSolid"" FontSize=""13""
                    TextColor=""#FFFFFF"" HorizontalOptions=""Center"" VerticalOptions=""Center"" />
            </Rock:StyledBorder>
            <Grid.GestureRecognizers>
                <TapGestureRecognizer Command=""{Binding UpdatePersonProfilePhoto}"">
                    <TapGestureRecognizer.CommandParameter>
                        <Rock:UpdatePersonProfilePhotoCommandParameters PersonGuid=""{{ CurrentPerson.Guid }}"" Image=""{x:Reference PersonImage}"" />
                    </TapGestureRecognizer.CommandParameter>
                </TapGestureRecognizer>
            </Grid.GestureRecognizers>
        {% else %}
            <Rock:StyledBorder WidthRequest=""92"" HeightRequest=""92"" CornerRadius=""46"" Padding=""0""
                StrokeThickness=""3"" Stroke=""{AppThemeBinding Light=#FFFFFF, Dark=#18181B}""
                BackgroundColor=""{Rock:PaletteColor App-Primary-Strong}"">
                <Rock:Icon IconClass=""user"" IconFamily=""FontAwesomeSolid"" FontSize=""44""
                    TextColor=""#FFFFFF"" HorizontalOptions=""Center"" VerticalOptions=""Center"" />
            </Rock:StyledBorder>
            <Grid.GestureRecognizers>
                <TapGestureRecognizer Command=""{Binding PushPage}"" CommandParameter=""{{ loginPageGuid }}"" />
            </Grid.GestureRecognizers>
        {% endif %}
        </Grid>
    </Grid>

    <!-- ===== Name + action link ===== -->
    <VerticalStackLayout Spacing=""2"" StyleClass=""p-8"" HorizontalOptions=""Center"">
        {% if CurrentPerson != null %}
            <Label Text=""{{ CurrentPerson.FullName | Escape }}""
                StyleClass=""title2, bold, text-interface-strongest"" HorizontalOptions=""Center"" />
            <Label Text=""Edit Profile"" TextColor=""{Rock:PaletteColor App-Primary-Strong}""
                StyleClass=""body, font-weight-semi-bold"" HorizontalOptions=""Center"">
                <Label.GestureRecognizers>
                    <TapGestureRecognizer Command=""{Binding PushPage}"" CommandParameter=""{{ editProfilePageGuid }}"" />
                </Label.GestureRecognizers>
            </Label>
        {% else %}
            <Label Text=""Welcome""
                StyleClass=""title2, bold, text-interface-strongest"" HorizontalOptions=""Center"" />
            <Label Text=""Sign in or Register"" TextColor=""{Rock:PaletteColor App-Primary-Strong}""
                StyleClass=""body, font-weight-semi-bold"" HorizontalOptions=""Center"">
                <Label.GestureRecognizers>
                    <TapGestureRecognizer Command=""{Binding PushPage}"" CommandParameter=""{{ loginPageGuid }}"" />
                </Label.GestureRecognizers>
            </Label>
        {% endif %}
    </VerticalStackLayout>

    <!-- ===== Faux tab bar ===== -->
    <Grid ColumnDefinitions=""*, *, *"" StyleClass=""bg-interface-softest"">

        <!-- Notifications -->
        <VerticalStackLayout Grid.Column=""0"" Spacing=""6"" BackgroundColor=""Transparent"" StyleClass=""pt-8"">
            <Rock:Icon IconClass=""bell"" IconFamily=""FontAwesomeSolid"" FontSize=""22"" HorizontalOptions=""Center""
                {% if activeTab == 'notifications' %}TextColor=""{Rock:PaletteColor App-Primary-Strong}""{% else %}StyleClass=""text-interface-medium""{% endif %} />
            <Label Text=""Notifications"" HorizontalOptions=""Center""
                StyleClass=""caption1, font-weight-semi-bold{% unless activeTab == 'notifications' %}, text-interface-medium{% endunless %}""
                {% if activeTab == 'notifications' %}TextColor=""{Rock:PaletteColor App-Primary-Strong}""{% endif %} />
            <BoxView HeightRequest=""3"" Color=""{% if activeTab == 'notifications' %}{Rock:PaletteColor App-Primary-Strong}{% else %}Transparent{% endif %}"" />
            {% unless activeTab == 'notifications' %}
                <VerticalStackLayout.GestureRecognizers>
                    <TapGestureRecognizer Command=""{Binding ReplacePage}"">
                        <TapGestureRecognizer.CommandParameter>
                            <Rock:ReplacePageParameters PageGuid=""{{ notificationsPageGuid }}"" WaitForReady=""true"" />
                        </TapGestureRecognizer.CommandParameter>
                    </TapGestureRecognizer>
                </VerticalStackLayout.GestureRecognizers>
            {% endunless %}
        </VerticalStackLayout>

        <!-- My List -->
        <VerticalStackLayout Grid.Column=""1"" Spacing=""6"" BackgroundColor=""Transparent"" StyleClass=""pt-8"">
            <Rock:Icon IconClass=""bookmark"" IconFamily=""FontAwesomeSolid"" FontSize=""22"" HorizontalOptions=""Center""
                {% if activeTab == 'mylist' %}TextColor=""{Rock:PaletteColor App-Primary-Strong}""{% else %}StyleClass=""text-interface-medium""{% endif %} />
            <Label Text=""My List"" HorizontalOptions=""Center""
                StyleClass=""caption1, font-weight-semi-bold{% unless activeTab == 'mylist' %}, text-interface-medium{% endunless %}""
                {% if activeTab == 'mylist' %}TextColor=""{Rock:PaletteColor App-Primary-Strong}""{% endif %} />
            <BoxView HeightRequest=""3"" Color=""{% if activeTab == 'mylist' %}{Rock:PaletteColor App-Primary-Strong}{% else %}Transparent{% endif %}"" />
            {% unless activeTab == 'mylist' %}
                <VerticalStackLayout.GestureRecognizers>
                    <TapGestureRecognizer Command=""{Binding ReplacePage}"">
                        <TapGestureRecognizer.CommandParameter>
                            <Rock:ReplacePageParameters PageGuid=""{{ myListPageGuid }}"" WaitForReady=""true"" />
                        </TapGestureRecognizer.CommandParameter>
                    </TapGestureRecognizer>
                </VerticalStackLayout.GestureRecognizers>
            {% endunless %}
        </VerticalStackLayout>

        <!-- My Giving -->
        <VerticalStackLayout Grid.Column=""2"" Spacing=""6"" BackgroundColor=""Transparent"" StyleClass=""pt-8"">
            <Rock:Icon IconClass=""hand-holding-heart"" IconFamily=""FontAwesomeSolid"" FontSize=""22"" HorizontalOptions=""Center""
                {% if activeTab == 'mygiving' %}TextColor=""{Rock:PaletteColor App-Primary-Strong}""{% else %}StyleClass=""text-interface-medium""{% endif %} />
            <Label Text=""My Giving"" HorizontalOptions=""Center""
                StyleClass=""caption1, font-weight-semi-bold{% unless activeTab == 'mygiving' %}, text-interface-medium{% endunless %}""
                {% if activeTab == 'mygiving' %}TextColor=""{Rock:PaletteColor App-Primary-Strong}""{% endif %} />
            <BoxView HeightRequest=""3"" Color=""{% if activeTab == 'mygiving' %}{Rock:PaletteColor App-Primary-Strong}{% else %}Transparent{% endif %}"" />
            {% unless activeTab == 'mygiving' %}
                <VerticalStackLayout.GestureRecognizers>
                    <TapGestureRecognizer Command=""{Binding ReplacePage}"">
                        <TapGestureRecognizer.CommandParameter>
                            <Rock:ReplacePageParameters PageGuid=""{{ myGivingPageGuid }}"" WaitForReady=""true"" />
                        </TapGestureRecognizer.CommandParameter>
                    </TapGestureRecognizer>
                </VerticalStackLayout.GestureRecognizers>
            {% endunless %}
        </VerticalStackLayout>

    </Grid>

    <BoxView HeightRequest=""1"" StyleClass=""bg-interface-softer"" />

</VerticalStackLayout>" );   // Content
            RockMigrationHelper.AddBlockAttributeValue( "78C66DE9-AE93-4B11-9667-B87EF00A1C4C", "B31D29A0-3725-4AEB-8360-7D91B9CDFE47", @"True" );   // DynamicContent
            RockMigrationHelper.AddBlockAttributeValue( "597C9DB9-24E6-4FBF-A97D-A21A32F3B81D", "FB51E7CE-C987-4269-B22B-B48CE0F765AB", @"False" );   // ShowInactiveMembersFilter
            RockMigrationHelper.AddBlockAttributeValue( "597C9DB9-24E6-4FBF-A97D-A21A32F3B81D", "F98CF893-5C82-46AA-A044-8CAAFB0DBD56", @"False" );   // ShowGroupRoleTypeFilter
            RockMigrationHelper.AddBlockAttributeValue( "597C9DB9-24E6-4FBF-A97D-A21A32F3B81D", "0CBCDB7C-CC81-4BFD-BEF8-1F18291D8B0E", @"True" );   // ShowGroupRoleFilter
            RockMigrationHelper.AddBlockAttributeValue( "597C9DB9-24E6-4FBF-A97D-A21A32F3B81D", "93F3D6EF-97F7-42B0-833F-A9CB49EBD2F3", @"False" );   // ShowGenderFilter
            RockMigrationHelper.AddBlockAttributeValue( "597C9DB9-24E6-4FBF-A97D-A21A32F3B81D", "FA15A633-0BF0-45E0-A602-AAC2E222E048", @"False" );   // ShowChildGroupsFilter
            RockMigrationHelper.AddBlockAttributeValue( "597C9DB9-24E6-4FBF-A97D-A21A32F3B81D", "78DD984C-A7F6-4754-9483-03225375C98E", @"False" );   // ShowAttendanceFilter
            RockMigrationHelper.AddBlockAttributeValue( "597C9DB9-24E6-4FBF-A97D-A21A32F3B81D", "7240E7E6-8502-4127-B3BE-8EB8365A1AAD", @"3" );   // AttendanceFilterShortWeekRange
            RockMigrationHelper.AddBlockAttributeValue( "597C9DB9-24E6-4FBF-A97D-A21A32F3B81D", "69586657-1240-4D83-A3E5-3E1AAC448B3A", @"12" );   // AttendanceFilterLongWeekRange
            RockMigrationHelper.AddBlockAttributeValue( "8380B1DB-4D13-47D5-B290-D8D98E6FB4BD", "5682EDBF-68DA-4B43-A593-6C2B936C2839", @"{%- if CurrentPerson == null -%}
{%- assign loginPageGuid = '9bb25932-4d56-417c-911b-dc915167e7bc' -%}
{%- assign registerPageGuid = '' -%}
{%- assign heading = 'Sign in to continue' -%}
{%- assign subtext = ""Sign in to see the messages and series you've saved."" -%}

<VerticalStackLayout Spacing=""16"" StyleClass=""p-24"" HorizontalOptions=""Fill"" VerticalOptions=""Center"">

    <Rock:StyledBorder WidthRequest=""76"" HeightRequest=""76"" CornerRadius=""38"" HorizontalOptions=""Center"" StyleClass=""bg-interface-softer"">
        <Rock:Icon IconClass=""lock"" IconFamily=""FontAwesomeSolid"" FontSize=""30""
            StyleClass=""text-interface-medium"" HorizontalOptions=""Center"" VerticalOptions=""Center"" />
    </Rock:StyledBorder>

    <Label Text=""{{ heading | Escape }}""
        StyleClass=""title2, bold, text-interface-strongest"" HorizontalTextAlignment=""Center"" />
    <Label Text=""{{ subtext | Escape }}""
        StyleClass=""body, text-interface-medium"" HorizontalTextAlignment=""Center"" />

    <Rock:StyledBorder CornerRadius=""10"" Padding=""0"" BackgroundColor=""{Rock:PaletteColor App-Primary-Strong}"" HorizontalOptions=""Fill"" Margin=""0,8,0,0"">
        <Label Text=""Sign In"" TextColor=""#FFFFFF"" StyleClass=""body, bold""
            HorizontalOptions=""Center"" HorizontalTextAlignment=""Center"" Margin=""0,14"" />
        <Rock:StyledBorder.GestureRecognizers>
            <TapGestureRecognizer Command=""{Binding PushPage}"" CommandParameter=""{{ loginPageGuid }}"" />
        </Rock:StyledBorder.GestureRecognizers>
    </Rock:StyledBorder>

    {% if registerPageGuid != '' %}
    <Label HorizontalOptions=""Center"" HorizontalTextAlignment=""Center"" Margin=""0,4,0,0"">
        <Label.FormattedText>
            <FormattedString>
                <Span Text=""New here?"" StyleClass=""text-interface-medium"" />
                <Span Text=""Create an account"" TextColor=""{Rock:PaletteColor App-Primary-Strong}"" StyleClass=""bold"" />
            </FormattedString>
        </Label.FormattedText>
        <Label.GestureRecognizers>
            <TapGestureRecognizer Command=""{Binding PushPage}"" CommandParameter=""{{ registerPageGuid }}"" />
        </Label.GestureRecognizers>
    </Label>
    {% endif %}

</VerticalStackLayout>
{% else %}

    {%- comment -%} channel -> detail page. Add a line when you add a channel; anything unmapped falls back to Item Detail. {%- endcomment -%}
    {%- assign channelPages = '4^6d3762e4-0689-42fe-8535-a7b89c4fc028,5^4079b24c-d548-4cd0-a833-c5688bbef052' -%}
    {%- assign itemDetailGuid = 'c9d8bd2d-8f1e-42e6-a4c3-b71b0511e9c7' -%}
    {%- assign pageMap = channelPages | Split:',' -%}

    {% sql %}
        SELECT i.Id, i.Title, i.StartDateTime, i.ContentChannelId, cc.Name AS ChannelName, cc.IconCssClass AS ChannelIcon
        FROM Following f
        JOIN PersonAlias pa ON pa.Id = f.PersonAliasId
        JOIN ContentChannelItem i ON i.Id = f.EntityId
        JOIN ContentChannel cc ON cc.Id = i.ContentChannelId
        WHERE pa.PersonId = {{ CurrentPerson.Id }}
          AND f.EntityTypeId = (SELECT Id FROM EntityType WHERE [Name] = 'Rock.Model.ContentChannelItem')
          AND ISNULL(f.PurposeKey, '') = ''
        ORDER BY cc.Name, i.StartDateTime DESC
    {% endsql %}
    {%- assign savedItems = results -%}
    {%- assign total = savedItems | Size -%}

    <VerticalStackLayout Spacing=""0"" StyleClass=""p-16"">

        {% if total == 0 %}
            <Label StyleClass=""body, text-interface-medium""
                Text=""Nothing saved yet. Tap the heart on anything to add it here."" />
        {% endif %}

        {%- assign lastChannel = '' -%}
        {% for row in savedItems %}
            {%- capture rowChannelId %}{{ row.ContentChannelId }}{% endcapture -%}

            {%- comment -%} section header whenever the channel changes {%- endcomment -%}
            {% if row.ChannelName != lastChannel %}
                <Label Text=""{{ row.ChannelName | Upcase | Escape }}""
                    StyleClass=""caption1, font-weight-semi-bold, text-interface-medium"" Margin=""4,16,0,8"" />
                {%- assign lastChannel = row.ChannelName -%}
            {% endif %}

            {%- comment -%} icon comes from the channel's own IconCssClass, e.g. ""fa fa-archive"" -> ""archive"" {%- endcomment -%}
            {%- assign glyph = row.ChannelIcon | Default:'' | Replace:'fa fa-','' | Replace:'fas fa-','' | Replace:'far fa-','' | Replace:'fab fa-','' | Replace:'fa-','' | Trim -%}
            {%- if glyph == '' -%}{%- assign glyph = 'bookmark' -%}{%- endif -%}

            {%- assign detailGuid = itemDetailGuid -%}
            {%- for m in pageMap -%}
                {%- assign parts = m | Split:'^' -%}
                {%- assign mapId = parts | First | Trim -%}
                {%- if mapId == rowChannelId -%}{%- assign detailGuid = parts | Last | Trim -%}{%- endif -%}
            {%- endfor -%}

            <Rock:StyledBorder CornerRadius=""12"" Padding=""16,14"" StyleClass=""bg-interface-softest, my-4"">
                <Rock:StyledBorder.GestureRecognizers>
                    <TapGestureRecognizer Command=""{Binding PushPage}"" CommandParameter=""{{ detailGuid }}?ContentChannelItemId={{ row.Id }}"" />
                </Rock:StyledBorder.GestureRecognizers>
                <Grid ColumnDefinitions=""Auto, *, Auto"" ColumnSpacing=""14"">
                    <Rock:Icon Grid.Column=""0"" IconClass=""{{ glyph }}"" IconFamily=""FontAwesomeSolid"" FontSize=""18""
                        TextColor=""{Rock:PaletteColor App-Primary-Strong}"" VerticalOptions=""Center"" />
                    <VerticalStackLayout Grid.Column=""1"" Spacing=""3"" VerticalOptions=""Center"">
                        <Label StyleClass=""body, bold, text-interface-strongest"" Text=""{{ row.Title | Escape }}"" MaxLines=""2"" LineBreakMode=""TailTruncation"" />
                        {% if row.StartDateTime %}
                        <Label StyleClass=""caption1, text-interface-medium"" Text=""{{ row.StartDateTime | Date:'MMMM d, yyyy' | Upcase }}"" />
                        {% endif %}
                    </VerticalStackLayout>
                    <Rock:Icon Grid.Column=""2"" IconClass=""chevron-right"" IconFamily=""FontAwesomeSolid"" FontSize=""16"" StyleClass=""text-interface-soft"" VerticalOptions=""Center"" />
                </Grid>
            </Rock:StyledBorder>
        {% endfor %}

        {% sql %}
            SELECT o.Id AS EntityId, o.Guid AS OccurrenceGuid, ei.Name AS Title, o.NextStartDateTime AS SortDate
            FROM Following f
            JOIN PersonAlias pa ON pa.Id = f.PersonAliasId
            JOIN EventItemOccurrence o ON o.Id = f.EntityId
            JOIN EventItem ei ON ei.Id = o.EventItemId
            WHERE pa.PersonId = {{ CurrentPerson.Id }}
              AND f.EntityTypeId = (SELECT Id FROM EntityType WHERE [Name] = 'Rock.Model.EventItemOccurrence')
              AND ISNULL(f.PurposeKey, '') = ''
            ORDER BY o.NextStartDateTime
        {% endsql %}
        {%- assign savedEvents = results -%}

        {% if savedEvents != empty %}
            <Label Text=""EVENTS"" StyleClass=""caption1, font-weight-semi-bold, text-interface-medium"" Margin=""4,16,0,8"" />
            {% for row in savedEvents %}
                <Rock:StyledBorder CornerRadius=""12"" Padding=""16,14"" StyleClass=""bg-interface-softest, my-4"">
                    <Rock:StyledBorder.GestureRecognizers>
                        <TapGestureRecognizer Command=""{Binding PushPage}"" CommandParameter=""a2156601-d477-465e-addf-e745dee935f5?EventOccurrenceGuid={{ row.OccurrenceGuid }}"" />
                    </Rock:StyledBorder.GestureRecognizers>
                    <Grid ColumnDefinitions=""Auto, *, Auto"" ColumnSpacing=""14"">
                        <Rock:Icon Grid.Column=""0"" IconClass=""calendar"" IconFamily=""FontAwesomeSolid"" FontSize=""18"" TextColor=""{Rock:PaletteColor App-Primary-Strong}"" VerticalOptions=""Center"" />
                        <Label Grid.Column=""1"" StyleClass=""body, bold, text-interface-strongest"" Text=""{{ row.Title | Escape }}"" MaxLines=""2"" LineBreakMode=""TailTruncation"" VerticalOptions=""Center"" />
                        <Rock:Icon Grid.Column=""2"" IconClass=""chevron-right"" IconFamily=""FontAwesomeSolid"" FontSize=""16"" StyleClass=""text-interface-soft"" VerticalOptions=""Center"" />
                    </Grid>
                </Rock:StyledBorder>
            {% endfor %}
        {% endif %}

    </VerticalStackLayout>
{% endif %}

" );   // Content
            RockMigrationHelper.AddBlockAttributeValue( "8380B1DB-4D13-47D5-B290-D8D98E6FB4BD", "24516448-3F1F-4F27-97A1-CFB4F8B277B5", @"Sql" );   // EnabledLavaCommands
            RockMigrationHelper.AddBlockAttributeValue( "8380B1DB-4D13-47D5-B290-D8D98E6FB4BD", "B31D29A0-3725-4AEB-8360-7D91B9CDFE47", @"True" );   // DynamicContent
            RockMigrationHelper.AddBlockAttributeValue( "597C9DB9-24E6-4FBF-A97D-A21A32F3B81D", "34EDD658-BE58-4E3F-9FDD-D8C5D575F2EA", @"False" );   // GroupByPerson
            RockMigrationHelper.AddBlockAttributeValue( "597C9DB9-24E6-4FBF-A97D-A21A32F3B81D", "7F3E38F0-41E1-4DA9-BFDC-AEC067E1240B", @"True" );   // ShowUnknownAsGenderFilterOption
            RockMigrationHelper.AddBlockAttributeValue( "FF2703A4-BF7D-4A53-A214-10AD2E850BAA", "5682EDBF-68DA-4B43-A593-6C2B936C2839", @"{%- assign giveAgainPageGuid = '72a04441-a8ff-44df-aa70-7059d7c9b8f8' -%}
{%- assign viewAllPageGuid = 'd77959e1-8a63-49d7-9fa1-a8720214d073' -%}

{%- sql return:'transactionlist' -%}
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
{%- endsql -%}
{%- sql return:'previousyear' -%}
SELECT SUM(Amount) as Amount
FROM FinancialTransactionDetail ftd
JOIN FinancialTransaction ft ON ftd.TransactionId = ft.Id
JOIN FinancialAccount fa ON ftd.AccountId = fa.Id
WHERE ft.AuthorizedPersonAliasId IN (SELECT pa.Id FROM PersonAlias pa JOIN Person p ON p.Id = pa.PersonId WHERE p.GivingId = '{{ CurrentPerson.GivingId }}')
AND fa.IsTaxDeductible = 1
AND DATEPART(YEAR,ft.TransactionDateTime) = DATEPART(YEAR,DATEADD(YEAR,-1,GETDATE()))
{%- endsql -%}

{% assign transactionListSize = transactionlist | Size %}
{% assign previousYearTotal = 0 %}
{% assign currentYearTotal = 0 %}
{%- for total in previousyear %}{% assign previousYearTotal = total.Amount | AsDouble %}{% endfor -%}
{%- for total in currentyear %}{% assign currentYearTotal = total.Amount | AsDouble %}{% endfor -%}
{%- assign loopTransactionDate = '' -%}

<VerticalStackLayout Spacing=""0"" StyleClass=""p-16"">

    <Label Text=""Overview"" StyleClass=""title3, bold, text-interface-strongest"" Margin=""0,0,0,8"" />

    <!-- ===== Year-total overview card ===== -->
    <Rock:StyledBorder CornerRadius=""16"" Padding=""20"" StrokeThickness=""0"" StyleClass=""bg-interface-softest"" HorizontalOptions=""Fill"">
        <Rock:StyledBorder.Shadow><Shadow Brush=""#000000"" Offset=""0,4"" Radius=""12"" Opacity=""0.15"" /></Rock:StyledBorder.Shadow>
        <VerticalStackLayout Spacing=""2"" HorizontalOptions=""Center"">
            <Label Text=""Total giving in {{ 'Now' | Date:'yyyy' }}""
                StyleClass=""footnote, font-weight-semi-bold, text-interface-medium"" HorizontalOptions=""Center"" />
            <Label Text=""{{ currentYearTotal | Default:'0' | FormatAsCurrency }}""
                StyleClass=""title1, bold, m-8"" TextColor=""{Rock:PaletteColor App-Primary-Strong}"" HorizontalOptions=""Center"" />
            <Label Text=""{{ previousYearTotal | Default:'0' | FormatAsCurrency }} in {{ 'Now' | DateAdd:-1,'y' | Date:'yyyy' }}""
                StyleClass=""footnote, text-interface-soft"" HorizontalOptions=""Center"" />
        </VerticalStackLayout>
    </Rock:StyledBorder>

    <!-- ===== Generosity + Give Again ===== -->
    <Grid ColumnDefinitions=""Auto, *, Auto"" ColumnSpacing=""10"" Margin=""0,16,0,4"" VerticalOptions=""Center"">
        <Rock:Icon Grid.Column=""0"" IconClass=""heart"" IconFamily=""FontAwesomeSolid"" FontSize=""18""
            TextColor=""{Rock:PaletteColor App-Primary-Strong}"" VerticalOptions=""Center"" />
        <Label Grid.Column=""1"" Text=""Thank you for your generosity!""
            StyleClass=""body, text-interface-strong"" VerticalOptions=""Center""
            LineBreakMode=""TailTruncation"" />
        <Rock:StyledBorder Grid.Column=""2"" CornerRadius=""8"" Padding=""16,9"" StrokeThickness=""1.5""
            Stroke=""{Rock:PaletteColor App-Primary-Strong}"" BackgroundColor=""Transparent"" VerticalOptions=""Center"">
            <Label Text=""Give Again"" TextColor=""{Rock:PaletteColor App-Primary-Strong}"" StyleClass=""footnote, bold"" />
            <Rock:StyledBorder.GestureRecognizers>
                <TapGestureRecognizer Command=""{Binding PushPage}"" CommandParameter=""{{ giveAgainPageGuid }}"" />
            </Rock:StyledBorder.GestureRecognizers>
        </Rock:StyledBorder>
    </Grid>

{%- if transactionListSize > 0 %}
    <!-- ===== Latest transactions ===== -->
    <Grid ColumnDefinitions=""*, Auto"" Margin=""0,20,0,4"" VerticalOptions=""Center"">
        <Label Grid.Column=""0"" Text=""Latest Transactions""
            StyleClass=""title3, bold, text-interface-strongest"" VerticalOptions=""Center"" />
        <Label Grid.Column=""1"" Text=""View All"" TextColor=""{Rock:PaletteColor App-Primary-Strong}""
            StyleClass=""body, font-weight-semi-bold"" VerticalOptions=""Center"">
            <Label.GestureRecognizers>
                <TapGestureRecognizer Command=""{Binding PushPage}"" CommandParameter=""{{ viewAllPageGuid }}"" />
            </Label.GestureRecognizers>
        </Label>
    </Grid>

    <VerticalStackLayout Spacing=""0"">
    {%- for transaction in transactionlist -%}
        {%- assign txnDate = transaction.TransactionDateTime | Date:'d MMM yyyy' | Upcase -%}
        {% if loopTransactionDate != txnDate %}
            <Label Text=""{{ txnDate }}""
                StyleClass=""caption1, font-weight-semi-bold, text-interface-medium"" Margin=""4,12,0,6"" />
        {% endif %}
        {%- assign loopTransactionDate = txnDate -%}
        <Rock:StyledBorder CornerRadius=""12"" Padding=""14,12"" StrokeThickness=""0"" StyleClass=""bg-interface-softest, mb-8"">
            <Rock:StyledBorder.Shadow><Shadow Brush=""#000000"" Offset=""0,2"" Radius=""8"" Opacity=""0.10"" /></Rock:StyledBorder.Shadow>
            <Grid ColumnDefinitions=""*, Auto"" ColumnSpacing=""12"" VerticalOptions=""Center"">
                <VerticalStackLayout Grid.Column=""0"" Spacing=""2"" VerticalOptions=""Center"">
                    <Label Text=""{{ transaction.Name | Escape }}""
                        StyleClass=""body, bold, text-interface-strongest"" LineBreakMode=""TailTruncation"" />
                    <Label Text=""{{ transaction.AccountName | Escape }}""
                        StyleClass=""footnote, text-interface-medium"" LineBreakMode=""TailTruncation"" />
                </VerticalStackLayout>
                <Label Grid.Column=""1"" Text=""{{ transaction.Amount | FormatAsCurrency }}""
                    StyleClass=""body, bold, text-interface-strongest"" VerticalOptions=""Center"" />
            </Grid>
        </Rock:StyledBorder>
    {%- endfor -%}
    </VerticalStackLayout>
{%- endif -%}

</VerticalStackLayout>
" );   // Content
            RockMigrationHelper.AddBlockAttributeValue( "FF2703A4-BF7D-4A53-A214-10AD2E850BAA", "24516448-3F1F-4F27-97A1-CFB4F8B277B5", @"Sql" );   // EnabledLavaCommands
            RockMigrationHelper.AddBlockAttributeValue( "FF2703A4-BF7D-4A53-A214-10AD2E850BAA", "B31D29A0-3725-4AEB-8360-7D91B9CDFE47", @"True" );   // DynamicContent
            RockMigrationHelper.AddBlockAttributeValue( "05137346-7182-43EC-B7BD-581237869417", "5682EDBF-68DA-4B43-A593-6C2B936C2839", @"    <!-- ===== Custom nav bar (page's Hide Navigation Bar must be ON) ===== -->
    <VerticalStackLayout StyleClass=""bg-interface-softest"" Spacing=""0"">
        <VerticalStackLayout.Behaviors>
            <Rock:SafeAreaPaddingBehavior Edges=""Top"" />
        </VerticalStackLayout.Behaviors>

        <Grid ColumnDefinitions=""56, *, Auto"" ColumnSpacing=""0"" Padding=""16,16"">
            <Rock:Icon Grid.Column=""0""
                IconClass=""arrow-left""
                IconFamily=""MaterialDesignIcons""
                FontSize=""24""
                StyleClass=""text-interface-strongest""
                VerticalOptions=""Center""
                VerticalTextAlignment=""Center""
                HorizontalOptions=""Start"">
                <Rock:Icon.GestureRecognizers>
                    <TapGestureRecognizer Command=""{Binding PopPage}"" />
                </Rock:Icon.GestureRecognizers>
            </Rock:Icon>

            <Label Grid.Column=""1""
                Text=""Profile""
                StyleClass=""title3, font-weight-semi-bold, text-interface-strongest""
                VerticalOptions=""Center""
                VerticalTextAlignment=""Center""
                HorizontalOptions=""Start""
                LineBreakMode=""TailTruncation"" />

            <Rock:Icon Grid.Column=""2""
                IconClass=""cog""
                IconFamily=""MaterialDesignIcons""
                FontSize=""22""
                StyleClass=""text-interface-strongest""
                VerticalOptions=""Center""
                VerticalTextAlignment=""Center""
                HorizontalOptions=""End"">
                <Rock:Icon.GestureRecognizers>
                    <TapGestureRecognizer Command=""{Binding PushPage}""
                        CommandParameter=""5f46d984-6597-4834-9b78-8f009ab1e1e7"" />
                </Rock:Icon.GestureRecognizers>
            </Rock:Icon>
        </Grid>
    </VerticalStackLayout>" );   // Content
            RockMigrationHelper.AddBlockAttributeValue( "05137346-7182-43EC-B7BD-581237869417", "B31D29A0-3725-4AEB-8360-7D91B9CDFE47", @"False" );   // DynamicContent
            RockMigrationHelper.AddBlockAttributeValue( "65350FBF-7EDA-4D89-9778-6020D53B785F", "5682EDBF-68DA-4B43-A593-6C2B936C2839", @"    <!-- ===== Custom nav bar (page's Hide Navigation Bar must be ON) ===== -->
    <VerticalStackLayout StyleClass=""bg-interface-softest"" Spacing=""0"">
        <VerticalStackLayout.Behaviors>
            <Rock:SafeAreaPaddingBehavior Edges=""Top"" />
        </VerticalStackLayout.Behaviors>

        <Grid ColumnDefinitions=""56, *, Auto"" ColumnSpacing=""0"" Padding=""16,16"">
            <Rock:Icon Grid.Column=""0""
                IconClass=""arrow-left""
                IconFamily=""MaterialDesignIcons""
                FontSize=""24""
                StyleClass=""text-interface-strongest""
                VerticalOptions=""Center""
                VerticalTextAlignment=""Center""
                HorizontalOptions=""Start"">
                <Rock:Icon.GestureRecognizers>
                    <TapGestureRecognizer Command=""{Binding PopPage}"" />
                </Rock:Icon.GestureRecognizers>
            </Rock:Icon>

            <Label Grid.Column=""1""
                Text=""Profile""
                StyleClass=""title3, font-weight-semi-bold, text-interface-strongest""
                VerticalOptions=""Center""
                VerticalTextAlignment=""Center""
                HorizontalOptions=""Start""
                LineBreakMode=""TailTruncation"" />

            <Rock:Icon Grid.Column=""2""
                IconClass=""cog""
                IconFamily=""MaterialDesignIcons""
                FontSize=""22""
                StyleClass=""text-interface-strongest""
                VerticalOptions=""Center""
                VerticalTextAlignment=""Center""
                HorizontalOptions=""End"">
                <Rock:Icon.GestureRecognizers>
                    <TapGestureRecognizer Command=""{Binding PushPage}""
                        CommandParameter=""5f46d984-6597-4834-9b78-8f009ab1e1e7"" />
                </Rock:Icon.GestureRecognizers>
            </Rock:Icon>
        </Grid>
    </VerticalStackLayout>" );   // Content
            RockMigrationHelper.AddBlockAttributeValue( "65350FBF-7EDA-4D89-9778-6020D53B785F", "B31D29A0-3725-4AEB-8360-7D91B9CDFE47", @"False" );   // DynamicContent
            RockMigrationHelper.AddBlockAttributeValue( "81E84E8F-4FC6-4D89-997E-8BBF1A7B2E05", "5682EDBF-68DA-4B43-A593-6C2B936C2839", @"    <!-- ===== Custom nav bar (page's Hide Navigation Bar must be ON) ===== -->
    <VerticalStackLayout StyleClass=""bg-interface-softest"" Spacing=""0"">
        <VerticalStackLayout.Behaviors>
            <Rock:SafeAreaPaddingBehavior Edges=""Top"" />
        </VerticalStackLayout.Behaviors>

        <Grid ColumnDefinitions=""56, *, Auto"" ColumnSpacing=""0"" Padding=""16,16"">
            <Rock:Icon Grid.Column=""0""
                IconClass=""arrow-left""
                IconFamily=""MaterialDesignIcons""
                FontSize=""24""
                StyleClass=""text-interface-strongest""
                VerticalOptions=""Center""
                VerticalTextAlignment=""Center""
                HorizontalOptions=""Start"">
                <Rock:Icon.GestureRecognizers>
                    <TapGestureRecognizer Command=""{Binding PopPage}"" />
                </Rock:Icon.GestureRecognizers>
            </Rock:Icon>

            <Label Grid.Column=""1""
                Text=""Profile""
                StyleClass=""title3, font-weight-semi-bold, text-interface-strongest""
                VerticalOptions=""Center""
                VerticalTextAlignment=""Center""
                HorizontalOptions=""Start""
                LineBreakMode=""TailTruncation"" />

            <Rock:Icon Grid.Column=""2""
                IconClass=""cog""
                IconFamily=""MaterialDesignIcons""
                FontSize=""22""
                StyleClass=""text-interface-strongest""
                VerticalOptions=""Center""
                VerticalTextAlignment=""Center""
                HorizontalOptions=""End"">
                <Rock:Icon.GestureRecognizers>
                    <TapGestureRecognizer Command=""{Binding PushPage}""
                        CommandParameter=""5f46d984-6597-4834-9b78-8f009ab1e1e7"" />
                </Rock:Icon.GestureRecognizers>
            </Rock:Icon>
        </Grid>
    </VerticalStackLayout>" );   // Content
            RockMigrationHelper.AddBlockAttributeValue( "81E84E8F-4FC6-4D89-997E-8BBF1A7B2E05", "B31D29A0-3725-4AEB-8360-7D91B9CDFE47", @"False" );   // DynamicContent
            RockMigrationHelper.AddBlockAttributeValue( "BD66F886-936E-4624-AEF5-00698C7C0BFC", "5682EDBF-68DA-4B43-A593-6C2B936C2839", @"    <!-- ===== Custom nav bar (page's Hide Navigation Bar must be ON) ===== -->
    <VerticalStackLayout StyleClass=""bg-interface-softest"" Spacing=""0"">
        <VerticalStackLayout.Behaviors>
            <Rock:SafeAreaPaddingBehavior Edges=""Top"" />
        </VerticalStackLayout.Behaviors>

        <Grid ColumnDefinitions=""56, *, Auto"" ColumnSpacing=""0"" Padding=""16,16"">
            <Rock:Icon Grid.Column=""0""
                IconClass=""arrow-left""
                IconFamily=""MaterialDesignIcons""
                FontSize=""24""
                StyleClass=""text-interface-strongest""
                VerticalOptions=""Center""
                VerticalTextAlignment=""Center""
                HorizontalOptions=""Start"">
                <Rock:Icon.GestureRecognizers>
                    <TapGestureRecognizer Command=""{Binding PopPage}"" />
                </Rock:Icon.GestureRecognizers>
            </Rock:Icon>

            <Label Grid.Column=""1""
                Text=""Profile""
                StyleClass=""title3, font-weight-semi-bold, text-interface-strongest""
                VerticalOptions=""Center""
                VerticalTextAlignment=""Center""
                HorizontalOptions=""Start""
                LineBreakMode=""TailTruncation"" />

            <Rock:Icon Grid.Column=""2""
                IconClass=""cog""
                IconFamily=""MaterialDesignIcons""
                FontSize=""22""
                StyleClass=""text-interface-strongest""
                VerticalOptions=""Center""
                VerticalTextAlignment=""Center""
                HorizontalOptions=""End"">
                <Rock:Icon.GestureRecognizers>
                    <TapGestureRecognizer Command=""{Binding PushPage}""
                        CommandParameter=""5f46d984-6597-4834-9b78-8f009ab1e1e7"" />
                </Rock:Icon.GestureRecognizers>
            </Rock:Icon>
        </Grid>
    </VerticalStackLayout>" );   // Content
            RockMigrationHelper.AddBlockAttributeValue( "BD66F886-936E-4624-AEF5-00698C7C0BFC", "B31D29A0-3725-4AEB-8360-7D91B9CDFE47", @"False" );   // DynamicContent
            RockMigrationHelper.AddBlockAttributeValue( "DDA29EBD-B8A0-44FD-A1A4-2E783F050005", "20C706F6-D690-401B-83A6-9BD41661AAD2", @"39b8b16d-d213-46fd-9b8f-710453806193|" );   // Template
            RockMigrationHelper.AddBlockAttributeValue( "AC2DDABC-C82A-4827-8565-248724D1C324", "5682EDBF-68DA-4B43-A593-6C2B936C2839", @"{%- if PageParameter.ItemId == ""-1"" or PageParameter.ItemId == """" or PageParameter.ItemId == empty -%}

<VerticalStackLayout Spacing=""0"" StyleClass=""p-16"">
    <Label Text=""Sorry, you currently have no push notifications to view.""
        StyleClass=""body, text-interface-medium"" />
</VerticalStackLayout>

{%- else -%}

{%- communication id:'{{ PageParameter.ItemId }}' securityenabled:'false' %}
{%- assign pushData = communication.PushData | FromJSON -%}

<VerticalStackLayout Spacing=""8"" StyleClass=""p-16"">
    <Label Text=""{{ communication.PushTitle | Escape }}""
        StyleClass=""title2, bold, text-interface-strongest"" />
    <Label Text=""{{ communication.SendDateTime | Date:'ddd, MMM d, yyyy, h:mm tt' | Upcase }}""
        StyleClass=""caption1, font-weight-semi-bold, text-interface-medium"" />

    <BoxView HeightRequest=""1"" StyleClass=""bg-interface-softer"" Margin=""0,4"" />

    {%- if communication.PushOpenMessage and communication.PushOpenMessage != '' -%}
    <Rock:Html StyleClass=""body, text-interface-stronger"">
        <![CDATA[
        {{ communication.PushOpenMessage | RunLava }}
        ]]>
    </Rock:Html>
    {%- else -%}
    <Label Text=""{{ communication.PushMessage | RunLava | Escape }}""
        StyleClass=""body, text-interface-stronger"" />
    {%- endif -%}
</VerticalStackLayout>

{%- endcommunication -%}
{%- endif -%}" );   // Content
            RockMigrationHelper.AddBlockAttributeValue( "AC2DDABC-C82A-4827-8565-248724D1C324", "24516448-3F1F-4F27-97A1-CFB4F8B277B5", @"RockEntity" );   // EnabledLavaCommands
            RockMigrationHelper.AddBlockAttributeValue( "AC2DDABC-C82A-4827-8565-248724D1C324", "B31D29A0-3725-4AEB-8360-7D91B9CDFE47", @"True" );   // DynamicContent
            RockMigrationHelper.AddBlockAttributeValue( "10E07AFD-C2EB-4E97-9121-027811648F4B", "5682EDBF-68DA-4B43-A593-6C2B936C2839", @"{%- if CurrentPerson == null -%}
{%- comment -%} Set these to your app's login / register page guids {%- endcomment -%}
{%- assign loginPageGuid = '9bb25932-4d56-417c-911b-dc915167e7bc' -%}
{%- assign registerPageGuid = '' -%}
{%- assign heading = 'Sign in to continue' -%}
{%- assign subtext = 'Sign in to view this page and keep it personal to you.' -%}

<VerticalStackLayout Spacing=""16"" StyleClass=""p-24"" HorizontalOptions=""Fill"" VerticalOptions=""Center"">

    <Rock:StyledBorder WidthRequest=""76"" HeightRequest=""76"" CornerRadius=""38"" HorizontalOptions=""Center"" StyleClass=""bg-interface-softer"">
        <Rock:Icon IconClass=""lock"" IconFamily=""FontAwesomeSolid"" FontSize=""30""
            StyleClass=""text-interface-medium"" HorizontalOptions=""Center"" VerticalOptions=""Center"" />
    </Rock:StyledBorder>

    <Label Text=""{{ heading | Escape }}""
        StyleClass=""title2, bold, text-interface-strongest"" HorizontalTextAlignment=""Center"" />
    <Label Text=""{{ subtext | Escape }}""
        StyleClass=""body, text-interface-medium"" HorizontalTextAlignment=""Center"" />

    <Rock:StyledBorder CornerRadius=""10"" Padding=""0"" BackgroundColor=""{Rock:PaletteColor App-Primary-Strong}"" HorizontalOptions=""Fill"" Margin=""0,8,0,0"">
        <Label Text=""Sign In"" TextColor=""#FFFFFF"" StyleClass=""body, bold""
            HorizontalOptions=""Center"" HorizontalTextAlignment=""Center"" Margin=""0,14"" />
        <Rock:StyledBorder.GestureRecognizers>
            <TapGestureRecognizer Command=""{Binding PushPage}"" CommandParameter=""{{ loginPageGuid }}"" />
        </Rock:StyledBorder.GestureRecognizers>
    </Rock:StyledBorder>

    {% if registerPageGuid != '' %}
    <Label HorizontalOptions=""Center"" HorizontalTextAlignment=""Center"" Margin=""0,4,0,0"">
        <Label.FormattedText>
            <FormattedString>
                <Span Text=""New here?  "" StyleClass=""text-interface-medium"" />
                <Span Text=""Create an account"" TextColor=""{Rock:PaletteColor App-Primary-Strong}"" StyleClass=""bold"" />
            </FormattedString>
        </Label.FormattedText>
        <Label.GestureRecognizers>
            <TapGestureRecognizer Command=""{Binding PushPage}"" CommandParameter=""{{ registerPageGuid }}"" />
        </Label.GestureRecognizers>
    </Label>
    {% endif %}

</VerticalStackLayout>
{%- endif -%}" );   // Content
            RockMigrationHelper.AddBlockAttributeValue( "10E07AFD-C2EB-4E97-9121-027811648F4B", "B31D29A0-3725-4AEB-8360-7D91B9CDFE47", @"True" );   // DynamicContent
            RockMigrationHelper.AddBlockAttributeValue( "77A80F5F-CED1-4471-BCE3-1F405BE29C6B", "5682EDBF-68DA-4B43-A593-6C2B936C2839", @"{%- if CurrentPerson == null -%}
{%- comment -%} Set these to your app's login / register page guids {%- endcomment -%}
{%- assign loginPageGuid = '9bb25932-4d56-417c-911b-dc915167e7bc' -%}
{%- assign registerPageGuid = '' -%}
{%- assign heading = 'Sign in to continue' -%}
{%- assign subtext = 'Sign in to view this page and keep it personal to you.' -%}

<VerticalStackLayout Spacing=""16"" StyleClass=""p-24"" HorizontalOptions=""Fill"" VerticalOptions=""Center"">

    <Rock:StyledBorder WidthRequest=""76"" HeightRequest=""76"" CornerRadius=""38"" HorizontalOptions=""Center"" StyleClass=""bg-interface-softer"">
        <Rock:Icon IconClass=""lock"" IconFamily=""FontAwesomeSolid"" FontSize=""30""
            StyleClass=""text-interface-medium"" HorizontalOptions=""Center"" VerticalOptions=""Center"" />
    </Rock:StyledBorder>

    <Label Text=""{{ heading | Escape }}""
        StyleClass=""title2, bold, text-interface-strongest"" HorizontalTextAlignment=""Center"" />
    <Label Text=""{{ subtext | Escape }}""
        StyleClass=""body, text-interface-medium"" HorizontalTextAlignment=""Center"" />

    <Rock:StyledBorder CornerRadius=""10"" Padding=""0"" BackgroundColor=""{Rock:PaletteColor App-Primary-Strong}"" HorizontalOptions=""Fill"" Margin=""0,8,0,0"">
        <Label Text=""Sign In"" TextColor=""#FFFFFF"" StyleClass=""body, bold""
            HorizontalOptions=""Center"" HorizontalTextAlignment=""Center"" Margin=""0,14"" />
        <Rock:StyledBorder.GestureRecognizers>
            <TapGestureRecognizer Command=""{Binding PushPage}"" CommandParameter=""{{ loginPageGuid }}"" />
        </Rock:StyledBorder.GestureRecognizers>
    </Rock:StyledBorder>

    {% if registerPageGuid != '' %}
    <Label HorizontalOptions=""Center"" HorizontalTextAlignment=""Center"" Margin=""0,4,0,0"">
        <Label.FormattedText>
            <FormattedString>
                <Span Text=""New here?  "" StyleClass=""text-interface-medium"" />
                <Span Text=""Create an account"" TextColor=""{Rock:PaletteColor App-Primary-Strong}"" StyleClass=""bold"" />
            </FormattedString>
        </Label.FormattedText>
        <Label.GestureRecognizers>
            <TapGestureRecognizer Command=""{Binding PushPage}"" CommandParameter=""{{ registerPageGuid }}"" />
        </Label.GestureRecognizers>
    </Label>
    {% endif %}

</VerticalStackLayout>
{%- endif -%}" );   // Content
            RockMigrationHelper.AddBlockAttributeValue( "77A80F5F-CED1-4471-BCE3-1F405BE29C6B", "B31D29A0-3725-4AEB-8360-7D91B9CDFE47", @"True" );   // DynamicContent
            RockMigrationHelper.AddBlockAttributeValue( "4687C5D1-BEAE-48F7-AB47-C5F723369EB6", "D77299F8-37F8-4F3C-8747-A9F1C7C5CEF1", @"766b7660-857e-4c6b-80d1-1a34002d657b" );   // WorkflowType
            RockMigrationHelper.AddBlockAttributeValue( "4687C5D1-BEAE-48F7-AB47-C5F723369EB6", "87BAB537-0EB1-4894-B72B-D70472C802D7", @"0" );   // CompletionAction
            RockMigrationHelper.AddBlockAttributeValue( "3B3985B1-5A10-441C-94DD-C83FA15B5579", "5682EDBF-68DA-4B43-A593-6C2B936C2839", @"{%- assign bannerUrl = 'Global' | Attribute:'PublicApplicationRoot' | Append:'Content/PostServiceSurvey.png' -%}
<VerticalStackLayout Spacing=""0"">
    {% if bannerUrl != '' %}
    <Rock:Image Source=""{{ bannerUrl | Escape }}"" Aspect=""AspectFill"" Ratio=""2:1"" />
    {% endif %}
    <VerticalStackLayout Spacing=""14"" StyleClass=""p-24"">
        <Label Text=""How was church for you?"" StyleClass=""body, bold, text-interface-strongest"" />
        <Label Text=""We care about every moment of your experience. Would you take a moment to share how things went for you this week? Your feedback helps us love and serve people better."" StyleClass=""body, text-interface-stronger"" />
        <BoxView HeightRequest=""1"" StyleClass=""bg-interface-softer"" Margin=""0,8,0,0"" />
        <Rock:StyledBorder CornerRadius=""6"" Padding=""0"" BackgroundColor=""{Rock:PaletteColor App-Primary-Strong}"" HorizontalOptions=""Center"" Margin=""0,8,0,0"">
            <Label Text=""FILL OUT THE SURVEY"" TextColor=""#FFFFFF"" StyleClass=""body, bold"" Margin=""28,15"" HorizontalTextAlignment=""Center"" />
            <Rock:StyledBorder.GestureRecognizers>
                <TapGestureRecognizer Command=""{Binding PushPage}"" CommandParameter=""2A5CE7F5-1A5D-49C6-B06A-730CB6FD8ACE"" />
            </Rock:StyledBorder.GestureRecognizers>
        </Rock:StyledBorder>
    </VerticalStackLayout>
</VerticalStackLayout>" );   // Content
            RockMigrationHelper.AddBlockAttributeValue( "3B3985B1-5A10-441C-94DD-C83FA15B5579", "B31D29A0-3725-4AEB-8360-7D91B9CDFE47", @"True" );   // DynamicContent
            RockMigrationHelper.AddBlockAttributeValue( "4687C5D1-BEAE-48F7-AB47-C5F723369EB6", "370F3617-CE26-4FA8-96CA-26B82E4D4F15", @"0" );   // ScanMode
            RockMigrationHelper.AddBlockAttributeValue( "8830E449-A6B7-42B4-8A24-3E847E750502", "5682EDBF-68DA-4B43-A593-6C2B936C2839", @"{%- assign groupGuid = PageParameter.GroupGuid -%}
{%- assign attendancePageGuid = '2826EB49-8A4D-42F7-AD4F-8538F9A2CB05' -%}
{%- assign groupName = '' -%}
{%- assign groupId = 0 -%}
{% group where:'Guid == ""{{ groupGuid }}""' securityenabled:'false' %}
    {%- for g in groupItems -%}{%- assign groupName = g.Name -%}{%- assign groupId = g.Id -%}{%- endfor -%}
{% endgroup %}

{%- assign isLeader = false -%}
{% if CurrentPerson %}
{% groupmember where:'GroupId == {{ groupId }} && PersonId == {{ CurrentPerson.Id }}' securityenabled:'false' %}
    {%- for gm in groupmemberItems -%}{%- if gm.GroupRole.IsLeader -%}{%- assign isLeader = true -%}{%- endif -%}{%- endfor -%}
{% endgroupmember %}
{% endif %}

{%- comment -%}
    Pending-attendance badge.

    This has to agree with what the Group Attendance Entry block actually offers,
    and that block builds its date picker from the GROUP'S SCHEDULE, not from
    stored AttendanceOccurrence rows. Counting stored rows misses every scheduled
    date a leader has not opened yet - the common case - while counting ad-hoc
    rows the picker never lists.

    So: expand the schedule across the same window the block uses
    (NumberOfDaysBackToAllow = 30, NumberOfDaysForwardToAllow = 0 - keep in sync
    if those block settings change), then subtract the dates already dealt with.
    DatesFromICal's 4th argument sets the window start and it walks a year forward
    from there, which is what makes looking backwards possible at all.
{%- endcomment -%}
{%- assign pending = 0 -%}
{% if isLeader %}
    {%- assign lookbackDays = 30 -%}
    {%- assign negLookback = 0 | Minus:lookbackDays -%}
    {%- assign windowStart = 'Now' | DateAdd:negLookback,'d' -%}
    {%- assign windowStartIso = windowStart | Date:'yyyy-MM-ddTHH:mm:ss' -%}
    {%- assign startKey = windowStart | Date:'yyyyMMdd' | AsInteger -%}
    {%- assign todayKey = 'Now' | Date:'yyyyMMdd' | AsInteger -%}

    {%- comment -%} dates already handled: attendance taken, or flagged ""did not meet"" {%- endcomment -%}
    {%- capture handledKeys -%}
    {% attendanceoccurrence where:'GroupId == {{ groupId }} && OccurrenceDate >= ""{{ windowStartIso }}""' securityenabled:'false' %}
        {%- for ao in attendanceoccurrenceItems -%}
            {%- assign attendeeCount = ao.Attendees | Size -%}
            {%- if ao.DidNotOccur == true or attendeeCount > 0 -%}|{{ ao.OccurrenceDate | Date:'yyyyMMdd' }}|{%- endif -%}
        {%- endfor -%}
    {% endattendanceoccurrence %}
    {%- endcapture -%}

    {%- assign ical = '' -%}
    {% group where:'Id == {{ groupId }}' securityenabled:'false' %}
        {%- for g in groupItems -%}{%- assign ical = g.Schedule.iCalendarContent -%}{%- endfor -%}
    {% endgroup %}

    {% if ical != '' and ical != null %}
        {%- assign schedDates = ical | DatesFromICal:'all','',windowStartIso -%}
        {%- for sd in schedDates -%}
            {%- assign sdKey = sd | Date:'yyyyMMdd' | AsInteger -%}
            {%- if sdKey >= startKey and sdKey <= todayKey -%}
                {%- capture sdLookup -%}|{{ sd | Date:'yyyyMMdd' }}|{%- endcapture -%}
                {%- unless handledKeys contains sdLookup -%}
                    {%- assign pending = pending | Plus:1 -%}
                {%- endunless -%}
            {%- endif -%}
        {%- endfor -%}
    {% endif %}
{% endif %}

<VerticalStackLayout Spacing=""24"" StyleClass=""px-16, pt-16"">

    <Label Text=""{{ groupName | Escape }}"" StyleClass=""title1, text-interface-strongest, bold""
        HorizontalTextAlignment=""Center"" HorizontalOptions=""Fill"" />

    <Grid ColumnDefinitions=""{% if isLeader %}*,*,*,*{% else %}*,*,*{% endif %}"" ColumnSpacing=""10"">

        <VerticalStackLayout Grid.Column=""0"" Spacing=""6"" HorizontalOptions=""Center"">
            <Rock:StyledBorder WidthRequest=""58"" HeightRequest=""58"" CornerRadius=""12"" StyleClass=""bg-interface-softest, border, border-interface-soft"" HorizontalOptions=""Center"">
                <Rock:Icon IconClass=""comments"" IconFamily=""FontAwesomeSolid"" FontSize=""22"" StyleClass=""text-primary-strong"" HorizontalOptions=""Center"" VerticalOptions=""Center"" />
            </Rock:StyledBorder>
            <Label Text=""Messages"" StyleClass=""caption1, text-interface-medium"" HorizontalTextAlignment=""Center"" />
            <VerticalStackLayout.GestureRecognizers><TapGestureRecognizer Command=""{Binding PushPage}"" CommandParameter=""AAB4218E-E5B1-4728-8E52-CB7A19DCB124?GroupGuid={{ groupGuid }}"" /></VerticalStackLayout.GestureRecognizers>
        </VerticalStackLayout>

        <VerticalStackLayout Grid.Column=""1"" Spacing=""6"" HorizontalOptions=""Center"">
            <Rock:StyledBorder WidthRequest=""58"" HeightRequest=""58"" CornerRadius=""12"" StyleClass=""bg-interface-softest, border, border-interface-soft"" HorizontalOptions=""Center"">
                <Rock:Icon IconClass=""calendar-alt"" IconFamily=""FontAwesomeSolid"" FontSize=""22"" StyleClass=""text-primary-strong"" HorizontalOptions=""Center"" VerticalOptions=""Center"" />
            </Rock:StyledBorder>
            <Label Text=""Calendar"" StyleClass=""caption1, text-interface-medium"" HorizontalTextAlignment=""Center"" />
            <VerticalStackLayout.GestureRecognizers><TapGestureRecognizer Command=""{Binding ShowToast}"" CommandParameter=""Calendar — coming soon"" /></VerticalStackLayout.GestureRecognizers>
        </VerticalStackLayout>

        <VerticalStackLayout Grid.Column=""2"" Spacing=""6"" HorizontalOptions=""Center"">
            <Rock:StyledBorder WidthRequest=""58"" HeightRequest=""58"" CornerRadius=""12"" StyleClass=""bg-interface-softest, border, border-interface-soft"" HorizontalOptions=""Center"">
                <Rock:Icon IconClass=""clipboard-list"" IconFamily=""FontAwesomeSolid"" FontSize=""22"" StyleClass=""text-primary-strong"" HorizontalOptions=""Center"" VerticalOptions=""Center"" />
            </Rock:StyledBorder>
            <Label Text=""Needs"" StyleClass=""caption1, text-interface-medium"" HorizontalTextAlignment=""Center"" />
            <VerticalStackLayout.GestureRecognizers><TapGestureRecognizer Command=""{Binding PushPage}"" CommandParameter=""DD64051F-EC4A-4AE0-87A1-CA5392DFEB3F?GroupGuid={{ groupGuid }}"" /></VerticalStackLayout.GestureRecognizers>
        </VerticalStackLayout>

        {% if isLeader %}
        <VerticalStackLayout Grid.Column=""3"" Spacing=""6"" HorizontalOptions=""Center"">
            <Grid WidthRequest=""58"" HeightRequest=""58"" HorizontalOptions=""Center"">
                <Rock:StyledBorder WidthRequest=""58"" HeightRequest=""58"" CornerRadius=""12"" StyleClass=""bg-interface-softest, border, border-interface-soft"">
                    <Rock:Icon IconClass=""user-check"" IconFamily=""FontAwesomeSolid"" FontSize=""22"" StyleClass=""text-primary-strong"" HorizontalOptions=""Center"" VerticalOptions=""Center"" />
                </Rock:StyledBorder>
                {% if pending > 0 %}
                <Rock:StyledBorder BackgroundColor=""{Rock:PaletteColor App-Primary-Strong}"" CornerRadius=""11"" HeightRequest=""22"" MinimumWidthRequest=""22"" Padding=""6,0"" HorizontalOptions=""End"" VerticalOptions=""Start"" Margin=""0,-6,-6,0"">
                    <Label Text=""{{ pending }}"" TextColor=""#FFFFFF"" StyleClass=""caption2, bold"" HorizontalOptions=""Center"" VerticalOptions=""Center"" HorizontalTextAlignment=""Center"" />
                </Rock:StyledBorder>
                {% endif %}
            </Grid>
            <Label Text=""Attendance"" StyleClass=""caption1, text-interface-medium"" HorizontalTextAlignment=""Center"" />
            <VerticalStackLayout.GestureRecognizers><TapGestureRecognizer Command=""{Binding PushPage}"" CommandParameter=""{{ attendancePageGuid }}?GroupGuid={{ groupGuid }}"" /></VerticalStackLayout.GestureRecognizers>
        </VerticalStackLayout>
        {% endif %}

    </Grid>

</VerticalStackLayout>
" );   // Content
            RockMigrationHelper.AddBlockAttributeValue( "8830E449-A6B7-42B4-8A24-3E847E750502", "24516448-3F1F-4F27-97A1-CFB4F8B277B5", @"RockEntity" );   // EnabledLavaCommands
            RockMigrationHelper.AddBlockAttributeValue( "8830E449-A6B7-42B4-8A24-3E847E750502", "B31D29A0-3725-4AEB-8360-7D91B9CDFE47", @"True" );   // DynamicContent
            RockMigrationHelper.AddBlockAttributeValue( "F9B0195A-585F-4706-99E5-29DE2A392666", "5682EDBF-68DA-4B43-A593-6C2B936C2839", @"{%- assign groupGuid = PageParameter.GroupGuid -%}
{%- assign detailPageGuid = 'C317451F-B45F-4C05-9068-A74265B0568C' -%}
{%- assign ntGuid = '5F272031-0C1F-4503-8F71-557D44BB4E19' -%}
{%- assign appRoot = 'Global' | Attribute:'PublicApplicationRoot' -%}
{%- assign groupId = 0 -%}
{% group where:'Guid == ""{{ groupGuid }}""' securityenabled:'false' %}{%- for g in groupItems -%}{%- assign groupId = g.Id -%}{%- endfor -%}{% endgroup %}
{%- assign ntId = 0 -%}
{% notetype where:'Guid == ""{{ ntGuid }}""' securityenabled:'false' %}{%- for nt in notetypeItems -%}{%- assign ntId = nt.Id -%}{%- endfor -%}{% endnotetype %}
{%- assign composePageGuid = '80BD4600-EB25-4405-B798-57AC6590B390' -%}
{%- assign isLeader = false -%}
{% if CurrentPerson %}
{% groupmember where:'GroupId == {{ groupId }} && PersonId == {{ CurrentPerson.Id }}' securityenabled:'false' %}
    {%- for gm in groupmemberItems -%}{%- if gm.GroupRole.IsLeader -%}{%- assign isLeader = true -%}{%- endif -%}{%- endfor -%}
{% endgroupmember %}
{% endif %}

<VerticalStackLayout Spacing=""0"" StyleClass=""p-16"">
{% if isLeader %}
    <Rock:StyledBorder CornerRadius=""10"" Padding=""0"" BackgroundColor=""{Rock:PaletteColor App-Primary-Strong}"" HorizontalOptions=""Fill"" Margin=""0,0,0,12"">
        <Rock:StyledBorder.GestureRecognizers>
            <TapGestureRecognizer Command=""{Binding PushPage}"" CommandParameter=""{{ composePageGuid }}?GroupGuid={{ groupGuid }}&amp;NoteTypeGuid={{ ntGuid }}"" />
        </Rock:StyledBorder.GestureRecognizers>
        <HorizontalStackLayout Spacing=""8"" HorizontalOptions=""Center"" Margin=""0,13"">
            <Rock:Icon IconClass=""pen"" IconFamily=""FontAwesomeSolid"" FontSize=""15"" TextColor=""#FFFFFF"" VerticalOptions=""Center"" />
            <Label Text=""NEW POST"" TextColor=""#FFFFFF"" StyleClass=""body, bold"" VerticalOptions=""Center"" />
        </HorizontalStackLayout>
    </Rock:StyledBorder>
{% endif %}
{% note where:'EntityId == {{ groupId }} && NoteTypeId == {{ ntId }} && ParentNoteId == null' sort:'IsAlert desc,CreatedDateTime desc' securityenabled:'false' %}
    {%- assign msgCount = noteItems | Size -%}
    {% for note in noteItems %}
        {%- assign creatorPersonId = 0 -%}
        {% personalias where:'Id == {{ note.CreatedByPersonAliasId }}' securityenabled:'false' %}{%- for pa in personaliasItems -%}{%- assign creatorPersonId = pa.PersonId -%}{%- endfor -%}{% endpersonalias %}
        {%- assign creatorGuid = '' -%}
        {% person where:'Id == {{ creatorPersonId }}' securityenabled:'false' %}{%- for p in personItems -%}{%- assign creatorGuid = p.Guid -%}{%- endfor -%}{% endperson %}
        {%- assign senderPhoto = appRoot | Append:'GetAvatar.ashx?PersonGuid=' | Append:creatorGuid -%}
        <Rock:StyledBorder StyleClass=""bg-interface-softest, mb-8"" CornerRadius=""12"" Padding=""14,12"">
            <Rock:StyledBorder.GestureRecognizers>
                <TapGestureRecognizer Command=""{Binding PushPage}"" CommandParameter=""{{ detailPageGuid }}?ItemGuid={{ note.Guid }}"" />
            </Rock:StyledBorder.GestureRecognizers>
            <Grid ColumnDefinitions=""Auto, *, Auto"" ColumnSpacing=""12"">
                <Rock:Avatar Grid.Column=""0"" Source=""{{ senderPhoto | Escape }}"" HeightRequest=""44"" WidthRequest=""44"" ShowStroke=""false"" VerticalOptions=""Start"" />
                <VerticalStackLayout Grid.Column=""1"" Spacing=""2"" VerticalOptions=""Center"">
                    <HorizontalStackLayout Spacing=""6"">
                        {% if note.IsAlert %}<Rock:Icon IconClass=""thumbtack"" IconFamily=""FontAwesomeSolid"" FontSize=""12"" StyleClass=""text-primary-strong"" VerticalOptions=""Center"" />{% endif %}
                        <Label StyleClass=""body, bold, text-interface-strongest"" Text=""{{ note.Caption | Escape }}"" MaxLines=""1"" LineBreakMode=""TailTruncation"" />
                    </HorizontalStackLayout>
                    <Label StyleClass=""footnote, text-interface-medium"" Text=""{{ note.Text | StripHtml | Escape }}"" MaxLines=""1"" LineBreakMode=""TailTruncation"" />
                </VerticalStackLayout>
                <Label Grid.Column=""2"" StyleClass=""caption2, text-interface-soft"" Text=""{{ note.CreatedDateTime | Date:'MMM d, yyyy' }}"" VerticalOptions=""Start"" />
            </Grid>
        </Rock:StyledBorder>
    {% endfor %}
    {% if msgCount == 0 %}
        <VerticalStackLayout Spacing=""12"" StyleClass=""p-24"" HorizontalOptions=""Center"">
            <Rock:Icon IconClass=""comments"" IconFamily=""FontAwesomeRegular"" FontSize=""34"" StyleClass=""text-interface-soft"" HorizontalOptions=""Center"" />
            <Label Text=""No messages posted for this group yet."" StyleClass=""body, text-interface-medium"" HorizontalTextAlignment=""Center"" />
        </VerticalStackLayout>
    {% endif %}
{% endnote %}
</VerticalStackLayout>
" );   // Content
            RockMigrationHelper.AddBlockAttributeValue( "F9B0195A-585F-4706-99E5-29DE2A392666", "24516448-3F1F-4F27-97A1-CFB4F8B277B5", @"RockEntity" );   // EnabledLavaCommands
            RockMigrationHelper.AddBlockAttributeValue( "F9B0195A-585F-4706-99E5-29DE2A392666", "B31D29A0-3725-4AEB-8360-7D91B9CDFE47", @"True" );   // DynamicContent
            RockMigrationHelper.AddBlockAttributeValue( "B3FC2629-4E9C-403F-AA52-4CDBBE9AC126", "5682EDBF-68DA-4B43-A593-6C2B936C2839", @"{%- assign noteGuid = PageParameter.ItemGuid -%}
{%- assign appRoot = 'Global' | Attribute:'PublicApplicationRoot' -%}
{% note where:'Guid == ""{{ noteGuid }}""' securityenabled:'false' %}
    {% for note in noteItems %}
        {%- assign creatorPersonId = 0 -%}
        {% personalias where:'Id == {{ note.CreatedByPersonAliasId }}' securityenabled:'false' %}{%- for pa in personaliasItems -%}{%- assign creatorPersonId = pa.PersonId -%}{%- endfor -%}{% endpersonalias %}
        {%- assign creatorGuid = '' -%}{%- assign creatorName = '' -%}
        {% person where:'Id == {{ creatorPersonId }}' securityenabled:'false' %}{%- for p in personItems -%}{%- assign creatorGuid = p.Guid -%}{%- assign creatorName = p.FullName -%}{%- endfor -%}{% endperson %}
        {%- assign senderPhoto = appRoot | Append:'GetAvatar.ashx?PersonGuid=' | Append:creatorGuid -%}

        <VerticalStackLayout Spacing=""0"">

            <!-- Author card -->
            <Rock:StyledBorder StyleClass=""bg-interface-softest, mt-4"" CornerRadius=""0"" Padding=""16,14"">
                <Grid ColumnDefinitions=""Auto, *, Auto"" ColumnSpacing=""12"">
                    <Rock:Avatar Grid.Column=""0"" Source=""{{ senderPhoto | Escape }}"" HeightRequest=""46"" WidthRequest=""46"" ShowStroke=""false"" VerticalOptions=""Center"" />
                    <VerticalStackLayout Grid.Column=""1"" Spacing=""1"" VerticalOptions=""Center"">
                        <Label StyleClass=""body, bold, text-interface-strongest"" Text=""{{ creatorName | Escape }}"" />
                        <Label StyleClass=""footnote, text-interface-soft"" Text=""{{ note.CreatedDateTime | Date:'MMM d, yyyy' }} &#8226; {{ note.CreatedDateTime | Date:'h:mm tt' }}"" />
                    </VerticalStackLayout>
                    {% if note.IsAlert %}
                    <Rock:Icon Grid.Column=""2"" IconClass=""thumbtack"" IconFamily=""FontAwesomeSolid"" FontSize=""15"" StyleClass=""text-primary-strong"" VerticalOptions=""Center"" />
                    {% endif %}
                </Grid>
            </Rock:StyledBorder>

            <BoxView HeightRequest=""1"" StyleClass=""bg-interface-softer"" />

            <!-- Subject + body -->
            <VerticalStackLayout Spacing=""10"" StyleClass=""p-16"">
                <Label Text=""{{ note.Caption | Escape }}"" StyleClass=""title3, bold, text-interface-strongest"" />
                <Rock:Html><![CDATA[{{ note.Text }}]]></Rock:Html>
            </VerticalStackLayout>

        </VerticalStackLayout>
    {% endfor %}
{% endnote %}
" );   // Content
            RockMigrationHelper.AddBlockAttributeValue( "B3FC2629-4E9C-403F-AA52-4CDBBE9AC126", "24516448-3F1F-4F27-97A1-CFB4F8B277B5", @"RockEntity" );   // EnabledLavaCommands
            RockMigrationHelper.AddBlockAttributeValue( "B3FC2629-4E9C-403F-AA52-4CDBBE9AC126", "B31D29A0-3725-4AEB-8360-7D91B9CDFE47", @"True" );   // DynamicContent
            RockMigrationHelper.AddBlockAttributeValue( "12A48122-F5C7-4731-8780-873E0FAADDAC", "5682EDBF-68DA-4B43-A593-6C2B936C2839", @"{%- assign groupGuid = PageParameter.GroupGuid -%}
{%- assign detailPageGuid = 'C317451F-B45F-4C05-9068-A74265B0568C' -%}
{%- assign ntGuid = '8717C4C5-DFF8-41BB-8F94-7277981CB1B6' -%}
{%- assign appRoot = 'Global' | Attribute:'PublicApplicationRoot' -%}
{%- assign groupId = 0 -%}
{% group where:'Guid == ""{{ groupGuid }}""' securityenabled:'false' %}{%- for g in groupItems -%}{%- assign groupId = g.Id -%}{%- endfor -%}{% endgroup %}
{%- assign ntId = 0 -%}
{% notetype where:'Guid == ""{{ ntGuid }}""' securityenabled:'false' %}{%- for nt in notetypeItems -%}{%- assign ntId = nt.Id -%}{%- endfor -%}{% endnotetype %}
{%- assign composePageGuid = '80BD4600-EB25-4405-B798-57AC6590B390' -%}
{%- assign isLeader = false -%}
{% if CurrentPerson %}
{% groupmember where:'GroupId == {{ groupId }} && PersonId == {{ CurrentPerson.Id }}' securityenabled:'false' %}
    {%- for gm in groupmemberItems -%}{%- if gm.GroupRole.IsLeader -%}{%- assign isLeader = true -%}{%- endif -%}{%- endfor -%}
{% endgroupmember %}
{% endif %}

<VerticalStackLayout Spacing=""0"" StyleClass=""p-16"">
{% if isLeader %}
    <Rock:StyledBorder CornerRadius=""10"" Padding=""0"" BackgroundColor=""{Rock:PaletteColor App-Primary-Strong}"" HorizontalOptions=""Fill"" Margin=""0,0,0,12"">
        <Rock:StyledBorder.GestureRecognizers>
            <TapGestureRecognizer Command=""{Binding PushPage}"" CommandParameter=""{{ composePageGuid }}?GroupGuid={{ groupGuid }}&amp;NoteTypeGuid={{ ntGuid }}"" />
        </Rock:StyledBorder.GestureRecognizers>
        <HorizontalStackLayout Spacing=""8"" HorizontalOptions=""Center"" Margin=""0,13"">
            <Rock:Icon IconClass=""pen"" IconFamily=""FontAwesomeSolid"" FontSize=""15"" TextColor=""#FFFFFF"" VerticalOptions=""Center"" />
            <Label Text=""NEW POST"" TextColor=""#FFFFFF"" StyleClass=""body, bold"" VerticalOptions=""Center"" />
        </HorizontalStackLayout>
    </Rock:StyledBorder>
{% endif %}
{% note where:'EntityId == {{ groupId }} && NoteTypeId == {{ ntId }} && ParentNoteId == null' sort:'IsAlert desc,CreatedDateTime desc' securityenabled:'false' %}
    {%- assign msgCount = noteItems | Size -%}
    {% for note in noteItems %}
        {%- assign creatorPersonId = 0 -%}
        {% personalias where:'Id == {{ note.CreatedByPersonAliasId }}' securityenabled:'false' %}{%- for pa in personaliasItems -%}{%- assign creatorPersonId = pa.PersonId -%}{%- endfor -%}{% endpersonalias %}
        {%- assign creatorGuid = '' -%}
        {% person where:'Id == {{ creatorPersonId }}' securityenabled:'false' %}{%- for p in personItems -%}{%- assign creatorGuid = p.Guid -%}{%- endfor -%}{% endperson %}
        {%- assign senderPhoto = appRoot | Append:'GetAvatar.ashx?PersonGuid=' | Append:creatorGuid -%}
        <Rock:StyledBorder StyleClass=""bg-interface-softest, mb-8"" CornerRadius=""12"" Padding=""14,12"">
            <Rock:StyledBorder.GestureRecognizers>
                <TapGestureRecognizer Command=""{Binding PushPage}"" CommandParameter=""{{ detailPageGuid }}?ItemGuid={{ note.Guid }}"" />
            </Rock:StyledBorder.GestureRecognizers>
            <Grid ColumnDefinitions=""Auto, *, Auto"" ColumnSpacing=""12"">
                <Rock:Avatar Grid.Column=""0"" Source=""{{ senderPhoto | Escape }}"" HeightRequest=""44"" WidthRequest=""44"" ShowStroke=""false"" VerticalOptions=""Start"" />
                <VerticalStackLayout Grid.Column=""1"" Spacing=""2"" VerticalOptions=""Center"">
                    <HorizontalStackLayout Spacing=""6"">
                        {% if note.IsAlert %}<Rock:Icon IconClass=""thumbtack"" IconFamily=""FontAwesomeSolid"" FontSize=""12"" StyleClass=""text-primary-strong"" VerticalOptions=""Center"" />{% endif %}
                        <Label StyleClass=""body, bold, text-interface-strongest"" Text=""{{ note.Caption | Escape }}"" MaxLines=""1"" LineBreakMode=""TailTruncation"" />
                    </HorizontalStackLayout>
                    <Label StyleClass=""footnote, text-interface-medium"" Text=""{{ note.Text | StripHtml | Escape }}"" MaxLines=""1"" LineBreakMode=""TailTruncation"" />
                </VerticalStackLayout>
                <Label Grid.Column=""2"" StyleClass=""caption2, text-interface-soft"" Text=""{{ note.CreatedDateTime | Date:'MMM d, yyyy' }}"" VerticalOptions=""Start"" />
            </Grid>
        </Rock:StyledBorder>
    {% endfor %}
    {% if msgCount == 0 %}
        <VerticalStackLayout Spacing=""12"" StyleClass=""p-24"" HorizontalOptions=""Center"">
            <Rock:Icon IconClass=""comments"" IconFamily=""FontAwesomeRegular"" FontSize=""34"" StyleClass=""text-interface-soft"" HorizontalOptions=""Center"" />
            <Label Text=""No needs posted for this group yet."" StyleClass=""body, text-interface-medium"" HorizontalTextAlignment=""Center"" />
        </VerticalStackLayout>
    {% endif %}
{% endnote %}
</VerticalStackLayout>
" );   // Content
            RockMigrationHelper.AddBlockAttributeValue( "12A48122-F5C7-4731-8780-873E0FAADDAC", "24516448-3F1F-4F27-97A1-CFB4F8B277B5", @"RockEntity" );   // EnabledLavaCommands
            RockMigrationHelper.AddBlockAttributeValue( "12A48122-F5C7-4731-8780-873E0FAADDAC", "B31D29A0-3725-4AEB-8360-7D91B9CDFE47", @"True" );   // DynamicContent
            RockMigrationHelper.AddBlockAttributeValue( "43523BEC-BC89-448E-BE74-9DB32CF4BB3B", "D77299F8-37F8-4F3C-8747-A9F1C7C5CEF1", @"d7719f5c-96b3-4994-8a6d-0985ac7521e7" );   // WorkflowType
            RockMigrationHelper.AddBlockAttributeValue( "43523BEC-BC89-448E-BE74-9DB32CF4BB3B", "87BAB537-0EB1-4894-B72B-D70472C802D7", @"1" );   // CompletionAction
            RockMigrationHelper.AddBlockAttributeValue( "43523BEC-BC89-448E-BE74-9DB32CF4BB3B", "46BB8051-EE66-4128-B47E-75130EA855F1", @"{%- assign gGuid = Workflow | Attribute:'GroupGuid','RawValue' -%}
{%- assign ntGuid = Workflow | Attribute:'NoteTypeGuid','RawValue' | Upcase -%}
{%- assign msgNt = '5F272031-0C1F-4503-8F71-557D44BB4E19' -%}
{%- if ntGuid == msgNt -%}
    {%- assign backPage = 'AAB4218E-E5B1-4728-8E52-CB7A19DCB124' -%}
    {%- assign backLabel = 'BACK TO MESSAGES' -%}
{%- else -%}
    {%- assign backPage = 'DD64051F-EC4A-4AE0-87A1-CA5392DFEB3F' -%}
    {%- assign backLabel = 'BACK TO NEEDS' -%}
{%- endif -%}
<VerticalStackLayout Spacing=""18"" StyleClass=""p-24"" VerticalOptions=""Center"">
    <Rock:StyledBorder WidthRequest=""72"" HeightRequest=""72"" CornerRadius=""36"" HorizontalOptions=""Center"" StyleClass=""bg-interface-softer"">
        <Rock:Icon IconClass=""check"" IconFamily=""FontAwesomeSolid"" FontSize=""30"" StyleClass=""text-primary-strong"" HorizontalOptions=""Center"" VerticalOptions=""Center"" />
    </Rock:StyledBorder>
    <Label Text=""Posted!"" StyleClass=""title2, bold, text-interface-strongest"" HorizontalTextAlignment=""Center"" />
    <Label Text=""Your post has been shared with the group."" StyleClass=""body, text-interface-medium"" HorizontalTextAlignment=""Center"" />
    <Rock:StyledBorder CornerRadius=""10"" Padding=""0"" BackgroundColor=""{Rock:PaletteColor App-Primary-Strong}"" HorizontalOptions=""Fill"" Margin=""0,8,0,0"">
        <Label Text=""{{ backLabel }}"" TextColor=""#FFFFFF"" StyleClass=""body, bold"" HorizontalOptions=""Center"" HorizontalTextAlignment=""Center"" Margin=""0,14"" />
        <Rock:StyledBorder.GestureRecognizers>
            <TapGestureRecognizer Command=""{Binding PopPage}"" />
        </Rock:StyledBorder.GestureRecognizers>
    </Rock:StyledBorder>
</VerticalStackLayout>
" );   // CompletionXaml
            RockMigrationHelper.AddBlockAttributeValue( "1C5A37BB-A556-4777-9E33-51A11D4DB8A8", "5682EDBF-68DA-4B43-A593-6C2B936C2839", @"<Rock:BibleBrowser Reference=""{{ PageParameter.Reference | Default:'Genesis 1' }}"">
    <StackLayout>
        <Label Text=""{Binding BibleBrowser.Reference}""
            StyleClass=""text-interface-strongest, title1, bold""
            HorizontalOptions=""Center"">
            <Label.GestureRecognizers>
                <TapGestureRecognizer Command=""{Binding BibleBrowser.ShowPickerCommand}"" />
            </Label.GestureRecognizers>
        </Label>
    
        <Grid ColumnDefinitions=""*,*,*"">
            <Rock:Icon IconClass=""chevron-left""
                StyleClass=""text-interface-stronger""
                IconFamily=""MaterialDesignIcons""
                FontSize=""36""
                Command=""{Binding BibleBrowser.ShowPreviousBookOrChapterCommand}""
                IsVisible=""{Binding BibleBrowser.HasPreviousBookOrChapter}""
                HorizontalOptions=""Start""
                VerticalOptions=""Center""
                Grid.Column=""0"" />
                      
            <Rock:Icon IconClass=""chevron-right""
                StyleClass=""text-interface-stronger""
                IconFamily=""MaterialDesignIcons""
                FontSize=""36""
                Command=""{Binding BibleBrowser.ShowNextBookOrChapterCommand}""
                IsVisible=""{Binding BibleBrowser.HasNextBookOrChapter}""
                HorizontalOptions=""End""
                VerticalOptions=""Center""
                Grid.Column=""2"" />
        </Grid>
        
        <Rock:BibleReader Reading=""{Binding BibleBrowser.Reading}""
            ShowReference=""false""
            Margin=""0,12,0,0"" />
    </StackLayout>
</Rock:BibleBrowser>" );   // Content
            RockMigrationHelper.AddBlockAttributeValue( "1C5A37BB-A556-4777-9E33-51A11D4DB8A8", "B31D29A0-3725-4AEB-8360-7D91B9CDFE47", @"True" );   // DynamicContent
            RockMigrationHelper.AddBlockAttributeValue( "C82E63A0-0D0F-4EA7-A2B8-A53FF93C88F0", "5682EDBF-68DA-4B43-A593-6C2B936C2839", @"{%- assign messageChannelId = 5 -%}
{%- assign detailPageGuid = '129CD3DF-28B5-44B0-B77C-1241041E2B50' -%}
{%- assign appRoot = 'Global' | Attribute:'PublicApplicationRoot' -%}

<VerticalStackLayout Spacing=""0"" StyleClass=""p-16"">
{%- contentchannelitem where:'ContentChannelId == ""{{ messageChannelId }}""' sort:'StartDateTime desc' -%}
    {%- assign shown = 0 -%}
    {% for item in contentchannelitemItems %}
        {%- assign audio = item | Attribute:'AudioLink','RawValue' -%}
        {%- if audio == '' -%}{% continue %}{%- endif -%}
        {%- assign shown = shown | Plus:1 -%}
        {%- assign img = item | Attribute:'Image','RawValue' -%}
        {%- assign speaker = item | Attribute:'Speaker' -%}
        {%- assign dur = item | Attribute:'Duration','RawValue' | AsInteger -%}
        <Rock:StyledBorder CornerRadius=""12"" Padding=""12"" StyleClass=""bg-interface-softest, mb-8"">
            <Rock:StyledBorder.GestureRecognizers>
                <TapGestureRecognizer Command=""{Binding PushPage}"" CommandParameter=""{{ detailPageGuid }}?ContentChannelItemId={{ item.Id }}"" />
            </Rock:StyledBorder.GestureRecognizers>
            <Grid ColumnDefinitions=""Auto, *, Auto"" ColumnSpacing=""12"">
                <Rock:StyledBorder Grid.Column=""0"" WidthRequest=""60"" HeightRequest=""60"" CornerRadius=""8"" Padding=""0"" StyleClass=""bg-interface-softer"">
                    {% if img != '' %}
                        <Rock:Image Source=""{{ img | Escape }}"" Aspect=""AspectFill"" WidthRequest=""60"" HeightRequest=""60"" />
                    {% else %}
                        <Rock:Icon IconClass=""microphone"" IconFamily=""FontAwesomeSolid"" FontSize=""22"" StyleClass=""text-primary-strong"" HorizontalOptions=""Center"" VerticalOptions=""Center"" />
                    {% endif %}
                </Rock:StyledBorder>
                <VerticalStackLayout Grid.Column=""1"" Spacing=""3"" VerticalOptions=""Center"">
                    <Label StyleClass=""body, bold, text-interface-strongest"" Text=""{{ item.Title | Escape }}"" MaxLines=""2"" LineBreakMode=""TailTruncation"" />
                    <Label StyleClass=""caption1, text-interface-medium""
                        Text=""{{ item.StartDateTime | Date:'MMM d, yyyy' }}{% if speaker != '' %} &#8226; {{ speaker | Escape }}{% endif %}{% if dur > 0 %} &#8226; {{ dur }} min{% endif %}"" />
                </VerticalStackLayout>
                <Rock:Icon Grid.Column=""2"" IconClass=""circle-play"" IconFamily=""FontAwesomeSolid"" FontSize=""26"" StyleClass=""text-primary-strong"" VerticalOptions=""Center"" />
            </Grid>
        </Rock:StyledBorder>
    {% endfor %}
    {% if shown == 0 %}
        <VerticalStackLayout Spacing=""12"" StyleClass=""p-24"" HorizontalOptions=""Center"">
            <Rock:Icon IconClass=""microphone-slash"" IconFamily=""FontAwesomeSolid"" FontSize=""34"" StyleClass=""text-interface-soft"" HorizontalOptions=""Center"" />
            <Label Text=""No episodes available yet."" StyleClass=""body, text-interface-medium"" HorizontalTextAlignment=""Center"" />
        </VerticalStackLayout>
    {% endif %}
{%- endcontentchannelitem -%}
</VerticalStackLayout>
" );   // Content
            RockMigrationHelper.AddBlockAttributeValue( "C82E63A0-0D0F-4EA7-A2B8-A53FF93C88F0", "24516448-3F1F-4F27-97A1-CFB4F8B277B5", @"RockEntity" );   // EnabledLavaCommands
            RockMigrationHelper.AddBlockAttributeValue( "C82E63A0-0D0F-4EA7-A2B8-A53FF93C88F0", "B31D29A0-3725-4AEB-8360-7D91B9CDFE47", @"True" );   // DynamicContent
            RockMigrationHelper.AddBlockAttributeValue( "43523BEC-BC89-448E-BE74-9DB32CF4BB3B", "370F3617-CE26-4FA8-96CA-26B82E4D4F15", @"0" );   // ScanMode
            RockMigrationHelper.AddBlockAttributeValue( "B0264469-6A70-46F8-8A13-AC0B3375652A", "5682EDBF-68DA-4B43-A593-6C2B936C2839", @"{%- assign appRoot = 'Global' | Attribute:'PublicApplicationRoot' -%}
{%- assign itemId = PageParameter.ContentChannelItemId -%}
{%- assign appRoot = 'Global' | Attribute:'PublicApplicationRoot' -%}
{%- contentchannelitem id:'{{ itemId }}' -%}
    {%- assign audio = contentchannelitem | Attribute:'AudioLink','RawValue' -%}
    {%- assign speaker = contentchannelitem | Attribute:'Speaker' -%}
    {%- comment -%} artwork: Media File thumbnail -> item image -> parent series image.
        If all three are empty we leave the player surface bare on purpose: an audio-only
        source makes ExoPlayer render the mp3's own embedded ID3 cover art, which is a
        fine last resort (Buzzsprout bakes the podcast logo into every episode). {%- endcomment -%}
    {%- assign art = contentchannelitem | Attribute:'MediaFile','DefaultThumbnailUrl' -%}
    {%- if art == '' or art == null -%}
        {%- assign art = contentchannelitem | Attribute:'Image','RawValue' -%}{%- if art != '' and art != null -%}{%- unless art contains 'http' -%}{%- assign art = appRoot | Append:'GetImage.ashx?Guid=' | Append:art -%}{%- endunless -%}{%- endif -%}
    {%- endif -%}
    {%- if art == '' or art == null -%}
        {%- for p in contentchannelitem.ParentItems limit:1 -%}
            {%- comment -%} seriesGuid ends up holding an absolute image URL, not a guid: SeriesImage (Image field, a guid) wrapped in GetImage, else SeriesImageLink (Text field, already a URL). {%- endcomment -%}
{%- assign seriesGuidGuid = p.ContentChannelItem | Attribute:'SeriesImage','RawValue' -%}
{%- assign seriesGuidLink = p.ContentChannelItem | Attribute:'SeriesImageLink','RawValue' -%}
{%- assign seriesGuid = '' -%}
{%- if seriesGuidGuid != '' and seriesGuidGuid != null -%}{%- assign seriesGuid = seriesGuidGuid -%}
{%- elsif seriesGuidLink != '' and seriesGuidLink != null -%}{%- assign seriesGuid = seriesGuidLink -%}{%- endif -%}{%- if seriesGuid != '' and seriesGuid != null -%}{%- unless seriesGuid contains 'http' -%}{%- assign seriesGuid = appRoot | Append:'GetImage.ashx?Guid=' | Append:seriesGuid -%}{%- endunless -%}{%- endif -%}
            {%- if seriesGuid != '' -%}{%- assign art = seriesGuid -%}{%- endif -%}
        {%- endfor -%}
    {%- endif -%}

<VerticalStackLayout Spacing=""0"" StyleClass=""pb-16"">

    <!-- ===== Player: artwork + single play/pause toggle driven by CurrentState ===== -->
    <Rock:MediaPlayer x:Name=""episodePlayer""
        Source=""{{ audio | Escape }}""
        Title=""{{ contentchannelitem.Title | Escape }}""
        Subtitle=""{% if speaker != '' %}{{ speaker | Escape }}{% endif %}""
        ShowThumbnail=""false""
        IsCastEnabled=""true""
        MeasureWithAspectRatio=""false""
        HeightRequest=""300"">
        <Rock:MediaPlayer.OverlayContent>
            <Grid InputTransparent=""False"">
                {%- comment -%} When we have artwork, an opaque base sits under it so nothing
                    shows through. With no artwork we draw nothing and let the player surface
                    show the mp3's own embedded ID3 cover art as the final fallback. {%- endcomment -%}
                {% if art != '' and art != null %}
                <Rock:StyledBorder StrokeThickness=""0"" StyleClass=""bg-interface-softest""
                    HorizontalOptions=""Fill"" VerticalOptions=""Fill"" />
                <Rock:Image Source=""{{ art | Escape }}"" Aspect=""AspectFill"" HorizontalOptions=""Fill"" VerticalOptions=""Fill"" />
                {% endif %}

                <!-- PLAY: shown unless playing -->
                <Rock:StyledBorder WidthRequest=""70"" HeightRequest=""70"" CornerRadius=""35"" Padding=""0""
                    HorizontalOptions=""Center"" VerticalOptions=""Center"" InputTransparent=""False""
                    StrokeThickness=""3"" Stroke=""#FFFFFF""
                    BackgroundColor=""{Rock:PaletteColor App-Primary-Strong}"">
                    <Rock:StyledBorder.Shadow><Shadow Brush=""#000000"" Offset=""0,2"" Radius=""10"" Opacity=""0.45"" /></Rock:StyledBorder.Shadow>
                    <Rock:Icon IconClass=""play"" IconFamily=""FontAwesomeSolid"" FontSize=""28""
                        TextColor=""#FFFFFF"" HorizontalOptions=""Center"" VerticalOptions=""Center"" />
                    <Rock:StyledBorder.GestureRecognizers>
                        <TapGestureRecognizer Command=""{Binding PlayCommand}"" />
                    </Rock:StyledBorder.GestureRecognizers>
                    <Rock:StyledBorder.Triggers>
                        <DataTrigger TargetType=""Rock:StyledBorder""
                            Binding=""{Binding Source={x:Reference episodePlayer}, Path=CurrentState}"" Value=""Playing"">
                            <Setter Property=""IsVisible"" Value=""False"" />
                        </DataTrigger>
                    </Rock:StyledBorder.Triggers>
                </Rock:StyledBorder>

                <!-- PAUSE: shown only while playing -->
                <Rock:StyledBorder WidthRequest=""70"" HeightRequest=""70"" CornerRadius=""35"" Padding=""0""
                    HorizontalOptions=""Center"" VerticalOptions=""Center"" IsVisible=""False"" InputTransparent=""False""
                    StrokeThickness=""3"" Stroke=""#FFFFFF""
                    BackgroundColor=""{Rock:PaletteColor App-Primary-Strong}"">
                    <Rock:StyledBorder.Shadow><Shadow Brush=""#000000"" Offset=""0,2"" Radius=""10"" Opacity=""0.45"" /></Rock:StyledBorder.Shadow>
                    <Rock:Icon IconClass=""pause"" IconFamily=""FontAwesomeSolid"" FontSize=""28""
                        TextColor=""#FFFFFF"" HorizontalOptions=""Center"" VerticalOptions=""Center"" />
                    <Rock:StyledBorder.GestureRecognizers>
                        <TapGestureRecognizer Command=""{Binding PauseCommand}"" />
                    </Rock:StyledBorder.GestureRecognizers>
                    <Rock:StyledBorder.Triggers>
                        <DataTrigger TargetType=""Rock:StyledBorder""
                            Binding=""{Binding Source={x:Reference episodePlayer}, Path=CurrentState}"" Value=""Playing"">
                            <Setter Property=""IsVisible"" Value=""True"" />
                        </DataTrigger>
                    </Rock:StyledBorder.Triggers>
                </Rock:StyledBorder>

            </Grid>
        </Rock:MediaPlayer.OverlayContent>
    </Rock:MediaPlayer>

    <!-- ===== Title / meta ===== -->
    <VerticalStackLayout Spacing=""4"" StyleClass=""px-16, pt-16"">
        <Label Text=""{{ contentchannelitem.Title | Escape }}"" StyleClass=""title3, bold, text-interface-strongest"" />
        <Label StyleClass=""caption1, text-interface-medium""
            Text=""{{ contentchannelitem.StartDateTime | Date:'MMMM d, yyyy' }}{% if speaker != '' %} &#8226; {{ speaker | Escape }}{% endif %}"" />
    </VerticalStackLayout>

    <!-- ===== Actions: Save (follow) | Download mp3 ===== -->
    <Grid ColumnDefinitions=""*, *"" ColumnSpacing=""8"" StyleClass=""px-16, pt-16"">

        <VerticalStackLayout Grid.Column=""0"" Spacing=""6"" HorizontalOptions=""Center"">
            <Rock:FollowingIcon
                EntityTypeId=""{{ contentchannelitem.TypeId }}""
                EntityId=""{{ contentchannelitem.Id }}""
                IsFollowed=""{{ contentchannelitem | IsFollowed }}""
                FontSize=""22""
                HorizontalOptions=""Center""
                FollowingIconClass=""heart""
                FollowingIconFamily=""FontAwesomeSolid""
                FollowingIconColor=""{Rock:PaletteColor App-Primary-Strong}""
                NotFollowingIconClass=""heart""
                NotFollowingIconFamily=""FontAwesomeRegular""
                NotFollowingIconColor=""{AppThemeBinding Light=#3F3F46, Dark=#E4E4E7}""
                NotLoggedInText=""Sign in to save this episode to your list."" />
            <Label StyleClass=""caption1, font-weight-semi-bold, text-interface-medium""
                Text=""SAVE"" HorizontalOptions=""Center"" />
        </VerticalStackLayout>

        <VerticalStackLayout Grid.Column=""1"" Spacing=""6"" HorizontalOptions=""Center"">
            <Rock:Icon IconClass=""download"" IconFamily=""FontAwesomeSolid"" FontSize=""22""
                StyleClass=""text-interface-stronger"" HorizontalOptions=""Center"" />
            <Label StyleClass=""caption1, font-weight-semi-bold, text-interface-medium""
                Text=""DOWNLOAD"" HorizontalOptions=""Center"" />
            <VerticalStackLayout.GestureRecognizers>
                <TapGestureRecognizer Command=""{Binding OpenExternalBrowser}"" CommandParameter=""{{ audio | Escape }}"" />
            </VerticalStackLayout.GestureRecognizers>
        </VerticalStackLayout>

    </Grid>

    <!-- ===== Description ===== -->
    <VerticalStackLayout Spacing=""10"" StyleClass=""px-16, pt-12"">
        <BoxView HeightRequest=""1"" StyleClass=""bg-interface-softer"" />
        <Rock:Html><![CDATA[{{ contentchannelitem.Content }}]]></Rock:Html>
    </VerticalStackLayout>

</VerticalStackLayout>
{%- endcontentchannelitem -%}" );   // Content
            RockMigrationHelper.AddBlockAttributeValue( "B0264469-6A70-46F8-8A13-AC0B3375652A", "24516448-3F1F-4F27-97A1-CFB4F8B277B5", @"RockEntity" );   // EnabledLavaCommands
            RockMigrationHelper.AddBlockAttributeValue( "B0264469-6A70-46F8-8A13-AC0B3375652A", "B31D29A0-3725-4AEB-8360-7D91B9CDFE47", @"True" );   // DynamicContent
            RockMigrationHelper.AddBlockAttributeValue( "258628D3-224E-4CFA-856B-B9C3A8E097BD", "D80CF7C7-F6F4-4E77-97A8-B0842E4AF7FB", @"{%- assign loginPageGuid = '9bb25932-4d56-417c-911b-dc915167e7bc' -%}
<VerticalStackLayout>
    <VerticalStackLayout StyleClass=""bg-interface-softest"" Spacing=""0"">
        <VerticalStackLayout.Behaviors>
            <Rock:SafeAreaPaddingBehavior Edges=""Top"" />
        </VerticalStackLayout.Behaviors>

        <Grid ColumnDefinitions=""56, *"" ColumnSpacing=""0"" Padding=""16,16"">
            <Rock:Icon Grid.Column=""0""
                IconClass=""arrow-left""
                IconFamily=""MaterialDesignIcons""
                FontSize=""24""
                StyleClass=""text-interface-strongest""
                VerticalOptions=""Center""
                VerticalTextAlignment=""Center""
                HorizontalOptions=""Start"">
                <Rock:Icon.GestureRecognizers>
                    <TapGestureRecognizer Command=""{Binding PopPage}"" />
                </Rock:Icon.GestureRecognizers>
            </Rock:Icon>

            <Label Grid.Column=""1""
                StyleClass=""title3, font-weight-semi-bold, text-interface-strongest""
                Text=""{{ Item.Title | Escape }}""
                VerticalOptions=""Center""
                VerticalTextAlignment=""Center""
                LineBreakMode=""TailTruncation"" />
        </Grid>
    </VerticalStackLayout>

    <VerticalStackLayout>
      {%- assign image = Item | Attribute:'Image','Url' -%}
      {%- assign imageUrl = Item | Attribute:'ImageUrl' -%}
      {%- if imageUrl != '' or image != '' -%}
      <Rock:Image Source=""{% if imageUrl != '' %}{{ imageUrl | Escape }}{% else %}{{ image | Escape }}{% endif %}"" Aspect=""AspectFill"" Ratio=""16:9"" />
      {%- endif -%}
      <VerticalStackLayout StyleClass=""p-16"" Spacing=""8"">
        {%- assign tagline = Item | Attribute:'Tagline' -%}
        {%- if tagline != '' -%}<Label Text=""{{ tagline | Escape }}"" StyleClass=""caption1,font-weight-semi-bold,text-interface-strong"" />{%- endif -%}
        <Label Text=""{{ Item.Title | Escape }}"" StyleClass=""title1,text-interface-strongest"" />
        {%- assign speaker = Item | Attribute:'Speaker' -%}
        {%- capture meta -%}{{ Item.StartDateTime | Date:'MMMM d, yyyy' }}{% if speaker != '' %} | {{ speaker | Escape }}{% endif %}{%- endcapture -%}
        <Label Text=""{{ meta | Trim }}"" StyleClass=""footnote,text-interface-medium,mb-8"" />
        {% if CurrentPerson == null %}
        <Rock:Html Text=""{{ Item.Content | Escape }}"" StyleClass=""text-interface-strong"" />
        <VerticalStackLayout StyleClass=""px-16, pt-8"">
            <Rock:NotificationBox NotificationType=""Dark""
                HeaderText=""Sign in to take notes""
                Text=""Log in to add your own notes to this message and pick up where you left off."" StyleClass=""bg-interface-softest"">
                <Rock:StyledBorder.GestureRecognizers>
                    <TapGestureRecognizer Command=""{Binding PushPage}"" CommandParameter=""{{ loginPageGuid }}"" />
                </Rock:StyledBorder.GestureRecognizers>
            </Rock:NotificationBox>
        </VerticalStackLayout>
        {% endif %}
      </VerticalStackLayout>
    </VerticalStackLayout>
</VerticalStackLayout>" );   // ContentTemplate
            RockMigrationHelper.AddBlockAttributeValue( "258628D3-224E-4CFA-856B-B9C3A8E097BD", "616351D9-41FD-4E84-9378-78140BE30605", @"False" );   // LogInteractions
            RockMigrationHelper.AddBlockAttributeValue( "500D9BFF-47F3-4AEA-9601-6450337C9CE2", "5682EDBF-68DA-4B43-A593-6C2B936C2839", @"{%- assign itemGuid = PageParameter.ItemGuid -%}
{%- assign biblePageGuid = '6419FA93-A317-47FC-9C8B-A4265F7BC7EF' -%}
{%- contentchannelitem where:'Guid == ""{{ itemGuid }}""' -%}
    {%- assign refs = contentchannelitem | Attribute:'ScriptureReferences','RawValue' -%}
    {%- if refs != '' -%}
        {%- assign refList = refs | Split:'|' -%}
        <VerticalStackLayout Spacing=""8"" StyleClass=""px-16, pt-8, pb-4"">
            <Label Text=""SCRIPTURE IN THIS MESSAGE"" StyleClass=""caption1, font-weight-semi-bold, text-interface-medium"" />
            <ScrollView Orientation=""Horizontal"" HorizontalScrollBarVisibility=""Never"">
                <HorizontalStackLayout Spacing=""8"">
                {%- for r in refList -%}
                    {%- assign label = r | Trim -%}
                    {%- if label != '' -%}
                        {%- comment -%} BibleBrowser takes book + chapter only, so drop any :verse {%- endcomment -%}
                        {%- assign navRef = label | Split:':' | First | Trim -%}
                        <Rock:StyledBorder CornerRadius=""16"" Padding=""14,8"" StrokeThickness=""1.5""
                            Stroke=""{Rock:PaletteColor App-Primary-Strong}"" BackgroundColor=""Transparent"">
                            <Rock:StyledBorder.GestureRecognizers>
                                <TapGestureRecognizer Command=""{Binding PushPage}"" CommandParameter=""{{ biblePageGuid }}?Reference={{ navRef | UrlEncode }}"" />
                            </Rock:StyledBorder.GestureRecognizers>
                            <HorizontalStackLayout Spacing=""6"">
                                <Rock:Icon IconClass=""book-bible"" IconFamily=""FontAwesomeSolid"" FontSize=""12""
                                    TextColor=""{Rock:PaletteColor App-Primary-Strong}"" VerticalOptions=""Center"" />
                                <Label Text=""{{ label | Escape }}"" TextColor=""{Rock:PaletteColor App-Primary-Strong}""
                                    StyleClass=""footnote, bold"" VerticalOptions=""Center"" />
                            </HorizontalStackLayout>
                        </Rock:StyledBorder>
                    {%- endif -%}
                {%- endfor -%}
                </HorizontalStackLayout>
            </ScrollView>
        </VerticalStackLayout>
    {%- else -%}
        {%- comment -%} a Content block that renders NOTHING shows ""Invalid configuration data"" - emit a no-op {%- endcomment -%}
        <BoxView HeightRequest=""0"" Color=""Transparent"" />
    {%- endif -%}
{%- endcontentchannelitem -%}

" );   // Content
            RockMigrationHelper.AddBlockAttributeValue( "500D9BFF-47F3-4AEA-9601-6450337C9CE2", "24516448-3F1F-4F27-97A1-CFB4F8B277B5", @"RockEntity" );   // EnabledLavaCommands
            RockMigrationHelper.AddBlockAttributeValue( "500D9BFF-47F3-4AEA-9601-6450337C9CE2", "B31D29A0-3725-4AEB-8360-7D91B9CDFE47", @"True" );   // DynamicContent
            RockMigrationHelper.AddBlockAttributeValue( "633A561C-B5A3-44A6-97E5-5A422CCD1AA8", "D80CF7C7-F6F4-4E77-97A8-B0842E4AF7FB", @"{%- assign appRoot = 'Global' | Attribute:'PublicApplicationRoot' -%}
{%- assign videoLink = Item | Attribute:'VideoLink','RawValue' -%}
{%- assign videoEmbed = Item | Attribute:'VideoEmbed','RawValue' -%}
{%- assign speaker = Item | Attribute:'Speaker' -%}
{%- assign img = Item | Attribute:'Image','RawValue' -%}{%- if img != '' and img != null -%}{%- unless img contains 'http' -%}{%- assign img = appRoot | Append:'GetImage.ashx?Guid=' | Append:img -%}{%- endunless -%}{%- endif -%}
{%- assign appRoot = 'Global' | Attribute:'PublicApplicationRoot' -%}
{%- assign sermonPageGuid = '4079b24c-d548-4cd0-a833-c5688bbef052' -%}
{%- if videoLink != '' -%}
    {%- assign shareUri = videoLink -%}
{%- else -%}
    {%- capture shareUri -%}{{ appRoot }}mediaplayer/{{ Item.Id }}{%- endcapture -%}
{%- endif -%}

{%- assign siblings = '' -%}
{%- assign seriesGuid = '' -%}
{%- for p in Item.ParentItems limit:1 -%}
    {%- assign parentSeries = p.ContentChannelItem -%}
    {%- comment -%} seriesGuid ends up holding an absolute image URL, not a guid: SeriesImage (Image field, a guid) wrapped in GetImage, else SeriesImageLink (Text field, already a URL). {%- endcomment -%}
{%- assign seriesGuidGuid = parentSeries | Attribute:'SeriesImage','RawValue' -%}
{%- assign seriesGuidLink = parentSeries | Attribute:'SeriesImageLink','RawValue' -%}
{%- assign seriesGuid = '' -%}
{%- if seriesGuidGuid != '' and seriesGuidGuid != null -%}{%- assign seriesGuid = seriesGuidGuid -%}
{%- elsif seriesGuidLink != '' and seriesGuidLink != null -%}{%- assign seriesGuid = seriesGuidLink -%}{%- endif -%}{%- if seriesGuid != '' and seriesGuid != null -%}{%- unless seriesGuid contains 'http' -%}{%- assign seriesGuid = appRoot | Append:'GetImage.ashx?Guid=' | Append:seriesGuid -%}{%- endunless -%}{%- endif -%}
    {%- assign siblings = parentSeries.ChildItems | Select:'ChildContentChannelItem' | Sort:'StartDateTime' -%}
{%- endfor -%}
{%- assign siblingCount = siblings | Size -%}

<VerticalStackLayout Spacing=""0"">
    {% if videoEmbed != '' or videoLink != '' %}
        <Rock:RatioView Ratio=""16:9"" BackgroundColor=""#000000"">
            <Rock:WebView Source=""{{ appRoot }}mediaplayer/{{ Item.Id }}"" />
        </Rock:RatioView>
    {% elsif img != '' %}
        <Rock:Image Source=""{{ img | Escape }}"" Aspect=""AspectFill"" Ratio=""16:9"" />
    {% endif %}

    <VerticalStackLayout Spacing=""6"" StyleClass=""p-16"">
        <Label StyleClass=""title2, bold, text-interface-strongest"" Text=""{{ Item.Title | Escape }}"" />
        <Label StyleClass=""caption1, font-weight-semi-bold, text-interface-medium"" Text=""{{ Item.StartDateTime | Date:'MMMM d, yyyy' | Upcase }}{% if speaker != '' %} - {{ speaker | Upcase | Escape }}{% endif %}"" />
    </VerticalStackLayout>

    <!-- ===== Action row: SAVE | SHARE ===== -->
    <Grid ColumnDefinitions=""*, *"" ColumnSpacing=""8"" StyleClass=""px-16"">

        <VerticalStackLayout Grid.Column=""0"" Spacing=""6"" HorizontalOptions=""Center"">
            <Rock:FollowingIcon
                EntityTypeId=""{{ Item.TypeId }}""
                EntityId=""{{ Item.Id }}""
                IsFollowed=""{{ Item | IsFollowed }}""
                FontSize=""22""
                HorizontalOptions=""Center""
                FollowingIconClass=""heart""
                FollowingIconFamily=""FontAwesomeSolid""
                FollowingIconColor=""{Rock:PaletteColor App-Primary-Strong}""
                NotFollowingIconClass=""heart""
                NotFollowingIconFamily=""FontAwesomeRegular""
                NotFollowingIconColor=""{AppThemeBinding Light=#3F3F46, Dark=#E4E4E7}""
                NotLoggedInText=""Sign in to save this message to your list."" />
            <Label StyleClass=""caption1, font-weight-semi-bold, text-interface-medium""
                Text=""SAVE"" HorizontalOptions=""Center"" />
        </VerticalStackLayout>

        <VerticalStackLayout Grid.Column=""1"" Spacing=""6"" HorizontalOptions=""Center"">
            <Rock:Icon IconClass=""share-square"" IconFamily=""FontAwesomeSolid"" FontSize=""22""
                StyleClass=""text-interface-stronger"" HorizontalOptions=""Center"" />
            <Label StyleClass=""caption1, font-weight-semi-bold, text-interface-medium""
                Text=""SHARE"" HorizontalOptions=""Center"" />
            <VerticalStackLayout.GestureRecognizers>
                <TapGestureRecognizer Command=""{Binding ShareContent}"">
                    <TapGestureRecognizer.CommandParameter>
                        <Rock:ShareContentParameters
                            Title=""{{ Item.Title | Escape }}""
                            Text=""{{ Item.Title | Escape }}{% if speaker != '' %} - {{ speaker | Escape }}{% endif %}""
                            Uri=""{{ shareUri | Escape }}"" />
                    </TapGestureRecognizer.CommandParameter>
                </TapGestureRecognizer>
            </VerticalStackLayout.GestureRecognizers>
        </VerticalStackLayout>

    </Grid>

    <BoxView HeightRequest=""1"" StyleClass=""bg-interface-softer, mx-16, my-16"" />

    <Rock:Html Text=""{{ Item.Content | Escape }}"" StyleClass=""body, text-interface-stronger, px-16"" />

    <!-- ===== More in This Series ===== -->
    {% if siblingCount > 1 %}
        <BoxView HeightRequest=""1"" StyleClass=""bg-interface-softer, mx-16, mt-16, mb-16"" />
        <Label Text=""More in This Series"" StyleClass=""title3, bold, text-interface-strongest"" Margin=""16,0,16,12"" />
        <ScrollView Orientation=""Horizontal"" HorizontalScrollBarVisibility=""Never"">
            <HorizontalStackLayout Spacing=""12"" Padding=""16,0"">
                {% for sib in siblings %}
                    {% unless sib.Id == Item.Id %}
                        {%- assign sibImg = sib | Attribute:'Image','RawValue' -%}{%- if sibImg != '' and sibImg != null -%}{%- unless sibImg contains 'http' -%}{%- assign sibImg = appRoot | Append:'GetImage.ashx?Guid=' | Append:sibImg -%}{%- endunless -%}{%- endif -%}
                        {%- if sibImg == '' and seriesGuid != '' -%}{%- assign sibImg = seriesGuid -%}{%- endif -%}
                        <VerticalStackLayout WidthRequest=""240"" Spacing=""8"">
                            <Rock:StyledBorder CornerRadius=""10"" Padding=""0"" WidthRequest=""240"" HeightRequest=""135"" StyleClass=""bg-interface-softer"">{% if sibImg != '' %}<Rock:Image Source=""{{ sibImg | Escape }}"" Aspect=""AspectFill"" WidthRequest=""240"" HeightRequest=""135"" />{% else %}<Rock:Icon IconClass=""video"" IconFamily=""FontAwesomeSolid"" FontSize=""28"" StyleClass=""text-interface-soft"" HorizontalOptions=""Center"" VerticalOptions=""Center"" />{% endif %}</Rock:StyledBorder>
                            <Label StyleClass=""body, bold, text-interface-strongest"" Text=""{{ sib.Title | Escape }}"" MaxLines=""2"" LineBreakMode=""TailTruncation"" />
                            <VerticalStackLayout.GestureRecognizers>
                                <TapGestureRecognizer Command=""{Binding PushPage}"" CommandParameter=""{{ sermonPageGuid }}?ContentChannelItemId={{ sib.Id }}"" />
                            </VerticalStackLayout.GestureRecognizers>
                        </VerticalStackLayout>
                    {% endunless %}
                {% endfor %}
            </HorizontalStackLayout>
        </ScrollView>
    {% endif %}

    <BoxView HeightRequest=""24"" Color=""Transparent"" />
</VerticalStackLayout>" );   // ContentTemplate
            RockMigrationHelper.AddBlockAttributeValue( "633A561C-B5A3-44A6-97E5-5A422CCD1AA8", "49913217-BF13-4270-8023-C56BDA52C790", @"0a63a427-e6b5-2284-45b3-789b293c02ea" );   // ContentChannel
            RockMigrationHelper.AddBlockAttributeValue( "633A561C-B5A3-44A6-97E5-5A422CCD1AA8", "616351D9-41FD-4E84-9378-78140BE30605", @"False" );   // LogInteractions
            RockMigrationHelper.AddBlockAttributeValue( "5D9A02B7-84C1-4E36-A7F8-B0629C4E1D53", "5682EDBF-68DA-4B43-A593-6C2B936C2839", @"{%- comment -%}
    ===========================================================================
    INHERITANCE CAMPAIGN
    ---------------------------------------------------------------------------
    Content is hardcoded for now. Everything you are likely to change lives in
    the CONFIG block directly below - hero image, goal, pledge link and the
    gallery. Copy (objective + FAQs) is further down as plain XAML.

    Gallery images are a comma-separated list, so adding one is a single edit.
    Tapping a thumbnail raises a popup with the full-size image; ShowPopup takes
    a Content view, which is how ""click to enlarge"" is done without a lightbox
    control (the shell has none).
    ===========================================================================
{%- endcomment -%}

{%- comment -%} ============================ CONFIG ============================ {%- endcomment -%}
{%- assign heroImage   = '/Content/MobileApp/InheritanceCampaign.png' -%}
{%- assign goalAmount  = '$500,000' -%}
{%- assign goalLabel   = 'Inheritance Pledge - Phase 2' -%}
{%- assign pledgeText  = 'PLEDGE' -%}
{%- assign pledgeUrl   = 'https://app.securegive.com/nfluencenetwork/goals/the-inheritance-campaign-phase-2' -%}
{%- assign galleryUrls = '/Content/MobileApp/Goal1.png,/Content/MobileApp/Goal2.png,/Content/MobileApp/Goal3.png,/Content/MobileApp/Goal4.png,/Content/MobileApp/Goal5.png' -%}
{%- comment -%} =============================================================== {%- endcomment -%}

{%- comment -%}
    Image paths in CONFIG are app-relative and start with ""/"", while
    PublicApplicationRoot normally ends with ""/"". Trim one so the two do not
    join into ""//"". Guarded on the last character: applying ReplaceLast blindly
    to a root with no trailing slash would corrupt ""https://"" into ""https:/"".
{%- endcomment -%}
{%- assign appRoot = 'Global' | Attribute:'PublicApplicationRoot' -%}
{%- assign appRootLastChar = appRoot | Right:1 -%}
{%- if appRootLastChar == '/' -%}{%- assign appRoot = appRoot | ReplaceLast:'/','' -%}{%- endif -%}
{%- assign galleryList = galleryUrls | Split:',' -%}
{%- assign galleryCount = galleryList | Size -%}

<VerticalStackLayout Spacing=""0"">

    <!-- ===== Hero ===== -->
    {% if heroImage != '' %}
        <Rock:Image Source=""{% unless heroImage contains 'http' %}{{ appRoot }}{% endunless %}{{ heroImage | Escape }}"" Aspect=""AspectFit"" />
    {% else %}
        <Rock:StyledBorder HeightRequest=""150"" StrokeThickness=""0"" StyleClass=""bg-interface-softest"">
            <VerticalStackLayout Spacing=""6"" HorizontalOptions=""Center"" VerticalOptions=""Center"">
                <Rock:Icon IconClass=""church"" IconFamily=""FontAwesomeSolid"" FontSize=""34""
                    StyleClass=""text-interface-soft"" HorizontalOptions=""Center"" />
                <Label Text=""INHERITANCE CAMPAIGN""
                    StyleClass=""title1, font-weight-semi-bold, text-interface-medium""
                    HorizontalOptions=""Center"" />
            </VerticalStackLayout>
        </Rock:StyledBorder>
    {% endif %}

    <VerticalStackLayout Spacing=""0"" StyleClass=""p-16"">

        <Label Text=""Inheritance Campaign""
            StyleClass=""title1, bold, text-interface-strongest"" Margin=""0,4,0,16"" />

        <!-- ===== Goal + pledge ===== -->
        <Rock:StyledBorder CornerRadius=""14"" Padding=""24,20"" StrokeThickness=""0""
            StyleClass=""bg-interface-softest"" HorizontalOptions=""Fill"">
            <Rock:StyledBorder.Shadow>
                <Shadow Brush=""#000000"" Offset=""0,3"" Radius=""12"" Opacity=""0.12"" />
            </Rock:StyledBorder.Shadow>
            <VerticalStackLayout Spacing=""2"" HorizontalOptions=""Center"">
                <Label Text=""{{ goalAmount | Escape }}""
                    StyleClass=""title1, bold, text-interface-strongest, m-8"" HorizontalOptions=""Center"" />
                <Label Text=""{{ goalLabel | Escape }}""
                    StyleClass=""footnote, text-interface-medium"" HorizontalOptions=""Center"" />
            </VerticalStackLayout>
        </Rock:StyledBorder>

        <Button Text=""{{ pledgeText | Escape }}""
            Command=""{Binding OpenBrowser}""
            CommandParameter=""{{ pledgeUrl | Escape }}""
            StyleClass=""btn, btn-primary""
            Margin=""0,16,0,0"" />

        <!-- ===== Objective ===== -->
        <Label Text=""Objective"" StyleClass=""title3, bold, text-interface-strongest"" Margin=""0,28,0,8"" />

        <Rock:Html StyleClass=""body, text-interface-stronger""><![CDATA[
            <p>The purpose of this capital campaign is to raise funding to secure the future of
            The Nfluence Network by <b>acquiring ownership of the building we currently lease</b>.
            This amount will serve as a <b>down payment</b> for <b>purchasing</b> the property,
            ensuring stability and room for growth in our ministry.</p>

            <p>If purchasing the building from our current landlord is not feasible, the funds
            raised will be directed toward exploring alternative options, including building a
            new facility or relocating to a more suitable space. This campaign will ensure that
            The Nfluence Network continues to have a physical home to fulfill its mission of
            influencing the world with the Gospel of Grace.</p>
        ]]></Rock:Html>

    </VerticalStackLayout>

    <!-- ===== Gallery =====
         Square thumbnails: these graphics are timelines and tables, so a square
         crop keeps more of them legible than the old 220x130 landscape tile did.

         The popup uses the default Center anchor. Bottom/Top do set the
         container to Fill, but PopupPage also carries an mx-48 gutter, so the
         enlarged image gained nothing visible and picked up a bottom-sheet
         animation instead - not worth the trade.
    -->
    {% if galleryCount > 0 %}
        <ScrollView Orientation=""Horizontal"" HorizontalScrollBarVisibility=""Never"" Margin=""0,4,0,0"">
            <HorizontalStackLayout Spacing=""10"" Padding=""16,0"">
                {% for raw in galleryList %}
                    {%- assign img = raw | Trim -%}
                    {% if img != '' %}
                        {%- capture imgSrc -%}{% unless img contains 'http' %}{{ appRoot }}{% endunless %}{{ img }}{%- endcapture -%}
                        <Rock:StyledBorder CornerRadius=""10"" Padding=""0"" StrokeThickness=""0""
                            WidthRequest=""130"" HeightRequest=""130"" StyleClass=""bg-interface-softest"">
                            <Rock:Image Source=""{{ imgSrc | Escape }}"" Aspect=""AspectFill""
                                WidthRequest=""130"" HeightRequest=""130"" />
                            <Rock:StyledBorder.GestureRecognizers>
                                <TapGestureRecognizer Command=""{Binding ShowPopup}"">
                                    <TapGestureRecognizer.CommandParameter>
                                        <Rock:ShowPopupParameters Title=""Inheritance Campaign""
                                            ShowHeader=""true"">
                                            <Rock:ShowPopupParameters.Content>
                                                <Rock:Image Source=""{{ imgSrc | Escape }}"" Aspect=""AspectFit""
                                                    HorizontalOptions=""Fill"" />
                                            </Rock:ShowPopupParameters.Content>
                                        </Rock:ShowPopupParameters>
                                    </TapGestureRecognizer.CommandParameter>
                                </TapGestureRecognizer>
                            </Rock:StyledBorder.GestureRecognizers>
                        </Rock:StyledBorder>
                    {% endif %}
                {% endfor %}
            </HorizontalStackLayout>
        </ScrollView>
        <Label Text=""Tap a photo to enlarge""
            StyleClass=""caption2, text-interface-soft"" Margin=""16,8,16,0"" />
    {% endif %}

    <!-- ===== FAQs =====
         Collapsed by default. There is a lot of copy here and leaving it all
         expanded buried the pledge button under several screens of scrolling.
         The chevron rotates via a DataTrigger bound to each Expander's
         IsExpanded through x:Reference, so every expander needs its own x:Name.
    -->
    <VerticalStackLayout Spacing=""0"" StyleClass=""px-16, pb-16"">

        <Label Text=""FAQs"" StyleClass=""title1, bold, text-interface-strongest"" Margin=""0,20,0,4"" />

            <Rock:Expander x:Name=""faq1"">
                <Rock:Expander.Header>
                    <Grid ColumnDefinitions=""*, Auto"" ColumnSpacing=""12"" Padding=""0,16"">
                        <Label Grid.Column=""0"" Text=""1. What&#39;s the purpose behind The Nfluence Network Inheritance Capital Campaign?""
                            StyleClass=""body, bold, text-interface-strongest"" VerticalOptions=""Center"" />
                        <Rock:Icon Grid.Column=""1"" IconClass=""chevron-down"" IconFamily=""FontAwesomeSolid""
                            FontSize=""14"" StyleClass=""text-interface-medium"" VerticalOptions=""Center"">
                            <Rock:Icon.Triggers>
                                <DataTrigger TargetType=""Rock:Icon""
                                    Binding=""{Binding Source={x:Reference faq1}, Path=IsExpanded}"" Value=""True"">
                                    <Setter Property=""Rotation"" Value=""180"" />
                                </DataTrigger>
                            </Rock:Icon.Triggers>
                        </Rock:Icon>
                    </Grid>
                </Rock:Expander.Header>
                <Rock:Html StyleClass=""body, text-interface-stronger"" Margin=""0,0,0,16""><![CDATA[
                    <p>For over 20 years, The Nfluence Network has been more than a ministry&mdash;it has been a home, a family, and a source of hope and transformation for the Michiana region. Together, we&rsquo;ve witnessed lives changed, faith deepened, and a community united in the Gospel of Grace.</p>
                    <p>Today, we possess a rare and urgent opportunity to secure a permanent home for our ministry. This is about far more than walls and a roof. It&rsquo;s about building a place where truth is proclaimed, lives are restored, and the Gospel continues to reach hearts in an ever-changing world.</p>
                    <p>By stepping out in faith now, we can lay a foundation that not only meets our present needs but also positions us for Kingdom impact for generations to come. This campaign is about preserving the heart of who we are, standing firm on biblical truth, and creating space for growth, spiritual healing, and transformation.</p>
                    <p>Now is the time for us to come together as a family, to dream boldly, and to invest in a vision that will shape the future of The Nfluence Network and beyond. Let&rsquo;s make this moment count&mdash;because what we build today will impact eternity.</p>
                ]]></Rock:Html>
            </Rock:Expander>
            <BoxView HeightRequest=""1"" StyleClass=""bg-interface-softer"" />

            <Rock:Expander x:Name=""faq2"">
                <Rock:Expander.Header>
                    <Grid ColumnDefinitions=""*, Auto"" ColumnSpacing=""12"" Padding=""0,16"">
                        <Label Grid.Column=""0"" Text=""2. What will the funds of the Inheritance Campaign be used for?""
                            StyleClass=""body, bold, text-interface-strongest"" VerticalOptions=""Center"" />
                        <Rock:Icon Grid.Column=""1"" IconClass=""chevron-down"" IconFamily=""FontAwesomeSolid""
                            FontSize=""14"" StyleClass=""text-interface-medium"" VerticalOptions=""Center"">
                            <Rock:Icon.Triggers>
                                <DataTrigger TargetType=""Rock:Icon""
                                    Binding=""{Binding Source={x:Reference faq2}, Path=IsExpanded}"" Value=""True"">
                                    <Setter Property=""Rotation"" Value=""180"" />
                                </DataTrigger>
                            </Rock:Icon.Triggers>
                        </Rock:Icon>
                    </Grid>
                </Rock:Expander.Header>
                <Rock:Html StyleClass=""body, text-interface-stronger"" Margin=""0,0,0,16""><![CDATA[
                    <p>The purpose of this capital campaign is to raise a minimum of $500,000 in overall funding to secure the future of The Nfluence Network by acquiring ownership of the building we currently lease. This amount will serve as a down payment for purchasing the property, ensuring stability and room for growth in our ministry.</p>
                ]]></Rock:Html>
            </Rock:Expander>
            <BoxView HeightRequest=""1"" StyleClass=""bg-interface-softer"" />

            <Rock:Expander x:Name=""faq3"">
                <Rock:Expander.Header>
                    <Grid ColumnDefinitions=""*, Auto"" ColumnSpacing=""12"" Padding=""0,16"">
                        <Label Grid.Column=""0"" Text=""3. Can I use my ongoing Sunday morning tithe toward the campaign?""
                            StyleClass=""body, bold, text-interface-strongest"" VerticalOptions=""Center"" />
                        <Rock:Icon Grid.Column=""1"" IconClass=""chevron-down"" IconFamily=""FontAwesomeSolid""
                            FontSize=""14"" StyleClass=""text-interface-medium"" VerticalOptions=""Center"">
                            <Rock:Icon.Triggers>
                                <DataTrigger TargetType=""Rock:Icon""
                                    Binding=""{Binding Source={x:Reference faq3}, Path=IsExpanded}"" Value=""True"">
                                    <Setter Property=""Rotation"" Value=""180"" />
                                </DataTrigger>
                            </Rock:Icon.Triggers>
                        </Rock:Icon>
                    </Grid>
                </Rock:Expander.Header>
                <Rock:Html StyleClass=""body, text-interface-stronger"" Margin=""0,0,0,16""><![CDATA[
                    <p>While your regular Sunday morning tithe is critical for supporting the ongoing ministry of The Nfluence Network&mdash;such as staff salaries, weekly programs, outreach efforts, and operational costs&mdash;redirecting those funds toward the campaign could actually harm the church by creating gaps in our weekly budget.</p>
                    <p>The capital campaign is a special, short term initiative focused specifically on securing a permanent home for our ministry. To ensure the continued strength and stability of our regular operations, we encourage you to prayerfully consider making a gift to the campaign that is above and beyond your regular giving. This way, we can continue to grow and thrive in both our daily ministry and long-term vision.</p>
                ]]></Rock:Html>
            </Rock:Expander>
            <BoxView HeightRequest=""1"" StyleClass=""bg-interface-softer"" />

            <Rock:Expander x:Name=""faq4"">
                <Rock:Expander.Header>
                    <Grid ColumnDefinitions=""*, Auto"" ColumnSpacing=""12"" Padding=""0,16"">
                        <Label Grid.Column=""0"" Text=""4. Are donations tax deductible?""
                            StyleClass=""body, bold, text-interface-strongest"" VerticalOptions=""Center"" />
                        <Rock:Icon Grid.Column=""1"" IconClass=""chevron-down"" IconFamily=""FontAwesomeSolid""
                            FontSize=""14"" StyleClass=""text-interface-medium"" VerticalOptions=""Center"">
                            <Rock:Icon.Triggers>
                                <DataTrigger TargetType=""Rock:Icon""
                                    Binding=""{Binding Source={x:Reference faq4}, Path=IsExpanded}"" Value=""True"">
                                    <Setter Property=""Rotation"" Value=""180"" />
                                </DataTrigger>
                            </Rock:Icon.Triggers>
                        </Rock:Icon>
                    </Grid>
                </Rock:Expander.Header>
                <Rock:Html StyleClass=""body, text-interface-stronger"" Margin=""0,0,0,16""><![CDATA[
                    <p>Yes, all donations to the Nfluence Inheritance Campaign are tax deductible.</p>
                ]]></Rock:Html>
            </Rock:Expander>
            <BoxView HeightRequest=""1"" StyleClass=""bg-interface-softer"" />

            <Rock:Expander x:Name=""faq5"">
                <Rock:Expander.Header>
                    <Grid ColumnDefinitions=""*, Auto"" ColumnSpacing=""12"" Padding=""0,16"">
                        <Label Grid.Column=""0"" Text=""5. How to make a donation?""
                            StyleClass=""body, bold, text-interface-strongest"" VerticalOptions=""Center"" />
                        <Rock:Icon Grid.Column=""1"" IconClass=""chevron-down"" IconFamily=""FontAwesomeSolid""
                            FontSize=""14"" StyleClass=""text-interface-medium"" VerticalOptions=""Center"">
                            <Rock:Icon.Triggers>
                                <DataTrigger TargetType=""Rock:Icon""
                                    Binding=""{Binding Source={x:Reference faq5}, Path=IsExpanded}"" Value=""True"">
                                    <Setter Property=""Rotation"" Value=""180"" />
                                </DataTrigger>
                            </Rock:Icon.Triggers>
                        </Rock:Icon>
                    </Grid>
                </Rock:Expander.Header>
                <VerticalStackLayout Spacing=""12"" Margin=""0,0,0,16"">
                    <Rock:Html StyleClass=""body, text-interface-stronger""><![CDATA[
                    <p><b>Donors can give by:</b> cash, check, credit card, stocks, property, vehicles, boats, crypto, land, jewelry, etc.</p>
                    ]]></Rock:Html>
                    <Button Text=""{{ pledgeText | Escape }}"" Command=""{Binding OpenBrowser}""
                        CommandParameter=""{{ pledgeUrl | Escape }}"" StyleClass=""btn, btn-primary"" />
                </VerticalStackLayout>
            </Rock:Expander>
            <BoxView HeightRequest=""1"" StyleClass=""bg-interface-softer"" />

            <Rock:Expander x:Name=""faq6"">
                <Rock:Expander.Header>
                    <Grid ColumnDefinitions=""*, Auto"" ColumnSpacing=""12"" Padding=""0,16"">
                        <Label Grid.Column=""0"" Text=""6. What is the timeline of the Inheritance Capital Campaign?""
                            StyleClass=""body, bold, text-interface-strongest"" VerticalOptions=""Center"" />
                        <Rock:Icon Grid.Column=""1"" IconClass=""chevron-down"" IconFamily=""FontAwesomeSolid""
                            FontSize=""14"" StyleClass=""text-interface-medium"" VerticalOptions=""Center"">
                            <Rock:Icon.Triggers>
                                <DataTrigger TargetType=""Rock:Icon""
                                    Binding=""{Binding Source={x:Reference faq6}, Path=IsExpanded}"" Value=""True"">
                                    <Setter Property=""Rotation"" Value=""180"" />
                                </DataTrigger>
                            </Rock:Icon.Triggers>
                        </Rock:Icon>
                    </Grid>
                </Rock:Expander.Header>
                <Rock:Html StyleClass=""body, text-interface-stronger"" Margin=""0,0,0,16""><![CDATA[
                    <p><b>January 26th to March 31st</b> &mdash; Public Pledge Collection</p>
                    <p><b>April 1st 2025 to April 1st 2026</b> &mdash; Pledge Collection over 12 months</p>
                    <p><b>May 2026</b> &mdash; Step into Our Inheritance</p>
                ]]></Rock:Html>
            </Rock:Expander>
            <BoxView HeightRequest=""1"" StyleClass=""bg-interface-softer"" />

            <Rock:Expander x:Name=""faq7"">
                <Rock:Expander.Header>
                    <Grid ColumnDefinitions=""*, Auto"" ColumnSpacing=""12"" Padding=""0,16"">
                        <Label Grid.Column=""0"" Text=""7. How will Nfluence sustain operations after the campaign is completed?""
                            StyleClass=""body, bold, text-interface-strongest"" VerticalOptions=""Center"" />
                        <Rock:Icon Grid.Column=""1"" IconClass=""chevron-down"" IconFamily=""FontAwesomeSolid""
                            FontSize=""14"" StyleClass=""text-interface-medium"" VerticalOptions=""Center"">
                            <Rock:Icon.Triggers>
                                <DataTrigger TargetType=""Rock:Icon""
                                    Binding=""{Binding Source={x:Reference faq7}, Path=IsExpanded}"" Value=""True"">
                                    <Setter Property=""Rotation"" Value=""180"" />
                                </DataTrigger>
                            </Rock:Icon.Triggers>
                        </Rock:Icon>
                    </Grid>
                </Rock:Expander.Header>
                <Rock:Html StyleClass=""body, text-interface-stronger"" Margin=""0,0,0,16""><![CDATA[
                    <p>Ensuring the sustainability of the property after the purchase is a top priority, and we&rsquo;ve been carefully considering how to maintain it without compromising the health of our current ministries or operations.</p>
                    <p>Based on our current estimates, we anticipate that an additional 30-50 regular financial partners, contributing at our average donor amount, would be needed to successfully cover the ongoing costs associated with the property. These costs include things like utilities, maintenance, insurance, and any other expenses that come with owning and operating a facility.</p>
                    <p>We believe this is not only achievable but also an incredible opportunity for new and existing partners to join in this exciting vision. As we grow, this new space will enable us to expand our reach, welcome more people into the ministry, and create additional opportunities for engagement, which will naturally invite more individuals to partner with us financially.</p>
                    <p>Furthermore, we are committed to responsible stewardship. This includes exploring cost-saving measures, leveraging volunteer efforts, and ensuring the facility is used efficiently for ministry activities and community outreach. With your partnership, we can confidently move forward, knowing this property will be a blessing for generations to come, not a burden.</p>
                ]]></Rock:Html>
            </Rock:Expander>
            <BoxView HeightRequest=""1"" StyleClass=""bg-interface-softer"" />

            <Rock:Expander x:Name=""faq8"">
                <Rock:Expander.Header>
                    <Grid ColumnDefinitions=""*, Auto"" ColumnSpacing=""12"" Padding=""0,16"">
                        <Label Grid.Column=""0"" Text=""8. What happens to the funds if we are unable to meet the goal?""
                            StyleClass=""body, bold, text-interface-strongest"" VerticalOptions=""Center"" />
                        <Rock:Icon Grid.Column=""1"" IconClass=""chevron-down"" IconFamily=""FontAwesomeSolid""
                            FontSize=""14"" StyleClass=""text-interface-medium"" VerticalOptions=""Center"">
                            <Rock:Icon.Triggers>
                                <DataTrigger TargetType=""Rock:Icon""
                                    Binding=""{Binding Source={x:Reference faq8}, Path=IsExpanded}"" Value=""True"">
                                    <Setter Property=""Rotation"" Value=""180"" />
                                </DataTrigger>
                            </Rock:Icon.Triggers>
                        </Rock:Icon>
                    </Grid>
                </Rock:Expander.Header>
                <Rock:Html StyleClass=""body, text-interface-stronger"" Margin=""0,0,0,16""><![CDATA[
                    <p>If purchasing the building from our current landlord is not a viable option, the funds raised will be redirected toward other opportunities, such as acquiring land or purchasing and renovating an existing building to better meet our needs. Through this campaign, we will ensure that The Nfluence Network continues to have a dedicated physical home to carry out its mission of spreading the Gospel of Grace and transforming lives.</p>
                ]]></Rock:Html>
            </Rock:Expander>

    </VerticalStackLayout>

    <BoxView HeightRequest=""24"" Color=""Transparent"" />
</VerticalStackLayout>
" );   // Content
            RockMigrationHelper.AddBlockAttributeValue( "5D9A02B7-84C1-4E36-A7F8-B0629C4E1D53", "B31D29A0-3725-4AEB-8360-7D91B9CDFE47", @"False" );   // DynamicContent
            RockMigrationHelper.AddBlockAttributeValue( "8FC5061A-927B-4E34-C358-A064D7F1B9E2", "5682EDBF-68DA-4B43-A593-6C2B936C2839", @"{%- comment -%}
    Serving nav strip - sits at the top of all four serving pages.

    Set activeKey per page: schedule | prefs | signup | unavail.
    That one line is the only difference between the four block instances.

    The four mobile schedule blocks carry no page-link settings of their own
    (Toolbox exposes only ToolboxTemplate / ConfirmDeclineTemplate), so they
    cannot cross-link - this strip is what ties them together.

    ReplacePage rather than PushPage: these behave as tabs, so moving between
    them should not stack pages up behind the user. Back from any of the four
    returns to wherever they entered from.
{%- endcomment -%}
{%- assign activeKey = 'prefs' -%}

{%- assign schedulePage = '56302b84-36e3-4e62-9e74-c5739d7de977' -%}
{%- assign prefsPage    = '1e4c7a55-8b92-4d30-a6f1-3c08d5b72e41' -%}
{%- assign signupPage   = '2f7b3c88-4d61-4e29-b0a5-9e13f6c48d72' -%}
{%- assign unavailPage  = '3a6d9e14-7c25-4f83-91b6-5d420ae7c193' -%}

<VerticalStackLayout Spacing=""0"" StyleClass=""px-16, pt-16, pb-8"">

    <Grid ColumnDefinitions=""*, *, *, *"" ColumnSpacing=""8"">

        {%- comment -%} 1. Current Schedule {%- endcomment -%}
        <VerticalStackLayout Grid.Column=""0"" Spacing=""6"" HorizontalOptions=""Center"">
            <Rock:StyledBorder WidthRequest=""58"" HeightRequest=""58"" CornerRadius=""12"" HorizontalOptions=""Center""
                {% if activeKey == 'schedule' %}BackgroundColor=""{Rock:PaletteColor App-Primary-Strong}"" StrokeThickness=""0""{% else %}StyleClass=""bg-interface-softest, border, border-interface-soft""{% endif %}>
                <Rock:Icon IconClass=""calendar-check"" IconFamily=""FontAwesomeSolid"" FontSize=""22""
                    {% if activeKey == 'schedule' %}TextColor=""#FFFFFF""{% else %}StyleClass=""text-primary-strong""{% endif %}
                    HorizontalOptions=""Center"" VerticalOptions=""Center"" />
            </Rock:StyledBorder>
            <Label Text=""Schedule"" HorizontalTextAlignment=""Center""
                StyleClass=""caption1, {% if activeKey == 'schedule' %}font-weight-semi-bold, text-interface-strongest{% else %}text-interface-medium{% endif %}"" />
            {% if activeKey != 'schedule' %}
            <VerticalStackLayout.GestureRecognizers>
                <TapGestureRecognizer Command=""{Binding ReplacePage}"" CommandParameter=""{{ schedulePage }}"" />
            </VerticalStackLayout.GestureRecognizers>
            {% endif %}
        </VerticalStackLayout>

        {%- comment -%} 2. Schedule Preferences {%- endcomment -%}
        <VerticalStackLayout Grid.Column=""1"" Spacing=""6"" HorizontalOptions=""Center"">
            <Rock:StyledBorder WidthRequest=""58"" HeightRequest=""58"" CornerRadius=""12"" HorizontalOptions=""Center""
                {% if activeKey == 'prefs' %}BackgroundColor=""{Rock:PaletteColor App-Primary-Strong}"" StrokeThickness=""0""{% else %}StyleClass=""bg-interface-softest, border, border-interface-soft""{% endif %}>
                <Rock:Icon IconClass=""sliders-h"" IconFamily=""FontAwesomeSolid"" FontSize=""22""
                    {% if activeKey == 'prefs' %}TextColor=""#FFFFFF""{% else %}StyleClass=""text-primary-strong""{% endif %}
                    HorizontalOptions=""Center"" VerticalOptions=""Center"" />
            </Rock:StyledBorder>
            <Label Text=""Preferences"" HorizontalTextAlignment=""Center""
                StyleClass=""caption1, {% if activeKey == 'prefs' %}font-weight-semi-bold, text-interface-strongest{% else %}text-interface-medium{% endif %}"" />
            {% if activeKey != 'prefs' %}
            <VerticalStackLayout.GestureRecognizers>
                <TapGestureRecognizer Command=""{Binding ReplacePage}"" CommandParameter=""{{ prefsPage }}"" />
            </VerticalStackLayout.GestureRecognizers>
            {% endif %}
        </VerticalStackLayout>

        {%- comment -%} 3. Sign Up for Additional Times {%- endcomment -%}
        <VerticalStackLayout Grid.Column=""2"" Spacing=""6"" HorizontalOptions=""Center"">
            <Rock:StyledBorder WidthRequest=""58"" HeightRequest=""58"" CornerRadius=""12"" HorizontalOptions=""Center""
                {% if activeKey == 'signup' %}BackgroundColor=""{Rock:PaletteColor App-Primary-Strong}"" StrokeThickness=""0""{% else %}StyleClass=""bg-interface-softest, border, border-interface-soft""{% endif %}>
                <Rock:Icon IconClass=""calendar-plus"" IconFamily=""FontAwesomeSolid"" FontSize=""22""
                    {% if activeKey == 'signup' %}TextColor=""#FFFFFF""{% else %}StyleClass=""text-primary-strong""{% endif %}
                    HorizontalOptions=""Center"" VerticalOptions=""Center"" />
            </Rock:StyledBorder>
            <Label Text=""Sign Up"" HorizontalTextAlignment=""Center""
                StyleClass=""caption1, {% if activeKey == 'signup' %}font-weight-semi-bold, text-interface-strongest{% else %}text-interface-medium{% endif %}"" />
            {% if activeKey != 'signup' %}
            <VerticalStackLayout.GestureRecognizers>
                <TapGestureRecognizer Command=""{Binding ReplacePage}"" CommandParameter=""{{ signupPage }}"" />
            </VerticalStackLayout.GestureRecognizers>
            {% endif %}
        </VerticalStackLayout>

        {%- comment -%} 4. Schedule Unavailability {%- endcomment -%}
        <VerticalStackLayout Grid.Column=""3"" Spacing=""6"" HorizontalOptions=""Center"">
            <Rock:StyledBorder WidthRequest=""58"" HeightRequest=""58"" CornerRadius=""12"" HorizontalOptions=""Center""
                {% if activeKey == 'unavail' %}BackgroundColor=""{Rock:PaletteColor App-Primary-Strong}"" StrokeThickness=""0""{% else %}StyleClass=""bg-interface-softest, border, border-interface-soft""{% endif %}>
                <Rock:Icon IconClass=""calendar-times"" IconFamily=""FontAwesomeSolid"" FontSize=""22""
                    {% if activeKey == 'unavail' %}TextColor=""#FFFFFF""{% else %}StyleClass=""text-primary-strong""{% endif %}
                    HorizontalOptions=""Center"" VerticalOptions=""Center"" />
            </Rock:StyledBorder>
            <Label Text=""Unavailable"" HorizontalTextAlignment=""Center""
                StyleClass=""caption1, {% if activeKey == 'unavail' %}font-weight-semi-bold, text-interface-strongest{% else %}text-interface-medium{% endif %}"" />
            {% if activeKey != 'unavail' %}
            <VerticalStackLayout.GestureRecognizers>
                <TapGestureRecognizer Command=""{Binding ReplacePage}"" CommandParameter=""{{ unavailPage }}"" />
            </VerticalStackLayout.GestureRecognizers>
            {% endif %}
        </VerticalStackLayout>

    </Grid>

    <BoxView HeightRequest=""1"" StyleClass=""bg-interface-softer"" Margin=""0,14,0,0"" />

</VerticalStackLayout>
" );   // Content
            RockMigrationHelper.AddBlockAttributeValue( "90D6172B-A38C-4F45-D469-B175E8021CA3", "5682EDBF-68DA-4B43-A593-6C2B936C2839", @"{%- comment -%}
    Serving nav strip - sits at the top of all four serving pages.

    Set activeKey per page: schedule | prefs | signup | unavail.
    That one line is the only difference between the four block instances.

    The four mobile schedule blocks carry no page-link settings of their own
    (Toolbox exposes only ToolboxTemplate / ConfirmDeclineTemplate), so they
    cannot cross-link - this strip is what ties them together.

    ReplacePage rather than PushPage: these behave as tabs, so moving between
    them should not stack pages up behind the user. Back from any of the four
    returns to wherever they entered from.
{%- endcomment -%}
{%- assign activeKey = 'signup' -%}

{%- assign schedulePage = '56302b84-36e3-4e62-9e74-c5739d7de977' -%}
{%- assign prefsPage    = '1e4c7a55-8b92-4d30-a6f1-3c08d5b72e41' -%}
{%- assign signupPage   = '2f7b3c88-4d61-4e29-b0a5-9e13f6c48d72' -%}
{%- assign unavailPage  = '3a6d9e14-7c25-4f83-91b6-5d420ae7c193' -%}

<VerticalStackLayout Spacing=""0"" StyleClass=""px-16, pt-16, pb-8"">

    <Grid ColumnDefinitions=""*, *, *, *"" ColumnSpacing=""8"">

        {%- comment -%} 1. Current Schedule {%- endcomment -%}
        <VerticalStackLayout Grid.Column=""0"" Spacing=""6"" HorizontalOptions=""Center"">
            <Rock:StyledBorder WidthRequest=""58"" HeightRequest=""58"" CornerRadius=""12"" HorizontalOptions=""Center""
                {% if activeKey == 'schedule' %}BackgroundColor=""{Rock:PaletteColor App-Primary-Strong}"" StrokeThickness=""0""{% else %}StyleClass=""bg-interface-softest, border, border-interface-soft""{% endif %}>
                <Rock:Icon IconClass=""calendar-check"" IconFamily=""FontAwesomeSolid"" FontSize=""22""
                    {% if activeKey == 'schedule' %}TextColor=""#FFFFFF""{% else %}StyleClass=""text-primary-strong""{% endif %}
                    HorizontalOptions=""Center"" VerticalOptions=""Center"" />
            </Rock:StyledBorder>
            <Label Text=""Schedule"" HorizontalTextAlignment=""Center""
                StyleClass=""caption1, {% if activeKey == 'schedule' %}font-weight-semi-bold, text-interface-strongest{% else %}text-interface-medium{% endif %}"" />
            {% if activeKey != 'schedule' %}
            <VerticalStackLayout.GestureRecognizers>
                <TapGestureRecognizer Command=""{Binding ReplacePage}"" CommandParameter=""{{ schedulePage }}"" />
            </VerticalStackLayout.GestureRecognizers>
            {% endif %}
        </VerticalStackLayout>

        {%- comment -%} 2. Schedule Preferences {%- endcomment -%}
        <VerticalStackLayout Grid.Column=""1"" Spacing=""6"" HorizontalOptions=""Center"">
            <Rock:StyledBorder WidthRequest=""58"" HeightRequest=""58"" CornerRadius=""12"" HorizontalOptions=""Center""
                {% if activeKey == 'prefs' %}BackgroundColor=""{Rock:PaletteColor App-Primary-Strong}"" StrokeThickness=""0""{% else %}StyleClass=""bg-interface-softest, border, border-interface-soft""{% endif %}>
                <Rock:Icon IconClass=""sliders-h"" IconFamily=""FontAwesomeSolid"" FontSize=""22""
                    {% if activeKey == 'prefs' %}TextColor=""#FFFFFF""{% else %}StyleClass=""text-primary-strong""{% endif %}
                    HorizontalOptions=""Center"" VerticalOptions=""Center"" />
            </Rock:StyledBorder>
            <Label Text=""Preferences"" HorizontalTextAlignment=""Center""
                StyleClass=""caption1, {% if activeKey == 'prefs' %}font-weight-semi-bold, text-interface-strongest{% else %}text-interface-medium{% endif %}"" />
            {% if activeKey != 'prefs' %}
            <VerticalStackLayout.GestureRecognizers>
                <TapGestureRecognizer Command=""{Binding ReplacePage}"" CommandParameter=""{{ prefsPage }}"" />
            </VerticalStackLayout.GestureRecognizers>
            {% endif %}
        </VerticalStackLayout>

        {%- comment -%} 3. Sign Up for Additional Times {%- endcomment -%}
        <VerticalStackLayout Grid.Column=""2"" Spacing=""6"" HorizontalOptions=""Center"">
            <Rock:StyledBorder WidthRequest=""58"" HeightRequest=""58"" CornerRadius=""12"" HorizontalOptions=""Center""
                {% if activeKey == 'signup' %}BackgroundColor=""{Rock:PaletteColor App-Primary-Strong}"" StrokeThickness=""0""{% else %}StyleClass=""bg-interface-softest, border, border-interface-soft""{% endif %}>
                <Rock:Icon IconClass=""calendar-plus"" IconFamily=""FontAwesomeSolid"" FontSize=""22""
                    {% if activeKey == 'signup' %}TextColor=""#FFFFFF""{% else %}StyleClass=""text-primary-strong""{% endif %}
                    HorizontalOptions=""Center"" VerticalOptions=""Center"" />
            </Rock:StyledBorder>
            <Label Text=""Sign Up"" HorizontalTextAlignment=""Center""
                StyleClass=""caption1, {% if activeKey == 'signup' %}font-weight-semi-bold, text-interface-strongest{% else %}text-interface-medium{% endif %}"" />
            {% if activeKey != 'signup' %}
            <VerticalStackLayout.GestureRecognizers>
                <TapGestureRecognizer Command=""{Binding ReplacePage}"" CommandParameter=""{{ signupPage }}"" />
            </VerticalStackLayout.GestureRecognizers>
            {% endif %}
        </VerticalStackLayout>

        {%- comment -%} 4. Schedule Unavailability {%- endcomment -%}
        <VerticalStackLayout Grid.Column=""3"" Spacing=""6"" HorizontalOptions=""Center"">
            <Rock:StyledBorder WidthRequest=""58"" HeightRequest=""58"" CornerRadius=""12"" HorizontalOptions=""Center""
                {% if activeKey == 'unavail' %}BackgroundColor=""{Rock:PaletteColor App-Primary-Strong}"" StrokeThickness=""0""{% else %}StyleClass=""bg-interface-softest, border, border-interface-soft""{% endif %}>
                <Rock:Icon IconClass=""calendar-times"" IconFamily=""FontAwesomeSolid"" FontSize=""22""
                    {% if activeKey == 'unavail' %}TextColor=""#FFFFFF""{% else %}StyleClass=""text-primary-strong""{% endif %}
                    HorizontalOptions=""Center"" VerticalOptions=""Center"" />
            </Rock:StyledBorder>
            <Label Text=""Unavailable"" HorizontalTextAlignment=""Center""
                StyleClass=""caption1, {% if activeKey == 'unavail' %}font-weight-semi-bold, text-interface-strongest{% else %}text-interface-medium{% endif %}"" />
            {% if activeKey != 'unavail' %}
            <VerticalStackLayout.GestureRecognizers>
                <TapGestureRecognizer Command=""{Binding ReplacePage}"" CommandParameter=""{{ unavailPage }}"" />
            </VerticalStackLayout.GestureRecognizers>
            {% endif %}
        </VerticalStackLayout>

    </Grid>

    <BoxView HeightRequest=""1"" StyleClass=""bg-interface-softer"" Margin=""0,14,0,0"" />

</VerticalStackLayout>
" );   // Content
            RockMigrationHelper.AddBlockAttributeValue( "A1E7283C-B49D-4056-E570-C286F91302B4", "5682EDBF-68DA-4B43-A593-6C2B936C2839", @"{%- comment -%}
    Serving nav strip - sits at the top of all four serving pages.

    Set activeKey per page: schedule | prefs | signup | unavail.
    That one line is the only difference between the four block instances.

    The four mobile schedule blocks carry no page-link settings of their own
    (Toolbox exposes only ToolboxTemplate / ConfirmDeclineTemplate), so they
    cannot cross-link - this strip is what ties them together.

    ReplacePage rather than PushPage: these behave as tabs, so moving between
    them should not stack pages up behind the user. Back from any of the four
    returns to wherever they entered from.
{%- endcomment -%}
{%- assign activeKey = 'unavail' -%}

{%- assign schedulePage = '56302b84-36e3-4e62-9e74-c5739d7de977' -%}
{%- assign prefsPage    = '1e4c7a55-8b92-4d30-a6f1-3c08d5b72e41' -%}
{%- assign signupPage   = '2f7b3c88-4d61-4e29-b0a5-9e13f6c48d72' -%}
{%- assign unavailPage  = '3a6d9e14-7c25-4f83-91b6-5d420ae7c193' -%}

<VerticalStackLayout Spacing=""0"" StyleClass=""px-16, pt-16, pb-8"">

    <Grid ColumnDefinitions=""*, *, *, *"" ColumnSpacing=""8"">

        {%- comment -%} 1. Current Schedule {%- endcomment -%}
        <VerticalStackLayout Grid.Column=""0"" Spacing=""6"" HorizontalOptions=""Center"">
            <Rock:StyledBorder WidthRequest=""58"" HeightRequest=""58"" CornerRadius=""12"" HorizontalOptions=""Center""
                {% if activeKey == 'schedule' %}BackgroundColor=""{Rock:PaletteColor App-Primary-Strong}"" StrokeThickness=""0""{% else %}StyleClass=""bg-interface-softest, border, border-interface-soft""{% endif %}>
                <Rock:Icon IconClass=""calendar-check"" IconFamily=""FontAwesomeSolid"" FontSize=""22""
                    {% if activeKey == 'schedule' %}TextColor=""#FFFFFF""{% else %}StyleClass=""text-primary-strong""{% endif %}
                    HorizontalOptions=""Center"" VerticalOptions=""Center"" />
            </Rock:StyledBorder>
            <Label Text=""Schedule"" HorizontalTextAlignment=""Center""
                StyleClass=""caption1, {% if activeKey == 'schedule' %}font-weight-semi-bold, text-interface-strongest{% else %}text-interface-medium{% endif %}"" />
            {% if activeKey != 'schedule' %}
            <VerticalStackLayout.GestureRecognizers>
                <TapGestureRecognizer Command=""{Binding ReplacePage}"" CommandParameter=""{{ schedulePage }}"" />
            </VerticalStackLayout.GestureRecognizers>
            {% endif %}
        </VerticalStackLayout>

        {%- comment -%} 2. Schedule Preferences {%- endcomment -%}
        <VerticalStackLayout Grid.Column=""1"" Spacing=""6"" HorizontalOptions=""Center"">
            <Rock:StyledBorder WidthRequest=""58"" HeightRequest=""58"" CornerRadius=""12"" HorizontalOptions=""Center""
                {% if activeKey == 'prefs' %}BackgroundColor=""{Rock:PaletteColor App-Primary-Strong}"" StrokeThickness=""0""{% else %}StyleClass=""bg-interface-softest, border, border-interface-soft""{% endif %}>
                <Rock:Icon IconClass=""sliders-h"" IconFamily=""FontAwesomeSolid"" FontSize=""22""
                    {% if activeKey == 'prefs' %}TextColor=""#FFFFFF""{% else %}StyleClass=""text-primary-strong""{% endif %}
                    HorizontalOptions=""Center"" VerticalOptions=""Center"" />
            </Rock:StyledBorder>
            <Label Text=""Preferences"" HorizontalTextAlignment=""Center""
                StyleClass=""caption1, {% if activeKey == 'prefs' %}font-weight-semi-bold, text-interface-strongest{% else %}text-interface-medium{% endif %}"" />
            {% if activeKey != 'prefs' %}
            <VerticalStackLayout.GestureRecognizers>
                <TapGestureRecognizer Command=""{Binding ReplacePage}"" CommandParameter=""{{ prefsPage }}"" />
            </VerticalStackLayout.GestureRecognizers>
            {% endif %}
        </VerticalStackLayout>

        {%- comment -%} 3. Sign Up for Additional Times {%- endcomment -%}
        <VerticalStackLayout Grid.Column=""2"" Spacing=""6"" HorizontalOptions=""Center"">
            <Rock:StyledBorder WidthRequest=""58"" HeightRequest=""58"" CornerRadius=""12"" HorizontalOptions=""Center""
                {% if activeKey == 'signup' %}BackgroundColor=""{Rock:PaletteColor App-Primary-Strong}"" StrokeThickness=""0""{% else %}StyleClass=""bg-interface-softest, border, border-interface-soft""{% endif %}>
                <Rock:Icon IconClass=""calendar-plus"" IconFamily=""FontAwesomeSolid"" FontSize=""22""
                    {% if activeKey == 'signup' %}TextColor=""#FFFFFF""{% else %}StyleClass=""text-primary-strong""{% endif %}
                    HorizontalOptions=""Center"" VerticalOptions=""Center"" />
            </Rock:StyledBorder>
            <Label Text=""Sign Up"" HorizontalTextAlignment=""Center""
                StyleClass=""caption1, {% if activeKey == 'signup' %}font-weight-semi-bold, text-interface-strongest{% else %}text-interface-medium{% endif %}"" />
            {% if activeKey != 'signup' %}
            <VerticalStackLayout.GestureRecognizers>
                <TapGestureRecognizer Command=""{Binding ReplacePage}"" CommandParameter=""{{ signupPage }}"" />
            </VerticalStackLayout.GestureRecognizers>
            {% endif %}
        </VerticalStackLayout>

        {%- comment -%} 4. Schedule Unavailability {%- endcomment -%}
        <VerticalStackLayout Grid.Column=""3"" Spacing=""6"" HorizontalOptions=""Center"">
            <Rock:StyledBorder WidthRequest=""58"" HeightRequest=""58"" CornerRadius=""12"" HorizontalOptions=""Center""
                {% if activeKey == 'unavail' %}BackgroundColor=""{Rock:PaletteColor App-Primary-Strong}"" StrokeThickness=""0""{% else %}StyleClass=""bg-interface-softest, border, border-interface-soft""{% endif %}>
                <Rock:Icon IconClass=""calendar-times"" IconFamily=""FontAwesomeSolid"" FontSize=""22""
                    {% if activeKey == 'unavail' %}TextColor=""#FFFFFF""{% else %}StyleClass=""text-primary-strong""{% endif %}
                    HorizontalOptions=""Center"" VerticalOptions=""Center"" />
            </Rock:StyledBorder>
            <Label Text=""Unavailable"" HorizontalTextAlignment=""Center""
                StyleClass=""caption1, {% if activeKey == 'unavail' %}font-weight-semi-bold, text-interface-strongest{% else %}text-interface-medium{% endif %}"" />
            {% if activeKey != 'unavail' %}
            <VerticalStackLayout.GestureRecognizers>
                <TapGestureRecognizer Command=""{Binding ReplacePage}"" CommandParameter=""{{ unavailPage }}"" />
            </VerticalStackLayout.GestureRecognizers>
            {% endif %}
        </VerticalStackLayout>

    </Grid>

    <BoxView HeightRequest=""1"" StyleClass=""bg-interface-softer"" Margin=""0,14,0,0"" />

</VerticalStackLayout>
" );   // Content
            RockMigrationHelper.AddBlockAttributeValue( "7EB4F509-816A-4D23-B247-9F53C6E0A8D1", "5682EDBF-68DA-4B43-A593-6C2B936C2839", @"{%- comment -%}
    Serving nav strip - sits at the top of all four serving pages.

    Set activeKey per page: schedule | prefs | signup | unavail.
    That one line is the only difference between the four block instances.

    The four mobile schedule blocks carry no page-link settings of their own
    (Toolbox exposes only ToolboxTemplate / ConfirmDeclineTemplate), so they
    cannot cross-link - this strip is what ties them together.

    ReplacePage rather than PushPage: these behave as tabs, so moving between
    them should not stack pages up behind the user. Back from any of the four
    returns to wherever they entered from.
{%- endcomment -%}
{%- assign activeKey = 'schedule' -%}

{%- assign schedulePage = '56302b84-36e3-4e62-9e74-c5739d7de977' -%}
{%- assign prefsPage    = '1e4c7a55-8b92-4d30-a6f1-3c08d5b72e41' -%}
{%- assign signupPage   = '2f7b3c88-4d61-4e29-b0a5-9e13f6c48d72' -%}
{%- assign unavailPage  = '3a6d9e14-7c25-4f83-91b6-5d420ae7c193' -%}

<VerticalStackLayout Spacing=""0"" StyleClass=""px-16, pt-16, pb-8"">

    <Grid ColumnDefinitions=""*, *, *, *"" ColumnSpacing=""8"">

        {%- comment -%} 1. Current Schedule {%- endcomment -%}
        <VerticalStackLayout Grid.Column=""0"" Spacing=""6"" HorizontalOptions=""Center"">
            <Rock:StyledBorder WidthRequest=""58"" HeightRequest=""58"" CornerRadius=""12"" HorizontalOptions=""Center""
                {% if activeKey == 'schedule' %}BackgroundColor=""{Rock:PaletteColor App-Primary-Strong}"" StrokeThickness=""0""{% else %}StyleClass=""bg-interface-softest, border, border-interface-soft""{% endif %}>
                <Rock:Icon IconClass=""calendar-check"" IconFamily=""FontAwesomeSolid"" FontSize=""22""
                    {% if activeKey == 'schedule' %}TextColor=""#FFFFFF""{% else %}StyleClass=""text-primary-strong""{% endif %}
                    HorizontalOptions=""Center"" VerticalOptions=""Center"" />
            </Rock:StyledBorder>
            <Label Text=""Schedule"" HorizontalTextAlignment=""Center""
                StyleClass=""caption1, {% if activeKey == 'schedule' %}font-weight-semi-bold, text-interface-strongest{% else %}text-interface-medium{% endif %}"" />
            {% if activeKey != 'schedule' %}
            <VerticalStackLayout.GestureRecognizers>
                <TapGestureRecognizer Command=""{Binding ReplacePage}"" CommandParameter=""{{ schedulePage }}"" />
            </VerticalStackLayout.GestureRecognizers>
            {% endif %}
        </VerticalStackLayout>

        {%- comment -%} 2. Schedule Preferences {%- endcomment -%}
        <VerticalStackLayout Grid.Column=""1"" Spacing=""6"" HorizontalOptions=""Center"">
            <Rock:StyledBorder WidthRequest=""58"" HeightRequest=""58"" CornerRadius=""12"" HorizontalOptions=""Center""
                {% if activeKey == 'prefs' %}BackgroundColor=""{Rock:PaletteColor App-Primary-Strong}"" StrokeThickness=""0""{% else %}StyleClass=""bg-interface-softest, border, border-interface-soft""{% endif %}>
                <Rock:Icon IconClass=""sliders-h"" IconFamily=""FontAwesomeSolid"" FontSize=""22""
                    {% if activeKey == 'prefs' %}TextColor=""#FFFFFF""{% else %}StyleClass=""text-primary-strong""{% endif %}
                    HorizontalOptions=""Center"" VerticalOptions=""Center"" />
            </Rock:StyledBorder>
            <Label Text=""Preferences"" HorizontalTextAlignment=""Center""
                StyleClass=""caption1, {% if activeKey == 'prefs' %}font-weight-semi-bold, text-interface-strongest{% else %}text-interface-medium{% endif %}"" />
            {% if activeKey != 'prefs' %}
            <VerticalStackLayout.GestureRecognizers>
                <TapGestureRecognizer Command=""{Binding ReplacePage}"" CommandParameter=""{{ prefsPage }}"" />
            </VerticalStackLayout.GestureRecognizers>
            {% endif %}
        </VerticalStackLayout>

        {%- comment -%} 3. Sign Up for Additional Times {%- endcomment -%}
        <VerticalStackLayout Grid.Column=""2"" Spacing=""6"" HorizontalOptions=""Center"">
            <Rock:StyledBorder WidthRequest=""58"" HeightRequest=""58"" CornerRadius=""12"" HorizontalOptions=""Center""
                {% if activeKey == 'signup' %}BackgroundColor=""{Rock:PaletteColor App-Primary-Strong}"" StrokeThickness=""0""{% else %}StyleClass=""bg-interface-softest, border, border-interface-soft""{% endif %}>
                <Rock:Icon IconClass=""calendar-plus"" IconFamily=""FontAwesomeSolid"" FontSize=""22""
                    {% if activeKey == 'signup' %}TextColor=""#FFFFFF""{% else %}StyleClass=""text-primary-strong""{% endif %}
                    HorizontalOptions=""Center"" VerticalOptions=""Center"" />
            </Rock:StyledBorder>
            <Label Text=""Sign Up"" HorizontalTextAlignment=""Center""
                StyleClass=""caption1, {% if activeKey == 'signup' %}font-weight-semi-bold, text-interface-strongest{% else %}text-interface-medium{% endif %}"" />
            {% if activeKey != 'signup' %}
            <VerticalStackLayout.GestureRecognizers>
                <TapGestureRecognizer Command=""{Binding ReplacePage}"" CommandParameter=""{{ signupPage }}"" />
            </VerticalStackLayout.GestureRecognizers>
            {% endif %}
        </VerticalStackLayout>

        {%- comment -%} 4. Schedule Unavailability {%- endcomment -%}
        <VerticalStackLayout Grid.Column=""3"" Spacing=""6"" HorizontalOptions=""Center"">
            <Rock:StyledBorder WidthRequest=""58"" HeightRequest=""58"" CornerRadius=""12"" HorizontalOptions=""Center""
                {% if activeKey == 'unavail' %}BackgroundColor=""{Rock:PaletteColor App-Primary-Strong}"" StrokeThickness=""0""{% else %}StyleClass=""bg-interface-softest, border, border-interface-soft""{% endif %}>
                <Rock:Icon IconClass=""calendar-times"" IconFamily=""FontAwesomeSolid"" FontSize=""22""
                    {% if activeKey == 'unavail' %}TextColor=""#FFFFFF""{% else %}StyleClass=""text-primary-strong""{% endif %}
                    HorizontalOptions=""Center"" VerticalOptions=""Center"" />
            </Rock:StyledBorder>
            <Label Text=""Unavailable"" HorizontalTextAlignment=""Center""
                StyleClass=""caption1, {% if activeKey == 'unavail' %}font-weight-semi-bold, text-interface-strongest{% else %}text-interface-medium{% endif %}"" />
            {% if activeKey != 'unavail' %}
            <VerticalStackLayout.GestureRecognizers>
                <TapGestureRecognizer Command=""{Binding ReplacePage}"" CommandParameter=""{{ unavailPage }}"" />
            </VerticalStackLayout.GestureRecognizers>
            {% endif %}
        </VerticalStackLayout>

    </Grid>

    <BoxView HeightRequest=""1"" StyleClass=""bg-interface-softer"" Margin=""0,14,0,0"" />

</VerticalStackLayout>
" );   // Content
            RockMigrationHelper.AddBlockAttributeValue( "2C7A4F19-6B83-4E05-9D24-A15C8E30B7F6", "5682EDBF-68DA-4B43-A593-6C2B936C2839", @"{%- comment -%}
    ===========================================================================
    NOTIFICATIONS - Content (first paint)
    ---------------------------------------------------------------------------
    Generated by build_notif.py - edit that, not this, or Content and
    CallbackLogic will drift apart.

    Dynamic Content is ON, so ""Show More"" fires {Binding Callback} and this block
    re-renders IN PLACE. That is the whole point: ReplacePage rebuilt the page and
    threw away scroll position, which made paging feel like a navigation.

    Sql is granted to this block alone. The global DefaultEnabledLavaCommands is
    ""RockEntity"", and the Lava Item List inherits that global - which is why
    {% sql %} could never run there.
    ===========================================================================
{%- endcomment -%}

{%- assign pageSize = 25 -%}
{%- assign take = pageSize -%}
{%- assign detailPageGuid = 'f77c9f5c-3aa5-4ed3-8390-db086abe7bf7' -%}
{%- assign takePlusOne = take | Plus:1 -%}
{%- assign pushMediumGuid = '3638c6df-4ff3-4a52-b4b8-afb754991597' -%}
{%- assign notifyGroupTypeGuid = 'd1d95777-ffa3-cbb3-4a6d-658706daed33' -%}

{% if CurrentPerson %}
{%- sql return:'rows' -%}
    SELECT TOP ({{ takePlusOne }})
           c.[Id], c.[PushTitle], c.[SendDateTime]
    FROM [Communication] c
    WHERE c.[CommunicationType] = 3
      AND ISNULL( c.[PushMessage], '' ) <> ''
      AND (
            EXISTS (
                SELECT 1
                FROM [CommunicationRecipient] cr
                JOIN [EntityType] et ON et.[Id] = cr.[MediumEntityTypeId]
                                    AND et.[Guid] = '{{ pushMediumGuid }}'
                WHERE cr.[CommunicationId] = c.[Id]
                  AND ( cr.[PersonAliasId] = {{ CurrentPerson.PrimaryAliasId }}
                        OR cr.[PersonalDeviceId] IN (
                            SELECT pd.[Id] FROM [PersonalDevice] pd
                            WHERE pd.[PersonAliasId] = {{ CurrentPerson.PrimaryAliasId }} ) )
            )
            OR c.[ListGroupId] IN (
                SELECT gm.[GroupId]
                FROM [GroupMember] gm
                JOIN [Group] g ON g.[Id] = gm.[GroupId]
                JOIN [GroupType] gt ON gt.[Id] = g.[GroupTypeId]
                                   AND gt.[Guid] = '{{ notifyGroupTypeGuid }}'
                WHERE gm.[PersonId] = {{ CurrentPerson.Id }}
            )
            OR ( c.[UrlReferrer] IS NULL AND c.[ListGroupId] IS NULL )
          )
    ORDER BY c.[SendDateTime] DESC
{%- endsql -%}

{%- comment -%} one row beyond the window tells us whether to offer Show More {%- endcomment -%}
{%- assign fetched = rows | Size -%}
{%- assign hasMore = false -%}
{%- if fetched > take -%}{%- assign hasMore = true -%}{%- endif -%}

<VerticalStackLayout Spacing=""0"" StyleClass=""p-16"">

    {%- if fetched == 0 -%}
        <VerticalStackLayout Spacing=""12"" StyleClass=""p-32"" HorizontalOptions=""Fill"">
            <Rock:Icon IconClass=""bell-slash"" IconFamily=""FontAwesomeSolid"" FontSize=""36""
                StyleClass=""text-interface-soft"" HorizontalOptions=""Center"" />
            <Label Text=""You have no notifications yet.""
                StyleClass=""body, text-interface-medium""
                HorizontalTextAlignment=""Center"" HorizontalOptions=""Fill"" />
        </VerticalStackLayout>
    {%- else -%}
        {%- assign shown = 0 -%}
        {%- for row in rows -%}
            {%- if shown < take -%}
                {%- assign shown = shown | Plus:1 -%}
                <Rock:StyledBorder CornerRadius=""12"" Padding=""14,12"" StyleClass=""bg-interface-softest"" Margin=""0,0,0,8"">
                    <Rock:StyledBorder.GestureRecognizers>
                        <TapGestureRecognizer Command=""{Binding PushPage}""
                            CommandParameter=""{{ detailPageGuid }}?ItemId={{ row.Id }}"" />
                    </Rock:StyledBorder.GestureRecognizers>
                    <Grid ColumnDefinitions=""*, Auto"" ColumnSpacing=""12"" VerticalOptions=""Center"">
                        <VerticalStackLayout Grid.Column=""0"" Spacing=""2"" VerticalOptions=""Center"">
                            <Label Text=""{{ row.PushTitle | Escape }}""
                                StyleClass=""body, bold, text-interface-strongest"" LineBreakMode=""TailTruncation"" />
                            <Label Text=""{{ row.SendDateTime | Date:'MMM d, yyyy' | Upcase }}""
                                StyleClass=""caption1, text-interface-medium"" />
                        </VerticalStackLayout>
                        <Rock:Icon Grid.Column=""1"" IconClass=""chevron-right"" IconFamily=""FontAwesomeSolid""
                            FontSize=""16"" StyleClass=""text-interface-soft"" VerticalOptions=""Center"" />
                    </Grid>
                </Rock:StyledBorder>
            {%- endif -%}
        {%- endfor -%}

        {%- if hasMore -%}
            {%- assign nextTake = take | Plus:pageSize -%}
            <Button Text=""Show More"" StyleClass=""btn, btn-primary""
                HorizontalOptions=""Center"" Margin=""0,8,0,0""
                Command=""{Binding Callback}"">
                <Button.CommandParameter>
                    <Rock:CallbackParameters Name=""ShowMore"">
                        <Rock:Parameter Name=""Take"" Value=""{{ nextTake }}"" />
                    </Rock:CallbackParameters>
                </Button.CommandParameter>
            </Button>
        {%- endif -%}
    {%- endif -%}

</VerticalStackLayout>
{% endif %}" );   // Content
            RockMigrationHelper.AddBlockAttributeValue( "2C7A4F19-6B83-4E05-9D24-A15C8E30B7F6", "24516448-3F1F-4F27-97A1-CFB4F8B277B5", @"Sql" );   // EnabledLavaCommands
            RockMigrationHelper.AddBlockAttributeValue( "2C7A4F19-6B83-4E05-9D24-A15C8E30B7F6", "B31D29A0-3725-4AEB-8360-7D91B9CDFE47", @"True" );   // DynamicContent
            RockMigrationHelper.AddBlockAttributeValue( "2C7A4F19-6B83-4E05-9D24-A15C8E30B7F6", "2725F971-243A-4B60-83EB-527BA8C08737", @"{%- comment -%}
    ===========================================================================
    NOTIFICATIONS - CallbackLogic (Show More)
    ---------------------------------------------------------------------------
    Generated by build_notif.py - edit that, not this, or Content and
    CallbackLogic will drift apart.

    Dynamic Content is ON, so ""Show More"" fires {Binding Callback} and this block
    re-renders IN PLACE. That is the whole point: ReplacePage rebuilt the page and
    threw away scroll position, which made paging feel like a navigation.

    Sql is granted to this block alone. The global DefaultEnabledLavaCommands is
    ""RockEntity"", and the Lava Item List inherits that global - which is why
    {% sql %} could never run there.
    ===========================================================================
{%- endcomment -%}

{%- assign pageSize = 25 -%}
{%- assign take = Parameters.Take | AsInteger -%}
{%- if take == null or take < 1 -%}{%- assign take = pageSize -%}{%- endif -%}
{% if Command == 'ShowMore' %}
{%- assign detailPageGuid = 'f77c9f5c-3aa5-4ed3-8390-db086abe7bf7' -%}
{%- assign takePlusOne = take | Plus:1 -%}
{%- assign pushMediumGuid = '3638c6df-4ff3-4a52-b4b8-afb754991597' -%}
{%- assign notifyGroupTypeGuid = 'd1d95777-ffa3-cbb3-4a6d-658706daed33' -%}

{% if CurrentPerson %}
{%- sql return:'rows' -%}
    SELECT TOP ({{ takePlusOne }})
           c.[Id], c.[PushTitle], c.[SendDateTime]
    FROM [Communication] c
    WHERE c.[CommunicationType] = 3
      AND ISNULL( c.[PushMessage], '' ) <> ''
      AND (
            EXISTS (
                SELECT 1
                FROM [CommunicationRecipient] cr
                JOIN [EntityType] et ON et.[Id] = cr.[MediumEntityTypeId]
                                    AND et.[Guid] = '{{ pushMediumGuid }}'
                WHERE cr.[CommunicationId] = c.[Id]
                  AND ( cr.[PersonAliasId] = {{ CurrentPerson.PrimaryAliasId }}
                        OR cr.[PersonalDeviceId] IN (
                            SELECT pd.[Id] FROM [PersonalDevice] pd
                            WHERE pd.[PersonAliasId] = {{ CurrentPerson.PrimaryAliasId }} ) )
            )
            OR c.[ListGroupId] IN (
                SELECT gm.[GroupId]
                FROM [GroupMember] gm
                JOIN [Group] g ON g.[Id] = gm.[GroupId]
                JOIN [GroupType] gt ON gt.[Id] = g.[GroupTypeId]
                                   AND gt.[Guid] = '{{ notifyGroupTypeGuid }}'
                WHERE gm.[PersonId] = {{ CurrentPerson.Id }}
            )
            OR ( c.[UrlReferrer] IS NULL AND c.[ListGroupId] IS NULL )
          )
    ORDER BY c.[SendDateTime] DESC
{%- endsql -%}

{%- comment -%} one row beyond the window tells us whether to offer Show More {%- endcomment -%}
{%- assign fetched = rows | Size -%}
{%- assign hasMore = false -%}
{%- if fetched > take -%}{%- assign hasMore = true -%}{%- endif -%}

<VerticalStackLayout Spacing=""0"" StyleClass=""p-16"">

    {%- if fetched == 0 -%}
        <VerticalStackLayout Spacing=""12"" StyleClass=""p-32"" HorizontalOptions=""Fill"">
            <Rock:Icon IconClass=""bell-slash"" IconFamily=""FontAwesomeSolid"" FontSize=""36""
                StyleClass=""text-interface-soft"" HorizontalOptions=""Center"" />
            <Label Text=""You have no notifications yet.""
                StyleClass=""body, text-interface-medium""
                HorizontalTextAlignment=""Center"" HorizontalOptions=""Fill"" />
        </VerticalStackLayout>
    {%- else -%}
        {%- assign shown = 0 -%}
        {%- for row in rows -%}
            {%- if shown < take -%}
                {%- assign shown = shown | Plus:1 -%}
                <Rock:StyledBorder CornerRadius=""12"" Padding=""14,12"" StyleClass=""bg-interface-softest"" Margin=""0,0,0,8"">
                    <Rock:StyledBorder.GestureRecognizers>
                        <TapGestureRecognizer Command=""{Binding PushPage}""
                            CommandParameter=""{{ detailPageGuid }}?ItemId={{ row.Id }}"" />
                    </Rock:StyledBorder.GestureRecognizers>
                    <Grid ColumnDefinitions=""*, Auto"" ColumnSpacing=""12"" VerticalOptions=""Center"">
                        <VerticalStackLayout Grid.Column=""0"" Spacing=""2"" VerticalOptions=""Center"">
                            <Label Text=""{{ row.PushTitle | Escape }}""
                                StyleClass=""body, bold, text-interface-strongest"" LineBreakMode=""TailTruncation"" />
                            <Label Text=""{{ row.SendDateTime | Date:'MMM d, yyyy' | Upcase }}""
                                StyleClass=""caption1, text-interface-medium"" />
                        </VerticalStackLayout>
                        <Rock:Icon Grid.Column=""1"" IconClass=""chevron-right"" IconFamily=""FontAwesomeSolid""
                            FontSize=""16"" StyleClass=""text-interface-soft"" VerticalOptions=""Center"" />
                    </Grid>
                </Rock:StyledBorder>
            {%- endif -%}
        {%- endfor -%}

        {%- if hasMore -%}
            {%- assign nextTake = take | Plus:pageSize -%}
            <Button Text=""Show More"" StyleClass=""btn, btn-primary""
                HorizontalOptions=""Center"" Margin=""0,8,0,0""
                Command=""{Binding Callback}"">
                <Button.CommandParameter>
                    <Rock:CallbackParameters Name=""ShowMore"">
                        <Rock:Parameter Name=""Take"" Value=""{{ nextTake }}"" />
                    </Rock:CallbackParameters>
                </Button.CommandParameter>
            </Button>
        {%- endif -%}
    {%- endif -%}

</VerticalStackLayout>
{% endif %}
{% endif %}" );   // CallbackLogic
            RockMigrationHelper.AddBlockAttributeValue( "C519E24F-A086-4D2B-B7C4-9F203B85D67A", "5682EDBF-68DA-4B43-A593-6C2B936C2839", @"{%- comment -%}
    ===========================================================================
    PODCAST FEED - LIST   (reads an external RSS feed, no content channel)
    ---------------------------------------------------------------------------
    Everything on this page comes from the feed at render time. Nothing is
    stored in Rock, so there is no sync job and no content channel to keep in
    step - but equally there is nothing to query, sort or page beyond what the
    feed itself returns.

    !! THE FEED ONLY CARRIES THE 30 MOST RECENT EPISODES !!
    That is Buzzsprout's default, not a limit of this block. The back catalogue
    (300+ episodes) is NOT reachable this way. If the full archive is needed,
    this approach cannot deliver it and a stored copy is required.

    HOW THE XML ARRIVES
    {% webrequest responsecontenttype:'xml' %} hands back an object built by
    Rock's ExpandoObjectHelper, and its shape is not obvious:

      feed.rss.channel.item        list of episodes
      channel.image                a LIST even though there is one image, so it
                                   is read with a for/limit:1 rather than [0]
      ep.guid.Value                the id - <guid> has an attribute, so the text
                                   lands under .Value rather than on guid itself
      ep.enclosure.url             the mp3
      ep.title                     NOTE: has a leading space, hence Trim

    Namespaced tags keep their full {uri}local name as the KEY, which cannot be
    written with dot notation - hence durationKey below and ep[durationKey].
    If this block ever errors on parse, that indexing is the first thing to
    suspect: delete durationKey and the two lines using it and the rest stands.

    CACHING - two separate layers, deliberately
      {% cache %}    server side, 15 min. Stops every device that opens the page
                     from pulling 250KB from Buzzsprout.
      CacheDuration  client side, 1 hour, set on the block itself.
    Episodes publish weekly, so neither needs to be tight.
    ===========================================================================
{%- endcomment -%}
{%- assign feedUrl = 'https://feeds.buzzsprout.com/21178.rss' -%}
{%- assign detailPageGuid = 'b4081d3e-9f75-4c1a-a6b3-8e1f2a74c569' -%}
{%- assign durationKey = '{http://www.itunes.com/dtds/podcast-1.0.dtd}duration' -%}

<VerticalStackLayout Spacing=""0"">
{% cache key:'nfluence-podcast-rss-list' duration:'900' %}
{% webrequest url:'{{ feedUrl }}' responsecontenttype:'xml' return:'feed' timeout:'15000' %}
    {%- assign channel = feed.rss.channel -%}
    {%- assign episodes = channel.item -%}
    {%- assign artwork = '' -%}
    {%- for img in channel.image limit:1 -%}{%- assign artwork = img.url -%}{%- endfor -%}

    {%- if episodes == null or episodes == empty -%}
        {%- comment -%} covers a dead feed, a timeout and a 500 alike: all three
            leave episodes unset rather than throwing {%- endcomment -%}
        <VerticalStackLayout Spacing=""12"" StyleClass=""p-32"" VerticalOptions=""Center"">
            <Rock:Icon IconClass=""podcast"" IconFamily=""FontAwesomeSolid"" FontSize=""40""
                StyleClass=""text-interface-soft"" HorizontalOptions=""Center"" />
            <Label Text=""Episodes are unavailable right now.""
                StyleClass=""title3, bold, text-interface-strongest""
                HorizontalTextAlignment=""Center"" HorizontalOptions=""Fill"" />
            <Label Text=""Please check back in a little while.""
                StyleClass=""body, text-interface-medium""
                HorizontalTextAlignment=""Center"" HorizontalOptions=""Fill"" />
        </VerticalStackLayout>
    {%- else -%}

        <VerticalStackLayout Spacing=""10"" StyleClass=""p-16"">
            {%- if artwork != '' and artwork != null -%}
            <Rock:StyledBorder CornerRadius=""14"" StrokeThickness=""0"" Padding=""0""
                HorizontalOptions=""Center"" WidthRequest=""150"" HeightRequest=""150"">
                <Rock:Image Source=""{{ artwork | Escape }}"" Aspect=""AspectFill""
                    HorizontalOptions=""Fill"" VerticalOptions=""Fill"" />
            </Rock:StyledBorder>
            {%- endif -%}
            <Label Text=""{{ channel.title | Trim | Escape }}""
                StyleClass=""title2, bold, text-interface-strongest""
                HorizontalTextAlignment=""Center"" HorizontalOptions=""Fill"" />
        </VerticalStackLayout>

        <VerticalStackLayout Spacing=""0"" StyleClass=""px-16, pb-16"">
            {%- for ep in episodes -%}
                {%- assign epTitle = ep.title | Trim -%}
                {%- assign durSec = ep[durationKey] | AsInteger -%}
                {%- assign durMin = durSec | DividedBy:60 | AsInteger -%}
                <Rock:StyledBorder CornerRadius=""12"" Padding=""12"" StrokeThickness=""0""
                    StyleClass=""bg-interface-softest"" Margin=""0,0,0,10"">
                    <Rock:StyledBorder.GestureRecognizers>
                        <TapGestureRecognizer Command=""{Binding PushPage}""
                            CommandParameter=""{{ detailPageGuid }}?EpisodeId={{ ep.guid.Value | Escape }}"" />
                    </Rock:StyledBorder.GestureRecognizers>
                    <Grid ColumnDefinitions=""64, *, Auto"" ColumnSpacing=""12"" VerticalOptions=""Center"">
                        {%- if artwork != '' and artwork != null -%}
                        <Rock:StyledBorder Grid.Column=""0"" CornerRadius=""8"" StrokeThickness=""0"" Padding=""0""
                            WidthRequest=""64"" HeightRequest=""64"">
                            <Rock:Image Source=""{{ artwork | Escape }}"" Aspect=""AspectFill""
                                HorizontalOptions=""Fill"" VerticalOptions=""Fill"" />
                        </Rock:StyledBorder>
                        {%- endif -%}
                        <VerticalStackLayout Grid.Column=""1"" Spacing=""3"" VerticalOptions=""Center"">
                            <Label Text=""{{ epTitle | Escape }}""
                                StyleClass=""body, bold, text-interface-strongest""
                                LineBreakMode=""TailTruncation"" MaxLines=""2"" />
                            <Label StyleClass=""caption1, text-interface-medium""
                                Text=""{{ ep.pubDate | Date:'MMM d, yyyy' }}{% if durMin > 0 %} &#8226; {{ durMin }} min{% endif %}"" />
                        </VerticalStackLayout>
                        <Rock:Icon Grid.Column=""2"" IconClass=""chevron-right"" IconFamily=""FontAwesomeSolid""
                            FontSize=""16"" StyleClass=""text-interface-soft"" VerticalOptions=""Center"" />
                    </Grid>
                </Rock:StyledBorder>
            {%- endfor -%}
        </VerticalStackLayout>

    {%- endif -%}
{% endwebrequest %}
{% endcache %}
</VerticalStackLayout>
" );   // Content
            RockMigrationHelper.AddBlockAttributeValue( "C519E24F-A086-4D2B-B7C4-9F203B85D67A", "24516448-3F1F-4F27-97A1-CFB4F8B277B5", @"WebRequest,Cache" );   // EnabledLavaCommands
            RockMigrationHelper.AddBlockAttributeValue( "C519E24F-A086-4D2B-B7C4-9F203B85D67A", "B31D29A0-3725-4AEB-8360-7D91B9CDFE47", @"True" );   // DynamicContent
            RockMigrationHelper.AddBlockAttributeValue( "D62AF350-B197-4E3C-98D5-A0314C96E78B", "5682EDBF-68DA-4B43-A593-6C2B936C2839", @"{%- comment -%}
    ===========================================================================
    PODCAST FEED - EPISODE DETAIL   (external RSS, no content channel)
    ---------------------------------------------------------------------------
    Arrives as ?EpisodeId=Buzzsprout-19568132, taken from <guid>. The feed has no
    per-episode endpoint, so the whole feed is re-read and the matching item is
    picked out. That is why the server-side {% cache %} matters here far more
    than on the list.

    CACHING - block CacheDuration is 60 and that is SAFE here
    Confirmed against the decompiled shell (BlockBase.GetCachedValueAsync), not
    assumed: the cache key is

        block-{BlockGuid}-{sorted querystring}-initialContent

    so ?EpisodeId=Buzzsprout-19568132 and ?EpisodeId=Buzzsprout-19568164 are
    different entries and cannot serve each other's content. The old
    content-channel Podcast Detail (13512) sits at 0, but that is a preference,
    not a requirement.

    The {% cache %} below is a separate, server-side layer keyed by episode id.
    It is what stops each device pulling 250KB from Buzzsprout on a cache miss.

    Show notes come through as HTML, so they go into Rock:Html inside CDATA
    rather than being escaped - the same treatment block 13512 gives its body.
    ===========================================================================
{%- endcomment -%}
{%- assign feedUrl = 'https://feeds.buzzsprout.com/21178.rss' -%}
{%- assign episodeId = PageParameter.EpisodeId -%}
{%- assign durationKey = '{http://www.itunes.com/dtds/podcast-1.0.dtd}duration' -%}
{%- assign contentKey = '{http://purl.org/rss/1.0/modules/content/}encoded' -%}
{%- assign authorKey = '{http://www.itunes.com/dtds/podcast-1.0.dtd}author' -%}

<VerticalStackLayout Spacing=""0"" StyleClass=""pb-16"">
{% cache key:'nfluence-podcast-rss-ep-{{ episodeId }}' duration:'900' %}
{% webrequest url:'{{ feedUrl }}' responsecontenttype:'xml' return:'feed' timeout:'15000' %}
    {%- assign channel = feed.rss.channel -%}
    {%- assign artwork = '' -%}
    {%- for img in channel.image limit:1 -%}{%- assign artwork = img.url -%}{%- endfor -%}

    {%- comment -%} assigns inside a for survive it, so the match is carried out {%- endcomment -%}
    {%- assign found = false -%}
    {%- assign epTitle = '' -%}
    {%- assign epAudio = '' -%}
    {%- assign epDate = '' -%}
    {%- assign epNotes = '' -%}
    {%- assign epAuthor = '' -%}
    {%- assign epDurSec = 0 -%}
    {%- for ep in channel.item -%}
        {%- if found == false and ep.guid.Value == episodeId -%}
            {%- assign found = true -%}
            {%- assign epTitle = ep.title | Trim -%}
            {%- assign epAudio = ep.enclosure.url -%}
            {%- assign epDate = ep.pubDate -%}
            {%- assign epAuthor = ep[authorKey] -%}
            {%- assign epDurSec = ep[durationKey] | AsInteger -%}
            {%- assign epNotes = ep[contentKey] -%}
            {%- if epNotes == '' or epNotes == null -%}{%- assign epNotes = ep.description -%}{%- endif -%}
        {%- endif -%}
    {%- endfor -%}

    {%- if found == false -%}
        {%- comment -%} the feed only holds the latest 30, so an episode that has
            aged out of it lands here rather than rendering blank {%- endcomment -%}
        <VerticalStackLayout Spacing=""12"" StyleClass=""p-32"" VerticalOptions=""Center"">
            <Rock:Icon IconClass=""circle-question"" IconFamily=""FontAwesomeSolid"" FontSize=""40""
                StyleClass=""text-interface-soft"" HorizontalOptions=""Center"" />
            <Label Text=""This episode is no longer in the feed.""
                StyleClass=""title3, bold, text-interface-strongest""
                HorizontalTextAlignment=""Center"" HorizontalOptions=""Fill"" />
            <Label Text=""Only the most recent episodes are available here.""
                StyleClass=""body, text-interface-medium""
                HorizontalTextAlignment=""Center"" HorizontalOptions=""Fill"" />
            <Button Text=""Back"" StyleClass=""btn, btn-primary"" HorizontalOptions=""Center""
                Command=""{Binding PopPage}"" Margin=""0,8,0,0"" />
        </VerticalStackLayout>
    {%- else -%}
        {%- assign epDurMin = epDurSec | DividedBy:60 | AsInteger -%}

        <Rock:MediaPlayer x:Name=""episodePlayer""
            Source=""{{ epAudio | Escape }}""
            Title=""{{ epTitle | Escape }}""
            Subtitle=""{{ epAuthor | Escape }}""
            ShowThumbnail=""false""
            IsCastEnabled=""true""
            MeasureWithAspectRatio=""false""
            HeightRequest=""300"">
            <Rock:MediaPlayer.OverlayContent>
                <Grid InputTransparent=""False"">
                    {% if artwork != '' and artwork != null %}
                    <Rock:StyledBorder StrokeThickness=""0"" StyleClass=""bg-interface-softest""
                        HorizontalOptions=""Fill"" VerticalOptions=""Fill"" />
                    <Rock:Image Source=""{{ artwork | Escape }}"" Aspect=""AspectFill""
                        HorizontalOptions=""Fill"" VerticalOptions=""Fill"" />
                    {% endif %}

                    <Rock:StyledBorder WidthRequest=""70"" HeightRequest=""70"" CornerRadius=""35"" Padding=""0""
                        HorizontalOptions=""Center"" VerticalOptions=""Center"" InputTransparent=""False""
                        StrokeThickness=""3"" Stroke=""#FFFFFF""
                        BackgroundColor=""{Rock:PaletteColor App-Primary-Strong}"">
                        <Rock:StyledBorder.Shadow><Shadow Brush=""#000000"" Offset=""0,2"" Radius=""10"" Opacity=""0.45"" /></Rock:StyledBorder.Shadow>
                        <Rock:Icon IconClass=""play"" IconFamily=""FontAwesomeSolid"" FontSize=""28""
                            TextColor=""#FFFFFF"" HorizontalOptions=""Center"" VerticalOptions=""Center"" />
                        <Rock:StyledBorder.GestureRecognizers>
                            <TapGestureRecognizer Command=""{Binding PlayCommand}"" />
                        </Rock:StyledBorder.GestureRecognizers>
                        <Rock:StyledBorder.Triggers>
                            <DataTrigger TargetType=""Rock:StyledBorder""
                                Binding=""{Binding Source={x:Reference episodePlayer}, Path=CurrentState}"" Value=""Playing"">
                                <Setter Property=""IsVisible"" Value=""False"" />
                            </DataTrigger>
                        </Rock:StyledBorder.Triggers>
                    </Rock:StyledBorder>

                    <Rock:StyledBorder WidthRequest=""70"" HeightRequest=""70"" CornerRadius=""35"" Padding=""0""
                        HorizontalOptions=""Center"" VerticalOptions=""Center"" IsVisible=""False"" InputTransparent=""False""
                        StrokeThickness=""3"" Stroke=""#FFFFFF""
                        BackgroundColor=""{Rock:PaletteColor App-Primary-Strong}"">
                        <Rock:StyledBorder.Shadow><Shadow Brush=""#000000"" Offset=""0,2"" Radius=""10"" Opacity=""0.45"" /></Rock:StyledBorder.Shadow>
                        <Rock:Icon IconClass=""pause"" IconFamily=""FontAwesomeSolid"" FontSize=""28""
                            TextColor=""#FFFFFF"" HorizontalOptions=""Center"" VerticalOptions=""Center"" />
                        <Rock:StyledBorder.GestureRecognizers>
                            <TapGestureRecognizer Command=""{Binding PauseCommand}"" />
                        </Rock:StyledBorder.GestureRecognizers>
                        <Rock:StyledBorder.Triggers>
                            <DataTrigger TargetType=""Rock:StyledBorder""
                                Binding=""{Binding Source={x:Reference episodePlayer}, Path=CurrentState}"" Value=""Playing"">
                                <Setter Property=""IsVisible"" Value=""True"" />
                            </DataTrigger>
                        </Rock:StyledBorder.Triggers>
                    </Rock:StyledBorder>
                </Grid>
            </Rock:MediaPlayer.OverlayContent>
        </Rock:MediaPlayer>

        <VerticalStackLayout Spacing=""4"" StyleClass=""px-16, pt-16"">
            <Label Text=""{{ epTitle | Escape }}"" StyleClass=""title3, bold, text-interface-strongest"" />
            <Label StyleClass=""caption1, text-interface-medium""
                Text=""{{ epDate | Date:'MMMM d, yyyy' }}{% if epDurMin > 0 %} &#8226; {{ epDurMin }} min{% endif %}"" />
        </VerticalStackLayout>

        <Grid ColumnDefinitions=""*, *"" ColumnSpacing=""8"" StyleClass=""px-16, pt-16"">
            <VerticalStackLayout Grid.Column=""0"" Spacing=""6"" HorizontalOptions=""Center"">
                <Rock:Icon IconClass=""download"" IconFamily=""FontAwesomeSolid"" FontSize=""22""
                    StyleClass=""text-interface-stronger"" HorizontalOptions=""Center"" />
                <Label StyleClass=""caption1, font-weight-semi-bold, text-interface-medium""
                    Text=""DOWNLOAD"" HorizontalOptions=""Center"" />
                <VerticalStackLayout.GestureRecognizers>
                    <TapGestureRecognizer Command=""{Binding OpenExternalBrowser}""
                        CommandParameter=""{{ epAudio | Escape }}"" />
                </VerticalStackLayout.GestureRecognizers>
            </VerticalStackLayout>

            <VerticalStackLayout Grid.Column=""1"" Spacing=""6"" HorizontalOptions=""Center"">
                <Rock:Icon IconClass=""share-square"" IconFamily=""FontAwesomeSolid"" FontSize=""22""
                    StyleClass=""text-interface-stronger"" HorizontalOptions=""Center"" />
                <Label StyleClass=""caption1, font-weight-semi-bold, text-interface-medium""
                    Text=""SHARE"" HorizontalOptions=""Center"" />
                <VerticalStackLayout.GestureRecognizers>
                    <TapGestureRecognizer Command=""{Binding ShareContent}"">
                        <TapGestureRecognizer.CommandParameter>
                            <Rock:ShareContentParameters
                                Title=""{{ epTitle | Escape }}""
                                Text=""{{ epTitle | Escape }}""
                                Uri=""{{ epAudio | Escape }}"" />
                        </TapGestureRecognizer.CommandParameter>
                    </TapGestureRecognizer>
                </VerticalStackLayout.GestureRecognizers>
            </VerticalStackLayout>
        </Grid>

        {%- if epNotes != '' and epNotes != null -%}
        <VerticalStackLayout Spacing=""10"" StyleClass=""px-16, pt-12"">
            <BoxView HeightRequest=""1"" StyleClass=""bg-interface-softer"" />
            <Rock:Html><![CDATA[{{ epNotes }}]]></Rock:Html>
        </VerticalStackLayout>
        {%- endif -%}
    {%- endif -%}
{% endwebrequest %}
{% endcache %}
</VerticalStackLayout>
" );   // Content
            RockMigrationHelper.AddBlockAttributeValue( "D62AF350-B197-4E3C-98D5-A0314C96E78B", "24516448-3F1F-4F27-97A1-CFB4F8B277B5", @"WebRequest,Cache" );   // EnabledLavaCommands
            RockMigrationHelper.AddBlockAttributeValue( "D62AF350-B197-4E3C-98D5-A0314C96E78B", "B31D29A0-3725-4AEB-8360-7D91B9CDFE47", @"True" );   // DynamicContent
            RockMigrationHelper.AddBlockAttributeValue( "4B81C2D6-5E37-4A90-8F14-6C2093BD75AE", "A93E4436-B226-4911-8C9C-780D9F83C2A5", @"8df04e4b-9abf-477d-8cd2-d36ff06dbdb8|" );   // LandingTemplate
            RockMigrationHelper.AddBlockAttributeValue( "5C92D3E7-6F48-4B01-9025-7D31A4CE86BF", "B89B35C0-EC4C-43B9-89FF-13B67D5EF296", @"2204d103-a145-4003-9da0-1c9461d6baa1|" );   // LandingTemplate
            RockMigrationHelper.AddBlockAttributeValue( "5C92D3E7-6F48-4B01-9025-7D31A4CE86BF", "0894056C-B20F-4C80-9505-9BE289FC86F6", @"6" );   // FutureWeeksToShow
            RockMigrationHelper.AddBlockAttributeValue( "6DA3E4F8-7059-4C12-A136-8E42B5DF97C0", "476529FA-F47B-4AD7-8B8F-77E3BD72F3EA", @"fcfb9f90-9c94-4405-bbf9-df62dc85defd|" );   // TypeTemplate
            RockMigrationHelper.AddBlockAttributeValue( "6DA3E4F8-7059-4C12-A136-8E42B5DF97C0", "9B34B12E-0930-44E6-AE39-FB109E64E8EF", @"False" );   // IsDescriptionRequired

            //
            // Mobile block settings (ProcessLavaOnServer / ProcessLavaOnClient, CssStyles, ...)
            //
            Sql( @"UPDATE [Block] SET [AdditionalSettings] = N'{""ShowOnTablet"":true,""ShowOnPhone"":true,""RequiresNetwork"":true,""NoNetworkContent"":""<VerticalStackLayout Spacing=\""14\"" StyleClass=\""p-24\"" VerticalOptions=\""Center\"">\n    <Rock:Icon IconClass=\""wifi-slash\"" IconFamily=\""FontAwesomeSolid\"" FontSize=\""40\"" StyleClass=\""text-interface-soft\"" HorizontalOptions=\""Center\"" />\n    <Label Text=\""You are offline\"" StyleClass=\""title3, bold, text-interface-strongest\"" HorizontalTextAlignment=\""Center\"" />\n    <Label Text=\""Connect to the internet to load this page.\"" StyleClass=\""body, text-interface-medium\"" HorizontalTextAlignment=\""Center\"" />\n</VerticalStackLayout>"",""CssStyles"":"""",""CacheDuration"":300,""ProcessLavaOnServer"":true,""ProcessLavaOnClient"":false}' WHERE [Guid] = 'D3C3432F-78C9-4891-9AC4-0C0E6329DCB4'" );   // Content
            Sql( @"UPDATE [Block] SET [AdditionalSettings] = N'{""ShowOnTablet"":true,""ShowOnPhone"":true,""RequiresNetwork"":true,""NoNetworkContent"":""<VerticalStackLayout Spacing=\""14\"" StyleClass=\""p-24\"" VerticalOptions=\""Center\"">\n    <Rock:Icon IconClass=\""wifi-slash\"" IconFamily=\""FontAwesomeSolid\"" FontSize=\""40\"" StyleClass=\""text-interface-soft\"" HorizontalOptions=\""Center\"" />\n    <Label Text=\""You are offline\"" StyleClass=\""title3, bold, text-interface-strongest\"" HorizontalTextAlignment=\""Center\"" />\n    <Label Text=\""Connect to the internet to load this page.\"" StyleClass=\""body, text-interface-medium\"" HorizontalTextAlignment=\""Center\"" />\n</VerticalStackLayout>"",""CssStyles"":"""",""CacheDuration"":0,""ProcessLavaOnServer"":false,""ProcessLavaOnClient"":false}' WHERE [Guid] = '85EF791C-9F1A-4EB0-B2FB-DA53256E9848'" );   // Content
            Sql( @"UPDATE [Block] SET [AdditionalSettings] = N'{""ShowOnTablet"":true,""ShowOnPhone"":true,""RequiresNetwork"":true,""NoNetworkContent"":""<VerticalStackLayout Spacing=\""14\"" StyleClass=\""p-24\"" VerticalOptions=\""Center\"">\n    <Rock:Icon IconClass=\""wifi-slash\"" IconFamily=\""FontAwesomeSolid\"" FontSize=\""40\"" StyleClass=\""text-interface-soft\"" HorizontalOptions=\""Center\"" />\n    <Label Text=\""You are offline\"" StyleClass=\""title3, bold, text-interface-strongest\"" HorizontalTextAlignment=\""Center\"" />\n    <Label Text=\""Connect to the internet to load this page.\"" StyleClass=\""body, text-interface-medium\"" HorizontalTextAlignment=\""Center\"" />\n</VerticalStackLayout>"",""CssStyles"":"""",""CacheDuration"":0,""ProcessLavaOnServer"":true,""ProcessLavaOnClient"":false}' WHERE [Guid] = 'DBD2ABC9-945F-457B-8B0F-80F0A87792F0'" );   // Content
            Sql( @"UPDATE [Block] SET [AdditionalSettings] = N'{""ShowOnTablet"":true,""ShowOnPhone"":true,""RequiresNetwork"":true,""NoNetworkContent"":""<VerticalStackLayout Spacing=\""14\"" StyleClass=\""p-24\"" VerticalOptions=\""Center\"">\n    <Rock:Icon IconClass=\""wifi-slash\"" IconFamily=\""FontAwesomeSolid\"" FontSize=\""40\"" StyleClass=\""text-interface-soft\"" HorizontalOptions=\""Center\"" />\n    <Label Text=\""You are offline\"" StyleClass=\""title3, bold, text-interface-strongest\"" HorizontalTextAlignment=\""Center\"" />\n    <Label Text=\""Connect to the internet to load this page.\"" StyleClass=\""body, text-interface-medium\"" HorizontalTextAlignment=\""Center\"" />\n</VerticalStackLayout>"",""CssStyles"":"""",""CacheDuration"":60,""ProcessLavaOnServer"":false,""ProcessLavaOnClient"":false}' WHERE [Guid] = '8B02B9CD-0474-4D79-AEEB-0F91407713CA'" );   // Content Channel Item View
            Sql( @"UPDATE [Block] SET [AdditionalSettings] = N'{""ShowOnTablet"":true,""ShowOnPhone"":true,""RequiresNetwork"":true,""NoNetworkContent"":""<VerticalStackLayout Spacing=\""14\"" StyleClass=\""p-24\"" VerticalOptions=\""Center\"">\n    <Rock:Icon IconClass=\""wifi-slash\"" IconFamily=\""FontAwesomeSolid\"" FontSize=\""40\"" StyleClass=\""text-interface-soft\"" HorizontalOptions=\""Center\"" />\n    <Label Text=\""You are offline\"" StyleClass=\""title3, bold, text-interface-strongest\"" HorizontalTextAlignment=\""Center\"" />\n    <Label Text=\""Connect to the internet to load this page.\"" StyleClass=\""body, text-interface-medium\"" HorizontalTextAlignment=\""Center\"" />\n</VerticalStackLayout>"",""CssStyles"":"""",""CacheDuration"":0,""ProcessLavaOnServer"":false,""ProcessLavaOnClient"":false}' WHERE [Guid] = '783BB975-D313-4722-A444-D3FF6EE06B3B'" );   // Login
            Sql( @"UPDATE [Block] SET [AdditionalSettings] = N'{""ShowOnTablet"":true,""ShowOnPhone"":true,""RequiresNetwork"":true,""NoNetworkContent"":""<VerticalStackLayout Spacing=\""14\"" StyleClass=\""p-24\"" VerticalOptions=\""Center\"">\n    <Rock:Icon IconClass=\""wifi-slash\"" IconFamily=\""FontAwesomeSolid\"" FontSize=\""40\"" StyleClass=\""text-interface-soft\"" HorizontalOptions=\""Center\"" />\n    <Label Text=\""You are offline\"" StyleClass=\""title3, bold, text-interface-strongest\"" HorizontalTextAlignment=\""Center\"" />\n    <Label Text=\""Connect to the internet to load this page.\"" StyleClass=\""body, text-interface-medium\"" HorizontalTextAlignment=\""Center\"" />\n</VerticalStackLayout>"",""CssStyles"":"""",""CacheDuration"":0,""ProcessLavaOnServer"":false,""ProcessLavaOnClient"":false}' WHERE [Guid] = '86FA86D1-936D-4AD3-908B-D07D9A874F1F'" );   // Onboard Person
            Sql( @"UPDATE [Block] SET [AdditionalSettings] = N'{""ShowOnTablet"":true,""ShowOnPhone"":true,""RequiresNetwork"":false,""NoNetworkContent"":"""",""CssStyles"":"""",""CacheDuration"":0,""ProcessLavaOnServer"":true,""ProcessLavaOnClient"":false}' WHERE [Guid] = '545B0BD2-51C8-48D6-A680-57B50D79454C'" );   // Content
            Sql( @"UPDATE [Block] SET [AdditionalSettings] = N'{""ShowOnTablet"":true,""ShowOnPhone"":true,""RequiresNetwork"":true,""NoNetworkContent"":""<VerticalStackLayout Spacing=\""14\"" StyleClass=\""p-24\"" VerticalOptions=\""Center\"">\n    <Rock:Icon IconClass=\""wifi-slash\"" IconFamily=\""FontAwesomeSolid\"" FontSize=\""40\"" StyleClass=\""text-interface-soft\"" HorizontalOptions=\""Center\"" />\n    <Label Text=\""You are offline\"" StyleClass=\""title3, bold, text-interface-strongest\"" HorizontalTextAlignment=\""Center\"" />\n    <Label Text=\""Connect to the internet to load this page.\"" StyleClass=\""body, text-interface-medium\"" HorizontalTextAlignment=\""Center\"" />\n</VerticalStackLayout>"",""CssStyles"":"""",""CacheDuration"":3600,""ProcessLavaOnServer"":true,""ProcessLavaOnClient"":false}' WHERE [Guid] = 'F8DA4BE8-D31D-479F-9306-F9E0CD450A86'" );   // Content
            Sql( @"UPDATE [Block] SET [AdditionalSettings] = N'{""ShowOnTablet"":true,""ShowOnPhone"":true,""RequiresNetwork"":true,""NoNetworkContent"":""<VerticalStackLayout Spacing=\""14\"" StyleClass=\""p-24\"" VerticalOptions=\""Center\"">\n    <Rock:Icon IconClass=\""wifi-slash\"" IconFamily=\""FontAwesomeSolid\"" FontSize=\""40\"" StyleClass=\""text-interface-soft\"" HorizontalOptions=\""Center\"" />\n    <Label Text=\""You are offline\"" StyleClass=\""title3, bold, text-interface-strongest\"" HorizontalTextAlignment=\""Center\"" />\n    <Label Text=\""Connect to the internet to load this page.\"" StyleClass=\""body, text-interface-medium\"" HorizontalTextAlignment=\""Center\"" />\n</VerticalStackLayout>"",""CssStyles"":"""",""CacheDuration"":3600,""ProcessLavaOnServer"":true,""ProcessLavaOnClient"":false}' WHERE [Guid] = 'F147C24C-E1A1-4C41-968E-0F4FABCD3DE6'" );   // Content
            Sql( @"UPDATE [Block] SET [AdditionalSettings] = N'{""ShowOnTablet"":true,""ShowOnPhone"":true,""RequiresNetwork"":true,""NoNetworkContent"":""<VerticalStackLayout Spacing=\""14\"" StyleClass=\""p-24\"" VerticalOptions=\""Center\"">\n    <Rock:Icon IconClass=\""wifi-slash\"" IconFamily=\""FontAwesomeSolid\"" FontSize=\""40\"" StyleClass=\""text-interface-soft\"" HorizontalOptions=\""Center\"" />\n    <Label Text=\""You are offline\"" StyleClass=\""title3, bold, text-interface-strongest\"" HorizontalTextAlignment=\""Center\"" />\n    <Label Text=\""Connect to the internet to load this page.\"" StyleClass=\""body, text-interface-medium\"" HorizontalTextAlignment=\""Center\"" />\n</VerticalStackLayout>"",""CssStyles"":"""",""CacheDuration"":3600,""ProcessLavaOnServer"":true,""ProcessLavaOnClient"":false}' WHERE [Guid] = '7D8C58C0-0FE1-4F17-B651-3BDC3B306423'" );   // Content
            Sql( @"UPDATE [Block] SET [AdditionalSettings] = N'{""ShowOnTablet"":true,""ShowOnPhone"":true,""RequiresNetwork"":true,""NoNetworkContent"":""<VerticalStackLayout Spacing=\""14\"" StyleClass=\""p-24\"" VerticalOptions=\""Center\"">\n    <Rock:Icon IconClass=\""wifi-slash\"" IconFamily=\""FontAwesomeSolid\"" FontSize=\""40\"" StyleClass=\""text-interface-soft\"" HorizontalOptions=\""Center\"" />\n    <Label Text=\""You are offline\"" StyleClass=\""title3, bold, text-interface-strongest\"" HorizontalTextAlignment=\""Center\"" />\n    <Label Text=\""Connect to the internet to load this page.\"" StyleClass=\""body, text-interface-medium\"" HorizontalTextAlignment=\""Center\"" />\n</VerticalStackLayout>"",""CssStyles"":"""",""CacheDuration"":0,""ProcessLavaOnServer"":true,""ProcessLavaOnClient"":false}' WHERE [Guid] = '83D77AF7-E32C-4994-9CEA-9698E5F7BF25'" );   // Content
            Sql( @"UPDATE [Block] SET [AdditionalSettings] = N'{""ShowOnTablet"":true,""ShowOnPhone"":true,""RequiresNetwork"":true,""NoNetworkContent"":""<VerticalStackLayout Spacing=\""14\"" StyleClass=\""p-24\"" VerticalOptions=\""Center\"">\n    <Rock:Icon IconClass=\""wifi-slash\"" IconFamily=\""FontAwesomeSolid\"" FontSize=\""40\"" StyleClass=\""text-interface-soft\"" HorizontalOptions=\""Center\"" />\n    <Label Text=\""You are offline\"" StyleClass=\""title3, bold, text-interface-strongest\"" HorizontalTextAlignment=\""Center\"" />\n    <Label Text=\""Connect to the internet to load this page.\"" StyleClass=\""body, text-interface-medium\"" HorizontalTextAlignment=\""Center\"" />\n</VerticalStackLayout>"",""CssStyles"":"""",""CacheDuration"":0,""ProcessLavaOnServer"":false,""ProcessLavaOnClient"":false}' WHERE [Guid] = '9C074994-EE12-41C8-8072-49A3012A72E8'" );   // Profile Details
            Sql( @"UPDATE [Block] SET [AdditionalSettings] = N'{""ShowOnTablet"":true,""ShowOnPhone"":true,""RequiresNetwork"":true,""NoNetworkContent"":""<VerticalStackLayout Spacing=\""14\"" StyleClass=\""p-24\"" VerticalOptions=\""Center\"">\n    <Rock:Icon IconClass=\""wifi-slash\"" IconFamily=\""FontAwesomeSolid\"" FontSize=\""40\"" StyleClass=\""text-interface-soft\"" HorizontalOptions=\""Center\"" />\n    <Label Text=\""You are offline\"" StyleClass=\""title3, bold, text-interface-strongest\"" HorizontalTextAlignment=\""Center\"" />\n    <Label Text=\""Connect to the internet to load this page.\"" StyleClass=\""body, text-interface-medium\"" HorizontalTextAlignment=\""Center\"" />\n</VerticalStackLayout>"",""CssStyles"":"""",""CacheDuration"":3600,""ProcessLavaOnServer"":true,""ProcessLavaOnClient"":false}' WHERE [Guid] = '5081D904-65B8-46D7-9BF4-602661982712'" );   // Content
            Sql( @"UPDATE [Block] SET [AdditionalSettings] = N'{""ShowOnTablet"":true,""ShowOnPhone"":true,""RequiresNetwork"":true,""NoNetworkContent"":""<VerticalStackLayout Spacing=\""14\"" StyleClass=\""p-24\"" VerticalOptions=\""Center\"">\n    <Rock:Icon IconClass=\""wifi-slash\"" IconFamily=\""FontAwesomeSolid\"" FontSize=\""40\"" StyleClass=\""text-interface-soft\"" HorizontalOptions=\""Center\"" />\n    <Label Text=\""You are offline\"" StyleClass=\""title3, bold, text-interface-strongest\"" HorizontalTextAlignment=\""Center\"" />\n    <Label Text=\""Connect to the internet to load this page.\"" StyleClass=\""body, text-interface-medium\"" HorizontalTextAlignment=\""Center\"" />\n</VerticalStackLayout>"",""CssStyles"":"""",""CacheDuration"":3600,""ProcessLavaOnServer"":true,""ProcessLavaOnClient"":false}' WHERE [Guid] = '4CAF5978-5265-4633-8021-D53E30C318EE'" );   // Content
            Sql( @"UPDATE [Block] SET [AdditionalSettings] = N'{""ShowOnTablet"":true,""ShowOnPhone"":true,""RequiresNetwork"":true,""NoNetworkContent"":""<VerticalStackLayout Spacing=\""14\"" StyleClass=\""p-24\"" VerticalOptions=\""Center\"">\n    <Rock:Icon IconClass=\""wifi-slash\"" IconFamily=\""FontAwesomeSolid\"" FontSize=\""40\"" StyleClass=\""text-interface-soft\"" HorizontalOptions=\""Center\"" />\n    <Label Text=\""You are offline\"" StyleClass=\""title3, bold, text-interface-strongest\"" HorizontalTextAlignment=\""Center\"" />\n    <Label Text=\""Connect to the internet to load this page.\"" StyleClass=\""body, text-interface-medium\"" HorizontalTextAlignment=\""Center\"" />\n</VerticalStackLayout>"",""CssStyles"":"""",""CacheDuration"":0,""ProcessLavaOnServer"":false,""ProcessLavaOnClient"":false}' WHERE [Guid] = 'E44BCD5E-AFA1-42D2-8371-0041ADB65CC5'" );   // Workflow Entry
            Sql( @"UPDATE [Block] SET [AdditionalSettings] = N'{""ShowOnTablet"":true,""ShowOnPhone"":true,""RequiresNetwork"":true,""NoNetworkContent"":""<VerticalStackLayout Spacing=\""14\"" StyleClass=\""p-24\"" VerticalOptions=\""Center\"">\n    <Rock:Icon IconClass=\""wifi-slash\"" IconFamily=\""FontAwesomeSolid\"" FontSize=\""40\"" StyleClass=\""text-interface-soft\"" HorizontalOptions=\""Center\"" />\n    <Label Text=\""You are offline\"" StyleClass=\""title3, bold, text-interface-strongest\"" HorizontalTextAlignment=\""Center\"" />\n    <Label Text=\""Connect to the internet to load this page.\"" StyleClass=\""body, text-interface-medium\"" HorizontalTextAlignment=\""Center\"" />\n</VerticalStackLayout>"",""CssStyles"":"""",""CacheDuration"":3600,""ProcessLavaOnServer"":true,""ProcessLavaOnClient"":false}' WHERE [Guid] = '0AF332B5-1F4C-4D17-B3C6-91C899A6C6FC'" );   // Content
            Sql( @"UPDATE [Block] SET [AdditionalSettings] = N'{""ShowOnTablet"":true,""ShowOnPhone"":true,""RequiresNetwork"":true,""NoNetworkContent"":""<VerticalStackLayout Spacing=\""14\"" StyleClass=\""p-24\"" VerticalOptions=\""Center\"">\n    <Rock:Icon IconClass=\""wifi-slash\"" IconFamily=\""FontAwesomeSolid\"" FontSize=\""40\"" StyleClass=\""text-interface-soft\"" HorizontalOptions=\""Center\"" />\n    <Label Text=\""You are offline\"" StyleClass=\""title3, bold, text-interface-strongest\"" HorizontalTextAlignment=\""Center\"" />\n    <Label Text=\""Connect to the internet to load this page.\"" StyleClass=\""body, text-interface-medium\"" HorizontalTextAlignment=\""Center\"" />\n</VerticalStackLayout>"",""CssStyles"":"""",""CacheDuration"":0,""ProcessLavaOnServer"":false,""ProcessLavaOnClient"":false}' WHERE [Guid] = '82791881-D9E4-48E1-9844-A84CFDC78955'" );   // Check-in
            Sql( @"UPDATE [Block] SET [AdditionalSettings] = N'{""ShowOnTablet"":true,""ShowOnPhone"":true,""RequiresNetwork"":true,""NoNetworkContent"":""<VerticalStackLayout Spacing=\""14\"" StyleClass=\""p-24\"" VerticalOptions=\""Center\"">\n    <Rock:Icon IconClass=\""wifi-slash\"" IconFamily=\""FontAwesomeSolid\"" FontSize=\""40\"" StyleClass=\""text-interface-soft\"" HorizontalOptions=\""Center\"" />\n    <Label Text=\""You are offline\"" StyleClass=\""title3, bold, text-interface-strongest\"" HorizontalTextAlignment=\""Center\"" />\n    <Label Text=\""Connect to the internet to load this page.\"" StyleClass=\""body, text-interface-medium\"" HorizontalTextAlignment=\""Center\"" />\n</VerticalStackLayout>"",""CssStyles"":"""",""CacheDuration"":0,""ProcessLavaOnServer"":false,""ProcessLavaOnClient"":false}' WHERE [Guid] = 'BEBEC594-4C65-411E-8013-BAC2983D2DD8'" );   // Calendar View
            Sql( @"UPDATE [Block] SET [AdditionalSettings] = N'{""ShowOnTablet"":true,""ShowOnPhone"":true,""RequiresNetwork"":true,""NoNetworkContent"":""<VerticalStackLayout Spacing=\""14\"" StyleClass=\""p-24\"" VerticalOptions=\""Center\"">\n    <Rock:Icon IconClass=\""wifi-slash\"" IconFamily=\""FontAwesomeSolid\"" FontSize=\""40\"" StyleClass=\""text-interface-soft\"" HorizontalOptions=\""Center\"" />\n    <Label Text=\""You are offline\"" StyleClass=\""title3, bold, text-interface-strongest\"" HorizontalTextAlignment=\""Center\"" />\n    <Label Text=\""Connect to the internet to load this page.\"" StyleClass=\""body, text-interface-medium\"" HorizontalTextAlignment=\""Center\"" />\n</VerticalStackLayout>"",""CssStyles"":"""",""CacheDuration"":300,""ProcessLavaOnServer"":false,""ProcessLavaOnClient"":false}' WHERE [Guid] = 'B36C1077-5312-44E3-ACFF-DCB32EC72B4A'" );   // Calendar Event Item Occurrence View
            Sql( @"UPDATE [Block] SET [AdditionalSettings] = N'{""ShowOnTablet"":true,""ShowOnPhone"":true,""RequiresNetwork"":true,""NoNetworkContent"":""<VerticalStackLayout Spacing=\""14\"" StyleClass=\""p-24\"" VerticalOptions=\""Center\"">\n    <Rock:Icon IconClass=\""wifi-slash\"" IconFamily=\""FontAwesomeSolid\"" FontSize=\""40\"" StyleClass=\""text-interface-soft\"" HorizontalOptions=\""Center\"" />\n    <Label Text=\""You are offline\"" StyleClass=\""title3, bold, text-interface-strongest\"" HorizontalTextAlignment=\""Center\"" />\n    <Label Text=\""Connect to the internet to load this page.\"" StyleClass=\""body, text-interface-medium\"" HorizontalTextAlignment=\""Center\"" />\n</VerticalStackLayout>"",""CssStyles"":"""",""CacheDuration"":0,""ProcessLavaOnServer"":false,""ProcessLavaOnClient"":false}' WHERE [Guid] = '7B49DE66-B707-417F-8A65-A082508A548B'" );   // Schedule Toolbox
            Sql( @"UPDATE [Block] SET [AdditionalSettings] = N'{""ShowOnTablet"":true,""ShowOnPhone"":true,""RequiresNetwork"":true,""NoNetworkContent"":""<VerticalStackLayout Spacing=\""14\"" StyleClass=\""p-24\"" VerticalOptions=\""Center\"">\n    <Rock:Icon IconClass=\""wifi-slash\"" IconFamily=\""FontAwesomeSolid\"" FontSize=\""40\"" StyleClass=\""text-interface-soft\"" HorizontalOptions=\""Center\"" />\n    <Label Text=\""You are offline\"" StyleClass=\""title3, bold, text-interface-strongest\"" HorizontalTextAlignment=\""Center\"" />\n    <Label Text=\""Connect to the internet to load this page.\"" StyleClass=\""body, text-interface-medium\"" HorizontalTextAlignment=\""Center\"" />\n</VerticalStackLayout>"",""CssStyles"":"""",""CacheDuration"":0,""ProcessLavaOnServer"":true,""ProcessLavaOnClient"":false}' WHERE [Guid] = '5E4FFC63-BAA6-4676-AD4C-3C7E0034E0BD'" );   // Content
            Sql( @"UPDATE [Block] SET [AdditionalSettings] = N'{""ShowOnTablet"":true,""ShowOnPhone"":true,""RequiresNetwork"":true,""NoNetworkContent"":""<VerticalStackLayout Spacing=\""14\"" StyleClass=\""p-24\"" VerticalOptions=\""Center\"">\n    <Rock:Icon IconClass=\""wifi-slash\"" IconFamily=\""FontAwesomeSolid\"" FontSize=\""40\"" StyleClass=\""text-interface-soft\"" HorizontalOptions=\""Center\"" />\n    <Label Text=\""You are offline\"" StyleClass=\""title3, bold, text-interface-strongest\"" HorizontalTextAlignment=\""Center\"" />\n    <Label Text=\""Connect to the internet to load this page.\"" StyleClass=\""body, text-interface-medium\"" HorizontalTextAlignment=\""Center\"" />\n</VerticalStackLayout>"",""CssStyles"":"""",""CacheDuration"":0,""ProcessLavaOnServer"":false,""ProcessLavaOnClient"":false}' WHERE [Guid] = '98963BBF-5F11-4B6B-ACCA-ECFFFDB96480'" );   // Group View
            Sql( @"UPDATE [Block] SET [AdditionalSettings] = N'{""ShowOnTablet"":true,""ShowOnPhone"":true,""RequiresNetwork"":true,""NoNetworkContent"":""<VerticalStackLayout Spacing=\""14\"" StyleClass=\""p-24\"" VerticalOptions=\""Center\"">\n    <Rock:Icon IconClass=\""wifi-slash\"" IconFamily=\""FontAwesomeSolid\"" FontSize=\""40\"" StyleClass=\""text-interface-soft\"" HorizontalOptions=\""Center\"" />\n    <Label Text=\""You are offline\"" StyleClass=\""title3, bold, text-interface-strongest\"" HorizontalTextAlignment=\""Center\"" />\n    <Label Text=\""Connect to the internet to load this page.\"" StyleClass=\""body, text-interface-medium\"" HorizontalTextAlignment=\""Center\"" />\n</VerticalStackLayout>"",""CssStyles"":"""",""CacheDuration"":0,""ProcessLavaOnServer"":false,""ProcessLavaOnClient"":false}' WHERE [Guid] = '597C9DB9-24E6-4FBF-A97D-A21A32F3B81D'" );   // Group Member List
            Sql( @"UPDATE [Block] SET [AdditionalSettings] = N'{""ShowOnTablet"":true,""ShowOnPhone"":true,""RequiresNetwork"":true,""NoNetworkContent"":""<VerticalStackLayout Spacing=\""14\"" StyleClass=\""p-24\"" VerticalOptions=\""Center\"">\n    <Rock:Icon IconClass=\""wifi-slash\"" IconFamily=\""FontAwesomeSolid\"" FontSize=\""40\"" StyleClass=\""text-interface-soft\"" HorizontalOptions=\""Center\"" />\n    <Label Text=\""You are offline\"" StyleClass=\""title3, bold, text-interface-strongest\"" HorizontalTextAlignment=\""Center\"" />\n    <Label Text=\""Connect to the internet to load this page.\"" StyleClass=\""body, text-interface-medium\"" HorizontalTextAlignment=\""Center\"" />\n</VerticalStackLayout>"",""CssStyles"":"""",""CacheDuration"":3600,""ProcessLavaOnServer"":true,""ProcessLavaOnClient"":false}' WHERE [Guid] = 'A5F5F565-6542-47B4-8FD8-642B3CC3E7C6'" );   // Content
            Sql( @"UPDATE [Block] SET [AdditionalSettings] = N'{""ShowOnTablet"":true,""ShowOnPhone"":true,""RequiresNetwork"":true,""NoNetworkContent"":""<VerticalStackLayout Spacing=\""14\"" StyleClass=\""p-24\"" VerticalOptions=\""Center\"">\n    <Rock:Icon IconClass=\""wifi-slash\"" IconFamily=\""FontAwesomeSolid\"" FontSize=\""40\"" StyleClass=\""text-interface-soft\"" HorizontalOptions=\""Center\"" />\n    <Label Text=\""You are offline\"" StyleClass=\""title3, bold, text-interface-strongest\"" HorizontalTextAlignment=\""Center\"" />\n    <Label Text=\""Connect to the internet to load this page.\"" StyleClass=\""body, text-interface-medium\"" HorizontalTextAlignment=\""Center\"" />\n</VerticalStackLayout>"",""CssStyles"":"""",""CacheDuration"":3600,""ProcessLavaOnServer"":false,""ProcessLavaOnClient"":false}' WHERE [Guid] = '00AF4314-F34E-4AEF-ADF6-462182AF8D89'" );   // Content Channel Item View
            Sql( @"UPDATE [Block] SET [AdditionalSettings] = N'{""ShowOnTablet"":true,""ShowOnPhone"":true,""RequiresNetwork"":true,""NoNetworkContent"":""<VerticalStackLayout Spacing=\""14\"" StyleClass=\""p-24\"" VerticalOptions=\""Center\"">\n    <Rock:Icon IconClass=\""wifi-slash\"" IconFamily=\""FontAwesomeSolid\"" FontSize=\""40\"" StyleClass=\""text-interface-soft\"" HorizontalOptions=\""Center\"" />\n    <Label Text=\""You are offline\"" StyleClass=\""title3, bold, text-interface-strongest\"" HorizontalTextAlignment=\""Center\"" />\n    <Label Text=\""Connect to the internet to load this page.\"" StyleClass=\""body, text-interface-medium\"" HorizontalTextAlignment=\""Center\"" />\n</VerticalStackLayout>"",""CssStyles"":"""",""CacheDuration"":3600,""ProcessLavaOnServer"":true,""ProcessLavaOnClient"":false}' WHERE [Guid] = 'E430DB8D-53B7-432A-BF3D-28D590295FE1'" );   // Content
            Sql( @"UPDATE [Block] SET [AdditionalSettings] = N'{""ShowOnTablet"":true,""ShowOnPhone"":true,""RequiresNetwork"":true,""NoNetworkContent"":""<VerticalStackLayout Spacing=\""14\"" StyleClass=\""p-24\"" VerticalOptions=\""Center\"">\n    <Rock:Icon IconClass=\""wifi-slash\"" IconFamily=\""FontAwesomeSolid\"" FontSize=\""40\"" StyleClass=\""text-interface-soft\"" HorizontalOptions=\""Center\"" />\n    <Label Text=\""You are offline\"" StyleClass=\""title3, bold, text-interface-strongest\"" HorizontalTextAlignment=\""Center\"" />\n    <Label Text=\""Connect to the internet to load this page.\"" StyleClass=\""body, text-interface-medium\"" HorizontalTextAlignment=\""Center\"" />\n</VerticalStackLayout>"",""CssStyles"":"""",""CacheDuration"":3600,""ProcessLavaOnServer"":false,""ProcessLavaOnClient"":false}' WHERE [Guid] = 'C63AD407-B2D3-4DA5-BC49-B34DF3554EE1'" );   // Content Channel Item View
            Sql( @"UPDATE [Block] SET [AdditionalSettings] = N'{""ShowOnTablet"":true,""ShowOnPhone"":true,""RequiresNetwork"":true,""NoNetworkContent"":""<VerticalStackLayout Spacing=\""14\"" StyleClass=\""p-24\"" VerticalOptions=\""Center\"">\n    <Rock:Icon IconClass=\""wifi-slash\"" IconFamily=\""FontAwesomeSolid\"" FontSize=\""40\"" StyleClass=\""text-interface-soft\"" HorizontalOptions=\""Center\"" />\n    <Label Text=\""You are offline\"" StyleClass=\""title3, bold, text-interface-strongest\"" HorizontalTextAlignment=\""Center\"" />\n    <Label Text=\""Connect to the internet to load this page.\"" StyleClass=\""body, text-interface-medium\"" HorizontalTextAlignment=\""Center\"" />\n</VerticalStackLayout>"",""CssStyles"":"""",""CacheDuration"":0,""ProcessLavaOnServer"":false,""ProcessLavaOnClient"":false}' WHERE [Guid] = 'A67C5D2C-1161-4EBA-8F92-93962EF739F6'" );   // Content Channel Item View
            Sql( @"UPDATE [Block] SET [AdditionalSettings] = N'{""ShowOnTablet"":true,""ShowOnPhone"":true,""RequiresNetwork"":true,""NoNetworkContent"":""<VerticalStackLayout Spacing=\""14\"" StyleClass=\""p-24\"" VerticalOptions=\""Center\"">\n    <Rock:Icon IconClass=\""wifi-slash\"" IconFamily=\""FontAwesomeSolid\"" FontSize=\""40\"" StyleClass=\""text-interface-soft\"" HorizontalOptions=\""Center\"" />\n    <Label Text=\""You are offline\"" StyleClass=\""title3, bold, text-interface-strongest\"" HorizontalTextAlignment=\""Center\"" />\n    <Label Text=\""Connect to the internet to load this page.\"" StyleClass=\""body, text-interface-medium\"" HorizontalTextAlignment=\""Center\"" />\n</VerticalStackLayout>"",""CssStyles"":"""",""CacheDuration"":0,""ProcessLavaOnServer"":true,""ProcessLavaOnClient"":false}' WHERE [Guid] = 'C3E6A9C2-EBA4-4AF9-A37B-3E7F8CFE0DF4'" );   // Content
            Sql( @"UPDATE [Block] SET [AdditionalSettings] = N'{""ShowOnTablet"":true,""ShowOnPhone"":true,""RequiresNetwork"":true,""NoNetworkContent"":""<VerticalStackLayout Spacing=\""14\"" StyleClass=\""p-24\"" VerticalOptions=\""Center\"">\n    <Rock:Icon IconClass=\""wifi-slash\"" IconFamily=\""FontAwesomeSolid\"" FontSize=\""40\"" StyleClass=\""text-interface-soft\"" HorizontalOptions=\""Center\"" />\n    <Label Text=\""You are offline\"" StyleClass=\""title3, bold, text-interface-strongest\"" HorizontalTextAlignment=\""Center\"" />\n    <Label Text=\""Connect to the internet to load this page.\"" StyleClass=\""body, text-interface-medium\"" HorizontalTextAlignment=\""Center\"" />\n</VerticalStackLayout>"",""CssStyles"":"""",""CacheDuration"":0,""ProcessLavaOnServer"":true,""ProcessLavaOnClient"":false}' WHERE [Guid] = '5166C7A4-C480-4F93-8A1F-634775438974'" );   // Content
            Sql( @"UPDATE [Block] SET [AdditionalSettings] = N'{""ShowOnTablet"":true,""ShowOnPhone"":true,""RequiresNetwork"":true,""NoNetworkContent"":""<VerticalStackLayout Spacing=\""14\"" StyleClass=\""p-24\"" VerticalOptions=\""Center\"">\n    <Rock:Icon IconClass=\""wifi-slash\"" IconFamily=\""FontAwesomeSolid\"" FontSize=\""40\"" StyleClass=\""text-interface-soft\"" HorizontalOptions=\""Center\"" />\n    <Label Text=\""You are offline\"" StyleClass=\""title3, bold, text-interface-strongest\"" HorizontalTextAlignment=\""Center\"" />\n    <Label Text=\""Connect to the internet to load this page.\"" StyleClass=\""body, text-interface-medium\"" HorizontalTextAlignment=\""Center\"" />\n</VerticalStackLayout>"",""CssStyles"":"""",""CacheDuration"":0,""ProcessLavaOnServer"":true,""ProcessLavaOnClient"":false}' WHERE [Guid] = '78C66DE9-AE93-4B11-9667-B87EF00A1C4C'" );   // Content
            Sql( @"UPDATE [Block] SET [AdditionalSettings] = N'{""ShowOnTablet"":true,""ShowOnPhone"":true,""RequiresNetwork"":true,""NoNetworkContent"":""<VerticalStackLayout Spacing=\""14\"" StyleClass=\""p-24\"" VerticalOptions=\""Center\"">\n    <Rock:Icon IconClass=\""wifi-slash\"" IconFamily=\""FontAwesomeSolid\"" FontSize=\""40\"" StyleClass=\""text-interface-soft\"" HorizontalOptions=\""Center\"" />\n    <Label Text=\""You are offline\"" StyleClass=\""title3, bold, text-interface-strongest\"" HorizontalTextAlignment=\""Center\"" />\n    <Label Text=\""Connect to the internet to load this page.\"" StyleClass=\""body, text-interface-medium\"" HorizontalTextAlignment=\""Center\"" />\n</VerticalStackLayout>"",""CssStyles"":"""",""CacheDuration"":60,""ProcessLavaOnServer"":true,""ProcessLavaOnClient"":false}' WHERE [Guid] = '8380B1DB-4D13-47D5-B290-D8D98E6FB4BD'" );   // Content
            Sql( @"UPDATE [Block] SET [AdditionalSettings] = N'{""ShowOnTablet"":true,""ShowOnPhone"":true,""RequiresNetwork"":true,""NoNetworkContent"":""<VerticalStackLayout Spacing=\""14\"" StyleClass=\""p-24\"" VerticalOptions=\""Center\"">\n    <Rock:Icon IconClass=\""wifi-slash\"" IconFamily=\""FontAwesomeSolid\"" FontSize=\""40\"" StyleClass=\""text-interface-soft\"" HorizontalOptions=\""Center\"" />\n    <Label Text=\""You are offline\"" StyleClass=\""title3, bold, text-interface-strongest\"" HorizontalTextAlignment=\""Center\"" />\n    <Label Text=\""Connect to the internet to load this page.\"" StyleClass=\""body, text-interface-medium\"" HorizontalTextAlignment=\""Center\"" />\n</VerticalStackLayout>"",""CssStyles"":"""",""CacheDuration"":3600,""ProcessLavaOnServer"":true,""ProcessLavaOnClient"":false}' WHERE [Guid] = 'FF2703A4-BF7D-4A53-A214-10AD2E850BAA'" );   // Content
            Sql( @"UPDATE [Block] SET [AdditionalSettings] = N'{""ShowOnTablet"":true,""ShowOnPhone"":true,""RequiresNetwork"":true,""NoNetworkContent"":""<VerticalStackLayout Spacing=\""14\"" StyleClass=\""p-24\"" VerticalOptions=\""Center\"">\n    <Rock:Icon IconClass=\""wifi-slash\"" IconFamily=\""FontAwesomeSolid\"" FontSize=\""40\"" StyleClass=\""text-interface-soft\"" HorizontalOptions=\""Center\"" />\n    <Label Text=\""You are offline\"" StyleClass=\""title3, bold, text-interface-strongest\"" HorizontalTextAlignment=\""Center\"" />\n    <Label Text=\""Connect to the internet to load this page.\"" StyleClass=\""body, text-interface-medium\"" HorizontalTextAlignment=\""Center\"" />\n</VerticalStackLayout>"",""CssStyles"":"""",""CacheDuration"":0,""ProcessLavaOnServer"":false,""ProcessLavaOnClient"":false}' WHERE [Guid] = 'DDA29EBD-B8A0-44FD-A1A4-2E783F050005'" );   // Communication View
            Sql( @"UPDATE [Block] SET [AdditionalSettings] = N'{""ShowOnTablet"":true,""ShowOnPhone"":true,""RequiresNetwork"":true,""NoNetworkContent"":""<VerticalStackLayout Spacing=\""14\"" StyleClass=\""p-24\"" VerticalOptions=\""Center\"">\n    <Rock:Icon IconClass=\""wifi-slash\"" IconFamily=\""FontAwesomeSolid\"" FontSize=\""40\"" StyleClass=\""text-interface-soft\"" HorizontalOptions=\""Center\"" />\n    <Label Text=\""You are offline\"" StyleClass=\""title3, bold, text-interface-strongest\"" HorizontalTextAlignment=\""Center\"" />\n    <Label Text=\""Connect to the internet to load this page.\"" StyleClass=\""body, text-interface-medium\"" HorizontalTextAlignment=\""Center\"" />\n</VerticalStackLayout>"",""CssStyles"":"""",""CacheDuration"":0,""ProcessLavaOnServer"":false,""ProcessLavaOnClient"":false}' WHERE [Guid] = 'DE95662D-6AD7-47F3-92F7-7851C3E9E6E8'" );   // Communication List Subscribe
            Sql( @"UPDATE [Block] SET [AdditionalSettings] = N'{""ShowOnTablet"":true,""ShowOnPhone"":true,""RequiresNetwork"":false,""NoNetworkContent"":"""",""CssStyles"":"""",""CacheDuration"":0,""ProcessLavaOnServer"":false,""ProcessLavaOnClient"":false}' WHERE [Guid] = '05137346-7182-43EC-B7BD-581237869417'" );   // Content
            Sql( @"UPDATE [Block] SET [AdditionalSettings] = N'{""ShowOnTablet"":true,""ShowOnPhone"":true,""RequiresNetwork"":false,""NoNetworkContent"":"""",""CssStyles"":"""",""CacheDuration"":0,""ProcessLavaOnServer"":false,""ProcessLavaOnClient"":false}' WHERE [Guid] = '65350FBF-7EDA-4D89-9778-6020D53B785F'" );   // Content
            Sql( @"UPDATE [Block] SET [AdditionalSettings] = N'{""ShowOnTablet"":true,""ShowOnPhone"":true,""RequiresNetwork"":false,""NoNetworkContent"":"""",""CssStyles"":"""",""CacheDuration"":0,""ProcessLavaOnServer"":false,""ProcessLavaOnClient"":false}' WHERE [Guid] = '81E84E8F-4FC6-4D89-997E-8BBF1A7B2E05'" );   // Content
            Sql( @"UPDATE [Block] SET [AdditionalSettings] = N'{""ShowOnTablet"":true,""ShowOnPhone"":true,""RequiresNetwork"":false,""NoNetworkContent"":"""",""CssStyles"":"""",""CacheDuration"":0,""ProcessLavaOnServer"":false,""ProcessLavaOnClient"":false}' WHERE [Guid] = 'BD66F886-936E-4624-AEF5-00698C7C0BFC'" );   // Content
            Sql( @"UPDATE [Block] SET [AdditionalSettings] = N'{""ShowOnTablet"":true,""ShowOnPhone"":true,""RequiresNetwork"":true,""NoNetworkContent"":""<VerticalStackLayout Spacing=\""14\"" StyleClass=\""p-24\"" VerticalOptions=\""Center\"">\n    <Rock:Icon IconClass=\""wifi-slash\"" IconFamily=\""FontAwesomeSolid\"" FontSize=\""40\"" StyleClass=\""text-interface-soft\"" HorizontalOptions=\""Center\"" />\n    <Label Text=\""You are offline\"" StyleClass=\""title3, bold, text-interface-strongest\"" HorizontalTextAlignment=\""Center\"" />\n    <Label Text=\""Connect to the internet to load this page.\"" StyleClass=\""body, text-interface-medium\"" HorizontalTextAlignment=\""Center\"" />\n</VerticalStackLayout>"",""CssStyles"":"""",""CacheDuration"":0,""ProcessLavaOnServer"":true,""ProcessLavaOnClient"":false}' WHERE [Guid] = 'AC2DDABC-C82A-4827-8565-248724D1C324'" );   // Content
            Sql( @"UPDATE [Block] SET [AdditionalSettings] = N'{""ShowOnTablet"":true,""ShowOnPhone"":true,""RequiresNetwork"":true,""NoNetworkContent"":""<VerticalStackLayout Spacing=\""14\"" StyleClass=\""p-24\"" VerticalOptions=\""Center\"">\n    <Rock:Icon IconClass=\""wifi-slash\"" IconFamily=\""FontAwesomeSolid\"" FontSize=\""40\"" StyleClass=\""text-interface-soft\"" HorizontalOptions=\""Center\"" />\n    <Label Text=\""You are offline\"" StyleClass=\""title3, bold, text-interface-strongest\"" HorizontalTextAlignment=\""Center\"" />\n    <Label Text=\""Connect to the internet to load this page.\"" StyleClass=\""body, text-interface-medium\"" HorizontalTextAlignment=\""Center\"" />\n</VerticalStackLayout>"",""CssStyles"":"""",""CacheDuration"":0,""ProcessLavaOnServer"":true,""ProcessLavaOnClient"":true}' WHERE [Guid] = '10E07AFD-C2EB-4E97-9121-027811648F4B'" );   // Content - Not logged in
            Sql( @"UPDATE [Block] SET [AdditionalSettings] = N'{""ShowOnTablet"":true,""ShowOnPhone"":true,""RequiresNetwork"":true,""NoNetworkContent"":""<VerticalStackLayout Spacing=\""14\"" StyleClass=\""p-24\"" VerticalOptions=\""Center\"">\n    <Rock:Icon IconClass=\""wifi-slash\"" IconFamily=\""FontAwesomeSolid\"" FontSize=\""40\"" StyleClass=\""text-interface-soft\"" HorizontalOptions=\""Center\"" />\n    <Label Text=\""You are offline\"" StyleClass=\""title3, bold, text-interface-strongest\"" HorizontalTextAlignment=\""Center\"" />\n    <Label Text=\""Connect to the internet to load this page.\"" StyleClass=\""body, text-interface-medium\"" HorizontalTextAlignment=\""Center\"" />\n</VerticalStackLayout>"",""CssStyles"":"""",""CacheDuration"":0,""ProcessLavaOnServer"":true,""ProcessLavaOnClient"":false}' WHERE [Guid] = '77A80F5F-CED1-4471-BCE3-1F405BE29C6B'" );   // Content - Not signed in
            Sql( @"UPDATE [Block] SET [AdditionalSettings] = N'{""ShowOnTablet"":true,""ShowOnPhone"":true,""RequiresNetwork"":true,""NoNetworkContent"":""<VerticalStackLayout Spacing=\""14\"" StyleClass=\""p-24\"" VerticalOptions=\""Center\"">\n    <Rock:Icon IconClass=\""wifi-slash\"" IconFamily=\""FontAwesomeSolid\"" FontSize=\""40\"" StyleClass=\""text-interface-soft\"" HorizontalOptions=\""Center\"" />\n    <Label Text=\""You are offline\"" StyleClass=\""title3, bold, text-interface-strongest\"" HorizontalTextAlignment=\""Center\"" />\n    <Label Text=\""Connect to the internet to load this page.\"" StyleClass=\""body, text-interface-medium\"" HorizontalTextAlignment=\""Center\"" />\n</VerticalStackLayout>"",""CssStyles"":"""",""CacheDuration"":0,""ProcessLavaOnServer"":false,""ProcessLavaOnClient"":false}' WHERE [Guid] = '4687C5D1-BEAE-48F7-AB47-C5F723369EB6'" );   // Workflow Entry
            Sql( @"UPDATE [Block] SET [AdditionalSettings] = N'{""ShowOnTablet"":true,""ShowOnPhone"":true,""RequiresNetwork"":true,""NoNetworkContent"":""<VerticalStackLayout Spacing=\""14\"" StyleClass=\""p-24\"" VerticalOptions=\""Center\"">\n    <Rock:Icon IconClass=\""wifi-slash\"" IconFamily=\""FontAwesomeSolid\"" FontSize=\""40\"" StyleClass=\""text-interface-soft\"" HorizontalOptions=\""Center\"" />\n    <Label Text=\""You are offline\"" StyleClass=\""title3, bold, text-interface-strongest\"" HorizontalTextAlignment=\""Center\"" />\n    <Label Text=\""Connect to the internet to load this page.\"" StyleClass=\""body, text-interface-medium\"" HorizontalTextAlignment=\""Center\"" />\n</VerticalStackLayout>"",""CssStyles"":"""",""CacheDuration"":0,""ProcessLavaOnServer"":true,""ProcessLavaOnClient"":false}' WHERE [Guid] = '3B3985B1-5A10-441C-94DD-C83FA15B5579'" );   // Intro
            Sql( @"UPDATE [Block] SET [AdditionalSettings] = N'{""ShowOnTablet"":true,""ShowOnPhone"":true,""RequiresNetwork"":true,""NoNetworkContent"":""<VerticalStackLayout Spacing=\""14\"" StyleClass=\""p-24\"" VerticalOptions=\""Center\"">\n    <Rock:Icon IconClass=\""wifi-slash\"" IconFamily=\""FontAwesomeSolid\"" FontSize=\""40\"" StyleClass=\""text-interface-soft\"" HorizontalOptions=\""Center\"" />\n    <Label Text=\""You are offline\"" StyleClass=\""title3, bold, text-interface-strongest\"" HorizontalTextAlignment=\""Center\"" />\n    <Label Text=\""Connect to the internet to load this page.\"" StyleClass=\""body, text-interface-medium\"" HorizontalTextAlignment=\""Center\"" />\n</VerticalStackLayout>"",""CssStyles"":"""",""CacheDuration"":0,""ProcessLavaOnServer"":false,""ProcessLavaOnClient"":false}' WHERE [Guid] = 'E06F0FFF-4020-4B11-9F1E-038724978A34'" );   // Group Attendance Entry
            Sql( @"UPDATE [Block] SET [AdditionalSettings] = N'{""ShowOnTablet"":true,""ShowOnPhone"":true,""RequiresNetwork"":true,""NoNetworkContent"":""<VerticalStackLayout Spacing=\""14\"" StyleClass=\""p-24\"" VerticalOptions=\""Center\"">\n    <Rock:Icon IconClass=\""wifi-slash\"" IconFamily=\""FontAwesomeSolid\"" FontSize=\""40\"" StyleClass=\""text-interface-soft\"" HorizontalOptions=\""Center\"" />\n    <Label Text=\""You are offline\"" StyleClass=\""title3, bold, text-interface-strongest\"" HorizontalTextAlignment=\""Center\"" />\n    <Label Text=\""Connect to the internet to load this page.\"" StyleClass=\""body, text-interface-medium\"" HorizontalTextAlignment=\""Center\"" />\n</VerticalStackLayout>"",""CssStyles"":"""",""CacheDuration"":0,""ProcessLavaOnServer"":true,""ProcessLavaOnClient"":false}' WHERE [Guid] = '8830E449-A6B7-42B4-8A24-3E847E750502'" );   // Group Header
            Sql( @"UPDATE [Block] SET [AdditionalSettings] = N'{""ShowOnTablet"":true,""ShowOnPhone"":true,""RequiresNetwork"":true,""NoNetworkContent"":""<VerticalStackLayout Spacing=\""14\"" StyleClass=\""p-24\"" VerticalOptions=\""Center\"">\n    <Rock:Icon IconClass=\""wifi-slash\"" IconFamily=\""FontAwesomeSolid\"" FontSize=\""40\"" StyleClass=\""text-interface-soft\"" HorizontalOptions=\""Center\"" />\n    <Label Text=\""You are offline\"" StyleClass=\""title3, bold, text-interface-strongest\"" HorizontalTextAlignment=\""Center\"" />\n    <Label Text=\""Connect to the internet to load this page.\"" StyleClass=\""body, text-interface-medium\"" HorizontalTextAlignment=\""Center\"" />\n</VerticalStackLayout>"",""CssStyles"":"""",""CacheDuration"":0,""ProcessLavaOnServer"":true,""ProcessLavaOnClient"":false}' WHERE [Guid] = 'F9B0195A-585F-4706-99E5-29DE2A392666'" );   // Messages List
            Sql( @"UPDATE [Block] SET [AdditionalSettings] = N'{""ShowOnTablet"":true,""ShowOnPhone"":true,""RequiresNetwork"":true,""NoNetworkContent"":""<VerticalStackLayout Spacing=\""14\"" StyleClass=\""p-24\"" VerticalOptions=\""Center\"">\n    <Rock:Icon IconClass=\""wifi-slash\"" IconFamily=\""FontAwesomeSolid\"" FontSize=\""40\"" StyleClass=\""text-interface-soft\"" HorizontalOptions=\""Center\"" />\n    <Label Text=\""You are offline\"" StyleClass=\""title3, bold, text-interface-strongest\"" HorizontalTextAlignment=\""Center\"" />\n    <Label Text=\""Connect to the internet to load this page.\"" StyleClass=\""body, text-interface-medium\"" HorizontalTextAlignment=\""Center\"" />\n</VerticalStackLayout>"",""CssStyles"":"""",""CacheDuration"":0,""ProcessLavaOnServer"":true,""ProcessLavaOnClient"":false}' WHERE [Guid] = 'B3FC2629-4E9C-403F-AA52-4CDBBE9AC126'" );   // Message Detail
            Sql( @"UPDATE [Block] SET [AdditionalSettings] = N'{""ShowOnTablet"":true,""ShowOnPhone"":true,""RequiresNetwork"":true,""NoNetworkContent"":""<VerticalStackLayout Spacing=\""14\"" StyleClass=\""p-24\"" VerticalOptions=\""Center\"">\n    <Rock:Icon IconClass=\""wifi-slash\"" IconFamily=\""FontAwesomeSolid\"" FontSize=\""40\"" StyleClass=\""text-interface-soft\"" HorizontalOptions=\""Center\"" />\n    <Label Text=\""You are offline\"" StyleClass=\""title3, bold, text-interface-strongest\"" HorizontalTextAlignment=\""Center\"" />\n    <Label Text=\""Connect to the internet to load this page.\"" StyleClass=\""body, text-interface-medium\"" HorizontalTextAlignment=\""Center\"" />\n</VerticalStackLayout>"",""CssStyles"":"""",""CacheDuration"":0,""ProcessLavaOnServer"":true,""ProcessLavaOnClient"":false}' WHERE [Guid] = '12A48122-F5C7-4731-8780-873E0FAADDAC'" );   // Needs List
            Sql( @"UPDATE [Block] SET [AdditionalSettings] = N'{""ShowOnTablet"":true,""ShowOnPhone"":true,""RequiresNetwork"":true,""NoNetworkContent"":""<VerticalStackLayout Spacing=\""14\"" StyleClass=\""p-24\"" VerticalOptions=\""Center\"">\n    <Rock:Icon IconClass=\""wifi-slash\"" IconFamily=\""FontAwesomeSolid\"" FontSize=\""40\"" StyleClass=\""text-interface-soft\"" HorizontalOptions=\""Center\"" />\n    <Label Text=\""You are offline\"" StyleClass=\""title3, bold, text-interface-strongest\"" HorizontalTextAlignment=\""Center\"" />\n    <Label Text=\""Connect to the internet to load this page.\"" StyleClass=\""body, text-interface-medium\"" HorizontalTextAlignment=\""Center\"" />\n</VerticalStackLayout>"",""CssStyles"":"""",""CacheDuration"":0,""ProcessLavaOnServer"":false,""ProcessLavaOnClient"":false}' WHERE [Guid] = '43523BEC-BC89-448E-BE74-9DB32CF4BB3B'" );   // Workflow Entry
            Sql( @"UPDATE [Block] SET [AdditionalSettings] = N'{""ShowOnTablet"":true,""ShowOnPhone"":true,""RequiresNetwork"":true,""NoNetworkContent"":""<VerticalStackLayout Spacing=\""14\"" StyleClass=\""p-24\"" VerticalOptions=\""Center\"">\n    <Rock:Icon IconClass=\""wifi-slash\"" IconFamily=\""FontAwesomeSolid\"" FontSize=\""40\"" StyleClass=\""text-interface-soft\"" HorizontalOptions=\""Center\"" />\n    <Label Text=\""You are offline\"" StyleClass=\""title3, bold, text-interface-strongest\"" HorizontalTextAlignment=\""Center\"" />\n    <Label Text=\""Connect to the internet to load this page.\"" StyleClass=\""body, text-interface-medium\"" HorizontalTextAlignment=\""Center\"" />\n</VerticalStackLayout>"",""CssStyles"":"""",""CacheDuration"":0,""ProcessLavaOnServer"":true,""ProcessLavaOnClient"":false}' WHERE [Guid] = '1C5A37BB-A556-4777-9E33-51A11D4DB8A8'" );   // Content
            Sql( @"UPDATE [Block] SET [AdditionalSettings] = N'{""ShowOnTablet"":true,""ShowOnPhone"":true,""RequiresNetwork"":true,""NoNetworkContent"":""<VerticalStackLayout Spacing=\""14\"" StyleClass=\""p-24\"" VerticalOptions=\""Center\"">\n    <Rock:Icon IconClass=\""wifi-slash\"" IconFamily=\""FontAwesomeSolid\"" FontSize=\""40\"" StyleClass=\""text-interface-soft\"" HorizontalOptions=\""Center\"" />\n    <Label Text=\""You are offline\"" StyleClass=\""title3, bold, text-interface-strongest\"" HorizontalTextAlignment=\""Center\"" />\n    <Label Text=\""Connect to the internet to load this page.\"" StyleClass=\""body, text-interface-medium\"" HorizontalTextAlignment=\""Center\"" />\n</VerticalStackLayout>"",""CssStyles"":"""",""CacheDuration"":0,""ProcessLavaOnServer"":false,""ProcessLavaOnClient"":false}' WHERE [Guid] = 'B4E573CC-516E-4E32-A9B6-8E1475F8086F'" );   // Structured Content View
            Sql( @"UPDATE [Block] SET [AdditionalSettings] = N'{""ShowOnTablet"":true,""ShowOnPhone"":true,""RequiresNetwork"":true,""NoNetworkContent"":""<VerticalStackLayout Spacing=\""14\"" StyleClass=\""p-24\"" VerticalOptions=\""Center\"">\n    <Rock:Icon IconClass=\""wifi-slash\"" IconFamily=\""FontAwesomeSolid\"" FontSize=\""40\"" StyleClass=\""text-interface-soft\"" HorizontalOptions=\""Center\"" />\n    <Label Text=\""You are offline\"" StyleClass=\""title3, bold, text-interface-strongest\"" HorizontalTextAlignment=\""Center\"" />\n    <Label Text=\""Connect to the internet to load this page.\"" StyleClass=\""body, text-interface-medium\"" HorizontalTextAlignment=\""Center\"" />\n</VerticalStackLayout>"",""CssStyles"":"""",""CacheDuration"":3600,""ProcessLavaOnServer"":true,""ProcessLavaOnClient"":false}' WHERE [Guid] = 'C82E63A0-0D0F-4EA7-A2B8-A53FF93C88F0'" );   // Podcast List
            Sql( @"UPDATE [Block] SET [AdditionalSettings] = N'{""ShowOnTablet"":true,""ShowOnPhone"":true,""RequiresNetwork"":false,""NoNetworkContent"":"""",""CssStyles"":"""",""CacheDuration"":0,""ProcessLavaOnServer"":true,""ProcessLavaOnClient"":false}' WHERE [Guid] = 'B0264469-6A70-46F8-8A13-AC0B3375652A'" );   // Podcast Detail
            Sql( @"UPDATE [Block] SET [AdditionalSettings] = N'{""ShowOnTablet"":true,""ShowOnPhone"":true,""RequiresNetwork"":true,""NoNetworkContent"":""<VerticalStackLayout Spacing=\""14\"" StyleClass=\""p-24\"" VerticalOptions=\""Center\"">\n    <Rock:Icon IconClass=\""wifi-slash\"" IconFamily=\""FontAwesomeSolid\"" FontSize=\""40\"" StyleClass=\""text-interface-soft\"" HorizontalOptions=\""Center\"" />\n    <Label Text=\""You are offline\"" StyleClass=\""title3, bold, text-interface-strongest\"" HorizontalTextAlignment=\""Center\"" />\n    <Label Text=\""Connect to the internet to load this page.\"" StyleClass=\""body, text-interface-medium\"" HorizontalTextAlignment=\""Center\"" />\n</VerticalStackLayout>"",""CssStyles"":"""",""CacheDuration"":0,""ProcessLavaOnServer"":false,""ProcessLavaOnClient"":false}' WHERE [Guid] = '258628D3-224E-4CFA-856B-B9C3A8E097BD'" );   // Content Channel Item View
            Sql( @"UPDATE [Block] SET [AdditionalSettings] = N'{""ShowOnTablet"":true,""ShowOnPhone"":true,""RequiresNetwork"":true,""NoNetworkContent"":""<VerticalStackLayout Spacing=\""14\"" StyleClass=\""p-24\"" VerticalOptions=\""Center\"">\n    <Rock:Icon IconClass=\""wifi-slash\"" IconFamily=\""FontAwesomeSolid\"" FontSize=\""40\"" StyleClass=\""text-interface-soft\"" HorizontalOptions=\""Center\"" />\n    <Label Text=\""You are offline\"" StyleClass=\""title3, bold, text-interface-strongest\"" HorizontalTextAlignment=\""Center\"" />\n    <Label Text=\""Connect to the internet to load this page.\"" StyleClass=\""body, text-interface-medium\"" HorizontalTextAlignment=\""Center\"" />\n</VerticalStackLayout>"",""CssStyles"":"""",""CacheDuration"":0,""ProcessLavaOnServer"":true,""ProcessLavaOnClient"":false}' WHERE [Guid] = '500D9BFF-47F3-4AEA-9601-6450337C9CE2'" );   // Scripture References
            Sql( @"UPDATE [Block] SET [AdditionalSettings] = N'{""ShowOnTablet"":true,""ShowOnPhone"":true,""RequiresNetwork"":true,""NoNetworkContent"":""<VerticalStackLayout Spacing=\""14\"" StyleClass=\""p-24\"" VerticalOptions=\""Center\"">\n    <Rock:Icon IconClass=\""wifi-slash\"" IconFamily=\""FontAwesomeSolid\"" FontSize=\""40\"" StyleClass=\""text-interface-soft\"" HorizontalOptions=\""Center\"" />\n    <Label Text=\""You are offline\"" StyleClass=\""title3, bold, text-interface-strongest\"" HorizontalTextAlignment=\""Center\"" />\n    <Label Text=\""Connect to the internet to load this page.\"" StyleClass=\""body, text-interface-medium\"" HorizontalTextAlignment=\""Center\"" />\n</VerticalStackLayout>"",""CssStyles"":"""",""CacheDuration"":3600,""ProcessLavaOnServer"":false,""ProcessLavaOnClient"":false}' WHERE [Guid] = '633A561C-B5A3-44A6-97E5-5A422CCD1AA8'" );   // Content Channel Item View
            Sql( @"UPDATE [Block] SET [AdditionalSettings] = N'{""ShowOnTablet"":true,""ShowOnPhone"":true,""RequiresNetwork"":false,""NoNetworkContent"":"""",""CssStyles"":"""",""CacheDuration"":0,""ProcessLavaOnServer"":true,""ProcessLavaOnClient"":false}' WHERE [Guid] = '5D9A02B7-84C1-4E36-A7F8-B0629C4E1D53'" );   // Inheritance Campaign
            Sql( @"UPDATE [Block] SET [AdditionalSettings] = N'{""ShowOnTablet"":true,""ShowOnPhone"":true,""RequiresNetwork"":false,""NoNetworkContent"":"""",""CssStyles"":"""",""CacheDuration"":0,""ProcessLavaOnServer"":true,""ProcessLavaOnClient"":false}' WHERE [Guid] = '8FC5061A-927B-4E34-C358-A064D7F1B9E2'" );   // Serving Nav
            Sql( @"UPDATE [Block] SET [AdditionalSettings] = N'{""ShowOnTablet"":true,""ShowOnPhone"":true,""RequiresNetwork"":false,""NoNetworkContent"":"""",""CssStyles"":"""",""CacheDuration"":0,""ProcessLavaOnServer"":true,""ProcessLavaOnClient"":false}' WHERE [Guid] = '4B81C2D6-5E37-4A90-8F14-6C2093BD75AE'" );   // Schedule Preferences
            Sql( @"UPDATE [Block] SET [AdditionalSettings] = N'{""ShowOnTablet"":true,""ShowOnPhone"":true,""RequiresNetwork"":false,""NoNetworkContent"":"""",""CssStyles"":"""",""CacheDuration"":0,""ProcessLavaOnServer"":true,""ProcessLavaOnClient"":false}' WHERE [Guid] = '90D6172B-A38C-4F45-D469-B175E8021CA3'" );   // Serving Nav
            Sql( @"UPDATE [Block] SET [AdditionalSettings] = N'{""ShowOnTablet"":true,""ShowOnPhone"":true,""RequiresNetwork"":false,""NoNetworkContent"":"""",""CssStyles"":"""",""CacheDuration"":0,""ProcessLavaOnServer"":true,""ProcessLavaOnClient"":false}' WHERE [Guid] = '5C92D3E7-6F48-4B01-9025-7D31A4CE86BF'" );   // Sign Up for Serving
            Sql( @"UPDATE [Block] SET [AdditionalSettings] = N'{""ShowOnTablet"":true,""ShowOnPhone"":true,""RequiresNetwork"":false,""NoNetworkContent"":"""",""CssStyles"":"""",""CacheDuration"":0,""ProcessLavaOnServer"":true,""ProcessLavaOnClient"":false}' WHERE [Guid] = 'A1E7283C-B49D-4056-E570-C286F91302B4'" );   // Serving Nav
            Sql( @"UPDATE [Block] SET [AdditionalSettings] = N'{""ShowOnTablet"":true,""ShowOnPhone"":true,""RequiresNetwork"":false,""NoNetworkContent"":"""",""CssStyles"":"""",""CacheDuration"":0,""ProcessLavaOnServer"":true,""ProcessLavaOnClient"":false}' WHERE [Guid] = '6DA3E4F8-7059-4C12-A136-8E42B5DF97C0'" );   // Schedule Unavailability
            Sql( @"UPDATE [Block] SET [AdditionalSettings] = N'{""ShowOnTablet"":true,""ShowOnPhone"":true,""RequiresNetwork"":false,""NoNetworkContent"":"""",""CssStyles"":"""",""CacheDuration"":0,""ProcessLavaOnServer"":true,""ProcessLavaOnClient"":false}' WHERE [Guid] = '7EB4F509-816A-4D23-B247-9F53C6E0A8D1'" );   // Serving Nav
            Sql( @"UPDATE [Block] SET [AdditionalSettings] = N'{""ShowOnTablet"":true,""ShowOnPhone"":true,""RequiresNetwork"":true,""NoNetworkContent"":"""",""CssStyles"":"""",""CacheDuration"":300,""ProcessLavaOnServer"":true,""ProcessLavaOnClient"":false}' WHERE [Guid] = '2C7A4F19-6B83-4E05-9D24-A15C8E30B7F6'" );   // Notifications
            Sql( @"UPDATE [Block] SET [AdditionalSettings] = N'{""ShowOnTablet"":true,""ShowOnPhone"":true,""RequiresNetwork"":true,""NoNetworkContent"":"""",""CssStyles"":"""",""CacheDuration"":3600,""ProcessLavaOnServer"":true,""ProcessLavaOnClient"":false}' WHERE [Guid] = 'C519E24F-A086-4D2B-B7C4-9F203B85D67A'" );   // Podcast Feed List
            Sql( @"UPDATE [Block] SET [AdditionalSettings] = N'{""ShowOnTablet"":true,""ShowOnPhone"":true,""RequiresNetwork"":true,""NoNetworkContent"":"""",""CssStyles"":"""",""CacheDuration"":60,""ProcessLavaOnServer"":true,""ProcessLavaOnClient"":false}' WHERE [Guid] = 'D62AF350-B197-4E3C-98D5-A0314C96E78B'" );   // Podcast Feed Episode

            //
            // Block CSS class (Advanced Settings > CSS Class - not carried by AddBlock)
            //
            Sql( @"UPDATE [Block] SET [CssClass] = N'm-16' WHERE [Guid] = '783BB975-D313-4722-A444-D3FF6EE06B3B'" );   // Login
            Sql( @"UPDATE [Block] SET [CssClass] = N'm-16' WHERE [Guid] = '86FA86D1-936D-4AD3-908B-D07D9A874F1F'" );   // Onboard Person
            Sql( @"UPDATE [Block] SET [CssClass] = N'm-16' WHERE [Guid] = '9C074994-EE12-41C8-8072-49A3012A72E8'" );   // Profile Details
            Sql( @"UPDATE [Block] SET [CssClass] = N'm-16' WHERE [Guid] = 'E44BCD5E-AFA1-42D2-8371-0041ADB65CC5'" );   // Workflow Entry
            Sql( @"UPDATE [Block] SET [CssClass] = N'm-16' WHERE [Guid] = '82791881-D9E4-48E1-9844-A84CFDC78955'" );   // Check-in
            Sql( @"UPDATE [Block] SET [CssClass] = N'm-16' WHERE [Guid] = 'BEBEC594-4C65-411E-8013-BAC2983D2DD8'" );   // Calendar View
            Sql( @"UPDATE [Block] SET [CssClass] = N'm-16' WHERE [Guid] = 'B36C1077-5312-44E3-ACFF-DCB32EC72B4A'" );   // Calendar Event Item Occurrence View
            Sql( @"UPDATE [Block] SET [CssClass] = N'm-16' WHERE [Guid] = '7B49DE66-B707-417F-8A65-A082508A548B'" );   // Schedule Toolbox
            Sql( @"UPDATE [Block] SET [CssClass] = N'm-16' WHERE [Guid] = '98963BBF-5F11-4B6B-ACCA-ECFFFDB96480'" );   // Group View
            Sql( @"UPDATE [Block] SET [CssClass] = N'm-16' WHERE [Guid] = '597C9DB9-24E6-4FBF-A97D-A21A32F3B81D'" );   // Group Member List
            Sql( @"UPDATE [Block] SET [CssClass] = N'm-16' WHERE [Guid] = '4687C5D1-BEAE-48F7-AB47-C5F723369EB6'" );   // Workflow Entry
            Sql( @"UPDATE [Block] SET [CssClass] = N'm-16' WHERE [Guid] = 'E06F0FFF-4020-4B11-9F1E-038724978A34'" );   // Group Attendance Entry
            Sql( @"UPDATE [Block] SET [CssClass] = N'm-16' WHERE [Guid] = '43523BEC-BC89-448E-BE74-9DB32CF4BB3B'" );   // Workflow Entry
            Sql( @"UPDATE [Block] SET [CssClass] = N'm-16' WHERE [Guid] = '1C5A37BB-A556-4777-9E33-51A11D4DB8A8'" );   // Content
            Sql( @"UPDATE [Block] SET [CssClass] = N'm-16' WHERE [Guid] = 'B4E573CC-516E-4E32-A9B6-8E1475F8086F'" );   // Structured Content View
            Sql( @"UPDATE [Block] SET [CssClass] = N'm-16' WHERE [Guid] = '4B81C2D6-5E37-4A90-8F14-6C2093BD75AE'" );   // Schedule Preferences
            Sql( @"UPDATE [Block] SET [CssClass] = N'm-16' WHERE [Guid] = '5C92D3E7-6F48-4B01-9025-7D31A4CE86BF'" );   // Sign Up for Serving
            Sql( @"UPDATE [Block] SET [CssClass] = N'm-16' WHERE [Guid] = '6DA3E4F8-7059-4C12-A136-8E42B5DF97C0'" );   // Schedule Unavailability
            Sql( @"UPDATE [Block] SET [CssClass] = N'my-16' WHERE [Guid] = '2C7A4F19-6B83-4E05-9D24-A15C8E30B7F6'" );   // Notifications

            //
            // Page advanced settings (CSS class, icon, description, keywords, header content)
            //

            //
            // Security - custom page/block permissions
            //
            RockMigrationHelper.AddSecurityAuthForPage( "941096AE-FB51-4450-9DB9-6248B584D917", 0, "View", true, "", (int) Rock.Model.SpecialRole.AllAuthenticatedUsers, "88C5C9BE-A5FA-4E1E-A60F-8936139B3A75" );   // Edit Profile
            RockMigrationHelper.AddSecurityAuthForPage( "941096AE-FB51-4450-9DB9-6248B584D917", 1, "View", false, "", (int) Rock.Model.SpecialRole.AllUsers, "ACAE4711-47AC-47A1-898D-AFB2602E1C11" );   // Edit Profile
            RockMigrationHelper.AddSecurityAuthForPage( "5AC83965-D553-4255-B1ED-85F0D8742B6A", 0, "View", true, "", (int) Rock.Model.SpecialRole.AllAuthenticatedUsers, "CF70AAAC-6A1C-4880-AAFB-F2ADEB2BB80D" );   // Delete Account
            RockMigrationHelper.AddSecurityAuthForPage( "5AC83965-D553-4255-B1ED-85F0D8742B6A", 1, "View", false, "", (int) Rock.Model.SpecialRole.AllUsers, "320920F7-A751-4340-8ED2-A68CEB8C0C02" );   // Delete Account
            RockMigrationHelper.AddSecurityAuthForPage( "56302B84-36E3-4E62-9E74-C5739D7DE977", 0, "View", true, "", (int) Rock.Model.SpecialRole.AllAuthenticatedUsers, "E4CF7E5E-5053-44FC-B5A9-36E58D6FD991" );   // My Serving Schedule
            RockMigrationHelper.AddSecurityAuthForPage( "56302B84-36E3-4E62-9E74-C5739D7DE977", 1, "View", false, "", (int) Rock.Model.SpecialRole.AllUsers, "B046D0F8-9705-4A82-B43D-2927A5F7573D" );   // My Serving Schedule
            RockMigrationHelper.AddSecurityAuthForPage( "3430667F-D38C-4A9C-A65E-BC8D15B4FC51", 0, "View", true, "", (int) Rock.Model.SpecialRole.AllAuthenticatedUsers, "7FBE3097-693B-48E6-BE70-C0A719C2419C" );   // My Groups
            RockMigrationHelper.AddSecurityAuthForPage( "3430667F-D38C-4A9C-A65E-BC8D15B4FC51", 1, "View", false, "", (int) Rock.Model.SpecialRole.AllUsers, "A537F4E4-CD8E-4099-8753-13E24A168363" );   // My Groups
            RockMigrationHelper.AddSecurityAuthForBlock( "FF2703A4-BF7D-4A53-A214-10AD2E850BAA", 0, "View", true, "", Rock.Model.SpecialRole.AllAuthenticatedUsers, "1739D63A-1E8C-415E-BAE7-94B9F7E97D3F" );   // Content
            RockMigrationHelper.AddSecurityAuthForBlock( "FF2703A4-BF7D-4A53-A214-10AD2E850BAA", 1, "View", false, "", Rock.Model.SpecialRole.AllUsers, "D6D36810-0E3C-402F-953A-6DE18A60F61F" );   // Content
            RockMigrationHelper.AddSecurityAuthForBlock( "10E07AFD-C2EB-4E97-9121-027811648F4B", 0, "View", true, "", Rock.Model.SpecialRole.AllUnAuthenticatedUsers, "48FC36F9-566C-4C9E-8731-94E350F2ED45" );   // Content - Not logged in
            RockMigrationHelper.AddSecurityAuthForBlock( "10E07AFD-C2EB-4E97-9121-027811648F4B", 1, "View", false, "", Rock.Model.SpecialRole.AllAuthenticatedUsers, "5F8A6EC1-8360-43F9-B522-8754D9527B7F" );   // Content - Not logged in
            RockMigrationHelper.AddSecurityAuthForBlock( "77A80F5F-CED1-4471-BCE3-1F405BE29C6B", 0, "View", true, "", Rock.Model.SpecialRole.AllUnAuthenticatedUsers, "264249E4-7CD8-4BD7-A5F5-28F9D86E2038" );   // Content - Not signed in
            RockMigrationHelper.AddSecurityAuthForBlock( "77A80F5F-CED1-4471-BCE3-1F405BE29C6B", 1, "View", false, "", Rock.Model.SpecialRole.AllAuthenticatedUsers, "F7873091-68D8-4D46-AD7C-14E7FCBE744E" );   // Content - Not signed in
            RockMigrationHelper.AddSecurityAuthForBlock( "B4E573CC-516E-4E32-A9B6-8E1475F8086F", 0, "View", true, "", Rock.Model.SpecialRole.AllAuthenticatedUsers, "C4C9591C-F288-4BB4-B751-E7800F6BA222" );   // Structured Content View
            RockMigrationHelper.AddSecurityAuthForBlock( "B4E573CC-516E-4E32-A9B6-8E1475F8086F", 1, "View", false, "", Rock.Model.SpecialRole.AllUnAuthenticatedUsers, "76DE382F-8014-4874-A3D2-CD876CBB92D6" );   // Structured Content View

            // Site page references, re-resolved by Guid.
            // Emitting the raw ids here would point the target at unrelated pages.
            Sql( @"
                DECLARE @SiteId INT = ( SELECT TOP 1 [Id] FROM [Site] WHERE [Guid] = '1D501408-CA88-4565-8822-BD318F255A59' );
                DECLARE @HomePage INT = ( SELECT TOP 1 [Id] FROM [Page] WHERE [Guid] = 'EF0257AD-B5E4-4D53-B7D0-17561941EE1E' );
                DECLARE @LoginPage INT = ( SELECT TOP 1 [Id] FROM [Page] WHERE [Guid] = '9BB25932-4D56-417C-911B-DC915167E7BC' );
                DECLARE @ProfilePage INT = ( SELECT TOP 1 [Id] FROM [Page] WHERE [Guid] = '941096AE-FB51-4450-9DB9-6248B584D917' );
                DECLARE @CommView INT = ( SELECT TOP 1 [Id] FROM [Page] WHERE [Guid] = 'E626FC0E-18F1-49DB-8A17-AA0AE375E4E8' );

                UPDATE [Site] SET [DefaultPageId] = ISNULL( @HomePage, [DefaultPageId] ),
                                 [LoginPageId]   = ISNULL( @LoginPage, [LoginPageId] )
                 WHERE [Id] = @SiteId;

                DECLARE @Json NVARCHAR(MAX) = ( SELECT CAST([AdditionalSettings] AS NVARCHAR(MAX)) FROM [Site] WHERE [Id] = @SiteId );
                IF @Json IS NOT NULL AND ISJSON( @Json ) = 1
                BEGIN
                    SET @Json = JSON_MODIFY( @Json, '$.ProfilePageId', @ProfilePage );
                    SET @Json = JSON_MODIFY( @Json, '$.CommunicationViewPageId', @CommView );
                    SET @Json = JSON_MODIFY( @Json, '$.LastDeploymentDate', NULL );
                    SET @Json = JSON_MODIFY( @Json, '$.LastDeploymentVersionId', NULL );
                    SET @Json = JSON_MODIFY( @Json, '$.PhoneUpdatePackageUrl', NULL );
                    SET @Json = JSON_MODIFY( @Json, '$.TabletUpdatePackageUrl', NULL );

                    -- ApiKeyId must be RESOLVED, not copied and not blindly cleared.
                    -- MobileApplicationDetail.SaveApiKey branches on it:
                    --   set   -> Get( id ) then read .Person   -> NullReferenceException if the
                    --            id came from the SOURCE server and does not exist here.
                    --   empty -> INSERT a UserLogin named mobile_application_{SiteId} -> duplicate
                    --            key on IX_UserName if a previous run already created it.
                    -- So look the login up by the name Rock derives, and use it if present.
                    -- JSON_MODIFY with a NULL value removes the property (lax mode), which is
                    -- exactly the "let Rock create one" case - so one statement covers both and
                    -- the migration stays safely re-runnable.
                    DECLARE @RestUserName NVARCHAR(255) = 'mobile_application_' + CAST( @SiteId AS NVARCHAR(20) );
                    DECLARE @RestLoginId INT = ( SELECT TOP 1 [Id] FROM [UserLogin] WHERE [UserName] = @RestUserName );
                    SET @Json = JSON_MODIFY( @Json, '$.ApiKeyId', @RestLoginId );

                    UPDATE [Site] SET [AdditionalSettings] = @Json WHERE [Id] = @SiteId;
                END
                " );

﻿
            //
            // App-specific content channels (created by the app, referenced by Guid)
            //
            Sql( @"
                DECLARE @ChannelTypeId INT = ( SELECT TOP 1 [Id] FROM [ContentChannelType] WHERE [Guid] = '0A69DA05-F671-454F-A25D-99A01E10ADB8' );
                IF @ChannelTypeId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [ContentChannel] WHERE [Guid] = '68A8458F-B632-46BA-A1F4-CB65C62DF18F' )
                BEGIN
                    INSERT INTO [ContentChannel] ( [ContentChannelTypeId], [Name], [Description], [IconCssClass], [RequiresApproval], [EnableRss], [ChannelUrl], [ItemUrl], [TimeToLive], [ContentControlType], [RootImageDirectory], [IsIndexEnabled], [ItemsManuallyOrdered], [ChildItemsManuallyOrdered], [IsStructuredContent], [Guid] )
                    VALUES ( @ChannelTypeId, 'Staff Bios', 'KFS Staff Bios', '', 0, 0, '', '', 0, 0, '', 0, 0, 0, 0, '68A8458F-B632-46BA-A1F4-CB65C62DF18F' );
                END
                " );   // Staff Bios
            Sql( @"
                DECLARE @ChannelTypeId INT = ( SELECT TOP 1 [Id] FROM [ContentChannelType] WHERE [Guid] = 'A2F62EDC-576A-45F4-9DF1-F7867B21CDEE' );
                IF @ChannelTypeId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [ContentChannel] WHERE [Guid] = '122CAAAE-0698-4869-89FE-D818E109BAEA' )
                BEGIN
                    INSERT INTO [ContentChannel] ( [ContentChannelTypeId], [Name], [Description], [IconCssClass], [RequiresApproval], [EnableRss], [ChannelUrl], [ItemUrl], [TimeToLive], [ContentControlType], [RootImageDirectory], [IsIndexEnabled], [ItemsManuallyOrdered], [ChildItemsManuallyOrdered], [IsStructuredContent], [Guid] )
                    VALUES ( @ChannelTypeId, 'App Home Feed', '', '', 0, 0, '', '', 0, 0, '', 0, 1, 0, 0, '122CAAAE-0698-4869-89FE-D818E109BAEA' );
                END
                " );   // App Home Feed
            Sql( @"
                DECLARE @ChannelTypeId INT = ( SELECT TOP 1 [Id] FROM [ContentChannelType] WHERE [Guid] = 'A2F62EDC-576A-45F4-9DF1-F7867B21CDEE' );
                IF @ChannelTypeId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [ContentChannel] WHERE [Guid] = 'DF6276E8-B00E-4E77-88D8-AB1241A4A8C7' )
                BEGIN
                    INSERT INTO [ContentChannel] ( [ContentChannelTypeId], [Name], [Description], [IconCssClass], [RequiresApproval], [EnableRss], [ChannelUrl], [ItemUrl], [TimeToLive], [ContentControlType], [RootImageDirectory], [IsIndexEnabled], [ItemsManuallyOrdered], [ChildItemsManuallyOrdered], [IsStructuredContent], [Guid] )
                    VALUES ( @ChannelTypeId, 'App Menu', 'This content channel is used to drive generic ""App menu"" style content. Not sure if it is going to drive actual content items either, but for now just the menu.', '', 0, 0, '', '', 0, 0, '', 0, 1, 0, 1, 'DF6276E8-B00E-4E77-88D8-AB1241A4A8C7' );
                END
                " );   // App Menu

            //
            // Defined types used by content channel item attributes
            //

            //
            // Content channel item attributes (qualified to the new channel Id on the target server)
            //
            Sql( @"
                DECLARE @ChannelId INT = ( SELECT TOP 1 [Id] FROM [ContentChannel] WHERE [Guid] = '68A8458F-B632-46BA-A1F4-CB65C62DF18F' );
                DECLARE @EntityTypeId INT = ( SELECT TOP 1 [Id] FROM [EntityType] WHERE [Name] = 'Rock.Model.ContentChannelItem' );
                DECLARE @FieldTypeId INT = ( SELECT TOP 1 [Id] FROM [FieldType] WHERE [Guid] = '9C204CD0-1233-41C5-818A-C5DA439445AA' );
                IF @ChannelId IS NOT NULL AND @FieldTypeId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [Attribute] WHERE [Guid] = '0D4119AA-604C-4ED8-8D8C-1F326398B9E6' )
                BEGIN
                    INSERT INTO [Attribute] ( [IsSystem], [FieldTypeId], [EntityTypeId], [EntityTypeQualifierColumn], [EntityTypeQualifierValue], [Key], [Name], [Description], [Order], [IsGridColumn], [DefaultValue], [IsMultiValue], [IsRequired], [Guid] )
                    VALUES ( 0, @FieldTypeId, @EntityTypeId, 'ContentChannelId', CAST( @ChannelId AS NVARCHAR(10) ), 'Role', 'Role', 'Staff Role', 1001, 0, '', 0, 0, '0D4119AA-604C-4ED8-8D8C-1F326398B9E6' );
                END
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = '8F1921A0-DE27-47EF-9FE5-D108ABFD33A1' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '0D4119AA-604C-4ED8-8D8C-1F326398B9E6' ), 'ispassword', 'False', '8F1921A0-DE27-47EF-9FE5-D108ABFD33A1';
                " );   // Staff Bios . Role
            Sql( @"
                DECLARE @ChannelId INT = ( SELECT TOP 1 [Id] FROM [ContentChannel] WHERE [Guid] = '68A8458F-B632-46BA-A1F4-CB65C62DF18F' );
                DECLARE @EntityTypeId INT = ( SELECT TOP 1 [Id] FROM [EntityType] WHERE [Name] = 'Rock.Model.ContentChannelItem' );
                DECLARE @FieldTypeId INT = ( SELECT TOP 1 [Id] FROM [FieldType] WHERE [Guid] = '97F8157D-A8C8-4AB3-96A2-9CB2A9049E6D' );
                IF @ChannelId IS NOT NULL AND @FieldTypeId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [Attribute] WHERE [Guid] = '6A6EE481-E412-4F48-AC3B-B1145E81841F' )
                BEGIN
                    INSERT INTO [Attribute] ( [IsSystem], [FieldTypeId], [EntityTypeId], [EntityTypeQualifierColumn], [EntityTypeQualifierValue], [Key], [Name], [Description], [Order], [IsGridColumn], [DefaultValue], [IsMultiValue], [IsRequired], [Guid] )
                    VALUES ( 0, @FieldTypeId, @EntityTypeId, 'ContentChannelId', CAST( @ChannelId AS NVARCHAR(10) ), 'Image', 'Image', '', 1002, 0, '', 0, 0, '6A6EE481-E412-4F48-AC3B-B1145E81841F' );
                END
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = 'C6E4A746-4FEE-43D7-BF6B-9D7195D17E22' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '6A6EE481-E412-4F48-AC3B-B1145E81841F' ), 'binaryFileType', '8dbf874c-f3c2-4848-8137-c963c431eb0b', 'C6E4A746-4FEE-43D7-BF6B-9D7195D17E22';
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = 'D05768B3-6EEE-4077-9FAF-DB0E9A4D6BC9' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '6A6EE481-E412-4F48-AC3B-B1145E81841F' ), 'formatAsLink', 'False', 'D05768B3-6EEE-4077-9FAF-DB0E9A4D6BC9';
                " );   // Staff Bios . Image
            Sql( @"
                DECLARE @ChannelId INT = ( SELECT TOP 1 [Id] FROM [ContentChannel] WHERE [Guid] = '68A8458F-B632-46BA-A1F4-CB65C62DF18F' );
                DECLARE @EntityTypeId INT = ( SELECT TOP 1 [Id] FROM [EntityType] WHERE [Name] = 'Rock.Model.ContentChannelItem' );
                DECLARE @FieldTypeId INT = ( SELECT TOP 1 [Id] FROM [FieldType] WHERE [Guid] = '73B02051-0D38-4AD9-BF81-A2D477DE4F70' );
                IF @ChannelId IS NOT NULL AND @FieldTypeId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [Attribute] WHERE [Guid] = 'EC188901-EBBD-4DEC-BBEE-BEA26796BC5F' )
                BEGIN
                    INSERT INTO [Attribute] ( [IsSystem], [FieldTypeId], [EntityTypeId], [EntityTypeQualifierColumn], [EntityTypeQualifierValue], [Key], [Name], [Description], [Order], [IsGridColumn], [DefaultValue], [IsMultiValue], [IsRequired], [Guid] )
                    VALUES ( 0, @FieldTypeId, @EntityTypeId, 'ContentChannelId', CAST( @ChannelId AS NVARCHAR(10) ), 'SocialMedia', 'Social Media', '', 1003, 0, '', 0, 0, 'EC188901-EBBD-4DEC-BBEE-BEA26796BC5F' );
                END
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = '8D6CDB28-CDE8-4F70-8CFA-E90354FE2A97' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = 'EC188901-EBBD-4DEC-BBEE-BEA26796BC5F' ), 'customvalues', '', '8D6CDB28-CDE8-4F70-8CFA-E90354FE2A97';
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = '7C6B3FA3-2E27-4BAD-8DD8-9DED7CC4B8E3' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = 'EC188901-EBBD-4DEC-BBEE-BEA26796BC5F' ), 'definedtype', '', '7C6B3FA3-2E27-4BAD-8DD8-9DED7CC4B8E3';
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = 'F1B56238-49E8-4B66-A4AD-8DC602F7846B' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = 'EC188901-EBBD-4DEC-BBEE-BEA26796BC5F' ), 'displayvaluefirst', 'False', 'F1B56238-49E8-4B66-A4AD-8DC602F7846B';
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = '1A29D174-F907-4B66-A632-ECD89E37788B' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = 'EC188901-EBBD-4DEC-BBEE-BEA26796BC5F' ), 'keyprompt', 'Social Media Icon', '1A29D174-F907-4B66-A632-ECD89E37788B';
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = 'B74E05C9-5041-4B87-9D13-B082C5380FA6' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = 'EC188901-EBBD-4DEC-BBEE-BEA26796BC5F' ), 'valueprompt', 'Social Media Link', 'B74E05C9-5041-4B87-9D13-B082C5380FA6';
                " );   // Staff Bios . SocialMedia
            Sql( @"
                DECLARE @ChannelId INT = ( SELECT TOP 1 [Id] FROM [ContentChannel] WHERE [Guid] = '68A8458F-B632-46BA-A1F4-CB65C62DF18F' );
                DECLARE @EntityTypeId INT = ( SELECT TOP 1 [Id] FROM [EntityType] WHERE [Name] = 'Rock.Model.ContentChannelItem' );
                DECLARE @FieldTypeId INT = ( SELECT TOP 1 [Id] FROM [FieldType] WHERE [Guid] = '9C204CD0-1233-41C5-818A-C5DA439445AA' );
                IF @ChannelId IS NOT NULL AND @FieldTypeId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [Attribute] WHERE [Guid] = 'E6191C21-2443-44E4-9A1F-05CC15B6E6AE' )
                BEGIN
                    INSERT INTO [Attribute] ( [IsSystem], [FieldTypeId], [EntityTypeId], [EntityTypeQualifierColumn], [EntityTypeQualifierValue], [Key], [Name], [Description], [Order], [IsGridColumn], [DefaultValue], [IsMultiValue], [IsRequired], [Guid] )
                    VALUES ( 0, @FieldTypeId, @EntityTypeId, 'ContentChannelId', CAST( @ChannelId AS NVARCHAR(10) ), 'DisplayName', 'Display Name', '', 1004, 0, '', 0, 0, 'E6191C21-2443-44E4-9A1F-05CC15B6E6AE' );
                END
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = 'EBD53B89-2D25-4F88-AA5C-75C03C3EEB5E' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = 'E6191C21-2443-44E4-9A1F-05CC15B6E6AE' ), 'ispassword', 'False', 'EBD53B89-2D25-4F88-AA5C-75C03C3EEB5E';
                " );   // Staff Bios . DisplayName
            Sql( @"
                DECLARE @ChannelId INT = ( SELECT TOP 1 [Id] FROM [ContentChannel] WHERE [Guid] = '68A8458F-B632-46BA-A1F4-CB65C62DF18F' );
                DECLARE @EntityTypeId INT = ( SELECT TOP 1 [Id] FROM [EntityType] WHERE [Name] = 'Rock.Model.ContentChannelItem' );
                DECLARE @FieldTypeId INT = ( SELECT TOP 1 [Id] FROM [FieldType] WHERE [Guid] = '9C204CD0-1233-41C5-818A-C5DA439445AA' );
                IF @ChannelId IS NOT NULL AND @FieldTypeId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [Attribute] WHERE [Guid] = '49870C28-60BE-4347-8CCB-7900406B6433' )
                BEGIN
                    INSERT INTO [Attribute] ( [IsSystem], [FieldTypeId], [EntityTypeId], [EntityTypeQualifierColumn], [EntityTypeQualifierValue], [Key], [Name], [Description], [Order], [IsGridColumn], [DefaultValue], [IsMultiValue], [IsRequired], [Guid] )
                    VALUES ( 0, @FieldTypeId, @EntityTypeId, 'ContentChannelId', CAST( @ChannelId AS NVARCHAR(10) ), 'Subtitle', 'Subtitle', '', 1008, 0, '', 0, 0, '49870C28-60BE-4347-8CCB-7900406B6433' );
                END
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = '5991AC97-5A7E-427C-9521-FFA93F8EF331' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '49870C28-60BE-4347-8CCB-7900406B6433' ), 'ispassword', 'False', '5991AC97-5A7E-427C-9521-FFA93F8EF331';
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = '82B58D85-B5A6-4843-B878-E422054E5993' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '49870C28-60BE-4347-8CCB-7900406B6433' ), 'maxcharacters', '', '82B58D85-B5A6-4843-B878-E422054E5993';
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = '3D6C857F-8A18-4E21-94EA-7D7E958CBA45' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '49870C28-60BE-4347-8CCB-7900406B6433' ), 'showcountdown', 'False', '3D6C857F-8A18-4E21-94EA-7D7E958CBA45';
                " );   // Staff Bios . Subtitle
            Sql( @"
                DECLARE @ChannelId INT = ( SELECT TOP 1 [Id] FROM [ContentChannel] WHERE [Guid] = '68A8458F-B632-46BA-A1F4-CB65C62DF18F' );
                DECLARE @EntityTypeId INT = ( SELECT TOP 1 [Id] FROM [EntityType] WHERE [Name] = 'Rock.Model.ContentChannelItem' );
                DECLARE @FieldTypeId INT = ( SELECT TOP 1 [Id] FROM [FieldType] WHERE [Guid] = '73B02051-0D38-4AD9-BF81-A2D477DE4F70' );
                IF @ChannelId IS NOT NULL AND @FieldTypeId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [Attribute] WHERE [Guid] = '2F8E6A31-7C05-4D92-B4E8-1A63D905C7F4' )
                BEGIN
                    INSERT INTO [Attribute] ( [IsSystem], [FieldTypeId], [EntityTypeId], [EntityTypeQualifierColumn], [EntityTypeQualifierValue], [Key], [Name], [Description], [Order], [IsGridColumn], [DefaultValue], [IsMultiValue], [IsRequired], [Guid] )
                    VALUES ( 0, @FieldTypeId, @EntityTypeId, 'ContentChannelId', CAST( @ChannelId AS NVARCHAR(10) ), 'ContactLinks', 'Contact Links', 'Rows shown on the staff detail page. Key = icon (envelope, phone, or a Font Awesome class like ""fa fa-linkedin""); value = the text displayed to its right. Leave the icon blank for a text-only row such as ""ext 6001"". Emails and phone numbers become tappable automatically.', 1009, 0, '', 0, 0, '2F8E6A31-7C05-4D92-B4E8-1A63D905C7F4' );
                END
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = 'BF407310-8EC6-4D59-E327-A6B510C8D4F9' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '2F8E6A31-7C05-4D92-B4E8-1A63D905C7F4' ), 'customvalues', '', 'BF407310-8EC6-4D59-E327-A6B510C8D4F9';
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = 'C0518421-9FD7-4E6A-F438-B7C621D9E50A' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '2F8E6A31-7C05-4D92-B4E8-1A63D905C7F4' ), 'definedtype', '', 'C0518421-9FD7-4E6A-F438-B7C621D9E50A';
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = 'AE3F62A9-7DB5-4C48-D216-95A40FB7C3E8' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '2F8E6A31-7C05-4D92-B4E8-1A63D905C7F4' ), 'displayvaluefirst', 'False', 'AE3F62A9-7DB5-4C48-D216-95A40FB7C3E8';
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = '8C1D40E7-5B93-4A26-B0F4-73E28D5A91C6' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '2F8E6A31-7C05-4D92-B4E8-1A63D905C7F4' ), 'keyprompt', 'Icon', '8C1D40E7-5B93-4A26-B0F4-73E28D5A91C6';
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = '9D2E51F8-6CA4-4B37-C105-84F39E6BA2D7' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '2F8E6A31-7C05-4D92-B4E8-1A63D905C7F4' ), 'valueprompt', 'Text to display', '9D2E51F8-6CA4-4B37-C105-84F39E6BA2D7';
                " );   // Staff Bios . ContactLinks
            Sql( @"
                DECLARE @ChannelId INT = ( SELECT TOP 1 [Id] FROM [ContentChannel] WHERE [Guid] = '122CAAAE-0698-4869-89FE-D818E109BAEA' );
                DECLARE @EntityTypeId INT = ( SELECT TOP 1 [Id] FROM [EntityType] WHERE [Name] = 'Rock.Model.ContentChannelItem' );
                DECLARE @FieldTypeId INT = ( SELECT TOP 1 [Id] FROM [FieldType] WHERE [Guid] = '9C204CD0-1233-41C5-818A-C5DA439445AA' );
                IF @ChannelId IS NOT NULL AND @FieldTypeId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [Attribute] WHERE [Guid] = 'C47BC6A6-68E9-46AF-B1FC-EDD09E37C4FF' )
                BEGIN
                    INSERT INTO [Attribute] ( [IsSystem], [FieldTypeId], [EntityTypeId], [EntityTypeQualifierColumn], [EntityTypeQualifierValue], [Key], [Name], [Description], [Order], [IsGridColumn], [DefaultValue], [IsMultiValue], [IsRequired], [Guid] )
                    VALUES ( 0, @FieldTypeId, @EntityTypeId, 'ContentChannelId', CAST( @ChannelId AS NVARCHAR(10) ), 'Tagline', 'Tagline', 'Small label above the title', 1000, 0, '', 0, 0, 'C47BC6A6-68E9-46AF-B1FC-EDD09E37C4FF' );
                END
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = 'E2BD483E-E959-4EC5-8822-0D9DE0916F9E' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = 'C47BC6A6-68E9-46AF-B1FC-EDD09E37C4FF' ), 'ispassword', 'False', 'E2BD483E-E959-4EC5-8822-0D9DE0916F9E';
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = '0B7BB443-21B0-4D21-88F1-5FCA88C574CC' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = 'C47BC6A6-68E9-46AF-B1FC-EDD09E37C4FF' ), 'maxcharacters', '', '0B7BB443-21B0-4D21-88F1-5FCA88C574CC';
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = '47208FBC-18DE-4F16-8C9B-78549122FAF8' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = 'C47BC6A6-68E9-46AF-B1FC-EDD09E37C4FF' ), 'showcountdown', 'False', '47208FBC-18DE-4F16-8C9B-78549122FAF8';
                " );   // App Home Feed . Tagline
            Sql( @"
                DECLARE @ChannelId INT = ( SELECT TOP 1 [Id] FROM [ContentChannel] WHERE [Guid] = '122CAAAE-0698-4869-89FE-D818E109BAEA' );
                DECLARE @EntityTypeId INT = ( SELECT TOP 1 [Id] FROM [EntityType] WHERE [Name] = 'Rock.Model.ContentChannelItem' );
                DECLARE @FieldTypeId INT = ( SELECT TOP 1 [Id] FROM [FieldType] WHERE [Guid] = '9C204CD0-1233-41C5-818A-C5DA439445AA' );
                IF @ChannelId IS NOT NULL AND @FieldTypeId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [Attribute] WHERE [Guid] = 'FA6FB88D-AC36-4570-B8AF-B1BDF38A15C5' )
                BEGIN
                    INSERT INTO [Attribute] ( [IsSystem], [FieldTypeId], [EntityTypeId], [EntityTypeQualifierColumn], [EntityTypeQualifierValue], [Key], [Name], [Description], [Order], [IsGridColumn], [DefaultValue], [IsMultiValue], [IsRequired], [Guid] )
                    VALUES ( 0, @FieldTypeId, @EntityTypeId, 'ContentChannelId', CAST( @ChannelId AS NVARCHAR(10) ), 'Subtitle', 'Subtitle', 'Small text, bottom-left of card', 1001, 0, '', 0, 0, 'FA6FB88D-AC36-4570-B8AF-B1BDF38A15C5' );
                END
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = 'B0A47447-FC07-4369-8DA0-084EB7277BE3' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = 'FA6FB88D-AC36-4570-B8AF-B1BDF38A15C5' ), 'ispassword', 'False', 'B0A47447-FC07-4369-8DA0-084EB7277BE3';
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = '2C4BF6DB-EB1D-430B-9012-E6F524415C3B' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = 'FA6FB88D-AC36-4570-B8AF-B1BDF38A15C5' ), 'maxcharacters', '', '2C4BF6DB-EB1D-430B-9012-E6F524415C3B';
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = 'ADEB718D-09DE-4193-AF04-6AF87248D74A' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = 'FA6FB88D-AC36-4570-B8AF-B1BDF38A15C5' ), 'showcountdown', 'False', 'ADEB718D-09DE-4193-AF04-6AF87248D74A';
                " );   // App Home Feed . Subtitle
            Sql( @"
                DECLARE @ChannelId INT = ( SELECT TOP 1 [Id] FROM [ContentChannel] WHERE [Guid] = '122CAAAE-0698-4869-89FE-D818E109BAEA' );
                DECLARE @EntityTypeId INT = ( SELECT TOP 1 [Id] FROM [EntityType] WHERE [Name] = 'Rock.Model.ContentChannelItem' );
                DECLARE @FieldTypeId INT = ( SELECT TOP 1 [Id] FROM [FieldType] WHERE [Guid] = '97F8157D-A8C8-4AB3-96A2-9CB2A9049E6D' );
                IF @ChannelId IS NOT NULL AND @FieldTypeId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [Attribute] WHERE [Guid] = '8D6E1DED-2A74-4FD1-B69E-DBBB05DFCDE4' )
                BEGIN
                    INSERT INTO [Attribute] ( [IsSystem], [FieldTypeId], [EntityTypeId], [EntityTypeQualifierColumn], [EntityTypeQualifierValue], [Key], [Name], [Description], [Order], [IsGridColumn], [DefaultValue], [IsMultiValue], [IsRequired], [Guid] )
                    VALUES ( 0, @FieldTypeId, @EntityTypeId, 'ContentChannelId', CAST( @ChannelId AS NVARCHAR(10) ), 'Image', 'Image', '', 1002, 0, '', 0, 0, '8D6E1DED-2A74-4FD1-B69E-DBBB05DFCDE4' );
                END
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = 'C60EF58D-2402-45D5-8B5B-A41403F55E7C' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '8D6E1DED-2A74-4FD1-B69E-DBBB05DFCDE4' ), 'binaryFileType', '8dbf874c-f3c2-4848-8137-c963c431eb0b', 'C60EF58D-2402-45D5-8B5B-A41403F55E7C';
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = '4D1FA99E-D76E-4E36-A01C-64ACA9723760' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '8D6E1DED-2A74-4FD1-B69E-DBBB05DFCDE4' ), 'formatAsLink', 'False', '4D1FA99E-D76E-4E36-A01C-64ACA9723760';
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = '58528C82-9DC8-4D28-A14D-657BBB645051' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '8D6E1DED-2A74-4FD1-B69E-DBBB05DFCDE4' ), 'img_tag_template', '', '58528C82-9DC8-4D28-A14D-657BBB645051';
                " );   // App Home Feed . Image
            Sql( @"
                DECLARE @ChannelId INT = ( SELECT TOP 1 [Id] FROM [ContentChannel] WHERE [Guid] = '122CAAAE-0698-4869-89FE-D818E109BAEA' );
                DECLARE @EntityTypeId INT = ( SELECT TOP 1 [Id] FROM [EntityType] WHERE [Name] = 'Rock.Model.ContentChannelItem' );
                DECLARE @FieldTypeId INT = ( SELECT TOP 1 [Id] FROM [FieldType] WHERE [Guid] = '9C204CD0-1233-41C5-818A-C5DA439445AA' );
                IF @ChannelId IS NOT NULL AND @FieldTypeId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [Attribute] WHERE [Guid] = 'CB07C7DC-7274-4D9C-9210-BAAF934D8AE7' )
                BEGIN
                    INSERT INTO [Attribute] ( [IsSystem], [FieldTypeId], [EntityTypeId], [EntityTypeQualifierColumn], [EntityTypeQualifierValue], [Key], [Name], [Description], [Order], [IsGridColumn], [DefaultValue], [IsMultiValue], [IsRequired], [Guid] )
                    VALUES ( 0, @FieldTypeId, @EntityTypeId, 'ContentChannelId', CAST( @ChannelId AS NVARCHAR(10) ), 'ImageUrl', 'Image URL', 'External image URL, such as from a CDN Provider or other resource. This will override the ""Image"" attribute.', 1003, 0, '', 0, 0, 'CB07C7DC-7274-4D9C-9210-BAAF934D8AE7' );
                END
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = '1FBDD0E4-CBB4-4B43-BB0D-FBC9075EB622' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = 'CB07C7DC-7274-4D9C-9210-BAAF934D8AE7' ), 'ispassword', 'False', '1FBDD0E4-CBB4-4B43-BB0D-FBC9075EB622';
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = '992B9CE9-EEB9-4006-8C27-89D7D0A47F27' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = 'CB07C7DC-7274-4D9C-9210-BAAF934D8AE7' ), 'maxcharacters', '', '992B9CE9-EEB9-4006-8C27-89D7D0A47F27';
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = '16C8481C-D47D-4D81-B1A2-B2366AFD69BE' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = 'CB07C7DC-7274-4D9C-9210-BAAF934D8AE7' ), 'showcountdown', 'False', '16C8481C-D47D-4D81-B1A2-B2366AFD69BE';
                " );   // App Home Feed . ImageUrl
            Sql( @"
                DECLARE @ChannelId INT = ( SELECT TOP 1 [Id] FROM [ContentChannel] WHERE [Guid] = '122CAAAE-0698-4869-89FE-D818E109BAEA' );
                DECLARE @EntityTypeId INT = ( SELECT TOP 1 [Id] FROM [EntityType] WHERE [Name] = 'Rock.Model.ContentChannelItem' );
                DECLARE @FieldTypeId INT = ( SELECT TOP 1 [Id] FROM [FieldType] WHERE [Guid] = '1EDAFDED-DFE6-4334-B019-6EECBA89E05A' );
                IF @ChannelId IS NOT NULL AND @FieldTypeId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [Attribute] WHERE [Guid] = '9E36D033-83A7-4114-B7F3-94583912E538' )
                BEGIN
                    INSERT INTO [Attribute] ( [IsSystem], [FieldTypeId], [EntityTypeId], [EntityTypeQualifierColumn], [EntityTypeQualifierValue], [Key], [Name], [Description], [Order], [IsGridColumn], [DefaultValue], [IsMultiValue], [IsRequired], [Guid] )
                    VALUES ( 0, @FieldTypeId, @EntityTypeId, 'ContentChannelId', CAST( @ChannelId AS NVARCHAR(10) ), 'ShowTitle', 'Show Title', 'Show the item Title on the card', 1004, 0, 'True', 0, 1, '9E36D033-83A7-4114-B7F3-94583912E538' );
                END
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = '4F30255A-932E-4ECE-B32B-2BA0BD95ACA1' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '9E36D033-83A7-4114-B7F3-94583912E538' ), 'BooleanControlType', '0', '4F30255A-932E-4ECE-B32B-2BA0BD95ACA1';
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = 'A112D6DF-23A4-42CC-9218-BBEE4F2D4C7A' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '9E36D033-83A7-4114-B7F3-94583912E538' ), 'falsetext', 'No', 'A112D6DF-23A4-42CC-9218-BBEE4F2D4C7A';
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = 'A0E41A3C-3197-4101-8A25-FDEEE3C67237' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '9E36D033-83A7-4114-B7F3-94583912E538' ), 'truetext', 'Yes', 'A0E41A3C-3197-4101-8A25-FDEEE3C67237';
                " );   // App Home Feed . ShowTitle
            Sql( @"
                DECLARE @ChannelId INT = ( SELECT TOP 1 [Id] FROM [ContentChannel] WHERE [Guid] = '122CAAAE-0698-4869-89FE-D818E109BAEA' );
                DECLARE @EntityTypeId INT = ( SELECT TOP 1 [Id] FROM [EntityType] WHERE [Name] = 'Rock.Model.ContentChannelItem' );
                DECLARE @FieldTypeId INT = ( SELECT TOP 1 [Id] FROM [FieldType] WHERE [Guid] = '1EDAFDED-DFE6-4334-B019-6EECBA89E05A' );
                IF @ChannelId IS NOT NULL AND @FieldTypeId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [Attribute] WHERE [Guid] = '75AAC674-FCFD-4C4F-A113-BDA278E14D12' )
                BEGIN
                    INSERT INTO [Attribute] ( [IsSystem], [FieldTypeId], [EntityTypeId], [EntityTypeQualifierColumn], [EntityTypeQualifierValue], [Key], [Name], [Description], [Order], [IsGridColumn], [DefaultValue], [IsMultiValue], [IsRequired], [Guid] )
                    VALUES ( 0, @FieldTypeId, @EntityTypeId, 'ContentChannelId', CAST( @ChannelId AS NVARCHAR(10) ), 'DisplayDetailsButton', 'Display Details Button', 'Show a blurb + ""SEE DETAILS"" button.', 1005, 0, 'False', 0, 1, '75AAC674-FCFD-4C4F-A113-BDA278E14D12' );
                END
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = 'D20E0472-B357-442B-A3D2-D93FBEDA72FE' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '75AAC674-FCFD-4C4F-A113-BDA278E14D12' ), 'BooleanControlType', '0', 'D20E0472-B357-442B-A3D2-D93FBEDA72FE';
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = 'F6AC209C-3C51-4347-B40D-DCE94F24FDEF' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '75AAC674-FCFD-4C4F-A113-BDA278E14D12' ), 'falsetext', 'No', 'F6AC209C-3C51-4347-B40D-DCE94F24FDEF';
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = '6497F45B-169F-4811-949B-BC431EF6E18C' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '75AAC674-FCFD-4C4F-A113-BDA278E14D12' ), 'truetext', 'Yes', '6497F45B-169F-4811-949B-BC431EF6E18C';
                " );   // App Home Feed . DisplayDetailsButton
            Sql( @"
                DECLARE @ChannelId INT = ( SELECT TOP 1 [Id] FROM [ContentChannel] WHERE [Guid] = '122CAAAE-0698-4869-89FE-D818E109BAEA' );
                DECLARE @EntityTypeId INT = ( SELECT TOP 1 [Id] FROM [EntityType] WHERE [Name] = 'Rock.Model.ContentChannelItem' );
                DECLARE @FieldTypeId INT = ( SELECT TOP 1 [Id] FROM [FieldType] WHERE [Guid] = '7525C4CB-EE6B-41D4-9B64-A08048D5A5C0' );
                IF @ChannelId IS NOT NULL AND @FieldTypeId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [Attribute] WHERE [Guid] = 'CE0ACC4F-161C-45DF-A908-24A5596CCC21' )
                BEGIN
                    INSERT INTO [Attribute] ( [IsSystem], [FieldTypeId], [EntityTypeId], [EntityTypeQualifierColumn], [EntityTypeQualifierValue], [Key], [Name], [Description], [Order], [IsGridColumn], [DefaultValue], [IsMultiValue], [IsRequired], [Guid] )
                    VALUES ( 0, @FieldTypeId, @EntityTypeId, 'ContentChannelId', CAST( @ChannelId AS NVARCHAR(10) ), 'CardWidth', 'Card Width', 'Full = edge-to-edge
Contained = side margins', 1006, 0, 'Contained', 0, 0, 'CE0ACC4F-161C-45DF-A908-24A5596CCC21' );
                END
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = '0C83E34D-8194-4754-8232-1B7E46DA64FE' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = 'CE0ACC4F-161C-45DF-A908-24A5596CCC21' ), 'fieldtype', 'ddl', '0C83E34D-8194-4754-8232-1B7E46DA64FE';
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = '0FF042BA-7638-49A7-91C6-2EBAD5080D75' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = 'CE0ACC4F-161C-45DF-A908-24A5596CCC21' ), 'repeatColumns', '', '0FF042BA-7638-49A7-91C6-2EBAD5080D75';
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = 'D5F1E7B0-9BFD-4E0C-9DB0-89FDB9398474' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = 'CE0ACC4F-161C-45DF-A908-24A5596CCC21' ), 'values', 'Full, Contained', 'D5F1E7B0-9BFD-4E0C-9DB0-89FDB9398474';
                " );   // App Home Feed . CardWidth
            Sql( @"
                DECLARE @ChannelId INT = ( SELECT TOP 1 [Id] FROM [ContentChannel] WHERE [Guid] = '122CAAAE-0698-4869-89FE-D818E109BAEA' );
                DECLARE @EntityTypeId INT = ( SELECT TOP 1 [Id] FROM [EntityType] WHERE [Name] = 'Rock.Model.ContentChannelItem' );
                DECLARE @FieldTypeId INT = ( SELECT TOP 1 [Id] FROM [FieldType] WHERE [Guid] = '7525C4CB-EE6B-41D4-9B64-A08048D5A5C0' );
                IF @ChannelId IS NOT NULL AND @FieldTypeId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [Attribute] WHERE [Guid] = 'FA7406AC-3A4D-4162-814F-FD60DCBC5E2B' )
                BEGIN
                    INSERT INTO [Attribute] ( [IsSystem], [FieldTypeId], [EntityTypeId], [EntityTypeQualifierColumn], [EntityTypeQualifierValue], [Key], [Name], [Description], [Order], [IsGridColumn], [DefaultValue], [IsMultiValue], [IsRequired], [Guid] )
                    VALUES ( 0, @FieldTypeId, @EntityTypeId, 'ContentChannelId', CAST( @ChannelId AS NVARCHAR(10) ), 'ShadowDepth', 'Shadow Depth', 'Card elevation/shadow', 1007, 0, '4', 0, 0, 'FA7406AC-3A4D-4162-814F-FD60DCBC5E2B' );
                END
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = 'DE70B2CF-DEB8-44A3-8080-4EF77EB367B1' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = 'FA7406AC-3A4D-4162-814F-FD60DCBC5E2B' ), 'fieldtype', 'ddl', 'DE70B2CF-DEB8-44A3-8080-4EF77EB367B1';
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = '189F2555-6E3A-4425-9261-0A5370BFB07C' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = 'FA7406AC-3A4D-4162-814F-FD60DCBC5E2B' ), 'repeatColumns', '', '189F2555-6E3A-4425-9261-0A5370BFB07C';
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = '19361DC4-ED96-4F8D-8F3F-1D23C6B708A9' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = 'FA7406AC-3A4D-4162-814F-FD60DCBC5E2B' ), 'values', '0,2,4,8,16', '19361DC4-ED96-4F8D-8F3F-1D23C6B708A9';
                " );   // App Home Feed . ShadowDepth
            Sql( @"
                DECLARE @ChannelId INT = ( SELECT TOP 1 [Id] FROM [ContentChannel] WHERE [Guid] = '122CAAAE-0698-4869-89FE-D818E109BAEA' );
                DECLARE @EntityTypeId INT = ( SELECT TOP 1 [Id] FROM [EntityType] WHERE [Name] = 'Rock.Model.ContentChannelItem' );
                DECLARE @FieldTypeId INT = ( SELECT TOP 1 [Id] FROM [FieldType] WHERE [Guid] = '7525C4CB-EE6B-41D4-9B64-A08048D5A5C0' );
                IF @ChannelId IS NOT NULL AND @FieldTypeId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [Attribute] WHERE [Guid] = 'FCE7A15B-4EA0-4E06-A04D-02159B79331F' )
                BEGIN
                    INSERT INTO [Attribute] ( [IsSystem], [FieldTypeId], [EntityTypeId], [EntityTypeQualifierColumn], [EntityTypeQualifierValue], [Key], [Name], [Description], [Order], [IsGridColumn], [DefaultValue], [IsMultiValue], [IsRequired], [Guid] )
                    VALUES ( 0, @FieldTypeId, @EntityTypeId, 'ContentChannelId', CAST( @ChannelId AS NVARCHAR(10) ), 'ImageAspectRatio', 'Image Aspect Ratio', 'Image Crop ratio', 1008, 0, '', 0, 0, 'FCE7A15B-4EA0-4E06-A04D-02159B79331F' );
                END
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = '2906A5BA-8D8F-49EF-B159-400957AA5CCE' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = 'FCE7A15B-4EA0-4E06-A04D-02159B79331F' ), 'fieldtype', 'ddl', '2906A5BA-8D8F-49EF-B159-400957AA5CCE';
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = '6526515D-D2DB-4C59-8C1C-35DF14D0B776' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = 'FCE7A15B-4EA0-4E06-A04D-02159B79331F' ), 'repeatColumns', '', '6526515D-D2DB-4C59-8C1C-35DF14D0B776';
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = '15367506-EFCF-414D-99DB-25EF64FABB7B' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = 'FCE7A15B-4EA0-4E06-A04D-02159B79331F' ), 'values', '1:1,4:3,16:9,2:1', '15367506-EFCF-414D-99DB-25EF64FABB7B';
                " );   // App Home Feed . ImageAspectRatio
            Sql( @"
                DECLARE @ChannelId INT = ( SELECT TOP 1 [Id] FROM [ContentChannel] WHERE [Guid] = '122CAAAE-0698-4869-89FE-D818E109BAEA' );
                DECLARE @EntityTypeId INT = ( SELECT TOP 1 [Id] FROM [EntityType] WHERE [Name] = 'Rock.Model.ContentChannelItem' );
                DECLARE @FieldTypeId INT = ( SELECT TOP 1 [Id] FROM [FieldType] WHERE [Guid] = '7525C4CB-EE6B-41D4-9B64-A08048D5A5C0' );
                IF @ChannelId IS NOT NULL AND @FieldTypeId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [Attribute] WHERE [Guid] = '97BAEB0F-2E0B-4280-AA5B-C7CAA04C347B' )
                BEGIN
                    INSERT INTO [Attribute] ( [IsSystem], [FieldTypeId], [EntityTypeId], [EntityTypeQualifierColumn], [EntityTypeQualifierValue], [Key], [Name], [Description], [Order], [IsGridColumn], [DefaultValue], [IsMultiValue], [IsRequired], [Guid] )
                    VALUES ( 0, @FieldTypeId, @EntityTypeId, 'ContentChannelId', CAST( @ChannelId AS NVARCHAR(10) ), 'LinktoURLType', 'Link to URL Type', 'How a tapped link opens', 1009, 0, '', 0, 0, '97BAEB0F-2E0B-4280-AA5B-C7CAA04C347B' );
                END
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = '37512DB7-4F5B-4505-8A67-5B8EEF285A00' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '97BAEB0F-2E0B-4280-AA5B-C7CAA04C347B' ), 'fieldtype', 'ddl', '37512DB7-4F5B-4505-8A67-5B8EEF285A00';
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = 'BA294326-A783-4B0B-AC7A-4E9372677EFB' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '97BAEB0F-2E0B-4280-AA5B-C7CAA04C347B' ), 'repeatColumns', '', 'BA294326-A783-4B0B-AC7A-4E9372677EFB';
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = 'C2CE369D-14AF-4324-87E0-48E4D8A3EFEA' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '97BAEB0F-2E0B-4280-AA5B-C7CAA04C347B' ), 'values', 'External Browser, Internal Browser, Webview', 'C2CE369D-14AF-4324-87E0-48E4D8A3EFEA';
                " );   // App Home Feed . LinktoURLType
            Sql( @"
                DECLARE @ChannelId INT = ( SELECT TOP 1 [Id] FROM [ContentChannel] WHERE [Guid] = '122CAAAE-0698-4869-89FE-D818E109BAEA' );
                DECLARE @EntityTypeId INT = ( SELECT TOP 1 [Id] FROM [EntityType] WHERE [Name] = 'Rock.Model.ContentChannelItem' );
                DECLARE @FieldTypeId INT = ( SELECT TOP 1 [Id] FROM [FieldType] WHERE [Guid] = '9C204CD0-1233-41C5-818A-C5DA439445AA' );
                IF @ChannelId IS NOT NULL AND @FieldTypeId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [Attribute] WHERE [Guid] = '91C526A0-E675-475C-8C0C-221124BB35EB' )
                BEGIN
                    INSERT INTO [Attribute] ( [IsSystem], [FieldTypeId], [EntityTypeId], [EntityTypeQualifierColumn], [EntityTypeQualifierValue], [Key], [Name], [Description], [Order], [IsGridColumn], [DefaultValue], [IsMultiValue], [IsRequired], [Guid] )
                    VALUES ( 0, @FieldTypeId, @EntityTypeId, 'ContentChannelId', CAST( @ChannelId AS NVARCHAR(10) ), 'LinktoURL', 'Link to URL', 'URL to open on tap instead of opening a detail page to this content channel item.', 1010, 0, '', 0, 0, '91C526A0-E675-475C-8C0C-221124BB35EB' );
                END
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = '4B3B7A3C-AE7C-46E2-9EB8-975D3171F7D8' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '91C526A0-E675-475C-8C0C-221124BB35EB' ), 'ispassword', 'False', '4B3B7A3C-AE7C-46E2-9EB8-975D3171F7D8';
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = '4083B5A3-CC05-4BB2-B5CC-F67768A88E07' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '91C526A0-E675-475C-8C0C-221124BB35EB' ), 'maxcharacters', '', '4083B5A3-CC05-4BB2-B5CC-F67768A88E07';
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = '612CDA2E-B6BF-48B0-BB4F-90591269474A' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '91C526A0-E675-475C-8C0C-221124BB35EB' ), 'showcountdown', 'False', '612CDA2E-B6BF-48B0-BB4F-90591269474A';
                " );   // App Home Feed . LinktoURL
            Sql( @"
                DECLARE @ChannelId INT = ( SELECT TOP 1 [Id] FROM [ContentChannel] WHERE [Guid] = '122CAAAE-0698-4869-89FE-D818E109BAEA' );
                DECLARE @EntityTypeId INT = ( SELECT TOP 1 [Id] FROM [EntityType] WHERE [Name] = 'Rock.Model.ContentChannelItem' );
                DECLARE @FieldTypeId INT = ( SELECT TOP 1 [Id] FROM [FieldType] WHERE [Guid] = 'BD53F9C9-EBA9-4D3F-82EA-DE5DD34A8108' );
                IF @ChannelId IS NOT NULL AND @FieldTypeId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [Attribute] WHERE [Guid] = 'AA78BD64-E376-4896-A487-7F8C4C7A6778' )
                BEGIN
                    INSERT INTO [Attribute] ( [IsSystem], [FieldTypeId], [EntityTypeId], [EntityTypeQualifierColumn], [EntityTypeQualifierValue], [Key], [Name], [Description], [Order], [IsGridColumn], [DefaultValue], [IsMultiValue], [IsRequired], [Guid] )
                    VALUES ( 0, @FieldTypeId, @EntityTypeId, 'ContentChannelId', CAST( @ChannelId AS NVARCHAR(10) ), 'LinktoAppPage', 'Link to App Page', 'Mobile page to push to the screen on tap (so you don''t have to know Page GUID''s). Be sure to select one under ""Nfluence Church App Homepage"".', 1011, 0, '', 0, 0, 'AA78BD64-E376-4896-A487-7F8C4C7A6778' );
                END

                " );   // App Home Feed . LinktoAppPage
            Sql( @"
                DECLARE @ChannelId INT = ( SELECT TOP 1 [Id] FROM [ContentChannel] WHERE [Guid] = '122CAAAE-0698-4869-89FE-D818E109BAEA' );
                DECLARE @EntityTypeId INT = ( SELECT TOP 1 [Id] FROM [EntityType] WHERE [Name] = 'Rock.Model.ContentChannelItem' );
                DECLARE @FieldTypeId INT = ( SELECT TOP 1 [Id] FROM [FieldType] WHERE [Guid] = '1EDAFDED-DFE6-4334-B019-6EECBA89E05A' );
                IF @ChannelId IS NOT NULL AND @FieldTypeId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [Attribute] WHERE [Guid] = 'F24E8F59-3B62-4FDE-B2A0-AB7B0118B754' )
                BEGIN
                    INSERT INTO [Attribute] ( [IsSystem], [FieldTypeId], [EntityTypeId], [EntityTypeQualifierColumn], [EntityTypeQualifierValue], [Key], [Name], [Description], [Order], [IsGridColumn], [DefaultValue], [IsMultiValue], [IsRequired], [Guid] )
                    VALUES ( 0, @FieldTypeId, @EntityTypeId, 'ContentChannelId', CAST( @ChannelId AS NVARCHAR(10) ), 'PassUserAuthentication', 'Pass User Authentication', 'Appends a person token to the URL. This is used primarily for external links to your Rock web page where you would still like the user to be authenticated to view.', 1012, 0, 'False', 0, 1, 'F24E8F59-3B62-4FDE-B2A0-AB7B0118B754' );
                END
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = 'D48BA9BB-FFAE-41C6-B75D-82F3B1921883' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = 'F24E8F59-3B62-4FDE-B2A0-AB7B0118B754' ), 'BooleanControlType', '0', 'D48BA9BB-FFAE-41C6-B75D-82F3B1921883';
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = '4C656CA0-42F4-4F86-8BC2-1962EF7F8470' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = 'F24E8F59-3B62-4FDE-B2A0-AB7B0118B754' ), 'falsetext', 'No', '4C656CA0-42F4-4F86-8BC2-1962EF7F8470';
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = '49A388EE-578C-4B9A-8E22-FE75530A3467' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = 'F24E8F59-3B62-4FDE-B2A0-AB7B0118B754' ), 'truetext', 'Yes', '49A388EE-578C-4B9A-8E22-FE75530A3467';
                " );   // App Home Feed . PassUserAuthentication
            Sql( @"
                DECLARE @ChannelId INT = ( SELECT TOP 1 [Id] FROM [ContentChannel] WHERE [Guid] = '122CAAAE-0698-4869-89FE-D818E109BAEA' );
                DECLARE @EntityTypeId INT = ( SELECT TOP 1 [Id] FROM [EntityType] WHERE [Name] = 'Rock.Model.ContentChannelItem' );
                DECLARE @FieldTypeId INT = ( SELECT TOP 1 [Id] FROM [FieldType] WHERE [Guid] = '23713932-F558-45F7-BB00-2A550852F70D' );
                IF @ChannelId IS NOT NULL AND @FieldTypeId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [Attribute] WHERE [Guid] = '9C026AB7-E28F-4B59-968A-53115C0D9726' )
                BEGIN
                    INSERT INTO [Attribute] ( [IsSystem], [FieldTypeId], [EntityTypeId], [EntityTypeQualifierColumn], [EntityTypeQualifierValue], [Key], [Name], [Description], [Order], [IsGridColumn], [DefaultValue], [IsMultiValue], [IsRequired], [Guid] )
                    VALUES ( 0, @FieldTypeId, @EntityTypeId, 'ContentChannelId', CAST( @ChannelId AS NVARCHAR(10) ), 'EventItem', 'Event Item', 'If you would like to pull all of the details from an event item, enter a title and select the event item, it will pull the next upcoming occurrence of the event.', 1013, 0, '', 0, 0, '9C026AB7-E28F-4B59-968A-53115C0D9726' );
                END

                " );   // App Home Feed . EventItem
            Sql( @"
                DECLARE @ChannelId INT = ( SELECT TOP 1 [Id] FROM [ContentChannel] WHERE [Guid] = '122CAAAE-0698-4869-89FE-D818E109BAEA' );
                DECLARE @EntityTypeId INT = ( SELECT TOP 1 [Id] FROM [EntityType] WHERE [Name] = 'Rock.Model.ContentChannelItem' );
                DECLARE @FieldTypeId INT = ( SELECT TOP 1 [Id] FROM [FieldType] WHERE [Guid] = '9C204CD0-1233-41C5-818A-C5DA439445AA' );
                IF @ChannelId IS NOT NULL AND @FieldTypeId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [Attribute] WHERE [Guid] = '07FDE24E-3F5E-4FD5-AE80-C952B8C0858F' )
                BEGIN
                    INSERT INTO [Attribute] ( [IsSystem], [FieldTypeId], [EntityTypeId], [EntityTypeQualifierColumn], [EntityTypeQualifierValue], [Key], [Name], [Description], [Order], [IsGridColumn], [DefaultValue], [IsMultiValue], [IsRequired], [Guid] )
                    VALUES ( 0, @FieldTypeId, @EntityTypeId, 'ContentChannelId', CAST( @ChannelId AS NVARCHAR(10) ), 'PageParameters', 'Page Parameters', 'If using the App Page setting, what parameters should we send with it? Could be a content channel item id, event id, etc.', 1014, 0, '', 0, 0, '07FDE24E-3F5E-4FD5-AE80-C952B8C0858F' );
                END
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = '1D314824-AA5D-43E4-957B-BF37D7A9721E' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '07FDE24E-3F5E-4FD5-AE80-C952B8C0858F' ), 'ispassword', 'False', '1D314824-AA5D-43E4-957B-BF37D7A9721E';
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = 'F9E4FE5E-D6DD-43FC-8CFF-1E9B05C6EA6C' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '07FDE24E-3F5E-4FD5-AE80-C952B8C0858F' ), 'maxcharacters', '', 'F9E4FE5E-D6DD-43FC-8CFF-1E9B05C6EA6C';
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = 'E2A11F17-8287-4883-AC05-B43605204F03' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '07FDE24E-3F5E-4FD5-AE80-C952B8C0858F' ), 'showcountdown', 'False', 'E2A11F17-8287-4883-AC05-B43605204F03';
                " );   // App Home Feed . PageParameters
            Sql( @"
                DECLARE @ChannelId INT = ( SELECT TOP 1 [Id] FROM [ContentChannel] WHERE [Guid] = '122CAAAE-0698-4869-89FE-D818E109BAEA' );
                DECLARE @EntityTypeId INT = ( SELECT TOP 1 [Id] FROM [EntityType] WHERE [Name] = 'Rock.Model.ContentChannelItem' );
                DECLARE @FieldTypeId INT = ( SELECT TOP 1 [Id] FROM [FieldType] WHERE [Guid] = 'C0D0D7E2-C3B0-4004-ABEA-4BBFAD10D5D2' );
                IF @ChannelId IS NOT NULL AND @FieldTypeId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [Attribute] WHERE [Guid] = 'E2C6B94A-1F73-4E58-B0D9-6A4E8C15F7B3' )
                BEGIN
                    INSERT INTO [Attribute] ( [IsSystem], [FieldTypeId], [EntityTypeId], [EntityTypeQualifierColumn], [EntityTypeQualifierValue], [Key], [Name], [Description], [Order], [IsGridColumn], [DefaultValue], [IsMultiValue], [IsRequired], [Guid] )
                    VALUES ( 0, @FieldTypeId, @EntityTypeId, 'ContentChannelId', CAST( @ChannelId AS NVARCHAR(10) ), 'LinkUrl', 'Button Link Url', 'Adds a call-to-action button to the item detail page.', 1015, 0, '', 0, 0, 'E2C6B94A-1F73-4E58-B0D9-6A4E8C15F7B3' );
                END

                " );   // App Home Feed . LinkUrl
            Sql( @"
                DECLARE @ChannelId INT = ( SELECT TOP 1 [Id] FROM [ContentChannel] WHERE [Guid] = '122CAAAE-0698-4869-89FE-D818E109BAEA' );
                DECLARE @EntityTypeId INT = ( SELECT TOP 1 [Id] FROM [EntityType] WHERE [Name] = 'Rock.Model.ContentChannelItem' );
                DECLARE @FieldTypeId INT = ( SELECT TOP 1 [Id] FROM [FieldType] WHERE [Guid] = '9C204CD0-1233-41C5-818A-C5DA439445AA' );
                IF @ChannelId IS NOT NULL AND @FieldTypeId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [Attribute] WHERE [Guid] = 'F8D50A21-4C96-4B7E-83A1-2D9F6E0B85C4' )
                BEGIN
                    INSERT INTO [Attribute] ( [IsSystem], [FieldTypeId], [EntityTypeId], [EntityTypeQualifierColumn], [EntityTypeQualifierValue], [Key], [Name], [Description], [Order], [IsGridColumn], [DefaultValue], [IsMultiValue], [IsRequired], [Guid] )
                    VALUES ( 0, @FieldTypeId, @EntityTypeId, 'ContentChannelId', CAST( @ChannelId AS NVARCHAR(10) ), 'LinkButtonText', 'Button Link Text', 'Label for the link button. Defaults to ""Learn More"".', 1016, 0, '', 0, 0, 'F8D50A21-4C96-4B7E-83A1-2D9F6E0B85C4' );
                END

                " );   // App Home Feed . LinkButtonText
            Sql( @"
                DECLARE @ChannelId INT = ( SELECT TOP 1 [Id] FROM [ContentChannel] WHERE [Guid] = '122CAAAE-0698-4869-89FE-D818E109BAEA' );
                DECLARE @EntityTypeId INT = ( SELECT TOP 1 [Id] FROM [EntityType] WHERE [Name] = 'Rock.Model.ContentChannelItem' );
                DECLARE @FieldTypeId INT = ( SELECT TOP 1 [Id] FROM [FieldType] WHERE [Guid] = '1EDAFDED-DFE6-4334-B019-6EECBA89E05A' );
                IF @ChannelId IS NOT NULL AND @FieldTypeId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [Attribute] WHERE [Guid] = '3B7E1D68-9A05-4C2F-B54D-7E8A0C36F91D' )
                BEGIN
                    INSERT INTO [Attribute] ( [IsSystem], [FieldTypeId], [EntityTypeId], [EntityTypeQualifierColumn], [EntityTypeQualifierValue], [Key], [Name], [Description], [Order], [IsGridColumn], [DefaultValue], [IsMultiValue], [IsRequired], [Guid] )
                    VALUES ( 0, @FieldTypeId, @EntityTypeId, 'ContentChannelId', CAST( @ChannelId AS NVARCHAR(10) ), 'LinkOpensExternally', 'Button Link Opens Externally', 'On: hand off to the device browser. Off (default): open inside the app.', 1017, 0, 'False', 0, 0, '3B7E1D68-9A05-4C2F-B54D-7E8A0C36F91D' );
                END

                " );   // App Home Feed . LinkOpensExternally
            Sql( @"
                DECLARE @ChannelId INT = ( SELECT TOP 1 [Id] FROM [ContentChannel] WHERE [Guid] = '122CAAAE-0698-4869-89FE-D818E109BAEA' );
                DECLARE @EntityTypeId INT = ( SELECT TOP 1 [Id] FROM [EntityType] WHERE [Name] = 'Rock.Model.ContentChannelItem' );
                DECLARE @FieldTypeId INT = ( SELECT TOP 1 [Id] FROM [FieldType] WHERE [Guid] = 'BD53F9C9-EBA9-4D3F-82EA-DE5DD34A8108' );
                IF @ChannelId IS NOT NULL AND @FieldTypeId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [Attribute] WHERE [Guid] = '6D2F81A4-3E57-4C0B-9A16-7B8E4D05C2F3' )
                BEGIN
                    INSERT INTO [Attribute] ( [IsSystem], [FieldTypeId], [EntityTypeId], [EntityTypeQualifierColumn], [EntityTypeQualifierValue], [Key], [Name], [Description], [Order], [IsGridColumn], [DefaultValue], [IsMultiValue], [IsRequired], [Guid] )
                    VALUES ( 0, @FieldTypeId, @EntityTypeId, 'ContentChannelId', CAST( @ChannelId AS NVARCHAR(10) ), 'LinkAppPage', 'Button Link App Page', 'Opens a page inside the app when the call-to-action button is tapped. Takes precedence over Button Link Url if both are set. Point it at a page containing a WebView or Workflow Entry block to open a web page or start a workflow.', 1018, 0, '', 0, 0, '6D2F81A4-3E57-4C0B-9A16-7B8E4D05C2F3' );
                END

                " );   // App Home Feed . LinkAppPage
            Sql( @"
                DECLARE @ChannelId INT = ( SELECT TOP 1 [Id] FROM [ContentChannel] WHERE [Guid] = '122CAAAE-0698-4869-89FE-D818E109BAEA' );
                DECLARE @EntityTypeId INT = ( SELECT TOP 1 [Id] FROM [EntityType] WHERE [Name] = 'Rock.Model.ContentChannelItem' );
                DECLARE @FieldTypeId INT = ( SELECT TOP 1 [Id] FROM [FieldType] WHERE [Guid] = '9C204CD0-1233-41C5-818A-C5DA439445AA' );
                IF @ChannelId IS NOT NULL AND @FieldTypeId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [Attribute] WHERE [Guid] = '4E9B0C57-8A31-4F6D-B25C-0D71E3A8F94B' )
                BEGIN
                    INSERT INTO [Attribute] ( [IsSystem], [FieldTypeId], [EntityTypeId], [EntityTypeQualifierColumn], [EntityTypeQualifierValue], [Key], [Name], [Description], [Order], [IsGridColumn], [DefaultValue], [IsMultiValue], [IsRequired], [Guid] )
                    VALUES ( 0, @FieldTypeId, @EntityTypeId, 'ContentChannelId', CAST( @ChannelId AS NVARCHAR(10) ), 'LinkPageParameters', 'Button Link Page Parameters', 'Query string passed to Button Link App Page, e.g. ""?WorkflowTypeGuid=abc"" or ""WorkflowTypeGuid=abc"". The leading ? is optional. Ignored unless Button Link App Page is set.', 1019, 0, '', 0, 0, '4E9B0C57-8A31-4F6D-B25C-0D71E3A8F94B' );
                END

                " );   // App Home Feed . LinkPageParameters
            Sql( @"
                DECLARE @ChannelId INT = ( SELECT TOP 1 [Id] FROM [ContentChannel] WHERE [Guid] = '122CAAAE-0698-4869-89FE-D818E109BAEA' );
                DECLARE @EntityTypeId INT = ( SELECT TOP 1 [Id] FROM [EntityType] WHERE [Name] = 'Rock.Model.ContentChannelItem' );
                DECLARE @FieldTypeId INT = ( SELECT TOP 1 [Id] FROM [FieldType] WHERE [Guid] = '7525C4CB-EE6B-41D4-9B64-A08048D5A5C0' );
                IF @ChannelId IS NOT NULL AND @FieldTypeId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [Attribute] WHERE [Guid] = '0C4B7F93-5D28-4A61-8E07-3F92D6C41B85' )
                BEGIN
                    INSERT INTO [Attribute] ( [IsSystem], [FieldTypeId], [EntityTypeId], [EntityTypeQualifierColumn], [EntityTypeQualifierValue], [Key], [Name], [Description], [Order], [IsGridColumn], [DefaultValue], [IsMultiValue], [IsRequired], [Guid] )
                    VALUES ( 0, @FieldTypeId, @EntityTypeId, 'ContentChannelId', CAST( @ChannelId AS NVARCHAR(10) ), 'ShareMode', 'Share Button', 'Shows a share icon in the header. ""Share this item"" needs a public web page for the item to exist; ""Share the button link"" hands out Button Link Url and renders nothing if the item uses an app page instead.', 1020, 0, 'None', 0, 0, '0C4B7F93-5D28-4A61-8E07-3F92D6C41B85' );
                END
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = 'B1E70A94-5C38-4D26-9F03-7A61D8025E4C' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '0C4B7F93-5D28-4A61-8E07-3F92D6C41B85' ), 'fieldtype', 'ddl', 'B1E70A94-5C38-4D26-9F03-7A61D8025E4C';
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = 'D3092CB6-7E5A-4F48-B125-9C83FA247061' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '0C4B7F93-5D28-4A61-8E07-3F92D6C41B85' ), 'repeatColumns', '', 'D3092CB6-7E5A-4F48-B125-9C83FA247061';
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = 'C2F81BA5-6D49-4E37-A014-8B72E9136F5D' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '0C4B7F93-5D28-4A61-8E07-3F92D6C41B85' ), 'values', 'None^No share,Item^Share this item,Link^Share the button link', 'C2F81BA5-6D49-4E37-A014-8B72E9136F5D';
                " );   // App Home Feed . ShareMode
            Sql( @"
                DECLARE @ChannelId INT = ( SELECT TOP 1 [Id] FROM [ContentChannel] WHERE [Guid] = '122CAAAE-0698-4869-89FE-D818E109BAEA' );
                DECLARE @EntityTypeId INT = ( SELECT TOP 1 [Id] FROM [EntityType] WHERE [Name] = 'Rock.Model.ContentChannelItem' );
                DECLARE @FieldTypeId INT = ( SELECT TOP 1 [Id] FROM [FieldType] WHERE [Guid] = '1EDAFDED-DFE6-4334-B019-6EECBA89E05A' );
                IF @ChannelId IS NOT NULL AND @FieldTypeId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [Attribute] WHERE [Guid] = '2A6E4C81-9B37-4D50-8F62-1E05D7A93C46' )
                BEGIN
                    INSERT INTO [Attribute] ( [IsSystem], [FieldTypeId], [EntityTypeId], [EntityTypeQualifierColumn], [EntityTypeQualifierValue], [Key], [Name], [Description], [Order], [IsGridColumn], [DefaultValue], [IsMultiValue], [IsRequired], [Guid] )
                    VALUES ( 0, @FieldTypeId, @EntityTypeId, 'ContentChannelId', CAST( @ChannelId AS NVARCHAR(10) ), 'ShowDetailTitle', 'Show Title (Detail Page)', 'Off hides the large title in the body of the detail page. The header/nav bar always keeps the title. Separate from ""Show Title"", which controls the card.', 1021, 0, 'True', 0, 0, '2A6E4C81-9B37-4D50-8F62-1E05D7A93C46' );
                END
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = 'E41A3DC7-8F6B-4059-C236-AD940B358172' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '2A6E4C81-9B37-4D50-8F62-1E05D7A93C46' ), 'BooleanControlType', '0', 'E41A3DC7-8F6B-4059-C236-AD940B358172';
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = '063C5FE9-A17D-4271-E458-CFB62D57A394' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '2A6E4C81-9B37-4D50-8F62-1E05D7A93C46' ), 'falsetext', '', '063C5FE9-A17D-4271-E458-CFB62D57A394';
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = 'F52B4ED8-906C-4160-D347-BEA51C469283' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '2A6E4C81-9B37-4D50-8F62-1E05D7A93C46' ), 'truetext', '', 'F52B4ED8-906C-4160-D347-BEA51C469283';
                " );   // App Home Feed . ShowDetailTitle
            Sql( @"
                DECLARE @ChannelId INT = ( SELECT TOP 1 [Id] FROM [ContentChannel] WHERE [Guid] = '122CAAAE-0698-4869-89FE-D818E109BAEA' );
                DECLARE @EntityTypeId INT = ( SELECT TOP 1 [Id] FROM [EntityType] WHERE [Name] = 'Rock.Model.ContentChannelItem' );
                DECLARE @FieldTypeId INT = ( SELECT TOP 1 [Id] FROM [FieldType] WHERE [Guid] = '1EDAFDED-DFE6-4334-B019-6EECBA89E05A' );
                IF @ChannelId IS NOT NULL AND @FieldTypeId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [Attribute] WHERE [Guid] = '8B0D7F35-4A92-4E18-B76C-3D91C5E204A7' )
                BEGIN
                    INSERT INTO [Attribute] ( [IsSystem], [FieldTypeId], [EntityTypeId], [EntityTypeQualifierColumn], [EntityTypeQualifierValue], [Key], [Name], [Description], [Order], [IsGridColumn], [DefaultValue], [IsMultiValue], [IsRequired], [Guid] )
                    VALUES ( 0, @FieldTypeId, @EntityTypeId, 'ContentChannelId', CAST( @ChannelId AS NVARCHAR(10) ), 'ShowDetailDate', 'Show Date (Detail Page)', 'Off hides the date on the detail page. The speaker still shows if set.', 1022, 0, 'True', 0, 0, '8B0D7F35-4A92-4E18-B76C-3D91C5E204A7' );
                END
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = '174D60FA-B28E-4382-F569-D0C73E68B4A5' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '8B0D7F35-4A92-4E18-B76C-3D91C5E204A7' ), 'BooleanControlType', '0', '174D60FA-B28E-4382-F569-D0C73E68B4A5';
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = '396F821C-D4A0-45A4-B78B-F2E95A8AD6C7' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '8B0D7F35-4A92-4E18-B76C-3D91C5E204A7' ), 'falsetext', '', '396F821C-D4A0-45A4-B78B-F2E95A8AD6C7';
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = '285E710B-C39F-4493-A67A-E1D84F79C5B6' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '8B0D7F35-4A92-4E18-B76C-3D91C5E204A7' ), 'truetext', '', '285E710B-C39F-4493-A67A-E1D84F79C5B6';
                " );   // App Home Feed . ShowDetailDate
            Sql( @"
                DECLARE @ChannelId INT = ( SELECT TOP 1 [Id] FROM [ContentChannel] WHERE [Guid] = '122CAAAE-0698-4869-89FE-D818E109BAEA' );
                DECLARE @EntityTypeId INT = ( SELECT TOP 1 [Id] FROM [EntityType] WHERE [Name] = 'Rock.Model.ContentChannelItem' );
                DECLARE @FieldTypeId INT = ( SELECT TOP 1 [Id] FROM [FieldType] WHERE [Guid] = '19BFB635-DC31-4C1E-8BB5-CDA120890BDE' );
                IF @ChannelId IS NOT NULL AND @FieldTypeId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [Attribute] WHERE [Guid] = '7C1F4E8A-3D62-4B05-9E17-2A48C0D95F63' )
                BEGIN
                    INSERT INTO [Attribute] ( [IsSystem], [FieldTypeId], [EntityTypeId], [EntityTypeQualifierColumn], [EntityTypeQualifierValue], [Key], [Name], [Description], [Order], [IsGridColumn], [DefaultValue], [IsMultiValue], [IsRequired], [Guid] )
                    VALUES ( 0, @FieldTypeId, @EntityTypeId, 'ContentChannelId', CAST( @ChannelId AS NVARCHAR(10) ), 'LinkedItem', 'Linked Content Item', 'Point this at an existing content channel item (a message, series, notes item...) and the card is built from THAT item - its title, image and summary - instead of re-entering them here. Tapping opens the linked item. Leave the other fields blank unless you want to override what the card shows.', 1023, 0, '', 0, 0, '7C1F4E8A-3D62-4B05-9E17-2A48C0D95F63' );
                END

                " );   // App Home Feed . LinkedItem
            Sql( @"
                DECLARE @ChannelId INT = ( SELECT TOP 1 [Id] FROM [ContentChannel] WHERE [Guid] = 'DF6276E8-B00E-4E77-88D8-AB1241A4A8C7' );
                DECLARE @EntityTypeId INT = ( SELECT TOP 1 [Id] FROM [EntityType] WHERE [Name] = 'Rock.Model.ContentChannelItem' );
                DECLARE @FieldTypeId INT = ( SELECT TOP 1 [Id] FROM [FieldType] WHERE [Guid] = '7525C4CB-EE6B-41D4-9B64-A08048D5A5C0' );
                IF @ChannelId IS NOT NULL AND @FieldTypeId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [Attribute] WHERE [Guid] = '99B3387C-7526-43CC-92AB-AC8E173E95F6' )
                BEGIN
                    INSERT INTO [Attribute] ( [IsSystem], [FieldTypeId], [EntityTypeId], [EntityTypeQualifierColumn], [EntityTypeQualifierValue], [Key], [Name], [Description], [Order], [IsGridColumn], [DefaultValue], [IsMultiValue], [IsRequired], [Guid] )
                    VALUES ( 0, @FieldTypeId, @EntityTypeId, 'ContentChannelId', CAST( @ChannelId AS NVARCHAR(10) ), 'Menu', 'Menu', 'Which menu this row lives in, i.e. sundays, explore, explore-about, etc.', 1000, 0, '', 0, 1, '99B3387C-7526-43CC-92AB-AC8E173E95F6' );
                END
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = '2F216418-643E-4B85-A213-1F3F6551F894' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '99B3387C-7526-43CC-92AB-AC8E173E95F6' ), 'fieldtype', 'ddl', '2F216418-643E-4B85-A213-1F3F6551F894';
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = '516D757D-9E22-4601-B537-007EFAD032A6' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '99B3387C-7526-43CC-92AB-AC8E173E95F6' ), 'repeatColumns', '', '516D757D-9E22-4601-B537-007EFAD032A6';
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = 'AB8A58C3-2B4C-458A-B6C2-D7C7DD7A925F' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '99B3387C-7526-43CC-92AB-AC8E173E95F6' ), 'values', 'sundays,explore,profile,explore-about,explore-podcasts', 'AB8A58C3-2B4C-458A-B6C2-D7C7DD7A925F';
                " );   // App Menu . Menu
            Sql( @"
                DECLARE @ChannelId INT = ( SELECT TOP 1 [Id] FROM [ContentChannel] WHERE [Guid] = 'DF6276E8-B00E-4E77-88D8-AB1241A4A8C7' );
                DECLARE @EntityTypeId INT = ( SELECT TOP 1 [Id] FROM [EntityType] WHERE [Name] = 'Rock.Model.ContentChannelItem' );
                DECLARE @FieldTypeId INT = ( SELECT TOP 1 [Id] FROM [FieldType] WHERE [Guid] = '7525C4CB-EE6B-41D4-9B64-A08048D5A5C0' );
                IF @ChannelId IS NOT NULL AND @FieldTypeId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [Attribute] WHERE [Guid] = 'A05333F7-EC3B-4BFD-98A9-40C33CB7BEB0' )
                BEGIN
                    INSERT INTO [Attribute] ( [IsSystem], [FieldTypeId], [EntityTypeId], [EntityTypeQualifierColumn], [EntityTypeQualifierValue], [Key], [Name], [Description], [Order], [IsGridColumn], [DefaultValue], [IsMultiValue], [IsRequired], [Guid] )
                    VALUES ( 0, @FieldTypeId, @EntityTypeId, 'ContentChannelId', CAST( @ChannelId AS NVARCHAR(10) ), 'DisplayStyle', 'Display Style', '', 1001, 0, 'Icon Row', 0, 0, 'A05333F7-EC3B-4BFD-98A9-40C33CB7BEB0' );
                END
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = '1A495D92-A7D3-420E-A983-474380F68534' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = 'A05333F7-EC3B-4BFD-98A9-40C33CB7BEB0' ), 'fieldtype', 'ddl', '1A495D92-A7D3-420E-A983-474380F68534';
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = '1931F0B7-83E8-4BD6-8988-57358F2DA5EC' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = 'A05333F7-EC3B-4BFD-98A9-40C33CB7BEB0' ), 'repeatColumns', '', '1931F0B7-83E8-4BD6-8988-57358F2DA5EC';
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = '9708D8A5-473B-4583-88BA-4CFCE1E03119' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = 'A05333F7-EC3B-4BFD-98A9-40C33CB7BEB0' ), 'values', 'Section Header, Icon Row, Plain Row, Thumbnail Row, Avatar Row, Meta Row, Hero Banner, Image Card', '9708D8A5-473B-4583-88BA-4CFCE1E03119';
                " );   // App Menu . DisplayStyle
            Sql( @"
                DECLARE @ChannelId INT = ( SELECT TOP 1 [Id] FROM [ContentChannel] WHERE [Guid] = 'DF6276E8-B00E-4E77-88D8-AB1241A4A8C7' );
                DECLARE @EntityTypeId INT = ( SELECT TOP 1 [Id] FROM [EntityType] WHERE [Name] = 'Rock.Model.ContentChannelItem' );
                DECLARE @FieldTypeId INT = ( SELECT TOP 1 [Id] FROM [FieldType] WHERE [Guid] = '7525C4CB-EE6B-41D4-9B64-A08048D5A5C0' );
                IF @ChannelId IS NOT NULL AND @FieldTypeId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [Attribute] WHERE [Guid] = '7FDC1A9A-1DCE-4B93-B2F5-EAA1F5663AF7' )
                BEGIN
                    INSERT INTO [Attribute] ( [IsSystem], [FieldTypeId], [EntityTypeId], [EntityTypeQualifierColumn], [EntityTypeQualifierValue], [Key], [Name], [Description], [Order], [IsGridColumn], [DefaultValue], [IsMultiValue], [IsRequired], [Guid] )
                    VALUES ( 0, @FieldTypeId, @EntityTypeId, 'ContentChannelId', CAST( @ChannelId AS NVARCHAR(10) ), 'OpensSubMenu', 'Opens Sub-Menu', 'Leave blank for a normal link, but this will link to a specific sub-menu of options such as explore-about', 1002, 0, '', 0, 0, '7FDC1A9A-1DCE-4B93-B2F5-EAA1F5663AF7' );
                END
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = '831CED23-5A75-4B4B-86C3-EE9B1F37C8A5' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '7FDC1A9A-1DCE-4B93-B2F5-EAA1F5663AF7' ), 'fieldtype', 'ddl', '831CED23-5A75-4B4B-86C3-EE9B1F37C8A5';
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = 'E8A862E4-90AE-4BE7-A595-43AE94D3974A' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '7FDC1A9A-1DCE-4B93-B2F5-EAA1F5663AF7' ), 'repeatColumns', '', 'E8A862E4-90AE-4BE7-A595-43AE94D3974A';
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = 'CD86751A-2400-4318-A78C-0E7343FB8AF9' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '7FDC1A9A-1DCE-4B93-B2F5-EAA1F5663AF7' ), 'values', 'sundays,explore,profile,explore-about,explore-podcasts', 'CD86751A-2400-4318-A78C-0E7343FB8AF9';
                " );   // App Menu . OpensSubMenu
            Sql( @"
                DECLARE @ChannelId INT = ( SELECT TOP 1 [Id] FROM [ContentChannel] WHERE [Guid] = 'DF6276E8-B00E-4E77-88D8-AB1241A4A8C7' );
                DECLARE @EntityTypeId INT = ( SELECT TOP 1 [Id] FROM [EntityType] WHERE [Name] = 'Rock.Model.ContentChannelItem' );
                DECLARE @FieldTypeId INT = ( SELECT TOP 1 [Id] FROM [FieldType] WHERE [Guid] = '9C204CD0-1233-41C5-818A-C5DA439445AA' );
                IF @ChannelId IS NOT NULL AND @FieldTypeId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [Attribute] WHERE [Guid] = '73690FC9-9D86-4468-BCF5-FF683B9FDDAD' )
                BEGIN
                    INSERT INTO [Attribute] ( [IsSystem], [FieldTypeId], [EntityTypeId], [EntityTypeQualifierColumn], [EntityTypeQualifierValue], [Key], [Name], [Description], [Order], [IsGridColumn], [DefaultValue], [IsMultiValue], [IsRequired], [Guid] )
                    VALUES ( 0, @FieldTypeId, @EntityTypeId, 'ContentChannelId', CAST( @ChannelId AS NVARCHAR(10) ), 'Icon', 'Icon', 'Font Awesome class, eg. calendar-check (Icon Row)', 1003, 0, '', 0, 0, '73690FC9-9D86-4468-BCF5-FF683B9FDDAD' );
                END
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = 'A51F1A0D-FD0F-4FE6-906A-64B177F8330B' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '73690FC9-9D86-4468-BCF5-FF683B9FDDAD' ), 'ispassword', 'False', 'A51F1A0D-FD0F-4FE6-906A-64B177F8330B';
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = 'AE4A4954-D44E-4AD5-9D0F-F9BCB7DBF125' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '73690FC9-9D86-4468-BCF5-FF683B9FDDAD' ), 'maxcharacters', '', 'AE4A4954-D44E-4AD5-9D0F-F9BCB7DBF125';
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = '26E7308A-3EB0-4556-98BD-30491D352C37' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '73690FC9-9D86-4468-BCF5-FF683B9FDDAD' ), 'showcountdown', 'False', '26E7308A-3EB0-4556-98BD-30491D352C37';
                " );   // App Menu . Icon
            Sql( @"
                DECLARE @ChannelId INT = ( SELECT TOP 1 [Id] FROM [ContentChannel] WHERE [Guid] = 'DF6276E8-B00E-4E77-88D8-AB1241A4A8C7' );
                DECLARE @EntityTypeId INT = ( SELECT TOP 1 [Id] FROM [EntityType] WHERE [Name] = 'Rock.Model.ContentChannelItem' );
                DECLARE @FieldTypeId INT = ( SELECT TOP 1 [Id] FROM [FieldType] WHERE [Guid] = '9C204CD0-1233-41C5-818A-C5DA439445AA' );
                IF @ChannelId IS NOT NULL AND @FieldTypeId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [Attribute] WHERE [Guid] = '8DB428E4-702D-403F-A0E2-D5A8537C1731' )
                BEGIN
                    INSERT INTO [Attribute] ( [IsSystem], [FieldTypeId], [EntityTypeId], [EntityTypeQualifierColumn], [EntityTypeQualifierValue], [Key], [Name], [Description], [Order], [IsGridColumn], [DefaultValue], [IsMultiValue], [IsRequired], [Guid] )
                    VALUES ( 0, @FieldTypeId, @EntityTypeId, 'ContentChannelId', CAST( @ChannelId AS NVARCHAR(10) ), 'Subtitle', 'Subtitle', 'For Avatar Row/Meta Row (e.g. a title or ""DATE - SPEAKER"")', 1004, 0, '', 0, 0, '8DB428E4-702D-403F-A0E2-D5A8537C1731' );
                END
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = 'F4397E3C-8BEE-4963-99B9-64D514FF6F37' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '8DB428E4-702D-403F-A0E2-D5A8537C1731' ), 'ispassword', 'False', 'F4397E3C-8BEE-4963-99B9-64D514FF6F37';
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = '5CFCED31-98BC-4DA9-A193-EB8F011101C9' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '8DB428E4-702D-403F-A0E2-D5A8537C1731' ), 'maxcharacters', '', '5CFCED31-98BC-4DA9-A193-EB8F011101C9';
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = 'D6BDA2E5-2D29-43DE-9743-0FFB85A46688' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '8DB428E4-702D-403F-A0E2-D5A8537C1731' ), 'showcountdown', 'False', 'D6BDA2E5-2D29-43DE-9743-0FFB85A46688';
                " );   // App Menu . Subtitle
            Sql( @"
                DECLARE @ChannelId INT = ( SELECT TOP 1 [Id] FROM [ContentChannel] WHERE [Guid] = 'DF6276E8-B00E-4E77-88D8-AB1241A4A8C7' );
                DECLARE @EntityTypeId INT = ( SELECT TOP 1 [Id] FROM [EntityType] WHERE [Name] = 'Rock.Model.ContentChannelItem' );
                DECLARE @FieldTypeId INT = ( SELECT TOP 1 [Id] FROM [FieldType] WHERE [Guid] = '97F8157D-A8C8-4AB3-96A2-9CB2A9049E6D' );
                IF @ChannelId IS NOT NULL AND @FieldTypeId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [Attribute] WHERE [Guid] = '8290BE1C-646C-4C79-995F-2A17E6F4EDE5' )
                BEGIN
                    INSERT INTO [Attribute] ( [IsSystem], [FieldTypeId], [EntityTypeId], [EntityTypeQualifierColumn], [EntityTypeQualifierValue], [Key], [Name], [Description], [Order], [IsGridColumn], [DefaultValue], [IsMultiValue], [IsRequired], [Guid] )
                    VALUES ( 0, @FieldTypeId, @EntityTypeId, 'ContentChannelId', CAST( @ChannelId AS NVARCHAR(10) ), 'Image', 'Image', 'Thumbnail / Avatar / Hero / Image Card', 1005, 0, '', 0, 0, '8290BE1C-646C-4C79-995F-2A17E6F4EDE5' );
                END
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = 'A5A64259-5537-44DD-B760-CA850B1750FE' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '8290BE1C-646C-4C79-995F-2A17E6F4EDE5' ), 'binaryFileType', 'c1142570-8cd6-4a20-83b1-acb47c1cd377', 'A5A64259-5537-44DD-B760-CA850B1750FE';
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = 'CB99C93B-F6B4-4031-BC57-33100264B2F2' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '8290BE1C-646C-4C79-995F-2A17E6F4EDE5' ), 'formatAsLink', 'False', 'CB99C93B-F6B4-4031-BC57-33100264B2F2';
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = '1E74706A-491A-47C1-8730-EC055020C3DB' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '8290BE1C-646C-4C79-995F-2A17E6F4EDE5' ), 'img_tag_template', '', '1E74706A-491A-47C1-8730-EC055020C3DB';
                " );   // App Menu . Image
            Sql( @"
                DECLARE @ChannelId INT = ( SELECT TOP 1 [Id] FROM [ContentChannel] WHERE [Guid] = 'DF6276E8-B00E-4E77-88D8-AB1241A4A8C7' );
                DECLARE @EntityTypeId INT = ( SELECT TOP 1 [Id] FROM [EntityType] WHERE [Name] = 'Rock.Model.ContentChannelItem' );
                DECLARE @FieldTypeId INT = ( SELECT TOP 1 [Id] FROM [FieldType] WHERE [Guid] = '9C204CD0-1233-41C5-818A-C5DA439445AA' );
                IF @ChannelId IS NOT NULL AND @FieldTypeId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [Attribute] WHERE [Guid] = '93240002-A37D-4578-80A1-D7E5BC8EF30A' )
                BEGIN
                    INSERT INTO [Attribute] ( [IsSystem], [FieldTypeId], [EntityTypeId], [EntityTypeQualifierColumn], [EntityTypeQualifierValue], [Key], [Name], [Description], [Order], [IsGridColumn], [DefaultValue], [IsMultiValue], [IsRequired], [Guid] )
                    VALUES ( 0, @FieldTypeId, @EntityTypeId, 'ContentChannelId', CAST( @ChannelId AS NVARCHAR(10) ), 'ImageUrl', 'Image URL', 'external image alternative (takes precedence)', 1006, 0, '', 0, 0, '93240002-A37D-4578-80A1-D7E5BC8EF30A' );
                END
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = 'BF5AED7A-F240-478A-A804-E50A7F246500' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '93240002-A37D-4578-80A1-D7E5BC8EF30A' ), 'ispassword', 'False', 'BF5AED7A-F240-478A-A804-E50A7F246500';
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = 'F9F5989E-1C29-406D-B5A7-9BCB61AF47B3' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '93240002-A37D-4578-80A1-D7E5BC8EF30A' ), 'maxcharacters', '', 'F9F5989E-1C29-406D-B5A7-9BCB61AF47B3';
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = '43E758E1-E0EF-4D73-8C2D-B17B23439339' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '93240002-A37D-4578-80A1-D7E5BC8EF30A' ), 'showcountdown', 'False', '43E758E1-E0EF-4D73-8C2D-B17B23439339';
                " );   // App Menu . ImageUrl
            Sql( @"
                DECLARE @ChannelId INT = ( SELECT TOP 1 [Id] FROM [ContentChannel] WHERE [Guid] = 'DF6276E8-B00E-4E77-88D8-AB1241A4A8C7' );
                DECLARE @EntityTypeId INT = ( SELECT TOP 1 [Id] FROM [EntityType] WHERE [Name] = 'Rock.Model.ContentChannelItem' );
                DECLARE @FieldTypeId INT = ( SELECT TOP 1 [Id] FROM [FieldType] WHERE [Guid] = '7525C4CB-EE6B-41D4-9B64-A08048D5A5C0' );
                IF @ChannelId IS NOT NULL AND @FieldTypeId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [Attribute] WHERE [Guid] = 'D1CA0756-964F-42C2-9839-ED121F76D986' )
                BEGIN
                    INSERT INTO [Attribute] ( [IsSystem], [FieldTypeId], [EntityTypeId], [EntityTypeQualifierColumn], [EntityTypeQualifierValue], [Key], [Name], [Description], [Order], [IsGridColumn], [DefaultValue], [IsMultiValue], [IsRequired], [Guid] )
                    VALUES ( 0, @FieldTypeId, @EntityTypeId, 'ContentChannelId', CAST( @ChannelId AS NVARCHAR(10) ), 'LinktoURLType', 'Link to URL Type', '', 1007, 0, '', 0, 0, 'D1CA0756-964F-42C2-9839-ED121F76D986' );
                END
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = '75C9FB70-12B0-421A-89C2-C5104FAD5FC8' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = 'D1CA0756-964F-42C2-9839-ED121F76D986' ), 'fieldtype', 'ddl', '75C9FB70-12B0-421A-89C2-C5104FAD5FC8';
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = '99A73403-8E36-4C2C-BDDA-BEE837F4157B' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = 'D1CA0756-964F-42C2-9839-ED121F76D986' ), 'repeatColumns', '', '99A73403-8E36-4C2C-BDDA-BEE837F4157B';
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = '9D02E3D9-D605-4125-9180-85F4B6D1CC84' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = 'D1CA0756-964F-42C2-9839-ED121F76D986' ), 'values', 'External Browser, Internal Browser, Webview', '9D02E3D9-D605-4125-9180-85F4B6D1CC84';
                " );   // App Menu . LinktoURLType
            Sql( @"
                DECLARE @ChannelId INT = ( SELECT TOP 1 [Id] FROM [ContentChannel] WHERE [Guid] = 'DF6276E8-B00E-4E77-88D8-AB1241A4A8C7' );
                DECLARE @EntityTypeId INT = ( SELECT TOP 1 [Id] FROM [EntityType] WHERE [Name] = 'Rock.Model.ContentChannelItem' );
                DECLARE @FieldTypeId INT = ( SELECT TOP 1 [Id] FROM [FieldType] WHERE [Guid] = '9C204CD0-1233-41C5-818A-C5DA439445AA' );
                IF @ChannelId IS NOT NULL AND @FieldTypeId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [Attribute] WHERE [Guid] = 'E2F9FC20-5C69-4733-8B14-F7817F49F021' )
                BEGIN
                    INSERT INTO [Attribute] ( [IsSystem], [FieldTypeId], [EntityTypeId], [EntityTypeQualifierColumn], [EntityTypeQualifierValue], [Key], [Name], [Description], [Order], [IsGridColumn], [DefaultValue], [IsMultiValue], [IsRequired], [Guid] )
                    VALUES ( 0, @FieldTypeId, @EntityTypeId, 'ContentChannelId', CAST( @ChannelId AS NVARCHAR(10) ), 'LinktoURL', 'Link to URL', '', 1008, 0, '', 0, 0, 'E2F9FC20-5C69-4733-8B14-F7817F49F021' );
                END
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = 'F15484C4-46A8-44FA-83A5-75EDE7F7B14B' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = 'E2F9FC20-5C69-4733-8B14-F7817F49F021' ), 'ispassword', 'False', 'F15484C4-46A8-44FA-83A5-75EDE7F7B14B';
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = '586BFD8F-D9E5-4435-92AE-5E765E4E96D1' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = 'E2F9FC20-5C69-4733-8B14-F7817F49F021' ), 'maxcharacters', '', '586BFD8F-D9E5-4435-92AE-5E765E4E96D1';
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = '8DA85634-8D7A-4460-8D34-D2B7EA6B6449' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = 'E2F9FC20-5C69-4733-8B14-F7817F49F021' ), 'showcountdown', 'False', '8DA85634-8D7A-4460-8D34-D2B7EA6B6449';
                " );   // App Menu . LinktoURL
            Sql( @"
                DECLARE @ChannelId INT = ( SELECT TOP 1 [Id] FROM [ContentChannel] WHERE [Guid] = 'DF6276E8-B00E-4E77-88D8-AB1241A4A8C7' );
                DECLARE @EntityTypeId INT = ( SELECT TOP 1 [Id] FROM [EntityType] WHERE [Name] = 'Rock.Model.ContentChannelItem' );
                DECLARE @FieldTypeId INT = ( SELECT TOP 1 [Id] FROM [FieldType] WHERE [Guid] = 'BD53F9C9-EBA9-4D3F-82EA-DE5DD34A8108' );
                IF @ChannelId IS NOT NULL AND @FieldTypeId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [Attribute] WHERE [Guid] = '92E457C6-3A42-43C4-BBC8-05790F5F5941' )
                BEGIN
                    INSERT INTO [Attribute] ( [IsSystem], [FieldTypeId], [EntityTypeId], [EntityTypeQualifierColumn], [EntityTypeQualifierValue], [Key], [Name], [Description], [Order], [IsGridColumn], [DefaultValue], [IsMultiValue], [IsRequired], [Guid] )
                    VALUES ( 0, @FieldTypeId, @EntityTypeId, 'ContentChannelId', CAST( @ChannelId AS NVARCHAR(10) ), 'LinktoAppPage', 'Link to App Page', '', 1009, 0, '', 0, 0, '92E457C6-3A42-43C4-BBC8-05790F5F5941' );
                END

                " );   // App Menu . LinktoAppPage
            Sql( @"
                DECLARE @ChannelId INT = ( SELECT TOP 1 [Id] FROM [ContentChannel] WHERE [Guid] = 'DF6276E8-B00E-4E77-88D8-AB1241A4A8C7' );
                DECLARE @EntityTypeId INT = ( SELECT TOP 1 [Id] FROM [EntityType] WHERE [Name] = 'Rock.Model.ContentChannelItem' );
                DECLARE @FieldTypeId INT = ( SELECT TOP 1 [Id] FROM [FieldType] WHERE [Guid] = '9C204CD0-1233-41C5-818A-C5DA439445AA' );
                IF @ChannelId IS NOT NULL AND @FieldTypeId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [Attribute] WHERE [Guid] = 'E77D3263-DC15-4E09-8C37-A18973FAEFBA' )
                BEGIN
                    INSERT INTO [Attribute] ( [IsSystem], [FieldTypeId], [EntityTypeId], [EntityTypeQualifierColumn], [EntityTypeQualifierValue], [Key], [Name], [Description], [Order], [IsGridColumn], [DefaultValue], [IsMultiValue], [IsRequired], [Guid] )
                    VALUES ( 0, @FieldTypeId, @EntityTypeId, 'ContentChannelId', CAST( @ChannelId AS NVARCHAR(10) ), 'PageParameters', 'Page Parameters', 'Page parameters to append to a link to app page, must include the ""?"" character to start.', 1010, 0, '', 0, 0, 'E77D3263-DC15-4E09-8C37-A18973FAEFBA' );
                END
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = 'A846C332-62E8-4449-AFD1-3BCB9B3CFB0E' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = 'E77D3263-DC15-4E09-8C37-A18973FAEFBA' ), 'ispassword', 'False', 'A846C332-62E8-4449-AFD1-3BCB9B3CFB0E';
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = 'E3549AE4-CFB5-4178-BBBD-F183F65ADC53' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = 'E77D3263-DC15-4E09-8C37-A18973FAEFBA' ), 'maxcharacters', '', 'E3549AE4-CFB5-4178-BBBD-F183F65ADC53';
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = 'BDEABBA9-2332-4129-BF4E-2FA3BCA39D45' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = 'E77D3263-DC15-4E09-8C37-A18973FAEFBA' ), 'showcountdown', 'False', 'BDEABBA9-2332-4129-BF4E-2FA3BCA39D45';
                " );   // App Menu . PageParameters
            Sql( @"
                DECLARE @ChannelId INT = ( SELECT TOP 1 [Id] FROM [ContentChannel] WHERE [Guid] = 'DF6276E8-B00E-4E77-88D8-AB1241A4A8C7' );
                DECLARE @EntityTypeId INT = ( SELECT TOP 1 [Id] FROM [EntityType] WHERE [Name] = 'Rock.Model.ContentChannelItem' );
                DECLARE @FieldTypeId INT = ( SELECT TOP 1 [Id] FROM [FieldType] WHERE [Guid] = 'C0D0D7E2-C3B0-4004-ABEA-4BBFAD10D5D2' );
                IF @ChannelId IS NOT NULL AND @FieldTypeId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [Attribute] WHERE [Guid] = '7A4E1C09-3D62-4B85-9F03-2E8D5A61C74B' )
                BEGIN
                    INSERT INTO [Attribute] ( [IsSystem], [FieldTypeId], [EntityTypeId], [EntityTypeQualifierColumn], [EntityTypeQualifierValue], [Key], [Name], [Description], [Order], [IsGridColumn], [DefaultValue], [IsMultiValue], [IsRequired], [Guid] )
                    VALUES ( 0, @FieldTypeId, @EntityTypeId, 'ContentChannelId', CAST( @ChannelId AS NVARCHAR(10) ), 'LinkUrl', 'Button Link Url', 'Adds a call-to-action button to the item detail page. Separate from ""Link to URL"", which controls where the menu row itself navigates.', 1011, 0, '', 0, 0, '7A4E1C09-3D62-4B85-9F03-2E8D5A61C74B' );
                END
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = '3B7DE6C3-2E07-431D-B87E-94802CED4123' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '7A4E1C09-3D62-4B85-9F03-2E8D5A61C74B' ), 'ShouldAlwaysShowCondensed', 'False', '3B7DE6C3-2E07-431D-B87E-94802CED4123';
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = '143EEF8D-6768-4807-939C-2D4C677F134B' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '7A4E1C09-3D62-4B85-9F03-2E8D5A61C74B' ), 'ShouldRequireTrailingForwardSlash', 'False', '143EEF8D-6768-4807-939C-2D4C677F134B';
                " );   // App Menu . LinkUrl
            Sql( @"
                DECLARE @ChannelId INT = ( SELECT TOP 1 [Id] FROM [ContentChannel] WHERE [Guid] = 'DF6276E8-B00E-4E77-88D8-AB1241A4A8C7' );
                DECLARE @EntityTypeId INT = ( SELECT TOP 1 [Id] FROM [EntityType] WHERE [Name] = 'Rock.Model.ContentChannelItem' );
                DECLARE @FieldTypeId INT = ( SELECT TOP 1 [Id] FROM [FieldType] WHERE [Guid] = '9C204CD0-1233-41C5-818A-C5DA439445AA' );
                IF @ChannelId IS NOT NULL AND @FieldTypeId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [Attribute] WHERE [Guid] = 'B85D3F27-6A14-4E90-8C5B-1D07F2A639E8' )
                BEGIN
                    INSERT INTO [Attribute] ( [IsSystem], [FieldTypeId], [EntityTypeId], [EntityTypeQualifierColumn], [EntityTypeQualifierValue], [Key], [Name], [Description], [Order], [IsGridColumn], [DefaultValue], [IsMultiValue], [IsRequired], [Guid] )
                    VALUES ( 0, @FieldTypeId, @EntityTypeId, 'ContentChannelId', CAST( @ChannelId AS NVARCHAR(10) ), 'LinkButtonText', 'Button Link Text', 'Label for the link button. Defaults to ""Learn More"".', 1012, 0, '', 0, 0, 'B85D3F27-6A14-4E90-8C5B-1D07F2A639E8' );
                END
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = 'C69BEAB5-DBFF-43F3-9B4B-E8B6B38E12EB' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = 'B85D3F27-6A14-4E90-8C5B-1D07F2A639E8' ), 'ispassword', 'False', 'C69BEAB5-DBFF-43F3-9B4B-E8B6B38E12EB';
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = 'D150A4FE-05CE-4DA1-BB93-5AC8071BC3E5' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = 'B85D3F27-6A14-4E90-8C5B-1D07F2A639E8' ), 'maxcharacters', '', 'D150A4FE-05CE-4DA1-BB93-5AC8071BC3E5';
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = 'A8429985-C052-4ACC-93D3-97D955220F45' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = 'B85D3F27-6A14-4E90-8C5B-1D07F2A639E8' ), 'showcountdown', 'False', 'A8429985-C052-4ACC-93D3-97D955220F45';
                " );   // App Menu . LinkButtonText
            Sql( @"
                DECLARE @ChannelId INT = ( SELECT TOP 1 [Id] FROM [ContentChannel] WHERE [Guid] = 'DF6276E8-B00E-4E77-88D8-AB1241A4A8C7' );
                DECLARE @EntityTypeId INT = ( SELECT TOP 1 [Id] FROM [EntityType] WHERE [Name] = 'Rock.Model.ContentChannelItem' );
                DECLARE @FieldTypeId INT = ( SELECT TOP 1 [Id] FROM [FieldType] WHERE [Guid] = '1EDAFDED-DFE6-4334-B019-6EECBA89E05A' );
                IF @ChannelId IS NOT NULL AND @FieldTypeId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [Attribute] WHERE [Guid] = 'C93A6E58-2F71-4D06-B14E-8A5C0D3B72F1' )
                BEGIN
                    INSERT INTO [Attribute] ( [IsSystem], [FieldTypeId], [EntityTypeId], [EntityTypeQualifierColumn], [EntityTypeQualifierValue], [Key], [Name], [Description], [Order], [IsGridColumn], [DefaultValue], [IsMultiValue], [IsRequired], [Guid] )
                    VALUES ( 0, @FieldTypeId, @EntityTypeId, 'ContentChannelId', CAST( @ChannelId AS NVARCHAR(10) ), 'LinkOpensExternally', 'Button Link Opens Externally', 'On: hand off to the device browser. Off (default): open inside the app.', 1013, 0, 'False', 0, 0, 'C93A6E58-2F71-4D06-B14E-8A5C0D3B72F1' );
                END
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = '88147B5F-B2B9-4E13-9994-228E14712F9E' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = 'C93A6E58-2F71-4D06-B14E-8A5C0D3B72F1' ), 'BooleanControlType', '0', '88147B5F-B2B9-4E13-9994-228E14712F9E';
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = 'A8E5F784-0381-4B5C-816D-BF84D47B2E95' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = 'C93A6E58-2F71-4D06-B14E-8A5C0D3B72F1' ), 'falsetext', '', 'A8E5F784-0381-4B5C-816D-BF84D47B2E95';
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = '996CD565-7033-4859-B8E3-9FBF3AA7705A' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = 'C93A6E58-2F71-4D06-B14E-8A5C0D3B72F1' ), 'truetext', '', '996CD565-7033-4859-B8E3-9FBF3AA7705A';
                " );   // App Menu . LinkOpensExternally
            Sql( @"
                DECLARE @ChannelId INT = ( SELECT TOP 1 [Id] FROM [ContentChannel] WHERE [Guid] = 'DF6276E8-B00E-4E77-88D8-AB1241A4A8C7' );
                DECLARE @EntityTypeId INT = ( SELECT TOP 1 [Id] FROM [EntityType] WHERE [Name] = 'Rock.Model.ContentChannelItem' );
                DECLARE @FieldTypeId INT = ( SELECT TOP 1 [Id] FROM [FieldType] WHERE [Guid] = 'BD53F9C9-EBA9-4D3F-82EA-DE5DD34A8108' );
                IF @ChannelId IS NOT NULL AND @FieldTypeId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [Attribute] WHERE [Guid] = '9F3C6A18-2D74-4E85-B0A9-5C61F7D82E30' )
                BEGIN
                    INSERT INTO [Attribute] ( [IsSystem], [FieldTypeId], [EntityTypeId], [EntityTypeQualifierColumn], [EntityTypeQualifierValue], [Key], [Name], [Description], [Order], [IsGridColumn], [DefaultValue], [IsMultiValue], [IsRequired], [Guid] )
                    VALUES ( 0, @FieldTypeId, @EntityTypeId, 'ContentChannelId', CAST( @ChannelId AS NVARCHAR(10) ), 'LinkAppPage', 'Button Link App Page', 'Opens a page inside the app when the call-to-action button is tapped. Takes precedence over Button Link Url if both are set. Point it at a page containing a WebView or Workflow Entry block to open a web page or start a workflow.', 1014, 0, '', 0, 0, '9F3C6A18-2D74-4E85-B0A9-5C61F7D82E30' );
                END

                " );   // App Menu . LinkAppPage
            Sql( @"
                DECLARE @ChannelId INT = ( SELECT TOP 1 [Id] FROM [ContentChannel] WHERE [Guid] = 'DF6276E8-B00E-4E77-88D8-AB1241A4A8C7' );
                DECLARE @EntityTypeId INT = ( SELECT TOP 1 [Id] FROM [EntityType] WHERE [Name] = 'Rock.Model.ContentChannelItem' );
                DECLARE @FieldTypeId INT = ( SELECT TOP 1 [Id] FROM [FieldType] WHERE [Guid] = '9C204CD0-1233-41C5-818A-C5DA439445AA' );
                IF @ChannelId IS NOT NULL AND @FieldTypeId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [Attribute] WHERE [Guid] = '1A8D5E62-7B04-4C39-96F1-E2A5C0B73D48' )
                BEGIN
                    INSERT INTO [Attribute] ( [IsSystem], [FieldTypeId], [EntityTypeId], [EntityTypeQualifierColumn], [EntityTypeQualifierValue], [Key], [Name], [Description], [Order], [IsGridColumn], [DefaultValue], [IsMultiValue], [IsRequired], [Guid] )
                    VALUES ( 0, @FieldTypeId, @EntityTypeId, 'ContentChannelId', CAST( @ChannelId AS NVARCHAR(10) ), 'LinkPageParameters', 'Button Link Page Parameters', 'Query string passed to Button Link App Page, e.g. ""?WorkflowTypeGuid=abc"" or ""WorkflowTypeGuid=abc"". The leading ? is optional. Ignored unless Button Link App Page is set.', 1015, 0, '', 0, 0, '1A8D5E62-7B04-4C39-96F1-E2A5C0B73D48' );
                END

                " );   // App Menu . LinkPageParameters
            Sql( @"
                DECLARE @ChannelId INT = ( SELECT TOP 1 [Id] FROM [ContentChannel] WHERE [Guid] = 'DF6276E8-B00E-4E77-88D8-AB1241A4A8C7' );
                DECLARE @EntityTypeId INT = ( SELECT TOP 1 [Id] FROM [EntityType] WHERE [Name] = 'Rock.Model.ContentChannelItem' );
                DECLARE @FieldTypeId INT = ( SELECT TOP 1 [Id] FROM [FieldType] WHERE [Guid] = '7525C4CB-EE6B-41D4-9B64-A08048D5A5C0' );
                IF @ChannelId IS NOT NULL AND @FieldTypeId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [Attribute] WHERE [Guid] = '7E1A9D46-B3C5-4028-95F7-6D0B84E23A19' )
                BEGIN
                    INSERT INTO [Attribute] ( [IsSystem], [FieldTypeId], [EntityTypeId], [EntityTypeQualifierColumn], [EntityTypeQualifierValue], [Key], [Name], [Description], [Order], [IsGridColumn], [DefaultValue], [IsMultiValue], [IsRequired], [Guid] )
                    VALUES ( 0, @FieldTypeId, @EntityTypeId, 'ContentChannelId', CAST( @ChannelId AS NVARCHAR(10) ), 'ShareMode', 'Share Button', 'Shows a share icon in the header. ""Share this item"" needs a public web page for the item to exist; ""Share the button link"" hands out Button Link Url and renders nothing if the item uses an app page instead.', 1016, 0, 'None', 0, 0, '7E1A9D46-B3C5-4028-95F7-6D0B84E23A19' );
                END
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = '4A07932D-E5B1-46B5-C89C-03FA6B9BE7D8' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '7E1A9D46-B3C5-4028-95F7-6D0B84E23A19' ), 'fieldtype', 'ddl', '4A07932D-E5B1-46B5-C89C-03FA6B9BE7D8';
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = '6C29B54F-07D3-48D7-EABE-25FC8DBD09FA' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '7E1A9D46-B3C5-4028-95F7-6D0B84E23A19' ), 'repeatColumns', '', '6C29B54F-07D3-48D7-EABE-25FC8DBD09FA';
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = '5B18A43E-F6C2-47C6-D9AD-14EB7CACF8E9' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '7E1A9D46-B3C5-4028-95F7-6D0B84E23A19' ), 'values', 'None^No share,Item^Share this item,Link^Share the button link', '5B18A43E-F6C2-47C6-D9AD-14EB7CACF8E9';
                " );   // App Menu . ShareMode
            Sql( @"
                DECLARE @ChannelId INT = ( SELECT TOP 1 [Id] FROM [ContentChannel] WHERE [Guid] = 'DF6276E8-B00E-4E77-88D8-AB1241A4A8C7' );
                DECLARE @EntityTypeId INT = ( SELECT TOP 1 [Id] FROM [EntityType] WHERE [Name] = 'Rock.Model.ContentChannelItem' );
                DECLARE @FieldTypeId INT = ( SELECT TOP 1 [Id] FROM [FieldType] WHERE [Guid] = '1EDAFDED-DFE6-4334-B019-6EECBA89E05A' );
                IF @ChannelId IS NOT NULL AND @FieldTypeId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [Attribute] WHERE [Guid] = '5C24F8B0-6E13-49A7-8D3F-0B76E1A5C92D' )
                BEGIN
                    INSERT INTO [Attribute] ( [IsSystem], [FieldTypeId], [EntityTypeId], [EntityTypeQualifierColumn], [EntityTypeQualifierValue], [Key], [Name], [Description], [Order], [IsGridColumn], [DefaultValue], [IsMultiValue], [IsRequired], [Guid] )
                    VALUES ( 0, @FieldTypeId, @EntityTypeId, 'ContentChannelId', CAST( @ChannelId AS NVARCHAR(10) ), 'ShowDetailTitle', 'Show Title (Detail Page)', 'Off hides the large title in the body of the detail page. The header/nav bar always keeps the title. Separate from ""Show Title"", which controls the card.', 1017, 0, 'True', 0, 0, '5C24F8B0-6E13-49A7-8D3F-0B76E1A5C92D' );
                END
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = '7D3AC650-18E4-49E8-FBCF-36AD9ECE1A0B' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '5C24F8B0-6E13-49A7-8D3F-0B76E1A5C92D' ), 'BooleanControlType', '0', '7D3AC650-18E4-49E8-FBCF-36AD9ECE1A0B';
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = '9F5CE872-3A06-4B0A-1DE1-58CFB0E03C2D' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '5C24F8B0-6E13-49A7-8D3F-0B76E1A5C92D' ), 'falsetext', '', '9F5CE872-3A06-4B0A-1DE1-58CFB0E03C2D';
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = '8E4BD761-29F5-4AF9-0CD0-47BEAFDF2B1C' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '5C24F8B0-6E13-49A7-8D3F-0B76E1A5C92D' ), 'truetext', '', '8E4BD761-29F5-4AF9-0CD0-47BEAFDF2B1C';
                " );   // App Menu . ShowDetailTitle
            Sql( @"
                DECLARE @ChannelId INT = ( SELECT TOP 1 [Id] FROM [ContentChannel] WHERE [Guid] = 'DF6276E8-B00E-4E77-88D8-AB1241A4A8C7' );
                DECLARE @EntityTypeId INT = ( SELECT TOP 1 [Id] FROM [EntityType] WHERE [Name] = 'Rock.Model.ContentChannelItem' );
                DECLARE @FieldTypeId INT = ( SELECT TOP 1 [Id] FROM [FieldType] WHERE [Guid] = '1EDAFDED-DFE6-4334-B019-6EECBA89E05A' );
                IF @ChannelId IS NOT NULL AND @FieldTypeId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [Attribute] WHERE [Guid] = '3D95B172-8C40-4F6E-A218-97E4D0B36F51' )
                BEGIN
                    INSERT INTO [Attribute] ( [IsSystem], [FieldTypeId], [EntityTypeId], [EntityTypeQualifierColumn], [EntityTypeQualifierValue], [Key], [Name], [Description], [Order], [IsGridColumn], [DefaultValue], [IsMultiValue], [IsRequired], [Guid] )
                    VALUES ( 0, @FieldTypeId, @EntityTypeId, 'ContentChannelId', CAST( @ChannelId AS NVARCHAR(10) ), 'ShowDetailDate', 'Show Date (Detail Page)', 'Off hides the date on the detail page. The speaker still shows if set.', 1018, 0, 'True', 0, 0, '3D95B172-8C40-4F6E-A218-97E4D0B36F51' );
                END
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = 'A06DF983-4B17-4C1B-2EF2-69D0C1F14D3E' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '3D95B172-8C40-4F6E-A218-97E4D0B36F51' ), 'BooleanControlType', '0', 'A06DF983-4B17-4C1B-2EF2-69D0C1F14D3E';
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = 'C28F1BA5-6D39-4E3D-4014-8BF2E3136F50' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '3D95B172-8C40-4F6E-A218-97E4D0B36F51' ), 'falsetext', '', 'C28F1BA5-6D39-4E3D-4014-8BF2E3136F50';
                IF NOT EXISTS ( SELECT 1 FROM [AttributeQualifier] WHERE [Guid] = 'B17E0A94-5C28-4D2C-3F03-7AE1D2025E4F' )
                    INSERT INTO [AttributeQualifier] ( [IsSystem], [AttributeId], [Key], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '3D95B172-8C40-4F6E-A218-97E4D0B36F51' ), 'truetext', '', 'B17E0A94-5C28-4D2C-3F03-7AE1D2025E4F';
                " );   // App Menu . ShowDetailDate

﻿
            //
            // App Menu items that link to app pages.
            // Church-specific menu content is intentionally NOT migrated.
            //
            Sql( @"
                DECLARE @ChannelId INT = ( SELECT TOP 1 [Id] FROM [ContentChannel] WHERE [Guid] = 'DF6276E8-B00E-4E77-88D8-AB1241A4A8C7' );
                DECLARE @ChannelTypeId INT = ( SELECT TOP 1 [ContentChannelTypeId] FROM [ContentChannel] WHERE [Id] = @ChannelId );
                IF @ChannelId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [ContentChannelItem] WHERE [Guid] = '58993C0C-3453-4A11-94FE-FA890242ADFB' )
                BEGIN
                    INSERT INTO [ContentChannelItem] ( [ContentChannelId], [ContentChannelTypeId], [Title], [Content], [Priority], [Status], [StartDateTime], [Order], [Guid] )
                    VALUES ( @ChannelId, @ChannelTypeId, 'Check-in', '', 0, 1, GETDATE(), 1, '58993C0C-3453-4A11-94FE-FA890242ADFB' );
                END
                DECLARE @ItemId INT = ( SELECT TOP 1 [Id] FROM [ContentChannelItem] WHERE [Guid] = '58993C0C-3453-4A11-94FE-FA890242ADFB' );
                IF @ItemId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [AttributeValue] av2 JOIN [Attribute] a2 ON a2.[Id] = av2.[AttributeId] AND a2.[Guid] = '99B3387C-7526-43CC-92AB-AC8E173E95F6' WHERE av2.[EntityId] = @ItemId )
                    INSERT INTO [AttributeValue] ( [IsSystem], [AttributeId], [EntityId], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '99B3387C-7526-43CC-92AB-AC8E173E95F6' ), @ItemId, 'sundays', NEWID();
                IF @ItemId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [AttributeValue] av2 JOIN [Attribute] a2 ON a2.[Id] = av2.[AttributeId] AND a2.[Guid] = 'A05333F7-EC3B-4BFD-98A9-40C33CB7BEB0' WHERE av2.[EntityId] = @ItemId )
                    INSERT INTO [AttributeValue] ( [IsSystem], [AttributeId], [EntityId], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = 'A05333F7-EC3B-4BFD-98A9-40C33CB7BEB0' ), @ItemId, 'Icon Row', NEWID();
                IF @ItemId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [AttributeValue] av2 JOIN [Attribute] a2 ON a2.[Id] = av2.[AttributeId] AND a2.[Guid] = '73690FC9-9D86-4468-BCF5-FF683B9FDDAD' WHERE av2.[EntityId] = @ItemId )
                    INSERT INTO [AttributeValue] ( [IsSystem], [AttributeId], [EntityId], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '73690FC9-9D86-4468-BCF5-FF683B9FDDAD' ), @ItemId, 'user-check', NEWID();
                IF @ItemId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [AttributeValue] av2 JOIN [Attribute] a2 ON a2.[Id] = av2.[AttributeId] AND a2.[Guid] = '92E457C6-3A42-43C4-BBC8-05790F5F5941' WHERE av2.[EntityId] = @ItemId )
                    INSERT INTO [AttributeValue] ( [IsSystem], [AttributeId], [EntityId], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '92E457C6-3A42-43C4-BBC8-05790F5F5941' ), @ItemId, 'b29e8335-21c0-4459-a1ad-d85537fc2c08', NEWID();
                " );   // menu item: Check-in
            Sql( @"
                DECLARE @ChannelId INT = ( SELECT TOP 1 [Id] FROM [ContentChannel] WHERE [Guid] = 'DF6276E8-B00E-4E77-88D8-AB1241A4A8C7' );
                DECLARE @ChannelTypeId INT = ( SELECT TOP 1 [ContentChannelTypeId] FROM [ContentChannel] WHERE [Id] = @ChannelId );
                IF @ChannelId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [ContentChannelItem] WHERE [Guid] = 'F69DF9D0-C242-4BF6-A737-78C333463CAC' )
                BEGIN
                    INSERT INTO [ContentChannelItem] ( [ContentChannelId], [ContentChannelTypeId], [Title], [Content], [Priority], [Status], [StartDateTime], [Order], [Guid] )
                    VALUES ( @ChannelId, @ChannelTypeId, 'Sermon Notes', '', 0, 1, GETDATE(), 2, 'F69DF9D0-C242-4BF6-A737-78C333463CAC' );
                END
                DECLARE @ItemId INT = ( SELECT TOP 1 [Id] FROM [ContentChannelItem] WHERE [Guid] = 'F69DF9D0-C242-4BF6-A737-78C333463CAC' );
                IF @ItemId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [AttributeValue] av2 JOIN [Attribute] a2 ON a2.[Id] = av2.[AttributeId] AND a2.[Guid] = '99B3387C-7526-43CC-92AB-AC8E173E95F6' WHERE av2.[EntityId] = @ItemId )
                    INSERT INTO [AttributeValue] ( [IsSystem], [AttributeId], [EntityId], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '99B3387C-7526-43CC-92AB-AC8E173E95F6' ), @ItemId, 'sundays', NEWID();
                IF @ItemId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [AttributeValue] av2 JOIN [Attribute] a2 ON a2.[Id] = av2.[AttributeId] AND a2.[Guid] = 'A05333F7-EC3B-4BFD-98A9-40C33CB7BEB0' WHERE av2.[EntityId] = @ItemId )
                    INSERT INTO [AttributeValue] ( [IsSystem], [AttributeId], [EntityId], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = 'A05333F7-EC3B-4BFD-98A9-40C33CB7BEB0' ), @ItemId, 'Icon Row', NEWID();
                IF @ItemId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [AttributeValue] av2 JOIN [Attribute] a2 ON a2.[Id] = av2.[AttributeId] AND a2.[Guid] = '73690FC9-9D86-4468-BCF5-FF683B9FDDAD' WHERE av2.[EntityId] = @ItemId )
                    INSERT INTO [AttributeValue] ( [IsSystem], [AttributeId], [EntityId], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '73690FC9-9D86-4468-BCF5-FF683B9FDDAD' ), @ItemId, 'edit', NEWID();
                IF @ItemId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [AttributeValue] av2 JOIN [Attribute] a2 ON a2.[Id] = av2.[AttributeId] AND a2.[Guid] = '92E457C6-3A42-43C4-BBC8-05790F5F5941' WHERE av2.[EntityId] = @ItemId )
                    INSERT INTO [AttributeValue] ( [IsSystem], [AttributeId], [EntityId], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '92E457C6-3A42-43C4-BBC8-05790F5F5941' ), @ItemId, '289d32ad-ad5f-431f-bcbe-7ebee71d0f19', NEWID();
                " );   // menu item: Sermon Notes
            Sql( @"
                DECLARE @ChannelId INT = ( SELECT TOP 1 [Id] FROM [ContentChannel] WHERE [Guid] = 'DF6276E8-B00E-4E77-88D8-AB1241A4A8C7' );
                DECLARE @ChannelTypeId INT = ( SELECT TOP 1 [ContentChannelTypeId] FROM [ContentChannel] WHERE [Id] = @ChannelId );
                IF @ChannelId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [ContentChannelItem] WHERE [Guid] = 'AF932289-CA24-4C18-A94D-7587927B61B9' )
                BEGIN
                    INSERT INTO [ContentChannelItem] ( [ContentChannelId], [ContentChannelTypeId], [Title], [Content], [Priority], [Status], [StartDateTime], [Order], [Guid] )
                    VALUES ( @ChannelId, @ChannelTypeId, 'Live Stream', '', 0, 1, GETDATE(), 3, 'AF932289-CA24-4C18-A94D-7587927B61B9' );
                END
                DECLARE @ItemId INT = ( SELECT TOP 1 [Id] FROM [ContentChannelItem] WHERE [Guid] = 'AF932289-CA24-4C18-A94D-7587927B61B9' );
                IF @ItemId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [AttributeValue] av2 JOIN [Attribute] a2 ON a2.[Id] = av2.[AttributeId] AND a2.[Guid] = '99B3387C-7526-43CC-92AB-AC8E173E95F6' WHERE av2.[EntityId] = @ItemId )
                    INSERT INTO [AttributeValue] ( [IsSystem], [AttributeId], [EntityId], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '99B3387C-7526-43CC-92AB-AC8E173E95F6' ), @ItemId, 'sundays', NEWID();
                IF @ItemId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [AttributeValue] av2 JOIN [Attribute] a2 ON a2.[Id] = av2.[AttributeId] AND a2.[Guid] = 'A05333F7-EC3B-4BFD-98A9-40C33CB7BEB0' WHERE av2.[EntityId] = @ItemId )
                    INSERT INTO [AttributeValue] ( [IsSystem], [AttributeId], [EntityId], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = 'A05333F7-EC3B-4BFD-98A9-40C33CB7BEB0' ), @ItemId, 'Icon Row', NEWID();
                IF @ItemId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [AttributeValue] av2 JOIN [Attribute] a2 ON a2.[Id] = av2.[AttributeId] AND a2.[Guid] = '73690FC9-9D86-4468-BCF5-FF683B9FDDAD' WHERE av2.[EntityId] = @ItemId )
                    INSERT INTO [AttributeValue] ( [IsSystem], [AttributeId], [EntityId], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '73690FC9-9D86-4468-BCF5-FF683B9FDDAD' ), @ItemId, 'tv', NEWID();
                IF @ItemId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [AttributeValue] av2 JOIN [Attribute] a2 ON a2.[Id] = av2.[AttributeId] AND a2.[Guid] = '92E457C6-3A42-43C4-BBC8-05790F5F5941' WHERE av2.[EntityId] = @ItemId )
                    INSERT INTO [AttributeValue] ( [IsSystem], [AttributeId], [EntityId], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '92E457C6-3A42-43C4-BBC8-05790F5F5941' ), @ItemId, '223193f9-9833-4a19-ba36-2b49d312d02a', NEWID();
                " );   // menu item: Live Stream
            Sql( @"
                DECLARE @ChannelId INT = ( SELECT TOP 1 [Id] FROM [ContentChannel] WHERE [Guid] = 'DF6276E8-B00E-4E77-88D8-AB1241A4A8C7' );
                DECLARE @ChannelTypeId INT = ( SELECT TOP 1 [ContentChannelTypeId] FROM [ContentChannel] WHERE [Id] = @ChannelId );
                IF @ChannelId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [ContentChannelItem] WHERE [Guid] = '912C03E5-DF07-4938-A5D9-33BE097E81BB' )
                BEGIN
                    INSERT INTO [ContentChannelItem] ( [ContentChannelId], [ContentChannelTypeId], [Title], [Content], [Priority], [Status], [StartDateTime], [Order], [Guid] )
                    VALUES ( @ChannelId, @ChannelTypeId, 'Post-Service Survey', '', 0, 1, GETDATE(), 4, '912C03E5-DF07-4938-A5D9-33BE097E81BB' );
                END
                DECLARE @ItemId INT = ( SELECT TOP 1 [Id] FROM [ContentChannelItem] WHERE [Guid] = '912C03E5-DF07-4938-A5D9-33BE097E81BB' );
                IF @ItemId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [AttributeValue] av2 JOIN [Attribute] a2 ON a2.[Id] = av2.[AttributeId] AND a2.[Guid] = '99B3387C-7526-43CC-92AB-AC8E173E95F6' WHERE av2.[EntityId] = @ItemId )
                    INSERT INTO [AttributeValue] ( [IsSystem], [AttributeId], [EntityId], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '99B3387C-7526-43CC-92AB-AC8E173E95F6' ), @ItemId, 'sundays', NEWID();
                IF @ItemId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [AttributeValue] av2 JOIN [Attribute] a2 ON a2.[Id] = av2.[AttributeId] AND a2.[Guid] = 'A05333F7-EC3B-4BFD-98A9-40C33CB7BEB0' WHERE av2.[EntityId] = @ItemId )
                    INSERT INTO [AttributeValue] ( [IsSystem], [AttributeId], [EntityId], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = 'A05333F7-EC3B-4BFD-98A9-40C33CB7BEB0' ), @ItemId, 'Icon Row', NEWID();
                IF @ItemId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [AttributeValue] av2 JOIN [Attribute] a2 ON a2.[Id] = av2.[AttributeId] AND a2.[Guid] = '73690FC9-9D86-4468-BCF5-FF683B9FDDAD' WHERE av2.[EntityId] = @ItemId )
                    INSERT INTO [AttributeValue] ( [IsSystem], [AttributeId], [EntityId], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '73690FC9-9D86-4468-BCF5-FF683B9FDDAD' ), @ItemId, 'comment-dots', NEWID();
                IF @ItemId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [AttributeValue] av2 JOIN [Attribute] a2 ON a2.[Id] = av2.[AttributeId] AND a2.[Guid] = '92E457C6-3A42-43C4-BBC8-05790F5F5941' WHERE av2.[EntityId] = @ItemId )
                    INSERT INTO [AttributeValue] ( [IsSystem], [AttributeId], [EntityId], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '92E457C6-3A42-43C4-BBC8-05790F5F5941' ), @ItemId, '1519a17f-8cb5-487e-87dd-30fd2e5cf0da', NEWID();
                " );   // menu item: Post-Service Survey
            Sql( @"
                DECLARE @ChannelId INT = ( SELECT TOP 1 [Id] FROM [ContentChannel] WHERE [Guid] = 'DF6276E8-B00E-4E77-88D8-AB1241A4A8C7' );
                DECLARE @ChannelTypeId INT = ( SELECT TOP 1 [ContentChannelTypeId] FROM [ContentChannel] WHERE [Id] = @ChannelId );
                IF @ChannelId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [ContentChannelItem] WHERE [Guid] = '9FE1FE48-3244-4ED7-BB24-32F8FBD57331' )
                BEGIN
                    INSERT INTO [ContentChannelItem] ( [ContentChannelId], [ContentChannelTypeId], [Title], [Content], [Priority], [Status], [StartDateTime], [Order], [Guid] )
                    VALUES ( @ChannelId, @ChannelTypeId, 'Events, Groups & Classes', '', 0, 1, GETDATE(), 8, '9FE1FE48-3244-4ED7-BB24-32F8FBD57331' );
                END
                DECLARE @ItemId INT = ( SELECT TOP 1 [Id] FROM [ContentChannelItem] WHERE [Guid] = '9FE1FE48-3244-4ED7-BB24-32F8FBD57331' );
                IF @ItemId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [AttributeValue] av2 JOIN [Attribute] a2 ON a2.[Id] = av2.[AttributeId] AND a2.[Guid] = '99B3387C-7526-43CC-92AB-AC8E173E95F6' WHERE av2.[EntityId] = @ItemId )
                    INSERT INTO [AttributeValue] ( [IsSystem], [AttributeId], [EntityId], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '99B3387C-7526-43CC-92AB-AC8E173E95F6' ), @ItemId, 'explore', NEWID();
                IF @ItemId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [AttributeValue] av2 JOIN [Attribute] a2 ON a2.[Id] = av2.[AttributeId] AND a2.[Guid] = 'A05333F7-EC3B-4BFD-98A9-40C33CB7BEB0' WHERE av2.[EntityId] = @ItemId )
                    INSERT INTO [AttributeValue] ( [IsSystem], [AttributeId], [EntityId], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = 'A05333F7-EC3B-4BFD-98A9-40C33CB7BEB0' ), @ItemId, 'Icon Row', NEWID();
                IF @ItemId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [AttributeValue] av2 JOIN [Attribute] a2 ON a2.[Id] = av2.[AttributeId] AND a2.[Guid] = '73690FC9-9D86-4468-BCF5-FF683B9FDDAD' WHERE av2.[EntityId] = @ItemId )
                    INSERT INTO [AttributeValue] ( [IsSystem], [AttributeId], [EntityId], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '73690FC9-9D86-4468-BCF5-FF683B9FDDAD' ), @ItemId, 'calendar-alt', NEWID();
                IF @ItemId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [AttributeValue] av2 JOIN [Attribute] a2 ON a2.[Id] = av2.[AttributeId] AND a2.[Guid] = '92E457C6-3A42-43C4-BBC8-05790F5F5941' WHERE av2.[EntityId] = @ItemId )
                    INSERT INTO [AttributeValue] ( [IsSystem], [AttributeId], [EntityId], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '92E457C6-3A42-43C4-BBC8-05790F5F5941' ), @ItemId, 'cbb319a2-9c4e-40fe-829a-55d70842efdc', NEWID();
                " );   // menu item: Events, Groups & Classes
            Sql( @"
                DECLARE @ChannelId INT = ( SELECT TOP 1 [Id] FROM [ContentChannel] WHERE [Guid] = 'DF6276E8-B00E-4E77-88D8-AB1241A4A8C7' );
                DECLARE @ChannelTypeId INT = ( SELECT TOP 1 [ContentChannelTypeId] FROM [ContentChannel] WHERE [Id] = @ChannelId );
                IF @ChannelId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [ContentChannelItem] WHERE [Guid] = '6B78CBC2-CD57-4605-A848-E6057B6570AB' )
                BEGIN
                    INSERT INTO [ContentChannelItem] ( [ContentChannelId], [ContentChannelTypeId], [Title], [Content], [Priority], [Status], [StartDateTime], [Order], [Guid] )
                    VALUES ( @ChannelId, @ChannelTypeId, 'Discover Media & Podcasts', '', 0, 1, GETDATE(), 9, '6B78CBC2-CD57-4605-A848-E6057B6570AB' );
                END
                DECLARE @ItemId INT = ( SELECT TOP 1 [Id] FROM [ContentChannelItem] WHERE [Guid] = '6B78CBC2-CD57-4605-A848-E6057B6570AB' );
                IF @ItemId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [AttributeValue] av2 JOIN [Attribute] a2 ON a2.[Id] = av2.[AttributeId] AND a2.[Guid] = '99B3387C-7526-43CC-92AB-AC8E173E95F6' WHERE av2.[EntityId] = @ItemId )
                    INSERT INTO [AttributeValue] ( [IsSystem], [AttributeId], [EntityId], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '99B3387C-7526-43CC-92AB-AC8E173E95F6' ), @ItemId, 'explore', NEWID();
                IF @ItemId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [AttributeValue] av2 JOIN [Attribute] a2 ON a2.[Id] = av2.[AttributeId] AND a2.[Guid] = 'A05333F7-EC3B-4BFD-98A9-40C33CB7BEB0' WHERE av2.[EntityId] = @ItemId )
                    INSERT INTO [AttributeValue] ( [IsSystem], [AttributeId], [EntityId], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = 'A05333F7-EC3B-4BFD-98A9-40C33CB7BEB0' ), @ItemId, 'Icon Row', NEWID();
                IF @ItemId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [AttributeValue] av2 JOIN [Attribute] a2 ON a2.[Id] = av2.[AttributeId] AND a2.[Guid] = '7FDC1A9A-1DCE-4B93-B2F5-EAA1F5663AF7' WHERE av2.[EntityId] = @ItemId )
                    INSERT INTO [AttributeValue] ( [IsSystem], [AttributeId], [EntityId], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '7FDC1A9A-1DCE-4B93-B2F5-EAA1F5663AF7' ), @ItemId, 'explore-podcasts', NEWID();
                IF @ItemId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [AttributeValue] av2 JOIN [Attribute] a2 ON a2.[Id] = av2.[AttributeId] AND a2.[Guid] = '73690FC9-9D86-4468-BCF5-FF683B9FDDAD' WHERE av2.[EntityId] = @ItemId )
                    INSERT INTO [AttributeValue] ( [IsSystem], [AttributeId], [EntityId], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '73690FC9-9D86-4468-BCF5-FF683B9FDDAD' ), @ItemId, 'key', NEWID();
                " );   // menu item: Discover Media & Podcasts
            Sql( @"
                DECLARE @ChannelId INT = ( SELECT TOP 1 [Id] FROM [ContentChannel] WHERE [Guid] = 'DF6276E8-B00E-4E77-88D8-AB1241A4A8C7' );
                DECLARE @ChannelTypeId INT = ( SELECT TOP 1 [ContentChannelTypeId] FROM [ContentChannel] WHERE [Id] = @ChannelId );
                IF @ChannelId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [ContentChannelItem] WHERE [Guid] = '55E49D6D-2D85-42B8-AC72-96F8F4A94295' )
                BEGIN
                    INSERT INTO [ContentChannelItem] ( [ContentChannelId], [ContentChannelTypeId], [Title], [Content], [Priority], [Status], [StartDateTime], [Order], [Guid] )
                    VALUES ( @ChannelId, @ChannelTypeId, 'Sermon Library', '', 0, 1, GETDATE(), 10, '55E49D6D-2D85-42B8-AC72-96F8F4A94295' );
                END
                DECLARE @ItemId INT = ( SELECT TOP 1 [Id] FROM [ContentChannelItem] WHERE [Guid] = '55E49D6D-2D85-42B8-AC72-96F8F4A94295' );
                IF @ItemId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [AttributeValue] av2 JOIN [Attribute] a2 ON a2.[Id] = av2.[AttributeId] AND a2.[Guid] = '99B3387C-7526-43CC-92AB-AC8E173E95F6' WHERE av2.[EntityId] = @ItemId )
                    INSERT INTO [AttributeValue] ( [IsSystem], [AttributeId], [EntityId], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '99B3387C-7526-43CC-92AB-AC8E173E95F6' ), @ItemId, 'explore', NEWID();
                IF @ItemId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [AttributeValue] av2 JOIN [Attribute] a2 ON a2.[Id] = av2.[AttributeId] AND a2.[Guid] = 'A05333F7-EC3B-4BFD-98A9-40C33CB7BEB0' WHERE av2.[EntityId] = @ItemId )
                    INSERT INTO [AttributeValue] ( [IsSystem], [AttributeId], [EntityId], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = 'A05333F7-EC3B-4BFD-98A9-40C33CB7BEB0' ), @ItemId, 'Icon Row', NEWID();
                IF @ItemId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [AttributeValue] av2 JOIN [Attribute] a2 ON a2.[Id] = av2.[AttributeId] AND a2.[Guid] = '73690FC9-9D86-4468-BCF5-FF683B9FDDAD' WHERE av2.[EntityId] = @ItemId )
                    INSERT INTO [AttributeValue] ( [IsSystem], [AttributeId], [EntityId], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '73690FC9-9D86-4468-BCF5-FF683B9FDDAD' ), @ItemId, 'play', NEWID();
                IF @ItemId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [AttributeValue] av2 JOIN [Attribute] a2 ON a2.[Id] = av2.[AttributeId] AND a2.[Guid] = '92E457C6-3A42-43C4-BBC8-05790F5F5941' WHERE av2.[EntityId] = @ItemId )
                    INSERT INTO [AttributeValue] ( [IsSystem], [AttributeId], [EntityId], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '92E457C6-3A42-43C4-BBC8-05790F5F5941' ), @ItemId, 'f42357ce-077b-4986-b602-cdbaf2eaeaad', NEWID();
                " );   // menu item: Sermon Library
            Sql( @"
                DECLARE @ChannelId INT = ( SELECT TOP 1 [Id] FROM [ContentChannel] WHERE [Guid] = 'DF6276E8-B00E-4E77-88D8-AB1241A4A8C7' );
                DECLARE @ChannelTypeId INT = ( SELECT TOP 1 [ContentChannelTypeId] FROM [ContentChannel] WHERE [Id] = @ChannelId );
                IF @ChannelId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [ContentChannelItem] WHERE [Guid] = '2060F127-0602-470D-A142-F472C7CC3DF2' )
                BEGIN
                    INSERT INTO [ContentChannelItem] ( [ContentChannelId], [ContentChannelTypeId], [Title], [Content], [Priority], [Status], [StartDateTime], [Order], [Guid] )
                    VALUES ( @ChannelId, @ChannelTypeId, 'Staff Directory', '', 0, 1, GETDATE(), 13, '2060F127-0602-470D-A142-F472C7CC3DF2' );
                END
                DECLARE @ItemId INT = ( SELECT TOP 1 [Id] FROM [ContentChannelItem] WHERE [Guid] = '2060F127-0602-470D-A142-F472C7CC3DF2' );
                IF @ItemId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [AttributeValue] av2 JOIN [Attribute] a2 ON a2.[Id] = av2.[AttributeId] AND a2.[Guid] = '99B3387C-7526-43CC-92AB-AC8E173E95F6' WHERE av2.[EntityId] = @ItemId )
                    INSERT INTO [AttributeValue] ( [IsSystem], [AttributeId], [EntityId], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '99B3387C-7526-43CC-92AB-AC8E173E95F6' ), @ItemId, 'explore', NEWID();
                IF @ItemId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [AttributeValue] av2 JOIN [Attribute] a2 ON a2.[Id] = av2.[AttributeId] AND a2.[Guid] = 'A05333F7-EC3B-4BFD-98A9-40C33CB7BEB0' WHERE av2.[EntityId] = @ItemId )
                    INSERT INTO [AttributeValue] ( [IsSystem], [AttributeId], [EntityId], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = 'A05333F7-EC3B-4BFD-98A9-40C33CB7BEB0' ), @ItemId, 'Icon Row', NEWID();
                IF @ItemId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [AttributeValue] av2 JOIN [Attribute] a2 ON a2.[Id] = av2.[AttributeId] AND a2.[Guid] = '73690FC9-9D86-4468-BCF5-FF683B9FDDAD' WHERE av2.[EntityId] = @ItemId )
                    INSERT INTO [AttributeValue] ( [IsSystem], [AttributeId], [EntityId], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '73690FC9-9D86-4468-BCF5-FF683B9FDDAD' ), @ItemId, 'user-heart', NEWID();
                IF @ItemId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [AttributeValue] av2 JOIN [Attribute] a2 ON a2.[Id] = av2.[AttributeId] AND a2.[Guid] = '92E457C6-3A42-43C4-BBC8-05790F5F5941' WHERE av2.[EntityId] = @ItemId )
                    INSERT INTO [AttributeValue] ( [IsSystem], [AttributeId], [EntityId], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '92E457C6-3A42-43C4-BBC8-05790F5F5941' ), @ItemId, '2b7f224a-ea02-4e60-98b0-bcbba72ceb7c', NEWID();
                " );   // menu item: Staff Directory
            Sql( @"
                DECLARE @ChannelId INT = ( SELECT TOP 1 [Id] FROM [ContentChannel] WHERE [Guid] = 'DF6276E8-B00E-4E77-88D8-AB1241A4A8C7' );
                DECLARE @ChannelTypeId INT = ( SELECT TOP 1 [ContentChannelTypeId] FROM [ContentChannel] WHERE [Id] = @ChannelId );
                IF @ChannelId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [ContentChannelItem] WHERE [Guid] = 'A6234DE7-90BC-4D92-BFB4-17997F9B8303' )
                BEGIN
                    INSERT INTO [ContentChannelItem] ( [ContentChannelId], [ContentChannelTypeId], [Title], [Content], [Priority], [Status], [StartDateTime], [Order], [Guid] )
                    VALUES ( @ChannelId, @ChannelTypeId, 'About Nfluence', '', 0, 1, GETDATE(), 14, 'A6234DE7-90BC-4D92-BFB4-17997F9B8303' );
                END
                DECLARE @ItemId INT = ( SELECT TOP 1 [Id] FROM [ContentChannelItem] WHERE [Guid] = 'A6234DE7-90BC-4D92-BFB4-17997F9B8303' );
                IF @ItemId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [AttributeValue] av2 JOIN [Attribute] a2 ON a2.[Id] = av2.[AttributeId] AND a2.[Guid] = '99B3387C-7526-43CC-92AB-AC8E173E95F6' WHERE av2.[EntityId] = @ItemId )
                    INSERT INTO [AttributeValue] ( [IsSystem], [AttributeId], [EntityId], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '99B3387C-7526-43CC-92AB-AC8E173E95F6' ), @ItemId, 'explore', NEWID();
                IF @ItemId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [AttributeValue] av2 JOIN [Attribute] a2 ON a2.[Id] = av2.[AttributeId] AND a2.[Guid] = 'A05333F7-EC3B-4BFD-98A9-40C33CB7BEB0' WHERE av2.[EntityId] = @ItemId )
                    INSERT INTO [AttributeValue] ( [IsSystem], [AttributeId], [EntityId], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = 'A05333F7-EC3B-4BFD-98A9-40C33CB7BEB0' ), @ItemId, 'Icon Row', NEWID();
                IF @ItemId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [AttributeValue] av2 JOIN [Attribute] a2 ON a2.[Id] = av2.[AttributeId] AND a2.[Guid] = '7FDC1A9A-1DCE-4B93-B2F5-EAA1F5663AF7' WHERE av2.[EntityId] = @ItemId )
                    INSERT INTO [AttributeValue] ( [IsSystem], [AttributeId], [EntityId], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '7FDC1A9A-1DCE-4B93-B2F5-EAA1F5663AF7' ), @ItemId, 'explore-about', NEWID();
                IF @ItemId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [AttributeValue] av2 JOIN [Attribute] a2 ON a2.[Id] = av2.[AttributeId] AND a2.[Guid] = '73690FC9-9D86-4468-BCF5-FF683B9FDDAD' WHERE av2.[EntityId] = @ItemId )
                    INSERT INTO [AttributeValue] ( [IsSystem], [AttributeId], [EntityId], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '73690FC9-9D86-4468-BCF5-FF683B9FDDAD' ), @ItemId, 'info-circle', NEWID();
                " );   // menu item: About Nfluence
            Sql( @"
                DECLARE @ChannelId INT = ( SELECT TOP 1 [Id] FROM [ContentChannel] WHERE [Guid] = 'DF6276E8-B00E-4E77-88D8-AB1241A4A8C7' );
                DECLARE @ChannelTypeId INT = ( SELECT TOP 1 [ContentChannelTypeId] FROM [ContentChannel] WHERE [Id] = @ChannelId );
                IF @ChannelId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [ContentChannelItem] WHERE [Guid] = '3D4C90AA-0AFA-410A-87A8-930990092A2B' )
                BEGIN
                    INSERT INTO [ContentChannelItem] ( [ContentChannelId], [ContentChannelTypeId], [Title], [Content], [Priority], [Status], [StartDateTime], [Order], [Guid] )
                    VALUES ( @ChannelId, @ChannelTypeId, 'Bible', '', 0, 1, GETDATE(), 15, '3D4C90AA-0AFA-410A-87A8-930990092A2B' );
                END
                DECLARE @ItemId INT = ( SELECT TOP 1 [Id] FROM [ContentChannelItem] WHERE [Guid] = '3D4C90AA-0AFA-410A-87A8-930990092A2B' );
                IF @ItemId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [AttributeValue] av2 JOIN [Attribute] a2 ON a2.[Id] = av2.[AttributeId] AND a2.[Guid] = '99B3387C-7526-43CC-92AB-AC8E173E95F6' WHERE av2.[EntityId] = @ItemId )
                    INSERT INTO [AttributeValue] ( [IsSystem], [AttributeId], [EntityId], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '99B3387C-7526-43CC-92AB-AC8E173E95F6' ), @ItemId, 'explore-podcasts', NEWID();
                IF @ItemId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [AttributeValue] av2 JOIN [Attribute] a2 ON a2.[Id] = av2.[AttributeId] AND a2.[Guid] = 'A05333F7-EC3B-4BFD-98A9-40C33CB7BEB0' WHERE av2.[EntityId] = @ItemId )
                    INSERT INTO [AttributeValue] ( [IsSystem], [AttributeId], [EntityId], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = 'A05333F7-EC3B-4BFD-98A9-40C33CB7BEB0' ), @ItemId, 'Plain Row', NEWID();
                IF @ItemId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [AttributeValue] av2 JOIN [Attribute] a2 ON a2.[Id] = av2.[AttributeId] AND a2.[Guid] = '92E457C6-3A42-43C4-BBC8-05790F5F5941' WHERE av2.[EntityId] = @ItemId )
                    INSERT INTO [AttributeValue] ( [IsSystem], [AttributeId], [EntityId], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '92E457C6-3A42-43C4-BBC8-05790F5F5941' ), @ItemId, '6419fa93-a317-47fc-9c8b-a4265f7bc7ef', NEWID();
                " );   // menu item: Bible
            Sql( @"
                DECLARE @ChannelId INT = ( SELECT TOP 1 [Id] FROM [ContentChannel] WHERE [Guid] = 'DF6276E8-B00E-4E77-88D8-AB1241A4A8C7' );
                DECLARE @ChannelTypeId INT = ( SELECT TOP 1 [ContentChannelTypeId] FROM [ContentChannel] WHERE [Id] = @ChannelId );
                IF @ChannelId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [ContentChannelItem] WHERE [Guid] = 'A42AA426-59D2-40FC-B5C6-90A438E4309A' )
                BEGIN
                    INSERT INTO [ContentChannelItem] ( [ContentChannelId], [ContentChannelTypeId], [Title], [Content], [Priority], [Status], [StartDateTime], [Order], [Guid] )
                    VALUES ( @ChannelId, @ChannelTypeId, 'Podcasts', '', 0, 1, GETDATE(), 16, 'A42AA426-59D2-40FC-B5C6-90A438E4309A' );
                END
                DECLARE @ItemId INT = ( SELECT TOP 1 [Id] FROM [ContentChannelItem] WHERE [Guid] = 'A42AA426-59D2-40FC-B5C6-90A438E4309A' );
                IF @ItemId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [AttributeValue] av2 JOIN [Attribute] a2 ON a2.[Id] = av2.[AttributeId] AND a2.[Guid] = '99B3387C-7526-43CC-92AB-AC8E173E95F6' WHERE av2.[EntityId] = @ItemId )
                    INSERT INTO [AttributeValue] ( [IsSystem], [AttributeId], [EntityId], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '99B3387C-7526-43CC-92AB-AC8E173E95F6' ), @ItemId, 'explore-podcasts', NEWID();
                IF @ItemId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [AttributeValue] av2 JOIN [Attribute] a2 ON a2.[Id] = av2.[AttributeId] AND a2.[Guid] = 'A05333F7-EC3B-4BFD-98A9-40C33CB7BEB0' WHERE av2.[EntityId] = @ItemId )
                    INSERT INTO [AttributeValue] ( [IsSystem], [AttributeId], [EntityId], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = 'A05333F7-EC3B-4BFD-98A9-40C33CB7BEB0' ), @ItemId, 'Plain Row', NEWID();
                IF @ItemId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [AttributeValue] av2 JOIN [Attribute] a2 ON a2.[Id] = av2.[AttributeId] AND a2.[Guid] = '92E457C6-3A42-43C4-BBC8-05790F5F5941' WHERE av2.[EntityId] = @ItemId )
                    INSERT INTO [AttributeValue] ( [IsSystem], [AttributeId], [EntityId], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '92E457C6-3A42-43C4-BBC8-05790F5F5941' ), @ItemId, '8eab3bb6-f327-420f-8fe7-00c78a2449c4', NEWID();
                " );   // menu item: Podcasts
            Sql( @"
                DECLARE @ChannelId INT = ( SELECT TOP 1 [Id] FROM [ContentChannel] WHERE [Guid] = 'DF6276E8-B00E-4E77-88D8-AB1241A4A8C7' );
                DECLARE @ChannelTypeId INT = ( SELECT TOP 1 [ContentChannelTypeId] FROM [ContentChannel] WHERE [Id] = @ChannelId );
                IF @ChannelId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [ContentChannelItem] WHERE [Guid] = '4485A230-B524-4948-93B7-055AD9400A9E' )
                BEGIN
                    INSERT INTO [ContentChannelItem] ( [ContentChannelId], [ContentChannelTypeId], [Title], [Content], [Priority], [Status], [StartDateTime], [Order], [Guid] )
                    VALUES ( @ChannelId, @ChannelTypeId, 'Notifications', '', 0, 1, GETDATE(), 18, '4485A230-B524-4948-93B7-055AD9400A9E' );
                END
                DECLARE @ItemId INT = ( SELECT TOP 1 [Id] FROM [ContentChannelItem] WHERE [Guid] = '4485A230-B524-4948-93B7-055AD9400A9E' );
                IF @ItemId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [AttributeValue] av2 JOIN [Attribute] a2 ON a2.[Id] = av2.[AttributeId] AND a2.[Guid] = '99B3387C-7526-43CC-92AB-AC8E173E95F6' WHERE av2.[EntityId] = @ItemId )
                    INSERT INTO [AttributeValue] ( [IsSystem], [AttributeId], [EntityId], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '99B3387C-7526-43CC-92AB-AC8E173E95F6' ), @ItemId, 'profile', NEWID();
                IF @ItemId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [AttributeValue] av2 JOIN [Attribute] a2 ON a2.[Id] = av2.[AttributeId] AND a2.[Guid] = 'A05333F7-EC3B-4BFD-98A9-40C33CB7BEB0' WHERE av2.[EntityId] = @ItemId )
                    INSERT INTO [AttributeValue] ( [IsSystem], [AttributeId], [EntityId], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = 'A05333F7-EC3B-4BFD-98A9-40C33CB7BEB0' ), @ItemId, 'Icon Row', NEWID();
                IF @ItemId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [AttributeValue] av2 JOIN [Attribute] a2 ON a2.[Id] = av2.[AttributeId] AND a2.[Guid] = '73690FC9-9D86-4468-BCF5-FF683B9FDDAD' WHERE av2.[EntityId] = @ItemId )
                    INSERT INTO [AttributeValue] ( [IsSystem], [AttributeId], [EntityId], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '73690FC9-9D86-4468-BCF5-FF683B9FDDAD' ), @ItemId, 'comments', NEWID();
                IF @ItemId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [AttributeValue] av2 JOIN [Attribute] a2 ON a2.[Id] = av2.[AttributeId] AND a2.[Guid] = '92E457C6-3A42-43C4-BBC8-05790F5F5941' WHERE av2.[EntityId] = @ItemId )
                    INSERT INTO [AttributeValue] ( [IsSystem], [AttributeId], [EntityId], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '92E457C6-3A42-43C4-BBC8-05790F5F5941' ), @ItemId, '9d8435bd-8583-4325-aefc-af073d0e9020', NEWID();
                " );   // menu item: Notifications
            Sql( @"
                DECLARE @ChannelId INT = ( SELECT TOP 1 [Id] FROM [ContentChannel] WHERE [Guid] = 'DF6276E8-B00E-4E77-88D8-AB1241A4A8C7' );
                DECLARE @ChannelTypeId INT = ( SELECT TOP 1 [ContentChannelTypeId] FROM [ContentChannel] WHERE [Id] = @ChannelId );
                IF @ChannelId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [ContentChannelItem] WHERE [Guid] = 'BB95890F-A98F-47F5-9909-1260D05B9D24' )
                BEGIN
                    INSERT INTO [ContentChannelItem] ( [ContentChannelId], [ContentChannelTypeId], [Title], [Content], [Priority], [Status], [StartDateTime], [Order], [Guid] )
                    VALUES ( @ChannelId, @ChannelTypeId, 'My Serving Schedule', '', 0, 1, GETDATE(), 19, 'BB95890F-A98F-47F5-9909-1260D05B9D24' );
                END
                DECLARE @ItemId INT = ( SELECT TOP 1 [Id] FROM [ContentChannelItem] WHERE [Guid] = 'BB95890F-A98F-47F5-9909-1260D05B9D24' );
                IF @ItemId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [AttributeValue] av2 JOIN [Attribute] a2 ON a2.[Id] = av2.[AttributeId] AND a2.[Guid] = '99B3387C-7526-43CC-92AB-AC8E173E95F6' WHERE av2.[EntityId] = @ItemId )
                    INSERT INTO [AttributeValue] ( [IsSystem], [AttributeId], [EntityId], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '99B3387C-7526-43CC-92AB-AC8E173E95F6' ), @ItemId, 'profile', NEWID();
                IF @ItemId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [AttributeValue] av2 JOIN [Attribute] a2 ON a2.[Id] = av2.[AttributeId] AND a2.[Guid] = 'A05333F7-EC3B-4BFD-98A9-40C33CB7BEB0' WHERE av2.[EntityId] = @ItemId )
                    INSERT INTO [AttributeValue] ( [IsSystem], [AttributeId], [EntityId], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = 'A05333F7-EC3B-4BFD-98A9-40C33CB7BEB0' ), @ItemId, 'Icon Row', NEWID();
                IF @ItemId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [AttributeValue] av2 JOIN [Attribute] a2 ON a2.[Id] = av2.[AttributeId] AND a2.[Guid] = '73690FC9-9D86-4468-BCF5-FF683B9FDDAD' WHERE av2.[EntityId] = @ItemId )
                    INSERT INTO [AttributeValue] ( [IsSystem], [AttributeId], [EntityId], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '73690FC9-9D86-4468-BCF5-FF683B9FDDAD' ), @ItemId, 'hand-paper', NEWID();
                IF @ItemId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [AttributeValue] av2 JOIN [Attribute] a2 ON a2.[Id] = av2.[AttributeId] AND a2.[Guid] = '92E457C6-3A42-43C4-BBC8-05790F5F5941' WHERE av2.[EntityId] = @ItemId )
                    INSERT INTO [AttributeValue] ( [IsSystem], [AttributeId], [EntityId], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '92E457C6-3A42-43C4-BBC8-05790F5F5941' ), @ItemId, '56302b84-36e3-4e62-9e74-c5739d7de977', NEWID();
                " );   // menu item: My Serving Schedule
            Sql( @"
                DECLARE @ChannelId INT = ( SELECT TOP 1 [Id] FROM [ContentChannel] WHERE [Guid] = 'DF6276E8-B00E-4E77-88D8-AB1241A4A8C7' );
                DECLARE @ChannelTypeId INT = ( SELECT TOP 1 [ContentChannelTypeId] FROM [ContentChannel] WHERE [Id] = @ChannelId );
                IF @ChannelId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [ContentChannelItem] WHERE [Guid] = '16BDBBFB-1594-4B10-91C3-11EC9E0704D0' )
                BEGIN
                    INSERT INTO [ContentChannelItem] ( [ContentChannelId], [ContentChannelTypeId], [Title], [Content], [Priority], [Status], [StartDateTime], [Order], [Guid] )
                    VALUES ( @ChannelId, @ChannelTypeId, 'My Groups', '', 0, 1, GETDATE(), 20, '16BDBBFB-1594-4B10-91C3-11EC9E0704D0' );
                END
                DECLARE @ItemId INT = ( SELECT TOP 1 [Id] FROM [ContentChannelItem] WHERE [Guid] = '16BDBBFB-1594-4B10-91C3-11EC9E0704D0' );
                IF @ItemId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [AttributeValue] av2 JOIN [Attribute] a2 ON a2.[Id] = av2.[AttributeId] AND a2.[Guid] = '99B3387C-7526-43CC-92AB-AC8E173E95F6' WHERE av2.[EntityId] = @ItemId )
                    INSERT INTO [AttributeValue] ( [IsSystem], [AttributeId], [EntityId], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '99B3387C-7526-43CC-92AB-AC8E173E95F6' ), @ItemId, 'profile', NEWID();
                IF @ItemId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [AttributeValue] av2 JOIN [Attribute] a2 ON a2.[Id] = av2.[AttributeId] AND a2.[Guid] = 'A05333F7-EC3B-4BFD-98A9-40C33CB7BEB0' WHERE av2.[EntityId] = @ItemId )
                    INSERT INTO [AttributeValue] ( [IsSystem], [AttributeId], [EntityId], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = 'A05333F7-EC3B-4BFD-98A9-40C33CB7BEB0' ), @ItemId, 'Icon Row', NEWID();
                IF @ItemId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [AttributeValue] av2 JOIN [Attribute] a2 ON a2.[Id] = av2.[AttributeId] AND a2.[Guid] = '73690FC9-9D86-4468-BCF5-FF683B9FDDAD' WHERE av2.[EntityId] = @ItemId )
                    INSERT INTO [AttributeValue] ( [IsSystem], [AttributeId], [EntityId], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '73690FC9-9D86-4468-BCF5-FF683B9FDDAD' ), @ItemId, 'sitemap', NEWID();
                IF @ItemId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [AttributeValue] av2 JOIN [Attribute] a2 ON a2.[Id] = av2.[AttributeId] AND a2.[Guid] = '92E457C6-3A42-43C4-BBC8-05790F5F5941' WHERE av2.[EntityId] = @ItemId )
                    INSERT INTO [AttributeValue] ( [IsSystem], [AttributeId], [EntityId], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '92E457C6-3A42-43C4-BBC8-05790F5F5941' ), @ItemId, '3430667f-d38c-4a9c-a65e-bc8d15b4fc51', NEWID();
                " );   // menu item: My Groups
            Sql( @"
                DECLARE @ChannelId INT = ( SELECT TOP 1 [Id] FROM [ContentChannel] WHERE [Guid] = 'DF6276E8-B00E-4E77-88D8-AB1241A4A8C7' );
                DECLARE @ChannelTypeId INT = ( SELECT TOP 1 [ContentChannelTypeId] FROM [ContentChannel] WHERE [Id] = @ChannelId );
                IF @ChannelId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [ContentChannelItem] WHERE [Guid] = 'B19742C1-D11E-4CA0-BAD0-9911DE68AE74' )
                BEGIN
                    INSERT INTO [ContentChannelItem] ( [ContentChannelId], [ContentChannelTypeId], [Title], [Content], [Priority], [Status], [StartDateTime], [Order], [Guid] )
                    VALUES ( @ChannelId, @ChannelTypeId, 'Check-In', '', 0, 1, GETDATE(), 21, 'B19742C1-D11E-4CA0-BAD0-9911DE68AE74' );
                END
                DECLARE @ItemId INT = ( SELECT TOP 1 [Id] FROM [ContentChannelItem] WHERE [Guid] = 'B19742C1-D11E-4CA0-BAD0-9911DE68AE74' );
                IF @ItemId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [AttributeValue] av2 JOIN [Attribute] a2 ON a2.[Id] = av2.[AttributeId] AND a2.[Guid] = '99B3387C-7526-43CC-92AB-AC8E173E95F6' WHERE av2.[EntityId] = @ItemId )
                    INSERT INTO [AttributeValue] ( [IsSystem], [AttributeId], [EntityId], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '99B3387C-7526-43CC-92AB-AC8E173E95F6' ), @ItemId, 'profile', NEWID();
                IF @ItemId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [AttributeValue] av2 JOIN [Attribute] a2 ON a2.[Id] = av2.[AttributeId] AND a2.[Guid] = 'A05333F7-EC3B-4BFD-98A9-40C33CB7BEB0' WHERE av2.[EntityId] = @ItemId )
                    INSERT INTO [AttributeValue] ( [IsSystem], [AttributeId], [EntityId], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = 'A05333F7-EC3B-4BFD-98A9-40C33CB7BEB0' ), @ItemId, 'Icon Row', NEWID();
                IF @ItemId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [AttributeValue] av2 JOIN [Attribute] a2 ON a2.[Id] = av2.[AttributeId] AND a2.[Guid] = '73690FC9-9D86-4468-BCF5-FF683B9FDDAD' WHERE av2.[EntityId] = @ItemId )
                    INSERT INTO [AttributeValue] ( [IsSystem], [AttributeId], [EntityId], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '73690FC9-9D86-4468-BCF5-FF683B9FDDAD' ), @ItemId, 'check-circle', NEWID();
                IF @ItemId IS NOT NULL AND NOT EXISTS ( SELECT 1 FROM [AttributeValue] av2 JOIN [Attribute] a2 ON a2.[Id] = av2.[AttributeId] AND a2.[Guid] = '92E457C6-3A42-43C4-BBC8-05790F5F5941' WHERE av2.[EntityId] = @ItemId )
                    INSERT INTO [AttributeValue] ( [IsSystem], [AttributeId], [EntityId], [Value], [Guid] )
                    SELECT 0, ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '92E457C6-3A42-43C4-BBC8-05790F5F5941' ), @ItemId, 'b29e8335-21c0-4459-a1ad-d85537fc2c08', NEWID();
                " );   // menu item: Check-In

            //
            // Supporting data: Note Types (entity = Rock.Model.Group)
            //
            Sql( @"
                DECLARE @GroupEntityTypeId INT = ( SELECT TOP 1 [Id] FROM [EntityType] WHERE [Name] = 'Rock.Model.Group' );

                IF NOT EXISTS ( SELECT 1 FROM [NoteType] WHERE [Guid] = '5F272031-0C1F-4503-8F71-557D44BB4E19' )
                BEGIN
                    INSERT INTO [NoteType] ( [IsSystem], [EntityTypeId], [Name], [UserSelectable], [IconCssClass],
                                             [Order], [EntityTypeQualifierColumn], [EntityTypeQualifierValue],
                                             [AllowsWatching], [AllowsReplies], [Guid] )
                    VALUES ( 0, @GroupEntityTypeId, 'Group Message', 1, 'fa fa-comment', 0, '', '', 0, 1,
                             '5F272031-0C1F-4503-8F71-557D44BB4E19' );
                END

                IF NOT EXISTS ( SELECT 1 FROM [NoteType] WHERE [Guid] = '8717C4C5-DFF8-41BB-8F94-7277981CB1B6' )
                BEGIN
                    INSERT INTO [NoteType] ( [IsSystem], [EntityTypeId], [Name], [UserSelectable], [IconCssClass],
                                             [Order], [EntityTypeQualifierColumn], [EntityTypeQualifierValue],
                                             [AllowsWatching], [AllowsReplies], [Guid] )
                    VALUES ( 0, @GroupEntityTypeId, 'Group Need', 1, 'fa fa-hands-helping', 1, '', '', 0, 1,
                             '8717C4C5-DFF8-41BB-8F94-7277981CB1B6' );
                END
                " );

            //
            // REST security required by the mobile app
            //   - Followings  (Save / heart on sermons + series)
            //   - UpdatePersonProfilePhoto  (profile photo editing)
            //   Granted to 'RSR - Mobile Application Users'
            //
            Sql( @"
                DECLARE @MobileRoleGuid UNIQUEIDENTIFIER = '42175217-1BA4-401B-AA4E-21EC4F1F0AB4';
                DECLARE @MobileGroupId INT = ( SELECT TOP 1 [Id] FROM [Group] WHERE [Guid] = @MobileRoleGuid );
                DECLARE @RestActionEntityTypeId INT = ( SELECT TOP 1 [Id] FROM [EntityType] WHERE [Name] = 'Rock.Model.RestAction' );

                IF @MobileGroupId IS NOT NULL AND @RestActionEntityTypeId IS NOT NULL
                BEGIN
                    INSERT INTO [Auth] ( [EntityTypeId], [EntityId], [Order], [Action], [AllowOrDeny], [SpecialRole], [GroupId], [Guid] )
                    SELECT @RestActionEntityTypeId, ra.[Id], 0, 'Edit', 'A', 0, @MobileGroupId, NEWID()
                    FROM [RestAction] ra
                    WHERE ra.[ApiId] LIKE '%UpdatePersonProfilePhoto%'
                      AND NOT EXISTS ( SELECT 1 FROM [Auth] a
                                       WHERE a.[EntityTypeId] = @RestActionEntityTypeId
                                         AND a.[EntityId] = ra.[Id]
                                         AND a.[Action] = 'Edit'
                                         AND a.[GroupId] = @MobileGroupId );

                    INSERT INTO [Auth] ( [EntityTypeId], [EntityId], [Order], [Action], [AllowOrDeny], [SpecialRole], [GroupId], [Guid] )
                    SELECT @RestActionEntityTypeId, ra.[Id], 0, 'Edit', 'A', 0, @MobileGroupId, NEWID()
                    FROM [RestAction] ra
                    WHERE ra.[ApiId] LIKE 'POSTapi/Followings%'
                      AND NOT EXISTS ( SELECT 1 FROM [Auth] a
                                       WHERE a.[EntityTypeId] = @RestActionEntityTypeId
                                         AND a.[EntityId] = ra.[Id]
                                         AND a.[Action] = 'Edit'
                                         AND a.[GroupId] = @MobileGroupId );
                END
                " );

            //
            // Custom attributes added to STANDARD content channels (Messages, Message Notes).
            // NOTE: those channels are install specific - verify the channel Guids below on the
            // target server, or set them by hand, before running.
            //
            Sql( @"
                DECLARE @EntityTypeId INT = ( SELECT TOP 1 [Id] FROM [EntityType] WHERE [Name] = 'Rock.Model.ContentChannelItem' );

                -- Messages -> Media File (Resi video)
                DECLARE @MsgChannelId INT = ( SELECT TOP 1 [Id] FROM [ContentChannel] WHERE [Guid] = '0A63A427-E6B5-2284-45B3-789B293C02EA' );
                DECLARE @MediaFieldTypeId INT = ( SELECT TOP 1 [Id] FROM [FieldType] WHERE [Guid] = 'A17D5AAC-B7AE-4587-B703-A0FC3625F0F8' );
                IF @MsgChannelId IS NOT NULL AND @MediaFieldTypeId IS NOT NULL
                   AND NOT EXISTS ( SELECT 1 FROM [Attribute] WHERE [Guid] = '0C85EE81-1327-4861-AC05-D4AB41D408C1' )
                BEGIN
                    INSERT INTO [Attribute] ( [IsSystem], [FieldTypeId], [EntityTypeId], [EntityTypeQualifierColumn], [EntityTypeQualifierValue],
                                              [Key], [Name], [Description], [Order], [IsGridColumn], [DefaultValue], [IsMultiValue], [IsRequired], [Guid] )
                    VALUES ( 0, @MediaFieldTypeId, @EntityTypeId, 'ContentChannelId', CAST( @MsgChannelId AS NVARCHAR(10) ),
                             'MediaFile', 'Media File', 'Resi media element used by the sermon player.', 1011, 0, '', 0, 0,
                             '0C85EE81-1327-4861-AC05-D4AB41D408C1' );
                END

                -- Message Notes -> Scripture References (value list, rendered as tappable chips)
                DECLARE @NotesChannelId INT = ( SELECT TOP 1 [Id] FROM [ContentChannel] WHERE [Guid] = '48951E97-0E45-4494-B87C-4EB9FCA067EB' );
                DECLARE @ValueListFieldTypeId INT = ( SELECT TOP 1 [Id] FROM [FieldType] WHERE [Guid] = '7BDAE237-6E49-47AC-9961-A45AFB69E240' );
                IF @NotesChannelId IS NOT NULL AND @ValueListFieldTypeId IS NOT NULL
                   AND NOT EXISTS ( SELECT 1 FROM [Attribute] WHERE [Guid] = 'C607CE45-A2A8-4C69-8172-60DE4FA78F5A' )
                BEGIN
                    INSERT INTO [Attribute] ( [IsSystem], [FieldTypeId], [EntityTypeId], [EntityTypeQualifierColumn], [EntityTypeQualifierValue],
                                              [Key], [Name], [Description], [Order], [IsGridColumn], [DefaultValue], [IsMultiValue], [IsRequired], [Guid] )
                    VALUES ( 0, @ValueListFieldTypeId, @EntityTypeId, 'ContentChannelId', CAST( @NotesChannelId AS NVARCHAR(10) ),
                             'ScriptureReferences', 'Scripture References', 'Passages shown as tappable chips, e.g. Revelation 4.', 1010, 0, '', 0, 0,
                             'C607CE45-A2A8-4C69-8172-60DE4FA78F5A' );
                END
                " );

            //
            // Event audience "Highlight Color" values.
            //
            // The mobile Calendar View block draws a vertical stack of dots on the right edge of
            // every event row, one per audience, colored from this attribute. There is no setting
            // to hide them (ShowPerAudienceEventIndicators only governs the month-grid dots), so
            // unset colors render as a washed-out default. Anything not present on the target
            // server is simply skipped.
            //
            Sql( @"
                DECLARE @AudienceDefinedTypeId INT = ( SELECT TOP 1 [Id] FROM [DefinedType] WHERE [Guid] = '799301A3-2026-4977-994E-45DC68502559' );
                DECLARE @AudienceColorAttrId INT = ( SELECT TOP 1 [Id] FROM [Attribute]
                                                     WHERE [Key] = 'HighlightColor'
                                                       AND [EntityTypeQualifierColumn] = 'DefinedTypeId'
                                                       AND [EntityTypeQualifierValue] = CAST( @AudienceDefinedTypeId AS NVARCHAR(10) ) );

                IF @AudienceColorAttrId IS NOT NULL
                BEGIN
                    DECLARE @AudienceColors TABLE ( DvGuid UNIQUEIDENTIFIER, Hex NVARCHAR(10) );
                    INSERT INTO @AudienceColors ( DvGuid, Hex ) VALUES
                        ( '6107EA37-5DD3-4E4F-A2D0-1D4010811D4D', '#DE5A25' ),   -- All Church (brand orange)
                        ( '95E49778-AE72-454F-91CC-2FC864557DEC', '#4A78C4' ),   -- Adults
                        ( '59CD7FD8-6A62-4C3B-8966-1520E74EED58', '#4FB3E8' ),   -- Youth
                        ( 'F2BFF319-A109-4B42-BEC2-76590E11627B', '#F2A93B' ),   -- Children
                        ( 'A4BEBC2F-09F0-488A-B2F3-C416F4D02E35', '#3F8F7A' ),   -- Men
                        ( '4CE2E860-2F03-40F9-8B60-68EBDB21E026', '#C264A8' ),   -- Women
                        ( '833EE2C7-F83A-4744-AD14-6907554DF8AE', '#8A8F98' ),   -- Staff
                        ( 'B364CDEE-F000-4965-AE67-0C80DDA365DC', '#6B7280' ),   -- Homepage - Rotator
                        ( '57B2A23F-3B0C-43A8-9F45-332120DCD0EE', '#9AA0AE' );   -- Homepage - Sub-Ads

                    UPDATE av
                       SET av.[Value] = c.Hex,
                           av.[ModifiedDateTime] = GETDATE()
                    FROM [AttributeValue] av
                    JOIN [DefinedValue] dv ON dv.[Id] = av.[EntityId]
                    JOIN @AudienceColors c ON c.DvGuid = dv.[Guid]
                    WHERE av.[AttributeId] = @AudienceColorAttrId;

                    INSERT INTO [AttributeValue] ( [IsSystem], [AttributeId], [EntityId], [Value], [Guid], [CreatedDateTime], [ModifiedDateTime] )
                    SELECT 0, @AudienceColorAttrId, dv.[Id], c.Hex, NEWID(), GETDATE(), GETDATE()
                    FROM [DefinedValue] dv
                    JOIN @AudienceColors c ON c.DvGuid = dv.[Guid]
                    WHERE NOT EXISTS ( SELECT 1 FROM [AttributeValue] av
                                       WHERE av.[AttributeId] = @AudienceColorAttrId
                                         AND av.[EntityId] = dv.[Id] );
                END
                " );

            //
            // Item Detail (page 16818) call-to-action button.
            //
            // Added to the App Home Feed channel. The template looks these up by
            // key and renders nothing when they are absent, so the same three can
            // be added to any other channel that routes to the item detail page.
            //
            Sql( @"
                DECLARE @CciEntityTypeId INT = ( SELECT TOP 1 [Id] FROM [EntityType] WHERE [Name] = 'Rock.Model.ContentChannelItem' );
                DECLARE @TextFieldTypeId INT = ( SELECT TOP 1 [Id] FROM [FieldType] WHERE [Guid] = '9C204CD0-1233-41C5-818A-C5DA439445AA' );
                DECLARE @UrlFieldTypeId  INT = ( SELECT TOP 1 [Id] FROM [FieldType] WHERE [Guid] = 'C0D0D7E2-C3B0-4004-ABEA-4BBFAD10D5D2' );
                DECLARE @BoolFieldTypeId INT = ( SELECT TOP 1 [Id] FROM [FieldType] WHERE [Guid] = '1EDAFDED-DFE6-4334-B019-6EECBA89E05A' );
                DECLARE @FeedChannelId   INT = ( SELECT TOP 1 [Id] FROM [ContentChannel] WHERE [Guid] = '122CAAAE-0698-4869-89FE-D818E109BAEA' );

                IF @FeedChannelId IS NOT NULL
                BEGIN
                    IF NOT EXISTS ( SELECT 1 FROM [Attribute] WHERE [Guid] = 'E2C6B94A-1F73-4E58-B0D9-6A4E8C15F7B3' )
                        INSERT INTO [Attribute] ( [IsSystem], [FieldTypeId], [EntityTypeId], [EntityTypeQualifierColumn], [EntityTypeQualifierValue],
                                                  [Key], [Name], [Description], [Order], [IsGridColumn], [DefaultValue], [IsMultiValue], [IsRequired], [Guid] )
                        VALUES ( 0, @UrlFieldTypeId, @CciEntityTypeId, 'ContentChannelId', CAST( @FeedChannelId AS NVARCHAR(10) ),
                                 'LinkUrl', 'Link Url', 'Adds a call-to-action button to the item detail page.',
                                 1020, 0, '', 0, 0, 'E2C6B94A-1F73-4E58-B0D9-6A4E8C15F7B3' );

                    IF NOT EXISTS ( SELECT 1 FROM [Attribute] WHERE [Guid] = 'F8D50A21-4C96-4B7E-83A1-2D9F6E0B85C4' )
                        INSERT INTO [Attribute] ( [IsSystem], [FieldTypeId], [EntityTypeId], [EntityTypeQualifierColumn], [EntityTypeQualifierValue],
                                                  [Key], [Name], [Description], [Order], [IsGridColumn], [DefaultValue], [IsMultiValue], [IsRequired], [Guid] )
                        VALUES ( 0, @TextFieldTypeId, @CciEntityTypeId, 'ContentChannelId', CAST( @FeedChannelId AS NVARCHAR(10) ),
                                 'LinkButtonText', 'Link Button Text', 'Label for the link button. Defaults to ""Learn More"".',
                                 1021, 0, '', 0, 0, 'F8D50A21-4C96-4B7E-83A1-2D9F6E0B85C4' );

                    IF NOT EXISTS ( SELECT 1 FROM [Attribute] WHERE [Guid] = '3B7E1D68-9A05-4C2F-B54D-7E8A0C36F91D' )
                        INSERT INTO [Attribute] ( [IsSystem], [FieldTypeId], [EntityTypeId], [EntityTypeQualifierColumn], [EntityTypeQualifierValue],
                                                  [Key], [Name], [Description], [Order], [IsGridColumn], [DefaultValue], [IsMultiValue], [IsRequired], [Guid] )
                        VALUES ( 0, @BoolFieldTypeId, @CciEntityTypeId, 'ContentChannelId', CAST( @FeedChannelId AS NVARCHAR(10) ),
                                 'LinkOpensExternally', 'Link Opens Externally',
                                 'On: hand off to the device browser. Off (default): open inside the app.',
                                 1022, 0, 'False', 0, 0, '3B7E1D68-9A05-4C2F-B54D-7E8A0C36F91D' );
                END
                " );

        }

        public override void Down()
        {
            // Intentionally left blank - removing an entire mobile application
            // is a destructive operation that should be done deliberately.
        }
    }
}
