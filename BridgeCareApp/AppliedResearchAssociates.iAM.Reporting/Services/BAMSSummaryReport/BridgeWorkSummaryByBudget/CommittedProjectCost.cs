using System;
using System.Collections.Generic;
using System.Drawing;
using OfficeOpenXml;
using AppliedResearchAssociates.iAM.ExcelHelpers;
using AppliedResearchAssociates.iAM.Reporting.Models.BAMSSummaryReport;
using AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport.BridgeWorkSummary;
using AppliedResearchAssociates.iAM.Reporting.Models;
using System.Linq;
using AppliedResearchAssociates.iAM.DTOs.Abstract;
using AppliedResearchAssociates.iAM.DTOs.Enums;

namespace AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport.BridgeWorkSummaryByBudget
{
    public class CommittedProjectCost
    {
        private readonly BridgeWorkSummaryCommon _bridgeWorkSummaryCommon;

        public CommittedProjectCost()
        {
            _bridgeWorkSummaryCommon = new BridgeWorkSummaryCommon();
        }

        internal void FillCostOfCommittedWork(ExcelWorksheet worksheet,
            CurrentCell currentCell,
            List<int> simulationYears,
            List<YearsData> costForCommittedBudgets,
            Dictionary<int, decimal> totalBudgetPerYearForCommittedWork,
            WorkTypeTotal workTypeTotal)
        {
            var startYear = simulationYears[0];
            currentCell.Row += 1;
            _bridgeWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Cost of Committed Work", "Committed Work Type");
            currentCell.Row += 1;
            var startOfCommittedBudget = currentCell.Row;
            currentCell.Column = 1;
            var treatmentTracker = new Dictionary<string, int>();
            foreach (var item in costForCommittedBudgets)
            {
                if (item.ProjectSource == "Committed")
                {
                    string currentProjectSource = item.ProjectSource;
                    var rowNum = currentCell.Row++;
                    worksheet.Cells[rowNum, currentCell.Column].Value = item.Treatment;
                    worksheet.Cells[rowNum, currentCell.Column + 2, rowNum, currentCell.Column + 1 + simulationYears.Count].Value = 0.0;
                    var cellToEnterCost = item.Year - startYear;
                    var cellValue = worksheet.Cells[rowNum, currentCell.Column + cellToEnterCost + 2].Value;
                    var totalAmount = 0.0;
                    if (cellValue != null)
                    {
                        totalAmount = (double)cellValue;
                    }
                    totalAmount += item.Amount;
                    worksheet.Cells[rowNum, currentCell.Column + cellToEnterCost + 2].Value = totalAmount;
                    WorkTypeTotalHelper.FillWorkTypeTotals(item, workTypeTotal);
                }
            }
            worksheet.Cells[currentCell.Row, currentCell.Column].Value = BAMSConstants.CommittedTotal;

            foreach (var totalCommittedBudget in totalBudgetPerYearForCommittedWork)
            {
                var year = totalCommittedBudget.Key;
                var totalAmount = costForCommittedBudgets
                    .Where(item => item.Year == year && (item.ProjectSource == "Committed"))
                    .Sum(item => item.Amount);

                var cellToEnterTotalBridgeCost = year - startYear;
                worksheet.Cells[currentCell.Row, currentCell.Column + cellToEnterTotalBridgeCost + 2].Value = totalAmount;
            }
            ExcelHelper.ApplyBorder(worksheet.Cells[startOfCommittedBudget, currentCell.Column, currentCell.Row, simulationYears.Count + 2]);
            ExcelHelper.SetCustomFormat(worksheet.Cells[startOfCommittedBudget, currentCell.Column + 2, currentCell.Row, simulationYears.Count + 2], ExcelHelperCellFormat.NegativeCurrency);
            ExcelHelper.ApplyColor(worksheet.Cells[startOfCommittedBudget, currentCell.Column + 2, currentCell.Row, simulationYears.Count + 2], Color.DarkSeaGreen);
            ExcelHelper.ApplyColor(worksheet.Cells[currentCell.Row, currentCell.Column + 2, currentCell.Row, simulationYears.Count + 2], Color.FromArgb(84, 130, 53));
            ExcelHelper.SetTextColor(worksheet.Cells[currentCell.Row, currentCell.Column + 2, currentCell.Row, simulationYears.Count + 2], Color.White);
            currentCell.Row++;
        }

        internal void FillCostOfMPMSWork(ExcelWorksheet worksheet,
            CurrentCell currentCell,
            List<int> simulationYears,
            List<YearsData> costForCommittedBudgets,
            Dictionary<int, decimal> totalBudgetPerYearForCommittedWork,
            WorkTypeTotal workTypeTotal)
        {
            var startYear = simulationYears[0];
            currentCell.Row += 1;
            _bridgeWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Cost of MPMS Work", "MPMS Work Type");
            currentCell.Row += 1;
            var startOfCommittedBudget = currentCell.Row;
            currentCell.Column = 1;
            var treatmentTracker = new Dictionary<string, int>();            
            foreach (var item in costForCommittedBudgets)
            {
                if (item.ProjectSource == "MPMS")
                {
                    string currentProjectSource = item.ProjectSource;
                    var rowNum = currentCell.Row++;
                    worksheet.Cells[rowNum, currentCell.Column].Value = item.Treatment;
                    worksheet.Cells[rowNum, currentCell.Column + 2, rowNum, currentCell.Column + 1 + simulationYears.Count].Value = 0.0;
                    var cellToEnterCost = item.Year - startYear;
                    var cellValue = worksheet.Cells[rowNum, currentCell.Column + cellToEnterCost + 2].Value;
                    var totalAmount = 0.0;
                    if (cellValue != null)
                    {
                        totalAmount = (double)cellValue;
                    }
                    totalAmount += item.Amount;
                    worksheet.Cells[rowNum, currentCell.Column + cellToEnterCost + 2].Value = totalAmount;
                    WorkTypeTotalHelper.FillWorkTypeTotals(item, workTypeTotal);
                }
            }
            worksheet.Cells[currentCell.Row, currentCell.Column].Value = BAMSConstants.CommittedTotal;

            foreach (var totalCommittedBudget in totalBudgetPerYearForCommittedWork)
            {
                var year = totalCommittedBudget.Key;
                var totalAmount = costForCommittedBudgets
                    .Where(item => item.Year == year && (item.ProjectSource == "MPMS"))
                    .Sum(item => item.Amount);

                var cellToEnterTotalBridgeCost = year - startYear;
                worksheet.Cells[currentCell.Row, currentCell.Column + cellToEnterTotalBridgeCost + 2].Value = totalAmount;
            }
            ExcelHelper.ApplyBorder(worksheet.Cells[startOfCommittedBudget, currentCell.Column, currentCell.Row, simulationYears.Count + 2]);
            ExcelHelper.SetCustomFormat(worksheet.Cells[startOfCommittedBudget, currentCell.Column + 2, currentCell.Row, simulationYears.Count + 2], ExcelHelperCellFormat.NegativeCurrency);
            ExcelHelper.ApplyColor(worksheet.Cells[startOfCommittedBudget, currentCell.Column + 2, currentCell.Row, simulationYears.Count + 2], Color.DarkSeaGreen);
            ExcelHelper.ApplyColor(worksheet.Cells[currentCell.Row, currentCell.Column + 2, currentCell.Row, simulationYears.Count + 2], Color.FromArgb(84, 130, 53));
            ExcelHelper.SetTextColor(worksheet.Cells[currentCell.Row, currentCell.Column + 2, currentCell.Row, simulationYears.Count + 2], Color.White);
            currentCell.Row++;
        }

        internal void FillCostOfSAPWork(
            ExcelWorksheet worksheet,
            CurrentCell currentCell,
            List<int> simulationYears,
            List<YearsData> costForSAPBudgets,
            Dictionary<int, decimal> totalBudgetPerYearForSAPWork,
            WorkTypeTotal workTypeTotal)
        {
            var startYear = simulationYears[0];
            currentCell.Row += 1; 
            _bridgeWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Cost of SAP Work", "SAP Work Type");
            currentCell.Row += 1; 
            var startOfSAPBudget = currentCell.Row;
            currentCell.Column = 1;
            var treatmentTracker = new Dictionary<string, int>();
            foreach (var item in costForSAPBudgets)
            {
                if (item.ProjectSource == "SAP")
                {
                    var rowNum = currentCell.Row++;
                    worksheet.Cells[rowNum, currentCell.Column].Value = item.Treatment;
                    worksheet.Cells[rowNum, currentCell.Column + 2, rowNum, currentCell.Column + 1 + simulationYears.Count].Value = 0.0;
                    var cellToEnterCost = item.Year - startYear;
                    var totalAmount = (double)(worksheet.Cells[rowNum, currentCell.Column + cellToEnterCost + 2].Value ?? 0.0);
                    totalAmount += item.Amount;
                    worksheet.Cells[rowNum, currentCell.Column + cellToEnterCost + 2].Value = totalAmount;
                    WorkTypeTotalHelper.FillWorkTypeTotals(item, workTypeTotal);
                }
            }
            worksheet.Cells[currentCell.Row, currentCell.Column].Value = BAMSConstants.SAPTotal;
            foreach (var totalSAPBudget in totalBudgetPerYearForSAPWork)
            {
                var year = totalSAPBudget.Key;
                var totalAmount = costForSAPBudgets
                    .Where(item => item.Year == year && item.ProjectSource == "SAP")
                    .Sum(item => item.Amount);

                var cellToEnterTotalBridgeCost = year - startYear;
                worksheet.Cells[currentCell.Row, currentCell.Column + cellToEnterTotalBridgeCost + 2].Value = totalAmount;
            }
            int startRow = Math.Min(startOfSAPBudget, currentCell.Row);
            int endRow = Math.Max(startOfSAPBudget, currentCell.Row);
            int startCol = Math.Min(currentCell.Column + 2, simulationYears.Count + 2);
            int endCol = Math.Max(currentCell.Column + 2, simulationYears.Count + 2);

            ExcelHelper.ApplyBorder(worksheet.Cells[startOfSAPBudget, currentCell.Column, currentCell.Row, simulationYears.Count + 2]);
            ExcelHelper.SetCustomFormat(worksheet.Cells[startRow, startCol, endRow, endCol], ExcelHelperCellFormat.NegativeCurrency);
            ExcelHelper.ApplyColor(worksheet.Cells[startOfSAPBudget, startCol, currentCell.Row, simulationYears.Count + 2], Color.DarkSeaGreen);
            ExcelHelper.ApplyColor(worksheet.Cells[currentCell.Row, startCol, currentCell.Row, endCol], Color.FromArgb(84, 130, 53));
            ExcelHelper.SetTextColor(worksheet.Cells[currentCell.Row, startCol, currentCell.Row, endCol], Color.White);
            currentCell.Row++;  
        }

        internal void FillCostOfProjectBuilderWork(
            ExcelWorksheet worksheet,
            CurrentCell currentCell,
            List<int> simulationYears,
            List<YearsData> costForProjectBuilderBudgets,
            Dictionary<int, decimal> totalBudgetPerYearForProjectBuilderWork,
            WorkTypeTotal workTypeTotal)
        {
            var startYear = simulationYears[0];
            currentCell.Row += 1;  
            _bridgeWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Cost of Project Builder Work", "Project Builder Work Type");
            currentCell.Row += 1;  
            var startOfProjectBuilderBudget = currentCell.Row; 
            currentCell.Column = 1;
            var treatmentTracker = new Dictionary<string, int>();
            foreach (var item in costForProjectBuilderBudgets)
            {
                if (item.ProjectSource == "ProjectBuilder")
                {
                    var rowNum = currentCell.Row++;
                    worksheet.Cells[rowNum, currentCell.Column].Value = item.Treatment;
                    worksheet.Cells[rowNum, currentCell.Column + 2, rowNum, currentCell.Column + 1 + simulationYears.Count].Value = 0.0;
                    var cellToEnterCost = item.Year - startYear;
                    var totalAmount = (double)(worksheet.Cells[rowNum, currentCell.Column + cellToEnterCost + 2].Value ?? 0.0);
                    totalAmount += item.Amount;
                    worksheet.Cells[rowNum, currentCell.Column + cellToEnterCost + 2].Value = totalAmount;
                    WorkTypeTotalHelper.FillWorkTypeTotals(item, workTypeTotal);
                }
            }
            worksheet.Cells[currentCell.Row, currentCell.Column].Value = BAMSConstants.ProjectBuilderTotal;
            foreach (var totalProjectBuilderBudget in totalBudgetPerYearForProjectBuilderWork)
            {
                var year = totalProjectBuilderBudget.Key;
                var totalAmount = costForProjectBuilderBudgets
                    .Where(item => item.Year == year && item.ProjectSource == "ProjectBuilder")
                    .Sum(item => item.Amount);

                var cellToEnterTotal = year - startYear;
                worksheet.Cells[currentCell.Row, currentCell.Column + cellToEnterTotal + 2].Value = totalAmount;
            }
            ExcelHelper.ApplyBorder(worksheet.Cells[startOfProjectBuilderBudget, currentCell.Column, currentCell.Row, simulationYears.Count + 2]);
            ExcelHelper.SetCustomFormat(worksheet.Cells[startOfProjectBuilderBudget, currentCell.Column + 2, currentCell.Row, simulationYears.Count + 2], ExcelHelperCellFormat.NegativeCurrency);
            ExcelHelper.ApplyColor(worksheet.Cells[startOfProjectBuilderBudget, currentCell.Column + 2, currentCell.Row, simulationYears.Count + 2], Color.DarkSeaGreen);
            ExcelHelper.ApplyColor(worksheet.Cells[currentCell.Row, currentCell.Column + 2, currentCell.Row, simulationYears.Count + 2], Color.FromArgb(84, 130, 53));
            ExcelHelper.SetTextColor(worksheet.Cells[currentCell.Row, currentCell.Column + 2, currentCell.Row, simulationYears.Count + 2], Color.White);
            currentCell.Row++; 
        }

        internal void AddCostOfWorkOutsideScope(WorkTypeTotal workTypeTotal, List<BaseCommittedProjectDTO> committedProjectsForWorkOutsideScope)
        {
            foreach(var committedProjectForWorkOutsideScope in committedProjectsForWorkOutsideScope)
            {
                var yearsData = new YearsData { TreatmentCategory = TreatmentCategory.WorkOutsideScope, Year = committedProjectForWorkOutsideScope.Year, Amount = committedProjectForWorkOutsideScope.Cost };
                WorkTypeTotalHelper.FillWorkTypeTotals(yearsData, workTypeTotal);
            }
        }
    }
}
