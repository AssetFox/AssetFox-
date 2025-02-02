﻿using System;


namespace AppliedResearchAssociates.iAM.ExcelHelpers
{
    public static class ExcelWorksheetContentModels
    {
        public static AutoFitColumnsExcelWorksheetContentModel AutoFitColumns(double minWidth)
            => new AutoFitColumnsExcelWorksheetContentModel
            {
                MinWidth = minWidth,
            };
        public static IExcelWorksheetContentModel SpecificColumnWidthDelta(int columnNumber, Func<double, double> columnWidthChange)
            => new SpecificColumnWidthChangeExcelWorksheetModel
            {
                ColumnNumber = columnNumber,
                WidthChange = columnWidthChange,
            };

        public static IExcelWorksheetContentModel ColumnWidth(int oneBasedColumnIndex, double width)
        {
            var returnValue = new SpecifiedColumnWidthExcelWorksheetModel
            {
                ColumnNumber = oneBasedColumnIndex,
                Width = width,
            };
            return returnValue;
        }
    }
}
