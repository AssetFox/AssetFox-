using AppliedResearchAssociates.iAM.Analysis;
using BridgeCareCore.Models.SummaryReport;
using OfficeOpenXml;

namespace BridgeCareCore.Interfaces.SummaryReport
{
    public interface IBridgeDataForSummaryReport
    {
        WorkSummaryModel Fill(ExcelWorksheet worksheet, SimulationOutput reportOutputData);
    }
}
