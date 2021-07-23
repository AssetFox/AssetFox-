﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.Domains;
using BridgeCareCore.Interfaces.SummaryReport;
using BridgeCareCore.Logging;
using BridgeCareCore.Models.SummaryReport;
using BridgeCareCore.Services.SummaryReport.BridgeWorkSummary;
using BridgeCareCore.Services.SummaryReport.BridgeWorkSummary.StaticContent;
using OfficeOpenXml;

namespace BridgeCareCore.Services.SummaryReport.BridgeWorkSummaryByBudget
{
    public class BridgeWorkSummaryByBudget : IBridgeWorkSummaryByBudget
    {
        private readonly BridgeWorkSummaryCommon _bridgeWorkSummaryCommon;
        private readonly CulvertCost _culvertCost;
        private readonly BridgeWorkCost _bridgeWorkCost;
        private readonly CommittedProjectCost _committedProjectCost;

        public BridgeWorkSummaryByBudget(BridgeWorkSummaryCommon bridgeWorkSummaryCommon,
            CulvertCost culvertCost, BridgeWorkCost bridgeWorkCost, CommittedProjectCost committedProjectCost)
        {
            _bridgeWorkSummaryCommon = bridgeWorkSummaryCommon ?? throw new ArgumentNullException(nameof(bridgeWorkSummaryCommon));
            _culvertCost = culvertCost ?? throw new ArgumentNullException(nameof(culvertCost));
            _bridgeWorkCost = bridgeWorkCost ?? throw new ArgumentNullException(nameof(bridgeWorkCost));
            _committedProjectCost = committedProjectCost ?? throw new ArgumentNullException(nameof(committedProjectCost));
        }

        public void Fill(ExcelWorksheet worksheet, SimulationOutput reportOutputData,
            List<int> simulationYears, Dictionary<string, Budget> yearlyBudgetAmount)
        {
            var startYear = simulationYears[0];
            var currentCell = new CurrentCell { Row = 1, Column = 1 };

            // setting up model to store data. This will be used to fill up Bridge Work Summary By
            // Budget TAB
            var workSummaryByBudgetData = new List<WorkSummaryByBudgetModel>();

            var budgets = new HashSet<string>();
            foreach (var yearData in reportOutputData.Years)
            {
                foreach (var item in yearData.Budgets)
                {
                    budgets.Add(item.BudgetName);
                }
            }
            foreach (var item in budgets)
            {
                workSummaryByBudgetData.Add(new WorkSummaryByBudgetModel
                {
                    Budget = item,
                    YearlyData = new List<YearsData>()
                });
            }

            var committedTreatments = new HashSet<string>();
            foreach (var summaryData in workSummaryByBudgetData)
            {
                foreach (var yearData in reportOutputData.Years)
                {
                    foreach (var section in yearData.Sections)
                    {
                        if (section.TreatmentCause == TreatmentCause.CommittedProject &&
                            section.AppliedTreatment.ToLower() != Properties.Resources.NoTreatment)
                        {
                            var committedtTreatment = section.TreatmentConsiderations;
                            var budgetAmount = (double)committedtTreatment.Sum(_ =>
                            _.BudgetUsages.Where(b => b.BudgetName == summaryData.Budget).Sum(bu => bu.CoveredCost));

                            summaryData.YearlyData.Add(new YearsData
                            {
                                Year = yearData.Year,
                                Treatment = section.AppliedTreatment,
                                Amount = budgetAmount,
                                isCommitted = true,
                                costPerBPN = (section.ValuePerTextAttribute["BUS_PLAN_NETWORK"], budgetAmount)
                            });
                            committedTreatments.Add(section.AppliedTreatment);
                            continue;
                        }

                        if (section.TreatmentCause != TreatmentCause.NoSelection)
                        {
                            var treatmentConsideration = section.TreatmentConsiderations;
                            var budgetAmount = (double)treatmentConsideration.Sum(_ =>
                            _.BudgetUsages.Where(b => b.BudgetName == summaryData.Budget).Sum(bu => bu.CoveredCost));

                            summaryData.YearlyData.Add(new YearsData
                            {
                                Year = yearData.Year,
                                Treatment = section.AppliedTreatment,
                                Amount = budgetAmount,
                                costPerBPN = (section.ValuePerTextAttribute["BUS_PLAN_NETWORK"], budgetAmount)
                            });
                        }
                    }
                }
            }
            // Model setup complete. [TODO]: Make it efficient

            foreach (var summaryData in workSummaryByBudgetData)
            {
                //Filtering treatments for the given budget

                var costForCulvertBudget = new List<YearsData>();
                var costForBridgeBudgets = new List<YearsData>();
                var costForCommittedBudgets = new List<YearsData>();

                var workTypeTotal = new WorkTypeTotal();

                costForCulvertBudget = summaryData.YearlyData
                                            .FindAll(_ => _.Treatment.Contains("culvert", StringComparison.OrdinalIgnoreCase) && !_.isCommitted);

                costForBridgeBudgets = summaryData.YearlyData
                                                .FindAll(_ => !_.Treatment.Contains("culvert", StringComparison.OrdinalIgnoreCase) && !_.isCommitted);

                costForCommittedBudgets = summaryData.YearlyData
                                                    .FindAll(_ => _.isCommitted && _.Treatment.ToLower() != Properties.Resources.NoTreatment);

                var totalBudgetPerYearForCulvert = new Dictionary<int, double>();
                var totalBudgetPerYearForBridgeWork = new Dictionary<int, double>();
                var totalBudgetPerYearForMPMS = new Dictionary<int, double>();

                var totalSpent = new List<(int year, double amount)>();

                // Filling up the total, "culvert" and "Bridge work" costs
                foreach (var year in simulationYears)
                {
                    var yearlyBudget = costForCulvertBudget.FindAll(_ => _.Year == year);
                    var culvertAmountSum = yearlyBudget.Sum(s => s.Amount);
                    totalBudgetPerYearForCulvert.Add(year, culvertAmountSum);

                    yearlyBudget.Clear();

                    yearlyBudget = costForBridgeBudgets.FindAll(_ => _.Year == year);
                    var budgetAmountSum = yearlyBudget.Sum(s => s.Amount);
                    totalBudgetPerYearForBridgeWork.Add(year, budgetAmountSum);

                    yearlyBudget.Clear();

                    yearlyBudget = costForCommittedBudgets.FindAll(_ => _.Year == year);
                    var committedAmountSum = yearlyBudget.Sum(s => s.Amount);
                    totalBudgetPerYearForMPMS.Add(year, committedAmountSum);

                    yearlyBudget.Clear();

                    totalSpent.Add((year, culvertAmountSum + budgetAmountSum + committedAmountSum));
                }

                currentCell.Column = 1;
                currentCell.Row += 2;
                worksheet.Cells[currentCell.Row, currentCell.Column].Value = summaryData.Budget;
                ExcelHelper.MergeCells(worksheet, currentCell.Row, currentCell.Column, currentCell.Row, simulationYears.Count);
                ExcelHelper.ApplyColor(worksheet.Cells[currentCell.Row, currentCell.Column, currentCell.Row, simulationYears.Count + 2], Color.Gray);
                currentCell.Row += 1;

                var amount = totalSpent.Sum(_ => _.amount);
                if (amount > 0)
                {
                    _committedProjectCost.FillCostOfCommittedWork(worksheet, currentCell, simulationYears, costForCommittedBudgets,
                        committedTreatments, totalBudgetPerYearForMPMS, workTypeTotal);

                    _culvertCost.FillCostOfCulvert(worksheet, currentCell, costForCulvertBudget, totalBudgetPerYearForCulvert, simulationYears, workTypeTotal);

                    _bridgeWorkCost.FillCostOfBridgeWork(worksheet, currentCell, simulationYears, costForBridgeBudgets, totalBudgetPerYearForBridgeWork, workTypeTotal);
                }

                currentCell.Row += 1;
                _bridgeWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Total Budget", "Totals");
                currentCell.Row += 1;
                currentCell.Column = 1;
                var startOfTotalBudget = currentCell.Row;
                worksheet.Cells[currentCell.Row, currentCell.Column].Value = Properties.Resources.TotalSpent;

                foreach (var spentAmount in totalSpent)
                {
                    var cellFortotalSpentAmount = spentAmount.year - startYear;
                    worksheet.Cells[currentCell.Row, currentCell.Column + cellFortotalSpentAmount + 2].Value = spentAmount.amount;
                }
                ExcelHelper.ApplyColor(worksheet.Cells[startOfTotalBudget, currentCell.Column + 2, currentCell.Row, simulationYears.Count + 2],
                    Color.FromArgb(84, 130, 53));
                ExcelHelper.SetTextColor(worksheet.Cells[startOfTotalBudget, currentCell.Column + 2, currentCell.Row, simulationYears.Count + 2], Color.White);

                currentCell.Row += 2; // For BAMS Work type Totals
                _bridgeWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "", "BAMS Work Type Totals");

                var workTypes = EnumExtensions.GetValues<WorkTypeName>();
                currentCell.Row++;
                var firstContentRow = currentCell.Row;
                var rowTrackerForColoring = firstContentRow;
                for (var workType = workTypes[0]; workType <= workTypes.Last(); workType++)
                {
                    var rowIndex = firstContentRow + (int)workType;
                    worksheet.Cells[rowIndex, 1].Value = workType.ToSpreadsheetString();
                    worksheet.Cells[rowIndex, 3, rowIndex, simulationYears.Count + 2].Value = 0.0;
                    currentCell.Row++;
                }

                InsertWorkTypeTotals(startYear, firstContentRow, worksheet, workTypeTotal);

                ExcelHelper.SetCustomFormat(worksheet.Cells[rowTrackerForColoring, 3, rowTrackerForColoring + 5, simulationYears.Count + 2], ExcelHelperCellFormat.NegativeCurrency);
                ExcelHelper.ApplyColor(worksheet.Cells[rowTrackerForColoring, 3, rowTrackerForColoring + 5, simulationYears.Count + 2], Color.FromArgb(84, 130, 53));
                ExcelHelper.SetTextColor(worksheet.Cells[rowTrackerForColoring, 3, rowTrackerForColoring + 5, simulationYears.Count + 2], Color.White);

                currentCell.Row += 2;
                currentCell.Column = 1;
                worksheet.Cells[currentCell.Row, currentCell.Column].Value = Properties.Resources.TotalBridgeCareBudget;

                var budgetDetails = yearlyBudgetAmount[summaryData.Budget];
                var yearTracker = 0;
                foreach (var item in budgetDetails.YearlyAmounts)
                {
                    var cellFortotalBudget = yearTracker;
                    var currValue = worksheet.Cells[currentCell.Row, currentCell.Column + cellFortotalBudget + 2].Value;
                    decimal totalCost = 0;
                    if (currValue != null)
                    {
                        totalCost = (decimal)worksheet.Cells[currentCell.Row, currentCell.Column + cellFortotalBudget + 2].Value;
                    }
                    totalCost += item.Value;
                    worksheet.Cells[currentCell.Row, currentCell.Column + cellFortotalBudget + 2].Value = totalCost;
                    yearTracker++;
                }
                ExcelHelper.ApplyBorder(worksheet.Cells[startOfTotalBudget, currentCell.Column, currentCell.Row, simulationYears.Count + 2]);
                ExcelHelper.SetCustomFormat(worksheet.Cells[startOfTotalBudget, currentCell.Column + 2, startOfTotalBudget, simulationYears.Count + 2], ExcelHelperCellFormat.NegativeCurrency);
                ExcelHelper.SetCustomFormat(worksheet.Cells[currentCell.Row, currentCell.Column + 2, currentCell.Row, simulationYears.Count + 2], ExcelHelperCellFormat.NegativeCurrency);

                ExcelHelper.ApplyColor(worksheet.Cells[currentCell.Row, currentCell.Column + 2, currentCell.Row, simulationYears.Count + 2],
                    Color.FromArgb(84, 130, 53));
                ExcelHelper.SetTextColor(worksheet.Cells[currentCell.Row, currentCell.Column + 2, currentCell.Row, simulationYears.Count + 2], Color.White);

                // Cost per BPN
                currentCell.Row += 2;
                _bridgeWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Total Cost Per BPN", "BPN");
                currentCell.Row++;

                var firstBPNRowForFormat = currentCell.Row;
                InsertCostPerBPN(worksheet, currentCell, startYear, summaryData, simulationYears);

                ExcelHelper.ApplyBorder(worksheet.Cells[firstBPNRowForFormat, 1, currentCell.Row, simulationYears.Count + 2]);
                ExcelHelper.SetCustomFormat(worksheet.Cells[firstBPNRowForFormat, 3, currentCell.Row, simulationYears.Count + 2], ExcelHelperCellFormat.NegativeCurrency);

                ExcelHelper.ApplyColor(worksheet.Cells[firstBPNRowForFormat, 3, currentCell.Row, simulationYears.Count + 2],
                    Color.FromArgb(255, 230, 153));
                // End of Cost per BPN

                currentCell.Row += 2;
                _bridgeWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Budget Analysis", "");
                currentCell.Row += 1;
                currentCell.Column = 1;
                worksheet.Cells[currentCell.Row, currentCell.Column].Value = Properties.Resources.RemainingBudget;
                worksheet.Cells[currentCell.Row + 1, currentCell.Column].Value = Properties.Resources.PercentBudgetSpentMPMS;
                worksheet.Cells[currentCell.Row + 2, currentCell.Column].Value = Properties.Resources.PercentBudgetSpentBAMS;

                yearTracker = 0;
                foreach (var budgetData in budgetDetails.YearlyAmounts)
                {
                    var perYearTotalSpent = totalSpent.Find(_ => _.year == startYear + yearTracker);
                    var cellFortotalBudget = yearTracker;
                    worksheet.Cells[currentCell.Row, currentCell.Column + cellFortotalBudget + 2].Value = budgetData.Value - (decimal)perYearTotalSpent.amount;

                    worksheet.Cells[currentCell.Row + 1, currentCell.Column + cellFortotalBudget + 2].Value =
                        totalBudgetPerYearForMPMS[perYearTotalSpent.year] / perYearTotalSpent.amount;

                    worksheet.Cells[currentCell.Row + 2, currentCell.Column + cellFortotalBudget + 2].Value = 1 -
                        totalBudgetPerYearForMPMS[perYearTotalSpent.year] / perYearTotalSpent.amount;
                    yearTracker++;
                }

                ExcelHelper.ApplyBorder(worksheet.Cells[currentCell.Row, currentCell.Column, currentCell.Row + 2, simulationYears.Count + 2]);

                ExcelHelper.SetCustomFormat(worksheet.Cells[currentCell.Row + 1, currentCell.Column + 2,
                    currentCell.Row + 2, simulationYears.Count + 2], ExcelHelperCellFormat.Percentage);
                ExcelHelper.SetCustomFormat(worksheet.Cells[currentCell.Row, currentCell.Column + 2,
                    currentCell.Row, simulationYears.Count + 2], ExcelHelperCellFormat.NegativeCurrency);

                ExcelHelper.ApplyColor(worksheet.Cells[currentCell.Row, currentCell.Column + 2, currentCell.Row, simulationYears.Count + 2],
                    Color.Red);
                ExcelHelper.ApplyColor(worksheet.Cells[currentCell.Row + 1, currentCell.Column + 2, currentCell.Row + 2, simulationYears.Count + 2], Color.FromArgb(248, 203, 173));
                currentCell.Row += 2;
            }
            worksheet.Cells.AutoFitColumns();
        }

        private void InsertCostPerBPN(ExcelWorksheet worksheet, CurrentCell currentCell, int startYear, WorkSummaryByBudgetModel summaryData,
            List<int> simulationYears)
        {
            var bpnNames = EnumExtensions.GetValues<BPNCostBudgetName>();
            var bpnTracker = new Dictionary<string, int>();
            var firstBPNRow = currentCell.Row;
            for (var name = bpnNames[0]; name <= bpnNames.Last(); name++)
            {
                var rowIndex = firstBPNRow + (int)name;
                worksheet.Cells[rowIndex, 1].Value = name.ToSpreadsheetString();
                worksheet.Cells[rowIndex, 3, rowIndex, simulationYears.Count + 2].Value = 0.0;

                if (!bpnTracker.ContainsKey(name.ToMatchInDictionaryString()))
                {
                    bpnTracker.Add(name.ToMatchInDictionaryString(), rowIndex);
                }
                currentCell.Row++;
            }

            var totalBPNPerYear = new Dictionary<int, double>();
            foreach (var item in summaryData.YearlyData)
            {
                var rowIndex = 0;
                if (!bpnTracker.ContainsKey(item.costPerBPN.bpnName))
                {
                    rowIndex = firstBPNRow + bpnNames.Count() - 1;
                }
                else
                {
                    rowIndex = bpnTracker[item.costPerBPN.bpnName];
                }
                var cellToEnterCost = item.Year - startYear;
                var currVal = worksheet.Cells[rowIndex, cellToEnterCost + 3].Value;
                var totalAmt = 0.0;
                if (currVal != null)
                {
                    totalAmt = (double)currVal;
                }
                totalAmt += item.costPerBPN.cost;
                worksheet.Cells[rowIndex, cellToEnterCost + 3].Value = totalAmt;

                if (!totalBPNPerYear.ContainsKey(item.Year))
                {
                    totalBPNPerYear.Add(item.Year, 0);
                }
                totalBPNPerYear[item.Year] += item.costPerBPN.cost;
            }
            worksheet.Cells[firstBPNRow + bpnNames.Count(), 1].Value = Properties.Resources.TotalBPNCost;

            // Total BPN Cost
            foreach (var bpnCost in totalBPNPerYear)
            {
                var cellToEnterCost = bpnCost.Key - startYear;
                var currVal = worksheet.Cells[firstBPNRow + bpnNames.Count(), cellToEnterCost + 3].Value;
                var totalAmt = 0.0;
                if (currVal != null)
                {
                    totalAmt = (double)currVal;
                }
                totalAmt += bpnCost.Value;
                worksheet.Cells[firstBPNRow + bpnNames.Count(), cellToEnterCost + 3].Value = totalAmt;
            }
        }

        private void InsertWorkTypeTotals(int startYear, int firstContentRow, ExcelWorksheet worksheet, WorkTypeTotal workTypeTotal)
        {
            // Add data for BAMS work type totals "Preservation"
            foreach (var item in workTypeTotal.MPMSpreservationCostPerYear)
            {
                FillTheExcelColumns(startYear, item, firstContentRow, worksheet);
            }
            foreach (var item in workTypeTotal.BAMSPreservationCostPerYear)
            {
                FillTheExcelColumns(startYear, item, firstContentRow, worksheet);
            }
            // End of BAMS work type totals "Preservation"

            // Add data for Work type totals "Emergency repair"
            firstContentRow++;
            foreach (var item in workTypeTotal.MPMSEmergencyRepairCostPerYear)
            {
                FillTheExcelColumns(startYear, item, firstContentRow, worksheet);
            }
            // End BAMS work type totals "Emergency repair"

            firstContentRow++;
            // Add data for Work Type Totals "Rehab"
            foreach (var item in workTypeTotal.BAMSRehabCostPerYear)
            {
                FillTheExcelColumns(startYear, item, firstContentRow, worksheet);
            }
            foreach (var item in workTypeTotal.CulvertRehabCostPerYear)
            {
                FillTheExcelColumns(startYear, item, firstContentRow, worksheet);
            }
            foreach (var item in workTypeTotal.MPMSRehabRepairCostPerYear)
            {
                FillTheExcelColumns(startYear, item, firstContentRow, worksheet);
            }
            // End BAMS work type totals "Rehab"

            firstContentRow++;

            // Add data for BAMS Work Type Totals "Replacement"
            foreach (var item in workTypeTotal.MPMSReplacementCostPerYear)
            {
                FillTheExcelColumns(startYear, item, firstContentRow, worksheet);
            }
            foreach (var item in workTypeTotal.CulvertReplacementCostPerYear)
            {
                FillTheExcelColumns(startYear, item, firstContentRow, worksheet);
            }
            foreach (var item in workTypeTotal.BAMSReplacementCostPerYear)
            {
                FillTheExcelColumns(startYear, item, firstContentRow, worksheet);
            }
            // End BAMS work type totals "Replacement"

            firstContentRow++; // this row is for "Other" category
            foreach (var item in workTypeTotal.OtherCostPerYear)
            {
                FillTheExcelColumns(startYear, item, firstContentRow, worksheet);
            }

            firstContentRow++;
            // Add data for BAMS Work Type Totals "Total Spent"
            worksheet.Cells[firstContentRow, 1].Value = Properties.Resources.TotalSpent;
            foreach (var item in workTypeTotal.TotalCostPerYear)
            {
                FillTheExcelColumns(startYear, item, firstContentRow, worksheet);
            }
        }

        private void FillTheExcelColumns(int startYear, KeyValuePair<int, double> item, int firstContentRow, ExcelWorksheet worksheet)
        {
            var cellToEnterCost = item.Key - startYear;
            var currVal = worksheet.Cells[firstContentRow, cellToEnterCost + 3].Value;
            var totalAmt = 0.0;
            if (currVal != null)
            {
                totalAmt = (double)currVal;
            }
            totalAmt += item.Value;
            worksheet.Cells[firstContentRow, cellToEnterCost + 3].Value = totalAmt;
        }
    }
}
