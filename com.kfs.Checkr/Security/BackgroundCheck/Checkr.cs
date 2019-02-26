using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using Rock;
using Rock.Attribute;
using Rock.Data;
using Rock.Model;
using Rock.Security;
using Rock.Web.Cache;
using Model = Rock.Model;

namespace com.kfs.Checkr.Security.BackgroundCheck
{
    /// <summary>
    /// Checkr Background Check 
    /// Note: This component requires 
    /// </summary>
    [Description( "Checkr Background Check" )]
    [Export( typeof( BackgroundCheckComponent ) )]
    [ExportMetadata( "ComponentName", "Checkr" )]

    [EncryptedTextField( "API Key", "Your Checkr API Key", true, "", "", 0, null, true )]
    
    public class Checkr : BackgroundCheckComponent
    {
        /// <summary>
        /// Sends a background request to Checkr
        /// </summary>
        /// <param name="rockContext">The rock context.</param>
        /// <param name="workflow">The Workflow initiating the request.</param>
        /// <param name="personAttribute">The person attribute.</param>
        /// <param name="blankAttributeCache">A blank attribute cache. This is required by the model and would normally be the SSN.</param>
        /// <param name="requestTypeAttribute">The request type attribute.</param>
        /// <param name="billingCodeAttribute">The billing code attribute.</param>
        /// <param name="errorMessages">The error messages.</param>
        /// <returns>
        /// True/False value of whether the request was successfully sent or not
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        /// <remarks>
        /// Note: If the associated workflow type does not have attributes with the following keys, they
        /// will automatically be added to the workflow type configuration in order to store the results
        /// of the Checkr background check request
        ///     CandidateId:            The Candidate Id returned from Checkr that represents the Person
        ///     RequestStatus:          The request status returned by Checkr request (pending, complete, expired)
        ///     ReportStatus:           The report status returned by Checkr (clear, consider)
        /// </remarks>
        public override bool SendRequest( RockContext rockContext, Model.Workflow workflow,
            AttributeCache personAttribute, AttributeCache blankAttribute, AttributeCache requestTypeAttribute,
            AttributeCache billingCodeAttribute, out List<string> errorMessages )
        {
            errorMessages = new List<string>();

            try
            {
                // Check to make sure workflow is not null
                if ( workflow == null )
                {
                    errorMessages.Add( "The 'Checkr' background check provider requires a valid workflow." );
                    return false;
                }

                // Get the person that the request is for
                Person person = null;
                if ( personAttribute != null )
                {
                    Guid? personAliasGuid = workflow.GetAttributeValue( personAttribute.Key ).AsGuidOrNull();
                    if ( personAliasGuid.HasValue )
                    {
                        person = new PersonAliasService( rockContext ).Queryable()
                            .Where( p => p.Guid.Equals( personAliasGuid.Value ) )
                            .Select( p => p.Person )
                            .FirstOrDefault();
                        person.LoadAttributes( rockContext );
                    }
                }

                if ( person == null )
                {
                    errorMessages.Add( "The 'Checkr' background check provider requires the workflow to have a 'Person' attribute that contains the person who the background check is for." );
                    return false;
                }

                var apiKey = Encryption.DecryptString( GetAttributeValue( "APIKey" ) );

                JObject candidate = Candidate.CreateCandidate( apiKey, person );
                var candidateId = candidate.Value<string>( "id" );

                if ( !string.IsNullOrWhiteSpace( candidateId ) )
                {
                    var packageSlug = string.Empty;

                    DefinedValueCache pkgTypeDefinedValue = null;

                    if ( requestTypeAttribute != null )
                    {
                        pkgTypeDefinedValue = DefinedValueCache.Read( workflow.GetAttributeValue( requestTypeAttribute.Key ).AsGuid() );
                        if ( pkgTypeDefinedValue != null )
                        {
                            packageSlug = pkgTypeDefinedValue.Value;
                        }
                    }

                    if ( !string.IsNullOrWhiteSpace( packageSlug ) )
                    {
                        JObject invitation = Invitation.CreateInvitation( apiKey, packageSlug, candidateId );
                        var invitationId = invitation.Value<string>( "id" );
                        if ( !string.IsNullOrWhiteSpace( invitationId ) )
                        {
                            var requestDateTime = RockDateTime.Now;

                            int? personAliasId = person.PrimaryAliasId;
                            if ( personAliasId.HasValue )
                            {
                                // Create a background check file
                                using ( var newRockContext = new RockContext() )
                                {
                                    var backgroundCheckService = new BackgroundCheckService( newRockContext );
                                    var backgroundCheck = backgroundCheckService.Queryable()
                                        .Where( c =>
                                            c.WorkflowId.HasValue &&
                                            c.WorkflowId.Value == workflow.Id )
                                        .FirstOrDefault();

                                    if ( backgroundCheck == null )
                                    {
                                        backgroundCheck = new Rock.Model.BackgroundCheck();
                                        backgroundCheck.PersonAliasId = personAliasId.Value;
                                        backgroundCheck.WorkflowId = workflow.Id;
                                        backgroundCheckService.Add( backgroundCheck );
                                    }

                                    backgroundCheck.RequestDate = RockDateTime.Now;

                                    backgroundCheck.ResponseXml = string.Format( @"
Request Date Time: {0}

Candidate JSON: 
------------------------ 
{1}

Invitation JSON: 
------------------------ 
{2}

", requestDateTime, candidate.ToString(), invitation.ToString() );
                                    newRockContext.SaveChanges();
                                }
                            }

                            using ( var newRockContext = new RockContext() )
                            {
                                var handledErrorMessages = new List<string>();

                                bool createdNewAttribute = false;

                                if ( SaveAttributeValue( workflow, "RequestStatus", "SUCCESS",
                                    FieldTypeCache.Read( Rock.SystemGuid.FieldType.TEXT.AsGuid() ), newRockContext, null ) )
                                {
                                    createdNewAttribute = true;
                                }

                                if ( SaveAttributeValue( workflow, "CandidateId", candidateId,
                                        FieldTypeCache.Read( Rock.SystemGuid.FieldType.TEXT.AsGuid() ), newRockContext, null ) )
                                {
                                    createdNewAttribute = true;
                                }

                                newRockContext.SaveChanges();

                                if ( createdNewAttribute )
                                {
                                    AttributeCache.FlushEntityAttributes();
                                }

                                return true;
                            }
                        }
                        else
                        {
                            var ex = new Exception( string.Format( "There was a problem creating the Invitation. {0}", invitation ) );
                            ExceptionLogService.LogException( ex, null );
                            return false;
                        }
                    }
                    else
                    {
                        var ex = new Exception( "The Package Id cannot be blank. Check the Defined Value's Value to ensure it is a valid Package Slug name and not blank. i.e. tasker_basic" );
                        ExceptionLogService.LogException( ex, null );
                        return false;
                    }
                }
                else
                {
                    var ex = new Exception( string.Format( "There was a problem creating the Candidate. {0}", candidate ) );
                    ExceptionLogService.LogException( ex, null );
                    return false;
                }
                
            }

            catch ( Exception ex )
            {
                ExceptionLogService.LogException( ex, null );
                errorMessages.Add( ex.Message );
                return false;
            }
        }
        
        /// <summary>
        /// Saves the results.
        /// </summary>
        /// <param name="xResult">The x result.</param>
        /// <param name="workflow">The workflow.</param>
        /// <param name="rockContext">The rock context.</param>
        /// <param name="saveResponse">if set to <c>true</c> [save response].</param>
        public static void SaveResults( RootObject rootObject, Rock.Model.Workflow workflow, RockContext rockContext, bool saveResponse = true )
        {
            bool createdNewAttribute = false;

            var newRockContext = new RockContext();
            var service = new BackgroundCheckService( newRockContext );
            var backgroundCheck = service.Queryable()
                .Where( c =>
                    c.WorkflowId.HasValue &&
                    c.WorkflowId.Value == workflow.Id )
                .FirstOrDefault();

            if ( backgroundCheck != null && saveResponse )
            {

                backgroundCheck.ResponseXml = backgroundCheck.ResponseXml + string.Format( @"
Response XML ({0}): 
------------------------ 
{1}

", RockDateTime.Now.ToString(), rootObject.ToString() );
            }

            var reportStatus = string.Empty;
            if ( rootObject.data.@object.status.Equals( "clear", StringComparison.CurrentCultureIgnoreCase ) )
            {
                reportStatus = "Pass";
            }
            else if ( rootObject.data.@object.status.Equals( "consider", StringComparison.CurrentCultureIgnoreCase ) )
            {
                reportStatus = "Review";
            }

            // Save the report link 
            var checkr = BackgroundCheckContainer.GetComponent( "com.kfs.Checkr.Security.BackgroundCheck.Checkr" );
            var apiKey = string.Empty;
            if ( checkr != null )
            {
                apiKey = Encryption.DecryptString( checkr.GetAttributeValue( "APIKey" ) );
            }

            Guid? binaryFileGuid = null;
            
            JObject document = Document.RetrieveDocument( apiKey, rootObject.data.@object.document_ids[0] );
            var reportLink = document.Value<string>( "download_uri" );

            if ( !string.IsNullOrWhiteSpace( reportLink ) )
            {
                if ( SaveAttributeValue( workflow, "ReportLink", reportLink,
                    FieldTypeCache.Read( Rock.SystemGuid.FieldType.URL_LINK.AsGuid() ), rockContext ) )
                {
                    createdNewAttribute = true;
                }

                // Save the report
                binaryFileGuid = SaveFile( workflow.Attributes["Report"], reportLink, workflow.Id.ToString() + ".pdf" );
                if ( binaryFileGuid.HasValue )
                {
                    if ( SaveAttributeValue( workflow, "Report", binaryFileGuid.Value.ToString(),
                        FieldTypeCache.Read( Rock.SystemGuid.FieldType.BINARY_FILE.AsGuid() ), rockContext,
                        new Dictionary<string, string> { { "binaryFileType", "" } } ) )
                    {
                        createdNewAttribute = true;
                    }
                }
            }

            // Save the status
            if ( SaveAttributeValue( workflow, "ReportStatus", reportStatus,
                FieldTypeCache.Read( Rock.SystemGuid.FieldType.SINGLE_SELECT.AsGuid() ), rockContext,
                new Dictionary<string, string> { { "fieldtype", "ddl" }, { "values", "Pass,Fail,Review" } } ) )
            {
                createdNewAttribute = true;
            }

            // Update the background check file
            if ( backgroundCheck != null )
            {
                backgroundCheck.ResponseDate = RockDateTime.Now;
                backgroundCheck.RecordFound = reportStatus == "Review";

                if ( binaryFileGuid.HasValue )
                {
                    var binaryFile = new BinaryFileService( newRockContext ).Get( binaryFileGuid.Value );
                    if ( binaryFile != null )
                    {
                        backgroundCheck.ResponseDocumentId = binaryFile.Id;
                    }
                }
            }
            
            newRockContext.SaveChanges();

            if ( createdNewAttribute )
            {
                AttributeCache.FlushEntityAttributes();
            }

        }
        
        /// <summary>
        /// Saves the attribute value.
        /// </summary>
        /// <param name="workflow">The workflow.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="fieldType">Type of the field.</param>
        /// <param name="rockContext">The rock context.</param>
        /// <param name="qualifiers">The qualifiers.</param>
        /// <returns></returns>
        private static bool SaveAttributeValue( Rock.Model.Workflow workflow, string key, string value,
            FieldTypeCache fieldType, RockContext rockContext, Dictionary<string, string> qualifiers = null )
        {
            bool createdNewAttribute = false;

            if ( workflow.Attributes.ContainsKey( key ) )
            {
                workflow.SetAttributeValue( key, value );
            }
            else
            {
                // Read the attribute
                var attributeService = new AttributeService( rockContext );
                var attribute = attributeService
                    .Get( workflow.TypeId, "WorkflowTypeId", workflow.WorkflowTypeId.ToString() )
                    .Where( a => a.Key == key )
                    .FirstOrDefault();

                // If workflow attribute doesn't exist, create it 
                // ( should only happen first time a background check is processed for given workflow type)
                if ( attribute == null )
                {
                    attribute = new Rock.Model.Attribute();
                    attribute.EntityTypeId = workflow.TypeId;
                    attribute.EntityTypeQualifierColumn = "WorkflowTypeId";
                    attribute.EntityTypeQualifierValue = workflow.WorkflowTypeId.ToString();
                    attribute.Name = key.SplitCase();
                    attribute.Key = key;
                    attribute.FieldTypeId = fieldType.Id;
                    attributeService.Add( attribute );

                    if ( qualifiers != null )
                    {
                        foreach ( var keyVal in qualifiers )
                        {
                            var qualifier = new Rock.Model.AttributeQualifier();
                            qualifier.Key = keyVal.Key;
                            qualifier.Value = keyVal.Value;
                            attribute.AttributeQualifiers.Add( qualifier );
                        }
                    }

                    createdNewAttribute = true;
                }

                // Set the value for this action's instance to the current time
                var attributeValue = new Rock.Model.AttributeValue();
                attributeValue.Attribute = attribute;
                attributeValue.EntityId = workflow.Id;
                attributeValue.Value = value;
                new AttributeValueService( rockContext ).Add( attributeValue );
            }

            return createdNewAttribute;
        }

        /// <summary>
        /// Saves the file.
        /// </summary>
        /// <param name="binaryFileAttribute">The binary file attribute.</param>
        /// <param name="url">The URL.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        private static Guid? SaveFile( AttributeCache binaryFileAttribute, string url, string fileName )
        {
            // get BinaryFileType info
            if ( binaryFileAttribute != null &&
                binaryFileAttribute.QualifierValues != null &&
                binaryFileAttribute.QualifierValues.ContainsKey( "binaryFileType" ) )
            {
                Guid? fileTypeGuid = binaryFileAttribute.QualifierValues["binaryFileType"].Value.AsGuidOrNull();
                if ( fileTypeGuid.HasValue )
                {
                    RockContext rockContext = new RockContext();
                    BinaryFileType binaryFileType = new BinaryFileTypeService( rockContext ).Get( fileTypeGuid.Value );

                    if ( binaryFileType != null )
                    {
                        byte[] data = null;

                        using ( WebClient wc = new WebClient() )
                        {
                            data = wc.DownloadData( url );
                        }

                        BinaryFile binaryFile = new BinaryFile();
                        binaryFile.Guid = Guid.NewGuid();
                        binaryFile.IsTemporary = true;
                        binaryFile.BinaryFileTypeId = binaryFileType.Id;
                        binaryFile.MimeType = "application/pdf";
                        binaryFile.FileName = fileName;
                        //binaryFile.FileSize = data.Length;  //v7
                        binaryFile.ContentStream = new MemoryStream( data );

                        var binaryFileService = new BinaryFileService( rockContext );
                        binaryFileService.Add( binaryFile );

                        rockContext.SaveChanges();

                        return binaryFile.Guid;
                    }
                }
            }

            return null;
        }
    }

    public class Object
    {
        public string id { get; set; }
        public string @object { get; set; }
        public string uri { get; set; }
        public string status { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? completed_at { get; set; }
        public DateTime? revised_at { get; set; }
        public DateTime? upgraded_at { get; set; }
        public int? turnaround_time { get; set; }
        public DateTime? due_time { get; set; }
        public string package { get; set; }
        public List<string> tags { get; set; }
        public string adjudication { get; set; }
        public string assessment { get; set; }
        public string candidate_id { get; set; }
        public List<string> county_criminal_search_ids { get; set; }
        public List<string> document_ids { get; set; }
        public string federal_criminal_search_id { get; set; }
        public string global_watchlist_search_id { get; set; }
        public string motor_vehicle_report_id { get; set; }
        public string national_criminal_search_id { get; set; }
        public List<string> personal_reference_verification_ids { get; set; }
        public List<string> professional_reference_verification_ids { get; set; }
        public string sex_offender_search_id { get; set; }
        public string ssn_trace_id { get; set; }
        public List<string> state_criminal_search_ids { get; set; }
        public string terrorist_watchlist_search_id { get; set; }
        public string facis_search_id { get; set; }
    }

    public class Data
    {
        public Object @object { get; set; }
    }

    public class RootObject
    {
        public string id { get; set; }
        public string @object { get; set; }
        public string type { get; set; }
        public DateTime? created_at { get; set; }
        public Data data { get; set; }
        public string account_id { get; set; }
    }
}
