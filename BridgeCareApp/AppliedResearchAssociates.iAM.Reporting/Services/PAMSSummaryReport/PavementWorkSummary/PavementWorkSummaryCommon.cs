﻿using System.Collections.Generic;
using System.Linq;
using OfficeOpenXml;
using OfficeOpenXml.Style;

using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.ExcelHelpers;

using AppliedResearchAssociates.iAM.Reporting.Models.PAMSSummaryReport;
using System;
//using AppliedResearchAssociates.iAM.Reporting.Services.PAMSSummaryReport.PavementWorkSummary.StaticContent;

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

        // Treatment Groups for H/Bituminous (Hot Mix Asphalt)
        //1         Routine Maintenance            0,1,2,3,4,5,6,7,8,9,10,11
        //2         Seal Coat                               12,13,14,15
        //3         Minor Rehabilitation               16,17
        //4         Major Rehabilitation               18,19,20,21,22
        //5         Reconstruction                       23 (edited)

        // Treatment Groups for J/Concrete (Jointed Concrete)
        //1         Routine Maintenance            0,1,2,3,4,5,6,
        //           Pavement Preservation:
        //2                     CPR                           7,8,9,10,11,12,13,14
        //3                     Major Rehabilitation   15,16,17,18,19,20,21,22,23,24,25,26
        //4         Reconstruction                       27


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
            new TreatmentGroup (TreatmentGroupCategory.Concrete, 4, "Reconstruction", 27, 27)
        };

        public enum TreatmentGroupCategory
        {
            Bituminous = 'h',
            Concrete = 'j'
        }


        private static void GetTreatmentCategoryAndNumber(string treatmentName, out TreatmentGroupCategory treatmentCategory, out int treatmentNumber)
        {
            var treatments = treatmentName.Split("+", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            var highestTreatment = treatments.Last();
            treatmentCategory = (TreatmentGroupCategory)highestTreatment.Substring(0, 1).ToLower()[0];
            var numberText = highestTreatment.Substring(1);
            if (!int.TryParse(numberText, out treatmentNumber))
            {
                treatmentNumber = -1;
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


        public static string GetTreatmentGroupString(PavementTreatmentHelper.TreatmentGroupCategory treatmentCategory)
        {
            switch (treatmentCategory)
            {
            case PavementTreatmentHelper.TreatmentGroupCategory.Bituminous: return "Bituminous";
            case PavementTreatmentHelper.TreatmentGroupCategory.Concrete: return "Concrete";
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

        public void AddPavementHeaders(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, string sectionName, bool showPrevYearHeader)
        {
            AddMergePavementSectionHeader(worksheet, sectionName, simulationYears.Count + 1, currentCell);
            AddPavementYearsHeaderRow(worksheet, simulationYears, currentCell, showPrevYearHeader);
        }


        internal void SetPavementTreatmentExcelString(ExcelWorksheet worksheet,
            List<string> simulationTreatments, ref int row, ref int column)
        {
            foreach (var item in simulationTreatments)
            {
                //if (item.AssetType == AssetCategory.Culvert || item.Name == BAMSConstants.CulvertNoTreatment)
                //{
                //    if (item.Name == BAMSConstants.CulvertNoTreatment)
                //    {
                //        worksheet.Cells[row++, column].Value = BAMSConstants.NoTreatmentForWorkSummary;
                //    }
                //    else
                //    {
                //        worksheet.Cells[row++, column].Value = item.Name;
                //    }
                //}
                worksheet.Cells[row++, column].Value = item;
            }
        }


        internal void SetPavementTreatmentGroupsExcelString(ExcelWorksheet worksheet,
            List<string> treatmentGroupNames, ref int row, ref int column)
        {
            foreach (var item in treatmentGroupNames)
            {
                //if (item.AssetType == AssetCategory.Culvert || item.Name == BAMSConstants.CulvertNoTreatment)
                //{
                //    if (item.Name == BAMSConstants.CulvertNoTreatment)
                //    {
                //        worksheet.Cells[row++, column].Value = BAMSConstants.NoTreatmentForWorkSummary;
                //    }
                //    else
                //    {
                //        worksheet.Cells[row++, column].Value = item.Name;
                //    }
                //}
                worksheet.Cells[row++, column].Value = item;
            }
        }

        internal void SetPavementTreatmentExcelString(ExcelWorksheet worksheet,
            List<(string Name, AssetCategory AssetType, TreatmentCategory Category)> simulationTreatments, ref int row, ref int column)
        {
            foreach (var item in simulationTreatments)
            {
                //if (item.AssetType == AssetCategory.Culvert || item.Name == BAMSConstants.CulvertNoTreatment)
                //{
                //    if (item.Name == BAMSConstants.CulvertNoTreatment)
                //    {
                //        worksheet.Cells[row++, column].Value = BAMSConstants.NoTreatmentForWorkSummary;
                //    }
                //    else
                //    {
                //        worksheet.Cells[row++, column].Value = item.Name;
                //    }
                //}
                worksheet.Cells[row++, column].Value = item.Name;
            }
        }
        

        public void InitializeLabelCells(ExcelWorksheet worksheet, CurrentCell currentCell, out int startRow, out int startColumn, out int row, out int column)
        {
            SetRowColumns(currentCell, out startRow, out startColumn, out row, out column);
            //worksheet.Cells[row++, column].Value = PAMSConstants.Good;
            //worksheet.Cells[row++, column].Value = PAMSConstants.Fair;
            //worksheet.Cells[row++, column].Value = PAMSConstants.Poor;
            //worksheet.Cells[row++, column++].Value = PAMSConstants.Closed;
            //worksheet.Cells[row - 4, column - 1, row - 1, column - 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
        }

        //public void InitializeBPNLabels(ExcelWorksheet worksheet, CurrentCell currentCell, out int startRow, out int startColumn, out int row, out int column)
        //{
        //    SetRowColumns(currentCell, out startRow, out startColumn, out row, out column);

        //    var bpnNames = EnumExtensions.GetValues<BPNName>();
        //    for (var bpnName = bpnNames[0]; bpnName <= bpnNames.Last(); bpnName++)
        //    {
        //        worksheet.Cells[row++, column].Value = bpnName.ToReportLabel();
        //    }

        //    worksheet.Cells[row - bpnNames.Count, column, row - 1, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
        //    column++;
        //}

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



        public List<(string Name, AssetCategory AssetType, TreatmentCategory Category)> GetAsphaltTreatments(List<(string Name, AssetCategory AssetType, TreatmentCategory Category)> allTreatments)
        {
            return allTreatments.Where(treatment => treatment.Name.ToLower().StartsWith((char)PavementTreatmentHelper.TreatmentGroupCategory.Bituminous)).ToList();
        }

        public List<(string Name, AssetCategory AssetType, TreatmentCategory Category)> GetConcreteTreatments(List<(string Name, AssetCategory AssetType, TreatmentCategory Category)> allTreatments)
        {
            return allTreatments.Where(treatment => treatment.Name.ToLower().StartsWith((char)PavementTreatmentHelper.TreatmentGroupCategory.Concrete)).ToList();
        }

        public List<(string Name, AssetCategory AssetType, TreatmentCategory Category)> GetNoTreatments(List<(string Name, AssetCategory AssetType, TreatmentCategory Category)> allTreatments)
        {
            return allTreatments.Where(treatment => treatment.Name.ToLower().Equals(PAMSConstants.NoTreatment)).ToList();
        }

        #endregion Private methods
    }
}
