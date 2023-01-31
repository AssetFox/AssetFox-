using System;
using System.Collections.Generic;
using System.Drawing;
using OfficeOpenXml;

using AppliedResearchAssociates.iAM.ExcelHelpers;

using AppliedResearchAssociates.iAM.Reporting.Models.BAMSSummaryReport;
using AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport.BridgeWorkSummary;
using AppliedResearchAssociates.iAM.Reporting.Models;

namespace AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport.BridgeWorkSummaryByBudget
{
    public class BridgeWorkCost
    {
        private BridgeWorkSummaryCommon _bridgeWorkSummaryCommon;

        public BridgeWorkCost()
        {
            _bridgeWorkSummaryCommon = new BridgeWorkSummaryCommon();
        }

        internal void FillCostOfBridgeWork(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears,
            List<YearsData> costForBridgeBudgets, Dictionary<int, double> totalBudgetPerYearForBridgeWork, WorkTypeTotal workTypeTotal)
        {
            var startYear = simulationYears[0];
            currentCell.Row += 1;
            _bridgeWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Cost of BAMS Bridge Work", "BAMS Bridge Work Type");

            currentCell.Row += 1;
            var startOfBridgeBudget = currentCell.Row;
            currentCell.Column = 1;
            var treatmentTracker = new Dictionary<string, int>();
            foreach (var treatment in costForBridgeBudgets)
            {
                if (!treatmentTracker.ContainsKey(treatment.Treatment))
                {
                    treatmentTracker.Add(treatment.Treatment, currentCell.Row);
                    worksheet.Cells[currentCell.Row, currentCell.Column].Value = treatment.Treatment;

                    worksheet.Cells[currentCell.Row, currentCell.Column + 2, currentCell.Row,
                    currentCell.Column + 1 + simulationYears.Count].Value = 0.0;
                    currentCell.Row += 1;
                }

                var rowNum = treatmentTracker[treatment.Treatment];
                var cellToEnterCost = treatment.Year - startYear;
                var cellValue = worksheet.Cells[rowNum, currentCell.Column + cellToEnterCost + 2].Value;
                var totalAmount = 0.0;
                if (cellValue != null)
                {
                    totalAmount = (double)cellValue;
                }
                totalAmount += treatment.Amount;
                worksheet.Cells[rowNum, currentCell.Column + cellToEnterCost + 2].Value = totalAmount;
                WorkTypeTotalHelper.FillWorkTypeTotals(treatment, workTypeTotal);
            }

            worksheet.Cells[currentCell.Row, currentCell.Column].Value = BAMSConstants.BridgeTotal;

            foreach (var totalBridgeBudget in totalBudgetPerYearForBridgeWork)
            {
                var cellToEnterTotalBridgeCost = totalBridgeBudget.Key - startYear;
                worksheet.Cells[currentCell.Row, currentCell.Column + cellToEnterTotalBridgeCost + 2].Value = totalBridgeBudget.Value;
            }
            ExcelHelper.ApplyBorder(worksheet.Cells[startOfBridgeBudget, currentCell.Column, currentCell.Row, simulationYears.Count + 2]);
            ExcelHelper.SetCustomFormat(worksheet.Cells[startOfBridgeBudget, currentCell.Column + 2, currentCell.Row, simulationYears.Count + 2], ExcelHelperCellFormat.NegativeCurrency);
            ExcelHelper.ApplyColor(worksheet.Cells[startOfBridgeBudget, currentCell.Column + 2, currentCell.Row, simulationYears.Count + 2], Color.DarkSeaGreen);

            ExcelHelper.ApplyColor(worksheet.Cells[currentCell.Row, currentCell.Column + 2, currentCell.Row, simulationYears.Count + 2], Color.FromArgb(84, 130, 53));
            ExcelHelper.SetTextColor(worksheet.Cells[currentCell.Row, currentCell.Column + 2, currentCell.Row, simulationYears.Count + 2], Color.White);
        }
    }
}
