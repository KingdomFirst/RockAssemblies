using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rocks.kfs.CyberSource
{
    public class CyberSourceTypes
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public enum BillingPeriodUnit
        {
            // D - day
            // M - month
            // W - week
            // Y - year
            [Description("day")]
            D,
            [Description("month")]
            M,
            [Description("week")]
            W,
            [Description("year")]
            Y
        }
    }
}
