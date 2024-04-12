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
            generalSummaryWorksheet.Cells[currentCell.Row, currentCell.Column].Value = "Target Budgets";
            currentCell.Row++;
            int startingColumn = currentCell.Column;

            int currentRow = currentCell.Row + 1;

            foreach (var year in simulationOutput.Years)
            {
                // Write year as column header
                generalSummaryWorksheet.Cells[currentCell.Row, startingColumn + 1].Value = year.Year;

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
                            if (treatmentConsideration.FundingCalculationInput != null)
                            {
                                if (treatmentConsideration.FundingCalculationInput.CurrentBudgetsToSpend != null)
                                {
                                    // Iterate over the budget spent data
                                    foreach (var fundedTreatment in treatmentConsideration.FundingCalculationInput.TreatmentsToFund)
                                    {
                                        // Add treatment cost and name to worksheet
                                        generalSummaryWorksheet.Cells[currentRow, currentCell.Column].Value = fundedTreatment.Name;
                                        generalSummaryWorksheet.Cells[currentRow, currentCell.Column + 1].Value = fundedTreatment.Cost;

                                        // Increment total yearly spent
                                        totalYearlySpent += fundedTreatment.Cost;

                                        // Move to the next row
                                        currentRow++;
                                    }
                                }

                            }
                        }
                    }
                }

                // Write total yearly spent at the bottom of the column
                generalSummaryWorksheet.Cells[currentRow + 1, startingColumn].Value = "Total Target Budgets";
                generalSummaryWorksheet.Cells[currentRow + 1, startingColumn + 1].Value = totalYearlySpent;

                // Reset currentRow for the next column
                currentRow = currentCell.Row + 1;

                // Move to the next column for the next year
                startingColumn++;
            }
        }
        public void FillBudgetSpent(ExcelWorksheet generalSummaryWorksheet, SimulationOutput simulationOutput)
        {
            CurrentCell currentCell = new CurrentCell { Row = 1, Column = 1 };
            generalSummaryWorksheet.Cells[currentCell.Row, currentCell.Column].Value = "Target Budgets";
            currentCell.Row++;
            int startingColumn = currentCell.Column;

            int currentRow = currentCell.Row + 1;

            foreach (var year in simulationOutput.Years)
            {
                // Write year as column header
                generalSummaryWorksheet.Cells[currentRow, startingColumn + 1].Value = year.Year;

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
                            if (treatmentConsideration.FundingCalculationInput != null)
                            {
                                if (treatmentConsideration.FundingCalculationInput.CurrentBudgetsToSpend != null)
                                {
                                    // Iterate over the budget spent data
                                    foreach (var budgetSpent in treatmentConsideration.FundingCalculationInput.CurrentBudgetsToSpend)
                                    {
                                        // Write budget name and spent amount to the worksheet
                                        generalSummaryWorksheet.Cells[currentRow, currentCell.Column].Value = budgetSpent.Name;
                                        generalSummaryWorksheet.Cells[currentRow, currentCell.Column + 1].Value = budgetSpent.Amount;

                                        // Increment total yearly spent
                                        totalYearlySpent += budgetSpent.Amount;

                                        // Move to the next row
                                        currentRow++;
                                    }
                                }

                            }
                        }
                    }
                }

                // Write total yearly spent at the bottom of the column
                generalSummaryWorksheet.Cells[currentRow, startingColumn].Value = "Total Budget Spent";
                generalSummaryWorksheet.Cells[currentRow, startingColumn + 1].Value = totalYearlySpent;

                // Reset currentRow for the next column
                currentRow = currentRow + 1;

                // Move to the next column for the next year
                startingColumn++;
            }
        }

        public void FillBudgetRemaining(ExcelWorksheet generalSummaryWorksheet, SimulationOutput simulationOutput)
        {
            CurrentCell currentCell = new CurrentCell { Row = 1, Column = 1 };
            generalSummaryWorksheet.Cells[currentCell.Row, currentCell.Column].Value = "Budget Remaining";
            currentCell.Row++;
            int startingColumn = currentCell.Column;

            int currentRow = currentCell.Row + 1;

            foreach (var year in simulationOutput.Years)
            {
                // Write year as column header
                generalSummaryWorksheet.Cells[currentRow, startingColumn + 1].Value = year.Year;

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
                                        // Ensure the index is valid for treatments funded
                                        if (i < treatmentsFunded.Count)
                                        {
                                            var budgetSpent = budgetsSpent[i];
                                            var treatmentFunded = treatmentsFunded[i];

                                            // Calculate remaining budget
                                            decimal remainingBudget = budgetSpent.Amount - treatmentFunded.Cost;

                                            generalSummaryWorksheet.Cells[currentRow, currentCell.Column].Value = budgetSpent.Name;
                                            generalSummaryWorksheet.Cells[currentRow, currentCell.Column + 1].Value = remainingBudget;

                                            // Increment total yearly spent
                                            totalYearlySpent += remainingBudget;

                                            // Move to the next row
                                            currentRow++;
                                        }
                                    }

                                }
                            }
                        }
                    }
                }
                // Write total yearly spent at the bottom of the column
                generalSummaryWorksheet.Cells[currentRow, startingColumn].Value = "Total Budget Remaining";
                generalSummaryWorksheet.Cells[currentRow, startingColumn + 1].Value = totalYearlySpent;

                // Reset currentRow for the next column
                currentRow = currentRow + 1;

                // Move to the next column for the next year
                startingColumn++;

            }
        }

    }
    }
