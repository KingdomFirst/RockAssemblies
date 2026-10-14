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
import { EntityTypeName, SystemGuid, TestNamePrefix } from "../systemGuids";

/** Summary put on every test transaction, so leftovers can be found and deleted. */
export const TestTransactionSummary = `${TestNamePrefix} test contribution. Safe to delete.`;

/**
 * Records a contribution toward a group member's fundraising goal, the way Rock's
 * fundraising donation entry does: a Contribution transaction whose detail points at
 * the GroupMember entity. Returns the transaction Id.
 */
export async function createGroupMemberContribution( api: RockApi, args: { groupMemberId: number; authorizedPersonAliasId: number; accountId: number; amount: number; transactionDateTime?: string } ): Promise<number> {
    // Rock 17 requires every transaction to have a payment detail, even an empty one.
    const paymentDetailId = await api.post( "/api/FinancialPaymentDetails", { Guid: randomUUID() } );

    const transactionId = await api.post( "/api/FinancialTransactions", {
        Guid: randomUUID(),
        FinancialPaymentDetailId: paymentDetailId,
        // A Rock local date-time ("yyyy-MM-ddTHH:mm:ss") when the test needs a specific date.
        TransactionDateTime: args.transactionDateTime ?? new Date().toISOString(),
        TransactionTypeValueId: await api.getIdByGuid( "DefinedValues", SystemGuid.DefinedValue.TransactionTypeContribution ),
        AuthorizedPersonAliasId: args.authorizedPersonAliasId,
        Summary: TestTransactionSummary,
        ShowAsAnonymous: false,
        IsReconciled: false,
        IsSettled: false
    } );

    await api.post( "/api/FinancialTransactionDetails", {
        Guid: randomUUID(),
        TransactionId: transactionId,
        AccountId: args.accountId,
        Amount: args.amount,
        FeeAmount: 0,
        EntityTypeId: await api.getEntityTypeId( EntityTypeName.GroupMember ),
        EntityId: args.groupMemberId
    } );

    return transactionId;
}

/** Deletes a transaction, its details and its payment detail. */
export async function deleteTransaction( api: RockApi, transactionId: number ): Promise<void> {
    const paymentDetailId = ( await api.getById( "FinancialTransactions", transactionId ) ).FinancialPaymentDetailId as number | null;

    for ( const detail of await api.query( "FinancialTransactionDetails", `TransactionId eq ${transactionId}` ) ) {
        await api.delete( "FinancialTransactionDetails", detail.Id );
    }

    await api.delete( "FinancialTransactions", transactionId );
    if ( paymentDetailId ) {
        await api.delete( "FinancialPaymentDetails", paymentDetailId );
    }
}

/** Deletes test transactions left behind by an earlier run that did not finish cleanup. */
export async function deleteLeftoverTransactions( api: RockApi ): Promise<void> {
    for ( const transaction of await api.query( "FinancialTransactions", `Summary eq '${TestTransactionSummary}'` ) ) {
        await deleteTransaction( api, transaction.Id );
    }
}
