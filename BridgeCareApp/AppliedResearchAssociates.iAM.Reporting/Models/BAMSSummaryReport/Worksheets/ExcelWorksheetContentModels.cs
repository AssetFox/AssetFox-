using System;

using AppliedResearchAssociates.iAM.Reporting.Interfaces.BAMSSummaryReport.Worksheets;

namespace AppliedResearchAssociates.iAM.Reporting.Models.BAMSSummaryReport.Worksheets
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
    }
}
