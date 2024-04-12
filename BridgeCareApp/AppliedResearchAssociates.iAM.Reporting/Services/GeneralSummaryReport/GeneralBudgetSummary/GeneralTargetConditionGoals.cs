using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.Reporting.Models;
using OfficeOpenXml;

namespace AppliedResearchAssociates.iAM.Reporting.Services.GeneralSummaryReport.GeneralBudgetSummary
{
    public class GeneralTargetConditionGoals
    {
        private ReportHelper _reportHelper;
        private readonly IUnitOfWork _unitOfWork;

        public GeneralTargetConditionGoals(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _reportHelper = new ReportHelper(_unitOfWork);
        }

        public static void Fill(ExcelWorksheet generalSummaryWorksheet, SimulationOutput reportOutputData, IList<TargetConditionGoalDTO> targetConditions, CurrentCell currentCell)
        {
            generalSummaryWorksheet.Cells[currentCell.Row, currentCell.Column].Value = "Target Condition Goals";
            currentCell.Row++;
            int startingColumn = currentCell.Column;

            // Set headers for the table
            generalSummaryWorksheet.Cells[currentCell.Row, currentCell.Column++].Value = "Attribute";
            generalSummaryWorksheet.Cells[currentCell.Row, currentCell.Column++].Value = "Goal";

            var yearDetails = new List<SimulationYearDetail>();
            foreach (var year in reportOutputData.Years)
            {
                yearDetails.Add(year);
            }

            // Add year columns based on the simulation output
            foreach (var yearDetail in yearDetails)
            {
                generalSummaryWorksheet.Cells[currentCell.Row, currentCell.Column++].Value = yearDetail.Year;
            }

            currentCell.Row++;

            // Check if the list is null or empty
            if (targetConditions == null || targetConditions.Count == 0)
            {
                generalSummaryWorksheet.Cells[currentCell.Row, startingColumn].Value = "No Target Condition Goals for Scenario";
                generalSummaryWorksheet.Cells[currentCell.Row, startingColumn, currentCell.Row, currentCell.Column - 1].Merge = true; // Optional: merge cells to center the text
                generalSummaryWorksheet.Cells[currentCell.Row, startingColumn].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center; // Center the text horizontally
                return; // Exit the method as there's nothing more to fill
            }

            // Iterate over each TargetConditionGoalDTO to populate the table
            foreach (var goalDTO in targetConditions)
            {
                currentCell.Column = startingColumn; // Reset to the first data column for each goal
                generalSummaryWorksheet.Cells[currentCell.Row, currentCell.Column++].Value = goalDTO.Attribute;
                generalSummaryWorksheet.Cells[currentCell.Row, currentCell.Column++].Value = goalDTO.Target;

                // Iterate over each year and find matching TargetConditionGoalDetail
                foreach (var yearDetail in reportOutputData.Years)
                {
                    var goalDetail = yearDetail.TargetConditionGoals.FirstOrDefault(g => g.GoalName.Equals(goalDTO.Name, StringComparison.OrdinalIgnoreCase) && g.AttributeName.Equals(goalDTO.Attribute, StringComparison.OrdinalIgnoreCase));

                    if (goalDetail != null)
                    {
                        // If the goalDetail is found, use ActualValue to fill in the data for this year
                        generalSummaryWorksheet.Cells[currentCell.Row, currentCell.Column].Value = goalDetail.ActualValue;
                    }
                    else
                    {
                        // If no matching goalDetail is found for this year, use a placeholder
                        generalSummaryWorksheet.Cells[currentCell.Row, currentCell.Column].Value = "N/A";
                    }

                    currentCell.Column++; // Move to the next column for the next year
                }

                currentCell.Row++; // Move to the next row for the next goal
            }
        }
    }
}
