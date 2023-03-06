using System;
using System.Collections.Generic;
using OfficeOpenXml;

using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.ExcelHelpers;

using AppliedResearchAssociates.iAM.Reporting.Models.BAMSSummaryReport;
using AppliedResearchAssociates.iAM.Reporting.Models;

namespace AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport.BridgeWorkSummary
{
    public class NHSBridgeDeckAreaWorkSummary
    {
        private BridgeWorkSummaryCommon _bridgeWorkSummaryCommon;
        private BridgeWorkSummaryComputationHelper _bridgeWorkSummaryComputationHelper;

        public NHSBridgeDeckAreaWorkSummary()
        {
            _bridgeWorkSummaryCommon = new BridgeWorkSummaryCommon();
            _bridgeWorkSummaryComputationHelper = new BridgeWorkSummaryComputationHelper();
        }

        internal void FillNHSBridgeDeckAreaWorkSummarySections(ExcelWorksheet worksheet, CurrentCell currentCell,
            List<int> simulationYears, SimulationOutput reportOutputData, ChartRowsModel chartRowsModel)
        {
            var dataStartRow = FillNHSBridgeCountSection(worksheet, currentCell, simulationYears, reportOutputData);
            chartRowsModel.NHSBridgeCountSectionYearsRow = dataStartRow - 1;
            FillNHSBridgeCountPercentSection(worksheet, currentCell, simulationYears, dataStartRow, chartRowsModel);

            dataStartRow = FillNonNHSBridgeCountSection(worksheet, currentCell, simulationYears,
                chartRowsModel.TotalBridgeCountSectionYearsRow, chartRowsModel.NHSBridgeCountSectionYearsRow, chartRowsModel);
            chartRowsModel.NonNHSBridgeCountSectionYearsRow = dataStartRow - 1;

            FillNonNHSBridgeCountPercentSection(worksheet, currentCell, simulationYears, dataStartRow, chartRowsModel);

            dataStartRow = FillNHSBridgeDeckAreaSection(worksheet, currentCell, simulationYears, reportOutputData);
            chartRowsModel.NHSDeckAreaSectionYearsRow = dataStartRow - 1;
            FillNHSBridgeDeckAreaPercentSection(worksheet, currentCell, simulationYears, dataStartRow, chartRowsModel);

            dataStartRow = FillNonNHSDeckAreaSection(worksheet, currentCell, simulationYears, chartRowsModel.TotalDeckAreaSectionYearsRow,
                chartRowsModel.NHSDeckAreaSectionYearsRow);
            chartRowsModel.NonNHSDeckAreaYearsRow = dataStartRow - 1;

            FillNonNHSDeckAreaPercentSection(worksheet, currentCell, simulationYears, dataStartRow, chartRowsModel);
        }

        #region

        private int FillNHSBridgeCountSection(ExcelWorksheet worksheet, CurrentCell currentCell,
            List<int> simulationYears, SimulationOutput reportOutputData)
        {
            _bridgeWorkSummaryCommon.AddBridgeHeaders(worksheet, currentCell, simulationYears, "NHS Bridge Count", true);
            var dataStartRow = currentCell.Row + 1;
            AddDetailsForNHSBridgeCount(worksheet, currentCell, reportOutputData);
            return dataStartRow;
        }

        private int FillNonNHSBridgeCountSection(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, int totalBridgeCountSectionStartRow, int nHSBridgeCountSectionStartRow, ChartRowsModel chartRowsModel)
        {
            _bridgeWorkSummaryCommon.AddBridgeHeaders(worksheet, currentCell, simulationYears, "Non-NHS Bridge Count", true);
            var dataStartRow = currentCell.Row + 1;
            AddDetailsForNonNHSCountAndArea(worksheet, currentCell, simulationYears, totalBridgeCountSectionStartRow, nHSBridgeCountSectionStartRow, false);
            return dataStartRow;
        }

        private void AddDetailsForNHSBridgeCount(ExcelWorksheet worksheet, CurrentCell currentCell,
             SimulationOutput reportOutputData)
        {
            int startRow, startColumn, row, column;
            _bridgeWorkSummaryCommon.InitializeLabelCells(worksheet, currentCell, out startRow, out startColumn, out row, out column);
            AddNHSBridgeCount(worksheet, startRow, column, reportOutputData.InitialAssetSummaries, null);
            foreach (var yearlyData in reportOutputData.Years)
            {
                row = startRow;
                column = ++column;
                AddNHSBridgeCount(worksheet, row, column, null, yearlyData.Assets);
            }
            ExcelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row + 3, column]);
            _bridgeWorkSummaryCommon.UpdateCurrentCell(currentCell, row + 4, column);
        }

        private void FillNHSBridgeCountPercentSection(ExcelWorksheet worksheet, CurrentCell currentCell,
            List<int> simulationYears, int dataStartRow, ChartRowsModel chartRowsModel)
        {
            _bridgeWorkSummaryCommon.AddBridgeHeaders(worksheet, currentCell, simulationYears, "NHS Bridge Count Percentage", true);
            chartRowsModel.NHSBridgeCountPercentSectionYearsRow = currentCell.Row;
            AddDetailsForNHSPercentSection(worksheet, currentCell, simulationYears, dataStartRow);
        }

        private int FillNHSBridgeDeckAreaSection(ExcelWorksheet worksheet, CurrentCell currentCell,
            List<int> simulationYears, SimulationOutput reportOutputData)
        {
            _bridgeWorkSummaryCommon.AddBridgeHeaders(worksheet, currentCell, simulationYears, "NHS Bridge Deck Area", true);
            var dataStartRow = currentCell.Row + 1;
            AddDetailsForNHSBridgeDeckArea(worksheet, currentCell, reportOutputData);
            return dataStartRow;
        }

        private void FillNonNHSBridgeCountPercentSection(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears,
            int dataStartRow, ChartRowsModel chartRowsModel)
        {
            _bridgeWorkSummaryCommon.AddBridgeHeaders(worksheet, currentCell, simulationYears, "Non-NHS Bridge Count Percentage", true);
            chartRowsModel.NonNHSBridgeCountPercentSectionYearsRow = currentCell.Row;
            AddDetailsForNonNHSPercentSection(worksheet, currentCell, simulationYears, dataStartRow);
        }

        private void FillNHSBridgeDeckAreaPercentSection(ExcelWorksheet worksheet, CurrentCell currentCell,
            List<int> simulationYears, int dataStartRow, ChartRowsModel chartRowsModel)
        {
            _bridgeWorkSummaryCommon.AddBridgeHeaders(worksheet, currentCell, simulationYears, "NHS Bridge Deck Area Percentage", true);
            chartRowsModel.NHSBridgeDeckAreaPercentSectionYearsRow = currentCell.Row;
            AddDetailsForNHSPercentSection(worksheet, currentCell, simulationYears, dataStartRow);
        }

        private int FillNonNHSDeckAreaSection(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears,
            int totalDeckAreaSectionStartRow, int nHSDeckAreaSectionStartRow)
        {
            _bridgeWorkSummaryCommon.AddBridgeHeaders(worksheet, currentCell, simulationYears, "Non-NHS Deck Area", true);
            var dataStartRow = currentCell.Row + 1;
            AddDetailsForNonNHSCountAndArea(worksheet, currentCell, simulationYears, totalDeckAreaSectionStartRow, nHSDeckAreaSectionStartRow, true);
            return dataStartRow;
        }

        private void FillNonNHSDeckAreaPercentSection(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears,
            int dataStartRow, ChartRowsModel chartRowsModel)
        {
            _bridgeWorkSummaryCommon.AddBridgeHeaders(worksheet, currentCell, simulationYears, "Non-NHS Bridge Deck Area Percentage", true);
            chartRowsModel.NonNHSDeckAreaPercentSectionYearsRow = currentCell.Row;
            AddDetailsForNonNHSPercentSection(worksheet, currentCell, simulationYears, dataStartRow);
        }

        private void AddDetailsForNonNHSPercentSection(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, int dataStartRow)
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

        private void AddNHSBridgeCount(ExcelWorksheet worksheet, int row, int column,
            List<AssetSummaryDetail> initialSectionSummaries, List<AssetDetail> sectionDetails)
        {
            int goodCount;
            int poorCount;
            int closedCount;
            if (initialSectionSummaries != null)
            {
                goodCount = (int)_bridgeWorkSummaryComputationHelper.InitialNHSBridgeGoodCountOrArea(initialSectionSummaries, true);
                poorCount = (int)_bridgeWorkSummaryComputationHelper.InitialNHSBridgePoorCountOrArea(initialSectionSummaries, true);
                closedCount = (int)_bridgeWorkSummaryComputationHelper.InitialNHSBridgeClosedCountOrArea(initialSectionSummaries, true);
            }
            else
            {
                goodCount = (int)_bridgeWorkSummaryComputationHelper.SectionalNHSBridgeGoodCountOrArea(sectionDetails, true);
                poorCount = (int)_bridgeWorkSummaryComputationHelper.SectionalNHSBridgePoorCountOrArea(sectionDetails, true);
                closedCount = (int)_bridgeWorkSummaryComputationHelper.SectionalNHSBridgeClosedCountOrArea(sectionDetails, true);
            }
            worksheet.Cells[row, column].Value = goodCount;
            worksheet.Cells[row + 2, column].Value = poorCount;
            worksheet.Cells[row + 3, column].Value = closedCount;

            var yNHSCount = _bridgeWorkSummaryComputationHelper.TotalNHSBridgeCountOrArea(initialSectionSummaries, sectionDetails, true);
            var fairCount = yNHSCount - (goodCount + poorCount);
            worksheet.Cells[row + 1, column].Value = fairCount;
        }

        private void AddDetailsForNHSPercentSection(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, int dataStartRow)
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

        private void AddDetailsForNonNHSCountAndArea(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears,
            int totalBridgeCountOrDeckAreaStartRow, int nHSBridgeCountOrDeckAreaStartRow, bool isDeckArea)
        {
            int startRow, startColumn, row, column;
            _bridgeWorkSummaryCommon.InitializeLabelCells(worksheet, currentCell, out startRow, out startColumn, out row, out column);
            for (var index = 0; index <= simulationYears.Count; index++)
            {
                var bridgeGood = Convert.ToDouble(worksheet.Cells[totalBridgeCountOrDeckAreaStartRow + 1, column].Value);
                var bridgeFair = Convert.ToDouble(worksheet.Cells[totalBridgeCountOrDeckAreaStartRow + 2, column].Value);
                var bridgePoor = Convert.ToDouble(worksheet.Cells[totalBridgeCountOrDeckAreaStartRow + 3, column].Value);
                var bridgeClosed = Convert.ToDouble(worksheet.Cells[totalBridgeCountOrDeckAreaStartRow + 4, column].Value);

                var NHSGood = Convert.ToDouble(worksheet.Cells[nHSBridgeCountOrDeckAreaStartRow + 1, column].Value);
                var NHSFair = Convert.ToDouble(worksheet.Cells[nHSBridgeCountOrDeckAreaStartRow + 2, column].Value);
                var NHSPoor = Convert.ToDouble(worksheet.Cells[nHSBridgeCountOrDeckAreaStartRow + 3, column].Value);
                var NHSClosed = Convert.ToDouble(worksheet.Cells[nHSBridgeCountOrDeckAreaStartRow + 4, column].Value);

                var nonNHSGood = bridgeGood - NHSGood;
                var nonNHSFair = bridgeFair - NHSFair;
                var nonNHSPoor = bridgePoor - NHSPoor;
                var nonNHSClosed = bridgeClosed - NHSClosed;

                worksheet.Cells[startRow, column].Value = nonNHSGood;
                worksheet.Cells[startRow + 1, column].Value = nonNHSFair;
                worksheet.Cells[startRow + 2, column].Value = nonNHSPoor;
                worksheet.Cells[startRow + 3, column].Value = nonNHSClosed;
                column++;
            }

            ExcelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row - 1, column - 1]);
            ExcelHelper.SetCustomFormat(worksheet.Cells[startRow, startColumn + 1, row - 1, column - 1], ExcelHelperCellFormat.Number);
            _bridgeWorkSummaryCommon.UpdateCurrentCell(currentCell, row, column);
        }

        private void AddDetailsForNHSBridgeDeckArea(ExcelWorksheet worksheet, CurrentCell currentCell,
            SimulationOutput reportOutputData)
        {
            int startRow, startColumn, row, column;
            _bridgeWorkSummaryCommon.InitializeLabelCells(worksheet, currentCell, out startRow, out startColumn, out row, out column);
            AddNHSBridgeDeckArea(worksheet, startRow, column, reportOutputData.InitialAssetSummaries, null);
            foreach (var yearlyData in reportOutputData.Years)
            {
                row = startRow;
                column = ++column;
                AddNHSBridgeDeckArea(worksheet, row, column, null, yearlyData.Assets);
            }
            ExcelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row + 3, column]);
            ExcelHelper.SetCustomFormat(worksheet.Cells[startRow, startColumn + 1, row + 3, column], ExcelHelperCellFormat.Number);
            _bridgeWorkSummaryCommon.UpdateCurrentCell(currentCell, row + 3, column);
        }

        private void AddNHSBridgeDeckArea(ExcelWorksheet worksheet, int row, int column,
            List<AssetSummaryDetail> initialSectionSummaries, List<AssetDetail> sectionDetails)
        {
            double goodDeckArea;
            double poorDeckArea;
            double closedDeckArea;
            if (initialSectionSummaries != null)
            {
                goodDeckArea = _bridgeWorkSummaryComputationHelper.InitialNHSBridgeGoodCountOrArea(initialSectionSummaries, false);
                poorDeckArea = _bridgeWorkSummaryComputationHelper.InitialNHSBridgePoorCountOrArea(initialSectionSummaries, false);
                closedDeckArea = _bridgeWorkSummaryComputationHelper.InitialNHSBridgeClosedCountOrArea(initialSectionSummaries, false);
            }
            else
            {
                goodDeckArea = _bridgeWorkSummaryComputationHelper.SectionalNHSBridgeGoodCountOrArea(sectionDetails, false);
                poorDeckArea = _bridgeWorkSummaryComputationHelper.SectionalNHSBridgePoorCountOrArea(sectionDetails, false); ;
                closedDeckArea = _bridgeWorkSummaryComputationHelper.SectionalNHSBridgeClosedCountOrArea(sectionDetails, false); ;
            }
            worksheet.Cells[row, column].Value = goodDeckArea;
            worksheet.Cells[row + 2, column].Value = poorDeckArea;
            worksheet.Cells[row + 3, column].Value = closedDeckArea;

            var totalDeckArea = _bridgeWorkSummaryComputationHelper.TotalNHSBridgeCountOrArea(initialSectionSummaries, sectionDetails, false);
            var fairDeckArea = totalDeckArea - (goodDeckArea + poorDeckArea);
            worksheet.Cells[row + 1, column].Value = fairDeckArea;
        }

        #endregion
    }
}
