using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBriteDotNetFramework.Entities
{
    public class QuestionEntity
    {
        public string Resource_Uri { get; set; }
        public string Id { get; set; }
        public HybridString Question { get; set; }
        public string Type { get; set; }
        public Boolean Required { get; set; }
        public List<Choice> Choices { get; set; }
        public List<TicketClass> Ticket_Classes { get; set; }
        public string Group_Id { get; set; }
        public string Respondent { get; set; }
    }
}
