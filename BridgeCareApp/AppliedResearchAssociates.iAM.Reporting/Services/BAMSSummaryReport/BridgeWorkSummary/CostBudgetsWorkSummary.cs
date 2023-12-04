using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OfficeOpenXml;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.Reporting.Models.BAMSSummaryReport;
using AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport.BridgeWorkSummary.StaticContent;
using AppliedResearchAssociates.iAM.ExcelHelpers;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using AppliedResearchAssociates.iAM.Reporting.Models;

namespace AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport.BridgeWorkSummary
{
    public class CostBudgetsWorkSummary
    {
        private BridgeWorkSummaryCommon _bridgeWorkSummaryCommon;
        private WorkSummaryModel _workSummaryModel;

        private Dictionary<int, decimal> TotalCulvertSpent = new Dictionary<int, decimal>();
        private Dictionary<int, decimal> TotalBridgeSpent = new Dictionary<int, decimal>();
        private Dictionary<int, decimal> TotalCommittedSpent = new Dictionary<int, decimal>();
        private Dictionary<int, decimal> TotalSAPSpent = new Dictionary<int, decimal>();
        private Dictionary<int, decimal> TotalProjectBuilderSpent = new Dictionary<int, decimal>();

        private int BridgeTotalRow = 0;
        private int CulvertTotalRow = 0;
        private int CommittedTotalRow = 0;

        public CostBudgetsWorkSummary(WorkSummaryModel workSummaryModel)
        {
            _bridgeWorkSummaryCommon = new BridgeWorkSummaryCommon();
            _workSummaryModel = workSummaryModel;
        }

        public void FillCostBudgetWorkSummarySections(
            ExcelWorksheet worksheet,
            CurrentCell currentCell,
            Dictionary<int, Dictionary<string, (decimal treatmentCost, int bridgeCount)>> costPerTreatmentPerYear,
            Dictionary<int, Dictionary<string, (decimal treatmentCost, int bridgeCount, string projectSource)>> yearlyCostCommittedProj,
            List<int> simulationYears,
            Dictionary<string, Budget> yearlyBudgetAmount,
            Dictionary<int, Dictionary<string, decimal>> bpnCostPerYear,
            List<(string Name, AssetCategories AssetType, TreatmentCategory Category)> simulationTreatments)
        {
            var localSimulationTreatments = new List<(string Name, AssetCategories AssetType, TreatmentCategory Category)>(simulationTreatments);
            localSimulationTreatments.Remove((BAMSConstants.CulvertNoTreatment, AssetCategories.Culvert, TreatmentCategory.Other));
            localSimulationTreatments.Remove((BAMSConstants.NonCulvertNoTreatment, AssetCategories.Bridge, TreatmentCategory.Other));

            var workTypeTotalMPMS = FillCostOfCommittedWorkSection(worksheet, currentCell, simulationYears, yearlyCostCommittedProj);
            var workTypeTotalSAP = FillCostOfSAPWorkSection(worksheet, currentCell, simulationYears, yearlyCostCommittedProj);
            var workTypeTotalProjectBuilder = FillCostOfProjectBuilderWorkSection(worksheet, currentCell, simulationYears, yearlyCostCommittedProj);

            var workTypeTotalCulvert = FillCostOfCulvertWorkSection(worksheet, currentCell,
                simulationYears, costPerTreatmentPerYear, localSimulationTreatments);
            var workTypeTotalBridge = FillCostOfBridgeWorkSection(worksheet, currentCell,
                simulationYears, costPerTreatmentPerYear, localSimulationTreatments);

            var workTypeTotalAggregated = new WorkTypeTotalAggregated
            {
                WorkTypeTotalCulvert = workTypeTotalCulvert,
                WorkTypeTotalBridge = workTypeTotalBridge,
                WorkTypeTotalMPMS = workTypeTotalMPMS,
                WorkTypeTotalSAP = workTypeTotalSAP,
                WorkTypeTotalProjectBuilder = workTypeTotalProjectBuilder
            };
            var workTypeTotalRow = FillWorkTypeTotalsSection(worksheet, currentCell, simulationYears,
                yearlyBudgetAmount,
                workTypeTotalAggregated);

            var bpnTotalRow = FillBpnSection(worksheet, currentCell, simulationYears, bpnCostPerYear);
            FillRemainingBudgetSection(worksheet, simulationYears, currentCell, workTypeTotalRow);
        }

        #region Private methods

        private Dictionary<TreatmentCategory, SortedDictionary<int, decimal>> FillCostOfCommittedWorkSection(ExcelWorksheet worksheet, CurrentCell currentCell,
            List<int> simulationYears,
            Dictionary<int, Dictionary<string, (decimal treatmentCost, int bridgeCount, string projectSource)>> yearlyCostCommittedProj)
        {
            var headerRange = new Range(currentCell.Row, currentCell.Row + 1);
            _bridgeWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Cost of MPMS Work", "MPMS Work Type");
            var workTypeTotalData = AddCostsOfCommittedWork(worksheet, simulationYears, currentCell, yearlyCostCommittedProj);
            return workTypeTotalData;
        }

        private Dictionary<TreatmentCategory, SortedDictionary<int, decimal>> FillCostOfSAPWorkSection(
            ExcelWorksheet worksheet,
            CurrentCell currentCell,
            List<int> simulationYears,
            Dictionary<int, Dictionary<string, (decimal treatmentCost, int bridgeCount, string projectSource)>> yearlyCostCommittedProj)
        {
            var headerRange = new Range(currentCell.Row, currentCell.Row + 1);
            _bridgeWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Cost of SAP Work", "SAP Work Type");
            var workTypeTotalDataSAP = AddCostsOfSAPWork(worksheet, simulationYears, currentCell, yearlyCostCommittedProj);
            return workTypeTotalDataSAP;
        }

        private Dictionary<TreatmentCategory, SortedDictionary<int, decimal>> FillCostOfProjectBuilderWorkSection(
            ExcelWorksheet worksheet,
            CurrentCell currentCell,
            List<int> simulationYears,
            Dictionary<int, Dictionary<string, (decimal treatmentCost, int bridgeCount, string projectSource)>> yearlyCostCommittedProj)
        {
            var headerRange = new Range(currentCell.Row, currentCell.Row + 1);
            _bridgeWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Cost of Project Builder Work", "Project Builder Work Type");
            var workTypeTotalDataProjectBuilder = AddCostsOfProjectBuilderWork(worksheet, simulationYears, currentCell, yearlyCostCommittedProj);
            return workTypeTotalDataProjectBuilder;
        }

        private Dictionary<TreatmentCategory, SortedDictionary<int, decimal>> FillCostOfCulvertWorkSection(ExcelWorksheet worksheet, CurrentCell currentCell,
            List<int> simulationYears,
            Dictionary<int, Dictionary<string, (decimal treatmentCost, int bridgeCount)>> costPerTreatmentPerYear,
            List<(string Name, AssetCategories AssetType, TreatmentCategory Category)> simulationTreatments)
        {
            var headerRange = new Range(currentCell.Row, currentCell.Row + 1);
            _bridgeWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Cost of BAMS Culvert Work", "BAMS Culvert Work Type");
            var workTypeTotalCulvert = AddCostsOfCulvertWork(worksheet, simulationYears, currentCell, costPerTreatmentPerYear, simulationTreatments);

            return workTypeTotalCulvert;
        }

        private Dictionary<TreatmentCategory, SortedDictionary<int, decimal>> FillCostOfBridgeWorkSection(ExcelWorksheet worksheet, CurrentCell currentCell,
            List<int> simulationYears,
            Dictionary<int, Dictionary<string, (decimal treatmentCost, int bridgeCount)>> costPerTreatmentPerYear,
            List<(string Name, AssetCategories AssetType, TreatmentCategory Category)> simulationTreatments)
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
            Dictionary<string, Budget> yearlyBudgetAmount,
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

                // For MPMS data
                AddWorkTypeTotalData(workTypeTotalAggregated.WorkTypeTotalMPMS, workType, worksheet, rowIndex);

                // For SAP data
                AddWorkTypeTotalData(workTypeTotalAggregated.WorkTypeTotalSAP, workType, worksheet, rowIndex);

                // For Project Builder data
                AddWorkTypeTotalData(workTypeTotalAggregated.WorkTypeTotalProjectBuilder, workType, worksheet, rowIndex);

                // For culvert data
                AddWorkTypeTotalData(workTypeTotalAggregated.WorkTypeTotalCulvert, workType, worksheet, rowIndex);

                // For non culvert data
                AddWorkTypeTotalData(workTypeTotalAggregated.WorkTypeTotalBridge, workType, worksheet, rowIndex);

                // This line fills up data for "Total (all years)"
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
            TreatmentCategory workType, ExcelWorksheet worksheet,
            int rowIndex
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
             int budgetTotalRow)
        {
            _bridgeWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Budget Analysis", "");
            worksheet.Cells[currentCell.Row, simulationYears.Count + 3].Value = "Total Remaining Budget(all years)";
            ExcelHelper.ApplyStyle(worksheet.Cells[currentCell.Row, simulationYears.Count + 3]);
            ExcelHelper.ApplyBorder(worksheet.Cells[currentCell.Row, simulationYears.Count + 3]);
            AddDetailsForRemainingBudget(worksheet, simulationYears, currentCell, budgetTotalRow);
        }

        private Dictionary<TreatmentCategory, SortedDictionary<int, decimal>> AddCostsOfCommittedWork(ExcelWorksheet worksheet, List<int> simulationYears, CurrentCell currentCell,
            Dictionary<int, Dictionary<string, (decimal treatmentCost, int bridgeCount, string projectSource)>> yearlyCostCommittedProj)
        {
            var workTypeTotalMPMS = new Dictionary<TreatmentCategory, SortedDictionary<int, decimal>>();
            if (simulationYears.Count <= 0)
            {
                return workTypeTotalMPMS;
            }
            var startYear = simulationYears[0];
            _bridgeWorkSummaryCommon.SetRowColumns(currentCell, out var startRow, out var startColumn, out var row, out var column);
            currentCell.Column = column;
            var committedTotalRow = 0;

            var uniqueTreatments = new Dictionary<string, int>();
            var costForTreatments = new Dictionary<string, decimal>();
            var map = WorkTypeMap.Map;
            // filling in the committed treatments in the excel TAB
            foreach (var yearlyItem in yearlyCostCommittedProj)
            {
                decimal committedTotalCost = 0;
                row = currentCell.Row;
                foreach (var data in yearlyItem.Value)
                {
                    // Check that the projectSource is neither 'Maintenance' nor 'ProjectBuilder'
                    if (data.Value.projectSource != "Maintenance" && data.Value.projectSource != "ProjectBuilder")
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

                        // setting up data for Work type totals
                        if (map.ContainsKey(data.Key))
                        {
                            var category = map[data.Key];
                            var treatmentCost = data.Value.treatmentCost;
                            var currYear = yearlyItem.Key;
                            FillWorkTypeTotalMPMS(workTypeTotalMPMS, category, simulationYears, currYear, treatmentCost);
                        }
                        else
                        {
                            var treatmentCost = data.Value.treatmentCost;
                            var currYear = yearlyItem.Key;
                            var category = TreatmentCategory.Other;
                            FillWorkTypeTotalMPMS(workTypeTotalMPMS, category, simulationYears, currYear, treatmentCost);

                        }
                    }
                }
                TotalCommittedSpent.Add(yearlyItem.Key, committedTotalCost);
            }

            column = currentCell.Column;
            worksheet.Cells[currentCell.Row, column].Value = BAMSConstants.CommittedTotal;
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

            CommittedTotalRow = committedTotalRow;
            return workTypeTotalMPMS;
        }

        private Dictionary<TreatmentCategory, SortedDictionary<int, decimal>> AddCostsOfSAPWork(
            ExcelWorksheet worksheet,
            List<int> simulationYears,
            CurrentCell currentCell,
            Dictionary<int, Dictionary<string, (decimal treatmentCost, int bridgeCount, string projectSource)>> yearlyCostSAPProj)
        {
            var workTypeTotalSAP = new Dictionary<TreatmentCategory, SortedDictionary<int, decimal>>();
            if (simulationYears.Count <= 0)
            {
                return workTypeTotalSAP;
            }

            var startYear = simulationYears[0];
            _bridgeWorkSummaryCommon.SetRowColumns(currentCell, out var startRow, out var startColumn, out var row, out var column);
            currentCell.Column = column;
            var sapTotalRow = 0;

            var uniqueTreatments = new Dictionary<string, int>();
            var costForTreatments = new Dictionary<string, decimal>();
            var map = WorkTypeMap.Map;

            foreach (var yearlyItem in yearlyCostSAPProj)
            {
                decimal sapTotalCost = 0;
                row = currentCell.Row; 

                foreach (var data in yearlyItem.Value)
                {
                    if (data.Value.projectSource == "Maintenance")
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
                        sapTotalCost += data.Value.treatmentCost;

                        if (map.ContainsKey(data.Key))
                        {
                            var category = map[data.Key];
                            var treatmentCost = data.Value.treatmentCost;
                            var currYear = yearlyItem.Key;
                            FillWorkTypeTotalSAP(workTypeTotalSAP, category, simulationYears, currYear, treatmentCost);
                        }
                        else
                        {
                            var treatmentCost = data.Value.treatmentCost;
                            var currYear = yearlyItem.Key;
                            var category = TreatmentCategory.Other;
                            FillWorkTypeTotalSAP(workTypeTotalSAP, category, simulationYears, currYear, treatmentCost);
                        }
                    }
                }

                TotalSAPSpent.Add(yearlyItem.Key, sapTotalCost);
            }

            column = currentCell.Column;
            worksheet.Cells[currentCell.Row, column].Value = "SAP Total"; 
            column++;
            var fromColumn = column + 1;

            foreach (var cost in TotalSAPSpent)
            {
                if (cost.Value == 0)
                {
                    worksheet.Cells[currentCell.Row, fromColumn++].Value = "-"; 
                }
                else
                {
                    worksheet.Cells[currentCell.Row, fromColumn++].Value = cost.Value;
                }
            }
            sapTotalRow = currentCell.Row;

            fromColumn = column + 1;

            var endColumn = startColumn + simulationYears.Count + 1; 

            ExcelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, sapTotalRow, endColumn]);
            ExcelHelper.SetCustomFormat(worksheet.Cells[startRow, fromColumn, sapTotalRow, endColumn], ExcelHelperCellFormat.NegativeCurrency); 
            ExcelHelper.ApplyColor(worksheet.Cells[startRow, fromColumn, sapTotalRow, endColumn], Color.FromArgb(198, 224, 180)); 

            ExcelHelper.ApplyColor(worksheet.Cells[sapTotalRow, fromColumn, sapTotalRow, endColumn], Color.FromArgb(84, 130, 53)); 
            ExcelHelper.SetTextColor(worksheet.Cells[sapTotalRow, fromColumn, sapTotalRow, endColumn], Color.White);

            _bridgeWorkSummaryCommon.UpdateCurrentCell(currentCell, sapTotalRow + 1, endColumn);

            return workTypeTotalSAP;
        }

        private Dictionary<TreatmentCategory, SortedDictionary<int, decimal>> AddCostsOfProjectBuilderWork(
                 ExcelWorksheet worksheet,
                 List<int> simulationYears,
                 CurrentCell currentCell,
                 Dictionary<int, Dictionary<string, (decimal treatmentCost, int bridgeCount, string projectSource)>> yearlyCostProjectBuilderProj)
        {
            var workTypeTotalProjectBuilder = new Dictionary<TreatmentCategory, SortedDictionary<int, decimal>>();
            if (simulationYears.Count <= 0)
            {
                return workTypeTotalProjectBuilder;
            }

            var startYear = simulationYears[0];
            _bridgeWorkSummaryCommon.SetRowColumns(currentCell, out var startRow, out var startColumn, out var row, out var column);
            currentCell.Column = column;
            var projectBuilderTotalRow = 0;

            var uniqueTreatments = new Dictionary<string, int>();
            var costForTreatments = new Dictionary<string, decimal>();
            var map = WorkTypeMap.Map;

            foreach (var yearlyItem in yearlyCostProjectBuilderProj)
            {
                decimal projectBuilderTotalCost = 0;
                row = currentCell.Row;

                foreach (var data in yearlyItem.Value)
                {
                    if (data.Value.projectSource == "ProjectBuilder")
                    {
                        if (!uniqueTreatments.ContainsKey(data.Key))
                        {
                            uniqueTreatments.Add(data.Key, currentCell.Row);
                            worksheet.Cells[row++, column].Value = data.Key;

                            var cellToEnterCost = yearlyItem.Key - startYear + 2;
                            worksheet.Cells[uniqueTreatments[data.Key], column + cellToEnterCost].Value = data.Value.treatmentCost;

                            costForTreatments.Add(data.Key, data.Value.treatmentCost);
                            currentCell.Row += 1;
                        }
                        else
                        {
                            var cellToEnterCost = yearlyItem.Key - startYear + 2;
                            worksheet.Cells[uniqueTreatments[data.Key], column + cellToEnterCost].Value = data.Value.treatmentCost;
                        }
                        projectBuilderTotalCost += data.Value.treatmentCost;

                        if (map.ContainsKey(data.Key))
                        {
                            var category = map[data.Key];
                            var treatmentCost = data.Value.treatmentCost;
                            var currYear = yearlyItem.Key;
                            FillWorkTypeTotalProjectBuilder(workTypeTotalProjectBuilder, category, simulationYears, currYear, treatmentCost);
                        }
                        else
                        {
                            var treatmentCost = data.Value.treatmentCost;
                            var currYear = yearlyItem.Key;
                            var category = TreatmentCategory.Other;
                            FillWorkTypeTotalProjectBuilder(workTypeTotalProjectBuilder, category, simulationYears, currYear, treatmentCost);
                        }
                    }
                }

                TotalProjectBuilderSpent.Add(yearlyItem.Key, projectBuilderTotalCost); 
            }

            column = currentCell.Column;
            worksheet.Cells[currentCell.Row, column].Value = "Project Builder Total"; 
            column++;
            var fromColumn = column + 1;

            foreach (var cost in TotalProjectBuilderSpent)
            {
                if (cost.Value == 0)
                {
                    worksheet.Cells[currentCell.Row, fromColumn++].Value = "-";
                }
                else
                {
                    worksheet.Cells[currentCell.Row, fromColumn++].Value = cost.Value;
                }
            }
            projectBuilderTotalRow = currentCell.Row;

            fromColumn = column + 1;

            var endColumn = startColumn + simulationYears.Count + 1; 

            ExcelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, projectBuilderTotalRow, endColumn]);
            ExcelHelper.SetCustomFormat(worksheet.Cells[startRow, fromColumn, projectBuilderTotalRow, endColumn], ExcelHelperCellFormat.NegativeCurrency);
            ExcelHelper.ApplyColor(worksheet.Cells[startRow, fromColumn, projectBuilderTotalRow, endColumn], Color.FromArgb(198, 224, 180));
            ExcelHelper.ApplyColor(worksheet.Cells[projectBuilderTotalRow, fromColumn, projectBuilderTotalRow, endColumn], Color.FromArgb(84, 130, 53));
            ExcelHelper.SetTextColor(worksheet.Cells[projectBuilderTotalRow, fromColumn, projectBuilderTotalRow, endColumn], Color.White);

            _bridgeWorkSummaryCommon.UpdateCurrentCell(currentCell, projectBuilderTotalRow + 1, endColumn);

            return workTypeTotalProjectBuilder;
        }

        private Dictionary<TreatmentCategory, SortedDictionary<int, decimal>> AddCostsOfCulvertWork(ExcelWorksheet worksheet,
            List<int> simulationYears, CurrentCell currentCell,
            Dictionary<int, Dictionary<string, (decimal treatmentCost, int bridgeCount)>> costPerTreatmentPerYear,
            List<(string Name, AssetCategories AssetType, TreatmentCategory Category)> simulationTreatments)
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

            worksheet.Cells[row++, column].Value = BAMSConstants.CulvertTotal;
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
                    if (treatment.AssetType == AssetCategories.Culvert &&
                        !treatment.Name.Contains(BAMSConstants.NoTreatment, StringComparison.OrdinalIgnoreCase))
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
            List<(string Name, AssetCategories AssetType, TreatmentCategory Category)> simulationTreatments)
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

            worksheet.Cells[row++, column].Value = BAMSConstants.BridgeTotal;
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
                    if (treatment.AssetType == AssetCategories.Bridge &&
                    !treatment.Name.Contains(BAMSConstants.NoTreatment, StringComparison.OrdinalIgnoreCase))
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

        private void AddDetailsForRemainingBudget(ExcelWorksheet worksheet, List<int> simulationYears, CurrentCell currentCell,
             int budgetTotalRow)
        {
            int startRow, startColumn, row, column;
            _bridgeWorkSummaryCommon.SetRowColumns(currentCell, out startRow, out startColumn, out row, out column);
            worksheet.Cells[row++, column].Value = BAMSConstants.RemainingBudget;
            worksheet.Cells[row++, column].Value = BAMSConstants.PercentBudgetSpentMPMS;
            worksheet.Cells[row++, column].Value = BAMSConstants.PercentBudgetSpentBAMS;
            column++;
            var fromColumn = column + 1;
            foreach (var year in simulationYears)
            {
                row = startRow;
                column = ++column;
                var totalSpent = Convert.ToDouble(worksheet.Cells[CulvertTotalRow, column].Value) +
                    Convert.ToDouble(worksheet.Cells[BridgeTotalRow, column].Value) +
                Convert.ToDouble(worksheet.Cells[CommittedTotalRow, column].Value);

                worksheet.Cells[row, column].Value = Convert.ToDouble(worksheet.Cells[budgetTotalRow, column].Value) - totalSpent;
                row++;

                if (totalSpent != 0)
                {
                    worksheet.Cells[row, column].Formula = worksheet.Cells[CommittedTotalRow, column] + "/" + totalSpent;
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

        private void FillWorkTypeTotalMPMS(Dictionary<TreatmentCategory, SortedDictionary<int, decimal>> workTypeTotalMPMS,
            TreatmentCategory category, List<int> simulationYears, int currYear, decimal treatmentCost)
        {
            if (!workTypeTotalMPMS.ContainsKey(category))
            {
                workTypeTotalMPMS.Add(category, new SortedDictionary<int, decimal>());
                foreach (var year in simulationYears)
                {
                    workTypeTotalMPMS[category].Add(year, 0);
                }
                workTypeTotalMPMS[category][currYear] += treatmentCost;
            }
            else
            {
                workTypeTotalMPMS[category][currYear] += treatmentCost;
            }
        }

        private void FillWorkTypeTotalSAP(Dictionary<TreatmentCategory, SortedDictionary<int, decimal>> workTypeTotalSAP,
            TreatmentCategory category, List<int> simulationYears, int currYear, decimal treatmentCost)
        {
            if (!workTypeTotalSAP.ContainsKey(category))
            {
                workTypeTotalSAP.Add(category, new SortedDictionary<int, decimal>());
                foreach (var year in simulationYears)
                {
                    workTypeTotalSAP[category].Add(year, 0);
                }
            }
            workTypeTotalSAP[category][currYear] += treatmentCost;
        }

        private void FillWorkTypeTotalProjectBuilder(Dictionary<TreatmentCategory, SortedDictionary<int, decimal>> workTypeTotalProjectBuilder,
            TreatmentCategory category, List<int> simulationYears, int currYear, decimal treatmentCost)
        {

            if (!workTypeTotalProjectBuilder.ContainsKey(category))
            {
                workTypeTotalProjectBuilder.Add(category, new SortedDictionary<int, decimal>());
                foreach (var year in simulationYears)
                {
                    workTypeTotalProjectBuilder[category].Add(year, 0); 
                }
            }
            workTypeTotalProjectBuilder[category][currYear] += treatmentCost;
        }
    }

    #endregion Private methods
}
