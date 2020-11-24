using System;
using System.Collections.Generic;
using System.Drawing;
using AppliedResearchAssociates.iAM.Analysis;
using BridgeCareCore.Interfaces.SummaryReport;
using BridgeCareCore.Models.SummaryReport;
using OfficeOpenXml;

namespace BridgeCareCore.Services.SummaryReport.BridgeWorkSummary
{
    public class BridgesCulvertsWorkSummary
    {
        private readonly BridgeWorkSummaryCommon _bridgeWorkSummaryCommon;
        private readonly IExcelHelper _excelHelper;

        public BridgesCulvertsWorkSummary(BridgeWorkSummaryCommon bridgeWorkSummaryCommon, IExcelHelper excelHelper)
        {
            _bridgeWorkSummaryCommon = bridgeWorkSummaryCommon;
            _excelHelper = excelHelper;
        }
        public void FillBridgesCulvertsWorkSummarySections(ExcelWorksheet worksheet, CurrentCell currentCell,
            Dictionary<int, Dictionary<string, (double treatmentCost, int bridgeCount)>> countPerTreatmentPerYear,
            List<int> simulationYears, List<string> treatments)
        {
            var projectRowNumberModel = new ProjectRowNumberModel();
            FillNumberOfCulvertsWorkedOnSection(worksheet, currentCell, countPerTreatmentPerYear, simulationYears, projectRowNumberModel, treatments);
            FillNumberOfBridgesWorkedOnSection(worksheet, currentCell, countPerTreatmentPerYear, simulationYears, projectRowNumberModel, treatments);
            FillNumberOfBridgesCulvertsWorkedOnSection(worksheet, currentCell, simulationYears, projectRowNumberModel, treatments);
        }

        #region Private methods
        private void FillNumberOfCulvertsWorkedOnSection(ExcelWorksheet worksheet, CurrentCell currentCell,
            Dictionary<int, Dictionary<string, (double treatmentCost, int bridgeCount)>> countPerTreatmentPerYear,
            List<int> simulationYears, ProjectRowNumberModel projectRowNumberModel, List<string> treatments)
        {
            _bridgeWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Number of Culverts Worked on", "Culvert Work Type");
            AddCountsOfCulvertsWorkedOn(worksheet, currentCell, countPerTreatmentPerYear, projectRowNumberModel, treatments);
        }
        private void FillNumberOfBridgesWorkedOnSection(ExcelWorksheet worksheet, CurrentCell currentCell,
            Dictionary<int, Dictionary<string, (double treatmentCost, int bridgeCount)>> countPerTreatmentPerYear,
            List<int> simulationYears, ProjectRowNumberModel projectRowNumberModel, List<string> treatments)
        {
            _bridgeWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Number of Bridges Worked on", "Bridge Work Type");
            AddCountsOfBridgesWorkedOn(worksheet, currentCell, countPerTreatmentPerYear, projectRowNumberModel, treatments);
        }
        private void FillNumberOfBridgesCulvertsWorkedOnSection(ExcelWorksheet worksheet, CurrentCell currentCell,
            List<int> simulationYears, ProjectRowNumberModel projectRowNumberModel, List<string> treatments)
        {
            _bridgeWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Number of Bridges and Culverts Worked on", "Bridge and Cultvert Work Types");
            AddDetailsForNumberOfBridgesCulvertsWorkedOn(worksheet, currentCell, simulationYears, projectRowNumberModel, treatments);
        }
        private void AddCountsOfCulvertsWorkedOn(ExcelWorksheet worksheet, CurrentCell currentCell,
            Dictionary<int, Dictionary<string, (double treatmentCost, int bridgeCount)>> countPerTreatmentPerYear,
            ProjectRowNumberModel projectRowNumberModel, List<string> treatments)
        {
            int startRow, startColumn, row, column;
            _bridgeWorkSummaryCommon.SetRowColumns(currentCell, out startRow, out startColumn, out row, out column);
            foreach (var item in treatments)
            {
                if (item.Contains("culvert", StringComparison.OrdinalIgnoreCase))
                {
                    worksheet.Cells[row++, column].Value = item;
                }
            }
            worksheet.Cells[row++, column].Value = Properties.Resources.Total;
            column++;
            var fromColumn = column + 1;
            foreach (var yearlyValues in countPerTreatmentPerYear)
            {
                row = startRow;
                column = ++column;
                double culvertTotalCount = 0;

                foreach (var treatment in treatments)
                {
                    if (treatment.Contains("culvert", StringComparison.OrdinalIgnoreCase))
                    {
                        //var culvertCount = bridgeWorkSummaryComputationHelper.CalculateCountByProject(simulationDataModels, year, item);
                        yearlyValues.Value.TryGetValue(treatment, out var culvertCostAndCount);
                        worksheet.Cells[row, column].Value = culvertCostAndCount.bridgeCount;
                        projectRowNumberModel.TreatmentsCount.Add(treatment + "_" + yearlyValues.Key, row);
                        row++;
                        culvertTotalCount += culvertCostAndCount.bridgeCount;
                    }
                }
                worksheet.Cells[row, column].Value = culvertTotalCount;
            }
            _excelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row, column]);
            _excelHelper.ApplyColor(worksheet.Cells[startRow, fromColumn, row, column], Color.LightSteelBlue);
            _bridgeWorkSummaryCommon.UpdateCurrentCell(currentCell, ++row, column);
        }

        private void AddCountsOfBridgesWorkedOn(ExcelWorksheet worksheet, CurrentCell currentCell,
            Dictionary<int, Dictionary<string, (double treatmentCost, int bridgeCount)>> countPerTreatmentPerYear,
            ProjectRowNumberModel projectRowNumberModel, List<string> treatments)
        {
            int startRow, startColumn, row, column;
            _bridgeWorkSummaryCommon.SetRowColumns(currentCell, out startRow, out startColumn, out row, out column);

            foreach (var item in treatments)
            {
                if (!item.Contains("culvert", StringComparison.OrdinalIgnoreCase) &&
                    !item.Contains(Properties.Resources.NoTreatment, StringComparison.OrdinalIgnoreCase))
                {
                    worksheet.Cells[row++, column].Value = item;
                }
            }
            worksheet.Cells[row++, column].Value = Properties.Resources.Total;
            column++;
            var fromColumn = column + 1;
            foreach (var yearlyValues in countPerTreatmentPerYear)
            {
                row = startRow;
                column = ++column;
                double nonCulvertTotalCount = 0;

                foreach (var treatment in treatments)
                {
                    if (!treatment.Contains("culvert", StringComparison.OrdinalIgnoreCase) &&
                        !treatment.Contains(Properties.Resources.NoTreatment, StringComparison.OrdinalIgnoreCase))
                    {
                        yearlyValues.Value.TryGetValue(treatment, out var nonCulvertCostAndCount);
                        worksheet.Cells[row, column].Value = nonCulvertCostAndCount.bridgeCount;
                        projectRowNumberModel.TreatmentsCount.Add(treatment + "_" + yearlyValues.Key, row);
                        row++;
                        nonCulvertTotalCount += nonCulvertCostAndCount.bridgeCount;
                    }
                }
                worksheet.Cells[row, column].Value = nonCulvertTotalCount;
            }
            _excelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row, column]);
            _excelHelper.ApplyColor(worksheet.Cells[startRow, fromColumn, row, column], Color.SlateGray);
            _bridgeWorkSummaryCommon.UpdateCurrentCell(currentCell, ++row, column);
        }

        private void AddDetailsForNumberOfBridgesCulvertsWorkedOn(ExcelWorksheet worksheet, CurrentCell currentCell,
            List<int> simulationYears, ProjectRowNumberModel projectRowNumberModel, List<string> treatments)
        {
            int startRow, startColumn, row, column;
            _bridgeWorkSummaryCommon.SetRowColumns(currentCell, out startRow, out startColumn, out row, out column);
            foreach (var item in treatments)
            {
                if (!item.Contains(Properties.Resources.NoTreatment, StringComparison.OrdinalIgnoreCase))
                {
                    worksheet.Cells[row++, column].Value = item;
                }
            }
            worksheet.Cells[row++, column].Value = Properties.Resources.Total;
            column++;
            var fromColumn = column + 1;
            foreach (var year in simulationYears)
            {
                row = startRow;
                column = ++column;
                var totalCount = 0;

                foreach (var treatment in treatments)
                {
                    var count = 0;
                    if (!treatment.Contains(Properties.Resources.NoTreatment, StringComparison.OrdinalIgnoreCase))
                    {
                        count = Convert.ToInt32(worksheet.Cells[projectRowNumberModel.TreatmentsCount[treatment + "_" + year], column].Value);
                        worksheet.Cells[row++, column].Value = count;
                        totalCount += count;
                    }
                }
                worksheet.Cells[row, column].Value = totalCount;
            }
            _excelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row, column]);
            _excelHelper.ApplyColor(worksheet.Cells[startRow, fromColumn, row, column], Color.LightBlue);
            _bridgeWorkSummaryCommon.UpdateCurrentCell(currentCell, ++row, column);
            _excelHelper.ApplyColor(worksheet.Cells[row + 1, startColumn, row + 1, column], Color.DimGray);
        }
        #endregion
    }
}
