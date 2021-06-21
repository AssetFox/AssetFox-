using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BridgeCareCore.Services.SummaryReport.Visitors;

namespace BridgeCareCore.Services.SummaryReport.Models
{
    /// <summary>A region model, together with its location in the worksheet.</summary>
    public class AnchoredExcelRegionModel: IExcelWorksheetContentModel
    {
        public RowBasedExcelRegionModel Region { get; set; }
        public int StartRow { get; set; } = 1;
        public int StartColumn { get; set; } = 1;
        public TOutput Accept<TOutput, THelper>(IExcelWorksheetModelVisitor<THelper, TOutput> visitor, THelper helper)
            => visitor.Visit(this, helper);
    }
}
