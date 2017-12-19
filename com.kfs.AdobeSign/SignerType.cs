using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.kfs.AdobeSign
{
    public enum SignerType
    {
        [Description( "Applies To" )]
        APPLIES_TO,

        [Description( "Assigned To" )]
        ASSIGNED_TO,

        [Description( "Custom" )]
        CUSTOM
    }
}
