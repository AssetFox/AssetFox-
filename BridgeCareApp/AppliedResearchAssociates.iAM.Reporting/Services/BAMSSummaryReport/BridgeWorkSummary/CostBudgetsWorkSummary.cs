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
using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport.BridgeWorkSummary
{
    public class CostBudgetsWorkSummary
    {
        private BridgeWorkSummaryCommon _bridgeWorkSummaryCommon;
        private WorkSummaryModel _workSummaryModel;

        private Dictionary<int, decimal> TotalCulvertSpent = new();
        private Dictionary<int, decimal> TotalBridgeSpent = new();
        private Dictionary<int, decimal> TotalCommittedSpent = new();
        private Dictionary<int, decimal> TotalMPMSSpent = new();
        private Dictionary<int, decimal> TotalSAPSpent = new();
        private Dictionary<int, decimal> TotalProjectBuilderSpent = new();
                     
        private int TotalSpentRow = 0;
        private bool ShouldBundleFeasibleTreatments;

        public CostBudgetsWorkSummary(WorkSummaryModel workSummaryModel)
        {
            _bridgeWorkSummaryCommon = new BridgeWorkSummaryCommon();
            _workSummaryModel = workSummaryModel;
        }

        public void FillCostBudgetWorkSummarySections(
            ExcelWorksheet worksheet,
            CurrentCell currentCell,
            Dictionary<int, Dictionary<string, (decimal treatmentCost, int bridgeCount)>> costPerTreatmentPerYear,
            Dictionary<int, Dictionary<string, (decimal treatmentCost, int bridgeCount, string projectSource, string treatmentCategory)>> yearlyCostCommittedProjects,
            List<int> simulationYears,
            Dictionary<string, Budget> yearlyBudgetAmount,
            Dictionary<int, Dictionary<string, decimal>> bpnCostPerYear,
            List<(string Name, string AssetType, TreatmentCategory Category)> simulationTreatments,
            List<BaseCommittedProjectDTO> committedProjectsForWorkOutsideScope,
            bool shouldBundleFeasibleTreatments)
        {
            ShouldBundleFeasibleTreatments = shouldBundleFeasibleTreatments;
            var localSimulationTreatments = new List<(string Name, string AssetType, TreatmentCategory Category)>(simulationTreatments);
            localSimulationTreatments.Remove((BAMSConstants.CulvertNoTreatment, "Culvert", TreatmentCategory.Other));
            localSimulationTreatments.Remove((BAMSConstants.NonCulvertNoTreatment, "Bridge", TreatmentCategory.Other));
                        
            var workTypeTotalCommitted = FillCostOfCommittedWorkSection(worksheet, currentCell, simulationYears, yearlyCostCommittedProjects);
            var workTypeTotalMPMS = FillCostOfMPMSWorkSection(worksheet, currentCell, simulationYears, yearlyCostCommittedProjects);
            var workTypeTotalSAP = FillCostOfSAPWorkSection(worksheet, currentCell, simulationYears, yearlyCostCommittedProjects);
            var workTypeTotalProjectBuilder = FillCostOfProjectBuilderWorkSection(worksheet, currentCell, simulationYears, yearlyCostCommittedProjects);

            var workTypeTotalCulvert = FillCostOfCulvertWorkSection(worksheet, currentCell,
                simulationYears, costPerTreatmentPerYear, localSimulationTreatments);
            var WorkTypeTotalBundled = new Dictionary<TreatmentCategory, SortedDictionary<int, decimal>>();
            var workTypeTotalBridge = FillCostOfBridgeWorkSection(worksheet, currentCell,
                simulationYears, costPerTreatmentPerYear, localSimulationTreatments, WorkTypeTotalBundled);            
            var workTypeTotalWorkOutsideScope = AddCostOfWorkOutsideScope(committedProjectsForWorkOutsideScope);
            
            var workTypeTotalAggregated = new WorkTypeTotalAggregated
            {
                WorkTypeTotalCulvert = workTypeTotalCulvert,
                WorkTypeTotalBridge = workTypeTotalBridge,
                WorkTypeTotalMPMS = workTypeTotalMPMS,
                WorkTypeTotalSAP = workTypeTotalSAP,
                WorkTypeTotalProjectBuilder = workTypeTotalProjectBuilder,
                WorkTypeTotalWorkOutsideScope = workTypeTotalWorkOutsideScope,
                WorkTypeTotalBundled = WorkTypeTotalBundled,
                WorkTypeTotalCommitted = workTypeTotalCommitted
            };
            
            var workTypeTotalRow = FillWorkTypeTotalsSection(worksheet, currentCell, simulationYears, yearlyBudgetAmount, workTypeTotalAggregated, shouldBundleFeasibleTreatments);

            var bpnTotalRow = FillBpnSection(worksheet, currentCell, simulationYears, bpnCostPerYear);
            FillRemainingBudgetSection(worksheet, simulationYears, currentCell, workTypeTotalRow);
        }

        #region Private methods
        private Dictionary<TreatmentCategory, SortedDictionary<int, decimal>> AddCostOfWorkOutsideScope(List<BaseCommittedProjectDTO> committedProjectsForWorkOutsideScope)
        {
            var workTypeTotalWorkOutsideScope = new Dictionary<TreatmentCategory, SortedDictionary<int, decimal>>();
            var category = TreatmentCategory.WorkOutsideScope;

            foreach (var committedProjectForWorkOutsideScope in committedProjectsForWorkOutsideScope)
            {
                var currYear = committedProjectForWorkOutsideScope.Year;
                var treatmentCost = Convert.ToDecimal(committedProjectForWorkOutsideScope.Cost);

                if (!workTypeTotalWorkOutsideScope.ContainsKey(category))
                {
                    workTypeTotalWorkOutsideScope.Add(category, new SortedDictionary<int, decimal>());
                }
                if (!workTypeTotalWorkOutsideScope[category].ContainsKey(currYear))
                {
                    workTypeTotalWorkOutsideScope[category].Add(currYear, 0);
                }
                workTypeTotalWorkOutsideScope[category][currYear] += treatmentCost;
            }

            return workTypeTotalWorkOutsideScope;
        }

        private Dictionary<TreatmentCategory, SortedDictionary<int, decimal>> FillCostOfCommittedWorkSection(ExcelWorksheet worksheet, CurrentCell currentCell,
            List<int> simulationYears, Dictionary<int, Dictionary<string, (decimal treatmentCost, int bridgeCount, string projectSource, string treatmentCategory)>> yearlyCostCommittedProj)
        {
            _bridgeWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Cost of Committed Work", "Committed Work Type");
            var workTypeTotalData = AddCostsOfCommittedWork(worksheet, simulationYears, currentCell, yearlyCostCommittedProj);
            return workTypeTotalData;
        }

        private Dictionary<TreatmentCategory, SortedDictionary<int, decimal>> FillCostOfMPMSWorkSection(ExcelWorksheet worksheet, CurrentCell currentCell,
            List<int> simulationYears, Dictionary<int, Dictionary<string, (decimal treatmentCost, int bridgeCount, string projectSource, string treatmentCategory)>> yearlyCostCommittedProj)
        {
            _bridgeWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Cost of MPMS Work", "MPMS Work Type");
            var workTypeTotalData = AddCostsOfMPMSWork(worksheet, simulationYears, currentCell, yearlyCostCommittedProj);
            return workTypeTotalData;
        }

        private Dictionary<TreatmentCategory, SortedDictionary<int, decimal>> FillCostOfSAPWorkSection(
            ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, Dictionary<int, Dictionary<string, (decimal treatmentCost, int bridgeCount, string projectSource, string treatmentCategory)>> yearlyCostCommittedProj)
        {
            _bridgeWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Cost of SAP Work", "SAP Work Type");
            var workTypeTotalDataSAP = AddCostsOfSAPWork(worksheet, simulationYears, currentCell, yearlyCostCommittedProj);
            return workTypeTotalDataSAP;
        }

        private Dictionary<TreatmentCategory, SortedDictionary<int, decimal>> FillCostOfProjectBuilderWorkSection(
            ExcelWorksheet worksheet,
            CurrentCell currentCell,
            List<int> simulationYears,
            Dictionary<int, Dictionary<string, (decimal treatmentCost, int bridgeCount, string projectSource, string treatmentCategory)>> yearlyCostCommittedProj)
        {
            _bridgeWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Cost of Project Builder Work", "Project Builder Work Type");
            var workTypeTotalDataProjectBuilder = AddCostsOfProjectBuilderWork(worksheet, simulationYears, currentCell, yearlyCostCommittedProj);
            return workTypeTotalDataProjectBuilder;
        }

        private Dictionary<TreatmentCategory, SortedDictionary<int, decimal>> FillCostOfCulvertWorkSection(ExcelWorksheet worksheet, CurrentCell currentCell,
            List<int> simulationYears,
            Dictionary<int, Dictionary<string, (decimal treatmentCost, int bridgeCount)>> costPerTreatmentPerYear,
            List<(string Name, string AssetType, TreatmentCategory Category)> simulationTreatments)
        {
            _bridgeWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Cost of BAMS Culvert Work", "BAMS Culvert Work Type");
            var workTypeTotalCulvert = AddCostsOfCulvertWork(worksheet, simulationYears, currentCell, costPerTreatmentPerYear, simulationTreatments);

            return workTypeTotalCulvert;
        }

        private Dictionary<TreatmentCategory, SortedDictionary<int, decimal>> FillCostOfBridgeWorkSection(ExcelWorksheet worksheet, CurrentCell currentCell,
            List<int> simulationYears,
            Dictionary<int, Dictionary<string, (decimal treatmentCost, int bridgeCount)>> costPerTreatmentPerYear,
            List<(string Name, string AssetType, TreatmentCategory Category)> simulationTreatments, Dictionary<TreatmentCategory, SortedDictionary<int, decimal>> workTypeTotalBundled)
        {
            _bridgeWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Cost of BAMS Bridge Work", "BAMS Bridge Work Type");
            var workTypeTotalBridge = AddCostsOfBridgeWork(worksheet, simulationYears, currentCell, costPerTreatmentPerYear, simulationTreatments, workTypeTotalBundled);

            return workTypeTotalBridge;
        }

        private int FillWorkTypeTotalsSection(
            ExcelWorksheet worksheet,
            CurrentCell currentCell,
            List<int> simulationYears,
            Dictionary<string, Budget> yearlyBudgetAmount,
            WorkTypeTotalAggregated workTypeTotalAggregated,
            bool shouldBundleFeasibleTreatments)
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
            var rowIndex = firstContentRow;
            for (var workType = workTypes[0]; workType <= workTypes.Last(); workType++)
            {
                if (workType == TreatmentCategory.Reconstruction)
                {
                    continue;
                }
                                
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

                // For work outside scope
                AddWorkTypeTotalData(workTypeTotalAggregated.WorkTypeTotalWorkOutsideScope, workType, worksheet, rowIndex);

                // Bundled
                if (workType == TreatmentCategory.Bundled)
                {
                    AddWorkTypeTotalData(workTypeTotalAggregated.WorkTypeTotalBundled, workType, worksheet, rowIndex);
                }

                // This line fills up data for "Total (all years)"
                worksheet.Cells[rowIndex, startColumnIndex + numberOfYears].Formula = ExcelFormulas.Sum(rowIndex, startColumnIndex, rowIndex, startColumnIndex + numberOfYears - 1);

                rowIndex++;
            }
            var lastContentRow = rowIndex - 1; //firstContentRow + workTypes.Count - 1;
            currentCell.Row = lastContentRow + 1;//+= workTypes.Count();
            var totalSpentRow = currentCell.Row;
            TotalSpentRow = totalSpentRow;
            worksheet.Cells[totalSpentRow, startColumnIndex + numberOfYears].Formula = ExcelFormulas.Sum(totalSpentRow, startColumnIndex, currentCell.Row, startColumnIndex + numberOfYears - 1);
            worksheet.Cells[totalSpentRow, 1].Value = "Total Spent";
            for (var columnIndex = 3; columnIndex < 3 + numberOfYears; columnIndex++)
            {
                var startAddress = worksheet.Cells[firstContentRow, columnIndex].Address;
                var endAddress = worksheet.Cells[lastContentRow, columnIndex].Address;
                var year = simulationYears[columnIndex - 3];
                worksheet.Cells[currentCell.Row, columnIndex].Formula = ExcelFormulas.RangeSum(startAddress, endAddress);
                worksheet.Cells[currentCell.Row, columnIndex].Calculate();
                worksheet.Cells[currentCell.Row, columnIndex].Value = Convert.ToDecimal(worksheet.Cells[currentCell.Row, columnIndex].Value) +
                                                                      TotalCommittedSpent[year] + TotalMPMSSpent[year] + TotalSAPSpent[year] + TotalProjectBuilderSpent[year];
            }

            // Adding percentage after the Total (all years)
            rowIndex = firstContentRow;
            for (var workType = workTypes[0]; workType <= workTypes.Last(); workType++)
            {
                if (workType == TreatmentCategory.Reconstruction)
                {
                    continue;
                }

                var col = startColumnIndex + numberOfYears + 1;
                worksheet.Cells[rowIndex, col].Formula = ExcelFormulas.Percentage(rowIndex, col - 1, totalSpentRow, col - 1);

                worksheet.Cells[rowIndex, col + 1].Value = $"Percentage Spent on {workType.ToSpreadsheetString().ToUpper()}";

                rowIndex++;
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
            TreatmentCategory workType, ExcelWorksheet worksheet, int rowIndex)
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

        private Dictionary<TreatmentCategory, SortedDictionary<int, decimal>> AddCostsOfCommittedWork(ExcelWorksheet worksheet, List<int> simulationYears, CurrentCell currentCell, Dictionary<int, Dictionary<string, (decimal treatmentCost, int bridgeCount, string projectSource, string treatmentCategory)>> yearlyCostCommittedProj)
        {
            var workTypeTotalCommitted = new Dictionary<TreatmentCategory, SortedDictionary<int, decimal>>();
            if (simulationYears.Count <= 0)
            {
                return workTypeTotalCommitted;
            }
            var startYear = simulationYears[0];
            _bridgeWorkSummaryCommon.SetRowColumns(currentCell, out var startRow, out var startColumn, out var row, out var column);
            currentCell.Column = column;
            var committedTotalRow = 0;
            var uniqueTreatments = new Dictionary<string, int>();
            var map = WorkTypeMap.Map;

            // filling in the committed treatments in the excel TAB
            foreach (var yearlyItem in yearlyCostCommittedProj)
            {
                decimal committedTotalCost = 0;
                row = currentCell.Row;
                foreach (var data in yearlyItem.Value)
                {                    
                    var dataValue = data.Value;
                    if (dataValue.projectSource == "Committed")
                    {
                        var key = data.Key.Contains("Bundle") ? data.Key : dataValue.treatmentCategory;
                        if (!uniqueTreatments.ContainsKey(key))
                        {
                            uniqueTreatments.Add(key, currentCell.Row);
                            worksheet.Cells[row++, column].Value = key;
                            var cellToEnterCost = yearlyItem.Key - startYear;
                            worksheet.Cells[uniqueTreatments[key], column + cellToEnterCost + 2].Value = dataValue.treatmentCost;
                            currentCell.Row += 1;
                        }
                        else
                        {
                            var cellToEnterCost = yearlyItem.Key - startYear;
                            var currentValue = worksheet.Cells[uniqueTreatments[key], column + cellToEnterCost + 2].Value;
                            decimal toAdd = currentValue == null ? 0 : Convert.ToDecimal(currentValue);
                            worksheet.Cells[uniqueTreatments[key], column + cellToEnterCost + 2].Value = dataValue.treatmentCost + toAdd;
                        }
                        committedTotalCost += dataValue.treatmentCost;

                        // setting up data for Work type totals
                        if (map.ContainsKey(dataValue.treatmentCategory))
                        {
                            var category = map[dataValue.treatmentCategory];
                            var treatmentCost = dataValue.treatmentCost;
                            var currYear = yearlyItem.Key;
                            FillWorkTypeTotalCommitted(workTypeTotalCommitted, category, simulationYears, currYear, treatmentCost);
                        }
                        else
                        {
                            var treatmentCost = dataValue.treatmentCost;
                            var currYear = yearlyItem.Key;
                            var category = TreatmentCategory.Other;
                            FillWorkTypeTotalCommitted(workTypeTotalCommitted, category, simulationYears, currYear, treatmentCost);

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
                        
            return workTypeTotalCommitted;
        }

        private Dictionary<TreatmentCategory, SortedDictionary<int, decimal>> AddCostsOfMPMSWork(ExcelWorksheet worksheet, List<int> simulationYears, CurrentCell currentCell, Dictionary<int, Dictionary<string, (decimal treatmentCost, int bridgeCount, string projectSource, string treatmentCategory)>> yearlyCostCommittedProj)
        {
            var workTypeTotalMPMS = new Dictionary<TreatmentCategory, SortedDictionary<int, decimal>>();
            if (simulationYears.Count <= 0)
            {
                return workTypeTotalMPMS;
            }
            var startYear = simulationYears[0];
            _bridgeWorkSummaryCommon.SetRowColumns(currentCell, out var startRow, out var startColumn, out var row, out var column);
            currentCell.Column = column;
            var mpmsTotalRow = 0;
            var uniqueTreatments = new Dictionary<string, int>();
            var map = WorkTypeMap.Map;

            // filling in the committed treatments in the excel TAB
            foreach (var yearlyItem in yearlyCostCommittedProj)
            {
                decimal committedTotalCost = 0;
                row = currentCell.Row;
                foreach (var data in yearlyItem.Value)
                {                    
                    var dataValue = data.Value;
                    if (dataValue.projectSource == "MPMS")
                    {
                        var key = data.Key.Contains("Bundle") ? data.Key : dataValue.treatmentCategory;
                        if (!uniqueTreatments.ContainsKey(key))
                        {                            
                            uniqueTreatments.Add(key, currentCell.Row);
                            worksheet.Cells[row++, column].Value = key;
                            var cellToEnterCost = yearlyItem.Key - startYear;
                            worksheet.Cells[uniqueTreatments[key], column + cellToEnterCost + 2].Value = dataValue.treatmentCost;
                            currentCell.Row += 1;
                        }
                        else
                        {
                            var cellToEnterCost = yearlyItem.Key - startYear;
                            var currentValue = worksheet.Cells[uniqueTreatments[key], column + cellToEnterCost + 2].Value;
                            decimal toAdd = currentValue == null ? 0 : Convert.ToDecimal(currentValue);
                            worksheet.Cells[uniqueTreatments[key], column + cellToEnterCost + 2].Value = dataValue.treatmentCost + toAdd;
                        }
                        committedTotalCost += dataValue.treatmentCost;

                        // setting up data for Work type totals
                        if (map.ContainsKey(dataValue.treatmentCategory))
                        {
                            var category = map[dataValue.treatmentCategory];
                            var treatmentCost = dataValue.treatmentCost;
                            var currYear = yearlyItem.Key;
                            FillWorkTypeTotalMPMS(workTypeTotalMPMS, category, simulationYears, currYear, treatmentCost);
                        }
                        else
                        {
                            var treatmentCost = dataValue.treatmentCost;
                            var currYear = yearlyItem.Key;
                            var category = TreatmentCategory.Other;
                            FillWorkTypeTotalMPMS(workTypeTotalMPMS, category, simulationYears, currYear, treatmentCost);

                        }                        
                    }
                }
                TotalMPMSSpent.Add(yearlyItem.Key, committedTotalCost);
            }

            column = currentCell.Column;
            worksheet.Cells[currentCell.Row, column].Value = BAMSConstants.CommittedTotal;
            column++;
            var fromColumn = column + 1;

            foreach (var cost in TotalMPMSSpent)
            {
                worksheet.Cells[currentCell.Row, fromColumn++].Value = cost.Value;
            }
            mpmsTotalRow = currentCell.Row;
            fromColumn = column + 1;
            var endColumn = simulationYears.Count + 2;

            ExcelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, mpmsTotalRow, endColumn]);
            ExcelHelper.SetCustomFormat(worksheet.Cells[startRow, fromColumn, mpmsTotalRow, endColumn], ExcelHelperCellFormat.NegativeCurrency);
            ExcelHelper.ApplyColor(worksheet.Cells[startRow, fromColumn, mpmsTotalRow, endColumn], Color.FromArgb(198, 224, 180));

            ExcelHelper.ApplyColor(worksheet.Cells[mpmsTotalRow, fromColumn, mpmsTotalRow, endColumn], Color.FromArgb(84, 130, 53));
            ExcelHelper.SetTextColor(worksheet.Cells[mpmsTotalRow, fromColumn, mpmsTotalRow, endColumn], Color.White);
            _bridgeWorkSummaryCommon.UpdateCurrentCell(currentCell, mpmsTotalRow + 1, endColumn);
                        
            return workTypeTotalMPMS;
        }

        private Dictionary<TreatmentCategory, SortedDictionary<int, decimal>> AddCostsOfSAPWork(
            ExcelWorksheet worksheet,
            List<int> simulationYears,
            CurrentCell currentCell,
            Dictionary<int, Dictionary<string, (decimal treatmentCost, int bridgeCount, string projectSource, string treatmentCategory)>> yearlyCostSAPProj)
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
            var map = WorkTypeMap.Map;

            foreach (var yearlyItem in yearlyCostSAPProj)
            {
                decimal sapTotalCost = 0;
                row = currentCell.Row; 

                foreach (var data in yearlyItem.Value)
                {
                    var dataValue = data.Value;
                    if (dataValue.projectSource == "SAP")
                    {
                        var key = data.Key.Contains("Bundle") ? data.Key : dataValue.treatmentCategory;
                        if (!uniqueTreatments.ContainsKey(key))
                        {
                            uniqueTreatments.Add(key, currentCell.Row);
                            worksheet.Cells[row++, column].Value = key;
                            var cellToEnterCost = yearlyItem.Key - startYear;
                            worksheet.Cells[uniqueTreatments[key], column + cellToEnterCost + 2].Value = dataValue.treatmentCost;
                            currentCell.Row += 1;
                        }
                        else
                        {
                            var cellToEnterCost = yearlyItem.Key - startYear;
                            var currentValue = worksheet.Cells[uniqueTreatments[key], column + cellToEnterCost + 2].Value;
                            decimal toAdd = currentValue == null ? 0 : Convert.ToDecimal(currentValue);
                            worksheet.Cells[uniqueTreatments[key], column + cellToEnterCost + 2].Value = dataValue.treatmentCost + toAdd;
                        }
                        sapTotalCost += dataValue.treatmentCost;

                        if (map.ContainsKey(dataValue.treatmentCategory))
                        {
                            var category = map[dataValue.treatmentCategory];
                            var treatmentCost = dataValue.treatmentCost;
                            var currYear = yearlyItem.Key;
                            FillWorkTypeTotalSAP(workTypeTotalSAP, category, simulationYears, currYear, treatmentCost);
                        }
                        else
                        {
                            var treatmentCost = dataValue.treatmentCost;
                            var currYear = yearlyItem.Key;
                            var category = TreatmentCategory.Other;
                            FillWorkTypeTotalSAP(workTypeTotalSAP, category, simulationYears, currYear, treatmentCost);
                        }
                    }
                }

                TotalSAPSpent.Add(yearlyItem.Key, sapTotalCost);
            }

            column = currentCell.Column;
            worksheet.Cells[currentCell.Row, column].Value = BAMSConstants.SAPTotal; 
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
                 Dictionary<int, Dictionary<string, (decimal treatmentCost, int bridgeCount, string projectSource, string treatmentCategory)>> yearlyCostProjectBuilderProj)
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
            var map = WorkTypeMap.Map;

            foreach (var yearlyItem in yearlyCostProjectBuilderProj)
            {
                decimal projectBuilderTotalCost = 0;
                row = currentCell.Row;

                foreach (var data in yearlyItem.Value)
                {
                    var dataValue = data.Value;
                    if (dataValue.projectSource == "ProjectBuilder")
                    {
                        var key = data.Key.Contains("Bundle") ? data.Key : dataValue.treatmentCategory;
                        if (!uniqueTreatments.ContainsKey(key))
                        {                           
                            uniqueTreatments.Add(key, currentCell.Row);
                            worksheet.Cells[row++, column].Value = key;
                            var cellToEnterCost = yearlyItem.Key - startYear + 2;
                            worksheet.Cells[uniqueTreatments[key], column + cellToEnterCost].Value = dataValue.treatmentCost;
                            currentCell.Row += 1;
                        }
                        else
                        {
                            var cellToEnterCost = yearlyItem.Key - startYear + 2;
                            var currentValue = worksheet.Cells[uniqueTreatments[key], column + cellToEnterCost].Value;
                            decimal toAdd = currentValue == null ? 0 : Convert.ToDecimal(currentValue);
                            worksheet.Cells[uniqueTreatments[key], column + cellToEnterCost].Value = dataValue.treatmentCost = toAdd;
                        }
                        projectBuilderTotalCost += dataValue.treatmentCost;

                        if (map.ContainsKey(dataValue.treatmentCategory))
                        {
                            var category = map[dataValue.treatmentCategory];
                            var treatmentCost = dataValue.treatmentCost;
                            var currYear = yearlyItem.Key;
                            FillWorkTypeTotalProjectBuilder(workTypeTotalProjectBuilder, category, simulationYears, currYear, treatmentCost);
                        }
                        else
                        {
                            var treatmentCost = dataValue.treatmentCost;
                            var currYear = yearlyItem.Key;
                            var category = TreatmentCategory.Other;
                            FillWorkTypeTotalProjectBuilder(workTypeTotalProjectBuilder, category, simulationYears, currYear, treatmentCost);
                        }
                    }
                }

                TotalProjectBuilderSpent.Add(yearlyItem.Key, projectBuilderTotalCost); 
            }

            column = currentCell.Column;
            worksheet.Cells[currentCell.Row, column].Value = BAMSConstants.ProjectBuilderTotal; 
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
            List<(string Name, string AssetType, TreatmentCategory Category)> simulationTreatments)
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
                    if (treatment.AssetType == "Culvert" &&
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

            return workTypeTotalCulvert;
        }

        private Dictionary<TreatmentCategory, SortedDictionary<int, decimal>> AddCostsOfBridgeWork(ExcelWorksheet worksheet,
            List<int> simulationYears, CurrentCell currentCell,
            Dictionary<int, Dictionary<string, (decimal treatmentCost, int bridgeCount)>> costPerTreatmentPerYear,
            List<(string Name, string AssetType, TreatmentCategory Category)> simulationTreatments, Dictionary<TreatmentCategory, SortedDictionary<int, decimal>> workTypeTotalBundled)
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
            // Bundled Treatments
            if (ShouldBundleFeasibleTreatments)
            {
                worksheet.Cells[row++, column].Value = BAMSConstants.BundledTreatments;
            }
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
                    if (treatment.AssetType == "Bridge" &&
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

                if (ShouldBundleFeasibleTreatments)
                {
                    var category = TreatmentCategory.Bundled;
                    if (!workTypeTotalBundled.ContainsKey(category))
                    {
                        workTypeTotalBundled.Add(category, new SortedDictionary<int, decimal>()
                        {
                            { yearlyValues.Key, 0 }
                        });
                    }
                    else
                    {
                        if (!workTypeTotalBundled[category].ContainsKey(yearlyValues.Key))
                        {
                            workTypeTotalBundled[category].Add(yearlyValues.Key, 0);
                        }
                    }
                    decimal bundledCost = 0;
                    foreach (var yearlyValue in yearlyValues.Value)
                    {
                        var treatment = yearlyValue.Key;
                        if (treatment.Contains("Bundle"))
                        {
                            decimal cost = 0;
                            cost = yearlyValue.Value.treatmentCost;
                            bundledCost += cost;
                            nonCulvertTotalCost += cost;
                            workTypeTotalBundled[category][yearlyValues.Key] += cost;
                        }
                    }
                    worksheet.Cells[row++, column].Value = bundledCost;
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
            return workTypeTotalBridge;
        }

        private void AddDetailsForRemainingBudget(ExcelWorksheet worksheet, List<int> simulationYears, CurrentCell currentCell,
             int budgetTotalRow)
        {
            int startRow, startColumn, row, column;
            _bridgeWorkSummaryCommon.SetRowColumns(currentCell, out startRow, out startColumn, out row, out column);
            var rowTitles = new List<string> { BAMSConstants.RemainingBudget, BAMSConstants.PercentBudgetSpentBAMS, BAMSConstants.PercentBudgetSpentCommitted, BAMSConstants.PercentBudgetSpentMPMS, BAMSConstants.PercentBudgetSpentSAP, BAMSConstants.PercentBudgetSpentProjectBuilder };
            for (int index= 0;index < rowTitles.Count;index++)
            {
                worksheet.Cells[row++, column].Value = rowTitles[index];
            }
            column++;

            var fromColumn = column + 1;
            foreach (var year in simulationYears)
            {
                row = startRow;
                column = ++column;
                var totalSpent = Convert.ToDecimal(worksheet.Cells[TotalSpentRow, column].Value);

                // Remaining
                var yearlyBudget = Convert.ToDecimal(worksheet.Cells[budgetTotalRow, column].Value);
                worksheet.Cells[row, column].Value = yearlyBudget - totalSpent;
                row++;

                var committedBudgetTotal = TotalCommittedSpent[year];
                var mpmsBudgetTotal = TotalMPMSSpent[year];
                var sapBudgetTotal = TotalSAPSpent[year];
                var projectBuilderBudgetTotal = TotalProjectBuilderSpent[year];
                var bamsBudgetTotal = totalSpent - (committedBudgetTotal + mpmsBudgetTotal + sapBudgetTotal + projectBuilderBudgetTotal);
                var categoryBudgetTotals = new decimal[] { bamsBudgetTotal, committedBudgetTotal, mpmsBudgetTotal, sapBudgetTotal, projectBuilderBudgetTotal };
                // Budget spent in each category
                for (int rowIndex = 0; rowIndex < categoryBudgetTotals.Length; rowIndex++)
                {
                    // Calculate percentage
                    var percentage = categoryBudgetTotals[rowIndex] / yearlyBudget;
                    worksheet.Cells[row++, column].Value = percentage;
                }
            }

            worksheet.Cells[startRow, column + 1].Formula = "SUM(" + worksheet.Cells[startRow, fromColumn, startRow, column] + ")";
            if (_workSummaryModel.AnnualizedAmount != 0)
            {
                worksheet.Cells[startRow, column + 2].Formula = worksheet.Cells[startRow, column + 1] + "/"
                    + (_workSummaryModel.AnnualizedAmount * simulationYears.Count);
            }

            worksheet.Cells[startRow, column + 2].Style.Numberformat.Format = "#0.00%";
            worksheet.Cells[startRow, column + 3].Value = "Percentage of Total Budget that was Unspent";

            ExcelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row - 1, column + 1]);

            ExcelHelper.SetCustomFormat(worksheet.Cells[row - 6, fromColumn, row - 6, column + 1], ExcelHelperCellFormat.NegativeCurrency);
            ExcelHelper.SetCustomFormat(worksheet.Cells[row - 5, fromColumn, row - 1, column], ExcelHelperCellFormat.Percentage);

            ExcelHelper.ApplyColor(worksheet.Cells[startRow, fromColumn, startRow, column], Color.Red);
            ExcelHelper.ApplyColor(worksheet.Cells[startRow + 1, fromColumn, row - 1, column], Color.FromArgb(248, 203, 173));
            _bridgeWorkSummaryCommon.UpdateCurrentCell(currentCell, row + 2, column);
            ExcelHelper.ApplyColor(worksheet.Cells[row + 1, startColumn, row + 1, column], Color.DimGray);
        }


        private void FillWorkTypeTotalCommitted(Dictionary<TreatmentCategory, SortedDictionary<int, decimal>> workTypeTotalMPMS,
           TreatmentCategory category, List<int> simulationYears, int currYear, decimal treatmentCost)
        {
            if (!workTypeTotalMPMS.ContainsKey(category))
            {
                workTypeTotalMPMS.Add(category, new SortedDictionary<int, decimal>());
                foreach (var year in simulationYears)
                {
                    workTypeTotalMPMS[category].Add(year, 0);
                }
            }
            workTypeTotalMPMS[category][currYear] += treatmentCost;
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
            }
            workTypeTotalMPMS[category][currYear] += treatmentCost;
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
