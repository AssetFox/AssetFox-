﻿using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OfficeOpenXml;
using AppliedResearchAssociates.iAM.ExcelHelpers;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.Reporting.Models.BAMSSummaryReport;
using AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport.BridgeWorkSummary;
using AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport.BridgeWorkSummary.StaticContent;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using AppliedResearchAssociates.iAM.Reporting.Models;
using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs.Abstract;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport.BridgeWorkSummaryByBudget
{
    public class BridgeWorkSummaryByBudget
    {
        private BridgeWorkSummaryCommon _bridgeWorkSummaryCommon;
        private CulvertCost _culvertCost;
        private BridgeWorkCost _bridgeWorkCost;
        private CommittedProjectCost _committedProjectCost;
        private ReportHelper _reportHelper;
        private readonly IUnitOfWork _unitOfWork;
        private int TotalSpentRow = 0;

        public BridgeWorkSummaryByBudget(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _bridgeWorkSummaryCommon = new BridgeWorkSummaryCommon();
            _culvertCost = new CulvertCost();
            _bridgeWorkCost = new BridgeWorkCost();
            _committedProjectCost = new CommittedProjectCost();
            _reportHelper = new ReportHelper(_unitOfWork);
        }

        public void Fill(ExcelWorksheet worksheet, SimulationOutput reportOutputData, List<int> simulationYears, Dictionary<string, BudgetDTO> yearlyBudgets
            , List<TreatmentDTO> selectableTreatments, Dictionary<string, string> treatmentCategoryLookup, List<BaseCommittedProjectDTO> committedProjectList, List<BaseCommittedProjectDTO> committedProjectsForWorkOutsideScope, bool shouldBundleFeasibleTreatments, List<SimpleBudgetDetailDTO> scenarioSimpleBudgets)
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
            var map = WorkTypeMap.Map;
            
            foreach (var budgetSummaryData in workSummaryByBudgetData)
            {
                var summaryData = budgetSummaryData;
                var keyCashFlowFundingDetails = new Dictionary<double, List<TreatmentConsiderationDetail>>();

                foreach (var yearData in reportOutputData.Years)
                {
                    var assets = yearData.Assets.Where(_ => _.TreatmentCause != TreatmentCause.NoSelection);
                    foreach (var section in assets)
                    {
                        var section_BRKEY = _reportHelper.CheckAndGetValue<double>(section.ValuePerNumericAttribute, "BRKEY_");

                        // Build keyCashFlowFundingDetails                    
                        _reportHelper.BuildKeyCashFlowFundingDetails(yearData, section, section_BRKEY, keyCashFlowFundingDetails);

                        // If CF then use obj from keyCashFlowFundingDetails otherwise from section
                        var treatmentConsiderations = ((section.TreatmentCause == TreatmentCause.SelectedTreatment &&
                                                      section.TreatmentStatus == TreatmentStatus.Progressed) ||
                                                      (section.TreatmentCause == TreatmentCause.CashFlowProject &&
                                                      section.TreatmentStatus == TreatmentStatus.Progressed) ||
                                                      (section.TreatmentCause == TreatmentCause.CashFlowProject &&
                                                      section.TreatmentStatus == TreatmentStatus.Applied)) ?
                                                      keyCashFlowFundingDetails[section_BRKEY] :
                                                      section.TreatmentConsiderations ?? new();

                        var treatmentConsideration = shouldBundleFeasibleTreatments ?
                                                     treatmentConsiderations.FirstOrDefault(_ => _.FundingCalculationOutput != null &&
                                                        _.FundingCalculationOutput.AllocationMatrix.Any(_ => _.Year == yearData.Year &&
                                                        _.BudgetName == summaryData.Budget) &&
                                                        section.AppliedTreatment.Contains(_.TreatmentName)) :
                                                     treatmentConsiderations.FirstOrDefault(_ => _.FundingCalculationOutput != null &&
                                                        _.FundingCalculationOutput.AllocationMatrix.Any(_ => _.Year == yearData.Year &&
                                                        _.BudgetName == summaryData.Budget) &&
                                                        _.TreatmentName == section.AppliedTreatment);

                        var appliedTreatment = treatmentConsideration?.TreatmentName ?? section.AppliedTreatment;
                        var budgetAmount = (double)(treatmentConsideration?.FundingCalculationOutput?.AllocationMatrix?.
                                           Where(_ => _.BudgetName == summaryData.Budget && _.Year == yearData.Year).
                                           Sum(b => b.AllocatedAmount) ?? 0);
                        budgetAmount = Math.Round(budgetAmount, 0);
                        var bpnName = _reportHelper.CheckAndGetValue<string>(section?.ValuePerTextAttribute, "BUS_PLAN_NETWORK");
                        if (section.TreatmentCause == TreatmentCause.CommittedProject &&
                            appliedTreatment.ToLower() != BAMSConstants.NoTreatment)
                        {
                            var category = appliedTreatment.Contains("Bundle") ?
                                           TreatmentCategory.Bundled :
                                           (map.ContainsKey(treatmentCategoryLookup[appliedTreatment]) ? map[treatmentCategoryLookup[appliedTreatment]] : TreatmentCategory.Other);
                            var projectSource = committedProjectList.FirstOrDefault(_ => appliedTreatment.Contains(_.Treatment) &&
                                                _.Year == yearData.Year && _.ProjectSource.ToString() == section.ProjectSource)?.ProjectSource.ToString();
                            summaryData.YearlyData.Add(new YearsData
                            {
                                Year = yearData.Year,
                                Treatment = appliedTreatment,
                                Amount = budgetAmount,
                                ProjectSource = projectSource,
                                isCommitted = true,
                                costPerBPN = (bpnName, budgetAmount),
                                TreatmentCategory = category
                            });
                            committedTreatments.Add(appliedTreatment);
                            continue;
                        }

                        var treatmentData = appliedTreatment.Contains("Bundle") ?
                                            selectableTreatments.FirstOrDefault(_ => appliedTreatment.Contains(_.Name)) :
                                            selectableTreatments.FirstOrDefault(_ => _.Name == appliedTreatment);
                        summaryData.YearlyData.Add(new YearsData
                        {
                            Year = yearData.Year,
                            Treatment = appliedTreatment,
                            Amount = budgetAmount,
                            costPerBPN = (bpnName, budgetAmount),
                            TreatmentCategory = appliedTreatment.Contains("Bundle") ? TreatmentCategory.Bundled : treatmentData.Category,
                            AssetType = treatmentData.AssetType
                        });
                    }
                }

                //Filtering treatments for the given budget             
                var costForCulvertBudget = summaryData.YearlyData
                                            .Where(_ => _.AssetType == "Culvert" && !_.isCommitted);

                var costForBridgeBudgets = summaryData.YearlyData
                                                .Where(_ => _.AssetType == "Bridge" && !_.isCommitted);

                var costForCommittedBudgets = summaryData.YearlyData
                                                    .Where(_ => _.isCommitted && _.Treatment.ToLower() != BAMSConstants.NoTreatment);

                var totalBudgetPerYearForCulvert = new Dictionary<int, decimal>();
                var totalBudgetPerYearForBridgeWork = new Dictionary<int, decimal>();
                var totalBudgetPerYearForCommitted = new Dictionary<int, decimal>();
                var totalBudgetPerYearForMPMS = new Dictionary<int, decimal>();
                var totalBudgetPerYearForSAP = new Dictionary<int, decimal>();
                var totalBudgetPerYearForProjectBuilder = new Dictionary<int, decimal>();
                var totalSpent = new List<(int year, decimal amount)>();
                var numberOfYears = simulationYears.Count;

                var scenarioBudgetId = scenarioSimpleBudgets.FirstOrDefault(_ => _.Name == summaryData.Budget)?.Id;

                // Filling up the total, "culvert" and "Bridge work" costs
                foreach (var year in simulationYears)
                {
                    // Where works faster than FindAll
                    var yearlycostForCulvertBudget = costForCulvertBudget.Where(_ => _.Year == year);
                    var culvertAmountSum = Convert.ToDecimal(yearlycostForCulvertBudget.Sum(s => s.Amount));
                    totalBudgetPerYearForCulvert.Add(year, culvertAmountSum);

                    var yearlycostForBridgeBudget = costForBridgeBudgets.Where(_ => _.Year == year);
                    var bridgeAmountSum = Convert.ToDecimal(yearlycostForBridgeBudget.Sum(s => s.Amount));
                    totalBudgetPerYearForBridgeWork.Add(year, bridgeAmountSum);

                    var yearlycostForCommittedBudget = costForCommittedBudgets.Where(_ => _.Year == year && _.ProjectSource == "Committed");
                    var committedAmountSum = Convert.ToDecimal(yearlycostForCommittedBudget.Sum(s => s.Amount));
                    totalBudgetPerYearForCommitted.Add(year, committedAmountSum);

                    var yearlycostForMpmsBudget = costForCommittedBudgets.Where(_ => _.Year == year && _.ProjectSource == "MPMS");
                    var mpmsAmountSum = Convert.ToDecimal(yearlycostForMpmsBudget.Sum(s => s.Amount));
                    totalBudgetPerYearForMPMS.Add(year, mpmsAmountSum);                                       

                    var yearlycostForSapBudget = costForCommittedBudgets.Where(_ => _.Year == year && _.ProjectSource == "SAP");
                    var sapAmountSum = Convert.ToDecimal(yearlycostForSapBudget.Sum(s => s.Amount));
                    totalBudgetPerYearForSAP.Add(year, sapAmountSum);

                    var yearlycostForProjectBuilderBudget = costForCommittedBudgets.Where(_ => _.Year == year && _.ProjectSource == "ProjectBuilder");
                    var projectBuilderAmountSum = Convert.ToDecimal(yearlycostForProjectBuilderBudget.Sum(s => s.Amount));
                    totalBudgetPerYearForProjectBuilder.Add(year, projectBuilderAmountSum);

                    totalSpent.Add((year, culvertAmountSum + bridgeAmountSum + committedAmountSum + mpmsAmountSum + sapAmountSum + projectBuilderAmountSum));
                }

                currentCell.Column = 1;
                currentCell.Row += 1;                
                
                worksheet.Cells[currentCell.Row, currentCell.Column].Value = summaryData.Budget;
                ExcelHelper.MergeCells(worksheet, currentCell.Row, currentCell.Column, currentCell.Row, simulationYears.Count);
                ExcelHelper.ApplyColor(worksheet.Cells[currentCell.Row, currentCell.Column, currentCell.Row, simulationYears.Count + 2], Color.Gray);
                var budgetAnalysisRow = currentCell.Row + 2;
                currentCell.Row += 11;                

                var workTypeTotal = new WorkTypeTotal();
                var amount = totalSpent.Sum(_ => _.amount);
                if (amount > 0)
                {                    
                    _committedProjectCost.FillCostOfCommittedWork(worksheet, currentCell, simulationYears, costForCommittedBudgets.ToList(),
                        totalBudgetPerYearForCommitted, workTypeTotal);
                    _committedProjectCost.FillCostOfMPMSWork(worksheet, currentCell, simulationYears, costForCommittedBudgets.ToList(),
                        totalBudgetPerYearForMPMS, workTypeTotal);
                    _committedProjectCost.FillCostOfSAPWork(worksheet, currentCell, simulationYears, costForCommittedBudgets.ToList(),
                        totalBudgetPerYearForSAP, workTypeTotal);
                    _committedProjectCost.FillCostOfProjectBuilderWork(worksheet, currentCell, simulationYears, costForCommittedBudgets.ToList(),
                        totalBudgetPerYearForProjectBuilder, workTypeTotal);                                     
                    _committedProjectCost.AddCostOfWorkOutsideScope(workTypeTotal, committedProjectsForWorkOutsideScope, scenarioBudgetId);

                    _culvertCost.FillCostOfCulvert(worksheet, currentCell, costForCulvertBudget.ToList(), totalBudgetPerYearForCulvert, simulationYears, workTypeTotal);
                    _bridgeWorkCost.FillCostOfBridgeWork(worksheet, currentCell, simulationYears, costForBridgeBudgets.ToList(), totalBudgetPerYearForBridgeWork, workTypeTotal);
                }

                currentCell.Row += 2; // For BAMS Work type Totals
                _bridgeWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "", "BAMS Work Type Totals");

                var initialRow = currentCell.Row;
                worksheet.Cells[initialRow, 3 + numberOfYears].Value = "Total (all years)";
                var totalColumnHeaderRange = worksheet.Cells[initialRow, 3 + numberOfYears];
                ExcelHelper.ApplyBorder(totalColumnHeaderRange);
                ExcelHelper.ApplyStyle(totalColumnHeaderRange);
                var workTypes = new List<TreatmentCategory> { TreatmentCategory.Maintenance, TreatmentCategory.Preservation, TreatmentCategory.Rehabilitation, TreatmentCategory.Replacement, TreatmentCategory.CapacityAdding, TreatmentCategory.Other, TreatmentCategory.Bundled };
                currentCell.Row++;
                var firstContentRow = currentCell.Row;
                var rowIndex = firstContentRow;
                var rowTrackerForColoring = firstContentRow;

                foreach (var workType in workTypes)
                {   
                    worksheet.Cells[rowIndex, 1].Value = workType.ToSpreadsheetString();
                    worksheet.Cells[rowIndex, 3, rowIndex, simulationYears.Count + 2].Value = 0.0;
                    currentCell.Row++;

                    rowIndex++;
                }

                InsertWorkTypeTotals(startYear, firstContentRow, worksheet, workTypeTotal);
                insertTotalAndPercentagePerCategory(worksheet, currentCell, numberOfYears, firstContentRow);

                ExcelHelper.SetCustomFormat(worksheet.Cells[rowTrackerForColoring, 3, rowTrackerForColoring + 8, simulationYears.Count + 3], ExcelHelperCellFormat.NegativeCurrency);
                ExcelHelper.ApplyColor(worksheet.Cells[rowTrackerForColoring, 3, rowTrackerForColoring + 8, simulationYears.Count + 2], Color.FromArgb(84, 130, 53));
                ExcelHelper.SetTextColor(worksheet.Cells[rowTrackerForColoring, 3, rowTrackerForColoring + 8, simulationYears.Count + 2], Color.White);

                currentCell.Row += 2;
                currentCell.Column = 1;
                worksheet.Cells[currentCell.Row, currentCell.Column].Value = BAMSConstants.TotalBridgeCareBudget;
                var budgetTotalRow = currentCell.Row;
                var budgetDetails = yearlyBudgets[summaryData.Budget];
                var yearTracker = 0;
                foreach (var item in budgetDetails.BudgetAmounts)
                {
                    var cellFortotalBudget = yearTracker;
                    var currValue = worksheet.Cells[currentCell.Row, currentCell.Column + cellFortotalBudget + 2].Value;
                    decimal totalCost = 0;
                    if (currValue != null)
                    {
                        totalCost = (decimal)currValue;
                    }
                    totalCost += item.Value;
                    worksheet.Cells[currentCell.Row, currentCell.Column + cellFortotalBudget + 2].Value = totalCost;
                    yearTracker++;
                }
                // Total (All Years) for Bridge Care Budget
                var cellTotalBridgeCareBudgetAllYears = worksheet.Cells[currentCell.Row, currentCell.Column + numberOfYears + 2];
                cellTotalBridgeCareBudgetAllYears.Formula = ExcelFormulas.Sum(worksheet.Cells[currentCell.Row, currentCell.Column + 2, currentCell.Row, currentCell.Column + numberOfYears + 1]);
                ExcelHelper.ApplyColor(cellTotalBridgeCareBudgetAllYears, Color.FromArgb(217, 217, 217));
                ExcelHelper.ApplyBorder(cellTotalBridgeCareBudgetAllYears);
                ExcelHelper.SetCustomFormat(cellTotalBridgeCareBudgetAllYears, ExcelHelperCellFormat.NegativeCurrency);                              
                
                ExcelHelper.ApplyBorder(worksheet.Cells[initialRow, currentCell.Column, currentCell.Row, simulationYears.Count + 2]);
                ExcelHelper.SetCustomFormat(worksheet.Cells[initialRow + 1, currentCell.Column + 2, initialRow + 1, simulationYears.Count + 2], ExcelHelperCellFormat.NegativeCurrency);
                
                ExcelHelper.ApplyColor(worksheet.Cells[currentCell.Row, currentCell.Column + 2, currentCell.Row, simulationYears.Count + 2],
                    Color.FromArgb(84, 130, 53));
                ExcelHelper.SetTextColor(worksheet.Cells[currentCell.Row, currentCell.Column + 2, currentCell.Row, simulationYears.Count + 2], Color.White);
                ExcelHelper.SetCustomFormat(worksheet.Cells[currentCell.Row, currentCell.Column + 2, currentCell.Row, simulationYears.Count + 2], ExcelHelperCellFormat.NegativeCurrency);
                currentCell.Row++;

                FillWorkTypeTotalWorkOutsideScope(worksheet, currentCell, startYear, simulationYears, workTypeTotal.WorkOutsideScopeCostPerYear);

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

                var currentRow = currentCell.Row + 1;
                currentCell.Row = budgetAnalysisRow;
                _bridgeWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Budget Analysis", "");                
                currentCell.Column = 1;

                int startRow, startColumn, row, column;
                _bridgeWorkSummaryCommon.SetRowColumns(currentCell, out startRow, out startColumn, out row, out column);
                var rowTitles = new List<string> { BAMSConstants.RemainingBudget, BAMSConstants.PercentBudgetSpentBAMS, BAMSConstants.PercentBudgetSpentCommitted, BAMSConstants.PercentBudgetSpentMPMS, BAMSConstants.PercentBudgetSpentSAP, BAMSConstants.PercentBudgetSpentProjectBuilder };
                for (int index = 0; index < rowTitles.Count; index++)
                {
                    worksheet.Cells[row++, column].Value = rowTitles[index];
                }
                column++;

                var fromColumn = column + 1;
                yearTracker = 0;
                foreach (var budgetData in budgetDetails.BudgetAmounts)
                {
                    row = startRow;
                    column = ++column;

                    var year = startYear + yearTracker;                    
                    var perYearTotalSpent = totalSpent.Find(_ => _.year == year);                    
                    var perYearTotalSpentAmount = perYearTotalSpent.amount;
                                        
                    // Remaining
                    var yearlyBudget = Convert.ToDecimal(worksheet.Cells[budgetTotalRow, column].Value);
                    worksheet.Cells[row, column].Value = yearlyBudget - perYearTotalSpentAmount;
                    row++;

                    var committedBudgetTotal = totalBudgetPerYearForCommitted[year];
                    var mpmsBudgetTotal = totalBudgetPerYearForMPMS[year];
                    var sapBudgetTotal = totalBudgetPerYearForSAP[year];
                    var projectBuilderBudgetTotal = totalBudgetPerYearForProjectBuilder[year];
                    var bamsBudgetTotal = perYearTotalSpentAmount - (committedBudgetTotal + mpmsBudgetTotal + sapBudgetTotal + projectBuilderBudgetTotal);
                    var categoryBudgetTotals = new decimal[] { bamsBudgetTotal, committedBudgetTotal, mpmsBudgetTotal, sapBudgetTotal, projectBuilderBudgetTotal };
                    // Budget spent in each category
                    for (int index = 0; index < categoryBudgetTotals.Length; index++)
                    {
                        // Calculate percentage
                        var percentage = categoryBudgetTotals[index] / yearlyBudget;
                        worksheet.Cells[row++, column].Value = percentage;
                    }
                    yearTracker++;
                }

                ExcelHelper.ApplyBorder(worksheet.Cells[currentCell.Row, currentCell.Column, currentCell.Row + 5, simulationYears.Count + 2]);

                ExcelHelper.SetCustomFormat(worksheet.Cells[currentCell.Row + 1, currentCell.Column + 2,
                    currentCell.Row + 5, simulationYears.Count + 2], ExcelHelperCellFormat.Percentage);
                ExcelHelper.SetCustomFormat(worksheet.Cells[currentCell.Row, currentCell.Column + 2,
                    currentCell.Row, simulationYears.Count + 2], ExcelHelperCellFormat.NegativeCurrency);

                ExcelHelper.ApplyColor(worksheet.Cells[currentCell.Row, currentCell.Column + 2, currentCell.Row, simulationYears.Count + 2],
                    Color.Red);
                ExcelHelper.ApplyColor(worksheet.Cells[currentCell.Row + 1, currentCell.Column + 2, currentCell.Row + 5, simulationYears.Count + 2], Color.FromArgb(248, 203, 173));
                currentCell.Row = currentRow;
            }

            worksheet.Calculate();
            worksheet.Cells.AutoFitColumns();

            // local function
            void insertTotalAndPercentagePerCategory(ExcelWorksheet worksheet, CurrentCell currentCell, int numberOfYears, int firstContentRow)
            {
                var startColumnIndex = 3;
                var totalSpentRow = currentCell.Row;

                worksheet.Cells[totalSpentRow, startColumnIndex + numberOfYears].Formula = ExcelFormulas.Sum(totalSpentRow, startColumnIndex, totalSpentRow, startColumnIndex + numberOfYears - 1);
                for (var row = firstContentRow; row <= currentCell.Row - 1; row++)
                {
                    // Add Total(all years)
                    worksheet.Cells[row, startColumnIndex + numberOfYears].Formula = ExcelFormulas.Sum(row, startColumnIndex, row, startColumnIndex + numberOfYears - 1);

                    // Percentage
                    worksheet.Cells[row, startColumnIndex + numberOfYears + 1].Formula = ExcelFormulas.Percentage(row, startColumnIndex + numberOfYears, totalSpentRow, startColumnIndex + numberOfYears);

                    // Text
                    worksheet.Cells[row, startColumnIndex + numberOfYears + 2].Value = $"Percentage Spent on " + worksheet.Cells[row, 1].Value.ToString().ToUpper();
                }
                var excelRange = worksheet.Cells[firstContentRow, numberOfYears + 3, firstContentRow + 8, numberOfYears + 3];
                ExcelHelper.ApplyColor(excelRange, Color.FromArgb(217, 217, 217));
                ExcelHelper.ApplyBorder(excelRange);
                ExcelHelper.SetCustomFormat(worksheet.Cells[firstContentRow, numberOfYears + 4, firstContentRow + 6, numberOfYears + 4], ExcelHelperCellFormat.Percentage);
            }
        }

        private void FillWorkTypeTotalWorkOutsideScope(ExcelWorksheet worksheet, CurrentCell currentCell, int startYear, List<int> simulationYears, Dictionary<int, double> workOutsideScopeCostPerYear)
        {
            _bridgeWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "", TreatmentCategory.WorkOutsideScope.ToSpreadsheetString());

            var startColumnIndex = 3;
            var workOutsideScopeRow = ++currentCell.Row;
            var numberOfYears = simulationYears.Count;
            var contentColor = Color.FromArgb(84, 130, 53);

            foreach (var item in workOutsideScopeCostPerYear)
            {
                FillTheExcelColumns(startYear, item, workOutsideScopeRow, worksheet);
            }
            var workOutsideScopeRowRangeForBorder = worksheet.Cells[workOutsideScopeRow, 1, workOutsideScopeRow, startColumnIndex + numberOfYears];
            ExcelHelper.ApplyBorder(workOutsideScopeRowRangeForBorder);
            var workOutsideScopeRowRange = worksheet.Cells[workOutsideScopeRow, startColumnIndex, workOutsideScopeRow, startColumnIndex + numberOfYears - 1];
            ExcelHelper.ApplyColor(workOutsideScopeRowRange, contentColor);
            ExcelHelper.SetCustomFormat(workOutsideScopeRowRange, ExcelHelperCellFormat.NegativeCurrency);
            ExcelHelper.SetTextColor(workOutsideScopeRowRange, Color.White);

            // Total cell
            var workOutsideScopeRowTotalCell = worksheet.Cells[workOutsideScopeRow, startColumnIndex + numberOfYears];
            workOutsideScopeRowTotalCell.Formula = ExcelFormulas.Sum(workOutsideScopeRow, startColumnIndex, workOutsideScopeRow, startColumnIndex + numberOfYears - 1);            
            ExcelHelper.ApplyColor(workOutsideScopeRowTotalCell, Color.FromArgb(217, 217, 217));
            ExcelHelper.ApplyBorder(workOutsideScopeRowTotalCell);
            ExcelHelper.SetCustomFormat(workOutsideScopeRowTotalCell, ExcelHelperCellFormat.NegativeCurrency);
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
            worksheet.Cells[firstBPNRow + bpnNames.Count(), 1].Value = BAMSConstants.TotalBPNCost;

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
            foreach (var item in workTypeTotal.MaintenanceCostPerYear)
            {
                FillTheExcelColumns(startYear, item, firstContentRow, worksheet);
            }
            firstContentRow++;
            foreach (var item in workTypeTotal.PreservationCostPerYear)
            {
                FillTheExcelColumns(startYear, item, firstContentRow, worksheet);
            }
            firstContentRow++;
            foreach (var item in workTypeTotal.RehabilitationCostPerYear)
            {
                FillTheExcelColumns(startYear, item, firstContentRow, worksheet);
            }
            firstContentRow++;
            foreach (var item in workTypeTotal.ReplacementCostPerYear)
            {
                FillTheExcelColumns(startYear, item, firstContentRow, worksheet);
            }
            firstContentRow++;
            foreach (var item in workTypeTotal.CapacityAddingCostPerYear)
            {
                FillTheExcelColumns(startYear, item, firstContentRow, worksheet);
            }
            firstContentRow++;
            foreach (var item in workTypeTotal.OtherCostPerYear)
            {
                FillTheExcelColumns(startYear, item, firstContentRow, worksheet);
            }
            firstContentRow++;            
            foreach (var item in workTypeTotal.BundledCostPerYear)
            {
                FillTheExcelColumns(startYear, item, firstContentRow, worksheet);
            }
            firstContentRow++;
            // Add data for BAMS Work Type Totals "Total Spent"
            worksheet.Cells[firstContentRow, 1].Value = BAMSConstants.TotalSpent;
            TotalSpentRow = firstContentRow;
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
