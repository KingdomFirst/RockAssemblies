using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rock.Model;

namespace rocks.kfs.CyberSource.Model
{
    public class AddressToken
    {
        public Location BillingAddress { get; set; }

        public string OriginalToken { get; set; }
    }
}
