using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rock.Attribute;
using Rock.Field;

using com.kfs.Reporting.SQLReportingServices;
namespace com.kfs.Reporting.SQLReportingServices.Attribute
{
    public class ReportingServicesFolderAttribute : FieldAttribute
    {
        public ReportingServicesFolderAttribute( string name = "Reporting Services Item", string description = "", ItemType rsItemType = ItemType.Report, bool isRecursive = false, bool showHiddenItems = false, bool required = true, string defaultValue = "", string category = "", int order = 0, string key = null ) :
            base( name, description, required, defaultValue, category, order, key, typeof( Rock.Field.Types.TextFieldType ).FullName, "Rock" )
        {
            FieldConfigurationValues.Add( "itemType", new ConfigurationValue( rsItemType.ToString() ) );
            FieldConfigurationValues.Add( "isRecursive", new ConfigurationValue( isRecursive.ToString() ) );
            FieldConfigurationValues.Add( "showHiddenItems", new ConfigurationValue( showHiddenItems.ToString() ) );
        }
    }
}
