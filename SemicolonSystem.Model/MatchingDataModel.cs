using System.Collections.Generic;

namespace SemicolonSystem.Model
{
    public class MatchingDataModel
    {
        public string SheetName { get; set; }

        public List<MatchingDataSheetModel> Items { get; set; }
    }
}
