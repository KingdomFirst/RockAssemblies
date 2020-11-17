// <copyright>
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
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Data.Entity;
using System.Linq;

using Rock;
using Rock.Attribute;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;
using Rock.Workflow;

namespace rocks.kfs.Workflow.Action.WorkflowAttributes
{
    #region Action Attributes

    [ActionCategory( "KFS: Workflow Attributes" )]
    [Description( "Sets an attribute to the leader of the geo fenced group type provided. Returns the first person in a role marked 'Is Leader'." )]
    [Export( typeof( ActionComponent ) )]
    [ExportMetadata( "ComponentName", "Attribute Set to Geo Fenced Group Leader" )]

    #endregion

    #region Action Settings

    [GroupTypeField( "Group Type", "The type of group to use which has geo-fence locations.", true, "", "", 0 )]
    [CustomDropdownListField( "Role", "Optional role to limit scope of 'IsLeader'.", "SELECT r.Guid AS [Value], t.Name + ' - ' + r.Name AS [Text] FROM GroupTypeRole r JOIN GroupType t ON r.GroupTypeId = t.Id ORDER BY t.Name, r.Name", false, "", "", 1 )]
    [WorkflowAttribute( "Person", "The attribute of the person to have map locations evaluated in selected group type.", true, "", "", 2, null,
        new string[] { "Rock.Field.Types.TextFieldType", "Rock.Field.Types.PersonFieldType" } )]
    [WorkflowAttribute( "Leader", "The attribute to set to the group leader.", true, "", "", 3, null,
        new string[] { "Rock.Field.Types.TextFieldType", "Rock.Field.Types.PersonFieldType" } )]

    #endregion

    /// <summary>
    /// Sets an attribute equal to the person who created workflow (if known).
    /// </summary>
    public class SetAttributeToGroupLeader : ActionComponent
    {
        /// <summary>
        /// Executes the specified workflow.
        /// </summary>
        /// <param name="rockContext">The rock context.</param>
        /// <param name="action">The action.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="errorMessages">The error messages.</param>
        /// <returns></returns>
        public override bool Execute( RockContext rockContext, WorkflowAction action, Object entity, out List<string> errorMessages )
        {
            errorMessages = new List<string>();

            // get the person
            Person person = null;
            Guid? personAttributeGuid = GetAttributeValue( action, "Person" ).AsGuidOrNull();
            if ( personAttributeGuid.HasValue )
            {
                Guid? personAliasGuid = action.GetWorkflowAttributeValue( personAttributeGuid.Value ).AsGuidOrNull();
                if ( personAliasGuid.HasValue )
                {
                    var personAlias = new PersonAliasService( rockContext ).Get( personAliasGuid.Value );
                    if ( personAlias != null )
                    {
                        person = personAlias.Person;
                    }
                }
                else
                {
                    errorMessages.Add( "Invalid Person Attribute or Value!" );
                    return false;
                }
            }

            // get the group type attribute
            Guid? groupTypeGuid = GetAttributeValue( action, "GroupType" ).AsGuidOrNull();

            if ( person != null && person.Id > 0 )
            {
                if ( groupTypeGuid.HasValue )
                {
                    // get the role
                    Guid? roleGuid = GetAttributeValue( action, "Role" ).AsGuidOrNull();

                    var groupMemberService = new GroupMemberService( rockContext );

                    var groups = new GroupService( rockContext ).GetGeofencingGroups( person.Id, (Guid)groupTypeGuid ).AsNoTracking();

                    var result = new List<Person>();
                    foreach ( var group in groups.OrderBy( g => g.Name ).OrderByDescending( g => g.Order ) )
                    {
                        var info = new List<Person>();
                        info = groupMemberService
                            .Queryable().AsNoTracking()
                            .Where( m =>
                                m.GroupId == group.Id &&
                                ( !roleGuid.HasValue || m.GroupRole.Guid == roleGuid ) &&
                                m.GroupRole.IsLeader )
                            .OrderBy( m => m.GroupRole.Order )
                            .Select( m => m.Person )
                            .ToList();
                        result.AddRange( info );
                    }

                    var groupLeader = result.FirstOrDefault();

                    if ( groupLeader != null )
                    {
                        // Get the attribute to set
                        Guid leaderGuid = GetAttributeValue( action, "Leader" ).AsGuid();
                        if ( !leaderGuid.IsEmpty() )
                        {
                            var personAttribute = AttributeCache.Get( leaderGuid, rockContext );
                            if ( personAttribute != null )
                            {
                                // If this is a person type attribute
                                if ( personAttribute.FieldTypeId == FieldTypeCache.Get( Rock.SystemGuid.FieldType.PERSON.AsGuid(), rockContext ).Id )
                                {
                                    SetWorkflowAttributeValue( action, leaderGuid, groupLeader.PrimaryAlias.Guid.ToString() );
                                }
                                else if ( personAttribute.FieldTypeId == FieldTypeCache.Get( Rock.SystemGuid.FieldType.TEXT.AsGuid(), rockContext ).Id )
                                {
                                    SetWorkflowAttributeValue( action, leaderGuid, groupLeader.FullName );
                                }
                            }
                        }
                    }
                }
                else
                {
                    errorMessages.Add( "The group type could not be found!" );
                }
            }
            else
            {
                errorMessages.Add( "The person could not be found!" );
            }

            errorMessages.ForEach( m => action.AddLogEntry( m, true ) );

            return true;
        }
    }
}
