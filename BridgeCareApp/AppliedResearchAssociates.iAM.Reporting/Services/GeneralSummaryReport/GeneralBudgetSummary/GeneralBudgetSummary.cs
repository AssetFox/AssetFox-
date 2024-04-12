using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.ExcelHelpers;
using AppliedResearchAssociates.iAM.Reporting.Models;
using AppliedResearchAssociates.iAM.Reporting.Models.BAMSSummaryReport;
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

            Dictionary<string, List<(decimal AllocatedAmount, int Year)>> listOfBudgetsSpent = new Dictionary<string, List<(decimal, int)>>();
            foreach (var year in simulationOutput.Years)
            {
                finalRow = 1;
                // Initialize total spent for the current year
                decimal totalYearlySpent = 0;

                // Iterate over the assets for the current year
                foreach (var asset in year.Assets)
                {
                    // Check if the asset contains treatment considerations
                    if (asset.TreatmentConsiderations != null && asset.TreatmentConsiderations.Any())
                    {
                        // Iterate over the treatment considerations
                        foreach (var treatmentConsideration in asset.TreatmentConsiderations)
                        {
                            // Check if the treatment has funding data
                            if (treatmentConsideration.FundingCalculationOutput != null)
                            {
                                Dictionary<(string Name, int Year), decimal> aggregatedAmounts = new Dictionary<(string, int), decimal>();

                                foreach (var fundedTreatment in treatmentConsideration.FundingCalculationOutput.AllocationMatrix)
                                {
                                    decimal totalAllocatedAmount = 0;
                                    var key = (fundedTreatment.BudgetName, fundedTreatment.Year);

                                    // Iterate over the allocation matrices
                                    foreach (var allocationMatrix in treatmentConsideration.FundingCalculationOutput.AllocationMatrix)
                                    {
                                        if(allocationMatrix.BudgetName == key.BudgetName)
                                        {
                                            // Sum up the allocated amounts
                                            totalAllocatedAmount += allocationMatrix.AllocatedAmount;
                                        }
                                    }
                                    int rowNumber = budgetMap[fundedTreatment.BudgetName];

                                    // Add or update the aggregated amount for the current budget name and year
                                    if (aggregatedAmounts.ContainsKey(key))
                                    {
                                        aggregatedAmounts[key] = totalAllocatedAmount;
                                    }
                                    else
                                    {
                                        aggregatedAmounts[key] = totalAllocatedAmount;
                                    }

                                    // Add treatment cost to the total
                                    if (totalYearlySpent == 0)
                                    {
                                        totalYearlySpent += totalAllocatedAmount;
                                    }
                                    // Add cost to the worksheet
                                    generalSummaryWorksheet.Cells[rowNumber, currentYearColumn].Value = aggregatedAmounts[key];
                                    ExcelHelper.ApplyBorder(generalSummaryWorksheet.Cells[rowNumber, currentYearColumn, rowNumber, currentYearColumn]);

                                    // Move to the next row for the next treatment
                                    currentRow++;
                                }

                                foreach (var kvp in aggregatedAmounts)
                                {
                                    // Check if an entry already exists for the same name
                                    if (listOfBudgetsSpent.TryGetValue(kvp.Key.Name, out var existingEntries))
                                    {
                                        // Check if any existing entry has the same year
                                        bool yearExists = existingEntries.Any(entry => entry.Year == kvp.Key.Year);

                                        if (!yearExists)
                                        {
                                            // Year does not exist, add the new value to the existing list
                                            listOfBudgetsSpent[kvp.Key.Name].Add((kvp.Value, kvp.Key.Year));
                                        }
                                    }
                                    else
                                    {
                                        // Entry does not exist, create a new list and add the new entry
                                        var newList = new List<(decimal, int)>();
                                        newList.Add((kvp.Value, kvp.Key.Year));
                                        listOfBudgetsSpent.Add(kvp.Key.Name, newList);
                                    }
                                }
                                finalRow = currentRow;
                                currentCell.Row = currentRow;
                                currentCell.Column = currentYearColumn;
                            }
                        }
                    }
                }
                generalSummaryWorksheet.Cells[firstRow + simulationOutput.Years[0].Budgets.Count + 1, currentYearColumn].Value = totalYearlySpent;
                ExcelHelper.ApplyBorder(generalSummaryWorksheet.Cells[firstRow + simulationOutput.Years[0].Budgets.Count + 1, currentYearColumn]);

                // Move to the next column
                currentYearColumn++;


                currentRow = firstRow + 1;
                ExcelHelper.ApplyBorder(generalSummaryWorksheet.Cells[simulationOutput.Years[0].Assets.Count + 3, currentYearColumn - 1, simulationOutput.Years[0].Assets.Count + 3, currentYearColumn - 1]);

                currentCell.Row = finalRow;
            }
            int firstYear = simulationOutput.Years[0].Year;
            foreach (var kvp in budgetMap)
            {
                // Get the corresponding row number and budget name
                int rowNumber = kvp.Value;
                string budgetName = kvp.Key;

                // Check if the budget name exists in listOfBudgetsSpent
                if (listOfBudgetsSpent.ContainsKey(budgetName))
                {
                    foreach (var listofBudgets in listOfBudgetsSpent[budgetName])
                    {
                        var eachAmount = listofBudgets.AllocatedAmount;
                        var eachYear = listofBudgets.Year - firstYear;
                        eachYear = eachYear + 2;
                        generalSummaryWorksheet.Cells[rowNumber, eachYear].Value = eachAmount;
                    }
                }
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
