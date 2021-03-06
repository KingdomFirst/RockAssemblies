﻿// <copyright>
// Copyright 2019 by Kingdom First Solutions
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
using System.Linq;

using rocks.kfs.Reporting.SQLReportingServices.API;

namespace rocks.kfs.Reporting.SQLReportingServices
{
    /// <summary>
    /// The Report Service Item
    /// </summary>
    public class ReportingServiceItem
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public ItemType Type { get; set; }
        public List<ReportingServiceItem> Children { get; set; }
        public bool Hidden { get; set; }

        internal string ParentPath
        {
            get
            {
                int ix = Path.IndexOf( Name );
                return Path.Substring( 0, ix - 1 );
            }
        }

        private static List<ReportingServiceItem> rawItems = null;

        /// <summary>
        /// Gets the folders in a flat list.
        /// </summary>
        /// <param name="rootPath">The root path.</param>
        /// <param name="recursive">if set to <c>true</c> [recursive].</param>
        /// <param name="includeHidden">if set to <c>true</c> [include hidden].</param>
        /// <returns></returns>
        public static List<ReportingServiceItem> GetFoldersFlat( string rootPath, bool recursive, bool includeHidden )
        {
            rawItems = new List<ReportingServiceItem>();

            var apiItems = GetCatalogItems( rootPath, recursive )
                .Where( c => c.TypeName.Equals( "folder", StringComparison.InvariantCultureIgnoreCase ) )
                .Where( c => includeHidden || c.Hidden == false );

            foreach ( var item in apiItems )
            {
                ReportingServiceItem i = new ReportingServiceItem();
                i.Name = item.Name;
                i.Path = item.Path;
                i.Type = ItemType.Folder;
                i.Hidden = item.Hidden;

                rawItems.Add( i );
            }

            return rawItems;
        }

        /// <summary>
        /// Gets the folder tree.
        /// </summary>
        /// <param name="rootPath">The root path.</param>
        /// <param name="recursive">if set to <c>true</c> [recursive].</param>
        /// <param name="includeHidden">if set to <c>true</c> [include hidden].</param>
        /// <returns></returns>
        public static ReportingServiceItem GetFoldersTree( string rootPath, bool recursive, bool includeHidden )
        {
            var rawItems = GetFoldersFlat( rootPath, recursive, includeHidden );
            rootPath = new ReportingServicesProvider().GetFolderPath( rootPath );
            ReportingServiceItem rsi = new ReportingServiceItem { Type = ItemType.Folder, Path = rootPath, Name = rootPath.Substring( rootPath.LastIndexOf( "/" ) + 1 ) };
            rsi.Children = LoadChildren( rawItems, rsi.Path );

            return rsi;
        }

        /// <summary>
        /// Gets the items in a flat list.
        /// </summary>
        /// <param name="rootPath">The root path.</param>
        /// <param name="recursive">if set to <c>true</c> [recursive].</param>
        /// <param name="includeHidden">if set to <c>true</c> [include hidden].</param>
        /// <returns></returns>
        private static List<ReportingServiceItem> GetItemsFlat( string rootPath, bool recursive, bool includeHidden )
        {
            var apiItems = GetCatalogItems( rootPath, recursive )
                .Where( i => includeHidden || i.Hidden == false );

            var rawItems = new List<ReportingServiceItem>();

            foreach ( var item in apiItems )
            {
                ReportingServiceItem i = new ReportingServiceItem();
                i.Name = item.Name;
                i.Path = item.Path;
                i.Hidden = item.Hidden;

                switch ( item.TypeName.ToLower() )
                {
                    case "folder":
                        i.Type = ItemType.Folder;
                        break;

                    case "report":
                        i.Type = ItemType.Report;
                        break;

                    case "datasource":
                        i.Type = ItemType.DataSource;
                        break;

                    default:
                        break;
                }

                rawItems.Add( i );
            }
            return rawItems;
        }

        /// <summary>
        /// Gets the report in a flat list.
        /// </summary>
        /// <param name="rootPath">The root path.</param>
        /// <param name="recursive">if set to <c>true</c> [recursive].</param>
        /// <param name="includeHidden">if set to <c>true</c> [include hidden].</param>
        /// <returns></returns>
        public static List<ReportingServiceItem> GetReportFlat( string rootPath, bool recursive, bool includeHidden )
        {
            var rawitems = GetItemsFlat( rootPath, recursive, includeHidden );
            return rawitems.Where( r => r.Type == ItemType.Report )
                .Where( r => includeHidden || r.Hidden == false ).ToList();
        }

        /// <summary>
        /// Gets the report tree.
        /// </summary>
        /// <param name="rootPath">The root path.</param>
        /// <param name="recursive">if set to <c>true</c> [recursive].</param>
        /// <param name="includeHidden">if set to <c>true</c> [include hidden].</param>
        /// <returns></returns>
        public static ReportingServiceItem GetReportTree( string rootPath, bool recursive, bool includeHidden )
        {
            var rawItems = GetItemsFlat( rootPath, recursive, includeHidden );

            rawItems.RemoveAll( i => i.Type == ItemType.DataSource );

            ReportingServiceItem rsi = new ReportingServiceItem();
            rootPath = new ReportingServicesProvider().GetFolderPath( rootPath );
            rsi.Name = rootPath.Substring( rootPath.LastIndexOf( "/" ) + 1 );
            rsi.Path = rootPath;
            rsi.Type = ItemType.Folder;

            rsi.Children = LoadChildren( rawItems, rsi.Path );

            return rsi;
        }

        /// <summary>
        /// Gets the report parameter list.
        /// </summary>
        /// <param name="reportPath">The report path.</param>
        /// <returns></returns>
        public static List<string> GetReportParameterList( string reportPath )
        {
            return GetReportParameterList( new ReportingServicesProvider(), reportPath );
        }

        /// <summary>
        /// Gets the report parameter list.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="reportPath">The report path.</param>
        /// <returns></returns>
        public static List<string> GetReportParameterList( ReportingServicesProvider provider, string reportPath )
        {
            var client = provider.GetAPIClient( UserType.Browser );
            ItemParameter[] reportParams = null;
            client.GetItemParameters( null, reportPath, null, true, null, null, out reportParams );

            var paramNames = new List<string>();
            foreach ( var p in reportParams )
            {
                paramNames.Add( p.Name );
            }

            return paramNames;
        }

        /// <summary>
        /// Gets the item by path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static ReportingServiceItem GetItemByPath( string path )
        {
            ReportingServicesProvider provider = new ReportingServicesProvider();
            path = provider.GetFolderPath( path );

            ReportingServiceItem rsItem = GetItemsFlat( provider.ReportPath, true, true )
                .Where( i => i.Path.Equals( path, StringComparison.InvariantCultureIgnoreCase ) )
                .FirstOrDefault();
            return rsItem;
        }

        /// <summary>
        /// Loads the children.
        /// </summary>
        /// <param name="rawItems">The raw items.</param>
        /// <param name="parentPath">The parent path.</param>
        /// <returns></returns>
        private static List<ReportingServiceItem> LoadChildren( List<ReportingServiceItem> rawItems, string parentPath )
        {
            List<ReportingServiceItem> items = new List<ReportingServiceItem>();

            foreach ( var item in rawItems.Where( i => i.ParentPath == parentPath ) )
            {
                if ( item.Type == ItemType.Folder )
                {
                    item.Children = LoadChildren( rawItems, item.Path );
                }
                items.Add( item );
            }

            return items;
        }

        /// <summary>
        /// Gets the catalog items.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="recursive">if set to <c>true</c> [recursive].</param>
        /// <returns></returns>
        private static API.CatalogItem[] GetCatalogItems( string path, bool recursive )
        {
            ReportingServicesProvider provider = new ReportingServicesProvider();
            path = provider.GetFolderPath( path );
            var apiClient = provider.GetAPIClient( UserType.Browser );

            API.CatalogItem[] catalog;
            apiClient.ListChildren( null, path, recursive, out catalog );

            return catalog;
        }
    }

    /// <summary>
    ///
    /// </summary>
    public enum ItemType
    {
        Folder = 0,
        Report = 1,
        DataSource = 2
    }
}
