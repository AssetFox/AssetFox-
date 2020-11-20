using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis;
using BridgeCareCore.Interfaces.SummaryReport;
using BridgeCareCore.Models.SummaryReport;
using OfficeOpenXml;

namespace BridgeCareCore.Services.SummaryReport.BridgeWorkSummary
{
    public class CostBudgetsWorkSummary
    {
        private readonly BridgeWorkSummaryCommon _bridgeWorkSummaryCommon;
        private readonly IExcelHelper _excelHelper;
        //private readonly BridgeWorkSummaryComputationHelper _bridgeWorkSummaryComputationHelper;
        private Dictionary<int, double> TotalCulvertSpent = new Dictionary<int, double>();
        private Dictionary<int, double> TotalBridgeSpent = new Dictionary<int, double>();
        private Dictionary<int, double> TotalCommittedSpent = new Dictionary<int, double>();

        public CostBudgetsWorkSummary(BridgeWorkSummaryCommon bridgeWorkSummaryCommon, IExcelHelper excelHelper)
        {
            _bridgeWorkSummaryCommon = bridgeWorkSummaryCommon;
            _excelHelper = excelHelper;
            //_bridgeWorkSummaryComputationHelper = bridgeWorkSummaryComputationHelper;
        }
        public void FillCostBudgetWorkSummarySections(ExcelWorksheet worksheet, CurrentCell currentCell,
            SimulationOutput reportOutputData, List<int> simulationYears, List<string> treatments)
        {
            // cache to store total cost per treatment for a given year
            var costPerTreatmentPerYear = new Dictionary<int, Dictionary<string, double>>();
            foreach (var yearData in reportOutputData.Years)
            {
                costPerTreatmentPerYear.Add(yearData.Year, new Dictionary<string, double>());
                foreach (var section in yearData.Sections)
                {
                    if (section.TreatmentCause == TreatmentCause.NoSelection || section.TreatmentOptions.Count <= 0)
                    {
                        continue;
                    }
                    //[TODO] - ask Jake regarding cash flow project. It won't have anything in the TreartmentOptions barring 1st year
                    var cost = section.TreatmentOptions.Find(_ => _.TreatmentName == section.AppliedTreatment).Cost;
                    if (!costPerTreatmentPerYear[yearData.Year].ContainsKey(section.AppliedTreatment))
                    {
                        costPerTreatmentPerYear[yearData.Year].Add(section.AppliedTreatment, cost);
                    }
                    else
                    {
                        costPerTreatmentPerYear[yearData.Year][section.AppliedTreatment] += cost;
                    }
                }
            }

            var culvertTotalRow = FillCostOfCulvertWorkSection(worksheet, currentCell,
                simulationYears, treatments, costPerTreatmentPerYear);
            var bridgeTotalRow = FillCostOfBridgeWorkSection(worksheet, currentCell,
                simulationYears, treatments, costPerTreatmentPerYear);
            var budgetTotalRow = FillTotalBudgetSection(worksheet, currentCell, simulationYears, costPerTreatmentPerYear);
            FillRemainingBudgetSection(worksheet, simulationYears, currentCell, culvertTotalRow, bridgeTotalRow, budgetTotalRow);
        }

        #region Private methods

        private int FillCostOfCulvertWorkSection(ExcelWorksheet worksheet, CurrentCell currentCell,
            List<int> simulationYears, List<string> treatments,
            Dictionary<int, Dictionary<string, double>> costPerTreatmentPerYear)
        {
            _bridgeWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Cost of BAMS Culvert Work", "BAMS Culvert Work Type");
            var culvertTotalRow = AddCostsOfCulvertWork(worksheet, simulationYears, currentCell, treatments, costPerTreatmentPerYear);
            return culvertTotalRow;
        }

        private int FillCostOfBridgeWorkSection(ExcelWorksheet worksheet, CurrentCell currentCell,
            List<int> simulationYears, List<string> treatments,
            Dictionary<int, Dictionary<string, double>> costPerTreatmentPerYear)
        {
            _bridgeWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Cost of BAMS Bridge Work", "BAMS Bridge Work Type");
            var bridgeTotalRow = AddCostsOfBridgeWork(worksheet, simulationYears, currentCell, treatments, costPerTreatmentPerYear);
            return bridgeTotalRow;
        }

        private int FillTotalBudgetSection(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears,
            Dictionary<int, Dictionary<string, double>> costPerTreatmentPerYear)
        {
            _bridgeWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Total Budget", "Totals");
            worksheet.Cells[currentCell.Row, simulationYears.Count + 3].Value = "Total Analysis Budget (all year)";
            _excelHelper.ApplyStyle(worksheet.Cells[currentCell.Row, simulationYears.Count + 3]);
            _excelHelper.ApplyBorder(worksheet.Cells[currentCell.Row, simulationYears.Count + 3]);
            var budgetTotalRow = AddDetailsForTotalBudget(worksheet, simulationYears, currentCell, costPerTreatmentPerYear);
            return budgetTotalRow;
        }

        private void FillRemainingBudgetSection(ExcelWorksheet worksheet, List<int> simulationYears, CurrentCell currentCell,
            int culvertTotalRow, int bridgeTotalRow, int budgetTotalRow)
        {
            _bridgeWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Budget Analysis", "");
            worksheet.Cells[currentCell.Row, simulationYears.Count + 3].Value = "Total Remaining Budget(all years)";
            _excelHelper.ApplyStyle(worksheet.Cells[currentCell.Row, simulationYears.Count + 3]);
            _excelHelper.ApplyBorder(worksheet.Cells[currentCell.Row, simulationYears.Count + 3]);
            AddDetailsForRemainingBudget(worksheet, simulationYears, currentCell, culvertTotalRow, bridgeTotalRow, budgetTotalRow);
        }

        private int AddCostsOfCulvertWork(ExcelWorksheet worksheet,
            List<int> simulationYears, CurrentCell currentCell, List<string> treatments,
            Dictionary<int, Dictionary<string, double>> costPerTreatmentPerYear)
        {
            if(simulationYears.Count <= 0)
            {
                return 0;
            }
            int startRow, startColumn, row, column;
            _bridgeWorkSummaryCommon.SetRowColumns(currentCell, out startRow, out startColumn, out row, out column);
            int culvertTotalRow = 0;

            // filling in the culvert treatments in the excel TAB
            foreach (var item in treatments)
            {
                if (item.Contains("culvert", StringComparison.OrdinalIgnoreCase))
                {
                    worksheet.Cells[row++, column].Value = item;
                }
            }
            worksheet.Cells[row++, column].Value = Properties.Resources.CulvertTotal;
            column++;
            var fromColumn = column + 1;

            // Filling in the cost per treatment per year in the excel TAB
            foreach (var yearlyValues in costPerTreatmentPerYear)
            {
                double culvertTotalCost = 0;
                row = startRow;
                column = ++column;

                foreach (var treatment in treatments)
                {
                    if (treatment.Contains("culvert", StringComparison.OrdinalIgnoreCase))
                    {
                        yearlyValues.Value.TryGetValue(treatment, out var culvertCost);
                        //var culvertCost = _bridgeWorkSummaryComputationHelper.CalculateCost(simulationDataModels, yearlyValues.Key, item);
                        worksheet.Cells[row++, column].Value = culvertCost;
                        culvertTotalCost += culvertCost;
                    }
                }
                worksheet.Cells[row, column].Value = culvertTotalCost;
                culvertTotalRow = row;
                TotalCulvertSpent.Add(yearlyValues.Key, culvertTotalCost);
            }
            _excelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row, column]);
            _excelHelper.SetCustomFormat(worksheet.Cells[startRow, fromColumn, row, column], "NegativeCurrency");
            _excelHelper.ApplyColor(worksheet.Cells[startRow, fromColumn, row, column], Color.FromArgb(198, 224, 180));

            _excelHelper.ApplyColor(worksheet.Cells[culvertTotalRow, fromColumn, culvertTotalRow, column], Color.FromArgb(84, 130, 53));
            _excelHelper.SetTextColor(worksheet.Cells[culvertTotalRow, fromColumn, culvertTotalRow, column], Color.White);
            _bridgeWorkSummaryCommon.UpdateCurrentCell(currentCell, ++row, column);
            return culvertTotalRow;
        }
        private int AddCostsOfBridgeWork(ExcelWorksheet worksheet,
            List<int> simulationYears, CurrentCell currentCell, List<string> treatments,
            Dictionary<int, Dictionary<string, double>> costPerTreatmentPerYear)
        {
            if (simulationYears.Count <= 0)
            {
                return 0;
            }
            int startRow, startColumn, row, column;
            _bridgeWorkSummaryCommon.SetRowColumns(currentCell, out startRow, out startColumn, out row, out column);
            int bridgeTotalRow = 0;
            foreach (var item in treatments)
            {
                if (!item.Contains("culvert", StringComparison.OrdinalIgnoreCase) &&
                    !item.Contains("no treatment", StringComparison.OrdinalIgnoreCase))
                {
                    worksheet.Cells[row++, column].Value = item;
                }
            }
            worksheet.Cells[row++, column].Value = Properties.Resources.BridgeTotal;
            column++;
            var fromColumn = column + 1;
            // Filling in the cost per treatment per year in the excel TAB
            foreach (var yearlyValues in costPerTreatmentPerYear)
            {
                row = startRow;
                column = ++column;
                double nonCulvertTotalCost = 0;

                foreach (var treatment in treatments)
                {
                    if (!treatment.Contains("culvert", StringComparison.OrdinalIgnoreCase) &&
                    !treatment.Contains("no treatment", StringComparison.OrdinalIgnoreCase))
                    {
                        yearlyValues.Value.TryGetValue(treatment, out var nonCulvertCost);
                        //var nonCulvertCost = _bridgeWorkSummaryComputationHelper.CalculateCost(simulationDataModels, year, item);
                        worksheet.Cells[row++, column].Value = nonCulvertCost;
                        nonCulvertTotalCost += nonCulvertCost;
                    }
                }

                worksheet.Cells[row, column].Value = nonCulvertTotalCost;
                bridgeTotalRow = row;
                TotalBridgeSpent.Add(yearlyValues.Key, nonCulvertTotalCost);
            }
            _excelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row, column]);
            _excelHelper.SetCustomFormat(worksheet.Cells[startRow, fromColumn, row, column], "NegativeCurrency");
            _excelHelper.ApplyColor(worksheet.Cells[startRow, fromColumn, row, column], Color.FromArgb(198, 224, 180));

            _excelHelper.ApplyColor(worksheet.Cells[bridgeTotalRow, fromColumn, bridgeTotalRow, column], Color.FromArgb(84, 130, 53));
            _excelHelper.SetTextColor(worksheet.Cells[bridgeTotalRow, fromColumn, bridgeTotalRow, column], Color.White);
            _bridgeWorkSummaryCommon.UpdateCurrentCell(currentCell, ++row, column);
            return bridgeTotalRow;
        }

        private int AddDetailsForTotalBudget(ExcelWorksheet worksheet, List<int> simulationYears, CurrentCell currentCell,
            Dictionary<int, Dictionary<string, double>> costPerTreatmentPerYear)
        {
            int startRow, startColumn, row, column;
            _bridgeWorkSummaryCommon.SetRowColumns(currentCell, out startRow, out startColumn, out row, out column);
            int budgetTotalRow = 0;
            worksheet.Cells[row++, column].Value = Properties.Resources.TotalSpent;
            worksheet.Cells[row++, column].Value = Properties.Resources.TotalBridgeCareBudget;
            column++;
            var fromColumn = column + 1;
            foreach (var yearlyData in costPerTreatmentPerYear)
            {
                row = startRow;
                column = ++column;

                worksheet.Cells[row++, column].Value = TotalCulvertSpent[yearlyData.Key] + TotalBridgeSpent[yearlyData.Key]; // + TotalCommittedSpent[yearlyData.Key];
                var totalCost = yearlyData.Value.Sum(_ => _.Value);
                worksheet.Cells[row, column].Value = totalCost;
                budgetTotalRow = row;
            }
            worksheet.Cells[row, column + 1].Formula = "SUM(" + worksheet.Cells[row, fromColumn, row, column] + ")";

            _excelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row, column + 1]);
            _excelHelper.SetCustomFormat(worksheet.Cells[startRow, fromColumn, row, column + 1], "NegativeCurrency");
            _excelHelper.ApplyColor(worksheet.Cells[startRow, fromColumn, row, column], Color.FromArgb(84, 130, 53));
            _excelHelper.SetTextColor(worksheet.Cells[startRow, fromColumn, row, column], Color.White);
            _bridgeWorkSummaryCommon.UpdateCurrentCell(currentCell, ++row, column);
            return budgetTotalRow;
        }

        private void AddDetailsForRemainingBudget(ExcelWorksheet worksheet, List<int> simulationYears, CurrentCell currentCell,
            int culvertTotalRow, int bridgeTotalRow, int budgetTotalRow)
        {
            int startRow, startColumn, row, column;
            _bridgeWorkSummaryCommon.SetRowColumns(currentCell, out startRow, out startColumn, out row, out column);
            worksheet.Cells[row++, column].Value = Properties.Resources.RemainingBudget;
            worksheet.Cells[row++, column].Value = Properties.Resources.PercentBudgetSpentMPMS;
            worksheet.Cells[row++, column].Value = Properties.Resources.PercentBudgetSpentBAMS;
            column++;
            var fromColumn = column + 1;
            foreach (var year in simulationYears)
            {
                row = startRow;
                column = ++column;
                var totalSpent = Convert.ToDouble(worksheet.Cells[culvertTotalRow, column].Value) +
                    Convert.ToDouble(worksheet.Cells[bridgeTotalRow, column].Value);
                    //Convert.ToDouble(worksheet.Cells[committedTotalRow, column].Value);

                worksheet.Cells[row, column].Value = Convert.ToDouble(worksheet.Cells[budgetTotalRow, column].Value) - totalSpent;
                row++;

                // [TODO] - right now, we are not entering committed project data in the excel file
                //worksheet.Cells[row, column].Formula = worksheet.Cells[committedTotalRow, column] + "/" + totalSpent;
                row++;

                worksheet.Cells[row, column].Formula = 1 + "-" + worksheet.Cells[row - 1, column];

                //[TODO] - Because we do not have anything for committed project. % for BAMS spent will always be 100
                worksheet.Cells[row, column].Formula = 1 + "-" + 0;
            }
            worksheet.Cells[startRow, column + 1].Formula = "SUM(" + worksheet.Cells[startRow, fromColumn, startRow, column] + ")";
            worksheet.Cells[startRow, column + 2].Formula = worksheet.Cells[startRow, column + 1] + "/" + worksheet.Cells[budgetTotalRow, column + 1];
            worksheet.Cells[startRow, column + 2].Style.Numberformat.Format = "#0.00%";
            worksheet.Cells[startRow, column + 3].Value = "Percentage of Total Budget that was Unspent";

            _excelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row, column + 1]);

            _excelHelper.SetCustomFormat(worksheet.Cells[row - 2, fromColumn, row - 2, column + 1], "NegativeCurrency");
            _excelHelper.SetCustomFormat(worksheet.Cells[row - 1, fromColumn, row, column], "Percentage");

            _excelHelper.ApplyColor(worksheet.Cells[startRow, fromColumn, startRow, column], Color.Red);
            _excelHelper.ApplyColor(worksheet.Cells[startRow + 1, fromColumn, row, column], Color.FromArgb(248, 203, 173));
            _bridgeWorkSummaryCommon.UpdateCurrentCell(currentCell, row + 3, column);
            _excelHelper.ApplyColor(worksheet.Cells[row + 2, startColumn, row + 2, column], Color.DimGray);
        }

    }
    #endregion
}
