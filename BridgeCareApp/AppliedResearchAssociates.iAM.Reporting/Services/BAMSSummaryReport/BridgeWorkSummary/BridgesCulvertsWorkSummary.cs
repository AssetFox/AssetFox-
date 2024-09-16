using System;
using System.Collections.Generic;
using System.Drawing;
using OfficeOpenXml;

using AppliedResearchAssociates.iAM.ExcelHelpers;
using AppliedResearchAssociates.iAM.Reporting.Models.BAMSSummaryReport;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using AppliedResearchAssociates.iAM.Reporting.Models;

namespace AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport.BridgeWorkSummary
{
    public class BridgesCulvertsWorkSummary
    {
        private BridgeWorkSummaryCommon _bridgeWorkSummaryCommon;
        private HashSet<string> MPMSTreatments = new HashSet<string>();
        private Dictionary<int, decimal> TotalCompletedCommittedCount = new Dictionary<int, decimal>();

        private IList<string> _warnings;

        public BridgesCulvertsWorkSummary(IList<string> Warnings)
        {
            _bridgeWorkSummaryCommon = new BridgeWorkSummaryCommon();
            _warnings = Warnings;
        }

        public void FillBridgesCulvertsWorkSummarySections(ExcelWorksheet worksheet, CurrentCell currentCell,
            Dictionary<int, Dictionary<string, (decimal treatmentCost, int bridgeCount)>> countPerTreatmentPerYear,
            Dictionary<int, Dictionary<string, List<CommittedProjectMetaData>>> workedOnCommitedProjCount,
            List<int> simulationYears,
            List<(string Name, string AssetType, TreatmentCategory Category)> simulationTreatments)
        {
            var projectRowNumberModel = new ProjectRowNumberModel();
            FillMPMSWorkedOnCount(worksheet, currentCell, workedOnCommitedProjCount, simulationYears, projectRowNumberModel);
            FillNumberOfCulvertsWorkedOnSection(worksheet, currentCell, countPerTreatmentPerYear, simulationYears, projectRowNumberModel, simulationTreatments);
            FillNumberOfBridgesWorkedOnSection(worksheet, currentCell, countPerTreatmentPerYear, simulationYears, projectRowNumberModel, simulationTreatments);
            FillNumberOfBridgesCulvertsWorkedOnSection(worksheet, currentCell, simulationYears, projectRowNumberModel, simulationTreatments);
        }

        #region Private methods

        private void FillMPMSWorkedOnCount(ExcelWorksheet worksheet, CurrentCell currentCell,
            Dictionary<int, Dictionary<string, List<CommittedProjectMetaData>>> workedOnCommitedProjCount, List<int> simulationYears, ProjectRowNumberModel projectRowNumberModel)
        {
            _bridgeWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Number of MPMS Projects Worked On", "MPMS Work Type");
            AddCountOfMPMSCompleted(worksheet, currentCell, workedOnCommitedProjCount, simulationYears, projectRowNumberModel);
        }

        private void FillNumberOfCulvertsWorkedOnSection(ExcelWorksheet worksheet, CurrentCell currentCell,
            Dictionary<int, Dictionary<string, (decimal treatmentCost, int bridgeCount)>> countPerTreatmentPerYear,
            List<int> simulationYears, ProjectRowNumberModel projectRowNumberModel,
            List<(string Name, string AssetType, TreatmentCategory Category)> simulationTreatments)
        {
            _bridgeWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Number of Culverts Worked on", "Culvert Work Type");
            AddCountsOfCulvertsWorkedOn(worksheet, currentCell, countPerTreatmentPerYear, projectRowNumberModel, simulationTreatments);
        }

        private void FillNumberOfBridgesWorkedOnSection(ExcelWorksheet worksheet, CurrentCell currentCell,
            Dictionary<int, Dictionary<string, (decimal treatmentCost, int bridgeCount)>> countPerTreatmentPerYear,
            List<int> simulationYears, ProjectRowNumberModel projectRowNumberModel,
            List<(string Name, string AssetType, TreatmentCategory Category)> simulationTreatments)
        {
            _bridgeWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Number of Bridges Worked on", "Bridge Work Type");
            AddCountsOfBridgesWorkedOn(worksheet, currentCell, countPerTreatmentPerYear, projectRowNumberModel, simulationTreatments);
        }

        private void FillNumberOfBridgesCulvertsWorkedOnSection(ExcelWorksheet worksheet, CurrentCell currentCell,
            List<int> simulationYears, ProjectRowNumberModel projectRowNumberModel,
            List<(string Name, string AssetType, TreatmentCategory Category)> simulationTreatments)
        {
            _bridgeWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Number of Bridges and Culverts Worked on", "Bridge and Culvert Work Types");
            AddDetailsForNumberOfBridgesCulvertsWorkedOn(worksheet, currentCell, simulationYears, projectRowNumberModel, simulationTreatments);
        }

        private void AddCountOfMPMSCompleted(ExcelWorksheet worksheet, CurrentCell currentCell,
            Dictionary<int, Dictionary<string, List<CommittedProjectMetaData>>> workedOnCommitedProjCount, List<int> simulationYears,
            ProjectRowNumberModel projectRowNumberModel)
        {
            var startYear = simulationYears[0];
            _bridgeWorkSummaryCommon.SetRowColumns(currentCell, out var startRow, out var startColumn, out var row, out var column);
            currentCell.Column = column;
            var committedTotalRow = 0;

            var uniqueTreatments = new Dictionary<string, int>();
            // filling in the committed treatments in the excel TAB
            foreach (var yearlyItem in workedOnCommitedProjCount)
            {
                decimal committedTotalCount = 0;
                foreach (var data in yearlyItem.Value)
                {
                    var dataKey = data.Key.Contains("Bundle") ? BAMSConstants.BundledTreatments : data.Key;
                    MPMSTreatments.Add(dataKey); // Tracking treatment names for MPMS projects
                    if (!uniqueTreatments.ContainsKey(dataKey))
                    {
                        uniqueTreatments.Add(dataKey, currentCell.Row);
                        worksheet.Cells[currentCell.Row, column].Value = dataKey;
                        // setting up the row with zeros
                        worksheet.Cells[currentCell.Row, currentCell.Column + 2, currentCell.Row, currentCell.Column + 1 + simulationYears.Count].Value = 0;

                        var cellToEnterCount = yearlyItem.Key - startYear;
                        worksheet.Cells[uniqueTreatments[dataKey], column + cellToEnterCount + 2].Value = data.Value.Count;

                        var keyItem = dataKey + "_" + yearlyItem.Key;
                        if(projectRowNumberModel.TreatmentsCount.ContainsKey(keyItem) == false)
                        {
                            projectRowNumberModel.TreatmentsCount.Add(keyItem, currentCell.Row);
                        }
                        else
                        {
                            var warningMessage = "Item key '" + keyItem + "' already exists in the list";
                            if (_warnings.Contains(warningMessage) == false) { _warnings.Add(warningMessage); }
                        }
                        
                        currentCell.Row += 1;
                    }
                    else
                    {
                        var cellToEnterCost = yearlyItem.Key - startYear;
                        worksheet.Cells[uniqueTreatments[dataKey], column + cellToEnterCost + 2].Value = data.Value.Count;

                        var keyItem = dataKey + "_" + yearlyItem.Key;
                        if (projectRowNumberModel.TreatmentsCount.ContainsKey(keyItem) == false)
                        {
                            projectRowNumberModel.TreatmentsCount.Add(keyItem, uniqueTreatments[dataKey]);
                        }
                        else
                        {
                            var warningMessage = "Item key '" + keyItem + "' already exists in the list";
                            if (_warnings.Contains(warningMessage) == false) { _warnings.Add(warningMessage); }
                        }
                    }
                    committedTotalCount += data.Value.Count;
                }
                TotalCompletedCommittedCount.Add(yearlyItem.Key, committedTotalCount);
            }
            column = currentCell.Column;
            worksheet.Cells[currentCell.Row, column].Value = BAMSConstants.Total;
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

        private void AddCountsOfCulvertsWorkedOn(ExcelWorksheet worksheet, CurrentCell currentCell,
            Dictionary<int, Dictionary<string, (decimal treatmentCost, int bridgeCount)>> countPerTreatmentPerYear,
            ProjectRowNumberModel projectRowNumberModel,
            List<(string Name, string AssetType, TreatmentCategory Category)> simulationTreatments)
        {
            int startRow, startColumn, row, column;
            _bridgeWorkSummaryCommon.SetRowColumns(currentCell, out startRow, out startColumn, out row, out column);

            _bridgeWorkSummaryCommon.SetCulvertSectionExcelString(worksheet, simulationTreatments, ref row, ref column);
            worksheet.Cells[row++, column].Value = BAMSConstants.Total;
            column++;
            var fromColumn = column + 1;
            foreach (var yearlyValues in countPerTreatmentPerYear)
            {
                row = startRow;
                column = ++column;
                double culvertTotalCount = 0; 
                foreach (var treatment in simulationTreatments)
                {
                    if (treatment.AssetType == "Culvert" || treatment.Name == BAMSConstants.CulvertNoTreatment)
                    {
                        yearlyValues.Value.TryGetValue(treatment.Name, out var culvertCostAndCount);
                        worksheet.Cells[row, column].Value = culvertCostAndCount.bridgeCount;

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
                        if (treatment.Name != BAMSConstants.CulvertNoTreatment) { culvertTotalCount += culvertCostAndCount.bridgeCount; }
                    }
                }
                worksheet.Cells[row, column].Value = culvertTotalCount;
            }
            ExcelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row, column]);
            ExcelHelper.ApplyColor(worksheet.Cells[startRow, fromColumn, row, column], Color.LightSteelBlue);
            _bridgeWorkSummaryCommon.UpdateCurrentCell(currentCell, ++row, column);
        }

        private void AddCountsOfBridgesWorkedOn(ExcelWorksheet worksheet, CurrentCell currentCell,
            Dictionary<int, Dictionary<string, (decimal treatmentCost, int bridgeCount)>> countPerTreatmentPerYear,
            ProjectRowNumberModel projectRowNumberModel,
            List<(string Name, string AssetType, TreatmentCategory Category)> simulationTreatments)
        {
            int startRow, startColumn, row, column;
            _bridgeWorkSummaryCommon.SetRowColumns(currentCell, out startRow, out startColumn, out row, out column);

            _bridgeWorkSummaryCommon.SetNonCulvertSectionExcelString(worksheet, simulationTreatments, ref row, ref column);
            worksheet.Cells[row++, column].Value = BAMSConstants.Total;
            column++;
            var fromColumn = column + 1;
            foreach (var yearlyValues in countPerTreatmentPerYear)
            {
                row = startRow;
                column = ++column;
                double nonCulvertTotalCount = 0;

                foreach (var treatment in simulationTreatments)
                {
                    if (treatment.AssetType == "Bridge" && treatment.Name != BAMSConstants.CulvertNoTreatment)
                    {
                        var treatmentName = treatment.Name.Contains("Bundle") ? BAMSConstants.BundledTreatments : treatment.Name;                        
                        yearlyValues.Value.TryGetValue(treatmentName, out var nonCulvertCostAndCount);
                        worksheet.Cells[row, column].Value = nonCulvertCostAndCount.bridgeCount;

                        var keyItem = treatmentName + "_" + yearlyValues.Key;
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
                        if (treatment.Name != BAMSConstants.NonCulvertNoTreatment) { nonCulvertTotalCount += nonCulvertCostAndCount.bridgeCount; }
                    }
                }
                worksheet.Cells[row, column].Value = nonCulvertTotalCount;
            }
            ExcelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row, column]);
            ExcelHelper.ApplyColor(worksheet.Cells[startRow, fromColumn, row, column], Color.SlateGray);
            _bridgeWorkSummaryCommon.UpdateCurrentCell(currentCell, ++row, column);
        }

        private void AddDetailsForNumberOfBridgesCulvertsWorkedOn(ExcelWorksheet worksheet, CurrentCell currentCell,
            List<int> simulationYears, ProjectRowNumberModel projectRowNumberModel,
            List<(string Name, string AssetType, TreatmentCategory Category)> simulationTreatments)
        {
            int startRow, startColumn, row, column;
            _bridgeWorkSummaryCommon.SetRowColumns(currentCell, out startRow, out startColumn, out row, out column);

            worksheet.Cells[row++, column].Value = BAMSConstants.NoTreatmentForWorkSummary;

            simulationTreatments.Remove((BAMSConstants.CulvertNoTreatment, "Culvert", TreatmentCategory.Other));
            simulationTreatments.Remove((BAMSConstants.NonCulvertNoTreatment, "Bridge", TreatmentCategory.Other));

            foreach (var item in simulationTreatments)
            {
                worksheet.Cells[row++, column].Value = item.Name;
            }
            foreach (var item in MPMSTreatments) 
            {
                worksheet.Cells[row++, column].Value = item;
            }
            worksheet.Cells[row++, column].Value = BAMSConstants.Total;
            column++;
            var fromColumn = column + 1;
            foreach (var year in simulationYears)
            {
                row = startRow;
                column = ++column;
                var totalCount = 0;

                // Getting count for No Treatment from Culvert and Non-culvert
                var noTreatmentCount = Convert.ToInt32(worksheet.Cells[projectRowNumberModel.TreatmentsCount[BAMSConstants.CulvertNoTreatment + "_" + year], column].Value);
                noTreatmentCount += Convert.ToInt32(worksheet.Cells[projectRowNumberModel.TreatmentsCount[BAMSConstants.NonCulvertNoTreatment + "_" + year], column].Value);
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
                    else
                    {
                        worksheet.Cells[row, column].Value = 0;
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
            simulationTreatments.Add((BAMSConstants.CulvertNoTreatment, "Culvert", TreatmentCategory.Other));
            simulationTreatments.Add((BAMSConstants.NonCulvertNoTreatment, "Bridge", TreatmentCategory.Other));
        }

        #endregion Private methods
    }
}
