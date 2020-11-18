using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis;
using BridgeCareCore.Interfaces.SummaryReport;
using BridgeCareCore.Models.SummaryReport;
using OfficeOpenXml;

namespace BridgeCareCore.Services.SummaryReport.BridgeWorkSummary
{
    public class BridgeWorkSummary : IBridgeWorkSummary
    {
        private readonly CostBudgetsWorkSummary costBudgetsWorkSummary;

        public BridgeWorkSummary(CostBudgetsWorkSummary costBudgetsWorkSummary)
        {
            this.costBudgetsWorkSummary = costBudgetsWorkSummary ?? throw new ArgumentNullException(nameof(costBudgetsWorkSummary));
        }
        public ChartRowsModel Fill(ExcelWorksheet worksheet, SimulationOutput reportOutputData, List<int> simulationYears)
        {
            var currentCell = new CurrentCell { Row = 1, Column = 1 };

            // Getting list of treatments. It will be used in several places throughout this excel TAB
            var treatments = new List<string>();
            var singleSection = reportOutputData.Years[0].Sections[0];
            treatments = singleSection.TreatmentOptions.Select(_ => _.TreatmentName)
                .Union(singleSection.TreatmentRejections.Select(r => r.TreatmentName)).ToList();

            costBudgetsWorkSummary.FillCostBudgetWorkSummarySections(worksheet, currentCell, reportOutputData, simulationYears, treatments);

            return new ChartRowsModel();
        }
    }
}
