﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BridgeCareCore.Services.SummaryReport.Models.Worksheets
{
    public static class ExcelWorksheetContentModels
    {
        public static AutoFitColumnsExcelWorksheetContentModel AutoFitColumns(double minWidth)
            => new AutoFitColumnsExcelWorksheetContentModel
            {
                MinWidth = minWidth,
            };
        internal static IExcelWorksheetContentModel SpecificColumnWidthDelta(int columnNumber, Func<double, double> columnWidthChange)
            => new SpecificColumnWidthChangeExcelWorksheetModel
            {
                ColumnNumber = columnNumber,
                WidthChange = columnWidthChange,
            };

        internal static IExcelWorksheetContentModel ColumnWidth(int oneBasedColumnIndex, double width)
        {
            var r = new SpecifiedColumnWidthExcelWorksheetModel
            {
                ColumnNumber = oneBasedColumnIndex,
                Width = width,
            };
            return r;
        }
    }
}
