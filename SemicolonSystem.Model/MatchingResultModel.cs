﻿using System.Collections.Generic;

namespace SemicolonSystem.Model
{
    public class MatchingResultModel
    {
        public string SheetName { get; set; }

        public List<MatchingRowModel> MatchingRows { get; set; }
    }

}
