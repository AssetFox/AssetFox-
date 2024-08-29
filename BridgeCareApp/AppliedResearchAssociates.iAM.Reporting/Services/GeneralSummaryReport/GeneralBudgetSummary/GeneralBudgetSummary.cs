﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.ExcelHelpers;
using AppliedResearchAssociates.iAM.Reporting.Models;
using AppliedResearchAssociates.iAM.Reporting.Models.BAMSSummaryReport;
using AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport;
using AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport.BridgeWorkSummaryByBudget;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Excel.Functions.RefAndLookup;

namespace AppliedResearchAssociates.iAM.Reporting.Services.GeneralSummaryReport.GeneralBudgetSummary
{
    public class GeneralBudgetSummary
    {

        private ReportHelper _reportHelper;
        private readonly IUnitOfWork _unitOfWork;

        public GeneralBudgetSummary(IList<string> Warnings, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _reportHelper = new ReportHelper(_unitOfWork);
        }

        public void FillTargetBudgets(ExcelWorksheet generalSummaryWorksheet, SimulationOutput simulationOutput, CurrentCell currentCell)
        {
            generalSummaryWorksheet.Column(1).SetTrueWidth(20);
            currentCell.Row = currentCell.Row;
            generalSummaryWorksheet.Cells[currentCell.Row, 1].Value = "Target Budgets";
            currentCell.Row++;
            int startingColumn = 1;

            int currentRow = currentCell.Row + 1;

            var budgets = _reportHelper.GetBudgets(simulationOutput.Years);

            startingColumn = startingColumn + 1;
            foreach (var year in simulationOutput.Years)
            {
                generalSummaryWorksheet.Cells[currentCell.Row, startingColumn].Value = year.Year;
                ExcelHelper.ApplyBorder(generalSummaryWorksheet.Cells[currentRow - 1, startingColumn, currentRow - 1, startingColumn]);
                startingColumn++;
            }
            int finalColumn = startingColumn;
            startingColumn = 1;

            //Set Column widths
            for (int i = 0; i < simulationOutput.Years.Count; i++)
            {
                generalSummaryWorksheet.Column(startingColumn + 1).SetTrueWidth(12);
                i++;
                startingColumn++;
            }
            startingColumn = 1;
            foreach (var budget in budgets)
            {
                generalSummaryWorksheet.Cells[currentRow, startingColumn].Value = budget;
                ExcelHelper.ApplyBorder(generalSummaryWorksheet.Cells[currentRow, startingColumn, currentRow, startingColumn]);
                currentRow++;
            }
            generalSummaryWorksheet.Cells[currentRow, startingColumn].Value = "Total Budget Spent";
            // Reset currentRow and column for writing yearly spent
            currentRow = currentCell.Row + 1;
            startingColumn = 1;

            int firstRow = currentCell.Row;

            int currentYearColumn = startingColumn + 1;

            foreach (var year in simulationOutput.Years)
            {
                int finalRow = 1;

                decimal totalYearlySpent = 0; 
                foreach (var budget in year.Budgets)
                {
                    if (budget.AvailableFunding != null)
                    {
                            // Write budget name and spent amount to the worksheet
                            generalSummaryWorksheet.Cells[currentRow, currentYearColumn].Value = budget.AvailableFunding;
                            totalYearlySpent += budget.AvailableFunding;
                            ExcelHelper.ApplyBorder(generalSummaryWorksheet.Cells[currentRow, currentYearColumn, currentRow, currentYearColumn]);

                        // Move to the next row
                        currentRow++;
                        finalRow = currentRow;
                        ExcelHelper.ApplyBorder(generalSummaryWorksheet.Cells[firstRow - 1, 1, firstRow - 1, finalColumn - 1]);
                    }
                }
                ExcelHelper.MergeCells(generalSummaryWorksheet, firstRow - 1, 1, firstRow - 1, finalColumn - 1);
                // Write total yearly spent at the bottom of the column
                generalSummaryWorksheet.Cells[finalRow, startingColumn + 1].Value = totalYearlySpent;
                ExcelHelper.ApplyBorder(generalSummaryWorksheet.Cells[finalRow, startingColumn, finalRow, startingColumn + 1]);

                // Reset currentRow for the next column
                currentYearColumn++;
                currentRow = firstRow + 1;

                // Move to the next column for the next year
                startingColumn++;
                currentCell.Row = finalRow;
            }
            FillBudgetSpent(generalSummaryWorksheet, simulationOutput, currentCell);
        }
        public void FillBudgetSpent(ExcelWorksheet generalSummaryWorksheet, SimulationOutput simulationOutput, CurrentCell currentCell)
        {
            generalSummaryWorksheet.Column(1).SetTrueWidth(20);
            generalSummaryWorksheet.Cells[currentCell.Row + 2, 1].Value = "Budget Spent";
            currentCell.Row++;
            int startingColumn = currentCell.Column;

            int currentRow = currentCell.Row + 2;
            var budgets = _reportHelper.GetBudgets(simulationOutput.Years);

            startingColumn = startingColumn + 1;
            int firstRow = currentCell.Row + 2;
            currentCell.Row = currentCell.Row + 2;
            foreach (var year in simulationOutput.Years)
            {
                generalSummaryWorksheet.Cells[currentCell.Row, startingColumn].Value = year.Year;
                ExcelHelper.ApplyBorder(generalSummaryWorksheet.Cells[currentRow, startingColumn, currentRow, startingColumn]);
                startingColumn++;
            }

            int finalColumn = startingColumn;
            startingColumn = currentCell.Column;
            currentRow = currentRow + 1;
            int assetCount = simulationOutput.Years[0].Assets.Count;

            Dictionary<string, int> budgetMap = new Dictionary<string, int>();
            foreach (var budget in simulationOutput.Years[0].Budgets)
            {
                budgetMap.Add(budget.BudgetName, currentRow);
                generalSummaryWorksheet.Cells[currentRow, startingColumn].Value = budget.BudgetName;
                ExcelHelper.ApplyBorder(generalSummaryWorksheet.Cells[currentRow, startingColumn, currentRow, startingColumn]);
                currentRow++;
            }
            generalSummaryWorksheet.Cells[currentRow, startingColumn].Value = "Total Target Budgets";
            ExcelHelper.ApplyBorder(generalSummaryWorksheet.Cells[currentRow, 1, currentRow, startingColumn]);

            // Reset currentRow for writing yearly spent
            currentRow = firstRow + 1;
            // Initialize the column index for the current year
            int currentYearColumn = startingColumn + 1;

            var workSummaryByBudgetData = new List<WorkSummaryByBudgetModel>();

            var budgetsList = new HashSet<string>();
            int finalRow = 1;

            Dictionary<double, List<TreatmentConsiderationDetail>> keyCashFlowFundingDetails = new Dictionary<double, List<TreatmentConsiderationDetail>>();
            // setting up model to store data. This will be used to fill up Bridge Work Summary By
            // Budget TAB
            var workSummaryByBudgets = new List<WorkSummaryByBudgetModel>();

            var workBudgets = new HashSet<string>();
            foreach (var yearData in simulationOutput.Years)
            {
                foreach (var item in yearData.Budgets)
                {
                    workBudgets.Add(item.BudgetName);
                }
            }
            foreach (var item in workBudgets)
            {
                workSummaryByBudgetData.Add(new WorkSummaryByBudgetModel
                {
                    Budget = item,
                    YearlyData = new List<YearsData>()
                });
            }

            foreach (var summaryData in workSummaryByBudgetData)
            {
                foreach (var yearData in simulationOutput.Years)
                {
                    var assets = yearData.Assets.Where(_ => _.TreatmentCause != TreatmentCause.NoSelection);
                    foreach (var section in assets)
                    {
                        var section_BRKEY = _reportHelper.CheckAndGetValue<double>(section.ValuePerNumericAttribute, "BRKEY_");

                        // Build keyCashFlowFundingDetails                    
                        if (section.TreatmentStatus != TreatmentStatus.Applied)
                        {
                            var fundingSection = yearData.Assets.
                                                  FirstOrDefault(_ => _reportHelper.CheckAndGetValue<double>(_.ValuePerNumericAttribute, "BRKEY_") == section_BRKEY &&
                                                                _.TreatmentCause == TreatmentCause.SelectedTreatment &&
                                                                _.AppliedTreatment.ToLower() != BAMSConstants.NoTreatment &&
                                                                _.AppliedTreatment == section.AppliedTreatment);
                            if (fundingSection != null)
                            {
                                if (!keyCashFlowFundingDetails.ContainsKey(section_BRKEY))
                                {
                                    keyCashFlowFundingDetails.Add(section_BRKEY, fundingSection.TreatmentConsiderations ?? new());
                                }
                                else
                                {
                                    keyCashFlowFundingDetails[section_BRKEY].AddRange(fundingSection.TreatmentConsiderations);
                                }
                            }
                        }

                        // If CF then use obj from keyCashFlowFundingDetails otherwise from section
                        var treatmentConsiderations = ((section.TreatmentCause == TreatmentCause.SelectedTreatment &&
                                                      section.TreatmentStatus == TreatmentStatus.Progressed) ||
                                                      (section.TreatmentCause == TreatmentCause.CashFlowProject &&
                                                      section.TreatmentStatus == TreatmentStatus.Progressed) ||
                                                      (section.TreatmentCause == TreatmentCause.CashFlowProject &&
                                                      section.TreatmentStatus == TreatmentStatus.Applied)) ?
                                                      keyCashFlowFundingDetails[section_BRKEY] :
                                                      section.TreatmentConsiderations ?? new();
                        var treatmentConsideration = treatmentConsiderations.FirstOrDefault(_ => _.FundingCalculationOutput != null &&
                                                     _.FundingCalculationOutput.AllocationMatrix.Any(_ => _.Year == yearData.Year));
                        var appliedTreatment = treatmentConsideration?.TreatmentName ?? section.AppliedTreatment;
                        var budgetAmount = (double)treatmentConsiderations.Sum(_ =>
                                                           (_.FundingCalculationOutput?.AllocationMatrix
                                                            .Where(b => b.BudgetName == summaryData.Budget)
                                                            .Sum(bu => bu.AllocatedAmount)) ?? 0);
                        var bpnName = _reportHelper.CheckAndGetValue<string>(section?.ValuePerTextAttribute, "BUS_PLAN_NETWORK");
                        if (section.TreatmentCause == TreatmentCause.CommittedProject &&
                            appliedTreatment.ToLower() != BAMSConstants.NoTreatment)
                        {
                            summaryData.YearlyData.Add(new YearsData
                            {
                                Year = yearData.Year,
                                Treatment = appliedTreatment,
                                Amount = budgetAmount,
                                isCommitted = true,
                                costPerBPN = (bpnName, budgetAmount),
                            });
                        }
                        }
                    }
            }

            Dictionary<string, List<(decimal AllocatedAmount, int Year)>> listOfBudgetsSpent = new Dictionary<string, List<(decimal, int)>>();
            var simulationYear = simulationOutput.Years.First().Year;
            foreach (var year in simulationOutput.Years)
            {
                 double totalYearlySpent = 0;

                foreach (var item in workSummaryByBudgetData)
                {
                    if(item.YearlyData.Count > 0)
                    {
                        if (item.YearlyData[0].Year == simulationYear)
                        {
                            generalSummaryWorksheet.Cells[currentRow + 1, currentYearColumn].Value = item.YearlyData[0].Amount;
                            string budgetName = item.YearlyData[0].Treatment;
                            decimal allocatedAmount = (decimal)item.YearlyData[0].Amount;
                            int itemYear = item.YearlyData[0].Year;

                            if (listOfBudgetsSpent.ContainsKey(budgetName))
                            {
                                // If the budget name exists, add the allocated amount and year to the existing list
                                listOfBudgetsSpent[budgetName].Add((allocatedAmount, itemYear));
                            }
                            else
                            {
                                // If the budget name doesn't exist, create a new list and add the allocated amount and year
                                listOfBudgetsSpent[budgetName] = new List<(decimal, int)>() { (allocatedAmount, itemYear) };
                            }
                            totalYearlySpent += item.YearlyData[0].Amount;
                            ExcelHelper.ApplyBorder(generalSummaryWorksheet.Cells[currentRow, currentYearColumn, currentRow, currentYearColumn]);
                        }

                    }
                    currentRow++;
                }
                generalSummaryWorksheet.Cells[firstRow + simulationOutput.Years[0].Budgets.Count + 1, currentYearColumn].Value = totalYearlySpent;
                ExcelHelper.ApplyBorder(generalSummaryWorksheet.Cells[firstRow + simulationOutput.Years[0].Budgets.Count + 1, currentYearColumn]);
                currentRow = firstRow;
                currentYearColumn++;
                simulationYear = simulationYear + 1;
            }

            // Get the dimensions of the worksheet
            int rowCount = generalSummaryWorksheet.Dimension.Rows;
            int colCount = generalSummaryWorksheet.Dimension.Columns;

            // Iterate through each row and column
            for (int row = firstRow + 1; row <= firstRow + simulationOutput.Years[0].Budgets.Count; row++)
            {
                for (int col = 1; col <= colCount; col++)
                {
                    // Get the cell value
                    var cellValue = generalSummaryWorksheet.Cells[row, col].Value;

                    // Check if the cell is empty
                    if (cellValue == null)
                    {
                        // If the cell is empty, set its value to 0
                        generalSummaryWorksheet.Cells[row, col].Value = 0;
                        ExcelHelper.ApplyBorder(generalSummaryWorksheet.Cells[row, col, row, col]);
                    }
                }
            }

            currentCell.Row = firstRow + simulationOutput.Years[0].Budgets.Count;
            ExcelHelper.MergeCells(generalSummaryWorksheet, firstRow - 1, 1, firstRow - 1, finalColumn - 1);
            ExcelHelper.ApplyBorder(generalSummaryWorksheet.Cells[firstRow - 1, 1, firstRow, finalColumn - 1]);
            FillBudgetRemaining(generalSummaryWorksheet, simulationOutput, currentCell, listOfBudgetsSpent);
        }

        public void FillBudgetRemaining(ExcelWorksheet generalSummaryWorksheet, SimulationOutput simulationOutput, CurrentCell currentCell, Dictionary<string, List<(decimal AllocatedAmount, int Year)>> listOfBudgetsSpent)
        {
            currentCell.Row = currentCell.Row + 3;
            generalSummaryWorksheet.Cells[currentCell.Row, 1].Value = "Budget Remaining";
            currentCell.Row++;
            int startingColumn = 1;

            int currentRow = currentCell.Row + 1;

            var budgets = _reportHelper.GetBudgets(simulationOutput.Years);

            startingColumn = startingColumn + 1;
            foreach (var year in simulationOutput.Years)
            {
                generalSummaryWorksheet.Cells[currentCell.Row, startingColumn].Value = year.Year;
                ExcelHelper.ApplyBorder(generalSummaryWorksheet.Cells[currentRow - 1, startingColumn, currentRow - 1, startingColumn]);
                startingColumn++;
            }
            int finalColumn = startingColumn;
            startingColumn = 1;
            foreach (var budget in simulationOutput.Years[0].Budgets)
            {
                generalSummaryWorksheet.Cells[currentRow, startingColumn].Value = budget.BudgetName;
                ExcelHelper.ApplyBorder(generalSummaryWorksheet.Cells[currentRow, startingColumn, currentRow, startingColumn]);
                currentRow++;
            }
            generalSummaryWorksheet.Cells[currentRow, startingColumn].Value = "Total Budget Remaining";
            // Reset currentRow and column for writing yearly spent
            currentRow = currentCell.Row + 1;
            startingColumn = 1;

            int firstRow = currentCell.Row;
            int currentYearColumn = startingColumn + 1;
            foreach (var year in simulationOutput.Years)
            {
                int finalRow = 1;
                decimal totalYearlySpent = 0; // Initialize total spent for the current year

                // Iterate over the assets for the current year
                foreach (var budget in year.Budgets)
                {
                    // Check if the asset contains treatment considerations
                    if (budget.AvailableFunding != null)
                    {
                        var budgetsSpent = budget.AvailableFunding;
                        var treatmentName = budget.BudgetName;
                            if (listOfBudgetsSpent.ContainsKey(treatmentName))
                            {
                                // Find the entry with the matching year
                                var matchingEntry = listOfBudgetsSpent[treatmentName].FirstOrDefault(entry => entry.Year == year.Year);

                                if (matchingEntry != default)
                                {
                                    // Entry with the matching year found
                                    decimal currentAllocatedAmount = matchingEntry.AllocatedAmount;

                                    // Subtract budgetSpent.Amount from the current allocated amount
                                    decimal updatedAllocatedAmount = budgetsSpent - currentAllocatedAmount;

                                    generalSummaryWorksheet.Cells[currentRow, currentYearColumn].Value = updatedAllocatedAmount;
                                    ExcelHelper.ApplyBorder(generalSummaryWorksheet.Cells[currentRow, currentYearColumn, currentRow, currentYearColumn]);
                                }
                                else
                                {
                                    // Write budget name and spent amount to the worksheet
                                    generalSummaryWorksheet.Cells[currentRow, currentYearColumn].Value = budgetsSpent;
                                    ExcelHelper.ApplyBorder(generalSummaryWorksheet.Cells[currentRow, currentYearColumn, currentRow, currentYearColumn]);

                                    // Increment total yearly spent
                                    totalYearlySpent += budgetsSpent;

                                }
                            }
                            else
                            {
                                // Write budget name and spent amount to the worksheet
                                generalSummaryWorksheet.Cells[currentRow, currentYearColumn].Value = budgetsSpent;
                                ExcelHelper.ApplyBorder(generalSummaryWorksheet.Cells[currentRow, currentYearColumn, currentRow, currentYearColumn]);

                                // Increment total yearly spent
                                totalYearlySpent += budgetsSpent;
                            }

                        // Move to the next row
                        currentRow++;
                        finalRow = currentRow;
                        ExcelHelper.ApplyBorder(generalSummaryWorksheet.Cells[firstRow - 1, 1, firstRow - 1, finalColumn - 1]);
                    }
                }
                currentRow = firstRow;
                currentYearColumn++;
                ExcelHelper.MergeCells(generalSummaryWorksheet, firstRow - 1, 1, firstRow - 1, finalColumn - 1);
                // Write total yearly spent at the bottom of the column
                generalSummaryWorksheet.Cells[finalRow, startingColumn + 1].Value = totalYearlySpent;
                ExcelHelper.ApplyBorder(generalSummaryWorksheet.Cells[finalRow, startingColumn, finalRow, startingColumn + 1]);

                // Reset currentRow for the next column
                currentRow = currentRow + 1;

                // Move to the next column for the next year
                startingColumn++;
                currentCell.Row = finalRow;
            }
        }
    }
}
