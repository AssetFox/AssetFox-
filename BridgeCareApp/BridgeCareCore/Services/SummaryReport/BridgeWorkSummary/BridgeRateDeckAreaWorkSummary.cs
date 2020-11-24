using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Analysis;
using BridgeCareCore.Interfaces.SummaryReport;
using BridgeCareCore.Models.SummaryReport;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace BridgeCareCore.Services.SummaryReport.BridgeWorkSummary
{
    public class BridgeRateDeckAreaWorkSummary
    {
        private readonly BridgeWorkSummaryCommon _bridgeWorkSummaryCommon;
        private readonly IExcelHelper _excelHelper;
        private readonly BridgeWorkSummaryComputationHelper _bridgeWorkSummaryComputationHelper;

        public BridgeRateDeckAreaWorkSummary(BridgeWorkSummaryCommon bridgeWorkSummaryCommon, IExcelHelper excelHelper,
            BridgeWorkSummaryComputationHelper bridgeWorkSummaryComputationHelper)
        {
            _bridgeWorkSummaryCommon = bridgeWorkSummaryCommon;
            _excelHelper = excelHelper;
            _bridgeWorkSummaryComputationHelper = bridgeWorkSummaryComputationHelper;
        }
        public ChartRowsModel FillBridgeRateDeckAreaWorkSummarySections(ExcelWorksheet worksheet, CurrentCell currentCell,
            List<int> simulationYears, WorkSummaryModel workSummaryModel, SimulationOutput reportOutputData)
        {
            var chartRowsModel = new ChartRowsModel();
            FillPoorBridgeOnOffRateSection(worksheet, currentCell, simulationYears, workSummaryModel.PoorOnOffCount);
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
            List<int> simulationYears, Dictionary<int, (int on, int off)> poorOnOffPerYear)
        {
            currentCell.Row = currentCell.Row + 2;
            _bridgeWorkSummaryCommon.AddBridgeHeaders(worksheet, currentCell, simulationYears, "Poor Bridge On and Off Rate", false);
            AddDetailsForPoorBridgeOnOfRate(worksheet, currentCell, poorOnOffPerYear);
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
        private void AddDetailsForPoorBridgeOnOfRate(ExcelWorksheet worksheet, CurrentCell currentCell,
            Dictionary<int, (int on, int off)> poorOnOffPerYear)
        {
            int startRow, startColumn, row, column;
            _bridgeWorkSummaryCommon.SetRowColumns(currentCell, out startRow, out startColumn, out row, out column);
            worksheet.Cells[row++, column].Value = "Number Bridge On";
            worksheet.Cells[row - 1, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            worksheet.Cells[row++, column].Value = "Number Bridge Off";
            worksheet.Cells[row - 1, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            column++;
            foreach (var yearlyData in poorOnOffPerYear)
            {
                row = startRow;
                column = ++column;
                worksheet.Cells[row, column].Value = yearlyData.Value.on;
                worksheet.Cells[++row, column].Value = yearlyData.Value.off;
            }
            _excelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row, column]);
            _bridgeWorkSummaryCommon.UpdateCurrentCell(currentCell, ++row, column);
        }
        private void AddDetailsForTotalPoorBridgesCount(ExcelWorksheet worksheet, CurrentCell currentCell,
             SimulationOutput reportOutputData)
        {
            int startRow, startColumn, row, column;
            _bridgeWorkSummaryCommon.SetRowColumns(currentCell, out startRow, out startColumn, out row, out column);
            worksheet.Cells[row, column++].Value = Properties.Resources.BridgeCare;
            worksheet.Cells[row, column - 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            worksheet.Cells[row, column].Value = _bridgeWorkSummaryComputationHelper.TotalInitialPoorBridgesCount(reportOutputData);
            foreach (var yearlyData in reportOutputData.Years)
            {
                column = ++column;
                worksheet.Cells[row, column].Value = _bridgeWorkSummaryComputationHelper.TotalSectionalPoorBridgesCount(yearlyData);
            }
            _excelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row, column]);
            _bridgeWorkSummaryCommon.UpdateCurrentCell(currentCell, ++row, column);
        }
        private void AddDetailsForTotalPoorBridgesDeckArea(ExcelWorksheet worksheet, CurrentCell currentCell,
            SimulationOutput reportOutputData)
        {
            int startRow, startColumn, row, column;
            _bridgeWorkSummaryCommon.SetRowColumns(currentCell, out startRow, out startColumn, out row, out column);
            worksheet.Cells[row, column++].Value = Properties.Resources.BridgeCare;
            worksheet.Cells[row, column - 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            worksheet.Cells[row, column].Value = _bridgeWorkSummaryComputationHelper.TotalInitialPoorBridgesDeckArea(reportOutputData);
            foreach (var yearlyData in reportOutputData.Years)
            {
                column = ++column;
                worksheet.Cells[row, column].Value = _bridgeWorkSummaryComputationHelper.CalculateTotalPoorBridgesDeckArea(yearlyData);
            }
            _excelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row, column]);
            _excelHelper.SetCustomFormat(worksheet.Cells[startRow, startColumn + 1, row, column], "Number");
            _bridgeWorkSummaryCommon.UpdateCurrentCell(currentCell, ++row, column);
        }
        private void AddDetailsForTotalBridgeCount(ExcelWorksheet worksheet, CurrentCell currentCell, SimulationOutput reportOutputData)
        {
            var totalCount = reportOutputData.InitialSectionSummaries.Count;
            int startRow, startColumn, row, column;
            _bridgeWorkSummaryCommon.InitializeLabelCells(worksheet, currentCell, out startRow, out startColumn, out row, out column);
            AddInitialBridgeCount(worksheet, reportOutputData, totalCount, startRow, column);
            foreach (var yearlyData in reportOutputData.Years)
            {
                row = startRow;
                column = ++column;
                AddTotalBridgeCount(worksheet, yearlyData, totalCount, row, column);
            }
            _excelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row + 2, column]);
            _bridgeWorkSummaryCommon.UpdateCurrentCell(currentCell, row + 3, column);
        }
        private void AddInitialBridgeCount(ExcelWorksheet worksheet, SimulationOutput reportOutputData,
            int totalSimulationDataModelCount, int row, int column)
        {
            var goodCount = _bridgeWorkSummaryComputationHelper.TotalInitialBridgeGoodCount(reportOutputData);
            worksheet.Cells[row, column].Value = goodCount;

            var poorCount = _bridgeWorkSummaryComputationHelper.TotalInitialPoorBridgesCount(reportOutputData);
            worksheet.Cells[row + 2, column].Value = poorCount;

            worksheet.Cells[row + 1, column].Value = totalSimulationDataModelCount - (goodCount + poorCount);
        }
        private void AddTotalBridgeCount(ExcelWorksheet worksheet, SimulationYearDetail yearlyData, int totalSimulationDataModelCount, int row, int column)
        {
            var goodCount = _bridgeWorkSummaryComputationHelper.CalculateTotalBridgeGoodCount(yearlyData);
            worksheet.Cells[row, column].Value = goodCount;

            var poorCount = _bridgeWorkSummaryComputationHelper.CalculateTotalBridgePoorCount(yearlyData);
            worksheet.Cells[row + 2, column].Value = poorCount;

            worksheet.Cells[row + 1, column].Value = totalSimulationDataModelCount - (goodCount + poorCount);
        }

        private void AddDetailsForTotalBridgeAndDeckPercent(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, int dataStartRow)
        {
            int startRow, startColumn, row, column;
            _bridgeWorkSummaryCommon.InitializeLabelCells(worksheet, currentCell, out startRow, out startColumn, out row, out column);
            for (var index = 0; index <= simulationYears.Count; index++)
            {
                var sumFormula = "SUM(" + worksheet.Cells[dataStartRow, column, dataStartRow + 2, column] + ")";
                worksheet.Cells[startRow, column].Formula = worksheet.Cells[dataStartRow, column] + "/" + sumFormula;
                worksheet.Cells[startRow + 1, column].Formula = worksheet.Cells[dataStartRow + 1, column] + "/" + sumFormula;
                worksheet.Cells[startRow + 2, column].Formula = worksheet.Cells[dataStartRow + 2, column] + "/" + sumFormula;
                column++;
            }
            _excelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, startRow + 2, column - 1]);
            _excelHelper.SetCustomFormat(worksheet.Cells[startRow, startColumn + 1, startRow + 2, column], "Percentage");
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
            _excelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row + 2, column]);
            _excelHelper.SetCustomFormat(worksheet.Cells[startRow, startColumn + 1, row + 2, column], "Number");
            _bridgeWorkSummaryCommon.UpdateCurrentCell(currentCell, row + 3, column);
        }
        private void AddTotalInitialDeckArea(ExcelWorksheet worksheet, SimulationOutput reportOutputData, int row, int column)
        {
            var goodCount = _bridgeWorkSummaryComputationHelper.TotalInitialGoodDeckArea(reportOutputData);
            worksheet.Cells[row, column].Value = goodCount;

            var poorCount = _bridgeWorkSummaryComputationHelper.TotalInitialPoorDeckArea(reportOutputData);
            worksheet.Cells[row + 2, column].Value = poorCount;

            worksheet.Cells[row + 1, column].Value = _bridgeWorkSummaryComputationHelper.InitialTotalDeckArea(reportOutputData) - (goodCount + poorCount);
        }
        private void AddTotalDeckArea(ExcelWorksheet worksheet, SimulationYearDetail yearlyData, int row, int column)
        {
            var goodCount = _bridgeWorkSummaryComputationHelper.CalculateTotalGoodDeckArea(yearlyData);
            worksheet.Cells[row, column].Value = goodCount;

            var poorCount = _bridgeWorkSummaryComputationHelper.CalculateTotalPoorDeckArea(yearlyData);
            worksheet.Cells[row + 2, column].Value = poorCount;

            worksheet.Cells[row + 1, column].Value = _bridgeWorkSummaryComputationHelper.CalculateTotalDeckArea(yearlyData) - (goodCount + poorCount);
        }

        #endregion
    }
}
