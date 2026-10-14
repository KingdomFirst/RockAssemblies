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
import { Locator, Page } from "@playwright/test";
import { expectNoRockError, waitForPostback } from "../../../shared/rockBrowser";

/**
 * Page object for RockWeb/Plugins/rocks_kfs/Groups/GroupFinder.ascx. Filter controls are
 * matched on the IDs the block gives them (filter_keyword, filter_cblCampus, filter_dows,
 * filter_{AttributeKey}_{FieldTypeId}, ...).
 */
export class GroupFinderBlock {
    public readonly root: Locator;

    public constructor( private readonly page: Page, blockId: number, private readonly resultCssClass = "kfs-e2e-gf-group" ) {
        this.root = page.locator( `#bid_${blockId}` );
    }

    /** Names of the groups in the results, in display order. */
    public async resultNames(): Promise<string[]> {
        return ( await this.root.locator( `.${this.resultCssClass}` ).allTextContents() ).map( name => name.trim() );
    }

    public get results(): Locator {
        return this.root.locator( `.${this.resultCssClass}` );
    }

    public get keyword(): Locator {
        return this.root.locator( "input[id$='_filter_keyword']" );
    }

    // Check box lists render no wrapper with the list's ID; each check box's own ID starts with it.
    public campusCheckBox( campusId: number ): Locator {
        return this.root.locator( `input[type='checkbox'][id*='_filter_cblCampus_'][value='${campusId}']` );
    }

    public dayCheckBox( dayOfWeek: number ): Locator {
        return this.root.locator( `input[type='checkbox'][id*='_filter_dows_'][value='${dayOfWeek}']` );
    }

    public attributeCheckBox( attributeKey: string, value: string ): Locator {
        return this.root.locator( `input[type='checkbox'][id*='_filter_${attributeKey}_'][value='${value}']` );
    }

    public get postalCode(): Locator {
        return this.root.locator( "input[id$='_filter_tbPostalCode']" );
    }

    public get showFullGroups(): Locator {
        return this.root.locator( "[id$='_filter_tglShowFullGroups']" );
    }

    public get searchButton(): Locator {
        return this.root.locator( "[id$='_btnSearch']" );
    }

    public get clearButton(): Locator {
        return this.root.locator( "[id$='_btnClear']" );
    }

    /** Types a keyword and leaves the box, which with Auto Filter on runs the search. */
    public async typeKeyword( text: string ): Promise<void> {
        await this.keyword.fill( text );
        await this.keyword.press( "Tab" );
        await this.settle();
    }

    /** Checks a filter check box, which with Auto Filter on runs the search. */
    public async check( checkBox: Locator ): Promise<void> {
        await checkBox.check();
        await this.settle();
    }

    public async click( button: Locator ): Promise<void> {
        await button.click();
        await this.settle();
    }

    private async settle(): Promise<void> {
        await waitForPostback( this.page );
        await expectNoRockError( this.page );
    }
}
