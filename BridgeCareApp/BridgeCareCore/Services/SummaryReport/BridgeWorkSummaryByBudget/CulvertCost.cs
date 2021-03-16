using System;
using System.Collections.Generic;
using System.Drawing;
using BridgeCareCore.Interfaces.SummaryReport;
using BridgeCareCore.Models.SummaryReport;
using BridgeCareCore.Services.SummaryReport.BridgeWorkSummary;
using OfficeOpenXml;

namespace BridgeCareCore.Services.SummaryReport.BridgeWorkSummaryByBudget
{
    public class CulvertCost
    {
        private readonly IExcelHelper _excelHelper;
        private readonly BridgeWorkSummaryCommon _bridgeWorkSummaryCommon;

        public CulvertCost(IExcelHelper excelHelper, BridgeWorkSummaryCommon bridgeWorkSummaryCommon)
        {
            _excelHelper = excelHelper;
            _bridgeWorkSummaryCommon = bridgeWorkSummaryCommon ?? throw new ArgumentNullException(nameof(bridgeWorkSummaryCommon));
        }

        internal void FillCostOfCulvert(ExcelWorksheet worksheet, CurrentCell currentCell, List<YearsData> costForCulvertBudget,
            Dictionary<int, double> totalBudgetPerYearForCulvert, List<string> culvertTreatments, List<int> simulationYears)
        {
            var startYear = simulationYears[0];
            _bridgeWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Cost of BAMS Culvert Work", "BAMS Culvert Work Type");

            currentCell.Row += 1;
            var startOfCulvertBudget = currentCell.Row += 1;
            currentCell.Column = 1;

            var treatmentTracker = new Dictionary<string, int>();
            //var uniqueTreatments = new Dictionary<string, int>();

            foreach (var treatment in culvertTreatments)
            {
                worksheet.Cells[currentCell.Row, currentCell.Column].Value = treatment;
                treatmentTracker.Add(treatment, currentCell.Row);
                worksheet.Cells[currentCell.Row, currentCell.Column + 2, currentCell.Row,
                    currentCell.Column + 1 + simulationYears.Count].Value = 0.0;
                currentCell.Row += 1;
            }
            foreach (var item in costForCulvertBudget)
            {
                var rowNum = treatmentTracker[item.Treatment];
                //if (!uniqueTreatments.ContainsKey(item.Treatment))
                //{
                //    uniqueTreatments.Add(item.Treatment, currentCell.Row);
                //    worksheet.Cells[currentCell.Row, currentCell.Column].Value = item.Treatment;
                //    var cellToEnterCost = item.Year - startYear;
                //    worksheet.Cells[uniqueTreatments[item.Treatment], currentCell.Column + cellToEnterCost + 2].Value = item.Amount;
                //    currentCell.Row += 1;
                //}
                //else
                //{
                var cellToEnterCost = item.Year - startYear;
                var cellValue = worksheet.Cells[rowNum, currentCell.Column + cellToEnterCost + 2].Value;
                var totalAmount = 0.0;
                if (cellValue != null)
                {
                    totalAmount = (double)cellValue;
                }
                totalAmount += item.Amount;
                worksheet.Cells[rowNum, currentCell.Column + cellToEnterCost + 2].Value = totalAmount;
                //}
            }

            worksheet.Cells[currentCell.Row, currentCell.Column].Value = Properties.Resources.CulvertTotal;

            foreach (var totalculvertBudget in totalBudgetPerYearForCulvert)
            {
                var cellToEnterTotalCulvertCost = totalculvertBudget.Key - startYear;
                worksheet.Cells[currentCell.Row, currentCell.Column + cellToEnterTotalCulvertCost + 2].Value = totalculvertBudget.Value;
            }
            _excelHelper.ApplyBorder(worksheet.Cells[startOfCulvertBudget, currentCell.Column, currentCell.Row, simulationYears.Count + 2]);
            _excelHelper.SetCustomFormat(worksheet.Cells[startOfCulvertBudget, currentCell.Column + 2, currentCell.Row, simulationYears.Count + 2], "NegativeCurrency");
            _excelHelper.ApplyColor(worksheet.Cells[startOfCulvertBudget, currentCell.Column + 2, currentCell.Row, simulationYears.Count + 2], Color.DarkSeaGreen);

            _excelHelper.ApplyColor(worksheet.Cells[currentCell.Row, currentCell.Column + 2, currentCell.Row, simulationYears.Count + 2], Color.FromArgb(84, 130, 53));
            _excelHelper.SetTextColor(worksheet.Cells[currentCell.Row, currentCell.Column + 2, currentCell.Row, simulationYears.Count + 2], Color.White);
        }
    }
}
