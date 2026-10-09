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
import { RockApi, RockEntity } from "../rockApi";

/** Rock's Scheduled Job List block type (Jobs Administration page), whose Run Now action the tests call. */
const ScheduledJobListBlockTypeGuid = "9B90F2D1-0C7B-4F08-A808-8BA4C9A70A20";

export type TestJob = {
    id: number;
    guid: string;
};

/*
    10/5/2026 - CLAUDE

    Rock has no REST endpoint that runs a job. "Run Now" is an action on the
    Scheduled Job List block (Jobs Administration), and Rock's v2 block action
    API accepts the REST key, so the tests press that same button through the
    API: POST api/v2/BlockActions/{page}/{block}/RunNow. Test jobs are created
    inactive, so Rock's scheduler never runs them on its own.

    Reason: Run jobs the way an admin does, without a scheduler race.
*/

/** Finds or creates an inactive job of the given class, by fixed Guid. */
export async function ensureTestJob( api: RockApi, args: { guid: string; name: string; jobClass: string } ): Promise<TestJob> {
    const existing = await api.getByGuid( "ServiceJobs", args.guid );
    if ( existing ) {
        if ( existing.IsActive ) {
            await api.patch( "ServiceJobs", existing.Id, { IsActive: false } );
        }
        return { id: existing.Id, guid: args.guid };
    }

    const id = await api.post( "/api/ServiceJobs", {
        Guid: args.guid,
        Name: args.name,
        Description: "Created by the KFS end-to-end tests and run only by them (inactive). Do not activate.",
        Class: args.jobClass,
        // Never fires on its own; the job is also inactive.
        CronExpression: "0 0 0 1 1 ? 2099",
        IsActive: false,
        IsSystem: false,
        NotificationStatus: 4
    } );

    return { id, guid: args.guid };
}

/** The job type's settings (attributes on ServiceJob qualified by its class), Ids keyed by Key. */
export async function getJobSettingIds( api: RockApi, jobClass: string ): Promise<Record<string, number>> {
    const jobEntityTypeId = await api.getEntityTypeId( "Rock.Model.ServiceJob" );
    const ids: Record<string, number> = {};
    for ( const attribute of await api.query( "Attributes", `EntityTypeId eq ${jobEntityTypeId} and EntityTypeQualifierColumn eq 'Class' and EntityTypeQualifierValue eq '${jobClass}'` ) ) {
        ids[ attribute.Key as string ] = attribute.Id;
    }

    return ids;
}

/**
 * Sets the job's last run so its next run only picks up items scheduled after `since`.
 * The Scheduled Group Communication jobs send items dated from the last run (minus its
 * duration and a buffer) to now; a job that never ran looks back a whole day, which
 * would resend real groups' messages.
 */
export async function setJobLastRun( api: RockApi, job: TestJob, since: string ): Promise<void> {
    await api.patch( "ServiceJobs", job.id, { LastRunDateTime: since, LastRunDurationSeconds: 0 } );
}

/** Presses Run Now for the job (as an admin would on Jobs Administration) and waits for the run to finish. */
export async function runJobNow( api: RockApi, job: TestJob, options: { timeoutMs?: number } = {} ): Promise<RockEntity> {
    const before = await api.getById( "ServiceJobs", job.id );
    const previousRun = String( before.LastRunDateTime ?? "" );

    const blockTypeId = await api.getIdByGuid( "BlockTypes", ScheduledJobListBlockTypeGuid );
    const block = await api.first( "Blocks", `BlockTypeId eq ${blockTypeId}` );
    if ( !block || !block.PageId ) {
        throw new Error( "No Scheduled Job List block (Jobs Administration) found on the site; the tests use its Run Now action to run jobs." );
    }
    const page = await api.getById( "Pages", block.PageId as number );

    await api.postJson( `/api/v2/BlockActions/${page.Guid}/${block.Guid}/RunNow`, { key: String( job.id ) } );

    const deadline = Date.now() + ( options.timeoutMs ?? 180_000 );
    while ( Date.now() < deadline ) {
        await new Promise( resolve => setTimeout( resolve, 2_000 ) );
        const current = await api.getById( "ServiceJobs", job.id );
        const lastRun = String( current.LastRunDateTime ?? "" );
        if ( lastRun && lastRun !== previousRun && current.LastStatus ) {
            return current;
        }
    }

    throw new Error( `Job ${job.id} did not finish a run within ${( options.timeoutMs ?? 180_000 ) / 1000}s after Run Now.` );
}
