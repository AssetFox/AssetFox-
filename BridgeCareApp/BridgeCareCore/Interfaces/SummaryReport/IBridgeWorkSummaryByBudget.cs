using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.Analysis;
using OfficeOpenXml;

namespace BridgeCareCore.Interfaces.SummaryReport
{
    public interface IBridgeWorkSummaryByBudget
    {
        public void Fill(ExcelWorksheet summaryByBudgetWorksheet, SimulationOutput reportOutputData,
            List<int> simulationYears, Dictionary<string, Budget> yearlyBudgetAmount,
            IReadOnlyCollection<SelectableTreatment> selectableTreatments);
    }
}
