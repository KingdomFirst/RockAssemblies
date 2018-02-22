using System;
using System.Text.RegularExpressions;

namespace com.kfs.Checkr
{
    class Checkr
    {
        public static string encodeClientCredentials( string Client, string Secret = "" )
        {
            string idAndSecret = Client + ":" + Secret;
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes( idAndSecret );
            return System.Convert.ToBase64String( plainTextBytes );
        }

        public static Uri checkrBaseUrl = new Uri( "https://api.checkr.com/" );

        public static string safeString( string inputString )
        {
            var safeString = string.Empty;

            safeString = Regex.Replace( inputString, @"[^a-zA-Z0-9\-,.' ]", "" );

            return safeString;
        }
    }
}
