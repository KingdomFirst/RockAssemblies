using JWT.Algorithms;
using JWT.Builder;
using Rock.Data;
using Rock.Model;
using Rock.Security;
using Rock.Web.Cache;
using System;
using System.Collections.Generic;
using System.Linq;

namespace rocks.kfs.Zoom
{
    public class Settings
    {
        /// <summary>
        /// Builds a JSON Web Token value.
        /// </summary>
        /// <returns></returns>
        public static string GetJwtString()
        {
            string tokenString = null;
            using ( RockContext rockContext = new RockContext() )
            {
                var settings = GetSettings( rockContext );
                if ( settings != null )
                {
                    var zoomApiKey = GetSettingValue( settings, "KFSZoomApiKey", true );
                    var zoomApiSecret = GetSettingValue( settings, "KFSZoomApiSecret", true );
                    if ( !String.IsNullOrWhiteSpace( zoomApiKey ) && !String.IsNullOrWhiteSpace( zoomApiKey ) )
                    {
                        tokenString = JwtBuilder.Create()
                            .WithAlgorithm( new HMACSHA256Algorithm() )
                            .WithSecret( zoomApiSecret )
                            .AddClaim( "aud", null )
                            .AddClaim( "iss", zoomApiKey )
                            .AddClaim( "exp", DateTimeOffset.UtcNow.AddHours( 1 ).Subtract( new DateTime( 1970, 1, 1, 0, 0, 0, DateTimeKind.Utc ) ).TotalSeconds )  // Replace .Subtract() with  .ToUnixTimeSeconds() once assembly is moved to .NET 4.6+
                            .AddClaim( "iat", DateTimeOffset.UtcNow.Subtract( new DateTime( 1970, 1, 1, 0, 0, 0, DateTimeKind.Utc ) ).TotalSeconds )  // Replace .Subtract() with  .ToUnixTimeSeconds() once assembly is moved to .NET 4.6+
                            .Encode();
                    }
                }
            }
            return tokenString;
        }

        /// <summary>
        /// Gets the API Key value.
        /// </summary>
        /// <returns></returns>
        public static string GetApiKey()
        {
            string zoomApiKey = null;
            using ( RockContext rockContext = new RockContext() )
            {
                var settings = GetSettings( rockContext );
                if ( settings != null )
                {
                    zoomApiKey = GetSettingValue( settings, "KFSZoomApiKey", true );
                }
            }
            return zoomApiKey;
        }

        /// <summary>
        /// Gets the API Secret value.
        /// </summary>
        /// <returns></returns>
        public static string GetApiSecret()
        {
            string zoomApiSecret = null;
            using ( RockContext rockContext = new RockContext() )
            {
                var settings = GetSettings( rockContext );
                if ( settings != null )
                {
                    zoomApiSecret = GetSettingValue( settings, "KFSZoomApiSecret", true );
                }
            }
            return zoomApiSecret;
        }

        /// <summary>
        /// Gets the Zoom webhook url.
        /// </summary>
        /// <returns></returns>
        public static string GetWebhookUrl()
        {
            string webhookUrl = null;
            var appUrl = GlobalAttributesCache.Get().GetValue( "InternalApplicationRoot" );
            if ( !string.IsNullOrWhiteSpace( appUrl ) )
            {
                webhookUrl = string.Format( "{0}Plugins/rocks_kfs/Zoom/Webhook.ashx", appUrl );
            }
            return webhookUrl;
        }

        /// <summary>
        /// Saves the API Key and API Secret.
        /// </summary>
        /// <param name="apiKey">The api key.</param>
        /// <param name="apiSecret">The api secret.</param>

        public static void SaveApiSettings( string apiKey, string apiSecret )
        {
            using ( var rockContext = new RockContext() )
            {
                var settings = GetSettings( rockContext );
                SetSettingValue( rockContext, settings, "KFSZoomApiKey", apiKey, true );
                SetSettingValue( rockContext, settings, "KFSZoomApiSecret", apiSecret, true );

                rockContext.SaveChanges();
            }
        }

        /// <summary>
        /// Gets the settings.
        /// </summary>
        /// <param name="rockContext">The rock context.</param>
        /// <returns></returns>
        private static List<AttributeValue> GetSettings( RockContext rockContext )
        {
            var zoomEntityType = EntityTypeCache.Get( ZoomGuid.EntityType.ZOOM );
            if ( zoomEntityType != null )
            {
                var service = new AttributeValueService( rockContext );
                return service.Queryable( "Attribute" )
                    .Where( v => v.Attribute.EntityTypeId == zoomEntityType.Id )
                    .ToList();
            }

            return null;
        }

        /// <summary>
        /// Gets the setting value.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        private static string GetSettingValue( List<AttributeValue> values, string key, bool encryptedValue = false )
        {
            string value = values
                .Where( v => v.AttributeKey == key )
                .Select( v => v.Value )
                .FirstOrDefault();
            if ( encryptedValue && !string.IsNullOrWhiteSpace( value ) )
            {
                try { value = Encryption.DecryptString( value ); }
                catch { }
            }

            return value;
        }

        /// <summary>
        /// Sets the setting value.
        /// </summary>
        /// <param name="rockContext">The rock context.</param>
        /// <param name="values">The values.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        private static void SetSettingValue( RockContext rockContext, List<AttributeValue> values, string key, string value, bool encryptValue = false )
        {
            if ( encryptValue && !string.IsNullOrWhiteSpace( value ) )
            {
                try { value = Encryption.EncryptString( value ); }
                catch { }
            }

            var attributeValue = values
                .Where( v => v.AttributeKey == key )
                .FirstOrDefault();
            if ( attributeValue != null )
            {
                attributeValue.Value = value;
            }
            else
            {
                var zoomEntityType = EntityTypeCache.Get( ZoomGuid.EntityType.ZOOM );
                if ( zoomEntityType != null )
                {
                    var attribute = new AttributeService( rockContext )
                        .Queryable()
                        .Where( a =>
                            a.EntityTypeId == zoomEntityType.Id &&
                            a.Key == key
                        )
                        .FirstOrDefault();

                    if ( attribute != null )
                    {
                        attributeValue = new AttributeValue();
                        new AttributeValueService( rockContext ).Add( attributeValue );
                        attributeValue.AttributeId = attribute.Id;
                        attributeValue.Value = value;
                        attributeValue.EntityId = 0;
                    }
                }
            }
        }
    }
}
