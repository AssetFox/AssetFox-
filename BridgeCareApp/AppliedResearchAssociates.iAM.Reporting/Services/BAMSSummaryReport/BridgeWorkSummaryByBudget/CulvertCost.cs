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
    public class CulvertCost
    {
        private BridgeWorkSummaryCommon _bridgeWorkSummaryCommon;

        public CulvertCost()
        {
            _bridgeWorkSummaryCommon = new BridgeWorkSummaryCommon();
        }

        internal void FillCostOfCulvert(ExcelWorksheet worksheet, CurrentCell currentCell, List<YearsData> costForCulvertBudget,
            Dictionary<int, double> totalBudgetPerYearForCulvert, List<int> simulationYears, WorkTypeTotal workTypeTotal)
        {
            var startYear = simulationYears[0];
            _bridgeWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Cost of BAMS Culvert Work", "BAMS Culvert Work Type");

            currentCell.Row += 1;
            var startOfCulvertBudget = currentCell.Row += 1;
            currentCell.Column = 1;

            var treatmentTracker = new Dictionary<string, int>();
            foreach (var treatment in costForCulvertBudget)
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

            worksheet.Cells[currentCell.Row, currentCell.Column].Value = AuditReportConstants.CulvertTotal;

            foreach (var totalculvertBudget in totalBudgetPerYearForCulvert)
            {
                var cellToEnterTotalCulvertCost = totalculvertBudget.Key - startYear;
                worksheet.Cells[currentCell.Row, currentCell.Column + cellToEnterTotalCulvertCost + 2].Value = totalculvertBudget.Value;
            }
            ExcelHelper.ApplyBorder(worksheet.Cells[startOfCulvertBudget, currentCell.Column, currentCell.Row, simulationYears.Count + 2]);
            ExcelHelper.SetCustomFormat(worksheet.Cells[startOfCulvertBudget, currentCell.Column + 2, currentCell.Row, simulationYears.Count + 2], ExcelHelperCellFormat.NegativeCurrency);
            ExcelHelper.ApplyColor(worksheet.Cells[startOfCulvertBudget, currentCell.Column + 2, currentCell.Row, simulationYears.Count + 2], Color.DarkSeaGreen);

            ExcelHelper.ApplyColor(worksheet.Cells[currentCell.Row, currentCell.Column + 2, currentCell.Row, simulationYears.Count + 2], Color.FromArgb(84, 130, 53));
            ExcelHelper.SetTextColor(worksheet.Cells[currentCell.Row, currentCell.Column + 2, currentCell.Row, simulationYears.Count + 2], Color.White);
        }
    }
}
