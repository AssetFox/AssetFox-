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
            var bridgeTotalRange = FillCostOfBridgeWorkSection(worksheet, currentCell,
                simulationYears, treatments, costPerTreatmentPerYear);
            var bridgeTotalRow = bridgeTotalRange.FooterRange.End.Value;
            var map = WorkTypeMap.Map;
            var workTypeTotalRow = FillWorkTypeTotalsSection(worksheet, currentCell, simulationYears, map, bridgeTotalRange.ContentRange, yearlyBudgetAmount);
            var bpnTotalRow = FillBpnSection(worksheet, currentCell, simulationYears);
            FillRemainingBudgetSection(worksheet, simulationYears, currentCell, culvertTotalRow, bridgeTotalRow, bpnTotalRow, committedTotalRow);
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

        private ExcelTableRowRanges FillCostOfBridgeWorkSection(ExcelWorksheet worksheet, CurrentCell currentCell,
            List<int> simulationYears, SortedSet<string> treatments,
            Dictionary<int, Dictionary<string, (decimal treatmentCost, int bridgeCount)>> costPerTreatmentPerYear)
        {
            var headerRange = new Range(currentCell.Row, currentCell.Row + 1);
            _bridgeWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Cost of BAMS Bridge Work", "BAMS Bridge Work Type");
            var bridgeTotalRow = AddCostsOfBridgeWork(worksheet, simulationYears, currentCell, treatments, costPerTreatmentPerYear);
            var contentRange = new Range(headerRange.End.Value + 1, bridgeTotalRow - 1);
            var footerRange = new Range(bridgeTotalRow, bridgeTotalRow);
            return new ExcelTableRowRanges
            {
                HeaderRange = headerRange,
                ContentRange = contentRange,
                FooterRange = footerRange,
            };
        }


        private int FillWorkTypeTotalsSection(
            ExcelWorksheet worksheet,
            CurrentCell currentCell,
            List<int> simulationYears,
            Dictionary<string, WorkTypeName> workTypeMap,
            Range rangeForSummands,
            Dictionary<string, Budget> yearlyBudgetAmount)
        {
            _bridgeWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "", "BAMS Work Type Totals");
            var initialRow = currentCell.Row;
            currentCell.Row++;
            var workTypes = EnumExtensions.GetValues<WorkTypeName>();
            var numberOfYears = simulationYears.Count;
            worksheet.Cells[initialRow, 3 + numberOfYears].Value = "Total (all years)";
            var totalColumnHeaderRange = worksheet.Cells[initialRow, 3 + numberOfYears];
            _excelHelper.ApplyBorder(totalColumnHeaderRange);
            _excelHelper.ApplyStyle(totalColumnHeaderRange);


            var startColumnIndex = 3;
            var firstContentRow = currentCell.Row;
            for (var workType = workTypes[0]; workType <= workTypes.Last(); workType++)
            {
                var rowIndex = firstContentRow + (int)workType;
                worksheet.Cells[rowIndex, 1].Value = workType.ToSpreadsheetString();
                for (var columnIndex = startColumnIndex; columnIndex < startColumnIndex + numberOfYears; columnIndex++)
                {
                    var cellsToSum = new List<ExcelRange>();
                    for (var summandRowIndex = rangeForSummands.Start.Value; summandRowIndex <= rangeForSummands.End.Value; summandRowIndex++)
                    {
                        var summandRowTitle = worksheet.Cells[summandRowIndex, 1].Value?.ToString();
                        if (summandRowTitle != null)
                        {
                            var summandWorkType = workTypeMap.ContainsKey(summandRowTitle) ? workTypeMap[summandRowTitle] : WorkTypeName.Other;
                            if (workType == summandWorkType)
                            {
                                var cell = worksheet.Cells[summandRowIndex, columnIndex];
                                cellsToSum.Add(cell);
                            }
                        }
                        if (cellsToSum.Any())
                        {
                            worksheet.Cells[rowIndex, columnIndex].Formula = ExcelFormulas.SumOrReference(cellsToSum);
                        }
                        else
                        {
                            worksheet.Cells[rowIndex, columnIndex].Value = 0;
                        }
                    }
                }
                worksheet.Cells[rowIndex, startColumnIndex + numberOfYears].Formula = ExcelFormulas.Sum(rowIndex, startColumnIndex, rowIndex, startColumnIndex + numberOfYears - 1); ;
            }
            var lastContentRow = firstContentRow + workTypes.Count - 1;
            currentCell.Row += workTypes.Count();
            var totalSpentRow = currentCell.Row;
            worksheet.Cells[totalSpentRow, startColumnIndex + numberOfYears].Formula = ExcelFormulas.Sum(totalSpentRow, startColumnIndex, currentCell.Row, startColumnIndex + numberOfYears - 1); ;
            worksheet.Cells[totalSpentRow, 1].Value = "Total Spent";
            for (var columnIndex = 3; columnIndex < 3 + numberOfYears; columnIndex++)
            {
                var startAddress = worksheet.Cells[firstContentRow, columnIndex].Address;
                var endAddress = worksheet.Cells[lastContentRow, columnIndex].Address;
                worksheet.Cells[currentCell.Row, columnIndex].Formula = ExcelFormulas.RangeSum(startAddress, endAddress);
            }
            currentCell.Row += 2;
            var contentColor = Color.FromArgb(84, 130, 53);
            _excelHelper.ApplyBorder(worksheet.Cells[firstContentRow, 1, totalSpentRow, 3 + numberOfYears]);
            _excelHelper.SetCustomFormat(worksheet.Cells[firstContentRow, 3, currentCell.Row, 3 + numberOfYears], "NegativeCurrency");
            _excelHelper.ApplyColor(worksheet.Cells[firstContentRow, 3, totalSpentRow, 3 + numberOfYears - 1], contentColor);
            _excelHelper.SetTextColor(worksheet.Cells[firstContentRow, 3, currentCell.Row, 3 + numberOfYears - 1], Color.White);
            worksheet.Cells[currentCell.Row, 1].Value = "Total Bridge Care Budget";
            var totalColumnRange = worksheet.Cells[firstContentRow, 3 + numberOfYears, totalSpentRow, 3 + numberOfYears];
            _excelHelper.ApplyColor(totalColumnRange, Color.FromArgb(217, 217, 217));
            foreach (var year in simulationYears)
            {
                var yearIndex = year - simulationYears[0];
                var columnIndex = yearIndex + 3;
                var budgetTotal = yearlyBudgetAmount.Sum(x => x.Value.YearlyAmounts[yearIndex].Value);
                worksheet.Cells[currentCell.Row, columnIndex].Value = budgetTotal;
            }

            worksheet.Cells[currentCell.Row, startColumnIndex + numberOfYears].Formula = ExcelFormulas.Sum(currentCell.Row, startColumnIndex, currentCell.Row, startColumnIndex + numberOfYears - 1); ;
            var totalRowRange = worksheet.Cells[currentCell.Row, 3, currentCell.Row, 2 + numberOfYears];
            _excelHelper.ApplyColor(totalRowRange, Color.FromArgb(0, 128, 0));
            var grandTotalRange = worksheet.Cells[currentCell.Row, startColumnIndex + numberOfYears];
            _excelHelper.ApplyColor(grandTotalRange, Color.FromArgb(217, 217, 217));
            _excelHelper.ApplyBorder(totalRowRange);
            currentCell.Row++;
            return 666;
        }

        private int FillBpnSection(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears)
        {
            _bridgeWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Total Cost Per BPN", "BPN");
            currentCell.Row++;
            return currentCell.Row;
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
