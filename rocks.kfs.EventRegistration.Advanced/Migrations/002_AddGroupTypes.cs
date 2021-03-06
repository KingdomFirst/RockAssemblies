﻿// <copyright>
// Copyright 2019 by Kingdom First Solutions
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
using System.Linq;
using Rock;
using Rock.Data;
using Rock.Model;
using Rock.Plugin;

namespace rocks.kfs.EventRegistration.Advanced.Migrations
{
    [MigrationNumber( 2, "1.8.5" )]
    public class AddGroupTypes : Migration
    {
        private string BooleanFieldTypeGuid = "1EDAFDED-DFE6-4334-B019-6EECBA89E05A";
        private string AdvancedEventGuid = "8C203BFA-A777-4DD1-AC1F-FE5B8EA97F2E";
        private string EventActivityGuid = "7EE55A72-D3CA-4728-B7B9-E843348C1A36";
        private string EventLodgingGuid = "EB8D40E7-A71E-4288-853F-2084AE3CB24D";
        private string EventTransportationGuid = "6BDD3A27-E178-44FD-A3AC-6C177D59EE18";
        private string EventVolunteerGuid = "181EAC76-7FA5-47B1-8D5D-2D1E25C76A8B";
        private string GroupTypePurposeServingAreaGuid = "36A554CE-7815-41B9-A435-93F3D52A2828";

        /// <summary>
        /// Ups this instance.
        /// </summary>
        public override void Up()
        {
            // check if grouptype already exists
            using ( var rockContext = new RockContext() )
            {
                var eventGuid = AdvancedEventGuid.AsGuid();
                if ( new GroupTypeService( rockContext ).Queryable().Any( gt => gt.Name == "Advanced Event Registration" || gt.Guid.Equals( eventGuid ) ) )
                {
                    return;
                }
            }

            var migrateNamespace = false;
            var oldNamespace = "com.kfs.EventRegistration.Advanced";

            // check if migration has previously run in case the grouptype check doesn't escape
            using ( var rockContext = new RockContext() )
            {
                var migrationNumber = ( System.Attribute.GetCustomAttribute( this.GetType(), typeof( MigrationNumberAttribute ) ) as MigrationNumberAttribute ).Number;
                migrateNamespace = new PluginMigrationService( rockContext )
                    .Queryable()
                    .Where( m => m.PluginAssemblyName.Equals( oldNamespace, StringComparison.CurrentCultureIgnoreCase ) && m.MigrationNumber == migrationNumber )
                    .Any();
            }

            // add primary grouptype
            RockMigrationHelper.AddGroupType( "Advanced Event Registration", "This template triggers advanced features for the Event Registration module.", "Advanced Events", "Member", false, true, true, "fa fa-fire", 0, "", 0, "", AdvancedEventGuid, false );

            // add grouptype attributes to the primary grouptype
            AddGroupTypeAttribute( "Rock.Model.GroupType", AdvancedEventGuid, BooleanFieldTypeGuid, "Allow Multiple Registrations", @"Should registrants be allowed to join multiple groups of this type?", 0, "False", true, "4F9CA590-882A-4A2A-9262-C9350953C996" );
            AddGroupTypeAttribute( "Rock.Model.GroupType", AdvancedEventGuid, BooleanFieldTypeGuid, "Allow Volunteer Assignment", @"Should volunteers be allowed to join groups of this type?", 1, "False", true, "7129D352-5468-4BD9-BF2E-5CF9758D83BF" );
            AddGroupTypeAttribute( "Rock.Model.GroupType", AdvancedEventGuid, BooleanFieldTypeGuid, "Show On Grid", @"Should the Registrants grid show assignment columns for this type?", 2, "False", true, "60BD7029-9D83-42CC-B904-9A1F3A89C1E6" );
            AddGroupTypeAttribute( "Rock.Model.GroupType", AdvancedEventGuid, BooleanFieldTypeGuid, "Display Combined Memberships", @"Should the resource panel display a combined list of groups? If not, each group will be listed separately.", 3, "False", true, "7DD366B4-0A8C-4DA0-B14E-A17A1AFF55A6" );
            AddGroupTypeAttribute( "Rock.Model.GroupType", AdvancedEventGuid, BooleanFieldTypeGuid, "Display Separate Roles", @"Should the resource panel display group members separately by role?  If not, group members will be listed together.", 4, "False", true, "469BA2BC-FEB5-4C95-9BA2-B382F01C88E3" );

            // add child grouptypes
            RockMigrationHelper.AddGroupType( "Event Activities", "Activity Groups for Advanced Events.", "Activity", "Participant", false, true, true, "fa fa-futbol-o", 0, AdvancedEventGuid, 0, "", EventActivityGuid, false );
            RockMigrationHelper.AddGroupType( "Event Lodging", "Lodging Groups for Advanced Events.", "Lodging", "Occupant", false, true, true, "fa fa-bed", 0, AdvancedEventGuid, 0, "", EventLodgingGuid, false );
            RockMigrationHelper.AddGroupType( "Event Transportation", "Transportation Groups for Advanced Events.", "Vehicle", "Passenger", false, true, true, "fa fa-bus", 0, AdvancedEventGuid, 0, "", EventTransportationGuid, false );
            RockMigrationHelper.AddGroupType( "Event Volunteers", "Volunteer Groups for Advanced Events.", "Group", "Volunteer", false, true, true, "fa fa-handshake-o", 0, AdvancedEventGuid, 0, GroupTypePurposeServingAreaGuid, EventVolunteerGuid, false );

            // only add the association if this is the first time
            if ( !migrateNamespace )
            {
                Sql( $@"
                    DECLARE @AdvancedEventId INT = (SELECT [ID] FROM GroupType WHERE [Guid] = '{AdvancedEventGuid}')
                    INSERT GroupTypeAssociation (GroupTypeId, ChildGroupTypeId)
                    VALUES (@AdvancedEventId, (SELECT [ID] FROM GroupType WHERE [Guid] = '{EventActivityGuid}')),
                        (@AdvancedEventId, (SELECT [ID] FROM GroupType WHERE [Guid] = '{EventLodgingGuid}')),
                        (@AdvancedEventId, (SELECT [ID] FROM GroupType WHERE [Guid] = '{EventTransportationGuid}')),
                        (@AdvancedEventId, (SELECT [ID] FROM GroupType WHERE [Guid] = '{EventVolunteerGuid}'))
                " );
            }

            // add group roles
            RockMigrationHelper.AddGroupTypeRole( AdvancedEventGuid, "Member", "Member of Advanced Events (not used)", 0, null, null, Guid.NewGuid().ToString(), false, false, true );
            RockMigrationHelper.AddGroupTypeRole( EventActivityGuid, "Participant", "Participant role for Advanced Event Activity Groups", 0, null, null, Guid.NewGuid().ToString(), false, false, true );
            RockMigrationHelper.AddGroupTypeRole( EventLodgingGuid, "Leader", "Leader role for Advanced Event Lodging Groups", 0, null, null, Guid.NewGuid().ToString(), false, true, false );
            RockMigrationHelper.AddGroupTypeRole( EventLodgingGuid, "Occupant", "Occupant role for Advanced Event Lodging Groups", 0, null, null, Guid.NewGuid().ToString(), false, false, true );
            RockMigrationHelper.AddGroupTypeRole( EventTransportationGuid, "Driver", "Driver role for Advanced Event Transportation Groups", 0, null, null, Guid.NewGuid().ToString(), false, true, false );
            RockMigrationHelper.AddGroupTypeRole( EventTransportationGuid, "Passenger", "Passenger role for Advanced Event Transportation Groups", 0, null, null, Guid.NewGuid().ToString(), false, false, true );
            RockMigrationHelper.AddGroupTypeRole( EventVolunteerGuid, "Member", "Member role for Advanced Event Volunteer Groups", 0, null, null, Guid.NewGuid().ToString(), false, false, true );
        }

        /// <summary>
        /// Downs this instance.
        /// </summary>
        public override void Down()
        {
            RockMigrationHelper.DeleteAttribute( "4F9CA590-882A-4A2A-9262-C9350953C996" );    // GroupType - Group Type Attribute, Advanced Event Registration: Allow Multiple Registrations
            RockMigrationHelper.DeleteAttribute( "7129D352-5468-4BD9-BF2E-5CF9758D83BF" );    // GroupType - Group Type Attribute, Advanced Event Registration: Allow Volunteer Assignment
            RockMigrationHelper.DeleteAttribute( "7DD366B4-0A8C-4DA0-B14E-A17A1AFF55A6" );    // GroupType - Group Type Attribute, Advanced Event Registration: Display Combined Memberships
            RockMigrationHelper.DeleteAttribute( "60BD7029-9D83-42CC-B904-9A1F3A89C1E6" );    // GroupType - Group Type Attribute, Advanced Event Registration: Show On Grid
            RockMigrationHelper.DeleteAttribute( "469BA2BC-FEB5-4C95-9BA2-B382F01C88E3" );    // GroupType - Group Type Attribute, Advanced Event Registration: Display Separate Roles

            RockMigrationHelper.DeleteGroupType( EventActivityGuid );
            RockMigrationHelper.DeleteGroupType( EventLodgingGuid );
            RockMigrationHelper.DeleteGroupType( EventTransportationGuid );
            RockMigrationHelper.DeleteGroupType( EventVolunteerGuid );
            RockMigrationHelper.DeleteGroupType( AdvancedEventGuid );
        }

        /// <summary>
        /// Adds the group type attribute.
        /// </summary>
        /// <param name="entityTypeName">Name of the entity type.</param>
        /// <param name="groupTypeGuid">The group type unique identifier.</param>
        /// <param name="fieldTypeGuid">The field type unique identifier.</param>
        /// <param name="name">The name.</param>
        /// <param name="description">The description.</param>
        /// <param name="order">The order.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="isRequired">if set to <c>true</c> [is required].</param>
        /// <param name="guid">The unique identifier.</param>
        /// <see cref="Rock.Data.MigrationHelper.AddGroupTypeAttribute"/>
        private void AddGroupTypeAttribute( string entityTypeName, string groupTypeGuid, string fieldTypeGuid, string name, string description, int order, string defaultValue, bool isRequired, string guid )
        {
            string defaultValueDbParam = ( defaultValue == null ) ? "NULL" : "'" + defaultValue + "'";
            string attributeKey = name.RemoveSpaces();

            Sql( $@"

                DECLARE @EntityTypeId int
                SET @EntityTypeId = (SELECT [Id] FROM [EntityType] WHERE [Name] = '{entityTypeName}')

                DECLARE @GroupTypeId int
                SET @GroupTypeId = (SELECT [Id] FROM [GroupType] WHERE [Guid] = '{groupTypeGuid}')

                DECLARE @FieldTypeId int
                SET @FieldTypeId = (SELECT [Id] FROM [FieldType] WHERE [Guid] = '{fieldTypeGuid}')

                IF EXISTS (
                    SELECT [Id]
                    FROM [Attribute]
                    WHERE [EntityTypeId] = @EntityTypeId
                    AND [EntityTypeQualifierColumn] = 'Id'
                    AND [EntityTypeQualifierValue] = @GroupTypeId
                    AND [Key] = '{attributeKey}' )
                BEGIN
                    UPDATE [Attribute] SET
                        [FieldTypeId] = @FieldTypeId,
                        [Name] = '{name}',
                        [Description] = '{description.Replace( "'", "''" )}',
                        [Order] = {order},
                        [DefaultValue] = {defaultValueDbParam},
                        [IsRequired]= {isRequired.Bit()},
                        [Guid] = '{guid}'
                    WHERE [EntityTypeId] = @EntityTypeId
                    AND [EntityTypeQualifierColumn] = 'Id'
                    AND [EntityTypeQualifierValue] = @GroupTypeId
                    AND [Key] = '{attributeKey}'
                END
                ELSE
                BEGIN
                    INSERT INTO [Attribute]
                        ([IsSystem]
                        ,[FieldTypeId]
                        ,[EntityTypeId]
                        ,[EntityTypeQualifierColumn]
                        ,[EntityTypeQualifierValue]
                        ,[Key]
                        ,[Name]
                        ,[Description]
                        ,[Order]
                        ,[IsGridColumn]
                        ,[DefaultValue]
                        ,[IsMultiValue]
                        ,[IsRequired]
                        ,[Guid])
                    VALUES
                        (1
                        ,@FieldTypeId
                        ,@EntityTypeId
                        ,'Id'
                        ,@GroupTypeId
                        ,'{attributeKey}'
                        ,'{name}'
                        ,'{description.Replace( "'", "''" )}'
                        ,{order}
                        ,0
                        ,{defaultValueDbParam}
                        ,0
                        ,{isRequired.Bit()}
                        ,'{guid}')
                END
                " );
        }
    }
}
