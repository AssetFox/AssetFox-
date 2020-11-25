using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Analysis;
using BridgeCareCore.Interfaces.SummaryReport;
using BridgeCareCore.Services.SummaryReport.BridgeWorkSummary;
using OfficeOpenXml;

namespace BridgeCareCore.Services.SummaryReport.BridgeWorkSummaryByBudget
{
    public class BridgeWorkSummaryByBudget : IBridgeWorkSummaryByBudget
    {
        private readonly IExcelHelper _excelHelper;
        private readonly BridgeWorkSummaryCommon _bridgeWorkSummaryCommon;

        public BridgeWorkSummaryByBudget(IExcelHelper excelHelper, BridgeWorkSummaryCommon bridgeWorkSummaryCommon)
        {
            _excelHelper = excelHelper ?? throw new ArgumentNullException(nameof(excelHelper));
            _bridgeWorkSummaryCommon = bridgeWorkSummaryCommon ?? throw new ArgumentNullException(nameof(bridgeWorkSummaryCommon));
        }
        public void Fill(ExcelWorksheet summaryByBudgetWorksheet, SimulationOutput reportOutputData, List<int> simulationYears)
        {

        }
    }
}
