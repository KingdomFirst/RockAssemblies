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
import { EntityTypeName, TestNamePrefix } from "../systemGuids";

/** A temporary page holding one instance of the block under test. */
export type TestBlockPage = {
    pageId: number;
    blockId: number;
    blockTypeId: number;
    /** Block attribute Ids keyed by attribute Key. */
    attributeIds: Record<string, number>;
};

/**
 * Finds a block type by display name and category, e.g. "Person Attribute Forms Advanced"
 * in "KFS > CRM". (BlockType.Path is not exposed to REST queries.)
 */
export async function getBlockTypeId( api: RockApi, name: string, category: string ): Promise<number> {
    const blockType = await api.first( "BlockTypes", `Name eq '${name}' and Category eq '${category}'` );
    if ( !blockType ) {
        throw new Error( `Block type '${name}' (${category}) is not registered on the site under test. Is the plugin installed?` );
    }

    return blockType.Id;
}

/** The persistent, hidden page all test pages are created under. Its security is set up once by hand. */
export const TestPagesParent = {
    guid: "4B5F3A1E-0E2E-4E2E-8E2E-000000000001",
    name: `${TestNamePrefix} Test Pages`
} as const;

/**
 * Finds or creates the persistent "KFS E2E Test Pages" page under the configured parent.
 * Rights the tests need beyond View (such as Edit) are granted on this page once, by hand,
 * so they never apply to a real page; every test page inherits them.
 */
export async function ensureTestPagesParent( api: RockApi, parentPageId: number ): Promise<number> {
    const existing = await api.getByGuid( "Pages", TestPagesParent.guid );
    if ( existing ) {
        return existing.Id;
    }

    const parent = await api.getById( "Pages", parentPageId );
    return await api.post( "/api/Pages", {
        Guid: TestPagesParent.guid,
        InternalName: TestPagesParent.name,
        PageTitle: TestPagesParent.name,
        BrowserTitle: TestPagesParent.name,
        Description: "Parent of the pages the KFS end-to-end tests create. Do not delete; its security grants the test role what the tests need.",
        ParentPageId: parentPageId,
        LayoutId: parent.LayoutId,
        // DisplayInNavWhen.Never, so the page never shows up in site navigation.
        DisplayInNavWhen: 2,
        Order: 999,
        IsSystem: false,
        EnableViewState: true,
        IncludeAdminFooter: true,
        MenuDisplayChildPages: true,
        MenuDisplayDescription: false,
        MenuDisplayIcon: false
    } );
}

/** Creates a hidden page under the parent page (reusing its layout) and adds the block to it. */
export async function createBlockPage( api: RockApi, args: { name: string; parentPageId: number; blockTypeId: number; zone: string } ): Promise<TestBlockPage> {
    const parent = await api.getById( "Pages", args.parentPageId );
    const pageName = `${TestNamePrefix} ${args.name}`;

    const pageId = await api.post( "/api/Pages", {
        Guid: randomUUID(),
        InternalName: pageName,
        PageTitle: pageName,
        BrowserTitle: pageName,
        ParentPageId: args.parentPageId,
        LayoutId: parent.LayoutId,
        // DisplayInNavWhen.Never, so the page never shows up in site navigation.
        DisplayInNavWhen: 2,
        Order: 999,
        IsSystem: false,
        EnableViewState: true,
        IncludeAdminFooter: true,
        MenuDisplayChildPages: true,
        MenuDisplayDescription: false,
        MenuDisplayIcon: false
    } );

    const blockId = await api.post( "/api/Blocks", {
        Guid: randomUUID(),
        PageId: pageId,
        BlockTypeId: args.blockTypeId,
        Zone: args.zone,
        Name: pageName,
        Order: 0,
        IsSystem: false,
        OutputCacheDuration: 0
    } );

    return {
        pageId,
        blockId,
        blockTypeId: args.blockTypeId,
        attributeIds: await getBlockAttributeIds( api, args.blockTypeId )
    };
}

/**
 * Gets the block type's attribute Ids keyed by Key. Rock creates these when it first
 * loads the block type, so on a site where the plugin has never been placed on a page
 * this can be empty until the test page has been viewed once.
 */
export async function getBlockAttributeIds( api: RockApi, blockTypeId: number ): Promise<Record<string, number>> {
    const blockEntityTypeId = await api.getEntityTypeId( EntityTypeName.Block );
    const attributes = await api.query( "Attributes",
        `EntityTypeId eq ${blockEntityTypeId} and EntityTypeQualifierColumn eq 'BlockTypeId' and EntityTypeQualifierValue eq '${blockTypeId}'` );

    const ids: Record<string, number> = {};
    for ( const attribute of attributes ) {
        ids[ attribute.Key as string ] = attribute.Id;
    }

    return ids;
}

/** Sets block settings by attribute Key. Unknown keys fail loudly; a renamed setting is a real compatibility break. */
export async function setBlockSettings( api: RockApi, blockPage: TestBlockPage, values: Record<string, string> ): Promise<void> {
    for ( const [ key, value ] of Object.entries( values ) ) {
        const attributeId = blockPage.attributeIds[ key ];
        if ( !attributeId ) {
            throw new Error( `Block setting '${key}' does not exist on block type ${blockPage.blockTypeId}. Known settings: ${Object.keys( blockPage.attributeIds ).join( ", " )}` );
        }

        await api.setAttributeValue( attributeId, blockPage.blockId, value );
    }
}

/** A test copy of an existing Rock page, with one of its blocks swapped for the block under test. */
export type CopiedPage = TestBlockPage & {
    /** The block on the source page that was swapped out, so its settings can be copied. */
    sourceBlockId: number;
    sourceBlockTypeId: number;
};

/**
 * Copies a Rock page (same layout and blocks, hidden from navigation) and swaps one
 * block type for another, the way an admin installs a plugin that replaces a core
 * block. Settings of the blocks that are not swapped are copied over; the swapped
 * block's settings are copied with copyBlockSettings once its block type has registered
 * its settings.
 *
 * Without swapBlockTypeGuid nothing is swapped: every block and its settings is copied,
 * and blockId is the copy of the page's block of type blockTypeId (a plugin block the
 * source page already holds). parentPageId places the copy elsewhere than beside the
 * source, e.g. under the test pages parent so it inherits the test role's rights.
 * pageGuid gives the copy a fixed Guid, for a page kept between runs.
 */
export async function createPageCopy( api: RockApi, args: { sourcePageGuid: string; name: string; swapBlockTypeGuid?: string; blockTypeId: number; parentPageId?: number; pageGuid?: string } ): Promise<CopiedPage> {
    const source = await api.getByGuid( "Pages", args.sourcePageGuid );
    if ( !source ) {
        throw new Error( `Page ${args.sourcePageGuid} does not exist on the site under test.` );
    }

    const swap = args.swapBlockTypeGuid !== undefined;
    const sourceBlockTypeId = swap ? await api.getIdByGuid( "BlockTypes", args.swapBlockTypeGuid! ) : args.blockTypeId;
    const sourceBlocks = await api.query( "Blocks", `PageId eq ${source.Id}`, "&$orderby=Zone,Order" );
    const sourceBlock = sourceBlocks.find( block => block.BlockTypeId === sourceBlockTypeId );
    if ( !sourceBlock ) {
        throw new Error( `Page '${source.InternalName}' has no block of type ${swap ? args.swapBlockTypeGuid : `Id ${args.blockTypeId}`}.` );
    }

    const pageName = `${TestNamePrefix} ${args.name}`;
    const pageId = await api.post( "/api/Pages", {
        Guid: args.pageGuid ?? randomUUID(),
        InternalName: pageName,
        PageTitle: pageName,
        BrowserTitle: pageName,
        ParentPageId: args.parentPageId ?? source.ParentPageId,
        LayoutId: source.LayoutId,
        // DisplayInNavWhen.Never, so the page never shows up in site navigation.
        DisplayInNavWhen: 2,
        Order: 999,
        IsSystem: false,
        EnableViewState: true,
        IncludeAdminFooter: true,
        MenuDisplayChildPages: true,
        MenuDisplayDescription: false,
        MenuDisplayIcon: false
    } );

    let blockId = 0;
    for ( const block of sourceBlocks ) {
        const swapped = swap && block.Id === sourceBlock.Id;
        const newBlockId = await api.post( "/api/Blocks", {
            Guid: randomUUID(),
            PageId: pageId,
            BlockTypeId: swapped ? args.blockTypeId : block.BlockTypeId,
            Zone: block.Zone,
            Name: swapped ? pageName : block.Name,
            Order: block.Order,
            IsSystem: false,
            OutputCacheDuration: 0
        } );

        if ( block.Id === sourceBlock.Id ) {
            blockId = newBlockId;
        }
        if ( !swapped ) {
            const blockTypeId = block.BlockTypeId as number;
            await copyBlockSettings( api, { fromBlockId: block.Id, fromBlockTypeId: blockTypeId, toBlockId: newBlockId, toBlockTypeId: blockTypeId } );
        }
    }

    return {
        pageId,
        blockId,
        blockTypeId: args.blockTypeId,
        attributeIds: await getBlockAttributeIds( api, args.blockTypeId ),
        sourceBlockId: sourceBlock.Id,
        sourceBlockTypeId
    };
}

/** Copies a block's settings to another block, for every setting key the target block type also has. Returns the keys copied. */
export async function copyBlockSettings( api: RockApi, args: { fromBlockId: number; fromBlockTypeId: number; toBlockId: number; toBlockTypeId: number } ): Promise<string[]> {
    const fromAttributeIds = await getBlockAttributeIds( api, args.fromBlockTypeId );
    const toAttributeIds = args.toBlockTypeId === args.fromBlockTypeId ? fromAttributeIds : await getBlockAttributeIds( api, args.toBlockTypeId );
    const copied: string[] = [];

    for ( const [ key, fromAttributeId ] of Object.entries( fromAttributeIds ) ) {
        const toAttributeId = toAttributeIds[ key ];
        const value = toAttributeId ? await api.getAttributeValue( fromAttributeId, args.fromBlockId ) : null;
        if ( toAttributeId && value !== null ) {
            await api.setAttributeValue( toAttributeId, args.toBlockId, value );
            copied.push( key );
        }
    }

    return copied;
}

/** Deletes the test page, its block and any auth rules on them. */
export async function deleteBlockPage( api: RockApi, blockPage: { pageId: number; blockId?: number } ): Promise<void> {
    const pageEntityTypeId = await api.getEntityTypeId( EntityTypeName.Page );
    const blockEntityTypeId = await api.getEntityTypeId( EntityTypeName.Block );

    const blocks = await api.query( "Blocks", `PageId eq ${blockPage.pageId}` );
    for ( const block of blocks ) {
        for ( const auth of await api.query( "Auths", `EntityTypeId eq ${blockEntityTypeId} and EntityId eq ${block.Id}` ) ) {
            await api.delete( "Auths", auth.Id );
        }
        await api.delete( "Blocks", block.Id );
    }

    for ( const auth of await api.query( "Auths", `EntityTypeId eq ${pageEntityTypeId} and EntityId eq ${blockPage.pageId}` ) ) {
        await api.delete( "Auths", auth.Id );
    }

    await api.delete( "Pages", blockPage.pageId );
}

/** Deletes test pages left behind by an earlier run that did not finish cleanup. */
export async function deleteLeftoverTestPages( api: RockApi, name: string ): Promise<void> {
    const pages = await api.query( "Pages", `InternalName eq '${TestNamePrefix} ${name}'` );
    for ( const page of pages ) {
        await deleteBlockPage( api, { pageId: page.Id } );
    }
}
