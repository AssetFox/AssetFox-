using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.Analysis;
using BridgeCareCore.Interfaces.SummaryReport;
using BridgeCareCore.Models.SummaryReport;
using BridgeCareCore.Services.SummaryReport.BridgeWorkSummary.StaticContent;
using OfficeOpenXml;

namespace BridgeCareCore.Services.SummaryReport.BridgeWorkSummary
{
    public class DeckAreaBridgeWorkSummary
    {
        private readonly BridgeWorkSummaryCommon _bridgeWorkSummaryCommon;
        private readonly IExcelHelper _excelHelper;
        private readonly BridgeWorkSummaryComputationHelper _bridgeWorkSummaryComputationHelper;

        public DeckAreaBridgeWorkSummary(BridgeWorkSummaryCommon bridgeWorkSummaryCommon, IExcelHelper excelHelper, BridgeWorkSummaryComputationHelper bridgeWorkSummaryComputationHelper)
        {
            _bridgeWorkSummaryCommon = bridgeWorkSummaryCommon ?? throw new ArgumentNullException(nameof(bridgeWorkSummaryCommon));
            _excelHelper = excelHelper ?? throw new ArgumentNullException(nameof(excelHelper));
            _bridgeWorkSummaryComputationHelper = bridgeWorkSummaryComputationHelper ?? throw new ArgumentNullException(nameof(bridgeWorkSummaryComputationHelper));
        }

        internal ChartRowsModel FillPoorDeckArea(ExcelWorksheet worksheet, CurrentCell currentCell,
            List<int> simulationYears, SimulationOutput reportOutputData, ChartRowsModel chartRowsModel)
        {
            _bridgeWorkSummaryCommon.AddBridgeHeaders(worksheet, currentCell, simulationYears, "Poor Deck Area", true);
            chartRowsModel.TotalPoorDeckAreaByBPNSectionYearsRow = currentCell.Row;
            AddDetailsForPoorDeckArea(worksheet, currentCell, reportOutputData);
            return chartRowsModel;
        }

        #region

        private void AddDetailsForPoorDeckArea(ExcelWorksheet worksheet, CurrentCell currentCell,
            SimulationOutput reportOutputData)
        {
            int startRow, startColumn, row, column;
            _bridgeWorkSummaryCommon.InitializeBPNLabels(worksheet, currentCell, out startRow, out startColumn, out row, out column);
            AddInitialPoorDeckArea(worksheet, startRow, column, reportOutputData.InitialSectionSummaries);
            foreach (var yearlyData in reportOutputData.Years)
            {
                row = startRow;
                column = ++column;
                AddPoorDeckArea(worksheet, row, column, yearlyData.Sections);
            }
            var bpnNames = EnumExtensions.GetValues<BPNName>();
            _excelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row + bpnNames.Count - 1, column]);
            _bridgeWorkSummaryCommon.UpdateCurrentCell(currentCell, row + bpnNames.Count, column);
        }

        private void AddInitialPoorDeckArea(ExcelWorksheet worksheet, int row, int column, List<SectionSummaryDetail> initialSectionSummaries)
        {
            var bpnNames = EnumExtensions.GetValues<BPNName>();
            for (var bpnName = bpnNames[0]; bpnName <= bpnNames.Last(); bpnName++)
            {
                var bpnKey = bpnName.ToMatchInDictionary();
                var poorDeckArea = _bridgeWorkSummaryComputationHelper.CalculatePoorCountOrAreaForBPN(initialSectionSummaries, bpnKey, false);
                worksheet.Cells[row++, column].Value = poorDeckArea;
            }
        }

        private void AddPoorDeckArea(ExcelWorksheet worksheet, int row, int column, List<SectionDetail> sectionDetails)
        {
            var bpnNames = EnumExtensions.GetValues<BPNName>();
            for (var bpnName = bpnNames[0]; bpnName <= bpnNames.Last(); bpnName++)
            {
                var bpnKey = bpnName.ToMatchInDictionary();
                var poorDeckArea = _bridgeWorkSummaryComputationHelper.CalculatePoorCountOrAreaForBPN(sectionDetails, bpnKey, false);
                worksheet.Cells[row++, column].Value = poorDeckArea;
            }
        }

        #endregion
    }
}
