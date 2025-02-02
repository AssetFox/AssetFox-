﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppliedResearchAssociates.iAM.ExcelHelpers
{
    public class ExcelRowModel
    {
        public List<RelativeExcelRangeModel> Values { get; set; }
        /// <summary>When writing a cell, the EveryCell models are written first.
        /// Therefore, style entries in EveryCell can be overridden by the Values.</summary>
        public IExcelModel EveryCell { get; set; } = StackedExcelModels.Empty;
    }
}
