using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.ExcelHelpers;
using AppliedResearchAssociates.iAM.Reporting.Models;
using OfficeOpenXml;

namespace AppliedResearchAssociates.iAM.Reporting.Services.GeneralSummaryReport.GeneralBudgetSummary
{
    public class GeneralDeficientConditionGoals
    {
        private readonly ReportHelper _reportHelper;
        private readonly IUnitOfWork _unitOfWork;

        public GeneralDeficientConditionGoals(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _reportHelper = new ReportHelper(_unitOfWork);
        }

        public static void Fill(ExcelWorksheet generalSummaryWorksheet, SimulationOutput reportOutputData, IList<DeficientConditionGoalDTO> deficientConditions, CurrentCell currentCell)
        {
            currentCell.Column = 1;
            generalSummaryWorksheet.Cells[currentCell.Row, currentCell.Column].Value = "Deficient Condition Goals";
            int titleEndColumn = currentCell.Column + 1 + reportOutputData.Years.Count;
            ExcelHelper.MergeCells(generalSummaryWorksheet, currentCell.Row, currentCell.Column, currentCell.Row, titleEndColumn, true);
            var titleRange = generalSummaryWorksheet.Cells[currentCell.Row, currentCell.Column, currentCell.Row, titleEndColumn];
            ExcelHelper.ApplyBorder(titleRange);

            currentCell.Row++;
            int startingColumn = currentCell.Column;
            generalSummaryWorksheet.Cells[currentCell.Row, currentCell.Column++].Value = "Attribute";
            generalSummaryWorksheet.Cells[currentCell.Row, currentCell.Column++].Value = "Goal";

            var yearDetails = new List<SimulationYearDetail>();
            foreach (var year in reportOutputData.Years)
            {
                yearDetails.Add(year);
            }

            foreach (var yearDetail in yearDetails)
            {
                generalSummaryWorksheet.Cells[currentCell.Row, currentCell.Column++].Value = yearDetail.Year;
            }

            currentCell.Row++;

            // Check if the list is null or empty
            if (deficientConditions == null || deficientConditions.Count == 0)
            {
                generalSummaryWorksheet.Cells[currentCell.Row, startingColumn].Value = "No Deficient Condition Goals for Scenario";
                int endColumn = currentCell.Column - 1;
                ExcelHelper.MergeCells(generalSummaryWorksheet, currentCell.Row, startingColumn, currentCell.Row, endColumn, false);
                ExcelHelper.HorizontalCenterAlign(generalSummaryWorksheet.Cells[currentCell.Row, startingColumn, currentCell.Row, endColumn]);
                var range = generalSummaryWorksheet.Cells[currentCell.Row, startingColumn, currentCell.Row, endColumn];
                ExcelHelper.ApplyBorder(range);

                return;
            }

            // Populating data for each goal
            var count = 0;
            foreach (var goalDTO in deficientConditions)
            {
                currentCell.Column = startingColumn; // Reset to the first data column for each goal
                generalSummaryWorksheet.Cells[currentCell.Row, currentCell.Column++].Value = goalDTO.Attribute;
                generalSummaryWorksheet.Cells[currentCell.Row, currentCell.Column++].Value = goalDTO.Name;

                foreach (var yearDetail in yearDetails)
                {
                    var goalDetail = yearDetail.DeficientConditionGoals.FirstOrDefault(d => d.GoalName.Equals(goalDTO.Name, StringComparison.OrdinalIgnoreCase));
                    if (goalDetail != null)
                    {
                        // Use ActualDeficientPercentage to fill in the data for this year
                        string formattedPercentage = $"{goalDetail.ActualDeficientPercentage:0.##}%"; // Format string as needed
                        var currentCellRange = generalSummaryWorksheet.Cells[currentCell.Row, currentCell.Column];
                        currentCellRange.Value = formattedPercentage;

                        // Right align the cell
                        ExcelHelper.HorizontalRightAlign(currentCellRange);

                        // Format the cell as percentage 
                        ExcelHelper.SetCustomFormat(currentCellRange, ExcelHelperCellFormat.PercentageDecimal2);
                    }
                    else
                    {
                        generalSummaryWorksheet.Cells[currentCell.Row, currentCell.Column].Value = "N/A"; // Placeholder, adjust as necessary
                    }

                    currentCell.Column++; // Move to the next column for the next year
                }

                currentCell.Row++; // Move to the next row for the next goal
                count++;
            }

            int totalRows = count;
            int endColumnForMerge = startingColumn + 1 + yearDetails.Count;
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
