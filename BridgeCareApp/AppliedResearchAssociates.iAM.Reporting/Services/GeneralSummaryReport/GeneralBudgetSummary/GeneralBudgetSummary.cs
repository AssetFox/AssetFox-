using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.ExcelHelpers;
using AppliedResearchAssociates.iAM.Reporting.Models;
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

        public void FillTargetBudgets(ExcelWorksheet generalSummaryWorksheet, SimulationOutput simulationOutput)
        {
            CurrentCell currentCell = new CurrentCell { Row = 1, Column = 1 };
            generalSummaryWorksheet.Cells[1, 1].Value = "Target Budgets";
            currentCell.Row++;
            int startingColumn = currentCell.Column;

            int currentRow = currentCell.Row;
            var budgets = _reportHelper.GetBudgets(simulationOutput.Years);

            startingColumn = startingColumn + 1;
            int firstRow = currentCell.Row;
            foreach (var year in simulationOutput.Years)
            {
                generalSummaryWorksheet.Cells[currentCell.Row, startingColumn].Value = year.Year;
                ExcelHelper.ApplyBorder(generalSummaryWorksheet.Cells[currentRow - 1, startingColumn, currentRow - 1, startingColumn]);
                startingColumn++;
            }
            int finalColumn = startingColumn;
            startingColumn = currentCell.Column;
            currentRow = currentRow + 1;
            int assetCount = simulationOutput.Years[0].Assets.Count;
            int budgetIndex = 0;

            for (int i = 0; i < assetCount; i++)
            {
                foreach (var budget in budgets)
                {
                    // Add budget to worksheet only if there are more budgets than assets
                    if (budgetIndex < assetCount)
                    {
                        generalSummaryWorksheet.Cells[currentRow, startingColumn].Value = budget;
                        ExcelHelper.ApplyBorder(generalSummaryWorksheet.Cells[currentRow, startingColumn, currentRow, startingColumn]);
                        currentRow++;
                        budgetIndex++;
                    }
                }
            }
            generalSummaryWorksheet.Cells[currentRow, startingColumn].Value = "Total Target Budgets";
            ExcelHelper.ApplyBorder(generalSummaryWorksheet.Cells[currentRow, 1, currentRow, startingColumn]);

            // Reset currentRow for writing yearly spent
            currentRow = firstRow + 1;
            // Initialize the column index for the current year
            int currentYearColumn = startingColumn + 1;

            // Iterate over the years
            foreach (var year in simulationOutput.Years)
            {
                int finalRow = 1;

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
                            if (treatmentConsideration.FundingCalculationInput != null && treatmentConsideration.FundingCalculationInput.CurrentBudgetsToSpend != null)
                            {
                                // Iterate over the treatments to fund
                                foreach (var fundedTreatment in treatmentConsideration.FundingCalculationInput.TreatmentsToFund)
                                {
                                    // Add treatment cost to the worksheet
                                    generalSummaryWorksheet.Cells[currentRow, currentYearColumn].Value = fundedTreatment.Cost;
                                    ExcelHelper.ApplyBorder(generalSummaryWorksheet.Cells[currentRow, currentYearColumn, currentRow, currentYearColumn]);

                                    // Increment total yearly spent
                                    totalYearlySpent += fundedTreatment.Cost;

                                    // Move to the next row for the next treatment
                                    currentRow++;

                                }
                                finalRow = currentRow;
                                currentCell.Row = currentRow;
                                currentCell.Column = currentYearColumn;

                            }
                        }
                    }
                }
                // Move to the next column
                currentYearColumn++;


                // Write total yearly spent at the bottom of the column
                generalSummaryWorksheet.Cells[finalRow, currentYearColumn].Value = totalYearlySpent;
                currentRow = firstRow + 1;
                ExcelHelper.ApplyBorder(generalSummaryWorksheet.Cells[finalRow, startingColumn, finalRow, startingColumn + 1]);

                currentCell.Row = finalRow;
            }
            ExcelHelper.MergeCells(generalSummaryWorksheet, 1, 1, 1, finalColumn);

            FillBudgetSpent(generalSummaryWorksheet, simulationOutput, currentCell);
        }
        public void FillBudgetSpent(ExcelWorksheet generalSummaryWorksheet, SimulationOutput simulationOutput, CurrentCell currentCell)
        {
            generalSummaryWorksheet.Column(1).SetTrueWidth(20);
            currentCell.Row = currentCell.Row + 7;
            generalSummaryWorksheet.Cells[currentCell.Row, 1].Value = "Budget Spent";
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

            foreach (var year in simulationOutput.Years)
            {
                int finalRow = 1;

                decimal totalYearlySpent = 0; // Initialize total spent for the current year

                // Iterate over the assets for the current year
                foreach (var asset in year.Assets)
                {
                    // Check if the asset contains treatment considerations
                    if (asset.TreatmentConsiderations != null && asset.TreatmentConsiderations.Any())
                    {
                        // Iterate over the treatment considerations
                        foreach (var treatmentConsideration in asset.TreatmentConsiderations)
                        {
                            int currentYearColumn = startingColumn + 1;
                            if (treatmentConsideration.FundingCalculationInput != null)
                            {
                                if (treatmentConsideration.FundingCalculationInput.CurrentBudgetsToSpend != null)
                                {
                                    // Iterate over the budget spent data
                                    foreach (var budgetSpent in treatmentConsideration.FundingCalculationInput.CurrentBudgetsToSpend)
                                    {
                                        // Write budget name and spent amount to the worksheet
                                        generalSummaryWorksheet.Cells[currentRow, currentYearColumn].Value = budgetSpent.Amount;
                                        ExcelHelper.ApplyBorder(generalSummaryWorksheet.Cells[currentRow, currentYearColumn, currentRow, currentYearColumn]);

                                        // Increment total yearly spent
                                        totalYearlySpent += budgetSpent.Amount;

                                        // Move to the next row
                                        currentRow++;
                                    }
                                    finalRow = currentRow;
                                    currentRow = firstRow + 1;
                                    currentYearColumn++;
                                    ExcelHelper.ApplyBorder(generalSummaryWorksheet.Cells[firstRow - 1, 1, firstRow - 1, finalColumn - 1]);
                                }

                            }
                        }
                    }
                }
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
            FillBudgetRemaining(generalSummaryWorksheet, simulationOutput, currentCell);
        }

        public void FillBudgetRemaining(ExcelWorksheet generalSummaryWorksheet, SimulationOutput simulationOutput, CurrentCell currentCell)
        {
            currentCell.Row = currentCell.Row + 2;
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
            foreach (var budget in budgets)
            {
                generalSummaryWorksheet.Cells[currentRow, startingColumn].Value = budget;
                ExcelHelper.ApplyBorder(generalSummaryWorksheet.Cells[currentRow, startingColumn, currentRow, startingColumn]);
                currentRow++;
            }
            generalSummaryWorksheet.Cells[currentRow, startingColumn].Value = "Total Budget Remaining";
            // Reset currentRow and column for writing yearly spent
            currentRow = currentCell.Row + 1;
            startingColumn = 1;

            int firstRow = currentCell.Row;

            foreach (var year in simulationOutput.Years)
            {
                int finalRow = 1;
                decimal totalYearlySpent = 0; // Initialize total spent for the current year

                // Iterate over the assets for the current year
                foreach (var asset in year.Assets)
                {
                    // Check if the asset contains treatment considerations
                    if (asset.TreatmentConsiderations != null && asset.TreatmentConsiderations.Any())
                    {
                        // Iterate over the treatment considerations
                        foreach (var treatmentConsideration in asset.TreatmentConsiderations)
                        {
                            int currentYearColumn = startingColumn + 1;
                            if (treatmentConsideration.FundingCalculationInput != null)
                            {
                                if (treatmentConsideration.FundingCalculationInput.CurrentBudgetsToSpend != null &&
                                    treatmentConsideration.FundingCalculationInput.TreatmentsToFund != null)
                                {
                                    var budgetsSpent = treatmentConsideration.FundingCalculationInput.CurrentBudgetsToSpend;
                                    var treatmentsFunded = treatmentConsideration.FundingCalculationInput.TreatmentsToFund;

                                    // Iterate over budgets spent and treatments funded
                                    for (int i = 0; i < budgetsSpent.Count; i++)
                                    {
                                            var budgetSpent = budgetsSpent[i];
                                            var treatmentFunded = treatmentsFunded[0];

                                            // Calculate remaining budget
                                            decimal remainingBudget = budgetSpent.Amount - treatmentFunded.Cost;

                                            // Write budget name and spent amount to the worksheet
                                            generalSummaryWorksheet.Cells[currentRow, currentYearColumn].Value = remainingBudget;
                                            ExcelHelper.ApplyBorder(generalSummaryWorksheet.Cells[currentRow, currentYearColumn, currentRow, currentYearColumn]);

                                            // Increment total yearly spent
                                            totalYearlySpent += remainingBudget;

                                            // Move to the next row
                                            currentRow++;
                                    }
                                    finalRow = currentRow;
                                    currentRow = firstRow + 1;
                                    currentYearColumn++;
                                    ExcelHelper.ApplyBorder(generalSummaryWorksheet.Cells[firstRow - 1, 1, firstRow - 1, finalColumn - 1]);
                                }
                            }
                        }
                    }
                }
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
