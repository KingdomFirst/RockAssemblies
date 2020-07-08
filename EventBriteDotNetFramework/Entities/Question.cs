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
