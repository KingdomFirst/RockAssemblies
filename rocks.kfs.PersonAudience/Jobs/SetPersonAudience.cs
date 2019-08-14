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

            using ( var rockContext = new RockContext() )
            {
                rockContext.Database.CommandTimeout = commandTimeout;

                var audienceDataViews = new Dictionary<Guid, List<int>>();
                var audienceDefinedType = DefinedTypeCache.Get( Rock.SystemGuid.DefinedType.MARKETING_CAMPAIGN_AUDIENCE_TYPE );

                foreach ( var audience in audienceDefinedType.DefinedValues )
                {
                    var dv = audience.GetAttributeValue( "AudienceDataView" ).AsGuidOrNull();
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
                    var personAudiencesDictionary = new Dictionary<int, HashSet<Guid>>();

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

                    if ( personAudiencesDictionary.Count > 0 )
                    {
                        var personAudienceAttribute = AttributeCache.Get( "67CD39C1-4AEA-4ED1-AB57-608AD6F7DB8B" );

                        if ( personAudienceAttribute != null )
                        {
                            foreach ( var personAudiences in personAudiencesDictionary )
                            {
                                var person = new PersonService( rockContext ).Get( personAudiences.Key );
                                var audiences = string.Join( ",", personAudiences.Value );

                                var personAttributeValue = rockContext.AttributeValues.Where( v => v.Attribute.Id == personAudienceAttribute.Id && v.EntityId == person.Id ).FirstOrDefault();
                                var newAttributeValue = new AttributeValueCache
                                {
                                    AttributeId = personAudienceAttribute.Id,
                                    Value = audiences
                                };

                                if ( personAttributeValue == null )
                                {
                                    personAttributeValue = new AttributeValue
                                    {
                                        AttributeId = newAttributeValue.AttributeId,
                                        EntityId = person.Id,
                                        Value = newAttributeValue.Value
                                    };

                                    rockContext.AttributeValues.Add( personAttributeValue );
                                }
                                else if ( !personAttributeValue.Value.Equals( newAttributeValue.Value, StringComparison.CurrentCultureIgnoreCase ) )
                                {
                                    personAttributeValue.Value = newAttributeValue.Value;
                                    rockContext.Entry( personAttributeValue ).State = EntityState.Modified;
                                }
                            }
                        }

                        rockContext.ChangeTracker.DetectChanges();
                        rockContext.SaveChanges();
                    }
                }
            }
        }
    }
}
