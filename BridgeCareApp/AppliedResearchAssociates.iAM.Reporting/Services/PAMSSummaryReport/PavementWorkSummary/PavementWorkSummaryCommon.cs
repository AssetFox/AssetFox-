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
        public struct TreatmentGroup
        {
            public TreatmentGroup(TreatmentGroupCategory category, int groupNumber, string groupDescription, int rangeLow, int rangeHigh)
            {
                Category = category;
                GroupNumber = groupNumber;
                GroupDescription = groupDescription;
                RangeLow = rangeLow;
                RangeHigh = rangeHigh;

            }
            public readonly TreatmentGroupCategory Category;
            public readonly int GroupNumber;
            public readonly string GroupDescription;
            public readonly int RangeLow;
            public readonly int RangeHigh;
        }

        private static TreatmentGroup[] _treatmentGroups = new TreatmentGroup[]
        {
            new TreatmentGroup (TreatmentGroupCategory.Bituminous, 1, "Routine Maintenance", 0, 11),
            new TreatmentGroup (TreatmentGroupCategory.Bituminous, 2, "Seal Coat", 12, 15),
            new TreatmentGroup (TreatmentGroupCategory.Bituminous, 3, "Minor Rehabilitation", 16, 17),
            new TreatmentGroup (TreatmentGroupCategory.Bituminous, 4, "Major Rehabilitation", 18, 22),
            new TreatmentGroup (TreatmentGroupCategory.Bituminous, 5, "Reconstruction", 23, 23),
            new TreatmentGroup (TreatmentGroupCategory.Concrete,1, "Routine Maintenance", 0, 6),
            new TreatmentGroup (TreatmentGroupCategory.Concrete, 2, "CPR", 7, 14),
            new TreatmentGroup (TreatmentGroupCategory.Concrete, 3, "Major Rehabilitation", 15, 26),
            new TreatmentGroup (TreatmentGroupCategory.Concrete, 4, "Reconstruction", 27, 27),
            new TreatmentGroup (TreatmentGroupCategory.Bundled, 1, "Multi Treatments", 0, 27)
        };

        // TODO update the usage of this as required
        public enum TreatmentGroupCategory
        {
            Bituminous = 'h',
            Concrete = 'j',
            Bundled = 'b'
        }


        private static void GetTreatmentCategoryAndNumber(string treatmentName, out TreatmentGroupCategory treatmentCategory, out int treatmentNumber)
        {
            var treatments = treatmentName.Split("+", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

            // Bundled treatments
            if (treatments != null && treatments.Length > 0 && treatments.FirstOrDefault().Contains("Bundle"))
            {
                treatmentCategory = TreatmentGroupCategory.Bundled;
                treatmentNumber = 0; // default, change if required
            }
            else
            {
                //var highestTreatment = treatments.Last(); // this comes as number and then no valid TreatmentGroupCategory and hence TreatmentGroup
                var firstTreatment = treatments?.First();
                treatmentCategory = (TreatmentGroupCategory)firstTreatment.Substring(0, 1).ToLower()[0];
                var highestTreatmentText = treatments.Last();
                var numberText = !int.TryParse(highestTreatmentText, out _) ? firstTreatment.Substring(1) : highestTreatmentText;
                if (!int.TryParse(numberText, out treatmentNumber))
                {
                    treatmentNumber = -1;
                }
            }
        }

        public static TreatmentGroup GetTreatmentGroup(string treatmentName)
        {
            TreatmentGroupCategory treatmentCategory;
            int treatmentNumber;

            GetTreatmentCategoryAndNumber(treatmentName, out treatmentCategory, out treatmentNumber);
            return _treatmentGroups.SingleOrDefault(tg => tg.Category == treatmentCategory && tg.RangeLow <= treatmentNumber && tg.RangeHigh >= treatmentNumber);
        }

        public static List<TreatmentGroup> GetListOfTreatmentGroupForCategory(TreatmentGroupCategory treatmentCategory)
        {
            return _treatmentGroups.Where(tg => treatmentCategory == tg.Category).ToList();
        }


        public static string GetTreatmentGroupString(TreatmentGroupCategory treatmentCategory)
        {
            switch (treatmentCategory)
            {
            case TreatmentGroupCategory.Bituminous: return "Bituminous";
            case TreatmentGroupCategory.Concrete: return "Concrete";
            case TreatmentGroupCategory.Bundled: return "Bundled";
            default: return "Undefined";
            }
        }

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
            List<(string Name, AssetCategories AssetType, TreatmentCategory Category)> simulationTreatments, ref int row, ref int column)
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

        public List<(string Name, AssetCategories AssetType, TreatmentCategory Category)> GetAsphaltTreatments(List<(string Name, AssetCategories AssetType, TreatmentCategory Category)> allTreatments)
        {
            return allTreatments.Where(treatment => treatment.AssetType.ToString().ToLower() == PAMSConstants.Asphalt).ToList();
        }

        public List<(string Name, AssetCategories AssetType, TreatmentCategory Category)> GetConcreteTreatments(List<(string Name, AssetCategories AssetType, TreatmentCategory Category)> allTreatments)
        {
            return allTreatments.Where(treatment => treatment.AssetType.ToString().ToLower() == PAMSConstants.Concrete).ToList();
        }

        public List<(string Name, AssetCategories AssetType, TreatmentCategory Category)> GetNoTreatments(List<(string Name, AssetCategories AssetType, TreatmentCategory Category)> allTreatments)
        {
            return allTreatments.Where(treatment => treatment.Name.ToLower().Equals(PAMSConstants.NoTreatment)).ToList();
        }

        #endregion Private methods
    }
}
