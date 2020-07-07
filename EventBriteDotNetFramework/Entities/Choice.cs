using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBriteDotNetFramework.Entities
{
    public class Choice
    {
        public HybridString Answer { get; set; }
        public string Id { get; set; }
        public List<int> Subquestion_Ids { get; set; }
    }
}
