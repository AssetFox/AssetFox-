using System;
using System.Collections.Generic;
using System.Drawing;
using BridgeCareCore.Models.SummaryReport;
using BridgeCareCore.Services.SummaryReport.BridgeWorkSummary;
using OfficeOpenXml;

namespace BridgeCareCore.Services.SummaryReport.BridgeWorkSummaryByBudget
{
    public class CulvertCost
    {
        private readonly BridgeWorkSummaryCommon _bridgeWorkSummaryCommon;

        public CulvertCost(BridgeWorkSummaryCommon bridgeWorkSummaryCommon)
        {
            _bridgeWorkSummaryCommon = bridgeWorkSummaryCommon ?? throw new ArgumentNullException(nameof(bridgeWorkSummaryCommon));
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
            }
            foreach (var item in costForCulvertBudget)
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

                FillWorkTypeTotals(item, workTypeTotal);
            }

            worksheet.Cells[currentCell.Row, currentCell.Column].Value = Properties.Resources.CulvertTotal;

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

        private void FillWorkTypeTotals(YearsData item, WorkTypeTotal workTypeTotal)
        {
            CulvertTreatmentMap.Map.TryGetValue(item.Treatment, out var treatment);
            switch (treatment)
            {
            case CulvertTreatmentName.CulvertRehabOther:
                if (!workTypeTotal.CulvertRehabCostPerYear.ContainsKey(item.Year))
                {
                    workTypeTotal.CulvertRehabCostPerYear.Add(item.Year, 0);
                }
                if (!workTypeTotal.TotalCostPerYear.ContainsKey(item.Year))
                {
                    workTypeTotal.TotalCostPerYear.Add(item.Year, 0);
                }
                workTypeTotal.CulvertRehabCostPerYear[item.Year] += item.Amount;
                workTypeTotal.TotalCostPerYear[item.Year] += item.Amount;
                break;
            case CulvertTreatmentName.CulvertReplacementBoxFrameArch:
            case CulvertTreatmentName.CulvertReplacementOther:
            case CulvertTreatmentName.CulvertReplacementPipe:
                if (!workTypeTotal.CulvertReplacementCostPerYear.ContainsKey(item.Year))
                {
                    workTypeTotal.CulvertReplacementCostPerYear.Add(item.Year, 0);
                }
                if (!workTypeTotal.TotalCostPerYear.ContainsKey(item.Year))
                {
                    workTypeTotal.TotalCostPerYear.Add(item.Year, 0);
                }
                workTypeTotal.CulvertReplacementCostPerYear[item.Year] += item.Amount;
                workTypeTotal.TotalCostPerYear[item.Year] += item.Amount;
                break;
            default:
                if (!workTypeTotal.OtherCostPerYear.ContainsKey(item.Year))
                {
                    workTypeTotal.OtherCostPerYear.Add(item.Year, 0);
                }
                if (!workTypeTotal.TotalCostPerYear.ContainsKey(item.Year))
                {
                    workTypeTotal.TotalCostPerYear.Add(item.Year, 0);
                }
                workTypeTotal.OtherCostPerYear[item.Year] += item.Amount;
                workTypeTotal.TotalCostPerYear[item.Year] += item.Amount;
                break;
            }
        }
    }
}
