using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SemicolonSystem.Model
{
    public class OrderRow
    {
        public string Name { get; set; }

        public string Sex { get; set; }

        public List<SizeRuleModel> SizeRules { get; set; }
    }
}
