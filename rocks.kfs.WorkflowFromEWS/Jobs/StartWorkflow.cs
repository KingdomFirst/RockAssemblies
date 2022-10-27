// <copyright>
// Copyright 2022 by Kingdom First Solutions
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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;

using Microsoft.Exchange.WebServices.Data;
using Microsoft.Identity.Client;
using Quartz;

using Rock;
using Rock.Attribute;
using Rock.Data;
using Rock.Model;
using Rock.Security;
using Rock.Web.Cache;

namespace rocks.kfs.WorkflowFromEWS.Jobs
{
    [EncryptedTextField( "ApplicationId", "The Application (client) ID in Microsoft Azure for the registered application that has access to the target Email Address.", order: 0 )]
    [EncryptedTextField( "TenantId", "The Directory (tenant) ID in Microsoft Azure for the registered application that has access to the target Email Address.", order: 1 )]
    [EncryptedTextField( "Application Secret", "The Secret Value in Microsoft Azure for the registered application that has access to the target Email Address.", order: 2 )]
    [TextField( "Email Address", "The email address for the authenticated user to check.", order: 3 )]
    [UrlLinkField( "Server Url", "", defaultValue: "https://outlook.office365.com/EWS/Exchange.asmx", order: 4 )]
    [IntegerField( "Max Emails", "The maximum number of emails to process each time the job runs.", defaultValue: 100, order: 5 )]
    [BooleanField( "Delete Messages", "Each message will be deleted after it is processed.", false, order: 6 )]
    [BooleanField( "One Workflow Per Conversation", "If a workflow has already been created for a message in this conversation, additional workflows be not created. For example, replies will not activate new workflows.", false, order: 7 )]
    [WorkflowTypeField( "Workflow Type", "The workflow type to be initiated for each message.", required: true, order: 8 )]
    [KeyValueListField( "Workflow Attributes", "Used to match the email properties to the new workflow.", true, keyPrompt: "Attribute Key", valuePrompt: "Email Property", customValues: "DateReceived^Date Received,FromEmail^From Email,FromName^From Name,Subject^Subject,Body^Body", displayValueFirst: true, order: 9 )]

    /// <summary>
    /// Job to create workflow using Exchange Web Services.
    /// </summary>
    [DisallowConcurrentExecution]
    public class StartWorkflow : IJob
    {
        /// <summary>
        /// Empty constructor for job initialization
        /// <para>
        /// Jobs require a public empty constructor so that the
        /// scheduler can instantiate the class whenever it needs.
        /// </para>
        /// </summary>
        public StartWorkflow()
        {
        }

        /// <summary>
        /// Job that will send scheduled group emails.
        ///
        /// Called by the <see cref="IScheduler" /> when a
        /// <see cref="ITrigger" /> fires that is associated with
        /// the <see cref="IJob" />.
        /// </summary>
        public virtual void Execute( IJobExecutionContext context )
        {
            var dataMap = context.JobDetail.JobDataMap;
            var messages = new List<string>();
            var workflowsStarted = 0;
            var workflowTypeGuid = dataMap.GetString( "WorkflowType" ).AsGuidOrNull();

            if ( workflowTypeGuid.HasValue )
            {
                var workflowType = WorkflowTypeCache.Get( workflowTypeGuid.Value );

                if ( workflowType != null )
                {
                    var applicationId = Encryption.DecryptString( dataMap.GetString( "ApplicationId" ) );
                    var tenantId = Encryption.DecryptString( dataMap.GetString( "TenantId" ) );
                    var appSecret = Encryption.DecryptString( dataMap.GetString( "ApplicationSecret" ) );
                    var emailAddress = dataMap.GetString( "EmailAddress" );
                    var url = new Uri( dataMap.GetString( "ServerUrl" ) );
                    var maxEmails = dataMap.GetString( "MaxEmails" ).AsInteger();
                    var delete = dataMap.GetString( "DeleteMessages" ).AsBoolean();
                    var onePer = dataMap.GetString( "OneWorkflowPerConversation" ).AsBoolean();

                    Dictionary<string, string> attributeKeyMap = null;
                    var workflowAttributeKeys = dataMap.GetString( "WorkflowAttributes" );
                    if ( workflowAttributeKeys.IsNotNullOrWhiteSpace() )
                    {
                        attributeKeyMap = workflowAttributeKeys.AsDictionaryOrNull();
                    }

                    attributeKeyMap = attributeKeyMap ?? new Dictionary<string, string>();

                    try
                    {
                        var oauthCreds = GetOauthCreds( applicationId, tenantId, appSecret );
                        using ( var rockContext = new RockContext() )
                        {
                            var service = new ExchangeService();
                            service.Credentials = oauthCreds;

                            //Impersonate the mailbox we want to access.
                            service.ImpersonatedUserId = new ImpersonatedUserId( ConnectingIdType.SmtpAddress, emailAddress );
                            service.TraceEnabled = true;
                            service.TraceFlags = TraceFlags.All;
                            service.Url = url;

                            var findItemPropertySet = new PropertySet( BasePropertySet.IdOnly );
                            findItemPropertySet.Add( ItemSchema.DateTimeReceived );

                            var userMailbox = new Mailbox( emailAddress );
                            var folderId = new FolderId( WellKnownFolderName.Inbox, userMailbox );
                            var sf = new SearchFilter.SearchFilterCollection( LogicalOperator.And, new SearchFilter.IsEqualTo( EmailMessageSchema.IsRead, false ) );
                            var view = new ItemView( maxEmails );
                            view.PropertySet = findItemPropertySet;
                            view.OrderBy.Add( ItemSchema.DateTimeReceived, SortDirection.Descending );
                            var findResults = service.FindItems( folderId, sf, view );

                            if ( findResults.Items.Count > 0 )
                            {
                                var getMessagePropertySet = new PropertySet( BasePropertySet.FirstClassProperties );
                                //getMessagePropertySet.Add( ItemSchema.Attachments ); // for future

                                foreach ( var item in findResults )
                                {
                                    var message = EmailMessage.Bind( service, item.Id, getMessagePropertySet );
                                    message.Load();

                                    var foreignKey = string.Empty;
                                    var existingWorkflow = new Workflow();
                                    var createWorkflow = true;

                                    if ( onePer )
                                    {
                                        var conversationId = message.ConversationId.UniqueId.ToString();

                                        if ( conversationId.Length > 100 )
                                        {
                                            foreignKey = conversationId.Substring( conversationId.Length - 100, 100 );
                                        }
                                        else
                                        {
                                            foreignKey = conversationId;
                                        }

                                        existingWorkflow = new WorkflowService( rockContext )
                                            .Queryable()
                                            .AsNoTracking()
                                            .FirstOrDefault( w => w.ActivatedDateTime.HasValue && !w.CompletedDateTime.HasValue && w.ForeignKey.Equals( foreignKey, StringComparison.Ordinal ) );

                                        createWorkflow = existingWorkflow.IsNull();
                                    }
                                    else
                                    {
                                        var emailId = message.Id.UniqueId.ToString();

                                        if ( emailId.Length > 100 )
                                        {
                                            foreignKey = emailId.Substring( emailId.Length - 100, 100 );
                                        }
                                        else
                                        {
                                            foreignKey = emailId;
                                        }
                                    }

                                    if ( createWorkflow )
                                    {
                                        var workflow = Rock.Model.Workflow.Activate( workflowType, string.Format( "{0} <{1}> [{2}]", message.From.Name.ToStringSafe(), message.From.Address.ToStringSafe(), message.DateTimeReceived.ToShortDateTimeString() ) );
                                        workflow.ForeignKey = foreignKey;

                                        workflow.LoadAttributes( rockContext );

                                        foreach ( var keyPair in attributeKeyMap )
                                        {
                                            var value = string.Empty;

                                            switch ( keyPair.Value )
                                            {
                                                case "DateReceived":
                                                    value = message.DateTimeReceived.ToString( "o", CultureInfo.CreateSpecificCulture( "en-US" ) );
                                                    break;
                                                case "FromEmail":
                                                    value = message.From.Address.ToStringSafe();
                                                    break;
                                                case "FromName":
                                                    value = message.From.Name.ToStringSafe();
                                                    break;
                                                case "Subject":
                                                    value = message.Subject.ToStringSafe();
                                                    break;
                                                case "Body":
                                                    value = message.Body.Text;
                                                    break;
                                                default:
                                                    break;
                                            }

                                            if ( workflow.Attributes.ContainsKey( keyPair.Key ) )
                                            {
                                                workflow.SetAttributeValue( keyPair.Key, value );
                                            }
                                            else
                                            {
                                                messages.Add( string.Format( "'{0}' is not an attribute key in the activated workflow: '{1}'", keyPair.Key, workflow.Name ) );
                                            }
                                        }

                                        List<string> workflowErrorMessages = new List<string>();
                                        new Rock.Model.WorkflowService( rockContext ).Process( workflow, out workflowErrorMessages );
                                        messages.AddRange( workflowErrorMessages );
                                        workflowsStarted++;
                                    }
                                    else if ( existingWorkflow?.Id > 0 )
                                    {
                                        existingWorkflow.LoadAttributes();

                                        foreach ( var keyPair in attributeKeyMap )
                                        {
                                            if ( keyPair.Value.Contains( "Body" ) )
                                            {
                                                var newValue = message.Body.Text;

                                                if ( existingWorkflow.Attributes.ContainsKey( keyPair.Key ) )
                                                {
                                                    var existingValue = existingWorkflow.GetAttributeValue( keyPair.Key );
                                                    var value = string.Format( "{0}{1}{2}", existingValue, Environment.NewLine, newValue );
                                                    existingWorkflow.SetAttributeValue( keyPair.Key, value );
                                                }
                                                else
                                                {
                                                    messages.Add( string.Format( "'{0}' is not an attribute key in the activated workflow: '{1}'", keyPair.Key, existingWorkflow.Name ) );
                                                }
                                            }
                                        }

                                        existingWorkflow.SaveAttributeValues();

                                        messages.Add( string.Format( "{0} workflow was appended.", existingWorkflow.Name ) );
                                    }

                                    message.IsRead = true;
                                    message.Update( ConflictResolutionMode.AlwaysOverwrite, true );

                                    if ( delete )
                                    {
                                        message.Delete( DeleteMode.MoveToDeletedItems );
                                    }
                                }
                            }

                            if ( workflowsStarted > 0 )
                            {
                                messages.Add( string.Format( "Started {0} {1}", workflowsStarted, "workflow".PluralizeIf( workflowsStarted > 1 ) ) );
                            }
                            else
                            {
                                messages.Add( "No workflows started" );
                            }

                            var results = new StringBuilder();
                            foreach ( var message in messages )
                            {
                                results.AppendLine( message );
                            }
                            context.Result = results.ToString();
                        }
                    }
                    catch ( System.Exception ex )
                    {
                        var context2 = HttpContext.Current;
                        ExceptionLogService.LogException( ex, context2 );
                        throw;
                    }
                }
                else
                {
                    context.Result = "No valid workflow type found.";
                }
            }
            else
            {
                context.Result = "Valid workflow type guid was not set.";
            }
        }


        private OAuthCredentials GetOauthCreds( string applicationId, string tenantId, string secret )
        {
            // Using Microsoft.Identity.Client 4.22.0
            var cca = ConfidentialClientApplicationBuilder
            .Create( applicationId )
            .WithClientSecret( secret )
            .WithTenantId( tenantId )
            .Build();

            // The permission scope required for EWS access
            var ewsScopes = new string[] { "https://outlook.office365.com/.default" };

            //Make the token request
            var authResult = cca.AcquireTokenForClient( ewsScopes ).ExecuteAsync().Result;

            var oauthCreds = new OAuthCredentials( authResult.AccessToken );
            return oauthCreds;
        }
    }
}
