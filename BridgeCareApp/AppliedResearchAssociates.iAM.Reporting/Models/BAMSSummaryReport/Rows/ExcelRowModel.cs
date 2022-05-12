using System.Collections.Generic;

using AppliedResearchAssociates.iAM.Reporting.Interfaces.BAMSSummaryReport;
using AppliedResearchAssociates.iAM.Reporting.Models.BAMSSummaryReport.ExcelRanges;

namespace AppliedResearchAssociates.iAM.Reporting.Models.BAMSSummaryReport.Rows
{
    public class ExcelRowModel
    {
        public List<RelativeExcelRangeModel> Values { get; set; }
        /// <summary>When writing a cell, the EveryCell models are written first.
        /// Therefore, style entries in EveryCell can be overridden by the Values.</summary>
        public IExcelModel EveryCell { get; set; } = (IExcelModel)StackedExcelModels.Empty;
    }
}
