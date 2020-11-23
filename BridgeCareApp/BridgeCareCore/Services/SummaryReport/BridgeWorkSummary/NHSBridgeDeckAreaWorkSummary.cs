using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Analysis;
using BridgeCareCore.Interfaces.SummaryReport;
using BridgeCareCore.Models.SummaryReport;
using OfficeOpenXml;

namespace BridgeCareCore.Services.SummaryReport.BridgeWorkSummary
{
    public class NHSBridgeDeckAreaWorkSummary
    {
        private readonly BridgeWorkSummaryCommon _bridgeWorkSummaryCommon;
        private readonly IExcelHelper _excelHelper;
        private readonly BridgeWorkSummaryComputationHelper _bridgeWorkSummaryComputationHelper;

        public NHSBridgeDeckAreaWorkSummary(BridgeWorkSummaryCommon bridgeWorkSummaryCommon, ExcelHelper excelHelper, BridgeWorkSummaryComputationHelper bridgeWorkSummaryComputationHelper)
        {
            _bridgeWorkSummaryCommon = bridgeWorkSummaryCommon ?? throw new ArgumentNullException(nameof(bridgeWorkSummaryCommon));
            _excelHelper = excelHelper ?? throw new ArgumentNullException(nameof(excelHelper));
            _bridgeWorkSummaryComputationHelper = bridgeWorkSummaryComputationHelper ?? throw new ArgumentNullException(nameof(bridgeWorkSummaryComputationHelper));
        }
        internal void FillNHSBridgeDeckAreaWorkSummarySections(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, SimulationOutput reportOutputData, ChartRowsModel chartRowsModel)
        {
            var dataStartRow = FillNHSBridgeCountSection(worksheet, currentCell, simulationYears);
            chartRowsModel.NHSBridgeCountSectionYearsRow = dataStartRow - 1;
            FillNHSBridgeCountPercentSection(worksheet, currentCell, simulationYears, dataStartRow, chartRowsModel);

            dataStartRow = FillNonNHSBridgeCountSection(worksheet, currentCell, simulationYears,
                chartRowsModel.TotalBridgeCountSectionYearsRow, chartRowsModel.NHSBridgeCountSectionYearsRow, chartRowsModel);
            chartRowsModel.NonNHSBridgeCountSectionYearsRow = dataStartRow - 1;

            FillNonNHSBridgeCountPercentSection(worksheet, currentCell, simulationYears, dataStartRow, chartRowsModel);

            dataStartRow = FillNHSBridgeDeckAreaSection(worksheet, currentCell, simulationYears);
            chartRowsModel.NHSDeckAreaSectionYearsRow = dataStartRow - 1;
            FillNHSBridgeDeckAreaPercentSection(worksheet, currentCell, simulationYears, dataStartRow, chartRowsModel);

            dataStartRow = FillNonNHSDeckAreaSection(worksheet, currentCell, simulationYears, chartRowsModel.TotalDeckAreaSectionYearsRow,
                chartRowsModel.NHSDeckAreaSectionYearsRow, chartRowsModel);
            chartRowsModel.NonNHSDeckAreaYearsRow = dataStartRow - 1;

            FillNonNHSDeckAreaPercentSection(worksheet, currentCell, simulationYears, dataStartRow, chartRowsModel);
        }

        #region
        private int FillNHSBridgeCountSection(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears)
        {
            _bridgeWorkSummaryCommon.AddBridgeHeaders(worksheet, currentCell, simulationYears, "NHS Bridge Count", true);
            var dataStartRow = currentCell.Row + 1;
            AddDetailsForNHSBridgeCount(worksheet, currentCell, simulationYears);
            return dataStartRow;
        }
        private void AddDetailsForNHSBridgeCount(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears)
        {
            int startRow, startColumn, row, column;
            _bridgeWorkSummaryCommon.InitializeLabelCells(worksheet, currentCell, out startRow, out startColumn, out row, out column);
            AddNHSBridgeCount(worksheet, startRow, column, 0);
            foreach (var year in simulationYears)
            {
                row = startRow;
                column = ++column;
                AddNHSBridgeCount(worksheet, row, column, year);
            }
            _excelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row + 2, column]);
            _bridgeWorkSummaryCommon.UpdateCurrentCell(currentCell, row + 3, column);
        }
        private void AddNHSBridgeCount(ExcelWorksheet worksheet, int row, int column, int year)
        {
            var goodCount = _bridgeWorkSummaryComputationHelper.CalculateNHSBridgeGoodCount(year);
            worksheet.Cells[row, column].Value = goodCount;

            var poorCount = _bridgeWorkSummaryComputationHelper.CalculateNHSBridgePoorCount(year);
            worksheet.Cells[row + 2, column].Value = poorCount;

            var yNHSCount = bridgeDataModels.FindAll(b => b.NHS == "Y").Count;
            worksheet.Cells[row + 1, column].Value = yNHSCount - (goodCount + poorCount);
        }
        #endregion
    }
}