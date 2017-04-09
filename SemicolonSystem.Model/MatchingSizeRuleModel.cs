using System.Collections.Generic;

namespace SemicolonSystem.Model
{
    public class MatchingSizeRuleModel
    {
        public string SheetName { get; set; }

        public List<MatchingRowModel> MatchingRows { get; set; }
    }

}
