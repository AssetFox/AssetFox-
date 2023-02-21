using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Analysis.Engine;

using OfficeOpenXml;
using OfficeOpenXml.Style;

using AppliedResearchAssociates.iAM.Reporting.Interfaces.BAMSSummaryReport;
using AppliedResearchAssociates.iAM.Reporting.Models.BAMSSummaryReport;
using AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport.BridgeWorkSummary.StaticContent;

using System.Linq;
using AppliedResearchAssociates.iAM.ExcelHelpers;
using AppliedResearchAssociates.iAM.Reporting.Models;

namespace AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport.BridgeWorkSummary
{
    public class BridgeRateDeckAreaWorkSummary
    {
        private BridgeWorkSummaryCommon _bridgeWorkSummaryCommon;
        private BridgeWorkSummaryComputationHelper _bridgeWorkSummaryComputationHelper;

        public BridgeRateDeckAreaWorkSummary()
        {
            _bridgeWorkSummaryCommon = new BridgeWorkSummaryCommon();
            _bridgeWorkSummaryComputationHelper = new BridgeWorkSummaryComputationHelper();
        }

        public ChartRowsModel FillBridgeRateDeckAreaWorkSummarySections(ExcelWorksheet worksheet, CurrentCell currentCell,
            List<int> simulationYears, WorkSummaryModel workSummaryModel, SimulationOutput reportOutputData)
        {
            var chartRowsModel = new ChartRowsModel(); // Remember final rows of each summary section; these will be used for chart graph series

            FillPoorBridgeOnOffRateSection(worksheet, currentCell, simulationYears, workSummaryModel);

            chartRowsModel.TotalPoorBridgesCountSectionYearsRow = FillTotalPoorBridgesCountSection(worksheet, currentCell,
                simulationYears, reportOutputData);
            chartRowsModel.TotalPoorBridgesDeckAreaSectionYearsRow = FillTotalPoorBridgesDeckAreaSection(worksheet, currentCell,
                simulationYears, reportOutputData);
            chartRowsModel.TotalBridgeCountSectionYearsRow = FillTotalBridgeCountSection(worksheet, currentCell, simulationYears,
                reportOutputData);
            chartRowsModel.TotalBridgeCountPercentYearsRow = FillTotalBridgeCountPercent(worksheet, currentCell, simulationYears,
                chartRowsModel.TotalBridgeCountSectionYearsRow + 1);
            chartRowsModel.TotalDeckAreaSectionYearsRow = FillTotalDeckAreaSection(worksheet, currentCell, simulationYears,
                reportOutputData);
            chartRowsModel.TotalDeckAreaPercentYearsRow = FillTotalDeckAreaPercent(worksheet, currentCell, simulationYears,
                chartRowsModel.TotalDeckAreaSectionYearsRow + 1);

            return chartRowsModel;
        }

        #region Private methods

        private void FillPoorBridgeOnOffRateSection(ExcelWorksheet worksheet, CurrentCell currentCell,
            List<int> simulationYears, WorkSummaryModel workSummaryModel)
        {
            currentCell.Row = currentCell.Row + 2;
            _bridgeWorkSummaryCommon.AddBridgeHeaders(worksheet, currentCell, simulationYears, "Poor Bridge On and Off Rate", false);
            AddDetailsForPoorBridgeOnOffRate(worksheet, currentCell, workSummaryModel);
        }

        private int FillTotalPoorBridgesCountSection(ExcelWorksheet worksheet, CurrentCell currentCell,
            List<int> simulationYears, SimulationOutput reportOutputData)
        {
            _bridgeWorkSummaryCommon.AddBridgeHeaders(worksheet, currentCell, simulationYears, "Total Poor Bridges Count", true);
            var totalPoorBridgesCountSectionYearsRow = currentCell.Row;
            AddDetailsForTotalPoorBridgesCount(worksheet, currentCell, reportOutputData);
            return totalPoorBridgesCountSectionYearsRow;
        }

        private int FillTotalPoorBridgesDeckAreaSection(ExcelWorksheet worksheet, CurrentCell currentCell,
            List<int> simulationYears, SimulationOutput reportOutputData)
        {
            _bridgeWorkSummaryCommon.AddBridgeHeaders(worksheet, currentCell, simulationYears, "Total Poor Bridges Deck Area", true);
            var totalPoorBridgesDeckAreaSectionYearsRow = currentCell.Row;
            AddDetailsForTotalPoorBridgesDeckArea(worksheet, currentCell, reportOutputData);
            return totalPoorBridgesDeckAreaSectionYearsRow;
        }

        private int FillTotalBridgeCountSection(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears,
            SimulationOutput reportOutputData)
        {
            _bridgeWorkSummaryCommon.AddBridgeHeaders(worksheet, currentCell, simulationYears, "Total Bridge Count", true);
            var totalBridgeCountSectionYearsRow = currentCell.Row;
            AddDetailsForTotalBridgeCount(worksheet, currentCell, reportOutputData);
            return totalBridgeCountSectionYearsRow;
        }

        private int FillTotalBridgeCountPercent(ExcelWorksheet worksheet, CurrentCell currentCell,
            List<int> simulationYears, int dataStartRow)
        {
            _bridgeWorkSummaryCommon.AddBridgeHeaders(worksheet, currentCell, simulationYears, "Total Bridge Count Percentage", true);
            var totalBridgeCountPercentYearsRow = currentCell.Row;
            AddDetailsForTotalBridgeAndDeckPercent(worksheet, currentCell, simulationYears, dataStartRow);
            return totalBridgeCountPercentYearsRow;
        }

        private int FillTotalDeckAreaSection(ExcelWorksheet worksheet, CurrentCell currentCell,
            List<int> simulationYears, SimulationOutput reportOutputData)
        {
            _bridgeWorkSummaryCommon.AddBridgeHeaders(worksheet, currentCell, simulationYears, "Total Deck Area", true);
            var totalDeckAreaSectionYearsRow = currentCell.Row;
            AddDetailsForTotalDeckArea(worksheet, currentCell, reportOutputData);
            return totalDeckAreaSectionYearsRow;
        }

        private int FillTotalDeckAreaPercent(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, int dataStartRow)
        {
            _bridgeWorkSummaryCommon.AddBridgeHeaders(worksheet, currentCell, simulationYears, "Total Deck Area Percentage", true);
            var totalDeckAreaPercentYearsRow = currentCell.Row;
            AddDetailsForTotalBridgeAndDeckPercent(worksheet, currentCell, simulationYears, dataStartRow);
            return totalDeckAreaPercentYearsRow;
        }

        private void AddDetailsForPoorBridgeOnOffRate(ExcelWorksheet worksheet, CurrentCell currentCell, WorkSummaryModel workSummaryModel)
        {
            var poorOnOffPerYear = workSummaryModel.PoorOnOffCount;
            var bpnInfoPerYear = workSummaryModel.BpnPoorOnPerYear;

            int startRow, startColumn, row, column;
            _bridgeWorkSummaryCommon.SetRowColumns(currentCell, out startRow, out startColumn, out row, out column);

            // Add Bridge All On row label
            worksheet.Cells[row, column].Value = "# Bridge On";
            worksheet.Cells[row, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            var bridgeAllOnRow = row;
            row++;

            // Add NHS/Non NHS row labels
            worksheet.Cells[row++, column].Value = "# Bridge On - NHS";
            worksheet.Cells[row - 1, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

            worksheet.Cells[row++, column].Value = "# Bridge On - Non NHS";
            worksheet.Cells[row - 1, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

            // Add row labels per BPN
            var bpnNames = EnumExtensions.GetValues<BPNName>();
            for (var bpnName = bpnNames[0]; bpnName <= bpnNames.Last(); bpnName++)
            {
                worksheet.Cells[row++, column].Value = bpnName.ToReportLabel("# Bridge On - ");
                worksheet.Cells[row - 1, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            }

            // Add Bridge All Off row label
            worksheet.Cells[row, column].Value = "# Bridge Off";
            worksheet.Cells[row, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

            // Render data column-by-column (year-by-year)
            column++;
            foreach (var yearlyData in poorOnOffPerYear)
            {
                // All On
                var year = yearlyData.Key; // assumes years are the same in yearlyData & bpnInfoPerYear
                row = startRow;
                column = ++column;
                worksheet.Cells[row++, column].Value = yearlyData.Value.on;

                // NHS/Non NHS On
                worksheet.Cells[row++, column].Value = workSummaryModel.NhsPoorOnPerYear[year];
                worksheet.Cells[row++, column].Value = workSummaryModel.NonNhsPoorOnPerYear[year];

                // BPN On
                var bpnInfo = bpnInfoPerYear[year];
                for (var bpnName = bpnNames[0]; bpnName <= bpnNames.Last(); bpnName++)
                {
                    int poorBridgeOnCount = 0;
                    var bpnKey = bpnName.ToMatchInDictionary();
                    if (bpnInfo.ContainsKey(bpnKey))
                    {
                        poorBridgeOnCount = bpnInfo[bpnKey];
                    }
                    worksheet.Cells[row++, column].Value = poorBridgeOnCount;
                }

                // All Off
                worksheet.Cells[row, column].Value = yearlyData.Value.off;
            }
            ExcelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row, column]);
            _bridgeWorkSummaryCommon.UpdateCurrentCell(currentCell, ++row, column);
        }

        private void AddDetailsForTotalPoorBridgesCount(ExcelWorksheet worksheet, CurrentCell currentCell,
             SimulationOutput reportOutputData)
        {
            int startRow, startColumn, row, column;
            _bridgeWorkSummaryCommon.SetRowColumns(currentCell, out startRow, out startColumn, out row, out column);

            // Add Bridge All On row label
            worksheet.Cells[row, column].Value = "Overall";
            worksheet.Cells[row, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            var bridgeAllOnRow = row;
            row++;

            // Add NHS/Non NHS row labels
            worksheet.Cells[row++, column].Value = "NHS";
            worksheet.Cells[row - 1, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

            worksheet.Cells[row++, column].Value = "Non NHS";
            worksheet.Cells[row - 1, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

            // Add row labels per BPN
            var bpnNames = EnumExtensions.GetValues<BPNName>();
            for (var bpnName = bpnNames[0]; bpnName <= bpnNames.Last(); bpnName++)
            {
                worksheet.Cells[row++, column].Value = bpnName.ToReportLabel();
                worksheet.Cells[row - 1, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            }

            column++;
            row = startRow;
            worksheet.Cells[row++, column].Value = _bridgeWorkSummaryComputationHelper.TotalInitialPoorBridgesCount(reportOutputData);

            var poorBridgesNHSCountInitial = _bridgeWorkSummaryComputationHelper.InitialNHSBridgePoorCountOrArea(reportOutputData.InitialAssetSummaries, true);
            worksheet.Cells[row++, column].Value = poorBridgesNHSCountInitial;
            worksheet.Cells[row++, column].Value = _bridgeWorkSummaryComputationHelper.TotalInitialPoorBridgesCount(reportOutputData) - poorBridgesNHSCountInitial;

            for (var bpnName = bpnNames[0]; bpnName <= bpnNames.Last(); bpnName++)
            {
                worksheet.Cells[row++, column].Value = _bridgeWorkSummaryComputationHelper.CalculatePoorCountOrAreaForBPN(reportOutputData.InitialAssetSummaries, bpnName.ToMatchInDictionary(), true);
            }

            foreach (var yearlyData in reportOutputData.Years)
            {
                column++;
                row = startRow;

                var totalPoorBridgesCount = _bridgeWorkSummaryComputationHelper.TotalSectionalPoorBridgesCount(yearlyData);
                worksheet.Cells[row++, column].Value = totalPoorBridgesCount;

                var poorBridgesNHSCount = _bridgeWorkSummaryComputationHelper.SectionalNHSBridgePoorCountOrArea(yearlyData.Assets, true);
                worksheet.Cells[row++, column].Value = poorBridgesNHSCount;
                worksheet.Cells[row++, column].Value = totalPoorBridgesCount - poorBridgesNHSCount;

                for (var bpnName = bpnNames[0]; bpnName <= bpnNames.Last(); bpnName++)
                {
                    worksheet.Cells[row++, column].Value = _bridgeWorkSummaryComputationHelper.CalculatePoorCountOrAreaForBPN(yearlyData.Assets, bpnName.ToMatchInDictionary(), true);
                }
            }
            ExcelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row, column]);
            _bridgeWorkSummaryCommon.UpdateCurrentCell(currentCell, ++row, column);
        }

        private void AddDetailsForTotalPoorBridgesDeckArea(ExcelWorksheet worksheet, CurrentCell currentCell,
            SimulationOutput reportOutputData)
        {
            int startRow, startColumn, row, column;
            _bridgeWorkSummaryCommon.SetRowColumns(currentCell, out startRow, out startColumn, out row, out column);
            worksheet.Cells[row, column++].Value = "Overall";
            worksheet.Cells[row, column - 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            worksheet.Cells[row, column].Value = _bridgeWorkSummaryComputationHelper.TotalInitialPoorBridgesDeckArea(reportOutputData);
            foreach (var yearlyData in reportOutputData.Years)
            {
                column = ++column;
                worksheet.Cells[row, column].Value = _bridgeWorkSummaryComputationHelper.CalculateTotalPoorBridgesDeckArea(yearlyData);
            }
            ExcelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row, column]);
            ExcelHelper.SetCustomFormat(worksheet.Cells[startRow, startColumn + 1, row, column], ExcelHelperCellFormat.Number);
            _bridgeWorkSummaryCommon.UpdateCurrentCell(currentCell, ++row, column);
        }

        private void AddDetailsForTotalBridgeCount(ExcelWorksheet worksheet, CurrentCell currentCell, SimulationOutput reportOutputData)
        {
            var totalCount = reportOutputData.InitialAssetSummaries.Count;
            int startRow, startColumn, row, column;
            _bridgeWorkSummaryCommon.InitializeLabelCells(worksheet, currentCell, out startRow, out startColumn, out row, out column);
            AddInitialBridgeCount(worksheet, reportOutputData, totalCount, startRow, column);
            foreach (var yearlyData in reportOutputData.Years)
            {
                row = startRow;
                column = ++column;
                AddTotalBridgeCount(worksheet, yearlyData, totalCount, row, column);
            }
            ExcelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row + 3, column]);
            _bridgeWorkSummaryCommon.UpdateCurrentCell(currentCell, row + 4, column);
        }

        private void AddInitialBridgeCount(ExcelWorksheet worksheet, SimulationOutput reportOutputData,
            int totalSimulationDataModelCount, int row, int column)
        {
            var goodCount = _bridgeWorkSummaryComputationHelper.TotalInitialBridgeGoodCount(reportOutputData);
            worksheet.Cells[row, column].Value = goodCount;

            var poorCount = _bridgeWorkSummaryComputationHelper.TotalInitialPoorBridgesCount(reportOutputData);
            worksheet.Cells[row + 2, column].Value = poorCount;

            var closedCount = _bridgeWorkSummaryComputationHelper.TotalInitialBridgeClosedCount(reportOutputData);
            worksheet.Cells[row + 3, column].Value = closedCount;

            var fairCount = totalSimulationDataModelCount - (goodCount + poorCount);
            worksheet.Cells[row + 1, column].Value = fairCount;
        }

        private void AddTotalBridgeCount(ExcelWorksheet worksheet, SimulationYearDetail yearlyData, int totalSimulationDataModelCount, int row, int column)
        {
            var goodCount = _bridgeWorkSummaryComputationHelper.CalculateTotalBridgeGoodCount(yearlyData);
            worksheet.Cells[row, column].Value = goodCount;

            var poorCount = _bridgeWorkSummaryComputationHelper.CalculateTotalBridgePoorCount(yearlyData);
            worksheet.Cells[row + 2, column].Value = poorCount;

            var closedCount = _bridgeWorkSummaryComputationHelper.CalculateTotalBridgeClosedCount(yearlyData);
            worksheet.Cells[row + 3, column].Value = closedCount;

            //worksheet.Cells[row + 1, column].Value = totalSimulationDataModelCount - (goodCount + poorCount + closedCount);

            var fairCount = totalSimulationDataModelCount - (goodCount + poorCount);
            worksheet.Cells[row + 1, column].Value = fairCount;
        }

        private void AddDetailsForTotalBridgeAndDeckPercent(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, int dataStartRow)
        {
            int startRow, startColumn, row, column;
            _bridgeWorkSummaryCommon.InitializeLabelCells(worksheet, currentCell, out startRow, out startColumn, out row, out column);
            for (var index = 0; index <= simulationYears.Count; index++)
            {
                var sumFormula = "SUM(" + worksheet.Cells[dataStartRow, column, dataStartRow + 2, column] + ")";   // Sum is Good + Fair + Poor; "Closed" is a subset of "Poor"
                worksheet.Cells[startRow, column].Formula = worksheet.Cells[dataStartRow, column] + "/" + sumFormula;
                worksheet.Cells[startRow + 1, column].Formula = worksheet.Cells[dataStartRow + 1, column] + "/" + sumFormula;
                worksheet.Cells[startRow + 2, column].Formula = worksheet.Cells[dataStartRow + 2, column] + "/" + sumFormula;
                worksheet.Cells[startRow + 3, column].Formula = worksheet.Cells[dataStartRow + 3, column] + "/" + sumFormula;
                column++;
            }
            ExcelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, startRow + 3, column - 1]);
            ExcelHelper.SetCustomFormat(worksheet.Cells[startRow, startColumn + 1, startRow + 3, column], ExcelHelperCellFormat.PercentageDecimal2);
            _bridgeWorkSummaryCommon.UpdateCurrentCell(currentCell, row, column - 1);
        }

        private void AddDetailsForTotalDeckArea(ExcelWorksheet worksheet, CurrentCell currentCell,
            SimulationOutput reportOutputData)
        {
            int startRow, startColumn, row, column;
            _bridgeWorkSummaryCommon.InitializeLabelCells(worksheet, currentCell, out startRow, out startColumn, out row, out column);
            AddTotalInitialDeckArea(worksheet, reportOutputData, startRow, column);
            foreach (var yearlyData in reportOutputData.Years)
            {
                row = startRow;
                column = ++column;
                AddTotalDeckArea(worksheet, yearlyData, row, column);
            }
            ExcelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row + 3, column]);
            ExcelHelper.SetCustomFormat(worksheet.Cells[startRow, startColumn + 1, row + 3, column], ExcelHelperCellFormat.Number);
            _bridgeWorkSummaryCommon.UpdateCurrentCell(currentCell, row + 4, column);
        }

        private void AddTotalInitialDeckArea(ExcelWorksheet worksheet, SimulationOutput reportOutputData, int row, int column)
        {
            var goodCount = _bridgeWorkSummaryComputationHelper.TotalInitialGoodDeckArea(reportOutputData);
            worksheet.Cells[row, column].Value = goodCount;

            var poorCount = _bridgeWorkSummaryComputationHelper.TotalInitialPoorDeckArea(reportOutputData);
            worksheet.Cells[row + 2, column].Value = poorCount;

            var closedCount = _bridgeWorkSummaryComputationHelper.TotalInitialClosedDeckArea(reportOutputData);
            worksheet.Cells[row + 3, column].Value = closedCount;

            var fairCount = _bridgeWorkSummaryComputationHelper.InitialTotalDeckArea(reportOutputData) - (goodCount + poorCount);
            worksheet.Cells[row + 1, column].Value = fairCount;
        }

        private void AddTotalDeckArea(ExcelWorksheet worksheet, SimulationYearDetail yearlyData, int row, int column)
        {
            var goodCount = _bridgeWorkSummaryComputationHelper.CalculateTotalGoodDeckArea(yearlyData);
            worksheet.Cells[row, column].Value = goodCount;

            var poorCount = _bridgeWorkSummaryComputationHelper.CalculateTotalPoorDeckArea(yearlyData);
            worksheet.Cells[row + 2, column].Value = poorCount;

            var closedCount = _bridgeWorkSummaryComputationHelper.CalculateTotalClosedDeckArea(yearlyData);
            worksheet.Cells[row + 3, column].Value = closedCount;

            var fairCount = _bridgeWorkSummaryComputationHelper.CalculateTotalDeckArea(yearlyData) - (goodCount + poorCount);
            worksheet.Cells[row + 1, column].Value = fairCount;
        }

        #endregion Private methods
    }
}
