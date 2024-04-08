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
    public class GeneralDeficientConditionGoals
    {
        private ReportHelper _reportHelper;
        private readonly IUnitOfWork _unitOfWork;

        public GeneralDeficientConditionGoals(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _reportHelper = new ReportHelper(_unitOfWork);
        }

        public void Fill(ExcelWorksheet generalSummaryWorksheet, SimulationOutput reportOutputData, IList<DeficientConditionGoalDTO> deficientConditions)
        {
            CurrentCell currentCell = new CurrentCell { Row = 2, Column = 1 };
            generalSummaryWorksheet.Cells[currentCell.Row, currentCell.Column].Value = "Deficient Condition Goals";
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

            // Populating data for each goal
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
                        string formattedPercentage = $"{goalDetail.ActualDeficientPercentage:0.##}%";
                        generalSummaryWorksheet.Cells[currentCell.Row, currentCell.Column].Value = formattedPercentage;
                    }
                    else
                    {
                        generalSummaryWorksheet.Cells[currentCell.Row, currentCell.Column].Value = "N/A"; // Placeholder, adjust as necessary
                    }

                    currentCell.Column++; // Move to the next column for the next year
                }

                currentCell.Row++; // Move to the next row for the next goal
            }
        }
    }
}
