// <copyright>
// Copyright 2019 by Kingdom First Solutions
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
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

using Quartz;

using Rock;
using Rock.Attribute;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;

namespace rocks.kfs.PersonAudience.Jobs
{
    [IntegerField( "Command Timeout", "Maximum amount of time (in seconds) to wait for the SQL Query to complete.", true, 180 )]

    /// <summary>
    /// Job to set the audience attribute value on every person record.
    /// </summary>
    [DisallowConcurrentExecution]
    public class SetPersonAudience : IJob
    {
        /// <summary>
        /// Empty constructor for job initialization
        /// <para>
        /// Jobs require a public empty constructor so that the
        /// scheduler can instantiate the class whenever it needs.
        /// </para>
        /// </summary>
        public SetPersonAudience()
        {
        }

        /// <summary>
        /// Job to inactivate groups based on group attribute values.
        ///
        /// Called by the <see cref="IScheduler" /> when a
        /// <see cref="ITrigger" /> fires that is associated with
        /// the <see cref="IJob" />.
        /// </summary>
        public virtual void Execute( IJobExecutionContext context )
        {
            JobDataMap dataMap = context.JobDetail.JobDataMap;
            var commandTimeout = dataMap.GetString( "CommandTimeout" ).AsIntegerOrNull() ?? 180;
            var results = new StringBuilder();

            var personAudiencesDictionary = new Dictionary<int, HashSet<Guid>>();

            using ( var rockContext = new RockContext() )
            {
                rockContext.Database.CommandTimeout = commandTimeout;

                var audienceDataViews = new Dictionary<Guid, List<int>>();
                var audienceDefinedType = DefinedTypeCache.Get( Rock.SystemGuid.DefinedType.MARKETING_CAMPAIGN_AUDIENCE_TYPE );

                foreach ( var audience in audienceDefinedType.DefinedValues )
                {
                    var dv = audience.GetAttributeValue( "rocks.kfs.AudienceDataView" ).AsGuidOrNull();
                    if ( dv.HasValue )
                    {
                        var dataView = new DataViewService( rockContext ).Get( dv.Value );

                        List<IEntity> resultSet = null;
                        var errorMessages = new List<string>();
                        try
                        {
                            var qry = dataView.GetQuery( null, rockContext, commandTimeout, out errorMessages );
                            if ( qry != null )
                            {
                                resultSet = qry.AsNoTracking().ToList();
                            }
                        }
                        catch ( Exception exception )
                        {
                            ExceptionLogService.LogException( exception, HttpContext.Current );
                            while ( exception != null )
                            {
                                if ( exception is SqlException && ( exception as SqlException ).Number == -2 )
                                {
                                    // if there was a SQL Server Timeout, have the warning be a friendly message about that.
                                    errorMessages.Add( "This dataview did not complete in a timely manner. You can try again or adjust the timeout setting of this job." );
                                    exception = exception.InnerException;
                                }
                                else
                                {
                                    errorMessages.Add( exception.Message );
                                    exception = exception.InnerException;
                                }

                                return;
                            }
                        }

                        if ( resultSet.Count > 0 )
                        {
                            var personIdList = resultSet.Select( p => p.Id ).ToList();
                            audienceDataViews.Add( audience.Guid, personIdList );
                        }
                    }
                }

                if ( audienceDataViews.Count > 0 )
                {
                    foreach ( var dataView in audienceDataViews )
                    {
                        foreach ( var personId in dataView.Value )
                        {
                            if ( personAudiencesDictionary.ContainsKey( personId ) )
                            {
                                var hashSet = personAudiencesDictionary[personId];
                                if ( !hashSet.Contains( dataView.Key ) )
                                {
                                    hashSet.Add( dataView.Key );
                                }
                                personAudiencesDictionary[personId] = hashSet;
                            }
                            else
                            {
                                var hashSet = new HashSet<Guid>();
                                hashSet.Add( dataView.Key );
                                personAudiencesDictionary.Add( personId, hashSet );
                            }
                        }
                    }
                }
                else
                {
                    results.AppendLine( "The Audience Type Defined Values need to be configured with their corresponding DataViews or no people were found." );
                }
            }

            var personAudienceAttributeId = AttributeCache.Get( "67CD39C1-4AEA-4ED1-AB57-608AD6F7DB8B" )?.Id;

            if ( personAudienceAttributeId.HasValue )
            {
                if ( personAudiencesDictionary.Count > 0 )
                {
                    var newRockContext = new RockContext();
                    var attributeId = personAudienceAttributeId.Value;
                    var modified = 0;
                    var totalUpdated = 0;

                    // delete the attribute values for people who are not being updated
                    var peopleToUpdate = personAudiencesDictionary.Keys.ToList();
                    var attributeValuesToDelete = new AttributeValueService( newRockContext )
                        .Queryable()
                        .Where( v => v.AttributeId == attributeId && v.EntityId.HasValue && !peopleToUpdate.Contains( v.EntityId.Value ) );
                    var totalDeleted = newRockContext.BulkDelete( attributeValuesToDelete, 1000 );

                    // update attribute values for everyone in the dataviews
                    foreach ( var personAudiences in personAudiencesDictionary )
                    {
                        var personId = personAudiences.Key;
                        var value = string.Join( ",", personAudiences.Value );

                        var personAttributeValue = newRockContext.AttributeValues.Where( v => v.Attribute.Id == attributeId && v.EntityId == personId ).FirstOrDefault();

                        if ( personAttributeValue == null )
                        {
                            modified++;
                            totalUpdated++;

                            personAttributeValue = new AttributeValue
                            {
                                AttributeId = attributeId,
                                EntityId = personId,
                                Value = value
                            };

                            newRockContext.AttributeValues.Add( personAttributeValue );
                        }
                        else if ( !personAttributeValue.Value.Equals( value, StringComparison.OrdinalIgnoreCase ) )
                        {
                            modified++;
                            totalUpdated++;

                            personAttributeValue.Value = value;
                            newRockContext.Entry( personAttributeValue ).State = EntityState.Modified;
                        }

                        // save every 100 changes
                        if ( modified == 100 )
                        {
                            newRockContext.SaveChanges();

                            foreach ( var dbEntityEntry in newRockContext.ChangeTracker.Entries() )
                            {
                                if ( dbEntityEntry.Entity != null )
                                {
                                    dbEntityEntry.State = EntityState.Detached;
                                }
                            }

                            modified = 0;
                        }
                    }

                    // save any leftover
                    newRockContext.SaveChanges();

                    results.AppendLine( $"{ totalUpdated } {"person".PluralizeIf( totalUpdated != 1 )} updated and { totalDeleted } attribute values deleted." );
                }
            }

            context.Result = results.ToString();
        }
    }
}
