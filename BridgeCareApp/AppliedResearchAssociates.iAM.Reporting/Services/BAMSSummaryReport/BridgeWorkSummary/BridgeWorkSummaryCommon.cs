using System.Collections.Generic;
using System.Linq;
using OfficeOpenXml;
using OfficeOpenXml.Style;

using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.ExcelHelpers;
using AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport.BridgeWorkSummary.StaticContent;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using AppliedResearchAssociates.iAM.Reporting.Models;

namespace AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport.BridgeWorkSummary
{
    public class BridgeWorkSummaryCommon
    {
        public void AddHeaders(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, string sectionName, string workTypeName)
        {
            AddWorkTypeHeader(worksheet, currentCell, workTypeName);
            AddMergeSectionHeader(worksheet, sectionName, simulationYears.Count, currentCell);
            AddYearsHeaderRow(worksheet, simulationYears, currentCell);
        }

        public void UpdateCurrentCell(CurrentCell currentCell, int row, int column)
        {
            currentCell.Row = row;
            currentCell.Column = column;
        }

        public void SetRowColumns(CurrentCell currentCell, out int startRow, out int startColumn, out int row, out int column)
        {
            startRow = ++currentCell.Row;
            startColumn = 1;
            row = startRow;
            column = startColumn;
        }

        public void AddBridgeHeaders(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, string sectionName, bool showPrevYearHeader)
        {
            AddMergeBridgeSectionHeader(worksheet, sectionName, simulationYears.Count + 1, currentCell);
            AddBridgeYearsHeaderRow(worksheet, simulationYears, currentCell, showPrevYearHeader);
        }

        public void InitializeLabelCells(ExcelWorksheet worksheet, CurrentCell currentCell, out int startRow, out int startColumn, out int row, out int column)
        {
            SetRowColumns(currentCell, out startRow, out startColumn, out row, out column);
            worksheet.Cells[row++, column].Value = BAMSConstants.Good;
            worksheet.Cells[row++, column].Value = BAMSConstants.Fair;
            worksheet.Cells[row++, column].Value = BAMSConstants.Poor;
            worksheet.Cells[row++, column++].Value = BAMSConstants.Closed;
            worksheet.Cells[row - 4, column - 1, row - 1, column - 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
        }

        public void InitializeBPNLabels(ExcelWorksheet worksheet, CurrentCell currentCell, out int startRow, out int startColumn, out int row, out int column)
        {
            SetRowColumns(currentCell, out startRow, out startColumn, out row, out column);

            var bpnNames = EnumExtensions.GetValues<BPNName>();
            for (var bpnName = bpnNames[0]; bpnName <= bpnNames.Last(); bpnName++)
            {
                worksheet.Cells[row++, column].Value = bpnName.ToReportLabel();
            }

            worksheet.Cells[row - bpnNames.Count, column, row - 1, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            column++;
        }

        #region Private methods

        private void AddMergeSectionHeader(ExcelWorksheet worksheet, string headerText, int yearsCount, CurrentCell currentCell)
        {
            var row = currentCell.Row;
            var column = currentCell.Column;
            worksheet.Cells[row, ++column].Value = headerText;
            var cells = worksheet.Cells[row, column];
            ExcelHelper.ApplyStyle(cells);
            ExcelHelper.MergeCells(worksheet, row, column, row, column + yearsCount - 1);
            cells = worksheet.Cells[row, column, row, column + yearsCount - 1];
            ExcelHelper.ApplyBorder(cells);
            ++row;
            UpdateCurrentCell(currentCell, row, column);
        }

        private void AddYearsHeaderRow(ExcelWorksheet worksheet, List<int> simulationYears, CurrentCell currentCell)
        {
            var row = currentCell.Row;
            var column = currentCell.Column;
            foreach (var year in simulationYears)
            {
                worksheet.Cells[row, column].Value = year;
                var cells = worksheet.Cells[row, column];
                ExcelHelper.ApplyStyle(cells);
                ExcelHelper.ApplyBorder(cells);
                column++;
            }
            currentCell.Column = column - 1;
        }

        private void AddWorkTypeHeader(ExcelWorksheet worksheet, CurrentCell currentCell, string workTypeName)
        {
            var WorkTypeHeader = workTypeName;
            var row = currentCell.Row;
            var column = 1;
            worksheet.Cells[++row, column].Value = WorkTypeHeader;
            var cells = worksheet.Cells[row, column, row + 1, column];
            ExcelHelper.ApplyStyle(cells);
            ExcelHelper.ApplyBorder(cells);
            ExcelHelper.MergeCells(worksheet, row, column, row + 1, column);

            // Empty column
            column++;
            UpdateCurrentCell(currentCell, row, column);
        }

        private void AddMergeBridgeSectionHeader(ExcelWorksheet worksheet, string headerText, int mergeColumns, CurrentCell currentCell)
        {
            var row = currentCell.Row + 1;
            var column = 1;
            worksheet.Cells[row, column].Value = headerText;
            var cells = worksheet.Cells[row, column];
            ExcelHelper.ApplyStyle(cells);
            ExcelHelper.MergeCells(worksheet, row, column, row, column + mergeColumns);
            cells = worksheet.Cells[row, column, row, column + mergeColumns];
            ExcelHelper.ApplyBorder(cells);
            ++row;
            UpdateCurrentCell(currentCell, row, column);
        }

        private void AddBridgeYearsHeaderRow(ExcelWorksheet worksheet, List<int> simulationYears, CurrentCell currentCell, bool showPrevYearHeader)
        {
            var row = currentCell.Row;
            var startColumn = currentCell.Column + 1;
            var column = startColumn;
            if (showPrevYearHeader)
            {
                worksheet.Cells[row, column].Value = simulationYears[0] - 1;
            }
            ++column;
            foreach (var year in simulationYears)
            {
                worksheet.Cells[row, column].Value = year;
                column++;
            }
            currentCell.Column = column - 1;
            var cells = worksheet.Cells[row, startColumn, row, currentCell.Column];
            ExcelHelper.ApplyStyle(cells);
            ExcelHelper.ApplyBorder(cells);
        }

        internal void SetNonCulvertSectionExcelString(ExcelWorksheet worksheet,
            List<(string Name, AssetCategory AssetType, TreatmentCategory Category)> simulationTreatments, ref int row, ref int column)
        {
            foreach (var item in simulationTreatments)
            {
                if (item.AssetType == AssetCategory.Bridge || item.Name == BAMSConstants.NonCulvertNoTreatment)
                {
                    if (item.Name == BAMSConstants.NonCulvertNoTreatment)
                    {
                        worksheet.Cells[row++, column].Value = BAMSConstants.NoTreatmentForWorkSummary;
                    }
                    else
                    {
                        worksheet.Cells[row++, column].Value = item.Name;
                    }
                }
            }
        }

        internal void SetCulvertSectionExcelString(ExcelWorksheet worksheet,
            List<(string Name, AssetCategory AssetType, TreatmentCategory Category)> simulationTreatments, ref int row, ref int column)
        {
            foreach (var item in simulationTreatments)
            {
                if (item.AssetType == AssetCategory.Culvert || item.Name == BAMSConstants.CulvertNoTreatment)
                {
                    if (item.Name == BAMSConstants.CulvertNoTreatment)
                    {
                        worksheet.Cells[row++, column].Value = BAMSConstants.NoTreatmentForWorkSummary;
                    }
                    else
                    {
                        worksheet.Cells[row++, column].Value = item.Name;
                    }
                }
            }
        }

        #endregion Private methods
    }
}
