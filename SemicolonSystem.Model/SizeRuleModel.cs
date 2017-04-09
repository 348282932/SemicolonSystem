using System.Collections.Generic;

namespace SemicolonSystem.Model
{
    public class SizeRuleModel
    {
        public string Name { get; set; }

        public string Sex { get; set; }

        public List<SizeRuleItemModel> Items { get; set; }
    }
}
