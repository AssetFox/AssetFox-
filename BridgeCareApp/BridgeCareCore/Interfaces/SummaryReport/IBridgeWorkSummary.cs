using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Analysis;
using BridgeCareCore.Models.SummaryReport;
using OfficeOpenXml;

namespace BridgeCareCore.Interfaces.SummaryReport
{
    public interface IBridgeWorkSummary
    {
        public ChartRowsModel Fill(ExcelWorksheet worksheet, SimulationOutput reportOutputData, List<int> simulationYears);
    }
}
