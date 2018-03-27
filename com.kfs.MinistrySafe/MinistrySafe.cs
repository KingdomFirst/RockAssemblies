using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.kfs.MinistrySafe
{
    class MinistrySafe
    {
        public static Uri BaseUrl( bool StagingMode = false )
        {
            if ( StagingMode )
            {
                return new Uri( "https://staging.ministrysafe.com/api/" );
            }
            else
            {
                return new Uri( "https://ministrysafe.com/api/" );
            }
        }
    }
}
