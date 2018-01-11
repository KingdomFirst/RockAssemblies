using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;

using Newtonsoft.Json;

namespace com.kfs.AdobeSign
{
    [Serializable]
    public class AgreementSigner
    {
        public AgreementSigner()
        {
            Id = Guid.NewGuid();
        }
        public Guid Id { get; set; }
        public SignerType SignerType { get; set; }

        [JsonIgnore]
        public string SignerTypeDescription
        {
            get
            {
                return GetSignerTypeDescription( SignerType );
            }
        }
        public int SigningOrder { get; set; }
        public int SignerCFID { get; set; }

        [JsonIgnore]
        public string SignerCFTitle
        {
            get
            {
                string title = String.Empty;
                //if ( SignerCFID > 0 )
                //{
                //    title = GetCustomFieldTitle();
                //}
                return title;
            }
        }


        public static string SerializeSignerList( List<AgreementSigner> signers )
        {
            return JsonConvert.SerializeObject( signers, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore } );
        }

        public static List<AgreementSigner> DeserializeList( string json )
        {
            List<AgreementSigner> signers = null;

            try
            {
                signers = JsonConvert.DeserializeObject<List<AgreementSigner>>( json );
            }
            catch ( Exception ex )
            {
                throw new Exception( "An error has occurred while retrieving Signer List", ex );
            }

            return signers;
        }

        public static Dictionary<int, string> GetSignerType()
        {
            Dictionary<int, string> signerTypes = new Dictionary<int, string>();
            int[] signerInfoIds = ( int[] ) Enum.GetValues( typeof( SignerType ) );

            foreach ( var item in signerInfoIds )
            {
                signerTypes.Add( item, GetSignerTypeDescription( ( SignerType ) item ) );
            }

            return signerTypes;
        }

        public static Dictionary<int, string> GetSignerCustomFields( int profileId )
        {
            //DataTable dt = new AgreementSignerData().GetSignerFieldsByProfileId( profileId );
            Dictionary<int, string> cfDictionary = new Dictionary<int, string>();
            //foreach ( DataRow dr in dt.Rows )
            //{
            //    cfDictionary.Add( (int)dr["custom_field_id"], dr["title"].ToString() );
            //}

            return cfDictionary;
        }

        //private string GetCustomFieldTitle()
        //{
        //    return new CustomField( SignerCFID ).Title;
        //}

        private static string GetSignerTypeDescription( SignerType value )
        {
            System.Reflection.FieldInfo fi = value.GetType().GetField( value.ToString() );

            DescriptionAttribute a = ( ( DescriptionAttribute[] ) fi.GetCustomAttributes( typeof( DescriptionAttribute ), false ) ).FirstOrDefault();

            if ( a != null )
            {
                return a.Description;
            }
            else
            {
                return value.ToString();
            }
        }


    }

    //public class AgreementSignerData : Arena.DataLib.SqlData
    //{
    //    public DataTable GetSignerFieldsByProfileId( int profileId )
    //    {
    //        string sp_name = "dbo.cust_kfs_get_ProfileMemberCustomFieldByFieldType";
    //        ArrayList param = new ArrayList();
    //        param.Add( new SqlParameter( "@ProfileId", profileId ) );

    //        string assemblyName = typeof( FieldTypes.EchosignAgreementSignerField ).GetAssemblyQualifiedName();
    //        param.Add( new SqlParameter( "@FieldTypeAssembly", assemblyName ) );

    //        return base.ExecuteDataTable( sp_name, param );
    //    }
    //}
}
