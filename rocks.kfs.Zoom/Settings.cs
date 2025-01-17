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

using Rock.Data;
using Rock.Model;
using Rock.Security;
using Rock.Web.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using ZoomDotNetFramework;

namespace rocks.kfs.Zoom
{
    public class Settings
    {
        /// <summary>
        /// Gets and OAuth token from Zoom API.
        /// </summary>
        /// <returns></returns>
        public static string GetOauthString()
        {
            string tokenString = null;
            using ( RockContext rockContext = new RockContext() )
            {
                var settings = GetSettings( rockContext );
                if ( settings != null )
                {
                    var zoomAppAccountId = GetSettingValue( settings, "rocks.kfs.ZoomAppAccountId", true );
                    var zoomAppClientId = GetSettingValue( settings, "rocks.kfs.ZoomAppClientId", true );
                    var zoomAppSecret = GetSettingValue( settings, "rocks.kfs.ZoomAppClientSecret", true );
                    tokenString = ZoomApi.GetOauthToken( zoomAppAccountId, zoomAppClientId, zoomAppSecret );
                }
            }
            return tokenString;
        }

        /// <summary>
        /// Gets the App Account Id value.
        /// </summary>
        /// <returns></returns>
        public static string GetAppAccountId()
        {
            string appAccountId = null;
            using ( RockContext rockContext = new RockContext() )
            {
                var settings = GetSettings( rockContext );
                if ( settings != null )
                {
                    appAccountId = GetSettingValue( settings, "rocks.kfs.ZoomAppAccountId", true );
                }
            }
            return appAccountId;
        }

        /// <summary>
        /// Gets the App Client Id value.
        /// </summary>
        /// <returns></returns>
        public static string GetAppClientId()
        {
            string appClientId = null;
            using ( RockContext rockContext = new RockContext() )
            {
                var settings = GetSettings( rockContext );
                if ( settings != null )
                {
                    appClientId = GetSettingValue( settings, "rocks.kfs.ZoomAppClientId", true );
                }
            }
            return appClientId;
        }

        /// <summary>
        /// Gets the App Client Secret value.
        /// </summary>
        /// <returns></returns>
        public static string GetAppClientSecret()
        {
            string zoomAppClientSecret = null;
            using ( RockContext rockContext = new RockContext() )
            {
                var settings = GetSettings( rockContext );
                if ( settings != null )
                {
                    zoomAppClientSecret = GetSettingValue( settings, "rocks.kfs.ZoomAppClientSecret", true );
                }
            }
            return zoomAppClientSecret;
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
        /// Saves the App Account ID, Client ID, and Client Secret attribute values.
        /// </summary>
        /// <param name="appAccountId">The Account Id.</param>
        /// <param name="appClientId">The Client Id.</param>
        /// <param name="appClientSecret">The Client Secret.</param>

        public static void SaveApiSettings( string appAccountId, string appClientId, string appClientSecret )
        {
            using ( var rockContext = new RockContext() )
            {
                var settings = GetSettings( rockContext );
                SetSettingValue( rockContext, settings, "rocks.kfs.ZoomAppAccountId", appAccountId, true );
                SetSettingValue( rockContext, settings, "rocks.kfs.ZoomAppClientId", appClientId, true );
                SetSettingValue( rockContext, settings, "rocks.kfs.ZoomAppClientSecret", appClientSecret, true );

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
