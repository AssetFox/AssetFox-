using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using AppliedResearchAssociates.iAM.Domains;
using BridgeCareCore.Interfaces.SummaryReport;
using BridgeCareCore.Models.SummaryReport;
using OfficeOpenXml;

namespace BridgeCareCore.Services.SummaryReport.BridgeWorkSummary
{
    public class CostBudgetsWorkSummary
    {
        private readonly BridgeWorkSummaryCommon _bridgeWorkSummaryCommon;
        private readonly IExcelHelper _excelHelper;
        private Dictionary<int, decimal> TotalCulvertSpent = new Dictionary<int, decimal>();
        private Dictionary<int, decimal> TotalBridgeSpent = new Dictionary<int, decimal>();
        private Dictionary<int, decimal> TotalCommittedSpent = new Dictionary<int, decimal>();

        public CostBudgetsWorkSummary(BridgeWorkSummaryCommon bridgeWorkSummaryCommon, IExcelHelper excelHelper)
        {
            _bridgeWorkSummaryCommon = bridgeWorkSummaryCommon;
            _excelHelper = excelHelper;
        }

        public void FillCostBudgetWorkSummarySections(ExcelWorksheet worksheet, CurrentCell currentCell,
            Dictionary<int, Dictionary<string, (decimal treatmentCost, int bridgeCount)>> costPerTreatmentPerYear,
Dictionary<int, Dictionary<string, (decimal treatmentCost, int bridgeCount)>> yearlyCostCommittedProj, List<int> simulationYears, SortedSet<string> treatments, Dictionary<string, Budget> yearlyBudgetAmount)
        {
            var committedTotalRow = FillCostOfCommittedWorkSection(worksheet, currentCell, simulationYears, yearlyCostCommittedProj);

            var culvertTotalRow = FillCostOfCulvertWorkSection(worksheet, currentCell,
                simulationYears, treatments, costPerTreatmentPerYear);
            var bridgeTotalRow = FillCostOfBridgeWorkSection(worksheet, currentCell,
                simulationYears, treatments, costPerTreatmentPerYear);
            var budgetTotalRow = FillTotalBudgetSection(worksheet, currentCell, simulationYears,
                costPerTreatmentPerYear, yearlyBudgetAmount);
            FillRemainingBudgetSection(worksheet, simulationYears, currentCell, culvertTotalRow, bridgeTotalRow, budgetTotalRow, committedTotalRow);
        }

        #region Private methods

        private int FillCostOfCommittedWorkSection(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears,
            Dictionary<int, Dictionary<string, (decimal treatmentCost, int bridgeCount)>> yearlyCostCommittedProj)
        {
            _bridgeWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Cost of MPMS Work", "MPMS Work Type");
            var committedTotalRow = AddCostsOfCommittedWork(worksheet, simulationYears, currentCell, yearlyCostCommittedProj);
            return committedTotalRow;
        }

        private int FillCostOfCulvertWorkSection(ExcelWorksheet worksheet, CurrentCell currentCell,
            List<int> simulationYears, SortedSet<string> treatments,
            Dictionary<int, Dictionary<string, (decimal treatmentCost, int bridgeCount)>> costPerTreatmentPerYear)
        {
            _bridgeWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Cost of BAMS Culvert Work", "BAMS Culvert Work Type");
            var culvertTotalRow = AddCostsOfCulvertWork(worksheet, simulationYears, currentCell, treatments, costPerTreatmentPerYear);
            return culvertTotalRow;
        }

        private int FillCostOfBridgeWorkSection(ExcelWorksheet worksheet, CurrentCell currentCell,
            List<int> simulationYears, SortedSet<string> treatments,
            Dictionary<int, Dictionary<string, (decimal treatmentCost, int bridgeCount)>> costPerTreatmentPerYear)
        {
            _bridgeWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Cost of BAMS Bridge Work", "BAMS Bridge Work Type");
            var bridgeTotalRow = AddCostsOfBridgeWork(worksheet, simulationYears, currentCell, treatments, costPerTreatmentPerYear);
            return bridgeTotalRow;
        }

        private int FillTotalBudgetSection(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears,
            Dictionary<int, Dictionary<string, (decimal treatmentCost, int bridgeCount)>> costPerTreatmentPerYear,
            Dictionary<string, Budget> yearlyBudgetAmount)
        {
            _bridgeWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Total Budget", "Totals");
            worksheet.Cells[currentCell.Row, simulationYears.Count + 3].Value = "Total Analysis Budget (all year)";
            _excelHelper.ApplyStyle(worksheet.Cells[currentCell.Row, simulationYears.Count + 3]);
            _excelHelper.ApplyBorder(worksheet.Cells[currentCell.Row, simulationYears.Count + 3]);
            var budgetTotalRow = AddDetailsForTotalBudget(worksheet, simulationYears[0], currentCell,
                costPerTreatmentPerYear, yearlyBudgetAmount);
            return budgetTotalRow;
        }

        private void FillRemainingBudgetSection(ExcelWorksheet worksheet, List<int> simulationYears, CurrentCell currentCell,
            int culvertTotalRow, int bridgeTotalRow, int budgetTotalRow, int committedTotalRow)
        {
            _bridgeWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Budget Analysis", "");
            worksheet.Cells[currentCell.Row, simulationYears.Count + 3].Value = "Total Remaining Budget(all years)";
            _excelHelper.ApplyStyle(worksheet.Cells[currentCell.Row, simulationYears.Count + 3]);
            _excelHelper.ApplyBorder(worksheet.Cells[currentCell.Row, simulationYears.Count + 3]);
            AddDetailsForRemainingBudget(worksheet, simulationYears, currentCell, culvertTotalRow, bridgeTotalRow, budgetTotalRow, committedTotalRow);
        }

        private int AddCostsOfCommittedWork(ExcelWorksheet worksheet, List<int> simulationYears, CurrentCell currentCell,
    Dictionary<int, Dictionary<string, (decimal treatmentCost, int bridgeCount)>> yearlyCostCommittedProj)
        {
            if (simulationYears.Count <= 0)
            {
                return 0;
            }
            var startYear = simulationYears[0];
            _bridgeWorkSummaryCommon.SetRowColumns(currentCell, out var startRow, out var startColumn, out var row, out var column);
            currentCell.Column = column;
            var committedTotalRow = 0;

            var uniqueTreatments = new Dictionary<string, int>();
            var costForTreatments = new Dictionary<string, decimal>();
            // filling in the committed treatments in the excel TAB
            foreach (var yearlyItem in yearlyCostCommittedProj)
            {
                decimal committedTotalCost = 0;
                row = currentCell.Row;
                //column = ++column;
                foreach (var data in yearlyItem.Value)
                {
                    if (!uniqueTreatments.ContainsKey(data.Key))
                    {
                        uniqueTreatments.Add(data.Key, currentCell.Row);
                        worksheet.Cells[row++, column].Value = data.Key;
                        var cellToEnterCost = yearlyItem.Key - startYear;
                        worksheet.Cells[uniqueTreatments[data.Key], column + cellToEnterCost + 2].Value = data.Value.treatmentCost;
                        costForTreatments.Add(data.Key, data.Value.treatmentCost);
                        currentCell.Row += 1;
                    }
                    else
                    {
                        var cellToEnterCost = yearlyItem.Key - startYear;
                        worksheet.Cells[uniqueTreatments[data.Key], column + cellToEnterCost + 2].Value = data.Value.treatmentCost;
                    }
                    committedTotalCost += data.Value.treatmentCost;
                }
                //worksheet.Cells[row, column].Value = committedTotalCost;
                TotalCommittedSpent.Add(yearlyItem.Key, committedTotalCost);
            }

            column = currentCell.Column;
            worksheet.Cells[currentCell.Row, column].Value = Properties.Resources.CommittedTotal;
            column++;
            var fromColumn = column + 1;

            foreach (var cost in TotalCommittedSpent)
            {
                worksheet.Cells[currentCell.Row, fromColumn++].Value = cost.Value;
            }
            committedTotalRow = currentCell.Row;
            fromColumn = column + 1;
            var endColumn = simulationYears.Count + 2;

            _excelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, committedTotalRow, endColumn]);
            _excelHelper.SetCustomFormat(worksheet.Cells[startRow, fromColumn, committedTotalRow, endColumn], "NegativeCurrency");
            _excelHelper.ApplyColor(worksheet.Cells[startRow, fromColumn, committedTotalRow, endColumn], Color.FromArgb(198, 224, 180));

            _excelHelper.ApplyColor(worksheet.Cells[committedTotalRow, fromColumn, committedTotalRow, endColumn], Color.FromArgb(84, 130, 53));
            _excelHelper.SetTextColor(worksheet.Cells[committedTotalRow, fromColumn, committedTotalRow, endColumn], Color.White);
            _bridgeWorkSummaryCommon.UpdateCurrentCell(currentCell, committedTotalRow + 1, endColumn);

            return committedTotalRow;
        }

        private int AddCostsOfCulvertWork(ExcelWorksheet worksheet,
            List<int> simulationYears, CurrentCell currentCell, SortedSet<string> treatments,
            Dictionary<int, Dictionary<string, (decimal treatmentCost, int bridgeCount)>> costPerTreatmentPerYear)
        {
            if (simulationYears.Count <= 0)
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
                decimal culvertTotalCost = 0;
                row = startRow;
                column = ++column;

                foreach (var treatment in treatments)
                {
                    if (treatment.Contains("culvert", StringComparison.OrdinalIgnoreCase))
                    {
                        yearlyValues.Value.TryGetValue(treatment, out var culvertCostAndCount);
                        worksheet.Cells[row++, column].Value = culvertCostAndCount.treatmentCost;
                        culvertTotalCost += culvertCostAndCount.treatmentCost;
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
            List<int> simulationYears, CurrentCell currentCell, SortedSet<string> treatments,
            Dictionary<int, Dictionary<string, (decimal treatmentCost, int bridgeCount)>> costPerTreatmentPerYear)
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
                    !item.Contains(Properties.Resources.NoTreatment, StringComparison.OrdinalIgnoreCase))
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
                decimal nonCulvertTotalCost = 0;

                foreach (var treatment in treatments)
                {
                    if (!treatment.Contains(Properties.Resources.Culvert, StringComparison.OrdinalIgnoreCase) &&
                    !treatment.Contains(Properties.Resources.NoTreatment, StringComparison.OrdinalIgnoreCase))
                    {
                        yearlyValues.Value.TryGetValue(treatment, out var nonCulvertCostAndCount);
                        worksheet.Cells[row++, column].Value = nonCulvertCostAndCount.treatmentCost;
                        nonCulvertTotalCost += nonCulvertCostAndCount.treatmentCost;
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

        private int AddDetailsForTotalBudget(ExcelWorksheet worksheet, int initialYear, CurrentCell currentCell,
            Dictionary<int, Dictionary<string, (decimal treatmentCost, int bridgeCount)>> costPerTreatmentPerYear,
            Dictionary<string, Budget> yearlyBudgetAmount)
        {
            int startRow, startColumn, row, column;
            _bridgeWorkSummaryCommon.SetRowColumns(currentCell, out startRow, out startColumn, out row, out column);
            int budgetTotalRow = 0;
            worksheet.Cells[row++, column].Value = Properties.Resources.TotalSpent;
            row++; // for committed project
            worksheet.Cells[row++, column].Value = Properties.Resources.TotalBridgeCareBudget;
            column++;
            var fromColumn = column + 1;
            foreach (var yearlyData in costPerTreatmentPerYear)
            {
                row = startRow;
                column = ++column;

                worksheet.Cells[row++, column].Value = TotalCulvertSpent[yearlyData.Key] + TotalBridgeSpent[yearlyData.Key]; // + TotalCommittedSpent[yearlyData.Key];
                row++; // for committed project
                var totalCost = yearlyBudgetAmount.Values.Sum(_ => _.YearlyAmounts[yearlyData.Key - initialYear].Value);
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
            int culvertTotalRow, int bridgeTotalRow, int budgetTotalRow, int committedTotalRow)
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
                Convert.ToDouble(worksheet.Cells[committedTotalRow, column].Value);

                worksheet.Cells[row, column].Value = Convert.ToDouble(worksheet.Cells[budgetTotalRow, column].Value) - totalSpent;
                row++;

                if (totalSpent != 0)
                {
                    worksheet.Cells[row, column].Formula = worksheet.Cells[committedTotalRow, column] + "/" + totalSpent;
                }
                else
                {
                    worksheet.Cells[row, column].Value = 0;
                }
                row++;

                worksheet.Cells[row, column].Formula = 1 + "-" + worksheet.Cells[row - 1, column];
            }
            worksheet.Cells[startRow, column + 1].Formula = "SUM(" + worksheet.Cells[startRow, fromColumn, startRow, column] + ")";
            if (worksheet.Cells[budgetTotalRow, column + 1].Value != null)
            {
                worksheet.Cells[startRow, column + 2].Formula = worksheet.Cells[startRow, column + 1] + "/" + worksheet.Cells[budgetTotalRow, column + 1];
            }

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

    #endregion Private methods
}
