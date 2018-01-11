using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json;

namespace com.kfs.AdobeSign.RestAPI
{
    public class User
    {
        AdobeSignClient mClient = null;
        bool retryAttempt = false;

        public User()
        {
            mClient = new AdobeSignClient();
        }

        public User( AdobeSignClient client )
        {
            mClient = client;
        }

        public UserDetail GetUserDetail( string userId )
        {
            string path = string.Format( "users/{0}", userId );


            RawResponse resp = mClient.SendRequest( path, "Get" );

            if ( resp.StatusCode == System.Net.HttpStatusCode.NotFound )
            {
                return null;
            }
            else if ( resp.StatusCode != System.Net.HttpStatusCode.OK )
            {
                ErrorResponse errResp = JsonConvert.DeserializeObject<ErrorResponse>( resp.jsonItem );
                if ( errResp.ErrorCode == "INVALID_ACCESS_TOKEN" && retryAttempt == false )
                {
                    retryAttempt = true;
                    mClient.RefreshAccessToken();
                    return GetUserDetail( userId );
                }

                throw new Exception( string.Format( "An error has occurred while retrieving user information. Code: {0} Message: {1}", errResp.ErrorCode, errResp.ErrorMessage ) );

            }

            return JsonConvert.DeserializeObject<UserDetail>( resp.jsonItem );

        }

        public List<UserInfoItem> GetUserList()
        {
            string path = "users";


            RawResponse resp = mClient.SendRequest( path, "Get" );

            if ( resp.StatusCode != System.Net.HttpStatusCode.OK )
            {
                ErrorResponse errResp = JsonConvert.DeserializeObject<ErrorResponse>( resp.jsonItem );
                if ( resp.StatusCode == System.Net.HttpStatusCode.Unauthorized && errResp.ErrorCode == "INVALID_ACCESS_TOKEN" && !retryAttempt )
                {
                    mClient.RefreshAccessToken();
                    retryAttempt = true;
                    return GetUserList();
                }
                else
                {
                    throw new Exception( string.Format( "An error has occurred while retrieving the AdobeSign User List. Code: {0} Message:{1}.",
                        errResp.ErrorCode,
                        errResp.ErrorMessage ) );
                }
            }

            List<UserInfoItem> users = JsonConvert.DeserializeObject<UserInfoRespose>( resp.jsonItem ).UserInfoList;

            return users;

        }

    }

    public class UserDetail
    {
        public string account { get; set; }
        public string accountType { get; set; }
        public List<string> capabilityFlags { get; set; }
        public string channel { get; set; }
        public string company { get; set; }
        public string email { get; set; }
        public string firstName { get; set; }
        public string group { get; set; }
        public string groupId { get; set; }
        public string initials { get; set; }
        public string lastName { get; set; }
        public string locale { get; set; }
        public string optIn { get; set; }
        public string passwordExpiration { get; set; }
        public string phone { get; set; }
        public List<string> roles { get; set; }
        public string title { get; set; }
        public string userStatus { get; set; }
    }

    public class UserInfoRespose
    {
        [JsonProperty( "userInfoList" )]
        public List<UserInfoItem> UserInfoList { get; set; }
    }

    public class UserInfoItem
    {
        [JsonProperty( "email" )]
        public string Email { get; set; }

        [JsonProperty( "fullNameOrEmail" )]
        public string FullNameOrEmail { get; set; }

        [JsonProperty( "groupId" )]
        public string GroupId { get; set; }

        [JsonProperty( "userId" )]
        public string UserId { get; set; }

        [JsonProperty( PropertyName = "company" )]
        public string Company { get; set; }

    }
}
