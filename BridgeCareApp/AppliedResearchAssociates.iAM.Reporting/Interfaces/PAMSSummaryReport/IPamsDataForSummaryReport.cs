using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.Reporting;
using AppliedResearchAssociates.iAM.Reporting.Models.PAMSSummaryReport;
using OfficeOpenXml;

namespace AppliedResearchAssociates.iAM.Reporting.Interfaces.PAMSSummaryReport
{
    public interface IPamsDataForSummaryReport
    {
        WorkSummaryModel Fill(ExcelWorksheet worksheet, SimulationOutput reportOutputData);
    }
}
