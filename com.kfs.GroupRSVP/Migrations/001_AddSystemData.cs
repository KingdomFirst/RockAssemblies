using Rock.Plugin;

namespace com.kfs.GroupRSVP.Migrations
{
    [MigrationNumber( 1, "1.8.0" )]
    class AddSystemData : Migration
    {
        /// <summary>
        /// The commands to run to migrate plugin to the specific version
        /// </summary>
        public override void Up()
        {
            // group type
            RockMigrationHelper.AddGroupType( "RSVP Group", "A group that can be used to manage RSVP counts for the group members.", "Group", "Member", false, true, true, "fa fa-clipboard-list", 0, "", 0, "", "1A082EFF-30DA-44B2-8E48-02385C20828E", true );

            // group type role
            RockMigrationHelper.AddGroupTypeRole( "1A082EFF-30DA-44B2-8E48-02385C20828E", "Member", "", 0, null, null, "60E0B95A-04D3-4839-917B-6BDAF9808EB5", true, false, true );

            // max rsvp
            RockMigrationHelper.AddGroupTypeGroupAttribute( "1A082EFF-30DA-44B2-8E48-02385C20828E", Rock.SystemGuid.FieldType.INTEGER, "Max RSVP", "The RSVP limit for this group. '0' is unlimited.", 0, "", "AE34AFA5-8CB0-4BDA-8ACB-BAB661803BDC", true );

            // send email
            RockMigrationHelper.AddGroupTypeGroupAttribute( "1A082EFF-30DA-44B2-8E48-02385C20828E", Rock.SystemGuid.FieldType.BOOLEAN, "Send Email", "Flag indicating if the group email should be sent when someone joins this group.", 1, "False", "9B67F3BF-5F7A-4A9E-A352-67F3D515F63A", true );

            // from email
            RockMigrationHelper.AddGroupTypeGroupAttribute( "1A082EFF-30DA-44B2-8E48-02385C20828E", Rock.SystemGuid.FieldType.EMAIL, "From Email", "", 2, "", "6F8FB284-4CCB-45C1-A99E-F8ADA93856B1" );

            // from name
            RockMigrationHelper.AddGroupTypeGroupAttribute( "1A082EFF-30DA-44B2-8E48-02385C20828E", Rock.SystemGuid.FieldType.TEXT, "From Name", "", 3, "", "9BBE3207-1939-45E3-8A26-6519045E8EA9" );
            RockMigrationHelper.AddAttributeQualifier( "9BBE3207-1939-45E3-8A26-6519045E8EA9", "ispassword", "false", "8F791A08-72BC-482E-8FBC-8F6463C9669E" );

            // subject
            RockMigrationHelper.AddGroupTypeGroupAttribute( "1A082EFF-30DA-44B2-8E48-02385C20828E", Rock.SystemGuid.FieldType.TEXT, "Subject", "", 4, "", "C0AD8B37-4D7F-4C17-9B4E-1E8D4352E22D" );
            RockMigrationHelper.AddAttributeQualifier( "C0AD8B37-4D7F-4C17-9B4E-1E8D4352E22D", "ispassword", "false", "7284B714-AF12-440C-91FE-C4850F0D75F8" );

            // message
            RockMigrationHelper.AddGroupTypeGroupAttribute( "1A082EFF-30DA-44B2-8E48-02385C20828E", Rock.SystemGuid.FieldType.HTML, "Message", "", 5, "", "7DCB5A58-1FD2-4261-9483-EA65A97151CD" );
            RockMigrationHelper.AddAttributeQualifier( "7DCB5A58-1FD2-4261-9483-EA65A97151CD", "documentfolderroot", "", "B3C8C1C5-2DEB-49DA-87A8-8B87D48C9ED2" );
            RockMigrationHelper.AddAttributeQualifier( "7DCB5A58-1FD2-4261-9483-EA65A97151CD", "imagefolderroot", "", "C5CA6A49-E7A7-45AA-B9F4-85F8F6FA8B5D" );
            RockMigrationHelper.AddAttributeQualifier( "7DCB5A58-1FD2-4261-9483-EA65A97151CD", "toolbar", "Light", "242E3FFC-0680-494F-9EA3-55E8BEF11477" );
            RockMigrationHelper.AddAttributeQualifier( "7DCB5A58-1FD2-4261-9483-EA65A97151CD", "userspecificroot", "False", "A59A34A7-0EA8-414C-A78F-034FFE954768" );

            // member rsvp
            RockMigrationHelper.AddGroupTypeGroupMemberAttribute( "1A082EFF-30DA-44B2-8E48-02385C20828E", Rock.SystemGuid.FieldType.INTEGER, "RSVP Count", "", 0, "1", "877D17DD-6303-4863-B87C-F8D05111E835", true );

            // set member rsvp to display in grid, set the association to be self referenced, and set role to view
            Sql( @"
                DECLARE @GroupMemberRsvpCount INT = (SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '877D17DD-6303-4863-B87C-F8D05111E835')
                UPDATE [Attribute]
                SET [IsGridColumn] = 1
                WHERE [Id] = @GroupMemberRsvpCount

                DECLARE @RsvpGroupTypeId INT = (SELECT TOP 1 [Id] FROM [GroupType] WHERE [Guid] = '1A082EFF-30DA-44B2-8E48-02385C20828E')
                INSERT INTO [GroupTypeAssociation] ( [GroupTypeId], [ChildGroupTypeId] )
                SELECT @RsvpGroupTypeId, @RsvpGroupTypeId

                UPDATE [GroupTypeRole]
                SET [CanView] = 1
                WHERE [GroupTypeId] = @RsvpGroupTypeId
            " );

            // register blocks
            RockMigrationHelper.AddBlockType( "RSVP Group Registration", "Allows a person to register for an RSVP Group.", "~/Plugins/com_kfs/RsvpGroups/RsvpGroupRegistration.ascx", "KFS > RSVP Groups", "F7B249C3-7FFD-483D-820F-A44D04E2BAB1" );
            RockMigrationHelper.UpdateBlockType( "Group List", "Lists groups for lava display.", "~/Plugins/com_kfs/Groups/GroupListLava.ascx", "KFS > Groups", "6731AF9D-F3CB-4CCB-AA42-19C9CB15CBF5" ); // shared block type
        }

        /// <summary>
        /// The commands to undo a migration from a specific version
        /// </summary>
        public override void Down()
        {
            RockMigrationHelper.DeleteGroupType( "1A082EFF-30DA-44B2-8E48-02385C20828E" );
            RockMigrationHelper.DeleteGroupTypeRole( "60E0B95A-04D3-4839-917B-6BDAF9808EB5" );
            RockMigrationHelper.DeleteAttribute( "AE34AFA5-8CB0-4BDA-8ACB-BAB661803BDC" );
            RockMigrationHelper.DeleteAttribute( "9B67F3BF-5F7A-4A9E-A352-67F3D515F63A" );
            RockMigrationHelper.DeleteAttribute( "6F8FB284-4CCB-45C1-A99E-F8ADA93856B1" );
            RockMigrationHelper.DeleteAttribute( "9BBE3207-1939-45E3-8A26-6519045E8EA9" );
            RockMigrationHelper.DeleteAttribute( "C0AD8B37-4D7F-4C17-9B4E-1E8D4352E22D" );
            RockMigrationHelper.DeleteAttribute( "7DCB5A58-1FD2-4261-9483-EA65A97151CD" );
            RockMigrationHelper.DeleteAttribute( "877D17DD-6303-4863-B87C-F8D05111E835" );
            RockMigrationHelper.DeleteBlockType( "F7B249C3-7FFD-483D-820F-A44D04E2BAB1" );
        }
    }
}
