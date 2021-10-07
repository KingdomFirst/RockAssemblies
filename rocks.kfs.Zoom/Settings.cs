using JWT.Algorithms;
using JWT.Builder;
using Rock.Data;
using Rock.Model;
using Rock.Security;
using Rock.Web.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rocks.kfs.Zoom
{
    public class Settings
    {
        /// <summary>
        /// Gets the token value.
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
                            .AddClaim( "exp", DateTimeOffset.UtcNow.AddHours( 1 ).ToUnixTimeSeconds() )
                            .AddClaim( "iat", DateTimeOffset.UtcNow.ToUnixTimeSeconds() )
                            .Encode();
                    }
                }
            }
            return tokenString;
        }

        /// <summary>
        /// Gets the settings.
        /// </summary>
        /// <param name="rockContext">The rock context.</param>
        /// <returns></returns>
        private static List<AttributeValue> GetSettings( RockContext rockContext )
        {
            var zoomEntityType = EntityTypeCache.Get( typeof( Zoom ) );
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
    }
}
