using SemicolonSystem.Model.Enum;

namespace SemicolonSystem.Model
{
    public class MatchingRowModel
    {
        public string Name { get; set; }

        public string Sex { get; set; }

        public string Property1 { get; set; }

        public string Property2 { get; set; }

        public string Property3 { get; set; }

        public string Model { get; set; }

        public MatchingLevel MatchingLevel { get; set; }

        public string SheetName { get; set; }
    }
}
