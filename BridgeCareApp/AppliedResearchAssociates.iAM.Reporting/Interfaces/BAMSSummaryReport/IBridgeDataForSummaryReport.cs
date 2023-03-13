using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.Reporting;
using AppliedResearchAssociates.iAM.Reporting.Models.BAMSSummaryReport;
using OfficeOpenXml;

namespace AppliedResearchAssociates.iAM.Reporting.Interfaces.BAMSSummaryReport
{
    public interface IBridgeDataForSummaryReport
    {
        WorkSummaryModel Fill(ExcelWorksheet worksheet, SimulationOutput reportOutputData
            , Dictionary<string, string> treatmentCategoryLookup, bool allowFundingFromMultipleBudgets);
    }
}
