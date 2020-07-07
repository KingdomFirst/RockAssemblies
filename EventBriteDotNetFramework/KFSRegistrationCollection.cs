using Arena.Custom.KFS.ArenaData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBriteDotNetFramework
{
    class KFSRegistrationCollection
    {
        public List<KFSEventbriteRegistrant> GetEventbriteRegistrants( int profileid )
        {
            var retVar = new List<KFSEventbriteRegistrant>();
            var registrants = new Arena.Event.RegistrantCollection( profileid );
            foreach ( var rec in registrants )
            {
                var newRec = new KFSEventbriteRegistrant
                {
                    Registrant = rec
                };
                var MemberFieldCollection = new Arena.Core.ProfileMemberFieldValueCollection( profileid, rec.PersonID );


                foreach ( var field in MemberFieldCollection )
                {
                    if ( newRec.MemberFieldValuesByGuid == null )
                    {
                        newRec.MemberFieldValuesByGuid = new Dictionary<Guid, string>();
                    }
                    newRec.MemberFieldValuesByGuid.Add( field.Guid, field.SelectedValue );
                    if ( newRec.MemberFieldValuesById == null )
                    {
                        newRec.MemberFieldValuesById = new Dictionary<int, string>();
                    }
                    newRec.MemberFieldValuesById.Add( field.CustomFieldId, field.SelectedValue );
                }
                retVar.Add( newRec );
            }
            return retVar;
        }
    }

    class KFSEventbriteRegistrant
    {
        public Dictionary<Guid, string> MemberFieldValuesByGuid { get; set; }
        public Dictionary<int, string> MemberFieldValuesById { get; set; }
        public Event.Registrant Registrant { get; set; }
    }
}
