using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json;

namespace com.kfs.AdobeSign.RestAPI
{
    public class Agreement
    {
        private AdobeSignClient mClient = null;
        private Agreement() { }
        public Agreement( AdobeSignClient client )
        {
            mClient = client;
        }

        public AgreementCreationResponse SendAgreement( AgreementCreationInfo agreementInfo, string adobeUserId )
        {
            string apiPath = "agreements";
            string method = "POST";

            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add( "x-api-user", string.Concat( "userid:" + adobeUserId ) );

            RawResponse rawResp = mClient.SendRequest( apiPath, method, null, headers, JsonConvert.SerializeObject( agreementInfo ) );

            if ( rawResp.StatusCode != System.Net.HttpStatusCode.Created )
            {
                ErrorResponse errResp = JsonConvert.DeserializeObject<ErrorResponse>( rawResp.jsonItem );

                throw new Exception( string.Format( "An error has occurred while sending agreement. Code {0}. Message{1}",
                    errResp.ErrorCode, errResp.ErrorMessage ) );
            }

            return JsonConvert.DeserializeObject<AgreementCreationResponse>( rawResp.jsonItem );
        }

        public string CancelAgreement( string agreementId, string comment, bool notifySigner )
        {
            string apiPath = string.Format( "agreements/{0}/status", agreementId );
            string method = "PUT";

            AgreementStatusUpdateInfo updateInfo = new AgreementStatusUpdateInfo();
            updateInfo.Value = "CANCEL";
            updateInfo.Comment = comment;
            updateInfo.NotifySigner = notifySigner;

            RawResponse rawResp = mClient.SendRequest( apiPath, method, null, null, JsonConvert.SerializeObject( updateInfo ) );

            if ( rawResp.StatusCode != System.Net.HttpStatusCode.OK )
            {
                ErrorResponse errResp = JsonConvert.DeserializeObject<ErrorResponse>( rawResp.jsonItem );
                throw new Exception( string.Format( "An error occurred while attempting to cancel agreement. Code {0}. Message {1}.",
                    errResp.ErrorCode, errResp.ErrorMessage ) );
            }

            AgreementStatusUpdateResponse updateResp = JsonConvert.DeserializeObject<AgreementStatusUpdateResponse>( rawResp.jsonItem );

            return updateResp.Result;

        }

        public AgreementInfo GetAgreementInfo( string agreementId )
        {
            string apiPath = string.Format( "agreements/{0}", agreementId );
            string method = "GET";

            RawResponse rawResp = mClient.SendRequest( apiPath, method );

            if ( rawResp.StatusCode != System.Net.HttpStatusCode.OK )
            {
                ErrorResponse errResp = JsonConvert.DeserializeObject<ErrorResponse>( rawResp.jsonItem );

                throw new Exception( string.Format( "An error occurred while retrieving Agreement Info. Code {0}. Message {1}.",
                    errResp.ErrorCode, errResp.ErrorMessage ) );
            }

            return JsonConvert.DeserializeObject<AgreementInfo>( rawResp.jsonItem );
        }

        public string GetLatestDocumentUrl( string agreementId, bool includeAuditReport )
        {
            string apiPath = string.Format( "agreements/{0}/combinedDocument/url", agreementId );
            string method = "GET";
            Dictionary<string, string> qs = new Dictionary<string, string>();

            if ( includeAuditReport )
            {
                qs.Add( "auditReport", includeAuditReport.ToString().ToLower() );
            }

            RawResponse rawResp = mClient.SendRequest( apiPath, method, qs, null, null );

            if ( rawResp.StatusCode != System.Net.HttpStatusCode.OK )
            {
                ErrorResponse errResp = JsonConvert.DeserializeObject<ErrorResponse>( rawResp.jsonItem );
                throw new Exception( string.Format( "An error has occurred while retrieving the Agreement Document URL. Code: {0}. Message: {1}",
                    errResp.ErrorCode, errResp.ErrorMessage ) );
            }

            var documentUrlObj = new { url = "" };
            var documentUrl = JsonConvert.DeserializeAnonymousType( rawResp.jsonItem, documentUrlObj );


            return documentUrl.url;

        }
    }

    #region AgreementInfo
    public class AgreementInfo
    {
        [JsonProperty( PropertyName = "agreementId", Required = Required.Always )]
        public string AgreementId { get; set; }

        [JsonProperty( PropertyName = "events", Required = Required.Always )]
        public DocumentHistoryEvent[] Events { get; set; }

        [JsonProperty( PropertyName = "locale", Required = Required.Always )]
        public string Locale { get; set; }

        [JsonProperty( PropertyName = "modifiable", Required = Required.Always )]
        public bool Modifiable { get; set; }

        [JsonProperty( PropertyName = "name", Required = Required.Always )]
        public string Name { get; set; }

        [JsonProperty( PropertyName = "nextParticipantSetInfos" )]
        public NextParticipantSetInfo[] NextParticipantSetInfos { get; set; }

        [JsonProperty( PropertyName = "participantSetInfos", Required = Required.Always )]
        public ParticipantSetInfo[] ParticipantSetInfos { get; set; }

        [JsonProperty( PropertyName = "status", Required = Required.Always )]
        [JsonConverter( typeof( Newtonsoft.Json.Converters.StringEnumConverter ) )]
        public AgreementStatus Status { get; set; }

        [JsonProperty( PropertyName = "vaultingEnabled", Required = Required.Always )]
        public bool ValutingEnabled { get; set; }

        [JsonProperty( PropertyName = "expiration", NullValueHandling = NullValueHandling.Ignore )]
        public DateTime? Expiration { get; set; }

        [JsonProperty( PropertyName = "message", NullValueHandling = NullValueHandling.Ignore )]
        public string Message { get; set; }

        [JsonProperty( PropertyName = "securityOptions", NullValueHandling = NullValueHandling.Ignore )]
        public DocSecurityOption[] SecurityOptions { get; set; }
    }

    public class DocumentHistoryEvent
    {
        [JsonProperty( PropertyName = "actingUserEmail", Required = Required.Always )]
        public string ActingUserEmail { get; set; }
        [JsonProperty( PropertyName = "actingUserIpAddress", NullValueHandling = NullValueHandling.Ignore )]
        public string ActingUserIPAddress { get; set; }
        [JsonProperty( PropertyName = "date", Required = Required.Always )]
        public DateTime HistoryDate { get; set; }

        [JsonProperty( PropertyName = "description", Required = Required.Always )]
        public string Description { get; set; }

        [JsonProperty( PropertyName = "participantEmail", Required = Required.Always )]
        public string ParticipantEmail { get; set; }

        [JsonProperty( PropertyName = "type", Required = Required.Always )]
        [JsonConverter( typeof( Newtonsoft.Json.Converters.StringEnumConverter ) )]
        public AgreementEventType AgreeementEventType { get; set; }

        [JsonProperty( PropertyName = "versionId" )]
        public string VersionId { get; set; }

        [JsonProperty( PropertyName = "comment", NullValueHandling = NullValueHandling.Ignore )]
        public string Comment { get; set; }

        [JsonProperty( PropertyName = "deviceLocation", NullValueHandling = NullValueHandling.Ignore )]
        public DeviceLocation DeviceLocation { get; set; }

        [JsonProperty( PropertyName = "synchronizationId", NullValueHandling = NullValueHandling.Ignore )]
        public string SynchronizationId { get; set; }

        [JsonProperty( PropertyName = "vaultEventId", NullValueHandling = NullValueHandling.Ignore )]
        public string VaultEventId { get; set; }

    }

    public enum AgreementEventType
    {
        AGREEMENT_MODIFIED,
        APPROVAL_REQUESTED,
        APPROVED,
        AUTO_CANCELLED_CONVERSION_PROBLEM,
        AUTO_DELEGATED,
        CREATED,
        DELEGATED,
        DIGSIGNED,
        DOCUMENTS_DELETED,
        EMAIL_BOUNCED,
        EMAIL_VIEWED,
        ESIGNED,
        EXPIRED,
        EXPIRED_AUTOMATICALLY,
        FAXED_BY_SENDER,
        FAXIN_RECEIVED,
        KBA_AUTHENTICATED,
        KBA_AUTHENTICATION_FAILED,
        OFFLINE_SYNC,
        OTHER,
        PASSWORD_AUTHENTICATION_FAILED,
        PRESIGNED,
        RECALLED,
        REJECTED,
        REPLACED_SIGNER,
        SENDER_CREATED_NEW_REVISION,
        SHARED,
        SIGNATURE_REQUESTED,
        SIGNED,
        SIGNER_SUGGESTED_CHANGES,
        UPLOADED_BY_SENDER,
        USER_ACK_AGREEMENT_MODIFIED,
        VALULTED,
        WEB_IDENTITY_AUTHENTICATED,
        WEB_IDENTITY_SPECIFIED,
        WIDGET_DISABLED,
        WIDGET_ENABLED

    }

    public class DeviceLocation
    {
        [JsonProperty( PropertyName = "latitude" )]
        public decimal Latitude { get; set; }
        [JsonProperty( PropertyName = "longitude" )]
        public decimal Longitude { get; set; }
    }

    public class NextParticipantSetInfo
    {
        [JsonProperty( PropertyName = "NextParticipantSetMemberInfos" )]
        public NextParticipantInfo[] NextParticipantSetMemberInfos { get; set; }
    }

    public class NextParticipantInfo
    {
        [JsonProperty( PropertyName = "email" )]
        public string Email { get; set; }

        [JsonProperty( PropertyName = "waitingSince" )]
        public DateTime WaitingSince { get; set; }

        [JsonProperty( PropertyName = "name", NullValueHandling = NullValueHandling.Ignore )]
        public string Name { get; set; }
    }

    public class ParticipantSetInfo
    {
        [JsonProperty( PropertyName = "participantSetId", NullValueHandling = NullValueHandling.Ignore )]
        public string ParticipantSetId { get; set; }

        [JsonProperty( PropertyName = "participantSetMemberInfos" )]
        public ParticipantInfo[] ParticipantSetMemberInfos { get; set; }


        [JsonProperty( PropertyName = "roles", NullValueHandling = NullValueHandling.Ignore, ItemConverterType = typeof( Newtonsoft.Json.Converters.StringEnumConverter ) )]
        public ParticipantRole[] Roles { get; set; }

        [JsonProperty( PropertyName = "status" )]
        [JsonConverter( typeof( Newtonsoft.Json.Converters.StringEnumConverter ) )]
        public UserAgreementStatus Status { get; set; }

        [JsonProperty( PropertyName = "privateMessage", NullValueHandling = NullValueHandling.Ignore )]
        public string PrivateMessasge { get; set; }

        [JsonProperty( PropertyName = "securityOptions", NullValueHandling = NullValueHandling.Ignore )]
        [JsonConverter( typeof( Newtonsoft.Json.Converters.StringEnumConverter ) )]
        public ParticipantSecurityOption[] SecurityOptions { get; set; }

        [JsonProperty( PropertyName = "signingOrder", NullValueHandling = NullValueHandling.Ignore )]
        public int? SigningOrder { get; set; }
    }

    public class ParticipantInfo
    {
        [JsonProperty( PropertyName = "email" )]
        public string Email { get; set; }
        [JsonProperty( PropertyName = "participantId" )]
        public string ParticipantId { get; set; }
        [JsonProperty( PropertyName = "alternateParticipants", NullValueHandling = NullValueHandling.Ignore )]
        public ParticipantSetInfo[] AlternateParticipants { get; set; }
        [JsonProperty( PropertyName = "company", NullValueHandling = NullValueHandling.Ignore )]
        public string Company { get; set; }
        [JsonProperty( PropertyName = "name", NullValueHandling = NullValueHandling.Ignore )]
        public string Name { get; set; }

        [JsonConverter( typeof( Newtonsoft.Json.Converters.StringEnumConverter ) )]
        [JsonProperty( PropertyName = "securityOptions", NullValueHandling = NullValueHandling.Ignore )]
        public ParticipantSecurityOption[] SecurityOptions { get; set; }

        [JsonProperty( PropertyName = "title", NullValueHandling = NullValueHandling.Ignore )]
        public string Title { get; set; }

    }

    public enum ParticipantSecurityOption
    {
        KBA,
        OTHER,
        PASSWORD,
        PHONE,
        WEB_IDENTITY
    }

    public enum ParticipantRole
    {
        APPROVER,
        CC,
        DELEGATE,
        DELEGATE_TO_APPROVER,
        DELEGATE_TO_SIGNER,
        OTHER,
        SENDER,
        SHARE,
        SIGNER
    }

    public enum UserAgreementStatus
    {
        APPROVED,
        ARCHIVED,
        EXPIRED,
        FORM,
        HIDDEN,
        IN_REVIEW,
        NOT_YET_VISIBLE,
        OTHER,
        OUT_FOR_APPROVAL,
        OUT_FOR_SIGNATURE,
        PARTIAL,
        RECALLED,
        SIGNED,
        UNKNOWN,
        WAITING_FOR_AUTHORING,
        WAITING_FOR_FAXIN,
        WAITING_FOR_MY_APPROVAL,
        WAITING_FOR_MY_DELEGATION,
        WAITING_FOR_MY_REVIEW,
        WAITING_FOR_MY_SIGNATURE,
        WIDGET
    }

    public enum AgreementStatus
    {
        ABORTED,
        APPROVED,
        ARCHIVED,
        AUTHORING,
        DOCUMENT_LIBRARY,
        EXPIRED,
        OTHER,
        OUT_FOR_APPROVAL,
        OUT_FOR_SIGNATURE,
        PREFILL,
        SIGNED,
        WAITING_FOR_FAXIN,
        WAITING_FOR_PAYMENT,
        WAITING_FOR_REVIEW,
        WAITING_FOR_VERIFICATION,
        WIDGET,
        WIDGET_WAITING_FOR_VERIFICATION
    }

    public enum DocSecurityOption
    {
        OPEN_PROTECTED,
        OTHER
    }

    #endregion



    public class AgreementStatusUpdateInfo
    {
        [JsonProperty( PropertyName = "value" )]
        public string Value { get; set; }
        [JsonProperty( PropertyName = "comment", NullValueHandling = NullValueHandling.Ignore )]
        public string Comment { get; set; }
        [JsonProperty( PropertyName = "notifySigner", NullValueHandling = NullValueHandling.Ignore )]
        public bool? NotifySigner { get; set; }
    }
    public class AgreementStatusUpdateResponse
    {
        [JsonProperty( PropertyName = "result" )]
        public string Result { get; set; }
    }

    #region Agreement Creation Request
    public class AgreementCreationInfo
    {
        [JsonProperty( PropertyName = "documentCreationInfo", Required = Required.Always )]
        public DocumentCreationInfo DocumentCreationInfo { get; set; }

        [JsonProperty( PropertyName = "options", NullValueHandling = NullValueHandling.Ignore )]
        public InteractiveOptions Options { get; set; }

    }

    public class DocumentCreationInfo
    {

        [JsonProperty( PropertyName = "fileInfos", NullValueHandling = NullValueHandling.Ignore )]
        public List<FileInfo> FileInfos { get; set; }

        [JsonProperty( PropertyName = "name", Required = Required.Always )]
        public string Name { get; set; }

        [JsonProperty( PropertyName = "recipientSetInfos", Required = Required.Always )]
        public List<RecipientSetInfo> RecipientSetInfos { get; set; }

        [JsonProperty( PropertyName = "signatureType", Required = Required.Always )]
        [JsonConverter( typeof( Newtonsoft.Json.Converters.StringEnumConverter ) )]
        public SignatureType SignatureType
        {
            get;
            set;
        }

        [JsonProperty( PropertyName = "callbackInfo", NullValueHandling = NullValueHandling.Ignore )]
        public string callbackInfo { get; set; }

        [JsonProperty( PropertyName = "ccs", NullValueHandling = NullValueHandling.Ignore )]
        public string[] CCs { get; set; }

        [JsonProperty( PropertyName = "daysUntilSigningDeadline", NullValueHandling = NullValueHandling.Ignore )]
        public int? DaysUntilSigningDeadline { get; set; }

        [JsonProperty( PropertyName = "externalId", NullValueHandling = NullValueHandling.Ignore )]
        public ExternalId ExternalId { get; set; }

        [JsonProperty( PropertyName = "formFieldLayerTemplates", NullValueHandling = NullValueHandling.Ignore )]
        public FileInfo[] FormFieldLayerTemplates { get; set; }

        [JsonProperty( PropertyName = "formFields", NullValueHandling = NullValueHandling.Ignore )]
        public RequestFormField[] FormFields { get; set; }

        [JsonProperty( PropertyName = "locale", NullValueHandling = NullValueHandling.Ignore )]
        public string Locale { get; set; }

        [JsonProperty( PropertyName = "mergeFieldInfo", NullValueHandling = NullValueHandling.Ignore )]
        public List<MergeFieldInfo> MergeFieldInfo { get; set; }

        [JsonProperty( PropertyName = "message", NullValueHandling = NullValueHandling.Ignore )]
        public string Message { get; set; }

        [JsonProperty( PropertyName = "postSignOptions", NullValueHandling = NullValueHandling.Ignore )]
        public PostSignOptions PostSignOptions { get; set; }

        [JsonProperty( PropertyName = "reminderFrequency", NullValueHandling = NullValueHandling.Ignore )]
        public string ReminderFrequency { get; set; }

        [JsonProperty( PropertyName = "securityOption", NullValueHandling = NullValueHandling.Ignore )]
        public SecurityOption SecurityOptions { get; set; }

        [JsonProperty( PropertyName = "signatureFlow", NullValueHandling = NullValueHandling.Ignore )]
        [JsonConverter( typeof( Newtonsoft.Json.Converters.StringEnumConverter ) )]
        public SignatureFlow SignatureFlow { get; set; }

        [JsonProperty( PropertyName = "valutingInfo", NullValueHandling = NullValueHandling.Ignore )]
        public VaultingInfo VaultingInfo { get; set; }


        public void AddEmailRecipient( string emailAddress, RecipientRole role )
        {
            if ( RecipientSetInfos == null )
            {
                RecipientSetInfos = new List<RecipientSetInfo>();
            }

            RecipientSetInfos.Add( new RecipientSetInfo()
            {
                RecipientSetRole = role,
                RecipientSetMemberInfos = new List<RecipientInfo>() { new RecipientInfo { Email = emailAddress } }
            } );
        }

        public void AddLibraryDocument( string libraryDocumentId )
        {
            if ( FileInfos == null )
            {
                FileInfos = new List<FileInfo>();
            }

            FileInfos.Add( new FileInfo() { LibraryDocumentId = libraryDocumentId } );


        }

        public void AddMergeField( string fieldName, string fieldValue )
        {
            if ( MergeFieldInfo == null )
            {
                MergeFieldInfo = new List<RestAPI.MergeFieldInfo>();
            }

            MergeFieldInfo.Add( new MergeFieldInfo() { FieldName = fieldName, DefaultValue = fieldValue } );
        }
    }

    public class FileInfo
    {
        [JsonProperty( PropertyName = "documentURL", NullValueHandling = NullValueHandling.Ignore )]
        public URLFileInfo DocumentUrl { get; set; }

        [JsonProperty( PropertyName = "libraryDocumentId", NullValueHandling = NullValueHandling.Ignore )]
        public string LibraryDocumentId { get; set; }

        [JsonProperty( PropertyName = "libraryDocumentName", NullValueHandling = NullValueHandling.Ignore )]
        public string LibraryDocumentName { get; set; }

        [JsonProperty( PropertyName = "transientDocumentId", NullValueHandling = NullValueHandling.Ignore )]
        public string TransientDocumentId { get; set; }
    }

    public class URLFileInfo
    {
        [JsonProperty( PropertyName = "mimeType", NullValueHandling = NullValueHandling.Ignore )]
        public string MimeType { get; set; }

        [JsonProperty( PropertyName = "name", NullValueHandling = NullValueHandling.Ignore )]
        public string Name { get; set; }

        [JsonProperty( PropertyName = "url", NullValueHandling = NullValueHandling.Ignore )]
        public string URL { get; set; }

    }

    public class RecipientSetInfo
    {
        [JsonProperty( PropertyName = "recipientSetMemberInfos", Required = Required.Always )]
        public List<RecipientInfo> RecipientSetMemberInfos { get; set; }

        [JsonConverter( typeof( Newtonsoft.Json.Converters.StringEnumConverter ) )]
        [JsonProperty( PropertyName = "recipientSetRole", Required = Required.Always )]
        public RecipientRole RecipientSetRole { get; set; }

        [JsonProperty( PropertyName = "privateMessage", NullValueHandling = NullValueHandling.Ignore )]
        public string PrivateMessage { get; set; }

        [JsonProperty( PropertyName = "securityOptions", NullValueHandling = NullValueHandling.Ignore )]
        public RecipientSecurityOption[] SecurityOptions { get; set; }

        [JsonProperty( PropertyName = "signingOrder", NullValueHandling = NullValueHandling.Ignore )]
        public int? SigningOrder { get; set; }

    }

    public class RecipientInfo
    {
        //TODO: ADD VALIDATION!
        [JsonProperty( PropertyName = "email", NullValueHandling = NullValueHandling.Ignore )]
        public string Email { get; set; }

        [JsonProperty( PropertyName = "fax", NullValueHandling = NullValueHandling.Ignore )]
        public string Fax { get; set; }

        [JsonProperty( PropertyName = "securityOptions", NullValueHandling = NullValueHandling.Ignore )]
        public RecipientSecurityOption[] SecurityOptions { get; set; }
    }

    public class RecipientSecurityOption
    {
        [JsonProperty( PropertyName = "authenticationMethod", Required = Required.Always )]
        public AuthenticationMethod AuthenticationMethod { get; set; }

        [JsonProperty( PropertyName = "phoneInfos", NullValueHandling = NullValueHandling.Ignore )]
        public PhoneInfo[] PhoneInfos { get; set; }

        [JsonProperty( PropertyName = "password", NullValueHandling = NullValueHandling.Ignore )]
        public string Password { get; set; }

    }

    public enum AuthenticationMethod
    {
        INHERITED_FROM_DOCUMENT,
        KBA,
        NONE,
        PASSWORD,
        PHONE,
        WEB_IDENTITY

    }

    public class PhoneInfo
    {
        [JsonProperty( PropertyName = "phone", Required = Required.Always )]
        public string Phone { get; set; }

        [JsonProperty( PropertyName = "contryCode", NullValueHandling = NullValueHandling.Ignore )]
        public string ContryCode { get; set; }
    }

    public enum RecipientRole
    {
        APPROVER,
        DELEGATE_TO_APPROVER,
        DELEGATE_TO_SIGNER,
        SIGNER
    }

    public class ExternalId
    {
        [JsonProperty( PropertyName = "group", NullValueHandling = NullValueHandling.Ignore )]
        public string Group { get; set; }

        [JsonProperty( PropertyName = "id", NullValueHandling = NullValueHandling.Ignore )]
        public string Id { get; set; }

        [JsonProperty( PropertyName = "namespace", NullValueHandling = NullValueHandling.Ignore )]
        public string Namespace { get; set; }
    }

    public class RequestFormField
    {
        [JsonProperty( PropertyName = "locations", Required = Required.Always )]
        public FormFieldLocation[] Locations { get; set; }

        [JsonProperty( PropertyName = "name", Required = Required.Always )]
        public string Name { get; set; }

        [JsonProperty( PropertyName = "alignment", NullValueHandling = NullValueHandling.Ignore )]
        public string Alignment { get; set; }

        [JsonProperty( PropertyName = "anyOrAll", NullValueHandling = NullValueHandling.Ignore )]
        public string AnyOrAll { get; set; }

        [JsonProperty( PropertyName = "backgroundColor", NullValueHandling = NullValueHandling.Ignore )]
        public string BackgroundColor { get; set; }

        [JsonProperty( PropertyName = "borderColor", NullValueHandling = NullValueHandling.Ignore )]
        public string BorderColor { get; set; }

        [JsonProperty( PropertyName = "borderStyle", NullValueHandling = NullValueHandling.Ignore )]
        public string BorderStyle { get; set; }

        [JsonProperty( PropertyName = "borderWidth", NullValueHandling = NullValueHandling.Ignore )]
        public float BorderWidth { get; set; }

        [JsonProperty( PropertyName = "calculatedExpression", NullValueHandling = NullValueHandling.Ignore )]
        public string CalculatedExpression { get; set; }

        [JsonProperty( PropertyName = "conditions", NullValueHandling = NullValueHandling.Ignore )]
        public FormFieldCondition[] Conditions { get; set; }

        [JsonProperty( PropertyName = "contentType", NullValueHandling = NullValueHandling.Ignore )]
        public string ContentType { get; set; }

        [JsonProperty( PropertyName = "defaultValue", NullValueHandling = NullValueHandling.Ignore )]
        public string DefaultValue { get; set; }

        [JsonProperty( PropertyName = "displayFormat", NullValueHandling = NullValueHandling.Ignore )]
        public string DisplayFormat { get; set; }

        [JsonProperty( PropertyName = "displayFormatType", NullValueHandling = NullValueHandling.Ignore )]
        public string DisplayFormatType { get; set; }

        [JsonProperty( PropertyName = "displayLabel", NullValueHandling = NullValueHandling.Ignore )]
        public string DisplayLabel { get; set; }

        [JsonProperty( PropertyName = "fontColor", NullValueHandling = NullValueHandling.Ignore )]
        public string FontColor { get; set; }

        [JsonProperty( PropertyName = "fontName", NullValueHandling = NullValueHandling.Ignore )]
        public string FontName { get; set; }

        [JsonProperty( PropertyName = "fontSize", NullValueHandling = NullValueHandling.Ignore )]
        public string FontSize { get; set; }

        [JsonProperty( PropertyName = "format", NullValueHandling = NullValueHandling.Ignore )]
        public string Format { get; set; }

        [JsonProperty( PropertyName = "formatData", NullValueHandling = NullValueHandling.Ignore )]
        public string FormatData { get; set; }

        [JsonProperty( PropertyName = "hidden", NullValueHandling = NullValueHandling.Ignore )]
        public bool? Hidden { get; set; }

        [JsonProperty( PropertyName = "hiddenOptions", NullValueHandling = NullValueHandling.Ignore )]
        public string[] HiddenOptions { get; set; }

        [JsonProperty( PropertyName = "inputType", NullValueHandling = NullValueHandling.Ignore )]
        public string InputType { get; set; }

        [JsonProperty( PropertyName = "masked", NullValueHandling = NullValueHandling.Ignore )]
        public bool? Masked { get; set; }

        [JsonProperty( PropertyName = "maskingText", NullValueHandling = NullValueHandling.Ignore )]
        public string MaskingText { get; set; }

        [JsonProperty( PropertyName = "maxLength", NullValueHandling = NullValueHandling.Ignore )]
        public int? MaxLength { get; set; }

        [JsonProperty( PropertyName = "maxNumberValue", NullValueHandling = NullValueHandling.Ignore )]
        public double? MaxNumberValue { get; set; }

        [JsonProperty( PropertyName = "minLength", NullValueHandling = NullValueHandling.Ignore )]
        public int? MinLength { get; set; }

        [JsonProperty( PropertyName = "minNumberValue", NullValueHandling = NullValueHandling.Ignore )]
        public double? MinNumberValue { get; set; }

        [JsonProperty( PropertyName = "radioCheckType", NullValueHandling = NullValueHandling.Ignore )]
        public string RadioCheckType { get; set; }

        [JsonProperty( PropertyName = "readOnly", NullValueHandling = NullValueHandling.Ignore )]
        public bool? ReadOnly { get; set; }

        [JsonProperty( PropertyName = "recipientIndex", NullValueHandling = NullValueHandling.Ignore )]
        public int? RecipientIndex { get; set; }

        [JsonProperty( PropertyName = "requiredExpression", NullValueHandling = NullValueHandling.Ignore )]
        public string RegularExpression { get; set; }

        [JsonProperty( PropertyName = "required", NullValueHandling = NullValueHandling.Ignore )]
        public bool RequiredField { get; set; }

        [JsonProperty( PropertyName = "showOrHide", NullValueHandling = NullValueHandling.Ignore )]
        public string ShowOrHide { get; set; }

        [JsonProperty( PropertyName = "specialErrMsg", NullValueHandling = NullValueHandling.Ignore )]
        public string SpecialErrorMessage { get; set; }

        [JsonProperty( PropertyName = "speciaFormula", NullValueHandling = NullValueHandling.Ignore )]
        public string SpecialFormula { get; set; }

        [JsonProperty( PropertyName = "toolTip", NullValueHandling = NullValueHandling.Ignore )]
        public string ToolTip { get; set; }

        [JsonProperty( PropertyName = "visibleOptions", NullValueHandling = NullValueHandling.Ignore )]
        public string[] VisibleOptions { get; set; }

    }

    public class FormFieldLocation
    {
        [JsonProperty( PropertyName = "height", Required = Required.Always )]
        public double Height { get; set; }

        [JsonProperty( PropertyName = "left", Required = Required.Always )]
        public double Left { get; set; }

        [JsonProperty( PropertyName = "pageNumber", Required = Required.Always )]
        public int PageNumber { get; set; }

        [JsonProperty( PropertyName = "top", Required = Required.Always )]
        public double Top { get; set; }

        [JsonProperty( PropertyName = "width", Required = Required.Always )]
        public double Width { get; set; }

    }

    public class FormFieldCondition
    {
        [JsonProperty( PropertyName = "value", NullValueHandling = NullValueHandling.Ignore )]
        public string Value { get; set; }

        [JsonProperty( PropertyName = "whenfieldLocationIndex", NullValueHandling = NullValueHandling.Ignore )]
        public int WhenFieldLocationIndex { get; set; }

        [JsonProperty( PropertyName = "whenFieldName", NullValueHandling = NullValueHandling.Ignore )]
        public string WhenFieldName { get; set; }
    }

    public class MergeFieldInfo
    {
        [JsonProperty( PropertyName = "defaultValue", NullValueHandling = NullValueHandling.Ignore )]
        public string DefaultValue { get; set; }

        [JsonProperty( PropertyName = "fieldName", NullValueHandling = NullValueHandling.Ignore )]
        public string FieldName { get; set; }
    }

    public class PostSignOptions
    {
        [JsonProperty( PropertyName = "redirectUrl", Required = Required.Always, NullValueHandling = NullValueHandling.Ignore )]
        public string RedirectUrl { get; set; }

        [JsonProperty( PropertyName = "redirectDelay", NullValueHandling = NullValueHandling.Ignore )]
        public int RedirectDelay { get; set; }
    }


    public class SecurityOption
    {
        [JsonProperty( PropertyName = "externalPassword", NullValueHandling = NullValueHandling.Ignore )]
        public string ExternalPassword { get; set; }

        [JsonProperty( PropertyName = "internalPassword", NullValueHandling = NullValueHandling.Ignore )]
        public string InternalPassword { get; set; }

        [JsonProperty( PropertyName = "kbaProtection", NullValueHandling = NullValueHandling.Ignore )]
        public string KBAProtection { get; set; }

        [JsonProperty( PropertyName = "openPassword", NullValueHandling = NullValueHandling.Ignore )]
        public string OpenPassword { get; set; }

        [JsonProperty( PropertyName = "passwordProtection", NullValueHandling = NullValueHandling.Ignore )]
        public string PasswordProtection { get; set; }

        [JsonProperty( PropertyName = "protectOpen", NullValueHandling = NullValueHandling.Ignore )]
        public bool? ProtectOpen { get; set; }

        [JsonProperty( PropertyName = "webIdentityProtection", NullValueHandling = NullValueHandling.Ignore )]
        public string WebIdentityProtection { get; set; }
    }

    public class VaultingInfo
    {
        [JsonProperty( PropertyName = "enabled", NullValueHandling = NullValueHandling.Ignore )]
        public bool? Enabled { get; set; }
    }

    public class InteractiveOptions
    {
        [JsonProperty( PropertyName = "authoringRequested", NullValueHandling = NullValueHandling.Ignore )]
        public bool? AuthoringRequested { get; set; }

        [JsonProperty( PropertyName = "autoLoginUser", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Ignore )]
        public bool? AutoLoginUser { get; set; }

        [JsonProperty( PropertyName = "locale", NullValueHandling = NullValueHandling.Ignore )]
        public string Locale { get; set; }

        [JsonProperty( PropertyName = "noChrome", NullValueHandling = NullValueHandling.Ignore )]
        public bool? NoChrome { get; set; }

        [JsonProperty( PropertyName = "sendThroughWeb", NullValueHandling = NullValueHandling.Ignore )]
        public string SendThroughWeb { get; set; }

        [JsonProperty( PropertyName = "sendThroughWebOptions", NullValueHandling = NullValueHandling.Ignore )]
        public SendThroughWebOptions SendThroughWebOptions { get; set; }

    }

    public class SendThroughWebOptions
    {
        [JsonProperty( PropertyName = "fileUploadOptions", NullValueHandling = NullValueHandling.Ignore )]
        public FileUploadOptions FileUploadOptions { get; set; }
    }

    public class FileUploadOptions
    {
        [JsonProperty( PropertyName = "libraryDocument", NullValueHandling = NullValueHandling.Ignore )]
        public bool? LibraryDocument { get; set; }

        [JsonProperty( PropertyName = "localFile", NullValueHandling = NullValueHandling.Ignore )]
        public bool? LocalFile { get; set; }

        [JsonProperty( PropertyName = "webConnectors", NullValueHandling = NullValueHandling.Ignore )]
        public bool? WebConnectors { get; set; }
    }

    public enum SignatureType
    {
        ESIGN,
        WRITTEN
    }

    public enum SignatureFlow
    {
        SENDER_SIGNATURE_NOT_REQUIRED,
        SENDER_SIGNS_LAST,
        SENDER_SIGNS_FIRST,
        SEQUENTIAL,
        PARALLEL,
        SENDER_SIGNS_ONLY
    }

    #endregion


    #region Agreement Creation Response
    public class AgreementCreationResponse
    {
        [JsonProperty( PropertyName = "agreementId", Required = Required.Always )]
        public string AgreementId { get; set; }

        [JsonProperty( PropertyName = "embeddedCode", NullValueHandling = NullValueHandling.Ignore )]
        public string EmbeddedCode { get; set; }

        [JsonProperty( PropertyName = "expired", NullValueHandling = NullValueHandling.Ignore )]
        public DateTime Expiration { get; set; }

        [JsonProperty( PropertyName = "url", NullValueHandling = NullValueHandling.Ignore )]
        public string Url { get; set; }
    }

    #endregion


}
