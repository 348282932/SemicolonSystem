using System.Collections.Generic;

namespace SemicolonSystem.Model
{
    public class OrderModel
    {
        public string SheetName { get; set; }

        public List<OrderRow> OrderRows { get; set; }
    }
}
