using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.Reporting.Models.BAMSSummaryReport;
using AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport.BridgeWorkSummary.StaticContent;
using AppliedResearchAssociates.iAM.ExcelHelpers;
using AppliedResearchAssociates.iAM.Reporting.Models;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using System;

namespace AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport.BridgeWorkSummary
{
    public class PostedClosedBridgeWorkSummary
    {
        private BridgeWorkSummaryCommon _bridgeWorkSummaryCommon;
        private BridgeWorkSummaryComputationHelper _bridgeWorkSummaryComputationHelper;
        private readonly IUnitOfWork _unitOfWork;

        public PostedClosedBridgeWorkSummary(WorkSummaryModel workSummaryModel,IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _bridgeWorkSummaryCommon = new BridgeWorkSummaryCommon(); 
            _bridgeWorkSummaryComputationHelper = new BridgeWorkSummaryComputationHelper(_unitOfWork);
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
            AddInitialPostedBridgesCountBPN(worksheet, startRow, column, reportOutputData.InitialAssetSummaries);
            foreach (var yearlyData in reportOutputData.Years)
            {
                row = startRow;
                column = ++column;
                AddPostedBridgesCountBPN(worksheet, row, column, yearlyData.Assets);
            }
            var bpnNames = EnumExtensions.GetValues<BPNName>();
            ExcelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row + bpnNames.Count - 1, column]);
            _bridgeWorkSummaryCommon.UpdateCurrentCell(currentCell, row + bpnNames.Count, column);
        }


        private void AddInitialPostedBridgesCountBPN(ExcelWorksheet worksheet, int row, int column, List<AssetSummaryDetail> initialSectionSummaries)
        {
            var bpnNames = EnumExtensions.GetValues<BPNName>();
            for (var bpnName = bpnNames[0]; bpnName <= bpnNames.Last(); bpnName++)
            {
                var bpnKey = bpnName.ToMatchInDictionary();
                var postedBPNCount = _bridgeWorkSummaryComputationHelper.CalculatePostedCountOrDeckAreaForBPN(initialSectionSummaries, bpnKey, true);
                worksheet.Cells[row++, column].Value = postedBPNCount;
            }
        }

        private void AddPostedBridgesCountBPN(ExcelWorksheet worksheet, int row, int column, List<AssetDetail> sectionDetails)
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
            AddInitialPostedBridgesDeckArea(worksheet, startRow, column, reportOutputData.InitialAssetSummaries);
            foreach (var yearlyData in reportOutputData.Years)
            {
                row = startRow;
                column = ++column;
                AddPostedBridgesDeckArea(worksheet, row, column, yearlyData.Assets);
            }
            var bpnNames = EnumExtensions.GetValues<BPNName>();
            ExcelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row + bpnNames.Count - 1, column]);
            _bridgeWorkSummaryCommon.UpdateCurrentCell(currentCell, row + bpnNames.Count, column);
        }

        private void AddInitialPostedBridgesDeckArea(ExcelWorksheet worksheet, int row, int column, List<AssetSummaryDetail> initialSectionSummaries)
        {
            var bpnNames = EnumExtensions.GetValues<BPNName>();
            for (var bpnName = bpnNames[0]; bpnName <= bpnNames.Last(); bpnName++)
            {
                var bpnKey = bpnName.ToMatchInDictionary();
                var postedDeckArea = _bridgeWorkSummaryComputationHelper.CalculatePostedCountOrDeckAreaForBPN(initialSectionSummaries, bpnKey, false);
                worksheet.Cells[row++, column].Value = postedDeckArea;
            }
        }

        private void AddPostedBridgesDeckArea(ExcelWorksheet worksheet, int row, int column, List<AssetDetail> sectionDetails)
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
            AddInitialClosedBridgesCountBPN(worksheet, startRow, column, reportOutputData.InitialAssetSummaries);
            foreach (var yearlyData in reportOutputData.Years)
            {
                row = startRow;
                column = ++column;
                AddClosedBridgesCountBPN(worksheet, row, column, yearlyData.Assets);
            }
            var bpnNames = EnumExtensions.GetValues<BPNName>();
            ExcelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row + bpnNames.Count - 1, column]);
            _bridgeWorkSummaryCommon.UpdateCurrentCell(currentCell, row + bpnNames.Count, column);
        }


        private void AddInitialClosedBridgesCountBPN(ExcelWorksheet worksheet, int row, int column, List<AssetSummaryDetail> initialSectionSummaries)
        {
            var bpnNames = EnumExtensions.GetValues<BPNName>();
            for (var bpnName = bpnNames[0]; bpnName <= bpnNames.Last(); bpnName++)
            {
                var bpnKey = bpnName.ToMatchInDictionary();
                var closedDecArea = _bridgeWorkSummaryComputationHelper.CalculateClosedCountOrDeckAreaForBPN(initialSectionSummaries, bpnKey, true);
                worksheet.Cells[row++, column].Value = closedDecArea;
            }
        }

        private void AddClosedBridgesCountBPN(ExcelWorksheet worksheet, int row, int column, List<AssetDetail> sectionDetails)
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
            AddInitialClosedBridgesDeckArea(worksheet, startRow, column, reportOutputData.InitialAssetSummaries);
            foreach (var yearlyData in reportOutputData.Years)
            {
                row = startRow;
                column = ++column;
                AddClosedBridgesDeckArea(worksheet, row, column, yearlyData.Assets);
            }
            var bpnNames = EnumExtensions.GetValues<BPNName>();
            ExcelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row + bpnNames.Count - 1, column]);
            _bridgeWorkSummaryCommon.UpdateCurrentCell(currentCell, row + bpnNames.Count, column);
        }

        private void AddInitialClosedBridgesDeckArea(ExcelWorksheet worksheet, int row, int column, List<AssetSummaryDetail> initialSectionSummaries)
        {
            var bpnNames = EnumExtensions.GetValues<BPNName>();
            for (var bpnName = bpnNames[0]; bpnName <= bpnNames.Last(); bpnName++)
            {
                var bpnKey = bpnName.ToMatchInDictionary();
                var closedDeckArea = _bridgeWorkSummaryComputationHelper.CalculateClosedCountOrDeckAreaForBPN(initialSectionSummaries, bpnKey, false);
                worksheet.Cells[row++, column].Value = closedDeckArea;
            }
        }

        private void AddClosedBridgesDeckArea(ExcelWorksheet worksheet, int row, int column, List<AssetDetail> sectionDetails)
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
            worksheet.Cells[row++, column].Value = "Closed";
            worksheet.Cells[row, column++].Value = "Posted";

            row = startRow;
            worksheet.Cells[row++, column].Value = _bridgeWorkSummaryComputationHelper.CalculateClosedCount(reportOutputData.InitialAssetSummaries); 
            worksheet.Cells[row, column].Value = _bridgeWorkSummaryComputationHelper.CalculatePostedCount(reportOutputData.InitialAssetSummaries);
            foreach (var yearlyData in reportOutputData.Years)
            {
                row = startRow; column = ++column;

                worksheet.Cells[row++, column].Value = _bridgeWorkSummaryComputationHelper.CalculateClosedCount(yearlyData.Assets);
                worksheet.Cells[row++, column].Value = _bridgeWorkSummaryComputationHelper.CalculatePostedCount(yearlyData.Assets);                
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

            // Left side row labels
            _bridgeWorkSummaryCommon.InitializeBPNLabels(worksheet, currentCell, out startRow, out startColumn, out row, out column);

            var bpnNames = EnumExtensions.GetValues<BPNName>();
            var bpnRowCount = bpnNames.Count;

            worksheet.Cells[startRow + bpnRowCount, column - 1].Value = "Annualized Amount";
            worksheet.Cells[startRow + bpnRowCount, column - 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

            var totalMoney = 0.0;

            var yearlyAssetsByBPN = new Dictionary<string, List<AssetDetail>>();

            // Dollar amounts
            foreach (var yearlyData in reportOutputData.Years)
            {
                row = startRow;
                column = ++column;

                var totalMoneyPerYear = MoneyNeededBPNColumnForAssets(worksheet, row, column, yearlyData.Assets, reportOutputData, yearlyData);

                totalMoney += totalMoneyPerYear;
            }

            //calculate BPN Annualized Amount for all the years
            double bpnAnnualizedAmount = 0;
            if (reportOutputData?.Years?.Any() == true)
            {
                bpnAnnualizedAmount = totalMoney / reportOutputData.Years.Count;
            }

            //fill BPN annualized amount
            for (var i = 0; i < reportOutputData.Years.Count; i++)
            {
                worksheet.Cells[row + bpnRowCount, startColumn + i + 2].Value = bpnAnnualizedAmount;
            }
            ExcelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row + bpnRowCount, column]);
            ExcelHelper.SetCustomFormat(worksheet.Cells[startRow, startColumn, row + bpnRowCount, column], ExcelHelperCellFormat.NegativeCurrency);
            ExcelHelper.ApplyColor(worksheet.Cells[startRow, startColumn + 2, row + bpnRowCount, column], Color.FromArgb(198, 224, 180));
            _bridgeWorkSummaryCommon.UpdateCurrentCell(currentCell, row + bpnRowCount, column);
        }

        private double MoneyNeededBPNColumnForAssets(ExcelWorksheet worksheet, int row, int column, List<AssetDetail> assets, SimulationOutput simulationOutput, SimulationYearDetail currentYearDetail)
        {
            var totalMoney = 0.0;

            var bpnKeys = EnumExtensions.GetValues<BPNName>().Select(_ => _.ToMatchInDictionary());
            foreach (var bpnKey in bpnKeys)
            {
                var moneyForBPN = _bridgeWorkSummaryComputationHelper.CalculateMoneyNeededByBPN(assets, bpnKey, simulationOutput, currentYearDetail);
                worksheet.Cells[row++, column].Value = moneyForBPN;
                totalMoney += moneyForBPN;
            }

            return totalMoney;
        }
    }
}
