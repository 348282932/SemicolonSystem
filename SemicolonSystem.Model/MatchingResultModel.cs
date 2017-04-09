using System.Collections.Generic;

namespace SemicolonSystem.Model
{
    public class MatchingResultModel
    {
        public string SizeRuleName { get; set; }

        public string Sex { get; set; }

        public List<MatchingSizeRuleModel> Items { get; set; }
    }
}
