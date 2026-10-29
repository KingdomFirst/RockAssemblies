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
import { RockApi, RockEntity } from "../rockApi";
import { SystemGuid, TestNamePrefix } from "../systemGuids";

/** Rock's SignatureType enum. */
export const SignatureType = { Typed: 0, Drawn: 1 } as const;

/** Text the template's document shows, so a test can find it in the signature iframe. */
export const SignatureDocumentText = "KFS E2E signature document.";

/**
 * Creates an active electronic signature template that uses Rock's own signing (no
 * provider), a typed signature and no completion email, so signing sends nothing.
 */
export async function createSignatureTemplate( api: RockApi, name: string ): Promise<number> {
    return await api.post( "/api/SignatureDocumentTemplates", {
        Guid: randomUUID(),
        Name: `${TestNamePrefix} ${name}`,
        Description: "Created by rocks.kfs.Tests.EndToEnd. Safe to delete.",
        BinaryFileTypeId: await api.getIdByGuid( "BinaryFileTypes", SystemGuid.BinaryFileType.DigitallySignedDocuments ),
        LavaTemplate: `<p>${SignatureDocumentText} Applies to {{ AppliesToPerson.FullName }}.</p>`,
        DocumentTerm: "Document",
        SignatureType: SignatureType.Typed,
        IsActive: true,
        IsValidInFuture: false
    } );
}

/** Signature documents created from the template, newest first. */
export async function getSignatureDocuments( api: RockApi, templateId: number ): Promise<RockEntity[]> {
    return await api.query( "SignatureDocuments", `SignatureDocumentTemplateId eq ${templateId}`, "&$orderby=Id desc" );
}

/** Deletes every signature document made from the template, and its PDF. */
export async function deleteSignatureDocuments( api: RockApi, templateId: number ): Promise<void> {
    for ( const document of await getSignatureDocuments( api, templateId ) ) {
        await api.delete( "SignatureDocuments", document.Id );
        if ( document.BinaryFileId ) {
            try {
                await api.delete( "BinaryFiles", document.BinaryFileId as number );
            }
            catch {
                // Rock may already have removed the file with its document.
            }
        }
    }
}

export async function deleteSignatureTemplate( api: RockApi, templateId: number ): Promise<void> {
    await deleteSignatureDocuments( api, templateId );
    await api.delete( "SignatureDocumentTemplates", templateId );
}

/** Deletes a template with this name left behind by an earlier run. */
export async function deleteLeftoverSignatureTemplates( api: RockApi, name: string ): Promise<void> {
    for ( const template of await api.query( "SignatureDocumentTemplates", `Name eq '${TestNamePrefix} ${name}'` ) ) {
        await deleteSignatureTemplate( api, template.Id );
    }
}
