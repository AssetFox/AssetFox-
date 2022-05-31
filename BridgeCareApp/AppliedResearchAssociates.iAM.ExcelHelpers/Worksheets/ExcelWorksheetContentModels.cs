using System;

<<<<<<<< HEAD:BridgeCareApp/AppliedResearchAssociates.iAM.ExcelHelpers/Worksheets/ExcelWorksheetContentModels.cs
namespace AppliedResearchAssociates.iAM.ExcelHelpers
========
namespace BridgeCareCore.Helpers.Excel
>>>>>>>> master:BridgeCareApp/BridgeCareCore/Helpers/Excel/Worksheets/ExcelWorksheetContentModels.cs
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

        internal static IExcelWorksheetContentModel ColumnWidth(int oneBasedColumnIndex, double width)
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
