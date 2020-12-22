using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using BridgeCareCore.Interfaces.SummaryReport;
using BridgeCareCore.Models.SummaryReport;
using BridgeCareCore.Services.SummaryReport.BridgeWorkSummary;
using OfficeOpenXml;

namespace BridgeCareCore.Services.SummaryReport.BridgeWorkSummaryByBudget
{
    public class BridgeWorkCost
    {
        private readonly IExcelHelper _excelHelper;
        private readonly BridgeWorkSummaryCommon _bridgeWorkSummaryCommon;

        public BridgeWorkCost(IExcelHelper excelHelper, BridgeWorkSummaryCommon bridgeWorkSummaryCommon)
        {
            _excelHelper = excelHelper;
            _bridgeWorkSummaryCommon = bridgeWorkSummaryCommon ?? throw new ArgumentNullException(nameof(bridgeWorkSummaryCommon));
        }
        internal void FillCostOfBridgeWork(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears,
            List<YearsData> costForBridgeBudgets, List<string> nonCulvertTreatments,
            Dictionary<int, double> totalBudgetPerYearForBridgeWork)
        {
            var startYear = simulationYears[0];
            currentCell.Row += 1;
            _bridgeWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Cost of BAMS Bridge Work", "BAMS Bridge Work Type");

            currentCell.Row += 1;
            var startOfBridgeBudget = currentCell.Row;
            currentCell.Column = 1;
           // var uniqueTreatments = new Dictionary<string, int>();
            // Fill Bridge Budget
            var treatmentTracker = new Dictionary<string, int>();
            foreach (var treatment in nonCulvertTreatments)
            {
                worksheet.Cells[currentCell.Row, currentCell.Column].Value = treatment;
                treatmentTracker.Add(treatment, currentCell.Row);
                worksheet.Cells[currentCell.Row, currentCell.Column + 2, currentCell.Row,
                    currentCell.Column + 1 + simulationYears.Count].Value = 0.0;
                currentCell.Row += 1;
            }
            //costForBridgeBudgets.Sort((a, b) => a.Treatment.CompareTo(b.Treatment));
            foreach (var item in costForBridgeBudgets)
            {
                var rowNum = treatmentTracker[item.Treatment];
                //if (!uniqueTreatments.ContainsKey(item.Treatment))
                //{
                //    uniqueTreatments.Add(item.Treatment, currentCell.Row);
                //    //worksheet.Cells[currentCell.Row, currentCell.Column].Value = item.Treatment;
                //    var cellToEnterCost = item.Year - startYear;
                //    worksheet.Cells[rowNum, currentCell.Column + cellToEnterCost + 2].Value = item.Amount;
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
            
            worksheet.Cells[currentCell.Row, currentCell.Column].Value = Properties.Resources.BridgeTotal;

            foreach (var totalBridgeBudget in totalBudgetPerYearForBridgeWork)
            {
                var cellToEnterTotalBridgeCost = totalBridgeBudget.Key - startYear;
                worksheet.Cells[currentCell.Row, currentCell.Column + cellToEnterTotalBridgeCost + 2].Value = totalBridgeBudget.Value;
            }
            _excelHelper.ApplyBorder(worksheet.Cells[startOfBridgeBudget, currentCell.Column, currentCell.Row, simulationYears.Count + 2]);
            _excelHelper.SetCustomFormat(worksheet.Cells[startOfBridgeBudget, currentCell.Column + 2, currentCell.Row, simulationYears.Count + 2], "NegativeCurrency");
            _excelHelper.ApplyColor(worksheet.Cells[startOfBridgeBudget, currentCell.Column + 2, currentCell.Row, simulationYears.Count + 2], Color.DarkSeaGreen);

            _excelHelper.ApplyColor(worksheet.Cells[currentCell.Row, currentCell.Column + 2, currentCell.Row, simulationYears.Count + 2], Color.FromArgb(84, 130, 53));
            _excelHelper.SetTextColor(worksheet.Cells[currentCell.Row, currentCell.Column + 2, currentCell.Row, simulationYears.Count + 2], Color.White);
        }
    }
}
