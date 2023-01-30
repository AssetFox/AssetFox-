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
    public class CommittedProjectCost
    {
        private readonly BridgeWorkSummaryCommon _bridgeWorkSummaryCommon;

        public CommittedProjectCost()
        {
            _bridgeWorkSummaryCommon = new BridgeWorkSummaryCommon();
        }

        internal void FillCostOfCommittedWork(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears,
            List<YearsData> costForCommittedBudgets, HashSet<string> committedTreatments,
            Dictionary<int, double> totalBudgetPerYearForCommittedWork, WorkTypeTotal workTypeTotal)
        {
            var startYear = simulationYears[0];
            currentCell.Row += 1;
            _bridgeWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Cost of MPMS Work", "MPMS Work Type");

            currentCell.Row += 1;
            var startOfCommittedBudget = currentCell.Row;
            currentCell.Column = 1;
            var treatmentTracker = new Dictionary<string, int>();
            foreach (var treatment in committedTreatments)
            {
                worksheet.Cells[currentCell.Row, currentCell.Column].Value = treatment;
                treatmentTracker.Add(treatment, currentCell.Row);
                worksheet.Cells[currentCell.Row, currentCell.Column + 2, currentCell.Row,
                    currentCell.Column + 1 + simulationYears.Count].Value = 0.0;
                currentCell.Row += 1;
            }
            foreach (var item in costForCommittedBudgets)
            {
                var rowNum = treatmentTracker[item.Treatment];

                var cellToEnterCost = item.Year - startYear;
                var cellValue = worksheet.Cells[rowNum, currentCell.Column + cellToEnterCost + 2].Value;
                var totalAmount = 0.0;
                if (cellValue != null)
                {
                    totalAmount = (double)cellValue;
                }
                totalAmount += item.Amount;
                worksheet.Cells[rowNum, currentCell.Column + cellToEnterCost + 2].Value = totalAmount;

                WorkTypeTotalHelper.FillWorkTypeTotals(item, workTypeTotal);
            }

            worksheet.Cells[currentCell.Row, currentCell.Column].Value = AuditReportConstants.BridgeTotal;

            foreach (var totalCommittedBudget in totalBudgetPerYearForCommittedWork)
            {
                var cellToEnterTotalBridgeCost = totalCommittedBudget.Key - startYear;
                worksheet.Cells[currentCell.Row, currentCell.Column + cellToEnterTotalBridgeCost + 2].Value = totalCommittedBudget.Value;
            }
            ExcelHelper.ApplyBorder(worksheet.Cells[startOfCommittedBudget, currentCell.Column, currentCell.Row, simulationYears.Count + 2]);
            ExcelHelper.SetCustomFormat(worksheet.Cells[startOfCommittedBudget, currentCell.Column + 2, currentCell.Row, simulationYears.Count + 2], ExcelHelperCellFormat.NegativeCurrency);
            ExcelHelper.ApplyColor(worksheet.Cells[startOfCommittedBudget, currentCell.Column + 2, currentCell.Row, simulationYears.Count + 2], Color.DarkSeaGreen);

            ExcelHelper.ApplyColor(worksheet.Cells[currentCell.Row, currentCell.Column + 2, currentCell.Row, simulationYears.Count + 2], Color.FromArgb(84, 130, 53));
            ExcelHelper.SetTextColor(worksheet.Cells[currentCell.Row, currentCell.Column + 2, currentCell.Row, simulationYears.Count + 2], Color.White);
            currentCell.Row++;
        }
    }
}
