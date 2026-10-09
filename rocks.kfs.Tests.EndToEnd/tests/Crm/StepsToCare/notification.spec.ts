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
import { waitForPostback } from "../../../shared/rockBrowser";
import { pickRadio } from "../../../shared/rockControls";
import { PluginGuid, expect, test } from "./stepsToCare.fixture";

// Traces are off: see actions.spec.ts.
test.use( { trace: "off" } );

/*
    Covers the manual plan's notification step: the bell in the dashboard's top right offers
    None, Email, SMS and Email & SMS, and the choice is saved (as the person's "Steps to Care
    Notification" attribute).
*/
test.describe( "Steps to Care: notification preference", () => {
    test( "NotificationType_AllFourOptions_ChoiceIsSaved", async ( { page, stc } ) => {
        const attributeId = await stc.api.getIdByGuid( "Attributes", PluginGuid.notificationAttribute );
        const original = await stc.api.getAttributeValue( attributeId, stc.tester.id ) ?? "";
        const choice = original === "SMS" ? "None" : "SMS";

        try {
            const dashboard = await stc.openDashboard( page );
            await dashboard.notificationButton.click();
            await waitForPostback( page );
            const dialog = dashboard.dialog;
            await expect( dialog ).toContainText( "Choose Notification Type" );
            const options = dialog.locator( "input[type='radio']" );
            await expect( options.locator( "xpath=ancestor::label[1]" ) ).toHaveText( [ "None", "Email", "SMS", "Email & SMS" ] );

            await pickRadio( page, dialog.locator( `input[type='radio'][value='${choice}']` ) );
            await dialog.locator( "a[id$='_serverSaveLink']" ).click();
            await waitForPostback( page );
            await expect( dialog, "The dialog closes when saved" ).toBeHidden();

            await expect.poll( () => stc.api.getAttributeValue( attributeId, stc.tester.id ), { message: "Saved as the tester's notification preference" } ).toBe( choice );

            // Shown as saved when the dialog is opened again.
            await stc.openDashboard( page );
            await dashboard.notificationButton.click();
            await waitForPostback( page );
            await expect( dashboard.dialog.locator( `input[type='radio'][value='${choice}']` ) ).toBeChecked();
        }
        finally {
            await stc.api.setAttributeValue( attributeId, stc.tester.id, original );
        }
    } );
} );
