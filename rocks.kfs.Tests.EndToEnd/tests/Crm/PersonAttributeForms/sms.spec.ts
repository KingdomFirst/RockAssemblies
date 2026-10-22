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
import { RockEntity } from "../../../shared/rockApi";
import { openPageAsPerson } from "../../../shared/rockBrowser";
import { setCheckBox } from "../../../shared/rockControls";
import { SystemGuid } from "../../../shared/systemGuids";
import { PersonAttributeFormsFixture, PersonFieldType, expect, test } from "./personAttributeForms.fixture";

/** A 555 number, never a real line. */
const testMobileNumber = "6155550187";

/*
    The "Display SMS Checkbox on Mobile Phone" setting and what the checkbox saves: the
    mobile number, with SMS (messaging) enabled or not. Each test starts with the person
    having no mobile number.
*/
test.describe( "Person Attribute Forms Advanced: SMS checkbox", () => {
    test.beforeEach( async ( { pafa } ) => {
        await pafa.configure( { Forms: pafa.buildCustomFormJson( [ { field: PersonFieldType.MobilePhone } ] ) } );
        for ( const phone of await getMobilePhones( pafa ) ) {
            await pafa.api.delete( "PhoneNumbers", phone.Id );
        }
    } );

    test( "PageLoad_SmsCheckboxSettingOff_HidesSmsCheckbox", async ( { page, pafa, block } ) => {
        await openPageAsPerson( page, pafa.api, { pageId: pafa.blockPage.pageId, personId: pafa.person.id } );

        await expect( block.mobilePhoneInput ).toBeVisible();
        await expect( block.smsCheckBox ).toBeHidden();
    } );

    test( "PageLoad_SmsCheckboxSettingOn_ShowsSmsCheckbox", async ( { page, pafa, block } ) => {
        await pafa.configure( { Forms: pafa.buildCustomFormJson( [ { field: PersonFieldType.MobilePhone } ] ), DisplaySMSCheckboxonMobilePhone: "True" } );

        await openPageAsPerson( page, pafa.api, { pageId: pafa.blockPage.pageId, personId: pafa.person.id } );

        await expect( block.mobilePhoneInput ).toBeVisible();
        await expect( block.smsCheckBox ).toBeVisible();
    } );

    test( "Submit_MobilePhoneWithSmsChecked_SavesNumberWithMessagingEnabled", async ( { page, pafa, block } ) => {
        await pafa.configure( { Forms: pafa.buildCustomFormJson( [ { field: PersonFieldType.MobilePhone } ] ), DisplaySMSCheckboxonMobilePhone: "True" } );

        await openPageAsPerson( page, pafa.api, { pageId: pafa.blockPage.pageId, personId: pafa.person.id } );
        await block.mobilePhoneInput.fill( testMobileNumber );
        await setCheckBox( page, block.smsCheckBox, true );
        await block.finish();

        const phones = await getMobilePhones( pafa );
        expect( phones ).toHaveLength( 1 );
        expect( phones[ 0 ].Number ).toBe( testMobileNumber );
        expect( phones[ 0 ].IsMessagingEnabled, "SMS checked should enable messaging on the number" ).toBe( true );
    } );

    test( "Submit_SmsUncheckedOnMessagingNumber_DisablesMessaging", async ( { page, pafa, block } ) => {
        await pafa.configure( { Forms: pafa.buildCustomFormJson( [ { field: PersonFieldType.MobilePhone } ] ), DisplaySMSCheckboxonMobilePhone: "True" } );
        await pafa.api.post( "/api/PhoneNumbers", {
            PersonId: pafa.person.id,
            Number: testMobileNumber,
            CountryCode: "1",
            NumberTypeValueId: await pafa.api.getIdByGuid( "DefinedValues", SystemGuid.DefinedValue.PhoneTypeMobile ),
            IsMessagingEnabled: true,
            IsUnlisted: false,
            IsSystem: false
        } );

        await openPageAsPerson( page, pafa.api, { pageId: pafa.blockPage.pageId, personId: pafa.person.id } );
        await expect( block.smsCheckBox, "The form should show the number's current SMS setting" ).toBeChecked();
        await setCheckBox( page, block.smsCheckBox, false );
        await block.finish();

        const phones = await getMobilePhones( pafa );
        expect( phones ).toHaveLength( 1 );
        expect( phones[ 0 ].IsMessagingEnabled, "SMS unchecked should disable messaging on the number" ).toBe( false );
    } );
} );

async function getMobilePhones( pafa: PersonAttributeFormsFixture ): Promise<RockEntity[]> {
    const mobileTypeId = await pafa.api.getIdByGuid( "DefinedValues", SystemGuid.DefinedValue.PhoneTypeMobile );
    return await pafa.api.query( "PhoneNumbers", `PersonId eq ${pafa.person.id} and NumberTypeValueId eq ${mobileTypeId}` );
}
