using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;

using Rock;
using Rock.Attribute;
using Rock.Communication;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;
using Rock.Workflow;

namespace com.kfs.Workflow.Action.Communications
{
    /// <summary>
    /// Sends advanced email
    /// </summary>
    [ActionCategory( "Communications" )]
    [Description( "Sends an advanced email. The recipient can either be a group, person or email address determined by the 'To Attribute' value, or an email address entered in the 'To' field. Only people with an active email address without the 'Do Not Email' preference are included. If attribute is a group, only members with an <em>Active</em> member status are included." )]
    [Export( typeof( ActionComponent ) )]
    [ExportMetadata( "ComponentName", "Email Send Advanced" )]
    [WorkflowTextOrAttribute( "From Email Address", "Attribute Value", "The email address or an attribute that contains the person or email address that email should be sent from (will default to organization email). <span class='tip tip-lava'></span>", false, "", "", 0, "From",
        new string[] { "Rock.Field.Types.TextFieldType", "Rock.Field.Types.EmailFieldType", "Rock.Field.Types.PersonFieldType" } )]
    [WorkflowTextOrAttribute( "Send To Email Addresses", "Attribute Value", "The email addresses or an attribute that contains the person or email address that email should be sent to. <span class='tip tip-lava'></span>", true, "", "", 1, "To",
        new string[] { "Rock.Field.Types.TextFieldType", "Rock.Field.Types.EmailFieldType", "Rock.Field.Types.PersonFieldType", "Rock.Field.Types.GroupFieldType", "Rock.Field.Types.SecurityRoleFieldType" } )]
    [WorkflowAttribute( "Send to Group Role", "An optional Group Role attribute to limit recipients to if the 'Send to Email Address' is a group or security role.", false, "", "", 2, "GroupRole",
        new string[] { "Rock.Field.Types.GroupRoleFieldType" } )]
    [TextField( "Subject", "The subject that should be used when sending email. <span class='tip tip-lava'></span>", false, "", "", 3 )]
    [CodeEditorField( "Body", "The body of the email that should be sent. <span class='tip tip-lava'></span> <span class='tip tip-html'></span>", Rock.Web.UI.Controls.CodeEditorMode.Html, Rock.Web.UI.Controls.CodeEditorTheme.Rock, 200, false, "", "", 4 )]
    [BooleanField( "Save Communication History", "Should a record of this communication be saved to the recipient's profile", false, "", 5 )]
    [WorkflowTextOrAttribute( "Reply To Email Address", "Attribute Value", "The email address or an attribute that contains the person or email address that email should reply to (will default to From Email). <span class='tip tip-lava'></span>", false, "", "Advanced", 6, "ReplyTo",
        new string[] { "Rock.Field.Types.TextFieldType", "Rock.Field.Types.EmailFieldType", "Rock.Field.Types.PersonFieldType" } )]
    [WorkflowTextOrAttribute( "CC Email Address", "Attribute Value", "Optional email address or an attribute that contains the person or email address that email should be CC'd to. <span class='tip tip-lava'></span>", false, "", "Advanced", 7, "CC",
        new string[] { "Rock.Field.Types.TextFieldType", "Rock.Field.Types.EmailFieldType", "Rock.Field.Types.PersonFieldType", "Rock.Field.Types.GroupFieldType", "Rock.Field.Types.SecurityRoleFieldType" } )]
    [WorkflowAttribute( "CC to Group Role", "An optional Group Role attribute to limit recipients to if the 'CC Email Address' is a group or security role.", false, "", "Advanced", 8, "CCGroupRole",
        new string[] { "Rock.Field.Types.GroupRoleFieldType" } )]
    [WorkflowTextOrAttribute( "BCC Email Address", "Attribute Value", "Optional email address or an attribute that contains the person or email address that email should be BCC'd to <span class='tip tip-lava'></span>", false, "", "Advanced", 9, "BCC",
        new string[] { "Rock.Field.Types.TextFieldType", "Rock.Field.Types.EmailFieldType", "Rock.Field.Types.PersonFieldType", "Rock.Field.Types.GroupFieldType", "Rock.Field.Types.SecurityRoleFieldType" } )]
    [WorkflowAttribute( "BCC to Group Role", "An optional Group Role attribute to limit recipients to if the 'BCC Email Address' is a group or security role.", false, "", "Advanced", 9, "BCCGroupRole",
        new string[] { "Rock.Field.Types.GroupRoleFieldType" } )]
    [WorkflowTextOrAttribute( "Attachments", "Attribute Value", "A comma separated list of integers or guids of the Binary Files which should be included as attachments.<span class='tip tip-lava'></span>", false, "", "Advanced", 10, "Attachments",
        new string[] { "Rock.Field.Types.TextFieldType", "Rock.Field.Types.BinaryFileFieldType" } )]
    public class SendEmailAdvanced : ActionComponent
    {
        private string replyToEmail;
        private List<string> ccEmails;
        private List<string> bccEmails;
        private List<BinaryFile> attachments;

        /// <summary>
        /// Executes the specified workflow.
        /// </summary>
        /// <param name="rockContext">The rock context.</param>
        /// <param name="action">The action.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="errorMessages">The error messages.</param>
        /// <returns></returns>
        public override bool Execute( RockContext rockContext, WorkflowAction action, Object entity, out List<string> errorMessages )
        {
            replyToEmail = string.Empty;
            ccEmails = new List<string>();
            bccEmails = new List<string>();
            attachments = new List<BinaryFile>();

            errorMessages = new List<string>();

            var mergeFields = GetMergeFields( action );

            string attachmentValue = GetAttributeValue( action, "Attachments" );
            Guid? attachmentGuid = attachmentValue.AsGuidOrNull();
            AttributeCache attachmentAttribute = null;
            if ( attachmentGuid.HasValue )
            {
                attachmentAttribute = AttributeCache.Read( attachmentGuid.Value, rockContext );
            }

            if ( attachmentAttribute != null )
            {
                if ( attachmentAttribute.FieldType.Class == "Rock.Field.Types.BinaryFileFieldType" )
                {
                    string attachmentAttributeValue = action.GetWorklowAttributeValue( attachmentGuid.Value );
                    if ( !string.IsNullOrWhiteSpace( attachmentAttributeValue ) )
                    {
                        var file = GetBinaryFile( rockContext, fileGuid: attachmentAttributeValue.AsGuidOrNull() );
                        if ( file != null )
                        {
                            attachments.Add( file );
                        }
                    }
                }
                else
                {
                    string attachmentAttributeValue = action.GetWorklowAttributeValue( attachmentGuid.Value );
                    if ( !string.IsNullOrWhiteSpace( attachmentAttributeValue ) )
                    {
                        var attachmentIds = attachmentAttributeValue.ResolveMergeFields( mergeFields ).SplitDelimitedValues().ToList();
                        foreach ( var id in attachmentIds )
                        {
                            var file = GetBinaryFile( rockContext, id.AsIntegerOrNull(), id.AsGuidOrNull() );
                            if ( file != null )
                            {
                                attachments.Add( file );
                            }
                        }
                    }
                }
            }
            else
            {
                var attachmentIds = attachmentValue.ResolveMergeFields( mergeFields ).SplitDelimitedValues().ToList();
                foreach ( var id in attachmentIds )
                {
                    var file = GetBinaryFile( rockContext, id.AsIntegerOrNull(), id.AsGuidOrNull() );
                    if ( file != null )
                    {
                        attachments.Add( file );
                    }
                }
            }

            string to = GetAttributeValue( action, "To" );
            string fromValue = GetAttributeValue( action, "From" );
            string subject = GetAttributeValue( action, "Subject" );
            string body = GetAttributeValue( action, "Body" );
            string replytoValue = GetAttributeValue( action, "ReplyTo" );
            string cc = GetAttributeValue( action, "CC" );
            string bcc = GetAttributeValue( action, "BCC" );

            bool createCommunicationRecord = GetAttributeValue( action, "SaveCommunicationHistory" ).AsBoolean();

            string fromEmail = string.Empty;
            string fromName = string.Empty;
            Guid? fromGuid = fromValue.AsGuidOrNull();
            if ( fromGuid.HasValue )
            {
                var attribute = AttributeCache.Read( fromGuid.Value, rockContext );
                if ( attribute != null )
                {
                    string fromAttributeValue = action.GetWorklowAttributeValue( fromGuid.Value );
                    if ( !string.IsNullOrWhiteSpace( fromAttributeValue ) )
                    {
                        if ( attribute.FieldType.Class == "Rock.Field.Types.PersonFieldType" )
                        {
                            Guid personAliasGuid = fromAttributeValue.AsGuid();
                            if ( !personAliasGuid.IsEmpty() )
                            {
                                var person = new PersonAliasService( rockContext ).Queryable()
                                    .Where( a => a.Guid.Equals( personAliasGuid ) )
                                    .Select( a => a.Person )
                                    .FirstOrDefault();
                                if ( person != null && !string.IsNullOrWhiteSpace( person.Email ) )
                                {
                                    fromEmail = person.Email;
                                    fromName = person.FullName;
                                }
                            }
                        }
                        else
                        {
                            fromEmail = fromAttributeValue;
                        }
                    }
                }
            }
            else
            {
                fromEmail = fromValue;
            }

            Guid? replytoGuid = replytoValue.AsGuidOrNull();
            if ( replytoGuid.HasValue )
            {
                var attribute = AttributeCache.Read( replytoGuid.Value, rockContext );
                if ( attribute != null )
                {
                    string replytoAttributeValue = action.GetWorklowAttributeValue( replytoGuid.Value );
                    if ( !string.IsNullOrWhiteSpace( replytoAttributeValue ) )
                    {
                        if ( attribute.FieldType.Class == "Rock.Field.Types.PersonFieldType" )
                        {
                            Guid personAliasGuid = replytoAttributeValue.AsGuid();
                            if ( !personAliasGuid.IsEmpty() )
                            {
                                var person = new PersonAliasService( rockContext ).Queryable()
                                    .Where( a => a.Guid.Equals( personAliasGuid ) )
                                    .Select( a => a.Person )
                                    .FirstOrDefault();
                                if ( person != null && !string.IsNullOrWhiteSpace( person.Email ) )
                                {
                                    replyToEmail = person.Email;
                                }
                            }
                        }
                        else
                        {
                            replyToEmail = replytoAttributeValue;
                        }
                    }
                }
            }

            Guid? ccguid = cc.AsGuidOrNull();
            if ( ccguid.HasValue )
            {
                var attribute = AttributeCache.Read( ccguid.Value, rockContext );
                if ( attribute != null )
                {
                    string ccValue = action.GetWorklowAttributeValue( ccguid.Value );
                    if ( !string.IsNullOrWhiteSpace( ccValue ) )
                    {
                        switch ( attribute.FieldType.Class )
                        {
                            case "Rock.Field.Types.TextFieldType":
                            case "Rock.Field.Types.EmailFieldType":
                                {
                                    ccEmails.Add( ccValue );
                                    break;
                                }
                            case "Rock.Field.Types.PersonFieldType":
                                {
                                    Guid personAliasGuid = ccValue.AsGuid();
                                    if ( !personAliasGuid.IsEmpty() )
                                    {
                                        var person = new PersonAliasService( rockContext ).Queryable()
                                            .Where( a => a.Guid.Equals( personAliasGuid ) )
                                            .Select( a => a.Person )
                                            .FirstOrDefault();
                                        if ( person == null )
                                        {
                                            action.AddLogEntry( "Invalid CC Recipient: Person not found", true );
                                        }
                                        else if ( string.IsNullOrWhiteSpace( person.Email ) )
                                        {
                                            action.AddLogEntry( "CC Email was not sent: CC Recipient does not have an email address", true );
                                        }
                                        else if ( !person.IsEmailActive )
                                        {
                                            action.AddLogEntry( "CC Email was not sent: CC Recipient email is not active", true );
                                        }
                                        else if ( person.EmailPreference == EmailPreference.DoNotEmail )
                                        {
                                            action.AddLogEntry( "CC Email was not sent: CC Recipient has requested 'Do Not Email'", true );
                                        }
                                        else
                                        {
                                            ccEmails.Add( person.Email );
                                        }
                                    }
                                    break;
                                }
                            case "Rock.Field.Types.GroupFieldType":
                            case "Rock.Field.Types.SecurityRoleFieldType":
                                {
                                    int? groupId = ccValue.AsIntegerOrNull();
                                    Guid? groupGuid = ccValue.AsGuidOrNull();

                                    //Get the Group Role attribute value
                                    Guid? groupRoleValueGuid = GetGroupRoleValue( action, "CCGroupRole" );

                                    IQueryable<GroupMember> qry = null;

                                    // Handle situations where the attribute value is the ID
                                    if ( groupId.HasValue )
                                    {
                                        qry = new GroupMemberService( rockContext ).GetByGroupId( groupId.Value );
                                    }

                                    // Handle situations where the attribute value stored is the Guid
                                    else if ( groupGuid.HasValue )
                                    {
                                        qry = new GroupMemberService( rockContext ).GetByGroupGuid( groupGuid.Value );
                                    }
                                    else
                                    {
                                        action.AddLogEntry( "Invalid CC Recipient: No valid group id or Guid", true );
                                    }

                                    if ( groupRoleValueGuid.HasValue )
                                    {
                                        qry = qry.Where( m => m.GroupRole != null && m.GroupRole.Guid.Equals( groupRoleValueGuid.Value ) );
                                    }

                                    if ( qry != null )
                                    {
                                        foreach ( var person in qry
                                            .Where( m => m.GroupMemberStatus == GroupMemberStatus.Active )
                                            .Select( m => m.Person ) )
                                        {
                                            if ( ( person.IsEmailActive ) &&
                                                person.EmailPreference != EmailPreference.DoNotEmail &&
                                                !string.IsNullOrWhiteSpace( person.Email ) )
                                            {
                                                ccEmails.Add( person.Email );
                                            }
                                        }
                                    }
                                    break;
                                }
                        }
                    }
                }
            }
            else if ( string.IsNullOrWhiteSpace( cc ) && cc.IsValidEmail() )
            {
                ccEmails.Add( cc );
            }

            Guid? bccguid = bcc.AsGuidOrNull();
            if ( bccguid.HasValue )
            {
                var attribute = AttributeCache.Read( bccguid.Value, rockContext );
                if ( attribute != null )
                {
                    string bccValue = action.GetWorklowAttributeValue( bccguid.Value );
                    if ( !string.IsNullOrWhiteSpace( bccValue ) )
                    {
                        switch ( attribute.FieldType.Class )
                        {
                            case "Rock.Field.Types.TextFieldType":
                            case "Rock.Field.Types.EmailFieldType":
                                {
                                    bccEmails.Add( bccValue );
                                    break;
                                }
                            case "Rock.Field.Types.PersonFieldType":
                                {
                                    Guid personAliasGuid = bccValue.AsGuid();
                                    if ( !personAliasGuid.IsEmpty() )
                                    {
                                        var person = new PersonAliasService( rockContext ).Queryable()
                                            .Where( a => a.Guid.Equals( personAliasGuid ) )
                                            .Select( a => a.Person )
                                            .FirstOrDefault();
                                        if ( person == null )
                                        {
                                            action.AddLogEntry( "Invalid BCC Recipient: Person not found", true );
                                        }
                                        else if ( string.IsNullOrWhiteSpace( person.Email ) )
                                        {
                                            action.AddLogEntry( "BCC Email was not sent: BCC Recipient does not have an email address", true );
                                        }
                                        else if ( !person.IsEmailActive )
                                        {
                                            action.AddLogEntry( "BCC Email was not sent: BCC Recipient email is not active", true );
                                        }
                                        else if ( person.EmailPreference == EmailPreference.DoNotEmail )
                                        {
                                            action.AddLogEntry( "BCC Email was not sent: BCC Recipient has requested 'Do Not Email'", true );
                                        }
                                        else
                                        {
                                            bccEmails.Add( person.Email );
                                        }
                                    }
                                    break;
                                }
                            case "Rock.Field.Types.GroupFieldType":
                            case "Rock.Field.Types.SecurityRoleFieldType":
                                {
                                    int? groupId = bccValue.AsIntegerOrNull();
                                    Guid? groupGuid = bccValue.AsGuidOrNull();

                                    //Get the Group Role attribute value
                                    Guid? groupRoleValueGuid = GetGroupRoleValue( action, "BCCGroupRole" );

                                    IQueryable<GroupMember> qry = null;

                                    // Handle situations where the attribute value is the ID
                                    if ( groupId.HasValue )
                                    {
                                        qry = new GroupMemberService( rockContext ).GetByGroupId( groupId.Value );
                                    }

                                    // Handle situations where the attribute value stored is the Guid
                                    else if ( groupGuid.HasValue )
                                    {
                                        qry = new GroupMemberService( rockContext ).GetByGroupGuid( groupGuid.Value );
                                    }
                                    else
                                    {
                                        action.AddLogEntry( "Invalid BCC Recipient: No valid group id or Guid", true );
                                    }

                                    if ( groupRoleValueGuid.HasValue )
                                    {
                                        qry = qry.Where( m => m.GroupRole != null && m.GroupRole.Guid.Equals( groupRoleValueGuid.Value ) );
                                    }

                                    if ( qry != null )
                                    {
                                        foreach ( var person in qry
                                            .Where( m => m.GroupMemberStatus == GroupMemberStatus.Active )
                                            .Select( m => m.Person ) )
                                        {
                                            if ( ( person.IsEmailActive ) &&
                                                person.EmailPreference != EmailPreference.DoNotEmail &&
                                                !string.IsNullOrWhiteSpace( person.Email ) )
                                            {
                                                bccEmails.Add( person.Email );
                                            }
                                        }
                                    }
                                    break;
                                }
                        }
                    }
                }
            }
            else if ( string.IsNullOrWhiteSpace( bcc ) && bcc.IsValidEmail() )
            {
                bccEmails.Add( bcc );
            }

            Guid? guid = to.AsGuidOrNull();
            if ( guid.HasValue )
            {
                var attribute = AttributeCache.Read( guid.Value, rockContext );
                if ( attribute != null )
                {
                    string toValue = action.GetWorklowAttributeValue( guid.Value );
                    if ( !string.IsNullOrWhiteSpace( toValue ) )
                    {
                        switch ( attribute.FieldType.Class )
                        {
                            case "Rock.Field.Types.TextFieldType":
                            case "Rock.Field.Types.EmailFieldType":
                                {
                                    Send( toValue, fromEmail, fromName, subject, body, mergeFields, rockContext, createCommunicationRecord );
                                    break;
                                }
                            case "Rock.Field.Types.PersonFieldType":
                                {
                                    Guid personAliasGuid = toValue.AsGuid();
                                    if ( !personAliasGuid.IsEmpty() )
                                    {
                                        var person = new PersonAliasService( rockContext ).Queryable()
                                            .Where( a => a.Guid.Equals( personAliasGuid ) )
                                            .Select( a => a.Person )
                                            .FirstOrDefault();
                                        if ( person == null )
                                        {
                                            action.AddLogEntry( "Invalid Recipient: Person not found", true );
                                        }
                                        else if ( string.IsNullOrWhiteSpace( person.Email ) )
                                        {
                                            action.AddLogEntry( "Email was not sent: Recipient does not have an email address", true );
                                        }
                                        else if ( !person.IsEmailActive )
                                        {
                                            action.AddLogEntry( "Email was not sent: Recipient email is not active", true );
                                        }
                                        else if ( person.EmailPreference == EmailPreference.DoNotEmail )
                                        {
                                            action.AddLogEntry( "Email was not sent: Recipient has requested 'Do Not Email'", true );
                                        }
                                        else
                                        {
                                            var personDict = new Dictionary<string, object>( mergeFields );
                                            personDict.Add( "Person", person );
                                            Send( person.Email, fromEmail, fromName, subject, body, personDict, rockContext, createCommunicationRecord );
                                        }
                                    }
                                    break;
                                }
                            case "Rock.Field.Types.GroupFieldType":
                            case "Rock.Field.Types.SecurityRoleFieldType":
                                {
                                    int? groupId = toValue.AsIntegerOrNull();
                                    Guid? groupGuid = toValue.AsGuidOrNull();

                                    //Get the Group Role attribute value
                                    Guid? groupRoleValueGuid = GetGroupRoleValue( action, "GroupRole" );

                                    IQueryable<GroupMember> qry = null;

                                    // Handle situations where the attribute value is the ID
                                    if ( groupId.HasValue )
                                    {
                                        qry = new GroupMemberService( rockContext ).GetByGroupId( groupId.Value );
                                    }

                                    // Handle situations where the attribute value stored is the Guid
                                    else if ( groupGuid.HasValue )
                                    {
                                        qry = new GroupMemberService( rockContext ).GetByGroupGuid( groupGuid.Value );
                                    }
                                    else
                                    {
                                        action.AddLogEntry( "Invalid Recipient: No valid group id or Guid", true );
                                    }

                                    if ( groupRoleValueGuid.HasValue )
                                    {
                                        qry = qry.Where( m => m.GroupRole != null && m.GroupRole.Guid.Equals( groupRoleValueGuid.Value ) );
                                    }

                                    if ( qry != null )
                                    {
                                        foreach ( var person in qry
                                            .Where( m => m.GroupMemberStatus == GroupMemberStatus.Active )
                                            .Select( m => m.Person ) )
                                        {
                                            if ( ( person.IsEmailActive ) &&
                                                person.EmailPreference != EmailPreference.DoNotEmail &&
                                                !string.IsNullOrWhiteSpace( person.Email ) )
                                            {
                                                var personDict = new Dictionary<string, object>( mergeFields );
                                                personDict.Add( "Person", person );
                                                Send( person.Email, fromEmail, fromName, subject, body, personDict, rockContext, createCommunicationRecord );
                                            }
                                        }
                                    }
                                    break;
                                }
                        }
                    }
                }
            }
            else
            {
                Send( to.ResolveMergeFields( mergeFields ), fromEmail, fromName, subject, body, mergeFields, rockContext, createCommunicationRecord );
            }

            return true;
        }

        private Guid? GetGroupRoleValue( WorkflowAction action, string key )
        {
            Guid? groupRoleGuid = null;

            string groupRole = GetAttributeValue( action, key );
            Guid? groupRoleAttributeGuid = GetAttributeValue( action, key ).AsGuidOrNull();

            if ( groupRoleAttributeGuid.HasValue )
            {
                groupRoleGuid = action.GetWorklowAttributeValue( groupRoleAttributeGuid.Value ).AsGuidOrNull();
            }

            return groupRoleGuid;
        }

        private void Send( string recipients, string fromEmail, string fromName, string subject, string body, Dictionary<string, object> mergeFields, RockContext rockContext, bool createCommunicationRecord )
        {
            var emailMessage = new RockEmailMessage();
            foreach ( string recipient in recipients.SplitDelimitedValues().ToList() )
            {
                emailMessage.AddRecipient( new RecipientData( recipient, mergeFields ) );
            }
            emailMessage.FromEmail = fromEmail;
            emailMessage.FromName = fromName;
            emailMessage.Subject = subject;
            emailMessage.Message = body;
            emailMessage.CreateCommunicationRecord = createCommunicationRecord;

            if ( !string.IsNullOrWhiteSpace( replyToEmail ) && replyToEmail.IsValidEmail() )
            {
                emailMessage.ReplyToEmail = replyToEmail;
            }

            if ( ccEmails.Count > 0 )
            {
                emailMessage.CCEmails = ccEmails;
            }

            if ( bccEmails.Count > 0 )
            {
                emailMessage.BCCEmails = bccEmails;
            }

            if ( attachments.Count > 0 )
            {
                emailMessage.Attachments = attachments;
            }

            emailMessage.Send();
        }

        private BinaryFile GetBinaryFile( RockContext rockContext, int? fileId = null, Guid? fileGuid = null )
        {
            BinaryFileService binaryFileService = new BinaryFileService( rockContext );
            if ( fileGuid != null && fileGuid != Guid.Empty )
            {
                return binaryFileService.Get( ( Guid ) fileGuid );
            }
            else if ( fileId != null )
            {
                return binaryFileService.Get( ( int ) fileId );
            }
            else
            {
                return null;
            }
        }
    }
}