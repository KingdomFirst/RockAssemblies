// <copyright>
// Copyright 2023 by Kingdom First Solutions
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

namespace rocks.kfs.Microsoft365Utilities.Jobs
{
    [EncryptedTextField( "Application Id",
        Description = "The Application (client) ID in Microsoft Azure for the registered application that has access to the target Email Address.",
        Order = 0,
        Key = AttributeKey.ApplicationId )]

    [EncryptedTextField( "Tenant Id",
        Description = "The Directory (tenant) ID in Microsoft Azure for the registered application that has access to the target Email Address.",
        Order = 1,
        Key = AttributeKey.TenantId )]

    [EncryptedTextField( "Application Secret",
        Description = "The Secret Value in Microsoft Azure for the registered application that has access to the target Email Address.",
        Order = 2,
        Key = AttributeKey.ApplicationSecret )]

    [TextField( "Email Address",
        Description = "The email address for the authenticated user to check.",
        Order = 3,
        Key = AttributeKey.EmailAddress )]

    [TextField( "Impersonate User",
        Description = "The email address of the account to use for impersonation. This account must have access to the inbox provided in the Email Address setting. If left blank, the Email Address will be used.",
        Order = 4,
        Key = AttributeKey.ImpersonateUser )]

    [UrlLinkField( "Server Url",
        DefaultValue = "https://outlook.office365.com/EWS/Exchange.asmx",
        Order = 5,
        Key = AttributeKey.ServerUrl )]

    [IntegerField( "Max Emails",
        Description = "The maximum number of emails to process each time the job runs.",
        DefaultIntegerValue = 100,
        Order = 6,
        Key = AttributeKey.MaxEmails )]

    [EnumsField( "Launch Workflows with",
        Description = "What emails should this job use to launch workflows? If none are selected, any items within the inbox provided in the Email Address setting will be processed. Default: Unread (When multiple options are selected these are a combined search result, i.e Unread AND Flagged)",
        IsRequired = false,
        EnumSourceType = typeof( ProcessEmailsBy ),
        Order = 7,
        Key = AttributeKey.LaunchWorkflowsWith )]

    [EnumsField( "Mark Processed Emails by",
        Description = "How should the emails be marked within EWS once they are processed. Default: Read (Note: not all options will work without appropriate permissions. Multiple options will perform all that it can in order presented.)",
        DefaultValue = "0",
        EnumSourceType = typeof( MarkEmailBy ),
        Order = 8,
        Key = AttributeKey.MarkEmailsBy )]

    [BooleanField( "One Workflow Per Conversation",
        Description = "If a workflow has already been created for a message in this conversation, additional workflows be not created. For example, replies will not activate new workflows.",
        DefaultBooleanValue = true,
        Order = 9,
        Key = AttributeKey.OneWorkflowPerConversation )]

    [WorkflowTypeField( "Workflow Type",
        Description = "The workflow type to be initiated for each message.",
        IsRequired = true,
        Order = 10,
        Key = AttributeKey.WorkflowType )]

    [KeyValueListField( "Workflow Attributes",
        description: "Used to match the email properties to the new workflow.",
        required: true,
        keyPrompt: "Attribute Key",
        valuePrompt: "Email Property",
        customValues: "DateReceived^Date Received,FromEmail^From Email,FromName^From Name,Subject^Subject,Body^Body",
        displayValueFirst: true,
        order: 11,
        key: AttributeKey.WorkflowAttributes )]


    /// <summary>
    /// Job to create workflow using Exchange Web Services.
    /// </summary>
    [DisallowConcurrentExecution]
    public class LaunchWorkflowFromEWSAccount : IJob
    {
        private static class AttributeKey
        {
            public const string ApplicationId = "ApplicationId";
            public const string TenantId = "TenantId";
            public const string ApplicationSecret = "ApplicationSecret";
            public const string EmailAddress = "EmailAddress";
            public const string ImpersonateUser = "ImpersonateUser";
            public const string ServerUrl = "ServerUrl";
            public const string MaxEmails = "MaxEmails";
            public const string OneWorkflowPerConversation = "OneWorkflowPerConversation";
            public const string WorkflowType = "WorkflowType";
            public const string WorkflowAttributes = "WorkflowAttributes";
            public const string LaunchWorkflowsWith = "LaunchWorkflowsWith";
            public const string MarkEmailsBy = "MarkEmailsBy";
        }

        /// <summary>
        /// Empty constructor for job initialization
        /// <para>
        /// Jobs require a public empty constructor so that the
        /// scheduler can instantiate the class whenever it needs.
        /// </para>
        /// </summary>
        public LaunchWorkflowFromEWSAccount()
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
            var workflowTypeGuid = dataMap.GetString( AttributeKey.WorkflowType ).AsGuidOrNull();

            if ( workflowTypeGuid.HasValue )
            {
                var workflowType = WorkflowTypeCache.Get( workflowTypeGuid.Value );

                if ( workflowType != null )
                {
                    var applicationId = Encryption.DecryptString( dataMap.GetString( AttributeKey.ApplicationId ) );
                    var tenantId = Encryption.DecryptString( dataMap.GetString( AttributeKey.TenantId ) );
                    var appSecret = Encryption.DecryptString( dataMap.GetString( AttributeKey.ApplicationSecret ) );
                    var emailAddress = dataMap.GetString( AttributeKey.EmailAddress );
                    var impersonate = dataMap.GetString( AttributeKey.ImpersonateUser );
                    if ( impersonate.IsNullOrWhiteSpace() )
                    {
                        impersonate = emailAddress;
                    }
                    var url = new Uri( dataMap.GetString( AttributeKey.ServerUrl ) );
                    var maxEmails = dataMap.GetString( AttributeKey.MaxEmails ).AsInteger();
                    var onePer = dataMap.GetString( AttributeKey.OneWorkflowPerConversation ).AsBoolean();
                    var launchWith = dataMap.GetString( AttributeKey.LaunchWorkflowsWith ).StringToIntList();
                    var markWith = dataMap.GetString( AttributeKey.MarkEmailsBy ).StringToIntList();

                    Dictionary<string, string> attributeKeyMap = null;
                    var workflowAttributeKeys = dataMap.GetString( AttributeKey.WorkflowAttributes );
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
                            service.ImpersonatedUserId = new ImpersonatedUserId( ConnectingIdType.SmtpAddress, impersonate );
                            service.TraceEnabled = true;
                            service.TraceFlags = TraceFlags.All;
                            service.Url = url;

                            var findItemPropertySet = new PropertySet( BasePropertySet.IdOnly );
                            findItemPropertySet.Add( ItemSchema.DateTimeReceived );

                            var userMailbox = new Mailbox( emailAddress );
                            var folderId = new FolderId( WellKnownFolderName.Inbox, userMailbox );
                            var sf = new SearchFilter.SearchFilterCollection( LogicalOperator.And );

                            if ( launchWith.Contains( ( ( int ) ProcessEmailsBy.Unread ) ) )
                            {
                                sf.Add( new SearchFilter.IsEqualTo( EmailMessageSchema.IsRead, false ) );
                            }
                            if ( launchWith.Contains( ( ( int ) ProcessEmailsBy.Flagged ) ) )
                            {
                                ExtendedPropertyDefinition pidTagFlagStatus = new ExtendedPropertyDefinition( 0x1090, MapiPropertyType.Integer );
                                SearchFilter Flagged = new SearchFilter.IsEqualTo( pidTagFlagStatus, 2 );
                                sf.Add( Flagged );
                            }
                            if ( launchWith.Contains( ( ( int ) ProcessEmailsBy.FlaggedComplete ) ) )
                            {
                                ExtendedPropertyDefinition pidTagTaskStatus = new ExtendedPropertyDefinition( DefaultExtendedPropertySet.Task, 0x8101, MapiPropertyType.Integer );
                                SearchFilter SetComplete = new SearchFilter.IsEqualTo( pidTagTaskStatus, 2 );
                                sf.Add( SetComplete );
                            }
                            if ( launchWith.Contains( ( ( int ) ProcessEmailsBy.HasAttachments ) ) )
                            {
                                sf.Add( new SearchFilter.IsEqualTo( EmailMessageSchema.HasAttachments, true ) );
                            }
                            var view = new ItemView( maxEmails );
                            view.PropertySet = findItemPropertySet;
                            view.OrderBy.Add( ItemSchema.DateTimeReceived, SortDirection.Descending );
                            FindItemsResults<Item> findResults = null;
                            if ( sf.Count > 0 )
                            {
                                findResults = service.FindItems( folderId, sf, view );
                            }
                            else
                            {
                                findResults = service.FindItems( folderId, view );
                            }

                            if ( findResults.Items.Count > 0 )
                            {
                                var archiveItemIds = new List<ItemId>();

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

                                    if ( markWith.Contains( ( ( int ) MarkEmailBy.Read ) ) )
                                    {
                                        message.IsRead = true;
                                    }
                                    if ( markWith.Contains( ( ( int ) MarkEmailBy.RemoveFlag ) ) )
                                    {
                                        message.Flag = new Flag { FlagStatus = ItemFlagStatus.NotFlagged };
                                    }
                                    if ( markWith.Contains( ( ( int ) MarkEmailBy.AddFlag ) ) )
                                    {
                                        message.Flag = new Flag { FlagStatus = ItemFlagStatus.Flagged };
                                    }
                                    if ( markWith.Contains( ( ( int ) MarkEmailBy.MarkComplete ) ) && message.Flag != null )
                                    {
                                        var flag = message.Flag;
                                        flag.CompleteDate = RockDateTime.Now;
                                        flag.FlagStatus = ItemFlagStatus.Complete;
                                        message.Flag = flag;
                                    }
                                    // Most users will receive error "Archive mailbox is not enabled for this user."
                                    // This archive is actual mailbox archiving, not just move to archive folder.
                                    // Unable to test feature, requires Exchange Online Plan 2 or Exchange Online Archiving license, add note to documentation or remove?
                                    //if ( markWith.Contains( ( ( int ) MarkEmailBy.Archive ) ) )
                                    //{
                                    //    archiveItemIds.Add( message.Id );
                                    //}

                                    message.Update( ConflictResolutionMode.AlwaysOverwrite, true );

                                    if ( markWith.Contains( ( ( int ) MarkEmailBy.Delete ) ) )
                                    {
                                        message.Delete( DeleteMode.MoveToDeletedItems );
                                    }
                                }
                                //if ( archiveItemIds.Any() )
                                //{
                                //    service.ArchiveItems( archiveItemIds, folderId );
                                //}
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

    public enum ProcessEmailsBy
    {
        Unread,
        Flagged,
        FlaggedComplete,
        HasAttachments
    }

    public enum MarkEmailBy
    {
        Read,
        AddFlag,
        RemoveFlag,
        MarkComplete,
        Delete
    }
}
