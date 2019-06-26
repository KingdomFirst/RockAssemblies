using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json;

namespace com.kfs.AdobeSign.RestAPI
{
    public class Reminder
    {
        private AdobeSignClient mClient = null;

        private Reminder() { }
        public Reminder( AdobeSignClient client )
        {
            mClient = client;
        }

        public string SendReminder( string agreementId )
        {
            string apiPath = "reminders";
            string method = "POST";
            string response = String.Empty;

            ReminderCreationInfo info = new ReminderCreationInfo();
            info.AgreementId = agreementId;
            string json = JsonConvert.SerializeObject( info );
            RawResponse rawResp = mClient.SendRequest( apiPath, method, null, null, json );

            if ( rawResp.StatusCode != System.Net.HttpStatusCode.Created )
            {
                ErrorResponse errResp = JsonConvert.DeserializeObject<ErrorResponse>( rawResp.jsonItem );
                if ( rawResp.StatusCode == System.Net.HttpStatusCode.Forbidden || rawResp.StatusCode == System.Net.HttpStatusCode.NotFound )
                {
                    response = string.Format( "{0} - {1}", rawResp.StatusCode, errResp.ErrorCode );
                }
                else
                {
                    throw new Exception( string.Format( "An error has occurred while sending agreement reminder. Code {0}. Message {1}", errResp.ErrorCode, errResp.ErrorMessage ) );
                }
            }

            ReminderCreationResult reminderResult = JsonConvert.DeserializeObject<ReminderCreationResult>( rawResp.jsonItem );
            return reminderResult.result;
        }
    }

    #region Send Reminder Request

    public class ReminderCreationInfo
    {
        [JsonProperty( PropertyName = "agreementId", Required = Required.Always )]
        public string AgreementId { get; set; }

        [JsonProperty( PropertyName = "comment", NullValueHandling = NullValueHandling.Ignore )]
        public string Comment { get; set; }
    }
    #endregion

    #region SendReminderResponse

    public class ParticipantEmailSetInfo
    {
        [JsonProperty( PropertyName = "participantEmail", Required = Required.Always )]
        public string ParticipantEmail { get; set; }
    }

    public class ParticipantEmailsSet
    {
        [JsonProperty( "participantEmailSetInfo", Required = Required.Always )]
        public List<ParticipantEmailSetInfo> ParticipantEmailSetInfo { get; set; }
    }

    public class ReminderCreationResult
    {
        [JsonProperty( PropertyName = "participantEmailsSet" )]
        public List<ParticipantEmailsSet> ParticipantEmailsSet { get; set; }
        [JsonProperty( PropertyName = "result" )]
        public string result { get; set; }
    }

    #endregion  

}
