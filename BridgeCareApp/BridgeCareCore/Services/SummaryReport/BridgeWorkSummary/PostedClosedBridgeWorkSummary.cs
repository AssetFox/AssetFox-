using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using BridgeCareCore.Interfaces.SummaryReport;
using BridgeCareCore.Models.SummaryReport;
using BridgeCareCore.Services.SummaryReport.BridgeWorkSummary.StaticContent;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace BridgeCareCore.Services.SummaryReport.BridgeWorkSummary
{
    public class PostedClosedBridgeWorkSummary
    {
        private readonly BridgeWorkSummaryCommon _bridgeWorkSummaryCommon;
        private readonly BridgeWorkSummaryComputationHelper _bridgeWorkSummaryComputationHelper;
        private readonly WorkSummaryModel _workSummaryModel;

        public PostedClosedBridgeWorkSummary(BridgeWorkSummaryCommon bridgeWorkSummaryCommon,
            BridgeWorkSummaryComputationHelper bridgeWorkSummaryComputationHelper, WorkSummaryModel workSummaryModel)
        {
            _bridgeWorkSummaryCommon = bridgeWorkSummaryCommon ?? throw new ArgumentNullException(nameof(bridgeWorkSummaryCommon));
            _bridgeWorkSummaryComputationHelper = bridgeWorkSummaryComputationHelper ?? throw new ArgumentNullException(nameof(bridgeWorkSummaryComputationHelper));
            _workSummaryModel = workSummaryModel ?? throw new ArgumentNullException(nameof(workSummaryModel));
        }

        internal ChartRowsModel FillPostedBridgesCountByBPN(ExcelWorksheet worksheet, CurrentCell currentCell,
            List<int> simulationYears, SimulationOutput reportOutputData, ChartRowsModel chartRowsModel)
        {
            _bridgeWorkSummaryCommon.AddBridgeHeaders(worksheet, currentCell, simulationYears, "Posted Bridges - Count", true);
            chartRowsModel.TotalBridgePostedCountByBPNYearsRow = currentCell.Row;
            AddDetailsForPostedBridgesCountByBPN(worksheet, currentCell, reportOutputData);
            return chartRowsModel;
        }

        private void AddDetailsForPostedBridgesCountByBPN(ExcelWorksheet worksheet, CurrentCell currentCell,
            SimulationOutput reportOutputData)
        {
            int startRow, startColumn, row, column;
            _bridgeWorkSummaryCommon.InitializeBPNLabels(worksheet, currentCell, out startRow, out startColumn, out row, out column);
            AddInitialPostedBridgesCountBPN(worksheet, startRow, column, reportOutputData.InitialSectionSummaries);
            foreach (var yearlyData in reportOutputData.Years)
            {
                row = startRow;
                column = ++column;
                AddPostedBridgesCountBPN(worksheet, row, column, yearlyData.Sections);
            }
            var bpnNames = EnumExtensions.GetValues<BPNName>();
            ExcelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row + bpnNames.Count - 1, column]);
            _bridgeWorkSummaryCommon.UpdateCurrentCell(currentCell, row + bpnNames.Count, column);
        }


        private void AddInitialPostedBridgesCountBPN(ExcelWorksheet worksheet, int row, int column, List<SectionSummaryDetail> initialSectionSummaries)
        {
            var bpnNames = EnumExtensions.GetValues<BPNName>();
            for (var bpnName = bpnNames[0]; bpnName <= bpnNames.Last(); bpnName++)
            {
                var bpnKey = bpnName.ToMatchInDictionary();
                var postedBPNCount = _bridgeWorkSummaryComputationHelper.CalculatePostedCountOrDeckAreaForBPN(initialSectionSummaries, bpnKey, true);
                worksheet.Cells[row++, column].Value = postedBPNCount;
            }
        }

        private void AddPostedBridgesCountBPN(ExcelWorksheet worksheet, int row, int column, List<SectionDetail> sectionDetails)
        {
            var bpnNames = EnumExtensions.GetValues<BPNName>();
            for (var bpnName = bpnNames[0]; bpnName <= bpnNames.Last(); bpnName++)
            {
                var bpnKey = bpnName.ToMatchInDictionary();
                var postedBPNCount = _bridgeWorkSummaryComputationHelper.CalculatePostedCountOrDeckAreaForBPN(sectionDetails, bpnKey, true);
                worksheet.Cells[row++, column].Value = postedBPNCount;
            }
        }


        internal ChartRowsModel FillPostedBridgesDeckAreaByBPN(ExcelWorksheet worksheet, CurrentCell currentCell,
            List<int> simulationYears, SimulationOutput reportOutputData, ChartRowsModel chartRowsModel)
        {
            _bridgeWorkSummaryCommon.AddBridgeHeaders(worksheet, currentCell, simulationYears, "Posted Bridges - Deck Area", true);
            chartRowsModel.TotalPostedBridgeDeckAreaByBPNYearsRow = currentCell.Row;
            AddDetailsForPostedBridgesDeckAreaByBPN(worksheet, currentCell, reportOutputData);
            return chartRowsModel;
        }

        private void AddDetailsForPostedBridgesDeckAreaByBPN(ExcelWorksheet worksheet, CurrentCell currentCell,
            SimulationOutput reportOutputData)
        {
            int startRow, startColumn, row, column;
            _bridgeWorkSummaryCommon.InitializeBPNLabels(worksheet, currentCell, out startRow, out startColumn, out row, out column);
            AddInitialPostedBridgesDeckArea(worksheet, startRow, column, reportOutputData.InitialSectionSummaries);
            foreach (var yearlyData in reportOutputData.Years)
            {
                row = startRow;
                column = ++column;
                AddPostedBridgesDeckArea(worksheet, row, column, yearlyData.Sections);
            }
            var bpnNames = EnumExtensions.GetValues<BPNName>();
            ExcelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row + bpnNames.Count - 1, column]);
            _bridgeWorkSummaryCommon.UpdateCurrentCell(currentCell, row + bpnNames.Count, column);
        }

        private void AddInitialPostedBridgesDeckArea(ExcelWorksheet worksheet, int row, int column, List<SectionSummaryDetail> initialSectionSummaries)
        {
            var bpnNames = EnumExtensions.GetValues<BPNName>();
            for (var bpnName = bpnNames[0]; bpnName <= bpnNames.Last(); bpnName++)
            {
                var bpnKey = bpnName.ToMatchInDictionary();
                var postedDeckArea = _bridgeWorkSummaryComputationHelper.CalculatePostedCountOrDeckAreaForBPN(initialSectionSummaries, bpnKey, false);
                worksheet.Cells[row++, column].Value = postedDeckArea;
            }
        }

        private void AddPostedBridgesDeckArea(ExcelWorksheet worksheet, int row, int column, List<SectionDetail> sectionDetails)
        {
            var bpnNames = EnumExtensions.GetValues<BPNName>();
            for (var bpnName = bpnNames[0]; bpnName <= bpnNames.Last(); bpnName++)
            {
                var bpnKey = bpnName.ToMatchInDictionary();
                var postedDeckArea = _bridgeWorkSummaryComputationHelper.CalculatePostedCountOrDeckAreaForBPN(sectionDetails, bpnKey, false);
                worksheet.Cells[row++, column].Value = postedDeckArea;
            }
        }



        internal ChartRowsModel FillClosedBridgesCountByBPN(ExcelWorksheet worksheet, CurrentCell currentCell,
            List<int> simulationYears, SimulationOutput reportOutputData, ChartRowsModel chartRowsModel)
        {
            _bridgeWorkSummaryCommon.AddBridgeHeaders(worksheet, currentCell, simulationYears, "Closed Bridges - Count", true);
            chartRowsModel.TotalClosedBridgeCountByBPNYearsRow = currentCell.Row;
            AddDetailsForClosedBridgesCountByBPN(worksheet, currentCell, reportOutputData);
            return chartRowsModel;
        }

        private void AddDetailsForClosedBridgesCountByBPN(ExcelWorksheet worksheet, CurrentCell currentCell,
            SimulationOutput reportOutputData)
        {
            int startRow, startColumn, row, column;
            _bridgeWorkSummaryCommon.InitializeBPNLabels(worksheet, currentCell, out startRow, out startColumn, out row, out column);
            AddInitialClosedBridgesCountBPN(worksheet, startRow, column, reportOutputData.InitialSectionSummaries);
            foreach (var yearlyData in reportOutputData.Years)
            {
                row = startRow;
                column = ++column;
                AddClosedBridgesCountBPN(worksheet, row, column, yearlyData.Sections);
            }
            var bpnNames = EnumExtensions.GetValues<BPNName>();
            ExcelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row + bpnNames.Count - 1, column]);
            _bridgeWorkSummaryCommon.UpdateCurrentCell(currentCell, row + bpnNames.Count, column);
        }


        private void AddInitialClosedBridgesCountBPN(ExcelWorksheet worksheet, int row, int column, List<SectionSummaryDetail> initialSectionSummaries)
        {
            var bpnNames = EnumExtensions.GetValues<BPNName>();
            for (var bpnName = bpnNames[0]; bpnName <= bpnNames.Last(); bpnName++)
            {
                var bpnKey = bpnName.ToMatchInDictionary();
                var closedDecArea = _bridgeWorkSummaryComputationHelper.CalculateClosedCountOrDeckAreaForBPN(initialSectionSummaries, bpnKey, true);
                worksheet.Cells[row++, column].Value = closedDecArea;
            }
        }

        private void AddClosedBridgesCountBPN(ExcelWorksheet worksheet, int row, int column, List<SectionDetail> sectionDetails)
        {
            var bpnNames = EnumExtensions.GetValues<BPNName>();
            for (var bpnName = bpnNames[0]; bpnName <= bpnNames.Last(); bpnName++)
            {
                var bpnKey = bpnName.ToMatchInDictionary();
                var closedDeckArea = _bridgeWorkSummaryComputationHelper.CalculateClosedCountOrDeckAreaForBPN(sectionDetails, bpnKey, true);
                worksheet.Cells[row++, column].Value = closedDeckArea;
            }
        }


        internal ChartRowsModel FillClosedBridgesDeckAreaByBPN(ExcelWorksheet worksheet, CurrentCell currentCell,
            List<int> simulationYears, SimulationOutput reportOutputData, ChartRowsModel chartRowsModel)
        {
            _bridgeWorkSummaryCommon.AddBridgeHeaders(worksheet, currentCell, simulationYears, "Closed Bridges - Deck Area", true);
            chartRowsModel.TotalClosedBridgeDeckAreaByBPNYearsRow = currentCell.Row;
            AddDetailsForClosedBridgesDeckAreaByBPN(worksheet, currentCell, reportOutputData);
            return chartRowsModel;
        }

        private void AddDetailsForClosedBridgesDeckAreaByBPN(ExcelWorksheet worksheet, CurrentCell currentCell,
            SimulationOutput reportOutputData)
        {
            int startRow, startColumn, row, column;
            _bridgeWorkSummaryCommon.InitializeBPNLabels(worksheet, currentCell, out startRow, out startColumn, out row, out column);
            AddInitialClosedBridgesDeckArea(worksheet, startRow, column, reportOutputData.InitialSectionSummaries);
            foreach (var yearlyData in reportOutputData.Years)
            {
                row = startRow;
                column = ++column;
                AddClosedBridgesDeckArea(worksheet, row, column, yearlyData.Sections);
            }
            var bpnNames = EnumExtensions.GetValues<BPNName>();
            ExcelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row + bpnNames.Count - 1, column]);
            _bridgeWorkSummaryCommon.UpdateCurrentCell(currentCell, row + bpnNames.Count, column);
        }

        private void AddInitialClosedBridgesDeckArea(ExcelWorksheet worksheet, int row, int column, List<SectionSummaryDetail> initialSectionSummaries)
        {
            var bpnNames = EnumExtensions.GetValues<BPNName>();
            for (var bpnName = bpnNames[0]; bpnName <= bpnNames.Last(); bpnName++)
            {
                var bpnKey = bpnName.ToMatchInDictionary();
                var closedDeckArea = _bridgeWorkSummaryComputationHelper.CalculateClosedCountOrDeckAreaForBPN(initialSectionSummaries, bpnKey, false);
                worksheet.Cells[row++, column].Value = closedDeckArea;
            }
        }

        private void AddClosedBridgesDeckArea(ExcelWorksheet worksheet, int row, int column, List<SectionDetail> sectionDetails)
        {
            var bpnNames = EnumExtensions.GetValues<BPNName>();
            for (var bpnName = bpnNames[0]; bpnName <= bpnNames.Last(); bpnName++)
            {
                var bpnKey = bpnName.ToMatchInDictionary();
                var closedDeckArea = _bridgeWorkSummaryComputationHelper.CalculateClosedCountOrDeckAreaForBPN(sectionDetails, bpnKey, false);
                worksheet.Cells[row++, column].Value = closedDeckArea;
            }
        }

        internal ChartRowsModel FillPostedAndClosedBridgesTotalCount(ExcelWorksheet worksheet, CurrentCell currentCell,
            List<int> simulationYears, SimulationOutput reportOutputData, ChartRowsModel chartRowsModel)
        {
            _bridgeWorkSummaryCommon.AddBridgeHeaders(worksheet, currentCell, simulationYears, "Posted Bridges - Count", true);
            chartRowsModel.TotalPostedAndClosedByBPNYearsRow = currentCell.Row;
            AddDetailsForPostedAndClosedBridgesTotalCount(worksheet, currentCell, reportOutputData);
            return chartRowsModel;
        }

        private void AddDetailsForPostedAndClosedBridgesTotalCount(ExcelWorksheet worksheet, CurrentCell currentCell,
            SimulationOutput reportOutputData)
        {
            int startRow, startColumn, row, column;

            _bridgeWorkSummaryCommon.SetRowColumns(currentCell, out startRow, out startColumn, out row, out column);
            worksheet.Cells[row++, column].Value = "Posted";
            worksheet.Cells[row, column++].Value = "Closed";

            row = startRow;
            worksheet.Cells[row++, column].Value = _bridgeWorkSummaryComputationHelper.CalculatePostedCount(reportOutputData.InitialSectionSummaries);
            worksheet.Cells[row, column].Value = _bridgeWorkSummaryComputationHelper.CalculateClosedCount(reportOutputData.InitialSectionSummaries);
            foreach (var yearlyData in reportOutputData.Years)
            {
                row = startRow;
                column = ++column;
                worksheet.Cells[row++, column].Value = _bridgeWorkSummaryComputationHelper.CalculatePostedCount(yearlyData.Sections);
                worksheet.Cells[row++, column].Value = _bridgeWorkSummaryComputationHelper.CalculateClosedCount(yearlyData.Sections);
            }

            worksheet.Cells[startRow, startColumn, row + 1, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ExcelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row + 1, column]);
            _bridgeWorkSummaryCommon.UpdateCurrentCell(currentCell, row - 1, column);
        }


        internal ChartRowsModel FillMoneyNeededByBPN(ExcelWorksheet worksheet, CurrentCell currentCell,
            List<int> simulationYears, SimulationOutput reportOutputData, ChartRowsModel chartRowsModel)
        {
            _bridgeWorkSummaryCommon.AddBridgeHeaders(worksheet, currentCell, simulationYears, "Dollar Needs By BPN", false);
            chartRowsModel.TotalCashNeededByBPNYearsRow = currentCell.Row;
            AddDetailsForMoneyNeededByBPN(worksheet, currentCell, reportOutputData);
            return chartRowsModel;
        }

        private void AddDetailsForMoneyNeededByBPN(ExcelWorksheet worksheet, CurrentCell currentCell,
            SimulationOutput reportOutputData)
        {
            int startRow, startColumn, row, column;
            _bridgeWorkSummaryCommon.InitializeBPNLabels(worksheet, currentCell, out startRow, out startColumn, out row, out column);

            var bpnNames = EnumExtensions.GetValues<BPNName>();
            var bpnRowCount = bpnNames.Count;

            worksheet.Cells[startRow + bpnRowCount, column - 1].Value = "Annualized Amount";
            worksheet.Cells[startRow + bpnRowCount, column - 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            var totalMoney = 0.0;
            foreach (var yearlyData in reportOutputData.Years)
            {
                row = startRow;
                column = ++column;
                var totalMoneyPerYear = AddMoneyNeededByBPN(worksheet, row, column, yearlyData.Sections);
                totalMoney += totalMoneyPerYear;
            }
            for (var i = 0; i < reportOutputData.Years.Count; i++)
            {
                worksheet.Cells[row + bpnRowCount, startColumn + i + 2].Value = _workSummaryModel.AnnualizedAmount;
            }
            ExcelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row + bpnRowCount, column]);
            ExcelHelper.SetCustomFormat(worksheet.Cells[startRow, startColumn, row + bpnRowCount, column], ExcelHelperCellFormat.NegativeCurrency);
            ExcelHelper.ApplyColor(worksheet.Cells[startRow, startColumn + 2, row + bpnRowCount, column], Color.FromArgb(198, 224, 180));
            _bridgeWorkSummaryCommon.UpdateCurrentCell(currentCell, row + bpnRowCount, column);
        }

        private double AddMoneyNeededByBPN(ExcelWorksheet worksheet, int row, int column, List<SectionDetail> sectionDetails)
        {
            var totalMoney = 0.0;

            var bpnNames = EnumExtensions.GetValues<BPNName>();
            for (var bpnName = bpnNames[0]; bpnName <= bpnNames.Last(); bpnName++)
            {
                var bpnKey = bpnName.ToMatchInDictionary();
                var moneyForBPN = _bridgeWorkSummaryComputationHelper.CalculateMoneyNeededByBPN(sectionDetails, bpnKey);
                worksheet.Cells[row++, column].Value = moneyForBPN;
                totalMoney += moneyForBPN;
            }

            return totalMoney;
        }
    }
}
