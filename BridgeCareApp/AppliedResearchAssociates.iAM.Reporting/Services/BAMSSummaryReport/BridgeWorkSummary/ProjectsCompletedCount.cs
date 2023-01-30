using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OfficeOpenXml;

using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.Reporting.Models.BAMSSummaryReport;
using AppliedResearchAssociates.iAM.ExcelHelpers;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using AppliedResearchAssociates.iAM.Reporting.Models;

namespace AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport.BridgeWorkSummary
{
    public class ProjectsCompletedCount
    {
        private BridgeWorkSummaryCommon _bridgeWorkSummaryCommon;

        private Dictionary<int, decimal> TotalCompletedCommittedCount = new Dictionary<int, decimal>();
        private HashSet<string> MPMSTreatments = new HashSet<string>();

        private IList<string> _warnings;

        public ProjectsCompletedCount(IList<string> Warnings)
        {
            _bridgeWorkSummaryCommon = new BridgeWorkSummaryCommon();
            _warnings = Warnings;
        }

        public void FillProjectCompletedCountSection(ExcelWorksheet worksheet, CurrentCell currentCell,
            Dictionary<int, Dictionary<string, int>> completedProjectCount, Dictionary<int, Dictionary<string, int>> completedCommittedCount,
            List<int> simulationYears,
            List<(string Name, AssetCategory AssetType, TreatmentCategory Category)> simulationTreatments)
        {
            var projectRowNumberModel = new ProjectRowNumberModel();
            FillMPMSCompletedProjectsCount(worksheet, currentCell, completedCommittedCount, simulationYears, projectRowNumberModel);
            FillCulvertCompletedProjectsCount(worksheet, currentCell, completedProjectCount, simulationYears, projectRowNumberModel, simulationTreatments);
            FillBridgeCompletedProjectsCount(worksheet, currentCell, completedProjectCount, simulationYears, projectRowNumberModel, simulationTreatments);
            FillTotalCompletedProjectCount(worksheet, currentCell, simulationYears, projectRowNumberModel, simulationTreatments);
        }

        private void FillTotalCompletedProjectCount(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, ProjectRowNumberModel projectRowNumberModel,
            List<(string Name, AssetCategory AssetType, TreatmentCategory Category)> simulationTreatments)
        {
            _bridgeWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Number of Bridges and Culverts Comlpeted", "Work Types");
            AddCountOfAllCompletedProjects(worksheet, currentCell, simulationYears, projectRowNumberModel, simulationTreatments);
        }

        #region Private methods

        private void AddCountOfAllCompletedProjects(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears,
            ProjectRowNumberModel projectRowNumberModel,
            List<(string Name, AssetCategory AssetType, TreatmentCategory Category)> simulationTreatments)
        {
            int startRow, startColumn, row, column;
            _bridgeWorkSummaryCommon.SetRowColumns(currentCell, out startRow, out startColumn, out row, out column);

            worksheet.Cells[row++, column].Value = AuditReportConstants.NoTreatmentForWorkSummary;

            simulationTreatments.Remove((AuditReportConstants.CulvertNoTreatment, AssetCategory.Culvert, TreatmentCategory.Other));
            simulationTreatments.Remove((AuditReportConstants.NonCulvertNoTreatment, AssetCategory.Bridge, TreatmentCategory.Other));

            foreach (var item in simulationTreatments)
            {
                worksheet.Cells[row++, column].Value = item.Name;
            }
            foreach (var item in MPMSTreatments)
            {
                worksheet.Cells[row++, column].Value = item;
            }
            worksheet.Cells[row++, column].Value = AuditReportConstants.Total;
            column++;
            var fromColumn = column + 1;

            foreach (var year in simulationYears)
            {
                row = startRow;
                column = ++column;
                var totalCount = 0;

                // Getting count for No Treatment from Culvert and Non-culvert
                var noTreatmentCount = Convert.ToInt32(worksheet.Cells[projectRowNumberModel.TreatmentsCount[AuditReportConstants.CulvertNoTreatment + "_" + year], column].Value);
                noTreatmentCount += Convert.ToInt32(worksheet.Cells[projectRowNumberModel.TreatmentsCount[AuditReportConstants.NonCulvertNoTreatment + "_" + year], column].Value);
                worksheet.Cells[row++, column].Value = noTreatmentCount;
                
                foreach (var treatment in simulationTreatments)
                {
                    var count = 0;
                    count = Convert.ToInt32(worksheet.Cells[projectRowNumberModel.TreatmentsCount[treatment.Name + "_" + year], column].Value);
                    worksheet.Cells[row++, column].Value = count;
                    totalCount += count;
                }
                foreach (var item in MPMSTreatments)
                {
                    if (projectRowNumberModel.TreatmentsCount.ContainsKey(item + "_" + year))
                    {
                        var count = 0;
                        count = Convert.ToInt32(worksheet.Cells[projectRowNumberModel.TreatmentsCount[item + "_" + year], column].Value);
                        worksheet.Cells[row, column].Value = count;
                        totalCount += count;
                    }
                    row++;
                }
                worksheet.Cells[row, column].Value = totalCount;
            }
            ExcelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row, column]);
            ExcelHelper.ApplyColor(worksheet.Cells[startRow, fromColumn, row, column], Color.LightBlue);
            _bridgeWorkSummaryCommon.UpdateCurrentCell(currentCell, row + 3, column);
            ExcelHelper.ApplyColor(worksheet.Cells[row + 2, startColumn, row + 2, column], Color.DimGray);

            // Adding back the two types of No treatments.
            simulationTreatments.Add((AuditReportConstants.CulvertNoTreatment, AssetCategory.Culvert, TreatmentCategory.Other));
            simulationTreatments.Add((AuditReportConstants.NonCulvertNoTreatment, AssetCategory.Bridge, TreatmentCategory.Other));
        }

        private void FillMPMSCompletedProjectsCount(ExcelWorksheet worksheet, CurrentCell currentCell, Dictionary<int, Dictionary<string, int>> completedCommittedCount,
            List<int> simulationYears, ProjectRowNumberModel projectRowNumberModel)
        {
            _bridgeWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Number of MPMS Projects Completed", "MPMS Work Type");
            AddCountOfMPMSCompleted(worksheet, currentCell, completedCommittedCount, simulationYears, projectRowNumberModel);
        }

        private void AddCountOfMPMSCompleted(ExcelWorksheet worksheet, CurrentCell currentCell, Dictionary<int, Dictionary<string, int>> completedCommittedCount,
            List<int> simulationYears, ProjectRowNumberModel projectRowNumberModel)
        {
            var startYear = simulationYears[0];
            _bridgeWorkSummaryCommon.SetRowColumns(currentCell, out var startRow, out var startColumn, out var row, out var column);
            currentCell.Column = column;
            var committedTotalRow = 0;

            var uniqueTreatments = new Dictionary<string, int>();
            // filling in the committed treatments in the excel TAB
            foreach (var yearlyItem in completedCommittedCount)
            {
                decimal committedTotalCount = 0;
                foreach (var data in yearlyItem.Value)
                {
                    MPMSTreatments.Add(data.Key); // Tracking treatment names for MPMS projects
                    if (!uniqueTreatments.ContainsKey(data.Key))
                    {
                        uniqueTreatments.Add(data.Key, currentCell.Row);
                        worksheet.Cells[currentCell.Row, column].Value = data.Key;
                        // setting up the row with zeros
                        worksheet.Cells[currentCell.Row, currentCell.Column + 2, currentCell.Row, currentCell.Column + 1 + simulationYears.Count()].Value = 0;

                        var cellToEnterCount = yearlyItem.Key - startYear;
                        worksheet.Cells[uniqueTreatments[data.Key], column + cellToEnterCount + 2].Value = data.Value;

                        projectRowNumberModel.TreatmentsCount.Add(data.Key + "_" + yearlyItem.Key, currentCell.Row);
                        currentCell.Row += 1;
                    }
                    else
                    {
                        var cellToEnterCost = yearlyItem.Key - startYear;
                        worksheet.Cells[uniqueTreatments[data.Key], column + cellToEnterCost + 2].Value = data.Value;
                        projectRowNumberModel.TreatmentsCount.Add(data.Key + "_" + yearlyItem.Key, uniqueTreatments[data.Key]);
                    }
                    committedTotalCount += data.Value;
                }
                TotalCompletedCommittedCount.Add(yearlyItem.Key, committedTotalCount);
            }
            column = currentCell.Column;
            worksheet.Cells[currentCell.Row, column].Value = AuditReportConstants.Total;
            column++;
            var fromColumn = column + 1;

            foreach (var cost in TotalCompletedCommittedCount)
            {
                worksheet.Cells[currentCell.Row, fromColumn++].Value = cost.Value;
            }
            committedTotalRow = currentCell.Row;
            fromColumn = column + 1;
            var endColumn = simulationYears.Count + 2;

            ExcelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, committedTotalRow, endColumn]);
            ExcelHelper.ApplyColor(worksheet.Cells[startRow, fromColumn, committedTotalRow, endColumn], Color.LightSteelBlue);
            _bridgeWorkSummaryCommon.UpdateCurrentCell(currentCell, committedTotalRow + 1, endColumn);
        }

        private void FillBridgeCompletedProjectsCount(ExcelWorksheet worksheet, CurrentCell currentCell, Dictionary<int, Dictionary<string, int>> completedProjectCount,
            List<int> simulationYears, ProjectRowNumberModel projectRowNumberModel,
            List<(string Name, AssetCategory AssetType, TreatmentCategory Category)> simulationTreatments)
        {
            _bridgeWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Number of Bridges Completed", "Bridge Work Type");
            AddCountOfBridgeCompleted(worksheet, currentCell, completedProjectCount, projectRowNumberModel, simulationTreatments);
        }

        private void AddCountOfBridgeCompleted(ExcelWorksheet worksheet, CurrentCell currentCell, Dictionary<int, Dictionary<string, int>> completedProjectCount,
            ProjectRowNumberModel projectRowNumberModel,
            List<(string Name, AssetCategory AssetType, TreatmentCategory Category)> simulationTreatments)
        {
            int startRow, startColumn, row, column;
            _bridgeWorkSummaryCommon.SetRowColumns(currentCell, out startRow, out startColumn, out row, out column);

            _bridgeWorkSummaryCommon.SetNonCulvertSectionExcelString(worksheet, simulationTreatments, ref row, ref column);
            worksheet.Cells[row++, column].Value = AuditReportConstants.Total;
            column++;
            var fromColumn = column + 1;

            foreach (var yearlyValues in completedProjectCount)
            {
                row = startRow;
                column = ++column;
                double nonCulvertTotalCount = 0;

                foreach (var treatment in simulationTreatments)
                {
                    if (treatment.AssetType == AssetCategory.Bridge || treatment.Name == AuditReportConstants.NonCulvertNoTreatment)
                    {
                        yearlyValues.Value.TryGetValue(treatment.Name, out var nonCulvertCount);
                        worksheet.Cells[row, column].Value = nonCulvertCount;

                        var keyItem = treatment.Name + "_" + yearlyValues.Key;
                        if (projectRowNumberModel.TreatmentsCount.ContainsKey(keyItem) == false)
                        {
                            projectRowNumberModel.TreatmentsCount.Add(keyItem, row);
                        }
                        else
                        {
                            var warningMessage = "Item key '" + keyItem + "' already exists in the list";
                            if (_warnings.Contains(warningMessage) == false) { _warnings.Add(warningMessage); }
                        }

                        row++;

                        //exclude No Treatment from total
                        if (treatment.Name != AuditReportConstants.NonCulvertNoTreatment) { nonCulvertTotalCount += nonCulvertCount; }
                    }
                }
                worksheet.Cells[row, column].Value = nonCulvertTotalCount;
            }
            ExcelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row, column]);
            ExcelHelper.ApplyColor(worksheet.Cells[startRow, fromColumn, row, column], Color.SlateGray);
            _bridgeWorkSummaryCommon.UpdateCurrentCell(currentCell, ++row, column);
        }

        private void FillCulvertCompletedProjectsCount(ExcelWorksheet worksheet, CurrentCell currentCell, Dictionary<int, Dictionary<string, int>> completedProjectCount,
            List<int> simulationYears, ProjectRowNumberModel projectRowNumberModel,
            List<(string Name, AssetCategory AssetType, TreatmentCategory Category)> simulationTreatments)
        {
            _bridgeWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Number of Culverts Completed", "Culvert Work Type");
            AddCountOfCulvertCompleted(worksheet, currentCell, completedProjectCount, projectRowNumberModel, simulationTreatments);
        }

        private void AddCountOfCulvertCompleted(ExcelWorksheet worksheet, CurrentCell currentCell, Dictionary<int, Dictionary<string, int>> completedProjectCount,
            ProjectRowNumberModel projectRowNumberModel,
            List<(string Name, AssetCategory AssetType, TreatmentCategory Category)> simulationTreatments)
        {
            int startRow, startColumn, row, column;
            _bridgeWorkSummaryCommon.SetRowColumns(currentCell, out startRow, out startColumn, out row, out column);

            _bridgeWorkSummaryCommon.SetCulvertSectionExcelString(worksheet, simulationTreatments, ref row, ref column);
            worksheet.Cells[row++, column].Value = AuditReportConstants.Total;
            column++;
            var fromColumn = column + 1;

            foreach (var yearlyValues in completedProjectCount)
            {
                row = startRow;
                column = ++column;
                double culvertTotalCount = 0;

                foreach (var treatment in simulationTreatments)
                {
                    if (treatment.AssetType == AssetCategory.Culvert || treatment.Name == AuditReportConstants.CulvertNoTreatment)
                    {
                        yearlyValues.Value.TryGetValue(treatment.Name, out var culvertCount);
                        worksheet.Cells[row, column].Value = culvertCount;
                        projectRowNumberModel.TreatmentsCount.Add(treatment.Name + "_" + yearlyValues.Key, row);
                        row++;

                        //exclude No Treatment from total
                        if (treatment.Name != AuditReportConstants.CulvertNoTreatment) { culvertTotalCount += culvertCount; }
                    }
                }
                worksheet.Cells[row, column].Value = culvertTotalCount;
            }

            ExcelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row, column]);
            ExcelHelper.ApplyColor(worksheet.Cells[startRow, fromColumn, row, column], Color.LightSteelBlue);
            _bridgeWorkSummaryCommon.UpdateCurrentCell(currentCell, ++row, column);
        }

        #endregion Private methods
    }
}
