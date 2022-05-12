using System.Collections.Generic;

using AppliedResearchAssociates.iAM.Reporting.Interfaces.BAMSSummaryReport.Worksheets;

namespace AppliedResearchAssociates.iAM.Reporting.Models.BAMSSummaryReport.Worksheets
{
    public class ExcelWorksheetModel
    {
        public string TabName { get; set; }
        public List<IExcelWorksheetContentModel> Content { get; set; }
    }
}
