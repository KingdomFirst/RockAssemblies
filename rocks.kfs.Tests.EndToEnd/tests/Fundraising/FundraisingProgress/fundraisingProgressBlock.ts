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
import { Download, Locator, Page } from "@playwright/test";

/**
 * Page object for RockWeb/Plugins/rocks_kfs/Fundraising/FundraisingProgress.ascx.
 * Selectors match on the server control ID suffix, which is stable across Rock versions
 * even when the generated ClientID prefix changes.
 */
export class FundraisingProgressBlock {
    public readonly root: Locator;

    public constructor( private readonly page: Page, blockId: number ) {
        this.root = page.locator( `#bid_${blockId}` );
    }

    /** The whole progress panel; not rendered when there is no fundraising group to show. */
    public get view(): Locator {
        return this.root.locator( "[id$='_pnlView']" );
    }

    public get title(): Locator {
        return this.root.locator( "[id$='_divPanelHeading'] .panel-title" );
    }

    /** "Total raised year to date" amount. */
    public get totalRaised(): Locator {
        return this.root.locator( "[id$='_divTotalRaised'] .alert-total-raised" );
    }

    /** "Total Individual Goals" section. */
    public get groupTotals(): Locator {
        return this.root.locator( "[id$='_pnlHeader']" );
    }

    /** "$raised/$goal" for the whole group. */
    public get groupTotalAmounts(): Locator {
        return this.root.locator( "[id$='_pTotalAmounts']" );
    }

    public get groupProgressBar(): Locator {
        return this.root.locator( "[id$='_divTotalProgress'] .progress-bar" );
    }

    public get memberRows(): Locator {
        return this.root.locator( "[id$='_ulGroupMembers'] li.list-group-item" );
    }

    public memberRow( fullName: string ): Locator {
        return this.memberRows.filter( { hasText: fullName } );
    }

    /** "$raised/$goal" for one member. */
    public memberAmounts( fullName: string ): Locator {
        return this.memberRow( fullName ).locator( "p.pull-right" );
    }

    public memberProgressBar( fullName: string ): Locator {
        return this.memberRow( fullName ).locator( ".progress-bar" );
    }

    public get exportButton(): Locator {
        return this.root.locator( "[id$='_btnExport']" );
    }

    /** Clicks "Export to Excel" and returns the download. */
    public async exportToExcel(): Promise<Download> {
        const download = this.page.waitForEvent( "download" );
        await this.exportButton.click();
        return await download;
    }
}
