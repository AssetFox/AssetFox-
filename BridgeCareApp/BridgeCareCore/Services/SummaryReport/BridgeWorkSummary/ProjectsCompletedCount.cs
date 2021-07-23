﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using BridgeCareCore.Models.SummaryReport;
using OfficeOpenXml;

namespace BridgeCareCore.Services.SummaryReport.BridgeWorkSummary
{
    public class ProjectsCompletedCount
    {
        private readonly BridgeWorkSummaryCommon _bridgeWorkSummaryCommon;
        private Dictionary<int, decimal> TotalCompletedCommittedCount = new Dictionary<int, decimal>();
        private HashSet<string> MPMSTreatments = new HashSet<string>();

        public ProjectsCompletedCount(BridgeWorkSummaryCommon bridgeWorkSummaryCommon)
        {
            _bridgeWorkSummaryCommon = bridgeWorkSummaryCommon;
        }

        public void FillProjectCompletedCountSection(ExcelWorksheet worksheet, CurrentCell currentCell,
            Dictionary<int, Dictionary<string, int>> completedProjectCount, Dictionary<int, Dictionary<string, int>> completedCommittedCount,
            List<int> simulationYears, SortedSet<string> treatments)
        {
            var projectRowNumberModel = new ProjectRowNumberModel();
            FillMPMSCompletedProjectsCount(worksheet, currentCell, completedCommittedCount, simulationYears, projectRowNumberModel);
            FillCulvertCompletedProjectsCount(worksheet, currentCell, completedProjectCount, simulationYears, treatments, projectRowNumberModel);
            FillBridgeCompletedProjectsCount(worksheet, currentCell, completedProjectCount, simulationYears, treatments, projectRowNumberModel);
            FillTotalCompletedProjectCount(worksheet, currentCell, simulationYears, projectRowNumberModel, treatments);
        }

        private void FillTotalCompletedProjectCount(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, ProjectRowNumberModel projectRowNumberModel,
            SortedSet<string> treatments)
        {
            _bridgeWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Number of Bridges and Culverts Comlpeted", "Work Types");
            AddCountOfAllCompletedProjects(worksheet, currentCell, simulationYears, projectRowNumberModel, treatments);
        }

        #region Private methods

        private void AddCountOfAllCompletedProjects(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, ProjectRowNumberModel projectRowNumberModel,
            SortedSet<string> treatments)
        {
            int startRow, startColumn, row, column;
            _bridgeWorkSummaryCommon.SetRowColumns(currentCell, out startRow, out startColumn, out row, out column);

            worksheet.Cells[row++, column].Value = Properties.Resources.NoTreatmentForWorkSummary;
            treatments.Remove(Properties.Resources.NonCulvertNoTreatment);
            treatments.Remove(Properties.Resources.CulvertNoTreatment);

            foreach (var item in treatments)
            {
                worksheet.Cells[row++, column].Value = item;
            }
            foreach (var item in MPMSTreatments)
            {
                worksheet.Cells[row++, column].Value = item;
            }
            worksheet.Cells[row++, column].Value = Properties.Resources.Total;
            column++;
            var fromColumn = column + 1;

            foreach (var year in simulationYears)
            {
                row = startRow;
                column = ++column;
                var totalCount = 0;

                // Getting count for No Treatment from Culvert and Non-culvert
                var noTreatmentCount = Convert.ToInt32(worksheet.Cells[projectRowNumberModel.TreatmentsCount[Properties.Resources.CulvertNoTreatment + "_" + year], column].Value);
                noTreatmentCount += Convert.ToInt32(worksheet.Cells[projectRowNumberModel.TreatmentsCount[Properties.Resources.NonCulvertNoTreatment + "_" + year], column].Value);
                worksheet.Cells[row++, column].Value = noTreatmentCount;
                totalCount += noTreatmentCount;
                foreach (var treatment in treatments)
                {
                    var count = 0;
                    count = Convert.ToInt32(worksheet.Cells[projectRowNumberModel.TreatmentsCount[treatment + "_" + year], column].Value);
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
            treatments.Add(Properties.Resources.NonCulvertNoTreatment);
            treatments.Add(Properties.Resources.CulvertNoTreatment);
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
                        worksheet.Cells[currentCell.Row, currentCell.Column + 2, currentCell.Row, currentCell.Column + 2 + simulationYears.Count()].Value = 0;

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
            worksheet.Cells[currentCell.Row, column].Value = Properties.Resources.Total;
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
            List<int> simulationYears, SortedSet<string> treatments, ProjectRowNumberModel projectRowNumberModel)
        {
            _bridgeWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Number of Bridges Completed", "Bridge Work Type");
            AddCountOfBridgeCompleted(worksheet, currentCell, completedProjectCount, treatments, projectRowNumberModel);
        }

        private void AddCountOfBridgeCompleted(ExcelWorksheet worksheet, CurrentCell currentCell, Dictionary<int, Dictionary<string, int>> completedProjectCount,
            SortedSet<string> treatments, ProjectRowNumberModel projectRowNumberModel)
        {
            int startRow, startColumn, row, column;
            _bridgeWorkSummaryCommon.SetRowColumns(currentCell, out startRow, out startColumn, out row, out column);

            _bridgeWorkSummaryCommon.SetNonCulvertSectionExcelString(worksheet, treatments, ref row, ref column);
            worksheet.Cells[row++, column].Value = Properties.Resources.Total;
            column++;
            var fromColumn = column + 1;

            foreach (var yearlyValues in completedProjectCount)
            {
                row = startRow;
                column = ++column;
                double nonCulvertTotalCount = 0;

                foreach (var treatment in treatments)
                {
                    if (!treatment.Contains("culvert", StringComparison.OrdinalIgnoreCase) && treatment != Properties.Resources.CulvertNoTreatment)
                    {
                        yearlyValues.Value.TryGetValue(treatment, out var nonCulvertCount);
                        worksheet.Cells[row, column].Value = nonCulvertCount;
                        projectRowNumberModel.TreatmentsCount.Add(treatment + "_" + yearlyValues.Key, row);
                        row++;
                        nonCulvertTotalCount += nonCulvertCount;
                    }
                }
                worksheet.Cells[row, column].Value = nonCulvertTotalCount;
            }
            ExcelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row, column]);
            ExcelHelper.ApplyColor(worksheet.Cells[startRow, fromColumn, row, column], Color.SlateGray);
            _bridgeWorkSummaryCommon.UpdateCurrentCell(currentCell, ++row, column);
        }

        private void FillCulvertCompletedProjectsCount(ExcelWorksheet worksheet, CurrentCell currentCell, Dictionary<int, Dictionary<string, int>> completedProjectCount,
            List<int> simulationYears, SortedSet<string> treatments, ProjectRowNumberModel projectRowNumberModel)
        {
            _bridgeWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Number of Culverts Completed", "Culvert Work Type");
            AddCountOfCulvertCompleted(worksheet, currentCell, completedProjectCount, treatments, projectRowNumberModel);
        }

        private void AddCountOfCulvertCompleted(ExcelWorksheet worksheet, CurrentCell currentCell, Dictionary<int, Dictionary<string, int>> completedProjectCount,
            SortedSet<string> treatments, ProjectRowNumberModel projectRowNumberModel)
        {
            int startRow, startColumn, row, column;
            _bridgeWorkSummaryCommon.SetRowColumns(currentCell, out startRow, out startColumn, out row, out column);

            _bridgeWorkSummaryCommon.SetCulvertSectionExcelString(worksheet, treatments, ref row, ref column);
            worksheet.Cells[row++, column].Value = Properties.Resources.Total;
            column++;
            var fromColumn = column + 1;

            foreach (var yearlyValues in completedProjectCount)
            {
                row = startRow;
                column = ++column;
                double culvertTotalCount = 0;

                foreach (var treatment in treatments)
                {
                    if (treatment.Contains("culvert", StringComparison.OrdinalIgnoreCase) || treatment == Properties.Resources.CulvertNoTreatment)
                    {
                        yearlyValues.Value.TryGetValue(treatment, out var culvertCount);
                        worksheet.Cells[row, column].Value = culvertCount;
                        projectRowNumberModel.TreatmentsCount.Add(treatment + "_" + yearlyValues.Key, row);
                        row++;
                        culvertTotalCount += culvertCount;
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
