using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.ExcelHelpers;
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
            currentCell.Column = 1;
            generalSummaryWorksheet.Cells[currentCell.Row, currentCell.Column].Value = "Target Condition Goals";
            int titleEndColumn = currentCell.Column + 1 + reportOutputData.Years.Count;
            ExcelHelper.MergeCells(generalSummaryWorksheet, currentCell.Row, currentCell.Column, currentCell.Row, titleEndColumn, true);
            var titleRange = generalSummaryWorksheet.Cells[currentCell.Row, currentCell.Column, currentCell.Row, titleEndColumn];
            ExcelHelper.ApplyBorder(titleRange);

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
                int endColumn = currentCell.Column - 1;
                ExcelHelper.MergeCells(generalSummaryWorksheet, currentCell.Row, startingColumn, currentCell.Row, endColumn, false);
                ExcelHelper.HorizontalCenterAlign(generalSummaryWorksheet.Cells[currentCell.Row, startingColumn, currentCell.Row, endColumn]);
                var range = generalSummaryWorksheet.Cells[currentCell.Row, startingColumn, currentCell.Row, endColumn];
                ExcelHelper.ApplyBorder(range);

                return;
            }

            // Sort targetConditions by Attribute before populating the worksheet
            //var sortedTargetConditions = targetConditions.OrderBy(tc => tc.Goal).ToList();


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
                        
                        string formattedPercentage = $"{goalDetail.ActualValue:0.##}%"; // Format string as needed
                        var currentCellRange = generalSummaryWorksheet.Cells[currentCell.Row, currentCell.Column];
                        currentCellRange.Value = formattedPercentage;

                        // Right align the cell
                        ExcelHelper.HorizontalRightAlign(currentCellRange);

                        // Format the cell as percentage 
                        ExcelHelper.SetCustomFormat(currentCellRange, ExcelHelperCellFormat.PercentageDecimal2);
                    }
                    else
                    {
                        // If no matching goalDetail is found for this year, use a placeholder
                        var currentCellRange = generalSummaryWorksheet.Cells[currentCell.Row, currentCell.Column];
                        currentCellRange.Value = "N/A";

                        // Right align the cell
                        ExcelHelper.HorizontalRightAlign(currentCellRange);
                    }

                    currentCell.Column++; // Move to the next column for the next year
                }

                currentCell.Row++; // Move to the next row for the next goal
            }

            int totalRows = currentCell.Row - 1;
            int endColumnForMerge = startingColumn + 1 + yearDetails.Count; // As calculated earlier
            var usedRange = generalSummaryWorksheet.Cells[1, startingColumn, totalRows, endColumnForMerge];
            ExcelHelper.ApplyBorder(usedRange);

            // After all data is populated
            for (int col = startingColumn; col <= endColumnForMerge; col++)
            {
                generalSummaryWorksheet.Column(col).AutoFit();  // Auto adjust column width
            }
        }
    }
}
