using System;
using System.Collections.Generic;
using System.Drawing;
using AppliedResearchAssociates.iAM.Analysis;
using BridgeCareCore.Interfaces.SummaryReport;
using BridgeCareCore.Models.SummaryReport;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace BridgeCareCore.Services.SummaryReport.BridgeWorkSummary
{
    public class PostedClosedBridgeWorkSummary
    {
        private readonly BridgeWorkSummaryCommon _bridgeWorkSummaryCommon;
        private readonly IExcelHelper _excelHelper;
        private readonly BridgeWorkSummaryComputationHelper _bridgeWorkSummaryComputationHelper;

        private Dictionary<int, int> TotalPostedBridgeCount = new Dictionary<int, int>();
        private Dictionary<int, int> TotalClosedBridgeCount = new Dictionary<int, int>();

        public PostedClosedBridgeWorkSummary(BridgeWorkSummaryCommon bridgeWorkSummaryCommon, IExcelHelper excelHelper, BridgeWorkSummaryComputationHelper bridgeWorkSummaryComputationHelper)
        {
            _bridgeWorkSummaryCommon = bridgeWorkSummaryCommon ?? throw new ArgumentNullException(nameof(bridgeWorkSummaryCommon));
            _excelHelper = excelHelper ?? throw new ArgumentNullException(nameof(excelHelper));
            _bridgeWorkSummaryComputationHelper = bridgeWorkSummaryComputationHelper ?? throw new ArgumentNullException(nameof(bridgeWorkSummaryComputationHelper));
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
            worksheet.Cells[startRow + 4, column - 1].Value = "Annualized Amount";
            worksheet.Cells[startRow + 4, column - 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
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
                worksheet.Cells[row + 4, startColumn + i + 2].Value = totalMoney / reportOutputData.Years.Count;
            }
            _excelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row + 4, column]);
            _excelHelper.SetCustomFormat(worksheet.Cells[startRow, startColumn, row + 4, column], "NegativeCurrency");
            _excelHelper.ApplyColor(worksheet.Cells[startRow, startColumn + 2, row + 4, column], Color.FromArgb(198, 224, 180));
            _bridgeWorkSummaryCommon.UpdateCurrentCell(currentCell, row + 4, column);
        }

        private double AddMoneyNeededByBPN(ExcelWorksheet worksheet, int row, int column, List<SectionDetail> sectionDetails)
        {
            var totalMoney = 0.0;
            var moneyForBPN = _bridgeWorkSummaryComputationHelper.CalculateMoneyNeededByBPN13(sectionDetails, "1");
            worksheet.Cells[row++, column].Value = moneyForBPN;
            totalMoney += moneyForBPN;
            moneyForBPN = _bridgeWorkSummaryComputationHelper.CalculateMoneyNeededByBPN2H(sectionDetails);
            worksheet.Cells[row++, column].Value = moneyForBPN;
            totalMoney += moneyForBPN;
            moneyForBPN = _bridgeWorkSummaryComputationHelper.CalculateMoneyNeededByBPN13(sectionDetails, "3");
            worksheet.Cells[row++, column].Value = moneyForBPN;
            totalMoney += moneyForBPN;
            moneyForBPN = _bridgeWorkSummaryComputationHelper.CalculateMoneyNeededByRemainingBPN(sectionDetails);
            worksheet.Cells[row, column].Value = moneyForBPN;
            totalMoney += moneyForBPN;
            return totalMoney;
        }
    }
}
