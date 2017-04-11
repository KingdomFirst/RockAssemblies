using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;

using Rock;
using Rock.Field;
using Rock.Web.UI.Controls;
namespace com.kfs.Reporting.SQLReportingServices.Field.Type
{
    public class ReportItemFieldType : FieldType
    {

        const string ITEM_TYPE_CONFIG_KEY = "itemType";
        const string IS_RECURSIVE_CONFIG_KEY = "isRecursive";
        const string SHOW_HIDDEN_CONFIG_KEY = "showHiddenItems";


        #region Configuration
        public override List<string> ConfigurationKeys()
        {
            List<string> configurationKeys = new List<string>();
            configurationKeys.Add( "itemType" );
            configurationKeys.Add( "isRecursive" );
            configurationKeys.Add( "showHiddenItems" );
            return configurationKeys;

        }
        public override List<System.Web.UI.Control> ConfigurationControls()
        {
            List<Control> controls = new List<Control>();
            RockDropDownList ddl = new RockDropDownList();
            controls.Add( ddl );
            ddl.Label = "Report Item Type";
            ddl.Help = "The type of reporting services item that is selectable in the tree";
            ddl.AutoPostBack = true;
            ddl.SelectedIndexChanged += OnQualifierUpdated;

            foreach ( var item in Enum.GetValues(typeof(ItemType) ) )
            {
                ddl.Items.Add( new ListItem( item.ToString().SplitCase(), ( ( int )item ).ToString() ) );
            }

            RockCheckBox cbChild = new RockCheckBox();
            controls.Add( cbChild );
            cbChild.Label = "Include Child Items";
            cbChild.Help = "Include items from all sub folders";

            RockCheckBox cbHidden = new RockCheckBox();
            controls.Add( cbHidden );
            cbHidden.Label = "Show Hidden Items";
            cbHidden.Help = "Include Reporting Services items that are marked as hidden";

            return controls; 


        }

        public override Dictionary<string, ConfigurationValue> ConfigurationValues( List<Control> controls )
        {
            Dictionary<string, ConfigurationValue> configurationValues = new Dictionary<string, ConfigurationValue>();
            configurationValues.Add( ITEM_TYPE_CONFIG_KEY, new ConfigurationValue( "Reporting Service Item type", "The type of reporting services item that is selectable in the tree.", string.Empty ) );
            configurationValues.Add( IS_RECURSIVE_CONFIG_KEY, new ConfigurationValue( "Include Child Items", "Include items from all sub folders", string.Empty ) );
            configurationValues.Add( SHOW_HIDDEN_CONFIG_KEY, new ConfigurationValue( "Show Hidden Items", "Include Reporting Services items that are marked as hidden.", string.Empty ) );
            if ( controls != null && controls.Count >= 3 )
            {
                var ddlItemType = controls[0] as RockDropDownList;
                var cbChild = controls[1] as RockCheckBox;
                var cbHidden = controls[2] as RockCheckBox;
                if ( ddlItemType != null )
                {
                    configurationValues[ITEM_TYPE_CONFIG_KEY].Value = ddlItemType.SelectedValue.ToString();  
                }

                if ( cbChild != null )
                {
                    configurationValues[IS_RECURSIVE_CONFIG_KEY].Value = cbChild.Checked.ToString();
                }

                if ( cbHidden != null )
                {
                    configurationValues[SHOW_HIDDEN_CONFIG_KEY].Value = cbHidden.Checked.ToString();
                }

            }

            return configurationValues;
        }

        public override void SetConfigurationValues( List<Control> controls, Dictionary<string, ConfigurationValue> configurationValues )
        {
            if ( controls != null && configurationValues != null && controls.Count >= 3 )
            {
                var ddlItemType = controls[0] as RockDropDownList;
                var cbChild = controls[1] as RockCheckBox;
                var cbHidden = controls[2] as RockCheckBox;

                if ( ddlItemType != null && configurationValues.ContainsKey( ITEM_TYPE_CONFIG_KEY ) )
                {
                    ddlItemType.SelectedValue = configurationValues[ITEM_TYPE_CONFIG_KEY].Value;
                }

                if ( cbChild != null && configurationValues.ContainsKey( IS_RECURSIVE_CONFIG_KEY ) )
                {
                    cbChild.Checked = configurationValues[IS_RECURSIVE_CONFIG_KEY].Value.AsBoolean();
                }

                if ( cbHidden != null && configurationValues.ContainsKey( SHOW_HIDDEN_CONFIG_KEY ) )
                {
                    cbHidden.Checked = configurationValues[SHOW_HIDDEN_CONFIG_KEY].Value.AsBoolean();
                }
            }
        }
        #endregion

        #region formatting
        public override string FormatValue( Control parentControl, string value, Dictionary<string, ConfigurationValue> configurationValues, bool condensed )
        {
            string formattedValue = string.Empty;
            string path = value.Replace( "$", "/" );
            var propIn = new API.Property[] { new API.Property { Name = "Name", Value = "" }, new API.Property { Name = "Description" } };
            API.Property[] propOut;
            var provider = new ReportingServicesProvider();
            var client = provider.GetAPIClient( UserType.Browser );
             client.GetProperties( null, null, value, propIn, out propOut );

            if ( propOut.Length > 0 && propOut.Where(p => p.Name == "Name").FirstOrDefault() != null )
            {
                formattedValue = propOut.Where( p => p.Name == "Name" ).Select( p => p.Value ).FirstOrDefault();
            }

            return formattedValue;
        }
        #endregion
    }
}
