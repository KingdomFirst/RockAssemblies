using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rock.Plugin;

namespace com.kfs.Avalanche.Migrations
{
    [MigrationNumber( 2, "1.8.0" )]
    public class CreatePages : Migration
    {
        /// <summary>
        /// The commands to run to migrate plugin to the specific version
        /// </summary>
        public override void Up()
        {
            RockMigrationHelper.AddPage( true, "567FFD63-53F9-4419-AD96-C2F07CAE09F1", "901926F9-AD81-41A4-9B1E-254F5B45E471", "Sermons", "", "9711DB54-0FB0-4722-AB45-1DFB6158F922", "fa fa-tv" ); // Site:Avalanche
            RockMigrationHelper.AddPage( true, "567FFD63-53F9-4419-AD96-C2F07CAE09F1", "355F6C23-29B3-4976-AE43-30426BE12B99", "Visit", "", "CD8A05F8-24FF-4D38-8ED0-FE2BF07C0CDE", "fa fa-map-marker" ); // Site:Avalanche
            RockMigrationHelper.AddPage( true, "567FFD63-53F9-4419-AD96-C2F07CAE09F1", "355F6C23-29B3-4976-AE43-30426BE12B99", "Connect", "", "DE6D125E-892E-4F10-A33D-F84942582B1E", "fa fa-link" ); // Site:Avalanche
            RockMigrationHelper.AddPage( true, "567FFD63-53F9-4419-AD96-C2F07CAE09F1", "355F6C23-29B3-4976-AE43-30426BE12B99", "Give", "", "78EDFA36-FEC9-4D74-A34B-07C76A4FC071", "fa fa-heart" ); // Site:Avalanche
            RockMigrationHelper.AddPage( true, "567FFD63-53F9-4419-AD96-C2F07CAE09F1", "60D99A36-8D00-467E-9993-3C2F0B249EBD", "Footer", "", "FF495C30-29C5-420C-A35B-E9E808EEBCEF", "" ); // Site:Avalanche
            RockMigrationHelper.AddPage( true, "CD8A05F8-24FF-4D38-8ED0-FE2BF07C0CDE", "355F6C23-29B3-4976-AE43-30426BE12B99", "Visit Detail", "", "5F1E1759-0A3A-4A57-896B-7B8B8BD892B3", "" ); // Site:Avalanche
            RockMigrationHelper.AddPage( true, "9711DB54-0FB0-4722-AB45-1DFB6158F922", "355F6C23-29B3-4976-AE43-30426BE12B99", "Series Detail", "", "4F8A1FFA-05A7-4522-8C23-74468FAFD6BC", "" ); // Site:Avalanche
            RockMigrationHelper.AddPage( true, "4F8A1FFA-05A7-4522-8C23-74468FAFD6BC", "355F6C23-29B3-4976-AE43-30426BE12B99", "Sermon Detail", "", "1CDE2032-482B-483E-B273-A1A3B421E04C", "" ); // Site:Avalanche
            RockMigrationHelper.AddPage( true, "4F8A1FFA-05A7-4522-8C23-74468FAFD6BC", "355F6C23-29B3-4976-AE43-30426BE12B99", "Sermon Detail Audio", "", "344BABC7-5C30-4B53-87F9-A00F0DDA38E7", "" ); // Site:Avalanche
            RockMigrationHelper.AddPage( true, "DE6D125E-892E-4F10-A33D-F84942582B1E", "355F6C23-29B3-4976-AE43-30426BE12B99", "Groups", "", "D943417B-F167-4CD0-8321-C779B1C9E92B", "" ); // Site:Avalanche
            RockMigrationHelper.AddPage( true, "DE6D125E-892E-4F10-A33D-F84942582B1E", "355F6C23-29B3-4976-AE43-30426BE12B99", "Baptism", "", "8E94B8E8-171D-4A81-9A69-D34667510231", "" ); // Site:Avalanche
            RockMigrationHelper.AddPage( true, "DE6D125E-892E-4F10-A33D-F84942582B1E", "355F6C23-29B3-4976-AE43-30426BE12B99", "Events", "", "B95062A4-770C-47DF-B88F-0626F7BFDF2F", "" ); // Site:Avalanche
            RockMigrationHelper.AddPage( true, "D4A5C0AB-0A90-416F-9B3F-C0ACFED92D43", "355F6C23-29B3-4976-AE43-30426BE12B99", "Contact Us", "", "7817298D-0A76-4039-A5D7-AF273AC05952", "" ); // Site:Avalanche
            RockMigrationHelper.AddPage( true, "B95062A4-770C-47DF-B88F-0626F7BFDF2F", "355F6C23-29B3-4976-AE43-30426BE12B99", "Event Detail", "", "14A25533-186B-49C4-A949-0F9DA864A87E", "" ); // Site:Avalanche
            RockMigrationHelper.AddPage( true, "DE6D125E-892E-4F10-A33D-F84942582B1E", "355F6C23-29B3-4976-AE43-30426BE12B99", "Serve", "", "C90B1E1B-EC24-49E0-9562-7F92BB2D24AB", "" ); // Site:Avalanche
            RockMigrationHelper.AddPage( true, "DE6D125E-892E-4F10-A33D-F84942582B1E", "355F6C23-29B3-4976-AE43-30426BE12B99", "Other Blocks", "", "D4A5C0AB-0A90-416F-9B3F-C0ACFED92D43", "" ); // Site:Avalanche
            RockMigrationHelper.AddPage( true, "D4A5C0AB-0A90-416F-9B3F-C0ACFED92D43", "355F6C23-29B3-4976-AE43-30426BE12B99", "Login", "", "F53E2612-3E8B-4D26-87ED-468D4C9C02E6", "" ); // Site:Avalanche
            RockMigrationHelper.AddPage( true, "D4A5C0AB-0A90-416F-9B3F-C0ACFED92D43", "355F6C23-29B3-4976-AE43-30426BE12B99", "Prayer Request", "", "BB61CCD2-9E50-45A3-9D94-A37F3D7313FF", "" ); // Site:Avalanche
            RockMigrationHelper.AddPage( true, "D4A5C0AB-0A90-416F-9B3F-C0ACFED92D43", "355F6C23-29B3-4976-AE43-30426BE12B99", "Person Card", "", "79EF0279-9800-4BAF-A8D9-DD42E6868BAA", "" ); // Site:Avalanche
            RockMigrationHelper.AddPage( true, "D4A5C0AB-0A90-416F-9B3F-C0ACFED92D43", "355F6C23-29B3-4976-AE43-30426BE12B99", "Group Attendance", "", "2FB9C8A1-B3E6-4608-8CE7-411AA27945BA", "" ); // Site:Avalanche
            RockMigrationHelper.AddPage( true, "D4A5C0AB-0A90-416F-9B3F-C0ACFED92D43", "355F6C23-29B3-4976-AE43-30426BE12B99", "Group List", "", "0EC21B17-EA03-450E-A0B4-594C09340458", "" ); // Site:Avalanche
            RockMigrationHelper.AddPage( true, "D4A5C0AB-0A90-416F-9B3F-C0ACFED92D43", "355F6C23-29B3-4976-AE43-30426BE12B99", "Group Member List", "", "C4A65B7E-D57C-44C9-93AD-9E2E51145481", "" ); // Site:Avalanche
            RockMigrationHelper.AddPage( true, "D4A5C0AB-0A90-416F-9B3F-C0ACFED92D43", "355F6C23-29B3-4976-AE43-30426BE12B99", "Note Block", "", "F522A717-2D7A-405B-87BB-CCB16C15BB44", "" ); // Site:Avalanche
            RockMigrationHelper.AddPage( true, "D4A5C0AB-0A90-416F-9B3F-C0ACFED92D43", "901926F9-AD81-41A4-9B1E-254F5B45E471", "Webview Block", "", "0FBC5CE7-CF91-45EC-B28A-A5E486764B9A", "" ); // Site:Avalanche

            RockMigrationHelper.AddGlobalAttribute( Rock.SystemGuid.FieldType.TEXT, "", "", "Avalanche Footer Page", "Page Id of footer blocks/layout configuration", 0, "", "1B8B1811-D82F-48B0-A55C-4A463552264C" );
            RockMigrationHelper.AddGlobalAttribute( Rock.SystemGuid.FieldType.TEXT, "", "", "Avalanche Header Page", "Page Id of header blocks/layout configuration", 0, "", "B2F7130E-1C0C-41F8-A13E-3EE9B77F59B1" );

        }

        /// <summary>
        /// The commands to undo a migration from a specific version
        /// </summary>
        public override void Down()
        {
            //
            // remove pages
            //

            RockMigrationHelper.DeletePage( "0FBC5CE7-CF91-45EC-B28A-A5E486764B9A" ); //  Page: Webview Block, Layout: No Scroll, Site: Avalanche
            RockMigrationHelper.DeletePage( "F522A717-2D7A-405B-87BB-CCB16C15BB44" ); //  Page: Note Block, Layout: Simple, Site: Avalanche
            RockMigrationHelper.DeletePage( "C4A65B7E-D57C-44C9-93AD-9E2E51145481" ); //  Page: Group Member List, Layout: Simple, Site: Avalanche
            RockMigrationHelper.DeletePage( "0EC21B17-EA03-450E-A0B4-594C09340458" ); //  Page: Group List, Layout: Simple, Site: Avalanche
            RockMigrationHelper.DeletePage( "2FB9C8A1-B3E6-4608-8CE7-411AA27945BA" ); //  Page: Group Attendance, Layout: Simple, Site: Avalanche
            RockMigrationHelper.DeletePage( "79EF0279-9800-4BAF-A8D9-DD42E6868BAA" ); //  Page: Person Card, Layout: Simple, Site: Avalanche
            RockMigrationHelper.DeletePage( "BB61CCD2-9E50-45A3-9D94-A37F3D7313FF" ); //  Page: Prayer Request, Layout: Simple, Site: Avalanche
            RockMigrationHelper.DeletePage( "F53E2612-3E8B-4D26-87ED-468D4C9C02E6" ); //  Page: Login, Layout: Simple, Site: Avalanche
            RockMigrationHelper.DeletePage( "D4A5C0AB-0A90-416F-9B3F-C0ACFED92D43" ); //  Page: Other Blocks, Layout: Simple, Site: Avalanche
            RockMigrationHelper.DeletePage( "C90B1E1B-EC24-49E0-9562-7F92BB2D24AB" ); //  Page: Serve, Layout: Simple, Site: Avalanche
            RockMigrationHelper.DeletePage( "14A25533-186B-49C4-A949-0F9DA864A87E" ); //  Page: Event Detail, Layout: Simple, Site: Avalanche
            RockMigrationHelper.DeletePage( "7817298D-0A76-4039-A5D7-AF273AC05952" ); //  Page: Contact Us, Layout: Simple, Site: Avalanche
            RockMigrationHelper.DeletePage( "B95062A4-770C-47DF-B88F-0626F7BFDF2F" ); //  Page: Events, Layout: Simple, Site: Avalanche
            RockMigrationHelper.DeletePage( "8E94B8E8-171D-4A81-9A69-D34667510231" ); //  Page: Baptism, Layout: Simple, Site: Avalanche
            RockMigrationHelper.DeletePage( "D943417B-F167-4CD0-8321-C779B1C9E92B" ); //  Page: Groups, Layout: Simple, Site: Avalanche
            RockMigrationHelper.DeletePage( "344BABC7-5C30-4B53-87F9-A00F0DDA38E7" ); //  Page: Sermon Detail Audio, Layout: Simple, Site: Avalanche
            RockMigrationHelper.DeletePage( "1CDE2032-482B-483E-B273-A1A3B421E04C" ); //  Page: Sermon Detail, Layout: Simple, Site: Avalanche
            RockMigrationHelper.DeletePage( "4F8A1FFA-05A7-4522-8C23-74468FAFD6BC" ); //  Page: Series Detail, Layout: Simple, Site: Avalanche
            RockMigrationHelper.DeletePage( "5F1E1759-0A3A-4A57-896B-7B8B8BD892B3" ); //  Page: Visit Detail, Layout: Simple, Site: Avalanche
            RockMigrationHelper.DeletePage( "FF495C30-29C5-420C-A35B-E9E808EEBCEF" ); //  Page: Footer, Layout: Footer, Site: Avalanche
            RockMigrationHelper.DeletePage( "78EDFA36-FEC9-4D74-A34B-07C76A4FC071" ); //  Page: Give, Layout: Simple, Site: Avalanche
            RockMigrationHelper.DeletePage( "DE6D125E-892E-4F10-A33D-F84942582B1E" ); //  Page: Connect, Layout: Simple, Site: Avalanche
            RockMigrationHelper.DeletePage( "CD8A05F8-24FF-4D38-8ED0-FE2BF07C0CDE" ); //  Page: Visit, Layout: Simple, Site: Avalanche
            RockMigrationHelper.DeletePage( "9711DB54-0FB0-4722-AB45-1DFB6158F922" ); //  Page: Sermons, Layout: No Scroll, Site: Avalanche
            RockMigrationHelper.DeletePage( "567FFD63-53F9-4419-AD96-C2F07CAE09F1" ); //  Page: Avalanche Home Page, Layout: Main Page, Site: Avalanche

        }
    }
}
