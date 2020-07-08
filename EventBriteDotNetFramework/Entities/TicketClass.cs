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

namespace EventBriteDotNetFramework.Entities
{
  public class TicketClass
  {
    public long Id { get; set; }

    public string Name { get; set; }

    public object Cost { get; set; }

    public object Fee { get; set; }

    public object Currency { get; set; }

    public bool Free { get; set; }

    public int MinimumQuantity { get; set; }

    public object MaximumQuantity { get; set; }

    public int QuantityTotal { get; set; }

    public int QuantitySold { get; set; }

    public object SalesStart { get; set; }

    public DateTime SalesEnd { get; set; }
  }
}