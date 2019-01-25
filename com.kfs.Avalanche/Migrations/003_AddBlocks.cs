using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rock.Plugin;

namespace com.kfs.Avalanche.Migrations
{
    [MigrationNumber( 3, "1.8.0" )]
    public class AddBlocks : Migration
    {
        /// <summary>
        /// The commands to run to migrate plugin to the specific version
        /// </summary>
        public override void Up()
        {
            RockMigrationHelper.UpdateBlockType( "Audio Player Block", "Mobile audio player.", "~/Plugins/com_kfs/Avalanche/AudioPlayerBlock.ascx", "Avalanche", "7515DA74-D55F-44C0-ABC0-A6D9DBFEF76B" );
            RockMigrationHelper.UpdateBlockType( "Avalanche Configuration", "Configuration settings for Avalanche.", "~/Plugins/com_kfs/Avalanche/AvalancheConfiguration.ascx", "Avalanche > Settings", "2E5AAAEB-C731-4F7A-B502-BA67589AF69F" );
            RockMigrationHelper.UpdateBlockType( "Button", "A button.", "~/Plugins/com_kfs/Avalanche/ButtonBlock.ascx", "Avalanche", "65CE4075-6A52-40F4-8ED0-6540F387BE76" );
            RockMigrationHelper.UpdateBlockType( "Avalanche Event Calendar Lava", "Renders a particular calendar using Lava.", "~/Plugins/com_kfs/Avalanche/CalendarLava.ascx", "Avalanche", "A5129AF8-9744-4974-9EC3-F5D898DC7B77" );
            RockMigrationHelper.UpdateBlockType( "Content Channel Mobile List", "Block to display a list of content itmes.", "~/Plugins/com_kfs/Avalanche/ContentChannelMobileList.ascx", "Avalanche", "641A5E85-F2B4-46A3-8B9F-4AA759E33804" );
            RockMigrationHelper.UpdateBlockType( "Group Attendance Block", "Mobile block to take group attendance.", "~/Plugins/com_kfs/Avalanche/GroupAttendanceBlock.ascx", "Avalanche", "06A2663C-9292-4202-ACEE-A5F884671E55" );
            RockMigrationHelper.UpdateBlockType( "Group List", "Block to show a list of groups.", "~/Plugins/com_kfs/Avalanche/GroupListBlock.ascx", "Avalanche", "55CB76A9-74F8-4F31-97B5-2567EBB25DFF" );
            RockMigrationHelper.UpdateBlockType( "Group Member List Block", "Mobile block to show group members of a group.", "~/Plugins/com_kfs/Avalanche/GroupMemberListBlock.ascx", "Avalanche", "E6BAE8D3-B244-400F-8092-AFD27156F6BD" );
            RockMigrationHelper.UpdateBlockType( "Icon Block", "Font Awesome Icon.", "~/Plugins/com_kfs/Avalanche/IconBlock.ascx", "Avalanche", "1112BA70-CE0D-4158-8A2B-D5F2FD217BAE" );
            RockMigrationHelper.UpdateBlockType( "Icon Button", "An icon button", "~/Plugins/com_kfs/Avalanche/IconButton.ascx", "Avalanche", "4CCA5B0C-63A6-40C1-B981-648663029092" );
            RockMigrationHelper.UpdateBlockType( "Image Block", "A button.", "~/Plugins/com_kfs/Avalanche/ImageBlock.ascx", "Avalanche", "47C24453-10F9-4A11-9BD0-3D9B1CC943D3" );
            RockMigrationHelper.UpdateBlockType( "Label Block", "A button.", "~/Plugins/com_kfs/Avalanche/LabelBlock.ascx", "Avalanche", "A42B3143-E970-4BEC-A694-9BCB37B9B737" );
            RockMigrationHelper.UpdateBlockType( "Login App", "Login Screen", "~/Plugins/com_kfs/Avalanche/Login.ascx", "Avalanche", "857ABAF2-1F35-404E-827D-F4ADD629CBDF" );
            RockMigrationHelper.UpdateBlockType( "Markdown Detail", "A control to display Markdown.", "~/Plugins/com_kfs/Avalanche/MarkdownDetail.ascx", "Avalanche", "35B0B2B4-BE73-4C03-8F56-DD8D28EBD7FA" );
            RockMigrationHelper.UpdateBlockType( "Mobile Content Item", "Block to show a mobile content item.", "~/Plugins/com_kfs/Avalanche/MobileContentItem.ascx", "Avalanche", "C2E57F26-0F77-4844-A298-1A24B088D645" );
            RockMigrationHelper.UpdateBlockType( "Mobile ListView Lava", "Displays mobile list view from lava", "~/Plugins/com_kfs/Avalanche/MobileListViewLava.ascx", "Avalanche", "755D550E-FB64-4DBE-A054-9D0141A18001" );
            RockMigrationHelper.UpdateBlockType( "Mobile Workflow", "Mobile block to allow workflow to be accessed from app", "~/Plugins/com_kfs/Avalanche/MobileWorkflow.ascx", "Avalanche", "6B9DDC12-D7B3-4521-9D49-B79BE6578CB1" );
            RockMigrationHelper.UpdateBlockType( "Note Block", "Allows the user to make a single personal note for an entity. Useful for sermons and blog posts.", "~/Plugins/com_kfs/Avalanche/NoteBlock.ascx", "Avalanche", "275D5C9B-7D0B-42EE-A0D1-198B5C1A8A3B" );
            RockMigrationHelper.UpdateBlockType( "Person Card", "Card to display person's information from guid.", "~/Plugins/com_kfs/Avalanche/PersonCard.ascx", "Avalanche", "9610B4D8-EB3B-45ED-B8DF-9E97FA1DF7AF" );
            RockMigrationHelper.UpdateBlockType( "Phone Number Login", "Block to log in with your phone number", "~/Plugins/com_kfs/Avalanche/PhoneNumberLogin.ascx", "Avalanche", "912A2617-C744-4D65-A0E1-A795469CFD0D" );
            RockMigrationHelper.UpdateBlockType( "Prayer Request Entry", "Block to add prayer requests to the prayer system via mobile.", "~/Plugins/com_kfs/Avalanche/PrayerRequestEntry.ascx", "Avalanche", "8189E73E-7E5C-4D37-BD62-FD7FBDEEC9FD" );
            RockMigrationHelper.UpdateBlockType( "Preload Block", "Block to preload pages in Avalanche.", "~/Plugins/com_kfs/Avalanche/PreloadBlock.ascx", "Avalanche", "01CFA67B-429A-432E-BA56-4F4289917D06" );
            RockMigrationHelper.UpdateBlockType( "Text Over Image Block", "Creates an image with text centered over it.", "~/Plugins/com_kfs/Avalanche/TextOverImage.ascx", "Avalanche", "207ABAA7-8C45-472E-8649-3E4E895184B7" );
            RockMigrationHelper.UpdateBlockType( "Video Player Block", "Mobile video player.", "~/Plugins/com_kfs/Avalanche/VideoPlayerBlock.ascx", "Avalanche", "6FA66F35-5D80-4BD5-AD39-71618C1FFEAD" );
            RockMigrationHelper.UpdateBlockType( "WebViewBlock", "A control to display Markdown.", "~/Plugins/com_kfs/Avalanche/WebViewBlock.ascx", "Avalanche", "78FC6291-B753-4782-8AED-DB04681F1D0E" );
            RockMigrationHelper.UpdateBlockType( "Foreign Objects", "This block displays Foreign Objects (Key, Guid, & Id) and allows for a Lava formatted output. Currently Supports; Person, FinancialAccount, FinancialBatch, FinancialPledge, FinancialTransaction, Group, GroupMember, Metric", "~/Plugins/KFS/ForeignObjects.ascx", "Utility", "6AE6B96C-689F-434B-AC3A-683DA598D07C" );
            RockMigrationHelper.UpdateBlockType( "Event Filter", "Creates a form for event filtering ", "~/Plugins/com_kfs/Avalanche/EventFilter.ascx", "Avalanche", "DE3997BC-AE8E-43B0-A41B-1F52C1BFE3B0" );
            RockMigrationHelper.UpdateBlockType( "Public Profile Edit Block", "Allows the user to update their personal information", "~/Plugins/com_kfs/Avalanche/PersonProfileEdit.ascx", "Avalanche", "41D83AAD-F2B8-43DA-978E-1391B4427280" );
            RockMigrationHelper.UpdateBlockType( "Person Profile Family", "Block to show a list of Family members.", "~/Plugins/com_kfs/Avalanche/PersonProfileFamily.ascx", "Avalanche", "36569642-F6D7-4EAC-AF0E-C8238BEEAF7E" );
            // Add Block to Layout: No Scroll Site: Avalanche
            RockMigrationHelper.AddBlock( true, null, "901926F9-AD81-41A4-9B1E-254F5B45E471", "1112BA70-CE0D-4158-8A2B-D5F2FD217BAE", "Icon Block", "TopLeft", @"", @"", 0, "962317D3-42D0-4152-839E-35AAF2BD66E2" );
            // Add Block to Layout: Simple Site: Avalanche
            RockMigrationHelper.AddBlock( true, null, "355F6C23-29B3-4976-AE43-30426BE12B99", "1112BA70-CE0D-4158-8A2B-D5F2FD217BAE", "Icon Block - Back", "TopLeft", @"", @"", 0, "D2DFBCB4-894C-4E5C-A534-110B21A00FEB" );
            //// Add Block to  Site: Avalanche (v9?)
            //RockMigrationHelper.AddBlock( true, null, null, "613631FF-D19C-4F9C-B163-E9331C4BA61B", "47C24453-10F9-4A11-9BD0-3D9B1CC943D3", "Logo Image", "TopCenter", @"", @"", 0, "BD1F22F8-783B-4B87-AEB6-9B6AFE9A8938" );
            // Add Block to Layout: Main Page Site: Avalanche
            RockMigrationHelper.AddBlock( true, null, "FC61CD1A-15DC-4FDD-9DDD-4A0BD8936E16", "47C24453-10F9-4A11-9BD0-3D9B1CC943D3", "Logo Image", "TopCenter", @"", @"", 0, "0EDCFD24-D064-49D7-8004-A5A74BF94F9C" );
            // Add Block to Layout: No Scroll Site: Avalanche
            RockMigrationHelper.AddBlock( true, null, "901926F9-AD81-41A4-9B1E-254F5B45E471", "47C24453-10F9-4A11-9BD0-3D9B1CC943D3", "Logo Image", "TopCenter", @"", @"", 0, "BD1F22F8-783B-4B87-AEB6-9B6AFE9A8938" );
            // Add Block to Layout: Simple Site: Avalanche
            RockMigrationHelper.AddBlock( true, null, "355F6C23-29B3-4976-AE43-30426BE12B99", "47C24453-10F9-4A11-9BD0-3D9B1CC943D3", "Logo Image", "TopCenter", @"", @"", 0, "AB5D577E-92B4-49F8-AA50-9ADDA515F881" );
            // Add Block to Page: Avalanche Home Page Site: Avalanche
            RockMigrationHelper.AddBlock( true, "567FFD63-53F9-4419-AD96-C2F07CAE09F1", null, "01CFA67B-429A-432E-BA56-4F4289917D06", "Preload Block", "Main", @"", @"", 0, "FCB9F034-0036-48DC-B588-6FBE969938C9" );
            // Add Block to Page: Avalanche Home Page Site: Avalanche
            RockMigrationHelper.AddBlock( true, "567FFD63-53F9-4419-AD96-C2F07CAE09F1", null, "207ABAA7-8C45-472E-8649-3E4E895184B7", "Text Over Image Block - Events", "Main", @"", @"", 2, "7E1A52B6-68A9-423F-88DB-4E402360FED0" );
            // Add Block to Page: Avalanche Home Page Site: Avalanche
            RockMigrationHelper.AddBlock( true, "567FFD63-53F9-4419-AD96-C2F07CAE09F1", null, "207ABAA7-8C45-472E-8649-3E4E895184B7", "Text Over Image Block - Sermons", "Main", @"", @"", 1, "A2487BA5-843D-4D7E-92A6-36A52027EB26" );
            // Add Block to Page: Avalanche Home Page Site: Avalanche
            RockMigrationHelper.AddBlock( true, "567FFD63-53F9-4419-AD96-C2F07CAE09F1", null, "207ABAA7-8C45-472E-8649-3E4E895184B7", "Text Over Image Block - Visit", "Main", @"", @"", 3, "99865F0D-BD8A-4AA7-88A2-D7D7859E1992" );
            // Add Block to Page: Sermons Site: Avalanche
            RockMigrationHelper.AddBlock( true, "9711DB54-0FB0-4722-AB45-1DFB6158F922", null, "641A5E85-F2B4-46A3-8B9F-4AA759E33804", "Content Channel Mobile List", "Main", @"", @"", 0, "724E2C2F-213B-4959-9F01-1957C77733DD" );
            // Add Block to Page: Visit Site: Avalanche
            RockMigrationHelper.AddBlock( true, "CD8A05F8-24FF-4D38-8ED0-FE2BF07C0CDE", null, "207ABAA7-8C45-472E-8649-3E4E895184B7", "Text Over Image Block - Plan a visit", "Main", @"", @"", 0, "C730CAF4-F17F-4D9C-B2F3-9A6A29491FD7" );
            // Add Block to Page: Visit Site: Avalanche
            RockMigrationHelper.AddBlock( true, "CD8A05F8-24FF-4D38-8ED0-FE2BF07C0CDE", null, "A42B3143-E970-4BEC-A694-9BCB37B9B737", "Label Block - Welcome", "Main", @"", @"", 1, "57096CBB-21E8-4EF5-ABD3-05B2414225D0" );
            // Add Block to Page: Visit Site: Avalanche
            RockMigrationHelper.AddBlock( true, "CD8A05F8-24FF-4D38-8ED0-FE2BF07C0CDE", null, "35B0B2B4-BE73-4C03-8F56-DD8D28EBD7FA", "Welcome Text", "Main", @"", @"", 2, "0B08E530-6763-473F-A338-C7E2B2F753BA" );
            // Add Block to Page: Visit Site: Avalanche
            RockMigrationHelper.AddBlock( true, "CD8A05F8-24FF-4D38-8ED0-FE2BF07C0CDE", null, "755D550E-FB64-4DBE-A054-9D0141A18001", "Campus Listview", "Main", @"", @"", 3, "14EFA755-D8F8-4BC2-A199-E67B2E960A95" );
            // Add Block to Page: Connect Site: Avalanche
            RockMigrationHelper.AddBlock( true, "DE6D125E-892E-4F10-A33D-F84942582B1E", null, "207ABAA7-8C45-472E-8649-3E4E895184B7", "Text Over Image Block - Events", "Main", @"", @"", 3, "E7E0ADD1-0863-492E-B7D7-A32B5680816F" );
            // Add Block to Page: Connect Site: Avalanche
            RockMigrationHelper.AddBlock( true, "DE6D125E-892E-4F10-A33D-F84942582B1E", null, "207ABAA7-8C45-472E-8649-3E4E895184B7", "Text Over Image Block - Baptism", "Main", @"", @"", 2, "DC43E7F1-A294-4D99-B37A-CC04F18BD32C" );
            // Add Block to Page: Connect Site: Avalanche
            RockMigrationHelper.AddBlock( true, "DE6D125E-892E-4F10-A33D-F84942582B1E", null, "207ABAA7-8C45-472E-8649-3E4E895184B7", "Text Over Image Block - Serve", "Main", @"", @"", 4, "2901482E-A8C5-4A47-8EEB-2D3C9E0EA407" );
            // Add Block to Page: Connect Site: Avalanche
            RockMigrationHelper.AddBlock( true, "DE6D125E-892E-4F10-A33D-F84942582B1E", null, "207ABAA7-8C45-472E-8649-3E4E895184B7", "Text Over Image Block - Groups", "Main", @"", @"", 1, "44D7ED68-22F4-4D1F-B443-75FF6B9F791B" );
            // Add Block to Page: Connect Site: Avalanche
            RockMigrationHelper.AddBlock( true, "DE6D125E-892E-4F10-A33D-F84942582B1E", null, "01CFA67B-429A-432E-BA56-4F4289917D06", "Preload Block", "Main", @"", @"", 0, "C6A448AB-B39B-48D1-9EDC-06062BF98766" );
            // Add Block to Page: Connect Site: Avalanche
            RockMigrationHelper.AddBlock( true, "DE6D125E-892E-4F10-A33D-F84942582B1E", null, "65CE4075-6A52-40F4-8ED0-6540F387BE76", "Button", "Main", @"", @"", 5, "99CAEBAF-9E80-410E-BB33-9DCD6B4ECF9B" );
            // Add Block to Page: Give Site: Avalanche
            RockMigrationHelper.AddBlock( true, "78EDFA36-FEC9-4D74-A34B-07C76A4FC071", null, "78FC6291-B753-4782-8AED-DB04681F1D0E", "WebViewBlock", "Main", @"", @"", 0, "FAF7BF20-4F53-47CC-B7E7-A5223BED5188" );
            // Add Block to Page: Footer Site: Avalanche
            RockMigrationHelper.AddBlock( true, "FF495C30-29C5-420C-A35B-E9E808EEBCEF", null, "755D550E-FB64-4DBE-A054-9D0141A18001", "Listview Footer", "Footer", @"", @"", 1, "0DBA9AAB-FA2E-4C53-B820-A4C0B3FF29D0" );
            // Add Block to Page: Visit Detail Site: Avalanche
            RockMigrationHelper.AddBlock( true, "5F1E1759-0A3A-4A57-896B-7B8B8BD892B3", null, "35B0B2B4-BE73-4C03-8F56-DD8D28EBD7FA", "Description/Location summary text", "Main", @"", @"", 2, "BDAAFA41-836E-4C27-9696-847D685D3980" );
            // Add Block to Page: Visit Detail Site: Avalanche
            RockMigrationHelper.AddBlock( true, "5F1E1759-0A3A-4A57-896B-7B8B8BD892B3", null, "4CCA5B0C-63A6-40C1-B981-648663029092", "More Info Button", "Main", @"", @"", 4, "20E578B0-BAB2-40DA-944D-9189465C6A96" );
            // Add Block to Page: Visit Detail Site: Avalanche
            RockMigrationHelper.AddBlock( true, "5F1E1759-0A3A-4A57-896B-7B8B8BD892B3", null, "A42B3143-E970-4BEC-A694-9BCB37B9B737", "Header Title", "Main", @"", @"", 1, "089C9B48-AE89-416C-98FB-32304206DB20" );
            // Add Block to Page: Visit Detail Site: Avalanche
            RockMigrationHelper.AddBlock( true, "5F1E1759-0A3A-4A57-896B-7B8B8BD892B3", null, "4CCA5B0C-63A6-40C1-B981-648663029092", "Parking Map Button", "Main", @"", @"", 3, "41BD57CC-71E3-4DC0-957C-F9265061FA65" );
            // Add Block to Page: Visit Detail Site: Avalanche
            RockMigrationHelper.AddBlock( true, "5F1E1759-0A3A-4A57-896B-7B8B8BD892B3", null, "47C24453-10F9-4A11-9BD0-3D9B1CC943D3", "Image Block", "Main", @"", @"", 0, "941F3F7B-2922-4D29-9B8E-FE22A5337486" );
            // Add Block to Page: Series Detail Site: Avalanche
            RockMigrationHelper.AddBlock( true, "4F8A1FFA-05A7-4522-8C23-74468FAFD6BC", null, "47C24453-10F9-4A11-9BD0-3D9B1CC943D3", "Image Block", "Main", @"", @"", 0, "A47A5177-CD5B-4A1F-9F6D-C1718DB0BA16" );
            // Add Block to Page: Series Detail Site: Avalanche
            RockMigrationHelper.AddBlock( true, "4F8A1FFA-05A7-4522-8C23-74468FAFD6BC", null, "755D550E-FB64-4DBE-A054-9D0141A18001", "Mobile ListView Lava - Sermon Content", "Main", @"", @"", 2, "7AA9738F-BA87-4761-9FD3-FF8A38703473" );
            // Add Block to Page: Series Detail Site: Avalanche
            RockMigrationHelper.AddBlock( true, "4F8A1FFA-05A7-4522-8C23-74468FAFD6BC", null, "35B0B2B4-BE73-4C03-8F56-DD8D28EBD7FA", "Markdown Detail", "Main", @"", @"", 1, "EDC51B07-CA23-4415-8997-E03A60D5FA5A" );
            // Add Block to Page: Sermon Detail Site: Avalanche
            RockMigrationHelper.AddBlock( true, "1CDE2032-482B-483E-B273-A1A3B421E04C", null, "35B0B2B4-BE73-4C03-8F56-DD8D28EBD7FA", "Markdown Detail", "Main", @"", @"", 1, "11955742-2F20-40BB-B304-7865C992C7B4" );
            // Add Block to Page: Sermon Detail Site: Avalanche
            RockMigrationHelper.AddBlock( true, "1CDE2032-482B-483E-B273-A1A3B421E04C", null, "4CCA5B0C-63A6-40C1-B981-648663029092", "Icon Button - Listen", "Main", @"", @"", 2, "BA50E899-CEAB-4816-8AD0-A650889882A4" );
            // Add Block to Page: Sermon Detail Site: Avalanche
            RockMigrationHelper.AddBlock( true, "1CDE2032-482B-483E-B273-A1A3B421E04C", null, "6FA66F35-5D80-4BD5-AD39-71618C1FFEAD", "Video Player Block", "Main", @"", @"", 0, "7E110550-159D-463A-A532-673564806896" );
            // Add Block to Page: Sermon Detail Audio Site: Avalanche
            RockMigrationHelper.AddBlock( true, "344BABC7-5C30-4B53-87F9-A00F0DDA38E7", null, "4CCA5B0C-63A6-40C1-B981-648663029092", "Icon Button - Watch", "Main", @"", @"", 3, "F915EB5A-9260-4681-9A49-0C6778D2F084" );
            // Add Block to Page: Sermon Detail Audio Site: Avalanche
            RockMigrationHelper.AddBlock( true, "344BABC7-5C30-4B53-87F9-A00F0DDA38E7", null, "47C24453-10F9-4A11-9BD0-3D9B1CC943D3", "Image Block", "Main", @"", @"", 0, "9735B56B-32A3-45CE-9298-5C1F7EFB3DEA" );
            // Add Block to Page: Sermon Detail Audio Site: Avalanche
            RockMigrationHelper.AddBlock( true, "344BABC7-5C30-4B53-87F9-A00F0DDA38E7", null, "7515DA74-D55F-44C0-ABC0-A6D9DBFEF76B", "Audio Player Block", "Main", @"", @"", 1, "7761BC7C-DDE7-4DCB-81C9-720339A24477" );
            // Add Block to Page: Sermon Detail Audio Site: Avalanche
            RockMigrationHelper.AddBlock( true, "344BABC7-5C30-4B53-87F9-A00F0DDA38E7", null, "35B0B2B4-BE73-4C03-8F56-DD8D28EBD7FA", "Markdown Detail", "Main", @"", @"", 2, "8F975583-92F5-4827-909B-97D3D09E4BEC" );
            // Add Block to Page: Groups Site: Avalanche
            RockMigrationHelper.AddBlock( true, "D943417B-F167-4CD0-8321-C779B1C9E92B", null, "35B0B2B4-BE73-4C03-8F56-DD8D28EBD7FA", "Markdown Detail", "Main", @"", @"", 1, "898CD5D8-AB1E-4267-8084-161FFBAA7820" );
            // Add Block to Page: Groups Site: Avalanche
            RockMigrationHelper.AddBlock( true, "D943417B-F167-4CD0-8321-C779B1C9E92B", null, "35B0B2B4-BE73-4C03-8F56-DD8D28EBD7FA", "Next Steps detail", "Main", @"", @"", 3, "87B3FD61-73E7-4BEF-AC56-1A96E497E916" );
            // Add Block to Page: Groups Site: Avalanche
            RockMigrationHelper.AddBlock( true, "D943417B-F167-4CD0-8321-C779B1C9E92B", null, "207ABAA7-8C45-472E-8649-3E4E895184B7", "Text Over Image Block", "Main", @"", @"", 0, "5C6A4458-56A3-4F60-8792-5271326DED00" );
            // Add Block to Page: Groups Site: Avalanche
            RockMigrationHelper.AddBlock( true, "D943417B-F167-4CD0-8321-C779B1C9E92B", null, "65CE4075-6A52-40F4-8ED0-6540F387BE76", "Button - Attend Growth Track", "Main", @"", @"", 2, "6B2D9BCB-6229-4859-BA77-9E4BF34AE181" );
            // Add Block to Page: Groups Site: Avalanche
            RockMigrationHelper.AddBlock( true, "D943417B-F167-4CD0-8321-C779B1C9E92B", null, "65CE4075-6A52-40F4-8ED0-6540F387BE76", "Next Steps Button", "Main", @"", @"", 4, "D0B758C4-18E1-45DE-82B0-70906E45B99A" );
            // Add Block to Page: Baptism Site: Avalanche
            RockMigrationHelper.AddBlock( true, "8E94B8E8-171D-4A81-9A69-D34667510231", null, "65CE4075-6A52-40F4-8ED0-6540F387BE76", "Button", "Main", @"", @"", 2, "9B44C218-D744-43AC-B4A0-9DBD7C78911D" );
            // Add Block to Page: Baptism Site: Avalanche
            RockMigrationHelper.AddBlock( true, "8E94B8E8-171D-4A81-9A69-D34667510231", null, "207ABAA7-8C45-472E-8649-3E4E895184B7", "Text Over Image Block", "Main", @"", @"", 0, "F6B935B9-6DA2-421A-9645-F9EF7B432E8C" );
            // Add Block to Page: Baptism Site: Avalanche
            RockMigrationHelper.AddBlock( true, "8E94B8E8-171D-4A81-9A69-D34667510231", null, "35B0B2B4-BE73-4C03-8F56-DD8D28EBD7FA", "Markdown Detail", "Main", @"", @"", 1, "010B3F1F-2149-4DBA-ABB3-1DCF795C4C89" );
            // Add Block to Page: Baptism Site: Avalanche
            RockMigrationHelper.AddBlock( true, "8E94B8E8-171D-4A81-9A69-D34667510231", null, "35B0B2B4-BE73-4C03-8F56-DD8D28EBD7FA", "Markdown Detail", "Main", @"", @"", 3, "6AA0CB59-E4BA-4E74-8198-749EEB553835" );
            // Add Block to Page: Events Site: Avalanche
            RockMigrationHelper.AddBlock( true, "B95062A4-770C-47DF-B88F-0626F7BFDF2F", null, "207ABAA7-8C45-472E-8649-3E4E895184B7", "Text Over Image Block", "Main", @"", @"", 0, "FB586E92-764C-4321-871F-47C38D768E94" );
            // Add Block to Page: Events Site: Avalanche
            RockMigrationHelper.AddBlock( true, "B95062A4-770C-47DF-B88F-0626F7BFDF2F", null, "A5129AF8-9744-4974-9EC3-F5D898DC7B77", "Avalanche Event Calendar Lava", "Main", @"", @"", 2, "713083B7-B5A8-416C-9603-594589A67B8D" );
            // Add Block to Page: Events Site: Avalanche
            RockMigrationHelper.AddBlock( true, "B95062A4-770C-47DF-B88F-0626F7BFDF2F", null, "DE3997BC-AE8E-43B0-A41B-1F52C1BFE3B0", "Event Filter", "Main", @"", @"", 1, "A9D36E34-4D02-4179-B6DE-EF30B341D1D9" );
            // Add Block to Page: Contact Us Site: Avalanche
            RockMigrationHelper.AddBlock( true, "7817298D-0A76-4039-A5D7-AF273AC05952", null, "6B9DDC12-D7B3-4521-9D49-B79BE6578CB1", "Mobile Workflow", "Main", @"", @"", 0, "F05B2A93-CEF8-4EAE-9358-EACC73C31173" );
            // Add Block to Page: Event Detail Site: Avalanche
            RockMigrationHelper.AddBlock( true, "14A25533-186B-49C4-A949-0F9DA864A87E", null, "35B0B2B4-BE73-4C03-8F56-DD8D28EBD7FA", "Markdown Detail", "Main", @"", @"", 2, "10F85187-7796-45FC-A65D-4EC1314E3609" );
            // Add Block to Page: Event Detail Site: Avalanche
            RockMigrationHelper.AddBlock( true, "14A25533-186B-49C4-A949-0F9DA864A87E", null, "65CE4075-6A52-40F4-8ED0-6540F387BE76", "Mobile Button", "Main", @"", @"", 3, "C454F889-AB57-4D38-A18F-195D60445EDF" );
            // Add Block to Page: Event Detail Site: Avalanche
            RockMigrationHelper.AddBlock( true, "14A25533-186B-49C4-A949-0F9DA864A87E", null, "A42B3143-E970-4BEC-A694-9BCB37B9B737", "Label Block", "Main", @"", @"", 1, "897FE435-F9E8-4755-AC87-5B273A415DCD" );
            // Add Block to Page: Event Detail Site: Avalanche
            RockMigrationHelper.AddBlock( true, "14A25533-186B-49C4-A949-0F9DA864A87E", null, "47C24453-10F9-4A11-9BD0-3D9B1CC943D3", "Image Block", "Main", @"", @"", 0, "BAAA075C-AD3F-4A0B-AF0C-D9462760D16F" );
            // Add Block to Page: Serve Site: Avalanche
            RockMigrationHelper.AddBlock( true, "C90B1E1B-EC24-49E0-9562-7F92BB2D24AB", null, "755D550E-FB64-4DBE-A054-9D0141A18001", "Mobile ListView Lava", "Main", @"", @"", 2, "77E64C5A-7F2A-4634-BEA4-46ECBAF4E56D" );
            // Add Block to Page: Serve Site: Avalanche
            RockMigrationHelper.AddBlock( true, "C90B1E1B-EC24-49E0-9562-7F92BB2D24AB", null, "35B0B2B4-BE73-4C03-8F56-DD8D28EBD7FA", "Markdown Detail", "Main", @"", @"", 1, "897EB15F-66BB-44F6-BF89-C9012CA36BCA" );
            // Add Block to Page: Serve Site: Avalanche
            RockMigrationHelper.AddBlock( true, "C90B1E1B-EC24-49E0-9562-7F92BB2D24AB", null, "207ABAA7-8C45-472E-8649-3E4E895184B7", "Text Over Image Block", "Main", @"", @"", 0, "454646F6-C317-4764-B514-4AF84C1E7FEC" );
            // Add Block to Page: Other Blocks Site: Avalanche
            RockMigrationHelper.AddBlock( true, "D4A5C0AB-0A90-416F-9B3F-C0ACFED92D43", null, "755D550E-FB64-4DBE-A054-9D0141A18001", "Mobile ListView Lava - Child Pages", "Main", @"", @"", 0, "253974FD-3F00-4677-86A4-F4F8C84F275C" );
            // Add Block to Page: Login Site: Avalanche
            RockMigrationHelper.AddBlock( true, "F53E2612-3E8B-4D26-87ED-468D4C9C02E6", null, "857ABAF2-1F35-404E-827D-F4ADD629CBDF", "Login App", "Main", @"", @"", 0, "0DB6C226-836D-4CEB-B3CC-9F50ED499DD5" );
            // Add Block to Page: Prayer Request Site: Avalanche
            RockMigrationHelper.AddBlock( true, "BB61CCD2-9E50-45A3-9D94-A37F3D7313FF", null, "8189E73E-7E5C-4D37-BD62-FD7FBDEEC9FD", "Prayer Request Entry", "Main", @"", @"", 0, "BFB73CDC-B2B6-4484-B35F-3BC25C430ECB" );
            // Add Block to Page: Person Card Site: Avalanche
            RockMigrationHelper.AddBlock( true, "79EF0279-9800-4BAF-A8D9-DD42E6868BAA", null, "9610B4D8-EB3B-45ED-B8DF-9E97FA1DF7AF", "Person Card", "Main", @"", @"", 0, "86FF3353-4FEE-4272-BD9B-78B5C373051A" );
            // Add Block to Page: Group Attendance Site: Avalanche
            RockMigrationHelper.AddBlock( true, "2FB9C8A1-B3E6-4608-8CE7-411AA27945BA", null, "06A2663C-9292-4202-ACEE-A5F884671E55", "Group Attendance Block", "Main", @"", @"", 0, "BD824150-F1B1-4A7D-93FA-5A4AA7AE565D" );
            // Add Block to Page: Group List Site: Avalanche
            RockMigrationHelper.AddBlock( true, "0EC21B17-EA03-450E-A0B4-594C09340458", null, "55CB76A9-74F8-4F31-97B5-2567EBB25DFF", "Group List", "Main", @"", @"", 0, "13472A3E-A5FD-47C1-A382-4243BBE0BC98" );
            // Add Block to Page: Group Member List Site: Avalanche
            RockMigrationHelper.AddBlock( true, "C4A65B7E-D57C-44C9-93AD-9E2E51145481", null, "E6BAE8D3-B244-400F-8092-AFD27156F6BD", "Group Member List Block", "Main", @"", @"", 0, "D53AE8B1-9F7B-457C-A2AB-297D7E0B0C71" );
            // Add Block to Page: Note Block Site: Avalanche
            RockMigrationHelper.AddBlock( true, "F522A717-2D7A-405B-87BB-CCB16C15BB44", null, "275D5C9B-7D0B-42EE-A0D1-198B5C1A8A3B", "Note Block", "Main", @"", @"", 0, "FB213CB9-C91F-4546-9371-73839FBD447A" );
            // Add Block to Page: Webview Block Site: Avalanche
            RockMigrationHelper.AddBlock( true, "0FBC5CE7-CF91-45EC-B28A-A5E486764B9A", null, "78FC6291-B753-4782-8AED-DB04681F1D0E", "WebViewBlock", "Main", @"", @"", 0, "E43CF3E1-D8FA-41B3-AED9-E09FAA594F7B" );
            // update block order for pages with new blocks if the page,zone has multiple blocks
            Sql( @"UPDATE [Block] SET [Order] = 0 WHERE [Guid] = '454646F6-C317-4764-B514-4AF84C1E7FEC'" );  // Page: Serve,  Zone: Main,  Block: Text Over Image Block
            //Sql( @"UPDATE [Block] SET [Order] = 0 WHERE [Guid] = '5B91008B-7B07-43FB-A5C0-F44E0EADF9D7'" );  // Page: Note Editor,  Zone: Main,  Block: Person Context Setter
            Sql( @"UPDATE [Block] SET [Order] = 0 WHERE [Guid] = '5C6A4458-56A3-4F60-8792-5271326DED00'" );  // Page: Groups,  Zone: Main,  Block: Text Over Image Block
            Sql( @"UPDATE [Block] SET [Order] = 0 WHERE [Guid] = '7E110550-159D-463A-A532-673564806896'" );  // Page: Sermon Detail,  Zone: Main,  Block: Video Player Block
            Sql( @"UPDATE [Block] SET [Order] = 0 WHERE [Guid] = '941F3F7B-2922-4D29-9B8E-FE22A5337486'" );  // Page: Visit Detail,  Zone: Main,  Block: Image Block
            Sql( @"UPDATE [Block] SET [Order] = 0 WHERE [Guid] = '9735B56B-32A3-45CE-9298-5C1F7EFB3DEA'" );  // Page: Sermon Detail Audio,  Zone: Main,  Block: Image Block
            Sql( @"UPDATE [Block] SET [Order] = 0 WHERE [Guid] = 'A47A5177-CD5B-4A1F-9F6D-C1718DB0BA16'" );  // Page: Series Detail,  Zone: Main,  Block: Image Block
            Sql( @"UPDATE [Block] SET [Order] = 0 WHERE [Guid] = 'B8224C72-4168-40F0-96BE-38F2AFD525F5'" );  // Page: Shared Documents,  Zone: Main,  Block: Content
            Sql( @"UPDATE [Block] SET [Order] = 0 WHERE [Guid] = 'BAAA075C-AD3F-4A0B-AF0C-D9462760D16F'" );  // Page: Event Detail,  Zone: Main,  Block: Image Block
            Sql( @"UPDATE [Block] SET [Order] = 0 WHERE [Guid] = 'C6A448AB-B39B-48D1-9EDC-06062BF98766'" );  // Page: Connect,  Zone: Main,  Block: Preload Block
            Sql( @"UPDATE [Block] SET [Order] = 0 WHERE [Guid] = 'C730CAF4-F17F-4D9C-B2F3-9A6A29491FD7'" );  // Page: Visit,  Zone: Main,  Block: Text Over Image Block - Plan a visit
            Sql( @"UPDATE [Block] SET [Order] = 0 WHERE [Guid] = 'F3CE46B0-CB2C-49B2-95AB-91F6E29642D4'" );  // Page: Package Detail,  Zone: Main,  Block: Disable Buy
            Sql( @"UPDATE [Block] SET [Order] = 0 WHERE [Guid] = 'F6B935B9-6DA2-421A-9645-F9EF7B432E8C'" );  // Page: Baptism,  Zone: Main,  Block: Text Over Image Block
            Sql( @"UPDATE [Block] SET [Order] = 0 WHERE [Guid] = 'FB586E92-764C-4321-871F-47C38D768E94'" );  // Page: Events,  Zone: Main,  Block: Text Over Image Block
            Sql( @"UPDATE [Block] SET [Order] = 0 WHERE [Guid] = 'FCB9F034-0036-48DC-B588-6FBE969938C9'" );  // Page: Avalanche Home Page,  Zone: Main,  Block: Preload Block
            Sql( @"UPDATE [Block] SET [Order] = 1 WHERE [Guid] = '010B3F1F-2149-4DBA-ABB3-1DCF795C4C89'" );  // Page: Baptism,  Zone: Main,  Block: Markdown Detail
            Sql( @"UPDATE [Block] SET [Order] = 1 WHERE [Guid] = '089C9B48-AE89-416C-98FB-32304206DB20'" );  // Page: Visit Detail,  Zone: Main,  Block: Header Title
            Sql( @"UPDATE [Block] SET [Order] = 1 WHERE [Guid] = '11955742-2F20-40BB-B304-7865C992C7B4'" );  // Page: Sermon Detail,  Zone: Main,  Block: Markdown Detail
            Sql( @"UPDATE [Block] SET [Order] = 1 WHERE [Guid] = '33D3ED38-5558-43EA-B108-6E36239272A0'" );  // Page: Package Detail,  Zone: Main,  Block: Package Detail
            Sql( @"UPDATE [Block] SET [Order] = 1 WHERE [Guid] = '44D7ED68-22F4-4D1F-B443-75FF6B9F791B'" );  // Page: Connect,  Zone: Main,  Block: Text Over Image Block - Groups
            Sql( @"UPDATE [Block] SET [Order] = 1 WHERE [Guid] = '57096CBB-21E8-4EF5-ABD3-05B2414225D0'" );  // Page: Visit,  Zone: Main,  Block: Label Block - Welcome
            Sql( @"UPDATE [Block] SET [Order] = 1 WHERE [Guid] = '7761BC7C-DDE7-4DCB-81C9-720339A24477'" );  // Page: Sermon Detail Audio,  Zone: Main,  Block: Audio Player Block
            Sql( @"UPDATE [Block] SET [Order] = 1 WHERE [Guid] = '897EB15F-66BB-44F6-BF89-C9012CA36BCA'" );  // Page: Serve,  Zone: Main,  Block: Markdown Detail
            Sql( @"UPDATE [Block] SET [Order] = 1 WHERE [Guid] = '897FE435-F9E8-4755-AC87-5B273A415DCD'" );  // Page: Event Detail,  Zone: Main,  Block: Label Block
            Sql( @"UPDATE [Block] SET [Order] = 1 WHERE [Guid] = '898CD5D8-AB1E-4267-8084-161FFBAA7820'" );  // Page: Groups,  Zone: Main,  Block: Markdown Detail
            Sql( @"UPDATE [Block] SET [Order] = 1 WHERE [Guid] = 'A2487BA5-843D-4D7E-92A6-36A52027EB26'" );  // Page: Avalanche Home Page,  Zone: Main,  Block: Text Over Image Block - Sermons
            Sql( @"UPDATE [Block] SET [Order] = 1 WHERE [Guid] = 'A9D36E34-4D02-4179-B6DE-EF30B341D1D9'" );  // Page: Events,  Zone: Main,  Block: Event Filter
            Sql( @"UPDATE [Block] SET [Order] = 1 WHERE [Guid] = 'B4E000EB-5E15-454D-B16D-EF4B4FF5958B'" );  // Page: Internal Homepage,  Zone: Feature,  Block: HTML Content
            Sql( @"UPDATE [Block] SET [Order] = 1 WHERE [Guid] = 'E970D367-2901-4658-B8A4-D364EB4ABAA0'" );  // Page: Note Editor,  Zone: Main,  Block: Notes
            Sql( @"UPDATE [Block] SET [Order] = 1 WHERE [Guid] = 'EDC51B07-CA23-4415-8997-E03A60D5FA5A'" );  // Page: Series Detail,  Zone: Main,  Block: Markdown Detail
            Sql( @"UPDATE [Block] SET [Order] = 1 WHERE [Guid] = 'F4381F77-C8AB-4894-B5D6-94D58C996ADA'" );  // Page: Shared Documents,  Zone: Main,  Block: File Manager
            Sql( @"UPDATE [Block] SET [Order] = 2 WHERE [Guid] = '0B08E530-6763-473F-A338-C7E2B2F753BA'" );  // Page: Visit,  Zone: Main,  Block: Welcome Text
            Sql( @"UPDATE [Block] SET [Order] = 2 WHERE [Guid] = '10F85187-7796-45FC-A65D-4EC1314E3609'" );  // Page: Event Detail,  Zone: Main,  Block: Markdown Detail
            Sql( @"UPDATE [Block] SET [Order] = 2 WHERE [Guid] = '6B2D9BCB-6229-4859-BA77-9E4BF34AE181'" );  // Page: Groups,  Zone: Main,  Block: Button - Attend Growth Track
            Sql( @"UPDATE [Block] SET [Order] = 2 WHERE [Guid] = '713083B7-B5A8-416C-9603-594589A67B8D'" );  // Page: Events,  Zone: Main,  Block: Avalanche Event Calendar Lava
            Sql( @"UPDATE [Block] SET [Order] = 2 WHERE [Guid] = '77E64C5A-7F2A-4634-BEA4-46ECBAF4E56D'" );  // Page: Serve,  Zone: Main,  Block: Mobile ListView Lava
            Sql( @"UPDATE [Block] SET [Order] = 2 WHERE [Guid] = '7AA9738F-BA87-4761-9FD3-FF8A38703473'" );  // Page: Series Detail,  Zone: Main,  Block: Mobile ListView Lava - Sermon Content
            Sql( @"UPDATE [Block] SET [Order] = 2 WHERE [Guid] = '7E1A52B6-68A9-423F-88DB-4E402360FED0'" );  // Page: Avalanche Home Page,  Zone: Main,  Block: Text Over Image Block - Events
            Sql( @"UPDATE [Block] SET [Order] = 2 WHERE [Guid] = '8F975583-92F5-4827-909B-97D3D09E4BEC'" );  // Page: Sermon Detail Audio,  Zone: Main,  Block: Markdown Detail
            Sql( @"UPDATE [Block] SET [Order] = 2 WHERE [Guid] = '9B44C218-D744-43AC-B4A0-9DBD7C78911D'" );  // Page: Baptism,  Zone: Main,  Block: Button
            Sql( @"UPDATE [Block] SET [Order] = 2 WHERE [Guid] = 'BA50E899-CEAB-4816-8AD0-A650889882A4'" );  // Page: Sermon Detail,  Zone: Main,  Block: Icon Button - Listen
            Sql( @"UPDATE [Block] SET [Order] = 2 WHERE [Guid] = 'BDAAFA41-836E-4C27-9696-847D685D3980'" );  // Page: Visit Detail,  Zone: Main,  Block: Description/Location summary text
            Sql( @"UPDATE [Block] SET [Order] = 2 WHERE [Guid] = 'DC43E7F1-A294-4D99-B37A-CC04F18BD32C'" );  // Page: Connect,  Zone: Main,  Block: Text Over Image Block - Baptism
            Sql( @"UPDATE [Block] SET [Order] = 3 WHERE [Guid] = '14EFA755-D8F8-4BC2-A199-E67B2E960A95'" );  // Page: Visit,  Zone: Main,  Block: Campus Listview
            Sql( @"UPDATE [Block] SET [Order] = 3 WHERE [Guid] = '41BD57CC-71E3-4DC0-957C-F9265061FA65'" );  // Page: Visit Detail,  Zone: Main,  Block: Parking Map Button
            Sql( @"UPDATE [Block] SET [Order] = 3 WHERE [Guid] = '4622ABFE-2EEB-4F0C-9F5D-E4D37C36310A'" );  // Page: Shared Documents,  Zone: Main,  Block: Content
            Sql( @"UPDATE [Block] SET [Order] = 3 WHERE [Guid] = '6AA0CB59-E4BA-4E74-8198-749EEB553835'" );  // Page: Baptism,  Zone: Main,  Block: Markdown Detail
            Sql( @"UPDATE [Block] SET [Order] = 3 WHERE [Guid] = '87B3FD61-73E7-4BEF-AC56-1A96E497E916'" );  // Page: Groups,  Zone: Main,  Block: Next Steps detail
            Sql( @"UPDATE [Block] SET [Order] = 3 WHERE [Guid] = '99865F0D-BD8A-4AA7-88A2-D7D7859E1992'" );  // Page: Avalanche Home Page,  Zone: Main,  Block: Text Over Image Block - Visit
            Sql( @"UPDATE [Block] SET [Order] = 3 WHERE [Guid] = 'C454F889-AB57-4D38-A18F-195D60445EDF'" );  // Page: Event Detail,  Zone: Main,  Block: Mobile Button
            Sql( @"UPDATE [Block] SET [Order] = 3 WHERE [Guid] = 'E7E0ADD1-0863-492E-B7D7-A32B5680816F'" );  // Page: Connect,  Zone: Main,  Block: Text Over Image Block - Events
            Sql( @"UPDATE [Block] SET [Order] = 3 WHERE [Guid] = 'F915EB5A-9260-4681-9A49-0C6778D2F084'" );  // Page: Sermon Detail Audio,  Zone: Main,  Block: Icon Button - Watch
            Sql( @"UPDATE [Block] SET [Order] = 4 WHERE [Guid] = '20E578B0-BAB2-40DA-944D-9189465C6A96'" );  // Page: Visit Detail,  Zone: Main,  Block: More Info Button
            Sql( @"UPDATE [Block] SET [Order] = 4 WHERE [Guid] = '2901482E-A8C5-4A47-8EEB-2D3C9E0EA407'" );  // Page: Connect,  Zone: Main,  Block: Text Over Image Block - Serve
            Sql( @"UPDATE [Block] SET [Order] = 4 WHERE [Guid] = 'D0B758C4-18E1-45DE-82B0-70906E45B99A'" );  // Page: Groups,  Zone: Main,  Block: Next Steps Button
            Sql( @"UPDATE [Block] SET [Order] = 5 WHERE [Guid] = '99CAEBAF-9E80-410E-BB33-9DCD6B4ECF9B'" );  // Page: Connect,  Zone: Main,  Block: Button
        }

        /// <summary>
        /// The commands to undo a migration from a specific version
        /// </summary>
        public override void Down()
        {
            //
            // remove blocks
            //

            // Remove Block: Icon Block, from , Layout: No Scroll, Site: Avalanche
            RockMigrationHelper.DeleteBlock( "962317D3-42D0-4152-839E-35AAF2BD66E2" );
            // Remove Block: WebViewBlock, from Page: Webview Block, Site: Avalanche
            RockMigrationHelper.DeleteBlock( "E43CF3E1-D8FA-41B3-AED9-E09FAA594F7B" );
            // Remove Block: Button, from Page: Connect, Site: Avalanche
            RockMigrationHelper.DeleteBlock( "99CAEBAF-9E80-410E-BB33-9DCD6B4ECF9B" );
            // Remove Block: Group Attendance Block, from Page: Group Attendance, Site: Avalanche
            RockMigrationHelper.DeleteBlock( "BD824150-F1B1-4A7D-93FA-5A4AA7AE565D" );
            // Remove Block: Note Block, from Page: Note Block, Site: Avalanche
            RockMigrationHelper.DeleteBlock( "FB213CB9-C91F-4546-9371-73839FBD447A" );
            // Remove Block: Group Member List Block, from Page: Group Member List, Site: Avalanche
            RockMigrationHelper.DeleteBlock( "D53AE8B1-9F7B-457C-A2AB-297D7E0B0C71" );
            // Remove Block: Group List, from Page: Group List, Site: Avalanche
            RockMigrationHelper.DeleteBlock( "13472A3E-A5FD-47C1-A382-4243BBE0BC98" );
            // Remove Block: Person Card, from Page: Person Card, Site: Avalanche
            RockMigrationHelper.DeleteBlock( "86FF3353-4FEE-4272-BD9B-78B5C373051A" );
            // Remove Block: Prayer Request Entry, from Page: Prayer Request, Site: Avalanche
            RockMigrationHelper.DeleteBlock( "BFB73CDC-B2B6-4484-B35F-3BC25C430ECB" );
            // Remove Block: Login App, from Page: Login, Site: Avalanche
            RockMigrationHelper.DeleteBlock( "0DB6C226-836D-4CEB-B3CC-9F50ED499DD5" );
            // Remove Block: Mobile ListView Lava - Child Pages, from Page: Other Blocks, Site: Avalanche
            RockMigrationHelper.DeleteBlock( "253974FD-3F00-4677-86A4-F4F8C84F275C" );
            // Remove Block: WebViewBlock, from Page: Give, Site: Avalanche
            RockMigrationHelper.DeleteBlock( "FAF7BF20-4F53-47CC-B7E7-A5223BED5188" );
            // Remove Block: Mobile ListView Lava, from Page: Serve, Site: Avalanche
            RockMigrationHelper.DeleteBlock( "77E64C5A-7F2A-4634-BEA4-46ECBAF4E56D" );
            // Remove Block: Markdown Detail, from Page: Serve, Site: Avalanche
            RockMigrationHelper.DeleteBlock( "897EB15F-66BB-44F6-BF89-C9012CA36BCA" );
            // Remove Block: Text Over Image Block, from Page: Serve, Site: Avalanche
            RockMigrationHelper.DeleteBlock( "454646F6-C317-4764-B514-4AF84C1E7FEC" );
            // Remove Block: Text Over Image Block - Serve, from Page: Connect, Site: Avalanche
            RockMigrationHelper.DeleteBlock( "2901482E-A8C5-4A47-8EEB-2D3C9E0EA407" );
            // Remove Block: Markdown Detail, from Page: Baptism, Site: Avalanche
            RockMigrationHelper.DeleteBlock( "6AA0CB59-E4BA-4E74-8198-749EEB553835" );
            // Remove Block: Button, from Page: Baptism, Site: Avalanche
            RockMigrationHelper.DeleteBlock( "9B44C218-D744-43AC-B4A0-9DBD7C78911D" );
            // Remove Block: Markdown Detail, from Page: Baptism, Site: Avalanche
            RockMigrationHelper.DeleteBlock( "010B3F1F-2149-4DBA-ABB3-1DCF795C4C89" );
            // Remove Block: Text Over Image Block, from Page: Baptism, Site: Avalanche
            RockMigrationHelper.DeleteBlock( "F6B935B9-6DA2-421A-9645-F9EF7B432E8C" );
            // Remove Block: Mobile Button, from Page: Event Detail, Site: Avalanche
            RockMigrationHelper.DeleteBlock( "C454F889-AB57-4D38-A18F-195D60445EDF" );
            // Remove Block: Markdown Detail, from Page: Event Detail, Site: Avalanche
            RockMigrationHelper.DeleteBlock( "10F85187-7796-45FC-A65D-4EC1314E3609" );
            // Remove Block: Label Block, from Page: Event Detail, Site: Avalanche
            RockMigrationHelper.DeleteBlock( "897FE435-F9E8-4755-AC87-5B273A415DCD" );
            // Remove Block: Image Block, from Page: Event Detail, Site: Avalanche
            RockMigrationHelper.DeleteBlock( "BAAA075C-AD3F-4A0B-AF0C-D9462760D16F" );
            // Remove Block: Event Filter, from Page: Events, Site: Avalanche
            RockMigrationHelper.DeleteBlock( "A9D36E34-4D02-4179-B6DE-EF30B341D1D9" );
            // Remove Block: Text Over Image Block, from Page: Events, Site: Avalanche
            RockMigrationHelper.DeleteBlock( "FB586E92-764C-4321-871F-47C38D768E94" );
            // Remove Block: Avalanche Event Calendar Lava, from Page: Events, Site: Avalanche
            RockMigrationHelper.DeleteBlock( "713083B7-B5A8-416C-9603-594589A67B8D" );
            // Remove Block: Mobile Workflow, from Page: Contact Us, Site: Avalanche
            RockMigrationHelper.DeleteBlock( "F05B2A93-CEF8-4EAE-9358-EACC73C31173" );
            // Remove Block: Next Steps Button, from Page: Groups, Site: Avalanche
            RockMigrationHelper.DeleteBlock( "D0B758C4-18E1-45DE-82B0-70906E45B99A" );
            // Remove Block: Next Steps detail, from Page: Groups, Site: Avalanche
            RockMigrationHelper.DeleteBlock( "87B3FD61-73E7-4BEF-AC56-1A96E497E916" );
            // Remove Block: Button - Attend Growth Track, from Page: Groups, Site: Avalanche
            RockMigrationHelper.DeleteBlock( "6B2D9BCB-6229-4859-BA77-9E4BF34AE181" );
            // Remove Block: Markdown Detail, from Page: Groups, Site: Avalanche
            RockMigrationHelper.DeleteBlock( "898CD5D8-AB1E-4267-8084-161FFBAA7820" );
            // Remove Block: Text Over Image Block, from Page: Groups, Site: Avalanche
            RockMigrationHelper.DeleteBlock( "5C6A4458-56A3-4F60-8792-5271326DED00" );
            // Remove Block: Text Over Image Block - Events, from Page: Connect, Site: Avalanche
            RockMigrationHelper.DeleteBlock( "E7E0ADD1-0863-492E-B7D7-A32B5680816F" );
            // Remove Block: Text Over Image Block - Groups, from Page: Connect, Site: Avalanche
            RockMigrationHelper.DeleteBlock( "44D7ED68-22F4-4D1F-B443-75FF6B9F791B" );
            // Remove Block: Text Over Image Block - Baptism, from Page: Connect, Site: Avalanche
            RockMigrationHelper.DeleteBlock( "DC43E7F1-A294-4D99-B37A-CC04F18BD32C" );
            // Remove Block: Preload Block, from Page: Connect, Site: Avalanche
            RockMigrationHelper.DeleteBlock( "C6A448AB-B39B-48D1-9EDC-06062BF98766" );
            // Remove Block: Preload Block, from Page: Avalanche Home Page, Site: Avalanche
            RockMigrationHelper.DeleteBlock( "FCB9F034-0036-48DC-B588-6FBE969938C9" );
            // Remove Block: Image Block, from Page: Sermon Detail Audio, Site: Avalanche
            RockMigrationHelper.DeleteBlock( "9735B56B-32A3-45CE-9298-5C1F7EFB3DEA" );
            // Remove Block: Icon Button - Watch, from Page: Sermon Detail Audio, Site: Avalanche
            RockMigrationHelper.DeleteBlock( "F915EB5A-9260-4681-9A49-0C6778D2F084" );
            // Remove Block: Icon Button - Listen, from Page: Sermon Detail, Site: Avalanche
            RockMigrationHelper.DeleteBlock( "BA50E899-CEAB-4816-8AD0-A650889882A4" );
            // Remove Block: Audio Player Block, from Page: Sermon Detail Audio, Site: Avalanche
            RockMigrationHelper.DeleteBlock( "7761BC7C-DDE7-4DCB-81C9-720339A24477" );
            // Remove Block: Markdown Detail, from Page: Sermon Detail Audio, Site: Avalanche
            RockMigrationHelper.DeleteBlock( "8F975583-92F5-4827-909B-97D3D09E4BEC" );
            // Remove Block: Markdown Detail, from Page: Sermon Detail, Site: Avalanche
            RockMigrationHelper.DeleteBlock( "11955742-2F20-40BB-B304-7865C992C7B4" );
            // Remove Block: Video Player Block, from Page: Sermon Detail, Site: Avalanche
            RockMigrationHelper.DeleteBlock( "7E110550-159D-463A-A532-673564806896" );
            // Remove Block: Markdown Detail, from Page: Series Detail, Site: Avalanche
            RockMigrationHelper.DeleteBlock( "EDC51B07-CA23-4415-8997-E03A60D5FA5A" );
            // Remove Block: Image Block, from Page: Series Detail, Site: Avalanche
            RockMigrationHelper.DeleteBlock( "A47A5177-CD5B-4A1F-9F6D-C1718DB0BA16" );
            // Remove Block: Mobile ListView Lava - Sermon Content, from Page: Series Detail, Site: Avalanche
            RockMigrationHelper.DeleteBlock( "7AA9738F-BA87-4761-9FD3-FF8A38703473" );
            // Remove Block: Content Channel Mobile List, from Page: Sermons, Site: Avalanche
            RockMigrationHelper.DeleteBlock( "724E2C2F-213B-4959-9F01-1957C77733DD" );
            // Remove Block: More Info Button, from Page: Visit Detail, Site: Avalanche
            RockMigrationHelper.DeleteBlock( "20E578B0-BAB2-40DA-944D-9189465C6A96" );
            // Remove Block: Parking Map Button, from Page: Visit Detail, Site: Avalanche
            RockMigrationHelper.DeleteBlock( "41BD57CC-71E3-4DC0-957C-F9265061FA65" );
            // Remove Block: Description/Location summary text, from Page: Visit Detail, Site: Avalanche
            RockMigrationHelper.DeleteBlock( "BDAAFA41-836E-4C27-9696-847D685D3980" );
            // Remove Block: Header Title, from Page: Visit Detail, Site: Avalanche
            RockMigrationHelper.DeleteBlock( "089C9B48-AE89-416C-98FB-32304206DB20" );
            // Remove Block: Image Block, from Page: Visit Detail, Site: Avalanche
            RockMigrationHelper.DeleteBlock( "941F3F7B-2922-4D29-9B8E-FE22A5337486" );
            // Remove Block: Campus Listview, from Page: Visit, Site: Avalanche
            RockMigrationHelper.DeleteBlock( "14EFA755-D8F8-4BC2-A199-E67B2E960A95" );
            // Remove Block: Welcome Text, from Page: Visit, Site: Avalanche
            RockMigrationHelper.DeleteBlock( "0B08E530-6763-473F-A338-C7E2B2F753BA" );
            // Remove Block: Label Block - Welcome, from Page: Visit, Site: Avalanche
            RockMigrationHelper.DeleteBlock( "57096CBB-21E8-4EF5-ABD3-05B2414225D0" );
            // Remove Block: Text Over Image Block - Plan a visit, from Page: Visit, Site: Avalanche
            RockMigrationHelper.DeleteBlock( "C730CAF4-F17F-4D9C-B2F3-9A6A29491FD7" );
            // Remove Block: Icon Block - Back, from , Layout: Simple, Site: Avalanche
            RockMigrationHelper.DeleteBlock( "D2DFBCB4-894C-4E5C-A534-110B21A00FEB" );
            // Remove Block: Listview Footer, from Page: Footer, Site: Avalanche
            RockMigrationHelper.DeleteBlock( "0DBA9AAB-FA2E-4C53-B820-A4C0B3FF29D0" );
            // Remove Block: Text Over Image Block - Visit, from Page: Avalanche Home Page, Site: Avalanche
            RockMigrationHelper.DeleteBlock( "99865F0D-BD8A-4AA7-88A2-D7D7859E1992" );
            // Remove Block: Text Over Image Block - Events, from Page: Avalanche Home Page, Site: Avalanche
            RockMigrationHelper.DeleteBlock( "7E1A52B6-68A9-423F-88DB-4E402360FED0" );
            RockMigrationHelper.DeleteBlock( "47C24453-10F9-4A11-9BD0-3D9B1CC943D3" );
            // Remove Block: Text Over Image Block - Sermons, from Page: Avalanche Home Page, Site: Avalanche
            RockMigrationHelper.DeleteBlock( "A2487BA5-843D-4D7E-92A6-36A52027EB26" );
            RockMigrationHelper.DeleteBlockType( "36569642-F6D7-4EAC-AF0E-C8238BEEAF7E" ); // Person Profile Family
            RockMigrationHelper.DeleteBlockType( "41D83AAD-F2B8-43DA-978E-1391B4427280" ); // Public Profile Edit Block
            RockMigrationHelper.DeleteBlockType( "DE3997BC-AE8E-43B0-A41B-1F52C1BFE3B0" ); // Event Filter
            RockMigrationHelper.DeleteBlockType( "6AE6B96C-689F-434B-AC3A-683DA598D07C" ); // Foreign Objects
            RockMigrationHelper.DeleteBlockType( "78FC6291-B753-4782-8AED-DB04681F1D0E" ); // WebViewBlock
            RockMigrationHelper.DeleteBlockType( "6FA66F35-5D80-4BD5-AD39-71618C1FFEAD" ); // Video Player Block
            RockMigrationHelper.DeleteBlockType( "207ABAA7-8C45-472E-8649-3E4E895184B7" ); // Text Over Image Block
            RockMigrationHelper.DeleteBlockType( "01CFA67B-429A-432E-BA56-4F4289917D06" ); // Preload Block
            RockMigrationHelper.DeleteBlockType( "8189E73E-7E5C-4D37-BD62-FD7FBDEEC9FD" ); // Prayer Request Entry
            RockMigrationHelper.DeleteBlockType( "912A2617-C744-4D65-A0E1-A795469CFD0D" ); // Phone Number Login
            RockMigrationHelper.DeleteBlockType( "9610B4D8-EB3B-45ED-B8DF-9E97FA1DF7AF" ); // Person Card
            RockMigrationHelper.DeleteBlockType( "275D5C9B-7D0B-42EE-A0D1-198B5C1A8A3B" ); // Note Block
            RockMigrationHelper.DeleteBlockType( "6B9DDC12-D7B3-4521-9D49-B79BE6578CB1" ); // Mobile Workflow
            RockMigrationHelper.DeleteBlockType( "755D550E-FB64-4DBE-A054-9D0141A18001" ); // Mobile ListView Lava
            RockMigrationHelper.DeleteBlockType( "C2E57F26-0F77-4844-A298-1A24B088D645" ); // Mobile Content Item
            RockMigrationHelper.DeleteBlockType( "35B0B2B4-BE73-4C03-8F56-DD8D28EBD7FA" ); // Markdown Detail
            RockMigrationHelper.DeleteBlockType( "857ABAF2-1F35-404E-827D-F4ADD629CBDF" ); // Login App
            RockMigrationHelper.DeleteBlockType( "A42B3143-E970-4BEC-A694-9BCB37B9B737" ); // Label Block
            RockMigrationHelper.DeleteBlockType( "47C24453-10F9-4A11-9BD0-3D9B1CC943D3" ); // Image Block
            RockMigrationHelper.DeleteBlockType( "4CCA5B0C-63A6-40C1-B981-648663029092" ); // Icon Button
            RockMigrationHelper.DeleteBlockType( "1112BA70-CE0D-4158-8A2B-D5F2FD217BAE" ); // Icon Block
            RockMigrationHelper.DeleteBlockType( "E6BAE8D3-B244-400F-8092-AFD27156F6BD" ); // Group Member List Block
            RockMigrationHelper.DeleteBlockType( "55CB76A9-74F8-4F31-97B5-2567EBB25DFF" ); // Group List
            RockMigrationHelper.DeleteBlockType( "06A2663C-9292-4202-ACEE-A5F884671E55" ); // Group Attendance Block
            RockMigrationHelper.DeleteBlockType( "641A5E85-F2B4-46A3-8B9F-4AA759E33804" ); // Content Channel Mobile List
            RockMigrationHelper.DeleteBlockType( "A5129AF8-9744-4974-9EC3-F5D898DC7B77" ); // Avalanche Event Calendar Lava
            RockMigrationHelper.DeleteBlockType( "65CE4075-6A52-40F4-8ED0-6540F387BE76" ); // Button
            RockMigrationHelper.DeleteBlockType( "2E5AAAEB-C731-4F7A-B502-BA67589AF69F" ); // Avalanche Configuration
            RockMigrationHelper.DeleteBlockType( "7515DA74-D55F-44C0-ABC0-A6D9DBFEF76B" ); // Audio Player Block
        }
    }
}
