﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SemicolonSystem.Model
{
    public class OrderRow
    {
        public string Name { get; set; }

        public string Sex { get; set; }

        public string Property1 { get; set; }

        public string Property2 { get; set; }

        public string Property3 { get; set; }

        public List<SizeRuleItemModel> SizeRules { get; set; }
    }
}
