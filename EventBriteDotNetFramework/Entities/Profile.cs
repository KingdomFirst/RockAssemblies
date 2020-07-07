using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBriteDotNetFramework.Entities
{
    public class Profile
    {
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public AttendeeAddress Addresses { get; set; }
        public string Gender { get; set; }
        public string Prefix { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Home_Phone { get; set; }
        public string Cell_Phone { get; set; }
        public string Work_Phone { get; set; }
        public DateTime Birth_Date { get; set; }
        public int Age { get; set; }
    }
}
