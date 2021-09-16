using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using AppliedResearchAssociates.iAM.Domains;
using BridgeCareCore.Models.SummaryReport;
using BridgeCareCore.Services.SummaryReport.BridgeWorkSummary.StaticContent;
using OfficeOpenXml;
using static AppliedResearchAssociates.iAM.Domains.SelectableTreatment;

namespace BridgeCareCore.Services.SummaryReport.BridgeWorkSummary
{
    public class CostBudgetsWorkSummary
    {
        private readonly BridgeWorkSummaryCommon _bridgeWorkSummaryCommon;
        private Dictionary<int, decimal> TotalCulvertSpent = new Dictionary<int, decimal>();
        private Dictionary<int, decimal> TotalBridgeSpent = new Dictionary<int, decimal>();
        private Dictionary<int, decimal> TotalCommittedSpent = new Dictionary<int, decimal>();
        private readonly WorkSummaryModel _workSummaryModel;
        private int BridgeTotalRow = 0;
        private int CulvertTotalRow = 0;

        public CostBudgetsWorkSummary(BridgeWorkSummaryCommon bridgeWorkSummaryCommon,
            WorkSummaryModel workSummaryModel)
        {
            _bridgeWorkSummaryCommon = bridgeWorkSummaryCommon;
            _workSummaryModel = workSummaryModel ?? throw new ArgumentNullException(nameof(workSummaryModel));
        }

        public void FillCostBudgetWorkSummarySections(ExcelWorksheet worksheet, CurrentCell currentCell,
            Dictionary<int, Dictionary<string, (decimal treatmentCost, int bridgeCount)>> costPerTreatmentPerYear,
            Dictionary<int, Dictionary<string, (decimal treatmentCost, int bridgeCount)>> yearlyCostCommittedProj, List<int> simulationYears, Dictionary<string, Budget> yearlyBudgetAmount,
            Dictionary<int, Dictionary<string, decimal>> bpnCostPerYear,
            List<(string Name, AssetType AssetType, TreatmentCategory Category)> simulationTreatments)
        {
            var localSimulationTreatments = new List<(string Name, AssetType AssetType, TreatmentCategory Category)>(simulationTreatments);
            localSimulationTreatments.Remove((Properties.Resources.CulvertNoTreatment, AssetType.Culvert, TreatmentCategory.Other));
            localSimulationTreatments.Remove((Properties.Resources.NonCulvertNoTreatment, AssetType.Bridge, TreatmentCategory.Other));

            var committedTotalRange = FillCostOfCommittedWorkSection(worksheet, currentCell, simulationYears, yearlyCostCommittedProj);

            var workTypeTotalCulvert = FillCostOfCulvertWorkSection(worksheet, currentCell,
                simulationYears, costPerTreatmentPerYear, localSimulationTreatments);
            var workTypeTotalBridge = FillCostOfBridgeWorkSection(worksheet, currentCell,
                simulationYears, costPerTreatmentPerYear, localSimulationTreatments);

            var committedTotalRow = committedTotalRange.FooterRange.End.Value;

            var workTypeTotalAggregated = new WorkTypeTotalAggregated
            {
                WorkTypeTotalCulvert = workTypeTotalCulvert,
                WorkTypeTotalBridge = workTypeTotalBridge
            };
            var map = WorkTypeMap.Map;
            var workTypeTotalRow = FillWorkTypeTotalsSection(worksheet, currentCell, simulationYears, map,
                committedTotalRange.ContentRange, yearlyBudgetAmount, localSimulationTreatments,
                workTypeTotalAggregated);

            var bpnTotalRow = FillBpnSection(worksheet, currentCell, simulationYears, bpnCostPerYear);
            FillRemainingBudgetSection(worksheet, simulationYears, currentCell, workTypeTotalRow, committedTotalRow);
        }

        #region Private methods

        private ExcelTableRowRanges FillCostOfCommittedWorkSection(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears,
            Dictionary<int, Dictionary<string, (decimal treatmentCost, int bridgeCount)>> yearlyCostCommittedProj)
        {
            var headerRange = new Range(currentCell.Row, currentCell.Row + 1);
            _bridgeWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Cost of MPMS Work", "MPMS Work Type");
            var committedTotalRow = AddCostsOfCommittedWork(worksheet, simulationYears, currentCell, yearlyCostCommittedProj);

            var contentRange = new Range(headerRange.End.Value + 1, committedTotalRow - 1);
            var footerRange = new Range(committedTotalRow, committedTotalRow);
            return new ExcelTableRowRanges
            {
                HeaderRange = headerRange,
                ContentRange = contentRange,
                FooterRange = footerRange,
            };
        }

        private Dictionary<TreatmentCategory, SortedDictionary<int, decimal>> FillCostOfCulvertWorkSection(ExcelWorksheet worksheet, CurrentCell currentCell,
            List<int> simulationYears,
            Dictionary<int, Dictionary<string, (decimal treatmentCost, int bridgeCount)>> costPerTreatmentPerYear,
            List<(string Name, AssetType AssetType, TreatmentCategory Category)> simulationTreatments)
        {
            var headerRange = new Range(currentCell.Row, currentCell.Row + 1);
            _bridgeWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Cost of BAMS Culvert Work", "BAMS Culvert Work Type");
            var workTypeTotalCulvert = AddCostsOfCulvertWork(worksheet, simulationYears, currentCell, costPerTreatmentPerYear, simulationTreatments);

            return workTypeTotalCulvert;
        }

        private Dictionary<TreatmentCategory, SortedDictionary<int, decimal>> FillCostOfBridgeWorkSection(ExcelWorksheet worksheet, CurrentCell currentCell,
            List<int> simulationYears,
            Dictionary<int, Dictionary<string, (decimal treatmentCost, int bridgeCount)>> costPerTreatmentPerYear,
            List<(string Name, AssetType AssetType, TreatmentCategory Category)> simulationTreatments)
        {
            var headerRange = new Range(currentCell.Row, currentCell.Row + 1);
            _bridgeWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Cost of BAMS Bridge Work", "BAMS Bridge Work Type");
            var workTypeTotalBridge = AddCostsOfBridgeWork(worksheet, simulationYears, currentCell, costPerTreatmentPerYear, simulationTreatments);

            return workTypeTotalBridge;
        }

        private int FillWorkTypeTotalsSection(
            ExcelWorksheet worksheet,
            CurrentCell currentCell,
            List<int> simulationYears,
            Dictionary<string, TreatmentCategory> workTypeMap,
            Range committedRangeForSummands,
            Dictionary<string, Budget> yearlyBudgetAmount,
            List<(string Name, AssetType AssetType, TreatmentCategory Category)> simulationTreatments,
            WorkTypeTotalAggregated workTypeTotalAggregated)
        {
            _bridgeWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "", "BAMS Work Type Totals");
            var initialRow = currentCell.Row;
            currentCell.Row++;
            var workTypes = EnumExtensions.GetValues<TreatmentCategory>();
            var numberOfYears = simulationYears.Count;
            worksheet.Cells[initialRow, 3 + numberOfYears].Value = "Total (all years)";
            var totalColumnHeaderRange = worksheet.Cells[initialRow, 3 + numberOfYears];
            ExcelHelper.ApplyBorder(totalColumnHeaderRange);
            ExcelHelper.ApplyStyle(totalColumnHeaderRange);

            var startColumnIndex = 3;
            var firstContentRow = currentCell.Row;
            for (var workType = workTypes[0]; workType <= workTypes.Last(); workType++)
            {
                var rowIndex = firstContentRow + (int)workType;
                worksheet.Cells[rowIndex, 1].Value = workType.ToSpreadsheetString();
                for (var columnIndex = startColumnIndex; columnIndex < startColumnIndex + numberOfYears; columnIndex++)
                {
                    var cellsToSum = new List<ExcelRange>();
                    // for committed project data
                    AddWorkTypeTotalForCommmitted(committedRangeForSummands, cellsToSum, workType, worksheet, workTypeMap, rowIndex, columnIndex);
                }

                // For culvert data
                AddWorkTypeTotalData(workTypeTotalAggregated.WorkTypeTotalCulvert, workType, worksheet, rowIndex, simulationTreatments);

                // For non culvert data
                AddWorkTypeTotalData(workTypeTotalAggregated.WorkTypeTotalBridge, workType, worksheet, rowIndex, simulationTreatments);

                worksheet.Cells[rowIndex, startColumnIndex + numberOfYears].Formula = ExcelFormulas.Sum(rowIndex, startColumnIndex, rowIndex, startColumnIndex + numberOfYears - 1);
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

            // Adding percentage after the Total (all years)
            for (var workType = workTypes[0]; workType <= workTypes.Last(); workType++)
            {
                var rowIndex = firstContentRow + (int)workType;
                var col = startColumnIndex + numberOfYears + 1;
                worksheet.Cells[rowIndex, col].Formula = ExcelFormulas.Percentage(rowIndex, col - 1, totalSpentRow, col - 1);

                worksheet.Cells[rowIndex, col + 1].Value = $"Percentage Spent on {workType.ToSpreadsheetString().ToUpper()}";
            }
            currentCell.Row += 2;
            var contentColor = Color.FromArgb(84, 130, 53);
            ExcelHelper.ApplyBorder(worksheet.Cells[firstContentRow, 1, totalSpentRow, 3 + numberOfYears]);
            ExcelHelper.SetCustomFormat(worksheet.Cells[firstContentRow, 3, currentCell.Row, 3 + numberOfYears], ExcelHelperCellFormat.NegativeCurrency);
            ExcelHelper.ApplyColor(worksheet.Cells[firstContentRow, 3, totalSpentRow, 3 + numberOfYears - 1], contentColor);
            ExcelHelper.SetTextColor(worksheet.Cells[firstContentRow, 3, currentCell.Row, 3 + numberOfYears - 1], Color.White);

            // Style for percentages
            var percentColumn = startColumnIndex + numberOfYears + 1;
            ExcelHelper.SetCustomFormat(worksheet.Cells[firstContentRow, percentColumn, totalSpentRow, percentColumn], ExcelHelperCellFormat.Percentage);
            ExcelHelper.HorizontalCenterAlign(worksheet.Cells[firstContentRow, percentColumn, totalSpentRow, percentColumn]);

            worksheet.Cells[currentCell.Row, 1].Value = "Total Bridge Care Budget";
            var totalColumnRange = worksheet.Cells[firstContentRow, 3 + numberOfYears, totalSpentRow, 3 + numberOfYears];
            ExcelHelper.ApplyColor(totalColumnRange, Color.FromArgb(217, 217, 217));

            decimal averageAnnualBudget = 0;
            foreach (var year in simulationYears)
            {
                var yearIndex = year - simulationYears[0];
                var columnIndex = yearIndex + 3;
                var budgetTotal = yearlyBudgetAmount.Sum(x => x.Value.YearlyAmounts[yearIndex].Value);
                worksheet.Cells[currentCell.Row, columnIndex].Value = budgetTotal;
                averageAnnualBudget += budgetTotal;
            }

            worksheet.Cells[currentCell.Row, startColumnIndex + numberOfYears].Formula = ExcelFormulas.Sum(currentCell.Row, startColumnIndex, currentCell.Row, startColumnIndex + numberOfYears - 1);

            // AnnualizedAmount is used to fill the "Annualized Amount" row of Bridge Work Summary
            _workSummaryModel.AnnualizedAmount = (averageAnnualBudget / simulationYears.Count);

            var totalRowRange = worksheet.Cells[currentCell.Row, 3, currentCell.Row, 2 + numberOfYears];
            ExcelHelper.ApplyColor(totalRowRange, Color.FromArgb(0, 128, 0));
            var grandTotalRange = worksheet.Cells[currentCell.Row, startColumnIndex + numberOfYears];
            ExcelHelper.ApplyColor(grandTotalRange, Color.FromArgb(217, 217, 217));
            ExcelHelper.ApplyBorder(totalRowRange);
            currentCell.Row++;
            return currentCell.Row - 1;
        }

        private void AddWorkTypeTotalData(Dictionary<TreatmentCategory, SortedDictionary<int, decimal>> workTypeTotal,
            //List<ExcelRange> cellsToSum,
            TreatmentCategory workType, ExcelWorksheet worksheet,
            int rowIndex,
            //int columnIndex,
            List<(string Name, AssetType AssetType, TreatmentCategory Category)> simulationTreatments
            )
        {
            workTypeTotal.TryGetValue(workType, out SortedDictionary<int, decimal> yearAndAmount);
            var col = 3;
            var i = 0;
            if (yearAndAmount == null)
            {
                return;
            }
            foreach (var item in yearAndAmount)
            {
                if (worksheet.Cells[rowIndex, col + i].Value == null)
                {
                    worksheet.Cells[rowIndex, col + i].Value = 0.0;
                };
                var temp = Convert.ToDouble(worksheet.Cells[rowIndex, col + i].Value);
                temp += (double)item.Value;
                worksheet.Cells[rowIndex, col + i].Value = temp;
                i++;
            }
        }

        private void AddWorkTypeTotalForCommmitted(Range rangeForSummands,
            List<ExcelRange> cellsToSum,
            TreatmentCategory workType, ExcelWorksheet worksheet, Dictionary<string, TreatmentCategory> workTypeMap,
            int rowIndex, int columnIndex)
        {
            for (var summandRowIndex = rangeForSummands.Start.Value; summandRowIndex <= rangeForSummands.End.Value; summandRowIndex++)
            {
                var summandRowTitle = worksheet.Cells[summandRowIndex, 1].Value?.ToString();
                if (summandRowTitle != null)
                {
                    var summandWorkType = workTypeMap.ContainsKey(summandRowTitle) ? workTypeMap[summandRowTitle] : TreatmentCategory.Other;
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

        private int FillBpnSection(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears,
            Dictionary<int, Dictionary<string, decimal>> bpnCostPerYear)
        {
            // Add budget category headers & year columns
            _bridgeWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Total Cost Per BPN", "BPN");
            currentCell.Row++;
            var bpnCostBudgetNames = EnumExtensions.GetValues<BPNCostBudgetName>();

            // Add row labels
            var firstContentRow = currentCell.Row;
            for (var bpnCostBudgetName = bpnCostBudgetNames[0]; bpnCostBudgetName <= bpnCostBudgetNames.Last(); bpnCostBudgetName++)
            {
                var rowIndex = firstContentRow + (int)bpnCostBudgetName;
                worksheet.Cells[rowIndex, 1].Value = bpnCostBudgetName.ToSpreadsheetString();
            }
            worksheet.Cells[firstContentRow + bpnCostBudgetNames.Count, 1].Value = "Total BPN Cost";

            // Add cost data
            var startColumnIndex = 3;
            var totalCostPerYear = new Dictionary<int, decimal>();
            for (var i = 0; i < simulationYears.Count; i++)
            {
                decimal totalCost = 0;
                for (var bpnCostBudgetName = bpnCostBudgetNames[0]; bpnCostBudgetName <= bpnCostBudgetNames.Last(); bpnCostBudgetName++)
                {
                    var cost = GetCostForBPNCostBudgetName(bpnCostPerYear, simulationYears[i], bpnCostBudgetName);
                    totalCost += cost;
                    var rowIndex = firstContentRow + (int)bpnCostBudgetName;
                    worksheet.Cells[rowIndex, startColumnIndex + i].Value = cost;
                    ExcelHelper.SetCurrencyFormat(worksheet.Cells[rowIndex, startColumnIndex + i]);
                }
                totalCostPerYear.Add(simulationYears[i], totalCost);
            }

            // Set formatting for cost rows
            var r = 0;
            foreach (var item in totalCostPerYear)
            {
                worksheet.Cells[firstContentRow + bpnCostBudgetNames.Count, startColumnIndex + r].Value = item.Value;
                ExcelHelper.SetCurrencyFormat(worksheet.Cells[firstContentRow + bpnCostBudgetNames.Count, startColumnIndex + r]);
                r++;
            }
            var numberOfYears = simulationYears.Count;
            var totalRowRange = worksheet.Cells[currentCell.Row, 1, currentCell.Row + bpnCostBudgetNames.Count, 2 + numberOfYears];
            ExcelHelper.ApplyBorder(totalRowRange);
            var rowColorRange = worksheet.Cells[currentCell.Row, startColumnIndex, currentCell.Row + bpnCostBudgetNames.Count, startColumnIndex + numberOfYears - 1];
            ExcelHelper.ApplyColor(rowColorRange, Color.FromArgb(255, 230, 153));

            // Return index of next row
            currentCell.Row += bpnCostBudgetNames.Count() + 1;
            return currentCell.Row;
        }

        private decimal GetCostForBPNCostBudgetName(Dictionary<int, Dictionary<string, decimal>> bpnInfoPerYear, int year, BPNCostBudgetName bpnValue)
        {
            decimal cost = 0;
            if (bpnValue == BPNCostBudgetName.BPNOther)
            {
                cost = bpnInfoPerYear[year].Where(_ => _.Key.IsBpnOther()).Sum(_ => _.Value);
            }
            else
            {
                if (bpnInfoPerYear[year].ContainsKey(bpnValue.ToMatchInDictionaryString()))
                {
                    cost = bpnInfoPerYear[year][bpnValue.ToMatchInDictionaryString()];
                }
            }
            return cost;
        }

        private void FillRemainingBudgetSection(ExcelWorksheet worksheet, List<int> simulationYears, CurrentCell currentCell,
             int budgetTotalRow, int committedTotalRow)
        {
            _bridgeWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Budget Analysis", "");
            worksheet.Cells[currentCell.Row, simulationYears.Count + 3].Value = "Total Remaining Budget(all years)";
            ExcelHelper.ApplyStyle(worksheet.Cells[currentCell.Row, simulationYears.Count + 3]);
            ExcelHelper.ApplyBorder(worksheet.Cells[currentCell.Row, simulationYears.Count + 3]);
            AddDetailsForRemainingBudget(worksheet, simulationYears, currentCell, budgetTotalRow, committedTotalRow);
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

            ExcelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, committedTotalRow, endColumn]);
            ExcelHelper.SetCustomFormat(worksheet.Cells[startRow, fromColumn, committedTotalRow, endColumn], ExcelHelperCellFormat.NegativeCurrency);
            ExcelHelper.ApplyColor(worksheet.Cells[startRow, fromColumn, committedTotalRow, endColumn], Color.FromArgb(198, 224, 180));

            ExcelHelper.ApplyColor(worksheet.Cells[committedTotalRow, fromColumn, committedTotalRow, endColumn], Color.FromArgb(84, 130, 53));
            ExcelHelper.SetTextColor(worksheet.Cells[committedTotalRow, fromColumn, committedTotalRow, endColumn], Color.White);
            _bridgeWorkSummaryCommon.UpdateCurrentCell(currentCell, committedTotalRow + 1, endColumn);

            return committedTotalRow;
        }

        private Dictionary<TreatmentCategory, SortedDictionary<int, decimal>> AddCostsOfCulvertWork(ExcelWorksheet worksheet,
            List<int> simulationYears, CurrentCell currentCell,
            Dictionary<int, Dictionary<string, (decimal treatmentCost, int bridgeCount)>> costPerTreatmentPerYear,
            List<(string Name, AssetType AssetType, TreatmentCategory Category)> simulationTreatments)
        {
            var workTypeTotalCulvert = new Dictionary<TreatmentCategory, SortedDictionary<int, decimal>>();
            if (simulationYears.Count <= 0)
            {
                return workTypeTotalCulvert;
            }
            int startRow, startColumn, row, column;
            _bridgeWorkSummaryCommon.SetRowColumns(currentCell, out startRow, out startColumn, out row, out column);
            int culvertTotalRow = 0;

            // filling in the culvert treatments in the excel TAB
            _bridgeWorkSummaryCommon.SetCulvertSectionExcelString(worksheet, simulationTreatments, ref row, ref column);

            worksheet.Cells[row++, column].Value = Properties.Resources.CulvertTotal;
            column++;
            var fromColumn = column + 1;
            // Filling in the cost per treatment per year in the excel TAB
            foreach (var yearlyValues in costPerTreatmentPerYear)
            {
                decimal culvertTotalCost = 0;
                row = startRow;
                column = ++column;

                foreach (var treatment in simulationTreatments)
                {
                    decimal cost = 0;
                    if (treatment.AssetType == AssetType.Culvert &&
                        !treatment.Name.Contains(Properties.Resources.NoTreatment, StringComparison.OrdinalIgnoreCase))
                    {
                        yearlyValues.Value.TryGetValue(treatment.Name, out var culvertCostAndCount);
                        worksheet.Cells[row++, column].Value = culvertCostAndCount.treatmentCost;
                        culvertTotalCost += culvertCostAndCount.treatmentCost;
                        cost = culvertCostAndCount.treatmentCost;

                        if (!workTypeTotalCulvert.ContainsKey(treatment.Category))
                        {
                            workTypeTotalCulvert.Add(treatment.Category, new SortedDictionary<int, decimal>()
                        {
                            { yearlyValues.Key, cost }
                        });
                        }
                        else
                        {
                            if (!workTypeTotalCulvert[treatment.Category].ContainsKey(yearlyValues.Key))
                            {
                                workTypeTotalCulvert[treatment.Category].Add(yearlyValues.Key, 0);
                            }
                            workTypeTotalCulvert[treatment.Category][yearlyValues.Key] += cost;
                        }
                    }
                }
                worksheet.Cells[row, column].Value = culvertTotalCost;
                culvertTotalRow = row;
                TotalCulvertSpent.Add(yearlyValues.Key, culvertTotalCost);
            }
            ExcelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row, column]);
            ExcelHelper.SetCustomFormat(worksheet.Cells[startRow, fromColumn, row, column], ExcelHelperCellFormat.NegativeCurrency);
            ExcelHelper.ApplyColor(worksheet.Cells[startRow, fromColumn, row, column], Color.FromArgb(198, 224, 180));

            ExcelHelper.ApplyColor(worksheet.Cells[culvertTotalRow, fromColumn, culvertTotalRow, column], Color.FromArgb(84, 130, 53));
            ExcelHelper.SetTextColor(worksheet.Cells[culvertTotalRow, fromColumn, culvertTotalRow, column], Color.White);
            _bridgeWorkSummaryCommon.UpdateCurrentCell(currentCell, ++row, column);

            CulvertTotalRow = culvertTotalRow;
            return workTypeTotalCulvert;
        }

        private Dictionary<TreatmentCategory, SortedDictionary<int, decimal>> AddCostsOfBridgeWork(ExcelWorksheet worksheet,
            List<int> simulationYears, CurrentCell currentCell,
            Dictionary<int, Dictionary<string, (decimal treatmentCost, int bridgeCount)>> costPerTreatmentPerYear,
            List<(string Name, AssetType AssetType, TreatmentCategory Category)> simulationTreatments)
        {
            var workTypeTotalBridge = new Dictionary<TreatmentCategory, SortedDictionary<int, decimal>>();
            if (simulationYears.Count <= 0)
            {
                return workTypeTotalBridge;
            }
            int startRow, startColumn, row, column;
            _bridgeWorkSummaryCommon.SetRowColumns(currentCell, out startRow, out startColumn, out row, out column);
            int bridgeTotalRow = 0;
            _bridgeWorkSummaryCommon.SetNonCulvertSectionExcelString(worksheet, simulationTreatments, ref row, ref column);

            worksheet.Cells[row++, column].Value = Properties.Resources.BridgeTotal;
            column++;
            var fromColumn = column + 1;
            // Filling in the cost per treatment per year in the excel TAB
            foreach (var yearlyValues in costPerTreatmentPerYear)
            {
                row = startRow;
                column = ++column;
                decimal nonCulvertTotalCost = 0;

                foreach (var treatment in simulationTreatments)
                {
                    decimal cost = 0;
                    if (treatment.AssetType == AssetType.Bridge &&
                    !treatment.Name.Contains(Properties.Resources.NoTreatment, StringComparison.OrdinalIgnoreCase))
                    {
                        yearlyValues.Value.TryGetValue(treatment.Name, out var nonCulvertCostAndCount);
                        worksheet.Cells[row++, column].Value = nonCulvertCostAndCount.treatmentCost;
                        nonCulvertTotalCost += nonCulvertCostAndCount.treatmentCost;
                        cost = nonCulvertCostAndCount.treatmentCost;

                        if (!workTypeTotalBridge.ContainsKey(treatment.Category))
                        {
                            workTypeTotalBridge.Add(treatment.Category, new SortedDictionary<int, decimal>()
                        {
                            { yearlyValues.Key, cost }
                        });
                        }
                        else
                        {
                            if (!workTypeTotalBridge[treatment.Category].ContainsKey(yearlyValues.Key))
                            {
                                workTypeTotalBridge[treatment.Category].Add(yearlyValues.Key, 0);
                            }
                            workTypeTotalBridge[treatment.Category][yearlyValues.Key] += cost;
                        }
                    }
                }

                worksheet.Cells[row, column].Value = nonCulvertTotalCost;
                bridgeTotalRow = row;
                TotalBridgeSpent.Add(yearlyValues.Key, nonCulvertTotalCost);
            }
            ExcelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row, column]);
            ExcelHelper.SetCustomFormat(worksheet.Cells[startRow, fromColumn, row, column], ExcelHelperCellFormat.NegativeCurrency);
            ExcelHelper.ApplyColor(worksheet.Cells[startRow, fromColumn, row, column], Color.FromArgb(198, 224, 180));

            ExcelHelper.ApplyColor(worksheet.Cells[bridgeTotalRow, fromColumn, bridgeTotalRow, column], Color.FromArgb(84, 130, 53));
            ExcelHelper.SetTextColor(worksheet.Cells[bridgeTotalRow, fromColumn, bridgeTotalRow, column], Color.White);
            _bridgeWorkSummaryCommon.UpdateCurrentCell(currentCell, ++row, column);
            BridgeTotalRow = bridgeTotalRow;
            return workTypeTotalBridge;
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

            ExcelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row, column + 1]);
            ExcelHelper.SetCustomFormat(worksheet.Cells[startRow, fromColumn, row, column + 1], ExcelHelperCellFormat.NegativeCurrency);
            ExcelHelper.ApplyColor(worksheet.Cells[startRow, fromColumn, row, column], Color.FromArgb(84, 130, 53));
            ExcelHelper.SetTextColor(worksheet.Cells[startRow, fromColumn, row, column], Color.White);
            _bridgeWorkSummaryCommon.UpdateCurrentCell(currentCell, ++row, column);
            return budgetTotalRow;
        }

        private void AddDetailsForRemainingBudget(ExcelWorksheet worksheet, List<int> simulationYears, CurrentCell currentCell,
             int budgetTotalRow, int committedTotalRow)
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
                var totalSpent = Convert.ToDouble(worksheet.Cells[CulvertTotalRow, column].Value) +
                    Convert.ToDouble(worksheet.Cells[BridgeTotalRow, column].Value);
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
            if (_workSummaryModel.AnnualizedAmount != 0)
            {
                worksheet.Cells[startRow, column + 2].Formula = worksheet.Cells[startRow, column + 1] + "/"
                    + (_workSummaryModel.AnnualizedAmount * simulationYears.Count);
            }

            worksheet.Cells[startRow, column + 2].Style.Numberformat.Format = "#0.00%";
            worksheet.Cells[startRow, column + 3].Value = "Percentage of Total Budget that was Unspent";

            ExcelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row, column + 1]);

            ExcelHelper.SetCustomFormat(worksheet.Cells[row - 2, fromColumn, row - 2, column + 1], ExcelHelperCellFormat.NegativeCurrency);
            ExcelHelper.SetCustomFormat(worksheet.Cells[row - 1, fromColumn, row, column], ExcelHelperCellFormat.Percentage);

            ExcelHelper.ApplyColor(worksheet.Cells[startRow, fromColumn, startRow, column], Color.Red);
            ExcelHelper.ApplyColor(worksheet.Cells[startRow + 1, fromColumn, row, column], Color.FromArgb(248, 203, 173));
            _bridgeWorkSummaryCommon.UpdateCurrentCell(currentCell, row + 3, column);
            ExcelHelper.ApplyColor(worksheet.Cells[row + 2, startColumn, row + 2, column], Color.DimGray);
        }
    }

    #endregion Private methods
}
