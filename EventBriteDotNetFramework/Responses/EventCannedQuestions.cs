using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventBriteDotNetFramework.Entities;

namespace EventBriteDotNetFramework.Responses
{
    public class EventCannedQuestions
    {
        public Pagination Pagination { get; set; }
        public List<QuestionEntity> Questions { get; set; }
    }
}
