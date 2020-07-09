// <copyright>
// Copyright 2020 by Kingdom First Solutions
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
using System.Text;
using System.Threading.Tasks;

namespace EventbriteDotNetFramework
{
    class KFSRegistrationCollection
    {
        public List<KFSEventbriteRegistrant> GetEventbriteRegistrants( int profileid )
        {
            var retVar = new List<KFSEventbriteRegistrant>();
            //var registrants = new Arena.Event.RegistrantCollection( profileid );
            //foreach ( var rec in registrants )
            //{
            //    var newRec = new KFSEventbriteRegistrant
            //    {
            //        Registrant = rec
            //    };
            //    var MemberFieldCollection = new Arena.Core.ProfileMemberFieldValueCollection( profileid, rec.PersonID );


            //    foreach ( var field in MemberFieldCollection )
            //    {
            //        if ( newRec.MemberFieldValuesByGuid == null )
            //        {
            //            newRec.MemberFieldValuesByGuid = new Dictionary<Guid, string>();
            //        }
            //        newRec.MemberFieldValuesByGuid.Add( field.Guid, field.SelectedValue );
            //        if ( newRec.MemberFieldValuesById == null )
            //        {
            //            newRec.MemberFieldValuesById = new Dictionary<int, string>();
            //        }
            //        newRec.MemberFieldValuesById.Add( field.CustomFieldId, field.SelectedValue );
            //    }
            //    retVar.Add( newRec );
            //}
            return retVar;
        }
    }

    public class KFSEventbriteRegistrant
    {
        public Dictionary<Guid, string> MemberFieldValuesByGuid { get; set; }
        public Dictionary<int, string> MemberFieldValuesById { get; set; }
        //public Event.Registrant Registrant { get; set; }
    }
}
