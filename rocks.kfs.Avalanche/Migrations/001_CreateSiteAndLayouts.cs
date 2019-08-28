using Avalanche;
using Rock.Plugin;

namespace rocks.kfs.Avalanche.Migrations
{
    [MigrationNumber( 1, "1.8.0" )]
    public class CreateSiteAndLayouts : Migration
    {
        /// <summary>
        /// The commands to run to migrate plugin to the specific version
        /// </summary>
        public override void Up()
        {
            RockMigrationHelper.AddSite( "KFS Avalanche", "This site is used for the KFS Avalanche Mobile Application", "Avalanche", "613631FF-D19C-4F9C-B163-E9331C4BA61B" );
            RockMigrationHelper.AddLayout( "613631FF-D19C-4F9C-B163-E9331C4BA61B", "Boxes", "Boxes", "", "B510AB94-04B0-48C0-BEDD-16812BEDECB1" ); // Site:KFS Avalanche
            RockMigrationHelper.AddLayout( "613631FF-D19C-4F9C-B163-E9331C4BA61B", "Footer", "Footer", "", "60D99A36-8D00-467E-9993-3C2F0B249EBD" ); // Site:KFS Avalanche
            RockMigrationHelper.AddLayout( "613631FF-D19C-4F9C-B163-E9331C4BA61B", "MainPage", "Main Page", "", "FC61CD1A-15DC-4FDD-9DDD-4A0BD8936E16" ); // Site:KFS Avalanche
            RockMigrationHelper.AddLayout( "613631FF-D19C-4F9C-B163-E9331C4BA61B", "NoScroll", "No Scroll", "", "901926F9-AD81-41A4-9B1E-254F5B45E471" ); // Site:KFS Avalanche
            RockMigrationHelper.AddLayout( "613631FF-D19C-4F9C-B163-E9331C4BA61B", "Simple", "Simple", "", "355F6C23-29B3-4976-AE43-30426BE12B99" ); // Site:KFS Avalanche

            var simple = @"[
    {
        ""Name"": ""TopHorizontal"",
        ""Type"": ""Grid"",
        ""Attributes"": {
            ""HorizontalOptions"": ""FillAndExpand"",
            ""VerticalOptions"": ""Start"",
            ""MinimumHeightRequest"": ""70"",
            ""BackgroundColor"": ""#0094d9""
        },
        ""Children"": [
            {
                ""Name"": ""TopLeft"",
                ""Type"": ""StackLayout"",
                ""Orientation"": ""Vertical"",
                ""Row"": 0,
                ""Column"": 0,
                ""Attributes"": {}
            },
            {
                ""Name"": ""TopCenter"",
                ""Type"": ""StackLayout"",
                ""Orientation"": ""Vertical"",
                ""Row"": 0,
                ""Column"": 1,
                ""Attributes"": {}
            },
            {
                ""Name"": ""TopRight"",
                ""Type"": ""StackLayout"",
                ""Orientation"": ""Vertical"",
                ""Row"": 0,
                ""Column"": 2,
                ""Attributes"": {}
            }
        ]
    },
    {
        ""Name"": ""Featured"",
        ""Type"": ""StackLayout"",
        ""Orientation"": ""Vertical"",
        ""Attributes"": {
            ""VerticalOptions"": ""Start""
        }
    },
    {
        ""Name"": ""Container"",
        ""Type"": ""StackLayout"",
        ""Orientation"": ""Vertical"",
        ""ScrollY"": true,
        ""Attributes"": {
            ""VerticalOptions"": ""FillAndExpand""
        },
        ""Children"": [
            {
                ""Name"": ""Main"",
                ""Type"": ""StackLayout"",
                ""Orientation"": ""Vertical""
            },
            {
                ""Name"": ""Horizontal"",
                ""Type"": ""StackLayout"",
                ""Orientation"": ""Horizontal"",
                ""Children"": [
                    {
                        ""Name"": ""Left"",
                        ""Type"": ""StackLayout"",
                        ""Orientation"": ""Vertical""
                    },
                    {
                        ""Name"": ""Center"",
                        ""Type"": ""StackLayout"",
                        ""Orientation"": ""Vertical""
                    },
                    {
                        ""Name"": ""Right"",
                        ""Type"": ""StackLayout"",
                        ""Orientation"": ""Vertical""
                    }
                ]
            }
        ]
    }
]";

            //RockMigrationHelper.UpdateDefinedValue( AvalancheUtilities.LayoutsDefinedType, "Simple", "Simple layout with a featured, main, three sub sections, and a footer.", "BB5006D8-7F51-43D5-B977-5E07F5ACA8C2", false );
            RockMigrationHelper.UpdateDefinedValueAttributeValue( "BB5006D8-7F51-43D5-B977-5E07F5ACA8C2", "E5DE699C-49F6-488B-BA1F-F4CC13CE8B91", simple );

            var noscroll = @"[
    {
        ""Name"": ""TopHorizontal"",
        ""Type"": ""Grid"",
        ""Attributes"": {
            ""HorizontalOptions"": ""FillAndExpand"",
            ""VerticalOptions"": ""Start"",
            ""MinimumHeightRequest"": ""70"",
            ""BackgroundColor"": ""#0094d9""
        },
        ""Children"": [
            {
                ""Name"": ""TopLeft"",
                ""Type"": ""StackLayout"",
                ""Orientation"": ""Vertical"",
                ""Row"": 0,
                ""Column"": 0,
                ""Attributes"": {}
            },
            {
                ""Name"": ""TopCenter"",
                ""Type"": ""StackLayout"",
                ""Orientation"": ""Vertical"",
                ""Row"": 0,
                ""Column"": 1,
                ""Attributes"": {}
            },
            {
                ""Name"": ""TopRight"",
                ""Type"": ""StackLayout"",
                ""Orientation"": ""Vertical"",
                ""Row"": 0,
                ""Column"": 2,
                ""Attributes"": {}
            }
        ]
    },
    {
        ""Name"": ""Featured"",
        ""Type"": ""StackLayout"",
        ""Orientation"": ""Vertical"",
        ""Attributes"": {
            ""VerticalOptions"": ""Start""
        }
    },
    {
        ""Name"": ""Main"",
        ""Type"": ""StackLayout"",
        ""Orientation"": ""Vertical"",
        ""Attributes"": {
            ""VerticalOptions"": ""FillAndExpand""
        }
    },
    {
        ""Name"": ""Footer"",
        ""Type"": ""StackLayout"",
        ""Orientation"": ""Vertical"",
    }
]";
            //RockMigrationHelper.UpdateDefinedValue( AvalancheUtilities.LayoutsDefinedType, "No Scroll", "Layout with no scrolling elements. Good for list views.", "3812C543-8B80-4A7C-BBD0-4DEFEABBA7DC", false );
            RockMigrationHelper.UpdateDefinedValueAttributeValue( "3812C543-8B80-4A7C-BBD0-4DEFEABBA7DC", "E5DE699C-49F6-488B-BA1F-F4CC13CE8B91", noscroll );

            var mainpage = @"[
    {
        ""Name"": ""TopHorizontal"",
        ""Type"": ""Grid"",
        ""Attributes"": {
            ""HorizontalOptions"": ""FillAndExpand"",
            ""VerticalOptions"": ""Start"",
            ""MinimumHeightRequest"": ""70"",
            ""BackgroundColor"": ""Black""
        },
        ""Children"": [
            {
                ""Name"": ""TopLeft"",
                ""Type"": ""StackLayout"",
                ""Orientation"": ""Vertical"",
                ""Row"": 0,
                ""Column"": 0,
                ""Attributes"": {}
            },
            {
                ""Name"": ""TopCenter"",
                ""Type"": ""StackLayout"",
                ""Orientation"": ""Vertical"",
                ""Row"": 0,
                ""Column"": 1,
                ""Attributes"": {}
            },
            {
                ""Name"": ""TopRight"",
                ""Type"": ""StackLayout"",
                ""Orientation"": ""Vertical"",
                ""Row"": 0,
                ""Column"": 2,
                ""Attributes"": {}
            }
        ]
    },
    {
        ""Name"": ""Featured"",
        ""Type"": ""StackLayout"",
        ""Orientation"": ""Vertical"",
        ""Attributes"": {
            ""VerticalOptions"": ""Start""
        }
    },
    {
        ""Name"": ""Max"",
        ""Type"": ""StackLayout"",
        ""Orientation"": ""Vertical"",
        ""Attributes"": {
            ""VerticalOptions"": ""FillAndExpand""
        },
        ""Children"": [
            {
                ""Name"": ""Container"",
                ""Type"": ""StackLayout"",
                ""Orientation"": ""Vertical"",
                ""ScrollY"": true,
                ""Attributes"": {
                    ""VerticalOptions"": ""FillAndExpand""
                },
                ""Children"": [
                    {
                        ""Name"": ""MainBackground"",
                        ""Type"": ""StackLayout"",
                        ""Orientation"": ""Vertical""
                    },
                    {
                        ""Name"": ""Main"",
                        ""Type"": ""StackLayout"",
                        ""Orientation"": ""Vertical""
                    }
                ]
            }
        ]
    }
]";
            RockMigrationHelper.UpdateDefinedValue( AvalancheUtilities.LayoutsDefinedType, "Main Page", "MainPage layout to have some different options, came from SECC", "a28546a5-54d8-4a2e-b912-af9f5e2f0aa2", false );
            RockMigrationHelper.UpdateDefinedValueAttributeValue( "a28546a5-54d8-4a2e-b912-af9f5e2f0aa2", "E5DE699C-49F6-488B-BA1F-F4CC13CE8B91", mainpage );

            var footer = @"[ { ""Name"": ""FooterHorizontal"", ""Type"": ""Grid"", ""RowDefinitions"": ""100"", ""Children"": [ { ""Name"": ""Footer"", ""Type"": ""StackLayout"", ""Orientation"": ""Vertical"", ""Attributes"": { ""VerticalOptions"": ""End"" } } ] } ]";
            RockMigrationHelper.UpdateDefinedValue( AvalancheUtilities.LayoutsDefinedType, "Footer", "Footer layout fairly simple", "7f0d651b-6acc-4207-a985-6d9deb81e71d", false );
            RockMigrationHelper.UpdateDefinedValueAttributeValue( "7f0d651b-6acc-4207-a985-6d9deb81e71d", "E5DE699C-49F6-488B-BA1F-F4CC13CE8B91", footer );

            RockMigrationHelper.AddPage( true, null, "FC61CD1A-15DC-4FDD-9DDD-4A0BD8936E16", "KFS Avalanche Home Page", "", "567FFD63-53F9-4419-AD96-C2F07CAE09F1", "" ); // Site:KFS Avalanche
            Sql( string.Format( @"
                DECLARE @PageId int = ( SELECT TOP 1 [Id] FROM [Page] WHERE [Guid] = '{0}' )
                UPDATE Site SET DefaultPageId = @PageId WHERE [Guid] = '{1}'
                ", "567FFD63-53F9-4419-AD96-C2F07CAE09F1", "613631FF-D19C-4F9C-B163-E9331C4BA61B" ) );
        }

        /// <summary>
        /// The commands to undo a migration from a specific version
        /// </summary>
        public override void Down()
        {
            //
            // remove layouts and site
            //
            RockMigrationHelper.DeletePage( "567FFD63-53F9-4419-AD96-C2F07CAE09F1" );
            RockMigrationHelper.DeleteLayout( "355F6C23-29B3-4976-AE43-30426BE12B99" ); //  Layout: Simple, Site: KFS Avalanche
            RockMigrationHelper.DeleteLayout( "901926F9-AD81-41A4-9B1E-254F5B45E471" ); //  Layout: No Scroll, Site: KFS Avalanche
            RockMigrationHelper.DeleteLayout( "FC61CD1A-15DC-4FDD-9DDD-4A0BD8936E16" ); //  Layout: Main Page, Site: KFS Avalanche
            RockMigrationHelper.DeleteLayout( "60D99A36-8D00-467E-9993-3C2F0B249EBD" ); //  Layout: Footer, Site: KFS Avalanche
            RockMigrationHelper.DeleteLayout( "B510AB94-04B0-48C0-BEDD-16812BEDECB1" ); //  Layout: Boxes, Site: KFS Avalanche
            RockMigrationHelper.DeleteSite( "613631FF-D19C-4F9C-B163-E9331C4BA61B" );
        }
    }
}
