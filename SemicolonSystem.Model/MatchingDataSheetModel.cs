using System.Collections.Generic;

namespace SemicolonSystem.Model
{
    public class MatchingDataSheetModel
    {
        public string Name { get; set; }

        public string Sex { get; set; }

        public string Property1 { get; set; }

        public string Property2 { get; set; }

        public string Property3 { get; set; }

        public string SheetName { get; set; }

        public List<MatchingDataSheetItemModel> Items { get; set; }
    }
}
