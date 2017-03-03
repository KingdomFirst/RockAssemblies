using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.kfs.Reporting.SQLReportingServices
{
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

        public static ReportingServiceItem GetFoldersTree( string rootPath, bool recursive, bool includeHidden )
        {
            var rawItems = GetFoldersFlat( rootPath, recursive, includeHidden );
            rootPath = new ReportingServicesProvider().GetFolderPath( rootPath );
            ReportingServiceItem rsi = new ReportingServiceItem { Type = ItemType.Folder, Path = rootPath, Name = rootPath.Substring( rootPath.LastIndexOf( "/" ) + 1 ) };
            rsi.Children = LoadChildren( rawItems, rsi.Path );

            return rsi;
        }

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

        public static List<ReportingServiceItem> GetReportFlat( string rootPath, bool recursive, bool includeHidden )
        {
            var rawitems = GetItemsFlat( rootPath, recursive, includeHidden );
            return rawitems.Where( r => r.Type == ItemType.Report )
                .Where( r => includeHidden || r.Hidden == false ).ToList();
        }

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

        private static List<ReportingServiceItem> LoadChildren( List<ReportingServiceItem> rawItems, string parentPath )
        {
            List<ReportingServiceItem> items = new List<ReportingServiceItem>();

            foreach ( var item in rawItems.Where(i => i.ParentPath == parentPath) )
            {
                if ( item.Type == ItemType.Folder )
                {
                    item.Children = LoadChildren( rawItems, item.Path );
                }
                items.Add( item );
            }

            return items;
        }

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



    public enum ItemType
    {
        Folder = 0,
        Report = 1, 
        DataSource = 2

    }
}
