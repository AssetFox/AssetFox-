using System.Collections.Generic;
using System.Linq;
using OfficeOpenXml;
using AppliedResearchAssociates.iAM.ExcelHelpers;
using AppliedResearchAssociates.iAM.Reporting.Models.PAMSSummaryReport;
using System;
using AppliedResearchAssociates.iAM.DTOs.Enums;

namespace AppliedResearchAssociates.iAM.Reporting.Services.PAMSSummaryReport.PavementWorkSummary
{
    public static class PavementTreatmentHelper
    {
        public readonly struct TreatmentGroup
        {
            public TreatmentGroup(TreatmentCategory treatmentCategory, TreatmentGroupCategory category, string groupDescription)
            {
                TreatmentCategory = treatmentCategory;
                GroupCategory = category;
                GroupDescription = groupDescription;

            }
            public readonly TreatmentGroupCategory GroupCategory;
            public readonly string GroupDescription;
            public readonly TreatmentCategory TreatmentCategory;
        }

        private static readonly TreatmentGroup[] _treatmentGroups = new[]
        {
            new TreatmentGroup (TreatmentCategory.Preservation, TreatmentGroupCategory.Bituminous, "Routine Maintenance"),
            new TreatmentGroup (TreatmentCategory.Maintenance, TreatmentGroupCategory.Bituminous, "Routine Maintenance"),
            new TreatmentGroup (TreatmentCategory.Rehabilitation, TreatmentGroupCategory.Bituminous, "Major Rehabilitation"),
            new TreatmentGroup (TreatmentCategory.Reconstruction, TreatmentGroupCategory.Bituminous, "Reconstruction"),
            new TreatmentGroup (TreatmentCategory.Preservation, TreatmentGroupCategory.Concrete, "Preventive Maintenance"),
            new TreatmentGroup (TreatmentCategory.Maintenance, TreatmentGroupCategory.Concrete, "Preventive Maintenance"),
            new TreatmentGroup (TreatmentCategory.Rehabilitation, TreatmentGroupCategory.Concrete, "Major Rehabilitation"),
            new TreatmentGroup (TreatmentCategory.Reconstruction, TreatmentGroupCategory.Concrete, "Reconstruction"),
            new TreatmentGroup (TreatmentCategory.Bundled, TreatmentGroupCategory.Bundled, "Multi Treatments")
        };

        public enum TreatmentGroupCategory
        {
            Bituminous = 'h',
            Concrete = 'j',
            Bundled = 'b',
            Other = 'o'
        }

        private static void GetTreatmentCategoryAndGroup(string treatmentName, out TreatmentGroupCategory groupCategory, out TreatmentCategory treatmentCategory, List<(string Name, string AssetType, TreatmentCategory Category)> simulationTreatments)
        {
            var treatments = treatmentName.Split("+", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

            // Bundled treatments
            if (treatments != null && treatments.Length > 0 && treatments.FirstOrDefault().Contains("Bundle"))
            {
                groupCategory = TreatmentGroupCategory.Bundled;
                treatmentCategory = TreatmentCategory.Bundled;
            }
            else
            {   
                var treatment = simulationTreatments.FirstOrDefault(_ => _.Name.Equals(treatmentName));
                var assetType = treatment.AssetType;
                groupCategory = assetType.ToLower().Equals(PAMSConstants.Asphalt) ?
                                    TreatmentGroupCategory.Bituminous :
                                    (assetType.ToLower().Equals(PAMSConstants.Concrete) ?
                                        TreatmentGroupCategory.Concrete :
                                        TreatmentGroupCategory.Other);
                treatmentCategory = treatment.Category;
            }
        }

        public static TreatmentGroup GetTreatmentGroup(string treatmentName, List<(string Name, string AssetType, TreatmentCategory Category)> simulationTreatments)
        {
            GetTreatmentCategoryAndGroup(treatmentName, out var groupCategory, out var treatmentCategory, simulationTreatments);
            return _treatmentGroups.SingleOrDefault(tg => tg.GroupCategory == groupCategory && tg.TreatmentCategory == treatmentCategory);
        }

        public static List<TreatmentGroup> GetListOfTreatmentGroupForCategory(TreatmentGroupCategory treatmentCategory)
        {
            return _treatmentGroups.Where(tg => treatmentCategory == tg.GroupCategory).ToList();
        }


        public static string GetTreatmentGroupString(TreatmentGroupCategory groupCategory) => groupCategory switch
        {
            TreatmentGroupCategory.Bituminous => "Bituminous",
            TreatmentGroupCategory.Concrete => "Concrete",
            TreatmentGroupCategory.Bundled => "Bundled",
            _ => "Undefined",
        };

    }

    public class PavementWorkSummaryCommon
    {
        public void AddHeaders(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, string sectionName, string workTypeName, string totalHeaderLabel = null)
        {
            AddWorkTypeHeader(worksheet, currentCell, workTypeName);
            AddMergeSectionHeader(worksheet, sectionName, simulationYears.Count, currentCell);
            AddYearsHeaderRow(worksheet, simulationYears, currentCell, totalHeaderLabel);
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

        internal void SetPavementTreatmentExcelString(ExcelWorksheet worksheet,
            List<string> simulationTreatments, ref int row, ref int column)
        {
            foreach (var item in simulationTreatments)
            {
                worksheet.Cells[row++, column].Value = item;
            }
        }

        internal void SetPavementTreatmentGroupsExcelString(ExcelWorksheet worksheet,
            List<string> treatmentGroupNames, ref int row, ref int column)
        {
            foreach (var item in treatmentGroupNames)
            {
                worksheet.Cells[row++, column].Value = item;
            }
        }

        internal void SetPavementTreatmentExcelString(ExcelWorksheet worksheet,
            List<(string Name, string AssetType, TreatmentCategory Category)> simulationTreatments, ref int row, ref int column)
        {
            foreach (var item in simulationTreatments)
            {
                worksheet.Cells[row++, column].Value = item.Name;
            }
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

        private void AddYearsHeaderRow(ExcelWorksheet worksheet, List<int> simulationYears, CurrentCell currentCell, string totalHeaderLabel = null)
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

            if (!string.IsNullOrEmpty(totalHeaderLabel))
            { 
                worksheet.Cells[row, column].Value = totalHeaderLabel;
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

        private void AddMergePavementSectionHeader(ExcelWorksheet worksheet, string headerText, int mergeColumns, CurrentCell currentCell)
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

        private void AddPavementYearsHeaderRow(ExcelWorksheet worksheet, List<int> simulationYears, CurrentCell currentCell, bool showPrevYearHeader)
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

        public List<(string Name, string AssetType, TreatmentCategory Category)> GetAsphaltTreatments(List<(string Name, string AssetType, TreatmentCategory Category)> allTreatments)
        {
            return allTreatments.Where(treatment => treatment.AssetType.ToString().ToLower() == PAMSConstants.Asphalt).ToList();
        }

        public List<(string Name, string AssetType, TreatmentCategory Category)> GetConcreteTreatments(List<(string Name, string AssetType, TreatmentCategory Category)> allTreatments)
        {
            return allTreatments.Where(treatment => treatment.AssetType.ToString().ToLower() == PAMSConstants.Concrete).ToList();
        }

        public List<(string Name, string AssetType, TreatmentCategory Category)> GetNoTreatments(List<(string Name, string AssetType, TreatmentCategory Category)> allTreatments)
        {
            return allTreatments.Where(treatment => treatment.Name.ToLower().Equals(PAMSConstants.NoTreatment)).ToList();
        }

        #endregion Private methods
    }
}
