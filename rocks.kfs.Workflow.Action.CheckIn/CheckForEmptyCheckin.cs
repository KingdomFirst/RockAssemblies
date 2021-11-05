// <copyright>
// Copyright 2021 by Kingdom First Solutions
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
using System.Linq;

using Rock;
using Rock.Attribute;
using Rock.CheckIn;
using Rock.Data;
using Rock.Model;
using Rock.Workflow;
using Rock.Workflow.Action.CheckIn;

namespace rocks.kfs.Workflow.Action.CheckIn
{
    #region Action Attributes

    [ActionCategory( "KFS: Check-In" )]
    [Description( "Check for Empty Check-in session due to thresholds, already checked-in, etc." )]
    [Export( typeof( ActionComponent ) )]
    [ExportMetadata( "ComponentName", "Check for Empty Check-in" )]

    #endregion

    #region Action Settings
    [LavaField( "Error Message Template", "This will be the error message that gets attached to the check-in state for this instance of the action. Logic can be applied based on 'RemovedPeople', 'RemovedGroupTypes', 'RemovedGroups' and 'RemovedLocations'. If you return an empty message it will not display for this instance in our block.", true, "Check-in empty at this point. Customize this error message for where you insert it in the process. " )]
    [BooleanField( "Always Log Lava", "Enable this if you want the message to be created no matter the result. By default it will only create a message if all people will be removed, such as when your whole family is already checked in or the classrooms are full.", false )]
    #endregion

    /// <summary>
    /// Adds the locations for each members group types
    /// </summary>
    public class CheckForEmptyCheckin : CheckInActionComponent
    {
        /// <summary>
        /// Executes the specified workflow.
        /// </summary>
        /// <param name="rockContext">The rock context.</param>
        /// <param name="action">The workflow action.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="errorMessages">The error messages.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override bool Execute( RockContext rockContext, WorkflowAction action, Object entity, out List<string> errorMessages )
        {
            var checkInState = GetCheckInState( entity, out errorMessages );
            if ( checkInState != null )
            {
                string errorMessageTemplate = GetAttributeValue( action, "ErrorMessageTemplate" );
                bool logLava = GetAttributeValue( action, "AlwaysLogLava" ).AsBoolean();

                var peopleRemoved = new List<int>();
                // personId, list of Group Types removed
                var totalGroupTypesRemoved = new List<Dictionary<int, List<int>>>();
                // groupTypeId, list of Groups removed
                var totalGroupsRemoved = new List<Dictionary<int, List<int>>>();

                foreach ( var family in checkInState.CheckIn.Families.ToList() )
                {
                    foreach ( var person in family.People.ToList() )
                    {
                        var groupTypesRemoved = new List<int>();
                        foreach ( var groupType in person.GroupTypes.ToList() )
                        {
                            var groupsRemoved = new List<int>();
                            foreach ( var group in groupType.Groups.ToList() )
                            {
                                if ( group.Locations.Count == 0 || !group.Locations.Any( l => !l.ExcludedByFilter ) )
                                {
                                    groupsRemoved.Add( group.Group.Id );
                                }
                            }
                            if ( groupsRemoved.Count > 0 )
                            {
                                var dGroups = new Dictionary<int, List<int>>();
                                dGroups.Add( groupType.GroupType.Id, groupsRemoved );
                                totalGroupsRemoved.Add( dGroups );
                            }
                            if ( groupType.Groups.Count == 0 || !groupType.Groups.Any( g => !groupsRemoved.Contains( g.Group.Id ) ) || !groupType.Groups.Any( g => !g.ExcludedByFilter ) )
                            {
                                groupTypesRemoved.Add( groupType.GroupType.Id );
                            }
                        }
                        if ( groupTypesRemoved.Count > 0 )
                        {
                            var dGroupTypes = new Dictionary<int, List<int>>();
                            dGroupTypes.Add( person.Person.Id, groupTypesRemoved );
                            totalGroupTypesRemoved.Add( dGroupTypes );
                        }
                        if ( person.GroupTypes.Count == 0 || person.GroupTypes.All( gt => groupTypesRemoved.Contains( gt.GroupType.Id ) ) || person.GroupTypes.All( t => t.ExcludedByFilter ) )
                        {
                            peopleRemoved.Add( person.Person.Id );
                        }
                    }
                }

                var noMatchingFamilies =
                (
                    checkInState.CheckIn.Families.All( f => f.People.Count == 0 || !f.People.Any( fp => !peopleRemoved.Contains( fp.Person.Id ) ) ) &&
                    checkInState.CheckIn.Families.All( f => f.Action == CheckinAction.CheckIn ) // not sure this is needed
                )
                &&
                (
                    !checkInState.AllowCheckout ||
                    (
                        checkInState.AllowCheckout &&
                        checkInState.CheckIn.Families.All( f => f.CheckOutPeople.Count == 0 || !f.CheckOutPeople.Any( fp => !peopleRemoved.Contains( fp.Person.Id ) ) )
                    )
                );
                if ( logLava || noMatchingFamilies )
                {
                    var mergeFields = new Dictionary<string, object>();
                    mergeFields.Add( "CurrentFamily", checkInState.CheckIn.CurrentFamily );
                    mergeFields.Add( "Families", checkInState.CheckIn.Families );
                    mergeFields.Add( "CurrentPerson", checkInState.CheckIn.CurrentPerson );
                    mergeFields.Add( "SearchValue", checkInState.CheckIn.SearchValue );
                    mergeFields.Add( "Messages", checkInState.Messages );
                    mergeFields.Add( "RemovedPeople", peopleRemoved );
                    mergeFields.Add( "RemovedGroupTypes", totalGroupTypesRemoved );
                    mergeFields.Add( "RemovedGroups", totalGroupsRemoved );
                    mergeFields.Add( "NoMatchingFamilies", noMatchingFamilies );

                    checkInState.Messages.Add( new CheckInMessage { MessageText = errorMessageTemplate.ResolveMergeFields( mergeFields, checkInState.CheckIn.CurrentPerson?.Person ), MessageType = MessageType.Warning } );
                }

                return true;
            }

            return false;
        }
    }
}
