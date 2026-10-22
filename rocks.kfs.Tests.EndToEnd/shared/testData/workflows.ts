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
import { randomUUID } from "crypto";
import { RockApi } from "../rockApi";
import { EntityTypeName, FieldTypeClass, TestNamePrefix } from "../systemGuids";

/**
 * A persisted workflow type whose only action records the entity it was launched with.
 * Asserting on that value proves a block both launched the workflow and passed it the
 * right entity (Person vs GroupMember vs ConnectionRequest), which is exactly what a
 * tester checks by hand with a "real" workflow.
 */
export type EntityCaptureWorkflowType = {
    id: number;
    guid: string;
    name: string;
    /** Workflow attribute that receives "{EntityType.Guid}|{Entity.Id}". */
    entityAttributeId: number;
};

/** The entity a workflow was launched with, as recorded by the capture action. */
export type CapturedEntity = {
    workflowId: number;
    entityTypeGuid: string;
    entityId: number;
};

/** Name of the persistent capture workflow type, built once by hand in Rock (see README). */
export const EntityCaptureWorkflowTypeName = `${TestNamePrefix} Entity Capture`;

/*
    10/5/2026 - CLAUDE

    On Rock 17.9 (rockbeta), creating a WorkflowActionType through the REST API
    (v1 or v2) never returns and pegs the server's CPU, while the same save in
    Rock's Workflow Configuration UI is instant. Likely cause: ASP.NET's model
    validation walks the computed WorkflowActionType.WorkflowAction property into
    the action component's cached attribute graph. So the capture workflow type
    is built once by hand and reused; the fixture only finds and checks it.

    Reason: Workflow action types cannot be created through the REST API on Rock 17.9.
*/

/**
 * Finds the persistent capture workflow type and checks it is built the way the tests
 * need. Throws with the exact build steps if it is missing or incomplete.
 */
export async function findEntityCaptureWorkflowType( api: RockApi ): Promise<EntityCaptureWorkflowType> {
    const steps = `In Rock (Admin Tools > Workflow Configuration), create a workflow type named '${EntityCaptureWorkflowTypeName}' in any category, with "Automatically Persisted" on. ` +
        "Add a workflow attribute 'Launched Entity' (key LaunchedEntity, field type Entity). " +
        "In its first activity (activated with the workflow), add one action of type 'Set Attribute From Entity' (Workflow Attributes category) whose Attribute setting is 'Launched Entity'. Save.";

    const workflowType = await api.first( "WorkflowTypes", `Name eq '${EntityCaptureWorkflowTypeName}'` );
    if ( !workflowType ) {
        throw new Error( `One-time setup: the capture workflow type is missing. ${steps}` );
    }

    const workflowEntityTypeId = await api.getEntityTypeId( EntityTypeName.Workflow );
    const entityAttribute = await api.first( "Attributes",
        `EntityTypeId eq ${workflowEntityTypeId} and EntityTypeQualifierColumn eq 'WorkflowTypeId' and EntityTypeQualifierValue eq '${workflowType.Id}' and Key eq 'LaunchedEntity'` );
    const problems: string[] = [];
    if ( !entityAttribute ) {
        problems.push( "no workflow attribute with key 'LaunchedEntity'" );
    }
    if ( !workflowType.IsPersisted ) {
        problems.push( "'Automatically Persisted' is off" );
    }

    const actionEntityTypeId = await api.getEntityTypeId( EntityTypeName.SetAttributeFromEntityAction );
    const actionTypeEntityTypeId = await api.getEntityTypeId( EntityTypeName.WorkflowActionType );
    const actionSetting = await api.first( "Attributes",
        `EntityTypeId eq ${actionTypeEntityTypeId} and EntityTypeQualifierColumn eq 'EntityTypeId' and EntityTypeQualifierValue eq '${actionEntityTypeId}' and Key eq 'Attribute'` );

    let captureActionFound = false;
    for ( const activity of await api.query( "WorkflowActivityTypes", `WorkflowTypeId eq ${workflowType.Id} and IsActivatedWithWorkflow eq true` ) ) {
        for ( const action of await api.query( "WorkflowActionTypes", `ActivityTypeId eq ${activity.Id} and EntityTypeId eq ${actionEntityTypeId}` ) ) {
            const target = actionSetting ? await api.getAttributeValue( actionSetting.Id, action.Id ) : null;
            if ( entityAttribute && target?.toLowerCase() === String( entityAttribute.Guid ).toLowerCase() ) {
                captureActionFound = true;
            }
        }
    }
    if ( !captureActionFound ) {
        problems.push( "no 'Set Attribute From Entity' action, in an activity activated with the workflow, that sets 'Launched Entity'" );
    }

    if ( problems.length > 0 ) {
        throw new Error( `One-time setup: '${EntityCaptureWorkflowTypeName}' (workflow type ${workflowType.Id}) is not built as the tests need: ${problems.join( "; " )}. ${steps}` );
    }

    return { id: workflowType.Id, guid: String( workflowType.Guid ), name: EntityCaptureWorkflowTypeName, entityAttributeId: entityAttribute!.Id };
}

/** Creates a capture workflow type through REST. Hangs on Rock 17.9; use findEntityCaptureWorkflowType there. */
export async function createEntityCaptureWorkflowType( api: RockApi, name: string ): Promise<EntityCaptureWorkflowType> {
    const fullName = `${TestNamePrefix} ${name}`;
    const workflowTypeGuid = randomUUID();

    const workflowTypeId = await api.post( "/api/WorkflowTypes", {
        Guid: workflowTypeGuid,
        Name: fullName,
        Description: "Created by rocks.kfs.Tests.EndToEnd. Safe to delete.",
        IsActive: true,
        IsSystem: false,
        IsPersisted: true,
        WorkTerm: "Work",
        LoggingLevel: 0,
        Order: 0
    } );

    // Workflow attribute that will receive the launching entity.
    const entityAttributeGuid = randomUUID();
    const entityAttributeId = await api.post( "/api/Attributes", {
        Guid: entityAttributeGuid,
        EntityTypeId: await api.getEntityTypeId( EntityTypeName.Workflow ),
        EntityTypeQualifierColumn: "WorkflowTypeId",
        EntityTypeQualifierValue: workflowTypeId.toString(),
        FieldTypeId: await api.getFieldTypeId( FieldTypeClass.Entity ),
        Key: "LaunchedEntity",
        Name: "Launched Entity",
        Order: 0,
        IsGridColumn: false,
        IsMultiValue: false,
        IsRequired: false,
        IsSystem: false
    } );

    const activityTypeId = await api.post( "/api/WorkflowActivityTypes", {
        Guid: randomUUID(),
        WorkflowTypeId: workflowTypeId,
        Name: "Start",
        IsActive: true,
        IsActivatedWithWorkflow: true,
        Order: 0
    } );

    const actionEntityTypeId = await api.getEntityTypeId( EntityTypeName.SetAttributeFromEntityAction );
    const actionTypeId = await api.post( "/api/WorkflowActionTypes", {
        Guid: randomUUID(),
        ActivityTypeId: activityTypeId,
        Name: "Capture Entity",
        EntityTypeId: actionEntityTypeId,
        IsActionCompletedOnSuccess: true,
        IsActivityCompletedOnSuccess: true,
        Order: 0
    } );

    // The action's own settings are attributes on WorkflowActionType qualified by the action's entity type.
    const actionTypeEntityTypeId = await api.getEntityTypeId( EntityTypeName.WorkflowActionType );
    const actionSetting = await api.first( "Attributes",
        `EntityTypeId eq ${actionTypeEntityTypeId} and EntityTypeQualifierColumn eq 'EntityTypeId' and EntityTypeQualifierValue eq '${actionEntityTypeId}' and Key eq 'Attribute'` );
    if ( !actionSetting ) {
        throw new Error( "The 'Set Attribute From Entity' workflow action has no 'Attribute' setting registered on this site." );
    }

    await api.setAttributeValue( actionSetting.Id, actionTypeId, entityAttributeGuid );

    return { id: workflowTypeId, guid: workflowTypeGuid, name: fullName, entityAttributeId };
}

/**
 * Creates a persisted workflow type that only holds Text attributes, for code (such as a job)
 * that fills in workflow attribute values itself. It has no activities, so each workflow
 * completes at once and Rock saves it with its attribute values. No action types are
 * created, so this works on Rock 17.9 (see the note above).
 */
export async function createAttributeOnlyWorkflowType( api: RockApi, args: { name: string; attributeKeys: string[] } ): Promise<{ id: number; guid: string; attributeIds: Record<string, number> }> {
    const guid = randomUUID();
    const id = await api.post( "/api/WorkflowTypes", {
        Guid: guid,
        Name: `${TestNamePrefix} ${args.name}`,
        Description: "Created by rocks.kfs.Tests.EndToEnd. Safe to delete.",
        IsActive: true,
        IsSystem: false,
        IsPersisted: true,
        WorkTerm: "Work",
        LoggingLevel: 0,
        Order: 0
    } );

    const workflowEntityTypeId = await api.getEntityTypeId( EntityTypeName.Workflow );
    const textFieldTypeId = await api.getFieldTypeId( FieldTypeClass.Text );
    const attributeIds: Record<string, number> = {};
    for ( const [ order, key ] of args.attributeKeys.entries() ) {
        attributeIds[ key ] = await api.post( "/api/Attributes", {
            Guid: randomUUID(),
            EntityTypeId: workflowEntityTypeId,
            EntityTypeQualifierColumn: "WorkflowTypeId",
            EntityTypeQualifierValue: id.toString(),
            FieldTypeId: textFieldTypeId,
            Key: key,
            Name: key,
            Order: order,
            IsGridColumn: false,
            IsMultiValue: false,
            IsRequired: false,
            IsSystem: false
        } );
    }

    return { id, guid, attributeIds };
}

/** Deletes every workflow of the type, so the next assertion only sees workflows the test launched. */
export async function deleteWorkflowsOfType( api: RockApi, workflowTypeId: number ): Promise<void> {
    for ( const workflow of await api.query( "Workflows", `WorkflowTypeId eq ${workflowTypeId}` ) ) {
        await api.delete( "Workflows", workflow.Id );
    }
}

/** Gets the entity recorded by the most recent workflow of the type, or null if none was launched. */
export async function getLastCapturedEntity( api: RockApi, workflowType: EntityCaptureWorkflowType ): Promise<CapturedEntity | null> {
    const workflow = await api.first( "Workflows", `WorkflowTypeId eq ${workflowType.id}`, "&$orderby=Id desc" );
    if ( !workflow ) {
        return null;
    }

    const value = await api.getAttributeValue( workflowType.entityAttributeId, workflow.Id ) ?? "";
    const [ entityTypeGuid, entityId ] = value.split( "|" );

    return {
        workflowId: workflow.Id,
        entityTypeGuid: ( entityTypeGuid ?? "" ).toUpperCase(),
        entityId: parseInt( entityId ?? "", 10 )
    };
}

export async function deleteWorkflowType( api: RockApi, workflowTypeId: number ): Promise<void> {
    await deleteWorkflowsOfType( api, workflowTypeId );

    for ( const activityType of await api.query( "WorkflowActivityTypes", `WorkflowTypeId eq ${workflowTypeId}` ) ) {
        for ( const actionType of await api.query( "WorkflowActionTypes", `ActivityTypeId eq ${activityType.Id}` ) ) {
            await api.delete( "WorkflowActionTypes", actionType.Id );
        }
        await api.delete( "WorkflowActivityTypes", activityType.Id );
    }

    const workflowEntityTypeId = await api.getEntityTypeId( EntityTypeName.Workflow );
    for ( const attribute of await api.query( "Attributes", `EntityTypeId eq ${workflowEntityTypeId} and EntityTypeQualifierColumn eq 'WorkflowTypeId' and EntityTypeQualifierValue eq '${workflowTypeId}'` ) ) {
        await api.delete( "Attributes", attribute.Id );
    }

    await api.delete( "WorkflowTypes", workflowTypeId );
}

/** Deletes workflow types left behind by an earlier run that did not finish cleanup. */
export async function deleteLeftoverWorkflowTypes( api: RockApi, name: string ): Promise<void> {
    for ( const workflowType of await api.query( "WorkflowTypes", `Name eq '${TestNamePrefix} ${name}'` ) ) {
        await deleteWorkflowType( api, workflowType.Id );
    }
}
