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

/** Creates a global Person text attribute. The key is fixed so leftovers can be found and removed. */
export async function createPersonTextAttribute( api: RockApi, args: { key: string; name: string } ): Promise<number> {
    return await api.post( "/api/Attributes", {
        Guid: randomUUID(),
        EntityTypeId: await api.getEntityTypeId( EntityTypeName.Person ),
        EntityTypeQualifierColumn: "",
        EntityTypeQualifierValue: "",
        FieldTypeId: await api.getFieldTypeId( FieldTypeClass.Text ),
        Key: args.key,
        Name: `${TestNamePrefix} ${args.name}`,
        Description: "Created by rocks.kfs.Tests.EndToEnd. Safe to delete.",
        Order: 0,
        IsGridColumn: false,
        IsMultiValue: false,
        IsRequired: false,
        IsSystem: false
    } );
}

export async function deletePersonAttribute( api: RockApi, attributeId: number ): Promise<void> {
    for ( const value of await api.query( "AttributeValues", `AttributeId eq ${attributeId}` ) ) {
        await api.delete( "AttributeValues", value.Id );
    }

    await api.delete( "Attributes", attributeId );
}

/** Deletes a Person attribute with this key left behind by an earlier run. */
export async function deleteLeftoverPersonAttribute( api: RockApi, key: string ): Promise<void> {
    const personEntityTypeId = await api.getEntityTypeId( EntityTypeName.Person );
    for ( const attribute of await api.query( "Attributes", `EntityTypeId eq ${personEntityTypeId} and Key eq '${key}'` ) ) {
        await deletePersonAttribute( api, attribute.Id );
    }
}

/** Gets a global attribute's stored value (encrypted values stay encrypted), or its default. */
export async function getGlobalAttributeValue( api: RockApi, key: string ): Promise<string | null> {
    const attribute = await api.first( "Attributes", `Key eq '${key}' and EntityTypeId eq null` );
    if ( !attribute ) {
        return null;
    }
    // Global attribute values have no entity.
    const value = await api.first( "AttributeValues", `AttributeId eq ${attribute.Id} and EntityId eq null` );
    return ( value?.Value as string | undefined ) || ( attribute.DefaultValue as string | null ) || null;
}
